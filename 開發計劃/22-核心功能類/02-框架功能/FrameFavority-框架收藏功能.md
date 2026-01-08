# FrameFavority - 框架收藏功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FrameFavority
- **功能名稱**: 框架收藏功能
- **功能描述**: 提供系統框架的收藏選單功能，顯示使用者收藏的選單項目，支援3個框架切換、搜尋功能，用於快速存取常用功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameFavority.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameFavority.aspx.cs`

### 1.2 業務需求
- 顯示使用者收藏的選單項目（系統、子系統、作業）
- 支援3個框架切換（框架1、框架2、框架3）
- 支援選單搜尋功能
- 支援選單樹狀結構顯示
- 支援選單節點點擊導航
- 支援選單展開/收合
- 根據使用者權限顯示可用的選單項目
- 支援非IE瀏覽器相容性

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UserMenuFavorites` (使用者選單收藏)

參考 `FrameMenu-框架選單功能.md` 的 `UserMenuFavorites` 資料表設計

```sql
CREATE TABLE [dbo].[UserMenuFavorites] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [MenuId] NVARCHAR(50) NOT NULL, -- 選單項目代號
    [MenuType] NVARCHAR(20) NOT NULL, -- 選單類型 (SYSTEM/SUBSYSTEM/PROGRAM)
    [FrameIndex] INT NOT NULL DEFAULT 0, -- 框架索引 (0, 1, 2)
    [SortOrder] INT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_UserMenuFavorites_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserMenuFavorites_UserId] ON [dbo].[UserMenuFavorites] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserMenuFavorites_MenuId] ON [dbo].[UserMenuFavorites] ([MenuId]);
CREATE NONCLUSTERED INDEX [IX_UserMenuFavorites_FrameIndex] ON [dbo].[UserMenuFavorites] ([FrameIndex]);
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserMenuFavorites_UserId_MenuId_FrameIndex] ON [dbo].[UserMenuFavorites] ([UserId], [MenuId], [FrameIndex]);
```

### 2.2 資料字典

#### UserMenuFavorites 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users表 |
| MenuId | NVARCHAR | 50 | NO | - | 選單項目代號 | - |
| MenuType | NVARCHAR | 20 | NO | - | 選單類型 | SYSTEM, SUBSYSTEM, PROGRAM |
| FrameIndex | INT | - | NO | 0 | 框架索引 | 0, 1, 2 |
| SortOrder | INT | - | NO | 0 | 排序順序 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | YES | - | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢收藏選單列表

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/menu-favorites`
- **說明**: 查詢當前使用者的收藏選單列表，支援框架索引篩選
- **請求參數**:
  - `frameIndex`: 框架索引 (0, 1, 2)
  - `searchKeyword`: 搜尋關鍵字
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "menuId": "SYS001",
        "menuName": "系統管理",
        "menuType": "SYSTEM",
        "frameIndex": 0,
        "sortOrder": 1,
        "url": "/sys0000",
        "children": [
          {
            "menuId": "SYS001001",
            "menuName": "使用者管理",
            "menuType": "SUBSYSTEM",
            "url": "/sys0000/user"
          }
        ]
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 新增收藏選單

- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/menu-favorites`
- **說明**: 新增收藏選單項目
- **請求格式**:
  ```json
  {
    "menuId": "SYS001",
    "menuType": "SYSTEM",
    "frameIndex": 0,
    "sortOrder": 1
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "id": 1,
      "menuId": "SYS001",
      "menuType": "SYSTEM",
      "frameIndex": 0,
      "sortOrder": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 刪除收藏選單

- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/menu-favorites/{id}`
- **說明**: 刪除收藏選單項目
- **路徑參數**:
  - `id`: 收藏記錄ID
- **回應格式**: 標準回應

#### 3.1.4 更新收藏選單排序

- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/menu-favorites/sort`
- **說明**: 更新收藏選單的排序順序
- **請求格式**:
  ```json
  {
    "favorites": [
      { "id": 1, "sortOrder": 1 },
      { "id": 2, "sortOrder": 2 }
    ]
  }
  ```
- **回應格式**: 標準回應

#### 3.1.5 切換框架

- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/menu-favorites/switch-frame`
- **說明**: 切換收藏選單的框架索引
- **請求格式**:
  ```json
  {
    "favoriteId": 1,
    "frameIndex": 1
  }
  ```
- **回應格式**: 標準回應

### 3.2 後端實作類別

