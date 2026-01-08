# SYSW53M - 盤點維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW53M
- **功能名稱**: 盤點維護作業
- **功能描述**: 提供盤點單的維護功能，包含盤點計劃建立、盤點資料上傳、盤點差異計算、盤點結果確認等，支援盤點報表查詢與列印
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSW000/SYSW53M_PR.aspx` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW53M_PR.rpt` (報表定義)
  - `IMS3/HANSHIN/IMS3/SYSW000/SYSW53D.ascx.cs` (盤點相關功能)

### 1.2 業務需求
- 管理盤點計劃
- 支援盤點資料上傳（HT上傳）
- 支援盤點差異計算
- 支援盤點結果確認
- 支援盤點狀態管理（計劃、確認、盤點中、計算、帳面庫存、結案、認列完成）
- 支援多店別盤點
- 支援盤點報表查詢與列印
- 支援盤點資料匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `StocktakingPlans` (盤點計劃主檔)

```sql
CREATE TABLE [dbo].[StocktakingPlans] (
    [PlanId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 盤點計劃單號 (PLAN_ID)
    [PlanDate] DATETIME2 NOT NULL, -- 盤點日期 (CHECK_DATE)
    [StartDate] DATETIME2 NULL, -- 開始日期 (B_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (E_DATE)
    [StartTime] DATETIME2 NULL, -- 開始時間 (B_TIME)
    [EndTime] DATETIME2 NULL, -- 結束時間 (E_TIME)
    [SakeType] NVARCHAR(50) NULL, -- 盤點類型 (SAKE_TYPE)
    [SakeDept] NVARCHAR(50) NULL, -- 盤點部門 (SAKE_DEPT)
    [PlanStatus] NVARCHAR(10) NOT NULL DEFAULT '0', -- 計劃狀態 (PLAN_STATUS, -1:申請中, 0:未審核, 1:已審核, 4:作廢, 5:結案)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_StocktakingPlans] PRIMARY KEY CLUSTERED ([PlanId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanDate] ON [dbo].[StocktakingPlans] ([PlanDate]);
CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_PlanStatus] ON [dbo].[StocktakingPlans] ([PlanStatus]);
CREATE NONCLUSTERED INDEX [IX_StocktakingPlans_SiteId] ON [dbo].[StocktakingPlans] ([SiteId]);
```

### 2.2 相關資料表

#### 2.2.1 `StocktakingPlanShops` - 盤點計劃店舖檔
```sql
CREATE TABLE [dbo].[StocktakingPlanShops] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號 (PLAN_ID)
    [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼 (SHOP_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '0', -- 狀態 (STATUS, 0:計劃, 1:確認, 2:盤點中, 3:計算, 4:帳面庫存, 5:作廢, 6:結案, 7:認列完成)
    [InvStatus] NVARCHAR(10) NULL, -- 盤點狀態 (INV_STATUS)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_StocktakingPlanShops_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StocktakingPlanShops_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_PlanId] ON [dbo].[StocktakingPlanShops] ([PlanId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_ShopId] ON [dbo].[StocktakingPlanShops] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingPlanShops_Status] ON [dbo].[StocktakingPlanShops] ([Status]);
```

