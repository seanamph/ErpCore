# XCOM190 - 布局管理維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM190
- **功能名稱**: 布局管理維護作業
- **功能描述**: 提供系統頁面布局配置的新增、修改、刪除、查詢功能，包含作業代碼(PROG_ID)、頁面代碼(PAGE_ID)、序號(SEQ_NO)、表格名稱(TABLE_NAME)、標題(TITLE)、樣式(STYLE)等資訊管理
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOM190.ascx` (使用者控件)
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_FB.aspx` (瀏覽)
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_FI.aspx` (新增)
  - `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_AJAX.aspx` (AJAX處理)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM190_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理系統頁面布局配置
- 支援作業代碼(PROG_ID)和頁面代碼(PAGE_ID)組合維護
- 支援布局區塊序號(SEQ_NO)維護
- 支援表格名稱(TABLE_NAME)設定
- 支援標題(TITLE)維護
- 支援樣式(STYLE)維護
- 支援欄位配置維護（MNG_LAYOUT_D）
- 支援臨時表(MNG_LAYOUT_M_TEMP)功能
- 支援布局預覽功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LayoutM` (布局主檔，對應舊系統 `MNG_LAYOUT_M`)

```sql
CREATE TABLE [dbo].[LayoutM] (
    [LayoutMId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProgId] NVARCHAR(50) NOT NULL, -- 作業代碼
    [PageId] NVARCHAR(50) NOT NULL, -- 頁面代碼
    [SeqNo] INT NOT NULL, -- 序號
    [TableName] NVARCHAR(200) NULL, -- 表格名稱
    [NextUrl] NVARCHAR(500) NULL, -- 下一頁URL
    [KeyFields] NVARCHAR(500) NULL, -- 主鍵欄位
    [Title] NVARCHAR(200) NULL, -- 標題
    [Style] NVARCHAR(50) NULL, -- 樣式
    [InputCnt] INT NULL DEFAULT 1, -- 輸入筆數
    [TabName] NVARCHAR(200) NULL, -- 標籤名稱
    [ColumnCnt] INT NULL DEFAULT 1, -- 欄位數
    [KeyFieldsM] NVARCHAR(500) NULL, -- 主鍵欄位M
    [ShowDispCol] NVARCHAR(500) NULL, -- 顯示欄位
    [Status] NVARCHAR(10) NULL DEFAULT '1', -- 狀態
    [SqlTable] NVARCHAR(200) NULL, -- SQL表格
    [Attr] NVARCHAR(MAX) NULL, -- 屬性
    [AttrValue] NVARCHAR(MAX) NULL, -- 屬性值
    [SqlOrder] NVARCHAR(500) NULL, -- SQL排序
    [Fun] NVARCHAR(500) NULL, -- 函數
    [LvOp] NVARCHAR(50) NULL, -- 層級操作
    [SelectRows] INT NULL DEFAULT 1, -- 選擇行數
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_LayoutM] PRIMARY KEY CLUSTERED ([LayoutMId] ASC),
    CONSTRAINT [UQ_LayoutM_ProgId_PageId_SeqNo] UNIQUE ([ProgId], [PageId], [SeqNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LayoutM_ProgId] ON [dbo].[LayoutM] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_LayoutM_PageId] ON [dbo].[LayoutM] ([PageId]);
CREATE NONCLUSTERED INDEX [IX_LayoutM_ProgId_PageId] ON [dbo].[LayoutM] ([ProgId], [PageId]);
CREATE NONCLUSTERED INDEX [IX_LayoutM_Status] ON [dbo].[LayoutM] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `LayoutD` - 布局明細檔 (對應舊系統 `MNG_LAYOUT_D`)
```sql
CREATE TABLE [dbo].[LayoutD] (
    [LayoutDId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProgId] NVARCHAR(50) NOT NULL, -- 作業代碼
    [PageId] NVARCHAR(50) NOT NULL, -- 頁面代碼
    [MSeqNo] INT NOT NULL, -- 主檔序號
    [SeqNo] INT NOT NULL, -- 明細序號
    [FieldId] NVARCHAR(100) NOT NULL, -- 欄位代碼
    [FieldName] NVARCHAR(200) NULL, -- 欄位名稱
    [ShowLayout] NVARCHAR(50) NULL, -- 顯示布局
    [MustKeyinYn] NVARCHAR(10) NULL DEFAULT 'N', -- 必填
    [ReadonlyYn] NVARCHAR(10) NULL DEFAULT 'N', -- 唯讀
    [Maxlength] INT NULL, -- 最大長度
    [InputSize] INT NULL, -- 輸入大小
    [ListSql] NVARCHAR(MAX) NULL, -- 列表SQL
    [BtnEtekYn] NVARCHAR(10) NULL DEFAULT 'N', -- 按鈕ETEK
    [FqSelected] NVARCHAR(10) NULL DEFAULT 'N', -- 查詢選中
    [DataType] NVARCHAR(50) NULL, -- 資料類型
    [DefaultValue] NVARCHAR(500) NULL, -- 預設值
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LayoutD_LayoutM] FOREIGN KEY ([ProgId], [PageId], [MSeqNo]) 
        REFERENCES [dbo].[LayoutM] ([ProgId], [PageId], [SeqNo]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LayoutD_ProgId_PageId_MSeqNo] ON [dbo].[LayoutD] ([ProgId], [PageId], [MSeqNo]);
