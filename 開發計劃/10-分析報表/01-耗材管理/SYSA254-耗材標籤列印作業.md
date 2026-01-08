# SYSA254 - 耗材標籤列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSA254
- **功能名稱**: 耗材標籤列印作業
- **功能描述**: 提供耗材標籤的列印功能，支援多種列印格式和條碼生成，包含耗材基本資訊、狀態、位置等資訊
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSA000/SYSA254.aspx` (列印頁面)
  - `WEB/IMS_CORE/SYSA000/js/SYSA254.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSA000/js/SYSA254_aspx.js` (列印邏輯)
  - `WEB/IMS_CORE/SYSA000/css/SYSA254_aspx.css` (樣式)

### 1.2 業務需求
- 支援耗材標籤列印
- 支援多種列印格式 (type=1: 耗材管理報表, type=2: 耗材標籤列印)
- 支援條碼生成 (使用jquery-barcode.js)
- 支援多種狀態篩選 (status參數)
- 支援多個店別篩選 (site_id參數)
- 支援資產狀態篩選 (asset_status參數)
- 支援列印預覽
- 支援批次列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Consumables` (耗材主檔)

```sql
CREATE TABLE [dbo].[Consumables] (
    [ConsumableId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 耗材編號
    [ConsumableName] NVARCHAR(200) NOT NULL, -- 耗材名稱
    [CategoryId] NVARCHAR(50) NULL, -- 分類代碼
    [Unit] NVARCHAR(20) NULL, -- 單位
    [Specification] NVARCHAR(200) NULL, -- 規格
    [Brand] NVARCHAR(100) NULL, -- 品牌
    [Model] NVARCHAR(100) NULL, -- 型號
    [BarCode] NVARCHAR(50) NULL, -- 條碼
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:正常, 2:停用)
    [AssetStatus] NVARCHAR(10) NULL, -- 資產狀態
    [SiteId] NVARCHAR(50) NULL, -- 店別代碼
    [Location] NVARCHAR(200) NULL, -- 位置
    [Quantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 數量
    [MinQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 最小庫存量
    [MaxQuantity] DECIMAL(18, 2) NULL DEFAULT 0, -- 最大庫存量
    [Price] DECIMAL(18, 2) NULL DEFAULT 0, -- 單價
    [SupplierId] NVARCHAR(50) NULL, -- 供應商代碼
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_Consumables] PRIMARY KEY CLUSTERED ([ConsumableId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Consumables_SiteId] ON [dbo].[Consumables] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Consumables_Status] ON [dbo].[Consumables] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Consumables_AssetStatus] ON [dbo].[Consumables] ([AssetStatus]);
CREATE NONCLUSTERED INDEX [IX_Consumables_BarCode] ON [dbo].[Consumables] ([BarCode]);
CREATE NONCLUSTERED INDEX [IX_Consumables_CategoryId] ON [dbo].[Consumables] ([CategoryId]);
```

### 2.2 相關資料表

#### 2.2.1 `ConsumableCategories` - 耗材分類
```sql
CREATE TABLE [dbo].[ConsumableCategories] (
    [CategoryId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [ParentCategoryId] NVARCHAR(50) NULL,
    [SeqNo] INT NULL DEFAULT 0,
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
    CONSTRAINT [FK_ConsumableCategories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[ConsumableCategories] ([CategoryId])
);
```

#### 2.2.2 `Sites` - 店別主檔
- 用於查詢店別列表
- 參考: 基本資料管理相關設計

