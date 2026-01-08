# SYSG110-SYSG190 - 發票資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG110-SYSG190 系列
- **功能名稱**: 發票資料維護系列
- **功能描述**: 提供發票基本資料的新增、修改、刪除、查詢功能，包含發票編號、發票日期、發票類型、稅籍資料、發票格式、總公司發票基本資料等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_FB.ASP` (瀏覽 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_FI.ASP` (新增 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_FU.ASP` (修改 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_FD.ASP` (刪除 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_FQ.ASP` (查詢 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG110_PR.ASP` (報表 - 稅籍資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG120_*.ASP` (發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12A_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12B_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12C_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12D_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12E_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12F_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12G_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG12H_*.ASP` (總公司發票基本資料維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG130_*.ASP` (發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG140_*.ASP` (發票格式代號維護)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG150_*.ASP` (發票相關功能)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG190_*.ASP` (發票相關功能)

### 1.2 業務需求
- 管理發票基本資料資訊
- 支援發票的新增、修改、刪除、查詢
- 記錄發票的建立與變更資訊
- 支援發票類型設定（統一發票、電子發票等）
- 支援稅籍資料維護
- 支援發票格式代號維護
- 支援總公司發票基本資料維護
- 支援發票年月管理
- 支援發票號碼區間管理
- 支援發票狀態管理（啟用/停用）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Invoices` (發票主檔)

```sql
CREATE TABLE [dbo].[Invoices] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票編號
    [InvoiceType] NVARCHAR(20) NOT NULL, -- 發票類型 (INV_TYPE, 01:統一發票, 02:電子發票, 03:收據)
    [InvoiceYear] INT NOT NULL, -- 發票年份 (INV_YEAR)
    [InvoiceMonth] INT NOT NULL, -- 發票月份 (INV_MONTH)
    [InvoiceYm] NVARCHAR(6) NOT NULL, -- 發票年月 (INV_YM, YYYYMM格式)
    [Track] NVARCHAR(2) NULL, -- 字軌 (TRACK)
    [InvoiceNoB] NVARCHAR(10) NULL, -- 發票號碼起 (INV_NO_B)
    [InvoiceNoE] NVARCHAR(10) NULL, -- 發票號碼迄 (INV_NO_E)
    [InvoiceFormat] NVARCHAR(50) NULL, -- 發票格式代號 (INV_FORMAT)
    [TaxId] NVARCHAR(20) NULL, -- 統一編號 (TAX_ID)
    [CompanyName] NVARCHAR(200) NULL, -- 公司名稱 (COMPANY_NAME)
    [CompanyNameEn] NVARCHAR(200) NULL, -- 公司英文名稱
    [Address] NVARCHAR(500) NULL, -- 地址 (ADDRESS)
    [City] NVARCHAR(50) NULL, -- 城市 (CITY)
    [Zone] NVARCHAR(50) NULL, -- 區域 (ZONE)
    [PostalCode] NVARCHAR(20) NULL, -- 郵遞區號 (POSTAL_CODE)
    [Phone] NVARCHAR(50) NULL, -- 電話 (PHONE)
    [Fax] NVARCHAR(50) NULL, -- 傳真 (FAX)
    [Email] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [SubCopy] NVARCHAR(10) NULL, -- 副聯 (SUB_COPY)
    [SubCopyValue] NVARCHAR(100) NULL, -- 副聯值 (SUB_COPY_VALUE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Notes] NVARCHAR(1000) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Invoices_InvoiceId] UNIQUE ([InvoiceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceId] ON [dbo].[Invoices] ([InvoiceId]);
CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceType] ON [dbo].[Invoices] ([InvoiceType]);
CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceYm] ON [dbo].[Invoices] ([InvoiceYm]);
CREATE NONCLUSTERED INDEX [IX_Invoices_TaxId] ON [dbo].[Invoices] ([TaxId]);
CREATE NONCLUSTERED INDEX [IX_Invoices_SiteId] ON [dbo].[Invoices] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `InvoiceFormats` - 發票格式代號
```sql
CREATE TABLE [dbo].[InvoiceFormats] (
    [FormatId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [FormatName] NVARCHAR(100) NOT NULL,
    [FormatNameEn] NVARCHAR(100) NULL,
    [Description] NVARCHAR(500) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.2 `TaxRegistrations` - 稅籍資料
```sql
CREATE TABLE [dbo].[TaxRegistrations] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TaxId] NVARCHAR(20) NOT NULL, -- 統一編號
    [CompanyName] NVARCHAR(200) NOT NULL, -- 公司名稱
    [CompanyNameEn] NVARCHAR(200) NULL, -- 公司英文名稱
    [Address] NVARCHAR(500) NULL, -- 地址
    [City] NVARCHAR(50) NULL, -- 城市
    [Zone] NVARCHAR(50) NULL, -- 區域
    [PostalCode] NVARCHAR(20) NULL, -- 郵遞區號
    [Phone] NVARCHAR(50) NULL, -- 電話
    [Fax] NVARCHAR(50) NULL, -- 傳真
    [Email] NVARCHAR(100) NULL, -- 電子郵件
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_TaxRegistrations_TaxId] UNIQUE ([TaxId])
);
```

### 2.3 資料字典

#### 2.3.1 Invoices 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| InvoiceId | NVARCHAR | 50 | NO | - | 發票編號 | 唯一 |
| InvoiceType | NVARCHAR | 20 | NO | - | 發票類型 | 01:統一發票, 02:電子發票, 03:收據 |
| InvoiceYear | INT | - | NO | - | 發票年份 | - |
| InvoiceMonth | INT | - | NO | - | 發票月份 | - |
| InvoiceYm | NVARCHAR | 6 | NO | - | 發票年月 | YYYYMM格式 |
| Track | NVARCHAR | 2 | YES | - | 字軌 | - |
| InvoiceNoB | NVARCHAR | 10 | YES | - | 發票號碼起 | - |
| InvoiceNoE | NVARCHAR | 10 | YES | - | 發票號碼迄 | - |
| InvoiceFormat | NVARCHAR | 50 | YES | - | 發票格式代號 | 外鍵至InvoiceFormats |
| TaxId | NVARCHAR | 20 | YES | - | 統一編號 | 外鍵至TaxRegistrations |
| CompanyName | NVARCHAR | 200 | YES | - | 公司名稱 | - |
| CompanyNameEn | NVARCHAR | 200 | YES | - | 公司英文名稱 | - |
| Address | NVARCHAR | 500 | YES | - | 地址 | - |
| City | NVARCHAR | 50 | YES | - | 城市 | - |
| Zone | NVARCHAR | 50 | YES | - | 區域 | - |
| PostalCode | NVARCHAR | 20 | YES | - | 郵遞區號 | - |
| Phone | NVARCHAR | 50 | YES | - | 電話 | - |
| Fax | NVARCHAR | 50 | YES | - | 傳真 | - |
| Email | NVARCHAR | 100 | YES | - | 電子郵件 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |
| SubCopy | NVARCHAR | 10 | YES | - | 副聯 | - |
| SubCopyValue | NVARCHAR | 100 | YES | - | 副聯值 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Notes | NVARCHAR | 1000 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢發票列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/invoices`
- **說明**: 查詢發票列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "InvoiceId",
    "sortOrder": "ASC",
    "filters": {
      "invoiceId": "",
      "invoiceType": "",
      "invoiceYm": "",
      "taxId": "",
      "siteId": "",
      "status": ""
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "tKey": 1,
          "invoiceId": "INV001",
          "invoiceType": "01",
          "invoiceYear": 2024,
          "invoiceMonth": 1,
          "invoiceYm": "202401",
          "track": "AA",
          "invoiceNoB": "00000001",
          "invoiceNoE": "00000100",
          "invoiceFormat": "FORMAT001",
          "taxId": "12345678",
          "companyName": "測試公司",
          "status": "A"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20
    }
  }
  ```

#### 3.1.2 查詢單筆發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/invoices/{tKey}`
- **說明**: 查詢單筆發票資料
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "invoiceId": "INV001",
      "invoiceType": "01",
      "invoiceYear": 2024,
      "invoiceMonth": 1,
      "invoiceYm": "202401",
      "track": "AA",
      "invoiceNoB": "00000001",
      "invoiceNoE": "00000100",
      "invoiceFormat": "FORMAT001",
      "taxId": "12345678",
      "companyName": "測試公司",
      "companyNameEn": "Test Company",
      "address": "台北市信義區信義路五段7號",
      "city": "台北市",
      "zone": "信義區",
      "postalCode": "110",
      "phone": "02-12345678",
      "fax": "02-12345679",
      "email": "test@example.com",
      "siteId": "SITE001",
      "subCopy": "1",
      "subCopyValue": "副聯1",
      "status": "A",
      "notes": "備註",
      "createdBy": "USER001",
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedBy": "USER001",
      "updatedAt": "2024-01-01T00:00:00Z"
    }
  }
  ```

#### 3.1.3 新增發票
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/invoices`
- **說明**: 新增發票資料
- **請求格式**:
  ```json
  {
    "invoiceId": "INV001",
    "invoiceType": "01",
    "invoiceYear": 2024,
    "invoiceMonth": 1,
    "invoiceYm": "202401",
    "track": "AA",
    "invoiceNoB": "00000001",
    "invoiceNoE": "00000100",
    "invoiceFormat": "FORMAT001",
    "taxId": "12345678",
    "companyName": "測試公司",
    "companyNameEn": "Test Company",
    "address": "台北市信義區信義路五段7號",
    "city": "台北市",
    "zone": "信義區",
    "postalCode": "110",
    "phone": "02-12345678",
    "fax": "02-12345679",
    "email": "test@example.com",
    "siteId": "SITE001",
    "subCopy": "1",
    "subCopyValue": "副聯1",
    "status": "A",
    "notes": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1,
      "invoiceId": "INV001"
    }
  }
  ```

