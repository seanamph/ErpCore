# SYSCUST_JGJN - 客戶定制JGJN模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSCUST_JGJN 系列
- **功能名稱**: 客戶定制JGJN模組系列
- **功能描述**: 提供JGJN客戶定制功能，包含SYS16系列查詢報表、客戶管理、發票列印等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1610_*.ASP` (SYS1610查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1620_*.ASP` (SYS1620查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1630_*.ASP` (SYS1630查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1640_*.ASP` (SYS1640查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1645_*.ASP` (SYS1645查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1646_*.ASP` (SYS1646查詢和報表功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYSC210_*.asp` (SYSC210客戶管理功能)
  - `WEB/IMS_CORE/ASP/SYSCUST_JGJN/PnInvoice_*.asp` (發票列印功能)

### 1.2 業務需求
- 提供SYS16系列查詢和報表功能
- 支援客戶資料維護功能
- 支援發票列印功能
- 支援多種報表格式（明細報表、商店報表等）
- 支援資料匯出功能

### 1.3 子模組清單
- **SYS1610**: 查詢和報表功能
- **SYS1620**: 查詢和報表功能
- **SYS1630**: 查詢和報表功能
- **SYS1640**: 查詢和報表功能
- **SYS1645**: 查詢和報表功能（包含明細和商店報表）
- **SYS1646**: 查詢和報表功能（包含明細和商店報表）
- **SYSC210**: 客戶管理功能（新增、修改、刪除、查詢）
- **PnInvoice**: 發票列印功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `JgjNData`

```sql
CREATE TABLE [dbo].[JgjNData] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
    [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼 (SYS1610, SYS1620, SYS1630, SYS1640, SYS1645, SYS1646, SYSC210等)
    [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
    [DataValue] NVARCHAR(MAX) NULL, -- 資料值
    [DataType] NVARCHAR(20) NULL, -- 資料類型
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [SortOrder] INT NULL DEFAULT 0, -- 排序順序
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_JgjNData_DataId_ModuleCode] UNIQUE ([DataId], [ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_JgjNData_ModuleCode] ON [dbo].[JgjNData] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_JgjNData_DataType] ON [dbo].[JgjNData] ([DataType]);
CREATE NONCLUSTERED INDEX [IX_JgjNData_Status] ON [dbo].[JgjNData] ([Status]);
```

### 2.2 客戶資料表: `JgjNCustomer`

```sql
CREATE TABLE [dbo].[JgjNCustomer] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CustomerId] NVARCHAR(50) NOT NULL, -- 客戶代碼
    [CustomerName] NVARCHAR(100) NOT NULL, -- 客戶名稱
    [CustomerType] NVARCHAR(20) NULL, -- 客戶類型
    [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人
    [Phone] NVARCHAR(50) NULL, -- 電話
    [Email] NVARCHAR(100) NULL, -- 電子郵件
    [Address] NVARCHAR(200) NULL, -- 地址
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_JgjNCustomer_CustomerId] UNIQUE ([CustomerId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_JgjNCustomer_CustomerName] ON [dbo].[JgjNCustomer] ([CustomerName]);
CREATE NONCLUSTERED INDEX [IX_JgjNCustomer_Status] ON [dbo].[JgjNCustomer] ([Status]);
```

### 2.3 發票資料表: `JgjNInvoice`

```sql
CREATE TABLE [dbo].[JgjNInvoice] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票代碼
    [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼
    [InvoiceDate] DATETIME2 NULL, -- 發票日期
    [CustomerId] NVARCHAR(50) NULL, -- 客戶代碼
    [Amount] DECIMAL(18,2) NULL, -- 金額
    [Currency] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING:待處理, PROCESSED:已處理, FAILED:失敗)
    [PrintStatus] NVARCHAR(20) NULL, -- 列印狀態
    [PrintDate] DATETIME2 NULL, -- 列印日期
    [FilePath] NVARCHAR(500) NULL, -- 檔案路徑
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_JgjNInvoice_InvoiceId] UNIQUE ([InvoiceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_CustomerId] ON [dbo].[JgjNInvoice] ([CustomerId]);
CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_InvoiceDate] ON [dbo].[JgjNInvoice] ([InvoiceDate]);
CREATE NONCLUSTERED INDEX [IX_JgjNInvoice_Status] ON [dbo].[JgjNInvoice] ([Status]);
```

### 2.4 資料字典

#### JgjNData 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| DataId | NVARCHAR | 50 | NO | - | 資料代碼 | 與ModuleCode組合唯一 |
| ModuleCode | NVARCHAR | 20 | NO | - | 模組代碼 | SYS1610, SYS1620等 |
| DataName | NVARCHAR | 100 | NO | - | 資料名稱 | - |
| DataValue | NVARCHAR(MAX) | - | YES | - | 資料值 | - |
| DataType | NVARCHAR | 20 | YES | - | 資料類型 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SortOrder | INT | - | YES | 0 | 排序順序 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 查詢資料列表

```http
GET /api/v1/jgjn/data?moduleCode={moduleCode}&dataType={dataType}&status={status}&keyword={keyword}&page={page}&pageSize={pageSize}
```

**請求參數**:
- `moduleCode`: 模組代碼（選填，SYS1610, SYS1620, SYS1630, SYS1640, SYS1645, SYS1646, SYSC210等）
- `dataType`: 資料類型（選填）
- `status`: 狀態（選填，A:啟用, I:停用）
- `keyword`: 關鍵字搜尋（選填，搜尋資料代碼或名稱）
- `page`: 頁碼（預設1）
- `pageSize`: 每頁筆數（預設20）

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "dataId": "DATA001",
        "moduleCode": "SYS1610",
        "dataName": "資料名稱",
        "dataValue": "資料值",
        "dataType": "TYPE001",
        "status": "A",
        "sortOrder": 1,
        "memo": "備註",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedBy": "USER001",
        "updatedAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

### 3.2 新增資料

```http
POST /api/v1/jgjn/data
```

**請求體**:
```json
{
  "dataId": "DATA001",
  "moduleCode": "SYS1610",
  "dataName": "資料名稱",
  "dataValue": "資料值",
  "dataType": "TYPE001",
  "status": "A",
  "sortOrder": 1,
  "memo": "備註"
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "新增成功",
  "data": {
    "tKey": 1,
    "dataId": "DATA001",
    "moduleCode": "SYS1610",
    "dataName": "資料名稱",
    "status": "A",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

### 3.3 修改資料

```http
PUT /api/v1/jgjn/data/{tKey}
```

**請求體**:
```json
{
  "dataName": "修改後的資料名稱",
  "dataValue": "修改後的資料值",
  "dataType": "TYPE002",
  "status": "I",
  "sortOrder": 2,
  "memo": "修改備註"
}
```

### 3.4 刪除資料

```http
DELETE /api/v1/jgjn/data/{tKey}
```

### 3.5 查詢客戶列表

```http
GET /api/v1/jgjn/customers?customerType={customerType}&status={status}&keyword={keyword}&page={page}&pageSize={pageSize}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "customerId": "CUST001",
        "customerName": "客戶名稱",
        "customerType": "TYPE001",
        "contactPerson": "聯絡人",
        "phone": "電話",
        "email": "email@example.com",
        "address": "地址",
        "status": "A",
        "memo": "備註",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 50,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  }
}
```

### 3.6 新增客戶

```http
POST /api/v1/jgjn/customers
```

**請求體**:
```json
{
  "customerId": "CUST001",
  "customerName": "客戶名稱",
  "customerType": "TYPE001",
  "contactPerson": "聯絡人",
  "phone": "電話",
  "email": "email@example.com",
  "address": "地址",
  "status": "A",
  "memo": "備註"
}
```

### 3.7 發票列印

```http
POST /api/v1/jgjn/invoice/{invoiceId}/print
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "列印成功",
  "data": {
    "invoiceId": "INV001",
    "printStatus": "SUCCESS",
    "printDate": "2024-01-01T00:00:00Z",
    "filePath": "/files/invoices/inv001.pdf"
  }
}
```

### 3.8 報表查詢

```http
GET /api/v1/jgjn/reports/{moduleCode}?startDate={startDate}&endDate={endDate}&reportType={reportType}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "moduleCode": "SYS1645",
    "reportType": "DETAIL",
    "items": [
      {
        "reportData": "報表資料"
      }
    ],
    "total": 100
  }
}
```

---

## 四、前端 UI 設計

### 4.1 資料列表頁面

#### 4.1.1 路由配置
- **路由**: `/jgjn/data`
- **組件**: `JgjNDataList.vue`
- **權限**: `JGJN_DATA_VIEW`

#### 4.1.2 頁面結構
```
┌─────────────────────────────────────────┐
│  客戶定制JGJN模組資料維護                 │
├─────────────────────────────────────────┤
│  [模組] [類型] [狀態] [關鍵字] [查詢]      │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐  │
│  │ 資料代碼 │ 模組 │ 資料名稱 │ 狀態 │  │
│  ├───────────────────────────────────┤  │
│  │ DATA001 │SYS1610│ 資料名稱1 │ 啟用│  │
│  │ DATA002 │SYS1620│ 資料名稱2 │ 停用│  │
│  └───────────────────────────────────┘  │
│  [新增] [報表] [匯出]          [1][2][3]│
└─────────────────────────────────────────┘
```

#### 4.1.3 功能說明
- **篩選區**:
  - 模組代碼下拉選單（SYS1610, SYS1620, SYS1630, SYS1640, SYS1645, SYS1646, SYSC210等）
  - 資料類型下拉選單（選填）
  - 狀態下拉選單（全部/啟用/停用）
  - 關鍵字輸入框（搜尋資料代碼或名稱）
  - 查詢按鈕
- **資料表格**:
  - 顯示欄位：資料代碼、模組代碼、資料名稱、資料值、資料類型、狀態、排序順序、備註
  - 操作欄位：修改、刪除、報表按鈕
  - 支援排序（點擊欄位標題）
  - 支援分頁
- **操作按鈕**:
  - 新增按鈕：開啟新增對話框
  - 報表按鈕：開啟報表查詢對話框
  - 匯出按鈕：匯出資料檔案
  - 修改按鈕：開啟修改對話框
  - 刪除按鈕：確認後刪除

### 4.2 客戶管理頁面

#### 4.2.1 客戶列表頁面
- **路由**: `/jgjn/customers`
- **組件**: `JgjNCustomerList.vue`
- **功能**: 管理客戶資料
- **顯示欄位**: 客戶代碼、客戶名稱、客戶類型、聯絡人、電話、電子郵件、地址、狀態

### 4.3 發票列印頁面

#### 4.3.1 發票列表頁面
- **路由**: `/jgjn/invoices`
- **組件**: `JgjNInvoiceList.vue`
- **功能**: 管理發票並執行列印
- **顯示欄位**: 發票代碼、發票號碼、發票日期、客戶代碼、金額、幣別、狀態、列印狀態

### 4.4 報表查詢頁面

#### 4.4.1 報表查詢頁面
- **路由**: `/jgjn/reports`
- **組件**: `JgjNReportQuery.vue`
- **功能**: 查詢和列印報表
- **支援報表類型**: 明細報表、商店報表等

---

## 五、開發時程

### 階段一：資料庫設計（1週）
- 資料表設計
- 索引設計
- 資料字典建立

### 階段二：後端API開發（2週）
- 查詢API開發
- 新增API開發
- 修改API開發
- 刪除API開發
- 客戶管理API開發
- 發票列印API開發
- 報表查詢API開發

### 階段三：前端UI開發（2週）
- 資料列表頁面開發
- 資料維護對話框開發
- 客戶管理頁面開發
- 發票列印頁面開發
- 報表查詢頁面開發
- 表單驗證實作

### 階段四：測試與優化（1週）
- 單元測試
- 整合測試
- 效能優化
- 文件整理

**總計**: 約 6 週

---

## 六、注意事項

### 6.1 資料驗證
- 資料代碼與模組代碼組合必須唯一
- 資料名稱不能為空
- 客戶代碼必須唯一
- 資料代碼格式驗證（根據業務需求）

### 6.2 權限控制
- 資料維護需特定權限
- 客戶管理需特定權限
- 發票列印需特定權限
- 報表查詢需特定權限
- 刪除功能需確認對話框

### 6.3 效能考量
- 大量資料查詢需使用分頁
- 關鍵字搜尋需使用索引
- 報表查詢需使用非同步處理
- 資料匯出需使用批次處理

### 6.4 報表格式
- 支援多種報表格式（明細報表、商店報表等）
- 報表列印需支援PDF格式
- 報表資料需支援匯出功能

### 6.5 資料備份
- 重要資料需定期備份
- 刪除操作需記錄日誌
- 報表查詢記錄需保留歷史資料

---

## 七、測試案例

### 7.1 新增資料測試
- **測試案例1**: 正常新增
  - 輸入：完整的資料資訊
  - 預期：成功新增並顯示在列表中
- **測試案例2**: 資料代碼重複
  - 輸入：已存在的資料代碼（相同模組）
  - 預期：顯示錯誤訊息「資料代碼已存在」
- **測試案例3**: 必填欄位為空
  - 輸入：資料代碼或名稱為空
  - 預期：顯示驗證錯誤訊息

### 7.2 客戶管理測試
- **測試案例1**: 正常新增客戶
  - 輸入：完整的客戶資訊
  - 預期：成功新增並顯示在列表中
- **測試案例2**: 客戶代碼重複
  - 輸入：已存在的客戶代碼
  - 預期：顯示錯誤訊息「客戶代碼已存在」

### 7.3 發票列印測試
- **測試案例1**: 正常列印發票
  - 輸入：有效的發票代碼
  - 預期：成功列印並更新列印狀態
- **測試案例2**: 列印不存在的發票
  - 輸入：不存在的發票代碼
  - 預期：顯示錯誤訊息「發票不存在」

### 7.4 報表查詢測試
- **測試案例1**: 正常查詢報表
  - 輸入：選擇模組和日期範圍
  - 預期：成功查詢並顯示報表資料
- **測試案例2**: 無資料報表
  - 輸入：選擇無資料的條件
  - 預期：顯示訊息「無資料可顯示」

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1610_*.ASP` - SYS1610查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1620_*.ASP` - SYS1620查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1630_*.ASP` - SYS1630查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1640_*.ASP` - SYS1640查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1645_*.ASP` - SYS1645查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYS1646_*.ASP` - SYS1646查詢和報表功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/SYSC210_*.asp` - SYSC210客戶管理功能
- `WEB/IMS_CORE/ASP/SYSCUST_JGJN/PnInvoice_*.asp` - 發票列印功能

### 8.2 相關文件
- 系統架構分析.md - SYSCUST_JGJN 目錄分析
- 目錄掃描狀態統計.md - SYSCUST_JGJN 模組狀態

### 8.3 技術文件
- .NET Core 8.0 API 開發指南
- Vue 3 開發指南
- Dapper 使用手冊
- SQL Server 資料庫設計指南
- Element Plus 組件庫文件
- 報表列印指南

