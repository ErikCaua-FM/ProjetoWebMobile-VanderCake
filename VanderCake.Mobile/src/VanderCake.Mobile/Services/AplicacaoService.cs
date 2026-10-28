using VanderCake.Contracts;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;

namespace VanderCake.Services;
// Uma única fila de operações evita que sincronização, edição e envio se sobreponham.
public sealed class AplicacaoService
{
    private readonly BancoLocal _banco;
    private readonly IApiService _api;
    private readonly ISessaoSegura _seguro;
    private readonly SemaphoreSlim _operacao = new(1, 1);
    private string? _refreshToken;
    private DateTimeOffset _expiraEm = DateTimeOffset.MaxValue;
    public event EventHandler? SessaoAlterada;
    public Vendedor? VendedorAtual
    {
        get; private set;
    }
    public bool TemSessao => VendedorAtual is not null;

    public AplicacaoService(BancoLocal banco, IApiService api, ISessaoSegura seguro)
    {
        _banco = banco;
        _api = api;
        _seguro = seguro;
    }

    private int VendedorId => VendedorAtual?.Id ?? throw new ApiException(401, "Entre na sua conta.");

    private async Task<T> ExecutarAsync<T>(Func<Task<T>> acao)
    {
        await _operacao.WaitAsync();
        try
        {
            try { return await acao(); }
            catch (ApiException ex) when (ex.Status == 401 && TemSessao && !string.IsNullOrEmpty(_refreshToken))
            {
                await RenovarInternoAsync();
                return await acao();
            }
        }
        catch (ApiException ex) when (ex.Status == 401)
        {
            VendedorAtual = null;
            _api.Token = null;
            _refreshToken = null;
            await _seguro.LimparAsync();
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
            throw;
        }
        finally
        {
            _operacao.Release();
        }
    }

