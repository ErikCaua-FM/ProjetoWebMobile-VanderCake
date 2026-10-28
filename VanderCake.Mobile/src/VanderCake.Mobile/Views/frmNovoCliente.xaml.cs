using VanderCake.Contracts;
using VanderCake.Models;

namespace VanderCake.Views;

public partial class frmNovoCliente : PaginaBase
{
    private List<Cidade> _cidades = [];
    public frmNovoCliente()
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
        var dados = await App.Sistema.ObterDadosAsync();
        _cidades = dados.Cidades;
        EstadoPicker.ItemsSource = dados.Estados.OrderBy(e => e.Uf).ToList();
    }

    private void OnEstado(object sender, EventArgs e)
    {
        CidadePicker.ItemsSource = EstadoPicker.SelectedItem is Estado estado ? _cidades.Where(c => c.EstadoId == estado.Id).OrderBy(c => c.Nome).ToList() : new List<Cidade>();
    }

    private async void OnAtualizar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        await App.Sistema.AtualizarAsync();
        await CarregarAsync();
    });
    private async void OnSalvar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        await App.Sistema.CadastrarClienteAsync(new NovoClienteRequisicao { Nome = NomeEntry.Text ?? "", Documento = DocumentoEntry.Text ?? "", Telefone = TelefoneEntry.Text ?? "", Email = EmailEntry.Text, Cep = CepEntry.Text ?? "", Rua = RuaEntry.Text ?? "", Numero = NumeroEntry.Text ?? "", Bairro = BairroEntry.Text ?? "", CidadeId = (CidadePicker.SelectedItem as Cidade)?.Id ?? 0 });
        await DisplayAlertAsync("Cliente cadastrado", "O cliente foi incluído na sua carteira.", "OK");
        await Navigation.PopAsync();
    });
}
