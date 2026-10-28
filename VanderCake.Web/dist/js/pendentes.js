import { exigirLogin } from "./api.js";
import { dentroPeriodo } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, escapar, normalizar, dinheiro, dataHora, badge, abrirJanela, mostrarMensagem, iniciais } from "./comum.js";
let lotes = [];
let versao = 0;
function desenhar() {
    const busca = normalizar($("#pesquisa").value);
    const lista = lotes.filter(l => dentroPeriodo(l.dataHoraRecebimento, $("#periodo-de").value, $("#periodo-ate").value) && normalizar(`${l.id} ${l.vendedorNome} ${dataHora(l.dataHoraRecebimento)} ${l.pedidos.map(p => p.clienteNome).join(" ")}`).includes(busca));
    $("#total").textContent = lotes.length;
    $("#vazio").hidden = lista.length > 0;
    $("#lista-pendentes").innerHTML = lista.map(l => `<tr><td><strong>#${Number(l.id)}</strong><span class="texto-secundario">${dataHora(l.dataHoraRecebimento)}</span></td><td><div class="nome-registro"><span class="avatar">${escapar(iniciais(l.vendedorNome))}</span><strong>${escapar(l.vendedorNome)}</strong></div></td>
        <td><strong>${l.pedidos.length} ${l.pedidos.length === 1 ? "cliente" : "clientes"}</strong><span class="texto-secundario">${escapar(l.pedidos.map(p => p.clienteNome).join(" · "))}</span></td>
        <td class="coluna-valor">${dinheiro(l.pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0))}</td><td>${badge(l.status)}</td><td><button class="botao secundario pequeno" data-lote="${Number(l.id)}">Ver detalhes →</button></td></tr>`).join("");
}
async function carregar() {
    const atual = ++versao;
    $("#atualizar").disabled = true;
    try {
        const dados = await repositorio.obterDados();
        if (atual !== versao) return;
        lotes = dados.pedidosGerais.filter(l => l.status === "AguardandoConfirmacao").sort((a, b) => new Date(a.dataHoraRecebimento) - new Date(b.dataHoraRecebimento));
        desenhar();
        $("#atualizado-em").textContent = `Atualizado em ${dataHora(new Date().toISOString())}`;
    } catch (erro) { mostrarMensagem(erro.message, true); }
    finally { if (atual === versao) $("#atualizar").disabled = false; }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    $("#pesquisa").addEventListener("input", desenhar);
    for (const id of ["periodo-de", "periodo-ate"]) $(`#${id}`).addEventListener("input", desenhar);
    $("#atualizar").addEventListener("click", carregar);
    $("#lista-pendentes").addEventListener("click", e => {
        const botao = e.target.closest("[data-lote]");
        if (botao) abrirJanela(`pedido.html?lote=${Number(botao.dataset.lote)}`, `vandercake-lote-${botao.dataset.lote}`);
    });
    observarDados(carregar, true);
    carregar();
}
