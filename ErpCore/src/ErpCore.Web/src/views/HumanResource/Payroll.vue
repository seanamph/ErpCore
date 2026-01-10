<template>
  <div class="payroll">
    <div class="page-header">
      <h1>薪資管理 (SYSH210)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="薪資單號">
          <el-input v-model="queryForm.PayrollId" placeholder="請輸入薪資單號" clearable />
        </el-form-item>
        <el-form-item label="員工編號">
          <el-input v-model="queryForm.EmployeeId" placeholder="請輸入員工編號" clearable />
        </el-form-item>
        <el-form-item label="員工姓名">
          <el-input v-model="queryForm.EmployeeName" placeholder="請輸入員工姓名" clearable />
        </el-form-item>
        <el-form-item label="薪資月份">
          <el-date-picker
            v-model="queryForm.PayrollMonth"
            type="month"
            placeholder="請選擇薪資月份"
            format="YYYY-MM"
            value-format="YYYY-MM"
            clearable
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已確認" value="C" />
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
        <el-table-column prop="PayrollId" label="薪資單號" width="150" />
        <el-table-column prop="EmployeeId" label="員工編號" width="120" />
        <el-table-column prop="EmployeeName" label="員工姓名" width="150" />
        <el-table-column prop="PayrollMonth" label="薪資月份" width="120" />
        <el-table-column prop="BaseSalary" label="基本薪資" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.BaseSalary) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'C'">修改</el-button>
            <el-button type="success" size="small" @click="handleConfirm(row)" :disabled="row.Status === 'C'">確認</el-button>
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
      width="900px"
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
            <el-form-item label="員工編號" prop="EmployeeId">
              <el-input v-model="formData.EmployeeId" :disabled="isEdit" placeholder="請輸入員工編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="員工姓名" prop="EmployeeName">
              <el-input v-model="formData.EmployeeName" placeholder="請輸入員工姓名" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="薪資月份" prop="PayrollMonth">
              <el-date-picker
                v-model="formData.PayrollMonth"
                type="month"
                placeholder="請選擇薪資月份"
                format="YYYY-MM"
                value-format="YYYY-MM"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="基本薪資" prop="BaseSalary">
              <el-input-number
                v-model="formData.BaseSalary"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="獎金" prop="Bonus">
              <el-input-number
                v-model="formData.Bonus"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="扣除金額" prop="Deduction">
              <el-input-number
                v-model="formData.Deduction"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="總金額" prop="TotalAmount">
              <el-input-number
                v-model="formData.TotalAmount"
                :min="0"
                :precision="2"
                :disabled="true"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="草稿" value="D" />
                <el-option label="已確認" value="C" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註" prop="Memo">
          <el-input
            v-model="formData.Memo"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="info" @click="handleCalculate">計算</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { humanResourceApi } from '@/api/humanResource'

export default {
  name: 'Payroll',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      PayrollId: '',
      EmployeeId: '',
      EmployeeName: '',
      PayrollMonth: '',
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
      EmployeeId: '',
      EmployeeName: '',
      PayrollMonth: '',
      BaseSalary: 0,
      Bonus: 0,
      Deduction: 0,
      TotalAmount: 0,
      Status: 'D',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      EmployeeId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
      PayrollMonth: [{ required: true, message: '請選擇薪資月份', trigger: 'change' }],
      BaseSalary: [{ required: true, message: '請輸入基本薪資', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 監聽表單資料變化，自動計算總金額
    watch([() => formData.BaseSalary, () => formData.Bonus, () => formData.Deduction], () => {
      formData.TotalAmount = (formData.BaseSalary || 0) + (formData.Bonus || 0) - (formData.Deduction || 0)
    })

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'D': 'info',
        'C': 'success'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'D': '草稿',
        'C': '已確認'
      }
      return texts[status] || status
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
        const response = await humanResourceApi.getPayrolls(params)
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
        PayrollId: '',
        EmployeeId: '',
        EmployeeName: '',
        PayrollMonth: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        EmployeeId: '',
        EmployeeName: '',
        PayrollMonth: '',
        BaseSalary: 0,
        Bonus: 0,
        Deduction: 0,
        TotalAmount: 0,
        Status: 'D',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await humanResourceApi.getPayroll(row.PayrollId)
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

    // 確認
    const handleConfirm = async (row) => {
      try {
        await ElMessageBox.confirm('確定要確認此薪資單嗎？', '確認', {
          type: 'warning'
        })
        await humanResourceApi.confirmPayroll(row.PayrollId)
        ElMessage.success('確認成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('確認失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此薪資單嗎？', '確認', {
          type: 'warning'
        })
        await humanResourceApi.deletePayroll(row.PayrollId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 計算薪資
    const handleCalculate = async () => {
      try {
        const response = await humanResourceApi.calculatePayroll({
          EmployeeId: formData.EmployeeId,
          PayrollMonth: formData.PayrollMonth,
          BaseSalary: formData.BaseSalary,
          Bonus: formData.Bonus,
          Deduction: formData.Deduction
        })
        if (response.Data) {
          formData.TotalAmount = response.Data.TotalAmount || 0
          ElMessage.success('計算完成')
        }
      } catch (error) {
        ElMessage.error('計算失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await humanResourceApi.updatePayroll(formData.PayrollId, formData)
              ElMessage.success('修改成功')
            } else {
              await humanResourceApi.createPayroll(formData)
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
      return isEdit.value ? '修改薪資單' : '新增薪資單'
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
      formatCurrency,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleConfirm,
      handleDelete,
      handleCalculate,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.payroll {
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

