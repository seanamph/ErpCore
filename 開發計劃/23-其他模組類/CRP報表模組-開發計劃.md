# CRP - 報表模組 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: CRP
- **功能名稱**: CRP報表模組
- **功能描述**: 提供Crystal Reports報表功能，用於報表生成、資料處理、報表列印等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/UTIL/CRP.ASP` (CRP報表類別)
  - `WEB/IMS_CORE/CRP/RPT/` (Crystal Reports報表檔案)
  - `WEB/IMS_CORE/SYSW000/*.rpt` (各模組的Crystal Reports報表)

### 1.2 業務需求
- Crystal Reports報表生成
- 報表資料處理
- 報表列印功能
- 報表匯出功能 (PDF, Excel)
- 報表參數設定
- 報表快取機制

### 1.3 使用場景
- 報表生成與列印
- 資料報表匯出
- 報表資料分析

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CrystalReports` (Crystal Reports設定)

```sql
CREATE TABLE [dbo].[CrystalReports] (
    [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [ReportPath] NVARCHAR(500) NOT NULL, -- 報表檔案路徑
    [MdbName] NVARCHAR(200) NULL, -- MDB檔案名稱
    [Parameters] NVARCHAR(MAX) NULL, -- 報表參數 (JSON格式)
    [ExportOptions] NVARCHAR(MAX) NULL, -- 匯出選項 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_CrystalReports_ReportCode] UNIQUE ([ReportCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CrystalReports_ReportCode] ON [dbo].[CrystalReports] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_CrystalReports_Status] ON [dbo].[CrystalReports] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `CrystalReportLogs` - Crystal Reports操作記錄
```sql
CREATE TABLE [dbo].[CrystalReportLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReportId] BIGINT NOT NULL,
    [ReportCode] NVARCHAR(50) NOT NULL,
    [OperationType] NVARCHAR(20) NOT NULL, -- GENERATE, PRINT, EXPORT
    [Parameters] NVARCHAR(MAX) NULL, -- 報表參數 (JSON格式)
    [Status] NVARCHAR(20) NOT NULL, -- SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [FileSize] BIGINT NULL, -- 檔案大小 (位元組)
    [Duration] INT NULL, -- 執行時間 (毫秒)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_CrystalReportLogs_CrystalReports] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[CrystalReports] ([ReportId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_ReportId] ON [dbo].[CrystalReportLogs] ([ReportId]);
CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_ReportCode] ON [dbo].[CrystalReportLogs] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_CrystalReportLogs_CreatedAt] ON [dbo].[CrystalReportLogs] ([CreatedAt]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 生成報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/crp/reports/generate`
- **說明**: 生成Crystal Reports報表
- **請求格式**:
  ```json
  {
    "reportCode": "SYSW170_PR1",
    "parameters": {
      "param1": "value1",
      "param2": "value2"
    },
    "exportFormat": "PDF"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "報表生成成功",
    "data": {
      "reportId": 1,
      "downloadUrl": "/api/v1/crp/reports/1/download"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 下載報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/crp/reports/{reportId}/download`
- **說明**: 下載生成的報表
- **回應格式**: 檔案下載

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 CRP報表管理頁面 (`CrystalReport.vue`)
- **路徑**: `/system/crp/reports`
- **功能**: Crystal Reports報表管理、報表生成、報表下載
- **主要元件**:
  - 報表列表
  - 報表參數設定表單
  - 報表生成按鈕
  - 報表下載連結

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] Crystal Reports整合
- [ ] 報表生成邏輯
- [ ] 報表匯出邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] CRP報表管理頁面開發
- [ ] 報表參數設定元件
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] Crystal Reports整合測試
- [ ] 報表生成測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 技術考量
- 需要安裝Crystal Reports Runtime
- 需要處理Crystal Reports授權
- 需要處理報表檔案路徑
- 需要處理MDB檔案生成

### 6.2 效能
- 報表生成必須使用非同步處理
- 必須使用快取機制
- 必須處理大量資料報表

### 6.3 相容性
- 必須相容舊版Crystal Reports格式
- 必須處理報表版本差異

---

## 七、參考資料

### 7.1 舊程式碼
- `WEB/IMS_CORE/ASP/UTIL/CRP.ASP`
- `WEB/IMS_CORE/CRP/RPT/` 目錄下報表檔案

### 7.2 相關功能
- 報表生成
- 資料匯出
- 報表列印

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

