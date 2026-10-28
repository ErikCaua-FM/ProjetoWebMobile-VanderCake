namespace VanderCake.Api.Contracts;

public class ItemPedidoEnvio
{
    public int ProdutoId
    {
        get; set;
    }
    public int QuantidadeSolicitada
    {
        get; set;
    }
    public decimal PrecoUnitario
    {
        get; set;
    }
}
