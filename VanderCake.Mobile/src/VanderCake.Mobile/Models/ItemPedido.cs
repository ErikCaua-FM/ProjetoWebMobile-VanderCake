using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class ItemPedido
{
    public int ProdutoId
    {
        get; set;
    }
    public string NomeProduto { get; set; } = "";
    public int QuantidadeSolicitada
    {
        get; set;
    }
    public int? QuantidadeAtendida
    {
        get; set;
    }
    public decimal PrecoUnitario
    {
        get; set;
    }

    [JsonIgnore]
    public decimal SubtotalSolicitado => PrecoUnitario * QuantidadeSolicitada;

    [JsonIgnore]
    public decimal? SubtotalAtendido => QuantidadeAtendida * PrecoUnitario;

    [JsonIgnore]
    public string QuantidadeTexto => $"{QuantidadeSolicitada} un.";

    [JsonIgnore]
    public string PrecoTexto => Dinheiro.Formatar(PrecoUnitario);

    [JsonIgnore]
    public string SubtotalTexto => Dinheiro.Formatar(SubtotalSolicitado);

    [JsonIgnore]
    public string SubtotalAtendidoTexto => SubtotalAtendido is decimal valor ? $"Subtotal atendido: {Dinheiro.Formatar(valor)}" : "";

    [JsonIgnore]
    public string AtendimentoTexto => QuantidadeAtendida is int q ? $"Solicitado: {QuantidadeSolicitada} · Atendido: {q} · Corte: {QuantidadeSolicitada - q}" : $"Solicitado: {QuantidadeSolicitada} · Atendimento ainda não definido";
}
