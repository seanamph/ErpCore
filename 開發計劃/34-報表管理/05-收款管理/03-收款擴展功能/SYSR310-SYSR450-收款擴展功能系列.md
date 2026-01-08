# SYSR310-SYSR450 - 收款擴展功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSR310-SYSR450 系列
- **功能名稱**: 收款擴展功能系列
- **功能描述**: 提供拋轉收款沖帳傳票、收款查詢與報表等擴展功能，包含收款沖帳處理、收款查詢、收款報表等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR310_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR310_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR410_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR410_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR420_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR420_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR425_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR425_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR427_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR427_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR430_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR430_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR440_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR440_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR450_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR450_PR.ASP` (報表)

### 1.2 業務需求
- 提供拋轉收款沖帳傳票功能
- 支援收款查詢功能
- 支援收款報表查詢與列印
- 支援多種收款查詢條件
- 支援收款統計分析
- 支援收款資料匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReceiptVoucherTransfer` (收款沖帳傳票)

```sql
CREATE TABLE [dbo].[ReceiptVoucherTransfer] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ReceiptNo] NVARCHAR(50) NOT NULL, -- 收款單號 (RECEIPT_NO)
    [ReceiptDate] DATETIME2 NOT NULL, -- 收款日期 (RECEIPT_DATE)
    [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
    [AcctKey] NVARCHAR(100) NULL, -- 對帳KEY值 (ACCT_KEY)
    [ReceiptAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 收款金額 (RECEIPT_AMOUNT)
    [AritemId] NVARCHAR(50) NULL, -- 收款項目代號 (ARITEM_ID)
    [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
    [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
    [TransferStatus] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 拋轉狀態 (TRANSFER_STATUS, P:待拋轉, S:已拋轉, F:失敗)
    [TransferDate] DATETIME2 NULL, -- 拋轉日期 (TRANSFER_DATE)
    [TransferUser] NVARCHAR(50) NULL, -- 拋轉人員 (TRANSFER_USER)
    [ErrorMessage] NVARCHAR(500) NULL, -- 錯誤訊息 (ERROR_MESSAGE)
    [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReceiptVoucherTransfer] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ReceiptNo] ON [dbo].[ReceiptVoucherTransfer] ([ReceiptNo]);
CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_ReceiptDate] ON [dbo].[ReceiptVoucherTransfer] ([ReceiptDate]);
CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_TransferStatus] ON [dbo].[ReceiptVoucherTransfer] ([TransferStatus]);
CREATE NONCLUSTERED INDEX [IX_ReceiptVoucherTransfer_VoucherNo] ON [dbo].[ReceiptVoucherTransfer] ([VoucherNo]);
```

### 2.2 資料字典

#### 2.2.1 ReceiptVoucherTransfer 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ReceiptNo | NVARCHAR | 50 | NO | - | 收款單號 | RECEIPT_NO |
| ReceiptDate | DATETIME2 | - | NO | - | 收款日期 | RECEIPT_DATE |
| ObjectId | NVARCHAR | 50 | YES | - | 對象別編號 | OBJECT_ID |
| AcctKey | NVARCHAR | 100 | YES | - | 對帳KEY值 | ACCT_KEY |
| ReceiptAmount | DECIMAL | 18,4 | NO | 0 | 收款金額 | RECEIPT_AMOUNT |
| AritemId | NVARCHAR | 50 | YES | - | 收款項目代號 | ARITEM_ID |
| VoucherNo | NVARCHAR | 50 | YES | - | 傳票單號 | VOUCHER_NO |
| VoucherM_TKey | BIGINT | - | YES | - | 傳票主檔KEY值 | VOUCHERM_T_KEY |
| TransferStatus | NVARCHAR | 10 | NO | 'P' | 拋轉狀態 | P:待拋轉, S:已拋轉, F:失敗 |
| TransferDate | DATETIME2 | - | YES | - | 拋轉日期 | TRANSFER_DATE |
| TransferUser | NVARCHAR | 50 | YES | - | 拋轉人員 | TRANSFER_USER |
| ErrorMessage | NVARCHAR | 500 | YES | - | 錯誤訊息 | ERROR_MESSAGE |
| ShopId | NVARCHAR | 50 | YES | - | 分店代碼 | SHOP_ID |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | SITE_ID |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | ORG_ID |
| Notes | NVARCHAR | 500 | YES | - | 備註 | NOTES |

