<template>
  <div class="store-query">
    <div class="page-header">
      <h1>?ÜÂ??•Ë©¢‰ΩúÊ•≠ (SYS6210-SYS6270)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" class="search-form" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="?ÜÂ?Á∑®Ë?">
              <el-input v-model="queryForm.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫óÁ∑®?? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?ÜÂ??çÁ®±">
              <el-input v-model="queryForm.ShopName" placeholder="Ë´ãËº∏?•Â?Â∫óÂ?Á®? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?ÜÂ?È°ûÂ?">
              <el-input v-model="queryForm.ShopType" placeholder="Ë´ãËº∏?•Â?Â∫óÈ??? clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="?éÂ?">
              <el-input v-model="queryForm.City" placeholder="Ë´ãËº∏?•Â?Â∏? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?Ä??>
              <el-input v-model="queryForm.Zone" placeholder="Ë´ãËº∏?•Â??? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?Ä??>
              <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?? clearable>
                <el-option label="?®ÈÉ®" value="" />
                <el-option label="?üÁî®" value="A" />
                <el-option label="?úÁî®" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="Ê®ìÂ±§‰ª?¢º">
              <el-input v-model="queryForm.FloorId" placeholder="Ë´ãËº∏?•Ê?Â±§‰ª£Á¢? clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="POS?üÁî®">
              <el-select v-model="queryForm.PosEnabled" placeholder="Ë´ãÈÅ∏?? clearable>
                <el-option label="?®ÈÉ®" value="" />
                <el-option label="?? :value="true" />
                <el-option label="?? :value="false" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleExport">?ØÂá∫</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="ShopId" label="?ÜÂ?Á∑®Ë?" width="150" />
        <el-table-column prop="ShopName" label="?ÜÂ??çÁ®±" width="200" />
        <el-table-column prop="ShopNameEn" label="?±Ê??çÁ®±" width="200" />
        <el-table-column prop="ShopType" label="?ÜÂ?È°ûÂ?" width="120" />
        <el-table-column prop="FloorId" label="Ê®ìÂ±§‰ª?¢º" width="120" />
        <el-table-column prop="FloorName" label="Ê®ìÂ±§?çÁ®±" width="120" />
        <el-table-column prop="City" label="?éÂ?" width="100" />
        <el-table-column prop="Zone" label="?Ä?? width="100" />
        <el-table-column prop="Phone" label="?ªË©±" width="150" />
        <el-table-column prop="ManagerName" label="Â∫óÈï∑" width="120" />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PosEnabled" label="POS?üÁî®" width="100">
          <template #default="{ row }">
            <el-tag :type="row.PosEnabled ? 'success' : 'info'">
              {{ row.PosEnabled ? '?? : '?? }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
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
import { storeQueryApi } from '@/api/storeFloor'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  ShopId: '',
  ShopName: '',
  ShopType: '',
  City: '',
  Zone: '',
  FloorId: '',
  Status: '',
  PosEnabled: ''
})

// Ë°®Ê†ºË≥áÊ?
const tableData = ref([])
const loading = ref(false)

// ?ÜÈ?
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// ?•Ë©¢
const handleSearch = async () => {
  loading.value = true
  try {
    const data = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ShopId: queryForm.ShopId || undefined,
      ShopName: queryForm.ShopName || undefined,
      ShopType: queryForm.ShopType || undefined,
      City: queryForm.City || undefined,
      Zone: queryForm.Zone || undefined,
      FloorId: queryForm.FloorId || undefined,
      Status: queryForm.Status || undefined,
      PosEnabled: queryForm.PosEnabled !== '' ? queryForm.PosEnabled : undefined
    }
    const response = await storeQueryApi.queryShopFloors(data)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  } finally {
    loading.value = false
  }
}

// ?çÁΩÆ
const handleReset = () => {
  queryForm.ShopId = ''
  queryForm.ShopName = ''
  queryForm.ShopType = ''
  queryForm.City = ''
  queryForm.Zone = ''
  queryForm.FloorId = ''
  queryForm.Status = ''
  queryForm.PosEnabled = ''
  pagination.PageIndex = 1
  handleSearch()
}

// ?ØÂá∫
const handleExport = async () => {
  try {
    const data = {
      ...queryForm,
      PageIndex: 1,
      PageSize: 999999,
      Format: 'Excel'
    }
    const response = await storeQueryApi.exportShopFloors(data)
    const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `?ÜÂ??•Ë©¢ÁµêÊ?_${new Date().toISOString().slice(0, 10)}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('?ØÂá∫?êÂ?')
  } catch (error) {
    ElMessage.error('?ØÂá∫Â§±Ê?Ôº? + error.message)
  }
}

// ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// ?ÜÈ?ËÆäÊõ¥
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// ?ºÂ??ñÊó•?üÊ???
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.store-query {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>

