<template>
  <div class="member-data">
    <div class="page-header">
      <h1>會員資料維護 (SYS3310-SYS3320)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="會員編號">
          <el-input v-model="queryForm.MemberId" placeholder="請輸入會員編號" clearable />
        </el-form-item>
        <el-form-item label="會員姓名">
          <el-input v-model="queryForm.MemberName" placeholder="請輸入會員姓名" clearable />
        </el-form-item>
        <el-form-item label="身分證字號">
          <el-input v-model="queryForm.PersonalId" placeholder="請輸入身分證字號" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇" clearable>
            <el-option label="全部" value="" />
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="暫停" value="S" />
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
        <el-table-column prop="MemberId" label="會員編號" width="150" />
        <el-table-column prop="MemberName" label="會員姓名" width="150" />
        <el-table-column prop="PersonalId" label="身分證字號" width="150" />
        <el-table-column prop="Phone" label="電話" width="150" />
        <el-table-column prop="Mobile" label="手機" width="150" />
        <el-table-column prop="Email" label="電子郵件" width="200" />
        <el-table-column prop="MemberLevel" label="會員等級" width="120" />
        <el-table-column prop="CardNo" label="卡號" width="150" />
        <el-table-column prop="JoinDate" label="加入日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.JoinDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : row.Status === 'S' ? 'warning' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : row.Status === 'S' ? '暫停' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="info" size="small" @click="handleViewPoints(row)">積分</el-button>
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
      v-model="dialogVisible"
      :title="dialogTitle"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="150px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="會員編號" prop="MemberId">
              <el-input v-model="formData.MemberId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="會員姓名" prop="MemberName">
              <el-input v-model="formData.MemberName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="身分證字號">
              <el-input v-model="formData.PersonalId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="會員等級">
              <el-input v-model="formData.MemberLevel" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="電話">
              <el-input v-model="formData.Phone" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="手機">
              <el-input v-model="formData.Mobile" />
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
            <el-form-item label="卡號">
              <el-input v-model="formData.CardNo" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="生日">
              <el-date-picker
                v-model="formData.Birthday"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="加入日期">
              <el-date-picker
                v-model="formData.JoinDate"
                type="date"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="地址">
          <el-input v-model="formData.Address" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="店別代碼">
              <el-input v-model="formData.ShopId" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-radio-group v-model="formData.Status">
                <el-radio label="A">啟用</el-radio>
                <el-radio label="I">停用</el-radio>
                <el-radio label="S">暫停</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 積分記錄對話框 -->
    <el-dialog
      v-model="pointsDialogVisible"
      title="會員積分記錄"
      width="800px"
    >
      <el-table
        :data="pointsData"
        v-loading="pointsLoading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="TransactionDate" label="交易日期" width="150">
          <template #default="{ row }">
            {{ formatDateTime(row.TransactionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="TransactionType" label="交易類型" width="120" />
        <el-table-column prop="Points" label="積分" width="100" />
        <el-table-column prop="Remarks" label="備註" />
      </el-table>
      <template #footer>
        <el-button @click="pointsDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { memberApi } from '@/api/storeMember'

// 查詢表單
const queryForm = reactive({
  MemberId: '',
  MemberName: '',
  PersonalId: '',
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
const dialogTitle = computed(() => isEdit.value ? '修改會員資料' : '新增會員資料')
const isEdit = ref(false)
const formRef = ref(null)
const formData = reactive({
  MemberId: '',
  MemberName: '',
  PersonalId: '',
  Phone: '',
  Mobile: '',
  Email: '',
  MemberLevel: '',
  CardNo: '',
  Birthday: null,
  JoinDate: null,
  Address: '',
  ShopId: '',
  Status: 'A',
  Memo: ''
})
const formRules = {
  MemberId: [{ required: true, message: '請輸入會員編號', trigger: 'blur' }],
  MemberName: [{ required: true, message: '請輸入會員姓名', trigger: 'blur' }],
  Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
}
const currentMemberId = ref(null)

// 積分記錄
const pointsDialogVisible = ref(false)
const pointsData = ref([])
const pointsLoading = ref(false)

// 查詢
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      PageIndex: pagination.PageIndex,
      PageSize: pagination.PageSize,
      MemberId: queryForm.MemberId || undefined,
      MemberName: queryForm.MemberName || undefined,
      PersonalId: queryForm.PersonalId || undefined,
      Status: queryForm.Status || undefined
    }
    const response = await memberApi.getMembers(params)
    if (response.data?.success) {
      tableData.value = response.data.data?.items || []
      pagination.TotalCount = response.data.data?.totalCount || 0
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    loading.value = false
  }
}

// 重置
const handleReset = () => {
  queryForm.MemberId = ''
  queryForm.MemberName = ''
  queryForm.PersonalId = ''
  queryForm.Status = ''
  pagination.PageIndex = 1
  handleSearch()
}

// 新增
const handleCreate = () => {
  isEdit.value = false
  currentMemberId.value = null
  Object.assign(formData, {
    MemberId: '',
    MemberName: '',
    PersonalId: '',
    Phone: '',
    Mobile: '',
    Email: '',
    MemberLevel: '',
    CardNo: '',
    Birthday: null,
    JoinDate: new Date().toISOString().split('T')[0],
    Address: '',
    ShopId: '',
    Status: 'A',
    Memo: ''
  })
  dialogVisible.value = true
}

// 查看
const handleView = async (row) => {
  await handleEdit(row)
}

// 修改
const handleEdit = async (row) => {
  try {
    const response = await memberApi.getMember(row.MemberId)
    if (response.data?.success) {
      isEdit.value = true
      currentMemberId.value = row.MemberId
      Object.assign(formData, response.data.data)
      dialogVisible.value = true
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 查看積分
const handleViewPoints = async (row) => {
  pointsDialogVisible.value = true
  pointsLoading.value = true
  try {
    const response = await memberApi.getMemberPoints(row.MemberId, {})
    if (response.data?.success) {
      pointsData.value = response.data.data?.items || []
    } else {
      ElMessage.error(response.data?.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  } finally {
    pointsLoading.value = false
  }
}

// 刪除
const handleDelete = async (row) => {
  try {
    await ElMessageBox.confirm('確定要刪除此會員資料嗎？', '確認', {
      type: 'warning'
    })
    const response = await memberApi.deleteMember(row.MemberId)
    if (response.data?.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    } else {
      ElMessage.error(response.data?.message || '刪除失敗')
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
          response = await memberApi.updateMember(currentMemberId.value, formData)
        } else {
          response = await memberApi.createMember(formData)
        }
        if (response.data?.success) {
          ElMessage.success(isEdit.value ? '修改成功' : '新增成功')
          dialogVisible.value = false
          handleSearch()
        } else {
          ElMessage.error(response.data?.message || '操作失敗')
        }
      } catch (error) {
        ElMessage.error('操作失敗：' + error.message)
      }
    }
  })
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

// 格式化日期
const formatDate = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
}

// 格式化日期時間
const formatDateTime = (date) => {
  if (!date) return ''
  const d = new Date(date)
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
}

// 初始化
onMounted(() => {
  handleSearch()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.member-data {
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
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    margin-bottom: 20px;
  }
}
</style>

