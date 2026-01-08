# SYSW362 - 調撥單驗退作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW362
- **功能名稱**: 調撥單驗退作業
- **功能描述**: 提供調撥單驗退作業的新增、修改、刪除、查詢功能，用於管理調撥單的驗退流程，當調撥商品有問題或需要退回時，可透過此功能進行驗退作業，包含驗退數量、驗退日期、驗退人員、驗退狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW362_*.asp` (相關功能頁面)
  - `WEB/IMS_CORE/SYSW000/SYSW362_PR.rpt` (報表，如有)

### 1.2 業務需求
- 管理調撥單驗退作業
- 支援依調撥單號查詢待驗退單據
- 支援驗退數量維護（可部分驗退）
- 支援驗退日期、驗退人員記錄
- 支援驗退狀態管理（待驗退、部分驗退、已驗退）
- 支援驗退單據列印
- 支援驗退後自動更新庫存（調入庫減少、調出庫增加）
- 支援驗退後自動更新調撥單已退數量

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TransferReturns` (調撥驗退單主檔)

```sql
CREATE TABLE [dbo].[TransferReturns] (
    [ReturnId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗退單號
    [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
    [ReceiptId] NVARCHAR(50) NULL, -- 原驗收單號 (關聯至 TransferReceipts，如有)
    [ReturnDate] DATETIME2 NOT NULL, -- 驗退日期
    [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
    [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待驗退, R:部分驗退, C:已驗退, X:已取消)
    [ReturnUserId] NVARCHAR(50) NULL, -- 驗退人員
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
    [ReturnReason] NVARCHAR(500) NULL, -- 驗退原因
    [Memo] NVARCHAR(500) NULL, -- 備註
    [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
    [SettledDate] DATETIME2 NULL, -- 日結日期
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TransferReturns] PRIMARY KEY CLUSTERED ([ReturnId] ASC),
    CONSTRAINT [FK_TransferReturns_TransferOrders] FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId]),
    CONSTRAINT [FK_TransferReturns_TransferReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[TransferReceipts] ([ReceiptId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferReturns_TransferId] ON [dbo].[TransferReturns] ([TransferId]);
CREATE NONCLUSTERED INDEX [IX_TransferReturns_ReceiptId] ON [dbo].[TransferReturns] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_TransferReturns_FromShopId] ON [dbo].[TransferReturns] ([FromShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferReturns_ToShopId] ON [dbo].[TransferReturns] ([ToShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferReturns_Status] ON [dbo].[TransferReturns] ([Status]);
CREATE NONCLUSTERED INDEX [IX_TransferReturns_ReturnDate] ON [dbo].[TransferReturns] ([ReturnDate]);
```

### 2.2 相關資料表

#### 2.2.1 `TransferReturnDetails` - 調撥驗退單明細
```sql
CREATE TABLE [dbo].[TransferReturnDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ReturnId] NVARCHAR(50) NOT NULL, -- 驗退單號
    [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
    [ReceiptDetailId] UNIQUEIDENTIFIER NULL, -- 原驗收單明細ID (關聯至 TransferReceiptDetails，如有)
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
    [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
    [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 原驗收數量
    [ReturnQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗退數量
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
    [Amount] DECIMAL(18, 4) NULL, -- 金額
    [ReturnReason] NVARCHAR(500) NULL, -- 驗退原因
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_TransferReturnDetails_TransferReturns] FOREIGN KEY ([ReturnId]) REFERENCES [dbo].[TransferReturns] ([ReturnId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransferReturnDetails_TransferOrderDetails] FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId]),
    CONSTRAINT [FK_TransferReturnDetails_TransferReceiptDetails] FOREIGN KEY ([ReceiptDetailId]) REFERENCES [dbo].[TransferReceiptDetails] ([DetailId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_ReturnId] ON [dbo].[TransferReturnDetails] ([ReturnId]);
CREATE NONCLUSTERED INDEX [IX_TransferReturnDetails_GoodsId] ON [dbo].[TransferReturnDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ReturnId | NVARCHAR | 50 | NO | - | 驗退單號 | 主鍵，唯一 |
| TransferId | NVARCHAR | 50 | NO | - | 調撥單號 | 外鍵至 TransferOrders |
| ReceiptId | NVARCHAR | 50 | YES | - | 原驗收單號 | 外鍵至 TransferReceipts |
| ReturnDate | DATETIME2 | - | NO | - | 驗退日期 | - |
| FromShopId | NVARCHAR | 50 | NO | - | 調出分店代碼 | 外鍵至分店表 |
| ToShopId | NVARCHAR | 50 | NO | - | 調入分店代碼 | 外鍵至分店表 |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待驗退, R:部分驗退, C:已驗退, X:已取消 |
| ReturnUserId | NVARCHAR | 50 | YES | - | 驗退人員 | 外鍵至使用者表 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| ReturnReason | NVARCHAR | 500 | YES | - | 驗退原因 | - |
| IsSettled | BIT | - | NO | 0 | 是否已日結 | - |
| SettledDate | DATETIME2 | - | YES | - | 日結日期 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢待驗退調撥單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-returns/pending-transfers`
- **說明**: 查詢待驗退的調撥單列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "transferId": "",
      "fromShopId": "",
      "toShopId": "",
      "transferDateFrom": "",
      "transferDateTo": ""
    }
  }
  ```

#### 3.1.2 查詢驗退單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-returns`
- **說明**: 查詢驗退單列表，支援分頁、排序、篩選
- **請求參數**: 參考其他查詢 API