CREATE NONCLUSTERED INDEX [IX_LayoutD_FieldId] ON [dbo].[LayoutD] ([FieldId]);
```

#### 2.2.2 `LayoutMTemp` - 布局主檔臨時表 (對應舊系統 `MNG_LAYOUT_M_TEMP`)
```sql
CREATE TABLE [dbo].[LayoutMTemp] (
    -- 結構同 LayoutM
    [LayoutMId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProgId] NVARCHAR(50) NOT NULL,
    [PageId] NVARCHAR(50) NOT NULL,
    [SeqNo] INT NOT NULL,
    -- ... 其他欄位同 LayoutM
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_LayoutMTemp_ProgId_PageId_SeqNo] UNIQUE ([ProgId], [PageId], [SeqNo])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LayoutMId | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ProgId | NVARCHAR | 50 | NO | - | 作業代碼 | 與PageId、SeqNo組成唯一鍵 |
| PageId | NVARCHAR | 50 | NO | - | 頁面代碼 | - |
| SeqNo | INT | - | NO | - | 序號 | - |
| TableName | NVARCHAR | 200 | YES | - | 表格名稱 | - |
| NextUrl | NVARCHAR | 500 | YES | - | 下一頁URL | - |
| KeyFields | NVARCHAR | 500 | YES | - | 主鍵欄位 | - |
| Title | NVARCHAR | 200 | YES | - | 標題 | - |
| Style | NVARCHAR | 50 | YES | - | 樣式 | - |
| InputCnt | INT | - | YES | 1 | 輸入筆數 | - |
| TabName | NVARCHAR | 200 | YES | - | 標籤名稱 | - |
| ColumnCnt | INT | - | YES | 1 | 欄位數 | - |
| Status | NVARCHAR | 10 | YES | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢布局列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom190/layouts`
- **說明**: 查詢布局列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ProgId",
    "sortOrder": "ASC",
    "filters": {
      "progId": "",
      "pageId": "",
      "progName": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆布局
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}`
- **說明**: 根據作業代碼、頁面代碼、序號查詢單筆布局資料
- **路徑參數**:
  - `progId`: 作業代碼
  - `pageId`: 頁面代碼
  - `seqNo`: 序號
- **回應格式**: 標準單筆回應格式

#### 3.1.3 查詢布局明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}/details`
- **說明**: 查詢布局明細資料
- **回應格式**: 標準列表回應格式

