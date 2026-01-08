# CMS2310-CMS2320 - CMS合同維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: CMS2310-CMS2320 系列
- **功能名稱**: CMS合同維護系列
- **功能描述**: 提供CMS合同資料的新增、修改、刪除、查詢功能，包含CMS合同編號、合同類型、廠商、簽約日期、生效日期、到期日期、合同金額、合同狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2310_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FB.ASP` (瀏覽)

### 1.2 業務需求
- 管理CMS合同基本資料
- 支援CMS合同類型管理
- 支援廠商選擇
- 支援合同版本管理
- 支援合同狀態管理（草稿、審核中、已生效、已到期、已終止）
- 支援合同金額計算
- 支援合同審核流程
- 支援合同報表列印
- 支援合同歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CmsContracts` (CMS合同主檔)

```sql
CREATE TABLE [dbo].[CmsContracts] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CmsContractId] NVARCHAR(50) NOT NULL, -- CMS合同編號 (CMS_CONTRACT_ID)
    [ContractType] NVARCHAR(20) NOT NULL, -- 合同類型 (CONTRACT_TYPE)
    [Version] INT NOT NULL DEFAULT 1, -- 版本號 (VERSION)
    [VendorId] NVARCHAR(50) NOT NULL, -- 廠商代碼 (VENDOR_ID)
    [VendorName] NVARCHAR(200) NULL, -- 廠商名稱 (VENDOR_NAME)
    [SignDate] DATETIME2 NULL, -- 簽約日期 (SIGN_DATE)
    [EffectiveDate] DATETIME2 NULL, -- 生效日期 (EFFECTIVE_DATE)
    [ExpiryDate] DATETIME2 NULL, -- 到期日期 (EXPIRY_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, P:審核中, A:已生效, E:已到期, T:已終止)
    [TotalAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 總金額 (TOTAL_AMT)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [LocationId] NVARCHAR(50) NULL, -- 位置編號 (LOCATION_ID)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_CmsContracts] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_CmsContracts_CmsContractId_Version] UNIQUE ([CmsContractId], [Version])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CmsContracts_CmsContractId] ON [dbo].[CmsContracts] ([CmsContractId]);
CREATE NONCLUSTERED INDEX [IX_CmsContracts_VendorId] ON [dbo].[CmsContracts] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_CmsContracts_Status] ON [dbo].[CmsContracts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_CmsContracts_ContractType] ON [dbo].[CmsContracts] ([ContractType]);
CREATE NONCLUSTERED INDEX [IX_CmsContracts_EffectiveDate] ON [dbo].[CmsContracts] ([EffectiveDate]);
CREATE NONCLUSTERED INDEX [IX_CmsContracts_ExpiryDate] ON [dbo].[CmsContracts] ([ExpiryDate]);
```

### 2.2 相關資料表

#### 2.2.1 `CmsContractTerms` - CMS合同條款
```sql
CREATE TABLE [dbo].[CmsContractTerms] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CmsContractId] NVARCHAR(50) NOT NULL, -- CMS合同編號
    [Version] INT NOT NULL, -- 版本號
    [TermType] NVARCHAR(50) NULL, -- 條款類型 (TERM_TYPE)
    [TermContent] NVARCHAR(MAX) NULL, -- 條款內容 (TERM_CONTENT)
    [TermOrder] INT NULL, -- 條款順序 (TERM_ORDER)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_CmsContractTerms_CmsContracts] FOREIGN KEY ([CmsContractId], [Version]) REFERENCES [dbo].[CmsContracts] ([CmsContractId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CmsContractTerms_CmsContractId] ON [dbo].[CmsContractTerms] ([CmsContractId], [Version]);
```

### 2.3 資料字典

