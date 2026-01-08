# SYSW352 - 調撥單驗收作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW352
- **功能名稱**: 調撥單驗收作業
- **功能描述**: 提供調撥單驗收作業的新增、修改、刪除、查詢功能，用於管理調撥單的驗收流程，包含驗收數量、驗收日期、驗收人員、驗收狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW352_*.asp` (相關功能頁面)
  - `WEB/IMS_CORE/SYSW000/SYSW352_PR.rpt` (報表，如有)

### 1.2 業務需求
- 管理調撥單驗收作業
- 支援依調撥單號查詢待驗收單據
- 支援驗收數量維護（可部分驗收）
- 支援驗收日期、驗收人員記錄
- 支援驗收狀態管理（待驗收、部分驗收、已驗收）
- 支援驗收單據列印
- 支援驗收後自動更新庫存（調出庫、調入庫）
- 支援驗收後自動更新調撥單已收數量

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TransferReceipts` (調撥驗收單主檔)

```sql
CREATE TABLE [dbo].[TransferReceipts] (
    [ReceiptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 驗收單號
    [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
    [ReceiptDate] DATETIME2 NOT NULL, -- 驗收日期
    [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
    [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
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
    CONSTRAINT [PK_TransferReceipts] PRIMARY KEY CLUSTERED ([ReceiptId] ASC),
    CONSTRAINT [FK_TransferReceipts_TransferOrders] FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferReceipts_TransferId] ON [dbo].[TransferReceipts] ([TransferId]);
CREATE NONCLUSTERED INDEX [IX_TransferReceipts_FromShopId] ON [dbo].[TransferReceipts] ([FromShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferReceipts_ToShopId] ON [dbo].[TransferReceipts] ([ToShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferReceipts_Status] ON [dbo].[TransferReceipts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_TransferReceipts_ReceiptDate] ON [dbo].[TransferReceipts] ([ReceiptDate]);
```

### 2.2 相關資料表

#### 2.2.1 `TransferReceiptDetails` - 調撥驗收單明細
```sql
CREATE TABLE [dbo].[TransferReceiptDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ReceiptId] NVARCHAR(50) NOT NULL, -- 驗收單號
    [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
    [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
    [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
    [Amount] DECIMAL(18, 4) NULL, -- 金額
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_TransferReceiptDetails_TransferReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[TransferReceipts] ([ReceiptId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransferReceiptDetails_TransferOrderDetails] FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferReceiptDetails_ReceiptId] ON [dbo].[TransferReceiptDetails] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_TransferReceiptDetails_GoodsId] ON [dbo].[TransferReceiptDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ReceiptId | NVARCHAR | 50 | NO | - | 驗收單號 | 主鍵，唯一 |
| TransferId | NVARCHAR | 50 | NO | - | 調撥單號 | 外鍵至 TransferOrders |
| ReceiptDate | DATETIME2 | - | NO | - | 驗收日期 | - |
| FromShopId | NVARCHAR | 50 | NO | - | 調出分店代碼 | 外鍵至分店表 |
| ToShopId | NVARCHAR | 50 | NO | - | 調入分店代碼 | 外鍵至分店表 |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待驗收, R:部分驗收, C:已驗收, X:已取消 |
| ReceiptUserId | NVARCHAR | 50 | YES | - | 驗收人員 | 外鍵至使用者表 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| IsSettled | BIT | - | NO | 0 | 是否已日結 | - |
| SettledDate | DATETIME2 | - | YES | - | 日結日期 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢待驗收調撥單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-receipts/pending-orders`
- **說明**: 查詢待驗收的調撥單列表

#### 3.1.2 查詢驗收單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-receipts`
- **說明**: 查詢調撥驗收單列表，支援分頁、排序、篩選

#### 3.1.3 查詢單筆驗收單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-receipts/{receiptId}`
- **說明**: 根據驗收單號查詢單筆驗收單資料（含明細）

#### 3.1.4 依調撥單號建立驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-receipts/from-order/{transferId}`
- **說明**: 依調撥單號建立驗收單（帶入調撥單明細）

#### 3.1.5 新增驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-receipts`
- **說明**: 新增調撥驗收單（含明細）

#### 3.1.6 修改驗收單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/transfer-receipts/{receiptId}`
- **說明**: 修改驗收單（僅未日結且未取消狀態可修改）

