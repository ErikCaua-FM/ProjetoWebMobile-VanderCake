using System.Net;
using System.Text;
using VanderCake.Contracts;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;
using VanderCake.Services;

int verificacoes = 0;
void Verificar(bool condicao, string descricao)
{
    if (!condicao)
        throw new Exception("FALHOU: " + descricao);
    verificacoes++;
    Console.WriteLine("OK: " + descricao);
}

async Task EsperarErro(Func<Task> acao, string descricao)
{
    bool falhou = false;
    try
    {
        await acao();
    }
    catch (Exception)
    {
        falhou = true;
    }

    Verificar(falhou, descricao);
}

Verificar(Dinheiro.Dividir(100m, 3).SequenceEqual(new[] { 33.33m, 33.33m, 33.34m }), "Arredondamento conserva os centavos na última parcela");
Verificar(Dinheiro.Dividir(0.01m, 3).SequenceEqual(new[] { 0m, 0m, 0.01m }), "Valor pequeno não cria dinheiro adicional");
var caminho = Path.Combine(Path.GetTempPath(), $"vandercake-teste-{Guid.NewGuid():N}.db3");
var seguro = new SessaoEmMemoria();
var api = new ApiDeTeste();
var banco = new BancoLocal(caminho);
var sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
Verificar(!sistema.TemSessao, "Primeira abertura exige login");
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
await sistema.AtualizarAsync();
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 10);
await sistema.SelecionarCondicaoAsync(10, 3);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.SelecionarFormaAsync(10, 2, 2);
await sistema.SelecionarFormaAsync(10, 3, 3);
var carrinho = await sistema.ObterCarrinhoAsync(10);
Verificar(carrinho.Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }), "7 dias Pix, 14 dias boleto, 21 dias dinheiro são independentes");
Verificar(carrinho.Parcelas.Select(p => p.ValorPrevisto).SequenceEqual(new[] { 33.33m, 33.33m, 33.34m }), "Carrinho distribui R$ 100,00 corretamente");
await banco.FecharAsync();
banco = new BancoLocal(caminho);
sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
Verificar(sistema.TemSessao && sistema.VendedorAtual?.Id == 1 && api.Logins == 1, "Reabertura restaura a sessão sem novo login");
Verificar((await sistema.ObterCarrinhoAsync(10)).Itens.Single().QuantidadeSolicitada == 10, "Carrinho sobrevive ao fechamento do banco e do serviço");
await sistema.AcrescentarPedidoAsync(10);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Status == "Rascunho", "Pedido aparece imediatamente no histórico do cliente");
Verificar(JsonDados.Ler<PedidoGeral>((await sistema.ObterLoteAbertoAsync())!.ConteudoJson).Pedidos.Count == 1, "Mesmo pedido aparece na lista geral");
Verificar(api.Posts == 0, "Acrescentar à lista não transmite pedidos");
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 1, "Sincronização não apaga pedidos locais");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AlterarQuantidadeAsync(10, 100, 12);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Itens.Single().QuantidadeSolicitada == 10, "Editar carrinho não altera a versão concluída antes de confirmar");
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Envio bloqueado enquanto há carrinho em edição");
await sistema.DescartarCarrinhoAsync(10);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Itens.Single().QuantidadeSolicitada == 10, "Descartar edição preserva o pedido anterior");
await sistema.SairAsync();
await sistema.EntrarAsync("vendedor2@teste.local", "senha");
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 0 && await sistema.ObterLoteAbertoAsync() is null, "Outra conta não acessa os pedidos do vendedor anterior");
await sistema.SairAsync();
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 1, "Rascunhos reaparecem no login da conta correta");
await sistema.AbrirCarrinhoAsync(11);
await sistema.AdicionarProdutoAsync(11, 100, 2);
await sistema.SelecionarCondicaoAsync(11, 1);
await sistema.SelecionarFormaAsync(11, 1, 3);
await sistema.AcrescentarPedidoAsync(11);
api.FalharDepoisDeGravar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Simulação de resposta perdida após gravação no servidor");
Verificar((await sistema.ObterLoteAbertoAsync())?.SituacaoLocal == "EnvioIncerto", "Falha deixa o lote protegido e persistido");
await EsperarErro(() => sistema.RemoverPedidoAsync(10), "Envio incerto bloqueia alteração de conteúdo");
await EsperarErro(() => sistema.ComprarNovamenteAsync(10, identificadorOrigem: ""), "Envio incerto bloqueia a recompra");
await banco.FecharAsync();
banco = new BancoLocal(caminho);
sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
var recebido = await sistema.EnviarPedidosAsync();
Verificar(recebido.Pedidos.Count == 2 && api.Posts == 1, "Retomada após reabertura consulta o lote recebido sem duplicar o POST");
Verificar(await sistema.ObterLoteAbertoAsync() is null, "Lote recebido libera uma nova lista");
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Id > 0, "Pedido local é conciliado com o identificador oficial sem duplicar histórico");
var envio = JsonDados.Ler<PedidoGeralEnvio>(api.Corpos.Single());
Verificar(envio.Pedidos.Single(p => p.ClienteId == 10).Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }), "API recebe a forma de pagamento de cada parcela");
api.Lotes[0].Status = "Processado";
var confirmado = api.Lotes[0].Pedidos.Single(p => p.ClienteId == 10);
confirmado.Status = "ConfirmadoComCorte";
confirmado.Itens[0].QuantidadeAtendida = 6;
confirmado.ValorTotalAtendido = 60m;
foreach (var parcela in confirmado.Parcelas)
{
    parcela.ValorFinal = 20m;
    parcela.DataVencimento = new DateTime(2026, 10, 22).AddDays(parcela.DiasAposConfirmacao);
    parcela.Situacao = "Pendente";
}