#### 2.3.1 CmsContracts 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| CmsContractId | NVARCHAR | 50 | NO | - | CMS合同編號 | 唯一，與版本號組合唯一 |
| ContractType | NVARCHAR | 20 | NO | - | 合同類型 | - |
| Version | INT | - | NO | 1 | 版本號 | - |
| VendorId | NVARCHAR | 50 | NO | - | 廠商代碼 | 外鍵至供應商表 |
| VendorName | NVARCHAR | 200 | YES | - | 廠商名稱 | - |
| SignDate | DATETIME2 | - | YES | - | 簽約日期 | - |
| EffectiveDate | DATETIME2 | - | YES | - | 生效日期 | - |
| ExpiryDate | DATETIME2 | - | YES | - | 到期日期 | - |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, P:審核中, A:已生效, E:已到期, T:已終止 |
| TotalAmount | DECIMAL | 18,4 | YES | 0 | 總金額 | - |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | - |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | - |
| LocationId | NVARCHAR | 50 | YES | - | 位置編號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢CMS合同列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cms-contracts`
- **說明**: 查詢CMS合同列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CmsContractId",
    "sortOrder": "ASC",
    "filters": {
      "cmsContractId": "",
      "vendorId": "",
      "contractType": "",
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
          "cmsContractId": "CMS001",
          "contractType": "TYPE001",
          "version": 1,
          "vendorId": "V001",
          "vendorName": "供應商A",
          "signDate": "2024-01-01",
          "effectiveDate": "2024-01-01",
          "expiryDate": "2024-12-31",
          "status": "A",
          "totalAmount": 1000000
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

#### 3.1.2 查詢單筆CMS合同
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cms-contracts/{tKey}`
- **說明**: 根據主鍵查詢單筆CMS合同資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 新增CMS合同
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cms-contracts`
- **說明**: 新增CMS合同資料
- **請求格式**:
  ```json
  {
    "cmsContractId": "CMS001",
    "contractType": "TYPE001",
    "version": 1,
    "vendorId": "V001",
    "vendorName": "供應商A",
    "signDate": "2024-01-01",
    "effectiveDate": "2024-01-01",
    "expiryDate": "2024-12-31",
    "status": "D",
    "totalAmount": 1000000,
    "currencyId": "TWD",
    "exchangeRate": 1,
    "locationId": "LOC001",
    "memo": "備註"
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

#### 3.1.4 修改CMS合同
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cms-contracts/{tKey}`
- **說明**: 修改CMS合同資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 和 `cmsContractId` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除CMS合同
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cms-contracts/{tKey}`
- **說明**: 刪除CMS合同資料
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

#### 3.1.6 批次刪除CMS合同
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cms-contracts/batch`
- **說明**: 批次刪除多筆CMS合同
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

---

## 四、前端 UI 設計

### 4.1 CMS合同維護頁面

#### 4.1.1 列表頁面
- **路由**: `/cms-contracts`
- **功能**: 顯示CMS合同列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單（CMS合同編號、廠商、合同類型、狀態）
  - 資料表格（顯示CMS合同列表）
  - 操作按鈕（新增、修改、刪除、查詢、報表）

#### 4.1.2 新增/修改頁面
- **路由**: `/cms-contracts/add` 或 `/cms-contracts/edit/:tKey`
- **功能**: 新增或修改CMS合同資料
- **主要元件**:
  - 表單欄位（CMS合同編號、合同類型、版本號、廠商、簽約日期、生效日期、到期日期、狀態、總金額、幣別、匯率、位置編號、備註）
  - 操作按鈕（儲存、取消）

#### 4.1.3 查詢頁面
- **路由**: `/cms-contracts/query`
- **功能**: 查詢CMS合同資料
- **主要元件**:
  - 查詢條件表單
  - 查詢結果表格
  - 匯出功能

#### 4.1.4 報表頁面
- **路由**: `/cms-contracts/report`
- **功能**: 顯示CMS合同報表
- **主要元件**:
  - 報表查詢條件
  - 報表顯示區域
  - 列印、匯出功能

---

## 五、開發時程

### 5.1 階段一：資料庫設計與API開發（3天）
- 資料表設計與建立
- API 端點開發（新增、修改、刪除、查詢）
- API 測試

### 5.2 階段二：前端UI開發（3天）
- 列表頁面開發
- 新增/修改頁面開發
- 查詢頁面開發
- 報表頁面開發

### 5.3 階段三：整合測試（2天）
- 前後端整合測試
- 功能測試
- 效能測試

### 5.4 階段四：文件與部署（1天）
- 文件整理
- 部署準備

**總計**: 約 9 個工作天

---

## 六、注意事項

### 6.1 資料驗證
- CMS合同編號必須唯一（與版本號組合）
- 廠商代碼必須存在於供應商表
- 生效日期必須在簽約日期之後
- 到期日期必須在生效日期之後
- 總金額必須大於等於0

### 6.2 權限控制
- 新增、修改、刪除需要相應權限
- 查詢、報表需要查詢權限

### 6.3 效能優化
- CMS合同列表查詢需要建立適當索引
- 大量資料查詢需要分頁處理

### 6.4 資料一致性
- 刪除CMS合同時，相關的合同條款資料需要一併處理（級聯刪除）

---

## 七、測試案例

### 7.1 新增測試
- **測試案例1**: 新增一筆有效的CMS合同資料
  - **預期結果**: 新增成功，返回新建立的主鍵
- **測試案例2**: 新增一筆CMS合同編號重複的資料
  - **預期結果**: 新增失敗，返回錯誤訊息

### 7.2 修改測試
- **測試案例1**: 修改一筆存在的CMS合同資料
  - **預期結果**: 修改成功
- **測試案例2**: 修改一筆不存在的主鍵
  - **預期結果**: 修改失敗，返回錯誤訊息

### 7.3 刪除測試
- **測試案例1**: 刪除一筆存在的CMS合同資料
  - **預期結果**: 刪除成功，相關的合同條款一併刪除
- **測試案例2**: 刪除一筆不存在的主鍵
  - **預期結果**: 刪除失敗，返回錯誤訊息

### 7.4 查詢測試
- **測試案例1**: 查詢所有CMS合同資料
  - **預期結果**: 返回所有資料列表
- **測試案例2**: 根據CMS合同編號查詢
  - **預期結果**: 返回符合條件的資料列表

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FI.ASP` - 新增功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FU.ASP` - 修改功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FD.ASP` - 刪除功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FQ.ASP` - 查詢功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_FB.ASP` - 瀏覽功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2310_PR.ASP` - 報表功能
- `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FI.ASP` - 新增功能（CMS2320）
- `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FU.ASP` - 修改功能（CMS2320）
- `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FD.ASP` - 刪除功能（CMS2320）
- `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FQ.ASP` - 查詢功能（CMS2320）
- `WEB/IMS_CORE/ASP/SYSF000/CMS2320_FB.ASP` - 瀏覽功能（CMS2320）

### 8.2 相關文件
- [合同資料維護系列開發計劃](../01-合同基礎功能/SYSF110-SYSF140-合同資料維護系列.md)
- [合同處理作業系列開發計劃](../02-合同處理功能/SYSF210-SYSF220-合同處理作業系列.md)
- [合同擴展維護系列開發計劃](../03-合同擴展功能/SYSF350-SYSF540-合同擴展維護系列.md)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

