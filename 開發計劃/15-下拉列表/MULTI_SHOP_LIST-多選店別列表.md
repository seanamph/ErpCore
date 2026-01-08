# MULTI_SHOP_LIST - 多選店別列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MULTI_SHOP_LIST
- **功能名稱**: 多選店別列表
- **功能描述**: 提供多選店別選擇的下拉列表功能，支援店別查詢、篩選（區域、店型態、店級別）、多選等功能，用於需要選擇多個店別的業務場景
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_SHOP_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_SHOP_LIST.aspx.cs`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_LIST.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_LIST1.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_LIST2.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_LIST_CHG.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_LIST_HT.aspx`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_SHOP_DEPOT_LIST.aspx`

### 1.2 業務需求
- 提供店別列表查詢功能
- 支援店別名稱篩選
- 支援區域篩選（REGION_ID）
- 支援店型態篩選（TYPE_ID）
- 支援店級別篩選（SHOP_LEVEL）
- 支援多選店別功能
- 支援「全部分店」選項（ALL_SHOP）
- 支援商品相關店別查詢（GOODS_ID）
- 支援供應商店別查詢（KIND=1）
- 支援已選店別的清除功能
- 支援選擇結果回傳至父視窗
- 與店別主檔（SHOP）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Shops` (店別主檔，對應舊系統 `SHOP`)

```sql
CREATE TABLE [dbo].[Shops] (
    [ShopId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 店別代碼 (SHOP_ID)
    [ShopName] NVARCHAR(100) NOT NULL, -- 店別名稱 (DOC)
    [RegionId] NVARCHAR(50) NULL, -- 區域代碼 (REGION_ID)
    [TypeId] NVARCHAR(50) NULL, -- 店型態 (TYPE_ID)
    [ShopLevel] NVARCHAR(50) NULL, -- 店級別 (SHOP_LEVEL)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Shops] PRIMARY KEY CLUSTERED ([ShopId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Shops_ShopName] ON [dbo].[Shops] ([ShopName]);
CREATE NONCLUSTERED INDEX [IX_Shops_RegionId] ON [dbo].[Shops] ([RegionId]);
CREATE NONCLUSTERED INDEX [IX_Shops_TypeId] ON [dbo].[Shops] ([TypeId]);
CREATE NONCLUSTERED INDEX [IX_Shops_ShopLevel] ON [dbo].[Shops] ([ShopLevel]);
CREATE NONCLUSTERED INDEX [IX_Shops_Status] ON [dbo].[Shops] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `Regions` - 區域主檔
參考 `SYSB450-區域基本資料維護作業.md` 的 `Regions` 資料表設計

#### 2.2.2 `Parameters` - 參數主檔
用於查詢店型態（SHOP_TYPE）和店級別（SHOP_LEVEL）參數
參考 `SYSBC40-參數資料設定維護作業.md` 的 `Parameters` 資料表設計

#### 2.2.3 `SuppShops` - 供應商店別對應（可選）
```sql
CREATE TABLE [dbo].[SuppShops] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ShopId] NVARCHAR(50) NOT NULL,
    [SuppId] NVARCHAR(50) NOT NULL,
    CONSTRAINT [FK_SuppShops_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SuppShops_ShopId] ON [dbo].[SuppShops] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_SuppShops_SuppId] ON [dbo].[SuppShops] ([SuppId]);
```

#### 2.2.4 `GoodsD` - 商品店別對應（可選）
```sql
CREATE TABLE [dbo].[GoodsD] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [GoodsId] NVARCHAR(50) NOT NULL,
    [ShopId] NVARCHAR(50) NOT NULL,
    [MinQty] DECIMAL(18,2) NULL, -- 最小庫存量 (MINQTY)
    CONSTRAINT [FK_GoodsD_Shops] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[Shops] ([ShopId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_GoodsD_ShopId] ON [dbo].[GoodsD] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_GoodsD_GoodsId] ON [dbo].[GoodsD] ([GoodsId]);
