# XCOM130 - 通訊資料維護(函式庫參考手冊) 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM130
- **功能名稱**: 通訊資料維護(函式庫參考手冊)
- **功能描述**: 提供線上程式參考手冊的新增、修改、刪除、查詢功能，包含函式名稱、出處、類別、說明、撰寫者、參考等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FI2.asp` (新增處理)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FU2.asp` (修改處理)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FR.asp` (詳細)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM130.ini` (設定檔)

### 1.2 業務需求
- 管理系統函式庫參考手冊
- 支援函式名稱、出處、類別、說明、撰寫者、參考等資訊維護
- 支援函式參數資訊維護（最多5個參數）
- 支援函式返回值型態維護
- 支援函式範例維護
- 支援函式查詢與瀏覽
- 支援函式參考連結維護

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ETEK_REFM` (函式參考主檔)

```sql
CREATE TABLE [dbo].[ETEK_REFM] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ORI_OBJECT] NVARCHAR(200) NULL, -- 出處
    [FUNCTION_NAME] NVARCHAR(200) NOT NULL, -- 函式名稱
    [FUNCTION_TYPE] INT NOT NULL DEFAULT 0, -- 函式類別 (0:ASP, 1:PHP, 2:DELPHI, 3:JavaScript, 4:VBScript, 5:Java, 6:C, 7:C++, 8:Perl, 9:Stored Procedure)
    [REMARK] NVARCHAR(MAX) NULL, -- 說明
    [EXAMPLE] NVARCHAR(MAX) NULL, -- 範例
    [SEE_ALSO] NVARCHAR(500) NULL, -- 參考（以逗點分隔）
    [PROGRAMMER] NVARCHAR(100) NULL, -- 撰寫者
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_ETEK_REFM_FUNCTION_NAME] UNIQUE ([FUNCTION_NAME])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ETEK_REFM_FUNCTION_NAME] ON [dbo].[ETEK_REFM] ([FUNCTION_NAME]);
CREATE NONCLUSTERED INDEX [IX_ETEK_REFM_FUNCTION_TYPE] ON [dbo].[ETEK_REFM] ([FUNCTION_TYPE]);
CREATE NONCLUSTERED INDEX [IX_ETEK_REFM_ORI_OBJECT] ON [dbo].[ETEK_REFM] ([ORI_OBJECT]);
```

### 2.2 明細資料表: `ETEK_REFD` (函式參考明細)

```sql
CREATE TABLE [dbo].[ETEK_REFD] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [REFM_T_KEY] BIGINT NOT NULL, -- 主檔T_KEY
    [PARAM_TYPE] INT NOT NULL, -- 參數類型 (1:輸入參數, 2:返回值)
    [VAR_NAME] NVARCHAR(200) NULL, -- 參數名稱
    [TYPE] INT NOT NULL DEFAULT 0, -- 型態 (0:無, 1:字串, 2:數值, 3:日期, 4:布林, 5:物件, 6:陣列, 7:集合, 8:其他, 9:無限制)
    [REMARK] NVARCHAR(500) NULL, -- 說明
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ETEK_REFD_REFM] FOREIGN KEY ([REFM_T_KEY]) REFERENCES [dbo].[ETEK_REFM] ([T_KEY]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ETEK_REFD_REFM_T_KEY] ON [dbo].[ETEK_REFD] ([REFM_T_KEY]);
CREATE NONCLUSTERED INDEX [IX_ETEK_REFD_PARAM_TYPE] ON [dbo].[ETEK_REFD] ([PARAM_TYPE]);
```

### 2.3 資料字典

#### 2.3.1 ETEK_REFM 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ORI_OBJECT | NVARCHAR | 200 | YES | - | 出處 | - |
| FUNCTION_NAME | NVARCHAR | 200 | NO | - | 函式名稱 | 唯一，必填 |
| FUNCTION_TYPE | INT | - | NO | 0 | 函式類別 | 0:ASP, 1:PHP, 2:DELPHI, 3:JavaScript, 4:VBScript, 5:Java, 6:C, 7:C++, 8:Perl, 9:Stored Procedure |
| REMARK | NVARCHAR(MAX) | - | YES | - | 說明 | - |
| EXAMPLE | NVARCHAR(MAX) | - | YES | - | 範例 | - |
| SEE_ALSO | NVARCHAR | 500 | YES | - | 參考 | 以逗點分隔 |
| PROGRAMMER | NVARCHAR | 100 | YES | - | 撰寫者 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

#### 2.3.2 ETEK_REFD 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| REFM_T_KEY | BIGINT | - | NO | - | 主檔T_KEY | 外鍵至ETEK_REFM |
| PARAM_TYPE | INT | - | NO | - | 參數類型 | 1:輸入參數, 2:返回值 |
| VAR_NAME | NVARCHAR | 200 | YES | - | 參數名稱 | - |
| TYPE | INT | - | NO | 0 | 型態 | 0:無, 1:字串, 2:數值, 3:日期, 4:布林, 5:物件, 6:陣列, 7:集合, 8:其他, 9:無限制 |
| REMARK | NVARCHAR | 500 | YES | - | 說明 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢函式參考列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom130/functions`
- **說明**: 查詢函式參考列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "FUNCTION_NAME",
    "sortOrder": "ASC",
    "filters": {
      "functionName": "",
      "functionType": "",
      "oriObject": "",
      "programmer": ""
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
          "tKey": 1,
          "oriObject": "util.asp",
          "functionName": "GetDateString",
          "functionType": 0,
          "functionTypeName": "ASP",
          "remark": "取得日期字串",
          "programmer": "Robinson Jeng",
          "seeAlso": "GetTimeString,GetDateTimeString"
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

#### 3.1.2 查詢單筆函式參考
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom130/functions/{tKey}`
- **說明**: 根據T_KEY查詢單筆函式參考資料（包含參數和返回值）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "oriObject": "util.asp",
      "functionName": "GetDateString",
      "functionType": 0,
      "functionTypeName": "ASP",
      "remark": "取得日期字串",
      "example": "GetDateString('yyyy/mm/dd')",
      "seeAlso": "GetTimeString,GetDateTimeString",
      "programmer": "Robinson Jeng",
      "returnValue": {
        "type": 1,
        "typeName": "字串",
        "remark": "日期字串"
      },
      "parameters": [
        {
          "varName": "format",
          "type": 1,
          "typeName": "字串",
          "remark": "日期格式"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增函式參考
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom130/functions`
- **說明**: 新增函式參考資料
- **請求格式**:
  ```json
  {
    "oriObject": "util.asp",
    "functionName": "GetDateString",
    "functionType": 0,
    "remark": "取得日期字串",
    "example": "GetDateString('yyyy/mm/dd')",
    "seeAlso": "GetTimeString,GetDateTimeString",
    "programmer": "Robinson Jeng",
    "returnValue": {
      "type": 1,
      "remark": "日期字串"
    },
    "parameters": [
      {
        "varName": "format",
        "type": 1,
        "remark": "日期格式"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改函式參考
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom130/functions/{tKey}`
- **說明**: 修改函式參考資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `functionName` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除函式參考
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom130/functions/{tKey}`
- **說明**: 刪除函式參考資料（會同時刪除相關的參數和返回值資料）
- **路徑參數**:
  - `tKey`: 主鍵
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

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCOM130Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom130")]
    [Authorize]
    public class XCOM130Controller : ControllerBase
    {
        private readonly IXCOM130Service _service;
        
        public XCOM130Controller(IXCOM130Service service)
        {
            _service = service;
        }
        
        [HttpGet("functions")]
        public async Task<ActionResult<ApiResponse<PagedResult<FunctionRefDto>>>> GetFunctions([FromQuery] FunctionRefQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("functions/{tKey}")]
        public async Task<ActionResult<ApiResponse<FunctionRefDetailDto>>> GetFunction(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("functions")]
        public async Task<ActionResult<ApiResponse<long>>> CreateFunction([FromBody] CreateFunctionRefDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("functions/{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateFunction(long tKey, [FromBody] UpdateFunctionRefDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("functions/{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteFunction(long tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `XCOM130Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCOM130Service
    {
        Task<PagedResult<FunctionRefDto>> GetFunctionsAsync(FunctionRefQueryDto query);
        Task<FunctionRefDetailDto> GetFunctionByIdAsync(long tKey);
        Task<long> CreateFunctionAsync(CreateFunctionRefDto dto);
        Task UpdateFunctionAsync(long tKey, UpdateFunctionRefDto dto);
        Task DeleteFunctionAsync(long tKey);
    }
}
```

#### 3.2.3 Repository: `XCOM130Repository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCOM130Repository
    {
        Task<FunctionRef> GetByIdAsync(long tKey);
        Task<PagedResult<FunctionRef>> GetPagedAsync(FunctionRefQuery query);
        Task<FunctionRef> CreateAsync(FunctionRef functionRef);
        Task<FunctionRef> UpdateAsync(FunctionRef functionRef);
        Task DeleteAsync(long tKey);
        Task<bool> ExistsByFunctionNameAsync(string functionName);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 函式參考列表頁面 (`FunctionRefList.vue`)
- **路徑**: `/xcom130/functions`
- **功能**: 顯示函式參考列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (FunctionRefSearchForm)
  - 資料表格 (FunctionRefDataTable)
  - 新增/修改對話框 (FunctionRefDialog)
  - 詳細對話框 (FunctionRefDetailDialog)
  - 刪除確認對話框

#### 4.1.2 函式參考詳細頁面 (`FunctionRefDetail.vue`)
- **路徑**: `/xcom130/functions/:tKey`
- **功能**: 顯示函式參考詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`FunctionRefSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="函式名稱">
      <el-input v-model="searchForm.functionName" placeholder="請輸入函式名稱" />
    </el-form-item>
    <el-form-item label="函式類別">
      <el-select v-model="searchForm.functionType" placeholder="請選擇函式類別">
        <el-option label="全部" value="" />
        <el-option label="ASP" value="0" />
        <el-option label="PHP" value="1" />
        <el-option label="DELPHI" value="2" />
        <el-option label="JavaScript" value="3" />
        <el-option label="VBScript" value="4" />
        <el-option label="Java" value="5" />
        <el-option label="C" value="6" />
        <el-option label="C++" value="7" />
        <el-option label="Perl" value="8" />
        <el-option label="Stored Procedure" value="9" />
      </el-select>
    </el-form-item>
    <el-form-item label="出處">
      <el-input v-model="searchForm.oriObject" placeholder="請輸入出處" />
    </el-form-item>
    <el-form-item label="撰寫者">
      <el-input v-model="searchForm.programmer" placeholder="請輸入撰寫者" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`FunctionRefDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="functionList" v-loading="loading">
      <el-table-column prop="functionName" label="函式名稱" width="200" />
      <el-table-column prop="oriObject" label="出處" width="150" />
      <el-table-column prop="functionTypeName" label="類別" width="120" />
      <el-table-column prop="remark" label="說明" min-width="200" show-overflow-tooltip />
      <el-table-column prop="programmer" label="撰寫者" width="120" />
      <el-table-column prop="seeAlso" label="參考" width="150" show-overflow-tooltip />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">詳細</el-button>
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

#### 4.2.3 新增/修改對話框 (`FunctionRefDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="出處" prop="oriObject">
        <el-input v-model="form.oriObject" placeholder="請輸入出處" />
      </el-form-item>
      <el-form-item label="函式名稱" prop="functionName">
        <el-input v-model="form.functionName" :disabled="isEdit" placeholder="請輸入函式名稱" />
      </el-form-item>
      <el-form-item label="函式類別" prop="functionType">
        <el-select v-model="form.functionType" placeholder="請選擇函式類別">
          <el-option label="ASP" value="0" />
          <el-option label="PHP" value="1" />
          <el-option label="DELPHI" value="2" />
          <el-option label="JavaScript" value="3" />
          <el-option label="VBScript" value="4" />
          <el-option label="Java" value="5" />
          <el-option label="C" value="6" />
          <el-option label="C++" value="7" />
          <el-option label="Perl" value="8" />
          <el-option label="Stored Procedure" value="9" />
        </el-select>
      </el-form-item>
      <el-form-item label="撰寫者" prop="programmer">
        <el-input v-model="form.programmer" placeholder="請輸入撰寫者" />
      </el-form-item>
      <el-form-item label="返回值型態" prop="returnValue.type">
        <el-select v-model="form.returnValue.type" placeholder="請選擇返回值型態">
          <el-option label="無" value="0" />
          <el-option label="字串" value="1" />
          <el-option label="數值" value="2" />
          <el-option label="日期" value="3" />
          <el-option label="布林" value="4" />
          <el-option label="物件" value="5" />
          <el-option label="陣列" value="6" />
          <el-option label="集合" value="7" />
          <el-option label="其他" value="8" />
          <el-option label="無限制" value="9" />
        </el-select>
        <el-input v-model="form.returnValue.remark" placeholder="返回值說明" style="width: 300px; margin-left: 10px;" />
      </el-form-item>
      <el-divider>輸入參數</el-divider>
      <div v-for="(param, index) in form.parameters" :key="index" style="margin-bottom: 10px;">
        <el-row :gutter="10">
          <el-col :span="6">
            <el-input v-model="param.varName" :placeholder="`參數${index + 1}名稱`" />
          </el-col>
          <el-col :span="6">
            <el-select v-model="param.type" placeholder="型態">
              <el-option label="無" value="0" />
              <el-option label="字串" value="1" />
              <el-option label="數值" value="2" />
              <el-option label="日期" value="3" />
              <el-option label="布林" value="4" />
              <el-option label="物件" value="5" />
              <el-option label="陣列" value="6" />
              <el-option label="集合" value="7" />
              <el-option label="其他" value="8" />
              <el-option label="無限制" value="9" />
            </el-select>
          </el-col>
          <el-col :span="10">
            <el-input v-model="param.remark" placeholder="說明" />
          </el-col>
          <el-col :span="2">
            <el-button type="danger" size="small" @click="removeParameter(index)">刪除</el-button>
          </el-col>
        </el-row>
      </div>
      <el-button type="primary" size="small" @click="addParameter">新增參數</el-button>
      <el-form-item label="說明" prop="remark">
        <el-input v-model="form.remark" type="textarea" :rows="5" placeholder="請輸入說明" />
      </el-form-item>
      <el-form-item label="範例" prop="example">
        <el-input v-model="form.example" type="textarea" :rows="5" placeholder="請輸入範例" />
      </el-form-item>
      <el-form-item label="參考" prop="seeAlso">
        <el-input v-model="form.seeAlso" placeholder="請以逗點分隔" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom130.api.ts`)
```typescript
import request from '@/utils/request';

export interface FunctionRefDto {
  tKey: number;
  oriObject?: string;
  functionName: string;
  functionType: number;
  functionTypeName?: string;
  remark?: string;
  example?: string;
  seeAlso?: string;
  programmer?: string;
}

export interface FunctionRefDetailDto extends FunctionRefDto {
  returnValue?: {
    type: number;
    typeName?: string;
    remark?: string;
  };
  parameters?: Array<{
    varName?: string;
    type: number;
    typeName?: string;
    remark?: string;
  }>;
}

export interface FunctionRefQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    functionName?: string;
    functionType?: string;
    oriObject?: string;
    programmer?: string;
  };
}

