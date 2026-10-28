import { exigirLogin } from "./api.js";
import { CONFIG } from "./config.js";
import { novoIdentificador } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, obterId, dinheiro, dataHora, dataCurta, escapar, badge, endereco, mostrarMensagem, criarConfirmacao } from "./comum.js";
let id;
let foco;
let dados;
let previa;
let ocupado = false;
let leitura = 0;

function desenhar() {
    const lote = dados.lote;
    const pendente = lote.status === "AguardandoConfirmacao";
    const cancelado = lote.status === "Cancelado";
    const pedidos = pendente ? previa.pedidos : lote.pedidos;
    $("#titulo-pagina").textContent = `Pedido Geral #${lote.id}`;
    document.title = `Pedido Geral #${lote.id} | VanderCake`;
    $("#subtitulo-pedido").textContent = `${lote.vendedorNome} · Recebido em ${dataHora(lote.dataHoraRecebimento)}`;
    $("#conteudo-pedido").hidden = false;
    $("#acoes-pedido").hidden = !pendente;
    $("#aviso-pedido").hidden = true;
    if (cancelado) {
        $("#aviso-pedido").hidden = false;
        $("#aviso-pedido").textContent = `Pedido cancelado: ${lote.motivoCancelamento}. Nenhuma venda ou baixa de estoque foi gerada.`;
    } else if (pendente) {
        $("#aviso-pedido").hidden = false;
        $("#aviso-pedido").textContent = previa.unidadesCortadas > 0 ?
            `Estoque insuficiente: a prévia distribui os produtos proporcionalmente entre os clientes, com ${previa.unidadesCortadas} unidade(s) de corte. Confira antes de confirmar.` :
            "O estoque atende este envio. A prévia não reserva produtos; as quantidades serão conferidas novamente na confirmação.";
    }
    $("#pedidos-clientes").innerHTML = pedidos.map(p => {
        const cliente = dados.clientes.find(c => c.id === p.clienteId);
        const venda = dados.vendas.find(v => v.pedidoId === p.id);
        return `<section class="card pedido-cliente ${foco === p.id ? "destaque" : ""}" id="pedido-${Number(p.id)}">
            <div class="card-cabecalho"><div><p class="sobretitulo">PEDIDO #${Number(p.id)}${venda ? ` · VENDA #${Number(venda.id)}` : ""}</p><h2>${escapar(p.clienteNome)}</h2></div>${badge(p.status)}</div>
            <div class="cliente-contato"><div>${escapar(cliente?.documento ?? "")} · ${escapar(cliente?.telefone ?? "")} · ${escapar(cliente?.email ?? "")}</div><div>${escapar(endereco(cliente, dados))}</div></div>
            <div class="tabela-container"><table><caption class="sr-only">Itens do pedido de ${escapar(p.clienteNome)}</caption><thead><tr><th>Produto</th><th class="numero">Solicitado</th><th class="numero">${pendente ? "Prévia" : "Atendido"}</th><th class="numero">Corte</th><th class="numero">Preço un.</th><th class="numero">Subtotal ${pendente ? "previsto" : "final"}</th></tr></thead><tbody>
            ${p.itens.map(i => `<tr><td><strong>${escapar(i.nomeProduto)}</strong></td><td class="numero">${i.quantidadeSolicitada}</td><td class="numero">${i.quantidadeAtendida ?? "—"}</td><td class="numero ${i.quantidadeAtendida !== null && i.quantidadeAtendida < i.quantidadeSolicitada ? "perigo-texto" : ""}">${i.quantidadeAtendida === null ? "—" : i.quantidadeSolicitada - i.quantidadeAtendida}</td><td class="numero coluna-valor">${dinheiro(i.precoUnitarioCentavos)}</td><td class="numero coluna-valor">${dinheiro(i.quantidadeAtendida === null ? null : i.quantidadeAtendida * i.precoUnitarioCentavos)}</td></tr>`).join("")}</tbody></table></div>
            <div class="pagamentos"><h3>Pagamento · ${escapar(p.condicaoPagamentoDescricao)}</h3><div class="parcelas-grid">${p.parcelas.map(parcela => {
                const conta = dados.contasReceber.find(c => c.pedidoId === p.id && c.numeroParcela === parcela.numeroParcela);
                return `<div class="parcela"><p>${parcela.numeroParcela}ª parcela · ${parcela.diasAposConfirmacao === 0 ? "À vista" : `${parcela.diasAposConfirmacao} dias`}</p><strong>${escapar(parcela.formaPagamentoNome)}</strong><p>${pendente ? "Prévia" : cancelado ? "Solicitado" : "Valor final"}: <b>${dinheiro(cancelado ? parcela.valorPrevistoCentavos : parcela.valorFinalCentavos)}</b></p><small>Original: ${dinheiro(parcela.valorPrevistoCentavos)}</small><br><small>${cancelado ? "Cancelada · sem cobrança" : `Vencimento: ${parcela.dataVencimento ? dataCurta(parcela.dataVencimento) : p.status === "NaoAtendido" ? "Sem cobrança" : "após confirmação"}`}</small>${conta ? `<br><small>${conta.status === "Paga" ? "Recebida" : "A receber"}</small>` : ""}</div>`;
            }).join("")}</div></div>
            <div class="pedido-totais"><div><span>Total solicitado</span>${dinheiro(p.valorTotalSolicitadoCentavos)}</div><div><span>${pendente ? "Total da prévia" : "Total confirmado"}</span><strong>${dinheiro(p.valorTotalAtendidoCentavos)}</strong></div></div>
        </section>`;
    }).join("");
    const solicitado = pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0);
    const final = cancelado ? null : pedidos.reduce((s, p) => s + (p.valorTotalAtendidoCentavos ?? 0), 0);
    $("#resumo-pedido").innerHTML = `<h2>Resumo do envio</h2>${badge(lote.status)}<dl><dt>Vendedor</dt><dd>${escapar(lote.vendedorNome)}</dd><dt>Clientes neste envio</dt><dd>${pedidos.length}</dd><dt>Recebido em</dt><dd>${dataHora(lote.dataHoraRecebimento)}</dd><dt>Total solicitado</dt><dd>${dinheiro(solicitado)}</dd>
        ${!pendente ? `<dt>${cancelado ? "Cancelado" : "Confirmado"} em</dt><dd>${dataHora(lote.dataHoraProcessamento)}</dd>` : ""}</dl><div class="resumo-total"><p>${pendente ? "Total da prévia" : "Total confirmado"}</p><strong>${dinheiro(final)}</strong><small>${pendente ? "As formas de pagamento de cada parcela serão mantidas após o corte." : cancelado ? "Cancelamento registrado sem movimentar estoque." : "Valores de atendimento registrados pela loja."}</small></div>`;
}
async function carregar(forcar = false) {
    if (ocupado && !forcar) return;
    const versao = ++leitura;
    $("#atualizar").disabled = true;
    $("#confirmar").disabled = true;
    $("#cancelar").disabled = true;
    try {
        const novos = await repositorio.obterLote(id);
        if (foco && !novos.lote.pedidos.some(p => p.id === foco)) throw new Error("Este pedido não pertence ao envio informado.");
        const novaPrevia = novos.lote.status === "AguardandoConfirmacao" ? await repositorio.obterPrevia(id) : null;
        if (versao !== leitura) return;
        dados = novos;
        previa = novaPrevia;
        desenhar();
        $("#confirmar").disabled = ocupado;
        $("#cancelar").disabled = ocupado;
    } catch (erro) { if (versao === leitura) mostrarMensagem(erro.message, true); }
    finally { if (versao === leitura) $("#atualizar").disabled = ocupado; }
}
function identificadorOperacao(acao, conteudo) {
    const chave = `vandercake.web.${CONFIG.modo}.operacao.${acao}.${id}`;
    const assinatura = JSON.stringify(conteudo);
    let anterior;
    try { anterior = JSON.parse(sessionStorage.getItem(chave) ?? "null"); } catch { anterior = null; }
    if (anterior?.assinatura === assinatura) return anterior.id;
    const registro = { assinatura, id: novoIdentificador() };
    sessionStorage.setItem(chave, JSON.stringify(registro));
    return registro.id;
}
async function processar(acao) {
    if (ocupado || !dados || dados.lote.status !== "AguardandoConfirmacao") return;
    ocupado = true;
    $("#confirmar").disabled = true;
    $("#cancelar").disabled = true;
    $("#atualizar").disabled = true;
    try {
        const confirmar = acao === "confirmar";
        const resposta = await criarConfirmacao(confirmar ? {
            titulo: "Confirmar este Pedido Geral?", descricao: `${dados.lote.pedidos.length} pedido(s), total de ${dinheiro(previa.totalAtendidoCentavos)} e ${previa.unidadesCortadas} unidade(s) de corte. O estoque será baixado pelas quantidades atendidas.`, confirmar: "Confirmar envio"
        } : { titulo: "Cancelar este Pedido Geral?", descricao: "Todos os pedidos deste envio serão cancelados. O estoque será mantido.", confirmar: "Cancelar pedidos", perigo: true, motivo: true });
        if (!resposta) return;
        const corpo = confirmar ? { tokenPrevia: previa.tokenPrevia } : { motivo: resposta.motivo, versao: dados.lote.versao };
        corpo.identificadorOperacao = identificadorOperacao(acao, corpo);
        await (confirmar ? repositorio.confirmar(id, corpo) : repositorio.cancelar(id, corpo));
        mostrarMensagem(confirmar ? "Envio confirmado. Os resultados já aparecem na tela de Vendas." : "Envio cancelado. O histórico foi atualizado.");
        await carregar(true);
    } catch (erro) {
        await carregar(true);
        mostrarMensagem(erro.message + (erro.status === 409 ? " Revise os dados atualizados antes de continuar." : ""), true);
    } finally {
        ocupado = false;
        $("#atualizar").disabled = false;
        $("#confirmar").disabled = !previa;
        $("#cancelar").disabled = !dados || dados.lote.status !== "AguardandoConfirmacao";
    }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    try {
        id = obterId("lote");
        foco = obterId("pedido", false);
        $("#atualizar").addEventListener("click", () => carregar());
        $("#confirmar").addEventListener("click", () => processar("confirmar"));
        $("#cancelar").addEventListener("click", () => processar("cancelar"));
        observarDados(() => carregar(), true);
        carregar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