#### 2.2.2 `StocktakingTemp` - 店舖盤點記錄品暫存檔
```sql
CREATE TABLE [dbo].[StocktakingTemp] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號 (PLAN_ID)
    [SPlanId] NVARCHAR(50) NULL, -- 子盤點單號 (SPLAN_ID)
    [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼 (SHOP_ID)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (GOODS_ID)
    [Kind] NVARCHAR(50) NULL, -- 盤點區域 (KIND)
    [ShelfNo] NVARCHAR(50) NULL, -- 盤點貨架 (SHELF_NO)
    [SerialNo] INT NULL, -- 盤點貨架序號 (SERIAL_NO)
    [Qty] DECIMAL(18, 4) NULL DEFAULT 0, -- HT上傳量 (QTY)
    [IQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 人工量 (IQTY)
    [IsAdd] NVARCHAR(1) NULL DEFAULT 'N', -- 是否新增 (IS_ADD, Y/N)
    [HtStatus] NVARCHAR(10) NOT NULL DEFAULT '0', -- HT狀態 (HT_STATUS, -1:申請中, 0:未審核, 1:已審核, 4:作廢)
    [Status] NVARCHAR(10) NULL, -- 狀態 (STATUS)
    [BUser] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [BTime] DATETIME2 NULL, -- 建立時間 (BTIME)
    [ApprvId] NVARCHAR(50) NULL, -- 審核者 (APRV_ID)
    [ApprvDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
    [InvDate] DATETIME2 NULL, -- 盤點日期 (INV_DATE)
    [IsUpdate] NVARCHAR(1) NULL DEFAULT 'N', -- 是否更新 (IS_UPDATE, Y/N)
    [NumNo] NVARCHAR(50) NULL, -- 序號 (NUM_NO)
    [HtAuto] NVARCHAR(1) NULL DEFAULT 'N', -- HT自動 (HT_AUTO, Y/N)
    [IsSuccess] NVARCHAR(1) NULL DEFAULT 'N', -- 是否成功 (IS_SUCCESS, Y/N)
    [ErrMsg] NVARCHAR(500) NULL, -- 錯誤訊息 (ERR_MSG)
    [IsHt] NVARCHAR(1) NULL DEFAULT 'N', -- 是否HT (IS_HT, Y/N)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_StocktakingTemp_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StocktakingTemp_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_PlanId] ON [dbo].[StocktakingTemp] ([PlanId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_ShopId] ON [dbo].[StocktakingTemp] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_GoodsId] ON [dbo].[StocktakingTemp] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingTemp_HtStatus] ON [dbo].[StocktakingTemp] ([HtStatus]);
```

