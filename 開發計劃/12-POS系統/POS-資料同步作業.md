# POS - POS資料同步作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: POS
- **功能名稱**: POS資料同步作業
- **功能描述**: 提供POS系統與IMS系統的資料同步功能，包含交易資料同步、商品資料同步、庫存資料同步等
- **參考舊程式**: 
  - `WEB/IMS_CORE/rsl_pos/handshake.asp`
  - `WEB/IMS_CORE/ASP/Kernel/IDENTIFY_KIOSK.ASP`

### 1.2 業務需求
- 同步POS交易資料至IMS系統
- 同步POS商品資料至IMS系統
- 同步POS庫存資料至IMS系統
- 支援雙向同步
- 支援增量同步
- 支援同步狀態查詢
- 支援同步錯誤處理
- 支援同步記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PosTransactions` (POS交易主檔)

```sql
CREATE TABLE [dbo].[PosTransactions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [StoreId] NVARCHAR(50) NOT NULL, -- 店別代號
    [PosId] NVARCHAR(50) NULL, -- POS機號
    [TransactionDate] DATETIME2 NOT NULL, -- 交易日期時間
    [TransactionType] NVARCHAR(20) NOT NULL, -- 交易類型 (Sale/Return/Refund)
    [TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 總金額
    [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式
    [CustomerId] NVARCHAR(50) NULL, -- 客戶編號
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- 狀態 (Pending/Synced/Failed)
    [SyncAt] DATETIME2 NULL, -- 同步時間
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [UQ_PosTransactions_TransactionId] UNIQUE ([TransactionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosTransactions_StoreId] ON [dbo].[PosTransactions] ([StoreId]);
CREATE NONCLUSTERED INDEX [IX_PosTransactions_TransactionDate] ON [dbo].[PosTransactions] ([TransactionDate]);
CREATE NONCLUSTERED INDEX [IX_PosTransactions_Status] ON [dbo].[PosTransactions] ([Status]);
```

### 2.2 主要資料表: `PosTransactionDetails` (POS交易明細)

```sql
CREATE TABLE [dbo].[PosTransactionDetails] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [LineNo] INT NOT NULL, -- 行號
    [ProductId] NVARCHAR(50) NOT NULL, -- 商品編號
    [ProductName] NVARCHAR(200) NULL, -- 商品名稱
    [Quantity] DECIMAL(18,3) NOT NULL DEFAULT 0, -- 數量
    [UnitPrice] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 單價
    [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 金額
    [Discount] DECIMAL(18,2) NULL DEFAULT 0, -- 折扣
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PosTransactionDetails_PosTransactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[PosTransactions] ([TransactionId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosTransactionDetails_TransactionId] ON [dbo].[PosTransactionDetails] ([TransactionId]);
CREATE NONCLUSTERED INDEX [IX_PosTransactionDetails_ProductId] ON [dbo].[PosTransactionDetails] ([ProductId]);
```

### 2.3 主要資料表: `PosSyncLogs` (POS同步記錄)

```sql
CREATE TABLE [dbo].[PosSyncLogs] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SyncType] NVARCHAR(20) NOT NULL, -- 同步類型 (Transaction/Product/Inventory)
    [SyncDirection] NVARCHAR(20) NOT NULL, -- 同步方向 (ToIMS/FromPOS)
    [RecordCount] INT NOT NULL DEFAULT 0, -- 記錄筆數
    [SuccessCount] INT NOT NULL DEFAULT 0, -- 成功筆數
    [FailedCount] INT NOT NULL DEFAULT 0, -- 失敗筆數
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Running', -- 狀態 (Running/Completed/Failed)
    [StartTime] DATETIME2 NOT NULL, -- 開始時間
    [EndTime] DATETIME2 NULL, -- 結束時間
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE() -- 建立時間
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosSyncLogs_SyncType] ON [dbo].[PosSyncLogs] ([SyncType]);
CREATE NONCLUSTERED INDEX [IX_PosSyncLogs_CreatedAt] ON [dbo].[PosSyncLogs] ([CreatedAt]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 同步POS交易資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pos/sync/transactions`
- **說明**: 同步POS交易資料至IMS系統
- **請求格式**:
  ```json
  {
    "storeId": "ST001",
    "startDate": "2024-01-01T00:00:00",
    "endDate": "2024-01-31T23:59:59",
    "syncType": "Incremental"
  }
  ```

#### 3.1.2 查詢POS交易資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/transactions`
- **說明**: 查詢POS交易資料，支援分頁、排序、篩選

#### 3.1.3 查詢POS同步記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/sync-logs`
- **說明**: 查詢POS同步記錄，支援分頁、排序、篩選

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 POS同步作業頁面 (`PosSync.vue`)
- 同步設定表單（同步類型、同步方向、日期範圍）
- 同步執行按鈕
- 同步進度顯示
- 同步結果顯示

#### 4.1.2 POS交易查詢頁面 (`PosTransactions.vue`)
- 查詢表單（店別、日期範圍、交易類型）
- 資料表格（顯示交易資料）
- 分頁元件
- 查看詳情對話框

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 PosTransactions 資料表
- [ ] 建立 PosTransactionDetails 資料表
- [ ] 建立 PosSyncLogs 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (10天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] PosSyncService 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 同步邏輯實作
- [ ] 錯誤處理
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數
- [ ] POS同步作業頁面開發
- [ ] POS交易查詢頁面開發
- [ ] 同步進度顯示
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (3天)
- [ ] API 整合測試
- [ ] 同步功能測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 21天

---

## 六、注意事項

### 6.1 同步機制
- 需實作增量同步機制
- 需處理同步衝突
- 需實作重試機制
- 需記錄同步狀態

### 6.2 資料一致性
- 需確保資料一致性
- 需處理同步失敗的情況
- 需實作資料驗證

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

