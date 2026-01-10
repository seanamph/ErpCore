<template>
  <div class="lease-syse-data-management">
    <div class="page-header">
      <h1>租賃資料維護 (SYSE110-SYSE140)</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 租賃條件 Tab -->
      <el-tab-pane label="租賃條件" name="terms">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="termsQueryForm" class="search-form">
            <el-form-item label="租賃編號">
              <el-input v-model="termsQueryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
            </el-form-item>
            <el-form-item label="版本號">
              <el-input v-model="termsQueryForm.Version" placeholder="請輸入版本號" clearable />
            </el-form-item>
            <el-form-item label="條件類型">
              <el-select v-model="termsQueryForm.TermType" placeholder="請選擇條件類型" clearable>
                <el-option label="租金條件" value="RENT" />
                <el-option label="押金條件" value="DEPOSIT" />
                <el-option label="其他條件" value="OTHER" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleTermsSearch">查詢</el-button>
              <el-button @click="handleTermsReset">重置</el-button>
              <el-button type="success" @click="handleTermsCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="termsTableData"
            v-loading="termsLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="TKey" label="主鍵" width="80" />
            <el-table-column prop="LeaseId" label="租賃編號" width="150" />
            <el-table-column prop="Version" label="版本號" width="100" />
            <el-table-column prop="TermType" label="條件類型" width="120" />
            <el-table-column prop="TermName" label="條件名稱" width="200" />
            <el-table-column prop="TermValue" label="條件值" width="200" />
            <el-table-column prop="TermAmount" label="條件金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.TermAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="TermDate" label="條件日期" width="120" />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleTermsView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleTermsEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleTermsDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="termsPagination.PageIndex"
            v-model:page-size="termsPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="termsPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleTermsSizeChange"
            @current-change="handleTermsPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>

      <!-- 會計分類 Tab -->
      <el-tab-pane label="會計分類" name="accounting">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="accountingQueryForm" class="search-form">
            <el-form-item label="租賃編號">
              <el-input v-model="accountingQueryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
            </el-form-item>
            <el-form-item label="版本號">
              <el-input v-model="accountingQueryForm.Version" placeholder="請輸入版本號" clearable />
            </el-form-item>
            <el-form-item label="會計科目">
              <el-input v-model="accountingQueryForm.AccountCode" placeholder="請輸入會計科目" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleAccountingSearch">查詢</el-button>
              <el-button @click="handleAccountingReset">重置</el-button>
              <el-button type="success" @click="handleAccountingCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="accountingTableData"
            v-loading="accountingLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="TKey" label="主鍵" width="80" />
            <el-table-column prop="LeaseId" label="租賃編號" width="150" />
            <el-table-column prop="Version" label="版本號" width="100" />
            <el-table-column prop="AccountCode" label="會計科目" width="150" />
            <el-table-column prop="AccountName" label="會計科目名稱" width="200" />
            <el-table-column prop="CategoryType" label="分類類型" width="120" />
            <el-table-column prop="Amount" label="金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.Amount) }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleAccountingView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleAccountingEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleAccountingDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="accountingPagination.PageIndex"
            v-model:page-size="accountingPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="accountingPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleAccountingSizeChange"
            @current-change="handleAccountingPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 租賃條件對話框 -->
    <el-dialog
      v-model="termsDialogVisible"
      :title="termsDialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="termsFormRef"
        :model="termsFormData"
        :rules="termsFormRules"
        label-width="140px"
      >
        <el-form-item label="租賃編號" prop="LeaseId">
          <el-input v-model="termsFormData.LeaseId" placeholder="請輸入租賃編號" />
        </el-form-item>
        <el-form-item label="版本號" prop="Version">
          <el-input v-model="termsFormData.Version" placeholder="請輸入版本號" />
        </el-form-item>
        <el-form-item label="條件類型" prop="TermType">
          <el-select v-model="termsFormData.TermType" placeholder="請選擇條件類型" style="width: 100%">
            <el-option label="租金條件" value="RENT" />
            <el-option label="押金條件" value="DEPOSIT" />
            <el-option label="其他條件" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="條件名稱" prop="TermName">
          <el-input v-model="termsFormData.TermName" placeholder="請輸入條件名稱" />
        </el-form-item>
        <el-form-item label="條件值" prop="TermValue">
          <el-input v-model="termsFormData.TermValue" type="textarea" :rows="3" placeholder="請輸入條件值" />
        </el-form-item>
        <el-form-item label="條件金額" prop="TermAmount">
          <el-input-number v-model="termsFormData.TermAmount" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="條件日期" prop="TermDate">
          <el-date-picker
            v-model="termsFormData.TermDate"
            type="date"
            placeholder="請選擇條件日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="termsFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="termsDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleTermsSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 會計分類對話框 -->
    <el-dialog
      v-model="accountingDialogVisible"
      :title="accountingDialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="accountingFormRef"
        :model="accountingFormData"
        :rules="accountingFormRules"
        label-width="140px"
      >
        <el-form-item label="租賃編號" prop="LeaseId">
          <el-input v-model="accountingFormData.LeaseId" placeholder="請輸入租賃編號" />
        </el-form-item>
        <el-form-item label="版本號" prop="Version">
          <el-input v-model="accountingFormData.Version" placeholder="請輸入版本號" />
        </el-form-item>
        <el-form-item label="會計科目" prop="AccountCode">
          <el-input v-model="accountingFormData.AccountCode" placeholder="請輸入會計科目" />
        </el-form-item>
        <el-form-item label="會計科目名稱" prop="AccountName">
          <el-input v-model="accountingFormData.AccountName" placeholder="請輸入會計科目名稱" />
        </el-form-item>
        <el-form-item label="分類類型" prop="CategoryType">
          <el-select v-model="accountingFormData.CategoryType" placeholder="請選擇分類類型" style="width: 100%">
            <el-option label="收入" value="INCOME" />
            <el-option label="支出" value="EXPENSE" />
            <el-option label="資產" value="ASSET" />
            <el-option label="負債" value="LIABILITY" />
          </el-select>
        </el-form-item>
        <el-form-item label="金額" prop="Amount">
          <el-input-number v-model="accountingFormData.Amount" :min="0" :precision="2" style="width: 100%" />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="accountingFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="accountingDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleAccountingSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { leaseSYSEApi } from '@/api/leaseSYSE'

