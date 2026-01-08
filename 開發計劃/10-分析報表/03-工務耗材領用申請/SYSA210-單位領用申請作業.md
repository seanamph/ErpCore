# SYSA210 - 單位領用申請作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSA210
- **功能名稱**: 單位領用申請作業
- **功能描述**: 提供單位領用申請的新增、修改、刪除、查詢功能，包含領用單主檔和明細檔管理，支援審核流程、發料作業等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FS.ASP` (顯示)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FMI.ASP` (多筆新增)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA210_PR3.ASP` (報表)

### 1.2 業務需求
- 管理單位領用申請單資訊
- 支援領用申請的新增、修改、刪除、查詢
- 支援多筆新增（快速輸入、購物車模式）
- 支援領用單審核流程
- 支援發料作業
- 支援分店權限控制
- 支援申請人僅能查詢該分店人員
- 記錄領用單的建立與變更資訊
- 支援領用單狀態管理（待審核、已審核、已發料等）
- 支援領用品總價值計算
- 支援品項單位自動帶出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MaterialApplyMasters` (對應舊系統 `AM_MTAPPLYM`)

```sql
CREATE TABLE [dbo].[MaterialApplyMasters] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [ApplyId] NVARCHAR(20) NOT NULL, -- 領用單號 (MTAPPLY_ID)
    [EmpId] NVARCHAR(50) NOT NULL, -- 申請人代號 (EMP_ID)
    [OrgId] NVARCHAR(50) NOT NULL, -- 部門代號 (ORG_ID)
    [SiteId] NVARCHAR(50) NULL, -- 分店代號 (SITE_ID)
    [ApplyDate] DATETIME2 NOT NULL, -- 申請日期 (APPLY_DATE)
    [ApplyStatus] NVARCHAR(10) NOT NULL DEFAULT '0', -- 領用單狀態 (MTAPPLY_STATUS) 0:待審核, 1:已審核, 2:已發料, 3:已取消
    [Amount] DECIMAL(18,2) NULL DEFAULT 0, -- 領用品總價值 (AMT)
    [AprvEmpId] NVARCHAR(50) NULL, -- 審核者 (APRV_EMP_ID)
    [AprvDate] DATETIME2 NULL, -- 審核日期 (APRV_DATE)
    [CheckDate] DATETIME2 NULL, -- 發料日期 (CHECK_DATE)
    [WhId] NVARCHAR(50) NULL, -- 倉別 (WH_ID)
    [StoreId] NVARCHAR(50) NULL, -- 儲位 (STORE_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_MaterialApplyMasters] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_MaterialApplyMasters_ApplyId] UNIQUE ([ApplyId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_ApplyId] ON [dbo].[MaterialApplyMasters] ([ApplyId]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_EmpId] ON [dbo].[MaterialApplyMasters] ([EmpId]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_OrgId] ON [dbo].[MaterialApplyMasters] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_SiteId] ON [dbo].[MaterialApplyMasters] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_ApplyDate] ON [dbo].[MaterialApplyMasters] ([ApplyDate]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_ApplyStatus] ON [dbo].[MaterialApplyMasters] ([ApplyStatus]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyMasters_AprvEmpId] ON [dbo].[MaterialApplyMasters] ([AprvEmpId]);
```

### 2.2 明細資料表: `MaterialApplyDetails` (對應舊系統 `AM_MTAPPLYD`)

