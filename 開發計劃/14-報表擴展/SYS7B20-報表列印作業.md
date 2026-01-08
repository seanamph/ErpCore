# SYS7B20 - 報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS7B20
- **功能名稱**: 報表列印作業
- **功能描述**: 提供報表模組7的報表列印功能，使用Crystal Reports進行報表列印，支援PDF格式列印、報表預覽、列印設定等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYS7000/SYS7B20_PR.aspx` (報表列印頁面)
  - `WEB/IMS_CORE/SYS7000/SYS7B20_PR.rpt` (Crystal Reports報表定義)
  - `WEB/IMS_CORE/SYS7000/SYS7B20_PR.xsd` (資料結構定義)
  - `WEB/IMS_CORE/SYS7000/SYS7B20.ascx` (使用者控制項)

### 1.2 業務需求
- 提供報表列印功能
- 支援PDF格式列印
- 支援報表預覽
- 支援列印設定（頁面大小、邊界等）
- 支援報表資料查詢與篩選
- 記錄報表列印記錄
- 支援報表匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportPrints` (報表列印記錄)

```sql
CREATE TABLE [dbo].[ReportPrints] (
    [PrintId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 列印ID
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS7B20)
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [PrintParams] NVARCHAR(MAX) NULL, -- 列印參數 (JSON格式)
    [PrintFormat] NVARCHAR(20) NOT NULL DEFAULT 'PDF', -- 列印格式 (PDF, Excel, Word等)
    [PrintStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 列印狀態 (PENDING, PROCESSING, COMPLETED, FAILED)
    [FileUrl] NVARCHAR(500) NULL, -- 檔案URL
    [FileSize] BIGINT NULL, -- 檔案大小(位元組)
    [PageCount] INT NULL, -- 頁數
    [PrintedBy] NVARCHAR(50) NULL, -- 列印者
    [PrintedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 列印時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReportPrints] PRIMARY KEY CLUSTERED ([PrintId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrints_ReportCode] ON [dbo].[ReportPrints] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintedBy] ON [dbo].[ReportPrints] ([PrintedBy]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintedAt] ON [dbo].[ReportPrints] ([PrintedAt]);
CREATE NONCLUSTERED INDEX [IX_ReportPrints_PrintStatus] ON [dbo].[ReportPrints] ([PrintStatus]);
```

### 2.2 相關資料表

#### 2.2.1 `ReportQueries` - 報表查詢設定
- 參考: `開發計劃/14-報表擴展/SYS7000-報表模組7-報表查詢.md`

#### 2.2.2 `ReportPrintDetails` - 報表列印明細
```sql
CREATE TABLE [dbo].[ReportPrintDetails] (
    [DetailId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PrintId] BIGINT NOT NULL, -- 列印ID (外鍵至ReportPrints)
    [RowNo] INT NOT NULL, -- 行號
    [DataJson] NVARCHAR(MAX) NULL, -- 資料內容 (JSON格式)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ReportPrintDetails_ReportPrints] FOREIGN KEY ([PrintId]) REFERENCES [dbo].[ReportPrints] ([PrintId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrintDetails_PrintId] ON [dbo].[ReportPrintDetails] ([PrintId]);
```

### 2.3 資料字典

#### ReportPrints 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PrintId | BIGINT | - | NO | IDENTITY(1,1) | 列印ID | 主鍵 |
| ReportCode | NVARCHAR | 50 | NO | - | 報表代碼 | SYS7B20 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| PrintParams | NVARCHAR(MAX) | - | YES | - | 列印參數 | JSON格式 |
| PrintFormat | NVARCHAR | 20 | NO | 'PDF' | 列印格式 | PDF, Excel, Word等 |
| PrintStatus | NVARCHAR | 20 | NO | 'PENDING' | 列印狀態 | PENDING, PROCESSING, COMPLETED, FAILED |
| FileUrl | NVARCHAR | 500 | YES | - | 檔案URL | - |
| FileSize | BIGINT | - | YES | - | 檔案大小 | 位元組 |
| PageCount | INT | - | YES | - | 頁數 | - |
| PrintedBy | NVARCHAR | 50 | YES | - | 列印者 | - |
| PrintedAt | DATETIME2 | - | NO | GETDATE() | 列印時間 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/prints`
- **說明**: 查詢報表列印記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PrintedAt",
    "sortOrder": "DESC",
    "filters": {
      "reportCode": "SYS7B20",
      "printStatus": "",
      "printedBy": "",
      "startDate": "",
      "endDate": ""
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
          "printId": 1,
          "reportCode": "SYS7B20",
          "reportName": "報表列印作業",
          "printFormat": "PDF",
          "printStatus": "COMPLETED",
          "fileUrl": "/files/reports/sys7b20_20240101_001.pdf",
          "fileSize": 1024000,
          "pageCount": 10,
          "printedBy": "U001",
          "printedAt": "2024-01-01T10:00:00"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 執行報表列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/print`
