param(
    [ValidateSet('all', 'unit', 'ui', 'smoke')]
    [string]$Suite = 'all',

    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

function Convert-ToMilliseconds {
    param(
        [string]$Duration,
        [string]$StartTime,
        [string]$EndTime
    )

    if (-not [string]::IsNullOrWhiteSpace($Duration)) {
        $ts = [TimeSpan]::Zero
        if ([TimeSpan]::TryParse($Duration, [ref]$ts)) {
            return [int][Math]::Round($ts.TotalMilliseconds)
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($StartTime) -and -not [string]::IsNullOrWhiteSpace($EndTime)) {
        $start = [datetime]::Parse($StartTime)
        $end = [datetime]::Parse($EndTime)
        return [int][Math]::Round(($end - $start).TotalMilliseconds)
    }

    return 0
}

function Simplify-TestName {
    param([string]$Name)

    if ([string]::IsNullOrWhiteSpace($Name)) {
        return $Name
    }

    if ($Name.Contains('.')) {
        return $Name.Split('.')[-1]
    }

    return $Name
}

function Ensure-ReportGenerator {
    $tool = Get-Command reportgenerator -ErrorAction SilentlyContinue
    if ($tool) {
        return $true
    }

    Write-Host 'Instalando ReportGenerator (dotnet global tool)...'
    dotnet tool install --global dotnet-reportgenerator-globaltool | Out-Null
    $env:PATH = "$env:PATH;$([Environment]::GetFolderPath('UserProfile'))\\.dotnet\\tools"

    $tool = Get-Command reportgenerator -ErrorAction SilentlyContinue
    return [bool]$tool
}

function Ensure-Newman {
    $newman = Get-Command newman -ErrorAction SilentlyContinue
    if ($newman) {
        return $true
    }

    $npm = Get-Command npm -ErrorAction SilentlyContinue
    if (-not $npm) {
        throw 'No se encontro npm. Instala Node.js para ejecutar pruebas de humo con Newman.'
    }

    Write-Host 'Instalando Newman y newman-reporter-htmlextra...'
    npm install -g newman newman-reporter-htmlextra | Out-Null

    $newman = Get-Command newman -ErrorAction SilentlyContinue
    return [bool]$newman
}

function Test-UrlReady {
    param(
        [string]$Url,
        [int]$Attempts = 30,
        [int]$DelayMs = 1000
    )

    for ($i = 0; $i -lt $Attempts; $i++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 500) {
                return $true
            }
        }
        catch {
        }

        Start-Sleep -Milliseconds $DelayMs
    }

    return $false
}

function Run-SmokeReport {
    param(
        [string]$RunDir,
        [string]$Configuration
    )

    $collection = 'Pruebas de humo - Hotel Osiris.json'
    if (-not (Test-Path $collection)) {
        throw "No se encontro la coleccion de humo: $collection"
    }

    if (-not (Ensure-Newman)) {
        throw 'No se pudo instalar/encontrar Newman.'
    }

    $baseUrl = 'http://127.0.0.1:5000/'
    $startedByScript = $false
    $webProcess = $null

    if (-not (Test-UrlReady -Url $baseUrl -Attempts 2 -DelayMs 500)) {
        Write-Host 'Iniciando aplicacion web local para pruebas de humo...'
        $startInfo = New-Object System.Diagnostics.ProcessStartInfo
        $startInfo.FileName = 'dotnet'
        $startInfo.Arguments = 'run --project "HotelProyecto/HotelProyecto.csproj" --configuration Release --urls "http://127.0.0.1:5000"'
        $startInfo.WorkingDirectory = (Get-Location).Path
        $startInfo.UseShellExecute = $false
        $startInfo.RedirectStandardOutput = $true
        $startInfo.RedirectStandardError = $true
        $startInfo.CreateNoWindow = $true

        $webProcess = [System.Diagnostics.Process]::Start($startInfo)
        $startedByScript = $true

        if (-not (Test-UrlReady -Url $baseUrl -Attempts 60 -DelayMs 1000)) {
            $out = ''
            $err = ''
            if ($webProcess) {
                $out = $webProcess.StandardOutput.ReadToEnd()
                $err = $webProcess.StandardError.ReadToEnd()
            }
            throw "La aplicacion web no inicio para smoke tests. Output: $out Error: $err"
        }
    }

    $htmlReport = Join-Path $RunDir 'report.html'
    $junitReport = Join-Path $RunDir 'newman-results.xml'

    Write-Host 'Running smoke tests with Newman...'
    
    # Crear un archivo HTML personalizado con estilo consistente con otros reportes
    $tempHtmlReport = Join-Path $RunDir 'newman-report-temp.html'
    $args = @(
        'run',
        $collection,
        '--reporters',
        'cli,htmlextra,junit',
        '--reporter-htmlextra-export',
        $tempHtmlReport,
        '--reporter-junit-export',
        $junitReport
    )

    & newman @args
    $newmanExitCode = $LASTEXITCODE

    if ($startedByScript -and $webProcess -and -not $webProcess.HasExited) {
        $webProcess.Kill()
        $webProcess.Dispose()
    }

    # Renombrar el reporte Newman generado a report.html para consistencia
    if (Test-Path $tempHtmlReport) {
        Copy-Item -Path $tempHtmlReport -Destination $htmlReport -Force
        Remove-Item -Path $tempHtmlReport -Force -ErrorAction SilentlyContinue
    }

    Write-Host ''
    Write-Host '========================================='
    Write-Host 'Reportes Generados'
    Write-Host '========================================='
    Write-Host "Reporte de humo (HTML): $htmlReport"
    Write-Host "Reporte de humo (JUnit): $junitReport"
    Write-Host '========================================='

    exit $newmanExitCode
}

