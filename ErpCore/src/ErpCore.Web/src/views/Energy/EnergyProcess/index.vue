<template>
  <div class="energy-process-management">
    <div class="page-header">
      <h1>?ΩÊ??ïÁ?‰ΩúÊ•≠ (SYSO310)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?ïÁ??ÆË?">
          <el-input v-model="queryForm.ProcessId" placeholder="Ë´ãËº∏?•Ë??ÜÂñÆ?? clearable />
        </el-form-item>
        <el-form-item label="?ΩÊ?Á∑®Ë?">
          <el-input v-model="queryForm.EnergyId" placeholder="Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ïÁ?È°ûÂ?">
          <el-select v-model="queryForm.ProcessType" placeholder="Ë´ãÈÅ∏?áË??ÜÈ??? clearable>
            <el-option label="?®ÈÉ®" value="" />
            <el-option label="Ë®àÁ?" value="CALCULATE" />
            <el-option label="Ë™øÊï¥" value="ADJUST" />
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
        <el-form-item label="?ïÁ??•Ê?">
          <el-date-picker
            v-model="queryForm.ProcessDateRange"
            type="daterange"
            range-separator="??
            start-placeholder="?ãÂ??•Ê?"
            end-placeholder="ÁµêÊ??•Ê?"
            value-format="YYYY-MM-DD"
          />
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
        <el-table-column prop="ProcessId" label="?ïÁ??ÆË?" width="150" />
        <el-table-column prop="EnergyId" label="?ΩÊ?Á∑®Ë?" width="150" />
        <el-table-column prop="ProcessDate" label="?ïÁ??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ProcessDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="ProcessType" label="?ïÁ?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getProcessTypeTag(row.ProcessType)">
              {{ getProcessTypeText(row.ProcessType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Amount" label="?∏È?" width="120" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.Amount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Cost" label="?êÊú¨" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Cost) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="?Ä?? width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '?üÁî®' : '?úÁî®' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="?ôË®ª" min-width="200" show-overflow-tooltip />
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
        <el-form-item label="?ïÁ??ÆË?" prop="ProcessId">
          <el-input v-model="formData.ProcessId" :disabled="isEdit" placeholder="Ë´ãËº∏?•Ë??ÜÂñÆ?? />
        </el-form-item>
        <el-form-item label="?ΩÊ?Á∑®Ë?" prop="EnergyId">
          <el-input v-model="formData.EnergyId" placeholder="Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®?? />
        </el-form-item>
        <el-form-item label="?ïÁ??•Ê?" prop="ProcessDate">
          <el-date-picker
            v-model="formData.ProcessDate"
            type="date"
            placeholder="Ë´ãÈÅ∏?áË??ÜÊó•??
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?ïÁ?È°ûÂ?" prop="ProcessType">
          <el-select v-model="formData.ProcessType" placeholder="Ë´ãÈÅ∏?áË??ÜÈ??? style="width: 100%">
            <el-option label="Ë®àÁ?" value="CALCULATE" />
            <el-option label="Ë™øÊï¥" value="ADJUST" />
            <el-option label="?∂‰?" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="?∏È?">
          <el-input-number
            v-model="formData.Amount"
            :min="0"
            :precision="4"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="?êÊú¨">
          <el-input-number
            v-model="formData.Cost"
            :min="0"
            :precision="4"
            style="width: 100%"
          />
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
  name: 'EnergyProcess',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const currentTKey = ref(null)

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      ProcessId: '',
      EnergyId: '',
      ProcessType: '',
      Status: '',
      ProcessDateRange: null
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // Ë°®ÂñÆË≥áÊ?
    const formData = reactive({
      ProcessId: '',
      EnergyId: '',
      ProcessDate: new Date().toISOString().split('T')[0],
      ProcessType: '',
      Amount: null,
      Cost: null,
      Status: 'A',
      Notes: ''
    })

    // Ë°®ÂñÆÈ©óË?Ë¶èÂ?
    const formRules = {
      ProcessId: [{ required: true, message: 'Ë´ãËº∏?•Ë??ÜÂñÆ??, trigger: 'blur' }],
      EnergyId: [{ required: true, message: 'Ë´ãËº∏?•ËÉΩÊ∫êÁ∑®??, trigger: 'blur' }],
      ProcessDate: [{ required: true, message: 'Ë´ãÈÅ∏?áË??ÜÊó•??, trigger: 'change' }]
    }

    // Â∞çË©±Ê°ÜÊ?È°?
    const dialogTitle = computed(() => {
      return isEdit.value ? '‰øÆÊîπ?ΩÊ??ïÁ?Ë≥áÊ?' : '?∞Â??ΩÊ??ïÁ?Ë≥áÊ?'
    })

    // ?ºÂ??ñÊó•??
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // ?ºÂ??ñË≤®Âπ?
    const formatCurrency = (amount) => {
      if (!amount && amount !== 0) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // ?ºÂ??ñÊï∏Â≠?
    const formatNumber = (num) => {
      if (!num && num !== 0) return '0'
      return Number(num).toLocaleString('zh-TW', { minimumFractionDigits: 0, maximumFractionDigits: 4 })
    }

    // ?ñÂ??ïÁ?È°ûÂ?Ê®ôÁ±§
    const getProcessTypeTag = (type) => {
      const tags = {
        'CALCULATE': 'success',
        'ADJUST': 'warning',
        'OTHER': 'info'
      }
      return tags[type] || 'info'
    }

    // ?ñÂ??ïÁ?È°ûÂ??áÂ?
    const getProcessTypeText = (type) => {
      const texts = {
        'CALCULATE': 'Ë®àÁ?',
        'ADJUST': 'Ë™øÊï¥',
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

        // ?ïÁ??•Ê?ÁØÑÂ?
        if (queryForm.ProcessDateRange && queryForm.ProcessDateRange.length === 2) {
          params.ProcessDateFrom = queryForm.ProcessDateRange[0]
          params.ProcessDateTo = queryForm.ProcessDateRange[1]
        }
        delete params.ProcessDateRange

        const response = await energyApi.getEnergyProcesses(params)
        if (response.data && response.data.Success) {
          tableData.value = response.data.Data.Items || []
          pagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ΩÊ??ïÁ??óË°®Â§±Ê?:', error)
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
        ProcessId: '',
        EnergyId: '',
        ProcessType: '',
        Status: '',
        ProcessDateRange: null
      })
      handleSearch()
    }

    // ?∞Â?
    const handleCreate = () => {
      isEdit.value = false
      currentTKey.value = null
      dialogVisible.value = true
      Object.assign(formData, {
        ProcessId: '',
        EnergyId: '',
        ProcessDate: new Date().toISOString().split('T')[0],
        ProcessType: '',
        Amount: null,
        Cost: null,
        Status: 'A',
        Notes: ''
      })
    }

    // ?•Á?
    const handleView = async (row) => {
      try {
        const response = await energyApi.getEnergyProcess(row.TKey)
        if (response.data && response.data.Success) {
          isEdit.value = true
          currentTKey.value = row.TKey
          dialogVisible.value = true
          const data = response.data.Data
          Object.assign(formData, {
            ProcessId: data.ProcessId,
            EnergyId: data.EnergyId,
            ProcessDate: formatDate(data.ProcessDate),
            ProcessType: data.ProcessType || '',
            Amount: data.Amount,
            Cost: data.Cost,
            Status: data.Status || 'A',
            Notes: data.Notes || ''
          })
        } else {
          ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
        }
      } catch (error) {
        console.error('?•Ë©¢?ΩÊ??ïÁ?Ë≥áÊ?Â§±Ê?:', error)
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
        await ElMessageBox.confirm('Á¢∫Â?Ë¶ÅÂà™?§Ê≠§?ΩÊ??ïÁ?Ë≥áÊ??éÔ?', 'Á¢∫Ë?', {
          type: 'warning'
        })
        await energyApi.deleteEnergyProcess(row.TKey)
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
          await energyApi.updateEnergyProcess(currentTKey.value, formData)
          ElMessage.success('‰øÆÊîπ?êÂ?')
        } else {
          await energyApi.createEnergyProcess(formData)
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
      formatDate,
      formatCurrency,
      formatNumber,
      getProcessTypeTag,
      getProcessTypeText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.energy-process-management {
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