#### 3.2.1 Controller: `MenuFavoritesController.cs`

```csharp
[ApiController]
[Route("api/v1/menu-favorites")]
[Authorize]
public class MenuFavoritesController : ControllerBase
{
    private readonly IMenuFavoriteService _menuFavoriteService;

    public MenuFavoritesController(IMenuFavoriteService menuFavoriteService)
    {
        _menuFavoriteService = menuFavoriteService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MenuFavoriteDto>>>> GetFavorites(
        [FromQuery] int? frameIndex,
        [FromQuery] string searchKeyword)
    {
        var result = await _menuFavoriteService.GetFavoritesAsync(frameIndex, searchKeyword);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuFavoriteDto>>> AddFavorite(
        [FromBody] CreateMenuFavoriteDto dto)
    {
        var result = await _menuFavoriteService.AddFavoriteAsync(dto);
        return Ok(ApiResponse.Success(result));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> RemoveFavorite(long id)
    {
        await _menuFavoriteService.RemoveFavoriteAsync(id);
        return Ok(ApiResponse.Success());
    }

    [HttpPut("sort")]
    public async Task<ActionResult<ApiResponse>> UpdateSort([FromBody] UpdateSortDto dto)
    {
        await _menuFavoriteService.UpdateSortAsync(dto.Favorites);
        return Ok(ApiResponse.Success());
    }

    [HttpPost("switch-frame")]
    public async Task<ActionResult<ApiResponse>> SwitchFrame([FromBody] SwitchFrameDto dto)
    {
        await _menuFavoriteService.SwitchFrameAsync(dto.FavoriteId, dto.FrameIndex);
        return Ok(ApiResponse.Success());
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 收藏選單頁面 (`FavoriteFrame.vue`)

```vue
<template>
  <div class="favorite-frame">
    <!-- 框架切換按鈕 -->
    <div class="frame-switcher" v-if="!isNotIE">
      <el-button
        v-for="index in 3"
        :key="index"
        :type="currentFrameIndex === index - 1 ? 'primary' : 'default'"
        @click="handleSwitchFrame(index - 1)"
        size="small"
      >
        {{ index }}
      </el-button>
    </div>

    <!-- 搜尋框 -->
    <div class="search-section">
      <el-input
        v-model="searchKeyword"
        :placeholder="$t('favorite.searchPlaceholder')"
        clearable
        @input="handleSearch"
        size="small"
      />
    </div>

    <!-- 選單樹 -->
    <div class="menu-tree">
      <el-tree
        :data="menuTree"
        :props="treeProps"
        :filter-node-method="filterNode"
        :expand-on-click-node="false"
        node-key="menuId"
        default-expand-all
        @node-click="handleNodeClick"
        ref="treeRef"
      >
        <template #default="{ node, data }">
          <span class="tree-node">
            <i :class="getMenuIcon(data.menuType)"></i>
            <span>{{ data.menuName }}</span>
          </span>
        </template>
      </el-tree>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { getFavorites, addFavorite, removeFavorite, switchFrame } from '@/api/menu-favorite.api';
import type { MenuFavoriteDto } from '@/types/menu-favorite';

const { t } = useI18n();
const router = useRouter();

const currentFrameIndex = ref<number>(0);
const searchKeyword = ref<string>('');
const menuTree = ref<any[]>([]);
const treeRef = ref();
const isNotIE = ref<boolean>(false);

const treeProps = {
  children: 'children',
  label: 'menuName'
};

const loadFavorites = async () => {
  try {
    const response = await getFavorites(currentFrameIndex.value, searchKeyword.value);
    menuTree.value = buildMenuTree(response.data);
    nextTick(() => {
      if (treeRef.value) {
        treeRef.value.filter(searchKeyword.value);
      }
    });
  } catch (error) {
    ElMessage.error(t('favorite.loadError'));
  }
};

const buildMenuTree = (favorites: MenuFavoriteDto[]): any[] => {
  // 建立選單樹狀結構
  const tree: any[] = [];
  const map = new Map<string, any>();

  favorites.forEach(fav => {
    const node = {
      menuId: fav.menuId,
      menuName: fav.menuName,
      menuType: fav.menuType,
      url: fav.url,
      children: []
    };
    map.set(fav.menuId, node);

    if (fav.parentId) {
      const parent = map.get(fav.parentId);
      if (parent) {
        parent.children.push(node);
      }
    } else {
      tree.push(node);
    }
  });

  return tree;
};

