# VanderCake: MySQL local e integração completa

Este roteiro usa **Windows 11 de 64 bits e PowerShell**. O MySQL fica instalado no computador, como serviço do Windows. Não há Docker, Workbench nem acesso direto ao MySQL pela WEB ou pelo MOBILE.

O pacote inclui a API no estilo do VanderShop: C#, ASP.NET Core 10, Controllers, Dapper e MySqlConnector. A WEB continua em HTML/CSS/JavaScript e o MOBILE em .NET MAUI. Use as três versões deste pacote juntas.

## 1. Entenda o caminho dos dados

| Componente | Responsabilidade |
| --- | --- |
| MySQL, porta 3306 | Dados permanentes; acessível somente no próprio computador |
| API, porta 5080 | Login, validações, cadastros, pedidos, rateio, transações e avisos |
| WEB | Administração; acessa a API por HTTP e recebe avisos pelo SignalR |
| MOBILE | Carteira, catálogo, carrinhos e pedidos; usa a API e guarda cópias/rascunhos no SQLite |
| SQLite | Dados locais de cada vendedor, inclusive pedidos ainda não enviados |

O botão **Acrescentar Pedido à Lista** salva localmente. **Enviar para a Loja** transmite um Pedido Geral com os pedidos individuais. Na WEB, **Confirmar** calcula o atendimento, baixa o estoque, gera vendas e contas a receber na mesma transação. Um aviso faz os aplicativos buscarem os dados atualizados.

A confirmação da venda não significa pagamento recebido. As contas nascem como `Pendente`. Na WEB, **Contas a receber** permite consultar as parcelas e registrar o recebimento integral de cada uma, com data e administrador responsável. O registro é manual: não processa cartões, gera boletos nem faz cobrança Pix.

## 2. Extraia o projeto

Salve o ZIP entregue na pasta Downloads. Abra o **PowerShell** e execute, ajustando apenas o nome do arquivo se o navegador o renomear:

```powershell
New-Item -ItemType Directory -Path C:\VanderCake -Force
Expand-Archive -Path "$env:USERPROFILE\Downloads\VanderCake.Integracao.zip" -DestinationPath C:\VanderCake
Set-Location C:\VanderCake\VanderCake.Integracao
Get-ChildItem
```

Você deve encontrar `Banco`, `Scripts`, `VanderCake.Api`, `VanderCake.Web`, `VanderCake.Mobile` e este tutorial. Prefira esse caminho sem espaços. Não extraia por cima de um projeto com alterações suas; preserve uma cópia dele.

Os comandos seguintes partem dessa pasta. Não digite os caracteres `PS>` ou `mysql>`: eles representam prompts, não fazem parte dos comandos.

## 3. Instale o SDK .NET 10

No PowerShell:

```powershell
winget install --id Microsoft.DotNet.SDK.10 --exact --source winget
```

Feche e abra o terminal, volte à pasta do projeto e verifique:

```powershell
Set-Location C:\VanderCake\VanderCake.Integracao
dotnet --info
```

Deve aparecer um **SDK 10.0.x**. Instalar apenas o Runtime não permite compilar. Se já tiver o SDK 10 atualizado, não é necessário reinstalá-lo. Se `winget` não estiver disponível, utilize o instalador oficial do SDK .NET 10 indicado nas fontes ao final.

## 4. Instale o MySQL pelo terminal

Abra uma janela de **PowerShell como Administrador**. A elevação é necessária para instalar o serviço do Windows.

```powershell
Set-Location C:\VanderCake\VanderCake.Integracao
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\01-Instalar-MySQL.ps1
```

O `Bypass` acima vale para esse processo, sem mudar permanentemente a política do computador. O script é fornecido em texto para leitura. Ele:

1. Recusa uma instalação se já encontrar serviço, diretório de dados ou porta 3306 ocupada.
2. Instala o Visual C++ Redistributable x64 necessário ao servidor.
3. Baixa o ZIP oficial do MySQL Community 8.4.13 e confere o checksum publicado pela Oracle.
4. Extrai o servidor em `C:\VanderCake\mysql` e escreve `C:\VanderCake\my.ini`.
5. Inicializa `C:\VanderCake\mysql-data` e mostra uma **senha temporária de root**.
6. Registra e inicia o serviço `VanderCakeMySQL`, com inicialização automática.

**Anote a senha temporária exibida.** Ela será usada uma única vez no próximo passo. Não compartilhe essa senha nem o conteúdo de arquivos de credenciais.

