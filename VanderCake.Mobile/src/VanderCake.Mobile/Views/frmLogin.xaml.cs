namespace VanderCake.Views;

public partial class frmLogin : PaginaBase
{
    public frmLogin()
    {
        InitializeComponent();
        BindingContext = this;
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private async void OnEntrar(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        await App.Sistema.EntrarAsync(EmailEntry.Text ?? "", SenhaEntry.Text ?? "");
        SenhaEntry.Text = "";
        App.TrocarRaiz(new frmClientes());
    });
}
