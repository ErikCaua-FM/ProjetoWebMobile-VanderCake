namespace VanderCake.Api.Contracts;

public class ProdutoRequest
{
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public int CategoriaId { get; set; }
    public long PrecoCentavos { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
}
