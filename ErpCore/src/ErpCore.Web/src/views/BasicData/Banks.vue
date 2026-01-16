<template>
  <div class="banks">
    <div class="page-header">
      <h1>銀行基本資料維護 (SYSBC20)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="銀行代號">
          <el-input v-model="queryForm.BankId" placeholder="請輸入銀行代號" clearable />
        </el-form-item>
        <el-form-item label="銀行名稱">
          <el-input v-model="queryForm.BankName" placeholder="請輸入銀行名稱" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="銀行種類">
          <el-select v-model="queryForm.BankKind" placeholder="請選擇銀行種類" clearable>
            <el-option label="銀行" value="1" />
            <el-option label="郵局" value="2" />
            <el-option label="信用合作社" value="3" />
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
        <el-table-column prop="BankId" label="銀行代號" width="120" />
        <el-table-column prop="BankName" label="銀行名稱" width="200" />
        <el-table-column prop="AcctLen" label="帳號最小長度" width="120" />
        <el-table-column prop="AcctLenMax" label="帳號最大長度" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="BankKind" label="銀行種類" width="120">
          <template #default="{ row }">
            {{ getBankKindText(row.BankKind) }}
          </template>
        </el-table-column>
        <el-table-column prop="SeqNo" label="排序序號" width="100" />
        <el-table-column prop="Notes" label="備註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="UpdatedAt" label="更新時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.UpdatedAt) }}
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
      width="700px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
        <el-form-item label="銀行代號" prop="BankId">
          <el-input v-model="form.BankId" placeholder="請輸入銀行代號" :disabled="isEdit" />
        </el-form-item>
        <el-form-item label="銀行名稱" prop="BankName">
          <el-input v-model="form.BankName" placeholder="請輸入銀行名稱" />
        </el-form-item>
        <el-form-item label="帳號最小長度" prop="AcctLen">
          <el-input-number v-model="form.AcctLen" :min="0" :max="20" placeholder="請輸入帳號最小長度" />
        </el-form-item>
        <el-form-item label="帳號最大長度" prop="AcctLenMax">
          <el-input-number v-model="form.AcctLenMax" :min="0" :max="20" placeholder="請輸入帳號最大長度" />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="form.Status" placeholder="請選擇狀態">
            <el-option label="啟用" value="1" />
            <el-option label="停用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="銀行種類" prop="BankKind">
          <el-select v-model="form.BankKind" placeholder="請選擇銀行種類">
            <el-option label="銀行" value="1" />
            <el-option label="郵局" value="2" />
            <el-option label="信用合作社" value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="排序序號" prop="SeqNo">
          <el-input-number v-model="form.SeqNo" :min="0" placeholder="請輸入排序序號" />
        </el-form-item>
        <el-form-item label="備註" prop="Notes">
          <el-input v-model="form.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
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
import { banksApi } from '@/api/banks'

// 查詢表單
const queryForm = reactive({
  BankId: '',
  BankName: '',
  Status: '',
  BankKind: ''
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
const dialogTitle = ref('新增銀行')
const isEdit = ref(false)
const formRef = ref(null)

// 表單資料
const form = reactive({
  BankId: '',
  BankName: '',
  AcctLen: null,
  AcctLenMax: null,
  Status: '1',
  BankKind: '',
  SeqNo: null,
  Notes: ''
})

// 保存原始銀行代號（用於修改）
const originalBankId = ref('')

// 表單驗證規則
const rules = {
  BankId: [{ required: true, message: '請輸入銀行代號', trigger: 'blur' }],
  BankName: [{ required: true, message: '請輸入銀行名稱', trigger: 'blur' }],
  AcctLen: [
    {
      validator: (rule, value, callback) => {
        if (form.AcctLenMax !== null && value !== null && value > form.AcctLenMax) {
          callback(new Error('帳號最小長度不能大於最大長度'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  AcctLenMax: [
    {
      validator: (rule, value, callback) => {
        if (form.AcctLen !== null && value !== null && form.AcctLen > value) {
          callback(new Error('帳號最大長度不能小於最小長度'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 格式化日期時間
const formatDateTime = (dateTime) => {
  if (!dateTime) return ''
  const date = new Date(dateTime)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

// 取得銀行種類文字
const getBankKindText = (bankKind) => {
  const map = {
    '1': '銀行',
    '2': '郵局',
    '3': '信用合作社'
  }
  return map[bankKind] || bankKind || ''
}

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      BankId: queryForm.BankId || undefined,
      BankName: queryForm.BankName || undefined,
      Status: queryForm.Status || undefined,
      BankKind: queryForm.BankKind || undefined
    }
    const response = await banksApi.getBanks(params)
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
  queryForm.BankId = ''
  queryForm.BankName = ''
  queryForm.Status = ''
  queryForm.BankKind = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  dialogTitle.value = '新增銀行'
  resetForm()
  dialogVisible.value = true
}

// 修改
const handleEdit = async (row) => {
  isEdit.value = true
  dialogTitle.value = '修改銀行'
  originalBankId.value = row.BankId
  try {
    const response = await banksApi.getBank(row.BankId)
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
    await ElMessageBox.confirm('確定要刪除此銀行嗎？', '提示', {
      type: 'warning'
    })
    const response = await banksApi.deleteBank(row.BankId)
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
          // 修改時使用原始銀行代號
          const updateData = {
            BankName: form.BankName,
            AcctLen: form.AcctLen,
            AcctLenMax: form.AcctLenMax,
            Status: form.Status,
            BankKind: form.BankKind,
            SeqNo: form.SeqNo,
            Notes: form.Notes
          }
          response = await banksApi.updateBank(originalBankId.value, updateData)
        } else {
          response = await banksApi.createBank(form)
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
  form.BankId = ''
  form.BankName = ''
  form.AcctLen = null
  form.AcctLenMax = null
  form.Status = '1'
  form.BankKind = ''
  form.SeqNo = null
  form.Notes = ''
  originalBankId.value = ''
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

.banks {
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
