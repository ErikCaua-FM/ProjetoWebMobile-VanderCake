using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Estado
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
}
