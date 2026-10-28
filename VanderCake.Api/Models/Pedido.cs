namespace VanderCake.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public int PedidoGeralId { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = "";
    public DateTime DataHoraCriacao { get; set; }
    public int CondicaoPagamentoId { get; set; }
    public string CondicaoPagamentoDescricao { get; set; } = "";
    public string Status { get; set; } = "Pendente";
    public decimal ValorTotalSolicitado { get; set; }
    public decimal? ValorTotalAtendido { get; set; }
    public List<ItemPedido> Itens { get; set; } = [];
    public List<ParcelaPedido> Parcelas { get; set; } = [];
}
