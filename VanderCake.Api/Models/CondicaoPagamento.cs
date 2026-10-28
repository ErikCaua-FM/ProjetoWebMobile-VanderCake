namespace VanderCake.Api.Models;

public class CondicaoPagamento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public long Versao { get; set; } = 1;
    public bool Ativo { get; set; } = true;
    public List<RegraParcela> Parcelas { get; set; } = [];
}
