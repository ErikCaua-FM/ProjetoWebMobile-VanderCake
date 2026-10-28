# Execute em PowerShell como Administrador. Somente para instalacao NOVA.
[CmdletBinding()]
param([string]$Pasta = 'C:\VanderCake', [string]$Versao = '8.4.13')
$ErrorActionPreference = 'Stop'
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Abra o PowerShell como Administrador para instalar o servico.'
}
$mysql = Join-Path $Pasta 'mysql'
$dados = Join-Path $Pasta 'mysql-data'
$ini = Join-Path $Pasta 'my.ini'
if ((Get-Service -Name 'VanderCakeMySQL' -ErrorAction SilentlyContinue) -or
    (Test-Path $dados) -or (Test-Path $mysql) -or (Test-Path $ini)) {
    throw 'Ja existem arquivos ou servico VanderCakeMySQL. Nao inicialize novamente; consulte o tutorial.'
}
if (Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorAction SilentlyContinue) {
    throw 'A porta 3306 ja esta ocupada. Identifique a instalacao existente antes de continuar.'
}
New-Item -ItemType Directory -Path $Pasta -Force | Out-Null
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$redistribuivel = Join-Path $Pasta 'vc_redist.x64.exe'
Invoke-WebRequest 'https://aka.ms/vc14/vc_redist.x64.exe' -OutFile $redistribuivel -UseBasicParsing
$resultado = Start-Process $redistribuivel -ArgumentList '/install','/quiet','/norestart' -Wait -PassThru
if ($resultado.ExitCode -notin @(0, 3010, 1638)) { throw "Falha no Visual C++ Redistributable: $($resultado.ExitCode)" }
$zip = Join-Path $Pasta "mysql-$Versao-winx64.zip"
Invoke-WebRequest "https://cdn.mysql.com/Downloads/MySQL-8.4/mysql-$Versao-winx64.zip" -OutFile $zip -UseBasicParsing
if ($Versao -eq '8.4.13' -and (Get-FileHash $zip -Algorithm MD5).Hash -ne '7B94489D5524DEA9B27097068BEA25A8') {
    throw 'O arquivo baixado nao corresponde ao checksum publicado pela Oracle. Nao foi instalado.'
}
$extracao = Join-Path $Pasta ('extracao-' + [Guid]::NewGuid().ToString('N'))
Expand-Archive -Path $zip -DestinationPath $extracao
Move-Item (Join-Path $extracao "mysql-$Versao-winx64") $mysql
Remove-Item $extracao
$baseSql = $mysql.Replace('\','/')
$dadosSql = $dados.Replace('\','/')
@"
[mysqld]
basedir=$baseSql
datadir=$dadosSql
port=3306
bind-address=127.0.0.1
mysqlx=0
character-set-server=utf8mb4
collation-server=utf8mb4_0900_ai_ci
default-time-zone=+00:00

[client]
port=3306
default-character-set=utf8mb4
"@ | Set-Content $ini -Encoding ASCII
$mysqld = Join-Path $mysql 'bin\mysqld.exe'
Write-Host 'Anote a senha TEMPORARIA de root que aparecera abaixo. A inicializacao e executada uma unica vez.'
& $mysqld "--defaults-file=$ini" --initialize --console
if ($LASTEXITCODE -ne 0) { throw 'Falha na inicializacao. Preserve a pasta de dados e consulte o log.' }
# Na instalacao do servico, --install deve vir antes de --defaults-file.
& $mysqld --install VanderCakeMySQL "--defaults-file=$ini"
if ($LASTEXITCODE -ne 0) { throw 'Nao foi possivel registrar o servico.' }
Start-Service VanderCakeMySQL
Set-Service VanderCakeMySQL -StartupType Automatic
Get-Service VanderCakeMySQL
Write-Host 'MySQL instalado. Proximo passo: trocar a senha temporaria de root, conforme o tutorial.'
