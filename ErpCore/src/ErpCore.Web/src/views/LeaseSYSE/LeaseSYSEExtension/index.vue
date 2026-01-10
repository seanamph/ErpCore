<template>
  <div class="lease-syse-extension-management">
    <div class="page-header">
      <h1>租賃擴展維護 (SYSE210-SYSE230)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="擴展編號">
          <el-input v-model="queryForm.ExtensionId" placeholder="請輸入擴展編號" clearable />
        </el-form-item>
        <el-form-item label="租賃編號">
          <el-input v-model="queryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
        </el-form-item>
        <el-form-item label="擴展類型">
          <el-select v-model="queryForm.ExtensionType" placeholder="請選擇擴展類型" clearable>
            <el-option label="特殊條件" value="CONDITION" />
            <el-option label="附加條款" value="TERM" />
            <el-option label="擴展設定" value="SETTING" />
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
      <el-table :data="tableData" v-loading="loading" border stripe style="width: 100%">
        <el-table-column prop="ExtensionId" label="擴展編號" width="150" />
        <el-table-column prop="LeaseId" label="租賃編號" width="150" />
        <el-table-column prop="ExtensionType" label="擴展類型" width="120" />
        <el-table-column prop="ExtensionName" label="擴展名稱" width="200" />
        <el-table-column prop="StartDate" label="開始日期" width="120" />
        <el-table-column prop="EndDate" label="結束日期" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">{{ getStatusText(row.Status) }}</el-tag>
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
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="1000px" :close-on-click-modal="false">
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="140px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="擴展編號" prop="ExtensionId">
              <el-input v-model="formData.ExtensionId" :disabled="isEdit" placeholder="請輸入擴展編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="租賃編號" prop="LeaseId">
              <el-input v-model="formData.LeaseId" placeholder="請輸入租賃編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="擴展類型" prop="ExtensionType">
              <el-select v-model="formData.ExtensionType" placeholder="請選擇擴展類型" style="width: 100%">
                <el-option label="特殊條件" value="CONDITION" />
                <el-option label="附加條款" value="TERM" />
                <el-option label="擴展設定" value="SETTING" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="擴展名稱" prop="ExtensionName">
              <el-input v-model="formData.ExtensionName" placeholder="請輸入擴展名稱" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="開始日期" prop="StartDate">
              <el-date-picker v-model="formData.StartDate" type="date" placeholder="請選擇開始日期" value-format="YYYY-MM-DD" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="EndDate">
              <el-date-picker v-model="formData.EndDate" type="date" placeholder="請選擇結束日期" value-format="YYYY-MM-DD" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="擴展值" prop="ExtensionValue">
          <el-input v-model="formData.ExtensionValue" type="textarea" :rows="4" placeholder="請輸入擴展值" />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
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
import { leaseApi } from '@/api/lease'

export default {
  name: 'LeaseSYSEExtensionManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    const queryForm = reactive({
      ExtensionId: '',
      LeaseId: '',
      ExtensionType: '',
      PageIndex: 1,
      PageSize: 20
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const formData = reactive({
      ExtensionId: '',
      LeaseId: '',
      ExtensionType: '',
      ExtensionName: '',
      ExtensionValue: '',
      StartDate: '',
      EndDate: '',
      Status: 'A',
      Memo: ''
    })

    const formRules = {
      ExtensionId: [{ required: true, message: '請輸入擴展編號', trigger: 'blur' }],
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      ExtensionType: [{ required: true, message: '請選擇擴展類型', trigger: 'change' }]
    }

    const dialogTitle = computed(() => (isEdit.value ? '修改租賃擴展' : '新增租賃擴展'))

    const loadData = async () => {
      loading.value = true
      try {
        const params = { ...queryForm, PageIndex: pagination.PageIndex, PageSize: pagination.PageSize }
        const response = await leaseApi.getLeaseExtensions(params)
        if (response.data?.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        loading.value = false
      }
    }

    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    const handleReset = () => {
      Object.assign(queryForm, { ExtensionId: '', LeaseId: '', ExtensionType: '' })
      handleSearch()
    }

    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, { ExtensionId: '', LeaseId: '', ExtensionType: '', ExtensionName: '', ExtensionValue: '', StartDate: '', EndDate: '', Status: 'A', Memo: '' })
    }

    const handleView = async (row) => {
      try {
        const response = await leaseApi.getLeaseExtension(row.ExtensionId)
        if (response.data?.Success) {
          Object.assign(formData, response.data.Data)
          isEdit.value = true
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    const handleEdit = async (row) => await handleView(row)

    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(`確定要刪除擴展「${row.ExtensionId}」嗎？`, '確認刪除', { confirmButtonText: '確定', cancelButtonText: '取消', type: 'warning' })
        await leaseApi.deleteLeaseExtension(row.ExtensionId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await leaseApi.updateLeaseExtension(formData.ExtensionId, formData)
          ElMessage.success('修改成功')
        } else {
          await leaseApi.createLeaseExtension(formData)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    const getStatusType = (status) => ({ 'A': 'success', 'I': 'danger' }[status] || 'info')
    const getStatusText = (status) => ({ 'A': '啟用', 'I': '停用' }[status] || status)

    onMounted(() => loadData())

    return { loading, dialogVisible, isEdit, formRef, tableData, queryForm, pagination, formData, formRules, dialogTitle, handleSearch, handleReset, handleCreate, handleView, handleEdit, handleDelete, handleSubmit, handleSizeChange, handlePageChange, getStatusType, getStatusText }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-syse-extension-management {
  .page-header {
    margin-bottom: 20px;
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
    }
  }
  .search-card {
    margin-bottom: 20px;
    .search-form .el-form-item {
      margin-bottom: 0;
    }
  }
}
</style>

