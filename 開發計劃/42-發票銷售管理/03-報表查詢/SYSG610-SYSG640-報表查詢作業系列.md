# SYSG610-SYSG640 - 報表查詢作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG610-SYSG640 系列
- **功能名稱**: 報表查詢作業系列
- **功能描述**: 提供銷售報表的查詢功能，包含銷售報表、統計報表、分析報表等各種報表查詢與列印
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG610_*.ASP` (報表查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG620_*.ASP` (報表查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG630_*.ASP` (報表查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG640_*.ASP` (報表查詢)

### 1.2 業務需求
- 查詢銷售報表資料
- 支援多條件組合查詢
- 支援分頁查詢
- 支援排序功能
- 支援報表列印
- 支援Excel匯出
- 支援PDF匯出
- 支援報表統計分析

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

使用與 SYSG410-SYSG460 相同的資料表結構，參考 `SalesOrders` 和 `SalesOrderDetails` 資料表。

### 2.2 報表視圖: `v_SalesReport` (銷售報表視圖)

```sql
CREATE VIEW [dbo].[v_SalesReport] AS
SELECT 
    so.OrderId,
    so.OrderDate,
    so.OrderType,
    so.ShopId,
    s.ShopName,
    so.CustomerId,
    c.CustomerName,
    so.Status,
    so.TotalAmount,
    so.TotalQty,
    sod.GoodsId,
    g.GoodsName,
    sod.OrderQty,
    sod.UnitPrice,
    sod.Amount,
    sod.ShippedQty,
    sod.ReturnQty,
    so.CurrencyId,
    so.ExchangeRate,
    so.ApplyUserId,
    u1.UserName AS ApplyUserName,
    so.ApplyDate,
    so.ApproveUserId,
    u2.UserName AS ApproveUserName,
    so.ApproveDate
FROM [dbo].[SalesOrders] so
LEFT JOIN [dbo].[SalesOrderDetails] sod ON so.OrderId = sod.OrderId
LEFT JOIN [dbo].[Shops] s ON so.ShopId = s.ShopId
LEFT JOIN [dbo].[Customers] c ON so.CustomerId = c.CustomerId
LEFT JOIN [dbo].[Goods] g ON sod.GoodsId = g.GoodsId
LEFT JOIN [dbo].[Users] u1 ON so.ApplyUserId = u1.UserId
LEFT JOIN [dbo].[Users] u2 ON so.ApproveUserId = u2.UserId;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-reports/query`
- **說明**: 查詢銷售報表資料，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "OrderId",
    "sortOrder": "ASC",
    "reportType": "DETAIL", // DETAIL:明細報表, SUMMARY:彙總報表
    "filters": {
      "orderId": "",
      "orderType": "",
      "shopId": "",
      "customerId": "",
      "goodsId": "",
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
          "goodsId": "G001",
          "goodsName": "商品A",
          "orderQty": 10.00,
          "unitPrice": 1000.00,
          "amount": 10000.00,
          "shippedQty": 0.00,
          "returnQty": 0.00,
          "status": "A",
          "totalAmount": 100000.00
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

#### 3.1.2 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-reports/print`
- **說明**: 產生銷售報表PDF
- **請求格式**: 同查詢報表資料的請求參數
- **回應格式**: 返回PDF報表下載連結

#### 3.1.3 匯出Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-reports/export-excel`
- **說明**: 匯出銷售報表資料為Excel檔案
- **請求格式**: 同查詢報表資料的請求參數
- **回應格式**: 返回Excel檔案下載連結

---

## 四、前端 UI 設計

### 4.1 報表查詢頁面

#### 4.1.1 頁面結構
- **標題**: 報表查詢作業
- **查詢區塊**: 
  - 報表類型（下拉選單：明細報表、彙總報表）
  - 銷售單號（文字輸入）
  - 單據類型（下拉選單：全部、銷售、退貨）
  - 分店代碼（下拉選單）
  - 客戶代碼（文字輸入）
  - 商品編號（文字輸入）
  - 狀態（下拉選單：全部、草稿、已送出、已審核、已出貨、已取消、已結案）
  - 銷售日期起（日期選擇器）
  - 銷售日期迄（日期選擇器）
  - 查詢按鈕、重置按鈕
- **操作按鈕區塊**:
  - 列印報表按鈕
  - 匯出Excel按鈕
  - 匯出PDF按鈕
- **資料表格區塊**:
  - 序號
  - 銷售單號
  - 銷售日期
  - 單據類型
  - 分店代碼
  - 客戶代碼
  - 商品編號
  - 商品名稱
  - 訂購數量
  - 單價
  - 金額
  - 已出貨數量
  - 已退數量
  - 狀態

#### 4.1.2 元件設計
- 使用 Element Plus 的 `el-table` 元件顯示列表
- 使用 `el-pagination` 元件實現分頁
- 使用 `el-form` 元件實現查詢表單

---

## 五、開發時程

### 5.1 後端開發（2天）
- **第1天**: 報表視圖建立、Repository 層開發
- **第2天**: Service 層開發、API Controller 開發、報表列印功能、Excel匯出功能

### 5.2 前端開發（2天）
- **第1天**: 查詢頁面開發、查詢功能開發
- **第2天**: 報表列印功能、Excel匯出功能、PDF匯出功能

### 5.3 整合測試（1天）
- 前後端整合測試
- 功能測試
- 效能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 報表效能
- 報表查詢時使用分頁，避免一次載入過多資料
- 建立適當的索引以提升查詢效能
- 使用快取機制快取常用資料

### 6.2 權限控制
- 查詢權限：需要有報表查詢權限
- 列印權限：需要有報表列印權限
- 匯出權限：需要有報表匯出權限

### 6.3 資料安全
- 報表資料僅顯示使用者有權限查看的資料
- 敏感資料需要加密處理

---

## 七、測試案例

### 7.1 查詢測試
1. **測試案例1**: 正常查詢報表資料
   - 查詢報表資料
   - 預期結果：返回報表資料

2. **測試案例2**: 條件查詢
   - 使用多個條件查詢
   - 預期結果：返回符合條件的報表資料

### 7.2 列印測試
1. **測試案例1**: 正常列印報表
   - 列印報表
   - 預期結果：成功產生PDF報表

### 7.3 匯出測試
1. **測試案例1**: 正常匯出Excel
   - 匯出報表資料為Excel
   - 預期結果：成功產生Excel檔案

---

## 八、參考資料

### 8.1 舊程式檔案
- `WEB/IMS_CORE/ASP/SYSG000/SYSG610_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG620_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG630_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG640_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

