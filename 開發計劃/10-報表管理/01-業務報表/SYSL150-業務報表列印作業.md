# SYSL150 - 業務報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL150
- **功能名稱**: 業務報表列印作業
- **功能描述**: 提供業務報表列印功能，包含年度審核、批次審核、複製下一年度資料、數量計算等功能，支援專屬審核帳號設定
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL150.ascx` (主要頁面)
  - `WEB/IMS_CORE/SYSL000/SYSL150.ascx.cs` (後端邏輯)
  - `WEB/IMS_CORE/SYSL000/js/SYSL150.js` (前端邏輯)

### 1.2 業務需求
- 管理業務報表列印資料
- 支援年度審核功能
- 支援批次審核功能
- 支援複製下一年度資料功能
- 支援數量計算功能
- 支援專屬審核帳號設定
- 支援年度修改唯讀控制
- 支援店別切換功能
- 記錄業務報表的建立與變更資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `BusinessReportPrint` (對應舊系統業務報表列印)

```sql
CREATE TABLE [dbo].[BusinessReportPrint] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [GiveYear] INT NOT NULL, -- 發放年度 (GIVE_YEAR)
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [EmpId] NVARCHAR(50) NOT NULL, -- 員工編號 (EMP_ID)
    [EmpName] NVARCHAR(100) NULL, -- 員工姓名 (EMP_NAME)
    [Qty] DECIMAL(18,2) NULL DEFAULT 0, -- 數量 (QTY)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS) P:待審核, A:已審核, R:已拒絕
    [Verifier] NVARCHAR(50) NULL, -- 審核者 (VERIFIER)
    [VerifyDate] DATETIME2 NULL, -- 審核日期 (VERIFY_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_BusinessReportPrint] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_GiveYear] ON [dbo].[BusinessReportPrint] ([GiveYear]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_SiteId] ON [dbo].[BusinessReportPrint] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_OrgId] ON [dbo].[BusinessReportPrint] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_EmpId] ON [dbo].[BusinessReportPrint] ([EmpId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_Status] ON [dbo].[BusinessReportPrint] ([Status]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrint_Verifier] ON [dbo].[BusinessReportPrint] ([Verifier]);
```

### 2.2 相關資料表

#### 2.2.1 `Sites` - 店別主檔
- 用於查詢店別列表
- 參考: `開發計劃/02-基本資料管理/04-組織架構/SYSWB60-庫別資料維護作業.md`

#### 2.2.2 `Organizations` - 組織主檔
- 用於查詢組織列表
- 參考組織架構相關功能

#### 2.2.3 `Users` - 使用者主檔
- 用於查詢使用者列表
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| GiveYear | INT | - | NO | - | 發放年度 | - |
| SiteId | NVARCHAR | 50 | NO | - | 店別代碼 | 外鍵至Sites表 |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至Organizations表 |
| EmpId | NVARCHAR | 50 | NO | - | 員工編號 | - |
| EmpName | NVARCHAR | 100 | YES | - | 員工姓名 | - |
| Qty | DECIMAL | 18,2 | YES | 0 | 數量 | - |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待審核, A:已審核, R:已拒絕 |
| Verifier | NVARCHAR | 50 | YES | - | 審核者 | 外鍵至Users表 |
| VerifyDate | DATETIME2 | - | YES | - | 審核日期 | - |
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

#### 3.1.1 查詢業務報表列印列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-report-print`
- **說明**: 查詢業務報表列印列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TKey",
    "sortOrder": "ASC",
    "filters": {
      "giveYear": 2024,
      "siteId": "",
      "orgId": "",
      "empId": "",
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
          "giveYear": 2024,
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "orgId": "ORG001",
          "orgName": "組織名稱",
          "empId": "EMP001",
          "empName": "員工姓名",
          "qty": 100.00,
          "status": "P",
          "verifier": "U001",
          "verifierName": "審核者名稱",
          "verifyDate": null,
          "createdAt": "2024-01-01T00:00:00",
          "updatedAt": "2024-01-01T00:00:00"
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

#### 3.1.2 查詢單筆業務報表列印
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-report-print/{tKey}`
- **說明**: 根據主鍵查詢單筆業務報表列印資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表單筆資料

#### 3.1.3 新增業務報表列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-print`
- **說明**: 新增業務報表列印資料
- **請求格式**:
  ```json
  {
    "giveYear": 2024,
    "siteId": "SITE001",
    "orgId": "ORG001",
    "empId": "EMP001",
    "empName": "員工姓名",
    "qty": 100.00,
    "status": "P",
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

#### 3.1.4 修改業務報表列印
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/business-report-print/{tKey}`
- **說明**: 修改業務報表列印資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增
- **回應格式**: 同新增

#### 3.1.5 刪除業務報表列印
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/business-report-print/{tKey}`
- **說明**: 刪除業務報表列印資料
- **路徑參數**:
  - `tKey`: 主鍵
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

#### 3.1.6 批次審核
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-print/batch-audit`
- **說明**: 批次審核多筆業務報表列印資料
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3],
    "status": "A",
    "notes": "批次審核備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "批次審核成功",
    "data": {
      "successCount": 3,
      "failCount": 0
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 複製下一年度資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-print/copy-next-year`
- **說明**: 複製指定年度的資料到下一年度
- **請求格式**:
  ```json
  {
    "sourceYear": 2024,
    "targetYear": 2025,
    "siteId": "SITE001"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "複製成功",
    "data": {
      "copiedCount": 100
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 計算數量
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-print/calculate-qty`
- **說明**: 計算業務報表列印數量
- **請求格式**:
  ```json
  {
    "tKey": 1,
    "calculationRules": {}
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "計算成功",
    "data": {
      "qty": 100.00
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 查詢頁面 (`SYSL150Query.vue`)

#### 4.1.1 頁面結構
- 查詢表單（年度、店別、組織、員工、狀態）
- 查詢結果列表（支援多選、排序、分頁）
- 新增、修改、刪除、批次刪除按鈕
- 批次審核、複製下一年度按鈕
- 報表列印與匯出功能

#### 4.1.2 主要功能
- 支援多條件查詢
- 支援列表排序與分頁
- 支援多選與批次操作
- 支援批次審核功能
- 支援複製下一年度資料功能
- 支援報表列印與匯出

### 4.2 新增/修改頁面 (`SYSL150Edit.vue`)

#### 4.2.1 頁面結構
- 表單欄位（年度、店別、組織、員工、數量、狀態、備註）
- 數量計算功能
- 年度修改唯讀控制
- 店別切換功能
- 審核功能（需符合審核權限）
- 儲存、取消按鈕

#### 4.2.2 主要功能
- 表單驗證（必填欄位、格式驗證）
- 數量計算功能
- 年度修改唯讀控制（已審核的年度不可修改）
- 店別切換功能（切換店別時重新載入資料）
- 審核功能（需符合審核權限，支援專屬審核帳號）

### 4.3 報表列印頁面
- 報表列印功能
- 報表匯出功能（Excel、PDF）

---

## 五、業務邏輯說明

### 5.1 年度審核
- 支援單筆審核與批次審核
- 審核者需符合審核權限
- 支援專屬審核帳號設定（僅特定帳號可審核）
- 審核後更新狀態與審核日期

### 5.2 複製下一年度資料
- 複製指定年度的資料到下一年度
- 可選擇特定店別進行複製
- 複製的資料狀態為待審核

### 5.3 數量計算
- 根據業務規則計算數量
- 支援自動計算與手動調整

### 5.4 年度修改唯讀控制
- 已審核的年度資料不可修改
- 僅待審核的資料可修改

### 5.5 專屬審核帳號
- 支援設定專屬審核帳號
- 僅專屬審核帳號可進行審核操作

---

## 六、開發注意事項

### 6.1 權限控制
- 需檢查使用者是否有業務報表列印權限
- 需檢查使用者是否有審核權限
- 需檢查專屬審核帳號設定

### 6.2 資料驗證
- 需實作年度修改唯讀控制邏輯
- 需實作數量計算邏輯
- 需提供清楚的錯誤訊息

### 6.3 效能優化
- 批次審核需考慮大量資料處理
- 複製下一年度資料需考慮效能
- 查詢結果需支援分頁與索引優化

### 6.4 錯誤處理
- 需處理審核權限檢查失敗的情況
- 需處理批次操作失敗的情況
- 需提供使用者友善的錯誤訊息

---

## 七、測試計劃

### 7.1 單元測試
- 年度審核邏輯測試
- 批次審核邏輯測試
- 複製下一年度資料邏輯測試
- 數量計算邏輯測試

### 7.2 整合測試
- API 端點測試
- 前端表單驗證測試
- 權限控制測試

### 7.3 使用者驗收測試
- 業務流程測試
- 錯誤處理測試
- 報表列印與匯出測試

---

## 八、相關文件
- [SYSL130 - 業務報表查詢作業](./SYSL130-業務報表查詢作業.md)
- [SYSL135 - 業務報表查詢作業](./SYSL135-業務報表查詢作業.md)
- [SYSL145 - 業務報表查詢作業](./SYSL145-業務報表查詢作業.md)

