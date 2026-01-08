# SYSN210-SYSN213 - 財務交易處理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN210-SYSN213系列
- **功能名稱**: 財務交易處理系列
- **功能描述**: 提供財務交易資料的新增、修改、刪除、查詢功能，包含交易單號、交易日期、交易類型、交易金額、會計科目、借貸方向等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理財務交易基本資料
- 支援多種交易類型（收入、支出、轉帳等）
- 支援借貸平衡檢查
- 支援交易審核流程
- 支援交易異動記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `FinancialTransactions` (對應舊系統 `FIN_TRANS`)

```sql
CREATE TABLE [dbo].[FinancialTransactions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TxnNo] NVARCHAR(50) NOT NULL, -- 交易單號 (TXN_NO)
    [TxnDate] DATETIME2 NOT NULL, -- 交易日期 (TXN_DATE)
    [TxnType] NVARCHAR(20) NOT NULL, -- 交易類型 (TXN_TYPE)
    [StypeId] NVARCHAR(50) NOT NULL, -- 會計科目 (STYPE_ID)
    [Dc] NVARCHAR(1) NOT NULL, -- 借貸方向 (DC, D:借方, C:貸方)
    [Amount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 交易金額 (AMOUNT)
    [Description] NVARCHAR(500) NULL, -- 交易說明 (DESCRIPTION)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, CONFIRMED:確認, POSTED:過帳)
    [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
    [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
    [PostedBy] NVARCHAR(50) NULL, -- 過帳者 (POSTED_BY)
    [PostedDate] DATETIME2 NULL, -- 過帳日期 (POSTED_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_FinancialTransactions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_FinancialTransactions_TxnNo] UNIQUE ([TxnNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnNo] ON [dbo].[FinancialTransactions] ([TxnNo]);
CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnDate] ON [dbo].[FinancialTransactions] ([TxnDate]);
CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_TxnType] ON [dbo].[FinancialTransactions] ([TxnType]);
CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_StypeId] ON [dbo].[FinancialTransactions] ([StypeId]);
CREATE NONCLUSTERED INDEX [IX_FinancialTransactions_Status] ON [dbo].[FinancialTransactions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `AccountSubjects` - 會計科目主檔
```sql
-- 參考會計科目主檔設計
-- 用於查詢會計科目資料
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| TxnNo | NVARCHAR | 50 | NO | - | 交易單號 | 唯一 |
| TxnDate | DATETIME2 | - | NO | - | 交易日期 | - |
| TxnType | NVARCHAR | 20 | NO | - | 交易類型 | - |
| StypeId | NVARCHAR | 50 | NO | - | 會計科目 | 外鍵至AccountSubjects |
| Dc | NVARCHAR | 1 | NO | - | 借貸方向 | D:借方, C:貸方 |
| Amount | DECIMAL | 18,2 | NO | 0 | 交易金額 | - |
| Description | NVARCHAR | 500 | YES | - | 交易說明 | - |
| Status | NVARCHAR | 10 | NO | 'DRAFT' | 狀態 | DRAFT:草稿, CONFIRMED:確認, POSTED:過帳 |
| Verifier | NVARCHAR | 50 | YES | - | 審核者 | - |
| VerifyDate | DATETIME2 | - | YES | - | 審核日期 | - |
| PostedBy | NVARCHAR | 50 | YES | - | 過帳者 | - |
| PostedDate | DATETIME2 | - | YES | - | 過帳日期 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢財務交易列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/financial-transactions`
- **說明**: 查詢財務交易列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應

#### 3.1.2 查詢單筆財務交易
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/financial-transactions/{tKey}`
- **說明**: 根據主鍵查詢單筆財務交易資料

#### 3.1.3 新增財務交易
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-transactions`
- **說明**: 新增財務交易資料（需檢查借貸平衡）

#### 3.1.4 修改財務交易
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/financial-transactions/{tKey}`
- **說明**: 修改財務交易資料（僅限草稿狀態）

#### 3.1.5 刪除財務交易
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/financial-transactions/{tKey}`
- **說明**: 刪除財務交易資料（僅限草稿狀態）

#### 3.1.6 確認財務交易
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-transactions/{tKey}/confirm`
- **說明**: 確認財務交易（狀態變更為CONFIRMED）

#### 3.1.7 過帳財務交易
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-transactions/{tKey}/post`
- **說明**: 過帳財務交易（狀態變更為POSTED）

#### 3.1.8 檢查借貸平衡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-transactions/check-balance`
- **說明**: 檢查借貸平衡（不儲存）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 財務交易列表頁面 (`FinancialTransactionList.vue`)
- **路徑**: `/accounting/financial-transactions`
- **功能**: 顯示財務交易列表，支援查詢、新增、修改、刪除、確認、過帳

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件
- 交易單號
- 交易日期範圍
- 交易類型
- 會計科目
- 狀態

#### 4.2.2 資料表格元件
- 交易單號
- 交易日期
- 交易類型
- 會計科目
- 借貸方向
- 交易金額
- 狀態
- 操作按鈕

#### 4.2.3 新增/修改對話框
- 交易單號（自動產生或手動輸入）
- 交易日期
- 交易類型
- 會計科目
- 借貸方向
- 交易金額
- 交易說明
- 備註

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含借貸平衡檢查）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 交易單號必須唯一
- 會計科目必須存在
- 借貸方向必須在允許範圍內
- 交易金額必須為正數
- 借貸必須平衡

### 6.4 業務邏輯
- 僅草稿狀態的資料可修改/刪除
- 確認後的資料不可修改
- 過帳後的資料不可修改/刪除
- 借貸必須平衡才能確認/過帳

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增財務交易成功
- [ ] 新增財務交易失敗 (借貸不平衡)
- [ ] 修改財務交易成功
- [ ] 修改財務交易失敗 (非草稿狀態)
- [ ] 刪除財務交易成功
- [ ] 確認財務交易成功
- [ ] 過帳財務交易成功
- [ ] 檢查借貸平衡正確

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 借貸平衡檢查測試
- [ ] 狀態變更測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

