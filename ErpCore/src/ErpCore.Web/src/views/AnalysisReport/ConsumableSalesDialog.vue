<template>
  <el-dialog
    :model-value="modelValue"
    :title="isEdit ? '修改耗材出售單' : '新增耗材出售單'"
    width="1200px"
    @update:model-value="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="店別" prop="siteId">
            <el-select v-model="form.siteId" placeholder="請選擇店別" :disabled="isEdit" style="width: 100%">
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
          <el-form-item label="出售日期" prop="purchaseDate">
            <el-date-picker
              v-model="form.purchaseDate"
              type="date"
              placeholder="請選擇日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              :disabled="isEdit"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
      <el-form-item label="明細">
        <el-table :data="form.details" border>
          <el-table-column type="index" label="序號" width="60" />
          <el-table-column label="耗材編號" width="150">
            <template #default="{ row, $index }">
              <el-select v-model="row.consumableId" placeholder="請選擇耗材" @change="handleConsumableChange($index)" style="width: 100%" filterable>
                <el-option
                  v-for="consumable in consumableList"
                  :key="consumable.consumableId"
                  :label="`${consumable.consumableId} - ${consumable.consumableName}`"
                  :value="consumable.consumableId"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="耗材名稱" width="200">
            <template #default="{ row }">
              {{ row.consumableName || '' }}
            </template>
          </el-table-column>
          <el-table-column label="數量" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.quantity" :min="0.01" :precision="2" @change="handleDetailChange($index)" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="單位" width="80">
            <template #default="{ row }">
              {{ row.unit || '' }}
            </template>
          </el-table-column>
          <el-table-column label="單價" width="120">
            <template #default="{ row, $index }">
              <el-input-number v-model="row.unitPrice" :min="0" :precision="2" @change="handleDetailChange($index)" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="稅別" width="100">
            <template #default="{ row, $index }">
              <el-select v-model="row.tax" @change="handleDetailChange($index)" style="width: 100%">
                <el-option label="應稅" value="1" />
                <el-option label="免稅" value="0" />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.amount) }}
            </template>
          </el-table-column>
          <el-table-column label="稅額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.taxAmount) }}
            </template>
          </el-table-column>
          <el-table-column label="未稅金額" width="120" align="right">
            <template #default="{ row }">
              {{ formatCurrency(row.netAmount) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="{ $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" @click="handleAddDetail" style="margin-top: 10px">新增明細</el-button>
      </el-form-item>
      <el-form-item label="合計">
        <el-row :gutter="20">
          <el-col :span="8">
            <span>總金額: <strong>{{ formatCurrency(totalAmount) }}</strong></span>
          </el-col>
          <el-col :span="8">
            <span>稅額: <strong>{{ formatCurrency(taxAmount) }}</strong></span>
          </el-col>
          <el-col :span="8">
            <span>未稅金額: <strong>{{ formatCurrency(netAmount) }}</strong></span>
          </el-col>
        </el-row>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { consumableSalesApi } from '@/api/consumableSales'
import { dropdownListApi } from '@/api/dropdownList'
import { consumablePrintApi } from '@/api/consumablePrint'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  salesData: {
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
const consumableList = ref([])

const form = reactive({
  siteId: '',
  purchaseDate: '',
  notes: '',
  details: []
})

const rules = {
  siteId: [{ required: true, message: '請選擇店別', trigger: 'change' }],
  purchaseDate: [{ required: true, message: '請選擇出售日期', trigger: 'change' }]
}

const totalAmount = computed(() => {
  return form.details.reduce((sum, item) => sum + (item.amount || 0), 0)
})

const taxAmount = computed(() => {
  return form.details.reduce((sum, item) => sum + (item.taxAmount || 0), 0)
})

const netAmount = computed(() => {
  return form.details.reduce((sum, item) => sum + (item.netAmount || 0), 0)
})

watch(() => props.salesData, (newVal) => {
  if (newVal) {
    form.siteId = newVal.siteId || ''
    form.purchaseDate = newVal.purchaseDate || ''
    form.notes = newVal.notes || ''
    form.details = (newVal.details || []).map(d => ({
      consumableId: d.consumableId || '',
      consumableName: d.consumableName || '',
      quantity: d.quantity || 0,
      unit: d.unit || '',
      unitPrice: d.unitPrice || 0,
      tax: d.tax || '1',
      amount: d.amount || 0,
      taxAmount: d.taxAmount || 0,
      netAmount: d.netAmount || 0,
      notes: d.notes || ''
    }))
  } else {
    resetForm()
  }
}, { immediate: true })

const resetForm = () => {
  form.siteId = ''
  form.purchaseDate = ''
  form.notes = ''
  form.details = []
}

const handleClose = () => {
  emit('update:modelValue', false)
  resetForm()
}

const handleAddDetail = () => {
  form.details.push({
    consumableId: '',
    consumableName: '',
    quantity: 0,
    unit: '',
    unitPrice: 0,
    tax: '1',
    amount: 0,
    taxAmount: 0,
    netAmount: 0,
    notes: ''
  })
}

const handleRemoveDetail = (index) => {
  form.details.splice(index, 1)
  form.details.forEach((item, idx) => {
    calculateDetailAmount(idx)
  })
}

const handleConsumableChange = async (index) => {
  const detail = form.details[index]
  if (!detail.consumableId) return

  // 從耗材列表中查找耗材資訊
  const consumable = consumableList.value.find(c => c.consumableId === detail.consumableId)
  if (consumable) {
    detail.consumableName = consumable.consumableName || ''
    detail.unit = consumable.unit || ''
    // 如果沒有設定單價，使用耗材的預設單價
    if (!detail.unitPrice && consumable.price) {
      detail.unitPrice = consumable.price
    }
  }
  calculateDetailAmount(index)
}

const handleDetailChange = (index) => {
  calculateDetailAmount(index)
}

const calculateDetailAmount = (index) => {
  const detail = form.details[index]
  if (!detail) return

  detail.amount = (detail.quantity || 0) * (detail.unitPrice || 0)
  detail.taxAmount = detail.tax === '1' ? detail.amount * 0.05 : 0 // 假設稅率為 5%
  detail.netAmount = detail.amount - detail.taxAmount
}

const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    
    if (form.details.length === 0) {
      ElMessage.warning('請至少新增一筆明細')
      return
    }

    const data = {
      siteId: form.siteId,
      purchaseDate: form.purchaseDate,
      notes: form.notes,
      details: form.details.map(d => ({
        consumableId: d.consumableId,
        quantity: d.quantity,
        unitPrice: d.unitPrice,
        tax: d.tax,
        notes: d.notes
      }))
    }

    if (props.isEdit) {
      const response = await consumableSalesApi.updateSales(props.salesData.txnNo, {
        notes: data.notes,
        details: data.details
      })
      if (response.data.success) {
        ElMessage.success('修改成功')
        emit('success')
      } else {
        ElMessage.error(response.data.message || '修改失敗')
      }
    } else {
      const response = await consumableSalesApi.createSales(data)
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

const formatCurrency = (amount) => {
  if (amount == null) return '0.00'
  return new Intl.NumberFormat('zh-TW', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
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
    console.error('載入店別列表失敗:', error)
  }
}

const loadConsumableList = async () => {
  try {
    const response = await consumablePrintApi.getPrintList({
      type: '2',
      status: '1', // 只載入正常狀態的耗材
      pageSize: 1000
    })
    if (response.data && response.data.success && response.data.data && response.data.data.items) {
      consumableList.value = response.data.data.items.map(item => ({
        consumableId: item.consumableId || item.ConsumableId,
        consumableName: item.consumableName || item.ConsumableName,
        unit: item.unit || item.Unit,
        price: item.price || item.Price
      }))
    }
  } catch (error) {
    console.error('載入耗材列表失敗:', error)
  }
}

watch(() => props.modelValue, (newVal) => {
  if (newVal) {
    loadSiteList()
    loadConsumableList()
  }
}, { immediate: true })
</script>

<style scoped>
</style>
