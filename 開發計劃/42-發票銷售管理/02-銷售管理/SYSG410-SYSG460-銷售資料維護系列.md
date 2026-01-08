# SYSG410-SYSG460 - 銷售資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG410-SYSG460 系列
- **功能名稱**: 銷售資料維護系列
- **功能描述**: 提供銷售資料的新增、修改、刪除、查詢功能，包含銷售單號、銷售日期、客戶、商品明細、數量、價格、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG410_*.ASP` (銷售資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG420_*.ASP` (銷售資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG430_*.ASP` (銷售資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG440_*.ASP` (銷售資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG450_*.ASP` (銷售資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG460_*.ASP` (銷售資料維護)

### 1.2 業務需求
- 管理銷售單基本資料
- 支援銷售單狀態管理（草稿、已送出、已審核、已出貨、已取消、已結案）
- 支援客戶選擇
- 支援商品明細維護
- 支援數量、單價、金額計算
- 支援銷售單審核流程
- 支援多店別管理
- 支援銷售單報表列印
- 支援銷售單歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SalesOrders` (銷售單主檔)

```sql
CREATE TABLE [dbo].[SalesOrders] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號 (SO_NO)
    [OrderDate] DATETIME2 NOT NULL, -- 銷售日期 (SO_DATE)
    [OrderType] NVARCHAR(20) NOT NULL, -- 單據類型 (ORDER_TYPE, SO:銷售, RT:退貨)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [CustomerId] NVARCHAR(50) NULL, -- 客戶代碼 (CUSTOMER_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, S:已送出, A:已審核, X:已取消, C:已結案)
    [ApplyUserId] NVARCHAR(50) NULL, -- 申請人員 (APPLY_USER)
    [ApplyDate] DATETIME2 NULL, -- 申請日期 (APPLY_DATE)
    [ApproveUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVE_USER)
    [ApproveDate] DATETIME2 NULL, -- 審核日期 (APPROVE_DATE)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [TotalQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 總數量 (TOTAL_QTY)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [ExpectedDate] DATETIME2 NULL, -- 預期交貨日期 (EXPECTED_DATE)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SalesOrders] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SalesOrders_OrderId] UNIQUE ([OrderId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderId] ON [dbo].[SalesOrders] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_ShopId] ON [dbo].[SalesOrders] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_CustomerId] ON [dbo].[SalesOrders] ([CustomerId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_Status] ON [dbo].[SalesOrders] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderDate] ON [dbo].[SalesOrders] ([OrderDate]);
