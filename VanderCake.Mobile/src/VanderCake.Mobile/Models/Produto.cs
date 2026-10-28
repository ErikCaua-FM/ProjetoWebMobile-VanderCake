using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Produto
{
    public int Id
    {
        get; set;
    }
    public int CategoriaId
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string? Descricao
    {
        get; set;
    }
    public decimal Preco
    {
        get; set;
    }
    public int Estoque
    {
        get; set;
    }
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public string PrecoTexto => Dinheiro.Formatar(Preco);

    [JsonIgnore]
    public string EstoqueTexto => $"Estoque informado: {Estoque}";
}
