# SYST211-SYST212 - 發票資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST211-SYST212 系列
- **功能名稱**: 發票資料維護系列
- **功能描述**: 提供傳票資料的新增、修改、刪除、查詢功能，包含傳票維護作業、費用/收入分攤比率設定等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FI.ASP` (新增 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FU.ASP` (修改 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FV.ASP` (修改 - 非手切傳票或非建立傳票)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FQ.ASP` (查詢 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FB.ASP` (瀏覽 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_PR.ASP` (報表 - 傳票維護作業)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FI_INV.ASP` (新增 - 進項稅額\折讓\退回發票)
  - `WEB/IMS_CORE/ASP/SYST000/SYST211_FU_INV.ASP` (修改 - 進項稅額\折讓\退回發票)
  - `WEB/IMS_CORE/ASP/SYST000/SYST212_FI.ASP` (新增 - 費用/收入分攤比率設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST212_FQ.ASP` (查詢 - 費用/收入分攤比率設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST212_FB.ASP` (瀏覽 - 費用/收入分攤比率設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST212_PR.ASP` (報表 - 費用/收入分攤比率設定)

### 1.2 業務需求
- 管理傳票主檔及明細資料
- 支援傳票新增、修改、刪除、查詢功能
- 支援傳票作廢功能
- 支援傳票審批流程
- 支援常用傳票套用功能
- 支援傳票列印功能
- 支援進項稅額、折讓、退回發票維護
- 支援費用/收入分攤比率設定
- 支援傳票自訂欄位（依據參數設定）
- 支援廠商名稱欄位（依據參數設定）
- 支援關係人功能（依據參數設定）
- 支援手開應收/應付票據維護

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Vouchers` (對應舊系統 `VOUCHER_M`)

```sql
CREATE TABLE [dbo].[Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 傳票編號 (VOUCHER_ID)
    [VoucherKind] NVARCHAR(10) NULL, -- 傳票種類 (VOUCHER_KIND, 1:一般, 2:建立, 3:分攤, 4:手切)
    [VoucherDate] DATETIME2 NOT NULL, -- 傳票日期 (VOUCHER_DATE)
    [VoucherStatus] NVARCHAR(10) NOT NULL DEFAULT '1', -- 傳票狀態 (VOUCHER_STATUS, 1:正常, 2:作廢, 3:已結帳)
    [Notes] NVARCHAR(500) NULL, -- 傳票摘要 (NOTES)
    [InvYn] NVARCHAR(1) NULL DEFAULT 'N', -- 是否補發票 (INV_YN, Y/N)
    [TypeId] NVARCHAR(50) NULL, -- 傳票型態 (TYPE_ID, 外鍵至VoucherTypes)
    [SiteId] NVARCHAR(50) NULL, -- 店代號 (SITE_ID)
    [VendorId] NVARCHAR(50) NULL, -- 廠客代號 (VENDOR_ID)
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Vouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Vouchers_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherId] ON [dbo].[Vouchers] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherDate] ON [dbo].[Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherStatus] ON [dbo].[Vouchers] ([VoucherStatus]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VoucherKind] ON [dbo].[Vouchers] ([VoucherKind]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_TypeId] ON [dbo].[Vouchers] ([TypeId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_SiteId] ON [dbo].[Vouchers] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Vouchers_VendorId] ON [dbo].[Vouchers] ([VendorId]);
```

### 2.2 主要資料表: `VoucherDetails` (對應舊系統 `VOUCHER_D`)

```sql
CREATE TABLE [dbo].[VoucherDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔T_KEY (VOUCHER_T_KEY, 外鍵至Vouchers)
    [Sn] INT NOT NULL, -- 序號 (SN)
    [Amt] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 金額 (AMT)
    [Dc] NVARCHAR(1) NOT NULL, -- 借/貸 (DC, D:借方, C:貸方)
    [StypeId] NVARCHAR(50) NULL, -- 會計科目 (STYPE_ID, 外鍵至AccountSubjects)
    [OrgId] NVARCHAR(50) NULL, -- 組織代號 (ORG_ID)
    [ActId] NVARCHAR(50) NULL, -- 對象代號 (ACT_ID)
    [AbatId] NVARCHAR(50) NULL, -- 立沖代號 (ABAT_ID)
    [Notes] NVARCHAR(500) NULL, -- 摘要 (NOTES)
    [VendorId] NVARCHAR(50) NULL, -- 廠商/員工代號 (VENDOR_ID)
    [AcctKey] NVARCHAR(50) NULL, -- 帳戶KEY值 (ACCT_KEY)
    [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
    [CustomField1] NVARCHAR(200) NULL, -- 自訂欄位1 (CUSTOM_FIELD1)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_VoucherDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_VoucherDetails_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VoucherTKey] ON [dbo].[VoucherDetails] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_StypeId] ON [dbo].[VoucherDetails] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_OrgId] ON [dbo].[VoucherDetails] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_VoucherDetails_VendorId] ON [dbo].[VoucherDetails] ([VendorId]);
