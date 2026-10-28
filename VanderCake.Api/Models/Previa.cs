namespace VanderCake.Api.Models;

public class Previa
{
    public string TokenPrevia { get; set; } = "";
    public List<Pedido> Pedidos { get; set; } = [];
    public decimal TotalSolicitado { get; set; }
    public decimal TotalAtendido { get; set; }
    public long UnidadesCortadas { get; set; }
}
