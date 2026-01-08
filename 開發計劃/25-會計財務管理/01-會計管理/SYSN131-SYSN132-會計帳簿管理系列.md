# SYSN131-SYSN132 - 會計帳簿管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN131-SYSN132 系列
- **功能名稱**: 會計帳簿管理系列
- **功能描述**: 提供會計帳簿資料的新增、修改、刪除、查詢功能，包含帳簿代號、帳簿名稱、帳簿類型、會計期間、科目設定等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FS.ASP` (排序)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN132_*.ASP` (相關功能)

### 1.2 業務需求
- 管理會計帳簿基本資料
- 支援帳簿代號的唯一性檢查
- 支援帳簿類型設定（總帳、明細帳、日記帳等）
- 支援會計期間設定
- 支援科目設定與對應
- 支援帳簿狀態管理（啟用/停用）
- 支援帳簿查詢與報表
- 支援帳簿排序功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Ledgers` (對應舊系統 `LEDGER`)

```sql
CREATE TABLE [dbo].[Ledgers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LedgerId] NVARCHAR(50) NOT NULL, -- 帳簿代號 (LEDGER_ID)
    [LedgerName] NVARCHAR(200) NOT NULL, -- 帳簿名稱 (LEDGER_NAME)
    [LedgerType] NVARCHAR(20) NULL, -- 帳簿類型 (LEDGER_TYPE, 總帳/明細帳/日記帳)
    [PeriodType] NVARCHAR(20) NULL, -- 會計期間類型 (PERIOD_TYPE, 月/季/年)
    [StartDate] DATETIME2 NULL, -- 起始日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [LedgerOrder] INT NULL, -- 排序 (LEDGER_ORDER)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Ledgers] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Ledgers_LedgerId] UNIQUE ([LedgerId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Ledgers_LedgerId] ON [dbo].[Ledgers] ([LedgerId]);
CREATE NONCLUSTERED INDEX [IX_Ledgers_LedgerType] ON [dbo].[Ledgers] ([LedgerType]);
CREATE NONCLUSTERED INDEX [IX_Ledgers_Status] ON [dbo].[Ledgers] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Ledgers_LedgerOrder] ON [dbo].[Ledgers] ([LedgerOrder]);
```

### 2.2 相關資料表

#### 2.2.1 `LedgerAccountSubjects` - 帳簿科目對應
```sql
CREATE TABLE [dbo].[LedgerAccountSubjects] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LedgerId] NVARCHAR(50) NOT NULL, -- 帳簿代號
    [StypeId] NVARCHAR(50) NOT NULL, -- 科目代號
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LedgerAccountSubjects_Ledgers] FOREIGN KEY ([LedgerId]) REFERENCES [dbo].[Ledgers] ([LedgerId]) ON DELETE CASCADE,
    CONSTRAINT [FK_LedgerAccountSubjects_AccountSubjects] FOREIGN KEY ([StypeId]) REFERENCES [dbo].[AccountSubjects] ([StypeId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LedgerAccountSubjects_LedgerId] ON [dbo].[LedgerAccountSubjects] ([LedgerId]);
CREATE NONCLUSTERED INDEX [IX_LedgerAccountSubjects_StypeId] ON [dbo].[LedgerAccountSubjects] ([StypeId]);
```

### 2.3 資料字典

#### Ledgers 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| LedgerId | NVARCHAR | 50 | NO | - | 帳簿代號 | 唯一，主鍵候選 |
| LedgerName | NVARCHAR | 200 | NO | - | 帳簿名稱 | - |
| LedgerType | NVARCHAR | 20 | YES | - | 帳簿類型 | 總帳/明細帳/日記帳 |
| PeriodType | NVARCHAR | 20 | YES | - | 會計期間類型 | 月/季/年 |
| StartDate | DATETIME2 | - | YES | - | 起始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| LedgerOrder | INT | - | YES | - | 排序 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢帳簿列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ledgers`
- **說明**: 查詢帳簿列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "LedgerId",
    "sortOrder": "ASC",
    "filters": {
      "ledgerId": "",
      "ledgerName": "",
      "ledgerType": "",
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
      "items": [
        {
          "tKey": 1,
          "ledgerId": "L001",
          "ledgerName": "總帳",
          "ledgerType": "GENERAL",
          "periodType": "MONTHLY",
          "status": "A"
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

#### 3.1.2 查詢單筆帳簿
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ledgers/{ledgerId}`
- **說明**: 根據帳簿代號查詢單筆帳簿資料
- **路徑參數**:
  - `ledgerId`: 帳簿代號
- **回應格式**: 同查詢列表單筆資料，包含科目對應列表

#### 3.1.3 新增帳簿
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/ledgers`
- **說明**: 新增帳簿資料
- **請求格式**:
  ```json
  {
    "ledgerId": "L001",
    "ledgerName": "總帳",
    "ledgerType": "GENERAL",
    "periodType": "MONTHLY",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "status": "A",
    "notes": "備註",
    "ledgerOrder": 1,
    "accountSubjects": ["1000", "2000"]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "ledgerId": "L001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改帳簿
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/ledgers/{ledgerId}`
- **說明**: 修改帳簿資料
- **路徑參數**:
  - `ledgerId`: 帳簿代號
- **請求格式**: 同新增，但 `ledgerId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除帳簿
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/ledgers/{ledgerId}`
- **說明**: 刪除帳簿資料（軟刪除或硬刪除）
- **路徑參數**:
  - `ledgerId`: 帳簿代號
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

#### 3.1.6 批次刪除帳簿
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/ledgers/batch`
- **說明**: 批次刪除多筆帳簿
- **請求格式**:
  ```json
  {
    "ledgerIds": ["L001", "L002", "L003"]
  }
  ```

#### 3.1.7 啟用/停用帳簿
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/ledgers/{ledgerId}/status`
- **說明**: 啟用或停用帳簿
- **請求格式**:
  ```json
  {
    "status": "A" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `LedgersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ledgers")]
    [Authorize]
    public class LedgersController : ControllerBase
    {
        private readonly ILedgerService _ledgerService;
        
        public LedgersController(ILedgerService ledgerService)
        {
            _ledgerService = ledgerService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LedgerDto>>>> GetLedgers([FromQuery] LedgerQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{ledgerId}")]
        public async Task<ActionResult<ApiResponse<LedgerDto>>> GetLedger(string ledgerId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLedger([FromBody] CreateLedgerDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{ledgerId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLedger(string ledgerId, [FromBody] UpdateLedgerDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{ledgerId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLedger(string ledgerId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `LedgerService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILedgerService
    {
        Task<PagedResult<LedgerDto>> GetLedgersAsync(LedgerQueryDto query);
        Task<LedgerDto> GetLedgerByIdAsync(string ledgerId);
        Task<string> CreateLedgerAsync(CreateLedgerDto dto);
        Task UpdateLedgerAsync(string ledgerId, UpdateLedgerDto dto);
        Task DeleteLedgerAsync(string ledgerId);
        Task UpdateStatusAsync(string ledgerId, string status);
    }
}
```

#### 3.2.3 Repository: `LedgerRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ILedgerRepository
    {
        Task<Ledger> GetByIdAsync(string ledgerId);
        Task<PagedResult<Ledger>> GetPagedAsync(LedgerQuery query);
        Task<Ledger> CreateAsync(Ledger ledger);
        Task<Ledger> UpdateAsync(Ledger ledger);
        Task DeleteAsync(string ledgerId);
        Task<bool> ExistsAsync(string ledgerId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 帳簿列表頁面 (`LedgerList.vue`)
- **路徑**: `/accounting/ledgers`
- **功能**: 顯示帳簿列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (LedgerSearchForm)
  - 資料表格 (LedgerDataTable)
  - 新增/修改對話框 (LedgerDialog)
  - 刪除確認對話框

#### 4.1.2 帳簿詳細頁面 (`LedgerDetail.vue`)
- **路徑**: `/accounting/ledgers/:ledgerId`
- **功能**: 顯示帳簿詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LedgerSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="帳簿代號">
      <el-input v-model="searchForm.ledgerId" placeholder="請輸入帳簿代號" />
    </el-form-item>
    <el-form-item label="帳簿名稱">
      <el-input v-model="searchForm.ledgerName" placeholder="請輸入帳簿名稱" />
    </el-form-item>
    <el-form-item label="帳簿類型">
      <el-select v-model="searchForm.ledgerType" placeholder="請選擇帳簿類型">
        <el-option label="總帳" value="GENERAL" />
        <el-option label="明細帳" value="DETAIL" />
        <el-option label="日記帳" value="JOURNAL" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
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

#### 4.2.2 資料表格元件 (`LedgerDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="ledgerList" v-loading="loading">
      <el-table-column prop="ledgerId" label="帳簿代號" width="120" />
      <el-table-column prop="ledgerName" label="帳簿名稱" width="200" />
      <el-table-column prop="ledgerType" label="帳簿類型" width="100">
        <template #default="{ row }">
          {{ getLedgerTypeText(row.ledgerType) }}
        </template>
      </el-table-column>
      <el-table-column prop="periodType" label="會計期間" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>
      </el-table-column>
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

#### 4.2.3 新增/修改對話框 (`LedgerDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="帳簿代號" prop="ledgerId">
        <el-input v-model="form.ledgerId" :disabled="isEdit" placeholder="請輸入帳簿代號" />
      </el-form-item>
      <el-form-item label="帳簿名稱" prop="ledgerName">
        <el-input v-model="form.ledgerName" placeholder="請輸入帳簿名稱" />
      </el-form-item>
      <el-form-item label="帳簿類型" prop="ledgerType">
        <el-select v-model="form.ledgerType" placeholder="請選擇帳簿類型">
          <el-option label="總帳" value="GENERAL" />
          <el-option label="明細帳" value="DETAIL" />
          <el-option label="日記帳" value="JOURNAL" />
        </el-select>
      </el-form-item>
      <el-form-item label="會計期間類型" prop="periodType">
        <el-select v-model="form.periodType" placeholder="請選擇會計期間類型">
          <el-option label="月" value="MONTHLY" />
          <el-option label="季" value="QUARTERLY" />
          <el-option label="年" value="YEARLY" />
        </el-select>
      </el-form-item>
      <el-form-item label="起始日期" prop="startDate">
        <el-date-picker v-model="form.startDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="結束日期" prop="endDate">
        <el-date-picker v-model="form.endDate" type="date" placeholder="請選擇日期" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
        </el-select>
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

### 4.3 API 呼叫 (`ledger.api.ts`)
```typescript
import request from '@/utils/request';

export interface LedgerDto {
  tKey: number;
  ledgerId: string;
  ledgerName: string;
  ledgerType?: string;
  periodType?: string;
  startDate?: string;
  endDate?: string;
  status: string;
  notes?: string;
  ledgerOrder?: number;
  accountSubjects?: string[];
}

export interface LedgerQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    ledgerId?: string;
    ledgerName?: string;
    ledgerType?: string;
    status?: string;
  };
}

export interface CreateLedgerDto {
  ledgerId: string;
  ledgerName: string;
  ledgerType?: string;
  periodType?: string;
  startDate?: string;
  endDate?: string;
  status: string;
  notes?: string;
  ledgerOrder?: number;
  accountSubjects?: string[];
}

export interface UpdateLedgerDto extends Omit<CreateLedgerDto, 'ledgerId'> {}

// API 函數
export const getLedgerList = (query: LedgerQueryDto) => {
  return request.get<ApiResponse<PagedResult<LedgerDto>>>('/api/v1/ledgers', { params: query });
};

export const getLedgerById = (ledgerId: string) => {
  return request.get<ApiResponse<LedgerDto>>(`/api/v1/ledgers/${ledgerId}`);
};

export const createLedger = (data: CreateLedgerDto) => {
  return request.post<ApiResponse<string>>('/api/v1/ledgers', data);
};

export const updateLedger = (ledgerId: string, data: UpdateLedgerDto) => {
  return request.put<ApiResponse>(`/api/v1/ledgers/${ledgerId}`, data);
};

export const deleteLedger = (ledgerId: string) => {
  return request.delete<ApiResponse>(`/api/v1/ledgers/${ledgerId}`);
};

export const updateStatus = (ledgerId: string, status: string) => {
  return request.put<ApiResponse>(`/api/v1/ledgers/${ledgerId}/status`, { status });
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

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
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

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須驗證帳簿代號的唯一性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 帳簿代號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除帳簿前必須檢查是否有相關資料
- 停用帳簿時必須檢查是否有進行中的業務
- 科目對應關係必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增帳簿成功
- [ ] 新增帳簿失敗 (重複代號)
- [ ] 修改帳簿成功
- [ ] 修改帳簿失敗 (不存在)
- [ ] 刪除帳簿成功
- [ ] 查詢帳簿列表成功
- [ ] 查詢單筆帳簿成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_PR.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN131_FS.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN132_*.ASP`

### 8.2 資料庫 Schema
- 舊系統資料表：`LEDGER`
- 主要欄位：`LEDGER_ID`, `LEDGER_NAME`, `LEDGER_TYPE`, `PERIOD_TYPE`, `START_DATE`, `END_DATE`, `STATUS`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

