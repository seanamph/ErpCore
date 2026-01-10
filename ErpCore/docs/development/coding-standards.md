# 編碼標準文件

## 概述

本文檔定義 ErpCore 專案的編碼標準和最佳實踐。

## C# 編碼標準

### 命名規範
- **類別**: PascalCase（例如：`UserService`）
- **方法**: PascalCase（例如：`GetUserById`）
- **屬性**: PascalCase（例如：`UserId`）
- **私有欄位**: camelCase（例如：`_userId`）
- **常數**: PascalCase（例如：`MaxRetryCount`）
- **介面**: 以 `I` 開頭（例如：`IUserService`）

### API 欄位命名
- 所有 API 欄位使用 **PascalCase** 命名
- 與 C# 屬性命名保持一致
- Vue 前端必須遵循 C# API 欄位命名

### 程式碼結構
- 所有程式碼都要有 **try-catch** 異常處理
- 所有資料庫操作都要記錄 **NLog 日誌**
- 日誌檔名以小時為單位：`yyyyMMdd-HH.log`
- 使用 **Dapper** 進行資料庫存取
- 使用參數化查詢，避免 SQL 注入

### 註解規範
- 所有類別和方法都要有簡易說明
- 使用 XML 文件註解（`///`）
- 複雜邏輯要有行內註解

## Vue 編碼標準

### 命名規範
- **元件**: PascalCase（例如：`UserList.vue`）
- **方法**: camelCase（例如：`getUserList`）
- **變數**: camelCase（例如：`userList`）
- **常數**: UPPER_SNAKE_CASE（例如：`API_BASE_URL`）

### API 欄位命名
- Vue 前端必須遵循 C# API 欄位命名（PascalCase）
- 所有 API 呼叫的欄位名稱與後端保持一致

### 樣式規範
- 使用 SCSS 進行樣式編寫
- 主色系：`#198754`
- 確保所有 root、card、badge 的文字和背景顏色對比度
- 使用 Bootstrap 色系為主

## 資料庫編碼標準

### 命名規範
- **資料表**: PascalCase（例如：`Users`）
- **欄位**: PascalCase（例如：`UserId`）
- **索引**: `IX_TableName_ColumnName`
- **外鍵**: `FK_TableName_ReferencedTable`

### Dapper 使用規範
- 使用 Dapper 進行資料庫存取
- 使用 `DynamicParameters` 傳遞參數
- 不支援的功能不使用 Dapper
- 所有 Repository 都要有 try-catch 和 NLog 日誌記錄

## 專案結構規範

### 後端結構
- **Domain**: 實體模型
- **Application**: 業務邏輯（Services、DTOs）
- **Infrastructure**: 資料存取（Repositories）
- **Api**: API 端點（Controllers）

### 前端結構
- **views**: Vue 元件頁面
- **api**: API 呼叫封裝
- **router**: 路由配置
- **store**: 狀態管理
- **components**: 共用元件

## 測試規範

### 單元測試
- 每個 Service 都要有對應的單元測試
- 測試覆蓋率目標：80% 以上

### 整合測試
- 測試 API 端點
- 測試資料庫操作

### 端對端測試
- 測試完整的使用者流程

