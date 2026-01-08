# XCOM911 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM911
- **功能名稱**: 通訊資料維護
- **功能描述**: 提供通訊資料的新增、修改、刪除、查詢、瀏覽、報表功能，包含通訊資料代碼、資料名稱、資料值、備註等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM911_PR.ASP` (報表)

### 1.2 業務需求
- 管理通訊資料
- 支援資料的新增、修改、刪除、查詢、瀏覽
- 支援資料排序功能
- 支援報表列印功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XCom911Data` (XCOM911通訊資料，對應舊系統相關資料表)

```sql
CREATE TABLE [dbo].[XCom911Data] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [DataId] NVARCHAR(50) NOT NULL, -- 資料代碼
    [DataName] NVARCHAR(200) NULL, -- 資料名稱
    [DataValue] NVARCHAR(500) NULL, -- 資料值
    [DataNote] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_XCom911Data] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_XCom911Data_DataId] UNIQUE ([DataId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XCom911Data_DataId] ON [dbo].[XCom911Data] ([DataId]);
CREATE NONCLUSTERED INDEX [IX_XCom911Data_DataName] ON [dbo].[XCom911Data] ([DataName]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| DataId | NVARCHAR | 50 | NO | - | 資料代碼 | 唯一，主鍵候選 |
| DataName | NVARCHAR | 200 | YES | - | 資料名稱 | - |
| DataValue | NVARCHAR | 500 | YES | - | 資料值 | - |
| DataNote | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢XCOM911通訊資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom911/data`
- **說明**: 查詢XCOM911通訊資料列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "DataId",
    "sortOrder": "ASC",
    "filters": {
      "dataId": "",
      "dataName": "",
      "dataValue": ""
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
          "dataId": "DATA001",
          "dataName": "資料名稱",
          "dataValue": "資料值",
          "dataNote": "備註",
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

#### 3.1.2 查詢單筆XCOM911通訊資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom911/data/{dataId}`
- **說明**: 根據資料代碼查詢單筆XCOM911通訊資料
- **路徑參數**:
  - `dataId`: 資料代碼
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增XCOM911通訊資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom911/data`
- **說明**: 新增XCOM911通訊資料
- **請求格式**:
  ```json
  {
    "dataId": "DATA001",
    "dataName": "資料名稱",
    "dataValue": "資料值",
    "dataNote": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "dataId": "DATA001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改XCOM911通訊資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom911/data/{dataId}`
- **說明**: 修改XCOM911通訊資料
- **路徑參數**:
  - `dataId`: 資料代碼
- **請求格式**: 同新增，但 `dataId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除XCOM911通訊資料
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom911/data/{dataId}`
- **說明**: 刪除XCOM911通訊資料
- **路徑參數**:
  - `dataId`: 資料代碼
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

#### 3.2.1 Controller: `XCom911DataController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom911/data")]
    [Authorize]
    public class XCom911DataController : ControllerBase
    {
        private readonly IXCom911DataService _dataService;
        
        public XCom911DataController(IXCom911DataService dataService)
        {
            _dataService = dataService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<XCom911DataDto>>>> GetDataList([FromQuery] XCom911DataQueryDto query)
        {
            var result = await _dataService.GetDataListAsync(query);
            return Ok(ApiResponse<PagedResult<XCom911DataDto>>.Success(result));
        }
        
        [HttpGet("{dataId}")]
        public async Task<ActionResult<ApiResponse<XCom911DataDto>>> GetData(string dataId)
        {
            var result = await _dataService.GetDataByIdAsync(dataId);
            return Ok(ApiResponse<XCom911DataDto>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateData([FromBody] CreateXCom911DataDto dto)
        {
            var dataId = await _dataService.CreateDataAsync(dto);
            return Ok(ApiResponse<string>.Success(dataId));
        }
        
        [HttpPut("{dataId}")]
        public async Task<ActionResult<ApiResponse>> UpdateData(string dataId, [FromBody] UpdateXCom911DataDto dto)
        {
            await _dataService.UpdateDataAsync(dataId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{dataId}")]
        public async Task<ActionResult<ApiResponse>> DeleteData(string dataId)
        {
            await _dataService.DeleteDataAsync(dataId);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `XCom911DataService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom911DataService
    {
        Task<PagedResult<XCom911DataDto>> GetDataListAsync(XCom911DataQueryDto query);
        Task<XCom911DataDto> GetDataByIdAsync(string dataId);
        Task<string> CreateDataAsync(CreateXCom911DataDto dto);
        Task UpdateDataAsync(string dataId, UpdateXCom911DataDto dto);
        Task DeleteDataAsync(string dataId);
    }
}
```

#### 3.2.3 Repository: `XCom911DataRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCom911DataRepository
    {
        Task<XCom911Data> GetByIdAsync(string dataId);
        Task<PagedResult<XCom911Data>> GetPagedAsync(XCom911DataQuery query);
        Task<XCom911Data> CreateAsync(XCom911Data data);
        Task<XCom911Data> UpdateAsync(XCom911Data data);
        Task DeleteAsync(string dataId);
        Task<bool> ExistsAsync(string dataId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 XCOM911通訊資料列表頁面 (`XCom911DataList.vue`)
- **路徑**: `/xcom/xcom911/data`
- **功能**: 顯示XCOM911通訊資料列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (XCom911DataSearchForm)
  - 資料表格 (XCom911DataDataTable)
  - 新增/修改對話框 (XCom911DataDialog)
  - 刪除確認對話框

#### 4.1.2 XCOM911通訊資料詳細頁面 (`XCom911DataDetail.vue`)
- **路徑**: `/xcom/xcom911/data/:dataId`
- **功能**: 顯示XCOM911通訊資料詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`XCom911DataSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="資料代碼">
      <el-input v-model="searchForm.dataId" placeholder="請輸入資料代碼" clearable />
    </el-form-item>
    <el-form-item label="資料名稱">
      <el-input v-model="searchForm.dataName" placeholder="請輸入資料名稱" clearable />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`XCom911DataDataTable.vue`)
```vue
<template>
  <div>
    <div style="margin-bottom: 10px">
      <el-button type="primary" @click="handleCreate">新增</el-button>
    </div>
    <el-table :data="dataList" v-loading="loading">
      <el-table-column prop="dataId" label="資料代碼" width="150" sortable />
      <el-table-column prop="dataName" label="資料名稱" width="200" sortable />
      <el-table-column prop="dataValue" label="資料值" min-width="200" />
      <el-table-column prop="dataNote" label="備註" min-width="200" />
      <el-table-column prop="createdBy" label="建立者" width="100" />
      <el-table-column prop="createdAt" label="建立時間" width="160" />
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

#### 4.2.3 新增/修改對話框 (`XCom911DataDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="700px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="資料代碼" prop="dataId">
        <el-input v-model="form.dataId" :disabled="isEdit" placeholder="請輸入資料代碼" />
      </el-form-item>
      <el-form-item label="資料名稱" prop="dataName">
        <el-input v-model="form.dataName" placeholder="請輸入資料名稱" />
      </el-form-item>
      <el-form-item label="資料值" prop="dataValue">
        <el-input v-model="form.dataValue" type="textarea" :rows="3" placeholder="請輸入資料值" />
      </el-form-item>
      <el-form-item label="備註" prop="dataNote">
        <el-input v-model="form.dataNote" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom911.api.ts`)
```typescript
import request from '@/utils/request';

export interface XCom911DataDto {
  tKey: number;
  dataId: string;
  dataName?: string;
  dataValue?: string;
  dataNote?: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface XCom911DataQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    dataId?: string;
    dataName?: string;
    dataValue?: string;
  };
}

export interface CreateXCom911DataDto {
  dataId: string;
  dataName?: string;
  dataValue?: string;
  dataNote?: string;
}

export interface UpdateXCom911DataDto {
  dataName?: string;
  dataValue?: string;
  dataNote?: string;
}

// API 函數
export const getXCom911DataList = (query: XCom911DataQueryDto) => {
  return request.get<ApiResponse<PagedResult<XCom911DataDto>>>('/api/v1/xcom911/data', { params: query });
};

export const getXCom911DataById = (dataId: string) => {
  return request.get<ApiResponse<XCom911DataDto>>(`/api/v1/xcom911/data/${dataId}`);
};

export const createXCom911Data = (data: CreateXCom911DataDto) => {
  return request.post<ApiResponse<string>>('/api/v1/xcom911/data', data);
};

export const updateXCom911Data = (dataId: string, data: UpdateXCom911DataDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom911/data/${dataId}`, data);
};

export const deleteXCom911Data = (dataId: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom911/data/${dataId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
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

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 資料代碼必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 資料代碼不可修改

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增XCOM911通訊資料成功
- [ ] 新增XCOM911通訊資料失敗 (重複代碼)
- [ ] 修改XCOM911通訊資料成功
- [ ] 修改XCOM911通訊資料失敗 (不存在)
- [ ] 刪除XCOM911通訊資料成功
- [ ] 查詢XCOM911通訊資料列表成功
- [ ] 查詢單筆XCOM911通訊資料成功

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
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FI.ASP` - 新增畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FD.ASP` - 刪除畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_FS.ASP` - 保存畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM911_PR.ASP` - 報表畫面

### 8.2 資料庫 Schema
- 舊系統資料表：需參考舊系統實際資料表結構
- 主要欄位：T_KEY, DATA_ID, DATA_NAME, DATA_VALUE, DATA_NOTE, BUSER, BTIME, CUSER, CTIME, CPRIORITY, CGROUP

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

