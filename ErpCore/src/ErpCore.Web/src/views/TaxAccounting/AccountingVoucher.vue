<template>
  <div class="accounting-voucher">
    <div class="page-header">
      <h1>會計憑證管理 (SYST121-SYST12B)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="憑證編號">
          <el-input v-model="queryForm.VoucherNo" placeholder="請輸入憑證編號" clearable />
        </el-form-item>
        <el-form-item label="憑證日期">
          <el-date-picker
            v-model="queryForm.VoucherDate"
            type="date"
            placeholder="請選擇憑證日期"
            value-format="YYYY-MM-DD"
            clearable
          />
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
        <el-table-column prop="VoucherNo" label="憑證編號" width="150" />
        <el-table-column prop="VoucherDate" label="憑證日期" width="120" />
        <el-table-column prop="VoucherType" label="憑證類型" width="120" />
        <el-table-column prop="Description" label="摘要" width="200" />
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
        <el-form-item label="憑證編號" prop="VoucherNo">
          <el-input v-model="formData.VoucherNo" :disabled="isEdit" placeholder="請輸入憑證編號" />
        </el-form-item>
        <el-form-item label="憑證日期" prop="VoucherDate">
          <el-date-picker
            v-model="formData.VoucherDate"
            type="date"
            placeholder="請選擇憑證日期"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="憑證類型" prop="VoucherType">
          <el-input v-model="formData.VoucherType" placeholder="請輸入憑證類型" />
        </el-form-item>
        <el-form-item label="摘要" prop="Description">
          <el-input v-model="formData.Description" type="textarea" :rows="3" placeholder="請輸入摘要" />
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
import { taxAccountingApi } from '@/api/taxAccounting'

export default {
  name: 'AccountingVoucher',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      VoucherNo: '',
      VoucherDate: null,
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
      VoucherNo: '',
      VoucherDate: null,
      VoucherType: '',
      Description: ''
    })

    // 表單驗證規則
    const formRules = {
      VoucherNo: [{ required: true, message: '請輸入憑證編號', trigger: 'blur' }],
      VoucherDate: [{ required: true, message: '請選擇憑證日期', trigger: 'change' }]
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
        const response = await taxAccountingApi.getVouchers(params)
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
      queryForm.VoucherNo = ''
      queryForm.VoucherDate = null
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      Object.assign(formData, {
        VoucherNo: '',
        VoucherDate: null,
        VoucherType: '',
        Description: ''
      })
      dialogVisible.value = true
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await taxAccountingApi.getVoucher(row.VoucherNo)
        if (response.Data) {
          Object.assign(formData, response.Data)
          isEdit.value = true
          dialogVisible.value = true
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
        await ElMessageBox.confirm('確定要刪除此憑證嗎？', '確認刪除', {
          type: 'warning'
        })
        await taxAccountingApi.deleteVoucher(row.VoucherNo)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 提交
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await taxAccountingApi.updateVoucher(formData.VoucherNo, formData)
              ElMessage.success('修改成功')
            } else {
              await taxAccountingApi.createVoucher(formData)
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
      return isEdit.value ? '修改會計憑證' : '新增會計憑證'
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

.accounting-voucher {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $primary-color;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border: 1px solid $card-border;
    
    .search-form {
      margin: 0;
    }
  }

  .table-card {
    background-color: $card-bg;
    border: 1px solid $card-border;
  }
}
</style>
