# XCOMA02 - 系統功能按鈕資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOMA02
- **功能名稱**: 系統功能按鈕資料維護
- **功能描述**: 提供通訊模組系統功能按鈕資料的新增、修改、刪除、查詢功能，包含按鈕代碼、按鈕名稱、作業代碼、頁面代碼、按鈕訊息、按鈕屬性、網頁鏈結位址、訊息型態等資訊管理
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOMA02.ascx` (主程式)
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOMA02.ascx.cs` (後端邏輯)
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FI.ASP` (參考新增功能)
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FU.ASP` (參考修改功能)
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FD.ASP` (參考刪除功能)
  - `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FQ.asp` (參考查詢功能)

### 1.2 業務需求
- 管理通訊模組系統功能按鈕資訊
- 支援按鈕的新增、修改、刪除、查詢
- 記錄按鈕的建立與變更資訊
- 支援訊息型態設定
- 與作業、權限系統整合
- 支援通訊模組特定的按鈕配置

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XComButtons` (對應舊系統 `MNG_BUTTON`，XCOM模組專用)

```sql
CREATE TABLE [dbo].[XComButtons] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [ProgramId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROG_ID)
    [ButtonId] NVARCHAR(50) NOT NULL, -- 按鈕代碼 (BUTTON_ID)
    [ButtonName] NVARCHAR(100) NOT NULL, -- 按鈕名稱 (BUTTON_NAME)
    [PageId] NVARCHAR(50) NULL, -- 頁面代碼 (PAGE_ID)
    [ButtonMsg] NVARCHAR(500) NULL, -- 按鈕訊息 (BUTTON_MSG)
    [ButtonAttr] NVARCHAR(50) NULL, -- 按鈕屬性 (BUTTON_ATTR)
    [ButtonUrl] NVARCHAR(500) NULL, -- 網頁鏈結位址 (URL)
    [MsgType] NVARCHAR(20) NULL, -- 訊息型態 (MSG_TYPE)
    [XComModule] NVARCHAR(50) NULL, -- 通訊模組代碼 (XCOM_MODULE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_XComButtons] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_XComButtons_Programs] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[Programs] ([ProgramId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XComButtons_ProgramId] ON [dbo].[XComButtons] ([ProgramId]);
CREATE NONCLUSTERED INDEX [IX_XComButtons_ButtonId] ON [dbo].[XComButtons] ([ButtonId]);
CREATE NONCLUSTERED INDEX [IX_XComButtons_PageId] ON [dbo].[XComButtons] ([PageId]);
CREATE NONCLUSTERED INDEX [IX_XComButtons_XComModule] ON [dbo].[XComButtons] ([XComModule]);
CREATE NONCLUSTERED INDEX [IX_XComButtons_ProgramId_PageId_ButtonId] ON [dbo].[XComButtons] ([ProgramId], [PageId], [ButtonId]);
```

### 2.2 相關資料表

#### 2.2.1 `Programs` - 作業主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md`

