# Arquitetura e banco de dados

## Fontes e compatibilidade

Esta implementação usa as especificações VanderCake disponíveis, o VanderCake.Mobile com Comprar novamente, a etapa VanderCake.Web e o VanderShop.Api recuperado do pacote VanderShop-Integracao. Não foi presumido um novo anexo inexistente nesta etapa.

A API conserva o estilo do VanderShop: Controllers ASP.NET Core, serviços em C#, Dapper e MySqlConnector. Os contratos de negócio são do VanderCake. A autenticação usa os tokens Bearer protegidos do ASP.NET Core; eles são opacos, não JWT. Senhas usam `PasswordHasher`, com salt e derivação de chave.

Os nomes de propriedades C# usam PascalCase; a API serializa camelCase. As colunas MySQL acompanham os modelos da API (`Id`, `VendedorId`, `PedidoGeralId` etc.), em vez da notação conceitual `ID_Vendedor` do documento original. Isso permite mapear com Dapper sem renomear as classes do MOBILE. A WEB recebe projeções próprias com valores inteiros em centavos, enquanto MOBILE e banco usam valores decimais.

## Tabelas

| Tabela | Papel e vínculos principais |
| --- | --- |
| Estados | UF única; possui cidades |
| Cidades | FK EstadoId; nome único por estado |
| Administradores | Conta da loja, cidade, hash e versão de segurança |
| Vendedores | Conta do vendedor, cidade, ativo, hash e versão de segurança |
| Clientes | FK CidadeId e VendedorId; documento único |
| Categorias | Agrupamento dos produtos |
| Produtos | FK CategoriaId; preço decimal, estoque inteiro, ativo e versão |
| CondicoesPagamento | À vista, 7/14 dias, 7/14/21 dias |
| ParcelasCondicaoPagamento | Número da parcela e prazo de cada condição |
| FormasPagamento | Pix, boleto, dinheiro e cartões |
| PedidoGeral | Identificador de envio único, vendedor, datas, status e administrador da decisão |
| Pedidos | FK PedidoGeralId, ClienteId e CondicaoPagamentoId; totais solicitado/atendido |
| ItemPedido | Produto, nome/preço registrados no envio, quantidade solicitada e atendida |
| ParcelasPedido | FK PedidoId e **FormaPagamentoId por parcela**, prazo e valor previsto |
| Vendas | Uma venda por pedido que recebeu atendimento positivo |
| ItemVenda | Quantidades efetivamente vendidas; referência ao item do pedido |
| ContasReceber | FK VendaId e ParcelaPedidoId; vencimento, valor final e situação financeira |
| ControleEnvios | UUID + hash do conteúdo + resultado; evita duplicação e conserva rejeições definitivas |
| Operacoes | Idempotência das confirmações/cancelamentos administrativos |
| EventosAlteracao | Avisos persistidos junto da transação e publicados por SignalR |
| VersoesBanco | Identifica a versão da estrutura instalada |

As quatro últimas são tabelas técnicas de integração. Não representam novos cadastros na interface.

`ContasReceber` obtém a forma de pagamento pela parcela referenciada. Assim, não existe uma forma única que sobrescreva o pedido inteiro nem uma segunda escolha divergente dentro da cobrança.

Chaves compostas em ItemVenda e ContasReceber impedem associar uma venda ao item/parcela de outro pedido. As FKs usam RESTRICT. Não se apagam pedidos históricos em cascata. Produtos já utilizados são desativados, conservando os itens históricos.

## Regras transacionais

