# URDEF_FUN_LIST - 使用者定義功能列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: URDEF_FUN_LIST
- **功能名稱**: 使用者定義功能列表
- **功能描述**: 提供加入我的最愛功能列表，讓使用者可以自訂常用功能選單
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/URDEF_FUN_LIST.asp`
  - `WEB/IMS_CORE/ASP/Kernel/URDEF_FUN_LIST.asp`

### 1.2 業務需求
- 管理使用者的自訂功能列表
- 支援功能的新增、移除
- 支援功能列表查詢
- 與選單系統整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UserDefinedFunctions` (使用者定義功能)

```sql
CREATE TABLE [dbo].[UserDefinedFunctions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL, -- 使用者代碼
    [ProgId] NVARCHAR(50) NOT NULL, -- 作業代碼
    [SysId] NVARCHAR(50) NULL, -- 系統代碼
    [MenuId] NVARCHAR(50) NULL, -- 選單代碼
    [SeqNo] INT NULL, -- 排序序號
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_UserDefinedFunctions_UserId_ProgId] UNIQUE ([UserId], [ProgId]),
    CONSTRAINT [FK_UserDefinedFunctions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserDefinedFunctions_UserId] ON [dbo].[UserDefinedFunctions] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserDefinedFunctions_ProgId] ON [dbo].[UserDefinedFunctions] ([ProgId]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢使用者定義功能列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/functions`
- **說明**: 查詢當前使用者的自訂功能列表
- **回應格式**: 標準列表回應格式

#### 3.1.2 新增使用者定義功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/urdef/functions`
- **說明**: 新增使用者自訂功能
- **請求格式**:
  ```json
  {
    "progId": "SYS0110",
    "sysId": "SYS0000",
    "menuId": "MENU001",
    "seqNo": 1
  }
  ```

#### 3.1.3 修改使用者定義功能（更新排序）
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/urdef/functions/{id}`
- **說明**: 修改使用者自訂功能的排序序號
- **請求格式**:
  ```json
  {
    "seqNo": 2
  }
  ```
- **回應格式**: 標準修改回應格式

#### 3.1.4 批量更新排序
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/urdef/functions/batch-update-seq`
- **說明**: 批量更新使用者自訂功能的排序序號
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

