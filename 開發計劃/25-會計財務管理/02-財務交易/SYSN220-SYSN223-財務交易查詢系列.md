# SYSN220-SYSN223 - 財務交易查詢系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN220-SYSN223系列
- **功能名稱**: 財務交易查詢系列
- **功能描述**: 提供財務交易資料的查詢功能，包含多種查詢條件、報表查詢、統計查詢等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN221_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN222_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN223_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多種查詢條件（交易單號、交易日期、交易類型、會計科目、狀態等）
- 支援報表查詢與列印
- 支援統計查詢
- 支援資料匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考 SYSN210-SYSN213-財務交易處理系列.md 的資料表設計

### 2.2 查詢視圖

#### 2.2.1 `v_FinancialTransactionQuery` - 財務交易查詢視圖
```sql
CREATE VIEW [dbo].[v_FinancialTransactionQuery] AS
SELECT 
    ft.TKey,
    ft.TxnNo,
    ft.TxnDate,
    ft.TxnType,
    ft.StypeId,
    asub.StypeName,
    ft.Dc,
    ft.Amount,
    ft.Description,
    ft.Status,
    ft.Verifier,
    ft.VerifyDate,
    ft.PostedBy,
    ft.PostedDate,
    ft.Notes,
    ft.CreatedBy,
    ft.CreatedAt,
    ft.UpdatedAt
FROM [dbo].[FinancialTransactions] ft
LEFT JOIN [dbo].[AccountSubjects] asub ON ft.StypeId = asub.StypeId;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢財務交易列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/financial-transactions/query`
- **說明**: 查詢財務交易列表，支援多種查詢條件
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應

#### 3.1.2 統計查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-transactions/statistics`
- **說明**: 財務交易統計查詢
- **請求格式**: 查詢條件
- **回應格式**: 統計結果

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 財務交易查詢頁面 (`FinancialTransactionQuery.vue`)
- **路徑**: `/accounting/financial-transactions/query`
- **功能**: 顯示財務交易查詢介面，支援多種查詢條件、報表查詢、統計查詢

### 4.2 UI 元件設計

#### 4.2.1 查詢條件表單
- 交易單號
- 交易日期範圍
- 交易類型
- 會計科目
- 借貸方向
- 狀態

#### 4.2.2 查詢結果表格
- 資料表格
- 統計資訊
- 匯出按鈕

---

## 五、開發時程

**總計**: 8天

---

## 六、注意事項

與SYSN210-SYSN213-財務交易處理系列相同

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢財務交易成功
- [ ] 統計查詢成功
- [ ] 匯出資料成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN220_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN221_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN222_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN223_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

