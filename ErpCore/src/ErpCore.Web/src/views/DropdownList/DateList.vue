<template>
  <div class="date-list">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>日期選擇 - 日期列表 (DATE_LIST)</span>
          <div>
            <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
          </div>
        </div>
      </template>

      <!-- 日期選擇器 -->
      <div class="date-picker-container">
        <el-date-picker
          v-model="selectedDate"
          type="date"
          :format="dateFormat"
          :value-format="valueFormat"
          placeholder="請選擇日期"
          :picker-options="pickerOptions"
          style="width: 100%; max-width: 300px"
          @change="handleDateChange"
        />
      </div>

      <!-- 快速選擇按鈕 -->
      <div class="quick-select">
        <el-button @click="selectToday">今天</el-button>
        <el-button @click="selectYesterday">昨天</el-button>
        <el-button @click="selectTomorrow">明天</el-button>
        <el-button @click="selectFirstDayOfMonth">本月第一天</el-button>
        <el-button @click="selectLastDayOfMonth">本月最後一天</el-button>
      </div>

      <!-- 日期資訊顯示 -->
      <div class="date-info" v-if="selectedDate">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="選中日期">
            {{ formatDisplayDate(selectedDate) }}
          </el-descriptions-item>
          <el-descriptions-item label="星期">
            {{ getWeekday(selectedDate) }}
          </el-descriptions-item>
          <el-descriptions-item label="年份">
            {{ getYear(selectedDate) }}
          </el-descriptions-item>
          <el-descriptions-item label="月份">
            {{ getMonth(selectedDate) }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 操作按鈕 -->
      <div class="action-buttons" style="margin-top: 20px">
        <el-button type="primary" @click="handleConfirm">確定</el-button>
        <el-button @click="handleClear">清除</el-button>
        <el-button @click="handleClose">取消</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { dropdownListApi } from '@/api/dropdownList'

// Props
const props = defineProps({
  returnField: {
    type: String,
    default: 'Date'
  },
  returnControl: {
    type: String,
    default: null
  }
})

const emit = defineEmits(['select', 'close'])

// 日期相關
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

// 日期變更處理
const handleDateChange = async (value) => {
  if (value) {
    // 驗證日期格式
    try {
      const response = await dropdownListApi.validateDate({
        dateString: value,
        dateFormat: valueFormat.value
      })
      
      if (!response.data?.success || !response.data.data?.isValid) {
        ElMessage.warning(response.data?.data?.errorMessage || '日期格式不正確')
        selectedDate.value = null
      }
    } catch (error) {
      console.error('驗證日期格式失敗', error)
    }
  }
}

// 快速選擇：今天
const selectToday = () => {
  const today = new Date()
  const formatted = formatDate(today)
  selectedDate.value = formatted
}

// 快速選擇：昨天
const selectYesterday = () => {
  const yesterday = new Date()
  yesterday.setDate(yesterday.getDate() - 1)
  const formatted = formatDate(yesterday)
  selectedDate.value = formatted
}

// 快速選擇：明天
const selectTomorrow = () => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  const formatted = formatDate(tomorrow)
  selectedDate.value = formatted
}

// 快速選擇：本月第一天
const selectFirstDayOfMonth = () => {
  const today = new Date()
  const firstDay = new Date(today.getFullYear(), today.getMonth(), 1)
  const formatted = formatDate(firstDay)
  selectedDate.value = formatted
}

// 快速選擇：本月最後一天
const selectLastDayOfMonth = () => {
  const today = new Date()
  const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0)
  const formatted = formatDate(lastDay)
  selectedDate.value = formatted
}

// 格式化日期
const formatDate = (date) => {
  if (!date) return null
  
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  
  // 根據 valueFormat 格式化
  if (valueFormat.value.includes('yyyy') && valueFormat.value.includes('MM') && valueFormat.value.includes('dd')) {
    return valueFormat.value
      .replace('yyyy', year)
      .replace('MM', month)
      .replace('dd', day)
  }
  
  return `${year}/${month}/${day}`
}

