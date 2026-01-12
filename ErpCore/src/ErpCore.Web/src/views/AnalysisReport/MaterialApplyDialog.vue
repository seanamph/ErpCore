<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? '修改單位領用申請單' : '新增單位領用申請單'"
    width="1400px"
    @update:model-value="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="領用單號" prop="applyId">
            <el-input v-model="form.applyId" :disabled="isEdit" placeholder="請輸入領用單號" />
            <el-button type="primary" size="small" @click="generateApplyId" v-if="!isEdit" style="margin-left: 10px">產生</el-button>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="申請日期" prop="applyDate">
            <el-date-picker
              v-model="form.applyDate"
              type="date"
              placeholder="請選擇申請日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="申請人" prop="empId">
            <el-input v-model="form.empId" placeholder="請輸入申請人代號" @blur="loadEmployeeInfo" />
            <el-button type="primary" size="small" @click="showEmployeeDialog = true" style="margin-left: 10px">選擇</el-button>
            <el-input v-model="form.empName" readonly style="margin-top: 10px" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="部門" prop="orgId">
            <el-input v-model="form.orgId" placeholder="請輸入部門代號" @blur="loadOrganizationInfo" />
            <el-button type="primary" size="small" @click="showOrganizationDialog = true" style="margin-left: 10px">選擇</el-button>
            <el-input v-model="form.orgName" readonly style="margin-top: 10px" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="分店" prop="siteId">
            <el-select v-model="form.siteId" placeholder="請選擇分店" style="width: 100%">
              <el-option
                v-for="site in siteList"
                :key="site.value"
                :label="site.label"
                :value="site.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="倉別">
            <el-input v-model="form.whId" placeholder="請輸入倉別" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="儲位">
            <el-input v-model="form.storeId" placeholder="請輸入儲位" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-divider>明細資料</el-divider>
      
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column label="品項編號" width="150">
          <template #default="{ row, $index }">
            <el-input v-model="row.goodsId" placeholder="請輸入品項編號" @blur="handleGoodsChange($index)" />
            <el-button type="primary" size="small" @click="showGoodsDialog($index)" style="margin-top: 5px">選擇</el-button>
          </template>
        </el-table-column>
        <el-table-column label="品項名稱" width="200">
          <template #default="{ row }">
            <el-input v-model="row.goodsName" readonly />
          </template>
        </el-table-column>
        <el-table-column label="單位" width="100">
          <template #default="{ row }">
            <el-input v-model="row.unit" readonly />
          </template>
        </el-table-column>
        <el-table-column label="申請數量" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.applyQty" :min="0" :precision="3" @change="calculateAmount($index)" style="width: 100%" />
          </template>
        </el-table-column>
        <el-table-column label="單價" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.unitPrice" :min="0" :precision="2" @change="calculateAmount($index)" style="width: 100%" />
          </template>
        </el-table-column>
        <el-table-column label="金額" width="120" align="right">
          <template #default="{ row }">
            <el-input-number v-model="row.amount" :min="0" :precision="2" readonly style="width: 100%" />
          </template>
        </el-table-column>
        <el-table-column label="附註" width="200">
          <template #default="{ row }">
            <el-input v-model="row.notes" placeholder="請輸入附註" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="removeDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-button type="primary" @click="addDetail" style="margin-top: 10px">新增明細</el-button>
      
      <el-form-item label="總價值">
        <el-input-number v-model="form.amount" :min="0" :precision="2" readonly style="width: 200px" />
      </el-form-item>
      
      <el-form-item label="備註">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>

    <!-- 員工選擇對話框 -->
    <MultiUserListDialog
      v-model="showEmployeeDialog"
      :multiple="false"
      @confirm="handleEmployeeSelect"
    />

    <!-- 部門選擇對話框（暫時使用多選用戶對話框的邏輯，或創建專門的部門選擇組件） -->
    <!-- 這裡可以根據實際情況使用部門選擇組件 -->
  </el-dialog>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { materialApplyApi } from '@/api/materialApply'
