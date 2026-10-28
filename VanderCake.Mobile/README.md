# VanderCake Mobile

Aplicação de vendedores da distribuidora VanderCake, criada a partir da estrutura do VanderShop.Mobile e das especificações revisadas em 22/10/2026.

Esta versão do cliente .NET MAUI está integrada à API e ao MySQL entregues no pacote VanderCake.Integracao. Siga `../TUTORIAL_WINDOWS.md` antes de executar. O aplicativo não inicia o servidor automaticamente e não contém contas ou catálogo fictícios.

## Abrir e executar

1. Extraia o ZIP inteiro, preservando as pastas.
2. Abra `VanderCake.slnx` em um ambiente com SDK .NET 10 e ferramentas .NET MAUI. No VS Code, utilize a extensão .NET MAUI e configure um dispositivo Android ou o destino Windows.
3. Defina o endereço da API em `src/VanderCake.Mobile/Helpers/ApiConfig.cs`.
4. Restaure os pacotes e selecione `VanderCake.Mobile` como projeto de inicialização.
5. Execute em modo Debug no dispositivo escolhido. Na primeira abertura, aparecerá o Login.
6. Para entrar e carregar dados reais, inicie a API deste pacote e cadastre um vendedor pela WEB. O antigo servidor VanderShop não é compatível com estes novos contratos.

Comandos no terminal, na pasta que contém esta solução:

```powershell
dotnet --info
dotnet workload install maui
dotnet restore src/VanderCake.Mobile/VanderCake.Mobile.csproj
```

Para compilar para Android, com Android SDK e Java configurados pelo ambiente MAUI:

```powershell
dotnet build src/VanderCake.Mobile/VanderCake.Mobile.csproj -f net10.0-android -c Debug
```

Para compilar para Windows, em um computador Windows com as ferramentas correspondentes:

```powershell
dotnet build src/VanderCake.Mobile/VanderCake.Mobile.csproj -f net10.0-windows10.0.19041.0 -c Debug
```

Os arquivos de plataforma iOS e Mac Catalyst também acompanham o projeto. Esses destinos são habilitados no macOS e dependem de Xcode, assinatura e configuração de Keychain do ambiente Apple. A validação visual em dispositivos e as configurações de publicação são uma etapa posterior; esta entrega é código-fonte, sem APK/instalador.

## Endereço da API

| Ambiente | Endereço de desenvolvimento configurado |
| --- | --- |
| Emulador Android padrão | `http://10.0.2.2:5080/api/` |
| Windows no mesmo computador da API | `http://localhost:5080/api/` |
| Celular físico | Substituir em `ApiConfig.cs` pelo IP do computador na rede, por exemplo `http://192.168.1.50:5080/api/` |

O endereço do celular físico é apenas um exemplo: use o IP real do computador. A API precisa aceitar conexões nessa interface e a porta deve estar acessível. O aplicativo usa HTTP local apenas no Debug Android; a configuração Release exige HTTPS e contém um domínio `.example` que deve ser substituído antes da distribuição. Não é necessário criar banco MySQL dentro do aplicativo nem inserir credenciais do banco no Mobile.

## Funcionamento

- Sem sessão salva: Login com e-mail e senha do vendedor.
- Com sessão salva: restauração pelo SecureStorage e abertura da Carteira de Clientes. A senha não é armazenada. Se a API recusar o token, tenta renová-lo; se a renovação também for recusada, solicita novo login e preserva os rascunhos.
- Carteira: pesquisa, dados cadastrais, `Novo Cliente+` e `Lista de Pedidos` no canto superior direito. Clientes inativos permanecem consultáveis quando retornados pela API, mas não recebem novos pedidos.
- Cliente: histórico individual, detalhes e `Novo Pedido+`.
- Catálogo: produtos e categorias recebidos da API, pesquisa e inclusão com quantidade inteira. O saldo exibido é informativo; o corte será feito pela loja.
- `Comprar novamente`: acima dos produtos, mostra os itens e as quantidades do último pedido salvo daquele cliente. O botão preenche o carrinho e o abre com o pagamento sem seleção.
- Carrinho: quantidade, preço, subtotal, total, condição de pagamento e seleção independente da forma de cada parcela.
- `Acrescentar Pedido à Lista`: salva no SQLite e atualiza tanto a lista geral quanto o histórico daquele cliente. Ainda não envia à loja.
- Lista: revisão, detalhes, edição, remoção e envio conjunto pelo botão `Enviar para a Loja`.
- Depois do recebimento: o pedido permanece no histórico e o vendedor pode montar outro lote. O SignalR dispara sincronização para apresentar confirmação, cortes, valores finais, vencimentos e recebimentos registrados pela loja. Há recuperação por consulta a cada 30 segundos enquanto ativo e ao retomar o aplicativo.

