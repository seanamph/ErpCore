# SYSX120 - 系統擴展查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSX120
- **功能名稱**: 系統擴展查詢
- **功能描述**: 提供系統擴展資料的查詢功能，支援多條件查詢、報表查詢等
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSX000/SYSX120_FQ.aspx` (查詢)
  - `WEB/IMS_CORE/SYSX000/SYSX120_FB.aspx` (瀏覽)

### 1.2 業務需求
- 查詢系統擴展資料
- 支援多條件組合查詢
- 支援報表查詢與列印
- 支援資料匯出
- 支援查詢結果排序與篩選

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemExtensions` (對應舊系統 `SYSX_EXTENSION`)

參考: `開發計劃/13-系統擴展/SYSX110-系統擴展資料維護.md`

### 2.2 查詢相關索引

```sql
-- 查詢優化索引
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionId_Status] ON [dbo].[SystemExtensions] ([ExtensionId], [Status]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionType_Status] ON [dbo].[SystemExtensions] ([ExtensionType], [Status]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_CreatedAt] ON [dbo].[SystemExtensions] ([CreatedAt]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/query`
- **說明**: 查詢系統擴展列表，支援多條件查詢、分頁、排序
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SeqNo",
    "sortOrder": "ASC",
    "filters": {
      "extensionId": "",
      "extensionName": "",
      "extensionType": "",
      "status": "",
      "createdDateFrom": "",
      "createdDateTo": ""
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
          "tKey": 1,
          "extensionId": "EXT001",
          "extensionName": "擴展功能1",
          "extensionType": "TYPE1",
          "extensionValue": "VALUE1",
          "seqNo": 1,
          "status": "1",
          "createdAt": "2024-01-01T00:00:00"
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

#### 3.1.2 匯出查詢結果
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/system-extensions/export`
- **說明**: 匯出查詢結果為 Excel 檔案
- **請求格式**: 同查詢列表的請求參數
- **回應格式**: Excel 檔案 (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

#### 3.1.3 查詢統計資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/statistics`
- **說明**: 查詢系統擴展的統計資訊
- **請求參數**:
  ```json
  {
    "filters": {
      "extensionType": "",
      "status": ""
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
      "totalCount": 100,
      "activeCount": 80,
      "inactiveCount": 20,
      "byType": [
        {
          "extensionType": "TYPE1",
          "count": 50
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SystemExtensionQueryController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/system-extensions")]
    [Authorize]
    public class SystemExtensionQueryController : ControllerBase
    {
        private readonly ISystemExtensionQueryService _queryService;
        
        public SystemExtensionQueryController(ISystemExtensionQueryService queryService)
        {
            _queryService = queryService;
        }
        
        [HttpGet("query")]
        public async Task<ActionResult<ApiResponse<PagedResult<SystemExtensionDto>>>> QuerySystemExtensions([FromQuery] SystemExtensionQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportSystemExtensions([FromBody] SystemExtensionQueryDto query)
        {
            // 實作匯出邏輯
        }
        
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<SystemExtensionStatisticsDto>>> GetStatistics([FromQuery] SystemExtensionStatisticsQueryDto query)
        {
            // 實作統計邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統擴展查詢頁面 (`SystemExtensionQuery.vue`)
- **路徑**: `/system-extensions/query`
- **功能**: 系統擴展資料查詢，支援多條件查詢、報表查詢、資料匯出
- **主要元件**:
  - 查詢表單 (SystemExtensionQueryForm)
  - 查詢結果表格 (SystemExtensionQueryTable)
  - 統計資訊面板 (SystemExtensionStatistics)
  - 匯出功能按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SystemExtensionQueryForm.vue`)
```vue
<template>
  <el-card>
    <el-form :model="queryForm" inline>
      <el-form-item label="擴展功能代碼">
        <el-input v-model="queryForm.extensionId" placeholder="請輸入擴展功能代碼" />
      </el-form-item>
      <el-form-item label="擴展功能名稱">
        <el-input v-model="queryForm.extensionName" placeholder="請輸入擴展功能名稱" />
      </el-form-item>
      <el-form-item label="擴展類型">
        <el-select v-model="queryForm.extensionType" placeholder="請選擇擴展類型" clearable>
          <el-option v-for="type in extensionTypeList" :key="type.value" :label="type.label" :value="type.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="狀態">
        <el-select v-model="queryForm.status" placeholder="請選擇狀態" clearable>
          <el-option label="全部" value="" />
          <el-option label="啟用" value="1" />
          <el-option label="停用" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item label="建立日期">
        <el-date-picker
          v-model="dateRange"
          type="daterange"
          range-separator="至"
          start-placeholder="開始日期"
          end-placeholder="結束日期"
          @change="handleDateRangeChange"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
        <el-button type="success" @click="handleExport">匯出</el-button>
      </el-form-item>
    </el-form>
  </el-card>
</template>
```

