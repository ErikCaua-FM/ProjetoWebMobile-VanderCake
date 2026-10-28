# Contrato implementado da API

Base local: `http://localhost:5080/api`. JSON UTF-8 em camelCase. HTTPS é necessário para implantação fora do ambiente local de desenvolvimento.

## Autenticação

| Método | Rota | Perfil |
| --- | --- | --- |
| POST | /auth/administradores/login | Público; e-mail/senha |
| POST | /auth/vendedores/login | Público; e-mail/senha |
| POST | /auth/renovar | Público; refreshToken válido |
| GET | /saude | Público; disponibilidade e versão do banco |

Login recebe `{"email":"...","senha":"..."}`. Retorna `accessToken`, `refreshToken`, `expiresIn` e `administrador` ou `vendedor`, conforme o perfil. O objeto da pessoa contém id, nome, e-mail e ativo. Renovar recebe `{"refreshToken":"..."}` e retorna o mesmo formato do login.

Acesso padrão de oito horas e renovação de 30 dias, configuráveis em appsettings. Renovação emite novos tokens e verifica atividade/versão de segurança. Não há lista de revogação individual por dispositivo; sair elimina tokens locais. Alteração de senha/desativação revoga os acessos anteriores da conta. A WEB usa sessionStorage e propaga saída entre guias; o MOBILE usa SecureStorage, restaurado ao reabrir.

Demais chamadas enviam `Authorization: Bearer <accessToken>`. Os endpoints de login/renovação têm limite de 30 chamadas por minuto por IP. Senhas e hashes não aparecem nas respostas.

## MOBILE

| Método | Rota | Entrada/retorno |
| --- | --- | --- |
| GET | /mobile/sincronizacao | Snapshot completo autorizado: vendedor, clientes, estados, cidades, categorias, produtos, condições, formas, pedidosGerais, sincronizadoEm |
| POST | /clientes | NovoClienteRequisicao; retorna Cliente com cidade/UF e vendedorId derivado do token |
| POST | /pedidos-gerais | PedidoGeralEnvio; retorna PedidoGeral completo |
| GET | /pedidos-gerais/por-identificador/{uuid} | Retorna envio pertencente ao vendedor; 404 se ausente nessa conta |

Use os DTOs em `VanderCake.Mobile/src/VanderCake.Mobile/Contracts`. As propriedades adicionais do servidor são compatíveis com a desserialização existente. Dinheiro é decimal (ex.: 33.34), eventos são ISO 8601 em UTC e cada parcela tem sua formaPagamentoId. A resposta de vencimento do Mobile usa DateTime à meia-noite; a interface mostra o dia sem converter para hora local. O banco usa DATE.

Envio mínimo ilustrativo (IDs devem existir no seu banco):

```json
{
    "identificadorEnvio": "0fef536f-2eb1-412c-92ca-d70207b80e09",
    "dataHoraCriacao": "2026-10-27T12:00:00Z",
    "pedidos": [
        {
            "clienteId": 1,
            "dataHoraCriacao": "2026-10-27T12:00:00Z",
            "condicaoPagamentoId": 3,
            "itens": [{ "produtoId": 1, "quantidadeSolicitada": 10, "precoUnitario": 10.00 }],
            "parcelas": [
                { "numeroParcela": 1, "formaPagamentoId": 1 },
                { "numeroParcela": 2, "formaPagamentoId": 2 },
                { "numeroParcela": 3, "formaPagamentoId": 3 }
            ]
        }
    ]
}
```

A data acima é apenas exemplo; o aplicativo usa a data atual. A API calcula totais e prazos; não confia em totais enviados. Até 500 pedidos por lote e 500 itens por pedido nesta versão; condições de 1 a 3 parcelas. Quantidades positivas inteiras.

## WEB

Todos os endpoints abaixo exigem Administrador:

