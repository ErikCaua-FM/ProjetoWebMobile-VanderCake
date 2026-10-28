using VanderCake.Models;
using VanderCake.Rules;

namespace VanderCake.Views;

public partial class frmCatalogo : PaginaBase
{
    private readonly int _clienteId;
    private List<Produto> _produtos = [];
    private PedidoResumo? _ultimoPedido;
    private string _pesquisa = "";
    private bool _primeira = true;
    public frmCatalogo(int clienteId)
    {
        InitializeComponent();
        BindingContext = this;
        _clienteId = clienteId;
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
        _produtos = dados.Produtos.Where(p => p.Ativo).OrderBy(p => p.Nome).ToList();
        int categoria = (CategoriasPicker.SelectedItem as Categoria)?.Id ?? 0;
        var categorias = new List<Categoria>
        {
            new()
            {
                Id = 0,
                Nome = "Todas as categorias"
            }
        };
        categorias.AddRange(dados.Categorias.Where(c => c.Ativo).OrderBy(c => c.Nome));
        CategoriasPicker.ItemsSource = categorias;
        CategoriasPicker.SelectedItem = categorias.FirstOrDefault(c => c.Id == categoria) ?? categorias[0];
        _ultimoPedido = await App.Sistema.ObterUltimoPedidoClienteAsync(_clienteId);
        ComprarNovamenteCard.IsVisible = _ultimoPedido is not null;
        if (_ultimoPedido is not null)
        {
            UltimoPedidoLabel.Text = $"{_ultimoPedido.Pedido.Titulo} · {_ultimoPedido.Pedido.DataTexto} · {_ultimoPedido.SituacaoTexto}";
            BindableLayout.SetItemsSource(UltimoPedidoItensLayout, _ultimoPedido.Pedido.Itens);
        }
        await AtualizarCarrinhoAsync();
        Filtrar();
    }

    private async Task AtualizarCarrinhoAsync()
    {
        var pedido = await App.Sistema.ObterCarrinhoAsync(_clienteId);
        ClienteLabel.Text = pedido.ClienteNome;
        CarrinhoButton.Text = $"Ver carrinho · {pedido.Itens.Sum(i => (long)i.QuantidadeSolicitada)} un. · {Dinheiro.Formatar(pedido.ValorTotalSolicitado)}";
    }

    private void Filtrar()
    {
        int categoria = (CategoriasPicker.SelectedItem as Categoria)?.Id ?? 0;
        ProdutosView.ItemsSource = _produtos.Where(p => (categoria == 0 || p.CategoriaId == categoria) && p.Nome.Contains(_pesquisa, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void OnPesquisar(object sender, TextChangedEventArgs e)
    {
        _pesquisa = e.NewTextValue?.Trim() ?? "";
        Filtrar();
    }

    private void OnCategoria(object sender, EventArgs e) => Filtrar();
    private async Task AdicionarAsync(Produto produto) => await ExecutarAsync(async () =>
    {
        var texto = await DisplayPromptAsync(produto.Nome, "Quantas unidades deseja acrescentar?", "Adicionar", "Cancelar", keyboard: Keyboard.Numeric, initialValue: "1", maxLength: 10);
        if (texto is null)
            return;
        if (!int.TryParse(texto, out int quantidade) || quantidade <= 0)
            throw new InvalidOperationException("Informe uma quantidade inteira positiva.");
        await App.Sistema.AdicionarProdutoAsync(_clienteId, produto.Id, quantidade);
        await AtualizarCarrinhoAsync();
    });
    private async void OnProduto(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Produto p)
            await AdicionarAsync(p);
    }

    private async void OnAdicionar(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Produto p)
            await AdicionarAsync(p);
    }

    private async void OnComprarNovamente(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (_ultimoPedido is null)
            return;
        var carrinho = await App.Sistema.ObterCarrinhoAsync(_clienteId);
        bool substituir = carrinho.Itens.Count > 0 || carrinho.CondicaoPagamentoId != 0 || carrinho.Parcelas.Count > 0;
        if (substituir && !await DisplayAlertAsync("Substituir carrinho?",
            "Os itens atuais serão substituídos pelos do último pedido. A condição e as formas de pagamento serão selecionadas novamente.",
            "Substituir", "Cancelar"))
            return;
        await App.Sistema.ComprarNovamenteAsync(_clienteId, _ultimoPedido.IdentificadorEnvio, substituir);
        await AtualizarCarrinhoAsync();
        await Navigation.PushAsync(new frmCarrinho(_clienteId));
    });

    private async void OnCarrinho(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmCarrinho(_clienteId)));
    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
}
