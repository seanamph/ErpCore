# SYSH3D0_FMI - 人事批量新增系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSH3D0_FMI 系列
- **功能名稱**: 人事批量新增系列
- **功能描述**: 提供人事資料的批量新增功能，支援從Excel檔案匯入多筆人事資料，包含人事編號、姓名、部門、職位、到職日期等資訊的批量處理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSPH00/SYSH3D0_FMI.ASP` (人事批量新增)

### 1.2 業務需求
- 支援從Excel檔案匯入人事資料
- 支援批量新增多筆人事資料
- 支援資料驗證與錯誤處理
- 支援匯入結果報表
- 支援匯入失敗資料匯出
- 支援匯入進度顯示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Personnel` (人事主檔)

使用與SYSPED0-人事資料維護系列相同的資料表結構，參考 `Personnel` 資料表。

### 2.2 相關資料表

#### 2.2.1 `PersonnelImportLogs` - 人事匯入記錄
```sql
CREATE TABLE [dbo].[PersonnelImportLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ImportId] NVARCHAR(50) NOT NULL, -- 匯入批次編號
    [FileName] NVARCHAR(500) NULL, -- 檔案名稱
    [TotalCount] INT NULL DEFAULT 0, -- 總筆數
    [SuccessCount] INT NULL DEFAULT 0, -- 成功筆數
    [FailCount] INT NULL DEFAULT 0, -- 失敗筆數
    [ImportStatus] NVARCHAR(20) NOT NULL, -- 匯入狀態 (PENDING, PROCESSING, SUCCESS, FAILED)
    [ImportDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 匯入日期
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_PersonnelImportLogs_ImportId] UNIQUE ([ImportId])
);
```

#### 2.2.2 `PersonnelImportDetails` - 人事匯入明細
```sql
CREATE TABLE [dbo].[PersonnelImportDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ImportId] NVARCHAR(50) NOT NULL, -- 匯入批次編號
    [RowNum] INT NOT NULL, -- 行號
    [PersonnelId] NVARCHAR(50) NULL, -- 人事編號
    [PersonnelName] NVARCHAR(100) NULL, -- 人事姓名
    [ImportStatus] NVARCHAR(20) NOT NULL, -- 匯入狀態 (SUCCESS, FAILED)
    [ErrorMessage] NVARCHAR(1000) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PersonnelImportDetails_PersonnelImportLogs] FOREIGN KEY ([ImportId]) REFERENCES [dbo].[PersonnelImportLogs] ([ImportId]) ON DELETE CASCADE
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 上傳Excel檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/personnel/batch-import/upload`
- **說明**: 上傳Excel檔案進行人事批量新增
- **請求格式**: multipart/form-data
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "上傳成功",
    "data": {
      "importId": "IMP001",
      "fileName": "personnel.xlsx",
      "totalCount": 100
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 執行批量新增
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/personnel/batch-import/{importId}/execute`
- **說明**: 執行批量新增作業
- **路徑參數**:
  - `importId`: 匯入批次編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "批量新增成功",
    "data": {
      "importId": "IMP001",
      "totalCount": 100,
      "successCount": 95,
      "failCount": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢匯入進度
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/personnel/batch-import/{importId}/progress`
- **說明**: 查詢批量新增進度
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "importId": "IMP001",
      "importStatus": "PROCESSING",
      "totalCount": 100,
      "processedCount": 50,
      "successCount": 48,
      "failCount": 2,
      "progress": 50
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 查詢匯入結果
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/personnel/batch-import/{importId}`
- **說明**: 查詢批量新增結果
- **回應格式**: 標準單筆回應格式

#### 3.1.5 匯出失敗資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/personnel/batch-import/{importId}/export-failed`
- **說明**: 匯出失敗的資料為Excel檔案
- **回應格式**: 返回Excel檔案下載連結

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 人事批量新增頁面 (`PersonnelBatchImport.vue`)
- **路徑**: `/personnel/batch-import`
- **功能**: 上傳Excel檔案、執行批量新增、查詢進度、查看結果

### 4.2 UI 元件設計

#### 4.2.1 檔案上傳元件
```vue
<template>
  <el-upload
    :action="uploadUrl"
    :on-success="handleUploadSuccess"
    :on-error="handleUploadError"
    :before-upload="beforeUpload"
    :file-list="fileList"
    accept=".xlsx,.xls"
  >
    <el-button type="primary">選擇檔案</el-button>
    <template #tip>
      <div class="el-upload__tip">
        只能上傳 xlsx/xls 檔案，且不超過 10MB
      </div>
    </template>
  </el-upload>
</template>
```

#### 4.2.2 進度顯示元件
```vue
<template>
  <el-progress
    :percentage="progress"
    :status="progressStatus"
  />
  <div class="progress-info">
    <span>總筆數: {{ totalCount }}</span>
    <span>已處理: {{ processedCount }}</span>
    <span>成功: {{ successCount }}</span>
    <span>失敗: {{ failCount }}</span>
  </div>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立匯入記錄資料表
- [ ] 建立匯入明細資料表
- [ ] 建立索引

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] Excel解析功能實作
- [ ] 批量新增功能實作
- [ ] 進度追蹤功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 檔案上傳功能開發
- [ ] 進度顯示功能開發
- [ ] 結果顯示功能開發
- [ ] 錯誤處理
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 檔案上傳必須驗證檔案類型
- 檔案大小必須限制
- 必須實作權限檢查

### 6.2 效能
- 大量資料處理必須使用批次處理
- 必須支援非同步處理
- 必須提供進度追蹤

### 6.3 資料驗證
- Excel檔案格式必須驗證
- 每筆資料必須驗證
- 錯誤資料必須記錄

### 6.4 業務邏輯
- 重複資料必須處理
- 失敗資料必須匯出
- 匯入結果必須記錄

---

## 七、測試案例

### 7.1 單元測試
- [ ] 上傳Excel檔案成功
- [ ] 解析Excel檔案成功
- [ ] 批量新增成功
- [ ] 進度追蹤成功
- [ ] 錯誤處理成功

### 7.2 整合測試
- [ ] 完整批量新增流程測試
- [ ] 大量資料處理測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSPH00/SYSH3D0_FMI.ASP`

### 8.2 相關功能
- SYSPED0-人事資料維護系列（人事資料維護功能）
- SYSH110-人事基本資料維護系列（人事基本資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

