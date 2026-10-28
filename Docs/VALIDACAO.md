# Validação realizada — 27/10/2026

Os resultados abaixo se referem ao código deste pacote. Os testes passaram nos cenários executados; não significam garantia de ausência de falhas em qualquer computador ou situação.

## Ambiente efetivamente usado

Linux Ubuntu 24.04, SDK .NET 10.0.403, runtime ASP.NET Core 10.0.14, MySQL Community 8.4.13, Node.js 24.22.1 e SQLite real em arquivo. O MySQL de testes foi executado como processo local isolado, com dados fictícios; não foi utilizado Docker nem um banco da loja.

A API foi executada com uma conta MySQL limitada a SELECT/INSERT/UPDATE/DELETE. A criação inicial do schema/usuário de teste usa uma conexão administrativa separada. O cliente SignalR .NET está na versão 10.0.14; a biblioteca oficial JavaScript distribuída localmente é 10.0.0, que possui versionamento separado.

## Resultado

| Verificação | Resultado |
| --- | --- |
| Compilação da API e do executor de integração | Aprovada, zero erros e zero avisos |
| C# e XAML do MOBILE compilados como biblioteca .NET 10 em Release | Aprovados, zero erros e zero avisos |
| Testes de regras/SQLite/serviços do MOBILE | 65 verificações aprovadas |
| Testes JavaScript das regras e demonstração WEB | 24 testes aprovados |
| Cenários com API HTTP e MySQL reais | 47 verificações aprovadas |
| Cliente JavaScript real, HTTP e SignalR pelo proxy de desenvolvimento | 12 verificações aprovadas |
| Sintaxe dos cinco scripts PowerShell | Zero erros no parser PowerShell |
| Cliente mysql: SOURCE com caminho contendo espaços e interrupção após erro SQL | Aprovado |

Quando `VANDERCAKE_TESTAR_WEB_HTTP=1`, o runner C# termina com **48 verificações**: as 47 de integração e uma verificação agregada de sucesso do subprocesso que executa os 12 cenários JavaScript. Isso não representa 48 cenários independentes além dos 12.

## O que os testes de integração cobrem

- Execução dos SQLs no MySQL: 21 tabelas e regras das três condições de pagamento.
- Criação do usuário técnico e impedimento de ALTER TABLE; conexão com senha contendo aspas, barra, ponto e vírgula e acento.
- Comando real `--criar-admin`, preenchido por entrada padrão, cria uma conta utilizável na WEB.
- Login, senha incorreta, acesso sem token, perfis separados, renovação válida e inválida.
- Vendedor inicialmente ativo; ausência de hash nas respostas; cidade inexistente e duplicidade de documento recusadas.
- Vínculos de cidade/estado e cliente/vendedor; cadastro MOBILE obtém o dono pelo token.
- Desserialização dos DTOs pelo `ApiService` real do MOBILE.
- Dois envios simultâneos com a mesma chave produzem um lote; conteúdo diferente na mesma chave é recusado.
- Um vendedor não consulta o lote de outra carteira.
- Preço divergente e cliente de outra carteira recusam o lote inteiro, sem pedidos parciais; rejeição definitiva é persistida.
- Rateio 10/6/4 com estoque 11 resulta em 6/3/2; prévia não baixa estoque.
- Dois administradores confirmando simultaneamente o mesmo lote não duplicam baixa, vendas ou parcelas.
- Meio de pagamento independente por parcela, centavos preservados e vencimentos em dias corridos a partir da data da loja.
- Confirmação repetida; bloqueio de cancelamento de venda já confirmada; edição antiga não sobrescreve estoque alterado.
- Atendimento zero sem venda/cobrança e cancelamento repetido sem baixa de estoque.
- Prévia obsoleta recusada; dois lotes diferentes disputando o mesmo estoque não vendem a mesma unidade.
- Total de um centavo com três parcelas mantém as escolhas, sem criar cobranças de valor zero.
- Clientes SignalR reais recebem avisos destinados à loja e ao vendedor correto; outra carteira não recebe seus avisos de pedido.
- Serviço real de tempo real do MOBILE sincroniza o SQLite após uma decisão e conserva o carrinho aberto.
- MOBILE renova acesso recusado, repete a consulta e persiste a nova sessão.
- Recebimento exige administrador e conta existente; duas baixas simultâneas conservam um valor, uma data e um responsável; repetir não gera outro evento.
- Recebimento chega ao SQLite por SignalR, preserva o meio da parcela e o carrinho; outra carteira não recebe esse aviso.
- Cadastro/alteração de condições e formas, bloqueio de versão antiga/prazo inválido, preservação do histórico e rejeição de novas escolhas desativadas.
- Categoria inativa não recebe produto; categoria com produtos ativos não pode ser desativada.
- Troca de senha/desligamento revogam a sessão; FKs e CHECKs recusam inserção órfã e estoque negativo diretamente no SQL.

