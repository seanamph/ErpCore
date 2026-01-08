# SYSC165 - 潛客主檔 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSC165
- **功能名稱**: 潛客主檔
- **功能描述**: 提供潛客主檔資料的新增、修改、刪除、查詢功能，作為潛客管理的基礎資料維護，包含潛客基本資訊、分類、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSC000/SYSC165*.asp` (相關功能)
  - `IMS3/HANSHIN/IMS3/SYSC000/SYSC165.ascx` (如存在)

### 1.2 業務需求
- 管理潛客主檔基本資料資訊
- 支援潛客主檔的新增、修改、刪除、查詢
- 記錄潛客主檔的建立與變更資訊
- 支援潛客分類管理
- 支援潛客狀態管理
- 作為潛客管理（SYSC180）的基礎資料
- 支援潛客主檔報表查詢與列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ProspectMasters` (對應舊系統 `RIM_PROSPECT_MASTER` 或類似)

```sql
CREATE TABLE [dbo].[ProspectMasters] (
    [MasterId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 主檔代碼 (MASTER_ID)
    [MasterName] NVARCHAR(100) NOT NULL, -- 主檔名稱 (MASTER_NAME)
    [MasterType] NVARCHAR(20) NULL, -- 主檔類型 (MASTER_TYPE) COMPANY:公司, INDIVIDUAL:個人, OTHER:其他
    [Category] NVARCHAR(50) NULL, -- 分類 (CATEGORY)
    [Industry] NVARCHAR(50) NULL, -- 產業別 (INDUSTRY)
    [BusinessType] NVARCHAR(50) NULL, -- 業種 (BUSINESS_TYPE)
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', -- 狀態 (STATUS) ACTIVE:有效, INACTIVE:無效, ARCHIVED:已歸檔
    [Priority] INT NULL DEFAULT 0, -- 優先順序 (PRIORITY)
    [Source] NVARCHAR(50) NULL, -- 來源 (SOURCE) REFERRAL:推薦, ADVERTISEMENT:廣告, EXHIBITION:展覽, OTHER:其他
    [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT_PERSON)
    [ContactTel] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_TEL)
    [ContactEmail] NVARCHAR(100) NULL, -- 電子郵件 (CONTACT_EMAIL)
    [ContactAddress] NVARCHAR(200) NULL, -- 聯絡地址 (CONTACT_ADDRESS)
    [Website] NVARCHAR(200) NULL, -- 網站 (WEBSITE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_ProspectMasters] PRIMARY KEY CLUSTERED ([MasterId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_MasterName] ON [dbo].[ProspectMasters] ([MasterName]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_MasterType] ON [dbo].[ProspectMasters] ([MasterType]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Category] ON [dbo].[ProspectMasters] ([Category]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Industry] ON [dbo].[ProspectMasters] ([Industry]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_BusinessType] ON [dbo].[ProspectMasters] ([BusinessType]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Status] ON [dbo].[ProspectMasters] ([Status]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_Source] ON [dbo].[ProspectMasters] ([Source]);
CREATE NONCLUSTERED INDEX [IX_ProspectMasters_CreatedAt] ON [dbo].[ProspectMasters] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `Prospects` - 潛客資料
- 潛客資料可能關聯到潛客主檔
- 參考: `開發計劃/11-招商管理/SYSC180-潛客.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| MasterId | NVARCHAR | 50 | NO | - | 主檔代碼 | 主鍵 |
| MasterName | NVARCHAR | 100 | NO | - | 主檔名稱 | - |
| MasterType | NVARCHAR | 20 | YES | - | 主檔類型 | COMPANY:公司, INDIVIDUAL:個人, OTHER:其他 |
| Category | NVARCHAR | 50 | YES | - | 分類 | - |
| Industry | NVARCHAR | 50 | YES | - | 產業別 | - |
| BusinessType | NVARCHAR | 50 | YES | - | 業種 | - |
| Status | NVARCHAR | 20 | NO | 'ACTIVE' | 狀態 | ACTIVE:有效, INACTIVE:無效, ARCHIVED:已歸檔 |
| Priority | INT | - | YES | 0 | 優先順序 | - |
| Source | NVARCHAR | 50 | YES | - | 來源 | REFERRAL:推薦, ADVERTISEMENT:廣告, EXHIBITION:展覽, OTHER:其他 |
| ContactPerson | NVARCHAR | 50 | YES | - | 聯絡人 | - |
| ContactTel | NVARCHAR | 50 | YES | - | 聯絡電話 | - |
| ContactEmail | NVARCHAR | 100 | YES | - | 電子郵件 | - |
| ContactAddress | NVARCHAR | 200 | YES | - | 聯絡地址 | - |
| Website | NVARCHAR | 200 | YES | - | 網站 | - |
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

