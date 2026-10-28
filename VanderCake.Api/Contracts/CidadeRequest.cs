namespace VanderCake.Api.Contracts;

public class CidadeRequest
{
    public string Nome { get; set; } = "";
    public int EstadoId { get; set; }
    public long Versao { get; set; }
}
