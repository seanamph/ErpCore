<template>
  <div class="prospect-list">
    <div class="page-header">
      <h1>潛客維護作業 (SYSC180)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="潛客代碼">
          <el-input v-model="queryForm.ProspectId" placeholder="請輸入潛客代碼" clearable />
        </el-form-item>
        <el-form-item label="潛客名稱">
          <el-input v-model="queryForm.ProspectName" placeholder="請輸入潛客名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="待訪談" value="PENDING" />
            <el-option label="訪談中" value="INTERVIEWING" />
            <el-option label="已簽約" value="SIGNED" />
            <el-option label="已取消" value="CANCELLED" />
          </el-select>
        </el-form-item>
        <el-form-item label="據點代碼">
          <el-input v-model="queryForm.SiteId" placeholder="請輸入據點代碼" clearable />
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
        <el-table-column prop="ProspectId" label="潛客代碼" width="150" />
        <el-table-column prop="ProspectName" label="潛客名稱" width="200" />
        <el-table-column prop="ContactPerson" label="聯絡人" width="120" />
        <el-table-column prop="ContactTel" label="聯絡電話" width="150" />
        <el-table-column prop="ContactEmail" label="電子郵件" width="200" />
        <el-table-column prop="StoreName" label="店名" width="150" />
        <el-table-column prop="SiteId" label="據點代碼" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ formatStatus(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ContactDate" label="聯絡日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ContactDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="info" size="small" @click="handleViewInterviews(row)">訪談記錄</el-button>
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
      width="90%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-tabs v-model="activeTab">
          <el-tab-pane label="基本資料" name="basic">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="潛客代碼" prop="ProspectId">
                  <el-input v-model="formData.ProspectId" :disabled="isEdit" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="潛客名稱" prop="ProspectName">
                  <el-input v-model="formData.ProspectName" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="聯絡人">
                  <el-input v-model="formData.ContactPerson" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="聯絡電話">
                  <el-input v-model="formData.ContactTel" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="傳真">
                  <el-input v-model="formData.ContactFax" />
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
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="店名">
                  <el-input v-model="formData.StoreName" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="店電話">
                  <el-input v-model="formData.StoreTel" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="據點代碼">
                  <el-input v-model="formData.SiteId" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="招商代碼">
                  <el-input v-model="formData.RecruitId" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="狀態" prop="Status">
                  <el-select v-model="formData.Status" placeholder="請選擇狀態">
                    <el-option label="待訪談" value="PENDING" />
                    <el-option label="訪談中" value="INTERVIEWING" />
                    <el-option label="已簽約" value="SIGNED" />
                    <el-option label="已取消" value="CANCELLED" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="聯絡日期">
                  <el-date-picker
                    v-model="formData.ContactDate"
                    type="date"
                    placeholder="請選擇聯絡日期"
                    format="YYYY-MM-DD"
                    value-format="YYYY-MM-DD"
                  />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>
          <el-tab-pane label="業務資料" name="business">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="基本租金">
                  <el-input-number v-model="formData.BaseRent" :precision="2" :min="0" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="保證金">
                  <el-input-number v-model="formData.Deposit" :precision="2" :min="0" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="目標金額(月)">
                  <el-input-number v-model="formData.TargetAmountM" :precision="2" :min="0" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="目標金額(年)">
                  <el-input-number v-model="formData.TargetAmountV" :precision="2" :min="0" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="到期日">
                  <el-date-picker
                    v-model="formData.DueDate"
                    type="date"
                    placeholder="請選擇到期日"
                    format="YYYY-MM-DD"
                    value-format="YYYY-MM-DD"
                  />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="回饋日期">
                  <el-date-picker
                    v-model="formData.FeedbackDate"
                    type="date"
                    placeholder="請選擇回饋日期"
                    format="YYYY-MM-DD"
                    value-format="YYYY-MM-DD"
                  />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>
          <el-tab-pane label="其他資訊" name="other">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="統一編號">
                  <el-input v-model="formData.GuiId" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="銀行代碼">
                  <el-input v-model="formData.BankId" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="帳戶名稱">
                  <el-input v-model="formData.AccName" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="帳戶號碼">
                  <el-input v-model="formData.AccNo" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="發票電子郵件">
              <el-input v-model="formData.InvEmail" />
            </el-form-item>
            <el-form-item label="備註">
              <el-input v-model="formData.Notes" type="textarea" :rows="4" />
            </el-form-item>
          </el-tab-pane>
        </el-tabs>
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
import { useRouter } from 'vue-router'
import { prospectApi } from '@/api/recruitment'

export default {
  name: 'ProspectList',
  setup() {
    const router = useRouter()
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const activeTab = ref('basic')
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ProspectId: '',
      ProspectName: '',
      Status: '',
      SiteId: '',
      RecruitId: '',
      ContactDateFrom: '',
      ContactDateTo: '',
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
      ProspectId: '',
      ProspectName: '',
      ContactPerson: '',
      ContactTel: '',
      ContactFax: '',
      ContactEmail: '',
      ContactAddress: '',
      StoreName: '',
      StoreTel: '',
      SiteId: '',
      RecruitId: '',
      StoreId: '',
      VendorId: '',
      OrgId: '',
      BtypeId: '',
      SalesType: '',
      Status: 'PENDING',
      OverallStatus: '',
      PaperType: '',
      LocationType: '',
      DecoType: '',
      CommType: '',
      PdType: '',
      BaseRent: null,
      Deposit: null,
      CreditCard: '',
      TargetAmountM: null,
      TargetAmountV: null,
      ExerciseFees: null,
      CheckDay: null,
      AgmDateB: null,
      AgmDateE: null,
      ContractProidB: '',
      ContractProidE: '',
      FeedbackDate: null,
      DueDate: null,
      ContactDate: null,
      VersionNo: '',
      GuiId: '',
      BankId: '',
      AccName: '',
      AccNo: '',
      InvEmail: '',
      EdcYn: 'N',
      ReceYn: 'N',
      PosYn: 'N',
      CashYn: 'N',
      CommYn: 'N',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      ProspectId: [{ required: true, message: '請輸入潛客代碼', trigger: 'blur' }],
      ProspectName: [{ required: true, message: '請輸入潛客名稱', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化狀態
    const formatStatus = (status) => {
      const statuses = {
        PENDING: '待訪談',
        INTERVIEWING: '訪談中',
        SIGNED: '已簽約',
        CANCELLED: '已取消'
      }
      return statuses[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        PENDING: 'warning',
        INTERVIEWING: 'primary',
        SIGNED: 'success',
        CANCELLED: 'danger'
      }
      return types[status] || ''
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      return new Date(date).toLocaleDateString('zh-TW')
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
        const response = await prospectApi.getProspects(params)
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
        ProspectId: '',
        ProspectName: '',
        Status: '',
        SiteId: '',
        RecruitId: '',
        ContactDateFrom: '',
        ContactDateTo: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      activeTab.value = 'basic'
      dialogVisible.value = true
      Object.assign(formData, {
        ProspectId: '',
        ProspectName: '',
        ContactPerson: '',
        ContactTel: '',
        ContactFax: '',
        ContactEmail: '',
        ContactAddress: '',
        StoreName: '',
        StoreTel: '',
        SiteId: '',
        RecruitId: '',
        StoreId: '',
        VendorId: '',
        OrgId: '',
        BtypeId: '',
        SalesType: '',
        Status: 'PENDING',
        OverallStatus: '',
        PaperType: '',
        LocationType: '',
        DecoType: '',
        CommType: '',
        PdType: '',
        BaseRent: null,
        Deposit: null,
        CreditCard: '',
        TargetAmountM: null,
        TargetAmountV: null,
        ExerciseFees: null,
        CheckDay: null,
        AgmDateB: null,
        AgmDateE: null,
        ContractProidB: '',
        ContractProidE: '',
        FeedbackDate: null,
        DueDate: null,
        ContactDate: null,
        VersionNo: '',
        GuiId: '',
        BankId: '',
        AccName: '',
        AccNo: '',
        InvEmail: '',
        EdcYn: 'N',
        ReceYn: 'N',
        PosYn: 'N',
        CashYn: 'N',
        CommYn: 'N',
        Notes: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await prospectApi.getProspect(row.ProspectId)
        if (response.Data) {
          isEdit.value = true
          activeTab.value = 'basic'
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

    // 查看訪談記錄
    const handleViewInterviews = (row) => {
      router.push(`/recruitment/interviews?prospectId=${row.ProspectId}`)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此潛客嗎？', '確認', {
          type: 'warning'
        })
        await prospectApi.deleteProspect(row.ProspectId)
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
              await prospectApi.updateProspect(formData.ProspectId, formData)
              ElMessage.success('修改成功')
            } else {
              await prospectApi.createProspect(formData)
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
      return isEdit.value ? '修改潛客' : '新增潛客'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      activeTab,
      formRef,
      tableData,
      queryForm,
      pagination,
      formData,
      formRules,
      dialogTitle,
      formatStatus,
      getStatusType,
      formatDate,
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleViewInterviews,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.prospect-list {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>

