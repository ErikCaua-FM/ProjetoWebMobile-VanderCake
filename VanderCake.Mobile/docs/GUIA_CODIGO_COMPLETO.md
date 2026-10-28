# VanderCake.Mobile — código completo integrado

Código integral separado por arquivos e pastas, com indentação preservada. Use o ZIP completo VanderCake.Integracao e siga TUTORIAL_WINDOWS.md para configurar MySQL, API e endereço do aplicativo. Esta versão mantém Comprar novamente e inclui renovação de sessão, SignalR e recebimentos de parcelas.

App/MauiProgram inicia navegação e serviços; Views contém as telas XAML; AplicacaoService coordena os fluxos; ApiService se comunica com o servidor; TempoRealService recebe avisos e sincroniza; Data persiste o SQLite; Rules valida carrinhos e pagamentos. Models e Contracts são usados também nos testes reais de integração. Fontes TTF e demais recursos binários acompanham o ZIP e não podem ser copiados como texto.

## Arquivos

- `.gitignore`
- `VanderCake.slnx`
- `src/VanderCake.Mobile/App.xaml`
- `src/VanderCake.Mobile/App.xaml.cs`
- `src/VanderCake.Mobile/Contracts/ItemPedidoEnvio.cs`
- `src/VanderCake.Mobile/Contracts/LoginRequisicao.cs`
- `src/VanderCake.Mobile/Contracts/LoginResposta.cs`
- `src/VanderCake.Mobile/Contracts/NovoClienteRequisicao.cs`
- `src/VanderCake.Mobile/Contracts/ParcelaEnvio.cs`
- `src/VanderCake.Mobile/Contracts/PedidoEnvio.cs`
- `src/VanderCake.Mobile/Contracts/PedidoGeralEnvio.cs`
- `src/VanderCake.Mobile/Data/BancoLocal.cs`
- `src/VanderCake.Mobile/Data/CacheVendedorRegistro.cs`
- `src/VanderCake.Mobile/Data/CarrinhoRegistro.cs`
- `src/VanderCake.Mobile/Data/PedidoGeralRegistro.cs`
- `src/VanderCake.Mobile/Helpers/ApiConfig.cs`
- `src/VanderCake.Mobile/Helpers/ApiException.cs`
- `src/VanderCake.Mobile/Helpers/JsonDados.cs`
- `src/VanderCake.Mobile/MauiProgram.cs`
- `src/VanderCake.Mobile/Models/Categoria.cs`
- `src/VanderCake.Mobile/Models/Cidade.cs`
- `src/VanderCake.Mobile/Models/Cliente.cs`
- `src/VanderCake.Mobile/Models/CondicaoPagamento.cs`
- `src/VanderCake.Mobile/Models/DadosSincronizacao.cs`
- `src/VanderCake.Mobile/Models/Estado.cs`
- `src/VanderCake.Mobile/Models/FormaPagamento.cs`
- `src/VanderCake.Mobile/Models/ItemPedido.cs`
- `src/VanderCake.Mobile/Models/ParcelaPedido.cs`
- `src/VanderCake.Mobile/Models/Pedido.cs`
- `src/VanderCake.Mobile/Models/PedidoGeral.cs`
- `src/VanderCake.Mobile/Models/PedidoResumo.cs`
- `src/VanderCake.Mobile/Models/Produto.cs`
- `src/VanderCake.Mobile/Models/RegraParcela.cs`
- `src/VanderCake.Mobile/Models/SessaoSalva.cs`
- `src/VanderCake.Mobile/Models/Vendedor.cs`
- `src/VanderCake.Mobile/Platforms/Android/AndroidManifest.Debug.xml`
- `src/VanderCake.Mobile/Platforms/Android/AndroidManifest.xml`
- `src/VanderCake.Mobile/Platforms/Android/MainActivity.cs`
- `src/VanderCake.Mobile/Platforms/Android/MainApplication.cs`
- `src/VanderCake.Mobile/Platforms/Android/Resources/values/colors.xml`
- `src/VanderCake.Mobile/Platforms/MacCatalyst/AppDelegate.cs`
- `src/VanderCake.Mobile/Platforms/MacCatalyst/Entitlements.plist`
- `src/VanderCake.Mobile/Platforms/MacCatalyst/Info.plist`
- `src/VanderCake.Mobile/Platforms/MacCatalyst/Program.cs`
- `src/VanderCake.Mobile/Platforms/Windows/App.xaml`
- `src/VanderCake.Mobile/Platforms/Windows/App.xaml.cs`
- `src/VanderCake.Mobile/Platforms/iOS/AppDelegate.cs`
- `src/VanderCake.Mobile/Platforms/iOS/Info.plist`
- `src/VanderCake.Mobile/Platforms/iOS/Program.cs`
- `src/VanderCake.Mobile/Resources/AppIcon/appicon.svg`
- `src/VanderCake.Mobile/Resources/Splash/splash.svg`
- `src/VanderCake.Mobile/Resources/Styles/VanderCake.xaml`
- `src/VanderCake.Mobile/Rules/Dinheiro.cs`
- `src/VanderCake.Mobile/Rules/RegrasPedido.cs`
- `src/VanderCake.Mobile/Rules/StatusPedido.cs`
- `src/VanderCake.Mobile/Rules/ValidacaoCliente.cs`
- `src/VanderCake.Mobile/Services/ApiService.cs`
- `src/VanderCake.Mobile/Services/AplicacaoService.cs`
- `src/VanderCake.Mobile/Services/IApiService.cs`
- `src/VanderCake.Mobile/Services/ISessaoSegura.cs`
- `src/VanderCake.Mobile/Services/SecureStorageSessao.cs`
- `src/VanderCake.Mobile/Services/TempoRealService.cs`
- `src/VanderCake.Mobile/VanderCake.Mobile.csproj`
- `src/VanderCake.Mobile/Views/PaginaBase.cs`
- `src/VanderCake.Mobile/Views/frmCarrinho.xaml`
- `src/VanderCake.Mobile/Views/frmCarrinho.xaml.cs`
- `src/VanderCake.Mobile/Views/frmCatalogo.xaml`
- `src/VanderCake.Mobile/Views/frmCatalogo.xaml.cs`
- `src/VanderCake.Mobile/Views/frmClientes.xaml`
- `src/VanderCake.Mobile/Views/frmClientes.xaml.cs`
- `src/VanderCake.Mobile/Views/frmDetalhesPedido.xaml`
- `src/VanderCake.Mobile/Views/frmDetalhesPedido.xaml.cs`
- `src/VanderCake.Mobile/Views/frmInicial.xaml`
- `src/VanderCake.Mobile/Views/frmInicial.xaml.cs`
- `src/VanderCake.Mobile/Views/frmListaPedidos.xaml`
- `src/VanderCake.Mobile/Views/frmListaPedidos.xaml.cs`
- `src/VanderCake.Mobile/Views/frmLogin.xaml`
- `src/VanderCake.Mobile/Views/frmLogin.xaml.cs`
- `src/VanderCake.Mobile/Views/frmNovoCliente.xaml`
- `src/VanderCake.Mobile/Views/frmNovoCliente.xaml.cs`
- `src/VanderCake.Mobile/Views/frmPedidosCliente.xaml`
- `src/VanderCake.Mobile/Views/frmPedidosCliente.xaml.cs`
- `tests/VanderCake.Tests/Program.cs`
- `tests/VanderCake.Tests/RecompraTests.cs`
- `tests/VanderCake.Tests/VanderCake.Tests.csproj`

## .gitignore

````text
**/bin/
**/obj/
.vs/
.idea/
*.user
*.suo
*.db3
*.db3-shm
*.db3-wal
*.keystore
*.pfx
.env
artifacts/
````

## VanderCake.slnx

````xml
<?xml version='1.0' encoding='utf-8'?>
<Solution>
    <Folder Name="/src/">
        <Project Path="src/VanderCake.Mobile/VanderCake.Mobile.csproj"/>
    </Folder>
    <Folder Name="/tests/">
        <Project Path="tests/VanderCake.Tests/VanderCake.Tests.csproj"/>
    </Folder>
</Solution>
````

## src/VanderCake.Mobile/App.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" x:Class="VanderCake.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/VanderCake.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
````

