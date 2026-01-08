# SYSL130 - 業務報表查詢作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL130
- **功能名稱**: 業務報表查詢作業
- **功能描述**: 提供業務報表資料的新增、修改、刪除、查詢功能，包含員工餐卡申請、審核、異動等業務報表管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/sysl130.ascx`
  - `WEB/IMS_CORE/SYSL000/0. Backup/2023.01.04 (可視權限)/sysl130.ascx.7f09223a.compiled`
  - `WEB/IMS_CORE/SYSL000/js/SYSL130.js`
  - `WEB/IMS_CORE/SYSL000/0. Backup/2022.11.11 (DB Procedure 更新新建立單據SP)/DB/IMS_SR.SP_SCHEDULE_EMP.txt`

### 1.2 業務需求
- 管理員工餐卡申請資料
- 支援餐卡申請的新增、修改、刪除、查詢
- 支援餐卡審核流程
- 支援餐卡異動記錄
- 支援檔案匯入功能
- 支援批次審核功能
- 支援報表列印功能
- 記錄餐卡申請的建立與變更資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EmployeeMealCards` (對應舊系統 `IMS_SR.EMP_M`)

```sql
CREATE TABLE [dbo].[EmployeeMealCards] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [EmpId] NVARCHAR(50) NOT NULL, -- 員工編號 (EMP_ID)
    [EmpName] NVARCHAR(100) NULL, -- 員工姓名 (EMP_NAME)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [SiteId] NVARCHAR(50) NULL, -- 店別代碼 (SITE_ID)
    [CardType] NVARCHAR(20) NULL, -- 卡片類型 (CARD_TYPE)
    [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
    [ActionTypeD] NVARCHAR(20) NULL, -- 動作類型明細 (ACTION_TYPE_D)
    [StartDate] DATETIME2 NULL, -- 起始日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS) P:待審核, A:已審核, R:已拒絕
    [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
    [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [TxnNo] NVARCHAR(50) NULL, -- 交易單號 (TXN_NO)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_EmployeeMealCards] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_EmpId] ON [dbo].[EmployeeMealCards] ([EmpId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_OrgId] ON [dbo].[EmployeeMealCards] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_SiteId] ON [dbo].[EmployeeMealCards] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_Status] ON [dbo].[EmployeeMealCards] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_TxnNo] ON [dbo].[EmployeeMealCards] ([TxnNo]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_StartDate] ON [dbo].[EmployeeMealCards] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCards_EndDate] ON [dbo].[EmployeeMealCards] ([EndDate]);
```

### 2.2 相關資料表

