// A API serve a WEB no mesmo endereço. Não há dependência de CDN.
// O modo "demo" é opcional e usa somente exemplos locais separados.
export const CONFIG = Object.freeze({
    modo: "api",
    apiUrl: `${location.origin}/api`,
    atualizacaoMs: 30000,
    timeoutMs: 20000
});
