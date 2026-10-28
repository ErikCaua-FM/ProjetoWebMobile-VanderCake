using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Cidade
{
    public int Id
    {
        get; set;
    }
    public int EstadoId
    {
        get; set;
    }
    public string Nome { get; set; } = "";
}
