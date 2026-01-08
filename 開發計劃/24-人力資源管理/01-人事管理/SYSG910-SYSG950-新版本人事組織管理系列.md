# SYSG910-SYSG950 - 新版本人事組織管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSG910-SYSG950系列
- **功能名稱**: 新版本人事組織管理系列
- **功能描述**: 提供人事組織架構的新增、修改、刪除、查詢功能，包含證號別維護、組織層級管理、組織群組管理、組織職位管理、組織人員管理等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FB.ASP` (證號別瀏覽)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FI.ASP` (證號別新增)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FU.ASP` (證號別修改)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FD.ASP` (證號別刪除)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FQ.ASP` (證號別查詢)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FS.ASP` (證號別排序)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG910_PR.ASP` (證號別報表)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG920_*.ASP` (組織層級管理)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG930_*.ASP` (組織群組管理)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG940_*.ASP` (組織職位管理)
  - `WEB/IMS_CORE/ASP/SYSH000/SYSG950_*.ASP` (組織人員管理)

### 1.2 業務需求
- 管理證號別資訊（SYSG910）
- 管理組織層級資訊（SYSG920）
- 管理組織群組資訊（SYSG930）
- 管理組織職位資訊（SYSG940）
- 管理組織人員對應資訊（SYSG950）
- 支援組織架構的階層式管理
- 記錄組織異動資訊
- 支援組織人員的批量管理

### 1.3 功能範圍
本系列包含以下子功能：
- **SYSG910**: 證號別維護
- **SYSG920**: 組織層級管理
- **SYSG930**: 組織群組管理
- **SYSG940**: 組織職位管理
- **SYSG950**: 組織人員管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `IdTypes` (證號別)

```sql
CREATE TABLE [dbo].[IdTypes] (
    [IdTypeId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [IdTypeName] NVARCHAR(100) NOT NULL,
    [IdTypeCode] NVARCHAR(20) NULL,
    [SeqNo] INT NULL DEFAULT 0,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_IdTypes] PRIMARY KEY CLUSTERED ([IdTypeId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_IdTypes_Status] ON [dbo].[IdTypes] ([Status]);
CREATE NONCLUSTERED INDEX [IX_IdTypes_SeqNo] ON [dbo].[IdTypes] ([SeqNo]);
```

### 2.2 主要資料表: `OrganizationLevels` (組織層級)

```sql
CREATE TABLE [dbo].[OrganizationLevels] (
    [LevelId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [LevelName] NVARCHAR(100) NOT NULL,
    [LevelCode] NVARCHAR(20) NULL,
    [ParentLevelId] NVARCHAR(50) NULL,
    [LevelOrder] INT NULL DEFAULT 0,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_OrganizationLevels_Parent] FOREIGN KEY ([ParentLevelId]) REFERENCES [dbo].[OrganizationLevels] ([LevelId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_OrganizationLevels_ParentLevelId] ON [dbo].[OrganizationLevels] ([ParentLevelId]);
CREATE NONCLUSTERED INDEX [IX_OrganizationLevels_Status] ON [dbo].[OrganizationLevels] ([Status]);
```

### 2.3 主要資料表: `OrganizationGroups` (組織群組)

```sql
CREATE TABLE [dbo].[OrganizationGroups] (
    [GroupId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [GroupName] NVARCHAR(100) NOT NULL,
    [GroupCode] NVARCHAR(20) NULL,
    [LevelId] NVARCHAR(50) NULL,
    [ParentGroupId] NVARCHAR(50) NULL,
    [GroupOrder] INT NULL DEFAULT 0,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_OrganizationGroups_Level] FOREIGN KEY ([LevelId]) REFERENCES [dbo].[OrganizationLevels] ([LevelId]),
    CONSTRAINT [FK_OrganizationGroups_Parent] FOREIGN KEY ([ParentGroupId]) REFERENCES [dbo].[OrganizationGroups] ([GroupId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_OrganizationGroups_LevelId] ON [dbo].[OrganizationGroups] ([LevelId]);
CREATE NONCLUSTERED INDEX [IX_OrganizationGroups_ParentGroupId] ON [dbo].[OrganizationGroups] ([ParentGroupId]);
```

### 2.4 主要資料表: `OrganizationPositions` (組織職位)

