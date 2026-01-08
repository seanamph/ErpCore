# SYSR110-SYSR120 - 收款基礎功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSR110-SYSR120 系列
- **功能名稱**: 收款基礎功能系列
- **功能描述**: 提供收款項目對照會計科目維護的新增、修改、刪除、查詢功能，包含收款項目代號、收款項目名稱、會計科目代號等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR110_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSR000/SYSR120_PR.ASP` (報表)

### 1.2 業務需求
- 管理收款項目基本資料
- 支援收款項目與會計科目對照設定
- 支援多店別管理
- 支援收款項目查詢與報表
- 支援收款項目資料維護（新增、修改、刪除）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ArItems` (收款項目主檔)

```sql
CREATE TABLE [dbo].[ArItems] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SiteId] NVARCHAR(50) NOT NULL, -- 分店代號 (SITE_ID)
    [AritemId] NVARCHAR(50) NOT NULL, -- 收款項目代號 (ARITEM_ID)
    [AritemName] NVARCHAR(200) NOT NULL, -- 收款項目名稱 (ARITEM_NAME)
    [StypeId] NVARCHAR(50) NULL, -- 會計科目代號 (STYPE_ID)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [Buser] NVARCHAR(50) NULL, -- 建立人員 (BUSER)
    [Btime] DATETIME2 NULL, -- 建立時間 (BTIME)
    [Cpriority] INT NULL, -- 建立優先權 (CPRIORITY)
    [Cgroup] NVARCHAR(50) NULL, -- 建立群組 (CGROUP)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ArItems] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_ArItems_SiteId_AritemId] UNIQUE ([SiteId], [AritemId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ArItems_SiteId] ON [dbo].[ArItems] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_ArItems_AritemId] ON [dbo].[ArItems] ([AritemId]);
CREATE NONCLUSTERED INDEX [IX_ArItems_StypeId] ON [dbo].[ArItems] ([StypeId]);
```

### 2.2 資料字典

#### 2.2.1 ArItems 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| SiteId | NVARCHAR | 50 | NO | - | 分店代號 | SITE_ID |
| AritemId | NVARCHAR | 50 | NO | - | 收款項目代號 | ARITEM_ID |
| AritemName | NVARCHAR | 200 | NO | - | 收款項目名稱 | ARITEM_NAME |
| StypeId | NVARCHAR | 50 | YES | - | 會計科目代號 | STYPE_ID，外鍵至會計科目表 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | NOTES |
| Buser | NVARCHAR | 50 | YES | - | 建立人員 | BUSER |
| Btime | DATETIME2 | - | YES | - | 建立時間 | BTIME |
| Cpriority | INT | - | YES | - | 建立優先權 | CPRIORITY |
| Cgroup | NVARCHAR | 50 | YES | - | 建立群組 | CGROUP |

---

## 三、後端API設計

### 3.1 新增收款項目 API

#### 3.1.1 請求
- **URL**: `POST /api/v1/receipt/aritems`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.1.2 請求參數
```json
{
  "siteId": "SITE001",
  "aritemId": "01",
  "aritemName": "現金收款",
  "stypeId": "ACCT001",
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
    "siteId": "SITE001",
    "aritemId": "01",
    "aritemName": "現金收款",
    "stypeId": "ACCT001",
    "notes": "備註說明",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.2 修改收款項目 API

#### 3.2.1 請求
- **URL**: `PUT /api/v1/receipt/aritems/{tKey}`
- **Headers**: 
  - `Content-Type: application/json`
  - `Authorization: Bearer {token}`

#### 3.2.2 請求參數
```json
{
  "aritemName": "現金收款(修改)",
  "stypeId": "ACCT002",
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
    "siteId": "SITE001",
    "aritemId": "01",
    "aritemName": "現金收款(修改)",
    "stypeId": "ACCT002",
    "notes": "備註說明(修改)",
    "updatedBy": "USER001",
    "updatedAt": "2024-01-01T00:00:00Z"
  },
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3.3 刪除收款項目 API

#### 3.3.1 請求
- **URL**: `DELETE /api/v1/receipt/aritems/{tKey}`
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

### 3.4 查詢收款項目 API

#### 3.4.1 請求
- **URL**: `GET /api/v1/receipt/aritems`
- **Headers**: 
  - `Authorization: Bearer {token}`
- **Query Parameters**:
  - `siteId` (可選): 分店代號
  - `aritemId` (可選): 收款項目代號（模糊查詢）
  - `aritemName` (可選): 收款項目名稱（模糊查詢）
  - `stypeId` (可選): 會計科目代號
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
        "siteId": "SITE001",
        "aritemId": "01",
        "aritemName": "現金收款",
        "stypeId": "ACCT001",
        "stypeName": "現金科目",
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

### 3.5 取得單筆收款項目 API

#### 3.5.1 請求
- **URL**: `GET /api/v1/receipt/aritems/{tKey}`
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
    "siteId": "SITE001",
    "aritemId": "01",
    "aritemName": "現金收款",
    "stypeId": "ACCT001",
    "stypeName": "現金科目",
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

### 4.1 收款項目列表頁面

