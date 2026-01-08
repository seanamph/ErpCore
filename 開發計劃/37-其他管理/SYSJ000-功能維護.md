# SYSJ000 - 功能維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSJ000 系列
- **功能名稱**: J系統功能維護
- **功能描述**: 提供J系統功能資料的新增、修改、刪除、查詢功能，用於J系統模組的資料管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSJ000/SYSJ000_PR.ASP` (報表)

### 1.2 業務需求
- 管理J系統功能基本資料
- 支援J系統功能的新增、修改、刪除、查詢
- 支援J系統功能的啟用/停用
- 支援J系統功能的排序
- 支援J系統功能的報表列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SYSJFunctions` (對應舊系統 `SYSJ_*`)

```sql
CREATE TABLE [dbo].[SYSJFunctions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
    [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
    [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE)
    [FunctionValue] NVARCHAR(500) NULL, -- 功能值 (FUNCTION_VALUE)
    [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能設定 (JSON格式) (FUNCTION_CONFIG)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_SYSJFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SYSJFunctions_FunctionId] UNIQUE ([FunctionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_FunctionId] ON [dbo].[SYSJFunctions] ([FunctionId]);
CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_FunctionType] ON [dbo].[SYSJFunctions] ([FunctionType]);
CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_Status] ON [dbo].[SYSJFunctions] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SYSJFunctions_SeqNo] ON [dbo].[SYSJFunctions] ([SeqNo]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| FunctionId | NVARCHAR | 50 | NO | - | 功能代碼 | 唯一鍵 |
| FunctionName | NVARCHAR | 100 | NO | - | 功能名稱 | - |
| FunctionType | NVARCHAR | 20 | YES | - | 功能類型 | - |
| FunctionValue | NVARCHAR | 500 | YES | - | 功能值 | - |
| FunctionConfig | NVARCHAR(MAX) | - | YES | - | 功能設定 | JSON格式 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢J系統功能列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysj-functions`
- **說明**: 查詢J系統功能列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SeqNo",
    "sortOrder": "ASC",
    "filters": {
      "functionId": "",
      "functionName": "",
      "functionType": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆J系統功能
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysj-functions/{tKey}`
- **說明**: 根據主鍵查詢單筆J系統功能資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 標準單筆回應格式

#### 3.1.3 根據功能代碼查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysj-functions/by-id/{functionId}`
- **說明**: 根據功能代碼查詢J系統功能資料
- **路徑參數**:
  - `functionId`: 功能代碼
- **回應格式**: 標準單筆回應格式

#### 3.1.4 新增J系統功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sysj-functions`
- **說明**: 新增J系統功能資料
- **請求格式**:
  ```json
  {
    "functionId": "SYSJ001",
    "functionName": "J系統功能1",
    "functionType": "TYPE1",
    "functionValue": "VALUE1",
    "functionConfig": "{}",
    "seqNo": 1,
    "status": "1",
    "notes": "功能說明"
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.5 修改J系統功能
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/sysj-functions/{tKey}`
- **說明**: 修改J系統功能資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `functionId` 不可修改
- **回應格式**: 標準修改回應格式

#### 3.1.6 刪除J系統功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/sysj-functions/{tKey}`
- **說明**: 刪除J系統功能資料（軟刪除或硬刪除）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 標準刪除回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 J系統功能維護頁面 (`SYSJFunctions.vue`)
- **路徑**: `/system/sysj-functions`
- **功能**: 顯示J系統功能列表，支援新增、修改、刪除、查詢

### 4.2 主要元件

#### 4.2.1 功能列表表格
- 顯示功能代碼、功能名稱、功能類型、狀態等欄位
- 支援排序、篩選
- 支援分頁

#### 4.2.2 功能表單
- 功能代碼（新增時可輸入，修改時唯讀）
- 功能名稱
- 功能類型
- 功能值
- 功能設定（JSON編輯器）
- 排序序號
- 狀態（啟用/停用）
- 備註

---

## 五、開發時程

**總計**: 10天
- 資料庫設計: 1天
- 後端API開發: 3天
- 前端UI開發: 4天
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸
- 功能代碼不可重複

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增J系統功能成功
- [ ] 修改J系統功能成功
- [ ] 刪除J系統功能成功
- [ ] 查詢J系統功能列表成功
- [ ] 查詢單筆J系統功能成功
- [ ] 功能代碼重複時應回傳錯誤

### 7.2 整合測試
- [ ] 完整的新增、修改、刪除、查詢流程
- [ ] 權限驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSJ000/` 目錄下的所有ASP檔案

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

