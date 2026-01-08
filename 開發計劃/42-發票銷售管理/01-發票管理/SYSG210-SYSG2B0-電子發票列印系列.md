# SYSG210-SYSG2B0 - 電子發票列印系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG210-SYSG2B0 系列
- **功能名稱**: 電子發票列印系列
- **功能描述**: 提供電子發票列印功能，包含電子發票手動取號列印、電子發票中獎清冊、電子發票列印設定等作業
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG210_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG220_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG230_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG240_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG250_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG260_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG270_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG280_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG290_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG2A0_*.ASP` (電子發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG2B0_PR.ASP` (電子發票中獎清冊)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12E_PT.ASP` (電子發票手動取號列印)
  - `WEB/IMS_CORE/ASP/SYSG000/einv_print*.ASP` (電子發票列印功能)

### 1.2 業務需求
- 支援電子發票手動取號列印
- 支援電子發票中獎清冊查詢與列印
- 支援電子發票列印設定
- 支援電子發票條碼列印
- 支援電子發票批次列印
- 支援電子發票預覽功能
- 支援多種電子發票格式（A4、A5、熱感紙等）
- 支援電子發票QR Code列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ElectronicInvoices` (電子發票主檔)

```sql
CREATE TABLE [dbo].[ElectronicInvoices] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PosId] NVARCHAR(50) NULL, -- POS代碼
    [InvYm] NVARCHAR(6) NOT NULL, -- 發票年月 (YYYYMM)
    [Track] NVARCHAR(2) NULL, -- 字軌
    [InvNoB] NVARCHAR(10) NULL, -- 發票號碼起
    [InvNoE] NVARCHAR(10) NULL, -- 發票號碼迄
    [PrintCode] NVARCHAR(50) NULL, -- 列印條碼
    [InvoiceDate] DATETIME2 NULL, -- 發票日期
    [PrizeType] NVARCHAR(10) NULL, -- 獎項類型
    [PrizeAmt] DECIMAL(18, 4) NULL, -- 獎項金額
    [CarrierIdClear] NVARCHAR(50) NULL, -- 載具識別碼（明碼）
    [AwardPrint] NVARCHAR(10) NULL, -- 中獎列印標記
    [AwardPos] NVARCHAR(50) NULL, -- 中獎POS
    [AwardDate] DATETIME2 NULL, -- 中獎日期
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_InvYm] ON [dbo].[ElectronicInvoices] ([InvYm]);
CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_Track] ON [dbo].[ElectronicInvoices] ([Track]);
CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_PrintCode] ON [dbo].[ElectronicInvoices] ([PrintCode]);
CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_PrizeType] ON [dbo].[ElectronicInvoices] ([PrizeType]);
CREATE NONCLUSTERED INDEX [IX_ElectronicInvoices_Status] ON [dbo].[ElectronicInvoices] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `ElectronicInvoicePrintSettings` - 電子發票列印設定
```sql
CREATE TABLE [dbo].[ElectronicInvoicePrintSettings] (
    [SettingId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PrintFormat] NVARCHAR(20) NOT NULL, -- 列印格式 (A4, A5, THERMAL)
    [BarcodeType] NVARCHAR(20) NULL, -- 條碼類型 (code128, ean13等)
    [BarcodeSize] INT NULL DEFAULT 40, -- 條碼高度
    [BarcodeMargin] INT NULL DEFAULT 5, -- 條碼間距
    [ColCount] INT NULL DEFAULT 2, -- 每頁欄數
    [PageCount] INT NULL DEFAULT 14, -- 每頁筆數
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

### 2.3 資料字典

#### 2.3.1 ElectronicInvoices 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| PosId | NVARCHAR | 50 | YES | - | POS代碼 | - |
| InvYm | NVARCHAR | 6 | NO | - | 發票年月 | YYYYMM格式 |
| Track | NVARCHAR | 2 | YES | - | 字軌 | - |
| InvNoB | NVARCHAR | 10 | YES | - | 發票號碼起 | - |
| InvNoE | NVARCHAR | 10 | YES | - | 發票號碼迄 | - |
| PrintCode | NVARCHAR | 50 | YES | - | 列印條碼 | - |
| InvoiceDate | DATETIME2 | - | YES | - | 發票日期 | - |
| PrizeType | NVARCHAR | 10 | YES | - | 獎項類型 | - |
| PrizeAmt | DECIMAL | 18,4 | YES | - | 獎項金額 | - |
| CarrierIdClear | NVARCHAR | 50 | YES | - | 載具識別碼（明碼） | - |
| AwardPrint | NVARCHAR | 10 | YES | - | 中獎列印標記 | - |
| AwardPos | NVARCHAR | 50 | YES | - | 中獎POS | - |
| AwardDate | DATETIME2 | - | YES | - | 中獎日期 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢電子發票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/electronic-invoices`
- **說明**: 查詢電子發票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "InvYm",
    "sortOrder": "DESC",
    "filters": {
      "invYm": "",
      "track": "",
      "posId": "",
      "prizeType": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆電子發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/electronic-invoices/{tKey}`
- **說明**: 查詢單筆電子發票資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增電子發票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/electronic-invoices`
- **說明**: 新增電子發票資料
- **請求格式**:
  ```json
  {
    "posId": "POS001",
    "invYm": "202401",
    "track": "AA",
    "invNoB": "00000001",
    "invNoE": "00000100",
    "printCode": "PRINT001",
    "invoiceDate": "2024-01-01",
    "status": "A"
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改電子發票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/electronic-invoices/{tKey}`
- **說明**: 修改電子發票資料
- **請求格式**: 同新增格式
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除電子發票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/electronic-invoices/{tKey}`
- **說明**: 刪除電子發票資料
- **回應格式**: 標準刪除回應格式

#### 3.1.6 電子發票手動取號列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/electronic-invoices/manual-print`
- **說明**: 電子發票手動取號列印
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3],
    "printFormat": "A4",
    "barcodeType": "code128"
  }
  ```
- **回應格式**: 標準回應格式，包含列印資料

#### 3.1.7 電子發票中獎清冊查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/electronic-invoices/award-list`
- **說明**: 查詢電子發票中獎清冊
- **請求格式**:
  ```json
  {
    "invYm": "202401",
    "prizeType": "",
    "pageIndex": 1,
    "pageSize": 20
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.8 電子發票中獎清冊列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/electronic-invoices/award-print`
- **說明**: 列印電子發票中獎清冊
- **請求格式**:
  ```json
  {
    "invYm": "202401",
    "prizeType": ""
  }
  ```
