<template>
  <div class="lease-process-management">
    <div class="page-header">
      <h1>租賃處理作業 (SYS8B50-SYS8B90)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="處理編號">
          <el-input v-model="queryForm.ProcessId" placeholder="請輸入處理編號" clearable />
        </el-form-item>
        <el-form-item label="租賃編號">
          <el-input v-model="queryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
        </el-form-item>
        <el-form-item label="處理類型">
          <el-select v-model="queryForm.ProcessType" placeholder="請選擇處理類型" clearable>
            <el-option label="續約" value="RENEW" />
            <el-option label="解約" value="TERMINATE" />
            <el-option label="調整" value="ADJUST" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="待處理" value="P" />
            <el-option label="處理中" value="I" />
            <el-option label="已完成" value="C" />
            <el-option label="已取消" value="X" />
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
        <el-table-column prop="ProcessId" label="處理編號" width="150" />
        <el-table-column prop="LeaseId" label="租賃編號" width="150" />
        <el-table-column prop="ProcessType" label="處理類型" width="120">
          <template #default="{ row }">
            {{ getProcessTypeText(row.ProcessType) }}
          </template>
        </el-table-column>
        <el-table-column prop="ProcessDate" label="處理日期" width="120" />
        <el-table-column prop="ProcessAmount" label="處理金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.ProcessAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'C'">修改</el-button>
            <el-button type="success" size="small" @click="handleExecute(row)" :disabled="row.Status !== 'P'">執行</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'C'">刪除</el-button>
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
      width="1000px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="處理編號" prop="ProcessId">
              <el-input v-model="formData.ProcessId" :disabled="isEdit" placeholder="請輸入處理編號" />
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
            <el-form-item label="處理類型" prop="ProcessType">
              <el-select v-model="formData.ProcessType" placeholder="請選擇處理類型" style="width: 100%">
                <el-option label="續約" value="RENEW" />
                <el-option label="解約" value="TERMINATE" />
                <el-option label="調整" value="ADJUST" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="處理日期" prop="ProcessDate">
              <el-date-picker
                v-model="formData.ProcessDate"
                type="date"
                placeholder="請選擇處理日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="處理金額" prop="ProcessAmount">
              <el-input-number v-model="formData.ProcessAmount" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="待處理" value="P" />
                <el-option label="處理中" value="I" />
                <el-option label="已完成" value="C" />
                <el-option label="已取消" value="X" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
          </el-col>
        </el-row>
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
  name: 'LeaseProcessManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ProcessId: '',
      LeaseId: '',
      ProcessType: '',
      Status: '',
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
      ProcessId: '',
      LeaseId: '',
      ProcessType: '',
      ProcessDate: '',
      ProcessAmount: 0,
      Status: 'P',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      ProcessId: [{ required: true, message: '請輸入處理編號', trigger: 'blur' }],
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      ProcessType: [{ required: true, message: '請選擇處理類型', trigger: 'change' }],
      ProcessDate: [{ required: true, message: '請選擇處理日期', trigger: 'change' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改租賃處理' : '新增租賃處理'
    })

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await leaseApi.getLeaseProcesses(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃處理列表失敗:', error)
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
        ProcessId: '',
        LeaseId: '',
        ProcessType: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        ProcessId: '',
        LeaseId: '',
        ProcessType: '',
        ProcessDate: '',
        ProcessAmount: 0,
        Status: 'P',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await leaseApi.getLeaseProcess(row.ProcessId)
        if (response.data && response.data.Success) {
          Object.assign(formData, response.data.Data)
          isEdit.value = true
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃處理失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 執行
    const handleExecute = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要執行租賃處理「${row.ProcessId}」嗎？`,
          '確認執行',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        const executeData = {
          ExecuteUserId: 'current_user', // 應該從登入資訊取得
          ExecuteDate: new Date().toISOString()
        }
        await leaseApi.executeLeaseProcess(row.ProcessId, executeData)
        ElMessage.success('執行成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('執行租賃處理失敗:', error)
          ElMessage.error('執行失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除租賃處理「${row.ProcessId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseApi.deleteLeaseProcess(row.ProcessId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除租賃處理失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await leaseApi.updateLeaseProcess(formData.ProcessId, formData)
          ElMessage.success('修改成功')
        } else {
          await leaseApi.createLeaseProcess(formData)
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

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 格式化貨幣
    const formatCurrency = (value) => {
      if (!value) return '0.00'
      return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 取得處理類型文字
    const getProcessTypeText = (type) => {
      const typeMap = {
        'RENEW': '續約',
        'TERMINATE': '解約',
        'ADJUST': '調整'
      }
      return typeMap[type] || type
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'P': 'info',
        'I': 'warning',
        'C': 'success',
        'X': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'P': '待處理',
        'I': '處理中',
        'C': '已完成',
        'X': '已取消'
      }
      return statusMap[status] || status
    }

    // 初始化
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
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleExecute,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      formatCurrency,
      getProcessTypeText,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-process-management {
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
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-tag {
        font-weight: 500;
      }
    }
  }
}
</style>

