# USER_LIST - 使用者列表系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: USER_LIST
- **功能名稱**: 使用者列表系列
- **功能描述**: 提供多種使用者列表選擇功能，包括使用者列表、部門列表、其他列表等，支援使用者查詢、篩選、選擇等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ETEK_LIST/USER_LIST_USER_LIST.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/USER_LIST_DEPT_LIST.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/USER_LIST_OTHER_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/USER_LIST_USER_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/USER_LIST_DEPT_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/USER_LIST_OTHER_LIST.aspx`

### 1.2 業務需求
- **USER_LIST_USER_LIST**: 提供使用者列表選擇功能
- **USER_LIST_DEPT_LIST**: 提供部門使用者列表選擇功能
- **USER_LIST_OTHER_LIST**: 提供其他使用者列表選擇功能
- 支援使用者編號、使用者名稱篩選
- 支援部門篩選
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

#### 3.1.1 查詢使用者列表（USER_LIST_USER_LIST）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/users/user-list`
- **說明**: 查詢使用者列表，支援篩選、排序、分頁
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
      "status": "A"
    }
  }
  ```
- **回應格式**: 參考 `MULTI_USERS_LIST-多選使用者列表.md`

#### 3.1.2 查詢部門使用者列表（USER_LIST_DEPT_LIST）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/users/dept-list`
- **說明**: 根據部門查詢使用者列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 50,
    "orgId": "ORG001",
    "filters": {
      "userId": "",
      "userName": ""
    }
  }
  ```
- **回應格式**: 參考 `MULTI_USERS_LIST-多選使用者列表.md`

#### 3.1.3 查詢其他使用者列表（USER_LIST_OTHER_LIST）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/users/other-list`
- **說明**: 查詢其他類型使用者列表（如外部使用者、廠商使用者等）
- **請求參數**: 類似 USER_LIST_USER_LIST
- **回應格式**: 參考 `MULTI_USERS_LIST-多選使用者列表.md`

### 3.2 後端實作類別

#### 3.2.1 Controller: `UserListController.cs`
```csharp
[ApiController]
[Route("api/v1/lists/users")]
public class UserListController : ControllerBase
{
    private readonly IUserListService _userListService;

    [HttpGet("user-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetUserList([FromQuery] UserListQueryDto query)
    {
        // 實作查詢使用者列表邏輯
    }

    [HttpGet("dept-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetDeptUserList([FromQuery] DeptUserListQueryDto query)
    {
        // 實作查詢部門使用者列表邏輯
    }

    [HttpGet("other-list")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserListDto>>>> GetOtherUserList([FromQuery] UserListQueryDto query)
    {
        // 實作查詢其他使用者列表邏輯
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者列表頁面 (`UserList.vue`)
```vue
<template>
  <div class="user-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>選擇使用者</span>
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
        <el-form-item v-if="listType === 'dept'" label="部門">
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

      <!-- 使用者列表 -->
      <el-table
        :data="userList"
        border
        highlight-current-row
        @current-change="handleCurrentChange"
        style="margin-top: 16px;"
      >
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
import { useRoute } from 'vue-router';
import { getUserList, getDeptUserList, getOtherUserList } from '@/api/user-list.api';
import { getOrgList } from '@/api/org.api';

const route = useRoute();
const listType = ref(route.query.type || 'user'); // user, dept, other

// 查詢表單
const queryForm = ref({
  userId: '',
  userName: '',
  orgId: '',
  status: 'A'
});

// 使用者列表
const userList = ref([]);
const selectedUser = ref(null);
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
    let response;
    const params = {
      pageIndex: pagination.value.pageIndex,
      pageSize: pagination.value.pageSize,
      filters: queryForm.value
    };

    if (listType.value === 'dept') {
      response = await getDeptUserList({ ...params, orgId: queryForm.value.orgId });
    } else if (listType.value === 'other') {
      response = await getOtherUserList(params);
    } else {
      response = await getUserList(params);
    }
    
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
  if (listType.value === 'dept') {
    try {
      const response = await getOrgList();
      if (response.success) {
        orgList.value = response.data;
      }
    } catch (error) {
      console.error('載入部門列表失敗:', error);
    }
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
const handleCurrentChange = (row: any) => {
  selectedUser.value = row;
};

// 確定
const handleConfirm = () => {
  if (!selectedUser.value) {
    ElMessage.warning('請選擇使用者');
    return;
  }

  if (window.opener) {
    // 回傳選擇的使用者至父視窗
    window.opener.postMessage({
      type: 'USER_SELECTED',
      data: selectedUser.value
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

### 4.2 API 呼叫 (`user-list.api.ts`)
```typescript
import request from '@/utils/request';
import type { ApiResponse, PagedResult } from '@/types/api';
import type { UserListDto } from '@/types/user';

export const getUserList = (params: any) => {
  return request.get<ApiResponse<PagedResult<UserListDto>>>('/api/v1/lists/users/user-list', { params });
};

export const getDeptUserList = (params: any) => {
  return request.get<ApiResponse<PagedResult<UserListDto>>>('/api/v1/lists/users/dept-list', { params });
};

export const getOtherUserList = (params: any) => {
  return request.get<ApiResponse<PagedResult<UserListDto>>>('/api/v1/lists/users/other-list', { params });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認 Users 資料表結構
- [ ] 確認索引設計

### 5.2 階段二: 後端開發 (2.5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作（三個端點）
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2.5天)
- [ ] API 呼叫函數
- [ ] 使用者列表頁面開發（三個版本）
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

**總計**: 7天

---

## 六、注意事項

### 6.1 業務邏輯
- USER_LIST_USER_LIST: 查詢所有使用者
- USER_LIST_DEPT_LIST: 根據部門查詢使用者（包含子部門）
- USER_LIST_OTHER_LIST: 查詢其他類型使用者（如外部使用者、廠商使用者等）
- 選擇結果需回傳至父視窗

### 6.2 資料驗證
- 使用者編號、名稱需支援模糊查詢
- 部門篩選需支援階層查詢

### 6.3 效能
- 大量使用者查詢必須使用分頁
- 部門列表需使用快取機制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者列表成功
- [ ] 查詢部門使用者列表成功
- [ ] 查詢其他使用者列表成功
- [ ] 分頁功能正常

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 選擇流程測試
- [ ] 回傳父視窗測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ETEK_LIST/USER_LIST_USER_LIST.aspx`
- `WEB/IMS_CORE/ETEK_LIST/USER_LIST_DEPT_LIST.aspx`
- `WEB/IMS_CORE/ETEK_LIST/USER_LIST_OTHER_LIST.aspx`

### 8.2 相關功能
- `SYS0110-使用者基本資料維護.md` - 使用者管理功能
- `MULTI_USERS_LIST-多選使用者列表.md` - 多選使用者列表參考

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

