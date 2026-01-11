<template>
  <div class="system-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>系統ID列表</span>
          <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
        </div>
      </template>

      <!-- 查詢表單 -->
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="請輸入系統ID或系統名稱"
          clearable
          @keyup.enter="handleSearch"
          style="width: 300px; margin-right: 10px"
        />
        <el-button type="primary" @click="handleSearch">開始查詢</el-button>
        <el-button @click="handleClose">關閉</el-button>
      </div>

      <!-- 系統列表 -->
      <el-table
        :data="systemList"
        v-loading="systemLoading"
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; margin-top: 20px; cursor: pointer"
      >
        <el-table-column type="index" label="序號" width="80" align="center" />
        <el-table-column prop="SystemId" label="系統ID" width="120" />
        <el-table-column prop="SystemName" label="系統名稱" />
      </el-table>

      <div class="footer" style="margin-top: 20px; text-align: right">
        <el-button @click="handleClose">關閉</el-button>
      </div>
    </el-card>
  </div>
</template>

        <el-tab-pane label="使用者列表" name="user">
          <el-form :inline="true" :model="userQueryForm" class="search-form">
            <el-form-item label="使用者名稱">
              <el-input v-model="userQueryForm.UserName" placeholder="請輸入使用者名稱" clearable />
            </el-form-item>
            <el-form-item label="使用者代碼">
              <el-input v-model="userQueryForm.UserId" placeholder="請輸入使用者代碼" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="userQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserSearch">查詢</el-button>
              <el-button @click="handleUserReset">重置</el-button>
            </el-form-item>
          </el-form>

          <!-- 使用者列表 -->
          <el-table
            :data="userList"
            v-loading="userLoading"
            border
            stripe
            highlight-current-row
            @row-click="handleUserRowClick"
            style="width: 100%; cursor: pointer"
          >
            <el-table-column type="index" label="序號" width="60" align="center" />
            <el-table-column prop="UserId" label="使用者代碼" width="120" />
            <el-table-column prop="UserName" label="使用者名稱" min-width="200" />
            <el-table-column prop="Title" label="職稱" width="150" />
            <el-table-column prop="OrgId" label="組織代碼" width="120" />
            <el-table-column prop="Status" label="狀態" width="80" align="center">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'" size="small">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>

          <!-- 分頁 -->
          <el-pagination
            v-model:current-page="userPagination.PageIndex"
            v-model:page-size="userPagination.PageSize"
            :total="userPagination.TotalCount"
            :page-sizes="[20, 50, 100, 200]"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleUserSizeChange"
            @current-change="handleUserPageChange"
            style="margin-top: 20px; text-align: right"
          />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

// 搜尋文字
const searchText = ref('')

// 系統列表
const systemList = ref([])
const systemLoading = ref(false)

// 載入系統列表
const loadSystemList = async () => {
  systemLoading.value = true
  try {
    const params = {}
    
    // 如果搜尋文字不為空，同時搜尋系統ID和系統名稱
    if (searchText.value) {
      params.SystemId = searchText.value
      params.SystemName = searchText.value
    }
    
    const response = await dropdownListApi.getSystems(params)
    if (response.data?.success) {
      systemList.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    systemLoading.value = false
  }
}

// 搜尋系統
const handleSearch = () => {
  loadSystemList()
}

// 點擊表格列
const handleRowClick = (row) => {
  // 選擇系統後，可以回傳給父視窗或導向選單列表頁面
  ElMessage.success(`已選擇系統：${row.SystemName} (${row.SystemId})`)
  
  // 這裡可以根據實際需求處理選擇結果
  // 例如：回傳給父視窗、導向選單列表頁面等
  // window.opener?.handleSystemSelected?.(row.SystemId, row.SystemName)
}

// 關閉視窗
const handleClose = () => {
  // 如果是彈窗模式，關閉視窗
  if (window.opener) {
    window.close()
  } else {
    // 如果是頁面模式，返回上一頁或導向首頁
    window.history.back()
  }
}

// 初始化
onMounted(() => {
  loadSystemList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.system-list {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-bar {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.footer {
  margin-top: 20px;
  text-align: right;
}
</style>