Para entender os comandos centrais, o script executa o equivalente a:

```powershell
& 'C:\VanderCake\mysql\bin\mysqld.exe' '--defaults-file=C:\VanderCake\my.ini' --initialize --console
& 'C:\VanderCake\mysql\bin\mysqld.exe' --install VanderCakeMySQL '--defaults-file=C:\VanderCake\my.ini'
Start-Service VanderCakeMySQL
Get-Service VanderCakeMySQL
```

**Não execute esses comandos de inicialização novamente se já executou o script.** `--initialize` prepara uma pasta nova; não é um comando para iniciar uma base existente. A ordem das opções na instalação do serviço também deve ser mantida.

O `my.ini` fixa o MySQL em `127.0.0.1`, porta 3306, UTF-8 e fuso UTC. O telefone acessará a API na porta 5080; não precisamos abrir a porta do banco na rede.

Se o serviço/porta já existir, identifique a instalação antes de prosseguir. Não apague a pasta de dados para resolver esse aviso. É possível utilizar um MySQL 8.4 existente, ajustando o caminho de `mysql.exe` e a porta nos scripts seguintes.

## 5. Troque a senha temporária de root

Ainda no terminal:

```powershell
& 'C:\VanderCake\mysql\bin\mysql.exe' --host=127.0.0.1 --port=3306 --user=root --password --connect-expired-password
```

Digite a senha temporária quando aparecer `Enter password:`. Ela não aparece enquanto você digita. Dentro do prompt do MySQL, execute:

```sql
ALTER USER 'root'@'localhost' IDENTIFIED BY 'SUBSTITUA_POR_UMA_SENHA_FORTE_SUA';
SELECT VERSION();
EXIT;
```

Substitua o texto de exemplo antes de executar. Guarde a senha definitiva. Ela administra o banco; não é a senha que você usará na tela de login da aplicação.

## 6. Crie tabelas, dados iniciais e usuário da API

Volte a um PowerShell normal, com o mesmo usuário do Windows que usará o projeto:

```powershell
Set-Location C:\VanderCake\VanderCake.Integracao
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\02-Criar-Banco.ps1
```

O script pede duas senhas em momentos diferentes:

- Primeiro, você **escolhe uma senha nova para `vandercake_app`**, a conta técnica da API.
- Depois, no prompt do MySQL, informa a **senha definitiva de root**, criada no passo anterior.

Ele aplica `Banco/01-estrutura.sql` e `Banco/02-dados-iniciais.sql`, cria a conta técnica com acesso somente ao schema `vandercake` e configura a conexão da API por `dotnet user-secrets`. A senha técnica não é gravada no código, no `appsettings.json`, na WEB ou no MOBILE. O arquivo SQL temporário fica restrito ao usuário atual e é removido ao terminar.

Os scripts não executam `DROP DATABASE` ou `DROP TABLE`. São scripts de instalação da versão 1. `CREATE TABLE IF NOT EXISTS` não converte uma estrutura antiga diferente: se você já tiver um banco VanderCake com tabelas anteriores, faça backup e compare os modelos antes de aplicar; não considere este arquivo uma migração automática.

Para uma instalação existente em outro local/porta:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\02-Criar-Banco.ps1 -MySqlExe 'C:\SeuMySQL\bin\mysql.exe' -Porta 3307
```

Se `vandercake_app` já existir, o script não troca sua senha silenciosamente. Após conferir se as tabelas foram criadas, configure a API com a senha atual:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\03-Configurar-Conexao.ps1
```

### Confira pelo cliente MySQL

```powershell
& 'C:\VanderCake\mysql\bin\mysql.exe' --host=127.0.0.1 --user=root --password
```

Dentro do MySQL:

```sql
USE vandercake;
SHOW TABLES;
SELECT COUNT(*) AS QuantidadeTabelas
FROM information_schema.tables
WHERE table_schema = DATABASE();

SELECT c.Descricao, p.NumeroParcela, p.DiasAposConfirmacao
FROM CondicoesPagamento c
JOIN ParcelasCondicaoPagamento p ON p.CondicaoPagamentoId = c.Id
ORDER BY c.Id, p.NumeroParcela;

SELECT * FROM FormasPagamento;
SHOW CREATE TABLE ParcelasPedido;
EXIT;
```