```

### 2.3 主要資料表: `InvoiceVouchers` (對應舊系統進項/銷項發票傳票)

```sql
CREATE TABLE [dbo].[InvoiceVouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherTKey] BIGINT NOT NULL, -- 傳票主檔T_KEY (外鍵至Vouchers)
    [InvoiceType] NVARCHAR(10) NOT NULL, -- 發票類型 (INVOICE_TYPE, 1:進項稅額, 2:進項折讓, 3:進項退回, 4:銷項稅額, 5:銷項折讓, 6:銷項退回)
    [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼 (INVOICE_NO)
    [InvoiceDate] DATETIME2 NULL, -- 發票日期 (INVOICE_DATE)
    [InvoiceFormat] NVARCHAR(10) NULL, -- 發票格式 (INVOICE_FORMAT)
    [InvoiceAmount] DECIMAL(18, 2) NULL, -- 發票金額 (INVOICE_AMOUNT)
    [TaxAmount] DECIMAL(18, 2) NULL, -- 稅額 (TAX_AMOUNT)
    [DeductCode] NVARCHAR(50) NULL, -- 扣抵代號 (DEDUCT_CODE)
    [CategoryType] NVARCHAR(50) NULL, -- 類別區分 (CATEGORY_TYPE)
    [VoucherNo] NVARCHAR(50) NULL, -- 憑證單號 (VOUCHER_NO)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_InvoiceVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_InvoiceVouchers_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_VoucherTKey] ON [dbo].[InvoiceVouchers] ([VoucherTKey]);
CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_InvoiceNo] ON [dbo].[InvoiceVouchers] ([InvoiceNo]);
CREATE NONCLUSTERED INDEX [IX_InvoiceVouchers_InvoiceType] ON [dbo].[InvoiceVouchers] ([InvoiceType]);
```

### 2.4 主要資料表: `AllocationRatios` (對應舊系統費用/收入分攤比率設定)

```sql
CREATE TABLE [dbo].[AllocationRatios] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DisYm] NVARCHAR(6) NOT NULL, -- 分攤年月 (DIS_YM, YYYYMM)
    [StypeId] NVARCHAR(50) NOT NULL, -- 會計科目 (STYPE_ID, 外鍵至AccountSubjects)
    [OrgId] NVARCHAR(50) NOT NULL, -- 組織代號 (ORG_ID)
    [Ratio] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 分攤比率 (RATIO, 0-1)
    [VoucherTKey] BIGINT NULL, -- 傳票主檔T_KEY (VOUCHER_T_KEY, 外鍵至Vouchers)
    [VoucherDTKey] BIGINT NULL, -- 傳票明細T_KEY (VOUCHER_D_T_KEY, 外鍵至VoucherDetails)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_AllocationRatios] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_AllocationRatios_Vouchers] FOREIGN KEY ([VoucherTKey]) REFERENCES [dbo].[Vouchers] ([TKey]),
    CONSTRAINT [FK_AllocationRatios_VoucherDetails] FOREIGN KEY ([VoucherDTKey]) REFERENCES [dbo].[VoucherDetails] ([TKey])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AllocationRatios_DisYm] ON [dbo].[AllocationRatios] ([DisYm]);