await sistema.AtualizarAsync();
var detalhe = await sistema.ObterDetalheAsync(recebido.IdentificadorEnvio, 10);
Verificar(detalhe.Pedido.Itens[0].QuantidadeSolicitada == 10 && detalhe.Pedido.Itens[0].QuantidadeAtendida == 6 && detalhe.Pedido.ValorTotalAtendido == 60m, "Resultado do corte preserva solicitação e atualiza atendimento/valor");
Verificar(detalhe.Pedido.Parcelas.All(p => p.ValorFinal == 20m), "Valores finais das parcelas são exibidos a partir da resposta da loja");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 1);
await sistema.SelecionarCondicaoAsync(10, 1);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.AcrescentarPedidoAsync(10);
api.FalharAntesDeGravar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Falha de rede antes do recebimento mantém a mesma chave");
await sistema.EnviarPedidosAsync();
Verificar(api.Corpos[^1] == api.Corpos[^2], "Consulta 404 retransmite exatamente o corpo congelado");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 1);
await sistema.SelecionarCondicaoAsync(10, 1);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.AcrescentarPedidoAsync(10);
api.Rejeitar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Rejeição definitiva de validação não perde a lista");
Verificar((await sistema.ObterLoteAbertoAsync())?.SituacaoLocal == "Montagem", "Somente rejeição com loteNaoGravado libera edição");
api.Rejeitar = false;
api.Preco = 12m;
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).First().Pedido.Itens[0].PrecoUnitario == 10m, "Atualizar catálogo não altera preços históricos ou de rascunhos");
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Preço divergente exige revisão explícita");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AtualizarPrecosAsync(10);
Verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 12m, "Revisão explícita atualiza o valor e recalcula as parcelas");
var novoCliente = await sistema.CadastrarClienteAsync(new NovoClienteRequisicao
{
    Nome = "Cliente novo",
    Documento = "12345678901",
    Rua = "Rua Teste",
    Bairro = "Centro",
    Numero = "10",
    Cep = "85851000",
    CidadeId = 1,
    Telefone = "45999999999"
});
Verificar(novoCliente.VendedorId == 1 && (await sistema.ObterDadosAsync()).Clientes.Any(c => c.Id == novoCliente.Id), "Cadastro confirmado pela API aparece imediatamente na carteira");
await EsperarErro(() => sistema.CadastrarClienteAsync(new NovoClienteRequisicao
{
    Nome = "Cliente repetido",
    Documento = "123.456.789-01",
    Rua = "Rua Teste",
    Bairro = "Centro",
    Numero = "10",
    Cep = "85851000",
    CidadeId = 1,
    Telefone = "45999999999"
}), "CPF com outra pontuação não permite duplicar cliente na carteira");
api.ErroSincronizacao = 0;
await EsperarErro(() => sistema.AtualizarAsync(), "Sincronização sem rede informa a falha");
Verificar(sistema.TemSessao && (await sistema.ObterDadosAsync()).Produtos.Count > 0, "Sem rede, sessão e catálogo local continuam disponíveis");
api.ErroSincronizacao = 401;
await EsperarErro(() => sistema.AtualizarAsync(), "Token recusado pela API exige autenticação");
Verificar(!sistema.TemSessao && await seguro.LerAsync() is null, "401 remove a sessão persistida sem apagar os rascunhos");
api.ErroSincronizacao = null;
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
Verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 12m, "Carrinho preservado após expiração da sessão");