Os testes JavaScript adicionais importam os módulos entregues (`api.js`, `dados.js`, `tempo-real.js`) e a biblioteca SignalR do navegador. Usam fetch/WebSocket nativos do Node e pequenos substitutos para armazenamento/eventos do navegador. Verificam HTML servido, login, painel em centavos, pendentes/detalhes/prévia, renovação após 401, aviso SignalR, mensagem de validação, manutenção de pagamentos, recebimento com atualização dos detalhes, disponibilidade das páginas novas e saída. O tráfego passa pelo servidor Node opcional e seu proxy HTTP/WebSocket até a API real.

**Esses testes JavaScript não são testes de renderização, clique ou automação de navegador.**

## Comandos reproduzíveis

A partir da raiz do pacote:

```powershell
dotnet build .\VanderCake.Api\VanderCake.Api.csproj
dotnet run --project .\VanderCake.Mobile\tests\VanderCake.Tests\VanderCake.Tests.csproj
```

Na pasta `VanderCake.Web`:

```powershell
npm test
```

Com `VANDERCAKE_TEST_MYSQL` configurada conforme o tutorial e Node.js disponível:

```powershell
$env:VANDERCAKE_TESTAR_WEB_HTTP = '1'
dotnet run --project .\TestesIntegracao\TestesIntegracao.csproj
Remove-Item Env:\VANDERCAKE_TESTAR_WEB_HTTP
```

O executor cria um schema `vandercake_teste_<uuid>` e um usuário técnico `vc_teste_<id>` próprios, mantendo ambos para inspeção. Não usa a base `vandercake` da loja. A API de teste é iniciada em uma porta livre e encerrada ao terminar. Para não testar o cliente JavaScript, omita a variável `VANDERCAKE_TESTAR_WEB_HTTP`.

A compilação neutra do MOBILE foi executada assim:

```powershell
dotnet build .\VanderCake.Mobile\src\VanderCake.Mobile\VanderCake.Mobile.csproj -p:TargetFrameworks=net10.0 -p:TargetFramework=net10.0 -p:OutputType=Library -c Release
```

Esse comando compila código comum e XAML. O resultado é uma biblioteca de validação, **não um APK nem um aplicativo Windows para abrir**. O projeto continua configurado para os destinos nativos MAUI.

## Limitações verificadas

1. **Instalação Windows:** os scripts foram revisados e passaram no parser, mas instalar serviço, executar DPAPI e configurar firewall não foram testados em um Windows real. O tutorial segue os procedimentos oficiais do MySQL/.NET.
2. **Android:** a instalação automática das dependências Android encontrou falha de conexão ao repositório do instalador; não havia Android SDK/JDK completo. A compilação de APK e a execução em emulador/aparelho não foram concluídas. Compilação neutra e testes de serviço não substituem testes de UI, SecureStorage nativo e ciclo de vida no aparelho.
3. **Navegador nesta integração:** o ambiente de prévia não conseguiu iniciar o runtime .NET (`Failed to create CoreCLR`). Não foi possível completar QA visual da WEB conectada ao MySQL. A etapa WEB anterior teve validação visual; nesta etapa a integração de seus módulos foi validada por HTTP/SignalR, sem alegar novos testes visuais.
4. **Outros destinos:** execução nativa Windows, iOS e Mac Catalyst não realizada.
5. **Escala/implantação:** não foram executados testes de carga prolongada, rede móvel real, Internet pública, múltiplas instâncias da API, recuperação de energia do PC nem restauração real de backup Windows.

## Roteiro que falta no seu equipamento

Instale seguindo o tutorial, cadastre administrador/vendedor/clientes/produto e faça o cenário 10/6/4 com estoque 11. Confira cadastro por estado/cidade, pesquisa, abertura de detalhes em nova guia, confirmação e cancelamento com as duas aplicações abertas. Confira os filtros por período, os cadastros auxiliares e a baixa de uma parcela, observando a atualização no MOBILE. Teste retorno do Android após ficar em segundo plano, encerramento/reabertura com sessão salva, perda de rede durante envio e retomada sem duplicar. Confira também o comportamento após reiniciar o computador e iniciar a API novamente.

A rotina preserva rascunhos por vendedor, mas a desinstalação/limpeza dos dados do MOBILE remove rascunhos que nunca chegaram à API. Faça os primeiros testes com dados fictícios.