#### 2.2.3 `ConsumablePrintLogs` - 耗材列印記錄
```sql
CREATE TABLE [dbo].[ConsumablePrintLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ConsumableId] NVARCHAR(50) NOT NULL,
    [PrintType] NVARCHAR(20) NULL, -- 1:耗材管理報表, 2:耗材標籤列印
    [PrintCount] INT NOT NULL DEFAULT 1,
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [PrintedBy] NVARCHAR(50) NULL,
    [SiteId] NVARCHAR(50) NULL,
    CONSTRAINT [FK_ConsumablePrintLogs_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ConsumableId | NVARCHAR | 50 | NO | - | 耗材編號 | 主鍵 |
| ConsumableName | NVARCHAR | 200 | NO | - | 耗材名稱 | - |
| CategoryId | NVARCHAR | 50 | YES | - | 分類代碼 | 外鍵至ConsumableCategories |
| Unit | NVARCHAR | 20 | YES | - | 單位 | - |
| Specification | NVARCHAR | 200 | YES | - | 規格 | - |
| Brand | NVARCHAR | 100 | YES | - | 品牌 | - |
| Model | NVARCHAR | 100 | YES | - | 型號 | - |
| BarCode | NVARCHAR | 50 | YES | - | 條碼 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:正常, 2:停用 |
| AssetStatus | NVARCHAR | 10 | YES | - | 資產狀態 | - |
| SiteId | NVARCHAR | 50 | YES | - | 店別代碼 | 外鍵至Sites |
| Location | NVARCHAR | 200 | YES | - | 位置 | - |
| Quantity | DECIMAL | 18,2 | YES | 0 | 數量 | - |
| MinQuantity | DECIMAL | 18,2 | YES | 0 | 最小庫存量 | - |
| MaxQuantity | DECIMAL | 18,2 | YES | 0 | 最大庫存量 | - |
| Price | DECIMAL | 18,2 | YES | 0 | 單價 | - |
| SupplierId | NVARCHAR | 50 | YES | - | 供應商代碼 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢耗材列表 (用於列印)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumables/print/list`
- **說明**: 查詢可列印的耗材列表，支援多條件篩選
- **請求參數**:
  ```json
  {
    "type": "2", // 1:耗材管理報表, 2:耗材標籤列印
    "status": "", // 狀態篩選
    "siteId": "", // 店別篩選
    "assetStatus": "", // 資產狀態篩選
    "consumableIds": [] // 指定耗材編號列表
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
          "consumableId": "C001",
          "consumableName": "耗材名稱",
          "barCode": "1234567890123",
          "categoryName": "分類名稱",
          "unit": "個",
          "specification": "規格",
          "brand": "品牌",
          "model": "型號",
          "status": "1",
          "assetStatus": "正常",
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "location": "位置",
          "quantity": 100.00,
          "price": 50.00
        }
      ],
      "totalCount": 100
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 批次列印耗材標籤
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/consumables/print/batch`
- **說明**: 批次列印耗材標籤，記錄列印記錄
- **請求格式**:
  ```json
  {
    "type": "2",
    "consumableIds": ["C001", "C002", "C003"],
    "printCount": 1,
    "siteId": "SITE001"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "printLogId": "guid",
      "printCount": 3
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢列印記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumables/print/logs`
- **說明**: 查詢耗材列印記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "consumableId": "",
    "siteId": "",
    "printedBy": ""
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ConsumablePrintController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/consumables/print")]
    [Authorize]
    public class ConsumablePrintController : ControllerBase
    {
        private readonly IConsumablePrintService _printService;
        
        public ConsumablePrintController(IConsumablePrintService printService)
        {
            _printService = printService;
        }
        
        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<ConsumablePrintListDto>>> GetPrintList([FromQuery] ConsumablePrintQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpPost("batch")]
        public async Task<ActionResult<ApiResponse<PrintLogDto>>> BatchPrint([FromBody] BatchPrintDto dto)
        {
            // 實作批次列印邏輯
        }
        
        [HttpGet("logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<PrintLogDto>>>> GetPrintLogs([FromQuery] PrintLogQueryDto query)
        {
            // 實作查詢列印記錄邏輯
        }
    }
}
```

#### 3.2.2 Service: `ConsumablePrintService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IConsumablePrintService
    {
        Task<ConsumablePrintListDto> GetPrintListAsync(ConsumablePrintQueryDto query);
        Task<PrintLogDto> BatchPrintAsync(BatchPrintDto dto, string userId);
        Task<PagedResult<PrintLogDto>> GetPrintLogsAsync(PrintLogQueryDto query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 耗材標籤列印頁面 (`ConsumableLabelPrint.vue`)
- **路徑**: `/analysis/consumables/label-print`
- **功能**: 顯示耗材標籤列印頁面，支援查詢、預覽、列印
- **主要元件**:
  - 查詢表單 (ConsumablePrintSearchForm)
  - 耗材列表 (ConsumablePrintList)
  - 列印預覽 (ConsumableLabelPreview)
  - 列印設定對話框 (PrintSettingsDialog)

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ConsumablePrintSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="列印類型">
      <el-select v-model="searchForm.type" placeholder="請選擇列印類型">
        <el-option label="耗材管理報表" value="1" />
        <el-option label="耗材標籤列印" value="2" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="正常" value="1" />
        <el-option label="停用" value="2" />
      </el-select>
    </el-form-item>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別">
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="資產狀態">
      <el-select v-model="searchForm.assetStatus" placeholder="請選擇資產狀態">
        <el-option label="全部" value="" />
        <el-option label="正常" value="正常" />
        <el-option label="維修中" value="維修中" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handlePrint" :disabled="selectedItems.length === 0">列印</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 耗材列表元件 (`ConsumablePrintList.vue`)
```vue
<template>
  <div>
    <el-table :data="consumableList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="consumableId" label="耗材編號" width="120" />
      <el-table-column prop="consumableName" label="耗材名稱" width="200" />
      <el-table-column prop="barCode" label="條碼" width="150" />
      <el-table-column prop="categoryName" label="分類" width="120" />
      <el-table-column prop="unit" label="單位" width="80" />
      <el-table-column prop="siteName" label="店別" width="120" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '正常' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
```

#### 4.2.3 列印預覽元件 (`ConsumableLabelPreview.vue`)
```vue
<template>
  <div class="label-preview">
    <div v-for="(item, index) in printItems" :key="index" class="label-box">
      <div class="label-header">
        <div class="label-title">{{ item.consumableName }}</div>
        <div class="label-barcode" :id="`barcode-${index}`"></div>
      </div>
      <div class="label-content">
        <div class="label-row">
          <span class="label-label">編號:</span>
          <span class="label-value">{{ item.consumableId }}</span>
        </div>
        <div class="label-row">
          <span class="label-label">分類:</span>
          <span class="label-value">{{ item.categoryName }}</span>
        </div>
        <div class="label-row">
          <span class="label-label">店別:</span>
          <span class="label-value">{{ item.siteName }}</span>
        </div>
        <div class="label-row">
          <span class="label-label">位置:</span>
          <span class="label-value">{{ item.location }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, watch } from 'vue';
import { generateBarcode } from '@/utils/barcode';

const props = defineProps<{
  printItems: ConsumablePrintItem[];
}>();

onMounted(() => {
  generateBarcodes();
});

watch(() => props.printItems, () => {
  generateBarcodes();
}, { deep: true });

const generateBarcodes = () => {
  props.printItems.forEach((item, index) => {
    if (item.barCode) {
      generateBarcode(`barcode-${index}`, item.barCode, {
        format: 'CODE128',
        width: 2,
        height: 50
      });
    }
  });
};
</script>

<style scoped>
.label-preview {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.label-box {
  width: 245px;
  height: 160px;
  border: 1px solid #ddd;
  padding: 10px;
  box-sizing: border-box;
}

.label-header {
  text-align: center;
  margin-bottom: 10px;
}

.label-title {
  font-size: 16px;
  font-weight: bold;
  margin-bottom: 10px;
}

.label-barcode {
  margin: 10px 0;
}

.label-content {
  font-size: 14px;
}

.label-row {
  display: flex;
  margin-bottom: 5px;
}

.label-label {
  width: 60px;
  font-weight: bold;
}

.label-value {
  flex: 1;
}
</style>
```

### 4.3 API 呼叫 (`consumable-print.api.ts`)
```typescript
import request from '@/utils/request';

export interface ConsumablePrintItem {
  consumableId: string;
  consumableName: string;
  barCode?: string;
  categoryName?: string;
  unit?: string;
  specification?: string;
  brand?: string;
  model?: string;
  status: string;
  assetStatus?: string;
  siteId?: string;
  siteName?: string;
  location?: string;
  quantity?: number;
  price?: number;
}

export interface ConsumablePrintQueryDto {
  type: string; // 1:耗材管理報表, 2:耗材標籤列印
  status?: string;
  siteId?: string;
  assetStatus?: string;
  consumableIds?: string[];
}

export interface BatchPrintDto {
  type: string;
  consumableIds: string[];
  printCount: number;
  siteId?: string;
}

// API 函數
export const getConsumablePrintList = (query: ConsumablePrintQueryDto) => {
  return request.get<ApiResponse<{ items: ConsumablePrintItem[]; totalCount: number }>>('/api/v1/consumables/print/list', { params: query });
};

export const batchPrintConsumables = (data: BatchPrintDto) => {
  return request.post<ApiResponse<{ printLogId: string; printCount: number }>>('/api/v1/consumables/print/batch', data);
};

export const getPrintLogs = (query: PrintLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<PrintLogDto>>>('/api/v1/consumables/print/logs', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立耗材主檔資料表
- [ ] 建立耗材分類資料表
- [ ] 建立列印記錄資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 條碼生成服務
- [ ] 列印記錄服務
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢表單開發
- [ ] 耗材列表開發
- [ ] 列印預覽開發
- [ ] 條碼生成元件
- [ ] 列印功能實作
- [ ] 列印設定對話框
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 列印功能測試
- [ ] 條碼生成測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 列印功能
- 支援瀏覽器列印功能
- 支援多種列印格式
- 列印前必須預覽
- 列印記錄必須保存

### 6.2 條碼生成
- 使用jquery-barcode.js或類似庫
- 支援多種條碼格式 (CODE128, EAN13等)
- 條碼必須清晰可讀
- 條碼尺寸必須符合標籤規格

### 6.3 效能
- 大量資料列印必須分批處理
- 列印預覽必須優化渲染效能
- 必須使用虛擬滾動處理大量標籤

### 6.4 業務邏輯
- 列印前必須檢查耗材狀態
- 列印記錄必須記錄操作者
- 列印記錄必須記錄列印時間和內容

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢耗材列表成功
- [ ] 批次列印成功
- [ ] 列印記錄查詢成功
- [ ] 條碼生成成功

### 7.2 整合測試
- [ ] 完整列印流程測試
- [ ] 多種列印格式測試
- [ ] 列印記錄保存測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料列印測試
- [ ] 列印預覽效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSA000/SYSA254.aspx`
- `WEB/IMS_CORE/SYSA000/js/SYSA254.js`
- `WEB/IMS_CORE/SYSA000/js/SYSA254_aspx.js`
- `WEB/IMS_CORE/SYSA000/css/SYSA254_aspx.css`

### 8.2 相關功能
- SYSA255 - 耗材管理報表
- SYSA297 - 耗材出售維護

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

