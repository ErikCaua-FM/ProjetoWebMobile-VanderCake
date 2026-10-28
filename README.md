# VanderCake — banco, API, Web e Mobile integrados

Comece por **TUTORIAL_WINDOWS.md**, na raiz deste pacote. O roteiro instala o MySQL 8.4 diretamente no Windows, pelo PowerShell, sem Docker e sem Workbench.

| Pasta | Conteúdo |
| --- | --- |
| `Banco/` | Estrutura completa, chaves estrangeiras, índices, restrições, dados iniciais e conta técnica |
| `Scripts/` | Instalação, preparação do banco, configuração da conexão, inicialização e testes |
| `VanderCake.Api/` | ASP.NET Core 10, Controllers, Dapper, MySqlConnector e SignalR |
| `VanderCake.Web/` | HTML, CSS e JavaScript; usa a API por padrão |
| `VanderCake.Mobile/` | .NET MAUI; SQLite, sessão segura e sincronização em tempo real |
| `TestesIntegracao/` | Testes HTTP, MySQL, SignalR e serviços reais do Mobile |
| `Docs/` | Arquitetura, alterações, contrato de integração e resultados de validação |

A API também serve os arquivos da WEB: depois da configuração, abra `http://localhost:5080`. Não é preciso instalar Node.js para usar a loja; ele só é necessário para os testes JavaScript e para o servidor de desenvolvimento opcional.

Não há usuário ou senha padrão no banco da loja. O primeiro administrador é criado pelo comando `dotnet run --project VanderCake.Api -- --criar-admin`. Cadastre os vendedores pela WEB e use suas credenciais no MOBILE.

As formas de pagamento pertencem às parcelas. Um pedido de 7/14/21 dias pode usar Pix, boleto e dinheiro, respectivamente. A confirmação aplica cortes proporcionais no lote, recalcula valores e mantém essas escolhas.

A WEB também inclui consulta/baixa integral de parcelas e manutenção de categorias, condições e formas de pagamento. Os recebimentos atualizam os detalhes do MOBILE.

Veja `Docs/VALIDACAO.md` para distinguir os testes executados das verificações que ainda dependem do seu Windows e de um aparelho Android.
