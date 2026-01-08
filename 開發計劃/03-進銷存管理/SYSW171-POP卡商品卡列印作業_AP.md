# SYSW171 - POP卡＆商品卡列印作業_AP 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW171
- **功能名稱**: POP卡＆商品卡列印作業_AP
- **功能描述**: 提供POP卡和商品卡的列印功能（AP版本），為SYSW170的AP變體版本，主要差異在於列印格式和設定選項
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP` (基礎列印功能)
  - `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx` (報表)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR1_AP.rpt` (AP版本報表格式)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR2_AP.rpt` (AP版本報表格式)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR3_AP.rpt` (AP版本報表格式)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR4_AP.rpt` (AP版本報表格式)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR5_AP.rpt` (AP版本報表格式)
  - `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR6_AP.rpt` (AP版本報表格式)

### 1.2 業務需求
- 支援POP卡列印（AP版本格式）
- 支援商品卡列印（AP版本格式）
- 支援多種條碼格式 (UPC, EAN, Code39等)
- 支援AP專屬列印格式 (PR1_AP-PR6_AP)
- 支援列印設定 (頁面大小、填補、除錯模式等)
- 支援品牌權限控制
- 支援Excel匯出
- AP版本特有的列印參數和格式設定

### 1.3 與SYSW170的差異
- 使用AP專屬的報表格式模板
- 列印參數設定可能有所不同
- 列印版面配置針對AP版本優化

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

本功能使用與SYSW170相同的資料表結構，但需要額外支援AP版本的列印設定：

#### 2.1.1 `Products` - 商品主檔
- 參考: `開發計劃/03-進銷存管理/SYSW170-POP卡商品卡列印作業.md`

#### 2.1.2 `PopPrintSettings` - POP列印設定（擴展）
```sql
CREATE TABLE [dbo].[PopPrintSettings] (
    [SettingId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ShopId] NVARCHAR(50) NULL,
    [Ip] NVARCHAR(50) NULL,
    [TypeId] NVARCHAR(50) NULL, -- 報表類型 (PR1_AP, PR2_AP等)
    [Version] NVARCHAR(20) NULL DEFAULT 'AP', -- 版本標記: AP, UA, STANDARD
    [DebugMode] BIT NOT NULL DEFAULT 0,
    [HeaderHeightPadding] INT NULL DEFAULT 0,
    [HeaderHeightPaddingRemain] INT NULL DEFAULT 851,
    [PageHeaderHeightPadding] INT NULL DEFAULT 0,
    [PagePadding] NVARCHAR(100) NULL, -- 左,右,上,下
    [PageSize] NVARCHAR(100) NULL, -- 高,寬
    [ApSpecificSettings] NVARCHAR(MAX) NULL, -- AP版本專屬設定 (JSON格式)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PopPrintSettings_ShopId_Version] ON [dbo].[PopPrintSettings] ([ShopId], [Version]);
```

#### 2.1.3 `PopPrintLogs` - POP列印記錄（擴展）
```sql
CREATE TABLE [dbo].[PopPrintLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [GoodsId] NVARCHAR(50) NOT NULL,
    [PrintType] NVARCHAR(20) NULL, -- POP, PRODUCT_CARD
    [PrintFormat] NVARCHAR(20) NULL, -- PR1_AP, PR2_AP, PR3_AP, PR4_AP, PR5_AP, PR6_AP
    [Version] NVARCHAR(20) NULL DEFAULT 'AP', -- 版本標記
    [PrintCount] INT NOT NULL DEFAULT 1,
    [PrintDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [PrintedBy] NVARCHAR(50) NULL,
    [ShopId] NVARCHAR(50) NULL,
    CONSTRAINT [FK_PopPrintLogs_Products] FOREIGN KEY ([GoodsId]) REFERENCES [dbo].[Products] ([GoodsId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_PrintDate] ON [dbo].[PopPrintLogs] ([PrintDate]);
CREATE NONCLUSTERED INDEX [IX_PopPrintLogs_Version] ON [dbo].[PopPrintLogs] ([Version]);
```

### 2.2 資料字典

