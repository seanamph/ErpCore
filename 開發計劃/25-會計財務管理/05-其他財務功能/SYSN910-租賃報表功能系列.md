# SYSN910 - 租賃報表功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN910系列
- **功能名稱**: 租賃報表功能系列
- **功能描述**: 提供租賃報表的查詢、列印、匯出功能，包含租賃報表類型、租賃報表參數等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN910_*.ASP`

### 1.2 業務需求
- 支援租賃報表類型
- 支援租賃報表參數設定
- 支援租賃報表列印與匯出
- 支援租賃報表瀏覽與查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LeaseReports`

```sql
CREATE TABLE [dbo].[LeaseReports] (
    [ReportId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [ReportType] NVARCHAR(50) NOT NULL,
    [ReportName] NVARCHAR(200) NOT NULL,
    [ReportDate] DATETIME2 NOT NULL,
    [LeaseId] NVARCHAR(50) NULL,
    [TenantId] NVARCHAR(50) NULL,
    [PropertyId] NVARCHAR(50) NULL,
    [ReportData] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ModifiedBy] NVARCHAR(50) NULL,
    [ModifiedDate] DATETIME2 NULL,
    CONSTRAINT [FK_LeaseReports_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [Leases]([LeaseId]),
    CONSTRAINT [FK_LeaseReports_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([TenantId]),
    CONSTRAINT [FK_LeaseReports_Properties] FOREIGN KEY ([PropertyId]) REFERENCES [Properties]([PropertyId])
);

CREATE INDEX [IX_LeaseReports_ReportType] ON [dbo].[LeaseReports]([ReportType]);
CREATE INDEX [IX_LeaseReports_ReportDate] ON [dbo].[LeaseReports]([ReportDate]);
CREATE INDEX [IX_LeaseReports_LeaseId] ON [dbo].[LeaseReports]([LeaseId]);
```

### 2.2 資料表: `LeaseReportDetails`

```sql
CREATE TABLE [dbo].[LeaseReportDetails] (
    [DetailId] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReportId] NVARCHAR(50) NOT NULL,
    [ItemName] NVARCHAR(200) NOT NULL,
    [ItemValue] DECIMAL(18,2) NULL,
    [ItemDescription] NVARCHAR(500) NULL,
    [SortOrder] INT NULL DEFAULT 0,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseReportDetails_LeaseReports] FOREIGN KEY ([ReportId]) REFERENCES [LeaseReports]([ReportId]) ON DELETE CASCADE
);

CREATE INDEX [IX_LeaseReportDetails_ReportId] ON [dbo].[LeaseReportDetails]([ReportId]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃報表列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-reports`
- **說明**: 查詢租賃報表列表
- **查詢參數**:
  - `reportType` (string, optional): 報表類型
  - `startDate` (datetime, optional): 開始日期
  - `endDate` (datetime, optional): 結束日期
  - `leaseId` (string, optional): 租賃ID
  - `tenantId` (string, optional): 租戶ID
  - `page` (int, optional): 頁碼，預設為1
  - `pageSize` (int, optional): 每頁筆數，預設為20
- **回應範例**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "reportId": "LR001",
        "reportType": "LEASE_SUMMARY",
        "reportName": "租賃摘要報表",
        "reportDate": "2024-01-01T00:00:00Z",
        "leaseId": "L001",
        "tenantId": "T001",
        "status": "A"
      }
    ],
    "totalCount": 100,
    "page": 1,
    "pageSize": 20
  }
}
```

#### 3.1.2 查詢單筆租賃報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-reports/{reportId}`
- **說明**: 查詢單筆租賃報表詳細資料
- **路徑參數**:
  - `reportId` (string, required): 報表ID
- **回應範例**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "reportId": "LR001",
    "reportType": "LEASE_SUMMARY",
    "reportName": "租賃摘要報表",
    "reportDate": "2024-01-01T00:00:00Z",
    "leaseId": "L001",
    "tenantId": "T001",
    "propertyId": "P001",
    "reportData": "{}",
    "status": "A",
    "details": [
      {
        "itemName": "租金",
        "itemValue": 50000.00,
        "itemDescription": "月租金"
      }
    ]
  }
}
```

#### 3.1.3 新增租賃報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-reports`
- **說明**: 新增租賃報表
- **請求範例**:
```json
{
  "reportType": "LEASE_SUMMARY",
  "reportName": "租賃摘要報表",
  "reportDate": "2024-01-01T00:00:00Z",
  "leaseId": "L001",
  "tenantId": "T001",
  "propertyId": "P001",
  "reportData": "{}",
  "details": [
    {
      "itemName": "租金",
      "itemValue": 50000.00,
      "itemDescription": "月租金",
      "sortOrder": 1
    }
  ]
}
```

