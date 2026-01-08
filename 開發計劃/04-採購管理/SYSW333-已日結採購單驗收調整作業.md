# SYSW333 - 已日結採購單驗收調整作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW333
- **功能名稱**: 已日結採購單驗收調整作業
- **功能描述**: 提供已日結採購單的驗收調整功能，用於處理已日結的採購單驗收作業，包含驗收單新增、修改、刪除、查詢功能，支援已日結採購單的驗收數量、價格調整等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_FS.asp` (儲存)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW333_PR.asp` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW333_PR.rpt` (報表定義)

### 1.2 業務需求
- 處理已日結的採購單驗收作業
- 支援驗收單新增、修改、刪除、查詢
- 支援驗收數量、價格調整
- 支援驗收單狀態管理（草稿、已審核、已日結）
- 支援驗收單審核流程
- 支援多店別管理
- 支援驗收單報表列印
- 已日結採購單的特殊處理邏輯

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseReceipts` (採購驗收單主檔)

**說明**: 與 SYSW324 共用同一資料表 `PurchaseReceipts`，但需增加欄位標示為已日結調整

```sql
-- 參考 SYSW324 的 PurchaseReceipts 資料表結構
-- 可考慮增加欄位：
-- [IsSettledAdjustment] BIT NOT NULL DEFAULT 0, -- 是否為已日結調整
-- [OriginalReceiptId] NVARCHAR(50) NULL, -- 原始驗收單號（如有）
-- [AdjustmentReason] NVARCHAR(500) NULL, -- 調整原因
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseReceiptDetails` - 採購驗收單明細

**說明**: 與 SYSW324 共用同一資料表

```sql
-- 參考 SYSW324 的 PurchaseReceiptDetails 資料表結構
-- 可考慮增加欄位：
-- [OriginalReceiptQty] DECIMAL(18, 4) NULL, -- 原始驗收數量
-- [AdjustmentQty] DECIMAL(18, 4) NULL, -- 調整數量
-- [OriginalUnitPrice] DECIMAL(18, 4) NULL, -- 原始單價
-- [AdjustmentPrice] DECIMAL(18, 4) NULL, -- 調整單價
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ReceiptId | NVARCHAR | 50 | NO | - | 驗收單號 | 主鍵，唯一 |
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 外鍵至 PurchaseOrders |
| IsSettledAdjustment | BIT | - | NO | 0 | 是否為已日結調整 | - |
| OriginalReceiptId | NVARCHAR | 50 | YES | - | 原始驗收單號 | - |
| AdjustmentReason | NVARCHAR | 500 | YES | - | 調整原因 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢已日結採購單驗收調整列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments`
- **說明**: 查詢已日結採購單驗收調整列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ReceiptDate",
    "sortOrder": "DESC",
    "filters": {
      "receiptId": "",
      "orderId": "",
      "shopId": "",
      "supplierId": "",
      "receiptDateFrom": "",
      "receiptDateTo": "",
      "status": ""
    }
  }
  ```

#### 3.1.2 查詢單筆已日結採購單驗收調整
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments/{receiptId}`
- **說明**: 根據驗收單號查詢單筆已日結採購單驗收調整資料（含明細）

#### 3.1.3 查詢可用已日結採購單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/settled-orders`
- **說明**: 查詢可用於調整的已日結採購單列表
- **請求參數**:
  ```json
  {
    "shopId": "",
    "supplierId": "",
    "orderDateFrom": "",
    "orderDateTo": ""
  }
  ```

#### 3.1.4 新增已日結採購單驗收調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments`
- **說明**: 新增已日結採購單驗收調整（含明細）
- **請求格式**:
  ```json
  {
    "orderId": "PO001",
    "receiptDate": "2024-01-01",
    "shopId": "SHOP001",
    "supplierId": "SUP001",
    "adjustmentReason": "數量調整",
    "details": [
      {
        "lineNum": 1,
        "goodsId": "GOODS001",
        "barcodeId": "BC001",
        "orderQty": 100,
        "originalReceiptQty": 100,
        "receiptQty": 110,
        "adjustmentQty": 10,
        "unitPrice": 100,
        "originalUnitPrice": 100,
        "adjustmentPrice": 0,
        "memo": ""
      }
    ]
  }
  ```

#### 3.1.5 修改已日結採購單驗收調整
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments/{receiptId}`
- **說明**: 修改已日結採購單驗收調整（僅未審核狀態可修改）

#### 3.1.6 刪除已日結採購單驗收調整
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments/{receiptId}`
- **說明**: 刪除已日結採購單驗收調整（僅未審核狀態可刪除）

#### 3.1.7 審核已日結採購單驗收調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/settled-adjustments/{receiptId}/approve`
- **說明**: 審核已日結採購單驗收調整
- **請求格式**:
  ```json
  {
    "approveUserId": "USER001",
    "approveDate": "2024-01-01",
    "notes": "審核通過"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseReceiptsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-receipts")]
    [Authorize]
    public class PurchaseReceiptsController : ControllerBase
    {
        private readonly IPurchaseReceiptService _purchaseReceiptService;
        
        [HttpGet("settled-adjustments")]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetSettledAdjustments([FromQuery] PurchaseReceiptQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("settled-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse<PurchaseReceiptDto>>> GetSettledAdjustment(string receiptId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("settled-orders")]
        public async Task<ActionResult<ApiResponse<List<PurchaseOrderDto>>>> GetSettledOrders([FromQuery] SettledOrderQueryDto query)
        {
            // 實作查詢可用已日結採購單邏輯
        }
        
        [HttpPost("settled-adjustments")]
        public async Task<ActionResult<ApiResponse<string>>> CreateSettledAdjustment([FromBody] CreatePurchaseReceiptDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("settled-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse>> UpdateSettledAdjustment(string receiptId, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("settled-adjustments/{receiptId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSettledAdjustment(string receiptId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("settled-adjustments/{receiptId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveSettledAdjustment(string receiptId, [FromBody] ApprovePurchaseReceiptDto dto)
        {
            // 實作審核邏輯
        }
    }
}
```