```sql
CREATE TABLE [dbo].[MaterialApplyDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [ApplyMasterKey] BIGINT NOT NULL, -- 主檔主鍵 (MTAPPLY_KEY)
    [ApplyId] NVARCHAR(20) NOT NULL, -- 領用單號 (MTAPPLY_ID)
    [GoodsTKey] BIGINT NOT NULL, -- 品項主鍵 (GOODS_T_KEY)
    [GoodsId] NVARCHAR(50) NOT NULL, -- 品項編號 (GOODS_ID)
    [ApplyQty] DECIMAL(18,3) NOT NULL DEFAULT 0, -- 申請數量 (APPLY_QTY)
    [IssueQty] DECIMAL(18,3) NULL DEFAULT 0, -- 發料數量 (ISSUE_QTY)
    [Unit] NVARCHAR(20) NULL, -- 單位 (UNIT)
    [UnitPrice] DECIMAL(18,2) NULL DEFAULT 0, -- 單價 (UNIT_PRICE)
    [Amount] DECIMAL(18,2) NULL DEFAULT 0, -- 金額 (AMT)
    [Notes] NVARCHAR(500) NULL, -- 附註 (NOTES)
    [SeqNo] INT NULL DEFAULT 0, -- 序號 (SEQ_NO)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_MaterialApplyDetails] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_MaterialApplyDetails_MaterialApplyMasters] FOREIGN KEY ([ApplyMasterKey]) REFERENCES [dbo].[MaterialApplyMasters] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MaterialApplyDetails_ApplyMasterKey] ON [dbo].[MaterialApplyDetails] ([ApplyMasterKey]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyDetails_ApplyId] ON [dbo].[MaterialApplyDetails] ([ApplyId]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyDetails_GoodsTKey] ON [dbo].[MaterialApplyDetails] ([GoodsTKey]);
CREATE NONCLUSTERED INDEX [IX_MaterialApplyDetails_GoodsId] ON [dbo].[MaterialApplyDetails] ([GoodsId]);
```

### 2.3 相關資料表

#### 2.3.1 `Employees` - 員工主檔
- 用於查詢申請人資訊
- 參考: `V_EMP_USER` 視圖

#### 2.3.2 `Organizations` - 組織主檔
- 用於查詢部門資訊
- 參考: `ORG_GROUP` 資料表

#### 2.3.3 `Goods` - 品項主檔
- 用於查詢品項資訊
- 參考: `AM_GOODS` 資料表

#### 2.3.4 `Sites` - 分店主檔
- 用於查詢分店資訊

### 2.4 資料字典

#### MaterialApplyMasters 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ApplyId | NVARCHAR | 20 | NO | - | 領用單號 | 唯一鍵 |
| EmpId | NVARCHAR | 50 | NO | - | 申請人代號 | 外鍵至Employees表 |
| OrgId | NVARCHAR | 50 | NO | - | 部門代號 | 外鍵至Organizations表 |
| SiteId | NVARCHAR | 50 | YES | - | 分店代號 | 外鍵至Sites表 |
| ApplyDate | DATETIME2 | - | NO | - | 申請日期 | - |
| ApplyStatus | NVARCHAR | 10 | NO | '0' | 領用單狀態 | 0:待審核, 1:已審核, 2:已發料, 3:已取消 |
| Amount | DECIMAL | 18,2 | YES | 0 | 領用品總價值 | - |
| AprvEmpId | NVARCHAR | 50 | YES | - | 審核者 | 外鍵至Employees表 |
| AprvDate | DATETIME2 | - | YES | - | 審核日期 | - |
| CheckDate | DATETIME2 | - | YES | - | 發料日期 | - |
| WhId | NVARCHAR | 50 | YES | - | 倉別 | - |
| StoreId | NVARCHAR | 50 | YES | - | 儲位 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

#### MaterialApplyDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ApplyMasterKey | BIGINT | - | NO | - | 主檔主鍵 | 外鍵至MaterialApplyMasters表 |
| ApplyId | NVARCHAR | 20 | NO | - | 領用單號 | - |
| GoodsTKey | BIGINT | - | NO | - | 品項主鍵 | 外鍵至Goods表 |
| GoodsId | NVARCHAR | 50 | NO | - | 品項編號 | - |
| ApplyQty | DECIMAL | 18,3 | NO | 0 | 申請數量 | - |
| IssueQty | DECIMAL | 18,3 | YES | 0 | 發料數量 | - |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| UnitPrice | DECIMAL | 18,2 | YES | 0 | 單價 | - |
| Amount | DECIMAL | 18,2 | YES | 0 | 金額 | - |
| Notes | NVARCHAR | 500 | YES | - | 附註 | - |
| SeqNo | INT | - | YES | 0 | 序號 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢領用申請列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/material-applies`
- **說明**: 查詢領用申請列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ApplyDate",
    "sortOrder": "DESC",
    "filters": {
      "applyId": "",
      "empId": "",
      "orgId": "",
      "siteId": "",
      "applyDateFrom": "",
      "applyDateTo": "",
      "aprvDateFrom": "",
      "aprvDateTo": "",
      "checkDate": "",
      "applyStatus": "",
      "goodsId": "",
      "whId": "",
      "storeId": ""
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
          "applyId": "MA20240101001",
          "empId": "E001",
          "empName": "張三",
          "orgId": "ORG001",
          "orgName": "資訊部",
          "siteId": "SITE001",
          "siteName": "總公司",
          "applyDate": "2024-01-01T00:00:00",
          "applyStatus": "0",
          "applyStatusName": "待審核",
          "amount": 1000.00,
          "aprvEmpId": null,
          "aprvEmpName": null,
          "aprvDate": null,
          "checkDate": null,
          "whId": null,
          "storeId": null,
          "notes": "",
          "createdAt": "2024-01-01T00:00:00",
          "updatedAt": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆領用申請（含明細）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/material-applies/{applyId}`
- **說明**: 根據領用單號查詢單筆領用申請資料，包含明細
- **路徑參數**:
  - `applyId`: 領用單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "applyId": "MA20240101001",
      "empId": "E001",
      "empName": "張三",
      "orgId": "ORG001",
      "orgName": "資訊部",
      "siteId": "SITE001",
      "siteName": "總公司",
      "applyDate": "2024-01-01T00:00:00",
      "applyStatus": "0",
      "applyStatusName": "待審核",
      "amount": 1000.00,
      "aprvEmpId": null,
      "aprvEmpName": null,
      "aprvDate": null,
      "checkDate": null,
      "whId": null,
      "storeId": null,
      "notes": "",
      "details": [
        {
          "tKey": 1,
          "goodsTKey": 100,
          "goodsId": "G001",
          "goodsName": "耗材A",
          "applyQty": 10.000,
          "issueQty": 0.000,
          "unit": "個",
          "unitPrice": 100.00,
          "amount": 1000.00,
          "notes": "",
          "seqNo": 1
        }
      ],
      "createdBy": "U001",
      "createdAt": "2024-01-01T00:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增領用申請
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/material-applies`
- **說明**: 新增領用申請資料，包含主檔和明細
- **請求格式**:
  ```json
  {
    "applyId": "MA20240101001",
    "empId": "E001",
    "orgId": "ORG001",
    "siteId": "SITE001",
    "applyDate": "2024-01-01T00:00:00",
    "whId": "WH001",
    "storeId": "STORE001",
    "notes": "",
    "details": [
      {
        "goodsTKey": 100,
        "goodsId": "G001",
        "applyQty": 10.000,
        "unitPrice": 100.00,
        "notes": "",
        "seqNo": 1
      }
    ]
  }
  ```
- **回應格式**: 同查詢單筆領用申請

#### 3.1.4 修改領用申請
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/material-applies/{applyId}`
- **說明**: 修改領用申請資料，包含主檔和明細
- **注意事項**: 僅待審核狀態的領用單可修改
- **請求格式**: 同新增領用申請

#### 3.1.5 刪除領用申請
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/material-applies/{applyId}`
- **說明**: 刪除領用申請資料，包含主檔和明細
- **注意事項**: 僅待審核狀態的領用單可刪除

#### 3.1.6 審核領用申請
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/material-applies/{applyId}/approve`
- **說明**: 審核領用申請
- **請求格式**:
  ```json
  {
    "aprvEmpId": "E002",
    "aprvDate": "2024-01-02T00:00:00",
    "notes": "審核通過"
  }
  ```
- **回應格式**: 同查詢單筆領用申請