São **21 tabelas**: 17 de domínio e quatro de controle técnico. Você verá as condições à vista, 7/14 e 7/14/21; as formas Pix, boleto, dinheiro, cartão de crédito e cartão de débito. `ParcelasPedido.FormaPagamentoId` permite escolher um meio diferente para cada parcela.

Para estudar a aplicação manual dos arquivos, dentro do cliente MySQL você pode usar:

```sql
SOURCE C:/VanderCake/VanderCake.Integracao/Banco/01-estrutura.sql;
SOURCE C:/VanderCake/VanderCake.Integracao/Banco/02-dados-iniciais.sql;
```

Esses dois comandos não criam a conta técnica nem configuram a conexão. O arquivo `03-usuario-api.sql` é um modelo: seu marcador precisa ser substituído por uma senha corretamente escapada. Prefira o script PowerShell fornecido para isso. Não use `< arquivo.sql` no PowerShell como se fosse o Prompt de Comando; neste tutorial a importação usa `SOURCE`.

## 7. Compile a API e crie o primeiro administrador

```powershell
Set-Location C:\VanderCake\VanderCake.Integracao
dotnet restore .\VanderCake.Api\VanderCake.Api.csproj
dotnet build .\VanderCake.Api\VanderCake.Api.csproj
dotnet run --project .\VanderCake.Api\VanderCake.Api.csproj -- --criar-admin
```

O último comando mostra as cidades cadastradas e pede nome, CPF/CNPJ, endereço, cidade, telefone, e-mail e senha. Escolha o ID de uma cidade da lista. Use o e-mail e a senha definidos aqui para entrar na WEB. A senha é armazenada como hash, não como texto simples.

Não existe conta padrão de administrador ou de vendedor na base da loja. Para cadastrar administradores adicionais, execute esse comando novamente com outro documento/e-mail. Cadastros existentes não são sobrescritos.

Se sua cidade ainda não estiver na lista, cadastre-a pelo terminal antes de criar o administrador:

```sql
USE vandercake;
SELECT Id, Nome, Uf FROM Estados ORDER BY Nome;
INSERT INTO Cidades (EstadoId, Nome)
SELECT Id, 'NOME_DA_SUA_CIDADE' FROM Estados WHERE Uf = 'PR';
SELECT c.Id, c.Nome, e.Uf FROM Cidades c JOIN Estados e ON e.Id = c.EstadoId;
```

Troque o nome e a UF pelos dados corretos. Depois a própria WEB permite cadastrar novas cidades/estados.

## 8. Inicie a API e abra a WEB

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\04-Iniciar-API.ps1
```

Deixe essa janela aberta. Em outro PowerShell:

```powershell
Invoke-RestMethod 'http://localhost:5080/api/saude'
Start-Process 'http://localhost:5080'
```

O primeiro comando deve retornar `status: Disponível` e `versaoBanco: 1`. O navegador abrirá o login. Entre com o administrador criado no passo 7.

A API copia os arquivos de `VanderCake.Web/dist` para sua saída de compilação e serve a página no mesmo endereço. Depois de editar HTML, CSS ou JavaScript, pare a API com `Ctrl+C` e inicie novamente para copiar os arquivos atualizados.

O código em `Infrastructure/Banco.cs` lê `ConnectionStrings:VanderCake`; o script já configurou esse valor. Use `03-Configurar-Conexao.ps1` se mudar a senha técnica. Os user-secrets são uma configuração local de desenvolvimento do usuário do Windows, fora do projeto; não são um cofre de produção.

O link da demonstração WEB anterior continua independente. Para trabalhar com o MySQL deste computador, use a WEB local deste pacote em `http://localhost:5080`.

## 9. Cadastre os dados para testar

Na WEB:

1. Em **Vendedores**, cadastre um vendedor, com seu e-mail e senha. Ele começa ativo.
2. Em **Clientes**, cadastre pelo menos dois clientes e vincule-os ao mesmo vendedor.
3. Escolha o estado antes da cidade; somente as cidades desse estado aparecem.
4. Em **Produtos**, cadastre um produto com categoria, preço, descrição e estoque.
5. Se necessário, use **Nova Categoria+**, **Novo Estado+** e **Nova Cidade+**.
6. Em **Cadastros auxiliares**, inclua, altere ou desative categorias, condições e formas de pagamento. As condições seguem os prazos acordados: à vista, 7/14 ou 7/14/21. Condições/formas usadas em pedidos preservam descrição e prazos; desativá-las impede novas escolhas sem alterar o histórico. Uma categoria só pode ser desativada depois de mover ou desativar seus produtos ativos.

