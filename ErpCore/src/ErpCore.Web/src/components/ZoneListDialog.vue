<template>
  <el-dialog
    v-model="visible"
    title="選擇區域"
    width="900px"
    @close="handleClose"
  >
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="城市">
        <el-select
          v-model="queryForm.CityId"
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
        <el-input v-model="queryForm.ZoneName" placeholder="請輸入區域名稱" clearable />
      </el-form-item>
      <el-form-item label="郵遞區號">
        <el-input v-model="queryForm.ZipCode" placeholder="請輸入郵遞區號" clearable />
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
      border
      stripe
    >
      <el-table-column v-if="multiple" type="selection" width="55" />
      <el-table-column type="index" label="序號" width="80" />
      <el-table-column prop="ZoneId" label="區域代碼" width="150" />
      <el-table-column prop="ZoneName" label="區域名稱" min-width="200" />
      <el-table-column prop="CityName" label="城市名稱" width="150" />
      <el-table-column prop="ZipCode" label="郵遞區號" width="120" align="center" />
      <el-table-column prop="Status" label="狀態" width="80">
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
      style="margin-top: 20px; justify-content: flex-end"
    />
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'ZoneId,ZoneName,CityId,CityName,ZipCode'
  },
  returnControl: {
    type: String,
    default: null
  },
  multiple: {
    type: Boolean,
    default: false
  },
  cityId: {
    type: String,
    default: null
  },
  status: {
    type: String,
    default: '1'
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedRows = ref([])
const cityOptions = ref([])

const queryForm = ref({
  CityId: props.cityId || '',
  ZoneName: '',
  ZipCode: ''
})

const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 載入城市選項
const loadCityOptions = async () => {
  try {
    const response = await dropdownListApi.getCityOptions({
      status: '1'
    })
    if (response.data?.success) {
      cityOptions.value = response.data.data || []
    }
  } catch (error) {
    console.error('載入城市選項失敗:', error)
  }
}

watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    loadCityOptions().then(() => {
      loadData()
    })
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

watch(() => props.cityId, (val) => {
  queryForm.value.CityId = val || ''
  if (visible.value) {
    loadData()
  }
})

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CityId: queryForm.value.CityId || undefined,
      ZoneName: queryForm.value.ZoneName || undefined,
      ZipCode: queryForm.value.ZipCode || undefined,
      Status: props.status || '1',
      SortField: 'ZoneName',
      SortOrder: 'ASC'
    }
    const response = await dropdownListApi.getZones(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.Items || []
      pagination.TotalCount = response.data.data?.TotalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入區域列表失敗:', error)
    ElMessage.error('載入區域列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleCityChange = () => {
  pagination.PageIndex = 1
  loadData()
}

const handleQuery = () => {
  pagination.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    CityId: props.cityId || '',
    ZoneName: '',
    ZipCode: ''
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  selectedRows.value = selection
}

const handleRowClick = (row) => {
  if (props.multiple) {
    // 多選模式：切換選中狀態
    const index = selectedRows.value.findIndex(item => item.ZoneId === row.ZoneId)
    if (index > -1) {
      selectedRows.value.splice(index, 1)
    } else {
      selectedRows.value.push(row)
    }
  } else {
    // 單選模式：直接選中
    selectedRows.value = [row]
  }
}

const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  loadData()
}

const handlePageChange = (page) => {
  pagination.PageIndex = page
  loadData()
}

const handleConfirm = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('請選擇至少一個區域')
    return
  }
  
  const returnFields = props.returnFields.split(',')
  const result = selectedRows.value.map(row => {
    const item = {}
    returnFields.forEach(field => {
      const fieldName = field.trim()
      // 轉換為 PascalCase
      const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1)
      item[fieldName] = row[pascalField] || row[fieldName] || ''
    })
    return item
  })
  
  emit('confirm', props.multiple ? result : result[0])
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedRows.value = []
  queryForm.value = {
    CityId: props.cityId || '',
    ZoneName: '',
    ZipCode: ''
  }
  pagination.PageIndex = 1
  pagination.TotalCount = 0
}

onMounted(() => {
  if (props.modelValue) {
    loadCityOptions()
  }
})
</script>

<style lang="scss" scoped>
.query-form {
  margin-bottom: 20px;
}
</style>
