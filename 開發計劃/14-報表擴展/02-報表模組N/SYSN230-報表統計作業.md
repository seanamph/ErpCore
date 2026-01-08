# SYSN230 - 報表統計作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN230
- **功能名稱**: 報表統計作業
- **功能描述**: 提供報表模組N的報表統計功能，支援資料統計、圖表展示、統計報表列印等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSN000/SYSN230_PR_P.rdlc` (報表定義-列印版)
  - `WEB/IMS_CORE/SYSN000/SYSN230_PR_PM.rdlc` (報表定義-列印版-多頁)

### 1.2 業務需求
- 提供報表資料統計功能
- 支援多種統計維度（日期、類別、區域等）
- 支援統計報表列印
- 支援統計資料匯出（Excel、PDF）
- 記錄統計查詢記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportStatistics` (報表統計記錄)
參考 `SYS7C10-報表統計作業.md` 的 `ReportStatistics` 資料表設計，使用相同的資料表結構，透過 `ReportCode` 欄位區分不同報表。

### 2.2 相關資料表
參考 `SYSN210-水費轉檔維護作業.md` 的相關資料表（如 `WaterBills`）

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表統計記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysn000/sysn230/statistics`
- **說明**: 查詢報表統計記錄列表，支援分頁、排序、篩選

#### 3.1.2 執行報表統計
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysn000/sysn230/statistics/calculate`
- **說明**: 執行報表統計，計算統計結果

#### 3.1.3 查詢單筆統計記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysn000/sysn230/statistics/{statId}`
- **說明**: 根據統計ID查詢單筆統計記錄

#### 3.1.4 匯出統計報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysn000/sysn230/statistics/{statId}/export`
- **說明**: 匯出統計報表（Excel、PDF）

#### 3.1.5 刪除統計記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sysn000/sysn230/statistics/{statId}`
- **說明**: 刪除統計記錄

---

## 四、前端 UI 設計

### 4.1 報表統計頁面 (`SYSN230Statistics.vue`)
參考 `SYS7C10-報表統計作業.md` 的前端 UI 設計，將報表代碼改為 `SYSN230`

---

## 五、後端實作

### 5.1 Controller (`SYSN230ReportStatisticsController.cs`)
參考 `SYS7C10-報表統計作業.md` 的 Controller 設計，將報表代碼改為 `SYSN230`

### 5.2 Service (`ReportStatisticsService.cs`)
參考 `SYS7C10-報表統計作業.md` 的 Service 設計

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
2. **後端 API 開發** (2.5 天)
3. **前端 UI 開發** (2 天)
4. **測試與優化** (0.5 天)

**總計**: 5.5 天

---

## 七、注意事項
參考 `SYS7C10-報表統計作業.md`

---

## 八、測試案例
參考 `SYS7C10-報表統計作業.md`

