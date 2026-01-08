# SYSW324 - 採購單驗收作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW324
- **功能名稱**: 採購單驗收作業
- **功能描述**: 提供採購單驗收作業的新增、修改、刪除、查詢功能，用於管理採購單的驗收流程，包含驗收數量、驗收日期、驗收人員、驗收狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW324_*.asp` (相關功能頁面)
  - `WEB/IMS_CORE/SYSW000/SYSW324_PR.rpt` (報表，如有)

### 1.2 業務需求
- 管理採購單驗收作業
- 支援依採購單號查詢待驗收單據
- 支援驗收數量維護（可部分驗收）
- 支援驗收日期、驗收人員記錄
- 支援驗收狀態管理（待驗收、部分驗收、已驗收）
- 支援驗收單據列印
- 支援驗收後自動更新庫存
- 支援驗收後自動更新採購單已收數量

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseReceipts` (採購驗收單主檔)

```sql
CREATE TABLE [dbo].[PurchaseReceipts] (
    [ReceiptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗收單號
    [OrderId] NVARCHAR(50) NOT NULL, -- 採購單號 (關聯至 PurchaseOrders)
    [ReceiptDate] DATETIME2 NOT NULL, -- 驗收日期
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商代碼
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待驗收, R:部分驗收, C:已驗收, X:已取消)
    [ReceiptUserId] NVARCHAR(50) NULL, -- 驗收人員
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
    [Memo] NVARCHAR(500) NULL, -- 備註
    [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
    [SettledDate] DATETIME2 NULL, -- 日結日期
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseReceipts] PRIMARY KEY CLUSTERED ([ReceiptId] ASC),
    CONSTRAINT [FK_PurchaseReceipts_PurchaseOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[PurchaseOrders] ([OrderId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_OrderId] ON [dbo].[PurchaseReceipts] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ShopId] ON [dbo].[PurchaseReceipts] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_Status] ON [dbo].[PurchaseReceipts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_ReceiptDate] ON [dbo].[PurchaseReceipts] ([ReceiptDate]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceipts_IsSettled] ON [dbo].[PurchaseReceipts] ([IsSettled]);
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseReceiptDetails` - 採購驗收單明細
```sql
CREATE TABLE [dbo].[PurchaseReceiptDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ReceiptId] NVARCHAR(50) NOT NULL, -- 驗收單號
    [OrderDetailId] UNIQUEIDENTIFIER NULL, -- 採購單明細ID (關聯至 PurchaseOrderDetails)
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量
    [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
    [Amount] DECIMAL(18, 4) NULL, -- 金額
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PurchaseReceiptDetails_PurchaseReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[PurchaseReceipts] ([ReceiptId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PurchaseReceiptDetails_PurchaseOrderDetails] FOREIGN KEY ([OrderDetailId]) REFERENCES [dbo].[PurchaseOrderDetails] ([DetailId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_ReceiptId] ON [dbo].[PurchaseReceiptDetails] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseReceiptDetails_GoodsId] ON [dbo].[PurchaseReceiptDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ReceiptId | NVARCHAR | 50 | NO | - | 驗收單號 | 主鍵，唯一 |
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 外鍵至 PurchaseOrders |
| ReceiptDate | DATETIME2 | - | NO | - | 驗收日期 | - |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| SupplierId | NVARCHAR | 50 | NO | - | 供應商代碼 | 外鍵至供應商表 |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待驗收, R:部分驗收, C:已驗收, X:已取消 |
| ReceiptUserId | NVARCHAR | 50 | YES | - | 驗收人員 | 外鍵至使用者表 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| IsSettled | BIT | - | NO | 0 | 是否已日結 | - |
| SettledDate | DATETIME2 | - | YES | - | 日結日期 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢待驗收採購單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/pending-orders`
- **說明**: 查詢待驗收的採購單列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "orderId": "",
      "shopId": "",
      "supplierId": "",
      "orderDateFrom": "",
      "orderDateTo": ""
    }
  }
  ```

#### 3.1.2 查詢驗收單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts`
- **說明**: 查詢驗收單列表，支援分頁、排序、篩選
- **請求參數**: 參考其他查詢 API

#### 3.1.3 查詢單筆驗收單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/{receiptId}`
- **說明**: 根據驗收單號查詢單筆驗收單資料（含明細）

#### 3.1.4 依採購單號建立驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/from-order/{orderId}`
- **說明**: 依採購單號建立驗收單（帶入採購單明細）
- **回應格式**: 返回建立的驗收單資料

#### 3.1.5 新增驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts`
- **說明**: 新增驗收單（含明細）

#### 3.1.6 修改驗收單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-receipts/{receiptId}`
- **說明**: 修改驗收單（僅未日結且未取消狀態可修改）

#### 3.1.7 刪除驗收單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-receipts/{receiptId}`
- **說明**: 刪除驗收單（僅未日結且未取消狀態可刪除）