## src/VanderCake.Mobile/App.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Contracts/ItemPedidoEnvio.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class ItemPedidoEnvio
{
    public int ProdutoId
    {
        get; set;
    }
    public int QuantidadeSolicitada
    {
        get; set;
    }
    public decimal PrecoUnitario
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Contracts/LoginRequisicao.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class LoginRequisicao
{
    public string Email { get; set; } = "";
    public string Senha { get; set; } = "";
}
````

## src/VanderCake.Mobile/Contracts/LoginResposta.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class LoginResposta
{
    public string AccessToken { get; set; } = "";
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public Vendedor Vendedor { get; set; } = new();
}
````

## src/VanderCake.Mobile/Contracts/NovoClienteRequisicao.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class NovoClienteRequisicao
{
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId
    {
        get; set;
    }
    public string Telefone { get; set; } = "";
    public string? Email
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Contracts/ParcelaEnvio.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class ParcelaEnvio
{
    public int NumeroParcela
    {
        get; set;
    }
    public int FormaPagamentoId
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Contracts/PedidoEnvio.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class PedidoEnvio
{
    public int ClienteId
    {
        get; set;
    }
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public int CondicaoPagamentoId
    {
        get; set;
    }
    public List<ItemPedidoEnvio> Itens { get; set; } = [];
    public List<ParcelaEnvio> Parcelas { get; set; } = [];
}
````

## src/VanderCake.Mobile/Contracts/PedidoGeralEnvio.cs

````csharp
using VanderCake.Models;

namespace VanderCake.Contracts;

public class PedidoGeralEnvio
{
    public string IdentificadorEnvio { get; set; } = "";
    public DateTimeOffset DataHoraCriacao
    {
        get; set;
    }
    public List<PedidoEnvio> Pedidos { get; set; } = [];
}
````

## src/VanderCake.Mobile/Data/BancoLocal.cs

````csharp
using SQLite;
using VanderCake.Helpers;
using VanderCake.Models;

namespace VanderCake.Data;
// O SQLite guarda documentos JSON em três tabelas locais. O banco central será relacional.
// Isso preserva decimais e permite salvar o lote inteiro atomicamente, sem gravar tokens.
public sealed class BancoLocal
{
    private readonly SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _inicializacao = new(1, 1);
    private bool _pronto;
    public BancoLocal(string caminho)
    {
        _db = new SQLiteAsyncConnection(caminho, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
    }

    public async Task InicializarAsync()
    {
        await _inicializacao.WaitAsync();
        try
        {
            if (_pronto)
                return;
            await _db.CreateTableAsync<CacheVendedorRegistro>();
            await _db.CreateTableAsync<PedidoGeralRegistro>();
            await _db.CreateTableAsync<CarrinhoRegistro>();
            _pronto = true;
        }
        finally
        {
            _inicializacao.Release();
        }
    }

    public async Task<DadosSincronizacao?> LerDadosAsync(int vendedorId)
    {
        var registro = await _db.FindAsync<CacheVendedorRegistro>(vendedorId);
        return registro is null ? null : JsonDados.Ler<DadosSincronizacao>(registro.ConteudoJson);
    }

    public Task SalvarDadosAsync(DadosSincronizacao dados) => _db.InsertOrReplaceAsync(new CacheVendedorRegistro { VendedorId = dados.Vendedor.Id, ConteudoJson = JsonDados.Escrever(dados) });
    public Task<List<PedidoGeralRegistro>> LerLotesAsync(int vendedorId) => _db.Table<PedidoGeralRegistro>().Where(p => p.VendedorId == vendedorId).ToListAsync();
    public async Task<PedidoGeralRegistro?> LerLoteAsync(string chave) => await _db.FindAsync<PedidoGeralRegistro>(chave);
    public Task SalvarLoteAsync(PedidoGeralRegistro registro) => _db.InsertOrReplaceAsync(registro);
    public Task<List<CarrinhoRegistro>> LerCarrinhosAsync(int vendedorId) => _db.Table<CarrinhoRegistro>().Where(c => c.VendedorId == vendedorId).ToListAsync();
    public async Task<CarrinhoRegistro?> LerCarrinhoAsync(int vendedorId, int clienteId) => await _db.FindAsync<CarrinhoRegistro>($"{vendedorId}:{clienteId}");
    public Task SalvarCarrinhoAsync(int vendedorId, Pedido pedido) => _db.InsertOrReplaceAsync(new CarrinhoRegistro { Chave = $"{vendedorId}:{pedido.ClienteId}", VendedorId = vendedorId, ClienteId = pedido.ClienteId, ConteudoJson = JsonDados.Escrever(pedido) });
    public Task ExcluirCarrinhoAsync(int vendedorId, int clienteId) => _db.DeleteAsync<CarrinhoRegistro>($"{vendedorId}:{clienteId}");
    public Task ConcluirCarrinhoAsync(PedidoGeralRegistro lote, int clienteId) => _db.RunInTransactionAsync(db =>
    {
        db.InsertOrReplace(lote);
        db.Delete<CarrinhoRegistro>($"{lote.VendedorId}:{clienteId}");
    });
    public Task SubstituirLoteRejeitadoAsync(string identificadorAnterior, PedidoGeralRegistro lote) => _db.RunInTransactionAsync(db =>
    {
        db.Delete<PedidoGeralRegistro>(identificadorAnterior);
        db.Insert(lote);
    });
    public Task AplicarSincronizacaoAsync(DadosSincronizacao dados) => _db.RunInTransactionAsync(db =>
    {
        // O histórico é mantido nos lotes, e não duplicado no snapshot do catálogo.
        var historico = dados.PedidosGerais;
        var cache = JsonDados.Copiar(dados);
        cache.PedidosGerais.Clear();
        db.InsertOrReplace(new CacheVendedorRegistro { VendedorId = dados.Vendedor.Id, ConteudoJson = JsonDados.Escrever(cache) });
        foreach (var lote in historico)
        {
            var anterior = db.Find<PedidoGeralRegistro>(lote.IdentificadorEnvio);
            if (anterior?.SituacaoLocal == "Montagem")
                throw new InvalidDataException("A API retornou um lote que ainda está em edição local.");
            db.InsertOrReplace(new PedidoGeralRegistro { IdentificadorEnvio = lote.IdentificadorEnvio, VendedorId = dados.Vendedor.Id, SituacaoLocal = "Sincronizado", ConteudoJson = JsonDados.Escrever(lote) });
        }
    });
    public Task FecharAsync() => _db.CloseAsync();
}
````

## src/VanderCake.Mobile/Data/CacheVendedorRegistro.cs

````csharp
using SQLite;

namespace VanderCake.Data;

public class CacheVendedorRegistro
{
    [PrimaryKey]
    public int VendedorId
    {
        get; set;
    }
    public string ConteudoJson { get; set; } = "";
}
````

## src/VanderCake.Mobile/Data/CarrinhoRegistro.cs

````csharp
using SQLite;

namespace VanderCake.Data;

public class CarrinhoRegistro
{
    [PrimaryKey]
    public string Chave { get; set; } = "";

    [Indexed]
    public int VendedorId
    {
        get; set;
    }
    public int ClienteId
    {
        get; set;
    }
    public string ConteudoJson { get; set; } = "";
}
````

## src/VanderCake.Mobile/Data/PedidoGeralRegistro.cs

````csharp
using SQLite;

namespace VanderCake.Data;

public class PedidoGeralRegistro
{
    [PrimaryKey]
    public string IdentificadorEnvio { get; set; } = "";

    [Indexed]
    public int VendedorId
    {
        get; set;
    }
    public string SituacaoLocal { get; set; } = "Montagem";
    public string ConteudoJson { get; set; } = "";
    public string? CorpoEnvioJson
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Helpers/ApiConfig.cs

````csharp
namespace VanderCake.Helpers;

public static class ApiConfig
{
    // Para aparelho físico, preencha, por exemplo: "http://192.168.1.10:5080/api/".
    // Deixe vazio para usar automaticamente o emulador Android ou o Windows local.
    private const string EnderecoComputadorNaRede = "";
    // No celular físico, troque pelo IP do computador na mesma rede.
    // A API deverá escutar a rede e liberar a porta 5080 no firewall.
    public static string BaseUrl
    {
        get
        {
#if DEBUG
            if (!string.IsNullOrWhiteSpace(EnderecoComputadorNaRede)) return EnderecoComputadorNaRede;
            return DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5080/api/"
                : "http://localhost:5080/api/";
#else
            // Substitua pelo domínio HTTPS real antes de distribuir.
            return "https://api.vandercake.example/api/";
#endif
        }
    }
}
````

## src/VanderCake.Mobile/Helpers/ApiException.cs

````csharp
namespace VanderCake.Helpers;

public sealed class ApiException : Exception
{
    public int Status
    {
        get;
    }
    public bool LoteNaoGravado
    {
        get;
    }

    public ApiException(int status, string mensagem, bool loteNaoGravado = false) : base(mensagem)
    {
        Status = status;
        LoteNaoGravado = loteNaoGravado;
    }
}
````

## src/VanderCake.Mobile/Helpers/JsonDados.cs

````csharp
using System.Text.Json;

namespace VanderCake.Helpers;

public static class JsonDados
{
    public static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web);
    public static string Escrever<T>(T valor) => JsonSerializer.Serialize(valor, Opcoes);
    public static T Ler<T>(string json) => JsonSerializer.Deserialize<T>(json, Opcoes) ?? throw new InvalidDataException("O registro está vazio ou é inválido.");
    public static T Copiar<T>(T valor) => Ler<T>(Escrever(valor));
}
````

## src/VanderCake.Mobile/MauiProgram.cs

````csharp
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
````

## src/VanderCake.Mobile/Models/Categoria.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Categoria
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
````

## src/VanderCake.Mobile/Models/Cidade.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Cidade
{
    public int Id
    {
        get; set;
    }
    public int EstadoId
    {
        get; set;
    }
    public string Nome { get; set; } = "";
}
````

## src/VanderCake.Mobile/Models/Cliente.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Cliente
{
    public int Id
    {
        get; set;
    }
    public int VendedorId
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Rua { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cep { get; set; } = "";
    public string Numero { get; set; } = "";
    public int CidadeId
    {
        get; set;
    }
    public string CidadeNome { get; set; } = "";
    public string Uf { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Email
    {
        get; set;
    }
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public string Endereco => $"{Rua}, {Numero} · {Bairro}";

    [JsonIgnore]
    public string Localidade => $"{CidadeNome}/{Uf} · CEP {Cep}";

    [JsonIgnore]
    public string Identificacao => $"CPF/CNPJ: {Documento}";

    [JsonIgnore]
    public string SituacaoTexto => Ativo ? "Consultar pedidos →" : "Cliente inativo · consultar histórico";
}
````

## src/VanderCake.Mobile/Models/CondicaoPagamento.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class CondicaoPagamento
{
    public int Id
    {
        get; set;
    }
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
    public List<RegraParcela> Parcelas { get; set; } = [];
}
````

## src/VanderCake.Mobile/Models/DadosSincronizacao.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class DadosSincronizacao
{
    public Vendedor Vendedor { get; set; } = new();
    public List<Cliente> Clientes { get; set; } = [];
    public List<Estado> Estados { get; set; } = [];
    public List<Cidade> Cidades { get; set; } = [];
    public List<Categoria> Categorias { get; set; } = [];
    public List<Produto> Produtos { get; set; } = [];
    public List<CondicaoPagamento> CondicoesPagamento { get; set; } = [];
    public List<FormaPagamento> FormasPagamento { get; set; } = [];
    public List<PedidoGeral> PedidosGerais { get; set; } = [];
    public DateTimeOffset? SincronizadoEm
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Models/Estado.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Estado
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string Uf { get; set; } = "";
}
````

## src/VanderCake.Mobile/Models/FormaPagamento.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class FormaPagamento
{
    public int Id
    {
        get; set;
    }
    public string Descricao { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
````

## src/VanderCake.Mobile/Models/ItemPedido.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class ItemPedido
{
    public int ProdutoId
    {
        get; set;
    }
    public string NomeProduto { get; set; } = "";
    public int QuantidadeSolicitada
    {
        get; set;
    }
    public int? QuantidadeAtendida
    {
        get; set;
    }
    public decimal PrecoUnitario
    {
        get; set;
    }

    [JsonIgnore]
    public decimal SubtotalSolicitado => PrecoUnitario * QuantidadeSolicitada;

    [JsonIgnore]
    public decimal? SubtotalAtendido => QuantidadeAtendida * PrecoUnitario;

    [JsonIgnore]
    public string QuantidadeTexto => $"{QuantidadeSolicitada} un.";

    [JsonIgnore]
    public string PrecoTexto => Dinheiro.Formatar(PrecoUnitario);

    [JsonIgnore]
    public string SubtotalTexto => Dinheiro.Formatar(SubtotalSolicitado);

    [JsonIgnore]
    public string SubtotalAtendidoTexto => SubtotalAtendido is decimal valor ? $"Subtotal atendido: {Dinheiro.Formatar(valor)}" : "";

    [JsonIgnore]
    public string AtendimentoTexto => QuantidadeAtendida is int q ? $"Solicitado: {QuantidadeSolicitada} · Atendido: {q} · Corte: {QuantidadeSolicitada - q}" : $"Solicitado: {QuantidadeSolicitada} · Atendimento ainda não definido";
}
````

## src/VanderCake.Mobile/Models/ParcelaPedido.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class ParcelaPedido
{
    public int NumeroParcela
    {
        get; set;
    }
    public int DiasAposConfirmacao
    {
        get; set;
    }
    public int FormaPagamentoId
    {
        get; set;
    }
    public string FormaPagamentoNome { get; set; } = "";
    public decimal ValorPrevisto
    {
        get; set;
    }
    public decimal? ValorFinal
    {
        get; set;
    }
    public DateTime? DataVencimento
    {
        get; set;
    }
    public DateTime? DataPagamento { get; set; }
    public string Situacao { get; set; } = "Prevista";

    [JsonIgnore]
    public string PrazoTexto => DiasAposConfirmacao == 0 ? "À vista" : $"{DiasAposConfirmacao} dias";

    [JsonIgnore]
    public string Descricao => $"{NumeroParcela}ª parcela · {PrazoTexto}";

    [JsonIgnore]
    public string ValorTexto => ValorFinal is decimal valor ? Dinheiro.Formatar(valor) : $"Previsto: {Dinheiro.Formatar(ValorPrevisto)}";

    [JsonIgnore]
    public string DetalheTexto => Situacao == "Paga" && DataPagamento is DateTime pagamento
        ? $"{FormaPagamentoNome} · Recebida em {pagamento:dd/MM/yyyy}"
        : DataVencimento is DateTime data ? $"{FormaPagamentoNome} · Vence em {data:dd/MM/yyyy} · {Situacao}" : $"{FormaPagamentoNome} · Prazo contado da confirmação da venda";
}
````

## src/VanderCake.Mobile/Models/Pedido.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Pedido
{
    public int Id
    {
        get; set;
    }
    public int ClienteId
    {
        get; set;
    }
    public string ClienteNome { get; set; } = "";
    public DateTimeOffset DataHoraCriacao { get; set; } = DateTimeOffset.UtcNow;
    public int CondicaoPagamentoId
    {
        get; set;
    }
    public string CondicaoPagamentoDescricao { get; set; } = "";
    public string Status { get; set; } = "Rascunho";
    public decimal ValorTotalSolicitado
    {
        get; set;
    }
    public decimal? ValorTotalAtendido
    {
        get; set;
    }
    public List<ItemPedido> Itens { get; set; } = [];
    public List<ParcelaPedido> Parcelas { get; set; } = [];

    [JsonIgnore]
    public string Titulo => Id > 0 ? $"Pedido #{Id}" : "Pedido local";

    [JsonIgnore]
    public string StatusTexto => StatusPedido.Descrever(Status);

    [JsonIgnore]
    public string DataTexto => DataHoraCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    [JsonIgnore]
    public string TotalTexto => Dinheiro.Formatar(ValorTotalAtendido ?? ValorTotalSolicitado);

    [JsonIgnore]
    public string ResumoItens => string.Join(" · ", Itens.Select(i => $"{i.QuantidadeSolicitada}x {i.NomeProduto}"));
}
````

## src/VanderCake.Mobile/Models/PedidoGeral.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class PedidoGeral
{
    public int Id
    {
        get; set;
    }
    public string IdentificadorEnvio { get; set; } = Guid.NewGuid().ToString();
    public int VendedorId
    {
        get; set;
    }
    public DateTimeOffset DataHoraCriacao { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DataHoraRecebimento
    {
        get; set;
    }
    public string Status { get; set; } = "Rascunho";
    public string? MotivoCancelamento
    {
        get; set;
    }
    public List<Pedido> Pedidos { get; set; } = [];

    [JsonIgnore]
    public decimal TotalSolicitado => Pedidos.Sum(p => p.ValorTotalSolicitado);
}
````

## src/VanderCake.Mobile/Models/PedidoResumo.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class PedidoResumo
{
    public string IdentificadorEnvio { get; set; } = "";
    public Pedido Pedido { get; set; } = new();
    public string SituacaoLocal { get; set; } = "";
    public string? MotivoCancelamento
    {
        get; set;
    }

    [JsonIgnore]
    public string SituacaoTexto => SituacaoLocal == "EnvioIncerto" ? "Envio a verificar" : Pedido.StatusTexto;

    [JsonIgnore]
    public bool PodeEditar => SituacaoLocal == "Montagem";
}
````

## src/VanderCake.Mobile/Models/Produto.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Produto
{
    public int Id
    {
        get; set;
    }
    public int CategoriaId
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string? Descricao
    {
        get; set;
    }
    public decimal Preco
    {
        get; set;
    }
    public int Estoque
    {
        get; set;
    }
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public string PrecoTexto => Dinheiro.Formatar(Preco);

    [JsonIgnore]
    public string EstoqueTexto => $"Estoque informado: {Estoque}";
}
````

## src/VanderCake.Mobile/Models/RegraParcela.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class RegraParcela
{
    public int NumeroParcela
    {
        get; set;
    }
    public int DiasAposConfirmacao
    {
        get; set;
    }
}
````

## src/VanderCake.Mobile/Models/SessaoSalva.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class SessaoSalva
{
    public string AccessToken { get; set; } = "";
    public string? RefreshToken { get; set; }
    public DateTimeOffset ExpiraEm { get; set; } = DateTimeOffset.MaxValue;
    public Vendedor Vendedor { get; set; } = new();
}
````

## src/VanderCake.Mobile/Models/Vendedor.cs

````csharp
using System.Text.Json.Serialization;
using VanderCake.Rules;

namespace VanderCake.Models;

public class Vendedor
{
    public int Id
    {
        get; set;
    }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public bool Ativo { get; set; } = true;
}
````

## src/VanderCake.Mobile/Platforms/Android/AndroidManifest.Debug.xml

````xml
<?xml version='1.0' encoding='utf-8'?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="false" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true" android:usesCleartextTraffic="true"/>
    <uses-permission android:name="android.permission.INTERNET"/>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
</manifest>
````

## src/VanderCake.Mobile/Platforms/Android/AndroidManifest.xml

````xml
<?xml version='1.0' encoding='utf-8'?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="false" android:icon="@mipmap/appicon" android:roundIcon="@mipmap/appicon_round" android:supportsRtl="true" android:usesCleartextTraffic="false"/>
    <uses-permission android:name="android.permission.INTERNET"/>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
</manifest>
````

## src/VanderCake.Mobile/Platforms/Android/MainActivity.cs

````csharp
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace VanderCake;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
````

## src/VanderCake.Mobile/Platforms/Android/MainApplication.cs

````csharp
using Android.App;
using Android.Runtime;

namespace VanderCake;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
````

## src/VanderCake.Mobile/Platforms/Android/Resources/values/colors.xml

````xml
<?xml version='1.0' encoding='utf-8'?>
<resources>
    <color name="colorPrimary">#EA1D2C</color>
    <color name="colorPrimaryDark">#BD1521</color>
    <color name="colorAccent">#EA1D2C</color>
</resources>
````

## src/VanderCake.Mobile/Platforms/MacCatalyst/AppDelegate.cs

````csharp
using Foundation;

namespace VanderCake;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
````

## src/VanderCake.Mobile/Platforms/MacCatalyst/Entitlements.plist

````xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <!-- See https://aka.ms/maui-publish-app-store#add-entitlements for more information about adding entitlements.-->
    <dict>
        <!-- App Sandbox must be enabled to distribute a MacCatalyst app through the Mac App Store. -->
        <key>com.apple.security.app-sandbox</key>
        <true/>
        <!-- When App Sandbox is enabled, this value is required to open outgoing network connections. -->
        <key>com.apple.security.network.client</key>
        <true/>
    </dict>
</plist>
````

## src/VanderCake.Mobile/Platforms/MacCatalyst/Info.plist

````xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>UIDeviceFamily</key>
	<array>
		<integer>2</integer>
	</array>
	<key>LSApplicationCategoryType</key>
	<string>public.app-category.lifestyle</string>
	<key>UIRequiredDeviceCapabilities</key>
	<array>
		<string>arm64</string>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>UISupportedInterfaceOrientations~ipad</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>XSAppIconAssets</key>
	<string>Assets.xcassets/appicon.appiconset</string>
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsLocalNetworking</key>
		<true/>
	</dict>
</dict>
</plist>
````

## src/VanderCake.Mobile/Platforms/MacCatalyst/Program.cs

````csharp
using ObjCRuntime;
using UIKit;

namespace VanderCake;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
````

## src/VanderCake.Mobile/Platforms/Windows/App.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<maui:MauiWinUIApplication xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:maui="using:Microsoft.Maui" xmlns:local="using:VanderCake.WinUI" x:Class="VanderCake.WinUI.App">

</maui:MauiWinUIApplication>
````

## src/VanderCake.Mobile/Platforms/Windows/App.xaml.cs

````csharp
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace VanderCake.WinUI;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
````

## src/VanderCake.Mobile/Platforms/iOS/AppDelegate.cs

````csharp
using Foundation;

namespace VanderCake;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
````

## src/VanderCake.Mobile/Platforms/iOS/Info.plist

````xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
	<key>LSRequiresIPhoneOS</key>
	<true/>
	<key>UIDeviceFamily</key>
	<array>
		<integer>1</integer>
		<integer>2</integer>
	</array>
	<key>UIRequiredDeviceCapabilities</key>
	<array>
		<string>arm64</string>
	</array>
	<key>UISupportedInterfaceOrientations</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>UISupportedInterfaceOrientations~ipad</key>
	<array>
		<string>UIInterfaceOrientationPortrait</string>
		<string>UIInterfaceOrientationPortraitUpsideDown</string>
		<string>UIInterfaceOrientationLandscapeLeft</string>
		<string>UIInterfaceOrientationLandscapeRight</string>
	</array>
	<key>XSAppIconAssets</key>
	<string>Assets.xcassets/appicon.appiconset</string>
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsLocalNetworking</key>
		<true/>
	</dict>
</dict>
</plist>
````

## src/VanderCake.Mobile/Platforms/iOS/Program.cs

````csharp
using ObjCRuntime;
using UIKit;

namespace VanderCake;

public class Program
{
    // This is the main entry point of the application.
    static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
````

## src/VanderCake.Mobile/Resources/AppIcon/appicon.svg

````xml
<svg xmlns="http://www.w3.org/2000/svg" width="128" height="128" viewBox="0 0 128 128"><rect width="128" height="128" rx="28" fill="#EA1D2C"/><path d="M28 61h72v37H28z" fill="white"/><path d="M28 61c-3-15 9-26 21-19 4-16 27-16 31 0 13-7 25 4 20 19" fill="none" stroke="white" stroke-width="9" stroke-linejoin="round"/><path d="M43 75v10M64 75v10M85 75v10" stroke="#EA1D2C" stroke-width="6" stroke-linecap="round"/></svg>
````

## src/VanderCake.Mobile/Resources/Splash/splash.svg

````xml
<svg xmlns="http://www.w3.org/2000/svg" width="128" height="128" viewBox="0 0 128 128"><rect width="128" height="128" rx="28" fill="#EA1D2C"/><path d="M28 61h72v37H28z" fill="white"/><path d="M28 61c-3-15 9-26 21-19 4-16 27-16 31 0 13-7 25 4 20 19" fill="none" stroke="white" stroke-width="9" stroke-linejoin="round"/><path d="M43 75v10M64 75v10M85 75v10" stroke="#EA1D2C" stroke-width="6" stroke-linecap="round"/></svg>
````

## src/VanderCake.Mobile/Resources/Styles/VanderCake.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Color x:Key="Primary">#EA1D2C</Color>
    <Color x:Key="Ink">#262626</Color>
    <Color x:Key="Muted">#686868</Color>
    <Color x:Key="Canvas">#F7F7F7</Color>
    <Style TargetType="ContentPage" ApplyToDerivedTypes="True">
        <Setter Property="BackgroundColor" Value="{StaticResource Canvas}"/>
    </Style>
    <Style TargetType="Label">
        <Setter Property="FontFamily" Value="OpenSansRegular"/>
        <Setter Property="TextColor" Value="{StaticResource Ink}"/>
        <Setter Property="FontSize" Value="14"/>
    </Style>
    <Style x:Key="Heading" TargetType="Label">
        <Setter Property="FontFamily" Value="OpenSansSemibold"/>
        <Setter Property="FontSize" Value="25"/>
        <Setter Property="TextColor" Value="{StaticResource Ink}"/>
    </Style>
    <Style x:Key="MutedLabel" TargetType="Label">
        <Setter Property="FontSize" Value="12"/>
        <Setter Property="TextColor" Value="{StaticResource Muted}"/>
    </Style>
    <Style x:Key="Card" TargetType="Border">
        <Setter Property="BackgroundColor" Value="White"/>
        <Setter Property="Stroke" Value="#E8E8E8"/>
        <Setter Property="StrokeShape" Value="RoundRectangle 14"/>
        <Setter Property="Padding" Value="16"/>
        <Setter Property="Margin" Value="0,0,0,10"/>
    </Style>
    <Style TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource Primary}"/>
        <Setter Property="TextColor" Value="White"/>
        <Setter Property="CornerRadius" Value="10"/>
        <Setter Property="FontFamily" Value="OpenSansSemibold"/>
        <Setter Property="FontSize" Value="14"/>
        <Setter Property="MinimumHeightRequest" Value="46"/>
        <Setter Property="Padding" Value="12,10"/>
    </Style>
    <Style x:Key="Secondary" TargetType="Button">
        <Setter Property="BackgroundColor" Value="White"/>
        <Setter Property="TextColor" Value="{StaticResource Primary}"/>
        <Setter Property="BorderColor" Value="#E8E8E8"/>
        <Setter Property="BorderWidth" Value="1"/>
        <Setter Property="CornerRadius" Value="10"/>
        <Setter Property="MinimumHeightRequest" Value="44"/>
    </Style>
    <Style TargetType="Entry">
        <Setter Property="TextColor" Value="{StaticResource Ink}"/>
        <Setter Property="PlaceholderColor" Value="{StaticResource Muted}"/>
        <Setter Property="FontSize" Value="15"/>
        <Setter Property="MinimumHeightRequest" Value="48"/>
    </Style>
    <Style TargetType="Picker">
        <Setter Property="TextColor" Value="{StaticResource Ink}"/>
        <Setter Property="TitleColor" Value="{StaticResource Muted}"/>
        <Setter Property="MinimumHeightRequest" Value="48"/>
    </Style>
</ResourceDictionary>
````

## src/VanderCake.Mobile/Rules/Dinheiro.cs

````csharp
using System.Globalization;

namespace VanderCake.Rules;

public static class Dinheiro
{
    public const decimal Limite = 9_999_999_999.99m;
    public static string Formatar(decimal valor) => valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
    public static List<decimal> Dividir(decimal total, int quantidade)
    {
        if (total < 0 || total > Limite || decimal.Round(total, 2) != total || quantidade < 1)
            throw new InvalidOperationException("Valor ou quantidade de parcelas inválidos.");
        decimal baseCentavos = decimal.Truncate(total * 100 / quantidade);
        var parcelas = Enumerable.Repeat(baseCentavos / 100, quantidade).ToList();
        parcelas[^1] += total - parcelas.Sum();
        return parcelas;
    }
}
````

## src/VanderCake.Mobile/Rules/RegrasPedido.cs

````csharp
using VanderCake.Contracts;
using VanderCake.Models;

namespace VanderCake.Rules;

public static class RegrasPedido
{
    public static void Recalcular(Pedido pedido)
    {
        pedido.ValorTotalSolicitado = pedido.Itens.Sum(i => i.SubtotalSolicitado);
        if (pedido.ValorTotalSolicitado > Dinheiro.Limite)
            throw new InvalidOperationException("O valor do pedido ultrapassa o limite permitido.");
        if (pedido.Parcelas.Count == 0)
            return;
        var valores = Dinheiro.Dividir(pedido.ValorTotalSolicitado, pedido.Parcelas.Count);
        for (int i = 0; i < pedido.Parcelas.Count; i++)
            pedido.Parcelas[i].ValorPrevisto = valores[i];
    }

    public static void AplicarCondicao(Pedido pedido, CondicaoPagamento condicao)
    {
        var regras = condicao.Parcelas.OrderBy(p => p.NumeroParcela).ToList();
        int[] prazos = regras.Select(p => p.DiasAposConfirmacao).ToArray();
        if (!condicao.Ativo || !(prazos.SequenceEqual(new[] { 0 }) || prazos.SequenceEqual(new[] { 7, 14 }) || prazos.SequenceEqual(new[] { 7, 14, 21 })) || !regras.Select(p => p.NumeroParcela).SequenceEqual(Enumerable.Range(1, regras.Count)))
            throw new InvalidOperationException("A condição recebida da API não possui os prazos previstos.");
        var anteriores = pedido.Parcelas.ToDictionary(p => p.NumeroParcela);
        pedido.CondicaoPagamentoId = condicao.Id;
        pedido.CondicaoPagamentoDescricao = condicao.Descricao;
        pedido.Parcelas = regras.Select(r => new ParcelaPedido { NumeroParcela = r.NumeroParcela, DiasAposConfirmacao = r.DiasAposConfirmacao, FormaPagamentoId = anteriores.GetValueOrDefault(r.NumeroParcela)?.FormaPagamentoId ?? 0, FormaPagamentoNome = anteriores.GetValueOrDefault(r.NumeroParcela)?.FormaPagamentoNome ?? "" }).ToList();
        Recalcular(pedido);
    }

    public static void Validar(Pedido pedido, DadosSincronizacao dados)
    {
        if (!dados.Clientes.Any(c => c.Id == pedido.ClienteId && c.Ativo && c.VendedorId == dados.Vendedor.Id))
            throw new InvalidOperationException("O cliente não está ativo na sua carteira.");
        if (pedido.Itens.Count == 0)
            throw new InvalidOperationException("Adicione pelo menos um produto.");
        if (pedido.Itens.Select(i => i.ProdutoId).Distinct().Count() != pedido.Itens.Count)
            throw new InvalidOperationException("O pedido contém produtos duplicados.");
        foreach (var item in pedido.Itens)
        {
            var produto = dados.Produtos.FirstOrDefault(p => p.Id == item.ProdutoId && p.Ativo);
            if (produto is null)
                throw new InvalidOperationException($"O produto {item.NomeProduto} não está disponível. Remova-o do carrinho.");
            if (item.QuantidadeSolicitada < 1 || item.PrecoUnitario <= 0 || decimal.Round(item.PrecoUnitario, 2) != item.PrecoUnitario)
                throw new InvalidOperationException("Revise as quantidades e os preços do pedido.");
            if (item.PrecoUnitario != produto.Preco)
                throw new InvalidOperationException($"O preço de {item.NomeProduto} mudou. Abra o carrinho e use Atualizar preços.");
        }

        var condicao = dados.CondicoesPagamento.FirstOrDefault(c => c.Id == pedido.CondicaoPagamentoId && c.Ativo) ?? throw new InvalidOperationException("Selecione a condição de pagamento.");
        var regras = condicao.Parcelas.OrderBy(p => p.NumeroParcela).ToList();
        if (regras.Count != pedido.Parcelas.Count)
            throw new InvalidOperationException("Revise as parcelas do pedido.");
        for (int i = 0; i < regras.Count; i++)
        {
            var parcela = pedido.Parcelas[i];
            if (parcela.NumeroParcela != regras[i].NumeroParcela || parcela.DiasAposConfirmacao != regras[i].DiasAposConfirmacao || !dados.FormasPagamento.Any(f => f.Id == parcela.FormaPagamentoId && f.Ativo))
                throw new InvalidOperationException($"Escolha uma forma de pagamento válida para a {i + 1}ª parcela.");
        }

        Recalcular(pedido);
    }

    public static PedidoGeralEnvio CriarEnvio(PedidoGeral lote) => new()
    {
        IdentificadorEnvio = lote.IdentificadorEnvio,
        DataHoraCriacao = lote.DataHoraCriacao,
        Pedidos = lote.Pedidos.Select(p => new PedidoEnvio { ClienteId = p.ClienteId, DataHoraCriacao = p.DataHoraCriacao, CondicaoPagamentoId = p.CondicaoPagamentoId, Itens = p.Itens.Select(i => new ItemPedidoEnvio { ProdutoId = i.ProdutoId, QuantidadeSolicitada = i.QuantidadeSolicitada, PrecoUnitario = i.PrecoUnitario }).ToList(), Parcelas = p.Parcelas.Select(f => new ParcelaEnvio { NumeroParcela = f.NumeroParcela, FormaPagamentoId = f.FormaPagamentoId }).ToList() }).ToList()
    };
}
````

## src/VanderCake.Mobile/Rules/StatusPedido.cs

````csharp
namespace VanderCake.Rules;

public static class StatusPedido
{
    public static string Descrever(string status) => status switch
    {
        "Rascunho" => "Na lista · ainda não enviado",
        "Pendente" => "Aguardando a loja",
        "Confirmado" => "Confirmado",
        "ConfirmadoComCorte" => "Confirmado com corte",
        "NaoAtendido" => "Não atendido",
        "Cancelado" => "Cancelado",
        _ => status
    };
}
````

## src/VanderCake.Mobile/Rules/ValidacaoCliente.cs

````csharp
using System.Net.Mail;
using VanderCake.Contracts;

namespace VanderCake.Rules;

public static class ValidacaoCliente
{
    public static string Digitos(string texto) => new(texto.Where(char.IsDigit).ToArray());
    public static void Validar(NovoClienteRequisicao cliente)
    {
        cliente.Nome = cliente.Nome.Trim();
        cliente.Documento = Digitos(cliente.Documento);
        cliente.Cep = Digitos(cliente.Cep);
        cliente.Telefone = Digitos(cliente.Telefone);
        cliente.Email = string.IsNullOrWhiteSpace(cliente.Email) ? null : cliente.Email.Trim();
        if (cliente.Nome.Length is < 2 or > 100 || cliente.Documento.Length is not (11 or 14))
            throw new InvalidOperationException("Informe nome/razão social e CPF (11 dígitos) ou CNPJ (14 dígitos).");
        if (string.IsNullOrWhiteSpace(cliente.Rua) || cliente.Rua.Length > 100 || string.IsNullOrWhiteSpace(cliente.Bairro) || cliente.Bairro.Length > 60 || string.IsNullOrWhiteSpace(cliente.Numero) || cliente.Numero.Length > 10 || cliente.Cep.Length != 8 || cliente.CidadeId <= 0)
            throw new InvalidOperationException("Preencha rua, número, bairro, CEP e cidade corretamente.");
        if (cliente.Telefone.Length is < 10 or > 13)
            throw new InvalidOperationException("Informe o telefone com DDD.");
        if (cliente.Email is not null && (cliente.Email.Length > 150 || !MailAddress.TryCreate(cliente.Email, out _)))
            throw new InvalidOperationException("Informe um e-mail válido ou deixe o campo vazio.");
    }
}
````

## src/VanderCake.Mobile/Services/ApiService.cs

````csharp
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using VanderCake.Contracts;
using VanderCake.Helpers;
using VanderCake.Models;

namespace VanderCake.Services;

public sealed class ApiService : IApiService
{
    private readonly HttpClient _http;
    public string? Token
    {
        get; set;
    }

    public ApiService(HttpClient http) => _http = http;
    public Task<LoginResposta> EntrarAsync(LoginRequisicao login) => EnviarAsync<LoginResposta>(HttpMethod.Post, "auth/vendedores/login", JsonDados.Escrever(login), false);
    public Task<LoginResposta> RenovarAsync(string refreshToken) => EnviarAsync<LoginResposta>(HttpMethod.Post, "auth/renovar", JsonDados.Escrever(new { refreshToken }), false);
    public Task<DadosSincronizacao> SincronizarAsync() => EnviarAsync<DadosSincronizacao>(HttpMethod.Get, "mobile/sincronizacao");
    public Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao cliente) => EnviarAsync<Cliente>(HttpMethod.Post, "clientes", JsonDados.Escrever(cliente));
    public Task<PedidoGeral> EnviarLoteAsync(string json) => EnviarAsync<PedidoGeral>(HttpMethod.Post, "pedidos-gerais", json);
    public async Task<PedidoGeral?> ConsultarLoteAsync(string identificador)
    {
        try
        {
            return await EnviarAsync<PedidoGeral>(HttpMethod.Get, $"pedidos-gerais/por-identificador/{Uri.EscapeDataString(identificador)}");
        }
        catch (ApiException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    private async Task<T> EnviarAsync<T>(HttpMethod metodo, string caminho, string? json = null, bool autenticado = true)
    {
        using var requisicao = new HttpRequestMessage(metodo, caminho);
        if (autenticado)
        {
            if (string.IsNullOrWhiteSpace(Token))
                throw new ApiException(401, "Faça login novamente.");
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        if (json is not null)
            requisicao.Content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
            using var resposta = await _http.SendAsync(requisicao);
            var texto = await resposta.Content.ReadAsStringAsync();
            if (!resposta.IsSuccessStatusCode)
            {
                string mensagem = $"Não foi possível concluir a operação (HTTP {(int)resposta.StatusCode}).";
                bool naoGravado = false;
                try
                {
                    using var doc = JsonDocument.Parse(texto);
                    var raiz = doc.RootElement;
                    if (raiz.TryGetProperty("mensagem", out var m) && m.ValueKind == JsonValueKind.String)
                        mensagem = m.GetString() ?? mensagem;
                    else if (raiz.TryGetProperty("title", out var titulo) && titulo.ValueKind == JsonValueKind.String)
                        mensagem = titulo.GetString() ?? mensagem;
                    if (raiz.TryGetProperty("loteNaoGravado", out var n))
                        naoGravado = n.ValueKind == JsonValueKind.True;
                    if (raiz.TryGetProperty("errors", out var erros) && erros.ValueKind == JsonValueKind.Object)
                        mensagem += " " + string.Join(" ", erros.EnumerateObject().Where(e => e.Value.ValueKind == JsonValueKind.Array).SelectMany(e => e.Value.EnumerateArray()).Select(e => e.ToString()));
                }
                catch (JsonException)
                {
                }

                if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                    mensagem = "Sua sessão expirou ou as credenciais são inválidas. Entre novamente.";
                throw new ApiException((int)resposta.StatusCode, mensagem, naoGravado);
            }

            return JsonDados.Ler<T>(texto);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new ApiException(0, "Não foi possível acessar a API. Confira sua conexão e o endereço configurado. Seus dados locais foram preservados.");
        }
        catch (Exception ex) when (ex is JsonException or InvalidDataException)
        {
            throw new ApiException(0, "A resposta da API está incompleta ou em formato incompatível. Os dados locais foram preservados.");
        }
    }
}
````

## src/VanderCake.Mobile/Services/AplicacaoService.cs

````csharp
using VanderCake.Contracts;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;

namespace VanderCake.Services;
// Uma única fila de operações evita que sincronização, edição e envio se sobreponham.
public sealed class AplicacaoService
{
    private readonly BancoLocal _banco;
    private readonly IApiService _api;
    private readonly ISessaoSegura _seguro;
    private readonly SemaphoreSlim _operacao = new(1, 1);
    private string? _refreshToken;
    private DateTimeOffset _expiraEm = DateTimeOffset.MaxValue;
    public event EventHandler? SessaoAlterada;
    public Vendedor? VendedorAtual
    {
        get; private set;
    }
    public bool TemSessao => VendedorAtual is not null;

    public AplicacaoService(BancoLocal banco, IApiService api, ISessaoSegura seguro)
    {
        _banco = banco;
        _api = api;
        _seguro = seguro;
    }

    private int VendedorId => VendedorAtual?.Id ?? throw new ApiException(401, "Entre na sua conta.");

    private async Task<T> ExecutarAsync<T>(Func<Task<T>> acao)
    {
        await _operacao.WaitAsync();
        try
        {
            try { return await acao(); }
            catch (ApiException ex) when (ex.Status == 401 && TemSessao && !string.IsNullOrEmpty(_refreshToken))
            {
                await RenovarInternoAsync();
                return await acao();
            }
        }
        catch (ApiException ex) when (ex.Status == 401)
        {
            VendedorAtual = null;
            _api.Token = null;
            _refreshToken = null;
            await _seguro.LimparAsync();
            SessaoAlterada?.Invoke(this, EventArgs.Empty);
            throw;
        }
        finally
        {
            _operacao.Release();
        }
    }

    public Task InicializarAsync() => ExecutarAsync(async () =>
    {
        await _banco.InicializarAsync();
        string? json = await _seguro.LerAsync();
        if (json is not null)
        {
            try
            {
                var sessao = JsonDados.Ler<SessaoSalva>(json);
                if (sessao.Vendedor.Id <= 0 || !sessao.Vendedor.Ativo || string.IsNullOrWhiteSpace(sessao.AccessToken))
                    throw new InvalidDataException("Sessão inválida.");
                VendedorAtual = sessao.Vendedor;
                _api.Token = sessao.AccessToken;
                _refreshToken = sessao.RefreshToken;
                _expiraEm = sessao.ExpiraEm;
                SessaoAlterada?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex) when (ex is System.Text.Json.JsonException or InvalidDataException)
            {
                await _seguro.LimparAsync();
            }
        }

        return true;
    });
    public Task EntrarAsync(string email, string senha) => ExecutarAsync(async () =>
    {
        if (!System.Net.Mail.MailAddress.TryCreate(email.Trim(), out _) || string.IsNullOrWhiteSpace(senha))
            throw new InvalidOperationException("Informe um e-mail válido e sua senha.");
        var resposta = await _api.EntrarAsync(new LoginRequisicao { Email = email.Trim(), Senha = senha });
        if (resposta.Vendedor.Id <= 0 || !resposta.Vendedor.Ativo || string.IsNullOrWhiteSpace(resposta.AccessToken))
            throw new ApiException(0, "A API não retornou uma sessão de vendedor válida.");
        var dados = await _banco.LerDadosAsync(resposta.Vendedor.Id) ?? new DadosSincronizacao();
        dados.Vendedor = resposta.Vendedor;
        await _banco.SalvarDadosAsync(dados);
        await GuardarSessaoAsync(resposta);
        SessaoAlterada?.Invoke(this, EventArgs.Empty);
        return true;
    });
    public Task SairAsync() => ExecutarAsync(async () =>
    {
        await _seguro.LimparAsync();
        VendedorAtual = null;
        _api.Token = null;
        _refreshToken = null;
        SessaoAlterada?.Invoke(this, EventArgs.Empty);
        // Não apagamos rascunhos não enviados. Cada consulta é filtrada pelo vendedor.
        return true;
    });
    public Task<DadosSincronizacao> ObterDadosAsync() => ExecutarAsync(LerDadosInternoAsync);
    public Task<string?> ObterTokenAsync() => ExecutarAsync(async () =>
    {
        if (TemSessao && _expiraEm <= DateTimeOffset.UtcNow.AddMinutes(1) && !string.IsNullOrEmpty(_refreshToken))
            await RenovarInternoAsync();
        return _api.Token;
    });
    private async Task RenovarInternoAsync()
    {
        var resposta = await _api.RenovarAsync(_refreshToken!);
        if (resposta.Vendedor.Id != VendedorId || !resposta.Vendedor.Ativo || string.IsNullOrWhiteSpace(resposta.AccessToken))
            throw new ApiException(401, "Sessão inválida. Entre novamente.");
        await GuardarSessaoAsync(resposta);
    }
    private async Task GuardarSessaoAsync(LoginResposta resposta)
    {
        var sessao = new SessaoSalva
        {
            AccessToken = resposta.AccessToken, RefreshToken = resposta.RefreshToken, Vendedor = resposta.Vendedor,
            ExpiraEm = resposta.ExpiresIn > 0 ? DateTimeOffset.UtcNow.AddSeconds(resposta.ExpiresIn) : DateTimeOffset.MaxValue
        };
        await _seguro.SalvarAsync(JsonDados.Escrever(sessao));
        VendedorAtual = sessao.Vendedor;
        _api.Token = sessao.AccessToken;
        _refreshToken = sessao.RefreshToken;
        _expiraEm = sessao.ExpiraEm;
    }
    private async Task<DadosSincronizacao> LerDadosInternoAsync() => await _banco.LerDadosAsync(VendedorId) ?? new DadosSincronizacao
    {
        Vendedor = VendedorAtual!
    };
    public Task AtualizarAsync() => ExecutarAsync(async () =>
    {
        int vendedorId = VendedorId;
        var dados = await _api.SincronizarAsync();
        if (dados.Vendedor.Id != vendedorId || !dados.Vendedor.Ativo)
            throw new ApiException(401, "A sessão não corresponde a um vendedor ativo.");
        if (dados.Clientes.Any(c => c.VendedorId != vendedorId))
            throw new ApiException(0, "A API retornou clientes de outra carteira.");
        foreach (var lote in dados.PedidosGerais)
        {
            ValidarLoteServidor(lote, vendedorId);
            var local = await _banco.LerLoteAsync(lote.IdentificadorEnvio);
            if (local?.SituacaoLocal == "EnvioIncerto")
                ValidarCorrespondencia(lote, JsonDados.Ler<PedidoGeral>(local.ConteudoJson));
        }

        dados.SincronizadoEm = DateTimeOffset.UtcNow;
        await _banco.AplicarSincronizacaoAsync(dados);
        VendedorAtual = dados.Vendedor;
        return true;
    });
    public Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao novo) => ExecutarAsync(async () =>
    {
        ValidacaoCliente.Validar(novo);
        var dados = await LerDadosInternoAsync();
        if (!dados.Cidades.Any(c => c.Id == novo.CidadeId))
            throw new InvalidOperationException("Selecione uma cidade sincronizada.");
        if (dados.Clientes.Any(c => ValidacaoCliente.Digitos(c.Documento) == novo.Documento))
            throw new InvalidOperationException("Este CPF/CNPJ já está na sua carteira.");
        var cliente = await _api.CadastrarClienteAsync(novo);
        if (cliente.Id <= 0 || cliente.VendedorId != VendedorId)
            throw new ApiException(0, "A API retornou um cadastro de cliente incompatível. Atualize a carteira antes de tentar novamente.");
        dados.Clientes.RemoveAll(c => c.Id == cliente.Id);
        dados.Clientes.Add(cliente);
        await _banco.SalvarDadosAsync(dados);
        return cliente;
    });
    private async Task<PedidoGeralRegistro?> LoteAbertoInternoAsync()
    {
        var lotes = await _banco.LerLotesAsync(VendedorId);
        return lotes.SingleOrDefault(l => l.SituacaoLocal != "Sincronizado");
    }

    private async Task GarantirEdicaoAsync()
    {
        if ((await LoteAbertoInternoAsync())?.SituacaoLocal == "EnvioIncerto")
            throw new InvalidOperationException("Existe um envio a verificar. Abra Lista de Pedidos e retome o envio antes de editar.");
    }

    public Task<PedidoGeralRegistro?> ObterLoteAbertoAsync() => ExecutarAsync(LoteAbertoInternoAsync);
    public Task<List<PedidoResumo>> ObterPedidosClienteAsync(int clienteId) => ExecutarAsync(() => LerPedidosClienteInternoAsync(clienteId));
    private async Task<List<PedidoResumo>> LerPedidosClienteInternoAsync(int clienteId)
    {
        var lotes = await _banco.LerLotesAsync(VendedorId);
        return lotes.SelectMany(r => JsonDados.Ler<PedidoGeral>(r.ConteudoJson).Pedidos.Where(p => p.ClienteId == clienteId).Select(p => new PedidoResumo { IdentificadorEnvio = r.IdentificadorEnvio, Pedido = p, SituacaoLocal = r.SituacaoLocal, MotivoCancelamento = JsonDados.Ler<PedidoGeral>(r.ConteudoJson).MotivoCancelamento })).OrderByDescending(p => p.Pedido.DataHoraCriacao).ThenByDescending(p => p.Pedido.Id).ThenBy(p => p.IdentificadorEnvio, StringComparer.Ordinal).ToList();
    }

    public Task<PedidoResumo?> ObterUltimoPedidoClienteAsync(int clienteId) => ExecutarAsync(async () =>
        (await LerPedidosClienteInternoAsync(clienteId)).FirstOrDefault());

    public Task ComprarNovamenteAsync(int clienteId, string identificadorOrigem, bool substituirCarrinho = false) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var dados = await LerDadosInternoAsync();
        var cliente = dados.Clientes.SingleOrDefault(c => c.Id == clienteId && c.Ativo && c.VendedorId == VendedorId)
            ?? throw new InvalidOperationException("O cliente não está ativo na sua carteira.");
        var ultimo = (await LerPedidosClienteInternoAsync(clienteId)).FirstOrDefault()
            ?? throw new InvalidOperationException("Este cliente ainda não possui um pedido anterior para repetir.");
        if (ultimo.IdentificadorEnvio != identificadorOrigem)
            throw new InvalidOperationException("O último pedido mudou. Atualize o catálogo e confira os itens antes de comprar novamente.");

        var registro = await _banco.LerCarrinhoAsync(VendedorId, clienteId);
        var carrinho = registro is null ? null : JsonDados.Ler<Pedido>(registro.ConteudoJson);
        if (!substituirCarrinho && carrinho is not null &&
            (carrinho.Itens.Count > 0 || carrinho.CondicaoPagamentoId != 0 || carrinho.Parcelas.Count > 0))
            throw new InvalidOperationException("Confirme a substituição dos itens e do pagamento do carrinho atual.");

        var anteriores = ultimo.Pedido.Itens;
        if (anteriores.Count == 0 || anteriores.Any(i => i.QuantidadeSolicitada <= 0) ||
            anteriores.Select(i => i.ProdutoId).Distinct().Count() != anteriores.Count)
            throw new InvalidOperationException("O pedido anterior não possui itens válidos para repetir. Escolha os produtos no catálogo.");

        var indisponiveis = anteriores.Where(i => !dados.Produtos.Any(p => p.Id == i.ProdutoId && p.Ativo)).ToList();
        if (indisponiveis.Count > 0)
            throw new InvalidOperationException($"Não foi possível repetir o pedido. Produtos indisponíveis: {string.Join(", ", indisponiveis.Select(i => i.NomeProduto))}. O carrinho foi preservado; escolha os produtos no catálogo.");

        // Criar outro objeto evita copiar pagamento, atendimento, status e identificador do pedido anterior.
        var novo = new Pedido
        {
            ClienteId = cliente.Id,
            ClienteNome = cliente.Nome
        };
        foreach (var anterior in anteriores)
        {
            var produto = dados.Produtos.Single(p => p.Id == anterior.ProdutoId && p.Ativo);
            if (produto.Preco <= 0 || decimal.Round(produto.Preco, 2) != produto.Preco)
                throw new InvalidOperationException($"O preço de {produto.Nome} está inválido. Atualize o catálogo antes de comprar novamente.");
            novo.Itens.Add(new ItemPedido
            {
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                QuantidadeSolicitada = anterior.QuantidadeSolicitada,
                PrecoUnitario = produto.Preco
            });
        }

        RegrasPedido.Recalcular(novo);
        // Uma única gravação: nenhuma falha acima substitui parcialmente o carrinho existente.
        await _banco.SalvarCarrinhoAsync(VendedorId, novo);
        return true;
    });
    public Task<PedidoResumo> ObterDetalheAsync(string identificador, int clienteId) => ExecutarAsync(async () =>
    {
        var registro = await _banco.LerLoteAsync(identificador);
        if (registro is null || registro.VendedorId != VendedorId)
            throw new InvalidOperationException("Pedido não encontrado nesta conta.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        return new PedidoResumo
        {
            IdentificadorEnvio = identificador,
            SituacaoLocal = registro.SituacaoLocal,
            Pedido = lote.Pedidos.Single(p => p.ClienteId == clienteId),
            MotivoCancelamento = lote.MotivoCancelamento
        };
    });
    public Task<Pedido> AbrirCarrinhoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var existente = await _banco.LerCarrinhoAsync(VendedorId, clienteId);
        if (existente is not null)
            return JsonDados.Ler<Pedido>(existente.ConteudoJson);
        var dados = await LerDadosInternoAsync();
        var cliente = dados.Clientes.SingleOrDefault(c => c.Id == clienteId && c.Ativo) ?? throw new InvalidOperationException("O cliente não está ativo na carteira sincronizada.");
        var registro = await LoteAbertoInternoAsync();
        var lote = registro is null ? null : JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        var pedido = lote?.Pedidos.SingleOrDefault(p => p.ClienteId == clienteId) ?? new Pedido
        {
            ClienteId = clienteId,
            ClienteNome = cliente.Nome
        };
        await _banco.SalvarCarrinhoAsync(VendedorId, pedido);
        return pedido;
    });
    private async Task<Pedido> LerCarrinhoInternoAsync(int clienteId)
    {
        var registro = await _banco.LerCarrinhoAsync(VendedorId, clienteId) ?? throw new InvalidOperationException("Abra um novo pedido para este cliente.");
        return JsonDados.Ler<Pedido>(registro.ConteudoJson);
    }

    public Task<Pedido> ObterCarrinhoAsync(int clienteId) => ExecutarAsync(() => LerCarrinhoInternoAsync(clienteId));
    private Task EditarCarrinhoAsync(int clienteId, Func<Pedido, DadosSincronizacao, Task> editar) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var pedido = await LerCarrinhoInternoAsync(clienteId);
        await editar(pedido, await LerDadosInternoAsync());
        RegrasPedido.Recalcular(pedido);
        await _banco.SalvarCarrinhoAsync(VendedorId, pedido);
        return true;
    });
    public Task AdicionarProdutoAsync(int clienteId, int produtoId, int quantidade) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        if (quantidade <= 0)
            throw new InvalidOperationException("Informe uma quantidade inteira positiva.");
        var produto = dados.Produtos.SingleOrDefault(p => p.Id == produtoId && p.Ativo) ?? throw new InvalidOperationException("Produto indisponível.");
        var item = pedido.Itens.SingleOrDefault(i => i.ProdutoId == produtoId);
        if (item is null)
            pedido.Itens.Add(new ItemPedido { ProdutoId = produto.Id, NomeProduto = produto.Nome, PrecoUnitario = produto.Preco, QuantidadeSolicitada = quantidade });
        else
            item.QuantidadeSolicitada = checked(item.QuantidadeSolicitada + quantidade);
        return Task.CompletedTask;
    });
    public Task AlterarQuantidadeAsync(int clienteId, int produtoId, int quantidade) => EditarCarrinhoAsync(clienteId, (pedido, _) =>
    {
        if (quantidade < 0)
            throw new InvalidOperationException("Quantidade inválida.");
        var item = pedido.Itens.Single(i => i.ProdutoId == produtoId);
        if (quantidade == 0)
            pedido.Itens.Remove(item);
        else
            item.QuantidadeSolicitada = quantidade;
        return Task.CompletedTask;
    });
    public Task SelecionarCondicaoAsync(int clienteId, int condicaoId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        RegrasPedido.AplicarCondicao(pedido, dados.CondicoesPagamento.Single(c => c.Id == condicaoId));
        return Task.CompletedTask;
    });
    public Task SelecionarFormaAsync(int clienteId, int numeroParcela, int formaId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        var forma = dados.FormasPagamento.Single(f => f.Id == formaId && f.Ativo);
        var parcela = pedido.Parcelas.Single(p => p.NumeroParcela == numeroParcela);
        parcela.FormaPagamentoId = forma.Id;
        parcela.FormaPagamentoNome = forma.Descricao;
        return Task.CompletedTask;
    });
    public Task AtualizarPrecosAsync(int clienteId) => EditarCarrinhoAsync(clienteId, (pedido, dados) =>
    {
        foreach (var item in pedido.Itens)
        {
            var atual = dados.Produtos.SingleOrDefault(p => p.Id == item.ProdutoId && p.Ativo) ?? throw new InvalidOperationException($"Remova o produto indisponível: {item.NomeProduto}.");
            item.PrecoUnitario = atual.Preco;
            item.NomeProduto = atual.Nome;
        }

        return Task.CompletedTask;
    });
    public Task DescartarCarrinhoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        await _banco.ExcluirCarrinhoAsync(VendedorId, clienteId);
        return true;
    });
    public Task AcrescentarPedidoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var pedido = await LerCarrinhoInternoAsync(clienteId);
        RegrasPedido.Validar(pedido, await LerDadosInternoAsync());
        var registro = await LoteAbertoInternoAsync();
        var lote = registro is null ? new PedidoGeral
        {
            VendedorId = VendedorId
        }

        : JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        lote.Pedidos.RemoveAll(p => p.ClienteId == clienteId);
        lote.Pedidos.Add(pedido);
        registro ??= new PedidoGeralRegistro
        {
            IdentificadorEnvio = lote.IdentificadorEnvio,
            VendedorId = VendedorId
        };
        registro.ConteudoJson = JsonDados.Escrever(lote);
        await _banco.ConcluirCarrinhoAsync(registro, clienteId);
        return true;
    });
    public Task RemoverPedidoAsync(int clienteId) => ExecutarAsync(async () =>
    {
        await GarantirEdicaoAsync();
        var registro = await LoteAbertoInternoAsync() ?? throw new InvalidOperationException("A lista está vazia.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        lote.Pedidos.RemoveAll(p => p.ClienteId == clienteId);
        registro.ConteudoJson = JsonDados.Escrever(lote);
        await _banco.ConcluirCarrinhoAsync(registro, clienteId);
        return true;
    });
    public Task<List<Pedido>> ObterCarrinhosPendentesAsync() => ExecutarAsync(async () => (await _banco.LerCarrinhosAsync(VendedorId)).Select(c => JsonDados.Ler<Pedido>(c.ConteudoJson)).ToList());
    public Task<PedidoGeral> EnviarPedidosAsync() => ExecutarAsync(async () =>
    {
        var registro = await LoteAbertoInternoAsync() ?? throw new InvalidOperationException("A lista está vazia.");
        var lote = JsonDados.Ler<PedidoGeral>(registro.ConteudoJson);
        bool retomada = registro.SituacaoLocal == "EnvioIncerto";
        if (!retomada)
        {
            if (lote.Pedidos.Count == 0)
                throw new InvalidOperationException("Acrescente pelo menos um pedido à lista.");
            if ((await _banco.LerCarrinhosAsync(VendedorId)).Count > 0)
                throw new InvalidOperationException("Há carrinhos em edição. Conclua ou descarte os carrinhos indicados na lista antes de enviar.");
            var dados = await LerDadosInternoAsync();
            foreach (var pedido in lote.Pedidos)
                RegrasPedido.Validar(pedido, dados);
            registro.CorpoEnvioJson = JsonDados.Escrever(RegrasPedido.CriarEnvio(lote));
            registro.SituacaoLocal = "EnvioIncerto";
            // Gravar ANTES da requisição permite retomar mesmo após o aplicativo fechar.
            await _banco.SalvarLoteAsync(registro);
        }

        try
        {
            PedidoGeral? recebido = retomada ? await _api.ConsultarLoteAsync(lote.IdentificadorEnvio) : null;
            // Um 404 não libera edição: repetimos o mesmo corpo e o mesmo identificador.
            recebido ??= await _api.EnviarLoteAsync(registro.CorpoEnvioJson ?? throw new InvalidDataException("O corpo do envio não está disponível."));
            ValidarLoteServidor(recebido, VendedorId);
            ValidarCorrespondencia(recebido, lote);
            registro.SituacaoLocal = "Sincronizado";
            registro.ConteudoJson = JsonDados.Escrever(recebido);
            registro.CorpoEnvioJson = null;
            await _banco.SalvarLoteAsync(registro);
            return recebido;
        }
        catch (ApiException ex) when ((ex.Status is 400 or 409 or 422) && ex.LoteNaoGravado)
        {
            // Somente uma rejeição definitiva da API permite corrigir o lote.
            string identificadorAnterior = registro.IdentificadorEnvio;
            lote.IdentificadorEnvio = Guid.NewGuid().ToString();
            registro.IdentificadorEnvio = lote.IdentificadorEnvio;
            registro.ConteudoJson = JsonDados.Escrever(lote);
            registro.SituacaoLocal = "Montagem";
            registro.CorpoEnvioJson = null;
            await _banco.SubstituirLoteRejeitadoAsync(identificadorAnterior, registro);
            throw;
        }
    });
    private static void ValidarLoteServidor(PedidoGeral lote, int vendedorId)
    {
        if (lote.Id <= 0 || lote.VendedorId != vendedorId || !Guid.TryParse(lote.IdentificadorEnvio, out _) || lote.Status is not ("AguardandoConfirmacao" or "Processado" or "Cancelado") || lote.Pedidos.Count == 0 || lote.Pedidos.Any(p => p.Id <= 0) || lote.Pedidos.Select(p => p.ClienteId).Distinct().Count() != lote.Pedidos.Count)
            throw new ApiException(0, "A API retornou um lote incompatível. O envio local será preservado para verificação.");
    }

    private static void ValidarCorrespondencia(PedidoGeral recebido, PedidoGeral local)
    {
        // Quantidades solicitadas e pagamentos permanecem iguais mesmo após o corte.
        if (recebido.IdentificadorEnvio != local.IdentificadorEnvio || !recebido.Pedidos.Select(p => p.ClienteId).Order().SequenceEqual(local.Pedidos.Select(p => p.ClienteId).Order()))
            throw new ApiException(0, "A confirmação não corresponde ao lote enviado.");
        foreach (var pedido in local.Pedidos)
        {
            var servidor = recebido.Pedidos.Single(p => p.ClienteId == pedido.ClienteId);
            var a = pedido.Itens.OrderBy(i => i.ProdutoId).Select(i => (i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario));
            var b = servidor.Itens.OrderBy(i => i.ProdutoId).Select(i => (i.ProdutoId, i.QuantidadeSolicitada, i.PrecoUnitario));
            if (!a.SequenceEqual(b) || pedido.CondicaoPagamentoId != servidor.CondicaoPagamentoId || !pedido.Parcelas.Select(p => (p.NumeroParcela, p.FormaPagamentoId)).Order().SequenceEqual(servidor.Parcelas.Select(p => (p.NumeroParcela, p.FormaPagamentoId)).Order()))
                throw new ApiException(0, "A API alterou os dados solicitados. Consulte o resultado antes de continuar.");
        }
    }
}
````

