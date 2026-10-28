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
