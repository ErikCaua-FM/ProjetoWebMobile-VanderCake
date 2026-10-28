using VanderCake.Models;

namespace VanderCake.Contracts;

public class PedidoEnvio
{
    public int ClienteId
    {
        get; set;
    }
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public int CondicaoPagamentoId
    {
        get; set;
    }
    public List<ItemPedidoEnvio> Itens { get; set; } = [];
    public List<ParcelaEnvio> Parcelas { get; set; } = [];
}
