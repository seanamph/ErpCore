# SYST131-SYST134 - 會計帳簿管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYST131-SYST134 系列
- **功能名稱**: 會計帳簿管理系列（現金流量分類設定）
- **功能描述**: 提供現金流量分類資料的新增、修改、刪除、查詢功能，包含大分類、中分類、科目設定、小計設定等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYST000/SYST131_*.ASP` (現金流量大分類設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST132_*.ASP` (現金流量中分類設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST133_*.ASP` (現金流量科目設定)
  - `WEB/IMS_CORE/ASP/SYST000/SYST134_*.ASP` (現金流量小計設定)

### 1.2 業務需求
- 管理現金流量大分類資料（SYST131）
- 管理現金流量中分類資料（SYST132）
- 管理現金流量科目設定資料（SYST133）
- 管理現金流量小計設定資料（SYST134）
- 支援階層式分類結構（大分類→中分類→科目→小計）
- 支援借貸項目設定
- 支援排序功能
- 支援多筆新增功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CashFlowLargeTypes` (對應舊系統 `CASH_LTYPE`)

```sql
CREATE TABLE [dbo].[CashFlowLargeTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
    [CashLTypeName] NVARCHAR(20) NOT NULL, -- 大分類名稱 (CASH_LTYPE_NAME)
    [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
    [Sn] NVARCHAR(10) NULL, -- 排序 (SN)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_CashFlowLargeTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CashFlowLargeTypes_CashLTypeId] UNIQUE ([CashLTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_CashLTypeId] ON [dbo].[CashFlowLargeTypes] ([CashLTypeId]);
CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_AbItem] ON [dbo].[CashFlowLargeTypes] ([AbItem]);
CREATE NONCLUSTERED INDEX [IX_CashFlowLargeTypes_Sn] ON [dbo].[CashFlowLargeTypes] ([Sn]);
```

### 2.2 主要資料表: `CashFlowMediumTypes` (對應舊系統 `CASH_MTYPE`)

```sql
CREATE TABLE [dbo].[CashFlowMediumTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
    [CashMTypeId] NVARCHAR(2) NOT NULL, -- 中分類代號 (CASH_MTYPE_ID)
    [CashMTypeName] NVARCHAR(50) NULL, -- 中分類名稱 (CASH_MTYPE_NAME)
    [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
    [Sn] NVARCHAR(10) NULL, -- 排序 (SN)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_CashFlowMediumTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CashFlowMediumTypes] UNIQUE ([CashLTypeId], [CashMTypeId]),
    CONSTRAINT [FK_CashFlowMediumTypes_CashFlowLargeTypes] FOREIGN KEY ([CashLTypeId]) REFERENCES [dbo].[CashFlowLargeTypes] ([CashLTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CashFlowMediumTypes_CashLTypeId] ON [dbo].[CashFlowMediumTypes] ([CashLTypeId]);
CREATE NONCLUSTERED INDEX [IX_CashFlowMediumTypes_CashMTypeId] ON [dbo].[CashFlowMediumTypes] ([CashMTypeId]);
```

### 2.3 主要資料表: `CashFlowSubjectTypes` (對應舊系統 `CASH_STYPE`)

```sql
CREATE TABLE [dbo].[CashFlowSubjectTypes] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CashMTypeId] NVARCHAR(2) NOT NULL, -- 中分類代號 (CASH_MTYPE_ID)
    [CashSTypeId] NVARCHAR(50) NOT NULL, -- 科目代號 (CASH_STYPE_ID, 對應STYPE.STYPE_ID)
    [AbItem] NVARCHAR(1) NULL, -- 借貸項目 (AB_ITEM, A:借方, B:貸方)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_CashFlowSubjectTypes] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CashFlowSubjectTypes] UNIQUE ([CashMTypeId], [CashSTypeId]),
    CONSTRAINT [FK_CashFlowSubjectTypes_CashFlowMediumTypes] FOREIGN KEY ([CashMTypeId]) REFERENCES [dbo].[CashFlowMediumTypes] ([CashMTypeId]),
    CONSTRAINT [FK_CashFlowSubjectTypes_AccountSubjects] FOREIGN KEY ([CashSTypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CashFlowSubjectTypes_CashMTypeId] ON [dbo].[CashFlowSubjectTypes] ([CashMTypeId]);
CREATE NONCLUSTERED INDEX [IX_CashFlowSubjectTypes_CashSTypeId] ON [dbo].[CashFlowSubjectTypes] ([CashSTypeId]);
```

### 2.4 主要資料表: `CashFlowSubTotals` (對應舊系統 `CASH_SUB`)

```sql
CREATE TABLE [dbo].[CashFlowSubTotals] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CashLTypeId] NVARCHAR(1) NOT NULL, -- 大分類代號 (CASH_LTYPE_ID)
    [CashSubId] NVARCHAR(10) NOT NULL, -- 小計代號 (CASH_SUB_ID)
    [CashSubName] NVARCHAR(50) NOT NULL, -- 小計名稱 (CASH_SUB_NAME)
    [CashMTypeIdB] NVARCHAR(2) NULL, -- 中分類代號起 (CASH_MTYPE_ID_B)
    [CashMTypeIdE] NVARCHAR(2) NULL, -- 中分類代號迄 (CASH_MTYPE_ID_E)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_CashFlowSubTotals] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CashFlowSubTotals] UNIQUE ([CashLTypeId], [CashSubId]),
    CONSTRAINT [FK_CashFlowSubTotals_CashFlowLargeTypes] FOREIGN KEY ([CashLTypeId]) REFERENCES [dbo].[CashFlowLargeTypes] ([CashLTypeId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CashFlowSubTotals_CashLTypeId] ON [dbo].[CashFlowSubTotals] ([CashLTypeId]);
CREATE NONCLUSTERED INDEX [IX_CashFlowSubTotals_CashSubId] ON [dbo].[CashFlowSubTotals] ([CashSubId]);
```

### 2.5 資料字典

#### CashFlowLargeTypes 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| CashLTypeId | NVARCHAR | 1 | NO | - | 大分類代號 | 唯一，主鍵候選 |
| CashLTypeName | NVARCHAR | 20 | NO | - | 大分類名稱 | - |
| AbItem | NVARCHAR | 1 | YES | - | 借貸項目 | A:借方, B:貸方 |
| Sn | NVARCHAR | 10 | YES | - | 排序 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 SYST131 - 現金流量大分類設定 API

#### 3.1.1 查詢現金流量大分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/large-types`
- **說明**: 查詢現金流量大分類列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CashLTypeId",
    "sortOrder": "ASC",
    "filters": {
      "cashLTypeId": "",
      "cashLTypeName": "",
      "abItem": ""
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
          "cashLTypeId": "1",
          "cashLTypeName": "營業活動",
          "abItem": "A",
          "sn": "1"
        }
      ],
      "totalCount": 10,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆現金流量大分類
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/large-types/{cashLTypeId}`
- **說明**: 根據大分類代號查詢單筆資料
- **路徑參數**:
  - `cashLTypeId`: 大分類代號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增現金流量大分類
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cash-flow/large-types`
- **說明**: 新增現金流量大分類資料
- **請求格式**:
  ```json
  {
    "cashLTypeId": "1",
    "cashLTypeName": "營業活動",
    "abItem": "A",
    "sn": "1"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "cashLTypeId": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改現金流量大分類
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cash-flow/large-types/{cashLTypeId}`
- **說明**: 修改現金流量大分類資料
- **路徑參數**:
  - `cashLTypeId`: 大分類代號
