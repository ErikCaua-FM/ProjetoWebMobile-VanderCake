using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Vendedor
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
