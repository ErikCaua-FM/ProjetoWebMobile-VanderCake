using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Cliente
{
    public int Id
    {
        get; set;
    }
    public int VendedorId
    {
        get; set;
    }
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
    public string CidadeNome { get; set; } = "";
    public string Uf { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Email
    {
        get; set;
    }
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public string Endereco => $"{Rua}, {Numero} · {Bairro}";

    [JsonIgnore]
    public string Localidade => $"{CidadeNome}/{Uf} · CEP {Cep}";

    [JsonIgnore]
    public string Identificacao => $"CPF/CNPJ: {Documento}";

    [JsonIgnore]
    public string SituacaoTexto => Ativo ? "Consultar pedidos →" : "Cliente inativo · consultar histórico";
}
