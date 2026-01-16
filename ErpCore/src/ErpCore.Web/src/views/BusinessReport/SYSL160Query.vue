<template>
  <div class="sysl160-query">
    <div class="page-header">
      <h1>業務報表列印明細作業 (SYSL160)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="報表列印ID">
          <el-input-number
            v-model="queryForm.PrintId"
            placeholder="請輸入報表列印ID"
            :min="1"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="請假代碼">
          <el-input
            v-model="queryForm.LeaveId"
            placeholder="請輸入請假代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="動作事件">
          <el-input
            v-model="queryForm.ActEvent"
            placeholder="請輸入動作事件"
            clearable
            style="width: 200px"
          />
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
        <el-table-column prop="PrintId" label="報表列印ID" width="120" />
        <el-table-column prop="LeaveId" label="請假代碼" width="120">
          <template #default="{ row }">
            <el-select
              v-model="row.LeaveId"
              placeholder="請選擇"
              clearable
              filterable
              @change="handleLeaveIdChange(row)"
              style="width: 100%"
            >
              <el-option
                v-for="item in leaveTypeList"
                :key="item.LeaveId"
                :label="item.LeaveName"
                :value="item.LeaveId"
              />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="LeaveName" label="請假名稱" width="150" />
        <el-table-column prop="ActEvent" label="動作事件" width="150">
          <template #default="{ row }">
            <el-input
              v-model="row.ActEvent"
              placeholder="請輸入動作事件"
              @change="handleActEventChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="DeductionQty" label="扣款數量" width="120">
          <template #default="{ row }">
            <el-input-number
              v-model="row.DeductionQty"
              :precision="2"
              :min="0"
              :disabled="!row.ActEvent || row.DeductionQtyDefaultEmpty === 'Y'"
              @change="handleDeductionQtyChange(row)"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column prop="DeductionQtyDefaultEmpty" label="扣款數量預設為空" width="150">
          <template #default="{ row }">
            <el-switch
              v-model="row.DeductionQtyDefaultEmpty"
              active-value="Y"
              inactive-value="N"
              @change="handleDeductionQtyDefaultEmptyChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDate(row.CreatedAt) }}
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
      width="600px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="160px">
        <el-form-item label="報表列印ID" prop="PrintId">
          <el-input-number
            v-model="form.PrintId"
            placeholder="請輸入報表列印ID"
            :min="1"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="請假代碼" prop="LeaveId">
          <el-select
            v-model="form.LeaveId"
            placeholder="請選擇請假代碼"
            clearable
            filterable
            @change="handleLeaveIdSelectChange"
            style="width: 100%"
          >
            <el-option
              v-for="item in leaveTypeList"
              :key="item.LeaveId"
              :label="item.LeaveName"
              :value="item.LeaveId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="請假名稱" prop="LeaveName">
          <el-input v-model="form.LeaveName" placeholder="請輸入請假名稱" />
        </el-form-item>
        <el-form-item label="動作事件" prop="ActEvent">
          <el-input v-model="form.ActEvent" placeholder="請輸入動作事件" />
        </el-form-item>
        <el-form-item label="扣款數量" prop="DeductionQty">
          <el-input-number
            v-model="form.DeductionQty"
            :precision="2"
            :min="0"
            :disabled="!form.ActEvent || form.DeductionQtyDefaultEmpty === 'Y'"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="扣款數量預設為空" prop="DeductionQtyDefaultEmpty">
          <el-switch
            v-model="form.DeductionQtyDefaultEmpty"
            active-value="Y"
            inactive-value="N"
            @change="handleDeductionQtyDefaultEmptyChangeInDialog"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
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
import { businessReportPrintDetailApi } from '@/api/businessReport'

