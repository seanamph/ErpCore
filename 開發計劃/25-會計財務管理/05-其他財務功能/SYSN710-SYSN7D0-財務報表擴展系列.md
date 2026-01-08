# SYSN710-SYSN7D0 - 財務報表擴展系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN710-SYSN7D0系列
- **功能名稱**: 財務報表擴展系列
- **功能描述**: 提供財務報表擴展功能的查詢、列印、匯出功能，包含擴展報表類型、擴展報表參數等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN710_*.ASP`
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN7D0_*.ASP`

### 1.2 業務需求
- 支援擴展報表類型
- 支援擴展報表參數設定
- 支援擴展報表列印與匯出

---

## 二、資料庫設計 (Schema)

參考 SYSN510-SYSN511-財務報表查詢系列.md 的資料表設計

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢財務報表擴展
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/financial-reports/extended`
- **說明**: 查詢財務報表擴展資料

#### 3.1.2 匯出財務報表擴展
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-reports/extended/export`
- **說明**: 匯出財務報表擴展（Excel、PDF）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 財務報表擴展頁面 (`FinancialReportExtended.vue`)
- **路徑**: `/accounting/financial-reports/extended`
- **功能**: 顯示財務報表擴展介面

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

與SYSN510-SYSN511-財務報表查詢系列相同

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢財務報表擴展成功
- [ ] 匯出財務報表擴展成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN710_*.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN7D0_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

