# SYSW170 - POP卡＆商品卡列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW170
- **功能名稱**: POP卡＆商品卡列印作業
- **功能描述**: 提供POP卡和商品卡的列印功能，支援多種列印格式和條碼生成
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW170_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW170_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP` (報表列印)
  - `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW170_POP.xml` (POP設定)

### 1.2 業務需求
- 支援POP卡列印
- 支援商品卡列印
- 支援多種條碼格式 (UPC, EAN, Code39等)
- 支援多種列印格式 (PR1-PR6, AP版本等)
- 支援列印設定 (頁面大小、填補、除錯模式等)
- 支援品牌權限控制
- 支援Excel匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Products` (商品主檔)
```sql
CREATE TABLE [dbo].[Products] (
    [GoodsId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [GoodsName] NVARCHAR(200) NOT NULL,
    [BarCode] NVARCHAR(50) NULL,
    [VendorGoodsId] NVARCHAR(50) NULL,
    [LogoId] NVARCHAR(50) NULL,
    [BClassId] NVARCHAR(50) NULL,
    [MClassId] NVARCHAR(50) NULL,
    [SClassId] NVARCHAR(50) NULL,
    [Unit] NVARCHAR(20) NULL,
    [UnitName] NVARCHAR(50) NULL,
    [MinQty] DECIMAL(18, 2) NULL,
    [Mprc] DECIMAL(18, 2) NULL,
    [Price] DECIMAL(18, 2) NULL,
    [PriceUnit] NVARCHAR(20) NULL,
    [StartDate] DATETIME2 NULL,
    [EndDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([GoodsId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Products_BarCode] ON [dbo].[Products] ([BarCode]);
CREATE NONCLUSTERED INDEX [IX_Products_LogoId] ON [dbo].[Products] ([LogoId]);
CREATE NONCLUSTERED INDEX [IX_Products_BClassId] ON [dbo].[Products] ([BClassId]);
CREATE NONCLUSTERED INDEX [IX_Products_MClassId] ON [dbo].[Products] ([MClassId]);
CREATE NONCLUSTERED INDEX [IX_Products_SClassId] ON [dbo].[Products] ([SClassId]);
```

### 2.2 相關資料表

#### 2.2.1 `ProductBarcodes` - 商品條碼
```sql
CREATE TABLE [dbo].[ProductBarcodes] (
    [BarcodeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [GoodsId] NVARCHAR(50) NOT NULL,
    [BarcodeType] NVARCHAR(20) NULL, -- UPC, EAN, Code39等
    [BarcodeText] NVARCHAR(100) NULL,
    [BarcodeUpc] NVARCHAR(50) NULL,
    [Barcode39] NVARCHAR(100) NULL,
    [IsDefault] BIT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ProductBarcodes_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId]) ON DELETE CASCADE
);
```