#### PopPrintSettings 資料表（擴展欄位）
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Version | NVARCHAR | 20 | YES | 'AP' | 版本標記 | AP, UA, STANDARD |
| ApSpecificSettings | NVARCHAR(MAX) | - | YES | - | AP版本專屬設定 | JSON格式儲存 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢商品列表 (用於列印)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ap/products`
- **說明**: 查詢可列印的商品列表，支援多條件篩選（AP版本）
- **請求參數**: 同SYSW170
- **回應格式**: 同SYSW170

#### 3.1.2 取得列印資料 (AP版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ap/generate`
- **說明**: 根據選取的商品和AP列印格式產生列印資料
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002", "G003"],
    "printType": "POP",
    "printFormat": "PR1_AP", // PR1_AP, PR2_AP, PR3_AP, PR4_AP, PR5_AP, PR6_AP
    "shopId": "SHOP001",
    "version": "AP",
    "options": {
      "includeBarcode": true,
      "includePrice": true,
      "includeNote": false,
      "apSpecificOptions": {} // AP版本專屬選項
    }
  }
  ```

#### 3.1.3 列印POP卡 (AP版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ap/print`
- **說明**: 執行AP版本列印作業並記錄列印日誌
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002"],
    "printType": "POP",
    "printFormat": "PR1_AP",
    "printCount": 1,
    "shopId": "SHOP001",
    "version": "AP"
  }
  ```

#### 3.1.4 取得列印設定 (AP版本)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ap/settings/{shopId}`
- **說明**: 取得指定分店的AP版本列印設定
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
      "typeId": "PR1_AP",
      "version": "AP",
      "debugMode": false,
      "headerHeightPadding": 0,
      "headerHeightPaddingRemain": 851,
      "pageHeaderHeightPadding": 0,
      "pagePadding": "10,10,10,10",
      "pageSize": "297,210",
      "apSpecificSettings": {}
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 更新列印設定 (AP版本)
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pop-print-ap/settings/{shopId}`
- **說明**: 更新指定分店的AP版本列印設定
- **請求格式**: 同取得列印設定

#### 3.1.6 查詢列印記錄 (AP版本)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ap/logs`
- **說明**: 查詢AP版本列印記錄
- **請求參數**: 同SYSW170，但會自動過濾 `version = 'AP'`

#### 3.1.7 匯出Excel (AP版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ap/export-excel`
- **說明**: 匯出AP版本列印資料為Excel檔案
- **請求格式**: 同取得列印資料

### 3.2 後端實作類別

#### 3.2.1 Controller: `PopPrintApController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/pop-print-ap")]
    [Authorize]
    public class PopPrintApController : ControllerBase
    {
        private readonly IPopPrintService _popPrintService;
        
        public PopPrintApController(IPopPrintService popPrintService)
        {
            _popPrintService = popPrintService;
        }
        
        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts([FromQuery] PopPrintProductQueryDto query)
        {
            // 實作查詢邏輯（AP版本）
        }
        
        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<PopPrintDataDto>>> GeneratePrintData([FromBody] GeneratePrintDataDto dto)
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            // 實作產生列印資料邏輯（AP版本）
        }
        
        [HttpPost("print")]
        public async Task<ActionResult<ApiResponse<PrintJobDto>>> Print([FromBody] PrintRequestDto dto)
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            // 實作列印邏輯（AP版本）
        }
        
        [HttpGet("settings/{shopId}")]
        public async Task<ActionResult<ApiResponse<PopPrintSettingDto>>> GetSettings(string shopId)
        {
            // 實作取得AP版本設定邏輯
        }
        
        [HttpPut("settings/{shopId}")]
        public async Task<ActionResult<ApiResponse>> UpdateSettings(string shopId, [FromBody] UpdatePopPrintSettingDto dto)
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            // 實作更新AP版本設定邏輯
        }
        
        [HttpGet("logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<PopPrintLogDto>>>> GetLogs([FromQuery] PopPrintLogQueryDto query)
        {
            // 自動過濾AP版本記錄
            query.Version = "AP";
            // 實作查詢記錄邏輯
        }
        
        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportExcel([FromBody] GeneratePrintDataDto dto)
        {
            // 強制設定版本為AP
            dto.Version = "AP";
            // 實作Excel匯出邏輯（AP版本）
        }
    }
}
```

#### 3.2.2 Service: `PopPrintService.cs` (擴展)
- 在現有的 `IPopPrintService` 介面中擴展支援版本參數
- 根據版本參數選擇對應的報表模板和列印邏輯

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 POP列印查詢頁面 (AP版本) (`PopPrintApQuery.vue`)
- **路徑**: `/inventory/pop-print-ap/query`
- **功能**: 查詢商品並選擇AP版本列印格式
- **主要元件**:
  - 查詢表單 (PopPrintSearchForm)
  - 商品列表 (ProductList)
  - 列印設定對話框 (PrintSettingDialog) - AP版本專屬
  - 列印預覽 (PrintPreview) - AP版本格式

### 4.2 UI 元件設計

#### 4.2.1 列印設定對話框 (AP版本) (`PrintSettingApDialog.vue`)
```vue
<template>
  <el-dialog
    title="列印設定 (AP版本)"
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
      <el-form-item label="列印格式 (AP)" prop="printFormat">
        <el-select v-model="form.printFormat" placeholder="請選擇列印格式">
          <el-option label="PR1_AP" value="PR1_AP" />
          <el-option label="PR2_AP" value="PR2_AP" />
          <el-option label="PR3_AP" value="PR3_AP" />
          <el-option label="PR4_AP" value="PR4_AP" />
          <el-option label="PR5_AP" value="PR5_AP" />
          <el-option label="PR6_AP" value="PR6_AP" />
        </el-select>
      </el-form-item>
      <el-form-item label="列印數量" prop="printCount">
        <el-input-number v-model="form.printCount" :min="1" :max="100" />
      </el-form-item>
      <!-- AP版本專屬設定 -->
      <el-form-item label="AP專屬選項">
        <el-checkbox v-model="form.apSpecificOptions.option1">選項1</el-checkbox>
        <el-checkbox v-model="form.apSpecificOptions.option2">選項2</el-checkbox>
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

