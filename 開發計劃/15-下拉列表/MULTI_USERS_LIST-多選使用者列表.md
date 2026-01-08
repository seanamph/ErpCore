# MULTI_USERS_LIST - 多選使用者列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MULTI_USERS_LIST
- **功能名稱**: 多選使用者列表
- **功能描述**: 提供多選使用者選擇的下拉列表功能，支援使用者查詢、篩選、多選等功能，用於需要選擇多個使用者的業務場景
- **參考舊程式**: 
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_USERS_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_USERS_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_USERS_LIST.aspx.cs`

### 1.2 業務需求
- 提供使用者列表查詢功能
- 支援使用者編號、使用者名稱篩選
- 支援多選使用者功能
- 支援部門篩選
- 支援已選使用者的清除功能
- 支援選擇結果回傳至父視窗
- 與使用者主檔（Users）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Users` (使用者主檔)
參考 `SYS0110-使用者基本資料維護.md` 的 `Users` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `Departments` - 部門主檔
參考 `SYSWB40-部別資料維護作業.md` 的 `Departments` 資料表設計

### 2.3 資料字典

參考 `SYS0110-使用者基本資料維護.md` 的資料字典

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢使用者列表（用於多選）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/users/multi`
- **說明**: 查詢使用者列表，支援篩選、排序、分頁，用於多選列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 50,
    "sortField": "UserId",
    "sortOrder": "ASC",
    "filters": {
      "userId": "",
      "userName": "",
      "orgId": "",
      "status": "A"
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
          "userId": "U001",
          "userName": "張三",
          "orgId": "ORG001",
          "orgName": "資訊部",
          "title": "經理",
          "status": "A"
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

#### 3.1.2 查詢使用者選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/users/options`
- **說明**: 取得使用者選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `orgId`: 部門代號（可選）
  - `status`: 狀態（預設為'A'）
  - `keyword`: 關鍵字（使用者編號或名稱）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "userId": "U001",
        "userName": "張三",
        "displayName": "U001 - 張三"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `UserListController.cs`
```csharp
[ApiController]
[Route("api/v1/lists/users")]
public class UserListController : ControllerBase
{
    private readonly IUserListService _userListService;

    [HttpGet("multi")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetUsersForMultiSelect([FromQuery] UserListQueryDto query)
    {
        // 實作查詢使用者列表邏輯
    }

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<List<UserOptionDto>>>> GetUserOptions([FromQuery] UserOptionQueryDto query)
    {
        // 實作查詢使用者選項邏輯
    }
}
```

#### 3.2.2 Service: `UserListService.cs`
```csharp
public interface IUserListService
{
    Task<PagedResult<UserListDto>> GetUsersForMultiSelectAsync(UserListQueryDto query);
    Task<List<UserOptionDto>> GetUserOptionsAsync(UserOptionQueryDto query);
}
```

