<template>
  <div class="sysa1018-report">
    <div class="page-header">
      <h1>工務維修件數統計表 (SYSA1018)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="組織單位">
          <el-select
            v-model="queryForm.OrgId"
            placeholder="請選擇組織單位"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="org in orgList"
              :key="org.OrgId"
              :label="org.OrgName"
              :value="org.OrgId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="年月">
          <el-date-picker
            v-model="yearMonth"
            type="month"
            placeholder="請選擇年月"
            format="YYYY-MM"
            value-format="YYYY-MM"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="篩選類型">
          <el-select
            v-model="queryForm.FilterType"
            placeholder="請選擇篩選類型"
            clearable
            style="width: 200px"
          >
            <el-option label="全部" value="" />
            <el-option label="待處理" value="pending" />
            <el-option label="處理中" value="processing" />
            <el-option label="已完成" value="completed" />
            <el-option label="已取消" value="cancelled" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">匯出</el-button>
          <el-button type="warning" @click="handlePrint">列印</el-button>
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
        <el-table-column prop="OrgName" label="組織單位" width="150" />
        <el-table-column prop="YearMonth" label="年月" width="100" />
        <el-table-column prop="MaintenanceType" label="維修類型" width="120" />
        <el-table-column prop="MaintenanceStatus" label="維修狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.MaintenanceStatus)">
              {{ row.MaintenanceStatus }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="ItemCount" label="維修件數" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.ItemCount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalCount" label="總件數" width="100" align="right">
          <template #default="{ row }">
            {{ formatNumber(row.TotalCount) }}
          </template>
        </el-table-column>
      </el-table>
      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px"
      />
    </el-card>
  </div>
</template>

<script>
import { analysisReportApi } from '@/api/analysisReport'
import { dropdownListApi } from '@/api/dropdownList'
import { ElMessage } from 'element-plus'

export default {
  name: 'SYSA1018Report',
  data() {
    return {
      loading: false,
      queryForm: {
        OrgId: '',
        YearMonth: '',
        FilterType: '',
        PageIndex: 1,
        PageSize: 20
      },
      yearMonth: '',
      tableData: [],
      pagination: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      },
      orgList: []
    }
  },
  watch: {
    yearMonth(newVal) {
      this.queryForm.YearMonth = newVal || ''
    }
  },
  mounted() {
    this.loadOrgList()
    // 預設為本月
    const now = new Date()
    this.yearMonth = this.formatMonthForInput(now)
    this.queryForm.YearMonth = this.yearMonth
  },
  methods: {
    async loadOrgList() {
      try {
        const response = await dropdownListApi.getOrgOptions({})
        if (response.data && response.data.success && response.data.data) {
          this.orgList = response.data.data.map(item => ({
            OrgId: item.value || item.OrgId || item.orgId,
            OrgName: item.label || item.OrgName || item.orgName
          }))
        }
      } catch (error) {
        console.error('載入組織列表失敗:', error)
        ElMessage.warning('載入組織列表失敗')
      }
    },
    async handleSearch() {
      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1018Report(this.queryForm)
        if (response.data.Success) {
          this.tableData = response.data.Data.Items || []
          this.pagination = {
            PageIndex: response.data.Data.PageIndex || 1,
            PageSize: response.data.Data.PageSize || 20,
            TotalCount: response.data.Data.TotalCount || 0,
            TotalPages: response.data.Data.TotalPages || 0
          }
        } else {
          ElMessage.error(response.data.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢工務維修件數統計報表失敗:', error)
        ElMessage.error('查詢工務維修件數統計報表失敗')
      } finally {
        this.loading = false
      }
    },
    handleReset() {
      this.queryForm = {
        OrgId: '',
        YearMonth: '',
        FilterType: '',
        PageIndex: 1,
        PageSize: 20
      }
      const now = new Date()
      this.yearMonth = this.formatMonthForInput(now)
      this.queryForm.YearMonth = this.yearMonth
      this.tableData = []
      this.pagination = {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      }
    },
    async handleExport() {
      try {
        const response = await analysisReportApi.exportSYSA1018Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSA1018_工務維修件數統計表_${this.queryForm.YearMonth || '全部'}_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出工務維修件數統計報表失敗:', error)
        ElMessage.error('匯出工務維修件數統計報表失敗')
      }
    },
    async handlePrint() {
      try {
        const response = await analysisReportApi.printSYSA1018Report(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印工務維修件數統計報表失敗:', error)
        ElMessage.error('列印工務維修件數統計報表失敗')
      }
    },
    handleSizeChange(val) {
      this.queryForm.PageSize = val
      this.queryForm.PageIndex = 1
      this.handleSearch()
    },
    handlePageChange(val) {
      this.queryForm.PageIndex = val
      this.handleSearch()
    },
    formatNumber(value) {
      if (value == null) return '0'
      return Number(value).toLocaleString('zh-TW')
    },
    formatMonthForInput(date) {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}`
    },
    getStatusType(status) {
      const statusMap = {
        '待處理': 'warning',
        '處理中': 'info',
        '已完成': 'success',
        '已取消': 'danger'
      }
      return statusMap[status] || ''
    }
  }
}
</script>

<style scoped>
.sysa1018-report {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h1 {
  margin: 0;
  font-size: 24px;
  font-weight: 500;
}

.search-card {
  margin-bottom: 20px;
}

.search-form {
  margin-top: 10px;
}

.table-card {
  margin-top: 20px;
}
</style>
