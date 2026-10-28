using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class CondicaoPagamento
{
    public int Id
    {
        get; set;
    }
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public List<RegraParcela> Parcelas { get; set; } = [];
}
