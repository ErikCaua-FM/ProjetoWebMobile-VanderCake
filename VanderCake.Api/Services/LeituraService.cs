using System.Data;
using Dapper;
using MySqlConnector;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class LeituraService(Banco banco)
{
    public async Task<DadosLoja> ConsultarAsync(int? vendedorId = null)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var dados = await LerAsync(c, tx, vendedorId);
        await tx.CommitAsync();
        return dados;
    }

    public static async Task<DadosLoja> LerAsync(MySqlConnection c, MySqlTransaction tx, int? vendedorId = null)
    {
        var dados = new DadosLoja
        {
            Estados = (await c.QueryAsync<Estado>("SELECT * FROM Estados ORDER BY Nome", transaction: tx)).AsList(),
            Cidades = (await c.QueryAsync<Cidade>("SELECT * FROM Cidades ORDER BY Nome", transaction: tx)).AsList(),
            Categorias = (await c.QueryAsync<Categoria>("SELECT * FROM Categorias ORDER BY Nome", transaction: tx)).AsList(),
            Produtos = (await c.QueryAsync<Produto>("SELECT * FROM Produtos ORDER BY Id", transaction: tx)).AsList(),
            Vendedores = (await c.QueryAsync<Vendedor>("SELECT * FROM Vendedores WHERE @vendedorId IS NULL OR Id = @vendedorId", new { vendedorId }, tx)).AsList(),
            Clientes = (await c.QueryAsync<Cliente>("""
                SELECT cl.*, ci.Nome AS CidadeNome, e.Uf
                FROM Clientes cl JOIN Cidades ci ON ci.Id = cl.CidadeId JOIN Estados e ON e.Id = ci.EstadoId
                WHERE @vendedorId IS NULL OR cl.VendedorId = @vendedorId ORDER BY cl.Id
                """, new { vendedorId }, tx)).AsList(),
            CondicoesPagamento = (await c.QueryAsync<CondicaoPagamento>("SELECT * FROM CondicoesPagamento ORDER BY Id", transaction: tx)).AsList(),
            FormasPagamento = (await c.QueryAsync<FormaPagamento>("SELECT * FROM FormasPagamento ORDER BY Id", transaction: tx)).AsList(),
            PedidosGerais = (await c.QueryAsync<PedidoGeral>("""
                SELECT pg.*, v.Nome AS VendedorNome, a.Nome AS AdministradorNome
                FROM PedidoGeral pg JOIN Vendedores v ON v.Id = pg.VendedorId
                LEFT JOIN Administradores a ON a.Id = pg.AdministradorId
                WHERE @vendedorId IS NULL OR pg.VendedorId = @vendedorId ORDER BY pg.Id
                """, new { vendedorId }, tx)).AsList()
        };
        var regras = (await c.QueryAsync<RegraParcela>("SELECT * FROM ParcelasCondicaoPagamento ORDER BY NumeroParcela", transaction: tx)).AsList();
        foreach (var condicao in dados.CondicoesPagamento)
            condicao.Parcelas = regras.Where(p => p.CondicaoPagamentoId == condicao.Id).ToList();
        if (dados.PedidosGerais.Count == 0) return dados;
        var lotes = dados.PedidosGerais.Select(x => x.Id).ToArray();
        var pedidos = (await c.QueryAsync<Pedido>("""
            SELECT p.*, cl.Nome AS ClienteNome, cp.Descricao AS CondicaoPagamentoDescricao
            FROM Pedidos p JOIN Clientes cl ON cl.Id = p.ClienteId
            JOIN CondicoesPagamento cp ON cp.Id = p.CondicaoPagamentoId
            WHERE p.PedidoGeralId IN @lotes ORDER BY p.Id
            """, new { lotes }, tx)).AsList();
        if (pedidos.Count == 0) return dados;
        var ids = pedidos.Select(x => x.Id).ToArray();
        var itens = (await c.QueryAsync<ItemPedido>("SELECT * FROM ItemPedido WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        var parcelas = (await c.QueryAsync<ParcelaPedido>("""
            SELECT pp.*, f.Descricao AS FormaPagamentoNome, cr.Valor AS ValorFinal, cr.DataVencimento, cr.DataPagamento,
                   COALESCE(cr.Status, 'Prevista') AS Situacao
            FROM ParcelasPedido pp JOIN FormasPagamento f ON f.Id = pp.FormaPagamentoId
            LEFT JOIN ContasReceber cr ON cr.ParcelaPedidoId = pp.Id
            WHERE pp.PedidoId IN @ids ORDER BY pp.PedidoId, pp.NumeroParcela
            """, new { ids }, tx)).AsList();
        dados.Vendas = (await c.QueryAsync<Venda>("SELECT * FROM Vendas WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        dados.ContasReceber = (await c.QueryAsync<ContaReceber>("""
            SELECT cr.*, pp.NumeroParcela, pp.FormaPagamentoId FROM ContasReceber cr
            JOIN ParcelasPedido pp ON pp.Id = cr.ParcelaPedidoId
            WHERE cr.PedidoId IN @ids ORDER BY cr.Id
            """, new { ids }, tx)).AsList();
        var vendidos = (await c.QueryAsync<ItemVenda>("SELECT * FROM ItemVenda WHERE PedidoId IN @ids ORDER BY Id", new { ids }, tx)).AsList();
        foreach (var venda in dados.Vendas) venda.Itens = vendidos.Where(i => i.VendaId == venda.Id).ToList();
        foreach (var p in pedidos)
        {
            p.Itens = itens.Where(i => i.PedidoId == p.Id).ToList();
            p.Parcelas = parcelas.Where(i => i.PedidoId == p.Id).ToList();
            if (p.Status is "Confirmado" or "ConfirmadoComCorte" or "NaoAtendido")
            {
                foreach (var parcela in p.Parcelas.Where(x => x.ValorFinal is null))
                {
                    parcela.ValorFinal = 0;
                    parcela.Situacao = "SemCobranca";
                }
            }
            if (p.Status == "Cancelado")
                foreach (var parcela in p.Parcelas) parcela.Situacao = "Cancelada";
        }
        foreach (var lote in dados.PedidosGerais) lote.Pedidos = pedidos.Where(p => p.PedidoGeralId == lote.Id).ToList();
        return dados;
    }
}