var http = new HttpClient(new HandlerDeTeste())
{
    BaseAddress = new Uri("https://teste.invalid/api/")
};
var transporte = new ApiService(http)
{
    Token = "token-teste"
};
try
{
    await transporte.EnviarLoteAsync("{}");
    throw new Exception("Deveria rejeitar");
}
catch (ApiException ex)
{
    Verificar(ex.Status == 422 && ex.LoteNaoGravado && ex.Message == "Revise o pedido.", "Transporte interpreta rejeição definitiva da API");
}

await banco.FecharAsync();
File.Delete(caminho);
await RecompraTests.ExecutarAsync(Verificar);
Console.WriteLine($"\n{verificacoes} verificações concluídas.");
sealed class SessaoEmMemoria : ISessaoSegura
{
    private string? _valor;
    public Task<string?> LerAsync() => Task.FromResult(_valor);
    public Task SalvarAsync(string conteudo)
    {
        _valor = conteudo;
        return Task.CompletedTask;
    }

    public Task LimparAsync()
    {
        _valor = null;
        return Task.CompletedTask;
    }
}

sealed class HandlerDeTeste : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Parameter != "token-teste" || request.RequestUri?.AbsolutePath != "/api/pedidos-gerais")
            throw new Exception("Cabeçalho ou rota incorretos.");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.UnprocessableEntity) { Content = new StringContent("{\"mensagem\":\"Revise o pedido.\",\"loteNaoGravado\":true}", Encoding.UTF8, "application/json") });
    }
}

sealed class ApiDeTeste : IApiService
{
    public string? Token
    {
        get; set;
    }
    public int Logins
    {
        get; private set;
    }
    public int Posts
    {
        get; private set;
    }
    public bool FalharDepoisDeGravar
    {
        get; set;
    }
    public bool FalharAntesDeGravar
    {
        get; set;
    }
    public bool Rejeitar
    {
        get; set;
    }
    public decimal Preco { get; set; } = 10m;
    public int? ErroSincronizacao
    {
        get; set;
    }
    public List<string> Corpos { get; } = [];
    public List<PedidoGeral> Lotes { get; } = [];

    private int _vendedorId = 1;
    private DadosSincronizacao Dados() => new()
    {
        Vendedor = new Vendedor
        {
            Id = _vendedorId,
            Nome = "Vendedor de teste"
        },
        Clientes = new[]
        {
            10,
            11
        }.Select(id => new Cliente { Id = id, VendedorId = _vendedorId, Nome = $"Cliente {id}" }).ToList(),
        Estados = [new Estado { Id = 1, Nome = "Paraná", Uf = "PR" }],
        Cidades = [new Cidade { Id = 1, EstadoId = 1, Nome = "Foz do Iguaçu" }],
        Produtos = [new Produto
        {
            Id = 100,
            Nome = "Produto de teste",
            Preco = Preco,
            Estoque = 0,
            CategoriaId = 1
        }

        ],
        CondicoesPagamento = new[]
        {
            new[]
            {
                0
            },
            new[]
            {
                7,
                14
            },
            new[]
            {
                7,
                14,
                21
            }
        }.Select((dias, i) => new CondicaoPagamento { Id = i + 1, Descricao = i == 0 ? "À vista" : string.Join('/', dias), Parcelas = dias.Select((dia, j) => new RegraParcela { NumeroParcela = j + 1, DiasAposConfirmacao = dia }).ToList() }).ToList(),
        FormasPagamento = [new()
        {
            Id = 1,
            Descricao = "Pix"
        }, new()
        {
            Id = 2,
            Descricao = "Boleto bancário"
        }, new()
        {
            Id = 3,
            Descricao = "Dinheiro"
        }

        ],
        PedidosGerais = JsonDados.Copiar(Lotes.Where(l => l.VendedorId == _vendedorId).ToList())
    };
    public Task<LoginResposta> EntrarAsync(LoginRequisicao login)
    {
        Logins++;
        _vendedorId = login.Email.Contains("2@") ? 2 : 1;
        return Task.FromResult(new LoginResposta { AccessToken = "token", Vendedor = Dados().Vendedor });
    }

