using VanderCake.Models;
using VanderCake.Rules;

namespace VanderCake.Views;

public partial class frmCarrinho : PaginaBase
{
    private readonly int _clienteId;
    private bool _renderizando;
    public frmCarrinho(int clienteId)
    {
        InitializeComponent();
        BindingContext = this;
        _clienteId = clienteId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(CarregarAsync);
    }

    private async Task CarregarAsync()
    {
        var pedido = await App.Sistema.ObterCarrinhoAsync(_clienteId);
        var dados = await App.Sistema.ObterDadosAsync();
        _renderizando = true;
        try
        {
            ClienteLabel.Text = pedido.ClienteNome;
            TotalLabel.Text = Dinheiro.Formatar(pedido.ValorTotalSolicitado);
            // Reatribuir a lista atualiza quantidade e subtotal, além do total geral.
            BindableLayout.SetItemsSource(ItensLayout, pedido.Itens);
            VazioLabel.IsVisible = pedido.Itens.Count == 0;
            var condicoes = dados.CondicoesPagamento.Where(c => c.Ativo).ToList();
            CondicaoPicker.ItemsSource = condicoes;
            CondicaoPicker.SelectedItem = condicoes.FirstOrDefault(c => c.Id == pedido.CondicaoPagamentoId);
            ParcelasLayout.Children.Clear();
            foreach (var parcela in pedido.Parcelas)
            {
                var formas = dados.FormasPagamento.Where(f => f.Ativo).ToList();
                var seletor = new Picker
                {
                    Title = "Forma de pagamento",
                    ItemsSource = formas,
                    ItemDisplayBinding = new Binding(nameof(FormaPagamento.Descricao)),
                    SelectedItem = formas.FirstOrDefault(f => f.Id == parcela.FormaPagamentoId)
                };
                int numero = parcela.NumeroParcela;
                // O evento é ligado depois da seleção inicial, evitando gravações na renderização.
                seletor.SelectedIndexChanged += async (_, _) =>
                {
                    if (_renderizando || seletor.SelectedItem is not FormaPagamento forma)
                        return;
                    await ExecutarAsync(async () =>
                    {
                        await App.Sistema.SelecionarFormaAsync(_clienteId, numero, forma.Id);
                    });
                };
                ParcelasLayout.Children.Add(new VerticalStackLayout { Spacing = 3, Children = { new Label { Text = $"{parcela.Descricao} · {Dinheiro.Formatar(parcela.ValorPrevisto)}", FontFamily = "OpenSansSemibold" }, seletor } });
            }
        }
        finally
        {
            _renderizando = false;
        }
    }

    private async void OnCondicao(object sender, EventArgs e)
    {
        if (_renderizando || CondicaoPicker.SelectedItem is not CondicaoPagamento condicao)
            return;
        await ExecutarAsync(async () =>
        {
            await App.Sistema.SelecionarCondicaoAsync(_clienteId, condicao.Id);
            await CarregarAsync();
        });
    }

    private async Task AlterarAsync(object sender, int delta, bool remover = false)
    {
        if ((sender as Button)?.CommandParameter is not ItemPedido item)
            return;
        await ExecutarAsync(async () =>
        {
            int quantidade = remover ? 0 : checked(item.QuantidadeSolicitada + delta);
            await App.Sistema.AlterarQuantidadeAsync(_clienteId, item.ProdutoId, quantidade);
            await CarregarAsync();
        });
    }

    private async void OnAumentar(object sender, EventArgs e) => await AlterarAsync(sender, 1);
    private async void OnDiminuir(object sender, EventArgs e) => await AlterarAsync(sender, -1);
    private async void OnRemover(object sender, EventArgs e) => await AlterarAsync(sender, 0, true);
    private async void OnPrecos(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (!await DisplayAlertAsync("Atualizar preços", "Aplicar os preços atuais do catálogo a este carrinho?", "Atualizar", "Cancelar"))
            return;
        await App.Sistema.AtualizarAsync();
        await App.Sistema.AtualizarPrecosAsync(_clienteId);
        await CarregarAsync();
    });
    private async void OnAcrescentar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        await App.Sistema.AcrescentarPedidoAsync(_clienteId);
        await DisplayAlertAsync("Pedido acrescentado", "O pedido já está na lista geral e no histórico deste cliente. Ele será enviado com os demais pedidos.", "OK");
        await VoltarDoCarrinhoAsync();
    });
    private async void OnDescartar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (!await DisplayAlertAsync("Descartar edição", "Descartar este carrinho? Se havia um pedido na lista, sua versão anterior será mantida.", "Descartar", "Continuar"))
            return;
        await App.Sistema.DescartarCarrinhoAsync(_clienteId);
        await VoltarDoCarrinhoAsync();
    });
}