Exemplo de pagamento suportado no mesmo pedido:

| Parcela | Condição 7/14/21 dias | Forma escolhida |
| --- | --- | --- |
| 1 | 7 dias após confirmação | Pix |
| 2 | 14 dias após confirmação | Boleto bancário |
| 3 | 21 dias após confirmação | Dinheiro em espécie |

As formas podem se repetir. A condição define os prazos; cada parcela guarda sua própria forma. A divisão usa `decimal` e distribui a diferença de centavos na última parcela.

## Comprar novamente

Ao entrar em `Novo Pedido+`, o catálogo mostra a opção quando o cliente possui histórico. A referência é o pedido com a data de criação mais recente no histórico local do vendedor: tanto pedidos sincronizados quanto pedidos já acrescentados à lista. Um carrinho ainda em edição não serve como histórico. A data e a situação do pedido usado aparecem junto aos produtos, independentemente da pesquisa ou categoria escolhida.

O botão repete todos os produtos e suas **quantidades solicitadas**, inclusive quando houve corte no atendimento anterior. Usa os nomes e os preços do catálogo atualmente sincronizado e recalcula o total; não reutiliza preços históricos nem limita quantidades pelo saldo informativo. Ao abrir o catálogo, o aplicativo tenta atualizar os dados pela API. Sem conexão, pode usar o histórico e o catálogo já salvos no SQLite.

A ação cria um carrinho com nova data e sem condição de pagamento, parcelas, formas de pagamento, identificador oficial ou dados de atendimento. Selecione novamente a condição e uma forma para cada parcela. O histórico e a lista geral só recebem a alteração quando você clicar em `Acrescentar Pedido à Lista`.

Se o carrinho já contém itens ou pagamento, o aplicativo pede confirmação antes de substituí-lo; cancelar preserva a edição atual. Repetir a ação não soma as quantidades. Se algum produto não existe mais ou está inativo no catálogo, a recompra é bloqueada com o nome dos produtos indisponíveis e o carrinho é preservado por inteiro. Nesse caso, os produtos podem ser escolhidos manualmente pelo catálogo.

Não foram criadas tabelas nem rotas de API adicionais: a opção utiliza o histórico recebido na sincronização e a persistência local já existente.

## Organização do código

| Pasta/arquivo | Responsabilidade |
| --- | --- |
| `App.xaml` / `App.xaml.cs` | Recursos gerais, janela, navegação inicial e retomada do aplicativo. |
| `MauiProgram.cs` | Registro dos serviços, fontes, banco local e cliente HTTP. |
| `Models/` | Vendedor, cliente, catálogo, pedidos, parcelas e dados da sessão. Uma classe por arquivo. |
| `Contracts/` | Corpos das requisições/respostas HTTP. Não contém acesso ao banco central. |
| `Rules/` | Validação de pedido/cliente, cálculo de parcelas e nomes dos estados. |
| `Data/` | SQLite e seus registros de cache, lotes e carrinhos. |
| `Services/AplicacaoService.cs` | Coordenação da sessão, sincronização, edição, histórico e envio seguro dos lotes. |
| `Services/ApiService.cs` | HTTP, Bearer token, serialização e tratamento de respostas. |
| `Services/SecureStorageSessao.cs` | Persistência protegida da sessão no dispositivo. |
| `Views/` | Telas XAML e eventos em C#, no estilo do projeto usado como base. |
| `Helpers/` | Configuração da API, JSON e exceção de comunicação. |
| `Resources/Styles/` | Paleta vermelha discreta, branco e cinza, botões, cartões e tipografia. |
| `Platforms/` | Inicialização e permissões específicas de cada plataforma. |
| `tests/VanderCake.Tests/` | Verificações executáveis das regras e dos serviços usando SQLite real e uma API simulada apenas nos testes. |
| `docs/` | Contrato HTTP, especificações de referência, validação e guia com todos os códigos. |

Foi mantida a abordagem XAML com code-behind para facilitar a comparação com o VanderShop. As regras e o banco foram retirados das telas e concentrados em classes próprias; não foi adicionada uma biblioteca MVVM.

## SQLite e MySQL

O banco local se chama `vandercake_v1.db3`, dentro de `FileSystem.AppDataDirectory`. O Mobile cria suas tabelas automaticamente:

| Tabela local | Conteúdo |
| --- | --- |
| `CacheVendedorRegistro` | Snapshot JSON dos clientes, catálogo, localidades e pagamentos de um vendedor. |
| `PedidoGeralRegistro` | Lote local ou sincronizado, situação de envio e corpo congelado de uma transmissão pendente. |
| `CarrinhoRegistro` | Carrinho em edição por vendedor e cliente. |

São tabelas de persistência local, diferentes das 17 tabelas de domínio e quatro tabelas técnicas implementadas no MySQL. Os objetos são serializados em JSON dentro do SQLite para conservar a estrutura do pedido e os valores decimais. O lote completo é salvo de forma transacional. Não se está usando um arquivo JSON solto no lugar do SQLite.

Os registros enviados são cópias sincronizadas do servidor. Os rascunhos ainda não enviados existem somente no dispositivo. Sair da conta remove o token e mantém os rascunhos vinculados ao vendedor; outra conta não os acessa. Desinstalar o aplicativo ou limpar seus dados apaga rascunhos que ainda não chegaram à API.

## Regras locais que merecem atenção

1. Um lote em montagem por vendedor neste dispositivo, e um pedido por cliente no lote. Abrir novamente um cliente que já possui pedido na lista permite editar a versão existente.
2. Alterações no carrinho em edição só substituem o pedido da lista ao clicar novamente em `Acrescentar Pedido à Lista`. Descartar a edição mantém a versão anterior.
3. Carrinhos incompletos aparecem na Lista de Pedidos. É necessário concluí-los ou descartá-los antes do envio, evitando enviar uma versão antiga sem perceber.
4. O mesmo produto ocupa uma linha por pedido. Acrescentar o produto novamente soma sua quantidade.
5. Atualizar o catálogo não muda preços de rascunhos silenciosamente. Use `Atualizar preços` no carrinho para aceitar os preços atuais e recalcular as parcelas.
6. O cliente não baixa estoque, não aplica corte e não gera vendas/contas a receber. Essas operações pertencem à confirmação transacional na API/Web.
7. A API deve recalcular preços e totais, validar a carteira e garantir idempotência. As validações locais ajudam a interface, mas não substituem as validações do servidor.
8. Se a resposta de envio se perder, o corpo e o identificador permanecem no SQLite. A lista fica protegida contra edição; `Verificar / retomar envio` consulta a API e, se necessário, retransmite o mesmo corpo.
9. Uma rejeição definitiva com `loteNaoGravado: true` permite corrigir o lote. Nesse caso, o Mobile cria outro identificador para a tentativa corrigida. Falhas de rede ou HTTP 5xx nunca liberam a edição automaticamente.

## Testes incluídos

Os testes não precisam da API real nem de um emulador; utilizam o mesmo código das regras, do banco e dos serviços do aplicativo.

```powershell
dotnet run --project tests/VanderCake.Tests/VanderCake.Tests.csproj
```

Veja o resultado efetivamente obtido e as limitações em `docs/VALIDACAO.md`. A execução no dispositivo com a API real deve seguir o roteiro de integração desse arquivo.

## Leitura do código

Comece por `docs/GUIA_CODIGO_COMPLETO.md`: ele contém a explicação das camadas e o conteúdo integral de cada arquivo de código, separado pelo caminho da pasta, com blocos para copiar. Os arquivos de fonte `.ttf` são binários e estão no ZIP; não precisam ser copiados como texto.

Referências técnicas consultadas: [banco local no MAUI](https://learn.microsoft.com/pt-br/dotnet/maui/data-cloud/database-sqlite?view=net-maui-10.0), [SecureStorage](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage?view=net-maui-10.0), [serviços locais](https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/local-web-services?view=net-maui-10.0) e [NavigationPage](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/pages/navigationpage?view=net-maui-10.0).

## Integração e tempo real

`Services/TempoRealService.cs` gerencia o SignalR em primeiro plano. `App.xaml.cs` acompanha início/retomada/parada da janela e `PaginaBase.cs` recarrega as listas locais após sincronização. Formulários de cadastro e carrinho não são sobrescritos por avisos. Sessão e refresh token ficam no SecureStorage. Para aparelho físico, preencha `EnderecoComputadorNaRede` em `Helpers/ApiConfig.cs`.

A compilação neutra de C#/XAML e os serviços foram validados; o APK e o ciclo de vida em aparelho precisam ser testados no ambiente Android. Consulte `../Docs/VALIDACAO.md`.

Se você baixou somente o ZIP do Mobile, obtenha também o pacote VanderCake.Integracao: nele estão a API, o banco, a WEB e o tutorial citados por este README.
