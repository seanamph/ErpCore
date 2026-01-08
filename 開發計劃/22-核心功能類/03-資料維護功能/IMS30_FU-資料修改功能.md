# IMS30_FU - 資料修改功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_FU
- **功能名稱**: 資料修改功能
- **功能描述**: 提供通用的資料修改UI組件，用於修改資料記錄，支援表單驗證、資料更新等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FU.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FU.aspx.cs`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FU.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_FU.aspx`

### 1.2 業務需求
- 提供通用的資料修改介面
- 支援動態表單生成
- 支援表單驗證
- 支援資料更新
- 支援樂觀鎖機制
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataUpdateConfigs` (資料修改設定)

```sql
CREATE TABLE [dbo].[DataUpdateConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [FormFields] NVARCHAR(MAX) NULL, -- 表單欄位 (JSON格式)
    [ReadOnlyFields] NVARCHAR(MAX) NULL, -- 唯讀欄位 (JSON格式)
    [ValidationRules] NVARCHAR(MAX) NULL, -- 驗證規則 (JSON格式)
    [UseOptimisticLock] BIT NOT NULL DEFAULT 1, -- 使用樂觀鎖
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataUpdateConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataUpdateConfigs_ModuleCode] UNIQUE ([ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataUpdateConfigs_ModuleCode] ON [dbo].[DataUpdateConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataUpdateConfigs_Status] ON [dbo].[DataUpdateConfigs] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | 唯一 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | - |
| FormFields | NVARCHAR(MAX) | - | YES | - | 表單欄位 | JSON格式 |
| ReadOnlyFields | NVARCHAR(MAX) | - | YES | - | 唯讀欄位 | JSON格式 |
| ValidationRules | NVARCHAR(MAX) | - | YES | - | 驗證規則 | JSON格式 |
| UseOptimisticLock | BIT | - | NO | 1 | 使用樂觀鎖 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢單筆資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-update/{moduleCode}/{id}`
- **說明**: 根據ID查詢單筆資料用於修改
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "id": 1,
      "field1": "value1",
      "field2": "value2",
      "rowVersion": "base64string"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 取得修改設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-update/{moduleCode}/config`
- **說明**: 取得資料修改設定

#### 3.1.3 修改資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/data-update/{moduleCode}/{id}`
- **說明**: 修改資料記錄
- **請求格式**:
  ```json
  {
    "field1": "value1",
    "field2": "value2",
    "rowVersion": "base64string"
  }
  ```

#### 3.1.4 新增修改設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-update/config`
- **說明**: 新增資料修改設定

#### 3.1.5 修改修改設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/data-update/config/{configId}`
- **說明**: 修改資料修改設定

#### 3.1.6 刪除修改設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-update/config/{configId}`
- **說明**: 刪除資料修改設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataUpdateController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-update")]
    [Authorize]
    public class DataUpdateController : ControllerBase
    {
        private readonly IDataUpdateService _dataUpdateService;
        
        public DataUpdateController(IDataUpdateService dataUpdateService)
        {
            _dataUpdateService = dataUpdateService;
        }
        
        [HttpGet("{moduleCode}/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetData(string moduleCode, string id)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<DataUpdateConfigDto>>> GetConfig(string moduleCode)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPut("{moduleCode}/{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateData(string moduleCode, string id, [FromBody] Dictionary<string, object> data)
        {
            // 實作修改資料邏輯
        }
        
        [HttpPost("config")]
        public async Task<ActionResult<ApiResponse<long>>> CreateConfig([FromBody] CreateDataUpdateConfigDto dto)
        {
            // 實作新增設定邏輯
        }
        
        [HttpPut("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> UpdateConfig(long configId, [FromBody] UpdateDataUpdateConfigDto dto)
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

#### 4.1.1 資料修改頁面 (`DataUpdate.vue`)
- **路徑**: `/kernel/data-update/:moduleCode/:id`
- **功能**: 顯示修改表單，支援表單驗證、資料更新
- **主要元件**:
  - 動態表單 (DynamicForm)
  - 表單驗證 (FormValidation)

### 4.2 UI 元件設計

#### 4.2.1 資料修改元件 (`DataUpdate.vue`)
```vue
<template>
  <div class="data-update">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - 修改</span>
        </div>
      </template>
      
      <!-- 動態表單 -->
      <el-form 
        :model="form" 
        :rules="rules" 
        ref="formRef" 
        label-width="120px"
        v-loading="loading"
      >
        <DynamicForm 
          :fields="formFields" 
          v-model="form"
          :read-only-fields="readOnlyFields"
        />
        
        <el-form-item>
          <el-button type="primary" @click="handleSubmit">確定</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button @click="handleCancel">取消</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`dataUpdate.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataUpdateConfigDto {
  moduleCode: string;
  tableName: string;
  formFields: any[];
  readOnlyFields: string[];
  validationRules: Record<string, any>;
  useOptimisticLock: boolean;
}

// API 函數
export const getDataForUpdate = (moduleCode: string, id: string) => {
  return request.get<ApiResponse<any>>(`/api/v1/kernel/data-update/${moduleCode}/${id}`);
};

export const getUpdateConfig = (moduleCode: string) => {
  return request.get<ApiResponse<DataUpdateConfigDto>>(`/api/v1/kernel/data-update/${moduleCode}/config`);
};

export const updateData = (moduleCode: string, id: string, data: Record<string, any>) => {
  return request.put<ApiResponse>(`/api/v1/kernel/data-update/${moduleCode}/${id}`, data);
};

export const createUpdateConfig = (data: CreateDataUpdateConfigDto) => {
  return request.post<ApiResponse<number>>('/api/v1/kernel/data-update/config', data);
};

export const updateUpdateConfig = (configId: number, data: UpdateDataUpdateConfigDto) => {
  return request.put<ApiResponse>(`/api/v1/kernel/data-update/config/${configId}`, data);
};

export const deleteUpdateConfig = (configId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-update/config/${configId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 樂觀鎖機制實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 資料修改頁面開發
- [ ] 動態表單元件開發
- [ ] 表單驗證開發
- [ ] 樂觀鎖處理
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 並發測試
- [ ] 資料驗證測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須防止SQL注入
- 必須驗證輸入參數
- 必須驗證表單資料

### 6.2 並發控制
- 必須實作樂觀鎖機制
- 必須處理並發衝突
- 必須提示使用者資料已被修改

### 6.3 資料驗證
- 必填欄位必須驗證
- 資料格式必須驗證
- 資料範圍必須驗證
- 唯一性必須驗證

### 6.4 業務邏輯
- 必須記錄更新者
- 必須記錄更新時間
- 必須處理唯讀欄位
- 必須處理關聯資料

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢單筆資料成功
- [ ] 取得修改設定成功
- [ ] 修改資料成功
- [ ] 修改資料失敗 (驗證錯誤)
- [ ] 修改資料失敗 (並發衝突)
- [ ] 修改設定CRUD測試

### 7.2 整合測試
- [ ] 完整修改流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 並發測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FU.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FU.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

