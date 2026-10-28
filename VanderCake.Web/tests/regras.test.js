import test from "node:test";
import assert from "node:assert/strict";
import { criarDemonstracao } from "../dist/js/demo-seed.js";
import { lerCentavos, dividirCentavos, ratear, gerarPrevia, confirmarLote, cancelarLote,
    salvarCadastro, alterarAtividade, excluirProduto, adicionarDias, Conflito, dentroPeriodo } from "../dist/js/regras.js";

test("Filtro por período usa o dia da loja, inclusive na virada UTC", () => {
    assert.equal(dentroPeriodo("2026-10-27T01:00:00Z", "2026-10-26", "2026-10-26"), true);
    assert.equal(dentroPeriodo("2026-10-27T01:00:00Z", "2026-10-27", "2026-10-27"), false);
    assert.equal(dentroPeriodo("2026-10-27T12:00:00Z", "2026-10-27", "2026-10-27"), true);
    assert.equal(dentroPeriodo("inválida", "2026-10-27", ""), false);
});

const lote = base => base.pedidosGerais.find(l => l.id === 1042);
const confirmar = base => confirmarLote(base, 1042, gerarPrevia(lote(base), base.produtos).tokenPrevia,
    new Date("2026-10-24T14:00:00Z"));
const vendedor = () => ({ nome: "Vendedor de teste", documento: "12345678901", email: "teste@exemplo.test",
    telefone: "45999999999", cidadeId: 1, cep: "85851000", rua: "Rua Teste", numero: "1", bairro: "Centro", senha: "Teste123!" });

