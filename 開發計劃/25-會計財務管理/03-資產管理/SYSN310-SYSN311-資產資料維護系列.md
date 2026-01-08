# SYSN310-SYSN311 - 資產資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN310-SYSN311系列
- **功能名稱**: 資產資料維護系列
- **功能描述**: 提供資產資料的新增、修改、刪除、查詢功能，包含資產編號、資產名稱、資產類別、取得日期、取得成本、折舊方法等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FQ.ASP` (查詢)

### 1.2 業務需求
- 管理資產基本資料
- 支援多種資產類別
- 支援折舊方法設定
- 支援資產異動記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Assets` (對應舊系統 `ASSETS`)

```sql
CREATE TABLE [dbo].[Assets] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [AssetId] NVARCHAR(50) NOT NULL, -- 資產編號 (ASSET_ID)
    [AssetName] NVARCHAR(200) NOT NULL, -- 資產名稱 (ASSET_NAME)
    [AssetType] NVARCHAR(50) NULL, -- 資產類別 (ASSET_TYPE)
    [AcquisitionDate] DATETIME2 NULL, -- 取得日期 (ACQUISITION_DATE)
    [AcquisitionCost] DECIMAL(18, 2) NULL DEFAULT 0, -- 取得成本 (ACQUISITION_COST)
    [DepreciationMethod] NVARCHAR(20) NULL, -- 折舊方法 (DEPRECIATION_METHOD)
    [UsefulLife] INT NULL, -- 使用年限 (USEFUL_LIFE)
    [ResidualValue] DECIMAL(18, 2) NULL DEFAULT 0, -- 殘值 (RESIDUAL_VALUE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:使用中, D:已處分)
    [Location] NVARCHAR(200) NULL, -- 存放地點 (LOCATION)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Assets] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_Assets_AssetId] UNIQUE ([AssetId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Assets_AssetId] ON [dbo].[Assets] ([AssetId]);
CREATE NONCLUSTERED INDEX [IX_Assets_AssetType] ON [dbo].[Assets] ([AssetType]);
CREATE NONCLUSTERED INDEX [IX_Assets_Status] ON [dbo].[Assets] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢資產列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/assets`
- **說明**: 查詢資產列表，支援分頁、排序、篩選

#### 3.1.2 查詢單筆資產
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/assets/{assetId}`
- **說明**: 根據資產編號查詢單筆資產資料

#### 3.1.3 新增資產
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/assets`
- **說明**: 新增資產資料

#### 3.1.4 修改資產
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/assets/{assetId}`
- **說明**: 修改資產資料

#### 3.1.5 刪除資產
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/assets/{assetId}`
- **說明**: 刪除資產資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 資產列表頁面 (`AssetList.vue`)
- **路徑**: `/accounting/assets`
- **功能**: 顯示資產列表，支援查詢、新增、修改、刪除

---

## 五、開發時程

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

### 6.2 資料驗證
- 資產編號必須唯一
- 取得成本必須為正數
- 使用年限必須為正數

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增資產成功
- [ ] 修改資產成功
- [ ] 刪除資產成功
- [ ] 查詢資產列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FI.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FU.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FD.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN310_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

