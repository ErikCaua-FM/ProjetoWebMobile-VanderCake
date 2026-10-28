# Use se a conta vandercake_app ja foi criada ou se a senha do banco mudou.
[CmdletBinding()]
param([string]$Servidor = '127.0.0.1', [int]$Porta = 3306)
$ErrorActionPreference = 'Stop'
$api = Join-Path (Split-Path $PSScriptRoot -Parent) 'VanderCake.Api\VanderCake.Api.csproj'
$segredo = Read-Host 'Senha atual do usuario MySQL vandercake_app' -AsSecureString
$ponteiro = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($segredo)
try {
    $cs = New-Object System.Data.Common.DbConnectionStringBuilder
    $cs['Server'] = $Servidor
    $cs['Port'] = $Porta
    $cs['Database'] = 'vandercake'
    $cs['User ID'] = 'vandercake_app'
    $cs['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ponteiro)
    $cs['SslMode'] = 'Preferred'
    $json = @{ 'ConnectionStrings:VanderCake' = $cs.ConnectionString } | ConvertTo-Json -Compress
    $OutputEncoding = New-Object Text.UTF8Encoding($false)
    $json | & dotnet user-secrets set --project $api
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao salvar a conexao da API.' }
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ponteiro)
    $segredo.Dispose()
    $json = $null
}