    public Task InicializarAsync() => ExecutarAsync(async () =>
    {
        await _banco.InicializarAsync();
        string? json = await _seguro.LerAsync();
        if (json is not null)
        {
            try
            {
                var sessao = JsonDados.Ler<SessaoSalva>(json);
                if (sessao.Vendedor.Id <= 0 || !sessao.Vendedor.Ativo || string.IsNullOrWhiteSpace(sessao.AccessToken))
                    throw new InvalidDataException("Sessão inválida.");
                VendedorAtual = sessao.Vendedor;
                _api.Token = sessao.AccessToken;
                _refreshToken = sessao.RefreshToken;
                _expiraEm = sessao.ExpiraEm;
                SessaoAlterada?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex) when (ex is System.Text.Json.JsonException or InvalidDataException)
            {
                await _seguro.LimparAsync();
            }
        }

        return true;
    });
    public Task EntrarAsync(string email, string senha) => ExecutarAsync(async () =>
    {
        if (!System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _) || string.IsNullOrWhiteSpace(senha))
            throw new InvalidOperationException("Informe um e-mail válido e sua senha.");
        var resposta = await _api.EntrarAsync(new LoginRequisicao { Email = email.Trim(), Senha = senha });
        if (resposta.Vendedor.Id <= 0 || !resposta.Vendedor.Ativo || string.IsNullOrWhiteSpace(resposta.AccessToken))
            throw new ApiException(0, "A API não retornou uma sessão de vendedor válida.");
        var dados = await _banco.LerDadosAsync(resposta.Vendedor.Id) ?? new DadosSincronizacao();
        dados.Vendedor = resposta.Vendedor;
        await _banco.SalvarDadosAsync(dados);
        await GuardarSessaoAsync(resposta);
        SessaoAlterada?.Invoke(this, EventArgs.Empty);
        return true;
    });
    public Task SairAsync() => ExecutarAsync(async () =>
    {
        await _seguro.LimparAsync();
        VendedorAtual = null;
        _api.Token = null;
        _refreshToken = null;
        SessaoAlterada?.Invoke(this, EventArgs.Empty);
        // Não apagamos rascunhos não enviados. Cada consulta é filtrada pelo vendedor.
        return true;
    });
    public Task<DadosSincronizacao> ObterDadosAsync() => ExecutarAsync(LerDadosInternoAsync);
    public Task<string?> ObterTokenAsync() => ExecutarAsync(async () =>
    {
        if (TemSessao && _expiraEm <= DateTimeOffset.UtcNow.AddMinutes(1) && !string.IsNullOrEmpty(_refreshToken))
            await RenovarInternoAsync();
        return _api.Token;
    });
    private async Task RenovarInternoAsync()
    {
        var resposta = await _api.RenovarAsync(_refreshToken!);
        if (resposta.Vendedor.Id != VendedorId || !resposta.Vendedor.Ativo || string.IsNullOrWhiteSpace(resposta.AccessToken))
            throw new ApiException(401, "Sessão inválida. Entre novamente.");
        await GuardarSessaoAsync(resposta);
    }
    private async Task GuardarSessaoAsync(LoginResposta resposta)
    {
        var sessao = new SessaoSalva
        {
            AccessToken = resposta.AccessToken, RefreshToken = resposta.RefreshToken, Vendedor = resposta.Vendedor,
            ExpiraEm = resposta.ExpiresIn > 0 ? DateTimeOffset.UtcNow.AddSeconds(resposta.ExpiresIn) : DateTimeOffset.MaxValue
        };
        await _seguro.SalvarAsync(JsonDados.Escrever(sessao));
        VendedorAtual = sessao.Vendedor;
        _api.Token = sessao.AccessToken;
        _refreshToken = sessao.RefreshToken;
        _expiraEm = sessao.ExpiraEm;
    }
    private async Task<DadosSincronizacao> LerDadosInternoAsync() => await _banco.LerDadosAsync(VendedorId) ?? new DadosSincronizacao
    {
        Vendedor = VendedorAtual!
    };
    public Task AtualizarAsync() => ExecutarAsync(async () =>
    {
        int vendedorId = VendedorId;
        var dados = await _api.SincronizarAsync();
        if (dados.Vendedor.Id != vendedorId || !dados.Vendedor.Ativo)
            throw new ApiException(401, "A sessão não corresponde a um vendedor ativo.");
        if (dados.Clientes.Any(c => c.VendedorId != vendedorId))
            throw new ApiException(0, "A API retornou clientes de outra carteira.");
        foreach (var lote in dados.PedidosGerais)
        {
            ValidarLoteServidor(lote, vendedorId);
            var local = await _banco.LerLoteAsync(lote.IdentificadorEnvio);
            if (local?.SituacaoLocal == "EnvioIncerto")
                ValidarCorrespondencia(lote, JsonDados.Ler<PedidoGeral>(local.ConteudoJson));
        }

        dados.SincronizadoEm = DateTimeOffset.UtcNow;
        await _banco.AplicarSincronizacaoAsync(dados);
        VendedorAtual = dados.Vendedor;
        return true;
    });
    public Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao novo) => ExecutarAsync(async () =>
    {
        ValidacaoCliente.Validar(novo);
        var dados = await LerDadosInternoAsync();
        if (!dados.Cidades.Any(c => c.Id == novo.CidadeId))
            throw new InvalidOperationException("Selecione uma cidade sincronizada.");
        if (dados.Clientes.Any(c => ValidacaoCliente.Digitos(c.Documento) == novo.Documento))
            throw new InvalidOperationException("Este CPF/CNPJ já está na sua carteira.");
        var cliente = await _api.CadastrarClienteAsync(novo);
        if (cliente.Id <= 0 || cliente.VendedorId != VendedorId)
            throw new ApiException(0, "A API retornou um cadastro de cliente incompatível. Atualize a carteira antes de tentar novamente.");
        dados.Clientes.RemoveAll(c => c.Id == cliente.Id);
        dados.Clientes.Add(cliente);
        await _banco.SalvarDadosAsync(dados);
        return cliente;
    });
    private async Task<PedidoGeralRegistro?> LoteAbertoInternoAsync()
    {
        var lotes = await _banco.LerLotesAsync(VendedorId);
        return lotes.SingleOrDefault(l => l.SituacaoLocal != "Sincronizado");
    }

    private async Task GarantirEdicaoAsync()
    {
        if ((await LoteAbertoInternoAsync())?.SituacaoLocal == "EnvioIncerto")
            throw new InvalidOperationException("Existe um envio a verificar. Abra Lista de Pedidos e retome o envio antes de editar.");
    }

    public Task<PedidoGeralRegistro?> ObterLoteAbertoAsync() => ExecutarAsync(LoteAbertoInternoAsync);
    public Task<List<PedidoResumo>> ObterPedidosClienteAsync(int clienteId) => ExecutarAsync(() => LerPedidosClienteInternoAsync(clienteId));
    private async Task<List<PedidoResumo>> LerPedidosClienteInternoAsync(int clienteId)
    {
        var lotes = await _banco.LerLotesAsync(VendedorId);
        return lotes.SelectMany(r => JsonDados.Ler<PedidoGeral>(r.ConteudoJson).Pedidos.Where(p => p.ClienteId == clienteId).Select(p => new PedidoResumo { IdentificadorEnvio = r.IdentificadorEnvio, Pedido = p, SituacaoLocal = r.SituacaoLocal, MotivoCancelamento = JsonDados.Ler<PedidoGeral>(r.ConteudoJson).MotivoCancelamento })).OrderByDescending(p => p.Pedido.DataHoraCriacao).ThenByDescending(p => p.Pedido.Id).ThenBy(p => p.IdentificadorEnvio, StringComparer.Ordinal).ToList();
    }

    public Task<PedidoResumo?> ObterUltimoPedidoClienteAsync(int clienteId) => ExecutarAsync(async () =>
        (await LerPedidosClienteInternoAsync(clienteId)).FirstOrDefault());

    public Task ComprarNovamenteAsync(int clienteId, string identificadorOrigem, bool substituirCarrinho = false) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var dados = await LerDadosInternoAsync();
        var cliente = dados.Clientes.SingleOrDefault(c => c.Id == clienteId && c.Ativo && c.VendedorId == VendedorId)
            ?? throw new InvalidOperationException("O cliente não está ativo na sua carteira.");
        var ultimo = (await LerPedidosClienteInternoAsync(clienteId)).FirstOrDefault()
            ?? throw new InvalidOperationException("Este cliente ainda não possui um pedido anterior para repetir.");
        if (ultimo.IdentificadorEnvio != identificadorOrigem)
            throw new InvalidOperationException("O último pedido mudou. Atualize o catálogo e confira os itens antes de comprar novamente.");

        var registro = await _banco.LerCarrinhoAsync(VendedorId, clienteId);
        var carrinho = registro is null ? null : JsonDados.Ler<Pedido>(registro.ConteudoJson);
        if (!substituirCarrinho && carrinho is not null &&
            (carrinho.Itens.Count > 0 || carrinho.CondicaoPagamentoId != 0 || carrinho.Parcelas.Count > 0))
            throw new InvalidOperationException("Confirme a substituição dos itens e do pagamento do carrinho atual.");

        var anteriores = ultimo.Pedido.Itens;
        if (anteriores.Count == 0 || anteriores.Any(i => i.QuantidadeSolicitada <= 0) ||
            anteriores.Select(i => i.ProdutoId).Distinct().Count() != anteriores.Count)
            throw new InvalidOperationException("O pedido anterior não possui itens válidos para repetir. Escolha os produtos no catálogo.");

        var indisponiveis = anteriores.Where(i => !dados.Produtos.Any(p => p.Id == i.ProdutoId && p.Ativo)).ToList();
        if (indisponiveis.Count > 0)
            throw new InvalidOperationException($"Não foi possível repetir o pedido. Produtos indisponíveis: {string.Join(", ", indisponiveis.Select(i => i.NomeProduto))}. O carrinho foi preservado; escolha os produtos no catálogo.");

        // Criar outro objeto evita copiar pagamento, atendimento, status e identificador do pedido anterior.
        var novo = new Pedido
        {
            ClienteId = cliente.Id,
            ClienteNome = cliente.Nome
        };
        foreach (var anterior in anteriores)
        {
            var produto = dados.Produtos.Single(p => p.Id == anterior.ProdutoId && p.Ativo);
            if (produto.Preco <= 0 || decimal.Round(produto.Preco, 2) != produto.Preco)
                throw new InvalidOperationException($"O preço de {produto.Nome} está inválido. Atualize o catálogo antes de comprar novamente.");
            novo.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                QuantidadeSolicitada = anterior.QuantidadeSolicitada,
                PrecoUnitario = produto.Preco
            });
        }

        RegrasPedido.Recalcular(novo);
        // Uma única gravação: nenhuma falha acima substitui parcialmente o carrinho existente.
        await _banco.SalvarCarrinhoAsync(VendedorId, novo);
        return true;
    });
    public Task<PedidoResumo> ObterDetalheAsync(string identificador, int clienteId) => ExecutarAsync(async () =>
    {
        var registro = await _banco.LerLoteAsync(identificador);
        if (registro is null || registro.VendedorId != VendedorId)
            throw new InvalidOperationException("Pedido não encontrado nesta conta.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        return new PedidoResumo
        {
            IdentificadorEnvio = identificador,
            SituacaoLocal = registro.SituacaoLocal,
            Pedido = lote.Pedidos.Single(p => p.ClienteId == clienteId),
            MotivoCancelamento = lote.MotivoCancelamento
        };
    });
    public Task<Pedido> AbrirCarrinhoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var existente = await _banco.LerCarrinhoAsync(VendedorId, clienteId);
        if (existente is not null)
            return JsonDados.Ler<Pedido>(existente.ConteudoJson);
        var dados = await LerDadosInternoAsync();
        var cliente = dados.Clientes.SingleOrDefault(c => c.Id == clienteId && c.Ativo) ?? throw new InvalidOperationException("O cliente não está ativo na carteira sincronizada.");
        var registro = await LoteAbertoInternoAsync();
        var lote = registro is null ? null : JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        var pedido = lote?.Pedidos.SingleOrDefault(p => p.ClienteId == clienteId) ?? new Pedido
        {
            ClienteId = clienteId,
            ClienteNome = cliente.Nome
        };
        await _banco.SalvarCarrinhoAsync(VendedorId, pedido);
        return pedido;
    });
    private async Task<Pedido> LerCarrinhoInternoAsync(int clienteId)
    {
        var registro = await _banco.LerCarrinhoAsync(VendedorId, clienteId) ?? throw new InvalidOperationException("Abra um novo pedido para este cliente.");
        return JsonDados.Ler<Pedido>(registro.ConteudoJson);
    }

    public Task<Pedido> ObterCarrinhoAsync(int clienteId) => ExecutarAsync(() => LerCarrinhoInternoAsync(clienteId));
    private Task EditarCarrinhoAsync(int clienteId, Func<Pedido, DadosSincronizacao, Task> editar) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var pedido = await LerCarrinhoInternoAsync(clienteId);
        await editar(pedido, await LerDadosInternoAsync());
        RegrasPedido.Recalcular(pedido);
        await _banco.SalvarCarrinhoAsync(VendedorId, pedido);
        return true;
    });
    public Task AdicionarProdutoAsync(int clienteId, int produtoId, int quantidade) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        if (quantidade <= 0)
            throw new InvalidOperationException("Informe uma quantidade inteira positiva.");
        var produto = dados.Produtos.SingleOrDefault(p => p.Id == produtoId && p.Ativo) ?? throw new InvalidOperationException("Produto indisponível.");
        var item = pedido.Itens.SingleOrDefault(i => i.ProdutoId == produtoId);
        if (item is null)
            pedido.Itens.Add(new ItemPedido { ProdutoId = produto.Id, NomeProduto = produto.Nome, PrecoUnitario = produto.Preco, QuantidadeSolicitada = quantidade });
        else
            item.QuantidadeSolicitada = checked(item.QuantidadeSolicitada + quantidade);
        return Task.CompletedTask;
    });
    public Task AlterarQuantidadeAsync(int clienteId, int produtoId, int quantidade) => EditarCarrinhoAsync(clienteId, (pedido, _) =>
    {
        if (quantidade < 0)
            throw new InvalidOperationException("Quantidade inválida.");
        var item = pedido.Itens.Single(i => i.ProdutoId == produtoId);
        if (quantidade == 0)
            pedido.Itens.Remove(item);
        else
            item.QuantidadeSolicitada = quantidade;
        return Task.CompletedTask;
    });
    public Task SelecionarCondicaoAsync(int clienteId, int condicaoId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        RegrasPedido.AplicarCondicao(pedido, dados.CondicoesPagamento.Single(c => c.Id == condicaoId));
        return Task.CompletedTask;
    });
    public Task SelecionarFormaAsync(int clienteId, int numeroParcela, int formaId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        var forma = dados.FormasPagamento.Single(f => f.Id == formaId && f.Ativo);
        var parcela = pedido.Parcelas.Single(p => p.NumeroParcela == numeroParcela);
        parcela.FormaPagamentoId = forma.Id;
        parcela.FormaPagamentoNome = forma.Descricao;
        return Task.CompletedTask;
    });
    public Task AtualizarPrecosAsync(int clienteId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        foreach (var item in pedido.Itens)
        {
            var atual = dados.Produtos.SingleOrDefault(p => p.Id == item.ProdutoId && p.Ativo) ?? throw new InvalidOperationException($"Remova o produto indisponível: {item.NomeProduto}.");
            item.PrecoUnitario = atual.Preco;
            item.NomeProduto = atual.Nome;
        }

        return Task.CompletedTask;
    });
    public Task DescartarCarrinhoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        await _banco.ExcluirCarrinhoAsync(VendedorId, clienteId);
        return true;
    });
    public Task AcrescentarPedidoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var pedido = await LerCarrinhoInternoAsync(clienteId);
        RegrasPedido.Validar(pedido, await LerDadosInternoAsync());
        var registro = await LoteAbertoInternoAsync();
        var lote = registro is null ? new PedidoGeral
        {
            VendedorId = VendedorId
        }

        : JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        lote.Pedidos.RemoveAll(p => p.ClienteId == clienteId);
        lote.Pedidos.Add(pedido);
        registro ??= new PedidoGeralRegistro
        {
            IdentificadorEnvio = lote.IdentificadorEnvio,
            VendedorId = VendedorId
        };
        registro.ConteudoJson = JsonDados.Escrever(lote);
        await _banco.ConcluirCarrinhoAsync(registro, clienteId);
        return true;
    });
    public Task RemoverPedidoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var registro = await LoteAbertoInternoAsync() ?? throw new InvalidOperationException("A lista está vazia.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        lote.Pedidos.RemoveAll(p => p.ClienteId == clienteId);
        registro.ConteudoJson = JsonDados.Escrever(lote);
        await _banco.ConcluirCarrinhoAsync(registro, clienteId);
        return true;
    });
    public Task<List<Pedido>> ObterCarrinhosPendentesAsync() => ExecutarAsync(async () => (await _banco.LerCarrinhosAsync(VendedorId)).Select(c => JsonDados.Ler<Pedido>(c.ConteudoJson)).ToList());
    public Task<PedidoGeral> EnviarPedidosAsync() => ExecutarAsync(async () =>
    {
        var registro = await LoteAbertoInternoAsync() ?? throw new InvalidOperationException("A lista está vazia.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        bool retomada = registro.SituacaoLocal == "EnvioIncerto";
        if (!retomada)
        {
            if (lote.Pedidos.Count == 0)
                throw new InvalidOperationException("Acrescente pelo menos um pedido à lista.");
            if ((await _banco.LerCarrinhosAsync(VendedorId)).Count > 0)
                throw new InvalidOperationException("Há carrinhos em edição. Conclua ou descarte os carrinhos indicados na lista antes de enviar.");
            var dados = await LerDadosInternoAsync();
            foreach (var pedido in lote.Pedidos)
                RegrasPedido.Validar(pedido, dados);
            registro.CorpoEnvioJson = JsonDados.Escrever(RegrasPedido.CriarEnvio(lote));
            registro.SituacaoLocal = "EnvioIncerto";
            // Gravar ANTES da requisição permite retomar mesmo após o aplicativo fechar.
            await _banco.SalvarLoteAsync(registro);
        }

        try
        {
            PedidoGeral? recebido = retomada ? await _api.ConsultarLoteAsync(lote.IdentificadorEnvio) : null;
            // Um 404 não libera edição: repetimos o mesmo corpo e o mesmo identificador.
            recebido ??= await _api.EnviarLoteAsync(registro.CorpoEnvioJson ?? throw new InvalidDataException("O corpo do envio não está disponível."));
            ValidarLoteServidor(recebido, VendedorId);
            ValidarCorrespondencia(recebido, lote);
            registro.SituacaoLocal = "Sincronizado";
            registro.ConteudoJson = JsonDados.Escrever(recebido);
            registro.CorpoEnvioJson = null;
            await _banco.SalvarLoteAsync(registro);
            return recebido;
        }
        catch (ApiException ex) when ((ex.Status is 400 or 409 or 422) && ex.LoteNaoGravado)
        {
            // Somente uma rejeição definitiva da API permite corrigir o lote.
            string identificadorAnterior = registro.IdentificadorEnvio;
            lote.IdentificadorEnvio = Guid.NewGuid().ToString();
            registro.IdentificadorEnvio = lote.IdentificadorEnvio;
            registro.ConteudoJson = JsonDados.Escrever(lote);
            registro.SituacaoLocal = "Montagem";
            registro.CorpoEnvioJson = null;
            await _banco.SubstituirLoteRejeitadoAsync(identificadorAnterior, registro);
            throw;
        }
    });
    private static void ValidarLoteServidor(PedidoGeral lote, int vendedorId)
    {
        if (lote.Id <= 0 || lote.VendedorId != vendedorId || !Guid.TryParse(lote.IdentificadorEnvio, out _) || lote.Status is not ("AguardandoConfirmacao" or "Processado" or "Cancelado") || lote.Pedidos.Count == 0 || lote.Pedidos.Any(p => p.Id <= 0) || lote.Pedidos.Select(p => p.ClienteId).Distinct().Count() != lote.Pedidos.Count)
            throw new ApiException(0, "A API retornou um lote incompatível. O envio local será preservado para verificação.");
    }

    private static void ValidarCorrespondencia(PedidoGeral recebido, PedidoGeral local)
    {
        // Quantidades solicitadas e pagamentos permanecem iguais mesmo após o corte.
        if (recebido.IdentificadorEnvio != local.IdentificadorEnvio || !recebido.Pedidos.Select(p => p.ClienteId).Order().SequenceEqual(local.Pedidos.Select(p => p.ClienteId).Order()))
            throw new ApiException(0, "A confirmação não corresponde ao lote enviado.");
        foreach (var pedido in local.Pedidos)
        {
            var servidor = recebido.Pedidos.Single(p => p.ClienteId == pedido.ClienteId);
            var a = pedido.Itens.OrderBy(i => i.ProdutoId).Select(i => (i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario));
            var b = servidor.Itens.OrderBy(i => i.ProdutoId).Select(i => (i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario));
            if (!a.SequenceEqual(b) || pedido.CondicaoPagamentoId != servidor.CondicaoPagamentoId || !pedido.Parcelas.Select(p => (p.NumeroParcela, p.FormaPagamentoId)).Order().SequenceEqual(servidor.Parcelas.Select(p => (p.NumeroParcela, p.FormaPagamentoId)).Order()))
                throw new ApiException(0, "A API alterou os dados solicitados. Consulte o resultado antes de continuar.");
        }
    }
}
