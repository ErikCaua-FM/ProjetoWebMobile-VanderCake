# Contrato da API VanderCake para o Mobile

Contrato utilizado pelo cliente integrado. A implementação está em VanderCake.Api no pacote VanderCake.Integracao. A referência consolidada está em ../../Docs/CONTRATO_API.md. Os nomes abaixo são propriedades JSON em camelCase, não nomes literais das colunas MySQL. Os DTOs estão em `Contracts/` e os objetos recebidos estão em `Models/`.

Base de desenvolvimento: `/api/`. Requisições e respostas usam JSON UTF-8. Datas de eventos usam ISO 8601 com UTC/offset; vencimentos são datas sem conversão de fuso. Valores monetários são números decimais, nunca textos com `R$` ou vírgula decimal.

## Rotas utilizadas

| Método | Rota relativa a `/api/` | Entrada | Resposta |
| --- | --- | --- | --- |
| POST | `auth/vendedores/login` | `LoginRequisicao` | `LoginResposta` |
| POST | `auth/renovar` | refreshToken válido | LoginResposta renovada |
| GET | `mobile/sincronizacao` | Bearer token | `DadosSincronizacao` |
| POST | `clientes` | `NovoClienteRequisicao` | `Cliente` completo, com ID oficial |
| POST | `pedidos-gerais` | `PedidoGeralEnvio` | `PedidoGeral` completo, com todos os pedidos/itens/parcelas |
| GET | `pedidos-gerais/por-identificador/{identificadorEnvio}` | UUID do lote e Bearer token | `PedidoGeral` completo, ou 404 se não encontrado |

Todas as rotas, exceto login e renovação, exigem `Authorization: Bearer TOKEN`. O vendedor será obtido do token; nunca poderá ser escolhido pelo cliente no corpo do cadastro/envio. Cada consulta será filtrada pela conta autenticada. Um vendedor não terá acesso aos lotes, clientes ou consultas por identificador de outro vendedor.

## Login

```json
{
    "email": "vendedor@empresa.com",
    "senha": "SENHA_INFORMADA_NA_TELA"
}
```

Resposta HTTP 200:

```json
{
    "accessToken": "TOKEN_EMITIDO_PELA_API",
    "refreshToken": "TOKEN_DE_RENOVACAO",
    "expiresIn": 28800,
    "vendedor": {
        "id": 1,
        "nome": "Nome do vendedor",
        "email": "vendedor@empresa.com",
        "ativo": true
    }
}
```

Tokens de acesso/renovação, validade e identificação ficam no SecureStorage. Nenhuma senha é devolvida. POST auth/renovar recebe refreshToken e retorna o mesmo formato do login. O cliente tenta renovar após 401; se a renovação também for recusada, exige novo login, sem apagar rascunhos.

## Sincronização

O GET retorna um snapshot completo dos dados disponíveis para o vendedor, sem paginação nesta primeira versão. Todas as coleções devem ser arrays, mesmo quando vazias; não enviar `null`.

Estrutura:

```json
{
    "vendedor": { "id": 1, "nome": "Nome do vendedor", "email": "vendedor@empresa.com", "ativo": true },
    "clientes": [],
    "estados": [],
    "cidades": [],
    "categorias": [],
    "produtos": [],
    "condicoesPagamento": [],
    "formasPagamento": [],
    "pedidosGerais": []
}
```

O arquivo `docs/exemplos/sincronizacao.json` apresenta a estrutura preenchida. Os valores são exemplos de contrato e não são carregados automaticamente pelo aplicativo.

- `clientes`: carteira do vendedor. Incluir clientes inativos que precisam continuar consultáveis no histórico; o Mobile não permitirá novos pedidos para eles.
- `estados` e `cidades`: cadastros para o formulário de novo cliente.
- `produtos`: preços atuais, categoria, descrição, saldo e flag `ativo`. Produto ativo com estoque zero pode ser solicitado.
- `condicoesPagamento`: IDs reais do banco, descrição, ativo e regras de parcela. Os planos aceitos nesta etapa são `[0]`, `[7,14]` e `[7,14,21]`.
- `formasPagamento`: IDs reais do banco para Pix, boleto bancário, cartões de crédito/débito e dinheiro. O Mobile não presume IDs fixos; utiliza os IDs retornados.
- `pedidosGerais`: histórico completo do vendedor com os pedidos, itens e parcelas. Não incluir pedidos de outros vendedores.

Dados recebidos não podem apagar os rascunhos locais. O Mobile concilia lotes pelo `identificadorEnvio`, preservando a quantidade solicitada e atualizando os dados de atendimento enviados pela loja.

## Cadastro de cliente

O corpo possui `nome`, `documento`, `rua`, `bairro`, `cep`, `numero`, `cidadeId`, `telefone` e `email` opcional. O Mobile remove pontuação de documento, CEP e telefone e faz validações de preenchimento e tamanho; a API deve validar os dados novamente, incluindo o documento e sua unicidade global.

A resposta deve ser o cliente completo e conter `id`, `vendedorId`, `cidadeNome`, `uf` e `ativo`. A API deriva `vendedorId` do token. O cadastro só é incluído no cache após resposta válida. Em caso de resposta perdida, a unicidade de documento impede um segundo cadastro; a sincronização permite recuperar o cliente já criado.

## Envio do PedidoGeral

Consulte `docs/exemplos/envio-pedido-geral.json`. O corpo inclui:

