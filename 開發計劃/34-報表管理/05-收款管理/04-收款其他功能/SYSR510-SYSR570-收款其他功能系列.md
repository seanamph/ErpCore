# SYSR510-SYSR570 - 收款其他功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSR510-SYSR570 系列
- **功能名稱**: 收款其他功能系列
- **功能描述**: 提供保證金維護、存款功能、收款查詢與報表等其他收款相關功能，包含保證金資料維護、存款處理、收款查詢、收款報表等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR510_DEPOSIT.ASP` (存款)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR520_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR520_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR530_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR540_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR550_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR550_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR550_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR550_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR550_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR560_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR570_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR570_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理保證金基本資料
- 支援保證金新增、修改、刪除、查詢
- 支援存款功能
- 支援保證金查詢與報表
- 支援多種收款查詢功能
- 支援收款資料維護

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Deposits` (保證金主檔)

```sql
CREATE TABLE [dbo].[Deposits] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DepositNo] NVARCHAR(50) NOT NULL, -- 保證金單號 (DEPOSIT_NO)
    [DepositDate] DATETIME2 NOT NULL, -- 保證金日期 (DEPOSIT_DATE)
    [ObjectId] NVARCHAR(50) NULL, -- 對象別編號 (OBJECT_ID)
    [DepositAmount] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 保證金金額 (DEPOSIT_AMOUNT)
    [DepositType] NVARCHAR(20) NULL, -- 保證金類型 (DEPOSIT_TYPE)
    [DepositStatus] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 保證金狀態 (DEPOSIT_STATUS, A:有效, R:退還, C:取消)
    [ReturnDate] DATETIME2 NULL, -- 退還日期 (RETURN_DATE)
    [ReturnAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 退還金額 (RETURN_AMOUNT)
    [VoucherNo] NVARCHAR(50) NULL, -- 傳票單號 (VOUCHER_NO)
    [VoucherM_TKey] BIGINT NULL, -- 傳票主檔KEY值 (VOUCHERM_T_KEY)
    [VoucherStatus] NVARCHAR(10) NULL, -- 傳票狀態 (VOUCHER_STATUS)
    [CheckDueDate] DATETIME2 NULL, -- 票據到期日 (CHECK_DUE_DATE)
    [ShopId] NVARCHAR(50) NULL, -- 分店代碼 (SHOP_ID)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Deposits] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Deposits_DepositNo] UNIQUE ([DepositNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Deposits_DepositNo] ON [dbo].[Deposits] ([DepositNo]);
CREATE NONCLUSTERED INDEX [IX_Deposits_DepositDate] ON [dbo].[Deposits] ([DepositDate]);
CREATE NONCLUSTERED INDEX [IX_Deposits_ObjectId] ON [dbo].[Deposits] ([ObjectId]);
CREATE NONCLUSTERED INDEX [IX_Deposits_DepositStatus] ON [dbo].[Deposits] ([DepositStatus]);
CREATE NONCLUSTERED INDEX [IX_Deposits_VoucherNo] ON [dbo].[Deposits] ([VoucherNo]);
CREATE NONCLUSTERED INDEX [IX_Deposits_ShopId] ON [dbo].[Deposits] ([ShopId]);
```

### 2.2 資料字典

#### 2.2.1 Deposits 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| DepositNo | NVARCHAR | 50 | NO | - | 保證金單號 | DEPOSIT_NO，唯一 |
| DepositDate | DATETIME2 | - | NO | - | 保證金日期 | DEPOSIT_DATE |
| ObjectId | NVARCHAR | 50 | YES | - | 對象別編號 | OBJECT_ID |
| DepositAmount | DECIMAL | 18,4 | NO | 0 | 保證金金額 | DEPOSIT_AMOUNT |
| DepositType | NVARCHAR | 20 | YES | - | 保證金類型 | DEPOSIT_TYPE |
| DepositStatus | NVARCHAR | 10 | NO | 'A' | 保證金狀態 | A:有效, R:退還, C:取消 |
| ReturnDate | DATETIME2 | - | YES | - | 退還日期 | RETURN_DATE |
| ReturnAmount | DECIMAL | 18,4 | YES | 0 | 退還金額 | RETURN_AMOUNT |
| VoucherNo | NVARCHAR | 50 | YES | - | 傳票單號 | VOUCHER_NO |
| VoucherM_TKey | BIGINT | - | YES | - | 傳票主檔KEY值 | VOUCHERM_T_KEY |
| VoucherStatus | NVARCHAR | 10 | YES | - | 傳票狀態 | VOUCHER_STATUS |
| CheckDueDate | DATETIME2 | - | YES | - | 票據到期日 | CHECK_DUE_DATE |
| ShopId | NVARCHAR | 50 | YES | - | 分店代碼 | SHOP_ID |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | SITE_ID |
| OrgId | NVARCHAR | 50 | YES | - | 組織代碼 | ORG_ID |
| Notes | NVARCHAR | 500 | YES | - | 備註 | NOTES |

