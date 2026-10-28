using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class DadosSincronizacao
{
    public Vendedor Vendedor { get; set; } = new();
    public List<Cliente> Clientes { get; set; } = [];
    public List<Estado> Estados { get; set; } = [];
    public List<Cidade> Cidades { get; set; } = [];
    public List<Categoria> Categorias { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];
    public List<CondicaoPagamento> CondicoesPagamento { get; set; } = [];
    public List<FormaPagamento> FormasPagamento { get; set; } = [];
    public List<PedidoGeral> PedidosGerais { get; set; } = [];
    public DateTimeOffset? SincronizadoEm
    {
        get; set;
    }
}
