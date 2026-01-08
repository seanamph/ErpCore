<template>
  <div class="product-goods-id">
    <div class="page-header">
      <h1>商品進銷碼維護作業 (SYSW137)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="進銷碼">
          <el-input v-model="queryForm.GoodsId" placeholder="請輸入進銷碼" clearable />
        </el-form-item>
        <el-form-item label="商品名稱">
          <el-input v-model="queryForm.GoodsName" placeholder="請輸入商品名稱" clearable />
        </el-form-item>
        <el-form-item label="國際條碼">
          <el-input v-model="queryForm.BarcodeId" placeholder="請輸入國際條碼" clearable />
        </el-form-item>
        <el-form-item label="小分類">
          <el-input v-model="queryForm.ScId" placeholder="請輸入小分類" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="正常" value="1" />
            <el-option label="停用" value="2" />
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
        <el-table-column prop="GoodsId" label="進銷碼" width="120" fixed="left" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" show-overflow-tooltip />
        <el-table-column prop="BarcodeId" label="國際條碼" width="150" />
        <el-table-column prop="ScName" label="小分類" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.StatusName || (row.Status === '1' ? '正常' : '停用') }}
            </el-tag>
          </template>
        </el-table-column>
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
        <el-table-column prop="Unit" label="單位" width="80" />
        <el-table-column prop="Tax" label="稅別" width="80">
          <template #default="{ row }">
            {{ row.TaxName || (row.Tax === '1' ? '應稅' : '免稅') }}
          </template>
        </el-table-column>
        <el-table-column prop="Discount" label="可折扣" width="80">
          <template #default="{ row }">
            {{ row.Discount === 'Y' ? '是' : '否' }}
          </template>
        </el-table-column>
        <el-table-column prop="AutoOrder" label="自動訂貨" width="100">
          <template #default="{ row }">
            {{ row.AutoOrder === 'Y' ? '是' : '否' }}
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
      width="1000px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
        <el-tabs v-model="activeTab">
          <!-- 基本資料 -->
          <el-tab-pane label="基本資料" name="basic">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="進銷碼" prop="GoodsId">
                  <el-input v-model="form.GoodsId" placeholder="請輸入進銷碼" :disabled="isEdit" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="商品名稱" prop="GoodsName">
                  <el-input v-model="form.GoodsName" placeholder="請輸入商品名稱" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="發票列印名稱" prop="InvPrintName">
                  <el-input v-model="form.InvPrintName" placeholder="請輸入發票列印名稱" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="商品規格" prop="GoodsSpace">
                  <el-input v-model="form.GoodsSpace" placeholder="請輸入商品規格" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="小分類" prop="ScId">
                  <el-input v-model="form.ScId" placeholder="請輸入小分類" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="稅別" prop="Tax">
                  <el-select v-model="form.Tax" placeholder="請選擇稅別" style="width: 100%">
                    <el-option label="應稅" value="1" />
                    <el-option label="免稅" value="0" />
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
                <el-form-item label="國際條碼" prop="BarcodeId">
                  <el-input v-model="form.BarcodeId" placeholder="請輸入國際條碼" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="單位" prop="Unit">
                  <el-input v-model="form.Unit" placeholder="請輸入單位" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="換算率" prop="ConvertRate">
                  <el-input-number v-model="form.ConvertRate" :min="1" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="狀態" prop="Status">
                  <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
                    <el-option label="正常" value="1" />
                    <el-option label="停用" value="2" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="可折扣" prop="Discount">
                  <el-radio-group v-model="form.Discount">
                    <el-radio label="Y">是</el-radio>
                    <el-radio label="N">否</el-radio>
                  </el-radio-group>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="自動訂貨" prop="AutoOrder">
                  <el-radio-group v-model="form.AutoOrder">
                    <el-radio label="Y">是</el-radio>
                    <el-radio label="N">否</el-radio>
                  </el-radio-group>
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>

          <!-- 進階設定 -->
          <el-tab-pane label="進階設定" name="advanced">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="價格種類" prop="PriceKind">
                  <el-input v-model="form.PriceKind" placeholder="請輸入價格種類" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="成本種類" prop="CostKind">
                  <el-input v-model="form.CostKind" placeholder="請輸入成本種類" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="安全庫存天數" prop="SafeDays">
                  <el-input-number v-model="form.SafeDays" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="有效期限天數" prop="ExpirationDays">
                  <el-input-number v-model="form.ExpirationDays" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="國別" prop="National">
                  <el-input v-model="form.National" placeholder="請輸入國別" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="產地" prop="Place">
                  <el-input v-model="form.Place" placeholder="請輸入產地" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="容量" prop="Capacity">
                  <el-input-number v-model="form.Capacity" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="容量單位" prop="CapacityUnit">
                  <el-input v-model="form.CapacityUnit" placeholder="請輸入容量單位" />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>

          <!-- 尺寸重量 -->
          <el-tab-pane label="尺寸重量" name="dimensions">
            <el-divider>商品尺寸 (公分)</el-divider>
            <el-row :gutter="20">
              <el-col :span="8">
                <el-form-item label="深" prop="GoodsDeep">
                  <el-input-number v-model="form.GoodsDeep" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="寬" prop="GoodsWide">
                  <el-input-number v-model="form.GoodsWide" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="高" prop="GoodsHigh">
                  <el-input-number v-model="form.GoodsHigh" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-divider>包裝尺寸 (公分)</el-divider>
            <el-row :gutter="20">
              <el-col :span="8">
                <el-form-item label="深" prop="PackDeep">
                  <el-input-number v-model="form.PackDeep" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="寬" prop="PackWide">
                  <el-input-number v-model="form.PackWide" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="高" prop="PackHigh">
                  <el-input-number v-model="form.PackHigh" :min="0" :step="1" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="包裝重量 (KG)" prop="PackWeight">
                  <el-input-number v-model="form.PackWeight" :min="0" :step="0.1" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
          </el-tab-pane>
        </el-tabs>
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
import { productGoodsIdApi } from '@/api/inventory'

// 查詢表單
const queryForm = reactive({
  GoodsId: '',
  GoodsName: '',
  BarcodeId: '',
  ScId: '',
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
const dialogTitle = ref('新增商品進銷碼')
const isEdit = ref(false)
const formRef = ref(null)
const activeTab = ref('basic')

// 表單資料
const form = reactive({
  GoodsId: '',
  GoodsName: '',
  InvPrintName: '',
  GoodsSpace: '',
  ScId: '',
  Tax: '1',
  Lprc: 0,
  Mprc: 0,
  BarcodeId: '',
  Unit: '',
  ConvertRate: 1,
  Capacity: 0,
  CapacityUnit: '',
  Status: '1',
  Discount: 'N',
  AutoOrder: 'N',
  PriceKind: '1',
  CostKind: '1',
  SafeDays: 0,
  ExpirationDays: 0,
  National: '',
  Place: '',
  GoodsDeep: 0,
  GoodsWide: 0,
  GoodsHigh: 0,
  PackDeep: 0,
  PackWide: 0,
  PackHigh: 0,
  PackWeight: 0
})

// 表單驗證規則
const rules = {
  GoodsId: [{ required: true, message: '請輸入進銷碼', trigger: 'blur' }],
  GoodsName: [{ required: true, message: '請輸入商品名稱', trigger: 'blur' }],
  Lprc: [{ type: 'number', min: 0, message: '進價不能小於0', trigger: 'blur' }],
  Mprc: [{ type: 'number', min: 0, message: '中價不能小於0', trigger: 'blur' }]
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      GoodsId: queryForm.GoodsId || undefined,
      GoodsName: queryForm.GoodsName || undefined,
      BarcodeId: queryForm.BarcodeId || undefined,
      ScId: queryForm.ScId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await productGoodsIdApi.getProductGoodsIds(params)
    if (response.data && response.data.Success !== false) {
      const data = response.data.Data || response.data
      tableData.value = data.Items || data.items || []
      pagination.TotalCount = data.TotalCount || data.totalCount || 0
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.GoodsId = ''
  queryForm.GoodsName = ''
  queryForm.BarcodeId = ''
  queryForm.ScId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增商品進銷碼'
  activeTab.value = 'basic'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改商品進銷碼'
  activeTab.value = 'basic'
  try {
    const response = await productGoodsIdApi.getProductGoodsIdById(row.GoodsId)
    if (response.data && response.data.Success !== false) {
      const data = response.data.Data || response.data
      Object.assign(form, {
        GoodsId: data.GoodsId || '',
        GoodsName: data.GoodsName || '',
        InvPrintName: data.InvPrintName || '',
        GoodsSpace: data.GoodsSpace || '',
        ScId: data.ScId || '',
        Tax: data.Tax || '1',
        Lprc: data.Lprc || 0,
        Mprc: data.Mprc || 0,
        BarcodeId: data.BarcodeId || '',
        Unit: data.Unit || '',
        ConvertRate: data.ConvertRate || 1,
        Capacity: data.Capacity || 0,
        CapacityUnit: data.CapacityUnit || '',
        Status: data.Status || '1',
        Discount: data.Discount || 'N',
        AutoOrder: data.AutoOrder || 'N',
        PriceKind: data.PriceKind || '1',
        CostKind: data.CostKind || '1',
        SafeDays: data.SafeDays || 0,
        ExpirationDays: data.ExpirationDays || 0,
        National: data.National || '',
        Place: data.Place || '',
        GoodsDeep: data.GoodsDeep || 0,
        GoodsWide: data.GoodsWide || 0,
        GoodsHigh: data.GoodsHigh || 0,
        PackDeep: data.PackDeep || 0,
        PackWide: data.PackWide || 0,
        PackHigh: data.PackHigh || 0,
        PackWeight: data.PackWeight || 0
      })
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + (error.message || '未知錯誤'))
  }
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此商品進銷碼嗎？', '提示', {
      type: 'warning'
    })
    const response = await productGoodsIdApi.deleteProductGoodsId(row.GoodsId)
    if (response.data && response.data.Success !== false) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.Message || response.data?.message || '刪除失敗')
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗：' + (error.message || '未知錯誤'))
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
          const updateData = { ...form }
          delete updateData.GoodsId
          response = await productGoodsIdApi.updateProductGoodsId(form.GoodsId, updateData)
        } else {
          response = await productGoodsIdApi.createProductGoodsId(form)
        }
        if (response.data && response.data.Success !== false) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.Message || response.data?.message || (isEdit.value ? '修改失敗' : '新增失敗'))
        }
      } catch (error) {
        ElMessage.error((isEdit.value ? '修改失敗' : '新增失敗') + '：' + (error.message || '未知錯誤'))
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
  form.GoodsId = ''
  form.GoodsName = ''
  form.InvPrintName = ''
  form.GoodsSpace = ''
  form.ScId = ''
  form.Tax = '1'
  form.Lprc = 0
  form.Mprc = 0
  form.BarcodeId = ''
  form.Unit = ''
  form.ConvertRate = 1
  form.Capacity = 0
  form.CapacityUnit = ''
  form.Status = '1'
  form.Discount = 'N'
  form.AutoOrder = 'N'
  form.PriceKind = '1'
  form.CostKind = '1'
  form.SafeDays = 0
  form.ExpirationDays = 0
  form.National = ''
  form.Place = ''
  form.GoodsDeep = 0
  form.GoodsWide = 0
  form.GoodsHigh = 0
  form.PackDeep = 0
  form.PackWide = 0
  form.PackHigh = 0
  form.PackWeight = 0
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

<style scoped>
.product-goods-id {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin: 0;
}

.table-card {
  margin-bottom: 20px;
}
</style>

