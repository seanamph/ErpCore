# SYSH220-SYSH2B0 - 薪資報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH220-SYSH2B0系列
- **功能名稱**: 薪資報表查詢系列
- **功能描述**: 提供薪資報表的查詢、列印、匯出功能，包含薪資明細表、薪資統計表、薪資發放表等多種報表類型
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH220_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH230_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSH2B0_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多種薪資報表類型查詢
- 支援多種查詢條件（員工、部門、年度、月份、狀態等）
- 支援報表列印與匯出（Excel、PDF）
- 支援報表參數設定
- 支援報表資料快取

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考 `SalaryData` 資料表設計（見 SYSH210-薪資資料維護系列.md）

### 2.2 報表查詢視圖

#### 2.2.1 `v_SalaryReportDetail` - 薪資明細報表視圖
```sql
CREATE VIEW [dbo].[v_SalaryReportDetail] AS
SELECT 
    sd.TKey,
    sd.EmployeeId,
    e.EmployeeName,
    e.DepartmentId,
    d.DepartmentName,
    sd.SalaryYear,
    sd.SalaryMonth,
    sd.BasicSalary,
    sd.Allowance,
    sd.Bonus,
    sd.OvertimePay,
    sd.Deduction,
    sd.Insurance,
    sd.Tax,
    sd.NetSalary,
    sd.Status,
    sd.PayDate,
    sd.CreatedAt,
    sd.UpdatedAt
FROM [dbo].[SalaryData] sd
LEFT JOIN [dbo].[Employees] e ON sd.EmployeeId = e.EmployeeId
LEFT JOIN [dbo].[Departments] d ON e.DepartmentId = d.DepartmentId;
```

#### 2.2.2 `v_SalaryReportSummary` - 薪資統計報表視圖
```sql
CREATE VIEW [dbo].[v_SalaryReportSummary] AS
SELECT 
    sd.SalaryYear,
    sd.SalaryMonth,
    e.DepartmentId,
    d.DepartmentName,
    COUNT(DISTINCT sd.EmployeeId) AS EmployeeCount,
    SUM(sd.BasicSalary) AS TotalBasicSalary,
    SUM(sd.Allowance) AS TotalAllowance,
    SUM(sd.Bonus) AS TotalBonus,
    SUM(sd.OvertimePay) AS TotalOvertimePay,
    SUM(sd.Deduction) AS TotalDeduction,
    SUM(sd.Insurance) AS TotalInsurance,
    SUM(sd.Tax) AS TotalTax,
    SUM(sd.NetSalary) AS TotalNetSalary
FROM [dbo].[SalaryData] sd
LEFT JOIN [dbo].[Employees] e ON sd.EmployeeId = e.EmployeeId
LEFT JOIN [dbo].[Departments] d ON e.DepartmentId = d.DepartmentId
WHERE sd.Status = 'CONFIRMED'
GROUP BY sd.SalaryYear, sd.SalaryMonth, e.DepartmentId, d.DepartmentName;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢薪資報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/salary-reports/query`
- **說明**: 查詢薪資報表資料，支援多種報表類型
- **請求參數**:
  ```json
  {
    "reportType": "DETAIL", // DETAIL:明細, SUMMARY:統計, PAYMENT:發放
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "employeeId": "",
      "departmentId": "",
      "salaryYear": 2024,
      "salaryMonth": 1,
      "status": "CONFIRMED"
    }
  }
  ```
- **回應格式**: 同查詢列表格式

#### 3.1.2 匯出薪資報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/salary-reports/export`
- **說明**: 匯出薪資報表（Excel、PDF）
- **請求格式**: 同查詢參數，加上 `exportType` (EXCEL/PDF)
- **回應格式**: 檔案下載

#### 3.1.3 列印薪資報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/salary-reports/print`
- **說明**: 列印薪資報表
- **請求格式**: 同查詢參數
- **回應格式**: PDF 檔案

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 薪資報表查詢頁面 (`SalaryReportQuery.vue`)
- **路徑**: `/hr/salary-reports`
- **功能**: 顯示薪資報表查詢介面，支援多種報表類型選擇、查詢條件設定、報表列印與匯出

### 4.2 UI 元件設計

#### 4.2.1 報表類型選擇
- 明細報表 (DETAIL)
- 統計報表 (SUMMARY)
- 發放報表 (PAYMENT)

#### 4.2.2 查詢條件表單
- 員工編號/姓名
- 部門
- 年度/月份
- 狀態

#### 4.2.3 報表顯示區域
- 資料表格
- 圖表（統計報表）
- 列印/匯出按鈕

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立報表查詢視圖
- [ ] 建立索引優化

### 5.2 階段二: 後端開發 (3天)
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 報表匯出功能
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 報表顯示元件
- [ ] 匯出功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 8天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限（薪資報表需特殊權限）
- 必須實作薪資保密機制
- 敏感資料必須加密傳輸

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 查詢條件必須驗證
- 報表類型必須在允許範圍內

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢薪資報表成功
- [ ] 匯出Excel成功
- [ ] 匯出PDF成功
- [ ] 列印報表成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000/SYSH220_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH220_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH230_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSH2B0_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

