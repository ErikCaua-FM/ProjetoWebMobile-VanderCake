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
