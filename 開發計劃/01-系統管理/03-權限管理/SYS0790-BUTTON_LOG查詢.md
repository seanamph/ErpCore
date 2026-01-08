# SYS0790 - BUTTON LOG 查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS0790
- **功能名稱**: BUTTON LOG 查詢
- **功能描述**: 提供按鈕操作記錄的查詢功能，記錄使用者點擊按鈕的操作歷史，包含使用者、作業、按鈕、操作時間等資訊
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0790_FQ.ASP` (查詢畫面)
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0790_FB.ASP` (瀏覽畫面)
  - `WEB/IMS_CORE/ASP/JS/BUTTON_LOG_INSERT.ASP` (記錄插入)

### 1.2 業務需求
- 查詢按鈕操作記錄
- 支援依使用者、作業代碼、日期時間範圍查詢
- 顯示操作記錄列表
- 支援報表列印與匯出
- 記錄所有按鈕點擊操作，用於審計追蹤

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ButtonLog` (對應舊系統 `BUTTON_LOG`)

```sql
CREATE TABLE [dbo].[ButtonLog] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [BUser] NVARCHAR(50) NOT NULL, -- 使用者編號 (BUSER)
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 操作時間 (BTIME)
    [ProgId] NVARCHAR(50) NULL, -- 作業代碼 (PROG_ID)
    [ProgName] NVARCHAR(100) NULL, -- 作業名稱 (PROG_NAME)
    [ButtonName] NVARCHAR(100) NULL, -- 按鈕名稱 (BUTTON_NAME)
    [Url] NVARCHAR(500) NULL, -- 網頁位址 (URL)
    [FrameName] NVARCHAR(100) NULL, -- 框架名稱 (FRAME_NAME)
    CONSTRAINT [PK_ButtonLog] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ButtonLog_BUser] ON [dbo].[ButtonLog] ([BUser]);
CREATE NONCLUSTERED INDEX [IX_ButtonLog_BTime] ON [dbo].[ButtonLog] ([BTime]);
CREATE NONCLUSTERED INDEX [IX_ButtonLog_ProgId] ON [dbo].[ButtonLog] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_ButtonLog_BUser_BTime] ON [dbo].[ButtonLog] ([BUser], [BTime]);
```

### 2.2 相關資料表

#### 2.2.1 `Users` - 使用者主檔
- 用於查詢使用者名稱
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md`

#### 2.2.2 `Programs` - 作業主檔
- 用於查詢作業名稱
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | - | 主鍵 | IDENTITY(1,1) |
| BUser | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users |
| BTime | DATETIME2 | - | NO | GETDATE() | 操作時間 | - |
| ProgId | NVARCHAR | 50 | YES | - | 作業代碼 | 外鍵至Programs |
| ProgName | NVARCHAR | 100 | YES | - | 作業名稱 | - |
| ButtonName | NVARCHAR | 100 | YES | - | 按鈕名稱 | - |
| Url | NVARCHAR | 500 | YES | - | 網頁位址 | - |
| FrameName | NVARCHAR | 100 | YES | - | 框架名稱 | - |

### 2.4 記錄插入機制

按鈕操作記錄由前端 JavaScript 自動記錄，當使用者點擊按鈕時，系統會自動呼叫 API 插入記錄。

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢按鈕操作記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/button-logs`
- **說明**: 查詢按鈕操作記錄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "BTime",
    "sortOrder": "DESC",
    "filters": {
      "userIds": ["USER001", "USER002"],
      "progId": "SYS0110",
      "startDate": "2024-01-01",
      "startTime": "0000",
      "endDate": "2024-01-31",
      "endTime": "2359"
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
          "bUser": "USER001",
          "userName": "張三",
          "bTime": "2024-01-01T10:30:00",
          "progId": "SYS0110",
          "progName": "使用者基本資料維護",
          "buttonName": "新增",
          "url": "/system/users",
          "frameName": "mainFrame"
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

#### 3.1.2 匯出按鈕操作記錄報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/button-logs/export`
- **說明**: 匯出按鈕操作記錄報表 (Excel/PDF)
- **請求格式**: 同查詢參數
- **回應格式**: 檔案下載

#### 3.1.3 插入按鈕操作記錄 (內部使用)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/button-logs`
- **說明**: 插入按鈕操作記錄 (由前端 JavaScript 自動呼叫)
- **請求格式**:
  ```json
  {
    "progId": "SYS0110",
    "buttonName": "新增",
    "url": "/system/users",
    "frameName": "mainFrame"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "記錄成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 資料傳輸物件 (DTO)

#### 3.2.1 `ButtonLogQueryDto`
```csharp
public class ButtonLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "DESC";
    public ButtonLogFilterDto? Filters { get; set; }
}

