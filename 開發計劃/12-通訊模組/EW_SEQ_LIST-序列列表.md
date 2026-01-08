# EW_SEQ_LIST - 序列列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: EW_SEQ_LIST
- **功能名稱**: 序列列表（作業列表）
- **功能描述**: 提供作業列表查詢功能，根據子系統項目（選單ID）顯示該選單下的所有作業及其序號，支援按作業代碼、作業名稱、序號排序
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/EW_SEQ_LIST.asp` (序列列表主程式)

### 1.2 業務需求
- 提供作業列表查詢功能（根據子系統項目篩選）
- 顯示作業代碼、作業名稱、序號
- 支援按作業代碼、作業名稱、序號排序
- 支援彈窗模式顯示
- 與作業主檔（MNG_PROG）整合
- 與選單主檔（MNG_MENU）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Programs` (作業主檔，對應舊系統 `MNG_PROG`)

```sql
CREATE TABLE [dbo].[Programs] (
    [ProgId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 作業代碼 (PROG_ID)
    [ProgName] NVARCHAR(200) NOT NULL, -- 作業名稱 (PROG_NAME)
    [MenuId] NVARCHAR(50) NULL, -- 選單ID (MENU_ID)
    [SeqNo] INT NULL, -- 序號 (SEQ_NO)
    [SystemId] NVARCHAR(50) NULL, -- 系統ID (SYS_ID)
    [Url] NVARCHAR(500) NULL, -- 連結網址 (URL)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Programs] PRIMARY KEY CLUSTERED ([ProgId] ASC),
    CONSTRAINT [FK_Programs_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([MenuId]),
    CONSTRAINT [FK_Programs_Systems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[Systems] ([SystemId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Programs_MenuId] ON [dbo].[Programs] ([MenuId]);
CREATE NONCLUSTERED INDEX [IX_Programs_ProgName] ON [dbo].[Programs] ([ProgName]);
CREATE NONCLUSTERED INDEX [IX_Programs_SeqNo] ON [dbo].[Programs] ([SeqNo]);
CREATE NONCLUSTERED INDEX [IX_Programs_SystemId] ON [dbo].[Programs] ([SystemId]);
```

### 2.2 相關資料表

