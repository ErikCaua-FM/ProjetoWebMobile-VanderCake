using VanderCake.Contracts;
using VanderCake.Models;

namespace VanderCake.Rules;

public static class RegrasPedido
{
    public static void Recalcular(Pedido pedido)
    {
        pedido.ValorTotalSolicitado = pedido.Itens.Sum(i => i.SubtotalSolicitado);
        if (pedido.ValorTotalSolicitado > Dinheiro.Limite)
            throw new InvalidOperationException("O valor do pedido ultrapassa o limite permitido.");
        if (pedido.Parcelas.Count == 0)
            return;
        var valores = Dinheiro.Dividir(pedido.ValorTotalSolicitado, pedido.Parcelas.Count);
        for (int i = 0; i < pedido.Parcelas.Count; i++)
            pedido.Parcelas[i].ValorPrevisto = valores[i];
    }

    public static void AplicarCondicao(Pedido pedido, CondicaoPagamento condicao)
    {
        var regras = condicao.Parcelas.OrderBy(p => p.NumeroParcela).ToList();
        int[] prazos = regras.Select(p => p.DiasAposConfirmacao).ToArray();
        if (!condicao.Ativo || !(prazos.SequenceEqual(new[] { 0 }) || prazos.SequenceEqual(new[] { 7, 14 }) || prazos.SequenceEqual(new[] { 7, 14, 21 })) || !regras.Select(p => p.NumeroParcela).SequenceEqual(Enumerable.Range(1, regras.Count)))
            throw new InvalidOperationException("A condição recebida da API não possui os prazos previstos.");
        var anteriores = pedido.Parcelas.ToDictionary(p => p.NumeroParcela);
        pedido.CondicaoPagamentoId = condicao.Id;
        pedido.CondicaoPagamentoDescricao = condicao.Descricao;
        pedido.Parcelas = regras.Select(r => new ParcelaPedido { NumeroParcela = r.NumeroParcela, DiasAposConfirmacao = r.DiasAposConfirmacao, FormaPagamentoId = anteriores.GetValueOrDefault(r.NumeroParcela)?.FormaPagamentoId ?? 0, FormaPagamentoNome = anteriores.GetValueOrDefault(r.NumeroParcela)?.FormaPagamentoNome ?? "" }).ToList();
        Recalcular(pedido);
    }

    public static void Validar(Pedido pedido, DadosSincronizacao dados)
    {
        if (!dados.Clientes.Any(c => c.Id == pedido.ClienteId && c.Ativo && c.VendedorId == dados.Vendedor.Id))
            throw new InvalidOperationException("O cliente não está ativo na sua carteira.");
        if (pedido.Itens.Count == 0)
            throw new InvalidOperationException("Adicione pelo menos um produto.");
        if (pedido.Itens.Select(i => i.ProdutoId).Distinct().Count() != pedido.Itens.Count)
            throw new InvalidOperationException("O pedido contém produtos duplicados.");
        foreach (var item in pedido.Itens)
        {
            var produto = dados.Produtos.FirstOrDefault(p => p.Id == item.ProdutoId && p.Ativo);
            if (produto is null)
                throw new InvalidOperationException($"O produto {item.NomeProduto} não está disponível. Remova-o do carrinho.");
            if (item.QuantidadeSolicitada < 1 || item.PrecoUnitario <= 0 || decimal.Round(item.PrecoUnitario, 2) != item.PrecoUnitario)
                throw new InvalidOperationException("Revise as quantidades e os preços do pedido.");
            if (item.PrecoUnitario != produto.Preco)
                throw new InvalidOperationException($"O preço de {item.NomeProduto} mudou. Abra o carrinho e use Atualizar preços.");
        }

        var condicao = dados.CondicoesPagamento.FirstOrDefault(c => c.Id == pedido.CondicaoPagamentoId && c.Ativo) ?? throw new InvalidOperationException("Selecione a condição de pagamento.");
        var regras = condicao.Parcelas.OrderBy(p => p.NumeroParcela).ToList();
        if (regras.Count != pedido.Parcelas.Count)
            throw new InvalidOperationException("Revise as parcelas do pedido.");
        for (int i = 0; i < regras.Count; i++)
        {
            var parcela = pedido.Parcelas[i];
            if (parcela.NumeroParcela != regras[i].NumeroParcela || parcela.DiasAposConfirmacao != regras[i].DiasAposConfirmacao || !dados.FormasPagamento.Any(f => f.Id == parcela.FormaPagamentoId && f.Ativo))
                throw new InvalidOperationException($"Escolha uma forma de pagamento válida para a {i + 1}ª parcela.");
        }

        Recalcular(pedido);
    }

    public static PedidoGeralEnvio CriarEnvio(PedidoGeral lote) => new()
    {
        IdentificadorEnvio = lote.IdentificadorEnvio,
        DataHoraCriacao = lote.DataHoraCriacao,
        Pedidos = lote.Pedidos.Select(p => new PedidoEnvio { ClienteId = p.ClienteId, DataHoraCriacao = p.DataHoraCriacao, CondicaoPagamentoId = p.CondicaoPagamentoId, Itens = p.Itens.Select(i => new ItemPedidoEnvio { ProdutoId = i.ProdutoId, QuantidadeSolicitada = i.QuantidadeSolicitada, PrecoUnitario = i.PrecoUnitario }).ToList(), Parcelas = p.Parcelas.Select(f => new ParcelaEnvio { NumeroParcela = f.NumeroParcela, FormaPagamentoId = f.FormaPagamentoId }).ToList() }).ToList()
    };
}
