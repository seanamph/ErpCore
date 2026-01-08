# ASPXTOASP - ASP轉ASPX工具 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ASPXTOASP
- **功能名稱**: ASP轉ASPX工具
- **功能描述**: 提供ASP.NET (ASPX) 與傳統ASP之間的資料轉換與會話狀態傳遞功能，用於系統遷移過渡期間的相容性支援
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/ASPXTOASP.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/ASPXTOASP.aspx.cs`
  - `WEB/IMS_CORE/Kernel/ASPXTOASP.asp`
  - `WEB/IMS_CORE/Kernel/ASPXTOASP.aspx`

### 1.2 業務需求
- 支援ASPX頁面轉換到ASP頁面
- 支援ASP頁面轉換到ASPX頁面
- 傳遞QueryString參數
- 傳遞Form表單資料
- 傳遞Session會話狀態
- 傳遞Cookie資料
- 保持使用者登入狀態
- 支援URL參數解析與轉換
- 支援系統識別資訊傳遞

### 1.3 使用場景
- 系統遷移過渡期間的相容性支援
- 舊版ASP功能與新版ASP.NET功能的整合
- 跨系統頁面跳轉
- 會話狀態保持

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PageTransitions` (頁面轉換記錄)

```sql
CREATE TABLE [dbo].[PageTransitions] (
    [TransitionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SourceUrl] NVARCHAR(500) NOT NULL, -- 來源URL
    [TargetUrl] NVARCHAR(500) NOT NULL, -- 目標URL
    [SourceType] NVARCHAR(10) NOT NULL, -- ASPX, ASP
    [TargetType] NVARCHAR(10) NOT NULL, -- ASPX, ASP
    [UserId] NVARCHAR(50) NULL,
    [SessionId] NVARCHAR(100) NULL,
    [QueryString] NVARCHAR(MAX) NULL, -- QueryString參數 (JSON格式)
    [FormData] NVARCHAR(MAX) NULL, -- Form表單資料 (JSON格式)
    [SessionData] NVARCHAR(MAX) NULL, -- Session資料 (JSON格式)
    [CookieData] NVARCHAR(MAX) NULL, -- Cookie資料 (JSON格式)
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'SUCCESS', -- SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PageTransitions] PRIMARY KEY CLUSTERED ([TransitionId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PageTransitions_UserId] ON [dbo].[PageTransitions] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_PageTransitions_SessionId] ON [dbo].[PageTransitions] ([SessionId]);
CREATE NONCLUSTERED INDEX [IX_PageTransitions_CreatedAt] ON [dbo].[PageTransitions] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `PageTransitionMappings` - 頁面轉換對應設定
```sql
CREATE TABLE [dbo].[PageTransitionMappings] (
    [MappingId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SourcePage] NVARCHAR(200) NOT NULL, -- 來源頁面
    [TargetPage] NVARCHAR(200) NOT NULL, -- 目標頁面
    [SourceType] NVARCHAR(10) NOT NULL, -- ASPX, ASP
    [TargetType] NVARCHAR(10) NOT NULL, -- ASPX, ASP
    [ParameterMapping] NVARCHAR(MAX) NULL, -- 參數對應 (JSON格式)
    [SessionMapping] NVARCHAR(MAX) NULL, -- Session對應 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_PageTransitionMappings_SourcePage_TargetPage] UNIQUE ([SourcePage], [TargetPage])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PageTransitionMappings_SourcePage] ON [dbo].[PageTransitionMappings] ([SourcePage]);
