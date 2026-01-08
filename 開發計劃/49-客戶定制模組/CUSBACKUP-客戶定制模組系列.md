# CUSBACKUP - 客戶定制模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: CUSBACKUP 系列
- **功能名稱**: 客戶定制模組備份系列
- **功能描述**: 提供客戶定制模組的備份功能，包含各客戶模組的備份代碼和配置。此模組主要用於備份管理，包含CUS3000.tssp、CUS3000D、CUS3000LP、CUS3200、CUS5000SSPLAZA、CUSBellavita等客戶備份
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000.tssp/*.ASP` (CUS3000備份)
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000D/*.ASP` (CUS3000D備份)
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000LP/*.ASP` (CUS3000LP備份)
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3200/*.ASP` (CUS3200備份)
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUS5000SSPLAZA/*.ASP` (CUS5000SSPLAZA備份)
  - `WEB/IMS_CORE/ASP/CUSBACKUP/CUSBellavita/*.ASP` (CUSBellavita備份)

### 1.2 業務需求
- 管理客戶定制模組的備份
- 支援備份版本管理
- 支援備份還原功能
- 支援備份查詢功能
- 記錄備份操作日誌

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CustomerModuleBackups` (客戶模組備份主檔)

```sql
CREATE TABLE [dbo].[CustomerModuleBackups] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [BackupId] NVARCHAR(50) NOT NULL, -- 備份編號
    [CustomerCode] NVARCHAR(50) NOT NULL, -- 客戶代碼
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [BackupVersion] NVARCHAR(50) NULL, -- 備份版本
    [BackupDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 備份日期
    [BackupPath] NVARCHAR(500) NULL, -- 備份路徑
    [BackupType] NVARCHAR(20) NULL, -- 備份類型 (FULL, INCREMENTAL)
    [FileCount] INT NULL DEFAULT 0, -- 檔案數量
    [FileSize] BIGINT NULL DEFAULT 0, -- 檔案大小（位元組）
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [Description] NVARCHAR(1000) NULL, -- 備份說明
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_CustomerModuleBackups_BackupId] UNIQUE ([BackupId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CustomerModuleBackups_CustomerCode] ON [dbo].[CustomerModuleBackups] ([CustomerCode]);
CREATE NONCLUSTERED INDEX [IX_CustomerModuleBackups_ModuleCode] ON [dbo].[CustomerModuleBackups] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_CustomerModuleBackups_BackupDate] ON [dbo].[CustomerModuleBackups] ([BackupDate]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢備份列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/customer-module-backups`
- **說明**: 查詢備份列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆備份
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/customer-module-backups/{backupId}`
- **說明**: 查詢單筆備份資料

#### 3.1.3 新增備份
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/customer-module-backups`
- **說明**: 新增備份記錄

#### 3.1.4 修改備份
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/customer-module-backups/{backupId}`
- **說明**: 修改備份記錄

#### 3.1.5 刪除備份
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/customer-module-backups/{backupId}`
- **說明**: 刪除備份記錄

#### 3.1.6 執行備份
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/customer-module-backups/execute`
- **說明**: 執行備份作業
- **請求格式**:
  ```json
  {
    "customerCode": "CUS3000",
    "moduleCode": "CUS3000",
    "backupVersion": "v1.0",
    "backupType": "FULL"
  }
  ```

#### 3.1.7 還原備份
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/customer-module-backups/{backupId}/restore`
- **說明**: 還原備份

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 備份列表頁面 (`CustomerModuleBackupList.vue`)
- **路徑**: `/customer-module-backups`
- **功能**: 顯示備份列表，支援查詢、新增、修改、刪除、執行備份、還原

### 4.2 UI 元件設計

參考一般資料維護功能的UI設計，但針對備份管理進行優化。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 備份功能實作
- [ ] 還原功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 備份/還原功能開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 備份/還原功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 備份檔案必須安全儲存
- 必須實作權限檢查
- 還原操作必須確認

### 6.2 效能
- 大量檔案備份必須使用批次處理
- 必須支援非同步備份
- 必須提供備份進度追蹤

### 6.3 資料驗證
- 備份編號必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 備份前必須檢查系統狀態
- 還原前必須備份現有資料
- 備份操作必須記錄日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增備份成功
- [ ] 執行備份成功
- [ ] 還原備份成功
- [ ] 查詢備份列表成功

### 7.2 整合測試
- [ ] 完整備份流程測試
- [ ] 還原功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000.tssp/*.ASP`
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000D/*.ASP`
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3000LP/*.ASP`
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUS3200/*.ASP`
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUS5000SSPLAZA/*.ASP`
- `WEB/IMS_CORE/ASP/CUSBACKUP/CUSBellavita/*.ASP`

### 8.2 相關功能
- CUS3000-客戶定制模組系列（客戶定制功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

