<template>
  <div class="interview-list">
    <div class="page-header">
      <h1>訪談維護作業 (SYSC222)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="潛客代碼">
          <el-input v-model="queryForm.ProspectId" placeholder="請輸入潛客代碼" clearable />
        </el-form-item>
        <el-form-item label="訪談日期起">
          <el-date-picker
            v-model="queryForm.InterviewDateFrom"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="訪談日期迄">
          <el-date-picker
            v-model="queryForm.InterviewDateTo"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="訪談結果">
          <el-select v-model="queryForm.InterviewResult" placeholder="請選擇訪談結果" clearable>
            <el-option label="成功" value="SUCCESS" />
            <el-option label="待追蹤" value="FOLLOW_UP" />
            <el-option label="取消" value="CANCELLED" />
            <el-option label="未到" value="NO_SHOW" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="有效" value="ACTIVE" />
            <el-option label="取消" value="CANCELLED" />
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
        <el-table-column prop="InterviewId" label="訪談ID" width="100" />
        <el-table-column prop="ProspectId" label="潛客代碼" width="150" />
        <el-table-column prop="ProspectName" label="潛客名稱" width="200" />
        <el-table-column prop="InterviewDate" label="訪談日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.InterviewDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="InterviewTime" label="訪談時間" width="120">
          <template #default="{ row }">
            {{ formatTime(row.InterviewTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="InterviewType" label="訪談類型" width="120">
          <template #default="{ row }">
            {{ formatInterviewType(row.InterviewType) }}
          </template>
        </el-table-column>
        <el-table-column prop="Interviewer" label="訪談人員" width="120" />
        <el-table-column prop="InterviewLocation" label="訪談地點" width="150" />
        <el-table-column prop="InterviewResult" label="訪談結果" width="120">
          <template #default="{ row }">
            <el-tag :type="getResultType(row.InterviewResult)">
              {{ formatInterviewResult(row.InterviewResult) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ formatStatus(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="NextActionDate" label="後續行動日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.NextActionDate) }}
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
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="潛客代碼" prop="ProspectId">
              <el-input v-model="formData.ProspectId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="訪談日期" prop="InterviewDate">
              <el-date-picker
                v-model="formData.InterviewDate"
                type="date"
                placeholder="請選擇訪談日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="訪談時間">
              <el-time-picker
                v-model="formData.InterviewTime"
                placeholder="請選擇訪談時間"
                format="HH:mm:ss"
                value-format="HH:mm:ss"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="訪談類型">
              <el-select v-model="formData.InterviewType" placeholder="請選擇訪談類型" clearable>
                <el-option label="電話" value="PHONE" />
                <el-option label="面對面" value="FACE_TO_FACE" />
                <el-option label="線上" value="ONLINE" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="訪談人員">
              <el-input v-model="formData.Interviewer" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="訪談地點">
              <el-input v-model="formData.InterviewLocation" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="訪談結果">
              <el-select v-model="formData.InterviewResult" placeholder="請選擇訪談結果" clearable>
                <el-option label="成功" value="SUCCESS" />
                <el-option label="待追蹤" value="FOLLOW_UP" />
                <el-option label="取消" value="CANCELLED" />
                <el-option label="未到" value="NO_SHOW" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態">
                <el-option label="有效" value="ACTIVE" />
                <el-option label="取消" value="CANCELLED" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="訪談內容">
          <el-input v-model="formData.InterviewContent" type="textarea" :rows="4" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="後續行動">
              <el-input v-model="formData.NextAction" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="後續行動日期">
              <el-date-picker
                v-model="formData.NextActionDate"
                type="date"
                placeholder="請選擇後續行動日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="追蹤日期">
          <el-date-picker
            v-model="formData.FollowUpDate"
            type="date"
            placeholder="請選擇追蹤日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="備註">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" />
        </el-form-item>
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
import { useRoute } from 'vue-router'
import { interviewApi } from '@/api/recruitment'

export default {
  name: 'InterviewList',
  setup() {
    const route = useRoute()
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      ProspectId: '',
      InterviewDateFrom: '',
      InterviewDateTo: '',
      InterviewResult: '',
      Status: '',
      Interviewer: '',
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
      ProspectId: '',
      InterviewDate: '',
      InterviewTime: '',
      InterviewType: '',
      Interviewer: '',
      InterviewLocation: '',
      InterviewContent: '',
      InterviewResult: '',
      NextAction: '',
      NextActionDate: null,
      FollowUpDate: null,
      Notes: '',
      Status: 'ACTIVE'
    })

    // 表單驗證規則
    const formRules = {
      ProspectId: [{ required: true, message: '請輸入潛客代碼', trigger: 'blur' }],
      InterviewDate: [{ required: true, message: '請選擇訪談日期', trigger: 'change' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 格式化訪談類型
    const formatInterviewType = (type) => {
      const types = {
        PHONE: '電話',
        FACE_TO_FACE: '面對面',
        ONLINE: '線上'
      }
      return types[type] || type
    }

    // 格式化訪談結果
    const formatInterviewResult = (result) => {
      const results = {
        SUCCESS: '成功',
        FOLLOW_UP: '待追蹤',
        CANCELLED: '取消',
        NO_SHOW: '未到'
      }
      return results[result] || result
    }

    // 取得訪談結果類型
    const getResultType = (result) => {
      const types = {
        SUCCESS: 'success',
        FOLLOW_UP: 'warning',
        CANCELLED: 'danger',
        NO_SHOW: 'info'
      }
      return types[result] || ''
    }

    // 格式化狀態
    const formatStatus = (status) => {
      const statuses = {
        ACTIVE: '有效',
        CANCELLED: '取消'
      }
      return statuses[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        ACTIVE: 'success',
        CANCELLED: 'danger'
      }
      return types[status] || ''
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      return new Date(date).toLocaleDateString('zh-TW')
    }

    // 格式化時間
    const formatTime = (time) => {
      if (!time) return ''
      return time
    }

    // 格式化日期時間
    const formatDateTime = (dateTime) => {
      if (!dateTime) return ''
      return new Date(dateTime).toLocaleString('zh-TW')
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        // 如果 URL 中有 prospectId 參數，使用該參數查詢
        const prospectId = route.query.prospectId
        if (prospectId) {
          queryForm.ProspectId = prospectId
        }

        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }

        let response
        if (prospectId) {
          response = await interviewApi.getInterviewsByProspect(prospectId, params)
        } else {
          response = await interviewApi.getInterviews(params)
        }

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
        ProspectId: route.query.prospectId || '',
        InterviewDateFrom: '',
        InterviewDateTo: '',
        InterviewResult: '',
        Status: '',
        Interviewer: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      const prospectId = route.query.prospectId || ''
      Object.assign(formData, {
        ProspectId: prospectId,
        InterviewDate: '',
        InterviewTime: '',
        InterviewType: '',
        Interviewer: '',
        InterviewLocation: '',
        InterviewContent: '',
        InterviewResult: '',
        NextAction: '',
        NextActionDate: null,
        FollowUpDate: null,
        Notes: '',
        Status: 'ACTIVE'
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await interviewApi.getInterview(row.InterviewId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          const data = response.Data
          Object.assign(formData, {
            ProspectId: data.ProspectId,
            InterviewDate: data.InterviewDate ? new Date(data.InterviewDate).toISOString().split('T')[0] : '',
            InterviewTime: data.InterviewTime || '',
            InterviewType: data.InterviewType || '',
            Interviewer: data.Interviewer || '',
            InterviewLocation: data.InterviewLocation || '',
            InterviewContent: data.InterviewContent || '',
            InterviewResult: data.InterviewResult || '',
            NextAction: data.NextAction || '',
            NextActionDate: data.NextActionDate ? new Date(data.NextActionDate).toISOString().split('T')[0] : null,
            FollowUpDate: data.FollowUpDate ? new Date(data.FollowUpDate).toISOString().split('T')[0] : null,
            Notes: data.Notes || '',
            Status: data.Status || 'ACTIVE'
          })
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
        await ElMessageBox.confirm('確定要刪除此訪談記錄嗎？', '確認', {
          type: 'warning'
        })
        await interviewApi.deleteInterview(row.InterviewId)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            const submitData = {
              ...formData,
              InterviewDate: formData.InterviewDate ? new Date(formData.InterviewDate) : new Date(),
              InterviewTime: formData.InterviewTime || null,
              NextActionDate: formData.NextActionDate ? new Date(formData.NextActionDate) : null,
              FollowUpDate: formData.FollowUpDate ? new Date(formData.FollowUpDate) : null
            }

            if (isEdit.value) {
              await interviewApi.updateInterview(formData.InterviewId || 0, submitData)
              ElMessage.success('修改成功')
            } else {
              await interviewApi.createInterview(submitData)
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
      return isEdit.value ? '修改訪談' : '新增訪談'
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
      formatInterviewType,
      formatInterviewResult,
      getResultType,
      formatStatus,
      getStatusType,
      formatDate,
      formatTime,
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.interview-list {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>

