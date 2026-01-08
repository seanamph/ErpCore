# SYSP271-SYSP2B0 - 付款單維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP271-SYSP2B0系列
- **功能名稱**: 付款單維護系列
- **功能描述**: 提供付款單資料的新增、修改、刪除、查詢功能，包含付款單號、付款日期、供應商、付款金額、付款方式等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理付款單基本資料
- 支援多種付款方式
- 支援付款審核流程
- 支援付款異動記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PaymentVouchers` (對應舊系統 `PAYMENT_VOUCHER`)

```sql
CREATE TABLE [dbo].[PaymentVouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PaymentNo] NVARCHAR(50) NOT NULL, -- 付款單號 (PAYMENT_NO)
    [PaymentDate] DATETIME2 NOT NULL, -- 付款日期 (PAYMENT_DATE)
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
    [PaymentAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 付款金額 (PAYMENT_AMOUNT)
    [PaymentMethod] NVARCHAR(20) NULL, -- 付款方式 (PAYMENT_METHOD)
    [BankAccount] NVARCHAR(50) NULL, -- 銀行帳號 (BANK_ACCOUNT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'DRAFT', -- 狀態 (STATUS, DRAFT:草稿, CONFIRMED:確認, PAID:已付款)
    [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
    [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PaymentVouchers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PaymentVouchers_PaymentNo] UNIQUE ([PaymentNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_PaymentNo] ON [dbo].[PaymentVouchers] ([PaymentNo]);
CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_SupplierId] ON [dbo].[PaymentVouchers] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_PaymentDate] ON [dbo].[PaymentVouchers] ([PaymentDate]);
CREATE NONCLUSTERED INDEX [IX_PaymentVouchers_Status] ON [dbo].[PaymentVouchers] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢付款單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/payment-vouchers`
- **說明**: 查詢付款單列表，支援分頁、排序、篩選

#### 3.1.2 查詢單筆付款單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/payment-vouchers/{paymentNo}`
- **說明**: 根據付款單號查詢單筆付款單資料

#### 3.1.3 新增付款單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/payment-vouchers`
- **說明**: 新增付款單資料

#### 3.1.4 修改付款單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/payment-vouchers/{paymentNo}`
- **說明**: 修改付款單資料（僅限草稿狀態）

#### 3.1.5 刪除付款單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/payment-vouchers/{paymentNo}`
- **說明**: 刪除付款單資料（僅限草稿狀態）

#### 3.1.6 確認付款單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/payment-vouchers/{paymentNo}/confirm`
- **說明**: 確認付款單（狀態變更為CONFIRMED）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 付款單列表頁面 (`PaymentVoucherList.vue`)
- **路徑**: `/procurement/payment-vouchers`
- **功能**: 顯示付款單列表，支援查詢、新增、修改、刪除、確認

---

## 五、開發時程

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

### 6.2 資料驗證
- 付款單號必須唯一
- 付款金額必須為正數
- 供應商必須存在

### 6.3 業務邏輯
- 僅草稿狀態的資料可修改/刪除
- 確認後的資料不可修改

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增付款單成功
- [ ] 修改付款單成功
- [ ] 刪除付款單成功
- [ ] 確認付款單成功
- [ ] 查詢付款單列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP271_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

