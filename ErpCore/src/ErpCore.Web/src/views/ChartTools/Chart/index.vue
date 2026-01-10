<template>
  <div class="chart">
    <div class="page-header">
      <h1>圖表功能 (SYS1000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="圖表名稱">
          <el-input v-model="queryForm.ChartName" placeholder="請輸入圖表名稱" clearable />
        </el-form-item>
        <el-form-item label="圖表類型">
          <el-select v-model="queryForm.ChartType" placeholder="請選擇圖表類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="柱狀圖" value="bar" />
            <el-option label="折線圖" value="line" />
            <el-option label="餅圖" value="pie" />
            <el-option label="散點圖" value="scatter" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增圖表</el-button>
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
        <el-table-column prop="ChartId" label="圖表ID" width="150" />
        <el-table-column prop="ChartName" label="圖表名稱" width="200" />
        <el-table-column prop="ChartType" label="圖表類型" width="120">
          <template #default="{ row }">
            <el-tag>{{ getChartTypeText(row.ChartType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

    <!-- 新增/修改對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="圖表ID" prop="ChartId">
          <el-input v-model="formData.ChartId" :disabled="isEdit" placeholder="請輸入圖表ID" />
        </el-form-item>
        <el-form-item label="圖表名稱" prop="ChartName">
          <el-input v-model="formData.ChartName" placeholder="請輸入圖表名稱" />
        </el-form-item>
        <el-form-item label="圖表類型" prop="ChartType">
          <el-select v-model="formData.ChartType" placeholder="請選擇圖表類型" style="width: 100%">
            <el-option label="柱狀圖" value="bar" />
            <el-option label="折線圖" value="line" />
            <el-option label="餅圖" value="pie" />
            <el-option label="散點圖" value="scatter" />
          </el-select>
        </el-form-item>
        <el-form-item label="說明">
          <el-input v-model="formData.Description" type="textarea" :rows="3" placeholder="請輸入說明" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '@/api/axios'

// API
const chartApi = {
  getCharts: (params) => axios.get('/api/v1/charts', { params }),
  getChart: (chartId) => axios.get(`/api/v1/charts/${chartId}`),
  createChart: (data) => axios.post('/api/v1/charts', data),
  updateChart: (chartId, data) => axios.put(`/api/v1/charts/${chartId}`, data),
  deleteChart: (chartId) => axios.delete(`/api/v1/charts/${chartId}`)
}

// 查詢表單
const queryForm = reactive({
  ChartName: '',
  ChartType: ''
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

// 對話框
const dialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '修改圖表' : '新增圖表')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  ChartId: '',
  ChartName: '',
  ChartType: 'bar',
  Description: '',
  Status: 'A'
})
const formRules = {
  ChartId: [{ required: true, message: '請輸入圖表ID', trigger: 'blur' }],
  ChartName: [{ required: true, message: '請輸入圖表名稱', trigger: 'blur' }],
  ChartType: [{ required: true, message: '請選擇圖表類型', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentChartId = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ChartName: queryForm.ChartName || undefined,
      ChartType: queryForm.ChartType || undefined
    }
    const response = await chartApi.getCharts(params)
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
  queryForm.ChartName = ''
  queryForm.ChartType = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentChartId.value = null
  Object.assign(formData, {
    ChartId: '',
    ChartName: '',
    ChartType: 'bar',
    Description: '',
    Status: 'A'
  })
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  await handleEdit(row)
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await chartApi.getChart(row.ChartId)
    if (response.data?.success) {
      isEdit.value = true
      currentChartId.value = row.ChartId
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此圖表嗎？', '確認', {
      type: 'warning'
    })
    const response = await chartApi.deleteChart(row.ChartId)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await chartApi.updateChart(currentChartId.value, formData)
        } else {
          response = await chartApi.createChart(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
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

// 取得圖表類型文字
const getChartTypeText = (type) => {
  const texts = {
    'bar': '柱狀圖',
    'line': '折線圖',
    'pie': '餅圖',
    'scatter': '散點圖'
  }
  return texts[type] || type
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.chart {
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

