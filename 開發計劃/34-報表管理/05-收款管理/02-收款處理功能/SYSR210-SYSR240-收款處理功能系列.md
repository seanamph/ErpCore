# SYSR210-SYSR240 - 收款處理功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSR210-SYSR240 系列
- **功能名稱**: 收款處理功能系列
- **功能描述**: 提供應收帳款維護的新增、修改、刪除、查詢功能，包含傳票單號、收款日期、對象別編號、對帳KEY值、收款金額、收款項目等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR210_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR220_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR240_PR.ASP` (報表)

### 1.2 業務需求
- 管理應收帳款基本資料
- 支援傳票單號管理
- 支援收款日期管理
- 支援對象別編號管理
- 支援對帳KEY值管理
- 支援收款金額計算
- 支援收款項目選擇
- 支援傳票狀態管理
- 支援多店別管理
- 支援收款單查詢與報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `AccountsReceivable` (應收帳款主檔)

```sql
CREATE TABLE [dbo].[AccountsReceivable] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
    [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
    [AcctKey] NVARCHAR(100) NULL, -- 對帳KEY值 (ACCT_KEY)
    [ReceiptDate] DATETIME2 NULL, -- 收款日期 (RECEIPT_DATE)
    [ReceiptAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 收款金額 (RECEIPT_AMOUNT)
    [AritemId] NVARCHAR(50) NULL, -- 收款項目代號 (ARITEM_ID)
    [ReceiptNo] NVARCHAR(50) NULL, -- 收款單號 (RECEIPT_NO)
    [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
    [VoucherStatus] NVARCHAR(10) NULL, -- 傳票狀態 (VOUCHER_STATUS)
    [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CurrencyId] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別 (CURRENCY_ID)
    [ExchangeRate] DECIMAL(18, 6) NULL DEFAULT 1, -- 匯率 (EXCHANGE_RATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_AccountsReceivable] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_VoucherM_TKey] ON [dbo].[AccountsReceivable] ([VoucherM_TKey]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ObjectId] ON [dbo].[AccountsReceivable] ([ObjectId]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_AcctKey] ON [dbo].[AccountsReceivable] ([AcctKey]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ReceiptDate] ON [dbo].[AccountsReceivable] ([ReceiptDate]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ReceiptNo] ON [dbo].[AccountsReceivable] ([ReceiptNo]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_VoucherNo] ON [dbo].[AccountsReceivable] ([VoucherNo]);
CREATE NONCLUSTERED INDEX [IX_AccountsReceivable_ShopId] ON [dbo].[AccountsReceivable] ([ShopId]);
```

### 2.2 相關資料表

#### 2.2.1 `VoucherM` - 傳票主檔（參考）
```sql
-- 傳票主檔結構（參考，實際結構需根據系統設計）
-- 主要欄位：T_KEY, VOUCHER_NO, VOUCHER_STATUS 等
```

### 2.3 資料字典

#### 2.3.1 AccountsReceivable 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| VoucherM_TKey | BIGINT | - | YES | - | 傳票主檔KEY值 | VOUCHERM_T_KEY，外鍵至傳票主檔 |
| ObjectId | NVARCHAR | 50 | YES | - | 對象別編號 | OBJECT_ID |
| AcctKey | NVARCHAR | 100 | YES | - | 對帳KEY值 | ACCT_KEY |
| ReceiptDate | DATETIME2 | - | YES | - | 收款日期 | RECEIPT_DATE |
| ReceiptAmount | DECIMAL | 18,4 | YES | 0 | 收款金額 | RECEIPT_AMOUNT |
| AritemId | NVARCHAR | 50 | YES | - | 收款項目代號 | ARITEM_ID，外鍵至收款項目表 |
| ReceiptNo | NVARCHAR | 50 | YES | - | 收款單號 | RECEIPT_NO |
| VoucherNo | NVARCHAR | 50 | YES | - | 傳票單號 | VOUCHER_NO |
| VoucherStatus | NVARCHAR | 10 | YES | - | 傳票狀態 | VOUCHER_STATUS |
| ShopId | NVARCHAR | 50 | YES | - | 分店代碼 | SHOP_ID |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | SITE_ID |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | ORG_ID |
| CurrencyId | NVARCHAR | 10 | YES | 'TWD' | 幣別 | CURRENCY_ID |
| ExchangeRate | DECIMAL | 18,6 | YES | 1 | 匯率 | EXCHANGE_RATE |
| Notes | NVARCHAR | 500 | YES | - | 備註 | NOTES |

---

## 三、後端API設計

### 3.1 新增應收帳款 API

#### 3.1.1 請求
- **URL**: `POST /api/v1/receipt/accountsreceivable`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.1.2 請求參數
```json
{
  "objectId": "OBJ001",
  "acctKey": "ACCT001",
  "receiptDate": "2024-01-01",
  "receiptAmount": 10000.00,
  "aritemId": "01",
  "receiptNo": "RCP20240101001",
  "voucherNo": "VCH20240101001",
  "shopId": "SHOP001",
  "siteId": "SITE001",
  "orgId": "ORG001",
  "currencyId": "TWD",
  "exchangeRate": 1.0,
  "notes": "備註說明"
}
```

#### 3.1.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "新增成功",
  "data": {
    "tKey": 1,
    "objectId": "OBJ001",
    "acctKey": "ACCT001",
    "receiptDate": "2024-01-01T00:00:00Z",
    "receiptAmount": 10000.00,
    "aritemId": "01",
    "aritemName": "現金收款",
    "receiptNo": "RCP20240101001",
    "voucherNo": "VCH20240101001",
    "voucherStatus": "D",
    "shopId": "SHOP001",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "notes": "備註說明",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.2 修改應收帳款 API

#### 3.2.1 請求
- **URL**: `PUT /api/v1/receipt/accountsreceivable/{tKey}`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.2.2 請求參數
```json
{
  "receiptDate": "2024-01-02",
  "receiptAmount": 15000.00,
  "aritemId": "02",
  "notes": "備註說明(修改)"
}
```

#### 3.2.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "修改成功",
  "data": {
    "tKey": 1,
    "objectId": "OBJ001",
    "acctKey": "ACCT001",
    "receiptDate": "2024-01-02T00:00:00Z",
    "receiptAmount": 15000.00,
    "aritemId": "02",
    "aritemName": "銀行轉帳",
    "receiptNo": "RCP20240101001",
    "voucherNo": "VCH20240101001",
    "voucherStatus": "D",
    "notes": "備註說明(修改)",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-02T00:00:00Z"
  },
  "timestamp": "2024-01-02T00:00:00Z"
}
```

### 3.3 刪除應收帳款 API

#### 3.3.1 請求
- **URL**: `DELETE /api/v1/receipt/accountsreceivable/{tKey}`
- **Headers**: 
  - `Authorization: Bearer {token}`

#### 3.3.2 回應
```json
{
  "success": true,
  "code": 200,
  "message": "刪除成功",
  "data": null,
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.4 查詢應收帳款 API

#### 3.4.1 請求
- **URL**: `GET /api/v1/receipt/accountsreceivable`
- **Headers**: 
  - `Authorization: Bearer {token}`
- **Query Parameters**:
  - `objectId` (可選): 對象別編號
  - `acctKey` (可選): 對帳KEY值
  - `receiptNo` (可選): 收款單號（模糊查詢）
  - `voucherNo` (可選): 傳票單號（模糊查詢）
  - `receiptDateFrom` (可選): 收款日期起
  - `receiptDateTo` (可選): 收款日期迄
  - `voucherStatus` (可選): 傳票狀態
  - `shopId` (可選): 分店代碼
  - `page` (可選): 頁碼，預設 1
  - `pageSize` (可選): 每頁筆數，預設 20
  - `sortBy` (可選): 排序欄位，預設 TKey
  - `sortOrder` (可選): 排序方向，預設 DESC

#### 3.4.2 回應
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "voucherM_TKey": 100,
        "objectId": "OBJ001",
        "objectName": "客戶A",
        "acctKey": "ACCT001",
        "receiptDate": "2024-01-01T00:00:00Z",
        "receiptAmount": 10000.00,
        "aritemId": "01",
        "aritemName": "現金收款",
        "receiptNo": "RCP20240101001",
        "voucherNo": "VCH20240101001",
        "voucherStatus": "D",
        "voucherStatusName": "草稿",
        "shopId": "SHOP001",
        "shopName": "分店A",
        "siteId": "SITE001",
        "orgId": "ORG001",
        "currencyId": "TWD",
        "exchangeRate": 1.0,
        "notes": "備註說明",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedBy": "USER001",
        "updatedAt": "2024-01-01T00:00:00Z"
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

### 3.5 取得單筆應收帳款 API

#### 3.5.1 請求
- **URL**: `GET /api/v1/receipt/accountsreceivable/{tKey}`
- **Headers**: 
  - `Authorization: Bearer {token}`

#### 3.5.2 回應
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "tKey": 1,
    "voucherM_TKey": 100,
    "objectId": "OBJ001",
    "objectName": "客戶A",
    "acctKey": "ACCT001",
    "receiptDate": "2024-01-01T00:00:00Z",
    "receiptAmount": 10000.00,
    "aritemId": "01",
    "aritemName": "現金收款",
    "receiptNo": "RCP20240101001",
    "voucherNo": "VCH20240101001",
    "voucherStatus": "D",
    "voucherStatusName": "草稿",
    "shopId": "SHOP001",
    "shopName": "分店A",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "currencyId": "TWD",
    "exchangeRate": 1.0,
    "notes": "備註說明",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## 四、前端UI設計

### 4.1 應收帳款列表頁面

#### 4.1.1 頁面結構
- **路由**: `/receipt/accountsreceivable`
- **組件**: `ReceiptAccountsReceivableList.vue`

#### 4.1.2 功能區塊
1. **查詢條件區**
   - 對象別編號（下拉選單）
   - 對帳KEY值（文字輸入）
   - 收款單號（文字輸入，模糊查詢）
   - 傳票單號（文字輸入，模糊查詢）
   - 收款日期起（日期選擇器）
   - 收款日期迄（日期選擇器）
   - 傳票狀態（下拉選單）
   - 分店代碼（下拉選單）
   - 查詢按鈕
   - 重置按鈕

2. **資料列表區**
   - 表格顯示應收帳款資料
   - 欄位：收款單號、傳票單號、傳票狀態、對象別、對帳KEY值、收款日期、收款金額、收款項目、分店、操作
   - 支援排序功能
   - 支援分頁功能

3. **操作按鈕區**
   - 新增按鈕
   - 修改按鈕（選取單筆）
   - 刪除按鈕（選取單筆或多筆）
   - 匯出按鈕
   - 列印按鈕

### 4.2 應收帳款新增/修改頁面

#### 4.2.1 頁面結構
- **路由**: `/receipt/accountsreceivable/add` (新增) / `/receipt/accountsreceivable/edit/:id` (修改)
- **組件**: `ReceiptAccountsReceivableForm.vue`

#### 4.2.2 表單欄位
1. **基本資料區**
   - 對象別編號（下拉選單，必填）
   - 對帳KEY值（文字輸入，可選）
   - 收款日期（日期選擇器，必填）
   - 收款金額（數字輸入，必填）
   - 收款項目（下拉選單，必填）
   - 收款單號（文字輸入，新增時自動產生或手動輸入，修改時唯讀）
   - 傳票單號（文字輸入，可選）
   - 傳票狀態（下拉選單，唯讀，顯示用）
   - 分店代碼（下拉選單，必填）
   - 分公司代碼（下拉選單，可選）
   - 組織代碼（下拉選單，可選）
   - 幣別（下拉選單，預設TWD）
   - 匯率（數字輸入，預設1.0）
   - 備註（文字輸入，多行，可選）

2. **操作按鈕區**
   - 儲存按鈕
   - 取消按鈕
   - 返回按鈕

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

#### 4.3.3 下拉選單組件
- 使用 Element Plus 的 `el-select` 組件
- 支援搜尋功能
- 支援遠端搜尋（如對象別、收款項目）

---

## 五、開發時程

### 5.1 後端開發 (4天)
- Day 1: 資料庫設計與建立、Entity 與 Repository 開發
- Day 2: Service 與 Controller 開發、API 測試
- Day 3: 傳票整合功能開發
- Day 4: 單元測試、整合測試

### 5.2 前端開發 (4天)
- Day 1: 列表頁面開發、查詢功能實作
- Day 2: 新增/修改頁面開發、表單驗證實作
- Day 3: 刪除功能實作、傳票狀態顯示實作
- Day 4: 報表功能實作、UI優化

### 5.3 測試與優化 (2天)
- Day 1: 功能測試、效能測試
- Day 2: Bug修復、文件整理

**總計**: 10天

---

## 六、注意事項

### 6.1 資料驗證
- 收款日期必填
- 收款金額必填且必須大於0
- 收款項目必填
- 傳票單號如果存在，必須存在於傳票主檔中

### 6.2 權限控制
- 新增、修改、刪除功能需要對應的權限
- 查詢功能需要讀取權限
- 報表功能需要報表權限
- 只有狀態為「建立」的傳票才可刪除

### 6.3 效能優化
- 查詢結果使用分頁，避免一次載入過多資料
- 對象別、收款項目下拉選單使用快取機制
- 建立適當的資料庫索引
- 傳票狀態查詢使用索引優化

### 6.4 錯誤處理
- 新增時檢查收款單號是否重複
- 修改時檢查資料是否存在
- 刪除時檢查傳票狀態，只有「建立」狀態才可刪除
- 傳票單號驗證，確保存在於傳票主檔中

### 6.5 業務邏輯
- 收款單號自動產生規則需符合業務需求
- 傳票狀態需與傳票主檔同步
- 收款金額需與傳票金額一致（如有傳票）

---

## 七、測試案例

### 7.1 新增測試
1. **正常新增**
   - 輸入完整的應收帳款資料
   - 驗證新增成功，資料正確寫入資料庫

2. **重複收款單號**
   - 輸入已存在的收款單號
   - 驗證系統提示錯誤訊息

3. **必填欄位驗證**
   - 不輸入必填欄位
   - 驗證系統提示必填欄位錯誤

4. **金額驗證**
   - 輸入負數或零
   - 驗證系統提示金額錯誤

### 7.2 修改測試
1. **正常修改**
   - 修改應收帳款資料
   - 驗證修改成功，資料正確更新

2. **不存在的資料**
   - 修改不存在的應收帳款
   - 驗證系統提示錯誤訊息

3. **傳票狀態限制**
   - 修改已結案的傳票相關資料
   - 驗證系統提示無法修改

### 7.3 刪除測試
1. **正常刪除**
   - 刪除狀態為「建立」的應收帳款
   - 驗證刪除成功，資料從資料庫移除

2. **傳票狀態限制**
   - 刪除狀態非「建立」的應收帳款
   - 驗證系統提示無法刪除

### 7.4 查詢測試
1. **基本查詢**
   - 不輸入任何條件
   - 驗證顯示所有資料（分頁）

2. **條件查詢**
   - 輸入各種查詢條件
   - 驗證查詢結果正確

3. **日期範圍查詢**
   - 使用收款日期起迄進行查詢
   - 驗證查詢結果正確

4. **模糊查詢**
   - 使用收款單號或傳票單號進行模糊查詢
   - 驗證查詢結果正確

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FB.ASP` - 應收帳款瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FI.ASP` - 應收帳款新增畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FU.ASP` - 應收帳款修改畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FD.ASP` - 應收帳款刪除畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_FQ.ASP` - 應收帳款查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR210_PR.ASP` - 應收帳款報表畫面

### 8.2 資料表參考
- `AR` - 應收帳款主檔（Oracle）
- `VOUCHER_M` - 傳票主檔（Oracle）
- `AR_ITEM` - 收款項目主檔（Oracle）

### 8.3 相關功能
- 收款基礎功能 (SYSR110-SYSR120)
- 收款擴展功能 (SYSR310-SYSR450)
- 傳票管理功能 (SYST121-SYST12B)