#### 3.1.4 修改租賃報表
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-reports/{reportId}`
- **說明**: 修改租賃報表
- **路徑參數**:
  - `reportId` (string, required): 報表ID
- **請求範例**:
```json
{
  "reportName": "租賃摘要報表（更新）",
  "reportData": "{}",
  "details": [
    {
      "itemName": "租金",
      "itemValue": 55000.00,
      "itemDescription": "月租金（更新）",
      "sortOrder": 1
    }
  ]
}
```

#### 3.1.5 刪除租賃報表
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-reports/{reportId}`
- **說明**: 刪除租賃報表
- **路徑參數**:
  - `reportId` (string, required): 報表ID

#### 3.1.6 匯出租賃報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-reports/{reportId}/export`
- **說明**: 匯出租賃報表（Excel、PDF）
- **路徑參數**:
  - `reportId` (string, required): 報表ID
- **查詢參數**:
  - `format` (string, required): 匯出格式（excel、pdf）

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃報表列表頁面 (`LeaseReportList.vue`)
- **路徑**: `/accounting/lease-reports`
- **功能**: 顯示租賃報表列表
- **元件**:
  - 搜尋表單（報表類型、日期範圍、租賃ID、租戶ID）
  - 資料表格（報表ID、報表類型、報表名稱、報表日期、狀態）
  - 操作按鈕（新增、修改、刪除、匯出、查詢）

#### 4.1.2 租賃報表詳細頁面 (`LeaseReportDetail.vue`)
- **路徑**: `/accounting/lease-reports/:reportId`
- **功能**: 顯示租賃報表詳細資料
- **元件**:
  - 報表基本資料表單
  - 報表明細資料表格
  - 操作按鈕（儲存、取消、匯出）

#### 4.1.3 租賃報表新增/修改頁面 (`LeaseReportForm.vue`)
- **路徑**: `/accounting/lease-reports/new` 或 `/accounting/lease-reports/:reportId/edit`
- **功能**: 新增或修改租賃報表
- **元件**:
  - 報表基本資料表單
  - 報表明細資料表格（可新增、修改、刪除明細）
  - 操作按鈕（儲存、取消）

---

## 五、開發時程

**總計**: 15天
- 資料庫設計與建立: 2天
- 後端API開發: 5天
- 前端UI開發: 6天
- 測試與修正: 2天

---

## 六、注意事項

1. 租賃報表資料需要與租賃主檔、租戶主檔、物業主檔關聯
2. 報表匯出功能需要支援Excel和PDF格式
3. 報表資料需要支援JSON格式儲存，以便後續擴展
4. 報表刪除時需要同時刪除相關的明細資料（級聯刪除）
5. 報表查詢需要支援多條件組合查詢
6. 報表列印需要支援自訂格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢租賃報表列表成功
- [ ] 查詢單筆租賃報表成功
- [ ] 新增租賃報表成功
- [ ] 修改租賃報表成功
- [ ] 刪除租賃報表成功
- [ ] 匯出租賃報表成功（Excel、PDF）

### 7.2 整合測試
- [ ] 租賃報表與租賃主檔關聯正確
- [ ] 租賃報表與租戶主檔關聯正確
- [ ] 租賃報表與物業主檔關聯正確
- [ ] 報表刪除時明細資料同時刪除

### 7.3 效能測試
- [ ] 大量資料查詢效能測試
- [ ] 報表匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN910_*.ASP`

### 8.2 相關功能
- 租賃管理功能（SYS8000、SYSE000、SYSM000系列）
- 財務報表功能（SYSN510-SYSN540系列）

---

**文件版本**: v1.0  
**建立日期**: 2024-12-20  
**最後更新**: 2024-12-20

