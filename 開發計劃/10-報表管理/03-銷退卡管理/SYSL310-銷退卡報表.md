# SYSL310 - 銷退卡報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL310-報表
- **功能名稱**: 銷退卡報表
- **功能描述**: 提供銷退卡報表的查詢與列印功能，支援依年度、月份、店別、組織等條件查詢銷退卡資料，包含銷退卡明細、統計資訊等，支援報表列印與匯出
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/js/SYSL310.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL310.css` (樣式)
  - 相關報表定義檔案

### 1.2 業務需求
- 查詢銷退卡報表資料
- 支援依年度、月份、店別、組織等條件查詢
- 支援銷退卡明細查詢
- 支援銷退卡統計資訊查詢
- 支援報表列印功能
- 支援報表匯出功能（Excel、PDF）
- 支援報表參數設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `ReturnCards` - 銷退卡主檔
- 參考: `開發計劃/10-報表管理/03-銷退卡管理/SYSL310-銷退卡管理.md` 的 `ReturnCards` 資料表結構

#### 2.1.2 `ReturnCardDetails` - 銷退卡明細檔
```sql
CREATE TABLE [dbo].[ReturnCardDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [ReturnCardId] BIGINT NOT NULL, -- 銷退卡主檔ID
    [EmployeeId] NVARCHAR(50) NULL, -- 員工編號
    [EmployeeName] NVARCHAR(100) NULL, -- 員工姓名
    [ReturnDate] DATETIME2 NULL, -- 銷退日期
    [ReturnReason] NVARCHAR(200) NULL, -- 銷退原因
    [Amount] DECIMAL(18, 2) NULL DEFAULT 0, -- 金額
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [FK_ReturnCardDetails_ReturnCards] FOREIGN KEY ([ReturnCardId]) REFERENCES [dbo].[ReturnCards] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_ReturnCardId] ON [dbo].[ReturnCardDetails] ([ReturnCardId]);
CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_EmployeeId] ON [dbo].[ReturnCardDetails] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_ReturnCardDetails_ReturnDate] ON [dbo].[ReturnCardDetails] ([ReturnDate]);
```

### 2.2 資料字典

#### ReturnCardDetails 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ReturnCardId | BIGINT | - | NO | - | 銷退卡主檔ID | 外鍵至ReturnCards |
| EmployeeId | NVARCHAR | 50 | YES | - | 員工編號 | - |
| EmployeeName | NVARCHAR | 100 | YES | - | 員工姓名 | - |
| ReturnDate | DATETIME2 | - | YES | - | 銷退日期 | - |
| ReturnReason | NVARCHAR | 200 | YES | - | 銷退原因 | - |
| Amount | DECIMAL | 18,2 | YES | 0 | 金額 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢銷退卡報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/return-cards/report`
- **說明**: 查詢銷退卡報表資料，支援多種查詢條件
- **請求參數**:
  ```json
  {
    "siteId": "",
    "orgId": "",
    "cardYear": 2024,
    "cardMonth": 1,
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "employeeId": "",
    "reportType": "detail", // detail: 明細, summary: 統計
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
      "items": [...],
      "summary": {
        "totalCount": 100,
        "totalAmount": 50000.00
      },
      "pagination": {
        "pageIndex": 1,
        "pageSize": 20,
        "totalCount": 100,
        "totalPages": 5
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出銷退卡報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/return-cards/report/export`
- **說明**: 匯出銷退卡報表資料（Excel、PDF）
- **請求格式**:
  ```json
  {
    "siteId": "",
    "orgId": "",
    "cardYear": 2024,
    "cardMonth": 1,
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "employeeId": "",
    "reportType": "detail",
    "exportFormat": "excel" // excel, pdf
  }
  ```
- **回應格式**: 檔案下載

#### 3.1.3 列印銷退卡報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/return-cards/report/print`
- **說明**: 產生銷退卡報表列印資料
- **請求格式**: 同查詢報表
- **回應格式**: PDF 檔案

### 3.2 後端實作類別

