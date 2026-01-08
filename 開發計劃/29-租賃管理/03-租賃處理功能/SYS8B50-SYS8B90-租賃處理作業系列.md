# SYS8B50-SYS8B90 - 租賃處理作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS8B50-SYS8B90 系列
- **功能名稱**: 租賃處理作業系列
- **功能描述**: 提供租賃處理作業的新增、修改、刪除、查詢功能，包含租賃處理、處理記錄、處理狀態、處理結果等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B60_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYS8000/SYS8B90_FI.ASP` (新增)

### 1.2 業務需求
- 管理租賃處理作業基本資料
- 支援租賃處理作業的新增、修改、刪除、查詢
- 支援處理記錄管理
- 支援處理狀態管理（待處理、處理中、已完成、已取消）
- 支援處理結果記錄
- 支援租賃處理報表列印
- 支援租賃處理歷史記錄查詢
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LeaseProcesses` (租賃處理主檔)

```sql
CREATE TABLE [dbo].[LeaseProcesses] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號 (PROCESS_ID)
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (PROCESS_TYPE, RENEWAL:續約, TERMINATION:終止, MODIFICATION:修改, PAYMENT:付款)
    [ProcessDate] DATETIME2 NOT NULL, -- 處理日期 (PROCESS_DATE)
    [ProcessStatus] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 處理狀態 (PROCESS_STATUS, P:待處理, I:處理中, C:已完成, X:已取消)
    [ProcessResult] NVARCHAR(20) NULL, -- 處理結果 (PROCESS_RESULT, SUCCESS:成功, FAILED:失敗, PENDING:待處理)
    [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員 (PROCESS_USER_ID)
    [ProcessUserName] NVARCHAR(100) NULL, -- 處理人員名稱 (PROCESS_USER_NAME)
    [ProcessMemo] NVARCHAR(500) NULL, -- 處理備註 (PROCESS_MEMO)
    [ApprovalUserId] NVARCHAR(50) NULL, -- 審核人員 (APPROVAL_USER_ID)
    [ApprovalDate] DATETIME2 NULL, -- 審核日期 (APPROVAL_DATE)
    [ApprovalStatus] NVARCHAR(10) NULL, -- 審核狀態 (APPROVAL_STATUS, P:待審核, A:已審核, R:已拒絕)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_LeaseProcesses] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseProcesses_ProcessId] UNIQUE ([ProcessId]),
    CONSTRAINT [FK_LeaseProcesses_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessId] ON [dbo].[LeaseProcesses] ([ProcessId]);
CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_LeaseId] ON [dbo].[LeaseProcesses] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessType] ON [dbo].[LeaseProcesses] ([ProcessType]);
CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessStatus] ON [dbo].[LeaseProcesses] ([ProcessStatus]);
CREATE NONCLUSTERED INDEX [IX_LeaseProcesses_ProcessDate] ON [dbo].[LeaseProcesses] ([ProcessDate]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseProcessDetails` - 租賃處理明細
```sql
CREATE TABLE [dbo].[LeaseProcessDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
    [OldValue] NVARCHAR(MAX) NULL, -- 舊值 (OLD_VALUE)
    [NewValue] NVARCHAR(MAX) NULL, -- 新值 (NEW_VALUE)
    [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseProcessDetails_LeaseProcesses] FOREIGN KEY ([ProcessId]) REFERENCES [dbo].[LeaseProcesses] ([ProcessId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseProcessDetails_ProcessId] ON [dbo].[LeaseProcessDetails] ([ProcessId]);
```

#### 2.2.2 `LeaseProcessLogs` - 租賃處理日誌
```sql
CREATE TABLE [dbo].[LeaseProcessLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號
    [LogDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 日誌日期 (LOG_DATE)
    [LogType] NVARCHAR(20) NULL, -- 日誌類型 (LOG_TYPE, INFO:資訊, WARNING:警告, ERROR:錯誤)
    [LogMessage] NVARCHAR(MAX) NULL, -- 日誌訊息 (LOG_MESSAGE)
    [LogUserId] NVARCHAR(50) NULL, -- 操作人員 (LOG_USER_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseProcessLogs_LeaseProcesses] FOREIGN KEY ([ProcessId]) REFERENCES [dbo].[LeaseProcesses] ([ProcessId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseProcessLogs_ProcessId] ON [dbo].[LeaseProcessLogs] ([ProcessId]);
CREATE NONCLUSTERED INDEX [IX_LeaseProcessLogs_LogDate] ON [dbo].[LeaseProcessLogs] ([LogDate]);
```