// 格式化顯示日期
const formatDisplayDate = (dateString) => {
  if (!dateString) return ''
  
  try {
    const date = parseDateString(dateString)
    if (!date) return dateString
    
    // 使用系統日期格式顯示
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    
    if (dateFormat.value.includes('yyyy') && dateFormat.value.includes('MM') && dateFormat.value.includes('dd')) {
      return dateFormat.value
        .replace('yyyy', year)
        .replace('MM', month)
        .replace('dd', day)
    }
    
    return `${year}/${month}/${day}`
  } catch {
    return dateString
  }
}

// 解析日期字串
const parseDateString = (dateString) => {
  if (!dateString) return null
  
  // 嘗試解析各種格式
  const formats = [
    'yyyy/MM/dd',
    'yyyy-MM-dd',
    'yyyyMMdd',
    'MM/dd/yyyy',
    'dd/MM/yyyy'
  ]
  
  for (const format of formats) {
    const normalized = format
      .replace('yyyy', '\\d{4}')
      .replace('MM', '\\d{2}')
      .replace('dd', '\\d{2}')
    const regex = new RegExp(`^${normalized}$`)
    
    if (regex.test(dateString)) {
      // 簡單解析（實際應該使用更嚴格的解析）
      const parts = dateString.split(/[-\/]/)
      if (parts.length === 3) {
        let year, month, day
        if (format.startsWith('yyyy')) {
          year = parseInt(parts[0])
          month = parseInt(parts[1])
          day = parseInt(parts[2])
        } else if (format.includes('yyyy')) {
          const yIndex = format.indexOf('yyyy')
          const mIndex = format.indexOf('MM')
          const dIndex = format.indexOf('dd')
          year = parseInt(parts[format.substring(0, yIndex).split(/[^y]/).length - 1])
          month = parseInt(parts[format.substring(0, mIndex).split(/[^M]/).length - 1])
          day = parseInt(parts[format.substring(0, dIndex).split(/[^d]/).length - 1])
        } else {
          return null
        }
        
        return new Date(year, month - 1, day)
      }
    }
  }
  
  // 如果都失敗，嘗試使用 Date 構造函數
  const date = new Date(dateString)
  if (!isNaN(date.getTime())) {
    return date
  }
  
  return null
}

// 取得星期
const getWeekday = (dateString) => {
  const date = parseDateString(dateString)
  if (!date) return ''
  
  const weekdays = ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六']
  return weekdays[date.getDay()]
}

// 取得年份
const getYear = (dateString) => {
  const date = parseDateString(dateString)
  if (!date) return ''
  return date.getFullYear()
}

// 取得月份
const getMonth = (dateString) => {
  const date = parseDateString(dateString)
  if (!date) return ''
  return `${date.getMonth() + 1}月`
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
      // 回傳選中的日期
      emit('select', {
        date: selectedDate.value,
        dateFormatted: formatDisplayDate(selectedDate.value),
        parsedDate: response.data.data.parsedDate
      })
      
      // 如果有returnControl，則設定父視窗的控制項值
      if (props.returnControl && window.opener) {
        const returnField = props.returnField || 'Date'
        const control = window.opener.document.getElementById(props.returnControl)
        if (control) {
          control.value = selectedDate.value
        }
      }
      
      // 關閉視窗或觸發關閉事件
      if (window.opener) {
        window.close()
      } else {
        emit('close')
      }
    } else {
      ElMessage.warning(response.data?.data?.errorMessage || '日期格式不正確')
    }
  } catch (error) {
    console.error('驗證日期格式失敗', error)
    ElMessage.error('驗證日期格式失敗')
  }
}

// 清除選擇
const handleClear = () => {
  selectedDate.value = null
}

// 關閉
const handleClose = () => {
  if (window.opener) {
    window.close()
  } else {
    emit('close')
  }
}

// 初始化
onMounted(() => {
  loadDateFormat()
})
</script>

<style lang="scss" scoped>
@import '@/assets/styles/variables.scss';

.date-list {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.date-picker-container {
  margin-bottom: 20px;
  display: flex;
  justify-content: center;
}

.quick-select {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
  justify-content: center;
  flex-wrap: wrap;
}

.date-info {
  margin-bottom: 20px;
}

.action-buttons {
  display: flex;
  justify-content: center;
  gap: 10px;
}
</style>
