# XCOM910 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM910
- **功能名稱**: 通訊資料維護（XCOM部門測試）
- **功能描述**: 提供XCOM部門測試資料的新增、修改、刪除、查詢、瀏覽、報表功能，包含部門代碼、部門名稱、備註等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FMI.ASP` (批量新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM910_PR.ASP` (報表)

### 1.2 業務需求
- 管理XCOM部門測試資料
- 支援部門代碼維護（長度8碼）
- 支援部門名稱維護（長度20碼）
- 支援備註維護（長度100碼）
- 支援資料的新增、修改、刪除、查詢、瀏覽
- 支援批量新增功能
- 支援資料排序功能
- 支援報表列印功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XComDeptTest` (XCOM部門測試，對應舊系統 `XCOM_DEPT_TEST`)

```sql
CREATE TABLE [dbo].[XComDeptTest] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [DeptId] NVARCHAR(8) NOT NULL, -- 部門代碼 (DEPT_ID)
    [DeptName] NVARCHAR(20) NULL, -- 部門名稱 (DEPT_NAME)
    [DeptNote] NVARCHAR(100) NULL, -- 備註 (DEPT_NOTE)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_XComDeptTest] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_XComDeptTest_DeptId] UNIQUE ([DeptId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XComDeptTest_DeptId] ON [dbo].[XComDeptTest] ([DeptId]);
CREATE NONCLUSTERED INDEX [IX_XComDeptTest_DeptName] ON [dbo].[XComDeptTest] ([DeptName]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| DeptId | NVARCHAR | 8 | NO | - | 部門代碼 | 唯一，主鍵候選 |
| DeptName | NVARCHAR | 20 | YES | - | 部門名稱 | - |
| DeptNote | NVARCHAR | 100 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢XCOM部門測試列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom910/dept-test`
- **說明**: 查詢XCOM部門測試列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "DeptId",
    "sortOrder": "ASC",
    "filters": {
      "deptId": "",
      "deptName": "",
      "deptNote": ""
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
          "deptId": "DEPT001",
          "deptName": "部門名稱",
          "deptNote": "備註",
          "createdBy": "U001",
          "createdAt": "2024-01-01T00:00:00",
          "updatedBy": "U001",
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

#### 3.1.2 查詢單筆XCOM部門測試
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom910/dept-test/{deptId}`
- **說明**: 根據部門代碼查詢單筆XCOM部門測試資料
- **路徑參數**:
  - `deptId`: 部門代碼
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增XCOM部門測試
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom910/dept-test`
- **說明**: 新增XCOM部門測試資料
- **請求格式**:
  ```json
  {
    "deptId": "DEPT001",
    "deptName": "部門名稱",
    "deptNote": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "deptId": "DEPT001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 批量新增XCOM部門測試
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom910/dept-test/batch`
- **說明**: 批量新增多筆XCOM部門測試資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "deptId": "DEPT001",
        "deptName": "部門名稱1",
        "deptNote": "備註1"
      },
      {
        "deptId": "DEPT002",
        "deptName": "部門名稱2",
        "deptNote": "備註2"
      }
    ]
  }
  ```

#### 3.1.5 修改XCOM部門測試
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom910/dept-test/{deptId}`
- **說明**: 修改XCOM部門測試資料
- **路徑參數**:
  - `deptId`: 部門代碼
- **請求格式**: 同新增，但 `deptId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除XCOM部門測試
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom910/dept-test/{deptId}`
- **說明**: 刪除XCOM部門測試資料
- **路徑參數**:
  - `deptId`: 部門代碼
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

#### 3.1.7 批次刪除XCOM部門測試
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom910/dept-test/batch`
- **說明**: 批次刪除多筆XCOM部門測試
- **請求格式**:
  ```json
  {
    "deptIds": ["DEPT001", "DEPT002", "DEPT003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom910DeptTestController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom910/dept-test")]
    [Authorize]
    public class XCom910DeptTestController : ControllerBase
    {
        private readonly IXCom910DeptTestService _deptTestService;
        
        public XCom910DeptTestController(IXCom910DeptTestService deptTestService)
        {
            _deptTestService = deptTestService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<XCom910DeptTestDto>>>> GetDeptTests([FromQuery] XCom910DeptTestQueryDto query)
        {
            var result = await _deptTestService.GetDeptTestsAsync(query);
            return Ok(ApiResponse<PagedResult<XCom910DeptTestDto>>.Success(result));
        }
        
        [HttpGet("{deptId}")]
        public async Task<ActionResult<ApiResponse<XCom910DeptTestDto>>> GetDeptTest(string deptId)
        {
            var result = await _deptTestService.GetDeptTestByIdAsync(deptId);
            return Ok(ApiResponse<XCom910DeptTestDto>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateDeptTest([FromBody] CreateXCom910DeptTestDto dto)
        {
            var deptId = await _deptTestService.CreateDeptTestAsync(dto);
            return Ok(ApiResponse<string>.Success(deptId));
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse>> BatchCreateDeptTests([FromBody] BatchCreateXCom910DeptTestDto dto)
        {
            await _deptTestService.BatchCreateDeptTestsAsync(dto.Items);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("{deptId}")]
        public async Task<ActionResult<ApiResponse>> UpdateDeptTest(string deptId, [FromBody] UpdateXCom910DeptTestDto dto)
        {
            await _deptTestService.UpdateDeptTestAsync(deptId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{deptId}")]
        public async Task<ActionResult<ApiResponse>> DeleteDeptTest(string deptId)
        {
            await _deptTestService.DeleteDeptTestAsync(deptId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> BatchDeleteDeptTests([FromBody] BatchDeleteXCom910DeptTestRequestDto request)
        {
            await _deptTestService.BatchDeleteDeptTestsAsync(request.DeptIds);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `XCom910DeptTestService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom910DeptTestService
    {
        Task<PagedResult<XCom910DeptTestDto>> GetDeptTestsAsync(XCom910DeptTestQueryDto query);
        Task<XCom910DeptTestDto> GetDeptTestByIdAsync(string deptId);
        Task<string> CreateDeptTestAsync(CreateXCom910DeptTestDto dto);
        Task BatchCreateDeptTestsAsync(List<CreateXCom910DeptTestDto> items);
        Task UpdateDeptTestAsync(string deptId, UpdateXCom910DeptTestDto dto);
        Task DeleteDeptTestAsync(string deptId);
        Task BatchDeleteDeptTestsAsync(List<string> deptIds);
    }
}
```

#### 3.2.3 Repository: `XCom910DeptTestRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCom910DeptTestRepository
    {
        Task<XCom910DeptTest> GetByIdAsync(string deptId);
        Task<PagedResult<XCom910DeptTest>> GetPagedAsync(XCom910DeptTestQuery query);
        Task<XCom910DeptTest> CreateAsync(XCom910DeptTest deptTest);
        Task<XCom910DeptTest> UpdateAsync(XCom910DeptTest deptTest);
        Task DeleteAsync(string deptId);
        Task<bool> ExistsAsync(string deptId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 XCOM部門測試列表頁面 (`XCom910DeptTestList.vue`)
- **路徑**: `/xcom/dept-test`
- **功能**: 顯示XCOM部門測試列表，支援查詢、新增、修改、刪除、批量新增
- **主要元件**:
  - 查詢表單 (XCom910DeptTestSearchForm)
  - 資料表格 (XCom910DeptTestDataTable)
  - 新增/修改對話框 (XCom910DeptTestDialog)
  - 批量新增對話框 (XCom910DeptTestBatchDialog)
  - 刪除確認對話框

#### 4.1.2 XCOM部門測試詳細頁面 (`XCom910DeptTestDetail.vue`)
- **路徑**: `/xcom/dept-test/:deptId`
- **功能**: 顯示XCOM部門測試詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`XCom910DeptTestSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="部門代碼">
      <el-input v-model="searchForm.deptId" placeholder="請輸入部門代碼" clearable maxlength="8" />
    </el-form-item>
    <el-form-item label="部門名稱">
      <el-input v-model="searchForm.deptName" placeholder="請輸入部門名稱" clearable maxlength="20" />
    </el-form-item>
    <el-form-item label="備註">
      <el-input v-model="searchForm.deptNote" placeholder="請輸入備註" clearable maxlength="100" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`XCom910DeptTestDataTable.vue`)
```vue
<template>
  <div>
    <div style="margin-bottom: 10px">
      <el-button type="primary" @click="handleCreate">新增</el-button>
      <el-button type="success" @click="handleBatchCreate">批量新增</el-button>
      <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">批次刪除</el-button>
    </div>
    <el-table :data="deptTestList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="deptId" label="部門代碼" width="120" sortable />
      <el-table-column prop="deptName" label="部門名稱" width="200" sortable />
      <el-table-column prop="deptNote" label="備註" min-width="200" />
      <el-table-column prop="createdBy" label="建立者" width="100" />
      <el-table-column prop="createdAt" label="建立時間" width="160" />
      <el-table-column prop="updatedBy" label="更新者" width="100" />
      <el-table-column prop="updatedAt" label="更新時間" width="160" />
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

#### 4.2.3 新增/修改對話框 (`XCom910DeptTestDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="部門代碼" prop="deptId">
        <el-input v-model="form.deptId" :disabled="isEdit" placeholder="請輸入部門代碼（8碼）" maxlength="8" />
      </el-form-item>
      <el-form-item label="部門名稱" prop="deptName">
        <el-input v-model="form.deptName" placeholder="請輸入部門名稱（20碼）" maxlength="20" />
      </el-form-item>
      <el-form-item label="備註" prop="deptNote">
        <el-input v-model="form.deptNote" type="textarea" :rows="3" placeholder="請輸入備註（100碼）" maxlength="100" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

#### 4.2.4 批量新增對話框 (`XCom910DeptTestBatchDialog.vue`)
```vue
<template>
  <el-dialog
    title="批量新增XCOM部門測試"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" ref="formRef" label-width="120px">
      <el-form-item label="批量資料">
        <el-input
          v-model="batchData"
          type="textarea"
          :rows="10"
          placeholder="請輸入批量資料，每行一筆，格式：部門代碼,部門名稱,備註"
        />
        <div style="margin-top: 10px; color: #909399; font-size: 12px">
          範例：<br>
          DEPT001,部門名稱1,備註1<br>
          DEPT002,部門名稱2,備註2
        </div>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom910.api.ts`)
```typescript
import request from '@/utils/request';

export interface XCom910DeptTestDto {
  tKey: number;
  deptId: string;
  deptName?: string;
  deptNote?: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface XCom910DeptTestQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    deptId?: string;
    deptName?: string;
    deptNote?: string;
  };
}

export interface CreateXCom910DeptTestDto {
  deptId: string;
  deptName?: string;
  deptNote?: string;
}

export interface UpdateXCom910DeptTestDto {
  deptName?: string;
  deptNote?: string;
}

// API 函數
export const getXCom910DeptTestList = (query: XCom910DeptTestQueryDto) => {
  return request.get<ApiResponse<PagedResult<XCom910DeptTestDto>>>('/api/v1/xcom910/dept-test', { params: query });
};

export const getXCom910DeptTestById = (deptId: string) => {
  return request.get<ApiResponse<XCom910DeptTestDto>>(`/api/v1/xcom910/dept-test/${deptId}`);
};

export const createXCom910DeptTest = (data: CreateXCom910DeptTestDto) => {
  return request.post<ApiResponse<string>>('/api/v1/xcom910/dept-test', data);
};

export const batchCreateXCom910DeptTests = (items: CreateXCom910DeptTestDto[]) => {
  return request.post<ApiResponse>('/api/v1/xcom910/dept-test/batch', { items });
};

export const updateXCom910DeptTest = (deptId: string, data: UpdateXCom910DeptTestDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom910/dept-test/${deptId}`, data);
};

export const deleteXCom910DeptTest = (deptId: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom910/dept-test/${deptId}`);
};

export const batchDeleteXCom910DeptTests = (deptIds: string[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom910/dept-test/batch', { data: { deptIds } });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3.5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 批量新增邏輯實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3.5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 批量新增對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 批量操作測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 批量新增必須使用事務處理

### 6.3 資料驗證
- 部門代碼必須唯一
- 部門代碼長度必須為8碼
- 部門名稱長度必須為20碼以內
- 備註長度必須為100碼以內
- 必填欄位必須驗證

### 6.4 業務邏輯
- 部門代碼不可修改
- 批量新增必須處理重複資料
- 批量新增必須處理錯誤資料

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增XCOM部門測試成功
- [ ] 新增XCOM部門測試失敗 (重複代碼)
- [ ] 批量新增XCOM部門測試成功
- [ ] 批量新增XCOM部門測試失敗 (部分重複)
- [ ] 修改XCOM部門測試成功
- [ ] 修改XCOM部門測試失敗 (不存在)
- [ ] 刪除XCOM部門測試成功
- [ ] 批次刪除XCOM部門測試成功
- [ ] 查詢XCOM部門測試列表成功
- [ ] 查詢單筆XCOM部門測試成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 批量操作測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 批量新增效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FI.ASP` - 新增畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FD.ASP` - 刪除畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FS.ASP` - 保存畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_FMI.ASP` - 批量新增畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM910_PR.ASP` - 報表畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`XCOM_DEPT_TEST`
- 主要欄位：T_KEY, DEPT_ID, DEPT_NAME, DEPT_NOTE, BUSER, BTIME, CUSER, CTIME, CPRIORITY, CGROUP

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

