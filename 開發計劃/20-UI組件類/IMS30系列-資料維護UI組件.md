# IMS30系列 - 資料維護UI組件 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS30系列
- **功能名稱**: 資料維護UI組件
- **功能描述**: 提供通用的資料維護UI組件，包含瀏覽(FB)、新增(FI)、修改(FU)、查詢(FQ)、列印(PR)、保存(FS)等功能，可被其他功能模組重用
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FB.aspx` - 瀏覽功能頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FI.aspx` - 新增功能頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FU.aspx` - 修改功能頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FQ.aspx` - 查詢功能頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_PR.aspx` - 報表功能頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FS.aspx` - 保存功能頁面
  - `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_*.aspx` - V201版本頁面
  - `IMS3/HANSHIN/IMS3/KERNEL/IMS30_DB.aspx` - 資料庫操作頁面

### 1.2 業務需求
- 提供通用的資料維護UI組件
- 支援資料瀏覽、新增、修改、刪除、查詢功能
- 支援資料列印功能
- 支援資料保存功能
- 支援資料驗證
- 支援AJAX非同步操作
- 可被其他功能模組重用
- 支援V1和V2版本

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UIComponents` (UI組件設定)

```sql
CREATE TABLE [dbo].[UIComponents] (
    [ComponentId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ComponentCode] NVARCHAR(50) NOT NULL, -- 組件代碼 (IMS30_FB, IMS30_FI等)
    [ComponentName] NVARCHAR(200) NOT NULL, -- 組件名稱
    [ComponentType] NVARCHAR(20) NOT NULL, -- 組件類型 (FB, FI, FU, FQ, PR, FS)
    [ComponentVersion] NVARCHAR(10) NOT NULL DEFAULT 'V1', -- 組件版本 (V1, V2)
    [ConfigJson] NVARCHAR(MAX) NULL, -- 組件配置 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_UIComponents] PRIMARY KEY CLUSTERED ([ComponentId] ASC),
    CONSTRAINT [UQ_UIComponents_Code_Version] UNIQUE ([ComponentCode], [ComponentVersion])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UIComponents_ComponentCode] ON [dbo].[UIComponents] ([ComponentCode]);
CREATE NONCLUSTERED INDEX [IX_UIComponents_ComponentType] ON [dbo].[UIComponents] ([ComponentType]);
CREATE NONCLUSTERED INDEX [IX_UIComponents_Status] ON [dbo].[UIComponents] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `UIComponentUsages` - UI組件使用記錄
```sql
CREATE TABLE [dbo].[UIComponentUsages] (
    [UsageId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ComponentId] BIGINT NOT NULL,
    [ModuleCode] NVARCHAR(50) NOT NULL, -- 使用模組代碼
    [ModuleName] NVARCHAR(200) NULL, -- 使用模組名稱
    [UsageCount] INT NOT NULL DEFAULT 0, -- 使用次數
    [LastUsedAt] DATETIME2 NULL, -- 最後使用時間
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UIComponentUsages_UIComponents] FOREIGN KEY ([ComponentId]) REFERENCES [dbo].[UIComponents] ([ComponentId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UIComponentUsages_ComponentId] ON [dbo].[UIComponentUsages] ([ComponentId]);
CREATE NONCLUSTERED INDEX [IX_UIComponentUsages_ModuleCode] ON [dbo].[UIComponentUsages] ([ModuleCode]);
```

### 2.3 資料字典

#### UIComponents 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ComponentId | BIGINT | - | NO | IDENTITY(1,1) | 組件ID | 主鍵 |
| ComponentCode | NVARCHAR | 50 | NO | - | 組件代碼 | IMS30_FB, IMS30_FI等 |
| ComponentName | NVARCHAR | 200 | NO | - | 組件名稱 | - |
| ComponentType | NVARCHAR | 20 | NO | - | 組件類型 | FB, FI, FU, FQ, PR, FS |
| ComponentVersion | NVARCHAR | 10 | NO | 'V1' | 組件版本 | V1, V2 |
| ConfigJson | NVARCHAR(MAX) | - | YES | - | 組件配置 | JSON格式 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢UI組件列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ui-components`
- **說明**: 查詢UI組件列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "componentCode": "",
    "componentType": "",
    "componentVersion": "",
    "status": ""
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
          "componentId": 1,
          "componentCode": "IMS30_FB",
          "componentName": "資料瀏覽組件",
          "componentType": "FB",
          "componentVersion": "V1",
          "status": "1",
          "createdAt": "2024-01-01T00:00:00"
        }
      ],
      "totalCount": 6,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆UI組件
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ui-components/{componentId}`
- **說明**: 根據組件ID查詢單筆UI組件
- **路徑參數**:
  - `componentId`: 組件ID