- **請求格式**: 同新增，但 `cashLTypeId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除現金流量大分類
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cash-flow/large-types/{cashLTypeId}`
- **說明**: 刪除現金流量大分類資料（需檢查是否有中分類資料）
- **路徑參數**:
  - `cashLTypeId`: 大分類代號
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

#### 3.1.6 檢查大分類代號是否存在
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/large-types/{cashLTypeId}/exists`
- **說明**: 檢查大分類代號是否已存在
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "exists": true
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 SYST132 - 現金流量中分類設定 API

#### 3.2.1 查詢現金流量中分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/medium-types`
- **說明**: 查詢現金流量中分類列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CashMTypeId",
    "sortOrder": "ASC",
    "filters": {
      "cashLTypeId": "",
      "cashMTypeId": "",
      "cashMTypeName": "",
      "abItem": ""
    }
  }
  ```

#### 3.2.2 新增現金流量中分類
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cash-flow/medium-types`
- **請求格式**:
  ```json
  {
    "cashLTypeId": "1",
    "cashMTypeId": "01",
    "cashMTypeName": "銷貨收入",
    "abItem": "A",
    "sn": "1"
  }
  ```

#### 3.2.3 修改現金流量中分類
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cash-flow/medium-types/{cashLTypeId}/{cashMTypeId}`

#### 3.2.4 刪除現金流量中分類
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cash-flow/medium-types/{cashLTypeId}/{cashMTypeId}`

