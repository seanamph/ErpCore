<template>
  <div class="supplier-goods">
    <div class="page-header">
      <h1>供應商商品資料維護 (SYSW110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="供應商編號">
          <el-input v-model="queryForm.SupplierId" placeholder="請輸入供應商編號" clearable />
        </el-form-item>
        <el-form-item label="商品條碼">
          <el-input v-model="queryForm.BarcodeId" placeholder="請輸入商品條碼" clearable />
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="queryForm.ShopId" placeholder="請輸入店別" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="正常" value="0" />
            <el-option label="停用" value="1" />
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
        <el-table-column prop="SupplierId" label="供應商編號" width="120" />
        <el-table-column prop="SupplierName" label="供應商名稱" width="150" />
        <el-table-column prop="BarcodeId" label="商品條碼" width="120" />
        <el-table-column prop="BarcodeName" label="商品名稱" width="150" />
        <el-table-column prop="ShopId" label="店別" width="100" />
        <el-table-column prop="Lprc" label="進價" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Lprc) }}
          </template>
        </el-table-column>
        <el-table-column prop="Mprc" label="中價" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Mprc) }}
          </template>
        </el-table-column>
        <el-table-column prop="Tax" label="稅別" width="80">
          <template #default="{ row }">
            {{ row.Tax === '1' ? '應稅' : '免稅' }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '0' ? 'success' : 'danger'">
              {{ row.Status === '0' ? '正常' : '停用' }}
            </el-tag>
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
      width="900px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="供應商編號" prop="SupplierId">
              <el-input v-model="form.SupplierId" placeholder="請輸入供應商編號" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="商品條碼" prop="BarcodeId">
              <el-input v-model="form.BarcodeId" placeholder="請輸入商品條碼" :disabled="isEdit" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="店別" prop="ShopId">
              <el-input v-model="form.ShopId" placeholder="請輸入店別" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="form.Status" placeholder="請選擇狀態">
                <el-option label="正常" value="0" />
                <el-option label="停用" value="1" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="進價" prop="Lprc">
              <el-input-number v-model="form.Lprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="中價" prop="Mprc">
              <el-input-number v-model="form.Mprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="稅別" prop="Tax">
              <el-select v-model="form.Tax" placeholder="請選擇稅別">
                <el-option label="應稅" value="1" />
                <el-option label="免稅" value="0" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="商品單位" prop="Unit">
              <el-input v-model="form.Unit" placeholder="請輸入商品單位" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="最小訂購量" prop="MinQty">
              <el-input-number v-model="form.MinQty" :precision="4" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="最大訂購量" prop="MaxQty">
              <el-input-number v-model="form.MaxQty" :precision="4" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="換算率" prop="Rate">
              <el-input-number v-model="form.Rate" :precision="4" :min="0" :step="0.01" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="到貨天數" prop="ArrivalDays">
              <el-input-number v-model="form.ArrivalDays" :min="0" :step="1" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="有效起始日" prop="StartDate">
              <el-date-picker v-model="form.StartDate" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="有效終止日" prop="EndDate">
              <el-date-picker v-model="form.EndDate" type="date" placeholder="請選擇日期" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="促銷價格" prop="Slprc">
              <el-input-number v-model="form.Slprc" :precision="4" :min="0" :step="0.01" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-divider>訂購日期設定</el-divider>
        <el-row :gutter="20">
          <el-col :span="6">
            <el-form-item label="週一">
              <el-switch v-model="form.OrdDay1" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="週二">
              <el-switch v-model="form.OrdDay2" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="週三">
              <el-switch v-model="form.OrdDay3" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="週四">
              <el-switch v-model="form.OrdDay4" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="6">
            <el-form-item label="週五">
              <el-switch v-model="form.OrdDay5" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="週六">
              <el-switch v-model="form.OrdDay6" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="週日">
              <el-switch v-model="form.OrdDay7" active-value="Y" inactive-value="N" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { supplierGoodsApi } from '@/api/inventory'

// 查詢表單
const queryForm = reactive({
  SupplierId: '',
  BarcodeId: '',
  ShopId: '',
  Status: ''
})

// 表格資料
const tableData = ref([])
const loading = ref(false)

// 分頁
const pagination = reactive({
  PageIndex: 1,
  PageSize: 20,
  TotalCount: 0
})

// 對話框
const dialogVisible = ref(false)
const dialogTitle = ref('新增供應商商品')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  SupplierId: '',
  BarcodeId: '',
  ShopId: '',
  Lprc: 0,
  Mprc: 0,
  Tax: '1',
  MinQty: 0,
  MaxQty: 0,
  Unit: '',
  Rate: 1,
  Status: '0',
  StartDate: null,
  EndDate: null,
  Slprc: 0,
  ArrivalDays: 0,
  OrdDay1: 'Y',
  OrdDay2: 'Y',
  OrdDay3: 'Y',
  OrdDay4: 'Y',
  OrdDay5: 'Y',
  OrdDay6: 'Y',
  OrdDay7: 'Y'
})

