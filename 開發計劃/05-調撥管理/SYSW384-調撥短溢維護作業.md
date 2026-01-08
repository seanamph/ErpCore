# SYSW384 - 調撥短溢維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW384
- **功能名稱**: 調撥短溢維護作業
- **功能描述**: 提供調撥短溢維護作業的新增、修改、刪除、查詢功能，用於處理調撥過程中發生的短少或溢收情況，包含短溢數量、短溢原因、處理方式等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_FS.asp` (儲存)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW384_PR.asp` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW384_PR.rpt` (報表定義)

### 1.2 業務需求
- 管理調撥短溢維護作業
- 支援依調撥單號查詢短溢記錄
- 支援短溢數量維護（短少為負數，溢收為正數）
- 支援短溢原因記錄
- 支援處理方式選擇（調整庫存、待處理、已處理）
- 支援短溢單據列印
- 支援短溢後自動更新庫存（如選擇調整庫存）
- 支援短溢記錄審核流程

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TransferShortages` (調撥短溢單主檔)

```sql
CREATE TABLE [dbo].[TransferShortages] (
    [ShortageId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 短溢單號
    [TransferId] NVARCHAR(50) NOT NULL, -- 調撥單號 (關聯至 TransferOrders)
    [ReceiptId] NVARCHAR(50) NULL, -- 驗收單號 (關聯至 TransferReceipts，如有)
    [ShortageDate] DATETIME2 NOT NULL, -- 短溢日期
    [FromShopId] NVARCHAR(50) NOT NULL, -- 調出分店代碼
    [ToShopId] NVARCHAR(50) NOT NULL, -- 調入分店代碼
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (P:待處理, A:已審核, C:已處理, X:已取消)
    [ProcessType] NVARCHAR(20) NULL, -- 處理方式 (ADJUST:調整庫存, PENDING:待處理, PROCESSED:已處理)
    [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
    [ProcessDate] DATETIME2 NULL, -- 處理日期
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員
    [ApproveDate] DATETIME2 NULL, -- 審核日期
    [TotalShortageQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總短溢數量（正數為溢收，負數為短少）
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
    [ShortageReason] NVARCHAR(500) NULL, -- 短溢原因
    [Memo] NVARCHAR(500) NULL, -- 備註
    [IsSettled] BIT NOT NULL DEFAULT 0, -- 是否已日結
    [SettledDate] DATETIME2 NULL, -- 日結日期
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TransferShortages] PRIMARY KEY CLUSTERED ([ShortageId] ASC),
    CONSTRAINT [FK_TransferShortages_TransferOrders] FOREIGN KEY ([TransferId]) REFERENCES [dbo].[TransferOrders] ([TransferId]),
    CONSTRAINT [FK_TransferShortages_TransferReceipts] FOREIGN KEY ([ReceiptId]) REFERENCES [dbo].[TransferReceipts] ([ReceiptId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferShortages_TransferId] ON [dbo].[TransferShortages] ([TransferId]);
CREATE NONCLUSTERED INDEX [IX_TransferShortages_ReceiptId] ON [dbo].[TransferShortages] ([ReceiptId]);
CREATE NONCLUSTERED INDEX [IX_TransferShortages_FromShopId] ON [dbo].[TransferShortages] ([FromShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferShortages_ToShopId] ON [dbo].[TransferShortages] ([ToShopId]);
CREATE NONCLUSTERED INDEX [IX_TransferShortages_Status] ON [dbo].[TransferShortages] ([Status]);
CREATE NONCLUSTERED INDEX [IX_TransferShortages_ShortageDate] ON [dbo].[TransferShortages] ([ShortageDate]);
```

### 2.2 相關資料表

