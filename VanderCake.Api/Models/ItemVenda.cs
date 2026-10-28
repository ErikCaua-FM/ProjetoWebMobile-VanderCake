namespace VanderCake.Api.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int PedidoId { get; set; }
    public int ItemPedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = "";
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