import { dropdownListApi } from '@/api/dropdownList'
import { getProductById } from '@/api/modules/product'
import { getOrganizations } from '@/api/users'
import MultiUserListDialog from '@/components/MultiUserListDialog.vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  applyData: {
    type: Object,
    default: null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'success'])

const formRef = ref(null)
const siteList = ref([])
const showEmployeeDialog = ref(false)
const showOrganizationDialog = ref(false)
const currentGoodsIndex = ref(-1)

const form = reactive({
  applyId: '',
  empId: '',
  empName: '',
  orgId: '',
  orgName: '',
  siteId: '',
  applyDate: '',
  whId: '',
  storeId: '',
  notes: '',
  amount: 0,
  details: []
})

const rules = {
  applyId: [{ required: true, message: '請輸入領用單號', trigger: 'blur' }],
  empId: [{ required: true, message: '請輸入申請人代號', trigger: 'blur' }],
  orgId: [{ required: true, message: '請輸入部門代號', trigger: 'blur' }],
  siteId: [{ required: true, message: '請選擇分店', trigger: 'change' }],
  applyDate: [{ required: true, message: '請選擇申請日期', trigger: 'change' }]
}

watch(() => props.applyData, (newVal) => {
  if (newVal) {
    form.applyId = newVal.applyId || ''
    form.empId = newVal.empId || ''
    form.empName = newVal.empName || ''
    form.orgId = newVal.orgId || ''
    form.orgName = newVal.orgName || ''
    form.siteId = newVal.siteId || ''
    form.applyDate = newVal.applyDate ? newVal.applyDate.split('T')[0] : ''
    form.whId = newVal.whId || ''
    form.storeId = newVal.storeId || ''
    form.notes = newVal.notes || ''
    form.amount = newVal.amount || 0
    form.details = (newVal.details || []).map((d, index) => ({
      tKey: d.tKey || 0,
      goodsTKey: d.goodsTKey || 0,
      goodsId: d.goodsId || '',
      goodsName: d.goodsName || '',
      applyQty: d.applyQty || 0,
      issueQty: d.issueQty || 0,
      unit: d.unit || '',
      unitPrice: d.unitPrice || 0,
      amount: d.amount || 0,
      notes: d.notes || '',
      seqNo: d.seqNo || index + 1
    }))
  } else {
    resetForm()
  }
}, { immediate: true })

const resetForm = () => {
  form.applyId = ''
  form.empId = ''
  form.empName = ''
  form.orgId = ''
  form.orgName = ''
  form.siteId = ''
  form.applyDate = ''
  form.whId = ''
  form.storeId = ''
  form.notes = ''
  form.amount = 0
  form.details = []
}

const handleClose = () => {
  emit('update:modelValue', false)
  resetForm()
}

const generateApplyId = async () => {
  try {
    const response = await materialApplyApi.generateApplyId()
    if (response.data.success) {
      form.applyId = response.data.data.applyId
    } else {
      ElMessage.error(response.data.message || '產生領用單號失敗')
    }
  } catch (error) {
    ElMessage.error('產生領用單號失敗: ' + (error.message || '未知錯誤'))
  }
}

const loadEmployeeInfo = async () => {
  if (!form.empId) return
  // 這裡可以調用 API 獲取員工資訊
  // 暫時留空，等待實際 API
}

const loadOrganizationInfo = async () => {
  if (!form.orgId) return
  try {
    const response = await getOrganizations()
    if (response.data && response.data.success) {
      const org = response.data.data.find(o => o.orgId === form.orgId)
      if (org) {
        form.orgName = org.orgName || ''
      }
    }
  } catch (error) {
    console.error('載入部門資訊失敗:', error)
  }
}

const handleEmployeeSelect = (users) => {
  if (users && users.length > 0) {
    const user = users[0]
    form.empId = user.UserId || user.userId || ''
    form.empName = user.UserName || user.userName || ''
  }
  showEmployeeDialog.value = false
}