- **回應格式**: 標準回應格式，包含列印資料

#### 3.1.9 查詢電子發票列印設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/electronic-invoice-print-settings`
- **說明**: 查詢電子發票列印設定
- **回應格式**: 標準單筆回應格式

#### 3.1.10 更新電子發票列印設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/electronic-invoice-print-settings/{settingId}`
- **說明**: 更新電子發票列印設定
- **請求格式**:
  ```json
  {
    "printFormat": "A4",
    "barcodeType": "code128",
    "barcodeSize": 40,
    "barcodeMargin": 5,
    "colCount": 2,
    "pageCount": 14
  }
  ```
- **回應格式**: 標準修改回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `ElectronicInvoicesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/electronic-invoices")]
    [Authorize]
    public class ElectronicInvoicesController : ControllerBase
    {
        private readonly IElectronicInvoiceService _electronicInvoiceService;
        
        public ElectronicInvoicesController(IElectronicInvoiceService electronicInvoiceService)
        {
            _electronicInvoiceService = electronicInvoiceService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ElectronicInvoiceDto>>>> GetElectronicInvoices([FromQuery] ElectronicInvoiceQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<ElectronicInvoiceDto>>> GetElectronicInvoice(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateElectronicInvoice([FromBody] CreateElectronicInvoiceDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateElectronicInvoice(long tKey, [FromBody] UpdateElectronicInvoiceDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteElectronicInvoice(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("manual-print")]
        public async Task<ActionResult<ApiResponse<PrintDataDto>>> ManualPrint([FromBody] ManualPrintDto dto)
        {
            // 實作手動取號列印邏輯
        }
        
        [HttpPost("award-list")]
        public async Task<ActionResult<ApiResponse<PagedResult<ElectronicInvoiceAwardDto>>>> GetAwardList([FromBody] AwardListQueryDto query)
        {
            // 實作中獎清冊查詢邏輯
        }
        
        [HttpPost("award-print")]
        public async Task<ActionResult<ApiResponse<PrintDataDto>>> AwardPrint([FromBody] AwardPrintDto dto)
        {
            // 實作中獎清冊列印邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 電子發票列印頁面 (`ElectronicInvoicePrint.vue`)
- **路徑**: `/invoice/electronic-invoice-print`
- **功能**: 顯示電子發票列印介面，支援手動取號列印、中獎清冊查詢與列印

#### 4.1.2 電子發票中獎清冊頁面 (`ElectronicInvoiceAwardList.vue`)
- **路徑**: `/invoice/electronic-invoice-award-list`
- **功能**: 顯示電子發票中獎清冊查詢與列印介面

### 4.2 UI 元件設計

#### 4.2.1 電子發票列印表單 (`ElectronicInvoicePrintForm.vue`)
- 發票年月選擇
- POS代碼選擇
- 字軌輸入
- 發票號碼區間輸入
- 列印格式選擇（A4、A5、熱感紙）
- 條碼類型選擇
- 列印按鈕

#### 4.2.2 電子發票中獎清冊查詢表單 (`AwardListSearchForm.vue`)
- 發票年月選擇
- 獎項類型選擇
- 查詢按鈕
- 列印按鈕

### 4.3 API 呼叫 (`electronicInvoice.api.ts`)
```typescript
import request from '@/utils/request';

