namespace VanderCake.Services;

public sealed class SecureStorageSessao : ISessaoSegura
{
    private const string Chave = "vandercake.sessao.v1";
    public async Task<string?> LerAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(Chave);
        }
        catch
        {
            // Uma chave criptográfica restaurada de outro aparelho pode ser inválida.
            SecureStorage.Default.Remove(Chave);
            return null;
        }
    }

    public Task SalvarAsync(string conteudo) => SecureStorage.Default.SetAsync(Chave, conteudo);
    public Task LimparAsync()
    {
        SecureStorage.Default.Remove(Chave);
        return Task.CompletedTask;
    }
}