#### 3.1.7 發料作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/material-applies/{applyId}/issue`
- **說明**: 執行發料作業，更新發料數量和發料日期
- **請求格式**:
  ```json
  {
    "checkDate": "2024-01-03T00:00:00",
    "details": [
      {
        "tKey": 1,
        "issueQty": 10.000
      }
    ]
  }
  ```
- **回應格式**: 同查詢單筆領用申請

#### 3.1.8 多筆新增領用申請（快速輸入）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/material-applies/batch`
- **說明**: 批次新增領用申請
- **請求格式**:
  ```json
  {
    "empId": "E001",
    "orgId": "ORG001",
    "siteId": "SITE001",
    "applyDate": "2024-01-01T00:00:00",
    "items": [
      {
        "goodsId": "G001",
        "applyQty": 10.000
      },
      {
        "goodsId": "G002",
        "applyQty": 5.000
      }
    ]
  }
  ```

#### 3.1.9 產生領用單號
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/material-applies/generate-apply-id`
- **說明**: 產生新的領用單號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "產生成功",
    "data": {
      "applyId": "MA20240101001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Entity: `MaterialApplyMaster.cs`
```csharp
namespace RSL.IMS3.Domain.Entities
{
    public class MaterialApplyMaster
    {
        public long TKey { get; set; }
        public string ApplyId { get; set; }
        public string EmpId { get; set; }
        public string OrgId { get; set; }
        public string SiteId { get; set; }
        public DateTime ApplyDate { get; set; }
        public string ApplyStatus { get; set; }
        public decimal Amount { get; set; }
        public string AprvEmpId { get; set; }
        public DateTime? AprvDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public string WhId { get; set; }
        public string StoreId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // 導航屬性
        public List<MaterialApplyDetail> Details { get; set; }
    }
}
```

#### 3.2.2 Entity: `MaterialApplyDetail.cs`
```csharp
namespace RSL.IMS3.Domain.Entities
{
    public class MaterialApplyDetail
    {
        public long TKey { get; set; }
        public long ApplyMasterKey { get; set; }
        public string ApplyId { get; set; }
        public long GoodsTKey { get; set; }
        public string GoodsId { get; set; }
        public decimal ApplyQty { get; set; }
        public decimal? IssueQty { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int SeqNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // 導航屬性
        public MaterialApplyMaster Master { get; set; }
    }
}
```

#### 3.2.3 Repository: `IMaterialApplyRepository.cs`
```csharp
namespace RSL.IMS3.Application.Interfaces
{
    public interface IMaterialApplyRepository
    {
        Task<MaterialApplyMaster> GetByApplyIdAsync(string applyId);
        Task<List<MaterialApplyMaster>> GetListAsync(MaterialApplyQueryDto query);
        Task<int> GetCountAsync(MaterialApplyQueryDto query);
        Task<MaterialApplyMaster> CreateAsync(MaterialApplyMaster master);
        Task<MaterialApplyMaster> UpdateAsync(MaterialApplyMaster master);
        Task DeleteAsync(string applyId);
        Task<string> GenerateApplyIdAsync();
    }
}
```

#### 3.2.4 Service: `MaterialApplyService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IMaterialApplyService
    {
        Task<MaterialApplyDto> GetByApplyIdAsync(string applyId);
        Task<PagedResult<MaterialApplyDto>> GetListAsync(MaterialApplyQueryDto query);
        Task<MaterialApplyDto> CreateAsync(CreateMaterialApplyDto dto);
        Task<MaterialApplyDto> UpdateAsync(string applyId, UpdateMaterialApplyDto dto);
        Task DeleteAsync(string applyId);
        Task<MaterialApplyDto> ApproveAsync(string applyId, ApproveMaterialApplyDto dto);
        Task<MaterialApplyDto> IssueAsync(string applyId, IssueMaterialApplyDto dto);
        Task<List<MaterialApplyDto>> BatchCreateAsync(BatchCreateMaterialApplyDto dto);
        Task<string> GenerateApplyIdAsync();
    }
    
