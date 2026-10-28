namespace VanderCake.Api.Models;

public class ControleEnvio
{
    public string IdentificadorEnvio { get; set; } = "";
    public int VendedorId { get; set; }
    public string ConteudoHash { get; set; } = "";
    public string Status { get; set; } = "";
    public int? PedidoGeralId { get; set; }
    public int? CodigoRejeicao { get; set; }
    public string? MotivoRejeicao { get; set; }
}