## src/VanderCake.Mobile/Services/IApiService.cs

````csharp
using VanderCake.Contracts;
using VanderCake.Models;

namespace VanderCake.Services;

public interface IApiService
{
    string? Token
    {
        get; set;
    }

    Task<LoginResposta> EntrarAsync(LoginRequisicao login);
    Task<LoginResposta> RenovarAsync(string refreshToken) =>
        Task.FromException<LoginResposta>(new VanderCake.Helpers.ApiException(401, "Entre novamente."));
    Task<DadosSincronizacao> SincronizarAsync();
    Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao cliente);
    Task<PedidoGeral> EnviarLoteAsync(string json);
    Task<PedidoGeral?> ConsultarLoteAsync(string identificador);
}
````

## src/VanderCake.Mobile/Services/ISessaoSegura.cs

````csharp
namespace VanderCake.Services;

public interface ISessaoSegura
{
    Task<string?> LerAsync();
    Task SalvarAsync(string conteudo);
    Task LimparAsync();
}
````

## src/VanderCake.Mobile/Services/SecureStorageSessao.cs

````csharp
namespace VanderCake.Services;

public sealed class SecureStorageSessao : ISessaoSegura
{
    private const string Chave = "vandercake.sessao.v1";
    public async Task<string?> LerAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(Chave);
        }
        catch
        {
            // Uma chave criptográfica restaurada de outro aparelho pode ser inválida.
            SecureStorage.Default.Remove(Chave);
            return null;
        }
    }

    public Task SalvarAsync(string conteudo) => SecureStorage.Default.SetAsync(Chave, conteudo);
    public Task LimparAsync()
    {
        SecureStorage.Default.Remove(Chave);
        return Task.CompletedTask;
    }
}
````

