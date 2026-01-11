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
      v-for="item in areaOptions"
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
    default: '請選擇區域'
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
  status: {
    type: String,
    default: 'A'
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)
const areaOptions = ref([])

// 載入區域選項
const loadAreaOptions = async () => {
  try {
    const params = {
      status: props.status || 'A'
    }
    const response = await dropdownListApi.getAreaOptions(params)
    if (response.data?.success) {
      areaOptions.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '載入區域選項失敗')
    }
  } catch (error) {
    console.error('載入區域選項失敗：', error)
    ElMessage.error('載入區域選項失敗：' + (error.message || '未知錯誤'))
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

// 監聽status變化，重新載入選項
watch(() => props.status, () => {
  loadAreaOptions()
})

// 初始化
onMounted(() => {
  loadAreaOptions()
})
</script>

<style lang="scss" scoped>
</style>
