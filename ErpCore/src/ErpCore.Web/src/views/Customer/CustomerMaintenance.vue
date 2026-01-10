<template>
  <div class="customer-maintenance">
    <div class="page-header">
      <h1>客戶基本資料維護作業 (CUS5110)</h1>
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
        <el-table-column prop="CustomerId" label="客戶編號" width="150" />
        <el-table-column prop="CustomerName" label="客戶名稱" width="200" />
        <el-table-column prop="GuiId" label="統一編號" width="120" />
        <el-table-column prop="ShortName" label="簡稱" width="120" />
        <el-table-column prop="ContactStaff" label="聯絡人" width="120" />
        <el-table-column prop="CompTel" label="公司電話" width="150" />
        <el-table-column prop="Cell" label="手機" width="120" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="SalesId" label="業務員" width="120" />
        <el-table-column prop="AccAmt" label="累積金額" width="120">
          <template #default="{ row }">
            {{ formatCurrency(row.AccAmt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
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
      width="80%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-tabs v-model="activeTab">
          <el-tab-pane label="基本資料" name="basic">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="客戶編號" prop="CustomerId">
                  <el-input v-model="formData.CustomerId" :disabled="isEdit" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="客戶名稱" prop="CustomerName">
                  <el-input v-model="formData.CustomerName" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="統一編號" prop="GuiId">
                  <el-input v-model="formData.GuiId" />
                  <el-button type="primary" size="small" @click="handleValidateGuiId" style="margin-top: 5px;">驗證</el-button>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="識別類型" prop="GuiType">
                  <el-select v-model="formData.GuiType" placeholder="請選擇識別類型" clearable>
                    <el-option label="統一編號" value="1" />
                    <el-option label="身份證字號" value="2" />
                    <el-option label="自編編號" value="3" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="簡稱">
                  <el-input v-model="formData.ShortName" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="聯絡人">
                  <el-input v-model="formData.ContactStaff" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="公司電話">
                  <el-input v-model="formData.CompTel" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="住家電話">
                  <el-input v-model="formData.HomeTel" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="手機">
                  <el-input v-model="formData.Cell" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="傳真">
                  <el-input v-model="formData.Fax" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="電子郵件">
                  <el-input v-model="formData.Email" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="狀態" prop="Status">
                  <el-select v-model="formData.Status" placeholder="請選擇狀態">
                    <el-option label="啟用" value="A" />
                    <el-option label="停用" value="I" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="業務員">
                  <el-input v-model="formData.SalesId" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="月結客戶">
                  <el-select v-model="formData.MonthlyYn" placeholder="請選擇">
                    <el-option label="是" value="Y" />
                    <el-option label="否" value="N" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="備註">
              <el-input v-model="formData.Notes" type="textarea" :rows="3" />
            </el-form-item>
          </el-tab-pane>
          <el-tab-pane label="地址資訊" name="address">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="城市">
                  <el-input v-model="formData.City" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="區域">
                  <el-input v-model="formData.Canton" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item label="地址">
              <el-input v-model="formData.Addr" type="textarea" :rows="2" />
            </el-form-item>
            <el-form-item label="發票地址">
              <el-input v-model="formData.TaxAddr" type="textarea" :rows="2" />
            </el-form-item>
            <el-form-item label="送貨地址">
              <el-input v-model="formData.DelyAddr" type="textarea" :rows="2" />
            </el-form-item>
            <el-form-item label="郵遞區號">
              <el-input v-model="formData.PostId" />
            </el-form-item>
          </el-tab-pane>
          <el-tab-pane label="聯絡人" name="contacts">
            <el-button type="primary" size="small" @click="handleAddContact" style="margin-bottom: 10px;">新增聯絡人</el-button>
            <el-table :data="formData.Contacts" border style="width: 100%">
              <el-table-column prop="ContactName" label="姓名" width="150" />
              <el-table-column prop="ContactTitle" label="職稱" width="150" />
              <el-table-column prop="ContactTel" label="電話" width="150" />
              <el-table-column prop="ContactCell" label="手機" width="150" />
              <el-table-column prop="ContactEmail" label="電子郵件" />
              <el-table-column prop="IsPrimary" label="主要聯絡人" width="120">
                <template #default="{ row }">
                  <el-tag v-if="row.IsPrimary" type="success">是</el-tag>
                  <span v-else>否</span>
                </template>
              </el-table-column>
              <el-table-column label="操作" width="150">
                <template #default="{ row, $index }">
                  <el-button type="warning" size="small" @click="handleEditContact($index)">修改</el-button>
                  <el-button type="danger" size="small" @click="handleDeleteContact($index)">刪除</el-button>
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 聯絡人對話框 -->
    <el-dialog
      v-model="contactDialogVisible"
      :title="contactDialogTitle"
      width="600px"
    >
      <el-form
        ref="contactFormRef"
        :model="contactFormData"
        :rules="contactFormRules"
        label-width="100px"
      >
        <el-form-item label="姓名" prop="ContactName">
          <el-input v-model="contactFormData.ContactName" />
        </el-form-item>
        <el-form-item label="職稱">
          <el-input v-model="contactFormData.ContactTitle" />
        </el-form-item>
        <el-form-item label="電話">
          <el-input v-model="contactFormData.ContactTel" />
        </el-form-item>
        <el-form-item label="手機">
          <el-input v-model="contactFormData.ContactCell" />
        </el-form-item>
        <el-form-item label="電子郵件">
          <el-input v-model="contactFormData.ContactEmail" />
        </el-form-item>
        <el-form-item label="主要聯絡人">
          <el-switch v-model="contactFormData.IsPrimary" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="contactDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleContactSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { customerApi } from '@/api/customer'

export default {
  name: 'CustomerMaintenance',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const contactDialogVisible = ref(false)
    const isEdit = ref(false)
    const isEditContact = ref(false)
    const editContactIndex = ref(-1)
    const activeTab = ref('basic')
    const formRef = ref(null)
    const contactFormRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      CustomerId: '',
      CustomerName: '',
      GuiId: '',
      Status: '',
      SalesId: '',
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
      CustomerId: '',
      CustomerName: '',
      GuiId: '',
      GuiType: '',
      ShortName: '',
      ContactStaff: '',
      HomeTel: '',
      CompTel: '',
      Fax: '',
      Cell: '',
      Email: '',
      City: '',
      Canton: '',
      Addr: '',
      TaxAddr: '',
      DelyAddr: '',
      PostId: '',
      Status: 'A',
      SalesId: '',
      MonthlyYn: 'N',
      Notes: '',
      Contacts: []
    })

    // 聯絡人表單資料
    const contactFormData = reactive({
      ContactName: '',
      ContactTitle: '',
      ContactTel: '',
      ContactCell: '',
      ContactEmail: '',
      IsPrimary: false
    })

    // 表單驗證規則
    const formRules = {
      CustomerId: [{ required: true, message: '請輸入客戶編號', trigger: 'blur' }],
      CustomerName: [{ required: true, message: '請輸入客戶名稱', trigger: 'blur' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 聯絡人表單驗證規則
    const contactFormRules = {
      ContactName: [{ required: true, message: '請輸入聯絡人姓名', trigger: 'blur' }]
    }

    // 格式化貨幣
    const formatCurrency = (amount) => {
      if (!amount) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
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
        const response = await customerApi.getCustomers(params)
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
        CustomerId: '',
        CustomerName: '',
        GuiId: '',
        Status: '',
        SalesId: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      activeTab.value = 'basic'
      dialogVisible.value = true
      Object.assign(formData, {
        CustomerId: '',
        CustomerName: '',
        GuiId: '',
        GuiType: '',
        ShortName: '',
        ContactStaff: '',
        HomeTel: '',
        CompTel: '',
        Fax: '',
        Cell: '',
        Email: '',
        City: '',
        Canton: '',
        Addr: '',
        TaxAddr: '',
        DelyAddr: '',
        PostId: '',
        Status: 'A',
        SalesId: '',
        MonthlyYn: 'N',
        Notes: '',
        Contacts: []
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await customerApi.getCustomer(row.CustomerId)
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

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此客戶嗎？', '確認', {
          type: 'warning'
        })
        await customerApi.deleteCustomer(row.CustomerId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 驗證統一編號
    const handleValidateGuiId = async () => {
      if (!formData.GuiId) {
        ElMessage.warning('請先輸入統一編號')
        return
      }
      try {
        const response = await customerApi.validateGuiId({
          GuiId: formData.GuiId,
          GuiType: formData.GuiType
        })
        if (response.Data && response.Data.IsValid) {
          ElMessage.success('驗證成功')
        } else {
          ElMessage.warning(response.Data?.Message || '驗證失敗')
        }
      } catch (error) {
        ElMessage.error('驗證失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 新增聯絡人
    const handleAddContact = () => {
      isEditContact.value = false
      editContactIndex.value = -1
      contactDialogVisible.value = true
      Object.assign(contactFormData, {
        ContactName: '',
        ContactTitle: '',
        ContactTel: '',
        ContactCell: '',
        ContactEmail: '',
        IsPrimary: false
      })
    }

    // 修改聯絡人
    const handleEditContact = (index) => {
      isEditContact.value = true
      editContactIndex.value = index
      contactDialogVisible.value = true
      Object.assign(contactFormData, formData.Contacts[index])
    }

    // 刪除聯絡人
    const handleDeleteContact = (index) => {
      formData.Contacts.splice(index, 1)
    }

    // 提交聯絡人
    const handleContactSubmit = async () => {
      if (!contactFormRef.value) return
      await contactFormRef.value.validate(async (valid) => {
        if (valid) {
          if (isEditContact.value && editContactIndex.value >= 0) {
            formData.Contacts[editContactIndex.value] = { ...contactFormData }
          } else {
            formData.Contacts.push({ ...contactFormData })
          }
          contactDialogVisible.value = false
        }
      })
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await customerApi.updateCustomer(formData.CustomerId, formData)
              ElMessage.success('修改成功')
            } else {
              await customerApi.createCustomer(formData)
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
      return isEdit.value ? '修改客戶' : '新增客戶'
    })

    const contactDialogTitle = computed(() => {
      return isEditContact.value ? '修改聯絡人' : '新增聯絡人'
    })

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      dialogVisible,
      contactDialogVisible,
      isEdit,
      isEditContact,
      activeTab,
      formRef,
      contactFormRef,
      tableData,
      queryForm,
      pagination,
      formData,
      contactFormData,
      formRules,
      contactFormRules,
      dialogTitle,
      contactDialogTitle,
      formatCurrency,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleValidateGuiId,
      handleAddContact,
      handleEditContact,
      handleDeleteContact,
      handleContactSubmit,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.customer-maintenance {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>

