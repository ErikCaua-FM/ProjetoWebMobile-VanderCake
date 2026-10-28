namespace VanderCake.Api.Models;

public class Cidade
{
    public int Id { get; set; }
    public int EstadoId { get; set; }
    public string Nome { get; set; } = "";
    public long Versao { get; set; } = 1;
}
