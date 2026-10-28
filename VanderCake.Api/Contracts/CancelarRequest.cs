namespace VanderCake.Api.Contracts;

public class CancelarRequest
{
    public string Motivo { get; set; } = "";
    public long Versao { get; set; }
    public string IdentificadorOperacao { get; set; } = "";
}