#### 3.1.4 新增布局
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom190/layouts`
- **說明**: 新增布局資料
- **請求格式**:
  ```json
  {
    "progId": "XCOM190",
    "pageId": "B",
    "seqNo": 1,
    "tableName": "MNG_LAYOUT_M",
    "title": "布局標題",
    "style": "01",
    "inputCnt": 1,
    "columnCnt": 1,
    "status": "1",
    "details": [
      {
        "fieldId": "FIELD001",
        "fieldName": "欄位名稱",
        "showLayout": "01",
        "mustKeyinYn": "N",
        "readonlyYn": "N"
      }
    ]
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.5 修改布局
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}`
- **說明**: 修改布局資料
- **請求格式**: 同新增
- **回應格式**: 標準修改回應格式

#### 3.1.6 刪除布局
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}`
- **說明**: 刪除布局資料
- **回應格式**: 標準刪除回應格式

#### 3.1.7 複製布局到臨時表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}/copy-to-temp`
- **說明**: 複製布局到臨時表
- **回應格式**: 標準操作回應格式

#### 3.1.8 從臨時表還原布局
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom190/layouts/{progId}/{pageId}/{seqNo}/restore-from-temp`
- **說明**: 從臨時表還原布局
- **回應格式**: 標準操作回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom190Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom190")]
    [Authorize]
    public class XCom190Controller : ControllerBase
    {
        private readonly IXCom190Service _service;
        
        public XCom190Controller(IXCom190Service service)
        {
            _service = service;
        }
        
        [HttpGet("layouts")]
        public async Task<ActionResult<ApiResponse<PagedResult<LayoutMDto>>>> GetLayouts([FromQuery] LayoutMQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("layouts/{progId}/{pageId}/{seqNo}")]
        public async Task<ActionResult<ApiResponse<LayoutMDto>>> GetLayout(string progId, string pageId, int seqNo)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("layouts/{progId}/{pageId}/{seqNo}/details")]
        public async Task<ActionResult<ApiResponse<List<LayoutDDto>>>> GetLayoutDetails(string progId, string pageId, int seqNo)
        {
            // 實作查詢明細邏輯
        }
        
        [HttpPost("layouts")]
        public async Task<ActionResult<ApiResponse>> CreateLayout([FromBody] CreateLayoutMDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("layouts/{progId}/{pageId}/{seqNo}")]
        public async Task<ActionResult<ApiResponse>> UpdateLayout(string progId, string pageId, int seqNo, [FromBody] UpdateLayoutMDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("layouts/{progId}/{pageId}/{seqNo}")]
        public async Task<ActionResult<ApiResponse>> DeleteLayout(string progId, string pageId, int seqNo)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `XCom190Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom190Service
    {
        Task<PagedResult<LayoutMDto>> GetLayoutsAsync(LayoutMQueryDto query);
        Task<LayoutMDto> GetLayoutAsync(string progId, string pageId, int seqNo);
        Task<List<LayoutDDto>> GetLayoutDetailsAsync(string progId, string pageId, int seqNo);
        Task CreateLayoutAsync(CreateLayoutMDto dto);
        Task UpdateLayoutAsync(string progId, string pageId, int seqNo, UpdateLayoutMDto dto);
        Task DeleteLayoutAsync(string progId, string pageId, int seqNo);
        Task CopyToTempAsync(string progId, string pageId, int seqNo);
        Task RestoreFromTempAsync(string progId, string pageId, int seqNo);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 布局列表頁面 (`LayoutList.vue`)
- **路徑**: `/xcom190/layouts`
- **功能**: 顯示布局列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (LayoutSearchForm)
  - 資料表格 (LayoutDataTable)
  - 新增/修改對話框 (LayoutDialog)

#### 4.1.2 布局詳細頁面 (`LayoutDetail.vue`)
- **路徑**: `/xcom190/layouts/:progId/:pageId/:seqNo`
- **功能**: 顯示布局詳細資料，支援修改、預覽

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LayoutSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="作業代碼">
      <el-input v-model="searchForm.progId" placeholder="請輸入作業代碼" />
    </el-form-item>
    <el-form-item label="頁面代碼">
      <el-input v-model="searchForm.pageId" placeholder="請輸入頁面代碼" />
    </el-form-item>
    <el-form-item label="作業名稱">
      <el-input v-model="searchForm.progName" placeholder="請輸入作業名稱" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="啟用" value="1" />
        <el-option label="停用" value="0" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`LayoutDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="layoutList" v-loading="loading">
      <el-table-column prop="progId" label="作業代碼" width="120" />
      <el-table-column prop="progName" label="作業名稱" width="150" />
      <el-table-column prop="pageId" label="頁面代碼" width="100" />
      <el-table-column prop="seqNo" label="序號" width="80" />
      <el-table-column prop="title" label="標題" width="200" />
      <el-table-column prop="style" label="樣式" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handlePreview(row)">預覽</el-button>
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

### 4.3 API 呼叫 (`xcom190.api.ts`)
```typescript
import request from '@/utils/request';

export interface LayoutMDto {
  layoutMId?: number;
  progId: string;
  pageId: string;
  seqNo: number;
  tableName?: string;
  nextUrl?: string;
  keyFields?: string;
  title?: string;
  style?: string;
  inputCnt?: number;
  tabName?: string;
  columnCnt?: number;
  status?: string;
  details?: LayoutDDto[];
}

export interface LayoutDDto {
  layoutDId?: number;
  fieldId: string;
  fieldName?: string;
  showLayout?: string;
  mustKeyinYn?: string;
  readonlyYn?: string;
  maxlength?: number;
  inputSize?: number;
  dataType?: string;
  defaultValue?: string;
}

export interface LayoutMQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    progId?: string;
    pageId?: string;
    progName?: string;
    status?: string;
  };
}

