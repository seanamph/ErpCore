# SYSC180 - 潛客 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSC180
- **功能名稱**: 潛客
- **功能描述**: 提供潛在客戶資料的新增、修改、刪除、查詢功能，包含潛客基本資料、聯絡資訊、招商狀態、訪談記錄等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSC000/SYSC180*.asp` (相關功能)
  - `IMS3/HANSHIN/IMS3/SYSC000/SYSC180.ascx` (如存在)

### 1.2 業務需求
- 管理潛在客戶基本資料資訊
- 支援潛客的新增、修改、刪除、查詢
- 記錄潛客的建立與變更資訊
- 支援潛客狀態管理（待訪談、訪談中、已簽約、已取消等）
- 支援潛客與訪談記錄的關聯
- 支援潛客與店別、區域的關聯
- 支援潛客報表查詢與列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Prospects` (對應舊系統 `RIM_PROSPECT` 或類似)

```sql
CREATE TABLE [dbo].[Prospects] (
    [ProspectId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 潛客代碼 (PROSPECT_ID)
    [ProspectName] NVARCHAR(100) NOT NULL, -- 潛客名稱 (PROSPECT_NAME)
    [ContactPerson] NVARCHAR(50) NULL, -- 聯絡人 (CONTACT)
    [ContactTel] NVARCHAR(50) NULL, -- 聯絡電話 (CONTACT_TEL)
    [ContactFax] NVARCHAR(50) NULL, -- 傳真 (CONTACT_FAX)
    [ContactEmail] NVARCHAR(100) NULL, -- 電子郵件 (EMAIL)
    [ContactAddress] NVARCHAR(200) NULL, -- 聯絡地址 (CONTACT_ADDR)
    [StoreName] NVARCHAR(100) NULL, -- 店名 (STORE_NAME)
    [StoreTel] NVARCHAR(50) NULL, -- 店電話 (STORE_TEL)
    [SiteId] NVARCHAR(50) NULL, -- 據點代碼 (SITE_ID)
    [RecruitId] NVARCHAR(50) NULL, -- 招商代碼 (RECRUIT_ID)
    [StoreId] NVARCHAR(50) NULL, -- 店別代碼 (STORE_ID)
    [VendorId] NVARCHAR(50) NULL, -- 廠商代碼 (VENDOR_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [BtypeId] NVARCHAR(50) NULL, -- 業種代碼 (BTYPE_ID)
    [SalesType] NVARCHAR(50) NULL, -- 銷售型態 (SALES_TYPE)
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (STATUS) PENDING:待訪談, INTERVIEWING:訪談中, SIGNED:已簽約, CANCELLED:已取消
    [OverallStatus] NVARCHAR(20) NULL, -- 整體狀態 (OVERALL_STATUS)
    [PaperType] NVARCHAR(20) NULL, -- 營業型態 (PAPER_TYPE)
    [LocationType] NVARCHAR(20) NULL, -- 正櫃櫃別 (LOCATION_TYPE)
    [DecoType] NVARCHAR(20) NULL, -- 裝潢方式 (DECO_TYPE)
    [CommType] NVARCHAR(20) NULL, -- 抽成別 (COMM_TYPE)
    [PdType] NVARCHAR(20) NULL, -- 抽成別 (PD_TYPE)
    [BaseRent] DECIMAL(18,2) NULL, -- 基本租金 (BASE_RENT)
    [Deposit] DECIMAL(18,2) NULL, -- 保證金 (DEPOSIT)
    [CreditCard] NVARCHAR(10) NULL, -- 信用卡 (CREDIT_CARD)
    [TargetAmountM] DECIMAL(18,2) NULL, -- 目標金額(月) (TARGET_AMOUNT_M)
    [TargetAmountV] DECIMAL(18,2) NULL, -- 目標金額(年) (TARGET_AMOUNT_V)
    [ExerciseFees] DECIMAL(18,2) NULL, -- 運動費用 (EXCERCISE_FEES)
    [CheckDay] INT NULL, -- 檢查日 (CHECK_DAY)
    [AgmDateB] DATETIME2 NULL, -- 會議日期起 (AGM_DATE_B)
    [AgmDateE] DATETIME2 NULL, -- 會議日期迄 (AGM_DATE_E)
    [ContractProidB] NVARCHAR(50) NULL, -- 合約專案代碼起 (CONTRACT_PROID_B)
    [ContractProidE] NVARCHAR(50) NULL, -- 合約專案代碼迄 (CONTRACT_PROID_E)
    [FeedbackDate] DATETIME2 NULL, -- 回饋日期 (FEEDBACK_DATE)
    [DueDate] DATETIME2 NULL, -- 到期日 (DUE_DATE)
    [ContactDate] DATETIME2 NULL, -- 聯絡日期 (CONTACT_DATE)
    [VersionNo] NVARCHAR(20) NULL, -- 版本號 (VERSION_NO)
    [GuiId] NVARCHAR(50) NULL, -- GUI ID (GUI_ID)
    [BankId] NVARCHAR(50) NULL, -- 銀行代碼 (BANK_ID)
    [AccName] NVARCHAR(100) NULL, -- 帳戶名稱 (ACC_NAME)
    [AccNo] NVARCHAR(50) NULL, -- 帳戶號碼 (ACC_NO)
    [InvEmail] NVARCHAR(100) NULL, -- 發票電子郵件 (INV_EMAIL)
    [EdcYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否使用業主商店代號 (EDC_YN) Y:是, N:否
    [ReceYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否開立業主發票 (RECE_YN) Y:是, N:否
    [PosYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否使用業主收銀機 (POS_YN) Y:是, N:否
    [CashYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否為業主收現 (CASH_YN) Y:是, N:否
    [CommYn] NVARCHAR(10) NULL DEFAULT 'N', -- 是否為抽成 (COMM_YN) Y:是, N:否
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Prospects] PRIMARY KEY CLUSTERED ([ProspectId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Prospects_ProspectName] ON [dbo].[Prospects] ([ProspectName]);
CREATE NONCLUSTERED INDEX [IX_Prospects_Status] ON [dbo].[Prospects] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Prospects_SiteId] ON [dbo].[Prospects] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Prospects_RecruitId] ON [dbo].[Prospects] ([RecruitId]);
CREATE NONCLUSTERED INDEX [IX_Prospects_StoreId] ON [dbo].[Prospects] ([StoreId]);
CREATE NONCLUSTERED INDEX [IX_Prospects_VendorId] ON [dbo].[Prospects] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_Prospects_ContactDate] ON [dbo].[Prospects] ([ContactDate]);
CREATE NONCLUSTERED INDEX [IX_Prospects_CreatedAt] ON [dbo].[Prospects] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `Interviews` - 訪談記錄
- 用於記錄潛客的訪談資訊
- 參考: `開發計劃/11-招商管理/SYSC222-訪談.md`

#### 2.2.2 `Sites` - 據點主檔
- 用於查詢據點列表
- 參考相關基本資料管理模組

#### 2.2.3 `Vendors` - 廠商主檔
- 用於查詢廠商列表
- 參考: `開發計劃/02-基本資料管理/05-廠商客戶/SYSB206-廠客基本資料維護作業.md`

#### 2.2.4 `Banks` - 銀行主檔
- 用於查詢銀行列表
- 參考: `開發計劃/02-基本資料管理/03-金融機構/SYSBC20-銀行基本資料維護作業.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ProspectId | NVARCHAR | 50 | NO | - | 潛客代碼 | 主鍵 |
| ProspectName | NVARCHAR | 100 | NO | - | 潛客名稱 | - |
| ContactPerson | NVARCHAR | 50 | YES | - | 聯絡人 | - |
| ContactTel | NVARCHAR | 50 | YES | - | 聯絡電話 | - |
| ContactFax | NVARCHAR | 50 | YES | - | 傳真 | - |
| ContactEmail | NVARCHAR | 100 | YES | - | 電子郵件 | - |
| ContactAddress | NVARCHAR | 200 | YES | - | 聯絡地址 | - |
| StoreName | NVARCHAR | 100 | YES | - | 店名 | - |
| StoreTel | NVARCHAR | 50 | YES | - | 店電話 | - |
| SiteId | NVARCHAR | 50 | YES | - | 據點代碼 | 外鍵至Sites表 |
| RecruitId | NVARCHAR | 50 | YES | - | 招商代碼 | - |
| StoreId | NVARCHAR | 50 | YES | - | 店別代碼 | 外鍵至Stores表 |
| VendorId | NVARCHAR | 50 | YES | - | 廠商代碼 | 外鍵至Vendors表 |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | 外鍵至Organizations表 |
| BtypeId | NVARCHAR | 50 | YES | - | 業種代碼 | - |
| SalesType | NVARCHAR | 50 | YES | - | 銷售型態 | - |
| Status | NVARCHAR | 20 | NO | 'PENDING' | 狀態 | PENDING:待訪談, INTERVIEWING:訪談中, SIGNED:已簽約, CANCELLED:已取消 |
| OverallStatus | NVARCHAR | 20 | YES | - | 整體狀態 | - |
| PaperType | NVARCHAR | 20 | YES | - | 營業型態 | - |
| LocationType | NVARCHAR | 20 | YES | - | 正櫃櫃別 | - |
| DecoType | NVARCHAR | 20 | YES | - | 裝潢方式 | - |
| CommType | NVARCHAR | 20 | YES | - | 抽成別 | - |
| PdType | NVARCHAR | 20 | YES | - | 抽成別 | - |
| BaseRent | DECIMAL | 18,2 | YES | - | 基本租金 | - |
| Deposit | DECIMAL | 18,2 | YES | - | 保證金 | - |
| CreditCard | NVARCHAR | 10 | YES | - | 信用卡 | - |
| TargetAmountM | DECIMAL | 18,2 | YES | - | 目標金額(月) | - |
| TargetAmountV | DECIMAL | 18,2 | YES | - | 目標金額(年) | - |
| ExerciseFees | DECIMAL | 18,2 | YES | - | 運動費用 | - |
| CheckDay | INT | - | YES | - | 檢查日 | - |
| AgmDateB | DATETIME2 | - | YES | - | 會議日期起 | - |
| AgmDateE | DATETIME2 | - | YES | - | 會議日期迄 | - |
| ContractProidB | NVARCHAR | 50 | YES | - | 合約專案代碼起 | - |
| ContractProidE | NVARCHAR | 50 | YES | - | 合約專案代碼迄 | - |
| FeedbackDate | DATETIME2 | - | YES | - | 回饋日期 | - |
| DueDate | DATETIME2 | - | YES | - | 到期日 | - |
| ContactDate | DATETIME2 | - | YES | - | 聯絡日期 | - |
| VersionNo | NVARCHAR | 20 | YES | - | 版本號 | - |
| GuiId | NVARCHAR | 50 | YES | - | GUI ID | - |
| BankId | NVARCHAR | 50 | YES | - | 銀行代碼 | 外鍵至Banks表 |
| AccName | NVARCHAR | 100 | YES | - | 帳戶名稱 | - |
| AccNo | NVARCHAR | 50 | YES | - | 帳戶號碼 | - |
| InvEmail | NVARCHAR | 100 | YES | - | 發票電子郵件 | - |
| EdcYn | NVARCHAR | 10 | YES | 'N' | 是否使用業主商店代號 | Y:是, N:否 |
| ReceYn | NVARCHAR | 10 | YES | 'N' | 是否開立業主發票 | Y:是, N:否 |
| PosYn | NVARCHAR | 10 | YES | 'N' | 是否使用業主收銀機 | Y:是, N:否 |
| CashYn | NVARCHAR | 10 | YES | 'N' | 是否為業主收現 | Y:是, N:否 |
| CommYn | NVARCHAR | 10 | YES | 'N' | 是否為抽成 | Y:是, N:否 |
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

