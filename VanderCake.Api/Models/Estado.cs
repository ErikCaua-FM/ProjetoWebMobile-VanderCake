namespace VanderCake.Api.Models;

public class Estado
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
    public long Versao { get; set; } = 1;
}
