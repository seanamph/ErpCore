<template>
  <div class="store-management">
    <div class="page-header">
      <h1>?ÜÂ?Ë≥áÊ?Á∂≠Ë≠∑ (SYS6110-SYS6140)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ÜÂ?Á∑®Ë?">
          <el-input v-model="queryForm.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫óÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ÜÂ??çÁ®±">
          <el-input v-model="queryForm.ShopName" placeholder="Ë´ãËº∏?•Â?Â∫óÂ?Á®? clearable />
        </el-form-item>
        <el-form-item label="?ÜÂ?È°ûÂ?">
          <el-input v-model="queryForm.ShopType" placeholder="Ë´ãËº∏?•Â?Â∫óÈ??? clearable />
        </el-form-item>
        <el-form-item label="?éÂ?">
          <el-input v-model="queryForm.City" placeholder="Ë´ãËº∏?•Â?Â∏? clearable />
        </el-form-item>
        <el-form-item label="Ê®ìÂ±§‰ª?¢º">
          <el-input v-model="queryForm.FloorId" placeholder="Ë´ãËº∏?•Ê?Â±§‰ª£Á¢? clearable />
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ÜÂ?Á∑®Ë?" prop="ShopId">
              <el-input v-model="formData.ShopId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÜÂ??çÁ®±" prop="ShopName">
              <el-input v-model="formData.ShopName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?±Ê??çÁ®±">
              <el-input v-model="formData.ShopNameEn" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ÜÂ?È°ûÂ?">
              <el-input v-model="formData.ShopType" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Ê®ìÂ±§‰ª?¢º">
              <el-input v-model="formData.FloorId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Ê®ìÂ±§?çÁ®±">
              <el-input v-model="formData.FloorName" disabled />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?∞Â?">
          <el-input v-model="formData.Address" />
        </el-form-item>
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?ªÂ??µ‰ª∂">
              <el-input v-model="formData.Email" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Â∫óÈï∑ÂßìÂ?">
              <el-input v-model="formData.ManagerName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Â∫óÈï∑?ªË©±">
              <el-input v-model="formData.ManagerPhone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?ãÂ??•Ê?">
              <el-date-picker
                v-model="formData.OpenDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?úÂ??•Ê?">
              <el-date-picker
                v-model="formData.CloseDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="POS?üÁî®">
              <el-switch v-model="formData.PosEnabled" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="POSÁ≥ªÁµ±‰ª?¢º">
              <el-input v-model="formData.PosSystemId" :disabled="!formData.PosEnabled" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="POSÁµÇÁ´Ø‰ª?¢º">
              <el-input v-model="formData.PosTerminalId" :disabled="!formData.PosEnabled" />
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
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { storeManagementApi } from '@/api/storeFloor'

// ?•Ë©¢Ë°®ÂñÆ
const queryForm = reactive({
  ShopId: '',
  ShopName: '',
  ShopType: '',
  City: '',
  FloorId: '',
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
const dialogTitle = computed(() => isEdit.value ? '‰øÆÊîπ?ÜÂ?Ë≥áÊ?' : '?∞Â??ÜÂ?Ë≥áÊ?')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  ShopId: '',
  ShopName: '',
  ShopNameEn: '',
  ShopType: '',
  FloorId: '',
  FloorName: '',
  Address: '',
  City: '',
  Zone: '',
  PostalCode: '',
  Phone: '',
  Fax: '',
  Email: '',
  ManagerName: '',
  ManagerPhone: '',
  OpenDate: null,
  CloseDate: null,
  Status: 'A',
  PosEnabled: false,
  PosSystemId: '',
  PosTerminalId: '',
  Notes: ''
})
const formRules = {
  ShopId: [{ required: true, message: 'Ë´ãËº∏?•Â?Â∫óÁ∑®??, trigger: 'blur' }],
  ShopName: [{ required: true, message: 'Ë´ãËº∏?•Â?Â∫óÂ?Á®?, trigger: 'blur' }],
  Status: [{ required: true, message: 'Ë´ãÈÅ∏?áÁ???, trigger: 'change' }]
}
const currentShopId = ref(null)

// ?•Ë©¢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ShopId: queryForm.ShopId || undefined,
      ShopName: queryForm.ShopName || undefined,
      ShopType: queryForm.ShopType || undefined,
      City: queryForm.City || undefined,
      FloorId: queryForm.FloorId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await storeManagementApi.getShopFloors(params)
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
  queryForm.FloorId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// ?∞Â?
const handleCreate = () => {
  isEdit.value = false
  currentShopId.value = null
  Object.assign(formData, {
    ShopId: '',
    ShopName: '',
    ShopNameEn: '',
    ShopType: '',
    FloorId: '',
    FloorName: '',
    Address: '',
    City: '',
    Zone: '',
    PostalCode: '',
    Phone: '',
    Fax: '',
    Email: '',
    ManagerName: '',
    ManagerPhone: '',
    OpenDate: null,
    CloseDate: null,
    Status: 'A',
    PosEnabled: false,
    PosSystemId: '',
    PosTerminalId: '',
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
    const response = await storeManagementApi.getShopFloor(row.ShopId)
    if (response.data?.success) {
      isEdit.value = true
      currentShopId.value = row.ShopId
      Object.assign(formData, response.data.data)
      // ?ïÁ??•Ê??ºÂ?
      if (formData.OpenDate) {
        formData.OpenDate = formData.OpenDate.split('T')[0]
      }
      if (formData.CloseDate) {
        formData.CloseDate = formData.CloseDate.split('T')[0]
      }
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
    await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?ÜÂ?Ë≥áÊ??éÔ?', 'Á¢∫Ë?', {
      type: 'warning'
    })
    const response = await storeManagementApi.deleteShopFloor(row.ShopId)
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
      try {
        let response
        if (isEdit.value) {
          response = await storeManagementApi.updateShopFloor(currentShopId.value, formData)
        } else {
          response = await storeManagementApi.createShopFloor(formData)
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

// ?ùÂ???
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.store-management {
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

