import { criarDemonstracao } from "./demo-seed.js";
import { copiar, novoIdentificador, gerarPrevia, salvarCadastro, alterarAtividade, excluirProduto, confirmarLote, cancelarLote } from "./regras.js";

export const CHAVE_DEMO = "vandercake.web.demo.dados.v1";
let conexao;
function abrirBanco() {
    if (conexao) return conexao;
    conexao = new Promise((resolve, reject) => {
        const pedido = indexedDB.open("vandercake-web-demonstracao", 1);
        pedido.onupgradeneeded = () => pedido.result.createObjectStore("estado");
        pedido.onsuccess = () => resolve(pedido.result);
        pedido.onerror = () => { conexao = null; reject(new Error("Não foi possível abrir os dados locais da demonstração.")); };
    });
    return conexao;
}

async function transacao(acao, alterar = false, reiniciar = false) {
    const banco = await abrirBanco();
    return new Promise((resolve, reject) => {
        // IndexedDB serializa as escritas entre guias, inclusive em servidores HTTP de desenvolvimento.
        const tx = banco.transaction("estado", "readwrite");
        const store = tx.objectStore("estado");
        const leitura = store.get("atual");
        let resultado;
        let falha;
        leitura.onsuccess = () => {
            try {
                const base = reiniciar || !leitura.result ? criarDemonstracao() : leitura.result;
                if (base.schema !== 1 || !Array.isArray(base.pedidosGerais))
                    throw new Error("Dados locais incompatíveis. Reinicie a demonstração na tela de login.");
                resultado = acao(base);
                if (alterar || !leitura.result || reiniciar) {
                    if (alterar) base.revisao++;
                    store.put(base, "atual");
                }
            } catch (erro) { falha = erro; tx.abort(); }
        };
        tx.oncomplete = () => resolve(copiar(resultado));
        tx.onerror = tx.onabort = () => reject(falha ?? new Error("A alteração local não pôde ser salva. Os dados anteriores foram preservados."));
    });
}
const gravar = acao => transacao(acao, true);
export const iniciarDemo = () => transacao(base => base);
export async function reiniciarDemo() {
    await transacao(base => base, true, true);
    localStorage.setItem(CHAVE_DEMO, novoIdentificador());
}

export const demo = {
    obterDados: iniciarDemo,
    obterLote: async id => {
        const base = await iniciarDemo();
        const lote = base.pedidosGerais.find(l => l.id === id);
        if (!lote) throw new Error("Pedido Geral não encontrado.");
        return { lote, clientes: base.clientes, cidades: base.cidades, estados: base.estados,
            vendas: base.vendas, contasReceber: base.contasReceber };
    },
    obterPrevia: async id => {
        const base = await iniciarDemo();
        const lote = base.pedidosGerais.find(l => l.id === id);
        if (!lote) throw new Error("Pedido Geral não encontrado.");
        return gerarPrevia(lote, base.produtos);
    },
    salvar: (tipo, valores, id) => gravar(base => salvarCadastro(base, tipo, valores, id)),
    atividadeVendedor: (id, ativo, versao) => gravar(base => alterarAtividade(base, id, ativo, versao)),
    excluirProduto: (id, versao) => gravar(base => excluirProduto(base, id, versao)),
    confirmar: (id, valores) => gravar(base => confirmarLote(base, id, valores.tokenPrevia)),
    cancelar: (id, valores) => gravar(base => cancelarLote(base, id, valores.motivo, valores.versao))
};
