# MENU_LIST - 選單列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MENU_LIST
- **功能名稱**: 選單列表
- **功能描述**: 提供系統選單選擇的下拉列表功能，支援選單查詢、篩選、選擇等功能，用於系統選單設定時的選單選擇
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MENU_LIST.aspx` (ASP.NET版本)
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MENU_LIST.aspx.cs` (業務邏輯)

### 1.2 業務需求
- 提供選單列表查詢功能
- 支援選單名稱篩選
- 支援選單選擇並回傳選單ID和名稱
- 支援系統ID篩選
- 支援狀態篩選（啟用/停用）
- 支援多選模式（可選）
- 支援排序功能
- 與系統選單主檔（MNG_MENU）整合

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
CREATE NONCLUSTERED INDEX [IX_Menus_ParentMenuId] ON [dbo].[Menus] ([ParentMenuId]);
CREATE NONCLUSTERED INDEX [IX_Menus_Status] ON [dbo].[Menus] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Menus_SeqNo] ON [dbo].[Menus] ([SeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `Systems` - 系統主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0410-主系統項目資料維護.md`

#### 2.2.2 `Programs` - 作業主檔
- 用於查詢選單下的作業

### 2.3 資料字典

#### Menus 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| MenuId | NVARCHAR | 50 | NO | - | 選單ID | 主鍵 |
| MenuName | NVARCHAR | 100 | NO | - | 選單名稱 | - |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至Systems表 |
| ParentMenuId | NVARCHAR | 50 | YES | - | 父選單ID | 外鍵至Menus表 |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| Icon | NVARCHAR | 100 | YES | - | 圖示 | - |
| Url | NVARCHAR | 500 | YES | - | 連結網址 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢選單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/menus`
- **說明**: 查詢選單列表，支援篩選、排序、分頁
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 50,
    "sortField": "MenuName",
    "sortOrder": "ASC",
    "filters": {
      "menuName": "",
      "systemId": "",
      "status": "1"
    }
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
          "menuName": "系統管理",
          "systemId": "SYS0000",
          "parentMenuId": null,
          "seqNo": 1,
          "icon": "system",
          "url": "/system",
          "status": "1"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 50,
      "totalPages": 2
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆選單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/menus/{menuId}`
- **說明**: 根據選單ID查詢單筆選單資料
- **路徑參數**:
  - `menuId`: 選單ID
- **回應格式**: 同查詢選單列表單筆資料

#### 3.1.3 查詢選單選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/menus/options`
- **說明**: 取得選單選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `systemId`: 系統ID（可選）
  - `status`: 狀態（預設為'1'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "MENU001",
        "label": "系統管理"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Entity: `Menu.cs`
```csharp
namespace RSL.IMS3.Domain.Entities
{
    public class Menu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string SystemId { get; set; }
        public string ParentMenuId { get; set; }
        public int? SeqNo { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

#### 3.2.2 Repository: `IMenuRepository.cs`
```csharp
namespace RSL.IMS3.Domain.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetMenusAsync(MenuQuery query);
        Task<Menu> GetMenuByIdAsync(string menuId);
        Task<IEnumerable<MenuOption>> GetMenuOptionsAsync(string systemId = null, string status = "1");
    }
}
```

#### 3.2.3 Service: `MenuService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IMenuService
    {
        Task<PagedResult<MenuDto>> GetMenusAsync(MenuQueryDto query);
        Task<MenuDto> GetMenuByIdAsync(string menuId);
        Task<IEnumerable<MenuOptionDto>> GetMenuOptionsAsync(string systemId = null, string status = "1");
    }
    
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        // 實作選單相關邏輯
    }
}
```

#### 3.2.4 Controller: `MenuListController.cs`
```csharp
namespace RSL.IMS3.Backend.Controllers
{
    [ApiController]
    [Route("api/v1/lists/menus")]
    public class MenuListController : ControllerBase
    {
        private readonly IMenuService _menuService;
        // 實作API端點
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 選單選擇對話框組件 (`MenuListDialog.vue`)
```vue
<template>
  <el-dialog
    v-model="visible"
    title="選擇選單"
    width="800px"
    @close="handleClose"
  >
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="選單名稱">
        <el-input v-model="queryForm.menuName" placeholder="請輸入選單名稱" clearable />
      </el-form-item>
      <el-form-item label="系統ID">
        <el-select v-model="queryForm.systemId" placeholder="請選擇" clearable>
          <el-option
            v-for="item in systemOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <el-table
      :data="tableData"
      v-loading="loading"
      @selection-change="handleSelectionChange"
      @row-click="handleRowClick"
      highlight-current-row
    >
      <el-table-column type="selection" width="55" />
      <el-table-column prop="menuId" label="選單ID" width="150" />
      <el-table-column prop="menuName" label="選單名稱" />
      <el-table-column prop="systemId" label="系統ID" width="120" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
          </el-tag>
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
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { menuListApi } from '@/api/lists';
import { systemListApi } from '@/api/system';

interface Props {
  modelValue: boolean;
  returnFields?: string; // 回傳欄位，如 "MENU_ID,MENU_NAME"
  returnControl?: string; // 回傳控制項ID
  systemId?: string; // 系統ID篩選
  multiple?: boolean; // 是否多選
}

const props = withDefaults(defineProps<Props>(), {
  returnFields: 'MENU_ID,MENU_NAME',
  multiple: false
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'confirm': [value: any];
}>();

const visible = ref(props.modelValue);
const loading = ref(false);
const tableData = ref([]);
const selectedRows = ref([]);
const systemOptions = ref([]);

const queryForm = ref({
  menuName: '',
  systemId: props.systemId || ''
});

const pagination = ref({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
});

watch(() => props.modelValue, (val) => {
  visible.value = val;
  if (val) {
    loadData();
    loadSystemOptions();
  }
});

watch(visible, (val) => {
  emit('update:modelValue', val);
});

const loadData = async () => {
  loading.value = true;
  try {
    const response = await menuListApi.getMenus({
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize,
      filters: {
        menuName: queryForm.value.menuName,
        systemId: queryForm.value.systemId,
        status: '1'
      }
    });
    tableData.value = response.data.items;
    pagination.value.totalCount = response.data.totalCount;
  } catch (error) {
    console.error('載入選單列表失敗:', error);
  } finally {
    loading.value = false;
  }
};

const loadSystemOptions = async () => {
  try {
    const response = await systemListApi.getSystemOptions();
    systemOptions.value = response.data;
  } catch (error) {
    console.error('載入系統選項失敗:', error);
  }
};

const handleQuery = () => {
  pagination.value.pageIndex = 1;
  loadData();
};

const handleReset = () => {
  queryForm.value = {
    menuName: '',
    systemId: props.systemId || ''
  };
  handleQuery();
};

const handleSelectionChange = (selection: any[]) => {
  selectedRows.value = selection;
};

const handleRowClick = (row: any) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.menuId === row.menuId);
    if (index > -1) {
      selectedRows.value.splice(index, 1);
    } else {
      selectedRows.value.push(row);
    }
  } else {
    // 單選模式：直接選中
    selectedRows.value = [row];
  }
};

const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size;
  loadData();
};

const handlePageChange = (page: number) => {
  pagination.value.pageIndex = page;
  loadData();
};

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個選單');
    return;
  }
  
  const returnFields = props.returnFields.split(',');
  const result = selectedRows.value.map(row => {
    const item: any = {};
    returnFields.forEach(field => {
      const fieldName = field.trim();
      item[fieldName] = row[fieldName.toLowerCase().replace(/_([a-z])/g, (_, c) => c.toUpperCase())];
    });
    return item;
  });
  
  emit('confirm', props.multiple ? result : result[0]);
  handleClose();
};

const handleClose = () => {
  visible.value = false;
  selectedRows.value = [];
  queryForm.value = {
    menuName: '',
    systemId: props.systemId || ''
  };
};
</script>
```

#### 4.1.2 選單選擇器組件 (`MenuSelector.vue`)
```vue
<template>
  <div>
    <el-input
      v-model="displayValue"
      :placeholder="placeholder"
      readonly
      @click="openDialog"
    >
      <template #append>
        <el-button @click="openDialog">選擇</el-button>
      </template>
    </el-input>
    <MenuListDialog
      v-model="dialogVisible"
      :return-fields="returnFields"
      :return-control="returnControl"
      :system-id="systemId"
      :multiple="multiple"
      @confirm="handleConfirm"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import MenuListDialog from './MenuListDialog.vue';

interface Props {
  modelValue?: string | string[];
  returnFields?: string;
  returnControl?: string;
  systemId?: string;
  multiple?: boolean;
  placeholder?: string;
}

const props = withDefaults(defineProps<Props>(), {
  returnFields: 'MENU_ID,MENU_NAME',
  placeholder: '請選擇選單',
  multiple: false
});

const emit = defineEmits<{
  'update:modelValue': [value: string | string[]];
  'change': [value: any];
}>();

const dialogVisible = ref(false);
const selectedValue = ref(props.modelValue);

const displayValue = computed(() => {
  if (Array.isArray(selectedValue.value)) {
    return selectedValue.value.join(', ');
  }
  return selectedValue.value || '';
});

const openDialog = () => {
  dialogVisible.value = true;
};

const handleConfirm = (value: any) => {
  if (props.multiple) {
    selectedValue.value = value.map((item: any) => item.MENU_ID);
  } else {
    selectedValue.value = value.MENU_ID;
  }
  emit('update:modelValue', selectedValue.value);
  emit('change', value);
};
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 Menus 資料表
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

### 5.3 階段三: 前端開發 (2.5天)
- [ ] API 呼叫函數
- [ ] MenuListDialog 組件開發
- [ ] MenuSelector 組件開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 分頁功能
- [ ] 多選/單選模式
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 多選模式測試
- [ ] 單選模式測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 組件使用文件
- [ ] 部署文件

**總計**: 6.5天

---

## 六、注意事項

### 6.1 選單階層
- 支援父子選單關係
- 可選顯示階層結構

### 6.2 系統篩選
- 支援依系統ID篩選選單
- 可選顯示所有系統的選單

### 6.3 狀態篩選
- 預設只顯示啟用的選單
- 可選顯示所有狀態的選單

### 6.4 多選模式
- 支援單選和多選模式
- 多選時回傳陣列格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢選單列表成功
- [ ] 依系統ID篩選成功
- [ ] 依選單名稱篩選成功
- [ ] 查詢單筆選單成功
- [ ] 取得選單選項成功

### 7.2 整合測試
- [ ] 選單選擇對話框正常運作
- [ ] 單選模式正常運作
- [ ] 多選模式正常運作
- [ ] 回傳值格式正確
- [ ] 分頁功能正常

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/ETEK_LIST/MENU_LIST.aspx`
- `IMS3/HANSHIN/IMS3/ETEK_LIST/MENU_LIST.aspx.cs`

### 8.2 相關功能
- `ADDR_CITY_LIST-地址城市列表.md` - 下拉列表功能參考
- `DATE_LIST-日期列表.md` - 下拉列表功能參考

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

