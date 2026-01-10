<template>
  <div class="employee-meal-card-list">
    <div class="page-header">
      <h1>業務報表查詢作業 (SYSL130)</h1>
    </div>

    <!-- 查詢表單 -->
    <el-card class="search-card" shadow="never">
      <el-form :inline="true" :model="queryForm" class="search-form">
        <el-form-item label="員工編號">
          <el-input v-model="queryForm.EmpId" placeholder="請輸入員工編號" clearable />
        </el-form-item>
        <el-form-item label="員工姓名">
          <el-input v-model="queryForm.EmpName" placeholder="請輸入員工姓名" clearable />
        </el-form-item>
        <el-form-item label="組織">
          <el-select v-model="queryForm.OrgId" placeholder="請選擇組織" clearable filterable>
            <el-option
              v-for="org in orgList"
              :key="org.OrgId"
              :label="org.OrgName"
              :value="org.OrgId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="店別">
          <el-select v-model="queryForm.SiteId" placeholder="請選擇店別" clearable filterable>
            <el-option
              v-for="site in siteList"
              :key="site.SiteId"
              :label="site.SiteName"
              :value="site.SiteId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="卡片類型">
          <el-select v-model="queryForm.CardType" placeholder="請選擇卡片類型" clearable>
            <el-option
              v-for="card in cardTypeList"
              :key="card.CardId"
              :label="card.CardName"
              :value="card.CardId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="動作類型">
          <el-select v-model="queryForm.ActionType" placeholder="請選擇動作類型" clearable>
            <el-option
              v-for="action in actionTypeList"
              :key="action.ActionId"
              :label="action.ActionName"
              :value="action.ActionId"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="queryForm.Status" placeholder="請選擇狀態" clearable>
            <el-option label="全部" value="" />
            <el-option label="待審核" value="P" />
            <el-option label="已審核" value="A" />
            <el-option label="已拒絕" value="R" />
          </el-select>
        </el-form-item>
        <el-form-item label="起始日期">
          <el-date-picker
            v-model="queryForm.StartDateFrom"
            type="date"
            placeholder="請選擇起始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item label="結束日期">
          <el-date-picker
            v-model="queryForm.StartDateTo"
            type="date"
            placeholder="請選擇結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            clearable
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleCreate">新增</el-button>
          <el-button
            type="warning"
            @click="handleBatchVerify"
            :disabled="selectedRows.length === 0"
          >
            批次審核
          </el-button>
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="TKey" label="主鍵" width="100" />
        <el-table-column prop="EmpId" label="員工編號" width="120" />
        <el-table-column prop="EmpName" label="員工姓名" width="120" />
        <el-table-column prop="OrgId" label="組織" width="120" />
        <el-table-column prop="SiteId" label="店別" width="120" />
        <el-table-column prop="CardType" label="卡片類型" width="120">
          <template #default="{ row }">
            {{ getCardTypeName(row.CardType) }}
          </template>
        </el-table-column>
        <el-table-column prop="ActionType" label="動作類型" width="120">
          <template #default="{ row }">
            {{ getActionTypeName(row.ActionType) }}
          </template>
        </el-table-column>
        <el-table-column prop="ActionTypeD" label="動作類型明細" width="150">
          <template #default="{ row }">
            {{ getActionTypeDetailName(row.ActionType, row.ActionTypeD) }}
          </template>
        </el-table-column>
        <el-table-column prop="StartDate" label="起始日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.StartDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="EndDate" label="結束日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.EndDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="狀態" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.Status)">
              {{ getStatusName(row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Verifier" label="審核者" width="120" />
        <el-table-column prop="VerifyDate" label="審核日期" width="120">
          <template #default="{ row }">
            {{ formatDate(row.VerifyDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="Notes" label="備註" min-width="150" show-overflow-tooltip />
        <el-table-column prop="CreatedAt" label="建立時間" width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="250" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
            <el-button
              type="warning"
              size="small"
              @click="handleEdit(row)"
              :disabled="row.Status === 'A'"
            >
              修改
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="handleDelete(row)"
              :disabled="row.Status === 'A'"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
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

    <!-- 新增/修改對話框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="90%"
      :close-on-click-modal="false"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="員工編號" prop="EmpId">
              <el-input v-model="formData.EmpId" :disabled="isEdit" placeholder="請輸入員工編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="員工姓名" prop="EmpName">
              <el-input v-model="formData.EmpName" placeholder="請輸入員工姓名" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="組織" prop="OrgId">
              <el-select
                v-model="formData.OrgId"
                placeholder="請選擇組織"
                clearable
                filterable
                style="width: 100%"
              >
                <el-option
                  v-for="org in orgList"
                  :key="org.OrgId"
                  :label="org.OrgName"
                  :value="org.OrgId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="店別" prop="SiteId">
              <el-select
                v-model="formData.SiteId"
                placeholder="請選擇店別"
                clearable
                filterable
                style="width: 100%"
              >
                <el-option
                  v-for="site in siteList"
                  :key="site.SiteId"
                  :label="site.SiteName"
                  :value="site.SiteId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="卡片類型" prop="CardType">
              <el-select
                v-model="formData.CardType"
                placeholder="請選擇卡片類型"
                clearable
                style="width: 100%"
              >
                <el-option
                  v-for="card in cardTypeList"
                  :key="card.CardId"
                  :label="card.CardName"
                  :value="card.CardId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="動作類型" prop="ActionType">
              <el-select
                v-model="formData.ActionType"
                placeholder="請選擇動作類型"
                clearable
                style="width: 100%"
                @change="handleActionTypeChange"
              >
                <el-option
                  v-for="action in actionTypeList"
                  :key="action.ActionId"
                  :label="action.ActionName"
                  :value="action.ActionId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="動作類型明細" prop="ActionTypeD">
              <el-select
                v-model="formData.ActionTypeD"
                placeholder="請選擇動作類型明細"
                clearable
                :disabled="!formData.ActionType"
                style="width: 100%"
              >
                <el-option
                  v-for="detail in filteredActionTypeDetails"
                  :key="detail.ActionIdD"
                  :label="detail.ActionNameD"
                  :value="detail.ActionIdD"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="交易單號" prop="TxnNo">
              <el-input v-model="formData.TxnNo" placeholder="請輸入交易單號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="起始日期" prop="StartDate">
              <el-date-picker
                v-model="formData.StartDate"
                type="date"
                placeholder="請選擇起始日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="EndDate">
              <el-date-picker
                v-model="formData.EndDate"
                type="date"
                placeholder="請選擇結束日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註" prop="Notes">
          <el-input
            v-model="formData.Notes"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 批次審核對話框 -->
    <el-dialog
      v-model="batchVerifyDialogVisible"
      title="批次審核"
      width="500px"
      :close-on-click-modal="false"
    >
      <el-form :model="batchVerifyForm" label-width="120px">
        <el-form-item label="審核動作">
          <el-radio-group v-model="batchVerifyForm.Action">
            <el-radio label="approve">通過</el-radio>
            <el-radio label="reject">拒絕</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="備註">
          <el-input
            v-model="batchVerifyForm.Notes"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
        <el-form-item label="選取筆數">
          <span>{{ selectedRows.length }} 筆</span>
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="batchVerifyDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleBatchVerifySubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { employeeMealCardApi } from '@/api/businessReport'
import { departmentsApi } from '@/api/departments'
import { shopsApi } from '@/api/modules/shop'

export default {
  name: 'EmployeeMealCardList',
  setup() {
    const loading = ref(false)
    const dialogVisible = ref(false)
    const batchVerifyDialogVisible = ref(false)
    const isEdit = ref(false)
    const formRef = ref(null)
    const tableData = ref([])
    const selectedRows = ref([])

    // 下拉選單資料
    const cardTypeList = ref([])
    const actionTypeList = ref([])
    const actionTypeDetailList = ref([])
    const orgList = ref([])
    const siteList = ref([])

    // 查詢表單
    const queryForm = reactive({
      EmpId: '',
      EmpName: '',
      OrgId: '',
      SiteId: '',
      CardType: '',
      ActionType: '',
      Status: '',
      StartDateFrom: '',
      StartDateTo: '',
      EndDateFrom: '',
      EndDateTo: '',
      PageIndex: 1,
      PageSize: 20
    })

    // 分頁資訊
    const pagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })

    // 表單資料
    const formData = reactive({
      TKey: null,
      EmpId: '',
      EmpName: '',
      OrgId: '',
      SiteId: '',
      CardType: '',
      ActionType: '',
      ActionTypeD: '',
      StartDate: '',
      EndDate: '',
      Notes: '',
      TxnNo: ''
    })

    // 批次審核表單
    const batchVerifyForm = reactive({
      Action: 'approve',
      Notes: ''
    })

    // 表單驗證規則
    const formRules = {
      EmpId: [{ required: true, message: '請輸入員工編號', trigger: 'blur' }],
      EmpName: [{ required: true, message: '請輸入員工姓名', trigger: 'blur' }]
    }

    // 過濾動作類型明細
    const filteredActionTypeDetails = computed(() => {
      if (!formData.ActionType) return []
      return actionTypeDetailList.value.filter(
        (detail) => detail.ActionId === formData.ActionType
      )
    })

    // 取得卡片類型名稱
    const getCardTypeName = (cardId) => {
      const card = cardTypeList.value.find((c) => c.CardId === cardId)
      return card ? card.CardName : cardId || ''
    }

    // 取得動作類型名稱
    const getActionTypeName = (actionId) => {
      const action = actionTypeList.value.find((a) => a.ActionId === actionId)
      return action ? action.ActionName : actionId || ''
    }

    // 取得動作類型明細名稱
    const getActionTypeDetailName = (actionId, actionIdD) => {
      const detail = actionTypeDetailList.value.find(
        (d) => d.ActionId === actionId && d.ActionIdD === actionIdD
      )
      return detail ? detail.ActionNameD : actionIdD || ''
    }

    // 取得狀態名稱
    const getStatusName = (status) => {
      const statuses = {
        P: '待審核',
        A: '已審核',
        R: '已拒絕'
      }
      return statuses[status] || status
    }

    // 取得狀態類型
    const getStatusType = (status) => {
      const types = {
        P: 'warning',
        A: 'success',
        R: 'danger'
      }
      return types[status] || ''
    }

    // 格式化日期
    const formatDate = (date) => {
      if (!date) return ''
      return new Date(date).toLocaleDateString('zh-TW')
    }

    // 格式化日期時間
    const formatDateTime = (dateTime) => {
      if (!dateTime) return ''
      return new Date(dateTime).toLocaleString('zh-TW')
    }

    // 載入下拉選單資料
    const loadDropdowns = async () => {
      try {
        const response = await employeeMealCardApi.getDropdowns()
        if (response.Data) {
          cardTypeList.value = response.Data.CardTypes || []
          actionTypeList.value = response.Data.ActionTypes || []
          actionTypeDetailList.value = response.Data.ActionTypeDetails || []
        }
      } catch (error) {
        console.error('載入下拉選單資料失敗:', error)
      }
    }

    // 載入組織和店別資料
    const loadOrgAndSiteList = async () => {
      try {
        // 載入部別（組織）資料
        const deptResponse = await departmentsApi.getDepartments({ PageSize: 1000 })
        if (deptResponse.Data && deptResponse.Data.Items) {
          orgList.value = deptResponse.Data.Items.map(dept => ({
            OrgId: dept.DeptId,
            OrgName: dept.DeptName
          }))
        }

        // 載入店別資料
        const shopResponse = await shopsApi.getShops()
        if (shopResponse.Data) {
          siteList.value = Array.isArray(shopResponse.Data)
            ? shopResponse.Data.map(shop => ({
                SiteId: shop.ShopId,
                SiteName: shop.ShopName
              }))
            : []
        }
      } catch (error) {
        console.error('載入組織和店別資料失敗:', error)
        ElMessage.warning('載入組織和店別資料失敗，請稍後再試')
      }
    }

    // 查詢資料
    const loadData = async () => {
      loading.value = true
      try {
        const params = {
          ...queryForm,
          PageIndex: pagination.PageIndex,
          PageSize: pagination.PageSize
        }
        const response = await employeeMealCardApi.getMealCards(params)
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
        EmpId: '',
        EmpName: '',
        OrgId: '',
        SiteId: '',
        CardType: '',
        ActionType: '',
        Status: '',
        StartDateFrom: '',
        StartDateTo: '',
        EndDateFrom: '',
        EndDateTo: ''
      })
      handleSearch()
    }

    // 新增
    const handleCreate = () => {
      isEdit.value = false
      dialogVisible.value = true
      Object.assign(formData, {
        TKey: null,
        EmpId: '',
        EmpName: '',
        OrgId: '',
        SiteId: '',
        CardType: '',
        ActionType: '',
        ActionTypeD: '',
        StartDate: '',
        EndDate: '',
        Notes: '',
        TxnNo: ''
      })
    }

    // 查看
    const handleView = async (row) => {
      try {
        const response = await employeeMealCardApi.getMealCard(row.TKey)
        if (response.Data) {
          isEdit.value = true
          dialogVisible.value = true
          Object.assign(formData, {
            ...response.Data,
            StartDate: response.Data.StartDate
              ? new Date(response.Data.StartDate).toISOString().split('T')[0]
              : '',
            EndDate: response.Data.EndDate
              ? new Date(response.Data.EndDate).toISOString().split('T')[0]
              : ''
          })
        }
      } catch (error) {
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }

    // 修改
    const handleEdit = async (row) => {
      await handleView(row)
    }

    // 刪除
    const handleDelete = async (row) => {
      try {
        await ElMessageBox.confirm('確定要刪除此員工餐卡申請嗎？', '確認', {
          type: 'warning'
        })
        await employeeMealCardApi.deleteMealCard(row.TKey)
        ElMessage.success('刪除成功')
        loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }

    // 動作類型變更
    const handleActionTypeChange = () => {
      formData.ActionTypeD = ''
    }

    // 提交表單
    const handleSubmit = async () => {
      if (!formRef.value) return
      await formRef.value.validate(async (valid) => {
        if (valid) {
          try {
            const submitData = {
              ...formData
            }
            if (isEdit.value) {
              await employeeMealCardApi.updateMealCard(formData.TKey, submitData)
              ElMessage.success('修改成功')
            } else {
              await employeeMealCardApi.createMealCard(submitData)
              ElMessage.success('新增成功')
            }
            dialogVisible.value = false
            loadData()
          } catch (error) {
            ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
          }
        }
      })
    }

    // 選擇變更
    const handleSelectionChange = (selection) => {
      selectedRows.value = selection
    }

    // 批次審核
    const handleBatchVerify = () => {
      if (selectedRows.value.length === 0) {
        ElMessage.warning('請至少選擇一筆資料')
        return
      }
      batchVerifyDialogVisible.value = true
      batchVerifyForm.Action = 'approve'
      batchVerifyForm.Notes = ''
    }

    // 批次審核提交
    const handleBatchVerifySubmit = async () => {
      try {
        const tKeys = selectedRows.value.map((row) => row.TKey)
        const response = await employeeMealCardApi.batchVerify({
          TKeys: tKeys,
          Action: batchVerifyForm.Action,
          Notes: batchVerifyForm.Notes
        })
        if (response.Data) {
          ElMessage.success(
            `批次審核成功: 成功 ${response.Data.SuccessCount} 筆，失敗 ${response.Data.FailCount} 筆`
          )
          if (response.Data.Errors && response.Data.Errors.length > 0) {
            console.error('批次審核錯誤:', response.Data.Errors)
          }
        }
        batchVerifyDialogVisible.value = false
        selectedRows.value = []
        loadData()
      } catch (error) {
        ElMessage.error('批次審核失敗: ' + (error.message || '未知錯誤'))
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

    // 計算對話框標題
    const dialogTitle = computed(() => {
      return isEdit.value ? '修改員工餐卡申請' : '新增員工餐卡申請'
    })

    onMounted(() => {
      loadDropdowns()
      loadOrgAndSiteList()
      loadData()
    })

    return {
      loading,
      dialogVisible,
      batchVerifyDialogVisible,
      isEdit,
      formRef,
      tableData,
      selectedRows,
      cardTypeList,
      actionTypeList,
      actionTypeDetailList,
      filteredActionTypeDetails,
      orgList,
      siteList,
      queryForm,
      pagination,
      formData,
      batchVerifyForm,
      formRules,
      dialogTitle,
      getCardTypeName,
      getActionTypeName,
      getActionTypeDetailName,
      getStatusName,
      getStatusType,
      formatDate,
      formatDateTime,
      handleSearch,
      handleReset,
      handleCreate,
      handleView,
      handleEdit,
      handleDelete,
      handleActionTypeChange,
      handleSubmit,
      handleSelectionChange,
      handleBatchVerify,
      handleBatchVerifySubmit,
      handleSizeChange,
      handlePageChange
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.employee-meal-card-list {
  padding: 20px;

  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: bold;
      color: $primary-color;
      margin: 0;
    }
  }

  .search-card {
    margin-bottom: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }

  .table-card {
    margin-top: 20px;
    background-color: $card-bg;
    border-color: $card-border;
    color: $card-text;
  }
}
</style>

