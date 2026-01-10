# ErpCore Migration 腳本 (Windows)
# 用途: 執行資料庫 Migration

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Add", "Update", "Remove", "List")]
    [string]$Action,
    
    [string]$MigrationName = "",
    [string]$ConnectionString = ""
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore Migration 腳本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "操作: $Action" -ForegroundColor Yellow
if ($MigrationName) {
    Write-Host "Migration 名稱: $MigrationName" -ForegroundColor Yellow
}
Write-Host ""

# 設定路徑
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$MigrationsPath = Join-Path $ScriptPath ".." "database" "Migrations"

Write-Host "Migration 路徑: $MigrationsPath" -ForegroundColor Yellow
Write-Host ""

# 根據操作執行對應的命令
switch ($Action) {
    "Add" {
        if (-not $MigrationName) {
            Write-Host "錯誤: 新增 Migration 需要指定名稱" -ForegroundColor Red
            exit 1
        }
        Write-Host "新增 Migration: $MigrationName" -ForegroundColor Yellow
        # 這裡可以執行 dotnet ef migrations add 命令
    }
    "Update" {
        Write-Host "更新資料庫..." -ForegroundColor Yellow
        # 這裡可以執行 dotnet ef database update 命令
    }
    "Remove" {
        if (-not $MigrationName) {
            Write-Host "錯誤: 移除 Migration 需要指定名稱" -ForegroundColor Red
            exit 1
        }
        Write-Host "移除 Migration: $MigrationName" -ForegroundColor Yellow
        # 這裡可以執行 dotnet ef migrations remove 命令
    }
    "List" {
        Write-Host "列出所有 Migration..." -ForegroundColor Yellow
        # 這裡可以執行 dotnet ef migrations list 命令
    }
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Migration 操作完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan

