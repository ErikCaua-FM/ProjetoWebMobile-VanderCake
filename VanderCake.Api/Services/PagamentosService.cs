using System.Data;
using Dapper;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class PagamentosService(Banco banco, IConfiguration configuracao)
{
    public async Task<object> SalvarAsync(PagamentoCadastroRequest r, int? id, bool condicao)
    {
        r.Descricao = Regras.Texto(r.Descricao, "Descrição", condicao ? 60 : 50);
        if (condicao)
            Regras.Exigir(r.Dias is not null && new[] { "0", "7/14", "7/14/21" }.Contains(string.Join('/', r.Dias)),
                "Selecione à vista, 7/14 ou 7/14/21 dias.");
        string tabela = condicao ? "CondicoesPagamento" : "FormasPagamento";
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var anterior = await c.QuerySingleOrDefaultAsync<FormaPagamento>($"SELECT * FROM {tabela} WHERE Id=@id FOR UPDATE", new { id }, tx)
                ?? throw new ApiException(404, "Cadastro não encontrado.");
            Regras.Exigir(anterior.Versao == r.Versao, "O cadastro mudou. Atualize e tente novamente.", 409);
            bool usado = await c.ExecuteScalarAsync<int>(condicao
                ? "SELECT COUNT(*) FROM Pedidos WHERE CondicaoPagamentoId=@id"
                : "SELECT COUNT(*) FROM ParcelasPedido WHERE FormaPagamentoId=@id", new { id }, tx) > 0;
            var diasAtuais = condicao
                ? (await c.QueryAsync<int>("SELECT DiasAposConfirmacao FROM ParcelasCondicaoPagamento WHERE CondicaoPagamentoId=@id ORDER BY NumeroParcela", new { id }, tx)).ToArray()
                : [];
            Regras.Exigir(!usado || (anterior.Descricao == r.Descricao && (!condicao || diasAtuais.SequenceEqual(r.Dias))),
                "Este cadastro já está no histórico. Mantenha descrição/prazos e altere somente a atividade, ou cadastre outro.", 409);
            await c.ExecuteAsync($"UPDATE {tabela} SET Descricao=@Descricao,Ativo=@Ativo,Versao=Versao+1 WHERE Id=@id", new { id, r.Descricao, r.Ativo }, tx);
            if (condicao && !diasAtuais.SequenceEqual(r.Dias))
            {
                await c.ExecuteAsync("DELETE FROM ParcelasCondicaoPagamento WHERE CondicaoPagamentoId=@id", new { id }, tx);
                for (int i = 0; i < r.Dias.Length; i++)
                    await c.ExecuteAsync("INSERT INTO ParcelasCondicaoPagamento (CondicaoPagamentoId,NumeroParcela,DiasAposConfirmacao) VALUES (@id,@numero,@dias)", new { id, numero = i + 1, dias = r.Dias[i] }, tx);
            }
        }
        else
        {
            id = await c.ExecuteScalarAsync<int>($"INSERT INTO {tabela} (Descricao,Ativo) VALUES (@Descricao,@Ativo); SELECT LAST_INSERT_ID();", r, tx);
            if (condicao)
                for (int i = 0; i < r.Dias.Length; i++)
                    await c.ExecuteAsync("INSERT INTO ParcelasCondicaoPagamento (CondicaoPagamentoId,NumeroParcela,DiasAposConfirmacao) VALUES (@id,@numero,@dias)", new { id, numero = i + 1, dias = r.Dias[i] }, tx);
        }
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return new { Id = id };
    }

    public async Task<object> ReceberAsync(int id, int administradorId)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var conta = await c.QuerySingleOrDefaultAsync<ContaReceber>("SELECT * FROM ContasReceber WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Parcela não encontrada.");
        if (conta.Status == "Pendente")
        {
            var fuso = TimeZoneInfo.FindSystemTimeZoneById(configuracao["Loja:FusoHorario"] ?? "America/Sao_Paulo");
            conta.Status = "Paga";
            conta.DataPagamento = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, fuso).Date;
            conta.AdministradorBaixaId = administradorId;
            await c.ExecuteAsync("UPDATE ContasReceber SET Status='Paga',DataPagamento=@DataPagamento,AdministradorBaixaId=@AdministradorBaixaId WHERE Id=@Id", conta, tx);
            int vendedorId = await c.ExecuteScalarAsync<int>("SELECT pg.VendedorId FROM Pedidos p JOIN PedidoGeral pg ON pg.Id=p.PedidoGeralId WHERE p.Id=@PedidoId", conta, tx);
            await EventosService.RegistrarAsync(c, tx, "Pagamentos", vendedorId);
        }
        // Repetir uma baixa não troca data, responsável, valor ou meio de pagamento.
        await tx.CommitAsync();
        return new { conta.Id, conta.Status, DataPagamento = conta.DataPagamento?.ToString("yyyy-MM-dd"), conta.AdministradorBaixaId };
    }
}