| Acesso | Quem usa | Onde é definido |
| --- | --- | --- |
| `root` do MySQL | Administração do banco e backups | Passo 5 |
| `vandercake_app` do MySQL | Somente a API | Passo 6 |
| Administrador da loja | Login da WEB | Passo 7 |
| Vendedor | Login do MOBILE | Cadastro na WEB |

Não tente usar `root` ou `vandercake_app` na tela de login.

## 10. Ligue o MOBILE ao computador

Use a pasta **VanderCake.Mobile deste pacote**, que já contém renovação de sessão, cliente SignalR e as alterações de sincronização.

O endereço fica em:

```text
VanderCake.Mobile/src/VanderCake.Mobile/Helpers/ApiConfig.cs
```

| Execução | Endereço |
| --- | --- |
| Aplicativo Windows no mesmo PC | `http://localhost:5080/api/` |
| Emulador Android padrão no PC | `http://10.0.2.2:5080/api/` |
| Aparelho Android físico | `http://IP_DO_COMPUTADOR:5080/api/` |

Para o emulador, deixe `EnderecoComputadorNaRede` vazio e compile em **Debug**. Para o aparelho físico:

1. Conecte o computador e o telefone à mesma rede local.
2. Execute `ipconfig` no computador e localize o IPv4 do adaptador utilizado.
3. Preencha a constante `EnderecoComputadorNaRede` com esse IP, mantendo `/api/` no final.
4. Inicie a API permitindo a rede local:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\04-Iniciar-API.ps1 -RedeLocal
```

Se o Firewall do Windows bloquear a conexão, em PowerShell como Administrador crie uma regra limitada à rede privada local:

```powershell
New-NetFirewallRule -DisplayName 'VanderCake API local' -Direction Inbound -Action Allow -Protocol TCP -LocalPort 5080 -Profile Private -RemoteAddress LocalSubnet
```

Use essa regra em uma rede privada de confiança. Não desligue o firewall. Não abra a porta 3306 nem encaminhe a 5080 no roteador para a Internet.

No navegador do telefone, teste `http://IP_DO_COMPUTADOR:5080/api/saude`. Só depois abra o aplicativo e entre com o vendedor cadastrado na WEB.

Para compilar em um ambiente MAUI já configurado:

```powershell
dotnet workload restore .\VanderCake.Mobile\src\VanderCake.Mobile\VanderCake.Mobile.csproj
dotnet build .\VanderCake.Mobile\src\VanderCake.Mobile\VanderCake.Mobile.csproj -f net10.0-android -c Debug
```

Se faltar Android SDK/JDK, configure o ambiente .NET MAUI/Android conforme a documentação da Microsoft; a instalação do MySQL não instala esses componentes. A compilação nativa Android não foi concluída no ambiente desta entrega. Veja `Docs/VALIDACAO.md`.

O modo Release continua exigindo um endereço HTTPS real; o domínio `.example` em `ApiConfig.cs` é um marcador. Não distribua essa configuração como versão de produção. Para o desenvolvimento local descrito aqui, use Debug; o manifesto Android Debug já permite HTTP local.

## 11. Faça o teste completo da venda

Use um produto de R$ 10,00 e estoque 11. Em três clientes do mesmo vendedor, faça pedidos de 10, 6 e 4 unidades:

1. No MOBILE, escolha os produtos e vá ao carrinho.
2. Escolha **7/14/21 dias**. Defina **Pix**, **boleto** e **dinheiro** nas parcelas 1, 2 e 3.
3. Toque em **Acrescentar Pedido à Lista** para cada cliente. Confira o pedido no histórico individual e na lista geral.
4. Na lista geral, toque em **Enviar para a Loja**.
5. Na WEB, abra **Pendentes** e **Ver Detalhes**. A prévia deve atender **6, 3 e 2 unidades**, respectivamente.
6. Confirme o envio. O estoque deve ficar em zero; as vendas somam R$ 110,00.
7. Sem tocar em Atualizar no MOBILE, confira os status, quantidades atendidas, valores e vencimentos.
8. No pedido de R$ 20,00, as parcelas finais devem ser R$ 6,66, R$ 6,66 e R$ 6,68. As formas continuam Pix, boleto e dinheiro.
9. Na WEB, entre em **Contas a receber**, filtre por cliente, vencimento e situação e registre o recebimento integral de uma parcela de teste. Confira no MOBILE a situação `Paga` e a data. Repetir a operação não cria outro recebimento. Pagamento parcial e estorno não estão incluídos.
10. Confira também os filtros de período em Vendas e Pendentes; eles usam a data de recebimento do lote no fuso da loja.