#### 2.2.1 `MainApplyMaster` - 主申請單主檔
```sql
CREATE TABLE [dbo].[MainApplyMaster] (
    [TxnNo] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 交易單號
    [ApplyType] NVARCHAR(20) NOT NULL, -- 申請類型
    [ApplyDate] DATETIME2 NOT NULL, -- 申請日期
    [EmpId] NVARCHAR(50) NOT NULL, -- 申請人
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態
    [Verifier] NVARCHAR(50) NULL, -- 審核者
    [VerifyDate] DATETIME2 NULL, -- 審核日期
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.2 `CardType` - 卡片類型主檔
```sql
CREATE TABLE [dbo].[CardType] (
    [CardId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 卡片類型代碼
    [CardName] NVARCHAR(100) NOT NULL, -- 卡片類型名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.3 `ActionType` - 動作類型主檔
```sql
CREATE TABLE [dbo].[ActionType] (
    [ActionId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 動作類型代碼
    [ActionName] NVARCHAR(100) NOT NULL, -- 動作類型名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.4 `ActionTypeDetail` - 動作類型明細主檔
```sql
CREATE TABLE [dbo].[ActionTypeDetail] (
    [ActionId] NVARCHAR(20) NOT NULL, -- 動作類型代碼
    [ActionIdD] NVARCHAR(20) NOT NULL, -- 動作類型明細代碼
    [ActionNameD] NVARCHAR(100) NOT NULL, -- 動作類型明細名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ActionTypeDetail] PRIMARY KEY CLUSTERED ([ActionId], [ActionIdD])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| EmpId | NVARCHAR | 50 | NO | - | 員工編號 | - |
| EmpName | NVARCHAR | 100 | YES | - | 員工姓名 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至組織表 |
| SiteId | NVARCHAR | 50 | YES | - | 店別代碼 | 外鍵至店別表 |
| CardType | NVARCHAR | 20 | YES | - | 卡片類型 | 外鍵至CardType表 |
| ActionType | NVARCHAR | 20 | YES | - | 動作類型 | 外鍵至ActionType表 |
| ActionTypeD | NVARCHAR | 20 | YES | - | 動作類型明細 | 外鍵至ActionTypeDetail表 |
| StartDate | DATETIME2 | - | YES | - | 起始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待審核, A:已審核, R:已拒絕 |
| Verifier | NVARCHAR | 50 | YES | - | 審核者 | 外鍵至Users表 |
| VerifyDate | DATETIME2 | - | YES | - | 審核日期 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| TxnNo | NVARCHAR | 50 | YES | - | 交易單號 | 外鍵至MainApplyMaster表 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢業務報表列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/meal-cards`
- **說明**: 查詢員工餐卡申請列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TKey",
    "sortOrder": "DESC",
    "filters": {
      "empId": "",
      "empName": "",
      "orgId": "",
      "siteId": "",
      "cardType": "",
      "actionType": "",
      "status": "",
      "startDate": "",
      "endDate": ""
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
          "empId": "E001",
          "empName": "張三",
          "orgId": "ORG001",
          "siteId": "SITE001",
          "cardType": "MEAL",
          "actionType": "APPLY",
          "actionTypeD": "NEW",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "status": "P",
          "verifier": null,
          "verifyDate": null,
          "notes": "申請餐卡",
          "txnNo": "TXN20240101001"
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

#### 3.1.2 查詢單筆業務報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/meal-cards/{tKey}`
- **說明**: 根據主鍵查詢單筆員工餐卡申請資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 新增業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/meal-cards`
- **說明**: 新增員工餐卡申請資料
- **請求格式**:
  ```json
  {
    "empId": "E001",
    "empName": "張三",
    "orgId": "ORG001",
    "siteId": "SITE001",
    "cardType": "MEAL",
    "actionType": "APPLY",
    "actionTypeD": "NEW",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "notes": "申請餐卡"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改業務報表
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/business-reports/meal-cards/{tKey}`
- **說明**: 修改員工餐卡申請資料（需檢查狀態，已審核的不可修改）
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除業務報表
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/business-reports/meal-cards/{tKey}`
- **說明**: 刪除員工餐卡申請資料（需檢查狀態，已審核的不可刪除）
- **路徑參數**:
  - `tKey`: 主鍵
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

#### 3.1.6 批次審核
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/meal-cards/batch-verify`
- **說明**: 批次審核員工餐卡申請
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3],
    "action": "approve", // approve: 通過, reject: 拒絕
    "notes": "批次審核通過"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "批次審核成功",
    "data": {
      "successCount": 3,
      "failCount": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 檔案匯入
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/meal-cards/import`
- **說明**: 匯入員工餐卡申請資料（支援Excel格式）
- **請求格式**: `multipart/form-data`
  - `file`: Excel檔案
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "匯入成功",
    "data": {
      "successCount": 100,
      "failCount": 0,
      "errors": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 取得下拉選單資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/meal-cards/dropdowns`
- **說明**: 取得卡片類型、動作類型等下拉選單資料
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "cardTypes": [
        { "cardId": "MEAL", "cardName": "餐卡" }
      ],
      "actionTypes": [
        { "actionId": "APPLY", "actionName": "申請" }
      ],
      "actionTypeDetails": [
        { "actionId": "APPLY", "actionIdD": "NEW", "actionNameD": "新申請" }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `BusinessReportMealCardController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/business-reports/meal-cards")]
    [Authorize]
    public class BusinessReportMealCardController : ControllerBase
    {
        private readonly IBusinessReportMealCardService _service;
        
        public BusinessReportMealCardController(IBusinessReportMealCardService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardDto>>>> GetMealCards([FromQuery] EmployeeMealCardQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardDto>>> GetMealCard(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardKeyDto>>> CreateMealCard([FromBody] CreateEmployeeMealCardDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateMealCard(long tKey, [FromBody] UpdateEmployeeMealCardDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteMealCard(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("batch-verify")]
        public async Task<ActionResult<ApiResponse<BatchVerifyResultDto>>> BatchVerify([FromBody] BatchVerifyDto dto)
        {
            // 實作批次審核邏輯
        }
        
        [HttpPost("import")]
        public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportMealCards(IFormFile file)
        {
            // 實作檔案匯入邏輯
        }
        
        [HttpGet("dropdowns")]
        public async Task<ActionResult<ApiResponse<MealCardDropdownsDto>>> GetDropdowns()
        {
            // 實作取得下拉選單資料邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 業務報表列表頁面 (`BusinessReportMealCardList.vue`)
- **路徑**: `/business-reports/meal-cards`
- **功能**: 顯示員工餐卡申請列表，支援查詢、新增、修改、刪除、批次審核、匯入
- **主要元件**:
  - 查詢表單 (MealCardSearchForm)
  - 資料表格 (MealCardDataTable)
  - 新增/修改對話框 (MealCardDialog)
  - 批次審核對話框 (BatchVerifyDialog)
  - 檔案匯入對話框 (ImportDialog)
  - 刪除確認對話框

#### 4.1.2 業務報表詳細頁面 (`BusinessReportMealCardDetail.vue`)
- **路徑**: `/business-reports/meal-cards/:tKey`
- **功能**: 顯示員工餐卡申請詳細資料，支援修改、審核

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`MealCardSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="員工編號">
      <el-input v-model="searchForm.empId" placeholder="請輸入員工編號" />
    </el-form-item>
    <el-form-item label="員工姓名">
      <el-input v-model="searchForm.empName" placeholder="請輸入員工姓名" />
    </el-form-item>
    <el-form-item label="組織">
      <el-select v-model="searchForm.orgId" placeholder="請選擇組織" clearable>
        <el-option v-for="org in orgList" :key="org.orgId" :label="org.orgName" :value="org.orgId" />
      </el-select>
    </el-form-item>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別" clearable>
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="卡片類型">
      <el-select v-model="searchForm.cardType" placeholder="請選擇卡片類型" clearable>
        <el-option v-for="card in cardTypeList" :key="card.cardId" :label="card.cardName" :value="card.cardId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態" clearable>
        <el-option label="全部" value="" />
        <el-option label="待審核" value="P" />
        <el-option label="已審核" value="A" />
        <el-option label="已拒絕" value="R" />
      </el-select>
    </el-form-item>
    <el-form-item label="起始日期">
      <el-date-picker v-model="searchForm.startDate" type="date" placeholder="請選擇起始日期" />
    </el-form-item>
    <el-form-item label="結束日期">
      <el-date-picker v-model="searchForm.endDate" type="date" placeholder="請選擇結束日期" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`MealCardDataTable.vue`)
```vue
<template>
  <div>
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">新增</el-button>
      <el-button type="success" @click="handleBatchVerify" :disabled="selectedRows.length === 0">批次審核</el-button>
      <el-button type="info" @click="handleImport">匯入</el-button>
      <el-button type="warning" @click="handleExport">匯出</el-button>
    </div>
    <el-table 
      :data="mealCardList" 
      v-loading="loading"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="55" />
      <el-table-column prop="empId" label="員工編號" width="120" />
      <el-table-column prop="empName" label="員工姓名" width="120" />
      <el-table-column prop="orgId" label="組織" width="120" />
      <el-table-column prop="siteId" label="店別" width="120" />
      <el-table-column prop="cardType" label="卡片類型" width="120">
        <template #default="{ row }">
          {{ getCardTypeName(row.cardType) }}
        </template>
      </el-table-column>
      <el-table-column prop="actionType" label="動作類型" width="120">
        <template #default="{ row }">
          {{ getActionTypeName(row.actionType) }}
        </template>
      </el-table-column>
      <el-table-column prop="startDate" label="起始日期" width="120" />
      <el-table-column prop="endDate" label="結束日期" width="120" />
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusName(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="verifier" label="審核者" width="120" />
      <el-table-column prop="verifyDate" label="審核日期" width="120" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.status === 'A'">刪除</el-button>
          <el-button type="success" size="small" @click="handleVerify(row)" :disabled="row.status !== 'P'">審核</el-button>
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

#### 4.2.3 新增/修改對話框 (`MealCardDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="員工編號" prop="empId">
        <el-input v-model="form.empId" placeholder="請輸入員工編號" />
      </el-form-item>
      <el-form-item label="員工姓名" prop="empName">
        <el-input v-model="form.empName" placeholder="請輸入員工姓名" />
      </el-form-item>
      <el-form-item label="組織" prop="orgId">
        <el-select v-model="form.orgId" placeholder="請選擇組織" clearable>
          <el-option v-for="org in orgList" :key="org.orgId" :label="org.orgName" :value="org.orgId" />
        </el-select>
      </el-form-item>
      <el-form-item label="店別" prop="siteId">
        <el-select v-model="form.siteId" placeholder="請選擇店別" clearable>
          <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
        </el-select>
      </el-form-item>
      <el-form-item label="卡片類型" prop="cardType">
        <el-select v-model="form.cardType" placeholder="請選擇卡片類型" clearable>
          <el-option v-for="card in cardTypeList" :key="card.cardId" :label="card.cardName" :value="card.cardId" />
        </el-select>
      </el-form-item>
      <el-form-item label="動作類型" prop="actionType">
        <el-select v-model="form.actionType" placeholder="請選擇動作類型" clearable @change="handleActionTypeChange">
          <el-option v-for="action in actionTypeList" :key="action.actionId" :label="action.actionName" :value="action.actionId" />
        </el-select>
      </el-form-item>
      <el-form-item label="動作類型明細" prop="actionTypeD">
        <el-select v-model="form.actionTypeD" placeholder="請選擇動作類型明細" clearable :disabled="!form.actionType">
          <el-option v-for="detail in actionTypeDetailList" :key="detail.actionIdD" :label="detail.actionNameD" :value="detail.actionIdD" />
        </el-select>
      </el-form-item>
      <el-form-item label="起始日期" prop="startDate">
        <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇起始日期" />
      </el-form-item>
      <el-form-item label="結束日期" prop="endDate">
        <el-date-picker v-model="form.endDate" type="date" placeholder="請選擇結束日期" />
      </el-form-item>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

---

## 五、開發注意事項

### 5.1 業務規則
- 起始日期必須是每月1日
- 已審核的資料不可修改或刪除
- 批次審核時需檢查所有選取的資料是否都是待審核狀態
- 檔案匯入時需驗證資料格式和必填欄位
- 審核時需記錄審核者和審核時間

### 5.2 權限控制
- 查詢權限：所有使用者
- 新增權限：需有新增權限
- 修改權限：需有修改權限，且資料為待審核狀態
- 刪除權限：需有刪除權限，且資料為待審核狀態
- 審核權限：需有審核權限
- 批次審核權限：需有審核權限
- 匯入權限：需有匯入權限

### 5.3 效能優化
- 查詢時使用適當的索引
- 大量資料匯入時使用批次處理
- 報表列印時使用分頁處理

### 5.4 錯誤處理
- 驗證必填欄位
- 驗證日期範圍
- 驗證資料格式
- 處理資料庫錯誤
- 處理檔案匯入錯誤

---

## 六、測試計劃

### 6.1 單元測試
- 測試Service層的業務邏輯
- 測試Repository層的資料存取
- 測試API端點

### 6.2 整合測試
- 測試完整的CRUD流程
- 測試批次審核流程
- 測試檔案匯入流程

### 6.3 前端測試
- 測試UI元件功能
- 測試表單驗證
- 測試資料表格操作

---

## 七、部署注意事項

### 7.1 資料庫遷移
- 建立資料表結構
- 建立索引
- 建立外鍵約束

### 7.2 設定檔
- 設定API端點
- 設定檔案上傳路徑
- 設定報表列印路徑

### 7.3 權限設定
- 設定功能權限
- 設定按鈕權限