## src/VanderCake.Mobile/Services/TempoRealService.cs

````csharp
using Microsoft.AspNetCore.SignalR.Client;

namespace VanderCake.Services;

// Os avisos não contêm pedidos: a sincronização autenticada continua sendo a fonte dos dados.
public sealed class TempoRealService(AplicacaoService sistema, string baseUrl) : IAsyncDisposable
{
    private readonly SemaphoreSlim _ciclo = new(1, 1);
    private readonly SemaphoreSlim _aviso = new(0, 1);
    private CancellationTokenSource? _cancelamento;
    private Task? _execucao;
    public event EventHandler? DadosAtualizados;

    public void Sinalizar()
    {
        try { _aviso.Release(); } catch (SemaphoreFullException) { }
    }
    public async Task IniciarAsync()
    {
        await _ciclo.WaitAsync();
        try
        {
            if (_execucao is { IsCompleted: false }) return;
            _cancelamento = new CancellationTokenSource();
            _execucao = ExecutarAsync(_cancelamento.Token);
        }
        finally { _ciclo.Release(); }
    }
    public async Task PararAsync()
    {
        await _ciclo.WaitAsync();
        try
        {
            if (_cancelamento is null) return;
            await _cancelamento.CancelAsync();
            if (_execucao is not null) await _execucao;
            _cancelamento.Dispose();
            _cancelamento = null;
            _execucao = null;
        }
        finally { _ciclo.Release(); }
    }
    private async Task ExecutarAsync(CancellationToken ct)
    {
        HubConnection? hub = null;
        int? vendedorId = null;
        var ultima = DateTimeOffset.MinValue;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bool recebeu = _aviso.Wait(0);
                    int? atual = sistema.VendedorAtual?.Id;
                    if (atual != vendedorId)
                    {
                        if (hub is not null) await hub.DisposeAsync();
                        hub = null;
                        vendedorId = atual;
                        ultima = DateTimeOffset.MinValue;
                    }
                    if (sistema.TemSessao)
                    {
                        if (hub is null)
                        {
                            var endereco = new Uri(new Uri(baseUrl), "/hubs/atualizacoes");
                            hub = new HubConnectionBuilder().WithUrl(endereco, o => o.AccessTokenProvider = sistema.ObterTokenAsync)
                                .WithAutomaticReconnect([TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15)]).Build();
                            hub.On("DadosAlterados", (System.Text.Json.JsonElement _) => Sinalizar());
                            hub.Reconnected += _ => { Sinalizar(); return Task.CompletedTask; };
                        }
                        if (hub.State == HubConnectionState.Disconnected)
                        {
                            try { await hub.StartAsync(ct); Sinalizar(); }
                            catch (Exception) when (!ct.IsCancellationRequested) { /* O HTTP ainda pode funcionar. */ }
                        }
                        if (recebeu || DateTimeOffset.UtcNow - ultima >= TimeSpan.FromSeconds(30))
                        {
                            await sistema.AtualizarAsync();
                            ultima = DateTimeOffset.UtcNow;
                            DadosAtualizados?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    // Sem apagar SQLite/carrinhos. A próxima passagem tentará recuperar a conexão.
                    System.Diagnostics.Debug.WriteLine($"Atualização adiada: {ex.GetType().Name}");
                }
                if (await _aviso.WaitAsync(TimeSpan.FromSeconds(5), ct)) Sinalizar();
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        finally { if (hub is not null) await hub.DisposeAsync(); }
    }
    public async ValueTask DisposeAsync() { await PararAsync(); _ciclo.Dispose(); _aviso.Dispose(); }
}
````

