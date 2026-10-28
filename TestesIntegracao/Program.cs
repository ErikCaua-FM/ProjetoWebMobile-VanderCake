using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using MySqlConnector;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Services;

// Cria um banco exclusivo e inicia outra API, sem tocar no banco vandercake da loja.
var conexao = Environment.GetEnvironmentVariable("VANDERCAKE_TEST_MYSQL");
if (string.IsNullOrWhiteSpace(conexao))
{
    Console.WriteLine("Defina VANDERCAKE_TEST_MYSQL com uma conexão administrativa de TESTES, sem Database.");
    return;
}
var raiz = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
var nomeBanco = "vandercake_teste_" + Guid.NewGuid().ToString("N");
var cs = new MySqlConnectionStringBuilder(conexao) { Database = "", AllowUserVariables = true, DateTimeKind = MySqlDateTimeKind.Utc };
await using var db = new MySqlConnection(cs.ConnectionString);
await db.OpenAsync();
foreach (var arquivo in new[] { "01-estrutura.sql", "02-dados-iniciais.sql" })
    await db.ExecuteAsync((await File.ReadAllTextAsync(Path.Combine(raiz, "Banco", arquivo))).Replace("vandercake", nomeBanco));
await db.ChangeDatabaseAsync(nomeBanco);
cs.Database = nomeBanco;
int testes = 0;
void Ok(bool condicao, string descricao)
{
    if (!condicao) throw new Exception("FALHOU: " + descricao);
    testes++;
    Console.WriteLine("OK: " + descricao);
}
Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=DATABASE()") == 21,
    "Os scripts criam as 21 tabelas no MySQL real");
Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ParcelasCondicaoPagamento") == 6, "Condições de pagamento com 1, 2 e 3 parcelas");
string senha = "Teste-integracao-2026!";
var hash = new PasswordHasher<object>().HashPassword(new object(), senha);
await db.ExecuteAsync("""
    INSERT INTO Administradores (Nome,Documento,Rua,Bairro,Cep,Numero,CidadeId,Telefone,Email,SenhaHash)
    VALUES ('Segundo administrador','11111111112','Rua Teste','Centro','85800000','2',1,'45999999998','admin2@teste.local',@hash)
    """, new { hash });
string usuarioApi = "vc_teste_" + Guid.NewGuid().ToString("N")[..20];
string senhaBancoApi = "Teste;\"'\\Á-" + Guid.NewGuid().ToString("N");
string senhaSql = "'" + senhaBancoApi.Replace("\\", "\\\\").Replace("'", "''") + "'";
var sqlUsuario = (await File.ReadAllTextAsync(Path.Combine(raiz, "Banco/03-usuario-api.sql")))
    .Replace("'vandercake_app'", $"'{usuarioApi}'").Replace("vandercake.*", nomeBanco + ".*")
    .Replace("{{SENHA_ESCAPADA}}", senhaSql);