    public Task<DadosSincronizacao> SincronizarAsync()
    {
        if (ErroSincronizacao is int status)
            throw new ApiException(status, "Falha simulada de sincronização.");
        return Task.FromResult(Dados());
    }
    public Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao cliente) => Task.FromResult(new Cliente
    {
        Id = 900,
        VendedorId = _vendedorId,
        Nome = cliente.Nome,
        Documento = cliente.Documento,
        Rua = cliente.Rua,
        Bairro = cliente.Bairro,
        Numero = cliente.Numero,
        Cep = cliente.Cep,
        CidadeId = cliente.CidadeId,
        Telefone = cliente.Telefone
    });
    public Task<PedidoGeral?> ConsultarLoteAsync(string identificador) => Task.FromResult(Lotes.FirstOrDefault(l => l.IdentificadorEnvio == identificador));
    public Task<PedidoGeral> EnviarLoteAsync(string json)
    {
        Posts++;
        Corpos.Add(json);
        if (Rejeitar)
            throw new ApiException(422, "Pedido rejeitado.", true);
        if (FalharAntesDeGravar)
        {
            FalharAntesDeGravar = false;
            throw new ApiException(0, "Timeout");
        }

        var envio = JsonDados.Ler<PedidoGeralEnvio>(json);
        var existente = Lotes.FirstOrDefault(l => l.IdentificadorEnvio == envio.IdentificadorEnvio);
        if (existente is not null)
            return Task.FromResult(JsonDados.Copiar(existente));
        var dados = Dados();
        var lote = new PedidoGeral
        {
            Id = Lotes.Count + 1,
            IdentificadorEnvio = envio.IdentificadorEnvio,
            VendedorId = _vendedorId,
            Status = "AguardandoConfirmacao",
            DataHoraCriacao = envio.DataHoraCriacao,
            Pedidos = envio.Pedidos.Select((p, i) =>
            {
                var pedido = new Pedido
                {
                    Id = (Lotes.Count + 1) * 100 + i,
                    ClienteId = p.ClienteId,
                    ClienteNome = $"Cliente {p.ClienteId}",
                    DataHoraCriacao = p.DataHoraCriacao,
                    Status = "Pendente",
                    Itens = p.Itens.Select(x => new ItemPedido { ProdutoId = x.ProdutoId, NomeProduto = "Produto de teste", QuantidadeSolicitada = x.QuantidadeSolicitada, PrecoUnitario = x.PrecoUnitario }).ToList()
                };
                RegrasPedido.AplicarCondicao(pedido, dados.CondicoesPagamento.Single(c => c.Id == p.CondicaoPagamentoId));
                foreach (var parcela in pedido.Parcelas)
                {
                    parcela.FormaPagamentoId = p.Parcelas.Single(x => x.NumeroParcela == parcela.NumeroParcela).FormaPagamentoId;
                    parcela.FormaPagamentoNome = dados.FormasPagamento.Single(f => f.Id == parcela.FormaPagamentoId).Descricao;
                }

                return pedido;
            }).ToList()
        };
        Lotes.Add(lote);
        if (FalharDepoisDeGravar)
        {
            FalharDepoisDeGravar = false;
            throw new ApiException(0, "Resposta perdida");
        }

        return Task.FromResult(JsonDados.Copiar(lote));
    }
}
