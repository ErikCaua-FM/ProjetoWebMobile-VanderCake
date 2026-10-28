# Alterações desta etapa

O pacote completo contém API, SQL e as versões atualizadas da WEB e do MOBILE. Use os três projetos juntos.

## Banco e API

- API nova em ASP.NET Core 10, Controllers, Dapper e MySqlConnector, seguindo a organização do VanderShop.Api.
- Estrutura MySQL com 21 tabelas, índices, FKs e restrições; detalhes em ARQUITETURA_E_BANCO.md.
- Login com hash de senha, perfis, renovação, bloqueio de conta inativa e conta técnica de permissões limitadas.
- Recebimento do Pedido Geral, idempotência, prévia, rateio, confirmação/cancelamento, estoque e parcelas em transação.
- SignalR com avisos persistidos e recuperação de desconexões.
- Baixa integral de parcelas com data/administrador, idempotência e atualização do MOBILE; manutenção de condições/formas, preservando histórico.
- API serve a WEB local no mesmo endereço; não exige outro servidor para a interface.

## MOBILE

- Sessão inclui refresh token e renovação automática após recusa do acesso.
- TempoRealService gerencia avisos, reconexão e recuperação periódica em primeiro plano.
- App acompanha o ciclo de vida da janela; listas recarregam a cópia local após sincronização.
- Carteira e pedidos refletem alterações, preservando carrinhos, lotes ainda locais e o fluxo Comprar novamente.
- Detalhes de parcelas mostram o recebimento e sua data quando registrados pela loja.
- ApiConfig recebe uma constante específica para o endereço do PC na rede.

## WEB

- Modo API ativado por padrão e URL derivada da origem atual.
- Renovação de sessão no cliente HTTP.
- Cliente SignalR distribuído localmente; listas atualizam por aviso e recuperação periódica.
- Servidor Node opcional encaminha HTTP/WebSocket à API local.
- Páginas de Contas a receber e Cadastros auxiliares; filtros de período em Vendas e Pendentes.
- O layout e as cores da etapa anterior foram preservados.

## Arquivos modificados ou acrescentados aos clientes

### VanderCake.Mobile

- `VanderCake.Mobile/README.md` — alterado.
- `VanderCake.Mobile/docs/CONTRATO_API.md` — alterado.
- `VanderCake.Mobile/docs/GUIA_CODIGO_COMPLETO.md` — alterado.
- `VanderCake.Mobile/docs/VALIDACAO.md` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/App.xaml.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Contracts/LoginResposta.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Helpers/ApiConfig.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Models/ParcelaPedido.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Models/SessaoSalva.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Services/ApiService.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Services/AplicacaoService.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Services/IApiService.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Services/TempoRealService.cs` — novo.
- `VanderCake.Mobile/src/VanderCake.Mobile/VanderCake.Mobile.csproj` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/PaginaBase.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/frmCatalogo.xaml.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/frmClientes.xaml.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/frmDetalhesPedido.xaml.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/frmListaPedidos.xaml.cs` — alterado.
- `VanderCake.Mobile/src/VanderCake.Mobile/Views/frmPedidosCliente.xaml.cs` — alterado.
- `VanderCake.Mobile/tests/VanderCake.Tests/VanderCake.Tests.csproj` — alterado.

### VanderCake.Web

- `VanderCake.Web/README.md` — alterado.
- `VanderCake.Web/dist/css/estilos.css` — alterado.
- `VanderCake.Web/dist/js/api.js` — alterado.
- `VanderCake.Web/dist/js/comum.js` — alterado.
- `VanderCake.Web/dist/js/config.js` — alterado.
- `VanderCake.Web/dist/js/dados.js` — alterado.
- `VanderCake.Web/dist/js/pagamentos.js` — novo.
- `VanderCake.Web/dist/js/painel.js` — alterado.
- `VanderCake.Web/dist/js/pendentes.js` — alterado.
- `VanderCake.Web/dist/js/recebimentos.js` — novo.
- `VanderCake.Web/dist/js/regras.js` — alterado.
- `VanderCake.Web/dist/js/tempo-real.js` — novo.
- `VanderCake.Web/dist/pagamentos.html` — novo.
- `VanderCake.Web/dist/recebimentos.html` — novo.
- `VanderCake.Web/dist/vendor/SignalR-LICENSE.txt` — novo.
- `VanderCake.Web/dist/vendor/signalr.min.js` — novo.
- `VanderCake.Web/docs/CONTRATO_API.md` — alterado.
- `VanderCake.Web/docs/GUIA_CODIGO_COMPLETO.md` — alterado.
- `VanderCake.Web/docs/VALIDACAO.md` — alterado.
- `VanderCake.Web/scripts/documentar.mjs` — alterado.
- `VanderCake.Web/scripts/serve.mjs` — alterado.
- `VanderCake.Web/tests/regras.test.js` — alterado.

## Instalação e validação

Scripts PowerShell e tutorial estão na raiz/pasta Scripts. Consulte VALIDACAO.md para os comandos executados, resultados e limitações. Não há alteração automática do banco antigo do VanderShop; esta é uma base própria do VanderCake.

Os dados de demonstração da WEB anterior não são migrados para o MySQL. Vendedores e produtos de teste são criados pelo roteiro ou pelo executor em um schema isolado. Não é preciso copiar alterações manualmente: o ZIP já reúne as versões compatíveis dos três projetos.
