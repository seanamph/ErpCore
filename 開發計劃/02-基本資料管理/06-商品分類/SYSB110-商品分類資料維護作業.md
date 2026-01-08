# SYSB110 - 商品分類資料維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSB110
- **功能名稱**: 商品分類資料維護作業
- **功能描述**: 提供商品分類資料的新增、修改、刪除、查詢功能，包含大分類、中分類、小分類的階層式管理，支援分類代碼、分類名稱、分類型式、會計科目等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FI.ASP` (大分類新增)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FU.ASP` (大分類修改)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FD.ASP` (大分類刪除)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FQ.ASP` (大分類查詢)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FB.ASP` (大分類瀏覽)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FI.ASP` (中分類新增)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FU.ASP` (中分類修改)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FD.ASP` (中分類刪除)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FQ.ASP` (中分類查詢)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FI.ASP` (小分類新增)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FU.ASP` (小分類修改)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FD.ASP` (小分類刪除)
  - `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FQ.ASP` (小分類查詢)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/BCATEGORY.cs` (大分類業務邏輯)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/MCATEGORY.cs` (中分類業務邏輯)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/SCATEGORY.cs` (小分類業務邏輯)

### 1.2 業務需求
- 管理商品分類的階層式結構（大分類→中分類→小分類）
- 支援分類代碼的唯一性檢查
- 支援分類型式設定（1:資料, 2:耗材）
- 支援會計科目設定（借/貸方科目、折舊科目、進項稅額科目）
- 支援分類狀態管理（啟用/停用）
- 支援分類項目個數統計
- 支援分類備註管理
- 支援分類的父子關係維護
- 支援分類查詢與報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ProductCategories` (對應舊系統 `CLASSIFY` / `AM_CLASSIFY` / `RIM_CLASSIFY`)

```sql
CREATE TABLE [dbo].[ProductCategories] (
    [TKey] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, -- 主鍵 (T_KEY)
    [ClassId] NVARCHAR(50) NOT NULL, -- 分類代碼 (CLASS_ID)
    [ClassName] NVARCHAR(100) NOT NULL, -- 分類名稱 (CLASS_NAME)
    [ClassType] NVARCHAR(10) NULL DEFAULT '1', -- 分類型式 (CLASS_TYPE, 1:資料, 2:耗材)
    [ClassMode] NVARCHAR(10) NOT NULL, -- 分類區分 (CLASS_MODE, 1:大分類, 2:中分類, 3:小分類)
    [BClassId] NVARCHAR(50) NULL, -- 大分類代碼 (B_CLASS_ID, 用於中分類和小分類)
    [MClassId] NVARCHAR(50) NULL, -- 中分類代碼 (M_CLASS_ID, 用於小分類)
    [ParentTKey] BIGINT NULL, -- 父分類主鍵 (PARENT_T_KEY)
    [StypeId] NVARCHAR(50) NULL, -- 所屬會計科目(借) (STYPE_ID)
    [StypeId2] NVARCHAR(50) NULL, -- 所屬會計科目(貸) (STYPE_ID2)
    [DepreStypeId] NVARCHAR(50) NULL, -- 折舊科目(借) (DEPRE_STYPE_ID)
    [DepreStypeId2] NVARCHAR(50) NULL, -- 累計折舊科目(貸) (DEPRE_STYPE_ID2)
    [StypeTax] NVARCHAR(50) NULL, -- 進項稅額科目(借) (STYPE_TAX)
    [ItemCount] INT NULL DEFAULT 0, -- 所屬項目個數 (ITEM_COUNT)
    [Status] NVARCHAR(10) NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [UQ_ProductCategories_ClassId_ClassMode_ParentTKey] UNIQUE ([ClassId], [ClassMode], [ParentTKey])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassId] ON [dbo].[ProductCategories] ([ClassId]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassMode] ON [dbo].[ProductCategories] ([ClassMode]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_BClassId] ON [dbo].[ProductCategories] ([BClassId]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_MClassId] ON [dbo].[ProductCategories] ([MClassId]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_ParentTKey] ON [dbo].[ProductCategories] ([ParentTKey]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_ClassType] ON [dbo].[ProductCategories] ([ClassType]);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_Status] ON [dbo].[ProductCategories] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `Products` - 商品主檔
```sql
-- 用於查詢商品與分類的對應關係
-- 商品主檔應包含 BClassId, MClassId, SClassId 欄位
```

#### 2.2.2 `AccountSubjects` - 會計科目主檔
```sql
-- 用於查詢會計科目列表
-- 參考會計科目設計
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ClassId | NVARCHAR | 50 | NO | - | 分類代碼 | 唯一性檢查需配合ClassMode和ParentTKey |
| ClassName | NVARCHAR | 100 | NO | - | 分類名稱 | - |
| ClassType | NVARCHAR | 10 | YES | '1' | 分類型式 | 1:資料, 2:耗材 |
| ClassMode | NVARCHAR | 10 | NO | - | 分類區分 | 1:大分類, 2:中分類, 3:小分類 |
| BClassId | NVARCHAR | 50 | YES | - | 大分類代碼 | 用於中分類和小分類 |
| MClassId | NVARCHAR | 50 | YES | - | 中分類代碼 | 用於小分類 |
| ParentTKey | BIGINT | - | YES | - | 父分類主鍵 | 外鍵至ProductCategories.TKey |
| StypeId | NVARCHAR | 50 | YES | - | 所屬會計科目(借) | 外鍵至會計科目表 |
| StypeId2 | NVARCHAR | 50 | YES | - | 所屬會計科目(貸) | 外鍵至會計科目表 |
| DepreStypeId | NVARCHAR | 50 | YES | - | 折舊科目(借) | 外鍵至會計科目表 |
| DepreStypeId2 | NVARCHAR | 50 | YES | - | 累計折舊科目(貸) | 外鍵至會計科目表 |
| StypeTax | NVARCHAR | 50 | YES | - | 進項稅額科目(借) | 外鍵至會計科目表 |
| ItemCount | INT | - | YES | 0 | 所屬項目個數 | 自動計算或手動維護 |
| Status | NVARCHAR | 10 | YES | 'A' | 狀態 | A:啟用, I:停用 |
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

