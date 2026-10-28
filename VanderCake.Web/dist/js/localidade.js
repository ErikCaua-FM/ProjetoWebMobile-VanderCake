import { exigirLogin } from "./api.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, preencherSelect, mostrarMensagem, concluir } from "./comum.js";
const tipo = document.body.dataset.cadastro;
let salvando = false;
let pronto = false;
let versao = 0;
async function carregar() {
    if (salvando) return;
    const atual = ++versao;
    try {
        if (tipo === "cidades") {
            const selecionado = pronto ? $("#estado").value : new URLSearchParams(location.search).get("estado");
            const dados = await repositorio.obterDados();
            if (atual !== versao) return;
            preencherSelect($("#estado"), dados.estados, selecionado, e => `${e.nome} (${e.uf})`, "Selecione um estado");
        }
        pronto = true;
        $("#campos").disabled = false;
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
if (exigirLogin()) {
    instalarCabecalho(tipo === "categorias" ? "produtos" : "vendedores");
    $("#form-localidade").addEventListener("submit", async e => {
        e.preventDefault();
        if (salvando || !pronto) return;
        salvando = true;
        $("#campos").disabled = true;
        try {
            const valores = { nome: $("#nome").value };
            if (tipo === "estados") valores.uf = $("#uf").value.toUpperCase();
            if (tipo === "cidades") valores.estadoId = Number($("#estado").value);
            const registro = await repositorio.salvar(tipo, valores);
            const chave = { categorias: "categoriaId", estados: "estadoId", cidades: "cidadeId" }[tipo];
            concluir("Cadastro salvo e selecionado no formulário de origem.", { [chave]: registro.id });
        } catch (erro) { mostrarMensagem(erro.message, true); }
        finally { salvando = false; $("#campos").disabled = false; }
    });
    if (tipo === "cidades") observarDados(carregar);
    carregar();
}
