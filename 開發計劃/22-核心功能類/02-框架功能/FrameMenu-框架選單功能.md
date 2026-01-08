# FrameMenu - 框架選單功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FrameMenu
- **功能名稱**: 框架選單功能
- **功能描述**: 提供系統框架的左側選單功能，包含系統選單樹狀結構顯示、搜尋功能、多框架切換（支援3個框架切換）
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMenu.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameMenu.aspx.cs`

### 1.2 業務需求
- 顯示系統選單樹狀結構
- 支援選單搜尋功能
- 支援多框架切換（框架1、框架2、框架3）
- 根據使用者權限顯示可用的選單項目
- 支援選單節點點擊導航
- 支援選單展開/收合
- 支援非IE瀏覽器相容性

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemMenus` (系統選單)
參考 `SYS0410-主系統項目資料維護.md`、`SYS0420-子系統項目資料維護.md`、`SYS0430-系統作業資料維護.md` 的選單資料表設計

### 2.2 相關資料表

#### 2.2.1 `UserMenuFavorites` - 使用者選單收藏
```sql
CREATE TABLE [dbo].[UserMenuFavorites] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [MenuId] NVARCHAR(50) NOT NULL, -- 選單項目代號
    [MenuType] NVARCHAR(20) NOT NULL, -- 選單類型 (SYSTEM/SUBSYSTEM/PROGRAM)
    [SortOrder] INT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserMenuFavorites_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserMenuFavorites_UserId] ON [dbo].[UserMenuFavorites] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserMenuFavorites_MenuId] ON [dbo].[UserMenuFavorites] ([MenuId]);
```

#### 2.2.2 `MenuSearchHistory` - 選單搜尋歷史
```sql
CREATE TABLE [dbo].[MenuSearchHistory] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [SearchKeyword] NVARCHAR(100) NOT NULL,
    [SearchCount] INT NOT NULL DEFAULT 1,
    [LastSearchedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_MenuSearchHistory_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MenuSearchHistory_UserId] ON [dbo].[MenuSearchHistory] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_MenuSearchHistory_Keyword] ON [dbo].[MenuSearchHistory] ([SearchKeyword]);
```

### 2.3 資料字典

