namespace VanderCake.Api.Models;

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = "";
    public int QuantidadeSolicitada { get; set; }
    public int? QuantidadeAtendida { get; set; }
    public decimal PrecoUnitario { get; set; }
}