$projectPath = 'HotelProyecto.Tests/HotelProyecto.Tests.csproj'
$resultsRoot = 'HotelProyecto.Tests/TestResults'
$runDir = Join-Path $resultsRoot ("Run-$Suite")

if (Test-Path $runDir) {
    Remove-Item -Path (Join-Path $runDir '*') -Recurse -Force -ErrorAction SilentlyContinue
}

New-Item -ItemType Directory -Force -Path $runDir | Out-Null

if ($Suite -eq 'smoke') {
    Run-SmokeReport -RunDir $runDir -Configuration $Configuration
    return
}

$trxFileName = 'TestResults.trx'
$trxPath = Join-Path $runDir $trxFileName

$dotnetArgs = @(
    'test',
    $projectPath,
    '--configuration',
    $Configuration,
    '--nologo',
    '--verbosity',
    'minimal',
    '--logger',
    "trx;LogFileName=$trxFileName",
    '--results-directory',
    $runDir,
    '--collect:XPlat Code Coverage'
)

switch ($Suite) {
    'unit' {
        $dotnetArgs += @('--filter', 'FullyQualifiedName~PruebasUnitarias')
    }
    'ui' {
        $env:UI_HEADLESS = 'false'
        $dotnetArgs += @('--filter', 'FullyQualifiedName~PruebasFuncionalesUI')
    }
}

Write-Host 'Running tests with coverage...'
$wallClock = [System.Diagnostics.Stopwatch]::StartNew()
$dotnetOutput = & dotnet @dotnetArgs 2>&1
$dotnetExitCode = $LASTEXITCODE
$wallClock.Stop()

if ($dotnetOutput) {
    $dotnetOutput | ForEach-Object { Write-Host $_ }
}

if (-not (Test-Path $trxPath)) {
    throw "No se encontro el archivo TRX esperado: $trxPath"
}

