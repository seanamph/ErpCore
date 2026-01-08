# XCOMA01 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOMA01
- **功能名稱**: 通訊資料維護
- **功能描述**: 提供通訊資料的瀏覽、查詢功能，包含通訊資料代碼、資料名稱、資料值、備註等資訊查詢
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOMA01_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOMA01_FQ.ASP` (查詢)

### 1.2 業務需求
- 瀏覽通訊資料
- 查詢通訊資料
- 支援多條件查詢
- 支援資料排序功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XComA01Data` (XCOMA01通訊資料，對應舊系統相關資料表)

```sql
CREATE TABLE [dbo].[XComA01Data] (
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
    CONSTRAINT [PK_XComA01Data] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_XComA01Data_DataId] UNIQUE ([DataId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XComA01Data_DataId] ON [dbo].[XComA01Data] ([DataId]);
CREATE NONCLUSTERED INDEX [IX_XComA01Data_DataName] ON [dbo].[XComA01Data] ([DataName]);
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

#### 3.1.1 查詢XCOMA01通訊資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcoma01/data`
- **說明**: 查詢XCOMA01通訊資料列表，支援分頁、排序、篩選
- **請求參數**: 同XCOM924
- **回應格式**: 同XCOM924

#### 3.1.2 查詢單筆XCOMA01通訊資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcoma01/data/{dataId}`
- **說明**: 根據資料代碼查詢單筆XCOMA01通訊資料
- **路徑參數**:
  - `dataId`: 資料代碼
- **回應格式**: 同XCOM924

### 3.2 後端實作類別

#### 3.2.1 Controller: `XComA01DataController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcoma01/data")]
    [Authorize]
    public class XComA01DataController : ControllerBase
    {
        private readonly IXComA01DataService _dataService;
        
        public XComA01DataController(IXComA01DataService dataService)
        {
            _dataService = dataService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<XComA01DataDto>>>> GetDataList([FromQuery] XComA01DataQueryDto query)
        {
            var result = await _dataService.GetDataListAsync(query);
            return Ok(ApiResponse<PagedResult<XComA01DataDto>>.Success(result));
        }
        
        [HttpGet("{dataId}")]
        public async Task<ActionResult<ApiResponse<XComA01DataDto>>> GetData(string dataId)
        {
            var result = await _dataService.GetDataByIdAsync(dataId);
            return Ok(ApiResponse<XComA01DataDto>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `XComA01DataService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXComA01DataService
    {
        Task<PagedResult<XComA01DataDto>> GetDataListAsync(XComA01DataQueryDto query);
        Task<XComA01DataDto> GetDataByIdAsync(string dataId);
    }
}
```

#### 3.2.3 Repository: `XComA01DataRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXComA01DataRepository
    {
        Task<XComA01Data> GetByIdAsync(string dataId);
        Task<PagedResult<XComA01Data>> GetPagedAsync(XComA01DataQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 XCOMA01通訊資料列表頁面 (`XComA01DataList.vue`)
- **路徑**: `/xcom/xcoma01/data`
- **功能**: 顯示XCOMA01通訊資料列表，支援查詢、瀏覽
- **主要元件**:
  - 查詢表單 (XComA01DataSearchForm)
  - 資料表格 (XComA01DataDataTable)

#### 4.1.2 XCOMA01通訊資料詳細頁面 (`XComA01DataDetail.vue`)
- **路徑**: `/xcom/xcoma01/data/:dataId`
- **功能**: 顯示XCOMA01通訊資料詳細資料

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`XComA01DataSearchForm.vue`)
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

#### 4.2.2 資料表格元件 (`XComA01DataDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="dataList" v-loading="loading">
      <el-table-column prop="dataId" label="資料代碼" width="150" sortable />
      <el-table-column prop="dataName" label="資料名稱" width="200" sortable />
      <el-table-column prop="dataValue" label="資料值" min-width="200" />
      <el-table-column prop="dataNote" label="備註" min-width="200" />
      <el-table-column prop="createdBy" label="建立者" width="100" />
      <el-table-column prop="createdAt" label="建立時間" width="160" />
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">瀏覽</el-button>
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

### 4.3 API 呼叫 (`xcoma01.api.ts`)
```typescript
import request from '@/utils/request';

export interface XComA01DataDto {
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

export interface XComA01DataQueryDto {
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

// API 函數
export const getXComA01DataList = (query: XComA01DataQueryDto) => {
  return request.get<ApiResponse<PagedResult<XComA01DataDto>>>('/api/v1/xcoma01/data', { params: query });
};

export const getXComA01DataById = (dataId: string) => {
  return request.get<ApiResponse<XComA01DataDto>>(`/api/v1/xcoma01/data/${dataId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 6天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 查詢條件必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢XCOMA01通訊資料列表成功
- [ ] 查詢單筆XCOMA01通訊資料成功
- [ ] 查詢條件驗證測試

### 7.2 整合測試
- [ ] 查詢流程測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發查詢測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOMA01_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOMA01_FQ.ASP` - 查詢畫面

### 8.2 資料庫 Schema
- 舊系統資料表：需參考舊系統實際資料表結構
- 主要欄位：T_KEY, DATA_ID, DATA_NAME, DATA_VALUE, DATA_NOTE, BUSER, BTIME, CUSER, CTIME, CPRIORITY, CGROUP

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

