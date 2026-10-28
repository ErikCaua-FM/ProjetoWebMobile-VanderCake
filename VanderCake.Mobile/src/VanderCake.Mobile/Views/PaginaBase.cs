using VanderCake.Helpers;

namespace VanderCake.Views;

public class PaginaBase : ContentPage
{
    private bool _visivel;
    private bool _recarregarPendente;
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _visivel = true;
        App.TempoReal.DadosAtualizados -= AoAtualizarDados;
        App.TempoReal.DadosAtualizados += AoAtualizarDados;
    }
    protected override void OnDisappearing()
    {
        _visivel = false;
        App.TempoReal.DadosAtualizados -= AoAtualizarDados;
        base.OnDisappearing();
    }
    protected virtual Task RecarregarDadosLocaisAsync() => Task.CompletedTask;
    private void AoAtualizarDados(object? sender, EventArgs e) => MainThread.BeginInvokeOnMainThread(async () =>
    {
        if (!_visivel) return;
        if (Ocupado) { _recarregarPendente = true; return; }
        await ExecutarAsync(RecarregarDadosLocaisAsync);
    });
    private string _erro = "";
    public string Erro
    {
        get => _erro;
        private set
        {
            _erro = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemErro));
        }
    }

    public bool TemErro => !string.IsNullOrWhiteSpace(Erro);
    private bool _ocupado;
    public bool Ocupado
    {
        get => _ocupado;
        private set
        {
            _ocupado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Livre));
        }
    }
    public bool Livre => !Ocupado;

    public virtual Task AtualizarAoRetomarAsync() => Task.CompletedTask;
    protected async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        if (Ocupado)
            return false;
        Ocupado = true;
        OnPropertyChanged(nameof(Livre));
        Erro = "";
        try
        {
            await acao();
            return true;
        }
        catch (ApiException ex) when (ex.Status == 401 && this is not frmLogin)
        {
            await DisplayAlertAsync("Sessão", ex.Message, "OK");
            App.TrocarRaiz(new frmLogin());
        }
        catch (OverflowException)
        {
            Erro = "A quantidade informada ultrapassa o limite permitido.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Erro = ex is ApiException or InvalidOperationException or InvalidDataException ? ex.Message : "Não foi possível concluir a operação. Tente novamente; os dados já salvos foram preservados.";
        }
        finally
        {
            Ocupado = false;
            OnPropertyChanged(nameof(Livre));
            if (_recarregarPendente && _visivel)
            {
                _recarregarPendente = false;
                AoAtualizarDados(this, EventArgs.Empty);
            }
        }

        return false;
    }

    protected async Task CarregarComSincronizacaoAsync(Func<Task> carregarLocal)
    {
        await carregarLocal();
        try
        {
            await App.Sistema.AtualizarAsync();
        }
        finally
        {
            if (App.Sistema.TemSessao)
                await carregarLocal();
        }
    }

    protected async Task VoltarDoCarrinhoAsync()
    {
        // O retorno direto dispara OnAppearing do histórico ou da lista e atualiza os pedidos.
        var catalogos = Navigation.NavigationStack.OfType<frmCatalogo>().ToList();
        foreach (var pagina in catalogos)
            Navigation.RemovePage(pagina);
        await Navigation.PopAsync();
    }
}