- Envio: valida a carteira, atividade do vendedor/cliente/produto, condição, cada forma de pagamento, preços, quantidades e datas. Todo o lote é aceito ou rejeitado.
- O recebimento não reserva nem baixa estoque. O preço precisa corresponder ao catálogo; uma divergência exige revisão explícita do carrinho.
- Um Pedido Geral contém no máximo um pedido por cliente. Um pedido contém no máximo um item por produto e uma ocorrência de cada número de parcela.
- Confirmação: bloqueia o lote e os produtos envolvidos com `FOR UPDATE`, revalida a prévia, calcula os cortes, gera vendas/contas e baixa o estoque em uma transação.
- A prévia tem validade de dez minutos e incorpora versões de lote/produtos. Mudança de estoque ou edição exige nova conferência.
- Repetição de envio usa UUID e hash canônico. Um mesmo UUID com conteúdo diferente é recusado. Confirmações repetidas não geram nova baixa.
- Rejeições definitivas ficam em ControleEnvios. O MOBILE só libera a edição se a resposta trouxer `loteNaoGravado: true`; incerteza de rede preserva a chave/corpo do envio.
- Totais usam DECIMAL(12,2); na divisão em parcelas, a última recebe a diferença dos centavos. Não são criadas contas com valor zero.
- O corte usa proporção por produto dentro do lote, maiores restos e menor ID do cliente como desempate. Quantidades são unidades inteiras.
- Prazos são dias corridos a partir da data da confirmação no fuso America/Sao_Paulo. Eventos são armazenados em UTC; vencimentos usam DATE.
- Pedido sem atendimento fica NaoAtendido e não gera venda/cobrança. Cancelamento de pendente exige motivo, preserva estoque e não gera venda.
- Recebimento financeiro: bloqueia a conta, registra a data da loja e o administrador, e publica o aviso na mesma transação. Repetições mantêm a primeira baixa; não alteram valor nem forma da parcela. Não inclui pagamento parcial ou estorno.
- Condições/formas usadas no histórico só podem mudar de atividade; descrição e prazos ficam preservados. A WEB mantém esses cadastros e categorias.
- Alterações de cadastro usam versão otimista. Um formulário aberto antes de uma venda não pode sobrescrever o novo estoque.

## Status

| Entidade | Status |
| --- | --- |
| Pedido Geral no MySQL | AguardandoConfirmacao, Processado, Cancelado |
| Pedido individual no MySQL | Pendente, Confirmado, ConfirmadoComCorte, NaoAtendido, Cancelado |
| Pedido local no MOBILE | Rascunho, antes do envio |
| Controle local de envio | Montagem, EnvioIncerto, Sincronizado |
| Conta a receber | Pendente, Paga; recebimento integral registrado pela WEB |

A lista WEB usa os pedidos decididos e os registros de venda: um cancelamento permanece visível no histórico, embora não crie uma linha financeira em Vendas.

## Atualização dos aplicativos

O evento é inserido na transação da alteração. O publicador tenta entregá-lo a cada 500 ms e repete após falhas. Os clientes tratam o evento como aviso de invalidação e consultam a API autenticada. Avisos de pedidos/clientes vão para a loja e a carteira responsável; avisos de catálogo são gerais. O servidor define os grupos do SignalR, sem permitir ao cliente escolher a carteira.

Pode haver aviso repetido se o processo cair após publicar e antes de marcar o evento. Isso é seguro porque a consulta substitui a cópia sincronizada; não cria outra venda. A reconexão e a consulta de recuperação a cada 30 segundos cobrem avisos perdidos. O banco permanente continua sendo a fonte dos dados.

A implementação considera uma instância da API no computador da loja. Não inclui cluster SignalR, Redis, publicação na Internet, migração automática de um banco antigo ou paginação de históricos muito grandes. Essas extensões dependem do volume e da etapa de implantação.

## Organização do código

| Caminho | Responsabilidade |
| --- | --- |
| API/Program.cs | Configuração, autenticação, limites de login, CORS, erros, arquivos WEB e hub |
| API/Controllers | Rotas e perfis autorizados; extração do usuário do token |
| API/Contracts | DTOs de entrada independentes do banco |
| API/Models | Modelos persistidos e de leitura |
| API/Infrastructure | Conexão, regras básicas, exceções e cadastro inicial do administrador |
| API/Services/AutenticacaoService.cs | Login, renovação e revogação por atividade/versão |
| API/Services/CadastroService.cs | Cadastros, versões e vínculos |
| API/Services/PagamentosService.cs | Condições/formas e recebimento integral de parcelas |
| API/Services/PedidosService.cs | Recebimento, idempotência, confirmação e cancelamento |
| API/Services/RateioService.cs | Prévia e cálculo proporcional |
| API/Services/LeituraService.cs | Leitura consistente e montagem de objetos relacionados |
| API/Services/ProjecaoWeb.cs | Adaptação para centavos e campos da WEB |
| API/Services/EventosService.cs | Publicação dos eventos confirmados |
| Mobile/Services/TempoRealService.cs | Conexão SignalR e sincronização em primeiro plano |
| Web/dist/js/tempo-real.js | Conexão SignalR, reconexão e avisos às páginas |

Não há senha do MySQL nos aplicativos clientes. A conta técnica não pode criar/apagar tabelas. Nas consultas, dados de senha/hash não são serializados. A desativação ou alteração de senha invalida acessos antigos. As credenciais de aplicação são diferentes da conta técnica do banco.

A validação cadastral confere formato/comprimento de CPF/CNPJ, CEP e telefone e normaliza pontuação. Não consulta Receita Federal, serviço postal nem comprova titularidade de documentos.
