import { CONFIG } from "./config.js";
import { exigirLogin, requisicao } from "./api.js";
import { demo, CHAVE_DEMO } from "./demo-store.js";
import { novoIdentificador } from "./regras.js";
import { observarTempoReal } from "./tempo-real.js";

export const CHAVE_ATUALIZACAO = `vandercake.web.${CONFIG.modo}.atualizacao.v1`;
const EVENTO = "vandercake:atualizacao";
function notificar() {
    localStorage.setItem(CHAVE_ATUALIZACAO, novoIdentificador());
    window.dispatchEvent(new Event(EVENTO));
}
async function executar(funcaoDemo, caminho, metodo = "GET", corpo) {
    if (!exigirLogin()) throw new Error("Entre na administração.");
    const resultado = CONFIG.modo === "demo" ? await funcaoDemo() : await requisicao(caminho, metodo, corpo);
    if (metodo !== "GET") notificar();
    return resultado;
}
export const repositorio = {
    obterDados: () => executar(() => demo.obterDados(), "/admin/painel"),
    obterLote: id => executar(() => demo.obterLote(id), `/admin/pedidos-gerais/${id}`),
    obterPrevia: id => executar(() => demo.obterPrevia(id), `/admin/pedidos-gerais/${id}/previa`),
    salvar: (tipo, valores, id = null) => executar(() => demo.salvar(tipo, valores, id),
        `/admin/${tipo}${id === null ? "" : `/${id}`}`, id === null ? "POST" : "PUT", valores),
    atividadeVendedor: (id, ativo, versao) => executar(() => demo.atividadeVendedor(id, ativo, versao),
        `/admin/vendedores/${id}/atividade`, "PATCH", { ativo, versao }),
    excluirProduto: (id, versao) => executar(() => demo.excluirProduto(id, versao),
        `/admin/produtos/${id}`, "DELETE", { versao }),
    confirmar: (id, valores) => executar(() => demo.confirmar(id, valores),
        `/admin/pedidos-gerais/${id}/confirmacao`, "POST", valores),
    receberParcela: id => executar(() => { throw new Error("Use a API para registrar recebimentos."); },
        `/admin/contas-receber/${id}/recebimento`, "POST", {}),
    cancelar: (id, valores) => executar(() => demo.cancelar(id, valores),
        `/admin/pedidos-gerais/${id}/cancelamento`, "POST", valores)
};

export function observarDados(callback, periodico = false) {
    let timer;
    const atualizar = () => {
        clearTimeout(timer);
        timer = setTimeout(() => { if (!document.hidden) callback(); }, 150);
    };
    window.addEventListener(EVENTO, atualizar);
    window.addEventListener("storage", e => {
        if (e.key === CHAVE_ATUALIZACAO || (CONFIG.modo === "demo" && e.key === CHAVE_DEMO)) atualizar();
    });
    window.addEventListener("focus", atualizar);
    document.addEventListener("visibilitychange", atualizar);
    observarTempoReal(atualizar);
    if (CONFIG.modo === "api") setInterval(atualizar, CONFIG.atualizacaoMs);
}
