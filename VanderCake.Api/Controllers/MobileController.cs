using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendedor")]
[Route("api")]
public sealed class MobileController(LeituraService leitura, CadastroService cadastros, PedidosService pedidos) : ControllerBase
{
    private int VendedorId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("mobile/sincronizacao")]
    public async Task<object> Sincronizar()
    {
        var d = await leitura.ConsultarAsync(VendedorId);
        var vendedor = d.Vendedores.Single();
        return new
        {
            Vendedor = new { vendedor.Id, vendedor.Nome, vendedor.Email, vendedor.Ativo },
            d.Clientes, d.Estados, d.Cidades, d.Categorias, d.Produtos,
            d.CondicoesPagamento, d.FormasPagamento, d.PedidosGerais, SincronizadoEm = DateTime.UtcNow
        };
    }

    [HttpPost("clientes")]
    public async Task<IActionResult> Cadastrar(NovoClienteRequisicao r) => Ok(await cadastros.SalvarPessoaAsync(new PessoaRequest
    {
        Nome = r.Nome, Documento = r.Documento, Rua = r.Rua, Bairro = r.Bairro, Cep = r.Cep,
        Numero = r.Numero, CidadeId = r.CidadeId, Telefone = r.Telefone, Email = r.Email
    }, null, false, VendedorId));

    [HttpPost("pedidos-gerais")]
    public async Task<IActionResult> Enviar(PedidoGeralEnvio r) => Ok(await pedidos.ReceberAsync(VendedorId, r));

    [HttpGet("pedidos-gerais/por-identificador/{identificador}")]
    public async Task<IActionResult> Consultar(string identificador) => Ok(await pedidos.PorIdentificadorAsync(VendedorId, identificador));
}
