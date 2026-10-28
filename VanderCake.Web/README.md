# VanderCake.Web integrado

A interface usa HTML, CSS e JavaScript e se conecta à VanderCake.Api deste pacote. O modo padrão é `api`.

## Executar

Siga `../TUTORIAL_WINDOWS.md` para instalar o MySQL, configurar a conexão e criar o administrador. Inicie a API pelo script `../Scripts/04-Iniciar-API.ps1` e abra `http://localhost:5080`.

A própria API serve `dist/` e os endpoints no mesmo endereço; não precisa de Node.js para usar a WEB. Após editar arquivos da interface, reinicie a API para atualizar a cópia dos arquivos na saída de compilação.

Cadastre vendedores, clientes e produtos. Os vendedores criados aqui podem entrar no MOBILE. Os pedidos recebidos aparecem em Pendentes; confirmar registra as vendas e cancelar preserva o histórico sem baixar estoque.

## Recebimentos e cadastros auxiliares

Use os atalhos abaixo do cabeçalho para consultar parcelas e registrar recebimentos integrais, ou manter categorias/condições/formas. A baixa registra a data e o administrador e atualiza o MOBILE. Não há processamento bancário, baixa parcial ou estorno. Condições/formas já utilizadas preservam descrição/prazos e podem ser desativadas. Vendas e Pendentes também possuem filtros por período de recebimento do lote.

## Atualizações

`dist/js/tempo-real.js` recebe avisos SignalR, reconecta e dispara novas consultas. Existe recuperação a cada 30 segundos e ao voltar à guia. Formulários mantêm os valores em edição; versões impedem sobrescrever alterações concorrentes.

`dist/js/api.js` guarda a sessão administrativa no sessionStorage e renova o acesso quando necessário. Sair limpa a sessão e avisa as outras guias. Não há senha padrão na base real.

## Demonstração opcional

Para estudar o modo independente, altere `modo` para `demo` em `dist/js/config.js`. Esse modo usa IndexedDB e exemplos locais; a conta é mostrada na própria tela de login. Esses dados são separados da API e não são usados como alternativa quando o servidor falha.

Com Node.js instalado, `npm run dev` serve os arquivos na porta 5500. No modo API, esse servidor de desenvolvimento encaminha `/api` e `/hubs` à API local em 5080. A execução recomendada do tutorial continua sendo pela própria API.

## Testes e estudo

Execute `npm test`: 24 testes de regras e demonstração. Os testes HTTP/SignalR contra a API real estão em `../TestesIntegracao/web-http.mjs`, executados pelo runner C# com `VANDERCAKE_TESTAR_WEB_HTTP=1`.

Veja `../Docs/CONTRATO_API.md`, `../Docs/ALTERACOES.md` e `../Docs/VALIDACAO.md`. O código está separado por páginas HTML, CSS e módulos JavaScript em `dist/`. A biblioteca oficial SignalR está em `dist/vendor`, acompanhada da licença; não depende de CDN.
