namespace VanderCake.Api.Contracts;

public class CategoriaRequest
{
    public string Nome { get; set; } = "";
    public long Versao { get; set; }
    public bool Ativo { get; set; } = true;
}