export interface ElectronicInvoiceDto {
  tKey: number;
  posId?: string;
  invYm: string;
  track?: string;
  invNoB?: string;
  invNoE?: string;
  printCode?: string;
  invoiceDate?: string;
  prizeType?: string;
  prizeAmt?: number;
  status: string;
}

// API 函數
export const getElectronicInvoiceList = (query: ElectronicInvoiceQueryDto) => {
  return request.get<ApiResponse<PagedResult<ElectronicInvoiceDto>>>('/api/v1/electronic-invoices', { params: query });
};

export const getElectronicInvoiceById = (tKey: number) => {
  return request.get<ApiResponse<ElectronicInvoiceDto>>(`/api/v1/electronic-invoices/${tKey}`);
};

export const createElectronicInvoice = (data: CreateElectronicInvoiceDto) => {
  return request.post<ApiResponse<number>>('/api/v1/electronic-invoices', data);
};

export const updateElectronicInvoice = (tKey: number, data: UpdateElectronicInvoiceDto) => {
  return request.put<ApiResponse>(`/api/v1/electronic-invoices/${tKey}`, data);
};

export const deleteElectronicInvoice = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/electronic-invoices/${tKey}`);
};

export const manualPrint = (data: ManualPrintDto) => {
  return request.post<ApiResponse<PrintDataDto>>('/api/v1/electronic-invoices/manual-print', data);
};

export const getAwardList = (query: AwardListQueryDto) => {
  return request.post<ApiResponse<PagedResult<ElectronicInvoiceAwardDto>>>('/api/v1/electronic-invoices/award-list', query);
};

export const awardPrint = (data: AwardPrintDto) => {
  return request.post<ApiResponse<PrintDataDto>>('/api/v1/electronic-invoices/award-print', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 列印邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列印頁面開發
- [ ] 中獎清冊頁面開發
- [ ] 查詢表單開發
- [ ] 列印預覽功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 列印功能測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 列印資料必須加密傳輸
- 敏感資料必須加密儲存

### 6.2 效能
- 大量資料列印必須使用批次處理
- 必須建立適當的索引
- 列印資料必須快取

### 6.3 資料驗證
- 發票年月格式必須驗證（YYYYMM）
- 發票號碼區間必須驗證（起號 <= 迄號）
- 列印格式必須驗證

### 6.4 業務邏輯
- 列印前必須檢查發票資料完整性
- 列印後必須記錄列印記錄
- 中獎清冊必須按照獎項類型排序

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增電子發票成功
- [ ] 修改電子發票成功
- [ ] 刪除電子發票成功
- [ ] 查詢電子發票列表成功
- [ ] 手動取號列印成功
- [ ] 中獎清冊查詢成功
- [ ] 中獎清冊列印成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 列印功能測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料列印測試
- [ ] 並發列印測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSG000/SYSG210_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG2B0_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12E_PT.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/einv_print*.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSG000/SP/*.sql`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

