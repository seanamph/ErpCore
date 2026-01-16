<template>
  <div class="customers">
    <div class="page-header">
      <h1>客戶基本資料維護 (CUS5110)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="客戶編號">
          <el-input v-model="queryForm.CustomerId" placeholder="請輸入客戶編號" clearable />
        </el-form-item>
        <el-form-item label="客戶名稱">
          <el-input v-model="queryForm.CustomerName" placeholder="請輸入客戶名稱" clearable />
        </el-form-item>
        <el-form-item label="統一編號">
          <el-input v-model="queryForm.GuiId" placeholder="請輸入統一編號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="業務員">
          <el-input v-model="queryForm.SalesId" placeholder="請輸入業務員" clearable />
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
        <el-table-column prop="CustomerId" label="客戶編號" width="120" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="ShortName" label="簡稱" width="100" />
        <el-table-column prop="ContactStaff" label="聯絡人" width="100" />
        <el-table-column prop="CompTel" label="公司電話" width="120" />
        <el-table-column prop="Cell" label="手機" width="120" />
        <el-table-column prop="Email" label="電子郵件" width="180" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="AccAmt" label="累積金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.AccAmt) }}
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
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-tabs v-model="activeTab">
          <!-- 基本資料 -->
          <el-tab-pane label="基本資料" name="basic">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="客戶編號" prop="CustomerId">
                  <el-input v-model="form.CustomerId" placeholder="請輸入客戶編號" :disabled="isEdit" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="統一編號" prop="GuiId">
                  <el-input v-model="form.GuiId" placeholder="請輸入統一編號" @blur="handleValidateGuiId" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="識別類型" prop="GuiType">
                  <el-select v-model="form.GuiType" placeholder="請選擇識別類型" clearable style="width: 100%">
                    <el-option label="統一編號" value="1" />
                    <el-option label="身份證字號" value="2" />
                    <el-option label="自編編號" value="3" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="狀態" prop="Status">
                  <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
                    <el-option label="啟用" value="A" />
                    <el-option label="停用" value="I" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="客戶名稱" prop="CustomerName">
                  <el-input v-model="form.CustomerName" placeholder="請輸入客戶名稱" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="客戶名稱(英文)" prop="CustomerNameE">
                  <el-input v-model="form.CustomerNameE" placeholder="請輸入客戶名稱(英文)" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="簡稱" prop="ShortName">
              <el-input v-model="form.ShortName" placeholder="請輸入簡稱" />
            </el-form-item>
          </el-tab-pane>

          <!-- 聯絡資訊 -->
          <el-tab-pane label="聯絡資訊" name="contact">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="聯絡人" prop="ContactStaff">
                  <el-input v-model="form.ContactStaff" placeholder="請輸入聯絡人" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="職稱" prop="Title">
                  <el-input v-model="form.Title" placeholder="請輸入職稱" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="公司電話" prop="CompTel">
                  <el-input v-model="form.CompTel" placeholder="請輸入公司電話" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="住家電話" prop="HomeTel">
                  <el-input v-model="form.HomeTel" placeholder="請輸入住家電話" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="手機" prop="Cell">
                  <el-input v-model="form.Cell" placeholder="請輸入手機" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="傳真" prop="Fax">
                  <el-input v-model="form.Fax" placeholder="請輸入傳真" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="電子郵件" prop="Email">
              <el-input v-model="form.Email" placeholder="請輸入電子郵件" />
            </el-form-item>
            <el-form-item label="性別" prop="Sex">
              <el-select v-model="form.Sex" placeholder="請選擇性別" clearable style="width: 100%">
                <el-option label="男" value="M" />
                <el-option label="女" value="F" />
              </el-select>
            </el-form-item>
          </el-tab-pane>

          <!-- 地址資訊 -->
          <el-tab-pane label="地址資訊" name="address">
            <el-row :gutter="20">
              <el-col :span="8">
                <el-form-item label="城市" prop="City">
                  <el-input v-model="form.City" placeholder="請輸入城市" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="區域" prop="Canton">
                  <el-input v-model="form.Canton" placeholder="請輸入區域" />
                </el-form-item>
              </el-col>
              <el-col :span="8">
                <el-form-item label="郵遞區號" prop="PostId">
                  <el-input v-model="form.PostId" placeholder="請輸入郵遞區號" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="地址" prop="Addr">
              <el-input v-model="form.Addr" placeholder="請輸入地址" />
            </el-form-item>
            <el-form-item label="發票地址" prop="TaxAddr">
              <el-input v-model="form.TaxAddr" placeholder="請輸入發票地址" />
            </el-form-item>
            <el-form-item label="送貨地址" prop="DelyAddr">
              <el-input v-model="form.DelyAddr" placeholder="請輸入送貨地址" />
            </el-form-item>
          </el-tab-pane>

          <!-- 其他資訊 -->
          <el-tab-pane label="其他資訊" name="other">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="業務員" prop="SalesId">
                  <el-input v-model="form.SalesId" placeholder="請輸入業務員" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="是否享有折扣" prop="DiscountYn">
                  <el-select v-model="form.DiscountYn" placeholder="請選擇" clearable style="width: 100%">
                    <el-option label="是" value="Y" />
                    <el-option label="否" value="N" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="折扣類別代碼" prop="DiscountNo">
              <el-input v-model="form.DiscountNo" placeholder="請輸入折扣類別代碼" />
            </el-form-item>
            <el-form-item label="是否為月結客戶" prop="MonthlyYn">
              <el-select v-model="form.MonthlyYn" placeholder="請選擇" clearable style="width: 100%">
                <el-option label="是" value="Y" />
                <el-option label="否" value="N" />
              </el-select>
            </el-form-item>
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
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
import { customerApi } from '@/api/customer'

