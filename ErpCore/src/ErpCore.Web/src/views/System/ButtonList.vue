<template>
  <div class="button-list">
    <div class="page-header">
      <h1>系統功能按鈕資料維護 (SYS0440)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" :inline="true" class="search-form">
        <el-form-item label="作業代碼">
          <el-input v-model="queryForm.Filters.ProgramId" placeholder="請輸入作業代碼" clearable />
        </el-form-item>
        <el-form-item label="按鈕代碼">
          <el-input v-model="queryForm.Filters.ButtonId" placeholder="請輸入按鈕代碼" clearable />
        </el-form-item>
        <el-form-item label="按鈕名稱">
          <el-input v-model="queryForm.Filters.ButtonName" placeholder="請輸入按鈕名稱" clearable />
        </el-form-item>
        <el-form-item label="頁面代碼">
          <el-input v-model="queryForm.Filters.PageId" placeholder="請輸入頁面代碼" clearable />
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="ProgramName" label="作業" width="200" />
        <el-table-column prop="ButtonId" label="按鈕代碼" width="120" />
        <el-table-column prop="ButtonName" label="按鈕名稱" width="150" />
        <el-table-column prop="PageId" label="頁面代碼" width="100" />
        <el-table-column prop="ButtonMsg" label="按鈕訊息" width="200" show-overflow-tooltip />
        <el-table-column prop="ButtonUrl" label="網頁鏈結位址" width="200" show-overflow-tooltip />
        <el-table-column prop="MsgTypeName" label="訊息型態" width="120" />
        <el-table-column prop="Status" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
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
      :title="dialogTitle"
      v-model="dialogVisible"
      width="800px"
      @close="handleDialogClose"
    >
      <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
        <el-form-item label="作業代碼" prop="ProgramId">
          <el-input v-model="form.ProgramId" placeholder="請輸入作業代碼" />
        </el-form-item>
        <el-form-item label="按鈕代碼" prop="ButtonId">
          <el-input v-model="form.ButtonId" placeholder="請輸入按鈕代碼" />
        </el-form-item>
        <el-form-item label="按鈕名稱" prop="ButtonName">
          <el-input v-model="form.ButtonName" placeholder="請輸入按鈕名稱" />
        </el-form-item>
        <el-form-item label="頁面代碼" prop="PageId">
          <el-input v-model="form.PageId" placeholder="請輸入頁面代碼" />
        </el-form-item>
        <el-form-item label="按鈕訊息" prop="ButtonMsg">
          <el-input v-model="form.ButtonMsg" type="textarea" :rows="3" placeholder="請輸入按鈕訊息" />
        </el-form-item>
        <el-form-item label="按鈕屬性" prop="ButtonAttr">
          <el-input v-model="form.ButtonAttr" placeholder="請輸入按鈕屬性" />
        </el-form-item>
        <el-form-item label="網頁鏈結位址" prop="ButtonUrl">
          <el-input v-model="form.ButtonUrl" placeholder="請輸入網頁鏈結位址" />
        </el-form-item>
        <el-form-item label="訊息型態" prop="MsgType">
          <el-input v-model="form.MsgType" placeholder="請輸入訊息型態" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import request from '@/utils/request'

export default {
  name: 'ButtonList',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const dialogVisible = ref(false)
    const dialogTitle = ref('新增按鈕')
    const formRef = ref(null)
    const selectedRows = ref([])

    // 查詢表單
    const queryForm = reactive({
      PageIndex: 1,
      PageSize: 20,
      SortField: 'ProgramId',
      SortOrder: 'ASC',
      Filters: {
        ProgramId: '',
        ButtonId: '',
        ButtonName: '',
        PageId: ''
      }
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const form = reactive({
      ProgramId: '',
      ButtonId: '',
      ButtonName: '',
      PageId: '',
      ButtonMsg: '',
      ButtonAttr: '',
      ButtonUrl: '',
      MsgType: ''
    })

    // 表單驗證規則
    const rules = {
      ProgramId: [
        { required: true, message: '作業代碼為必填', trigger: 'blur' }
      ],
      ButtonId: [
        { required: true, message: '按鈕代碼為必填', trigger: 'blur' }
      ],
      ButtonName: [
        { required: true, message: '按鈕名稱為必填', trigger: 'blur' }
      ]
    }

    // 當前編輯的 TKey
    const currentTKey = ref(null)

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await request.get('/api/v1/buttons', { params })
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
      queryForm.PageIndex = 1
      loadData()
    }

    // 重置
    const handleReset = () => {
      Object.assign(queryForm.Filters, {
        ProgramId: '',
        ButtonId: '',
        ButtonName: '',
        PageId: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      dialogTitle.value = '新增按鈕'
      currentTKey.value = null
      Object.assign(form, {
        ProgramId: '',
        ButtonId: '',
        ButtonName: '',
        PageId: '',
        ButtonMsg: '',
        ButtonAttr: '',
        ButtonUrl: '',
        MsgType: ''
      })
      dialogVisible.value = true
    }

    // 修改
    const handleEdit = (row) => {
      dialogTitle.value = '修改按鈕'
      currentTKey.value = row.TKey
      Object.assign(form, {
        ProgramId: row.ProgramId,
        ButtonId: row.ButtonId,
        ButtonName: row.ButtonName,
        PageId: row.PageId || '',
        ButtonMsg: row.ButtonMsg || '',
        ButtonAttr: row.ButtonAttr || '',
        ButtonUrl: row.ButtonUrl || '',
        MsgType: row.MsgType || ''
      })
      dialogVisible.value = true
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此按鈕嗎？', '確認刪除', {
          type: 'warning'
        })
        await request.delete(`/api/v1/buttons/${row.TKey}`)
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
            if (currentTKey.value) {
              // 修改
              await request.put(`/api/v1/buttons/${currentTKey.value}`, form)
              ElMessage.success('修改成功')
            } else {
              // 新增
              await request.post('/api/v1/buttons', form)
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error((currentTKey.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 關閉對話框
    const handleDialogClose = () => {
      dialogVisible.value = false
      if (formRef.value) {
        formRef.value.resetFields()
      }
    }

    // 選擇變更
    const handleSelectionChange = (selection) => {
      selectedRows.value = selection
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      queryForm.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      queryForm.PageIndex = page
      loadData()
    }

    // 狀態類型
    const getStatusType = (status) => {
      return status === '1' ? 'success' : 'danger'
    }

    // 狀態文字
    const getStatusText = (status) => {
      return status === '1' ? '啟用' : '停用'
    }

    // 初始化
    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
      dialogVisible,
      dialogTitle,
      formRef,
      queryForm,
      pagination,
      form,
      rules,
      handleSearch,
      handleReset,
      handleCreate,
      handleEdit,
      handleDelete,
      handleSubmit,
      handleDialogClose,
      handleSelectionChange,
      handleSizeChange,
      handlePageChange,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.button-list {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;

    h1 {
      color: $primary-color;
      font-size: 24px;
      font-weight: bold;
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
