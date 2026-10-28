using System.Globalization;

namespace VanderCake.Rules;

public static class Dinheiro
{
    public const decimal Limite = 9_999_999_999.99m;
    public static string Formatar(decimal valor) => valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
    public static List<decimal> Dividir(decimal total, int quantidade)
    {
        if (total < 0 || total > Limite || decimal.Round(total, 2) != total || quantidade < 1)
            throw new InvalidOperationException("Valor ou quantidade de parcelas inválidos.");
        decimal baseCentavos = decimal.Truncate(total * 100 / quantidade);
        var parcelas = Enumerable.Repeat(baseCentavos / 100, quantidade).ToList();
        parcelas[^1] += total - parcelas.Sum();
        return parcelas;
    }
}
