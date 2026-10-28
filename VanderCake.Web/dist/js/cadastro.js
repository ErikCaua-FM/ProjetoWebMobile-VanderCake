import { exigirLogin } from "./api.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, obterId, preencherSelect, abrirJanela, concluir, mostrarMensagem, lerCentavos } from "./comum.js";

const tipo = document.body.dataset.cadastro;
const singular = { produtos: "produto", vendedores: "vendedor", clientes: "cliente" }[tipo];
let id = null;
let atual = null;
let dados = null;
let pronto = false;
let salvando = false;
let versaoLeitura = 0;
const janelas = {};
const valor = nome => $(`#${nome}`)?.value ?? "";

function preencherCidades(selecionada = "") {
    const estadoId = Number(valor("estado"));
    const cidades = dados.cidades.filter(c => c.estadoId === estadoId);
    preencherSelect($("#cidade"), cidades, selecionada, c => c.nome,
        estadoId ? cidades.length ? "Selecione uma cidade" : "Nenhuma cidade cadastrada" : "Selecione o estado primeiro");
    $("#cidade").disabled = !estadoId || !cidades.length;
}
function referencias(extras = {}) {
    if (tipo === "produtos") {
        preencherSelect($("#categoria"), dados.categorias.filter(c => c.ativo), extras.categoriaId ?? valor("categoria"), c => c.nome, "Selecione uma categoria");
        return;
    }
    let estadoId = extras.estadoId ?? valor("estado");
    const cidadeId = extras.cidadeId ?? valor("cidade");
    if (extras.cidadeId) estadoId = dados.cidades.find(c => c.id === Number(extras.cidadeId))?.estadoId ?? estadoId;
    preencherSelect($("#estado"), dados.estados, estadoId, e => `${e.nome} (${e.uf})`, "Selecione um estado");
    preencherCidades(cidadeId);
    if (tipo === "clientes") {
        preencherSelect($("#vendedor"), dados.vendedores.filter(v => v.ativo || v.id === atual?.vendedorId),
            extras.vendedorId ?? valor("vendedor"), v => `${v.nome}${v.ativo ? "" : " (inativo — vínculo atual)"}`, "Selecione um vendedor");
    }
}
async function atualizarReferencias(extras = {}) {
    if (!pronto || salvando) return;
    const versao = ++versaoLeitura;
    try {
        const novos = await repositorio.obterDados();
        if (versao !== versaoLeitura || salvando) return;
        dados = novos;
        referencias(extras); // Não substitui os campos em edição nem a versão original do registro.
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
async function iniciar() {
    try {
        id = obterId("id", false);
        dados = await repositorio.obterDados();
        atual = id === null ? null : dados[tipo].find(x => x.id === id);
        if (id !== null && !atual) throw new Error("Cadastro não encontrado.");
        if (atual) {
            $("#titulo-pagina").textContent = `Alterar ${singular}`;
            document.title = `Alterar ${singular} | VanderCake`;
            for (const campo of ["nome", "documento", "email", "telefone", "rua", "bairro", "numero", "cep", "descricao", "estoque"])
                if ($(`#${campo}`)) $(`#${campo}`).value = atual[campo] ?? "";
            if (tipo === "produtos") {
                $("#preco").value = (atual.precoCentavos / 100).toFixed(2).replace(".", ",");
                referencias({ categoriaId: atual.categoriaId });
            } else {
                referencias({ cidadeId: atual.cidadeId, estadoId: dados.cidades.find(c => c.id === atual.cidadeId)?.estadoId, vendedorId: atual.vendedorId });
            }
            if ($("#ativo")) { $("#ativo").checked = atual.ativo; $("#linha-ativo").hidden = false; }
            if ($("#senha")) {
                $("#senha").required = false;
                $("#senha").placeholder = "Deixe em branco para manter a senha atual";
                $("#ajuda-senha").textContent = "Preencha somente se quiser alterar a senha de acesso.";
            }
        } else referencias();
        pronto = true;
        $("#campos").disabled = false;
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
function coletar() {
    const comuns = { nome: valor("nome"), versao: atual?.versao };
    if (tipo === "produtos") return { ...comuns, descricao: valor("descricao"),
        precoCentavos: lerCentavos(valor("preco")), estoque: Number(valor("estoque")),
        categoriaId: Number(valor("categoria")), ativo: $("#ativo").checked };
    const cidade = dados.cidades.find(c => c.id === Number(valor("cidade")));
    if (!cidade || cidade.estadoId !== Number(valor("estado"))) throw new Error("Selecione uma cidade do estado informado.");
    return { ...comuns, documento: valor("documento"), email: valor("email"), telefone: valor("telefone"),
        rua: valor("rua"), bairro: valor("bairro"), numero: valor("numero"), cep: valor("cep"), cidadeId: cidade.id,
        ...(tipo === "vendedores" ? { senha: valor("senha") || undefined } :
            { vendedorId: Number(valor("vendedor")), ativo: $("#ativo").checked }) };
}
if (exigirLogin()) {
    instalarCabecalho(tipo);
    $("#estado")?.addEventListener("change", () => preencherCidades());
    $("#nova-categoria")?.addEventListener("click", () => { janelas.categorias = abrirJanela("categoria.html", "vandercake-categoria"); });
    $("#novo-estado")?.addEventListener("click", () => { janelas.estados = abrirJanela("estado.html", "vandercake-estado"); });
    $("#nova-cidade")?.addEventListener("click", () => { janelas.cidades = abrirJanela(`cidade.html?estado=${Number(valor("estado")) || ""}`, "vandercake-cidade"); });
    window.addEventListener("message", async evento => {
        const mensagem = evento.data;
        if (evento.origin !== location.origin || mensagem?.tipo !== "vandercake:salvo" ||
            !Object.values(janelas).includes(evento.source)) return;
        await atualizarReferencias(mensagem);
        mostrarMensagem(String(mensagem.mensagem));
    });
    $("#form-cadastro").addEventListener("submit", async evento => {
        evento.preventDefault();
        if (!pronto || salvando) return;
        salvando = true;
        $("#campos").disabled = true;
        mostrarMensagem("");
        try {
            const valores = coletar();
            await repositorio.salvar(tipo, valores, id);
            concluir(`${singular[0].toUpperCase() + singular.slice(1)} ${id === null ? "cadastrado" : "atualizado"} com sucesso.`);
        } catch (erro) { mostrarMensagem(erro.message, true); }
        finally { salvando = false; $("#campos").disabled = false; }
    });
    observarDados(atualizarReferencias);
    iniciar();
}