## src/VanderCake.Mobile/VanderCake.Mobile.csproj

````xml
<?xml version='1.0' encoding='utf-8'?>
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>net10.0-android</TargetFrameworks>
        <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
        <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">$(TargetFrameworks);net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
        <OutputType>Exe</OutputType>
        <RootNamespace>VanderCake</RootNamespace>
        <UseMaui>true</UseMaui>
        <SingleProject>true</SingleProject>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <MauiEnableXamlCBindingWithSourceCompilation>true</MauiEnableXamlCBindingWithSourceCompilation>
        <AndroidManifest Condition="'$(Configuration)' == 'Debug'">Platforms/Android/AndroidManifest.Debug.xml</AndroidManifest>
        <ApplicationTitle>VanderCake</ApplicationTitle>
        <ApplicationId>br.com.vandercake.mobile</ApplicationId>
        <ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
        <ApplicationVersion>1</ApplicationVersion>
        <WindowsPackageType>None</WindowsPackageType>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">23.0</SupportedOSPlatformVersion>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>
        <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
        <TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
    </PropertyGroup>
    <ItemGroup>
        <MauiIcon Include="Resources/AppIcon/appicon.svg" Color="#EA1D2C"/>
        <MauiSplashScreen Include="Resources/Splash/splash.svg" Color="#EA1D2C" BaseSize="128,128"/>
        <MauiFont Include="Resources/Fonts/*"/>
    </ItemGroup>
    <ItemGroup>
        <PackageReference Include="Microsoft.Maui.Controls" Version="10.0.110"/>
        <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.12"/>
        <PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.14"/>
        <PackageReference Include="sqlite-net-pcl" Version="1.11.285"/>
    </ItemGroup>
</Project>
````

## src/VanderCake.Mobile/Views/PaginaBase.cs

````csharp
using VanderCake.Helpers;

namespace VanderCake.Views;

public class PaginaBase : ContentPage
{
    private bool _visivel;
    private bool _recarregarPendente;
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _visivel = true;
        App.TempoReal.DadosAtualizados -= AoAtualizarDados;
        App.TempoReal.DadosAtualizados += AoAtualizarDados;
    }
    protected override void OnDisappearing()
    {
        _visivel = false;
        App.TempoReal.DadosAtualizados -= AoAtualizarDados;
        base.OnDisappearing();
    }
    protected virtual Task RecarregarDadosLocaisAsync() => Task.CompletedTask;
    private void AoAtualizarDados(object? sender, EventArgs e) => MainThread.BeginInvokeOnMainThread(async () =>
    {
        if (!_visivel) return;
        if (Ocupado) { _recarregarPendente = true; return; }
        await ExecutarAsync(RecarregarDadosLocaisAsync);
    });
    private string _erro = "";
    public string Erro
    {
        get => _erro;
        private set
        {
            _erro = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemErro));
        }
    }

    public bool TemErro => !string.IsNullOrWhiteSpace(Erro);
    private bool _ocupado;
    public bool Ocupado
    {
        get => _ocupado;
        private set
        {
            _ocupado = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Livre));
        }
    }
    public bool Livre => !Ocupado;

    public virtual Task AtualizarAoRetomarAsync() => Task.CompletedTask;
    protected async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        if (Ocupado)
            return false;
        Ocupado = true;
        OnPropertyChanged(nameof(Livre));
        Erro = "";
        try
        {
            await acao();
            return true;
        }
        catch (ApiException ex) when (ex.Status == 401 && this is not frmLogin)
        {
            await DisplayAlertAsync("Sessão", ex.Message, "OK");
            App.TrocarRaiz(new frmLogin());
        }
        catch (OverflowException)
        {
            Erro = "A quantidade informada ultrapassa o limite permitido.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Erro = ex is ApiException or InvalidOperationException or InvalidDataException ? ex.Message : "Não foi possível concluir a operação. Tente novamente; os dados já salvos foram preservados.";
        }
        finally
        {
            Ocupado = false;
            OnPropertyChanged(nameof(Livre));
            if (_recarregarPendente && _visivel)
            {
                _recarregarPendente = false;
                AoAtualizarDados(this, EventArgs.Empty);
            }
        }

        return false;
    }

    protected async Task CarregarComSincronizacaoAsync(Func<Task> carregarLocal)
    {
        await carregarLocal();
        try
        {
            await App.Sistema.AtualizarAsync();
        }
        finally
        {
            if (App.Sistema.TemSessao)
                await carregarLocal();
        }
    }

    protected async Task VoltarDoCarrinhoAsync()
    {
        // O retorno direto dispara OnAppearing do histórico ou da lista e atualiza os pedidos.
        var catalogos = Navigation.NavigationStack.OfType<frmCatalogo>().ToList();
        foreach (var pagina in catalogos)
            Navigation.RemovePage(pagina);
        await Navigation.PopAsync();
    }
}
````

