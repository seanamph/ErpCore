# SYSTA00-SYSTA70 - 暫存傳票審核作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSTA00-SYSTA70 系列
- **功能名稱**: 暫存傳票審核作業系列
- **功能描述**: 提供暫存傳票的審核、維護、傳送、查詢功能，包含多個前端系統的傳票審核作業，如資材、禮券、專櫃營運、租戶、人資、委外、自營等系統的傳票審核
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA01_FQ.ASP` (暫存傳票審核作業-前端系統列表)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA02_FQ.ASP` (暫存傳票審核作業-前端系統列表，各系統暫存傳票使用共同的TABLE和共同的拋轉程式SYSTA03)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FQ.ASP` (傳票審核傳送作業-查詢畫面)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FU.ASP` (傳票審核傳送作業-傳票明細畫面)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FUP.ASP` (傳票審核傳送作業-拋轉處理)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FQ.ASP` (資材傳票審核)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FU.ASP` (資材傳票審核維護)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FUP.ASP` (資材傳票審核拋轉)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA30_FQ.ASP` (禮券傳票審核)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA30_FU.ASP` (禮券傳票審核維護)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FQ.ASP` (專櫃傳票審核)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FU.ASP` (專櫃傳票審核維護)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FUP.ASP` (專櫃傳票審核拋轉)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FQ.ASP` (專櫃傳票審核維護/傳送作業-查詢)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FU.ASP` (專櫃傳票審核維護/傳送作業-維護)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FUP.ASP` (專櫃傳票審核維護/傳送作業-拋轉)
  - `WEB/IMS_CORE/ASP/SYST000/SYSTA70_PR.ASP` (專櫃傳票審核維護/傳送作業-報表)

### 1.2 業務需求
- 管理各前端系統產生的暫存傳票
- 支援傳票審核作業（查詢、瀏覽、修改、刪除）
- 支援傳票拋轉作業（將暫存傳票轉入正式傳票）
- 支援多系統傳票管理（資材、禮券、專櫃營運、租戶、人資、委外、自營等）
- 支援傳票狀態追蹤（未審核、已審核、已拋轉）
- 支援傳票日期驗證（需大於關帳年月才可拋轉）
- 支援傳票明細維護（科目、部門、專案、摘要、借貸方金額等）
- 支援應收付票據管理（三松住金）
- 支援住宅金AHM款別代號
- 支援日立暫存傳票轉入
- 支援融資拋轉功能
- 支援電子商務傳票審核

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TmpVoucherM` (暫存傳票主檔)

```sql
CREATE TABLE [dbo].[TmpVoucherM] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NULL, -- 傳票編號 (VOUCHER_ID)
    [VoucherDate] DATETIME2 NULL, -- 傳票日期 (VOUCHER_DATE)
    [TypeId] NVARCHAR(50) NULL, -- 傳票類型 (TYPE_ID, 如SYSTA10, SYSTA30, SYSTA40等)
    [SysId] NVARCHAR(50) NULL, -- 系統代號 (SYS_ID, 如SYSA000, SYSV000, SYSH000等)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 傳票狀態 (STATUS, 1:未審核, 2:已審核, 3:已拋轉)
    [UpFlag] NVARCHAR(1) NULL DEFAULT '0', -- 拋轉標記 (UP_FLAG, 0:未拋轉, 1:已拋轉)
    [Notes] NVARCHAR(500) NULL, -- 傳票備註 (NOTES)
    [VendorId] NVARCHAR(50) NULL, -- 廠商代號 (VENDOR_ID)
    [StoreId] NVARCHAR(50) NULL, -- 專櫃代號 (STORE_ID)
    [SiteId] NVARCHAR(50) NULL, -- 公司別/分店代號 (SITE_ID)
    [SlipType] NVARCHAR(50) NULL, -- 單據別 (SLIP_TYPE)
    [SlipNo] NVARCHAR(50) NULL, -- 單據編號 (SLIP_NO)
    [SendFlag] NVARCHAR(1) NULL DEFAULT '0', -- 傳送標記 (SEND_FLAG)
    [ProgId] NVARCHAR(50) NULL, -- 程式代號 (PROG_ID)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_TmpVoucherM] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_TypeId] ON [dbo].[TmpVoucherM] ([TypeId]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_SysId] ON [dbo].[TmpVoucherM] ([SysId]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_Status] ON [dbo].[TmpVoucherM] ([Status]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_UpFlag] ON [dbo].[TmpVoucherM] ([UpFlag]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_VoucherDate] ON [dbo].[TmpVoucherM] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherM_VoucherId] ON [dbo].[TmpVoucherM] ([VoucherId]);