#### 3.1.5 刪除使用者定義功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/urdef/functions/{id}`
- **說明**: 刪除使用者自訂功能
- **回應格式**: 標準刪除回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `UserDefinedFunctionController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urdef/functions")]
    [Authorize]
    public class UserDefinedFunctionController : ControllerBase
    {
        private readonly IUserDefinedFunctionService _service;
        
        public UserDefinedFunctionController(IUserDefinedFunctionService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDefinedFunctionDto>>>> GetFunctions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetFunctionsByUserIdAsync(userId);
            return Ok(ApiResponse<List<UserDefinedFunctionDto>>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateFunction([FromBody] CreateUserDefinedFunctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = await _service.CreateFunctionAsync(userId, dto);
            return Ok(ApiResponse<long>.Success(id));
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateFunction(long id, [FromBody] UpdateUserDefinedFunctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.UpdateFunctionAsync(id, userId, dto);
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
        public async Task<ActionResult<ApiResponse>> DeleteFunction(long id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.DeleteFunctionAsync(id, userId);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `UserDefinedFunctionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IUserDefinedFunctionService
    {
        Task<List<UserDefinedFunctionDto>> GetFunctionsByUserIdAsync(string userId);
        Task<long> CreateFunctionAsync(string userId, CreateUserDefinedFunctionDto dto);
        Task UpdateFunctionAsync(long id, string userId, UpdateUserDefinedFunctionDto dto);
        Task BatchUpdateSeqAsync(string userId, List<UpdateSeqItemDto> items);
        Task DeleteFunctionAsync(long id, string userId);
    }
    
    public class UserDefinedFunctionService : IUserDefinedFunctionService
    {
        private readonly IUserDefinedFunctionRepository _repository;
        
        public async Task<List<UserDefinedFunctionDto>> GetFunctionsByUserIdAsync(string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }
        
        public async Task<long> CreateFunctionAsync(string userId, CreateUserDefinedFunctionDto dto)
        {
            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(userId, dto.ProgId);
            if (exists)
                throw new BusinessException("該功能已存在於我的最愛");
            
            // 檢查作業是否存在
            var progExists = await _repository.CheckProgExistsAsync(dto.ProgId);
            if (!progExists)
                throw new BusinessException("作業不存在");
            
            var entity = new UserDefinedFunction
            {
                UserId = userId,
                ProgId = dto.ProgId,
                SysId = dto.SysId,
                MenuId = dto.MenuId,
                SeqNo = dto.SeqNo ?? await _repository.GetNextSeqNoAsync(userId),
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            
            return await _repository.InsertAsync(entity);
        }
        
        public async Task UpdateFunctionAsync(long id, string userId, UpdateUserDefinedFunctionDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                throw new NotFoundException("功能不存在");
            
            entity.SeqNo = dto.SeqNo;
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
        
        public async Task DeleteFunctionAsync(long id, string userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                throw new NotFoundException("功能不存在");
            
            await _repository.DeleteAsync(id);
        }
    }
}
```

#### 3.2.3 Repository: `UserDefinedFunctionRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IUserDefinedFunctionRepository
    {
        Task<List<UserDefinedFunctionDto>> GetByUserIdAsync(string userId);
        Task<UserDefinedFunction> GetByIdAsync(long id);
        Task<bool> ExistsAsync(string userId, string progId);
        Task<bool> CheckProgExistsAsync(string progId);
        Task<int> GetNextSeqNoAsync(string userId);
        Task<long> InsertAsync(UserDefinedFunction entity);
        Task UpdateAsync(UserDefinedFunction entity);
        Task DeleteAsync(long id);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者定義功能列表頁面 (`UserDefinedFunctionList.vue`)
- **路徑**: `/xcom/urdef/functions`
- **功能**: 顯示使用者自訂功能列表，支援新增、修改、刪除、排序

### 4.2 UI 元件設計

#### 4.2.1 功能列表頁面 (`UserDefinedFunctionList.vue`)
```vue
<template>
  <div class="user-defined-function-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>我的最愛功能列表</span>
          <el-button type="primary" @click="handleAdd" :icon="Plus">新增功能</el-button>
        </div>
      </template>
      
      <el-table 
        :data="functionList" 
        border 
        stripe
        row-key="id"
        @row-dblclick="handleEdit"
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="progId" label="作業代碼" width="120" />
        <el-table-column prop="progName" label="作業名稱" width="200" />
        <el-table-column prop="sysId" label="系統代碼" width="120" />
        <el-table-column prop="menuId" label="選單代碼" width="120" />
        <el-table-column prop="seqNo" label="排序序號" width="100" align="center" />
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
      
      <!-- 拖拽排序提示 -->
      <div class="sort-hint">
        <el-icon><InfoFilled /></el-icon>
        <span>雙擊行可修改排序序號，或使用拖拽排序</span>
      </div>
    </el-card>
    
    <!-- 新增/修改對話框 -->
    <AddFunctionDialog 
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
import { Plus, Edit, Delete, InfoFilled } from '@element-plus/icons-vue';
import { 
  getUserDefinedFunctions, 
  createUserDefinedFunction,
  updateUserDefinedFunction,
  deleteUserDefinedFunction,
  batchUpdateSeq
} from '@/api/urdef.api';
import AddFunctionDialog from './components/AddFunctionDialog.vue';

const functionList = ref([]);
const dialogVisible = ref(false);
const dialogMode = ref<'add' | 'edit'>('add');
const currentFormData = ref(null);

const loadData = async () => {
  try {
    const response = await getUserDefinedFunctions();
    if (response.data.success) {
      functionList.value = response.data.data || [];
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

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('確定要刪除此功能嗎？', '確認刪除', {
      type: 'warning'
    });
    
    await deleteUserDefinedFunction(row.id);
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

#### 4.2.2 新增/修改功能對話框 (`AddFunctionDialog.vue`)
```vue
<template>
  <el-dialog
    v-model="visible"
    :title="mode === 'add' ? '新增功能' : '修改功能'"
    width="600px"
    @close="handleClose"
  >
    <el-form 
      :model="form" 
      :rules="rules" 
      ref="formRef" 
      label-width="120px"
    >
      <el-form-item label="作業代碼" prop="progId">
        <el-select
          v-model="form.progId"
          placeholder="請選擇作業"
          filterable
          :disabled="mode === 'edit'"
          @change="handleProgChange"
        >
          <el-option
            v-for="item in progList"
            :key="item.progId"
            :label="`${item.progId} - ${item.progName}`"
            :value="item.progId"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="系統代碼" prop="sysId">
        <el-input v-model="form.sysId" placeholder="請輸入系統代碼" />
      </el-form-item>
      
      <el-form-item label="選單代碼" prop="menuId">
        <el-input v-model="form.menuId" placeholder="請輸入選單代碼" />
      </el-form-item>
      
      <el-form-item label="排序序號" prop="seqNo">
        <el-input-number 
          v-model="form.seqNo" 
          :min="1" 
          placeholder="請輸入排序序號"
        />
      </el-form-item>
    </el-form>
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="loading">
        確定
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { 
  createUserDefinedFunction,
  updateUserDefinedFunction,
  getAvailablePrograms
} from '@/api/urdef.api';

const props = defineProps<{
  modelValue: boolean;
  formData?: any;
  mode: 'add' | 'edit';
}>();

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'success': [];
}>();

const formRef = ref();
const loading = ref(false);
const progList = ref([]);

const form = reactive({
  progId: '',
  sysId: '',
  menuId: '',
  seqNo: 1
});

const rules = {
  progId: [{ required: true, message: '請選擇作業', trigger: 'change' }],
  seqNo: [{ required: true, message: '請輸入排序序號', trigger: 'blur' }]
};

const visible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
});

