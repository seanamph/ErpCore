<template>
  <div class="contract-data-management">
    <div class="page-header">
      <h1>合同資料維護 (SYSF110-SYSF140)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="合同編號">
          <el-input v-model="queryForm.ContractId" placeholder="請輸入合同編號" clearable />
        </el-form-item>
        <el-form-item label="合同類型">
          <el-select v-model="queryForm.ContractType" placeholder="請選擇合同類型" clearable>
            <el-option label="商場招商合約" value="1" />
            <el-option label="委外廠商合約" value="2" />
            <el-option label="塔樓招商合約" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="廠商代號">
          <el-input v-model="queryForm.VendorId" placeholder="請輸入廠商代號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="審核中" value="P" />
            <el-option label="已生效" value="A" />
            <el-option label="已到期" value="E" />
            <el-option label="已終止" value="T" />
          </el-select>
        </el-form-item>
        <el-form-item label="生效日期">
          <el-date-picker
            v-model="queryForm.EffectiveDateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            value-format="YYYY-MM-DD"
          />
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
        <el-table-column prop="ContractId" label="合同編號" width="150" />
        <el-table-column prop="Version" label="版本" width="80" />
        <el-table-column prop="ContractType" label="合同類型" width="120">
          <template #default="{ row }">
            {{ getContractTypeText(row.ContractType) }}
          </template>
        </el-table-column>
        <el-table-column prop="VendorId" label="廠商代號" width="120" />
        <el-table-column prop="VendorName" label="廠商名稱" width="200" />
        <el-table-column prop="SignDate" label="簽約日期" width="120" />
        <el-table-column prop="EffectiveDate" label="生效日期" width="120" />
        <el-table-column prop="ExpiryDate" label="到期日期" width="120" />
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
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'A'">修改</el-button>
            <el-button type="success" size="small" @click="handleApprove(row)" :disabled="row.Status !== 'P'">審核</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'A'">刪除</el-button>
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
      width="1200px"
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
            <el-form-item label="合同編號" prop="ContractId">
              <el-input v-model="formData.ContractId" :disabled="isEdit" placeholder="請輸入合同編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="版本" prop="Version">
              <el-input-number v-model="formData.Version" :min="1" :disabled="isEdit" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="合同類型" prop="ContractType">
              <el-select v-model="formData.ContractType" placeholder="請選擇合同類型" style="width: 100%">
                <el-option label="商場招商合約" value="1" />
                <el-option label="委外廠商合約" value="2" />
                <el-option label="塔樓招商合約" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="廠商代號" prop="VendorId">
              <el-input v-model="formData.VendorId" placeholder="請輸入廠商代號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="廠商名稱" prop="VendorName">
              <el-input v-model="formData.VendorName" placeholder="請輸入廠商名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="草稿" value="D" />
                <el-option label="審核中" value="P" />
                <el-option label="已生效" value="A" />
                <el-option label="已到期" value="E" />
                <el-option label="已終止" value="T" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="簽約日期" prop="SignDate">
              <el-date-picker
                v-model="formData.SignDate"
                type="date"
                placeholder="請選擇簽約日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="生效日期" prop="EffectiveDate">
              <el-date-picker
                v-model="formData.EffectiveDate"
                type="date"
                placeholder="請選擇生效日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="到期日期" prop="ExpiryDate">
              <el-date-picker
                v-model="formData.ExpiryDate"
                type="date"
                placeholder="請選擇到期日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="總金額" prop="TotalAmount">
              <el-input-number v-model="formData.TotalAmount" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="幣別" prop="CurrencyId">
              <el-select v-model="formData.CurrencyId" placeholder="請選擇幣別" style="width: 100%">
                <el-option label="新台幣" value="TWD" />
                <el-option label="美元" value="USD" />
                <el-option label="人民幣" value="CNY" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="匯率" prop="ExchangeRate">
              <el-input-number v-model="formData.ExchangeRate" :min="0" :precision="6" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="位置編號" prop="LocationId">
              <el-input v-model="formData.LocationId" placeholder="請輸入位置編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="招商編號" prop="RecruitId">
              <el-input v-model="formData.RecruitId" placeholder="請輸入招商編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="委託人" prop="Attorney">
              <el-input v-model="formData.Attorney" placeholder="請輸入委託人" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="稱謂" prop="Salutation">
              <el-input v-model="formData.Salutation" placeholder="請輸入稱謂" />
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
        <el-button v-if="isEdit" type="success" @click="handleNewVersion">產生新版本</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { contractApi } from '@/api/contract'