- `identificadorEnvio`: UUID criado no Mobile.
- `dataHoraCriacao`: início da montagem.
- `pedidos`: um objeto por cliente, cada um com `clienteId`, `dataHoraCriacao`, `condicaoPagamentoId`, `itens` e `parcelas`.
- Cada item envia `produtoId`, `quantidadeSolicitada` e `precoUnitario` esperado. O preço é usado para detectar divergências; a API deve consultar o preço oficial.
- Cada parcela envia `numeroParcela` e seu próprio `formaPagamentoId`. A condição fornece os prazos. A API calcula os valores e valida todas as parcelas.

Ao receber, a API deverá:

1. Validar o vendedor, a carteira, os produtos, os preços e as formas/condições.
2. Garantir um pedido por cliente no lote e um item por produto no pedido.
3. Gravar todo o conjunto em uma transação. Qualquer falha de validação impede a gravação de todos os pedidos desse envio.
4. Preservar os valores solicitados. O recebimento não gera venda, não reserva estoque e não baixa estoque.
5. Responder com o lote inteiro, seus IDs oficiais e os estados `AguardandoConfirmacao` / `Pendente`.

O cliente aceita HTTP 200 ou 201 com o objeto completo. Não responder somente com o ID ou com HTTP 204.

## Idempotência e resposta perdida

O identificador do lote deve ser UNIQUE no MySQL e a API deve serializar operações concorrentes para a mesma chave.

- Repetir o mesmo identificador com o mesmo conteúdo deve retornar o lote já criado.
- Repetir um identificador já recebido com conteúdo diferente deve retornar conflito, sem alterar os registros originais e sem liberar a edição local.
- A consulta por identificador deve verificar o proprietário. Um 404 não é prova de que uma requisição anterior em andamento jamais será gravada; por isso o Mobile não libera a edição ao receber 404 e retransmite o corpo original.
- HTTP 5xx, timeout, resposta vazia ou incompatível mantêm o lote local protegido como `EnvioIncerto`.
- Só é permitido responder `loteNaoGravado: true` quando a API puder garantir definitivamente que aquele lote não foi aceito e não será gravado por uma tentativa concorrente em andamento. Essa sinalização deverá ser coerente com a transação/controle de concorrência do backend.
- Após essa rejeição definitiva, o Mobile mantém os pedidos, volta à montagem e cria outro UUID para a tentativa corrigida. Nunca reutiliza o corpo alterado com uma chave de envio já congelada.

Erro de validação definitivo, HTTP 400/409/422:

```json
{
    "mensagem": "O preço de um produto foi alterado. Atualize os preços do carrinho e revise o pedido.",
    "loteNaoGravado": true
}
```

O servidor pode acrescentar `errors` no formato de validação ASP.NET, um objeto de arrays de mensagens. Evitar usar 401 para erros de regra de negócio: esse status encerra a sessão local.

## Resposta completa e resultado da loja

Consulte `docs/exemplos/resposta-pedido-geral.json`. Essa é a forma esperada tanto no POST quanto na consulta por identificador e nos itens de `pedidosGerais` da sincronização.

No pedido:

- `valorTotalSolicitado`: total original validado.
- `valorTotalAtendido`: `null` enquanto pendente/cancelado; valor final na confirmação, inclusive zero em `NaoAtendido`.
- `status`: `Pendente`, `Confirmado`, `ConfirmadoComCorte`, `NaoAtendido` ou `Cancelado`.
- `itens[].quantidadeSolicitada`: imutável.
- `itens[].quantidadeAtendida`: `null` antes de processar; inteiro de zero até a quantidade solicitada depois do corte.
- `parcelas[].valorPrevisto`: valor anterior ao corte, preservado.
- `parcelas[].valorFinal`: valor resultante da confirmação. Para pedido não atendido, informar zero; nenhuma conta a receber será gerada.
- `parcelas[].dataVencimento`: `null` antes da venda; data da conta a receber após confirmação.
- `parcelas[].situacao`: `Prevista` antes da confirmação, `Pendente`/`Paga` para contas existentes ou uma descrição como `Sem atendimento` quando aplicável.

Nome do produto, preço original, condição, prazos e formas de pagamento devem permanecer coerentes com o pedido aceito. As formas de pagamento de cada parcela serão preservadas após o corte; mudam os valores, conforme o total atendido.

A confirmação da loja aplica o rateio e cria Vendas, ItemVenda e ContasReceber no servidor. Não há endpoint de confirmação de venda no Mobile; esse controle será da Web.

## Referência para o banco central

| Objeto JSON | Origem relacional prevista |
| --- | --- |
| `vendedor` | Vendedores |
| `clientes`, `estados`, `cidades` | Clientes, Estados, Cidades |
| `produtos`, `categorias` | Produtos, Categorias |
| `condicoesPagamento[].parcelas` | CondicoesPagamento + ParcelasCondicaoPagamento |
| `formasPagamento` | FormasPagamento |
| `PedidoGeral.pedidos` | PedidoGeral + Pedidos |
| `Pedido.itens` | ItemPedido |
| `Pedido.parcelas` | ParcelasPedido, complementadas por ContasReceber após a venda |
| Totais e quantidades atendidas | Resultado da confirmação e respectivas Vendas/ItemVenda |

Todos os IDs são gerados e validados no servidor, exceto o UUID de envio. O Mobile não abre conexão MySQL.

## Recebimento de parcelas

A sincronização inclui `situacao: "Paga"` e `dataPagamento` nas parcelas recebidas integralmente pela loja. A data representa o calendário da loja; não deve ser deslocada por conversão de fuso. O MOBILE apenas consulta o recebimento registrado pelo administrador e conserva a forma escolhida na parcela. O evento `Pagamentos` dispara a mesma sincronização autenticada usada pelos pedidos.
