# SYSL145 - 業務報表查詢作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL145
- **功能名稱**: 業務報表查詢作業
- **功能描述**: 提供業務報表資料的新增、修改、刪除、查詢功能，包含店別、卡片類型、動作類型、廠商等資訊管理，支援管理權限檢查與重複資料驗證
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL145.ascx` (主要頁面)
  - `WEB/IMS_CORE/SYSL000/SYSL145.ascx.cs` (後端邏輯)
  - `WEB/IMS_CORE/SYSL000/js/SYSL145.js` (前端邏輯)

### 1.2 業務需求
- 管理業務報表資料
- 支援業務報表的新增、修改、刪除、查詢
- 支援店別、卡片類型、動作類型、廠商等條件查詢
- 支援管理權限檢查
- 支援重複資料驗證（店別+類型+ID唯一性檢查）
- 記錄業務報表的建立與變更資訊
- 支援報表列印與匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `BusinessReportManagement` (對應舊系統業務報表管理)

```sql
CREATE TABLE [dbo].[BusinessReportManagement] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼 (SITE_ID)
    [Type] NVARCHAR(20) NOT NULL, -- 類型 (TYPE)
    [Id] NVARCHAR(50) NOT NULL, -- ID (ID)
    [UserId] NVARCHAR(50) NULL, -- 使用者編號 (USER_ID)
    [UserName] NVARCHAR(100) NULL, -- 使用者名稱 (USER_NAME)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (CREATED_BY)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (CREATED_AT)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (UPDATED_BY)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (UPDATED_AT)
    [CreatedPriority] INT NULL, -- 建立者等級 (CREATED_PRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CREATED_GROUP)
    CONSTRAINT [PK_BusinessReportManagement] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_BusinessReportManagement_SiteTypeId] UNIQUE ([SiteId], [Type], [Id])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_SiteId] ON [dbo].[BusinessReportManagement] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Type] ON [dbo].[BusinessReportManagement] ([Type]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Id] ON [dbo].[BusinessReportManagement] ([Id]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_UserId] ON [dbo].[BusinessReportManagement] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportManagement_Status] ON [dbo].[BusinessReportManagement] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `Sites` - 店別主檔
- 用於查詢店別列表
- 參考: `開發計劃/02-基本資料管理/04-組織架構/SYSWB60-庫別資料維護作業.md`

#### 2.2.2 `CardTypes` - 卡片類型主檔
- 用於查詢卡片類型列表
- 參考業務報表相關功能

#### 2.2.3 `ActionTypes` - 動作類型主檔
- 用於查詢動作類型列表
- 參考業務報表相關功能

#### 2.2.4 `Users` - 使用者主檔
- 用於查詢使用者列表
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| SiteId | NVARCHAR | 50 | NO | - | 店別代碼 | 外鍵至Sites表，與Type、Id組成唯一約束 |
| Type | NVARCHAR | 20 | NO | - | 類型 | 與SiteId、Id組成唯一約束 |
| Id | NVARCHAR | 50 | NO | - | ID | 與SiteId、Type組成唯一約束 |
| UserId | NVARCHAR | 50 | YES | - | 使用者編號 | 外鍵至Users表 |
| UserName | NVARCHAR | 100 | YES | - | 使用者名稱 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
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

