using System.Text.Json.Serialization;
namespace VanderCake.Api.Models;

public class Administrador : Pessoa
{
    [JsonIgnore]
    public string SenhaHash { get; set; } = "";
    [JsonIgnore]
    public int VersaoSeguranca { get; set; } = 1;
}