#### 3.1.3 新增UI組件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/ui-components`
- **說明**: 新增UI組件
- **請求格式**:
  ```json
  {
    "componentCode": "IMS30_FB",
    "componentName": "資料瀏覽組件",
    "componentType": "FB",
    "componentVersion": "V1",
    "configJson": "{}",
    "status": "1"
  }
  ```

#### 3.1.4 修改UI組件
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/ui-components/{componentId}`
- **說明**: 修改UI組件
- **路徑參數**:
  - `componentId`: 組件ID
- **請求格式**: 同新增

#### 3.1.5 刪除UI組件
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/ui-components/{componentId}`
- **說明**: 刪除UI組件
- **路徑參數**:
  - `componentId`: 組件ID

#### 3.1.6 查詢UI組件使用記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ui-components/{componentId}/usages`
- **說明**: 查詢UI組件使用記錄
- **路徑參數**:
  - `componentId`: 組件ID

### 3.2 後端實作類別

#### 3.2.1 Controller: `UIComponentsController.cs`
```csharp
[ApiController]
[Route("api/v1/ui-components")]
[Authorize]
public class UIComponentsController : ControllerBase
{
    private readonly IUIComponentService _uiComponentService;
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UIComponentDto>>>> GetComponents([FromQuery] UIComponentQueryRequest request)
    {
        // 實作查詢UI組件列表邏輯
    }
    
    [HttpGet("{componentId}")]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> GetComponent(long componentId)
    {
        // 實作查詢單筆UI組件邏輯
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> CreateComponent([FromBody] CreateUIComponentDto dto)
    {
        // 實作新增UI組件邏輯
    }
    
    [HttpPut("{componentId}")]
    public async Task<ActionResult<ApiResponse<UIComponentDto>>> UpdateComponent(long componentId, [FromBody] UpdateUIComponentDto dto)
    {
        // 實作修改UI組件邏輯
    }
    
    [HttpDelete("{componentId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComponent(long componentId)
    {
        // 實作刪除UI組件邏輯
    }
    
    [HttpGet("{componentId}/usages")]
    public async Task<ActionResult<ApiResponse<List<UIComponentUsageDto>>>> GetUsages(long componentId)
    {
        // 實作查詢UI組件使用記錄邏輯
    }
}
```

#### 3.2.2 Service: `UIComponentService.cs`
```csharp
public interface IUIComponentService
{
    Task<PagedResult<UIComponentDto>> GetComponentsAsync(UIComponentQueryRequest request);
    Task<UIComponentDto> GetComponentByIdAsync(long componentId);
    Task<UIComponentDto> CreateComponentAsync(CreateUIComponentDto dto);
    Task<UIComponentDto> UpdateComponentAsync(long componentId, UpdateUIComponentDto dto);
    Task<bool> DeleteComponentAsync(long componentId);
    Task<List<UIComponentUsageDto>> GetUsagesAsync(long componentId);
}
```

#### 3.2.3 Repository: `UIComponentRepository.cs`
```csharp
public interface IUIComponentRepository
{
    Task<PagedResult<UIComponent>> GetComponentsAsync(UIComponentQueryRequest request);
    Task<UIComponent> GetComponentByIdAsync(long componentId);
    Task<UIComponent> CreateComponentAsync(UIComponent component);
    Task<UIComponent> UpdateComponentAsync(UIComponent component);
    Task<bool> DeleteComponentAsync(long componentId);
    Task<List<UIComponentUsage>> GetUsagesAsync(long componentId);
    Task<UIComponentUsage> CreateUsageAsync(UIComponentUsage usage);
}
```

---

## 四、前端 UI 設計

### 4.1 組件結構

#### 4.1.1 資料瀏覽組件 (`IMS30FB.vue`)
- 提供資料瀏覽功能
- 支援分頁、排序、篩選
- 支援資料匯出

