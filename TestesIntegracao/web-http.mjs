// Executado pelo runner C# com a API real. Stubs somente para armazenamento/eventos do navegador.
import assert from "node:assert/strict";
import { spawn } from "node:child_process";
import { fileURLToPath } from "node:url";
const origemApi = process.env.VANDERCAKE_TEST_URL;
if (!origemApi) throw new Error("Execute pelo TestesIntegracao, com VANDERCAKE_TESTAR_WEB_HTTP=1.");
// Usa tambem o servidor opcional da Web para verificar o proxy HTTP e WebSocket.
const proxy = spawn(process.execPath, ["scripts/serve.mjs", "--port", "0"], {
    cwd: fileURLToPath(new URL("../VanderCake.Web", import.meta.url)),
    env: { ...process.env, VANDERCAKE_API_URL: origemApi }, stdio: ["ignore", "pipe", "inherit"]
});
process.on("exit", () => proxy.kill());
const origem = await new Promise((resolve, reject) => {
    const timer = setTimeout(() => reject(new Error("Servidor Web nao iniciou")), 10000);
    proxy.stdout.on("data", parte => {
        const porta = parte.toString().match(/localhost:(\d+)/)?.[1];
        if (porta) { clearTimeout(timer); resolve(`http://127.0.0.1:${porta}`); }
    });
    proxy.on("error", reject);
});
const eventos = new EventTarget();
globalThis.self = globalThis;
globalThis.window = eventos;
let destino = "";
globalThis.location = { origin: origem, href: origem + "/index.html", replace: url => { destino = url; } };
globalThis.document = { hidden: false, querySelectorAll: () => [], addEventListener: () => {}, removeEventListener: () => {} };
window.document = document;
window.location = location;
// Exercita o bundle de navegador usando fetch/WebSocket nativos do Node, sem instalar outro cliente SignalR.
Object.defineProperty(process, "release", { value: { ...process.release, name: "teste-web" }, configurable: true });
function armazenamento() {
    const itens = new Map();
    return { getItem: k => itens.get(k) ?? null, setItem: (k, v) => itens.set(k, String(v)), removeItem: k => itens.delete(k) };
}
globalThis.sessionStorage = armazenamento();
globalThis.localStorage = armazenamento();
const api = await import("../VanderCake.Web/dist/js/api.js");
const { repositorio } = await import("../VanderCake.Web/dist/js/dados.js");
const { observarTempoReal } = await import("../VanderCake.Web/dist/js/tempo-real.js");
let testes = 0;
function ok(condicao, descricao) { assert.ok(condicao, descricao); testes++; console.log("OK WEB HTTP: " + descricao); }
try {
    const pagina = await fetch(origem);
    ok(pagina.ok && (await pagina.text()).includes("VanderCake"), "Servidor Web entrega a pagina HTML do projeto");
    await api.entrar("admin@teste.local", "Teste-integracao-2026!");
    ok(api.conectado() && api.sessao().refreshToken, "Login guarda acesso e renovacao no contrato esperado");
    const dados = await repositorio.obterDados();
    ok(dados.produtos.length > 0 && Number.isSafeInteger(dados.produtos[0].precoCentavos), "Painel real retorna moeda em centavos");
    const lote = dados.pedidosGerais.find(l => l.status === "AguardandoConfirmacao");
    ok(lote, "Pendentes da API sao encontrados pela Web");
    const detalhes = await repositorio.obterLote(lote.id);
    const previa = await repositorio.obterPrevia(lote.id);
    ok(detalhes.lote.id === lote.id && previa.tokenPrevia && previa.pedidos[0].parcelas.length === 3, "Detalhes e previa usam os campos consumidos pela pagina");
    const sessao = api.sessao();
    sessionStorage.setItem("vandercake.web.api.sessao.v1", JSON.stringify({ ...sessao, token: "expirado" }));
    await repositorio.obterDados();
    ok(api.sessao().token !== "expirado", "401 renova o acesso e repete a consulta");
    let avisos = 0;
    observarTempoReal(() => avisos++);
    async function aguardar(predicado) {
        for (let i = 0; i < 80 && !predicado(); i++) await new Promise(r => setTimeout(r, 100));
        assert.ok(predicado(), "Aviso SignalR nao recebido no prazo de teste");
    }
    await aguardar(() => avisos > 0);
    const anterior = avisos;
    const categoria = await repositorio.salvar("categorias", { nome: "Categoria HTTP " + Date.now() });
    await aguardar(() => avisos > anterior);
    ok((await repositorio.obterDados()).categorias.some(c => c.id === categoria.id), "Cliente SignalR JavaScript recebe aviso e consulta cadastro confirmado");
    let rejeitou = false;
    try { await repositorio.salvar("cidades", { nome: "Invalida", estadoId: -1 }); }
    catch (erro) { rejeitou = erro.status === 422 && erro.message.length > 0; }
    ok(rejeitou, "Mensagem de validacao da API chega ao frontend");
    const condicao = await repositorio.salvar("condicoes-pagamento", { descricao: "Prazo Web", dias: [7, 14], ativo: true });
    const forma = await repositorio.salvar("formas-pagamento", { descricao: "Meio Web", ativo: true });
    await repositorio.salvar("formas-pagamento", { descricao: "Meio Web revisado", ativo: false, versao: 1 }, forma.id);
    const atualizados = await repositorio.obterDados();
    ok(atualizados.condicoesPagamento.some(c => c.id === condicao.id && c.dias.join("/") === "7/14") &&
        atualizados.formasPagamento.some(f => f.id === forma.id && f.descricao === "Meio Web revisado" && !f.ativo && f.versao === 2),
        "Cadastros auxiliares salvam condicoes e formas usando o contrato da Web");
    const conta = atualizados.contasReceber.find(c => c.status === "Pendente");
    const baixa = await repositorio.receberParcela(conta.id);
    const aposBaixa = await repositorio.obterDados();
    const contaPaga = aposBaixa.contasReceber.find(c => c.id === conta.id);
    const pedido = aposBaixa.pedidosGerais.flatMap(l => l.pedidos).find(p => p.id === conta.pedidoId);
    ok(baixa.status === "Paga" && contaPaga.dataPagamento && contaPaga.valorCentavos === conta.valorCentavos &&
        pedido.parcelas.some(p => p.numeroParcela === conta.numeroParcela && p.situacao === "Paga" && p.dataPagamento === contaPaga.dataPagamento),
        "Recebimento da Web atualiza conta e detalhes do pedido conservando o valor");
    const paginasExtras = await Promise.all(["recebimentos.html", "pagamentos.html"].map(async caminho => {
        const resposta = await fetch(origem + "/" + caminho);
        return resposta.ok && (await resposta.text()).includes('type="module"');
    }));
    ok(paginasExtras.every(Boolean), "Paginas de recebimentos e cadastros auxiliares sao servidas com seus modulos");
    api.sair();
    ok(!api.conectado() && destino === "login.html", "Sair remove a sessao e redireciona para login");
    console.log(`${testes} verificacoes do cliente JavaScript concluidas.`);
    window.dispatchEvent(new Event("pagehide"));
    process.exit(0);
} catch (erro) { console.error(erro); process.exit(1); }
