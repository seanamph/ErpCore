# ADDR_CITY_LIST - 地址城市列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ADDR_CITY_LIST
- **功能名稱**: 地址城市列表
- **功能描述**: 提供地址城市選擇的下拉列表功能，支援城市查詢、篩選、選擇等功能，用於地址輸入時的城市選擇
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/ADDR_CITY_LIST.aspx` (ASP.NET版本)
  - `WEB/IMS_CORE/ASP/Kernel/ADDR_CITY_LIST.asp` (ASP版本)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/CITYClass.cs` (業務邏輯)

### 1.2 業務需求
- 提供城市列表查詢功能
- 支援城市名稱篩選
- 支援城市選擇並回傳城市代碼
- 支援多選模式（可選）
- 支援排序功能
- 與地址區域列表（ADDR_ZONE_LIST）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Cities` (城市主檔)

```sql
CREATE TABLE [dbo].[Cities] (
    [CityId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 城市代碼 (CITY)
    [CityName] NVARCHAR(100) NOT NULL, -- 城市名稱 (CITY_NAME)
    [CountryCode] NVARCHAR(10) NULL, -- 國家代碼 (COUNTRY_CODES)
    [SeqNo] INT NULL, -- 排序序號
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED ([CityId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Cities_CityName] ON [dbo].[Cities] ([CityName]);
CREATE NONCLUSTERED INDEX [IX_Cities_CountryCode] ON [dbo].[Cities] ([CountryCode]);
CREATE NONCLUSTERED INDEX [IX_Cities_Status] ON [dbo].[Cities] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Cities_SeqNo] ON [dbo].[Cities] ([SeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `Zones` - 區域主檔
- 參考: `開發計劃/15-下拉列表/ADDR_ZONE_LIST-地址區域列表.md`

### 2.3 資料字典

#### Cities 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| CityId | NVARCHAR | 20 | NO | - | 城市代碼 | 主鍵 |
| CityName | NVARCHAR | 100 | NO | - | 城市名稱 | - |
| CountryCode | NVARCHAR | 10 | YES | - | 國家代碼 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢城市列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/cities`
- **說明**: 查詢城市列表，支援篩選、排序、分頁
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 50,
    "sortField": "CityName",
    "sortOrder": "ASC",
    "filters": {
      "cityName": "",
      "countryCode": "",
      "status": "1"
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "cityId": "TPE",
          "cityName": "台北市",
          "countryCode": "TW",
          "seqNo": 1,
          "status": "1"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 50,
      "totalPages": 2
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆城市
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/cities/{cityId}`
- **說明**: 根據城市代碼查詢單筆城市資料
- **路徑參數**:
  - `cityId`: 城市代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "cityId": "TPE",
      "cityName": "台北市",
      "countryCode": "TW",
      "seqNo": 1,
      "status": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢城市選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/cities/options`
- **說明**: 取得城市選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `countryCode`: 國家代碼（可選）
  - `status`: 狀態（預設為'1'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "TPE",
        "label": "台北市"
      },
      {
        "value": "NTP",
        "label": "新北市"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 城市列表選擇頁面 (`ADDR_CITY_LIST.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="addr-city-list">
    <el-card>
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>地址輸入 - 縣市列表</span>
          <div>
            <el-button type="info" @click="handleForeignAddress">國外地址</el-button>
            <el-button @click="handleClose">關閉</el-button>
          </div>
        </div>
      </template>
      
      <!-- 查詢條件 -->
      <el-form :model="queryForm" ref="queryFormRef" inline>
        <el-form-item label="城市名稱">
          <el-input 
            v-model="queryForm.cityName" 
            placeholder="請輸入城市名稱"
            clearable
            @keyup.enter="handleSearch"
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 城市列表 -->
      <el-table 
        :data="cityList" 
        border 
        stripe 
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%; cursor: pointer"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="cityId" label="城市代碼" width="120" />
        <el-table-column prop="cityName" label="城市名稱" min-width="200" />
        <el-table-column prop="countryCode" label="國家代碼" width="100" align="center" />
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[20, 50, 100, 200]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { listApi } from '@/api/list'

// 表單資料
const queryForm = reactive({
  cityName: ''
})

// 城市列表
const cityList = ref([])
const pagination = reactive({
  pageIndex: 1,
  pageSize: 50,
  totalCount: 0
})

// 查詢參數
const props = defineProps<{
  returnField?: string
  returnControl?: string
  countryCode?: string
}>()

const emit = defineEmits<{
  (e: 'select', city: any): void
  (e: 'close'): void
}>()

// 查詢城市列表
const loadCityList = async () => {
  try {
    const response = await listApi.getCities({
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      filters: {
        cityName: queryForm.cityName || undefined,
        countryCode: props.countryCode || undefined,
        status: '1'
      },
      sortField: 'CityName',
      sortOrder: 'ASC'
    })
    
    if (response.success) {
      cityList.value = response.data.items
      pagination.totalCount = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 查詢
const handleSearch = () => {
  pagination.pageIndex = 1
  loadCityList()
}

// 重置
const handleReset = () => {
  queryForm.cityName = ''
  handleSearch()
}

// 選擇城市
const handleRowClick = (row: any) => {
  // 回傳選中的城市
  emit('select', {
    cityId: row.cityId,
    cityName: row.cityName
  })
  
  // 如果有returnControl，則設定父視窗的控制項值
  if (props.returnControl && window.opener) {
    const returnField = props.returnField || 'cityId'
    const control = window.opener.document.getElementById(props.returnControl)
    if (control) {
      (control as HTMLInputElement).value = row[returnField]
    }
  }
  
  // 關閉視窗或觸發關閉事件
  if (window.opener) {
    window.close()
  } else {
    emit('close')
  }
}

// 國外地址
const handleForeignAddress = () => {
  if (confirm('選擇輸入國外地址？')) {
    // 開啟國外地址選擇頁面
    // 這裡需要實作國外地址選擇功能
    ElMessage.info('國外地址功能開發中')
  }
}

// 關閉
const handleClose = () => {
  if (window.opener) {
    window.close()
  } else {
    emit('close')
  }
}

// 分頁變更
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadCityList()
}

const handlePageChange = (page: number) => {
  pagination.pageIndex = page
  loadCityList()
}

// 初始化
onMounted(() => {
  loadCityList()
})
</script>
```

### 4.2 城市下拉選單組件 (`CitySelect.vue`)

```vue
<template>
  <el-select
    v-model="selectedValue"
    :placeholder="placeholder"
    :clearable="clearable"
    :filterable="filterable"
    :multiple="multiple"
    @change="handleChange"
    style="width: 100%"
  >
    <el-option
      v-for="item in cityOptions"
      :key="item.value"
      :label="item.label"
      :value="item.value"
    />
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { listApi } from '@/api/list'

const props = defineProps<{
  modelValue?: string | string[]
  placeholder?: string
  clearable?: boolean
  filterable?: boolean
  multiple?: boolean
  countryCode?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | string[]): void
  (e: 'change', value: string | string[]): void
}>()

const selectedValue = ref(props.modelValue)
const cityOptions = ref<Array<{ value: string; label: string }>>([])

// 載入城市選項
const loadCityOptions = async () => {
  try {
    const response = await listApi.getCityOptions({
      countryCode: props.countryCode,
      status: '1'
    })
    
    if (response.success) {
      cityOptions.value = response.data
    }
  } catch (error) {
    console.error('載入城市選項失敗：', error)
  }
}

// 變更處理
const handleChange = (value: string | string[]) => {
  emit('update:modelValue', value)
  emit('change', value)
}

// 監聽modelValue變化
watch(() => props.modelValue, (newValue) => {
  selectedValue.value = newValue
})

// 初始化
onMounted(() => {
  loadCityOptions()
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`CityListController.cs`)

```csharp
[ApiController]
[Route("api/v1/lists/cities")]
[Authorize]
public class CityListController : ControllerBase
{
    private readonly ICityListService _cityListService;
    private readonly ILogger<CityListController> _logger;

    public CityListController(
        ICityListService cityListService,
        ILogger<CityListController> logger)
    {
        _cityListService = cityListService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢城市列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CityDto>>>> GetCities(
        [FromQuery] CityListQueryDto query)
    {
        try
        {
            var result = await _cityListService.GetCitiesAsync(query);
            return Ok(ApiResponse<PagedResult<CityDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢城市列表失敗");
            return BadRequest(ApiResponse<PagedResult<CityDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢單筆城市
    /// </summary>
    [HttpGet("{cityId}")]
    public async Task<ActionResult<ApiResponse<CityDto>>> GetCity(string cityId)
    {
        try
        {
            var result = await _cityListService.GetCityAsync(cityId);
            if (result == null)
            {
                return NotFound(ApiResponse<CityDto>.Error("找不到城市"));
            }
            return Ok(ApiResponse<CityDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢城市失敗");
            return BadRequest(ApiResponse<CityDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢城市選項（用於下拉選單）
    /// </summary>
    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<List<CityOptionDto>>>> GetCityOptions(
        [FromQuery] CityOptionQueryDto query)
    {
        try
        {
            var result = await _cityListService.GetCityOptionsAsync(query);
            return Ok(ApiResponse<List<CityOptionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢城市選項失敗");
            return BadRequest(ApiResponse<List<CityOptionDto>>.Error("查詢失敗：" + ex.Message));
        }
    }
}
```

### 5.2 Service (`CityListService.cs`)

```csharp
public class CityListService : ICityListService
{
    private readonly IDbConnection _db;
    private readonly ILogger<CityListService> _logger;

    public CityListService(
        IDbConnection db,
        ILogger<CityListService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<CityDto>> GetCitiesAsync(CityListQueryDto query)
    {
        var sql = @"
            SELECT 
                CityId,
                CityName,
                CountryCode,
                SeqNo,
                Status
            FROM Cities
            WHERE 1 = 1
                AND (@CityName IS NULL OR CityName LIKE '%' + @CityName + '%')
                AND (@CountryCode IS NULL OR CountryCode = @CountryCode)
                AND (@Status IS NULL OR Status = @Status)
            ORDER BY 
                CASE WHEN @SortField = 'CityName' AND @SortOrder = 'ASC' THEN CityName END ASC,
                CASE WHEN @SortField = 'CityName' AND @SortOrder = 'DESC' THEN CityName END DESC,
                CASE WHEN @SortField = 'CityId' AND @SortOrder = 'ASC' THEN CityId END ASC,
                CASE WHEN @SortField = 'CityId' AND @SortOrder = 'DESC' THEN CityId END DESC,
                SeqNo, CityName
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT COUNT(*) AS TotalCount
            FROM Cities
            WHERE 1 = 1
                AND (@CityName IS NULL OR CityName LIKE '%' + @CityName + '%')
                AND (@CountryCode IS NULL OR CountryCode = @CountryCode)
                AND (@Status IS NULL OR Status = @Status);
        ";

        var parameters = new
        {
            CityName = string.IsNullOrEmpty(query.Filters?.CityName) ? (string?)null : query.Filters.CityName,
            CountryCode = string.IsNullOrEmpty(query.Filters?.CountryCode) ? (string?)null : query.Filters.CountryCode,
            Status = string.IsNullOrEmpty(query.Filters?.Status) ? (string?)null : query.Filters.Status,
            SortField = query.SortField ?? "CityName",
            SortOrder = query.SortOrder ?? "ASC",
            Offset = (query.PageIndex - 1) * query.PageSize,
            PageSize = query.PageSize
        };

        using var multi = await _db.QueryMultipleAsync(sql, parameters);
        var items = (await multi.ReadAsync<CityDto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedResult<CityDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<CityDto> GetCityAsync(string cityId)
    {
        var sql = @"
            SELECT 
                CityId,
                CityName,
                CountryCode,
                SeqNo,
                Status
            FROM Cities
            WHERE CityId = @CityId;
        ";

        return await _db.QueryFirstOrDefaultAsync<CityDto>(sql, new { CityId = cityId });
    }

    public async Task<List<CityOptionDto>> GetCityOptionsAsync(CityOptionQueryDto query)
    {
        var sql = @"
            SELECT 
                CityId AS Value,
                CityName AS Label
            FROM Cities
            WHERE 1 = 1
                AND (@CountryCode IS NULL OR CountryCode = @CountryCode)
                AND (@Status IS NULL OR Status = @Status)
            ORDER BY SeqNo, CityName;
        ";

        var parameters = new
        {
            CountryCode = string.IsNullOrEmpty(query.CountryCode) ? (string?)null : query.CountryCode,
            Status = string.IsNullOrEmpty(query.Status) ? "1" : query.Status
        };

        return (await _db.QueryAsync<CityOptionDto>(sql, parameters)).ToList();
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立 Cities 資料表
   - 建立索引
   - 資料遷移

2. **後端 API 開發** (1.5 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 單元測試

3. **前端 UI 開發** (1.5 天)
   - 城市列表選擇頁面
   - 城市下拉選單組件
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 4 天

---

## 七、注意事項

### 7.1 資料驗證
- 城市代碼必須唯一
- 城市名稱必填
- 狀態預設為啟用

### 7.2 效能優化
- 使用索引加速查詢
- 下拉選單選項可快取
- 大量資料時使用分頁查詢

### 7.3 整合
- 與地址區域列表（ADDR_ZONE_LIST）整合
- 支援國外地址選擇
- 支援多語言顯示

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依城市名稱查詢
   - 依國家代碼查詢
   - 組合條件查詢
   - 分頁查詢

2. **選擇測試**
   - 單選城市
   - 多選城市（可選）
   - 回傳值正確性

3. **下拉選單測試**
   - 載入城市選項
   - 篩選功能
   - 選擇功能

### 8.2 效能測試
- 大量資料查詢效能
- 下拉選單載入效能

### 8.3 整合測試
- 與地址區域列表整合
- 與地址輸入表單整合

