# SYSN320 - 報表統計作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN320
- **功能名稱**: 報表統計作業
- **功能描述**: 提供報表模組N的報表統計功能，支援資料統計、圖表展示、統計報表列印等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSN000/SYSN320_PR_P.rdlc` (報表定義-列印版)

### 1.2 業務需求
參考 `SYSN230-報表統計作業.md`

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportStatistics` (報表統計記錄)
參考 `SYS7C10-報表統計作業.md` 的 `ReportStatistics` 資料表設計，使用相同的資料表結構，透過 `ReportCode` 欄位區分不同報表。

---

## 三、後端 API 設計

### 3.1 API 端點列表
參考 `SYSN230-報表統計作業.md` 的 API 設計，將路徑改為 `/api/v1/reports/sysn000/sysn320/`

---

## 四、前端 UI 設計

### 4.1 報表統計頁面 (`SYSN320Statistics.vue`)
參考 `SYSN230-報表統計作業.md` 的前端 UI 設計，將報表代碼改為 `SYSN320`

---

## 五、後端實作

### 5.1 Controller (`SYSN320ReportStatisticsController.cs`)
參考 `SYSN230-報表統計作業.md` 的 Controller 設計，將報表代碼改為 `SYSN320`

### 5.2 Service (`ReportStatisticsService.cs`)
參考 `SYSN230-報表統計作業.md` 的 Service 設計

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
參考 `SYSN230-報表統計作業.md`

---

## 八、測試案例
參考 `SYSN230-報表統計作業.md`