[xml]$trx = Get-Content -Raw -Path $trxPath
$nsManager = New-Object System.Xml.XmlNamespaceManager($trx.NameTable)
$nsManager.AddNamespace('t', 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010')

$counters = $trx.SelectSingleNode('//t:ResultSummary/t:Counters', $nsManager)
$timesNode = $trx.SelectSingleNode('//t:Times', $nsManager)
$results = $trx.SelectNodes('//t:Results/t:UnitTestResult', $nsManager)

$total = [int]$counters.total
$passed = [int]$counters.passed
$failed = [int]$counters.failed + [int]$counters.error + [int]$counters.timeout + [int]$counters.aborted
$skipped = [int]$counters.notExecuted + [int]$counters.notRunnable + [int]$counters.inconclusive
$resultText = if ($failed -eq 0) { 'SUCCEEDED' } else { 'FAILED' }

$executionMs = 0
if ($timesNode -and $timesNode.start -and $timesNode.finish) {
    $executionMs = [int][Math]::Round(([datetime]::Parse($timesNode.finish) - [datetime]::Parse($timesNode.start)).TotalMilliseconds)
}

$rows = New-Object System.Collections.Generic.List[string]
foreach ($result in $results) {
    $name = Simplify-TestName -Name $result.testName
    $ms = Convert-ToMilliseconds -Duration $result.duration -StartTime $result.startTime -EndTime $result.endTime

    $rawOutcome = $result.outcome
    if ([string]::IsNullOrWhiteSpace($rawOutcome)) {
        $rawOutcome = 'Unknown'
    }

    $outcome = $rawOutcome.ToUpperInvariant()
    $cssClass = if ($outcome -eq 'PASSED') { 'ok' } elseif ($outcome -eq 'FAILED') { 'bad' } else { 'skip' }

    $message = ''
    $msgNode = $result.SelectSingleNode('.//t:Message', $nsManager)
    if ($msgNode) {
        $message = $msgNode.InnerText
    }

    $safeName = [System.Net.WebUtility]::HtmlEncode($name)
    $safeOutcome = [System.Net.WebUtility]::HtmlEncode($outcome)
    $safeMessage = [System.Net.WebUtility]::HtmlEncode($message)

    $rows.Add("<tr><td>$safeName</td><td class='$cssClass'>$safeOutcome</td><td>$ms ms</td><td><pre>$safeMessage</pre></td></tr>")
}

$testReportPath = Join-Path $runDir 'report.html'
$generatedAt = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss')
$totalMs = [int][Math]::Round($wallClock.Elapsed.TotalMilliseconds)

$html = @"
<!doctype html>
<html lang='es'>
<head>
<meta charset='utf-8' />
<meta name='viewport' content='width=device-width, initial-scale=1' />
<title>Test Report - HotelProyecto</title>
<style>
body { font-family: Segoe UI, Arial, sans-serif; margin: 24px; background: #f4f7fb; color: #1a1a1a; }
header { margin-bottom: 16px; }
h1 { margin: 0 0 4px 0; }
.meta { color: #555; }
.grid { display: grid; grid-template-columns: repeat(4, minmax(120px, 1fr)); gap: 10px; margin: 16px 0; }
.card { background: white; border-radius: 10px; padding: 14px; box-shadow: 0 1px 4px rgba(0,0,0,0.08); }
.k { font-size: 12px; color: #666; text-transform: uppercase; }
.v { font-size: 22px; font-weight: 700; margin-top: 4px; }
table { width: 100%; border-collapse: collapse; background: white; border-radius: 10px; overflow: hidden; }
th, td { border-bottom: 1px solid #eee; padding: 10px; text-align: left; vertical-align: top; }
th { background: #0f172a; color: white; }
.ok { color: #0a7a34; font-weight: 700; }
.bad { color: #b91c1c; font-weight: 700; }
.skip { color: #9a6700; font-weight: 700; }
pre { margin: 0; white-space: pre-wrap; max-width: 680px; }
</style>
</head>
<body>
<header>
  <h1>Test Report - $Suite</h1>
  <div class='meta'>Generado: $generatedAt</div>
  <div class='meta'>Resultado general: $resultText | Ejecucion: $executionMs ms | Total (incl. build): $totalMs ms</div>
</header>
<div class='grid'>
  <div class='card'><div class='k'>Total</div><div class='v'>$total</div></div>
  <div class='card'><div class='k'>Passed</div><div class='v'>$passed</div></div>
  <div class='card'><div class='k'>Failed</div><div class='v'>$failed</div></div>
  <div class='card'><div class='k'>Skipped</div><div class='v'>$skipped</div></div>
</div>
<table>
  <thead>
    <tr><th>Test</th><th>Status</th><th>Duration</th><th>Error</th></tr>
  </thead>
  <tbody>
    $($rows -join [Environment]::NewLine)
  </tbody>
</table>
</body>
</html>
"@

Set-Content -Path $testReportPath -Value $html -Encoding UTF8

$coverageXml = Get-ChildItem -Path $runDir -Filter 'coverage.cobertura.xml' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
$coverageIndex = ''

if ($coverageXml) {
    if (Ensure-ReportGenerator) {
        $coverageDir = Join-Path $runDir 'CoverageReport'
        New-Item -ItemType Directory -Force -Path $coverageDir | Out-Null
        & reportgenerator "-reports:$($coverageXml.FullName)" "-targetdir:$coverageDir" '-reporttypes:Html' | Out-Null
        $coverageIndex = Join-Path $coverageDir 'index.html'
    }
    else {
        Write-Host 'No se pudo instalar/encontrar reportgenerator. Solo se genero el reporte HTML de pruebas.'
    }
}
else {
    Write-Host 'No se encontro archivo de cobertura (coverage.cobertura.xml).'
}

Write-Host ''
Write-Host '========================================='
Write-Host 'Reportes Generados'
Write-Host '========================================='
Write-Host "Reporte de pruebas (HTML): $testReportPath"
if ($coverageIndex -and (Test-Path $coverageIndex)) {
    Write-Host "Reporte de cobertura (HTML): $coverageIndex"
}
Write-Host '========================================='

exit $dotnetExitCode