#### 4.1.2 資料新增組件 (`IMS30FI.vue`)
- 提供資料新增功能
- 支援表單驗證
- 支援AJAX非同步提交

#### 4.1.3 資料修改組件 (`IMS30FU.vue`)
- 提供資料修改功能
- 支援表單驗證
- 支援AJAX非同步提交

#### 4.1.4 資料查詢組件 (`IMS30FQ.vue`)
- 提供資料查詢功能
- 支援條件查詢
- 支援查詢結果顯示

#### 4.1.5 資料列印組件 (`IMS30PR.vue`)
- 提供資料列印功能
- 支援報表預覽
- 支援PDF列印

#### 4.1.6 資料保存組件 (`IMS30FS.vue`)
- 提供資料保存功能
- 支援批量保存
- 支援資料驗證

### 4.2 UI 元件設計

#### 4.2.1 UI組件管理頁面 (`UIComponentList.vue`)
```vue
<template>
  <div class="ui-component-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>UI組件管理</span>
          <el-button type="primary" @click="handleCreate">新增組件</el-button>
        </div>
      </template>
      
      <el-form :inline="true" :model="queryForm" class="query-form">
        <el-form-item label="組件代碼">
          <el-input v-model="queryForm.componentCode" placeholder="請輸入組件代碼" clearable />
        </el-form-item>
        <el-form-item label="組件類型">
          <el-select v-model="queryForm.componentType" placeholder="請選擇" clearable>
            <el-option label="瀏覽" value="FB" />
            <el-option label="新增" value="FI" />
            <el-option label="修改" value="FU" />
            <el-option label="查詢" value="FQ" />
            <el-option label="列印" value="PR" />
            <el-option label="保存" value="FS" />
          </el-select>
        </el-form-item>
        <el-form-item label="版本">
          <el-select v-model="queryForm.componentVersion" placeholder="請選擇" clearable>
            <el-option label="V1" value="V1" />
            <el-option label="V2" value="V2" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <el-table :data="componentList" border stripe>
        <el-table-column prop="componentCode" label="組件代碼" width="150" />
        <el-table-column prop="componentName" label="組件名稱" />
        <el-table-column prop="componentType" label="組件類型" width="100">
          <template #default="{ row }">
            <el-tag>{{ getComponentTypeName(row.componentType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="componentVersion" label="版本" width="80" />
        <el-table-column prop="status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === '1' ? 'success' : 'danger'">
              {{ row.status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleViewUsages(row)">使用記錄</el-button>
            <el-button type="info" size="small" @click="handleEdit(row)">編輯</el-button>
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
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { uiComponentApi } from '@/api/uiComponent.api'

const queryForm = ref({
  componentCode: '',
  componentType: '',
  componentVersion: '',
  status: ''
})

const componentList = ref([])
const pagination = ref({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

const getComponentTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'FB': '瀏覽',
    'FI': '新增',
    'FU': '修改',
    'FQ': '查詢',
    'PR': '列印',
    'FS': '保存'
  }
  return typeMap[type] || type
}

const loadComponents = async () => {
  try {
    const response = await uiComponentApi.getComponents({
      ...queryForm.value,
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize
    })
    componentList.value = response.data.items
    pagination.value.totalCount = response.data.totalCount
  } catch (error) {
    ElMessage.error('載入組件列表失敗')
  }
}

const handleQuery = () => {
  pagination.value.pageIndex = 1
  loadComponents()
}

const handleReset = () => {
  queryForm.value = {
    componentCode: '',
    componentType: '',
    componentVersion: '',
    status: ''
  }
  handleQuery()
}

const handleCreate = () => {
  // 開啟新增組件對話框
}

const handleEdit = (row: any) => {
  // 開啟編輯組件對話框
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('確定要刪除此組件嗎？', '提示', {
      type: 'warning'
    })
    await uiComponentApi.deleteComponent(row.componentId)
    ElMessage.success('刪除成功')
    loadComponents()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const handleViewUsages = (row: any) => {
  // 開啟使用記錄對話框
}

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size
  loadComponents()
}

const handlePageChange = (page: number) => {
  pagination.value.pageIndex = page
  loadComponents()
}

onMounted(() => {
  loadComponents()
})
</script>
```

