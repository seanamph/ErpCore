# FrameMainMenu - 框架主選單功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FrameMainMenu
- **功能名稱**: 框架主選單功能
- **功能描述**: 提供系統框架的主選單功能，顯示系統模組圖示按鈕（每行3個），點擊後顯示該模組的子系統列表，支援多模組切換
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMainMenu.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMainMenu.aspx.cs`

### 1.2 業務需求
- 顯示使用者有權限的系統模組圖示按鈕
- 每行顯示3個模組按鈕
- 點擊模組按鈕後，顯示該模組的子系統列表
- 支援模組按鈕的選中/未選中狀態切換（ON/OFF圖片切換）
- 無權限時顯示提示訊息
- 支援頁面轉場效果設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Systems` (系統主檔)
參考 `SYS0410-主系統項目資料維護.md` 的 `Systems` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `SystemModules` - 系統模組主檔
```sql
CREATE TABLE [dbo].[SystemModules] (
    [ModuleId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [ModuleName] NVARCHAR(100) NOT NULL,
    [ModuleNameEn] NVARCHAR(100) NULL,
    [ImgOnUrl] NVARCHAR(255) NULL, -- 選中狀態圖片路徑
    [ImgOffUrl] NVARCHAR(255) NULL, -- 未選中狀態圖片路徑
    [SortOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NULL
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemModules_SortOrder] ON [dbo].[SystemModules] ([SortOrder]);
CREATE NONCLUSTERED INDEX [IX_SystemModules_IsActive] ON [dbo].[SystemModules] ([IsActive]);
```

#### 2.2.2 `UserModulePermissions` - 使用者模組權限
```sql
CREATE TABLE [dbo].[UserModulePermissions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [ModuleId] NVARCHAR(50) NOT NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserModulePermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModulePermissions_SystemModules] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[SystemModules] ([ModuleId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserModulePermissions_UserId] ON [dbo].[UserModulePermissions] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserModulePermissions_ModuleId] ON [dbo].[UserModulePermissions] ([ModuleId]);
```

### 2.3 資料字典

#### SystemModules 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ModuleId | NVARCHAR | 50 | NO | - | 模組代號 | 主鍵 |
| ModuleName | NVARCHAR | 100 | NO | - | 模組名稱 | - |
| ModuleNameEn | NVARCHAR | 100 | YES | - | 模組英文名稱 | - |
| ImgOnUrl | NVARCHAR | 255 | YES | - | 選中狀態圖片路徑 | - |
| ImgOffUrl | NVARCHAR | 255 | YES | - | 未選中狀態圖片路徑 | - |
| SortOrder | INT | - | NO | 0 | 排序順序 | - |
| IsActive | BIT | - | NO | 1 | 是否啟用 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | YES | - | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢使用者可用的模組列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/frame/main-menu/modules`
- **說明**: 根據使用者權限查詢可用的系統模組列表
- **查詢參數**:
  - `userId` (string, required): 使用者編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "moduleId": "SYS0000",
        "moduleName": "系統管理",
        "moduleNameEn": "System Management",
        "imgOnUrl": "sys0000_on.jpg",
        "imgOffUrl": "sys0000_off.jpg",
        "sortOrder": 1
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢模組的子系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/frame/main-menu/modules/{moduleId}/subsystems`
- **說明**: 根據模組代號查詢子系統列表
- **路徑參數**:
  - `moduleId` (string, required): 模組代號
- **查詢參數**:
  - `userId` (string, required): 使用者編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "systemId": "SYS0100",
        "systemName": "使用者管理",
        "systemUrl": "/sys0100/users",
        "sortOrder": 1
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `FrameMainMenuController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/frame/main-menu")]
    [Authorize]
    public class FrameMainMenuController : ControllerBase
    {
        private readonly IMainMenuService _mainMenuService;
        
        public FrameMainMenuController(IMainMenuService mainMenuService)
        {
            _mainMenuService = mainMenuService;
        }
        
        [HttpGet("modules")]
        public async Task<ActionResult<ApiResponse<List<ModuleDto>>>> GetUserModules([FromQuery] string userId)
        {
            var modules = await _mainMenuService.GetUserModulesAsync(userId);
            return Ok(ApiResponse.Success(modules));
        }
        
        [HttpGet("modules/{moduleId}/subsystems")]
        public async Task<ActionResult<ApiResponse<List<SubsystemDto>>>> GetModuleSubsystems(
            string moduleId, 
            [FromQuery] string userId)
        {
            var subsystems = await _mainMenuService.GetModuleSubsystemsAsync(moduleId, userId);
            return Ok(ApiResponse.Success(subsystems));
        }
    }
}
```

