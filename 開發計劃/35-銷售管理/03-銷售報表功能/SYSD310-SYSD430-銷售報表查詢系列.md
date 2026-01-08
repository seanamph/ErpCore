# SYSD310-SYSD430 - 銷售報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSD310-SYSD430 系列
- **功能名稱**: 銷售報表查詢系列
- **功能描述**: 提供銷售報表查詢功能，包含銷售明細報表、銷售統計報表、銷售分析報表等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD310_*.ASP` (銷售報表查詢相關)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD320_*.ASP` (銷售統計報表相關)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD400_*.ASP` (銷售分析報表相關)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD430_*.ASP` (銷售擴展報表相關)

### 1.2 業務需求
- 支援銷售明細報表查詢
- 支援銷售統計報表查詢（按日期、客戶、商品等）
- 支援銷售分析報表查詢
- 支援報表匯出功能（Excel、PDF）
- 支援報表列印功能

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表
本功能主要使用 `SalesOrders` 和 `SalesOrderDetails` 資料表，參考「SYSD110-SYSD140-銷售資料維護系列」的資料庫設計。

### 2.2 報表快取表: `SalesReportCache` (銷售報表快取)

```sql
CREATE TABLE [dbo].[SalesReportCache] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportType] NVARCHAR(50) NOT NULL, -- 報表類型
    [ReportParams] NVARCHAR(MAX) NULL, -- 報表參數（JSON格式）
    [ReportData] NVARCHAR(MAX) NULL, -- 報表資料（JSON格式）
    [CacheExpireTime] DATETIME2 NOT NULL, -- 快取過期時間
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_SalesReportCache_Type_Params] UNIQUE ([ReportType], [ReportParams])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesReportCache_ReportType] ON [dbo].[SalesReportCache] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_SalesReportCache_CacheExpireTime] ON [dbo].[SalesReportCache] ([CacheExpireTime]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 銷售明細報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/reports/detail`
- **說明**: 查詢銷售明細報表
- **請求格式**:
  ```json
  {
    "orderDateFrom": "2024-01-01",
    "orderDateTo": "2024-01-31",
    "orderType": "",
    "shopId": "",
    "customerId": "",
    "status": "",
    "pageIndex": 1,
    "pageSize": 20
  }
  ```
- **回應格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計

#### 3.1.2 銷售統計報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/reports/statistics`
- **說明**: 查詢銷售統計報表
- **請求格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計
- **回應格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計

#### 3.1.3 銷售分析報表查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/reports/analysis`
- **說明**: 查詢銷售分析報表
- **請求格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計
- **回應格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計

#### 3.1.4 報表匯出
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/reports/export`
- **說明**: 匯出報表（Excel、PDF格式）
- **請求格式**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 銷售報表查詢頁面 (`SalesReport.vue`)
- **路徑**: `/sales-orders/reports`
- **功能**: 提供銷售報表查詢功能
- **主要元件**: 參考「SYSK310-SYSK500-憑證報表查詢系列」的設計

---

## 五、開發時程

### 5.1 階段一: 後端開發 (3天)
- [ ] 報表查詢邏輯實作
- [ ] 報表統計邏輯實作
- [ ] 報表分析邏輯實作
- [ ] 報表匯出邏輯實作
- [ ] 報表快取機制實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] 報表查詢頁面開發
- [ ] 報表結果顯示開發
- [ ] 報表匯出功能開發
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 6天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 必須使用報表快取機制
- 統計報表必須使用資料庫聚合函數

### 6.2 資料驗證
- 查詢條件必須驗證日期範圍
- 報表參數必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 明細報表查詢成功
- [ ] 統計報表查詢成功
- [ ] 分析報表查詢成功
- [ ] 報表匯出成功

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 報表快取測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSD000/SYSD310_*.ASP`
- `WEB/IMS_CORE/ASP/SYSD000/SYSD320_*.ASP`
- `WEB/IMS_CORE/ASP/SYSD000/SYSD400_*.ASP`
- `WEB/IMS_CORE/ASP/SYSD000/SYSD430_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

