# SYSL135 - 業務報表查詢作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL135
- **功能名稱**: 業務報表查詢作業
- **功能描述**: 提供業務報表資料的查詢功能，支援依店別、卡別、廠商等條件查詢業務報表資訊，包含卡片類型、動作類型、廠商資訊等，支援報表列印與匯出
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL135.ascx` (查詢頁面)
  - `WEB/IMS_CORE/SYSL000/js/SYSL135.js` (前端邏輯)

### 1.2 業務需求
- 支援依店別查詢
- 支援依卡別查詢
- 支援依廠商查詢
- 支援依專櫃查詢（卡別為2004時）
- 支援報表列印與匯出
- 顯示業務報表資訊（卡片類型、動作類型、廠商資訊等）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `BusinessReports` - 業務報表主檔
```sql
CREATE TABLE [dbo].[BusinessReports] (
    [ReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
    [CardType] NVARCHAR(20) NOT NULL, -- 卡片類型
    [VendorId] NVARCHAR(50) NULL, -- 廠商代碼
    [StoreId] NVARCHAR(50) NULL, -- 專櫃代碼
    [AgreementId] NVARCHAR(50) NULL, -- 合約代碼
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼
    [ActionType] NVARCHAR(20) NULL, -- 動作類型
    [ReportDate] DATETIME2 NOT NULL, -- 報表日期
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_BusinessReports_Sites] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[Sites] ([SiteId]),
    CONSTRAINT [FK_BusinessReports_Vendors] FOREIGN KEY ([VendorId]) REFERENCES [dbo].[Vendors] ([VendorId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BusinessReports_SiteId] ON [dbo].[BusinessReports] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReports_CardType] ON [dbo].[BusinessReports] ([CardType]);
CREATE NONCLUSTERED INDEX [IX_BusinessReports_VendorId] ON [dbo].[BusinessReports] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReports_ReportDate] ON [dbo].[BusinessReports] ([ReportDate]);
```

#### 2.1.2 相關資料表
- `Sites` - 店別主檔
- `Vendors` - 廠商主檔
- `CardType` - 卡片類型主檔
- `Organizations` - 組織主檔

### 2.2 報表資料結構

#### 2.2.1 報表資料集結構
```sql
-- 報表查詢結果結構
SELECT 
    SITE_ID,
    SITE_NAME,
    CARD_TYPE,
    CARD_TYPE_NAME,
    VENDOR_ID,
    VENDOR_NAME,
    STORE_ID,
    STORE_NAME,
    AGREEMENT_ID,
    ORG_ID,
    ORG_NAME,
    ACTION_TYPE,
    REPORT_DATE
