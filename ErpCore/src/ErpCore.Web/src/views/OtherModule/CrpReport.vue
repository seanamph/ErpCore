<template>
  <div class="crp-report">
    <div class="page-header">
      <h1>CRP報表模組</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表代碼">
          <el-input v-model="queryForm.ReportCode" placeholder="請輸入報表代碼" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleAdd">新增</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 報表列表 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="ReportCode" label="報表代碼" width="150" />
        <el-table-column prop="ReportName" label="報表名稱" width="200" />
        <el-table-column prop="Description" label="說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180" />
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleGenerate(row)">生成報表</el-button>
            <el-button type="success" size="small" @click="handleEdit(row)">編輯</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/編輯對話框 -->
    <el-dialog
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" ref="formRef" label-width="120px">
        <el-form-item label="報表代碼" prop="ReportCode">
          <el-input v-model="form.ReportCode" placeholder="請輸入報表代碼" />
        </el-form-item>
        <el-form-item label="報表名稱" prop="ReportName">
          <el-input v-model="form.ReportName" placeholder="請輸入報表名稱" />
        </el-form-item>
        <el-form-item label="說明" prop="Description">
          <el-input
            v-model="form.Description"
            type="textarea"
            :rows="4"
            placeholder="請輸入說明"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { otherModuleApi } from '@/api/otherModule'

// 查詢表單
const queryForm = reactive({
  ReportCode: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增報表')
const form = reactive({
  ReportId: null,
  ReportCode: '',
  ReportName: '',
  Description: ''
})
const formRef = ref(null)
const isEdit = ref(false)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const response = await otherModuleApi.crpReport.getReports()
    if (response.data.success) {
      tableData.value = response.data.data || []
      if (queryForm.ReportCode) {
        tableData.value = tableData.value.filter(item =>
          item.ReportCode.includes(queryForm.ReportCode)
        )
      }
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.ReportCode = ''
  handleSearch()
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增報表'
  isEdit.value = false
  form.ReportId = null
  form.ReportCode = ''
  form.ReportName = ''
  form.Description = ''
  dialogVisible.value = true
}

// 編輯
const handleEdit = (row) => {
  dialogTitle.value = '編輯報表'
  isEdit.value = true
  form.ReportId = row.ReportId
  form.ReportCode = row.ReportCode
  form.ReportName = row.ReportName
  form.Description = row.Description
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此報表嗎？', '警告', {
      type: 'warning',
      confirmButtonText: '確定',
      cancelButtonText: '取消'
    })
    const response = await otherModuleApi.crpReport.deleteReport(row.ReportId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 生成報表
const handleGenerate = async (row) => {
  try {
    const response = await otherModuleApi.crpReport.generateReport({
      ReportCode: row.ReportCode
    })
    if (response.data.success) {
      ElMessage.success('報表生成成功')
    } else {
      ElMessage.error(response.data.message || '報表生成失敗')
    }
  } catch (error) {
    ElMessage.error('報表生成失敗：' + error.message)
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
          response = await otherModuleApi.crpReport.updateReport(form.ReportId, form)
        } else {
          response = await otherModuleApi.crpReport.createReport(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '更新成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  form.ReportId = null
  form.ReportCode = ''
  form.ReportName = ''
  form.Description = ''
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.crp-report {
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
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .search-form {
    margin: 0;
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>

