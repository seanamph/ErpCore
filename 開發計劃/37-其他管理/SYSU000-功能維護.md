# SYSU000 - 功能維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSU000 系列
- **功能名稱**: U系統功能維護（支付和銀行管理、財務和報表功能）
- **功能描述**: 提供U系統功能資料的新增、修改、刪除、查詢功能，包含支付管理（SYSU20系列）、銀行和支付管理（SYSU210-270系列）、財務管理（SYSU310-360系列）、報表和查詢（SYSU410-4A0系列）、其他功能模組（SYSU510-630系列）等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSU000/SYSU20_*.ASP` (支付管理)
  - `WEB/IMS_CORE/ASP/SYSU000/SYSU210_*.ASP` (銀行和支付管理)
  - `WEB/IMS_CORE/ASP/SYSU000/SYSU310_*.ASP` (財務管理)
  - `WEB/IMS_CORE/ASP/SYSU000/SYSU410_*.ASP` (報表和查詢)
  - `WEB/IMS_CORE/ASP/SYSU000/SYSU510_*.ASP` (其他功能)
  - `WEB/IMS_CORE/ASP/INCLUDE/SYSU000/INIT.inc` (初始化)

### 1.2 業務需求
- 管理支付和銀行基本資料
- 支援財務資料的新增、修改、刪除、查詢
- 支援報表和查詢功能
- 支援支付管理功能
- 支援銀行管理功能
- 支援財務管理功能
- 支援其他功能模組

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SYSUFunctions` (對應舊系統 `SYSU_*`)

```sql
CREATE TABLE [dbo].[SYSUFunctions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼 (FUNCTION_ID)
    [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱 (FUNCTION_NAME)
    [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (FUNCTION_TYPE, PAYMENT:支付, BANK:銀行, FINANCE:財務, REPORT:報表, OTHER:其他)
    [FunctionCategory] NVARCHAR(50) NULL, -- 功能分類 (FUNCTION_CATEGORY)
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
    CONSTRAINT [PK_SYSUFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SYSUFunctions_FunctionId] UNIQUE ([FunctionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionId] ON [dbo].[SYSUFunctions] ([FunctionId]);
CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionType] ON [dbo].[SYSUFunctions] ([FunctionType]);
CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_FunctionCategory] ON [dbo].[SYSUFunctions] ([FunctionCategory]);
CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_Status] ON [dbo].[SYSUFunctions] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SYSUFunctions_SeqNo] ON [dbo].[SYSUFunctions] ([SeqNo]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| FunctionId | NVARCHAR | 50 | NO | - | 功能代碼 | 唯一鍵 |
| FunctionName | NVARCHAR | 100 | NO | - | 功能名稱 | - |
| FunctionType | NVARCHAR | 20 | YES | - | 功能類型 | PAYMENT:支付, BANK:銀行, FINANCE:財務, REPORT:報表, OTHER:其他 |
| FunctionCategory | NVARCHAR | 50 | YES | - | 功能分類 | - |
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

#### 3.1.1 查詢U系統功能列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysu-functions`
- **說明**: 查詢U系統功能列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（包含functionType、functionCategory等篩選條件）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆U系統功能
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sysu-functions/{tKey}`
- **說明**: 根據主鍵查詢單筆U系統功能資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增U系統功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sysu-functions`
- **說明**: 新增U系統功能資料
- **請求格式**: 標準新增請求格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改U系統功能
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/sysu-functions/{tKey}`
- **說明**: 修改U系統功能資料
- **請求格式**: 標準修改請求格式
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除U系統功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/sysu-functions/{tKey}`
- **說明**: 刪除U系統功能資料
- **回應格式**: 標準刪除回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 U系統功能維護頁面 (`SYSUFunctions.vue`)
- **路徑**: `/system/sysu-functions`
- **功能**: 顯示U系統功能列表，支援新增、修改、刪除、查詢
- **主要功能分類**:
  - 支付管理 (SYSU20系列)
  - 銀行和支付管理 (SYSU210-270系列)
  - 財務管理 (SYSU310-360系列)
  - 報表和查詢 (SYSU410-4A0系列)
  - 其他功能模組 (SYSU510-630系列)

### 4.2 主要元件

#### 4.2.1 功能列表表格
- 顯示功能代碼、功能名稱、功能類型、功能分類、狀態等欄位
- 支援按功能類型、功能分類篩選
- 支援排序、分頁

#### 4.2.2 功能表單
- 功能代碼、功能名稱、功能類型、功能分類、功能值、功能設定、排序序號、狀態、備註

---

## 五、開發時程

**總計**: 12天
- 資料庫設計: 1天
- 後端API開發: 4天
- 前端UI開發: 5天
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料（支付、銀行資訊）必須加密傳輸
- 功能代碼不可重複

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化
- 財務資料查詢需考慮效能

### 6.3 業務邏輯
- 支付和銀行管理功能需特別注意資料完整性
- 財務資料修改需記錄審計日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增U系統功能成功
- [ ] 修改U系統功能成功
- [ ] 刪除U系統功能成功
- [ ] 查詢U系統功能列表成功
- [ ] 按功能類型篩選成功
- [ ] 按功能分類篩選成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSU000/` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/INCLUDE/SYSU000/INIT.inc`

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