#### 3.2.1 Controller: `ReturnCardReportController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/return-cards/report")]
    [Authorize]
    public class ReturnCardReportController : ControllerBase
    {
        private readonly IReturnCardReportService _reportService;
        
        public ReturnCardReportController(IReturnCardReportService reportService)
        {
            _reportService = reportService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ReturnCardReportDto>>> GetReport([FromQuery] ReturnCardReportQueryDto query)
        {
            // 實作查詢報表邏輯
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportReport([FromBody] ReturnCardReportExportDto dto)
        {
            // 實作匯出報表邏輯
        }
        
        [HttpPost("print")]
        public async Task<IActionResult> PrintReport([FromBody] ReturnCardReportQueryDto query)
        {
            // 實作列印報表邏輯
        }
    }
}
```

#### 3.2.2 Service: `ReturnCardReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IReturnCardReportService
    {
        Task<ReturnCardReportDto> GetReportAsync(ReturnCardReportQueryDto query);
        Task<byte[]> ExportReportAsync(ReturnCardReportExportDto dto);
        Task<byte[]> PrintReportAsync(ReturnCardReportQueryDto query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 銷退卡報表查詢頁面 (`ReturnCardReport.vue`)
- **路徑**: `/reports/return-cards`
- **功能**: 顯示銷退卡報表查詢頁面，支援查詢條件設定、報表資料顯示、報表列印與匯出

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ReturnCardReportSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="店別">
      <el-select v-model="searchForm.siteId" placeholder="請選擇店別">
        <el-option v-for="site in siteList" :key="site.siteId" :label="site.siteName" :value="site.siteId" />
      </el-select>
    </el-form-item>
    <el-form-item label="組織">
      <el-select v-model="searchForm.orgId" placeholder="請選擇組織">
        <el-option v-for="org in orgList" :key="org.orgId" :label="org.orgName" :value="org.orgId" />
      </el-select>
    </el-form-item>
    <el-form-item label="年度">
      <el-date-picker v-model="searchForm.cardYear" type="year" placeholder="請選擇年度" />
    </el-form-item>
    <el-form-item label="月份">
      <el-select v-model="searchForm.cardMonth" placeholder="請選擇月份">
        <el-option v-for="month in 12" :key="month" :label="`${month}月`" :value="month" />
      </el-select>
    </el-form-item>
    <el-form-item label="報表類型">
      <el-radio-group v-model="searchForm.reportType">
        <el-radio label="detail">明細</el-radio>
        <el-radio label="summary">統計</el-radio>
      </el-radio-group>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleExport">匯出</el-button>
      <el-button type="warning" @click="handlePrint">列印</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 報表資料表格元件 (`ReturnCardReportTable.vue`)
```vue
<template>
  <div>
    <el-table :data="reportData.items" v-loading="loading" border>
      <el-table-column prop="siteName" label="店別" width="120" />
      <el-table-column prop="orgName" label="組織" width="120" />
      <el-table-column prop="cardYear" label="年度" width="80" />
      <el-table-column prop="cardMonth" label="月份" width="80" />
      <el-table-column prop="employeeId" label="員工編號" width="120" />
      <el-table-column prop="employeeName" label="員工姓名" width="150" />
      <el-table-column prop="returnDate" label="銷退日期" width="120" />
      <el-table-column prop="returnReason" label="銷退原因" width="200" />
      <el-table-column prop="amount" label="金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.amount) }}
        </template>
      </el-table-column>
    </el-table>
    <div v-if="reportData.summary" class="summary-section">
      <el-descriptions title="統計資訊" :column="4" border>
        <el-descriptions-item label="總筆數">{{ reportData.summary.totalCount }}</el-descriptions-item>
        <el-descriptions-item label="總金額">{{ formatCurrency(reportData.summary.totalAmount) }}</el-descriptions-item>
      </el-descriptions>
    </div>
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

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 ReturnCardDetails 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（報表查詢、匯出、列印）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表產生邏輯（Excel、PDF）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 報表查詢頁面開發
- [ ] 查詢表單開發
- [ ] 報表資料表格開發
- [ ] 報表匯出功能
- [ ] 報表列印功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 報表產生測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- 報表查詢需支援大量資料
- 報表匯出需處理大量資料
- 報表列印需支援多種格式

### 6.2 效能優化
- 大量資料查詢需使用分頁
- 報表查詢需使用適當索引
- 報表匯出需使用非同步處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢報表成功
- [ ] 匯出報表成功（Excel、PDF）
- [ ] 列印報表成功
- [ ] 查詢條件驗證測試

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 報表匯出流程測試
- [ ] 報表列印流程測試
- [ ] 大量資料處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSL000/js/SYSL310.js`
- `WEB/IMS_CORE/SYSL000/style/SYSL310.css`

### 8.2 相關功能
- `SYSL310-銷退卡管理.md` - 銷退卡管理功能
- `SYSL210-員餐卡報表.md` - 類似報表功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

