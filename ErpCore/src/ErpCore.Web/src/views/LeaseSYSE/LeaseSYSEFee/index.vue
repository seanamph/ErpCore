<template>
  <div class="lease-syse-fee-management">
    <div class="page-header">
      <h1>費用資料維護 (SYSE310-SYSE430)</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 費用主檔 Tab -->
      <el-tab-pane label="費用主檔" name="fees">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="feeQueryForm" class="search-form">
            <el-form-item label="費用編號">
              <el-input v-model="feeQueryForm.FeeId" placeholder="請輸入費用編號" clearable />
            </el-form-item>
            <el-form-item label="租賃編號">
              <el-input v-model="feeQueryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
            </el-form-item>
            <el-form-item label="費用類型">
              <el-select v-model="feeQueryForm.FeeType" placeholder="請選擇費用類型" clearable>
                <el-option label="租金" value="RENT" />
                <el-option label="管理費" value="MANAGEMENT" />
                <el-option label="水電費" value="UTILITY" />
                <el-option label="其他費用" value="OTHER" />
              </el-select>
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="feeQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="待繳" value="P" />
                <el-option label="部分繳" value="P" />
                <el-option label="已繳" value="F" />
                <el-option label="已取消" value="C" />
              </el-select>
            </el-form-item>
            <el-form-item label="費用日期">
              <el-date-picker
                v-model="feeQueryForm.FeeDateFrom"
                type="date"
                placeholder="開始日期"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item label="至">
              <el-date-picker
                v-model="feeQueryForm.FeeDateTo"
                type="date"
                placeholder="結束日期"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleFeeSearch">查詢</el-button>
              <el-button @click="handleFeeReset">重置</el-button>
              <el-button type="success" @click="handleFeeCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="feeTableData"
            v-loading="feeLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="FeeId" label="費用編號" width="150" />
            <el-table-column prop="LeaseId" label="租賃編號" width="150" />
            <el-table-column prop="FeeType" label="費用類型" width="120">
              <template #default="{ row }">
                <el-tag :type="getFeeTypeTagType(row.FeeType)">
                  {{ getFeeTypeText(row.FeeType) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="FeeItemName" label="費用項目" width="200" />
            <el-table-column prop="FeeAmount" label="費用金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.FeeAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="TaxAmount" label="稅額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.TaxAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="TotalAmount" label="總金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.TotalAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="FeeDate" label="費用日期" width="120" />
            <el-table-column prop="DueDate" label="到期日期" width="120" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="getStatusTagType(row.Status)">
                  {{ getStatusText(row.Status) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="250" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleFeeView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleFeeEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleFeeDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="feePagination.PageIndex"
            v-model:page-size="feePagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="feePagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleFeeSizeChange"
            @current-change="handleFeePageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>

      <!-- 費用項目主檔 Tab -->
      <el-tab-pane label="費用項目主檔" name="items">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="itemQueryForm" class="search-form">
            <el-form-item label="費用項目編號">
              <el-input v-model="itemQueryForm.FeeItemId" placeholder="請輸入費用項目編號" clearable />
            </el-form-item>
            <el-form-item label="費用項目名稱">
              <el-input v-model="itemQueryForm.FeeItemName" placeholder="請輸入費用項目名稱" clearable />
            </el-form-item>
            <el-form-item label="費用類型">
              <el-select v-model="itemQueryForm.FeeType" placeholder="請選擇費用類型" clearable>
                <el-option label="租金" value="RENT" />
                <el-option label="管理費" value="MANAGEMENT" />
                <el-option label="水電費" value="UTILITY" />
                <el-option label="其他費用" value="OTHER" />
              </el-select>
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="itemQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="啟用" value="A" />
                <el-option label="停用" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleItemSearch">查詢</el-button>
              <el-button @click="handleItemReset">重置</el-button>
              <el-button type="success" @click="handleItemCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="itemTableData"
            v-loading="itemLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="FeeItemId" label="費用項目編號" width="150" />
            <el-table-column prop="FeeItemName" label="費用項目名稱" width="200" />
            <el-table-column prop="FeeType" label="費用類型" width="120">
              <template #default="{ row }">
                <el-tag :type="getFeeTypeTagType(row.FeeType)">
                  {{ getFeeTypeText(row.FeeType) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="DefaultAmount" label="預設金額" width="120">
              <template #default="{ row }">
                {{ formatCurrency(row.DefaultAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '啟用' : '停用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="250" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleItemView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleItemEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleItemDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="itemPagination.PageIndex"
            v-model:page-size="itemPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="itemPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleItemSizeChange"
            @current-change="handleItemPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 費用主檔對話框 -->
    <el-dialog
      v-model="feeDialogVisible"
      :title="feeDialogTitle"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="feeFormRef"
        :model="feeFormData"
        :rules="feeFormRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="費用編號" prop="FeeId">
              <el-input v-model="feeFormData.FeeId" :disabled="feeIsEdit" placeholder="請輸入費用編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="租賃編號" prop="LeaseId">
              <el-input v-model="feeFormData.LeaseId" placeholder="請輸入租賃編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="費用類型" prop="FeeType">
              <el-select v-model="feeFormData.FeeType" placeholder="請選擇費用類型" style="width: 100%">
                <el-option label="租金" value="RENT" />
                <el-option label="管理費" value="MANAGEMENT" />
                <el-option label="水電費" value="UTILITY" />
                <el-option label="其他費用" value="OTHER" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="費用項目編號" prop="FeeItemId">
              <el-select
                v-model="feeFormData.FeeItemId"
                placeholder="請選擇費用項目"
                filterable
                style="width: 100%"
                @change="handleFeeItemChange"
              >
                <el-option
                  v-for="item in feeItemList"
                  :key="item.FeeItemId"
                  :label="`${item.FeeItemId} - ${item.FeeItemName}`"
                  :value="item.FeeItemId"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="費用項目名稱" prop="FeeItemName">
              <el-input v-model="feeFormData.FeeItemName" placeholder="費用項目名稱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="費用日期" prop="FeeDate">
              <el-date-picker
                v-model="feeFormData.FeeDate"
                type="date"
                placeholder="請選擇費用日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="到期日期" prop="DueDate">
              <el-date-picker
                v-model="feeFormData.DueDate"
                type="date"
                placeholder="請選擇到期日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="費用金額" prop="FeeAmount">
              <el-input-number
                v-model="feeFormData.FeeAmount"
                :min="0"
                :precision="2"
                style="width: 100%"
                @change="calculateFeeTotal"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="稅率 (%)" prop="TaxRate">
              <el-input-number
                v-model="feeFormData.TaxRate"
                :min="0"
                :max="100"
                :precision="2"
                style="width: 100%"
                @change="calculateFeeTotal"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="稅額">
              <el-input-number
                v-model="feeFormData.TaxAmount"
                :min="0"
                :precision="2"
                style="width: 100%"
                :disabled="true"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="總金額">
              <el-input-number
                v-model="feeFormData.TotalAmount"
                :min="0"
                :precision="2"
                style="width: 100%"
                :disabled="true"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="feeFormData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="待繳" value="P" />
                <el-option label="部分繳" value="P" />
                <el-option label="已繳" value="F" />
                <el-option label="已取消" value="C" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="幣別" prop="CurrencyId">
              <el-input v-model="feeFormData.CurrencyId" placeholder="請輸入幣別" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="匯率" prop="ExchangeRate">
              <el-input-number
                v-model="feeFormData.ExchangeRate"
                :min="0"
                :precision="6"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="已繳金額" prop="PaidAmount">
              <el-input-number
                v-model="feeFormData.PaidAmount"
                :min="0"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="繳費日期" prop="PaidDate">
              <el-date-picker
                v-model="feeFormData.PaidDate"
                type="date"
                placeholder="請選擇繳費日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="feeFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="feeDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleFeeSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 費用項目主檔對話框 -->
    <el-dialog
      v-model="itemDialogVisible"
      :title="itemDialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="itemFormRef"
        :model="itemFormData"
        :rules="itemFormRules"
        label-width="140px"
      >
        <el-form-item label="費用項目編號" prop="FeeItemId">
          <el-input v-model="itemFormData.FeeItemId" :disabled="itemIsEdit" placeholder="請輸入費用項目編號" />
        </el-form-item>
        <el-form-item label="費用項目名稱" prop="FeeItemName">
          <el-input v-model="itemFormData.FeeItemName" placeholder="請輸入費用項目名稱" />
        </el-form-item>
        <el-form-item label="費用類型" prop="FeeType">
          <el-select v-model="itemFormData.FeeType" placeholder="請選擇費用類型" style="width: 100%">
            <el-option label="租金" value="RENT" />
            <el-option label="管理費" value="MANAGEMENT" />
            <el-option label="水電費" value="UTILITY" />
            <el-option label="其他費用" value="OTHER" />
          </el-select>
        </el-form-item>
        <el-form-item label="預設金額" prop="DefaultAmount">
          <el-input-number
            v-model="itemFormData.DefaultAmount"
            :min="0"
            :precision="2"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="狀態" prop="Status">
          <el-select v-model="itemFormData.Status" placeholder="請選擇狀態" style="width: 100%">
            <el-option label="啟用" value="A" />
            <el-option label="停用" value="I" />
          </el-select>
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="itemFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="itemDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleItemSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { leaseSYSEApi } from '@/api/leaseSYSE'

export default {
  name: 'LeaseSYSEFeeManagement',
  setup() {
    const activeTab = ref('fees')
    
    // 費用主檔相關
    const feeLoading = ref(false)
    const feeDialogVisible = ref(false)
    const feeIsEdit = ref(false)
    const feeFormRef = ref(null)
    const feeTableData = ref([])
    const feeItemList = ref([])
    
    const feeQueryForm = reactive({
      FeeId: '',
      LeaseId: '',
      FeeType: '',
      Status: '',
      FeeDateFrom: '',
      FeeDateTo: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const feePagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const feeFormData = reactive({
      FeeId: '',
      LeaseId: '',
      FeeType: '',
      FeeItemId: '',
      FeeItemName: '',
      FeeAmount: 0,
      FeeDate: '',
      DueDate: '',
      PaidDate: '',
      PaidAmount: 0,
      Status: 'P',
      CurrencyId: 'TWD',
      ExchangeRate: 1,
      TaxRate: 0,
      TaxAmount: 0,
      TotalAmount: 0,
      Memo: ''
    })
    
    const feeFormRules = {
      FeeId: [{ required: true, message: '請輸入費用編號', trigger: 'blur' }],
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      FeeType: [{ required: true, message: '請選擇費用類型', trigger: 'change' }],
      FeeDate: [{ required: true, message: '請選擇費用日期', trigger: 'change' }]
    }
    
    const feeDialogTitle = computed(() => {
      return feeIsEdit.value ? '修改費用' : '新增費用'
    })
    
    // 費用項目主檔相關
    const itemLoading = ref(false)
    const itemDialogVisible = ref(false)
    const itemIsEdit = ref(false)
    const itemFormRef = ref(null)
    const itemTableData = ref([])
    
    const itemQueryForm = reactive({
      FeeItemId: '',
      FeeItemName: '',
      FeeType: '',
      Status: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const itemPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const itemFormData = reactive({
      FeeItemId: '',
      FeeItemName: '',
      FeeType: '',
      DefaultAmount: 0,
      Status: 'A',
      Memo: ''
    })
    
    const itemFormRules = {
      FeeItemId: [{ required: true, message: '請輸入費用項目編號', trigger: 'blur' }],
      FeeItemName: [{ required: true, message: '請輸入費用項目名稱', trigger: 'blur' }],
      FeeType: [{ required: true, message: '請選擇費用類型', trigger: 'change' }]
    }
    
    const itemDialogTitle = computed(() => {
      return itemIsEdit.value ? '修改費用項目' : '新增費用項目'
    })
    
    // 費用主檔方法
    const loadFeeData = async () => {
      feeLoading.value = true
      try {
        const params = {
          ...feeQueryForm,
          PageIndex: feePagination.PageIndex,
          PageSize: feePagination.PageSize
        }
        const response = await leaseSYSEApi.getLeaseFees(params)
        if (response.data && response.data.Success) {
          feeTableData.value = response.data.Data.Items || []
          feePagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢費用列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        feeLoading.value = false
      }
    }
    
    const loadFeeItemList = async () => {
      try {
        const response = await leaseSYSEApi.getLeaseFeeItems({ Status: 'A', PageSize: 1000 })
        if (response.data && response.data.Success) {
          feeItemList.value = response.data.Data.Items || []
        }
      } catch (error) {
        console.error('查詢費用項目列表失敗:', error)
      }
    }
    
    const handleFeeSearch = () => {
      feePagination.PageIndex = 1
      loadFeeData()
    }
    
    const handleFeeReset = () => {
      Object.assign(feeQueryForm, {
        FeeId: '',
        LeaseId: '',
        FeeType: '',
        Status: '',
        FeeDateFrom: '',
        FeeDateTo: ''
      })
      handleFeeSearch()
    }
    
    const handleFeeCreate = () => {
      feeIsEdit.value = false
      feeDialogVisible.value = true
      Object.assign(feeFormData, {
        FeeId: '',
        LeaseId: '',
        FeeType: '',
        FeeItemId: '',
        FeeItemName: '',
        FeeAmount: 0,
        FeeDate: '',
        DueDate: '',
        PaidDate: '',
        PaidAmount: 0,
        Status: 'P',
        CurrencyId: 'TWD',
        ExchangeRate: 1,
        TaxRate: 0,
        TaxAmount: 0,
        TotalAmount: 0,
        Memo: ''
      })
    }
    
    const handleFeeView = async (row) => {
      try {
        const response = await leaseSYSEApi.getLeaseFee(row.FeeId)
        if (response.data && response.data.Success) {
          Object.assign(feeFormData, response.data.Data)
          feeIsEdit.value = true
          feeDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢費用失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleFeeEdit = async (row) => {
      await handleFeeView(row)
    }
    
    const handleFeeDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除費用「${row.FeeId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSEApi.deleteLeaseFee(row.FeeId)
        ElMessage.success('刪除成功')
        loadFeeData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除費用失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleFeeItemChange = (feeItemId) => {
      const item = feeItemList.value.find(i => i.FeeItemId === feeItemId)
      if (item) {
        feeFormData.FeeItemName = item.FeeItemName
        if (item.DefaultAmount > 0) {
          feeFormData.FeeAmount = item.DefaultAmount
          calculateFeeTotal()
        }
      }
    }
    
    const calculateFeeTotal = () => {
      const feeAmount = feeFormData.FeeAmount || 0
      const taxRate = feeFormData.TaxRate || 0
      feeFormData.TaxAmount = (feeAmount * taxRate) / 100
      feeFormData.TotalAmount = feeAmount + feeFormData.TaxAmount
    }
    
    const handleFeeSubmit = async () => {
      if (!feeFormRef.value) return
      try {
        await feeFormRef.value.validate()
        if (feeIsEdit.value) {
          await leaseSYSEApi.updateLeaseFee(feeFormData.FeeId, feeFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSEApi.createLeaseFee(feeFormData)
          ElMessage.success('新增成功')
        }
        feeDialogVisible.value = false
        loadFeeData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((feeIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleFeeSizeChange = (size) => {
      feePagination.PageSize = size
      feePagination.PageIndex = 1
      loadFeeData()
    }
    
    const handleFeePageChange = (page) => {
      feePagination.PageIndex = page
      loadFeeData()
    }
    
    // 費用項目主檔方法
    const loadItemData = async () => {
      itemLoading.value = true
      try {
        const params = {
          ...itemQueryForm,
          PageIndex: itemPagination.PageIndex,
          PageSize: itemPagination.PageSize
        }
        const response = await leaseSYSEApi.getLeaseFeeItems(params)
        if (response.data && response.data.Success) {
          itemTableData.value = response.data.Data.Items || []
          itemPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢費用項目列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        itemLoading.value = false
      }
    }
    
    const handleItemSearch = () => {
      itemPagination.PageIndex = 1
      loadItemData()
    }
    
    const handleItemReset = () => {
      Object.assign(itemQueryForm, {
        FeeItemId: '',
        FeeItemName: '',
        FeeType: '',
        Status: ''
      })
      handleItemSearch()
    }
    
    const handleItemCreate = () => {
      itemIsEdit.value = false
      itemDialogVisible.value = true
      Object.assign(itemFormData, {
        FeeItemId: '',
        FeeItemName: '',
        FeeType: '',
        DefaultAmount: 0,
        Status: 'A',
        Memo: ''
      })
    }
    
    const handleItemView = async (row) => {
      try {
        const response = await leaseSYSEApi.getLeaseFeeItem(row.FeeItemId)
        if (response.data && response.data.Success) {
          Object.assign(itemFormData, response.data.Data)
          itemIsEdit.value = true
          itemDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢費用項目失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleItemEdit = async (row) => {
      await handleItemView(row)
    }
    
    const handleItemDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除費用項目「${row.FeeItemName}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSEApi.deleteLeaseFeeItem(row.FeeItemId)
        ElMessage.success('刪除成功')
        loadItemData()
        loadFeeItemList()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除費用項目失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleItemSubmit = async () => {
      if (!itemFormRef.value) return
      try {
        await itemFormRef.value.validate()
        if (itemIsEdit.value) {
          await leaseSYSEApi.updateLeaseFeeItem(itemFormData.FeeItemId, itemFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSEApi.createLeaseFeeItem(itemFormData)
          ElMessage.success('新增成功')
        }
        itemDialogVisible.value = false
        loadItemData()
        loadFeeItemList()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((itemIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleItemSizeChange = (size) => {
      itemPagination.PageSize = size
      itemPagination.PageIndex = 1
      loadItemData()
    }
    
    const handleItemPageChange = (page) => {
      itemPagination.PageIndex = page
      loadItemData()
    }
    
    // 格式化方法
    const formatCurrency = (value) => {
      if (!value) return '0.00'
      return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }
    
    const getFeeTypeText = (type) => {
      const map = {
        'RENT': '租金',
        'MANAGEMENT': '管理費',
        'UTILITY': '水電費',
        'OTHER': '其他費用'
      }
      return map[type] || type
    }
    
    const getFeeTypeTagType = (type) => {
      const map = {
        'RENT': 'primary',
        'MANAGEMENT': 'success',
        'UTILITY': 'warning',
        'OTHER': 'info'
      }
      return map[type] || 'info'
    }
    
    const getStatusText = (status) => {
      const map = {
        'P': '待繳',
        'F': '已繳',
        'C': '已取消'
      }
      return map[status] || status
    }
    
    const getStatusTagType = (status) => {
      const map = {
        'P': 'warning',
        'F': 'success',
        'C': 'danger'
      }
      return map[status] || 'info'
    }
    
    // 初始化
    onMounted(() => {
      loadFeeData()
      loadItemData()
      loadFeeItemList()
    })
    
    return {
      activeTab,
      // 費用主檔
      feeLoading,
      feeDialogVisible,
      feeIsEdit,
      feeFormRef,
      feeTableData,
      feeItemList,
      feeQueryForm,
      feePagination,
      feeFormData,
      feeFormRules,
      feeDialogTitle,
      handleFeeSearch,
      handleFeeReset,
      handleFeeCreate,
      handleFeeView,
      handleFeeEdit,
      handleFeeDelete,
      handleFeeItemChange,
      calculateFeeTotal,
      handleFeeSubmit,
      handleFeeSizeChange,
      handleFeePageChange,
      // 費用項目主檔
      itemLoading,
      itemDialogVisible,
      itemIsEdit,
      itemFormRef,
      itemTableData,
      itemQueryForm,
      itemPagination,
      itemFormData,
      itemFormRules,
      itemDialogTitle,
      handleItemSearch,
      handleItemReset,
      handleItemCreate,
      handleItemView,
      handleItemEdit,
      handleItemDelete,
      handleItemSubmit,
      handleItemSizeChange,
      handleItemPageChange,
      // 格式化方法
      formatCurrency,
      getFeeTypeText,
      getFeeTypeTagType,
      getStatusText,
      getStatusTagType
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-syse-fee-management {
  .page-header {
    margin-bottom: 20px;
    
    h1 {
      font-size: 24px;
      font-weight: 500;
      color: $text-color-primary;
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

