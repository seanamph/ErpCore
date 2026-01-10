<template>
  <div class="attendance">
    <div class="page-header">
      <h1>考勤管理</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="考勤單號">
          <el-input v-model="queryForm.AttendanceId" placeholder="請輸入考勤單號" clearable />
        </el-form-item>
        <el-form-item label="員工編號">
          <el-input v-model="queryForm.EmployeeId" placeholder="請輸入員工編號" clearable />
        </el-form-item>
        <el-form-item label="員工姓名">
          <el-input v-model="queryForm.EmployeeName" placeholder="請輸入員工姓名" clearable />
        </el-form-item>
        <el-form-item label="考勤日期">
          <el-date-picker
            v-model="queryForm.AttendanceDate"
            type="date"
            placeholder="請選擇考勤日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button type="info" @click="handleCheckIn">上班打卡</el-button>
          <el-button type="warning" @click="handleCheckOut">下班打卡</el-button>
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
        <el-table-column prop="AttendanceId" label="考勤單號" width="150" />
        <el-table-column prop="EmployeeId" label="員工編號" width="120" />
        <el-table-column prop="EmployeeName" label="員工姓名" width="150" />
        <el-table-column prop="AttendanceDate" label="考勤日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.AttendanceDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="CheckInTime" label="上班時間" width="150">
          <template #default="{ row }">
            {{ formatDateTime(row.CheckInTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="CheckOutTime" label="下班時間" width="150">
          <template #default="{ row }">
            {{ formatDateTime(row.CheckOutTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="WorkHours" label="工作時數" width="120">
          <template #default="{ row }">
            {{ row.WorkHours ? row.WorkHours.toFixed(2) : '-' }} 小時
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
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-form-item label="員工編號" prop="EmployeeId">
          <el-input v-model="formData.EmployeeId" :disabled="isEdit" placeholder="請輸入員工編號" />
        </el-form-item>
        <el-form-item label="員工姓名" prop="EmployeeName">
          <el-input v-model="formData.EmployeeName" placeholder="請輸入員工姓名" />
        </el-form-item>
        <el-form-item label="考勤日期" prop="AttendanceDate">
          <el-date-picker
            v-model="formData.AttendanceDate"
            type="date"
            placeholder="請選擇考勤日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="上班時間" prop="CheckInTime">
          <el-date-picker
            v-model="formData.CheckInTime"
            type="datetime"
            placeholder="請選擇上班時間"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="下班時間" prop="CheckOutTime">
          <el-date-picker
            v-model="formData.CheckOutTime"
            type="datetime"
            placeholder="請選擇下班時間"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input
            v-model="formData.Memo"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="info" @click="handleSupplement">補打卡</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { humanResourceApi } from '@/api/humanResource'

export default {
  name: 'Attendance',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      AttendanceId: '',
      EmployeeId: '',
      EmployeeName: '',
      AttendanceDate: '',
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
      EmployeeId: '',
      EmployeeName: '',
      AttendanceDate: new Date().toISOString().split('T')[0],
      CheckInTime: '',
      CheckOutTime: '',
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      EmployeeId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
      AttendanceDate: [{ required: true, message: '請選擇考勤日期', trigger: 'change' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化日期時間
    const formatDateTime = (dateTime) => {
      if (!dateTime) return '-'
      const d = new Date(dateTime)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        'N': 'info',
        'C': 'success',
        'L': 'warning'
      }
      return types[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const texts = {
        'N': '正常',
        'C': '完成',
        'L': '遲到'
      }
      return texts[status] || status
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
        const response = await humanResourceApi.getAttendances(params)
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
        AttendanceId: '',
        EmployeeId: '',
        EmployeeName: '',
        AttendanceDate: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        EmployeeId: '',
        EmployeeName: '',
        AttendanceDate: new Date().toISOString().split('T')[0],
        CheckInTime: '',
        CheckOutTime: '',
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await humanResourceApi.getAttendance(row.AttendanceId)
        if (response.Data) {
          isEdit.value = true
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
        await ElMessageBox.confirm('確定要刪除此考勤記錄嗎？', '確認', {
          type: 'warning'
        })
        await humanResourceApi.deleteAttendance(row.AttendanceId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 上班打卡
    const handleCheckIn = async () => {
      try {
        const { value: employeeId } = await ElMessageBox.prompt('請輸入員工編號', '上班打卡', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          inputPattern: /.+/,
          inputErrorMessage: '員工編號不能為空'
        })
        
        await humanResourceApi.checkIn({
          EmployeeId: employeeId,
          CheckInTime: new Date().toISOString()
        })
        ElMessage.success('打卡成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('打卡失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 下班打卡
    const handleCheckOut = async () => {
      try {
        const { value: employeeId } = await ElMessageBox.prompt('請輸入員工編號', '下班打卡', {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          inputPattern: /.+/,
          inputErrorMessage: '員工編號不能為空'
        })
        
        await humanResourceApi.checkOut({
          EmployeeId: employeeId,
          CheckOutTime: new Date().toISOString()
        })
        ElMessage.success('打卡成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('打卡失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 補打卡
    const handleSupplement = async () => {
      if (!formData.EmployeeId) {
        ElMessage.warning('請先輸入員工編號')
        return
      }
      
      try {
        const response = await humanResourceApi.supplementAttendance({
          EmployeeId: formData.EmployeeId,
          AttendanceDate: formData.AttendanceDate,
          CheckInTime: formData.CheckInTime,
          CheckOutTime: formData.CheckOutTime,
          Memo: formData.Memo
        })
        if (response.Data) {
          ElMessage.success('補打卡成功')
          dialogVisible.value = false
          loadData()
        }
      } catch (error) {
        ElMessage.error('補打卡失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await humanResourceApi.updateAttendance(formData.AttendanceId, formData)
              ElMessage.success('修改成功')
            } else {
              await humanResourceApi.createAttendance(formData)
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
      return isEdit.value ? '修改考勤記錄' : '新增考勤記錄'
    })

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
      formatDate,
      formatDateTime,
      getStatusType,
      getStatusText,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleCheckIn,
      handleCheckOut,
      handleSupplement,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.attendance {
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
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .search-form {
      .el-form-item {
        margin-bottom: 16px;
      }
    }
  }

  .table-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
  }
}
</style>