#### 4.1.1 頁面結構
- **路由**: `/receipt/aritems`
- **組件**: `ReceiptAritemsList.vue`

#### 4.1.2 功能區塊
1. **查詢條件區**
   - 分店代號（下拉選單）
   - 收款項目代號（文字輸入，模糊查詢）
   - 收款項目名稱（文字輸入，模糊查詢）
   - 會計科目代號（下拉選單）
   - 查詢按鈕
   - 重置按鈕

2. **資料列表區**
   - 表格顯示收款項目資料
   - 欄位：收款項目代號、收款項目名稱、會計科目代號、會計科目名稱、備註、操作
   - 支援排序功能
   - 支援分頁功能

3. **操作按鈕區**
   - 新增按鈕
   - 修改按鈕（選取單筆）
   - 刪除按鈕（選取單筆或多筆）
   - 匯出按鈕
   - 列印按鈕

### 4.2 收款項目新增/修改頁面

#### 4.2.1 頁面結構
- **路由**: `/receipt/aritems/add` (新增) / `/receipt/aritems/edit/:id` (修改)
- **組件**: `ReceiptAritemsForm.vue`

#### 4.2.2 表單欄位
1. **基本資料區**
   - 分店代號（下拉選單，必填，新增時可選，修改時唯讀）
   - 收款項目代號（文字輸入，必填，新增時自動產生或手動輸入，修改時唯讀）
   - 收款項目名稱（文字輸入，必填）
   - 會計科目代號（下拉選單，可選）
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

#### 4.3.2 表單組件
- 使用 Element Plus 的 `el-form` 組件
- 表單驗證使用 `el-form-item` 的 `rules` 屬性
- 必填欄位標示紅色星號

#### 4.3.3 下拉選單組件
- 使用 Element Plus 的 `el-select` 組件
- 支援搜尋功能
- 支援遠端搜尋（如會計科目）

---

## 五、開發時程

### 5.1 後端開發 (3天)
- Day 1: 資料庫設計與建立、Entity 與 Repository 開發
- Day 2: Service 與 Controller 開發、API 測試
- Day 3: 單元測試、整合測試

### 5.2 前端開發 (3天)
- Day 1: 列表頁面開發、查詢功能實作
- Day 2: 新增/修改頁面開發、表單驗證實作
- Day 3: 刪除功能實作、報表功能實作、UI優化

### 5.3 測試與優化 (2天)
- Day 1: 功能測試、效能測試
- Day 2: Bug修復、文件整理

**總計**: 8天

---

## 六、注意事項

### 6.1 資料驗證
- 收款項目代號在同一分店下必須唯一
- 收款項目名稱必填
- 會計科目代號必須存在於會計科目表中

### 6.2 權限控制
- 新增、修改、刪除功能需要對應的權限
- 查詢功能需要讀取權限
- 報表功能需要報表權限

### 6.3 效能優化
- 查詢結果使用分頁，避免一次載入過多資料
- 會計科目下拉選單使用快取機制
- 建立適當的資料庫索引

### 6.4 錯誤處理
- 新增時檢查收款項目代號是否重複
- 修改時檢查資料是否存在
- 刪除時檢查是否有關聯資料（如已使用於收款單）

---

## 七、測試案例

### 7.1 新增測試
1. **正常新增**
   - 輸入完整的收款項目資料
   - 驗證新增成功，資料正確寫入資料庫

2. **重複代號**
   - 輸入已存在的收款項目代號
   - 驗證系統提示錯誤訊息

3. **必填欄位驗證**
   - 不輸入必填欄位
   - 驗證系統提示必填欄位錯誤

### 7.2 修改測試
1. **正常修改**
   - 修改收款項目資料
   - 驗證修改成功，資料正確更新

2. **不存在的資料**
   - 修改不存在的收款項目
   - 驗證系統提示錯誤訊息

### 7.3 刪除測試
1. **正常刪除**
   - 刪除收款項目
   - 驗證刪除成功，資料從資料庫移除

2. **有關聯資料**
   - 刪除已被使用的收款項目
   - 驗證系統提示無法刪除

### 7.4 查詢測試
1. **基本查詢**
   - 不輸入任何條件
   - 驗證顯示所有資料（分頁）

2. **條件查詢**
   - 輸入各種查詢條件
   - 驗證查詢結果正確

3. **模糊查詢**
   - 使用收款項目代號或名稱進行模糊查詢
   - 驗證查詢結果正確

---

## 八、參考資料

### 8.1 舊程式參考
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FB.ASP` - 收款項目瀏覽畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FI.ASP` - 收款項目新增畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FU.ASP` - 收款項目修改畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FD.ASP` - 收款項目刪除畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_FQ.ASP` - 收款項目查詢畫面
- `WEB/IMS_CORE/ASP/SYSR000/SYSR110_PR.ASP` - 收款項目報表畫面

### 8.2 資料表參考
- `AR_ITEM` - 收款項目主檔（Oracle）
- `STYPE` - 會計科目主檔（Oracle）

### 8.3 相關功能
- 會計科目維護 (SYST111-SYST11A)
- 收款處理功能 (SYSR210-SYSR240)

