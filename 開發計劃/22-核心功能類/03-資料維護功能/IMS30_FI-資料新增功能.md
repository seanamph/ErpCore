# IMS30_FI - 資料新增功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30_FI
- **功能名稱**: 資料新增功能
- **功能描述**: 提供通用的資料新增UI組件，用於新增資料記錄，支援表單驗證、資料儲存等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FI.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FI.aspx.cs`
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FI.aspx`
  - `WEB/IMS_CORE/Kernel/IMS30_FI.aspx`

### 1.2 業務需求
- 提供通用的資料新增介面
- 支援動態表單生成
- 支援表單驗證
- 支援資料儲存
- 支援資料預設值設定
- 可被其他功能模組重用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `DataInsertConfigs` (資料新增設定)

```sql
CREATE TABLE [dbo].[DataInsertConfigs] (
    [ConfigId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 模組代碼
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [FormFields] NVARCHAR(MAX) NULL, -- 表單欄位 (JSON格式)
    [DefaultValues] NVARCHAR(MAX) NULL, -- 預設值 (JSON格式)
    [ValidationRules] NVARCHAR(MAX) NULL, -- 驗證規則 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_DataInsertConfigs] PRIMARY KEY CLUSTERED ([ConfigId] ASC),
    CONSTRAINT [UQ_DataInsertConfigs_ModuleCode] UNIQUE ([ModuleCode])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_DataInsertConfigs_ModuleCode] ON [dbo].[DataInsertConfigs] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_DataInsertConfigs_Status] ON [dbo].[DataInsertConfigs] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConfigId | BIGINT | - | NO | IDENTITY(1,1) | 設定ID | 主鍵 |
| ModuleCode | NVARCHAR | 50 | NO | - | 模組代碼 | 唯一 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | - |
| FormFields | NVARCHAR(MAX) | - | YES | - | 表單欄位 | JSON格式 |
| DefaultValues | NVARCHAR(MAX) | - | YES | - | 預設值 | JSON格式 |
| ValidationRules | NVARCHAR(MAX) | - | YES | - | 驗證規則 | JSON格式 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得新增設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/data-insert/{moduleCode}/config`
- **說明**: 取得資料新增設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "moduleCode": "SYS0110",
      "tableName": "Users",
      "formFields": [],
      "defaultValues": {},
      "validationRules": {}
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 新增資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-insert/{moduleCode}`
- **說明**: 新增資料記錄
- **請求格式**:
  ```json
  {
    "field1": "value1",
    "field2": "value2"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "id": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增新增設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/data-insert/config`
- **說明**: 新增資料新增設定

#### 3.1.4 修改新增設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/data-insert/config/{configId}`
- **說明**: 修改資料新增設定

#### 3.1.5 刪除新增設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/data-insert/config/{configId}`
- **說明**: 刪除資料新增設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `DataInsertController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/data-insert")]
    [Authorize]
    public class DataInsertController : ControllerBase
    {
        private readonly IDataInsertService _dataInsertService;
        
        public DataInsertController(IDataInsertService dataInsertService)
        {
            _dataInsertService = dataInsertService;
        }
        
        [HttpGet("{moduleCode}/config")]
        public async Task<ActionResult<ApiResponse<DataInsertConfigDto>>> GetConfig(string moduleCode)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPost("{moduleCode}")]
        public async Task<ActionResult<ApiResponse<object>>> InsertData(string moduleCode, [FromBody] Dictionary<string, object> data)
        {
            // 實作新增資料邏輯
        }
        
        [HttpPost("config")]
        public async Task<ActionResult<ApiResponse<long>>> CreateConfig([FromBody] CreateDataInsertConfigDto dto)
        {
            // 實作新增設定邏輯
        }
        
        [HttpPut("config/{configId}")]
        public async Task<ActionResult<ApiResponse>> UpdateConfig(long configId, [FromBody] UpdateDataInsertConfigDto dto)
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

#### 4.1.1 資料新增頁面 (`DataInsert.vue`)
- **路徑**: `/kernel/data-insert/:moduleCode`
- **功能**: 顯示新增表單，支援表單驗證、資料儲存
- **主要元件**:
  - 動態表單 (DynamicForm)
  - 表單驗證 (FormValidation)

### 4.2 UI 元件設計

#### 4.2.1 資料新增元件 (`DataInsert.vue`)
```vue
<template>
  <div class="data-insert">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ moduleName }} - 新增</span>
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
          :default-values="defaultValues"
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

### 4.3 API 呼叫 (`dataInsert.api.ts`)
```typescript
import request from '@/utils/request';

export interface DataInsertConfigDto {
  moduleCode: string;
  tableName: string;
  formFields: any[];
  defaultValues: Record<string, any>;
  validationRules: Record<string, any>;
}

// API 函數
export const getInsertConfig = (moduleCode: string) => {
  return request.get<ApiResponse<DataInsertConfigDto>>(`/api/v1/kernel/data-insert/${moduleCode}/config`);
};

export const insertData = (moduleCode: string, data: Record<string, any>) => {
  return request.post<ApiResponse<any>>(`/api/v1/kernel/data-insert/${moduleCode}`, data);
};

export const createInsertConfig = (data: CreateDataInsertConfigDto) => {
  return request.post<ApiResponse<number>>('/api/v1/kernel/data-insert/config', data);
};

export const updateInsertConfig = (configId: number, data: UpdateDataInsertConfigDto) => {
  return request.put<ApiResponse>(`/api/v1/kernel/data-insert/config/${configId}`, data);
};

export const deleteInsertConfig = (configId: number) => {
  return request.delete<ApiResponse>(`/api/v1/kernel/data-insert/config/${configId}`);
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
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 資料新增頁面開發
- [ ] 動態表單元件開發
- [ ] 表單驗證開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
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

### 6.2 資料驗證
- 必填欄位必須驗證
- 資料格式必須驗證
- 資料範圍必須驗證
- 唯一性必須驗證

### 6.3 業務邏輯
- 必須記錄建立者
- 必須記錄建立時間
- 必須處理預設值
- 必須處理關聯資料

---

## 七、測試案例

### 7.1 單元測試
- [ ] 取得新增設定成功
- [ ] 新增資料成功
- [ ] 新增資料失敗 (驗證錯誤)
- [ ] 新增資料失敗 (重複資料)
- [ ] 新增設定CRUD測試

### 7.2 整合測試
- [ ] 完整新增流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FI.aspx.cs`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_FI.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