export default {
  name: 'LeaseSYSEDataManagement',
  setup() {
    const activeTab = ref('terms')
    
    // 租賃條件相關
    const termsLoading = ref(false)
    const termsDialogVisible = ref(false)
    const termsIsEdit = ref(false)
    const termsFormRef = ref(null)
    const termsTableData = ref([])
    
    const termsQueryForm = reactive({
      LeaseId: '',
      Version: '',
      TermType: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const termsPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const termsFormData = reactive({
      LeaseId: '',
      Version: '',
      TermType: '',
      TermName: '',
      TermValue: '',
      TermAmount: 0,
      TermDate: '',
      Memo: ''
    })
    
    const termsFormRules = {
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      Version: [{ required: true, message: '請輸入版本號', trigger: 'blur' }],
      TermType: [{ required: true, message: '請選擇條件類型', trigger: 'change' }]
    }
    
    const termsDialogTitle = computed(() => {
      return termsIsEdit.value ? '修改租賃條件' : '新增租賃條件'
    })
    
    // 會計分類相關
    const accountingLoading = ref(false)
    const accountingDialogVisible = ref(false)
    const accountingIsEdit = ref(false)
    const accountingFormRef = ref(null)
    const accountingTableData = ref([])
    
    const accountingQueryForm = reactive({
      LeaseId: '',
      Version: '',
      AccountCode: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const accountingPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const accountingFormData = reactive({
      LeaseId: '',
      Version: '',
      AccountCode: '',
      AccountName: '',
      CategoryType: '',
      Amount: 0,
      Memo: ''
    })
    
    const accountingFormRules = {
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      Version: [{ required: true, message: '請輸入版本號', trigger: 'blur' }],
      AccountCode: [{ required: true, message: '請輸入會計科目', trigger: 'blur' }]
    }
    
    const accountingDialogTitle = computed(() => {
      return accountingIsEdit.value ? '修改會計分類' : '新增會計分類'
    })
    
    // 租賃條件方法
    const loadTermsData = async () => {
      termsLoading.value = true
      try {
        const params = {
          ...termsQueryForm,
          PageIndex: termsPagination.PageIndex,
          PageSize: termsPagination.PageSize
        }
        const response = await leaseSYSEApi.getLeaseTerms(params)
        if (response.data && response.data.Success) {
          termsTableData.value = response.data.Data.Items || []
          termsPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃條件列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        termsLoading.value = false
      }
    }
    
    const handleTermsSearch = () => {
      termsPagination.PageIndex = 1
      loadTermsData()
    }
    
    const handleTermsReset = () => {
      Object.assign(termsQueryForm, {
        LeaseId: '',
        Version: '',
        TermType: ''
      })
      handleTermsSearch()
    }
    
    const handleTermsCreate = () => {
      termsIsEdit.value = false
      termsDialogVisible.value = true
      Object.assign(termsFormData, {
        LeaseId: '',
        Version: '',
        TermType: '',
        TermName: '',
        TermValue: '',
        TermAmount: 0,
        TermDate: '',
        Memo: ''
      })
    }
    
    const handleTermsView = async (row) => {
      try {
        const response = await leaseSYSEApi.getLeaseTerm(row.TKey)
        if (response.data && response.data.Success) {
          Object.assign(termsFormData, response.data.Data)
          termsIsEdit.value = true
          termsDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃條件失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleTermsEdit = async (row) => {
      await handleTermsView(row)
    }
    
    const handleTermsDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除租賃條件「${row.TermName}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSEApi.deleteLeaseTerm(row.TKey)
        ElMessage.success('刪除成功')
        loadTermsData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除租賃條件失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleTermsSubmit = async () => {
      if (!termsFormRef.value) return
      try {
        await termsFormRef.value.validate()
        if (termsIsEdit.value) {
          await leaseSYSEApi.updateLeaseTerm(termsFormData.TKey, termsFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSEApi.createLeaseTerm(termsFormData)
          ElMessage.success('新增成功')
        }
        termsDialogVisible.value = false
        loadTermsData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((termsIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleTermsSizeChange = (size) => {
      termsPagination.PageSize = size
      termsPagination.PageIndex = 1
      loadTermsData()
    }
    
    const handleTermsPageChange = (page) => {
      termsPagination.PageIndex = page
      loadTermsData()
    }
    
    // 會計分類方法
    const loadAccountingData = async () => {
      accountingLoading.value = true
      try {
        const params = {
          ...accountingQueryForm,
          PageIndex: accountingPagination.PageIndex,
          PageSize: accountingPagination.PageSize
        }
        const response = await leaseSYSEApi.getLeaseAccountingCategories(params)
        if (response.data && response.data.Success) {
          accountingTableData.value = response.data.Data.Items || []
          accountingPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢會計分類列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        accountingLoading.value = false
      }
    }
    
    const handleAccountingSearch = () => {
      accountingPagination.PageIndex = 1
      loadAccountingData()
    }
    
    const handleAccountingReset = () => {
      Object.assign(accountingQueryForm, {
        LeaseId: '',
        Version: '',
        AccountCode: ''
      })
      handleAccountingSearch()
    }
    
    const handleAccountingCreate = () => {
      accountingIsEdit.value = false
      accountingDialogVisible.value = true
      Object.assign(accountingFormData, {
        LeaseId: '',
        Version: '',
        AccountCode: '',
        AccountName: '',
        CategoryType: '',
        Amount: 0,
        Memo: ''
      })
    }
    
    const handleAccountingView = async (row) => {
      try {
        const response = await leaseSYSEApi.getLeaseAccountingCategory(row.TKey)
        if (response.data && response.data.Success) {
          Object.assign(accountingFormData, response.data.Data)
          accountingIsEdit.value = true
          accountingDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢會計分類失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleAccountingEdit = async (row) => {
      await handleAccountingView(row)
    }
    
    const handleAccountingDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除會計分類「${row.AccountName}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSEApi.deleteLeaseAccountingCategory(row.TKey)
        ElMessage.success('刪除成功')
        loadAccountingData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除會計分類失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleAccountingSubmit = async () => {
      if (!accountingFormRef.value) return
      try {
        await accountingFormRef.value.validate()
        if (accountingIsEdit.value) {
          await leaseSYSEApi.updateLeaseAccountingCategory(accountingFormData.TKey, accountingFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSEApi.createLeaseAccountingCategory(accountingFormData)
          ElMessage.success('新增成功')
        }
        accountingDialogVisible.value = false
        loadAccountingData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((accountingIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleAccountingSizeChange = (size) => {
      accountingPagination.PageSize = size
      accountingPagination.PageIndex = 1
      loadAccountingData()
    }
    
    const handleAccountingPageChange = (page) => {
      accountingPagination.PageIndex = page
      loadAccountingData()
    }
    
    // 格式化貨幣
    const formatCurrency = (value) => {
      if (!value) return '0.00'
      return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }
    
    // 初始化
    onMounted(() => {
      loadTermsData()
      loadAccountingData()
    })
    
    return {
      activeTab,
      // 租賃條件
      termsLoading,
      termsDialogVisible,
      termsIsEdit,
      termsFormRef,
      termsTableData,
      termsQueryForm,
      termsPagination,
      termsFormData,
      termsFormRules,
      termsDialogTitle,
      handleTermsSearch,
      handleTermsReset,
      handleTermsCreate,
      handleTermsView,
      handleTermsEdit,
      handleTermsDelete,
      handleTermsSubmit,
      handleTermsSizeChange,
      handleTermsPageChange,
      // 會計分類
      accountingLoading,
      accountingDialogVisible,
      accountingIsEdit,
      accountingFormRef,
      accountingTableData,
      accountingQueryForm,
      accountingPagination,
      accountingFormData,
      accountingFormRules,
      accountingDialogTitle,
      handleAccountingSearch,
      handleAccountingReset,
      handleAccountingCreate,
      handleAccountingView,
      handleAccountingEdit,
      handleAccountingDelete,
      handleAccountingSubmit,
      handleAccountingSizeChange,
      handleAccountingPageChange,
      formatCurrency
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-syse-data-management {
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