#### 2.2.2 `Parameters` - 參數表
- 用於查詢訊息型態選項
- 參考參數表 `PARAM` (TITLE='BUT_MSG')

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | - | 主鍵 | IDENTITY(1,1) |
| ProgramId | NVARCHAR | 50 | NO | - | 作業代碼 | 外鍵至Programs |
| ButtonId | NVARCHAR | 50 | NO | - | 按鈕代碼 | - |
| ButtonName | NVARCHAR | 100 | NO | - | 按鈕名稱 | - |
| PageId | NVARCHAR | 50 | YES | - | 頁面代碼 | - |
| ButtonMsg | NVARCHAR | 500 | YES | - | 按鈕訊息 | - |
| ButtonAttr | NVARCHAR | 50 | YES | - | 按鈕屬性 | - |
| ButtonUrl | NVARCHAR | 500 | YES | - | 網頁鏈結位址 | - |
| MsgType | NVARCHAR | 20 | YES | - | 訊息型態 | 參考參數表 PARAM (TITLE='BUT_MSG') |
| XComModule | NVARCHAR | 50 | YES | - | 通訊模組代碼 | XCOM模組專用欄位 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢按鈕列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom/buttons`
- **說明**: 查詢通訊模組按鈕列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ButtonId",
    "sortOrder": "ASC",
    "filters": {
      "programId": "",
      "buttonId": "",
      "buttonName": "",
      "pageId": "",
      "xComModule": ""
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
          "programId": "XCOMA02",
          "programName": "系統功能按鈕資料維護",
          "buttonId": "BTN001",
          "buttonName": "新增",
          "pageId": "FI",
          "buttonMsg": "確定要新增嗎？",
          "buttonAttr": "1",
          "buttonUrl": "/api/v1/xcom/buttons",
          "msgType": "1",
          "msgTypeName": "確認訊息",
          "xComModule": "XCOM000",
          "status": "1",
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

#### 3.1.2 查詢單筆按鈕
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom/buttons/{tKey}`
- **說明**: 根據按鈕主鍵查詢單筆按鈕資料
- **路徑參數**:
  - `tKey`: 按鈕主鍵
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增按鈕
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom/buttons`
- **說明**: 新增通訊模組按鈕資料
- **請求格式**:
  ```json
  {
    "programId": "XCOMA02",
    "buttonId": "BTN001",
    "buttonName": "新增",
    "pageId": "FI",
    "buttonMsg": "確定要新增嗎？",
    "buttonAttr": "1",
    "buttonUrl": "/api/v1/xcom/buttons",
    "msgType": "1",
    "xComModule": "XCOM000"
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

#### 3.1.4 修改按鈕
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom/buttons/{tKey}`
- **說明**: 修改通訊模組按鈕資料
- **路徑參數**:
  - `tKey`: 按鈕主鍵
- **請求格式**: 同新增
- **回應格式**: 同新增

#### 3.1.5 刪除按鈕
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom/buttons/{tKey}`
- **說明**: 刪除通訊模組按鈕資料（軟刪除或硬刪除）
- **路徑參數**:
  - `tKey`: 按鈕主鍵
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

#### 3.1.6 批次刪除按鈕
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom/buttons/batch`
- **說明**: 批次刪除多筆按鈕
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

### 3.2 資料傳輸物件 (DTO)

#### 3.2.1 `XComButtonQueryDto`
```csharp
public class XComButtonQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public XComButtonFilterDto? Filters { get; set; }
}

