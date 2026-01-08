# EW_MENU_LIST - 選單列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: EW_MENU_LIST
- **功能名稱**: 選單列表
- **功能描述**: 提供子系統項目列表選擇功能，用於精靈或表單中的選單選擇，支援選單查詢、篩選、選擇等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/EW_MENU_LIST.asp` (選單列表主程式)

### 1.2 業務需求
- 提供子系統項目列表查詢功能
- 支援選單代碼、選單名稱篩選
- 支援選單選擇並回傳選單ID和名稱
- 支援系統ID篩選
- 支援彈窗模式選擇
- 與選單主檔（MNG_MENU）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Menus` (選單主檔，對應舊系統 `MNG_MENU`)

```sql
CREATE TABLE [dbo].[Menus] (
    [MenuId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 選單ID (MENU_ID)
    [MenuName] NVARCHAR(100) NOT NULL, -- 選單名稱 (MENU_NAME)
    [SystemId] NVARCHAR(50) NULL, -- 系統ID (SYS_ID)
    [ParentMenuId] NVARCHAR(50) NULL, -- 父選單ID (PARENT_MENU_ID)
    [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
    [Icon] NVARCHAR(100) NULL, -- 圖示 (ICON)
    [Url] NVARCHAR(500) NULL, -- 連結網址 (URL)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED ([MenuId] ASC),
    CONSTRAINT [FK_Menus_Systems] FOREIGN KEY ([SystemId]) REFERENCES [dbo].[Systems] ([SystemId]),
    CONSTRAINT [FK_Menus_ParentMenu] FOREIGN KEY ([ParentMenuId]) REFERENCES [dbo].[Menus] ([MenuId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Menus_MenuName] ON [dbo].[Menus] ([MenuName]);
CREATE NONCLUSTERED INDEX [IX_Menus_SystemId] ON [dbo].[Menus] ([SystemId]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| MenuId | NVARCHAR | 50 | NO | - | 選單ID | 主鍵 |
| MenuName | NVARCHAR | 100 | NO | - | 選單名稱 | - |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至Systems |
| ParentMenuId | NVARCHAR | 50 | YES | - | 父選單ID | 外鍵至Menus |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢選單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/menu-list`
- **說明**: 查詢選單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "systemId": "",
    "menuId": "",
    "menuName": "",
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
      "items": [
        {
          "menuId": "MENU001",
          "menuName": "選單名稱",
          "systemId": "SYS001",
          "systemName": "系統名稱"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆選單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/menu-list/{menuId}`
- **說明**: 根據選單ID查詢單筆選單資料
- **回應格式**: 標準單筆資料回應

### 3.2 後端實作類別

#### 3.2.1 Controller: `EwMenuListController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ew/menu-list")]
    [Authorize]
    public class EwMenuListController : ControllerBase
    {
        private readonly IEwMenuListService _menuListService;
        
        public EwMenuListController(IEwMenuListService menuListService)
        {
            _menuListService = menuListService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<MenuDto>>>> GetMenuList([FromQuery] MenuListQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{menuId}")]
        public async Task<ActionResult<ApiResponse<MenuDto>>> GetMenu(string menuId)
        {
            // 實作查詢單筆邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 選單列表選擇頁面 (`EwMenuList.vue`)
- **路徑**: `/ew/menu-list`
- **功能**: 顯示選單列表，支援查詢、篩選、選擇
- **主要元件**:
  - 查詢表單 (MenuListSearchForm)
  - 資料表格 (MenuListDataTable)
  - 選擇按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`MenuListSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="系統ID">
      <el-input v-model="searchForm.systemId" placeholder="請輸入系統ID" />
    </el-form-item>
    <el-form-item label="選單代碼">
      <el-input v-model="searchForm.menuId" placeholder="請輸入選單代碼" />
    </el-form-item>
    <el-form-item label="選單名稱">
      <el-input v-model="searchForm.menuName" placeholder="請輸入選單名稱" />
    </el-form-item>
    <el-form-item label="關鍵字">
      <el-input v-model="searchForm.filter" placeholder="請輸入關鍵字" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`MenuListDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="menuList" v-loading="loading" @row-click="handleRowClick">
      <el-table-column prop="menuId" label="選單代碼" width="150" />
      <el-table-column prop="menuName" label="選單名稱" width="200" />
      <el-table-column prop="systemId" label="系統ID" width="120" />
      <el-table-column prop="systemName" label="系統名稱" width="200" />
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

<script setup lang="ts">
const handleRowClick = (row: MenuDto) => {
  // 回傳選單ID和名稱給父視窗
  if (window.opener) {
    window.opener.document.forms[0].MENU_ID.value = row.menuId;
    window.opener.document.forms[0].MENU_NAME.value = row.menuName;
    window.close();
  }
};
</script>
```

### 4.3 API 呼叫 (`ewMenuList.api.ts`)
```typescript
import request from '@/utils/request';

export interface MenuDto {
  menuId: string;
  menuName: string;
  systemId?: string;
  systemName?: string;
}

export interface MenuListQueryDto {
  pageIndex: number;
  pageSize: number;
  systemId?: string;
  menuId?: string;
  menuName?: string;
  filter?: string;
}

// API 函數
export const getMenuList = (query: MenuListQueryDto) => {
  return request.get<ApiResponse<PagedResult<MenuDto>>>('/api/v1/ew/menu-list', { params: query });
};

export const getMenu = (menuId: string) => {
  return request.get<ApiResponse<MenuDto>>(`/api/v1/ew/menu-list/${menuId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（使用現有的Menus表）
- [ ] 確認索引

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 選單列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 彈窗選擇功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 5天

---

## 六、注意事項

### 6.1 業務邏輯
- 支援系統ID篩選
- 支援選單代碼、選單名稱模糊查詢
- 支援關鍵字搜尋（同時搜尋選單代碼和選單名稱）
- 選擇後需回傳選單ID和名稱給父視窗

### 6.2 資料驗證
- 系統ID必須存在（如果提供）
- 選單ID必須存在（如果提供）

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢選單列表成功
- [ ] 查詢選單列表（帶篩選條件）成功
- [ ] 查詢單筆選單成功
- [ ] 選單選擇功能測試

### 7.2 整合測試
- [ ] 完整選擇流程測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/EW_MENU_LIST.asp` - 選單列表主程式

### 8.2 資料庫 Schema
- 舊系統資料表：`MNG_MENU` (選單主檔)
- 主要欄位：MENU_ID, MENU_NAME, SYS_ID

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

