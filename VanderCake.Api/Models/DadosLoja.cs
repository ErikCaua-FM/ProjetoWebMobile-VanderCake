namespace VanderCake.Api.Models;

public class DadosLoja
{
    public List<Estado> Estados { get; set; } = [];
    public List<Cidade> Cidades { get; set; } = [];
    public List<Categoria> Categorias { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];
    public List<Vendedor> Vendedores { get; set; } = [];
    public List<Cliente> Clientes { get; set; } = [];
    public List<CondicaoPagamento> CondicoesPagamento { get; set; } = [];
    public List<FormaPagamento> FormasPagamento { get; set; } = [];
    public List<PedidoGeral> PedidosGerais { get; set; } = [];
    public List<Venda> Vendas { get; set; } = [];
    public List<ContaReceber> ContasReceber { get; set; } = [];
}
