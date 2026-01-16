<template>
  <div class="sysa1024-report">
    <div class="page-header">
      <h1>工務維修統計報表(其他) (SYSA1024)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.SiteId"
            placeholder="請選擇店別"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="site in siteList"
              :key="site.SiteId"
              :label="site.SiteName"
              :value="site.SiteId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="費用負擔">
          <el-select
            v-model="queryForm.BelongStatus"
            placeholder="請選擇費用負擔"
            clearable
            style="width: 200px"
          >
            <el-option label="員工負擔" value="1" />
            <el-option label="店別負擔" value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="日統計表起" required>
          <el-date-picker
            v-model="queryForm.ApplyDateB"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="日統計表迄" required>
          <el-date-picker
            v-model="queryForm.ApplyDateE"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="費用歸屬單位">
          <el-select
            v-model="queryForm.BelongOrg"
            placeholder="請選擇費用歸屬單位"
            clearable
            filterable
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
        <el-form-item label="維保人員">
          <el-select
            v-model="queryForm.MaintainEmp"
            placeholder="請選擇維保人員"
            clearable
            filterable
            style="width: 200px"
          >
            <el-option
              v-for="emp in empList"
              :key="emp.EmpId"
              :label="emp.EmpName"
              :value="emp.EmpId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="請修類別">
          <el-select
            v-model="queryForm.ApplyType"
            placeholder="請選擇請修類別"
            clearable
            style="width: 200px"
          >
            <el-option label="全部" value="" />
            <!-- 可以根據實際需求添加更多選項 -->
          </el-select>
        </el-form-item>
        <el-form-item label="其他條件1">
          <el-input
            v-model="queryForm.OtherCondition1"
            placeholder="請輸入其他條件1"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="其他條件2">
          <el-input
            v-model="queryForm.OtherCondition2"
            placeholder="請輸入其他條件2"
            clearable
            style="width: 200px"
          />
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
        <el-table-column prop="SiteName" label="店別名稱" width="150" />
        <el-table-column prop="BelongStatus" label="費用負擔" width="120">
          <template #default="scope">
            {{ scope.row.BelongStatus === '1' ? '員工負擔' : scope.row.BelongStatus === '2' ? '店別負擔' : scope.row.BelongStatus }}
          </template>
        </el-table-column>
        <el-table-column prop="ApplyDateB" label="日統計表起" width="120" />
        <el-table-column prop="ApplyDateE" label="日統計表迄" width="120" />
        <el-table-column prop="BelongOrg" label="費用歸屬單位" width="150" />
        <el-table-column prop="MaintainEmp" label="維保人員" width="120" />
        <el-table-column prop="ApplyType" label="請修類別" width="120" />
        <el-table-column prop="OtherCondition1" label="其他條件1" width="150" />
        <el-table-column prop="OtherCondition2" label="其他條件2" width="150" />
        <el-table-column prop="RequestCount" label="申請件數" width="120" align="right">
          <template #default="scope">
            {{ formatNumber(scope.row.RequestCount) }}
          </template>
        </el-table-column>
        <el-table-column prop="TotalAmount" label="總金額" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.TotalAmount) }}
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
  name: 'SYSA1024Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        BelongStatus: '',
        ApplyDateB: '',
        ApplyDateE: '',
        BelongOrg: '',
        MaintainEmp: '',
        ApplyType: '',
        OtherCondition1: '',
        OtherCondition2: '',
        PageIndex: 1,
        PageSize: 20
      },
      tableData: [],
      pagination: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      },
      siteList: [],
      orgList: [],
      empList: []
    }
  },
  mounted() {
    this.loadSiteList()
    this.loadOrgList()
    this.loadEmpList()
    // 設置預設日期範圍（最近一個月）
    const today = new Date()
    const lastMonth = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate())
    this.queryForm.ApplyDateB = this.formatDate(lastMonth)
    this.queryForm.ApplyDateE = this.formatDate(today)
  },
  methods: {
    formatDate(date) {
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      return `${year}-${month}-${day}`
    },
    async loadSiteList() {
      try {
        const response = await dropdownListApi.getSiteOptions({})
        if (response.data && response.data.success && response.data.data) {
          this.siteList = response.data.data.map(item => ({
            SiteId: item.value || item.SiteId || item.siteId,
            SiteName: item.label || item.SiteName || item.siteName
          }))
        }
      } catch (error) {
        console.error('載入店別列表失敗:', error)
        ElMessage.warning('載入店別列表失敗')
      }
    },
    async loadOrgList() {
      try {
        // TODO: 實現載入費用歸屬單位列表的 API
        // const response = await dropdownListApi.getOrgOptions({})
        // if (response.data && response.data.success && response.data.data) {
        //   this.orgList = response.data.data.map(item => ({
        //     OrgId: item.value || item.OrgId || item.orgId,
        //     OrgName: item.label || item.OrgName || item.orgName
        //   }))
        // }
        this.orgList = []
      } catch (error) {
        console.error('載入費用歸屬單位列表失敗:', error)
      }
    },
    async loadEmpList() {
      try {
        // TODO: 實現載入維保人員列表的 API
        // const response = await dropdownListApi.getEmpOptions({})
        // if (response.data && response.data.success && response.data.data) {
        //   this.empList = response.data.data.map(item => ({
        //     EmpId: item.value || item.EmpId || item.empId,
        //     EmpName: item.label || item.EmpName || item.empName
        //   }))
        // }
        this.empList = []
      } catch (error) {
        console.error('載入維保人員列表失敗:', error)
      }
    },
    async handleSearch() {
      // 驗證必填欄位
      if (!this.queryForm.ApplyDateB) {
        ElMessage.warning('請選擇日統計表起')
        return
      }
      if (!this.queryForm.ApplyDateE) {
        ElMessage.warning('請選擇日統計表迄')
        return
      }

      this.loading = true
      try {
        const response = await analysisReportApi.getSYSA1024Report(this.queryForm)
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
        console.error('查詢工務維修統計報表(其他)失敗:', error)
        ElMessage.error('查詢工務維修統計報表(其他)失敗')
      } finally {
        this.loading = false
      }
    },
    handleReset() {
      const today = new Date()
      const lastMonth = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate())
      this.queryForm = {
        SiteId: '',
        BelongStatus: '',
        ApplyDateB: this.formatDate(lastMonth),
        ApplyDateE: this.formatDate(today),
        BelongOrg: '',
        MaintainEmp: '',
        ApplyType: '',
        OtherCondition1: '',
        OtherCondition2: '',
        PageIndex: 1,
        PageSize: 20
      }
      this.tableData = []
      this.pagination = {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      }
    },
    async handleExport() {
      if (!this.queryForm.ApplyDateB || !this.queryForm.ApplyDateE) {
        ElMessage.warning('請選擇日期範圍')
        return
      }

      try {
        const response = await analysisReportApi.exportSYSA1024Report(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSA1024_工務維修統計報表(其他)_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出工務維修統計報表(其他)失敗:', error)
        ElMessage.error('匯出工務維修統計報表(其他)失敗')
      }
    },
    async handlePrint() {
      if (!this.queryForm.ApplyDateB || !this.queryForm.ApplyDateE) {
        ElMessage.warning('請選擇日期範圍')
        return
      }

      try {
        const response = await analysisReportApi.printSYSA1024Report(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印工務維修統計報表(其他)失敗:', error)
        ElMessage.error('列印工務維修統計報表(其他)失敗')
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
      if (value === null || value === undefined) return '0'
      return Number(value).toLocaleString('zh-TW')
    },
    formatCurrency(value) {
      if (value === null || value === undefined) return '$0.00'
      return '$' + Number(value).toLocaleString('zh-TW', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      })
    }
  }
}
</script>

<style scoped>
.sysa1024-report {
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
