import { readdir, readFile, writeFile } from "node:fs/promises";
import { dirname, join, relative, extname } from "node:path";
import { fileURLToPath } from "node:url";

const raiz = dirname(dirname(fileURLToPath(import.meta.url)));
const descricoes = {
    "dist/index.html": "Estrutura do painel com as quatro listas da loja.",
    "dist/login.html": "Formulário de entrada e acesso de demonstração.",
    "dist/pendentes.html": "Estrutura da lista de Pedidos Gerais pendentes.",
    "dist/recebimentos.html": "Consulta e baixa integral manual de parcelas.",
    "dist/pagamentos.html": "Manutenção de categorias, condições e formas de pagamento.",
    "dist/js/recebimentos.js": "Filtros, recebimento de parcelas e atualização dos dados.",
    "dist/js/pagamentos.js": "Formulários de cadastros auxiliares com controle de versão.",
    "dist/pedido.html": "Estrutura dos detalhes e das ações sobre o envio.",
    "dist/produto.html": "Formulário de produto com categoria, preço e estoque.",
    "dist/vendedor.html": "Formulário de vendedor, credenciais e endereço.",
    "dist/cliente.html": "Formulário de cliente, endereço e vendedor responsável.",
    "dist/categoria.html": "Formulário de inclusão de categoria.",
    "dist/estado.html": "Formulário de estado e UF.",
    "dist/cidade.html": "Formulário de cidade vinculada ao estado.",
    "dist/favicon.svg": "Ícone vetorial local da aplicação.",
    "dist/css/estilos.css": "Cores, espaçamentos, componentes e regras responsivas.",
    "dist/js/config.js": "Configuração do ambiente demo ou API.",
    "dist/js/api.js": "Cliente HTTP, autenticação, renovação de sessão e saída.",
    "dist/js/tempo-real.js": "SignalR, reconexão e avisos às páginas.",
    "dist/js/dados.js": "Interface comum de acesso aos dados e notificações entre guias.",
    "dist/js/demo-store.js": "Persistência da demonstração e transações entre guias no IndexedDB.",
    "dist/js/demo-seed.js": "Criação de registros fictícios para explorar a aplicação.",
    "dist/js/regras.js": "Validações e cálculos do simulador, isolados das telas e do armazenamento.",
    "dist/js/comum.js": "Formatação brasileira, elementos reutilizáveis e navegação de retorno.",
    "dist/js/login.js": "Envio do formulário de login e controles da demonstração.",
    "dist/js/painel.js": "Renderização, pesquisas e ações de cada lista do painel.",
    "dist/js/pendentes.js": "Busca e exibição dos lotes aguardando decisão.",
    "dist/js/pedido.js": "Carregamento da prévia, detalhes e decisão da loja.",
    "dist/js/cadastro.js": "Comportamento compartilhado dos formulários de produto, vendedor e cliente.",
    "dist/js/localidade.js": "Inclusão de categoria, estado e cidade, com retorno à guia de origem.",
    "scripts/serve.mjs": "Servidor de desenvolvimento com proxy HTTP/WebSocket para a API local.",
    "scripts/documentar.mjs": "Gera este guia a partir dos arquivos reais, preservando o código integral.",
    "tests/regras.test.js": "Cenários automatizados para as regras do domínio.",
    "package.json": "Metadados e comandos do projeto; SignalR é distribuído localmente em dist/vendor.",
    ".gitignore": "Exclusões de arquivos locais e temporários no controle de versão."
};

async function listar(pasta) {
    const entradas = await readdir(join(raiz, pasta), { withFileTypes: true });
    const arquivos = [];
    for (const entrada of entradas) {
        const caminho = join(pasta, entrada.name);
        arquivos.push(...(entrada.isDirectory() ? await listar(caminho) : [caminho]));
    }
    return arquivos;
}

const arquivos = ["package.json", ".gitignore"];
for (const pasta of ["dist", "scripts", "tests"]) arquivos.push(...(await listar(pasta)).filter(a => !a.replaceAll("\\", "/").startsWith("dist/vendor/")));
arquivos.sort((a, b) => a.localeCompare(b));
const textos = [
    "# VanderCake.Web — guia e código completo",
    "Este arquivo contém o código integral da etapa Web, separado por pasta e arquivo. Copie cada bloco para o caminho indicado, preservando os nomes e a indentação. O ZIP já traz essa estrutura pronta.",
    "## Como executar",
    "Siga o TUTORIAL_WINDOWS.md do pacote integrado. A API serve a WEB em http://localhost:5080; use o administrador criado por --criar-admin. O modo padrão usa a API. A biblioteca oficial SignalR e sua licença estão no ZIP em dist/vendor; seu código de terceiros não é reproduzido neste guia.",
    "## Como as partes se relacionam",
    "O HTML define os campos e espaços da tela; o CSS define a apresentação; os módulos de página carregam os dados e respondem aos cliques. `dados.js` encaminha cada operação para o simulador IndexedDB ou para o cliente HTTP. `regras.js` concentra os cálculos puros da demonstração, cobertos por testes. Na integração real, a API é a autoridade para autenticação, estoque, vendas e pagamentos.",
    "Leia nesta ordem: `dist/index.html`, `dist/js/painel.js`, `dist/js/dados.js`, `dist/js/demo-store.js` e `dist/js/regras.js`. Depois acompanhe `dist/js/pedido.js` para entender a confirmação e o pagamento de cada parcela. Os formulários compartilham `cadastro.js` e `localidade.js`.",
    "## Arquivos",
    ...arquivos.map(a => `- \`${a.replaceAll("\\", "/")}\`: ${descricoes[a.replaceAll("\\", "/")] ?? "Arquivo do projeto."}`)
];
const linguagens = { ".html": "html", ".css": "css", ".js": "javascript", ".mjs": "javascript", ".json": "json", ".svg": "xml" };
for (const arquivo of arquivos) {
    const caminho = relative(raiz, join(raiz, arquivo)).replaceAll("\\", "/");
    const codigo = await readFile(join(raiz, arquivo), "utf8");
    textos.push(`## ${caminho}`, descricoes[caminho] ?? "Arquivo do projeto.",
        "````" + (linguagens[extname(arquivo)] ?? "text") + "\n" + codigo + (codigo.endsWith("\n") ? "" : "\n") + "````");
}
textos.push("## Referências", "Consulte README.md, docs/CONTRATO_API.md e os documentos do pacote integrado em ../Docs. Os arquivos acima usam a API real por padrão.");
await writeFile(join(raiz, "docs/GUIA_CODIGO_COMPLETO.md"), textos.join("\n\n") + "\n");
console.log(`Guia gerado com ${arquivos.length} arquivos completos.`);
