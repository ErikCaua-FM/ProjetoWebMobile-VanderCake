using SQLite;

namespace VanderCake.Data;

public class CacheVendedorRegistro
{
    [PrimaryKey]
    public int VendedorId
    {
        get; set;
    }
    public string ConteudoJson { get; set; } = "";
}
