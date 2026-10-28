namespace VanderCake.Api.Infrastructure;

public sealed class ApiException(int status, string mensagem, bool loteNaoGravado = false) : Exception(mensagem)
{
    public int Status { get; } = status;
    public bool LoteNaoGravado { get; } = loteNaoGravado;
}
