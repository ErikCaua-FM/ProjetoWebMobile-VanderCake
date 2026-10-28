using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Categoria
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
