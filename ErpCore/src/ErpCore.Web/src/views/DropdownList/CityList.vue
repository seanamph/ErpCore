<template>
  <div class="city-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>地址輸入 - 縣市列表</span>
          <div>
            <el-button type="info" @click="handleForeignAddress">國外地址</el-button>
            <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
          </div>
        </div>
      </template>

      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" inline>
        <el-form-item label="城市名稱">
          <el-input 
            v-model="queryForm.cityName" 
            placeholder="請輸入城市名稱"
            clearable
            @keyup.enter="handleSearch"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 城市列表 -->
      <el-table 
        :data="cityList" 
        v-loading="cityLoading"
        border 
        stripe 
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer; margin-top: 20px"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="CityId" label="城市代碼" width="120" />
        <el-table-column prop="CityName" label="城市名稱" min-width="200" />
        <el-table-column prop="CountryCode" label="國家代碼" width="100" align="center" />
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
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

// 表單資料
const queryForm = reactive({
  cityName: ''
})

// 城市列表
const cityList = ref([])
const cityLoading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 50,
  TotalCount: 0
})

// 查詢參數
const props = defineProps({
  returnField: {
    type: String,
    default: 'CityId'
  },
  returnControl: {
    type: String,
    default: null
  },
  countryCode: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['select', 'close'])

// 查詢城市列表
const loadCityList = async () => {
  cityLoading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CityName: queryForm.cityName || undefined,
      CountryCode: props.countryCode || undefined,
      Status: '1',
      SortField: 'CityName',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getCities(params)
    
    if (response.data?.success) {
      cityList.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    cityLoading.value = false
  }
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadCityList()
}

// 重置
const handleReset = () => {
  queryForm.cityName = ''
  handleSearch()
}

// 選擇城市
const handleRowClick = (row) => {
  // 回傳選中的城市
  emit('select', {
    cityId: row.CityId,
    cityName: row.CityName
  })
  
  // 如果有returnControl，則設定父視窗的控制項值
  if (props.returnControl && window.opener) {
    const returnField = props.returnField || 'CityId'
    const control = window.opener.document.getElementById(props.returnControl)
    if (control) {
      control.value = row[returnField] || row[returnField.toLowerCase()]
    }
  }
  
  // 關閉視窗或觸發關閉事件
  if (window.opener) {
    window.close()
  } else {
    emit('close')
  }
}

// 國外地址
const handleForeignAddress = () => {
  if (confirm('選擇輸入國外地址？')) {
    // 開啟國外地址選擇頁面
    // 這裡需要實作國外地址選擇功能
    ElMessage.info('國外地址功能開發中')
  }
}

// 關閉
const handleClose = () => {
  if (window.opener) {
    window.close()
  } else {
    emit('close')
  }
}

// 分頁變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadCityList()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadCityList()
}

// 初始化
onMounted(() => {
  loadCityList()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.city-list {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
