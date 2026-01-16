<template>
  <div class="pop-print">
    <div class="page-header">
      <h1>POP卡商品卡列印作業_UA (SYSW172)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="商品編號">
          <el-input v-model="queryForm.GoodsId" placeholder="請輸入商品編號" clearable />
        </el-form-item>
        <el-form-item label="商品名稱">
          <el-input v-model="queryForm.GoodsName" placeholder="請輸入商品名稱" clearable />
        </el-form-item>
        <el-form-item label="條碼">
          <el-input v-model="queryForm.BarCode" placeholder="請輸入條碼" clearable />
        </el-form-item>
        <el-form-item label="品牌">
          <el-input v-model="queryForm.LogoId" placeholder="請輸入品牌編號" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="info" @click="handleSettings">列印設定</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 資料表格 -->
    <el-card class="table-card" shadow="never">
      <div class="table-toolbar">
        <el-button
          type="primary"
          :disabled="selectedProducts.length === 0"
          @click="handlePrint"
        >
          列印
        </el-button>
        <el-button
          type="success"
          :disabled="selectedProducts.length === 0"
          @click="handleExportExcel"
        >
          匯出Excel
        </el-button>
      </div>

      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        @selection-change="handleSelectionChange"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="GoodsId" label="商品編號" width="120" />
        <el-table-column prop="GoodsName" label="商品名稱" width="200" />
        <el-table-column prop="BarCode" label="條碼" width="150" />
        <el-table-column prop="Price" label="售價" width="100" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.Price) }}
          </template>
        </el-table-column>
        <el-table-column prop="Unit" label="單位" width="80" />
      </el-table>

      <el-pagination
        v-model:current-page="pagination.PageIndex"
        v-model:page-size="pagination.PageSize"
        :total="pagination.TotalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
      />
    </el-card>

    <!-- 列印設定對話框 -->
    <el-dialog
      v-model="printDialogVisible"
      title="列印設定 (UA版本)"
      width="600px"
      @close="handlePrintDialogClose"
    >
      <el-form :model="printForm" :rules="printRules" ref="printFormRef" label-width="150px">
        <el-form-item label="列印類型" prop="PrintType">
          <el-radio-group v-model="printForm.PrintType">
            <el-radio label="POP">POP卡</el-radio>
            <el-radio label="PRODUCT_CARD">商品卡</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="列印格式 (UA)" prop="PrintFormat">
          <el-select v-model="printForm.PrintFormat" placeholder="請選擇列印格式">
            <el-option label="PR1_UA" value="PR1_UA" />
            <el-option label="PR2_UA" value="PR2_UA" />
            <el-option label="PR3_UA" value="PR3_UA" />
            <el-option label="PR4_UA" value="PR4_UA" />
            <el-option label="PR5_UA" value="PR5_UA" />
            <el-option label="PR6_UA" value="PR6_UA" />
          </el-select>
        </el-form-item>
        <el-form-item label="列印數量" prop="PrintCount">
          <el-input-number v-model="printForm.PrintCount" :min="1" :max="100" />
        </el-form-item>
        <el-form-item label="店別">
          <el-input v-model="printForm.ShopId" placeholder="請輸入店別編號" clearable />
        </el-form-item>
        <el-form-item label="包含條碼">
          <el-switch v-model="printForm.Options.IncludeBarcode" />
        </el-form-item>
        <el-form-item label="包含價格">
          <el-switch v-model="printForm.Options.IncludePrice" />
        </el-form-item>
        <el-form-item label="包含備註">
          <el-switch v-model="printForm.Options.IncludeNote" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="printDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handlePrintConfirm">列印</el-button>
      </template>
    </el-dialog>

    <!-- 列印設定管理對話框 -->
    <el-dialog
      v-model="settingsDialogVisible"
      title="列印設定管理 (UA版本)"
      width="600px"
      @close="handleSettingsDialogClose"
    >
      <el-form :model="settingsForm" ref="settingsFormRef" label-width="150px">
        <el-form-item label="店別">
          <el-input v-model="settingsForm.ShopId" placeholder="請輸入店別編號" clearable />
        </el-form-item>
        <el-form-item label="IP位址">
          <el-input v-model="settingsForm.Ip" placeholder="請輸入IP位址" />
        </el-form-item>
        <el-form-item label="報表類型">
          <el-input v-model="settingsForm.TypeId" placeholder="請輸入報表類型 (如: PR1_UA)" />
        </el-form-item>
        <el-form-item label="除錯模式">
          <el-switch v-model="settingsForm.DebugMode" />
        </el-form-item>
        <el-form-item label="標題高度填補">
          <el-input-number v-model="settingsForm.HeaderHeightPadding" :min="0" />
        </el-form-item>
        <el-form-item label="標題高度填補剩餘">
          <el-input-number v-model="settingsForm.HeaderHeightPaddingRemain" :min="0" />
        </el-form-item>
        <el-form-item label="頁面標題高度填補">
          <el-input-number v-model="settingsForm.PageHeaderHeightPadding" :min="0" />
        </el-form-item>
        <el-form-item label="頁面填補">
          <el-input v-model="settingsForm.PagePadding" placeholder="左,右,上,下" />
        </el-form-item>
        <el-form-item label="頁面大小">
          <el-input v-model="settingsForm.PageSize" placeholder="高,寬" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="settingsDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSettingsSave">儲存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { popPrintUaApi } from '@/api/popPrintUa'

