# EW_SYS_LIST - 系統列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: EW_SYS_LIST
- **功能名稱**: 系統列表
- **功能描述**: 提供主系統項目列表選擇功能，用於精靈或表單中的系統選擇，支援系統查詢、篩選、選擇等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/EW_SYS_LIST.asp` (系統列表主程式)

### 1.2 業務需求
- 提供主系統項目列表查詢功能
- 支援系統ID、系統名稱篩選
- 支援系統選擇並回傳系統ID和名稱
- 支援彈窗模式選擇
- 與系統主檔（MNG_SYS）整合
- 用於EW_MENU_LIST等精靈功能的系統選擇

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Systems` (系統主檔，對應舊系統 `MNG_SYS`)

```sql
CREATE TABLE [dbo].[Systems] (
    [SystemId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 系統ID (SYS_ID)
    [SystemName] NVARCHAR(100) NOT NULL, -- 系統名稱 (SYS_NAME)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Systems] PRIMARY KEY CLUSTERED ([SystemId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Systems_SystemName] ON [dbo].[Systems] ([SystemName]);
CREATE NONCLUSTERED INDEX [IX_Systems_Status] ON [dbo].[Systems] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| SystemId | NVARCHAR | 50 | NO | - | 系統ID | 主鍵 |
| SystemName | NVARCHAR | 100 | NO | - | 系統名稱 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/sys-list`
- **說明**: 查詢系統列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "systemId": "",
    "systemName": "",
    "filter": ""
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
          "systemId": "SYS001",
          "systemName": "系統名稱"
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

#### 3.1.2 查詢單筆系統
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/ew/sys-list/{systemId}`
- **說明**: 根據系統ID查詢單筆系統資料
- **回應格式**: 標準單筆資料回應

### 3.2 後端實作類別

#### 3.2.1 Controller: `EwSysListController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ew/sys-list")]
    [Authorize]
    public class EwSysListController : ControllerBase
    {
        private readonly IEwSysListService _sysListService;
        
        public EwSysListController(IEwSysListService sysListService)
        {
            _sysListService = sysListService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SystemDto>>>> GetSysList([FromQuery] SysListQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{systemId}")]
        public async Task<ActionResult<ApiResponse<SystemDto>>> GetSystem(string systemId)
        {
            // 實作查詢單筆邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統列表選擇頁面 (`EwSysList.vue`)
- **路徑**: `/ew/sys-list`
- **功能**: 顯示系統列表，支援查詢、篩選、選擇
- **主要元件**:
  - 查詢表單 (SysListSearchForm)
  - 資料表格 (SysListDataTable)
  - 選擇按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SysListSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="系統代碼">
      <el-input v-model="searchForm.systemId" placeholder="請輸入系統代碼" />
    </el-form-item>
    <el-form-item label="系統名稱">
      <el-input v-model="searchForm.systemName" placeholder="請輸入系統名稱" />
    </el-form-item>
    <el-form-item label="關鍵字">
      <el-input v-model="searchForm.filter" placeholder="請輸入關鍵字" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button @click="handleClose">關閉</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`SysListDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="sysList" v-loading="loading" @row-click="handleRowClick">
      <el-table-column type="index" label="序號" width="60" />
      <el-table-column prop="systemId" label="主系統代碼" width="150" />
      <el-table-column prop="systemName" label="主系統名稱" />
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
const handleRowClick = (row: SystemDto) => {
  // 回傳系統ID給父視窗（用於EW_MENU_LIST等精靈）
  if (window.opener) {
    window.opener.document.forms[0].FrmSYSID.value = row.systemId;
    // 如果需要跳轉到選單列表
    if (window.opener.location.href.includes('EW_MENU_LIST')) {
      window.opener.location.href = `EW_MENU_LIST.asp?PG_ID=${pgId}&FrmSYSID=${row.systemId}`;
    }
    window.close();
  }
};
</script>
```

### 4.3 API 呼叫 (`ewSysList.api.ts`)
```typescript
import request from '@/utils/request';

export interface SystemDto {
  systemId: string;
  systemName: string;
}

export interface SysListQueryDto {
  pageIndex: number;
  pageSize: number;
  systemId?: string;
  systemName?: string;
  filter?: string;
}

// API 函數
export const getSysList = (query: SysListQueryDto) => {
  return request.get<ApiResponse<PagedResult<SystemDto>>>('/api/v1/ew/sys-list', { params: query });
};

export const getSystem = (systemId: string) => {
  return request.get<ApiResponse<SystemDto>>(`/api/v1/ew/sys-list/${systemId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（使用現有的Systems表）
- [ ] 確認索引

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 系統列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 彈窗選擇功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 與EW_MENU_LIST整合測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 5天

---

## 六、注意事項

### 6.1 業務邏輯
- 支援系統ID、系統名稱模糊查詢
- 支援關鍵字搜尋（同時搜尋系統ID和系統名稱）
- 選擇後需回傳系統ID給父視窗
- 通常用於EW_MENU_LIST等精靈功能的系統選擇前置步驟

### 6.2 資料驗證
- 系統ID必須存在（如果提供）

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢系統列表成功
- [ ] 查詢系統列表（帶篩選條件）成功
- [ ] 查詢單筆系統成功
- [ ] 系統選擇功能測試

### 7.2 整合測試
- [ ] 完整選擇流程測試
- [ ] 與EW_MENU_LIST整合測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/EW_SYS_LIST.asp` - 系統列表主程式

### 8.2 資料庫 Schema
- 舊系統資料表：`MNG_SYS` (系統主檔)
- 主要欄位：SYS_ID, SYS_NAME

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

