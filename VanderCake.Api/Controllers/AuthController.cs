using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("login")]
public sealed class AuthController(AutenticacaoService auth) : ControllerBase
{
    [HttpPost("administradores/login")]
    public Task<object> Administrador(LoginRequisicao r) => auth.EntrarAsync("Administrador", r.Email, r.Senha);

    [HttpPost("vendedores/login")]
    public Task<object> Vendedor(LoginRequisicao r) => auth.EntrarAsync("Vendedor", r.Email, r.Senha);

    [HttpPost("renovar")]
    public Task<object> Renovar(RenovarRequest r) => auth.RenovarAsync(r.RefreshToken);
}
