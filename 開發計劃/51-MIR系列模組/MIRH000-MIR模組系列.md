# MIRH000 - MIR模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MIRH000 系列
- **功能名稱**: MIR人事資源管理模組系列
- **功能描述**: 提供人事資源管理模組的完整功能，包含人事管理、薪資管理、考勤管理、報表等功能。此模組包含SYSH110-SYSH1Z5系列（人事基礎功能）、SYSH210-SYSH2B0系列（人事處理功能）、SYSG910-SYSG950系列（薪資管理功能）、SYSGA10-SYSGB40系列（薪資擴展功能）、SYSHBP00系列（獎金管理功能）等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/MIRH000/SYSH110_*.ASP` (人事基礎功能)
  - `WEB/IMS_CORE/ASP/MIRH000/SYSH210_*.ASP` (人事處理功能)
  - `WEB/IMS_CORE/ASP/MIRH000/SYSG910_*.ASP` (薪資管理功能)
  - `WEB/IMS_CORE/ASP/MIRH000/SYSGA10_*.ASP` (薪資擴展功能)
  - `WEB/IMS_CORE/ASP/MIRH000/SYSHBP00_*.ASP` (獎金管理功能)

### 1.2 業務需求
- 管理人事基本資料（SYSH110-SYSH1Z5系列）
- 支援人事處理功能（SYSH210-SYSH2B0系列）
- 管理薪資資料（SYSG910-SYSG950系列）
- 支援薪資擴展功能（SYSGA10-SYSGB40系列）
- 管理獎金資料（SYSHBP00系列）
- 支援多種人事操作和報表功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MirH000Personnel` (人事主檔)

```sql
CREATE TABLE [dbo].[MirH000Personnel] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PersonnelId] NVARCHAR(50) NOT NULL, -- 人事編號
    [PersonnelName] NVARCHAR(100) NOT NULL, -- 人事姓名
    [DepartmentId] NVARCHAR(50) NULL, -- 部門代碼
    [PositionId] NVARCHAR(50) NULL, -- 職位代碼
    [HireDate] DATETIME2 NULL, -- 到職日期
    [ResignDate] DATETIME2 NULL, -- 離職日期
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:在職, I:離職, L:留停)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_MirH000Personnel_PersonnelId] UNIQUE ([PersonnelId])
);
```

### 2.2 相關資料表

#### 2.2.1 `MirH000Salaries` - 薪資主檔
```sql
CREATE TABLE [dbo].[MirH000Salaries] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SalaryId] NVARCHAR(50) NOT NULL, -- 薪資編號
    [PersonnelId] NVARCHAR(50) NOT NULL, -- 人事編號
    [SalaryMonth] NVARCHAR(6) NOT NULL, -- 薪資月份 (YYYYMM)
    [BaseSalary] DECIMAL(18, 4) NULL DEFAULT 0, -- 基本薪資
    [Bonus] DECIMAL(18, 4) NULL DEFAULT 0, -- 獎金
    [TotalSalary] DECIMAL(18, 4) NULL DEFAULT 0, -- 總薪資
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_MirH000Salaries_SalaryId] UNIQUE ([SalaryId]),
    CONSTRAINT [FK_MirH000Salaries_Personnel] FOREIGN KEY ([PersonnelId]) REFERENCES [dbo].[MirH000Personnel] ([PersonnelId])
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢人事列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirh000/personnel`
- **說明**: 查詢人事列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆人事
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirh000/personnel/{personnelId}`
- **說明**: 查詢單筆人事資料

#### 3.1.3 新增人事
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/mirh000/personnel`
- **說明**: 新增人事資料

#### 3.1.4 修改人事
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/mirh000/personnel/{personnelId}`
- **說明**: 修改人事資料

#### 3.1.5 刪除人事
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/mirh000/personnel/{personnelId}`
- **說明**: 刪除人事資料

#### 3.1.6 查詢薪資列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirh000/salaries`
- **說明**: 查詢薪資列表

#### 3.1.7 新增薪資
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/mirh000/salaries`
- **說明**: 新增薪資資料

#### 3.1.8 修改薪資
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/mirh000/salaries/{salaryId}`
- **說明**: 修改薪資資料

#### 3.1.9 刪除薪資
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/mirh000/salaries/{salaryId}`
- **說明**: 刪除薪資資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 人事列表頁面 (`MirH000PersonnelList.vue`)
- **路徑**: `/mirh000/personnel`
- **功能**: 顯示人事列表，支援查詢、新增、修改、刪除

#### 4.1.2 薪資列表頁面 (`MirH000SalaryList.vue`)
- **路徑**: `/mirh000/salaries`
- **功能**: 顯示薪資列表，支援查詢、新增、修改、刪除

### 4.2 UI 元件設計

參考SYSH110-人事基本資料維護系列的UI設計。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (5天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 15天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 薪資資料必須嚴格權限控制

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 人事編號必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 刪除人事前必須檢查是否有相關資料
- 薪資計算必須正確
- 離職日期必須晚於到職日期

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增人事成功
- [ ] 修改人事成功
- [ ] 刪除人事成功
- [ ] 查詢人事列表成功
- [ ] 新增薪資成功
- [ ] 修改薪資成功
- [ ] 刪除薪資成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/MIRH000/SYSH110_*.ASP`
- `WEB/IMS_CORE/ASP/MIRH000/SYSH210_*.ASP`
- `WEB/IMS_CORE/ASP/MIRH000/SYSG910_*.ASP`
- `WEB/IMS_CORE/ASP/MIRH000/SYSGA10_*.ASP`
- `WEB/IMS_CORE/ASP/MIRH000/SYSHBP00_*.ASP`

### 8.2 相關功能
- SYSH110-人事基本資料維護系列（人事基本資料維護功能）
- SYSH210-薪資資料維護系列（薪資資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

