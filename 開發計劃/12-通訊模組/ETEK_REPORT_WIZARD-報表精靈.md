# ETEK_REPORT_WIZARD - 報表精靈 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ETEK_REPORT_WIZARD
- **功能名稱**: 報表精靈
- **功能描述**: 提供報表快速建立與維護的精靈功能，包含頁面代碼、頁面名稱、頁面排序序號、主項目代碼等設定，支援多步驟精靈流程
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/ETEK_REPORT_WIZARD.asp` (報表精靈主程式)

### 1.2 業務需求
- 提供報表快速建立精靈
- 支援頁面代碼、頁面名稱、頁面排序序號設定
- 支援主項目代碼選擇
- 支援多步驟精靈流程（挑選表格、填入資料、完成）
- 自動建立報表相關資料表結構
- 支援報表查詢、列印功能設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportPrograms` (報表作業主檔)

```sql
CREATE TABLE [dbo].[ReportPrograms] (
    [ProgId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 頁面代碼 (PROG_ID)
    [ProgName] NVARCHAR(200) NOT NULL, -- 頁面名稱 (PROG_NAME)
    [ProgSeqNo] INT NOT NULL, -- 頁面排序序號 (PROG_SEQ_NO)
    [MenuId] NVARCHAR(50) NOT NULL, -- 主項目代碼 (MENU_ID)
    [TableName] NVARCHAR(100) NULL, -- 資料表名稱 (TABLE_NAME)
    [ReportType] NVARCHAR(50) NULL, -- 報表類型
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_ReportPrograms] PRIMARY KEY CLUSTERED ([ProgId] ASC),
    CONSTRAINT [FK_ReportPrograms_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([MenuId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportPrograms_MenuId] ON [dbo].[ReportPrograms] ([MenuId]);
CREATE NONCLUSTERED INDEX [IX_ReportPrograms_ProgSeqNo] ON [dbo].[ReportPrograms] ([ProgSeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `ReportColumns` - 報表欄位設定
```sql
CREATE TABLE [dbo].[ReportColumns] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProgId] NVARCHAR(50) NOT NULL, -- 頁面代碼
    [ColumnName] NVARCHAR(100) NOT NULL, -- 欄位名稱
    [ColumnLabel] NVARCHAR(200) NULL, -- 欄位標籤
    [DataType] NVARCHAR(50) NULL, -- 資料類型
    [SeqNo] INT NULL, -- 排序序號
    [IsVisible] BIT NOT NULL DEFAULT 1, -- 是否顯示
    [Width] INT NULL, -- 欄位寬度
    [Format] NVARCHAR(50) NULL, -- 格式化規則
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ReportColumns_ReportPrograms] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[ReportPrograms] ([ProgId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportColumns_ProgId] ON [dbo].[ReportColumns] ([ProgId]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ProgId | NVARCHAR | 50 | NO | - | 頁面代碼 | 主鍵 |
| ProgName | NVARCHAR | 200 | NO | - | 頁面名稱 | - |
| ProgSeqNo | INT | - | NO | - | 頁面排序序號 | - |
| MenuId | NVARCHAR | 50 | NO | - | 主項目代碼 | 外鍵至Menus |
| TableName | NVARCHAR | 100 | YES | - | 資料表名稱 | - |
| ReportType | NVARCHAR | 50 | YES | - | 報表類型 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 步驟一：挑選表格
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-report-wizard/step1`
- **說明**: 建立報表基本資料（頁面代碼、頁面名稱、頁面排序序號、主項目代碼）
- **請求格式**:
  ```json
  {
    "progId": "RPT001",
    "progName": "報表名稱",
    "progSeqNo": 1,
    "menuId": "MENU001"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "步驟一完成",
    "data": {
      "progId": "RPT001",
      "wizardId": "WIZARD001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 步驟二：填入資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-report-wizard/step2`
- **說明**: 設定報表資料表名稱和欄位資訊
- **請求格式**:
  ```json
  {
    "wizardId": "WIZARD001",
    "progId": "RPT001",
    "tableName": "TableName",
    "columns": [
      {
        "columnName": "Column1",
        "columnLabel": "欄位1",
        "dataType": "NVARCHAR",
        "seqNo": 1,
        "isVisible": true
      }
    ]
  }
  ```
- **回應格式**: 標準回應格式

#### 3.1.3 完成精靈
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-report-wizard/complete`
- **說明**: 完成報表精靈並建立所有相關資料
- **請求格式**:
  ```json
  {
    "wizardId": "WIZARD001",
    "progId": "RPT001"
  }
  ```
- **回應格式**: 標準回應格式

#### 3.1.4 查詢報表作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/etek-report-wizard/reports`
- **說明**: 查詢報表作業列表
- **請求參數**:
  - `pageIndex`: 頁碼
  - `pageSize`: 每頁筆數
  - `menuId`: 主項目代碼（選填）
  - `progId`: 頁面代碼（選填）
  - `progName`: 頁面名稱（選填）
- **回應格式**: 標準分頁列表回應

#### 3.1.5 查詢單筆報表作業
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/etek-report-wizard/reports/{progId}`
- **說明**: 查詢單筆報表作業詳細資料
- **回應格式**: 標準單筆資料回應

#### 3.1.6 修改報表作業
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/etek-report-wizard/reports/{progId}`
- **說明**: 修改報表作業資料
- **請求格式**: 同步驟一，但 `progId` 不可修改
- **回應格式**: 標準回應格式

#### 3.1.7 刪除報表作業
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/etek-report-wizard/reports/{progId}`
- **說明**: 刪除報表作業（需檢查是否有相關資料）
- **回應格式**: 標準回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `EtekReportWizardController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/etek-report-wizard")]
    [Authorize]
    public class EtekReportWizardController : ControllerBase
    {
        private readonly IEtekReportWizardService _wizardService;
        
        public EtekReportWizardController(IEtekReportWizardService wizardService)
        {
            _wizardService = wizardService;
        }
        
        [HttpPost("step1")]
        public async Task<ActionResult<ApiResponse<WizardStep1ResultDto>>> Step1([FromBody] WizardStep1Dto dto)
        {
            // 實作步驟一邏輯
        }
        
        [HttpPost("step2")]
        public async Task<ActionResult<ApiResponse>> Step2([FromBody] WizardStep2Dto dto)
        {
            // 實作步驟二邏輯
        }
        
        [HttpPost("complete")]
        public async Task<ActionResult<ApiResponse>> Complete([FromBody] WizardCompleteDto dto)
        {
            // 實作完成邏輯
        }
        
        [HttpGet("reports")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReportProgramDto>>>> GetReports([FromQuery] ReportProgramQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("reports/{progId}")]
        public async Task<ActionResult<ApiResponse<ReportProgramDto>>> GetReport(string progId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPut("reports/{progId}")]
        public async Task<ActionResult<ApiResponse>> UpdateReport(string progId, [FromBody] UpdateReportProgramDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("reports/{progId}")]
        public async Task<ActionResult<ApiResponse>> DeleteReport(string progId)
        {
            // 實作刪除邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 報表精靈頁面 (`EtekReportWizard.vue`)
- **路徑**: `/xcom/etek-report-wizard`
- **功能**: 多步驟精靈流程，建立報表作業
- **主要元件**:
  - 步驟指示器 (WizardSteps)
  - 步驟一表單 (Step1Form)
  - 步驟二表單 (Step2Form)
  - 完成確認 (CompleteConfirmation)

#### 4.1.2 報表作業列表頁面 (`ReportProgramList.vue`)
- **路徑**: `/xcom/report-programs`
- **功能**: 顯示報表作業列表，支援查詢、修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 步驟一表單元件 (`Step1Form.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
    <el-form-item label="頁面代碼" prop="progId">
      <el-input v-model="form.progId" placeholder="請輸入頁面代碼" @blur="checkProgId" />
    </el-form-item>
    <el-form-item label="頁面名稱" prop="progName">
      <el-input v-model="form.progName" placeholder="請輸入頁面名稱" />
    </el-form-item>
    <el-form-item label="頁面排序序號" prop="progSeqNo">
      <el-input-number v-model="form.progSeqNo" :min="1" />
    </el-form-item>
    <el-form-item label="主項目代碼" prop="menuId">
      <el-input v-model="form.menuId" placeholder="請選擇主項目代碼" readonly>
        <template #append>
          <el-button @click="selectMenu">選擇</el-button>
        </template>
      </el-input>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 步驟二表單元件 (`Step2Form.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
    <el-form-item label="資料表名稱" prop="tableName">
      <el-input v-model="form.tableName" placeholder="請輸入資料表名稱" />
    </el-form-item>
    <el-form-item label="報表欄位">
      <el-table :data="form.columns">
        <el-table-column prop="columnName" label="欄位名稱" />
        <el-table-column prop="columnLabel" label="欄位標籤" />
        <el-table-column prop="dataType" label="資料類型" />
        <el-table-column prop="seqNo" label="排序序號" />
        <el-table-column label="操作">
          <template #default="{ row, $index }">
            <el-button type="danger" size="small" @click="removeColumn($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-button type="primary" @click="addColumn">新增欄位</el-button>
    </el-form-item>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`etekReportWizard.api.ts`)
```typescript
import request from '@/utils/request';

export interface WizardStep1Dto {
  progId: string;
  progName: string;
  progSeqNo: number;
  menuId: string;
}

export interface WizardStep2Dto {
  wizardId: string;
  progId: string;
  tableName: string;
  columns: ReportColumnDto[];
}

export interface ReportColumnDto {
  columnName: string;
  columnLabel: string;
  dataType: string;
  seqNo: number;
  isVisible: boolean;
}

// API 函數
export const wizardStep1 = (data: WizardStep1Dto) => {
  return request.post<ApiResponse<WizardStep1ResultDto>>('/api/v1/etek-report-wizard/step1', data);
};

export const wizardStep2 = (data: WizardStep2Dto) => {
  return request.post<ApiResponse>('/api/v1/etek-report-wizard/step2', data);
};

export const wizardComplete = (data: WizardCompleteDto) => {
  return request.post<ApiResponse>('/api/v1/etek-report-wizard/complete', data);
};

export const getReportPrograms = (query: ReportProgramQueryDto) => {
  return request.get<ApiResponse<PagedResult<ReportProgramDto>>>('/api/v1/etek-report-wizard/reports', { params: query });
};

export const getReportProgram = (progId: string) => {
  return request.get<ApiResponse<ReportProgramDto>>(`/api/v1/etek-report-wizard/reports/${progId}`);
};

export const updateReportProgram = (progId: string, data: UpdateReportProgramDto) => {
  return request.put<ApiResponse>(`/api/v1/etek-report-wizard/reports/${progId}`, data);
};

export const deleteReportProgram = (progId: string) => {
  return request.delete<ApiResponse>(`/api/v1/etek-report-wizard/reports/${progId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（頁面代碼唯一性、必填欄位驗證等）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 精靈頁面開發
- [ ] 步驟一表單開發
- [ ] 步驟二表單開發
- [ ] 報表作業列表頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 業務邏輯測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 8.5天

---

## 六、注意事項

### 6.1 業務邏輯
- 頁面代碼必須唯一
- 新增時需檢查頁面代碼是否已存在
- 頁面排序序號必須為正整數
- 主項目代碼必須存在於選單主檔中
- 刪除時需檢查是否有相關報表資料

### 6.2 資料驗證
- 頁面代碼必須唯一
- 必填欄位必須驗證
- 頁面排序序號必須為正整數

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 步驟一成功
- [ ] 步驟一失敗 (重複頁面代碼)
- [ ] 步驟二成功
- [ ] 完成精靈成功
- [ ] 查詢報表作業列表成功
- [ ] 修改報表作業成功
- [ ] 刪除報表作業成功

### 7.2 整合測試
- [ ] 完整精靈流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/ETEK_REPORT_WIZARD.asp` - 報表精靈主程式

### 8.2 資料庫 Schema
- 舊系統資料表：`MNG_PROG` (作業主檔)
- 主要欄位：PROG_ID, PROG_NAME, PROG_SEQ_NO, MENU_ID

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

