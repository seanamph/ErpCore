<template>
  <div class="invoice-data">
    <div class="page-header">
      <h1>發票資料維護 (SYSG110-SYSG190)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="發票編號">
          <el-input v-model="queryForm.InvoiceId" placeholder="請輸入發票編號" clearable />
        </el-form-item>
        <el-form-item label="發票類型">
          <el-select v-model="queryForm.InvoiceType" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="統一發票" value="01" />
            <el-option label="電子發票" value="02" />
            <el-option label="收據" value="03" />
          </el-select>
        </el-form-item>
        <el-form-item label="發票年月">
          <el-date-picker
            v-model="queryForm.InvoiceYm"
            type="month"
            placeholder="請選擇年月"
            value-format="YYYYMM"
            clearable
          />
        </el-form-item>
        <el-form-item label="統一編號">
          <el-input v-model="queryForm.TaxId" placeholder="請輸入統一編號" clearable />
        </el-form-item>
        <el-form-item label="分公司代碼">
          <el-input v-model="queryForm.SiteId" placeholder="請輸入分公司代碼" clearable />
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
        <el-table-column prop="InvoiceId" label="發票編號" width="150" />
        <el-table-column prop="InvoiceType" label="發票類型" width="120">
          <template #default="{ row }">
            {{ getInvoiceTypeName(row.InvoiceType) }}
          </template>
        </el-table-column>
        <el-table-column prop="InvoiceYm" label="發票年月" width="120" />
        <el-table-column prop="Track" label="字軌" width="100" />
        <el-table-column prop="InvoiceNoB" label="發票號碼起" width="120" />
        <el-table-column prop="InvoiceNoE" label="發票號碼迄" width="120" />
        <el-table-column prop="TaxId" label="統一編號" width="150" />
        <el-table-column prop="CompanyName" label="公司名稱" width="200" />
        <el-table-column prop="SiteId" label="分公司代碼" width="120" />
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
        <el-divider>基本資料</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="發票編號" prop="InvoiceId">
              <el-input v-model="formData.InvoiceId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="發票類型" prop="InvoiceType">
              <el-select v-model="formData.InvoiceType" placeholder="請選擇">
                <el-option label="統一發票" value="01" />
                <el-option label="電子發票" value="02" />
                <el-option label="收據" value="03" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="發票年份" prop="InvoiceYear">
              <el-input-number v-model="formData.InvoiceYear" :min="1900" :max="2100" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="發票月份" prop="InvoiceMonth">
              <el-input-number v-model="formData.InvoiceMonth" :min="1" :max="12" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="字軌">
              <el-input v-model="formData.Track" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="發票號碼起">
              <el-input v-model="formData.InvoiceNoB" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="發票號碼迄">
              <el-input v-model="formData.InvoiceNoE" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="發票格式代號">
              <el-input v-model="formData.InvoiceFormat" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分公司代碼">
              <el-input v-model="formData.SiteId" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-divider>稅籍資料</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="統一編號">
              <el-input v-model="formData.TaxId" maxlength="8" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="公司名稱">
              <el-input v-model="formData.CompanyName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="公司英文名稱">
              <el-input v-model="formData.CompanyNameEn" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="地址">
              <el-input v-model="formData.Address" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="城市">
              <el-input v-model="formData.City" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="區域">
              <el-input v-model="formData.Zone" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="郵遞區號">
              <el-input v-model="formData.PostalCode" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="電話">
              <el-input v-model="formData.Phone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="傳真">
              <el-input v-model="formData.Fax" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="電子郵件">
          <el-input v-model="formData.Email" type="email" />
        </el-form-item>

        <el-divider>其他資料</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="副聯">
              <el-input v-model="formData.SubCopy" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="副聯值">
              <el-input v-model="formData.SubCopyValue" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
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
          <el-input v-model="formData.Notes" type="textarea" :rows="3" />
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
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { invoiceDataApi } from '@/api/invoiceSales'

