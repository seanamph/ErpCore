# SYS1360 - 銷售報表模組(Crystal Reports) 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS1360
- **功能名稱**: 銷售報表模組(Crystal Reports)
- **功能描述**: 提供銷售報表的查詢和列印功能，使用 Crystal Reports 報表引擎生成報表，支援 ASP.NET 頁面顯示和報表列印
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360_PR.aspx` (報表頁面)
  - `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360_PR.rpt` (Crystal Reports 報表定義)
  - `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360.xsd` (資料架構)
  - `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/ASPXTOASP.asp` (ASPX 轉 ASP 工具)
  - `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SAMPLE.aspx` (範例頁面)

### 1.2 業務需求
- 提供銷售報表查詢功能
- 支援 Crystal Reports 報表引擎
- 支援報表列印功能
- 支援報表資料匯出（PDF、Excel）
- 支援報表參數設定
- 支援報表快取機制
- 提供 ASP 到 ASP.NET 的轉換工具

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `SalesReportData` - 銷售報表資料表
```sql
CREATE TABLE [dbo].[SalesReportData] (
    [ReportId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS1360)
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [ShopId] NVARCHAR(50) NULL, -- 店別代碼
    [StartDate] DATETIME2 NULL, -- 起始日期
    [EndDate] DATETIME2 NULL, -- 結束日期
    [ReportType] NVARCHAR(50) NULL, -- 報表類型
    [ReportData] NVARCHAR(MAX) NULL, -- 報表資料 (JSON格式)
    [ReportStatus] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_SalesReportData] PRIMARY KEY CLUSTERED ([ReportId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesReportData_ReportCode] ON [dbo].[SalesReportData] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_SalesReportData_ShopId] ON [dbo].[SalesReportData] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_SalesReportData_StartDate_EndDate] ON [dbo].[SalesReportData] ([StartDate], [EndDate]);
