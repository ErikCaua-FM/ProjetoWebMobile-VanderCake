import { exigirLogin } from "./api.js";
import { dentroPeriodo } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, icone, dinheiro, dataHora, escapar, normalizar, iniciais, endereco, badge,
    abrirJanela, mostrarMensagem, criarConfirmacao } from "./comum.js";

let dados;
let versaoCarregamento = 0;
let ocupado = false;
const janelas = new Set();
const abrir = (url, nome) => { const janela = abrirJanela(url, nome); if (janela) janelas.add(janela); };
const busca = tipo => normalizar($(`#pesquisa-${tipo}`).value);
const pessoa = (nome, sub, cor = "") => `<div class="nome-registro"><span class="avatar ${cor}">${escapar(iniciais(nome))}</span><div><strong>${escapar(nome)}</strong><span class="texto-secundario">${escapar(sub)}</span></div></div>`;
const link = (acao, id, texto, classe = "") => `<button type="button" class="link ${classe}" data-acao="${acao}" data-id="${Number(id)}">${texto}</button>`;

function preencher(tipo, linhas, total) {
    $(`#lista-${tipo}`).innerHTML = linhas.join("");
    $(`#total-${tipo}`).textContent = total;
    $(`#vazio-${tipo}`).hidden = linhas.length > 0;
    $(`#resumo-${tipo}`).textContent = `${linhas.length} de ${total} registros`;
}
function desenhar() {
    if (!dados) return;
    const pendentes = dados.pedidosGerais.filter(l => l.status === "AguardandoConfirmacao");
    const vendas = dados.pedidosGerais.flatMap(l => l.pedidos.filter(p => p.status !== "Pendente").map(p => ({
        ...p, lote: l, venda: dados.vendas.find(v => v.pedidoId === p.id),
        cliente: dados.clientes.find(c => c.id === p.clienteId)
    }))).sort((a, b) => new Date(b.lote.dataHoraProcessamento) - new Date(a.lote.dataHoraProcessamento));
    $("#total-pendentes").textContent = pendentes.length;
    const indicadores = [
        ["bag", "Vendas confirmadas", dados.vendas.length, dinheiro(dados.vendas.reduce((s, v) => s + v.valorTotalCentavos, 0)) + " em vendas"],
        ["clock", "Pedidos pendentes", pendentes.length, "envios para conferir"],
        ["box", "Produtos ativos", dados.produtos.filter(p => p.ativo).length, "disponíveis no catálogo"],
        ["users", "Vendedores ativos", dados.vendedores.filter(v => v.ativo).length, "cuidando da sua carteira"]
    ];
    $("#estatisticas").innerHTML = indicadores.map(([i, titulo, total, detalhe]) => `<div class="estatistica"><span class="icone-circulo">${icone(i)}</span><div><p>${titulo}</p><strong>${total}</strong><small>${escapar(detalhe)}</small></div></div>`).join("");
    const filtro = $("#filtro-vendas").value;
    const listaVendas = vendas.filter(p => dentroPeriodo(p.lote.dataHoraRecebimento, $("#vendas-de").value, $("#vendas-ate").value) && (!filtro || (filtro === "Confirmado" ? p.status.startsWith("Confirmado") : p.status === filtro)) &&
        normalizar(`${p.id} ${p.venda?.id ?? ""} ${p.lote.id} ${p.clienteNome} ${p.lote.vendedorNome} ${p.status} ${p.status.startsWith("Confirmado") ? "Confirmada" : p.status === "Cancelado" ? "Cancelada" : "Não atendida"} ${dataHora(p.dataHoraCriacao)} ${dinheiro(p.venda?.valorTotalCentavos ?? p.valorTotalSolicitadoCentavos)} ${p.cliente?.documento} ${p.cliente?.email} ${endereco(p.cliente, dados)}`).includes(busca("vendas")));
    preencher("vendas", listaVendas.map(p => `<tr><td class="coluna-valor"><strong>#${Number(p.id)}</strong><span class="texto-secundario">${p.venda ? `Venda #${Number(p.venda.id)}` : "Sem venda gerada"}</span></td>
        <td><strong>${escapar(p.clienteNome)}</strong><span class="texto-secundario">${escapar(p.lote.vendedorNome)}</span></td>
        <td class="coluna-valor">${dinheiro(p.venda?.valorTotalCentavos ?? (p.status === "NaoAtendido" ? 0 : p.valorTotalSolicitadoCentavos))}${p.status === "Cancelado" ? '<span class="texto-secundario">Solicitado</span>' : ""}</td>
        <td>${badge(p.status)}</td><td><button type="button" class="link" data-detalhe="${Number(p.lote.id)}" data-pedido="${Number(p.id)}">Ver detalhes</button></td></tr>`), vendas.length);

    const produtos = [...dados.produtos].sort((a, b) => Number(b.ativo) - Number(a.ativo) || a.nome.localeCompare(b.nome, "pt-BR"))
        .map(p => ({ ...p, categoria: dados.categorias.find(c => c.id === p.categoriaId)?.nome ?? "" }));
    preencher("produtos", produtos.filter(p => normalizar(`${p.id} ${p.nome} ${p.categoria} ${p.descricao} ${dinheiro(p.precoCentavos)} ${p.estoque}`).includes(busca("produtos")))
        .map(p => `<tr><td><strong>${escapar(p.nome)}</strong><span class="texto-secundario">${escapar(p.categoria)}${p.ativo ? "" : " · Inativo"}</span></td><td class="coluna-valor">${dinheiro(p.precoCentavos)}</td>
        <td class="coluna-valor ${p.estoque < 10 ? "estoque-baixo" : ""}">${p.estoque} un.</td><td><div class="acoes">${link("produto", p.id, "Alterar")}${p.ativo ? link("excluir", p.id, "Excluir", "discreto divisoria") : ""}</div></td></tr>`), produtos.length);

    preencher("vendedores", dados.vendedores.filter(v => normalizar(`${v.id} ${v.nome} ${v.documento} ${v.email} ${v.telefone} ${v.ativo ? "Ativo" : "Inativo"} ${endereco(v, dados)}`).includes(busca("vendedores")))
        .map(v => `<tr><td>${pessoa(v.nome, `#${v.id} · ${v.email}`, v.ativo ? "" : "neutro")}</td><td>${badge(v.ativo ? "Ativo" : "Inativo")}</td><td><div class="acoes">${link("vendedor", v.id, "Alterar")}${link("atividade", v.id, v.ativo ? "Desligar" : "Reativar", "discreto divisoria")}</div></td></tr>`), dados.vendedores.length);

    preencher("clientes", dados.clientes.map(c => ({ ...c, vendedor: dados.vendedores.find(v => v.id === c.vendedorId)?.nome ?? "" }))
        .filter(c => normalizar(`${c.id} ${c.nome} ${c.documento} ${c.email} ${c.telefone} ${endereco(c, dados)} ${c.vendedor}`).includes(busca("clientes")))
        .map(c => `<tr><td><strong>${escapar(c.nome)}${c.ativo ? "" : " · Inativo"}</strong><span class="texto-secundario">${escapar(endereco(c, dados))}</span></td><td>${escapar(c.vendedor)}</td><td>${link("cliente", c.id, "Alterar")}</td></tr>`), dados.clientes.length);
}
async function carregar() {
    const versao = ++versaoCarregamento;
    $("#atualizar").disabled = true;
    try {
        const novos = await repositorio.obterDados();
        if (versao !== versaoCarregamento) return;
        dados = novos;
        desenhar();
        $("#atualizado-em").textContent = `Atualizado em ${dataHora(new Date().toISOString())}`;
    } catch (erro) { if (versao === versaoCarregamento) mostrarMensagem(erro.message, true); }
    finally { if (versao === versaoCarregamento) $("#atualizar").disabled = false; }
}
async function acaoTabela(evento) {
    const detalhes = evento.target.closest("[data-detalhe]");
    if (detalhes) { abrir(`pedido.html?lote=${Number(detalhes.dataset.detalhe)}&pedido=${Number(detalhes.dataset.pedido)}`, `vandercake-pedido-${detalhes.dataset.pedido}`); return; }
    const botao = evento.target.closest("button[data-acao]");
    if (!botao || !dados || ocupado) return;
    const id = Number(botao.dataset.id);
    const acao = botao.dataset.acao;
    if (["produto", "vendedor", "cliente"].includes(acao)) { abrir(`${acao}.html?id=${id}`, `vandercake-${acao}-${id}`); return; }
    ocupado = true;
    botao.disabled = true;
    try {
        if (acao === "atividade") {
            const vendedor = dados.vendedores.find(v => v.id === id);
            const resposta = await criarConfirmacao({ titulo: vendedor.ativo ? "Desligar vendedor?" : "Reativar vendedor?",
                descricao: vendedor.ativo ? `${vendedor.nome} ficará inativo. A carteira e o histórico serão preservados.` : `${vendedor.nome} voltará a atuar como vendedor.`,
                confirmar: vendedor.ativo ? "Desligar" : "Reativar", perigo: vendedor.ativo });
            if (!resposta) return;
            await repositorio.atividadeVendedor(id, !vendedor.ativo, vendedor.versao);
            mostrarMensagem(vendedor.ativo ? "Vendedor desligado." : "Vendedor reativado.");
        } else if (acao === "excluir") {
            const produto = dados.produtos.find(p => p.id === id);
            const resposta = await criarConfirmacao({ titulo: "Excluir produto?", descricao: `${produto.nome}. Produtos utilizados em pedidos serão inativados para preservar o histórico.`, confirmar: "Excluir", perigo: true });
            if (!resposta) return;
            const resultado = await repositorio.excluirProduto(id, produto.versao);
            mostrarMensagem(resultado.desativado ? "Produto inativado. O histórico foi preservado." : "Produto excluído.");
        }
        await carregar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
    finally { ocupado = false; botao.disabled = false; }
}
if (exigirLogin()) {
    instalarCabecalho();
    document.querySelectorAll('input[type="search"]').forEach(e => e.addEventListener("input", desenhar));
    $("#filtro-vendas").addEventListener("change", desenhar);
    for (const id of ["vendas-de", "vendas-ate"]) $(`#${id}`).addEventListener("input", desenhar);
    $("#atualizar").addEventListener("click", () => { mostrarMensagem(""); carregar(); });
    $("#abrir-pendentes").addEventListener("click", () => abrir("pendentes.html", "vandercake-pendentes"));
    document.querySelectorAll("[data-novo]").forEach(e => e.addEventListener("click", () => abrir(`${e.dataset.novo}.html`, `vandercake-${e.dataset.novo}-novo`)));
    $(".painel-grid").addEventListener("click", acaoTabela);
    window.addEventListener("message", e => {
        if (e.origin === location.origin && janelas.has(e.source) && e.data?.tipo === "vandercake:salvo") {
            mostrarMensagem(String(e.data.mensagem)); carregar();
        }
    });
    observarDados(carregar, true);
    carregar();
}
