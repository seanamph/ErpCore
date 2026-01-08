# URDEF_SET_SYS - 使用者定義系統設定 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: URDEF_SET_SYS
- **功能名稱**: 使用者定義系統設定
- **功能描述**: 提供設定使用者自訂系統的功能，讓使用者可以自訂常用系統選單
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/URDEF_SET_SYS.asp`
  - `WEB/IMS_CORE/ASP/Kernel/URDEF_SET_SYS.asp`

### 1.2 業務需求
- 設定使用者的自訂系統列表
- 支援系統的新增、移除
- 支援系統列表查詢
- 與選單系統整合
- 支援系統排序

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UserDefinedSystems` (使用者定義系統)

```sql
CREATE TABLE [dbo].[UserDefinedSystems] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL, -- 使用者代碼
    [SysId] NVARCHAR(50) NOT NULL, -- 系統代碼
    [SysName] NVARCHAR(100) NULL, -- 系統名稱
    [SeqNo] INT NULL, -- 排序序號
    [IsDefault] BIT NOT NULL DEFAULT 0, -- 是否為預設系統
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [UQ_UserDefinedSystems_UserId_SysId] UNIQUE ([UserId], [SysId]),
    CONSTRAINT [FK_UserDefinedSystems_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserDefinedSystems_Systems] FOREIGN KEY ([SysId]) REFERENCES [dbo].[Systems] ([SysId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserDefinedSystems_UserId] ON [dbo].[UserDefinedSystems] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserDefinedSystems_SysId] ON [dbo].[UserDefinedSystems] ([SysId]);
CREATE NONCLUSTERED INDEX [IX_UserDefinedSystems_IsDefault] ON [dbo].[UserDefinedSystems] ([UserId], [IsDefault]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者代碼 | 外鍵至Users |
| SysId | NVARCHAR | 50 | NO | - | 系統代碼 | 外鍵至Systems |
| SysName | NVARCHAR | 100 | YES | - | 系統名稱 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| IsDefault | BIT | - | NO | 0 | 是否為預設系統 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 新增使用者定義系統
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/urdef/systems`
- **說明**: 新增使用者自訂系統
- **請求格式**:
  ```json
  {
    "sysId": "SYS0000",
    "seqNo": 1,
    "isDefault": false
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.2 修改使用者定義系統
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/urdef/systems/{id}`
- **說明**: 修改使用者自訂系統（主要用於更新排序序號或預設系統）
- **請求格式**:
  ```json
  {
    "seqNo": 2,
    "isDefault": true
  }
  ```
- **回應格式**: 標準修改回應格式

#### 3.1.3 設定預設系統
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/urdef/systems/{id}/set-default`
- **說明**: 設定使用者的預設系統（會自動取消其他系統的預設狀態）
- **回應格式**: 標準修改回應格式

#### 3.1.4 批量更新排序
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/urdef/systems/batch-update-seq`
- **說明**: 批量更新使用者自訂系統的排序序號
- **請求格式**:
  ```json
  {
    "items": [
      { "id": 1, "seqNo": 1 },
      { "id": 2, "seqNo": 2 },
      { "id": 3, "seqNo": 3 }
    ]
  }
  ```
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除使用者定義系統
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/urdef/systems/{id}`
- **說明**: 刪除使用者自訂系統
- **回應格式**: 標準刪除回應格式

#### 3.1.6 查詢使用者定義系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/systems`
- **說明**: 查詢當前使用者的自訂系統列表
- **回應格式**: 標準列表回應格式

#### 3.1.7 查詢可用的系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/available-systems`
- **說明**: 查詢使用者可以加入自訂系統的系統列表
- **查詢參數**:
  - `keyword`: 關鍵字搜尋（可選）
