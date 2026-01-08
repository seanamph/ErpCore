# SYSN120-SYSN123 - 會計憑證管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN120-SYSN123 系列
- **功能名稱**: 會計憑證管理系列
- **功能描述**: 提供會計憑證資料的新增、修改、刪除、查詢功能，包含傳票型態設定、常用傳票資料維護、傳票處理等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FS.ASP` (排序)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN121_*.ASP` (傳票型態設定相關)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN122_*.ASP` (常用傳票資料相關)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN123_*.ASP` (傳票處理相關)

### 1.2 業務需求
- 管理傳票型態設定（VOUCHER_TYPE）
- 管理常用傳票資料（常用傳票主檔及明細）
- 支援傳票處理功能（傳票新增、修改、刪除、查詢）
- 支援傳票批量處理
- 支援傳票審批流程
- 支援傳票列印功能
- 支援傳票匯出功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `VoucherTypes` (對應舊系統 `VOUCHER_TYPE`)

```sql
CREATE TABLE [dbo].[VoucherTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTypeId] NVARCHAR(50) NOT NULL, -- 傳票型態代號 (VOUCHER_TYPE_ID)
    [VoucherTypeName] NVARCHAR(200) NOT NULL, -- 傳票型態名稱 (VOUCHER_TYPE_NAME)
    [VoucherTypeNameE] NVARCHAR(200) NULL, -- 傳票型態英文名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Description] NVARCHAR(500) NULL, -- 說明 (DESCRIPTION)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_VoucherTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_VoucherTypes_VoucherTypeId] UNIQUE ([VoucherTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherTypes_VoucherTypeId] ON [dbo].[VoucherTypes] ([VoucherTypeId]);
CREATE NONCLUSTERED INDEX [IX_VoucherTypes_Status] ON [dbo].[VoucherTypes] ([Status]);
```

### 2.2 主要資料表: `CommonVouchers` (常用傳票主檔)

```sql
CREATE TABLE [dbo].[CommonVouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CommonVoucherId] NVARCHAR(50) NOT NULL, -- 常用傳票代號 (COMMON_VOUCHER_ID)
    [CommonVoucherName] NVARCHAR(200) NOT NULL, -- 常用傳票名稱 (COMMON_VOUCHER_NAME)
    [VoucherTypeId] NVARCHAR(50) NULL, -- 傳票型態代號 (VOUCHER_TYPE_ID)
    [Description] NVARCHAR(500) NULL, -- 說明 (DESCRIPTION)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [SortOrder] INT NULL, -- 排序 (SORT_ORDER)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_CommonVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CommonVouchers_CommonVoucherId] UNIQUE ([CommonVoucherId]),
    CONSTRAINT [FK_CommonVouchers_VoucherTypes] FOREIGN KEY ([VoucherTypeId]) REFERENCES [dbo].[VoucherTypes] ([VoucherTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_CommonVoucherId] ON [dbo].[CommonVouchers] ([CommonVoucherId]);
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_VoucherTypeId] ON [dbo].[CommonVouchers] ([VoucherTypeId]);
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_Status] ON [dbo].[CommonVouchers] ([Status]);
CREATE NONCLUSTERED INDEX [IX_CommonVouchers_SortOrder] ON [dbo].[CommonVouchers] ([SortOrder]);
```

### 2.3 主要資料表: `CommonVoucherDetails` (常用傳票明細)

```sql
CREATE TABLE [dbo].[CommonVoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CommonVoucherId] NVARCHAR(50) NOT NULL, -- 常用傳票代號 (COMMON_VOUCHER_ID)
    [SeqNo] INT NOT NULL, -- 序號 (SEQ_NO)
    [StypeId] NVARCHAR(50) NULL, -- 會計科目代號 (STYPE_ID)
    [Dc] NVARCHAR(1) NULL, -- 借/貸 (DC, D:借方, C:貸方)
    [Amount] DECIMAL(18, 2) NULL DEFAULT 0, -- 金額 (AMOUNT)
    [Description] NVARCHAR(500) NULL, -- 摘要 (DESCRIPTION)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_CommonVoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_CommonVoucherDetails_CommonVouchers] FOREIGN KEY ([CommonVoucherId]) REFERENCES [dbo].[CommonVouchers] ([CommonVoucherId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CommonVoucherDetails_AccountSubjects] FOREIGN KEY ([StypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_CommonVoucherId] ON [dbo].[CommonVoucherDetails] ([CommonVoucherId]);
CREATE NONCLUSTERED INDEX [IX_CommonVoucherDetails_StypeId] ON [dbo].[CommonVoucherDetails] ([StypeId]);
```

