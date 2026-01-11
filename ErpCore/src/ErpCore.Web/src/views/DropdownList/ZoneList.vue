<template>
  <div class="zone-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>地址輸入 - 區域列表</span>
          <div>
            <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
          </div>
        </div>
      </template>

      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" inline>
        <el-form-item label="城市">
          <el-select
            v-model="queryForm.cityId"
            placeholder="請選擇城市"
            clearable
            filterable
            style="width: 200px"
            @change="handleCityChange"
          >
            <el-option
              v-for="city in cityOptions"
              :key="city.Value"
              :label="city.Label"
              :value="city.Value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="區域名稱">
          <el-input 
            v-model="queryForm.zoneName" 
            placeholder="請輸入區域名稱"
            clearable
            @keyup.enter="handleSearch"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="郵遞區號">
          <el-input 
            v-model="queryForm.zipCode" 
            placeholder="請輸入郵遞區號"
            clearable
            @keyup.enter="handleSearch"
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 區域列表 -->
      <el-table 
        :data="zoneList" 
        v-loading="zoneLoading"
        border 
        stripe 
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer; margin-top: 20px"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="ZoneId" label="區域代碼" width="120" />
        <el-table-column prop="ZoneName" label="區域名稱" min-width="200" />
        <el-table-column prop="CityName" label="城市名稱" width="150" />
        <el-table-column prop="ZipCode" label="郵遞區號" width="120" align="center" />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="center" />
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
  cityId: '',
  zoneName: '',
  zipCode: ''
})

// 城市選項
const cityOptions = ref([])

// 區域列表
const zoneList = ref([])
const zoneLoading = ref(false)
const pagination = reactive({
  PageIndex: 1,
  PageSize: 50,
  TotalCount: 0
})

// 查詢參數
const props = defineProps({
  returnField: {
    type: String,
    default: 'ZoneId'
  },
  returnControl: {
    type: String,
    default: null
  },
  cityId: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['select', 'close'])

// 載入城市選項
const loadCityOptions = async () => {
  try {
    const response = await dropdownListApi.getCityOptions({
      status: '1'
    })
    
    if (response.data?.success) {
      cityOptions.value = response.data.data || []
      
      // 如果有傳入 cityId，設定為預設值
      if (props.cityId) {
        queryForm.cityId = props.cityId
      }
    }
  } catch (error) {
    console.error('載入城市選項失敗：', error)
  }
}

// 查詢區域列表
const loadZoneList = async () => {
  zoneLoading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CityId: queryForm.cityId || undefined,
      ZoneName: queryForm.zoneName || undefined,
      ZipCode: queryForm.zipCode || undefined,
      Status: '1',
      SortField: 'ZoneName',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getZones(params)
    
    if (response.data?.success) {
      zoneList.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    zoneLoading.value = false
  }
}

// 城市變更
const handleCityChange = () => {
  pagination.PageIndex = 1
  loadZoneList()
}

// 查詢
const handleSearch = () => {
  pagination.PageIndex = 1
  loadZoneList()
}

// 重置
const handleReset = () => {
  queryForm.cityId = props.cityId || ''
  queryForm.zoneName = ''
  queryForm.zipCode = ''
  handleSearch()
}

// 選擇區域
const handleRowClick = (row) => {
  // 回傳選中的區域
  emit('select', {
    zoneId: row.ZoneId,
    zoneName: row.ZoneName,
    cityId: row.CityId,
    cityName: row.CityName,
    zipCode: row.ZipCode
  })
  
  // 如果有returnControl，則設定父視窗的控制項值
  if (props.returnControl && window.opener) {
    const returnField = props.returnField || 'ZoneId'
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
  loadZoneList()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadZoneList()
}

// 初始化
onMounted(() => {
  loadCityOptions().then(() => {
    loadZoneList()
  })
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.zone-list {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
