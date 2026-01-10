<template>
  <div class="promotion-data">
    <div class="page-header">
      <h1>促銷活動維護 (SYS3510-SYS3600)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="活動編號">
          <el-input v-model="queryForm.PromotionId" placeholder="請輸入活動編號" clearable />
        </el-form-item>
        <el-form-item label="活動名稱">
          <el-input v-model="queryForm.PromotionName" placeholder="請輸入活動名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
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
        <el-table-column prop="PromotionId" label="活動編號" width="150" />
        <el-table-column prop="PromotionName" label="活動名稱" width="200" />
        <el-table-column prop="StartDate" label="開始日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.StartDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="EndDate" label="結束日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.EndDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="DiscountType" label="折扣類型" width="120" />
        <el-table-column prop="DiscountValue" label="折扣值" width="100" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
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
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="150px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="活動編號" prop="PromotionId">
              <el-input v-model="formData.PromotionId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="活動名稱" prop="PromotionName">
              <el-input v-model="formData.PromotionName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開始日期" prop="StartDate">
              <el-date-picker
                v-model="formData.StartDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="EndDate">
              <el-date-picker
                v-model="formData.EndDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="折扣類型" prop="DiscountType">
              <el-select v-model="formData.DiscountType" placeholder="請選擇">
                <el-option label="百分比" value="PERCENTAGE" />
                <el-option label="固定金額" value="FIXED" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="折扣值" prop="DiscountValue">
              <el-input-number v-model="formData.DiscountValue" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="適用商店">
              <el-input v-model="formData.ShopIds" placeholder="多個商店編號用逗號分隔" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="formData.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" />
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
import { promotionApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  PromotionId: '',
  PromotionName: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改促銷活動' : '新增促銷活動')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  PromotionId: '',
  PromotionName: '',
  StartDate: null,
  EndDate: null,
  DiscountType: 'PERCENTAGE',
  DiscountValue: 0,
  ShopIds: '',
  Status: 'A',
  Memo: ''
})
const formRules = {
  PromotionId: [{ required: true, message: '請輸入活動編號', trigger: 'blur' }],
  PromotionName: [{ required: true, message: '請輸入活動名稱', trigger: 'blur' }],
  StartDate: [{ required: true, message: '請選擇開始日期', trigger: 'change' }],
  EndDate: [{ required: true, message: '請選擇結束日期', trigger: 'change' }],
  DiscountType: [{ required: true, message: '請選擇折扣類型', trigger: 'change' }],
  DiscountValue: [{ required: true, message: '請輸入折扣值', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentPromotionId = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      PromotionId: queryForm.PromotionId || undefined,
      PromotionName: queryForm.PromotionName || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await promotionApi.getPromotions(params)
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
  queryForm.PromotionId = ''
  queryForm.PromotionName = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentPromotionId.value = null
  Object.assign(formData, {
    PromotionId: '',
    PromotionName: '',
    StartDate: null,
    EndDate: null,
    DiscountType: 'PERCENTAGE',
    DiscountValue: 0,
    ShopIds: '',
    Status: 'A',
    Memo: ''
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
    const response = await promotionApi.getPromotion(row.PromotionId)
    if (response.data?.success) {
      isEdit.value = true
      currentPromotionId.value = row.PromotionId
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
    await ElMessageBox.confirm('確定要刪除此促銷活動嗎？', '確認', {
      type: 'warning'
    })
    const response = await promotionApi.deletePromotion(row.PromotionId)
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
          response = await promotionApi.updatePromotion(currentPromotionId.value, formData)
        } else {
          response = await promotionApi.createPromotion(formData)
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

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.promotion-data {
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