await db.ExecuteAsync(sqlUsuario);
var csApi = new MySqlConnectionStringBuilder(cs.ConnectionString) { UserID = usuarioApi, Password = senhaBancoApi };
await using (var restrita = new MySqlConnection(csApi.ConnectionString))
{
    await restrita.OpenAsync();
    try { await restrita.ExecuteAsync("ALTER TABLE VersoesBanco ADD ColunaProibida INT"); Ok(false, "Conta tecnica nao pode alterar esquema"); }
    catch (MySqlException e) when (e.Number == 1142) { Ok(true, "Conta tecnica conecta mas nao pode alterar a estrutura do banco"); }
}
var bootstrapInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
{
    WorkingDirectory = Path.Combine(raiz, "VanderCake.Api"), RedirectStandardInput = true,
    RedirectStandardOutput = true, RedirectStandardError = true
};
bootstrapInfo.ArgumentList.Add(Path.Combine(raiz, "VanderCake.Api/bin/Debug/net10.0/VanderCake.Api.dll"));
bootstrapInfo.ArgumentList.Add("--criar-admin");
bootstrapInfo.Environment["ConnectionStrings__VanderCake"] = csApi.ConnectionString;
bootstrapInfo.Environment["Seguranca__PastaChaves"] = Path.Combine(Path.GetTempPath(), nomeBanco + "-chaves");
using (var bootstrap = Process.Start(bootstrapInfo)!)
{
    var saida = bootstrap.StandardOutput.ReadToEndAsync(); var erro = bootstrap.StandardError.ReadToEndAsync();
    await bootstrap.StandardInput.WriteAsync(string.Join(Environment.NewLine, new[]
    {
        "Administrador de teste", "11111111111", "Rua Teste", "1", "Centro", "85800000", "1",
        "45999999999", "admin@teste.local", senha, senha, ""
    }));
    bootstrap.StandardInput.Close(); await bootstrap.WaitForExitAsync();
    Ok(bootstrap.ExitCode == 0 && await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Administradores WHERE Email='admin@teste.local'") == 1,
        "Comando --criar-admin cadastra o administrador usando a conta tecnica");
    if (bootstrap.ExitCode != 0) Console.Error.WriteLine(await saida + await erro);
}
var tcp = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
tcp.Start(); int porta = ((IPEndPoint)tcp.LocalEndpoint).Port; tcp.Stop();
var url = $"http://127.0.0.1:{porta}";
var processo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
{
    WorkingDirectory = Path.Combine(raiz, "VanderCake.Api"), RedirectStandardOutput = true, RedirectStandardError = true
};
processo.ArgumentList.Add(Path.Combine(raiz, "VanderCake.Api/bin/Debug/net10.0/VanderCake.Api.dll"));
processo.ArgumentList.Add("--urls"); processo.ArgumentList.Add(url);
processo.Environment["ConnectionStrings__VanderCake"] = csApi.ConnectionString;
processo.Environment["Seguranca__PastaChaves"] = Path.Combine(Path.GetTempPath(), nomeBanco + "-chaves");
processo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
using var apiProcesso = Process.Start(processo)!;
var logs = new ConcurrentQueue<string>();
apiProcesso.OutputDataReceived += (_, e) => { if (e.Data is not null) logs.Enqueue(e.Data); };
apiProcesso.ErrorDataReceived += (_, e) => { if (e.Data is not null) logs.Enqueue(e.Data); };
apiProcesso.BeginOutputReadLine(); apiProcesso.BeginErrorReadLine();
using var http = new HttpClient { BaseAddress = new Uri(url + "/api/"), Timeout = TimeSpan.FromSeconds(20) };
try
{
    bool pronto = false;
    for (int i = 0; i < 100 && !pronto; i++)
    {
        try { pronto = (await http.GetAsync("saude")).IsSuccessStatusCode; } catch (HttpRequestException) { }
        if (!pronto) await Task.Delay(100);
    }
    Ok(pronto, "API inicia e verifica a versão do banco");
    async Task<JsonNode> Chamar(string metodo, string rota, object? corpo = null, string? token = null, int esperado = 200)
    {
        using var req = new HttpRequestMessage(new HttpMethod(metodo), rota);
        if (corpo is not null) req.Content = JsonContent.Create(corpo);
        if (token is not null) req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var resp = await http.SendAsync(req);
        string texto = await resp.Content.ReadAsStringAsync();
        if ((int)resp.StatusCode != esperado) throw new Exception($"{metodo} {rota}: esperado {esperado}, recebido {(int)resp.StatusCode}: {texto}");
        return string.IsNullOrWhiteSpace(texto) ? new JsonObject() : JsonNode.Parse(texto)!;
    }
    async Task<JsonNode> Login(string email, bool vendedor = false) => await Chamar("POST", $"auth/{(vendedor ? "vendedores" : "administradores")}/login", new { email, senha });
    string Token(JsonNode s) => s["accessToken"]!.GetValue<string>();
    int Id(JsonNode n) => n["id"]!.GetValue<int>();
    await Chamar("GET", "admin/painel", esperado: 401);
    await Chamar("POST", "auth/administradores/login", new { email = "admin@teste.local", senha = "incorreta" }, esperado: 401);
    Ok(true, "Rotas protegidas e senha incorreta recusadas");
    var login = await Login("admin@teste.local"); var admin = Token(login);
    var admin2 = Token(await Login("admin2@teste.local"));
    var renovada = await Chamar("POST", "auth/renovar", new { refreshToken = login["refreshToken"]!.GetValue<string>() });
    await Chamar("GET", "admin/painel", token: Token(renovada));
    await Chamar("POST", "auth/renovar", new { refreshToken = "invalido" }, esperado: 401);
    Ok(true, "Renovação válida funciona e token inválido é recusado");
    JsonObject Pessoa(string nome, string doc, string email, int vendedorId = 0) => new()
    {
        ["nome"] = nome, ["documento"] = doc, ["rua"] = "Rua das Flores", ["bairro"] = "Centro",
        ["cep"] = "85800000", ["numero"] = "10", ["cidadeId"] = 1, ["telefone"] = "45999999999",
        ["email"] = email, ["senha"] = senha, ["vendedorId"] = vendedorId, ["ativo"] = true
    };
    var v1 = await Chamar("POST", "admin/vendedores", Pessoa("Vendedor Um", "22222222221", "v1@teste.local"), admin);
    var v2 = await Chamar("POST", "admin/vendedores", Pessoa("Vendedor Dois", "22222222222", "v2@teste.local"), admin);
    var vendedor = Token(await Login("v1@teste.local", true));
    var outro = Token(await Login("v2@teste.local", true));
    Ok(v1["ativo"]!.GetValue<bool>() && v1["senhaHash"] is null, "Vendedor nasce ativo e hash não é exposto");
    await Chamar("GET", "admin/painel", token: vendedor, esperado: 403);
    await Chamar("GET", "mobile/sincronizacao", token: admin, esperado: 403);
    Ok(true, "Perfis de vendedor e administrador são separados");
    var invalida = Pessoa("Inválido", "22222222223", "invalido@teste.local"); invalida["cidadeId"] = 999999;
    await Chamar("POST", "admin/vendedores", invalida, admin, 422);
    await Chamar("POST", "admin/vendedores", Pessoa("Repetido", "222.222.222-21", "outro@teste.local"), admin, 409);
    await Chamar("POST", "admin/cidades", new { nome = "Sem estado", estadoId = 999999 }, admin, 422);
    Ok(true, "Cidade inexistente, documento duplicado e cidade sem estado são recusados");
    var estado = await Chamar("POST", "admin/estados", new { nome = "Estado de teste", uf = "ZZ" }, admin);
    var cidade = await Chamar("POST", "admin/cidades", new { nome = "Cidade de teste", estadoId = Id(estado) }, admin);
    Ok(cidade["estadoId"]!.GetValue<int>() == Id(estado), "Nova cidade preserva a chave estrangeira do estado");
    var clientes = new List<int>();
    for (int i = 1; i <= 3; i++) clientes.Add(Id(await Chamar("POST", "admin/clientes", Pessoa("Padaria " + i, "3333333333" + i, $"c{i}@teste.local", Id(v1)), admin)));
    int clienteOutro = Id(await Chamar("POST", "admin/clientes", Pessoa("Outra carteira", "33333333334", "c4@teste.local", Id(v2)), admin));
    var novoMobile = await Chamar("POST", "clientes", Pessoa("Cadastro mobile", "33333333335", "c5@teste.local", Id(v2)), vendedor);
    Ok(novoMobile["vendedorId"]!.GetValue<int>() == Id(v1), "Cliente cadastrado pelo Mobile usa o vendedor do token");
    var catalogo = await Chamar("GET", "admin/painel", token: admin);
    int categoria = Id(catalogo["categorias"]!.AsArray()[0]!);
    async Task<JsonNode> Produto(int estoque, int centavos = 1000) => await Chamar("POST", "admin/produtos", new { nome = "Chocolate de teste", descricao = "Pacote", categoriaId = categoria, precoCentavos = centavos, estoque }, admin);
    var produto = await Produto(11); int produtoId = Id(produto);
    var mobileApi = new ApiService(new HttpClient { BaseAddress = new Uri(url + "/api/") }) { Token = vendedor };
    var sincronizado = await mobileApi.SincronizarAsync();
    Ok(sincronizado.Clientes.All(c => c.VendedorId == Id(v1)) && sincronizado.CondicoesPagamento.Count == 3 && sincronizado.Produtos.Any(p => p.Preco == 10m),
        "Classes e cliente HTTP reais do Mobile desserializam o catálogo e isolam a carteira");
    object Pedido(int clienteId, int quantidade, int pId = 0, decimal preco = 10m, int condicao = 3) => new
    {
        clienteId, dataHoraCriacao = DateTimeOffset.UtcNow, condicaoPagamentoId = condicao,
        itens = new[] { new { produtoId = pId == 0 ? produtoId : pId, quantidadeSolicitada = quantidade, precoUnitario = preco } },
        parcelas = Enumerable.Range(1, condicao == 1 ? 1 : condicao == 2 ? 2 : 3).Select(n => new { numeroParcela = n, formaPagamentoId = n }).ToArray()
    };
    object Lote(params object[] pedidos) => new { identificadorEnvio = Guid.NewGuid().ToString(), dataHoraCriacao = DateTimeOffset.UtcNow, pedidos };
    var corpo = Lote(Pedido(clientes[0], 10), Pedido(clientes[1], 6), Pedido(clientes[2], 4));
    var recebidos = await Task.WhenAll(Chamar("POST", "pedidos-gerais", corpo, vendedor), Chamar("POST", "pedidos-gerais", corpo, vendedor));
    int loteId = Id(recebidos[0]);
    Ok(loteId == Id(recebidos[1]) && await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Pedidos") == 3, "Envios simultâneos com a mesma chave não duplicam pedidos");
    await Chamar("GET", "pedidos-gerais/por-identificador/" + recebidos[0]["identificadorEnvio"]!.GetValue<string>(), token: outro, esperado: 404);
    var divergente = JsonSerializer.SerializeToNode(corpo)!; divergente["pedidos"]![0]!["itens"]![0]!["quantidadeSolicitada"] = 11;
    await Chamar("POST", "pedidos-gerais", divergente, vendedor, 409);
    Ok(true, "Chave repetida com outro conteúdo e consulta por outro vendedor são recusadas");
    var errado = Lote(Pedido(clientes[0], 1, preco: 9m));
    var rejeicao = await Chamar("POST", "pedidos-gerais", errado, vendedor, 409);
    Ok(rejeicao["loteNaoGravado"]!.GetValue<bool>(), "Preço divergente recebe rejeição definitiva sem gravar lote parcial");
    await Chamar("POST", "pedidos-gerais", errado, vendedor, 409);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1), Pedido(clienteOutro, 1)), vendedor, 422);
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM PedidoGeral") == 1, "Lote inválido é atômico e repetição da rejeição permanece segura");
    var previa = await Chamar("GET", $"admin/pedidos-gerais/{loteId}/previa", token: admin);
    var quotas = previa["pedidos"]!.AsArray().Select(p => p!["itens"]![0]!["quantidadeAtendida"]!.GetValue<int>()).ToArray();
    Ok(quotas.SequenceEqual(new[] { 6, 3, 2 }) && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@produtoId", new { produtoId }) == 11,
        "Prévia distribui 11 unidades entre demandas 10/6/4 como 6/3/2 sem baixar estoque");
    var confirmar = new { tokenPrevia = previa["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() };
    await Task.WhenAll(Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", confirmar, admin),
        Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", new { confirmar.tokenPrevia, identificadorOperacao = Guid.NewGuid().ToString() }, admin2));
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Vendas") == 3 && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@produtoId", new { produtoId }) == 0,
        "Duas confirmações simultâneas geram uma venda por pedido e baixam o estoque uma vez");
    var final = (await mobileApi.SincronizarAsync()).PedidosGerais.Single();
    Ok(final.Status == "Processado" && final.Pedidos.All(p => p.Status == "ConfirmadoComCorte") && final.Pedidos.Sum(p => p.ValorTotalAtendido) == 110m,
        "Mobile recebe automaticamente os status, cortes e valores da confirmação");
    Ok(final.Pedidos.All(p => p.Parcelas.Select(x => x.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }) && p.Parcelas.Sum(x => x.ValorFinal) == p.ValorTotalAtendido),
        "Pix, boleto e dinheiro permanecem independentes e parcelas fecham o total após o corte");
    Ok(final.Pedidos.Single(p => p.ClienteId == clientes[2]).Parcelas.Select(p => p.ValorFinal).SequenceEqual(new decimal?[] { 6.66m, 6.66m, 6.68m }),
        "Confirmação conserva centavos e ajusta somente a última parcela");
    var dataLoja = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo")).Date;
    Ok(final.Pedidos.All(p => p.Parcelas.All(x => x.DataVencimento?.Date == dataLoja.AddDays(x.DiasAposConfirmacao))), "Vencimentos usam dias corridos após a confirmação na data da loja");
    await Chamar("POST", $"admin/pedidos-gerais/{loteId}/confirmacao", confirmar, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{loteId}/cancelamento", new { motivo = "Já confirmado", versao = 2, identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    Ok(await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ContasReceber") == 9, "Repetir confirmação não duplica parcelas; venda confirmada não passa pelo cancelamento de pendentes");
    await Chamar("PUT", $"admin/produtos/{produtoId}", new { nome = "Antigo", categoriaId = categoria, precoCentavos = 1000, estoque = 11, versao = 1, ativo = true }, admin, 409);
    Ok(true, "Cadastro antigo não sobrescreve estoque atualizado por venda");
    var pZero = await Produto(0);
    var loteZero = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pZero))), vendedor);
    var prevZero = await Chamar("GET", $"admin/pedidos-gerais/{Id(loteZero)}/previa", token: admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(loteZero)}/confirmacao", new { tokenPrevia = prevZero["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() }, admin);
    Ok((await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(loteZero)).Pedidos.Single().Status == "NaoAtendido" &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Vendas") == 3, "Estoque zero registra não atendimento sem criar cobrança");
    var pNovo = await Produto(5, 3333);
    var cancelavel = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 2, Id(pNovo), 33.33m)), vendedor);
    var cancelamento = new { motivo = "Solicitação do cliente", versao = 1, identificadorOperacao = Guid.NewGuid().ToString() };
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/cancelamento", cancelamento, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/cancelamento", cancelamento, admin);
    Ok((await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(cancelavel)).Status == "Cancelado" &&
        await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(pNovo) }) == 5, "Cancelamento repetido atualiza o Mobile e conserva o estoque");
    await Chamar("POST", $"admin/pedidos-gerais/{Id(cancelavel)}/confirmacao", new { tokenPrevia = "qualquer", identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    var stale = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 2, Id(pNovo), 33.33m)), vendedor);
    var prevStale = await Chamar("GET", $"admin/pedidos-gerais/{Id(stale)}/previa", token: admin);
    await Chamar("PUT", $"admin/produtos/{Id(pNovo)}", new { nome = "Atualizado", categoriaId = categoria, precoCentavos = 3333, estoque = 4, versao = 1, ativo = true }, admin);
    await Chamar("POST", $"admin/pedidos-gerais/{Id(stale)}/confirmacao", new { tokenPrevia = prevStale["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() }, admin, 409);
    Ok(await db.ExecuteScalarAsync<string>("SELECT Status FROM PedidoGeral WHERE Id=@id", new { id = Id(stale) }) == "AguardandoConfirmacao", "Prévia desatualizada recusa confirmação inteira");
    var concorrente = await Produto(5);
    var a = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 4, Id(concorrente), condicao: 1)), vendedor);
    var b = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[1], 4, Id(concorrente), condicao: 2)), vendedor);
    var pa = await Chamar("GET", $"admin/pedidos-gerais/{Id(a)}/previa", token: admin);
    var pb = await Chamar("GET", $"admin/pedidos-gerais/{Id(b)}/previa", token: admin);
    async Task<int> TentarConfirmar(int id, JsonNode previaAtual)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"admin/pedidos-gerais/{id}/confirmacao")
        {
            Content = JsonContent.Create(new { tokenPrevia = previaAtual["tokenPrevia"]!.GetValue<string>(), identificadorOperacao = Guid.NewGuid().ToString() })
        };
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin);
        using var resp = await http.SendAsync(req);
        return (int)resp.StatusCode;
    }
    var concorrencia = await Task.WhenAll(TentarConfirmar(Id(a), pa), TentarConfirmar(Id(b), pb));
    Ok(concorrencia.Order().SequenceEqual(new[] { 200, 409 }) && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(concorrente) }) == 1,
        "Lotes diferentes disputando estoque não vendem a mesma unidade duas vezes");
    int restanteId = concorrencia[0] == 409 ? Id(a) : Id(b);
    var novaPrevia = await Chamar("GET", $"admin/pedidos-gerais/{restanteId}/previa", token: admin);
    Ok(await TentarConfirmar(restanteId, novaPrevia) == 200 && await db.ExecuteScalarAsync<int>("SELECT Estoque FROM Produtos WHERE Id=@id", new { id = Id(concorrente) }) == 0,
        "Após rever a prévia, o segundo lote recebe somente o saldo restante");
    var centavo = await Produto(1, 1);
    var minimo = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(centavo), 0.01m)), vendedor);
    var pm = await Chamar("GET", $"admin/pedidos-gerais/{Id(minimo)}/previa", token: admin);
    await TentarConfirmar(Id(minimo), pm);
    var parcelaMinima = (await mobileApi.SincronizarAsync()).PedidosGerais.Single(l => l.Id == Id(minimo)).Pedidos.Single().Parcelas;
    Ok(parcelaMinima.Select(p => p.ValorFinal).SequenceEqual(new decimal?[] { 0m, 0m, 0.01m }) &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ContasReceber WHERE PedidoId=@id", new { id = minimo["pedidos"]![0]!["id"]!.GetValue<int>() }) == 1,
        "Total de um centavo mantém as três escolhas e não cria cobranças de zero");
    // Hub real: eventos posteriores ao marco não podem vazar para outra carteira.
    long marco = await db.ExecuteScalarAsync<long>("SELECT COALESCE(MAX(Id),0) FROM EventosAlteracao");
    var eventosLoja = new ConcurrentBag<JsonElement>(); var eventosV1 = new ConcurrentBag<JsonElement>(); var eventosV2 = new ConcurrentBag<JsonElement>();
    async Task<HubConnection> Hub(string token, ConcurrentBag<JsonElement> eventos)
    {
        var h = new HubConnectionBuilder().WithUrl(url + "/hubs/atualizacoes", o => o.AccessTokenProvider = () => Task.FromResult<string?>(token)).Build();
        h.On("DadosAlterados", (JsonElement e) => eventos.Add(e)); await h.StartAsync(); return h;
    }
    await using var hLoja = await Hub(admin, eventosLoja);
    await using var hV1 = await Hub(vendedor, eventosV1);
    await using var hV2 = await Hub(outro, eventosV2);
    var paraAvisar = await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor);
    bool Tem(ConcurrentBag<JsonElement> eventos) => eventos.Any(e => e.GetProperty("id").GetInt64() > marco && e.GetProperty("tipo").GetString() == "Pedidos");
    for (int i = 0; i < 40 && !(Tem(eventosLoja) && Tem(eventosV1)); i++) await Task.Delay(100);
    Ok(Tem(eventosLoja) && Tem(eventosV1) && !Tem(eventosV2), "SignalR real avisa loja e vendedor responsável, sem avisar outra carteira sobre seus pedidos");
    Ok(eventosLoja.All(e => !e.TryGetProperty("cliente", out _) && !e.TryGetProperty("pedidos", out _)), "Avisos contêm somente identificador/tipo/data, sem dados pessoais");
    var caminhoSqlite = Path.Combine(Path.GetTempPath(), nomeBanco + ".sqlite");
    var bancoLocal = new BancoLocal(caminhoSqlite); var seguro = new SessaoMemoria();
    var sistema = new AplicacaoService(bancoLocal, mobileApi, seguro);
    await sistema.InicializarAsync(); await sistema.EntrarAsync("v1@teste.local", senha); await sistema.AtualizarAsync();
    mobileApi.Token = "acesso-expirado-para-teste";
    await sistema.AtualizarAsync();
    Ok(sistema.TemSessao && mobileApi.Token != "acesso-expirado-para-teste" && (await seguro.LerAsync())!.Contains("refreshToken"),
        "Mobile renova acesso recusado, repete a consulta e persiste a nova sessão segura");
    await sistema.AbrirCarrinhoAsync(clientes[0]); await sistema.AdicionarProdutoAsync(clientes[0], Id(pNovo), 1);
    await using var tempoReal = new TempoRealService(sistema, url + "/api/");
    await tempoReal.IniciarAsync();
    await Chamar("POST", $"admin/pedidos-gerais/{Id(paraAvisar)}/cancelamento", new { motivo = "Teste atualização", versao = 1, identificadorOperacao = Guid.NewGuid().ToString() }, admin);
    bool refletiu = false;
    for (int i = 0; i < 60 && !refletiu; i++)
    {
        refletiu = (await sistema.ObterPedidosClienteAsync(clientes[0])).Any(p => p.Pedido.Id == paraAvisar["pedidos"]![0]!["id"]!.GetValue<int>() && p.Pedido.Status == "Cancelado");
        if (!refletiu) await Task.Delay(100);
    }
    Ok(refletiu && (await sistema.ObterCarrinhoAsync(clientes[0])).Itens.Count == 1, "Serviço real do Mobile atualiza o SQLite via SignalR sem apagar o carrinho aberto");
    var conta = await db.QuerySingleAsync<VanderCake.Api.Models.ContaReceber>("SELECT * FROM ContasReceber WHERE PedidoId=@id ORDER BY Id LIMIT 1",
        new { id = final.Pedidos.Single(p => p.ClienteId == clientes[0]).Id });
    await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", esperado: 401);
    await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: vendedor, esperado: 403);
    await Chamar("POST", "admin/contas-receber/2147483647/recebimento", token: admin, esperado: 404);
    Ok(true, "Somente administradores registram recebimento de uma parcela existente");
    long marcoPagamento = await db.ExecuteScalarAsync<long>("SELECT MAX(Id) FROM EventosAlteracao");
    var baixas = await Task.WhenAll(
        Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin),
        Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin2));
    var baixada = await db.QuerySingleAsync<VanderCake.Api.Models.ContaReceber>("SELECT * FROM ContasReceber WHERE Id=@Id", conta);
    Ok(baixas.All(bx => bx["status"]!.GetValue<string>() == "Paga") && baixada.DataPagamento?.Date == dataLoja &&
        baixas.Select(bx => bx["administradorBaixaId"]!.GetValue<int>()).Distinct().Count() == 1 && baixada.Valor == conta.Valor,
        "Dois recebimentos simultâneos mantêm uma baixa integral, uma data e um responsável");
    var repetida = await Chamar("POST", $"admin/contas-receber/{conta.Id}/recebimento", token: admin2);
    Ok(repetida["administradorBaixaId"]!.GetValue<int>() == baixada.AdministradorBaixaId &&
        await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM EventosAlteracao WHERE Tipo='Pagamentos' AND Id>@marcoPagamento", new { marcoPagamento }) == 1,
        "Repetir recebimento conserva a primeira baixa e não gera outro evento financeiro");
    bool refletiuBaixa = false;
    for (int i = 0; i < 60 && !refletiuBaixa; i++)
    {
        var atualizado = (await sistema.ObterPedidosClienteAsync(clientes[0])).Single(p => p.Pedido.Id == conta.PedidoId).Pedido;
        refletiuBaixa = atualizado.Parcelas.Any(p => p.NumeroParcela == 1 && p.Situacao == "Paga" && p.DataPagamento?.Date == dataLoja && p.FormaPagamentoId == 1);
        if (!refletiuBaixa) await Task.Delay(100);
    }
    Ok(refletiuBaixa && (await sistema.ObterCarrinhoAsync(clientes[0])).Itens.Count == 1,
        "Recebimento chega ao SQLite do Mobile por SignalR com data e meio da parcela, preservando o carrinho");
    Ok(eventosV1.Any(e => e.GetProperty("id").GetInt64() > marcoPagamento && e.GetProperty("tipo").GetString() == "Pagamentos") &&
        !eventosV2.Any(e => e.GetProperty("id").GetInt64() > marcoPagamento && e.GetProperty("tipo").GetString() == "Pagamentos"),
        "Aviso de recebimento é destinado à carteira correta");
    await tempoReal.PararAsync();
    var formaExtra = await Chamar("POST", "admin/formas-pagamento", new { descricao = "Forma de teste", ativo = true }, admin);
    await Chamar("PUT", $"admin/formas-pagamento/{Id(formaExtra)}", new { descricao = "Forma revisada", ativo = false, versao = 1 }, admin);
    await Chamar("PUT", $"admin/formas-pagamento/{Id(formaExtra)}", new { descricao = "Edição antiga", ativo = true, versao = 1 }, admin, 409);
    var condicaoExtra = await Chamar("POST", "admin/condicoes-pagamento", new { descricao = "Condição de teste", dias = new[] { 7, 14 }, ativo = true }, admin);
    await Chamar("PUT", $"admin/condicoes-pagamento/{Id(condicaoExtra)}", new { descricao = "Condição revisada", dias = new[] { 0 }, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "admin/condicoes-pagamento", new { descricao = "Prazo inválido", dias = new[] { 30 }, ativo = true }, admin, 422);
    var cadastros = await mobileApi.SincronizarAsync();
    Ok(cadastros.FormasPagamento.Any(f => f.Id == Id(formaExtra) && !f.Ativo && f.Descricao == "Forma revisada") &&
        cadastros.CondicoesPagamento.Any(c => c.Id == Id(condicaoExtra) && !c.Ativo && c.Parcelas.Single().DiasAposConfirmacao == 0),
        "Condições e formas são cadastradas/alteradas; versão antiga e prazo não previsto são recusados");
    string descricaoForma = await db.ExecuteScalarAsync<string>("SELECT Descricao FROM FormasPagamento WHERE Id=1") ?? "";
    string descricaoCondicao = await db.ExecuteScalarAsync<string>("SELECT Descricao FROM CondicoesPagamento WHERE Id=3") ?? "";
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = "Reescrever histórico", ativo = true, versao = 1 }, admin, 409);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 0 }, ativo = true, versao = 1 }, admin, 409);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 7, 14, 21 }, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor, 422);
    await Chamar("PUT", "admin/condicoes-pagamento/3", new { descricao = descricaoCondicao, dias = new[] { 7, 14, 21 }, ativo = true, versao = 2 }, admin);
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = descricaoForma, ativo = false, versao = 1 }, admin);
    await Chamar("POST", "pedidos-gerais", Lote(Pedido(clientes[0], 1, Id(pNovo), 33.33m)), vendedor, 422);
    await Chamar("PUT", "admin/formas-pagamento/1", new { descricao = descricaoForma, ativo = true, versao = 2 }, admin);
    Ok(true, "Cadastros usados preservam descrição/prazo; desativação impede novas escolhas sem alterar o histórico");
    var categoriaInativa = await Chamar("POST", "admin/categorias", new { nome = "Categoria inativa", ativo = false }, admin);
    await Chamar("POST", "admin/produtos", new { nome = "Produto inválido", categoriaId = Id(categoriaInativa), precoCentavos = 100, estoque = 1 }, admin, 422);
    var categoriaAtual = await db.QuerySingleAsync<VanderCake.Api.Models.Categoria>("SELECT * FROM Categorias WHERE Id=@categoria", new { categoria });
    await Chamar("PUT", $"admin/categorias/{categoria}", new { nome = categoriaAtual.Nome, ativo = false, versao = categoriaAtual.Versao }, admin, 409);
    Ok(true, "Categoria inativa não recebe produto e categoria com produtos ativos não pode ser desativada");
    var antes = Pessoa("Vendedor Um", "22222222221", "v1@teste.local"); antes["versao"] = 1; antes["senha"] = "Nova-senha-2026!";
    await Chamar("PUT", $"admin/vendedores/{Id(v1)}", antes, admin);
    await Chamar("GET", "mobile/sincronizacao", token: vendedor, esperado: 401);
    Ok(true, "Alterar senha revoga tokens antigos");
    var vendedor2Atual = (await Chamar("GET", "admin/painel", token: admin))["vendedores"]!.AsArray().Single(v => Id(v!) == Id(v2))!;
    await Chamar("PATCH", $"admin/vendedores/{Id(v2)}/atividade", new { ativo = false, versao = vendedor2Atual["versao"]!.GetValue<long>() }, admin);
    await Chamar("GET", "mobile/sincronizacao", token: outro, esperado: 401);
    await Chamar("POST", "auth/vendedores/login", new { email = "v2@teste.local", senha }, esperado: 401);
    Ok(true, "Desligar vendedor bloqueia sessão aberta e novo login sem apagar a carteira");
    try { await db.ExecuteAsync("INSERT INTO Cidades (Nome,EstadoId) VALUES ('Órfã',999999)"); Ok(false, "FK deve recusar órfão"); }
    catch (MySqlException e) when (e.Number == 1452) { Ok(true, "Chave estrangeira também protege inserções SQL diretas"); }
    try { await db.ExecuteAsync("UPDATE Produtos SET Estoque=-1 WHERE Id=@produtoId", new { produtoId }); Ok(false, "CHECK deve recusar negativo"); }
    catch (MySqlException e) when (e.Number == 3819) { Ok(true, "CHECK do MySQL impede estoque negativo"); }
    if (Environment.GetEnvironmentVariable("VANDERCAKE_TESTAR_WEB_HTTP") == "1")
    {
        var nodeInfo = new ProcessStartInfo("node") { WorkingDirectory = raiz };
        nodeInfo.ArgumentList.Add(Path.Combine(raiz, "TestesIntegracao/web-http.mjs"));
        nodeInfo.Environment["VANDERCAKE_TEST_URL"] = url;
        using var node = Process.Start(nodeInfo)!;
        await node.WaitForExitAsync();
        Ok(node.ExitCode == 0, "Cliente JavaScript real da Web passou pelos testes HTTP/SignalR");
    }
    await bancoLocal.FecharAsync();
    Console.WriteLine($"\n{testes} verificações de integração concluídas. Banco isolado: {nomeBanco}. Usuario isolado: {usuarioApi}");
}
catch
{
    Console.Error.WriteLine(string.Join(Environment.NewLine, logs));
    throw;
}
finally
{
    if (!apiProcesso.HasExited) { apiProcesso.Kill(entireProcessTree: true); await apiProcesso.WaitForExitAsync(); }
    // Banco de testes mantido para inspeção. Nenhum DROP automático.
}

sealed class SessaoMemoria : ISessaoSegura
{
    private string? _json;
    public Task<string?> LerAsync() => Task.FromResult(_json);
    public Task SalvarAsync(string conteudo) { _json = conteudo; return Task.CompletedTask; }
    public Task LimparAsync() { _json = null; return Task.CompletedTask; }
}