CREATE NONCLUSTERED INDEX [IX_SalesOrders_OrderType] ON [dbo].[SalesOrders] ([OrderType]);
```

### 2.2 相關資料表

#### 2.2.1 `SalesOrderDetails` - 銷售單明細
```sql
CREATE TABLE [dbo].[SalesOrderDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
    [OrderQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 訂購數量 (ORDER_QTY)
    [UnitPrice] DECIMAL(18, 4) NULL, -- 單價 (UNIT_PRICE)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [ShippedQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已出貨數量 (SHIPPED_QTY)
    [ReturnQty] DECIMAL(18, 4) NULL DEFAULT 0, -- 已退數量 (RETURN_QTY)
    [UnitId] NVARCHAR(50) NULL, -- 單位 (UNIT_ID)
    [TaxRate] DECIMAL(5, 2) NULL DEFAULT 0, -- 稅率 (TAX_RATE)
    [TaxAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 稅額 (TAX_AMOUNT)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SalesOrderDetails_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_OrderId] ON [dbo].[SalesOrderDetails] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_GoodsId] ON [dbo].[SalesOrderDetails] ([GoodsId]);
CREATE NONCLUSTERED INDEX [IX_SalesOrderDetails_BarcodeId] ON [dbo].[SalesOrderDetails] ([BarcodeId]);
```

### 2.3 資料字典

#### 2.3.1 SalesOrders 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 銷售單號 | 唯一，SO_NO |
| OrderDate | DATETIME2 | - | NO | - | 銷售日期 | SO_DATE |
| OrderType | NVARCHAR | 20 | NO | - | 單據類型 | SO:銷售, RT:退貨 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| CustomerId | NVARCHAR | 50 | YES | - | 客戶代碼 | 外鍵至客戶表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, X:已取消, C:已結案 |
| ApplyUserId | NVARCHAR | 50 | YES | - | 申請人員 | 外鍵至使用者表 |
| ApplyDate | DATETIME2 | - | YES | - | 申請日期 | - |
| ApproveUserId | NVARCHAR | 50 | YES | - | 審核人員 | 外鍵至使用者表 |
| ApproveDate | DATETIME2 | - | YES | - | 審核日期 | - |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| TotalQty | DECIMAL | 18,4 | YES | 0 | 總數量 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| ExpectedDate | DATETIME2 | - | YES | - | 預期交貨日期 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |

#### 2.3.2 SalesOrderDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| OrderId | NVARCHAR | 50 | NO | - | 銷售單號 | 外鍵至SalesOrders |
| LineNum | INT | - | NO | - | 行號 | - |
| GoodsId | NVARCHAR | 50 | NO | - | 商品編號 | 外鍵至商品表 |
| BarcodeId | NVARCHAR | 50 | YES | - | 條碼編號 | 外鍵至條碼表 |
| OrderQty | DECIMAL | 18,4 | NO | 0 | 訂購數量 | - |
| UnitPrice | DECIMAL | 18,4 | YES | - | 單價 | - |
| Amount | DECIMAL | 18,4 | YES | - | 金額 | - |
| ShippedQty | DECIMAL | 18,4 | YES | 0 | 已出貨數量 | - |
| ReturnQty | DECIMAL | 18,4 | YES | 0 | 已退數量 | - |
| UnitId | NVARCHAR | 50 | YES | - | 單位 | - |
| TaxRate | DECIMAL | 5,2 | YES | 0 | 稅率 | - |
| TaxAmount | DECIMAL | 18,4 | YES | 0 | 稅額 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢銷售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders`
- **說明**: 查詢銷售單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "OrderId",
    "sortOrder": "ASC",
    "filters": {
      "orderId": "",
      "orderType": "",
      "shopId": "",
      "customerId": "",
      "status": "",
      "orderDateFrom": "",
      "orderDateTo": ""
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
          "orderId": "SO20240101001",
          "orderDate": "2024-01-01",
          "orderType": "SO",
          "shopId": "SHOP001",
          "shopName": "總店",
          "customerId": "CUS001",
          "customerName": "客戶A",
          "status": "A",
          "totalAmount": 100000.00,
          "totalQty": 100.00,
          "applyUserId": "U001",
          "applyUserName": "張三",
          "applyDate": "2024-01-01T10:00:00",
          "approveUserId": "U002",
          "approveUserName": "李四",
          "approveDate": "2024-01-01T11:00:00"
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

#### 3.1.2 查詢單筆銷售單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 根據銷售單號查詢單筆銷售單資料（含明細）
- **路徑參數**:
  - `orderId`: 銷售單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "orderId": "SO20240101001",
      "orderDate": "2024-01-01",
      "orderType": "SO",
      "shopId": "SHOP001",
      "shopName": "總店",
      "customerId": "CUS001",
      "customerName": "客戶A",
      "status": "A",
      "totalAmount": 100000.00,
      "totalQty": 100.00,
      "memo": "備註",
      "expectedDate": "2024-01-15",
      "siteId": "SITE001",
      "orgId": "ORG001",
      "currencyId": "TWD",
      "exchangeRate": 1.0,
      "applyUserId": "U001",
      "applyUserName": "張三",
      "applyDate": "2024-01-01T10:00:00",
      "approveUserId": "U002",
      "approveUserName": "李四",
      "approveDate": "2024-01-01T11:00:00",
      "details": [
        {
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品A",
          "barcodeId": "BC001",
          "orderQty": 10.00,
          "unitPrice": 1000.00,
          "amount": 10000.00,
          "shippedQty": 0.00,
          "returnQty": 0.00,
          "unitId": "PCS",
          "taxRate": 5.00,
          "taxAmount": 500.00,
          "memo": "明細備註"
        }
      ],
      "createdBy": "U001",
      "createdAt": "2024-01-01T10:00:00",
      "updatedBy": "U002",
      "updatedAt": "2024-01-01T11:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增銷售單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders`
- **說明**: 新增銷售單資料（含明細）
- **請求格式**:
  ```json
  {
    "orderId": "SO20240101001",
    "orderDate": "2024-01-01",
    "orderType": "SO",
    "shopId": "SHOP001",
    "customerId": "CUS001",
    "status": "D",
    "totalAmount": 100000.00,
    "totalQty": 100.00,
    "memo": "備註",
    "expectedDate": "2024-01-15",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "details": [
      {
        "lineNum": 1,
        "goodsId": "G001",
        "barcodeId": "BC001",
        "orderQty": 10.00,
        "unitPrice": 1000.00,
        "amount": 10000.00,
        "unitId": "PCS",
        "taxRate": 5.00,
        "taxAmount": 500.00,
        "memo": "明細備註"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1,
      "orderId": "SO20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改銷售單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 修改銷售單資料（含明細）
- **請求格式**: 同新增格式
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "修改成功",
    "data": {
      "tKey": 1,
      "orderId": "SO20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 刪除銷售單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/sales-orders/{orderId}`
- **說明**: 刪除銷售單資料
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 業務邏輯

#### 3.2.1 新增邏輯
1. 驗證銷售單號唯一性
2. 驗證銷售日期格式
3. 驗證客戶是否存在
4. 驗證商品是否存在
5. 計算總金額與總數量
6. 記錄建立者與建立時間

#### 3.2.2 修改邏輯
1. 驗證銷售單號唯一性（排除自己）
2. 驗證銷售日期格式
3. 驗證客戶是否存在
4. 驗證商品是否存在
5. 重新計算總金額與總數量
6. 記錄更新者與更新時間

#### 3.2.3 刪除邏輯
1. 檢查是否有關聯的出貨記錄
2. 軟刪除（將狀態設為 'X'）或硬刪除（根據業務需求）

#### 3.2.4 查詢邏輯
1. 支援多條件組合查詢
2. 支援分頁查詢
3. 支援排序
4. 支援模糊搜尋（銷售單號、客戶名稱等）

---

## 四、前端 UI 設計

### 4.1 銷售單列表頁面

#### 4.1.1 頁面結構
- **標題**: 銷售資料維護
- **查詢區塊**: 
  - 銷售單號（文字輸入）
  - 單據類型（下拉選單：全部、銷售、退貨）
  - 分店代碼（下拉選單）
  - 客戶代碼（文字輸入）
  - 狀態（下拉選單：全部、草稿、已送出、已審核、已出貨、已取消、已結案）
  - 銷售日期起（日期選擇器）
  - 銷售日期迄（日期選擇器）
  - 查詢按鈕、重置按鈕
- **操作按鈕區塊**:
  - 新增按鈕
  - 修改按鈕
  - 刪除按鈕
  - 匯出按鈕
  - 列印按鈕
- **資料表格區塊**:
  - 序號
  - 銷售單號
  - 銷售日期
  - 單據類型
  - 分店代碼
  - 客戶代碼
  - 客戶名稱
  - 總金額
  - 總數量
  - 狀態
  - 操作（查看、修改、刪除）

#### 4.1.2 元件設計
- 使用 Element Plus 的 `el-table` 元件顯示列表
- 使用 `el-pagination` 元件實現分頁
- 使用 `el-form` 元件實現查詢表單
- 使用 `el-dialog` 元件實現新增/修改對話框

### 4.2 銷售單新增/修改對話框

#### 4.2.1 表單欄位
- **基本資料區塊**:
  - 銷售單號（必填，文字輸入）
  - 銷售日期（必填，日期選擇器）
  - 單據類型（必填，下拉選單：銷售、退貨）
  - 分店代碼（必填，下拉選單）
  - 客戶代碼（選填，下拉選單）
  - 預期交貨日期（選填，日期選擇器）
  - 分公司代碼（選填，下拉選單）
  - 組織代碼（選填，下拉選單）
  - 幣別（選填，下拉選單，預設TWD）
  - 匯率（選填，數字輸入，預設1）
  - 狀態（必填，下拉選單：草稿、已送出、已審核、已出貨、已取消、已結案）
  - 備註（選填，多行文字輸入）
- **明細資料區塊**:
  - 商品編號（必填，下拉選單）
  - 條碼編號（選填，下拉選單）
  - 訂購數量（必填，數字輸入）
  - 單價（必填，數字輸入）
  - 金額（自動計算）
  - 單位（選填，下拉選單）
  - 稅率（選填，數字輸入，預設0）
  - 稅額（自動計算）
  - 備註（選填，多行文字輸入）
  - 新增明細按鈕、刪除明細按鈕

#### 4.2.2 驗證規則
- 銷售單號：必填，唯一性驗證
- 銷售日期：必填
- 單據類型：必填
- 分店代碼：必填
- 訂購數量：必填，必須大於0
- 單價：必填，必須大於等於0
- 金額：自動計算（訂購數量 * 單價）
- 稅額：自動計算（金額 * 稅率 / 100）

### 4.3 銷售單刪除確認對話框

#### 4.3.1 確認訊息
- 顯示要刪除的銷售單號
- 提示刪除後無法復原
- 確認按鈕、取消按鈕

---

## 五、開發時程

### 5.1 後端開發（3天）
- **第1天**: 資料庫設計與建立、Repository 層開發
- **第2天**: Service 層開發、API Controller 開發
- **第3天**: 單元測試、API 測試

### 5.2 前端開發（3天）
- **第1天**: 列表頁面開發、查詢功能開發
- **第2天**: 新增/修改對話框開發、表單驗證
- **第3天**: 刪除功能開發、整合測試

### 5.3 整合測試（1天）
- 前後端整合測試
- 功能測試
- 效能測試

**總計**: 7天

---

## 六、注意事項

### 6.1 資料驗證
- 銷售單號必須唯一
- 銷售日期必須合理
- 客戶必須存在
- 商品必須存在
- 訂購數量必須大於0
- 單價必須大於等於0

### 6.2 權限控制
- 新增權限：需要有銷售單新增權限
- 修改權限：需要有銷售單修改權限
- 刪除權限：需要有銷售單刪除權限
- 查詢權限：需要有銷售單查詢權限

### 6.3 效能優化
- 查詢列表時使用分頁，避免一次載入過多資料
- 建立適當的索引以提升查詢效能
- 使用快取機制快取常用資料（如客戶資料、商品資料等）

### 6.4 錯誤處理
- 新增/修改時驗證資料完整性
- 刪除時檢查是否有關聯資料
- 提供明確的錯誤訊息

### 6.5 資料備份
- 刪除操作建議使用軟刪除（將狀態設為已取消）
- 如需硬刪除，必須先備份資料

---

## 七、測試案例

### 7.1 新增測試
1. **測試案例1**: 正常新增銷售單
   - 輸入完整的銷售單資料
   - 預期結果：新增成功，返回新增的銷售單資料

2. **測試案例2**: 銷售單號重複
   - 輸入已存在的銷售單號
   - 預期結果：新增失敗，提示銷售單號已存在

3. **測試案例3**: 必填欄位未填
   - 不填寫必填欄位
   - 預期結果：新增失敗，提示必填欄位未填

### 7.2 修改測試
1. **測試案例1**: 正常修改銷售單
   - 修改銷售單資料
   - 預期結果：修改成功，返回修改後的銷售單資料

2. **測試案例2**: 銷售單號重複
   - 修改為已存在的銷售單號
   - 預期結果：修改失敗，提示銷售單號已存在

### 7.3 刪除測試
1. **測試案例1**: 正常刪除銷售單
   - 刪除銷售單資料
   - 預期結果：刪除成功

2. **測試案例2**: 刪除有關聯資料的銷售單
   - 刪除有出貨記錄的銷售單
   - 預期結果：刪除失敗，提示有關聯資料

### 7.4 查詢測試
1. **測試案例1**: 正常查詢銷售單列表
   - 查詢銷售單列表
   - 預期結果：返回銷售單列表

2. **測試案例2**: 條件查詢
   - 使用多個條件查詢
   - 預期結果：返回符合條件的銷售單列表

3. **測試案例3**: 分頁查詢
   - 查詢第2頁資料
   - 預期結果：返回第2頁的銷售單列表

---

## 八、參考資料

### 8.1 舊程式檔案
- `WEB/IMS_CORE/ASP/SYSG000/SYSG410_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG420_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG430_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG440_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG450_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG460_*.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSG000/SYSG410.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

