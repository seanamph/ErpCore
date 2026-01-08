# SYSF350-SYSF540 - 合同擴展維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSF350-SYSF540 系列
- **功能名稱**: 合同擴展維護系列
- **功能描述**: 提供合同擴展資料的新增、修改、刪除、查詢功能，包含合同擴展資訊、供應商資訊、傳輸功能、詳細報表等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF350_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF410_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF410_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF510_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF520_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF530_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF540_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理合同擴展資料
- 支援供應商資訊維護
- 支援合同傳輸功能
- 支援合同詳細報表
- 支援合同複製功能
- 支援批量新增功能
- 支援合同擴展查詢與報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ContractExtensions` (合同擴展主檔)

```sql
CREATE TABLE [dbo].[ContractExtensions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號
    [Version] INT NOT NULL DEFAULT 1, -- 版本號
    [ExtensionType] NVARCHAR(20) NULL, -- 擴展類型 (EXT_TYPE)
    [VendorId] NVARCHAR(50) NULL, -- 供應商代碼
    [VendorName] NVARCHAR(200) NULL, -- 供應商名稱
    [ExtensionDate] DATETIME2 NULL, -- 擴展日期 (EXT_DATE)
    [ExtensionAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 擴展金額 (EXT_AMT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ContractExtensions_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ContractExtensions_ContractId] ON [dbo].[ContractExtensions] ([ContractId], [Version]);
CREATE NONCLUSTERED INDEX [IX_ContractExtensions_VendorId] ON [dbo].[ContractExtensions] ([VendorId]);
CREATE NONCLUSTERED INDEX [IX_ContractExtensions_Status] ON [dbo].[ContractExtensions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `ContractTransfers` - 合同傳輸記錄
```sql
CREATE TABLE [dbo].[ContractTransfers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ContractId] NVARCHAR(50) NOT NULL,
    [Version] INT NOT NULL,
    [TransferType] NVARCHAR(20) NULL, -- 傳輸類型 (TRANSFER_TYPE)
    [TransferDate] DATETIME2 NOT NULL, -- 傳輸日期 (TRANSFER_DATE)
    [TransferStatus] NVARCHAR(10) NULL, -- 傳輸狀態 (TRANSFER_STATUS)
    [TransferResult] NVARCHAR(MAX) NULL, -- 傳輸結果 (TRANSFER_RESULT)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ContractTransfers_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version]) ON DELETE CASCADE
);
```

### 2.3 資料字典

#### 2.3.1 ContractExtensions 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ContractId | NVARCHAR | 50 | NO | - | 合同編號 | 外鍵至Contracts |
| Version | INT | - | NO | 1 | 版本號 | - |
| ExtensionType | NVARCHAR | 20 | YES | - | 擴展類型 | - |
| VendorId | NVARCHAR | 50 | YES | - | 供應商代碼 | 外鍵至供應商表 |
| VendorName | NVARCHAR | 200 | YES | - | 供應商名稱 | - |
| ExtensionDate | DATETIME2 | - | YES | - | 擴展日期 | - |
| ExtensionAmount | DECIMAL | 18,4 | YES | 0 | 擴展金額 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢合同擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contracts/extensions`
- **說明**: 查詢合同擴展列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ContractId",
    "sortOrder": "ASC",
    "filters": {
      "contractId": "",
      "vendorId": "",
      "extensionType": "",
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
          "contractId": "CT001",
          "version": 1,
          "extensionType": "EXT001",
          "vendorId": "V001",
          "vendorName": "供應商A",
          "extensionDate": "2024-01-01",
          "extensionAmount": 100000,
          "status": "A"
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

#### 3.1.2 查詢單筆合同擴展
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contracts/extensions/{tKey}`
- **說明**: 根據主鍵查詢單筆合同擴展資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 新增合同擴展
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contracts/extensions`
- **說明**: 新增合同擴展資料
- **請求格式**:
  ```json
  {
    "contractId": "CT001",
    "version": 1,
    "extensionType": "EXT001",
    "vendorId": "V001",
    "vendorName": "供應商A",
    "extensionDate": "2024-01-01",
    "extensionAmount": 100000,
    "status": "A",
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

#### 3.1.4 修改合同擴展
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contracts/extensions/{tKey}`
- **說明**: 修改合同擴展資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除合同擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/contracts/extensions/{tKey}`
- **說明**: 刪除合同擴展資料
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

#### 3.1.6 批次刪除合同擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/contracts/extensions/batch`
- **說明**: 批次刪除多筆合同擴展
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

#### 3.1.7 合同傳輸
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contracts/{contractId}/transfer`
- **說明**: 執行合同傳輸功能
- **路徑參數**:
  - `contractId`: 合同編號
- **請求格式**:
  ```json
  {
    "transferType": "TYPE001",
    "transferDate": "2024-01-01"
  }
  ```

#### 3.1.8 合同複製
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contracts/{contractId}/copy`
- **說明**: 複製合同資料
- **路徑參數**:
  - `contractId`: 合同編號
- **請求格式**:
  ```json
  {
    "newContractId": "CT002",
    "copyTerms": true,
    "copyExtensions": true
  }
  ```

---

## 四、前端 UI 設計

### 4.1 合同擴展維護頁面

#### 4.1.1 列表頁面
- **路由**: `/contracts/extensions`
- **功能**: 顯示合同擴展列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單（合同編號、供應商、擴展類型、狀態）
  - 資料表格（顯示合同擴展列表）
  - 操作按鈕（新增、修改、刪除、查詢、報表）

#### 4.1.2 新增/修改頁面
- **路由**: `/contracts/extensions/add` 或 `/contracts/extensions/edit/:tKey`
- **功能**: 新增或修改合同擴展資料
- **主要元件**:
  - 表單欄位（合同編號、版本號、擴展類型、供應商、擴展日期、擴展金額、狀態、備註）
  - 操作按鈕（儲存、取消）

#### 4.1.3 查詢頁面
- **路由**: `/contracts/extensions/query`
- **功能**: 查詢合同擴展資料
- **主要元件**:
  - 查詢條件表單
  - 查詢結果表格
  - 匯出功能

#### 4.1.4 報表頁面
- **路由**: `/contracts/extensions/report`
- **功能**: 顯示合同擴展報表
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
- 合同編號必須存在於合同主檔
- 供應商代碼必須存在於供應商表
- 擴展金額必須大於等於0
- 擴展日期必須在合同生效日期之後

### 6.2 權限控制
- 新增、修改、刪除需要相應權限
- 查詢、報表需要查詢權限

### 6.3 效能優化
- 合同擴展列表查詢需要建立適當索引
- 大量資料查詢需要分頁處理

### 6.4 資料一致性
- 刪除合同時，相關的合同擴展資料需要一併處理（級聯刪除或檢查）

---

## 七、測試案例

### 7.1 新增測試
- **測試案例1**: 新增一筆有效的合同擴展資料
  - **預期結果**: 新增成功，返回新建立的主鍵
- **測試案例2**: 新增一筆合同編號不存在的資料
  - **預期結果**: 新增失敗，返回錯誤訊息

### 7.2 修改測試
- **測試案例1**: 修改一筆存在的合同擴展資料
  - **預期結果**: 修改成功
- **測試案例2**: 修改一筆不存在的主鍵
  - **預期結果**: 修改失敗，返回錯誤訊息

### 7.3 刪除測試
- **測試案例1**: 刪除一筆存在的合同擴展資料
  - **預期結果**: 刪除成功
- **測試案例2**: 刪除一筆不存在的主鍵
  - **預期結果**: 刪除失敗，返回錯誤訊息

### 7.4 查詢測試
- **測試案例1**: 查詢所有合同擴展資料
  - **預期結果**: 返回所有資料列表
- **測試案例2**: 根據合同編號查詢
  - **預期結果**: 返回符合條件的資料列表

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FI.ASP` - 新增功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FU.ASP` - 修改功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FD.ASP` - 刪除功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FQ.ASP` - 查詢功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_FB.ASP` - 瀏覽功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF350_PR.ASP` - 報表功能
- `WEB/IMS_CORE/ASP/SYSF000/SYSF410_FI.ASP` - 新增功能（SYSF410）
- `WEB/IMS_CORE/ASP/SYSF000/SYSF410_FU.ASP` - 修改功能（SYSF410）
- `WEB/IMS_CORE/ASP/SYSF000/SYSF510_FQ.ASP` - 查詢功能（SYSF510）
- `WEB/IMS_CORE/ASP/SYSF000/SYSF520_FQ.ASP` - 查詢功能（SYSF520）
- `WEB/IMS_CORE/ASP/SYSF000/SYSF530_FQ.ASP` - 查詢功能（SYSF530）
- `WEB/IMS_CORE/ASP/SYSF000/SYSF540_FQ.ASP` - 查詢功能（SYSF540）

### 8.2 相關文件
- [合同資料維護系列開發計劃](../01-合同基礎功能/SYSF110-SYSF140-合同資料維護系列.md)
- [合同處理作業系列開發計劃](../02-合同處理功能/SYSF210-SYSF220-合同處理作業系列.md)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

