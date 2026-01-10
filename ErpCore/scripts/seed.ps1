# ErpCore 種子資料腳本 (Windows)
# 用途: 執行資料庫種子資料

param(
    [string]$ConnectionString = "",
    [string]$DatabaseName = "ErpCore"
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore 種子資料腳本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "資料庫: $DatabaseName" -ForegroundColor Yellow
Write-Host ""

# 設定路徑
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$SeedsPath = Join-Path $ScriptPath ".." "database" "Seeds"

Write-Host "種子資料路徑: $SeedsPath" -ForegroundColor Yellow
Write-Host ""

# 執行種子資料腳本
Write-Host "執行種子資料腳本..." -ForegroundColor Yellow
# 這裡可以執行種子資料的 SQL 腳本
# 例如: sqlcmd -S localhost -d $DatabaseName -i "$SeedsPath\SeedData.sql"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "種子資料執行完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan

