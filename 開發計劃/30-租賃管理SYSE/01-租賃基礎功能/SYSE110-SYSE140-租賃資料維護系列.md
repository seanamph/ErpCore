# SYSE110-SYSE140 - 租賃資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSE110-SYSE140 系列
- **功能名稱**: 租賃資料維護系列
- **功能描述**: 提供租賃資料的新增、修改、刪除、查詢功能，包含租賃編號、租賃類型、租戶、租賃日期、租金、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE110_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE120_FU.ASP` (租賃條件維護)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE130_FU.ASP` (租賃條件維護)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE140_FU.ASP` (租賃類型之會計分類維護)

### 1.2 業務需求
- 管理租賃基本資料
- 支援租賃類型管理
- 支援租戶選擇
- 支援租賃版本管理
- 支援租賃狀態管理（草稿、已審核、已生效、已終止）
- 支援租賃條件維護
- 支援會計分類維護
- 支援租賃報表列印
- 支援租賃歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Leases` (租賃主檔)

```sql
CREATE TABLE [dbo].[Leases] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [LeaseType] NVARCHAR(20) NOT NULL, -- 租賃類型 (LEASE_TYPE)
    [Version] NVARCHAR(10) NOT NULL DEFAULT '1', -- 版本號 (VERSION)
    [TenantId] NVARCHAR(50) NOT NULL, -- 租戶代碼 (TENANT_ID)
    [TenantName] NVARCHAR(200) NULL, -- 租戶名稱 (TENANT_NAME)
    [LeaseDate] DATETIME2 NOT NULL, -- 租賃日期 (LEASE_DATE)
    [StartDate] DATETIME2 NULL, -- 生效日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 終止日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, A:已審核, E:已生效, T:已終止)
    [TotalRent] DECIMAL(18, 4) NULL DEFAULT 0, -- 總租金 (TOTAL_RENT)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [LocationId] NVARCHAR(50) NULL, -- 位置代碼 (LOCATION_ID)
    [FloorId] NVARCHAR(50) NULL, -- 樓層代碼 (FLOOR_ID)
    [Area] DECIMAL(18, 4) NULL, -- 面積 (AREA)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Leases] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Leases_LeaseId_Version] UNIQUE ([LeaseId], [Version])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Leases_LeaseId] ON [dbo].[Leases] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_Leases_TenantId] ON [dbo].[Leases] ([TenantId]);
CREATE NONCLUSTERED INDEX [IX_Leases_Status] ON [dbo].[Leases] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Leases_LeaseDate] ON [dbo].[Leases] ([LeaseDate]);
CREATE NONCLUSTERED INDEX [IX_Leases_LeaseType] ON [dbo].[Leases] ([LeaseType]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseTerms` - 租賃條件
```sql
CREATE TABLE [dbo].[LeaseTerms] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
    [Version] NVARCHAR(10) NOT NULL, -- 版本號
    [TermType] NVARCHAR(20) NOT NULL, -- 條件類型 (TERM_TYPE)
    [TermName] NVARCHAR(200) NULL, -- 條件名稱 (TERM_NAME)
    [TermValue] NVARCHAR(500) NULL, -- 條件值 (TERM_VALUE)
    [TermAmount] DECIMAL(18, 4) NULL, -- 條件金額 (TERM_AMOUNT)
    [TermDate] DATETIME2 NULL, -- 條件日期 (TERM_DATE)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseTerms_Leases] FOREIGN KEY ([LeaseId], [Version]) REFERENCES [dbo].[Leases] ([LeaseId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseTerms_LeaseId] ON [dbo].[LeaseTerms] ([LeaseId], [Version]);
```

#### 2.2.2 `LeaseAccountingCategories` - 租賃會計分類
```sql
CREATE TABLE [dbo].[LeaseAccountingCategories] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號
    [Version] NVARCHAR(10) NOT NULL, -- 版本號
    [CategoryId] NVARCHAR(50) NOT NULL, -- 會計分類代碼 (CATEGORY_ID)
    [CategoryName] NVARCHAR(200) NULL, -- 會計分類名稱 (CATEGORY_NAME)
    [Amount] DECIMAL(18, 4) NULL, -- 金額 (AMOUNT)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseAccountingCategories_Leases] FOREIGN KEY ([LeaseId], [Version]) REFERENCES [dbo].[Leases] ([LeaseId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseAccountingCategories_LeaseId] ON [dbo].[LeaseAccountingCategories] ([LeaseId], [Version]);
```

### 2.3 資料字典

