using Microsoft.Extensions.Logging;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Services;

namespace VanderCake;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        builder.Services.AddSingleton(new BancoLocal(Path.Combine(FileSystem.AppDataDirectory, "vandercake_v1.db3")));
        builder.Services.AddSingleton<ISessaoSegura, SecureStorageSessao>();
        builder.Services.AddSingleton<IApiService>(_ => new ApiService(new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl), Timeout = TimeSpan.FromSeconds(30) }));
        builder.Services.AddSingleton<AplicacaoService>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
