# SYSE310-SYSE430 - 費用資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSE310-SYSE430 系列
- **功能名稱**: 費用資料維護系列
- **功能描述**: 提供費用資料的新增、修改、刪除、查詢功能，包含費用編號、費用類型、費用項目、費用金額、費用日期、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE320_RENT.ASP` (租賃試算)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE320_FEES.ASP` (費用試算)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE330_RENT.ASP` (扣款試算)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE330_FEES.ASP` (費用試算)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE340_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE340_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE340_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE350_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE350_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE405_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE430_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理費用基本資料
- 支援費用類型管理（租金、管理費、水電費、其他費用等）
- 支援費用項目選擇
- 支援費用金額計算
- 支援費用試算功能
- 支援租賃試算功能
- 支援扣款試算功能
- 支援費用報表列印
- 支援費用歷史記錄查詢
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LeaseFees` (費用主檔)

```sql
CREATE TABLE [dbo].[LeaseFees] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [FeeId] NVARCHAR(50) NOT NULL, -- 費用編號 (FEE_ID)
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [FeeType] NVARCHAR(20) NOT NULL, -- 費用類型 (FEE_TYPE, RENT:租金, MANAGEMENT:管理費, UTILITY:水電費, OTHER:其他費用)
    [FeeItemId] NVARCHAR(50) NULL, -- 費用項目編號 (FEE_ITEM_ID)
    [FeeItemName] NVARCHAR(200) NULL, -- 費用項目名稱 (FEE_ITEM_NAME)
    [FeeAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 費用金額 (FEE_AMOUNT)
    [FeeDate] DATETIME2 NOT NULL, -- 費用日期 (FEE_DATE)
    [DueDate] DATETIME2 NULL, -- 到期日期 (DUE_DATE)
    [PaidDate] DATETIME2 NULL, -- 繳費日期 (PAID_DATE)
    [PaidAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 已繳金額 (PAID_AMOUNT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:待繳, P:部分繳, F:已繳, C:已取消)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMOUNT, 含稅)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_LeaseFees] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseFees_FeeId] UNIQUE ([FeeId]),
    CONSTRAINT [FK_LeaseFees_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeId] ON [dbo].[LeaseFees] ([FeeId]);