### 3.3 SYST133 - 現金流量科目設定 API

#### 3.3.1 查詢現金流量科目列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/subject-types`
- **說明**: 查詢現金流量科目設定列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "cashMTypeId": "",
      "cashSTypeId": ""
    }
  }
  ```

#### 3.3.2 新增現金流量科目設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cash-flow/subject-types`
- **請求格式**:
  ```json
  {
    "cashMTypeId": "01",
    "cashSTypeId": "1000",
    "abItem": "A"
  }
  ```

#### 3.3.3 修改現金流量科目設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cash-flow/subject-types/{cashMTypeId}/{cashSTypeId}`

#### 3.3.4 刪除現金流量科目設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cash-flow/subject-types/{cashMTypeId}/{cashSTypeId}`

### 3.4 SYST134 - 現金流量小計設定 API

#### 3.4.1 查詢現金流量小計列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cash-flow/sub-totals`
- **說明**: 查詢現金流量小計設定列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "cashLTypeId": "",
      "cashSubId": ""
    }
  }
  ```

#### 3.4.2 新增現金流量小計設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cash-flow/sub-totals`
- **請求格式**:
  ```json
  {
    "cashLTypeId": "1",
    "cashSubId": "001",
    "cashSubName": "小計1",
    "cashMTypeIdB": "01",
    "cashMTypeIdE": "05"
  }
  ```

#### 3.4.3 修改現金流量小計設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cash-flow/sub-totals/{cashLTypeId}/{cashSubId}`

#### 3.4.4 刪除現金流量小計設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cash-flow/sub-totals/{cashLTypeId}/{cashSubId}`

### 3.5 後端實作類別