O corte é proporcional por produto **dentro do Pedido Geral**. Os restos são distribuídos pelas maiores frações; empates usam o menor ID do cliente. Lotes diferentes utilizam o estoque disponível quando são confirmados. Se ele mudar após a prévia, a API recusa a confirmação para que a loja reveja os valores.

Teste também: cancelar outro lote; alterar um produto na WEB e observar o catálogo; cadastrar um cliente e observar a carteira; desligar um vendedor e tentar usar sua sessão. Não é possível cancelar uma venda já confirmada usando os botões de pendentes. Um fluxo de estorno/devolução não faz parte desta etapa.

## 12. Como funciona o “tempo real”

A API registra um evento na mesma transação da alteração. Um serviço envia o aviso por SignalR; a WEB e o MOBILE consultam novamente os dados autorizados. Assim, uma resposta antiga não substitui as regras do servidor.

Quando conectados, os avisos são processados normalmente em poucos segundos; não há garantia de latência em uma rede indisponível. Também existe recuperação por consulta a cada 30 segundos, reconexão automática e sincronização ao retomar o aplicativo/guia. No Android em segundo plano, o sistema pode suspender a conexão; o aplicativo reconcilia os dados ao voltar ao primeiro plano. Não foi implementado serviço permanente de segundo plano nem notificação push do sistema operacional.

O MOBILE mantém carrinhos e lotes em montagem no SQLite durante a sincronização. Uma resposta perdida deixa o envio protegido; a retomada consulta/reenvia a mesma chave e o mesmo conteúdo. O banco impede duplicar esse lote. A sessão fica no SecureStorage, com renovação quando necessária; conta desligada, senha alterada ou renovação vencida exige novo login, preservando os rascunhos.

## 13. Execute os testes automatizados

