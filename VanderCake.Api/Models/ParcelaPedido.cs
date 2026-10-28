namespace VanderCake.Api.Models;

public class ParcelaPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int NumeroParcela { get; set; }
    public int DiasAposConfirmacao { get; set; }
    public int FormaPagamentoId { get; set; }
    public string FormaPagamentoNome { get; set; } = "";
    public decimal ValorPrevisto { get; set; }
    public decimal? ValorFinal { get; set; }
    public DateTime? DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string Situacao { get; set; } = "Prevista";
}
