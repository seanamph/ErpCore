<template>
  <div class="energy-extension-management">
    <div class="page-header">
      <h1>?ΩÊ??¥Â?Á∂≠Ë≠∑ (SYSOU10-SYSOU33)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?¥Â?Á∑®Ë?">
          <el-input v-model="queryForm.ExtensionId" placeholder="Ë´ãËº∏?•Êì¥Â±ïÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ΩÊ?Á∑®Ë?">
          <el-input v-model="queryForm.EnergyId" placeholder="Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?¥Â?È°ûÂ?">
          <el-select v-model="queryForm.ExtensionType" placeholder="Ë´ãÈÅ∏?áÊì¥Â±ïÈ??? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="È°ûÂ?1" value="TYPE1" />
            <el-option label="È°ûÂ?2" value="TYPE2" />
            <el-option label="?∂‰?" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?áÁ??? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="?üÁî®" value="A" />
            <el-option label="?úÁî®" value="I" />
          </el-select>
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
        <el-table-column prop="ExtensionId" label="?¥Â?Á∑®Ë?" width="150" />
        <el-table-column prop="EnergyId" label="?ΩÊ?Á∑®Ë?" width="150" />
        <el-table-column prop="ExtensionType" label="?¥Â?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getExtensionTypeTag(row.ExtensionType)">
              {{ getExtensionTypeText(row.ExtensionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ExtensionValue" label="?¥Â??? width="200" show-overflow-tooltip />
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
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
        <el-form-item label="?¥Â?Á∑®Ë?" prop="ExtensionId">
          <el-input v-model="formData.ExtensionId" :disabled="isEdit" placeholder="Ë´ãËº∏?•Êì¥Â±ïÁ∑®?? />
        </el-form-item>
        <el-form-item label="?ΩÊ?Á∑®Ë?" prop="EnergyId">
          <el-input v-model="formData.EnergyId" placeholder="Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®?? />
        </el-form-item>
        <el-form-item label="?¥Â?È°ûÂ?" prop="ExtensionType">
          <el-select v-model="formData.ExtensionType" placeholder="Ë´ãÈÅ∏?áÊì¥Â±ïÈ??? style="width: 100%">
            <el-option label="È°ûÂ?1" value="TYPE1" />
            <el-option label="È°ûÂ?2" value="TYPE2" />
            <el-option label="?∂‰?" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="?¥Â???>
          <el-input v-model="formData.ExtensionValue" placeholder="Ë´ãËº∏?•Êì¥Â±ïÂÄ? />
        </el-form-item>
        <el-form-item label="?Ä?? prop="Status">
          <el-select v-model="formData.Status" placeholder="Ë´ãÈÅ∏?áÁ??? style="width: 100%">
            <el-option label="?üÁî®" value="A" />
            <el-option label="?úÁî®" value="I" />
          </el-select>
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
import { energyApi } from '@/api/energy'

export default {
  name: 'EnergyExtension',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      ExtensionId: '',
      EnergyId: '',
      ExtensionType: '',
      Status: ''
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      ExtensionId: '',
      EnergyId: '',
      ExtensionType: '',
      ExtensionValue: '',
      Status: 'A',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      ExtensionId: [{ required: true, message: 'Ë´ãËº∏?•Êì¥Â±ïÁ∑®??, trigger: 'blur' }],
      EnergyId: [{ required: true, message: 'Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®??, trigger: 'blur' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?ΩÊ??¥Â?Ë≥áÊ?' : '?∞Â??ΩÊ??¥Â?Ë≥áÊ?'
    })

    // ?ºÂ??ñÊó•?üÊ???
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
    }

    // ?ñÂ??¥Â?È°ûÂ?Ê®ôÁ±§
    const getExtensionTypeTag = (type) => {
      const tags = {
        'TYPE1': 'success',
        'TYPE2': 'warning',
        'OTHER': 'info'
      }
      return tags[type] || 'info'
    }

    // ?ñÂ??¥Â?È°ûÂ??áÂ?
    const getExtensionTypeText = (type) => {
      const texts = {
        'TYPE1': 'È°ûÂ?1',
        'TYPE2': 'È°ûÂ?2',
        'OTHER': '?∂‰?'
      }
      return texts[type] || type || ''
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

        const response = await energyApi.getEnergyExtensions(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ΩÊ??¥Â??óË°®Â§±Ê?:', error)
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
        ExtensionId: '',
        EnergyId: '',
        ExtensionType: '',
        Status: ''
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        ExtensionId: '',
        EnergyId: '',
        ExtensionType: '',
        ExtensionValue: '',
        Status: 'A',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await energyApi.getEnergyExtension(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            ExtensionId: data.ExtensionId,
            EnergyId: data.EnergyId,
            ExtensionType: data.ExtensionType || '',
            ExtensionValue: data.ExtensionValue || '',
            Status: data.Status || 'A',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ΩÊ??¥Â?Ë≥áÊ?Â§±Ê?:', error)
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
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?ΩÊ??¥Â?Ë≥áÊ??éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await energyApi.deleteEnergyExtension(row.TKey)
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
          await energyApi.updateEnergyExtension(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await energyApi.createEnergyExtension(formData)
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
      formatDateTime,
      getExtensionTypeTag,
      getExtensionTypeText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.energy-extension-management {
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