#### 3.1.3 查詢單筆驗退單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-returns/{returnId}`
- **說明**: 根據驗退單號查詢單筆驗退單資料（含明細）

#### 3.1.4 依調撥單號建立驗退單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-returns/from-transfer/{transferId}`
- **說明**: 依調撥單號建立驗退單（帶入調撥單明細或驗收單明細）

#### 3.1.5 新增驗退單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-returns`
- **說明**: 新增驗退單（含明細）

#### 3.1.6 修改驗退單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/transfer-returns/{returnId}`
- **說明**: 修改驗退單（僅未日結且未取消狀態可修改）

#### 3.1.7 刪除驗退單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/transfer-returns/{returnId}`
- **說明**: 刪除驗退單（僅未日結且未取消狀態可刪除）

#### 3.1.8 確認驗退
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-returns/{returnId}/confirm`
- **說明**: 確認驗退，更新庫存（調入庫減少、調出庫增加）及調撥單已退數量

#### 3.1.9 取消驗退單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-returns/{returnId}/cancel`
- **說明**: 取消驗退單

### 3.2 後端實作類別

#### 3.2.1 Controller: `TransferReturnsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/transfer-returns")]
    [Authorize]
    public class TransferReturnsController : ControllerBase
    {
        private readonly ITransferReturnService _transferReturnService;
        
        [HttpGet("pending-transfers")]
        public async Task<ActionResult<ApiResponse<PagedResult<PendingTransferOrderDto>>>> GetPendingTransfers([FromQuery] PendingTransferQueryDto query)
        {
            // 實作查詢待驗退調撥單邏輯
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TransferReturnDto>>>> GetTransferReturns([FromQuery] TransferReturnQueryDto query)
        {
            // 實作查詢驗退單列表邏輯
        }
        
        [HttpGet("{returnId}")]
        public async Task<ActionResult<ApiResponse<TransferReturnDetailDto>>> GetTransferReturn(string returnId)
        {
            // 實作查詢單筆驗退單邏輯
        }
        
        [HttpPost("from-transfer/{transferId}")]
        public async Task<ActionResult<ApiResponse<TransferReturnDetailDto>>> CreateReturnFromTransfer(string transferId)
        {
            // 實作依調撥單建立驗退單邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateTransferReturn([FromBody] CreateTransferReturnDto dto)
        {
            // 實作新增驗退單邏輯
        }
        
        [HttpPut("{returnId}")]
        public async Task<ActionResult<ApiResponse>> UpdateTransferReturn(string returnId, [FromBody] UpdateTransferReturnDto dto)
        {
            // 實作修改驗退單邏輯
        }
        
        [HttpDelete("{returnId}")]
        public async Task<ActionResult<ApiResponse>> DeleteTransferReturn(string returnId)
        {
            // 實作刪除驗退單邏輯
        }
        
        [HttpPost("{returnId}/confirm")]
        public async Task<ActionResult<ApiResponse>> ConfirmReturn(string returnId)
        {
            // 實作確認驗退邏輯（更新庫存、更新調撥單已退數量）
        }
        
        [HttpPost("{returnId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelTransferReturn(string returnId)
        {
            // 實作取消驗退單邏輯
        }
    }
}
```

#### 3.2.2 Service: `TransferReturnService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITransferReturnService
    {
        Task<PagedResult<PendingTransferOrderDto>> GetPendingTransfersAsync(PendingTransferQueryDto query);
        Task<PagedResult<TransferReturnDto>> GetTransferReturnsAsync(TransferReturnQueryDto query);
        Task<TransferReturnDetailDto> GetTransferReturnByIdAsync(string returnId);
        Task<TransferReturnDetailDto> CreateReturnFromTransferAsync(string transferId);
        Task<string> CreateTransferReturnAsync(CreateTransferReturnDto dto);
        Task UpdateTransferReturnAsync(string returnId, UpdateTransferReturnDto dto);
        Task DeleteTransferReturnAsync(string returnId);
        Task ConfirmReturnAsync(string returnId);
        Task CancelTransferReturnAsync(string returnId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 待驗退調撥單列表頁面 (`PendingTransferOrderList.vue`)
- **路徑**: `/transfer/pending-returns`
- **功能**: 顯示待驗退的調撥單列表，支援查詢、建立驗退單

#### 4.1.2 驗退單列表頁面 (`TransferReturnList.vue`)
- **路徑**: `/transfer/transfer-returns`
- **功能**: 顯示驗退單列表，支援查詢、新增、修改、刪除、確認驗退、取消

#### 4.1.3 驗退單詳細頁面 (`TransferReturnDetail.vue`)
- **路徑**: `/transfer/transfer-returns/:returnId`
- **功能**: 顯示驗退單詳細資料（含明細），支援修改、確認驗退、取消

### 4.2 UI 元件設計

#### 4.2.1 待驗退調撥單列表元件 (`PendingTransferOrderList.vue`)
- 顯示待驗退的調撥單
- 支援依調撥單建立驗退單

#### 4.2.2 驗退單列表元件 (`TransferReturnDataTable.vue`)
- 顯示驗退單列表
- 支援查詢、新增、修改、刪除、確認驗退、取消

#### 4.2.3 驗退單新增/修改對話框 (`TransferReturnDialog.vue`)
- 支援新增驗退單
- 支援修改驗退單
- 支援驗退數量維護
- 顯示調撥數量、已驗收數量、已驗退數量、本次驗退數量
- 支援驗退原因輸入

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
- [ ] 庫存更新邏輯實作（調入庫減少、調出庫增加）
- [ ] 調撥單已退數量更新邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 待驗退調撥單列表頁面開發
- [ ] 驗退單列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 驗退數量維護功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 庫存更新測試（調入庫減少、調出庫增加）
- [ ] 調撥單已退數量更新測試
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
- 確認驗退時必須更新庫存
- 調入庫減少（退回商品）
- 調出庫增加（商品退回）
- 必須處理庫存異動記錄
- 必須處理庫存鎖定機制（避免並發問題）

### 6.2 調撥單已退數量更新
- 確認驗退時必須更新調撥單明細的已退數量
- 必須檢查驗退數量是否超過已驗收數量減已驗退數量
- 必須更新調撥單狀態（全部驗退完成時）

### 6.3 日結處理
- 已日結的驗退單不可修改、刪除
- 日結時必須檢查驗退單狀態

### 6.4 資料驗證
- 驗退數量必須大於0
- 驗退數量不可超過已驗收數量減已驗退數量
- 驗退日期必須大於等於調撥單日期
- 驗退原因建議填寫

### 6.5 業務邏輯
- 僅未日結且未取消狀態可修改、刪除
- 確認驗退後不可修改、刪除（除非取消）
- 取消驗退時必須回退庫存及調撥單已退數量

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增驗退單成功
- [ ] 新增驗退單失敗 (驗退數量超過已驗收數量)
- [ ] 修改驗退單成功
- [ ] 修改驗退單失敗 (已日結)
- [ ] 刪除驗退單成功
- [ ] 刪除驗退單失敗 (已日結)
- [ ] 確認驗退成功（庫存更新、調撥單已退數量更新）
- [ ] 取消驗退成功（庫存回退、調撥單已退數量回退）

### 7.2 整合測試
- [ ] 完整驗退流程測試
- [ ] 庫存更新測試（調入庫減少、調出庫增加）
- [ ] 調撥單已退數量更新測試
- [ ] 日結處理測試
- [ ] 並發操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發驗退測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW362_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW362_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/05-調撥管理/SYSW352-調撥單驗收作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