#### 3.2.2 Service: `PurchaseReceiptService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPurchaseReceiptService
    {
        Task<PagedResult<PurchaseReceiptDto>> GetSettledAdjustmentsAsync(PurchaseReceiptQueryDto query);
        Task<PurchaseReceiptDto> GetSettledAdjustmentByIdAsync(string receiptId);
        Task<string> CreateSettledAdjustmentAsync(CreatePurchaseReceiptDto dto);
        Task UpdateSettledAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto);
        Task DeleteSettledAdjustmentAsync(string receiptId);
        Task ApproveSettledAdjustmentAsync(string receiptId, ApprovePurchaseReceiptDto dto);
        Task<List<PurchaseOrderDto>> GetSettledOrdersAsync(SettledOrderQueryDto query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 已日結採購單驗收調整列表頁面 (`SettledPurchaseReceiptAdjustmentList.vue`)
- **路徑**: `/purchase/settled-receipt-adjustments`
- **功能**: 顯示已日結採購單驗收調整列表，支援查詢、新增、修改、刪除、審核

#### 4.1.2 已日結採購單驗收調整詳細頁面 (`SettledPurchaseReceiptAdjustmentDetail.vue`)
- **路徑**: `/purchase/settled-receipt-adjustments/:receiptId`
- **功能**: 顯示已日結採購單驗收調整詳細資料（含明細），支援修改、審核

#### 4.1.3 已日結採購單驗收調整新增/修改頁面 (`SettledPurchaseReceiptAdjustmentForm.vue`)
- **路徑**: `/purchase/settled-receipt-adjustments/new` 或 `/purchase/settled-receipt-adjustments/:receiptId/edit`
- **功能**: 新增或修改已日結採購單驗收調整

### 4.2 UI 元件設計

#### 4.2.1 已日結採購單驗收調整列表元件 (`SettledPurchaseReceiptAdjustmentDataTable.vue`)
- 顯示已日結採購單驗收調整列表
- 支援查詢、新增、修改、刪除、審核

#### 4.2.2 已日結採購單驗收調整新增/修改對話框 (`SettledPurchaseReceiptAdjustmentDialog.vue`)
- 支援新增已日結採購單驗收調整
- 支援修改已日結採購單驗收調整
- 支援驗收數量、價格調整
- 顯示原始驗收數量、調整數量、調整後數量
- 顯示原始單價、調整單價、調整後單價

#### 4.2.3 已日結採購單選擇元件 (`SettledPurchaseOrderSelector.vue`)
- 選擇可用於調整的已日結採購單
- 顯示採購單基本資訊

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 確認資料表結構（與 SYSW324 共用）
- [ ] 新增已日結調整相關欄位
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立（擴充現有類別）
- [ ] Repository 實作（擴充現有 Repository）
- [ ] Service 實作（已日結調整邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 已日結調整業務邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 已日結採購單驗收調整列表頁面開發
- [ ] 已日結採購單驗收調整新增/修改頁面開發
- [ ] 已日結採購單選擇元件開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 已日結調整業務邏輯測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 13天

---

## 六、注意事項

### 6.1 已日結處理
- 僅已日結的採購單可進行驗收調整
- 已日結調整需經過審核流程
- 已審核的調整單不可修改、刪除

### 6.2 調整邏輯
- 需記錄原始驗收數量、調整數量
- 需記錄原始單價、調整單價
- 調整原因必須填寫

### 6.3 資料驗證
- 調整數量必須合理（不可為負數或過大）
- 調整單價必須合理
- 調整原因必須填寫

### 6.4 業務邏輯
- 僅未審核狀態可修改、刪除
- 審核後不可修改、刪除
- 審核時需記錄審核人員、審核日期

### 6.5 與 SYSW324 的差異
- SYSW333 專門處理已日結的採購單驗收調整
- 需要額外的審核流程
- 需要記錄調整原因

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增已日結採購單驗收調整成功
- [ ] 新增已日結採購單驗收調整失敗 (採購單未日結)
- [ ] 修改已日結採購單驗收調整成功
- [ ] 修改已日結採購單驗收調整失敗 (已審核)
- [ ] 刪除已日結採購單驗收調整成功
- [ ] 刪除已日結採購單驗收調整失敗 (已審核)
- [ ] 審核已日結採購單驗收調整成功

### 7.2 整合測試
- [ ] 完整已日結採購單驗收調整流程測試
- [ ] 已日結檢查測試
- [ ] 審核流程測試
- [ ] 並發操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發調整測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW333_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW333_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW324-採購單驗收作業.md`
- `開發計劃/04-採購管理/SYSW530-已日結退貨單驗退調整作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

