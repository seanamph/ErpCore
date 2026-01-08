# SYSG510-SYSG5D0 - 銷售查詢作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG510-SYSG5D0 系列
- **功能名稱**: 銷售查詢作業系列
- **功能描述**: 提供銷售資料的查詢功能，包含銷售單號、銷售日期、客戶、商品明細、數量、價格、狀態等資訊查詢與報表
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG510_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG520_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG530_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG540_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG550_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG560_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG570_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG590_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG5A0_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG5B0_*.ASP` (銷售查詢)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG5D0_*.ASP` (銷售查詢)

### 1.2 業務需求
- 查詢銷售單基本資料
- 支援多條件組合查詢
- 支援分頁查詢
- 支援排序功能
- 支援模糊搜尋
- 支援報表列印
- 支援Excel匯出
- 支援銷售統計分析

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SalesOrders` (銷售單主檔)

使用與 SYSG410-SYSG460 相同的資料表結構，參考 `SalesOrders` 和 `SalesOrderDetails` 資料表。

### 2.2 查詢視圖: `v_SalesOrderQuery` (銷售單查詢視圖)

```sql
CREATE VIEW [dbo].[v_SalesOrderQuery] AS
SELECT 
    so.TKey,
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
    so.Memo,
    so.ExpectedDate,
    so.SiteId,
    so.OrgId,
    so.CurrencyId,
    so.ExchangeRate,
    so.ApplyUserId,
    u1.UserName AS ApplyUserName,
    so.ApplyDate,
    so.ApproveUserId,
    u2.UserName AS ApproveUserName,
    so.ApproveDate,
    so.CreatedBy,
    so.CreatedAt,
    so.UpdatedBy,
    so.UpdatedAt
FROM [dbo].[SalesOrders] so
LEFT JOIN [dbo].[Shops] s ON so.ShopId = s.ShopId
LEFT JOIN [dbo].[Customers] c ON so.CustomerId = c.CustomerId
LEFT JOIN [dbo].[Users] u1 ON so.ApplyUserId = u1.UserId
LEFT JOIN [dbo].[Users] u2 ON so.ApproveUserId = u2.UserId;
```

### 2.3 統計視圖: `v_SalesOrderStatistics` (銷售單統計視圖)

```sql
CREATE VIEW [dbo].[v_SalesOrderStatistics] AS
SELECT 
    ShopId,
    ShopName,
    OrderType,
    Status,
    COUNT(*) AS OrderCount,
    SUM(TotalAmount) AS TotalAmount,
    SUM(TotalQty) AS TotalQty,
    AVG(TotalAmount) AS AvgAmount
FROM [dbo].[v_SalesOrderQuery]
GROUP BY ShopId, ShopName, OrderType, Status;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢銷售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders/query`
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
      "orderDateTo": "",
      "applyUserId": "",
      "approveUserId": ""
    }
  }
  ```
- **回應格式**: 同 SYSG410-SYSG460 的查詢回應格式

#### 3.1.2 查詢銷售單統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sales-orders/statistics`
- **說明**: 查詢銷售單統計資料
- **請求參數**:
  ```json
  {
    "shopId": "",
    "orderType": "",
    "status": "",
    "orderDateFrom": "",
    "orderDateTo": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "orderCount": 100,
      "totalAmount": 1000000.00,
      "totalQty": 1000.00,
      "avgAmount": 10000.00,
      "byShop": [
        {
          "shopId": "SHOP001",
          "shopName": "總店",
          "orderCount": 50,
          "totalAmount": 500000.00
        }
      ],
      "byStatus": [
        {
          "status": "A",
          "statusName": "已審核",
          "orderCount": 80,
          "totalAmount": 800000.00
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 匯出Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/export-excel`
- **說明**: 匯出銷售單資料為Excel檔案
- **請求格式**: 同查詢列表的請求參數
- **回應格式**: 返回Excel檔案下載連結

#### 3.1.4 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/print-report`
- **說明**: 產生銷售單報表
- **請求格式**: 同查詢列表的請求參數
- **回應格式**: 返回PDF報表下載連結

### 3.2 業務邏輯

