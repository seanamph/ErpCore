# SYSQ110-SYSQ120 - 質量管理基礎功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSQ110-SYSQ120 系列
- **功能名稱**: 質量管理基礎功能系列（零用金基礎設定）
- **功能描述**: 提供零用金參數維護與保管人及額度設定作業的新增、修改、刪除、查詢功能，包含零用金參數設定、保管人資料維護、零用金額度設定等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_PR.ASP` (報表)

### 1.2 業務需求
- 管理零用金參數設定（銀行存款、進項稅額等）
- 管理保管人基本資料
- 管理零用金額度設定
- 支援多店別管理（SITE_ID）
- 支援保管人代碼長度參數化
- 支援保管人資料來源參數化（CASH_KEEPER_SOURCE）
- 支援保管人模式參數化（CASH_KEEP_NAME_MODE）
- 支援零用金參數查詢與維護
- 支援保管人及額度資料維護（新增、修改、刪除、查詢）
- 支援保管人及額度報表列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CashParams` (零用金參數)

```sql
CREATE TABLE [dbo].[CashParams] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UnitId] NVARCHAR(50) NULL, -- 公司單位代號
    [ApexpLid] NVARCHAR(50) NOT NULL, -- 銀行存款會計科目代號
    [PtaxLid] NVARCHAR(50) NOT NULL, -- 進項稅額會計科目代號
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_CashParams] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CashParams_UnitId] ON [dbo].[CashParams] ([UnitId]);
```

### 2.2 主要資料表: `PcKeep` (保管人及額度設定)

```sql
CREATE TABLE [dbo].[PcKeep] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SiteId] NVARCHAR(50) NULL, -- 分店代號
    [KeepEmpId] NVARCHAR(50) NOT NULL, -- 保管人代碼
    [PcQuota] DECIMAL(18,2) NULL DEFAULT 0, -- 零用金額度
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    [CPriority] INT NULL, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_PcKeep] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PcKeep_KeepEmpId] UNIQUE ([KeepEmpId], [SiteId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PcKeep_SiteId] ON [dbo].[PcKeep] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_PcKeep_KeepEmpId] ON [dbo].[PcKeep] ([KeepEmpId]);
```

### 2.3 資料字典

#### CashParams 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UnitId | NVARCHAR | 50 | YES | - | 公司單位代號 | - |
| ApexpLid | NVARCHAR | 50 | NO | - | 銀行存款會計科目代號 | - |
| PtaxLid | NVARCHAR | 50 | NO | - | 進項稅額會計科目代號 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### PcKeep 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| SiteId | NVARCHAR | 50 | YES | - | 分店代號 | - |
| KeepEmpId | NVARCHAR | 50 | NO | - | 保管人代碼 | 唯一鍵（與SiteId組合） |
| PcQuota | DECIMAL | 18,2 | YES | 0 | 零用金額度 | - |
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

#### 3.1.1 查詢零用金參數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-params`
- **說明**: 查詢零用金參數資料
- **請求參數**: 標準查詢參數
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆零用金參數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-params/{tKey}`
- **說明**: 根據主鍵查詢單筆零用金參數資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增零用金參數
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cash-params`
- **說明**: 新增零用金參數資料
- **請求格式**: 標準新增請求格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改零用金參數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cash-params/{tKey}`
- **說明**: 修改零用金參數資料
- **請求格式**: 標準修改請求格式
- **回應格式**: 標準修改回應格式

#### 3.1.5 查詢保管人及額度列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pc-keep`
- **說明**: 查詢保管人及額度設定列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（包含siteId、keepEmpId等篩選條件）
- **回應格式**: 標準列表回應格式

#### 3.1.6 查詢單筆保管人及額度
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pc-keep/{tKey}`
- **說明**: 根據主鍵查詢單筆保管人及額度資料
- **回應格式**: 標準單筆回應格式

#### 3.1.7 新增保管人及額度
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-keep`
- **說明**: 新增保管人及額度資料
- **請求格式**:
  ```json
  {
    "siteId": "SITE001",
    "keepEmpId": "EMP001",
    "pcQuota": 10000.00,
    "notes": "備註說明"
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.8 修改保管人及額度
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pc-keep/{tKey}`
- **說明**: 修改保管人及額度資料
- **請求格式**: 同新增，但 `keepEmpId` 不可修改
- **回應格式**: 標準修改回應格式

#### 3.1.9 刪除保管人及額度
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/pc-keep/{tKey}`
- **說明**: 刪除保管人及額度資料（軟刪除或硬刪除）
- **回應格式**: 標準刪除回應格式

#### 3.1.10 保管人及額度報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pc-keep/report`
- **說明**: 產生保管人及額度報表
- **請求格式**: 報表查詢參數
- **回應格式**: 報表資料格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 零用金參數維護頁面 (`CashParams.vue`)
- **路徑**: `/system/cash-params`
- **功能**: 顯示零用金參數資料，支援查詢、修改

#### 4.1.2 保管人及額度設定頁面 (`PcKeep.vue`)
- **路徑**: `/system/pc-keep`
- **功能**: 顯示保管人及額度列表，支援新增、修改、刪除、查詢、報表

### 4.2 主要元件

#### 4.2.1 零用金參數表單
- 公司單位代號（可選）
- 銀行存款會計科目代號（必填）
- 進項稅額會計科目代號（必填）

#### 4.2.2 保管人及額度列表表格
- 顯示分店代號、保管人代碼、保管人名稱、零用金額度、備註等欄位
- 支援按分店代號、保管人代碼篩選
- 支援排序、分頁

#### 4.2.3 保管人及額度表單
- 分店代號（下拉選單）
- 保管人代碼（必填，可從V_EMP_USER選擇）
- 零用金額度（必填，數字）
- 備註

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
- 敏感資料必須加密傳輸
- 保管人代碼不可重複（同一分店）

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化

### 6.3 業務邏輯
- 保管人代碼長度需從系統參數讀取（SYSTEM_DEFAULT, "1", "SYSH000"）
- 保管人資料來源需從總帳參數讀取（CASH_KEEPER_SOURCE）
- 保管人模式需從總帳參數讀取（CASH_KEEP_NAME_MODE）
- 新增保管人時需檢查是否已存在（同一分店、同一保管人）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增零用金參數成功
- [ ] 修改零用金參數成功
- [ ] 查詢零用金參數成功
- [ ] 新增保管人及額度成功
- [ ] 修改保管人及額度成功
- [ ] 刪除保管人及額度成功
- [ ] 查詢保管人及額度列表成功
- [ ] 查詢單筆保管人及額度成功
- [ ] 保管人代碼重複檢查
- [ ] 保管人及額度報表產生成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ110_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ120_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/util/SYSH000/HR_UTIL.ASP` - 人資工具函數

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

