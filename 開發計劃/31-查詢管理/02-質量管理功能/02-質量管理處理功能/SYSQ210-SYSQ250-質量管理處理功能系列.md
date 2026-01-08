# SYSQ210-SYSQ250 - 質量管理處理功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSQ210-SYSQ250 系列
- **功能名稱**: 質量管理處理功能系列（零用金處理作業）
- **功能描述**: 提供零用金維護、請款、拋轉、盤點、審核傳送等處理作業的新增、修改、刪除、查詢功能，包含零用金單號、申請日期、申請人、保管人、金額、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_FMI.ASP` (批量新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ220_FI.ASP` (請款新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ220_FQ.ASP` (請款查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ220_FB.ASP` (請款瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ220_FS.ASP` (請款保存)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ230_FI.ASP` (拋轉新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_FI.ASP` (盤點新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_FQ.ASP` (盤點查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_FB.ASP` (盤點瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_FS.ASP` (盤點保存)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_PR.ASP` (盤點報表)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ242_FQ.ASP` (盤點查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ242_FB.ASP` (盤點瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ242_FS.ASP` (盤點保存)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ242_PR.ASP` (盤點報表)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ250_FQ.ASP` (審核傳送查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ250_FB.ASP` (審核傳送瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ250_FU.ASP` (審核傳送修改)

### 1.2 業務需求
- 管理零用金單基本資料
- 支援零用金申請、請款、拋轉、盤點、審核傳送等流程
- 支援零用金狀態管理（草稿、已申請、已請款、已拋轉、已盤點、已審核等）
- 支援保管人選擇與額度檢查
- 支援多店別管理（SITE_ID）
- 支援批量新增功能
- 支援零用金單查詢與報表
- 支援零用金盤點作業
- 支援傳票審核傳送作業

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PcCash` (零用金主檔)

```sql
CREATE TABLE [dbo].[PcCash] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CashId] NVARCHAR(50) NOT NULL, -- 零用金單號
    [SiteId] NVARCHAR(50) NULL, -- 分店代號
    [AppleDate] DATETIME2 NOT NULL, -- 申請日期
    [AppleName] NVARCHAR(50) NOT NULL, -- 申請人
    [OrgId] NVARCHAR(50) NULL, -- 申請組織代號
    [KeepEmpId] NVARCHAR(50) NULL, -- 保管人代碼
    [CashAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 零用金金額
    [CashStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態 (DRAFT:草稿, APPLIED:已申請, REQUESTED:已請款, TRANSFERRED:已拋轉, INVENTORIED:已盤點, APPROVED:已審核)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    [CPriority] INT NULL, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_PcCash] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PcCash_CashId] UNIQUE ([CashId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PcCash_SiteId] ON [dbo].[PcCash] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_PcCash_AppleDate] ON [dbo].[PcCash] ([AppleDate]);
CREATE NONCLUSTERED INDEX [IX_PcCash_AppleName] ON [dbo].[PcCash] ([AppleName]);
CREATE NONCLUSTERED INDEX [IX_PcCash_KeepEmpId] ON [dbo].[PcCash] ([KeepEmpId]);
CREATE NONCLUSTERED INDEX [IX_PcCash_CashStatus] ON [dbo].[PcCash] ([CashStatus]);
```

### 2.2 主要資料表: `PcCashRequest` (零用金請款檔)

```sql
CREATE TABLE [dbo].[PcCashRequest] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [RequestId] NVARCHAR(50) NOT NULL, -- 請款單號
    [SiteId] NVARCHAR(50) NULL, -- 分店代號
    [RequestDate] DATETIME2 NOT NULL, -- 請款日期
    [CashIds] NVARCHAR(MAX) NULL, -- 零用金單號列表 (JSON格式)
    [RequestAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 請款金額
    [RequestStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    CONSTRAINT [PK_PcCashRequest] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PcCashRequest_RequestId] UNIQUE ([RequestId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PcCashRequest_SiteId] ON [dbo].[PcCashRequest] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_PcCashRequest_RequestDate] ON [dbo].[PcCashRequest] ([RequestDate]);
```

### 2.3 主要資料表: `PcCashTransfer` (零用金拋轉檔)

```sql
CREATE TABLE [dbo].[PcCashTransfer] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransferId] NVARCHAR(50) NOT NULL, -- 拋轉單號
    [SiteId] NVARCHAR(50) NULL, -- 分店代號
    [TransferDate] DATETIME2 NOT NULL, -- 拋轉日期
    [VoucherId] NVARCHAR(50) NULL, -- 傳票編號
    [VoucherKind] NVARCHAR(20) NULL, -- 傳票種類
    [VoucherDate] DATETIME2 NULL, -- 傳票日期
    [TransferAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 拋轉金額
    [TransferStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    CONSTRAINT [PK_PcCashTransfer] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PcCashTransfer_TransferId] UNIQUE ([TransferId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_SiteId] ON [dbo].[PcCashTransfer] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_TransferDate] ON [dbo].[PcCashTransfer] ([TransferDate]);
CREATE NONCLUSTERED INDEX [IX_PcCashTransfer_VoucherId] ON [dbo].[PcCashTransfer] ([VoucherId]);
```

### 2.4 主要資料表: `PcCashInventory` (零用金盤點檔)

```sql
CREATE TABLE [dbo].[PcCashInventory] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InventoryId] NVARCHAR(50) NOT NULL, -- 盤點單號
    [SiteId] NVARCHAR(50) NULL, -- 分店代號
    [InventoryDate] DATETIME2 NOT NULL, -- 盤點日期
    [KeepEmpId] NVARCHAR(50) NOT NULL, -- 保管人代碼
    [InventoryAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 盤點金額
    [ActualAmount] DECIMAL(18,2) NULL, -- 實際金額
    [DifferenceAmount] DECIMAL(18,2) NULL, -- 差異金額
    [InventoryStatus] NVARCHAR(20) NULL DEFAULT 'DRAFT', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    CONSTRAINT [PK_PcCashInventory] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PcCashInventory_InventoryId] UNIQUE ([InventoryId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PcCashInventory_SiteId] ON [dbo].[PcCashInventory] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_PcCashInventory_InventoryDate] ON [dbo].[PcCashInventory] ([InventoryDate]);
CREATE NONCLUSTERED INDEX [IX_PcCashInventory_KeepEmpId] ON [dbo].[PcCashInventory] ([KeepEmpId]);
```

### 2.5 主要資料表: `VoucherAudit` (傳票審核傳送檔)

```sql
CREATE TABLE [dbo].[VoucherAudit] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票編號
    [VoucherKind] NVARCHAR(20) NULL, -- 傳票種類
    [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期
    [AuditStatus] NVARCHAR(20) NULL DEFAULT 'PENDING', -- 審核狀態 (PENDING:待審核, APPROVED:已審核, REJECTED:已拒絕)
    [AuditUser] NVARCHAR(50) NULL, -- 審核者
    [AuditTime] DATETIME2 NULL, -- 審核時間
    [AuditNotes] NVARCHAR(500) NULL, -- 審核備註
    [SendStatus] NVARCHAR(20) NULL DEFAULT 'PENDING', -- 傳送狀態 (PENDING:待傳送, SENT:已傳送)
    [SendTime] DATETIME2 NULL, -- 傳送時間
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    CONSTRAINT [PK_VoucherAudit] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_VoucherAudit_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherAudit_VoucherDate] ON [dbo].[VoucherAudit] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_VoucherAudit_AuditStatus] ON [dbo].[VoucherAudit] ([AuditStatus]);
CREATE NONCLUSTERED INDEX [IX_VoucherAudit_SendStatus] ON [dbo].[VoucherAudit] ([SendStatus]);
```

### 2.6 資料字典

#### PcCash 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| CashId | NVARCHAR | 50 | NO | - | 零用金單號 | 唯一鍵 |
| SiteId | NVARCHAR | 50 | YES | - | 分店代號 | - |
| AppleDate | DATETIME2 | - | NO | - | 申請日期 | - |
| AppleName | NVARCHAR | 50 | NO | - | 申請人 | - |
| OrgId | NVARCHAR | 50 | YES | - | 申請組織代號 | - |
| KeepEmpId | NVARCHAR | 50 | YES | - | 保管人代碼 | - |
| CashAmount | DECIMAL | 18,2 | NO | 0 | 零用金金額 | - |
| CashStatus | NVARCHAR | 20 | YES | 'DRAFT' | 狀態 | DRAFT, APPLIED, REQUESTED, TRANSFERRED, INVENTORIED, APPROVED |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| BUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| BTime | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| CUser | NVARCHAR | 50 | YES | - | 更新者 | - |
| CTime | DATETIME2 | - | YES | - | 更新時間 | - |
| CPriority | INT | - | YES | - | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢零用金列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pc-cash`
- **說明**: 查詢零用金列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（包含siteId、appleDate、appleName、keepEmpId、cashStatus等篩選條件）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆零用金
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pc-cash/{tKey}`
- **說明**: 根據主鍵查詢單筆零用金資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增零用金
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-cash`
- **說明**: 新增零用金資料
- **請求格式**: 標準新增請求格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改零用金
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pc-cash/{tKey}`
- **說明**: 修改零用金資料
- **請求格式**: 標準修改請求格式
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除零用金
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/pc-cash/{tKey}`
- **說明**: 刪除零用金資料（軟刪除或硬刪除）
- **回應格式**: 標準刪除回應格式

#### 3.1.6 批量新增零用金
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-cash/batch`
- **說明**: 批量新增零用金資料
- **請求格式**: 批量新增請求格式
- **回應格式**: 批量新增回應格式

#### 3.1.7 零用金請款作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-cash-request`
- **說明**: 執行零用金請款作業
- **請求格式**: 請款請求格式
- **回應格式**: 請款回應格式

#### 3.1.8 零用金拋轉作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-cash-transfer`
- **說明**: 執行零用金拋轉作業
- **請求格式**: 拋轉請求格式
- **回應格式**: 拋轉回應格式

#### 3.1.9 零用金盤點作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-cash-inventory`
- **說明**: 執行零用金盤點作業
- **請求格式**: 盤點請求格式
- **回應格式**: 盤點回應格式

#### 3.1.10 傳票審核傳送作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-audit`
- **說明**: 執行傳票審核傳送作業
- **請求格式**: 審核傳送請求格式
- **回應格式**: 審核傳送回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 零用金維護頁面 (`PcCash.vue`)
- **路徑**: `/system/pc-cash`
- **功能**: 顯示零用金列表，支援新增、修改、刪除、查詢、批量新增

#### 4.1.2 零用金請款頁面 (`PcCashRequest.vue`)
- **路徑**: `/system/pc-cash-request`
- **功能**: 顯示零用金請款列表，支援請款作業

#### 4.1.3 零用金拋轉頁面 (`PcCashTransfer.vue`)
- **路徑**: `/system/pc-cash-transfer`
- **功能**: 顯示零用金拋轉列表，支援拋轉作業

#### 4.1.4 零用金盤點頁面 (`PcCashInventory.vue`)
- **路徑**: `/system/pc-cash-inventory`
- **功能**: 顯示零用金盤點列表，支援盤點作業、報表

#### 4.1.5 傳票審核傳送頁面 (`VoucherAudit.vue`)
- **路徑**: `/system/voucher-audit`
- **功能**: 顯示傳票審核傳送列表，支援審核、傳送作業

### 4.2 主要元件

#### 4.2.1 零用金列表表格
- 顯示零用金單號、申請日期、申請人、保管人、金額、狀態等欄位
- 支援按分店代號、申請日期、申請人、保管人、狀態篩選
- 支援排序、分頁

#### 4.2.2 零用金表單
- 零用金單號（系統自動編碼）
- 分店代號（下拉選單）
- 申請日期（必填）
- 申請人（必填，可從V_EMP_USER選擇）
- 申請組織代號（可選）
- 保管人代碼（可選，可從V_EMP_USER選擇）
- 零用金金額（必填，數字）
- 備註

---

## 五、開發時程

**總計**: 20天
- 資料庫設計: 2天
- 後端API開發: 8天
- 前端UI開發: 8天
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸
- 零用金單號不可重複

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化

### 6.3 業務邏輯
- 零用金單號需系統自動編碼
- 保管人額度檢查
- 零用金狀態流程控制
- 拋轉作業需寫入應付/應收系統（根據總帳參數APAR_TEMP_TO_APAR）
- 拋轉作業需寫入即時損益檔（根據總帳參數SP_ACCT_VD_YN）
- 拋轉作業需檢查進項發票字軌（根據總帳參數INCOME_TRACK_CHK_YN）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增零用金成功
- [ ] 修改零用金成功
- [ ] 刪除零用金成功
- [ ] 查詢零用金列表成功
- [ ] 批量新增零用金成功
- [ ] 零用金請款作業成功
- [ ] 零用金拋轉作業成功
- [ ] 零用金盤點作業成功
- [ ] 傳票審核傳送作業成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ210_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ220_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ230_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ241_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ242_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ250_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/util/SYST000/util.asp` - 總帳工具函數
- `WEB/IMS_CORE/ASP/util/SYSG000/SYSG000_util.asp` - SYSG工具函數

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