#### 2.2.2 `PopPrintSettings` - POP列印設定
```sql
CREATE TABLE [dbo].[PopPrintSettings] (
    [SettingId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ShopId] NVARCHAR(50) NULL,
    [Ip] NVARCHAR(50) NULL,
    [TypeId] NVARCHAR(50) NULL, -- 報表類型
    [DebugMode] BIT NOT NULL DEFAULT 0,
    [HeaderHeightPadding] INT NULL DEFAULT 0,
    [HeaderHeightPaddingRemain] INT NULL DEFAULT 851,
    [PageHeaderHeightPadding] INT NULL DEFAULT 0,
    [PagePadding] NVARCHAR(100) NULL, -- 左,右,上,下
    [PageSize] NVARCHAR(100) NULL, -- 高,寬
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

#### 2.2.3 `PopPrintLogs` - POP列印記錄
```sql
CREATE TABLE [dbo].[PopPrintLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [GoodsId] NVARCHAR(50) NOT NULL,
    [PrintType] NVARCHAR(20) NULL, -- POP, PRODUCT_CARD
    [PrintFormat] NVARCHAR(20) NULL, -- PR1, PR2, PR3, PR4, PR5, PR6, AP等
    [PrintCount] INT NOT NULL DEFAULT 1,
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [PrintedBy] NVARCHAR(50) NULL,
    [ShopId] NVARCHAR(50) NULL,
    CONSTRAINT [FK_PopPrintLogs_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId])
);
```

### 2.3 資料字典

#### Products 資料表
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| GoodsId | NVARCHAR | 50 | NO | - | 商品編號 | 主鍵 |
| GoodsName | NVARCHAR | 200 | NO | - | 商品名稱 | - |
| BarCode | NVARCHAR | 50 | YES | - | 國際條碼 | - |
| VendorGoodsId | NVARCHAR | 50 | YES | - | 供應商商品編號 | - |
| LogoId | NVARCHAR | 50 | YES | - | 品牌代碼 | - |
| BClassId | NVARCHAR | 50 | YES | - | 大分類代號 | - |
| MClassId | NVARCHAR | 50 | YES | - | 中分類代碼 | - |
| SClassId | NVARCHAR | 50 | YES | - | 小分類代碼 | - |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| UnitName | NVARCHAR | 50 | YES | - | 單位名稱 | - |
| MinQty | DECIMAL | 18,2 | YES | - | 最小數量 | - |
| Mprc | DECIMAL | 18,2 | YES | - | 進價 | - |
| Price | DECIMAL | 18,2 | YES | - | 售價 | - |
| PriceUnit | NVARCHAR | 20 | YES | - | 價格單位 | - |
| StartDate | DATETIME2 | - | YES | - | 有效起始日 | - |
| EndDate | DATETIME2 | - | YES | - | 有效終止日 | - |
| Status | NVARCHAR | 10 | YES | - | 狀態 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢商品列表 (用於列印)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print/products`
- **說明**: 查詢可列印的商品列表，支援多條件篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "goodsId": "",
      "goodsName": "",
      "barCode": "",
      "vendorGoodsId": "",
      "logoId": "",
      "bClassId": "",
      "mClassId": "",
      "sClassId": ""
    },
    "displayColumns": ["goodsId", "goodsName", "barCode", "price"]
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
          "goodsId": "G001",
          "goodsName": "商品名稱",
          "barCode": "1234567890123",
          "vendorGoodsId": "VG001",
          "logoId": "L001",
          "price": 100.00,
          "mprc": 80.00,
          "unit": "PCS",
          "unitName": "個"
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

