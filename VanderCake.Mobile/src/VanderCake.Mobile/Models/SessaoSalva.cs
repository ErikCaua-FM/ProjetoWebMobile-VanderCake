using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class SessaoSalva
{
    public string AccessToken { get; set; } = "";
    public string? RefreshToken { get; set; }
    public DateTimeOffset ExpiraEm { get; set; } = DateTimeOffset.MaxValue;
    public Vendedor Vendedor { get; set; } = new();
}
