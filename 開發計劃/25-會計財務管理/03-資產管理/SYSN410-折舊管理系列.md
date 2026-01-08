# SYSN410 - 折舊管理系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN410系列
- **功能名稱**: 折舊管理系列
- **功能描述**: 提供資產折舊資料的計算、查詢、報表功能，包含折舊計算、折舊明細、折舊統計等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN410_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN410_PR.ASP` (報表)

### 1.2 業務需求
- 支援多種折舊方法計算
- 支援折舊明細查詢
- 支援折舊統計報表
- 支援折舊匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Depreciations` (對應舊系統 `DEPRECIATION`)

```sql
CREATE TABLE [dbo].[Depreciations] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [AssetId] NVARCHAR(50) NOT NULL, -- 資產編號 (ASSET_ID)
    [DepreciationYear] INT NOT NULL, -- 折舊年度 (DEPRECIATION_YEAR)
    [DepreciationMonth] INT NOT NULL, -- 折舊月份 (DEPRECIATION_MONTH)
    [DepreciationAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 折舊金額 (DEPRECIATION_AMOUNT)
    [AccumulatedDepreciation] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 累計折舊 (ACCUMULATED_DEPRECIATION)
    [BookValue] DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 帳面價值 (BOOK_VALUE)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Depreciations] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [FK_Depreciations_Assets] FOREIGN KEY ([AssetId]) REFERENCES [dbo].[Assets] ([AssetId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Depreciations_AssetId] ON [dbo].[Depreciations] ([AssetId]);
CREATE NONCLUSTERED INDEX [IX_Depreciations_Year_Month] ON [dbo].[Depreciations] ([DepreciationYear], [DepreciationMonth]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 計算折舊
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/depreciations/calculate`
- **說明**: 計算資產折舊

#### 3.1.2 查詢折舊明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/depreciations`
- **說明**: 查詢折舊明細列表

#### 3.1.3 查詢折舊統計
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/depreciations/statistics`
- **說明**: 查詢折舊統計資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 折舊管理頁面 (`DepreciationManagement.vue`)
- **路徑**: `/accounting/depreciations`
- **功能**: 顯示折舊管理介面，支援折舊計算、查詢、報表

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

### 6.1 業務邏輯
- 折舊計算必須符合會計準則
- 折舊方法必須正確計算
- 累計折舊必須正確累加

---

## 七、測試案例

### 7.1 單元測試
- [ ] 計算折舊成功
- [ ] 查詢折舊明細成功
- [ ] 查詢折舊統計成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN410_FQ.ASP`
- `WEB/IMS_CORE/ASP/SYSN000/SYSN410_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