### 2.3 資料字典

#### 2.3.1 LeaseProcesses 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ProcessId | NVARCHAR | 50 | NO | - | 處理編號 | 唯一，PROCESS_ID |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 外鍵至租賃表 |
| ProcessType | NVARCHAR | 20 | NO | - | 處理類型 | RENEWAL:續約, TERMINATION:終止, MODIFICATION:修改, PAYMENT:付款 |
| ProcessDate | DATETIME2 | - | NO | - | 處理日期 | - |
| ProcessStatus | NVARCHAR | 10 | NO | 'P' | 處理狀態 | P:待處理, I:處理中, C:已完成, X:已取消 |
| ProcessResult | NVARCHAR | 20 | YES | - | 處理結果 | SUCCESS:成功, FAILED:失敗, PENDING:待處理 |
| ProcessUserId | NVARCHAR | 50 | YES | - | 處理人員 | - |
| ProcessMemo | NVARCHAR | 500 | YES | - | 處理備註 | - |
| ApprovalStatus | NVARCHAR | 10 | YES | - | 審核狀態 | P:待審核, A:已審核, R:已拒絕 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃處理列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-processes`
- **說明**: 查詢租賃處理列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ProcessId",
    "sortOrder": "ASC",
    "filters": {
      "processId": "",
      "leaseId": "",
      "processType": "",
      "processStatus": "",
      "processDateFrom": "",
      "processDateTo": ""
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
          "processId": "PROC001",
          "leaseId": "L001",
          "processType": "RENEWAL",
          "processTypeName": "續約",
          "processDate": "2024-01-01",
          "processStatus": "C",
          "processStatusName": "已完成",
          "processResult": "SUCCESS",
          "processResultName": "成功",
          "processUserName": "張三"
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

#### 3.1.2 查詢單筆租賃處理
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-processes/{processId}`
- **說明**: 根據處理編號查詢單筆租賃處理資料
- **路徑參數**:
  - `processId`: 處理編號
- **回應格式**: 同查詢列表，但只返回單筆資料，包含明細和日誌資料

#### 3.1.3 根據租賃編號查詢處理列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-processes/by-lease/{leaseId}`
- **說明**: 根據租賃編號查詢該租賃的所有處理記錄
- **路徑參數**:
  - `leaseId`: 租賃編號
- **回應格式**: 同查詢列表

#### 3.1.4 新增租賃處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-processes`
- **說明**: 新增租賃處理作業
- **請求格式**:
  ```json
  {
    "processId": "PROC001",
    "leaseId": "L001",
    "processType": "RENEWAL",
    "processDate": "2024-01-01",
    "processStatus": "P",
    "processMemo": "續約處理",
    "details": [
      {
        "fieldName": "END_DATE",
        "oldValue": "2024-12-31",
        "newValue": "2025-12-31",
        "fieldType": "DATE"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "processId": "PROC001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改租賃處理
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-processes/{processId}`
- **說明**: 修改租賃處理資料
- **路徑參數**:
  - `processId`: 處理編號
- **請求格式**: 同新增，但 `processId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除租賃處理
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-processes/{processId}`
- **說明**: 刪除租賃處理資料（軟刪除或硬刪除）
- **路徑參數**:
  - `processId`: 處理編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 更新租賃處理狀態
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-processes/{processId}/status`
- **說明**: 更新租賃處理狀態
- **請求格式**:
  ```json
  {
    "processStatus": "C" // P:待處理, I:處理中, C:已完成, X:已取消
  }
  ```

#### 3.1.8 執行租賃處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-processes/{processId}/execute`
- **說明**: 執行租賃處理作業
- **請求格式**:
  ```json
  {
    "processResult": "SUCCESS",
    "processMemo": "處理完成"
  }
  ```

