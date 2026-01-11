<template>
  <el-date-picker
    v-model="selectedDate"
    :type="dateType"
    :format="dateFormat"
    :value-format="valueFormat"
    :placeholder="placeholder"
    :disabled="disabled"
    :clearable="clearable"
    :picker-options="pickerOptions"
    @change="handleDateChange"
  />
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { dropdownListApi } from '@/api/dropdownList'
import { ElMessage } from 'element-plus'

const props = defineProps({
  modelValue: {
    type: [String, Date],
    default: null
  },
  dateType: {
    type: String,
    default: 'date',
    validator: (value) => ['date', 'datetime', 'daterange'].includes(value)
  },
  placeholder: {
    type: String,
    default: '請選擇日期'
  },
  disabled: {
    type: Boolean,
    default: false
  },
  clearable: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedDate = ref(props.modelValue || null)
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
watch(() => props.modelValue, (newValue) => {
  selectedDate.value = newValue
})

// 監聽 selectedDate 變化
watch(selectedDate, (newValue) => {
  emit('update:modelValue', newValue)
})

// 日期變更處理
const handleDateChange = async (value) => {
  if (value) {
    // 驗證日期格式
    try {
      const response = await dropdownListApi.validateDate({
        dateString: value,
        dateFormat: valueFormat.value
      })
      
      if (response.data?.success && response.data.data?.isValid) {
        emit('change', value)
      } else {
        ElMessage.warning(response.data?.data?.errorMessage || '日期格式不正確')
        selectedDate.value = null
        emit('update:modelValue', null)
      }
    } catch (error) {
      console.error('驗證日期格式失敗', error)
      // 即使驗證失敗，也允許使用（前端已做基本驗證）
      emit('change', value)
    }
  } else {
    emit('change', null)
  }
}

onMounted(() => {
  loadDateFormat()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';
</style>
