namespace VanderCake.Api.Models;

public class Venda
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public DateTime DataHoraConfirmacao { get; set; }
    public decimal ValorTotal { get; set; }
    public List<ItemVenda> Itens { get; set; } = [];
}
