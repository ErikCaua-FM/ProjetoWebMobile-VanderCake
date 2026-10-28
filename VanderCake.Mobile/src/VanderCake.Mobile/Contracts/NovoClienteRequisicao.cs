using VanderCake.Models;

namespace VanderCake.Contracts;

public class NovoClienteRequisicao
{
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId
    {
        get; set;
    }
    public string Telefone { get; set; } = "";
    public string? Email
    {
        get; set;
    }
}
