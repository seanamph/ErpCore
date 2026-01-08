# SYSG710-SYSG7I0 - 報表列印作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG710-SYSG7I0 系列
- **功能名稱**: 報表列印作業系列
- **功能描述**: 提供銷售報表的列印功能，包含各種格式的報表列印、PDF產生、報表模板管理等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG710_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG720_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG730_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG740_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG750_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG760_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG770_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG780_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG790_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7A0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7B0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7C0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7D0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7E0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7F0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7G0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7H0_*.ASP` (報表列印)
  - `WEB/IMS_CORE/ASP/SYSG000/SYSG7I0_*.ASP` (報表列印)

### 1.2 業務需求
- 列印銷售報表
- 支援多種報表格式（PDF、Excel、Word等）
- 支援報表模板管理
- 支援報表參數設定
- 支援批次列印
- 支援報表預覽
- 支援報表下載

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

使用與 SYSG410-SYSG460 相同的資料表結構，參考 `SalesOrders` 和 `SalesOrderDetails` 資料表。

### 2.2 報表模板表: `ReportTemplates` (報表模板)

```sql
CREATE TABLE [dbo].[ReportTemplates] (
    [TemplateId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [TemplateName] NVARCHAR(100) NOT NULL,
    [TemplateType] NVARCHAR(20) NOT NULL, -- PDF, EXCEL, WORD
    [ReportType] NVARCHAR(50) NOT NULL, -- SALES_ORDER, SALES_SUMMARY等
    [TemplateContent] NVARCHAR(MAX) NULL, -- 模板內容（HTML、XML等）
    [TemplateFile] NVARCHAR(500) NULL, -- 模板檔案路徑
    [Parameters] NVARCHAR(MAX) NULL, -- 報表參數JSON
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportTemplates_ReportType] ON [dbo].[ReportTemplates] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_ReportTemplates_Status] ON [dbo].[ReportTemplates] ([Status]);
```

### 2.3 報表列印記錄表: `ReportPrintLogs` (報表列印記錄)

```sql
CREATE TABLE [dbo].[ReportPrintLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportId] NVARCHAR(50) NOT NULL, -- 報表編號
    [ReportType] NVARCHAR(50) NOT NULL,
    [TemplateId] NVARCHAR(50) NULL,
    [PrintUserId] NVARCHAR(50) NOT NULL,
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [PrintFormat] NVARCHAR(20) NOT NULL, -- PDF, EXCEL, WORD
    [FileUrl] NVARCHAR(500) NULL, -- 檔案下載連結
    [Parameters] NVARCHAR(MAX) NULL, -- 報表參數JSON
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'S', -- S:成功, F:失敗
    [ErrorMessage] NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_ReportType] ON [dbo].[ReportPrintLogs] ([ReportType]);
CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_PrintUserId] ON [dbo].[ReportPrintLogs] ([PrintUserId]);
CREATE NONCLUSTERED INDEX [IX_ReportPrintLogs_PrintDate] ON [dbo].[ReportPrintLogs] ([PrintDate]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/print`
- **說明**: 產生並列印報表
- **請求格式**:
  ```json
  {
    "reportType": "SALES_ORDER",
    "templateId": "TEMPLATE001",
    "printFormat": "PDF",
    "parameters": {
      "orderId": "SO20240101001",
      "orderDateFrom": "2024-01-01",
      "orderDateTo": "2024-01-31",
      "shopId": "SHOP001"
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
      "reportId": "RPT20240101001",
      "fileUrl": "/api/v1/reports/download/RPT20240101001",
      "fileName": "銷售報表_20240101.pdf"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 預覽報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/preview`
- **說明**: 預覽報表（不產生檔案）
- **請求格式**: 同列印報表
- **回應格式**: 返回報表HTML內容

#### 3.1.3 下載報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/download/{reportId}`
- **說明**: 下載已產生的報表檔案
- **路徑參數**: `reportId` - 報表編號
- **回應格式**: 返回報表檔案

#### 3.1.4 查詢報表模板列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/report-templates`
- **說明**: 查詢報表模板列表
- **請求參數**:
  ```json
  {
    "reportType": "SALES_ORDER",
    "status": "A"
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
          "templateId": "TEMPLATE001",
          "templateName": "銷售單報表",
          "templateType": "PDF",
          "reportType": "SALES_ORDER",
          "status": "A"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 報表列印頁面

#### 4.1.1 頁面結構
- **標題**: 報表列印作業
- **報表選擇區塊**: 
  - 報表類型（下拉選單：銷售單報表、銷售彙總報表等）
  - 報表模板（下拉選單）
  - 列印格式（下拉選單：PDF、Excel、Word）
- **參數設定區塊**:
  - 銷售單號（文字輸入）
  - 銷售日期起（日期選擇器）
  - 銷售日期迄（日期選擇器）
  - 分店代碼（下拉選單）
  - 客戶代碼（文字輸入）
  - 其他參數...
- **操作按鈕區塊**:
  - 預覽按鈕
  - 列印按鈕
  - 下載按鈕
  - 批次列印按鈕

#### 4.1.2 報表預覽對話框
- 顯示報表預覽內容
- 支援縮放功能
- 支援列印功能
- 支援下載功能

---

## 五、開發時程

### 5.1 後端開發（3天）
- **第1天**: 報表模板表建立、Repository 層開發
- **第2天**: Service 層開發、報表產生邏輯開發
- **第3天**: API Controller 開發、PDF產生功能、Excel產生功能

### 5.2 前端開發（2天）
- **第1天**: 列印頁面開發、參數設定功能
- **第2天**: 報表預覽功能、列印功能、下載功能

### 5.3 整合測試（1天）
- 前後端整合測試
- 功能測試
- 效能測試

**總計**: 6天

---

## 六、注意事項

### 6.1 報表產生效能
- 大量資料報表建議使用非同步處理
- 使用快取機制快取常用報表
- 報表檔案需要定期清理

### 6.2 權限控制
- 列印權限：需要有報表列印權限
- 下載權限：需要有報表下載權限
- 模板管理權限：需要有報表模板管理權限

### 6.3 檔案管理
- 報表檔案需要設定保留期限
- 報表檔案需要設定儲存空間限制
- 報表檔案需要設定下載次數限制

---

## 七、測試案例

### 7.1 列印測試
1. **測試案例1**: 正常列印報表
   - 列印報表
   - 預期結果：成功產生報表檔案

2. **測試案例2**: 預覽報表
   - 預覽報表
   - 預期結果：成功顯示報表預覽

3. **測試案例3**: 下載報表
   - 下載報表
   - 預期結果：成功下載報表檔案

---

## 八、參考資料

### 8.1 舊程式檔案
- `WEB/IMS_CORE/ASP/SYSG000/SYSG710_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG720_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG730_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG740_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG750_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG760_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG770_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG780_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG790_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7A0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7B0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7C0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7D0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7E0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7F0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7G0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7H0_*.ASP`
- `WEB/IMS_CORE/ASP/SYSG000/SYSG7I0_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