const handleSwitchFrame = async (frameIndex: number) => {
  currentFrameIndex.value = frameIndex;
  await loadFavorites();
};

const handleSearch = () => {
  if (treeRef.value) {
    treeRef.value.filter(searchKeyword.value);
  }
};

const filterNode = (value: string, data: any) => {
  if (!value) return true;
  return data.menuName.toLowerCase().includes(value.toLowerCase());
};

const handleNodeClick = (data: any) => {
  if (data.url) {
    router.push(data.url);
  }
};

const getMenuIcon = (menuType: string) => {
  switch (menuType) {
    case 'SYSTEM':
      return 'el-icon-s-home';
    case 'SUBSYSTEM':
      return 'el-icon-menu';
    case 'PROGRAM':
      return 'el-icon-document';
    default:
      return 'el-icon-folder';
  }
};

const detectBrowser = () => {
  const userAgent = navigator.userAgent;
  isNotIE.value = !userAgent.includes('MSIE') && !userAgent.includes('Trident');
};

onMounted(() => {
  detectBrowser();
  loadFavorites();
});

watch(searchKeyword, () => {
  handleSearch();
});
</script>

<style scoped lang="scss">
.favorite-frame {
  padding: 10px;
  background-color: #f5f5f5;

  .frame-switcher {
    display: flex;
    gap: 5px;
    margin-bottom: 10px;
    justify-content: center;
  }

  .search-section {
    margin-bottom: 10px;
  }

  .menu-tree {
    background: white;
    padding: 10px;
    border-radius: 4px;
    max-height: 500px;
    overflow-y: auto;

    .tree-node {
      display: flex;
      align-items: center;
      gap: 5px;
    }
  }
}
</style>
```

### 4.2 API 呼叫 (`menu-favorite.api.ts`)

```typescript
import request from '@/utils/request';
import type { ApiResponse } from '@/types/common';
import type { MenuFavoriteDto, CreateMenuFavoriteDto } from '@/types/menu-favorite';

export const getFavorites = (frameIndex?: number, searchKeyword?: string) => {
  return request.get<ApiResponse<MenuFavoriteDto[]>>('/api/v1/menu-favorites', {
    params: { frameIndex, searchKeyword }
  });
};

export const addFavorite = (dto: CreateMenuFavoriteDto) => {
  return request.post<ApiResponse<MenuFavoriteDto>>('/api/v1/menu-favorites', dto);
};

export const removeFavorite = (id: number) => {
  return request.delete<ApiResponse>(`/api/v1/menu-favorites/${id}`);
};

export const updateSort = (favorites: Array<{ id: number; sortOrder: number }>) => {
  return request.put<ApiResponse>('/api/v1/menu-favorites/sort', { favorites });
};

export const switchFrame = (favoriteId: number, frameIndex: number) => {
  return request.post<ApiResponse>('/api/v1/menu-favorites/switch-frame', {
    favoriteId,
    frameIndex
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 UserMenuFavorites 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
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
- [ ] 收藏選單頁面開發
- [ ] 選單樹元件開發
- [ ] 框架切換功能開發
- [ ] 搜尋功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 5.5天

---

## 六、注意事項

### 6.1 業務邏輯
- 收藏選單需根據使用者權限過濾
- 每個框架最多顯示使用者有權限的選單項目
- 選單項目不可重複收藏到同一框架
- 框架切換需即時更新顯示內容

### 6.2 資料驗證
- 選單項目必須存在
- 使用者必須對該選單項目有權限
- 框架索引必須為 0, 1, 2

### 6.3 效能
- 選單樹需使用懶加載
- 搜尋功能需使用防抖機制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢收藏選單列表成功
- [ ] 新增收藏選單成功
- [ ] 刪除收藏選單成功
- [ ] 更新排序成功
- [ ] 切換框架成功

### 7.2 整合測試
- [ ] 完整收藏選單流程測試
- [ ] 框架切換功能測試
- [ ] 搜尋功能測試
- [ ] 權限驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/FrameFavority.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/FrameFavority.aspx.cs`
- `IMS3/HANSHIN/RSL_CLASS/KERNEL/KernelClass.cs` (GenUserProfileTreeNode 方法)

### 8.2 相關功能
- `FrameMenu-框架選單功能.md` - 框架選單功能
- `FrameMainMenu-框架主選單功能.md` - 框架主選單功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

