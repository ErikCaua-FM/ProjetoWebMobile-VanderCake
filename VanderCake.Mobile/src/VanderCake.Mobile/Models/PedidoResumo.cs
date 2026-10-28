using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class PedidoResumo
{
    public string IdentificadorEnvio { get; set; } = "";
    public Pedido Pedido { get; set; } = new();
    public string SituacaoLocal { get; set; } = "";
    public string? MotivoCancelamento
    {
        get; set;
    }

    [JsonIgnore]
    public string SituacaoTexto => SituacaoLocal == "EnvioIncerto" ? "Envio a verificar" : Pedido.StatusTexto;

    [JsonIgnore]
    public bool PodeEditar => SituacaoLocal == "Montagem";
}
