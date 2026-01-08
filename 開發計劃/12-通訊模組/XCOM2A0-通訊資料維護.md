# XCOM2A0 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM2A0
- **功能名稱**: 通訊資料維護（系統參數設定）
- **功能描述**: 提供系統參數資料的新增、修改、刪除、查詢功能，包含參數代碼、參數名稱、參數值、參數類型、說明等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_PR.asp` (報表)

### 1.2 業務需求
- 管理系統參數設定資訊
- 支援參數的新增、修改、刪除、查詢
- 記錄參數的建立與變更資訊
- 支援參數類型的區分
- 支援參數值的驗證
- 與系統模組整合
- 支援參數的啟用/停用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XComSystemParams` (系統參數，對應舊系統 `XCOM_SYSPARAM`)

```sql
CREATE TABLE [dbo].[XComSystemParams] (
    [ParamCode] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 參數代碼
    [ParamName] NVARCHAR(100) NOT NULL, -- 參數名稱
    [ParamValue] NVARCHAR(500) NULL, -- 參數值
    [ParamType] NVARCHAR(20) NULL, -- 參數類型 (STRING, NUMBER, BOOLEAN, DATE等)
    [Description] NVARCHAR(500) NULL, -- 說明
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 A:啟用, I:停用
    [SystemId] NVARCHAR(50) NULL, -- 系統ID
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    [CreatedPriority] INT NULL, -- 建立者等級
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_XComSystemParams] PRIMARY KEY CLUSTERED ([ParamCode] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XComSystemParams_ParamName] ON [dbo].[XComSystemParams] ([ParamName]);
CREATE NONCLUSTERED INDEX [IX_XComSystemParams_Status] ON [dbo].[XComSystemParams] ([Status]);
CREATE NONCLUSTERED INDEX [IX_XComSystemParams_SystemId] ON [dbo].[XComSystemParams] ([SystemId]);
CREATE NONCLUSTERED INDEX [IX_XComSystemParams_ParamType] ON [dbo].[XComSystemParams] ([ParamType]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ParamCode | NVARCHAR | 50 | NO | - | 參數代碼 | 主鍵，唯一 |
| ParamName | NVARCHAR | 100 | NO | - | 參數名稱 | - |
| ParamValue | NVARCHAR | 500 | YES | - | 參數值 | - |
| ParamType | NVARCHAR | 20 | YES | - | 參數類型 | STRING, NUMBER, BOOLEAN, DATE等 |
| Description | NVARCHAR | 500 | YES | - | 說明 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至系統表 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢參數列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom2a0/system-params`
- **說明**: 查詢系統參數列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ParamCode",
    "sortOrder": "ASC",
    "filters": {
      "paramCode": "",
      "paramName": "",
      "paramType": "",
      "status": "",
      "systemId": ""
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
          "paramCode": "SYS_TIMEOUT",
          "paramName": "系統逾時時間",
          "paramValue": "30",
          "paramType": "NUMBER",
          "description": "系統登入逾時時間（分鐘）",
          "status": "A",
          "systemId": "SYS0000",
          "createdAt": "2024-01-01T00:00:00",
          "updatedAt": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆參數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom2a0/system-params/{paramCode}`
- **說明**: 根據參數代碼查詢單筆參數資料
- **路徑參數**:
  - `paramCode`: 參數代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "paramCode": "SYS_TIMEOUT",
      "paramName": "系統逾時時間",
      "paramValue": "30",
      "paramType": "NUMBER",
      "description": "系統登入逾時時間（分鐘）",
      "status": "A",
      "systemId": "SYS0000",
      "createdBy": "U001",
      "createdAt": "2024-01-01T00:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增參數
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom2a0/system-params`
- **說明**: 新增系統參數資料
- **請求格式**:
  ```json
  {
    "paramCode": "SYS_TIMEOUT",
    "paramName": "系統逾時時間",
    "paramValue": "30",
    "paramType": "NUMBER",
    "description": "系統登入逾時時間（分鐘）",
    "status": "A",
    "systemId": "SYS0000"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "paramCode": "SYS_TIMEOUT"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改參數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom2a0/system-params/{paramCode}`
- **說明**: 修改系統參數資料
- **路徑參數**:
  - `paramCode`: 參數代碼
- **請求格式**: 同新增，但 `paramCode` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除參數
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom2a0/system-params/{paramCode}`
- **說明**: 刪除系統參數資料（軟刪除或硬刪除）
- **路徑參數**:
  - `paramCode`: 參數代碼
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

#### 3.1.6 批次刪除參數
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom2a0/system-params/batch`
- **說明**: 批次刪除多筆參數
- **請求格式**:
  ```json
  {
    "paramCodes": ["SYS_TIMEOUT", "SYS_MAX_LOGIN", "SYS_PWD_EXPIRE"]
  }
  ```

#### 3.1.7 啟用/停用參數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom2a0/system-params/{paramCode}/status`
- **說明**: 啟用或停用系統參數
- **請求格式**:
  ```json
  {
    "status": "A" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom2A0Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom2a0/system-params")]
    [Authorize]
    public class XCom2A0Controller : ControllerBase
    {
        private readonly IXComSystemParamService _systemParamService;
        
        public XCom2A0Controller(IXComSystemParamService systemParamService)
        {
            _systemParamService = systemParamService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<XComSystemParamDto>>>> GetSystemParams([FromQuery] XComSystemParamQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{paramCode}")]
        public async Task<ActionResult<ApiResponse<XComSystemParamDto>>> GetSystemParam(string paramCode)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateSystemParam([FromBody] CreateXComSystemParamDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{paramCode}")]
        public async Task<ActionResult<ApiResponse>> UpdateSystemParam(string paramCode, [FromBody] UpdateXComSystemParamDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{paramCode}")]
        public async Task<ActionResult<ApiResponse>> DeleteSystemParam(string paramCode)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `XComSystemParamService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXComSystemParamService
    {
        Task<PagedResult<XComSystemParamDto>> GetSystemParamsAsync(XComSystemParamQueryDto query);
        Task<XComSystemParamDto> GetSystemParamByIdAsync(string paramCode);
        Task<string> CreateSystemParamAsync(CreateXComSystemParamDto dto);
        Task UpdateSystemParamAsync(string paramCode, UpdateXComSystemParamDto dto);
        Task DeleteSystemParamAsync(string paramCode);
        Task UpdateStatusAsync(string paramCode, string status);
    }
}
```

#### 3.2.3 Repository: `XComSystemParamRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXComSystemParamRepository
    {
        Task<XComSystemParam> GetByIdAsync(string paramCode);
        Task<PagedResult<XComSystemParam>> GetPagedAsync(XComSystemParamQuery query);
        Task<XComSystemParam> CreateAsync(XComSystemParam systemParam);
        Task<XComSystemParam> UpdateAsync(XComSystemParam systemParam);
        Task DeleteAsync(string paramCode);
        Task<bool> ExistsAsync(string paramCode);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統參數列表頁面 (`SystemParamList.vue`)
- **路徑**: `/xcom/system-params`
- **功能**: 顯示系統參數列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (SystemParamSearchForm)
  - 資料表格 (SystemParamDataTable)
  - 新增/修改對話框 (SystemParamDialog)
  - 刪除確認對話框

#### 4.1.2 系統參數詳細頁面 (`SystemParamDetail.vue`)
- **路徑**: `/xcom/system-params/:paramCode`
- **功能**: 顯示系統參數詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SystemParamSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="參數代碼">
      <el-input v-model="searchForm.paramCode" placeholder="請輸入參數代碼" />
    </el-form-item>
    <el-form-item label="參數名稱">
      <el-input v-model="searchForm.paramName" placeholder="請輸入參數名稱" />
    </el-form-item>
    <el-form-item label="參數類型">
      <el-select v-model="searchForm.paramType" placeholder="請選擇參數類型">
        <el-option label="字串" value="STRING" />
        <el-option label="數值" value="NUMBER" />
        <el-option label="布林值" value="BOOLEAN" />
        <el-option label="日期" value="DATE" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SystemParamDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="systemParamList" v-loading="loading">
      <el-table-column prop="paramCode" label="參數代碼" width="150" />
      <el-table-column prop="paramName" label="參數名稱" width="200" />
      <el-table-column prop="paramValue" label="參數值" width="150" />
      <el-table-column prop="paramType" label="參數類型" width="100" />
      <el-table-column prop="description" label="說明" width="200" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

#### 4.2.3 新增/修改對話框 (`SystemParamDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="參數代碼" prop="paramCode">
        <el-input v-model="form.paramCode" :disabled="isEdit" placeholder="請輸入參數代碼" />
      </el-form-item>
      <el-form-item label="參數名稱" prop="paramName">
        <el-input v-model="form.paramName" placeholder="請輸入參數名稱" />
      </el-form-item>
      <el-form-item label="參數值" prop="paramValue">
        <el-input v-model="form.paramValue" placeholder="請輸入參數值" />
      </el-form-item>
      <el-form-item label="參數類型" prop="paramType">
        <el-select v-model="form.paramType" placeholder="請選擇參數類型">
          <el-option label="字串" value="STRING" />
          <el-option label="數值" value="NUMBER" />
          <el-option label="布林值" value="BOOLEAN" />
          <el-option label="日期" value="DATE" />
        </el-select>
      </el-form-item>
      <el-form-item label="說明" prop="description">
        <el-input v-model="form.description" type="textarea" :rows="3" placeholder="請輸入說明" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
        </el-select>
      </el-form-item>
      <el-form-item label="系統ID" prop="systemId">
        <el-select v-model="form.systemId" placeholder="請選擇系統">
          <el-option v-for="sys in systemList" :key="sys.systemId" :label="sys.systemName" :value="sys.systemId" />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom2a0.api.ts`)
```typescript
import request from '@/utils/request';

export interface XComSystemParamDto {
  paramCode: string;
  paramName: string;
  paramValue?: string;
  paramType?: string;
  description?: string;
  status: string;
  systemId?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface XComSystemParamQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    paramCode?: string;
    paramName?: string;
    paramType?: string;
    status?: string;
    systemId?: string;
  };
}