- **回應格式**: 標準列表回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `UrdefSetSysController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urdef/systems")]
    [Authorize]
    public class UrdefSetSysController : ControllerBase
    {
        private readonly IUserDefinedSystemService _service;
        
        public UrdefSetSysController(IUserDefinedSystemService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDefinedSystemDto>>>> GetSystems()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetSystemsByUserIdAsync(userId);
            return Ok(ApiResponse<List<UserDefinedSystemDto>>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateSystem([FromBody] CreateUserDefinedSystemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = await _service.CreateSystemAsync(userId, dto);
            return Ok(ApiResponse<long>.Success(id));
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateSystem(long id, [FromBody] UpdateUserDefinedSystemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.UpdateSystemAsync(id, userId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("{id}/set-default")]
        public async Task<ActionResult<ApiResponse>> SetDefaultSystem(long id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.SetDefaultSystemAsync(id, userId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("batch-update-seq")]
        public async Task<ActionResult<ApiResponse>> BatchUpdateSeq([FromBody] BatchUpdateSeqDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.BatchUpdateSeqAsync(userId, dto.Items);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteSystem(long id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.DeleteSystemAsync(id, userId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpGet("available-systems")]
        public async Task<ActionResult<ApiResponse<List<SystemDto>>>> GetAvailableSystems(
            [FromQuery] string keyword = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetAvailableSystemsAsync(userId, keyword);
            return Ok(ApiResponse<List<SystemDto>>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `UserDefinedSystemService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IUserDefinedSystemService
    {
        Task<List<UserDefinedSystemDto>> GetSystemsByUserIdAsync(string userId);
        Task<long> CreateSystemAsync(string userId, CreateUserDefinedSystemDto dto);
        Task UpdateSystemAsync(long id, string userId, UpdateUserDefinedSystemDto dto);
        Task SetDefaultSystemAsync(long id, string userId);
        Task BatchUpdateSeqAsync(string userId, List<UpdateSeqItemDto> items);
        Task DeleteSystemAsync(long id, string userId);
        Task<List<SystemDto>> GetAvailableSystemsAsync(string userId, string keyword = null);
    }
    
    public class UserDefinedSystemService : IUserDefinedSystemService
    {
        private readonly IUserDefinedSystemRepository _repository;
        private readonly ISystemRepository _systemRepository;
        private readonly IPermissionService _permissionService;
        
        public async Task<List<UserDefinedSystemDto>> GetSystemsByUserIdAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }
        
        public async Task<long> CreateSystemAsync(string userId, CreateUserDefinedSystemDto dto)
        {
            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(userId, dto.SysId);
            if (exists)
                throw new BusinessException("該系統已存在於自訂系統列表");
            
            // 檢查系統是否存在
            var system = await _systemRepository.GetByIdAsync(dto.SysId);
            if (system == null)
                throw new BusinessException("系統不存在");
            
            // 檢查使用者是否有權限使用該系統
            var hasPermission = await _permissionService.CheckSystemPermissionAsync(userId, dto.SysId);
            if (!hasPermission)
                throw new ForbiddenException("無權限使用此系統");
            
            // 如果設定為預設系統，先取消其他系統的預設狀態
            if (dto.IsDefault)
            {
                await _repository.ClearDefaultSystemAsync(userId);
            }
            
            var entity = new UserDefinedSystem
            {
                UserId = userId,
                SysId = dto.SysId,
                SysName = system.SysName,
                SeqNo = dto.SeqNo ?? await _repository.GetNextSeqNoAsync(userId),
                IsDefault = dto.IsDefault ?? false,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            
            return await _repository.InsertAsync(entity);
        }
        
        public async Task UpdateSystemAsync(long id, string userId, UpdateUserDefinedSystemDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                throw new NotFoundException("系統不存在");
            
            if (dto.SeqNo.HasValue)
                entity.SeqNo = dto.SeqNo.Value;
            
            if (dto.IsDefault.HasValue)
            {
                if (dto.IsDefault.Value)
                {
                    await _repository.ClearDefaultSystemAsync(userId);
                }
                entity.IsDefault = dto.IsDefault.Value;
            }
            
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAsync(entity);
        }
        
        public async Task SetDefaultSystemAsync(long id, string userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                throw new NotFoundException("系統不存在");
            
            // 取消其他系統的預設狀態
            await _repository.ClearDefaultSystemAsync(userId);
            
            // 設定為預設系統
            entity.IsDefault = true;
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _repository.UpdateAsync(entity);
        }
        
        public async Task BatchUpdateSeqAsync(string userId, List<UpdateSeqItemDto> items)
        {
            foreach (var item in items)
            {
                var entity = await _repository.GetByIdAsync(item.Id);
                if (entity != null && entity.UserId == userId)
                {
                    entity.SeqNo = item.SeqNo;
                    entity.UpdatedBy = userId;
                    entity.UpdatedAt = DateTime.UtcNow;
                    await _repository.UpdateAsync(entity);
                }
            }
        }
        
        public async Task DeleteSystemAsync(long id, string userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                throw new NotFoundException("系統不存在");
            
            await _repository.DeleteAsync(id);
        }
        
        public async Task<List<SystemDto>> GetAvailableSystemsAsync(string userId, string keyword = null)
        {
            // 取得使用者有權限的系統列表
            var systems = await _systemRepository.GetByUserIdAsync(userId, keyword);
            
            // 排除已經加入自訂系統的系統
            var existingSysIds = await _repository.GetSysIdsByUserIdAsync(userId);
            return systems.Where(s => !existingSysIds.Contains(s.SysId)).ToList();
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者定義系統設定頁面 (`UserDefinedSystemSet.vue`)
- **路徑**: `/xcom/urdef/set-system`
- **功能**: 設定使用者自訂系統，支援新增、修改、刪除、排序

### 4.2 UI 元件設計

#### 4.2.1 系統設定頁面 (`UserDefinedSystemSet.vue`)
```vue
<template>
  <div class="user-defined-system-set">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>我的自訂系統列表</span>
          <el-button type="primary" @click="handleAdd" :icon="Plus">新增系統</el-button>
        </div>
      </template>
      
      <el-table 
        :data="systemList" 
        border 
        stripe
        row-key="id"
        @row-dblclick="handleEdit"
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="sysId" label="系統代碼" width="120" />
        <el-table-column prop="sysName" label="系統名稱" width="200" />
        <el-table-column prop="seqNo" label="排序序號" width="100" align="center" />
        <el-table-column label="預設系統" width="100" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.isDefault" type="success">預設</el-tag>
            <el-button 
              v-else
              type="text"
              size="small"
              @click="handleSetDefault(row)"
            >
              設為預設
            </el-button>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button 
              type="primary" 
              size="small" 
              @click="handleEdit(row)"
              :icon="Edit"
            >
              修改
            </el-button>
            <el-button 
              type="danger" 
              size="small" 
              @click="handleDelete(row)"
              :icon="Delete"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
    
    <!-- 新增/修改對話框 -->
    <AddSystemDialog 
      v-model="dialogVisible"
      :form-data="currentFormData"
      :mode="dialogMode"
      @success="handleDialogSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus, Edit, Delete } from '@element-plus/icons-vue';
import { 
  getUserDefinedSystems, 
  createUserDefinedSystem,
  updateUserDefinedSystem,
  deleteUserDefinedSystem,
  setDefaultSystem,
  batchUpdateSeq
} from '@/api/urdef.api';
import AddSystemDialog from './components/AddSystemDialog.vue';

const systemList = ref([]);
const dialogVisible = ref(false);
const dialogMode = ref<'add' | 'edit'>('add');
const currentFormData = ref(null);

const loadData = async () => {
  try {
    const response = await getUserDefinedSystems();
    if (response.data.success) {
      systemList.value = response.data.data || [];
    }
  } catch (error) {
    ElMessage.error('載入資料失敗');
  }
};

const handleAdd = () => {
  dialogMode.value = 'add';
  currentFormData.value = null;
  dialogVisible.value = true;
};

const handleEdit = (row: any) => {
  dialogMode.value = 'edit';
  currentFormData.value = { ...row };
  dialogVisible.value = true;
};

const handleSetDefault = async (row: any) => {
  try {
    await setDefaultSystem(row.id);
    ElMessage.success('設定預設系統成功');
    loadData();
  } catch (error) {
    ElMessage.error('設定失敗');
  }
};

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('確定要刪除此系統嗎？', '確認刪除', {
      type: 'warning'
    });
    
    await deleteUserDefinedSystem(row.id);
    ElMessage.success('刪除成功');
    loadData();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗');
    }
  }
};

const handleDialogSuccess = () => {
  loadData();
};

onMounted(() => {
  loadData();
});
</script>
```

### 4.3 API 呼叫 (`urdef.api.ts` 擴展)
```typescript
export interface UserDefinedSystemDto {
  id: number;
  userId: string;
  sysId: string;
  sysName?: string;
  seqNo: number;
  isDefault: boolean;
  createdAt: string;
}

export interface CreateUserDefinedSystemDto {
  sysId: string;
  seqNo?: number;
  isDefault?: boolean;
}

export interface UpdateUserDefinedSystemDto {
  seqNo?: number;
  isDefault?: boolean;
}

// API 函數
export const getUserDefinedSystems = () => {
  return request.get<ApiResponse<UserDefinedSystemDto[]>>('/api/v1/urdef/systems');
};

export const createUserDefinedSystem = (data: CreateUserDefinedSystemDto) => {
  return request.post<ApiResponse<number>>('/api/v1/urdef/systems', data);
};

export const updateUserDefinedSystem = (id: number, data: UpdateUserDefinedSystemDto) => {
  return request.put<ApiResponse>(`/api/v1/urdef/systems/${id}`, data);
};

export const setDefaultSystem = (id: number) => {
  return request.put<ApiResponse>(`/api/v1/urdef/systems/${id}/set-default`);
};

export const deleteUserDefinedSystem = (id: number) => {
  return request.delete<ApiResponse>(`/api/v1/urdef/systems/${id}`);
};

export const getAvailableSystems = (keyword?: string) => {
  const params: any = {};
  if (keyword) params.keyword = keyword;
  return request.get<ApiResponse<SystemDto[]>>('/api/v1/urdef/available-systems', { params });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須檢查系統是否存在
- 必須檢查使用者是否有權限使用該系統
- 必須記錄所有操作日誌

### 6.2 業務邏輯
- 同一使用者不能重複新增相同系統
- 必須與選單系統整合
- 必須支援預設系統設定（同一使用者只能有一個預設系統）
- 必須支援排序功能

### 6.3 資料驗證
- 系統代碼必須存在於系統中
- 使用者必須有權限使用該系統
- 排序序號必須為正整數

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者定義系統列表成功
- [ ] 新增系統成功
- [ ] 新增系統失敗（重複新增）
- [ ] 新增系統失敗（系統不存在）
- [ ] 新增系統失敗（無權限）
- [ ] 修改排序序號成功
- [ ] 設定預設系統成功
- [ ] 批量更新排序序號成功
- [ ] 刪除系統成功
- [ ] 刪除系統失敗（系統不存在）

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 預設系統設定測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_SET_SYS.asp`
- `WEB/IMS_CORE/ASP/Kernel/URDEF_SET_SYS.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

