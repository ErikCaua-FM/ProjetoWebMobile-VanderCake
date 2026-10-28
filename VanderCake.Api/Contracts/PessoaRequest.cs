namespace VanderCake.Api.Contracts;

public class PessoaRequest
{
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId { get; set; }
    public string Telefone { get; set; } = "";
    public string? Email { get; set; }
    public string? Senha { get; set; }
    public int VendedorId { get; set; }
    public bool Ativo { get; set; } = true;
    public long Versao { get; set; }
}