#### 3.1.9 審核租賃處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-processes/{processId}/approve`
- **說明**: 審核租賃處理作業
- **請求格式**:
  ```json
  {
    "approvalStatus": "A", // A:已審核, R:已拒絕
    "approvalMemo": "審核通過"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeaseProcessesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lease-processes")]
    [Authorize]
    public class LeaseProcessesController : ControllerBase
    {
        private readonly ILeaseProcessService _service;
        
        public LeaseProcessesController(ILeaseProcessService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseProcessDto>>>> GetLeaseProcesses([FromQuery] LeaseProcessQueryDto query)
        {
            var result = await _service.GetLeaseProcessesAsync(query);
            return Ok(ApiResponse<PagedResult<LeaseProcessDto>>.Success(result));
        }
        
        [HttpGet("{processId}")]
        public async Task<ActionResult<ApiResponse<LeaseProcessDto>>> GetLeaseProcess(string processId)
        {
            var result = await _service.GetLeaseProcessByIdAsync(processId);
            return Ok(ApiResponse<LeaseProcessDto>.Success(result));
        }
        
        [HttpGet("by-lease/{leaseId}")]
        public async Task<ActionResult<ApiResponse<List<LeaseProcessDto>>>> GetLeaseProcessesByLeaseId(string leaseId)
        {
            var result = await _service.GetLeaseProcessesByLeaseIdAsync(leaseId);
            return Ok(ApiResponse<List<LeaseProcessDto>>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLeaseProcess([FromBody] CreateLeaseProcessDto dto)
        {
            var processId = await _service.CreateLeaseProcessAsync(dto);
            return Ok(ApiResponse<string>.Success(processId));
        }
        
        [HttpPut("{processId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLeaseProcess(string processId, [FromBody] UpdateLeaseProcessDto dto)
        {
            await _service.UpdateLeaseProcessAsync(processId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{processId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLeaseProcess(string processId)
        {
            await _service.DeleteLeaseProcessAsync(processId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("{processId}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateStatus(string processId, [FromBody] UpdateLeaseProcessStatusDto dto)
        {
            await _service.UpdateStatusAsync(processId, dto.ProcessStatus);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPost("{processId}/execute")]
        public async Task<ActionResult<ApiResponse>> ExecuteProcess(string processId, [FromBody] ExecuteLeaseProcessDto dto)
        {
            await _service.ExecuteProcessAsync(processId, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPost("{processId}/approve")]
        public async Task<ActionResult<ApiResponse>> ApproveProcess(string processId, [FromBody] ApproveLeaseProcessDto dto)
        {
            await _service.ApproveProcessAsync(processId, dto);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `LeaseProcessService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILeaseProcessService
    {
        Task<PagedResult<LeaseProcessDto>> GetLeaseProcessesAsync(LeaseProcessQueryDto query);
        Task<LeaseProcessDto> GetLeaseProcessByIdAsync(string processId);
        Task<List<LeaseProcessDto>> GetLeaseProcessesByLeaseIdAsync(string leaseId);
        Task<string> CreateLeaseProcessAsync(CreateLeaseProcessDto dto);
        Task UpdateLeaseProcessAsync(string processId, UpdateLeaseProcessDto dto);
        Task DeleteLeaseProcessAsync(string processId);
        Task UpdateStatusAsync(string processId, string processStatus);
        Task ExecuteProcessAsync(string processId, ExecuteLeaseProcessDto dto);
        Task ApproveProcessAsync(string processId, ApproveLeaseProcessDto dto);
    }
}
```

#### 3.2.3 Repository: `LeaseProcessRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ILeaseProcessRepository
    {
        Task<LeaseProcess> GetByIdAsync(string processId);
        Task<PagedResult<LeaseProcess>> GetPagedAsync(LeaseProcessQuery query);
        Task<List<LeaseProcess>> GetByLeaseIdAsync(string leaseId);
        Task<LeaseProcess> CreateAsync(LeaseProcess process);
        Task<LeaseProcess> UpdateAsync(LeaseProcess process);
        Task DeleteAsync(string processId);
        Task<bool> ExistsAsync(string processId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃處理列表頁面 (`LeaseProcessList.vue`)
- **路徑**: `/lease/processes`
- **功能**: 顯示租賃處理列表，支援查詢、新增、修改、刪除、執行、審核
- **主要元件**:
  - 查詢表單 (LeaseProcessSearchForm)
  - 資料表格 (LeaseProcessDataTable)
  - 新增/修改對話框 (LeaseProcessDialog)
  - 執行對話框
  - 審核對話框
  - 刪除確認對話框

#### 4.1.2 租賃處理詳細頁面 (`LeaseProcessDetail.vue`)
- **路徑**: `/lease/processes/:processId`
- **功能**: 顯示租賃處理詳細資料，支援修改、執行、審核

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LeaseProcessSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="處理編號">
      <el-input v-model="searchForm.processId" placeholder="請輸入處理編號" />
    </el-form-item>
    <el-form-item label="租賃編號">
      <el-input v-model="searchForm.leaseId" placeholder="請輸入租賃編號" />
    </el-form-item>
    <el-form-item label="處理類型">
      <el-select v-model="searchForm.processType" placeholder="請選擇處理類型">
        <el-option label="續約" value="RENEWAL" />
        <el-option label="終止" value="TERMINATION" />
        <el-option label="修改" value="MODIFICATION" />
        <el-option label="付款" value="PAYMENT" />
      </el-select>
    </el-form-item>
    <el-form-item label="處理狀態">
      <el-select v-model="searchForm.processStatus" placeholder="請選擇處理狀態">
        <el-option label="待處理" value="P" />
        <el-option label="處理中" value="I" />
        <el-option label="已完成" value="C" />
        <el-option label="已取消" value="X" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`LeaseProcessDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="processList" v-loading="loading">
      <el-table-column prop="processId" label="處理編號" width="150" />
      <el-table-column prop="leaseId" label="租賃編號" width="150" />
      <el-table-column prop="processType" label="處理類型" width="120">
        <template #default="{ row }">
          {{ getProcessTypeText(row.processType) }}
        </template>
      </el-table-column>
      <el-table-column prop="processDate" label="處理日期" width="120" />
      <el-table-column prop="processStatus" label="處理狀態" width="120">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.processStatus)">
            {{ getStatusText(row.processStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="processResult" label="處理結果" width="120">
        <template #default="{ row }">
          <el-tag :type="row.processResult === 'SUCCESS' ? 'success' : row.processResult === 'FAILED' ? 'danger' : 'info'">
            {{ getResultText(row.processResult) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="processUserName" label="處理人員" width="120" />
      <el-table-column label="操作" width="300" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="success" size="small" @click="handleExecute(row)" v-if="row.processStatus === 'P' || row.processStatus === 'I'">執行</el-button>
          <el-button type="warning" size="small" @click="handleApprove(row)" v-if="row.approvalStatus === 'P'">審核</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
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

#### 4.2.3 新增/修改對話框 (`LeaseProcessDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="處理編號" prop="processId">
        <el-input v-model="form.processId" :disabled="isEdit" placeholder="請輸入處理編號" />
      </el-form-item>
      <el-form-item label="租賃編號" prop="leaseId">
        <el-select v-model="form.leaseId" placeholder="請選擇租賃" :disabled="isEdit">
          <el-option v-for="lease in leaseList" :key="lease.leaseId" :label="lease.leaseId" :value="lease.leaseId" />
        </el-select>
      </el-form-item>
      <el-form-item label="處理類型" prop="processType">
        <el-select v-model="form.processType" placeholder="請選擇處理類型">
          <el-option label="續約" value="RENEWAL" />
          <el-option label="終止" value="TERMINATION" />
          <el-option label="修改" value="MODIFICATION" />
          <el-option label="付款" value="PAYMENT" />
        </el-select>
      </el-form-item>
      <el-form-item label="處理日期" prop="processDate">
        <el-date-picker v-model="form.processDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="處理狀態" prop="processStatus">
        <el-select v-model="form.processStatus" placeholder="請選擇處理狀態">
          <el-option label="待處理" value="P" />
          <el-option label="處理中" value="I" />
          <el-option label="已完成" value="C" />
          <el-option label="已取消" value="X" />
        </el-select>
      </el-form-item>
      <el-form-item label="處理備註" prop="processMemo">
        <el-input v-model="form.processMemo" type="textarea" :rows="3" placeholder="請輸入處理備註" />
      </el-form-item>
      <!-- 處理明細 -->
      <el-form-item label="處理明細">
        <el-table :data="form.details" border>
          <el-table-column prop="fieldName" label="欄位名稱" width="150" />
          <el-table-column prop="oldValue" label="舊值" width="200" />
          <el-table-column prop="newValue" label="新值" width="200" />
          <el-table-column label="操作" width="100">
            <template #default="{ row, $index }">
              <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <el-button type="primary" size="small" @click="handleAddDetail" style="margin-top: 10px;">新增明細</el-button>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`leaseProcess.api.ts`)
```typescript
import request from '@/utils/request';

export interface LeaseProcessDto {
  processId: string;
  leaseId: string;
  processType: string;
  processTypeName?: string;
  processDate: string;
  processStatus: string;
  processStatusName?: string;
  processResult?: string;
  processResultName?: string;
  processUserId?: string;
  processUserName?: string;
  processMemo?: string;
  approvalUserId?: string;
  approvalDate?: string;
  approvalStatus?: string;
  details?: LeaseProcessDetailDto[];
  logs?: LeaseProcessLogDto[];
}

export interface LeaseProcessDetailDto {
  fieldName: string;
  oldValue?: string;
  newValue?: string;
  fieldType: string;
}

export interface LeaseProcessLogDto {
  logDate: string;
  logType: string;
  logMessage: string;
  logUserId?: string;
}

export interface LeaseProcessQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    processId?: string;
    leaseId?: string;
    processType?: string;
    processStatus?: string;
    processDateFrom?: string;
    processDateTo?: string;
  };
}

export interface CreateLeaseProcessDto {
  processId: string;
  leaseId: string;
  processType: string;
  processDate: string;
  processStatus: string;
  processMemo?: string;
  details?: LeaseProcessDetailDto[];
}

export interface UpdateLeaseProcessDto extends Omit<CreateLeaseProcessDto, 'processId'> {}

export interface ExecuteLeaseProcessDto {
  processResult: string;
  processMemo?: string;
}

export interface ApproveLeaseProcessDto {
  approvalStatus: string;
  approvalMemo?: string;
}

// API 函數
export const getLeaseProcessList = (query: LeaseProcessQueryDto) => {
  return request.get<ApiResponse<PagedResult<LeaseProcessDto>>>('/api/v1/lease-processes', { params: query });
};

export const getLeaseProcessById = (processId: string) => {
  return request.get<ApiResponse<LeaseProcessDto>>(`/api/v1/lease-processes/${processId}`);
};

export const getLeaseProcessesByLeaseId = (leaseId: string) => {
  return request.get<ApiResponse<LeaseProcessDto[]>>(`/api/v1/lease-processes/by-lease/${leaseId}`);
};

export const createLeaseProcess = (data: CreateLeaseProcessDto) => {
  return request.post<ApiResponse<string>>('/api/v1/lease-processes', data);
};

export const updateLeaseProcess = (processId: string, data: UpdateLeaseProcessDto) => {
  return request.put<ApiResponse>(`/api/v1/lease-processes/${processId}`, data);
};

export const deleteLeaseProcess = (processId: string) => {
  return request.delete<ApiResponse>(`/api/v1/lease-processes/${processId}`);
};

export const updateStatus = (processId: string, processStatus: string) => {
  return request.put<ApiResponse>(`/api/v1/lease-processes/${processId}/status`, { processStatus });
};

export const executeProcess = (processId: string, data: ExecuteLeaseProcessDto) => {
  return request.post<ApiResponse>(`/api/v1/lease-processes/${processId}/execute`, data);
};

export const approveProcess = (processId: string, data: ApproveLeaseProcessDto) => {
  return request.post<ApiResponse>(`/api/v1/lease-processes/${processId}/approve`, data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 處理邏輯實作
- [ ] 審核邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 執行對話框開發
- [ ] 審核對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 租賃處理資料變更必須記錄操作日誌
- 敏感資訊必須加密儲存
- 審核功能必須有權限控制

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 關聯查詢應使用 JOIN 優化
- 處理日誌查詢應使用分頁

### 6.3 資料驗證
- 必填欄位必須驗證
- 狀態值必須在允許範圍內
- 日期範圍必須驗證
- 處理類型必須在允許範圍內

### 6.4 業務邏輯
- 刪除租賃處理前必須檢查是否有關聯的明細和日誌資料
- 狀態變更必須符合業務流程
- 處理執行必須記錄處理日誌
- 審核必須記錄審核日誌
- 處理結果必須記錄

### 6.5 關聯資料
- 與租賃的關聯
- 與處理明細的關聯
- 與處理日誌的關聯

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增租賃處理成功
- [ ] 新增租賃處理失敗 (必填欄位驗證)
- [ ] 修改租賃處理成功
- [ ] 修改租賃處理失敗 (狀態驗證)
- [ ] 刪除租賃處理成功
- [ ] 刪除租賃處理失敗 (有關聯資料)
- [ ] 查詢租賃處理列表成功
- [ ] 查詢單筆租賃處理成功
- [ ] 根據租賃編號查詢處理列表成功
- [ ] 更新租賃處理狀態成功
- [ ] 執行租賃處理成功
- [ ] 審核租賃處理成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯資料測試
- [ ] 處理流程測試
- [ ] 審核流程測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 處理執行效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FU.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FD.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_FB.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50_PR.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B60_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B90_FI.ASP`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYS8000/SYS8B50.xsd` (如果存在)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

