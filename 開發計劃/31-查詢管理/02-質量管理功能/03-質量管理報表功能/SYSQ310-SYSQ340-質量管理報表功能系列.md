# SYSQ310-SYSQ340 - 質量管理報表功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSQ310-SYSQ340 系列
- **功能名稱**: 質量管理報表功能系列（零用金報表查詢）
- **功能描述**: 提供零用金相關報表的查詢、列印、匯出功能，包含零用金支付表、支付申請單、零用金撥補明細表等報表
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ310_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ310_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ320_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ320_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ330_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ330_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ340_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSQ000/SYSQ340_FB.ASP` (瀏覽)

### 1.2 業務需求
- 提供零用金支付表報表查詢
- 提供支付申請單報表查詢
- 提供零用金撥補明細表報表查詢
- 支援條件查詢與資料篩選
- 支援報表資料預覽
- 支援報表列印（PDF格式）
- 支援報表匯出（Excel、PDF）
- 支援日期範圍查詢
- 支援多店別查詢
- 支援保管人篩選
- 支援排序功能（零用金單號、輸入順序號等）

---

## 二、資料庫設計 (Schema)

### 2.1 報表查詢視圖

本功能主要使用現有的資料表進行報表查詢，不新增資料表。主要查詢的資料表包括：
- `PcCash` - 零用金主檔
- `PcKeep` - 保管人及額度設定
- `PcCashRequest` - 零用金請款檔
- `PcCashTransfer` - 零用金拋轉檔
- `V_EMP_USER` - 員工使用者視圖

### 2.2 報表查詢視圖

#### 2.2.1 `v_PcCashPaymentReport` - 零用金支付表視圖
```sql
CREATE VIEW [dbo].[v_PcCashPaymentReport] AS
SELECT 
    pc.TKey,
    pc.CashId,
    pc.SiteId,
    s.SiteName,
    pc.AppleDate,
    pc.AppleName,
    e1.EmpName AS AppleNameDesc,
    pc.OrgId,
    o.OrgName,
    pc.KeepEmpId,
    e2.EmpName AS KeepEmpName,
    pk.PcQuota,
    pc.CashAmount,
    pc.CashStatus,
    pc.Notes,
    pc.BTime,
    pc.CTime
FROM [dbo].[PcCash] pc
LEFT JOIN [dbo].[Sites] s ON pc.SiteId = s.SiteId
LEFT JOIN [dbo].[V_EMP_USER] e1 ON pc.AppleName = e1.EmpId
LEFT JOIN [dbo].[Organizations] o ON pc.OrgId = o.OrgId
LEFT JOIN [dbo].[PcKeep] pk ON pc.KeepEmpId = pk.KeepEmpId AND pc.SiteId = pk.SiteId
LEFT JOIN [dbo].[V_EMP_USER] e2 ON pc.KeepEmpId = e2.EmpId
WHERE pc.CashStatus IN ('APPLIED', 'REQUESTED', 'TRANSFERRED', 'APPROVED');
```

#### 2.2.2 `v_PaymentApplicationReport` - 支付申請單視圖
```sql
CREATE VIEW [dbo].[v_PaymentApplicationReport] AS
SELECT 
    pcr.TKey,
    pcr.RequestId,
    pcr.SiteId,
    s.SiteName,
    pcr.RequestDate,
    pcr.RequestAmount,
    pcr.RequestStatus,
    pcr.Notes,
    pcr.BTime,
    pcr.CTime,
    COUNT(pc.CashId) AS CashCount
FROM [dbo].[PcCashRequest] pcr
LEFT JOIN [dbo].[Sites] s ON pcr.SiteId = s.SiteId
LEFT JOIN [dbo].[PcCash] pc ON JSON_CONTAINS(pcr.CashIds, CONCAT('"', pc.CashId, '"'))
GROUP BY pcr.TKey, pcr.RequestId, pcr.SiteId, s.SiteName, pcr.RequestDate, pcr.RequestAmount, pcr.RequestStatus, pcr.Notes, pcr.BTime, pcr.CTime;
```

#### 2.2.3 `v_PcCashReplenishmentReport` - 零用金撥補明細表視圖
```sql
CREATE VIEW [dbo].[v_PcCashReplenishmentReport] AS
SELECT 
    pc.TKey,
    pc.CashId,
    pc.SiteId,
    s.SiteName,
    pc.AppleDate,
    pc.AppleName,
    e1.EmpName AS AppleNameDesc,
    pc.KeepEmpId,
    e2.EmpName AS KeepEmpName,
    pk.PcQuota,
    pc.CashAmount,
    pc.CashStatus,
    pc.BTime,
    pc.CTime,
    ROW_NUMBER() OVER (PARTITION BY pc.SiteId, pc.KeepEmpId ORDER BY pc.BTime) AS InputSeqNo
