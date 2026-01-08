# XCOM250 - 建立Partition Table作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM250
- **功能名稱**: 建立Partition Table作業
- **功能描述**: 提供資料庫分割表(Partition Table)的建立與管理功能，支援按月建立分割區，包含PROD_SALES_M、PROD_SALES_D、PAY_TYPE、DISC_T、POSREPORT、OTHER_RECORD、INVOICE_TMP等資料表的分割區管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM250_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM250_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理資料庫分割表的建立
- 支援按月自動建立分割區
- 支援多個資料表的分割區管理
- 支援分割區的刪除與重建
- 支援分割區查詢與瀏覽
- 支援預設分割區管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TablePartitions` (分割表管理)

```sql
CREATE TABLE [dbo].[TablePartitions] (
    [PartitionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TableName] NVARCHAR(100) NOT NULL, -- 資料表名稱
    [PartitionName] NVARCHAR(100) NOT NULL, -- 分割區名稱
    [PartitionDate] DATE NOT NULL, -- 分割區日期
    [PartitionType] NVARCHAR(20) NOT NULL DEFAULT 'RANGE', -- 分割類型 (RANGE, LIST, HASH)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_TablePartitions_TableName_PartitionName] UNIQUE ([TableName], [PartitionName])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TablePartitions_TableName] ON [dbo].[TablePartitions] ([TableName]);
