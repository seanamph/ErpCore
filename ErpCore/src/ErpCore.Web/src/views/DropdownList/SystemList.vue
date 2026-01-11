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