// 表單驗證規則
const rules = {
  SupplierId: [{ required: true, message: '請輸入供應商編號', trigger: 'blur' }],
  BarcodeId: [{ required: true, message: '請輸入商品條碼', trigger: 'blur' }],
  ShopId: [{ required: true, message: '請輸入店別', trigger: 'blur' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      SupplierId: queryForm.SupplierId || undefined,
      BarcodeId: queryForm.BarcodeId || undefined,
      ShopId: queryForm.ShopId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await supplierGoodsApi.getSupplierGoods(params)
    if (response.data.success) {
      tableData.value = response.data.data.items
      pagination.TotalCount = response.data.data.totalCount
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.SupplierId = ''
  queryForm.BarcodeId = ''
  queryForm.ShopId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增供應商商品'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改供應商商品'
  try {
    const response = await supplierGoodsApi.getSupplierGoodsById(row.SupplierId, row.BarcodeId, row.ShopId)
    if (response.data.success) {
      Object.assign(form, response.data.data)
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此供應商商品嗎？', '提示', {
      type: 'warning'
    })
    const response = await supplierGoodsApi.deleteSupplierGoods(row.SupplierId, row.BarcodeId, row.ShopId)
    if (response.data.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + error.message)
    }
  }
}

// 提交
const handleSubmit = async () => {
  if (!formRef.value) return
  await formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let response
        if (isEdit.value) {
          response = await supplierGoodsApi.updateSupplierGoods(
            form.SupplierId,
            form.BarcodeId,
            form.ShopId,
            form
          )
        } else {
          response = await supplierGoodsApi.createSupplierGoods(form)
        }
        if (response.data.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + error.message)
      }
    }
  })
}

// 關閉對話框
const handleDialogClose = () => {
  dialogVisible.value = false
  resetForm()
}

// 重置表單
const resetForm = () => {
  form.SupplierId = ''
  form.BarcodeId = ''
  form.ShopId = ''
  form.Lprc = 0
  form.Mprc = 0
  form.Tax = '1'
  form.MinQty = 0
  form.MaxQty = 0
  form.Unit = ''
  form.Rate = 1
  form.Status = '0'
  form.StartDate = null
  form.EndDate = null
  form.Slprc = 0
  form.ArrivalDays = 0
  form.OrdDay1 = 'Y'
  form.OrdDay2 = 'Y'
  form.OrdDay3 = 'Y'
  form.OrdDay4 = 'Y'
  form.OrdDay5 = 'Y'
  form.OrdDay6 = 'Y'
  form.OrdDay7 = 'Y'
  formRef.value?.resetFields()
}

// 分頁大小變更
const handleSizeChange = (size) => {
  pagination.PageSize = size
  pagination.PageIndex = 1
  handleSearch()
}

// 分頁變更
const handlePageChange = (page) => {
  pagination.PageIndex = page
  handleSearch()
}

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null) return '0.00'
  return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.supplier-goods {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
  }
}
</style>

