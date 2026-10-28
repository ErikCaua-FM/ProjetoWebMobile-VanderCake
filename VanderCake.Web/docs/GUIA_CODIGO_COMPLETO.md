# VanderCake.Web — guia e código completo

Este arquivo contém o código integral da etapa Web, separado por pasta e arquivo. Copie cada bloco para o caminho indicado, preservando os nomes e a indentação. O ZIP já traz essa estrutura pronta.

## Como executar

Siga o TUTORIAL_WINDOWS.md do pacote integrado. A API serve a WEB em http://localhost:5080; use o administrador criado por --criar-admin. O modo padrão usa a API. A biblioteca oficial SignalR e sua licença estão no ZIP em dist/vendor; seu código de terceiros não é reproduzido neste guia.

## Como as partes se relacionam

O HTML define os campos e espaços da tela; o CSS define a apresentação; os módulos de página carregam os dados e respondem aos cliques. `dados.js` encaminha cada operação para o simulador IndexedDB ou para o cliente HTTP. `regras.js` concentra os cálculos puros da demonstração, cobertos por testes. Na integração real, a API é a autoridade para autenticação, estoque, vendas e pagamentos.

Leia nesta ordem: `dist/index.html`, `dist/js/painel.js`, `dist/js/dados.js`, `dist/js/demo-store.js` e `dist/js/regras.js`. Depois acompanhe `dist/js/pedido.js` para entender a confirmação e o pagamento de cada parcela. Os formulários compartilham `cadastro.js` e `localidade.js`.

## Arquivos

- `.gitignore`: Exclusões de arquivos locais e temporários no controle de versão.

- `dist/categoria.html`: Formulário de inclusão de categoria.

- `dist/cidade.html`: Formulário de cidade vinculada ao estado.

- `dist/cliente.html`: Formulário de cliente, endereço e vendedor responsável.

- `dist/css/estilos.css`: Cores, espaçamentos, componentes e regras responsivas.

- `dist/estado.html`: Formulário de estado e UF.

- `dist/favicon.svg`: Ícone vetorial local da aplicação.

- `dist/index.html`: Estrutura do painel com as quatro listas da loja.

- `dist/js/api.js`: Cliente HTTP, autenticação, renovação de sessão e saída.

- `dist/js/cadastro.js`: Comportamento compartilhado dos formulários de produto, vendedor e cliente.

- `dist/js/comum.js`: Formatação brasileira, elementos reutilizáveis e navegação de retorno.

- `dist/js/config.js`: Configuração do ambiente demo ou API.

- `dist/js/dados.js`: Interface comum de acesso aos dados e notificações entre guias.

- `dist/js/demo-seed.js`: Criação de registros fictícios para explorar a aplicação.

- `dist/js/demo-store.js`: Persistência da demonstração e transações entre guias no IndexedDB.

- `dist/js/localidade.js`: Inclusão de categoria, estado e cidade, com retorno à guia de origem.

- `dist/js/login.js`: Envio do formulário de login e controles da demonstração.

- `dist/js/pagamentos.js`: Formulários de cadastros auxiliares com controle de versão.

- `dist/js/painel.js`: Renderização, pesquisas e ações de cada lista do painel.

- `dist/js/pedido.js`: Carregamento da prévia, detalhes e decisão da loja.

- `dist/js/pendentes.js`: Busca e exibição dos lotes aguardando decisão.

- `dist/js/recebimentos.js`: Filtros, recebimento de parcelas e atualização dos dados.

- `dist/js/regras.js`: Validações e cálculos do simulador, isolados das telas e do armazenamento.

- `dist/js/tempo-real.js`: SignalR, reconexão e avisos às páginas.

- `dist/login.html`: Formulário de entrada e acesso de demonstração.

- `dist/pagamentos.html`: Manutenção de categorias, condições e formas de pagamento.

- `dist/pedido.html`: Estrutura dos detalhes e das ações sobre o envio.

- `dist/pendentes.html`: Estrutura da lista de Pedidos Gerais pendentes.

- `dist/produto.html`: Formulário de produto com categoria, preço e estoque.

- `dist/recebimentos.html`: Consulta e baixa integral manual de parcelas.

- `dist/vendedor.html`: Formulário de vendedor, credenciais e endereço.

- `package.json`: Metadados e comandos do projeto; SignalR é distribuído localmente em dist/vendor.

- `scripts/documentar.mjs`: Gera este guia a partir dos arquivos reais, preservando o código integral.

- `scripts/serve.mjs`: Servidor de desenvolvimento com proxy HTTP/WebSocket para a API local.

- `tests/regras.test.js`: Cenários automatizados para as regras do domínio.

## .gitignore

Exclusões de arquivos locais e temporários no controle de versão.

````text
node_modules/
.env
.env.*
*.log
artifacts/
.DS_Store
````

## dist/categoria.html

Formulário de inclusão de categoria.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Nova categoria | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/localidade.js"></script>
</head>
<body data-cadastro="categorias">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Nova categoria</h1><p class="muted">Organize os produtos por categoria ou grupo.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
<form id="form-localidade" class="formulario formulario-curto"><fieldset id="campos" disabled><section class="card formulario-card"><div class="campos-grid"><label class="campo" for="nome">Nome da categoria<input id="nome" name="nome" type="text" required maxlength=60  placeholder=""></label></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/cidade.html

Formulário de cidade vinculada ao estado.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Nova cidade | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/localidade.js"></script>
</head>
<body data-cadastro="cidades">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Nova cidade</h1><p class="muted">Vincule a cidade a um estado cadastrado.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
<form id="form-localidade" class="formulario formulario-curto"><fieldset id="campos" disabled><section class="card formulario-card"><div class="campos-grid"><label class="campo" for="nome">Nome da cidade<input id="nome" name="nome" type="text" required maxlength=100  placeholder=""></label><div class="campo"><label for="estado">Estado</label><div class="picker-linha"><select id="estado" name="estado" required><option value="">Selecione</option></select></div></div></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/cliente.html

Formulário de cliente, endereço e vendedor responsável.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Cadastrar cliente | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/cadastro.js"></script>
</head>
<body data-cadastro="clientes">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Cadastrar cliente</h1><p class="muted">Cadastre os dados e defina o vendedor responsável.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <form id="form-cadastro" class="formulario"><fieldset id="campos" disabled><section class="card formulario-card"><div class="titulo-secao"><span>01</span><div><h2>Identificação e contato</h2><p>Preencha os campos para identificar o cadastro.</p></div></div><div class="campos-grid"><label class="campo" for="nome">Nome completo / razão social<input id="nome" name="nome" type="text" required maxlength=100  placeholder="Como devemos identificar este cadastro?"></label><label class="campo" for="documento">CPF / CNPJ<input id="documento" name="documento" type="text" required maxlength=20 inputmode="numeric" placeholder="Somente números ou com pontuação"></label><label class="campo" for="email">E-mail (opcional)<input id="email" name="email" type="email"  maxlength=150 autocomplete="off" placeholder="nome@exemplo.com"></label><label class="campo" for="telefone">Telefone com DDD<input id="telefone" name="telefone" type="tel" required maxlength=20  placeholder="(45) 99999-9999"></label><div class="coluna-inteira"><div class="campo"><label for="vendedor">Vendedor responsável</label><div class="picker-linha"><select id="vendedor" name="vendedor" required><option value="">Selecione</option></select></div></div></div><label class="checkbox coluna-inteira" id="linha-ativo" hidden><input type="checkbox" id="ativo" checked> Cliente ativo</label></div></section><section class="card formulario-card"><div class="titulo-secao"><span>02</span><div><h2>Endereço</h2><p>Selecione o estado para encontrar as cidades vinculadas.</p></div></div><div class="campos-grid"><div class="campo"><label for="estado">Estado</label><div class="picker-linha"><select id="estado" name="estado" required><option value="">Selecione</option></select><button type="button" class="botao secundario pequeno" id="novo-estado">Novo Estado+</button></div></div><div class="campo"><label for="cidade">Cidade</label><div class="picker-linha"><select id="cidade" name="cidade" required><option value="">Selecione</option></select><button type="button" class="botao secundario pequeno" id="nova-cidade">Nova Cidade+</button></div></div><label class="campo" for="rua">Rua / logradouro<input id="rua" name="rua" type="text" required maxlength=100  placeholder="Nome da rua"></label><label class="campo" for="numero">Número<input id="numero" name="numero" type="text" required maxlength=10  placeholder="120 ou S/N"></label><label class="campo" for="bairro">Bairro<input id="bairro" name="bairro" type="text" required maxlength=60  placeholder="Bairro"></label><label class="campo" for="cep">CEP<input id="cep" name="cep" type="text" required maxlength=9 inputmode="numeric" placeholder="00000-000"></label></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/css/estilos.css

Cores, espaçamentos, componentes e regras responsivas.

