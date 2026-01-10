<template>
  <div class="tenant-location-list">
    <div class="page-header">
      <h1>?��??��??�能 - 租戶位置維護 (SYSC999)</h1>
    </div>

    <!-- ?�詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="租戶主�?主鍵">
          <el-input-number v-model="queryForm.AgmTKey" placeholder="請輸?��??�主檔主?? :min="1" clearable style="width: 200px" />
        </el-form-item>
        <el-form-item label="位置�?��">
          <el-input v-model="queryForm.LocationId" placeholder="請輸?��?置代�? clearable />
        </el-form-item>
        <el-form-item label="?�?�代�?>
          <el-input v-model="queryForm.AreaId" placeholder="請輸?��??�代�? clearable />
        </el-form-item>
        <el-form-item label="樓層�?��">
          <el-input v-model="queryForm.FloorId" placeholder="請輸?��?層代�? clearable />
        </el-form-item>
        <el-form-item label="?�??>
          <el-select v-model="queryForm.Status" placeholder="請選?��??? clearable>
            <el-option label="?�部" value="" />
            <el-option label="?�用" value="1" />
            <el-option label="?�用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?�詢</el-button>
          <el-button @click="handleReset">?�置</el-button>
          <el-button type="success" @click="handleCreate">?��?</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資�?表格 -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="TKey" label="主鍵" width="100" />
        <el-table-column prop="AgmTKey" label="租戶主�?主鍵" width="150" />
        <el-table-column prop="LocationId" label="位置�?��" width="150" />
        <el-table-column prop="AreaId" label="?�?�代�? width="150" />
        <el-table-column prop="FloorId" label="樓層�?��" width="150" />
        <el-table-column prop="Status" label="?�?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '?�用' : '?�用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="?�註" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建�??��?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?��?" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">?�除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ?��? -->
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

    <!-- ?��?/修改對話�?-->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-form-item label="租戶主�?主鍵" prop="AgmTKey" v-if="!isEdit">
          <el-input-number v-model="formData.AgmTKey" :min="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="位置�?��" prop="LocationId">
          <el-input v-model="formData.LocationId" placeholder="請輸?��?置代�? />
        </el-form-item>
        <el-form-item label="?�?�代�? prop="AreaId">
          <el-input v-model="formData.AreaId" placeholder="請輸?��??�代�? />
        </el-form-item>
        <el-form-item label="樓層�?��" prop="FloorId">
          <el-input v-model="formData.FloorId" placeholder="請輸?��?層代�? />
        </el-form-item>
        <el-form-item label="?�?? prop="Status">
          <el-select v-model="formData.Status" placeholder="請選?��??? style="width: 100%">
            <el-option label="?�用" value="1" />
            <el-option label="?�用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item label="?�註" prop="Notes">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="請輸?��?�? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?��?</el-button>
        <el-button type="primary" @click="handleSubmit">確�?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { tenantLocationApi } from '@/api/recruitment'

export default {
  name: 'TenantLocationList',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // ?�詢表單
    const queryForm = reactive({
      AgmTKey: null,
      LocationId: '',
      AreaId: '',
      FloorId: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })

    // ?��?資�?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資�?
    const formData = reactive({
      AgmTKey: null,
      LocationId: '',
      AreaId: '',
      FloorId: '',
      Status: '1',
      Notes: ''
    })

    // 表單驗�?規�?
    const formRules = {
      AgmTKey: [{ required: true, message: '請輸?��??�主檔主??, trigger: 'blur' }],
      LocationId: [{ required: true, message: '請輸?��?置代�?, trigger: 'blur' }],
      Status: [{ required: true, message: '請選?��???, trigger: 'change' }]
    }

    // 對話框�?�?
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改租戶位置' : '?��?租戶位置'
    })

    // ?��??�日?��???
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

    // ?�詢?�表
    const fetchData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          AgmTKey: queryForm.AgmTKey || undefined,
          LocationId: queryForm.LocationId || undefined,
          AreaId: queryForm.AreaId || undefined,
          FloorId: queryForm.FloorId || undefined,
          Status: queryForm.Status || undefined
        }

        const response = await tenantLocationApi.getTenantLocations(params)
        if (response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data.Message || '?�詢失�?')
        }
      } catch (error) {
        console.error('?�詢失�?:', error)
        ElMessage.error('?�詢失�?')
      } finally {
        loading.value = false
      }
    }

    // ?�詢
    const handleSearch = () => {
      pagination.PageIndex = 1
      fetchData()
    }

    // ?�置
    const handleReset = () => {
      queryForm.AgmTKey = null
      queryForm.LocationId = ''
      queryForm.AreaId = ''
      queryForm.FloorId = ''
      queryForm.Status = ''
      pagination.PageIndex = 1
      fetchData()
    }

    // ?��?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      Object.assign(formData, {
        AgmTKey: null,
        LocationId: '',
        AreaId: '',
        FloorId: '',
        Status: '1',
        Notes: ''
      })
      dialogVisible.value = true
    }

    // 修改
    const handleEdit = (row) => {
      isEdit.value = true
      currentTKey.value = row.TKey
      Object.assign(formData, {
        LocationId: row.LocationId || '',
        AreaId: row.AreaId || '',
        FloorId: row.FloorId || '',
        Status: row.Status || '1',
        Notes: row.Notes || ''
      })
      dialogVisible.value = true
    }

    // ?�除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確�?要刪?�此筆�??��?�?, '確�??�除', {
          type: 'warning'
        })

        const response = await tenantLocationApi.deleteTenantLocation(row.TKey)
        if (response.data.Success) {
          ElMessage.success('?�除?��?')
          fetchData()
        } else {
          ElMessage.error(response.data.Message || '?�除失�?')
        }
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?�除失�?:', error)
          ElMessage.error('?�除失�?')
        }
      }
    }

    // ?�交表單
    const handleSubmit = async () => {
      if (!formRef.value) return

      await formRef.value.validate(async (valid) => {
        if (!valid) return

        try {
          if (isEdit.value) {
            // 修改
            const response = await tenantLocationApi.updateTenantLocation(currentTKey.value, formData)
            if (response.data.Success) {
              ElMessage.success('修改?��?')
              dialogVisible.value = false
              fetchData()
            } else {
              ElMessage.error(response.data.Message || '修改失�?')
            }
          } else {
            // ?��?
            const response = await tenantLocationApi.createTenantLocation(formData)
            if (response.data.Success) {
              ElMessage.success('?��??��?')
              dialogVisible.value = false
              fetchData()
            } else {
              ElMessage.error(response.data.Message || '?��?失�?')
            }
          }
        } catch (error) {
          console.error('?��?失�?:', error)
          ElMessage.error('?��?失�?')
        }
      })
    }

    // ?��?大�?變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      fetchData()
    }

    // ?��?變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      fetchData()
    }

    // ?��???
    onMounted(() => {
      fetchData()
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
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
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
.tenant-location-list {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  color: $primary-color;
  font-size: 24px;
  font-weight: bold;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-top: 10px;
}

.table-card {
  margin-bottom: 20px;
}
</style>