---

## 三、後端API設計

### 3.1 拋轉收款沖帳傳票 API

#### 3.1.1 請求
- **URL**: `POST /api/v1/receipt/vouchertransfer`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.1.2 請求參數
```json
{
  "receiptNo": "RCP20240101001",
  "receiptDate": "2024-01-01",
  "objectId": "OBJ001",
  "acctKey": "ACCT001",
  "receiptAmount": 10000.00,
  "aritemId": "01",
  "shopId": "SHOP001",
  "siteId": "SITE001",
  "orgId": "ORG001",
  "notes": "備註說明"
}
```

#### 3.1.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "拋轉成功",
  "data": {
    "tKey": 1,
    "receiptNo": "RCP20240101001",
    "receiptDate": "2024-01-01T00:00:00Z",
    "objectId": "OBJ001",
    "acctKey": "ACCT001",
    "receiptAmount": 10000.00,
    "aritemId": "01",
    "voucherNo": "VCH20240101001",
    "voucherM_TKey": 100,
    "transferStatus": "S",
    "transferDate": "2024-01-01T00:00:00Z",
    "transferUser": "USER001",
    "shopId": "SHOP001",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "notes": "備註說明"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.2 批次拋轉收款沖帳傳票 API

#### 3.2.1 請求
- **URL**: `POST /api/v1/receipt/vouchertransfer/batch`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.2.2 請求參數
```json
{
  "receiptNos": ["RCP20240101001", "RCP20240101002", "RCP20240101003"]
}
```

#### 3.2.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "批次拋轉完成",
  "data": {
    "total": 3,
    "success": 2,
    "failed": 1,
    "results": [
      {
        "receiptNo": "RCP20240101001",
        "status": "success",
        "voucherNo": "VCH20240101001"
      },
      {
        "receiptNo": "RCP20240101002",
        "status": "success",
        "voucherNo": "VCH20240101002"
      },
      {
        "receiptNo": "RCP20240101003",
        "status": "failed",
        "errorMessage": "收款單號不存在"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.3 查詢收款沖帳傳票 API

#### 3.3.1 請求
- **URL**: `GET /api/v1/receipt/vouchertransfer`
- **Headers**: 
  - `Authorization: Bearer {token}`
- **Query Parameters**:
  - `receiptNo` (可選): 收款單號（模糊查詢）
  - `voucherNo` (可選): 傳票單號（模糊查詢）
  - `receiptDateFrom` (可選): 收款日期起
  - `receiptDateTo` (可選): 收款日期迄
  - `transferStatus` (可選): 拋轉狀態
  - `objectId` (可選): 對象別編號
  - `shopId` (可選): 分店代碼
  - `page` (可選): 頁碼，預設 1
  - `pageSize` (可選): 每頁筆數，預設 20
  - `sortBy` (可選): 排序欄位，預設 ReceiptDate
  - `sortOrder` (可選): 排序方向，預設 DESC

#### 3.3.2 回應
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "receiptNo": "RCP20240101001",
        "receiptDate": "2024-01-01T00:00:00Z",
        "objectId": "OBJ001",
        "objectName": "客戶A",
        "acctKey": "ACCT001",
        "receiptAmount": 10000.00,
        "aritemId": "01",
        "aritemName": "現金收款",
        "voucherNo": "VCH20240101001",
        "voucherM_TKey": 100,
        "transferStatus": "S",
        "transferStatusName": "已拋轉",
        "transferDate": "2024-01-01T00:00:00Z",
        "transferUser": "USER001",
        "shopId": "SHOP001",
        "shopName": "分店A",
        "siteId": "SITE001",
        "orgId": "ORG001",
        "notes": "備註說明"
      }
    ],
    "total": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.4 收款報表查詢 API

#### 3.4.1 請求
- **URL**: `GET /api/v1/receipt/reports`
- **Headers**: 
  - `Authorization: Bearer {token}`
- **Query Parameters**:
  - `reportType` (必填): 報表類型 (SYSR410, SYSR420, SYSR425, SYSR427, SYSR430, SYSR440, SYSR450)
  - `receiptDateFrom` (可選): 收款日期起
  - `receiptDateTo` (可選): 收款日期迄
  - `objectId` (可選): 對象別編號
  - `aritemId` (可選): 收款項目代號
  - `shopId` (可選): 分店代碼
  - `siteId` (可選): 分公司代碼
  - `orgId` (可選): 組織代碼

#### 3.4.2 回應
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "reportType": "SYSR410",
    "reportName": "收款明細報表",
    "summary": {
      "totalAmount": 100000.00,
      "totalCount": 10
    },
    "items": [
      {
        "receiptNo": "RCP20240101001",
        "receiptDate": "2024-01-01T00:00:00Z",
        "objectId": "OBJ001",
        "objectName": "客戶A",
        "receiptAmount": 10000.00,
        "aritemId": "01",
        "aritemName": "現金收款",
        "shopId": "SHOP001",
        "shopName": "分店A"
      }
    ]
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.5 收款報表列印 API

#### 3.5.1 請求
- **URL**: `POST /api/v1/receipt/reports/print`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.5.2 請求參數
```json
{
  "reportType": "SYSR450",
  "receiptDateFrom": "2024-01-01",
  "receiptDateTo": "2024-01-31",
  "objectId": "OBJ001",
  "shopId": "SHOP001"
}
```

#### 3.5.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "報表產生成功",
  "data": {
    "reportId": "RPT20240101001",
    "reportUrl": "/api/v1/receipt/reports/download/RPT20240101001",
    "fileName": "收款報表_20240101_20240131.pdf"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## 四、前端UI設計

### 4.1 收款沖帳傳票列表頁面

#### 4.1.1 頁面結構
- **路由**: `/receipt/vouchertransfer`
- **組件**: `ReceiptVoucherTransferList.vue`

#### 4.1.2 功能區塊
1. **查詢條件區**
   - 收款單號（文字輸入，模糊查詢）
   - 傳票單號（文字輸入，模糊查詢）
   - 收款日期起（日期選擇器）
   - 收款日期迄（日期選擇器）
   - 拋轉狀態（下拉選單）
   - 對象別編號（下拉選單）
   - 分店代碼（下拉選單）
   - 查詢按鈕
   - 重置按鈕

2. **資料列表區**
   - 表格顯示收款沖帳傳票資料
   - 欄位：收款單號、收款日期、對象別、收款金額、收款項目、傳票單號、拋轉狀態、拋轉日期、操作
   - 支援排序功能
   - 支援分頁功能

3. **操作按鈕區**
   - 拋轉按鈕（選取單筆或多筆）
   - 批次拋轉按鈕
   - 查詢按鈕
   - 匯出按鈕
   - 列印按鈕

### 4.2 收款報表查詢頁面

#### 4.2.1 頁面結構
- **路由**: `/receipt/reports`
- **組件**: `ReceiptReportsQuery.vue`

#### 4.2.2 功能區塊
1. **報表類型選擇區**
   - 報表類型（下拉選單，必填）
     - SYSR410: 收款明細報表
     - SYSR420: 收款統計報表
     - SYSR425: 收款分析報表
     - SYSR427: 收款彙總報表
     - SYSR430: 收款對帳報表
     - SYSR440: 收款項目報表
     - SYSR450: 收款綜合報表

2. **查詢條件區**
   - 收款日期起（日期選擇器）
   - 收款日期迄（日期選擇器）
   - 對象別編號（下拉選單）
   - 收款項目（下拉選單）
   - 分店代碼（下拉選單）
   - 分公司代碼（下拉選單）
   - 組織代碼（下拉選單）
   - 查詢按鈕
   - 重置按鈕

3. **報表結果區**
   - 報表摘要資訊
   - 報表明細資料（表格顯示）
   - 支援匯出功能
   - 支援列印功能

### 4.3 UI組件規格

#### 4.3.1 表格組件
- 使用 Element Plus 的 `el-table` 組件
- 支援欄位排序
- 支援多選功能
- 支援欄位寬度調整
- 金額欄位右對齊顯示

#### 4.3.2 表單組件
- 使用 Element Plus 的 `el-form` 組件
- 表單驗證使用 `el-form-item` 的 `rules` 屬性
- 必填欄位標示紅色星號
- 日期欄位使用日期選擇器

#### 4.3.3 報表組件
- 使用 Element Plus 的 `el-card` 組件顯示報表摘要
- 使用 Element Plus 的 `el-table` 組件顯示報表明細
- 支援報表列印功能
- 支援報表匯出功能（Excel、PDF）

---

## 五、開發時程

### 5.1 後端開發 (5天)
- Day 1: 資料庫設計與建立、Entity 與 Repository 開發
- Day 2: Service 與 Controller 開發、拋轉功能實作
- Day 3: 批次拋轉功能實作、API 測試
- Day 4: 報表查詢功能實作
- Day 5: 單元測試、整合測試

### 5.2 前端開發 (5天)
- Day 1: 收款沖帳傳票列表頁面開發、查詢功能實作
- Day 2: 拋轉功能實作、批次拋轉功能實作
- Day 3: 報表查詢頁面開發、報表類型選擇實作
- Day 4: 報表結果顯示實作、報表列印功能實作
- Day 5: 報表匯出功能實作、UI優化

### 5.3 測試與優化 (2天)
- Day 1: 功能測試、效能測試
- Day 2: Bug修復、文件整理

**總計**: 12天

---

## 六、注意事項

### 6.1 資料驗證
- 收款單號必填
- 收款日期必填
- 收款金額必填且必須大於0
- 拋轉前需檢查收款單是否存在

### 6.2 權限控制
- 拋轉功能需要對應的權限
- 查詢功能需要讀取權限
- 報表功能需要報表權限

### 6.3 效能優化
- 查詢結果使用分頁，避免一次載入過多資料
- 批次拋轉使用非同步處理
- 報表查詢使用快取機制
- 建立適當的資料庫索引

### 6.4 錯誤處理
- 拋轉失敗時記錄錯誤訊息
- 批次拋轉時部分失敗需記錄詳細資訊
- 報表查詢無資料時提示使用者

### 6.5 業務邏輯
- 拋轉時需產生傳票單號
- 拋轉狀態需即時更新
- 報表資料需與實際資料一致

---

## 七、測試案例

### 7.1 拋轉測試
1. **正常拋轉**
   - 選擇收款單進行拋轉
   - 驗證拋轉成功，傳票單號產生

2. **批次拋轉**
   - 選擇多筆收款單進行批次拋轉
   - 驗證批次拋轉結果正確

3. **拋轉失敗**
   - 選擇不存在的收款單進行拋轉
   - 驗證系統提示錯誤訊息

### 7.2 查詢測試
1. **基本查詢**
   - 不輸入任何條件
   - 驗證顯示所有資料（分頁）

2. **條件查詢**
   - 輸入各種查詢條件
   - 驗證查詢結果正確

3. **日期範圍查詢**
   - 使用收款日期起迄進行查詢
   - 驗證查詢結果正確

### 7.3 報表測試
1. **報表查詢**
   - 選擇報表類型並輸入查詢條件
   - 驗證報表資料正確

2. **報表列印**
   - 產生報表並列印
   - 驗證報表格式正確

3. **報表匯出**
   - 匯出報表為Excel或PDF
   - 驗證匯出檔案正確

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSR000/SYSR310_FB.ASP` - 拋轉收款沖帳傳票瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR310_FQ.ASP` - 拋轉收款沖帳傳票查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR410_FB.ASP` - 收款報表瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR410_FQ.ASP` - 收款報表查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR450_FQ.ASP` - 收款報表查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR450_PR.ASP` - 收款報表列印畫面

### 8.2 資料表參考
- `AR` - 應收帳款主檔（Oracle）
- `VOUCHER_M` - 傳票主檔（Oracle）

### 8.3 相關功能
- 收款基礎功能 (SYSR110-SYSR120)
- 收款處理功能 (SYSR210-SYSR240)
- 傳票管理功能 (SYST121-SYST12B)