```

### 2.3 資料字典

#### Shops 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ShopId | NVARCHAR | 50 | NO | - | 店別代碼 | 主鍵 |
| ShopName | NVARCHAR | 100 | NO | - | 店別名稱 | - |
| RegionId | NVARCHAR | 50 | YES | - | 區域代碼 | 外鍵至Regions表 |
| TypeId | NVARCHAR | 50 | YES | - | 店型態 | 參數代碼 |
| ShopLevel | NVARCHAR | 50 | YES | - | 店級別 | 參數代碼 |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢店別列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/shops`
- **說明**: 查詢店別列表，支援篩選、排序
- **請求參數**:
  - `shopName`: 店別名稱（模糊查詢，可選）
  - `regionIds`: 區域代碼列表（逗號分隔，可選）
  - `typeIds`: 店型態代碼列表（逗號分隔，可選）
  - `shopLevels`: 店級別代碼列表（逗號分隔，可選）
  - `status`: 狀態（預設為'1'）
  - `kind`: 查詢類型（'1':供應商店別，其他:一般店別，可選）
  - `goodsId`: 商品代碼（當需要查詢商品相關店別時，可選）
  - `allShop`: 是否包含全部分店（'Y':是，可選）
  - `sortField`: 排序欄位（預設為'ShopId'）
  - `sortOrder`: 排序方向（ASC/DESC，預設為'ASC'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "shopId": "S001",
          "shopName": "台北店",
          "regionId": "R001",
          "regionName": "北區",
          "typeId": "T001",
          "typeName": "直營店",
          "shopLevel": "L001",
          "shopLevelName": "A級",
          "status": "1"
        }
      ],
      "totalCount": 10
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆店別
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/shops/{shopId}`
- **說明**: 根據店別代碼查詢單筆店別資料
- **路徑參數**:
  - `shopId`: 店別代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "shopId": "S001",
      "shopName": "台北店",
      "regionId": "R001",
      "regionName": "北區",
      "typeId": "T001",
      "typeName": "直營店",
      "shopLevel": "L001",
      "shopLevelName": "A級",
      "status": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢店別選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/shops/options`
- **說明**: 取得店別選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `regionIds`: 區域代碼列表（逗號分隔，可選）
  - `typeIds`: 店型態代碼列表（逗號分隔，可選）
  - `shopLevels`: 店級別代碼列表（逗號分隔，可選）
  - `status`: 狀態（預設為'1'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "S001",
        "label": "台北店"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 查詢區域選項（用於篩選）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/regions/options`
- **說明**: 取得區域選項列表（用於篩選下拉選單）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "R001",
        "label": "北區"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 查詢店型態選項（用於篩選）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/parameters/shop-type/options`
- **說明**: 取得店型態參數選項列表（用於篩選下拉選單）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "T001",
        "label": "直營店"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 查詢店級別選項（用於篩選）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/parameters/shop-level/options`
- **說明**: 取得店級別參數選項列表（用於篩選下拉選單）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "L001",
        "label": "A級"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ShopListController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lists/shops")]
    public class ShopListController : ControllerBase
    {
        private readonly IShopListService _shopListService;
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ShopDto>>>> GetShops([FromQuery] ShopListQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{shopId}")]
        public async Task<ActionResult<ApiResponse<ShopDto>>> GetShop(string shopId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpGet("options")]
        public async Task<ActionResult<ApiResponse<List<OptionDto>>>> GetShopOptions([FromQuery] ShopListQueryDto query)
        {
            // 實作查詢選項邏輯
        }
    }
}
```

