param(
    [string]$Filter = "MovementFlow_Integration_Tests"
)

$ErrorActionPreference = "Stop"

# Bu runner normal dotnet test ciktisina ek olarak testlerin urettiği
# markdown raporu terminale basar. Amac: test gecince sadece "Passed"
# gormek degil, her adimdan sonra hangi tablonun nasil degistigini okumak.
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "test\InventoryTrackingAutomation.EntityFrameworkCore.Tests\InventoryTrackingAutomation.EntityFrameworkCore.Tests.csproj"
$logDirectory = Join-Path $root "test\InventoryTrackingAutomation.EntityFrameworkCore.Tests\TestResults\movement-flow-logs"
$latestReport = Join-Path $logDirectory "latest.md"
$startedAt = Get-Date

Write-Host ""
Write-Host "Movement flow testleri calisiyor..."
Write-Host "Filter: $Filter"
Write-Host ""

& dotnet test $project --no-restore --filter $Filter --logger "console;verbosity=minimal"
$testExitCode = $LASTEXITCODE

Write-Host ""
Write-Host "================ MOVEMENT FLOW ADIM RAPORU ================"

if (Test-Path -LiteralPath $latestReport)
{
    $report = Get-Item -LiteralPath $latestReport

    if ($report.LastWriteTime -lt $startedAt)
    {
        Write-Warning "latest.md bu kosuda yenilenmemis gorunuyor. Build/test discovery patlamissa rapor eski olabilir."
    }

    Write-Host "Rapor dosyasi: $latestReport"
    Write-Host ""
    Get-Content -LiteralPath $latestReport
}
else
{
    Write-Warning "Movement flow raporu olusmadi: $latestReport"
}

Write-Host "==========================================================="
exit $testExitCode