CREATE NONCLUSTERED INDEX [IX_LeaseFees_LeaseId] ON [dbo].[LeaseFees] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeType] ON [dbo].[LeaseFees] ([FeeType]);
CREATE NONCLUSTERED INDEX [IX_LeaseFees_Status] ON [dbo].[LeaseFees] ([Status]);
CREATE NONCLUSTERED INDEX [IX_LeaseFees_FeeDate] ON [dbo].[LeaseFees] ([FeeDate]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseFeeItems` - 費用項目主檔
```sql
CREATE TABLE [dbo].[LeaseFeeItems] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [FeeItemId] NVARCHAR(50) NOT NULL, -- 費用項目編號 (FEE_ITEM_ID)
    [FeeItemName] NVARCHAR(200) NOT NULL, -- 費用項目名稱 (FEE_ITEM_NAME)
    [FeeType] NVARCHAR(20) NOT NULL, -- 費用類型 (FEE_TYPE)
    [DefaultAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 預設金額 (DEFAULT_AMOUNT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_LeaseFeeItems] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseFeeItems_FeeItemId] UNIQUE ([FeeItemId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseFeeItems_FeeItemId] ON [dbo].[LeaseFeeItems] ([FeeItemId]);
CREATE NONCLUSTERED INDEX [IX_LeaseFeeItems_FeeType] ON [dbo].[LeaseFeeItems] ([FeeType]);
```

### 2.3 資料字典

#### 2.3.1 LeaseFees 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| FeeId | NVARCHAR | 50 | NO | - | 費用編號 | 唯一，FEE_ID |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 外鍵至租賃表 |
| FeeType | NVARCHAR | 20 | NO | - | 費用類型 | RENT:租金, MANAGEMENT:管理費, UTILITY:水電費, OTHER:其他費用 |
| FeeItemId | NVARCHAR | 50 | YES | - | 費用項目編號 | 外鍵至費用項目表 |
| FeeItemName | NVARCHAR | 200 | YES | - | 費用項目名稱 | - |
| FeeAmount | DECIMAL | 18,4 | NO | 0 | 費用金額 | - |
| FeeDate | DATETIME2 | - | NO | - | 費用日期 | - |
| DueDate | DATETIME2 | - | YES | - | 到期日期 | - |
| PaidDate | DATETIME2 | - | YES | - | 繳費日期 | - |
| PaidAmount | DECIMAL | 18,4 | YES | 0 | 已繳金額 | - |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待繳, P:部分繳, F:已繳, C:已取消 |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |
| TaxRate | DECIMAL | 5,2 | YES | 0 | 稅率 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | 含稅 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢費用列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-fees`
- **說明**: 查詢費用列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "FeeId",
    "sortOrder": "ASC",
    "filters": {
      "feeId": "",
      "leaseId": "",
      "feeType": "",
      "status": "",
      "feeDateFrom": "",
      "feeDateTo": ""
    }
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆費用
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-fees/{feeId}`
- **說明**: 根據費用編號查詢單筆費用資料
- **路徑參數**:
  - `feeId`: 費用編號
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增費用
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-fees`
- **說明**: 新增費用資料
- **請求格式**: 標準新增格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改費用
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-fees/{feeId}`
- **說明**: 修改費用資料（僅待繳或部分繳狀態可修改）
- **路徑參數**:
  - `feeId`: 費用編號
- **請求格式**: 同新增，但 `feeId` 不可修改
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除費用
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-fees/{feeId}`
- **說明**: 刪除費用資料（僅待繳或已取消狀態可刪除）
- **路徑參數**:
  - `feeId`: 費用編號
- **回應格式**: 標準刪除回應格式

#### 3.1.6 費用試算
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-fees/calculate`
- **說明**: 計算費用金額（含稅額、總金額）
- **請求格式**:
  ```json
  {
    "feeAmount": 10000.00,
    "taxRate": 5.00,
    "currencyId": "TWD",
    "exchangeRate": 1.0
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "計算成功",
    "data": {
      "feeAmount": 10000.00,
      "taxRate": 5.00,
      "taxAmount": 500.00,
      "totalAmount": 10500.00
    }
  }
  ```

#### 3.1.7 租賃試算
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-fees/rent-calculate`
- **說明**: 計算租賃費用
- **請求格式**: 標準租賃試算格式
- **回應格式**: 標準試算回應格式

#### 3.1.8 扣款試算
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-fees/deduction-calculate`
- **說明**: 計算扣款費用
- **請求格式**: 標準扣款試算格式
- **回應格式**: 標準試算回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeaseFeesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lease-fees")]
    [Authorize]
    public class LeaseFeesController : ControllerBase
    {
        private readonly ILeaseFeeService _leaseFeeService;
        
        public LeaseFeesController(ILeaseFeeService leaseFeeService)
        {
            _leaseFeeService = leaseFeeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseFeeDto>>>> GetLeaseFees([FromQuery] LeaseFeeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{feeId}")]
        public async Task<ActionResult<ApiResponse<LeaseFeeDto>>> GetLeaseFee(string feeId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLeaseFee([FromBody] CreateLeaseFeeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{feeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLeaseFee(string feeId, [FromBody] UpdateLeaseFeeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{feeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLeaseFee(string feeId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("calculate")]
        public async Task<ActionResult<ApiResponse<FeeCalculationResult>>> CalculateFee([FromBody] FeeCalculationRequest request)
        {
            // 實作費用試算邏輯
        }
        
        [HttpPost("rent-calculate")]
        public async Task<ActionResult<ApiResponse<RentCalculationResult>>> CalculateRent([FromBody] RentCalculationRequest request)
        {
            // 實作租賃試算邏輯
        }
        
        [HttpPost("deduction-calculate")]
        public async Task<ActionResult<ApiResponse<DeductionCalculationResult>>> CalculateDeduction([FromBody] DeductionCalculationRequest request)
        {
            // 實作扣款試算邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 費用列表頁面 (`LeaseFeeList.vue`)
- **路徑**: `/lease/lease-fees`
- **功能**: 顯示費用列表，支援查詢、新增、修改、刪除、試算
- **主要元件**:
  - 查詢表單 (LeaseFeeSearchForm)
  - 資料表格 (LeaseFeeDataTable)
  - 新增/修改對話框 (LeaseFeeDialog)
  - 刪除確認對話框
  - 試算對話框

#### 4.1.2 費用詳細頁面 (`LeaseFeeDetail.vue`)
- **路徑**: `/lease/lease-fees/:feeId`
- **功能**: 顯示費用詳細資料，支援修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LeaseFeeSearchForm.vue`)
- 費用編號查詢
- 租賃編號查詢
- 費用類型查詢
- 狀態查詢
- 日期範圍查詢

#### 4.2.2 資料表格元件 (`LeaseFeeDataTable.vue`)
- 顯示費用編號、租賃編號、費用類型、費用項目、費用金額、狀態等欄位
- 支援操作按鈕（修改、刪除、試算）

#### 4.2.3 新增/修改對話框 (`LeaseFeeDialog.vue`)
- 基本資料表單
- 費用金額輸入
- 稅率輸入
- 自動計算稅額和總金額
- 表單驗證

#### 4.2.4 試算對話框 (`FeeCalculationDialog.vue`)
- 費用試算表單
- 租賃試算表單
- 扣款試算表單
- 試算結果顯示

### 4.3 API 呼叫 (`leaseFee.api.ts`)
- `getLeaseFeeList`: 查詢列表
- `getLeaseFeeById`: 查詢單筆
- `createLeaseFee`: 新增
- `updateLeaseFee`: 修改
- `deleteLeaseFee`: 刪除
- `calculateFee`: 費用試算
- `calculateRent`: 租賃試算
- `calculateDeduction`: 扣款試算

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
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
- [ ] 驗證邏輯實作
- [ ] 試算邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 試算對話框開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 試算功能測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 14天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 狀態變更必須記錄操作人員和時間

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 費用編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 費用金額必須大於0
- 稅率必須在0-100之間

### 6.4 業務邏輯
- 刪除費用前必須檢查狀態（僅待繳或已取消可刪除）
- 修改費用前必須檢查狀態（僅待繳或部分繳可修改）
- 租賃編號必須存在於租賃表中
- 費用項目編號必須存在於費用項目表中
- 稅額自動計算（費用金額×稅率）
- 總金額自動計算（費用金額+稅額）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增費用成功
- [ ] 新增費用失敗 (重複編號)
- [ ] 修改費用成功
- [ ] 修改費用失敗 (非待繳或部分繳狀態)
- [ ] 刪除費用成功
- [ ] 刪除費用失敗 (非待繳或已取消狀態)
- [ ] 查詢費用列表成功
- [ ] 查詢單筆費用成功
- [ ] 費用試算成功
- [ ] 租賃試算成功
- [ ] 扣款試算成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 試算功能測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE310_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE320_RENT.ASP` (租賃試算)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE320_FEES.ASP` (費用試算)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE330_RENT.ASP` (扣款試算)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE330_FEES.ASP` (費用試算)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE340_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE340_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE340_PR.ASP` (報表)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE350_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE350_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE405_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE430_FQ.ASP` (查詢)

### 8.2 相關功能
- `開發計劃/30-租賃管理SYSE/01-租賃基礎功能/SYSE110-SYSE140-租賃資料維護系列.md`
- `開發計劃/30-租賃管理SYSE/02-租賃擴展功能/SYSE210-SYSE230-租賃擴展維護系列.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

