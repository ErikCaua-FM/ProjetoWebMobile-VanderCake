import { CONFIG } from "./config.js";
import { sessao, sair } from "./api.js";
export { normalizar, lerCentavos } from "./regras.js";
export const $ = seletor => document.querySelector(seletor);
export const dinheiro = centavos => centavos === null || centavos === undefined ? "—" :
    (centavos / 100).toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
export const dataHora = valor => valor ? new Date(valor).toLocaleString("pt-BR", {
    dateStyle: "short", timeStyle: "short", timeZone: "America/Sao_Paulo"
}) : "—";
export const dataCurta = valor => valor ? new Date(`${valor}T12:00:00Z`).toLocaleDateString("pt-BR", { timeZone: "UTC" }) : "Após confirmação";
export const escapar = valor => String(valor ?? "").replace(/[&<>"']/g,
    c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" })[c]);
export const iniciais = nome => String(nome).split(/\s+/).filter(Boolean).slice(0, 2).map(p => p[0]).join("").toUpperCase();
const caminhos = {
    cake: '<path d="M3 9l9-6 9 6v11H3zM3 13c3-3 5 3 9 0s6 0 9-1M3 17h18M12 3V1"/>',
    grid: '<rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/>',
    bag: '<path d="M5 7h14l1 14H4L5 7zM9 8V6a3 3 0 016 0v2"/>',
    box: '<path d="M3 7l9-5 9 5v10l-9 5-9-5V7zM3 7l9 5 9-5M12 12v10M7.5 4.5l9 5"/>',
    users: '<circle cx="9" cy="7" r="3"/><path d="M3 21v-3a6 6 0 0112 0v3M16 4a3 3 0 010 6M18 14a5 5 0 013 4v3"/>',
    store: '<path d="M3 10l2-7h14l2 7M4 12v9h16v-9M9 21v-7h6v7M3 10c0 3 4 3 4 0 0 3 5 3 5 0 0 3 5 3 5 0 0 3 4 3 4 0"/>',
    clock: '<circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/>',
    search: '<circle cx="10.5" cy="10.5" r="6.5"/><path d="M16 16l5 5"/>',
    plus: '<path d="M12 5v14M5 12h14"/>',
    arrow: '<path d="M5 12h14M14 7l5 5-5 5"/>',
    back: '<path d="M19 12H5M10 7l-5 5 5 5"/>',
    refresh: '<path d="M20 7a9 9 0 00-15-2L3 7M3 3v4h4M4 17a9 9 0 0015 2l2-2M21 21v-4h-4"/>',
    check: '<path d="M5 12l4 4L19 6"/>',
    logout: '<path d="M9 3H3v18h6M9 12h12M17 8l4 4-4 4"/>',
    edit: '<path d="M16 3l5 5L9 20l-6 1 1-6L16 3zM13 6l5 5"/>',
    shield: '<path d="M12 2l8 3v7c0 5-8 10-8 10S4 17 4 12V5l8-3zM8 12l3 3 5-6"/>',
    chevron: '<path d="M9 5l7 7-7 7"/>',
    x: '<path d="M6 6l12 12M18 6L6 18"/>'
};
export function icone(nome, classe = "") {
    return `<svg class="icone ${classe}" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">${caminhos[nome] ?? caminhos.box}</svg>`;
}
export function instalarCabecalho(ativo = "inicio") {
    const el = $("#cabecalho");
    if (!el) return;
    el.innerHTML = `<div class="barra-marca"><a class="marca" href="index.html">${icone("cake")}<span>Vander<span class="marca-forte">Cake</span></span></a>
        <span class="separador"></span><span class="submarca">Gestão da loja</span></div>
        <nav class="navegacao" aria-label="Navegação principal">
        ${[["inicio", "Visão geral", "grid"], ["vendas", "Vendas", "bag"], ["produtos", "Produtos", "box"], ["vendedores", "Vendedores", "users"], ["clientes", "Clientes", "store"]].map(([id, nome, ico]) =>
            `<a class="${ativo === id ? "atual" : ""}" href="index.html${id === "inicio" ? "" : `#${id}`}" ${ativo === id ? 'aria-current="page"' : ""}>${icone(ico)}${nome}</a>`).join("")}</nav>
        <div class="conta"><span class="avatar neutro">${escapar(iniciais(sessao()?.administrador.nome ?? "AD"))}</span><span class="nome-conta">${escapar(sessao()?.administrador.nome ?? "Administrador")}</span><button class="botao-icone" id="sair" aria-label="Sair da conta" title="Sair">${icone("logout")}</button></div>`;
    $("#sair").addEventListener("click", sair);
    if (CONFIG.modo === "api" && !$("#atalhos")) {
        const atalhos = document.createElement("nav");
        atalhos.id = "atalhos";
        atalhos.className = "atalhos";
        atalhos.setAttribute("aria-label", "Financeiro e cadastros auxiliares");
        atalhos.innerHTML = '<a href="recebimentos.html">Contas a receber</a><a href="pagamentos.html">Cadastros auxiliares</a>';
        el.insertAdjacentElement("afterend", atalhos);
    }
    document.querySelectorAll("[data-modo]").forEach(e => {
        e.textContent = CONFIG.modo === "demo" ? "Demonstração local" : "Conectado à loja";
        e.classList.toggle("ambiente-real", CONFIG.modo === "api");
    });
    document.querySelectorAll("[data-demo]").forEach(e => { e.hidden = CONFIG.modo !== "demo"; });
    document.querySelectorAll("[data-voltar]").forEach(e => e.addEventListener("click", voltar));
}
export function mostrarMensagem(texto, erro = false) {
    const el = $("#mensagem");
    if (!el) return;
    el.textContent = texto;
    el.classList.toggle("erro", erro);
    el.hidden = !texto;
    el.setAttribute("role", erro ? "alert" : "status");
    if (erro) el.scrollIntoView({ behavior: "smooth", block: "nearest" });
}
export function badge(status) {
    const mapa = { Confirmado: ["Confirmada", "verde"], ConfirmadoComCorte: ["Confirmada · com corte", "verde"],
        Cancelado: ["Cancelada", "vermelho"], NaoAtendido: ["Não atendida", "neutro"],
        Pendente: ["Pendente", "amarelo"], AguardandoConfirmacao: ["Pendente", "amarelo"], Processado: ["Processado", "verde"],
        Ativo: ["Ativo", "verde"], Inativo: ["Inativo", "neutro"] };
    const [nome, cor] = mapa[status] ?? [status, "neutro"];
    return `<span class="badge ${cor}"><span class="ponto"></span>${escapar(nome)}</span>`;
}
export function endereco(cliente, dados) {
    if (!cliente) return "Cadastro não disponível";
    const cidade = dados.cidades.find(c => c.id === cliente.cidadeId);
    const estado = dados.estados.find(e => e.id === cidade?.estadoId);
    return `${cliente.rua}, ${cliente.numero} · ${cliente.bairro} · ${cidade?.nome ?? ""}/${estado?.uf ?? ""} · CEP ${cliente.cep}`;
}
export function abrirJanela(url, nome = "") {
    // Apenas páginas do próprio projeto; mantém o opener para o retorno dos cadastros.
    const destino = new URL(url, location.href);
    if (destino.origin !== location.origin) throw new Error("Destino de navegação inválido.");
    const janela = window.open(destino.href, nome || "_blank");
    if (janela) janela.focus();
    else mostrarMensagem("O navegador bloqueou a nova guia. Permita pop-ups deste site e tente novamente.", true);
    return janela;
}
export function voltar() {
    if (window.opener && !window.opener.closed) window.close();
    else location.assign("index.html");
}
export function concluir(mensagem, extras = {}) {
    if (window.opener && !window.opener.closed) {
        window.opener.postMessage({ tipo: "vandercake:salvo", mensagem, ...extras }, location.origin);
        window.close();
    } else location.assign("index.html");
}
export function obterId(nome = "id", obrigatorio = true) {
    const texto = new URLSearchParams(location.search).get(nome);
    if (texto === null && !obrigatorio) return null;
    const id = Number(texto);
    if (!/^\d+$/.test(texto ?? "") || !Number.isSafeInteger(id) || id <= 0) throw new Error("Identificador inválido no endereço.");
    return id;
}
export function preencherSelect(select, itens, selecionado = "", rotulo = x => x.nome, titulo = "Selecione") {
    select.replaceChildren(new Option(titulo, ""));
    [...itens].sort((a, b) => rotulo(a).localeCompare(rotulo(b), "pt-BR")).forEach(item =>
        select.add(new Option(rotulo(item), String(item.id))));
    select.value = String(selecionado ?? "");
}
export function criarConfirmacao({ titulo, descricao, confirmar = "Confirmar", perigo = false, motivo = false }) {
    return new Promise(resolve => {
        const dialog = document.createElement("dialog");
        dialog.className = "dialogo";
        dialog.innerHTML = `<form method="dialog"><div class="dialogo-icone">${icone(perigo ? "x" : "check")}</div>
            <h2 id="titulo-dialogo">${escapar(titulo)}</h2><p>${escapar(descricao)}</p>
            ${motivo ? '<label class="campo">Motivo do cancelamento<textarea name="motivo" maxlength="255" required rows="3" placeholder="Descreva o motivo"></textarea></label>' : ""}
            <div class="rodape-formulario"><button class="botao secundario" value="cancelar" formnovalidate autofocus>Voltar</button>
            <button class="botao ${perigo ? "perigo" : ""}" value="confirmar">${escapar(confirmar)}</button></div></form>`;
        dialog.setAttribute("aria-labelledby", "titulo-dialogo");
        dialog.addEventListener("close", () => {
            const resultado = dialog.returnValue === "confirmar" ? { motivo: dialog.querySelector("textarea")?.value.trim() } : null;
            dialog.remove();
            resolve(resultado);
        }, { once: true });
        document.body.append(dialog);
        dialog.showModal();
    });
}
