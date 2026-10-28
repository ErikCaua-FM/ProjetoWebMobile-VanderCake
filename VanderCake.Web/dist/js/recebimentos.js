import { exigirLogin } from "./api.js";
import { CONFIG } from "./config.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, dinheiro, dataCurta, escapar, normalizar, mostrarMensagem, criarConfirmacao, abrirJanela } from "./comum.js";

let dados;
let linhas = [];
let versao = 0;
let ocupado = false;
function desenhar() {
    const termo = normalizar($("#pesquisa").value);
    const de = $("#de").value;
    const ate = $("#ate").value;
    const situacao = $("#situacao").value;
    const visiveis = linhas.filter(c => (!situacao || c.status === situacao) && (!de || c.dataVencimento >= de) &&
        (!ate || c.dataVencimento <= ate) && normalizar(`${c.clienteNome} ${c.documento} ${c.vendedorNome} ${c.pedidoId}`).includes(termo));
    $("#total").textContent = visiveis.length;
    $("#total-valor").textContent = `Total filtrado: ${dinheiro(visiveis.reduce((s, c) => s + c.valorCentavos, 0))}`;
    $("#vazio").hidden = visiveis.length > 0;
    $("#parcelas").innerHTML = visiveis.map(c => `<tr>
        <td><strong>${escapar(c.clienteNome)}</strong><span class="texto-secundario">${escapar(c.vendedorNome)}</span></td>
        <td><button class="link" data-detalhe="${c.id}">Pedido #${c.pedidoId}</button><span class="texto-secundario">${c.numeroParcela}ª parcela</span></td>
        <td>${dataCurta(c.dataVencimento)}</td><td>${escapar(c.forma)}</td><td class="coluna-valor">${dinheiro(c.valorCentavos)}</td>
        <td><span class="badge ${c.status === "Paga" ? "verde" : "amarelo"}">${c.status === "Paga" ? "Recebida" : "A receber"}</span>${c.dataPagamento ? `<span class="texto-secundario">${dataCurta(c.dataPagamento)}</span>` : ""}</td>
        <td>${c.status === "Pendente" ? `<button class="botao secundario pequeno" data-receber="${c.id}" ${ocupado ? "disabled" : ""}>Registrar recebimento</button>` : ""}</td>
        </tr>`).join("");
}
async function carregar() {
    const atual = ++versao;
    try {
        const resposta = await repositorio.obterDados();
        if (atual !== versao) return;
        dados = resposta;
        const pedidos = new Map(dados.pedidosGerais.flatMap(l => l.pedidos.map(p => [p.id, { ...p, loteId: l.id, vendedorNome: l.vendedorNome }])));
        linhas = dados.contasReceber.map(c => {
            const p = pedidos.get(c.pedidoId);
            return { ...c, loteId: p.loteId, clienteNome: p.clienteNome, vendedorNome: p.vendedorNome,
                documento: dados.clientes.find(x => x.id === p.clienteId)?.documento ?? "",
                forma: dados.formasPagamento.find(f => f.id === c.formaPagamentoId)?.descricao ?? "" };
        }).sort((a, b) => a.dataVencimento.localeCompare(b.dataVencimento) || a.id - b.id);
        desenhar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    if (CONFIG.modo !== "api") mostrarMensagem("Esta tela usa os recebimentos registrados na API da loja.", true);
    else {
        for (const id of ["pesquisa", "de", "ate", "situacao"]) $(`#${id}`).addEventListener("input", desenhar);
        $("#atualizar").addEventListener("click", carregar);
        $("#parcelas").addEventListener("click", async evento => {
            const detalhe = evento.target.closest("[data-detalhe]");
            if (detalhe) {
                const contaDetalhe = linhas.find(c => c.id === Number(detalhe.dataset.detalhe));
                if (contaDetalhe) abrirJanela(`pedido.html?lote=${contaDetalhe.loteId}&pedido=${contaDetalhe.pedidoId}`, `vandercake-pedido-${contaDetalhe.pedidoId}`);
                return;
            }
            const botao = evento.target.closest("[data-receber]");
            if (!botao || ocupado) return;
            const conta = linhas.find(c => c.id === Number(botao.dataset.receber));
            if (!conta || conta.status !== "Pendente") return;
            ocupado = true;
            desenhar();
            try {
                const confirmado = await criarConfirmacao({ titulo: "Registrar recebimento integral?",
                    descricao: `${conta.clienteNome}: ${dinheiro(conta.valorCentavos)}, ${conta.numeroParcela}ª parcela, ${conta.forma}. Confirme somente se o valor já foi recebido. A data registrada será a de hoje.`, confirmar: "Registrar recebimento" });
                if (!confirmado) return;
                await repositorio.receberParcela(conta.id);
                await carregar();
                mostrarMensagem("Recebimento integral registrado.");
            } catch (erro) { mostrarMensagem(erro.message, true); }
            finally { ocupado = false; desenhar(); }
        });
        observarDados(carregar);
        carregar();
    }
}
