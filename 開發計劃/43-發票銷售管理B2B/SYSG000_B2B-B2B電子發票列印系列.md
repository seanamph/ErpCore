# SYSG000_B2B - B2B電子發票列印系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG000_B2B 系列
- **功能名稱**: B2B電子發票列印系列
- **功能描述**: 提供B2B電子發票列印功能，包含B2B電子發票手動取號列印、B2B電子發票中獎清冊、B2B電子發票列印設定等作業。此模組為SYSG210-SYSG2B0的B2B版本，功能類似但針對B2B場景優化
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP` (BREG發票列印功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP` (CTSG發票列印功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP` (EINB發票列印功能)
  - `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP` (EING發票列印功能)

### 1.2 業務需求
- 支援B2B電子發票手動取號列印
- 支援B2B電子發票中獎清冊查詢與列印
- 支援B2B電子發票列印設定
- 支援B2B電子發票條碼列印
- 支援B2B電子發票批次列印
- 支援B2B電子發票預覽功能
- 支援多種B2B電子發票格式（A4、A5、熱感紙等）
- 支援B2B電子發票QR Code列印
- 支援B2B發票傳輸功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `B2BElectronicInvoices` (B2B電子發票主檔)

```sql
CREATE TABLE [dbo].[B2BElectronicInvoices] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票編號
    [PosId] NVARCHAR(50) NULL, -- POS代碼
    [InvYm] NVARCHAR(6) NOT NULL, -- 發票年月 (YYYYMM)
    [Track] NVARCHAR(2) NULL, -- 字軌
    [InvNoB] NVARCHAR(10) NULL, -- 發票號碼起
    [InvNoE] NVARCHAR(10) NULL, -- 發票號碼迄
    [PrintCode] NVARCHAR(50) NULL, -- 列印條碼
    [InvoiceDate] DATETIME2 NULL, -- 發票日期
    [PrizeType] NVARCHAR(10) NULL, -- 獎項類型
    [PrizeAmt] DECIMAL(18, 4) NULL, -- 獎項金額
    [CarrierIdClear] NVARCHAR(50) NULL, -- 載具識別碼（明碼）
    [AwardPrint] NVARCHAR(10) NULL, -- 中獎列印標記
    [AwardPos] NVARCHAR(50) NULL, -- 中獎POS
    [AwardDate] DATETIME2 NULL, -- 中獎日期
    [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y', -- B2B標記
    [TransferType] NVARCHAR(20) NULL, -- 傳輸類型 (BREG, CTSG, EINB, EING等)
    [TransferStatus] NVARCHAR(20) NULL, -- 傳輸狀態
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_B2BElectronicInvoices] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_B2BElectronicInvoices_InvoiceId] UNIQUE ([InvoiceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_InvoiceId] ON [dbo].[B2BElectronicInvoices] ([InvoiceId]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_InvYm] ON [dbo].[B2BElectronicInvoices] ([InvYm]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_Track] ON [dbo].[B2BElectronicInvoices] ([Track]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_PrintCode] ON [dbo].[B2BElectronicInvoices] ([PrintCode]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_PrizeType] ON [dbo].[B2BElectronicInvoices] ([PrizeType]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_B2BFlag] ON [dbo].[B2BElectronicInvoices] ([B2BFlag]);
CREATE NONCLUSTERED INDEX [IX_B2BElectronicInvoices_TransferType] ON [dbo].[B2BElectronicInvoices] ([TransferType]);
```

### 2.2 相關資料表

#### 2.2.1 `B2BElectronicInvoicePrintSettings` - B2B電子發票列印設定
```sql
CREATE TABLE [dbo].[B2BElectronicInvoicePrintSettings] (
    [SettingId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PrintFormat] NVARCHAR(20) NOT NULL, -- 列印格式 (A4, A5, THERMAL)
    [BarcodeType] NVARCHAR(20) NULL, -- 條碼類型
    [BarcodeSize] INT NULL DEFAULT 40,
    [BarcodeMargin] INT NULL DEFAULT 5,
    [ColCount] INT NULL DEFAULT 2,
    [PageCount] INT NULL DEFAULT 14,
    [B2BFlag] NVARCHAR(10) NOT NULL DEFAULT 'Y',
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢B2B電子發票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-electronic-invoices`
- **說明**: 查詢B2B電子發票列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆B2B電子發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-electronic-invoices/{tKey}`
- **說明**: 查詢單筆B2B電子發票資料

#### 3.1.3 新增B2B電子發票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-electronic-invoices`
- **說明**: 新增B2B電子發票資料

#### 3.1.4 修改B2B電子發票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/b2b-electronic-invoices/{tKey}`
- **說明**: 修改B2B電子發票資料

#### 3.1.5 刪除B2B電子發票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/b2b-electronic-invoices/{tKey}`
- **說明**: 刪除B2B電子發票資料

#### 3.1.6 B2B電子發票列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-electronic-invoices/{tKey}/print`
- **說明**: 執行B2B電子發票列印作業

#### 3.1.7 B2B電子發票批次列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/b2b-electronic-invoices/batch-print`
- **說明**: 批次列印多筆B2B電子發票

#### 3.1.8 B2B電子發票中獎清冊查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/b2b-electronic-invoices/prize-list`
- **說明**: 查詢B2B電子發票中獎清冊

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 B2B電子發票列表頁面 (`B2BElectronicInvoiceList.vue`)
- **路徑**: `/b2b-electronic-invoices`
- **功能**: 顯示B2B電子發票列表，支援查詢、新增、修改、刪除、列印

#### 4.1.2 B2B電子發票列印頁面 (`B2BElectronicInvoicePrint.vue`)
- **路徑**: `/b2b-electronic-invoices/:tKey/print`
- **功能**: B2B電子發票列印功能

### 4.2 UI 元件設計

參考SYSG210-SYSG2B0-電子發票列印系列的UI設計，但針對B2B場景進行優化。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 列印功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 列印功能開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 列印功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- B2B電子發票資料必須與一般電子發票資料隔離

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 列印功能必須支援非同步處理

### 6.3 資料驗證
- 發票編號必須唯一
- 必填欄位必須驗證
- B2B標記必須為'Y'

### 6.4 業務邏輯
- B2B電子發票格式必須符合B2B業務規範
- 列印功能必須支援多種格式
- 傳輸功能必須記錄傳輸狀態

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增B2B電子發票成功
- [ ] 修改B2B電子發票成功
- [ ] 刪除B2B電子發票成功
- [ ] 查詢B2B電子發票列表成功
- [ ] B2B電子發票列印成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 列印功能測試
- [ ] 傳輸功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSG000_B2B/BREG410_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/CTSG120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EINB100_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000_B2B/EING120_*.ASP`

### 8.2 相關功能
- SYSG210-SYSG2B0-電子發票列印系列（一般電子發票列印功能）
- SYSG000_B2B-B2B發票資料維護系列（B2B發票資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