### 2.4 主要資料表: `Vouchers` (傳票主檔)

```sql
CREATE TABLE [dbo].[Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票編號 (VOUCHER_ID)
    [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期 (VOUCHER_DATE)
    [VoucherTypeId] NVARCHAR(50) NULL, -- 傳票型態代號 (VOUCHER_TYPE_ID)
    [Description] NVARCHAR(500) NULL, -- 摘要 (DESCRIPTION)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, P:已過帳, C:已取消)
    [PostedBy] NVARCHAR(50) NULL, -- 過帳者 (POSTED_BY)
    [PostedAt] DATETIME2 NULL, -- 過帳時間 (POSTED_AT)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId]),
    CONSTRAINT [FK_Vouchers_VoucherTypes] FOREIGN KEY ([VoucherTypeId]) REFERENCES [dbo].[VoucherTypes] ([VoucherTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherId] ON [dbo].[Vouchers] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherTypeId] ON [dbo].[Vouchers] ([VoucherTypeId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_Status] ON [dbo].[Vouchers] ([Status]);
```

### 2.5 主要資料表: `VoucherDetails` (傳票明細)

```sql
CREATE TABLE [dbo].[VoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票編號 (VOUCHER_ID)
    [SeqNo] INT NOT NULL, -- 序號 (SEQ_NO)
    [StypeId] NVARCHAR(50) NULL, -- 會計科目代號 (STYPE_ID)
    [Dc] NVARCHAR(1) NOT NULL, -- 借/貸 (DC, D:借方, C:貸方)
    [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額 (AMOUNT)
    [Description] NVARCHAR(500) NULL, -- 摘要 (DESCRIPTION)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_VoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherId]) REFERENCES [dbo].[Vouchers] ([VoucherId]) ON DELETE CASCADE,
    CONSTRAINT [FK_VoucherDetails_AccountSubjects] FOREIGN KEY ([StypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherId] ON [dbo].[VoucherDetails] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_StypeId] ON [dbo].[VoucherDetails] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_Dc] ON [dbo].[VoucherDetails] ([Dc]);
```

### 2.6 資料字典