// 查詢表單
const queryForm = reactive({
  CustomerId: '',
  CustomerName: '',
  GuiId: '',
  Status: '',
  SalesId: ''
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
const dialogTitle = ref('新增客戶')
const isEdit = ref(false)
const formRef = ref(null)
const activeTab = ref('basic')

// 表單資料
const form = reactive({
  CustomerId: '',
  GuiId: '',
  GuiType: '',
  CustomerName: '',
  CustomerNameE: '',
  ShortName: '',
  ContactStaff: '',
  HomeTel: '',
  CompTel: '',
  Fax: '',
  Cell: '',
  Email: '',
  Sex: '',
  Title: '',
  City: '',
  Canton: '',
  Addr: '',
  TaxAddr: '',
  DelyAddr: '',
  PostId: '',
  DiscountYn: 'N',
  DiscountNo: '',
  SalesId: '',
  MonthlyYn: 'N',
  Status: 'A',
  Notes: '',
  Contacts: []
})

// 表單驗證規則
const rules = {
  CustomerId: [{ required: true, message: '請輸入客戶編號', trigger: 'blur' }],
  CustomerName: [{ required: true, message: '請輸入客戶名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 格式化貨幣
const formatCurrency = (value) => {
  if (value == null || value === undefined) return '$0'
  return new Intl.NumberFormat('zh-TW', {
    style: 'currency',
    currency: 'TWD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value)
}

// 驗證統一編號
const handleValidateGuiId = async () => {
  if (!form.GuiId || !form.GuiType) return
  try {
    const response = await customerApi.validateGuiId({
      GuiId: form.GuiId,
      GuiType: form.GuiType
    })
    if (response.data.success && response.data.data) {
      if (!response.data.data.isValid) {
        ElMessage.warning(response.data.data.message || '統一編號驗證失敗')
      }
    }
  } catch (error) {
    // 驗證失敗不顯示錯誤，僅在提交時驗證
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      CustomerId: queryForm.CustomerId || undefined,
      CustomerName: queryForm.CustomerName || undefined,
      GuiId: queryForm.GuiId || undefined,
      Status: queryForm.Status || undefined,
      SalesId: queryForm.SalesId || undefined
    }
    const response = await customerApi.getCustomers(params)
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
  queryForm.CustomerId = ''
  queryForm.CustomerName = ''
  queryForm.GuiId = ''
  queryForm.Status = ''
  queryForm.SalesId = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增客戶'
  activeTab.value = 'basic'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改客戶'
  activeTab.value = 'basic'
  try {
    const response = await customerApi.getCustomer(row.CustomerId)
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
    await ElMessageBox.confirm('確定要刪除此客戶嗎？', '提示', {
      type: 'warning'
    })
    const response = await customerApi.deleteCustomer(row.CustomerId)
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
        // 驗證統一編號
        if (form.GuiId && form.GuiType) {
          const validateResponse = await customerApi.validateGuiId({
            GuiId: form.GuiId,
            GuiType: form.GuiType
          })
          if (validateResponse.data.success && validateResponse.data.data) {
            if (!validateResponse.data.data.isValid) {
              ElMessage.error(validateResponse.data.data.message || '統一編號驗證失敗')
              return
            }
          }
        }

        let response
        if (isEdit.value) {
          const updateData = {
            GuiId: form.GuiId,
            GuiType: form.GuiType,
            CustomerName: form.CustomerName,
            CustomerNameE: form.CustomerNameE,
            ShortName: form.ShortName,
            ContactStaff: form.ContactStaff,
            HomeTel: form.HomeTel,
            CompTel: form.CompTel,
            Fax: form.Fax,
            Cell: form.Cell,
            Email: form.Email,
            Sex: form.Sex,
            Title: form.Title,
            City: form.City,
            Canton: form.Canton,
            Addr: form.Addr,
            TaxAddr: form.TaxAddr,
            DelyAddr: form.DelyAddr,
            PostId: form.PostId,
            DiscountYn: form.DiscountYn,
            DiscountNo: form.DiscountNo,
            SalesId: form.SalesId,
            MonthlyYn: form.MonthlyYn,
            Status: form.Status,
            Notes: form.Notes,
            Contacts: form.Contacts
          }
          response = await customerApi.updateCustomer(form.CustomerId, updateData)
        } else {
          response = await customerApi.createCustomer(form)
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
  form.CustomerId = ''
  form.GuiId = ''
  form.GuiType = ''
  form.CustomerName = ''
  form.CustomerNameE = ''
  form.ShortName = ''
  form.ContactStaff = ''
  form.HomeTel = ''
  form.CompTel = ''
  form.Fax = ''
  form.Cell = ''
  form.Email = ''
  form.Sex = ''
  form.Title = ''
  form.City = ''
  form.Canton = ''
  form.Addr = ''
  form.TaxAddr = ''
  form.DelyAddr = ''
  form.PostId = ''
  form.DiscountYn = 'N'
  form.DiscountNo = ''
  form.SalesId = ''
  form.MonthlyYn = 'N'
  form.Status = 'A'
  form.Notes = ''
  form.Contacts = []
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

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.customers {
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