#### 3.2.1 查詢邏輯
1. 支援多條件組合查詢
2. 支援分頁查詢
3. 支援排序
4. 支援模糊搜尋（銷售單號、客戶名稱等）
5. 支援日期區間查詢
6. 支援狀態篩選

#### 3.2.2 統計邏輯
1. 按分店統計
2. 按狀態統計
3. 按單據類型統計
4. 按日期區間統計
5. 計算總金額、總數量、平均金額

---

## 四、前端 UI 設計

### 4.1 銷售單查詢頁面

#### 4.1.1 頁面結構
- **標題**: 銷售查詢作業
- **查詢區塊**: 
  - 銷售單號（文字輸入）
  - 單據類型（下拉選單：全部、銷售、退貨）
  - 分店代碼（下拉選單）
  - 客戶代碼（文字輸入）
  - 狀態（下拉選單：全部、草稿、已送出、已審核、已出貨、已取消、已結案）
  - 銷售日期起（日期選擇器）
  - 銷售日期迄（日期選擇器）
  - 申請人員（下拉選單）
  - 審核人員（下拉選單）
  - 查詢按鈕、重置按鈕
- **操作按鈕區塊**:
  - 匯出Excel按鈕
  - 列印報表按鈕
  - 統計分析按鈕
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
  - 申請人員
  - 審核人員
  - 操作（查看明細）

#### 4.1.2 元件設計
- 使用 Element Plus 的 `el-table` 元件顯示列表
- 使用 `el-pagination` 元件實現分頁
- 使用 `el-form` 元件實現查詢表單
- 使用 `el-dialog` 元件實現統計分析對話框

### 4.2 統計分析對話框

#### 4.2.1 統計內容
- **總計區塊**:
  - 銷售單筆數
  - 總金額
  - 總數量
  - 平均金額
- **分店統計區塊**:
  - 分店代碼
  - 分店名稱
  - 銷售單筆數
  - 總金額
- **狀態統計區塊**:
  - 狀態
  - 狀態名稱
  - 銷售單筆數
  - 總金額

---

## 五、開發時程

### 5.1 後端開發（2天）
- **第1天**: 查詢視圖建立、Repository 層開發、統計邏輯開發
- **第2天**: Service 層開發、API Controller 開發、Excel匯出功能、報表列印功能

### 5.2 前端開發（2天）
- **第1天**: 查詢頁面開發、查詢功能開發
- **第2天**: 統計分析對話框開發、Excel匯出功能、報表列印功能

### 5.3 整合測試（1天）
- 前後端整合測試
- 功能測試
- 效能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 查詢效能
- 查詢列表時使用分頁，避免一次載入過多資料
- 建立適當的索引以提升查詢效能
- 使用快取機制快取常用資料

### 6.2 權限控制
- 查詢權限：需要有銷售單查詢權限
- 匯出權限：需要有銷售單匯出權限
- 列印權限：需要有銷售單列印權限

### 6.3 資料安全
- 查詢結果僅顯示使用者有權限查看的資料
- 敏感資料需要加密處理

---

## 七、測試案例

### 7.1 查詢測試
1. **測試案例1**: 正常查詢銷售單列表
   - 查詢銷售單列表
   - 預期結果：返回銷售單列表

2. **測試案例2**: 條件查詢
   - 使用多個條件查詢
   - 預期結果：返回符合條件的銷售單列表

3. **測試案例3**: 分頁查詢
   - 查詢第2頁資料
   - 預期結果：返回第2頁的銷售單列表

### 7.2 統計測試
1. **測試案例1**: 正常統計
   - 查詢銷售單統計
   - 預期結果：返回統計資料

2. **測試案例2**: 條件統計
   - 使用條件查詢統計
   - 預期結果：返回符合條件的統計資料

### 7.3 匯出測試
1. **測試案例1**: 正常匯出Excel
   - 匯出銷售單資料為Excel
   - 預期結果：成功產生Excel檔案

---

## 八、參考資料

### 8.1 舊程式檔案
- `WEB/IMS_CORE/ASP/SYSG000/SYSG510_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG520_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG530_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG540_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG550_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG560_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG570_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG590_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG5A0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG5B0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG5D0_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

