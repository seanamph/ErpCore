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
      v-for="item in systemOptions"
      :key="item.Value"
      :label="item.Label"
      :value="item.Value"
    >
      <span>{{ item.Label }}</span>
    </el-option>
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
    default: '請選擇系統'
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
    default: '1'
  },
  excludeSystems: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)
const systemOptions = ref([])

// 載入系統選項
const loadSystemOptions = async () => {
  try {
    const params = {
      status: props.status || '1',
      excludeSystems: props.excludeSystems || undefined
    }
    const response = await dropdownListApi.getSystemOptions(params)
    if (response.data?.success) {
      systemOptions.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '載入系統選項失敗')
    }
  } catch (error) {
    console.error('載入系統選項失敗：', error)
    ElMessage.error('載入系統選項失敗：' + (error.message || '未知錯誤'))
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
  loadSystemOptions()
})

// 監聽excludeSystems變化，重新載入選項
watch(() => props.excludeSystems, () => {
  loadSystemOptions()
})

// 初始化
onMounted(() => {
  loadSystemOptions()
})
</script>

<style lang="scss" scoped>
</style>
