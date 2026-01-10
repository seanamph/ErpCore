<template>
  <div class="user-query-report">
    <div class="page-header">
      <h1>使用者查詢結果 (SYS0910)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :model="queryForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="使用者代碼">
              <el-input v-model="queryForm.UserId" placeholder="請輸入使用者代碼" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="使用者名稱">
              <el-input v-model="queryForm.UserName" placeholder="請輸入使用者名稱" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="組織代碼">
              <el-input v-model="queryForm.OrgId" placeholder="請輸入組織代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="狀態">
              <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="使用者類型">
              <el-input v-model="queryForm.UserType" placeholder="請輸入使用者類型" clearable />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="職稱">
              <el-input v-model="queryForm.Title" placeholder="請輸入職稱" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="店別代碼">
              <el-input v-model="queryForm.ShopId" placeholder="請輸入店別代碼" clearable />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExportExcel">匯出Excel</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
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
        <el-table-column prop="UserId" label="使用者代碼" width="150" />
        <el-table-column prop="UserName" label="使用者名稱" width="200" />
        <el-table-column prop="Title" label="職稱" width="150" />
        <el-table-column prop="OrgId" label="組織代碼" width="150" />
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
              {{ row.Status === 'A' ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="UserType" label="使用者類型" width="150" />
        <el-table-column prop="ShopId" label="店別代碼" width="150" />
        <el-table-column prop="LastLoginDate" label="最後登入日期" width="180">
          <template #default="{ row }">
            {{ row.LastLoginDate ? new Date(row.LastLoginDate).toLocaleString('zh-TW') : '' }}
          </template>
        </el-table-column>
        <el-table-column prop="LastLoginIp" label="最後登入IP" width="150" />
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
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { userQueryApi } from '@/api/systemPermission'

export default {
  name: 'UserQueryReport',
  setup() {
    const loading = ref(false)
    const tableData = ref([])

    // 查詢表單
    const queryForm = reactive({
      UserId: '',
      UserName: '',
      OrgId: '',
      Status: '',
      UserType: '',
      Title: '',
      ShopId: ''
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const data = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          ...queryForm
        }
        const response = await userQueryApi.queryUsers(data)
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
        UserId: '',
        UserName: '',
        OrgId: '',
        Status: '',
        UserType: '',
        Title: '',
        ShopId: ''
      })
      pagination.PageIndex = 1
      handleSearch()
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      try {
        const data = { ...queryForm }
        const response = await userQueryApi.exportUserQuery(data, 'excel')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `使用者查詢結果_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.xlsx`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 匯出PDF
    const handleExportPdf = async () => {
      try {
        const data = { ...queryForm }
        const response = await userQueryApi.exportUserQuery(data, 'pdf')
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/pdf'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `使用者查詢結果_${new Date().toISOString().slice(0, 10).replace(/-/g, '')}.pdf`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
      queryForm,
      pagination,
      handleSearch,
      handleReset,
      handleExportExcel,
      handleExportPdf,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.user-query-report {
  .search-card {
    margin-bottom: 20px;
  }

  .table-card {
    margin-top: 20px;
  }
}
</style>