```sql
CREATE TABLE [dbo].[OrganizationPositions] (
    [PositionId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PositionName] NVARCHAR(100) NOT NULL,
    [PositionCode] NVARCHAR(20) NULL,
    [GroupId] NVARCHAR(50) NULL,
    [PositionOrder] INT NULL DEFAULT 0,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_OrganizationPositions_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[OrganizationGroups] ([GroupId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_OrganizationPositions_GroupId] ON [dbo].[OrganizationPositions] ([GroupId]);
```

### 2.5 主要資料表: `OrganizationEmployees` (組織人員對應)

```sql
CREATE TABLE [dbo].[OrganizationEmployees] (
    [OrgEmployeeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EmployeeId] NVARCHAR(50) NOT NULL,
    [LevelId] NVARCHAR(50) NULL,
    [GroupId] NVARCHAR(50) NULL,
    [PositionId] NVARCHAR(50) NULL,
    [StartDate] DATETIME2 NULL,
    [EndDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_OrganizationEmployees_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]),
    CONSTRAINT [FK_OrganizationEmployees_Level] FOREIGN KEY ([LevelId]) REFERENCES [dbo].[OrganizationLevels] ([LevelId]),
    CONSTRAINT [FK_OrganizationEmployees_Group] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[OrganizationGroups] ([GroupId]),
    CONSTRAINT [FK_OrganizationEmployees_Position] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[OrganizationPositions] ([PositionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_OrganizationEmployees_EmployeeId] ON [dbo].[OrganizationEmployees] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_OrganizationEmployees_LevelId] ON [dbo].[OrganizationEmployees] ([LevelId]);
CREATE NONCLUSTERED INDEX [IX_OrganizationEmployees_GroupId] ON [dbo].[OrganizationEmployees] ([GroupId]);
CREATE NONCLUSTERED INDEX [IX_OrganizationEmployees_PositionId] ON [dbo].[OrganizationEmployees] ([PositionId]);
```

### 2.6 資料字典

#### IdTypes 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| IdTypeId | NVARCHAR | 50 | NO | - | 證號別編號 | 主鍵 |
| IdTypeName | NVARCHAR | 100 | NO | - | 證號別名稱 | - |
| IdTypeCode | NVARCHAR | 20 | YES | - | 證號別代碼 | - |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 SYSG910 API 端點列表