#### 2.2.1 `TransferShortageDetails` - 調撥短溢單明細
```sql
CREATE TABLE [dbo].[TransferShortageDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ShortageId] NVARCHAR(50) NOT NULL, -- 短溢單號
    [TransferDetailId] UNIQUEIDENTIFIER NULL, -- 調撥單明細ID (關聯至 TransferOrderDetails)
    [ReceiptDetailId] UNIQUEIDENTIFIER NULL, -- 驗收單明細ID (關聯至 TransferReceiptDetails，如有)
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
    [TransferQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調撥數量
    [ReceiptQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 驗收數量
    [ShortageQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 短溢數量（正數為溢收，負數為短少）
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
    [Amount] DECIMAL(18, 4) NULL, -- 金額
    [ShortageReason] NVARCHAR(500) NULL, -- 短溢原因
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_TransferShortageDetails_TransferShortages] FOREIGN KEY ([ShortageId]) REFERENCES [dbo].[TransferShortages] ([ShortageId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransferShortageDetails_TransferOrderDetails] FOREIGN KEY ([TransferDetailId]) REFERENCES [dbo].[TransferOrderDetails] ([DetailId]),
    CONSTRAINT [FK_TransferShortageDetails_TransferReceiptDetails] FOREIGN KEY ([ReceiptDetailId]) REFERENCES [dbo].[TransferReceiptDetails] ([DetailId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_ShortageId] ON [dbo].[TransferShortageDetails] ([ShortageId]);
CREATE NONCLUSTERED INDEX [IX_TransferShortageDetails_GoodsId] ON [dbo].[TransferShortageDetails] ([GoodsId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ShortageId | NVARCHAR | 50 | NO | - | 短溢單號 | 主鍵，唯一 |
| TransferId | NVARCHAR | 50 | NO | - | 調撥單號 | 外鍵至 TransferOrders |
| ReceiptId | NVARCHAR | 50 | YES | - | 驗收單號 | 外鍵至 TransferReceipts |
| ShortageDate | DATETIME2 | - | NO | - | 短溢日期 | - |
| FromShopId | NVARCHAR | 50 | NO | - | 調出分店代碼 | 外鍵至分店表 |
| ToShopId | NVARCHAR | 50 | NO | - | 調入分店代碼 | 外鍵至分店表 |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待處理, A:已審核, C:已處理, X:已取消 |
| ProcessType | NVARCHAR | 20 | YES | - | 處理方式 | ADJUST:調整庫存, PENDING:待處理, PROCESSED:已處理 |
| TotalShortageQty | DECIMAL | 18,4 | YES | 0 | 總短溢數量 | 正數為溢收，負數為短少 |
| IsSettled | BIT | - | NO | 0 | 是否已日結 | - |
| SettledDate | DATETIME2 | - | YES | - | 日結日期 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢短溢單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-shortages`
- **說明**: 查詢短溢單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ShortageDate",
    "sortOrder": "DESC",
    "filters": {
      "shortageId": "",
      "transferId": "",
      "fromShopId": "",
      "toShopId": "",
      "status": "",
      "processType": "",
      "shortageDateFrom": "",
      "shortageDateTo": ""
    }
  }
  ```

#### 3.1.2 查詢單筆短溢單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/transfer-shortages/{shortageId}`
- **說明**: 根據短溢單號查詢單筆短溢單資料（含明細）

#### 3.1.3 依調撥單號建立短溢單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-shortages/from-transfer/{transferId}`
- **說明**: 依調撥單號建立短溢單（帶入調撥單明細或驗收單明細）

#### 3.1.4 新增短溢單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-shortages`
- **說明**: 新增短溢單（含明細）
- **請求格式**:
  ```json
  {
    "transferId": "TR001",
    "receiptId": "TRR001",
    "shortageDate": "2024-01-01",
    "fromShopId": "SHOP001",
    "toShopId": "SHOP002",
    "processType": "ADJUST",
    "shortageReason": "運輸過程短少",
    "details": [
      {
        "lineNum": 1,
        "goodsId": "GOODS001",
        "barcodeId": "BC001",
        "transferQty": 100,
        "receiptQty": 98,
        "shortageQty": -2,
        "unitPrice": 100,
        "shortageReason": "包裝破損"
      }
    ]
  }
  ```

