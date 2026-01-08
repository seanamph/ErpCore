# SYSN510-SYSN511 - 財務報表查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN510-SYSN511系列
- **功能名稱**: 財務報表查詢系列
- **功能描述**: 提供財務報表的查詢功能，包含損益表、資產負債表、現金流量表等多種財務報表查詢
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN510_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN511_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多種財務報表類型查詢
- 支援多種查詢條件（年度、月份、會計科目等）
- 支援報表資料匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考會計科目、財務交易等相關資料表設計

### 2.2 報表查詢視圖

#### 2.2.1 `v_FinancialReportQuery` - 財務報表查詢視圖
```sql
CREATE VIEW [dbo].[v_FinancialReportQuery] AS
SELECT 
    -- 報表查詢所需欄位
    -- 根據報表類型動態組合
FROM [dbo].[FinancialTransactions] ft
LEFT JOIN [dbo].[AccountSubjects] asub ON ft.StypeId = asub.StypeId;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢財務報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/financial-reports/query`
- **說明**: 查詢財務報表資料

#### 3.1.2 匯出財務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-reports/export`
- **說明**: 匯出財務報表（Excel、PDF）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 財務報表查詢頁面 (`FinancialReportQuery.vue`)
- **路徑**: `/accounting/financial-reports/query`
- **功能**: 顯示財務報表查詢介面

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

### 6.1 業務邏輯
- 報表資料必須準確
- 報表格式必須符合會計準則

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢財務報表成功
- [ ] 匯出財務報表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN510_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN511_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