CREATE NONCLUSTERED INDEX [IX_TablePartitions_PartitionDate] ON [dbo].[TablePartitions] ([PartitionDate]);
CREATE NONCLUSTERED INDEX [IX_TablePartitions_Status] ON [dbo].[TablePartitions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `PartitionLogs` - 分割區操作記錄
```sql
CREATE TABLE [dbo].[PartitionLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TableName] NVARCHAR(100) NOT NULL,
    [PartitionName] NVARCHAR(100) NULL,
    [OperationType] NVARCHAR(20) NOT NULL, -- CREATE, DROP, ALTER
    [OperationSql] NVARCHAR(MAX) NULL, -- 執行的SQL語句
    [Status] NVARCHAR(10) NOT NULL, -- SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PartitionLogs_TableName] ON [dbo].[PartitionLogs] ([TableName]);
CREATE NONCLUSTERED INDEX [IX_PartitionLogs_CreatedAt] ON [dbo].[PartitionLogs] ([CreatedAt]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PartitionId | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| TableName | NVARCHAR | 100 | NO | - | 資料表名稱 | 必填 |
| PartitionName | NVARCHAR | 100 | NO | - | 分割區名稱 | 必填，唯一 |
| PartitionDate | DATE | - | NO | - | 分割區日期 | 必填 |
| PartitionType | NVARCHAR | 20 | NO | 'RANGE' | 分割類型 | RANGE, LIST, HASH |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢分割區列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom250/partitions`
- **說明**: 查詢分割區列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "PartitionDate",
    "sortOrder": "DESC",
    "filters": {
      "tableName": "",
      "partitionName": "",
      "status": "",
      "partitionDateFrom": "",
      "partitionDateTo": ""
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
          "partitionId": 1,
          "tableName": "PROD_SALES_M",
          "partitionName": "PROD_SALES_M240101",
          "partitionDate": "2024-01-01",
          "partitionType": "RANGE",
          "status": "A",
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

#### 3.1.2 建立分割區
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom250/partitions/create`
- **說明**: 建立資料表分割區
- **請求格式**:
  ```json
  {
    "tableName": "PROD_SALES_M",
    "year": 2024,
    "month": 1,
    "createDefault": true
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "分割區建立成功",
    "data": {
      "createdPartitions": [
        {
          "partitionName": "PROD_SALES_M240101",
          "partitionDate": "2024-01-01",
          "status": "SUCCESS"
        }
      ],
      "totalCreated": 31
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 批次建立分割區
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom250/partitions/batch-create`
- **說明**: 批次建立多個資料表的分割區
- **請求格式**:
  ```json
  {
    "tableNames": ["PROD_SALES_M", "PROD_SALES_D", "PAY_TYPE"],
    "year": 2024,
    "month": 1,
    "createDefault": true
  }
  ```
- **回應格式**: 標準批次操作回應格式

#### 3.1.4 刪除分割區
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom250/partitions/{partitionId}`
- **說明**: 刪除指定的分割區
- **路徑參數**:
  - `partitionId`: 分割區ID
- **回應格式**: 標準刪除回應格式

#### 3.1.5 查詢分割區操作記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom250/partition-logs`
- **說明**: 查詢分割區操作記錄
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom250Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom250")]
    [Authorize]
    public class XCom250Controller : ControllerBase
    {
        private readonly IXCom250Service _service;
        
        public XCom250Controller(IXCom250Service service)
        {
            _service = service;
        }
        
        [HttpGet("partitions")]
        public async Task<ActionResult<ApiResponse<PagedResult<PartitionDto>>>> GetPartitions([FromQuery] PartitionQueryDto query)
        {
            var result = await _service.GetPartitionsAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("partitions/create")]
        public async Task<ActionResult<ApiResponse<CreatePartitionResultDto>>> CreatePartitions([FromBody] CreatePartitionDto dto)
        {
            var result = await _service.CreatePartitionsAsync(dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("partitions/batch-create")]
        public async Task<ActionResult<ApiResponse<BatchCreatePartitionResultDto>>> BatchCreatePartitions([FromBody] BatchCreatePartitionDto dto)
        {
            var result = await _service.BatchCreatePartitionsAsync(dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpDelete("partitions/{partitionId}")]
        public async Task<ActionResult<ApiResponse>> DeletePartition(long partitionId)
        {
            await _service.DeletePartitionAsync(partitionId);
            return Ok(ApiResponse.Success());
        }
        
        [HttpGet("partition-logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<PartitionLogDto>>>> GetPartitionLogs([FromQuery] PartitionLogQueryDto query)
        {
            var result = await _service.GetPartitionLogsAsync(query);
            return Ok(ApiResponse.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `XCom250Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom250Service
    {
        Task<PagedResult<PartitionDto>> GetPartitionsAsync(PartitionQueryDto query);
        Task<CreatePartitionResultDto> CreatePartitionsAsync(CreatePartitionDto dto);
        Task<BatchCreatePartitionResultDto> BatchCreatePartitionsAsync(BatchCreatePartitionDto dto);
        Task DeletePartitionAsync(long partitionId);
        Task<PagedResult<PartitionLogDto>> GetPartitionLogsAsync(PartitionLogQueryDto query);
    }
}
```

#### 3.2.3 Repository: `PartitionRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IPartitionRepository
    {
        Task<PagedResult<TablePartition>> GetPartitionsAsync(PartitionQuery query);
        Task<TablePartition> CreatePartitionAsync(TablePartition partition);
        Task DeletePartitionAsync(long partitionId);
        Task<bool> PartitionExistsAsync(string tableName, string partitionName);
        Task LogPartitionOperationAsync(PartitionLog log);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 分割區管理頁面 (`PartitionManagement.vue`)
- **路徑**: `/xcom/partitions`
- **功能**: 顯示分割區列表，支援查詢、建立、刪除
- **主要元件**:
  - 查詢表單 (PartitionSearchForm)
  - 資料表格 (PartitionDataTable)
  - 建立分割區對話框 (CreatePartitionDialog)
  - 刪除確認對話框

#### 4.1.2 分割區操作記錄頁面 (`PartitionLogs.vue`)
- **路徑**: `/xcom/partitions/logs`
- **功能**: 顯示分割區操作記錄

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`PartitionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="資料表名稱">
      <el-select v-model="searchForm.tableName" placeholder="請選擇資料表">
        <el-option label="PROD_SALES_M" value="PROD_SALES_M" />
        <el-option label="PROD_SALES_D" value="PROD_SALES_D" />
        <el-option label="PAY_TYPE" value="PAY_TYPE" />
        <el-option label="DISC_T" value="DISC_T" />
        <el-option label="POSREPORT" value="POSREPORT" />
        <el-option label="OTHER_RECORD" value="OTHER_RECORD" />
        <el-option label="INVOICE_TMP" value="INVOICE_TMP" />
      </el-select>
    </el-form-item>
    <el-form-item label="分割區日期">
      <el-date-picker
        v-model="searchForm.partitionDateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
      />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 建立分割區對話框 (`CreatePartitionDialog.vue`)
```vue
<template>
  <el-dialog
    title="建立分割區"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="資料表名稱" prop="tableName">
        <el-select v-model="form.tableName" placeholder="請選擇資料表">
          <el-option label="PROD_SALES_M" value="PROD_SALES_M" />
          <el-option label="PROD_SALES_D" value="PROD_SALES_D" />
          <el-option label="PAY_TYPE" value="PAY_TYPE" />
          <el-option label="DISC_T" value="DISC_T" />
          <el-option label="POSREPORT" value="POSREPORT" />
          <el-option label="OTHER_RECORD" value="OTHER_RECORD" />
          <el-option label="INVOICE_TMP" value="INVOICE_TMP" />
          <el-option label="全部資料表" value="ALL_TABLES" />
        </el-select>
      </el-form-item>
      <el-form-item label="年份" prop="year">
        <el-input-number v-model="form.year" :min="2000" :max="2100" />
      </el-form-item>
      <el-form-item label="月份" prop="month">
        <el-input-number v-model="form.month" :min="1" :max="12" />
      </el-form-item>
      <el-form-item label="建立預設分割區">
        <el-switch v-model="form.createDefault" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="loading">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom250.api.ts`)
```typescript
import request from '@/utils/request';

export interface PartitionDto {
  partitionId: number;
  tableName: string;
  partitionName: string;
  partitionDate: string;
  partitionType: string;
  status: string;
  createdAt: string;
}

export interface CreatePartitionDto {
  tableName: string;
  year: number;
  month: number;
  createDefault: boolean;
}

export interface BatchCreatePartitionDto {
  tableNames: string[];
  year: number;
  month: number;
  createDefault: boolean;
}

// API 函數
export const getPartitions = (query: any) => {
  return request.get<ApiResponse<PagedResult<PartitionDto>>>('/api/v1/xcom250/partitions', { params: query });
};

export const createPartitions = (data: CreatePartitionDto) => {
  return request.post<ApiResponse<CreatePartitionResultDto>>('/api/v1/xcom250/partitions/create', data);
};

export const batchCreatePartitions = (data: BatchCreatePartitionDto) => {
  return request.post<ApiResponse<BatchCreatePartitionResultDto>>('/api/v1/xcom250/partitions/batch-create', data);
};

export const deletePartition = (partitionId: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom250/partitions/${partitionId}`);
};

export const getPartitionLogs = (query: any) => {
  return request.get<ApiResponse<PagedResult<PartitionLogDto>>>('/api/v1/xcom250/partition-logs', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立分割區管理邏輯
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含SQL Server分割區建立邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 建立分割區對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 分割區建立測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 8天

---

## 六、注意事項

### 6.1 安全性
- 分割區建立操作必須有管理員權限
- 必須記錄所有分割區操作
- 必須驗證資料表名稱的合法性
- 必須防止SQL注入攻擊

### 6.2 效能
- 分割區建立操作可能耗時較長，必須使用非同步處理
- 必須提供進度回報機制
- 必須優化分割區查詢效能

### 6.3 資料驗證
- 資料表名稱必須在允許清單中
- 年份和月份必須在合理範圍內
- 必須檢查分割區是否已存在

### 6.4 業務邏輯
- SQL Server分割區建立語法與Oracle不同，需要適配
- 必須處理分割區建立失敗的情況
- 必須支援預設分割區的管理
- 必須記錄所有操作日誌

### 6.5 SQL Server 分割區語法
- SQL Server使用`ON`子句指定分割區配置
- 需要先建立分割函數(Partition Function)和分割配置(Partition Scheme)
- 使用`ALTER TABLE ... SWITCH`進行分割區切換
- 使用`ALTER TABLE ... MERGE`進行分割區合併

---

## 七、測試案例

### 7.1 單元測試
- [ ] 建立分割區成功
- [ ] 建立分割區失敗 (資料表不存在)
- [ ] 批次建立分割區成功
- [ ] 刪除分割區成功
- [ ] 查詢分割區列表成功

### 7.2 整合測試
- [ ] 完整分割區建立流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量分割區建立測試
- [ ] 分割區查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM250_FI.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM250_FQ.ASP`

### 8.2 SQL Server 分割區文件
- SQL Server Partitioning Documentation
- SQL Server Partition Function and Scheme

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

