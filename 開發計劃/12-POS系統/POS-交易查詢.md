# POS - POS交易查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: POS-交易查詢
- **功能名稱**: POS交易查詢
- **功能描述**: 提供POS系統交易資料的查詢功能，包含交易明細查詢、交易狀態查詢、交易搜尋等
- **參考舊程式**: 
  - `WEB/IMS_CORE/rsl_pos/handshake.asp` (POS系統介接)
  - `WEB/IMS_CORE/ASP/SYS2000/SYS2A01_FQ.ASP` (相關查詢)

### 1.2 業務需求
- 提供POS交易資料查詢
- 支援依交易編號、日期、店別等條件查詢
- 支援交易明細查詢
- 支援交易狀態查詢
- 支援交易搜尋功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PosTransactions` (POS交易主檔)
參考 `POS-資料同步作業.md` 的 `PosTransactions` 資料表設計

### 2.2 主要資料表: `PosTransactionDetails` (POS交易明細)
參考 `POS-資料同步作業.md` 的 `PosTransactionDetails` 資料表設計

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢POS交易列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/transactions`
- **說明**: 查詢POS交易列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "transactionId": "",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "storeId": "",
    "posId": "",
    "transactionType": "",
    "status": "",
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TransactionDate",
    "sortOrder": "DESC"
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
          "id": 1,
          "transactionId": "TXN20240101001",
          "storeId": "S001",
          "storeName": "台北店",
          "posId": "POS001",
          "transactionDate": "2024-01-01T10:00:00",
          "transactionType": "Sale",
          "totalAmount": 1500.00,
          "paymentMethod": "Cash",
          "customerId": "C001",
          "status": "Synced",
          "syncAt": "2024-01-01T10:05:00",
          "createdAt": "2024-01-01T10:00:00"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆POS交易
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/transactions/{transactionId}`
- **說明**: 根據交易編號查詢單筆POS交易資料（含明細）
- **路徑參數**:
  - `transactionId`: 交易編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "id": 1,
      "transactionId": "TXN20240101001",
      "storeId": "S001",
      "storeName": "台北店",
      "posId": "POS001",
      "transactionDate": "2024-01-01T10:00:00",
      "transactionType": "Sale",
      "totalAmount": 1500.00,
      "paymentMethod": "Cash",
      "customerId": "C001",
      "status": "Synced",
      "syncAt": "2024-01-01T10:05:00",
      "details": [
        {
          "id": 1,
          "lineNo": 1,
          "productId": "P001",
          "productName": "商品A",
          "quantity": 2.0,
          "unitPrice": 500.00,
          "amount": 1000.00,
          "discount": 0.00
        }
      ],
      "createdAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢POS交易明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos/transactions/{transactionId}/details`