const addDetail = () => {
  form.details.push({
    tKey: 0,
    goodsTKey: 0,
    goodsId: '',
    goodsName: '',
    applyQty: 0,
    issueQty: 0,
    unit: '',
    unitPrice: 0,
    amount: 0,
    notes: '',
    seqNo: form.details.length + 1
  })
}

const removeDetail = (index) => {
  form.details.splice(index, 1)
  calculateTotalAmount()
}

const showGoodsDialog = (index) => {
  currentGoodsIndex.value = index
  // 這裡可以打開商品選擇對話框
  // 暫時使用直接輸入的方式
  ElMessage.info('請直接輸入品項編號，或等待商品選擇對話框實現')
}

const handleGoodsChange = async (index) => {
  const detail = form.details[index]
  if (!detail.goodsId) return

  try {
    const response = await getProductById(detail.goodsId)
    if (response.data && response.data.success) {
      const goods = response.data.data
      detail.goodsTKey = goods.tKey || 0
      detail.goodsName = goods.goodsName || goods.GoodsName || ''
      detail.unit = goods.unit || goods.Unit || ''
      detail.unitPrice = goods.unitPrice || goods.UnitPrice || 0
      calculateAmount(index)
    } else {
      ElMessage.warning('找不到該品項')
      detail.goodsName = ''
      detail.unit = ''
      detail.unitPrice = 0
    }
  } catch (error) {
    console.error('查詢品項資訊失敗:', error)
    ElMessage.warning('查詢品項資訊失敗')
  }
}

const calculateAmount = (index) => {
  const detail = form.details[index]
  if (detail) {
    detail.amount = (detail.applyQty || 0) * (detail.unitPrice || 0)
    calculateTotalAmount()
  }
}

const calculateTotalAmount = () => {
  form.amount = form.details.reduce((sum, item) => sum + (item.amount || 0), 0)
}

const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    
    if (form.details.length === 0) {
      ElMessage.warning('請至少新增一筆明細')
      return
    }

    const data = {
      applyId: form.applyId,
      empId: form.empId,
      orgId: form.orgId,
      siteId: form.siteId,
      applyDate: form.applyDate,
      whId: form.whId,
      storeId: form.storeId,
      notes: form.notes,
      details: form.details.map(d => ({
        goodsTKey: d.goodsTKey,
        goodsId: d.goodsId,
        applyQty: d.applyQty,
        unitPrice: d.unitPrice,
        notes: d.notes,
        seqNo: d.seqNo
      }))
    }

    if (props.isEdit) {
      const response = await materialApplyApi.updateMaterialApply(form.applyId, data)
      if (response.data.success) {
        ElMessage.success('修改成功')
        emit('success')
      } else {
        ElMessage.error(response.data.message || '修改失敗')
      }
    } else {
      const response = await materialApplyApi.createMaterialApply(data)
      if (response.data.success) {
        ElMessage.success('新增成功')
        emit('success')
      } else {
        ElMessage.error(response.data.message || '新增失敗')
      }
    }
  } catch (error) {
    if (error !== false) {
      ElMessage.error('操作失敗: ' + (error.message || '未知錯誤'))
    }
  }
}

const loadSiteList = async () => {
  try {
    const response = await dropdownListApi.getShopOptions({})
    if (response.data && response.data.success && response.data.data) {
      siteList.value = response.data.data.map(item => ({
        value: item.value || item.ShopId || item.siteId,
        label: item.label || item.ShopName || item.siteName
      }))
    }
  } catch (error) {
    console.error('載入分店列表失敗:', error)
  }
}

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    loadSiteList()
    if (!props.applyData) {
      // 新增時設定預設日期為今天
      const today = new Date()
      form.applyDate = today.toISOString().split('T')[0]
    }
  }
}, { immediate: true })
</script>

<style scoped>
</style>