#### 3.1.4 修改發票
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/invoices/{tKey}`
- **說明**: 修改發票資料
- **請求格式**: 同新增格式
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "修改成功",
    "data": {
      "tKey": 1,
      "invoiceId": "INV001"
    }
  }
  ```

#### 3.1.5 刪除發票
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/invoices/{tKey}`
- **說明**: 刪除發票資料
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功"
  }
  ```

### 3.2 業務邏輯

#### 3.2.1 新增邏輯
1. 驗證發票編號唯一性
2. 驗證發票年月格式（YYYYMM）
3. 驗證發票號碼區間（起號 <= 迄號）
4. 驗證統一編號格式（8位數字）
5. 自動產生發票年月（InvoiceYm = InvoiceYear + InvoiceMonth）
6. 記錄建立者與建立時間

#### 3.2.2 修改邏輯
1. 驗證發票編號唯一性（排除自己）
2. 驗證發票年月格式
3. 驗證發票號碼區間
4. 驗證統一編號格式
5. 記錄更新者與更新時間

#### 3.2.3 刪除邏輯
1. 檢查是否有關聯的發票使用記錄
2. 軟刪除（將狀態設為 'I'）或硬刪除（根據業務需求）

#### 3.2.4 查詢邏輯
1. 支援多條件組合查詢
2. 支援分頁查詢
3. 支援排序
4. 支援模糊搜尋（發票編號、公司名稱等）

