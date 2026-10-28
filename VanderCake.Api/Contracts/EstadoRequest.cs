namespace VanderCake.Api.Contracts;

public class EstadoRequest
{
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
    public long Versao { get; set; }
}
