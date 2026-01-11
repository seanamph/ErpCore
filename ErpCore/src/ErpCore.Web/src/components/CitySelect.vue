<template>
  <el-select
    v-model="selectedValue"
    :placeholder="placeholder"
    :clearable="clearable"
    :filterable="filterable"
    :multiple="multiple"
    :disabled="disabled"
    @change="handleChange"
    style="width: 100%"
  >
    <el-option
      v-for="item in cityOptions"
      :key="item.Value"
      :label="item.Label"
      :value="item.Value"
    />
  </el-select>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

const props = defineProps({
  modelValue: {
    type: [String, Array],
    default: null
  },
  placeholder: {
    type: String,
    default: '請選擇城市'
  },
  clearable: {
    type: Boolean,
    default: true
  },
  filterable: {
    type: Boolean,
    default: true
  },
  multiple: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  },
  countryCode: {
    type: String,
    default: null
  },
  status: {
    type: String,
    default: '1'
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)
const cityOptions = ref([])

// 載入城市選項
const loadCityOptions = async () => {
  try {
    const params = {
      countryCode: props.countryCode || undefined,
      status: props.status || '1'
    }
    const response = await dropdownListApi.getCityOptions(params)
    if (response.data?.success) {
      cityOptions.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '載入城市選項失敗')
    }
  } catch (error) {
    console.error('載入城市選項失敗：', error)
    ElMessage.error('載入城市選項失敗：' + (error.message || '未知錯誤'))
  }
}

// 變更處理
const handleChange = (value) => {
  emit('update:modelValue', value)
  emit('change', value)
}

// 監聽modelValue變化
watch(() => props.modelValue, (newValue) => {
  selectedValue.value = newValue
})

// 監聽countryCode變化，重新載入選項
watch(() => props.countryCode, () => {
  loadCityOptions()
})

// 初始化
onMounted(() => {
  loadCityOptions()
})
</script>

<style lang="scss" scoped>
</style>