---

## 四、前端 UI 設計

### 4.1 發票列表頁面

#### 4.1.1 頁面結構
- **標題**: 發票資料維護
- **查詢區塊**: 
  - 發票編號（文字輸入）
  - 發票類型（下拉選單）
  - 發票年月（日期選擇器）
  - 統一編號（文字輸入）
  - 分公司代碼（下拉選單）
  - 狀態（下拉選單：全部、啟用、停用）
  - 查詢按鈕、重置按鈕
- **操作按鈕區塊**:
  - 新增按鈕
  - 修改按鈕
  - 刪除按鈕
  - 匯出按鈕
  - 列印按鈕
- **資料表格區塊**:
  - 序號
  - 發票編號
  - 發票類型
  - 發票年月
  - 字軌
  - 發票號碼起
  - 發票號碼迄
  - 統一編號
  - 公司名稱
  - 狀態
  - 操作（查看、修改、刪除）

#### 4.1.2 元件設計
- 使用 Element Plus 的 `el-table` 元件顯示列表
- 使用 `el-pagination` 元件實現分頁
- 使用 `el-form` 元件實現查詢表單
- 使用 `el-dialog` 元件實現新增/修改對話框

### 4.2 發票新增/修改對話框

#### 4.2.1 表單欄位
- **基本資料區塊**:
  - 發票編號（必填，文字輸入）
  - 發票類型（必填，下拉選單）
  - 發票年份（必填，數字輸入）
  - 發票月份（必填，數字輸入，1-12）
  - 字軌（選填，文字輸入）
  - 發票號碼起（選填，文字輸入）
  - 發票號碼迄（選填，文字輸入）
  - 發票格式代號（選填，下拉選單）
