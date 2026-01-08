# SYSP210-SYSP260 - 供應商資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP210-SYSP260系列
- **功能名稱**: 供應商資料維護系列
- **功能描述**: 提供供應商資料的新增、修改、刪除、查詢功能，包含供應商編號、供應商名稱、聯絡人、地址、電話、付款條件等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理供應商基本資料
- 支援供應商分類管理
- 支援供應商評等管理
- 支援供應商異動記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Suppliers` (對應舊系統 `SUPPLIERS`)

```sql
CREATE TABLE [dbo].[Suppliers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SupplierId] NVARCHAR(50) NOT NULL, -- 供應商編號 (SUPPLIER_ID)
    [SupplierName] NVARCHAR(200) NOT NULL, -- 供應商名稱 (SUPPLIER_NAME)
    [SupplierNameE] NVARCHAR(200) NULL, -- 供應商英文名稱
    [ContactPerson] NVARCHAR(100) NULL, -- 聯絡人 (CONTACT_PERSON)
    [Phone] NVARCHAR(20) NULL, -- 電話 (PHONE)
    [Fax] NVARCHAR(20) NULL, -- 傳真 (FAX)
    [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
    [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
    [PaymentTerms] NVARCHAR(50) NULL, -- 付款條件 (PAYMENT_TERMS)
    [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Rating] NVARCHAR(10) NULL, -- 評等 (RATING)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Suppliers_SupplierId] UNIQUE ([SupplierId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Suppliers_SupplierId] ON [dbo].[Suppliers] ([SupplierId]);
CREATE NONCLUSTERED INDEX [IX_Suppliers_Status] ON [dbo].[Suppliers] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Suppliers_Rating] ON [dbo].[Suppliers] ([Rating]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢供應商列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/suppliers`
- **說明**: 查詢供應商列表，支援分頁、排序、篩選

#### 3.1.2 查詢單筆供應商
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/suppliers/{supplierId}`
- **說明**: 根據供應商編號查詢單筆供應商資料

#### 3.1.3 新增供應商
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/suppliers`
- **說明**: 新增供應商資料

#### 3.1.4 修改供應商
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/suppliers/{supplierId}`
- **說明**: 修改供應商資料

#### 3.1.5 刪除供應商
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/suppliers/{supplierId}`
- **說明**: 刪除供應商資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 供應商列表頁面 (`SupplierList.vue`)
- **路徑**: `/procurement/suppliers`
- **功能**: 顯示供應商列表，支援查詢、新增、修改、刪除

---

## 五、開發時程

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

### 6.2 資料驗證
- 供應商編號必須唯一
- 統一編號必須符合格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增供應商成功
- [ ] 修改供應商成功
- [ ] 刪除供應商成功
- [ ] 查詢供應商列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP210_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