#### 3.1.7 刪除驗收單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/transfer-receipts/{receiptId}`
- **說明**: 刪除驗收單（僅未日結且未取消狀態可刪除）

#### 3.1.8 確認驗收
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-receipts/{receiptId}/confirm`
- **說明**: 確認驗收，更新庫存（調出庫減少、調入庫增加）及調撥單已收數量

#### 3.1.9 取消驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-receipts/{receiptId}/cancel`
- **說明**: 取消驗收單

### 3.2 後端實作類別

#### 3.2.1 Controller: `TransferReceiptsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/transfer-receipts")]
    [Authorize]
    public class TransferReceiptsController : ControllerBase
    {
        private readonly ITransferReceiptService _transferReceiptService;
        
        // 實作參考 SYSW324 的 PurchaseReceiptsController
    }
}
```

#### 3.2.2 Service: `TransferReceiptService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITransferReceiptService
    {
        Task<PagedResult<PendingTransferOrderDto>> GetPendingOrdersAsync(PendingTransferOrderQueryDto query);
        Task<PagedResult<TransferReceiptDto>> GetTransferReceiptsAsync(TransferReceiptQueryDto query);
        Task<TransferReceiptDetailDto> GetTransferReceiptByIdAsync(string receiptId);
        Task<TransferReceiptDetailDto> CreateReceiptFromOrderAsync(string transferId);
        Task<string> CreateTransferReceiptAsync(CreateTransferReceiptDto dto);
        Task UpdateTransferReceiptAsync(string receiptId, UpdateTransferReceiptDto dto);
        Task DeleteTransferReceiptAsync(string receiptId);
        Task ConfirmReceiptAsync(string receiptId);
        Task CancelTransferReceiptAsync(string receiptId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 待驗收調撥單列表頁面 (`PendingTransferOrderList.vue`)
- **路徑**: `/transfer/pending-receipts`
- **功能**: 顯示待驗收的調撥單列表，支援查詢、建立驗收單

#### 4.1.2 驗收單列表頁面 (`TransferReceiptList.vue`)
- **路徑**: `/transfer/transfer-receipts`
- **功能**: 顯示調撥驗收單列表，支援查詢、新增、修改、刪除、確認驗收、取消

#### 4.1.3 驗收單詳細頁面 (`TransferReceiptDetail.vue`)
- **路徑**: `/transfer/transfer-receipts/:receiptId`
- **功能**: 顯示驗收單詳細資料（含明細），支援修改、確認驗收、取消

### 4.2 UI 元件設計

**說明**: UI 設計與 SYSW324 類似，但需顯示調出分店、調入分店資訊

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
- [ ] Service 實作（包含庫存更新邏輯：調出庫減少、調入庫增加）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 庫存更新邏輯實作
- [ ] 調撥單已收數量更新邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 待驗收調撥單列表頁面開發
- [ ] 驗收單列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 驗收數量維護功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 庫存更新測試（調出庫、調入庫）
- [ ] 調撥單已收數量更新測試
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
- 確認驗收時必須更新兩邊庫存：
  - 調出分店庫存減少
  - 調入分店庫存增加
- 必須處理庫存異動記錄（兩邊都要記錄）
- 必須處理庫存鎖定機制（避免並發問題）

### 6.2 調撥單已收數量更新
- 確認驗收時必須更新調撥單明細的已收數量
- 必須檢查驗收數量是否超過調撥數量
- 必須更新調撥單狀態（全部驗收完成時）

### 6.3 其他注意事項
- 參考 SYSW324 開發計劃的注意事項

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增驗收單成功
- [ ] 新增驗收單失敗 (驗收數量超過調撥數量)
- [ ] 修改驗收單成功
- [ ] 修改驗收單失敗 (已日結)
- [ ] 刪除驗收單成功
- [ ] 刪除驗收單失敗 (已日結)
- [ ] 確認驗收成功（調出庫減少、調入庫增加、調撥單已收數量更新）
- [ ] 取消驗收成功（調出庫增加、調入庫減少、調撥單已收數量回退）

### 7.2 整合測試
- [ ] 完整驗收流程測試
- [ ] 庫存更新測試（調出庫、調入庫）
- [ ] 調撥單已收數量更新測試
- [ ] 日結處理測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW352_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW352_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW324-採購單驗收作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

