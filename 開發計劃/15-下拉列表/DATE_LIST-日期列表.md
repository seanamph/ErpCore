# DATE_LIST - 日期列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: DATE_LIST
- **功能名稱**: 日期列表
- **功能描述**: 提供日期選擇的下拉列表功能，支援日曆選擇、日期格式設定、日期驗證等功能，用於表單中的日期輸入
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/DATE_LIST.aspx` (ASP.NET版本)
  - `WEB/IMS_CORE/ASP/Kernel/DATE_LIST.asp` (ASP版本)
  - `IMS3/HANSHIN/RSL_CLASS/UTILITY/UtilClass.cs` (DATE_LIST方法)

### 1.2 業務需求
- 提供日曆選擇器功能
- 支援年月日選擇
- 支援日期格式設定（yyyy/MM/dd等）
- 支援日期驗證（不能小於1582年）
- 支援今天日期快速選擇
- 支援多語言顯示（月份、星期）
- 與表單欄位整合，回傳選定的日期值

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表
此功能為前端UI組件，不需要資料表，但需要讀取系統參數設定：
- `Parameters` - 讀取日期格式設定（DATE_FORMAT）

### 2.2 相關資料表
無

### 2.3 資料字典
無

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得日期格式設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system/date-format`
- **說明**: 取得系統日期格式設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "dateFormat": "yyyy/MM/dd",
      "timeFormat": "HH:mm:ss"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 驗證日期格式
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/system/validate-date`
- **說明**: 驗證日期字串是否符合格式
- **請求格式**:
  ```json
  {
    "dateString": "2024/01/01",
    "dateFormat": "yyyy/MM/dd"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "驗證成功",
    "data": {
      "isValid": true,
      "parsedDate": "2024-01-01T00:00:00Z"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Service: `DateService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IDateService
    {
        Task<string> GetDateFormatAsync();
        Task<bool> ValidateDateAsync(string dateString, string format);
        Task<DateTime?> ParseDateAsync(string dateString, string format);
    }
    
    public class DateService : IDateService
    {
        // 實作日期相關邏輯
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 日期選擇器組件 (`DatePicker.vue`)
```vue
<template>
  <el-date-picker
    v-model="selectedDate"
    :type="dateType"
    :format="dateFormat"
    :value-format="valueFormat"
    :placeholder="placeholder"
    :disabled="disabled"
    :clearable="clearable"
    @change="handleDateChange"
  />
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useSystemStore } from '@/stores/system';

interface Props {
  modelValue?: string | Date;
  dateType?: 'date' | 'datetime' | 'daterange';
  placeholder?: string;
  disabled?: boolean;
  clearable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  dateType: 'date',
  placeholder: '請選擇日期',
  disabled: false,
  clearable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: string | Date | null];
  'change': [value: string | Date | null];
}>();

const systemStore = useSystemStore();
const selectedDate = ref<string | Date | null>(props.modelValue || null);
const dateFormat = ref('yyyy/MM/dd');
const valueFormat = ref('yyyy/MM/dd');

onMounted(async () => {
  // 取得系統日期格式設定
  const format = await systemStore.getDateFormat();
  if (format) {
    dateFormat.value = format;
    valueFormat.value = format;
  }
});

const handleDateChange = (value: string | Date | null) => {
  emit('update:modelValue', value);
  emit('change', value);
};
</script>
```

#### 4.1.2 日期選擇對話框組件 (`DatePickerDialog.vue`)
```vue
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
    />
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleConfirm">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

interface Props {
  modelValue: boolean;
  returnControl?: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'confirm': [value: string | null];
}>();

const visible = ref(props.modelValue);
const selectedDate = ref<string | null>(null);
const dateFormat = ref('yyyy/MM/dd');
const valueFormat = ref('yyyy/MM/dd');

watch(() => props.modelValue, (val) => {
  visible.value = val;
});

watch(visible, (val) => {
  emit('update:modelValue', val);
});

const handleClose = () => {
  visible.value = false;
  selectedDate.value = null;
};

const handleConfirm = () => {
  emit('confirm', selectedDate.value);
  handleClose();
};
</script>
```

### 4.2 頁面設計

#### 4.2.1 日期選擇頁面 (`DateListPage.vue`)
- 日曆顯示
- 年月選擇器
- 今天日期快速選擇
- 日期驗證
- 多語言支援

---

## 五、開發時程

### 5.1 階段一: 後端開發 (1天)
- [ ] DateService 實作
- [ ] API 端點實作
- [ ] 日期格式設定讀取
- [ ] 日期驗證邏輯
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] DatePicker 組件開發
- [ ] DatePickerDialog 組件開發
- [ ] DateListPage 頁面開發
- [ ] 多語言支援
- [ ] 日期格式處理
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 多語言測試
- [ ] 日期格式測試
- [ ] 瀏覽器相容性測試

### 5.4 階段四: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 組件使用文件
- [ ] 部署文件

**總計**: 4.5天

---

## 六、注意事項

### 6.1 日期格式
- 支援多種日期格式（yyyy/MM/dd, yyyy-MM-dd等）
- 從系統參數讀取預設格式
- 支援自訂格式

### 6.2 日期驗證
- 年份不能小於1582年（格里曆開始年份）
- 日期必須有效（如2月30日無效）
- 支援閏年判斷

### 6.3 多語言支援
- 月份名稱多語言顯示
- 星期名稱多語言顯示
- 使用 Vue I18n

### 6.4 瀏覽器相容性
- 支援現代瀏覽器（Chrome, Firefox, Edge, Safari）
- 使用 Element Plus DatePicker 組件

---

## 七、測試案例

### 7.1 單元測試
- [ ] 日期格式讀取成功
- [ ] 日期驗證成功
- [ ] 日期解析成功
- [ ] 無效日期處理

### 7.2 整合測試
- [ ] 日期選擇器正常運作
- [ ] 日期回傳正確格式
- [ ] 多語言顯示正確
- [ ] 日期驗證正確

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/ETEK_LIST/DATE_LIST.aspx`
- `IMS3/HANSHIN/IMS3/ETEK_LIST/DATE_LIST.aspx.cs`
- `WEB/IMS_CORE/ASP/Kernel/DATE_LIST.asp`

### 8.2 相關功能
- `ADDR_CITY_LIST-地址城市列表.md` - 下拉列表功能參考
- `ADDR_ZONE_LIST-地址區域列表.md` - 下拉列表功能參考

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

