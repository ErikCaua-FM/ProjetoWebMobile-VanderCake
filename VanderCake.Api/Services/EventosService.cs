using Dapper;
using Microsoft.AspNetCore.SignalR;
using MySqlConnector;
using VanderCake.Api.Hubs;
using VanderCake.Api.Infrastructure;
using VanderCake.Api.Models;

namespace VanderCake.Api.Services;

public sealed class EventosService(Banco banco, IHubContext<AtualizacoesHub> hub, ILogger<EventosService> logger) : BackgroundService
{
    public static Task RegistrarAsync(MySqlConnection c, MySqlTransaction tx, string tipo, int? vendedorId = null) =>
        c.ExecuteAsync("INSERT INTO EventosAlteracao (Tipo,VendedorId,CriadoEm) VALUES (@tipo,@vendedorId,UTC_TIMESTAMP(6))", new { tipo, vendedorId }, tx);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var c = await banco.AbrirAsync(stoppingToken);
                var eventos = await c.QueryAsync<EventoAlteracao>("SELECT * FROM EventosAlteracao WHERE PublicadoEm IS NULL ORDER BY Id LIMIT 100");
                foreach (var evento in eventos)
                {
                    var aviso = new { evento.Id, evento.Tipo, evento.CriadoEm };
                    if (evento.VendedorId.HasValue)
                        await hub.Clients.Groups("loja", $"vendedor:{evento.VendedorId}").SendAsync("DadosAlterados", aviso, stoppingToken);
                    else
                        await hub.Clients.All.SendAsync("DadosAlterados", aviso, stoppingToken);
                    // Se o processo cair antes desta marca, o aviso pode repetir. Os clientes recarregam dados idempotentemente.
                    await c.ExecuteAsync("UPDATE EventosAlteracao SET PublicadoEm=UTC_TIMESTAMP(6) WHERE Id=@Id", evento);
                }
                await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogWarning("A publicação de atualizações será repetida. Tipo da falha: {Tipo}", ex.GetType().Name);
                try { await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }
    }
}