public class XComButtonFilterDto
{
    public string? ProgramId { get; set; }
    public string? ButtonId { get; set; }
    public string? ButtonName { get; set; }
    public string? PageId { get; set; }
    public string? XComModule { get; set; }
}
```

#### 3.2.2 `XComButtonDto`
```csharp
public class XComButtonDto
{
    public long TKey { get; set; }
    public string ProgramId { get; set; } = string.Empty;
    public string? ProgramName { get; set; }
    public string ButtonId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public string? PageId { get; set; }
    public string? ButtonMsg { get; set; }
    public string? ButtonAttr { get; set; }
    public string? ButtonUrl { get; set; }
    public string? MsgType { get; set; }
    public string? MsgTypeName { get; set; }
    public string? XComModule { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### 3.2.3 `CreateXComButtonDto`
```csharp
public class CreateXComButtonDto
{
    [Required(ErrorMessage = "作業代碼為必填")]
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string ProgramId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "按鈕代碼為必填")]
    [StringLength(50, ErrorMessage = "按鈕代碼長度不能超過50")]
    public string ButtonId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "按鈕名稱為必填")]
    [StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string ButtonName { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "頁面代碼長度不能超過50")]
    public string? PageId { get; set; }
    
    [StringLength(500, ErrorMessage = "按鈕訊息長度不能超過500")]
    public string? ButtonMsg { get; set; }
    
    [StringLength(50, ErrorMessage = "按鈕屬性長度不能超過50")]
    public string? ButtonAttr { get; set; }
    
    [StringLength(500, ErrorMessage = "網頁鏈結位址長度不能超過500")]
    public string? ButtonUrl { get; set; }
    
    [StringLength(20, ErrorMessage = "訊息型態長度不能超過20")]
    public string? MsgType { get; set; }
    
    [StringLength(50, ErrorMessage = "通訊模組代碼長度不能超過50")]
    public string? XComModule { get; set; }
}
```

#### 3.2.4 `UpdateXComButtonDto`
```csharp
public class UpdateXComButtonDto
{
    [Required(ErrorMessage = "作業代碼為必填")]
    [StringLength(50, ErrorMessage = "作業代碼長度不能超過50")]
    public string ProgramId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "按鈕代碼為必填")]
    [StringLength(50, ErrorMessage = "按鈕代碼長度不能超過50")]
    public string ButtonId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "按鈕名稱為必填")]
    [StringLength(100, ErrorMessage = "按鈕名稱長度不能超過100")]
    public string ButtonName { get; set; } = string.Empty;
    
    [StringLength(50, ErrorMessage = "頁面代碼長度不能超過50")]
    public string? PageId { get; set; }
    
    [StringLength(500, ErrorMessage = "按鈕訊息長度不能超過500")]
    public string? ButtonMsg { get; set; }
    
    [StringLength(50, ErrorMessage = "按鈕屬性長度不能超過50")]
    public string? ButtonAttr { get; set; }
    
    [StringLength(500, ErrorMessage = "網頁鏈結位址長度不能超過500")]
    public string? ButtonUrl { get; set; }
    
    [StringLength(20, ErrorMessage = "訊息型態長度不能超過20")]
    public string? MsgType { get; set; }
    
    [StringLength(50, ErrorMessage = "通訊模組代碼長度不能超過50")]
    public string? XComModule { get; set; }
}
```

### 3.3 Service 層設計

#### 3.3.1 `IXComButtonService`
```csharp
public interface IXComButtonService
{
    Task<PagedResult<XComButtonDto>> GetButtonsAsync(XComButtonQueryDto query, CancellationToken cancellationToken = default);
    Task<XComButtonDto> GetButtonByIdAsync(long tKey, CancellationToken cancellationToken = default);
    Task<long> CreateButtonAsync(CreateXComButtonDto dto, CancellationToken cancellationToken = default);
    Task UpdateButtonAsync(long tKey, UpdateXComButtonDto dto, CancellationToken cancellationToken = default);
    Task DeleteButtonAsync(long tKey, CancellationToken cancellationToken = default);
    Task BatchDeleteButtonsAsync(List<long> tKeys, CancellationToken cancellationToken = default);
}
```

#### 3.3.2 `XComButtonService` 實作重點
- 作業代碼必須存在
- 刪除前需檢查是否有權限設定
- 訊息型態需從參數表讀取
- 通訊模組代碼驗證

### 3.4 Repository 層設計

#### 3.4.1 `IXComButtonRepository`
```csharp
public interface IXComButtonRepository
{
    Task<XComButton?> GetByIdAsync(long tKey, CancellationToken cancellationToken = default);
    Task<PagedResult<XComButton>> GetPagedAsync(XComButtonQuery query, CancellationToken cancellationToken = default);
    Task<XComButton> CreateAsync(XComButton button, CancellationToken cancellationToken = default);
    Task<XComButton> UpdateAsync(XComButton button, CancellationToken cancellationToken = default);
    Task DeleteAsync(long tKey, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionsAsync(long tKey, CancellationToken cancellationToken = default);
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 按鈕列表頁面 (`XComButtonList.vue`)
- **路徑**: `/xcom/buttons`
- **功能**: 顯示通訊模組按鈕列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (XComButtonSearchForm)
  - 資料表格 (XComButtonDataTable)
  - 新增/修改對話框 (XComButtonDialog)
  - 刪除確認對話框

#### 4.1.2 按鈕詳細頁面 (`XComButtonDetail.vue`)
- **路徑**: `/xcom/buttons/:tKey`
- **功能**: 顯示按鈕詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`XComButtonSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="作業代碼">
      <el-select v-model="searchForm.programId" placeholder="請選擇作業" clearable filterable>
        <el-option v-for="program in programList" :key="program.programId" :label="program.programName" :value="program.programId" />
      </el-select>
    </el-form-item>
    <el-form-item label="按鈕代碼">
      <el-input v-model="searchForm.buttonId" placeholder="請輸入按鈕代碼" />
    </el-form-item>
    <el-form-item label="按鈕名稱">
      <el-input v-model="searchForm.buttonName" placeholder="請輸入按鈕名稱" />
    </el-form-item>
    <el-form-item label="頁面代碼">
      <el-input v-model="searchForm.pageId" placeholder="請輸入頁面代碼" />
    </el-form-item>
    <el-form-item label="通訊模組">
      <el-select v-model="searchForm.xComModule" placeholder="請選擇通訊模組" clearable>
        <el-option label="XCOM000" value="XCOM000" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`XComButtonDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="buttonList" v-loading="loading">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="programName" label="作業" width="200" />
      <el-table-column prop="buttonId" label="按鈕代碼" width="120" />
      <el-table-column prop="buttonName" label="按鈕名稱" width="150" />
      <el-table-column prop="pageId" label="頁面代碼" width="100" />
      <el-table-column prop="buttonMsg" label="按鈕訊息" width="200" show-overflow-tooltip />
      <el-table-column prop="buttonUrl" label="網頁鏈結位址" width="200" show-overflow-tooltip />
      <el-table-column prop="msgTypeName" label="訊息型態" width="120" />
      <el-table-column prop="xComModule" label="通訊模組" width="120" />
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

#### 4.2.3 新增/修改對話框 (`XComButtonDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="作業代碼" prop="programId">
        <el-select v-model="form.programId" placeholder="請選擇作業" filterable>
          <el-option v-for="program in programList" :key="program.programId" :label="program.programName" :value="program.programId" />
        </el-select>
      </el-form-item>
      <el-form-item label="按鈕代碼" prop="buttonId">
        <el-input v-model="form.buttonId" placeholder="請輸入按鈕代碼" />
      </el-form-item>
      <el-form-item label="按鈕名稱" prop="buttonName">
        <el-input v-model="form.buttonName" placeholder="請輸入按鈕名稱" />
      </el-form-item>
      <el-form-item label="頁面代碼" prop="pageId">
        <el-input v-model="form.pageId" placeholder="請輸入頁面代碼" />
      </el-form-item>
      <el-form-item label="按鈕訊息" prop="buttonMsg">
        <el-input v-model="form.buttonMsg" type="textarea" :rows="3" placeholder="請輸入按鈕訊息" />
      </el-form-item>
      <el-form-item label="按鈕屬性" prop="buttonAttr">
        <el-input v-model="form.buttonAttr" placeholder="請輸入按鈕屬性" />
      </el-form-item>
      <el-form-item label="網頁鏈結位址" prop="buttonUrl">
        <el-input v-model="form.buttonUrl" placeholder="請輸入網頁鏈結位址" />
      </el-form-item>
      <el-form-item label="訊息型態" prop="msgType">
        <el-select v-model="form.msgType" placeholder="請選擇訊息型態" clearable>
          <el-option v-for="type in msgTypeList" :key="type.value" :label="type.label" :value="type.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="通訊模組" prop="xComModule">
        <el-select v-model="form.xComModule" placeholder="請選擇通訊模組" clearable>
          <el-option label="XCOM000" value="XCOM000" />
        </el-select>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom-button.api.ts`)
```typescript
import request from '@/utils/request';

export interface XComButtonDto {
  tKey: number;
  programId: string;
  programName?: string;
  buttonId: string;
  buttonName: string;
  pageId?: string;
  buttonMsg?: string;
  buttonAttr?: string;
  buttonUrl?: string;
  msgType?: string;
  msgTypeName?: string;
  xComModule?: string;
  status: string;
  createdBy?: string;
  createdAt: string;
  updatedBy?: string;
  updatedAt: string;
}

export interface XComButtonQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    programId?: string;
    buttonId?: string;
    buttonName?: string;
    pageId?: string;
    xComModule?: string;
  };
}