#### 2.6.1 VoucherTypes 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTypeId | NVARCHAR | 50 | NO | - | 傳票型態代號 | 唯一，主鍵候選 |
| VoucherTypeName | NVARCHAR | 200 | NO | - | 傳票型態名稱 | - |
| VoucherTypeNameE | NVARCHAR | 200 | YES | - | 傳票型態英文名稱 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Description | NVARCHAR | 500 | YES | - | 說明 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### 2.6.2 CommonVouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| CommonVoucherId | NVARCHAR | 50 | NO | - | 常用傳票代號 | 唯一，主鍵候選 |
| CommonVoucherName | NVARCHAR | 200 | NO | - | 常用傳票名稱 | - |
| VoucherTypeId | NVARCHAR | 50 | YES | - | 傳票型態代號 | 外鍵至VoucherTypes |
| Description | NVARCHAR | 500 | YES | - | 說明 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SortOrder | INT | - | YES | - | 排序 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### 2.6.3 Vouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 傳票編號 | 唯一，主鍵候選 |
| VoucherDate | DATETIME2 | - | NO | - | 傳票日期 | - |
| VoucherTypeId | NVARCHAR | 50 | YES | - | 傳票型態代號 | 外鍵至VoucherTypes |
| Description | NVARCHAR | 500 | YES | - | 摘要 | - |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, P:已過帳, C:已取消 |
| PostedBy | NVARCHAR | 50 | YES | - | 過帳者 | - |
| PostedAt | DATETIME2 | - | YES | - | 過帳時間 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢傳票型態列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-types`
- **說明**: 查詢傳票型態列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherTypeId",
    "sortOrder": "ASC",
    "filters": {
      "voucherTypeId": "",
      "voucherTypeName": "",
      "status": ""
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "voucherTypeId": "VT001",
          "voucherTypeName": "一般傳票",
          "voucherTypeNameE": "General Voucher",
          "status": "A",
          "description": "一般用途傳票"
        }
      ],
      "totalCount": 10,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆傳票型態
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/voucher-types/{voucherTypeId}`
- **說明**: 根據傳票型態代號查詢單筆傳票型態資料
- **路徑參數**:
  - `voucherTypeId`: 傳票型態代號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增傳票型態
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/voucher-types`
- **說明**: 新增傳票型態資料
- **請求格式**:
  ```json
  {
    "voucherTypeId": "VT001",
    "voucherTypeName": "一般傳票",
    "voucherTypeNameE": "General Voucher",
    "status": "A",
    "description": "一般用途傳票"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "voucherTypeId": "VT001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改傳票型態
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/voucher-types/{voucherTypeId}`
- **說明**: 修改傳票型態資料
- **路徑參數**:
  - `voucherTypeId`: 傳票型態代號
- **請求格式**: 同新增，但 `voucherTypeId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除傳票型態
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/voucher-types/{voucherTypeId}`
- **說明**: 刪除傳票型態資料（需檢查是否有使用中的傳票）
- **路徑參數**:
  - `voucherTypeId`: 傳票型態代號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 查詢常用傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/common-vouchers`
- **說明**: 查詢常用傳票列表，支援分頁、排序、篩選
- **請求參數**: 類似傳票型態查詢
- **回應格式**: 類似傳票型態查詢

#### 3.1.7 查詢常用傳票明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/common-vouchers/{commonVoucherId}/details`
- **說明**: 查詢常用傳票明細
- **路徑參數**:
  - `commonVoucherId`: 常用傳票代號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "seqNo": 1,
        "stypeId": "1000",
        "stypeName": "現金",
        "dc": "D",
        "amount": 1000,
        "description": "摘要"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 新增常用傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/common-vouchers`
- **說明**: 新增常用傳票（包含主檔及明細）
- **請求格式**:
  ```json
  {
    "commonVoucherId": "CV001",
    "commonVoucherName": "常用傳票1",
    "voucherTypeId": "VT001",
    "status": "A",
    "description": "說明",
    "details": [
      {
        "seqNo": 1,
        "stypeId": "1000",
        "dc": "D",
        "amount": 1000,
        "description": "摘要"
      }
    ]
  }
  ```

#### 3.1.9 修改常用傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/common-vouchers/{commonVoucherId}`
- **說明**: 修改常用傳票（包含主檔及明細）
- **請求格式**: 同新增

#### 3.1.10 刪除常用傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/common-vouchers/{commonVoucherId}`
- **說明**: 刪除常用傳票（會級聯刪除明細）

#### 3.1.11 查詢傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers`
- **說明**: 查詢傳票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherDate",
    "sortOrder": "DESC",
    "filters": {
      "voucherId": "",
      "voucherDateFrom": "",
      "voucherDateTo": "",
      "voucherTypeId": "",
      "status": ""
    }
  }
  ```

#### 3.1.12 查詢傳票明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/{voucherId}/details`
- **說明**: 查詢傳票明細
- **路徑參數**:
  - `voucherId`: 傳票編號

#### 3.1.13 新增傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers`
- **說明**: 新增傳票（包含主檔及明細），需檢查借貸平衡
- **請求格式**:
  ```json
  {
    "voucherId": "V20240101001",
    "voucherDate": "2024-01-01",
    "voucherTypeId": "VT001",
    "description": "摘要",
    "details": [
      {
        "seqNo": 1,
        "stypeId": "1000",
        "dc": "D",
        "amount": 1000,
        "description": "摘要"
      },
      {
        "seqNo": 2,
        "stypeId": "2000",
        "dc": "C",
        "amount": 1000,
        "description": "摘要"
      }
    ]
  }
  ```

#### 3.1.14 修改傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 修改傳票（僅限草稿狀態），需檢查借貸平衡
- **請求格式**: 同新增

#### 3.1.15 刪除傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 刪除傳票（僅限草稿狀態）

