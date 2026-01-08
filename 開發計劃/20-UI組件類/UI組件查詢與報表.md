# UI組件查詢與報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能名稱**: UI組件查詢與報表
- **功能描述**: 提供UI組件的查詢與報表功能，用於查詢UI組件使用情況、產生使用報表等

### 1.2 業務需求
- 查詢UI組件使用情況
- 產生UI組件使用報表
- 統計UI組件使用次數
- 分析UI組件使用趨勢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UIComponentUsages` (UI組件使用記錄)

此資料表已在 `IMS30系列-資料維護UI組件.md` 中定義，此處不再重複。

### 2.2 查詢視圖: `V_UIComponentUsageStats` (UI組件使用統計視圖)

```sql
CREATE VIEW [dbo].[V_UIComponentUsageStats] AS
SELECT 
    c.ComponentId,
    c.ComponentCode,
    c.ComponentName,
    c.ComponentType,
    c.ComponentVersion,
    COUNT(u.UsageId) AS TotalUsageCount,
    COUNT(DISTINCT u.ModuleCode) AS UsedModuleCount,
    MAX(u.LastUsedAt) AS LastUsedAt,
    MIN(u.CreatedAt) AS FirstUsedAt
FROM [dbo].[UIComponents] c
LEFT JOIN [dbo].[UIComponentUsages] u ON c.ComponentId = u.ComponentId
WHERE c.Status = '1'
GROUP BY 
    c.ComponentId,
    c.ComponentCode,
    c.ComponentName,
    c.ComponentType,
    c.ComponentVersion;
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢UI組件使用情況
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ui-components/usage`
- **說明**: 查詢UI組件使用情況
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "componentCode": "",
    "componentType": "",
    "moduleCode": "",
    "startDate": "",
    "endDate": ""
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
          "componentId": 1,
          "componentCode": "IMS30_FB",
          "componentName": "資料瀏覽功能",
          "componentType": "FB",
          "totalUsageCount": 100,
          "usedModuleCount": 10,
          "lastUsedAt": "2024-01-01T00:00:00",
          "firstUsedAt": "2024-01-01T00:00:00"
        }
      ],
      "totalCount": 50,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 3
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 取得UI組件使用統計
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ui-components/usage/stats`
- **說明**: 取得UI組件使用統計資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "totalComponents": 50,
      "activeComponents": 45,
      "totalUsageCount": 1000,
      "topUsedComponents": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 產生UI組件使用報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/ui-components/usage/report`
- **說明**: 產生UI組件使用報表
- **請求格式**:
  ```json
  {
    "reportType": "USAGE", // USAGE, STATS, TREND
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "componentTypes": [],
    "format": "EXCEL" // EXCEL, PDF
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `UIComponentUsageController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ui-components/usage")]
    [Authorize]
    public class UIComponentUsageController : ControllerBase
    {
        private readonly IUIComponentUsageService _usageService;
        
        public UIComponentUsageController(IUIComponentUsageService usageService)
        {
            _usageService = usageService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<UIComponentUsageDto>>>> GetUsage([FromQuery] UIComponentUsageQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<UIComponentUsageStatsDto>>> GetStats()
        {
            // 實作取得統計邏輯
        }
        
        [HttpPost("report")]
        public async Task<ActionResult> GenerateReport([FromBody] UIComponentUsageReportDto dto)
        {
            // 實作產生報表邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 UI組件使用查詢頁面 (`UIComponentUsage.vue`)
- **路徑**: `/ui-components/usage`
- **功能**: 顯示UI組件使用情況查詢和報表

### 4.2 UI 元件設計

#### 4.2.1 UI組件使用查詢元件 (`UIComponentUsage.vue`)
```vue
<template>
  <div class="ui-component-usage">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>UI組件使用查詢與報表</span>
          <el-button type="primary" @click="handleGenerateReport">產生報表</el-button>
        </div>
      </template>
      
      <!-- 查詢表單 -->
      <el-form :model="queryForm" inline>
        <el-form-item label="組件代碼">
          <el-input v-model="queryForm.componentCode" placeholder="請輸入組件代碼" />
        </el-form-item>
        <el-form-item label="組件類型">
          <el-select v-model="queryForm.componentType" placeholder="請選擇組件類型">
            <el-option label="全部" value="" />
            <el-option label="FB" value="FB" />
            <el-option label="FI" value="FI" />
            <el-option label="FU" value="FU" />
            <el-option label="FQ" value="FQ" />
            <el-option label="PR" value="PR" />
            <el-option label="FS" value="FS" />
          </el-select>
        </el-form-item>
        <el-form-item label="使用日期">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 統計資訊 -->
      <el-card style="margin-top: 20px;">
        <template #header>
          <span>使用統計</span>
        </template>
        <el-row :gutter="20">
          <el-col :span="6">
            <el-statistic title="總組件數" :value="stats.totalComponents" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="啟用組件數" :value="stats.activeComponents" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="總使用次數" :value="stats.totalUsageCount" />
          </el-col>
        </el-row>
      </el-card>
      
      <!-- 資料表格 -->
      <el-table :data="usageList" v-loading="loading" style="margin-top: 20px;">
        <el-table-column prop="componentCode" label="組件代碼" width="150" />
        <el-table-column prop="componentName" label="組件名稱" width="200" />
        <el-table-column prop="componentType" label="組件類型" width="100" />
        <el-table-column prop="totalUsageCount" label="使用次數" width="120" />
        <el-table-column prop="usedModuleCount" label="使用模組數" width="120" />
        <el-table-column prop="lastUsedAt" label="最後使用時間" width="180" />
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
      />
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`uiComponentUsage.api.ts`)
```typescript
import request from '@/utils/request';

export interface UIComponentUsageQueryDto {
  pageIndex: number;
  pageSize: number;
  componentCode?: string;
  componentType?: string;
  moduleCode?: string;
  startDate?: string;
  endDate?: string;
}

export interface UIComponentUsageDto {
  componentId: number;
  componentCode: string;
  componentName: string;
  componentType: string;
  totalUsageCount: number;
  usedModuleCount: number;
  lastUsedAt: string;
  firstUsedAt: string;
}

export interface UIComponentUsageStatsDto {
  totalComponents: number;
  activeComponents: number;
  totalUsageCount: number;
  topUsedComponents: any[];
}

// API 函數
export const getUIComponentUsage = (query: UIComponentUsageQueryDto) => {
  return request.get<ApiResponse<PagedResult<UIComponentUsageDto>>>('/api/v1/ui-components/usage', { params: query });
};

export const getUIComponentUsageStats = () => {
  return request.get<ApiResponse<UIComponentUsageStatsDto>>('/api/v1/ui-components/usage/stats');
};

export const generateUsageReport = (data: UIComponentUsageReportDto) => {
  return request.post('/api/v1/ui-components/usage/report', data, { responseType: 'blob' });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立查詢視圖
- [ ] 建立索引

### 5.2 階段二: 後端開發 (3天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表產生功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] UI組件使用查詢頁面開發
- [ ] 統計資訊顯示
- [ ] 報表產生功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 9天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.2 資料驗證
- 查詢條件必須驗證
- 日期範圍必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢UI組件使用情況成功
- [ ] 取得使用統計成功
- [ ] 產生使用報表成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 報表產生測試

---

## 八、參考資料

### 8.1 相關文件
- `開發計劃/20-UI組件類/IMS30系列-資料維護UI組件.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

