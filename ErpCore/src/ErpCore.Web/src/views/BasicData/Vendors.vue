<template>
  <div class="vendors">
    <div class="page-header">
      <h1>廠/客基本資料維護 (SYSB206)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="廠商編號">
          <el-input v-model="queryForm.VendorId" placeholder="請輸入廠商編號" clearable />
        </el-form-item>
        <el-form-item label="統一編號">
          <el-input v-model="queryForm.GuiId" placeholder="請輸入統一編號" clearable />
        </el-form-item>
        <el-form-item label="廠商名稱">
          <el-input v-model="queryForm.VendorName" placeholder="請輸入廠商名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
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
        <el-table-column prop="VendorId" label="廠商編號" width="150" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="VendorName" label="廠商名稱" width="200" />
        <el-table-column prop="VendorNameS" label="廠商簡稱" width="120" />
        <el-table-column prop="VendorRegTel" label="電話" width="120" />
        <el-table-column prop="ChargeStaff" label="聯絡人" width="100" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
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
      <el-tabs v-model="activeTab">
        <!-- 基本資料 -->
        <el-tab-pane label="基本資料" name="basic">
          <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="識別類型" prop="GuiType">
                  <el-radio-group v-model="form.GuiType">
                    <el-radio label="1">統一編號</el-radio>
                    <el-radio label="2">身份證字號</el-radio>
                    <el-radio label="3">自編編號</el-radio>
                  </el-radio-group>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="getGuiIdLabel()" prop="GuiId">
                  <el-input
                    v-model="form.GuiId"
                    :placeholder="getGuiIdPlaceholder()"
                    @blur="handleCheckGuiId"
                    :disabled="isEdit"
                  />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="廠商名稱(中文)" prop="VendorName">
                  <el-input v-model="form.VendorName" placeholder="請輸入廠商名稱" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="廠商名稱(英文)" prop="VendorNameE">
                  <el-input v-model="form.VendorNameE" placeholder="請輸入英文名稱" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="廠商簡稱" prop="VendorNameS">
              <el-input v-model="form.VendorNameS" placeholder="請輸入廠商簡稱" />
            </el-form-item>
            <el-form-item label="郵電費負擔" prop="Mcode">
              <el-input v-model="form.Mcode" placeholder="請輸入郵電費負擔" />
            </el-form-item>
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="form.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item label="系統別" prop="SysId">
              <el-input v-model="form.SysId" placeholder="請輸入系統別" />
            </el-form-item>
            <el-form-item label="組織代碼" prop="OrgId">
              <el-input v-model="form.OrgId" placeholder="請輸入組織代碼" />
            </el-form-item>
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 公司資料 -->
        <el-tab-pane label="公司資料" name="company">
          <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
            <el-form-item label="公司登記地址" prop="VendorRegAddr">
              <el-input v-model="form.VendorRegAddr" type="textarea" :rows="2" placeholder="請輸入公司登記地址" />
            </el-form-item>
            <el-form-item label="發票地址" prop="TaxAddr">
              <el-input v-model="form.TaxAddr" type="textarea" :rows="2" placeholder="請輸入發票地址" />
            </el-form-item>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="公司登記電話" prop="VendorRegTel">
                  <el-input v-model="form.VendorRegTel" placeholder="請輸入公司登記電話" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="公司傳真" prop="VendorFax">
                  <el-input v-model="form.VendorFax" placeholder="請輸入公司傳真" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="公司電子郵件" prop="VendorEmail">
                  <el-input v-model="form.VendorEmail" placeholder="請輸入公司電子郵件" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="發票電子郵件" prop="InvEmail">
                  <el-input v-model="form.InvEmail" placeholder="請輸入發票電子郵件" />
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-tab-pane>

        <!-- 聯絡人資料 -->
        <el-tab-pane label="聯絡人資料" name="contact">
          <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="聯絡人" prop="ChargeStaff">
                  <el-input v-model="form.ChargeStaff" placeholder="請輸入聯絡人" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="聯絡人職稱" prop="ChargeTitle">
                  <el-input v-model="form.ChargeTitle" placeholder="請輸入聯絡人職稱" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="聯絡人身份證字號" prop="ChargePid">
              <el-input v-model="form.ChargePid" placeholder="請輸入聯絡人身份證字號" />
            </el-form-item>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="聯絡人電話" prop="ChargeTel">
                  <el-input v-model="form.ChargeTel" placeholder="請輸入聯絡人電話" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="聯絡人電子郵件" prop="ChargeEmail">
                  <el-input v-model="form.ChargeEmail" placeholder="請輸入聯絡人電子郵件" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="聯絡人聯絡地址" prop="ChargeAddr">
              <el-input v-model="form.ChargeAddr" type="textarea" :rows="2" placeholder="請輸入聯絡人聯絡地址" />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 付款資料 -->
        <el-tab-pane label="付款資料" name="payment">
          <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
            <el-form-item label="付款方式" prop="PayType">
              <el-input v-model="form.PayType" placeholder="請輸入付款方式" />
            </el-form-item>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="匯款銀行代碼" prop="SuplBankId">
                  <el-input v-model="form.SuplBankId" placeholder="請輸入匯款銀行代碼" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="匯款銀行帳號" prop="SuplBankAcct">
                  <el-input v-model="form.SuplBankAcct" placeholder="請輸入匯款銀行帳號" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="帳戶名稱" prop="SuplAcctName">
              <el-input v-model="form.SuplAcctName" placeholder="請輸入帳戶名稱" />
            </el-form-item>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="票據別" prop="TicketBe">
                  <el-input v-model="form.TicketBe" placeholder="請輸入票據別" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="支票抬頭" prop="CheckTitle">
                  <el-input v-model="form.CheckTitle" placeholder="請輸入支票抬頭" />
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-tab-pane>
      </el-tabs>
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
import { vendorsApi } from '@/api/vendors'

