using System.Net.Mail;
using VanderCake.Contracts;

namespace VanderCake.Rules;

public static class ValidacaoCliente
{
    public static string Digitos(string texto) => new(texto.Where(char.IsDigit).ToArray());
    public static void Validar(NovoClienteRequisicao cliente)
    {
        cliente.Nome = cliente.Nome.Trim();
        cliente.Documento = Digitos(cliente.Documento);
        cliente.Cep = Digitos(cliente.Cep);
        cliente.Telefone = Digitos(cliente.Telefone);
        cliente.Email = string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email.Trim();
        if (cliente.Nome.Length is < 2 or > 100 || cliente.Documento.Length is not (11 or 14))
            throw new InvalidOperationException("Informe nome/razão social e CPF (11 dígitos) ou CNPJ (14 dígitos).");
        if (string.IsNullOrWhiteSpace(cliente.Rua) || cliente.Rua.Length > 100 || string.IsNullOrWhiteSpace(cliente.Bairro) || cliente.Bairro.Length > 60 || string.IsNullOrWhiteSpace(cliente.Numero) || cliente.Numero.Length > 10 || cliente.Cep.Length != 8 || cliente.CidadeId <= 0)
            throw new InvalidOperationException("Preencha rua, número, bairro, CEP e cidade corretamente.");
        if (cliente.Telefone.Length is < 10 or > 13)
            throw new InvalidOperationException("Informe o telefone com DDD.");
        if (cliente.Email is not null && (cliente.Email.Length > 150 || !MailAddress.TryCreate(cliente.Email, out _)))
            throw new InvalidOperationException("Informe um e-mail válido ou deixe o campo vazio.");
    }
}
