# SYSPED0 - 人事資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSPED0 系列
- **功能名稱**: 人事資料維護系列
- **功能描述**: 提供人事資料的新增、修改、刪除、查詢功能，包含人事編號、姓名、部門、職位、到職日期、離職日期等資訊管理。此模組為系統擴展E類的人事管理功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSPE00/SYSPED0_*.ASP` (人事資料維護)

### 1.2 業務需求
- 管理人事基本資料資訊
- 支援人事到職/離職管理
- 記錄人事異動資訊
- 支援人事組織架構設定
- 支援人事職位管理
- 支援多部門架構

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Personnel` (人事主檔)

```sql
CREATE TABLE [dbo].[Personnel] (
    [PersonnelId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PersonnelName] NVARCHAR(100) NOT NULL,
    [IdNumber] NVARCHAR(20) NULL,
    [DepartmentId] NVARCHAR(50) NULL,
    [PositionId] NVARCHAR(50) NULL,
    [HireDate] DATETIME2 NULL,
    [ResignDate] DATETIME2 NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:在職, I:離職, L:留停
    [Email] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Address] NVARCHAR(500) NULL,
    [BirthDate] DATETIME2 NULL,
    [Gender] NVARCHAR(10) NULL, -- M:男, F:女
    [Notes] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Personnel] PRIMARY KEY CLUSTERED ([PersonnelId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Personnel_DepartmentId] ON [dbo].[Personnel] ([DepartmentId]);
CREATE NONCLUSTERED INDEX [IX_Personnel_Status] ON [dbo].[Personnel] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Personnel_PositionId] ON [dbo].[Personnel] ([PositionId]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢人事列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/personnel`
- **說明**: 查詢人事列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆人事
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/personnel/{personnelId}`
- **說明**: 查詢單筆人事資料

#### 3.1.3 新增人事
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/personnel`
- **說明**: 新增人事資料

#### 3.1.4 修改人事
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/personnel/{personnelId}`
- **說明**: 修改人事資料

#### 3.1.5 刪除人事
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/personnel/{personnelId}`
- **說明**: 刪除人事資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 人事列表頁面 (`PersonnelList.vue`)
- **路徑**: `/personnel`
- **功能**: 顯示人事列表，支援查詢、新增、修改、刪除

#### 4.1.2 人事詳細頁面 (`PersonnelDetail.vue`)
- **路徑**: `/personnel/:personnelId`
- **功能**: 顯示人事詳細資料，支援修改

### 4.2 UI 元件設計

參考SYSH110-人事基本資料維護系列的UI設計。

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 人事編號必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 刪除人事前必須檢查是否有相關資料
- 離職日期必須晚於到職日期

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增人事成功
- [ ] 修改人事成功
- [ ] 刪除人事成功
- [ ] 查詢人事列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSPE00/SYSPED0_*.ASP`

### 8.2 相關功能
- SYSH110-人事基本資料維護系列（人事基本資料維護功能）
- SYSPE10-SYSPE11-員工資料維護系列（員工資料維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

