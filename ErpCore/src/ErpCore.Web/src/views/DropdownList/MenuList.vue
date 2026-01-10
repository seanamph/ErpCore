<template>
  <div class="menu-list">
    <div class="page-header">
      <h1>選單列表 (MENU_LIST)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="選單名稱">
          <el-input v-model="queryForm.MenuName" placeholder="請輸入選單名稱" clearable />
        </el-form-item>
        <el-form-item label="選單代碼">
          <el-input v-model="queryForm.MenuId" placeholder="請輸入選單代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 選單列表 -->
      <el-table
        :data="menuList"
        v-loading="loading"
        border
        stripe
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="MenuId" label="選單代碼" width="120" />
        <el-table-column prop="MenuName" label="選單名稱" min-width="200" />
        <el-table-column prop="ParentMenuId" label="父選單代碼" width="120" />
        <el-table-column prop="MenuPath" label="選單路徑" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Icon" label="圖示" width="100" align="center" />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" />
        <el-table-column prop="Status" label="狀態" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'" size="small">
              {{ row.Status === '1' ? '啟用' : '停用' }}
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

// 查詢表單
const queryForm = reactive({
  MenuName: '',
  MenuId: '',
  Status: ''
})

// 選單列表
const menuList = ref([])
const loading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 載入資料
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      MenuName: queryForm.MenuName || undefined,
      MenuId: queryForm.MenuId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await dropdownListApi.getMenus(params)
    if (response.data?.success) {
      menuList.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  queryForm.MenuName = ''
  queryForm.MenuId = ''
  queryForm.Status = ''
  handleSearch()
}

// 行點擊
const handleRowClick = (row) => {
  ElMessage.success(`已選擇選單：${row.MenuName} (${row.MenuId})`)
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
  loadData()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.menu-list {
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

