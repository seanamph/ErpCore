<template>
  <div class="customer-data">
    <div class="page-header">
      <h1>客戶資料維護 (SYS2000)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="客戶編號">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
        </el-form-item>
        <el-form-item label="客戶名稱">
          <el-input v-model="queryForm.CustomerName" placeholder="請輸入客戶名稱" clearable />
        </el-form-item>
        <el-form-item label="客戶類型">
          <el-select v-model="queryForm.CustomerType" placeholder="請選擇" clearable>
            <el-option label="客戶" value="C" />
            <el-option label="廠商" value="V" />
          </el-select>
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
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="CustomerType" label="客戶類型" width="100">
          <template #default="{ row }">
            {{ row.CustomerType === 'C' ? '客戶' : '廠商' }}
          </template>
        </el-table-column>
        <el-table-column prop="TaxId" label="統一編號" width="150" />
        <el-table-column prop="ContactPerson" label="聯絡人" width="120" />
        <el-table-column prop="ContactPhone" label="聯絡電話" width="150" />
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
      width="800px"
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
            <el-form-item label="客戶編號" prop="CustomerId">
              <el-input v-model="formData.CustomerId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="客戶名稱" prop="CustomerName">
              <el-input v-model="formData.CustomerName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="客戶類型" prop="CustomerType">
              <el-select v-model="formData.CustomerType" placeholder="請選擇">
                <el-option label="客戶" value="C" />
                <el-option label="廠商" value="V" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="統一編號">
              <el-input v-model="formData.TaxId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="聯絡人">
              <el-input v-model="formData.ContactPerson" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="聯絡電話">
              <el-input v-model="formData.ContactPhone" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="聯絡信箱">
              <el-input v-model="formData.ContactEmail" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="傳真號碼">
              <el-input v-model="formData.ContactFax" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="地址">
          <el-input v-model="formData.Address" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="城市代碼">
              <el-input v-model="formData.CityId" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域代碼">
              <el-input v-model="formData.ZoneId" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="郵遞區號">
              <el-input v-model="formData.ZipCode" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="付款條件">
              <el-input v-model="formData.PaymentTerm" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="信用額度">
              <el-input-number v-model="formData.CreditLimit" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="幣別">
              <el-input v-model="formData.CurrencyId" />
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
import { customerDataApi } from '@/api/customerInvoice'

// 查詢表單
const queryForm = reactive({
  CustomerId: '',
  CustomerName: '',
  CustomerType: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改客戶資料' : '新增客戶資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  CustomerId: '',
  CustomerName: '',
  CustomerType: 'C',
  TaxId: '',
  ContactPerson: '',
  ContactPhone: '',
  ContactEmail: '',
  ContactFax: '',
  Address: '',
  CityId: '',
  ZoneId: '',
  ZipCode: '',
  PaymentTerm: '',
  CreditLimit: 0,
  CurrencyId: 'TWD',
  Status: 'A',
  Memo: ''
})
const formRules = {
  CustomerId: [{ required: true, message: '請輸入客戶編號', trigger: 'blur' }],
  CustomerName: [{ required: true, message: '請輸入客戶名稱', trigger: 'blur' }],
  CustomerType: [{ required: true, message: '請選擇客戶類型', trigger: 'change' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentCustomerId = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CustomerId: queryForm.CustomerId || undefined,
      CustomerName: queryForm.CustomerName || undefined,
      CustomerType: queryForm.CustomerType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await customerDataApi.getCustomers(params)
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
  queryForm.CustomerId = ''
  queryForm.CustomerName = ''
  queryForm.CustomerType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentCustomerId.value = null
  Object.assign(formData, {
    CustomerId: '',
    CustomerName: '',
    CustomerType: 'C',
    TaxId: '',
    ContactPerson: '',
    ContactPhone: '',
    ContactEmail: '',
    ContactFax: '',
    Address: '',
    CityId: '',
    ZoneId: '',
    ZipCode: '',
    PaymentTerm: '',
    CreditLimit: 0,
    CurrencyId: 'TWD',
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
    const response = await customerDataApi.getCustomer(row.CustomerId)
    if (response.data?.success) {
      isEdit.value = true
      currentCustomerId.value = row.CustomerId
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
    await ElMessageBox.confirm('確定要刪除此客戶資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await customerDataApi.deleteCustomer(row.CustomerId)
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
          response = await customerDataApi.updateCustomer(currentCustomerId.value, formData)
        } else {
          response = await customerDataApi.createCustomer(formData)
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

.customer-data {
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

