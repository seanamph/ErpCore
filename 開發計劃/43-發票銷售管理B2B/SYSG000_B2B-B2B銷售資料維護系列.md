# SYSG000_B2B - B2B銷售資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG000_B2B 系列
- **功能名稱**: B2B銷售資料維護系列
- **功能描述**: 提供B2B銷售資料的新增、修改、刪除、查詢功能，包含B2B銷售單號、銷售日期、客戶、商品明細、數量、價格、狀態等資訊管理。此模組為SYSG410-SYSG460的B2B版本，功能類似但針對B2B場景優化
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP` (BREG銷售功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP` (CTSG銷售功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP` (EINB銷售功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP` (EING銷售功能)

### 1.2 業務需求
- 管理B2B銷售單基本資料
- 支援B2B銷售單狀態管理（草稿、已送出、已審核、已出貨、已取消、已結案）
- 支援B2B客戶選擇
- 支援B2B商品明細維護
- 支援數量、單價、金額計算
- 支援B2B銷售單審核流程
- 支援多店別管理
- 支援B2B銷售單報表列印
- 支援B2B銷售單歷史記錄查詢
- 支援B2B發票傳輸功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `B2BSalesOrders` (B2B銷售單主檔)

```sql
CREATE TABLE [dbo].[B2BSalesOrders] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
    [OrderDate] DATETIME2 NOT NULL, -- 銷售日期
    [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (SO:銷售, RT:退貨)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼
    [CustomerId] NVARCHAR(50) NULL, -- 客戶代碼
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員
    [ApplyDate] DATETIME2 NULL, -- 申請日期
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員
    [ApproveDate] DATETIME2 NULL, -- 審核日期
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量
    [Memo] NVARCHAR(500) NULL, -- 備註
    [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率
    [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y', -- B2B標記
    [TransferType] NVARCHAR(20) NULL, -- 傳輸類型 (BREG, CTSG, EINB, EING等)
    [TransferStatus] NVARCHAR(20) NULL, -- 傳輸狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_B2BSalesOrders] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_B2BSalesOrders_OrderId] UNIQUE ([OrderId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_OrderId] ON [dbo].[B2BSalesOrders] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_ShopId] ON [dbo].[B2BSalesOrders] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_CustomerId] ON [dbo].[B2BSalesOrders] ([CustomerId]);
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_Status] ON [dbo].[B2BSalesOrders] ([Status]);
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_OrderDate] ON [dbo].[B2BSalesOrders] ([OrderDate]);
CREATE NONCLUSTERED INDEX [IX_B2BSalesOrders_B2BFlag] ON [dbo].[B2BSalesOrders] ([B2BFlag]);
```

### 2.2 相關資料表

#### 2.2.1 `B2BSalesOrderDetails` - B2B銷售單明細
```sql
CREATE TABLE [dbo].[B2BSalesOrderDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價
    [Amount] DECIMAL(18, 4) NULL, -- 金額
    [ShippedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已出貨數量
    [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已退數量
    [UnitId] NVARCHAR(50) NULL, -- 單位
    [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_B2BSalesOrderDetails_B2BSalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[B2BSalesOrders] ([OrderId]) ON DELETE CASCADE
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢B2B銷售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-sales-orders`
- **說明**: 查詢B2B銷售單列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆B2B銷售單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-sales-orders/{orderId}`
- **說明**: 查詢單筆B2B銷售單資料

#### 3.1.3 新增B2B銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-sales-orders`
- **說明**: 新增B2B銷售單資料

#### 3.1.4 修改B2B銷售單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/b2b-sales-orders/{orderId}`
- **說明**: 修改B2B銷售單資料

#### 3.1.5 刪除B2B銷售單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/b2b-sales-orders/{orderId}`
- **說明**: 刪除B2B銷售單資料

#### 3.1.6 B2B銷售單傳輸
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-sales-orders/{orderId}/transfer`
- **說明**: 執行B2B銷售單傳輸作業

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 B2B銷售單列表頁面 (`B2BSalesOrderList.vue`)
- **路徑**: `/b2b-sales-orders`
- **功能**: 顯示B2B銷售單列表，支援查詢、新增、修改、刪除、傳輸

#### 4.1.2 B2B銷售單詳細頁面 (`B2BSalesOrderDetail.vue`)
- **路徑**: `/b2b-sales-orders/:orderId`
- **功能**: 顯示B2B銷售單詳細資料，支援修改

### 4.2 UI 元件設計

參考SYSG410-SYSG460-銷售資料維護系列的UI設計，但針對B2B場景進行優化。

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
- [ ] 傳輸功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 傳輸功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- B2B銷售單資料必須與一般銷售單資料隔離

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 傳輸功能必須支援非同步處理

### 6.3 資料驗證
- 銷售單號必須唯一
- 必填欄位必須驗證
- B2B標記必須為'Y'

### 6.4 業務邏輯
- B2B銷售單格式必須符合B2B業務規範
- 傳輸功能必須記錄傳輸狀態

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增B2B銷售單成功
- [ ] 修改B2B銷售單成功
- [ ] 刪除B2B銷售單成功
- [ ] 查詢B2B銷售單列表成功
- [ ] B2B銷售單傳輸成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 傳輸功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP`

### 8.2 相關功能
- SYSG410-SYSG460-銷售資料維護系列（一般銷售資料維護功能）
- SYSG000_B2B-B2B發票資料維護系列（B2B發票資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

