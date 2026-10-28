# Testes locais de regressao. Integracao MySQL possui instrucoes proprias no tutorial.
$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
& dotnet build (Join-Path $raiz 'VanderCake.Api\VanderCake.Api.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Falha na compilacao da API.' }
& dotnet run --project (Join-Path $raiz 'VanderCake.Mobile\tests\VanderCake.Tests\VanderCake.Tests.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes do Mobile.' }
Push-Location (Join-Path $raiz 'VanderCake.Web')
try {
    & npm test
    if ($LASTEXITCODE -ne 0) { throw 'Falha nos testes da Web.' }
}
finally { Pop-Location }
