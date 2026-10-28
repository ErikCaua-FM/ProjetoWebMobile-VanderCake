namespace VanderCake.Services;

public interface ISessaoSegura
{
    Task<string?> LerAsync();
    Task SalvarAsync(string conteudo);
    Task LimparAsync();
}