#### UserMenuFavorites 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users表 |
| MenuId | NVARCHAR | 50 | NO | - | 選單項目代號 | - |
| MenuType | NVARCHAR | 20 | NO | - | 選單類型 | SYSTEM/SUBSYSTEM/PROGRAM |
| SortOrder | INT | - | NO | 0 | 排序順序 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢使用者選單樹
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/menus/tree`
- **說明**: 根據使用者權限查詢選單樹狀結構
- **請求參數**:
  - `sysId`: 系統代號（可選）
  - `searchKeyword`: 搜尋關鍵字（可選）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "menus": [
        {
          "id": "SYS001",
          "name": "系統管理",
          "type": "SYSTEM",
          "icon": "system",
          "url": null,
          "children": [
            {
              "id": "SYS001001",
              "name": "使用者管理",
              "type": "SUBSYSTEM",
              "icon": "user",
              "url": null,
              "children": [
                {
                  "id": "SYS0110",
                  "name": "使用者基本資料維護",
                  "type": "PROGRAM",
                  "icon": "edit",
                  "url": "/system/users",
                  "children": []
                }
              ]
            }
          ]
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 搜尋選單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/menus/search`
- **說明**: 根據關鍵字搜尋選單項目
- **請求參數**:
  - `keyword`: 搜尋關鍵字（必填）
  - `sysId`: 系統代號（可選）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "results": [
        {
          "id": "SYS0110",
          "name": "使用者基本資料維護",
          "type": "PROGRAM",
          "url": "/system/users",
          "path": "系統管理 > 使用者管理 > 使用者基本資料維護"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢使用者收藏選單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/menus/favorites`
- **說明**: 查詢使用者的收藏選單列表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "favorites": [
        {
          "id": "SYS0110",
          "name": "使用者基本資料維護",
          "type": "PROGRAM",
          "url": "/system/users",
          "sortOrder": 1
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 新增收藏選單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/menus/favorites`
- **說明**: 新增選單到使用者收藏
- **請求格式**:
  ```json
  {
    "menuId": "SYS0110",
    "menuType": "PROGRAM"
  }
  ```

#### 3.1.5 刪除收藏選單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/menus/favorites/{favoriteId}`
- **說明**: 從使用者收藏中移除選單
- **路徑參數**:
  - `favoriteId`: 收藏ID

#### 3.1.6 更新收藏選單排序
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/menus/favorites/sort`
- **說明**: 更新收藏選單的排序順序
- **請求格式**:
  ```json
  {
    "favorites": [
      {
        "id": 1,
        "sortOrder": 1
      },
      {
        "id": 2,
        "sortOrder": 2
      }
    ]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `MenusController.cs`
```csharp
[ApiController]
[Route("api/v1/menus")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;
    
    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<MenuTreeDto>>> GetMenuTree([FromQuery] string sysId, [FromQuery] string searchKeyword)
    {
        // 實作查詢選單樹邏輯
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<MenuSearchResultDto>>> SearchMenus([FromQuery] string keyword, [FromQuery] string sysId)
    {
        // 實作搜尋選單邏輯
    }
    
    [HttpGet("favorites")]
    public async Task<ActionResult<ApiResponse<List<MenuFavoriteDto>>>> GetFavorites()
    {
        // 實作查詢收藏選單邏輯
    }
    
    [HttpPost("favorites")]
    public async Task<ActionResult<ApiResponse<MenuFavoriteDto>>> AddFavorite([FromBody] AddMenuFavoriteRequest request)
    {
        // 實作新增收藏邏輯
    }
    
    [HttpDelete("favorites/{favoriteId}")]
    public async Task<ActionResult<ApiResponse>> RemoveFavorite(long favoriteId)
    {
        // 實作刪除收藏邏輯
    }
    
    [HttpPut("favorites/sort")]
    public async Task<ActionResult<ApiResponse>> UpdateFavoriteSort([FromBody] UpdateFavoriteSortRequest request)
    {
        // 實作更新排序邏輯
    }
}
```

#### 3.2.2 Service: `MenuService.cs`
```csharp
public interface IMenuService
{
    Task<MenuTreeDto> GetMenuTreeAsync(string userId, string sysId, string searchKeyword);
    Task<MenuSearchResultDto> SearchMenusAsync(string userId, string keyword, string sysId);
    Task<List<MenuFavoriteDto>> GetFavoritesAsync(string userId);
    Task<MenuFavoriteDto> AddFavoriteAsync(string userId, string menuId, string menuType);
    Task RemoveFavoriteAsync(string userId, long favoriteId);
    Task UpdateFavoriteSortAsync(string userId, List<FavoriteSortItem> favorites);
}
```

#### 3.2.3 Repository: `MenuRepository.cs`
```csharp
public interface IMenuRepository
{
    Task<List<SystemMenu>> GetUserMenusAsync(string userId, string sysId);
    Task<List<SystemMenu>> SearchMenusAsync(string userId, string keyword, string sysId);
    Task<List<UserMenuFavorite>> GetUserFavoritesAsync(string userId);
    Task<UserMenuFavorite> AddFavoriteAsync(string userId, string menuId, string menuType);
    Task RemoveFavoriteAsync(string userId, long favoriteId);
    Task UpdateFavoriteSortAsync(string userId, List<FavoriteSortItem> favorites);
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 框架選單頁面 (`FrameMenu.vue`)
```vue
<template>
  <div class="frame-menu">
    <!-- 框架切換按鈕（僅IE顯示） -->
    <div v-if="!isNotIE" class="frame-switcher">
      <el-button 
        v-for="(frame, index) in frames" 
        :key="index"
        :type="currentFrame === index ? 'primary' : 'default'"
        @click="switchFrame(index)"
      >
        {{ index + 1 }}
      </el-button>
    </div>
    
    <!-- 搜尋區塊 -->
    <div class="search-section">
      <el-input
        v-model="searchKeyword"
        placeholder="搜尋選單"
        clearable
        @keyup.enter="handleSearch"
      >
        <template #suffix>
          <el-icon @click="handleSearch"><Search /></el-icon>
        </template>
      </el-input>
    </div>
    
    <!-- 選單樹狀結構 -->
    <div class="menu-tree">
      <el-tree
        ref="menuTreeRef"
        :data="menuTree"
        :props="treeProps"
        :default-expand-all="false"
        :highlight-current="true"
        node-key="id"
        @node-click="handleNodeClick"
      >
        <template #default="{ node, data }">
          <span class="tree-node">
            <el-icon v-if="data.icon"><component :is="data.icon" /></el-icon>
            <span>{{ data.name }}</span>
          </span>
        </template>
      </el-tree>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useMenuStore } from '@/stores/menu';
import { getMenuTree, searchMenus } from '@/api/menu.api';

const router = useRouter();
const menuStore = useMenuStore();

const isNotIE = ref(false);
const currentFrame = ref(0);
const frames = [0, 1, 2];
const searchKeyword = ref('');
const menuTree = ref([]);
const menuTreeRef = ref();

const treeProps = {
  children: 'children',
  label: 'name'
};

// 檢測是否為非IE瀏覽器
const checkBrowser = () => {
  const userAgent = navigator.userAgent;
  isNotIE.value = !userAgent.includes('MSIE') && !userAgent.includes('Trident');
};

// 切換框架
const switchFrame = (index: number) => {
  currentFrame.value = index;
  // 觸發父框架切換事件
  window.parent.postMessage({ type: 'CHANGE_FRAME', frameIndex: index }, '*');
};

// 載入選單樹
const loadMenuTree = async () => {
  try {
    const response = await getMenuTree({ searchKeyword: searchKeyword.value });
    menuTree.value = response.data.menus;
  } catch (error) {
    console.error('載入選單失敗:', error);
  }
};

// 搜尋選單
const handleSearch = async () => {
  if (!searchKeyword.value.trim()) {
    await loadMenuTree();
    return;
  }
  
  try {
    const response = await searchMenus({ keyword: searchKeyword.value });
    // 處理搜尋結果
    // 可以顯示搜尋結果列表或高亮匹配的節點
  } catch (error) {
    console.error('搜尋選單失敗:', error);
  }
};

// 點擊選單節點
const handleNodeClick = (data: any) => {
  if (data.type === 'PROGRAM' && data.url) {
    // 導航到對應頁面
    router.push(data.url);
    // 或使用 iframe 載入
    window.parent.postMessage({ type: 'LOAD_PAGE', url: data.url }, '*');
  }
};

onMounted(() => {
  checkBrowser();
  loadMenuTree();
});
</script>

<style scoped>
.frame-menu {
  padding: 10px;
  background-color: #91ACBD;
}

.frame-switcher {
  display: flex;
  justify-content: center;
  gap: 5px;
  margin-bottom: 10px;
}

.search-section {
  margin-bottom: 10px;
}

.menu-tree {
  max-height: calc(100vh - 150px);
  overflow-y: auto;
}

.tree-node {
  display: flex;
  align-items: center;
  gap: 5px;
}
</style>
```

### 4.2 API 呼叫 (`menu.api.ts`)
```typescript
import request from '@/utils/request';
import type { ApiResponse } from '@/types/api';

export interface MenuTreeDto {
  menus: MenuNode[];
}

export interface MenuNode {
  id: string;
  name: string;
  type: 'SYSTEM' | 'SUBSYSTEM' | 'PROGRAM';
  icon?: string;
  url?: string;
  children: MenuNode[];
}

export const getMenuTree = (params?: { sysId?: string; searchKeyword?: string }) => {
  return request.get<ApiResponse<MenuTreeDto>>('/api/v1/menus/tree', { params });
};

export const searchMenus = (params: { keyword: string; sysId?: string }) => {
  return request.get<ApiResponse<MenuSearchResultDto>>('/api/v1/menus/search', { params });
};

export const getFavorites = () => {
  return request.get<ApiResponse<MenuFavoriteDto[]>>('/api/v1/menus/favorites');
};

export const addFavorite = (data: { menuId: string; menuType: string }) => {
  return request.post<ApiResponse<MenuFavoriteDto>>('/api/v1/menus/favorites', data);
};

export const removeFavorite = (favoriteId: number) => {
  return request.delete<ApiResponse>(`/api/v1/menus/favorites/${favoriteId}`);
};

export const updateFavoriteSort = (data: { favorites: Array<{ id: number; sortOrder: number }> }) => {
  return request.put<ApiResponse>('/api/v1/menus/favorites/sort', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 UserMenuFavorites 資料表
- [ ] 建立 MenuSearchHistory 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 權限驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 選單樹組件開發
- [ ] 搜尋功能開發
- [ ] 框架切換功能開發
- [ ] 選單導航功能開發
- [ ] 瀏覽器相容性處理
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 權限測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- 選單樹根據使用者權限動態生成
- 支援多系統切換（透過 sysId 參數）
- 搜尋功能支援模糊匹配
- 框架切換功能僅在IE瀏覽器顯示
- 選單節點點擊後根據類型進行不同處理（PROGRAM類型導航到對應頁面）

### 6.2 資料驗證
- 選單ID必須存在
- 收藏選單不能重複
- 排序順序必須為正整數

### 6.3 效能
- 選單樹資料需使用快取機制
- 大量選單項目需使用虛擬滾動
- 搜尋結果需限制數量

### 6.4 安全性
- 選單權限驗證必須在後端進行
- 使用者只能看到有權限的選單項目
- 收藏選單需驗證使用者身份

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢選單樹成功
- [ ] 搜尋選單成功
- [ ] 新增收藏選單成功
- [ ] 刪除收藏選單成功
- [ ] 更新收藏選單排序成功
- [ ] 權限驗證正確

### 7.2 整合測試
- [ ] 完整選單載入流程測試
- [ ] 選單搜尋流程測試
- [ ] 選單導航流程測試
- [ ] 框架切換流程測試
- [ ] 權限控制測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMenu.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/FrameMenu.aspx.cs`

### 8.2 相關功能
- `SYS0410-主系統項目資料維護.md` - 主系統選單維護
- `SYS0420-子系統項目資料維護.md` - 子系統選單維護
- `SYS0430-系統作業資料維護.md` - 系統作業選單維護
- `FrameFavority-框架收藏功能.md` - 選單收藏功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

