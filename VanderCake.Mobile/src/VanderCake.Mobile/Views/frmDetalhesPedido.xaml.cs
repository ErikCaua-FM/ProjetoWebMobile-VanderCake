using VanderCake.Rules;

namespace VanderCake.Views;

public partial class frmDetalhesPedido : PaginaBase
{
    private readonly string _identificador;
    private readonly int _clienteId;
    public frmDetalhesPedido(string identificador, int clienteId)
    {
        InitializeComponent();
        BindingContext = this;
        _identificador = identificador;
        _clienteId = clienteId;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ExecutarAsync(CarregarAsync);
    }

    private async Task CarregarAsync()
    {
        var resumo = await App.Sistema.ObterDetalheAsync(_identificador, _clienteId);
        var pedido = resumo.Pedido;
        ClienteLabel.Text = pedido.ClienteNome;
        PedidoLabel.Text = $"{pedido.Titulo} · {pedido.DataTexto}";
        StatusLabel.Text = resumo.SituacaoTexto;
        MotivoLabel.Text = resumo.MotivoCancelamento;
        MotivoLabel.IsVisible = !string.IsNullOrWhiteSpace(resumo.MotivoCancelamento);
        BindableLayout.SetItemsSource(ItensLayout, pedido.Itens);
        TotalSolicitadoLabel.Text = $"Total solicitado: {Dinheiro.Formatar(pedido.ValorTotalSolicitado)}";
        TotalAtendidoLabel.Text = pedido.ValorTotalAtendido is decimal valor ? $"Total atendido: {Dinheiro.Formatar(valor)}" : "Atendimento ainda não definido";
        CondicaoLabel.Text = pedido.CondicaoPagamentoDescricao;
        BindableLayout.SetItemsSource(ParcelasLayout, pedido.Parcelas);
    }

    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
}
