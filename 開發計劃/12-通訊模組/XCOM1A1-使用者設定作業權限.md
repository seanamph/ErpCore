# XCOM1A1 - 使用者設定作業權限 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM1A1
- **功能名稱**: 使用者設定作業權限
- **功能描述**: 提供客戶作業權限的新增、修改、刪除、查詢功能，可設定客戶對特定作業的權限（全選、未選），包含作業列表查詢、選單列表查詢、按鈕列表查詢等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_PROG_AC.asp` (設定作業權限)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_MENU_AC.asp` (設定選單權限)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_BUTTON_AC.asp` (設定按鈕權限)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_PROG_LIST.asp` (作業列表)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_MENU_LIST.asp` (選單列表)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_BUTTON_LIST.asp` (按鈕列表)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SYS_LIST.asp` (系統列表)

### 1.2 業務需求
- 管理客戶作業權限設定
- 支援依客戶代碼查詢權限
- 支援作業權限設定（全選、未選）
- 支援選單權限設定
- 支援按鈕權限設定
- 支援作業列表查詢
- 支援選單列表查詢
- 支援按鈕列表查詢
- 支援系統列表查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CustomerProgramPermissions` (客戶作業權限，對應舊系統 `MNG_CUST`)

```sql
CREATE TABLE [dbo].[CustomerProgramPermissions] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CUST_ID] NVARCHAR(50) NOT NULL, -- 客戶代碼
    [PROG_ID] NVARCHAR(50) NOT NULL, -- 作業代碼
    [PERMISSION_TYPE] NVARCHAR(20) NULL, -- 權限類型 (PROG:作業, MENU:選單, BUTTON:按鈕)
    [STATUS] NVARCHAR(10) NOT NULL DEFAULT 'Y', -- 狀態 (Y:有權限, N:無權限)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_CustomerProgramPermissions] PRIMARY KEY CLUSTERED ([T_KEY] ASC),
    CONSTRAINT [FK_CustomerProgramPermissions_Customer] FOREIGN KEY ([CUST_ID]) REFERENCES [dbo].[Customers] ([CUST_ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomerProgramPermissions_Program] FOREIGN KEY ([PROG_ID]) REFERENCES [dbo].[Programs] ([PROG_ID]) ON DELETE CASCADE,
    CONSTRAINT [UQ_CustomerProgramPermissions_CustProg] UNIQUE ([CUST_ID], [PROG_ID], [PERMISSION_TYPE])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CustomerProgramPermissions_CUST_ID] ON [dbo].[CustomerProgramPermissions] ([CUST_ID]);
CREATE NONCLUSTERED INDEX [IX_CustomerProgramPermissions_PROG_ID] ON [dbo].[CustomerProgramPermissions] ([PROG_ID]);
CREATE NONCLUSTERED INDEX [IX_CustomerProgramPermissions_PERMISSION_TYPE] ON [dbo].[CustomerProgramPermissions] ([PERMISSION_TYPE]);
```

### 2.2 相關資料表

#### 2.2.1 `Customers` - 客戶主檔
- 參考: `開發計劃/09-客戶管理/CUS5110-客戶基本資料維護.md` 的 `Customers` 資料表結構

#### 2.2.2 `Programs` - 作業主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md` 的 `Programs` 資料表結構

