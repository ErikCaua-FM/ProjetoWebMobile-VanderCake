using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class RateioService(IDataProtectionProvider protecao)
{
    private readonly IDataProtector _protetor = protecao.CreateProtector("VanderCake.Previa.v1");
    private sealed record Chave(string Hash, DateTimeOffset Expira);

    public static string Assinatura(PedidoGeral lote, IReadOnlyList<Produto> produtos) => Regras.Hash(new
    {
        lote.Id, lote.Versao,
        Pedidos = lote.Pedidos.OrderBy(p => p.Id).Select(p => new
        {
            p.Id, p.ClienteId, p.CondicaoPagamentoId,
            Itens = p.Itens.OrderBy(i => i.ProdutoId).Select(i => new { i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario }),
            Parcelas = p.Parcelas.OrderBy(i => i.NumeroParcela).Select(i => new { i.NumeroParcela, i.DiasAposConfirmacao, i.FormaPagamentoId })
        }),
        Estoques = produtos.OrderBy(p => p.Id).Select(p => new { p.Id, p.Estoque, p.Versao })
    });

    public Previa Gerar(PedidoGeral lote, IReadOnlyList<Produto> produtos)
    {
        Regras.Exigir(lote.Status == "AguardandoConfirmacao", "Este lote já recebeu uma decisão.", 409);
        var pedidos = Regras.Copiar(lote.Pedidos);
        foreach (var produtoId in pedidos.SelectMany(p => p.Itens).Select(i => i.ProdutoId).Distinct())
        {
            var produto = produtos.Single(p => p.Id == produtoId);
            var demandas = pedidos.Where(p => p.Itens.Any(i => i.ProdutoId == produtoId))
                .Select(p => (p.ClienteId, p.Itens.Single(i => i.ProdutoId == produtoId).QuantidadeSolicitada)).ToList();
            var atendimento = Regras.Ratear(produto.Estoque, demandas);
            foreach (var pedido in pedidos)
            {
                var item = pedido.Itens.SingleOrDefault(i => i.ProdutoId == produtoId);
                if (item is not null) item.QuantidadeAtendida = atendimento[pedido.ClienteId];
            }
        }
        foreach (var pedido in pedidos)
        {
            pedido.ValorTotalAtendido = pedido.Itens.Sum(i => i.PrecoUnitario * i.QuantidadeAtendida!.Value);
            var parcelas = Regras.Parcelar(pedido.ValorTotalAtendido.Value, pedido.Parcelas.Count);
            for (int i = 0; i < parcelas.Length; i++) pedido.Parcelas[i].ValorFinal = parcelas[i];
        }
        return new Previa
        {
            TokenPrevia = _protetor.Protect(JsonSerializer.Serialize(new Chave(Assinatura(lote, produtos), DateTimeOffset.UtcNow.AddMinutes(10)))),
            Pedidos = pedidos,
            TotalSolicitado = pedidos.Sum(p => p.ValorTotalSolicitado),
            TotalAtendido = pedidos.Sum(p => p.ValorTotalAtendido!.Value),
            UnidadesCortadas = pedidos.SelectMany(p => p.Itens).Sum(i => (long)i.QuantidadeSolicitada - i.QuantidadeAtendida!.Value)
        };
    }

    public void Validar(string token, PedidoGeral lote, IReadOnlyList<Produto> produtos)
    {
        Chave? chave;
        try { chave = JsonSerializer.Deserialize<Chave>(_protetor.Unprotect(token)); }
        catch (Exception e) when (e is System.Security.Cryptography.CryptographicException or JsonException or ArgumentException)
        {
            throw new ApiException(409, "A prévia não é válida. Atualize os detalhes.");
        }
        Regras.Exigir(chave is not null && chave.Expira > DateTimeOffset.UtcNow && chave.Hash == Assinatura(lote, produtos),
            "O estoque ou a prévia mudou. Confira os dados atualizados antes de confirmar.", 409);
    }
}
