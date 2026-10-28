using VanderCake.Models;

namespace VanderCake.Views;

public partial class frmPedidosCliente : PaginaBase
{
    private readonly Cliente _cliente;
    public frmPedidosCliente(Cliente cliente)
    {
        InitializeComponent();
        BindingContext = this;
        _cliente = cliente;
        NomeLabel.Text = cliente.Nome;
        DocumentoLabel.Text = cliente.Identificacao;
        EnderecoLabel.Text = cliente.Endereco;
        NovoPedidoButton.IsEnabled = cliente.Ativo;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(CarregarAsync);
    }

    private async Task CarregarAsync()
    {
        var dados = await App.Sistema.ObterDadosAsync();
        var atual = dados.Clientes.SingleOrDefault(c => c.Id == _cliente.Id);
        NovoPedidoButton.IsEnabled = atual?.Ativo == true;
        if (atual is not null)
        {
            NomeLabel.Text = atual.Nome;
            DocumentoLabel.Text = atual.Identificacao;
            EnderecoLabel.Text = atual.Endereco;
        }
        PedidosView.ItemsSource = await App.Sistema.ObterPedidosClienteAsync(_cliente.Id);
    }
    private async void OnNovo(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        await App.Sistema.AbrirCarrinhoAsync(_cliente.Id);
        await Navigation.PushAsync(new frmCatalogo(_cliente.Id));
    });
    private async void OnDetalhes(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is PedidoResumo resumo)
            await ExecutarAsync(() => Navigation.PushAsync(new frmDetalhesPedido(resumo.IdentificadorEnvio, _cliente.Id)));
    }

    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
    private async void OnLista(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmListaPedidos()));
}
