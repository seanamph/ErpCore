<template>
  <div class="warehouses">
    <div class="page-header">
      <h1>庫別資料維護 (SYSWB60)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="庫別代碼">
          <el-input v-model="queryForm.WarehouseId" placeholder="請輸入庫別代碼" clearable />
        </el-form-item>
        <el-form-item label="庫別名稱">
          <el-input v-model="queryForm.WarehouseName" placeholder="請輸入庫別名稱" clearable />
        </el-form-item>
        <el-form-item label="庫別類型">
          <el-select v-model="queryForm.WarehouseType" placeholder="請選擇庫別類型" clearable>
            <el-option label="全部" value="" />
            <el-option label="主倉庫" value="MAIN" />
            <el-option label="分倉庫" value="BRANCH" />
            <el-option label="臨時倉庫" value="TEMPORARY" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
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
        <el-table-column prop="WarehouseId" label="庫別代碼" width="120" />
        <el-table-column prop="WarehouseName" label="庫別名稱" width="200" />
        <el-table-column prop="WarehouseType" label="庫別類型" width="120">
          <template #default="{ row }">
            {{ getWarehouseTypeText(row.WarehouseType) }}
          </template>
        </el-table-column>
        <el-table-column prop="Location" label="位置" width="200" />
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
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
      :title="dialogTitle"
      v-model="dialogVisible"
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="庫別代碼" prop="WarehouseId">
          <el-input v-model="form.WarehouseId" placeholder="請輸入庫別代碼" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="庫別名稱" prop="WarehouseName">
          <el-input v-model="form.WarehouseName" placeholder="請輸入庫別名稱" />
        </el-form-item>
        <el-form-item label="庫別類型" prop="WarehouseType">
          <el-select v-model="form.WarehouseType" placeholder="請選擇庫別類型" clearable style="width: 100%">
            <el-option label="主倉庫" value="MAIN" />
            <el-option label="分倉庫" value="BRANCH" />
            <el-option label="臨時倉庫" value="TEMPORARY" />
          </el-select>
        </el-form-item>
        <el-form-item label="位置" prop="Location">
          <el-input v-model="form.Location" placeholder="請輸入位置" />
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
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
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { warehousesApi } from '@/api/warehouses'

// 查詢表單
const queryForm = reactive({
  WarehouseId: '',
  WarehouseName: '',
  WarehouseType: '',
  Status: ''
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
const dialogTitle = ref('新增庫別')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  WarehouseId: '',
  WarehouseName: '',
  WarehouseType: '',
  Location: '',
  SeqNo: 0,
  Status: 'A',
  Notes: ''
})

// 表單驗證規則
const rules = {
  WarehouseId: [{ required: true, message: '請輸入庫別代碼', trigger: 'blur' }],
  WarehouseName: [{ required: true, message: '請輸入庫別名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 取得庫別類型文字
const getWarehouseTypeText = (type) => {
  const typeMap = {
    'MAIN': '主倉庫',
    'BRANCH': '分倉庫',
    'TEMPORARY': '臨時倉庫'
  }
  return typeMap[type] || type || ''
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      WarehouseId: queryForm.WarehouseId || undefined,
      WarehouseName: queryForm.WarehouseName || undefined,
      WarehouseType: queryForm.WarehouseType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await warehousesApi.getWarehouses(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
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
  queryForm.WarehouseId = ''
  queryForm.WarehouseName = ''
  queryForm.WarehouseType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增庫別'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改庫別'
  try {
    const response = await warehousesApi.getWarehouse(row.WarehouseId)
    if (response.data.success) {
      Object.assign(form, response.data.data)
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此庫別嗎？', '提示', {
      type: 'warning'
    })
    const response = await warehousesApi.deleteWarehouse(row.WarehouseId)
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

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await warehousesApi.updateWarehouse(form.WarehouseId, form)
        } else {
          response = await warehousesApi.createWarehouse(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + error.message)
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.WarehouseId = ''
  form.WarehouseName = ''
  form.WarehouseType = ''
  form.Location = ''
  form.SeqNo = 0
  form.Status = 'A'
  form.Notes = ''
  formRef.value?.resetFields()
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

.warehouses {
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
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }
}
</style>

