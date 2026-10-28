namespace VanderCake.Views;

public partial class frmInicial : PaginaBase
{
    private bool _iniciou;
    public frmInicial()
    {
        InitializeComponent();
        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_iniciou)
            return;
        _iniciou = true;
        await IniciarAsync();
    }

    private Task IniciarAsync() => ExecutarAsync(async () =>
    {
        await App.Sistema.InicializarAsync();
        App.TrocarRaiz(App.Sistema.TemSessao ? new frmClientes() : new frmLogin());
    });
    private async void OnTentar(object sender, EventArgs e) => await IniciarAsync();
}
