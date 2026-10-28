using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using VanderCake.Api.Contracts;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class CadastroService(Banco banco, IPasswordHasher<object> senhas)
{
    public async Task<object> SalvarProdutoAsync(ProdutoRequest r, int? id)
    {
        r.Nome = Regras.Texto(r.Nome, "Nome do produto", 100);
        r.Descricao = Regras.Texto(r.Descricao, "Descrição", 4000, true);
        Regras.Exigir(r.PrecoCentavos > 0 && r.PrecoCentavos <= 999999999999L && r.Estoque >= 0, "Preço ou estoque inválido.");
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var anterior = await c.QuerySingleOrDefaultAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id FOR UPDATE", new { id }, tx)
                ?? throw new ApiException(404, "Produto não encontrado.");
            Regras.Exigir(anterior.Versao == r.Versao, "O produto ou estoque mudou. Reabra o cadastro.", 409);
        }
        Regras.Exigir((await c.ExecuteScalarAsync<int?>("SELECT Id FROM Categorias WHERE Id=@CategoriaId AND Ativo=1 FOR SHARE", r, tx)).HasValue, "Selecione uma categoria ativa.");
        var parametros = new { id, r.Nome, r.Descricao, r.CategoriaId, Preco = r.PrecoCentavos / 100m, r.Estoque, r.Ativo };
        if (id is null)
            id = await c.ExecuteScalarAsync<int>("""
                INSERT INTO Produtos (Nome, Descricao, CategoriaId, Preco, Estoque, Ativo)
                VALUES (@Nome,@Descricao,@CategoriaId,@Preco,@Estoque,1); SELECT LAST_INSERT_ID();
                """, parametros, tx);
        else
            await c.ExecuteAsync("""
                UPDATE Produtos SET Nome=@Nome, Descricao=@Descricao, CategoriaId=@CategoriaId,
                    Preco=@Preco, Estoque=@Estoque, Ativo=@Ativo, Versao=Versao+1 WHERE Id=@id
                """, parametros, tx);
        var produto = await c.QuerySingleAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id", new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return ProjecaoWeb.Produto(produto);
    }

    public async Task<Pessoa> SalvarPessoaAsync(PessoaRequest r, int? id, bool vendedor, int? autorMobile = null)
    {
        Regras.ValidarPessoa(r, vendedor);
        if (vendedor && (id is null || !string.IsNullOrEmpty(r.Senha))) Regras.ValidarSenha(r.Senha);
        if (autorMobile.HasValue) r.VendedorId = autorMobile.Value;
        string tabela = vendedor ? "Vendedores" : "Clientes";
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        Pessoa? anterior = null;
        Cliente? clienteAnterior = null;
        if (id.HasValue)
        {
            if (vendedor) anterior = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR UPDATE", new { id }, tx);
            else anterior = clienteAnterior = await c.QuerySingleOrDefaultAsync<Cliente>("SELECT * FROM Clientes WHERE Id=@id FOR UPDATE", new { id }, tx);
            Regras.Exigir(anterior is not null, "Cadastro não encontrado.", 404);
            Regras.Exigir(anterior!.Versao == r.Versao, "O cadastro mudou. Reabra-o antes de alterar.", 409);
        }
        Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Cidades WHERE Id=@CidadeId", r, tx) == 1, "Selecione uma cidade cadastrada.");
        if (!vendedor)
        {
            var responsavel = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR SHARE", new { id = r.VendedorId }, tx);
            Regras.Exigir(responsavel is not null && (responsavel.Ativo || clienteAnterior?.VendedorId == r.VendedorId), "Selecione um vendedor ativo.");
        }
        var p = new DynamicParameters(r);
        p.Add("id", id);
        if (vendedor && !string.IsNullOrEmpty(r.Senha)) p.Add("SenhaHash", senhas.HashPassword(new object(), r.Senha));
        if (id is null)
        {
            var extra = vendedor ? "SenhaHash" : "VendedorId";
            id = await c.ExecuteScalarAsync<int>($"""
                INSERT INTO {tabela} (Nome,Documento,Rua,Bairro,Cep,Numero,CidadeId,Telefone,Email,Ativo,{extra})
                VALUES (@Nome,@Documento,@Rua,@Bairro,@Cep,@Numero,@CidadeId,@Telefone,@Email,1,@{extra});
                SELECT LAST_INSERT_ID();
                """, p, tx);
        }
        else
        {
            string extra = vendedor
                ? string.IsNullOrEmpty(r.Senha) ? "" : ", SenhaHash=@SenhaHash, VersaoSeguranca=VersaoSeguranca+1"
                : ", VendedorId=@VendedorId, Ativo=@Ativo";
            await c.ExecuteAsync($"""
                UPDATE {tabela} SET Nome=@Nome, Documento=@Documento, Rua=@Rua, Bairro=@Bairro,
                    Cep=@Cep, Numero=@Numero, CidadeId=@CidadeId, Telefone=@Telefone, Email=@Email,
                    Versao=Versao+1 {extra} WHERE Id=@id
                """, p, tx);
        }
        Pessoa resultado;
        if (vendedor)
            resultado = await c.QuerySingleAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id", new { id }, tx);
        else
            resultado = await c.QuerySingleAsync<Cliente>("""
                SELECT cl.*, ci.Nome AS CidadeNome, e.Uf FROM Clientes cl
                JOIN Cidades ci ON ci.Id=cl.CidadeId JOIN Estados e ON e.Id=ci.EstadoId WHERE cl.Id=@id
                """, new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, vendedor ? "Vendedores" : "Clientes", vendedor ? id : r.VendedorId);
        if (clienteAnterior is not null && clienteAnterior.VendedorId != r.VendedorId)
            await EventosService.RegistrarAsync(c, tx, "Clientes", clienteAnterior.VendedorId);
        await tx.CommitAsync();
        return resultado;
    }

    public async Task<object> AtividadeAsync(int id, AtividadeRequest r)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var vendedor = await c.QuerySingleOrDefaultAsync<Vendedor>("SELECT * FROM Vendedores WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Vendedor não encontrado.");
        Regras.Exigir(vendedor.Versao == r.Versao, "O vendedor mudou. Atualize a lista.", 409);
        await c.ExecuteAsync("UPDATE Vendedores SET Ativo=@Ativo, Versao=Versao+1, VersaoSeguranca=VersaoSeguranca+1 WHERE Id=@id", new { id, r.Ativo }, tx);
        await EventosService.RegistrarAsync(c, tx, "Vendedores", id);
        await tx.CommitAsync();
        return new { Id = id, r.Ativo };
    }

    public async Task<object> ExcluirProdutoAsync(int id, long versao)
    {
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        var produto = await c.QuerySingleOrDefaultAsync<Produto>("SELECT * FROM Produtos WHERE Id=@id FOR UPDATE", new { id }, tx)
            ?? throw new ApiException(404, "Produto não encontrado.");
        Regras.Exigir(produto.Versao == versao, "O produto mudou. Atualize a lista.", 409);
        bool usado = await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ItemPedido WHERE ProdutoId=@id", new { id }, tx) > 0;
        await c.ExecuteAsync(usado ? "UPDATE Produtos SET Ativo=0,Versao=Versao+1 WHERE Id=@id" : "DELETE FROM Produtos WHERE Id=@id", new { id }, tx);
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return new { Desativado = usado };
    }

    public async Task<object> SalvarReferenciaAsync(string tipo, string nome, string? uf, int? estadoId, long versao, int? id, bool ativo = true)
    {
        Regras.Exigir(tipo is "Categorias" or "Estados" or "Cidades", "Cadastro inválido.");
        nome = Regras.Texto(nome, "Nome", tipo == "Categorias" ? 60 : 100);
        if (tipo == "Estados")
        {
            uf = Regras.Texto(uf, "UF", 2).ToUpperInvariant();
            Regras.Exigir(System.Text.RegularExpressions.Regex.IsMatch(uf, "^[A-Z]{2}$"), "Informe uma UF com duas letras.");
        }
        await using var c = await banco.AbrirAsync();
        await using var tx = await c.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        if (id.HasValue)
        {
            var atual = await c.QuerySingleOrDefaultAsync<long?>($"SELECT Versao FROM {tipo} WHERE Id=@id FOR UPDATE", new { id }, tx);
            Regras.Exigir(atual.HasValue, "Cadastro não encontrado.", 404);
            Regras.Exigir(atual == versao, "O cadastro mudou. Atualize-o.", 409);
        }
        if (tipo == "Cidades")
            Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Estados WHERE Id=@estadoId", new { estadoId }, tx) == 1, "Selecione um estado cadastrado.");
        if (tipo == "Categorias" && !ativo && id.HasValue)
            Regras.Exigir(await c.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Produtos WHERE CategoriaId=@id AND Ativo=1", new { id }, tx) == 0,
                "Desative ou mova os produtos ativos desta categoria antes de desativá-la.", 409);
        var p = new { id, nome, uf, estadoId, ativo };
        if (id is null)
        {
            string colunas = tipo switch { "Estados" => "Nome,Uf", "Cidades" => "Nome,EstadoId", _ => "Nome,Ativo" };
            string valores = tipo switch { "Estados" => "@nome,@uf", "Cidades" => "@nome,@estadoId", _ => "@nome,@ativo" };
            id = await c.ExecuteScalarAsync<int>($"INSERT INTO {tipo} ({colunas}) VALUES ({valores}); SELECT LAST_INSERT_ID();", p, tx);
        }
        else
        {
            string extra = tipo switch { "Estados" => ",Uf=@uf", "Cidades" => ",EstadoId=@estadoId", _ => ",Ativo=@ativo" };
            await c.ExecuteAsync($"UPDATE {tipo} SET Nome=@nome,Versao=Versao+1{extra} WHERE Id=@id", p, tx);
        }
        object resultado = tipo switch
        {
            "Estados" => await c.QuerySingleAsync<Estado>("SELECT * FROM Estados WHERE Id=@id", new { id }, tx),
            "Cidades" => await c.QuerySingleAsync<Cidade>("SELECT * FROM Cidades WHERE Id=@id", new { id }, tx),
            _ => await c.QuerySingleAsync<Categoria>("SELECT * FROM Categorias WHERE Id=@id", new { id }, tx)
        };
        await EventosService.RegistrarAsync(c, tx, "Catalogo");
        await tx.CommitAsync();
        return resultado;
    }
}
