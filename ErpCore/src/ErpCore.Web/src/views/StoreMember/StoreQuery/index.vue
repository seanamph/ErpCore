<template>
  <div class="store-query">
    <div class="page-header">
      <h1>商店查詢作業 (SYS3210-SYS3299)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" class="search-form" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="商店編號">
              <el-input v-model="queryForm.ShopId" placeholder="請輸入商店編號" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="商店名稱">
              <el-input v-model="queryForm.ShopName" placeholder="請輸入商店名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="商店類型">
              <el-input v-model="queryForm.ShopType" placeholder="請輸入商店類型" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="城市">
              <el-input v-model="queryForm.City" placeholder="請輸入城市" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域">
              <el-input v-model="queryForm.Zone" placeholder="請輸入區域" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
                <el-option label="全部" value="" />
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="樓層代碼">
              <el-input v-model="queryForm.FloorId" placeholder="請輸入樓層代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域代碼">
              <el-input v-model="queryForm.AreaId" placeholder="請輸入區域代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="POS啟用">
              <el-select v-model="queryForm.PosEnabled" placeholder="請選擇" clearable>
                <el-option label="全部" value="" />
                <el-option label="是" :value="true" />
                <el-option label="否" :value="false" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
          <el-button type="warning" @click="handlePrint">列印</el-button>
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
        style="width: 100%"
      >
        <el-table-column prop="ShopId" label="商店編號" width="150" />
        <el-table-column prop="ShopName" label="商店名稱" width="200" />
        <el-table-column prop="ShopNameEn" label="英文名稱" width="200" />
        <el-table-column prop="ShopType" label="商店類型" width="120" />
        <el-table-column prop="City" label="城市" width="100" />
        <el-table-column prop="Zone" label="區域" width="100" />
        <el-table-column prop="Phone" label="電話" width="150" />
        <el-table-column prop="ManagerName" label="店長" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PosEnabled" label="POS啟用" width="100">
          <template #default="{ row }">
            <el-tag :type="row.PosEnabled ? 'success' : 'info'">
              {{ row.PosEnabled ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分頁 -->
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
import { storeQueryApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  ShopId: '',
  ShopName: '',
  ShopType: '',
  City: '',
  Zone: '',
  Status: '',
  FloorId: '',
  AreaId: '',
  PosEnabled: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const response = await storeQueryApi.queryStores({
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ...queryForm
    })
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    ShopId: '',
    ShopName: '',
    ShopType: '',
    City: '',
    Zone: '',
    Status: '',
    FloorId: '',
    AreaId: '',
    PosEnabled: ''
  })
  pagination.PageIndex = 1
  handleSearch()
}

// 匯出
const handleExport = async () => {
  try {
    const response = await storeQueryApi.exportStores({
      ...queryForm
    })
    // 處理檔案下載
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `商店查詢結果_${new Date().getTime()}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗：' + error.message)
  }
}

// 列印
const handlePrint = async () => {
  try {
    const response = await storeQueryApi.printStoreReport({
      ...queryForm
    })
    // 處理PDF列印
    const blob = new Blob([response.data], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    window.open(url, '_blank')
    ElMessage.success('列印成功')
  } catch (error) {
    ElMessage.error('列印失敗：' + error.message)
  }
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 初始化
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

