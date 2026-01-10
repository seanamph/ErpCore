# ErpCore 系統架構文件

## 系統概述
ErpCore 是一個基於 .NET 7 和 Vue 3 的企業資源規劃系統，採用前後端分離架構。

## 架構設計

### 後端架構
- **ErpCore.Api**: Web API 層，提供 RESTful API
- **ErpCore.Application**: 應用層，處理業務邏輯
- **ErpCore.Domain**: 領域層，定義實體模型
- **ErpCore.Infrastructure**: 基礎設施層，處理資料存取和外部服務
- **ErpCore.Shared**: 共用類別庫

### 前端架構
- **Vue 3**: 前端框架
- **Vue Router**: 路由管理
- **Element Plus**: UI 元件庫
- **Axios**: HTTP 客戶端

### 資料庫
- **SQL Server**: 關聯式資料庫
- **Dapper**: ORM 框架

### 日誌系統
- **NLog**: 日誌記錄框架
- 日誌檔案按小時分割，檔名格式：`yyyyMMdd-HH.log`

## 技術棧

### 後端技術
- .NET 7
- ASP.NET Core Web API
- Dapper
- NLog
- SQL Server

### 前端技術
- Vue 3
- Vue CLI
- Element Plus
- Axios

## 開發規範

### 程式碼規範
1. 所有程式碼必須使用 try-catch 進行異常處理
2. 使用 NLog 記錄日誌
3. 資料庫存取使用 Dapper
4. API 命名遵循 RESTful 規範
5. 資料庫欄位命名統一使用 PascalCase

### 顏色主題
- 主色系: #198754
- 所有 UI 元件使用統一的顏色主題