export interface CreateXComButtonDto {
  programId: string;
  buttonId: string;
  buttonName: string;
  pageId?: string;
  buttonMsg?: string;
  buttonAttr?: string;
  buttonUrl?: string;
  msgType?: string;
  xComModule?: string;
}

export interface UpdateXComButtonDto extends CreateXComButtonDto {}

// API 函數
export const getXComButtonList = (query: XComButtonQueryDto) => {
  return request.get<ApiResponse<PagedResult<XComButtonDto>>>('/api/v1/xcom/buttons', { params: query });
};

export const getXComButtonById = (tKey: number) => {
  return request.get<ApiResponse<XComButtonDto>>(`/api/v1/xcom/buttons/${tKey}`);
};

export const createXComButton = (data: CreateXComButtonDto) => {
  return request.post<ApiResponse<number>>('/api/v1/xcom/buttons', data);
};

export const updateXComButton = (tKey: number, data: UpdateXComButtonDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom/buttons/${tKey}`, data);
};

export const deleteXComButton = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom/buttons/${tKey}`);
};

export const batchDeleteXComButtons = (tKeys: number[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom/buttons/batch', { data: { tKeys } });
};
```

---

## 五、後端實作類別

### 5.1 Controller: `XComButtonsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom/buttons")]
    [Authorize]
    public class XComButtonsController : ControllerBase
    {
        private readonly IXComButtonService _buttonService;
        
        public XComButtonsController(IXComButtonService buttonService)
        {
            _buttonService = buttonService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<XComButtonDto>>>> GetButtons([FromQuery] XComButtonQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<XComButtonDto>>> GetButton(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateButton([FromBody] CreateXComButtonDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateButton(long tKey, [FromBody] UpdateXComButtonDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteButton(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> BatchDeleteButtons([FromBody] BatchDeleteButtonsRequestDto request)
        {
            // 實作批次刪除邏輯
        }
    }
}
```

### 5.2 Service: `XComButtonService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXComButtonService
    {
        Task<PagedResult<XComButtonDto>> GetButtonsAsync(XComButtonQueryDto query);
        Task<XComButtonDto> GetButtonByIdAsync(long tKey);
        Task<long> CreateButtonAsync(CreateXComButtonDto dto);
        Task UpdateButtonAsync(long tKey, UpdateXComButtonDto dto);
        Task DeleteButtonAsync(long tKey);
        Task BatchDeleteButtonsAsync(List<long> tKeys);
    }
}
```

### 5.3 Repository: `XComButtonRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXComButtonRepository
    {
        Task<XComButton> GetByIdAsync(long tKey);
        Task<PagedResult<XComButton>> GetPagedAsync(XComButtonQuery query);
        Task<XComButton> CreateAsync(XComButton button);
        Task<XComButton> UpdateAsync(XComButton button);
        Task DeleteAsync(long tKey);
        Task<bool> HasPermissionsAsync(long tKey);
    }
}
```

---

## 六、開發時程

### 6.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 6.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 6.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 6.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

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

### 7.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 複合索引 (ProgramId, PageId, ButtonId) 用於提升查詢效能

### 7.3 資料驗證
- 必填欄位必須驗證
- 作業代碼必須存在
- 頁面代碼格式驗證
- 網頁鏈結位址格式驗證
- 通訊模組代碼驗證

### 7.4 業務邏輯
- 刪除按鈕前必須檢查是否有權限設定
- 訊息型態需從參數表讀取
- 按鈕屬性需符合系統規範
- 通訊模組代碼需符合XCOM模組規範

---

## 八、測試案例

### 8.1 單元測試
- [ ] 新增按鈕成功
- [ ] 新增按鈕失敗 (作業不存在)
- [ ] 修改按鈕成功
- [ ] 修改按鈕失敗 (不存在)
- [ ] 刪除按鈕成功
- [ ] 刪除按鈕失敗 (有權限設定)
- [ ] 查詢按鈕列表成功
- [ ] 查詢單筆按鈕成功

### 8.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 8.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 九、參考資料

### 9.1 舊程式碼
- `IMS3/HANSHIN/IMS3/XCOM000/XCOMA02.ascx`
- `IMS3/HANSHIN/IMS3/XCOM000/XCOMA02.ascx.cs`
- `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FI.ASP`
- `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FU.ASP`
- `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FD.ASP`
- `WEB/IMS_CORE/ASP/SYS0000/SYS0440_FQ.asp`

### 9.2 相關功能
- SYS0440 - 系統功能按鈕資料維護
- SYS0430 - 系統作業資料維護
- SYS0310 - 角色系統權限設定
- SYS0320 - 使用者系統權限設定

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

