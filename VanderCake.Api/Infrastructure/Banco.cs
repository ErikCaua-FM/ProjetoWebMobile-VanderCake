using MySqlConnector;

namespace VanderCake.Api.Infrastructure;

public sealed class Banco(IConfiguration configuracao)
{
    public async Task<MySqlConnection> AbrirAsync(CancellationToken ct = default)
    {
        var texto = configuracao.GetConnectionString("VanderCake");
        if (string.IsNullOrWhiteSpace(texto))
            throw new ApiException(503, "Configure ConnectionStrings:VanderCake com dotnet user-secrets ou variável de ambiente.");
        // UUIDs são strings nos contratos do Mobile/Web. Evita converter CHAR(36) em Guid implicitamente.
        var opcoes = new MySqlConnectionStringBuilder(texto)
        {
            DateTimeKind = MySqlDateTimeKind.Utc,
            GuidFormat = MySqlGuidFormat.None
        };
        var conexao = new MySqlConnection(opcoes.ConnectionString);
        try
        {
            await conexao.OpenAsync(ct);
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }
}