#### 3.2.2 Service: `ShopListService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IShopListService
    {
        Task<PagedResult<ShopDto>> GetShopsAsync(ShopListQueryDto query);
        Task<ShopDto> GetShopAsync(string shopId);
        Task<List<OptionDto>> GetShopOptionsAsync(ShopListQueryDto query);
    }
}
```

#### 3.2.3 Repository: `ShopListRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IShopListRepository
    {
        Task<Shop> GetByIdAsync(string shopId);
        Task<PagedResult<Shop>> GetPagedAsync(ShopListQuery query);
        Task<List<Shop>> GetByFilterAsync(ShopListQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 主要元件
- **店別列表表格**: 顯示所有店別資料
- **篩選表單**: 支援區域、店型態、店級別、店別名稱篩選
- **選擇按鈕**: 支援多選功能
- **全部分店按鈕**: 選擇所有店別（當ALL_SHOP='Y'時顯示）
- **關閉按鈕**: 關閉視窗並回傳選擇結果

### 4.2 頁面佈局

```
┌─────────────────────────────────────┐
│  多選店別列表                       │
├─────────────────────────────────────┤
│  區域: [下拉選單] 店型態: [下拉選單] │
│  店級別: [下拉選單] 店別名稱: [輸入] │
│  [查詢] [重置]                      │
├─────────────────────────────────────┤
│  符合條件者共 X 筆                   │
│  [全部分店] [全部選取] [全部取消]    │
├─────────────────────────────────────┤
│  序號 │ 店別編號 │ 店別名稱 │ 區域 │ │
├─────────────────────────────────────┤
│   1  │  S001    │ 台北店   │ 北區 │ │
│   2  │  S002    │ 高雄店   │ 南區 │ │
│  ... │  ...     │ ...      │ ... │ │
└─────────────────────────────────────┘
│  [確定] [關閉]                      │
└─────────────────────────────────────┘
```

### 4.3 互動功能

#### 4.3.1 多選功能
- 點擊表格列可選擇/取消選擇店別
- 已選擇的店別會標記顯示
- 支援選擇結果累積
- 支援「全部選取」和「全部取消」功能

#### 4.3.2 篩選功能
- 支援區域多選篩選
- 支援店型態多選篩選
- 支援店級別多選篩選
- 支援店別名稱模糊查詢
- 支援篩選條件組合查詢

#### 4.3.3 回傳功能
- 選擇完成後，將選擇結果回傳至父視窗的指定控制項
- 回傳格式: `店別代碼1,店別代碼2,...`
- 支援「全部分店」選項回傳
- 支援選擇結果的清除確認

### 4.4 Vue 元件設計

#### 4.4.1 元件結構
```vue
<template>
  <div class="multi-shop-list">
    <el-card>
      <div slot="header">
        <span>多選店別列表</span>
        <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
      </div>
      
      <el-form :model="searchForm" inline>
        <el-form-item label="區域">
          <el-select
            v-model="searchForm.regionIds"
            multiple
            placeholder="請選擇區域"
            clearable
          >
            <el-option
              v-for="item in regionOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="店型態">
          <el-select
            v-model="searchForm.typeIds"
            multiple
            placeholder="請選擇店型態"
            clearable
          >
            <el-option
              v-for="item in typeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="店級別">
          <el-select
            v-model="searchForm.shopLevels"
            multiple
            placeholder="請選擇店級別"
            clearable
          >
            <el-option
              v-for="item in levelOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="店別名稱">
          <el-input
            v-model="searchForm.shopName"
            placeholder="請輸入店別名稱"
            clearable
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleSearch">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      
      <div class="toolbar">
        <span>符合條件者共 {{ totalCount }} 筆</span>
        <div>
          <el-button v-if="showAllShop" @click="handleSelectAllShops">全部分店</el-button>
          <el-button @click="handleSelectAll">全部選取</el-button>
          <el-button @click="handleClearAll">全部取消</el-button>
        </div>
      </div>
      
      <el-table
        :data="shopList"
        highlight-current-row
        @row-click="handleRowClick"
        @selection-change="handleSelectionChange"
        style="width: 100%"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column type="index" label="序號" width="80" />
        <el-table-column prop="shopId" label="店別編號" width="120" />
        <el-table-column prop="shopName" label="店別名稱" width="150" />
        <el-table-column prop="regionName" label="區域" width="100" />
        <el-table-column prop="typeName" label="店型態" width="100" />
        <el-table-column prop="shopLevelName" label="店級別" width="100" />
      </el-table>
      
      <div class="footer">
        <el-button type="primary" @click="handleConfirm">確定</el-button>
        <el-button @click="handleClose">關閉</el-button>
      </div>
    </el-card>
  </div>
</template>
```

#### 4.4.2 資料結構
```typescript
interface Shop {
  shopId: string;
  shopName: string;
  regionId?: string;
  regionName?: string;
  typeId?: string;
  typeName?: string;
  shopLevel?: string;
  shopLevelName?: string;
  status: string;
}

interface ShopListSearchForm {
  regionIds: string[];
  typeIds: string[];
  shopLevels: string[];
  shopName: string;
}

interface MultiShopListData {
  shopList: Shop[];
  searchForm: ShopListSearchForm;
  selectedShops: string[];
  regionOptions: Option[];
  typeOptions: Option[];
  levelOptions: Option[];
  loading: boolean;
  totalCount: number;
  showAllShop: boolean;
}
```

#### 4.4.3 方法設計
```typescript
methods: {
  // 載入店別列表
  async loadShopList(): Promise<void>;
  
  // 載入篩選選項
  async loadFilterOptions(): Promise<void>;
  
  // 搜尋店別
  handleSearch(): void;
  
  // 重置搜尋條件
  handleReset(): void;
  
  // 選擇全部分店
  handleSelectAllShops(): void;
  
  // 選擇全部店別
  handleSelectAll(): void;
  
  // 取消全部選擇
  handleClearAll(): void;
  
  // 點擊表格列
  handleRowClick(row: Shop): void;
  
  // 選擇變更
  handleSelectionChange(selection: Shop[]): void;
  
  // 確認選擇
  handleConfirm(): void;
  
  // 關閉視窗
  handleClose(): void;
}
```

---

## 五、開發計劃

### 5.1 開發階段

#### 階段一：資料庫設計與建立（1.5天）
- [ ] 建立 `Shops` 資料表
- [ ] 建立 `SuppShops` 資料表（可選）
- [ ] 建立 `GoodsD` 資料表（可選）
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立測試資料

#### 階段二：後端 API 開發（3天）
- [ ] 實作查詢店別列表 API
- [ ] 實作查詢單筆店別 API
- [ ] 實作查詢店別選項 API
- [ ] 實作查詢區域選項 API
- [ ] 實作查詢店型態選項 API
- [ ] 實作查詢店級別選項 API
- [ ] 實作篩選邏輯（區域、店型態、店級別、商品、供應商）
- [ ] 單元測試

#### 階段三：前端 UI 開發（4天）
- [ ] 建立 Vue 元件
- [ ] 實作店別列表顯示
- [ ] 實作多選功能
- [ ] 實作篩選功能（區域、店型態、店級別、店別名稱）
- [ ] 實作特殊選項（全部分店）
- [ ] 實作回傳功能
- [ ] 單元測試

#### 階段四：整合測試（1.5天）
- [ ] 前後端整合測試
- [ ] 功能測試
- [ ] 效能測試
- [ ] 多種查詢模式測試（一般、供應商、商品相關）

#### 階段五：文件與部署（1天）
- [ ] 更新 API 文件
- [ ] 更新使用者手冊
- [ ] 部署至測試環境

**總計**: 11天

### 5.2 技術要點

#### 5.2.1 後端技術
- 使用 Dapper 進行資料庫查詢
- 使用 Entity Framework Core 進行資料驗證
- 使用 AutoMapper 進行物件對應
- 使用 Serilog 進行日誌記錄
- 支援複雜的篩選條件組合查詢

#### 5.2.2 前端技術
- 使用 Vue 3 Composition API
- 使用 Element Plus UI 框架
- 使用 TypeScript 進行型別檢查
- 使用 Axios 進行 API 呼叫
- 支援多選下拉選單

### 5.3 注意事項

1. **多選功能**: 需要支援多個店別的選擇，並將選擇結果以逗號分隔的字串回傳
2. **篩選功能**: 支援多種篩選條件的組合查詢
3. **特殊查詢模式**: 
   - KIND=1: 僅查詢供應商相關店別
   - GOODS_ID=Y: 查詢商品相關店別（含最小庫存量）
   - ALL_SHOP=Y: 顯示「全部分店」按鈕
4. **回傳格式**: 需要確認父視窗接收選擇結果的格式要求
5. **效能優化**: 如果店別資料量大，需要實作分頁或虛擬滾動
6. **權限控制**: 需要確認是否有權限限制

---

## 六、測試計劃

### 6.1 單元測試
- [ ] 測試店別列表查詢功能
- [ ] 測試店別篩選功能（區域、店型態、店級別、店別名稱）
- [ ] 測試多選功能
- [ ] 測試特殊選項功能（全部分店）
- [ ] 測試回傳功能
- [ ] 測試供應商店別查詢（KIND=1）
- [ ] 測試商品相關店別查詢（GOODS_ID=Y）

### 6.2 整合測試
- [ ] 測試前後端整合
- [ ] 測試與父視窗的互動
- [ ] 測試錯誤處理
- [ ] 測試多種查詢模式

### 6.3 效能測試
- [ ] 測試大量資料的載入效能
- [ ] 測試搜尋功能的響應時間
- [ ] 測試多選功能的效能

---

## 七、相關文件

- [區域基本資料維護作業](../02-基本資料管理/02-地區設定/SYSB450-區域基本資料維護作業.md)
- [參數資料設定維護作業](../02-基本資料管理/01-參數設定/SYSBC40-參數資料設定維護作業.md)
- [多選區域列表](./MULTI_AREA_LIST-多選區域列表.md)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

