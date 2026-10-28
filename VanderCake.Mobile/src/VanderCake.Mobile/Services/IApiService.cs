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