FROM [dbo].[PcCash] pc
LEFT JOIN [dbo].[Sites] s ON pc.SiteId = s.SiteId
LEFT JOIN [dbo].[V_EMP_USER] e1 ON pc.AppleName = e1.EmpId
LEFT JOIN [dbo].[PcKeep] pk ON pc.KeepEmpId = pk.KeepEmpId AND pc.SiteId = pk.SiteId
LEFT JOIN [dbo].[V_EMP_USER] e2 ON pc.KeepEmpId = e2.EmpId
WHERE pc.CashStatus IN ('APPLIED', 'REQUESTED', 'TRANSFERRED', 'APPROVED');
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢零用金支付表報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/pc-cash-payment`
- **說明**: 查詢零用金支付表報表資料
- **請求格式**:
  ```json
  {
    "siteId": "",
    "appleDateFrom": "2024-01-01",
    "appleDateTo": "2024-12-31",
    "appleName": "",
    "keepEmpId": "",
    "cashStatus": "",
    "pageIndex": 1,
    "pageSize": 20,
    "orderBy": "AppleDate",
    "orderDirection": "DESC"
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 匯出零用金支付表報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/pc-cash-payment/export`
- **說明**: 匯出零用金支付表報表（Excel、PDF）
- **請求格式**: 同查詢
- **回應格式**: 檔案下載回應格式

#### 3.1.3 查詢支付申請單報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/payment-application`
- **說明**: 查詢支付申請單報表資料
- **請求格式**:
  ```json
  {
    "siteId": "",
    "requestDateFrom": "2024-01-01",
    "requestDateTo": "2024-12-31",
    "requestStatus": "",
    "pageIndex": 1,
    "pageSize": 20,
    "orderBy": "RequestDate",
    "orderDirection": "DESC"
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.4 匯出支付申請單報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/payment-application/export`
- **說明**: 匯出支付申請單報表（Excel、PDF）
- **請求格式**: 同查詢
- **回應格式**: 檔案下載回應格式

#### 3.1.5 查詢零用金撥補明細表報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/pc-cash-replenishment`
- **說明**: 查詢零用金撥補明細表報表資料
- **請求格式**:
  ```json
  {
    "siteId": "",
    "appleDateFrom": "2024-01-01",
    "appleDateTo": "2024-12-31",
    "keepEmpId": "",
    "cashStatus": "",
    "orderBy": "CashId",
    "orderDirection": "ASC",
    "pageIndex": 1,
    "pageSize": 20
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.6 匯出零用金撥補明細表報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/pc-cash-replenishment/export`
- **說明**: 匯出零用金撥補明細表報表（Excel、PDF）
- **請求格式**: 同查詢
- **回應格式**: 檔案下載回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 零用金支付表報表頁面 (`PcCashPaymentReport.vue`)
- **路徑**: `/reports/pc-cash-payment`
- **功能**: 顯示零用金支付表報表查詢條件，支援查詢、列印、匯出

#### 4.1.2 支付申請單報表頁面 (`PaymentApplicationReport.vue`)
- **路徑**: `/reports/payment-application`
- **功能**: 顯示支付申請單報表查詢條件，支援查詢、列印、匯出

#### 4.1.3 零用金撥補明細表報表頁面 (`PcCashReplenishmentReport.vue`)
- **路徑**: `/reports/pc-cash-replenishment`
- **功能**: 顯示零用金撥補明細表報表查詢條件，支援查詢、列印、匯出

### 4.2 主要元件

#### 4.2.1 報表查詢表單
- 分店代號（下拉選單，可多選）
- 申請日期起（日期選擇器）
- 申請日期迄（日期選擇器）
- 申請人（下拉選單，可從V_EMP_USER選擇）
- 保管人（下拉選單，可從V_EMP_USER選擇）
- 狀態（下拉選單）
- 排序方式（下拉選單：零用金單號、輸入順序號等）

#### 4.2.2 報表資料表格
- 顯示報表資料欄位
- 支援排序
- 支援分頁
- 支援列印按鈕
- 支援匯出按鈕（Excel、PDF）

---

## 五、開發時程

**總計**: 12天
- 資料庫設計: 1天
- 後端API開發: 4天
- 前端UI開發: 5天
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化
- 報表匯出需使用背景處理

### 6.3 業務邏輯
- 報表查詢需支援多條件篩選
- 報表查詢需支援日期範圍查詢
- 報表查詢需支援多店別查詢
- 報表查詢需支援排序功能
- 報表列印需支援PDF格式
- 報表匯出需支援Excel、PDF格式
- 保管人資料來源需從總帳參數讀取（CASH_KEEPER_SOURCE）
- 保管人模式需從總帳參數讀取（CASH_KEEP_NAME_MODE）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢零用金支付表報表成功
- [ ] 匯出零用金支付表報表成功（Excel）
- [ ] 匯出零用金支付表報表成功（PDF）
- [ ] 查詢支付申請單報表成功
- [ ] 匯出支付申請單報表成功（Excel）
- [ ] 匯出支付申請單報表成功（PDF）
- [ ] 查詢零用金撥補明細表報表成功
- [ ] 匯出零用金撥補明細表報表成功（Excel）
- [ ] 匯出零用金撥補明細表報表成功（PDF）
- [ ] 報表排序功能測試
- [ ] 報表分頁功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ310_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ320_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ330_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/SYSQ000/SYSQ340_*.ASP` 目錄下的所有ASP檔案
- `WEB/IMS_CORE/ASP/util/SYST000/util.asp` - 總帳工具函數
- `WEB/IMS_CORE/ASP/util/SYSH000/HR_UTIL.ASP` - 人資工具函數

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