## src/VanderCake.Mobile/Views/frmCarrinho.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmCarrinho" x:Name="Pagina" Title="Carrinho" x:DataType="views:frmCarrinho">
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <ScrollView>
                <VerticalStackLayout Spacing="10">
                    <Label Text="Pedido do cliente" Style="{StaticResource Heading}"/>
                    <Label x:Name="ClienteLabel" FontFamily="OpenSansSemibold"/>
                    <VerticalStackLayout x:Name="ItensLayout">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:ItemPedido">
                                <Border Style="{StaticResource Card}">
                                    <VerticalStackLayout Spacing="8">
                                        <Label Text="{Binding NomeProduto}" FontFamily="OpenSansSemibold" FontSize="17"/>
                                        <Label Text="{Binding PrecoTexto, StringFormat='Preço unitário: {0}'}" Style="{StaticResource MutedLabel}"/>
                                        <Grid ColumnDefinitions="Auto,Auto,Auto,*,Auto" ColumnSpacing="10">
                                            <Button Text="−" Style="{StaticResource Secondary}" WidthRequest="44" CommandParameter="{Binding .}" Clicked="OnDiminuir"/>
                                            <Label Grid.Column="1" Text="{Binding QuantidadeTexto}" VerticalOptions="Center"/>
                                            <Button Grid.Column="2" Text="+" Style="{StaticResource Secondary}" WidthRequest="44" CommandParameter="{Binding .}" Clicked="OnAumentar"/>
                                            <Label Grid.Column="4" Text="{Binding SubtotalTexto}" FontFamily="OpenSansSemibold" VerticalOptions="Center"/>
                                        </Grid>
                                        <Button Text="Remover item" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnRemover"/>
                                    </VerticalStackLayout>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </VerticalStackLayout>
                    <Label x:Name="VazioLabel" Text="Seu carrinho está vazio. Volte ao catálogo para escolher produtos."/>
                    <Button Text="Atualizar preços" Style="{StaticResource Secondary}" Clicked="OnPrecos"/>
                    <Border Style="{StaticResource Card}">
                        <VerticalStackLayout Spacing="10">
                            <Label Text="Condição de pagamento" FontFamily="OpenSansSemibold" FontSize="18"/>
                            <Picker x:Name="CondicaoPicker" Title="Selecione a condição" ItemDisplayBinding="{Binding Descricao, x:DataType=models:CondicaoPagamento}" SelectedIndexChanged="OnCondicao"/>
                            <Label Text="Escolha a forma de pagamento de cada parcela." Style="{StaticResource MutedLabel}"/>
                            <VerticalStackLayout x:Name="ParcelasLayout" Spacing="12"/>
                            <Label Text="Os valores são previstos. O prazo começa na confirmação da loja." Style="{StaticResource MutedLabel}"/>
                        </VerticalStackLayout>
                    </Border>
                    <Button Text="Descartar edição deste carrinho" Style="{StaticResource Secondary}" Clicked="OnDescartar"/>
                </VerticalStackLayout>
            </ScrollView>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">
            <Grid ColumnDefinitions="*,Auto">
                <Label Text="Total previsto" VerticalOptions="Center"/>
                <Label x:Name="TotalLabel" Grid.Column="1" FontFamily="OpenSansSemibold" FontSize="23"/>
            </Grid>
            <Button Text="Acrescentar Pedido à Lista" Clicked="OnAcrescentar"/>
        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmCarrinho.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmCatalogo.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmCatalogo" x:Name="Pagina" Title="Catálogo" x:DataType="views:frmCatalogo">
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Carrinho" Clicked="OnCarrinho"/>
    </ContentPage.ToolbarItems>
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <Grid RowDefinitions="Auto,Auto,Auto,*" RowSpacing="8">
                <VerticalStackLayout>
                    <Label x:Name="ClienteLabel" FontFamily="OpenSansSemibold"/>
                    <Label Text="O estoque é informativo. A loja confirmará as quantidades." Style="{StaticResource MutedLabel}"/>
                </VerticalStackLayout>
                <SearchBar Grid.Row="1" Placeholder="Pesquisar produto" TextChanged="OnPesquisar"/>
                <Grid Grid.Row="2" ColumnDefinitions="*,Auto" ColumnSpacing="8">
                    <Picker x:Name="CategoriasPicker" Title="Categoria" ItemDisplayBinding="{Binding Nome, x:DataType=models:Categoria}" SelectedIndexChanged="OnCategoria"/>
                    <Button Grid.Column="1" Text="Atualizar" Style="{StaticResource Secondary}" Clicked="OnAtualizar"/>
                </Grid>
                <CollectionView x:Name="ProdutosView" Grid.Row="3" SelectionMode="None">
                    <CollectionView.Header>
                        <Border x:Name="ComprarNovamenteCard" Style="{StaticResource Card}" IsVisible="False">
                            <VerticalStackLayout Spacing="10">
                                <Label Text="Comprar novamente" FontFamily="OpenSansSemibold" FontSize="20"/>
                                <Label x:Name="UltimoPedidoLabel" Style="{StaticResource MutedLabel}"/>
                                <VerticalStackLayout x:Name="UltimoPedidoItensLayout" Spacing="6">
                                    <BindableLayout.ItemTemplate>
                                        <DataTemplate x:DataType="models:ItemPedido">
                                            <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
                                                <Label Text="{Binding NomeProduto}"/>
                                                <Label Grid.Column="1" Text="{Binding QuantidadeTexto}" FontFamily="OpenSansSemibold"/>
                                            </Grid>
                                        </DataTemplate>
                                    </BindableLayout.ItemTemplate>
                                </VerticalStackLayout>
                                <Label Text="Repete as quantidades solicitadas com os preços do catálogo atual. Selecione o pagamento no carrinho." Style="{StaticResource MutedLabel}"/>
                                <Button Text="Comprar novamente" Clicked="OnComprarNovamente"/>
                            </VerticalStackLayout>
                        </Border>
                    </CollectionView.Header>
                    <CollectionView.EmptyView>
                        <Label Text="Nenhum produto encontrado. Atualize o catálogo pela API." Padding="16"/>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="models:Produto">
                            <Border Style="{StaticResource Card}">
                                <Border.GestureRecognizers>
                                    <TapGestureRecognizer Tapped="OnProduto" CommandParameter="{Binding .}"/>
                                </Border.GestureRecognizers>
                                <VerticalStackLayout Spacing="7">
                                    <Label Text="{Binding Nome}" FontFamily="OpenSansSemibold" FontSize="18"/>
                                    <Label Text="{Binding Descricao}" MaxLines="3" LineBreakMode="TailTruncation"/>
                                    <Label Text="{Binding EstoqueTexto}" Style="{StaticResource MutedLabel}"/>
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding PrecoTexto}" FontSize="20" FontFamily="OpenSansSemibold" VerticalOptions="Center"/>
                                        <Button Grid.Column="1" Text="Adicionar +" CommandParameter="{Binding .}" Clicked="OnAdicionar"/>
                                    </Grid>
                                </VerticalStackLayout>
                            </Border>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </Grid>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">
            <Button x:Name="CarrinhoButton" Text="Ver carrinho" Clicked="OnCarrinho"/>
        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmCatalogo.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmClientes.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmClientes" x:Name="Pagina" Title="Clientes" x:DataType="views:frmClientes">
    <ContentPage.ToolbarItems>
        <ToolbarItem x:Name="ListaToolbar" Text="Lista de Pedidos" Clicked="OnLista" Order="Primary"/>
        <ToolbarItem Text="Sair da conta" Clicked="OnSair" Order="Secondary"/>
    </ContentPage.ToolbarItems>
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <Grid RowDefinitions="Auto,Auto,Auto,*" RowSpacing="10">
                <VerticalStackLayout>
                    <Label Text="Carteira de Clientes" Style="{StaticResource Heading}"/>
                    <Label x:Name="VendedorLabel" Style="{StaticResource MutedLabel}"/>
                    <Label x:Name="SincronizacaoLabel" Style="{StaticResource MutedLabel}"/>
                </VerticalStackLayout>
                <SearchBar Grid.Row="1" Placeholder="Nome, razão social ou CPF/CNPJ" TextChanged="OnPesquisar"/>
                <Grid Grid.Row="2" ColumnDefinitions="*,Auto" ColumnSpacing="8">
                    <Button Text="Novo Cliente+" Clicked="OnNovoCliente"/>
                    <Button Grid.Column="1" Text="Atualizar" Style="{StaticResource Secondary}" Clicked="OnAtualizar"/>
                </Grid>
                <CollectionView x:Name="ClientesView" Grid.Row="3" SelectionMode="None">
                    <CollectionView.EmptyView>
                        <Label Text="Nenhum cliente encontrado. Cadastre um cliente ou atualize sua carteira." Padding="16"/>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="models:Cliente">
                            <Border Style="{StaticResource Card}">
                                <Border.GestureRecognizers>
                                    <TapGestureRecognizer Tapped="OnCliente" CommandParameter="{Binding .}"/>
                                </Border.GestureRecognizers>
                                <VerticalStackLayout Spacing="6">
                                    <Label Text="{Binding Nome}" FontFamily="OpenSansSemibold" FontSize="18"/>
                                    <Label Text="{Binding Identificacao}" Style="{StaticResource MutedLabel}"/>
                                    <Label Text="{Binding Endereco}"/>
                                    <Label Text="{Binding Localidade}" Style="{StaticResource MutedLabel}"/>
                                    <Label Text="{Binding SituacaoTexto}" TextColor="{StaticResource Primary}" Margin="0,4,0,0"/>
                                </VerticalStackLayout>
                            </Border>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </Grid>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">

        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmClientes.xaml.cs

````csharp
using VanderCake.Helpers;
using VanderCake.Models;

namespace VanderCake.Views;

public partial class frmClientes : PaginaBase
{
    private List<Cliente> _clientes = [];
    private string _pesquisa = "";
    private bool _primeira = true;
    public frmClientes()
    {
        InitializeComponent();
        BindingContext = this;
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
        _clientes = dados.Clientes.OrderByDescending(c => c.Ativo).ThenBy(c => c.Nome).ToList();
        VendedorLabel.Text = $"Olá, {App.Sistema.VendedorAtual?.Nome}";
        SincronizacaoLabel.Text = dados.SincronizadoEm is DateTimeOffset data ? $"Atualizado em {data.ToLocalTime():dd/MM HH:mm}" : "Atualize para carregar os dados da sua carteira.";
        var lote = await App.Sistema.ObterLoteAbertoAsync();
        int quantidade = lote is null ? 0 : JsonDados.Ler<PedidoGeral>(lote.ConteudoJson).Pedidos.Count;
        ListaToolbar.Text = $"Lista de Pedidos ({quantidade})";
        Filtrar();
    }

    private void Filtrar() => ClientesView.ItemsSource = _clientes.Where(c => c.Nome.Contains(_pesquisa, StringComparison.OrdinalIgnoreCase) || c.Documento.Contains(_pesquisa, StringComparison.OrdinalIgnoreCase)).ToList();
    private void OnPesquisar(object sender, TextChangedEventArgs e)
    {
        _pesquisa = e.NewTextValue?.Trim() ?? "";
        Filtrar();
    }

    private async void OnCliente(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Cliente cliente)
            await ExecutarAsync(() => Navigation.PushAsync(new frmPedidosCliente(cliente)));
    }

    private async void OnNovoCliente(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmNovoCliente()));
    private async void OnLista(object sender, EventArgs e) => await ExecutarAsync(() => Navigation.PushAsync(new frmListaPedidos()));
    private async void OnAtualizar(object sender, EventArgs e) => await AtualizarAoRetomarAsync();
    protected override Task RecarregarDadosLocaisAsync() => CarregarAsync();
    public override Task AtualizarAoRetomarAsync() => ExecutarAsync(() => CarregarComSincronizacaoAsync(CarregarAsync));
    private async void OnSair(object sender, EventArgs e) => await ExecutarAsync(async () =>
    {
        if (!await DisplayAlertAsync("Sair da conta", "Os pedidos não enviados continuarão guardados para esta conta. Deseja sair?", "Sair", "Continuar"))
            return;
        await App.Sistema.SairAsync();
        App.TrocarRaiz(new frmLogin());
    });
}
````

## src/VanderCake.Mobile/Views/frmDetalhesPedido.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmDetalhesPedido" x:Name="Pagina" Title="Detalhes do pedido" x:DataType="views:frmDetalhesPedido">
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Atualizar" Clicked="OnAtualizar"/>
    </ContentPage.ToolbarItems>
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <ScrollView>
                <VerticalStackLayout Spacing="12">
                    <Label x:Name="ClienteLabel" Style="{StaticResource Heading}"/>
                    <Label x:Name="PedidoLabel" Style="{StaticResource MutedLabel}"/>
                    <Label x:Name="StatusLabel" TextColor="{StaticResource Primary}" FontFamily="OpenSansSemibold"/>
                    <Label x:Name="MotivoLabel" TextColor="#A61925"/>
                    <Label Text="Itens do pedido" FontFamily="OpenSansSemibold" FontSize="19"/>
                    <VerticalStackLayout x:Name="ItensLayout">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:ItemPedido">
                                <Border Style="{StaticResource Card}">
                                    <VerticalStackLayout Spacing="6">
                                        <Label Text="{Binding NomeProduto}" FontFamily="OpenSansSemibold"/>
                                        <Label Text="{Binding AtendimentoTexto}"/>
                                        <Label Text="{Binding PrecoTexto, StringFormat='Preço unitário: {0}'}" Style="{StaticResource MutedLabel}"/>
                                        <Label Text="{Binding SubtotalTexto, StringFormat='Subtotal solicitado: {0}'}"/>
                                        <Label Text="{Binding SubtotalAtendidoTexto}" FontFamily="OpenSansSemibold"/>
                                    </VerticalStackLayout>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </VerticalStackLayout>
                    <Border Style="{StaticResource Card}">
                        <VerticalStackLayout Spacing="8">
                            <Label x:Name="TotalSolicitadoLabel"/>
                            <Label x:Name="TotalAtendidoLabel" FontFamily="OpenSansSemibold" FontSize="19"/>
                        </VerticalStackLayout>
                    </Border>
                    <Label Text="Pagamento" FontFamily="OpenSansSemibold" FontSize="19"/>
                    <Label x:Name="CondicaoLabel"/>
                    <VerticalStackLayout x:Name="ParcelasLayout">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:ParcelaPedido">
                                <Border Style="{StaticResource Card}">
                                    <VerticalStackLayout Spacing="6">
                                        <Grid ColumnDefinitions="*,Auto">
                                            <Label Text="{Binding Descricao}" FontFamily="OpenSansSemibold"/>
                                            <Label Grid.Column="1" Text="{Binding ValorTexto}" FontFamily="OpenSansSemibold"/>
                                        </Grid>
                                        <Label Text="{Binding DetalheTexto}"/>
                                    </VerticalStackLayout>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </VerticalStackLayout>
                </VerticalStackLayout>
            </ScrollView>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">

        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmDetalhesPedido.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmInicial.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmInicial" x:Name="Pagina" Title="VanderCake" x:DataType="views:frmInicial">
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <VerticalStackLayout VerticalOptions="Center" Spacing="18">
                <Label Text="VanderCake" Style="{StaticResource Heading}" HorizontalOptions="Center"/>
                <Label Text="Preparando sua carteira..." HorizontalOptions="Center"/>
                <Button Text="Tentar novamente" Clicked="OnTentar" IsVisible="{Binding TemErro}"/>
            </VerticalStackLayout>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">

        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmInicial.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmListaPedidos.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmListaPedidos" x:Name="Pagina" Title="Lista de Pedidos" x:DataType="views:frmListaPedidos">
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Atualizar" Clicked="OnAtualizar"/>
    </ContentPage.ToolbarItems>
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <ScrollView>
                <VerticalStackLayout Spacing="10">
                    <Label Text="Pedidos para a loja" Style="{StaticResource Heading}"/>
                    <Label x:Name="SituacaoLabel" Style="{StaticResource MutedLabel}"/>
                    <Label x:Name="VazioLabel" Text="A lista está vazia. Acrescente um pedido pelo carrinho de um cliente."/>
                    <VerticalStackLayout x:Name="PedidosLayout">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:Pedido">
                                <Border Style="{StaticResource Card}">
                                    <VerticalStackLayout Spacing="8">
                                        <Label Text="{Binding ClienteNome}" FontFamily="OpenSansSemibold" FontSize="19"/>
                                        <Label Text="{Binding DataTexto}" Style="{StaticResource MutedLabel}"/>
                                        <Label Text="{Binding ResumoItens}"/>
                                        <Label Text="{Binding CondicaoPagamentoDescricao}" Style="{StaticResource MutedLabel}"/>
                                        <Label Text="{Binding TotalTexto}" FontFamily="OpenSansSemibold" FontSize="20"/>
                                        <Grid ColumnDefinitions="*,*,*" ColumnSpacing="6">
                                            <Button Text="Detalhes" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnDetalhes"/>
                                            <Button Grid.Column="1" Text="Editar" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnEditar" IsEnabled="{Binding PodeEditar, Source={x:Reference Pagina}, x:DataType=views:frmListaPedidos}"/>
                                            <Button Grid.Column="2" Text="Remover" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnRemover" IsEnabled="{Binding PodeEditar, Source={x:Reference Pagina}, x:DataType=views:frmListaPedidos}"/>
                                        </Grid>
                                    </VerticalStackLayout>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </VerticalStackLayout>
                    <Label x:Name="CarrinhosLabel" Text="Carrinhos em edição — conclua ou descarte antes de enviar" FontFamily="OpenSansSemibold"/>
                    <VerticalStackLayout x:Name="CarrinhosLayout">
                        <BindableLayout.ItemTemplate>
                            <DataTemplate x:DataType="models:Pedido">
                                <Border Style="{StaticResource Card}">
                                    <VerticalStackLayout Spacing="8">
                                        <Label Text="{Binding ClienteNome}" FontFamily="OpenSansSemibold"/>
                                        <Label Text="Este carrinho ainda não foi concluído." Style="{StaticResource MutedLabel}"/>
                                        <Grid ColumnDefinitions="*,*" ColumnSpacing="8">
                                            <Button Text="Continuar" CommandParameter="{Binding .}" Clicked="OnEditar"/>
                                            <Button Grid.Column="1" Text="Descartar" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnDescartarCarrinho"/>
                                        </Grid>
                                    </VerticalStackLayout>
                                </Border>
                            </DataTemplate>
                        </BindableLayout.ItemTemplate>
                    </VerticalStackLayout>
                </VerticalStackLayout>
            </ScrollView>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">
            <Grid ColumnDefinitions="*,Auto">
                <Label Text="Total previsto do lote" VerticalOptions="Center"/>
                <Label x:Name="TotalLabel" Grid.Column="1" FontFamily="OpenSansSemibold" FontSize="23"/>
            </Grid>
            <Button x:Name="EnviarButton" Text="Enviar para a Loja" Clicked="OnEnviar"/>
        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmListaPedidos.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmLogin.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmLogin" x:Name="Pagina" Title="Entrar" x:DataType="views:frmLogin">
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <ScrollView>
                <VerticalStackLayout Spacing="18" Padding="4,52,4,12" MaximumWidthRequest="480" HorizontalOptions="Fill">
                    <Label Text="VanderCake" FontSize="36" FontFamily="OpenSansSemibold" TextColor="{StaticResource Primary}"/>
                    <Label Text="Sua carteira. Seus clientes. Tudo pronto para o próximo pedido." FontSize="18"/>
                    <Border Style="{StaticResource Card}" Margin="0,18,0,0">
                        <VerticalStackLayout Spacing="12">
                            <Label Text="Acesso do vendedor" FontFamily="OpenSansSemibold" FontSize="20"/>
                            <Label Text="E-mail"/>
                            <Entry x:Name="EmailEntry" Placeholder="vendedor@empresa.com" Keyboard="Email" MaxLength="150" ReturnType="Next"/>
                            <Label Text="Senha"/>
                            <Entry x:Name="SenhaEntry" Placeholder="Sua senha" IsPassword="True" ReturnType="Go" Completed="OnEntrar"/>
                            <Button Text="Login" Clicked="OnEntrar" Margin="0,10,0,0"/>
                        </VerticalStackLayout>
                    </Border>
                    <Label Text="Use a conta de vendedor cadastrada pela loja." Style="{StaticResource MutedLabel}"/>
                </VerticalStackLayout>
            </ScrollView>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">

        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmLogin.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmNovoCliente.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmNovoCliente" x:Name="Pagina" Title="Novo cliente" x:DataType="views:frmNovoCliente">
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <ScrollView>
                <VerticalStackLayout Spacing="8">
                    <Label Text="Novo cliente" Style="{StaticResource Heading}"/>
                    <Label Text="O cadastro ficará associado à sua carteira." Style="{StaticResource MutedLabel}"/>
                    <Border Style="{StaticResource Card}">
                        <VerticalStackLayout Spacing="8">
                            <Label Text="Nome / Razão Social"/>
                            <Entry x:Name="NomeEntry" Keyboard="Default" MaxLength="100"/>
                            <Label Text="CPF / CNPJ"/>
                            <Entry x:Name="DocumentoEntry" Keyboard="Numeric" MaxLength="20"/>
                            <Label Text="Telefone com DDD"/>
                            <Entry x:Name="TelefoneEntry" Keyboard="Telephone" MaxLength="20"/>
                            <Label Text="E-mail (opcional)"/>
                            <Entry x:Name="EmailEntry" Keyboard="Email" MaxLength="150"/>
                            <Label Text="CEP"/>
                            <Entry x:Name="CepEntry" Keyboard="Numeric" MaxLength="9"/>
                            <Label Text="Rua / Logradouro"/>
                            <Entry x:Name="RuaEntry" Keyboard="Default" MaxLength="100"/>
                            <Label Text="Número"/>
                            <Entry x:Name="NumeroEntry" Keyboard="Default" MaxLength="10"/>
                            <Label Text="Bairro"/>
                            <Entry x:Name="BairroEntry" Keyboard="Default" MaxLength="60"/>
                            <Label Text="Estado"/>
                            <Picker x:Name="EstadoPicker" Title="Selecione o estado" ItemDisplayBinding="{Binding Uf, x:DataType=models:Estado}" SelectedIndexChanged="OnEstado"/>
                            <Label Text="Cidade"/>
                            <Picker x:Name="CidadePicker" Title="Selecione a cidade" ItemDisplayBinding="{Binding Nome, x:DataType=models:Cidade}"/>
                        </VerticalStackLayout>
                    </Border>
                    <Button Text="Atualizar cidades e estados" Clicked="OnAtualizar" Style="{StaticResource Secondary}"/>
                </VerticalStackLayout>
            </ScrollView>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">
            <Button Text="Cadastrar cliente" Clicked="OnSalvar"/>
        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmNovoCliente.xaml.cs