export interface CreateFunctionRefDto {
  oriObject?: string;
  functionName: string;
  functionType: number;
  remark?: string;
  example?: string;
  seeAlso?: string;
  programmer?: string;
  returnValue?: {
    type: number;
    remark?: string;
  };
  parameters?: Array<{
    varName?: string;
    type: number;
    remark?: string;
  }>;
}

export interface UpdateFunctionRefDto extends Omit<CreateFunctionRefDto, 'functionName'> {}

// API 函數
export const getFunctionList = (query: FunctionRefQueryDto) => {
  return request.get<ApiResponse<PagedResult<FunctionRefDto>>>('/api/v1/xcom130/functions', { params: query });
};

export const getFunctionById = (tKey: number) => {
  return request.get<ApiResponse<FunctionRefDetailDto>>(`/api/v1/xcom130/functions/${tKey}`);
};

export const createFunction = (data: CreateFunctionRefDto) => {
  return request.post<ApiResponse<number>>('/api/v1/xcom130/functions', data);
};

export const updateFunction = (tKey: number, data: UpdateFunctionRefDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom130/functions/${tKey}`, data);
};

export const deleteFunction = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom130/functions/${tKey}`);
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
- [ ] 詳細對話框開發
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
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 必須防止SQL注入攻擊

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 函式名稱必須唯一
- 必填欄位必須驗證
- 函式類別必須在允許範圍內
- 參數型態必須在允許範圍內

### 6.4 業務邏輯
- 刪除函式參考時必須同時刪除相關的參數和返回值資料
- 函式名稱在新增後不可修改
- 參數數量限制為最多5個

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增函式參考成功
- [ ] 新增函式參考失敗 (重複函式名稱)
- [ ] 修改函式參考成功
- [ ] 修改函式參考失敗 (不存在)
- [ ] 刪除函式參考成功
- [ ] 查詢函式參考列表成功
- [ ] 查詢單筆函式參考成功

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
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FB.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FI.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FI2.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FU.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FU2.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FQ.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130_FR.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM130.ini`

### 8.2 資料庫 Schema
- `ETEK_REFM` - 函式參考主檔
- `ETEK_REFD` - 函式參考明細

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

