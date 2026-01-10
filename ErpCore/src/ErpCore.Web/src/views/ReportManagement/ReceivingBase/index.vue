<template>
  <div class="receiving-base-management">
    <div class="page-header">
      <h1>?∂Ê¨æ?∫Á??üËÉΩ (SYSR110-SYSR120)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ÜÂ?‰ª??">
          <el-input v-model="queryForm.SiteId" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÖÁõÆ‰ª??">
          <el-input v-model="queryForm.AritemId" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??Æ‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÖÁõÆ?çÁ®±">
          <el-input v-model="queryForm.AritemName" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??ÆÂ?Á®? clearable />
        </el-form-item>
        <el-form-item label="?ÉË?ÁßëÁõÆ‰ª??">
          <el-input v-model="queryForm.StypeId" placeholder="Ë´ãËº∏?•Ê?Ë®àÁ??Æ‰ª£?? clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">?•Ë©¢</el-button>
          <el-button @click="handleReset">?çÁΩÆ</el-button>
          <el-button type="success" @click="handleCreate">?∞Â?</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Ë≥áÊ?Ë°®Ê†º -->
    <el-card class="table-card" shadow="never">
      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="SiteId" label="?ÜÂ?‰ª??" width="120" />
        <el-table-column prop="AritemId" label="?∂Ê¨æ?ÖÁõÆ‰ª??" width="150" />
        <el-table-column prop="AritemName" label="?∂Ê¨æ?ÖÁõÆ?çÁ®±" width="200" />
        <el-table-column prop="StypeId" label="?ÉË?ÁßëÁõÆ‰ª??" width="150" />
        <el-table-column prop="StypeName" label="?ÉË?ÁßëÁõÆ?çÁ®±" width="200" />
        <el-table-column prop="Notes" label="?ôË®ª" min-width="200" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="160">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="?ç‰?" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">?•Á?</el-button>
            <el-button type="warning" size="small" @click="handleEdit(row)">‰øÆÊîπ</el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">?™Èô§</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- ?ÜÈ? -->
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

    <!-- ?∞Â?/‰øÆÊîπÂ∞çË©±Ê°?-->
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
        label-width="120px"
      >
        <el-form-item label="?ÜÂ?‰ª??" prop="SiteId">
          <el-input v-model="formData.SiteId" :disabled="isEdit" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÖÁõÆ‰ª??" prop="AritemId">
          <el-input v-model="formData.AritemId" :disabled="isEdit" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??Æ‰ª£?? />
        </el-form-item>
        <el-form-item label="?∂Ê¨æ?ÖÁõÆ?çÁ®±" prop="AritemName">
          <el-input v-model="formData.AritemName" placeholder="Ë´ãËº∏?•Êî∂Ê¨æÈ??ÆÂ?Á®? />
        </el-form-item>
        <el-form-item label="?ÉË?ÁßëÁõÆ‰ª??" prop="StypeId">
          <el-input v-model="formData.StypeId" placeholder="Ë´ãËº∏?•Ê?Ë®àÁ??Æ‰ª£?? />
        </el-form-item>
        <el-form-item label="?ôË®ª">
          <el-input v-model="formData.Notes" type="textarea" :rows="3" placeholder="Ë´ãËº∏?•Â?Ë®? />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmitForm">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { receiptApi } from '@/api/receipt'

export default {
  name: 'ReceivingBase',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      SiteId: '',
      AritemId: '',
      AritemName: '',
      StypeId: ''
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      SiteId: '',
      AritemId: '',
      AritemName: '',
      StypeId: '',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      SiteId: [{ required: true, message: 'Ë´ãËº∏?•Â?Â∫ó‰ª£??, trigger: 'blur' }],
      AritemId: [{ required: true, message: 'Ë´ãËº∏?•Êî∂Ê¨æÈ??Æ‰ª£??, trigger: 'blur' }],
      AritemName: [{ required: true, message: 'Ë´ãËº∏?•Êî∂Ê¨æÈ??ÆÂ?Á®?, trigger: 'blur' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?∂Ê¨æ?ÖÁõÆ' : '?∞Â??∂Ê¨æ?ÖÁõÆ'
    })

    // ?ºÂ??ñÊó•?üÊ???
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }

        const response = await receiptApi.getArItems(params)
        if (response.data && response.data.Success) {
          tableData.value = Array.isArray(response.data.Data) ? response.data.Data : []
          pagination.TotalCount = tableData.value.length
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?∂Ê¨æ?ÖÁõÆ?óË°®Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      } finally {
        loading.value = false
      }
    }

    // ?•Ë©¢
    const handleSearch = () => {
      pagination.PageIndex = 1
      loadData()
    }

    // ?çÁΩÆ
    const handleReset = () => {
      Object.assign(queryForm, {
        SiteId: '',
        AritemId: '',
        AritemName: '',
        StypeId: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        SiteId: '',
        AritemId: '',
        AritemName: '',
        StypeId: '',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await receiptApi.getArItem(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            SiteId: data.SiteId || '',
            AritemId: data.AritemId || '',
            AritemName: data.AritemName || '',
            StypeId: data.StypeId || '',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?∂Ê¨æ?ÖÁõÆË≥áÊ?Â§±Ê?:', error)
        ElMessage.error('?•Ë©¢Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
      }
    }

    // ‰øÆÊîπ
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // ?™Èô§
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?∂Ê¨æ?ÖÁõÆ?éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await receiptApi.deleteArItem(row.TKey)
        ElMessage.success('?™Èô§?êÂ?')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('?™Èô§Â§±Ê?:', error)
          ElMessage.error('?™Èô§Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ê‰∫§Ë°®ÂñÆ
    const handleSubmitForm = async () => {
      if (!formRef.value) return
      try {
        await formRef.value.validate()

        if (isEdit.value) {
          await receiptApi.updateArItem(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await receiptApi.createArItem(formData)
          ElMessage.success('?∞Â??êÂ?')
        }
        dialogVisible.value = false
        loadData()
      } catch (error) {
        if (error !== false) {
          console.error('?ê‰∫§Â§±Ê?:', error)
          ElMessage.error((isEdit.value ? '‰øÆÊîπ' : '?∞Â?') + 'Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // ?ÜÈ?ËÆäÊõ¥
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // ?ùÂ???
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
      handleSubmitForm,
      handleSizeChange,
      handlePageChange,
      formatDateTime
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.receiving-base-management {
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
    
    .search-form {
      .el-form-item {
        margin-bottom: 0;
      }
    }
  }

  .table-card {
    .el-table {
      .el-tag {
        font-weight: 500;
      }
    }
  }
}
</style>

