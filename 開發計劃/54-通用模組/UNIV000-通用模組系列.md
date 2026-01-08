# UNIV000 - 通用模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: UNIV000 系列
- **功能名稱**: 通用模組系列
- **功能描述**: 提供統一接口和資料交換功能，包含發票列印、資料匯入匯出、批量處理等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV220_*.ASP` (UNIV220系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV230_*.ASP` (UNIV230系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV240_*.ASP` (UNIV240系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV270_*.ASP` (UNIV270系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV280_*.ASP` (UNIV280系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV2A0_*.ASP` (UNIV2A0系列功能)
  - `WEB/IMS_CORE/ASP/UNIV000/UNIV2E0_*.ASP` (UNIV2E0系列功能)

### 1.2 業務需求
- 提供統一接口和資料交換功能
- 支援發票列印功能
- 支援資料匯入匯出功能
- 支援批量處理功能
- 支援多個子模組的完整CRUD操作和報表功能
- 支援自動處理功能
- 支援憑證處理功能

### 1.3 子模組清單
- **UNIV220**: 統一接口模組（AI、BI、變更、返回、檢查、刪除、瀏覽、新增、查詢、保存、修改、報表等功能）
- **UNIV230**: 統一接口模組（BI、客戶、日期列表、刪除、瀏覽、新增、查詢、保存、修改、發票檢查、發票報表、PF報表等功能）
- **UNIV240**: 統一接口模組（刪除、瀏覽、新增、查詢、保存、修改、歷史列表、發票報表、PF報表、憑證報表等功能）
- **UNIV270**: 統一接口模組（BI、檢查、瀏覽、刪除、新增、查詢、保存、修改、報表等功能）
- **UNIV280**: 統一接口模組（BI、瀏覽、刪除、新增、查詢、保存、修改、報表等功能）
- **UNIV2A0**: 統一接口模組（BI、檢查、刪除、瀏覽、新增、查詢、修改、發票報表、返回報表等功能）
- **UNIV2E0**: 統一接口模組（BI、檢查、刪除、瀏覽、新增、查詢、修改、報表、返回報表等功能）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Univ000Data`

```sql
CREATE TABLE [dbo].[Univ000Data] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
    [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼 (UNIV220, UNIV230, UNIV240, UNIV270, UNIV280, UNIV2A0, UNIV2E0)
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
    CONSTRAINT [UQ_Univ000Data_DataId_ModuleCode] UNIQUE ([DataId], [ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Univ000Data_ModuleCode] ON [dbo].[Univ000Data] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_Univ000Data_DataType] ON [dbo].[Univ000Data] ([DataType]);
CREATE NONCLUSTERED INDEX [IX_Univ000Data_Status] ON [dbo].[Univ000Data] ([Status]);
```

### 2.2 發票資料表: `UnivInvoice`

```sql
CREATE TABLE [dbo].[UnivInvoice] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InvoiceId] NVARCHAR(50) NOT NULL, -- 發票代碼
    [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼
    [InvoiceNo] NVARCHAR(50) NULL, -- 發票號碼
    [InvoiceDate] DATETIME2 NULL, -- 發票日期
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
    CONSTRAINT [UQ_UnivInvoice_InvoiceId] UNIQUE ([InvoiceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UnivInvoice_ModuleCode] ON [dbo].[UnivInvoice] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_UnivInvoice_InvoiceDate] ON [dbo].[UnivInvoice] ([InvoiceDate]);
CREATE NONCLUSTERED INDEX [IX_UnivInvoice_Status] ON [dbo].[UnivInvoice] ([Status]);
```

### 2.3 資料交換記錄表: `UnivDataExchange`

```sql
CREATE TABLE [dbo].[UnivDataExchange] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExchangeId] NVARCHAR(50) NOT NULL, -- 交換代碼
    [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼
    [ExchangeType] NVARCHAR(20) NOT NULL, -- 交換類型 (IMPORT:匯入, EXPORT:匯出)
    [FileName] NVARCHAR(200) NOT NULL, -- 檔案名稱
    [FilePath] NVARCHAR(500) NULL, -- 檔案路徑
    [RecordCount] INT NULL, -- 記錄筆數
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING:待處理, PROCESSING:處理中, SUCCESS:成功, FAILED:失敗)
    [ProcessDate] DATETIME2 NULL, -- 處理日期
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_UnivDataExchange_ExchangeId] UNIQUE ([ExchangeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UnivDataExchange_ModuleCode] ON [dbo].[UnivDataExchange] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_UnivDataExchange_ExchangeType] ON [dbo].[UnivDataExchange] ([ExchangeType]);
CREATE NONCLUSTERED INDEX [IX_UnivDataExchange_Status] ON [dbo].[UnivDataExchange] ([Status]);
```

### 2.4 資料字典

#### Univ000Data 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| DataId | NVARCHAR | 50 | NO | - | 資料代碼 | 與ModuleCode組合唯一 |
| ModuleCode | NVARCHAR | 20 | NO | - | 模組代碼 | UNIV220, UNIV230等 |
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
GET /api/v1/univ000/data?moduleCode={moduleCode}&dataType={dataType}&status={status}&keyword={keyword}&page={page}&pageSize={pageSize}
```

**請求參數**:
- `moduleCode`: 模組代碼（選填，UNIV220, UNIV230, UNIV240, UNIV270, UNIV280, UNIV2A0, UNIV2E0）
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
        "moduleCode": "UNIV220",
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
POST /api/v1/univ000/data
```

**請求體**:
```json
{
  "dataId": "DATA001",
  "moduleCode": "UNIV220",
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
    "moduleCode": "UNIV220",
    "dataName": "資料名稱",
    "status": "A",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

### 3.3 修改資料

```http
PUT /api/v1/univ000/data/{tKey}
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
DELETE /api/v1/univ000/data/{tKey}
```

### 3.5 發票列印

```http
POST /api/v1/univ000/invoice/{invoiceId}/print
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

### 3.6 資料匯入

```http
POST /api/v1/univ000/data/import
Content-Type: multipart/form-data
```

**請求體**:
- `file`: 檔案（multipart/form-data）
- `moduleCode`: 模組代碼

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "匯入成功",
  "data": {
    "exchangeId": "EXCH001",
    "recordCount": 100,
    "status": "SUCCESS"
  }
}
```

### 3.7 資料匯出

```http
POST /api/v1/univ000/data/export
```

**請求體**:
```json
{
  "moduleCode": "UNIV220",
  "dataType": "TYPE001",
  "format": "CSV"
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "匯出成功",
  "data": {
    "exchangeId": "EXCH002",
    "fileName": "export_data.csv",
    "filePath": "/files/exports/export_data.csv",
    "recordCount": 100
  }
}
```

### 3.8 批量處理

```http
POST /api/v1/univ000/data/batch
```

**請求體**:
```json
{
  "moduleCode": "UNIV220",
  "action": "UPDATE_STATUS",
  "dataIds": ["DATA001", "DATA002"],
  "parameters": {
    "status": "I"
  }
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "批量處理成功",
  "data": {
    "processedCount": 2,
    "failedCount": 0
  }
}
```

---

## 四、前端 UI 設計

### 4.1 資料列表頁面

#### 4.1.1 路由配置
- **路由**: `/univ000/data`
- **組件**: `Univ000DataList.vue`
- **權限**: `UNIV000_DATA_VIEW`

#### 4.1.2 頁面結構
```
┌─────────────────────────────────────────┐
│  通用模組資料維護                         │
├─────────────────────────────────────────┤
│  [模組] [類型] [狀態] [關鍵字] [查詢]      │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐  │
│  │ 資料代碼 │ 模組 │ 資料名稱 │ 狀態 │  │
│  ├───────────────────────────────────┤  │
│  │ DATA001 │UNIV220│ 資料名稱1 │ 啟用│  │
│  │ DATA002 │UNIV230│ 資料名稱2 │ 停用│  │
│  └───────────────────────────────────┘  │
│  [新增] [匯入] [匯出] [批量處理] [1][2][3]│
└─────────────────────────────────────────┘
```

#### 4.1.3 功能說明
- **篩選區**:
  - 模組代碼下拉選單（UNIV220, UNIV230, UNIV240, UNIV270, UNIV280, UNIV2A0, UNIV2E0）
  - 資料類型下拉選單（選填）
  - 狀態下拉選單（全部/啟用/停用）
  - 關鍵字輸入框（搜尋資料代碼或名稱）
  - 查詢按鈕
- **資料表格**:
  - 顯示欄位：資料代碼、模組代碼、資料名稱、資料值、資料類型、狀態、排序順序、備註
  - 操作欄位：修改、刪除按鈕
  - 支援排序（點擊欄位標題）
  - 支援分頁
- **操作按鈕**:
  - 新增按鈕：開啟新增對話框
  - 匯入按鈕：匯入資料檔案
  - 匯出按鈕：匯出資料檔案
  - 批量處理按鈕：執行批量操作
  - 修改按鈕：開啟修改對話框
  - 刪除按鈕：確認後刪除

### 4.2 發票列印頁面

#### 4.2.1 發票列表頁面
- **路由**: `/univ000/invoices`
- **組件**: `UnivInvoiceList.vue`
- **功能**: 管理發票並執行列印
- **顯示欄位**: 發票代碼、模組代碼、發票號碼、發票日期、金額、幣別、狀態、列印狀態

### 4.3 資料匯入匯出對話框

#### 4.3.1 匯入對話框
- **組件**: `UnivDataImportDialog.vue`
- **功能**: 匯入資料檔案
- **表單欄位**:
  - 模組代碼（必填，下拉選單）
  - 檔案（必填，檔案上傳）

#### 4.3.2 匯出對話框
- **組件**: `UnivDataExportDialog.vue`
- **功能**: 匯出資料檔案
- **表單欄位**:
  - 模組代碼（必填，下拉選單）
  - 資料類型（選填，下拉選單）
  - 格式（必填，下拉選單：CSV, Excel, JSON）

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
- 發票列印API開發
- 資料匯入匯出API開發
- 批量處理API開發

### 階段三：前端UI開發（2週）
- 資料列表頁面開發
- 資料維護對話框開發
- 發票列印頁面開發
- 資料匯入匯出對話框開發
- 批量處理對話框開發
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
- 資料代碼格式驗證（根據業務需求）

### 6.2 權限控制
- 資料維護需特定權限
- 發票列印需特定權限
- 資料匯入匯出需特定權限
- 批量處理需特定權限
- 刪除功能需確認對話框

### 6.3 效能考量
- 大量資料查詢需使用分頁
- 關鍵字搜尋需使用索引
- 資料匯入匯出需使用非同步處理
- 批量處理需使用批次處理

### 6.4 檔案管理
- 匯入檔案需限制大小（最大10MB）
- 匯入檔案格式需驗證（CSV, Excel, JSON）
- 匯出檔案需定期清理（如需要）

### 6.5 資料備份
- 重要資料需定期備份
- 刪除操作需記錄日誌
- 匯入匯出記錄需保留歷史資料

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

### 7.2 資料匯入測試
- **測試案例1**: 正常匯入CSV檔案
  - 輸入：有效的CSV檔案
  - 預期：成功匯入並建立資料記錄
- **測試案例2**: 匯入格式錯誤
  - 輸入：格式不符合要求的檔案
  - 預期：顯示錯誤訊息「檔案格式錯誤」

### 7.3 資料匯出測試
- **測試案例1**: 正常匯出CSV檔案
  - 輸入：選擇模組和資料類型
  - 預期：成功匯出並下載檔案
- **測試案例2**: 無資料匯出
  - 輸入：選擇無資料的條件
  - 預期：顯示訊息「無資料可匯出」

### 7.4 批量處理測試
- **測試案例1**: 正常批量更新狀態
  - 輸入：選擇多筆資料並更新狀態
  - 預期：成功更新所有選定資料的狀態
- **測試案例2**: 批量處理部分失敗
  - 輸入：包含無效資料的批量處理
  - 預期：顯示處理結果（成功筆數、失敗筆數）

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/UNIV000/UNIV220_*.ASP` - UNIV220系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV230_*.ASP` - UNIV230系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV240_*.ASP` - UNIV240系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV270_*.ASP` - UNIV270系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV280_*.ASP` - UNIV280系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV2A0_*.ASP` - UNIV2A0系列功能
- `WEB/IMS_CORE/ASP/UNIV000/UNIV2E0_*.ASP` - UNIV2E0系列功能

### 8.2 相關文件
- 系統架構分析.md - UNIV000 目錄分析
- 目錄掃描狀態統計.md - UNIV000 模組狀態

### 8.3 技術文件
- .NET Core 8.0 API 開發指南
- Vue 3 開發指南
- Dapper 使用手冊
- SQL Server 資料庫設計指南
- Element Plus 組件庫文件
- CSV/Excel 檔案處理指南