---

## 三、後端API設計

### 3.1 新增保證金 API

#### 3.1.1 請求
- **URL**: `POST /api/v1/receipt/deposits`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.1.2 請求參數
```json
{
  "depositDate": "2024-01-01",
  "objectId": "OBJ001",
  "depositAmount": 50000.00,
  "depositType": "CASH",
  "checkDueDate": "2024-12-31",
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
  "message": "新增成功",
  "data": {
    "tKey": 1,
    "depositNo": "DEP20240101001",
    "depositDate": "2024-01-01T00:00:00Z",
    "objectId": "OBJ001",
    "objectName": "客戶A",
    "depositAmount": 50000.00,
    "depositType": "CASH",
    "depositTypeName": "現金",
    "depositStatus": "A",
    "depositStatusName": "有效",
    "checkDueDate": "2024-12-31T00:00:00Z",
    "shopId": "SHOP001",
    "shopName": "分店A",
    "siteId": "SITE001",
    "orgId": "ORG001",
    "notes": "備註說明",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.2 修改保證金 API

#### 3.2.1 請求
- **URL**: `PUT /api/v1/receipt/deposits/{tKey}`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.2.2 請求參數
```json
{
  "depositAmount": 60000.00,
  "checkDueDate": "2025-12-31",
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
    "depositNo": "DEP20240101001",
    "depositDate": "2024-01-01T00:00:00Z",
    "objectId": "OBJ001",
    "objectName": "客戶A",
    "depositAmount": 60000.00,
    "depositType": "CASH",
    "depositTypeName": "現金",
    "depositStatus": "A",
    "depositStatusName": "有效",
    "checkDueDate": "2025-12-31T00:00:00Z",
    "notes": "備註說明(修改)",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-02T00:00:00Z"
  },
  "timestamp": "2024-01-02T00:00:00Z"
}
```

### 3.3 刪除保證金 API

#### 3.3.1 請求
- **URL**: `DELETE /api/v1/receipt/deposits/{tKey}`
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

### 3.4 查詢保證金 API

#### 3.4.1 請求
- **URL**: `GET /api/v1/receipt/deposits`
- **Headers**: 
  - `Authorization: Bearer {token}`
- **Query Parameters**:
  - `depositNo` (可選): 保證金單號（模糊查詢）
  - `objectId` (可選): 對象別編號
  - `depositDateFrom` (可選): 保證金日期起
  - `depositDateTo` (可選): 保證金日期迄
  - `depositStatus` (可選): 保證金狀態
  - `depositType` (可選): 保證金類型
  - `shopId` (可選): 分店代碼
  - `page` (可選): 頁碼，預設 1
  - `pageSize` (可選): 每頁筆數，預設 20
  - `sortBy` (可選): 排序欄位，預設 DepositDate
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
        "depositNo": "DEP20240101001",
        "depositDate": "2024-01-01T00:00:00Z",
        "objectId": "OBJ001",
        "objectName": "客戶A",
        "depositAmount": 50000.00,
        "depositType": "CASH",
        "depositTypeName": "現金",
        "depositStatus": "A",
        "depositStatusName": "有效",
        "returnDate": null,
        "returnAmount": 0.00,
        "voucherNo": null,
        "voucherStatus": null,
        "checkDueDate": "2024-12-31T00:00:00Z",
        "shopId": "SHOP001",
        "shopName": "分店A",
        "siteId": "SITE001",
        "orgId": "ORG001",
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

### 3.5 保證金退還 API

#### 3.5.1 請求
- **URL**: `POST /api/v1/receipt/deposits/{tKey}/return`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.5.2 請求參數
```json
{
  "returnDate": "2024-06-01",
  "returnAmount": 50000.00,
  "notes": "退還說明"
}
```

#### 3.5.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "退還成功",
  "data": {
    "tKey": 1,
    "depositNo": "DEP20240101001",
    "depositStatus": "R",
    "depositStatusName": "退還",
    "returnDate": "2024-06-01T00:00:00Z",
    "returnAmount": 50000.00,
    "notes": "退還說明",
    "updatedBy": "USER001",
    "updatedAt": "2024-06-01T00:00:00Z"
  },
  "timestamp": "2024-06-01T00:00:00Z"
}
```