````css
:root {
    --primary: #ea1d2c;
    --primary-dark: #c8101e;
    --primary-soft: #fff0f1;
    --ink: #24242c;
    --muted: #7b7c86;
    --line: #e9e9ee;
    --surface: #fff;
    --background: #f7f7f9;
    --green: #16845b;
    --radius: 14px;
    font-family: "Segoe UI", -apple-system, BlinkMacSystemFont, Arial, sans-serif;
    color: var(--ink);
    background: var(--background);
    font-size: 14px;
    font-synthesis: none;
}
* { box-sizing: border-box; }
[hidden] { display: none !important; }
body { margin: 0; min-width: 320px; }
button, input, select, textarea { font: inherit; }
button, a, input, select, textarea { -webkit-tap-highlight-color: transparent; }
a { color: inherit; text-decoration: none; }
button { cursor: pointer; }
button:disabled, fieldset:disabled button { cursor: wait; opacity: .55; }
:focus-visible { outline: 3px solid #ee7b84; outline-offset: 3px; }
h1, h2, h3, p { margin-top: 0; }
h1 { font-size: clamp(26px, 2.5vw, 36px); letter-spacing: -1.1px; font-weight: 700; margin-bottom: 9px; line-height: 1.18; }
h2 { font-size: 20px; font-weight: 650; letter-spacing: -.45px; margin-bottom: 6px; }
h3 { font-size: 16px; margin-bottom: 7px; }
p { line-height: 1.55; }
small { font-size: 12px; }
.muted, .opcional { color: var(--muted); }
.cor-primaria { color: var(--primary); }
.sr-only { position: absolute; width: 1px; height: 1px; padding: 0; overflow: hidden; clip: rect(0,0,0,0); white-space: nowrap; border: 0; }
.pular { position: fixed; z-index: 50; top: -80px; left: 10px; padding: 12px; background: #fff; }
.pular:focus { top: 10px; }
.icone { width: 20px; height: 20px; flex-shrink: 0; vertical-align: middle; }
.cabecalho { min-height: 84px; display: flex; align-items: center; gap: 26px; padding: 0 3.5%; background: var(--surface); border-bottom: 1px solid var(--line); }
.barra-marca, .marca, .conta { display: flex; align-items: center; }
.barra-marca { gap: 18px; flex-shrink: 0; }
.marca { color: var(--primary); gap: 9px; font-size: 25px; letter-spacing: -.9px; font-weight: 500; }
.marca-forte { font-weight: 750; }
.marca .icone { width: 31px; height: 31px; }
.separador { width: 1px; height: 22px; background: var(--line); }
.submarca { font-size: 12px; color: var(--muted); }
.navegacao { display: flex; align-self: stretch; gap: 18px; margin: 0 auto; }
.navegacao a { display: flex; align-items: center; gap: 7px; position: relative; color: #666772; font-size: 13px; white-space: nowrap; }
.navegacao .icone { width: 17px; height: 17px; }
.navegacao a.atual { color: var(--primary); font-weight: 650; }
.navegacao a.atual::after { content: ""; position: absolute; bottom: 0; height: 3px; left: 0; right: 0; border-radius: 3px 3px 0 0; background: var(--primary); }
.conta { gap: 10px; font-size: 12px; }
.avatar { width: 33px; height: 33px; border-radius: 10px; display: inline-flex; align-items: center; justify-content: center; color: #bf3646; background: #fff0f1; font-weight: 650; font-size: 11px; flex-shrink: 0; }
.avatar.neutro { background: #f0f1f4; color: #676873; }
.botao-icone { border: 0; background: none; color: #8b8b96; padding: 8px; border-radius: 8px; }
.botao-icone:hover { background: var(--background); color: var(--primary); }
.container { max-width: 1580px; margin: 0 auto; padding: 33px 3.5% 20px; }
.apresentacao { display: flex; align-items: center; justify-content: space-between; gap: 24px; margin-bottom: 26px; }
.apresentacao p { margin-bottom: 0; }
.apresentacao .sobretitulo { margin-bottom: 10px; }
.apresentacao-acoes { display: flex; align-items: center; gap: 12px; }
.sobretitulo { color: #9798a2; font-size: 10px; letter-spacing: 1.7px; font-weight: 700; margin-bottom: 8px; }
.ambiente { display: inline-flex; align-items: center; gap: 7px; padding: 7px 10px; font-size: 11px; color: #af5261; border-radius: 20px; background: #fcebed; white-space: nowrap; }
.ambiente::before { content: ""; width: 5px; height: 5px; border-radius: 50%; background: currentColor; }
.ambiente-real { color: var(--green); background: #e9f7f0; }
.botao { display: inline-flex; align-items: center; justify-content: center; gap: 7px; border: 1px solid var(--primary); background: var(--primary); color: white; font-weight: 600; padding: 12px 20px; border-radius: 8px; transition: background .15s, border-color .15s; line-height: 1.35; white-space: nowrap; }
.botao:hover:not(:disabled) { background: var(--primary-dark); border-color: var(--primary-dark); }
.botao.secundario { color: #51525c; background: white; border-color: #dedee5; }
.botao.secundario:hover:not(:disabled) { color: var(--primary); background: #fff9f9; border-color: #efc7cb; }
.botao.contorno { color: var(--primary); background: white; border-color: #f2b9bd; }
.botao.contorno:hover { background: var(--primary-soft); }
.botao.pequeno { font-size: 12px; padding: 9px 13px; }
.botao.perigo { background: #c71d2a; border-color: #c71d2a; }
.perigo-texto { color: #c71d2a !important; }
.numero-botao { font-size: 10px; background: var(--primary-soft); padding: 1px 6px; border-radius: 4px; }
.estatisticas { display: grid; grid-template-columns: repeat(4, 1fr); gap: 18px; margin-bottom: 26px; }
.estatistica { background: var(--surface); border: 1px solid var(--line); border-radius: 12px; padding: 18px 20px; display: flex; align-items: center; gap: 15px; }
.estatistica .icone-circulo { background: #fff2f3; color: var(--primary); border-radius: 10px; height: 43px; width: 43px; display: grid; place-items: center; flex-shrink: 0; }
.estatistica:nth-child(2) .icone-circulo { background: #fff7e8; color: #c58923; }
.estatistica:nth-child(4) .icone-circulo { background: #edf4f9; color: #537c9b; }
.estatistica p { color: var(--muted); font-size: 12px; margin-bottom: 3px; }
.estatistica strong { display: block; font-size: 23px; letter-spacing: -.6px; font-weight: 700; line-height: 1.3; }
.estatistica small { color: #999aa4; font-size: 10px; }
.painel-grid { display: grid; grid-template-columns: 1.1fr 1fr; gap: 23px; align-items: start; }
.card { background: var(--surface); border: 1px solid var(--line); border-radius: var(--radius); overflow: hidden; box-shadow: 0 2px 2px #20203001; }
.card-cabecalho { display: flex; align-items: center; justify-content: space-between; padding: 22px 22px 15px; gap: 10px; }
.card-cabecalho p { font-size: 12px; margin-bottom: 0; }
.card-cabecalho .sobretitulo { font-size: 9px; margin-bottom: 5px; }
.contador { font-size: 10px; font-weight: 500; background: #f1f1f5; color: #83838e; padding: 2px 6px; border-radius: 5px; vertical-align: middle; margin-left: 4px; letter-spacing: 0; }
.filtros { display: flex; gap: 10px; padding: 0 22px 18px; }
.filtros select { width: auto; max-width: 155px; padding: 9px 27px 9px 9px; font-size: 11px; background-color: #fff; }
.pesquisa { position: relative; flex: 1; min-width: 100px; }
.pesquisa input { background: #fafafc; border-color: #ededf2; padding: 10px 11px 10px 34px; font-size: 12px; }
.lupa { position: absolute; top: 13px; left: 13px; width: 11px; height: 11px; border: 1.5px solid #a1a1ac; border-radius: 100%; pointer-events: none; }
.lupa::after { content: ""; position: absolute; width: 5px; height: 1.5px; transform: rotate(45deg); background: #a1a1ac; right: -4px; bottom: -2px; }
.tabela-container { overflow: auto; width: 100%; }
.painel-card .tabela-container { max-height: 326px; }
table { width: 100%; border-collapse: collapse; text-align: left; font-size: 12px; }
th { color: #92929d; font-size: 10px; font-weight: 600; background: #fafafc; white-space: nowrap; position: sticky; top: 0; z-index: 1; }
th, td { padding: 13px 13px; border-bottom: 1px solid #f0f0f3; vertical-align: middle; }
th:first-child, td:first-child { padding-left: 22px; }
th:last-child, td:last-child { padding-right: 22px; }
tbody tr:last-child td { border-bottom: none; }
tbody tr:hover { background: #fdfdfe; }
td strong { font-weight: 600; }
.texto-secundario { display: block; color: #9797a1; font-size: 10px; line-height: 1.5; margin-top: 4px; font-weight: 400; }
.nome-registro { display: flex; align-items: center; gap: 10px; }
.nome-registro > div { min-width: 0; }
.coluna-valor { white-space: nowrap; font-variant-numeric: tabular-nums; }
.numero { text-align: right; font-variant-numeric: tabular-nums; }
.acoes { display: flex; align-items: center; justify-content: flex-end; gap: 8px; }
.link { border: 0; padding: 4px 0; background: transparent; color: var(--primary); font-size: 11px; font-weight: 600; text-decoration: none; white-space: nowrap; }
.link:hover { text-decoration: underline; }
.link.discreto { color: #888894; font-weight: 400; }
.link.divisoria { border-left: 1px solid var(--line); border-radius: 0; padding-left: 8px; }
.badge { display: inline-flex; align-items: center; gap: 5px; font-size: 9px; font-weight: 600; padding: 5px 7px; border-radius: 5px; white-space: nowrap; }
.ponto { width: 4px; height: 4px; border-radius: 100%; background: currentColor; flex-shrink: 0; }
.verde { color: #16845b; background: #eaf7f0; }
.vermelho { color: #d04c59; background: #fff0f1; }
.amarelo { color: #ac7927; background: #fff7e6; }
.neutro { color: #8a8c96; background: #f0f1f4; }
.estoque-baixo { color: #be7a28; }
.card-rodape { padding: 13px 22px; display: flex; justify-content: space-between; gap: 10px; border-top: 1px solid var(--line); color: #9999a4; font-size: 10px; background: #fefefe; }
.vazio { padding: 28px; text-align: center; color: var(--muted); }
.vazio p { margin: 0; }
.atualizacao { color: #9a9aa5; font-size: 10px; text-align: right; margin: 18px 0 0; }
.rodape { max-width: 1580px; margin: 0 auto; padding: 18px 3.5% 24px; display: flex; justify-content: space-between; color: #92929f; font-size: 10px; gap: 15px; }
.mensagem, .aviso { padding: 14px 18px; margin-bottom: 20px; border: 1px solid #d4ebdf; border-radius: 9px; background: #eef8f2; color: #21704c; font-size: 13px; line-height: 1.6; }
.mensagem.erro { border-color: #f5d3d5; background: #fff1f2; color: #aa2b37; }
.aviso { border-color: #f1e3c2; background: #fffaf0; color: #9c762b; }
.voltar { display: inline-flex; color: #858590; font-size: 12px; margin: 0 0 23px; }
.voltar:hover { color: var(--primary); }
.formulario { max-width: 880px; margin: 0 auto; }
.formulario-curto { max-width: 680px; }
.formulario-card { padding: 28px; margin-bottom: 22px; }
fieldset { margin: 0; padding: 0; border: 0; min-width: 0; }
.campos-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 22px 24px; }
.coluna-inteira { grid-column: 1 / -1; }
.campo { display: flex; flex-direction: column; gap: 8px; font-size: 12px; color: #555560; font-weight: 550; }
input, select, textarea { width: 100%; min-width: 0; border: 1px solid #dfe0e6; background: white; border-radius: 7px; padding: 12px 13px; color: var(--ink); outline-offset: 2px; font-size: 13px; font-weight: 400; }
input::placeholder, textarea::placeholder { color: #b0b1ba; }
select { cursor: pointer; }
textarea { resize: vertical; line-height: 1.55; }
input:disabled, select:disabled, textarea:disabled { background: #fafafa; }
.picker-linha { display: flex; align-items: center; gap: 8px; }
.picker-linha select { flex: 1; }
.picker-linha .botao { white-space: nowrap; padding: 12px 9px; font-size: 10px; }
.checkbox { display: flex; align-items: center; gap: 10px; font-size: 12px; }
.checkbox input { width: 17px; height: 17px; accent-color: var(--primary); }
.titulo-secao { display: flex; gap: 14px; align-items: flex-start; margin-bottom: 25px; }
.titulo-secao > span { background: var(--primary-soft); color: var(--primary); font-size: 11px; border-radius: 8px; padding: 10px; }
.titulo-secao h2 { font-size: 17px; margin: 0 0 5px; }
.titulo-secao p, .ajuda { font-size: 11px; color: var(--muted); margin: 0; line-height: 1.5; }
.ajuda { margin-top: 8px; }
.rodape-formulario { display: flex; justify-content: flex-end; gap: 10px; padding-top: 6px; }
.dialogo { border: 1px solid var(--line); border-radius: 17px; max-width: 470px; width: calc(100% - 32px); padding: 28px; color: var(--ink); box-shadow: 0 18px 80px #25253830; }
.dialogo::backdrop { background: #25253865; backdrop-filter: blur(3px); }
.dialogo p { color: var(--muted); font-size: 13px; }
.dialogo .rodape-formulario { margin-top: 23px; }
.dialogo-icone { width: 40px; height: 40px; display: grid; place-items: center; background: var(--primary-soft); color: var(--primary); border-radius: 12px; margin-bottom: 15px; }
.detalhes-grid { display: grid; grid-template-columns: minmax(0, 1fr) 300px; gap: 24px; align-items: start; }
.pedidos-clientes { display: grid; gap: 22px; }
.pedido-cliente .card-cabecalho { padding: 22px; background: #fff; }
.pedido-cliente.destaque { border-color: #f0afb6; box-shadow: 0 0 0 3px #fff0f1; }
.cliente-contato { padding: 0 22px 16px; font-size: 11px; color: #90909c; line-height: 1.7; }
.pagamentos { padding: 18px 22px; background: #fdfdfd; border-top: 1px solid var(--line); }
.pagamentos h3 { font-size: 12px; color: #646572; margin: 0 0 12px; }
.parcelas-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 10px; }
.parcela { padding: 12px; border: 1px solid var(--line); border-radius: 8px; background: #fff; }
.parcela p { margin: 0 0 4px; font-size: 11px; }
.parcela strong { font-size: 12px; display: block; margin: 6px 0 4px; }
.parcela small { font-size: 10px; color: var(--muted); }
.pedido-totais { display: flex; align-items: center; justify-content: flex-end; gap: 24px; padding: 15px 22px; border-top: 1px solid var(--line); font-size: 12px; }
.pedido-totais span { display: block; color: var(--muted); font-size: 10px; margin-bottom: 4px; }
.resumo-pedido { padding: 23px; position: sticky; top: 20px; }
.resumo-pedido h2 { font-size: 18px; margin-bottom: 22px; }
.resumo-pedido dl { margin: 0; }
.resumo-pedido dt { color: var(--muted); font-size: 11px; margin: 18px 0 5px; }
.resumo-pedido dd { margin: 0; font-size: 13px; }
.resumo-total { border-top: 1px solid var(--line); margin-top: 22px; padding-top: 18px; }
.resumo-total p { color: var(--muted); font-size: 12px; margin-bottom: 5px; }
.resumo-total strong { font-size: 27px; letter-spacing: -1px; }
.resumo-total small { display: block; margin-top: 8px; line-height: 1.5; color: var(--muted); font-size: 11px; }
.barra-confirmacao { margin-top: 25px; padding: 22px; display: flex; align-items: center; justify-content: space-between; gap: 20px; }
.barra-confirmacao p { font-size: 12px; margin: 5px 0 0; }
.pagina-login { background: white; }
.login-grid { min-height: 100vh; display: grid; grid-template-columns: 1.03fr 1fr; }
.login-marca { background: #faf6f5; padding: 40px 10%; display: flex; flex-direction: column; }
.login-marca .marca { align-self: flex-start; }
.login-intro { margin: auto 0; padding: 50px 0; }
.login-intro h1 { font-size: clamp(34px, 4vw, 58px); line-height: 1.1; letter-spacing: -2px; margin: 17px 0 24px; }
.login-intro > p:not(.sobretitulo) { color: #979296; font-size: 15px; line-height: 1.8; }
.login-intro .sobretitulo { color: #b68789; letter-spacing: 1.8px; font-size: 9px; }
.login-rodape { color: #b4a9ad; font-size: 11px; margin: 0; }
.login-formulario { display: grid; place-items: center; padding: 40px; }
.login-conteudo { width: 100%; max-width: 385px; }
.login-conteudo > .ambiente { margin-bottom: 24px; }
.login-conteudo h2 { font-size: 28px; letter-spacing: -.8px; margin-bottom: 10px; }
.login-conteudo > p.muted { font-size: 13px; margin-bottom: 32px; }
.login-conteudo .campo { margin-bottom: 22px; }
.largura-total { width: 100%; }
.senha-campo { position: relative; }
.senha-campo input { padding-right: 73px; }
.senha-campo button { position: absolute; right: 12px; top: 14px; background: none; border: 0; padding: 0; color: #9696a1; font-size: 11px; }
.acesso-demo { margin-top: 26px; padding: 18px; border: 1px solid #ececf1; border-radius: 10px; background: #fcfcfd; }
.acesso-demo strong { font-size: 12px; }
.acesso-demo p { color: var(--muted); font-size: 11px; margin: 5px 0 14px; }
.acesso-demo dl { display: grid; grid-template-columns: 45px 1fr; gap: 8px; font-size: 11px; margin: 0 0 17px; }
.acesso-demo dt { color: var(--muted); }
.acesso-demo dd { margin: 0; font-family: ui-monospace, Consolas, monospace; font-size: 11px; }
.acesso-demo .botao { font-size: 12px; padding: 10px; }
.acesso-demo .link { display: block; margin: 10px auto -5px; font-size: 10px; }
.nota-login { text-align: center; margin: 22px 0 0; color: #aaaab4; font-size: 10px; }
.ilustracao-pedido { margin-top: 34px; background: white; border: 1px solid #efe4e3; box-shadow: 0 16px 50px #68403708; border-radius: 14px; padding: 22px; max-width: 430px; transform: rotate(-2deg); }
.decor-cabecalho { display: flex; align-items: center; gap: 10px; padding-bottom: 20px; border-bottom: 1px solid #f4eeee; }
.decor-cabecalho strong, .decor-linha b { font-size: 11px; font-weight: 600; display: block; }
.decor-cabecalho small, .decor-linha small { font-size: 9px; color: #afa4a6; display: block; margin-top: 4px; }
.decor-icon { width: 30px; height: 30px; border-radius: 9px; background: #e8f7ee; color: #4b9a6f; display: grid; place-items: center; }
.decor-ponto { margin-left: auto; height: 6px; width: 6px; background: #5fa57a; border-radius: 50%; }
.decor-linha { display: flex; gap: 12px; align-items: center; margin: 18px 0; }
.decor-produto { padding: 12px; background: #fff4ee; color: #c6997a; border-radius: 10px; font-size: 11px; }
.decor-progresso { display: flex; gap: 5px; }
.decor-progresso i { height: 4px; flex: 1; background: #ebc6c6; border-radius: 2px; }
.decor-progresso i:last-child { background: #f8efef; }
.decor-rodape { display: flex; justify-content: space-between; font-size: 9px; color: #b0a4a7; margin-top: 13px; }
.decor-rodape b { color: #c97179; font-weight: 500; }
@media (min-width: 1600px) { .container { padding-left: 28px; padding-right: 28px; } }
@media (max-width: 1280px) {
    .submarca, .separador, .nome-conta { display: none; }
    .cabecalho { gap: 17px; }
    .navegacao { gap: 20px; }
    .painel-grid { grid-template-columns: 1fr 1fr; gap: 18px; }
    .dica-tabela { display: none; }
    td, th { padding-left: 10px; padding-right: 10px; }
    .badge { font-size: 9px; }
}
@media (max-width: 1000px) {
    .navegacao .icone { display: none; }
    .cabecalho { padding: 0 24px; }
    .marca { font-size: 22px; }
    .container { padding: 28px 24px; }
    .painel-grid { grid-template-columns: 1fr; }
    .estatisticas { gap: 12px; }
    .estatistica { padding: 15px; gap: 10px; }
    .estatistica .icone-circulo { width: 34px; height: 34px; }
    .estatistica strong { font-size: 20px; }
    .estatistica p { font-size: 10px; }
    .detalhes-grid { grid-template-columns: minmax(0, 1fr) 250px; gap: 16px; }
    .resumo-pedido { padding: 18px; }
}
@media (max-width: 720px) {
    .cabecalho { flex-wrap: wrap; padding: 16px 18px 0; gap: 12px; }
    .barra-marca { flex: 1; }
    .conta { order: 1; }
    .navegacao { width: 100%; order: 2; justify-content: space-between; overflow-x: auto; gap: 23px; }
    .navegacao a { padding: 11px 0; font-size: 11px; }
    .container { padding: 24px 16px; }
    .apresentacao { align-items: flex-start; flex-direction: column; gap: 15px; }
    .apresentacao-acoes { width: 100%; justify-content: space-between; }
    .estatisticas { grid-template-columns: 1fr 1fr; }
    .estatistica { padding: 13px; }
    .estatistica strong { font-size: 21px; }
    .card-cabecalho { padding: 18px 16px 15px; }
    .filtros { padding: 0 16px 15px; flex-wrap: wrap; }
    .filtros select { max-width: none; }
    table { min-width: 510px; }
    .rodape { flex-direction: column; padding: 12px 16px 24px; gap: 8px; }
    .formulario-card { padding: 20px 16px; }
    .campos-grid { grid-template-columns: 1fr; gap: 20px; }
    .detalhes-grid { display: flex; flex-direction: column-reverse; }
    .resumo-pedido { position: static; width: 100%; }
    .resumo-pedido dl { display: grid; grid-template-columns: 110px 1fr; gap: 8px; }
    .resumo-pedido dt, .resumo-pedido dd { margin: 0; }
    .resumo-total { margin-top: 16px; padding-top: 14px; }
    .pedidos-clientes { width: 100%; }
    .barra-confirmacao { flex-direction: column; align-items: stretch; }
    .barra-confirmacao .acoes { justify-content: stretch; }
    .barra-confirmacao .botao { flex: 1; }
    .login-grid { display: block; }
    .login-marca { background: white; padding: 24px 24px 0; }
    .login-intro, .login-rodape { display: none; }
    .login-formulario { padding: 32px 24px; }
    .login-conteudo { max-width: 420px; }
    .login-conteudo h2 { font-size: 26px; }
}
@media (prefers-reduced-motion: reduce) { * { transition: none !important; scroll-behavior: auto !important; } }

/* Financeiro e manutenção de cadastros auxiliares. */
.atalhos { display: flex; justify-content: flex-end; gap: 20px; padding: 10px 3.5%; background: #fff; border-bottom: 1px solid var(--line); font-size: 12px; }
.atalhos a:hover { color: var(--primary); }
.filtros.financeiro { flex-wrap: wrap; align-items: end; }
.financeiro input, .financeiro select { min-height: 39px; }
.auxiliares-grid { display: grid; gap: 22px; }
.dialogo .campo { margin: 16px 0; }
.dialogo .linha-checkbox { display: flex; align-items: center; gap: 10px; }
.dialogo .linha-checkbox input { width: auto; }
````

## dist/estado.html

Formulário de estado e UF.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Novo estado | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/localidade.js"></script>
</head>
<body data-cadastro="estados">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Novo estado</h1><p class="muted">Cadastre o estado e sua unidade federativa.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
<form id="form-localidade" class="formulario formulario-curto"><fieldset id="campos" disabled><section class="card formulario-card"><div class="campos-grid"><label class="campo" for="nome">Nome do estado<input id="nome" name="nome" type="text" required maxlength=100  placeholder=""></label><label class="campo" for="uf">Unidade federativa (UF)<input id="uf" name="uf" type="text" required maxlength=2 pattern="[A-Za-z]{2}" style="text-transform:uppercase" placeholder="PR"></label></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/favicon.svg

Ícone vetorial local da aplicação.

````xml
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 40 40"><rect width="40" height="40" rx="11" fill="#EA1D2C"/><path d="M9 15l11-7 11 7v17H9zM9 21c4-4 7 3 11 0s7 0 11-1M9 27h22M20 8V4" fill="none" stroke="white" stroke-width="2" stroke-linejoin="round"/></svg>
````

## dist/index.html

Estrutura do painel com as quatro listas da loja.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Visão geral | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/painel.js"></script>
</head>
<body >
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <div class="apresentacao"><div><p class="sobretitulo">PAINEL DA LOJA</p><h1>Sua loja, em dia<span class="cor-primaria">.</span></h1><p class="muted">Acompanhe suas vendas e cuide de cada detalhe.</p></div><div class="apresentacao-acoes"><span class="ambiente" data-modo>Demonstração local</span><button id="atualizar" class="botao secundario pequeno">↻ Atualizar</button></div></div>
        <div class="estatisticas" id="estatisticas" aria-label="Resumo da loja"></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <div class="painel-grid">
        <section class="card painel-card" id="vendas" aria-labelledby="titulo-vendas">
            <div class="card-cabecalho"><div><p class="sobretitulo">SEU MOVIMENTO</p><h2 id="titulo-vendas">Vendas <span id="total-vendas" class="contador">0</span></h2></div><button id="abrir-pendentes" class="botao contorno pequeno">Pendentes <span id="total-pendentes" class="numero-botao">0</span></button></div>
            <div class="filtros"><div class="pesquisa"><label class="sr-only" for="pesquisa-vendas">Pesquisar vendas</label><span class="lupa" aria-hidden="true"></span><input id="pesquisa-vendas" type="search" placeholder="Pedido, cliente, vendedor ou status" autocomplete="off"></div>
                <label class="sr-only" for="filtro-vendas">Situação da venda</label><select id="filtro-vendas" aria-label="Situação da venda"><option value="">Todos os status</option value="Confirmado">Confirmadas</option value="Cancelado">Canceladas</option><option value="NaoAtendido">Não atendidas</option></select>
            </div>
            <div class="tabela-container" tabindex="0" role="region" aria-label="Tabela de vendas"><table><caption class="sr-only">Vendas</caption><thead><tr><th scope="col">Pedido / venda</th><th scope="col">Cliente / vendedor</th><th scope="col">Valor</th><th scope="col">Status</th><th scope="col"></th></tr></thead><tbody id="lista-vendas"></tbody></table></div>
            <p id="vazio-vendas" class="vazio" hidden>Nenhum registro encontrado.</p>
            <div class="card-rodape"><span id="resumo-vendas">Carregando…</span><span class="dica-tabela">Pesquise para encontrar mais rápido</span></div>
        </section>        <section class="card painel-card" id="produtos" aria-labelledby="titulo-produtos">
            <div class="card-cabecalho"><div><p class="sobretitulo">SEU CATÁLOGO</p><h2 id="titulo-produtos">Produtos <span id="total-produtos" class="contador">0</span></h2></div><button data-novo="produto" class="botao pequeno">＋ Incluir</button></div>
            <div class="filtros"><div class="pesquisa"><label class="sr-only" for="pesquisa-produtos">Pesquisar produtos</label><span class="lupa" aria-hidden="true"></span><input id="pesquisa-produtos" type="search" placeholder="Nome do produto ou categoria" autocomplete="off"></div>
                
            </div>
            <div class="tabela-container" tabindex="0" role="region" aria-label="Tabela de produtos"><table><caption class="sr-only">Produtos</caption><thead><tr><th scope="col">Produto / categoria</th><th scope="col">Preço</th><th scope="col">Estoque</th><th scope="col">Ações</th></tr></thead><tbody id="lista-produtos"></tbody></table></div>
            <p id="vazio-produtos" class="vazio" hidden>Nenhum registro encontrado.</p>
            <div class="card-rodape"><span id="resumo-produtos">Carregando…</span><span class="dica-tabela">Pesquise para encontrar mais rápido</span></div>
        </section>        <section class="card painel-card" id="vendedores" aria-labelledby="titulo-vendedores">
            <div class="card-cabecalho"><div><p class="sobretitulo">SUA EQUIPE</p><h2 id="titulo-vendedores">Vendedores <span id="total-vendedores" class="contador">0</span></h2></div><button data-novo="vendedor" class="botao secundario pequeno">＋ Cadastrar</button></div>
            <div class="filtros"><div class="pesquisa"><label class="sr-only" for="pesquisa-vendedores">Pesquisar vendedores</label><span class="lupa" aria-hidden="true"></span><input id="pesquisa-vendedores" type="search" placeholder="Nome, documento, e-mail ou cidade" autocomplete="off"></div>
                
            </div>
            <div class="tabela-container" tabindex="0" role="region" aria-label="Tabela de vendedores"><table><caption class="sr-only">Vendedores</caption><thead><tr><th scope="col">ID / nome completo</th><th scope="col">Status</th><th scope="col">Ações</th></tr></thead><tbody id="lista-vendedores"></tbody></table></div>
            <p id="vazio-vendedores" class="vazio" hidden>Nenhum registro encontrado.</p>
            <div class="card-rodape"><span id="resumo-vendedores">Carregando…</span><span class="dica-tabela">Pesquise para encontrar mais rápido</span></div>
        </section>        <section class="card painel-card" id="clientes" aria-labelledby="titulo-clientes">
            <div class="card-cabecalho"><div><p class="sobretitulo">SUA CARTEIRA</p><h2 id="titulo-clientes">Clientes <span id="total-clientes" class="contador">0</span></h2></div><button data-novo="cliente" class="botao secundario pequeno">＋ Cadastrar</button></div>
            <div class="filtros"><div class="pesquisa"><label class="sr-only" for="pesquisa-clientes">Pesquisar clientes</label><span class="lupa" aria-hidden="true"></span><input id="pesquisa-clientes" type="search" placeholder="Nome, documento, endereço ou vendedor" autocomplete="off"></div>
                
            </div>
            <div class="tabela-container" tabindex="0" role="region" aria-label="Tabela de clientes"><table><caption class="sr-only">Clientes</caption><thead><tr><th scope="col">Nome / endereço</th><th scope="col">Vendedor</th><th scope="col">Ações</th></tr></thead><tbody id="lista-clientes"></tbody></table></div>
            <p id="vazio-clientes" class="vazio" hidden>Nenhum registro encontrado.</p>
            <div class="card-rodape"><span id="resumo-clientes">Carregando…</span><span class="dica-tabela">Pesquise para encontrar mais rápido</span></div>
        </section>
        </div>
        <p class="atualizacao" id="atualizado-em" aria-live="polite"></p>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/js/api.js

Cliente HTTP, autenticação, renovação de sessão e saída.

````javascript
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
````

## dist/js/cadastro.js

Comportamento compartilhado dos formulários de produto, vendedor e cliente.

````javascript
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
````

## dist/js/comum.js

Formatação brasileira, elementos reutilizáveis e navegação de retorno.

````javascript
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
````

## dist/js/config.js

Configuração do ambiente demo ou API.

````javascript
// A API serve a WEB no mesmo endereço. Não há dependência de CDN.
// O modo "demo" é opcional e usa somente exemplos locais separados.
export const CONFIG = Object.freeze({
    modo: "api",
    apiUrl: `${location.origin}/api`,
    atualizacaoMs: 30000,
    timeoutMs: 20000
});
````

## dist/js/dados.js

Interface comum de acesso aos dados e notificações entre guias.

````javascript
import { CONFIG } from "./config.js";
import { exigirLogin, requisicao } from "./api.js";
import { demo, CHAVE_DEMO } from "./demo-store.js";
import { novoIdentificador } from "./regras.js";
import { observarTempoReal } from "./tempo-real.js";

export const CHAVE_ATUALIZACAO = `vandercake.web.${CONFIG.modo}.atualizacao.v1`;
const EVENTO = "vandercake:atualizacao";
function notificar() {
    localStorage.setItem(CHAVE_ATUALIZACAO, novoIdentificador());
    window.dispatchEvent(new Event(EVENTO));
}
async function executar(funcaoDemo, caminho, metodo = "GET", corpo) {
    if (!exigirLogin()) throw new Error("Entre na administração.");
    const resultado = CONFIG.modo === "demo" ? await funcaoDemo() : await requisicao(caminho, metodo, corpo);
    if (metodo !== "GET") notificar();
    return resultado;
}
export const repositorio = {
    obterDados: () => executar(() => demo.obterDados(), "/admin/painel"),
    obterLote: id => executar(() => demo.obterLote(id), `/admin/pedidos-gerais/${id}`),
    obterPrevia: id => executar(() => demo.obterPrevia(id), `/admin/pedidos-gerais/${id}/previa`),
    salvar: (tipo, valores, id = null) => executar(() => demo.salvar(tipo, valores, id),
        `/admin/${tipo}${id === null ? "" : `/${id}`}`, id === null ? "POST" : "PUT", valores),
    atividadeVendedor: (id, ativo, versao) => executar(() => demo.atividadeVendedor(id, ativo, versao),
        `/admin/vendedores/${id}/atividade`, "PATCH", { ativo, versao }),
    excluirProduto: (id, versao) => executar(() => demo.excluirProduto(id, versao),
        `/admin/produtos/${id}`, "DELETE", { versao }),
    confirmar: (id, valores) => executar(() => demo.confirmar(id, valores),
        `/admin/pedidos-gerais/${id}/confirmacao`, "POST", valores),
    receberParcela: id => executar(() => { throw new Error("Use a API para registrar recebimentos."); },
        `/admin/contas-receber/${id}/recebimento`, "POST", {}),
    cancelar: (id, valores) => executar(() => demo.cancelar(id, valores),
        `/admin/pedidos-gerais/${id}/cancelamento`, "POST", valores)
};

export function observarDados(callback, periodico = false) {
    let timer;
    const atualizar = () => {
        clearTimeout(timer);
        timer = setTimeout(() => { if (!document.hidden) callback(); }, 150);
    };
    window.addEventListener(EVENTO, atualizar);
    window.addEventListener("storage", e => {
        if (e.key === CHAVE_ATUALIZACAO || (CONFIG.modo === "demo" && e.key === CHAVE_DEMO)) atualizar();
    });
    window.addEventListener("focus", atualizar);
    document.addEventListener("visibilitychange", atualizar);
    observarTempoReal(atualizar);
    if (CONFIG.modo === "api") setInterval(atualizar, CONFIG.atualizacaoMs);
}
````

## dist/js/demo-seed.js

Criação de registros fictícios para explorar a aplicação.

````javascript
import { confirmarLote, gerarPrevia, dividirCentavos } from "./regras.js";

export const CONTA_DEMO = Object.freeze({ email: "admin@vandercake.local", senha: "VanderCake123!" });

export function criarDemonstracao() {
    const data = (dias, hora = 10) => {
        const d = new Date();
        d.setDate(d.getDate() - dias);
        d.setHours(hora, 24, 0, 0);
        return d.toISOString();
    };
    const endereco = { rua: "Rua das Panificadoras", numero: "120", bairro: "Centro", cep: "85851000", cidadeId: 1,
        telefone: "45000000000", ativo: true, versao: 1 };
    const base = {
        schema: 1, revisao: 1,
        estados: [{ id: 1, nome: "Paraná", uf: "PR", versao: 1 }, { id: 2, nome: "Santa Catarina", uf: "SC", versao: 1 }],
        cidades: [{ id: 1, nome: "Foz do Iguaçu", estadoId: 1, versao: 1 }, { id: 2, nome: "Cascavel", estadoId: 1, versao: 1 },
            { id: 3, nome: "Curitiba", estadoId: 1, versao: 1 }, { id: 4, nome: "Joinville", estadoId: 2, versao: 1 }],
        categorias: ["Farinhas", "Chocolates", "Laticínios", "Fermentos", "Confeitaria"].map((nome, i) => ({ id: i + 1, nome, ativo: true, versao: 1 })),
        produtos: [
            { id: 1, nome: "Farinha de trigo · 25 kg", categoriaId: 1, precoCentavos: 8950, estoque: 40, descricao: "Farinha especial para pães e massas." },
            { id: 2, nome: "Chocolate meio amargo · 1 kg", categoriaId: 2, precoCentavos: 3290, estoque: 30, descricao: "Cobertura para a rotina da confeitaria." },
            { id: 3, nome: "Creme de leite · 1 L", categoriaId: 3, precoCentavos: 1890, estoque: 28, descricao: "Creme culinário para doces e recheios." },
            { id: 4, nome: "Fermento biológico · 500 g", categoriaId: 4, precoCentavos: 1290, estoque: 45, descricao: "Fermento seco para panificação." },
            { id: 5, nome: "Açúcar de confeiteiro · 1 kg", categoriaId: 5, precoCentavos: 890, estoque: 36, descricao: "Açúcar fino para coberturas." },
            { id: 6, nome: "Granulado de chocolate · 1 kg", categoriaId: 5, precoCentavos: 2150, estoque: 22, descricao: "Finalização de bolos e doces." }
        ].map(p => ({ ...p, ativo: true, versao: 1 })),
        vendedores: [
            { ...endereco, id: 1, nome: "Ana Martins", documento: "00000000001", email: "ana@exemplo.test" },
            { ...endereco, id: 2, nome: "Rafael Costa", documento: "00000000002", email: "rafael@exemplo.test", cidadeId: 2 },
            { ...endereco, id: 3, nome: "Beatriz Lima", documento: "00000000003", email: "beatriz@exemplo.test", ativo: false }
        ],
        clientes: [
            { ...endereco, id: 10, nome: "Padaria Trigo & Cia", documento: "00000000000010", email: "trigo@exemplo.test", vendedorId: 1 },
            { ...endereco, id: 11, nome: "Confeitaria Doce Ponto", documento: "00000000000011", email: "doceponto@exemplo.test", vendedorId: 1, numero: "284" },
            { ...endereco, id: 12, nome: "Padaria Bom Dia", documento: "00000000000012", email: "bomdia@exemplo.test", vendedorId: 2, cidadeId: 2 },
            { ...endereco, id: 13, nome: "Ateliê de Bolos", documento: "00000000000013", email: "atelie@exemplo.test", vendedorId: 2, cidadeId: 2 },
            { ...endereco, id: 14, nome: "Empório dos Pães", documento: "00000000000014", email: "emporio@exemplo.test", vendedorId: 1 }
        ],
        condicoesPagamento: [{ id: 1, descricao: "À vista", dias: [0] }, { id: 2, descricao: "7/14 dias", dias: [7, 14] },
            { id: 3, descricao: "7/14/21 dias", dias: [7, 14, 21] }],
        formasPagamento: ["Pix", "Boleto bancário", "Dinheiro em espécie", "Cartão de crédito", "Cartão de débito"]
            .map((descricao, i) => ({ id: i + 1, descricao })),
        pedidosGerais: [], vendas: [], contasReceber: []
    };
    let pedidoId = 2031;
    function pedido(clienteId, condicaoId, produtos, dias) {
        const itens = produtos.map(([produtoId, quantidadeSolicitada]) => {
            const produto = base.produtos.find(p => p.id === produtoId);
            return { produtoId, nomeProduto: produto.nome, quantidadeSolicitada, quantidadeAtendida: null, precoUnitarioCentavos: produto.precoCentavos };
        });
        const total = itens.reduce((s, i) => s + i.quantidadeSolicitada * i.precoUnitarioCentavos, 0);
        const condicao = base.condicoesPagamento.find(c => c.id === condicaoId);
        const valores = dividirCentavos(total, condicao.dias.length);
        return { id: pedidoId++, clienteId, clienteNome: base.clientes.find(c => c.id === clienteId).nome,
            dataHoraCriacao: data(dias), status: "Pendente", condicaoPagamentoId: condicaoId,
            condicaoPagamentoDescricao: condicao.descricao, valorTotalSolicitadoCentavos: total,
            valorTotalAtendidoCentavos: null, itens,
            parcelas: condicao.dias.map((prazo, i) => ({ numeroParcela: i + 1, diasAposConfirmacao: prazo,
                formaPagamentoId: i + 1, formaPagamentoNome: base.formasPagamento[i].descricao,
                valorPrevistoCentavos: valores[i], valorFinalCentavos: null, dataVencimento: null })) };
    }
    function lote(id, vendedorId, pedidos, dias) {
        return { id, identificadorEnvio: `00000000-0000-4000-8000-${String(id).padStart(12, "0")}`,
            vendedorId, vendedorNome: base.vendedores.find(v => v.id === vendedorId).nome,
            dataHoraRecebimento: data(dias, 11), dataHoraCriacao: data(dias), dataHoraProcessamento: null,
            status: "AguardandoConfirmacao", motivoCancelamento: null, versao: 1, pedidos };
    }
    const passado = lote(1040, 1, [pedido(10, 3, [[1, 2], [2, 3]], 3), pedido(11, 2, [[3, 4]], 3), pedido(14, 1, [[5, 6]], 3)], 3);
    base.pedidosGerais.push(passado);
    confirmarLote(base, passado.id, gerarPrevia(passado, base.produtos).tokenPrevia, new Date(data(3, 14)));
    const cancelado = lote(1041, 2, [pedido(12, 1, [[4, 8]], 2)], 2);
    cancelado.status = "Cancelado";
    cancelado.motivoCancelamento = "Cliente solicitou o cancelamento antes da confirmação.";
    cancelado.dataHoraProcessamento = data(2, 15);
    cancelado.pedidos[0].status = "Cancelado";
    base.pedidosGerais.push(cancelado,
        lote(1042, 1, [pedido(10, 3, [[1, 10], [2, 5]], 1), pedido(11, 2, [[1, 6], [2, 3]], 1)], 1),
        lote(1043, 2, [pedido(12, 1, [[3, 6], [4, 4]], 0), pedido(13, 3, [[3, 8], [6, 3]], 0)], 0),
        lote(1044, 1, [pedido(14, 2, [[5, 10], [4, 6]], 0)], 0));
    base.produtos[0].estoque = 12;
    base.produtos[1].estoque = 7;
    return base;
}
````

## dist/js/demo-store.js

Persistência da demonstração e transações entre guias no IndexedDB.

````javascript
import { criarDemonstracao } from "./demo-seed.js";
import { copiar, novoIdentificador, gerarPrevia, salvarCadastro, alterarAtividade, excluirProduto, confirmarLote, cancelarLote } from "./regras.js";

export const CHAVE_DEMO = "vandercake.web.demo.dados.v1";
let conexao;
function abrirBanco() {
    if (conexao) return conexao;
    conexao = new Promise((resolve, reject) => {
        const pedido = indexedDB.open("vandercake-web-demonstracao", 1);
        pedido.onupgradeneeded = () => pedido.result.createObjectStore("estado");
        pedido.onsuccess = () => resolve(pedido.result);
        pedido.onerror = () => { conexao = null; reject(new Error("Não foi possível abrir os dados locais da demonstração.")); };
    });
    return conexao;
}

async function transacao(acao, alterar = false, reiniciar = false) {
    const banco = await abrirBanco();
    return new Promise((resolve, reject) => {
        // IndexedDB serializa as escritas entre guias, inclusive em servidores HTTP de desenvolvimento.
        const tx = banco.transaction("estado", "readwrite");
        const store = tx.objectStore("estado");
        const leitura = store.get("atual");
        let resultado;
        let falha;
        leitura.onsuccess = () => {
            try {
                const base = reiniciar || !leitura.result ? criarDemonstracao() : leitura.result;
                if (base.schema !== 1 || !Array.isArray(base.pedidosGerais))
                    throw new Error("Dados locais incompatíveis. Reinicie a demonstração na tela de login.");
                resultado = acao(base);
                if (alterar || !leitura.result || reiniciar) {
                    if (alterar) base.revisao++;
                    store.put(base, "atual");
                }
            } catch (erro) { falha = erro; tx.abort(); }
        };
        tx.oncomplete = () => resolve(copiar(resultado));
        tx.onerror = tx.onabort = () => reject(falha ?? new Error("A alteração local não pôde ser salva. Os dados anteriores foram preservados."));
    });
}
const gravar = acao => transacao(acao, true);
export const iniciarDemo = () => transacao(base => base);
export async function reiniciarDemo() {
    await transacao(base => base, true, true);
    localStorage.setItem(CHAVE_DEMO, novoIdentificador());
}

export const demo = {
    obterDados: iniciarDemo,
    obterLote: async id => {
        const base = await iniciarDemo();
        const lote = base.pedidosGerais.find(l => l.id === id);
        if (!lote) throw new Error("Pedido Geral não encontrado.");
        return { lote, clientes: base.clientes, cidades: base.cidades, estados: base.estados,
            vendas: base.vendas, contasReceber: base.contasReceber };
    },
    obterPrevia: async id => {
        const base = await iniciarDemo();
        const lote = base.pedidosGerais.find(l => l.id === id);
        if (!lote) throw new Error("Pedido Geral não encontrado.");
        return gerarPrevia(lote, base.produtos);
    },
    salvar: (tipo, valores, id) => gravar(base => salvarCadastro(base, tipo, valores, id)),
    atividadeVendedor: (id, ativo, versao) => gravar(base => alterarAtividade(base, id, ativo, versao)),
    excluirProduto: (id, versao) => gravar(base => excluirProduto(base, id, versao)),
    confirmar: (id, valores) => gravar(base => confirmarLote(base, id, valores.tokenPrevia)),
    cancelar: (id, valores) => gravar(base => cancelarLote(base, id, valores.motivo, valores.versao))
};
````

## dist/js/localidade.js

Inclusão de categoria, estado e cidade, com retorno à guia de origem.

````javascript
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
````

## dist/js/login.js

Envio do formulário de login e controles da demonstração.

````javascript
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
````

## dist/js/pagamentos.js

Formulários de cadastros auxiliares com controle de versão.

````javascript
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
````

## dist/js/painel.js

Renderização, pesquisas e ações de cada lista do painel.

````javascript
import { exigirLogin } from "./api.js";
import { dentroPeriodo } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, icone, dinheiro, dataHora, escapar, normalizar, iniciais, endereco, badge,
    abrirJanela, mostrarMensagem, criarConfirmacao } from "./comum.js";

let dados;
let versaoCarregamento = 0;
let ocupado = false;
const janelas = new Set();
const abrir = (url, nome) => { const janela = abrirJanela(url, nome); if (janela) janelas.add(janela); };
const busca = tipo => normalizar($(`#pesquisa-${tipo}`).value);
const pessoa = (nome, sub, cor = "") => `<div class="nome-registro"><span class="avatar ${cor}">${escapar(iniciais(nome))}</span><div><strong>${escapar(nome)}</strong><span class="texto-secundario">${escapar(sub)}</span></div></div>`;
const link = (acao, id, texto, classe = "") => `<button type="button" class="link ${classe}" data-acao="${acao}" data-id="${Number(id)}">${texto}</button>`;

function preencher(tipo, linhas, total) {
    $(`#lista-${tipo}`).innerHTML = linhas.join("");
    $(`#total-${tipo}`).textContent = total;
    $(`#vazio-${tipo}`).hidden = linhas.length > 0;
    $(`#resumo-${tipo}`).textContent = `${linhas.length} de ${total} registros`;
}
function desenhar() {
    if (!dados) return;
    const pendentes = dados.pedidosGerais.filter(l => l.status === "AguardandoConfirmacao");
    const vendas = dados.pedidosGerais.flatMap(l => l.pedidos.filter(p => p.status !== "Pendente").map(p => ({
        ...p, lote: l, venda: dados.vendas.find(v => v.pedidoId === p.id),
        cliente: dados.clientes.find(c => c.id === p.clienteId)
    }))).sort((a, b) => new Date(b.lote.dataHoraProcessamento) - new Date(a.lote.dataHoraProcessamento));
    $("#total-pendentes").textContent = pendentes.length;
    const indicadores = [
        ["bag", "Vendas confirmadas", dados.vendas.length, dinheiro(dados.vendas.reduce((s, v) => s + v.valorTotalCentavos, 0)) + " em vendas"],
        ["clock", "Pedidos pendentes", pendentes.length, "envios para conferir"],
        ["box", "Produtos ativos", dados.produtos.filter(p => p.ativo).length, "disponíveis no catálogo"],
        ["users", "Vendedores ativos", dados.vendedores.filter(v => v.ativo).length, "cuidando da sua carteira"]
    ];
    $("#estatisticas").innerHTML = indicadores.map(([i, titulo, total, detalhe]) => `<div class="estatistica"><span class="icone-circulo">${icone(i)}</span><div><p>${titulo}</p><strong>${total}</strong><small>${escapar(detalhe)}</small></div></div>`).join("");
    const filtro = $("#filtro-vendas").value;
    const listaVendas = vendas.filter(p => dentroPeriodo(p.lote.dataHoraRecebimento, $("#vendas-de").value, $("#vendas-ate").value) && (!filtro || (filtro === "Confirmado" ? p.status.startsWith("Confirmado") : p.status === filtro)) &&
        normalizar(`${p.id} ${p.venda?.id ?? ""} ${p.lote.id} ${p.clienteNome} ${p.lote.vendedorNome} ${p.status} ${p.status.startsWith("Confirmado") ? "Confirmada" : p.status === "Cancelado" ? "Cancelada" : "Não atendida"} ${dataHora(p.dataHoraCriacao)} ${dinheiro(p.venda?.valorTotalCentavos ?? p.valorTotalSolicitadoCentavos)} ${p.cliente?.documento} ${p.cliente?.email} ${endereco(p.cliente, dados)}`).includes(busca("vendas")));
    preencher("vendas", listaVendas.map(p => `<tr><td class="coluna-valor"><strong>#${Number(p.id)}</strong><span class="texto-secundario">${p.venda ? `Venda #${Number(p.venda.id)}` : "Sem venda gerada"}</span></td>
        <td><strong>${escapar(p.clienteNome)}</strong><span class="texto-secundario">${escapar(p.lote.vendedorNome)}</span></td>
        <td class="coluna-valor">${dinheiro(p.venda?.valorTotalCentavos ?? (p.status === "NaoAtendido" ? 0 : p.valorTotalSolicitadoCentavos))}${p.status === "Cancelado" ? '<span class="texto-secundario">Solicitado</span>' : ""}</td>
        <td>${badge(p.status)}</td><td><button type="button" class="link" data-detalhe="${Number(p.lote.id)}" data-pedido="${Number(p.id)}">Ver detalhes</button></td></tr>`), vendas.length);

    const produtos = [...dados.produtos].sort((a, b) => Number(b.ativo) - Number(a.ativo) || a.nome.localeCompare(b.nome, "pt-BR"))
        .map(p => ({ ...p, categoria: dados.categorias.find(c => c.id === p.categoriaId)?.nome ?? "" }));
    preencher("produtos", produtos.filter(p => normalizar(`${p.id} ${p.nome} ${p.categoria} ${p.descricao} ${dinheiro(p.precoCentavos)} ${p.estoque}`).includes(busca("produtos")))
        .map(p => `<tr><td><strong>${escapar(p.nome)}</strong><span class="texto-secundario">${escapar(p.categoria)}${p.ativo ? "" : " · Inativo"}</span></td><td class="coluna-valor">${dinheiro(p.precoCentavos)}</td>
        <td class="coluna-valor ${p.estoque < 10 ? "estoque-baixo" : ""}">${p.estoque} un.</td><td><div class="acoes">${link("produto", p.id, "Alterar")}${p.ativo ? link("excluir", p.id, "Excluir", "discreto divisoria") : ""}</div></td></tr>`), produtos.length);

    preencher("vendedores", dados.vendedores.filter(v => normalizar(`${v.id} ${v.nome} ${v.documento} ${v.email} ${v.telefone} ${v.ativo ? "Ativo" : "Inativo"} ${endereco(v, dados)}`).includes(busca("vendedores")))
        .map(v => `<tr><td>${pessoa(v.nome, `#${v.id} · ${v.email}`, v.ativo ? "" : "neutro")}</td><td>${badge(v.ativo ? "Ativo" : "Inativo")}</td><td><div class="acoes">${link("vendedor", v.id, "Alterar")}${link("atividade", v.id, v.ativo ? "Desligar" : "Reativar", "discreto divisoria")}</div></td></tr>`), dados.vendedores.length);

    preencher("clientes", dados.clientes.map(c => ({ ...c, vendedor: dados.vendedores.find(v => v.id === c.vendedorId)?.nome ?? "" }))
        .filter(c => normalizar(`${c.id} ${c.nome} ${c.documento} ${c.email} ${c.telefone} ${endereco(c, dados)} ${c.vendedor}`).includes(busca("clientes")))
        .map(c => `<tr><td><strong>${escapar(c.nome)}${c.ativo ? "" : " · Inativo"}</strong><span class="texto-secundario">${escapar(endereco(c, dados))}</span></td><td>${escapar(c.vendedor)}</td><td>${link("cliente", c.id, "Alterar")}</td></tr>`), dados.clientes.length);
}
async function carregar() {
    const versao = ++versaoCarregamento;
    $("#atualizar").disabled = true;
    try {
        const novos = await repositorio.obterDados();
        if (versao !== versaoCarregamento) return;
        dados = novos;
        desenhar();
        $("#atualizado-em").textContent = `Atualizado em ${dataHora(new Date().toISOString())}`;
    } catch (erro) { if (versao === versaoCarregamento) mostrarMensagem(erro.message, true); }
    finally { if (versao === versaoCarregamento) $("#atualizar").disabled = false; }
}
async function acaoTabela(evento) {
    const detalhes = evento.target.closest("[data-detalhe]");
    if (detalhes) { abrir(`pedido.html?lote=${Number(detalhes.dataset.detalhe)}&pedido=${Number(detalhes.dataset.pedido)}`, `vandercake-pedido-${detalhes.dataset.pedido}`); return; }
    const botao = evento.target.closest("button[data-acao]");
    if (!botao || !dados || ocupado) return;
    const id = Number(botao.dataset.id);
    const acao = botao.dataset.acao;
    if (["produto", "vendedor", "cliente"].includes(acao)) { abrir(`${acao}.html?id=${id}`, `vandercake-${acao}-${id}`); return; }
    ocupado = true;
    botao.disabled = true;
    try {
        if (acao === "atividade") {
            const vendedor = dados.vendedores.find(v => v.id === id);
            const resposta = await criarConfirmacao({ titulo: vendedor.ativo ? "Desligar vendedor?" : "Reativar vendedor?",
                descricao: vendedor.ativo ? `${vendedor.nome} ficará inativo. A carteira e o histórico serão preservados.` : `${vendedor.nome} voltará a atuar como vendedor.`,
                confirmar: vendedor.ativo ? "Desligar" : "Reativar", perigo: vendedor.ativo });
            if (!resposta) return;
            await repositorio.atividadeVendedor(id, !vendedor.ativo, vendedor.versao);
            mostrarMensagem(vendedor.ativo ? "Vendedor desligado." : "Vendedor reativado.");
        } else if (acao === "excluir") {
            const produto = dados.produtos.find(p => p.id === id);
            const resposta = await criarConfirmacao({ titulo: "Excluir produto?", descricao: `${produto.nome}. Produtos utilizados em pedidos serão inativados para preservar o histórico.`, confirmar: "Excluir", perigo: true });
            if (!resposta) return;
            const resultado = await repositorio.excluirProduto(id, produto.versao);
            mostrarMensagem(resultado.desativado ? "Produto inativado. O histórico foi preservado." : "Produto excluído.");
        }
        await carregar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
    finally { ocupado = false; botao.disabled = false; }
}
if (exigirLogin()) {
    instalarCabecalho();
    document.querySelectorAll('input[type="search"]').forEach(e => e.addEventListener("input", desenhar));
    $("#filtro-vendas").addEventListener("change", desenhar);
    for (const id of ["vendas-de", "vendas-ate"]) $(`#${id}`).addEventListener("input", desenhar);
    $("#atualizar").addEventListener("click", () => { mostrarMensagem(""); carregar(); });
    $("#abrir-pendentes").addEventListener("click", () => abrir("pendentes.html", "vandercake-pendentes"));
    document.querySelectorAll("[data-novo]").forEach(e => e.addEventListener("click", () => abrir(`${e.dataset.novo}.html`, `vandercake-${e.dataset.novo}-novo`)));
    $(".painel-grid").addEventListener("click", acaoTabela);
    window.addEventListener("message", e => {
        if (e.origin === location.origin && janelas.has(e.source) && e.data?.tipo === "vandercake:salvo") {
            mostrarMensagem(String(e.data.mensagem)); carregar();
        }
    });
    observarDados(carregar, true);
    carregar();
}
````

## dist/js/pedido.js

Carregamento da prévia, detalhes e decisão da loja.

````javascript
import { exigirLogin } from "./api.js";
import { CONFIG } from "./config.js";
import { novoIdentificador } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, obterId, dinheiro, dataHora, dataCurta, escapar, badge, endereco, mostrarMensagem, criarConfirmacao } from "./comum.js";
let id;
let foco;
let dados;
let previa;
let ocupado = false;
let leitura = 0;

function desenhar() {
    const lote = dados.lote;
    const pendente = lote.status === "AguardandoConfirmacao";
    const cancelado = lote.status === "Cancelado";
    const pedidos = pendente ? previa.pedidos : lote.pedidos;
    $("#titulo-pagina").textContent = `Pedido Geral #${lote.id}`;
    document.title = `Pedido Geral #${lote.id} | VanderCake`;
    $("#subtitulo-pedido").textContent = `${lote.vendedorNome} · Recebido em ${dataHora(lote.dataHoraRecebimento)}`;
    $("#conteudo-pedido").hidden = false;
    $("#acoes-pedido").hidden = !pendente;
    $("#aviso-pedido").hidden = true;
    if (cancelado) {
        $("#aviso-pedido").hidden = false;
        $("#aviso-pedido").textContent = `Pedido cancelado: ${lote.motivoCancelamento}. Nenhuma venda ou baixa de estoque foi gerada.`;
    } else if (pendente) {
        $("#aviso-pedido").hidden = false;
        $("#aviso-pedido").textContent = previa.unidadesCortadas > 0 ?
            `Estoque insuficiente: a prévia distribui os produtos proporcionalmente entre os clientes, com ${previa.unidadesCortadas} unidade(s) de corte. Confira antes de confirmar.` :
            "O estoque atende este envio. A prévia não reserva produtos; as quantidades serão conferidas novamente na confirmação.";
    }
    $("#pedidos-clientes").innerHTML = pedidos.map(p => {
        const cliente = dados.clientes.find(c => c.id === p.clienteId);
        const venda = dados.vendas.find(v => v.pedidoId === p.id);
        return `<section class="card pedido-cliente ${foco === p.id ? "destaque" : ""}" id="pedido-${Number(p.id)}">
            <div class="card-cabecalho"><div><p class="sobretitulo">PEDIDO #${Number(p.id)}${venda ? ` · VENDA #${Number(venda.id)}` : ""}</p><h2>${escapar(p.clienteNome)}</h2></div>${badge(p.status)}</div>
            <div class="cliente-contato"><div>${escapar(cliente?.documento ?? "")} · ${escapar(cliente?.telefone ?? "")} · ${escapar(cliente?.email ?? "")}</div><div>${escapar(endereco(cliente, dados))}</div></div>
            <div class="tabela-container"><table><caption class="sr-only">Itens do pedido de ${escapar(p.clienteNome)}</caption><thead><tr><th>Produto</th><th class="numero">Solicitado</th><th class="numero">${pendente ? "Prévia" : "Atendido"}</th><th class="numero">Corte</th><th class="numero">Preço un.</th><th class="numero">Subtotal ${pendente ? "previsto" : "final"}</th></tr></thead><tbody>
            ${p.itens.map(i => `<tr><td><strong>${escapar(i.nomeProduto)}</strong></td><td class="numero">${i.quantidadeSolicitada}</td><td class="numero">${i.quantidadeAtendida ?? "—"}</td><td class="numero ${i.quantidadeAtendida !== null && i.quantidadeAtendida < i.quantidadeSolicitada ? "perigo-texto" : ""}">${i.quantidadeAtendida === null ? "—" : i.quantidadeSolicitada - i.quantidadeAtendida}</td><td class="numero coluna-valor">${dinheiro(i.precoUnitarioCentavos)}</td><td class="numero coluna-valor">${dinheiro(i.quantidadeAtendida === null ? null : i.quantidadeAtendida * i.precoUnitarioCentavos)}</td></tr>`).join("")}</tbody></table></div>
            <div class="pagamentos"><h3>Pagamento · ${escapar(p.condicaoPagamentoDescricao)}</h3><div class="parcelas-grid">${p.parcelas.map(parcela => {
                const conta = dados.contasReceber.find(c => c.pedidoId === p.id && c.numeroParcela === parcela.numeroParcela);
                return `<div class="parcela"><p>${parcela.numeroParcela}ª parcela · ${parcela.diasAposConfirmacao === 0 ? "À vista" : `${parcela.diasAposConfirmacao} dias`}</p><strong>${escapar(parcela.formaPagamentoNome)}</strong><p>${pendente ? "Prévia" : cancelado ? "Solicitado" : "Valor final"}: <b>${dinheiro(cancelado ? parcela.valorPrevistoCentavos : parcela.valorFinalCentavos)}</b></p><small>Original: ${dinheiro(parcela.valorPrevistoCentavos)}</small><br><small>${cancelado ? "Cancelada · sem cobrança" : `Vencimento: ${parcela.dataVencimento ? dataCurta(parcela.dataVencimento) : p.status === "NaoAtendido" ? "Sem cobrança" : "após confirmação"}`}</small>${conta ? `<br><small>${conta.status === "Paga" ? "Recebida" : "A receber"}</small>` : ""}</div>`;
            }).join("")}</div></div>
            <div class="pedido-totais"><div><span>Total solicitado</span>${dinheiro(p.valorTotalSolicitadoCentavos)}</div><div><span>${pendente ? "Total da prévia" : "Total confirmado"}</span><strong>${dinheiro(p.valorTotalAtendidoCentavos)}</strong></div></div>
        </section>`;
    }).join("");
    const solicitado = pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0);
    const final = cancelado ? null : pedidos.reduce((s, p) => s + (p.valorTotalAtendidoCentavos ?? 0), 0);
    $("#resumo-pedido").innerHTML = `<h2>Resumo do envio</h2>${badge(lote.status)}<dl><dt>Vendedor</dt><dd>${escapar(lote.vendedorNome)}</dd><dt>Clientes neste envio</dt><dd>${pedidos.length}</dd><dt>Recebido em</dt><dd>${dataHora(lote.dataHoraRecebimento)}</dd><dt>Total solicitado</dt><dd>${dinheiro(solicitado)}</dd>
        ${!pendente ? `<dt>${cancelado ? "Cancelado" : "Confirmado"} em</dt><dd>${dataHora(lote.dataHoraProcessamento)}</dd>` : ""}</dl><div class="resumo-total"><p>${pendente ? "Total da prévia" : "Total confirmado"}</p><strong>${dinheiro(final)}</strong><small>${pendente ? "As formas de pagamento de cada parcela serão mantidas após o corte." : cancelado ? "Cancelamento registrado sem movimentar estoque." : "Valores de atendimento registrados pela loja."}</small></div>`;
}
async function carregar(forcar = false) {
    if (ocupado && !forcar) return;
    const versao = ++leitura;
    $("#atualizar").disabled = true;
    $("#confirmar").disabled = true;
    $("#cancelar").disabled = true;
    try {
        const novos = await repositorio.obterLote(id);
        if (foco && !novos.lote.pedidos.some(p => p.id === foco)) throw new Error("Este pedido não pertence ao envio informado.");
        const novaPrevia = novos.lote.status === "AguardandoConfirmacao" ? await repositorio.obterPrevia(id) : null;
        if (versao !== leitura) return;
        dados = novos;
        previa = novaPrevia;
        desenhar();
        $("#confirmar").disabled = ocupado;
        $("#cancelar").disabled = ocupado;
    } catch (erro) { if (versao === leitura) mostrarMensagem(erro.message, true); }
    finally { if (versao === leitura) $("#atualizar").disabled = ocupado; }
}
function identificadorOperacao(acao, conteudo) {
    const chave = `vandercake.web.${CONFIG.modo}.operacao.${acao}.${id}`;
    const assinatura = JSON.stringify(conteudo);
    let anterior;
    try { anterior = JSON.parse(sessionStorage.getItem(chave) ?? "null"); } catch { anterior = null; }
    if (anterior?.assinatura === assinatura) return anterior.id;
    const registro = { assinatura, id: novoIdentificador() };
    sessionStorage.setItem(chave, JSON.stringify(registro));
    return registro.id;
}
async function processar(acao) {
    if (ocupado || !dados || dados.lote.status !== "AguardandoConfirmacao") return;
    ocupado = true;
    $("#confirmar").disabled = true;
    $("#cancelar").disabled = true;
    $("#atualizar").disabled = true;
    try {
        const confirmar = acao === "confirmar";
        const resposta = await criarConfirmacao(confirmar ? {
            titulo: "Confirmar este Pedido Geral?", descricao: `${dados.lote.pedidos.length} pedido(s), total de ${dinheiro(previa.totalAtendidoCentavos)} e ${previa.unidadesCortadas} unidade(s) de corte. O estoque será baixado pelas quantidades atendidas.`, confirmar: "Confirmar envio"
        } : { titulo: "Cancelar este Pedido Geral?", descricao: "Todos os pedidos deste envio serão cancelados. O estoque será mantido.", confirmar: "Cancelar pedidos", perigo: true, motivo: true });
        if (!resposta) return;
        const corpo = confirmar ? { tokenPrevia: previa.tokenPrevia } : { motivo: resposta.motivo, versao: dados.lote.versao };
        corpo.identificadorOperacao = identificadorOperacao(acao, corpo);
        await (confirmar ? repositorio.confirmar(id, corpo) : repositorio.cancelar(id, corpo));
        mostrarMensagem(confirmar ? "Envio confirmado. Os resultados já aparecem na tela de Vendas." : "Envio cancelado. O histórico foi atualizado.");
        await carregar(true);
    } catch (erro) {
        await carregar(true);
        mostrarMensagem(erro.message + (erro.status === 409 ? " Revise os dados atualizados antes de continuar." : ""), true);
    } finally {
        ocupado = false;
        $("#atualizar").disabled = false;
        $("#confirmar").disabled = !previa;
        $("#cancelar").disabled = !dados || dados.lote.status !== "AguardandoConfirmacao";
    }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    try {
        id = obterId("lote");
        foco = obterId("pedido", false);
        $("#atualizar").addEventListener("click", () => carregar());
        $("#confirmar").addEventListener("click", () => processar("confirmar"));
        $("#cancelar").addEventListener("click", () => processar("cancelar"));
        observarDados(() => carregar(), true);
        carregar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
````

## dist/js/pendentes.js

Busca e exibição dos lotes aguardando decisão.

````javascript
import { exigirLogin } from "./api.js";
import { dentroPeriodo } from "./regras.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, escapar, normalizar, dinheiro, dataHora, badge, abrirJanela, mostrarMensagem, iniciais } from "./comum.js";
let lotes = [];
let versao = 0;
function desenhar() {
    const busca = normalizar($("#pesquisa").value);
    const lista = lotes.filter(l => dentroPeriodo(l.dataHoraRecebimento, $("#periodo-de").value, $("#periodo-ate").value) && normalizar(`${l.id} ${l.vendedorNome} ${dataHora(l.dataHoraRecebimento)} ${l.pedidos.map(p => p.clienteNome).join(" ")}`).includes(busca));
    $("#total").textContent = lotes.length;
    $("#vazio").hidden = lista.length > 0;
    $("#lista-pendentes").innerHTML = lista.map(l => `<tr><td><strong>#${Number(l.id)}</strong><span class="texto-secundario">${dataHora(l.dataHoraRecebimento)}</span></td><td><div class="nome-registro"><span class="avatar">${escapar(iniciais(l.vendedorNome))}</span><strong>${escapar(l.vendedorNome)}</strong></div></td>
        <td><strong>${l.pedidos.length} ${l.pedidos.length === 1 ? "cliente" : "clientes"}</strong><span class="texto-secundario">${escapar(l.pedidos.map(p => p.clienteNome).join(" · "))}</span></td>
        <td class="coluna-valor">${dinheiro(l.pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0))}</td><td>${badge(l.status)}</td><td><button class="botao secundario pequeno" data-lote="${Number(l.id)}">Ver detalhes →</button></td></tr>`).join("");
}
async function carregar() {
    const atual = ++versao;
    $("#atualizar").disabled = true;
    try {
        const dados = await repositorio.obterDados();
        if (atual !== versao) return;
        lotes = dados.pedidosGerais.filter(l => l.status === "AguardandoConfirmacao").sort((a, b) => new Date(a.dataHoraRecebimento) - new Date(b.dataHoraRecebimento));
        desenhar();
        $("#atualizado-em").textContent = `Atualizado em ${dataHora(new Date().toISOString())}`;
    } catch (erro) { mostrarMensagem(erro.message, true); }
    finally { if (atual === versao) $("#atualizar").disabled = false; }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    $("#pesquisa").addEventListener("input", desenhar);
    for (const id of ["periodo-de", "periodo-ate"]) $(`#${id}`).addEventListener("input", desenhar);
    $("#atualizar").addEventListener("click", carregar);
    $("#lista-pendentes").addEventListener("click", e => {
        const botao = e.target.closest("[data-lote]");
        if (botao) abrirJanela(`pedido.html?lote=${Number(botao.dataset.lote)}`, `vandercake-lote-${botao.dataset.lote}`);
    });
    observarDados(carregar, true);
    carregar();
}
````

## dist/js/recebimentos.js

Filtros, recebimento de parcelas e atualização dos dados.

````javascript
import { exigirLogin } from "./api.js";
import { CONFIG } from "./config.js";
import { repositorio, observarDados } from "./dados.js";
import { $, instalarCabecalho, dinheiro, dataCurta, escapar, normalizar, mostrarMensagem, criarConfirmacao, abrirJanela } from "./comum.js";

let dados;
let linhas = [];
let versao = 0;
let ocupado = false;
function desenhar() {
    const termo = normalizar($("#pesquisa").value);
    const de = $("#de").value;
    const ate = $("#ate").value;
    const situacao = $("#situacao").value;
    const visiveis = linhas.filter(c => (!situacao || c.status === situacao) && (!de || c.dataVencimento >= de) &&
        (!ate || c.dataVencimento <= ate) && normalizar(`${c.clienteNome} ${c.documento} ${c.vendedorNome} ${c.pedidoId}`).includes(termo));
    $("#total").textContent = visiveis.length;
    $("#total-valor").textContent = `Total filtrado: ${dinheiro(visiveis.reduce((s, c) => s + c.valorCentavos, 0))}`;
    $("#vazio").hidden = visiveis.length > 0;
    $("#parcelas").innerHTML = visiveis.map(c => `<tr>
        <td><strong>${escapar(c.clienteNome)}</strong><span class="texto-secundario">${escapar(c.vendedorNome)}</span></td>
        <td><button class="link" data-detalhe="${c.id}">Pedido #${c.pedidoId}</button><span class="texto-secundario">${c.numeroParcela}ª parcela</span></td>
        <td>${dataCurta(c.dataVencimento)}</td><td>${escapar(c.forma)}</td><td class="coluna-valor">${dinheiro(c.valorCentavos)}</td>
        <td><span class="badge ${c.status === "Paga" ? "verde" : "amarelo"}">${c.status === "Paga" ? "Recebida" : "A receber"}</span>${c.dataPagamento ? `<span class="texto-secundario">${dataCurta(c.dataPagamento)}</span>` : ""}</td>
        <td>${c.status === "Pendente" ? `<button class="botao secundario pequeno" data-receber="${c.id}" ${ocupado ? "disabled" : ""}>Registrar recebimento</button>` : ""}</td>
        </tr>`).join("");
}
async function carregar() {
    const atual = ++versao;
    try {
        const resposta = await repositorio.obterDados();
        if (atual !== versao) return;
        dados = resposta;
        const pedidos = new Map(dados.pedidosGerais.flatMap(l => l.pedidos.map(p => [p.id, { ...p, loteId: l.id, vendedorNome: l.vendedorNome }])));
        linhas = dados.contasReceber.map(c => {
            const p = pedidos.get(c.pedidoId);
            return { ...c, loteId: p.loteId, clienteNome: p.clienteNome, vendedorNome: p.vendedorNome,
                documento: dados.clientes.find(x => x.id === p.clienteId)?.documento ?? "",
                forma: dados.formasPagamento.find(f => f.id === c.formaPagamentoId)?.descricao ?? "" };
        }).sort((a, b) => a.dataVencimento.localeCompare(b.dataVencimento) || a.id - b.id);
        desenhar();
    } catch (erro) { mostrarMensagem(erro.message, true); }
}
if (exigirLogin()) {
    instalarCabecalho("vendas");
    if (CONFIG.modo !== "api") mostrarMensagem("Esta tela usa os recebimentos registrados na API da loja.", true);
    else {
        for (const id of ["pesquisa", "de", "ate", "situacao"]) $(`#${id}`).addEventListener("input", desenhar);
        $("#atualizar").addEventListener("click", carregar);
        $("#parcelas").addEventListener("click", async evento => {
            const detalhe = evento.target.closest("[data-detalhe]");
            if (detalhe) {
                const contaDetalhe = linhas.find(c => c.id === Number(detalhe.dataset.detalhe));
                if (contaDetalhe) abrirJanela(`pedido.html?lote=${contaDetalhe.loteId}&pedido=${contaDetalhe.pedidoId}`, `vandercake-pedido-${contaDetalhe.pedidoId}`);
                return;
            }
            const botao = evento.target.closest("[data-receber]");
            if (!botao || ocupado) return;
            const conta = linhas.find(c => c.id === Number(botao.dataset.receber));
            if (!conta || conta.status !== "Pendente") return;
            ocupado = true;
            desenhar();
            try {
                const confirmado = await criarConfirmacao({ titulo: "Registrar recebimento integral?",
                    descricao: `${conta.clienteNome}: ${dinheiro(conta.valorCentavos)}, ${conta.numeroParcela}ª parcela, ${conta.forma}. Confirme somente se o valor já foi recebido. A data registrada será a de hoje.`, confirmar: "Registrar recebimento" });
                if (!confirmado) return;
                await repositorio.receberParcela(conta.id);
                await carregar();
                mostrarMensagem("Recebimento integral registrado.");
            } catch (erro) { mostrarMensagem(erro.message, true); }
            finally { ocupado = false; desenhar(); }
        });
        observarDados(carregar);
        carregar();
    }
}
````

## dist/js/regras.js

Validações e cálculos do simulador, isolados das telas e do armazenamento.

````javascript
// Regras puras do simulador local. Na integração real, a API é a autoridade.
export const LIMITE_CENTAVOS = 999999999999;
export const normalizar = valor => String(valor ?? "").normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "").toLowerCase().trim();
export const digitos = valor => String(valor ?? "").replace(/\D/g, "");
export const copiar = valor => structuredClone(valor);
export function dentroPeriodo(valor, inicio, fim) {
    if (!inicio && !fim) return true;
    const data = new Date(valor);
    if (!Number.isFinite(data.getTime())) return false;
    const partes = Object.fromEntries(new Intl.DateTimeFormat("pt-BR", {
        timeZone: "America/Sao_Paulo", year: "numeric", month: "2-digit", day: "2-digit"
    }).formatToParts(data).map(p => [p.type, p.value]));
    const dia = `${partes.year}-${partes.month}-${partes.day}`;
    return (!inicio || dia >= inicio) && (!fim || dia <= fim);
}

export function novoIdentificador() {
    if (typeof crypto.randomUUID === "function") return crypto.randomUUID();
    const bytes = crypto.getRandomValues(new Uint8Array(16));
    bytes[6] = (bytes[6] & 15) | 64;
    bytes[8] = (bytes[8] & 63) | 128;
    const hex = [...bytes].map(b => b.toString(16).padStart(2, "0")).join("");
    return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`;
}

export class Conflito extends Error {
    constructor(mensagem) { super(mensagem); this.name = "Conflito"; this.status = 409; }
}

export function lerCentavos(texto) {
    const valor = String(texto).trim();
    if (!/^\d+(?:[,.]\d{1,2})?$/.test(valor))
        throw new Error("Informe o preço como 49,90, sem separador de milhar.");
    const [inteiros, fracao = ""] = valor.replace(",", ".").split(".");
    const centavos = Number(inteiros) * 100 + Number(fracao.padEnd(2, "0"));
    if (!Number.isSafeInteger(centavos) || centavos < 0 || centavos > LIMITE_CENTAVOS)
        throw new Error("Preço fora do limite permitido.");
    return centavos;
}

export function dividirCentavos(total, quantidade) {
    if (!Number.isSafeInteger(total) || total < 0 || total > LIMITE_CENTAVOS ||
        ![1, 2, 3].includes(quantidade)) throw new Error("Total ou parcelas inválidos.");
    const parte = Math.floor(total / quantidade);
    return Array.from({ length: quantidade }, (_, i) =>
        i === quantidade - 1 ? total - parte * (quantidade - 1) : parte);
}

export function ratear(estoque, demandas) {
    if (!Number.isSafeInteger(estoque) || estoque < 0 || demandas.length === 0 ||
        demandas.some(d => !Number.isSafeInteger(d.quantidade) || d.quantidade <= 0) ||
        new Set(demandas.map(d => d.clienteId)).size !== demandas.length)
        throw new Error("Estoque ou demanda inválidos.");
    const total = demandas.reduce((soma, d) => soma + BigInt(d.quantidade), 0n);
    const saldo = BigInt(estoque) < total ? BigInt(estoque) : total;
    const quotas = demandas.map(d => ({
        clienteId: d.clienteId,
        quantidade: Number(saldo * BigInt(d.quantidade) / total),
        resto: saldo * BigInt(d.quantidade) % total
    }));
    let restantes = Number(saldo) - quotas.reduce((soma, q) => soma + q.quantidade, 0);
    const prioridade = [...quotas].sort((a, b) =>
        a.resto === b.resto ? a.clienteId - b.clienteId : a.resto > b.resto ? -1 : 1);
    for (const quota of prioridade) {
        if (restantes-- <= 0) break;
        quota.quantidade++;
    }
    return quotas.map(({ clienteId, quantidade }) => ({ clienteId, quantidade }));
}

function somarItens(itens, campo) {
    const total = itens.reduce((soma, item) => {
        if (!Number.isSafeInteger(item.precoUnitarioCentavos) || item.precoUnitarioCentavos <= 0)
            throw new Error("Preço inválido no pedido recebido.");
        return soma + BigInt(item[campo]) * BigInt(item.precoUnitarioCentavos);
    }, 0n);
    if (total > BigInt(LIMITE_CENTAVOS)) throw new Error("Total do pedido fora do limite.");
    return Number(total);
}

export function gerarPrevia(lote, produtos) {
    if (lote.status !== "AguardandoConfirmacao") throw new Error("Este lote não está pendente.");
    if (!lote.pedidos.length || new Set(lote.pedidos.map(p => p.clienteId)).size !== lote.pedidos.length)
        throw new Error("O lote deve possuir um pedido por cliente.");
    const pedidos = copiar(lote.pedidos);
    const ids = [...new Set(pedidos.flatMap(p => p.itens.map(i => i.produtoId)))].sort((a, b) => a - b);
    const estoqueUsado = [];
    for (const pedido of pedidos) {
        if (!pedido.itens.length || new Set(pedido.itens.map(i => i.produtoId)).size !== pedido.itens.length)
            throw new Error("Itens inválidos no pedido recebido.");
        const prazos = pedido.parcelas.map(p => p.diasAposConfirmacao).join("/");
        if (!["0", "7/14", "7/14/21"].includes(prazos) || pedido.parcelas.some((p, i) =>
            p.numeroParcela !== i + 1 || !Number.isInteger(p.formaPagamentoId) || p.formaPagamentoId <= 0))
            throw new Error("Pagamento inválido no pedido recebido.");
    }
    for (const id of ids) {
        const produto = produtos.find(p => p.id === id);
        if (!produto) throw new Error("Produto do pedido não encontrado no estoque.");
        const demandas = pedidos.filter(p => p.itens.some(i => i.produtoId === id)).map(p => ({
            clienteId: p.clienteId,
            quantidade: p.itens.find(i => i.produtoId === id).quantidadeSolicitada
        }));
        const distribuicao = ratear(produto.estoque, demandas);
        estoqueUsado.push([id, produto.estoque, produto.versao]);
        for (const pedido of pedidos) {
            const item = pedido.itens.find(i => i.produtoId === id);
            if (item) item.quantidadeAtendida = distribuicao.find(d => d.clienteId === pedido.clienteId).quantidade;
        }
    }
    for (const pedido of pedidos) {
        pedido.valorTotalSolicitadoCentavos = somarItens(pedido.itens, "quantidadeSolicitada");
        pedido.valorTotalAtendidoCentavos = somarItens(pedido.itens, "quantidadeAtendida");
        const valores = dividirCentavos(pedido.valorTotalAtendidoCentavos, pedido.parcelas.length);
        pedido.parcelas.forEach((p, i) => { p.valorFinalCentavos = valores[i]; });
    }
    return {
        // Em produção, este token será emitido e validado pelo servidor.
        tokenPrevia: JSON.stringify([lote.id, lote.versao, lote.pedidos, estoqueUsado]),
        pedidos,
        totalSolicitadoCentavos: pedidos.reduce((s, p) => s + p.valorTotalSolicitadoCentavos, 0),
        totalAtendidoCentavos: pedidos.reduce((s, p) => s + p.valorTotalAtendidoCentavos, 0),
        unidadesCortadas: pedidos.reduce((s, p) => s + p.itens.reduce((n, i) =>
            n + i.quantidadeSolicitada - i.quantidadeAtendida, 0), 0)
    };
}

export const proximoId = lista => Math.max(0, ...lista.map(i => i.id)) + 1;
const encontrarLote = (base, id) => base.pedidosGerais.find(l => l.id === id) ??
    (() => { throw new Error("Pedido Geral não encontrado."); })();

export function adicionarDias(data, dias) {
    const dia = new Date(`${data}T12:00:00Z`);
    dia.setUTCDate(dia.getUTCDate() + dias);
    return dia.toISOString().slice(0, 10);
}

export function confirmarLote(base, id, token, agora = new Date()) {
    const lote = encontrarLote(base, id);
    if (lote.status === "Processado") return lote; // Uma confirmação já feita não baixa novamente.
    const previa = gerarPrevia(lote, base.produtos);
    if (previa.tokenPrevia !== token)
        throw new Conflito("O estoque mudou. Confira a nova prévia antes de confirmar.");
    const dataConfirmacao = agora.toISOString();
    const dataBase = new Intl.DateTimeFormat("en-CA", { timeZone: "America/Sao_Paulo" }).format(agora);
    const vendas = [];
    const contas = [];
    let vendaId = proximoId(base.vendas);
    let contaId = proximoId(base.contasReceber);
    for (const pedido of previa.pedidos) {
        const corte = pedido.itens.some(i => i.quantidadeAtendida < i.quantidadeSolicitada);
        pedido.status = pedido.valorTotalAtendidoCentavos === 0 ? "NaoAtendido" :
            corte ? "ConfirmadoComCorte" : "Confirmado";
        pedido.parcelas.forEach(p => {
            p.dataVencimento = pedido.valorTotalAtendidoCentavos > 0 ? adicionarDias(dataBase, p.diasAposConfirmacao) : null;
        });
        if (pedido.valorTotalAtendidoCentavos === 0) continue;
        const venda = { id: vendaId++, pedidoId: pedido.id, dataHoraConfirmacao: dataConfirmacao,
            valorTotalCentavos: pedido.valorTotalAtendidoCentavos,
            itens: pedido.itens.filter(i => i.quantidadeAtendida > 0).map(i => copiar(i)) };
        vendas.push(venda);
        for (const parcela of pedido.parcelas.filter(p => p.valorFinalCentavos > 0)) {
            contas.push({ id: contaId++, vendaId: venda.id, pedidoId: pedido.id,
                numeroParcela: parcela.numeroParcela, formaPagamentoId: parcela.formaPagamentoId,
                dataVencimento: parcela.dataVencimento, valorCentavos: parcela.valorFinalCentavos, status: "Pendente" });
        }
    }
    // Todas as validações e contas são feitas antes de alterar o estado.
    for (const produto of base.produtos) {
        const quantidade = previa.pedidos.flatMap(p => p.itens).filter(i => i.produtoId === produto.id)
            .reduce((s, i) => s + i.quantidadeAtendida, 0);
        if (quantidade) { produto.estoque -= quantidade; produto.versao++; }
    }
    lote.pedidos = previa.pedidos;
    lote.status = "Processado";
    lote.dataHoraProcessamento = dataConfirmacao;
    lote.administradorNome = "Administrador de demonstração";
    lote.versao++;
    base.vendas.push(...vendas);
    base.contasReceber.push(...contas);
    return lote;
}

export function cancelarLote(base, id, motivo, versao, agora = new Date()) {
    const lote = encontrarLote(base, id);
    if (lote.status === "Cancelado") return lote;
    if (lote.status !== "AguardandoConfirmacao") throw new Conflito("Este lote já foi processado.");
    if (lote.versao !== versao) throw new Conflito("O pedido mudou. Atualize os detalhes.");
    const texto = String(motivo ?? "").trim();
    if (!texto || texto.length > 255) throw new Error("Informe o motivo do cancelamento, com até 255 caracteres.");
    lote.status = "Cancelado";
    lote.motivoCancelamento = texto;
    lote.dataHoraProcessamento = agora.toISOString();
    lote.administradorNome = "Administrador de demonstração";
    lote.versao++;
    lote.pedidos.forEach(p => { p.status = "Cancelado"; });
    return lote;
}

function texto(valor, campo, max, opcional = false) {
    const resultado = String(valor ?? "").trim();
    if ((!opcional && !resultado) || resultado.length > max)
        throw new Error(`${campo}: preencha com até ${max} caracteres.`);
    return resultado;
}
function inteiro(valor, campo, minimo = 1) {
    if (!Number.isSafeInteger(valor) || valor < minimo || valor > 2147483647)
        throw new Error(`${campo}: informe um número inteiro válido.`);
    return valor;
}
function unico(lista, campo, valor, id, nome = campo) {
    if (lista.some(x => x.id !== id && normalizar(x[campo]) === normalizar(valor)))
        throw new Error(`Já existe um cadastro com este ${nome}.`);
}

export function salvarCadastro(base, tipo, entrada, id = null) {
    const lista = base[tipo];
    if (!["produtos", "vendedores", "clientes", "categorias", "estados", "cidades"].includes(tipo))
        throw new Error("Cadastro inválido.");
    const atual = id === null ? null : lista.find(x => x.id === id);
    if (id !== null && !atual) throw new Error("Cadastro não encontrado.");
    if (atual && atual.versao !== entrada.versao)
        throw new Conflito("Este cadastro mudou em outra tela. Reabra-o antes de salvar.");
    let valores = {};
    if (tipo === "estados") {
        const nome = texto(entrada.nome, "Nome do estado", 100);
        const uf = texto(entrada.uf, "UF", 2).toUpperCase();
        if (!/^[A-Z]{2}$/.test(uf)) throw new Error("A UF deve conter duas letras.");
        unico(lista, "uf", uf, id, "UF");
        valores = { nome, uf };
    } else if (tipo === "cidades") {
        const nome = texto(entrada.nome, "Nome da cidade", 100);
        const estadoId = inteiro(entrada.estadoId, "Estado");
        if (!base.estados.some(e => e.id === estadoId)) throw new Error("Selecione um estado cadastrado.");
        unico(lista.filter(c => c.estadoId === estadoId), "nome", nome, id, "nome de cidade neste estado");
        valores = { nome, estadoId };
    } else if (tipo === "categorias") {
        const nome = texto(entrada.nome, "Categoria", 60);
        unico(lista, "nome", nome, id, "nome de categoria");
        valores = { nome, ativo: true };
    } else if (tipo === "produtos") {
        const nome = texto(entrada.nome, "Nome do produto", 100);
        const categoriaId = inteiro(entrada.categoriaId, "Categoria");
        if (!base.categorias.some(c => c.id === categoriaId && c.ativo)) throw new Error("Selecione uma categoria ativa.");
        const precoCentavos = entrada.precoCentavos;
        if (!Number.isSafeInteger(precoCentavos) || precoCentavos <= 0 || precoCentavos > LIMITE_CENTAVOS)
            throw new Error("Informe um preço positivo com até duas casas decimais.");
        valores = { nome, categoriaId, precoCentavos,
            estoque: inteiro(entrada.estoque, "Estoque", 0),
            descricao: texto(entrada.descricao, "Descrição", 4000, true), ativo: atual ? Boolean(entrada.ativo) : true };
    } else {
        const nome = texto(entrada.nome, "Nome", 100);
        const documento = digitos(entrada.documento);
        if (!(tipo === "clientes" ? [11, 14] : [11]).includes(documento.length))
            throw new Error(tipo === "clientes" ? "Informe CPF com 11 ou CNPJ com 14 dígitos." : "Informe o CPF com 11 dígitos.");
        if (lista.some(p => p.id !== id && digitos(p.documento) === documento))
            throw new Error("Este documento já está cadastrado.");
        const cidadeId = inteiro(entrada.cidadeId, "Cidade");
        if (!base.cidades.some(c => c.id === cidadeId)) throw new Error("Selecione uma cidade cadastrada.");
        const cep = digitos(entrada.cep);
        if (cep.length !== 8) throw new Error("Informe um CEP com 8 dígitos.");
        const telefone = digitos(entrada.telefone);
        if (telefone.length < 10 || telefone.length > 13) throw new Error("Informe um telefone com DDD.");
        const email = texto(entrada.email, "E-mail", 150, tipo === "clientes").toLowerCase();
        if (email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) throw new Error("Informe um e-mail válido.");
        valores = { nome, documento, cidadeId, cep, telefone, email,
            rua: texto(entrada.rua, "Rua", 100), numero: texto(entrada.numero, "Número", 10),
            bairro: texto(entrada.bairro, "Bairro", 60), ativo: atual?.ativo ?? true };
        if (tipo === "vendedores") {
            unico(lista, "email", email, id, "e-mail");
            if ((!atual || entrada.senha) && (String(entrada.senha ?? "").length < 8 || entrada.senha.length > 128))
                throw new Error("A senha deve ter de 8 a 128 caracteres.");
            // O simulador não armazena senhas de vendedores. O hash pertence à futura API.
        } else {
            const vendedorId = inteiro(entrada.vendedorId, "Vendedor");
            const vendedor = base.vendedores.find(v => v.id === vendedorId);
            if (!vendedor || (!vendedor.ativo && atual?.vendedorId !== vendedorId))
                throw new Error("Selecione um vendedor ativo.");
            valores.vendedorId = vendedorId;
            valores.ativo = atual ? Boolean(entrada.ativo) : true;
        }
    }
    const registro = { ...valores, id: atual?.id ?? proximoId(lista), versao: (atual?.versao ?? 0) + 1 };
    if (atual) lista[lista.indexOf(atual)] = registro;
    else lista.push(registro);
    return registro;
}

export function alterarAtividade(base, id, ativo, versao) {
    const vendedor = base.vendedores.find(v => v.id === id);
    if (!vendedor) throw new Error("Vendedor não encontrado.");
    if (vendedor.versao !== versao) throw new Conflito("O cadastro mudou. Atualize o painel.");
    vendedor.ativo = Boolean(ativo);
    vendedor.versao++;
    return vendedor;
}

export function excluirProduto(base, id, versao) {
    const produto = base.produtos.find(p => p.id === id);
    if (!produto) throw new Error("Produto não encontrado.");
    if (produto.versao !== versao) throw new Conflito("O produto mudou. Atualize o painel.");
    const utilizado = base.pedidosGerais.some(l => l.pedidos.some(p => p.itens.some(i => i.produtoId === id)));
    if (utilizado) { produto.ativo = false; produto.versao++; }
    else base.produtos = base.produtos.filter(p => p.id !== id);
    return { desativado: utilizado };
}
````

## dist/js/tempo-real.js

SignalR, reconexão e avisos às páginas.

````javascript
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
````

## dist/login.html

Formulário de entrada e acesso de demonstração.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <title>Entrar na loja | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/login.js"></script>
</head>
<body class="pagina-login">
    <main class="login-grid">
        <section class="login-marca">
            <a class="marca" href="login.html"><img src="favicon.svg" width="36" height="36" alt=""><span>Vander<span class="marca-forte">Cake</span></span></a>
            <div class="login-intro"><p class="sobretitulo">DA SUA LOJA PARA CADA CLIENTE</p><h1>Bons negócios.<br>Todos os dias<span class="cor-primaria">.</span></h1><p>Mais organização para sua equipe.<br>Mais cuidado com cada pedido.</p>
                <div class="ilustracao-pedido" aria-hidden="true"><div class="decor-cabecalho"><span class="decor-icon">✓</span><div><strong>Tudo pronto para o próximo pedido</strong><small>VanderCake · Gestão da loja</small></div><span class="decor-ponto"></span></div>
                <div class="decor-linha"><span class="decor-produto">01</span><div><b>Da carteira à venda confirmada</b><small>Clientes, produtos e equipe em um só lugar.</small></div></div>
                <div class="decor-progresso"><i></i><i></i><i></i><i></i></div><div class="decor-rodape"><span>Uma rotina mais simples</span><b>É por aqui →</b></div></div>
            </div>
            <p class="login-rodape">Distribuição para panificadoras e confeitarias.</p>
        </section>
        <section class="login-formulario">
            <div class="login-conteudo"><span class="ambiente" data-demo>Demonstração local</span><h2>Bem-vindo à sua loja</h2><p class="muted">Entre com sua conta de administrador.</p>
                <p id="mensagem" class="mensagem" role="status" hidden></p>
                <form id="form-login"><fieldset id="campos">
                    <label class="campo" for="email">E-mail<input id="email" name="email" type="email" required maxlength="150" autocomplete="username" placeholder="Seu e-mail de acesso"></label>
                    <label class="campo" for="senha">Senha<span class="senha-campo"><input id="senha" name="senha" type="password" required maxlength="128" autocomplete="current-password" placeholder="Sua senha"><button id="ver-senha" type="button" aria-pressed="false">Mostrar</button></span></label>
                    <button id="entrar" class="botao largura-total" type="submit">Entrar na loja</button>
                </fieldset></form>
                <div class="acesso-demo" data-demo><div><strong>Explore a demonstração</strong><p>Dados fictícios salvos apenas neste navegador.</p></div><dl><dt>E-mail</dt><dd>admin@vandercake.local</dd><dt>Senha</dt><dd>VanderCake123!</dd></dl><button type="button" id="preencher-demo" class="botao secundario largura-total">Preencher acesso de teste</button><button type="button" id="reiniciar-demo" class="link discreto">Reiniciar demonstração</button></div>
                <p class="nota-login">Acesso exclusivo à administração VanderCake.</p>
            </div>
        </section>
    </main>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/pagamentos.html

Manutenção de categorias, condições e formas de pagamento.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Cadastros auxiliares | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/pagamentos.js"></script>
</head>
<body >
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a>
        <div class="apresentacao"><div><p class="sobretitulo">CADASTROS</p><h1>Cadastros auxiliares</h1><p class="muted">Mantenha categorias, condições e formas de pagamento.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <p class="muted">Condições e formas já utilizadas preservam descrição e prazos. Você pode desativá-las ou cadastrar outra opção.</p>
        <div id="cadastros-auxiliares" class="auxiliares-grid"></div>
    </main>
    <footer class="rodape"><span>VanderCake · Gestão comercial</span></footer>
</body>
</html>
````

## dist/pedido.html

Estrutura dos detalhes e das ações sobre o envio.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Detalhes do pedido | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/pedido.js"></script>
</head>
<body >
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a>
        <div class="apresentacao"><div><p class="sobretitulo">REVISÃO DO ENVIO</p><h1 id="titulo-pagina">Detalhes do pedido</h1><p id="subtitulo-pedido" class="muted">Carregando informações…</p></div><div class="apresentacao-acoes"><span data-modo class="ambiente"></span><button id="atualizar" class="botao secundario pequeno">↻ Atualizar</button></div></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <div id="conteudo-pedido" hidden><div id="aviso-pedido" class="aviso" hidden></div><div class="detalhes-grid"><div id="pedidos-clientes" class="pedidos-clientes"></div><aside class="card resumo-pedido" id="resumo-pedido" aria-label="Resumo do Pedido Geral"></aside></div>
        <section id="acoes-pedido" class="card barra-confirmacao" hidden><div><strong>Confira o envio completo</strong><p class="muted">A confirmação considera todos os clientes deste Pedido Geral.</p></div><div class="acoes"><button id="cancelar" class="botao secundario perigo-texto">Cancelar</button><button id="confirmar" class="botao">Confirmar</button></div></section></div>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/pendentes.html

Estrutura da lista de Pedidos Gerais pendentes.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Pedidos pendentes | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/pendentes.js"></script>
</head>
<body >
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a>
        <div class="apresentacao"><div><p class="sobretitulo">RECEBIDOS DOS VENDEDORES</p><h1>Pedidos pendentes</h1><p class="muted">Revise os pedidos de cada envio antes de confirmar a venda.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <section class="card"><div class="card-cabecalho"><div><h2>Envios para conferir <span id="total" class="contador">0</span></h2><p class="muted">Cada envio reúne os pedidos dos clientes de um vendedor.</p></div><button id="atualizar" class="botao secundario pequeno">↻ Atualizar</button></div>
        <div class="filtros"><div class="pesquisa"><label class="sr-only" for="pesquisa">Pesquisar pedidos pendentes</label><span class="lupa" aria-hidden="true"></span><input id="pesquisa" type="search" placeholder="Número do envio, vendedor ou cliente"></div></div>
        <div class="tabela-container"><table><caption class="sr-only">Pedidos Gerais aguardando confirmação</caption><thead><tr><th>Pedido Geral</th><th>Vendedor</th><th>Clientes</th><th>Total solicitado</th><th>Situação</th><th></th></tr></thead><tbody id="lista-pendentes"></tbody></table></div>
        <div id="vazio" class="vazio" hidden><h3>Tudo em dia por aqui</h3><p>Nenhum pedido pendente encontrado.</p></div></section>
        <p class="atualizacao" id="atualizado-em"></p>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/produto.html

Formulário de produto com categoria, preço e estoque.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Incluir produto | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/cadastro.js"></script>
</head>
<body data-cadastro="produtos">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Incluir produto</h1><p class="muted">Mantenha seu catálogo pronto para novos pedidos.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <form id="form-cadastro" class="formulario"><fieldset id="campos" disabled><section class="card formulario-card"><div class="titulo-secao"><span>01</span><div><h2>Informações do produto</h2><p>Dados que os vendedores verão no catálogo.</p></div></div><div class="campos-grid"><label class="campo" for="nome">Nome do produto<input id="nome" name="nome" type="text" required maxlength=100  placeholder="Ex.: Farinha de trigo · 25 kg"></label><div class="campo"><label for="categoria">Categoria / grupo</label><div class="picker-linha"><select id="categoria" name="categoria" required><option value="">Selecione</option></select><button type="button" class="botao secundario pequeno" id="nova-categoria">Nova Categoria+</button></div></div><label class="campo" for="preco">Preço unitário (R$)<input id="preco" name="preco" type="text" required maxlength=15 inputmode="decimal" placeholder="0,00"></label><label class="campo" for="estoque">Estoque (unidades)<input id="estoque" name="estoque" type="number" required  min="0" max="2147483647" step="1" placeholder="0"></label><label class="campo coluna-inteira" for="descricao">Descrição <span class="opcional">opcional</span><textarea id="descricao" name="descricao" rows="4" maxlength="4000" placeholder="Descreva o produto"></textarea></label>
            <label class="checkbox coluna-inteira" id="linha-ativo" hidden><input type="checkbox" id="ativo" checked> Produto ativo no catálogo</label></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## dist/recebimentos.html

Consulta e baixa integral manual de parcelas.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Contas a receber | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/recebimentos.js"></script>
</head>
<body >
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a>
        <div class="apresentacao"><div><p class="sobretitulo">FINANCEIRO</p><h1>Contas a receber</h1><p class="muted">Consulte as parcelas e registre o recebimento integral de cada uma.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <section class="card">
            <div class="card-cabecalho"><div><h2>Parcelas <span id="total" class="contador">0</span></h2><p id="total-valor" class="muted"></p></div><button id="atualizar" class="botao secundario pequeno">↻ Atualizar</button></div>
            <div class="filtros financeiro">
                <label class="campo">Cliente ou vendedor<input id="pesquisa" type="search" placeholder="Nome, documento ou pedido"></label>
                <label class="campo">Vencimento de<input id="de" type="date"></label>
                <label class="campo">Vencimento até<input id="ate" type="date"></label>
                <label class="campo">Situação<select id="situacao"><option value="">Todas</option value="Pendente" selected>A receber</option><option value="Paga">Recebidas</option></select></label>
            </div>
            <div class="tabela-container"><table><caption class="sr-only">Parcelas das vendas</caption><thead><tr><th>Cliente / vendedor</th><th>Pedido / parcela</th><th>Vencimento</th><th>Forma</th><th>Valor</th><th>Situação</th><th></th></tr></thead><tbody id="parcelas"></tbody></table></div>
            <div id="vazio" class="vazio" hidden><h3>Nenhuma parcela encontrada</h3><p>Confira os filtros ou confirme um pedido na loja.</p></div>
        </section>
    </main>
    <footer class="rodape"><span>VanderCake · Gestão comercial</span></footer>
</body>
</html>
````

## dist/vendedor.html

Formulário de vendedor, credenciais e endereço.

````html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="theme-color" content="#EA1D2C">
    <meta name="description" content="VanderCake: gestão de vendas, produtos e carteiras de clientes.">
    <title>Cadastrar vendedor | VanderCake</title>
    <link rel="icon" href="favicon.svg" type="image/svg+xml">
    <link rel="stylesheet" href="css/estilos.css">
    <script type="module" src="js/cadastro.js"></script>
</head>
<body data-cadastro="vendedores">
    <a class="pular" href="#principal">Pular para o conteúdo</a>
    <header id="cabecalho" class="cabecalho"></header>
    <main id="principal" class="container">
        <a class="voltar" href="index.html">← Voltar ao painel</a><div class="apresentacao"><div><p class="sobretitulo">CADASTROS DA LOJA</p><h1 id="titulo-pagina">Cadastrar vendedor</h1><p class="muted">Uma nova pessoa para cuidar dos seus clientes.</p></div><span class="ambiente" data-modo></span></div>
        <p id="mensagem" class="mensagem" role="status" hidden></p>
        <form id="form-cadastro" class="formulario"><fieldset id="campos" disabled><section class="card formulario-card"><div class="titulo-secao"><span>01</span><div><h2>Identificação e contato</h2><p>Preencha os campos para identificar o cadastro.</p></div></div><div class="campos-grid"><label class="campo" for="nome">Nome completo<input id="nome" name="nome" type="text" required maxlength=100  placeholder="Como devemos identificar este cadastro?"></label><label class="campo" for="documento">CPF<input id="documento" name="documento" type="text" required maxlength=20 inputmode="numeric" placeholder="Somente números ou com pontuação"></label><label class="campo" for="email">E-mail de acesso<input id="email" name="email" type="email" required maxlength=150 autocomplete="off" placeholder="nome@exemplo.com"></label><label class="campo" for="telefone">Telefone com DDD<input id="telefone" name="telefone" type="tel" required maxlength=20  placeholder="(45) 99999-9999"></label><div class="coluna-inteira"><label class="campo" for="senha">Senha de acesso<input id="senha" name="senha" type="password" required maxlength=128 minlength="8" autocomplete="new-password" placeholder="No mínimo 8 caracteres"></label><p id="ajuda-senha" class="ajuda">O vendedor utilizará e-mail e senha para acessar o Mobile.</p></div></div></section><section class="card formulario-card"><div class="titulo-secao"><span>02</span><div><h2>Endereço</h2><p>Selecione o estado para encontrar as cidades vinculadas.</p></div></div><div class="campos-grid"><div class="campo"><label for="estado">Estado</label><div class="picker-linha"><select id="estado" name="estado" required><option value="">Selecione</option></select><button type="button" class="botao secundario pequeno" id="novo-estado">Novo Estado+</button></div></div><div class="campo"><label for="cidade">Cidade</label><div class="picker-linha"><select id="cidade" name="cidade" required><option value="">Selecione</option></select><button type="button" class="botao secundario pequeno" id="nova-cidade">Nova Cidade+</button></div></div><label class="campo" for="rua">Rua / logradouro<input id="rua" name="rua" type="text" required maxlength=100  placeholder="Nome da rua"></label><label class="campo" for="numero">Número<input id="numero" name="numero" type="text" required maxlength=10  placeholder="120 ou S/N"></label><label class="campo" for="bairro">Bairro<input id="bairro" name="bairro" type="text" required maxlength=60  placeholder="Bairro"></label><label class="campo" for="cep">CEP<input id="cep" name="cep" type="text" required maxlength=9 inputmode="numeric" placeholder="00000-000"></label></div></section><div class="rodape-formulario"><button type="button" data-voltar class="botao secundario">Voltar</button><button id="salvar" type="submit" class="botao">Salvar cadastro</button></div></fieldset></form>
    </main>
    <footer class="rodape"><span>VanderCake <span class="muted">· Gestão comercial</span></span><span data-demo>Dados fictícios · Demonstração neste navegador</span></footer>
    <noscript>Ative o JavaScript para utilizar o VanderCake.</noscript>
</body>
</html>
````

## package.json

Metadados e comandos do projeto; SignalR é distribuído localmente em dist/vendor.

````json
{
  "name": "vandercake-web",
  "version": "1.0.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "node scripts/serve.mjs",
    "test": "node --test tests/*.test.js"
  }
}
````

## scripts/documentar.mjs

Gera este guia a partir dos arquivos reais, preservando o código integral.

````javascript
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
````

## scripts/serve.mjs

Servidor de desenvolvimento com proxy HTTP/WebSocket para a API local.

````javascript
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
````

## tests/regras.test.js

Cenários automatizados para as regras do domínio.

````javascript
import test from "node:test";
import assert from "node:assert/strict";
import { criarDemonstracao } from "../dist/js/demo-seed.js";
import { lerCentavos, dividirCentavos, ratear, gerarPrevia, confirmarLote, cancelarLote,
    salvarCadastro, alterarAtividade, excluirProduto, adicionarDias, Conflito, dentroPeriodo } from "../dist/js/regras.js";

test("Filtro por período usa o dia da loja, inclusive na virada UTC", () => {
    assert.equal(dentroPeriodo("2026-10-27T01:00:00Z", "2026-10-26", "2026-10-26"), true);
    assert.equal(dentroPeriodo("2026-10-27T01:00:00Z", "2026-10-27", "2026-10-27"), false);
    assert.equal(dentroPeriodo("2026-10-27T12:00:00Z", "2026-10-27", "2026-10-27"), true);
    assert.equal(dentroPeriodo("inválida", "2026-10-27", ""), false);
});

const lote = base => base.pedidosGerais.find(l => l.id === 1042);
const confirmar = base => confirmarLote(base, 1042, gerarPrevia(lote(base), base.produtos).tokenPrevia,
    new Date("2026-10-24T14:00:00Z"));
const vendedor = () => ({ nome: "Vendedor de teste", documento: "12345678901", email: "teste@exemplo.test",
    telefone: "45999999999", cidadeId: 1, cep: "85851000", rua: "Rua Teste", numero: "1", bairro: "Centro", senha: "Teste123!" });

test("Dinheiro usa centavos, aceita vírgula e rejeita precisão extra", () => {
    assert.equal(lerCentavos("89,50"), 8950);
    assert.equal(lerCentavos("0.01"), 1);
    for (const valor of ["1.999", "-1", "1.000,00", "abc", "10000000000.00"]) assert.throws(() => lerCentavos(valor));
});
test("Divisão mantém o total e coloca a diferença na última parcela", () => {
    assert.deepEqual(dividirCentavos(10000, 3), [3333, 3333, 3334]);
    assert.deepEqual(dividirCentavos(1, 3), [0, 0, 1]);
    assert.throws(() => dividirCentavos(100, 4));
});
test("Rateio proporcional reproduz 10/6/4 com estoque 11", () => {
    assert.deepEqual(ratear(11, [{ clienteId: 1, quantidade: 10 }, { clienteId: 2, quantidade: 6 }, { clienteId: 3, quantidade: 4 }]),
        [{ clienteId: 1, quantidade: 6 }, { clienteId: 2, quantidade: 3 }, { clienteId: 3, quantidade: 2 }]);
});
test("Desempate favorece menor ID e não a ordem da lista", () => {
    assert.deepEqual(ratear(1, [{ clienteId: 2, quantidade: 1 }, { clienteId: 1, quantidade: 1 }]),
        [{ clienteId: 2, quantidade: 0 }, { clienteId: 1, quantidade: 1 }]);
});
test("Rateio conserva estoque mesmo com demandas altas", () => {
    const resultado = ratear(2147483647, [{ clienteId: 1, quantidade: 2147483647 }, { clienteId: 2, quantidade: 2147483647 }]);
    assert.equal(resultado.reduce((s, q) => s + q.quantidade, 0), 2147483647);
    assert.equal(resultado[0].quantidade, resultado[1].quantidade + 1);
    assert.equal(ratear(0, [{ clienteId: 1, quantidade: 20 }])[0].quantidade, 0);
    assert.equal(ratear(30, [{ clienteId: 1, quantidade: 20 }])[0].quantidade, 20);
});
test("Prévia não altera pedidos nem reserva estoque", () => {
    const base = criarDemonstracao();
    const antes = structuredClone(base);
    const previa = gerarPrevia(lote(base), base.produtos);
    assert.deepEqual(base, antes);
    assert.equal(previa.unidadesCortadas, 5);
    assert.deepEqual(previa.pedidos.map(p => p.itens[0].quantidadeAtendida), [8, 4]);
});
test("Confirmação baixa apenas o atendido e preserva as formas por parcela", () => {
    const base = criarDemonstracao();
    const original = structuredClone(lote(base));
    const vendasAntes = base.vendas.length;
    const resultado = confirmar(base);
    assert.equal(resultado.status, "Processado");
    assert.equal(base.produtos[0].estoque, 0);
    assert.equal(base.produtos[1].estoque, 0);
    assert.equal(base.vendas.length, vendasAntes + 2);
    for (let i = 0; i < resultado.pedidos.length; i++) {
        const p = resultado.pedidos[i];
        assert.equal(p.status, "ConfirmadoComCorte");
        assert.deepEqual(p.parcelas.map(x => x.formaPagamentoId), original.pedidos[i].parcelas.map(x => x.formaPagamentoId));
        assert.deepEqual(p.itens.map(x => x.quantidadeSolicitada), original.pedidos[i].itens.map(x => x.quantidadeSolicitada));
        const contas = base.contasReceber.filter(c => c.pedidoId === p.id);
        assert.equal(contas.reduce((s, c) => s + c.valorCentavos, 0), p.valorTotalAtendidoCentavos);
        assert.ok(contas.every(c => c.status === "Pendente"));
    }
});
test("Confirmação repetida não duplica vendas, parcelas ou baixas", () => {
    const base = criarDemonstracao();
    const token = gerarPrevia(lote(base), base.produtos).tokenPrevia;
    confirmarLote(base, 1042, token);
    const antes = structuredClone(base);
    confirmarLote(base, 1042, token);
    assert.deepEqual(base, antes);
});
test("Prévia desatualizada recusa toda a confirmação", () => {
    const base = criarDemonstracao();
    const token = gerarPrevia(lote(base), base.produtos).tokenPrevia;
    base.produtos[0].estoque--;
    const antes = structuredClone(base);
    assert.throws(() => confirmarLote(base, 1042, token), Conflito);
    assert.deepEqual(base, antes);
});
test("Estoque zero processa sem criar venda ou conta a receber", () => {
    const base = criarDemonstracao();
    base.produtos[0].estoque = 0;
    base.produtos[1].estoque = 0;
    const vendas = base.vendas.length;
    const contas = base.contasReceber.length;
    const resultado = confirmar(base);
    assert.ok(resultado.pedidos.every(p => p.status === "NaoAtendido"));
    assert.equal(base.vendas.length, vendas);
    assert.equal(base.contasReceber.length, contas);
});
test("Cancelamento exige motivo e conserva estoque, vendas e parcelas", () => {
    const base = criarDemonstracao();
    assert.throws(() => cancelarLote(base, 1042, " ", 1));
    const antes = structuredClone(base);
    const resultado = cancelarLote(base, 1042, "Pedido duplicado", 1);
    assert.equal(resultado.status, "Cancelado");
    assert.ok(resultado.pedidos.every(p => p.status === "Cancelado" && p.valorTotalAtendidoCentavos === null));
    assert.deepEqual(base.produtos, antes.produtos);
    assert.deepEqual(base.vendas, antes.vendas);
    assert.deepEqual(base.contasReceber, antes.contasReceber);
    const cancelado = structuredClone(base);
    cancelarLote(base, 1042, "Outro motivo", 1);
    assert.deepEqual(base, cancelado);
    assert.throws(() => confirmarLote(base, 1042, "token"));
});
test("Venda confirmada não pode ser cancelada pelo fluxo de pendentes", () => {
    const base = criarDemonstracao();
    confirmar(base);
    const antes = structuredClone(base);
    assert.throws(() => cancelarLote(base, 1042, "Cancelar", lote(base).versao), Conflito);
    assert.deepEqual(base, antes);
});
test("Produtos inativados depois do envio mantêm atendimento e preço histórico", () => {
    const base = criarDemonstracao();
    const preco = lote(base).pedidos[0].itens[0].precoUnitarioCentavos;
    base.produtos[0].ativo = false;
    base.produtos[0].precoCentavos = 99900;
    confirmar(base);
    assert.equal(lote(base).pedidos[0].itens[0].precoUnitarioCentavos, preco);
    assert.equal(lote(base).pedidos[0].itens[0].quantidadeAtendida, 8);
});
test("Vencimentos usam dias corridos e cruzam o mês corretamente", () => {
    const base = criarDemonstracao();
    const resultado = confirmar(base);
    assert.deepEqual(resultado.pedidos[0].parcelas.map(p => p.dataVencimento), ["2026-10-31", "2026-11-07", "2026-11-14"]);
    assert.equal(adicionarDias("2028-02-28", 1), "2028-02-29");
});
test("Vendedor começa ativo e a demonstração não guarda senha", () => {
    const base = criarDemonstracao();
    const v = salvarCadastro(base, "vendedores", { ...vendedor(), ativo: false });
    assert.equal(v.ativo, true);
    assert.ok(!Object.hasOwn(v, "senha"));
    assert.ok(!JSON.stringify(base).includes("Teste123!"));
});
test("Documentos e e-mails não podem duplicar com outra formatação", () => {
    const base = criarDemonstracao();
    salvarCadastro(base, "vendedores", vendedor());
    assert.throws(() => salvarCadastro(base, "vendedores", { ...vendedor(), documento: "123.456.789-01", email: "outro@exemplo.test" }), /documento/);
    assert.throws(() => salvarCadastro(base, "vendedores", { ...vendedor(), documento: "98765432100", email: "TESTE@EXEMPLO.TEST" }), /e-mail/);
});
test("Desligar vendedor conserva carteira e autoria dos pedidos", () => {
    const base = criarDemonstracao();
    const clientes = structuredClone(base.clientes);
    const pedidos = structuredClone(base.pedidosGerais);
    alterarAtividade(base, 1, false, 1);
    assert.equal(base.vendedores[0].ativo, false);
    assert.deepEqual(base.clientes, clientes);
    assert.deepEqual(base.pedidosGerais, pedidos);
    alterarAtividade(base, 1, true, 2);
    assert.equal(base.vendedores[0].ativo, true);
});
test("Novo cliente exige vendedor ativo; edição mantém vínculo inativo atual", () => {
    const base = criarDemonstracao();
    const entrada = { ...vendedor(), vendedorId: 3, ativo: true };
    assert.throws(() => salvarCadastro(base, "clientes", entrada), /vendedor ativo/);
    alterarAtividade(base, 1, false, 1);
    const c = base.clientes[0];
    const salvo = salvarCadastro(base, "clientes", { ...c, nome: "Nome atualizado" }, c.id);
    assert.equal(salvo.vendedorId, 1);
    assert.throws(() => salvarCadastro(base, "clientes", { ...salvo, vendedorId: 3 }, salvo.id), /vendedor ativo/);
});
test("UF é única e cidades precisam de um estado válido", () => {
    const base = criarDemonstracao();
    assert.throws(() => salvarCadastro(base, "estados", { nome: "Duplicado", uf: "pr" }), /UF/);
    assert.throws(() => salvarCadastro(base, "cidades", { nome: "Cidade", estadoId: 999 }));
    assert.throws(() => salvarCadastro(base, "cidades", { nome: "FOZ DO IGUAÇU", estadoId: 1 }), /cidade/);
    const cidade = salvarCadastro(base, "cidades", { nome: "Foz do Iguaçu", estadoId: 2 });
    assert.equal(cidade.estadoId, 2);
});
test("Cadastro rejeita categoria inexistente, preço zero e estoque fracionário", () => {
    const base = criarDemonstracao();
    const p = { nome: "Produto novo", categoriaId: 1, precoCentavos: 100, estoque: 3, descricao: "" };
    for (const alteracao of [{ categoriaId: 999 }, { precoCentavos: 0 }, { estoque: 1.5 }, { estoque: -1 }])
        assert.throws(() => salvarCadastro(base, "produtos", { ...p, ...alteracao }));
    assert.equal(salvarCadastro(base, "produtos", p).estoque, 3);
});
test("Histórico impede exclusão física de produtos utilizados", () => {
    const base = criarDemonstracao();
    assert.equal(excluirProduto(base, 1, base.produtos[0].versao).desativado, true);
    assert.ok(base.produtos.find(p => p.id === 1));
    const novo = salvarCadastro(base, "produtos", { nome: "Novo", categoriaId: 1, precoCentavos: 100, estoque: 0 });
    assert.equal(excluirProduto(base, novo.id, novo.versao).desativado, false);
    assert.ok(!base.produtos.find(p => p.id === novo.id));
});
test("Formulário antigo não sobrescreve estoque alterado por confirmação", () => {
    const base = criarDemonstracao();
    const antigo = structuredClone(base.produtos[0]);
    confirmar(base);
    assert.throws(() => salvarCadastro(base, "produtos", antigo, antigo.id), Conflito);
    assert.equal(base.produtos[0].estoque, 0);
});
test("Estado serializado conserva pagamentos independentes e histórico", () => {
    const base = criarDemonstracao();
    confirmar(base);
    const restaurado = JSON.parse(JSON.stringify(base));
    assert.deepEqual(restaurado, base);
    assert.deepEqual(lote(restaurado).pedidos[0].parcelas.map(p => p.formaPagamentoNome), ["Pix", "Boleto bancário", "Dinheiro em espécie"]);
});
````

## Referências

Consulte README.md, docs/CONTRATO_API.md e os documentos do pacote integrado em ../Docs. Os arquivos acima usam a API real por padrão.
