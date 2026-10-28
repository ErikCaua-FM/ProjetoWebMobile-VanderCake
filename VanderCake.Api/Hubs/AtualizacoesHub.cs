using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VanderCake.Api.Hubs;

[Authorize(Roles = "Administrador,Vendedor")]
public sealed class AtualizacoesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // O cliente não escolhe grupos: eles são derivados da identidade autenticada.
        string grupo = Context.User!.IsInRole("Administrador")
            ? "loja" : $"vendedor:{Context.User.FindFirstValue(ClaimTypes.NameIdentifier)}";
        await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
        await base.OnConnectedAsync();
    }
}