public class ButtonLogFilterDto
{
    public List<string>? UserIds { get; set; }
    public string? ProgId { get; set; }
    public DateTime? StartDate { get; set; }
    public string? StartTime { get; set; } // HHmm 格式
    public DateTime? EndDate { get; set; }
    public string? EndTime { get; set; } // HHmm 格式
}
```

#### 3.2.2 `ButtonLogDto`
```csharp
public class ButtonLogDto
{
    public long TKey { get; set; }
    public string BUser { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public DateTime BTime { get; set; }
    public string? ProgId { get; set; }
    public string? ProgName { get; set; }
    public string? ButtonName { get; set; }
    public string? Url { get; set; }
    public string? FrameName { get; set; }
}
```

#### 3.2.3 `CreateButtonLogDto`
```csharp
public class CreateButtonLogDto
{
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string? ProgId { get; set; }
    
    [StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string? ButtonName { get; set; }
    
    [StringLength(500, ErrorMessage = "網頁位址長度不能超過500")]
    public string? Url { get; set; }
    
    [StringLength(100, ErrorMessage = "框架名稱長度不能超過100")]
    public string? FrameName { get; set; }
}
```

### 3.3 Service 層設計

#### 3.3.1 `IButtonLogService`
```csharp
public interface IButtonLogService
{
    Task<PagedResult<ButtonLogDto>> GetButtonLogsAsync(ButtonLogQueryDto query, CancellationToken cancellationToken = default);
    Task<long> CreateButtonLogAsync(CreateButtonLogDto dto, CancellationToken cancellationToken = default);
    Task<byte[]> ExportButtonLogReportAsync(ButtonLogQueryDto query, string format, CancellationToken cancellationToken = default);
}
```

#### 3.3.2 `ButtonLogService` 實作重點
- 日期時間範圍查詢需合併日期與時間
- 使用者查詢支援多選
- 自動從 JWT Token 取得當前使用者資訊
- 大量資料查詢需使用分頁
- 匯出功能需支援 Excel 和 PDF 格式

### 3.4 Repository 層設計

#### 3.4.1 `IButtonLogRepository`
```csharp
public interface IButtonLogRepository
{
    Task<PagedResult<ButtonLog>> GetPagedAsync(ButtonLogQuery query, CancellationToken cancellationToken = default);
    Task<ButtonLog> CreateAsync(ButtonLog buttonLog, CancellationToken cancellationToken = default);
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 按鈕操作記錄查詢頁面 (`ButtonLogQuery.vue`)
- **路徑**: `/system/button-logs`
- **功能**: 顯示按鈕操作記錄查詢表單與結果列表
- **主要元件**:
  - 查詢表單 (ButtonLogSearchForm)
  - 資料表格 (ButtonLogDataTable)
  - 匯出按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ButtonLogSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="使用者">
      <el-select v-model="searchForm.userIds" placeholder="請選擇使用者" multiple filterable clearable>
        <el-option v-for="user in userList" :key="user.userId" :label="`${user.userId} - ${user.userName}`" :value="user.userId" />
      </el-select>
    </el-form-item>
    <el-form-item label="作業代碼">
      <el-select v-model="searchForm.progId" placeholder="請選擇作業" filterable clearable>
        <el-option v-for="program in programList" :key="program.programId" :label="program.programName" :value="program.programId" />
      </el-select>
    </el-form-item>
    <el-form-item label="LOG日期時間" required>
      <el-date-picker v-model="dateRange" type="datetimerange" range-separator="至" start-placeholder="開始日期" end-placeholder="結束日期" format="YYYY/MM/DD HH:mm" value-format="YYYY/MM/DD HH:mm" />
      <el-time-picker v-model="timeRange" is-range range-separator="至" start-placeholder="開始時間" end-placeholder="結束時間" format="HHmm" value-format="HHmm" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport">匯出</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`ButtonLogDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="buttonLogList" v-loading="loading" border>
      <el-table-column type="index" label="序號" width="60" />
      <el-table-column prop="bTime" label="操作時間" width="160" sortable="custom" />
      <el-table-column prop="bUser" label="使用者編號" width="120" />
      <el-table-column prop="userName" label="使用者名稱" width="150" />
      <el-table-column prop="progId" label="作業代碼" width="120" />
      <el-table-column prop="progName" label="作業名稱" width="200" />
      <el-table-column prop="buttonName" label="按鈕名稱" width="150" />
      <el-table-column prop="url" label="網頁位址" width="300" show-overflow-tooltip />
      <el-table-column prop="frameName" label="框架名稱" width="120" />
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

### 4.3 API 呼叫 (`buttonLog.api.ts`)
```typescript
import request from '@/utils/request';

export interface ButtonLogDto {
  tKey: number;
  bUser: string;
  userName?: string;
  bTime: string;
  progId?: string;
  progName?: string;
  buttonName?: string;
  url?: string;
  frameName?: string;
}

export interface ButtonLogQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    userIds?: string[];
    progId?: string;
    startDate?: string;
    startTime?: string;
    endDate?: string;
    endTime?: string;
  };
}

export interface CreateButtonLogDto {
  progId?: string;
  buttonName?: string;
  url?: string;
  frameName?: string;
}

// API 函數
export const getButtonLogList = (query: ButtonLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<ButtonLogDto>>>('/api/v1/button-logs', { params: query });
};

export const createButtonLog = (data: CreateButtonLogDto) => {
  return request.post<ApiResponse<number>>('/api/v1/button-logs', data);
};

export const exportButtonLogReport = (query: ButtonLogQueryDto, format: 'excel' | 'pdf') => {
  return request.post(`/api/v1/button-logs/export?format=${format}`, query, {
    responseType: 'blob'
  });
};
```

### 4.4 按鈕記錄插入機制

前端 JavaScript 需在按鈕點擊時自動呼叫 API 插入記錄：

```typescript
// utils/buttonLog.ts
import { createButtonLog } from '@/api/buttonLog.api';

export const logButtonClick = async (buttonName: string, progId?: string, url?: string, frameName?: string) => {
  try {
    await createButtonLog({
      progId,
      buttonName,
      url: url || window.location.href,
      frameName: frameName || window.name
    });
  } catch (error) {
    console.error('Failed to log button click:', error);
    // 記錄失敗不影響正常操作
  }
};
```

---

## 五、後端實作類別

### 5.1 Controller: `ButtonLogsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/button-logs")]
    [Authorize]
    public class ButtonLogsController : ControllerBase
    {
        private readonly IButtonLogService _buttonLogService;
        