// 查詢表單
const queryForm = reactive({
  InvoiceId: '',
  InvoiceType: '',
  InvoiceYm: '',
  TaxId: '',
  SiteId: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改發票資料' : '新增發票資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  InvoiceId: '',
  InvoiceType: '01',
  InvoiceYear: new Date().getFullYear(),
  InvoiceMonth: new Date().getMonth() + 1,
  InvoiceYm: '',
  Track: '',
  InvoiceNoB: '',
  InvoiceNoE: '',
  InvoiceFormat: '',
  TaxId: '',
  CompanyName: '',
  CompanyNameEn: '',
  Address: '',
  City: '',
  Zone: '',
  PostalCode: '',
  Phone: '',
  Fax: '',
  Email: '',
  SiteId: '',
  SubCopy: '',
  SubCopyValue: '',
  Status: 'A',
  Notes: ''
})
const formRules = {
  InvoiceId: [{ required: true, message: '請輸入發票編號', trigger: 'blur' }],
  InvoiceType: [{ required: true, message: '請選擇發票類型', trigger: 'change' }],
  InvoiceYear: [
    { required: true, message: '請輸入發票年份', trigger: 'blur' },
    { type: 'number', min: 1900, max: 2100, message: '年份範圍為 1900-2100', trigger: 'blur' }
  ],
  InvoiceMonth: [
    { required: true, message: '請輸入發票月份', trigger: 'blur' },
    { type: 'number', min: 1, max: 12, message: '月份範圍為 1-12', trigger: 'blur' }
  ],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }],
  TaxId: [
    { pattern: /^\d{8}$|^$/, message: '統一編號必須為8位數字', trigger: 'blur' }
  ],
  Email: [
    { type: 'email', message: '請輸入正確的電子郵件格式', trigger: 'blur' }
  ]
}
const currentTKey = ref(null)

// 監聽年份和月份變化，自動更新 InvoiceYm
watch([() => formData.InvoiceYear, () => formData.InvoiceMonth], ([year, month]) => {
  if (year && month) {
    formData.InvoiceYm = `${year}${String(month).padStart(2, '0')}`
  }
})

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      InvoiceId: queryForm.InvoiceId || undefined,
      InvoiceType: queryForm.InvoiceType || undefined,
      InvoiceYm: queryForm.InvoiceYm || undefined,
      TaxId: queryForm.TaxId || undefined,
      SiteId: queryForm.SiteId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await invoiceDataApi.getInvoices(params)
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
  queryForm.InvoiceId = ''
  queryForm.InvoiceType = ''
  queryForm.InvoiceYm = ''
  queryForm.TaxId = ''
  queryForm.SiteId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentTKey.value = null
  const currentDate = new Date()
  Object.assign(formData, {
    InvoiceId: '',
    InvoiceType: '01',
    InvoiceYear: currentDate.getFullYear(),
    InvoiceMonth: currentDate.getMonth() + 1,
    InvoiceYm: '',
    Track: '',
    InvoiceNoB: '',
    InvoiceNoE: '',
    InvoiceFormat: '',
    TaxId: '',
    CompanyName: '',
    CompanyNameEn: '',
    Address: '',
    City: '',
    Zone: '',
    PostalCode: '',
    Phone: '',
    Fax: '',
    Email: '',
    SiteId: '',
    SubCopy: '',
    SubCopyValue: '',
    Status: 'A',
    Notes: ''
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
    const response = await invoiceDataApi.getInvoice(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      const data = response.data.data
      // 從 InvoiceYm 解析年份和月份
      if (data.InvoiceYm && data.InvoiceYm.length === 6) {
        data.InvoiceYear = parseInt(data.InvoiceYm.substring(0, 4))
        data.InvoiceMonth = parseInt(data.InvoiceYm.substring(4, 6))
      }
      Object.assign(formData, data)
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
    await ElMessageBox.confirm('確定要刪除此發票資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await invoiceDataApi.deleteInvoice(row.TKey)
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
      // 驗證發票號碼區間
      if (formData.InvoiceNoB && formData.InvoiceNoE) {
        if (formData.InvoiceNoB > formData.InvoiceNoE) {
          ElMessage.error('發票號碼起必須小於等於發票號碼迄')
          return
        }
      }
      try {
        // 確保 InvoiceYm 正確
        if (formData.InvoiceYear && formData.InvoiceMonth) {
          formData.InvoiceYm = `${formData.InvoiceYear}${String(formData.InvoiceMonth).padStart(2, '0')}`
        }
        let response
        if (isEdit.value) {
          response = await invoiceDataApi.updateInvoice(currentTKey.value, formData)
        } else {
          response = await invoiceDataApi.createInvoice(formData)
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

// 取得發票類型名稱
const getInvoiceTypeName = (type) => {
  const types = {
    '01': '統一發票',
    '02': '電子發票',
    '03': '收據'
  }
  return types[type] || type
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.invoice-data {
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

