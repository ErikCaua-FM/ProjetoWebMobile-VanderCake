using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;

namespace VanderCake.Views;

public partial class frmListaPedidos : PaginaBase
{
    private string? _identificador;
    public bool PodeEditar { get; private set; } = true;

    public frmListaPedidos()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(CarregarAsync);
    }

    private async Task CarregarAsync()
    {
        var registro = await App.Sistema.ObterLoteAbertoAsync();
        var lote = registro is null ? null : JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        _identificador = lote?.IdentificadorEnvio;
        PodeEditar = registro?.SituacaoLocal != "EnvioIncerto";
        OnPropertyChanged(nameof(PodeEditar));
        var pedidos = lote?.Pedidos ?? [];
        BindableLayout.SetItemsSource(PedidosLayout, pedidos);
        VazioLabel.IsVisible = pedidos.Count == 0;
        TotalLabel.Text = Dinheiro.Formatar(lote?.TotalSolicitado ?? 0);
        SituacaoLabel.Text = PodeEditar ? $"{pedidos.Count} pedido(s) aguardando envio. A loja confirmará o atendimento." : "O resultado do envio precisa ser verificado. A lista está protegida contra alterações até a confirmação.";
        EnviarButton.Text = PodeEditar ? "Enviar para a Loja" : "Verificar / retomar envio";
        EnviarButton.IsVisible = pedidos.Count > 0;
        var carrinhos = await App.Sistema.ObterCarrinhosPendentesAsync();
        BindableLayout.SetItemsSource(CarrinhosLayout, carrinhos);
        CarrinhosLabel.IsVisible = carrinhos.Count > 0;
    }

    private async void OnEditar(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Pedido pedido)
            await ExecutarAsync(async () =>
            {
                await App.Sistema.AbrirCarrinhoAsync(pedido.ClienteId);
                await Navigation.PushAsync(new frmCatalogo(pedido.ClienteId));
            });
    }

    private async void OnDetalhes(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Pedido pedido && _identificador is not null)
            await ExecutarAsync(() => Navigation.PushAsync(new frmDetalhesPedido(_identificador, pedido.ClienteId)));
    }

    private async void OnRemover(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is not Pedido pedido)
            return;
        await ExecutarAsync(async () =>
        {
            if (!await DisplayAlertAsync("Remover pedido", $"Remover o pedido de {pedido.ClienteNome} da lista?", "Remover", "Cancelar"))
                return;
            await App.Sistema.RemoverPedidoAsync(pedido.ClienteId);
            await CarregarAsync();
        });
    }

    private async void OnDescartarCarrinho(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is not Pedido pedido)
            return;
        await ExecutarAsync(async () =>
        {
            if (!await DisplayAlertAsync("Descartar carrinho", $"Descartar a edição de {pedido.ClienteNome}? Um pedido já concluído na lista será preservado.", "Descartar", "Cancelar"))
                return;
            await App.Sistema.DescartarCarrinhoAsync(pedido.ClienteId);
            await CarregarAsync();
        });
    }

    private async void OnEnviar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (PodeEditar && !await DisplayAlertAsync("Enviar para a Loja", "Enviar todos os pedidos desta lista? Depois do recebimento, eles serão analisados pela loja.", "Enviar", "Revisar"))
            return;
        try
        {
            var recebido = await App.Sistema.EnviarPedidosAsync();
            await DisplayAlertAsync("Recebido pela loja", $"Lote #{recebido.Id} recebido. Os pedidos continuam disponíveis no histórico de cada cliente.", "OK");
        }
        finally
        {
            if (App.Sistema.TemSessao)
                await CarregarAsync();
        }
    });
    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
}