export default {
  name: 'SYSL160Query',
  setup() {
    const loading = ref(false)
    const submitting = ref(false)
    const dialogVisible = ref(false)
    const dialogTitle = ref('新增業務報表列印明細')
    const isEdit = ref(false)
    const currentTKey = ref(null)
    const selectedRows = ref([])
    const formRef = ref(null)
    const leaveTypeList = ref([])

    const tableData = ref([])
    const queryForm = reactive({
      PrintId: null,
      LeaveId: '',
      ActEvent: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const form = reactive({
      PrintId: null,
      LeaveId: '',
      LeaveName: '',
      ActEvent: '',
      DeductionQty: null,
      DeductionQtyDefaultEmpty: 'N',
      Status: '1'
    })

    const rules = {
      PrintId: [{ required: true, message: '請輸入報表列印ID', trigger: 'blur' }],
      LeaveId: [{ required: false, message: '請選擇請假代碼', trigger: 'change' }],
      ActEvent: [{ required: false, message: '請輸入動作事件', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      return status === '1' ? 'success' : 'danger'
    }

    // 載入請假類型列表
    const loadLeaveTypeList = async () => {
      try {
        // 這裡應該調用獲取請假類型列表的 API
        // 暫時使用空陣列，實際應該從 API 獲取
        leaveTypeList.value = []
      } catch (error) {
        console.error('載入請假類型列表失敗:', error)
      }
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          ...queryForm
        }
        const response = await businessReportPrintDetailApi.getBusinessReportPrintDetails(params)
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
        PrintId: null,
        LeaveId: '',
        ActEvent: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      dialogTitle.value = '新增業務報表列印明細'
      isEdit.value = false
      currentTKey.value = null
      Object.assign(form, {
        PrintId: null,
        LeaveId: '',
        LeaveName: '',
        ActEvent: '',
        DeductionQty: null,
        DeductionQtyDefaultEmpty: 'N',
        Status: '1'
      })
      dialogVisible.value = true
    }

    // 修改
    const handleEdit = async (row) => {
      dialogTitle.value = '修改業務報表列印明細'
      isEdit.value = true
      currentTKey.value = row.TKey
      try {
        const response = await businessReportPrintDetailApi.getBusinessReportPrintDetail(row.TKey)
        if (response.Data) {
          Object.assign(form, {
            PrintId: response.Data.PrintId || null,
            LeaveId: response.Data.LeaveId || '',
            LeaveName: response.Data.LeaveName || '',
            ActEvent: response.Data.ActEvent || '',
            DeductionQty: response.Data.DeductionQty || null,
            DeductionQtyDefaultEmpty: response.Data.DeductionQtyDefaultEmpty || 'N',
            Status: response.Data.Status || '1'
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
          `確定要刪除業務報表列印明細資料嗎？\n主鍵: ${row.TKey}`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await businessReportPrintDetailApi.deleteBusinessReportPrintDetail(row.TKey)
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
        const deleteTKeys = selectedRows.value.map(row => row.TKey)
        const response = await businessReportPrintDetailApi.batchProcess({
          CreateItems: [],
          UpdateItems: [],
          DeleteTKeys: deleteTKeys
        })
        if (response.Data) {
          ElMessage.success(`批次刪除成功: ${response.Data.DeleteCount || selectedRows.value.length} 筆`)
        }
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 請假代碼變更（表格中）
    const handleLeaveIdChange = (row) => {
      const leaveType = leaveTypeList.value.find(item => item.LeaveId === row.LeaveId)
      if (leaveType) {
        row.LeaveName = leaveType.LeaveName
      }
      // 自動儲存
      handleQuickSave(row)
    }

    // 請假代碼變更（對話框中）
    const handleLeaveIdSelectChange = (value) => {
      const leaveType = leaveTypeList.value.find(item => item.LeaveId === value)
      if (leaveType) {
        form.LeaveName = leaveType.LeaveName
      }
    }

    // 動作事件變更
    const handleActEventChange = (row) => {
      if (!row.ActEvent) {
        row.DeductionQty = null
      }
      // 自動儲存
      handleQuickSave(row)
    }

    // 扣款數量變更
    const handleDeductionQtyChange = (row) => {
      // 自動儲存
      handleQuickSave(row)
    }

    // 扣款數量預設為空變更（表格中）
    const handleDeductionQtyDefaultEmptyChange = (row) => {
      if (row.DeductionQtyDefaultEmpty === 'Y') {
        row.DeductionQty = null
      }
      // 自動儲存
      handleQuickSave(row)
    }

    // 扣款數量預設為空變更（對話框中）
    const handleDeductionQtyDefaultEmptyChangeInDialog = () => {
      if (form.DeductionQtyDefaultEmpty === 'Y') {
        form.DeductionQty = null
      }
    }

    // 快速儲存（表格中直接編輯）
    const handleQuickSave = async (row) => {
      try {
        const updateDto = {
          LeaveId: row.LeaveId,
          LeaveName: row.LeaveName,
          ActEvent: row.ActEvent,
          DeductionQty: row.DeductionQty,
          DeductionQtyDefaultEmpty: row.DeductionQtyDefaultEmpty
        }
        await businessReportPrintDetailApi.updateBusinessReportPrintDetail(row.TKey, updateDto)
        // 不顯示成功訊息，避免頻繁提示
      } catch (error) {
        ElMessage.error('儲存失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        submitting.value = true
        if (isEdit.value) {
          const updateDto = {
            LeaveId: form.LeaveId,
            LeaveName: form.LeaveName,
            ActEvent: form.ActEvent,
            DeductionQty: form.DeductionQty,
            DeductionQtyDefaultEmpty: form.DeductionQtyDefaultEmpty
          }
          await businessReportPrintDetailApi.updateBusinessReportPrintDetail(currentTKey.value, updateDto)
          ElMessage.success('修改成功')
        } else {
          const createDto = {
            PrintId: form.PrintId,
            LeaveId: form.LeaveId,
            LeaveName: form.LeaveName,
            ActEvent: form.ActEvent,
            DeductionQty: form.DeductionQty,
            DeductionQtyDefaultEmpty: form.DeductionQtyDefaultEmpty
          }
          await businessReportPrintDetailApi.createBusinessReportPrintDetail(createDto)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
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
      loadLeaveTypeList()
    })

    return {
      loading,
      submitting,
      dialogVisible,
      dialogTitle,
      tableData,
      queryForm,
      pagination,
      form,
      rules,
      formRef,
      selectedRows,
      leaveTypeList,
      formatDate,
      getStatusType,
      handleSearch,
      handleReset,
      handleCreate,
      handleEdit,
      handleDelete,
      handleBatchDelete,
      handleLeaveIdChange,
      handleLeaveIdSelectChange,
      handleActEventChange,
      handleDeductionQtyChange,
      handleDeductionQtyDefaultEmptyChange,
      handleDeductionQtyDefaultEmptyChangeInDialog,
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

.sysl160-query {
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
