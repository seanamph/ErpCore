<template>
  <el-dialog
    v-model="visible"
    title="選擇日期"
    width="400px"
    @close="handleClose"
  >
    <el-date-picker
      v-model="selectedDate"
      type="date"
      :format="dateFormat"
      :value-format="valueFormat"
      placeholder="請選擇日期"
      style="width: 100%"
      :picker-options="pickerOptions"
    />
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { dropdownListApi } from '@/api/dropdownList'
import { ElMessage } from 'element-plus'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  returnControl: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['update:modelValue', 'confirm'])

const visible = ref(props.modelValue)
const selectedDate = ref(null)
const dateFormat = ref('yyyy/MM/dd')
const valueFormat = ref('yyyy/MM/dd')

// 日期選擇器選項（限制年份不能小於1582年）
const pickerOptions = ref({
  disabledDate(time) {
    // 年份不能小於1582年（格里曆開始年份）
    return time.getTime() < new Date('1582-01-01').getTime()
  }
})

// 載入系統日期格式設定
const loadDateFormat = async () => {
  try {
    const response = await dropdownListApi.getDateFormat()
    if (response.data?.success && response.data.data) {
      const format = response.data.data.dateFormat || 'yyyy/MM/dd'
      dateFormat.value = format
      valueFormat.value = format
    }
  } catch (error) {
    console.warn('取得系統日期格式設定失敗，使用預設格式', error)
  }
}

// 監聽 modelValue 變化
watch(() => props.modelValue, (val) => {
  visible.value = val
})

// 監聽 visible 變化
watch(visible, (val) => {
  emit('update:modelValue', val)
})

// 關閉對話框
const handleClose = () => {
  visible.value = false
  selectedDate.value = null
}

// 確認選擇
const handleConfirm = async () => {
  if (!selectedDate.value) {
    ElMessage.warning('請選擇日期')
    return
  }

  // 驗證日期格式
  try {
    const response = await dropdownListApi.validateDate({
      dateString: selectedDate.value,
      dateFormat: valueFormat.value
    })
    
    if (response.data?.success && response.data.data?.isValid) {
      emit('confirm', selectedDate.value)
      handleClose()
    } else {
      ElMessage.warning(response.data?.data?.errorMessage || '日期格式不正確')
    }
  } catch (error) {
    console.error('驗證日期格式失敗', error)
    // 即使驗證失敗，也允許使用（前端已做基本驗證）
    emit('confirm', selectedDate.value)
    handleClose()
  }
}

onMounted(() => {
  loadDateFormat()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
</style>