CREATE NONCLUSTERED INDEX [IX_PageTransitionMappings_TargetPage] ON [dbo].[PageTransitionMappings] ([TargetPage]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TransitionId | BIGINT | - | NO | IDENTITY(1,1) | 轉換記錄ID | 主鍵 |
| SourceUrl | NVARCHAR | 500 | NO | - | 來源URL | - |
| TargetUrl | NVARCHAR | 500 | NO | - | 目標URL | - |
| SourceType | NVARCHAR | 10 | NO | - | 來源類型 | ASPX, ASP |
| TargetType | NVARCHAR | 10 | NO | - | 目標類型 | ASPX, ASP |
| UserId | NVARCHAR | 50 | YES | - | 使用者編號 | - |
| SessionId | NVARCHAR | 100 | YES | - | 會話ID | - |
| QueryString | NVARCHAR(MAX) | - | YES | - | QueryString參數 | JSON格式 |
| FormData | NVARCHAR(MAX) | - | YES | - | Form表單資料 | JSON格式 |
| SessionData | NVARCHAR(MAX) | - | YES | - | Session資料 | JSON格式 |
| CookieData | NVARCHAR(MAX) | - | YES | - | Cookie資料 | JSON格式 |
| Status | NVARCHAR | 20 | NO | 'SUCCESS' | 狀態 | SUCCESS, FAILED |
| ErrorMessage | NVARCHAR(MAX) | - | YES | - | 錯誤訊息 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 ASPX轉ASP
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/aspx-to-asp`
- **說明**: 將ASPX頁面的資料轉換並導向到ASP頁面
- **請求格式**:
  ```json
  {
    "targetUrl": "SYS9999/SYS9999.ASP",
    "queryParams": {
      "PG_ID": "SYS0110",
      "USER_ID": "U001"
    },
    "formData": {
      "field1": "value1",
      "field2": "value2"
    },
    "sessionData": {
      "UserId": "U001",
      "User_Name": "張三"
    }
  }
  ```
- **回應格式**: 返回HTML表單，自動提交到目標ASP頁面

#### 3.1.2 ASP轉ASPX
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/asp-to-aspx`
- **說明**: 將ASP頁面的資料轉換並導向到ASPX頁面
- **請求格式**: 同ASPX轉ASP
- **回應格式**: 返回HTML表單，自動提交到目標ASPX頁面

#### 3.1.3 查詢轉換記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/page-transitions`
- **說明**: 查詢頁面轉換記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "userId": "",
    "sourceUrl": "",
    "targetUrl": "",
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
          "transitionId": 1,
          "sourceUrl": "SYS0110.aspx",
          "targetUrl": "SYS0110.ASP",
          "sourceType": "ASPX",
          "targetType": "ASP",
          "userId": "U001",
          "status": "SUCCESS",
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

#### 3.1.4 取得頁面轉換對應設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/page-transition-mappings`
- **說明**: 取得頁面轉換對應設定列表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "mappingId": 1,
        "sourcePage": "SYS0110.aspx",
        "targetPage": "SYS0110.ASP",
        "sourceType": "ASPX",
        "targetType": "ASP",
        "parameterMapping": {},
        "sessionMapping": {},
        "status": "1"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 新增頁面轉換對應設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/page-transition-mappings`
- **說明**: 新增頁面轉換對應設定
- **請求格式**:
  ```json
  {
    "sourcePage": "SYS0110.aspx",
    "targetPage": "SYS0110.ASP",
    "sourceType": "ASPX",
    "targetType": "ASP",
    "parameterMapping": {
      "PG_ID": "PG_ID",
      "USER_ID": "UserId"
    },
    "sessionMapping": {
      "UserId": "UserId",
      "User_Name": "User_Name"
    }
  }
  ```

#### 3.1.6 修改頁面轉換對應設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/page-transition-mappings/{mappingId}`
- **說明**: 修改頁面轉換對應設定
- **請求格式**: 同新增

#### 3.1.7 刪除頁面轉換對應設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/kernel/page-transition-mappings/{mappingId}`
- **說明**: 刪除頁面轉換對應設定

### 3.2 後端實作類別

#### 3.2.1 Controller: `PageTransitionController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel")]
    [Authorize]
    public class PageTransitionController : ControllerBase
    {
        private readonly IPageTransitionService _pageTransitionService;
        
        public PageTransitionController(IPageTransitionService pageTransitionService)
        {
            _pageTransitionService = pageTransitionService;
        }
        
        [HttpPost("aspx-to-asp")]
        public async Task<ActionResult> AspxToAsp([FromBody] PageTransitionDto dto)
        {
            // 實作ASPX轉ASP邏輯
        }
        
        [HttpPost("asp-to-aspx")]
        public async Task<ActionResult> AspToAspx([FromBody] PageTransitionDto dto)
        {
            // 實作ASP轉ASPX邏輯
        }
        
        [HttpGet("page-transitions")]
        public async Task<ActionResult<ApiResponse<PagedResult<PageTransitionLogDto>>>> GetPageTransitions([FromQuery] PageTransitionQueryDto query)
        {
            // 實作查詢邏輯
        }
    }
}
```

#### 3.2.2 Service: `PageTransitionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPageTransitionService
    {
        Task<string> ConvertAspxToAspAsync(PageTransitionDto dto);
        Task<string> ConvertAspToAspxAsync(PageTransitionDto dto);
        Task<PagedResult<PageTransitionLogDto>> GetPageTransitionsAsync(PageTransitionQueryDto query);
        Task<List<PageTransitionMappingDto>> GetPageTransitionMappingsAsync();
        Task<long> CreatePageTransitionMappingAsync(CreatePageTransitionMappingDto dto);
        Task UpdatePageTransitionMappingAsync(long mappingId, UpdatePageTransitionMappingDto dto);
        Task DeletePageTransitionMappingAsync(long mappingId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 頁面轉換記錄查詢頁面 (`PageTransitionLog.vue`)
- **路徑**: `/system/kernel/page-transitions`
- **功能**: 顯示頁面轉換記錄，支援查詢、篩選
- **主要元件**:
  - 查詢表單 (PageTransitionSearchForm)
  - 資料表格 (PageTransitionDataTable)

#### 4.1.2 頁面轉換對應設定維護頁面 (`PageTransitionMapping.vue`)
- **路徑**: `/system/kernel/page-transition-mappings`
- **功能**: 維護頁面轉換對應設定，支援新增、修改、刪除、查詢
- **主要元件**:
  - 查詢表單
  - 資料表格
  - 新增/修改對話框

### 4.2 UI 元件設計

#### 4.2.1 頁面轉換記錄查詢表單 (`PageTransitionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="使用者編號">
      <el-input v-model="searchForm.userId" placeholder="請輸入使用者編號" />
    </el-form-item>
    <el-form-item label="來源URL">
      <el-input v-model="searchForm.sourceUrl" placeholder="請輸入來源URL" />
    </el-form-item>
    <el-form-item label="目標URL">
      <el-input v-model="searchForm.targetUrl" placeholder="請輸入目標URL" />
    </el-form-item>
    <el-form-item label="轉換日期">
      <el-date-picker
        v-model="searchForm.dateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 頁面轉換記錄資料表格 (`PageTransitionDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="transitionList" v-loading="loading">
      <el-table-column prop="sourceUrl" label="來源URL" width="200" />
      <el-table-column prop="targetUrl" label="目標URL" width="200" />
      <el-table-column prop="sourceType" label="來源類型" width="100" />
      <el-table-column prop="targetType" label="目標類型" width="100" />
      <el-table-column prop="userId" label="使用者編號" width="120" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="row.status === 'SUCCESS' ? 'success' : 'danger'">
            {{ row.status === 'SUCCESS' ? '成功' : '失敗' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="轉換時間" width="160" />
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleViewDetail(row)">詳情</el-button>
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
  </div>
</template>
```

### 4.3 API 呼叫 (`pageTransition.api.ts`)
```typescript
import request from '@/utils/request';

export interface PageTransitionDto {
  targetUrl: string;
  queryParams?: Record<string, string>;
  formData?: Record<string, any>;
  sessionData?: Record<string, any>;
}

export interface PageTransitionLogDto {
  transitionId: number;
  sourceUrl: string;
  targetUrl: string;
  sourceType: string;
  targetType: string;
  userId?: string;
  sessionId?: string;
  status: string;
  errorMessage?: string;
  createdAt: string;
}

export interface PageTransitionQueryDto {
  pageIndex: number;
  pageSize: number;
  userId?: string;
  sourceUrl?: string;
  targetUrl?: string;
  startDate?: string;
  endDate?: string;
}

// API 函數
export const convertAspxToAsp = (data: PageTransitionDto) => {
  return request.post<string>('/api/v1/kernel/aspx-to-asp', data);
};

export const convertAspToAspx = (data: PageTransitionDto) => {
  return request.post<string>('/api/v1/kernel/asp-to-aspx', data);
};

export const getPageTransitions = (query: PageTransitionQueryDto) => {
  return request.get<ApiResponse<PagedResult<PageTransitionLogDto>>>('/api/v1/kernel/page-transitions', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 轉換邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 轉換記錄查詢頁面開發
- [ ] 對應設定維護頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 轉換功能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證轉換請求的合法性
- 必須防止惡意URL注入
- 必須驗證Session資料的完整性
- 敏感資料必須加密傳輸

### 6.2 效能
- 轉換記錄必須定期清理
- 大量轉換時必須使用非同步處理
- 必須使用快取機制減少資料庫查詢

### 6.3 資料驗證
- URL格式必須驗證
- 參數資料必須驗證
- Session資料必須驗證

### 6.4 業務邏輯
- 轉換失敗時必須記錄錯誤訊息
- 必須保持使用者登入狀態
- 必須正確傳遞所有必要參數

### 6.5 遷移策略
- 此功能主要用於系統遷移過渡期間
- 遷移完成後可考慮停用或移除
- 建議保留轉換記錄用於問題追蹤

---

## 七、測試案例

### 7.1 單元測試
- [ ] ASPX轉ASP成功
- [ ] ASP轉ASPX成功
- [ ] 參數傳遞正確
- [ ] Session傳遞正確
- [ ] Cookie傳遞正確
- [ ] 轉換失敗處理

### 7.2 整合測試
- [ ] 完整轉換流程測試
- [ ] 多頁面轉換測試
- [ ] 會話狀態保持測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量轉換測試
- [ ] 並發轉換測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/ASPXTOASP.aspx.cs`
- `WEB/IMS_CORE/Kernel/ASPXTOASP.asp`
- `WEB/IMS_CORE/Kernel/ASPXTOASP.aspx`

### 8.2 相關功能
- Session管理
- Cookie管理
- URL路由

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

