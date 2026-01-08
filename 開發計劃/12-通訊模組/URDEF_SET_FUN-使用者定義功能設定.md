# URDEF_SET_FUN - 使用者定義功能設定 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: URDEF_SET_FUN
- **功能名稱**: 使用者定義功能設定
- **功能描述**: 提供設定使用者自訂功能的功能，讓使用者可以將常用功能加入我的最愛
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/URDEF_SET_FUN.ASP`
  - `WEB/IMS_CORE/ASP/Kernel/URDEF_SET_FUN.ASP`

### 1.2 業務需求
- 設定使用者的自訂功能
- 支援功能的新增、移除
- 支援功能列表查詢
- 與選單系統整合
- 支援批量設定

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

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者代碼 | 外鍵至Users |
| ProgId | NVARCHAR | 50 | NO | - | 作業代碼 | - |
| SysId | NVARCHAR | 50 | YES | - | 系統代碼 | - |
| MenuId | NVARCHAR | 50 | YES | - | 選單代碼 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 設定使用者定義功能（新增）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/urdef/functions`
- **說明**: 新增使用者自訂功能到我的最愛
- **請求格式**:
  ```json
  {
    "progId": "SYS0110",
    "sysId": "SYS0000",
    "menuId": "MENU001",
    "seqNo": 1
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.2 批量設定使用者定義功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/urdef/functions/batch`
- **說明**: 批量新增使用者自訂功能
- **請求格式**:
  ```json
  {
    "items": [
      {
        "progId": "SYS0110",
        "sysId": "SYS0000",
        "menuId": "MENU001",
        "seqNo": 1
      },
      {
        "progId": "SYS0210",
        "sysId": "SYS0000",
        "menuId": "MENU001",
        "seqNo": 2
      }
    ]
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.3 查詢可用的作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/available-programs`
- **說明**: 查詢使用者可以加入我的最愛的作業列表
- **查詢參數**:
  - `sysId`: 系統代碼（可選）
  - `keyword`: 關鍵字搜尋（可選）
- **回應格式**: 標準列表回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `UrdefSetFunController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urdef")]
    [Authorize]
    public class UrdefSetFunController : ControllerBase
    {
        private readonly IUserDefinedFunctionService _service;
        
        public UrdefSetFunController(IUserDefinedFunctionService service)
        {
            _service = service;
        }
        
        [HttpPost("functions")]
        public async Task<ActionResult<ApiResponse<long>>> SetFunction([FromBody] CreateUserDefinedFunctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = await _service.CreateFunctionAsync(userId, dto);
            return Ok(ApiResponse<long>.Success(id));
        }
        
        [HttpPost("functions/batch")]
        public async Task<ActionResult<ApiResponse>> BatchSetFunctions([FromBody] BatchCreateUserDefinedFunctionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _service.BatchCreateFunctionsAsync(userId, dto.Items);
            return Ok(ApiResponse.Success());
        }
        
        [HttpGet("available-programs")]
        public async Task<ActionResult<ApiResponse<List<ProgramDto>>>> GetAvailablePrograms(
            [FromQuery] string sysId = null,
            [FromQuery] string keyword = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetAvailableProgramsAsync(userId, sysId, keyword);
            return Ok(ApiResponse<List<ProgramDto>>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `UserDefinedFunctionService.cs` (擴展)
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IUserDefinedFunctionService
    {
        Task<long> CreateFunctionAsync(string userId, CreateUserDefinedFunctionDto dto);
        Task BatchCreateFunctionsAsync(string userId, List<CreateUserDefinedFunctionDto> items);
        Task<List<ProgramDto>> GetAvailableProgramsAsync(string userId, string sysId = null, string keyword = null);
    }
    
    public class UserDefinedFunctionService : IUserDefinedFunctionService
    {
        private readonly IUserDefinedFunctionRepository _repository;
        private readonly IProgramRepository _programRepository;
        private readonly IPermissionService _permissionService;
        
        public async Task<long> CreateFunctionAsync(string userId, CreateUserDefinedFunctionDto dto)
        {
            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(userId, dto.ProgId);
            if (exists)
                throw new BusinessException("該功能已存在於我的最愛");
            
            // 檢查作業是否存在
            var prog = await _programRepository.GetByIdAsync(dto.ProgId);
            if (prog == null)
                throw new BusinessException("作業不存在");
            
            // 檢查使用者是否有權限使用該作業
            var hasPermission = await _permissionService.CheckProgramPermissionAsync(userId, dto.ProgId);
            if (!hasPermission)
                throw new ForbiddenException("無權限使用此作業");
            
            var entity = new UserDefinedFunction
            {
                UserId = userId,
                ProgId = dto.ProgId,
                SysId = dto.SysId ?? prog.SysId,
                MenuId = dto.MenuId,
                SeqNo = dto.SeqNo ?? await _repository.GetNextSeqNoAsync(userId),
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            
            return await _repository.InsertAsync(entity);
        }
        
        public async Task BatchCreateFunctionsAsync(string userId, List<CreateUserDefinedFunctionDto> items)
        {
            foreach (var item in items)
            {
                try
                {
                    await CreateFunctionAsync(userId, item);
                }
                catch (Exception ex)
                {
                    // 記錄錯誤但繼續處理其他項目
                    _logger.LogWarning(ex, "批量新增功能失敗: {ProgId}", item.ProgId);
                }
            }
        }
        
        public async Task<List<ProgramDto>> GetAvailableProgramsAsync(string userId, string sysId = null, string keyword = null)
        {
            // 取得使用者有權限的作業列表
            var programs = await _programRepository.GetByUserIdAsync(userId, sysId, keyword);
            
            // 排除已經加入我的最愛的作業
            var existingProgIds = await _repository.GetProgIdsByUserIdAsync(userId);
            return programs.Where(p => !existingProgIds.Contains(p.ProgId)).ToList();
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者定義功能設定頁面 (`UserDefinedFunctionSet.vue`)
- **路徑**: `/xcom/urdef/set-function`
- **功能**: 設定使用者自訂功能，支援單筆新增和批量新增

### 4.2 UI 元件設計

#### 4.2.1 功能設定頁面 (`UserDefinedFunctionSet.vue`)
```vue
<template>
  <div class="user-defined-function-set">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>設定我的最愛功能</span>
        </div>
      </template>
      
      <el-form :model="form" label-width="120px">
        <el-form-item label="系統代碼">
          <el-select
            v-model="form.sysId"
            placeholder="請選擇系統"
            filterable
            clearable
            @change="handleSysChange"
          >
            <el-option
              v-for="item in sysList"
              :key="item.sysId"
              :label="`${item.sysId} - ${item.sysName}`"
              :value="item.sysId"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="搜尋作業">
          <el-input
            v-model="form.keyword"
            placeholder="請輸入作業代碼或名稱"
            clearable
            @input="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-form-item>
      </el-form>
      
      <el-table 
        :data="availablePrograms" 
        border 
        stripe
        @selection-change="handleSelectionChange"
        v-loading="loading"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="progId" label="作業代碼" width="120" />
        <el-table-column prop="progName" label="作業名稱" width="200" />
        <el-table-column prop="sysId" label="系統代碼" width="120" />
        <el-table-column prop="menuId" label="選單代碼" width="120" />
      </el-table>
      
      <div class="action-bar">
        <el-button 
          type="primary" 
          @click="handleBatchAdd"
          :disabled="selectedPrograms.length === 0"
          :icon="Plus"
        >
          批量加入我的最愛
        </el-button>
        <el-button @click="handleReset">重置</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Plus, Search } from '@element-plus/icons-vue';
import { 
  getAvailablePrograms,
  batchCreateUserDefinedFunctions
} from '@/api/urdef.api';

const loading = ref(false);
const sysList = ref([]);
const availablePrograms = ref([]);
const selectedPrograms = ref([]);

const form = reactive({
  sysId: '',
  keyword: ''
});

const loadAvailablePrograms = async () => {
  loading.value = true;
  try {
    const response = await getAvailablePrograms(form.sysId, form.keyword);
    if (response.data.success) {
      availablePrograms.value = response.data.data || [];
    }
  } catch (error) {
    ElMessage.error('載入作業列表失敗');
  } finally {
    loading.value = false;
  }
};

const handleSysChange = () => {
  loadAvailablePrograms();
};

const handleSearch = () => {
  loadAvailablePrograms();
};

const handleSelectionChange = (selection: any[]) => {
  selectedPrograms.value = selection;
};

const handleBatchAdd = async () => {
  if (selectedPrograms.value.length === 0) {
    ElMessage.warning('請至少選擇一個作業');
    return;
  }
  
  try {
    const items = selectedPrograms.value.map((prog, index) => ({
      progId: prog.progId,
      sysId: prog.sysId,
      menuId: prog.menuId,
      seqNo: index + 1
    }));
    
    await batchCreateUserDefinedFunctions({ items });
    ElMessage.success(`成功加入 ${items.length} 個功能到我的最愛`);
    loadAvailablePrograms();
    selectedPrograms.value = [];
  } catch (error) {
    ElMessage.error('加入失敗');
  }
};

const handleReset = () => {
  form.sysId = '';
  form.keyword = '';
  loadAvailablePrograms();
};

onMounted(() => {
  loadAvailablePrograms();
});
</script>
```

### 4.3 API 呼叫 (`urdef.api.ts` 擴展)
```typescript
// 批量新增使用者定義功能
export const batchCreateUserDefinedFunctions = (data: BatchCreateUserDefinedFunctionDto) => {
  return request.post<ApiResponse>('/api/v1/urdef/functions/batch', data);
};

// 查詢可用的作業列表
export const getAvailablePrograms = (sysId?: string, keyword?: string) => {
  const params: any = {};
  if (sysId) params.sysId = sysId;
  if (keyword) params.keyword = keyword;
  return request.get<ApiResponse<ProgramDto[]>>('/api/v1/urdef/available-programs', { params });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構
- [ ] 確認索引設計

### 5.2 階段二: 後端開發 (2天)
- [ ] Service 擴展實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 權限檢查邏輯

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 設定頁面開發
- [ ] 批量選擇功能

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須檢查作業是否存在
- 必須檢查使用者是否有權限使用該作業
- 必須記錄所有操作日誌

### 6.2 業務邏輯
- 同一使用者不能重複新增相同作業
- 必須與選單系統整合
- 必須支援批量新增
- 必須支援搜尋功能

### 6.3 資料驗證
- 作業代碼必須存在於系統中
- 使用者必須有權限使用該作業
- 排序序號必須為正整數

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增功能成功
- [ ] 新增功能失敗（重複新增）
- [ ] 新增功能失敗（作業不存在）
- [ ] 新增功能失敗（無權限）
- [ ] 批量新增功能成功
- [ ] 查詢可用作業列表成功

### 7.2 整合測試
- [ ] 完整設定流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 批量操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_SET_FUN.ASP`
- `WEB/IMS_CORE/ASP/Kernel/URDEF_SET_FUN.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

