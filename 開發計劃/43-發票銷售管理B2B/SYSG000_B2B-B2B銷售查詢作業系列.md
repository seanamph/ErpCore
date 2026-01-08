# SYSG000_B2B - B2B銷售查詢作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG000_B2B 系列
- **功能名稱**: B2B銷售查詢作業系列
- **功能描述**: 提供B2B銷售資料的查詢功能，包含B2B銷售單號、銷售日期、客戶、商品明細、數量、價格、狀態等資訊查詢與報表。此模組為SYSG510-SYSG5D0的B2B版本，功能類似但針對B2B場景優化
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP` (BREG銷售查詢功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP` (CTSG銷售查詢功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP` (EINB銷售查詢功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP` (EING銷售查詢功能)

### 1.2 業務需求
- 查詢B2B銷售單基本資料
- 支援多條件組合查詢
- 支援分頁查詢
- 支援排序功能
- 支援模糊搜尋
- 支援報表列印
- 支援Excel匯出
- 支援B2B銷售統計分析
- 支援B2B銷售傳輸記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `B2BSalesOrders` (B2B銷售單主檔)

使用與 SYSG000_B2B-B2B銷售資料維護系列相同的資料表結構，參考 `B2BSalesOrders` 和 `B2BSalesOrderDetails` 資料表。

### 2.2 查詢視圖: `v_B2BSalesOrderQuery` (B2B銷售單查詢視圖)

```sql
CREATE VIEW [dbo].[v_B2BSalesOrderQuery] AS
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
    so.B2BFlag,
    so.TransferType,
    so.TransferStatus,
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
FROM [dbo].[B2BSalesOrders] so
LEFT JOIN [dbo].[Shops] s ON so.ShopId = s.ShopId
LEFT JOIN [dbo].[Customers] c ON so.CustomerId = c.CustomerId
LEFT JOIN [dbo].[Users] u1 ON so.ApplyUserId = u1.UserId
LEFT JOIN [dbo].[Users] u2 ON so.ApproveUserId = u2.UserId
WHERE so.B2BFlag = 'Y';
```

### 2.3 統計視圖: `v_B2BSalesOrderStatistics` (B2B銷售單統計視圖)

```sql
CREATE VIEW [dbo].[v_B2BSalesOrderStatistics] AS
SELECT 
    ShopId,
    ShopName,
    OrderType,
    Status,
    TransferType,
    COUNT(*) AS OrderCount,
    SUM(TotalAmount) AS TotalAmount,
    SUM(TotalQty) AS TotalQty,
    AVG(TotalAmount) AS AvgAmount
FROM [dbo].[v_B2BSalesOrderQuery]
GROUP BY ShopId, ShopName, OrderType, Status, TransferType;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢B2B銷售單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-sales-orders/query`
- **說明**: 查詢B2B銷售單列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢B2B銷售單統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-sales-orders/statistics`
- **說明**: 查詢B2B銷售單統計資料
- **請求參數**: 標準統計查詢參數
- **回應格式**: 標準統計回應格式

#### 3.1.3 匯出Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-sales-orders/export-excel`
- **說明**: 匯出B2B銷售單資料為Excel檔案
- **請求格式**: 同查詢列表的請求參數
- **回應格式**: 返回Excel檔案下載連結

#### 3.1.4 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-sales-orders/print-report`
- **說明**: 產生B2B銷售單報表
- **請求格式**: 同查詢列表的請求參數
- **回應格式**: 返回PDF報表下載連結

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 B2B銷售單查詢頁面 (`B2BSalesOrderQuery.vue`)
- **路徑**: `/b2b-sales-orders/query`
- **功能**: 顯示B2B銷售單查詢結果，支援多條件查詢、統計、匯出、列印

### 4.2 UI 元件設計

參考SYSG510-SYSG5D0-銷售查詢作業系列的UI設計，但針對B2B場景進行優化。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立查詢視圖
- [ ] 建立統計視圖
- [ ] 建立索引

### 5.2 階段二: 後端開發 (3天)
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 統計功能實作
- [ ] Excel匯出功能實作
- [ ] 報表列印功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 統計功能開發
- [ ] 匯出功能開發
- [ ] 列印功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

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
- 統計查詢必須優化

### 6.3 資料驗證
- 查詢條件必須驗證
- B2B標記必須為'Y'

### 6.4 業務邏輯
- B2B銷售單查詢必須符合B2B業務規範
- 統計功能必須支援多維度統計

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢B2B銷售單列表成功
- [ ] 查詢B2B銷售單統計成功
- [ ] 匯出Excel成功
- [ ] 列印報表成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 統計功能測試
- [ ] 匯出功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP`

### 8.2 相關功能
- SYSG510-SYSG5D0-銷售查詢作業系列（一般銷售查詢功能）
- SYSG000_B2B-B2B銷售資料維護系列（B2B銷售資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