#### 2.2.1 `Menus` - 選單主檔
參考 `EW_MENU_LIST-選單列表.md` 的 `Menus` 資料表設計

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ProgId | NVARCHAR | 50 | NO | - | 作業代碼 | 主鍵 |
| ProgName | NVARCHAR | 200 | NO | - | 作業名稱 | - |
| MenuId | NVARCHAR | 50 | YES | - | 選單ID | 外鍵至Menus |
| SeqNo | INT | - | YES | - | 序號 | - |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至Systems |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/seq-list`
- **說明**: 根據選單ID查詢作業列表，支援排序
- **請求參數**:
  ```json
  {
    "menuId": "MENU001",
    "orderBy": "PROG_ID",  // PROG_ID, PROG_NAME, SEQ_NO
    "filter": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "menuId": "MENU001",
      "menuName": "子系統名稱",
      "items": [
        {
          "progId": "PROG001",
          "progName": "作業名稱",
          "seqNo": 1,
          "menuId": "MENU001",
          "menuName": "子系統名稱"
        }
      ],
      "totalCount": 50
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆作業
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/seq-list/{progId}`
- **說明**: 根據作業代碼查詢單筆作業資料
- **回應格式**: 標準單筆資料回應

### 3.2 後端實作類別

#### 3.2.1 Controller: `EwSeqListController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ew/seq-list")]
    [Authorize]
    public class EwSeqListController : ControllerBase
    {
        private readonly IEwSeqListService _seqListService;
        
        public EwSeqListController(IEwSeqListService seqListService)
        {
            _seqListService = seqListService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<SeqListResultDto>>> GetSeqList([FromQuery] SeqListQueryDto query)
        {
            if (string.IsNullOrEmpty(query.MenuId))
            {
                return BadRequest(new ApiResponse { Success = false, Message = "請先選擇子系統項目！" });
            }
            
            var result = await _seqListService.GetSeqListAsync(query);
            return Ok(new ApiResponse<SeqListResultDto> { Success = true, Data = result });
        }
        
        [HttpGet("{progId}")]
        public async Task<ActionResult<ApiResponse<ProgramDto>>> GetProgram(string progId)
        {
            var result = await _seqListService.GetProgramByIdAsync(progId);
            return Ok(new ApiResponse<ProgramDto> { Success = true, Data = result });
        }
    }
}
```

#### 3.2.2 Service: `EwSeqListService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IEwSeqListService
    {
        Task<SeqListResultDto> GetSeqListAsync(SeqListQueryDto query);
        Task<ProgramDto> GetProgramByIdAsync(string progId);
    }
    
    public class EwSeqListService : IEwSeqListService
    {
        private readonly IProgramRepository _programRepository;
        private readonly IMenuRepository _menuRepository;
        
        public EwSeqListService(
            IProgramRepository programRepository,
            IMenuRepository menuRepository)
        {
            _programRepository = programRepository;
            _menuRepository = menuRepository;
        }
        
        public async Task<SeqListResultDto> GetSeqListAsync(SeqListQueryDto query)
        {
            // 查詢選單名稱
            var menu = await _menuRepository.GetByIdAsync(query.MenuId);
            if (menu == null)
            {
                throw new NotFoundException($"選單 {query.MenuId} 不存在");
            }
            
            // 查詢作業列表
            var programs = await _programRepository.GetByMenuIdAsync(query.MenuId, query.OrderBy);
            
            return new SeqListResultDto
            {
                MenuId = query.MenuId,
                MenuName = menu.MenuName,
                Items = programs.Select(p => new ProgramDto
                {
                    ProgId = p.ProgId,
                    ProgName = p.ProgName,
                    SeqNo = p.SeqNo,
                    MenuId = p.MenuId,
                    MenuName = menu.MenuName
                }).ToList(),
                TotalCount = programs.Count
            };
        }
        
        public async Task<ProgramDto> GetProgramByIdAsync(string progId)
        {
            var program = await _programRepository.GetByIdAsync(progId);
            if (program == null)
            {
                throw new NotFoundException($"作業 {progId} 不存在");
            }
            
            var menu = program.MenuId != null 
                ? await _menuRepository.GetByIdAsync(program.MenuId) 
                : null;
            
            return new ProgramDto
            {
                ProgId = program.ProgId,
                ProgName = program.ProgName,
                SeqNo = program.SeqNo,
                MenuId = program.MenuId,
                MenuName = menu?.MenuName
            };
        }
    }
}
```

#### 3.2.3 Repository: `ProgramRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IProgramRepository
    {
        Task<List<Program>> GetByMenuIdAsync(string menuId, string orderBy = "PROG_ID");
        Task<Program> GetByIdAsync(string progId);
    }
    
    public class ProgramRepository : IProgramRepository
    {
        private readonly IDbConnection _connection;
        
        public ProgramRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<List<Program>> GetByMenuIdAsync(string menuId, string orderBy = "PROG_ID")
        {
            var validOrderBy = new[] { "PROG_ID", "PROG_NAME", "SEQ_NO" }.Contains(orderBy.ToUpper())
                ? orderBy.ToUpper()
                : "PROG_ID";
            
            var sql = $@"
                SELECT ProgId, ProgName, MenuId, SeqNo, SystemId, Status
                FROM Programs
                WHERE MenuId = @MenuId AND Status = '1'
                ORDER BY {validOrderBy}";
            
            var programs = await _connection.QueryAsync<Program>(sql, new { MenuId = menuId });
            return programs.ToList();
        }
        
        public async Task<Program> GetByIdAsync(string progId)
        {
            var sql = @"
                SELECT ProgId, ProgName, MenuId, SeqNo, SystemId, Status
                FROM Programs
                WHERE ProgId = @ProgId";
            
            return await _connection.QueryFirstOrDefaultAsync<Program>(sql, new { ProgId = progId });
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 作業列表顯示頁面 (`EwSeqList.vue`)
- **路徑**: `/ew/seq-list`
- **功能**: 顯示作業列表，支援排序、關閉
- **主要元件**:
  - 標題區域（顯示子系統代碼和名稱）
  - 資料表格 (SeqListDataTable)
  - 關閉按鈕

### 4.2 UI 元件設計

#### 4.2.1 作業列表頁面 (`EwSeqList.vue`)
```vue
<template>
  <div class="ew-seq-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span class="title">作業列表</span>
        </div>
      </template>
      
      <div class="menu-info">
        <el-text>子系統代碼：</el-text>
        <el-text type="primary">{{ menuId }}</el-text>
        <el-text>，</el-text>
        <el-text>子系統名稱：</el-text>
        <el-text type="primary">{{ menuName }}</el-text>
      </div>
      
      <div class="action-bar">
        <el-button @click="handleClose">關閉</el-button>
      </div>
      
      <seq-list-data-table
        :menu-id="menuId"
        :order-by="orderBy"
        @order-change="handleOrderChange"
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getMenu } from '@/api/ewMenuList.api';
import SeqListDataTable from './components/SeqListDataTable.vue';

const route = useRoute();
const menuId = ref<string>('');
const menuName = ref<string>('');
const orderBy = ref<string>('PROG_ID');

onMounted(async () => {
  menuId.value = route.query.menuId as string || '';
  orderBy.value = (route.query.orderBy as string) || 'PROG_ID';
  
  if (!menuId.value) {
    ElMessage.error('請先選擇子系統項目！');
    return;
  }
  
  // 查詢選單名稱
  try {
    const menu = await getMenu(menuId.value);
    menuName.value = menu.data.menuName;
  } catch (error) {
    console.error('查詢選單失敗:', error);
  }
});

const handleOrderChange = (newOrderBy: string) => {
  orderBy.value = newOrderBy;
};

const handleClose = () => {
  window.close();
};
</script>

<style scoped>
.ew-seq-list {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: center;
}

.title {
  font-size: 20px;
  font-weight: bold;
}

.menu-info {
  margin-bottom: 20px;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
}

.action-bar {
  margin-bottom: 20px;
}
</style>
```

#### 4.2.2 資料表格元件 (`SeqListDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="programList" v-loading="loading" border>
      <el-table-column type="index" label="序號" width="60" align="center" />
      <el-table-column prop="progId" label="作業代碼" width="150" align="center">
        <template #header>
          <el-link 
            :underline="false" 
            @click="handleSort('PROG_ID')"
            :class="{ 'active': orderBy === 'PROG_ID' }"
          >
            作業代碼
          </el-link>
        </template>
      </el-table-column>
      <el-table-column prop="progName" label="作業名稱">
        <template #header>
          <el-link 
            :underline="false" 
            @click="handleSort('PROG_NAME')"
            :class="{ 'active': orderBy === 'PROG_NAME' }"
          >
            作業名稱
          </el-link>
        </template>
      </el-table-column>
      <el-table-column prop="seqNo" label="序號" width="100" align="right">
        <template #header>
          <el-link 
            :underline="false" 
            @click="handleSort('SEQ_NO')"
            :class="{ 'active': orderBy === 'SEQ_NO' }"
          >
            序號
          </el-link>
        </template>
      </el-table-column>
    </el-table>
    
    <div class="total-info">
      <el-text>符合條件者共 {{ totalCount }} 筆</el-text>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { getSeqList } from '@/api/ewSeqList.api';

interface Props {
  menuId: string;
  orderBy: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  orderChange: [orderBy: string];
}>();

const loading = ref<boolean>(false);
const programList = ref<ProgramDto[]>([]);
const totalCount = ref<number>(0);

const loadData = async () => {
  if (!props.menuId) {
    return;
  }
  
  loading.value = true;
  try {
    const response = await getSeqList({
      menuId: props.menuId,
      orderBy: props.orderBy
    });
    
    programList.value = response.data.items;
    totalCount.value = response.data.totalCount;
  } catch (error) {
    console.error('查詢作業列表失敗:', error);
    ElMessage.error('查詢作業列表失敗');
  } finally {
    loading.value = false;
  }
};

const handleSort = (newOrderBy: string) => {
  emit('orderChange', newOrderBy);
};

onMounted(() => {
  loadData();
});

watch(() => [props.menuId, props.orderBy], () => {
  loadData();
}, { immediate: false });
</script>

<style scoped>
.total-info {
  margin-top: 10px;
  padding: 10px;
  background-color: #f5f5f5;
  border-radius: 4px;
  text-align: center;
}

.active {
  color: #409eff;
  font-weight: bold;
}
</style>
```

### 4.3 API 呼叫 (`ewSeqList.api.ts`)
```typescript
import request from '@/utils/request';

export interface ProgramDto {
  progId: string;
  progName: string;
  seqNo?: number;
  menuId?: string;
  menuName?: string;
}

export interface SeqListResultDto {
  menuId: string;
  menuName: string;
  items: ProgramDto[];
  totalCount: number;
}

export interface SeqListQueryDto {
  menuId: string;
  orderBy?: string; // PROG_ID, PROG_NAME, SEQ_NO
  filter?: string;
}

// API 函數
export const getSeqList = (query: SeqListQueryDto) => {
  return request.get<ApiResponse<SeqListResultDto>>('/api/v1/ew/seq-list', { params: query });
};

export const getProgram = (progId: string) => {
  return request.get<ApiResponse<ProgramDto>>(`/api/v1/ew/seq-list/${progId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（使用現有的Programs表）
- [ ] 確認索引
- [ ] 確認外鍵關係

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] Repository 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 作業列表頁面開發
- [ ] 資料表格開發
- [ ] 排序功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 排序功能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 5天

---

## 六、注意事項

### 6.1 業務邏輯
- 必須先選擇子系統項目（選單ID），否則無法查詢
- 支援按作業代碼、作業名稱、序號排序
- 只顯示啟用狀態的作業（Status = '1'）
- 顯示選單代碼和選單名稱

### 6.2 資料驗證
- 選單ID必須存在
- 排序欄位必須在允許範圍內（PROG_ID, PROG_NAME, SEQ_NO）

### 6.3 效能
- 查詢結果可能較多，但通常不會超過100筆
- 必須建立適當的索引（MenuId, SeqNo）

### 6.4 UI/UX
- 支援點擊表頭進行排序
- 顯示當前排序欄位（高亮顯示）
- 顯示符合條件的總筆數
- 支援彈窗模式（關閉按鈕）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢作業列表成功（有選單ID）
- [ ] 查詢作業列表失敗（無選單ID）
- [ ] 按作業代碼排序成功
- [ ] 按作業名稱排序成功
- [ ] 按序號排序成功
- [ ] 查詢單筆作業成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 排序功能測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/EW_SEQ_LIST.asp` - 序列列表主程式

### 8.2 資料庫 Schema
- 舊系統資料表：`MNG_PROG` (作業主檔)
- 主要欄位：PROG_ID, PROG_NAME, MENU_ID, SEQ_NO
- 查詢條件：WHERE MENU_ID = @MenuId ORDER BY @OrderBy

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

