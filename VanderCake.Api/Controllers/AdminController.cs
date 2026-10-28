using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VanderCake.Api.Contracts;
using VanderCake.Api.Services;

namespace VanderCake.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrador")]
[Route("api/admin")]
public sealed class AdminController(LeituraService leitura, CadastroService cadastros, PedidosService pedidos, PagamentosService pagamentos) : ControllerBase
{
    private int AdministradorId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("painel")]
    public async Task<object> Painel() => ProjecaoWeb.Painel(await leitura.ConsultarAsync());

    [HttpGet("pedidos-gerais/{id:int}")]
    public async Task<object> Detalhes(int id) => ProjecaoWeb.Detalhes(await leitura.ConsultarAsync(), id);

    [HttpGet("pedidos-gerais/{id:int}/previa")]
    public Task<object> Previa(int id) => pedidos.PreviaAsync(id);

    [HttpPost("pedidos-gerais/{id:int}/confirmacao")]
    public Task<object> Confirmar(int id, ConfirmarRequest r) => pedidos.DecidirAsync(id, AdministradorId, r, null);

    [HttpPost("pedidos-gerais/{id:int}/cancelamento")]
    public Task<object> Cancelar(int id, CancelarRequest r) => pedidos.DecidirAsync(id, AdministradorId, null, r);

    [HttpPost("produtos")]
    public Task<object> Produto(ProdutoRequest r) => cadastros.SalvarProdutoAsync(r, null);
    [HttpPut("produtos/{id:int}")]
    public Task<object> Produto(int id, ProdutoRequest r) => cadastros.SalvarProdutoAsync(r, id);
    [HttpDelete("produtos/{id:int}")]
    public Task<object> Excluir(int id, [FromBody] VersaoRequest r) => cadastros.ExcluirProdutoAsync(id, r.Versao);

    [HttpPost("vendedores")]
    public async Task<IActionResult> Vendedor(PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, null, true));
    [HttpPut("vendedores/{id:int}")]
    public async Task<IActionResult> Vendedor(int id, PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, id, true));
    [HttpPatch("vendedores/{id:int}/atividade")]
    public Task<object> Atividade(int id, AtividadeRequest r) => cadastros.AtividadeAsync(id, r);

    [HttpPost("clientes")]
    public async Task<IActionResult> Cliente(PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, null, false));
    [HttpPut("clientes/{id:int}")]
    public async Task<IActionResult> Cliente(int id, PessoaRequest r) => Ok(await cadastros.SalvarPessoaAsync(r, id, false));

    [HttpPost("categorias")]
    public Task<object> Categoria(CategoriaRequest r) => cadastros.SalvarReferenciaAsync("Categorias", r.Nome, null, null, r.Versao, null, r.Ativo);
    [HttpPut("categorias/{id:int}")]
    public Task<object> Categoria(int id, CategoriaRequest r) => cadastros.SalvarReferenciaAsync("Categorias", r.Nome, null, null, r.Versao, id, r.Ativo);

    [HttpPost("condicoes-pagamento")]
    public Task<object> Condicao(PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, null, true);
    [HttpPut("condicoes-pagamento/{id:int}")]
    public Task<object> Condicao(int id, PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, id, true);
    [HttpPost("formas-pagamento")]
    public Task<object> Forma(PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, null, false);
    [HttpPut("formas-pagamento/{id:int}")]
    public Task<object> Forma(int id, PagamentoCadastroRequest r) => pagamentos.SalvarAsync(r, id, false);
    [HttpPost("contas-receber/{id:int}/recebimento")]
    public Task<object> Receber(int id) => pagamentos.ReceberAsync(id, AdministradorId);

    [HttpPost("estados")]
    public Task<object> Estado(EstadoRequest r) => cadastros.SalvarReferenciaAsync("Estados", r.Nome, r.Uf, null, r.Versao, null);
    [HttpPut("estados/{id:int}")]
    public Task<object> Estado(int id, EstadoRequest r) => cadastros.SalvarReferenciaAsync("Estados", r.Nome, r.Uf, null, r.Versao, id);

    [HttpPost("cidades")]
    public Task<object> Cidade(CidadeRequest r) => cadastros.SalvarReferenciaAsync("Cidades", r.Nome, null, r.EstadoId, r.Versao, null);
    [HttpPut("cidades/{id:int}")]
    public Task<object> Cidade(int id, CidadeRequest r) => cadastros.SalvarReferenciaAsync("Cidades", r.Nome, null, r.EstadoId, r.Versao, id);
}