#### 2.3.1 Leases 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 唯一，LEASE_ID |
| LeaseType | NVARCHAR | 20 | NO | - | 租賃類型 | - |
| Version | NVARCHAR | 10 | NO | '1' | 版本號 | VERSION |
| TenantId | NVARCHAR | 50 | NO | - | 租戶代碼 | 外鍵至租戶表 |
| TenantName | NVARCHAR | 200 | YES | - | 租戶名稱 | - |
| LeaseDate | DATETIME2 | - | NO | - | 租賃日期 | LEASE_DATE |
| StartDate | DATETIME2 | - | YES | - | 生效日期 | START_DATE |
| EndDate | DATETIME2 | - | YES | - | 終止日期 | END_DATE |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, A:已審核, E:已生效, T:已終止 |
| TotalRent | DECIMAL | 18,4 | YES | 0 | 總租金 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |
| LocationId | NVARCHAR | 50 | YES | - | 位置代碼 | - |
| FloorId | NVARCHAR | 50 | YES | - | 樓層代碼 | - |
| Area | DECIMAL | 18,4 | YES | - | 面積 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases`
- **說明**: 查詢租賃列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "LeaseId",
    "sortOrder": "ASC",
    "filters": {
      "leaseId": "",
      "leaseType": "",
      "tenantId": "",
      "status": "",
      "leaseDateFrom": "",
      "leaseDateTo": ""
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
          "leaseId": "LS20240101001",
          "leaseType": "1",
          "leaseTypeName": "商場租賃",
          "version": "1",
          "tenantId": "T001",
          "tenantName": "租戶A",
          "leaseDate": "2024-01-01",
          "startDate": "2024-01-01",
          "endDate": "2024-12-31",
          "status": "E",
          "statusName": "已生效",
          "totalRent": 100000.00
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

#### 3.1.2 查詢單筆租賃
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}/{version}`
- **說明**: 查詢單筆租賃資料
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "leaseId": "LS20240101001",
      "leaseType": "1",
      "version": "1",
      "tenantId": "T001",
      "tenantName": "租戶A",
      "leaseDate": "2024-01-01",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "status": "E",
      "totalRent": 100000.00,
      "currencyId": "TWD",
      "exchangeRate": 1.0,
      "locationId": "LOC001",
      "floorId": "F001",
      "area": 50.00,
      "memo": "備註",
      "terms": [],
      "accountingCategories": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增租賃
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases`
- **說明**: 新增租賃資料
- **請求格式**:
  ```json
  {
    "leaseId": "LS20240101001",
    "leaseType": "1",
    "version": "1",
    "tenantId": "T001",
    "tenantName": "租戶A",
    "leaseDate": "2024-01-01",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "status": "D",
    "totalRent": 100000.00,
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "locationId": "LOC001",
    "floorId": "F001",
    "area": 50.00,
    "memo": "備註"
  }
  ```
- **回應格式**: 標準回應格式

#### 3.1.4 修改租賃
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/leases/{leaseId}/{version}`
- **說明**: 修改租賃資料
- **請求格式**: 同新增格式
- **回應格式**: 標準回應格式

#### 3.1.5 刪除租賃
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/leases/{leaseId}/{version}`
- **說明**: 刪除租賃資料（軟刪除或硬刪除）
- **回應格式**: 標準回應格式

#### 3.1.6 查詢租賃條件
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/terms`
- **說明**: 查詢租賃條件列表
- **回應格式**: 標準列表回應格式

#### 3.1.7 新增租賃條件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/terms`
- **說明**: 新增租賃條件
- **請求格式**:
  ```json
  {
    "termType": "1",
    "termName": "租金",
    "termValue": "100000",
    "termAmount": 100000.00,
    "termDate": "2024-01-01",
    "memo": "備註"
  }
  ```

#### 3.1.8 修改租賃條件
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/terms/{tKey}`
- **說明**: 修改租賃條件

#### 3.1.9 刪除租賃條件
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/terms/{tKey}`
- **說明**: 刪除租賃條件

#### 3.1.10 查詢會計分類
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/accounting-categories`
- **說明**: 查詢租賃會計分類列表

#### 3.1.11 新增會計分類
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/accounting-categories`
- **說明**: 新增租賃會計分類

#### 3.1.12 修改會計分類
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/accounting-categories/{tKey}`
- **說明**: 修改租賃會計分類

#### 3.1.13 刪除會計分類
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/leases/{leaseId}/{version}/accounting-categories/{tKey}`
- **說明**: 刪除租賃會計分類

---

## 四、前端 UI 設計

### 4.1 租賃列表頁面

#### 4.1.1 頁面結構
- **路由**: `/leases`
- **組件**: `LeaseList.vue`
- **功能**: 顯示租賃列表，支援查詢、新增、修改、刪除

