# MIRV000 - MIR模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MIRV000 系列
- **功能名稱**: MIR憑證管理模組系列
- **功能描述**: 提供憑證管理模組的完整功能，包含憑證管理、憑證查詢、憑證報表等功能。此模組包含SYSV110-SYSV1E0系列（憑證基礎功能）、SYSV210-SYSV2X0系列（憑證處理功能）、SYSV310-SYSV350系列（憑證查詢功能）、SYSV410-SYSV4Z0系列（憑證報表功能）等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/MIRV000/SYSV110_*.ASP` (憑證基礎功能)
  - `WEB/IMS_CORE/ASP/MIRV000/SYSV210_*.ASP` (憑證處理功能)
  - `WEB/IMS_CORE/ASP/MIRV000/SYSV310_*.ASP` (憑證查詢功能)
  - `WEB/IMS_CORE/ASP/MIRV000/SYSV410_*.ASP` (憑證報表功能)

### 1.2 業務需求
- 管理憑證基本資料（SYSV110-SYSV1E0系列）
- 支援憑證處理功能（SYSV210-SYSV2X0系列）
- 支援憑證查詢功能（SYSV310-SYSV350系列）
- 支援憑證報表功能（SYSV410-SYSV4Z0系列）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MirV000Vouchers` (憑證主檔)

```sql
CREATE TABLE [dbo].[MirV000Vouchers] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號
    [VoucherName] NVARCHAR(100) NOT NULL, -- 憑證名稱
    [VoucherType] NVARCHAR(20) NULL, -- 憑證類型
    [VoucherDate] DATETIME2 NOT NULL, -- 憑證日期
    [Amount] DECIMAL(18, 4) NULL DEFAULT 0, -- 金額
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_MirV000Vouchers_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MirV000Vouchers_VoucherId] ON [dbo].[MirV000Vouchers] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_MirV000Vouchers_VoucherType] ON [dbo].[MirV000Vouchers] ([VoucherType]);
CREATE NONCLUSTERED INDEX [IX_MirV000Vouchers_VoucherDate] ON [dbo].[MirV000Vouchers] ([VoucherDate]);
CREATE NONCLUSTERED INDEX [IX_MirV000Vouchers_Status] ON [dbo].[MirV000Vouchers] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢憑證列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirv000/vouchers`
- **說明**: 查詢憑證列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆憑證
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirv000/vouchers/{voucherId}`
- **說明**: 查詢單筆憑證資料

#### 3.1.3 新增憑證
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/mirv000/vouchers`
- **說明**: 新增憑證資料

#### 3.1.4 修改憑證
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/mirv000/vouchers/{voucherId}`
- **說明**: 修改憑證資料

#### 3.1.5 刪除憑證
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/mirv000/vouchers/{voucherId}`
- **說明**: 刪除憑證資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 憑證列表頁面 (`MirV000VoucherList.vue`)
- **路徑**: `/mirv000/vouchers`
- **功能**: 顯示憑證列表，支援查詢、新增、修改、刪除

### 4.2 UI 元件設計

參考SYSK110-SYSK150-憑證資料維護系列的UI設計。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
### 5.2 階段二: 後端開發 (5天)
### 5.3 階段三: 前端開發 (5天)
### 5.4 階段四: 整合測試 (2天)
### 5.5 階段五: 文件與部署 (1天)

**總計**: 15天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 憑證編號必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 刪除憑證前必須檢查是否有相關資料
- 憑證金額必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增憑證成功
- [ ] 修改憑證成功
- [ ] 刪除憑證成功
- [ ] 查詢憑證列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/MIRV000/SYSV110_*.ASP`
- `WEB/IMS_CORE/ASP/MIRV000/SYSV210_*.ASP`
- `WEB/IMS_CORE/ASP/MIRV000/SYSV310_*.ASP`
- `WEB/IMS_CORE/ASP/MIRV000/SYSV410_*.ASP`

### 8.2 相關功能
- SYSK110-SYSK150-憑證資料維護系列（憑證資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

