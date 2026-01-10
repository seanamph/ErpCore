<template>
  <div class="lease-data-management">
    <div class="page-header">
      <h1>租賃資料維護 (SYS8110-SYS8220)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="租賃編號">
          <el-input v-model="queryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
        </el-form-item>
        <el-form-item label="租戶代號">
          <el-input v-model="queryForm.TenantId" placeholder="請輸入租戶代號" clearable />
        </el-form-item>
        <el-form-item label="分店代碼">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入分店代碼" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="草稿" value="D" />
            <el-option label="已簽約" value="S" />
            <el-option label="已生效" value="E" />
            <el-option label="已終止" value="T" />
          </el-select>
        </el-form-item>
        <el-form-item label="租期開始日">
          <el-date-picker
            v-model="queryForm.StartDateRange"
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
        <el-table-column prop="LeaseId" label="租賃編號" width="150" />
        <el-table-column prop="TenantId" label="租戶代號" width="120" />
        <el-table-column prop="TenantName" label="租戶名稱" width="200" />
        <el-table-column prop="ShopId" label="分店代碼" width="120" />
        <el-table-column prop="LeaseDate" label="租賃日期" width="120" />
        <el-table-column prop="StartDate" label="租期開始日" width="120" />
        <el-table-column prop="EndDate" label="租期結束日" width="120" />
        <el-table-column prop="MonthlyRent" label="月租金" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.MonthlyRent) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalRent" label="總租金" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalRent) }}
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
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'E'">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.Status === 'E'">刪除</el-button>
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
            <el-form-item label="租賃編號" prop="LeaseId">
              <el-input v-model="formData.LeaseId" :disabled="isEdit" placeholder="請輸入租賃編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="租戶代號" prop="TenantId">
              <el-input v-model="formData.TenantId" placeholder="請輸入租戶代號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="租戶名稱" prop="TenantName">
              <el-input v-model="formData.TenantName" placeholder="請輸入租戶名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="formData.ShopId" placeholder="請輸入分店代碼" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="樓層代碼" prop="FloorId">
              <el-input v-model="formData.FloorId" placeholder="請輸入樓層代碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="位置代碼" prop="LocationId">
              <el-input v-model="formData.LocationId" placeholder="請輸入位置代碼" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="租賃日期" prop="LeaseDate">
              <el-date-picker
                v-model="formData.LeaseDate"
                type="date"
                placeholder="請選擇租賃日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="租期開始日" prop="StartDate">
              <el-date-picker
                v-model="formData.StartDate"
                type="date"
                placeholder="請選擇租期開始日"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="租期結束日" prop="EndDate">
              <el-date-picker
                v-model="formData.EndDate"
                type="date"
                placeholder="請選擇租期結束日"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="草稿" value="D" />
                <el-option label="已簽約" value="S" />
                <el-option label="已生效" value="E" />
                <el-option label="已終止" value="T" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="月租金" prop="MonthlyRent">
              <el-input-number v-model="formData.MonthlyRent" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="總租金" prop="TotalRent">
              <el-input-number v-model="formData.TotalRent" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="押金" prop="Deposit">
              <el-input-number v-model="formData.Deposit" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="幣別" prop="CurrencyId">
              <el-select v-model="formData.CurrencyId" placeholder="請選擇幣別" style="width: 100%">
                <el-option label="新台幣" value="TWD" />
                <el-option label="美元" value="USD" />
                <el-option label="人民幣" value="CNY" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="付款方式" prop="PaymentMethod">
              <el-select v-model="formData.PaymentMethod" placeholder="請選擇付款方式" style="width: 100%">
                <el-option label="月付" value="MONTHLY" />
                <el-option label="季付" value="QUARTERLY" />
                <el-option label="年付" value="YEARLY" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="付款日" prop="PaymentDay">
              <el-input-number v-model="formData.PaymentDay" :min="1" :max="31" style="width: 100%" />
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
  name: 'LeaseDataManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      LeaseId: '',
      TenantId: '',
      ShopId: '',
      Status: '',
      StartDateRange: null,
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
      LeaseId: '',
      TenantId: '',
      TenantName: '',
      ShopId: '',
      FloorId: '',
      LocationId: '',
      LeaseDate: '',
      StartDate: '',
      EndDate: '',
      Status: 'D',
      MonthlyRent: 0,
      TotalRent: 0,
      Deposit: 0,
      CurrencyId: 'TWD',
      PaymentMethod: '',
      PaymentDay: 1,
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      TenantId: [{ required: true, message: '請輸入租戶代號', trigger: 'blur' }],
      ShopId: [{ required: true, message: '請輸入分店代碼', trigger: 'blur' }],
      LeaseDate: [{ required: true, message: '請選擇租賃日期', trigger: 'change' }],
      StartDate: [{ required: true, message: '請選擇租期開始日', trigger: 'change' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改租賃' : '新增租賃'
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
        if (queryForm.StartDateRange && queryForm.StartDateRange.length === 2) {
          params.StartDateFrom = queryForm.StartDateRange[0]
          params.StartDateTo = queryForm.StartDateRange[1]
        }
        delete params.StartDateRange
        const response = await leaseApi.getLeases(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃列表失敗:', error)
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
        LeaseId: '',
        TenantId: '',
        ShopId: '',
        Status: '',
        StartDateRange: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        LeaseId: '',
        TenantId: '',
        TenantName: '',
        ShopId: '',
        FloorId: '',
        LocationId: '',
        LeaseDate: '',
        StartDate: '',
        EndDate: '',
        Status: 'D',
        MonthlyRent: 0,
        TotalRent: 0,
        Deposit: 0,
        CurrencyId: 'TWD',
        PaymentMethod: '',
        PaymentDay: 1,
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await leaseApi.getLease(row.LeaseId)
        if (response.data && response.data.Success) {
          Object.assign(formData, response.data.Data)
          isEdit.value = true
          dialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃失敗:', error)
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
        await ElMessageBox.confirm(
          `確定要刪除租賃「${row.LeaseId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseApi.deleteLease(row.LeaseId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除租賃失敗:', error)
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
          await leaseApi.updateLease(formData.LeaseId, formData)
          ElMessage.success('修改成功')
        } else {
          await leaseApi.createLease(formData)
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

    // 取得狀態類型
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'S': 'warning',
        'E': 'success',
        'T': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'D': '草稿',
        'S': '已簽約',
        'E': '已生效',
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
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange,
      formatCurrency,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-data-management {
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

