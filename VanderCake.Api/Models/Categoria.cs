namespace VanderCake.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; } = 1;
}