### 4.3 API 呼叫 (`popPrintAp.api.ts`)
```typescript
import request from '@/utils/request';

// 使用與SYSW170相同的DTO，但API路徑不同
export const getPopPrintApProducts = (query: PopPrintProductQueryDto) => {
  return request.get<ApiResponse<PagedResult<ProductDto>>>('/api/v1/pop-print-ap/products', { params: query });
};

export const generatePrintApData = (data: GeneratePrintDataDto) => {
  // 自動設定版本為AP
  data.version = 'AP';
  return request.post<ApiResponse<PopPrintDataDto>>('/api/v1/pop-print-ap/generate', data);
};

export const printPopAp = (data: PrintRequestDto) => {
  // 自動設定版本為AP
  data.version = 'AP';
  return request.post<ApiResponse<PrintJobDto>>('/api/v1/pop-print-ap/print', data);
};

export const getPopPrintApSettings = (shopId: string) => {
  return request.get<ApiResponse<PopPrintSettingDto>>(`/api/v1/pop-print-ap/settings/${shopId}`);
};

export const updatePopPrintApSettings = (shopId: string, data: UpdatePopPrintSettingDto) => {
  // 自動設定版本為AP
  data.version = 'AP';
  return request.put<ApiResponse>(`/api/v1/pop-print-ap/settings/${shopId}`, data);
};

export const getPopPrintApLogs = (query: PopPrintLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<PopPrintLogDto>>>('/api/v1/pop-print-ap/logs', { params: query });
};

export const exportPopPrintApExcel = (data: GeneratePrintDataDto) => {
  // 自動設定版本為AP
  data.version = 'AP';
  return request.post('/api/v1/pop-print-ap/export-excel', data, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 擴展現有資料表結構（新增Version欄位）
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] 擴展現有Service支援版本參數
- [ ] 建立AP版本Controller
- [ ] 實作AP版本專屬列印邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] 建立AP版本API呼叫函數
- [ ] 建立AP版本查詢頁面
- [ ] 建立AP版本列印設定對話框
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 列印功能測試（AP版本）
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6天

---

## 六、注意事項

### 6.1 版本區分
- 必須明確標記版本為AP
- 列印記錄必須記錄版本資訊
- 列印設定必須支援版本區分

### 6.2 報表模板
- 使用AP專屬的報表模板
- 模板路徑: `/templates/pop-print/ap/PR{1-6}_AP.rpt`

### 6.3 向後相容
- 必須與SYSW170共用核心邏輯
- 差異僅在於報表模板和部分設定選項

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢商品列表成功（AP版本）
- [ ] 產生列印資料成功（AP版本）
- [ ] 列印成功（AP版本）
- [ ] 取得列印設定成功（AP版本）
- [ ] 更新列印設定成功（AP版本）

### 7.2 整合測試
- [ ] 完整列印流程測試（AP版本）
- [ ] 版本區分測試
- [ ] Excel匯出測試（AP版本）

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP`
- `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx`
- `WEB/IMS_CORE/Script/OCX/POP/SYSW170_PR1_AP.rpt` 至 `SYSW170_PR6_AP.rpt`

### 8.2 相關開發計劃
- `開發計劃/03-進銷存管理/SYSW170-POP卡商品卡列印作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

