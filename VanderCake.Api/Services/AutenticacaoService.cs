using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class AutenticacaoService(Banco banco, IPasswordHasher<object> senhas,
    IOptionsMonitor<BearerTokenOptions> opcoes)
{
    private static string Tabela(string perfil) => perfil switch
    {
        "Administrador" => "Administradores",
        "Vendedor" => "Vendedores",
        _ => throw new ApiException(401, "Sessão inválida.")
    };

    public async Task<object> EntrarAsync(string perfil, string? email, string? senha)
    {
        email = Regras.Texto(email, "E-mail", 150).ToLowerInvariant();
        Regras.Exigir(senha is { Length: > 0 and <= 128 }, "E-mail ou senha inválidos.", 401);
        await using var c = await banco.AbrirAsync();
        var conta = await c.QuerySingleOrDefaultAsync<Vendedor>($"SELECT * FROM {Tabela(perfil)} WHERE Email=@email", new { email });
        Regras.Exigir(conta is not null && conta.Ativo && senhas.VerifyHashedPassword(new object(), conta.SenhaHash, senha!) != PasswordVerificationResult.Failed,
            "E-mail ou senha inválidos, ou conta inativa.", 401);
        return Emitir(conta!, perfil);
    }

    public async Task<bool> VigenteAsync(ClaimsPrincipal principal)
    {
        var perfil = principal.FindFirstValue(ClaimTypes.Role);
        if (perfil is not ("Administrador" or "Vendedor") ||
            !int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ||
            !int.TryParse(principal.FindFirstValue("versaoSeguranca"), out int versao)) return false;
        await using var c = await banco.AbrirAsync();
        return await c.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Tabela(perfil)} WHERE Id=@id AND Ativo=1 AND VersaoSeguranca=@versao", new { id, versao }) == 1;
    }

    public async Task<object> RenovarAsync(string refreshToken)
    {
        Regras.Exigir(!string.IsNullOrWhiteSpace(refreshToken) && refreshToken.Length <= 10000, "Sessão inválida.", 401);
        AuthenticationTicket? ticket = null;
        try { ticket = opcoes.Get(BearerTokenDefaults.AuthenticationScheme).RefreshTokenProtector.Unprotect(refreshToken); }
        catch (Exception ex) when (ex is System.Security.Cryptography.CryptographicException or ArgumentException or FormatException) { }
        Regras.Exigir(ticket is not null && ticket.Properties.ExpiresUtc > DateTimeOffset.UtcNow && await VigenteAsync(ticket.Principal),
            "A sessão não pode ser renovada. Entre novamente.", 401);
        string perfil = ticket!.Principal.FindFirstValue(ClaimTypes.Role)!;
        int id = int.Parse(ticket.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await using var c = await banco.AbrirAsync();
        var conta = await c.QuerySingleAsync<Vendedor>($"SELECT * FROM {Tabela(perfil)} WHERE Id=@id", new { id });
        return Emitir(conta, perfil);
    }

    private object Emitir(Vendedor conta, string perfil)
    {
        var config = opcoes.Get(BearerTokenDefaults.AuthenticationScheme);
        var agora = DateTimeOffset.UtcNow;
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, conta.Id.ToString()),
            new Claim(ClaimTypes.Name, conta.Nome),
            new Claim(ClaimTypes.Role, perfil),
            new Claim("versaoSeguranca", conta.VersaoSeguranca.ToString())
        }, BearerTokenDefaults.AuthenticationScheme));
        var acesso = new AuthenticationTicket(principal, new AuthenticationProperties { ExpiresUtc = agora + config.BearerTokenExpiration }, BearerTokenDefaults.AuthenticationScheme);
        var renovacao = new AuthenticationTicket(principal, new AuthenticationProperties { ExpiresUtc = agora + config.RefreshTokenExpiration }, BearerTokenDefaults.AuthenticationScheme);
        string accessToken = config.BearerTokenProtector.Protect(acesso);
        string refreshToken = config.RefreshTokenProtector.Protect(renovacao);
        var expiresIn = (int)config.BearerTokenExpiration.TotalSeconds;
        var pessoa = new { conta.Id, conta.Nome, conta.Email, conta.Ativo };
        if (perfil == "Administrador") return new { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresIn = expiresIn, Administrador = pessoa };
        return new { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresIn = expiresIn, Vendedor = pessoa };
    }
}
