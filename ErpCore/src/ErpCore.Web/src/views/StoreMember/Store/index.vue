<template>
  <div class="store-data">
    <div class="page-header">
      <h1>商店資料維護 (SYS3130-SYS3160)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="商店編號">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入商店編號" clearable />
        </el-form-item>
        <el-form-item label="商店名稱">
          <el-input v-model="queryForm.ShopName" placeholder="請輸入商店名稱" clearable />
        </el-form-item>
        <el-form-item label="商店類型">
          <el-input v-model="queryForm.ShopType" placeholder="請輸入商店類型" clearable />
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
            <el-form-item label="商店編號" prop="ShopId">
              <el-input v-model="formData.ShopId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="商店名稱" prop="ShopName">
              <el-input v-model="formData.ShopName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="英文名稱">
              <el-input v-model="formData.ShopNameEn" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="商店類型">
              <el-input v-model="formData.ShopType" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="地址">
          <el-input v-model="formData.Address" />
        </el-form-item>
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="電子郵件">
              <el-input v-model="formData.Email" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="店長姓名">
              <el-input v-model="formData.ManagerName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="店長電話">
              <el-input v-model="formData.ManagerPhone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="開店日期">
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
            <el-form-item label="關店日期">
              <el-date-picker
                v-model="formData.CloseDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
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
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="樓層代碼">
              <el-input v-model="formData.FloorId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="區域代碼">
              <el-input v-model="formData.AreaId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="POS啟用">
              <el-switch v-model="formData.PosEnabled" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="POS系統代碼">
              <el-input v-model="formData.PosSystemId" :disabled="!formData.PosEnabled" />
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
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { storeApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  ShopId: '',
  ShopName: '',
  ShopType: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改商店資料' : '新增商店資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  ShopId: '',
  ShopName: '',
  ShopNameEn: '',
  ShopType: '',
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
  FloorId: '',
  AreaId: '',
  PosEnabled: false,
  PosSystemId: '',
  Notes: ''
})
const formRules = {
  ShopId: [{ required: true, message: '請輸入商店編號', trigger: 'blur' }],
  ShopName: [{ required: true, message: '請輸入商店名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentShopId = ref(null)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      ShopId: queryForm.ShopId || undefined,
      ShopName: queryForm.ShopName || undefined,
      ShopType: queryForm.ShopType || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await storeApi.getStores(params)
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
  queryForm.ShopId = ''
  queryForm.ShopName = ''
  queryForm.ShopType = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentShopId.value = null
  Object.assign(formData, {
    ShopId: '',
    ShopName: '',
    ShopNameEn: '',
    ShopType: '',
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
    FloorId: '',
    AreaId: '',
    PosEnabled: false,
    PosSystemId: '',
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
    const response = await storeApi.getStore(row.ShopId)
    if (response.data?.success) {
      isEdit.value = true
      currentShopId.value = row.ShopId
      Object.assign(formData, response.data.data)
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
    await ElMessageBox.confirm('確定要刪除此商店資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await storeApi.deleteStore(row.ShopId)
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
          response = await storeApi.updateStore(currentShopId.value, formData)
        } else {
          response = await storeApi.createStore(formData)
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

.store-data {
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