#### 4.2.2 查詢結果表格元件 (`SystemExtensionQueryTable.vue`)
```vue
<template>
  <el-card>
    <div class="table-header">
      <span>查詢結果 (共 {{ pagination.totalCount }} 筆)</span>
      <el-button type="primary" size="small" @click="handleExport">匯出</el-button>
    </div>
    <el-table :data="queryResult" v-loading="loading" border>
      <el-table-column prop="extensionId" label="擴展功能代碼" width="150" />
      <el-table-column prop="extensionName" label="擴展功能名稱" width="200" />
      <el-table-column prop="extensionType" label="擴展類型" width="120" />
      <el-table-column prop="extensionValue" label="擴展值" width="200" show-overflow-tooltip />
      <el-table-column prop="seqNo" label="排序序號" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="建立時間" width="180" />
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </el-card>
</template>
```

#### 4.2.3 統計資訊面板 (`SystemExtensionStatistics.vue`)
```vue
<template>
  <el-card>
    <template #header>
      <span>統計資訊</span>
    </template>
    <el-row :gutter="20">
      <el-col :span="6">
        <el-statistic title="總數" :value="statistics.totalCount" />
      </el-col>
      <el-col :span="6">
        <el-statistic title="啟用數" :value="statistics.activeCount">
          <template #suffix>
            <el-tag type="success">啟用</el-tag>
          </template>
        </el-statistic>
      </el-col>
      <el-col :span="6">
        <el-statistic title="停用數" :value="statistics.inactiveCount">
          <template #suffix>
            <el-tag type="danger">停用</el-tag>
          </template>
        </el-statistic>
      </el-col>
    </el-row>
    <el-divider />
    <div v-if="statistics.byType && statistics.byType.length > 0">
      <h4>依類型統計</h4>
      <el-table :data="statistics.byType" border>
        <el-table-column prop="extensionType" label="擴展類型" />
        <el-table-column prop="count" label="數量" />
      </el-table>
    </div>
  </el-card>
</template>
```

### 4.3 API 呼叫 (`system-extension-query.api.ts`)
```typescript
import request from '@/utils/request';

export interface SystemExtensionQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    extensionId?: string;
    extensionName?: string;
    extensionType?: string;
    status?: string;
    createdDateFrom?: string;
    createdDateTo?: string;
  };
}

export interface SystemExtensionStatisticsQueryDto {
  filters?: {
    extensionType?: string;
    status?: string;
  };
}

export interface SystemExtensionStatisticsDto {
  totalCount: number;
  activeCount: number;
  inactiveCount: number;
  byType: Array<{
    extensionType: string;
    count: number;
  }>;
}

// API 函數
export const querySystemExtensions = (query: SystemExtensionQueryDto) => {
  return request.get<ApiResponse<PagedResult<SystemExtensionDto>>>('/api/v1/system-extensions/query', { params: query });
};

export const exportSystemExtensions = (query: SystemExtensionQueryDto) => {
  return request.post('/api/v1/system-extensions/export', query, {
    responseType: 'blob'
  });
};

export const getSystemExtensionStatistics = (query: SystemExtensionStatisticsQueryDto) => {
  return request.get<ApiResponse<SystemExtensionStatisticsDto>>('/api/v1/system-extensions/statistics', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立查詢優化索引
- [ ] 建立統計查詢視圖（如需要）

### 5.2 階段二: 後端開發 (2天)
- [ ] Query Service 實作
- [ ] Controller 實作
- [ ] 匯出功能實作
- [ ] 統計功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 查詢結果表格開發
- [ ] 統計資訊面板開發
- [ ] 匯出功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 5.5天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 統計查詢需要優化

### 6.2 資料驗證
- 查詢條件必須驗證
- 日期範圍必須驗證

### 6.3 匯出功能
- 大量資料匯出必須使用非同步處理
- 匯出檔案格式必須正確
- 匯出檔案名稱必須包含時間戳記

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢系統擴展列表成功
- [ ] 查詢系統擴展列表失敗 (無權限)
- [ ] 匯出查詢結果成功
- [ ] 查詢統計資訊成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 匯出功能測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發查詢測試
- [ ] 統計查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSX000/SYSX120_FQ.aspx`
- `WEB/IMS_CORE/SYSX000/SYSX120_FB.aspx`

### 8.2 相關功能
- `開發計劃/13-系統擴展/SYSX110-系統擴展資料維護.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


