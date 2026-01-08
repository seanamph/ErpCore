# SYSPA10-SYSPB60 - 採購擴展維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSPA10-SYSPB60系列
- **功能名稱**: 採購擴展維護系列
- **功能描述**: 提供採購擴展維護功能的新增、修改、刪除、查詢功能，包含擴展維護設定、擴展維護參數等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSPA10_*.ASP`
  - `WEB/IMS_CORE/ASP/SYSP000/SYSPB60_*.ASP`

### 1.2 業務需求
- 管理採購擴展維護功能
- 支援擴展維護參數設定
- 支援擴展維護查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseExtendedMaintenance` (採購擴展維護)

```sql
CREATE TABLE [dbo].[PurchaseExtendedMaintenance] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [MaintenanceId] NVARCHAR(50) NOT NULL, -- 維護代碼
    [MaintenanceName] NVARCHAR(100) NOT NULL, -- 維護名稱
    [MaintenanceType] NVARCHAR(20) NULL, -- 維護類型
    [MaintenanceDesc] NVARCHAR(500) NULL, -- 維護說明
    [MaintenanceConfig] NVARCHAR(MAX) NULL, -- 維護配置 (JSON格式)
    [ParameterConfig] NVARCHAR(MAX) NULL, -- 參數配置 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseExtendedMaintenance] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PurchaseExtendedMaintenance_MaintenanceId] UNIQUE ([MaintenanceId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_MaintenanceId] ON [dbo].[PurchaseExtendedMaintenance] ([MaintenanceId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_MaintenanceType] ON [dbo].[PurchaseExtendedMaintenance] ([MaintenanceType]);
CREATE NONCLUSTERED INDEX [IX_PurchaseExtendedMaintenance_Status] ON [dbo].[PurchaseExtendedMaintenance] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| MaintenanceId | NVARCHAR | 50 | NO | - | 維護代碼 | 唯一 |
| MaintenanceName | NVARCHAR | 100 | NO | - | 維護名稱 | - |
| MaintenanceType | NVARCHAR | 20 | YES | - | 維護類型 | - |
| MaintenanceDesc | NVARCHAR | 500 | YES | - | 維護說明 | - |
| MaintenanceConfig | NVARCHAR(MAX) | - | YES | - | 維護配置 | JSON格式 |
| ParameterConfig | NVARCHAR(MAX) | - | YES | - | 參數配置 | JSON格式 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購擴展維護列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended-maintenance`
- **說明**: 查詢採購擴展維護列表，支援分頁、排序、篩選
- **請求參數**: 同SYSP510-SYSP530
- **回應格式**: 同SYSP510-SYSP530

#### 3.1.2 查詢單筆採購擴展維護
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended-maintenance/{tKey}`
- **說明**: 根據主鍵查詢單筆採購擴展維護資料

#### 3.1.3 根據維護代碼查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-extended-maintenance/by-id/{maintenanceId}`
- **說明**: 根據維護代碼查詢採購擴展維護資料

#### 3.1.4 新增採購擴展維護
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-extended-maintenance`
- **說明**: 新增採購擴展維護資料
- **請求格式**: 包含maintenanceId、maintenanceName、maintenanceType、maintenanceConfig、parameterConfig等欄位

#### 3.1.5 修改採購擴展維護
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-extended-maintenance/{tKey}`
- **說明**: 修改採購擴展維護資料

#### 3.1.6 刪除採購擴展維護
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-extended-maintenance/{tKey}`
- **說明**: 刪除採購擴展維護資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購擴展維護頁面 (`PurchaseExtendedMaintenance.vue`)
- **路徑**: `/procurement/purchase-extended-maintenance`
- **功能**: 顯示採購擴展維護管理介面

### 4.2 頁面元件

#### 4.2.1 查詢條件區塊
- 維護代碼輸入框
- 維護名稱輸入框
- 維護類型下拉選單
- 狀態下拉選單
- 查詢按鈕、重置按鈕

#### 4.2.2 資料列表區塊
- 資料表格顯示：維護代碼、維護名稱、維護類型、維護說明、狀態、排序序號、操作按鈕
- 分頁控制元件、排序功能

#### 4.2.3 新增/修改表單區塊
- 維護代碼輸入框（新增時必填，修改時唯讀）
- 維護名稱輸入框（必填）
- 維護類型下拉選單（必填）
- 維護說明文字區塊
- 維護配置JSON編輯器
- 參數配置JSON編輯器
- 狀態下拉選單（必填）
- 排序序號輸入框
- 備註文字區塊
- 儲存按鈕、取消按鈕

### 4.3 表單驗證規則

- 維護代碼：必填，長度1-50，唯一
- 維護名稱：必填，長度1-100
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
- [ ] 新增採購擴展維護成功
- [ ] 修改採購擴展維護成功
- [ ] 刪除採購擴展維護成功
- [ ] 查詢採購擴展維護列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSPA10_*.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSPB60_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

