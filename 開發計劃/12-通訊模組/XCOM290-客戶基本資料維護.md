# XCOM290 - 客戶基本資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM290
- **功能名稱**: 客戶基本資料維護
- **功能描述**: 提供客戶基本資料的新增、修改、刪除、查詢功能，包含客戶代碼、客戶名稱、客戶簡碼、公司電話、傳真、地址、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FS.ASP` (排序)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM290_PR.ASP` (報表)

### 1.2 業務需求
- 管理客戶基本資料
- 支援客戶的新增、修改、刪除、查詢
- 支援多條件查詢
- 支援資料排序
- 支援資料列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Customers` (客戶資料，對應舊系統 `CUST`)

```sql
CREATE TABLE [dbo].[Customers] (
    [CustId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 客戶代碼
    [CustName] NVARCHAR(200) NULL, -- 客戶名稱
    [CustShortName] NVARCHAR(50) NULL, -- 客戶簡碼
    [Tel] NVARCHAR(50) NULL, -- 公司電話
    [Fax] NVARCHAR(50) NULL, -- 傳真
    [Address] NVARCHAR(500) NULL, -- 地址
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Customers_CustName] ON [dbo].[Customers] ([CustName]);
CREATE NONCLUSTERED INDEX [IX_Customers_CustShortName] ON [dbo].[Customers] ([CustShortName]);
CREATE NONCLUSTERED INDEX [IX_Customers_Status] ON [dbo].[Customers] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢客戶列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom290/customers`
- **說明**: 查詢客戶列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CustId",
    "sortOrder": "ASC",
    "filters": {
      "custId": "",
      "custName": "",
      "custShortName": "",
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
          "custId": "C001",
          "custName": "客戶A",
          "custShortName": "CUSTA",
          "tel": "02-12345678",
          "fax": "02-12345679",
          "address": "台北市信義區",
          "status": "A",
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

#### 3.1.2 查詢單筆客戶
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom290/customers/{custId}`
- **說明**: 根據客戶代碼查詢單筆客戶資料
- **路徑參數**:
  - `custId`: 客戶代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "custId": "C001",
      "custName": "客戶A",
      "custShortName": "CUSTA",
      "tel": "02-12345678",
      "fax": "02-12345679",
      "address": "台北市信義區",
      "status": "A",
      "createdBy": "U001",
      "createdAt": "2024-01-01T00:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增客戶
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom290/customers`
- **說明**: 新增客戶資料
- **請求格式**:
  ```json
  {
    "custId": "C001",
    "custName": "客戶A",
    "custShortName": "CUSTA",
    "tel": "02-12345678",
    "fax": "02-12345679",
    "address": "台北市信義區",
    "status": "A"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "custId": "C001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改客戶
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom290/customers/{custId}`
- **說明**: 修改客戶資料
- **路徑參數**:
  - `custId`: 客戶代碼
- **請求格式**: 同新增，但 `custId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除客戶
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom290/customers/{custId}`
- **說明**: 刪除客戶資料（軟刪除或硬刪除）
- **路徑參數**:
  - `custId`: 客戶代碼
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

#### 3.1.6 批次刪除客戶
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom290/customers/batch`
- **說明**: 批次刪除多筆客戶
- **請求格式**:
  ```json
  {
    "custIds": ["C001", "C002", "C003"]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `Xcom290CustomersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom290/customers")]
    [Authorize]
    public class Xcom290CustomersController : ControllerBase
    {
        private readonly IXcom290CustomerService _customerService;
        
        public Xcom290CustomersController(IXcom290CustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CustomerDto>>>> GetCustomers([FromQuery] CustomerQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{custId}")]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomer(string custId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{custId}")]
        public async Task<ActionResult<ApiResponse>> UpdateCustomer(string custId, [FromBody] UpdateCustomerDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{custId}")]
        public async Task<ActionResult<ApiResponse>> DeleteCustomer(string custId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `Xcom290CustomerService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXcom290CustomerService
    {
        Task<PagedResult<CustomerDto>> GetCustomersAsync(CustomerQueryDto query);
        Task<CustomerDto> GetCustomerByIdAsync(string custId);
        Task<string> CreateCustomerAsync(CreateCustomerDto dto);
        Task UpdateCustomerAsync(string custId, UpdateCustomerDto dto);
        Task DeleteCustomerAsync(string custId);
    }
}
```

#### 3.2.3 Repository: `CustomerRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(string custId);
        Task<PagedResult<Customer>> GetPagedAsync(CustomerQuery query);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(string custId);
        Task<bool> ExistsAsync(string custId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 客戶列表頁面 (`CustomerList.vue`)
- **路徑**: `/xcom/customers`
- **功能**: 顯示客戶列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (CustomerSearchForm)
  - 資料表格 (CustomerDataTable)
  - 新增/修改對話框 (CustomerDialog)
  - 刪除確認對話框

#### 4.1.2 客戶詳細頁面 (`CustomerDetail.vue`)
- **路徑**: `/xcom/customers/:custId`
- **功能**: 顯示客戶詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`CustomerSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="客戶代碼">
      <el-input v-model="searchForm.custId" placeholder="請輸入客戶代碼" />
    </el-form-item>
    <el-form-item label="客戶名稱">
      <el-input v-model="searchForm.custName" placeholder="請輸入客戶名稱" />
    </el-form-item>
    <el-form-item label="客戶簡碼">
      <el-input v-model="searchForm.custShortName" placeholder="請輸入客戶簡碼" />
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

#### 4.2.2 資料表格元件 (`CustomerDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="customerList" v-loading="loading">
      <el-table-column prop="custId" label="客戶代碼" width="120" />
      <el-table-column prop="custName" label="客戶名稱" width="200" />
      <el-table-column prop="custShortName" label="客戶簡碼" width="120" />
      <el-table-column prop="tel" label="公司電話" width="150" />
      <el-table-column prop="fax" label="傳真" width="150" />
      <el-table-column prop="address" label="地址" width="250" />
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

#### 4.2.3 新增/修改對話框 (`CustomerDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="客戶代碼" prop="custId">
        <el-input v-model="form.custId" :disabled="isEdit" placeholder="請輸入客戶代碼" />
      </el-form-item>
      <el-form-item label="客戶名稱" prop="custName">
        <el-input v-model="form.custName" placeholder="請輸入客戶名稱" />
      </el-form-item>
      <el-form-item label="客戶簡碼" prop="custShortName">
        <el-input v-model="form.custShortName" placeholder="請輸入客戶簡碼" />
      </el-form-item>
      <el-form-item label="公司電話" prop="tel">
        <el-input v-model="form.tel" placeholder="請輸入公司電話" />
      </el-form-item>
      <el-form-item label="傳真" prop="fax">
        <el-input v-model="form.fax" placeholder="請輸入傳真" />
      </el-form-item>
      <el-form-item label="地址" prop="address">
        <el-input v-model="form.address" type="textarea" :rows="3" placeholder="請輸入地址" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="A" />
          <el-option label="停用" value="I" />
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

### 4.3 API 呼叫 (`customer.api.ts`)
```typescript
import request from '@/utils/request';

export interface CustomerDto {
  custId: string;
  custName?: string;
  custShortName?: string;
  tel?: string;
  fax?: string;
  address?: string;
  status: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface CustomerQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    custId?: string;
    custName?: string;
    custShortName?: string;
    status?: string;
  };
}

export interface CreateCustomerDto {
  custId: string;
  custName?: string;
  custShortName?: string;
  tel?: string;
  fax?: string;
  address?: string;
  status: string;
}

export interface UpdateCustomerDto extends Omit<CreateCustomerDto, 'custId'> {}

// API 函數
export const getCustomerList = (query: CustomerQueryDto) => {
  return request.get<ApiResponse<PagedResult<CustomerDto>>>('/api/v1/xcom290/customers', { params: query });
};

export const getCustomerById = (custId: string) => {
  return request.get<ApiResponse<CustomerDto>>(`/api/v1/xcom290/customers/${custId}`);
};

export const createCustomer = (data: CreateCustomerDto) => {
  return request.post<ApiResponse<string>>('/api/v1/xcom290/customers', data);
};

export const updateCustomer = (custId: string, data: UpdateCustomerDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom290/customers/${custId}`, data);
};

export const deleteCustomer = (custId: string) => {
  return request.delete<ApiResponse>(`/api/v1/xcom290/customers/${custId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引

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

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須記錄所有操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 客戶代碼必須唯一
- 必填欄位必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除客戶前必須檢查是否有相關資料
- 停用客戶時必須檢查是否有進行中的業務
- 客戶資料變更必須記錄變更資訊

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增客戶成功
- [ ] 新增客戶失敗 (重複代碼)
- [ ] 修改客戶成功
- [ ] 修改客戶失敗 (不存在)
- [ ] 刪除客戶成功
- [ ] 查詢客戶列表成功
- [ ] 查詢單筆客戶成功

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

### 7.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FB.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FI.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FU.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FD.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM290_FQ.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