#### 3.1.8 確認驗收
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/{receiptId}/confirm`
- **說明**: 確認驗收，更新庫存及採購單已收數量

#### 3.1.9 取消驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/{receiptId}/cancel`
- **說明**: 取消驗收單

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
        
        [HttpGet("pending-orders")]
        public async Task<ActionResult<ApiResponse<PagedResult<PendingPurchaseOrderDto>>>> GetPendingOrders([FromQuery] PendingOrderQueryDto query)
        {
            // 實作查詢待驗收採購單邏輯
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseReceiptDto>>>> GetPurchaseReceipts([FromQuery] PurchaseReceiptQueryDto query)
        {
            // 實作查詢驗收單列表邏輯
        }
        
        [HttpGet("{receiptId}")]
        public async Task<ActionResult<ApiResponse<PurchaseReceiptDetailDto>>> GetPurchaseReceipt(string receiptId)
        {
            // 實作查詢單筆驗收單邏輯
        }
        
        [HttpPost("from-order/{orderId}")]
        public async Task<ActionResult<ApiResponse<PurchaseReceiptDetailDto>>> CreateReceiptFromOrder(string orderId)
        {
            // 實作依採購單建立驗收單邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreatePurchaseReceipt([FromBody] CreatePurchaseReceiptDto dto)
        {
            // 實作新增驗收單邏輯
        }
        
        [HttpPut("{receiptId}")]
        public async Task<ActionResult<ApiResponse>> UpdatePurchaseReceipt(string receiptId, [FromBody] UpdatePurchaseReceiptDto dto)
        {
            // 實作修改驗收單邏輯
        }
        
        [HttpDelete("{receiptId}")]
        public async Task<ActionResult<ApiResponse>> DeletePurchaseReceipt(string receiptId)
        {
            // 實作刪除驗收單邏輯
        }
        
        [HttpPost("{receiptId}/confirm")]
        public async Task<ActionResult<ApiResponse>> ConfirmReceipt(string receiptId)
        {
            // 實作確認驗收邏輯（更新庫存、更新採購單已收數量）
        }
        
        [HttpPost("{receiptId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelPurchaseReceipt(string receiptId)
        {
            // 實作取消驗收單邏輯
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
        Task<PagedResult<PendingPurchaseOrderDto>> GetPendingOrdersAsync(PendingOrderQueryDto query);
        Task<PagedResult<PurchaseReceiptDto>> GetPurchaseReceiptsAsync(PurchaseReceiptQueryDto query);
        Task<PurchaseReceiptDetailDto> GetPurchaseReceiptByIdAsync(string receiptId);
        Task<PurchaseReceiptDetailDto> CreateReceiptFromOrderAsync(string orderId);
        Task<string> CreatePurchaseReceiptAsync(CreatePurchaseReceiptDto dto);
        Task UpdatePurchaseReceiptAsync(string receiptId, UpdatePurchaseReceiptDto dto);
        Task DeletePurchaseReceiptAsync(string receiptId);
        Task ConfirmReceiptAsync(string receiptId);
        Task CancelPurchaseReceiptAsync(string receiptId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 待驗收採購單列表頁面 (`PendingPurchaseOrderList.vue`)
- **路徑**: `/procurement/pending-receipts`
- **功能**: 顯示待驗收的採購單列表，支援查詢、建立驗收單

#### 4.1.2 驗收單列表頁面 (`PurchaseReceiptList.vue`)
- **路徑**: `/procurement/purchase-receipts`
- **功能**: 顯示驗收單列表，支援查詢、新增、修改、刪除、確認驗收、取消

#### 4.1.3 驗收單詳細頁面 (`PurchaseReceiptDetail.vue`)
- **路徑**: `/procurement/purchase-receipts/:receiptId`
- **功能**: 顯示驗收單詳細資料（含明細），支援修改、確認驗收、取消

### 4.2 UI 元件設計

#### 4.2.1 待驗收採購單列表元件 (`PendingPurchaseOrderList.vue`)
- 顯示待驗收的採購單
- 支援依採購單建立驗收單

#### 4.2.2 驗收單列表元件 (`PurchaseReceiptDataTable.vue`)
- 顯示驗收單列表
- 支援查詢、新增、修改、刪除、確認驗收、取消

#### 4.2.3 驗收單新增/修改對話框 (`PurchaseReceiptDialog.vue`)
- 支援新增驗收單
- 支援修改驗收單
- 支援驗收數量維護
- 顯示訂購數量、已收數量、本次驗收數量

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含庫存更新邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 庫存更新邏輯實作
- [ ] 採購單已收數量更新邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 待驗收採購單列表頁面開發
- [ ] 驗收單列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 驗收數量維護功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 庫存更新測試
- [ ] 採購單已收數量更新測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 13天

---

## 六、注意事項

### 6.1 庫存更新
- 確認驗收時必須更新庫存
- 必須處理庫存異動記錄
- 必須處理庫存鎖定機制（避免並發問題）

### 6.2 採購單已收數量更新
- 確認驗收時必須更新採購單明細的已收數量
- 必須檢查驗收數量是否超過訂購數量
- 必須更新採購單狀態（全部驗收完成時）

### 6.3 日結處理
- 已日結的驗收單不可修改、刪除
- 日結時必須檢查驗收單狀態

### 6.4 資料驗證
- 驗收數量必須大於0
- 驗收數量不可超過訂購數量減已收數量
- 驗收日期必須大於等於採購單日期

### 6.5 業務邏輯
- 僅未日結且未取消狀態可修改、刪除
- 確認驗收後不可修改、刪除（除非取消）
- 取消驗收時必須回退庫存及採購單已收數量

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增驗收單成功
- [ ] 新增驗收單失敗 (驗收數量超過訂購數量)
- [ ] 修改驗收單成功
- [ ] 修改驗收單失敗 (已日結)
- [ ] 刪除驗收單成功
- [ ] 刪除驗收單失敗 (已日結)
- [ ] 確認驗收成功（庫存更新、採購單已收數量更新）
- [ ] 取消驗收成功（庫存回退、採購單已收數量回退）

### 7.2 整合測試
- [ ] 完整驗收流程測試
- [ ] 庫存更新測試
- [ ] 採購單已收數量更新測試
- [ ] 日結處理測試
- [ ] 並發操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發驗收測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW324_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW324_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW315-訂退貨申請作業.md`
- `開發計劃/04-採購管理/SYSW333-已日結採購單驗收調整作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