#### 3.1.1 查詢業務報表管理列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-report-management`
- **說明**: 查詢業務報表管理列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TKey",
    "sortOrder": "ASC",
    "filters": {
      "siteId": "",
      "type": "",
      "id": "",
      "userId": "",
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
          "siteId": "SITE001",
          "siteName": "店別名稱",
          "type": "TYPE001",
          "typeName": "類型名稱",
          "id": "ID001",
          "userId": "U001",
          "userName": "使用者名稱",
          "status": "A",
          "notes": "備註",
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

#### 3.1.2 查詢單筆業務報表管理
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-report-management/{tKey}`
- **說明**: 根據主鍵查詢單筆業務報表管理資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "siteId": "SITE001",
      "siteName": "店別名稱",
      "type": "TYPE001",
      "typeName": "類型名稱",
      "id": "ID001",
      "userId": "U001",
      "userName": "使用者名稱",
      "status": "A",
      "notes": "備註",
      "createdBy": "U001",
      "createdAt": "2024-01-01T00:00:00",
      "updatedBy": "U001",
      "updatedAt": "2024-01-01T00:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增業務報表管理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-management`
- **說明**: 新增業務報表管理資料，需檢查管理權限與重複資料
- **請求格式**:
  ```json
  {
    "siteId": "SITE001",
    "type": "TYPE001",
    "id": "ID001",
    "userId": "U001",
    "status": "A",
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
- **錯誤回應**:
  ```json
  {
    "success": false,
    "code": 400,
    "message": "店別+類型+ID已存在",
    "data": {
      "existingUserId": "U002",
      "existingUserName": "現有使用者名稱"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改業務報表管理
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/business-report-management/{tKey}`
- **說明**: 修改業務報表管理資料，需檢查管理權限與重複資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增
- **回應格式**: 同新增

#### 3.1.5 刪除業務報表管理
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/business-report-management/{tKey}`
- **說明**: 刪除業務報表管理資料
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

#### 3.1.6 批次刪除業務報表管理
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/business-report-management/batch`
- **說明**: 批次刪除多筆業務報表管理
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

#### 3.1.7 載入管理權限資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-report-management/load-management`
- **說明**: 載入管理權限資料，用於前端權限檢查
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "載入成功",
    "data": [
      {
        "siteId": "SITE001",
        "type": "TYPE001",
        "id": "ID001",
        "userId": "U001",
        "userName": "使用者名稱"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 檢查重複資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-report-management/check-duplicate`
- **說明**: 檢查店別+類型+ID是否重複
- **請求格式**:
  ```json
  {
    "siteId": "SITE001",
    "type": "TYPE001",
    "id": "ID001",
    "excludeTKey": 1
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檢查完成",
    "data": {
      "isDuplicate": false,
      "existingRecord": null
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 查詢頁面 (`SYSL145Query.vue`)

#### 4.1.1 頁面結構
- 查詢表單（店別、類型、ID、使用者、狀態）
- 查詢結果列表（支援多選、排序、分頁）
- 新增、修改、刪除、批次刪除按鈕
- 報表列印與匯出功能

#### 4.1.2 主要功能
- 支援多條件查詢
- 支援列表排序與分頁
- 支援多選與批次操作
- 支援報表列印與匯出

### 4.2 新增/修改頁面 (`SYSL145Edit.vue`)

#### 4.2.1 頁面結構
- 表單欄位（店別、類型、ID、使用者、狀態、備註）
- 管理權限檢查
- 重複資料驗證
- 儲存、取消按鈕

#### 4.2.2 主要功能
- 表單驗證（必填欄位、格式驗證）
- 管理權限檢查（載入管理權限資料）
- 重複資料驗證（店別+類型+ID唯一性檢查）
- 錯誤提示與處理

### 4.3 報表列印頁面
- 報表列印功能
- 報表匯出功能（Excel、PDF）

---

## 五、業務邏輯說明

### 5.1 管理權限檢查
- 在新增/修改前，需載入管理權限資料
- 檢查使用者是否有權限操作該店別+類型的資料
- 若無權限，顯示錯誤訊息並阻止操作

### 5.2 重複資料驗證
- 在新增/修改時，需檢查店別+類型+ID的唯一性
- 若重複，顯示現有資料的使用者資訊
- 阻止重複資料的新增或修改

### 5.3 資料驗證規則
- 店別、類型、ID為必填欄位
- 店別+類型+ID組合必須唯一
- 使用者編號需存在於使用者主檔
- 狀態必須為有效值（A:啟用, I:停用）

---

## 六、開發注意事項

### 6.1 權限控制
- 需檢查使用者是否有業務報表管理權限
- 需檢查使用者是否有特定店別+類型的操作權限

### 6.2 資料驗證
- 需實作管理權限檢查邏輯
- 需實作重複資料驗證邏輯
- 需提供清楚的錯誤訊息

### 6.3 效能優化
- 管理權限資料可考慮快取
- 查詢結果需支援分頁與索引優化

### 6.4 錯誤處理
- 需處理管理權限檢查失敗的情況
- 需處理重複資料驗證失敗的情況
- 需提供使用者友善的錯誤訊息

---

## 七、測試計劃

### 7.1 單元測試
- 管理權限檢查邏輯測試
- 重複資料驗證邏輯測試
- 資料新增、修改、刪除測試

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
- [SYSL150 - 業務報表列印作業](./SYSL150-業務報表列印作業.md)

