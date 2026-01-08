# ErpCore 專案

## 專案說明
ErpCore 是一個基於 .NET 7 和 Vue 3 的企業資源規劃系統，採用前後端分離架構。

## 專案結構

```
ErpCore/
├── src/
│   ├── ErpCore.Api/              # Web API 專案 (ASP.NET Core Web API)
│   ├── ErpCore.Application/      # 應用層 (業務邏輯)
│   ├── ErpCore.Domain/           # 領域層 (實體模型)
│   ├── ErpCore.Infrastructure/   # 基礎設施層 (資料存取、外部服務)
│   └── ErpCore.Shared/           # 共用類別庫
└── tests/                        # 測試專案目錄
```

## 技術棧

### 後端
- .NET 7
- ASP.NET Core Web API
- Dapper (資料庫存取)
- NLog (日誌記錄)
- SQL Server

### 前端 (待開發)
- Vue 3
- Vue CLI

## 開發規範

### 程式碼規範
1. **異常處理**: 所有程式碼必須使用 try-catch 進行異常處理
2. **日誌記錄**: 使用 NLog 記錄日誌，日誌檔案按小時分割
3. **資料庫存取**: 使用 Dapper，不支援的功能不使用
4. **API 命名**: 統一遵循 RESTful 規範
5. **資料庫命名**: 欄位命名統一使用 PascalCase
6. **程式碼風格**: 保持一致的程式碼風格

### 日誌配置
- 日誌檔案位置: `logs/`
- 檔案命名格式: `yyyyMMdd-HH.log` (按小時分割)
- 自動歸檔: 保留 168 小時 (7天)

## 已實作功能

### 調撥單驗退作業 (SYSW362)
- 查詢待驗退調撥單列表
- 查詢驗退單列表
- 查詢單筆驗退單
- 依調撥單號建立驗退單
- 新增驗退單
- 修改驗退單
- 刪除驗退單
- 確認驗退
- 取消驗退單

## 設定

### 資料庫連線
在 `appsettings.json` 中設定資料庫連線字串:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ErpCore;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

## 執行

### 後端 API
```bash
cd src/ErpCore.Api
dotnet run
```

API 文件: `https://localhost:5001/swagger`

## 注意事項

1. 所有程式碼均位於 ErpCore 目錄下
2. 後端僅提供 Web API，不包含 MVC Views
3. 前端將使用 Vue CLI 開發
4. 資料庫 Schema 需手動建立或使用 Migration

## 開發進度

詳細進度請參考 `work.txt` 檔案。