export default {
  name: 'PopPrintUa',
  setup() {
    const loading = ref(false)
    const tableData = ref([])
    const selectedProducts = ref([])
    const printDialogVisible = ref(false)
    const settingsDialogVisible = ref(false)
    const printFormRef = ref(null)
    const settingsFormRef = ref(null)

    const queryForm = reactive({
      GoodsId: '',
      GoodsName: '',
      BarCode: '',
      LogoId: ''
    })

    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    const printForm = reactive({
      PrintType: 'POP',
      PrintFormat: 'PR1_UA',
      PrintCount: 1,
      ShopId: '',
      Options: {
        IncludeBarcode: true,
        IncludePrice: true,
        IncludeNote: false
      }
    })

    const printRules = {
      PrintType: [{ required: true, message: '請選擇列印類型', trigger: 'change' }],
      PrintFormat: [{ required: true, message: '請選擇列印格式', trigger: 'change' }],
      PrintCount: [{ required: true, message: '請輸入列印數量', trigger: 'blur' }]
    }

    const settingsForm = reactive({
      ShopId: '',
      Ip: '',
      TypeId: '',
      DebugMode: false,
      HeaderHeightPadding: 0,
      HeaderHeightPaddingRemain: 851,
      PageHeaderHeightPadding: 0,
      PagePadding: '',
      PageSize: ''
    })

    // 查詢商品列表
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize,
          ...queryForm
        }
        const response = await popPrintUaApi.getProducts(params)
        if (response.Success) {
          tableData.value = response.Data.Items || []
          pagination.TotalCount = response.Data.TotalCount || 0
        } else {
          ElMessage.error(response.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢失敗:', error)
        ElMessage.error('查詢失敗')
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
        GoodsId: '',
        GoodsName: '',
        BarCode: '',
        LogoId: ''
      })
      handleSearch()
    }

    // 選擇變更
    const handleSelectionChange = (selection) => {
      selectedProducts.value = selection.map(item => item.GoodsId)
    }

    // 列印
    const handlePrint = () => {
      if (selectedProducts.value.length === 0) {
        ElMessage.warning('請選擇要列印的商品')
        return
      }
      printDialogVisible.value = true
    }

    // 列印確認
    const handlePrintConfirm = async () => {
      if (!printFormRef.value) return

      await printFormRef.value.validate(async (valid) => {
        if (valid) {
          try {
            const data = {
              GoodsIds: selectedProducts.value,
              PrintType: printForm.PrintType,
              PrintFormat: printForm.PrintFormat,
              PrintCount: printForm.PrintCount,
              ShopId: printForm.ShopId || null,
              Options: printForm.Options
            }
            const response = await popPrintUaApi.print(data)
            if (response.Success) {
              ElMessage.success('列印成功')
              printDialogVisible.value = false
              loadData()
            } else {
              ElMessage.error(response.Message || '列印失敗')
            }
          } catch (error) {
            console.error('列印失敗:', error)
            ElMessage.error('列印失敗')
          }
        }
      })
    }

    // 列印對話框關閉
    const handlePrintDialogClose = () => {
      if (printFormRef.value) {
        printFormRef.value.resetFields()
      }
    }

    // 匯出Excel
    const handleExportExcel = async () => {
      if (selectedProducts.value.length === 0) {
        ElMessage.warning('請選擇要匯出的商品')
        return
      }

      try {
        const data = {
          GoodsIds: selectedProducts.value,
          PrintType: 'POP',
          PrintFormat: 'PR1_UA',
          ShopId: null,
          Options: {
            IncludeBarcode: true,
            IncludePrice: true,
            IncludeNote: false
          }
        }
        const response = await popPrintUaApi.exportExcel(data)
        
        // 下載檔案
        const blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `POP列印資料_UA_${new Date().toISOString().slice(0, 10)}.xlsx`
        link.click()
        window.URL.revokeObjectURL(url)
        
        ElMessage.success('匯出成功')
      } catch (error) {
        console.error('匯出失敗:', error)
        ElMessage.error('匯出失敗')
      }
    }

    // 列印設定
    const handleSettings = async () => {
      try {
        const response = await popPrintUaApi.getSettings(null)
        if (response.Success && response.Data) {
          Object.assign(settingsForm, response.Data)
        }
        settingsDialogVisible.value = true
      } catch (error) {
        console.error('取得設定失敗:', error)
        settingsDialogVisible.value = true
      }
    }

    // 儲存設定
    const handleSettingsSave = async () => {
      try {
        const response = await popPrintUaApi.updateSettings(settingsForm.ShopId || null, settingsForm)
        if (response.Success) {
          ElMessage.success('儲存成功')
          settingsDialogVisible.value = false
        } else {
          ElMessage.error(response.Message || '儲存失敗')
        }
      } catch (error) {
        console.error('儲存失敗:', error)
        ElMessage.error('儲存失敗')
      }
    }

    // 設定對話框關閉
    const handleSettingsDialogClose = () => {
      // 重置表單
    }

    // 分頁大小變更
    const handleSizeChange = (size) => {
      pagination.PageSize = size
      pagination.PageIndex = 1
      loadData()
    }

    // 分頁變更
    const handlePageChange = (page) => {
      pagination.PageIndex = page
      loadData()
    }

    // 格式化貨幣
    const formatCurrency = (value) => {
      if (value == null) return '-'
      return new Intl.NumberFormat('zh-TW', {
        style: 'currency',
        currency: 'TWD',
        minimumFractionDigits: 0
      }).format(value)
    }

    // 初始化
    onMounted(() => {
      loadData()
    })

    return {
      loading,
      tableData,
      selectedProducts,
      queryForm,
      pagination,
      printDialogVisible,
      settingsDialogVisible,
      printForm,
      printRules,
      printFormRef,
      settingsForm,
      settingsFormRef,
      handleSearch,
      handleReset,
      handleSelectionChange,
      handlePrint,
      handlePrintConfirm,
      handlePrintDialogClose,
      handleExportExcel,
      handleSettings,
      handleSettingsSave,
      handleSettingsDialogClose,
      handleSizeChange,
      handlePageChange,
      formatCurrency
    }
  }
}
</script>

<style scoped lang="scss">
@import '@/assets/styles/variables.scss';

.pop-print {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;

    h1 {
      font-size: 24px;
      font-weight: 500;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;

    .search-form {
      .el-form-item {
        margin-bottom: 10px;
      }
    }
  }

  .table-card {
    .table-toolbar {
      margin-bottom: 15px;
    }
  }
}
</style>
