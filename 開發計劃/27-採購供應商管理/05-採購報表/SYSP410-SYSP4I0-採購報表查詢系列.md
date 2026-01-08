# SYSP410-SYSP4I0 - 採購報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP410-SYSP4I0系列
- **功能名稱**: 採購報表查詢系列
- **功能描述**: 提供採購報表的查詢、列印、匯出功能，包含採購明細表、採購統計表、供應商採購表等多種報表類型
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP410_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP4I0_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多種採購報表類型查詢
- 支援多種查詢條件（供應商、日期範圍、採購單號等）
- 支援報表列印與匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考採購單、供應商等相關資料表設計

### 2.2 報表查詢視圖

#### 2.2.1 `v_PurchaseReportQuery` - 採購報表查詢視圖
```sql
CREATE VIEW [dbo].[v_PurchaseReportQuery] AS
SELECT 
    po.PurchaseOrderNo,
    po.PurchaseDate,
    po.SupplierId,
    s.SupplierName,
    po.TotalAmount,
    po.Status,
    -- 其他報表所需欄位
FROM [dbo].[PurchaseOrders] po
LEFT JOIN [dbo].[Suppliers] s ON po.SupplierId = s.SupplierId;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-reports/query`
- **說明**: 查詢採購報表資料

#### 3.1.2 匯出採購報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-reports/export`
- **說明**: 匯出採購報表（Excel、PDF）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購報表查詢頁面 (`PurchaseReportQuery.vue`)
- **路徑**: `/procurement/purchase-reports`
- **功能**: 顯示採購報表查詢介面，支援報表列印與匯出

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢採購報表成功
- [ ] 匯出採購報表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP410_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP4I0_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

