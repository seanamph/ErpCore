# SYSH220-SYSH2B0 - 新版本薪資報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH220-SYSH2B0系列（新版本）
- **功能名稱**: 新版本薪資報表查詢系列
- **功能描述**: 提供新版本薪資報表的查詢、列印、匯出功能，功能與SYSH220-SYSH2B0-薪資報表查詢系列相同，但使用新的資料結構和API設計
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH220_PR.ASP` (報表)

### 1.2 業務需求
- 與SYSH220-SYSH2B0-薪資報表查詢系列功能相同
- 使用新的資料結構設計
- 支援更靈活的報表查詢條件

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考 SYSH220-SYSH2B0-薪資報表查詢系列.md 的視圖設計，但使用新的視圖名稱：
- `v_SalaryReportDetailV2` - 新版本薪資明細報表視圖
- `v_SalaryReportSummaryV2` - 新版本薪資統計報表視圖

---

## 三、後端 API 設計

### 3.1 API 端點列表

與SYSH220-SYSH2B0-薪資報表查詢系列相同，但使用新的API路徑：
- `/api/v2/salary-reports` - 新版本API路徑

---

## 四、前端 UI 設計

### 4.1 頁面結構

與SYSH220-SYSH2B0-薪資報表查詢系列相同，但使用新的頁面路徑：
- `/hr/v2/salary-reports` - 新版本頁面路徑

---

## 五、開發時程

與SYSH220-SYSH2B0-薪資報表查詢系列相同，**總計**: 8天

---

## 六、注意事項

與SYSH220-SYSH2B0-薪資報表查詢系列相同

---

## 七、測試案例

與SYSH220-SYSH2B0-薪資報表查詢系列相同

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH220_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH220_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

**備註**: 本功能與SYSH220-SYSH2B0-薪資報表查詢系列功能相同，但使用新的資料結構和API設計。詳細的schema、ui、api設計請參考SYSH220-SYSH2B0-薪資報表查詢系列.md文件。