#### 3.1.16 過帳傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/{voucherId}/post`
- **說明**: 將傳票狀態改為已過帳，需檢查借貸平衡
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "過帳成功",
    "data": {
      "voucherId": "V20240101001",
      "postedAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.17 取消傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/{voucherId}/cancel`
- **說明**: 取消傳票（僅限已過帳狀態）

#### 3.1.18 檢查借貸平衡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/check-balance`
- **說明**: 檢查傳票明細的借貸是否平衡
- **請求格式**:
  ```json
  {
    "details": [
      {
        "dc": "D",
        "amount": 1000
      },
      {
        "dc": "C",
        "amount": 1000
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檢查完成",
    "data": {
      "isBalanced": true,
      "debitTotal": 1000,
      "creditTotal": 1000,
      "difference": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `VoucherTypesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/voucher-types")]
    [Authorize]
    public class VoucherTypesController : ControllerBase
    {
        private readonly IVoucherTypeService _voucherTypeService;
        
        public VoucherTypesController(IVoucherTypeService voucherTypeService)
        {
            _voucherTypeService = voucherTypeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherTypeDto>>>> GetVoucherTypes([FromQuery] VoucherTypeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{voucherTypeId}")]
        public async Task<ActionResult<ApiResponse<VoucherTypeDto>>> GetVoucherType(string voucherTypeId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucherType([FromBody] CreateVoucherTypeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{voucherTypeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucherType(string voucherTypeId, [FromBody] UpdateVoucherTypeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{voucherTypeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucherType(string voucherTypeId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Controller: `CommonVouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/common-vouchers")]
    [Authorize]
    public class CommonVouchersController : ControllerBase
    {
        private readonly ICommonVoucherService _commonVoucherService;
        
        public CommonVouchersController(ICommonVoucherService commonVoucherService)
        {
            _commonVoucherService = commonVoucherService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CommonVoucherDto>>>> GetCommonVouchers([FromQuery] CommonVoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{commonVoucherId}/details")]
        public async Task<ActionResult<ApiResponse<List<CommonVoucherDetailDto>>>> GetCommonVoucherDetails(string commonVoucherId)
        {
            // 實作查詢明細邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateCommonVoucher([FromBody] CreateCommonVoucherDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{commonVoucherId}")]
        public async Task<ActionResult<ApiResponse>> UpdateCommonVoucher(string commonVoucherId, [FromBody] UpdateCommonVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{commonVoucherId}")]
        public async Task<ActionResult<ApiResponse>> DeleteCommonVoucher(string commonVoucherId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.3 Controller: `VouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vouchers")]
    [Authorize]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        
        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherDto>>>> GetVouchers([FromQuery] VoucherQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{voucherId}")]
        public async Task<ActionResult<ApiResponse<VoucherDto>>> GetVoucher(string voucherId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("{voucherId}/details")]
        public async Task<ActionResult<ApiResponse<List<VoucherDetailDto>>>> GetVoucherDetails(string voucherId)
        {
            // 實作查詢明細邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            // 實作新增邏輯，需檢查借貸平衡
        }
        
        [HttpPut("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucher(string voucherId, [FromBody] UpdateVoucherDto dto)
        {
            // 實作修改邏輯，僅限草稿狀態，需檢查借貸平衡
        }
        
        [HttpDelete("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucher(string voucherId)
        {
            // 實作刪除邏輯，僅限草稿狀態
        }
        
        [HttpPost("{voucherId}/post")]
        public async Task<ActionResult<ApiResponse>> PostVoucher(string voucherId)
        {
            // 實作過帳邏輯，需檢查借貸平衡
        }
        
        [HttpPost("{voucherId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelVoucher(string voucherId)
        {
            // 實作取消邏輯，僅限已過帳狀態
        }
        
        [HttpPost("check-balance")]
        public async Task<ActionResult<ApiResponse<BalanceCheckDto>>> CheckBalance([FromBody] BalanceCheckRequestDto dto)
        {
            // 實作借貸平衡檢查邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票型態列表頁面 (`VoucherTypeList.vue`)
- **路徑**: `/accounting/voucher-types`
- **功能**: 顯示傳票型態列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (VoucherTypeSearchForm)
  - 資料表格 (VoucherTypeDataTable)
  - 新增/修改對話框 (VoucherTypeDialog)
  - 刪除確認對話框

#### 4.1.2 常用傳票列表頁面 (`CommonVoucherList.vue`)
- **路徑**: `/accounting/common-vouchers`
- **功能**: 顯示常用傳票列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (CommonVoucherSearchForm)
  - 資料表格 (CommonVoucherDataTable)
  - 新增/修改對話框 (CommonVoucherDialog)
  - 明細編輯元件 (CommonVoucherDetailEditor)

#### 4.1.3 傳票列表頁面 (`VoucherList.vue`)
- **路徑**: `/accounting/vouchers`
- **功能**: 顯示傳票列表，支援查詢、新增、修改、刪除、過帳、取消
- **主要元件**:
  - 查詢表單 (VoucherSearchForm)
  - 資料表格 (VoucherDataTable)
  - 新增/修改對話框 (VoucherDialog)
  - 明細編輯元件 (VoucherDetailEditor)
  - 借貸平衡顯示元件 (BalanceDisplay)

### 4.2 UI 元件設計

#### 4.2.1 傳票型態查詢表單元件 (`VoucherTypeSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="傳票型態代號">
      <el-input v-model="searchForm.voucherTypeId" placeholder="請輸入傳票型態代號" />
    </el-form-item>
    <el-form-item label="傳票型態名稱">
      <el-input v-model="searchForm.voucherTypeName" placeholder="請輸入傳票型態名稱" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 傳票明細編輯元件 (`VoucherDetailEditor.vue`)
```vue
<template>
  <div>
    <el-table :data="details" border>
      <el-table-column type="index" label="序號" width="60" />
      <el-table-column label="會計科目" width="200">
        <template #default="{ row, $index }">
          <el-select v-model="row.stypeId" placeholder="請選擇會計科目" filterable>
            <el-option
              v-for="subject in accountSubjects"
              :key="subject.stypeId"
              :label="`${subject.stypeId} - ${subject.stypeName}`"
              :value="subject.stypeId"
            />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column label="借/貸" width="100">
        <template #default="{ row }">
          <el-select v-model="row.dc" placeholder="請選擇">
            <el-option label="借方" value="D" />
            <el-option label="貸方" value="C" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column label="金額" width="150">
        <template #default="{ row }">
          <el-input-number v-model="row.amount" :precision="2" :min="0" style="width: 100%" />
        </template>
      </el-table-column>
      <el-table-column label="摘要" min-width="200">
        <template #default="{ row }">
          <el-input v-model="row.description" placeholder="請輸入摘要" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ $index }">
          <el-button type="danger" size="small" @click="handleDelete($index)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top: 10px">
      <el-button type="primary" @click="handleAdd">新增明細</el-button>
      <el-button @click="handleCheckBalance">檢查借貸平衡</el-button>
    </div>
    <div v-if="balanceInfo" style="margin-top: 10px">
      <el-alert
        :type="balanceInfo.isBalanced ? 'success' : 'error'"
        :title="balanceInfo.isBalanced ? '借貸平衡' : '借貸不平衡'"
        :description="`借方總額: ${balanceInfo.debitTotal}, 貸方總額: ${balanceInfo.creditTotal}, 差額: ${balanceInfo.difference}`"
        show-icon
      />
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`voucher.api.ts`)
```typescript
import request from '@/utils/request';

// 傳票型態相關
export interface VoucherTypeDto {
  voucherTypeId: string;
  voucherTypeName: string;
  voucherTypeNameE?: string;
  status: string;
  description?: string;
}

export const getVoucherTypeList = (query: VoucherTypeQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherTypeDto>>>('/api/v1/voucher-types', { params: query });
};

export const getVoucherTypeById = (voucherTypeId: string) => {
  return request.get<ApiResponse<VoucherTypeDto>>(`/api/v1/voucher-types/${voucherTypeId}`);
};

export const createVoucherType = (data: CreateVoucherTypeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/voucher-types', data);
};

export const updateVoucherType = (voucherTypeId: string, data: UpdateVoucherTypeDto) => {
  return request.put<ApiResponse>(`/api/v1/voucher-types/${voucherTypeId}`, data);
};

export const deleteVoucherType = (voucherTypeId: string) => {
  return request.delete<ApiResponse>(`/api/v1/voucher-types/${voucherTypeId}`);
};

// 常用傳票相關
export interface CommonVoucherDto {
  commonVoucherId: string;
  commonVoucherName: string;
  voucherTypeId?: string;
  status: string;
  description?: string;
  sortOrder?: number;
}

export const getCommonVoucherList = (query: CommonVoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<CommonVoucherDto>>>('/api/v1/common-vouchers', { params: query });
};

export const getCommonVoucherDetails = (commonVoucherId: string) => {
  return request.get<ApiResponse<CommonVoucherDetailDto[]>>(`/api/v1/common-vouchers/${commonVoucherId}/details`);
};

export const createCommonVoucher = (data: CreateCommonVoucherDto) => {
  return request.post<ApiResponse<string>>('/api/v1/common-vouchers', data);
};

export const updateCommonVoucher = (commonVoucherId: string, data: UpdateCommonVoucherDto) => {
  return request.put<ApiResponse>(`/api/v1/common-vouchers/${commonVoucherId}`, data);
};

export const deleteCommonVoucher = (commonVoucherId: string) => {
  return request.delete<ApiResponse>(`/api/v1/common-vouchers/${commonVoucherId}`);
};

// 傳票相關
export interface VoucherDto {
  voucherId: string;
  voucherDate: string;
  voucherTypeId?: string;
  description?: string;
  status: string;
  postedBy?: string;
  postedAt?: string;
}

export interface VoucherDetailDto {
  seqNo: number;
  stypeId: string;
  stypeName?: string;
  dc: string;
  amount: number;
  description?: string;
}

export const getVoucherList = (query: VoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers', { params: query });
};

export const getVoucherById = (voucherId: string) => {
  return request.get<ApiResponse<VoucherDto>>(`/api/v1/vouchers/${voucherId}`);
};

export const getVoucherDetails = (voucherId: string) => {
  return request.get<ApiResponse<VoucherDetailDto[]>>(`/api/v1/vouchers/${voucherId}/details`);
};

export const createVoucher = (data: CreateVoucherDto) => {
  return request.post<ApiResponse<string>>('/api/v1/vouchers', data);
};

export const updateVoucher = (voucherId: string, data: UpdateVoucherDto) => {
  return request.put<ApiResponse>(`/api/v1/vouchers/${voucherId}`, data);
};

export const deleteVoucher = (voucherId: string) => {
  return request.delete<ApiResponse>(`/api/v1/vouchers/${voucherId}`);
};

export const postVoucher = (voucherId: string) => {
  return request.post<ApiResponse>(`/api/v1/vouchers/${voucherId}/post`);
};

export const cancelVoucher = (voucherId: string) => {
  return request.post<ApiResponse>(`/api/v1/vouchers/${voucherId}/cancel`);
};

export const checkBalance = (data: BalanceCheckRequestDto) => {
  return request.post<ApiResponse<BalanceCheckDto>>('/api/v1/vouchers/check-balance', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立傳票型態資料表結構
- [ ] 建立常用傳票主檔及明細資料表結構
- [ ] 建立傳票主檔及明細資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（借貸平衡檢查）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 傳票型態列表頁面開發
- [ ] 常用傳票列表頁面開發
- [ ] 傳票列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 明細編輯元件開發
- [ ] 借貸平衡檢查元件開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 借貸平衡檢查測試
- [ ] 過帳流程測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 安全性
- 傳票過帳必須有權限檢查
- 已過帳傳票不可修改或刪除
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制（傳票型態、常用傳票）

### 6.3 資料驗證
- 傳票型態代號必須唯一
- 常用傳票代號必須唯一
- 傳票編號必須唯一
- 傳票明細必須借貸平衡
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 新增/修改傳票時必須檢查借貸平衡
- 刪除傳票型態前必須檢查是否有使用中的傳票
- 刪除常用傳票前必須檢查是否有使用中的傳票
- 過帳傳票前必須檢查借貸平衡
- 已過帳傳票不可修改或刪除
- 取消傳票僅限已過帳狀態
- 傳票日期不可晚於系統日期

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增傳票型態成功
- [ ] 新增傳票型態失敗 (重複代號)
- [ ] 修改傳票型態成功
- [ ] 修改傳票型態失敗 (不存在)
- [ ] 刪除傳票型態成功
- [ ] 刪除傳票型態失敗 (有使用中的傳票)
- [ ] 新增常用傳票成功
- [ ] 新增常用傳票失敗 (重複代號)
- [ ] 新增傳票成功
- [ ] 新增傳票失敗 (借貸不平衡)
- [ ] 修改傳票成功
- [ ] 修改傳票失敗 (已過帳)
- [ ] 刪除傳票成功
- [ ] 刪除傳票失敗 (已過帳)
- [ ] 過帳傳票成功
- [ ] 過帳傳票失敗 (借貸不平衡)
- [ ] 取消傳票成功
- [ ] 取消傳票失敗 (草稿狀態)
- [ ] 查詢傳票列表成功
- [ ] 查詢單筆傳票成功
- [ ] 檢查借貸平衡成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 借貸平衡檢查測試
- [ ] 過帳流程測試
- [ ] 取消流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 借貸平衡檢查效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FI.ASP` - 新增畫面
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FD.ASP` - 刪除畫面
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSN000/SYSN120_FS.ASP` - 排序畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_TYPE`（傳票型態設定）
- 舊系統資料表：常用傳票主檔及明細表
- 舊系統資料表：傳票主檔及明細表
- 主要欄位：VOUCHER_ID, VOUCHER_DATE, VOUCHER_TYPE_ID, STATUS, T_KEY等

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

