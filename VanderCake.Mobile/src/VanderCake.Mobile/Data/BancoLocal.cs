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
