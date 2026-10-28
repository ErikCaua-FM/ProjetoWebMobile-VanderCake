using VanderCake.Helpers;
using VanderCake.Models;

namespace VanderCake.Views;

public partial class frmClientes : PaginaBase
{
    private List<Cliente> _clientes = [];
    private string _pesquisa = "";
    private bool _primeira = true;
    public frmClientes()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        bool sincronizar = _primeira;
        _primeira = false;
        await ExecutarAsync(() => sincronizar ? CarregarComSincronizacaoAsync(CarregarAsync) : CarregarAsync());
    }

    private async Task CarregarAsync()
    {
        var dados = await App.Sistema.ObterDadosAsync();
        _clientes = dados.Clientes.OrderByDescending(c => c.Ativo).ThenBy(c => c.Nome).ToList();
        VendedorLabel.Text = $"Olá, {App.Sistema.VendedorAtual?.Nome}";
        SincronizacaoLabel.Text = dados.SincronizadoEm is DateTimeOffset data ? $"Atualizado em {data.ToLocalTime():dd/MM HH:mm}" : "Atualize para carregar os dados da sua carteira.";
        var lote = await App.Sistema.ObterLoteAbertoAsync();
        int quantidade = lote is null ? 0 : JsonDados.Ler<PedidoGeral>(lote.ConteudoJson).Pedidos.Count;
        ListaToolbar.Text = $"Lista de Pedidos ({quantidade})";
        Filtrar();
    }

    private void Filtrar() => ClientesView.ItemsSource = _clientes.Where(c => c.Nome.Contains(_pesquisa, StringComparison.OrdinalIgnoreCase) || c.Documento.Contains(_pesquisa, StringComparison.OrdinalIgnoreCase)).ToList();
    private void OnPesquisar(object sender, TextChangedEventArgs e)
    {
        _pesquisa = e.NewTextValue?.Trim() ?? "";
        Filtrar();
    }

    private async void OnCliente(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Cliente cliente)
            await ExecutarAsync(() => Navigation.PushAsync(new frmPedidosCliente(cliente)));
    }

    private async void OnNovoCliente(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmNovoCliente()));
    private async void OnLista(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmListaPedidos()));
    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
    private async void OnSair(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (!await DisplayAlertAsync("Sair da conta", "Os pedidos não enviados continuarão guardados para esta conta. Deseja sair?", "Sair", "Continuar"))
            return;
        await App.Sistema.SairAsync();
        App.TrocarRaiz(new frmLogin());
    });
}