#### 3.1.1 查詢分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories`
- **說明**: 查詢分類列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ClassId",
    "sortOrder": "ASC",
    "filters": {
      "classId": "",
      "className": "",
      "classMode": "", // 1:大分類, 2:中分類, 3:小分類
      "classType": "", // 1:資料, 2:耗材
      "bClassId": "",
      "mClassId": "",
      "parentTKey": null,
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
          "classId": "01",
          "className": "食品類",
          "classType": "1",
          "classMode": "1",
          "bClassId": null,
          "mClassId": null,
          "parentTKey": null,
          "stypeId": "ST001",
          "stypeId2": "ST002",
          "depreStypeId": null,
          "depreStypeId2": null,
          "stypeTax": "ST003",
          "itemCount": 100,
          "status": "A",
          "notes": "備註",
          "createdBy": "U001",
          "createdAt": "2024-01-01T10:00:00",
          "updatedBy": "U001",
          "updatedAt": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆分類
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories/{tKey}`
- **說明**: 根據主鍵查詢單筆分類資料
- **路徑參數**:
  - `tKey`: 分類主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 查詢分類樹狀結構
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories/tree`
- **說明**: 查詢分類的樹狀結構（大分類→中分類→小分類）
- **請求參數**:
  ```json
  {
    "classType": "", // 1:資料, 2:耗材, 空值:全部
    "status": "" // A:啟用, I:停用, 空值:全部
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "tKey": 1,
        "classId": "01",
        "className": "食品類",
        "classMode": "1",
        "children": [
          {
            "tKey": 2,
            "classId": "0101",
            "className": "生鮮食品",
            "classMode": "2",
            "bClassId": "01",
            "children": [
              {
                "tKey": 3,
                "classId": "010101",
                "className": "蔬菜",
                "classMode": "3",
                "bClassId": "01",
                "mClassId": "0101",
                "children": []
              }
            ]
          }
        ]
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 新增分類
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/product-categories`
- **說明**: 新增分類資料
- **請求格式**:
  ```json
  {
    "classId": "01",
    "className": "食品類",
    "classType": "1",
    "classMode": "1",
    "bClassId": null,
    "mClassId": null,
    "parentTKey": null,
    "stypeId": "ST001",
    "stypeId2": "ST002",
    "depreStypeId": null,
    "depreStypeId2": null,
    "stypeTax": "ST003",
    "itemCount": 0,
    "status": "A",
    "notes": "備註"
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

#### 3.1.5 修改分類
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/product-categories/{tKey}`
- **說明**: 修改分類資料
- **路徑參數**:
  - `tKey`: 分類主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除分類
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/product-categories/{tKey}`
- **說明**: 刪除分類資料（需檢查是否有子分類或商品使用）
- **路徑參數**:
  - `tKey`: 分類主鍵
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

#### 3.1.7 批次刪除分類
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/product-categories/batch`
- **說明**: 批次刪除多筆分類
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

#### 3.1.8 查詢大分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories/b-class`
- **說明**: 查詢所有大分類列表（用於下拉選單）
- **請求參數**:
  ```json
  {
    "classType": "", // 1:資料, 2:耗材, 空值:全部
    "status": "" // A:啟用, I:停用, 空值:全部
  }
  ```

#### 3.1.9 查詢中分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories/m-class`
- **說明**: 根據大分類查詢中分類列表（用於下拉選單）
- **請求參數**:
  ```json
  {
    "bClassId": "01", // 大分類代碼
    "classType": "", // 1:資料, 2:耗材, 空值:全部
    "status": "" // A:啟用, I:停用, 空值:全部
  }
  ```

#### 3.1.10 查詢小分類列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/product-categories/s-class`
- **說明**: 根據大分類和中分類查詢小分類列表（用於下拉選單）
- **請求參數**:
  ```json
  {
    "bClassId": "01", // 大分類代碼
    "mClassId": "0101", // 中分類代碼
    "classType": "", // 1:資料, 2:耗材, 空值:全部
    "status": "" // A:啟用, I:停用, 空值:全部
  }
  ```

#### 3.1.11 更新分類項目個數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/product-categories/{tKey}/item-count`
- **說明**: 更新分類的項目個數（通常由系統自動計算）
- **請求格式**:
  ```json
  {
    "itemCount": 100
  }
  ```

#### 3.1.12 啟用/停用分類
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/product-categories/{tKey}/status`
- **說明**: 啟用或停用分類
- **請求格式**:
  ```json
  {
    "status": "A" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ProductCategoriesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/product-categories")]
    [Authorize]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;
        
        public ProductCategoriesController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductCategoryDto>>>> GetProductCategories([FromQuery] ProductCategoryQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetProductCategory(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("tree")]
        public async Task<ActionResult<ApiResponse<List<ProductCategoryTreeDto>>>> GetProductCategoryTree([FromQuery] ProductCategoryTreeQueryDto query)
        {
            // 實作查詢樹狀結構邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateProductCategory([FromBody] CreateProductCategoryDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateProductCategory(long tKey, [FromBody] UpdateProductCategoryDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteProductCategory(long tKey)
        {
            // 實作刪除邏輯
        }
        
        [HttpGet("b-class")]
        public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetBClassList([FromQuery] ProductCategoryListQueryDto query)
        {
            // 實作查詢大分類列表邏輯
        }
        
        [HttpGet("m-class")]
        public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetMClassList([FromQuery] ProductCategoryListQueryDto query)
        {
            // 實作查詢中分類列表邏輯
        }
        
        [HttpGet("s-class")]
        public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetSClassList([FromQuery] ProductCategoryListQueryDto query)
        {
            // 實作查詢小分類列表邏輯
        }
    }
}
```

#### 3.2.2 Service: `ProductCategoryService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IProductCategoryService
    {
        Task<PagedResult<ProductCategoryDto>> GetProductCategoriesAsync(ProductCategoryQueryDto query);
        Task<ProductCategoryDto> GetProductCategoryByIdAsync(long tKey);
        Task<List<ProductCategoryTreeDto>> GetProductCategoryTreeAsync(ProductCategoryTreeQueryDto query);
        Task<long> CreateProductCategoryAsync(CreateProductCategoryDto dto);
        Task UpdateProductCategoryAsync(long tKey, UpdateProductCategoryDto dto);
        Task DeleteProductCategoryAsync(long tKey);
        Task<List<ProductCategoryDto>> GetBClassListAsync(ProductCategoryListQueryDto query);
        Task<List<ProductCategoryDto>> GetMClassListAsync(ProductCategoryListQueryDto query);
        Task<List<ProductCategoryDto>> GetSClassListAsync(ProductCategoryListQueryDto query);
        Task<bool> ValidateClassIdUniqueAsync(string classId, string classMode, long? parentTKey, long? excludeTKey = null);
        Task<bool> HasChildrenAsync(long tKey);
        Task<bool> HasProductsAsync(long tKey);
    }
}
```

#### 3.2.3 Repository: `ProductCategoryRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<ProductCategory> GetByIdAsync(long tKey);
        Task<PagedResult<ProductCategory>> GetPagedAsync(ProductCategoryQuery query);
        Task<List<ProductCategory>> GetTreeAsync(ProductCategoryTreeQuery query);
        Task<ProductCategory> CreateAsync(ProductCategory category);
        Task<ProductCategory> UpdateAsync(ProductCategory category);
        Task DeleteAsync(long tKey);
        Task<bool> ExistsAsync(string classId, string classMode, long? parentTKey, long? excludeTKey = null);
        Task<bool> HasChildrenAsync(long tKey);
        Task<bool> HasProductsAsync(long tKey);
        Task<List<ProductCategory>> GetBClassListAsync(ProductCategoryListQuery query);
        Task<List<ProductCategory>> GetMClassListAsync(ProductCategoryListQuery query);
        Task<List<ProductCategory>> GetSClassListAsync(ProductCategoryListQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 分類列表頁面 (`ProductCategoryList.vue`)
- **路徑**: `/master-data/product-categories`
- **功能**: 顯示分類列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ProductCategorySearchForm)
  - 分類樹狀結構 (ProductCategoryTree)
  - 資料表格 (ProductCategoryDataTable)
  - 新增/修改對話框 (ProductCategoryDialog)
  - 刪除確認對話框

#### 4.1.2 分類詳細頁面 (`ProductCategoryDetail.vue`)
- **路徑**: `/master-data/product-categories/:tKey`
- **功能**: 顯示分類詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ProductCategorySearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="分類代碼">
      <el-input v-model="searchForm.classId" placeholder="請輸入分類代碼" />
    </el-form-item>
    <el-form-item label="分類名稱">
      <el-input v-model="searchForm.className" placeholder="請輸入分類名稱" />
    </el-form-item>
    <el-form-item label="分類區分">
      <el-select v-model="searchForm.classMode" placeholder="請選擇分類區分">
        <el-option label="大分類" value="1" />
        <el-option label="中分類" value="2" />
        <el-option label="小分類" value="3" />
      </el-select>
    </el-form-item>
    <el-form-item label="分類型式">
      <el-select v-model="searchForm.classType" placeholder="請選擇分類型式">
        <el-option label="資料" value="1" />
        <el-option label="耗材" value="2" />
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

#### 4.2.2 分類樹狀結構元件 (`ProductCategoryTree.vue`)
```vue
<template>
  <div class="category-tree">
    <el-tree
      :data="treeData"
      :props="treeProps"
      :expand-on-click-node="false"
      :default-expand-all="false"
      node-key="tKey"
      @node-click="handleNodeClick"
    >
      <template #default="{ node, data }">
        <span class="tree-node">
          <span>{{ data.classId }} - {{ data.className }}</span>
          <span class="tree-node-actions">
            <el-button type="primary" size="small" @click.stop="handleAdd(data)">新增</el-button>
            <el-button type="primary" size="small" @click.stop="handleEdit(data)">修改</el-button>
            <el-button type="danger" size="small" @click.stop="handleDelete(data)">刪除</el-button>
          </span>
        </span>
      </template>
    </el-tree>
  </div>
</template>
```

#### 4.2.3 資料表格元件 (`ProductCategoryDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="categoryList" v-loading="loading">
      <el-table-column prop="classId" label="分類代碼" width="120" />
      <el-table-column prop="className" label="分類名稱" width="200" />
      <el-table-column prop="classMode" label="分類區分" width="100">
        <template #default="{ row }">
          {{ getClassModeText(row.classMode) }}
        </template>
      </el-table-column>
      <el-table-column prop="classType" label="分類型式" width="100">
        <template #default="{ row }">
          {{ getClassTypeText(row.classType) }}
        </template>
      </el-table-column>
      <el-table-column prop="bClassId" label="大分類" width="100" />
      <el-table-column prop="mClassId" label="中分類" width="100" />
      <el-table-column prop="itemCount" label="項目個數" width="100" />
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

#### 4.2.4 新增/修改對話框 (`ProductCategoryDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="分類區分" prop="classMode">
        <el-select v-model="form.classMode" placeholder="請選擇分類區分" :disabled="isEdit">
          <el-option label="大分類" value="1" />
          <el-option label="中分類" value="2" />
          <el-option label="小分類" value="3" />
        </el-select>
      </el-form-item>
      <el-form-item label="大分類" prop="bClassId" v-if="form.classMode === '2' || form.classMode === '3'">
        <el-select v-model="form.bClassId" placeholder="請選擇大分類" @change="handleBClassChange">
          <el-option v-for="item in bClassList" :key="item.classId" :label="item.className" :value="item.classId" />
        </el-select>
      </el-form-item>
      <el-form-item label="中分類" prop="mClassId" v-if="form.classMode === '3'">
        <el-select v-model="form.mClassId" placeholder="請選擇中分類" :disabled="!form.bClassId">
          <el-option v-for="item in mClassList" :key="item.classId" :label="item.className" :value="item.classId" />
        </el-select>
      </el-form-item>
      <el-form-item label="分類代碼" prop="classId">
        <el-input v-model="form.classId" :disabled="isEdit" placeholder="請輸入分類代碼" />
      </el-form-item>
      <el-form-item label="分類名稱" prop="className">
        <el-input v-model="form.className" placeholder="請輸入分類名稱" />
      </el-form-item>
      <el-form-item label="分類型式" prop="classType">
        <el-select v-model="form.classType" placeholder="請選擇分類型式">
          <el-option label="資料" value="1" />
          <el-option label="耗材" value="2" />
        </el-select>
      </el-form-item>
      <el-form-item label="所屬會計科目(借)" prop="stypeId">
        <el-select v-model="form.stypeId" placeholder="請選擇會計科目" filterable>
          <el-option v-for="item in accountSubjectList" :key="item.subjectId" :label="item.subjectName" :value="item.subjectId" />
        </el-select>
      </el-form-item>
      <el-form-item label="所屬會計科目(貸)" prop="stypeId2">
        <el-select v-model="form.stypeId2" placeholder="請選擇會計科目" filterable>
          <el-option v-for="item in accountSubjectList" :key="item.subjectId" :label="item.subjectName" :value="item.subjectId" />
        </el-select>
      </el-form-item>
      <el-form-item label="折舊科目(借)" prop="depreStypeId">
        <el-select v-model="form.depreStypeId" placeholder="請選擇會計科目" filterable>
          <el-option v-for="item in accountSubjectList" :key="item.subjectId" :label="item.subjectName" :value="item.subjectId" />
        </el-select>
      </el-form-item>
      <el-form-item label="累計折舊科目(貸)" prop="depreStypeId2">
        <el-select v-model="form.depreStypeId2" placeholder="請選擇會計科目" filterable>
          <el-option v-for="item in accountSubjectList" :key="item.subjectId" :label="item.subjectName" :value="item.subjectId" />
        </el-select>
      </el-form-item>
      <el-form-item label="進項稅額科目(借)" prop="stypeTax">
        <el-select v-model="form.stypeTax" placeholder="請選擇會計科目" filterable>
          <el-option v-for="item in accountSubjectList" :key="item.subjectId" :label="item.subjectName" :value="item.subjectId" />
        </el-select>
      </el-form-item>
      <el-form-item label="項目個數" prop="itemCount">
        <el-input-number v-model="form.itemCount" :min="0" :disabled="true" />
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

### 4.3 API 呼叫 (`productCategory.api.ts`)
```typescript
import request from '@/utils/request';

export interface ProductCategoryDto {
  tKey: number;
  classId: string;
  className: string;
  classType?: string;
  classMode: string;
  bClassId?: string;
  mClassId?: string;
  parentTKey?: number;
  stypeId?: string;
  stypeId2?: string;
  depreStypeId?: string;
  depreStypeId2?: string;
  stypeTax?: string;
  itemCount?: number;
  status?: string;
  notes?: string;
  createdBy?: string;
  createdAt?: string;
  updatedBy?: string;
  updatedAt?: string;
}

export interface ProductCategoryTreeDto extends ProductCategoryDto {
  children: ProductCategoryTreeDto[];
}

export interface ProductCategoryQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    classId?: string;
    className?: string;
    classMode?: string;
    classType?: string;
    bClassId?: string;
    mClassId?: string;
    parentTKey?: number;
    status?: string;
  };
}

export interface CreateProductCategoryDto {
  classId: string;
  className: string;
  classType?: string;
  classMode: string;
  bClassId?: string;
  mClassId?: string;
  parentTKey?: number;
  stypeId?: string;
  stypeId2?: string;
  depreStypeId?: string;
  depreStypeId2?: string;
  stypeTax?: string;
  itemCount?: number;
  status?: string;
  notes?: string;
}

export interface UpdateProductCategoryDto extends Omit<CreateProductCategoryDto, 'classId' | 'classMode'> {}

// API 函數
export const getProductCategoryList = (query: ProductCategoryQueryDto) => {
  return request.get<ApiResponse<PagedResult<ProductCategoryDto>>>('/api/v1/product-categories', { params: query });
};

export const getProductCategoryById = (tKey: number) => {
  return request.get<ApiResponse<ProductCategoryDto>>(`/api/v1/product-categories/${tKey}`);
};

export const getProductCategoryTree = (query?: { classType?: string; status?: string }) => {
  return request.get<ApiResponse<ProductCategoryTreeDto[]>>('/api/v1/product-categories/tree', { params: query });
};

export const createProductCategory = (data: CreateProductCategoryDto) => {
  return request.post<ApiResponse<number>>('/api/v1/product-categories', data);
};

export const updateProductCategory = (tKey: number, data: UpdateProductCategoryDto) => {
  return request.put<ApiResponse>(`/api/v1/product-categories/${tKey}`, data);
};

export const deleteProductCategory = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/product-categories/${tKey}`);
};