````csharp
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
````

## src/VanderCake.Mobile/Views/frmPedidosCliente.xaml

````xml
<?xml version='1.0' encoding='utf-8'?>
<views:PaginaBase xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:views="clr-namespace:VanderCake.Views" xmlns:models="clr-namespace:VanderCake.Models" x:Class="VanderCake.Views.frmPedidosCliente" x:Name="Pagina" Title="Pedidos do cliente" x:DataType="views:frmPedidosCliente">
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="Lista de Pedidos" Clicked="OnLista"/>
    </ContentPage.ToolbarItems>
    <Grid RowDefinitions="Auto,*,Auto" Padding="18,8,18,16" RowSpacing="10">
        <VerticalStackLayout Grid.Row="0" Spacing="6">
            <ActivityIndicator IsRunning="{Binding Ocupado}" IsVisible="{Binding Ocupado}" Color="{StaticResource Primary}"/>
            <Label Text="{Binding Erro}" IsVisible="{Binding TemErro}" TextColor="#A61925" BackgroundColor="#FFF0F1" Padding="10"/>
        </VerticalStackLayout>
        <Grid Grid.Row="1" IsEnabled="{Binding Livre}">
            <Grid RowDefinitions="Auto,Auto,*" RowSpacing="12">
                <VerticalStackLayout Spacing="5">
                    <Label x:Name="NomeLabel" Style="{StaticResource Heading}"/>
                    <Label x:Name="DocumentoLabel" Style="{StaticResource MutedLabel}"/>
                    <Label x:Name="EnderecoLabel" Style="{StaticResource MutedLabel}"/>
                </VerticalStackLayout>
                <Grid Grid.Row="1" ColumnDefinitions="*,Auto" ColumnSpacing="8">
                    <Button x:Name="NovoPedidoButton" Text="Novo Pedido+" Clicked="OnNovo"/>
                    <Button Grid.Column="1" Text="Atualizar" Style="{StaticResource Secondary}" Clicked="OnAtualizar"/>
                </Grid>
                <CollectionView x:Name="PedidosView" Grid.Row="2" SelectionMode="None">
                    <CollectionView.EmptyView>
                        <Label Text="Nenhum pedido encontrado para este cliente." Padding="16"/>
                    </CollectionView.EmptyView>
                    <CollectionView.ItemTemplate>
                        <DataTemplate x:DataType="models:PedidoResumo">
                            <Border Style="{StaticResource Card}">
                                <VerticalStackLayout Spacing="7">
                                    <Grid ColumnDefinitions="*,Auto">
                                        <Label Text="{Binding Pedido.Titulo}" FontFamily="OpenSansSemibold"/>
                                        <Label Grid.Column="1" Text="{Binding Pedido.TotalTexto}" FontFamily="OpenSansSemibold"/>
                                    </Grid>
                                    <Label Text="{Binding SituacaoTexto}" TextColor="{StaticResource Primary}"/>
                                    <Label Text="{Binding Pedido.DataTexto}" Style="{StaticResource MutedLabel}"/>
                                    <Label Text="{Binding Pedido.ResumoItens}" MaxLines="3" LineBreakMode="TailTruncation"/>
                                    <Button Text="Ver detalhes" Style="{StaticResource Secondary}" CommandParameter="{Binding .}" Clicked="OnDetalhes"/>
                                </VerticalStackLayout>
                            </Border>
                        </DataTemplate>
                    </CollectionView.ItemTemplate>
                </CollectionView>
            </Grid>
        </Grid>
        <VerticalStackLayout Grid.Row="2" IsEnabled="{Binding Livre}" Spacing="8">

        </VerticalStackLayout>
    </Grid>
</views:PaginaBase>
````

## src/VanderCake.Mobile/Views/frmPedidosCliente.xaml.cs

````csharp
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
````

## tests/VanderCake.Tests/Program.cs

````csharp
using System.Net;
using System.Text;
using VanderCake.Contracts;
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;
using VanderCake.Services;

int verificacoes = 0;
void Verificar(bool condicao, string descricao)
{
    if (!condicao)
        throw new Exception("FALHOU: " + descricao);
    verificacoes++;
    Console.WriteLine("OK: " + descricao);
}

async Task EsperarErro(Func<Task> acao, string descricao)
{
    bool falhou = false;
    try
    {
        await acao();
    }
    catch (Exception)
    {
        falhou = true;
    }

    Verificar(falhou, descricao);
}

Verificar(Dinheiro.Dividir(100m, 3).SequenceEqual(new[] { 33.33m, 33.33m, 33.34m }), "Arredondamento conserva os centavos na última parcela");
Verificar(Dinheiro.Dividir(0.01m, 3).SequenceEqual(new[] { 0m, 0m, 0.01m }), "Valor pequeno não cria dinheiro adicional");
var caminho = Path.Combine(Path.GetTempPath(), $"vandercake-teste-{Guid.NewGuid():N}.db3");
var seguro = new SessaoEmMemoria();
var api = new ApiDeTeste();
var banco = new BancoLocal(caminho);
var sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
Verificar(!sistema.TemSessao, "Primeira abertura exige login");
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
await sistema.AtualizarAsync();
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 10);
await sistema.SelecionarCondicaoAsync(10, 3);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.SelecionarFormaAsync(10, 2, 2);
await sistema.SelecionarFormaAsync(10, 3, 3);
var carrinho = await sistema.ObterCarrinhoAsync(10);
Verificar(carrinho.Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }), "7 dias Pix, 14 dias boleto, 21 dias dinheiro são independentes");
Verificar(carrinho.Parcelas.Select(p => p.ValorPrevisto).SequenceEqual(new[] { 33.33m, 33.33m, 33.34m }), "Carrinho distribui R$ 100,00 corretamente");
await banco.FecharAsync();
banco = new BancoLocal(caminho);
sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
Verificar(sistema.TemSessao && sistema.VendedorAtual?.Id == 1 && api.Logins == 1, "Reabertura restaura a sessão sem novo login");
Verificar((await sistema.ObterCarrinhoAsync(10)).Itens.Single().QuantidadeSolicitada == 10, "Carrinho sobrevive ao fechamento do banco e do serviço");
await sistema.AcrescentarPedidoAsync(10);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Status == "Rascunho", "Pedido aparece imediatamente no histórico do cliente");
Verificar(JsonDados.Ler<PedidoGeral>((await sistema.ObterLoteAbertoAsync())!.ConteudoJson).Pedidos.Count == 1, "Mesmo pedido aparece na lista geral");
Verificar(api.Posts == 0, "Acrescentar à lista não transmite pedidos");
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 1, "Sincronização não apaga pedidos locais");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AlterarQuantidadeAsync(10, 100, 12);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Itens.Single().QuantidadeSolicitada == 10, "Editar carrinho não altera a versão concluída antes de confirmar");
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Envio bloqueado enquanto há carrinho em edição");
await sistema.DescartarCarrinhoAsync(10);
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Itens.Single().QuantidadeSolicitada == 10, "Descartar edição preserva o pedido anterior");
await sistema.SairAsync();
await sistema.EntrarAsync("vendedor2@teste.local", "senha");
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 0 && await sistema.ObterLoteAbertoAsync() is null, "Outra conta não acessa os pedidos do vendedor anterior");
await sistema.SairAsync();
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
Verificar((await sistema.ObterPedidosClienteAsync(10)).Count == 1, "Rascunhos reaparecem no login da conta correta");
await sistema.AbrirCarrinhoAsync(11);
await sistema.AdicionarProdutoAsync(11, 100, 2);
await sistema.SelecionarCondicaoAsync(11, 1);
await sistema.SelecionarFormaAsync(11, 1, 3);
await sistema.AcrescentarPedidoAsync(11);
api.FalharDepoisDeGravar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Simulação de resposta perdida após gravação no servidor");
Verificar((await sistema.ObterLoteAbertoAsync())?.SituacaoLocal == "EnvioIncerto", "Falha deixa o lote protegido e persistido");
await EsperarErro(() => sistema.RemoverPedidoAsync(10), "Envio incerto bloqueia alteração de conteúdo");
await EsperarErro(() => sistema.ComprarNovamenteAsync(10, identificadorOrigem: ""), "Envio incerto bloqueia a recompra");
await banco.FecharAsync();
banco = new BancoLocal(caminho);
sistema = new AplicacaoService(banco, api, seguro);
await sistema.InicializarAsync();
var recebido = await sistema.EnviarPedidosAsync();
Verificar(recebido.Pedidos.Count == 2 && api.Posts == 1, "Retomada após reabertura consulta o lote recebido sem duplicar o POST");
Verificar(await sistema.ObterLoteAbertoAsync() is null, "Lote recebido libera uma nova lista");
Verificar((await sistema.ObterPedidosClienteAsync(10)).Single().Pedido.Id > 0, "Pedido local é conciliado com o identificador oficial sem duplicar histórico");
var envio = JsonDados.Ler<PedidoGeralEnvio>(api.Corpos.Single());
Verificar(envio.Pedidos.Single(p => p.ClienteId == 10).Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }), "API recebe a forma de pagamento de cada parcela");
api.Lotes[0].Status = "Processado";
var confirmado = api.Lotes[0].Pedidos.Single(p => p.ClienteId == 10);
confirmado.Status = "ConfirmadoComCorte";
confirmado.Itens[0].QuantidadeAtendida = 6;
confirmado.ValorTotalAtendido = 60m;
foreach (var parcela in confirmado.Parcelas)
{
    parcela.ValorFinal = 20m;
    parcela.DataVencimento = new DateTime(2026, 10, 22).AddDays(parcela.DiasAposConfirmacao);
    parcela.Situacao = "Pendente";
}

await sistema.AtualizarAsync();
var detalhe = await sistema.ObterDetalheAsync(recebido.IdentificadorEnvio, 10);
Verificar(detalhe.Pedido.Itens[0].QuantidadeSolicitada == 10 && detalhe.Pedido.Itens[0].QuantidadeAtendida == 6 && detalhe.Pedido.ValorTotalAtendido == 60m, "Resultado do corte preserva solicitação e atualiza atendimento/valor");
Verificar(detalhe.Pedido.Parcelas.All(p => p.ValorFinal == 20m), "Valores finais das parcelas são exibidos a partir da resposta da loja");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 1);
await sistema.SelecionarCondicaoAsync(10, 1);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.AcrescentarPedidoAsync(10);
api.FalharAntesDeGravar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Falha de rede antes do recebimento mantém a mesma chave");
await sistema.EnviarPedidosAsync();
Verificar(api.Corpos[^1] == api.Corpos[^2], "Consulta 404 retransmite exatamente o corpo congelado");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AdicionarProdutoAsync(10, 100, 1);
await sistema.SelecionarCondicaoAsync(10, 1);
await sistema.SelecionarFormaAsync(10, 1, 1);
await sistema.AcrescentarPedidoAsync(10);
api.Rejeitar = true;
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Rejeição definitiva de validação não perde a lista");
Verificar((await sistema.ObterLoteAbertoAsync())?.SituacaoLocal == "Montagem", "Somente rejeição com loteNaoGravado libera edição");
api.Rejeitar = false;
api.Preco = 12m;
await sistema.AtualizarAsync();
Verificar((await sistema.ObterPedidosClienteAsync(10)).First().Pedido.Itens[0].PrecoUnitario == 10m, "Atualizar catálogo não altera preços históricos ou de rascunhos");
await EsperarErro(() => sistema.EnviarPedidosAsync(), "Preço divergente exige revisão explícita");
await sistema.AbrirCarrinhoAsync(10);
await sistema.AtualizarPrecosAsync(10);
Verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 12m, "Revisão explícita atualiza o valor e recalcula as parcelas");
var novoCliente = await sistema.CadastrarClienteAsync(new NovoClienteRequisicao
{
    Nome = "Cliente novo",
    Documento = "12345678901",
    Rua = "Rua Teste",
    Bairro = "Centro",
    Numero = "10",
    Cep = "85851000",
    CidadeId = 1,
    Telefone = "45999999999"
});
Verificar(novoCliente.VendedorId == 1 && (await sistema.ObterDadosAsync()).Clientes.Any(c => c.Id == novoCliente.Id), "Cadastro confirmado pela API aparece imediatamente na carteira");
await EsperarErro(() => sistema.CadastrarClienteAsync(new NovoClienteRequisicao
{
    Nome = "Cliente repetido",
    Documento = "123.456.789-01",
    Rua = "Rua Teste",
    Bairro = "Centro",
    Numero = "10",
    Cep = "85851000",
    CidadeId = 1,
    Telefone = "45999999999"
}), "CPF com outra pontuação não permite duplicar cliente na carteira");
api.ErroSincronizacao = 0;
await EsperarErro(() => sistema.AtualizarAsync(), "Sincronização sem rede informa a falha");
Verificar(sistema.TemSessao && (await sistema.ObterDadosAsync()).Produtos.Count > 0, "Sem rede, sessão e catálogo local continuam disponíveis");
api.ErroSincronizacao = 401;
await EsperarErro(() => sistema.AtualizarAsync(), "Token recusado pela API exige autenticação");
Verificar(!sistema.TemSessao && await seguro.LerAsync() is null, "401 remove a sessão persistida sem apagar os rascunhos");
api.ErroSincronizacao = null;
await sistema.EntrarAsync("vendedor1@teste.local", "senha");
Verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 12m, "Carrinho preservado após expiração da sessão");

var http = new HttpClient(new HandlerDeTeste())
{
    BaseAddress = new Uri("https://teste.invalid/api/")
};
var transporte = new ApiService(http)
{
    Token = "token-teste"
};
try
{
    await transporte.EnviarLoteAsync("{}");
    throw new Exception("Deveria rejeitar");
}
catch (ApiException ex)
{
    Verificar(ex.Status == 422 && ex.LoteNaoGravado && ex.Message == "Revise o pedido.", "Transporte interpreta rejeição definitiva da API");
}

await banco.FecharAsync();
File.Delete(caminho);
await RecompraTests.ExecutarAsync(Verificar);
Console.WriteLine($"\n{verificacoes} verificações concluídas.");
sealed class SessaoEmMemoria : ISessaoSegura
{
    private string? _valor;
    public Task<string?> LerAsync() => Task.FromResult(_valor);
    public Task SalvarAsync(string conteudo)
    {
        _valor = conteudo;
        return Task.CompletedTask;
    }

    public Task LimparAsync()
    {
        _valor = null;
        return Task.CompletedTask;
    }
}

sealed class HandlerDeTeste : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Parameter != "token-teste" || request.RequestUri?.AbsolutePath != "/api/pedidos-gerais")
            throw new Exception("Cabeçalho ou rota incorretos.");
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.UnprocessableEntity) { Content = new StringContent("{\"mensagem\":\"Revise o pedido.\",\"loteNaoGravado\":true}", Encoding.UTF8, "application/json") });
    }
}

sealed class ApiDeTeste : IApiService
{
    public string? Token
    {
        get; set;
    }
    public int Logins
    {
        get; private set;
    }
    public int Posts
    {
        get; private set;
    }
    public bool FalharDepoisDeGravar
    {
        get; set;
    }
    public bool FalharAntesDeGravar
    {
        get; set;
    }
    public bool Rejeitar
    {
        get; set;
    }
    public decimal Preco { get; set; } = 10m;
    public int? ErroSincronizacao
    {
        get; set;
    }
    public List<string> Corpos { get; } = [];
    public List<PedidoGeral> Lotes { get; } = [];

