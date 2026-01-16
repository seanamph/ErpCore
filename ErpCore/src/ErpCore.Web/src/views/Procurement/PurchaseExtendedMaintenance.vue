<template>
  <div class="purchase-extended-maintenance">
    <div class="page-header">
      <h1>採購擴展維護 (SYSPA10-SYSPB60)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="維護代碼">
          <el-input v-model="queryForm.MaintenanceId" placeholder="請輸入維護代碼" clearable />
        </el-form-item>
        <el-form-item label="維護名稱">
          <el-input v-model="queryForm.MaintenanceName" placeholder="請輸入維護名稱" clearable />
        </el-form-item>
        <el-form-item label="維護類型">
          <el-select v-model="queryForm.MaintenanceType" placeholder="請選擇維護類型" clearable>
            <el-option label="工具" value="TOOL" />
            <el-option label="輔助" value="AUX" />
            <el-option label="處理" value="PROCESS" />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
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
        <el-table-column prop="MaintenanceId" label="維護代碼" width="150" />
        <el-table-column prop="MaintenanceName" label="維護名稱" width="200" />
        <el-table-column prop="MaintenanceType" label="維護類型" width="120">
          <template #default="{ row }">
            {{ getMaintenanceTypeName(row.MaintenanceType) }}
          </template>
        </el-table-column>
        <el-table-column prop="MaintenanceDesc" label="維護說明" min-width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="SeqNo" label="排序序號" width="100" align="right" />
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
            <el-form-item label="維護代碼" prop="MaintenanceId">
              <el-input v-model="formData.MaintenanceId" :disabled="isEdit" placeholder="請輸入維護代碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="維護名稱" prop="MaintenanceName">
              <el-input v-model="formData.MaintenanceName" placeholder="請輸入維護名稱" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="維護類型" prop="MaintenanceType">
              <el-select v-model="formData.MaintenanceType" placeholder="請選擇維護類型" style="width: 100%">
                <el-option label="工具" value="TOOL" />
                <el-option label="輔助" value="AUX" />
                <el-option label="處理" value="PROCESS" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="排序序號" prop="SeqNo">
              <el-input-number v-model="formData.SeqNo" :min="0" :max="9999" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="維護說明" prop="MaintenanceDesc">
              <el-input v-model="formData.MaintenanceDesc" type="textarea" :rows="3" placeholder="請輸入維護說明" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="維護配置" prop="MaintenanceConfig">
              <el-input v-model="formData.MaintenanceConfig" type="textarea" :rows="5" placeholder="請輸入維護配置（JSON格式）" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="參數配置" prop="ParameterConfig">
              <el-input v-model="formData.ParameterConfig" type="textarea" :rows="5" placeholder="請輸入參數配置（JSON格式）" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="formData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="備註" prop="Memo">
              <el-input v-model="formData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
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
import { procurementApi } from '@/api/procurement'

export default {
  name: 'PurchaseExtendedMaintenance',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // 查詢表單
    const queryForm = reactive({
      MaintenanceId: '',
      MaintenanceName: '',
      MaintenanceType: '',
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
      MaintenanceId: '',
      MaintenanceName: '',
      MaintenanceType: '',
      MaintenanceDesc: '',
      MaintenanceConfig: '',
      ParameterConfig: '',
      Status: 'A',
      SeqNo: 0,
      Memo: ''
    })

    // 表單驗證規則
    const formRules = {
      MaintenanceId: [{ required: true, message: '請輸入維護代碼', trigger: 'blur' }],
      MaintenanceName: [{ required: true, message: '請輸入維護名稱', trigger: 'blur' }],
      MaintenanceType: [{ required: true, message: '請選擇維護類型', trigger: 'change' }],
      Status: [{ required: true, message: '請選擇狀態', trigger: 'change' }]
    }

    // 對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改採購擴展維護' : '新增採購擴展維護'
    })

    // 取得維護類型名稱
    const getMaintenanceTypeName = (type) => {
      const types = {
        'TOOL': '工具',
        'AUX': '輔助',
        'PROCESS': '處理'
      }
      return types[type] || type
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
        // 移除空值
        Object.keys(params).forEach(key => {
          if (params[key] === '' || params[key] === null) {
            delete params[key]
          }
        })
        const response = await procurementApi.getPurchaseExtendedMaintenances(params)
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
        MaintenanceId: '',
        MaintenanceName: '',
        MaintenanceType: '',
        Status: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        MaintenanceId: '',
        MaintenanceName: '',
        MaintenanceType: '',
        MaintenanceDesc: '',
        MaintenanceConfig: '',
        ParameterConfig: '',
        Status: 'A',
        SeqNo: 0,
        Memo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await procurementApi.getPurchaseExtendedMaintenance(row.TKey)
        if (response.Data) {
          Object.assign(formData, response.Data)
          currentTKey.value = row.TKey
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
        await ElMessageBox.confirm(
          `確定要刪除維護「${row.MaintenanceName}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await procurementApi.deletePurchaseExtendedMaintenance(row.TKey)
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
      try {
        await formRef.value.validate()
        
        // 驗證 JSON 格式（如果填寫了維護配置）
        if (formData.MaintenanceConfig) {
          try {
            JSON.parse(formData.MaintenanceConfig)
          } catch (e) {
            ElMessage.error('維護配置必須是有效的 JSON 格式')
            return
          }
        }

        // 驗證 JSON 格式（如果填寫了參數配置）
        if (formData.ParameterConfig) {
          try {
            JSON.parse(formData.ParameterConfig)
          } catch (e) {
            ElMessage.error('參數配置必須是有效的 JSON 格式')
            return
          }
        }

        if (isEdit.value) {
          const { MaintenanceId, TKey, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, ...updateData } = formData
          await procurementApi.updatePurchaseExtendedMaintenance(currentTKey.value, updateData)
          ElMessage.success('修改成功')
        } else {
          await procurementApi.createPurchaseExtendedMaintenance(formData)
          ElMessage.success('新增成功')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          ElMessage.error((isEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 初始化
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
      getMaintenanceTypeName,
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

.purchase-extended-maintenance {
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
