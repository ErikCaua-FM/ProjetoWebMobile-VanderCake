namespace VanderCake.Api.Contracts;

public sealed class PagamentoCadastroRequest
{
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
    public int[] Dias { get; set; } = [];
}