#### 2.2.3 `Menus` - 選單主檔
```sql
CREATE TABLE [dbo].[Menus] (
    [MENU_ID] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [MENU_NAME] NVARCHAR(100) NOT NULL,
    [PARENT_MENU_ID] NVARCHAR(50) NULL,
    [MENU_ORDER] INT NULL,
    [STATUS] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.4 `Buttons` - 按鈕主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0440-系統功能按鈕資料維護.md` 的 `Buttons` 資料表結構

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| CUST_ID | NVARCHAR | 50 | NO | - | 客戶代碼 | 外鍵至Customers |
| PROG_ID | NVARCHAR | 50 | NO | - | 作業代碼 | 外鍵至Programs/Menus/Buttons |
| PERMISSION_TYPE | NVARCHAR | 20 | YES | - | 權限類型 | PROG:作業, MENU:選單, BUTTON:按鈕 |
| STATUS | NVARCHAR | 10 | NO | 'Y' | 狀態 | Y:有權限, N:無權限 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢客戶作業權限
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a1/customer-permissions`
- **說明**: 查詢客戶作業權限列表，支援依客戶代碼篩選
- **請求參數**:
  ```json
  {
    "custId": "C001",
    "permissionType": "PROG", // PROG:作業, MENU:選單, BUTTON:按鈕
    "pageIndex": 1,
    "pageSize": 20
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
          "custId": "C001",
          "custName": "客戶名稱",
          "progId": "SYS0110",
          "progName": "使用者基本資料維護",
          "permissionType": "PROG",
          "status": "Y"
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

#### 3.1.2 查詢作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a1/programs`
- **說明**: 查詢作業列表，供權限設定使用
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "progId": "SYS0110",
        "progName": "使用者基本資料維護",
        "sysId": "SYS0000",
        "sysName": "系統管理"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢選單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a1/menus`
- **說明**: 查詢選單列表，供權限設定使用
- **回應格式**: 同作業列表

#### 3.1.4 查詢按鈕列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a1/buttons`
- **說明**: 查詢按鈕列表，供權限設定使用
- **請求參數**:
  ```json
  {
    "progId": "SYS0110"
  }
  ```
- **回應格式**: 同作業列表

#### 3.1.5 查詢系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a1/systems`
- **說明**: 查詢系統列表，供權限設定使用
- **回應格式**: 同作業列表

#### 3.1.6 設定作業權限
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1a1/set-program-permissions`
- **說明**: 批次設定客戶作業權限
- **請求格式**:
  ```json
  {
    "custId": "C001",
    "permissions": [
      {
        "progId": "SYS0110",
        "status": "Y" // Y:有權限, N:無權限
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "設定成功",
    "data": {
      "affectedCount": 10
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 設定選單權限
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1a1/set-menu-permissions`
- **說明**: 批次設定客戶選單權限
- **請求格式**: 同設定作業權限

#### 3.1.8 設定按鈕權限
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1a1/set-button-permissions`
- **說明**: 批次設定客戶按鈕權限
- **請求格式**: 同設定作業權限

#### 3.1.9 刪除客戶權限
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom1a1/customer-permissions/{tKey}`
- **說明**: 刪除單筆客戶權限
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

#### 3.2.1 Controller: `XCOM1A1Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom1a1")]
    [Authorize]
    public class XCOM1A1Controller : ControllerBase
    {
        private readonly IXCOM1A1Service _service;
        
        public XCOM1A1Controller(IXCOM1A1Service service)
        {
            _service = service;
        }
        
        [HttpGet("customer-permissions")]
        public async Task<ActionResult<ApiResponse<PagedResult<CustomerPermissionDto>>>> GetCustomerPermissions([FromQuery] CustomerPermissionQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("programs")]
        public async Task<ActionResult<ApiResponse<List<ProgramDto>>>> GetPrograms()
        {
            // 實作查詢作業列表邏輯
        }
        
        [HttpGet("menus")]
        public async Task<ActionResult<ApiResponse<List<MenuDto>>>> GetMenus()
        {
            // 實作查詢選單列表邏輯
        }
        
        [HttpGet("buttons")]
        public async Task<ActionResult<ApiResponse<List<ButtonDto>>>> GetButtons([FromQuery] string progId)
        {
            // 實作查詢按鈕列表邏輯
        }
        
        [HttpGet("systems")]
        public async Task<ActionResult<ApiResponse<List<SystemDto>>>> GetSystems()
        {
            // 實作查詢系統列表邏輯
        }
        
        [HttpPost("set-program-permissions")]
        public async Task<ActionResult<ApiResponse>> SetProgramPermissions([FromBody] SetProgramPermissionsDto dto)
        {
            // 實作設定作業權限邏輯
        }
        
        [HttpPost("set-menu-permissions")]
        public async Task<ActionResult<ApiResponse>> SetMenuPermissions([FromBody] SetMenuPermissionsDto dto)
        {
            // 實作設定選單權限邏輯
        }
        
        [HttpPost("set-button-permissions")]
        public async Task<ActionResult<ApiResponse>> SetButtonPermissions([FromBody] SetButtonPermissionsDto dto)
        {
            // 實作設定按鈕權限邏輯
        }
        
        [HttpDelete("customer-permissions/{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteCustomerPermission(long tKey)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `XCOM1A1Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCOM1A1Service
    {
        Task<PagedResult<CustomerPermissionDto>> GetCustomerPermissionsAsync(CustomerPermissionQueryDto query);
        Task<List<ProgramDto>> GetProgramsAsync();
        Task<List<MenuDto>> GetMenusAsync();
        Task<List<ButtonDto>> GetButtonsAsync(string progId);
        Task<List<SystemDto>> GetSystemsAsync();
        Task SetProgramPermissionsAsync(SetProgramPermissionsDto dto);
        Task SetMenuPermissionsAsync(SetMenuPermissionsDto dto);
        Task SetButtonPermissionsAsync(SetButtonPermissionsDto dto);
        Task DeleteCustomerPermissionAsync(long tKey);
    }
}
```

#### 3.2.3 Repository: `XCOM1A1Repository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IXCOM1A1Repository
    {
        Task<PagedResult<CustomerProgramPermission>> GetCustomerPermissionsAsync(CustomerPermissionQuery query);
        Task<List<Program>> GetProgramsAsync();
        Task<List<Menu>> GetMenusAsync();
        Task<List<Button>> GetButtonsAsync(string progId);
        Task<List<System>> GetSystemsAsync();
        Task<int> SetProgramPermissionsAsync(string custId, List<ProgramPermission> permissions);
        Task<int> SetMenuPermissionsAsync(string custId, List<MenuPermission> permissions);
        Task<int> SetButtonPermissionsAsync(string custId, List<ButtonPermission> permissions);
        Task DeleteCustomerPermissionAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 客戶作業權限查詢頁面 (`XCOM1A1Query.vue`)
- **路徑**: `/xcom1a1/query`
- **功能**: 查詢客戶作業權限，輸入客戶代碼進行查詢
- **主要元件**:
  - 查詢表單 (CustomerPermissionSearchForm)
  - 權限設定對話框 (PermissionSettingDialog)

#### 4.1.2 客戶作業權限設定頁面 (`XCOM1A1Setting.vue`)
- **路徑**: `/xcom1a1/setting/:custId`
- **功能**: 設定客戶的作業、選單、按鈕權限

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`CustomerPermissionSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="客戶代碼" prop="custId">
      <el-input v-model="searchForm.custId" placeholder="請輸入客戶代碼" />
      <el-button @click="handleSelectCustomer">選擇</el-button>
      <el-input v-model="searchForm.custName" readonly />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 權限設定對話框 (`PermissionSettingDialog.vue`)
```vue
<template>
  <el-dialog
    title="設定作業權限"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-tabs v-model="activeTab">
      <el-tab-pane label="作業權限" name="program">
        <el-table :data="programList" v-loading="loading">
          <el-table-column type="selection" width="55" />
          <el-table-column prop="progId" label="作業代碼" width="120" />
          <el-table-column prop="progName" label="作業名稱" />
          <el-table-column prop="sysName" label="系統名稱" width="150" />
        </el-table>
        <div style="margin-top: 10px;">
          <el-button @click="handleSelectAllPrograms">全選</el-button>
          <el-button @click="handleUnselectAllPrograms">未選</el-button>
        </div>
      </el-tab-pane>
      <el-tab-pane label="選單權限" name="menu">
        <!-- 選單權限設定 -->
      </el-tab-pane>
      <el-tab-pane label="按鈕權限" name="button">
        <!-- 按鈕權限設定 -->
      </el-tab-pane>
    </el-tabs>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom1a1.api.ts`)
```typescript
import request from '@/utils/request';

export interface CustomerPermissionDto {
  tKey: number;
  custId: string;
  custName?: string;
  progId: string;
  progName?: string;
  permissionType: string;
  status: string;
}

export interface CustomerPermissionQueryDto {
  custId?: string;
  permissionType?: string;
  pageIndex: number;
  pageSize: number;
}

export interface SetProgramPermissionsDto {
  custId: string;
  permissions: Array<{
    progId: string;
    status: string;
  }>;
}

// API 函數
export const getCustomerPermissions = (query: CustomerPermissionQueryDto) => {
  return request.get<ApiResponse<PagedResult<CustomerPermissionDto>>>('/api/v1/xcom1a1/customer-permissions', { params: query });
};

export const getPrograms = () => {
  return request.get<ApiResponse<List<ProgramDto>>>('/api/v1/xcom1a1/programs');
};

export const getMenus = () => {
  return request.get<ApiResponse<List<MenuDto>>>('/api/v1/xcom1a1/menus');
};

export const getButtons = (progId: string) => {
  return request.get<ApiResponse<List<ButtonDto>>>('/api/v1/xcom1a1/buttons', { params: { progId } });
};

export const getSystems = () => {
  return request.get<ApiResponse<List<SystemDto>>>('/api/v1/xcom1a1/systems');
};

export const setProgramPermissions = (data: SetProgramPermissionsDto) => {
  return request.post<ApiResponse>('/api/v1/xcom1a1/set-program-permissions', data);
};

export const setMenuPermissions = (data: SetMenuPermissionsDto) => {
  return request.post<ApiResponse>('/api/v1/xcom1a1/set-menu-permissions', data);
};

export const setButtonPermissions = (data: SetButtonPermissionsDto) => {
  return request.post<ApiResponse>('/api/v1/xcom1a1/set-button-permissions', data);
};

export const deleteCustomerPermission = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom1a1/customer-permissions/${tKey}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立唯一約束
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
- [ ] 查詢頁面開發
- [ ] 權限設定對話框開發
- [ ] 作業/選單/按鈕列表元件開發
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
- 必須實作權限檢查
- 客戶代碼必須驗證存在性
- 批次操作必須使用事務處理

### 6.2 效能
- 大量權限設定必須使用批次處理
- 必須建立適當的索引
- 查詢結果必須使用分頁

### 6.3 資料驗證
- 客戶代碼必須存在
- 作業/選單/按鈕代碼必須存在
- 權限狀態必須在允許範圍內

### 6.4 業務邏輯
- 設定權限時必須檢查客戶是否存在
- 刪除權限前必須檢查是否有相關資料
- 批次設定必須使用事務處理，確保資料一致性

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢客戶作業權限成功
- [ ] 查詢作業列表成功
- [ ] 設定作業權限成功
- [ ] 設定作業權限失敗 (客戶不存在)
- [ ] 刪除客戶權限成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 批次操作測試

### 7.3 效能測試
- [ ] 大量權限設定測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_FQ.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_PROG_AC.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_MENU_AC.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A1_SET_BUTTON_AC.asp`

### 8.2 相關功能
- `開發計劃/09-客戶管理/CUS5110-客戶基本資料維護.md`
- `開發計劃/01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md`
- `開發計劃/01-系統管理/04-系統設定/SYS0440-系統功能按鈕資料維護.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

