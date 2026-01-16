<template>
  <div class="sysl145-query">
    <div class="page-header">
      <h1>業務報表查詢作業 (SYSL145)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="店別">
          <el-input
            v-model="queryForm.SiteId"
            placeholder="請輸入店別代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="類型">
          <el-input
            v-model="queryForm.Type"
            placeholder="請輸入類型"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="ID">
          <el-input
            v-model="queryForm.Id"
            placeholder="請輸入ID"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="使用者">
          <el-input
            v-model="queryForm.UserId"
            placeholder="請輸入使用者編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 200px">
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="danger" @click="handleBatchDelete" :disabled="selectedRows.length === 0">批次刪除</el-button>
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="TKey" label="主鍵" width="100" />
        <el-table-column prop="SiteId" label="店別代碼" width="120" />
        <el-table-column prop="SiteName" label="店別名稱" width="150" />
        <el-table-column prop="Type" label="類型" width="120" />
        <el-table-column prop="TypeName" label="類型名稱" width="150" />
        <el-table-column prop="Id" label="ID" width="120" />
        <el-table-column prop="UserId" label="使用者編號" width="120" />
        <el-table-column prop="UserName" label="使用者名稱" width="150" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDate(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="UpdatedAt" label="更新時間" width="180">
          <template #default="{ row }">
            {{ formatDate(row.UpdatedAt) }}
          </template>
        </el-table-column>
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
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="店別代碼" prop="SiteId">
              <el-input v-model="form.SiteId" placeholder="請輸入店別代碼" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="類型" prop="Type">
              <el-input v-model="form.Type" placeholder="請輸入類型" :disabled="isEdit" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="ID" prop="Id">
              <el-input v-model="form.Id" placeholder="請輸入ID" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="使用者編號" prop="UserId">
              <el-input v-model="form.UserId" placeholder="請輸入使用者編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用者名稱" prop="UserName">
              <el-input v-model="form.UserName" placeholder="請輸入使用者名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Notes">
              <el-input
                v-model="form.Notes"
                type="textarea"
                :rows="3"
                placeholder="請輸入備註"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="submitting">確定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { businessReportManagementApi } from '@/api/businessReport'

export default {
  name: 'SYSL145Query',
  setup() {
    const loading = ref(false)
    const submitting = ref(false)
    const dialogVisible = ref(false)
    const dialogTitle = ref('新增業務報表管理')
    const isEdit = ref(false)
    const currentTKey = ref(null)
    const selectedRows = ref([])
    const formRef = ref(null)

    const tableData = ref([])
    const queryForm = reactive({
      SiteId: '',
      Type: '',
      Id: '',
      UserId: '',
      Status: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const form = reactive({
      SiteId: '',
      Type: '',
      Id: '',
      UserId: '',
      UserName: '',
      Status: 'A',
      Notes: ''
    })

    const rules = {
      SiteId: [{ required: true, message: '請輸入店別代碼', trigger: 'blur' }],
      Type: [{ required: true, message: '請輸入類型', trigger: 'blur' }],
      Id: [{ required: true, message: '請輸入ID', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          SortField: 'TKey',
          SortOrder: 'DESC',
          ...queryForm
        }
        const response = await businessReportManagementApi.getBusinessReportManagements(params)
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
        SiteId: '',
        Type: '',
        Id: '',
        UserId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      dialogTitle.value = '新增業務報表管理'
      isEdit.value = false
      currentTKey.value = null
      Object.assign(form, {
        SiteId: '',
        Type: '',
        Id: '',
        UserId: '',
        UserName: '',
        Status: 'A',
        Notes: ''
      })
      dialogVisible.value = true
    }

    // 修改
    const handleEdit = async (row) => {
      dialogTitle.value = '修改業務報表管理'
      isEdit.value = true
      currentTKey.value = row.TKey
      try {
        const response = await businessReportManagementApi.getBusinessReportManagement(row.TKey)
        if (response.Data) {
          Object.assign(form, {
            SiteId: response.Data.SiteId || '',
            Type: response.Data.Type || '',
            Id: response.Data.Id || '',
            UserId: response.Data.UserId || '',
            UserName: response.Data.UserName || '',
            Status: response.Data.Status || 'A',
            Notes: response.Data.Notes || ''
          })
        }
        dialogVisible.value = true
      } catch (error) {
        ElMessage.error('載入資料失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除業務報表管理資料嗎？\n店別: ${row.SiteId}, 類型: ${row.Type}, ID: ${row.Id}`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await businessReportManagementApi.deleteBusinessReportManagement(row.TKey)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 批次刪除
    const handleBatchDelete = async () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('請選擇要刪除的資料')
        return
      }
      try {
        await ElMessageBox.confirm(
          `確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`,
          '確認批次刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        const tKeys = selectedRows.value.map(row => row.TKey)
        await businessReportManagementApi.batchDeleteBusinessReportManagement({ TKeys: tKeys })
        ElMessage.success('批次刪除成功')
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        
        // 檢查重複資料
        const checkResponse = await businessReportManagementApi.checkDuplicate({
          SiteId: form.SiteId,
          Type: form.Type,
          Id: form.Id,
          ExcludeTKey: isEdit.value ? currentTKey.value : null
        })
        
        if (checkResponse.Data && checkResponse.Data.IsDuplicate) {
          ElMessage.error(`店別+類型+ID已存在，現有使用者: ${checkResponse.Data.ExistingRecord?.UserName || ''}`)
          return
        }

        submitting.value = true
        if (isEdit.value) {
          await businessReportManagementApi.updateBusinessReportManagement(currentTKey.value, form)
          ElMessage.success('修改成功')
        } else {
          await businessReportManagementApi.createBusinessReportManagement(form)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      } finally {
        submitting.value = false
      }
    }

    // 關閉對話框
    const handleDialogClose = () => {
      formRef.value?.resetFields()
    }

    // 選擇變更
    const handleSelectionChange = (selection) => {
      selectedRows.value = selection
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

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      submitting,
      dialogVisible,
      dialogTitle,
      isEdit,
      tableData,
      queryForm,
      pagination,
      form,
      rules,
      formRef,
      selectedRows,
      formatDate,
      handleSearch,
      handleReset,
      handleCreate,
      handleEdit,
      handleDelete,
      handleBatchDelete,
      handleSubmit,
      handleDialogClose,
      handleSelectionChange,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.sysl145-query {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
  }
}
</style>
