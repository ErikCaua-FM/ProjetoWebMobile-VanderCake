namespace VanderCake.Api.Models;

public class ContaReceber
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int PedidoId { get; set; }
    public int ParcelaPedidoId { get; set; }
    public int NumeroParcela { get; set; }
    public int FormaPagamentoId { get; set; }
    public DateTime DataVencimento { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTime? DataPagamento { get; set; }
    public int? AdministradorBaixaId { get; set; }
}
