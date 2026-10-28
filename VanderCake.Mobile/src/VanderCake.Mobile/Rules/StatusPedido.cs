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
