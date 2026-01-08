<template>
  <div class="prospect-master-list">
    <div class="page-header">
      <h1>潛客主檔維護作業 (SYSC165)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="主檔代碼">
          <el-input v-model="queryForm.MasterId" placeholder="請輸入主檔代碼" clearable />
        </el-form-item>
        <el-form-item label="主檔名稱">
          <el-input v-model="queryForm.MasterName" placeholder="請輸入主檔名稱" clearable />
        </el-form-item>
        <el-form-item label="主檔類型">
          <el-select v-model="queryForm.MasterType" placeholder="請選擇主檔類型" clearable>
            <el-option label="公司" value="COMPANY" />
            <el-option label="個人" value="INDIVIDUAL" />
            <el-option label="其他" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="有效" value="ACTIVE" />
            <el-option label="無效" value="INACTIVE" />
            <el-option label="已歸檔" value="ARCHIVED" />
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
        <el-table-column prop="MasterId" label="主檔代碼" width="150" />
        <el-table-column prop="MasterName" label="主檔名稱" width="200" />
        <el-table-column prop="MasterType" label="主檔類型" width="120">
          <template #default="{ row }">
            {{ formatMasterType(row.MasterType) }}
          </template>
        </el-table-column>
        <el-table-column prop="Category" label="分類" width="120" />
        <el-table-column prop="Industry" label="產業別" width="120" />
        <el-table-column prop="BusinessType" label="業種" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ formatStatus(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Source" label="來源" width="120" />
        <el-table-column prop="ContactPerson" label="聯絡人" width="120" />
        <el-table-column prop="ContactTel" label="聯絡電話" width="150" />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
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
        :page-sizes="[10, 20, 50, 100]"
        :total="pagination.TotalCount"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; text-align: right;"
      />
    </el-card>

    <!-- 新增/修改對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="80%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="主檔代碼" prop="MasterId">
              <el-input v-model="formData.MasterId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="主檔名稱" prop="MasterName">
              <el-input v-model="formData.MasterName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="主檔類型" prop="MasterType">
              <el-select v-model="formData.MasterType" placeholder="請選擇主檔類型" clearable>
                <el-option label="公司" value="COMPANY" />
                <el-option label="個人" value="INDIVIDUAL" />
                <el-option label="其他" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態">
                <el-option label="有效" value="ACTIVE" />
                <el-option label="無效" value="INACTIVE" />
                <el-option label="已歸檔" value="ARCHIVED" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="分類">
              <el-input v-model="formData.Category" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="產業別">
              <el-input v-model="formData.Industry" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="業種">
              <el-input v-model="formData.BusinessType" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="來源">
              <el-select v-model="formData.Source" placeholder="請選擇來源" clearable>
                <el-option label="推薦" value="REFERRAL" />
                <el-option label="廣告" value="ADVERTISEMENT" />
                <el-option label="展覽" value="EXHIBITION" />
                <el-option label="其他" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="優先順序">
              <el-input-number v-model="formData.Priority" :min="0" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="聯絡人">
              <el-input v-model="formData.ContactPerson" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="聯絡電話">
              <el-input v-model="formData.ContactTel" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="電子郵件">
              <el-input v-model="formData.ContactEmail" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="聯絡地址">
          <el-input v-model="formData.ContactAddress" />
        </el-form-item>
        <el-form-item label="網站">
          <el-input v-model="formData.Website" />
        </el-form-item>
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

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { prospectMasterApi } from '@/api/recruitment'

export default {
  name: 'ProspectMasterList',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      MasterId: '',
      MasterName: '',
      MasterType: '',
      Category: '',
      Industry: '',
      BusinessType: '',
      Status: '',
      Source: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      MasterId: '',
      MasterName: '',
      MasterType: '',
      Category: '',
      Industry: '',
      BusinessType: '',
      Status: 'ACTIVE',
      Priority: 0,
      Source: '',
      ContactPerson: '',
      ContactTel: '',
      ContactEmail: '',
      ContactAddress: '',
      Website: '',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      MasterId: [{ required: true, message: '請輸入主檔代碼', trigger: 'blur' }],
      MasterName: [{ required: true, message: '請輸入主檔名稱', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化主檔類型
    const formatMasterType = (type) => {
      const types = {
        COMPANY: '公司',
        INDIVIDUAL: '個人',
        OTHER: '其他'
      }
      return types[type] || type
    }

    // 格式化狀態
    const formatStatus = (status) => {
      const statuses = {
        ACTIVE: '有效',
        INACTIVE: '無效',
        ARCHIVED: '已歸檔'
      }
      return statuses[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        ACTIVE: 'success',
        INACTIVE: 'danger',
        ARCHIVED: 'info'
      }
      return types[status] || ''
    }

    // 格式化日期時間
    const formatDateTime = (dateTime) => {
      if (!dateTime) return ''
      return new Date(dateTime).toLocaleString('zh-TW')
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await prospectMasterApi.getProspectMasters(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    // 查詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm, {
        MasterId: '',
        MasterName: '',
        MasterType: '',
        Category: '',
        Industry: '',
        BusinessType: '',
        Status: '',
        Source: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        MasterId: '',
        MasterName: '',
        MasterType: '',
        Category: '',
        Industry: '',
        BusinessType: '',
        Status: 'ACTIVE',
        Priority: 0,
        Source: '',
        ContactPerson: '',
        ContactTel: '',
        ContactEmail: '',
        ContactAddress: '',
        Website: '',
        Notes: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await prospectMasterApi.getProspectMaster(row.MasterId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, response.Data)
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此潛客主檔嗎？', '確認', {
          type: 'warning'
        })
        await prospectMasterApi.deleteProspectMaster(row.MasterId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await prospectMasterApi.updateProspectMaster(formData.MasterId, formData)
              ElMessage.success('修改成功')
            } else {
              await prospectMasterApi.createProspectMaster(formData)
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 計算對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改潛客主檔' : '新增潛客主檔'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      formatMasterType,
      formatStatus,
      getStatusType,
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
.prospect-master-list {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>