#### 3.1.1 查詢證號別列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/id-types`
- **說明**: 查詢證號別列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SeqNo",
    "sortOrder": "ASC",
    "filters": {
      "idTypeId": "",
      "idTypeName": "",
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
          "idTypeId": "ID001",
          "idTypeName": "身分證",
          "idTypeCode": "ID",
          "seqNo": 1,
          "status": "A"
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

#### 3.1.2 查詢單筆證號別
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/hr/id-types/{idTypeId}`
- **說明**: 根據證號別編號查詢單筆資料

#### 3.1.3 新增證號別
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/hr/id-types`
- **說明**: 新增證號別資料
- **請求格式**:
  ```json
  {
    "idTypeId": "ID001",
    "idTypeName": "身分證",
    "idTypeCode": "ID",
    "seqNo": 1,
    "status": "A",
    "notes": "備註"
  }
  ```

#### 3.1.4 修改證號別
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/hr/id-types/{idTypeId}`
- **說明**: 修改證號別資料

#### 3.1.5 刪除證號別
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/hr/id-types/{idTypeId}`
- **說明**: 刪除證號別資料

### 3.2 SYSG920-SYSG950 API 端點列表

#### 3.2.1 組織層級管理 API
- **路徑**: `/api/v1/hr/organization-levels`
- **方法**: GET, POST, PUT, DELETE
- **說明**: 提供組織層級的完整 CRUD 功能

#### 3.2.2 組織群組管理 API
- **路徑**: `/api/v1/hr/organization-groups`
- **方法**: GET, POST, PUT, DELETE
- **說明**: 提供組織群組的完整 CRUD 功能

#### 3.2.3 組織職位管理 API
- **路徑**: `/api/v1/hr/organization-positions`
- **方法**: GET, POST, PUT, DELETE
- **說明**: 提供組織職位的完整 CRUD 功能

#### 3.2.4 組織人員管理 API
- **路徑**: `/api/v1/hr/organization-employees`
- **方法**: GET, POST, PUT, DELETE
- **說明**: 提供組織人員對應的完整 CRUD 功能

### 3.3 後端實作類別

#### 3.3.1 Controller: `IdTypesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/hr/id-types")]
    [Authorize]
    public class IdTypesController : ControllerBase
    {
        private readonly IIdTypeService _idTypeService;
        
        public IdTypesController(IIdTypeService idTypeService)
        {
            _idTypeService = idTypeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<IdTypeDto>>>> GetIdTypes([FromQuery] IdTypeQueryDto query)
        {
            var result = await _idTypeService.GetIdTypesAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("{idTypeId}")]
        public async Task<ActionResult<ApiResponse<IdTypeDto>>> GetIdType(string idTypeId)
        {
            var result = await _idTypeService.GetIdTypeByIdAsync(idTypeId);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateIdType([FromBody] CreateIdTypeDto dto)
        {
            var idTypeId = await _idTypeService.CreateIdTypeAsync(dto);
            return Ok(ApiResponse.Success(idTypeId, "新增成功"));
        }
        
        [HttpPut("{idTypeId}")]
        public async Task<ActionResult<ApiResponse>> UpdateIdType(string idTypeId, [FromBody] UpdateIdTypeDto dto)
        {
            await _idTypeService.UpdateIdTypeAsync(idTypeId, dto);
            return Ok(ApiResponse.Success(null, "修改成功"));
        }
        
        [HttpDelete("{idTypeId}")]
        public async Task<ActionResult<ApiResponse>> DeleteIdType(string idTypeId)
        {
            await _idTypeService.DeleteIdTypeAsync(idTypeId);
            return Ok(ApiResponse.Success(null, "刪除成功"));
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 證號別維護頁面 (`IdTypeList.vue`)
- **路徑**: `/hr/organization/id-types`
- **功能**: 顯示證號別列表，支援查詢、新增、修改、刪除

#### 4.1.2 組織層級管理頁面 (`OrganizationLevelList.vue`)
- **路徑**: `/hr/organization/levels`
- **功能**: 顯示組織層級列表，支援階層式顯示

#### 4.1.3 組織群組管理頁面 (`OrganizationGroupList.vue`)
- **路徑**: `/hr/organization/groups`
- **功能**: 顯示組織群組列表，支援階層式顯示

#### 4.1.4 組織職位管理頁面 (`OrganizationPositionList.vue`)
- **路徑**: `/hr/organization/positions`
- **功能**: 顯示組織職位列表

#### 4.1.5 組織人員管理頁面 (`OrganizationEmployeeList.vue`)
- **路徑**: `/hr/organization/employees`
- **功能**: 顯示組織人員對應列表

### 4.2 UI 元件設計

#### 4.2.1 證號別列表元件 (`IdTypeDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="idTypeList" v-loading="loading">
      <el-table-column prop="idTypeId" label="證號別編號" width="120" />
      <el-table-column prop="idTypeName" label="證號別名稱" width="150" />
      <el-table-column prop="idTypeCode" label="證號別代碼" width="120" />
      <el-table-column prop="seqNo" label="排序序號" width="100" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === 'A' ? 'success' : 'danger'">
            {{ row.status === 'A' ? '啟用' : '停用' }}
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
  </div>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (6天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (6天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 階層式顯示元件開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 17天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 組織架構變更必須記錄

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 階層式查詢必須優化

### 6.3 資料驗證
- 證號別編號必須唯一
- 組織層級必須避免循環引用
- 必填欄位必須驗證

### 6.4 業務邏輯
- 刪除組織前必須檢查是否有相關人員
- 組織異動資訊必須記錄
- 組織階層必須保持一致性

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增證號別成功
- [ ] 修改證號別成功
- [ ] 刪除證號別成功
- [ ] 查詢證號別列表成功
- [ ] 組織層級管理測試
- [ ] 組織群組管理測試
- [ ] 組織職位管理測試
- [ ] 組織人員管理測試

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 階層式查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG910_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG920_*.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG930_*.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG940_*.ASP`
- `WEB/IMS_CORE/ASP/SYSH000/SYSG950_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

