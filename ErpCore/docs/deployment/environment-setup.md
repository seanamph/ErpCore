# 環境設定文件

## 概述

本文檔描述 ErpCore 系統的環境設定步驟。

## 開發環境設定

### 必要條件
- .NET 7.0 SDK
- Node.js 16.x 或以上
- SQL Server 2019 或以上
- Visual Studio 2022 或 VS Code

### 後端設定步驟

1. **還原 NuGet 套件**
   ```bash
   dotnet restore
   ```

2. **設定資料庫連線**
   - 編輯 `appsettings.json` 或 `appsettings.Development.json`
   - 設定 `ConnectionStrings:DefaultConnection`

3. **執行資料庫遷移**
   ```bash
   dotnet ef database update
   ```
   或使用腳本：
   ```bash
   ./scripts/migrate.ps1
   ```

4. **執行種子資料**
   ```bash
   ./scripts/seed.ps1
   ```

5. **啟動 API**
   ```bash
   dotnet run --project src/ErpCore.Api
   ```

### 前端設定步驟

1. **安裝依賴**
   ```bash
   cd src/ErpCore.Web
   npm install
   ```

2. **設定 API 端點**
   - 編輯 `.env` 或 `.env.development`
   - 設定 `VUE_APP_API_BASE_URL`

3. **啟動開發伺服器**
   ```bash
   npm run serve
   ```

## 生產環境設定

### Docker 部署

1. **建置映像**
   ```bash
   docker-compose build
   ```

2. **啟動服務**
   ```bash
   docker-compose up -d
   ```

### 傳統部署

1. **發佈後端**
   ```bash
   dotnet publish -c Release
   ```

2. **建置前端**
   ```bash
   cd src/ErpCore.Web
   npm run build
   ```

3. **部署到伺服器**
   - 將發佈檔案複製到伺服器
   - 設定 IIS 或 Nginx
   - 設定資料庫連線

