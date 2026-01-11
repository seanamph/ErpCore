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
      v-for="item in zoneOptions"
      :key="item.Value"
      :label="item.Label"
      :value="item.Value"
    >
      <span>{{ item.Label }}</span>
      <span v-if="item.ZipCode" style="color: #8492a6; font-size: 13px; margin-left: 10px">
        ({{ item.ZipCode }})
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
  cityId: {
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
const zoneOptions = ref([])

// 載入區域選項
const loadZoneOptions = async () => {
  try {
    // 如果沒有 cityId，不載入選項
    if (!props.cityId) {
      zoneOptions.value = []
      return
    }

    const params = {
      cityId: props.cityId || undefined,
      status: props.status || '1'
    }
    const response = await dropdownListApi.getZoneOptions(params)
    if (response.data?.success) {
      zoneOptions.value = response.data.data || []
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

// 監聽cityId變化，重新載入選項
watch(() => props.cityId, (newCityId) => {
  if (newCityId) {
    loadZoneOptions()
  } else {
    zoneOptions.value = []
    selectedValue.value = null
  }
})

// 初始化
onMounted(() => {
  if (props.cityId) {
    loadZoneOptions()
  }
})
</script>

<style lang="scss" scoped>
</style>