CREATE NONCLUSTERED INDEX [IX_AllocationRatios_StypeId] ON [dbo].[AllocationRatios] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_AllocationRatios_OrgId] ON [dbo].[AllocationRatios] ([OrgId]);
```

### 2.5 資料字典

#### Vouchers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherId | NVARCHAR | 50 | NO | - | 傳票編號 | 唯一，主鍵候選 |
| VoucherKind | NVARCHAR | 10 | YES | - | 傳票種類 | 1:一般, 2:建立, 3:分攤, 4:手切 |
| VoucherDate | DATETIME2 | - | NO | - | 傳票日期 | - |
| VoucherStatus | NVARCHAR | 10 | NO | '1' | 傳票狀態 | 1:正常, 2:作廢, 3:已結帳 |
| Notes | NVARCHAR | 500 | YES | - | 傳票摘要 | - |
| InvYn | NVARCHAR | 1 | YES | 'N' | 是否補發票 | Y/N |
| TypeId | NVARCHAR | 50 | YES | - | 傳票型態 | 外鍵至VoucherTypes |
| SiteId | NVARCHAR | 50 | YES | - | 店代號 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠客代號 | - |
| VendorName | NVARCHAR | 200 | YES | - | 廠商名稱 | - |

#### VoucherDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherTKey | BIGINT | - | NO | - | 傳票主檔T_KEY | 外鍵至Vouchers |
| Sn | INT | - | NO | - | 序號 | - |
| Amt | DECIMAL | 18,2 | NO | 0 | 金額 | - |
| Dc | NVARCHAR | 1 | NO | - | 借/貸 | D:借方, C:貸方 |
| StypeId | NVARCHAR | 50 | YES | - | 會計科目 | 外鍵至AccountSubjects |
| OrgId | NVARCHAR | 50 | YES | - | 組織代號 | - |
| ActId | NVARCHAR | 50 | YES | - | 對象代號 | - |
| AbatId | NVARCHAR | 50 | YES | - | 立沖代號 | - |
| Notes | NVARCHAR | 500 | YES | - | 摘要 | - |
| VendorId | NVARCHAR | 50 | YES | - | 廠商/員工代號 | - |
| CustomField1 | NVARCHAR | 200 | YES | - | 自訂欄位1 | 依據參數設定 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢傳票列表
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
      "voucherStatus": "",
      "voucherKind": "",
      "typeId": "",
      "siteId": "",
      "vendorId": ""
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
          "voucherId": "V20240101001",
          "voucherDate": "2024-01-01",
          "voucherStatus": "1",
          "voucherKind": "1",
          "notes": "傳票摘要",
          "typeId": "GL",
          "siteId": "S001",
          "vendorId": "V001"
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

#### 3.1.2 查詢單筆傳票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 根據傳票編號查詢單筆傳票資料（含明細）
- **路徑參數**:
  - `voucherId`: 傳票編號
- **回應格式**: 包含傳票主檔及明細資料

#### 3.1.3 新增傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers`
- **說明**: 新增傳票資料（含主檔及明細）
- **請求格式**:
  ```json
  {
    "voucherDate": "2024-01-01",
    "voucherKind": "1",
    "typeId": "GL",
    "notes": "傳票摘要",
    "siteId": "S001",
    "vendorId": "V001",
    "vendorName": "廠商名稱",
    "invYn": "N",
    "details": [
      {
        "sn": 1,
        "amt": 1000,
        "dc": "D",
        "stypeId": "1000",
        "orgId": "ORG001",
        "actId": "",
        "abatId": "",
        "notes": "明細摘要",
        "vendorId": "",
        "customField1": ""
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "voucherId": "V20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改傳票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 修改傳票資料（需檢查傳票狀態及權限）
- **路徑參數**:
  - `voucherId`: 傳票編號
- **請求格式**: 同新增，但需包含 `tKey` 欄位
- **回應格式**: 同新增

#### 3.1.5 刪除傳票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vouchers/{voucherId}`
- **說明**: 刪除傳票資料（需檢查傳票狀態及權限）
- **路徑參數**:
  - `voucherId`: 傳票編號
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