export interface CreateXComSystemParamDto {
  paramCode: string;
  paramName: string;
  paramValue?: string;
  paramType?: string;
  description?: string;
  status: string;
  systemId?: string;
}

export interface UpdateXComSystemParamDto extends Omit<CreateXComSystemParamDto, 'paramCode'> {}

// API 函數
export const getSystemParamList = (query: XComSystemParamQueryDto) => {
  return request.get<ApiResponse<PagedResult<XComSystemParamDto>>>('/api/v1/xcom2a0/system-params', { params: query });
};

export const getSystemParamById = (paramCode: string) => {
  return request.get<ApiResponse<XComSystemParamDto>>(`/api/v1/xcom2a0/system-params/${paramCode}`);
};

export const createSystemParam = (data: CreateXComSystemParamDto) => {
  return request.post<ApiResponse<string>>('/api/v1/xcom2a0/system-params', data);
};

export const updateSystemParam = (paramCode: string, data: UpdateXComSystemParamDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom2a0/system-params/${paramCode}`, data);
};

export const deleteSystemParam = (paramCode: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom2a0/system-params/${paramCode}`);
};

export const updateStatus = (paramCode: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/xcom2a0/system-params/${paramCode}/status`, { status });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
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
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 參數代碼必須唯一
- 必須實作權限檢查
- 敏感參數必須加密儲存
- 必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制（常用參數）

### 6.3 資料驗證
- 參數代碼必須符合命名規範
- 參數值必須根據參數類型進行驗證
- 必填欄位必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除參數前必須檢查是否有相關資料引用
- 停用參數時必須檢查是否有系統正在使用
- 參數值變更必須記錄變更歷史
- 系統參數變更必須通知相關模組

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增參數成功
- [ ] 新增參數失敗 (重複代碼)
- [ ] 修改參數成功
- [ ] 修改參數失敗 (不存在)
- [ ] 刪除參數成功
- [ ] 查詢參數列表成功
- [ ] 查詢單筆參數成功
- [ ] 參數值類型驗證

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FI.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FU.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FD.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_FQ.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0_PR.asp`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/XCOM000/XCOM2A0.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

