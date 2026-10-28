using System.Text.Json;
using System.Threading.RateLimiting;
using Dapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using MySqlConnector;
using VanderCake.Api.Hubs;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Services;

var builder = WebApplication.CreateBuilder(args.Where(a => a != "--criar-admin").ToArray());
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
builder.Services.AddSingleton<Banco>();
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<CadastroService>();
builder.Services.AddScoped<LeituraService>();
builder.Services.AddScoped<RateioService>();
builder.Services.AddScoped<PedidosService>();
builder.Services.AddScoped<PagamentosService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<EventosService>();
var pastaChaves = builder.Configuration["Seguranca:PastaChaves"];
if (string.IsNullOrWhiteSpace(pastaChaves))
    pastaChaves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VanderCake", "ApiKeys");
Directory.CreateDirectory(pastaChaves);
var protecao = builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(pastaChaves)).SetApplicationName("VanderCake.Api.v1");
if (OperatingSystem.IsWindows()) protecao.ProtectKeysWithDpapi();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme).AddBearerToken(o =>
{
    o.BearerTokenExpiration = TimeSpan.FromHours(builder.Configuration.GetValue("Seguranca:HorasAcesso", 8));
    o.RefreshTokenExpiration = TimeSpan.FromDays(builder.Configuration.GetValue("Seguranca:DiasRenovacao", 30));
});
builder.Services.AddAuthorization();
var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(origens).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddRateLimiter(o =>
{
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(ctx.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 30, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
    o.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        await ctx.HttpContext.Response.WriteAsJsonAsync(new { mensagem = "Muitas tentativas. Aguarde um minuto e tente novamente." }, ct);
    };
});

var app = builder.Build();
if (args.Contains("--criar-admin"))
{
    using var escopo = app.Services.CreateScope();
    try { await Inicializacao.CriarAdministradorAsync(escopo.ServiceProvider); }
    catch (Exception ex) when (ex is ApiException or FormatException or MySqlException)
    {
        Console.Error.WriteLine(ex is MySqlException ? "Confira a conexão, as tabelas e a cidade informada. Nenhuma conta existente foi alterada." : ex.Message);
        Environment.ExitCode = 1;
    }
    return;
}

app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (Exception ex) when (!ctx.Response.HasStarted)
    {
        int status = 500;
        string mensagem = "Não foi possível concluir a operação. Tente novamente.";
        bool loteNaoGravado = false;
        if (ex is ApiException regra) { status = regra.Status; mensagem = regra.Message; loteNaoGravado = regra.LoteNaoGravado; }
        else if (ex is MySqlException sql)
        {
            (status, mensagem) = sql.Number switch
            {
                1062 => (409, "Já existe um registro com esses dados. Atualize a lista antes de repetir."),
                1451 => (409, "Este registro está vinculado a outros dados e não pode ser excluído."),
                1452 => (422, "Um dos registros vinculados não existe. Atualize os cadastros."),
                1205 or 1213 => (409, "Outra operação alterou os mesmos dados. Atualize e tente novamente."),
                3819 or 1406 or 1048 or 1264 => (422, "Um dos valores não atende às regras do cadastro."),
                _ => (503, "O banco não está disponível ou sua estrutura precisa ser verificada.")
            };
        }
        if (status >= 500) app.Logger.LogError("Falha na operação {Metodo} {Caminho}. Tipo: {Tipo}", ctx.Request.Method, ctx.Request.Path, ex.GetType().Name);
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(new { mensagem, loteNaoGravado });
    }
});
app.UseRouting();
app.UseCors();
app.UseStatusCodePages(async estado =>
{
    var ctx = estado.HttpContext;
    if (!ctx.Request.Path.StartsWithSegments("/api")) return;
    var mensagem = ctx.Response.StatusCode switch
    {
        401 => "Entre novamente para continuar.",
        403 => "Seu perfil não permite esta operação.",
        404 => "Recurso não encontrado.",
        _ => "A operação não pôde ser concluída."
    };
    await ctx.Response.WriteAsJsonAsync(new { mensagem });
});
// O transporte WebSocket/SSE usa o token na consulta. Nunca registrar a query string.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/hubs/atualizacoes") && ctx.Request.Query.TryGetValue("access_token", out var token))
        ctx.Request.Headers.Authorization = $"Bearer {token}";
    await next();
});
app.UseAuthentication();
app.Use(async (ctx, next) =>
{
    if (ctx.User.Identity?.IsAuthenticated == true &&
        !await ctx.RequestServices.GetRequiredService<AutenticacaoService>().VigenteAsync(ctx.User))
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsJsonAsync(new { mensagem = "Conta inativa ou sessão revogada. Entre novamente." });
        return;
    }
    await next();
});
app.UseAuthorization();
app.UseRateLimiter();
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.XContentTypeOptions = "nosniff";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    if (ctx.Request.Path.StartsWithSegments("/api")) ctx.Response.Headers.CacheControl = "no-store";
    await next();
});
var arquivosWeb = Path.Combine(AppContext.BaseDirectory, "wwwroot");
if (Directory.Exists(arquivosWeb))
{
    var provedor = new PhysicalFileProvider(arquivosWeb);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = provedor });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = provedor });
}
app.MapControllers();
app.MapHub<AtualizacoesHub>("/hubs/atualizacoes", o => o.CloseOnAuthenticationExpiration = true);
app.MapGet("/api/saude", async (Banco banco) =>
{
    await using var c = await banco.AbrirAsync();
    var versao = await c.ExecuteScalarAsync<int>("SELECT MAX(Versao) FROM VersoesBanco");
    Regras.Exigir(versao == 1, "A versão do banco não é compatível com esta API.", 503);
    return Results.Ok(new { status = "Disponível", versaoBanco = versao });
});
await app.RunAsync();
