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
      v-for="item in menuOptions"
      :key="item.Value"
      :label="item.Label"
      :value="item.Value"
    >
      <span>{{ item.Label }}</span>
      <span v-if="item.SystemId" style="color: #8492a6; font-size: 13px; margin-left: 10px">
        ({{ item.SystemId }})
      </span>
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
    default: '請選擇選單'
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
  systemId: {
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
const menuOptions = ref([])

// 載入選單選項
const loadMenuOptions = async () => {
  try {
    const params = {
      systemId: props.systemId || undefined,
      status: props.status || '1'
    }
    const response = await dropdownListApi.getMenuOptions(params)
    if (response.data?.success) {
      menuOptions.value = response.data.data || []
    } else {
      ElMessage.error(response.data?.message || '載入選單選項失敗')
    }
  } catch (error) {
    console.error('載入選單選項失敗：', error)
    ElMessage.error('載入選單選項失敗：' + (error.message || '未知錯誤'))
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

// 監聽systemId變化，重新載入選項
watch(() => props.systemId, () => {
  loadMenuOptions()
})

// 監聽status變化，重新載入選項
watch(() => props.status, () => {
  loadMenuOptions()
})

// 初始化
onMounted(() => {
  loadMenuOptions()
})
</script>

<style lang="scss" scoped>
</style>
