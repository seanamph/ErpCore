# SYSP610 - 採購擴展功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP610系列
- **功能名稱**: 採購擴展功能系列
- **功能描述**: 提供採購擴展功能的查詢、維護功能，包含擴展功能設定、擴展功能參數等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP610_*.ASP`

### 1.2 業務需求
- 管理採購擴展功能
- 支援擴展功能參數設定
- 支援擴展功能查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseExtendedFunctions` (採購擴展功能)

```sql
CREATE TABLE [dbo].[PurchaseExtendedFunctions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExtFunctionId] NVARCHAR(50) NOT NULL, -- 擴展功能代碼
    [ExtFunctionName] NVARCHAR(100) NOT NULL, -- 擴展功能名稱
    [ExtFunctionType] NVARCHAR(20) NULL, -- 擴展功能類型
    [ExtFunctionDesc] NVARCHAR(500) NULL, -- 擴展功能說明
    [ExtFunctionConfig] NVARCHAR(MAX) NULL, -- 擴展功能配置 (JSON格式)
    [ParameterConfig] NVARCHAR(MAX) NULL, -- 參數配置 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseExtendedFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PurchaseExtendedFunctions_ExtFunctionId] UNIQUE ([ExtFunctionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_ExtFunctionId] ON [dbo].[PurchaseExtendedFunctions] ([ExtFunctionId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_ExtFunctionType] ON [dbo].[PurchaseExtendedFunctions] ([ExtFunctionType]);
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedFunctions_Status] ON [dbo].[PurchaseExtendedFunctions] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ExtFunctionId | NVARCHAR | 50 | NO | - | 擴展功能代碼 | 唯一 |
| ExtFunctionName | NVARCHAR | 100 | NO | - | 擴展功能名稱 | - |
| ExtFunctionType | NVARCHAR | 20 | YES | - | 擴展功能類型 | - |
| ExtFunctionDesc | NVARCHAR | 500 | YES | - | 擴展功能說明 | - |
| ExtFunctionConfig | NVARCHAR(MAX) | - | YES | - | 擴展功能配置 | JSON格式 |
| ParameterConfig | NVARCHAR(MAX) | - | YES | - | 參數配置 | JSON格式 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購擴展功能列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended`
- **說明**: 查詢採購擴展功能列表，支援分頁、排序、篩選
- **請求參數**: 同SYSP510-SYSP530
- **回應格式**: 同SYSP510-SYSP530

#### 3.1.2 查詢單筆採購擴展功能
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended/{tKey}`
- **說明**: 根據主鍵查詢單筆採購擴展功能資料

#### 3.1.3 根據功能代碼查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended/by-id/{extFunctionId}`
- **說明**: 根據功能代碼查詢採購擴展功能資料

#### 3.1.4 新增採購擴展功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-extended`
- **說明**: 新增採購擴展功能資料
- **請求格式**: 包含extFunctionId、extFunctionName、extFunctionType、extFunctionConfig、parameterConfig等欄位

#### 3.1.5 修改採購擴展功能
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-extended/{tKey}`
- **說明**: 修改採購擴展功能資料

#### 3.1.6 刪除採購擴展功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-extended/{tKey}`
- **說明**: 刪除採購擴展功能資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購擴展功能頁面 (`PurchaseExtendedFunctions.vue`)
- **路徑**: `/procurement/purchase-extended`
- **功能**: 顯示採購擴展功能管理介面

### 4.2 頁面元件

#### 4.2.1 查詢條件區塊
- 功能代碼輸入框
- 功能名稱輸入框
- 功能類型下拉選單
- 狀態下拉選單
- 查詢按鈕、重置按鈕

#### 4.2.2 資料列表區塊
- 資料表格顯示：功能代碼、功能名稱、功能類型、功能說明、狀態、排序序號、操作按鈕
- 分頁控制元件、排序功能

#### 4.2.3 新增/修改表單區塊
- 功能代碼輸入框（新增時必填，修改時唯讀）
- 功能名稱輸入框（必填）
- 功能類型下拉選單（必填）
- 功能說明文字區塊
- 功能配置JSON編輯器
- 參數配置JSON編輯器
- 狀態下拉選單（必填）
- 排序序號輸入框
- 備註文字區塊
- 儲存按鈕、取消按鈕

### 4.3 表單驗證規則

- 功能代碼：必填，長度1-50，唯一
- 功能名稱：必填，長度1-100
- 狀態：必填，必須為A或I之一
- 排序序號：數字，範圍0-9999

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增採購擴展功能成功
- [ ] 修改採購擴展功能成功
- [ ] 刪除採購擴展功能成功
- [ ] 查詢採購擴展功能列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP610_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