```

### 2.2 相關資料表

#### 2.2.1 `TmpVoucherD` - 暫存傳票明細檔
```sql
CREATE TABLE [dbo].[TmpVoucherD] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔TKey (VOUCHER_T_KEY)
    [Sn] NVARCHAR(10) NOT NULL, -- 序號 (SN)
    [Dc] NVARCHAR(1) NULL, -- 借貸方 (DC, 0:借方, 1:貸方)
    [SubN] NVARCHAR(50) NULL, -- 科目代號 (SUB_N)
    [OrgId] NVARCHAR(50) NULL, -- 部門代號 (ORG_ID)
    [ActId] NVARCHAR(50) NULL, -- 專案代號 (ACT_ID)
    [Notes] NVARCHAR(500) NULL, -- 摘要 (NOTES)
    [Val0] DECIMAL(18, 2) NULL DEFAULT 0, -- 借方金額 (VAL0)
    [Val1] DECIMAL(18, 2) NULL DEFAULT 0, -- 貸方金額 (VAL1)
    [SupN] NVARCHAR(50) NULL, -- 立沖對象 (SUP_N)
    [InN] NVARCHAR(50) NULL, -- 立沖憑證 (IN_N)
    [VendorId] NVARCHAR(50) NULL, -- 廠商代號 (VENDOR_ID)
    [AbatId] NVARCHAR(50) NULL, -- 立沖代號 (ABAT_ID)
    [ObjectId] NVARCHAR(50) NULL, -- 關係人代號 (OBJECT_ID)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_TmpVoucherD] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_TmpVoucherD_TmpVoucherM] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[TmpVoucherM] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_VoucherTKey] ON [dbo].[TmpVoucherD] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_SubN] ON [dbo].[TmpVoucherD] ([SubN]);