#### 4.1.2 UI 元件
- **查詢表單**: 租賃編號、租賃類型、租戶、狀態、日期範圍
- **資料表格**: 顯示租賃列表，支援排序、分頁
- **操作按鈕**: 新增、修改、刪除、查詢、報表

#### 4.1.3 表格欄位
- 租賃編號
- 租賃類型
- 租戶名稱
- 租賃日期
- 生效日期
- 終止日期
- 狀態
- 總租金
- 操作（修改、刪除、查看）

### 4.2 租賃新增/修改頁面

#### 4.2.1 頁面結構
- **路由**: `/leases/new`, `/leases/edit/:leaseId/:version`
- **組件**: `LeaseForm.vue`
- **功能**: 新增或修改租賃資料

#### 4.2.2 表單欄位
- 租賃編號（新增時自動產生或手動輸入）
- 租賃類型（下拉選單）
- 版本號（預設為1）
- 租戶（下拉選單，可搜尋）
- 租賃日期（日期選擇器）
- 生效日期（日期選擇器）
- 終止日期（日期選擇器）
- 狀態（下拉選單）
- 總租金（數字輸入）
- 幣別（下拉選單）
- 匯率（數字輸入）
- 位置代碼（下拉選單）
- 樓層代碼（下拉選單）
- 面積（數字輸入）
- 備註（文字區域）

#### 4.2.3 子表單
- **租賃條件表**: 顯示、新增、修改、刪除租賃條件
- **會計分類表**: 顯示、新增、修改、刪除會計分類

### 4.3 租賃查詢頁面

#### 4.3.1 頁面結構
- **路由**: `/leases/query`
- **組件**: `LeaseQuery.vue`
- **功能**: 進階查詢租賃資料

#### 4.3.2 查詢條件
- 租賃編號
- 租賃類型
- 租戶
- 狀態
- 租賃日期範圍
- 生效日期範圍
- 終止日期範圍
- 總租金範圍

### 4.4 租賃報表頁面

#### 4.4.1 頁面結構
- **路由**: `/leases/report`
- **組件**: `LeaseReport.vue`
- **功能**: 產生租賃報表

#### 4.4.2 報表類型
- 租賃清單報表
- 租賃明細報表
- 租賃統計報表

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改頁面開發
- [ ] 查詢頁面開發
- [ ] 報表頁面開發

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 功能測試
- [ ] 效能測試

**總計**: 7.5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 必須檢查租賃是否存在
- 必須記錄所有操作日誌
- 必須驗證租戶是否存在

### 6.2 業務邏輯
- 租賃編號必須唯一
- 版本號必須唯一（同一租賃編號）
- 生效日期必須早於終止日期
- 已生效的租賃不能直接刪除
- 必須支援租賃歷史記錄查詢

### 6.3 資料驗證
- 租賃編號必須符合格式
- 租戶必須存在
- 日期必須有效
- 金額必須為正數
- 面積必須為正數

### 6.4 效能優化
- 查詢列表必須支援分頁
- 必須建立適當的索引
- 必須使用快取機制（如需要）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢租賃列表成功
- [ ] 查詢單筆租賃成功
- [ ] 新增租賃成功
- [ ] 新增租賃失敗（租賃編號重複）
- [ ] 新增租賃失敗（租戶不存在）
- [ ] 修改租賃成功
- [ ] 修改租賃失敗（租賃不存在）
- [ ] 刪除租賃成功
- [ ] 刪除租賃失敗（租賃不存在）
- [ ] 刪除租賃失敗（租賃已生效）

### 7.2 整合測試
- [ ] 新增租賃並新增條件成功
- [ ] 新增租賃並新增會計分類成功
- [ ] 修改租賃並修改條件成功
- [ ] 刪除租賃並刪除相關條件成功
- [ ] 查詢租賃列表並分頁成功
- [ ] 查詢租賃並排序成功

### 7.3 效能測試
- [ ] 查詢大量租賃資料的效能
- [ ] 新增大量租賃資料的效能
- [ ] 修改大量租賃資料的效能

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FI.ASP` - 新增功能
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FU.ASP` - 修改功能
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FD.ASP` - 刪除功能
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FQ.ASP` - 查詢功能
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_FB.ASP` - 瀏覽功能
- `WEB/IMS_CORE/ASP/SYSE000/SYSE110_PR.ASP` - 報表功能

### 8.2 相關文件
- 合同資料維護系列開發計劃（SYSF110-SYSF140）
- 租賃管理SYSM開發計劃（SYSM111-SYSM138）
- 租賃管理SYS8000開發計劃（SYS8110-SYS8220）

### 8.3 技術文件
- .NET Core API 開發指南
- Vue 3 開發指南
- Dapper 使用指南
- SQL Server 資料庫設計指南

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

