using System.Text.Json.Serialization;
namespace VanderCake.Api.Models;

public class Vendedor : Pessoa
{
    [JsonIgnore]
    public string SenhaHash { get; set; } = "";
    [JsonIgnore]
    public int VersaoSeguranca { get; set; } = 1;
}