#### 3.1.6 作廢傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/{voucherId}/void`
- **說明**: 作廢傳票（需檢查傳票狀態及權限）
- **路徑參數**:
  - `voucherId`: 傳票編號
- **回應格式**: 同刪除

#### 3.1.7 套用常用傳票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/apply-common-voucher`
- **說明**: 套用常用傳票資料
- **請求格式**:
  ```json
  {
    "commonVoucherId": "CV001",
    "voucherDate": "2024-01-01",
    "siteId": "S001"
  }
  ```
- **回應格式**: 返回傳票資料結構

#### 3.1.8 檢查傳票借貸平衡
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vouchers/{voucherId}/check-balance`
- **說明**: 檢查傳票借貸是否平衡
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "isBalanced": true,
      "debitAmount": 1000,
      "creditAmount": 1000,
      "difference": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.9 查詢費用/收入分攤比率
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/allocation-ratios`
- **說明**: 查詢費用/收入分攤比率設定
- **請求參數**:
  ```json
  {
    "disYm": "202401",
    "stypeId": "",
    "orgId": ""
  }
  ```
- **回應格式**: 返回分攤比率列表

#### 3.1.10 新增費用/收入分攤比率
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/allocation-ratios`
- **說明**: 新增費用/收入分攤比率設定
- **請求格式**:
  ```json
  {
    "disYm": "202401",
    "stypeId": "1000",
    "orgId": "ORG001",
    "ratio": 0.5,
    "voucherTKey": null,
    "voucherDTKey": null
  }
  ```
- **回應格式**: 返回新增結果

### 3.2 後端實作類別

#### 3.2.1 Controller: `VouchersController.cs`
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
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVoucher([FromBody] CreateVoucherDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoucher(string voucherId, [FromBody] UpdateVoucherDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{voucherId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoucher(string voucherId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{voucherId}/void")]
        public async Task<ActionResult<ApiResponse>> VoidVoucher(string voucherId)
        {
            // 實作作廢邏輯
        }
        
        [HttpPost("apply-common-voucher")]
        public async Task<ActionResult<ApiResponse<VoucherDto>>> ApplyCommonVoucher([FromBody] ApplyCommonVoucherDto dto)
        {
            // 實作套用常用傳票邏輯
        }
        
        [HttpGet("{voucherId}/check-balance")]
        public async Task<ActionResult<ApiResponse<BalanceCheckDto>>> CheckBalance(string voucherId)
        {
            // 實作檢查借貸平衡邏輯
        }
    }
}
```