- **說明**: 執行報表列印，產生PDF檔案
- **請求格式**:
  ```json
  {
    "reportCode": "SYS7B20",
    "printParams": {
      "startDate": "2024-01-01",
      "endDate": "2024-01-31",
      "filter1": "value1",
      "filter2": "value2"
    },
    "printFormat": "PDF",
    "pageSize": "A4",
    "orientation": "Portrait",
    "margins": {
      "top": 0.25,
      "bottom": 0.25,
      "left": 0.25,
      "right": 0.25
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "printId": 1,
      "fileUrl": "/api/v1/reports/sys7000/sys7b20/prints/1/download",
      "printStatus": "COMPLETED"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢單筆報表列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/prints/{printId}`
- **說明**: 根據列印ID查詢單筆報表列印記錄
- **路徑參數**:
  - `printId`: 列印ID
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.4 下載報表檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/prints/{printId}/download`
- **說明**: 下載報表檔案
- **路徑參數**:
  - `printId`: 列印ID
- **回應**: 檔案下載（PDF、Excel等）

#### 3.1.5 預覽報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/preview`
- **說明**: 預覽報表（不產生檔案，直接返回預覽資料）
- **請求格式**: 同執行報表列印
- **回應格式**: 返回報表預覽資料（Base64編碼的PDF或HTML）

#### 3.1.6 刪除報表列印記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sys7000/sys7b20/prints/{printId}`
- **說明**: 刪除報表列印記錄及相關檔案
- **路徑參數**:
  - `printId`: 列印ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 報表列印頁面 (`SYS7B20Print.vue`)

#### 4.1.1 頁面結構
參考 `SYS7B10-報表列印作業.md` 的 UI 設計，主要差異在於報表代碼為 SYS7B20

#### 4.1.2 主要功能
- 報表查詢條件設定
- 報表預覽功能
- 報表列印功能
- 列印記錄查詢
- 報表檔案下載
- 列印記錄刪除

---

## 五、後端實作

### 5.1 Controller (`SYS7B20ReportPrintController.cs`)

參考 `SYS7B10-報表列印作業.md` 的 Controller 實作，主要差異在於：
- 路由路徑: `/api/v1/reports/sys7000/sys7b20`
- 報表代碼: `SYS7B20`

### 5.2 Service (`ReportPrintService.cs`)

參考 `SYS7B10-報表列印作業.md` 的 Service 實作，可共用相同的 Service，透過報表代碼區分不同的報表類型。

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 使用共用的 ReportPrints 資料表（已存在）

2. **後端 API 開發** (2 天)
   - 實作 Controller 層（參考 SYS7B10）
   - 調整報表資料查詢邏輯
   - 單元測試

3. **前端 UI 開發** (1.5 天)
   - 報表列印頁面（參考 SYS7B10）
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 4.5 天

---

## 七、注意事項

### 7.1 報表產生
- 使用 RDLC (Report Definition Language Client) 或 iTextSharp 產生 PDF
- 支援多種報表格式（PDF、Excel、Word）
- 支援自訂頁面大小和邊界設定
- 注意報表資料量，避免記憶體溢出

### 7.2 檔案管理
- 報表檔案需儲存在安全的目錄
- 定期清理過期的報表檔案
- 檔案下載需驗證權限
- 檔案大小限制

### 7.3 效能優化
- 大量資料時使用分頁查詢
- 報表產生使用非同步處理
- 考慮使用背景任務處理報表產生
- 快取常用的報表資料

### 7.4 安全性
- 驗證使用者權限
- 防止路徑遍歷攻擊
- 檔案下載需驗證權限
- 記錄報表列印操作

---

## 八、測試案例

### 8.1 功能測試
1. **列印測試**
   - 正常列印報表
   - 不同格式列印（PDF、Excel）
   - 不同頁面大小列印
   - 大量資料列印

2. **預覽測試**
   - 報表預覽功能
   - 預覽資料正確性

3. **查詢測試**
   - 依日期範圍查詢
   - 依狀態查詢
   - 依列印者查詢
   - 分頁查詢

4. **下載測試**
   - 檔案下載功能
   - 檔案完整性驗證

5. **刪除測試**
   - 刪除列印記錄
   - 刪除相關檔案

### 8.2 效能測試
- 大量資料列印效能
- 並發列印測試
- 檔案下載效能

### 8.3 安全性測試
- 權限驗證測試
- 檔案下載權限測試
- 路徑遍歷攻擊測試

