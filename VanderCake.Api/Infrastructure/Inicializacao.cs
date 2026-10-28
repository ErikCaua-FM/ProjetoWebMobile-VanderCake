using Dapper;
using Microsoft.AspNetCore.Identity;
using VanderCake.Api.Contracts;

namespace VanderCake.Api.Infrastructure;

public static class Inicializacao
{
    public static async Task CriarAdministradorAsync(IServiceProvider servicos)
    {
        Console.WriteLine("Cadastro do administrador da loja. A senha não será exibida.");
        await using var c = await servicos.GetRequiredService<Banco>().AbrirAsync();
        var cidades = await c.QueryAsync("SELECT c.Id, c.Nome, e.Uf FROM Cidades c JOIN Estados e ON e.Id=c.EstadoId ORDER BY e.Uf,c.Nome");
        foreach (var cidade in cidades) Console.WriteLine($"{cidade.Id}: {cidade.Nome}/{cidade.Uf}");
        var r = new PessoaRequest
        {
            Nome = Ler("Nome completo"), Documento = Ler("CPF ou CNPJ (somente números)"),
            Rua = Ler("Rua"), Numero = Ler("Número"), Bairro = Ler("Bairro"),
            Cep = Ler("CEP"), CidadeId = int.Parse(Ler("ID da cidade da lista acima")),
            Telefone = Ler("Telefone com DDD"), Email = Ler("E-mail de acesso"), Senha = LerSenha()
        };
        Console.Write("Repita a senha: ");
        Regras.Exigir(r.Senha == LerSenha(false), "As senhas não conferem.");
        Regras.ValidarPessoa(r, true);
        Regras.ValidarSenha(r.Senha);
        var hash = servicos.GetRequiredService<IPasswordHasher<object>>().HashPassword(new object(), r.Senha!);
        var p = new DynamicParameters(r);
        p.Add("SenhaHash", hash);
        try
        {
            await c.ExecuteAsync("""
                INSERT INTO Administradores (Nome,Documento,Rua,Numero,Bairro,Cep,CidadeId,Telefone,Email,SenhaHash)
                VALUES (@Nome,@Documento,@Rua,@Numero,@Bairro,@Cep,@CidadeId,@Telefone,@Email,@SenhaHash)
                """, p);
            Console.WriteLine("Administrador criado. Inicie a API e use esse e-mail e senha na WEB.");
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062)
        { Console.WriteLine("O e-mail ou documento já está cadastrado. Nenhuma conta foi sobrescrita."); Environment.ExitCode = 1; }
    }

    private static string Ler(string campo) { Console.Write($"{campo}: "); return Console.ReadLine()?.Trim() ?? ""; }
    private static string LerSenha(bool titulo = true)
    {
        if (titulo) Console.Write("Senha (8 a 128 caracteres): ");
        if (Console.IsInputRedirected) return Console.ReadLine() ?? "";
        var texto = new System.Text.StringBuilder();
        while (true)
        {
            var tecla = Console.ReadKey(true);
            if (tecla.Key == ConsoleKey.Enter) break;
            if (tecla.Key == ConsoleKey.Backspace) { if (texto.Length > 0) texto.Length--; }
            else if (!char.IsControl(tecla.KeyChar) && texto.Length < 128) texto.Append(tecla.KeyChar);
        }
        Console.WriteLine();
        return texto.ToString();
    }
}