- **稅籍資料區塊**:
  - 統一編號（選填，文字輸入，8位數字）
  - 公司名稱（選填，文字輸入）
  - 公司英文名稱（選填，文字輸入）
  - 地址（選填，文字輸入）
  - 城市（選填，下拉選單）
  - 區域（選填，下拉選單）
  - 郵遞區號（選填，文字輸入）
  - 電話（選填，文字輸入）
  - 傳真（選填，文字輸入）
  - 電子郵件（選填，文字輸入，Email格式驗證）
- **其他資料區塊**:
  - 分公司代碼（選填，下拉選單）
  - 副聯（選填，文字輸入）
  - 副聯值（選填，文字輸入）
  - 狀態（必填，下拉選單：啟用、停用）
  - 備註（選填，多行文字輸入）

#### 4.2.2 驗證規則
- 發票編號：必填，唯一性驗證
- 發票類型：必填
- 發票年份：必填，範圍 1900-2100
- 發票月份：必填，範圍 1-12
- 發票號碼起：如果填寫，必須 <= 發票號碼迄
- 統一編號：如果填寫，必須為8位數字
- 電子郵件：如果填寫，必須符合Email格式

### 4.3 發票刪除確認對話框

#### 4.3.1 確認訊息
- 顯示要刪除的發票編號
- 提示刪除後無法復原
- 確認按鈕、取消按鈕

---

## 五、開發時程

### 5.1 後端開發（3天）
- **第1天**: 資料庫設計與建立、Repository 層開發
- **第2天**: Service 層開發、API Controller 開發
- **第3天**: 單元測試、API 測試

### 5.2 前端開發（3天）
- **第1天**: 列表頁面開發、查詢功能開發
- **第2天**: 新增/修改對話框開發、表單驗證
- **第3天**: 刪除功能開發、整合測試

### 5.3 整合測試（1天）
- 前後端整合測試
- 功能測試
- 效能測試

**總計**: 7天

---

## 六、注意事項

### 6.1 資料驗證
- 發票編號必須唯一
- 發票年月格式必須為 YYYYMM
- 發票號碼區間必須合理（起號 <= 迄號）
- 統一編號必須為8位數字

### 6.2 權限控制
- 新增權限：需要有發票新增權限
- 修改權限：需要有發票修改權限
- 刪除權限：需要有發票刪除權限
- 查詢權限：需要有發票查詢權限

### 6.3 效能優化
- 查詢列表時使用分頁，避免一次載入過多資料
- 建立適當的索引以提升查詢效能
- 使用快取機制快取常用資料（如發票格式代號、分公司代碼等）

### 6.4 錯誤處理
- 新增/修改時驗證資料完整性
- 刪除時檢查是否有關聯資料
- 提供明確的錯誤訊息

### 6.5 資料備份
- 刪除操作建議使用軟刪除（將狀態設為停用）
- 如需硬刪除，必須先備份資料

---

## 七、測試案例

### 7.1 新增測試
1. **測試案例1**: 正常新增發票
   - 輸入完整的發票資料
   - 預期結果：新增成功，返回新增的發票資料

2. **測試案例2**: 發票編號重複
   - 輸入已存在的發票編號
   - 預期結果：新增失敗，提示發票編號已存在

3. **測試案例3**: 必填欄位未填
   - 不填寫必填欄位
   - 預期結果：新增失敗，提示必填欄位未填

### 7.2 修改測試
1. **測試案例1**: 正常修改發票
   - 修改發票資料
   - 預期結果：修改成功，返回修改後的發票資料

2. **測試案例2**: 發票編號重複
   - 修改為已存在的發票編號
   - 預期結果：修改失敗，提示發票編號已存在

### 7.3 刪除測試
1. **測試案例1**: 正常刪除發票
   - 刪除發票資料
   - 預期結果：刪除成功

2. **測試案例2**: 刪除有關聯資料的發票
   - 刪除有使用記錄的發票
   - 預期結果：刪除失敗，提示有關聯資料

### 7.4 查詢測試
1. **測試案例1**: 正常查詢發票列表
   - 查詢發票列表
   - 預期結果：返回發票列表

2. **測試案例2**: 條件查詢
   - 使用多個條件查詢
   - 預期結果：返回符合條件的發票列表

3. **測試案例3**: 分頁查詢
   - 查詢第2頁資料
   - 預期結果：返回第2頁的發票列表

---

## 八、參考資料

### 8.1 舊程式檔案
- `WEB/IMS_CORE/ASP/SYSG000/SYSG110_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG120_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12A_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12B_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12C_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12D_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12E_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12F_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12G_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG12H_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG130_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG140_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG150_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG190_*.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSG000/SYSG110.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

