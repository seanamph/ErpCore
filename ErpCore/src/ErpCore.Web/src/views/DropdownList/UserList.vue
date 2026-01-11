<template>
  <div class="user-list">
    <div class="page-header">
      <h1>使用者列表 (USER_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="列表類型">
          <el-select v-model="listType" placeholder="請選擇列表類型" @change="handleListTypeChange" style="width: 150px">
            <el-option label="使用者列表" value="user" />
            <el-option label="部門使用者列表" value="dept" />
            <el-option label="其他使用者列表" value="other" />
          </el-select>
        </el-form-item>
        <el-form-item label="使用者編號">
          <el-input v-model="queryForm.UserId" placeholder="請輸入使用者編號" clearable />
        </el-form-item>
        <el-form-item label="使用者名稱">
          <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
        </el-form-item>
        <el-form-item v-if="listType === 'dept'" label="部門">
          <el-select v-model="queryForm.OrgId" placeholder="請選擇部門" clearable filterable style="width: 200px">
            <el-option
              v-for="item in orgList"
              :key="item.Value"
              :label="item.Label"
              :value="item.Value"
            />
          </el-select>
        </el-form-item>
        <el-form-item v-if="listType !== 'dept'" label="部門">
          <el-input v-model="queryForm.OrgId" placeholder="請輸入部門代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 120px">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="鎖定" value="L" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 使用者列表 -->
      <el-table
        :data="userList"
        v-loading="loading"
        border
        stripe
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer; margin-top: 20px"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="UserId" label="使用者編號" width="120" />
        <el-table-column prop="UserName" label="使用者名稱" min-width="150" />
        <el-table-column prop="OrgName" label="部門" min-width="150" />
        <el-table-column prop="Title" label="職稱" width="120" />
        <el-table-column prop="Status" label="狀態" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : row.Status === 'I' ? 'danger' : 'warning'" size="small">
              {{ row.Status === 'A' ? '啟用' : row.Status === 'I' ? '停用' : '鎖定' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[20, 50, 100, 200]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'
import { departmentsApi } from '@/api/departments'
import { useRoute } from 'vue-router'

const route = useRoute()
const listType = ref(route.query.type || 'user') // user, dept, other

// 查詢表單
const queryForm = reactive({
  UserId: '',
  UserName: '',
  OrgId: '',
  Status: 'A'
})

// 使用者列表
const userList = ref([])
const loading = ref(false)
const orgList = ref([])
const pagination = reactive({
  PageIndex: 1,
  PageSize: 50,
  TotalCount: 0
})

// 載入部門列表
const loadOrgList = async () => {
  if (listType.value === 'dept') {
    try {
      const deptResponse = await departmentsApi.getDepartments({ 
        PageIndex: 1, 
        PageSize: 1000,
        Status: 'A'
      })
      
      if (deptResponse.data?.success) {
        const items = deptResponse.data.data?.Items || deptResponse.data.data?.items || []
        orgList.value = items.map(dept => ({
          Value: dept.DeptId,
          Label: dept.DeptName || dept.DeptId
        }))
      } else if (deptResponse.data?.data) {
        // 如果返回格式不同，尝试其他格式
        const items = Array.isArray(deptResponse.data.data) 
          ? deptResponse.data.data 
          : deptResponse.data.data.Items || deptResponse.data.data.items || []
        orgList.value = items.map(dept => ({
          Value: dept.DeptId,
          Label: dept.DeptName || dept.DeptId
        }))
      }
    } catch (error) {
      console.error('載入部門列表失敗:', error)
      ElMessage.error('載入部門列表失敗')
    }
  } else {
    orgList.value = []
  }
}

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      UserId: queryForm.UserId || undefined,
      UserName: queryForm.UserName || undefined,
      OrgId: queryForm.OrgId || undefined,
      Status: queryForm.Status || undefined,
      SortField: 'UserId',
      SortOrder: 'ASC'
    }

    let response
    if (listType.value === 'dept') {
      response = await dropdownListApi.getDeptUserList(params)
    } else if (listType.value === 'other') {
      response = await dropdownListApi.getOtherUserList(params)
    } else {
      response = await dropdownListApi.getUserList(params)
    }

    if (response.data?.success) {
      userList.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
    console.error('查詢使用者列表失敗:', error)
  } finally {
    loading.value = false
  }
}

// 列表類型變更
const handleListTypeChange = () => {
  queryForm.OrgId = ''
  pagination.PageIndex = 1
  loadOrgList()
  loadData()
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  queryForm.UserId = ''
  queryForm.UserName = ''
  queryForm.OrgId = ''
  queryForm.Status = 'A'
  handleSearch()
}

// 行點擊
const handleRowClick = (row) => {
  ElMessage.success(`已選擇使用者：${row.UserName} (${row.UserId})`)
  
  // 如果是彈窗模式，回傳選擇的使用者至父視窗
  if (window.opener) {
    window.opener.postMessage({
      type: 'USER_SELECTED',
      data: row
    }, '*')
    window.close()
  }
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

// 初始化
onMounted(() => {
  loadOrgList()
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.user-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}
</style>
