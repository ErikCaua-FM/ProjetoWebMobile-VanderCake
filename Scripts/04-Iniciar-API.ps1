[CmdletBinding()]
param([switch]$RedeLocal)
$ErrorActionPreference = 'Stop'
$api = Join-Path (Split-Path $PSScriptRoot -Parent) 'VanderCake.Api\VanderCake.Api.csproj'
# Development carrega os user-secrets do usuario que executar este terminal.
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$endereco = if ($RedeLocal) { 'http://0.0.0.0:5080' } else { 'http://localhost:5080' }
& dotnet run --project $api --no-launch-profile -- --urls $endereco
if ($LASTEXITCODE -ne 0) { throw 'A API encerrou com erro. Confira as mensagens acima.' }