export default {
  name: 'ContractDataManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ContractId: '',
      ContractType: '',
      VendorId: '',
      Status: '',
      EffectiveDateRange: null,
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
      ContractId: '',
      ContractType: '',
      Version: 1,
      VendorId: '',
      VendorName: '',
      SignDate: '',
      EffectiveDate: '',
      ExpiryDate: '',
      Status: 'D',
      TotalAmount: 0,
      CurrencyId: 'TWD',
      ExchangeRate: 1,
      LocationId: '',
      RecruitId: '',
      Attorney: '',
      Salutation: '',
      VerStatus: '',
      AgmStatus: '',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      ContractId: [{ required: true, message: '請輸入合同編號', trigger: 'blur' }],
      ContractType: [{ required: true, message: '請選擇合同類型', trigger: 'change' }],
      VendorId: [{ required: true, message: '請輸入廠商代號', trigger: 'blur' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改合同' : '新增合同'
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
        if (queryForm.EffectiveDateRange && queryForm.EffectiveDateRange.length === 2) {
          params.EffectiveDateFrom = queryForm.EffectiveDateRange[0]
          params.EffectiveDateTo = queryForm.EffectiveDateRange[1]
        }
        delete params.EffectiveDateRange
        const response = await contractApi.getContracts(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢合同列表失敗:', error)
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
        ContractId: '',
        ContractType: '',
        VendorId: '',
        Status: '',
        EffectiveDateRange: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        ContractId: '',
        ContractType: '',
        Version: 1,
        VendorId: '',
        VendorName: '',
        SignDate: '',
        EffectiveDate: '',
        ExpiryDate: '',
        Status: 'D',
        TotalAmount: 0,
        CurrencyId: 'TWD',
        ExchangeRate: 1,
        LocationId: '',
        RecruitId: '',
        Attorney: '',
        Salutation: '',
        VerStatus: '',
        AgmStatus: '',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await contractApi.getContract(row.ContractId, row.Version)
        if (response.data && response.data.Success) {
          Object.assign(formData, response.data.Data)
          isEdit.value = true
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢合同失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 審核
    const handleApprove = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要審核合同「${row.ContractId}」嗎？`,
          '確認審核',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        const approveData = {
          ApproveUserId: 'current_user', // 應該從登入資訊取得
          ApproveDate: new Date().toISOString(),
          Status: 'A'
        }
        await contractApi.approveContract(row.ContractId, row.Version, approveData)
        ElMessage.success('審核成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('審核合同失敗:', error)
          ElMessage.error('審核失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除合同「${row.ContractId}」版本「${row.Version}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await contractApi.deleteContract(row.ContractId, row.Version)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除合同失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 產生新版本
    const handleNewVersion = async () => {
      try {
        await ElMessageBox.confirm(
          '確定要產生新版本嗎？',
          '確認產生新版本',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        const newVersionData = {
          VerStatus: '1'
        }
        const response = await contractApi.createNewVersion(formData.ContractId, formData.Version, newVersionData)
        if (response.data && response.data.Success) {
          ElMessage.success('產生新版本成功')
          dialogVisible.value = false
          loadData()
        } else {
          ElMessage.error(response.data?.Message || '產生新版本失敗')
        }
      } catch (error) {
        if (error !== 'cancel') {
          console.error('產生新版本失敗:', error)
          ElMessage.error('產生新版本失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await contractApi.updateContract(formData.ContractId, formData.Version, formData)
          ElMessage.success('修改成功')
        } else {
          await contractApi.createContract(formData)
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

    // 取得合同類型文字
    const getContractTypeText = (type) => {
      const typeMap = {
        '1': '商場招商合約',
        '2': '委外廠商合約',
        '3': '塔樓招商合約'
      }
      return typeMap[type] || type
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'P': 'warning',
        'A': 'success',
        'E': 'warning',
        'T': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'D': '草稿',
        'P': '審核中',
        'A': '已生效',
        'E': '已到期',
        'T': '已終止'
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
      handleApprove,
      handleDelete,
      handleNewVersion,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      formatCurrency,
      getContractTypeText,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.contract-data-management {
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

