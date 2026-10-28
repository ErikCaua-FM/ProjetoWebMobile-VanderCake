import { CONFIG } from "./config.js";
import { sessao, tokenAtual } from "./api.js";
import "../vendor/signalr.min.js";

let conexao;
let iniciando = false;
let timer;
const ouvintes = new Set();
function aviso() { ouvintes.forEach(fn => fn()); }
function estado(texto) {
    document.querySelectorAll("[data-modo]").forEach(e => { e.textContent = texto; });
}
async function conectar() {
    if (iniciando || !sessao() || conexao.state !== signalR.HubConnectionState.Disconnected) return;
    iniciando = true;
    try {
        await conexao.start();
        estado("Ao vivo");
        aviso(); // Recarrega inclusive alterações ocorridas durante a desconexão.
    } catch {
        estado("Reconectando…");
        clearTimeout(timer);
        timer = setTimeout(conectar, 5000);
    } finally { iniciando = false; }
}
export function observarTempoReal(callback) {
    if (CONFIG.modo !== "api") return;
    ouvintes.add(callback);
    if (conexao) return;
    const url = new URL(CONFIG.apiUrl);
    conexao = new signalR.HubConnectionBuilder()
        .withUrl(`${url.origin}/hubs/atualizacoes`, { accessTokenFactory: () => tokenAtual() })
        .configureLogging(signalR.LogLevel.None)
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000]).build();
    conexao.on("DadosAlterados", aviso);
    conexao.onreconnecting(() => estado("Reconectando…"));
    conexao.onreconnected(() => { estado("Ao vivo"); aviso(); });
    conexao.onclose(() => { estado("Reconectando…"); clearTimeout(timer); timer = setTimeout(conectar, 5000); });
    window.addEventListener("online", () => { conectar(); aviso(); });
    window.addEventListener("pagehide", () => { clearTimeout(timer); conexao.stop(); });
    conectar();
}
