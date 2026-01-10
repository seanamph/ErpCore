# ErpCore 部署指南

## 部署環境需求

### 伺服器需求
- Windows Server 2019+ 或 Linux (Ubuntu 20.04+)
- .NET 7 Runtime
- SQL Server 2019+ 或 SQL Server Express
- IIS (Windows) 或 Nginx (Linux)

### 資料庫需求
- SQL Server 2019+
- 資料庫名稱: ErpCore

## 部署步驟

### 1. 準備環境
```bash
# 安裝 .NET 7 Runtime
# Windows: 下載並安裝 .NET 7 Runtime
# Linux: 
sudo apt-get update
sudo apt-get install -y dotnet-runtime-7.0
```

### 2. 建置專案
```bash
# 使用建置腳本
cd ErpCore
.\scripts\build.ps1 -Configuration Release
```

### 3. 發佈專案
```bash
# 使用部署腳本
.\scripts\deploy.ps1 -Environment Production
```

### 4. 設定資料庫
```bash
# 執行資料庫腳本
.\scripts\database.ps1 -Action Create
```

### 5. 設定應用程式
修改 `appsettings.Production.json` 設定資料庫連線字串和其他設定。

### 6. 部署到 IIS (Windows)
1. 建立 IIS 網站
2. 設定應用程式集區為 .NET CLR 版本 "無受控程式碼"
3. 設定網站實體路徑為發佈目錄

### 7. 部署到 Nginx (Linux)
1. 設定 Nginx 反向代理
2. 設定 systemd 服務

## Docker 部署

### 使用 Docker Compose
```bash
cd ErpCore/docker
docker-compose -f docker-compose.prod.yml up -d
```

## 驗證部署

1. 檢查 API 是否正常運作: `https://your-domain/api/health`
2. 檢查 Swagger 文件: `https://your-domain/swagger`
3. 檢查前端是否正常載入

## 監控與維護

### 日誌位置
- Windows: `logs/` 目錄
- Linux: `/app/logs/` 目錄

### 備份策略
- 資料庫每日備份
- 應用程式設定檔備份
- 日誌檔案保留 7 天