#### 3.5.1 Controller: `CashFlowLargeTypesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/cash-flow/large-types")]
    [Authorize]
    public class CashFlowLargeTypesController : ControllerBase
    {
        private readonly ICashFlowLargeTypeService _service;
        
        public CashFlowLargeTypesController(ICashFlowLargeTypeService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CashFlowLargeTypeDto>>>> GetLargeTypes([FromQuery] CashFlowLargeTypeQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{cashLTypeId}")]
        public async Task<ActionResult<ApiResponse<CashFlowLargeTypeDto>>> GetLargeType(string cashLTypeId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLargeType([FromBody] CreateCashFlowLargeTypeDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{cashLTypeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLargeType(string cashLTypeId, [FromBody] UpdateCashFlowLargeTypeDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{cashLTypeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLargeType(string cashLTypeId)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("{cashLTypeId}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckExists(string cashLTypeId)
        {
            // 實作檢查邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 現金流量大分類列表頁面 (`CashFlowLargeTypeList.vue`)
- **路徑**: `/accounting/cash-flow/large-types`
- **功能**: 顯示現金流量大分類列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (CashFlowLargeTypeSearchForm)
  - 資料表格 (CashFlowLargeTypeDataTable)
  - 新增/修改對話框 (CashFlowLargeTypeDialog)
  - 刪除確認對話框

#### 4.1.2 現金流量中分類列表頁面 (`CashFlowMediumTypeList.vue`)
- **路徑**: `/accounting/cash-flow/medium-types`
- **功能**: 顯示現金流量中分類列表，支援查詢、新增、修改、刪除

#### 4.1.3 現金流量科目設定頁面 (`CashFlowSubjectTypeList.vue`)
- **路徑**: `/accounting/cash-flow/subject-types`
- **功能**: 顯示現金流量科目設定列表，支援查詢、新增、修改、刪除

#### 4.1.4 現金流量小計設定頁面 (`CashFlowSubTotalList.vue`)
- **路徑**: `/accounting/cash-flow/sub-totals`
- **功能**: 顯示現金流量小計設定列表，支援查詢、新增、修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`CashFlowLargeTypeSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="大分類代號">
      <el-input v-model="searchForm.cashLTypeId" placeholder="請輸入大分類代號" />
    </el-form-item>
    <el-form-item label="大分類名稱">
      <el-input v-model="searchForm.cashLTypeName" placeholder="請輸入大分類名稱" />
    </el-form-item>
    <el-form-item label="借貸項目">
      <el-select v-model="searchForm.abItem" placeholder="請選擇">
        <el-option label="借方" value="A" />
        <el-option label="貸方" value="B" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`CashFlowLargeTypeDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="largeTypeList" v-loading="loading">
      <el-table-column prop="cashLTypeId" label="大分類代號" width="120" />
      <el-table-column prop="cashLTypeName" label="大分類名稱" width="200" />
      <el-table-column prop="abItem" label="借貸項目" width="100">
        <template #default="{ row }">
          <el-tag :type="row.abItem === 'A' ? 'success' : 'danger'">
            {{ row.abItem === 'A' ? '借方' : '貸方' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="sn" label="排序" width="100" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
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

#### 4.2.3 新增/修改對話框 (`CashFlowLargeTypeDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
      <el-form-item label="大分類代號" prop="cashLTypeId">
        <el-input v-model="form.cashLTypeId" :disabled="isEdit" placeholder="請輸入大分類代號" maxlength="1" />
      </el-form-item>
      <el-form-item label="大分類名稱" prop="cashLTypeName">
        <el-input v-model="form.cashLTypeName" placeholder="請輸入大分類名稱" maxlength="20" />
      </el-form-item>
      <el-form-item label="借貸項目" prop="abItem">
        <el-select v-model="form.abItem" placeholder="請選擇">
          <el-option label="借方" value="A" />
          <el-option label="貸方" value="B" />
        </el-select>
      </el-form-item>
      <el-form-item label="排序" prop="sn">
        <el-input v-model="form.sn" placeholder="請輸入排序" maxlength="10" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`cashFlow.api.ts`)
```typescript
import request from '@/utils/request';

export interface CashFlowLargeTypeDto {
  tKey: number;
  cashLTypeId: string;
  cashLTypeName: string;
  abItem?: string;
  sn?: string;
}

export interface CashFlowLargeTypeQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    cashLTypeId?: string;
    cashLTypeName?: string;
    abItem?: string;
  };
}

export interface CreateCashFlowLargeTypeDto {
  cashLTypeId: string;
  cashLTypeName: string;
  abItem?: string;
  sn?: string;
}

export interface UpdateCashFlowLargeTypeDto extends Omit<CreateCashFlowLargeTypeDto, 'cashLTypeId'> {}

// API 函數
export const getCashFlowLargeTypeList = (query: CashFlowLargeTypeQueryDto) => {
  return request.get<ApiResponse<PagedResult<CashFlowLargeTypeDto>>>('/api/v1/cash-flow/large-types', { params: query });
};

export const getCashFlowLargeTypeById = (cashLTypeId: string) => {
  return request.get<ApiResponse<CashFlowLargeTypeDto>>(`/api/v1/cash-flow/large-types/${cashLTypeId}`);
};

export const createCashFlowLargeType = (data: CreateCashFlowLargeTypeDto) => {
  return request.post<ApiResponse<string>>('/api/v1/cash-flow/large-types', data);
};

export const updateCashFlowLargeType = (cashLTypeId: string, data: UpdateCashFlowLargeTypeDto) => {
  return request.put<ApiResponse>(`/api/v1/cash-flow/large-types/${cashLTypeId}`, data);
};

export const deleteCashFlowLargeType = (cashLTypeId: string) => {
  return request.delete<ApiResponse>(`/api/v1/cash-flow/large-types/${cashLTypeId}`);
};

export const checkCashFlowLargeTypeExists = (cashLTypeId: string) => {
  return request.get<ApiResponse<boolean>>(`/api/v1/cash-flow/large-types/${cashLTypeId}/exists`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構（4個主要資料表）
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立唯一約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立（4個實體）
- [ ] Repository 實作（4個儲存庫）
- [ ] Service 實作（4個服務）
- [ ] Controller 實作（4個控制器）
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（代號唯一性、階層關係驗證等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數（4個功能模組）
- [ ] 列表頁面開發（4個頁面）
- [ ] 新增/修改對話框開發（4個對話框）
- [ ] 查詢表單開發（4個表單）
- [ ] 資料表格開發（4個表格）
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 業務邏輯測試（階層關係驗證、刪除檢查等）

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 17天

---

## 六、注意事項

### 6.1 業務邏輯
- 大分類代號必須唯一（長度1碼）
- 中分類代號必須在大分類下唯一（長度2碼）
- 科目設定必須對應到有效的會計科目
- 小計設定必須指定有效的中分類範圍
- 刪除大分類時，需檢查是否有中分類資料
- 刪除中分類時，需檢查是否有科目設定資料
- 階層關係必須正確維護

### 6.2 資料驗證
- 大分類代號必須唯一
- 中分類代號必須在大分類下唯一
- 科目代號必須存在於會計科目主檔
- 必填欄位必須驗證
- 借貸項目必須為 A 或 B

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 階層查詢需要優化

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增大分類成功
- [ ] 新增大分類失敗 (重複代號)
- [ ] 修改大分類成功
- [ ] 修改大分類失敗 (不存在)
- [ ] 刪除大分類成功
- [ ] 刪除大分類失敗 (有中分類資料)
- [ ] 查詢大分類列表成功
- [ ] 查詢單筆大分類成功
- [ ] 中分類、科目設定、小計設定的類似測試案例

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 階層關係測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FI.ASP` - 大分類新增畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FU.ASP` - 大分類修改畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FD.ASP` - 大分類刪除畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FQ.ASP` - 大分類查詢畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FB.ASP` - 大分類瀏覽畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST131_FS.ASP` - 大分類排序畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST132_*.ASP` - 中分類相關畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST133_*.ASP` - 科目設定相關畫面
- `WEB/IMS_CORE/ASP/SYST000/SYST134_*.ASP` - 小計設定相關畫面

### 8.2 資料庫 Schema
- 舊系統資料表：`CASH_LTYPE`, `CASH_MTYPE`, `CASH_STYPE`, `CASH_SUB`
- 主要欄位：
  - CASH_LTYPE: CASH_LTYPE_ID, CASH_LTYPE_NAME, AB_ITEM, SN
  - CASH_MTYPE: CASH_LTYPE_ID, CASH_MTYPE_ID, CASH_MTYPE_NAME, AB_ITEM, SN
  - CASH_STYPE: CASH_MTYPE_ID, CASH_STYPE_ID, AB_ITEM
  - CASH_SUB: CASH_LTYPE_ID, CASH_SUB_ID, CASH_SUB_NAME, CASH_MTYPE_ID_B, CASH_MTYPE_ID_E

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