CREATE NONCLUSTERED INDEX [IX_TmpVoucherD_OrgId] ON [dbo].[TmpVoucherD] ([OrgId]);
```

#### 2.2.2 `VoucherType` - 傳票類型對應表
```sql
CREATE TABLE [dbo].[VoucherType] (
    [VoucherId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [ProgId] NVARCHAR(50) NULL, -- 程式代號 (PROG_ID)
    [TypeName] NVARCHAR(200) NULL, -- 類型名稱
    [SysId] NVARCHAR(50) NULL, -- 系統代號
    [Status] NVARCHAR(1) NULL DEFAULT '1', -- 狀態
    CONSTRAINT [PK_VoucherType] PRIMARY KEY CLUSTERED ([VoucherId] ASC)
);
```

### 2.3 資料字典

#### TmpVoucherM 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | YES | - | 傳票編號 | - |
| VoucherDate | DATETIME2 | - | YES | - | 傳票日期 | - |
| TypeId | NVARCHAR | 50 | YES | - | 傳票類型 | SYSTA10, SYSTA30, SYSTA40等 |
| SysId | NVARCHAR | 50 | YES | - | 系統代號 | SYSA000, SYSV000, SYSH000等 |
| Status | NVARCHAR | 10 | NO | '1' | 傳票狀態 | 1:未審核, 2:已審核, 3:已拋轉 |
| UpFlag | NVARCHAR | 1 | YES | '0' | 拋轉標記 | 0:未拋轉, 1:已拋轉 |
| Notes | NVARCHAR | 500 | YES | - | 傳票備註 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠商代號 | - |
| StoreId | NVARCHAR | 50 | YES | - | 專櫃代號 | - |
| SiteId | NVARCHAR | 50 | YES | - | 公司別/分店代號 | - |
| SlipType | NVARCHAR | 50 | YES | - | 單據別 | - |
| SlipNo | NVARCHAR | 50 | YES | - | 單據編號 | - |
| SendFlag | NVARCHAR | 1 | YES | '0' | 傳送標記 | - |
| ProgId | NVARCHAR | 50 | YES | - | 程式代號 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### TmpVoucherD 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTKey | BIGINT | - | NO | - | 傳票主檔TKey | 外鍵至TmpVoucherM |
| Sn | NVARCHAR | 10 | NO | - | 序號 | - |
| Dc | NVARCHAR | 1 | YES | - | 借貸方 | 0:借方, 1:貸方 |
| SubN | NVARCHAR | 50 | YES | - | 科目代號 | 外鍵至AccountSubjects |
| OrgId | NVARCHAR | 50 | YES | - | 部門代號 | - |
| ActId | NVARCHAR | 50 | YES | - | 專案代號 | - |
| Notes | NVARCHAR | 500 | YES | - | 摘要 | - |
| Val0 | DECIMAL | 18,2 | YES | 0 | 借方金額 | - |
| Val1 | DECIMAL | 18,2 | YES | 0 | 貸方金額 | - |
| SupN | NVARCHAR | 50 | YES | - | 立沖對象 | - |
| InN | NVARCHAR | 50 | YES | - | 立沖憑證 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠商代號 | - |
| AbatId | NVARCHAR | 50 | YES | - | 立沖代號 | - |
| ObjectId | NVARCHAR | 50 | YES | - | 關係人代號 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢前端系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tmp-vouchers/system-list`
- **說明**: 查詢各前端系統的暫存傳票未審核筆數
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "sysId": "SYSA000",
        "sysName": "資材管理系統",
        "progId": "SYSTA10",
        "unreviewedCount": 5
      },
      {
        "sysId": "SYSV000",
        "sysName": "禮券管理系統",
        "progId": "SYSTA30",
        "unreviewedCount": 3
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢暫存傳票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tmp-vouchers`
- **說明**: 查詢暫存傳票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VoucherDate",
    "sortOrder": "DESC",
    "filters": {
      "typeId": "",
      "sysId": "",
      "status": "1",
      "voucherDateFrom": "",
      "voucherDateTo": "",
      "slipType": "",
      "vendorId": "",
      "storeId": "",
      "siteId": ""
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
          "tKey": 1,
          "voucherId": "V001",
          "voucherDate": "2024-01-01",
          "typeId": "SYSTA10",
          "sysId": "SYSA000",
          "status": "1",
          "upFlag": "0",
          "notes": "備註",
          "vendorId": "V001",
          "storeId": "ST001",
          "siteId": "SITE001",
          "slipType": "PO",
          "slipNo": "PO001",
          "unreviewedCount": 0
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢單筆暫存傳票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tmp-vouchers/{tKey}`
- **說明**: 根據TKey查詢單筆暫存傳票資料（含明細）
- **路徑參數**:
  - `tKey`: 傳票主檔TKey
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "voucherId": "V001",
      "voucherDate": "2024-01-01",
      "typeId": "SYSTA10",
      "sysId": "SYSA000",
      "status": "1",
      "upFlag": "0",
      "notes": "備註",
      "vendorId": "V001",
      "storeId": "ST001",
      "siteId": "SITE001",
      "slipType": "PO",
      "slipNo": "PO001",
      "details": [
        {
          "tKey": 1,
          "sn": "001",
          "dc": "0",
          "subN": "1100",
          "orgId": "DEPT001",
          "actId": "ACT001",
          "notes": "摘要",
          "val0": 1000.00,
          "val1": 0.00,
          "vendorId": "V001",
          "abatId": null,
          "objectId": null
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改暫存傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/tmp-vouchers/{tKey}`
- **說明**: 修改暫存傳票主檔和明細資料
- **路徑參數**:
  - `tKey`: 傳票主檔TKey
- **請求格式**:
  ```json
  {
    "notes": "修改後的備註",
    "details": [
      {
        "sn": "001",
        "dc": "0",
        "subN": "1100",
        "orgId": "DEPT001",
        "actId": "ACT001",
        "notes": "修改後的摘要",
        "val0": 1000.00,
        "val1": 0.00,
        "vendorId": "V001",
        "abatId": null,
        "objectId": null
      }
    ]
  }
  ```
- **回應格式**: 同查詢單筆

#### 3.1.5 刪除暫存傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/tmp-vouchers/{tKey}`
- **說明**: 刪除暫存傳票（含明細）
- **路徑參數**:
  - `tKey`: 傳票主檔TKey
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

#### 3.1.6 拋轉暫存傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tmp-vouchers/{tKey}/transfer`
- **說明**: 將暫存傳票拋轉至正式傳票
- **路徑參數**:
  - `tKey`: 傳票主檔TKey
- **請求格式**:
  ```json
  {
    "voucherDate": "2024-01-01",
    "validateCloseDate": true
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "拋轉成功",
    "data": {
      "tKey": 1,
      "voucherId": "V001",
      "transferDate": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 批次拋轉暫存傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/tmp-vouchers/batch-transfer`
- **說明**: 批次拋轉多筆暫存傳票
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3],
    "validateCloseDate": true
  }
  ```

#### 3.1.8 查詢未審核筆數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/tmp-vouchers/unreviewed-count`
- **說明**: 查詢各系統的未審核傳票筆數
- **請求參數**:
  - `typeId`: 傳票類型（可選）
  - `sysId`: 系統代號（可選）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "totalCount": 10,
      "bySystem": [
        {
          "sysId": "SYSA000",
          "count": 5
        },
        {
          "sysId": "SYSV000",
          "count": 3
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `TmpVouchersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tmp-vouchers")]
    [Authorize]
    public class TmpVouchersController : ControllerBase
    {
        private readonly ITmpVoucherService _tmpVoucherService;
        
        public TmpVouchersController(ITmpVoucherService tmpVoucherService)
        {
            _tmpVoucherService = tmpVoucherService;
        }
        
        [HttpGet("system-list")]
        public async Task<ActionResult<ApiResponse<List<SystemVoucherCountDto>>>> GetSystemList()
        {
            // 實作查詢前端系統列表邏輯
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TmpVoucherDto>>>> GetTmpVouchers([FromQuery] TmpVoucherQueryDto query)
        {
            // 實作查詢列表邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<TmpVoucherDetailDto>>> GetTmpVoucher(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse<TmpVoucherDetailDto>>> UpdateTmpVoucher(long tKey, [FromBody] UpdateTmpVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteTmpVoucher(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{tKey}/transfer")]
        public async Task<ActionResult<ApiResponse<TransferVoucherResultDto>>> TransferTmpVoucher(long tKey, [FromBody] TransferVoucherDto dto)
        {
            // 實作拋轉邏輯
        }
        
        [HttpPost("batch-transfer")]
        public async Task<ActionResult<ApiResponse<BatchTransferResultDto>>> BatchTransferTmpVouchers([FromBody] BatchTransferVoucherDto dto)
        {
            // 實作批次拋轉邏輯
        }
        
        [HttpGet("unreviewed-count")]
        public async Task<ActionResult<ApiResponse<UnreviewedCountDto>>> GetUnreviewedCount([FromQuery] string typeId = null, [FromQuery] string sysId = null)
        {
            // 實作查詢未審核筆數邏輯
        }
    }
}
```

#### 3.2.2 Service: `TmpVoucherService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITmpVoucherService
    {
        Task<List<SystemVoucherCountDto>> GetSystemListAsync();
        Task<PagedResult<TmpVoucherDto>> GetTmpVouchersAsync(TmpVoucherQueryDto query);
        Task<TmpVoucherDetailDto> GetTmpVoucherByIdAsync(long tKey);
        Task<TmpVoucherDetailDto> UpdateTmpVoucherAsync(long tKey, UpdateTmpVoucherDto dto);
        Task DeleteTmpVoucherAsync(long tKey);
        Task<TransferVoucherResultDto> TransferTmpVoucherAsync(long tKey, TransferVoucherDto dto);
        Task<BatchTransferResultDto> BatchTransferTmpVouchersAsync(BatchTransferVoucherDto dto);
        Task<UnreviewedCountDto> GetUnreviewedCountAsync(string typeId = null, string sysId = null);
    }
}
```

#### 3.2.3 Repository: `TmpVoucherRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ITmpVoucherRepository
    {
        Task<TmpVoucherM> GetByIdAsync(long tKey);
        Task<PagedResult<TmpVoucherM>> GetPagedAsync(TmpVoucherQuery query);
        Task<TmpVoucherM> UpdateAsync(TmpVoucherM voucher);
        Task DeleteAsync(long tKey);
        Task<int> GetUnreviewedCountAsync(string typeId = null, string sysId = null);
        Task<List<SystemVoucherCountDto>> GetSystemVoucherCountsAsync();
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 前端系統列表頁面 (`TmpVoucherSystemList.vue`)
- **路徑**: `/accounting/tmp-vouchers/systems`
- **功能**: 顯示各前端系統的暫存傳票未審核筆數，點擊可進入該系統的傳票審核頁面
- **主要元件**:
  - 系統列表卡片 (SystemCard)
  - 未審核筆數標籤

#### 4.1.2 暫存傳票查詢頁面 (`TmpVoucherQuery.vue`)
- **路徑**: `/accounting/tmp-vouchers/query`
- **功能**: 查詢暫存傳票列表，支援多條件查詢
- **主要元件**:
  - 查詢表單 (TmpVoucherSearchForm)
  - 資料表格 (TmpVoucherDataTable)
  - 拋轉按鈕

#### 4.1.3 暫存傳票維護頁面 (`TmpVoucherMaintain.vue`)
- **路徑**: `/accounting/tmp-vouchers/{tKey}/maintain`
- **功能**: 顯示暫存傳票詳細資料，支援修改、刪除、拋轉
- **主要元件**:
  - 傳票主檔表單 (VoucherHeaderForm)
  - 傳票明細表格 (VoucherDetailTable)
  - 操作按鈕組

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`TmpVoucherSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="傳票類型">
      <el-select v-model="searchForm.typeId" placeholder="請選擇傳票類型" clearable>
        <el-option v-for="type in typeList" :key="type.typeId" :label="type.typeName" :value="type.typeId" />
      </el-select>
    </el-form-item>
    <el-form-item label="系統代號">
      <el-select v-model="searchForm.sysId" placeholder="請選擇系統代號" clearable>
        <el-option v-for="sys in sysList" :key="sys.sysId" :label="sys.sysName" :value="sys.sysId" />
      </el-select>
    </el-form-item>
    <el-form-item label="傳票狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態" clearable>
        <el-option label="未審核" value="1" />
        <el-option label="已審核" value="2" />
        <el-option label="已拋轉" value="3" />
      </el-select>
    </el-form-item>
    <el-form-item label="傳票日期">
      <el-date-picker
        v-model="searchForm.voucherDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item label="單據別">
      <el-input v-model="searchForm.slipType" placeholder="請輸入單據別" />
    </el-form-item>
    <el-form-item label="廠商代號">
      <el-input v-model="searchForm.vendorId" placeholder="請輸入廠商代號" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`TmpVoucherDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="voucherList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="voucherId" label="傳票編號" width="120" />
      <el-table-column prop="voucherDate" label="傳票日期" width="120" />
      <el-table-column prop="typeName" label="傳票類型" width="120" />
      <el-table-column prop="sysName" label="系統代號" width="120" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="slipType" label="單據別" width="100" />
      <el-table-column prop="slipNo" label="單據編號" width="120" />
      <el-table-column prop="notes" label="備註" min-width="200" show-overflow-tooltip />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button type="warning" size="small" @click="handleEdit(row)" v-if="row.status === '1'">修改</el-button>
          <el-button type="success" size="small" @click="handleTransfer(row)" v-if="row.status === '1'">拋轉</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.status === '1'">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top: 10px">
      <el-button type="success" @click="handleBatchTransfer" :disabled="selectedRows.length === 0">批次拋轉</el-button>
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
      />
    </div>
  </div>
</template>
```

#### 4.2.3 傳票明細維護元件 (`VoucherDetailTable.vue`)
```vue
<template>
  <div>
    <el-table :data="detailList" border>
      <el-table-column prop="sn" label="序號" width="80" />
      <el-table-column prop="dc" label="借貸方" width="100">
        <template #default="{ row }">
          <el-select v-model="row.dc" size="small">
            <el-option label="借方" value="0" />
            <el-option label="貸方" value="1" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="subN" label="科目代號" width="120">
        <template #default="{ row }">
          <el-input v-model="row.subN" size="small" @blur="handleSubNChange(row)" />
        </template>
      </el-table-column>
      <el-table-column prop="subNName" label="科目名稱" width="200" />
      <el-table-column prop="orgId" label="部門代號" width="120">
        <template #default="{ row }">
          <el-input v-model="row.orgId" size="small" />
        </template>
      </el-table-column>
      <el-table-column prop="actId" label="專案代號" width="120">
        <template #default="{ row }">
          <el-input v-model="row.actId" size="small" />
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="摘要" min-width="200">
        <template #default="{ row }">
          <el-input v-model="row.notes" size="small" type="textarea" :rows="1" />
        </template>
      </el-table-column>
      <el-table-column prop="val0" label="借方金額" width="120">
        <template #default="{ row }">
          <el-input-number v-model="row.val0" size="small" :precision="2" :min="0" />
        </template>
      </el-table-column>
      <el-table-column prop="val1" label="貸方金額" width="120">
        <template #default="{ row }">
          <el-input-number v-model="row.val1" size="small" :precision="2" :min="0" />
        </template>
      </el-table-column>
      <el-table-column prop="vendorId" label="廠商代號" width="120">
        <template #default="{ row }">
          <el-input v-model="row.vendorId" size="small" />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ row, $index }">
          <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
  </div>
</template>
```

### 4.3 API 呼叫 (`tmp-voucher.api.ts`)
```typescript
import request from '@/utils/request';

export interface TmpVoucherDto {
  tKey: number;
  voucherId?: string;
  voucherDate?: string;
  typeId?: string;
  sysId?: string;
  status: string;
  upFlag?: string;
  notes?: string;
  vendorId?: string;
  storeId?: string;
  siteId?: string;
  slipType?: string;
  slipNo?: string;
}

export interface TmpVoucherDetailDto extends TmpVoucherDto {
  details: TmpVoucherDetailItemDto[];
}

export interface TmpVoucherDetailItemDto {
  tKey?: number;
  sn: string;
  dc?: string;
  subN?: string;
  orgId?: string;
  actId?: string;
  notes?: string;
  val0?: number;
  val1?: number;
  vendorId?: string;
  abatId?: string;
  objectId?: string;
}

export interface TmpVoucherQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    typeId?: string;
    sysId?: string;
    status?: string;
    voucherDateFrom?: string;
    voucherDateTo?: string;
    slipType?: string;
    vendorId?: string;
    storeId?: string;
    siteId?: string;
  };
}