// API 函數
export const getLayoutList = (query: LayoutMQueryDto) => {
  return request.get<ApiResponse<PagedResult<LayoutMDto>>>('/api/v1/xcom190/layouts', { params: query });
};

export const getLayout = (progId: string, pageId: string, seqNo: number) => {
  return request.get<ApiResponse<LayoutMDto>>(`/api/v1/xcom190/layouts/${progId}/${pageId}/${seqNo}`);
};

export const getLayoutDetails = (progId: string, pageId: string, seqNo: number) => {
  return request.get<ApiResponse<LayoutDDto[]>>(`/api/v1/xcom190/layouts/${progId}/${pageId}/${seqNo}/details`);
};

export const createLayout = (data: CreateLayoutMDto) => {
  return request.post<ApiResponse>('/api/v1/xcom190/layouts', data);
};

export const updateLayout = (progId: string, pageId: string, seqNo: number, data: UpdateLayoutMDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom190/layouts/${progId}/${pageId}/${seqNo}`, data);
};

export const deleteLayout = (progId: string, pageId: string, seqNo: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom190/layouts/${progId}/${pageId}/${seqNo}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 臨時表處理邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 布局編輯器開發
- [ ] 布局預覽功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 布局預覽測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 業務邏輯
- ProgId、PageId、SeqNo 組合必須唯一
- 刪除布局主檔時，會級聯刪除布局明細
- 支援臨時表功能，用於布局預覽和測試
- 布局變更需要記錄操作日誌

### 6.2 資料驗證
- 必填欄位必須驗證
- 序號必須為正整數
- 狀態值必須在允許範圍內
- 欄位配置必須符合布局規則

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 布局預覽需要快取機制

### 6.4 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 布局變更必須記錄操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增布局成功
- [ ] 新增布局失敗 (重複ProgId+PageId+SeqNo)
- [ ] 修改布局成功
- [ ] 刪除布局成功
- [ ] 查詢布局列表成功
- [ ] 查詢單筆布局成功
- [ ] 查詢布局明細成功
- [ ] 複製到臨時表成功
- [ ] 從臨時表還原成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 布局預覽功能測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 布局預覽效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/XCOM000/XCOM190.ascx.cs`
- `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_FB.aspx.cs`
- `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_FI.aspx.cs`
- `IMS3/HANSHIN/IMS3/XCOM000/XCOM190_AJAX.aspx.cs`
- `IMS3/HANSHIN/RSL_CLASS/IMS3_SYS/MNG_LAYOUT_M.cs`

### 8.2 資料庫 Schema
- 舊系統 `MNG_LAYOUT_M` 資料表結構
- 舊系統 `MNG_LAYOUT_D` 資料表結構
- 舊系統 `MNG_LAYOUT_M_TEMP` 資料表結構

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01


