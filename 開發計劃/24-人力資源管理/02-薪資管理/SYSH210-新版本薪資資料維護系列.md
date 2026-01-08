# SYSH210 - 新版本薪資資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH210系列（新版本）
- **功能名稱**: 新版本薪資資料維護系列
- **功能描述**: 提供新版本員工薪資資料的新增、修改、刪除、查詢功能，功能與SYSH210-薪資資料維護系列相同，但使用新的資料結構和API設計
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FQ.ASP` (查詢)

### 1.2 業務需求
- 與SYSH210-薪資資料維護系列功能相同
- 使用新的資料結構設計
- 支援更靈活的薪資項目設定
- 支援更完善的薪資計算邏輯

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考 SYSH210-薪資資料維護系列.md 的資料表設計，但使用新的表名和結構：
- `SalaryDataV2` - 新版本薪資資料表
- `SalaryItemsV2` - 新版本薪資項目明細表

---

## 三、後端 API 設計

### 3.1 API 端點列表

與SYSH210-薪資資料維護系列相同，但使用新的API路徑：
- `/api/v2/salary-data` - 新版本API路徑

---

## 四、前端 UI 設計

### 4.1 頁面結構

與SYSH210-薪資資料維護系列相同，但使用新的頁面路徑：
- `/hr/v2/salary-data` - 新版本頁面路徑

---

## 五、開發時程

與SYSH210-薪資資料維護系列相同，**總計**: 11天

---

## 六、注意事項

與SYSH210-薪資資料維護系列相同

---

## 七、測試案例

與SYSH210-薪資資料維護系列相同

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSH000_NEW/SYSH210_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

**備註**: 本功能與SYSH210-薪資資料維護系列功能相同，但使用新的資料結構和API設計。詳細的schema、ui、api設計請參考SYSH210-薪資資料維護系列.md文件。

