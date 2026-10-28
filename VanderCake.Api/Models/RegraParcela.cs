namespace VanderCake.Api.Models;

public class RegraParcela
{
    public int Id { get; set; }
    public int CondicaoPagamentoId { get; set; }
    public int NumeroParcela { get; set; }
    public int DiasAposConfirmacao { get; set; }
}
