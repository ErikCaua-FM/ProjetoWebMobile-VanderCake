import { CONFIG } from "./config.js";
import { conectado, entrar } from "./api.js";
import { CONTA_DEMO } from "./demo-seed.js";
import { reiniciarDemo } from "./demo-store.js";
import { $, mostrarMensagem, criarConfirmacao } from "./comum.js";

if (conectado()) location.replace("index.html");
document.querySelectorAll("[data-demo]").forEach(e => { e.hidden = CONFIG.modo !== "demo"; });
let ocupado = false;
$("#form-login").addEventListener("submit", async evento => {
    evento.preventDefault();
    if (ocupado) return;
    ocupado = true;
    $("#campos").disabled = true;
    $("#entrar").textContent = "Entrando…";
    mostrarMensagem("");
    try {
        await entrar($("#email").value, $("#senha").value);
        location.replace("index.html");
    } catch (erro) { mostrarMensagem(erro.message, true); }
    finally { ocupado = false; $("#campos").disabled = false; $("#entrar").textContent = "Entrar na loja"; }
});
$("#preencher-demo").addEventListener("click", () => {
    $("#email").value = CONTA_DEMO.email;
    $("#senha").value = CONTA_DEMO.senha;
    $("#entrar").focus();
});
$("#ver-senha").addEventListener("click", () => {
    const mostrar = $("#senha").type === "password";
    $("#senha").type = mostrar ? "text" : "password";
    $("#ver-senha").textContent = mostrar ? "Ocultar" : "Mostrar";
    $("#ver-senha").setAttribute("aria-pressed", String(mostrar));
});
$("#reiniciar-demo").addEventListener("click", async () => {
    if (CONFIG.modo !== "demo" || ocupado) return;
    const confirmado = await criarConfirmacao({ titulo: "Reiniciar demonstração?",
        descricao: "Os cadastros e as alterações de demonstração deste navegador serão substituídos pelos exemplos iniciais.", confirmar: "Reiniciar", perigo: true });
    if (!confirmado) return;
    try { await reiniciarDemo(); mostrarMensagem("Dados de exemplo restaurados."); }
    catch (erro) { mostrarMensagem(erro.message, true); }
});
