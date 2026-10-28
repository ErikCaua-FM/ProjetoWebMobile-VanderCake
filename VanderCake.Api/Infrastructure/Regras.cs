using System.Net.Mail;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using VanderCake.Api.Contracts;
using VanderCake.Api.Models;

namespace VanderCake.Api.Infrastructure;

public static class Regras
{
    public const decimal LimiteDinheiro = 9999999999.99m;
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static void Exigir(bool condicao, string mensagem, int status = 422)
    {
        if (!condicao) throw new ApiException(status, mensagem);
    }

    public static string Texto(string? valor, string campo, int maximo, bool opcional = false)
    {
        var texto = (valor ?? "").Trim();
        Exigir((opcional || texto.Length > 0) && texto.Length <= maximo, $"{campo}: preencha com até {maximo} caracteres.");
        return texto;
    }

    public static string Digitos(string? valor) => Regex.Replace(valor ?? "", @"\D", "");
    public static string Uuid(string? valor)
    {
        Exigir(Guid.TryParseExact(valor, "D", out var id) && id != Guid.Empty, "Identificador da operação inválido.");
        return id.ToString("D");
    }
    public static long Centavos(decimal valor) => checked((long)(valor * 100m));
    public static long? Centavos(decimal? valor) => valor.HasValue ? Centavos(valor.Value) : null;
    public static string Hash(object valor) => Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(valor, Json)));
    public static T Copiar<T>(T valor) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(valor, Json), Json)!;

    public static decimal[] Parcelar(decimal total, int quantidade)
    {
        Exigir(total >= 0 && total <= LimiteDinheiro && decimal.Round(total, 2) == total && quantidade is >= 1 and <= 3, "Total ou parcelas inválidos.");
        long centavos = Centavos(total);
        long parte = centavos / quantidade;
        return Enumerable.Range(0, quantidade).Select(i => (i == quantidade - 1 ? centavos - parte * (quantidade - 1) : parte) / 100m).ToArray();
    }

    public static Dictionary<int, int> Ratear(int estoque, IReadOnlyList<(int ClienteId, int Quantidade)> demandas)
    {
        Exigir(estoque >= 0 && demandas.Count > 0 && demandas.All(x => x.Quantidade > 0) && demandas.Select(x => x.ClienteId).Distinct().Count() == demandas.Count, "Demanda ou estoque inválido.");
        var total = demandas.Aggregate(BigInteger.Zero, (s, x) => s + x.Quantidade);
        var saldo = BigInteger.Min(estoque, total);
        var quotas = demandas.Select(x => new { x.ClienteId, Quantidade = (int)(saldo * x.Quantidade / total), Resto = saldo * x.Quantidade % total }).ToList();
        var resultado = quotas.ToDictionary(x => x.ClienteId, x => x.Quantidade);
        int sobra = (int)saldo - resultado.Values.Sum();
        foreach (var quota in quotas.OrderByDescending(x => x.Resto).ThenBy(x => x.ClienteId).Take(sobra))
            resultado[quota.ClienteId]++;
        return resultado;
    }

    public static void ValidarPessoa(PessoaRequest p, bool emailObrigatorio)
    {
        p.Nome = Texto(p.Nome, "Nome", 100);
        p.Documento = Digitos(p.Documento);
        Exigir(p.Documento.Length is 11 or 14, "Informe um CPF ou CNPJ com 11 ou 14 dígitos.");
        p.Rua = Texto(p.Rua, "Rua", 100);
        p.Bairro = Texto(p.Bairro, "Bairro", 60);
        p.Numero = Texto(p.Numero, "Número", 10);
        p.Cep = Digitos(p.Cep);
        Exigir(p.Cep.Length == 8, "O CEP deve conter 8 dígitos.");
        p.Telefone = Digitos(p.Telefone);
        Exigir(p.Telefone.Length is 10 or 11, "Informe telefone com DDD.");
        p.Email = Texto(p.Email, "E-mail", 150, !emailObrigatorio).ToLowerInvariant();
        Exigir((!emailObrigatorio && p.Email.Length == 0) || (MailAddress.TryCreate(p.Email, out var email) && email.Address == p.Email), "E-mail inválido.");
        if (p.Email.Length == 0) p.Email = null;
        Exigir(p.CidadeId > 0, "Selecione uma cidade.");
    }
    public static void ValidarSenha(string? senha) => Exigir(senha is { Length: >= 8 and <= 128 }, "A senha deve ter entre 8 e 128 caracteres.");

    public static PedidoGeralEnvio Canonico(PedidoGeralEnvio e)
    {
        e.IdentificadorEnvio = Uuid(e.IdentificadorEnvio);
        Exigir(e.Pedidos is { Count: > 0 and <= 500 }, "Envie de 1 a 500 pedidos no lote.");
        Exigir(e.Pedidos.All(p => p is not null && p.Itens is { Count: > 0 and <= 500 } && p.Parcelas is { Count: >= 1 and <= 3 }), "Itens ou parcelas ausentes.");
        e.DataHoraCriacao = e.DataHoraCriacao.ToUniversalTime();
        e.Pedidos = e.Pedidos.OrderBy(p => p.ClienteId).ToList();
        foreach (var p in e.Pedidos)
        {
            Exigir(p.Itens.All(i => i is not null) && p.Parcelas.All(i => i is not null), "Itens ou parcelas inválidos.");
            p.DataHoraCriacao = p.DataHoraCriacao.ToUniversalTime();
            p.Itens = p.Itens.OrderBy(i => i.ProdutoId).ToList();
            p.Parcelas = p.Parcelas.OrderBy(i => i.NumeroParcela).ToList();
        }
        return e;
    }
}
