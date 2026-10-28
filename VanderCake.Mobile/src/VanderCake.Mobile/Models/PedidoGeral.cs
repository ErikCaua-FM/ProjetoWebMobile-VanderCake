using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class PedidoGeral
{
    public int Id
    {
        get; set;
    }
    public string IdentificadorEnvio { get; set; } = Guid.NewGuid().ToString();
    public int VendedorId
    {
        get; set;
    }
    public DateTimeOffset DataHoraCriacao { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DataHoraRecebimento
    {
        get; set;
    }
    public string Status { get; set; } = "Rascunho";
    public string? MotivoCancelamento
    {
        get; set;
    }
    public List<Pedido> Pedidos { get; set; } = [];

    [JsonIgnore]
    public decimal TotalSolicitado => Pedidos.Sum(p => p.ValorTotalSolicitado);
}