test("Dinheiro usa centavos, aceita vírgula e rejeita precisão extra", () => {
    assert.equal(lerCentavos("89,50"), 8950);
    assert.equal(lerCentavos("0.01"), 1);
    for (const valor of ["1.999", "-1", "1.000,00", "abc", "10000000000.00"]) assert.throws(() => lerCentavos(valor));
});
test("Divisão mantém o total e coloca a diferença na última parcela", () => {
    assert.deepEqual(dividirCentavos(10000, 3), [3333, 3333, 3334]);
    assert.deepEqual(dividirCentavos(1, 3), [0, 0, 1]);
    assert.throws(() => dividirCentavos(100, 4));
});
test("Rateio proporcional reproduz 10/6/4 com estoque 11", () => {
    assert.deepEqual(ratear(11, [{ clienteId: 1, quantidade: 10 }, { clienteId: 2, quantidade: 6 }, { clienteId: 3, quantidade: 4 }]),
        [{ clienteId: 1, quantidade: 6 }, { clienteId: 2, quantidade: 3 }, { clienteId: 3, quantidade: 2 }]);
});
test("Desempate favorece menor ID e não a ordem da lista", () => {
    assert.deepEqual(ratear(1, [{ clienteId: 2, quantidade: 1 }, { clienteId: 1, quantidade: 1 }]),
        [{ clienteId: 2, quantidade: 0 }, { clienteId: 1, quantidade: 1 }]);
});
test("Rateio conserva estoque mesmo com demandas altas", () => {
    const resultado = ratear(2147483647, [{ clienteId: 1, quantidade: 2147483647 }, { clienteId: 2, quantidade: 2147483647 }]);
    assert.equal(resultado.reduce((s, q) => s + q.quantidade, 0), 2147483647);
    assert.equal(resultado[0].quantidade, resultado[1].quantidade + 1);
    assert.equal(ratear(0, [{ clienteId: 1, quantidade: 20 }])[0].quantidade, 0);
    assert.equal(ratear(30, [{ clienteId: 1, quantidade: 20 }])[0].quantidade, 20);
});
test("Prévia não altera pedidos nem reserva estoque", () => {
    const base = criarDemonstracao();
    const antes = structuredClone(base);
    const previa = gerarPrevia(lote(base), base.produtos);
    assert.deepEqual(base, antes);
    assert.equal(previa.unidadesCortadas, 5);
    assert.deepEqual(previa.pedidos.map(p => p.itens[0].quantidadeAtendida), [8, 4]);
});
test("Confirmação baixa apenas o atendido e preserva as formas por parcela", () => {
    const base = criarDemonstracao();
    const original = structuredClone(lote(base));
    const vendasAntes = base.vendas.length;
    const resultado = confirmar(base);
    assert.equal(resultado.status, "Processado");
    assert.equal(base.produtos[0].estoque, 0);
    assert.equal(base.produtos[1].estoque, 0);
    assert.equal(base.vendas.length, vendasAntes + 2);
    for (let i = 0; i < resultado.pedidos.length; i++) {
        const p = resultado.pedidos[i];
        assert.equal(p.status, "ConfirmadoComCorte");
        assert.deepEqual(p.parcelas.map(x => x.formaPagamentoId), original.pedidos[i].parcelas.map(x => x.formaPagamentoId));
        assert.deepEqual(p.itens.map(x => x.quantidadeSolicitada), original.pedidos[i].itens.map(x => x.quantidadeSolicitada));
        const contas = base.contasReceber.filter(c => c.pedidoId === p.id);
        assert.equal(contas.reduce((s, c) => s + c.valorCentavos, 0), p.valorTotalAtendidoCentavos);
        assert.ok(contas.every(c => c.status === "Pendente"));
    }
});
test("Confirmação repetida não duplica vendas, parcelas ou baixas", () => {
    const base = criarDemonstracao();
    const token = gerarPrevia(lote(base), base.produtos).tokenPrevia;
    confirmarLote(base, 1042, token);
    const antes = structuredClone(base);
    confirmarLote(base, 1042, token);
    assert.deepEqual(base, antes);
});
test("Prévia desatualizada recusa toda a confirmação", () => {
    const base = criarDemonstracao();
    const token = gerarPrevia(lote(base), base.produtos).tokenPrevia;
    base.produtos[0].estoque--;
    const antes = structuredClone(base);
    assert.throws(() => confirmarLote(base, 1042, token), Conflito);
    assert.deepEqual(base, antes);
});
test("Estoque zero processa sem criar venda ou conta a receber", () => {
    const base = criarDemonstracao();
    base.produtos[0].estoque = 0;
    base.produtos[1].estoque = 0;
    const vendas = base.vendas.length;
    const contas = base.contasReceber.length;
    const resultado = confirmar(base);
    assert.ok(resultado.pedidos.every(p => p.status === "NaoAtendido"));
    assert.equal(base.vendas.length, vendas);
    assert.equal(base.contasReceber.length, contas);
});
test("Cancelamento exige motivo e conserva estoque, vendas e parcelas", () => {
    const base = criarDemonstracao();
    assert.throws(() => cancelarLote(base, 1042, " ", 1));
    const antes = structuredClone(base);
    const resultado = cancelarLote(base, 1042, "Pedido duplicado", 1);
    assert.equal(resultado.status, "Cancelado");
    assert.ok(resultado.pedidos.every(p => p.status === "Cancelado" && p.valorTotalAtendidoCentavos === null));
    assert.deepEqual(base.produtos, antes.produtos);
    assert.deepEqual(base.vendas, antes.vendas);
    assert.deepEqual(base.contasReceber, antes.contasReceber);
    const cancelado = structuredClone(base);
    cancelarLote(base, 1042, "Outro motivo", 1);
    assert.deepEqual(base, cancelado);
    assert.throws(() => confirmarLote(base, 1042, "token"));
});
test("Venda confirmada não pode ser cancelada pelo fluxo de pendentes", () => {
    const base = criarDemonstracao();
    confirmar(base);
    const antes = structuredClone(base);
    assert.throws(() => cancelarLote(base, 1042, "Cancelar", lote(base).versao), Conflito);
    assert.deepEqual(base, antes);
});
test("Produtos inativados depois do envio mantêm atendimento e preço histórico", () => {
    const base = criarDemonstracao();
    const preco = lote(base).pedidos[0].itens[0].precoUnitarioCentavos;
    base.produtos[0].ativo = false;
    base.produtos[0].precoCentavos = 99900;
    confirmar(base);
    assert.equal(lote(base).pedidos[0].itens[0].precoUnitarioCentavos, preco);
    assert.equal(lote(base).pedidos[0].itens[0].quantidadeAtendida, 8);
});
test("Vencimentos usam dias corridos e cruzam o mês corretamente", () => {
    const base = criarDemonstracao();
    const resultado = confirmar(base);
    assert.deepEqual(resultado.pedidos[0].parcelas.map(p => p.dataVencimento), ["2026-10-31", "2026-11-07", "2026-11-14"]);
    assert.equal(adicionarDias("2028-02-28", 1), "2028-02-29");
});
test("Vendedor começa ativo e a demonstração não guarda senha", () => {
    const base = criarDemonstracao();
    const v = salvarCadastro(base, "vendedores", { ...vendedor(), ativo: false });
    assert.equal(v.ativo, true);
    assert.ok(!Object.hasOwn(v, "senha"));
    assert.ok(!JSON.stringify(base).includes("Teste123!"));
});
test("Documentos e e-mails não podem duplicar com outra formatação", () => {
    const base = criarDemonstracao();
    salvarCadastro(base, "vendedores", vendedor());
    assert.throws(() => salvarCadastro(base, "vendedores", { ...vendedor(), documento: "123.456.789-01", email: "outro@exemplo.test" }), /documento/);
    assert.throws(() => salvarCadastro(base, "vendedores", { ...vendedor(), documento: "98765432100", email: "TESTE@EXEMPLO.TEST" }), /e-mail/);
});
test("Desligar vendedor conserva carteira e autoria dos pedidos", () => {
    const base = criarDemonstracao();
    const clientes = structuredClone(base.clientes);
    const pedidos = structuredClone(base.pedidosGerais);
    alterarAtividade(base, 1, false, 1);
    assert.equal(base.vendedores[0].ativo, false);
    assert.deepEqual(base.clientes, clientes);
    assert.deepEqual(base.pedidosGerais, pedidos);
    alterarAtividade(base, 1, true, 2);
    assert.equal(base.vendedores[0].ativo, true);
});
test("Novo cliente exige vendedor ativo; edição mantém vínculo inativo atual", () => {
    const base = criarDemonstracao();
    const entrada = { ...vendedor(), vendedorId: 3, ativo: true };
    assert.throws(() => salvarCadastro(base, "clientes", entrada), /vendedor ativo/);
    alterarAtividade(base, 1, false, 1);
    const c = base.clientes[0];
    const salvo = salvarCadastro(base, "clientes", { ...c, nome: "Nome atualizado" }, c.id);
    assert.equal(salvo.vendedorId, 1);
    assert.throws(() => salvarCadastro(base, "clientes", { ...salvo, vendedorId: 3 }, salvo.id), /vendedor ativo/);
});
test("UF é única e cidades precisam de um estado válido", () => {
    const base = criarDemonstracao();
    assert.throws(() => salvarCadastro(base, "estados", { nome: "Duplicado", uf: "pr" }), /UF/);
    assert.throws(() => salvarCadastro(base, "cidades", { nome: "Cidade", estadoId: 999 }));
    assert.throws(() => salvarCadastro(base, "cidades", { nome: "FOZ DO IGUAÇU", estadoId: 1 }), /cidade/);
    const cidade = salvarCadastro(base, "cidades", { nome: "Foz do Iguaçu", estadoId: 2 });
    assert.equal(cidade.estadoId, 2);
});
test("Cadastro rejeita categoria inexistente, preço zero e estoque fracionário", () => {
    const base = criarDemonstracao();
    const p = { nome: "Produto novo", categoriaId: 1, precoCentavos: 100, estoque: 3, descricao: "" };
    for (const alteracao of [{ categoriaId: 999 }, { precoCentavos: 0 }, { estoque: 1.5 }, { estoque: -1 }])
        assert.throws(() => salvarCadastro(base, "produtos", { ...p, ...alteracao }));
    assert.equal(salvarCadastro(base, "produtos", p).estoque, 3);
});
test("Histórico impede exclusão física de produtos utilizados", () => {
    const base = criarDemonstracao();
    assert.equal(excluirProduto(base, 1, base.produtos[0].versao).desativado, true);
    assert.ok(base.produtos.find(p => p.id === 1));
    const novo = salvarCadastro(base, "produtos", { nome: "Novo", categoriaId: 1, precoCentavos: 100, estoque: 0 });
    assert.equal(excluirProduto(base, novo.id, novo.versao).desativado, false);
    assert.ok(!base.produtos.find(p => p.id === novo.id));
});
test("Formulário antigo não sobrescreve estoque alterado por confirmação", () => {
    const base = criarDemonstracao();
    const antigo = structuredClone(base.produtos[0]);
    confirmar(base);
    assert.throws(() => salvarCadastro(base, "produtos", antigo, antigo.id), Conflito);
    assert.equal(base.produtos[0].estoque, 0);
});
test("Estado serializado conserva pagamentos independentes e histórico", () => {
    const base = criarDemonstracao();
    confirmar(base);
    const restaurado = JSON.parse(JSON.stringify(base));
    assert.deepEqual(restaurado, base);
    assert.deepEqual(lote(restaurado).pedidos[0].parcelas.map(p => p.formaPagamentoNome), ["Pix", "Boleto bancário", "Dinheiro em espécie"]);
});
