using VanderCake.Services;
using VanderCake.Views;

namespace VanderCake;

public partial class App : Application
{
    public static AplicacaoService Sistema { get; private set; } = null!;
    public static TempoRealService TempoReal { get; private set; } = null!;

    public App(AplicacaoService sistema)
    {
        InitializeComponent();
        Sistema = sistema;
        TempoReal = new TempoRealService(sistema, Helpers.ApiConfig.BaseUrl);
        sistema.SessaoAlterada += (_, _) =>
        {
            TempoReal.Sinalizar();
            if (!sistema.TemSessao) MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Current?.Windows.FirstOrDefault()?.Page is NavigationPage nav && nav.CurrentPage is not (frmLogin or frmInicial))
                    TrocarRaiz(new frmLogin());
            });
        };
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(CriarNavegacao(new frmInicial()))
        {
            Width = 440,
            Height = 850
        };
        window.Resumed += async (_, _) =>
        {
            await TempoReal.IniciarAsync();
            TempoReal.Sinalizar();
            if (window.Page is NavigationPage nav && nav.CurrentPage is PaginaBase pagina)
                await pagina.AtualizarAoRetomarAsync();
        };
        window.Activated += async (_, _) => await TempoReal.IniciarAsync();
        window.Stopped += async (_, _) => await TempoReal.PararAsync();
        window.Destroying += async (_, _) => await TempoReal.PararAsync();
        return window;
    }

    private static NavigationPage CriarNavegacao(Page pagina) => new(pagina)
    {
        BarBackgroundColor = Colors.White,
        BarTextColor = Color.FromArgb("#262626")
    };
    public static void TrocarRaiz(Page pagina)
    {
        if (Current?.Windows.FirstOrDefault() is Window window)
            window.Page = CriarNavegacao(pagina);
    }
}
