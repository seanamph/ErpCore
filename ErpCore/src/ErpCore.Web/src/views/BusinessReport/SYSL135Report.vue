<template>
  <div class="sysl135-report">
    <div class="page-header">
      <h1>業務報表查詢作業 (SYSL135)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="店別">
          <el-select
            v-model="queryForm.SiteId"
            placeholder="請選擇店別"
            clearable
            filterable
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
        <el-form-item label="卡別">
          <el-select
            v-model="queryForm.CardType"
            placeholder="請選擇卡別"
            clearable
            style="width: 200px"
            @change="handleCardTypeChange"
          >
            <el-option
              v-for="card in cardTypeList"
              :key="card.CardId"
              :label="card.CardName"
              :value="card.CardId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="廠商" v-if="queryForm.CardType !== '2004'">
          <el-input
            v-model="queryForm.VendorId"
            placeholder="請輸入廠商代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="專櫃" v-if="queryForm.CardType === '2004'">
          <el-input
            v-model="queryForm.StoreId"
            placeholder="請輸入專櫃代碼"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item label="組織">
          <el-select
            v-model="queryForm.OrgId"
            placeholder="請選擇組織"
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
        <el-form-item label="日期範圍">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 300px"
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
        <el-table-column prop="SiteId" label="店別代碼" width="120" />
        <el-table-column prop="SiteName" label="店別名稱" width="150" />
        <el-table-column prop="CardType" label="卡片類型" width="120" />
        <el-table-column prop="CardTypeName" label="卡片類型名稱" width="150" />
        <el-table-column prop="VendorId" label="廠商代碼" width="120" />
        <el-table-column prop="VendorName" label="廠商名稱" width="150" />
        <el-table-column prop="StoreId" label="專櫃代碼" width="120" />
        <el-table-column prop="StoreName" label="專櫃名稱" width="150" />
        <el-table-column prop="AgreementId" label="合約代碼" width="120" />
        <el-table-column prop="OrgId" label="組織代碼" width="120" />
        <el-table-column prop="OrgName" label="組織名稱" width="150" />
        <el-table-column prop="ActionType" label="動作類型" width="120" />
        <el-table-column prop="ActionTypeName" label="動作類型名稱" width="150" />
        <el-table-column prop="ReportDate" label="報表日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.ReportDate) }}
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
import { businessReportApi } from '@/api/businessReport'
import { dropdownListApi } from '@/api/dropdownList'
import { ElMessage } from 'element-plus'

export default {
  name: 'SYSL135Report',
  data() {
    return {
      loading: false,
      queryForm: {
        SiteId: '',
        CardType: '',
        VendorId: '',
        StoreId: '',
        OrgId: '',
        StartDate: null,
        EndDate: null,
        PageIndex: 1,
        PageSize: 20
      },
      dateRange: null,
      tableData: [],
      pagination: {
        PageIndex: 1,
        PageSize: 20,
        TotalCount: 0,
        TotalPages: 0
      },
      siteList: [],
      cardTypeList: [],
      orgList: []
    }
  },
  watch: {
    dateRange(newVal) {
      if (newVal && newVal.length === 2) {
        this.queryForm.StartDate = newVal[0]
        this.queryForm.EndDate = newVal[1]
      } else {
        this.queryForm.StartDate = null
        this.queryForm.EndDate = null
      }
    }
  },
  mounted() {
    this.loadSiteList()
    this.loadCardTypeList()
    this.loadOrgList()
  },
  methods: {
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
    async loadCardTypeList() {
      try {
        // TODO: 實作卡片類型列表 API，目前先使用空陣列
        // const response = await dropdownListApi.getCardTypeOptions({})
        // if (response.data && response.data.success && response.data.data) {
        //   this.cardTypeList = response.data.data.map(item => ({
        //     CardId: item.value || item.CardId || item.cardId,
        //     CardName: item.label || item.CardName || item.cardName
        //   }))
        // }
        this.cardTypeList = []
      } catch (error) {
        console.error('載入卡片類型列表失敗:', error)
        ElMessage.warning('載入卡片類型列表失敗')
      }
    },
    async loadOrgList() {
      try {
        // TODO: 實作組織列表 API，目前先使用空陣列
        // const response = await dropdownListApi.getOrgOptions({})
        // if (response.data && response.data.success && response.data.data) {
        //   this.orgList = response.data.data.map(item => ({
        //     OrgId: item.value || item.OrgId || item.orgId,
        //     OrgName: item.label || item.OrgName || item.orgName
        //   }))
        // }
        this.orgList = []
      } catch (error) {
        console.error('載入組織列表失敗:', error)
        ElMessage.warning('載入組織列表失敗')
      }
    },
    handleCardTypeChange(value) {
      if (value === '2004') {
        // 專櫃模式，清空廠商
        this.queryForm.VendorId = ''
      } else {
        // 廠商模式，清空專櫃
        this.queryForm.StoreId = ''
      }
    },
    async handleSearch() {
      this.loading = true
      try {
        const response = await businessReportApi.getBusinessReports(this.queryForm)
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
        console.error('查詢業務報表失敗:', error)
        ElMessage.error('查詢業務報表失敗')
      } finally {
        this.loading = false
      }
    },
    handleReset() {
      this.queryForm = {
        SiteId: '',
        CardType: '',
        VendorId: '',
        StoreId: '',
        OrgId: '',
        StartDate: null,
        EndDate: null,
        PageIndex: 1,
        PageSize: 20
      }
      this.dateRange = null
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
        const response = await businessReportApi.exportBusinessReports(this.queryForm, 'excel')
        const blob = new Blob([response.data], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `SYSL135_業務報表查詢_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出業務報表失敗:', error)
        ElMessage.error('匯出業務報表失敗')
      }
    },
    async handlePrint() {
      try {
        const response = await businessReportApi.printBusinessReports(this.queryForm)
        const blob = new Blob([response.data], { type: 'application/pdf' })
        const url = window.URL.createObjectURL(blob)
        window.open(url, '_blank')
        ElMessage.success('列印成功')
      } catch (error) {
        console.error('列印業務報表失敗:', error)
        ElMessage.error('列印業務報表失敗')
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
    formatDate(date) {
      if (!date) return ''
      const d = new Date(date)
      return d.toLocaleDateString('zh-TW', { year: 'numeric', month: '2-digit', day: '2-digit' })
    }
  }
}
</script>

<style scoped>
.sysl135-report {
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
