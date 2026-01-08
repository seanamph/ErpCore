# SYSX110 - 系統擴展資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSX110
- **功能名稱**: 系統擴展資料維護
- **功能描述**: 提供系統擴展資料的新增、修改、刪除、查詢功能，用於系統擴展功能的資料管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSX000/SYSX110_FI.aspx` (新增)
  - `WEB/IMS_CORE/SYSX000/SYSX110_FU.aspx` (修改)
  - `WEB/IMS_CORE/SYSX000/SYSX110_FD.aspx` (刪除)
  - `WEB/IMS_CORE/SYSX000/SYSX120_FQ.aspx` (查詢)
  - `WEB/IMS_CORE/SYSX000/SYSX120_FB.aspx` (瀏覽)

### 1.2 業務需求
- 管理系統擴展功能資料
- 支援擴展資料的新增、修改、刪除、查詢
- 記錄擴展資料的建立與變更資訊
- 支援擴展資料的啟用/停用
- 支援擴展資料的排序

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemExtensions` (對應舊系統 `SYSX_EXTENSION`)

```sql
CREATE TABLE [dbo].[SystemExtensions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展功能代碼 (EXTENSION_ID)
    [ExtensionName] NVARCHAR(100) NOT NULL, -- 擴展功能名稱 (EXTENSION_NAME)
    [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型 (EXTENSION_TYPE)
    [ExtensionValue] NVARCHAR(500) NULL, -- 擴展值 (EXTENSION_VALUE)
    [ExtensionConfig] NVARCHAR(MAX) NULL, -- 擴展設定 (JSON格式) (EXTENSION_CONFIG)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_SystemExtensions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_SystemExtensions_ExtensionId] UNIQUE ([ExtensionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionId] ON [dbo].[SystemExtensions] ([ExtensionId]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_ExtensionType] ON [dbo].[SystemExtensions] ([ExtensionType]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_Status] ON [dbo].[SystemExtensions] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SystemExtensions_SeqNo] ON [dbo].[SystemExtensions] ([SeqNo]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ExtensionId | NVARCHAR | 50 | NO | - | 擴展功能代碼 | 唯一鍵 |
| ExtensionName | NVARCHAR | 100 | NO | - | 擴展功能名稱 | - |
| ExtensionType | NVARCHAR | 20 | YES | - | 擴展類型 | - |
| ExtensionValue | NVARCHAR | 500 | YES | - | 擴展值 | - |
| ExtensionConfig | NVARCHAR(MAX) | - | YES | - | 擴展設定 | JSON格式 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions`
- **說明**: 查詢系統擴展列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SeqNo",
    "sortOrder": "ASC",
    "filters": {
      "extensionId": "",
      "extensionName": "",
      "extensionType": "",
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
          "extensionId": "EXT001",
          "extensionName": "擴展功能1",
          "extensionType": "TYPE1",
          "extensionValue": "VALUE1",
          "extensionConfig": "{}",
          "seqNo": 1,
          "status": "1",
          "notes": "擴展功能說明"
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

#### 3.1.2 查詢單筆系統擴展
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/{tKey}`
- **說明**: 根據主鍵查詢單筆系統擴展資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 根據擴展代碼查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system-extensions/by-id/{extensionId}`
- **說明**: 根據擴展功能代碼查詢系統擴展資料
- **路徑參數**:
  - `extensionId`: 擴展功能代碼
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.4 新增系統擴展
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/system-extensions`
- **說明**: 新增系統擴展資料
- **請求格式**:
  ```json
  {
    "extensionId": "EXT001",
    "extensionName": "擴展功能1",
    "extensionType": "TYPE1",
    "extensionValue": "VALUE1",
    "extensionConfig": "{}",
    "seqNo": 1,
    "status": "1",
    "notes": "擴展功能說明"
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

#### 3.1.5 修改系統擴展
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/system-extensions/{tKey}`
- **說明**: 修改系統擴展資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `extensionId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除系統擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/system-extensions/{tKey}`
- **說明**: 刪除系統擴展資料
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

### 3.2 後端實作類別

#### 3.2.1 Controller: `SystemExtensionController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/system-extensions")]
    [Authorize]
    public class SystemExtensionController : ControllerBase
    {
        private readonly ISystemExtensionService _service;
        
        public SystemExtensionController(ISystemExtensionService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SystemExtensionDto>>>> GetSystemExtensions([FromQuery] SystemExtensionQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<SystemExtensionDto>>> GetSystemExtension(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("by-id/{extensionId}")]
        public async Task<ActionResult<ApiResponse<SystemExtensionDto>>> GetSystemExtensionById(string extensionId)
        {
            // 實作根據擴展代碼查詢邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SystemExtensionKeyDto>>> CreateSystemExtension([FromBody] CreateSystemExtensionDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateSystemExtension(long tKey, [FromBody] UpdateSystemExtensionDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteSystemExtension(long tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統擴展列表頁面 (`SystemExtensionList.vue`)
- **路徑**: `/system-extensions`
- **功能**: 顯示系統擴展列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (SystemExtensionSearchForm)
  - 資料表格 (SystemExtensionDataTable)
  - 新增/修改對話框 (SystemExtensionDialog)
  - 刪除確認對話框

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SystemExtensionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="擴展功能代碼">
      <el-input v-model="searchForm.extensionId" placeholder="請輸入擴展功能代碼" />
    </el-form-item>
    <el-form-item label="擴展功能名稱">
      <el-input v-model="searchForm.extensionName" placeholder="請輸入擴展功能名稱" />
    </el-form-item>
    <el-form-item label="擴展類型">
      <el-select v-model="searchForm.extensionType" placeholder="請選擇擴展類型" clearable>
        <el-option v-for="type in extensionTypeList" :key="type.value" :label="type.label" :value="type.value" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態" clearable>
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

#### 4.2.2 資料表格元件 (`SystemExtensionDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="extensionList" v-loading="loading">
      <el-table-column prop="extensionId" label="擴展功能代碼" width="150" />
      <el-table-column prop="extensionName" label="擴展功能名稱" width="200" />
      <el-table-column prop="extensionType" label="擴展類型" width="120" />
      <el-table-column prop="extensionValue" label="擴展值" width="200" />
      <el-table-column prop="seqNo" label="排序序號" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="備註" width="200" show-overflow-tooltip />
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

#### 4.2.3 新增/修改對話框 (`SystemExtensionDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="擴展功能代碼" prop="extensionId">
        <el-input v-model="form.extensionId" :disabled="isEdit" placeholder="請輸入擴展功能代碼" />
      </el-form-item>
      <el-form-item label="擴展功能名稱" prop="extensionName">
        <el-input v-model="form.extensionName" placeholder="請輸入擴展功能名稱" />
      </el-form-item>
      <el-form-item label="擴展類型" prop="extensionType">
        <el-select v-model="form.extensionType" placeholder="請選擇擴展類型" clearable>
          <el-option v-for="type in extensionTypeList" :key="type.value" :label="type.label" :value="type.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="擴展值" prop="extensionValue">
        <el-input v-model="form.extensionValue" placeholder="請輸入擴展值" />
      </el-form-item>
      <el-form-item label="擴展設定" prop="extensionConfig">
        <el-input v-model="form.extensionConfig" type="textarea" :rows="5" placeholder="請輸入JSON格式的擴展設定" />
      </el-form-item>
      <el-form-item label="排序序號" prop="seqNo">
        <el-input-number v-model="form.seqNo" :min="0" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-radio-group v-model="form.status">
          <el-radio label="1">啟用</el-radio>
          <el-radio label="0">停用</el-radio>
        </el-radio-group>
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
- 擴展功能代碼必須唯一
- 擴展設定必須是有效的JSON格式
- 排序序號用於控制顯示順序

### 5.2 權限控制
- 查詢權限：所有使用者
- 新增權限：需有新增權限
- 修改權限：需有修改權限
- 刪除權限：需有刪除權限

### 5.3 效能優化
- 查詢時使用適當的索引
- 擴展設定使用JSON格式儲存，便於擴展

### 5.4 錯誤處理
- 驗證必填欄位
- 驗證擴展功能代碼唯一性
- 驗證JSON格式
- 處理資料庫錯誤

---

## 六、測試計劃

### 6.1 單元測試
- 測試Service層的業務邏輯
- 測試Repository層的資料存取
- 測試API端點

### 6.2 整合測試
- 測試完整的CRUD流程
- 測試JSON格式驗證

### 6.3 前端測試
- 測試UI元件功能
- 測試表單驗證
- 測試資料表格操作

---

## 七、部署注意事項

### 7.1 資料庫遷移
- 建立資料表結構
- 建立索引
- 建立唯一約束

### 7.2 設定檔
- 設定API端點

### 7.3 權限設定
- 設定功能權限
- 設定按鈕權限

