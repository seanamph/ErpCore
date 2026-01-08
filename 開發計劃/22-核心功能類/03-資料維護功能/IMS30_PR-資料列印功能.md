# IMS30_PR - 資料列印功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_PR
- **功能名稱**: 資料列印功能
- **功能描述**: 提供通用的資料列印UI組件，用於列印資料記錄，支援報表模板、預覽、列印等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_PR.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_PR_V2.aspx`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_PR.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_PR.aspx`

### 1.2 業務需求
- 提供通用的資料列印介面
- 支援報表模板設定
- 支援列印預覽
- 支援PDF列印
- 支援多種列印格式
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataPrintConfigs` (資料列印設定)

```sql
CREATE TABLE [dbo].[DataPrintConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [TemplatePath] NVARCHAR(500) NULL, -- 模板路徑
    [TemplateType] NVARCHAR(20) NOT NULL, -- 模板類型 (RDLC, HTML, PDF)
    [PrintFields] NVARCHAR(MAX) NULL, -- 列印欄位 (JSON格式)
    [PrintSettings] NVARCHAR(MAX) NULL, -- 列印設定 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataPrintConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataPrintConfigs_ModuleCode_ReportName] UNIQUE ([ModuleCode], [ReportName])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataPrintConfigs_ModuleCode] ON [dbo].[DataPrintConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataPrintConfigs_Status] ON [dbo].[DataPrintConfigs] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | - |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| TemplatePath | NVARCHAR | 500 | YES | - | 模板路徑 | - |
| TemplateType | NVARCHAR | 20 | NO | - | 模板類型 | RDLC, HTML, PDF |
| PrintFields | NVARCHAR(MAX) | - | YES | - | 列印欄位 | JSON格式 |
| PrintSettings | NVARCHAR(MAX) | - | YES | - | 列印設定 | JSON格式 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得列印設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-print/{moduleCode}/config`
- **說明**: 取得資料列印設定列表
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
        "reportName": "使用者清單",
        "templatePath": "/templates/user-list.rdlc",
        "templateType": "RDLC"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 列印預覽
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-print/{moduleCode}/preview`
- **說明**: 產生列印預覽
- **請求格式**:
  ```json
  {
    "configId": 1,
    "data": {},
    "parameters": {}
  }
  ```
- **回應格式**: PDF或HTML內容

#### 3.1.3 列印資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-print/{moduleCode}/print`
- **說明**: 列印資料
- **請求格式**: 同預覽

#### 3.1.4 新增列印設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-print/config`
- **說明**: 新增資料列印設定

#### 3.1.5 修改列印設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/data-print/config/{configId}`
- **說明**: 修改資料列印設定

#### 3.1.6 刪除列印設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-print/config/{configId}`
- **說明**: 刪除資料列印設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataPrintController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-print")]
    [Authorize]
    public class DataPrintController : ControllerBase
    {
        private readonly IDataPrintService _dataPrintService;
        
        public DataPrintController(IDataPrintService dataPrintService)
        {
            _dataPrintService = dataPrintService;
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<List<DataPrintConfigDto>>>> GetConfigs(string moduleCode)
        {
            // 實作取得設定列表邏輯
        }
        
        [HttpPost("{moduleCode}/preview")]
        public async Task<ActionResult> Preview(string moduleCode, [FromBody] PrintRequestDto dto)
        {
            // 實作列印預覽邏輯
        }
        
        [HttpPost("{moduleCode}/print")]
        public async Task<ActionResult> Print(string moduleCode, [FromBody] PrintRequestDto dto)
        {
            // 實作列印邏輯
        }
        
        [HttpPost("config")]
        public async Task<ActionResult<ApiResponse<long>>> CreateConfig([FromBody] CreateDataPrintConfigDto dto)
        {
            // 實作新增設定邏輯
        }
        
        [HttpPut("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> UpdateConfig(long configId, [FromBody] UpdateDataPrintConfigDto dto)
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

#### 4.1.1 資料列印頁面 (`DataPrint.vue`)
- **路徑**: `/kernel/data-print/:moduleCode`
- **功能**: 顯示列印設定和預覽，支援列印功能
- **主要元件**:
  - 列印設定選擇 (PrintConfigSelect)
  - 列印預覽 (PrintPreview)
  - 列印按鈕

### 4.2 UI 元件設計

#### 4.2.1 資料列印元件 (`DataPrint.vue`)
```vue
<template>
  <div class="data-print">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - 列印</span>
        </div>
      </template>
      
      <!-- 列印設定選擇 -->
      <el-form :model="printForm" label-width="120px">
        <el-form-item label="報表模板">
          <el-select v-model="printForm.configId" placeholder="請選擇報表模板">
            <el-option 
              v-for="config in printConfigs" 
              :key="config.configId" 
              :label="config.reportName" 
              :value="config.configId" 
            />
          </el-select>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handlePreview">預覽</el-button>
          <el-button type="success" @click="handlePrint">列印</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 列印預覽 -->
      <PrintPreview 
        v-if="previewUrl" 
        :url="previewUrl" 
      />
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`dataPrint.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataPrintConfigDto {
  configId: number;
  moduleCode: string;
  reportName: string;
  templatePath: string;
  templateType: string;
  printFields: any[];
  printSettings: Record<string, any>;
}

export interface PrintRequestDto {
  configId: number;
  data: Record<string, any>;
  parameters: Record<string, any>;
}

// API 函數
export const getPrintConfigs = (moduleCode: string) => {
  return request.get<ApiResponse<DataPrintConfigDto[]>>(`/api/v1/kernel/data-print/${moduleCode}/config`);
};

export const previewPrint = (moduleCode: string, data: PrintRequestDto) => {
  return request.post(`/api/v1/kernel/data-print/${moduleCode}/preview`, data, { responseType: 'blob' });
};

export const printData = (moduleCode: string, data: PrintRequestDto) => {
  return request.post(`/api/v1/kernel/data-print/${moduleCode}/print`, data, { responseType: 'blob' });
};

export const createPrintConfig = (data: CreateDataPrintConfigDto) => {
  return request.post<ApiResponse<number>>('/api/v1/kernel/data-print/config', data);
};

export const updatePrintConfig = (configId: number, data: UpdateDataPrintConfigDto) => {
  return request.put<ApiResponse>(`/api/v1/kernel/data-print/config/${configId}`, data);
};

export const deletePrintConfig = (configId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-print/config/${configId}`);
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
- [ ] 報表引擎整合 (RDLC/HTML/PDF)
- [ ] 列印功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 資料列印頁面開發
- [ ] 列印預覽元件開發
- [ ] 列印功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 列印功能測試

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
- 必須限制列印資料量

### 6.2 效能
- 大量資料列印時必須使用背景任務
- 必須使用快取機制
- 必須優化報表生成效能

### 6.3 報表模板
- 必須支援多種報表格式
- 必須支援動態欄位
- 必須支援參數化查詢

---

## 七、測試案例

### 7.1 單元測試
- [ ] 取得列印設定成功
- [ ] 列印預覽成功
- [ ] 列印資料成功
- [ ] 列印設定CRUD測試

### 7.2 整合測試
- [ ] 完整列印流程測試
- [ ] 權限檢查測試
- [ ] 報表格式測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_PR.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_PR.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