export const getBClassList = (query?: { classType?: string; status?: string }) => {
  return request.get<ApiResponse<ProductCategoryDto[]>>('/api/v1/product-categories/b-class', { params: query });
};

export const getMClassList = (query: { bClassId: string; classType?: string; status?: string }) => {
  return request.get<ApiResponse<ProductCategoryDto[]>>('/api/v1/product-categories/m-class', { params: query });
};

export const getSClassList = (query: { bClassId: string; mClassId: string; classType?: string; status?: string }) => {
  return request.get<ApiResponse<ProductCategoryDto[]>>('/api/v1/product-categories/s-class', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立唯一性約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作（唯一性檢查、階層關係驗證）
- [ ] 樹狀結構查詢邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 樹狀結構元件開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 階層式下拉選單（大分類→中分類→小分類）
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 階層關係測試
- [ ] 唯一性檢查測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作輸入驗證與防SQL注入

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 樹狀結構查詢必須使用遞迴CTE或預先計算
- 必須使用快取機制（分類列表）

### 6.3 資料驗證
- 分類代碼必須唯一（配合ClassMode和ParentTKey）
- 必填欄位必須驗證
- 階層關係必須驗證（中分類必須有對應的大分類，小分類必須有對應的大分類和中分類）
- 分類代碼格式必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除分類前必須檢查是否有子分類
- 刪除分類前必須檢查是否有商品使用該分類
- 修改分類代碼時必須檢查唯一性
- 修改父分類時必須檢查是否會造成循環引用
- 分類項目個數應由系統自動計算或手動維護
- 會計科目必須存在於會計科目主檔

### 6.5 階層關係
- 大分類：ClassMode='1', BClassId=NULL, MClassId=NULL, ParentTKey=NULL
- 中分類：ClassMode='2', BClassId必須存在, MClassId=NULL, ParentTKey指向大分類
- 小分類：ClassMode='3', BClassId必須存在, MClassId必須存在, ParentTKey指向中分類

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增大分類成功
- [ ] 新增大分類失敗（重複代碼）
- [ ] 新增中分類成功
- [ ] 新增中分類失敗（大分類不存在）
- [ ] 新增小分類成功
- [ ] 新增小分類失敗（中分類不存在）
- [ ] 修改分類成功
- [ ] 修改分類失敗（不存在）
- [ ] 刪除分類成功
- [ ] 刪除分類失敗（有子分類）
- [ ] 刪除分類失敗（有商品使用）
- [ ] 查詢分類列表成功
- [ ] 查詢分類樹狀結構成功
- [ ] 查詢單筆分類成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 階層關係測試
- [ ] 唯一性檢查測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 樹狀結構查詢效能測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FI.ASP` (大分類新增)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FU.ASP` (大分類修改)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FD.ASP` (大分類刪除)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FQ.ASP` (大分類查詢)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA110_FB.ASP` (大分類瀏覽)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FI.ASP` (中分類新增)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FU.ASP` (中分類修改)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FD.ASP` (中分類刪除)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA111_FQ.ASP` (中分類查詢)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FI.ASP` (小分類新增)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FU.ASP` (小分類修改)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FD.ASP` (小分類刪除)
- `WEB/IMS_CORE/ASP/SYSA000/SYSA113_FQ.ASP` (小分類查詢)
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/BCATEGORY.cs` (大分類業務邏輯)
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/MCATEGORY.cs` (中分類業務邏輯)
- `IMS3/HANSHIN/RSL_CLASS/IMS3_RS/SCATEGORY.cs` (小分類業務邏輯)

### 8.2 資料庫 Schema
- 舊系統資料表：`CLASSIFY`, `AM_CLASSIFY`, `RIM_CLASSIFY`
- 欄位對應關係請參考資料字典

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