### 3.6 保證金存款 API

#### 3.6.1 請求
- **URL**: `POST /api/v1/receipt/deposits/{tKey}/deposit`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.6.2 請求參數
```json
{
  "depositAmount": 50000.00,
  "depositDate": "2024-01-01",
  "notes": "存款說明"
}
```

#### 3.6.3 回應
```json
{
  "success": true,
  "code": 200,
  "message": "存款成功",
  "data": {
    "tKey": 1,
    "depositNo": "DEP20240101001",
    "depositAmount": 50000.00,
    "depositDate": "2024-01-01T00:00:00Z",
    "depositStatus": "A",
    "depositStatusName": "有效",
    "notes": "存款說明",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## 四、前端UI設計

### 4.1 保證金列表頁面

#### 4.1.1 頁面結構
- **路由**: `/receipt/deposits`
- **組件**: `ReceiptDepositsList.vue`

#### 4.1.2 功能區塊
1. **查詢條件區**
   - 保證金單號（文字輸入，模糊查詢）
   - 對象別編號（下拉選單）
   - 保證金日期起（日期選擇器）
   - 保證金日期迄（日期選擇器）
   - 保證金狀態（下拉選單）
   - 保證金類型（下拉選單）
   - 分店代碼（下拉選單）
   - 查詢按鈕
   - 重置按鈕

2. **資料列表區**
   - 表格顯示保證金資料
   - 欄位：保證金單號、保證金日期、對象別、保證金金額、保證金類型、保證金狀態、退還日期、退還金額、票據到期日、操作
   - 支援排序功能
   - 支援分頁功能

3. **操作按鈕區**
   - 新增按鈕
   - 修改按鈕（選取單筆）
   - 刪除按鈕（選取單筆或多筆）
   - 退還按鈕（選取單筆）
   - 存款按鈕（選取單筆）
   - 匯出按鈕
   - 列印按鈕

### 4.2 保證金新增/修改頁面

#### 4.2.1 頁面結構
- **路由**: `/receipt/deposits/add` (新增) / `/receipt/deposits/edit/:id` (修改)
- **組件**: `ReceiptDepositsForm.vue`

#### 4.2.2 表單欄位
1. **基本資料區**
   - 保證金日期（日期選擇器，必填）
   - 對象別編號（下拉選單，必填）
   - 保證金金額（數字輸入，必填）
   - 保證金類型（下拉選單，可選）
   - 票據到期日（日期選擇器，可選）
   - 保證金單號（文字輸入，新增時自動產生或手動輸入，修改時唯讀）
   - 分店代碼（下拉選單，必填）
   - 分公司代碼（下拉選單，可選）
   - 組織代碼（下拉選單，可選）
   - 備註（文字輸入，多行，可選）

2. **操作按鈕區**
   - 儲存按鈕
   - 取消按鈕
   - 返回按鈕

### 4.3 保證金退還頁面

#### 4.3.1 頁面結構
- **路由**: `/receipt/deposits/return/:id`
- **組件**: `ReceiptDepositsReturn.vue`

#### 4.3.2 表單欄位
1. **退還資料區**
   - 保證金單號（唯讀）
   - 保證金金額（唯讀）
   - 退還日期（日期選擇器，必填）
   - 退還金額（數字輸入，必填）
   - 備註（文字輸入，多行，可選）

2. **操作按鈕區**
   - 確認退還按鈕
   - 取消按鈕
   - 返回按鈕

### 4.4 UI組件規格

#### 4.4.1 表格組件
- 使用 Element Plus 的 `el-table` 組件
- 支援欄位排序
- 支援多選功能
- 支援欄位寬度調整
- 金額欄位右對齊顯示

#### 4.4.2 表單組件
- 使用 Element Plus 的 `el-form` 組件
- 表單驗證使用 `el-form-item` 的 `rules` 屬性
- 必填欄位標示紅色星號
- 日期欄位使用日期選擇器

#### 4.4.3 下拉選單組件
- 使用 Element Plus 的 `el-select` 組件
- 支援搜尋功能
- 支援遠端搜尋（如對象別）

---

## 五、開發時程

### 5.1 後端開發 (4天)
- Day 1: 資料庫設計與建立、Entity 與 Repository 開發
- Day 2: Service 與 Controller 開發、API 測試
- Day 3: 退還功能實作、存款功能實作
- Day 4: 單元測試、整合測試

### 5.2 前端開發 (4天)
- Day 1: 列表頁面開發、查詢功能實作
- Day 2: 新增/修改頁面開發、表單驗證實作
- Day 3: 刪除功能實作、退還功能實作、存款功能實作
- Day 4: 報表功能實作、UI優化

### 5.3 測試與優化 (2天)
- Day 1: 功能測試、效能測試
- Day 2: Bug修復、文件整理

**總計**: 10天

---

## 六、注意事項

### 6.1 資料驗證
- 保證金日期必填
- 保證金金額必填且必須大於0
- 對象別編號必填
- 退還金額不能超過保證金金額
- 只有狀態為「有效」的保證金才可退還

### 6.2 權限控制
- 新增、修改、刪除功能需要對應的權限
- 查詢功能需要讀取權限
- 退還功能需要退還權限
- 存款功能需要存款權限
- 報表功能需要報表權限
- 只有狀態為「建立」的傳票才可刪除

### 6.3 效能優化
- 查詢結果使用分頁，避免一次載入過多資料
- 對象別下拉選單使用快取機制
- 建立適當的資料庫索引

### 6.4 錯誤處理
- 新增時檢查保證金單號是否重複
- 修改時檢查資料是否存在
- 刪除時檢查保證金狀態，只有「建立」狀態才可刪除
- 退還時檢查保證金狀態和金額

### 6.5 業務邏輯
- 保證金單號自動產生規則需符合業務需求
- 退還後保證金狀態需更新為「退還」
- 存款功能需更新保證金金額和日期

---

## 七、測試案例

### 7.1 新增測試
1. **正常新增**
   - 輸入完整的保證金資料
   - 驗證新增成功，資料正確寫入資料庫

2. **重複保證金單號**
   - 輸入已存在的保證金單號
   - 驗證系統提示錯誤訊息

3. **必填欄位驗證**
   - 不輸入必填欄位
   - 驗證系統提示必填欄位錯誤

4. **金額驗證**
   - 輸入負數或零
   - 驗證系統提示金額錯誤

### 7.2 修改測試
1. **正常修改**
   - 修改保證金資料
   - 驗證修改成功，資料正確更新

2. **不存在的資料**
   - 修改不存在的保證金
   - 驗證系統提示錯誤訊息

### 7.3 刪除測試
1. **正常刪除**
   - 刪除狀態為「建立」的保證金
   - 驗證刪除成功，資料從資料庫移除

2. **狀態限制**
   - 刪除狀態非「建立」的保證金
   - 驗證系統提示無法刪除

### 7.4 退還測試
1. **正常退還**
   - 選擇狀態為「有效」的保證金進行退還
   - 驗證退還成功，狀態更新為「退還」

2. **退還金額驗證**
   - 輸入超過保證金金額的退還金額
   - 驗證系統提示錯誤訊息

3. **狀態限制**
   - 選擇狀態非「有效」的保證金進行退還
   - 驗證系統提示無法退還

### 7.5 存款測試
1. **正常存款**
   - 選擇保證金進行存款
   - 驗證存款成功，金額和日期更新

### 7.6 查詢測試
1. **基本查詢**
   - 不輸入任何條件
   - 驗證顯示所有資料（分頁）

2. **條件查詢**
   - 輸入各種查詢條件
   - 驗證查詢結果正確

3. **日期範圍查詢**
   - 使用保證金日期起迄進行查詢
   - 驗證查詢結果正確

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FB.ASP` - 保證金瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FI.ASP` - 保證金新增畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FU.ASP` - 保證金修改畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FD.ASP` - 保證金刪除畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_FQ.ASP` - 保證金查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_PR.ASP` - 保證金報表畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR510_DEPOSIT.ASP` - 保證金存款畫面

### 8.2 資料表參考
- `DEPOSIT` - 保證金主檔（Oracle）
- `VOUCHER_M` - 傳票主檔（Oracle）

### 8.3 相關功能
- 收款基礎功能 (SYSR110-SYSR120)
- 收款處理功能 (SYSR210-SYSR240)
- 收款擴展功能 (SYSR310-SYSR450)

