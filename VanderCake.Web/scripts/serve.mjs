import { createServer, request } from "node:http";
import { readFile, stat } from "node:fs/promises";
import { resolve, extname, sep } from "node:path";
const raiz = resolve("dist");
const indicePorta = process.argv.indexOf("--port");
const porta = Number(indicePorta >= 0 ? process.argv[indicePorta + 1] : process.env.PORT || 5500);
const tipos = { ".html": "text/html; charset=utf-8", ".js": "text/javascript; charset=utf-8", ".css": "text/css; charset=utf-8", ".svg": "image/svg+xml", ".json": "application/json" };
const api = new URL(process.env.VANDERCAKE_API_URL || "http://127.0.0.1:5080");
if (api.protocol !== "http:") throw new Error("O proxy de desenvolvimento espera uma API HTTP local.");
function encaminhar(req) {
    return request({ hostname: api.hostname, port: api.port, method: req.method,
        path: req.url, headers: { ...req.headers, host: api.host } });
}
const servidor = createServer(async (req, res) => {
    if (/^\/(api|hubs)(\/|$|\?)/.test(req.url)) {
        const destino = encaminhar(req);
        destino.on("response", resposta => { res.writeHead(resposta.statusCode, resposta.headers); resposta.pipe(res); });
        destino.on("error", () => {
            if (!res.headersSent) res.writeHead(502, { "Content-Type": "application/json" });
            res.end(JSON.stringify({ mensagem: "Inicie a API local para usar este endereço de desenvolvimento." }));
        });
        req.pipe(destino);
        return;
    }
    try {
        const url = new URL(req.url, "http://localhost");
        let caminho = resolve(raiz, "." + decodeURIComponent(url.pathname));
        if (caminho !== raiz && !caminho.startsWith(raiz + sep)) { res.writeHead(403).end(); return; }
        if ((await stat(caminho)).isDirectory()) caminho = resolve(caminho, "index.html");
        const bytes = await readFile(caminho);
        res.writeHead(200, { "Content-Type": tipos[extname(caminho)] ?? "application/octet-stream", "Cache-Control": "no-store" });
        res.end(bytes);
    } catch { res.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" }).end("Arquivo não encontrado."); }
});
servidor.on("upgrade", (req, cliente, cabecalhoCliente) => {
    if (!req.url.startsWith("/hubs/")) { cliente.destroy(); return; }
    const destino = encaminhar(req);
    destino.on("upgrade", (resposta, remoto, cabecalhoRemoto) => {
        const linhas = [];
        for (let i = 0; i < resposta.rawHeaders.length; i += 2)
            linhas.push(`${resposta.rawHeaders[i]}: ${resposta.rawHeaders[i + 1]}`);
        cliente.write(`HTTP/${resposta.httpVersion} ${resposta.statusCode} ${resposta.statusMessage}\r\n${linhas.join("\r\n")}\r\n\r\n`);
        if (cabecalhoRemoto.length) cliente.write(cabecalhoRemoto);
        if (cabecalhoCliente.length) remoto.write(cabecalhoCliente);
        cliente.on("error", () => remoto.destroy());
        remoto.on("error", () => cliente.destroy());
        cliente.pipe(remoto).pipe(cliente);
    });
    destino.on("response", () => cliente.destroy());
    destino.on("error", () => cliente.destroy());
    destino.end();
});
servidor.listen(porta, "0.0.0.0", () => console.log(`VanderCake em http://localhost:${servidor.address().port}`));
