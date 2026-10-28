using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Pedido
{
    public int Id
    {
        get; set;
    }
    public int ClienteId
    {
        get; set;
    }
    public string ClienteNome { get; set; } = "";
    public DateTimeOffset DataHoraCriacao { get; set; } = DateTimeOffset.UtcNow;
    public int CondicaoPagamentoId
    {
        get; set;
    }
    public string CondicaoPagamentoDescricao { get; set; } = "";
    public string Status { get; set; } = "Rascunho";
    public decimal ValorTotalSolicitado
    {
        get; set;
    }
    public decimal? ValorTotalAtendido
    {
        get; set;
    }
    public List<ItemPedido> Itens { get; set; } = [];
    public List<ParcelaPedido> Parcelas { get; set; } = [];

    [JsonIgnore]
    public string Titulo => Id > 0 ? $"Pedido #{Id}" : "Pedido local";

    [JsonIgnore]
    public string StatusTexto => StatusPedido.Descrever(Status);

    [JsonIgnore]
    public string DataTexto => DataHoraCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    [JsonIgnore]
    public string TotalTexto => Dinheiro.Formatar(ValorTotalAtendido ?? ValorTotalSolicitado);

    [JsonIgnore]
    public string ResumoItens => string.Join(" · ", Itens.Select(i => $"{i.QuantidadeSolicitada}x {i.NomeProduto}"));
}