```

#### 2.1.2 `ReportCache` - 報表快取表
```sql
CREATE TABLE [dbo].[ReportCache] (
    [CacheId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [ReportCode] NVARCHAR(50) NOT NULL,
    [CacheKey] NVARCHAR(255) NOT NULL, -- 快取鍵值
    [CacheData] VARBINARY(MAX) NULL, -- 快取資料
    [ExpireTime] DATETIME2 NOT NULL, -- 過期時間
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [IX_ReportCache_ReportCode_CacheKey] UNIQUE ([ReportCode], [CacheKey])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportCache_ExpireTime] ON [dbo].[ReportCache] ([ExpireTime]);
```

### 2.2 相關資料表

#### 2.2.1 `Shops` - 店別主檔
```sql
-- 參考店別主檔設計
-- 用於報表查詢的店別篩選
```

#### 2.2.2 `Products` - 商品主檔
```sql
-- 參考商品主檔設計
-- 用於報表查詢的商品資料
```

#### 2.2.3 `SalesTransactions` - 銷售交易資料
```sql
-- 參考銷售交易資料表設計
-- 用於報表查詢的銷售資料
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ReportId | UNIQUEIDENTIFIER | - | NO | NEWID() | 報表ID | 主鍵 |
| ReportCode | NVARCHAR | 50 | NO | - | 報表代碼 | SYS1360 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| ShopId | NVARCHAR | 50 | YES | - | 店別代碼 | 外鍵至店別表 |
| StartDate | DATETIME2 | - | YES | - | 起始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| ReportType | NVARCHAR | 50 | YES | - | 報表類型 | - |
| ReportData | NVARCHAR(MAX) | - | YES | - | 報表資料 | JSON格式 |
| ReportStatus | NVARCHAR | 10 | NO | 'A' | 報表狀態 | A:啟用, I:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys1360`
- **說明**: 查詢銷售報表資料，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CreatedAt",
    "sortOrder": "DESC",
    "filters": {
      "shopId": "",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "reportType": ""
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
          "reportId": "guid",
          "reportCode": "SYS1360",
          "reportName": "銷售報表",
          "shopId": "SHOP001",
          "startDate": "2024-01-01T00:00:00",
          "endDate": "2024-12-31T23:59:59",
          "reportType": "SALES",
          "reportStatus": "A",
          "createdAt": "2024-01-01T00:00:00"
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

#### 3.1.2 產生報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys1360/generate`
- **說明**: 產生銷售報表資料
- **請求格式**:
  ```json
  {
    "shopId": "SHOP001",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "reportType": "SALES",
    "parameters": {}
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "報表產生成功",
    "data": {
      "reportId": "guid",
      "reportUrl": "/api/v1/reports/sys1360/{reportId}/download"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 下載報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys1360/{reportId}/download`
- **說明**: 下載報表檔案（PDF、Excel）
- **路徑參數**:
  - `reportId`: 報表ID
- **查詢參數**:
  - `format`: 檔案格式 (pdf, excel)
- **回應格式**: 檔案下載

#### 3.1.4 列印報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys1360/{reportId}/print`
- **說明**: 列印報表
- **路徑參數**:
  - `reportId`: 報表ID
- **請求格式**:
  ```json
  {
    "printerName": "Printer001",
    "copies": 1,
    "pageRange": "1-10"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "報表列印成功",
    "data": {
      "jobId": "guid"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 刪除報表
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sys1360/{reportId}`
- **說明**: 刪除報表資料
- **路徑參數**:
  - `reportId`: 報表ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "報表刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 報表查詢頁面

#### 4.1.1 查詢條件區域
- **店別選擇**: 下拉選單，支援多選
- **日期範圍**: 日期選擇器（起始日期、結束日期）
- **報表類型**: 下拉選單
- **查詢按鈕**: 執行查詢
- **重置按鈕**: 清除查詢條件

#### 4.1.2 報表列表區域
- **表格顯示**: 顯示報表列表
  - 報表名稱
  - 店別
  - 日期範圍
  - 報表類型
  - 建立時間
  - 操作按鈕（查看、下載、列印、刪除）
- **分頁控制**: 分頁器
- **排序功能**: 點擊欄位標題排序

#### 4.1.3 報表預覽區域
- **報表預覽**: 使用 Crystal Reports Viewer 顯示報表
- **工具列**: 
  - 列印按鈕
  - 下載按鈕（PDF、Excel）
  - 重新整理按鈕
  - 縮放控制

### 4.2 報表產生頁面

#### 4.2.1 參數設定區域
- **報表參數表單**: 動態產生參數輸入欄位
- **預覽按鈕**: 預覽報表
- **產生按鈕**: 產生報表
- **取消按鈕**: 取消操作

#### 4.2.2 報表預覽區域
- **報表預覽**: 顯示產生的報表

### 4.3 UI 組件設計

#### 4.3.1 報表查詢組件
```vue
<template>
  <div class="report-query">
    <el-form :model="queryForm" :inline="true">
      <el-form-item label="店別">
        <el-select v-model="queryForm.shopId" multiple>
          <el-option v-for="shop in shops" :key="shop.id" :label="shop.name" :value="shop.id" />
        </el-select>
      </el-form-item>
      <el-form-item label="起始日期">
        <el-date-picker v-model="queryForm.startDate" type="date" />
      </el-form-item>
      <el-form-item label="結束日期">
        <el-date-picker v-model="queryForm.endDate" type="date" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
```

#### 4.3.2 報表列表組件
```vue
<template>
  <div class="report-list">
    <el-table :data="reportList" v-loading="loading">
      <el-table-column prop="reportName" label="報表名稱" />
      <el-table-column prop="shopName" label="店別" />
      <el-table-column prop="dateRange" label="日期範圍" />
      <el-table-column prop="reportType" label="報表類型" />
      <el-table-column prop="createdAt" label="建立時間" />
      <el-table-column label="操作" width="200">
        <template #default="scope">
          <el-button size="small" @click="handleView(scope.row)">查看</el-button>
          <el-button size="small" @click="handleDownload(scope.row)">下載</el-button>
          <el-button size="small" @click="handlePrint(scope.row)">列印</el-button>
          <el-button size="small" type="danger" @click="handleDelete(scope.row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

#### 4.3.3 報表預覽組件
```vue
<template>
  <div class="report-preview">
    <div class="report-toolbar">
      <el-button @click="handlePrint">列印</el-button>
      <el-button @click="handleDownload('pdf')">下載 PDF</el-button>
      <el-button @click="handleDownload('excel')">下載 Excel</el-button>
      <el-button @click="handleRefresh">重新整理</el-button>
    </div>
    <div class="report-viewer">
      <!-- Crystal Reports Viewer 或 PDF Viewer -->
      <iframe :src="reportUrl" />
    </div>
  </div>
</template>
```

---

## 五、開發時程

### 5.1 第一階段：資料庫設計與 API 開發（2週）
- 建立資料表結構
- 開發查詢 API
- 開發產生報表 API
- 開發下載報表 API
- 開發列印報表 API
- 開發刪除報表 API

### 5.2 第二階段：前端 UI 開發（2週）
- 開發報表查詢頁面
- 開發報表列表組件
- 開發報表預覽組件
- 開發報表產生頁面
- 整合 Crystal Reports Viewer

### 5.3 第三階段：測試與優化（1週）
- 單元測試
- 整合測試
- 效能優化
- 報表快取機制實作

---

## 六、注意事項

### 6.1 技術注意事項
1. **Crystal Reports 整合**:
   - 需要安裝 Crystal Reports Runtime
   - 報表檔案 (.rpt) 需要放在指定目錄
   - 報表資料來源需要正確設定

2. **報表快取**:
   - 實作報表快取機制，減少資料庫查詢
   - 設定快取過期時間
   - 處理快取失效邏輯

3. **報表產生效能**:
   - 大量資料報表需要分批處理
   - 考慮使用背景任務產生報表
   - 實作報表產生進度追蹤

4. **檔案下載**:
   - 支援 PDF 和 Excel 格式下載
   - 處理大檔案下載
   - 實作下載進度顯示

### 6.2 業務注意事項
1. **報表權限控制**:
   - 根據使用者權限顯示可查詢的店別
   - 控制報表的產生、下載、列印權限

2. **報表資料準確性**:
   - 確保報表資料與實際資料一致
   - 處理資料異動時的報表更新

3. **報表列印**:
   - 支援多種列印格式
   - 處理列印錯誤情況

---

## 七、測試案例

### 7.1 功能測試
1. **查詢報表**:
   - 測試基本查詢功能
   - 測試多條件查詢
   - 測試分頁功能
   - 測試排序功能

2. **產生報表**:
   - 測試報表產生功能
   - 測試報表參數設定
   - 測試報表產生錯誤處理

3. **下載報表**:
   - 測試 PDF 下載
   - 測試 Excel 下載
   - 測試大檔案下載

4. **列印報表**:
   - 測試報表列印功能
   - 測試列印參數設定
   - 測試列印錯誤處理

5. **刪除報表**:
   - 測試報表刪除功能
   - 測試刪除權限控制

### 7.2 效能測試
1. **報表產生效能**:
   - 測試大量資料報表產生時間
   - 測試報表快取效果

2. **查詢效能**:
   - 測試大量報表資料查詢效能
   - 測試索引效果

### 7.3 安全性測試
1. **權限控制**:
   - 測試報表查詢權限
   - 測試報表產生權限
   - 測試報表下載權限
   - 測試報表列印權限
   - 測試報表刪除權限

2. **資料安全**:
   - 測試 SQL 注入防護
   - 測試 XSS 攻擊防護

---

## 八、參考資料

### 8.1 舊系統檔案
- `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360_PR.aspx`
- `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360_PR.rpt`
- `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SYS1360.xsd`
- `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/ASPXTOASP.asp`
- `WEB/IMS_CORE/ASP/Reports/SYS1000/SYS1360/SAMPLE.aspx`

### 8.2 技術文件
- Crystal Reports 官方文件
- ASP.NET Core 報表整合文件
- Vue.js 報表組件文件

### 8.3 相關功能
- SYS1000 銷售報表模組系列
- 其他報表模組

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