#### 3.1.1 查詢潛客主檔列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/prospect-masters`
- **說明**: 查詢潛客主檔列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "MasterName",
    "sortOrder": "ASC",
    "filters": {
      "masterName": "",
      "masterType": "",
      "category": "",
      "industry": "",
      "businessType": "",
      "status": "",
      "source": ""
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
          "masterId": "M001",
          "masterName": "主檔名稱",
          "masterType": "COMPANY",
          "category": "分類",
          "status": "ACTIVE",
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

#### 3.1.2 查詢單筆潛客主檔
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/prospect-masters/{masterId}`
- **說明**: 根據主檔代碼查詢單筆潛客主檔資料
- **路徑參數**:
  - `masterId`: 主檔代碼
- **回應格式**: 包含完整潛客主檔資料

#### 3.1.3 新增潛客主檔
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/prospect-masters`
- **說明**: 新增潛客主檔資料
- **請求格式**:
  ```json
  {
    "masterName": "主檔名稱",
    "masterType": "COMPANY",
    "category": "分類",
    "industry": "產業別",
    "businessType": "業種",
    "status": "ACTIVE",
    "source": "REFERRAL",
    "contactPerson": "聯絡人",
    "contactTel": "02-12345678",
    "contactEmail": "contact@example.com",
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
      "masterId": "M001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改潛客主檔
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/prospect-masters/{masterId}`
- **說明**: 修改潛客主檔資料
- **路徑參數**:
  - `masterId`: 主檔代碼
- **請求格式**: 同新增，但 `masterId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除潛客主檔
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/prospect-masters/{masterId}`
- **說明**: 刪除潛客主檔資料（需檢查是否有關聯的潛客資料）
- **路徑參數**:
  - `masterId`: 主檔代碼
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

#### 3.1.6 批次刪除潛客主檔
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/prospect-masters/batch`
- **說明**: 批次刪除多筆潛客主檔
- **請求格式**:
  ```json
  {
    "items": ["M001", "M002"]
  }
  ```

#### 3.1.7 更新潛客主檔狀態
- **HTTP 方法**: `PATCH`
- **路徑**: `/api/v1/prospect-masters/{masterId}/status`
- **說明**: 更新潛客主檔狀態
- **請求格式**:
  ```json
  {
    "status": "ARCHIVED"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 潛客主檔列表頁面 (`ProspectMasterList.vue`)
- **路徑**: `/recruitment/prospect-masters`
- **功能**: 顯示潛客主檔列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ProspectMasterSearchForm)
  - 資料表格 (ProspectMasterDataTable)
  - 新增/修改對話框 (ProspectMasterDialog)
  - 刪除確認對話框

#### 4.1.2 潛客主檔詳細頁面 (`ProspectMasterDetail.vue`)
- **路徑**: `/recruitment/prospect-masters/:masterId`
- **功能**: 顯示潛客主檔詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ProspectMasterSearchForm.vue`)
- 支援依主檔名稱、主檔類型、分類、產業別、業種、狀態、來源查詢

#### 4.2.2 資料表格元件 (`ProspectMasterDataTable.vue`)
- 顯示主檔基本資訊、類型、分類、狀態
- 支援排序、分頁
- 提供修改、刪除、查看詳細資料操作

#### 4.2.3 新增/修改對話框 (`ProspectMasterDialog.vue`)
- 表單欄位包含：主檔名稱、主檔類型、分類、產業別、業種、狀態、來源、聯絡資訊等
- 支援表單驗證

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 潛客主檔資料變更必須記錄操作日誌
- 敏感資訊必須加密儲存

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 分類查詢應使用索引優化

### 6.3 資料驗證
- 必填欄位必須驗證
- 主檔代碼必須唯一
- 狀態值必須在允許範圍內
- 主檔類型值必須在允許範圍內

### 6.4 業務邏輯
- 刪除潛客主檔前必須檢查是否有關聯的潛客資料
- 狀態變更必須符合業務流程
- 潛客主檔資料變更必須記錄變更資訊

### 6.5 關聯資料
- 與潛客資料的關聯（如適用）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增潛客主檔成功
- [ ] 新增潛客主檔失敗 (必填欄位驗證)
- [ ] 修改潛客主檔成功
- [ ] 修改潛客主檔失敗 (狀態驗證)
- [ ] 刪除潛客主檔成功
- [ ] 刪除潛客主檔失敗 (有關聯資料)
- [ ] 查詢潛客主檔列表成功
- [ ] 查詢單筆潛客主檔成功
- [ ] 更新潛客主檔狀態成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯資料測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 分類查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSC000/SYSC165*.asp` (如存在)
- `IMS3/HANSHIN/IMS3/SYSC000/SYSC165.ascx` (如存在)

### 8.2 資料庫 Schema
- 舊系統相關資料表結構

---

