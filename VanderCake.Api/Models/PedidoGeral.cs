namespace VanderCake.Api.Models;

public class PedidoGeral
{
    public int Id { get; set; }
    public string IdentificadorEnvio { get; set; } = "";
    public int VendedorId { get; set; }
    public string VendedorNome { get; set; } = "";
    public DateTime DataHoraCriacao { get; set; }
    public DateTime DataHoraRecebimento { get; set; }
    public DateTime? DataHoraProcessamento { get; set; }
    public int? AdministradorId { get; set; }
    public string? AdministradorNome { get; set; }
    public string Status { get; set; } = "AguardandoConfirmacao";
    public string? MotivoCancelamento { get; set; }
    public long Versao { get; set; } = 1;
    public List<Pedido> Pedidos { get; set; } = [];
}