### 4.3 API 呼叫 (`uiComponent.api.ts`)
```typescript
import request from '@/utils/request'
import { ApiResponse, PagedResult } from '@/types/api'

export interface UIComponentDto {
  componentId: number
  componentCode: string
  componentName: string
  componentType: string
  componentVersion: string
  configJson: string
  status: string
  createdAt: string
}

export interface UIComponentUsageDto {
  usageId: number
  moduleCode: string
  moduleName: string
  usageCount: number
  lastUsedAt: string
}

export const uiComponentApi = {
  getComponents: (params: any) => {
    return request.get<ApiResponse<PagedResult<UIComponentDto>>>('/api/v1/ui-components', { params })
  },
  
  getComponent: (componentId: number) => {
    return request.get<ApiResponse<UIComponentDto>>(`/api/v1/ui-components/${componentId}`)
  },
  
  createComponent: (data: any) => {
    return request.post<ApiResponse<UIComponentDto>>('/api/v1/ui-components', data)
  },
  
  updateComponent: (componentId: number, data: any) => {
    return request.put<ApiResponse<UIComponentDto>>(`/api/v1/ui-components/${componentId}`, data)
  },
  
  deleteComponent: (componentId: number) => {
    return request.delete<ApiResponse<boolean>>(`/api/v1/ui-components/${componentId}`)
  },
  
  getUsages: (componentId: number) => {
    return request.get<ApiResponse<List<UIComponentUsageDto>>>(`/api/v1/ui-components/${componentId}/usages`)
  }
}
```

---

## 五、開發任務清單

### 5.1 後端開發
- [ ] 建立 `UIComponents`、`UIComponentUsages` 資料表
- [ ] 建立 Entity 類別（UIComponent, UIComponentUsage）
- [ ] 建立 DTO 類別（UIComponentDto, UIComponentUsageDto等）
- [ ] 實作 `UIComponentRepository` 類別
- [ ] 實作 `UIComponentService` 類別
- [ ] 實作 `UIComponentsController` 類別
- [ ] 撰寫單元測試

### 5.2 前端開發
- [ ] 建立 `IMS30FB.vue` 資料瀏覽組件
- [ ] 建立 `IMS30FI.vue` 資料新增組件
- [ ] 建立 `IMS30FU.vue` 資料修改組件
- [ ] 建立 `IMS30FQ.vue` 資料查詢組件
- [ ] 建立 `IMS30PR.vue` 資料列印組件
- [ ] 建立 `IMS30FS.vue` 資料保存組件
- [ ] 建立 `UIComponentList.vue` UI組件管理頁面
- [ ] 建立 `uiComponent.api.ts` API 呼叫函數
- [ ] 建立路由設定
- [ ] 建立類型定義
- [ ] 撰寫單元測試

### 5.3 資料庫開發
- [ ] 建立 `UIComponents` 資料表
- [ ] 建立 `UIComponentUsages` 資料表
- [ ] 建立必要的索引
- [ ] 撰寫查詢效能測試

### 5.4 測試
- [ ] 單元測試
- [ ] 整合測試
- [ ] 效能測試
- [ ] 使用者驗收測試

---

## 六、注意事項

### 6.1 業務邏輯
- IMS30系列組件為通用組件，可被其他功能模組重用
- 支援V1和V2版本，V2版本為改進版本
- 組件配置使用JSON格式儲存
- 組件使用記錄用於追蹤組件被哪些模組使用

### 6.2 資料驗證
- 組件代碼必須唯一（同一版本）
- 組件類型必須為FB、FI、FU、FQ、PR、FS之一
- 組件版本必須為V1或V2

### 6.3 效能
- 組件配置應使用快取機制
- 組件使用記錄應定期清理舊資料

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢UI組件列表成功
- [ ] 新增UI組件成功
- [ ] 修改UI組件成功
- [ ] 刪除UI組件成功
- [ ] 查詢UI組件使用記錄成功

### 7.2 整合測試
- [ ] 完整CRUD流程測試
- [ ] 組件重用測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FB.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FI.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FU.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FQ.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_PR.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/IMS30_FS.aspx`
- `IMS3/HANSHIN/IMS3/WEB/UI/V201/IMS30_*.aspx`

### 8.2 相關功能
- 各功能模組的資料維護功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

