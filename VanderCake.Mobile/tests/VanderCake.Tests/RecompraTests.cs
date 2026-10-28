using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;
using VanderCake.Services;

internal static class RecompraTests
{
    public static async Task ExecutarAsync(Action<bool, string> verificar)
    {
        async Task EsperarBloqueio(Func<Task> acao, string descricao)
        {
            bool bloqueado = false;
            try
            {
                await acao();
            }
            catch (InvalidOperationException)
            {
                bloqueado = true;
            }
            verificar(bloqueado, descricao);
        }

        var caminho = Path.Combine(Path.GetTempPath(), $"vandercake-recompra-{Guid.NewGuid():N}.db3");
        var banco = new BancoLocal(caminho);
        var api = new ApiDeTeste();
        var seguro = new SessaoEmMemoria();
        var sistema = new AplicacaoService(banco, api, seguro);
        try
        {
            await sistema.InicializarAsync();
            await sistema.EntrarAsync("vendedor1@teste.local", "senha");
            await sistema.AtualizarAsync();
            verificar(await sistema.ObterUltimoPedidoClienteAsync(10) is null, "Sem histórico, a consulta de recompra não retorna pedido");
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, Guid.NewGuid().ToString()),
                "Não é possível repetir um pedido inexistente");

            var dados = await sistema.ObterDadosAsync();
            dados.Produtos[0].Nome = "Farinha atual";
            dados.Produtos[0].Preco = 12m;
            dados.Produtos.Add(new Produto { Id = 101, Nome = "Chocolate", Preco = 3.50m, Estoque = 0 });
            await banco.SalvarDadosAsync(dados);

            Pedido CriarPedido(int id, int clienteId, int dias, int quantidade) => new()
            {
                Id = id,
                ClienteId = clienteId,
                ClienteNome = $"Cliente {clienteId}",
                DataHoraCriacao = DateTimeOffset.UtcNow.AddDays(-dias),
                Status = "Pendente",
                Itens = [new ItemPedido
                {
                    ProdutoId = 100,
                    NomeProduto = "Farinha antiga",
                    PrecoUnitario = 10m,
                    QuantidadeSolicitada = quantidade
                }]
            };

            var anterior = CriarPedido(101, 10, 2, 10);
            anterior.Itens[0].QuantidadeAtendida = 6;
            anterior.Itens.Add(new ItemPedido
            {
                ProdutoId = 101,
                NomeProduto = "Chocolate",
                PrecoUnitario = 2.50m,
                QuantidadeSolicitada = 3,
                QuantidadeAtendida = 1
            });
            anterior.Status = "ConfirmadoComCorte";
            anterior.ValorTotalAtendido = 62.50m;
            RegrasPedido.AplicarCondicao(anterior, dados.CondicoesPagamento.Single(c => c.Id == 3));
            foreach (var parcela in anterior.Parcelas)
            {
                parcela.FormaPagamentoId = parcela.NumeroParcela;
                parcela.FormaPagamentoNome = dados.FormasPagamento.Single(f => f.Id == parcela.FormaPagamentoId).Descricao;
                parcela.DataVencimento = DateTime.Today.AddDays(parcela.DiasAposConfirmacao);
                parcela.ValorFinal = 20m;
                parcela.Situacao = "Pago";
            }

            var antigo = new PedidoGeral { Id = 1, VendedorId = 1, Pedidos = [CriarPedido(999, 10, 3, 99)] };
            var origem = new PedidoGeral { Id = 2, VendedorId = 1, Pedidos = [anterior] };
            var outroCliente = new PedidoGeral { Id = 3, VendedorId = 1, Pedidos = [CriarPedido(300, 11, 1, 88)] };
            var outroVendedor = new PedidoGeral { Id = 4, VendedorId = 2, Pedidos = [CriarPedido(400, 10, 0, 77)] };
            foreach (var lote in new[] { antigo, origem, outroCliente, outroVendedor })
            {
                await banco.SalvarLoteAsync(new PedidoGeralRegistro
                {
                    VendedorId = lote.VendedorId,
                    IdentificadorEnvio = lote.IdentificadorEnvio,
                    SituacaoLocal = "Sincronizado",
                    ConteudoJson = JsonDados.Escrever(lote)
                });
            }

