namespace VanderCake.Api.Models;

public class FormaPagamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public long Versao { get; set; } = 1;
    public bool Ativo { get; set; } = true;
}
