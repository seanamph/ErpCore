<template>
  <div class="extension">
    <div class="page-header">
      <h1>?¥Â??üËÉΩÁ∂≠Ë≠∑ (SYS9000)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?¥Â??üËÉΩ‰ª?¢º">
          <el-input v-model="queryForm.ExtensionId" placeholder="Ë´ãËº∏?•Êì¥Â±ïÂ??Ω‰ª£Á¢? clearable />
        </el-form-item>
        <el-form-item label="?¥Â??üËÉΩ?çÁ®±">
          <el-input v-model="queryForm.ExtensionName" placeholder="Ë´ãËº∏?•Êì¥Â±ïÂ??ΩÂ?Á®? clearable />
        </el-form-item>
        <el-form-item label="?¥Â?È°ûÂ?">
          <el-select v-model="queryForm.ExtensionType" placeholder="Ë´ãÈÅ∏?áÊì¥Â±ïÈ??? clearable>
            <el-option label="Á≥ªÁµ±?¥Â?" value="SYS" />
            <el-option label="Ê•≠Â??¥Â?" value="BIZ" />
            <el-option label="?±Ë°®?¥Â?" value="RPT" />
            <el-option label="?∂‰?" value="OTH" />
          </el-select>
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?áÁ??? clearable>
            <el-option label="?üÁî®" value="1" />
            <el-option label="?úÁî®" value="0" />
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
        <el-table-column type="selection" width="55" />
        <el-table-column prop="ExtensionId" label="?¥Â??üËÉΩ‰ª?¢º" width="150" />
        <el-table-column prop="ExtensionName" label="?¥Â??üËÉΩ?çÁ®±" width="200" />
        <el-table-column prop="ExtensionType" label="?¥Â?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getExtensionTypeTag(row.ExtensionType)">
              {{ getExtensionTypeText(row.ExtensionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ExtensionValue" label="?¥Â??? width="200" show-overflow-tooltip />
        <el-table-column prop="SeqNo" label="?íÂ?Â∫èË?" width="100" align="center" />
        <el-table-column prop="Status" label="?Ä?? width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.Status === '1' ? 'success' : 'danger'">
              {{ row.Status === '1' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="Âª∫Á??ÇÈ?" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column prop="CreatedBy" label="Âª∫Á??? width="120" />
        <el-table-column label="?ç‰?" width="200" fixed="right">
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
      width="60%"
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
            <el-form-item label="?¥Â??üËÉΩ‰ª?¢º" prop="ExtensionId">
              <el-input v-model="formData.ExtensionId" :disabled="isEdit" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?¥Â??üËÉΩ?çÁ®±" prop="ExtensionName">
              <el-input v-model="formData.ExtensionName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?¥Â?È°ûÂ?" prop="ExtensionType">
              <el-select v-model="formData.ExtensionType" style="width: 100%">
                <el-option label="Á≥ªÁµ±?¥Â?" value="SYS" />
                <el-option label="Ê•≠Â??¥Â?" value="BIZ" />
                <el-option label="?±Ë°®?¥Â?" value="RPT" />
                <el-option label="?∂‰?" value="OTH" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="?íÂ?Â∫èË?" prop="SeqNo">
              <el-input-number v-model="formData.SeqNo" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?¥Â??? prop="ExtensionValue">
          <el-input v-model="formData.ExtensionValue" type="textarea" :rows="4" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="?Ä?? prop="Status">
              <el-radio-group v-model="formData.Status">
                <el-radio label="1">?üÁî®</el-radio>
                <el-radio label="0">?úÁî®</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="?ôË®ª">
          <el-input v-model="formData.Memo" type="textarea" :rows="3" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">?ñÊ?</el-button>
        <el-button type="primary" @click="handleSubmit">Á¢∫Â?</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { extensionApi } from '@/api/extension'

export default {
  name: 'Extension',
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
      ExtensionName: '',
      ExtensionType: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
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
      ExtensionName: '',
      ExtensionType: '',
      ExtensionValue: '',
      SeqNo: 0,
      Status: '1',
      Memo: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      ExtensionId: [{ required: true, message: 'Ë´ãËº∏?•Êì¥Â±ïÂ??Ω‰ª£Á¢?, trigger: 'blur' }],
      ExtensionName: [{ required: true, message: 'Ë´ãËº∏?•Êì¥Â±ïÂ??ΩÂ?Á®?, trigger: 'blur' }],
      ExtensionType: [{ required: true, message: 'Ë´ãÈÅ∏?áÊì¥Â±ïÈ???, trigger: 'change' }],
      Status: [{ required: true, message: 'Ë´ãÈÅ∏?áÁ???, trigger: 'change' }]
    }

    // ?ºÂ??ñÊó•?üÊ???
    const formatDateTime = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')} ${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}:${String(d.getSeconds()).padStart(2, '0')}`
    }

    // ?ñÂ??¥Â?È°ûÂ?Ê®ôÁ±§
    const getExtensionTypeTag = (type) => {
      const tags = {
        'SYS': 'primary',
        'BIZ': 'success',
        'RPT': 'warning',
        'OTH': 'info'
      }
      return tags[type] || 'info'
    }

    // ?ñÂ??¥Â?È°ûÂ??áÂ?
    const getExtensionTypeText = (type) => {
      const texts = {
        'SYS': 'Á≥ªÁµ±?¥Â?',
        'BIZ': 'Ê•≠Â??¥Â?',
        'RPT': '?±Ë°®?¥Â?',
        'OTH': '?∂‰?'
      }
      return texts[type] || type
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
        const response = await extensionApi.getExtensionFunctions(params)
        if (response.Data) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        }
      } catch (error) {
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
        ExtensionName: '',
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
        ExtensionName: '',
        ExtensionType: '',
        ExtensionValue: '',
        SeqNo: 0,
        Status: '1',
        Memo: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await extensionApi.getExtensionFunction(row.TKey)
        if (response.Data) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          Object.assign(formData, response.Data)
        }
      } catch (error) {
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
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?¥Â??üËÉΩ?éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await extensionApi.deleteExtensionFunction(row.TKey)
        ElMessage.success('?™Èô§?êÂ?')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('?™Èô§Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
        }
      }
    }

    // ?ê‰∫§Ë°®ÂñÆ
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            if (isEdit.value) {
              await extensionApi.updateExtensionFunction(currentTKey.value, {
                ExtensionName: formData.ExtensionName,
                ExtensionType: formData.ExtensionType,
                ExtensionValue: formData.ExtensionValue,
                SeqNo: formData.SeqNo,
                Status: formData.Status,
                Memo: formData.Memo
              })
              ElMessage.success('‰øÆÊîπ?êÂ?')
            } else {
              await extensionApi.createExtensionFunction({
                ExtensionId: formData.ExtensionId,
                ExtensionName: formData.ExtensionName,
                ExtensionType: formData.ExtensionType,
                ExtensionValue: formData.ExtensionValue,
                SeqNo: formData.SeqNo,
                Status: formData.Status,
                Memo: formData.Memo
              })
              ElMessage.success('?∞Â??êÂ?')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('?ç‰?Â§±Ê?: ' + (error.message || '?™Áü•?ØË™§'))
          }
        }
      })
    }

    // ?ÜÈ?Â§ßÂ?ËÆäÊõ¥
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // ?ÜÈ?ËÆäÊõ¥
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // Ë®àÁ?Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?¥Â??üËÉΩ' : '?∞Â??¥Â??üËÉΩ'
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
      formatDateTime,
      getExtensionTypeTag,
      getExtensionTypeText,
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

<style lang="scss" scoped>`n@import '@/assets/styles/variables.scss';
.extension {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      color: $primary-color;
      font-size: 24px;
      font-weight: bold;
    }
  }
}
</style>

