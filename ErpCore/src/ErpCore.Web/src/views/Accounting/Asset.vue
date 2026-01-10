<template>
  <div class="asset">
    <div class="page-header">
      <h1>資產管理 (SYSN310-SYSN311)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="資產編號">
          <el-input v-model="queryForm.AssetId" placeholder="請輸入資產編號" clearable />
        </el-form-item>
        <el-form-item label="資產名稱">
          <el-input v-model="queryForm.AssetName" placeholder="請輸入資產名稱" clearable />
        </el-form-item>
        <el-form-item label="資產類型">
          <el-input v-model="queryForm.AssetType" placeholder="請輸入資產類型" clearable />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
            <el-option label="報廢" value="D" />
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
        <el-table-column prop="AssetId" label="資產編號" width="150" />
        <el-table-column prop="AssetName" label="資產名稱" width="200" />
        <el-table-column prop="AssetType" label="資產類型" width="120" />
        <el-table-column prop="AcquisitionDate" label="取得日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.AcquisitionDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="AcquisitionCost" label="取得成本" width="120" align="right">
          <template #default="{ row }">
            {{ row.AcquisitionCost ? formatCurrency(row.AcquisitionCost) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="DepreciationMethod" label="折舊方法" width="120" />
        <el-table-column prop="UsefulLife" label="使用年限" width="100" align="center" />
        <el-table-column prop="ResidualValue" label="殘值" width="120" align="right">
          <template #default="{ row }">
            {{ row.ResidualValue ? formatCurrency(row.ResidualValue) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="Location" label="位置" width="150" />
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
      width="900px"
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
            <el-form-item label="資產編號" prop="AssetId">
              <el-input v-model="formData.AssetId" :disabled="isEdit" placeholder="請輸入資產編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="資產名稱" prop="AssetName">
              <el-input v-model="formData.AssetName" placeholder="請輸入資產名稱" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="資產類型" prop="AssetType">
              <el-input v-model="formData.AssetType" placeholder="請輸入資產類型" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="取得日期" prop="AcquisitionDate">
              <el-date-picker
                v-model="formData.AcquisitionDate"
                type="date"
                placeholder="請選擇取得日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="取得成本" prop="AcquisitionCost">
              <el-input-number v-model="formData.AcquisitionCost" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="折舊方法" prop="DepreciationMethod">
              <el-select v-model="formData.DepreciationMethod" placeholder="請選擇折舊方法" style="width: 100%">
                <el-option label="直線法" value="STRAIGHT" />
                <el-option label="年數合計法" value="SUM_OF_YEARS" />
                <el-option label="倍數餘額遞減法" value="DOUBLE_DECLINING" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="使用年限" prop="UsefulLife">
              <el-input-number v-model="formData.UsefulLife" :min="1" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="殘值" prop="ResidualValue">
              <el-input-number v-model="formData.ResidualValue" :min="0" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="位置" prop="Location">
              <el-input v-model="formData.Location" placeholder="請輸入位置" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
                <el-option label="報廢" value="D" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Notes">
              <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
          </el-col>
        </el-row>
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
import { accountingApi } from '@/api/accounting'

export default {
  name: 'Asset',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      AssetId: '',
      AssetName: '',
      AssetType: '',
      Status: '',
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
      AssetId: '',
      AssetName: '',
      AssetType: '',
      AcquisitionDate: null,
      AcquisitionCost: null,
      DepreciationMethod: '',
      UsefulLife: null,
      ResidualValue: null,
      Location: '',
      Status: 'A',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      AssetId: [{ required: true, message: '請輸入資產編號', trigger: 'blur' }],
      AssetName: [{ required: true, message: '請輸入資產名稱', trigger: 'blur' }]
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return '-'
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // 格式化金額
    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount)
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const map = { A: 'success', I: 'warning', D: 'danger' }
      return map[status] || 'info'
    }

    // 取得狀態文字
    const getStatusText = (status) => {
      const map = { A: '啟用', I: '停用', D: '報廢' }
      return map[status] || status
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
        const response = await accountingApi.getAssets(params)
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
        AssetId: '',
        AssetName: '',
        AssetType: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        AssetId: '',
        AssetName: '',
        AssetType: '',
        AcquisitionDate: null,
        AcquisitionCost: null,
        DepreciationMethod: '',
        UsefulLife: null,
        ResidualValue: null,
        Location: '',
        Status: 'A',
        Notes: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await accountingApi.getAsset(row.AssetId)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, {
            AssetId: response.Data.AssetId,
            AssetName: response.Data.AssetName,
            AssetType: response.Data.AssetType || '',
            AcquisitionDate: response.Data.AcquisitionDate ? formatDate(response.Data.AcquisitionDate) : null,
            AcquisitionCost: response.Data.AcquisitionCost,
            DepreciationMethod: response.Data.DepreciationMethod || '',
            UsefulLife: response.Data.UsefulLife,
            ResidualValue: response.Data.ResidualValue,
            Location: response.Data.Location || '',
            Status: response.Data.Status,
            Notes: response.Data.Notes || ''
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
        await ElMessageBox.confirm('確定要刪除此資產嗎？', '確認', {
          type: 'warning'
        })
        await accountingApi.deleteAsset(row.AssetId)
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
            if (isEdit.value) {
              await accountingApi.updateAsset(formData.AssetId, formData)
              ElMessage.success('修改成功')
            } else {
              await accountingApi.createAsset(formData)
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
      return isEdit.value ? '修改資產' : '新增資產'
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
      formatCurrency,
      getStatusType,
      getStatusText,
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

.asset {
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

