using Microsoft.AspNetCore.SignalR.Client;

namespace VanderCake.Services;

// Os avisos não contêm pedidos: a sincronização autenticada continua sendo a fonte dos dados.
public sealed class TempoRealService(AplicacaoService sistema, string baseUrl) : IAsyncDisposable
{
    private readonly SemaphoreSlim _ciclo = new(1, 1);
    private readonly SemaphoreSlim _aviso = new(0, 1);
    private CancellationTokenSource? _cancelamento;
    private Task? _execucao;
    public event EventHandler? DadosAtualizados;

    public void Sinalizar()
    {
        try { _aviso.Release(); } catch (SemaphoreFullException) { }
    }
    public async Task IniciarAsync()
    {
        await _ciclo.WaitAsync();
        try
        {
            if (_execucao is { IsCompleted: false }) return;
            _cancelamento = new CancellationTokenSource();
            _execucao = ExecutarAsync(_cancelamento.Token);
        }
        finally { _ciclo.Release(); }
    }
    public async Task PararAsync()
    {
        await _ciclo.WaitAsync();
        try
        {
            if (_cancelamento is null) return;
            await _cancelamento.CancelAsync();
            if (_execucao is not null) await _execucao;
            _cancelamento.Dispose();
            _cancelamento = null;
            _execucao = null;
        }
        finally { _ciclo.Release(); }
    }
    private async Task ExecutarAsync(CancellationToken ct)
    {
        HubConnection? hub = null;
        int? vendedorId = null;
        var ultima = DateTimeOffset.MinValue;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bool recebeu = _aviso.Wait(0);
                    int? atual = sistema.VendedorAtual?.Id;
                    if (atual != vendedorId)
                    {
                        if (hub is not null) await hub.DisposeAsync();
                        hub = null;
                        vendedorId = atual;
                        ultima = DateTimeOffset.MinValue;
                    }
                    if (sistema.TemSessao)
                    {
                        if (hub is null)
                        {
                            var endereco = new Uri(new Uri(baseUrl), "/hubs/atualizacoes");
                            hub = new HubConnectionBuilder().WithUrl(endereco, o => o.AccessTokenProvider = sistema.ObterTokenAsync)
                                .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15)]).Build();
                            hub.On("DadosAlterados", (System.Text.Json.JsonElement _) => Sinalizar());
                            hub.Reconnected += _ => { Sinalizar(); return Task.CompletedTask; };
                        }
                        if (hub.State == HubConnectionState.Disconnected)
                        {
                            try { await hub.StartAsync(ct); Sinalizar(); }
                            catch (Exception) when (!ct.IsCancellationRequested) { /* O HTTP ainda pode funcionar. */ }
                        }
                        if (recebeu || DateTimeOffset.UtcNow - ultima >= TimeSpan.FromSeconds(30))
                        {
                            await sistema.AtualizarAsync();
                            ultima = DateTimeOffset.UtcNow;
                            DadosAtualizados?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    // Sem apagar SQLite/carrinhos. A próxima passagem tentará recuperar a conexão.
                    System.Diagnostics.Debug.WriteLine($"Atualização adiada: {ex.GetType().Name}");
                }
                if (await _aviso.WaitAsync(TimeSpan.FromSeconds(5), ct)) Sinalizar();
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        finally { if (hub is not null) await hub.DisposeAsync(); }
    }
    public async ValueTask DisposeAsync() { await PararAsync(); _ciclo.Dispose(); _aviso.Dispose(); }
}