        public ButtonLogsController(IButtonLogService buttonLogService)
        {
            _buttonLogService = buttonLogService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ButtonLogDto>>>> GetButtonLogs([FromQuery] ButtonLogQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateButtonLog([FromBody] CreateButtonLogDto dto)
        {
            // 實作插入邏輯
            // 從 JWT Token 取得當前使用者資訊
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportButtonLogReport([FromBody] ButtonLogQueryDto query, [FromQuery] string format = "excel")
        {
            // 實作匯出邏輯
        }
    }
}
```

### 5.2 Service: `ButtonLogService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IButtonLogService
    {
        Task<PagedResult<ButtonLogDto>> GetButtonLogsAsync(ButtonLogQueryDto query);
        Task<long> CreateButtonLogAsync(CreateButtonLogDto dto);
        Task<byte[]> ExportButtonLogReportAsync(ButtonLogQueryDto query, string format);
    }
}
```

### 5.3 Repository: `ButtonLogRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IButtonLogRepository
    {
        Task<PagedResult<ButtonLog>> GetPagedAsync(ButtonLogQuery query);
        Task<ButtonLog> CreateAsync(ButtonLog buttonLog);
    }
}
```

---

## 六、開發時程

### 6.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 6.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 日期時間範圍查詢邏輯
- [ ] 匯出功能實作
- [ ] 單元測試

### 6.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 日期時間選擇器整合
- [ ] 匯出功能整合
- [ ] 按鈕記錄插入機制整合
- [ ] 元件測試

### 6.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 大量資料查詢測試

### 6.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 七、注意事項

### 7.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 記錄插入需驗證使用者身份

### 7.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 日期時間範圍查詢需使用索引優化
- 考慮資料歸檔機制，定期清理舊資料

### 7.3 資料驗證
- 日期時間格式驗證
- 時間格式需為 HHmm (0000-2359)
- 日期時間範圍合理性檢查

### 7.4 業務邏輯
- 記錄插入需自動取得當前使用者資訊
- 日期時間範圍查詢需合併日期與時間
- 匯出功能需支援 Excel 和 PDF 格式
- 記錄插入失敗不應影響正常操作

### 7.5 審計追蹤
- 所有按鈕操作都需記錄
- 記錄不可被修改或刪除（僅查詢）
- 記錄需保留足夠時間以供審計

---

## 八、測試案例

### 8.1 單元測試
- [ ] 查詢按鈕操作記錄成功
- [ ] 依使用者查詢成功
- [ ] 依作業代碼查詢成功
- [ ] 依日期時間範圍查詢成功
- [ ] 插入按鈕操作記錄成功
- [ ] 匯出 Excel 報表成功
- [ ] 匯出 PDF 報表成功

### 8.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 按鈕記錄自動插入測試

### 8.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發查詢測試
- [ ] 並發插入測試

---

## 九、參考資料

### 9.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYS0000/SYS0790_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYS0000/SYS0790_FB.ASP`
- `WEB/IMS_CORE/ASP/JS/BUTTON_LOG_INSERT.ASP`

### 9.2 相關功能
- SYS0110 - 使用者基本資料維護
- SYS0430 - 系統作業資料維護
- SYS0440 - 系統功能按鈕資料維護

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

