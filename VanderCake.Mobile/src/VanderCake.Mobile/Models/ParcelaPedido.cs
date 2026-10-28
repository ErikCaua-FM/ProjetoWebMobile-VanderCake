using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class ParcelaPedido
{
    public int NumeroParcela
    {
        get; set;
    }
    public int DiasAposConfirmacao
    {
        get; set;
    }
    public int FormaPagamentoId
    {
        get; set;
    }
    public string FormaPagamentoNome { get; set; } = "";
    public decimal ValorPrevisto
    {
        get; set;
    }
    public decimal? ValorFinal
    {
        get; set;
    }
    public DateTime? DataVencimento
    {
        get; set;
    }
    public DateTime? DataPagamento { get; set; }
    public string Situacao { get; set; } = "Prevista";

    [JsonIgnore]
    public string PrazoTexto => DiasAposConfirmacao == 0 ? "À vista" : $"{DiasAposConfirmacao} dias";

    [JsonIgnore]
    public string Descricao => $"{NumeroParcela}ª parcela · {PrazoTexto}";

    [JsonIgnore]
    public string ValorTexto => ValorFinal is decimal valor ? Dinheiro.Formatar(valor) : $"Previsto: {Dinheiro.Formatar(ValorPrevisto)}";

    [JsonIgnore]
    public string DetalheTexto => Situacao == "Paga" && DataPagamento is DateTime pagamento
        ? $"{FormaPagamentoNome} · Recebida em {pagamento:dd/MM/yyyy}"
        : DataVencimento is DateTime data ? $"{FormaPagamentoNome} · Vence em {data:dd/MM/yyyy} · {Situacao}" : $"{FormaPagamentoNome} · Prazo contado da confirmação da venda";
}
