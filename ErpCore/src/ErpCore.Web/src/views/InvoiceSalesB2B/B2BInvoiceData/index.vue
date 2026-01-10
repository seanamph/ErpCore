<template>
  <div class="b2b-invoice-data">
    <div class="page-header">
      <h1>B2B?ºÁ•®Ë≥áÊ?Á∂≠Ë≠∑</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ºÁ•®Á∑®Ë?">
          <el-input v-model="queryForm.InvoiceId" placeholder="Ë´ãËº∏?•ÁôºÁ•®Á∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ºÁ•®È°ûÂ?">
          <el-select v-model="queryForm.InvoiceType" placeholder="Ë´ãÈÅ∏?? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="Áµ±‰??ºÁ•®" value="01" />
            <el-option label="?ªÂ??ºÁ•®" value="02" />
            <el-option label="?∂Ê?" value="03" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ºÁ•®Âπ¥Ê?">
          <el-date-picker
            v-model="queryForm.InvoiceYm"
            type="month"
            placeholder="Ë´ãÈÅ∏?áÂπ¥??
            value-format="YYYYMM"
            clearable
          />
        </el-form-item>
        <el-form-item label="Áµ±‰?Á∑®Ë?">
          <el-input v-model="queryForm.TaxId" placeholder="Ë´ãËº∏?•Áµ±‰∏ÄÁ∑®Ë?" clearable />
        </el-form-item>
        <el-form-item label="?ÜÂÖ¨?∏‰ª£Á¢?>
          <el-input v-model="queryForm.SiteId" placeholder="Ë´ãËº∏?•Â??¨Âè∏‰ª?¢º" clearable />
        </el-form-item>
        <el-form-item label="B2BÊ®ôË?">
          <el-select v-model="queryForm.B2BFlag" placeholder="Ë´ãÈÅ∏?? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="?? value="Y" />
            <el-option label="?? value="N" />
          </el-select>
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="?üÁî®" value="A" />
            <el-option label="?úÁî®" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleCreate">?∞Â?</el-button>
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
        <el-table-column prop="InvoiceId" label="?ºÁ•®Á∑®Ë?" width="150" />
        <el-table-column prop="InvoiceType" label="?ºÁ•®È°ûÂ?" width="120">
          <template #default="{ row }">
            {{ getInvoiceTypeName(row.InvoiceType) }}
          </template>
        </el-table-column>
        <el-table-column prop="InvoiceYm" label="?ºÁ•®Âπ¥Ê?" width="120" />
        <el-table-column prop="Track" label="Â≠óË?" width="100" />
        <el-table-column prop="InvoiceNoB" label="?ºÁ•®?üÁ¢ºËµ? width="120" />
        <el-table-column prop="InvoiceNoE" label="?ºÁ•®?üÁ¢ºËø? width="120" />
        <el-table-column prop="TaxId" label="Áµ±‰?Á∑®Ë?" width="150" />
        <el-table-column prop="CompanyName" label="?¨Âè∏?çÁ®±" width="200" />
        <el-table-column prop="SiteId" label="?ÜÂÖ¨?∏‰ª£Á¢? width="120" />
        <el-table-column prop="B2BFlag" label="B2BÊ®ôË?" width="100">
          <template #default="{ row }">
            <el-tag :type="row.B2BFlag === 'Y' ? 'success' : 'info'">
              {{ row.B2BFlag === 'Y' ? '?? : '?? }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">?™Èô§</el-button>
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

    <!-- ?∞Â?/‰øÆÊîπÂ∞çË©±Ê°?-->
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
        <el-divider>?∫Êú¨Ë≥áÊ?</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ºÁ•®Á∑®Ë?" prop="InvoiceId">
              <el-input v-model="formData.InvoiceId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ºÁ•®È°ûÂ?" prop="InvoiceType">
              <el-select v-model="formData.InvoiceType" placeholder="Ë´ãÈÅ∏??>
                <el-option label="Áµ±‰??ºÁ•®" value="01" />
                <el-option label="?ªÂ??ºÁ•®" value="02" />
                <el-option label="?∂Ê?" value="03" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="?ºÁ•®Âπ¥‰ªΩ" prop="InvoiceYear">
              <el-input-number v-model="formData.InvoiceYear" :min="1900" :max="2100" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?ºÁ•®?à‰ªΩ" prop="InvoiceMonth">
              <el-input-number v-model="formData.InvoiceMonth" :min="1" :max="12" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Â≠óË?">
              <el-input v-model="formData.Track" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ºÁ•®?üÁ¢ºËµ?>
              <el-input v-model="formData.InvoiceNoB" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ºÁ•®?üÁ¢ºËø?>
              <el-input v-model="formData.InvoiceNoE" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ºÁ•®?ºÂ?‰ª??">
              <el-input v-model="formData.InvoiceFormat" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÜÂÖ¨?∏‰ª£Á¢?>
              <el-input v-model="formData.SiteId" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-divider>Á®ÖÁ?Ë≥áÊ?</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Áµ±‰?Á∑®Ë?">
              <el-input v-model="formData.TaxId" maxlength="8" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?¨Âè∏?çÁ®±">
              <el-input v-model="formData.CompanyName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?¨Âè∏?±Ê??çÁ®±">
              <el-input v-model="formData.CompanyNameEn" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?∞Â?">
              <el-input v-model="formData.Address" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="?éÂ?">
              <el-input v-model="formData.City" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?Ä??>
              <el-input v-model="formData.Zone" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="?µÈ??Ä??>
              <el-input v-model="formData.PostalCode" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ªË©±">
              <el-input v-model="formData.Phone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?≥Á?">
              <el-input v-model="formData.Fax" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?ªÂ??µ‰ª∂">
          <el-input v-model="formData.Email" type="email" />
        </el-form-item>

        <el-divider>?∂‰?Ë≥áÊ?</el-divider>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ØËÅØ">
              <el-input v-model="formData.SubCopy" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ØËÅØ??>
              <el-input v-model="formData.SubCopyValue" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="B2BÊ®ôË?" prop="B2BFlag">
              <el-radio-group v-model="formData.B2BFlag">
                <el-radio label="Y">??/el-radio>
                <el-radio label="N">??/el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?Ä?? prop="Status">
              <el-radio-group v-model="formData.Status">
                <el-radio label="A">?üÁî®</el-radio>
                <el-radio label="I">?úÁî®</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?ôË®ª">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmit">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { b2bInvoiceDataApi } from '@/api/invoiceSales'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  InvoiceId: '',
  InvoiceType: '',
  InvoiceYm: '',
  TaxId: '',
  SiteId: '',
  B2BFlag: '',
  Status: ''
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

// Â∞çË©±Ê°?
const dialogVisible = ref(false)
const dialogTitle = computed(() => isEdit.value ? '‰øÆÊîπB2B?ºÁ•®Ë≥áÊ?' : '?∞Â?B2B?ºÁ•®Ë≥áÊ?')
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
  B2BFlag: 'Y',
  Status: 'A',
  Notes: ''
})
const formRules = {
  InvoiceId: [{ required: true, message: 'Ë´ãËº∏?•ÁôºÁ•®Á∑®??, trigger: 'blur' }],
  InvoiceType: [{ required: true, message: 'Ë´ãÈÅ∏?áÁôºÁ•®È???, trigger: 'change' }],
  InvoiceYear: [
    { required: true, message: 'Ë´ãËº∏?•ÁôºÁ•®Âπ¥‰ª?, trigger: 'blur' },
    { type: 'number', min: 1900, max: 2100, message: 'Âπ¥‰ªΩÁØÑÂ???1900-2100', trigger: 'blur' }
  ],
  InvoiceMonth: [
    { required: true, message: 'Ë´ãËº∏?•ÁôºÁ•®Ê?‰ª?, trigger: 'blur' },
    { type: 'number', min: 1, max: 12, message: '?à‰ªΩÁØÑÂ???1-12', trigger: 'blur' }
  ],
  B2BFlag: [{ required: true, message: 'Ë´ãÈÅ∏?áB2BÊ®ôË?', trigger: 'change' }],
  Status: [{ required: true, message: 'Ë´ãÈÅ∏?áÁ???, trigger: 'change' }],
  TaxId: [
    { pattern: /^\d{8}$|^$/, message: 'Áµ±‰?Á∑®Ë?ÂøÖÈ???‰ΩçÊï∏Â≠?, trigger: 'blur' }
  ],
  Email: [
    { type: 'email', message: 'Ë´ãËº∏?•Ê≠£Á¢∫Á??ªÂ??µ‰ª∂?ºÂ?', trigger: 'blur' }
  ]
}
const currentTKey = ref(null)

// ??ÅΩÂπ¥‰ªΩ?åÊ?‰ªΩË??ñÔ??™Â??¥Êñ∞ InvoiceYm
watch([() => formData.InvoiceYear, () => formData.InvoiceMonth], ([year, month]) => {
  if (year && month) {
    formData.InvoiceYm = `${year}${String(month).padStart(2, '0')}`
  }
})

// ?•Ë©¢
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
      B2BFlag: queryForm.B2BFlag || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await b2bInvoiceDataApi.getB2BInvoices(params)
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
  queryForm.InvoiceId = ''
  queryForm.InvoiceType = ''
  queryForm.InvoiceYm = ''
  queryForm.TaxId = ''
  queryForm.SiteId = ''
  queryForm.B2BFlag = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// ?∞Â?
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
    B2BFlag: 'Y',
    Status: 'A',
    Notes: ''
  })
  dialogVisible.value = true
}

// ?•Á?
const handleView = async (row) => {
  await handleEdit(row)
}

// ‰øÆÊîπ
const handleEdit = async (row) => {
  try {
    const response = await b2bInvoiceDataApi.getB2BInvoice(row.TKey)
    if (response.data?.success) {
      isEdit.value = true
      currentTKey.value = row.TKey
      const data = response.data.data
      // Âæ?InvoiceYm Ëß??Âπ¥‰ªΩ?åÊ?‰ª?
      if (data.InvoiceYm && data.InvoiceYm.length === 6) {
        data.InvoiceYear = parseInt(data.InvoiceYm.substring(0, 4))
        data.InvoiceMonth = parseInt(data.InvoiceYm.substring(4, 6))
      }
      Object.assign(formData, data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '?•Ë©¢Â§±Ê?')
    }
  } catch (error) {
    ElMessage.error('?•Ë©¢Â§±Ê?Ôº? + error.message)
  }
}

// ?™Èô§
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§B2B?ºÁ•®Ë≥áÊ??éÔ?', 'Á¢∫Ë?', {
      type: 'warning'
    })
    const response = await b2bInvoiceDataApi.deleteB2BInvoice(row.TKey)
    if (response.data?.success) {
      ElMessage.success('?™Èô§?êÂ?')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '?™Èô§Â§±Ê?')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('?™Èô§Â§±Ê?Ôº? + error.message)
    }
  }
}

// ?ê‰∫§
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      // È©óË??ºÁ•®?üÁ¢º?Ä??
      if (formData.InvoiceNoB && formData.InvoiceNoE) {
        if (formData.InvoiceNoB > formData.InvoiceNoE) {
          ElMessage.error('?ºÁ•®?üÁ¢ºËµ∑Â??àÂ??ºÁ??ºÁôºÁ•®Ë?Á¢ºË?')
          return
        }
      }
      try {
        // Á¢∫‰? InvoiceYm Ê≠?¢∫
        if (formData.InvoiceYear && formData.InvoiceMonth) {
          formData.InvoiceYm = `${formData.InvoiceYear}${String(formData.InvoiceMonth).padStart(2, '0')}`
        }
        let response
        if (isEdit.value) {
          response = await b2bInvoiceDataApi.updateB2BInvoice(currentTKey.value, formData)
        } else {
          response = await b2bInvoiceDataApi.createB2BInvoice(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '‰øÆÊîπ?êÂ?' : '?∞Â??êÂ?')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '?ç‰?Â§±Ê?')
        }
      } catch (error) {
        ElMessage.error('?ç‰?Â§±Ê?Ôº? + error.message)
      }
    }
  })
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

// ?ñÂ??ºÁ•®È°ûÂ??çÁ®±
const getInvoiceTypeName = (type) => {
  const types = {
    '01': 'Áµ±‰??ºÁ•®',
    '02': '?ªÂ??ºÁ•®',
    '03': '?∂Ê?'
  }
  return types[type] || type
}

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.b2b-invoice-data {
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