export interface UpdateTmpVoucherDto {
  notes?: string;
  details: TmpVoucherDetailItemDto[];
}

export interface TransferVoucherDto {
  voucherDate?: string;
  validateCloseDate?: boolean;
}

// API 函數
export const getSystemList = () => {
  return request.get<ApiResponse<SystemVoucherCountDto[]>>('/api/v1/tmp-vouchers/system-list');
};

export const getTmpVoucherList = (query: TmpVoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<TmpVoucherDto>>>('/api/v1/tmp-vouchers', { params: query });
};

export const getTmpVoucherById = (tKey: number) => {
  return request.get<ApiResponse<TmpVoucherDetailDto>>(`/api/v1/tmp-vouchers/${tKey}`);
};

export const updateTmpVoucher = (tKey: number, data: UpdateTmpVoucherDto) => {
  return request.put<ApiResponse<TmpVoucherDetailDto>>(`/api/v1/tmp-vouchers/${tKey}`, data);
};

export const deleteTmpVoucher = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/tmp-vouchers/${tKey}`);
};

export const transferTmpVoucher = (tKey: number, data: TransferVoucherDto) => {
  return request.post<ApiResponse<TransferVoucherResultDto>>(`/api/v1/tmp-vouchers/${tKey}/transfer`, data);
};

export const batchTransferTmpVouchers = (data: BatchTransferVoucherDto) => {
  return request.post<ApiResponse<BatchTransferResultDto>>('/api/v1/tmp-vouchers/batch-transfer', data);
};

export const getUnreviewedCount = (typeId?: string, sysId?: string) => {
  return request.get<ApiResponse<UnreviewedCountDto>>('/api/v1/tmp-vouchers/unreviewed-count', {
    params: { typeId, sysId }
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 TmpVoucherM 資料表
- [ ] 建立 TmpVoucherD 資料表
- [ ] 建立 VoucherType 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含拋轉邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（關帳日期驗證等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 前端系統列表頁面開發
- [ ] 暫存傳票查詢頁面開發
- [ ] 暫存傳票維護頁面開發
- [ ] 傳票明細維護元件開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 拋轉功能測試
- [ ] 關帳日期驗證測試
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

### 6.1 業務邏輯
- 傳票日期必須大於關帳年月才可拋轉
- 拋轉前必須驗證傳票借貸平衡
- 已拋轉的傳票不可再修改或刪除
- 傳票明細必須至少有一筆
- 傳票借貸金額必須平衡
- 支援多系統傳票管理，各系統使用共同的TABLE和共同的拋轉程式

### 6.2 安全性
- 必須實作權限檢查（各系統的傳票審核權限）
- 拋轉操作必須記錄操作日誌
- 敏感資料必須加密傳輸 (HTTPS)

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 拋轉操作可能需要批次處理

### 6.4 資料驗證
- 傳票日期必須在有效範圍內
- 科目代號必須存在
- 部門代號必須存在（如果設定）
- 專案代號必須存在（如果設定）
- 借貸金額必須為正數
- 傳票借貸必須平衡

### 6.5 特殊功能
- 支援住宅金AHM款別代號
- 支援日立暫存傳票轉入
- 支援融資拋轉功能
- 支援應收付票據管理（三松住金）
- 支援關係人功能（OBJECT_ID_TYPE）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢前端系統列表成功
- [ ] 查詢暫存傳票列表成功
- [ ] 查詢單筆暫存傳票成功
- [ ] 修改暫存傳票成功
- [ ] 修改暫存傳票失敗（已拋轉）
- [ ] 刪除暫存傳票成功
- [ ] 刪除暫存傳票失敗（已拋轉）
- [ ] 拋轉暫存傳票成功
- [ ] 拋轉暫存傳票失敗（傳票日期小於關帳年月）
- [ ] 拋轉暫存傳票失敗（借貸不平衡）
- [ ] 批次拋轉暫存傳票成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 拋轉流程測試
- [ ] 關帳日期驗證測試
- [ ] 借貸平衡驗證測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 批次拋轉效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYSTA01_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA02_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FU.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA03_FUP.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FU.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA10_FUP.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA30_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA30_FU.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FU.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA40_FUP.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FU.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA70_FUP.ASP`
- `WEB/IMS_CORE/ASP/SYST000/SYSTA70_PR.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYST000/` 相關資料表結構
- `WEB/IMS_CORE/ASP/include/SYST000/` 相關工具函數

### 8.3 相關功能
- 會計憑證管理 (SYST121-SYST12B)
- 會計帳簿管理 (SYST131-SYST134)
- 交易資料處理 (SYST221)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