#### 3.2.2 Service: `VoucherService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVoucherService
    {
        Task<PagedResult<VoucherDto>> GetVouchersAsync(VoucherQueryDto query);
        Task<VoucherDto> GetVoucherByIdAsync(string voucherId);
        Task<string> CreateVoucherAsync(CreateVoucherDto dto);
        Task UpdateVoucherAsync(string voucherId, UpdateVoucherDto dto);
        Task DeleteVoucherAsync(string voucherId);
        Task VoidVoucherAsync(string voucherId);
        Task<VoucherDto> ApplyCommonVoucherAsync(ApplyCommonVoucherDto dto);
        Task<BalanceCheckDto> CheckBalanceAsync(string voucherId);
    }
}
```

#### 3.2.3 Repository: `VoucherRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IVoucherRepository
    {
        Task<Voucher> GetByIdAsync(string voucherId);
        Task<PagedResult<Voucher>> GetPagedAsync(VoucherQuery query);
        Task<Voucher> CreateAsync(Voucher voucher);
        Task<Voucher> UpdateAsync(Voucher voucher);
        Task DeleteAsync(string voucherId);
        Task<bool> ExistsAsync(string voucherId);
        Task<string> GenerateVoucherIdAsync(DateTime voucherDate);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 傳票列表頁面 (`VoucherList.vue`)
- **路徑**: `/accounting/vouchers`
- **功能**: 顯示傳票列表，支援查詢、新增、修改、刪除、作廢
- **主要元件**:
  - 查詢表單 (VoucherSearchForm)
  - 資料表格 (VoucherDataTable)
  - 新增/修改對話框 (VoucherDialog)
  - 作廢確認對話框

#### 4.1.2 傳票詳細頁面 (`VoucherDetail.vue`)
- **路徑**: `/accounting/vouchers/:voucherId`
- **功能**: 顯示傳票詳細資料（含明細），支援修改、作廢

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`VoucherSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="傳票編號">
      <el-input v-model="searchForm.voucherId" placeholder="請輸入傳票編號" />
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
    <el-form-item label="傳票狀態">
      <el-select v-model="searchForm.voucherStatus" placeholder="請選擇">
        <el-option label="正常" value="1" />
        <el-option label="作廢" value="2" />
        <el-option label="已結帳" value="3" />
      </el-select>
    </el-form-item>
    <el-form-item label="傳票種類">
      <el-select v-model="searchForm.voucherKind" placeholder="請選擇">
        <el-option label="一般" value="1" />
        <el-option label="建立" value="2" />
        <el-option label="分攤" value="3" />
        <el-option label="手切" value="4" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`VoucherDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="voucherList" v-loading="loading">
      <el-table-column prop="voucherId" label="傳票編號" width="150" />
      <el-table-column prop="voucherDate" label="傳票日期" width="120" />
      <el-table-column prop="voucherStatus" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.voucherStatus)">
            {{ getStatusText(row.voucherStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="摘要" width="200" />
      <el-table-column prop="typeId" label="傳票型態" width="120" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="warning" size="small" @click="handleVoid(row)" v-if="row.voucherStatus === '1'">作廢</el-button>
        </template>
      </el-table-column>
    </el-table>
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
</template>
```

#### 4.2.3 新增/修改對話框 (`VoucherDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1200px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="傳票編號" prop="voucherId">
            <el-input v-model="form.voucherId" :disabled="isEdit" placeholder="自動產生" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="傳票日期" prop="voucherDate">
            <el-date-picker v-model="form.voucherDate" type="date" placeholder="請選擇傳票日期" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="傳票種類" prop="voucherKind">
            <el-select v-model="form.voucherKind" placeholder="請選擇">
              <el-option label="一般" value="1" />
              <el-option label="建立" value="2" />
              <el-option label="分攤" value="3" />
              <el-option label="手切" value="4" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="傳票型態" prop="typeId">
            <el-select v-model="form.typeId" placeholder="請選擇" filterable>
              <el-option v-for="item in voucherTypeList" :key="item.voucherId" :label="item.voucherName" :value="item.voucherId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="店代號" prop="siteId">
            <el-select v-model="form.siteId" placeholder="請選擇" filterable clearable>
              <el-option v-for="item in siteList" :key="item.siteId" :label="item.siteName" :value="item.siteId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="傳票摘要" prop="notes">
            <el-input v-model="form.notes" placeholder="請輸入傳票摘要" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20" v-if="showVendorName">
        <el-col :span="8">
          <el-form-item label="廠客代號" prop="vendorId">
            <el-select v-model="form.vendorId" placeholder="請選擇" filterable clearable @change="handleVendorChange">
              <el-option v-for="item in vendorList" :key="item.vendorId" :label="item.vendorName" :value="item.vendorId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="廠商名稱" prop="vendorName">
            <el-input v-model="form.vendorName" placeholder="請輸入廠商名稱" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-divider>傳票明細</el-divider>
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="dc" label="借/貸" width="80">
          <template #default="{ row, $index }">
            <el-select v-model="row.dc" placeholder="請選擇">
              <el-option label="借方" value="D" />
              <el-option label="貸方" value="C" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="stypeId" label="會計科目" width="150">
          <template #default="{ row, $index }">
            <el-select v-model="row.stypeId" placeholder="請選擇" filterable @change="handleStypeChange($index)">
              <el-option v-for="item in accountSubjectList" :key="item.stypeId" :label="item.stypeName" :value="item.stypeId" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="orgId" label="組織代號" width="150">
          <template #default="{ row, $index }">
            <el-select v-model="row.orgId" placeholder="請選擇" filterable clearable>
              <el-option v-for="item in orgList" :key="item.orgId" :label="item.orgName" :value="item.orgId" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="amt" label="金額" width="150">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.amt" :min="0" :precision="2" />
          </template>
        </el-table-column>
        <el-table-column prop="notes" label="摘要" width="200">
          <template #default="{ row, $index }">
            <el-input v-model="row.notes" placeholder="請輸入摘要" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
      <el-button type="success" @click="handleApplyCommonVoucher">套用常用傳票</el-button>
      <el-divider />
      <el-row>
        <el-col :span="12">
          <el-form-item label="借方合計">
            <el-input :value="debitTotal" :disabled="true" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="貸方合計">
            <el-input :value="creditTotal" :disabled="true" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :span="12">
          <el-form-item label="差額">
            <el-input :value="difference" :disabled="true" :class="difference !== 0 ? 'error' : ''" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`voucher.api.ts`)
```typescript
import request from '@/utils/request';

export interface VoucherDto {
  tKey: number;
  voucherId: string;
  voucherDate: string;
  voucherStatus: string;
  voucherKind: string;
  notes?: string;
  typeId?: string;
  siteId?: string;
  vendorId?: string;
  vendorName?: string;
  invYn?: string;
  details?: VoucherDetailDto[];
}

export interface VoucherDetailDto {
  tKey?: number;
  sn: number;
  amt: number;
  dc: string;
  stypeId?: string;
  orgId?: string;
  actId?: string;
  abatId?: string;
  notes?: string;
  vendorId?: string;
  customField1?: string;
}

export interface VoucherQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    voucherId?: string;
    voucherDateFrom?: string;
    voucherDateTo?: string;
    voucherStatus?: string;
    voucherKind?: string;
    typeId?: string;
    siteId?: string;
    vendorId?: string;
  };
}

export interface CreateVoucherDto {
  voucherDate: string;
  voucherKind: string;
  typeId?: string;
  notes?: string;
  siteId?: string;
  vendorId?: string;
  vendorName?: string;
  invYn?: string;
  details: VoucherDetailDto[];
}

export interface UpdateVoucherDto extends Omit<CreateVoucherDto, 'voucherDate'> {}

export interface BalanceCheckDto {
  isBalanced: boolean;
  debitAmount: number;
  creditAmount: number;
  difference: number;
}

// API 函數
export const getVoucherList = (query: VoucherQueryDto) => {
  return request.get<ApiResponse<PagedResult<VoucherDto>>>('/api/v1/vouchers', { params: query });
};

export const getVoucherById = (voucherId: string) => {
  return request.get<ApiResponse<VoucherDto>>(`/api/v1/vouchers/${voucherId}`);
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

export const voidVoucher = (voucherId: string) => {
  return request.post<ApiResponse>(`/api/v1/vouchers/${voucherId}/void`);
};

export const applyCommonVoucher = (data: ApplyCommonVoucherDto) => {
  return request.post<ApiResponse<VoucherDto>>('/api/v1/vouchers/apply-common-voucher', data);
};

export const checkBalance = (voucherId: string) => {
  return request.get<ApiResponse<BalanceCheckDto>>(`/api/v1/vouchers/${voucherId}/check-balance`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（借貸平衡檢查、傳票狀態檢查、權限檢查等）
- [ ] 傳票編號產生邏輯
- [ ] 常用傳票套用邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 傳票明細表格開發
- [ ] 借貸平衡檢查功能
- [ ] 常用傳票套用功能
- [ ] 參數控制欄位顯示（廠商名稱、關係人、自訂欄位等）
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試（借貸平衡、傳票狀態、權限檢查等）

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 17天

---

## 六、注意事項

### 6.1 業務邏輯
- 傳票編號必須唯一，且需依據傳票日期自動產生
- 傳票借貸必須平衡（借方金額 = 貸方金額）
- 新增傳票時需檢查借貸平衡
- 修改傳票時需檢查借貸平衡
- 刪除傳票時需檢查傳票狀態（已結帳的傳票不可刪除）
- 作廢傳票時需檢查傳票狀態及權限
- 非原始傳票建立者刪除或作廢傳票時需提示確認
- 傳票明細的會計科目必須為相同科目類別（依據參數設定）
- 選擇立沖代號後需帶回立沖餘額及原本的組織代號
- 套用常用傳票時需帶入傳票名稱至傳票摘要

### 6.2 參數控制
- 廠商名稱欄位：由參數 `VOUCHER_VENDOR_NAME` 控制
- 關係人功能：由參數 `OBJECT_ID_TYPE` 控制（0:無, 1:必填, 2:非必填）
- 自訂欄位：由參數 `VOUCHER_CUSTOM_FIELD` 控制（格式：欄位名稱,欄位長度）
- 傳票日期預設：由參數 `DEF_VOUCHER_DATE` 控制
- 手開應收/應付票據維護：由參數 `HAND_APV_ARV_MODE` 控制

### 6.3 資料驗證
- 傳票編號必須唯一
- 傳票日期必須驗證
- 傳票借貸必須平衡
- 必填欄位必須驗證
- 會計科目必須驗證
- 組織代號必須驗證

### 6.4 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 傳票明細查詢需使用 JOIN 優化
- 常用傳票列表可快取

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增傳票成功
- [ ] 新增傳票失敗 (借貸不平衡)
- [ ] 修改傳票成功
- [ ] 修改傳票失敗 (已結帳)
- [ ] 刪除傳票成功
- [ ] 刪除傳票失敗 (已結帳)
- [ ] 作廢傳票成功
- [ ] 作廢傳票失敗 (已作廢)
- [ ] 套用常用傳票成功
- [ ] 檢查借貸平衡成功
- [ ] 查詢傳票列表成功
- [ ] 查詢單筆傳票成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 參數控制欄位顯示測試
- [ ] 借貸平衡檢查測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 傳票明細查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FI.ASP` - 新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FV.ASP` - 修改畫面（非手切傳票或非建立傳票）
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_PR.ASP` - 報表畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FI_INV.ASP` - 進項發票新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST211_FU_INV.ASP` - 進項發票修改畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST212_FI.ASP` - 費用/收入分攤比率設定新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST212_FQ.ASP` - 費用/收入分攤比率設定查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST212_FB.ASP` - 費用/收入分攤比率設定瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST212_PR.ASP` - 費用/收入分攤比率設定報表畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`VOUCHER_M` (傳票主檔), `VOUCHER_D` (傳票明細)
- 主要欄位：VOUCHER_ID, VOUCHER_DATE, VOUCHER_STATUS, VOUCHER_KIND, NOTES, TYPE_ID, SITE_ID, VENDOR_ID, VENDOR_NAME

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

