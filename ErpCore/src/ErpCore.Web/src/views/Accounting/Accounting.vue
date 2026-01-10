<template>
  <div class="accounting">
    <div class="page-header">
      <h1>會計管理 (SYSN120-SYSN154)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="傳票編號">
          <el-input v-model="queryForm.VoucherId" placeholder="請輸入傳票編號" clearable />
        </el-form-item>
        <el-form-item label="傳票日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="handleDateRangeChange"
          />
        </el-form-item>
        <el-form-item label="傳票類型">
          <el-input v-model="queryForm.VoucherTypeId" placeholder="請輸入傳票類型" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已過帳" value="P" />
            <el-option label="已取消" value="C" />
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
        <el-table-column prop="VoucherId" label="傳票編號" width="150" />
        <el-table-column prop="VoucherDate" label="傳票日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.VoucherDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="VoucherTypeId" label="傳票類型" width="120" />
        <el-table-column prop="Description" label="說明" width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PostedBy" label="過帳人員" width="120" />
        <el-table-column prop="PostedAt" label="過帳時間" width="150">
          <template #default="{ row }">
            {{ row.PostedAt ? formatDate(row.PostedAt) : '-' }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'P'">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'P'">刪除</el-button>
            <el-button v-if="row.Status === 'D'" type="success" size="small" @click="handlePost(row)">過帳</el-button>
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
            <el-form-item label="傳票編號" prop="VoucherId">
              <el-input v-model="formData.VoucherId" :disabled="isEdit" placeholder="請輸入傳票編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="傳票日期" prop="VoucherDate">
              <el-date-picker
                v-model="formData.VoucherDate"
                type="date"
                placeholder="請選擇傳票日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="傳票類型" prop="VoucherTypeId">
              <el-input v-model="formData.VoucherTypeId" placeholder="請輸入傳票類型" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="說明" prop="Description">
              <el-input v-model="formData.Description" placeholder="請輸入說明" />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- 傳票明細 -->
        <el-divider content-position="left">傳票明細</el-divider>
        <el-table :data="formData.Details" border style="width: 100%; margin-bottom: 20px;">
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="StypeId" label="會計科目" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.StypeId" placeholder="會計科目" />
            </template>
          </el-table-column>
          <el-table-column prop="Dc" label="借貸方向" width="120">
            <template #default="{ row }">
              <el-select v-model="row.Dc" placeholder="請選擇" style="width: 100%">
                <el-option label="借方" value="D" />
                <el-option label="貸方" value="C" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="150">
            <template #default="{ row }">
              <el-input-number v-model="row.Amount" :min="0" :precision="2" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column prop="Description" label="說明">
            <template #default="{ row }">
              <el-input v-model="row.Description" placeholder="說明" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
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
import { accountingApi } from '@/api/accounting'

export default {
  name: 'Accounting',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const dateRange = ref(null)

    // 查詢表單
    const queryForm = reactive({
      VoucherId: '',
      VoucherDateFrom: null,
      VoucherDateTo: null,
      VoucherTypeId: '',
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
      VoucherId: '',
      VoucherDate: '',
      VoucherTypeId: '',
      Description: '',
      Details: []
    })

    // 表單驗證規則
    const formRules = {
      VoucherId: [{ required: true, message: '請輸入傳票編號', trigger: 'blur' }],
      VoucherDate: [{ required: true, message: '請選擇傳票日期', trigger: 'change' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return '-'
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const map = { D: 'info', P: 'success', C: 'warning' }
      return map[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const map = { D: '草稿', P: '已過帳', C: '已取消' }
      return map[status] || status
    }

    // 日期範圍變更
    const handleDateRangeChange = (dates) => {
      if (dates && dates.length === 2) {
        queryForm.VoucherDateFrom = dates[0]
        queryForm.VoucherDateTo = dates[1]
      } else {
        queryForm.VoucherDateFrom = null
        queryForm.VoucherDateTo = null
      }
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
        const response = await accountingApi.getVouchers(params)
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
        VoucherId: '',
        VoucherDateFrom: null,
        VoucherDateTo: null,
        VoucherTypeId: '',
        Status: ''
      })
      dateRange.value = null
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        VoucherId: '',
        VoucherDate: '',
        VoucherTypeId: '',
        Description: '',
        Details: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await accountingApi.getVoucher(row.VoucherId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, {
            VoucherId: response.Data.VoucherId,
            VoucherDate: formatDate(response.Data.VoucherDate),
            VoucherTypeId: response.Data.VoucherTypeId || '',
            Description: response.Data.Description || '',
            Details: response.Data.Details || []
          })
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
        await ElMessageBox.confirm('確定要刪除此傳票嗎？', '確認', {
          type: 'warning'
        })
        await accountingApi.deleteVoucher(row.VoucherId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 過帳
    const handlePost = async (row) => {
      try {
        await ElMessageBox.confirm('確定要過帳此傳票嗎？', '確認', {
          type: 'warning'
        })
        await accountingApi.postVoucher(row.VoucherId)
        ElMessage.success('過帳成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('過帳失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 新增明細
    const handleAddDetail = () => {
      const seqNo = formData.Details.length + 1
      formData.Details.push({
        SeqNo: seqNo,
        StypeId: '',
        Dc: 'D',
        Amount: 0,
        Description: ''
      })
    }

    // 刪除明細
    const handleRemoveDetail = (index) => {
      formData.Details.splice(index, 1)
      // 重新編號
      formData.Details.forEach((item, idx) => {
        item.SeqNo = idx + 1
      })
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          if (formData.Details.length === 0) {
            ElMessage.warning('請至少新增一筆明細')
            return
          }
          try {
            const submitData = {
              ...formData,
              Details: formData.Details.map((item, index) => ({
                SeqNo: index + 1,
                StypeId: item.StypeId,
                Dc: item.Dc,
                Amount: item.Amount,
                Description: item.Description
              }))
            }
            if (isEdit.value) {
              await accountingApi.updateVoucher(formData.VoucherId, submitData)
              ElMessage.success('修改成功')
            } else {
              await accountingApi.createVoucher(submitData)
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
      return isEdit.value ? '修改傳票' : '新增傳票'
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
      dateRange,
      formatDate,
      getStatusType,
      getStatusText,
      handleDateRangeChange,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handlePost,
      handleAddDetail,
      handleRemoveDetail,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.accounting {
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
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .search-form {
      .el-form-item {
        margin-bottom: 16px;
      }
    }
  }

  .table-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
  }
}
</style>

