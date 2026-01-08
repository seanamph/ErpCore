# Export_Excel - Excel匯出功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Export_Excel
- **功能名稱**: Excel匯出功能
- **功能描述**: 提供通用的Excel匯出功能，用於將資料匯出為Excel格式，支援多種匯出格式、樣式設定等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/Kernel/Export_Excel.aspx`

### 1.2 業務需求
- 提供通用的Excel匯出介面
- 支援多種資料格式匯出
- 支援Excel樣式設定
- 支援大量資料匯出
- 支援背景任務處理
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ExcelExportConfigs` (Excel匯出設定)

```sql
CREATE TABLE [dbo].[ExcelExportConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [ExportName] NVARCHAR(200) NOT NULL, -- 匯出名稱
    [ExportFields] NVARCHAR(MAX) NULL, -- 匯出欄位 (JSON格式)
    [ExportSettings] NVARCHAR(MAX) NULL, -- 匯出設定 (JSON格式)
    [TemplatePath] NVARCHAR(500) NULL, -- 模板路徑
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ExcelExportConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_ExcelExportConfigs_ModuleCode_ExportName] UNIQUE ([ModuleCode], [ExportName])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ExcelExportConfigs_ModuleCode] ON [dbo].[ExcelExportConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_ExcelExportConfigs_Status] ON [dbo].[ExcelExportConfigs] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `ExcelExportLogs` - Excel匯出記錄
```sql
CREATE TABLE [dbo].[ExcelExportLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL,
    [ConfigId] BIGINT NULL,
    [UserId] NVARCHAR(50) NOT NULL,
    [FileName] NVARCHAR(500) NOT NULL,
    [FileSize] BIGINT NULL,
    [RecordCount] INT NULL,
    [Status] NVARCHAR(20) NOT NULL, -- PENDING, PROCESSING, COMPLETED, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CompletedAt] DATETIME2 NULL,
    CONSTRAINT [FK_ExcelExportLogs_ExcelExportConfigs] FOREIGN KEY ([ConfigId]) REFERENCES [dbo].[ExcelExportConfigs] ([ConfigId]),
    CONSTRAINT [FK_ExcelExportLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_ModuleCode] ON [dbo].[ExcelExportLogs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_UserId] ON [dbo].[ExcelExportLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_ExcelExportLogs_Status] ON [dbo].[ExcelExportLogs] ([Status]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | - |
| ExportName | NVARCHAR | 200 | NO | - | 匯出名稱 | - |
| ExportFields | NVARCHAR(MAX) | - | YES | - | 匯出欄位 | JSON格式 |
| ExportSettings | NVARCHAR(MAX) | - | YES | - | 匯出設定 | JSON格式 |
| TemplatePath | NVARCHAR | 500 | YES | - | 模板路徑 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得匯出設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/excel-export/{moduleCode}/config`
- **說明**: 取得Excel匯出設定列表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "configId": 1,
        "moduleCode": "SYS0110",
        "exportName": "使用者清單",
        "exportFields": [],
        "exportSettings": {}
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/excel-export/{moduleCode}/export`
- **說明**: 匯出資料為Excel格式
- **請求格式**:
  ```json
  {
    "configId": 1,
    "filters": {},
    "exportFields": [],
    "async": false
  }
  ```
- **回應格式**: Excel檔案 (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

#### 3.1.3 查詢匯出任務狀態
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/excel-export/tasks/{taskId}`
- **說明**: 查詢背景匯出任務狀態
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "taskId": "guid",
      "status": "COMPLETED",
      "fileName": "export.xlsx",
      "downloadUrl": "/api/v1/kernel/excel-export/download/guid"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 下載匯出檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/excel-export/download/{taskId}`
- **說明**: 下載已完成的匯出檔案

#### 3.1.5 新增匯出設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/excel-export/config`
- **說明**: 新增Excel匯出設定

#### 3.1.6 修改匯出設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/excel-export/config/{configId}`
- **說明**: 修改Excel匯出設定

#### 3.1.7 刪除匯出設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/excel-export/config/{configId}`
- **說明**: 刪除Excel匯出設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `ExcelExportController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/excel-export")]
    [Authorize]
    public class ExcelExportController : ControllerBase
    {
        private readonly IExcelExportService _excelExportService;
        
        public ExcelExportController(IExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<List<ExcelExportConfigDto>>>> GetConfigs(string moduleCode)
        {
            // 實作取得設定列表邏輯
        }
        
        [HttpPost("{moduleCode}/export")]
        public async Task<ActionResult> Export(string moduleCode, [FromBody] ExcelExportRequestDto dto)
        {
            // 實作匯出邏輯
        }
        
        [HttpGet("tasks/{taskId}")]
        public async Task<ActionResult<ApiResponse<ExcelExportTaskDto>>> GetTaskStatus(string taskId)
        {
            // 實作查詢任務狀態邏輯
        }
        
        [HttpGet("download/{taskId}")]
        public async Task<ActionResult> Download(string taskId)
        {
            // 實作下載檔案邏輯
        }
        
        [HttpPost("config")]
        public async Task<ActionResult<ApiResponse<long>>> CreateConfig([FromBody] CreateExcelExportConfigDto dto)
        {
            // 實作新增設定邏輯
        }
        
        [HttpPut("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> UpdateConfig(long configId, [FromBody] UpdateExcelExportConfigDto dto)
        {
            // 實作修改設定邏輯
        }
        
        [HttpDelete("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> DeleteConfig(long configId)
        {
            // 實作刪除設定邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 Excel匯出頁面 (`ExcelExport.vue`)
- **路徑**: `/kernel/excel-export/:moduleCode`
- **功能**: 顯示匯出設定和匯出功能
- **主要元件**:
  - 匯出設定選擇 (ExportConfigSelect)
  - 匯出選項設定 (ExportOptions)
  - 匯出進度顯示 (ExportProgress)

### 4.2 UI 元件設計

#### 4.2.1 Excel匯出元件 (`ExcelExport.vue`)
```vue
<template>
  <div class="excel-export">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - Excel匯出</span>
        </div>
      </template>
      
      <!-- 匯出設定 -->
      <el-form :model="exportForm" label-width="120px">
        <el-form-item label="匯出模板">
          <el-select v-model="exportForm.configId" placeholder="請選擇匯出模板">
            <el-option 
              v-for="config in exportConfigs" 
              :key="config.configId" 
              :label="config.exportName" 
              :value="config.configId" 
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="匯出方式">
          <el-radio-group v-model="exportForm.async">
            <el-radio :label="false">同步匯出</el-radio>
            <el-radio :label="true">背景匯出</el-radio>
          </el-radio-group>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleExport" :loading="exporting">匯出</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 匯出進度 -->
      <ExportProgress 
        v-if="exportTask" 
        :task="exportTask"
        @download="handleDownload"
      />
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`excelExport.api.ts`)
```typescript
import request from '@/utils/request';

export interface ExcelExportConfigDto {
  configId: number;
  moduleCode: string;
  exportName: string;
  exportFields: any[];
  exportSettings: Record<string, any>;
  templatePath?: string;
}

export interface ExcelExportRequestDto {
  configId: number;
  filters: Record<string, any>;
  exportFields?: any[];
  async: boolean;
}

export interface ExcelExportTaskDto {
  taskId: string;
  status: 'PENDING' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
  fileName?: string;
  downloadUrl?: string;
  errorMessage?: string;
}

// API 函數
export const getExportConfigs = (moduleCode: string) => {
  return request.get<ApiResponse<ExcelExportConfigDto[]>>(`/api/v1/kernel/excel-export/${moduleCode}/config`);
};

export const exportExcel = (moduleCode: string, data: ExcelExportRequestDto) => {
  if (data.async) {
    return request.post<ApiResponse<ExcelExportTaskDto>>(`/api/v1/kernel/excel-export/${moduleCode}/export`, data);
  } else {
    return request.post(`/api/v1/kernel/excel-export/${moduleCode}/export`, data, { responseType: 'blob' });
  }
};

export const getTaskStatus = (taskId: string) => {
  return request.get<ApiResponse<ExcelExportTaskDto>>(`/api/v1/kernel/excel-export/tasks/${taskId}`);
};

export const downloadExportFile = (taskId: string) => {
  return request.get(`/api/v1/kernel/excel-export/download/${taskId}`, { responseType: 'blob' });
};

export const createExportConfig = (data: CreateExcelExportConfigDto) => {
  return request.post<ApiResponse<number>>('/api/v1/kernel/excel-export/config', data);
};

export const updateExportConfig = (configId: number, data: UpdateExcelExportConfigDto) => {
  return request.put<ApiResponse>(`/api/v1/kernel/excel-export/config/${configId}`, data);
};

export const deleteExportConfig = (configId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/excel-export/config/${configId}`);
};
```

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
- [ ] DTO 類別建立
- [ ] Excel匯出功能實作 (EPPlus/NPOI)
- [ ] 背景任務處理實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] Excel匯出頁面開發
- [ ] 匯出進度顯示元件開發
- [ ] 檔案下載功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 大量資料匯出測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須驗證輸入參數
- 必須限制匯出資料量
- 必須限制檔案下載次數

### 6.2 效能
- 大量資料匯出時必須使用背景任務
- 必須使用記憶體優化
- 必須使用檔案快取機制

### 6.3 資料驗證
- 匯出欄位必須驗證
- 匯出設定必須驗證
- 檔案大小必須限制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 取得匯出設定成功
- [ ] 同步匯出成功
- [ ] 背景匯出成功
- [ ] 查詢任務狀態成功
- [ ] 下載檔案成功
- [ ] 匯出設定CRUD測試

### 7.2 整合測試
- [ ] 完整匯出流程測試
- [ ] 權限檢查測試
- [ ] 大量資料匯出測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/Kernel/Export_Excel.aspx`

### 8.2 技術文件
- EPPlus 文件
- NPOI 文件

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