            var ultimo = await sistema.ObterUltimoPedidoClienteAsync(10);
            verificar(ultimo?.IdentificadorEnvio == origem.IdentificadorEnvio,
                "Recompra escolhe pela data e isola o cliente e o vendedor");
            verificar((await sistema.ObterUltimoPedidoClienteAsync(11))?.IdentificadorEnvio == outroCliente.IdentificadorEnvio,
                "Cada cliente possui sua própria sugestão de recompra");
            string historicoAntes = JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10));
            await sistema.AbrirCarrinhoAsync(10);
            await sistema.AdicionarProdutoAsync(10, 100, 1);
            await sistema.SelecionarCondicaoAsync(10, 1);
            await sistema.SelecionarFormaAsync(10, 1, 1);
            string carrinhoAntes = JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10));
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio),
                "Carrinho preenchido exige confirmação de substituição");
            verificar(JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10)) == carrinhoAntes,
                "Sem confirmação, os itens e o pagamento existentes são preservados");

            DateTimeOffset inicio = DateTimeOffset.UtcNow;
            await sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true);
            var copiado = await sistema.ObterCarrinhoAsync(10);
            verificar(copiado.Itens.Select(i => (i.ProdutoId, i.QuantidadeSolicitada))
                .SequenceEqual(new[] { (100, 10), (101, 3) }),
                "Recompra copia todos os produtos e as quantidades solicitadas, inclusive antes dos cortes");
            verificar(copiado.Itens[0].NomeProduto == "Farinha atual" && copiado.Itens[0].PrecoUnitario == 12m &&
                copiado.Itens[1].PrecoUnitario == 3.50m && copiado.ValorTotalSolicitado == 130.50m,
                "Recompra usa os nomes e preços atuais e recalcula o total");
            verificar(copiado.CondicaoPagamentoId == 0 && copiado.CondicaoPagamentoDescricao == "" && copiado.Parcelas.Count == 0,
                "Condição, formas, vencimentos e valores antigos das parcelas não são copiados");
            verificar(copiado.Id == 0 && copiado.Status == "Rascunho" && copiado.DataHoraCriacao >= inicio &&
                copiado.ValorTotalAtendido is null && copiado.Itens.All(i => i.QuantidadeAtendida is null),
                "Recompra cria rascunho com nova data, sem IDs oficiais nem atendimento anterior");
            verificar(JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10)) == historicoAntes &&
                await sistema.ObterLoteAbertoAsync() is null && api.Posts == 0,
                "Preencher carrinho não altera histórico, não cria lote nem envia à API");
            await EsperarBloqueio(() => sistema.AcrescentarPedidoAsync(10),
                "Recompra não pode ser concluída sem escolher novamente o pagamento");
            await sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true);
            verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 130.50m,
                "Repetir a ação substitui o carrinho sem somar quantidades duplicadas");

            await banco.FecharAsync();
            banco = new BancoLocal(caminho);
            sistema = new AplicacaoService(banco, api, seguro);
            await sistema.InicializarAsync();
            var restaurado = await sistema.ObterCarrinhoAsync(10);
            verificar(restaurado.Itens.Count == 2 && restaurado.ValorTotalSolicitado == 130.50m &&
                restaurado.CondicaoPagamentoId == 0 && restaurado.Parcelas.Count == 0,
                "Carrinho da recompra persiste no SQLite após reinício, com pagamento vazio");
            await sistema.SelecionarCondicaoAsync(10, 3);
            verificar((await sistema.ObterCarrinhoAsync(10)).Parcelas.All(p => p.FormaPagamentoId == 0),
                "Selecionar 7/14/21 após recompra exige escolher cada forma de pagamento");
            for (int numero = 1; numero <= 3; numero++)
                await sistema.SelecionarFormaAsync(10, numero, numero);
            string antesDoBloqueio = JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10));

            dados.Produtos[1].Ativo = false;
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Produto inativo impede copiar parcialmente o pedido");
            var produtoRemovido = dados.Produtos[1];
            dados.Produtos.RemoveAt(1);
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Produto ausente no catálogo também impede a recompra incompleta");
            verificar(JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10)) == antesDoBloqueio,
                "Indisponibilidade preserva integralmente o carrinho e seu pagamento");
            produtoRemovido.Ativo = true;
            dados.Produtos.Add(produtoRemovido);
            dados.Clientes[0].Ativo = false;
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Recompra exige cliente ativo na carteira atual");
            dados.Clientes[0].Ativo = true;
            await banco.SalvarDadosAsync(dados);

            await sistema.AcrescentarPedidoAsync(10);
            var local = await sistema.ObterUltimoPedidoClienteAsync(10);
            verificar(local?.SituacaoLocal == "Montagem" && local.Pedido.Itens.Count == 2 &&
                local.Pedido.Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }),
                "Após escolher pagamento, recompra é acrescentada à lista e vira o último pedido salvo");
            await sistema.AbrirCarrinhoAsync(10);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Sugestão antiga é recusada se outro pedido passou a ser o mais recente");
            string historicoLocal = JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10));
            await sistema.ComprarNovamenteAsync(10, local!.IdentificadorEnvio, true);
            verificar((await sistema.ObterCarrinhoAsync(10)).Parcelas.Count == 0 &&
                JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10)) == historicoLocal,
                "Repetir pedido ainda local limpa apenas o pagamento do carrinho e preserva a lista");

            await sistema.SairAsync();
            await sistema.EntrarAsync("vendedor2@teste.local", "senha");
            await sistema.AtualizarAsync();
            verificar((await sistema.ObterUltimoPedidoClienteAsync(10))?.IdentificadorEnvio == outroVendedor.IdentificadorEnvio,
                "Trocar vendedor não expõe a sugestão de recompra da outra conta");
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Chave de pedido de outro vendedor não pode ser usada para recompra");
        }
        finally
        {
            await banco.FecharAsync();
            File.Delete(caminho);
        }
    }
}
