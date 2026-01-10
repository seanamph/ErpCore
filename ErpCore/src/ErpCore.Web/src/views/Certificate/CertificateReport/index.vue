<template>
  <div class="certificate-report-management">
    <div class="page-header">
      <h1>?ëË??±Ë°®?•Ë©¢ (SYSK310-SYSK500)</h1>
    </div>

    <!-- ?•Ë©¢Ë°®ÂñÆ -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="?±Ë°®È°ûÂ?">
          <el-select v-model="queryForm.ReportType" placeholder="Ë´ãÈÅ∏?áÂ†±Ë°®È??? style="width: 200px">
            <el-option label="?éÁ¥∞?±Ë°®" value="detail" />
            <el-option label="Áµ±Ë??±Ë°®" value="statistics" />
            <el-option label="?ÜÊ??±Ë°®" value="analysis" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ëË?Á∑®Ë?">
          <el-input v-model="queryForm.VoucherId" placeholder="Ë´ãËº∏?•Ê?Ë≠âÁ∑®?? clearable />
        </el-form-item>
        <el-form-item label="?ëË?È°ûÂ?">
          <el-select v-model="queryForm.VoucherType" placeholder="Ë´ãÈÅ∏?áÊ?Ë≠âÈ??? clearable>
            <el-option label="?≥Á•®" value="V" />
            <el-option label="?∂Ê?" value="R" />
            <el-option label="?ºÁ•®" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ÜÂ?‰ª??">
          <el-input v-model="queryForm.ShopId" placeholder="Ë´ãËº∏?•Â?Â∫ó‰ª£?? clearable />
        </el-form-item>
        <el-form-item label="?Ä??>
          <el-select v-model="queryForm.Status" placeholder="Ë´ãÈÅ∏?áÁ??? clearable>
            <el-option label="?âÁ®ø" value="D" />
            <el-option label="Â∑≤ÈÄÅÂá∫" value="S" />
            <el-option label="Â∑≤ÂØ©?? value="A" />
            <el-option label="Â∑≤Â?Ê∂? value="X" />
            <el-option label="Â∑≤Á?Ê°? value="C" />
          </el-select>
        </el-form-item>
        <el-form-item label="?ëË??•Ê?">
          <el-date-picker
            v-model="queryForm.VoucherDateRange"
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
          <el-button type="success" @click="handleExport">?ØÂá∫</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- ?éÁ¥∞?±Ë°®Ë°®Ê†º -->
    <el-card class="table-card" shadow="never" v-if="queryForm.ReportType === 'detail'">
      <el-table
        :data="detailTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="VoucherId" label="?ëË?Á∑®Ë?" width="150" />
        <el-table-column prop="VoucherDate" label="?ëË??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.VoucherDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="VoucherType" label="?ëË?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getVoucherTypeType(row.VoucherType)">
              {{ getVoucherTypeText(row.VoucherType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="?ÜÂ?‰ª??" width="120" />
        <el-table-column prop="Status" label="?Ä?? width="120">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusText(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalDebitAmount" label="?üÊñπÁ∏ΩÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalDebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalCreditAmount" label="Ë≤∏ÊñπÁ∏ΩÈ?" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalCreditAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="Memo" label="?ôË®ª" min-width="200" show-overflow-tooltip />
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

    <!-- Áµ±Ë??±Ë°® -->
    <el-card class="table-card" shadow="never" v-if="queryForm.ReportType === 'statistics'">
      <el-table
        :data="statisticsTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="VoucherType" label="?ëË?È°ûÂ?" width="150">
          <template #default="{ row }">
            <el-tag :type="getVoucherTypeType(row.VoucherType)">
              {{ getVoucherTypeText(row.VoucherType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Count" label="Á≠ÜÊï∏" width="120" align="right" />
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalDebitAmount" label="?üÊñπÁ∏ΩÈ?" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalDebitAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalCreditAmount" label="Ë≤∏ÊñπÁ∏ΩÈ?" width="150" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalCreditAmount) }}
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- ?ÜÊ??±Ë°®Ë°®Ê†º -->
    <el-card class="table-card" shadow="never" v-if="queryForm.ReportType === 'analysis'">
      <el-table
        :data="analysisTableData"
        v-loading="loading"
        border
        stripe
        style="width: 100%"
      >
        <el-table-column prop="VoucherId" label="?ëË?Á∑®Ë?" width="150" />
        <el-table-column prop="VoucherDate" label="?ëË??•Ê?" width="120">
          <template #default="{ row }">
            {{ formatDate(row.VoucherDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="VoucherType" label="?ëË?È°ûÂ?" width="120">
          <template #default="{ row }">
            <el-tag :type="getVoucherTypeType(row.VoucherType)">
              {{ getVoucherTypeText(row.VoucherType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ShopId" label="?ÜÂ?‰ª??" width="120" />
        <el-table-column prop="TotalAmount" label="Á∏ΩÈ?È°? width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.TotalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="AnalysisData" label="?ÜÊ?Ë≥áÊ?" min-width="300" show-overflow-tooltip />
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
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { certificateApi } from '@/api/certificate'

export default {
  name: 'CertificateReport',
  setup() {
    const loading = ref(false)
    const detailTableData = ref([])
    const statisticsTableData = ref([])
    const analysisTableData = ref([])

    // ?•Ë©¢Ë°®ÂñÆ
    const queryForm = reactive({
      ReportType: 'detail',
      VoucherId: '',
      VoucherType: '',
      ShopId: '',
      Status: '',
      VoucherDateRange: null
    })

    // ?ÜÈ?Ë≥áË?
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // ?ºÂ??ñÊó•??
    const formatDate = (date) => {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    }

    // ?ºÂ??ñË≤®Âπ?
    const formatCurrency = (amount) => {
      if (amount == null) return '0.00'
      return Number(amount).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }

    // ?ñÂ??ëË?È°ûÂ?È°ûÂ?
    const getVoucherTypeType = (type) => {
      const typeMap = {
        'V': 'primary',
        'R': 'success',
        'I': 'warning'
      }
      return typeMap[type] || 'info'
    }

    // ?ñÂ??ëË?È°ûÂ??áÂ?
    const getVoucherTypeText = (type) => {
      const typeMap = {
        'V': '?≥Á•®',
        'R': '?∂Ê?',
        'I': '?ºÁ•®'
      }
      return typeMap[type] || type
    }

    // ?ñÂ??Ä?ãÈ???
    const getStatusType = (status) => {
      const statusMap = {
        'D': 'info',
        'S': 'warning',
        'A': 'success',
        'X': 'danger',
        'C': 'success'
      }
      return statusMap[status] || 'info'
    }

    // ?ñÂ??Ä?ãÊ?Â≠?
    const getStatusText = (status) => {
      const statusMap = {
        'D': '?âÁ®ø',
        'S': 'Â∑≤ÈÄÅÂá∫',
        'A': 'Â∑≤ÂØ©??,
        'X': 'Â∑≤Â?Ê∂?,
        'C': 'Â∑≤Á?Ê°?
      }
      return statusMap[status] || status
    }

    // ?•Ë©¢Ë≥áÊ?
    const loadData = async () => {
      loading.value = true
      try {
        const query = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          VoucherId: queryForm.VoucherId || undefined,
          VoucherType: queryForm.VoucherType || undefined,
          ShopId: queryForm.ShopId || undefined,
          Status: queryForm.Status || undefined,
          VoucherDateFrom: queryForm.VoucherDateRange ? queryForm.VoucherDateRange[0] : undefined,
          VoucherDateTo: queryForm.VoucherDateRange ? queryForm.VoucherDateRange[1] : undefined
        }

        if (queryForm.ReportType === 'detail') {
          const response = await certificateApi.getVoucherDetailReport(query)
          if (response.data && response.data.Success) {
            const result = response.data.Data
            detailTableData.value = result.Items || []
            pagination.TotalCount = result.TotalCount || 0
          } else {
            ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
          }
        } else if (queryForm.ReportType === 'statistics') {
          const response = await certificateApi.getVoucherStatisticsReport(query)
          if (response.data && response.data.Success) {
            statisticsTableData.value = response.data.Data?.Statistics || []
          } else {
            ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
          }
        } else if (queryForm.ReportType === 'analysis') {
          const response = await certificateApi.getVoucherAnalysisReport(query)
          if (response.data && response.data.Success) {
            const result = response.data.Data
            analysisTableData.value = result.Items || []
            pagination.TotalCount = result.TotalCount || 0
          } else {
            ElMessage.error(response.data?.Message || '?•Ë©¢Â§±Ê?')
          }
        }
      } catch (error) {
        console.error('?•Ë©¢?ëË??±Ë°®Â§±Ê?:', error)
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
        ReportType: 'detail',
        VoucherId: '',
        VoucherType: '',
        ShopId: '',
        Status: '',
        VoucherDateRange: null
      })
      handleSearch()
    }

    // ?ØÂá∫
    const handleExport = () => {
      ElMessage.info('?ØÂá∫?üËÉΩ?ãÁôº‰∏?)
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
      detailTableData,
      statisticsTableData,
      analysisTableData,
      queryForm,
      pagination,
      handleSearch,
      handleReset,
      handleExport,
      handleSizeChange,
      handlePageChange,
      formatDate,
      formatCurrency,
      getVoucherTypeType,
      getVoucherTypeText,
      getStatusType,
      getStatusText
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.certificate-report-management {
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