#### 3.1.2 取得列印資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print/generate`
- **說明**: 根據選取的商品和列印格式產生列印資料
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002", "G003"],
    "printType": "POP", // POP, PRODUCT_CARD
    "printFormat": "PR1", // PR1, PR2, PR3, PR4, PR5, PR6, AP等
    "shopId": "SHOP001",
    "options": {
      "includeBarcode": true,
      "includePrice": true,
      "includeNote": false
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "產生成功",
    "data": {
      "printData": [
        {
          "goodsId": "G001",
          "goodsName": "商品名稱",
          "barCode": "1234567890123",
          "barCodeText": "1234567890123",
          "price": 100.00,
          "unit": "PCS",
          "printFormat": "PR1"
        }
      ],
      "totalCount": 3
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 列印POP卡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print/print`
- **說明**: 執行列印作業並記錄列印日誌
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002"],
    "printType": "POP",
    "printFormat": "PR1",
    "printCount": 1,
    "shopId": "SHOP001"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "printJobId": "PJ001",
      "printedCount": 2
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 取得列印設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print/settings/{shopId}`
- **說明**: 取得指定分店的列印設定
- **路徑參數**:
  - `shopId`: 分店編號
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "shopId": "SHOP001",
      "ip": "192.168.1.100",
      "typeId": "PR1",
      "debugMode": false,
      "headerHeightPadding": 0,
      "headerHeightPaddingRemain": 851,
      "pageHeaderHeightPadding": 0,
      "pagePadding": "10,10,10,10",
      "pageSize": "297,210"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 更新列印設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pop-print/settings/{shopId}`
- **說明**: 更新指定分店的列印設定
- **請求格式**:
  ```json
  {
    "ip": "192.168.1.100",
    "typeId": "PR1",
    "debugMode": false,
    "headerHeightPadding": 0,
    "headerHeightPaddingRemain": 851,
    "pageHeaderHeightPadding": 0,
    "pagePadding": "10,10,10,10",
    "pageSize": "297,210"
  }
  ```

#### 3.1.6 查詢列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print/logs`
- **說明**: 查詢列印記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "filters": {
      "goodsId": "",
      "printType": "",
      "printFormat": "",
      "shopId": "",
      "printDateFrom": "",
      "printDateTo": ""
    }
  }
  ```

#### 3.1.7 匯出Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print/export-excel`
- **說明**: 匯出列印資料為Excel檔案
- **請求格式**: 同取得列印資料
- **回應**: Excel檔案 (application/vnd.openxmlformats-officedocument.spreadsheetml.sheet)

### 3.2 後端實作類別

#### 3.2.1 Controller: `PopPrintController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/pop-print")]
    [Authorize]
    public class PopPrintController : ControllerBase
    {
        private readonly IPopPrintService _popPrintService;
        
        public PopPrintController(IPopPrintService popPrintService)
        {
            _popPrintService = popPrintService;
        }
        
        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts([FromQuery] PopPrintProductQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<PopPrintDataDto>>> GeneratePrintData([FromBody] GeneratePrintDataDto dto)
        {
            // 實作產生列印資料邏輯
        }
        
        [HttpPost("print")]
        public async Task<ActionResult<ApiResponse<PrintJobDto>>> Print([FromBody] PrintRequestDto dto)
        {
            // 實作列印邏輯
        }
        
        [HttpGet("settings/{shopId}")]
        public async Task<ActionResult<ApiResponse<PopPrintSettingDto>>> GetSettings(string shopId)
        {
            // 實作取得設定邏輯
        }
        
        [HttpPut("settings/{shopId}")]
        public async Task<ActionResult<ApiResponse>> UpdateSettings(string shopId, [FromBody] UpdatePopPrintSettingDto dto)
        {
            // 實作更新設定邏輯
        }
        
        [HttpGet("logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<PopPrintLogDto>>>> GetLogs([FromQuery] PopPrintLogQueryDto query)
        {
            // 實作查詢記錄邏輯
        }
        
        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportExcel([FromBody] GeneratePrintDataDto dto)
        {
            // 實作Excel匯出邏輯
        }
    }
}
```

#### 3.2.2 Service: `PopPrintService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPopPrintService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(PopPrintProductQueryDto query);
        Task<PopPrintDataDto> GeneratePrintDataAsync(GeneratePrintDataDto dto);
        Task<PrintJobDto> PrintAsync(PrintRequestDto dto);
        Task<PopPrintSettingDto> GetSettingsAsync(string shopId);
        Task UpdateSettingsAsync(string shopId, UpdatePopPrintSettingDto dto);
        Task<PagedResult<PopPrintLogDto>> GetLogsAsync(PopPrintLogQueryDto query);
        Task<byte[]> ExportExcelAsync(GeneratePrintDataDto dto);
    }
}
```

#### 3.2.3 Repository: `PopPrintRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IPopPrintRepository
    {
        Task<PagedResult<Product>> GetProductsAsync(PopPrintProductQuery query);
        Task<Product> GetProductByIdAsync(string goodsId);
        Task<PopPrintSetting> GetSettingAsync(string shopId);
        Task<PopPrintSetting> CreateOrUpdateSettingAsync(PopPrintSetting setting);
        Task<PopPrintLog> CreateLogAsync(PopPrintLog log);
        Task<PagedResult<PopPrintLog>> GetLogsAsync(PopPrintLogQuery query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 POP列印查詢頁面 (`PopPrintQuery.vue`)
- **路徑**: `/inventory/pop-print/query`
- **功能**: 查詢商品並選擇列印格式
- **主要元件**:
  - 查詢表單 (PopPrintSearchForm)
  - 商品列表 (ProductList)
  - 列印設定對話框 (PrintSettingDialog)
  - 列印預覽 (PrintPreview)

#### 4.1.2 POP列印設定頁面 (`PopPrintSettings.vue`)
- **路徑**: `/inventory/pop-print/settings`
- **功能**: 管理列印設定

#### 4.1.3 POP列印記錄頁面 (`PopPrintLogs.vue`)
- **路徑**: `/inventory/pop-print/logs`
- **功能**: 查詢列印記錄

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`PopPrintSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="商品編號">
      <el-input v-model="searchForm.goodsId" placeholder="請輸入商品編號" />
    </el-form-item>
    <el-form-item label="商品名稱">
      <el-input v-model="searchForm.goodsName" placeholder="請輸入商品名稱" />
    </el-form-item>
    <el-form-item label="條碼">
      <el-input v-model="searchForm.barCode" placeholder="請輸入條碼" />
    </el-form-item>
    <el-form-item label="品牌">
      <el-select v-model="searchForm.logoId" placeholder="請選擇品牌">
        <el-option v-for="logo in logoList" :key="logo.logoId" :label="logo.logoName" :value="logo.logoId" />
      </el-select>
    </el-form-item>
    <el-form-item label="大分類">
      <el-select v-model="searchForm.bClassId" placeholder="請選擇大分類">
        <el-option v-for="bClass in bClassList" :key="bClass.bClassId" :label="bClass.bClassName" :value="bClass.bClassId" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 商品列表元件 (`ProductList.vue`)
```vue
<template>
  <div>
    <el-table :data="productList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="goodsId" label="商品編號" width="120" />
      <el-table-column prop="goodsName" label="商品名稱" width="200" />
      <el-table-column prop="barCode" label="條碼" width="150" />
      <el-table-column prop="price" label="售價" width="100">
        <template #default="{ row }">
          {{ formatCurrency(row.price) }}
        </template>
      </el-table-column>
      <el-table-column prop="unit" label="單位" width="80" />
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
    <div class="action-buttons">
      <el-button type="primary" @click="handlePrint" :disabled="selectedProducts.length === 0">列印</el-button>
      <el-button type="success" @click="handleExportExcel" :disabled="selectedProducts.length === 0">匯出Excel</el-button>
      <el-button @click="handleSettings">列印設定</el-button>
    </div>
  </div>
</template>
```

#### 4.2.3 列印設定對話框 (`PrintSettingDialog.vue`)
```vue
<template>
  <el-dialog
    title="列印設定"
    v-model="visible"
    width="600px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="150px">
      <el-form-item label="列印類型" prop="printType">
        <el-radio-group v-model="form.printType">
          <el-radio label="POP">POP卡</el-radio>
          <el-radio label="PRODUCT_CARD">商品卡</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="列印格式" prop="printFormat">
        <el-select v-model="form.printFormat" placeholder="請選擇列印格式">
          <el-option label="PR1" value="PR1" />
          <el-option label="PR2" value="PR2" />
          <el-option label="PR3" value="PR3" />
          <el-option label="PR4" value="PR4" />
          <el-option label="PR5" value="PR5" />
          <el-option label="PR6" value="PR6" />
          <el-option label="AP" value="AP" />
        </el-select>
      </el-form-item>
      <el-form-item label="列印數量" prop="printCount">
        <el-input-number v-model="form.printCount" :min="1" :max="100" />
      </el-form-item>
      <el-form-item label="包含條碼">
        <el-switch v-model="form.options.includeBarcode" />
      </el-form-item>
      <el-form-item label="包含價格">
        <el-switch v-model="form.options.includePrice" />
      </el-form-item>
      <el-form-item label="包含備註">
        <el-switch v-model="form.options.includeNote" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handlePreview">預覽</el-button>
      <el-button type="success" @click="handlePrint">列印</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`popPrint.api.ts`)
```typescript
import request from '@/utils/request';

export interface ProductDto {
  goodsId: string;
  goodsName: string;
  barCode?: string;
  vendorGoodsId?: string;
  logoId?: string;
  price?: number;
  mprc?: number;
  unit?: string;
  unitName?: string;
}

export interface PopPrintProductQueryDto {
  pageIndex: number;
  pageSize: number;
  filters?: {
    goodsId?: string;
    goodsName?: string;
    barCode?: string;
    vendorGoodsId?: string;
    logoId?: string;
    bClassId?: string;
    mClassId?: string;
    sClassId?: string;
  };
  displayColumns?: string[];
}

export interface GeneratePrintDataDto {
  goodsIds: string[];
  printType: 'POP' | 'PRODUCT_CARD';
  printFormat: string;
  shopId?: string;
  options?: {
    includeBarcode?: boolean;
    includePrice?: boolean;
    includeNote?: boolean;
  };
}

export interface PrintRequestDto {
  goodsIds: string[];
  printType: 'POP' | 'PRODUCT_CARD';
  printFormat: string;
  printCount: number;
  shopId?: string;
}

// API 函數
export const getPopPrintProducts = (query: PopPrintProductQueryDto) => {
  return request.get<ApiResponse<PagedResult<ProductDto>>>('/api/v1/pop-print/products', { params: query });
};

export const generatePrintData = (data: GeneratePrintDataDto) => {
  return request.post<ApiResponse<PopPrintDataDto>>('/api/v1/pop-print/generate', data);
};

export const printPop = (data: PrintRequestDto) => {
  return request.post<ApiResponse<PrintJobDto>>('/api/v1/pop-print/print', data);
};

export const getPopPrintSettings = (shopId: string) => {
  return request.get<ApiResponse<PopPrintSettingDto>>(`/api/v1/pop-print/settings/${shopId}`);
};

export const updatePopPrintSettings = (shopId: string, data: UpdatePopPrintSettingDto) => {
  return request.put<ApiResponse>(`/api/v1/pop-print/settings/${shopId}`, data);
};

export const getPopPrintLogs = (query: PopPrintLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<PopPrintLogDto>>>('/api/v1/pop-print/logs', { params: query });
};

export const exportPopPrintExcel = (data: GeneratePrintDataDto) => {
  return request.post('/api/v1/pop-print/export-excel', data, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作 (包含條碼生成邏輯)
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 列印資料產生邏輯
- [ ] Excel匯出功能
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 商品列表元件開發
- [ ] 列印設定對話框開發
- [ ] 列印預覽功能
- [ ] 列印記錄查詢頁面
- [ ] 列印設定管理頁面
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 列印功能測試
- [ ] 條碼生成測試
- [ ] Excel匯出測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 條碼生成
- 支援多種條碼格式 (UPC, EAN, Code39等)
- 條碼必須符合國際標準
- 條碼圖片生成使用第三方套件 (如: BarcodeLib)

### 6.2 列印功能
- 支援多種列印格式
- 列印設定必須可配置
- 列印記錄必須完整
- 支援列印預覽

### 6.3 品牌權限
- 必須根據使用者品牌權限過濾商品
- 必須檢查使用者是否有列印權限

### 6.4 效能
- 大量商品列印必須使用批次處理
- 列印資料產生必須優化
- 必須使用快取機制

### 6.5 Excel匯出
- 必須支援大量資料匯出
- 必須使用串流處理
- 必須支援多種格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢商品列表成功
- [ ] 產生列印資料成功
- [ ] 列印成功
- [ ] 取得列印設定成功
- [ ] 更新列印設定成功
- [ ] 條碼生成正確

### 7.2 整合測試
- [ ] 完整列印流程測試
- [ ] 品牌權限檢查測試
- [ ] Excel匯出測試
- [ ] 列印記錄測試

### 7.3 效能測試
- [ ] 大量商品列印測試
- [ ] 並發列印測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW170_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW170_FB.ASP`
- `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP`
- `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx`
- `WEB/IMS_CORE/SYSW000/SYSW170_POP.xml`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSW000/SYSW170_PR.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

