using SQLite;

namespace VanderCake.Data;

public class CarrinhoRegistro
{
    [PrimaryKey]
    public string Chave { get; set; } = "";

    [Indexed]
    public int VendedorId
    {
        get; set;
    }
    public int ClienteId
    {
        get; set;
    }
    public string ConteudoJson { get; set; } = "";
}