| Método | Rota | Retorno/comportamento |
| --- | --- | --- |
| GET | /admin/painel | Coleções necessárias às listas e cadastros |
| GET | /admin/pedidos-gerais/{id} | lote, clientes relacionados, cidades, estados, vendas, contasReceber |
| GET | /admin/pedidos-gerais/{id}/previa | tokenPrevia, pedidos, totais em centavos, unidadesCortadas |
| POST | /admin/pedidos-gerais/{id}/confirmacao | tokenPrevia e identificadorOperacao; confirma lote |
| POST | /admin/pedidos-gerais/{id}/cancelamento | motivo, versao e identificadorOperacao; cancela lote pendente |
| POST / PUT | /admin/produtos [/{id}] | Cadastro/alteração; precoCentavos inteiro e estoque inteiro |
| DELETE | /admin/produtos/{id} | Corpo {versao}; exclui se não usado, ou desativa preservando histórico |
| POST / PUT | /admin/vendedores [/{id}] | Cadastro/alteração; senha só na entrada, omitida para mantê-la |
| PATCH | /admin/vendedores/{id}/atividade | ativo e versao; revoga sessão anterior |
| POST / PUT | /admin/clientes [/{id}] | Cadastro/alteração com cidadeId e vendedorId |
| POST / PUT | /admin/categorias [/{id}] | Nome, ativo e versão na edição |
| POST / PUT | /admin/condicoes-pagamento [/{id}] | descricao, ativo, dias e versao na edição |
| POST / PUT | /admin/formas-pagamento [/{id}] | descricao, ativo e versao na edição |
| POST | /admin/contas-receber/{id}/recebimento | Baixa integral manual; sem dados de valor/data no corpo |
| POST / PUT | /admin/estados [/{id}] | Nome, UF e versão na edição |
| POST / PUT | /admin/cidades [/{id}] | Nome, estadoId e versão na edição |

As projeções Web usam `precoCentavos`, `valorTotalSolicitadoCentavos`, `valorTotalAtendidoCentavos`, `valorPrevistoCentavos`, `valorFinalCentavos` etc. Vencimentos são `yyyy-MM-dd`. As versões são devolvidas na leitura e devem ser reenviadas ao editar.

## Recebimentos e manutenção de pagamentos

A página `recebimentos.html` usa as contas devolvidas em `/admin/painel` e filtra por cliente, vencimento e situação. O POST de recebimento retorna `{id,status,dataPagamento,administradorBaixaId}`. Data e administrador são definidos pelo servidor; a repetição retorna a mesma baixa sem alterar valor ou forma. Nos detalhes e na sincronização MOBILE, a parcela passa a `situacao: "Paga"` e recebe `dataPagamento`. Não há pagamento parcial, estorno ou integração bancária nesta etapa.

A página `pagamentos.html` mantém categorias, condições e formas. `dias` admite `[0]`, `[7,14]` ou `[7,14,21]`. Edições enviam a versão lida. Quando uma condição/forma já foi usada, descrição e prazos não podem ser reescritos; sua atividade ainda pode mudar. Pedidos existentes conservam suas escolhas mesmo se o cadastro for desativado.

## Tempo real

Hub: `/hubs/atualizacoes`, fora do prefixo `/api`. Requer token. Evento: `DadosAlterados`, com `{id,tipo,criadoEm}`. Não transmite lista de clientes/pedidos: o cliente consulta seu endpoint autorizado. O grupo é escolhido pelo servidor de acordo com o perfil/ID.

A conexão é encerrada ao vencer o token; o cliente obtém token atual e reconecta. Atualizações em background não substituem campos de formulários que o usuário está digitando. Erros de rede preservam dados locais.

## Erros

| HTTP | Significado |
| --- | --- |
| 400 | JSON/contrato inválido |
| 401 | Login inválido, token vencido/revogado ou conta inativa |
| 403 | Perfil sem permissão |
| 404 | Recurso inexistente ou fora da carteira |
| 409 | Duplicidade, versão, decisão ou prévia conflitante |
| 422 | Regra de validação não atendida |
| 429 | Limite de tentativas; aguarde um minuto |
| 503 | Conexão/estrutura do banco indisponível |

Erros da aplicação incluem `mensagem`. Erros de vinculação de DTO podem usar ProblemDetails com `errors`. `loteNaoGravado: true` só é emitido quando a API registrou rejeição definitiva antes de inserir os pedidos; sua ausência/falso nunca autoriza trocar o UUID de um envio incerto.