Para os testes JavaScript, instale Node.js LTS caso ainda não o tenha. Ele não é necessário para executar a WEB servida pela API.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\05-Testar.ps1
```

Esse comando compila a API e executa os 65 testes do MOBILE e os 24 da WEB. Os testes do MOBILE usam SQLite real e uma API simulada; os testes de integração abaixo usam a API e o MySQL reais.

### Integração com MySQL real, sem alterar a base da loja

O executor cria um schema exclusivo com prefixo `vandercake_teste_`, aplica a estrutura, inicia outra API em uma porta livre e testa seus endpoints. Não apaga schemas automaticamente. Precisa de uma conexão administrativa **destinada a testes**, porque cria esse schema.

Exemplo no computador de desenvolvimento, usando root local e pedindo a senha sem imprimi-la:

```powershell
$segredoTeste = Read-Host 'Senha de root para criar um banco isolado de testes' -AsSecureString
$ponteiroTeste = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segredoTeste)
try {
    $conexaoTeste = New-Object System.Data.Common.DbConnectionStringBuilder
    $conexaoTeste['Server'] = '127.0.0.1'
    $conexaoTeste['Port'] = 3306
    $conexaoTeste['User ID'] = 'root'
    $conexaoTeste['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiroTeste)
    $conexaoTeste['SslMode'] = 'Preferred'
    $env:VANDERCAKE_TEST_MYSQL = $conexaoTeste.ConnectionString
    # Opcional, com Node.js instalado: testa tambem o cliente HTTP/SignalR da WEB.
    $env:VANDERCAKE_TESTAR_WEB_HTTP = '1'
    dotnet run --project .\TestesIntegracao\TestesIntegracao.csproj
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiroTeste)
    $segredoTeste.Dispose()
    Remove-Item Env:\VANDERCAKE_TEST_MYSQL -ErrorAction SilentlyContinue
    Remove-Item Env:\VANDERCAKE_TESTAR_WEB_HTTP -ErrorAction SilentlyContinue
    $conexaoTeste = $null
}
```

O nome do schema de testes e do usuário técnico isolado aparecem no resultado. Ambos ficam disponíveis para inspeção; não são apagados automaticamente. As contas fictícias desse executor existem somente nesse schema, e não servem para entrar na loja configurada nos passos anteriores. Confira os resultados e limitações em `Docs/VALIDACAO.md`.

## 14. Uso diário e backup

O MySQL inicia com o Windows. A API precisa estar em execução para a WEB e o MOBILE se comunicarem. Nesta etapa, ela é iniciada pelo script; não foi instalada como serviço nem publicada na Internet.

```powershell
Get-Service VanderCakeMySQL
powershell -NoProfile -ExecutionPolicy Bypass -File .\Scripts\04-Iniciar-API.ps1
```

Para backup consistente, crie uma pasta e use o arquivo de saída do próprio `mysqldump`, evitando problemas de codificação da redireção do PowerShell:

```powershell
New-Item -ItemType Directory -Path C:\VanderCake\backups -Force
$arquivoBackup = 'C:\VanderCake\backups\vandercake-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.sql'
& 'C:\VanderCake\mysql\bin\mysqldump.exe' --host=127.0.0.1 --user=root --password --single-transaction --routines --triggers --no-tablespaces "--result-file=$arquivoBackup" vandercake
```

Confira `$LASTEXITCODE`: deve ser zero. Guarde uma cópia do backup fora desse computador. Para verificar um backup, restaure primeiro em um banco separado de teste; não execute uma restauração sobre a loja em uso sem backup e planejamento.

As chaves que protegem os tokens ficam em `%LOCALAPPDATA%\VanderCake\ApiKeys`; no Windows são protegidas pelo DPAPI do usuário que roda a API. Mantenha a API sob a mesma conta do Windows. A perda dessas chaves invalida sessões e prévias existentes, sem apagar vendas ou pedidos.

## 15. Solução de problemas frequentes

| Sintoma | Verificação |
| --- | --- |
| `mysql` não reconhecido | Use o caminho completo para `mysql.exe` mostrado no tutorial |
| Porta 3306 ocupada | Identifique o MySQL/serviço existente; não reinstale nem apague dados |
| Serviço não inicia | Confira `C:\VanderCake\mysql-data\*.err` e os caminhos em `my.ini` |
| `Access denied` | Confira usuário, senha, host e porta; `vandercake_app` foi criado para `127.0.0.1` |
| API retorna 503 | Verifique serviço, schema e user-secrets sob a mesma conta do Windows |
| Nenhuma venda aparece | Primeiro envie um lote no MOBILE e confirme em Pendentes |
| MOBILE não conecta no aparelho | Use IPv4 do PC, API com `-RedeLocal`, mesma rede e firewall privado |
| Emulador não conecta | Use `10.0.2.2`, não o `localhost` do Android |
| Login volta a pedir credenciais | Conta pode estar inativa, senha alterada ou renovação expirada |
| Cadastro/confirmar retorna 409 | Outra operação alterou os dados; atualize e confira antes de salvar |
| Produto tem preço diferente no carrinho | Use a revisão explícita de preços; o servidor não aceita preço antigo silenciosamente |
| Browser mostra dados de demonstração | Abra a WEB deste pacote em `http://localhost:5080`; `config.js` deve estar em modo `api` |

Se trocar a origem da WEB para outra porta, configure a origem exata em `Cors:Origens` na API e o endereço em `dist/js/config.js`. Na configuração padrão deste tutorial não é necessário alterar CORS: WEB e API compartilham a mesma origem.

## Fontes oficiais e escopo da validação

- MySQL 8.4 por ZIP: https://dev.mysql.com/doc/refman/8.4/en/windows-install-archive.html
- Inicialização do diretório: https://dev.mysql.com/doc/refman/8.4/en/data-directory-initialization.html
- Serviço no Windows: https://dev.mysql.com/doc/refman/8.4/en/windows-start-service.html
- Download Community 8.4: https://dev.mysql.com/downloads/mysql/8.4.html
- Visual C++ Redistributable: https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist
- SDK .NET no Windows: https://learn.microsoft.com/en-us/dotnet/core/install/windows
- User-secrets: https://learn.microsoft.com/pt-br/aspnet/core/security/app-secrets?view=aspnetcore-10.0
- SignalR JavaScript: https://learn.microsoft.com/en-us/aspnet/core/signalr/javascript-client?view=aspnetcore-10.0
- Ambiente Android: https://aka.ms/dotnet-android-install-sdk

As referências e downloads foram conferidos em 27/10/2026. Os testes de API, SQL e sincronização foram executados em Linux com MySQL 8.4.13. A instalação do serviço e os scripts PowerShell precisam ser executados e validados no seu Windows. Os resultados não constituem garantia de ausência de erros; o roteiro de testes no seu computador e no aparelho continua necessário.
