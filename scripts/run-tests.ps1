param(
    [ValidateSet('all', 'unit', 'ui', 'smoke')]
    [string]$Suite = 'all',

    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$scriptDir = Split-Path -Parent $PSCommandPath
$reportScript = Join-Path $scriptDir 'generate-report.ps1'

if (-not (Test-Path $reportScript)) {
    throw "No se encontro el script de reportes: $reportScript"
}

Write-Host "Ejecutando suite '$Suite' con generacion de reporte..."
& $reportScript -Suite $Suite -Configuration $Configuration
exit $LASTEXITCODE
