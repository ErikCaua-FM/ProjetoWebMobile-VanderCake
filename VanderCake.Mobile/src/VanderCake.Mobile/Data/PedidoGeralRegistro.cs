using SQLite;

namespace VanderCake.Data;

public class PedidoGeralRegistro
{
    [PrimaryKey]
    public string IdentificadorEnvio { get; set; } = "";

    [Indexed]
    public int VendedorId
    {
        get; set;
    }
    public string SituacaoLocal { get; set; } = "Montagem";
    public string ConteudoJson { get; set; } = "";
    public string? CorpoEnvioJson
    {
        get; set;
    }
}
