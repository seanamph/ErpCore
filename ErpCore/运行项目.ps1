# ErpCore 專案快速運行腳本
# 用途: 快速檢查環境並運行專案

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "ErpCore 專案運行腳本" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 檢查 .NET SDK
Write-Host "檢查 .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ .NET SDK 已安裝: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Host "✗ .NET SDK 未安裝或不在 PATH 中" -ForegroundColor Red
        Write-Host "  請從 https://dotnet.microsoft.com/download/dotnet/7.0 下載並安裝 .NET 7 SDK" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host "✗ .NET SDK 未安裝或不在 PATH 中" -ForegroundColor Red
    Write-Host "  請從 https://dotnet.microsoft.com/download/dotnet/7.0 下載並安裝 .NET 7 SDK" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# 檢查 Node.js
Write-Host "檢查 Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Node.js 已安裝: $nodeVersion" -ForegroundColor Green
    } else {
        Write-Host "✗ Node.js 未安裝或不在 PATH 中" -ForegroundColor Red
        Write-Host "  請從 https://nodejs.org/ 下載並安裝 Node.js" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Node.js 未安裝或不在 PATH 中" -ForegroundColor Red
    Write-Host "  請從 https://nodejs.org/ 下載並安裝 Node.js" -ForegroundColor Yellow
}

Write-Host ""

# 檢查專案結構
Write-Host "檢查專案結構..." -ForegroundColor Yellow
$apiProject = "src\ErpCore.Api\ErpCore.Api.csproj"
$webProject = "src\ErpCore.Web\package.json"

if (Test-Path $apiProject) {
    Write-Host "✓ 後端專案存在" -ForegroundColor Green
} else {
    Write-Host "✗ 後端專案不存在: $apiProject" -ForegroundColor Red
    exit 1
}

if (Test-Path $webProject) {
    Write-Host "✓ 前端專案存在" -ForegroundColor Green
} else {
    Write-Host "✗ 前端專案不存在: $webProject" -ForegroundColor Red
}

Write-Host ""

# 還原後端套件
Write-Host "還原後端 NuGet 套件..." -ForegroundColor Yellow
dotnet restore ErpCore.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ 還原失敗!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ 還原完成" -ForegroundColor Green
Write-Host ""

# 建置後端專案
Write-Host "建置後端專案..." -ForegroundColor Yellow
dotnet build ErpCore.sln --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ 建置失敗!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ 建置完成" -ForegroundColor Green
Write-Host ""

# 檢查資料庫連線設定
Write-Host "檢查資料庫連線設定..." -ForegroundColor Yellow
$appsettingsPath = "src\ErpCore.Api\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
    if ($appsettings.ConnectionStrings.DefaultConnection) {
        Write-Host "✓ 資料庫連線字串已設定" -ForegroundColor Green
        Write-Host "  連線字串: $($appsettings.ConnectionStrings.DefaultConnection)" -ForegroundColor Gray
    } else {
        Write-Host "⚠ 資料庫連線字串未設定，請編輯 $appsettingsPath" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠ appsettings.json 不存在" -ForegroundColor Yellow
}

Write-Host ""

# 選擇運行模式
Write-Host "請選擇運行模式:" -ForegroundColor Cyan
Write-Host "1. 僅運行後端 API" -ForegroundColor White
Write-Host "2. 僅運行前端 (需要先安裝 npm 套件)" -ForegroundColor White
Write-Host "3. 同時運行後端和前端" -ForegroundColor White
Write-Host "4. 僅檢查環境和建置" -ForegroundColor White
Write-Host ""

$choice = Read-Host "請輸入選項 (1-4)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "啟動後端 API..." -ForegroundColor Yellow
        Write-Host "API 將在以下網址運行:" -ForegroundColor Cyan
        Write-Host "  - HTTPS: https://localhost:5001" -ForegroundColor White
        Write-Host "  - HTTP: http://localhost:5000" -ForegroundColor White
        Write-Host "  - Swagger: https://localhost:5001/swagger" -ForegroundColor White
        Write-Host ""
        Write-Host "按 Ctrl+C 停止服務" -ForegroundColor Yellow
        Write-Host ""
        cd src\ErpCore.Api
        dotnet run
    }
    "2" {
        Write-Host ""
        Write-Host "檢查前端套件..." -ForegroundColor Yellow
        if (-not (Test-Path "src\ErpCore.Web\node_modules")) {
            Write-Host "⚠ node_modules 不存在，正在安裝套件..." -ForegroundColor Yellow
            cd src\ErpCore.Web
            npm install
            if ($LASTEXITCODE -ne 0) {
                Write-Host "✗ 安裝失敗!" -ForegroundColor Red
                exit 1
            }
            cd ..\..
        }
        Write-Host ""
        Write-Host "啟動前端開發伺服器..." -ForegroundColor Yellow
        Write-Host "前端將在 http://localhost:8080 運行" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "按 Ctrl+C 停止服務" -ForegroundColor Yellow
        Write-Host ""
        cd src\ErpCore.Web
        npm run serve
    }
    "3" {
        Write-Host ""
        Write-Host "同時運行後端和前端..." -ForegroundColor Yellow
        Write-Host "這將開啟兩個終端視窗" -ForegroundColor Cyan
        Write-Host ""
        
        # 啟動後端
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\src\ErpCore.Api'; Write-Host '後端 API 運行中...' -ForegroundColor Green; dotnet run"
        
        Start-Sleep -Seconds 3
        
        # 檢查並安裝前端套件
        if (-not (Test-Path "src\ErpCore.Web\node_modules")) {
            Write-Host "安裝前端套件..." -ForegroundColor Yellow
            cd src\ErpCore.Web
            npm install
            cd ..\..
        }
        
        # 啟動前端
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\src\ErpCore.Web'; Write-Host '前端開發伺服器運行中...' -ForegroundColor Green; npm run serve"
        
        Write-Host "✓ 後端和前端已啟動" -ForegroundColor Green
        Write-Host "  後端 API: https://localhost:5001/swagger" -ForegroundColor Cyan
        Write-Host "  前端應用: http://localhost:8080" -ForegroundColor Cyan
    }
    "4" {
        Write-Host ""
        Write-Host "✓ 環境檢查和建置完成" -ForegroundColor Green
        Write-Host ""
        Write-Host "下一步:" -ForegroundColor Cyan
        Write-Host "1. 設定資料庫連線字串 (src\ErpCore.Api\appsettings.json)" -ForegroundColor White
        Write-Host "2. 執行資料庫腳本 (database\Scripts\)" -ForegroundColor White
        Write-Host "3. 運行此腳本並選擇選項 1 或 3 來啟動專案" -ForegroundColor White
    }
    default {
        Write-Host "無效的選項" -ForegroundColor Red
        exit 1
    }
}

