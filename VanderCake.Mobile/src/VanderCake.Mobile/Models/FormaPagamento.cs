using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class FormaPagamento
{
    public int Id
    {
        get; set;
    }
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
