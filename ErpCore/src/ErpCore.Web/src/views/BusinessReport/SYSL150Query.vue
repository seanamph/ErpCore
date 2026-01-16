<template>
  <div class="sysl150-query">
    <div class="page-header">
      <h1>業務報表列印作業 (SYSL150)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="發放年度">
          <el-input-number
            v-model="queryForm.GiveYear"
            placeholder="請輸入年度"
            :min="2000"
            :max="2100"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="店別">
          <el-input
            v-model="queryForm.SiteId"
            placeholder="請輸入店別代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="組織">
          <el-input
            v-model="queryForm.OrgId"
            placeholder="請輸入組織代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="員工">
          <el-input
            v-model="queryForm.EmpId"
            placeholder="請輸入員工編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable style="width: 200px">
            <el-option label="全部" value="" />
            <el-option label="待審核" value="P" />
            <el-option label="已審核" value="A" />
            <el-option label="已拒絕" value="R" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="warning" @click="handleBatchAudit" :disabled="selectedRows.length === 0">批次審核</el-button>
          <el-button type="info" @click="handleCopyNextYear">複製下一年度</el-button>
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
        <el-table-column prop="GiveYear" label="發放年度" width="120" />
        <el-table-column prop="SiteId" label="店別代碼" width="120" />
        <el-table-column prop="SiteName" label="店別名稱" width="150" />
        <el-table-column prop="OrgId" label="組織代碼" width="120" />
        <el-table-column prop="OrgName" label="組織名稱" width="150" />
        <el-table-column prop="EmpId" label="員工編號" width="120" />
        <el-table-column prop="EmpName" label="員工姓名" width="150" />
        <el-table-column prop="Qty" label="數量" width="120">
          <template #default="{ row }">
            {{ formatNumber(row.Qty) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ row.StatusName || row.Status }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Verifier" label="審核者" width="120" />
        <el-table-column prop="VerifyDate" label="審核日期" width="180">
          <template #default="{ row }">
            {{ formatDate(row.VerifyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDate(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="warning" size="small" @click="handleCalculateQty(row)">計算數量</el-button>
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
            <el-form-item label="發放年度" prop="GiveYear">
              <el-input-number
                v-model="form.GiveYear"
                placeholder="請輸入年度"
                :min="2000"
                :max="2100"
                :disabled="isYearReadOnly"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="店別代碼" prop="SiteId">
              <el-input v-model="form.SiteId" placeholder="請輸入店別代碼" @change="handleSiteChange" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="組織代碼" prop="OrgId">
              <el-input v-model="form.OrgId" placeholder="請輸入組織代碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="員工編號" prop="EmpId">
              <el-input v-model="form.EmpId" placeholder="請輸入員工編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="員工姓名" prop="EmpName">
              <el-input v-model="form.EmpName" placeholder="請輸入員工姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="數量" prop="Qty">
              <el-input-number
                v-model="form.Qty"
                placeholder="請輸入數量"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
              <el-button type="primary" size="small" @click="handleCalculateQtyInDialog" style="margin-top: 5px">
                自動計算
              </el-button>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="待審核" value="P" />
                <el-option label="已審核" value="A" />
                <el-option label="已拒絕" value="R" />
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

    <!-- 批次審核對話框 -->
    <el-dialog
      title="批次審核"
      v-model="batchAuditDialogVisible"
      width="500px"
    >
      <el-form :model="batchAuditForm" label-width="120px">
        <el-form-item label="審核狀態">
          <el-select v-model="batchAuditForm.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="已審核" value="A" />
            <el-option label="已拒絕" value="R" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註">
          <el-input
            v-model="batchAuditForm.Notes"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="batchAuditDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleBatchAuditSubmit" :loading="submitting">確定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 複製下一年度對話框 -->
    <el-dialog
      title="複製下一年度資料"
      v-model="copyNextYearDialogVisible"
      width="500px"
    >
      <el-form :model="copyNextYearForm" label-width="120px">
        <el-form-item label="來源年度">
          <el-input-number
            v-model="copyNextYearForm.SourceYear"
            placeholder="請輸入來源年度"
            :min="2000"
            :max="2100"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="目標年度">
          <el-input-number
            v-model="copyNextYearForm.TargetYear"
            placeholder="請輸入目標年度"
            :min="2000"
            :max="2100"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="店別代碼">
          <el-input
            v-model="copyNextYearForm.SiteId"
            placeholder="請輸入店別代碼（可選）"
            clearable
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="copyNextYearDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleCopyNextYearSubmit" :loading="submitting">確定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { businessReportPrintApi } from '@/api/businessReport'

export default {
  name: 'SYSL150Query',
  setup() {
    const loading = ref(false)
    const submitting = ref(false)
    const dialogVisible = ref(false)
    const batchAuditDialogVisible = ref(false)
    const copyNextYearDialogVisible = ref(false)
    const dialogTitle = ref('新增業務報表列印')
    const isEdit = ref(false)
    const currentTKey = ref(null)
    const selectedRows = ref([])
    const formRef = ref(null)

    const tableData = ref([])
    const queryForm = reactive({
      GiveYear: null,
      SiteId: '',
      OrgId: '',
      EmpId: '',
      Status: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const form = reactive({
      GiveYear: new Date().getFullYear(),
      SiteId: '',
      OrgId: '',
      EmpId: '',
      EmpName: '',
      Qty: null,
      Status: 'P',
      Notes: ''
    })

    const batchAuditForm = reactive({
      Status: 'A',
      Notes: ''
    })

    const copyNextYearForm = reactive({
      SourceYear: new Date().getFullYear(),
      TargetYear: new Date().getFullYear() + 1,
      SiteId: ''
    })

    const rules = {
      GiveYear: [{ required: true, message: '請輸入發放年度', trigger: 'blur' }],
      SiteId: [{ required: true, message: '請輸入店別代碼', trigger: 'blur' }],
      EmpId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 年度是否唯讀（已審核的年度不可修改）
    const isYearReadOnly = computed(() => {
      if (!isEdit.value || !form.GiveYear) return false
      // 這裡可以根據實際需求檢查年度是否已審核
      return false
    })

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 格式化數字
    const formatNumber = (num) => {
      if (num == null) return '0.00'
      return parseFloat(num).toFixed(2)
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      switch (status) {
        case 'P':
          return 'warning'
        case 'A':
          return 'success'
        case 'R':
          return 'danger'
        default:
          return ''
      }
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
        const response = await businessReportPrintApi.getBusinessReportPrints(params)
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
        GiveYear: null,
        SiteId: '',
        OrgId: '',
        EmpId: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      dialogTitle.value = '新增業務報表列印'
      isEdit.value = false
      currentTKey.value = null
      Object.assign(form, {
        GiveYear: new Date().getFullYear(),
        SiteId: '',
        OrgId: '',
        EmpId: '',
        EmpName: '',
        Qty: null,
        Status: 'P',
        Notes: ''
      })
      dialogVisible.value = true
    }

    // 修改
    const handleEdit = async (row) => {
      dialogTitle.value = '修改業務報表列印'
      isEdit.value = true
      currentTKey.value = row.TKey
      try {
        const response = await businessReportPrintApi.getBusinessReportPrint(row.TKey)
        if (response.Data) {
          Object.assign(form, {
            GiveYear: response.Data.GiveYear || new Date().getFullYear(),
            SiteId: response.Data.SiteId || '',
            OrgId: response.Data.OrgId || '',
            EmpId: response.Data.EmpId || '',
            EmpName: response.Data.EmpName || '',
            Qty: response.Data.Qty || null,
            Status: response.Data.Status || 'P',
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
          `確定要刪除業務報表列印資料嗎？\n年度: ${row.GiveYear}, 員工: ${row.EmpName || row.EmpId}`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await businessReportPrintApi.deleteBusinessReportPrint(row.TKey)
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
        await businessReportPrintApi.batchDeleteBusinessReportPrint({ TKeys: tKeys })
        ElMessage.success('批次刪除成功')
        selectedRows.value = []
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('批次刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 批次審核
    const handleBatchAudit = () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('請選擇要審核的資料')
        return
      }
      batchAuditForm.Status = 'A'
      batchAuditForm.Notes = ''
      batchAuditDialogVisible.value = true
    }

    // 批次審核提交
    const handleBatchAuditSubmit = async () => {
      try {
        submitting.value = true
        const tKeys = selectedRows.value.map(row => row.TKey)
        const response = await businessReportPrintApi.batchAudit({
          TKeys: tKeys,
          Status: batchAuditForm.Status,
          Notes: batchAuditForm.Notes
        })
        if (response.Data) {
          ElMessage.success(`批次審核成功: ${response.Data.SuccessCount} 筆`)
        }
        batchAuditDialogVisible.value = false
        selectedRows.value = []
        loadData()
      } catch (error) {
        ElMessage.error('批次審核失敗: ' + (error.message || '未知錯誤'))
      } finally {
        submitting.value = false
      }
    }

    // 複製下一年度
    const handleCopyNextYear = () => {
      copyNextYearForm.SourceYear = new Date().getFullYear()
      copyNextYearForm.TargetYear = new Date().getFullYear() + 1
      copyNextYearForm.SiteId = ''
      copyNextYearDialogVisible.value = true
    }

    // 複製下一年度提交
    const handleCopyNextYearSubmit = async () => {
      try {
        submitting.value = true
        const response = await businessReportPrintApi.copyNextYear(copyNextYearForm)
        if (response.Data) {
          ElMessage.success(`複製成功: ${response.Data.CopiedCount} 筆`)
        }
        copyNextYearDialogVisible.value = false
        loadData()
      } catch (error) {
        ElMessage.error('複製失敗: ' + (error.message || '未知錯誤'))
      } finally {
        submitting.value = false
      }
    }

    // 計算數量
    const handleCalculateQty = async (row) => {
      try {
        const response = await businessReportPrintApi.calculateQty({ TKey: row.TKey })
        if (response.Data) {
          ElMessage.success(`計算完成，數量: ${formatNumber(response.Data.Qty)}`)
          loadData()
        }
      } catch (error) {
        ElMessage.error('計算數量失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 在對話框中計算數量
    const handleCalculateQtyInDialog = async () => {
      if (!currentTKey.value) {
        ElMessage.warning('請先儲存資料後再計算數量')
        return
      }
      try {
        const response = await businessReportPrintApi.calculateQty({ TKey: currentTKey.value })
        if (response.Data) {
          form.Qty = response.Data.Qty
          ElMessage.success('計算完成')
        }
      } catch (error) {
        ElMessage.error('計算數量失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 店別變更
    const handleSiteChange = async () => {
      // 切換店別時可以重新載入相關資料
      // 這裡可以根據實際需求實現
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        submitting.value = true
        if (isEdit.value) {
          await businessReportPrintApi.updateBusinessReportPrint(currentTKey.value, form)
          ElMessage.success('修改成功')
        } else {
          await businessReportPrintApi.createBusinessReportPrint(form)
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
    })

    return {
      loading,
      submitting,
      dialogVisible,
      batchAuditDialogVisible,
      copyNextYearDialogVisible,
      dialogTitle,
      isEdit,
      isYearReadOnly,
      tableData,
      queryForm,
      pagination,
      form,
      batchAuditForm,
      copyNextYearForm,
      rules,
      formRef,
      selectedRows,
      formatDate,
      formatNumber,
      getStatusType,
      handleSearch,
      handleReset,
      handleCreate,
      handleEdit,
      handleDelete,
      handleBatchDelete,
      handleBatchAudit,
      handleBatchAuditSubmit,
      handleCopyNextYear,
      handleCopyNextYearSubmit,
      handleCalculateQty,
      handleCalculateQtyInDialog,
      handleSiteChange,
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

.sysl150-query {
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
