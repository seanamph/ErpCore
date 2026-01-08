# XCOM240 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM240
- **功能名稱**: 通訊資料維護（部門測試）
- **功能描述**: 提供部門資料的瀏覽與查詢功能，包含部門代碼、部門名稱、備註等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM240_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM240_FQ.asp` (查詢)

### 1.2 業務需求
- 管理部門資料
- 支援部門資料的瀏覽與查詢
- 支援多條件查詢
- 支援資料排序

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Departments` (部門資料，對應舊系統 `DEPT`)

```sql
CREATE TABLE [dbo].[Departments] (
    [DeptId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 部門代碼
    [DeptName] NVARCHAR(100) NULL, -- 部門名稱
    [DeptNote] NVARCHAR(500) NULL, -- 備註
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Departments_DeptName] ON [dbo].[Departments] ([DeptName]);
CREATE NONCLUSTERED INDEX [IX_Departments_Status] ON [dbo].[Departments] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢部門列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom240/departments`
- **說明**: 查詢部門列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 部門資料瀏覽頁面 (`DepartmentBrowse.vue`)
- **路徑**: `/xcom/departments`
- **功能**: 顯示部門資料列表，支援查詢、排序
- **主要元件**:
  - 查詢表單 (DepartmentSearchForm)
  - 資料表格 (DepartmentDataTable)

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 瀏覽頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 4天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須記錄查詢日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

---

## 七、參考資料

### 7.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM240_FB.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM240_FQ.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

