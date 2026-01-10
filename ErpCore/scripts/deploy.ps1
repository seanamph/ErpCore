# ErpCore 部署腳本 (Windows)
# 用途: 部署 ErpCore 應用程式

param(
    [string]$Environment = "Production",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore 部署腳本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "部署環境: $Environment" -ForegroundColor Yellow
Write-Host "建置設定: $Configuration" -ForegroundColor Yellow
Write-Host ""

# 設定路徑
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$SolutionPath = Join-Path $ScriptPath ".." "ErpCore.sln"
$ApiProjectPath = Join-Path $ScriptPath ".." "src" "ErpCore.Api" "ErpCore.Api.csproj"

# 建置專案
Write-Host "建置專案..." -ForegroundColor Yellow
& "$ScriptPath\build.ps1" -Configuration $Configuration -Clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "建置失敗!" -ForegroundColor Red
    exit 1
}

# 發佈 API 專案
Write-Host "發佈 API 專案..." -ForegroundColor Yellow
$PublishPath = Join-Path $ScriptPath ".." "publish" "ErpCore.Api"
dotnet publish $ApiProjectPath --configuration $Configuration --output $PublishPath --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "發佈失敗!" -ForegroundColor Red
    exit 1
}
Write-Host "發佈完成: $PublishPath" -ForegroundColor Green
Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "部署成功完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan

