# URDEF_SYS_LIST - 使用者定義系統列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: URDEF_SYS_LIST
- **功能名稱**: 使用者定義系統列表
- **功能描述**: 提供查詢使用者自訂系統列表的功能，顯示使用者已設定的自訂系統
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/URDEF_SYS_LIST.asp`
  - `WEB/IMS_CORE/ASP/Kernel/URDEF_SYS_LIST.asp`

### 1.2 業務需求
- 查詢使用者的自訂系統列表
- 顯示系統基本資訊
- 支援系統排序顯示
- 支援預設系統標示
- 與選單系統整合

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

#### 3.1.1 查詢使用者定義系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/systems`
- **說明**: 查詢當前使用者的自訂系統列表，依排序序號排序
- **查詢參數**:
  - `includeDefault`: 是否只查詢預設系統（可選，預設為 false）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "id": 1,
        "userId": "USER001",
        "sysId": "SYS0000",
        "sysName": "系統管理",
        "seqNo": 1,
        "isDefault": true,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢預設系統
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/urdef/systems/default`
- **說明**: 查詢當前使用者的預設系統
- **回應格式**: 標準單筆回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `UrdefSysListController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urdef/systems")]
    [Authorize]
    public class UrdefSysListController : ControllerBase
    {
        private readonly IUserDefinedSystemService _service;
        
        public UrdefSysListController(IUserDefinedSystemService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDefinedSystemDto>>>> GetSystems(
            [FromQuery] bool includeDefault = false)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            List<UserDefinedSystemDto> result;
            
            if (includeDefault)
            {
                var defaultSystem = await _service.GetDefaultSystemAsync(userId);
                result = defaultSystem != null ? new List<UserDefinedSystemDto> { defaultSystem } : new List<UserDefinedSystemDto>();
            }
            else
            {
                result = await _service.GetSystemsByUserIdAsync(userId);
            }
            
            return Ok(ApiResponse<List<UserDefinedSystemDto>>.Success(result));
        }
        
        [HttpGet("default")]
        public async Task<ActionResult<ApiResponse<UserDefinedSystemDto>>> GetDefaultSystem()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.GetDefaultSystemAsync(userId);
            
            if (result == null)
            {
                return Ok(ApiResponse<UserDefinedSystemDto>.Success(null, "無預設系統"));
            }
            
            return Ok(ApiResponse<UserDefinedSystemDto>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `UserDefinedSystemService.cs` (擴展)
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IUserDefinedSystemService
    {
        Task<List<UserDefinedSystemDto>> GetSystemsByUserIdAsync(string userId);
        Task<UserDefinedSystemDto> GetDefaultSystemAsync(string userId);
    }
    
    public class UserDefinedSystemService : IUserDefinedSystemService
    {
        private readonly IUserDefinedSystemRepository _repository;
        
        public async Task<List<UserDefinedSystemDto>> GetSystemsByUserIdAsync(string userId)
        {
            var entities = await _repository.GetByUserIdAsync(userId);
            
            // 依排序序號排序
            return entities
                .OrderBy(e => e.SeqNo ?? int.MaxValue)
                .ThenBy(e => e.SysId)
                .Select(e => new UserDefinedSystemDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    SysId = e.SysId,
                    SysName = e.SysName,
                    SeqNo = e.SeqNo ?? 0,
                    IsDefault = e.IsDefault,
                    CreatedAt = e.CreatedAt
                })
                .ToList();
        }
        
        public async Task<UserDefinedSystemDto> GetDefaultSystemAsync(string userId)
        {
            var entity = await _repository.GetDefaultSystemByUserIdAsync(userId);
            
            if (entity == null)
                return null;
            
            return new UserDefinedSystemDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                SysId = entity.SysId,
                SysName = entity.SysName,
                SeqNo = entity.SeqNo ?? 0,
                IsDefault = entity.IsDefault,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
```

#### 3.2.3 Repository: `UserDefinedSystemRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IUserDefinedSystemRepository
    {
        Task<List<UserDefinedSystem>> GetByUserIdAsync(string userId);
        Task<UserDefinedSystem> GetDefaultSystemByUserIdAsync(string userId);
    }
    
    public class UserDefinedSystemRepository : IUserDefinedSystemRepository
    {
        private readonly IDbConnection _connection;
        
        public async Task<List<UserDefinedSystem>> GetByUserIdAsync(string userId)
        {
            const string sql = @"
                SELECT 
                    Id, UserId, SysId, SysName, SeqNo, IsDefault,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM UserDefinedSystems
                WHERE UserId = @UserId
                ORDER BY SeqNo, SysId";
            
            return (await _connection.QueryAsync<UserDefinedSystem>(sql, new { UserId = userId })).ToList();
        }
        
        public async Task<UserDefinedSystem> GetDefaultSystemByUserIdAsync(string userId)
        {
            const string sql = @"
                SELECT TOP 1
                    Id, UserId, SysId, SysName, SeqNo, IsDefault,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                FROM UserDefinedSystems
                WHERE UserId = @UserId AND IsDefault = 1";
            
            return await _connection.QueryFirstOrDefaultAsync<UserDefinedSystem>(sql, new { UserId = userId });
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者定義系統列表頁面 (`UserDefinedSystemList.vue`)
- **路徑**: `/xcom/urdef/system-list`
- **功能**: 顯示使用者自訂系統列表，支援查看、導航

### 4.2 UI 元件設計

#### 4.2.1 系統列表頁面 (`UserDefinedSystemList.vue`)
```vue
<template>
  <div class="user-defined-system-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>我的自訂系統</span>
          <el-button type="primary" @click="handleGoToSet" :icon="Setting">設定系統</el-button>
        </div>
      </template>
      
      <el-table 
        :data="systemList" 
        border 
        stripe
        row-key="id"
        @row-click="handleRowClick"
        v-loading="loading"
      >
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="sysId" label="系統代碼" width="120" />
        <el-table-column prop="sysName" label="系統名稱" width="200" />
        <el-table-column prop="seqNo" label="排序序號" width="100" align="center" />
        <el-table-column label="預設系統" width="100" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.isDefault" type="success">預設</el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button 
              type="primary" 
              size="small" 
              @click="handleNavigate(row)"
              :icon="Right"
            >
              進入系統
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div v-if="systemList.length === 0" class="empty-state">
        <el-empty description="尚未設定自訂系統">
          <el-button type="primary" @click="handleGoToSet">前往設定</el-button>
        </el-empty>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { Setting, Right } from '@element-plus/icons-vue';
import { getUserDefinedSystems } from '@/api/urdef.api';

const router = useRouter();
const loading = ref(false);
const systemList = ref([]);

const loadData = async () => {
  loading.value = true;
  try {
    const response = await getUserDefinedSystems();
    if (response.data.success) {
      systemList.value = response.data.data || [];
    }
  } catch (error) {
    ElMessage.error('載入資料失敗');
  } finally {
    loading.value = false;
  }
};

const handleRowClick = (row: any) => {
  // 雙擊進入系統
  handleNavigate(row);
};

const handleNavigate = (row: any) => {
  // 導航到對應的系統
  router.push(`/system/${row.sysId}`);
};

const handleGoToSet = () => {
  router.push('/xcom/urdef/set-system');
};

onMounted(() => {
  loadData();
});
</script>

<style scoped>
.empty-state {
  padding: 40px 0;
  text-align: center;
}
</style>
```

#### 4.2.2 系統選擇器元件 (`SystemSelector.vue`)
```vue
<template>
  <el-select
    v-model="selectedSysId"
    placeholder="請選擇系統"
    filterable
    @change="handleChange"
  >
    <el-option
      v-for="item in systemList"
      :key="item.sysId"
      :label="`${item.sysId} - ${item.sysName}`"
      :value="item.sysId"
    >
      <span>{{ item.sysId }} - {{ item.sysName }}</span>
      <el-tag v-if="item.isDefault" type="success" size="small" style="margin-left: 10px">預設</el-tag>
    </el-option>
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { getUserDefinedSystems } from '@/api/urdef.api';

const props = defineProps<{
  modelValue: string;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: string];
  'change': [value: string];
}>();

const selectedSysId = ref(props.modelValue);
const systemList = ref([]);

const loadData = async () => {
  try {
    const response = await getUserDefinedSystems();
    if (response.data.success) {
      systemList.value = response.data.data || [];
    }
  } catch (error) {
    console.error('載入系統列表失敗', error);
  }
};

const handleChange = (value: string) => {
  emit('update:modelValue', value);
  emit('change', value);
};

watch(() => props.modelValue, (newVal) => {
  selectedSysId.value = newVal;
});

onMounted(() => {
  loadData();
});
</script>
```

### 4.3 API 呼叫 (`urdef.api.ts` 擴展)
```typescript
// 查詢使用者定義系統列表
export const getUserDefinedSystems = (includeDefault?: boolean) => {
  const params: any = {};
  if (includeDefault) params.includeDefault = includeDefault;
  return request.get<ApiResponse<UserDefinedSystemDto[]>>('/api/v1/urdef/systems', { params });
};

// 查詢預設系統
export const getDefaultSystem = () => {
  return request.get<ApiResponse<UserDefinedSystemDto>>('/api/v1/urdef/systems/default');
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構
- [ ] 確認索引設計

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 系統選擇器元件開發

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 4天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 只能查詢當前使用者的自訂系統
- 必須記錄查詢日誌

### 6.2 業務邏輯
- 必須依排序序號排序顯示
- 必須標示預設系統
- 必須與選單系統整合
- 支援快速導航到系統

### 6.3 效能優化
- 查詢結果應快取
- 支援分頁查詢（如果系統數量很多）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者定義系統列表成功
- [ ] 查詢預設系統成功
- [ ] 查詢預設系統失敗（無預設系統）
- [ ] 列表依排序序號排序正確

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 排序功能測試
- [ ] 導航功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_SYS_LIST.asp`
- `WEB/IMS_CORE/ASP/Kernel/URDEF_SYS_LIST.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