    private int _vendedorId = 1;
    private DadosSincronizacao Dados() => new()
    {
        Vendedor = new Vendedor
        {
            Id = _vendedorId,
            Nome = "Vendedor de teste"
        },
        Clientes = new[]
        {
            10,
            11
        }.Select(id => new Cliente { Id = id, VendedorId = _vendedorId, Nome = $"Cliente {id}" }).ToList(),
        Estados = [new Estado { Id = 1, Nome = "Paraná", Uf = "PR" }],
        Cidades = [new Cidade { Id = 1, EstadoId = 1, Nome = "Foz do Iguaçu" }],
        Produtos = [new Produto
        {
            Id = 100,
            Nome = "Produto de teste",
            Preco = Preco,
            Estoque = 0,
            CategoriaId = 1
        }

        ],
        CondicoesPagamento = new[]
        {
            new[]
            {
                0
            },
            new[]
            {
                7,
                14
            },
            new[]
            {
                7,
                14,
                21
            }
        }.Select((dias, i) => new CondicaoPagamento { Id = i + 1, Descricao = i == 0 ? "À vista" : string.Join('/', dias), Parcelas = dias.Select((dia, j) => new RegraParcela { NumeroParcela = j + 1, DiasAposConfirmacao = dia }).ToList() }).ToList(),
        FormasPagamento = [new()
        {
            Id = 1,
            Descricao = "Pix"
        }, new()
        {
            Id = 2,
            Descricao = "Boleto bancário"
        }, new()
        {
            Id = 3,
            Descricao = "Dinheiro"
        }

        ],
        PedidosGerais = JsonDados.Copiar(Lotes.Where(l => l.VendedorId == _vendedorId).ToList())
    };
    public Task<LoginResposta> EntrarAsync(LoginRequisicao login)
    {
        Logins++;
        _vendedorId = login.Email.Contains("2@") ? 2 : 1;
        return Task.FromResult(new LoginResposta { AccessToken = "token", Vendedor = Dados().Vendedor });
    }

    public Task<DadosSincronizacao> SincronizarAsync()
    {
        if (ErroSincronizacao is int status)
            throw new ApiException(status, "Falha simulada de sincronização.");
        return Task.FromResult(Dados());
    }
    public Task<Cliente> CadastrarClienteAsync(NovoClienteRequisicao cliente) => Task.FromResult(new Cliente
    {
        Id = 900,
        VendedorId = _vendedorId,
        Nome = cliente.Nome,
        Documento = cliente.Documento,
        Rua = cliente.Rua,
        Bairro = cliente.Bairro,
        Numero = cliente.Numero,
        Cep = cliente.Cep,
        CidadeId = cliente.CidadeId,
        Telefone = cliente.Telefone
    });
    public Task<PedidoGeral?> ConsultarLoteAsync(string identificador) => Task.FromResult(Lotes.FirstOrDefault(l => l.IdentificadorEnvio == identificador));
    public Task<PedidoGeral> EnviarLoteAsync(string json)
    {
        Posts++;
        Corpos.Add(json);
        if (Rejeitar)
            throw new ApiException(422, "Pedido rejeitado.", true);
        if (FalharAntesDeGravar)
        {
            FalharAntesDeGravar = false;
            throw new ApiException(0, "Timeout");
        }

        var envio = JsonDados.Ler<PedidoGeralEnvio>(json);
        var existente = Lotes.FirstOrDefault(l => l.IdentificadorEnvio == envio.IdentificadorEnvio);
        if (existente is not null)
            return Task.FromResult(JsonDados.Copiar(existente));
        var dados = Dados();
        var lote = new PedidoGeral
        {
            Id = Lotes.Count + 1,
            IdentificadorEnvio = envio.IdentificadorEnvio,
            VendedorId = _vendedorId,
            Status = "AguardandoConfirmacao",
            DataHoraCriacao = envio.DataHoraCriacao,
            Pedidos = envio.Pedidos.Select((p, i) =>
            {
                var pedido = new Pedido
                {
                    Id = (Lotes.Count + 1) * 100 + i,
                    ClienteId = p.ClienteId,
                    ClienteNome = $"Cliente {p.ClienteId}",
                    DataHoraCriacao = p.DataHoraCriacao,
                    Status = "Pendente",
                    Itens = p.Itens.Select(x => new ItemPedido { ProdutoId = x.ProdutoId, NomeProduto = "Produto de teste", QuantidadeSolicitada = x.QuantidadeSolicitada, PrecoUnitario = x.PrecoUnitario }).ToList()
                };
                RegrasPedido.AplicarCondicao(pedido, dados.CondicoesPagamento.Single(c => c.Id == p.CondicaoPagamentoId));
                foreach (var parcela in pedido.Parcelas)
                {
                    parcela.FormaPagamentoId = p.Parcelas.Single(x => x.NumeroParcela == parcela.NumeroParcela).FormaPagamentoId;
                    parcela.FormaPagamentoNome = dados.FormasPagamento.Single(f => f.Id == parcela.FormaPagamentoId).Descricao;
                }

                return pedido;
            }).ToList()
        };
        Lotes.Add(lote);
        if (FalharDepoisDeGravar)
        {
            FalharDepoisDeGravar = false;
            throw new ApiException(0, "Resposta perdida");
        }

        return Task.FromResult(JsonDados.Copiar(lote));
    }
}
````

## tests/VanderCake.Tests/RecompraTests.cs

````csharp
using VanderCake.Data;
using VanderCake.Helpers;
using VanderCake.Models;
using VanderCake.Rules;
using VanderCake.Services;

internal static class RecompraTests
{
    public static async Task ExecutarAsync(Action<bool, string> verificar)
    {
        async Task EsperarBloqueio(Func<Task> acao, string descricao)
        {
            bool bloqueado = false;
            try
            {
                await acao();
            }
            catch (InvalidOperationException)
            {
                bloqueado = true;
            }
            verificar(bloqueado, descricao);
        }

        var caminho = Path.Combine(Path.GetTempPath(), $"vandercake-recompra-{Guid.NewGuid():N}.db3");
        var banco = new BancoLocal(caminho);
        var api = new ApiDeTeste();
        var seguro = new SessaoEmMemoria();
        var sistema = new AplicacaoService(banco, api, seguro);
        try
        {
            await sistema.InicializarAsync();
            await sistema.EntrarAsync("vendedor1@teste.local", "senha");
            await sistema.AtualizarAsync();
            verificar(await sistema.ObterUltimoPedidoClienteAsync(10) is null, "Sem histórico, a consulta de recompra não retorna pedido");
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, Guid.NewGuid().ToString()),
                "Não é possível repetir um pedido inexistente");

            var dados = await sistema.ObterDadosAsync();
            dados.Produtos[0].Nome = "Farinha atual";
            dados.Produtos[0].Preco = 12m;
            dados.Produtos.Add(new Produto { Id = 101, Nome = "Chocolate", Preco = 3.50m, Estoque = 0 });
            await banco.SalvarDadosAsync(dados);

            Pedido CriarPedido(int id, int clienteId, int dias, int quantidade) => new()
            {
                Id = id,
                ClienteId = clienteId,
                ClienteNome = $"Cliente {clienteId}",
                DataHoraCriacao = DateTimeOffset.UtcNow.AddDays(-dias),
                Status = "Pendente",
                Itens = [new ItemPedido
                {
                    ProdutoId = 100,
                    NomeProduto = "Farinha antiga",
                    PrecoUnitario = 10m,
                    QuantidadeSolicitada = quantidade
                }]
            };

            var anterior = CriarPedido(101, 10, 2, 10);
            anterior.Itens[0].QuantidadeAtendida = 6;
            anterior.Itens.Add(new ItemPedido
            {
                ProdutoId = 101,
                NomeProduto = "Chocolate",
                PrecoUnitario = 2.50m,
                QuantidadeSolicitada = 3,
                QuantidadeAtendida = 1
            });
            anterior.Status = "ConfirmadoComCorte";
            anterior.ValorTotalAtendido = 62.50m;
            RegrasPedido.AplicarCondicao(anterior, dados.CondicoesPagamento.Single(c => c.Id == 3));
            foreach (var parcela in anterior.Parcelas)
            {
                parcela.FormaPagamentoId = parcela.NumeroParcela;
                parcela.FormaPagamentoNome = dados.FormasPagamento.Single(f => f.Id == parcela.FormaPagamentoId).Descricao;
                parcela.DataVencimento = DateTime.Today.AddDays(parcela.DiasAposConfirmacao);
                parcela.ValorFinal = 20m;
                parcela.Situacao = "Pago";
            }

            var antigo = new PedidoGeral { Id = 1, VendedorId = 1, Pedidos = [CriarPedido(999, 10, 3, 99)] };
            var origem = new PedidoGeral { Id = 2, VendedorId = 1, Pedidos = [anterior] };
            var outroCliente = new PedidoGeral { Id = 3, VendedorId = 1, Pedidos = [CriarPedido(300, 11, 1, 88)] };
            var outroVendedor = new PedidoGeral { Id = 4, VendedorId = 2, Pedidos = [CriarPedido(400, 10, 0, 77)] };
            foreach (var lote in new[] { antigo, origem, outroCliente, outroVendedor })
            {
                await banco.SalvarLoteAsync(new PedidoGeralRegistro
                {
                    VendedorId = lote.VendedorId,
                    IdentificadorEnvio = lote.IdentificadorEnvio,
                    SituacaoLocal = "Sincronizado",
                    ConteudoJson = JsonDados.Escrever(lote)
                });
            }

            var ultimo = await sistema.ObterUltimoPedidoClienteAsync(10);
            verificar(ultimo?.IdentificadorEnvio == origem.IdentificadorEnvio,
                "Recompra escolhe pela data e isola o cliente e o vendedor");
            verificar((await sistema.ObterUltimoPedidoClienteAsync(11))?.IdentificadorEnvio == outroCliente.IdentificadorEnvio,
                "Cada cliente possui sua própria sugestão de recompra");
            string historicoAntes = JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10));
            await sistema.AbrirCarrinhoAsync(10);
            await sistema.AdicionarProdutoAsync(10, 100, 1);
            await sistema.SelecionarCondicaoAsync(10, 1);
            await sistema.SelecionarFormaAsync(10, 1, 1);
            string carrinhoAntes = JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10));
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio),
                "Carrinho preenchido exige confirmação de substituição");
            verificar(JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10)) == carrinhoAntes,
                "Sem confirmação, os itens e o pagamento existentes são preservados");

            DateTimeOffset inicio = DateTimeOffset.UtcNow;
            await sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true);
            var copiado = await sistema.ObterCarrinhoAsync(10);
            verificar(copiado.Itens.Select(i => (i.ProdutoId, i.QuantidadeSolicitada))
                .SequenceEqual(new[] { (100, 10), (101, 3) }),
                "Recompra copia todos os produtos e as quantidades solicitadas, inclusive antes dos cortes");
            verificar(copiado.Itens[0].NomeProduto == "Farinha atual" && copiado.Itens[0].PrecoUnitario == 12m &&
                copiado.Itens[1].PrecoUnitario == 3.50m && copiado.ValorTotalSolicitado == 130.50m,
                "Recompra usa os nomes e preços atuais e recalcula o total");
            verificar(copiado.CondicaoPagamentoId == 0 && copiado.CondicaoPagamentoDescricao == "" && copiado.Parcelas.Count == 0,
                "Condição, formas, vencimentos e valores antigos das parcelas não são copiados");
            verificar(copiado.Id == 0 && copiado.Status == "Rascunho" && copiado.DataHoraCriacao >= inicio &&
                copiado.ValorTotalAtendido is null && copiado.Itens.All(i => i.QuantidadeAtendida is null),
                "Recompra cria rascunho com nova data, sem IDs oficiais nem atendimento anterior");
            verificar(JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10)) == historicoAntes &&
                await sistema.ObterLoteAbertoAsync() is null && api.Posts == 0,
                "Preencher carrinho não altera histórico, não cria lote nem envia à API");
            await EsperarBloqueio(() => sistema.AcrescentarPedidoAsync(10),
                "Recompra não pode ser concluída sem escolher novamente o pagamento");
            await sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true);
            verificar((await sistema.ObterCarrinhoAsync(10)).ValorTotalSolicitado == 130.50m,
                "Repetir a ação substitui o carrinho sem somar quantidades duplicadas");

            await banco.FecharAsync();
            banco = new BancoLocal(caminho);
            sistema = new AplicacaoService(banco, api, seguro);
            await sistema.InicializarAsync();
            var restaurado = await sistema.ObterCarrinhoAsync(10);
            verificar(restaurado.Itens.Count == 2 && restaurado.ValorTotalSolicitado == 130.50m &&
                restaurado.CondicaoPagamentoId == 0 && restaurado.Parcelas.Count == 0,
                "Carrinho da recompra persiste no SQLite após reinício, com pagamento vazio");
            await sistema.SelecionarCondicaoAsync(10, 3);
            verificar((await sistema.ObterCarrinhoAsync(10)).Parcelas.All(p => p.FormaPagamentoId == 0),
                "Selecionar 7/14/21 após recompra exige escolher cada forma de pagamento");
            for (int numero = 1; numero <= 3; numero++)
                await sistema.SelecionarFormaAsync(10, numero, numero);
            string antesDoBloqueio = JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10));

            dados.Produtos[1].Ativo = false;
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Produto inativo impede copiar parcialmente o pedido");
            var produtoRemovido = dados.Produtos[1];
            dados.Produtos.RemoveAt(1);
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Produto ausente no catálogo também impede a recompra incompleta");
            verificar(JsonDados.Escrever(await sistema.ObterCarrinhoAsync(10)) == antesDoBloqueio,
                "Indisponibilidade preserva integralmente o carrinho e seu pagamento");
            produtoRemovido.Ativo = true;
            dados.Produtos.Add(produtoRemovido);
            dados.Clientes[0].Ativo = false;
            await banco.SalvarDadosAsync(dados);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Recompra exige cliente ativo na carteira atual");
            dados.Clientes[0].Ativo = true;
            await banco.SalvarDadosAsync(dados);

            await sistema.AcrescentarPedidoAsync(10);
            var local = await sistema.ObterUltimoPedidoClienteAsync(10);
            verificar(local?.SituacaoLocal == "Montagem" && local.Pedido.Itens.Count == 2 &&
                local.Pedido.Parcelas.Select(p => p.FormaPagamentoId).SequenceEqual(new[] { 1, 2, 3 }),
                "Após escolher pagamento, recompra é acrescentada à lista e vira o último pedido salvo");
            await sistema.AbrirCarrinhoAsync(10);
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Sugestão antiga é recusada se outro pedido passou a ser o mais recente");
            string historicoLocal = JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10));
            await sistema.ComprarNovamenteAsync(10, local!.IdentificadorEnvio, true);
            verificar((await sistema.ObterCarrinhoAsync(10)).Parcelas.Count == 0 &&
                JsonDados.Escrever(await sistema.ObterPedidosClienteAsync(10)) == historicoLocal,
                "Repetir pedido ainda local limpa apenas o pagamento do carrinho e preserva a lista");

            await sistema.SairAsync();
            await sistema.EntrarAsync("vendedor2@teste.local", "senha");
            await sistema.AtualizarAsync();
            verificar((await sistema.ObterUltimoPedidoClienteAsync(10))?.IdentificadorEnvio == outroVendedor.IdentificadorEnvio,
                "Trocar vendedor não expõe a sugestão de recompra da outra conta");
            await EsperarBloqueio(() => sistema.ComprarNovamenteAsync(10, origem.IdentificadorEnvio, true),
                "Chave de pedido de outro vendedor não pode ser usada para recompra");
        }
        finally
        {
            await banco.FecharAsync();
            File.Delete(caminho);
        }
    }
}
````

## tests/VanderCake.Tests/VanderCake.Tests.csproj

````xml
<?xml version='1.0' encoding='utf-8'?>
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>
    <ItemGroup>
        <Compile Include="../../src/VanderCake.Mobile/Models/**/*.cs" Link="Models/%(Filename)%(Extension)"/>
        <Compile Include="../../src/VanderCake.Mobile/Contracts/**/*.cs" Link="Contracts/%(Filename)%(Extension)"/>
        <Compile Include="../../src/VanderCake.Mobile/Rules/**/*.cs" Link="Rules/%(Filename)%(Extension)"/>
        <Compile Include="../../src/VanderCake.Mobile/Data/**/*.cs" Link="Data/%(Filename)%(Extension)"/>
        <Compile Include="../../src/VanderCake.Mobile/Services/*.cs" Exclude="../../src/VanderCake.Mobile/Services/SecureStorageSessao.cs" Link="Services/%(Filename)%(Extension)"/>
        <Compile Include="../../src/VanderCake.Mobile/Helpers/ApiException.cs" Link="Helpers/ApiException.cs"/>
        <Compile Include="../../src/VanderCake.Mobile/Helpers/JsonDados.cs" Link="Helpers/JsonDados.cs"/>
        <PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.14"/>
        <PackageReference Include="sqlite-net-pcl" Version="1.11.285"/>
    </ItemGroup>
</Project>
````
