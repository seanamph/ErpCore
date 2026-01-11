<template>
  <el-dialog
    v-model="visible"
    title="多選店別列表"
    width="1000px"
    @close="handleClose"
  >
    <div class="info-bar">
      <span>符合條件者共 {{ totalCount }} 筆</span>
    </div>
    
    <el-form :inline="true" :model="queryForm" class="query-form">
      <el-form-item label="區域">
        <el-select
          v-model="queryForm.RegionIds"
          multiple
          placeholder="請選擇區域"
          clearable
          style="width: 200px"
        >
          <el-option
            v-for="item in regionOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="店型態">
        <el-select
          v-model="queryForm.TypeIds"
          multiple
          placeholder="請選擇店型態"
          clearable
          style="width: 200px"
        >
          <el-option
            v-for="item in typeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="店級別">
        <el-select
          v-model="queryForm.ShopLevels"
          multiple
          placeholder="請選擇店級別"
          clearable
          style="width: 200px"
        >
          <el-option
            v-for="item in levelOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="店別名稱">
        <el-input
          v-model="queryForm.ShopName"
          placeholder="請輸入店別名稱"
          clearable
          style="width: 200px"
        />
      </el-form-item>
      
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <div class="toolbar">
      <div>
        <el-button v-if="showAllShop" @click="handleSelectAllShops">全部分店</el-button>
        <el-button @click="handleSelectAll">全部選取</el-button>
        <el-button @click="handleClearAll">全部取消</el-button>
      </div>
    </div>
    
    <el-table
      :data="tableData"
      v-loading="loading"
      @selection-change="handleSelectionChange"
      @row-click="handleRowClick"
      highlight-current-row
      border
      stripe
      style="width: 100%"
    >
      <el-table-column type="selection" width="55" />
      <el-table-column type="index" label="序號" width="80" />
      <el-table-column prop="ShopId" label="店別編號" width="120" />
      <el-table-column prop="ShopName" label="店別名稱" min-width="150" />
      <el-table-column prop="RegionName" label="區域" width="100" />
      <el-table-column prop="TypeName" label="店型態" width="100" />
      <el-table-column prop="ShopLevelName" label="店級別" width="100" />
      <el-table-column prop="Status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.Status === 'A' || row.Status === '1' ? 'success' : 'danger'" size="small">
            {{ row.Status === 'A' || row.Status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    
    <el-pagination
      v-model:current-page="pagination.PageIndex"
      v-model:page-size="pagination.PageSize"
      :total="pagination.TotalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
      style="margin-top: 20px"
    />
    
    <template #footer>
      <el-button @click="handleClose">關閉</el-button>
      <el-button type="primary" @click="handleConfirm">確認選擇</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnFields: {
    type: String,
    default: 'ShopId,ShopName'
  },
  returnControl: {
    type: String,
    default: null
  },
  status: {
    type: String,
    default: '1'
  },
  multiple: {
    type: Boolean,
    default: true
  },
  allShop: {
    type: String,
    default: null
  },
  kind: {
    type: String,
    default: null
  },
  goodsId: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const loading = ref(false)
const tableData = ref([])
const selectedShopIds = ref(new Set())
const totalCount = ref(0)
const showAllShop = ref(props.allShop === 'Y')

const queryForm = ref({
  ShopName: '',
  RegionIds: [],
  TypeIds: [],
  ShopLevels: []
})

const pagination = ref({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

const regionOptions = ref([])
const typeOptions = ref([])
const levelOptions = ref([])

watch(() => props.modelValue, (val) => {
  visible.value = val
  if (val) {
    loadFilterOptions()
    loadData()
  }
})

watch(visible, (val) => {
  emit('update:modelValue', val)
})

onMounted(() => {
  if (visible.value) {
    loadFilterOptions()
    loadData()
  }
})

const loadFilterOptions = async () => {
  try {
    // 載入區域選項
    const regionResponse = await dropdownListApi.getAreaOptions({ status: 'A' })
    if (regionResponse.data?.success) {
      regionOptions.value = regionResponse.data.data || []
    }
    
    // 載入店型態選項（從參數表）
    // TODO: 需要實現參數查詢 API
    // const typeResponse = await dropdownListApi.getParameterOptions({ parameterCode: 'SHOP_TYPE' })
    // if (typeResponse.data?.success) {
    //   typeOptions.value = typeResponse.data.data || []
    // }
    
    // 載入店級別選項（從參數表）
    // TODO: 需要實現參數查詢 API
    // const levelResponse = await dropdownListApi.getParameterOptions({ parameterCode: 'SHOP_LEVEL' })
    // if (levelResponse.data?.success) {
    //   levelOptions.value = levelResponse.data.data || []
    // }
  } catch (error) {
    console.error('載入篩選選項失敗:', error)
  }
}

const loadData = async () => {
  loading.value = true
  try {
    const params = {
      ShopName: queryForm.value.ShopName || undefined,
      RegionIds: queryForm.value.RegionIds.length > 0 ? queryForm.value.RegionIds.join(',') : undefined,
      TypeIds: queryForm.value.TypeIds.length > 0 ? queryForm.value.TypeIds.join(',') : undefined,
      ShopLevels: queryForm.value.ShopLevels.length > 0 ? queryForm.value.ShopLevels.join(',') : undefined,
      Status: props.status || '1',
      Kind: props.kind || undefined,
      GoodsId: props.goodsId || undefined,
      AllShop: props.allShop || undefined,
      SortField: 'ShopId',
      SortOrder: 'ASC'
    }
    
    const response = await dropdownListApi.getMultiShops(params)
    if (response.data?.success) {
      const items = response.data.data || []
      tableData.value = items
      totalCount.value = items.length
      pagination.value.TotalCount = items.length
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    console.error('載入店別列表失敗:', error)
    ElMessage.error('載入店別列表失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleQuery = () => {
  pagination.value.PageIndex = 1
  loadData()
}

const handleReset = () => {
  queryForm.value = {
    ShopName: '',
    RegionIds: [],
    TypeIds: [],
    ShopLevels: []
  }
  handleQuery()
}

const handleSelectionChange = (selection) => {
  // Element Plus 的 selection 功能，但我們使用自定義的選擇邏輯
}

const handleRowClick = (row) => {
  if (selectedShopIds.value.has(row.ShopId)) {
    selectedShopIds.value.delete(row.ShopId)
  } else {
    selectedShopIds.value.add(row.ShopId)
  }
}

const isSelected = (shopId) => {
  return selectedShopIds.value.has(shopId)
}

const handleSelectAllShops = () => {
  // 選擇所有店別（全部分店）
  tableData.value.forEach(row => {
    selectedShopIds.value.add(row.ShopId)
  })
  ElMessage.success('已選擇所有店別')
}

const handleSelectAll = () => {
  // 選擇當前頁面的所有店別
  tableData.value.forEach(row => {
    selectedShopIds.value.add(row.ShopId)
  })
  ElMessage.success('已選擇當前頁面的所有店別')
}

const handleClearAll = () => {
  selectedShopIds.value.clear()
  ElMessage.success('已清除所有選擇')
}

const handleSizeChange = (size) => {
  pagination.value.PageSize = size
  loadData()
}

const handlePageChange = (page) => {
  pagination.value.PageIndex = page
  loadData()
}

const handleConfirm = () => {
  if (selectedShopIds.value.size === 0) {
    ElMessage.warning('請選擇至少一個店別')
    return
  }
  
  // 處理正常選擇
  const returnFields = props.returnFields.split(',')
  const selectedRows = tableData.value.filter(row => selectedShopIds.value.has(row.ShopId))
  
  if (props.multiple) {
    // 多選模式：返回店別代碼列表（逗號分隔）
    const shopIdList = selectedRows.map(row => row.ShopId).join(',')
    if (props.returnControl) {
      if (window.opener && window.opener.document) {
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = shopIdList
        }
      }
    }
    emit('confirm', shopIdList)
  } else {
    // 單選模式：返回第一個選擇的店別
    if (selectedRows.length > 0) {
      const row = selectedRows[0]
      const result = {}
      returnFields.forEach(field => {
        const fieldName = field.trim()
        const pascalField = fieldName.charAt(0).toUpperCase() + fieldName.slice(1).toLowerCase()
        result[fieldName] = row[pascalField] || row[fieldName] || ''
      })
      if (props.returnControl) {
        if (window.opener && window.opener.document) {
          const control = window.opener.document.getElementById(props.returnControl)
          if (control) {
            control.value = result[props.returnFields.split(',')[0].trim()]
          }
        }
      }
      emit('confirm', result)
    }
  }
  
  handleClose()
}

const handleClose = () => {
  visible.value = false
  selectedShopIds.value.clear()
  queryForm.value = {
    ShopName: '',
    RegionIds: [],
    TypeIds: [],
    ShopLevels: []
  }
  pagination.value.PageIndex = 1
}
</script>

<style lang="scss" scoped>
.info-bar {
  margin-bottom: 10px;
  padding: 10px;
  background-color: #f5f7fa;
  border-radius: 4px;
  font-size: 14px;
}

.toolbar {
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.query-form {
  margin-bottom: 20px;
}
</style>
