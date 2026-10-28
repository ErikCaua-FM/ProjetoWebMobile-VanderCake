namespace VanderCake.Api.Models;

public class Operacao
{
    public string Identificador { get; set; } = "";
    public int PedidoGeralId { get; set; }
    public int AdministradorId { get; set; }
    public string Acao { get; set; } = "";
    public string ConteudoHash { get; set; } = "";
}
