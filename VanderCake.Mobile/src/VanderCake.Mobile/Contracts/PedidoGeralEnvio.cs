using VanderCake.Models;

namespace VanderCake.Contracts;

public class PedidoGeralEnvio
{
    public string IdentificadorEnvio { get; set; } = "";
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public List<PedidoEnvio> Pedidos { get; set; } = [];
}
