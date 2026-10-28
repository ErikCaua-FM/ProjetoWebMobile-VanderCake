using System.Data;
using Dapper;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class PedidosService(Banco banco, LeituraService leitura, RateioService rateio, IConfiguration configuracao)
{
    public async Task<PedidoGeral> ReceberAsync(int vendedorId, PedidoGeralEnvio entrada)
    {
        var envio = Regras.Canonico(entrada);
        string hash = Regras.Hash(envio);
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        // O índice único faz requisições simultâneas do mesmo UUID aguardarem esta transação.
        await c.ExecuteAsync("""
            INSERT IGNORE INTO ControleEnvios (IdentificadorEnvio,VendedorId,ConteudoHash,Status,CriadoEm)
            VALUES (@IdentificadorEnvio,@vendedorId,@hash,'EmProcessamento',UTC_TIMESTAMP(6))
            """, new { envio.IdentificadorEnvio, vendedorId, hash }, tx);
        var controle = await c.QuerySingleAsync<ControleEnvio>(
            "SELECT * FROM ControleEnvios WHERE IdentificadorEnvio=@IdentificadorEnvio FOR UPDATE", envio, tx);
        Regras.Exigir(controle.VendedorId == vendedorId, "Identificador de envio indisponível.", 409);
        Regras.Exigir(controle.ConteudoHash == hash, "O identificador já foi utilizado com outro conteúdo. Verifique o envio original.", 409);
        if (controle.Status == "Rejeitado")
        {
            await tx.CommitAsync();
            throw new ApiException(controle.CodigoRejeicao ?? 422, controle.MotivoRejeicao ?? "Envio rejeitado.", true);
        }
        if (controle.Status == "Aceito")
        {
            await tx.CommitAsync();
            return await PorIdentificadorAsync(vendedorId, envio.IdentificadorEnvio);
        }

        var lote = new PedidoGeral
        {
            VendedorId = vendedorId, IdentificadorEnvio = envio.IdentificadorEnvio,
            DataHoraCriacao = envio.DataHoraCriacao.UtcDateTime, DataHoraRecebimento = DateTime.UtcNow
        };
        try
        {
            var vendedor = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@vendedorId FOR SHARE", new { vendedorId }, tx);
            Regras.Exigir(vendedor?.Ativo == true, "Vendedor inativo.", 401);
            lote.VendedorNome = vendedor!.Nome;
            Regras.Exigir(envio.DataHoraCriacao.Year >= 2000 && envio.DataHoraCriacao <= DateTimeOffset.UtcNow.AddMinutes(5), "Data de criação do envio inválida.");
            Regras.Exigir(envio.Pedidos.Select(p => p.ClienteId).Distinct().Count() == envio.Pedidos.Count, "Só pode existir um pedido por cliente neste lote.");
            var condicoesIds = envio.Pedidos.Select(p => p.CondicaoPagamentoId).Distinct().Order().ToArray();
            var formasIds = envio.Pedidos.SelectMany(p => p.Parcelas).Select(p => p.FormaPagamentoId).Distinct().Order().ToArray();
            // Uma edição de pagamento não pode mudar o significado de uma parcela durante o envio.
            await c.QueryAsync<int>("SELECT Id FROM CondicoesPagamento WHERE Id IN @condicoesIds ORDER BY Id FOR SHARE", new { condicoesIds }, tx);
            await c.QueryAsync<int>("SELECT Id FROM FormasPagamento WHERE Id IN @formasIds ORDER BY Id FOR SHARE", new { formasIds }, tx);
            var dados = await LeituraService.LerAsync(c, tx, vendedorId);
            // Bloqueios compartilhados preservam preço/atividade/carteira até o fim do recebimento.
            var clientesIds = envio.Pedidos.Select(p => p.ClienteId).Distinct().Order().ToArray();
            var clientes = (await c.QueryAsync<Cliente>("SELECT * FROM Clientes WHERE Id IN @clientesIds ORDER BY Id FOR SHARE", new { clientesIds }, tx)).ToDictionary(p => p.Id);
            var produtosIds = envio.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct().Order().ToArray();
            var produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos WHERE Id IN @produtosIds ORDER BY Id FOR SHARE", new { produtosIds }, tx)).ToDictionary(p => p.Id);
            foreach (var requisicao in envio.Pedidos)
            {
                Regras.Exigir(clientes.TryGetValue(requisicao.ClienteId, out var cliente) && cliente.Ativo && cliente.VendedorId == vendedorId,
                    "Um cliente não está ativo na sua carteira. Atualize os dados.");
                Regras.Exigir(requisicao.DataHoraCriacao.Year >= 2000 && requisicao.DataHoraCriacao <= DateTimeOffset.UtcNow.AddMinutes(5), "Data do pedido inválida.");
                var condicao = dados.CondicoesPagamento.SingleOrDefault(x => x.Id == requisicao.CondicaoPagamentoId && x.Ativo);
                Regras.Exigir(condicao is not null, "Condição de pagamento inválida.");
                Regras.Exigir(new[] { "0", "7/14", "7/14/21" }.Contains(string.Join('/', condicao!.Parcelas.Select(p => p.DiasAposConfirmacao))), "Prazos de pagamento incompatíveis.");
                Regras.Exigir(requisicao.Parcelas.Select(p => p.NumeroParcela).SequenceEqual(condicao.Parcelas.Select(p => p.NumeroParcela)), "Informe a forma de pagamento de cada parcela da condição escolhida.");
                Regras.Exigir(requisicao.Itens.Select(i => i.ProdutoId).Distinct().Count() == requisicao.Itens.Count, "Produto repetido no mesmo pedido.");
                var pedido = new Pedido
                {
                    ClienteId = cliente!.Id, ClienteNome = cliente.Nome, DataHoraCriacao = requisicao.DataHoraCriacao.UtcDateTime,
                    CondicaoPagamentoId = condicao.Id, CondicaoPagamentoDescricao = condicao.Descricao
                };
                foreach (var item in requisicao.Itens)
                {
                    Regras.Exigir(item.QuantidadeSolicitada > 0, "A quantidade deve ser positiva.");
                    Regras.Exigir(produtos.TryGetValue(item.ProdutoId, out var produto) && produto.Ativo, "Um produto está indisponível. Atualize o catálogo.");
                    Regras.Exigir(item.PrecoUnitario == produto!.Preco, $"O preço de {produto.Nome} mudou. Revise o carrinho.", 409);
                    pedido.Itens.Add(new ItemPedido
                    {
                        ProdutoId = produto.Id, NomeProduto = produto.Nome,
                        PrecoUnitario = produto.Preco, QuantidadeSolicitada = item.QuantidadeSolicitada
                    });
                }
                pedido.ValorTotalSolicitado = pedido.Itens.Sum(i => i.PrecoUnitario * i.QuantidadeSolicitada);
                Regras.Exigir(pedido.ValorTotalSolicitado is > 0 and <= Regras.LimiteDinheiro, "Total do pedido fora do limite.");
                var valores = Regras.Parcelar(pedido.ValorTotalSolicitado, condicao.Parcelas.Count);
                for (int i = 0; i < valores.Length; i++)
                {
                    var forma = dados.FormasPagamento.SingleOrDefault(f => f.Id == requisicao.Parcelas[i].FormaPagamentoId && f.Ativo);
                    Regras.Exigir(forma is not null, "Forma de pagamento inválida em uma parcela.");
                    pedido.Parcelas.Add(new ParcelaPedido
                    {
                        NumeroParcela = i + 1, DiasAposConfirmacao = condicao.Parcelas[i].DiasAposConfirmacao,
                        FormaPagamentoId = forma!.Id, FormaPagamentoNome = forma.Descricao, ValorPrevisto = valores[i]
                    });
                }
                lote.Pedidos.Add(pedido);
            }
        }
        catch (ApiException e) when (e.Status is 400 or 409 or 422)
        {
            // Nenhum pedido foi inserido: a rejeição fica permanente para este UUID.
            await c.ExecuteAsync("""
                UPDATE ControleEnvios SET Status='Rejeitado',CodigoRejeicao=@Status,MotivoRejeicao=@Message
                WHERE IdentificadorEnvio=@IdentificadorEnvio
                """, new { e.Status, e.Message, envio.IdentificadorEnvio }, tx);
            await tx.CommitAsync();
            throw new ApiException(e.Status, e.Message, true);
        }

        lote.Id = await c.ExecuteScalarAsync<int>("""
            INSERT INTO PedidoGeral (IdentificadorEnvio,VendedorId,DataHoraCriacao,DataHoraRecebimento)
            VALUES (@IdentificadorEnvio,@VendedorId,@DataHoraCriacao,@DataHoraRecebimento); SELECT LAST_INSERT_ID();
            """, lote, tx);
        foreach (var pedido in lote.Pedidos)
        {
            pedido.PedidoGeralId = lote.Id;
            pedido.Id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO Pedidos (PedidoGeralId,ClienteId,CondicaoPagamentoId,DataHoraCriacao,ValorTotalSolicitado)
                VALUES (@PedidoGeralId,@ClienteId,@CondicaoPagamentoId,@DataHoraCriacao,@ValorTotalSolicitado); SELECT LAST_INSERT_ID();
                """, pedido, tx);
            foreach (var item in pedido.Itens)
            {
                item.PedidoId = pedido.Id;
                item.Id = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO ItemPedido (PedidoId,ProdutoId,NomeProduto,QuantidadeSolicitada,PrecoUnitario)
                    VALUES (@PedidoId,@ProdutoId,@NomeProduto,@QuantidadeSolicitada,@PrecoUnitario); SELECT LAST_INSERT_ID();
                    """, item, tx);
            }
            foreach (var parcela in pedido.Parcelas)
            {
                parcela.PedidoId = pedido.Id;
                parcela.Id = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO ParcelasPedido (PedidoId,NumeroParcela,DiasAposConfirmacao,FormaPagamentoId,ValorPrevisto)
                    VALUES (@PedidoId,@NumeroParcela,@DiasAposConfirmacao,@FormaPagamentoId,@ValorPrevisto); SELECT LAST_INSERT_ID();
                    """, parcela, tx);
            }
        }
        await c.ExecuteAsync("UPDATE ControleEnvios SET Status='Aceito',PedidoGeralId=@Id WHERE IdentificadorEnvio=@IdentificadorEnvio", lote, tx);
        await EventosService.RegistrarAsync(c, tx, "Pedidos", vendedorId);
        await tx.CommitAsync();
        return lote;
    }

    public async Task<PedidoGeral> PorIdentificadorAsync(int vendedorId, string identificador)
    {
        var uuid = Regras.Uuid(identificador);
        var d = await leitura.ConsultarAsync(vendedorId);
        return d.PedidosGerais.SingleOrDefault(l => l.IdentificadorEnvio == uuid)
            ?? throw new ApiException(404, "Envio não encontrado nesta conta.");
    }

    public async Task<object> PreviaAsync(int id)
    {
        var d = await leitura.ConsultarAsync();
        var lote = d.PedidosGerais.SingleOrDefault(l => l.Id == id) ?? throw new ApiException(404, "Envio não encontrado.");
        var ids = lote.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).ToHashSet();
        return ProjecaoWeb.Previa(rateio.Gerar(lote, d.Produtos.Where(p => ids.Contains(p.Id)).ToList()));
    }

    public async Task<object> DecidirAsync(int id, int administradorId, ConfirmarRequest? confirmar, CancelarRequest? cancelar)
    {
        bool confirma = confirmar is not null;
        string acao = confirma ? "Confirmar" : "Cancelar";
        string chave = Regras.Uuid(confirmar?.IdentificadorOperacao ?? cancelar?.IdentificadorOperacao);
        string hash = confirma ? Regras.Hash(new { id, acao, confirmar!.TokenPrevia }) : Regras.Hash(new { id, acao, cancelar!.Motivo, cancelar.Versao });
        if (!confirma) cancelar!.Motivo = Regras.Texto(cancelar.Motivo, "Motivo do cancelamento", 255);
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var bloqueado = await c.QuerySingleOrDefaultAsync<PedidoGeral>("SELECT * FROM PedidoGeral WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Envio não encontrado.");
        var operacao = await c.QuerySingleOrDefaultAsync<Operacao>("SELECT * FROM Operacoes WHERE Identificador=@chave FOR UPDATE", new { chave }, tx);
        if (operacao is not null)
        {
            Regras.Exigir(operacao.PedidoGeralId == id && operacao.AdministradorId == administradorId && operacao.Acao == acao && operacao.ConteudoHash == hash,
                "Esta chave de operação já foi utilizada com outros dados.", 409);
            await tx.CommitAsync();
            return new { Id = id, bloqueado.Status };
        }
        if (bloqueado.Status != "AguardandoConfirmacao")
        {
            Regras.Exigir((confirma && bloqueado.Status == "Processado") ||
                (!confirma && bloqueado.Status == "Cancelado" && bloqueado.MotivoCancelamento == cancelar!.Motivo), "O envio já recebeu uma decisão diferente.", 409);
        }
        else if (confirma)
        {
            var dados = await LeituraService.LerAsync(c, tx);
            var lote = dados.PedidosGerais.Single(l => l.Id == id);
            var ids = lote.Pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct().Order().ToArray();
            var produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos WHERE Id IN @ids ORDER BY Id FOR UPDATE", new { ids }, tx)).AsList();
            rateio.Validar(confirmar!.TokenPrevia, lote, produtos);
            var previa = rateio.Gerar(lote, produtos);
            var agora = DateTime.UtcNow;
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(configuracao["Loja:FusoHorario"] ?? "America/Sao_Paulo");
            var dataBase = TimeZoneInfo.ConvertTimeFromUtc(agora, fuso).Date;
            foreach (var p in previa.Pedidos)
            {
                p.Status = p.ValorTotalAtendido == 0 ? "NaoAtendido" : p.Itens.Any(i => i.QuantidadeAtendida < i.QuantidadeSolicitada) ? "ConfirmadoComCorte" : "Confirmado";
                await c.ExecuteAsync("UPDATE Pedidos SET Status=@Status,ValorTotalAtendido=@ValorTotalAtendido WHERE Id=@Id", p, tx);
                foreach (var i in p.Itens)
                    await c.ExecuteAsync("UPDATE ItemPedido SET QuantidadeAtendida=@QuantidadeAtendida WHERE Id=@Id", i, tx);
                if (p.ValorTotalAtendido == 0) continue;
                var vendaId = await c.ExecuteScalarAsync<int>("""
                    INSERT INTO Vendas (PedidoId,DataHoraConfirmacao,ValorTotal) VALUES (@Id,@agora,@ValorTotalAtendido);
                    SELECT LAST_INSERT_ID();
                    """, new { p.Id, agora, p.ValorTotalAtendido }, tx);
                foreach (var i in p.Itens.Where(i => i.QuantidadeAtendida > 0))
                    await c.ExecuteAsync("""
                        INSERT INTO ItemVenda (VendaId,PedidoId,ItemPedidoId,ProdutoId,NomeProduto,Quantidade,PrecoUnitario)
                        VALUES (@vendaId,@PedidoId,@Id,@ProdutoId,@NomeProduto,@QuantidadeAtendida,@PrecoUnitario)
                        """, new { vendaId, i.PedidoId, i.Id, i.ProdutoId, i.NomeProduto, i.QuantidadeAtendida, i.PrecoUnitario }, tx);
                foreach (var parcela in p.Parcelas.Where(x => x.ValorFinal > 0))
                    await c.ExecuteAsync("""
                        INSERT INTO ContasReceber (VendaId,PedidoId,ParcelaPedidoId,DataVencimento,Valor)
                        VALUES (@vendaId,@PedidoId,@Id,@vencimento,@ValorFinal)
                        """, new { vendaId, parcela.PedidoId, parcela.Id, vencimento = dataBase.AddDays(parcela.DiasAposConfirmacao), parcela.ValorFinal }, tx);
            }
            foreach (var produto in produtos)
            {
                int quantidade = previa.Pedidos.SelectMany(p => p.Itens).Where(i => i.ProdutoId == produto.Id).Sum(i => i.QuantidadeAtendida!.Value);
                if (quantidade > 0)
                    await c.ExecuteAsync("UPDATE Produtos SET Estoque=Estoque-@quantidade,Versao=Versao+1 WHERE Id=@Id", new { quantidade, produto.Id }, tx);
            }
            await c.ExecuteAsync("""
                UPDATE PedidoGeral SET Status='Processado',AdministradorId=@administradorId,
                    DataHoraProcessamento=@agora,Versao=Versao+1 WHERE Id=@id
                """, new { id, administradorId, agora }, tx);
            await EventosService.RegistrarAsync(c, tx, "Pedidos", lote.VendedorId);
            await EventosService.RegistrarAsync(c, tx, "Catalogo");
        }
        else
        {
            Regras.Exigir(bloqueado.Versao == cancelar!.Versao, "O envio mudou. Atualize os detalhes.", 409);
            await c.ExecuteAsync("UPDATE Pedidos SET Status='Cancelado' WHERE PedidoGeralId=@id", new { id }, tx);
            await c.ExecuteAsync("""
                UPDATE PedidoGeral SET Status='Cancelado',MotivoCancelamento=@Motivo,AdministradorId=@administradorId,
                    DataHoraProcessamento=UTC_TIMESTAMP(6),Versao=Versao+1 WHERE Id=@id
                """, new { id, cancelar.Motivo, administradorId }, tx);
            await EventosService.RegistrarAsync(c, tx, "Pedidos", bloqueado.VendedorId);
        }
        await c.ExecuteAsync("""
            INSERT INTO Operacoes (Identificador,PedidoGeralId,AdministradorId,Acao,ConteudoHash,CriadoEm)
            VALUES (@chave,@id,@administradorId,@acao,@hash,UTC_TIMESTAMP(6))
            """, new { chave, id, administradorId, acao, hash }, tx);
        await tx.CommitAsync();
        return new { Id = id, Status = confirma ? "Processado" : "Cancelado" };
    }
}