- **說明**: 根據交易編號查詢POS交易明細
- **路徑參數**:
  - `transactionId`: 交易編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "id": 1,
        "transactionId": "TXN20240101001",
        "lineNo": 1,
        "productId": "P001",
        "productName": "商品A",
        "quantity": 2.0,
        "unitPrice": 500.00,
        "amount": 1000.00,
        "discount": 0.00
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 交易查詢頁面 (`PosTransactionQuery.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="pos-transaction-query">
    <el-card>
      <template #header>
        <span>POS交易查詢</span>
      </template>
      
      <el-form :model="queryForm" ref="queryFormRef" label-width="120px" inline>
        <!-- 交易編號 -->
        <el-form-item label="交易編號">
          <el-input 
            v-model="queryForm.transactionId" 
            placeholder="請輸入交易編號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 日期區間 -->
        <el-form-item label="交易日期">
          <el-date-picker
            v-model="queryForm.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        
        <!-- 店別 -->
        <el-form-item label="店別">
          <el-select 
            v-model="queryForm.storeId" 
            placeholder="請選擇店別"
            clearable
            style="width: 200px"
          >
            <el-option
              v-for="item in storeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <!-- POS機號 -->
        <el-form-item label="POS機號">
          <el-input 
            v-model="queryForm.posId" 
            placeholder="請輸入POS機號"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 交易類型 -->
        <el-form-item label="交易類型">
          <el-select 
            v-model="queryForm.transactionType" 
            placeholder="請選擇交易類型"
            clearable
            style="width: 150px"
          >
            <el-option label="銷售" value="Sale" />
            <el-option label="退貨" value="Return" />
            <el-option label="退款" value="Refund" />
          </el-select>
        </el-form-item>
        
        <!-- 狀態 -->
        <el-form-item label="狀態">
          <el-select 
            v-model="queryForm.status" 
            placeholder="請選擇狀態"
            clearable
            style="width: 150px"
          >
            <el-option label="待同步" value="Pending" />
            <el-option label="已同步" value="Synced" />
            <el-option label="同步失敗" value="Failed" />
          </el-select>
        </el-form-item>
        
        <!-- 查詢按鈕 -->
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" style="margin-top: 20px">
      <template #header>
        <span>查詢結果 (共 {{ totalCount }} 筆)</span>
      </template>
      
      <el-table 
        :data="searchResult" 
        border 
        stripe 
        style="width: 100%"
        @row-click="handleRowClick"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="transactionId" label="交易編號" width="150" />
        <el-table-column prop="storeName" label="店別" width="120" />
        <el-table-column prop="posId" label="POS機號" width="100" />
        <el-table-column prop="transactionDate" label="交易日期時間" width="160" />
        <el-table-column prop="transactionType" label="交易類型" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getTransactionTypeTag(scope.row.transactionType)">
              {{ getTransactionTypeText(scope.row.transactionType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="交易金額" width="120" align="right">
          <template #default="scope">
            NT$ {{ scope.row.totalAmount.toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="paymentMethod" label="付款方式" width="100" />
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag :type="getStatusTag(scope.row.status)">
              {{ getStatusText(scope.row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleViewDetail(scope.row)">查看明細</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
    
    <!-- 交易明細對話框 -->
    <el-dialog 
      v-model="detailDialogVisible" 
      title="交易明細" 
      width="900px"
    >
      <el-table 
        :data="transactionDetails" 
        border 
        stripe 
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="lineNo" label="行號" width="80" align="center" />
        <el-table-column prop="productId" label="商品編號" width="120" />
        <el-table-column prop="productName" label="商品名稱" width="200" />
        <el-table-column prop="quantity" label="數量" width="100" align="right">
          <template #default="scope">
            {{ scope.row.quantity.toFixed(3) }}
          </template>
        </el-table-column>
        <el-table-column prop="unitPrice" label="單價" width="120" align="right">
          <template #default="scope">
            NT$ {{ scope.row.unitPrice.toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="discount" label="折扣" width="100" align="right">
          <template #default="scope">
            NT$ {{ scope.row.discount.toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="scope">
            NT$ {{ scope.row.amount.toLocaleString() }}
          </template>
        </el-table-column>
      </el-table>
      
      <template #footer>
        <el-button @click="detailDialogVisible = false">關閉</el-button>
      </template>
    </el-dialog>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { posApi } from '@/api/pos'
import { storeApi } from '@/api/store'

// 表單資料
const queryForm = reactive({
  transactionId: '',
  dateRange: [] as string[],
  storeId: '',
  posId: '',
  transactionType: '',
  status: ''
})

// 查詢結果
const searchResult = ref([])
const totalCount = ref(0)
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 交易明細
const detailDialogVisible = ref(false)
const transactionDetails = ref([])
const currentTransaction = ref<any>(null)

// 店別選項
const storeOptions = ref([])

// 查詢
const handleSearch = async () => {
  try {
    const params = {
      transactionId: queryForm.transactionId || undefined,
      startDate: queryForm.dateRange?.[0] || undefined,
      endDate: queryForm.dateRange?.[1] || undefined,
      storeId: queryForm.storeId || undefined,
      posId: queryForm.posId || undefined,
      transactionType: queryForm.transactionType || undefined,
      status: queryForm.status || undefined,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize,
      sortField: 'TransactionDate',
      sortOrder: 'DESC'
    }
    
    const response = await posApi.getTransactions(params)
    if (response.success) {
      searchResult.value = response.data.items
      pagination.totalCount = response.data.totalCount
      totalCount.value = response.data.totalCount
    } else {
      ElMessage.error(response.message || '查詢失敗')
    }
  } catch (error: any) {
    ElMessage.error('查詢失敗：' + error.message)
  }
}

// 重置
const handleReset = () => {
  queryForm.transactionId = ''
  queryForm.dateRange = []
  queryForm.storeId = ''
  queryForm.posId = ''
  queryForm.transactionType = ''
  queryForm.status = ''
  searchResult.value = []
  pagination.pageIndex = 1
  pagination.totalCount = 0
  totalCount.value = 0
}

// 查看明細
const handleViewDetail = async (row: any) => {
  try {
    currentTransaction.value = row
    const response = await posApi.getTransactionDetails(row.transactionId)
    if (response.success) {
      transactionDetails.value = response.data
      detailDialogVisible.value = true
    } else {
      ElMessage.error(response.message || '查詢明細失敗')
    }
  } catch (error: any) {
    ElMessage.error('查詢明細失敗：' + error.message)
  }
}

// 行點擊
const handleRowClick = (row: any) => {
  handleViewDetail(row)
}

// 分頁變更
const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  handleSearch()
}

const handlePageChange = (page: number) => {
  pagination.pageIndex = page
  handleSearch()
}

// 交易類型標籤
const getTransactionTypeTag = (type: string) => {
  const typeMap: Record<string, string> = {
    'Sale': 'success',
    'Return': 'warning',
    'Refund': 'danger'
  }
  return typeMap[type] || 'info'
}

// 交易類型文字
const getTransactionTypeText = (type: string) => {
  const typeMap: Record<string, string> = {
    'Sale': '銷售',
    'Return': '退貨',
    'Refund': '退款'
  }
  return typeMap[type] || type
}

// 狀態標籤
const getStatusTag = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': 'warning',
    'Synced': 'success',
    'Failed': 'danger'
  }
  return statusMap[status] || 'info'
}

// 狀態文字
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': '待同步',
    'Synced': '已同步',
    'Failed': '同步失敗'
  }
  return statusMap[status] || status
}

// 初始化
onMounted(async () => {
  // 載入店別選項
  try {
    const response = await storeApi.getList()
    if (response.success) {
      storeOptions.value = response.data.map((item: any) => ({
        value: item.storeId,
        label: item.storeName
      }))
    }
  } catch (error: any) {
    ElMessage.error('載入店別選項失敗：' + error.message)
  }
})
</script>
```

---

## 五、後端實作

### 5.1 Controller (`PosTransactionController.cs`)

```csharp
[ApiController]
[Route("api/v1/pos/transactions")]
[Authorize]
public class PosTransactionController : ControllerBase
{
    private readonly IPosTransactionService _transactionService;
    private readonly ILogger<PosTransactionController> _logger;

    public PosTransactionController(
        IPosTransactionService transactionService,
        ILogger<PosTransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢POS交易列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PosTransactionDto>>>> GetTransactions(
        [FromQuery] PosTransactionQueryDto query)
    {
        try
        {
            var result = await _transactionService.GetTransactionsAsync(query);
            return Ok(ApiResponse<PagedResult<PosTransactionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS交易列表失敗");
            return BadRequest(ApiResponse<PagedResult<PosTransactionDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢單筆POS交易
    /// </summary>
    [HttpGet("{transactionId}")]
    public async Task<ActionResult<ApiResponse<PosTransactionDetailDto>>> GetTransaction(string transactionId)
    {
        try
        {
            var result = await _transactionService.GetTransactionAsync(transactionId);
            if (result == null)
            {
                return NotFound(ApiResponse<PosTransactionDetailDto>.Error("找不到交易資料"));
            }
            return Ok(ApiResponse<PosTransactionDetailDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS交易失敗");
            return BadRequest(ApiResponse<PosTransactionDetailDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢POS交易明細
    /// </summary>
    [HttpGet("{transactionId}/details")]
    public async Task<ActionResult<ApiResponse<List<PosTransactionDetailItemDto>>>> GetTransactionDetails(string transactionId)
    {
        try
        {
            var result = await _transactionService.GetTransactionDetailsAsync(transactionId);
            return Ok(ApiResponse<List<PosTransactionDetailItemDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢POS交易明細失敗");
            return BadRequest(ApiResponse<List<PosTransactionDetailItemDto>>.Error("查詢失敗：" + ex.Message));
        }
    }
}
```

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 確認資料表結構
   - 建立索引

2. **後端 API 開發** (1.5 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 實作 DTO 定義
   - 單元測試

3. **前端 UI 開發** (1.5 天)
   - 交易查詢頁面
   - 交易明細對話框
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 4 天

---

## 七、注意事項

### 7.1 資料驗證
- 交易編號必須有效
- 日期區間必須有效
- 查詢條件需進行參數驗證

### 7.2 效能優化
- 使用索引加速查詢
- 大量資料時使用分頁查詢
- 考慮使用 Redis 快取常用查詢結果

### 7.3 使用者體驗
- 支援快速搜尋交易編號
- 支援點擊行查看明細
- 明細資料需即時載入

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 依交易編號查詢
   - 依日期區間查詢
   - 依店別查詢
   - 依POS機號查詢
   - 依交易類型查詢
   - 依狀態查詢
   - 組合條件查詢
   - 分頁查詢

2. **明細查詢測試**
   - 查看交易明細
   - 明細資料正確性

### 8.2 效能測試
- 大量資料查詢效能（10萬筆以上）
- 分頁查詢效能
- 明細查詢效能

### 8.3 安全性測試
- 權限驗證測試
- 輸入驗證測試
- SQL Injection 測試