#### 2.2.3 `StocktakingDetails` - 盤點單明細
```sql
CREATE TABLE [dbo].[StocktakingDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [PlanId] NVARCHAR(50) NOT NULL, -- 盤點計劃單號
    [ShopId] NVARCHAR(50) NOT NULL, -- 店舖代碼
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BookQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 帳面數量
    [PhysicalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 實盤數量
    [DiffQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 差異數量
    [UnitCost] DECIMAL(18, 4) NULL, -- 單位成本
    [DiffAmount] DECIMAL(18, 4) NULL, -- 差異金額
    [Kind] NVARCHAR(50) NULL, -- 盤點區域
    [ShelfNo] NVARCHAR(50) NULL, -- 盤點貨架
    [SerialNo] INT NULL, -- 盤點貨架序號
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_StocktakingDetails_StocktakingPlans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[StocktakingPlans] ([PlanId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StocktakingDetails_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_PlanId] ON [dbo].[StocktakingDetails] ([PlanId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_ShopId] ON [dbo].[StocktakingDetails] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_StocktakingDetails_GoodsId] ON [dbo].[StocktakingDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PlanId | NVARCHAR | 50 | NO | - | 盤點計劃單號 | 主鍵，PLAN_ID |
| PlanDate | DATETIME2 | - | NO | - | 盤點日期 | CHECK_DATE |
| StartDate | DATETIME2 | - | YES | - | 開始日期 | B_DATE |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | E_DATE |
| StartTime | DATETIME2 | - | YES | - | 開始時間 | B_TIME |
| EndTime | DATETIME2 | - | YES | - | 結束時間 | E_TIME |
| SakeType | NVARCHAR | 50 | YES | - | 盤點類型 | SAKE_TYPE |
| SakeDept | NVARCHAR | 50 | YES | - | 盤點部門 | SAKE_DEPT |
| PlanStatus | NVARCHAR | 10 | NO | '0' | 計劃狀態 | -1:申請中, 0:未審核, 1:已審核, 4:作廢, 5:結案 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢盤點計劃列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/stocktaking-plans`
- **說明**: 查詢盤點計劃列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PlanDate",
    "sortOrder": "DESC",
    "filters": {
      "planId": "",
      "planDateFrom": "",
      "planDateTo": "",
      "planStatus": "",
      "shopId": "",
      "sakeType": ""
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
          "planId": "PLAN001",
          "planDate": "2024-01-01",
          "startDate": "2024-01-01",
          "endDate": "2024-01-03",
          "startTime": "2024-01-01T08:00:00",
          "endTime": "2024-01-03T18:00:00",
          "sakeType": "FULL",
          "sakeDept": "DEPT001",
          "planStatus": "1",
          "planStatusName": "已審核",
          "shopCount": 5,
          "totalDiffQty": 100,
          "totalDiffAmount": 10000.00
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

#### 3.1.2 查詢單筆盤點計劃
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/stocktaking-plans/{planId}`
- **說明**: 根據盤點計劃單號查詢單筆盤點計劃資料（含店舖、明細）
- **路徑參數**:
  - `planId`: 盤點計劃單號
- **回應格式**: 包含計劃主檔、店舖列表、明細列表

#### 3.1.3 新增盤點計劃
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/stocktaking-plans`
- **說明**: 新增盤點計劃（含店舖）
- **請求格式**:
  ```json
  {
    "planDate": "2024-01-01",
    "startDate": "2024-01-01",
    "endDate": "2024-01-03",
    "startTime": "2024-01-01T08:00:00",
    "endTime": "2024-01-03T18:00:00",
    "sakeType": "FULL",
    "sakeDept": "DEPT001",
    "shopIds": ["SHOP001", "SHOP002", "SHOP003"]
  }
  ```

#### 3.1.4 修改盤點計劃
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/stocktaking-plans/{planId}`
- **說明**: 修改盤點計劃（僅未審核狀態可修改）

#### 3.1.5 刪除盤點計劃
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/stocktaking-plans/{planId}`
- **說明**: 刪除盤點計劃（僅未審核狀態可刪除）

#### 3.1.6 審核盤點計劃
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/stocktaking-plans/{planId}/approve`
- **說明**: 審核盤點計劃

#### 3.1.7 上傳盤點資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/stocktaking-plans/{planId}/upload`
- **說明**: 上傳盤點資料（HT上傳）
- **請求格式**: 支援檔案上傳或JSON格式

#### 3.1.8 計算盤點差異
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/stocktaking-plans/{planId}/calculate`
- **說明**: 計算盤點差異（帳面數量 vs 實盤數量）

#### 3.1.9 確認盤點結果
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/stocktaking-plans/{planId}/confirm`
- **說明**: 確認盤點結果，產生庫存調整單

#### 3.1.10 查詢盤點報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/stocktaking-plans/{planId}/report`
- **說明**: 查詢盤點報表資料
- **請求參數**:
  ```json
  {
    "reportType": "SUMMARY", // SUMMARY:彙總, DETAIL:明細
    "shopId": "",
    "goodsId": "",
    "includeZero": false
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `StocktakingPlansController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stocktaking-plans")]
    [Authorize]
    public class StocktakingPlansController : ControllerBase
    {
        private readonly IStocktakingPlanService _stocktakingPlanService;
        
        public StocktakingPlansController(IStocktakingPlanService stocktakingPlanService)
        {
            _stocktakingPlanService = stocktakingPlanService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StocktakingPlanDto>>>> GetStocktakingPlans([FromQuery] StocktakingPlanQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{planId}")]
        public async Task<ActionResult<ApiResponse<StocktakingPlanDto>>> GetStocktakingPlan(string planId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateStocktakingPlan([FromBody] CreateStocktakingPlanDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{planId}")]
        public async Task<ActionResult<ApiResponse>> UpdateStocktakingPlan(string planId, [FromBody] UpdateStocktakingPlanDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{planId}")]
        public async Task<ActionResult<ApiResponse>> DeleteStocktakingPlan(string planId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{planId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveStocktakingPlan(string planId)
        {
            // 實作審核邏輯
        }
        
        [HttpPost("{planId}/upload")]
        public async Task<ActionResult<ApiResponse>> UploadStocktakingData(string planId, [FromForm] IFormFile file)
        {
            // 實作上傳邏輯
        }
        
        [HttpPost("{planId}/calculate")]
        public async Task<ActionResult<ApiResponse>> CalculateStocktakingDiff(string planId)
        {
            // 實作計算差異邏輯
        }
        
        [HttpPost("{planId}/confirm")]
        public async Task<ActionResult<ApiResponse<string>>> ConfirmStocktakingResult(string planId)
        {
            // 實作確認邏輯，產生庫存調整單
        }
        
        [HttpGet("{planId}/report")]
        public async Task<ActionResult<ApiResponse<StocktakingReportDto>>> GetStocktakingReport(string planId, [FromQuery] StocktakingReportQueryDto query)
        {
            // 實作報表查詢邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 盤點計劃列表頁面 (`StocktakingPlanList.vue`)
- **路徑**: `/stocktaking/plans`
- **功能**: 顯示盤點計劃列表，支援查詢、新增、修改、刪除、審核

#### 4.1.2 盤點計劃詳細頁面 (`StocktakingPlanDetail.vue`)
- **路徑**: `/stocktaking/plans/:planId`
- **功能**: 顯示盤點計劃詳細資料，支援修改、上傳、計算、確認

#### 4.1.3 盤點報表頁面 (`StocktakingReport.vue`)
- **路徑**: `/stocktaking/plans/:planId/report`
- **功能**: 顯示盤點報表，支援查詢、列印、匯出

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`StocktakingPlanSearchForm.vue`)
- 支援盤點計劃單號、盤點日期範圍、狀態、店舖等篩選條件

#### 4.2.2 資料表格元件 (`StocktakingPlanDataTable.vue`)
- 顯示盤點計劃列表，包含計劃單號、盤點日期、狀態、店舖數量、差異數量、差異金額等

#### 4.2.3 新增/修改對話框 (`StocktakingPlanDialog.vue`)
- 支援盤點計劃基本資料維護、店舖選擇

#### 4.2.4 盤點資料上傳元件 (`StocktakingUpload.vue`)
- 支援檔案上傳或手動輸入盤點資料

#### 4.2.5 盤點差異計算元件 (`StocktakingCalculate.vue`)
- 顯示盤點差異計算結果

#### 4.2.6 盤點報表元件 (`StocktakingReportTable.vue`)
- 顯示盤點報表資料，支援彙總與明細切換

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
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
- [ ] 盤點差異計算邏輯
- [ ] 檔案上傳處理
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 盤點資料上傳元件開發
- [ ] 盤點差異計算元件開發
- [ ] 盤點報表元件開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 檔案上傳測試
- [ ] 差異計算測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 業務邏輯
- 盤點計劃必須先審核才能進行盤點作業
- 盤點資料上傳後需要審核才能計算差異
- 差異計算需要比較帳面數量與實盤數量
- 確認盤點結果後會自動產生庫存調整單
- 盤點狀態流程：計劃 → 確認 → 盤點中 → 計算 → 帳面庫存 → 結案 → 認列完成

### 6.2 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 操作記錄必須記錄

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 盤點差異計算需要優化效能

### 6.4 資料驗證
- 盤點計劃單號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內
- 盤點數量必須大於等於0

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增盤點計劃成功
- [ ] 修改盤點計劃成功
- [ ] 刪除盤點計劃成功
- [ ] 審核盤點計劃成功
- [ ] 上傳盤點資料成功
- [ ] 計算盤點差異成功
- [ ] 確認盤點結果成功

### 7.2 整合測試
- [ ] 完整盤點流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 檔案上傳測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 盤點差異計算效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSW000/SYSW53M_PR.aspx` (報表)
- `WEB/IMS_CORE/SYSW000/SYSW53M_PR.rpt` (報表定義)
- `IMS3/HANSHIN/IMS3/SYSW000/SYSW53D.ascx.cs` (盤點相關功能)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW57M_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW57M_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW520_FD.ASP` (刪除)

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSW000/SYSW5B0_PR.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

