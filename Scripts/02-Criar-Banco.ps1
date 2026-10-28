# Execute depois de definir a senha definitiva de root. Nao altera contas existentes.
[CmdletBinding()]
param([string]$MySqlExe = 'C:\VanderCake\mysql\bin\mysql.exe', [int]$Porta = 3306)
$ErrorActionPreference = 'Stop'
$projeto = Split-Path $PSScriptRoot -Parent
$api = Join-Path $projeto 'VanderCake.Api\VanderCake.Api.csproj'
if (-not (Test-Path $MySqlExe)) { throw 'mysql.exe nao encontrado. Confira -MySqlExe.' }
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Instale o SDK .NET 10 e reabra o terminal.' }
Write-Host 'Escolha a senha do usuario TECNICO vandercake_app. Nao e a senha de login da WEB.'
$segredo = Read-Host 'Senha do usuario da API (minimo 12 caracteres)' -AsSecureString
$ponteiro = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segredo)
try { $senha = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiro) }
finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiro) }
if ($senha.Length -lt 12) { throw 'Use pelo menos 12 caracteres para a conta tecnica.' }
if ($senha.Contains("`r") -or $senha.Contains("`n")) { throw 'A senha nao pode conter quebras de linha.' }
$arquivo = Join-Path ([IO.Path]::GetTempPath()) ('vandercake-setup-' + [Guid]::NewGuid().ToString('N') + '.sql')
try {
    New-Item -ItemType File -Path $arquivo | Out-Null
    $acl = New-Object Security.AccessControl.FileSecurity
    $acl.SetAccessRuleProtection($true, $false)
    $regra = New-Object Security.AccessControl.FileSystemAccessRule(
        [Security.Principal.WindowsIdentity]::GetCurrent().User, 'FullControl', 'Allow')
    $acl.AddAccessRule($regra)
    Set-Acl -Path $arquivo -AclObject $acl
    $senhaSql = "'" + $senha.Replace('\','\\').Replace("'","''") + "'"
    # CREATE USER falha se ja existir, sem trocar sua senha.
    $usuario = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\03-usuario-api.sql')).Replace('{{SENHA_ESCAPADA}}', $senhaSql)
    # O GRANT requer o banco. CREATE DATABASE nao apaga uma base existente.
    $estrutura = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\01-estrutura.sql'))
    $iniciais = [IO.File]::ReadAllText((Join-Path $projeto 'Banco\02-dados-iniciais.sql'))
    [IO.File]::WriteAllText($arquivo, $estrutura + "`r`n" + $iniciais + "`r`n" + $usuario, (New-Object Text.UTF8Encoding($false)))
    Write-Host 'Agora digite a senha definitiva de root no prompt do MySQL.'
    $source = $arquivo.Replace('\','/')
    & $MySqlExe --host=127.0.0.1 "--port=$Porta" --user=root --password --default-character-set=utf8mb4 --abort-source-on-error "--execute=SOURCE $source"
    if ($LASTEXITCODE -ne 0) { throw 'O SQL nao foi concluido. A conexao da API nao foi alterada. Confira a mensagem do MySQL.' }
    $cs = New-Object System.Data.Common.DbConnectionStringBuilder
    $cs['Server'] = '127.0.0.1'
    $cs['Port'] = $Porta
    $cs['Database'] = 'vandercake'
    $cs['User ID'] = 'vandercake_app'
    $cs['Password'] = $senha
    $cs['SslMode'] = 'Preferred'
    $json = @{ 'ConnectionStrings:VanderCake' = $cs.ConnectionString } | ConvertTo-Json -Compress
    $OutputEncoding = New-Object Text.UTF8Encoding($false)
    $json | & dotnet user-secrets set --project $api
    if ($LASTEXITCODE -ne 0) { throw 'Banco criado, mas a configuracao da API falhou. Use 03-Configurar-Conexao.ps1.' }
    Write-Host 'Banco e usuario criados; conexao da API configurada. Crie o administrador com --criar-admin.'
}
finally {
    if (Test-Path $arquivo) { Remove-Item $arquivo -Force }
    $senha = $null
    $senhaSql = $null
    $json = $null
    $segredo.Dispose()
}