#### 3.2.3 Repository: `UserListRepository.cs`
```csharp
public interface IUserListRepository
{
    Task<PagedResult<UserList>> GetUsersForMultiSelectAsync(UserListQuery query);
    Task<List<UserOption>> GetUserOptionsAsync(UserOptionQuery query);
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 多選使用者列表頁面 (`MultiUserList.vue`)
```vue
<template>
  <div class="multi-user-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>多選使用者</span>
          <el-button type="primary" size="small" @click="handleConfirm">確定</el-button>
        </div>
      </template>
      
      <!-- 查詢表單 -->
      <el-form :model="queryForm" :inline="true" class="query-form">
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.userId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.userName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item label="部門">
          <el-select v-model="queryForm.orgId" placeholder="請選擇部門" clearable>
            <el-option
              v-for="item in orgList"
              :key="item.orgId"
              :label="item.orgName"
              :value="item.orgId"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 已選使用者 -->
      <div class="selected-users" v-if="selectedUsers.length > 0">
        <el-tag
          v-for="user in selectedUsers"
          :key="user.userId"
          closable
          @close="handleRemoveSelected(user.userId)"
          style="margin-right: 8px; margin-bottom: 8px;"
        >
          {{ user.userName }} ({{ user.userId }})
        </el-tag>
      </div>

      <!-- 使用者列表 -->
      <el-table
        :data="userList"
        border
        @selection-change="handleSelectionChange"
        style="margin-top: 16px;"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="userId" label="使用者編號" width="120" />
        <el-table-column prop="userName" label="使用者名稱" width="150" />
        <el-table-column prop="orgName" label="部門" width="150" />
        <el-table-column prop="title" label="職稱" width="120" />
        <el-table-column prop="status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 'A' ? 'success' : 'danger'">
              {{ row.status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[20, 50, 100, 200]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 16px;"
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getUserListForMultiSelect, getUserOptions } from '@/api/user-list.api';
import { getOrgList } from '@/api/org.api';

// 查詢表單
const queryForm = ref({
  userId: '',
  userName: '',
  orgId: '',
  status: 'A'
});

// 使用者列表
const userList = ref([]);
const selectedUsers = ref([]);
const orgList = ref([]);

// 分頁
const pagination = ref({
  pageIndex: 1,
  pageSize: 50,
  totalCount: 0
});

// 查詢使用者列表
const loadUserList = async () => {
  try {
    const response = await getUserListForMultiSelect({
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize,
      filters: queryForm.value
    });
    
    if (response.success) {
      userList.value = response.data.items;
      pagination.value.totalCount = response.data.totalCount;
    }
  } catch (error) {
    console.error('查詢使用者列表失敗:', error);
  }
};

// 載入部門列表
const loadOrgList = async () => {
  try {
    const response = await getOrgList();
    if (response.success) {
      orgList.value = response.data;
    }
  } catch (error) {
    console.error('載入部門列表失敗:', error);
  }
};

// 查詢
const handleQuery = () => {
  pagination.value.pageIndex = 1;
  loadUserList();
};

// 重置
const handleReset = () => {
  queryForm.value = {
    userId: '',
    userName: '',
    orgId: '',
    status: 'A'
  };
  handleQuery();
};

// 選擇變更
const handleSelectionChange = (selection: any[]) => {
  selectedUsers.value = selection;
};

// 移除已選使用者
const handleRemoveSelected = (userId: string) => {
  selectedUsers.value = selectedUsers.value.filter(u => u.userId !== userId);
  // 同步更新表格選擇狀態
  // ...
};

// 確定
const handleConfirm = () => {
  if (window.opener) {
    // 回傳選擇的使用者至父視窗
    window.opener.postMessage({
      type: 'MULTI_USERS_SELECTED',
      data: selectedUsers.value
    }, '*');
    window.close();
  }
};

// 分頁變更
const handleSizeChange = (size: number) => {
  pagination.value.pageSize = size;
  pagination.value.pageIndex = 1;
  loadUserList();
};

const handlePageChange = (page: number) => {
  pagination.value.pageIndex = page;
  loadUserList();
};

onMounted(() => {
  loadOrgList();
  loadUserList();
});
</script>
```

### 4.2 UI 元件設計

#### 4.2.1 多選使用者選擇器元件 (`MultiUserSelector.vue`)
可重用的多選使用者選擇器元件，用於表單中

### 4.3 API 呼叫 (`user-list.api.ts`)
```typescript
import request from '@/utils/request';
import type { ApiResponse, PagedResult } from '@/types/api';
import type { UserListDto, UserOptionDto } from '@/types/user';

export const getUserListForMultiSelect = (params: any) => {
  return request.get<ApiResponse<PagedResult<UserListDto>>>('/api/v1/lists/users/multi', { params });
};

export const getUserOptions = (params: any) => {
  return request.get<ApiResponse<List<UserOptionDto>>>('/api/v1/lists/users/options', { params });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認 Users 資料表結構
- [ ] 確認索引設計

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 多選使用者列表頁面開發
- [ ] 多選使用者選擇器元件開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6天

---

## 六、注意事項

### 6.1 業務邏輯
- 多選使用者列表支援跨部門查詢
- 已選使用者需顯示在列表上方
- 選擇結果需回傳至父視窗
- 支援清除已選使用者功能

### 6.2 資料驗證
- 使用者編號、名稱需支援模糊查詢
- 部門篩選需支援階層查詢（包含子部門）

### 6.3 效能
- 大量使用者查詢必須使用分頁
- 部門列表需使用快取機制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者列表成功
- [ ] 多選使用者功能正常
- [ ] 部門篩選功能正常
- [ ] 分頁功能正常

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 多選流程測試
- [ ] 回傳父視窗測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ETEK_LIST/MULTI_USERS_LIST.aspx`
- `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_USERS_LIST.aspx`
- `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_USERS_LIST.aspx.cs`

### 8.2 相關功能
- `SYS0110-使用者基本資料維護.md` - 使用者管理功能
- `MULTI_AREA_LIST-多選區域列表.md` - 多選列表參考

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