    public class MaterialApplyService : IMaterialApplyService
    {
        // 實作業務邏輯
    }
}
```

#### 3.2.5 Controller: `MaterialApplyController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/material-applies")]
    [Authorize]
    public class MaterialApplyController : ControllerBase
    {
        private readonly IMaterialApplyService _service;
        
        // 實作API端點
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 查詢頁面 (`MaterialApplyQuery.vue`)
```vue
<template>
  <div class="material-apply-query">
    <el-form :model="queryForm" :inline="true" label-width="120px">
      <el-form-item label="領用單號">
        <el-input v-model="queryForm.applyId" placeholder="請輸入領用單號" />
      </el-form-item>
      <el-form-item label="申請人">
        <el-input v-model="queryForm.empId" placeholder="請輸入申請人代號" />
        <el-button type="primary" size="small" @click="selectEmployee">選擇</el-button>
      </el-form-item>
      <el-form-item label="部門">
        <el-input v-model="queryForm.orgId" placeholder="請輸入部門代號" />
        <el-button type="primary" size="small" @click="selectOrganization">選擇</el-button>
      </el-form-item>
      <el-form-item label="分店">
        <el-select v-model="queryForm.siteId" placeholder="請選擇分店" multiple>
          <el-option v-for="item in siteList" :key="item.siteId" :label="item.siteName" :value="item.siteId" />
        </el-select>
      </el-form-item>
      <el-form-item label="申請日期">
        <el-date-picker v-model="queryForm.applyDateFrom" type="date" placeholder="開始日期" />
        <span>至</span>
        <el-date-picker v-model="queryForm.applyDateTo" type="date" placeholder="結束日期" />
      </el-form-item>
      <el-form-item label="領用單狀態">
        <el-select v-model="queryForm.applyStatus" placeholder="請選擇狀態" clearable>
          <el-option label="待審核" value="0" />
          <el-option label="已審核" value="1" />
          <el-option label="已發料" value="2" />
          <el-option label="已取消" value="3" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
    
    <el-table :data="tableData" border>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="applyId" label="領用單號" width="150" />
      <el-table-column prop="empName" label="申請人" width="120" />
      <el-table-column prop="orgName" label="部門" width="150" />
      <el-table-column prop="siteName" label="分店" width="120" />
      <el-table-column prop="applyDate" label="申請日期" width="120" />
      <el-table-column prop="applyStatusName" label="狀態" width="100" />
      <el-table-column prop="amount" label="總價值" width="120" align="right" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">查看</el-button>
          <el-button type="warning" size="small" @click="handleEdit(row)" v-if="row.applyStatus === '0'">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)" v-if="row.applyStatus === '0'">刪除</el-button>
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

<script setup lang="ts">
// 實作查詢邏輯
</script>
```

#### 4.1.2 新增/修改對話框 (`MaterialApplyDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1200px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="領用單號" prop="applyId">
            <el-input v-model="form.applyId" :disabled="isEdit" />
            <el-button type="primary" size="small" @click="generateApplyId" v-if="!isEdit">產生</el-button>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="申請日期" prop="applyDate">
            <el-date-picker v-model="form.applyDate" type="date" placeholder="請選擇申請日期" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="申請人" prop="empId">
            <el-input v-model="form.empId" placeholder="請輸入申請人代號" />
            <el-button type="primary" size="small" @click="selectEmployee">選擇</el-button>
            <el-input v-model="form.empName" readonly />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="部門" prop="orgId">
            <el-input v-model="form.orgId" placeholder="請輸入部門代號" />
            <el-button type="primary" size="small" @click="selectOrganization">選擇</el-button>
            <el-input v-model="form.orgName" readonly />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="分店" prop="siteId">
            <el-select v-model="form.siteId" placeholder="請選擇分店">
              <el-option v-for="item in siteList" :key="item.siteId" :label="item.siteName" :value="item.siteId" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="倉別">
            <el-input v-model="form.whId" placeholder="請輸入倉別" />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-divider>明細資料</el-divider>
      
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column label="品項編號" width="150">
          <template #default="{ row, $index }">
            <el-input v-model="row.goodsId" placeholder="請輸入品項編號" />
            <el-button type="primary" size="small" @click="selectGoods($index)">選擇</el-button>
          </template>
        </el-table-column>
        <el-table-column label="品項名稱" width="200">
          <template #default="{ row }">
            <el-input v-model="row.goodsName" readonly />
          </template>
        </el-table-column>
        <el-table-column label="單位" width="100">
          <template #default="{ row }">
            <el-input v-model="row.unit" readonly />
          </template>
        </el-table-column>
        <el-table-column label="申請數量" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.applyQty" :min="0" :precision="3" @change="calculateAmount($index)" />
          </template>
        </el-table-column>
        <el-table-column label="單價" width="120">
          <template #default="{ row, $index }">
            <el-input-number v-model="row.unitPrice" :min="0" :precision="2" @change="calculateAmount($index)" />
          </template>
        </el-table-column>
        <el-table-column label="金額" width="120">
          <template #default="{ row }">
            <el-input-number v-model="row.amount" :min="0" :precision="2" readonly />
          </template>
        </el-table-column>
        <el-table-column label="附註" width="200">
          <template #default="{ row }">
            <el-input v-model="row.notes" placeholder="請輸入附註" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="removeDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <el-button type="primary" size="small" @click="addDetail" style="margin-top: 10px;">新增明細</el-button>
      
      <el-form-item label="總價值">
        <el-input-number v-model="form.amount" :min="0" :precision="2" readonly />
      </el-form-item>
      
      <el-form-item label="備註">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 實作新增/修改邏輯
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 MaterialApplyMasters 資料表
- [ ] 建立 MaterialApplyDetails 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本
- [ ] 測試資料準備

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含業務邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 領用單號產生邏輯
- [ ] 審核流程邏輯
- [ ] 發料作業邏輯
- [ ] 分店權限控制邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 新增/修改對話框開發
- [ ] 明細資料管理
- [ ] 多筆新增功能（快速輸入、購物車）
- [ ] 審核功能
- [ ] 發料功能
- [ ] 表單驗證
- [ ] 分店權限控制
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 審核流程測試
- [ ] 發料作業測試
- [ ] 分店權限測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 16天

---

## 六、注意事項

### 6.1 業務邏輯
- 領用單號需自動產生，格式：MA + 日期(YYYYMMDD) + 序號(3碼)
- 僅待審核狀態的領用單可修改或刪除
- 審核後狀態變更為已審核
- 發料後狀態變更為已發料
- 總價值需自動計算（明細金額總和）
- 品項單位需從品項主檔自動帶出

### 6.2 權限控制
- 申請人僅能查詢該分店人員
- 分店欄位必填，依使用者店權限顯示可查詢選單
- 需檢查使用者分店權限

### 6.3 資料驗證
- 領用單號不可重複
- 申請日期不可為未來日期
- 申請數量需大於0
- 單價需大於等於0
- 明細資料不可為空

### 6.4 效能優化
- 查詢列表需使用分頁
- 明細資料需使用批次查詢
- 需建立適當的索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增領用申請成功
- [ ] 修改領用申請成功
- [ ] 刪除領用申請成功
- [ ] 審核領用申請成功
- [ ] 發料作業成功
- [ ] 領用單號產生成功
- [ ] 總價值計算正確
- [ ] 分店權限控制正確

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 審核流程測試
- [ ] 發料作業流程測試
- [ ] 多筆新增流程測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 分店權限測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FI.ASP` - 新增畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FU.ASP` - 修改畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FD.ASP` - 刪除畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FS.ASP` - 顯示畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FB.ASP` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_FMI.ASP` - 多筆新增畫面
- `WEB/IMS_CORE/ASP/SYSA000/SYSA210_PR3.ASP` - 報表

### 8.2 相關功能
- `SYSA220-單位領用申請作業.md` - 相關功能
- `SYSA240-單位領用申請作業.md` - 相關功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

