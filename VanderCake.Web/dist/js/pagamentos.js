import { exigirLogin } from "./api.js";
import { CONFIG } from "./config.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, escapar, badge, mostrarMensagem } from "./comum.js";

const tipos = {
    categorias: { titulo: "Categorias", campo: "nome", rota: "categorias", max: 60 },
    condicoesPagamento: { titulo: "Condições de pagamento", campo: "descricao", rota: "condicoes-pagamento", max: 60 },
    formasPagamento: { titulo: "Formas de pagamento", campo: "descricao", rota: "formas-pagamento", max: 50 }
};
let dados;
let versao = 0;
async function carregar() {
    const atual = ++versao;
    try {
        const novos = await repositorio.obterDados();
        if (atual !== versao) return;
        dados = novos;
        $("#cadastros-auxiliares").innerHTML = Object.entries(tipos).map(([tipo, config]) => `<section class="card">
            <div class="card-cabecalho"><h2>${config.titulo}</h2><button class="botao pequeno" data-tipo="${tipo}">Incluir +</button></div>
            <div class="tabela-container"><table><caption class="sr-only">${config.titulo}</caption><thead><tr><th>Descrição</th>${tipo === "condicoesPagamento" ? "<th>Prazos</th>" : ""}<th>Situação</th><th></th></tr></thead><tbody>
            ${dados[tipo].map(item => `<tr><td><strong>${escapar(item[config.campo])}</strong></td>${tipo === "condicoesPagamento" ? `<td>${item.dias.join("/") === "0" ? "À vista" : `${item.dias.join("/")} dias`}</td>` : ""}<td>${badge(item.ativo ? "Ativo" : "Inativo")}</td>
            <td><button class="link" data-tipo="${tipo}" data-id="${item.id}">Alterar</button></td></tr>`).join("")}</tbody></table></div></section>`).join("");
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
function editar(tipo, id) {
    const config = tipos[tipo];
    if (!config || !dados) return;
    const atual = id ? dados[tipo].find(x => x.id === id) : null;
    if (id && !atual) return;
    const dialog = document.createElement("dialog");
    dialog.className = "dialogo";
    dialog.innerHTML = `<form><h2>${atual ? "Alterar" : "Incluir"} · ${config.titulo}</h2>
        <label class="campo">Descrição<input name="descricao" required maxlength="${config.max}" value="${escapar(atual?.[config.campo] ?? "")}"></label>
        ${tipo === "condicoesPagamento" ? '<label class="campo">Prazos<select name="dias"><option value="0">À vista</option><option value="7/14">7/14 dias</option><option value="7/14/21">7/14/21 dias</option></select></label>' : ""}
        <label class="linha-checkbox"><input name="ativo" type="checkbox" ${atual?.ativo !== false ? "checked" : ""}> Cadastro ativo</label>
        <p class="mensagem erro" role="alert" hidden></p>
        <div class="rodape-formulario"><button type="button" class="botao secundario" data-voltar>Voltar</button><button class="botao" type="submit">Salvar</button></div></form>`;
    const form = dialog.querySelector("form");
    if (tipo === "condicoesPagamento") form.elements.dias.value = atual?.dias.join("/") ?? "0";
    dialog.querySelector("[data-voltar]").addEventListener("click", () => dialog.close());
    dialog.addEventListener("close", () => dialog.remove(), { once: true });
    let salvando = false;
    form.addEventListener("submit", async evento => {
        evento.preventDefault();
        if (salvando) return;
        salvando = true;
        const botao = form.querySelector('[type="submit"]'); botao.disabled = true;
        const mensagem = dialog.querySelector('[role="alert"]'); mensagem.hidden = true;
        try {
            const valores = { [config.campo]: form.elements.descricao.value, ativo: form.elements.ativo.checked, versao: atual?.versao ?? 0 };
            if (tipo === "condicoesPagamento") valores.dias = form.elements.dias.value.split("/").map(Number);
            await repositorio.salvar(config.rota, valores, atual?.id ?? null);
            dialog.close();
            await carregar();
            mostrarMensagem("Cadastro salvo.");
        } catch (erro) {
            mensagem.textContent = erro.message + (erro.status === 409 ? " Feche este formulário e reabra os dados atualizados." : "");
            mensagem.hidden = false;
        } finally { salvando = false; botao.disabled = false; }
    });
    document.body.append(dialog);
    dialog.showModal();
}
if (exigirLogin()) {
    instalarCabecalho("produtos");
    if (CONFIG.modo !== "api") mostrarMensagem("Estes cadastros são mantidos pela API da loja.", true);
    else {
        $("#cadastros-auxiliares").addEventListener("click", evento => {
            const botao = evento.target.closest("[data-tipo]");
            if (botao) editar(botao.dataset.tipo, Number(botao.dataset.id) || null);
        });
        observarDados(carregar);
        carregar();
    }
}
