<template>
  <div class="procurement-management">
    <div class="page-header">
      <h1>採購管理 (SYSP110-SYSP190)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="採購單號">
          <el-input v-model="queryForm.OrderId" placeholder="請輸入採購單號" clearable />
        </el-form-item>
        <el-form-item label="供應商">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商代號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="待處理" value="P" />
            <el-option label="已確認" value="C" />
            <el-option label="已取消" value="X" />
          </el-select>
        </el-form-item>
        <el-form-item label="日期範圍">
          <el-date-picker
            v-model="queryForm.DateRange"
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
        <el-table-column prop="OrderId" label="採購單號" width="150" />
        <el-table-column prop="SupplierId" label="供應商代號" width="120" />
        <el-table-column prop="SupplierName" label="供應商名稱" width="200" />
        <el-table-column prop="OrderDate" label="採購日期" width="120" />
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
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)" :disabled="row.Status === 'C'">修改</el-button>
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
            <el-form-item label="採購單號" prop="OrderId">
              <el-input v-model="formData.OrderId" :disabled="isEdit" placeholder="系統自動產生" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供應商" prop="SupplierId">
              <el-select v-model="formData.SupplierId" placeholder="請選擇供應商" style="width: 100%" filterable>
                <el-option
                  v-for="supplier in supplierList"
                  :key="supplier.SupplierId"
                  :label="`${supplier.SupplierId} - ${supplier.SupplierName}`"
                  :value="supplier.SupplierId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="採購日期" prop="OrderDate">
              <el-date-picker
                v-model="formData.OrderDate"
                type="date"
                placeholder="請選擇採購日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="交貨日期" prop="DeliveryDate">
              <el-date-picker
                v-model="formData.DeliveryDate"
                type="date"
                placeholder="請選擇交貨日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="2" placeholder="請輸入備註" />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- 明細表格 -->
        <el-divider content-position="left">採購明細</el-divider>
        <el-table :data="formData.Items" border style="width: 100%">
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column prop="GoodsId" label="商品編號" width="150">
            <template #default="{ row, $index }">
              <el-input v-model="row.GoodsId" placeholder="請輸入商品編號" />
            </template>
          </el-table-column>
          <el-table-column prop="GoodsName" label="商品名稱" min-width="200">
            <template #default="{ row }">
              <el-input v-model="row.GoodsName" placeholder="請輸入商品名稱" />
            </template>
          </el-table-column>
          <el-table-column prop="Quantity" label="數量" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.Quantity" :min="0" :precision="2" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column prop="UnitPrice" label="單價" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.UnitPrice" :min="0" :precision="2" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column prop="Amount" label="金額" width="120">
            <template #default="{ row }">
              {{ formatCurrency((row.Quantity || 0) * (row.UnitPrice || 0)) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="removeItem($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <div style="margin-top: 10px;">
          <el-button type="primary" @click="addItem">新增明細</el-button>
        </div>
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
import { procurementApi } from '@/api/procurement'

export default {
  name: 'ProcurementManagement',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const supplierList = ref([])

    // 查詢表單
    const queryForm = reactive({
      OrderId: '',
      SupplierId: '',
      Status: '',
      DateRange: null,
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
      OrderId: '',
      SupplierId: '',
      OrderDate: '',
      DeliveryDate: '',
      Memo: '',
      Items: []
    })

    // 表單驗證規則
    const formRules = {
      SupplierId: [{ required: true, message: '請選擇供應商', trigger: 'change' }],
      OrderDate: [{ required: true, message: '請選擇採購日期', trigger: 'change' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改採購單' : '新增採購單'
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
        if (queryForm.DateRange && queryForm.DateRange.length === 2) {
          params.StartDate = queryForm.DateRange[0]
          params.EndDate = queryForm.DateRange[1]
        }
        delete params.DateRange
        const response = await procurementApi.getPurchaseOrders(params)
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

    // 載入供應商列表
    const loadSuppliers = async () => {
      try {
        const response = await procurementApi.getSuppliers({ PageSize: 1000 })
        if (response.Data) {
          supplierList.value = response.Data.Items || []
        }
      } catch (error) {
        console.error('載入供應商列表失敗:', error)
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
        OrderId: '',
        SupplierId: '',
        Status: '',
        DateRange: null
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        OrderId: '',
        SupplierId: '',
        OrderDate: new Date().toISOString().split('T')[0],
        DeliveryDate: '',
        Memo: '',
        Items: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getPurchaseOrder(row.OrderId)
        if (response.Data) {
          Object.assign(formData, response.Data)
          isEdit.value = true
          dialogVisible.value = true
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
        await ElMessageBox.confirm(
          `確定要刪除採購單「${row.OrderId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.deletePurchaseOrder(row.OrderId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 新增明細
    const addItem = () => {
      formData.Items.push({
        GoodsId: '',
        GoodsName: '',
        Quantity: 0,
        UnitPrice: 0,
        Amount: 0
      })
    }

    // 刪除明細
    const removeItem = (index) => {
      formData.Items.splice(index, 1)
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()
        if (isEdit.value) {
          await procurementApi.updatePurchaseOrder(formData.OrderId, formData)
          ElMessage.success('修改成功')
        } else {
          await procurementApi.createPurchaseOrder(formData)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
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
        'P': 'warning',
        'C': 'success',
        'X': 'danger'
      }
      return statusMap[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const statusMap = {
        'P': '待處理',
        'C': '已確認',
        'X': '已取消'
      }
      return statusMap[status] || status
    }

    // 初始化
    onMounted(() => {
      loadData()
      loadSuppliers()
    })

    return {
      loading,
      dialogVisible,
      isEdit,
      formRef,
      tableData,
      supplierList,
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
      addItem,
      removeItem,
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

.procurement-management {
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
      .el-button {
        margin-right: 5px;
      }
    }
  }
}
</style>