#### 3.1.5 修改短溢單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/transfer-shortages/{shortageId}`
- **說明**: 修改短溢單（僅未日結且未取消狀態可修改）

#### 3.1.6 刪除短溢單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/transfer-shortages/{shortageId}`
- **說明**: 刪除短溢單（僅未日結且未取消狀態可刪除）

#### 3.1.7 審核短溢單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-shortages/{shortageId}/approve`
- **說明**: 審核短溢單
- **請求格式**:
  ```json
  {
    "approveUserId": "USER001",
    "approveDate": "2024-01-01",
    "notes": "審核通過"
  }
  ```

#### 3.1.8 處理短溢單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/transfer-shortages/{shortageId}/process`
- **說明**: 處理短溢單（如選擇調整庫存，則更新庫存）
- **請求格式**:
  ```json
  {
    "processUserId": "USER001",
    "processDate": "2024-01-01",
    "processType": "ADJUST",
    "notes": "已調整庫存"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `TransferShortagesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/transfer-shortages")]
    [Authorize]
    public class TransferShortagesController : ControllerBase
    {
        private readonly ITransferShortageService _transferShortageService;
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<TransferShortageDto>>>> GetTransferShortages([FromQuery] TransferShortageQueryDto query)
        {
            // 實作查詢短溢單列表邏輯
        }
        
        [HttpGet("{shortageId}")]
        public async Task<ActionResult<ApiResponse<TransferShortageDetailDto>>> GetTransferShortage(string shortageId)
        {
            // 實作查詢單筆短溢單邏輯
        }
        
        [HttpPost("from-transfer/{transferId}")]
        public async Task<ActionResult<ApiResponse<TransferShortageDetailDto>>> CreateShortageFromTransfer(string transferId)
        {
            // 實作依調撥單建立短溢單邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateTransferShortage([FromBody] CreateTransferShortageDto dto)
        {
            // 實作新增短溢單邏輯
        }
        
        [HttpPut("{shortageId}")]
        public async Task<ActionResult<ApiResponse>> UpdateTransferShortage(string shortageId, [FromBody] UpdateTransferShortageDto dto)
        {
            // 實作修改短溢單邏輯
        }
        
        [HttpDelete("{shortageId}")]
        public async Task<ActionResult<ApiResponse>> DeleteTransferShortage(string shortageId)
        {
            // 實作刪除短溢單邏輯
        }
        
        [HttpPost("{shortageId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveTransferShortage(string shortageId, [FromBody] ApproveTransferShortageDto dto)
        {
            // 實作審核短溢單邏輯
        }
        
        [HttpPost("{shortageId}/process")]
        public async Task<ActionResult<ApiResponse>> ProcessTransferShortage(string shortageId, [FromBody] ProcessTransferShortageDto dto)
        {
            // 實作處理短溢單邏輯（如選擇調整庫存，則更新庫存）
        }
    }
}
```

#### 3.2.2 Service: `TransferShortageService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ITransferShortageService
    {
        Task<PagedResult<TransferShortageDto>> GetTransferShortagesAsync(TransferShortageQueryDto query);
        Task<TransferShortageDetailDto> GetTransferShortageByIdAsync(string shortageId);
        Task<TransferShortageDetailDto> CreateShortageFromTransferAsync(string transferId);
        Task<string> CreateTransferShortageAsync(CreateTransferShortageDto dto);
        Task UpdateTransferShortageAsync(string shortageId, UpdateTransferShortageDto dto);
        Task DeleteTransferShortageAsync(string shortageId);
        Task ApproveTransferShortageAsync(string shortageId, ApproveTransferShortageDto dto);
        Task ProcessTransferShortageAsync(string shortageId, ProcessTransferShortageDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 短溢單列表頁面 (`TransferShortageList.vue`)
- **路徑**: `/transfer/transfer-shortages`
- **功能**: 顯示短溢單列表，支援查詢、新增、修改、刪除、審核、處理

#### 4.1.2 短溢單詳細頁面 (`TransferShortageDetail.vue`)
- **路徑**: `/transfer/transfer-shortages/:shortageId`
- **功能**: 顯示短溢單詳細資料（含明細），支援修改、審核、處理

#### 4.1.3 短溢單新增/修改頁面 (`TransferShortageForm.vue`)
- **路徑**: `/transfer/transfer-shortages/new` 或 `/transfer/transfer-shortages/:shortageId/edit`
- **功能**: 新增或修改短溢單

### 4.2 UI 元件設計

#### 4.2.1 短溢單列表元件 (`TransferShortageDataTable.vue`)
- 顯示短溢單列表
- 支援查詢、新增、修改、刪除、審核、處理
- 顯示短溢數量（正數為溢收，負數為短少）

#### 4.2.2 短溢單新增/修改對話框 (`TransferShortageDialog.vue`)
- 支援新增短溢單
- 支援修改短溢單
- 支援短溢數量維護（正數為溢收，負數為短少）
- 顯示調撥數量、驗收數量、短溢數量
- 支援處理方式選擇
- 支援短溢原因輸入

#### 4.2.3 短溢單處理對話框 (`TransferShortageProcessDialog.vue`)
- 選擇處理方式（調整庫存、待處理、已處理）
- 輸入處理人員、處理日期
- 輸入處理備註

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
- [ ] 庫存更新邏輯實作（如選擇調整庫存）
- [ ] 審核流程邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 短溢單列表頁面開發
- [ ] 短溢單新增/修改頁面開發
- [ ] 短溢單處理對話框開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 庫存更新測試（如選擇調整庫存）
- [ ] 審核流程測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 13天

---

## 六、注意事項

### 6.1 短溢數量計算
- 短溢數量 = 驗收數量 - 調撥數量
- 正數表示溢收（驗收數量 > 調撥數量）
- 負數表示短少（驗收數量 < 調撥數量）

### 6.2 庫存更新
- 僅當處理方式選擇「調整庫存」時才更新庫存
- 短少時：調入庫減少，調出庫增加
- 溢收時：調入庫增加，調出庫減少
- 必須處理庫存異動記錄
- 必須處理庫存鎖定機制（避免並發問題）

### 6.3 審核流程
- 短溢單需經過審核流程
- 已審核的短溢單不可修改、刪除
- 審核時需記錄審核人員、審核日期

### 6.4 處理方式
- 調整庫存：自動更新庫存
- 待處理：不更新庫存，等待後續處理
- 已處理：已手動處理，不更新庫存

### 6.5 資料驗證
- 短溢數量必須合理（不可為0）
- 短溢原因建議填寫
- 處理方式必須選擇

### 6.6 業務邏輯
- 僅未日結且未取消狀態可修改、刪除
- 審核後不可修改、刪除
- 處理後不可修改、刪除（除非取消）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增短溢單成功
- [ ] 新增短溢單失敗 (短溢數量為0)
- [ ] 修改短溢單成功
- [ ] 修改短溢單失敗 (已審核)
- [ ] 刪除短溢單成功
- [ ] 刪除短溢單失敗 (已審核)
- [ ] 審核短溢單成功
- [ ] 處理短溢單成功（調整庫存）
- [ ] 處理短溢單成功（待處理）

### 7.2 整合測試
- [ ] 完整短溢流程測試
- [ ] 庫存更新測試（調整庫存）
- [ ] 審核流程測試
- [ ] 處理流程測試
- [ ] 並發操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發短溢處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW384_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW384_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/05-調撥管理/SYSW352-調撥單驗收作業.md`
- `開發計劃/05-調撥管理/SYSW362-調撥單驗退作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

