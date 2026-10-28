using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

// O banco e o Mobile usam decimal. A Web recebe inteiros em centavos.
public static class ProjecaoWeb
{
    public static object Produto(Produto p) => new
    {
        p.Id, p.Nome, p.CategoriaId, p.Descricao, p.Estoque, p.Ativo, p.Versao,
        PrecoCentavos = Regras.Centavos(p.Preco)
    };

    public static object Pedido(Pedido p) => new
    {
        p.Id, p.ClienteId, p.ClienteNome, p.DataHoraCriacao, p.CondicaoPagamentoId,
        p.CondicaoPagamentoDescricao, p.Status,
        ValorTotalSolicitadoCentavos = Regras.Centavos(p.ValorTotalSolicitado),
        ValorTotalAtendidoCentavos = Regras.Centavos(p.ValorTotalAtendido),
        Itens = p.Itens.Select(i => new
        {
            i.ProdutoId, i.NomeProduto, i.QuantidadeSolicitada, i.QuantidadeAtendida,
            PrecoUnitarioCentavos = Regras.Centavos(i.PrecoUnitario)
        }),
        Parcelas = p.Parcelas.Select(i => new
        {
            i.NumeroParcela, i.DiasAposConfirmacao, i.FormaPagamentoId, i.FormaPagamentoNome,
            ValorPrevistoCentavos = Regras.Centavos(i.ValorPrevisto),
            ValorFinalCentavos = Regras.Centavos(i.ValorFinal),
            DataVencimento = i.DataVencimento?.ToString("yyyy-MM-dd"), i.Situacao,
            DataPagamento = i.DataPagamento?.ToString("yyyy-MM-dd")
        })
    };

    public static object Lote(PedidoGeral l) => new
    {
        l.Id, l.IdentificadorEnvio, l.VendedorId, l.VendedorNome, l.DataHoraCriacao,
        l.DataHoraRecebimento, l.DataHoraProcessamento, l.AdministradorId, l.AdministradorNome,
        l.Status, l.MotivoCancelamento, l.Versao, Pedidos = l.Pedidos.Select(Pedido)
    };

    public static object Venda(Venda v) => new
    {
        v.Id, v.PedidoId, v.DataHoraConfirmacao, ValorTotalCentavos = Regras.Centavos(v.ValorTotal),
        Itens = v.Itens.Select(i => new
        {
            i.ProdutoId, i.NomeProduto, QuantidadeAtendida = i.Quantidade,
            PrecoUnitarioCentavos = Regras.Centavos(i.PrecoUnitario)
        })
    };

    public static object Conta(ContaReceber c) => new
    {
        c.Id, c.VendaId, c.PedidoId, c.NumeroParcela, c.FormaPagamentoId,
        DataVencimento = c.DataVencimento.ToString("yyyy-MM-dd"),
        ValorCentavos = Regras.Centavos(c.Valor), c.Status,
        DataPagamento = c.DataPagamento?.ToString("yyyy-MM-dd"), c.AdministradorBaixaId
    };

    public static object Painel(DadosLoja d) => new
    {
        d.Estados, d.Cidades, d.Categorias, Produtos = d.Produtos.Select(Produto),
        d.Vendedores, d.Clientes,
        CondicoesPagamento = d.CondicoesPagamento.Select(c => new { c.Id, c.Descricao, c.Ativo, c.Versao, Dias = c.Parcelas.Select(p => p.DiasAposConfirmacao) }),
        d.FormasPagamento, PedidosGerais = d.PedidosGerais.Select(Lote),
        Vendas = d.Vendas.Select(Venda), ContasReceber = d.ContasReceber.Select(Conta)
    };

    public static object Detalhes(DadosLoja d, int id)
    {
        var l = d.PedidosGerais.SingleOrDefault(x => x.Id == id) ?? throw new ApiException(404, "Pedido Geral não encontrado.");
        var pedidos = l.Pedidos.Select(p => p.Id).ToHashSet();
        var clientes = l.Pedidos.Select(p => p.ClienteId).ToHashSet();
        return new
        {
            Lote = Lote(l), Clientes = d.Clientes.Where(c => clientes.Contains(c.Id)), d.Cidades, d.Estados,
            Vendas = d.Vendas.Where(v => pedidos.Contains(v.PedidoId)).Select(Venda),
            ContasReceber = d.ContasReceber.Where(c => pedidos.Contains(c.PedidoId)).Select(Conta)
        };
    }

    public static object Previa(Previa p) => new
    {
        p.TokenPrevia, Pedidos = p.Pedidos.Select(Pedido),
        TotalSolicitadoCentavos = Regras.Centavos(p.TotalSolicitado),
        TotalAtendidoCentavos = Regras.Centavos(p.TotalAtendido), p.UnidadesCortadas
    };
}
