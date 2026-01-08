# SYSN220 - 報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN220
- **功能名稱**: 報表列印作業
- **功能描述**: 提供報表模組N的報表列印功能，使用Crystal Reports進行報表列印，支援PDF格式列印、報表預覽、列印設定等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSN000/SYSN220_PR.aspx` (報表列印頁面)
  - `WEB/IMS_CORE/SYSN000/SYSN220_PR.rpt` (Crystal Reports報表定義)
  - `WEB/IMS_CORE/SYSN000/SYSN220_PR.xsd` (資料結構定義)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_PR.ASP` (ASP版本報表列印)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FS.ASP` (儲存)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FT.ASP` (傳輸)

### 1.2 業務需求
- 提供報表列印功能
- 支援PDF格式列印
- 支援報表預覽
- 支援列印設定（頁面大小、邊界等）
- 支援報表資料查詢與篩選
- 記錄報表列印記錄
- 支援報表匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportPrints` (報表列印記錄)
參考 `SYS7B10-報表列印作業.md` 的 `ReportPrints` 資料表設計，使用相同的資料表結構，透過 `ReportCode` 欄位區分不同報表。

### 2.2 相關資料表
參考 `SYSN210-水費轉檔維護作業.md` 的相關資料表（如 `WaterBills`）

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysn000/sysn220/prints`
- **說明**: 查詢報表列印記錄列表，支援分頁、排序、篩選

#### 3.1.2 執行報表列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysn000/sysn220/print`
- **說明**: 執行報表列印，產生PDF檔案

#### 3.1.3 查詢單筆報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysn000/sysn220/prints/{printId}`
- **說明**: 根據列印ID查詢單筆報表列印記錄

#### 3.1.4 下載報表檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysn000/sysn220/prints/{printId}/download`
- **說明**: 下載報表檔案

#### 3.1.5 預覽報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysn000/sysn220/preview`
- **說明**: 預覽報表（不產生檔案，直接返回預覽資料）

#### 3.1.6 刪除報表列印記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sysn000/sysn220/prints/{printId}`
- **說明**: 刪除報表列印記錄及相關檔案

---

## 四、前端 UI 設計

### 4.1 報表列印頁面 (`SYSN220Print.vue`)
參考 `SYS7B10-報表列印作業.md` 的前端 UI 設計，將報表代碼改為 `SYSN220`

---

## 五、後端實作

### 5.1 Controller (`SYSN220ReportPrintController.cs`)
參考 `SYS7B10-報表列印作業.md` 的 Controller 設計，將報表代碼改為 `SYSN220`

### 5.2 Service (`ReportPrintService.cs`)
參考 `SYS7B10-報表列印作業.md` 的 Service 設計

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
2. **後端 API 開發** (2 天)
3. **前端 UI 開發** (1.5 天)
4. **測試與優化** (0.5 天)

**總計**: 4.5 天

---

## 七、注意事項
參考 `SYS7B10-報表列印作業.md`

---

## 八、測試案例
參考 `SYS7B10-報表列印作業.md`