// 查詢表單
const queryForm = reactive({
  VendorId: '',
  GuiId: '',
  VendorName: '',
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
const dialogTitle = ref('新增廠商')
const isEdit = ref(false)
const formRef = ref(null)
const activeTab = ref('basic')

// 表單資料
const form = reactive({
  VendorId: '',
  GuiId: '',
  GuiType: '1',
  VendorName: '',
  VendorNameE: '',
  VendorNameS: '',
  Mcode: '',
  VendorRegAddr: '',
  TaxAddr: '',
  VendorRegTel: '',
  VendorFax: '',
  VendorEmail: '',
  InvEmail: '',
  ChargeStaff: '',
  ChargeTitle: '',
  ChargePid: '',
  ChargeTel: '',
  ChargeAddr: '',
  ChargeEmail: '',
  Status: 'A',
  SysId: '1',
  PayType: '',
  SuplBankId: '',
  SuplBankAcct: '',
  SuplAcctName: '',
  TicketBe: '',
  CheckTitle: '',
  OrgId: '',
  Notes: ''
})

// 表單驗證規則
const rules = {
  GuiId: [{ required: true, message: '請輸入統一編號/身份證字號/自編編號', trigger: 'blur' }],
  VendorName: [{ required: true, message: '請輸入廠商名稱', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}

// 取得識別類型標籤
const getGuiIdLabel = () => {
  if (form.GuiType === '1') return '統一編號'
  if (form.GuiType === '2') return '身份證字號'
  if (form.GuiType === '3') return '自編編號'
  return '識別編號'
}

// 取得識別類型佔位符
const getGuiIdPlaceholder = () => {
  if (form.GuiType === '1') return '請輸入統一編號'
  if (form.GuiType === '2') return '請輸入身份證字號'
  if (form.GuiType === '3') return '請輸入自編編號'
  return '請輸入識別編號'
}

// 檢查統一編號
const handleCheckGuiId = async () => {
  if (!form.GuiId || isEdit.value) return
  try {
    const response = await vendorsApi.checkGuiId(form.GuiId)
    if (response.data.success && response.data.data.exists) {
      ElMessage.warning('統一編號已存在')
    }
  } catch (error) {
    // 忽略檢查錯誤
  }
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      VendorId: queryForm.VendorId || undefined,
      GuiId: queryForm.GuiId || undefined,
      VendorName: queryForm.VendorName || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await vendorsApi.getVendors(params)
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
  queryForm.VendorId = ''
  queryForm.GuiId = ''
  queryForm.VendorName = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增廠商'
  resetForm()
  activeTab.value = 'basic'
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改廠商'
  try {
    const response = await vendorsApi.getVendor(row.VendorId)
    if (response.data.success) {
      Object.assign(form, response.data.data)
    } else {
      ElMessage.error(response.data.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
  activeTab.value = 'basic'
  dialogVisible.value = true
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此廠商嗎？', '提示', {
      type: 'warning'
    })
    const response = await vendorsApi.deleteVendor(row.VendorId)
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
          const updateData = { ...form }
          delete updateData.VendorId
          delete updateData.GuiId
          delete updateData.GuiType
          response = await vendorsApi.updateVendor(form.VendorId, updateData)
        } else {
          const createData = { ...form }
          delete createData.VendorId
          response = await vendorsApi.createVendor(createData)
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
  form.VendorId = ''
  form.GuiId = ''
  form.GuiType = '1'
  form.VendorName = ''
  form.VendorNameE = ''
  form.VendorNameS = ''
  form.Mcode = ''
  form.VendorRegAddr = ''
  form.TaxAddr = ''
  form.VendorRegTel = ''
  form.VendorFax = ''
  form.VendorEmail = ''
  form.InvEmail = ''
  form.ChargeStaff = ''
  form.ChargeTitle = ''
  form.ChargePid = ''
  form.ChargeTel = ''
  form.ChargeAddr = ''
  form.ChargeEmail = ''
  form.Status = 'A'
  form.SysId = '1'
  form.PayType = ''
  form.SuplBankId = ''
  form.SuplBankAcct = ''
  form.SuplAcctName = ''
  form.TicketBe = ''
  form.CheckTitle = ''
  form.OrgId = ''
  form.Notes = ''
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

.vendors {
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