watch(() => props.formData, (newVal) => {
  if (newVal) {
    Object.assign(form, newVal);
  } else {
    resetForm();
  }
}, { immediate: true });

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    loadAvailablePrograms();
  }
});

const loadAvailablePrograms = async () => {
  try {
    const response = await getAvailablePrograms();
    if (response.data.success) {
      progList.value = response.data.data || [];
    }
  } catch (error) {
    ElMessage.error('載入作業列表失敗');
  }
};

const handleProgChange = (progId: string) => {
  const prog = progList.value.find(p => p.progId === progId);
  if (prog) {
    form.sysId = prog.sysId || '';
    form.menuId = prog.menuId || '';
  }
};

const handleSubmit = async () => {
  if (!formRef.value) return;
  
  await formRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    loading.value = true;
    try {
      if (props.mode === 'add') {
        await createUserDefinedFunction(form);
        ElMessage.success('新增成功');
      } else {
        await updateUserDefinedFunction(props.formData.id, { seqNo: form.seqNo });
        ElMessage.success('修改成功');
      }
      emit('success');
      handleClose();
    } catch (error) {
      ElMessage.error(props.mode === 'add' ? '新增失敗' : '修改失敗');
    } finally {
      loading.value = false;
    }
  });
};

const handleClose = () => {
  resetForm();
  emit('update:modelValue', false);
};

const resetForm = () => {
  form.progId = '';
  form.sysId = '';
  form.menuId = '';
  form.seqNo = 1;
  formRef.value?.resetFields();
};
</script>
```

### 4.3 API 呼叫 (`urdef.api.ts`)
```typescript
import request from '@/utils/request';

export interface UserDefinedFunctionDto {
  id: number;
  userId: string;
  progId: string;
  progName?: string;
  sysId?: string;
  menuId?: string;
  seqNo: number;
  createdAt: string;
}

export interface CreateUserDefinedFunctionDto {
  progId: string;
  sysId?: string;
  menuId?: string;
  seqNo?: number;
}

export interface UpdateUserDefinedFunctionDto {
  seqNo: number;
}

export interface UpdateSeqItemDto {
  id: number;
  seqNo: number;
}

export interface BatchUpdateSeqDto {
  items: UpdateSeqItemDto[];
}

// API 函數
export const getUserDefinedFunctions = () => {
  return request.get<ApiResponse<UserDefinedFunctionDto[]>>('/api/v1/urdef/functions');
};

export const createUserDefinedFunction = (data: CreateUserDefinedFunctionDto) => {
  return request.post<ApiResponse<number>>('/api/v1/urdef/functions', data);
};

export const updateUserDefinedFunction = (id: number, data: UpdateUserDefinedFunctionDto) => {
  return request.put<ApiResponse>(`/api/v1/urdef/functions/${id}`, data);
};

export const batchUpdateSeq = (data: BatchUpdateSeqDto) => {
  return request.put<ApiResponse>('/api/v1/urdef/functions/batch-update-seq', data);
};

export const deleteUserDefinedFunction = (id: number) => {
  return request.delete<ApiResponse>(`/api/v1/urdef/functions/${id}`);
};

export const getAvailablePrograms = () => {
  return request.get<ApiResponse<any[]>>('/api/v1/urdef/available-programs');
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
- [ ] 新增功能對話框開發

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須檢查作業是否存在
- 必須記錄所有操作日誌

### 6.2 業務邏輯
- 同一使用者不能重複新增相同作業
- 必須與選單系統整合
- 必須支援排序功能
- 修改功能主要用於更新排序序號
- 支援批量更新排序序號

### 6.3 資料驗證
- 作業代碼必須存在於系統中
- 排序序號必須為正整數
- 同一使用者的排序序號不應重複（可自動調整）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者定義功能列表成功
- [ ] 新增功能成功
- [ ] 新增功能失敗（重複新增）
- [ ] 新增功能失敗（作業不存在）
- [ ] 修改排序序號成功
- [ ] 批量更新排序序號成功
- [ ] 刪除功能成功
- [ ] 刪除功能失敗（功能不存在）

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 排序功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_FUN_LIST.asp`
- `WEB/IMS_CORE/ASP/Kernel/URDEF_FUN_LIST.asp`
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_SET_FUN.ASP`
- `WEB/IMS_CORE/ASP/Kernel/URDEF_SET_FUN.ASP`

---

**文件版本**: v1.1  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

