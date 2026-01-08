# SYSB206 - 廠/客基本資料維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSB206
- **功能名稱**: 廠/客基本資料維護作業
- **功能描述**: 提供廠商/客戶基本資料的新增、修改、刪除、查詢功能，包含統一編號、廠商名稱、聯絡資訊、銀行帳戶等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSB000/SYSB200_PR.ASP` (報表)

### 1.2 業務需求
- 管理廠商/客戶基本資料資訊
- 支援統一編號、身份證字號、自編編號三種識別方式
- 支援廠商編號自動產生（統一編號+"-"+流水號）
- 記錄廠商的聯絡資訊、地址、銀行帳戶等
- 支援多系統別管理
- 支援組織架構管理
- 支援狀態管理（啟用/停用）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Vendors` (對應舊系統 `VENDOR`)

```sql
CREATE TABLE [dbo].[Vendors] (
    [VendorId] NVARCHAR(12) NOT NULL PRIMARY KEY, -- 廠商編號 (VENDOR_ID)
    [GuiId] NVARCHAR(20) NOT NULL, -- 統一編號/身份證字號/自編編號 (GUI_ID)
    [GuiType] NVARCHAR(1) NOT NULL, -- 識別類型 (GUI_TYPE) 1:統一編號, 2:身份證字號, 3:自編編號
    [VendorName] NVARCHAR(200) NOT NULL, -- 廠商名稱(中文) (VENDOR_NAME)
    [VendorNameE] NVARCHAR(200) NULL, -- 廠商名稱(英文) (VENDOR_NAME_E)
    [VendorNameS] NVARCHAR(50) NULL, -- 廠商簡稱 (VENDOR_NAME_S)
    [Mcode] NVARCHAR(20) NULL, -- 郵電費負擔 (MCODE)
    [VendorRegAddr] NVARCHAR(500) NULL, -- 公司登記地址 (VENDOR_REG_ADDR)
    [TaxAddr] NVARCHAR(500) NULL, -- 發票地址 (TAX_ADDR)
    [VendorRegTel] NVARCHAR(50) NULL, -- 公司登記電話 (VENDOR_REG_TEL)
    [VendorFax] NVARCHAR(50) NULL, -- 公司傳真 (VENDOR_FAX)
    [VendorEmail] NVARCHAR(100) NULL, -- 公司電子郵件 (VENDOR_EMAIL)
    [InvEmail] NVARCHAR(100) NULL, -- 發票電子郵件 (INV_EMAIL)
    [ChargeStaff] NVARCHAR(50) NULL, -- 聯絡人 (CHARGE_STAFF)
    [ChargeTitle] NVARCHAR(50) NULL, -- 聯絡人職稱 (CHARGE_TITLE)
    [ChargePid] NVARCHAR(20) NULL, -- 聯絡人身份證字號 (CHARGE_PID)
    [ChargeTel] NVARCHAR(50) NULL, -- 聯絡人電話 (CHARGE_TEL)
    [ChargeAddr] NVARCHAR(500) NULL, -- 聯絡人聯絡地址 (CHARGE_ADDR)
    [ChargeEmail] NVARCHAR(100) NULL, -- 聯絡人電子郵件 (CHARGE_EMAIL)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
    [SysId] NVARCHAR(10) NOT NULL DEFAULT '1', -- 系統別 (SYS_ID)
    [PayType] NVARCHAR(20) NULL, -- 付款方式 (PAY_TYPE)
    [SuplBankId] NVARCHAR(50) NULL, -- 匯款銀行代碼 (SUPL_BANK_ID)
    [SuplBankAcct] NVARCHAR(50) NULL, -- 匯款銀行帳號 (SUPL_BANK_ACCT)
    [SuplAcctName] NVARCHAR(100) NULL, -- 帳戶名稱 (SUPL_ACCT_NAME)
    [TicketBe] NVARCHAR(20) NULL, -- 票據別 (TICKET_BE)
    [CheckTitle] NVARCHAR(100) NULL, -- 支票抬頭 (CHECK_TITLE)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED ([VendorId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Vendors_GuiId] ON [dbo].[Vendors] ([GuiId]);
CREATE NONCLUSTERED INDEX [IX_Vendors_VendorName] ON [dbo].[Vendors] ([VendorName]);
CREATE NONCLUSTERED INDEX [IX_Vendors_Status] ON [dbo].[Vendors] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Vendors_SysId] ON [dbo].[Vendors] ([SysId]);
CREATE NONCLUSTERED INDEX [IX_Vendors_OrgId] ON [dbo].[Vendors] ([OrgId]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| VendorId | NVARCHAR | 12 | NO | - | 廠商編號 | 主鍵，統一編號+"-"+流水號 |
| GuiId | NVARCHAR | 20 | NO | - | 統一編號/身份證字號/自編編號 | 唯一 |
| GuiType | NVARCHAR | 1 | NO | - | 識別類型 | 1:統一編號, 2:身份證字號, 3:自編編號 |
| VendorName | NVARCHAR | 200 | NO | - | 廠商名稱(中文) | - |
| VendorNameE | NVARCHAR | 200 | YES | - | 廠商名稱(英文) | - |
| VendorNameS | NVARCHAR | 50 | YES | - | 廠商簡稱 | - |
| Mcode | NVARCHAR | 20 | YES | - | 郵電費負擔 | - |
| VendorRegAddr | NVARCHAR | 500 | YES | - | 公司登記地址 | - |
| TaxAddr | NVARCHAR | 500 | YES | - | 發票地址 | - |
| VendorRegTel | NVARCHAR | 50 | YES | - | 公司登記電話 | - |
| VendorFax | NVARCHAR | 50 | YES | - | 公司傳真 | - |
| VendorEmail | NVARCHAR | 100 | YES | - | 公司電子郵件 | - |
| InvEmail | NVARCHAR | 100 | YES | - | 發票電子郵件 | - |
| ChargeStaff | NVARCHAR | 50 | YES | - | 聯絡人 | - |
| ChargeTitle | NVARCHAR | 50 | YES | - | 聯絡人職稱 | - |
| ChargePid | NVARCHAR | 20 | YES | - | 聯絡人身份證字號 | - |
| ChargeTel | NVARCHAR | 50 | YES | - | 聯絡人電話 | - |
| ChargeAddr | NVARCHAR | 500 | YES | - | 聯絡人聯絡地址 | - |
| ChargeEmail | NVARCHAR | 100 | YES | - | 聯絡人電子郵件 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SysId | NVARCHAR | 10 | NO | '1' | 系統別 | - |
| PayType | NVARCHAR | 20 | YES | - | 付款方式 | - |
| SuplBankId | NVARCHAR | 50 | YES | - | 匯款銀行代碼 | 外鍵至銀行表 |
| SuplBankAcct | NVARCHAR | 50 | YES | - | 匯款銀行帳號 | - |
| SuplAcctName | NVARCHAR | 100 | YES | - | 帳戶名稱 | - |
| TicketBe | NVARCHAR | 20 | YES | - | 票據別 | - |
| CheckTitle | NVARCHAR | 100 | YES | - | 支票抬頭 | - |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至組織表 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢廠商列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vendors`
- **說明**: 查詢廠商列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "VendorId",
    "sortOrder": "ASC",
    "filters": {
      "vendorId": "",
      "guiId": "",
      "vendorName": "",
      "status": "",
      "sysId": "",
      "orgId": ""
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
          "vendorId": "12345678-1",
          "guiId": "12345678",
          "guiType": "1",
          "vendorName": "測試廠商",
          "vendorNameE": "Test Vendor",
          "vendorNameS": "測試",
          "status": "A",
          "sysId": "1"
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

#### 3.1.2 查詢單筆廠商
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vendors/{vendorId}`
- **說明**: 根據廠商編號查詢單筆廠商資料
- **路徑參數**:
  - `vendorId`: 廠商編號
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 檢查統一編號是否存在
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/vendors/check-gui-id/{guiId}`
- **說明**: 檢查統一編號/身份證字號/自編編號是否已存在
- **路徑參數**:
  - `guiId`: 統一編號/身份證字號/自編編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "exists": false,
      "vendorId": null
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 新增廠商
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vendors`
- **說明**: 新增廠商資料，自動產生廠商編號
- **請求格式**:
  ```json
  {
    "guiId": "12345678",
    "guiType": "1",
    "vendorName": "測試廠商",
    "vendorNameE": "Test Vendor",
    "vendorNameS": "測試",
    "mcode": "",
    "vendorRegAddr": "台北市信義區",
    "taxAddr": "台北市信義區",
    "vendorRegTel": "02-12345678",
    "vendorFax": "02-12345679",
    "vendorEmail": "test@example.com",
    "invEmail": "invoice@example.com",
    "chargeStaff": "張三",
    "chargeTitle": "經理",
    "chargePid": "",
    "chargeTel": "0912345678",
    "chargeAddr": "",
    "chargeEmail": "",
    "status": "A",
    "sysId": "1",
    "payType": "",
    "suplBankId": "",
    "suplBankAcct": "",
    "suplAcctName": "",
    "ticketBe": "",
    "checkTitle": "",
    "orgId": "",
    "notes": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "vendorId": "12345678-1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改廠商
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vendors/{vendorId}`
- **說明**: 修改廠商資料
- **路徑參數**:
  - `vendorId`: 廠商編號
- **請求格式**: 同新增，但 `vendorId` 和 `guiId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除廠商
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vendors/{vendorId}`
- **說明**: 刪除廠商資料
- **路徑參數**:
  - `vendorId`: 廠商編號
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

#### 3.1.7 批次刪除廠商
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/vendors/batch`
- **說明**: 批次刪除多筆廠商
- **請求格式**:
  ```json
  {
    "vendorIds": ["12345678-1", "12345678-2", "12345678-3"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `VendorsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vendors")]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        
        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VendorDto>>>> GetVendors([FromQuery] VendorQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{vendorId}")]
        public async Task<ActionResult<ApiResponse<VendorDto>>> GetVendor(string vendorId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("check-gui-id/{guiId}")]
        public async Task<ActionResult<ApiResponse<CheckGuiIdResultDto>>> CheckGuiId(string guiId)
        {
            // 實作檢查統一編號邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateVendor([FromBody] CreateVendorDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{vendorId}")]
        public async Task<ActionResult<ApiResponse>> UpdateVendor(string vendorId, [FromBody] UpdateVendorDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{vendorId}")]
        public async Task<ActionResult<ApiResponse>> DeleteVendor(string vendorId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `VendorService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IVendorService
    {
        Task<PagedResult<VendorDto>> GetVendorsAsync(VendorQueryDto query);
        Task<VendorDto> GetVendorByIdAsync(string vendorId);
        Task<CheckGuiIdResultDto> CheckGuiIdAsync(string guiId);
        Task<string> CreateVendorAsync(CreateVendorDto dto);
        Task UpdateVendorAsync(string vendorId, UpdateVendorDto dto);
        Task DeleteVendorAsync(string vendorId);
        Task<string> GenerateVendorIdAsync(string guiId);
    }
}
```

#### 3.2.3 Repository: `VendorRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IVendorRepository
    {
        Task<Vendor> GetByIdAsync(string vendorId);
        Task<Vendor> GetByGuiIdAsync(string guiId);
        Task<PagedResult<Vendor>> GetPagedAsync(VendorQuery query);
        Task<Vendor> CreateAsync(Vendor vendor);
        Task<Vendor> UpdateAsync(Vendor vendor);
        Task DeleteAsync(string vendorId);
        Task<bool> ExistsByGuiIdAsync(string guiId);
        Task<int> GetNextSequenceAsync(string guiId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 廠商列表頁面 (`VendorList.vue`)
- **路徑**: `/master-data/vendors`
- **功能**: 顯示廠商列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (VendorSearchForm)
  - 資料表格 (VendorDataTable)
  - 新增/修改對話框 (VendorDialog)
  - 刪除確認對話框

#### 4.1.2 廠商詳細頁面 (`VendorDetail.vue`)
- **路徑**: `/master-data/vendors/:vendorId`
- **功能**: 顯示廠商詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`VendorSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="廠商編號">
      <el-input v-model="searchForm.vendorId" placeholder="請輸入廠商編號" />
    </el-form-item>
    <el-form-item label="統一編號">
      <el-input v-model="searchForm.guiId" placeholder="請輸入統一編號" />
    </el-form-item>
    <el-form-item label="廠商名稱">
      <el-input v-model="searchForm.vendorName" placeholder="請輸入廠商名稱" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
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

#### 4.2.2 資料表格元件 (`VendorDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="vendorList" v-loading="loading">
      <el-table-column prop="vendorId" label="廠商編號" width="150" />
      <el-table-column prop="guiId" label="統一編號" width="120" />
      <el-table-column prop="vendorName" label="廠商名稱" width="200" />
      <el-table-column prop="vendorNameS" label="廠商簡稱" width="120" />
      <el-table-column prop="vendorRegTel" label="電話" width="120" />
      <el-table-column prop="chargeStaff" label="聯絡人" width="100" />
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

#### 4.2.3 新增/修改對話框 (`VendorDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="900px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="識別類型" prop="guiType">
            <el-radio-group v-model="form.guiType">
              <el-radio label="1">統一編號</el-radio>
              <el-radio label="2">身份證字號</el-radio>
              <el-radio label="3">自編編號</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="getGuiIdLabel()" prop="guiId">
            <el-input 
              v-model="form.guiId" 
              :placeholder="getGuiIdPlaceholder()"
              @blur="handleCheckGuiId"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="廠商名稱(中文)" prop="vendorName">
            <el-input v-model="form.vendorName" placeholder="請輸入廠商名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="廠商名稱(英文)" prop="vendorNameE">
            <el-input v-model="form.vendorNameE" placeholder="請輸入英文名稱" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="廠商簡稱" prop="vendorNameS">
        <el-input v-model="form.vendorNameS" placeholder="請輸入廠商簡稱" />
      </el-form-item>
      <!-- 其他欄位... -->
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`vendor.api.ts`)
```typescript
import request from '@/utils/request';

export interface VendorDto {
  vendorId: string;
  guiId: string;
  guiType: string;
  vendorName: string;
  vendorNameE?: string;
  vendorNameS?: string;
  // ... 其他欄位
}

export interface VendorQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    vendorId?: string;
    guiId?: string;
    vendorName?: string;
    status?: string;
    sysId?: string;
    orgId?: string;
  };
}

export interface CreateVendorDto {
  guiId: string;
  guiType: string;
  vendorName: string;
  // ... 其他欄位
}

export interface UpdateVendorDto extends Omit<CreateVendorDto, 'guiId'> {}

// API 函數
export const getVendorList = (query: VendorQueryDto) => {
  return request.get<ApiResponse<PagedResult<VendorDto>>>('/api/v1/vendors', { params: query });
};

export const getVendorById = (vendorId: string) => {
  return request.get<ApiResponse<VendorDto>>(`/api/v1/vendors/${vendorId}`);
};

export const checkGuiId = (guiId: string) => {
  return request.get<ApiResponse<CheckGuiIdResultDto>>(`/api/v1/vendors/check-gui-id/${guiId}`);
};

export const createVendor = (data: CreateVendorDto) => {
  return request.post<ApiResponse<string>>('/api/v1/vendors', data);
};

export const updateVendor = (vendorId: string, data: UpdateVendorDto) => {
  return request.put<ApiResponse>(`/api/v1/vendors/${vendorId}`, data);
};

export const deleteVendor = (vendorId: string) => {
  return request.delete<ApiResponse>(`/api/v1/vendors/${vendorId}`);
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

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含廠商編號產生邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 統一編號檢查功能
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

**總計**: 11天

---

## 六、注意事項

### 6.1 業務邏輯
- 廠商編號產生規則：統一編號+"-"+流水號（1-9）
- 統一編號/身份證字號/自編編號必須唯一
- 新增時需檢查統一編號是否已存在
- 修改時不可變更統一編號和廠商編號
- 刪除前需檢查是否有相關業務資料（採購單、退貨單等）

### 6.2 資料驗證
- 統一編號格式驗證（8碼數字）
- 身份證字號格式驗證
- 電子郵件格式驗證
- 電話號碼格式驗證
- 必填欄位驗證

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 統一編號檢查需使用快取機制

### 6.4 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 防止 SQL Injection

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增廠商成功
- [ ] 新增廠商失敗 (統一編號重複)
- [ ] 修改廠商成功
- [ ] 修改廠商失敗 (不存在)
- [ ] 刪除廠商成功
- [ ] 查詢廠商列表成功
- [ ] 查詢單筆廠商成功
- [ ] 檢查統一編號成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 廠商編號自動產生測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200_FB.ASP`
- `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/VENDOR.cs`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/ASP/SYSB000/SYSB200.xsd` (如有)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