#### 3.2.2 Service: `MainMenuService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IMainMenuService
    {
        Task<List<ModuleDto>> GetUserModulesAsync(string userId);
        Task<List<SubsystemDto>> GetModuleSubsystemsAsync(string moduleId, string userId);
    }
    
    public class MainMenuService : IMainMenuService
    {
        private readonly IMainMenuRepository _repository;
        
        public MainMenuService(IMainMenuRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<List<ModuleDto>> GetUserModulesAsync(string userId)
        {
            // 實作查詢使用者可用模組邏輯
        }
        
        public async Task<List<SubsystemDto>> GetModuleSubsystemsAsync(string moduleId, string userId)
        {
            // 實作查詢模組子系統邏輯
        }
    }
}
```

#### 3.2.3 Repository: `MainMenuRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IMainMenuRepository
    {
        Task<List<SystemModule>> GetUserModulesAsync(string userId);
        Task<List<System>> GetModuleSubsystemsAsync(string moduleId, string userId);
    }
    
    public class MainMenuRepository : IMainMenuRepository
    {
        private readonly IDbConnection _connection;
        
        public MainMenuRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public async Task<List<SystemModule>> GetUserModulesAsync(string userId)
        {
            // 使用 Dapper 查詢
        }
        
        public async Task<List<System>> GetModuleSubsystemsAsync(string moduleId, string userId)
        {
            // 使用 Dapper 查詢
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 主選單頁面 (`FrameMainMenu.vue`)
```vue
<template>
  <div class="frame-main-menu">
    <!-- 模組按鈕區域 -->
    <div class="module-buttons" v-if="modules.length > 0">
      <div 
        v-for="(row, rowIndex) in moduleRows" 
        :key="rowIndex" 
        class="module-row"
      >
        <el-image
          v-for="module in row"
          :key="module.moduleId"
          :src="getModuleImageUrl(module)"
          :class="['module-button', { active: selectedModuleId === module.moduleId }]"
          @click="handleModuleClick(module)"
          fit="contain"
        />
      </div>
    </div>
    
    <!-- 無權限提示 -->
    <div v-else class="no-permission">
      <el-alert
        type="warning"
        :closable="false"
        show-icon
      >
        <template #title>
          <span>您並無任何權限！請洽系統管理者。</span>
        </template>
      </el-alert>
    </div>
    
    <!-- 子系統列表區域 -->
    <div v-if="selectedModuleId" class="subsystem-list">
      <el-table
        :data="subsystems"
        border
        stripe
        @row-click="handleSubsystemClick"
      >
        <el-table-column prop="systemName" label="子系統名稱" />
      </el-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useUserStore } from '@/stores/user';
import { getModules, getSubsystems } from '@/api/frame/main-menu';

const userStore = useUserStore();
const modules = ref<ModuleDto[]>([]);
const subsystems = ref<SubsystemDto[]>([]);
const selectedModuleId = ref<string>('');

// 計算模組按鈕行數（每行3個）
const moduleRows = computed(() => {
  const rows: ModuleDto[][] = [];
  for (let i = 0; i < modules.value.length; i += 3) {
    rows.push(modules.value.slice(i, i + 3));
  }
  return rows;
});

// 取得模組圖片URL
const getModuleImageUrl = (module: ModuleDto) => {
  const imagePath = userStore.imagePath || '';
  const imageName = selectedModuleId.value === module.moduleId 
    ? module.imgOnUrl 
    : module.imgOffUrl;
  return `/images/${imagePath}${imageName}`;
};

// 處理模組點擊
const handleModuleClick = async (module: ModuleDto) => {
  selectedModuleId.value = module.moduleId;
  await loadSubsystems(module.moduleId);
};

// 載入子系統列表
const loadSubsystems = async (moduleId: string) => {
  try {
    const response = await getSubsystems(moduleId, userStore.userId);
    subsystems.value = response.data;
  } catch (error) {
    console.error('載入子系統列表失敗:', error);
  }
};

// 處理子系統點擊
const handleSubsystemClick = (row: SubsystemDto) => {
  // 導航到子系統頁面
  window.parent.frames['rightFrame'].location.href = row.systemUrl;
};

// 初始化載入
onMounted(async () => {
  try {
    const response = await getModules(userStore.userId);
    modules.value = response.data;
  } catch (error) {
    console.error('載入模組列表失敗:', error);
  }
});
</script>

<style scoped>
.frame-main-menu {
  padding: 10px;
  background-color: #91ACBD;
}

.module-buttons {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.module-row {
  display: flex;
  gap: 10px;
}

.module-button {
  width: 110px;
  height: 50px;
  cursor: pointer;
  transition: opacity 0.3s;
}

.module-button:hover {
  opacity: 0.8;
}

.module-button.active {
  opacity: 1;
}

.no-permission {
  padding: 20px;
}

.subsystem-list {
  margin-top: 20px;
  background-color: #E6E6E6;
  padding: 10px;
}
</style>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 SystemModules 資料表
- [ ] 建立 UserModulePermissions 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 權限驗證邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 主選單頁面開發
- [ ] 模組按鈕顯示邏輯
- [ ] 子系統列表顯示
- [ ] 圖片切換邏輯
- [ ] 無權限提示
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 權限驗證測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 權限控制
- 需根據使用者權限過濾可顯示的模組
- 無權限時顯示提示訊息
- 子系統列表也需根據權限過濾

### 6.2 圖片管理
- 模組圖片需支援 ON/OFF 兩種狀態
- 圖片路徑需支援多語言版本（IMAGE_PATH）
- 圖片需預先準備並上傳到伺服器

### 6.3 效能優化
- 模組列表可快取（Redis）
- 子系統列表按需載入
- 圖片使用 CDN 加速

### 6.4 相容性
- 需支援多框架切換
- 需支援非IE瀏覽器
- 需支援響應式設計

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者模組列表成功
- [ ] 查詢模組子系統列表成功
- [ ] 無權限時返回空列表
- [ ] 權限驗證邏輯正確

### 7.2 整合測試
- [ ] 完整模組切換流程測試
- [ ] 子系統導航測試
- [ ] 權限驗證流程測試
- [ ] 圖片切換測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMainMenu.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMainMenu.aspx.cs`

### 8.2 相關功能
- `FrameMenu-框架選單功能.md` - 左側選單功能
- `SYS0410-主系統項目資料維護.md` - 系統主檔維護

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