FROM 
    -- 查詢邏輯（需根據實際業務邏輯實作）
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢業務報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/sysl135`
- **說明**: 查詢業務報表資料
- **請求參數**:
  ```json
  {
    "siteId": "",
    "cardType": "",
    "vendorId": "",
    "storeId": "",
    "orgId": "",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
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
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "cardType": "2001",
          "cardTypeName": "卡片類型名稱",
          "vendorId": "V001",
          "vendorName": "廠商名稱",
          "storeId": "ST001",
          "storeName": "專櫃名稱",
          "agreementId": "AGR001",
          "orgId": "ORG001",
          "orgName": "組織名稱",
          "actionType": "ACTION001",
          "reportDate": "2024-01-01"
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

#### 3.1.2 匯出業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/sysl135/export`
- **說明**: 匯出業務報表（Excel/PDF）
- **請求格式**: 同查詢參數
- **回應格式**: 檔案下載

#### 3.1.3 列印業務報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/sysl135/print`
- **說明**: 列印業務報表
- **請求格式**: 同查詢參數
- **回應格式**: PDF 檔案

### 3.2 後端實作類別

#### 3.2.1 Controller: `BusinessReportsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/business-reports")]
    [Authorize]
    public class BusinessReportsController : ControllerBase
    {
        private readonly IBusinessReportService _businessReportService;
        
        public BusinessReportsController(IBusinessReportService businessReportService)
        {
            _businessReportService = businessReportService;
        }
        
        [HttpGet("sysl135")]
        public async Task<ActionResult<ApiResponse<PagedResult<SYSL135ReportDto>>>> GetSYSL135Report([FromQuery] SYSL135QueryDto query)
        {
            var result = await _businessReportService.GetSYSL135ReportAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("sysl135/export")]
        public async Task<IActionResult> ExportSYSL135Report([FromBody] SYSL135QueryDto query, [FromQuery] string format = "excel")
        {
            var fileBytes = await _businessReportService.ExportSYSL135ReportAsync(query, format);
            var fileName = $"業務報表查詢_{DateTime.Now:yyyyMMddHHmmss}.{(format == "excel" ? "xlsx" : "pdf")}";
            return File(fileBytes, format == "excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf", fileName);
        }
        
        [HttpPost("sysl135/print")]
        public async Task<IActionResult> PrintSYSL135Report([FromBody] SYSL135QueryDto query)
        {
            var fileBytes = await _businessReportService.PrintSYSL135ReportAsync(query);
            return File(fileBytes, "application/pdf", $"業務報表查詢_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }
    }
}
```

#### 3.2.2 Service: `BusinessReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IBusinessReportService
    {
        Task<PagedResult<SYSL135ReportDto>> GetSYSL135ReportAsync(SYSL135QueryDto query);
        Task<byte[]> ExportSYSL135ReportAsync(SYSL135QueryDto query, string format);
        Task<byte[]> PrintSYSL135ReportAsync(SYSL135QueryDto query);
    }
}
```

#### 3.2.3 DTO: `SYSL135ReportDto.cs`
```csharp
namespace RSL.IMS3.Application.DTOs
{
    public class SYSL135ReportDto
    {
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string CardType { get; set; }
        public string CardTypeName { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string AgreementId { get; set; }
        public string OrgId { get; set; }
        public string OrgName { get; set; }
        public string ActionType { get; set; }
        public DateTime ReportDate { get; set; }
    }
    
    public class SYSL135QueryDto
    {
        public string SiteId { get; set; }
        public string CardType { get; set; }
        public string VendorId { get; set; }
        public string StoreId { get; set; }
        public string OrgId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 業務報表查詢頁面 (`SYSL135Report.vue`)
- **路徑**: `/reports/business/sysl135`
- **功能**: 顯示業務報表查詢表單與結果
- **主要元件**:
  - 查詢表單 (SYSL135SearchForm)
  - 報表結果表格 (SYSL135ReportTable)
  - 報表列印/匯出按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`SYSL135SearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別" clearable>
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="卡別">
      <el-select v-model="searchForm.cardType" placeholder="請選擇卡別" clearable @change="handleCardTypeChange">
        <el-option v-for="card in cardTypeList" :key="card.cardId" :label="card.cardName" :value="card.cardId" />
      </el-select>
    </el-form-item>
    <el-form-item label="廠商" v-if="searchForm.cardType !== '2004'">
      <el-input v-model="searchForm.vendorId" placeholder="請輸入廠商代碼" />
    </el-form-item>
    <el-form-item label="專櫃" v-if="searchForm.cardType === '2004'">
      <el-input v-model="searchForm.storeId" placeholder="請輸入專櫃代碼" />
    </el-form-item>
    <el-form-item label="組織">
      <el-select v-model="searchForm.orgId" placeholder="請選擇組織" clearable>
        <el-option v-for="org in orgList" :key="org.orgId" :label="org.orgName" :value="org.orgId" />
      </el-select>
    </el-form-item>
    <el-form-item label="日期範圍">
      <el-date-picker
        v-model="dateRange"
        type="daterange"
        range-separator="至"
        start-placeholder="開始日期"
        end-placeholder="結束日期"
        format="YYYY-MM-DD"
        value-format="YYYY-MM-DD"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport">匯出</el-button>
      <el-button type="warning" @click="handlePrint">列印</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import { getSYSL135Report, exportSYSL135Report, printSYSL135Report } from '@/api/business-report.api';

const searchForm = reactive({
  siteId: '',
  cardType: '',
  vendorId: '',
  storeId: '',
  orgId: ''
});

const dateRange = ref<[string, string]>(['', '']);

const handleCardTypeChange = (value: string) => {
  if (value === '2004') {
    // 專櫃模式
    searchForm.vendorId = '';
  } else {
    // 廠商模式
    searchForm.storeId = '';
  }
};

watch(dateRange, (newVal) => {
  if (newVal && newVal.length === 2) {
    searchForm.startDate = newVal[0];
    searchForm.endDate = newVal[1];
  }
});

const handleSearch = () => {
  // 查詢邏輯
};

const handleReset = () => {
  Object.assign(searchForm, {
    siteId: '',
    cardType: '',
    vendorId: '',
    storeId: '',
    orgId: ''
  });
  dateRange.value = ['', ''];
};

const handleExport = async () => {
  // 匯出邏輯
};

const handlePrint = async () => {
  // 列印邏輯
};
</script>
```

---

## 五、開發任務清單

### 5.1 資料庫設計 (0.5天)
- [ ] 建立 BusinessReports 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立報表查詢視圖或預存程序

### 5.2 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表匯出功能（Excel/PDF）
- [ ] 報表列印功能
- [ ] 單元測試

### 5.3 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 查詢表單開發
- [ ] 報表結果表格開發
- [ ] 報表匯出功能
- [ ] 報表列印功能
- [ ] 元件測試

### 5.4 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 報表查詢測試
- [ ] 報表匯出測試
- [ ] 報表列印測試

**總計**: 5天

---

## 六、注意事項

### 6.1 業務邏輯
- 卡別為2004時，應顯示專櫃欄位，隱藏廠商欄位
- 卡別為2004時，申請單位應從專櫃合約來，且為唯讀
- 廠商/專櫃編號輸入時應驗證是否存在
- 廠商/專櫃編號不應包含中文字

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 報表查詢結果應使用快取機制

### 6.3 報表功能
- 支援 Excel 匯出
- 支援 PDF 列印
- 報表格式應符合舊系統格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢業務報表成功
- [ ] 依店別查詢成功
- [ ] 依卡別查詢成功
- [ ] 依廠商查詢成功
- [ ] 依專櫃查詢成功（卡別為2004）
- [ ] 報表匯出成功
- [ ] 報表列印成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 卡別切換測試（2004與其他卡別）
- [ ] 報表匯出流程測試
- [ ] 報表列印流程測試
- [ ] 權限檢查測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSL000/SYSL135.ascx`
- `WEB/IMS_CORE/SYSL000/js/SYSL135.js`

### 8.2 相關開發計劃
- `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

