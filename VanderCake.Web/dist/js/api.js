import { CONFIG } from "./config.js";
import { CONTA_DEMO } from "./demo-seed.js";
import { novoIdentificador } from "./regras.js";

const CHAVE_SESSAO = `vandercake.web.${CONFIG.modo}.sessao.v1`;
const CHAVE_SAIDA = `vandercake.web.${CONFIG.modo}.saida.v1`;
export function sessao() {
    try {
        const s = JSON.parse(sessionStorage.getItem(CHAVE_SESSAO) ?? "null");
        return s?.modo === CONFIG.modo && s.token && (s.expira > Date.now() || s.refreshToken) ? s : null;
    } catch { return null; }
}
export const conectado = () => Boolean(sessao());
export function exigirLogin() {
    if (conectado()) return true;
    location.replace("login.html");
    return false;
}
export function sair() {
    sessionStorage.removeItem(CHAVE_SESSAO);
    localStorage.setItem(CHAVE_SAIDA, novoIdentificador());
    location.replace("login.html");
}
window.addEventListener("storage", e => {
    if (e.key === CHAVE_SAIDA) {
        sessionStorage.removeItem(CHAVE_SESSAO);
        location.replace("login.html");
    }
});

export class ErroApi extends Error {
    constructor(status, mensagem, dados = null) { super(mensagem); this.status = status; this.dados = dados; }
}
let renovacao = null;
export async function tokenAtual(forcar = false) {
    const atual = sessao();
    if (!atual) { sair(); throw new ErroApi(401, "Entre na administração."); }
    if (CONFIG.modo !== "api" || (!forcar && atual.expira > Date.now() + 60000)) return atual.token;
    if (!atual.refreshToken) { sair(); throw new ErroApi(401, "Entre novamente."); }
    renovacao ??= (async () => {
        try {
            const resposta = await requisicao("/auth/renovar", "POST", { refreshToken: atual.refreshToken }, false);
            guardarSessao(resposta);
            return resposta.accessToken;
        } catch (erro) {
            if (erro.status === 401) sair();
            throw erro;
        } finally { renovacao = null; }
    })();
    return renovacao;
}
export async function requisicao(caminho, metodo = "GET", corpo, autenticada = true, repetida = false) {
    if (CONFIG.modo !== "api") throw new Error("O cliente HTTP só funciona no modo API.");
    if (autenticada && !exigirLogin()) throw new ErroApi(401, "Entre na administração.");
    const headers = { Accept: "application/json" };
    if (corpo !== undefined) headers["Content-Type"] = "application/json";
    if (autenticada) headers.Authorization = `Bearer ${await tokenAtual()}`;
    const controller = new AbortController();
    const timer = setTimeout(() => controller.abort(), CONFIG.timeoutMs);
    let resposta;
    let dados;
    try {
        resposta = await fetch(CONFIG.apiUrl.replace(/\/$/, "") + caminho, {
            method: metodo, headers, body: corpo === undefined ? undefined : JSON.stringify(corpo),
            signal: controller.signal, cache: "no-store"
        });
        const conteudo = await resposta.text();
        try { dados = conteudo ? JSON.parse(conteudo) : null; }
        catch { throw new ErroApi(resposta.status, "A API retornou uma resposta que não é JSON válido."); }
    } catch (erro) {
        if (erro instanceof ErroApi) throw erro;
        throw new ErroApi(0, "Não foi possível comunicar com a API. Confira a conexão e tente novamente.");
    } finally { clearTimeout(timer); }
    if (!resposta.ok) {
        if (resposta.status === 401 && autenticada && !repetida) {
            await tokenAtual(true);
            return requisicao(caminho, metodo, corpo, autenticada, true);
        }
        if (resposta.status === 401 && autenticada) sair();
        const campos = Object.values(dados?.errors ?? dados?.erros ?? {}).flat().join(" ");
        throw new ErroApi(resposta.status, campos || dados?.mensagem || dados?.detail ||
            (resposta.status === 403 ? "Seu acesso não permite esta operação." : `A API retornou o erro ${resposta.status}.`), dados);
    }
    return dados;
}

export async function entrar(email, senha) {
    let resposta;
    if (CONFIG.modo === "demo") {
        if (email.trim().toLowerCase() !== CONTA_DEMO.email || senha !== CONTA_DEMO.senha)
            throw new Error("E-mail ou senha incorretos. Utilize a conta de demonstração indicada abaixo.");
        resposta = { accessToken: "demonstracao-local", expiresIn: 28800,
            administrador: { id: 1, nome: "Administrador", ativo: true } };
    } else {
        resposta = await requisicao("/auth/administradores/login", "POST", { email: email.trim(), senha }, false);
    }
    guardarSessao(resposta);
}

function guardarSessao(resposta) {
    if (!resposta?.accessToken || !Number.isFinite(resposta.expiresIn) || resposta.expiresIn <= 0 ||
        !resposta.administrador?.id || !resposta.administrador.ativo)
        throw new Error("A API não retornou uma sessão administrativa válida.");
    sessionStorage.setItem(CHAVE_SESSAO, JSON.stringify({ modo: CONFIG.modo, token: resposta.accessToken, refreshToken: resposta.refreshToken,
        expira: Date.now() + resposta.expiresIn * 1000, administrador: resposta.administrador }));
}
