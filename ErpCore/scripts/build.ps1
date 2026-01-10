# ErpCore 建置腳本 (Windows)
# 用途: 建置整個 ErpCore Solution

param(
    [string]$Configuration = "Release",
    [switch]$Clean = $false
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore 建置腳本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 設定路徑
$SolutionPath = Join-Path $PSScriptRoot ".." "ErpCore.sln"
$SolutionPath = Resolve-Path $SolutionPath

Write-Host "Solution 路徑: $SolutionPath" -ForegroundColor Yellow
Write-Host "建置設定: $Configuration" -ForegroundColor Yellow
Write-Host ""

# 清理 (如果指定)
if ($Clean) {
    Write-Host "清理舊的建置檔案..." -ForegroundColor Yellow
    dotnet clean $SolutionPath --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Host "清理失敗!" -ForegroundColor Red
        exit 1
    }
    Write-Host "清理完成" -ForegroundColor Green
    Write-Host ""
}

# 還原 NuGet 套件
Write-Host "還原 NuGet 套件..." -ForegroundColor Yellow
dotnet restore $SolutionPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "還原失敗!" -ForegroundColor Red
    exit 1
}
Write-Host "還原完成" -ForegroundColor Green
Write-Host ""

# 建置 Solution
Write-Host "建置 Solution..." -ForegroundColor Yellow
dotnet build $SolutionPath --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "建置失敗!" -ForegroundColor Red
    exit 1
}
Write-Host "建置完成" -ForegroundColor Green
Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "建置成功完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan

