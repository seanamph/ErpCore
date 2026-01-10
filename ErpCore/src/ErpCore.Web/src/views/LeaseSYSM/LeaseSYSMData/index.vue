<template>
  <div class="lease-sysm-data-management">
    <div class="page-header">
      <h1>租賃資料維護 (SYSM111-SYSM138)</h1>
    </div>

    <el-tabs v-model="activeTab" type="card">
      <!-- 停車位資料 Tab -->
      <el-tab-pane label="停車位資料" name="parking">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="parkingQueryForm" class="search-form">
            <el-form-item label="停車位代碼">
              <el-input v-model="parkingQueryForm.ParkingSpaceId" placeholder="請輸入停車位代碼" clearable />
            </el-form-item>
            <el-form-item label="分店代碼">
              <el-input v-model="parkingQueryForm.ShopId" placeholder="請輸入分店代碼" clearable />
            </el-form-item>
            <el-form-item label="樓層代碼">
              <el-input v-model="parkingQueryForm.FloorId" placeholder="請輸入樓層代碼" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="parkingQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="可用" value="A" />
                <el-option label="使用中" value="U" />
                <el-option label="維護中" value="M" />
              </el-select>
            </el-form-item>
            <el-form-item label="租賃編號">
              <el-input v-model="parkingQueryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleParkingSearch">查詢</el-button>
              <el-button @click="handleParkingReset">重置</el-button>
              <el-button type="success" @click="handleParkingCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="parkingTableData"
            v-loading="parkingLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ParkingSpaceId" label="停車位代碼" width="150" />
            <el-table-column prop="ParkingSpaceNo" label="停車位編號" width="150" />
            <el-table-column prop="ShopId" label="分店代碼" width="120" />
            <el-table-column prop="FloorId" label="樓層代碼" width="120" />
            <el-table-column prop="Area" label="面積" width="100">
              <template #default="{ row }">
                {{ row.Area ? formatNumber(row.Area) : '-' }}
              </template>
            </el-table-column>
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="getParkingStatusTagType(row.Status)">
                  {{ getParkingStatusText(row.Status) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="LeaseId" label="租賃編號" width="150" />
            <el-table-column prop="Memo" label="備註" min-width="200" show-overflow-tooltip />
            <el-table-column label="操作" width="250" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleParkingView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleParkingEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleParkingDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="parkingPagination.PageIndex"
            v-model:page-size="parkingPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="parkingPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleParkingSizeChange"
            @current-change="handleParkingPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>

      <!-- 租賃合同資料 Tab -->
      <el-tab-pane label="租賃合同資料" name="contract">
        <el-card class="search-card" shadow="never">
          <el-form :inline="true" :model="contractQueryForm" class="search-form">
            <el-form-item label="合同編號">
              <el-input v-model="contractQueryForm.ContractNo" placeholder="請輸入合同編號" clearable />
            </el-form-item>
            <el-form-item label="租賃編號">
              <el-input v-model="contractQueryForm.LeaseId" placeholder="請輸入租賃編號" clearable />
            </el-form-item>
            <el-form-item label="合同類型">
              <el-input v-model="contractQueryForm.ContractType" placeholder="請輸入合同類型" clearable />
            </el-form-item>
            <el-form-item label="狀態">
              <el-select v-model="contractQueryForm.Status" placeholder="請選擇狀態" clearable>
                <el-option label="有效" value="A" />
                <el-option label="無效" value="I" />
              </el-select>
            </el-form-item>
            <el-form-item label="合同日期">
              <el-date-picker
                v-model="contractQueryForm.ContractDateFrom"
                type="date"
                placeholder="開始日期"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item label="至">
              <el-date-picker
                v-model="contractQueryForm.ContractDateTo"
                type="date"
                placeholder="結束日期"
                value-format="YYYY-MM-DD"
                clearable
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleContractSearch">查詢</el-button>
              <el-button @click="handleContractReset">重置</el-button>
              <el-button type="success" @click="handleContractCreate">新增</el-button>
            </el-form-item>
          </el-form>
        </el-card>

        <el-card class="table-card" shadow="never">
          <el-table
            :data="contractTableData"
            v-loading="contractLoading"
            border
            stripe
            style="width: 100%"
          >
            <el-table-column prop="ContractNo" label="合同編號" width="150" />
            <el-table-column prop="LeaseId" label="租賃編號" width="150" />
            <el-table-column prop="ContractDate" label="合同日期" width="120" />
            <el-table-column prop="ContractType" label="合同類型" width="150" />
            <el-table-column prop="Status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag :type="row.Status === 'A' ? 'success' : 'danger'">
                  {{ row.Status === 'A' ? '有效' : '無效' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="SignedBy" label="簽約人" width="120" />
            <el-table-column prop="SignedDate" label="簽約日期" width="120" />
            <el-table-column prop="ContractContent" label="合同內容" min-width="200" show-overflow-tooltip />
            <el-table-column label="操作" width="250" fixed="right">
              <template #default="{ row }">
                <el-button type="primary" size="small" @click="handleContractView(row)">查看</el-button>
                <el-button type="warning" size="small" @click="handleContractEdit(row)">修改</el-button>
                <el-button type="danger" size="small" @click="handleContractDelete(row)">刪除</el-button>
              </template>
            </el-table-column>
          </el-table>

          <el-pagination
            v-model:current-page="contractPagination.PageIndex"
            v-model:page-size="contractPagination.PageSize"
            :page-sizes="[10, 20, 50, 100]"
            :total="contractPagination.TotalCount"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleContractSizeChange"
            @current-change="handleContractPageChange"
            style="margin-top: 20px; text-align: right;"
          />
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <!-- 停車位資料對話框 -->
    <el-dialog
      v-model="parkingDialogVisible"
      :title="parkingDialogTitle"
      width="800px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="parkingFormRef"
        :model="parkingFormData"
        :rules="parkingFormRules"
        label-width="140px"
      >
        <el-form-item label="停車位代碼" prop="ParkingSpaceId">
          <el-input v-model="parkingFormData.ParkingSpaceId" :disabled="parkingIsEdit" placeholder="請輸入停車位代碼" />
        </el-form-item>
        <el-form-item label="停車位編號" prop="ParkingSpaceNo">
          <el-input v-model="parkingFormData.ParkingSpaceNo" placeholder="請輸入停車位編號" />
        </el-form-item>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="分店代碼" prop="ShopId">
              <el-input v-model="parkingFormData.ShopId" placeholder="請輸入分店代碼" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="樓層代碼" prop="FloorId">
              <el-input v-model="parkingFormData.FloorId" placeholder="請輸入樓層代碼" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="面積" prop="Area">
              <el-input-number
                v-model="parkingFormData.Area"
                :min="0"
                :precision="2"
                style="width: 100%"
                placeholder="請輸入面積"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="parkingFormData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="可用" value="A" />
                <el-option label="使用中" value="U" />
                <el-option label="維護中" value="M" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="租賃編號" prop="LeaseId">
          <el-input v-model="parkingFormData.LeaseId" placeholder="請輸入租賃編號" />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="parkingFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="parkingDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleParkingSubmit">確定</el-button>
      </template>
    </el-dialog>

    <!-- 租賃合同資料對話框 -->
    <el-dialog
      v-model="contractDialogVisible"
      :title="contractDialogTitle"
      width="900px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="contractFormRef"
        :model="contractFormData"
        :rules="contractFormRules"
        label-width="140px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="合同編號" prop="ContractNo">
              <el-input v-model="contractFormData.ContractNo" :disabled="contractIsEdit" placeholder="請輸入合同編號" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="租賃編號" prop="LeaseId">
              <el-input v-model="contractFormData.LeaseId" placeholder="請輸入租賃編號" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="合同日期" prop="ContractDate">
              <el-date-picker
                v-model="contractFormData.ContractDate"
                type="date"
                placeholder="請選擇合同日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="合同類型" prop="ContractType">
              <el-input v-model="contractFormData.ContractType" placeholder="請輸入合同類型" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="狀態" prop="Status">
              <el-select v-model="contractFormData.Status" placeholder="請選擇狀態" style="width: 100%">
                <el-option label="有效" value="A" />
                <el-option label="無效" value="I" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="簽約人" prop="SignedBy">
              <el-input v-model="contractFormData.SignedBy" placeholder="請輸入簽約人" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="簽約日期" prop="SignedDate">
              <el-date-picker
                v-model="contractFormData.SignedDate"
                type="date"
                placeholder="請選擇簽約日期"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="合同內容" prop="ContractContent">
          <el-input v-model="contractFormData.ContractContent" type="textarea" :rows="5" placeholder="請輸入合同內容" />
        </el-form-item>
        <el-form-item label="備註" prop="Memo">
          <el-input v-model="contractFormData.Memo" type="textarea" :rows="3" placeholder="請輸入備註" />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="contractDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleContractSubmit">確定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { leaseSYSMApi } from '@/api/leaseSYSM'

export default {
  name: 'LeaseSYSMDataManagement',
  setup() {
    const activeTab = ref('parking')
    
    // 停車位資料相關
    const parkingLoading = ref(false)
    const parkingDialogVisible = ref(false)
    const parkingIsEdit = ref(false)
    const parkingFormRef = ref(null)
    const parkingTableData = ref([])
    
    const parkingQueryForm = reactive({
      ParkingSpaceId: '',
      ShopId: '',
      FloorId: '',
      Status: '',
      LeaseId: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const parkingPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const parkingFormData = reactive({
      ParkingSpaceId: '',
      ParkingSpaceNo: '',
      ShopId: '',
      FloorId: '',
      Area: null,
      Status: 'A',
      LeaseId: '',
      Memo: ''
    })
    
    const parkingFormRules = {
      ParkingSpaceId: [{ required: true, message: '請輸入停車位代碼', trigger: 'blur' }]
    }
    
    const parkingDialogTitle = computed(() => {
      return parkingIsEdit.value ? '修改停車位' : '新增停車位'
    })
    
    // 租賃合同資料相關
    const contractLoading = ref(false)
    const contractDialogVisible = ref(false)
    const contractIsEdit = ref(false)
    const contractFormRef = ref(null)
    const contractTableData = ref([])
    
    const contractQueryForm = reactive({
      ContractNo: '',
      LeaseId: '',
      ContractType: '',
      Status: '',
      ContractDateFrom: '',
      ContractDateTo: '',
      PageIndex: 1,
      PageSize: 20
    })
    
    const contractPagination = reactive({
      PageIndex: 1,
      PageSize: 20,
      TotalCount: 0
    })
    
    const contractFormData = reactive({
      ContractNo: '',
      LeaseId: '',
      ContractDate: '',
      ContractType: '',
      ContractContent: '',
      Status: 'A',
      SignedBy: '',
      SignedDate: '',
      Memo: ''
    })
    
    const contractFormRules = {
      ContractNo: [{ required: true, message: '請輸入合同編號', trigger: 'blur' }],
      LeaseId: [{ required: true, message: '請輸入租賃編號', trigger: 'blur' }],
      ContractDate: [{ required: true, message: '請選擇合同日期', trigger: 'change' }]
    }
    
    const contractDialogTitle = computed(() => {
      return contractIsEdit.value ? '修改租賃合同' : '新增租賃合同'
    })
    
    // 停車位資料方法
    const loadParkingData = async () => {
      parkingLoading.value = true
      try {
        const params = {
          ...parkingQueryForm,
          PageIndex: parkingPagination.PageIndex,
          PageSize: parkingPagination.PageSize
        }
        const response = await leaseSYSMApi.getParkingSpaces(params)
        if (response.data && response.data.Success) {
          parkingTableData.value = response.data.Data.Items || []
          parkingPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢停車位列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        parkingLoading.value = false
      }
    }
    
    const handleParkingSearch = () => {
      parkingPagination.PageIndex = 1
      loadParkingData()
    }
    
    const handleParkingReset = () => {
      Object.assign(parkingQueryForm, {
        ParkingSpaceId: '',
        ShopId: '',
        FloorId: '',
        Status: '',
        LeaseId: ''
      })
      handleParkingSearch()
    }
    
    const handleParkingCreate = () => {
      parkingIsEdit.value = false
      parkingDialogVisible.value = true
      Object.assign(parkingFormData, {
        ParkingSpaceId: '',
        ParkingSpaceNo: '',
        ShopId: '',
        FloorId: '',
        Area: null,
        Status: 'A',
        LeaseId: '',
        Memo: ''
      })
    }
    
    const handleParkingView = async (row) => {
      try {
        const response = await leaseSYSMApi.getParkingSpace(row.ParkingSpaceId)
        if (response.data && response.data.Success) {
          Object.assign(parkingFormData, response.data.Data)
          parkingIsEdit.value = true
          parkingDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢停車位失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleParkingEdit = async (row) => {
      await handleParkingView(row)
    }
    
    const handleParkingDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除停車位「${row.ParkingSpaceId}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSMApi.deleteParkingSpace(row.ParkingSpaceId)
        ElMessage.success('刪除成功')
        loadParkingData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除停車位失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleParkingSubmit = async () => {
      if (!parkingFormRef.value) return
      try {
        await parkingFormRef.value.validate()
        if (parkingIsEdit.value) {
          await leaseSYSMApi.updateParkingSpace(parkingFormData.ParkingSpaceId, parkingFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSMApi.createParkingSpace(parkingFormData)
          ElMessage.success('新增成功')
        }
        parkingDialogVisible.value = false
        loadParkingData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((parkingIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleParkingSizeChange = (size) => {
      parkingPagination.PageSize = size
      parkingPagination.PageIndex = 1
      loadParkingData()
    }
    
    const handleParkingPageChange = (page) => {
      parkingPagination.PageIndex = page
      loadParkingData()
    }
    
    // 租賃合同資料方法
    const loadContractData = async () => {
      contractLoading.value = true
      try {
        const params = {
          ...contractQueryForm,
          PageIndex: contractPagination.PageIndex,
          PageSize: contractPagination.PageSize
        }
        const response = await leaseSYSMApi.getLeaseContracts(params)
        if (response.data && response.data.Success) {
          contractTableData.value = response.data.Data.Items || []
          contractPagination.TotalCount = response.data.Data.TotalCount || 0
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃合同列表失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      } finally {
        contractLoading.value = false
      }
    }
    
    const handleContractSearch = () => {
      contractPagination.PageIndex = 1
      loadContractData()
    }
    
    const handleContractReset = () => {
      Object.assign(contractQueryForm, {
        ContractNo: '',
        LeaseId: '',
        ContractType: '',
        Status: '',
        ContractDateFrom: '',
        ContractDateTo: ''
      })
      handleContractSearch()
    }
    
    const handleContractCreate = () => {
      contractIsEdit.value = false
      contractDialogVisible.value = true
      Object.assign(contractFormData, {
        ContractNo: '',
        LeaseId: '',
        ContractDate: '',
        ContractType: '',
        ContractContent: '',
        Status: 'A',
        SignedBy: '',
        SignedDate: '',
        Memo: ''
      })
    }
    
    const handleContractView = async (row) => {
      try {
        const response = await leaseSYSMApi.getLeaseContract(row.ContractNo)
        if (response.data && response.data.Success) {
          Object.assign(contractFormData, response.data.Data)
          contractIsEdit.value = true
          contractDialogVisible.value = true
        } else {
          ElMessage.error(response.data?.Message || '查詢失敗')
        }
      } catch (error) {
        console.error('查詢租賃合同失敗:', error)
        ElMessage.error('查詢失敗: ' + (error.message || '未知錯誤'))
      }
    }
    
    const handleContractEdit = async (row) => {
      await handleContractView(row)
    }
    
    const handleContractDelete = async (row) => {
      try {
        await ElMessageBox.confirm(
          `確定要刪除租賃合同「${row.ContractNo}」嗎？`,
          '確認刪除',
          {
            confirmButtonText: '確定',
            cancelButtonText: '取消',
            type: 'warning'
          }
        )
        await leaseSYSMApi.deleteLeaseContract(row.ContractNo)
        ElMessage.success('刪除成功')
        loadContractData()
      } catch (error) {
        if (error !== 'cancel') {
          console.error('刪除租賃合同失敗:', error)
          ElMessage.error('刪除失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleContractSubmit = async () => {
      if (!contractFormRef.value) return
      try {
        await contractFormRef.value.validate()
        if (contractIsEdit.value) {
          await leaseSYSMApi.updateLeaseContract(contractFormData.ContractNo, contractFormData)
          ElMessage.success('修改成功')
        } else {
          await leaseSYSMApi.createLeaseContract(contractFormData)
          ElMessage.success('新增成功')
        }
        contractDialogVisible.value = false
        loadContractData()
      } catch (error) {
        if (error !== false) {
          console.error('提交失敗:', error)
          ElMessage.error((contractIsEdit.value ? '修改' : '新增') + '失敗: ' + (error.message || '未知錯誤'))
        }
      }
    }
    
    const handleContractSizeChange = (size) => {
      contractPagination.PageSize = size
      contractPagination.PageIndex = 1
      loadContractData()
    }
    
    const handleContractPageChange = (page) => {
      contractPagination.PageIndex = page
      loadContractData()
    }
    
    // 格式化方法
    const formatNumber = (value) => {
      if (!value) return '0'
      return Number(value).toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    }
    
    const getParkingStatusText = (status) => {
      const map = {
        'A': '可用',
        'U': '使用中',
        'M': '維護中'
      }
      return map[status] || status
    }
    
    const getParkingStatusTagType = (status) => {
      const map = {
        'A': 'success',
        'U': 'warning',
        'M': 'danger'
      }
      return map[status] || 'info'
    }
    
    // 初始化
    onMounted(() => {
      loadParkingData()
      loadContractData()
    })
    
    return {
      activeTab,
      // 停車位資料
      parkingLoading,
      parkingDialogVisible,
      parkingIsEdit,
      parkingFormRef,
      parkingTableData,
      parkingQueryForm,
      parkingPagination,
      parkingFormData,
      parkingFormRules,
      parkingDialogTitle,
      handleParkingSearch,
      handleParkingReset,
      handleParkingCreate,
      handleParkingView,
      handleParkingEdit,
      handleParkingDelete,
      handleParkingSubmit,
      handleParkingSizeChange,
      handleParkingPageChange,
      // 租賃合同資料
      contractLoading,
      contractDialogVisible,
      contractIsEdit,
      contractFormRef,
      contractTableData,
      contractQueryForm,
      contractPagination,
      contractFormData,
      contractFormRules,
      contractDialogTitle,
      handleContractSearch,
      handleContractReset,
      handleContractCreate,
      handleContractView,
      handleContractEdit,
      handleContractDelete,
      handleContractSubmit,
      handleContractSizeChange,
      handleContractPageChange,
      // 格式化方法
      formatNumber,
      getParkingStatusText,
      getParkingStatusTagType
    }
  }
}
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.lease-sysm-data-management {
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