#### 3.1.1 查詢潛客列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/prospects`
- **說明**: 查詢潛客列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ProspectName",
    "sortOrder": "ASC",
    "filters": {
      "prospectName": "",
      "status": "",
      "siteId": "",
      "recruitId": "",
      "contactDateFrom": "",
      "contactDateTo": ""
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
          "prospectId": "P001",
          "prospectName": "潛客名稱",
          "contactPerson": "聯絡人",
          "contactTel": "02-12345678",
          "status": "PENDING",
          "contactDate": "2024-01-01T00:00:00",
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

#### 3.1.2 查詢單筆潛客
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/prospects/{prospectId}`
- **說明**: 根據潛客代碼查詢單筆潛客資料
- **路徑參數**:
  - `prospectId`: 潛客代碼
- **回應格式**: 包含完整潛客資料

#### 3.1.3 新增潛客
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/prospects`
- **說明**: 新增潛客資料
- **請求格式**:
  ```json
  {
    "prospectName": "潛客名稱",
    "contactPerson": "聯絡人",
    "contactTel": "02-12345678",
    "contactEmail": "contact@example.com",
    "status": "PENDING",
    "siteId": "SITE001",
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
      "prospectId": "P001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改潛客
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/prospects/{prospectId}`
- **說明**: 修改潛客資料
- **路徑參數**:
  - `prospectId`: 潛客代碼
- **請求格式**: 同新增，但 `prospectId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除潛客
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/prospects/{prospectId}`
- **說明**: 刪除潛客資料（需檢查是否有關聯的訪談記錄）
- **路徑參數**:
  - `prospectId`: 潛客代碼
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

#### 3.1.6 批次刪除潛客
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/prospects/batch`
- **說明**: 批次刪除多筆潛客
- **請求格式**:
  ```json
  {
    "items": ["P001", "P002"]
  }
  ```

#### 3.1.7 更新潛客狀態
- **HTTP 方法**: `PATCH`
- **路徑**: `/api/v1/prospects/{prospectId}/status`
- **說明**: 更新潛客狀態
- **請求格式**:
  ```json
  {
    "status": "SIGNED"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 潛客列表頁面 (`ProspectList.vue`)
- **路徑**: `/recruitment/prospects`
- **功能**: 顯示潛客列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (ProspectSearchForm)
  - 資料表格 (ProspectDataTable)
  - 新增/修改對話框 (ProspectDialog)
  - 刪除確認對話框

#### 4.1.2 潛客詳細頁面 (`ProspectDetail.vue`)
- **路徑**: `/recruitment/prospects/:prospectId`
- **功能**: 顯示潛客詳細資料，支援修改、查看訪談記錄

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ProspectSearchForm.vue`)
- 支援依潛客名稱、狀態、據點、招商代碼、聯絡日期範圍查詢

#### 4.2.2 資料表格元件 (`ProspectDataTable.vue`)
- 顯示潛客基本資訊、狀態、聯絡資訊
- 支援排序、分頁
- 提供修改、刪除、查看詳細資料操作

#### 4.2.3 新增/修改對話框 (`ProspectDialog.vue`)
- 表單欄位包含：潛客名稱、聯絡人、聯絡資訊、據點、狀態等
- 支援表單驗證

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
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

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 潛客資料變更必須記錄操作日誌
- 敏感資訊必須加密儲存

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 關聯查詢應使用 JOIN 優化

### 6.3 資料驗證
- 必填欄位必須驗證
- 狀態值必須在允許範圍內
- 日期範圍必須驗證
- 金額欄位必須驗證格式

### 6.4 業務邏輯
- 刪除潛客前必須檢查是否有關聯的訪談記錄
- 狀態變更必須符合業務流程
- 潛客資料變更必須記錄變更資訊

### 6.5 關聯資料
- 與訪談記錄的關聯
- 與據點、店別、廠商的關聯
- 與銀行資料的關聯

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增潛客成功
- [ ] 新增潛客失敗 (必填欄位驗證)
- [ ] 修改潛客成功
- [ ] 修改潛客失敗 (狀態驗證)
- [ ] 刪除潛客成功
- [ ] 刪除潛客失敗 (有關聯資料)
- [ ] 查詢潛客列表成功
- [ ] 查詢單筆潛客成功
- [ ] 更新潛客狀態成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯資料測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 關聯查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSC000/SYSC180*.asp` (如存在)
- `IMS3/HANSHIN/IMS3/SYSC000/SYSC180.ascx` (如存在)

### 8.2 資料庫 Schema
- 舊系統相關資料表結構

---

