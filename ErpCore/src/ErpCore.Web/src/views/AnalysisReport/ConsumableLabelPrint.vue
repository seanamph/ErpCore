<template>
  <div class="consumable-label-print">
    <div class="page-header">
      <h1>耗材標籤列印作業 (SYSA254)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="列印類型">
          <el-select v-model="queryForm.Type" placeholder="請選擇列印類型" clearable>
            <el-option label="耗材管理報表" value="1" />
            <el-option label="耗材標籤列印" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="正常" value="1" />
            <el-option label="停用" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="queryForm.SiteId" placeholder="請輸入店別代碼" clearable />
        </el-form-item>
        <el-form-item label="資產狀態">
          <el-input v-model="queryForm.AssetStatus" placeholder="請輸入資產狀態" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button
            type="success"
            :disabled="selectedItems.length === 0"
            @click="handlePrint"
          >
            列印
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        @selection-change="handleSelectionChange"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="ConsumableId" label="耗材編號" width="120" />
        <el-table-column prop="ConsumableName" label="耗材名稱" width="200" />
        <el-table-column prop="BarCode" label="條碼" width="150" />
        <el-table-column prop="CategoryName" label="分類" width="120" />
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="SiteName" label="店別" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.StatusName || (row.Status === '1' ? '正常' : '停用') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Quantity" label="數量" width="100" align="right" />
        <el-table-column prop="Price" label="單價" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Price) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { consumablePrintApi } from '@/api/consumablePrint'

const loading = ref(false)
const tableData = ref([])
const selectedItems = ref([])
const queryForm = reactive({
  Type: '2',
  Status: '',
  SiteId: '',
  AssetStatus: ''
})

const handleSearch = async () => {
  try {
    loading.value = true
    const response = await consumablePrintApi.getPrintList(queryForm)
    if (response.data.success) {
      tableData.value = response.data.data.items || []
      ElMessage.success('查詢成功')
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryForm.Type = '2'
  queryForm.Status = ''
  queryForm.SiteId = ''
  queryForm.AssetStatus = ''
  tableData.value = []
}

const handleSelectionChange = (selection) => {
  selectedItems.value = selection
}

const handlePrint = async () => {
  if (selectedItems.value.length === 0) {
    ElMessage.warning('請選擇要列印的耗材')
    return
  }

  try {
    await ElMessageBox.confirm(
      `確定要列印 ${selectedItems.value.length} 筆耗材標籤嗎？`,
      '確認列印',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    loading.value = true
    const consumableIds = selectedItems.value.map(item => item.ConsumableId)
    const printData = {
      Type: queryForm.Type || '2',
      ConsumableIds: consumableIds,
      PrintCount: 1,
      SiteId: queryForm.SiteId || null
    }

    const response = await consumablePrintApi.batchPrint(printData)
    if (response.data.success) {
      ElMessage.success(`列印成功，共列印 ${response.data.data.printCount} 筆`)
      await handleSearch()
    } else {
      ElMessage.error(response.data.message || '列印失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('列印失敗: ' + (error.message || '未知錯誤'))
    }
  } finally {
    loading.value = false
  }
}

const formatCurrency = (value) => {
  if (value == null) return '-'
  return new Intl.NumberFormat('zh-TW', {
    style: 'currency',
    currency: 'TWD',
    minimumFractionDigits: 0
  }).format(value)
}

onMounted(() => {
  handleSearch()
})
</script>

<style scoped>
.consumable-label-print {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>
