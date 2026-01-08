# MIRW000 - MIR模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MIRW000 系列
- **功能名稱**: MIR模組系列
- **功能描述**: 提供MIRW000模組的完整功能，包含資料維護、查詢、報表等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/MIRW000/*.ASP` (MIRW000模組功能)

### 1.2 業務需求
- 管理MIRW000模組資料
- 支援資料的新增、修改、刪除、查詢功能
- 支援資料報表功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MirW000Data` (MIRW000資料主檔)

```sql
CREATE TABLE [dbo].[MirW000Data] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DataId] NVARCHAR(50) NOT NULL, -- 資料編號
    [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
    [DataValue] NVARCHAR(200) NULL, -- 資料值
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_MirW000Data_DataId] UNIQUE ([DataId])
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirw000/data`
- **說明**: 查詢資料列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/mirw000/data/{dataId}`
- **說明**: 查詢單筆資料

#### 3.1.3 新增資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/mirw000/data`
- **說明**: 新增資料

#### 3.1.4 修改資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/mirw000/data/{dataId}`
- **說明**: 修改資料

#### 3.1.5 刪除資料
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/mirw000/data/{dataId}`
- **說明**: 刪除資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 資料列表頁面 (`MirW000DataList.vue`)
- **路徑**: `/mirw000/data`
- **功能**: 顯示資料列表，支援查詢、新增、修改、刪除

### 4.2 UI 元件設計

參考一般資料維護功能的UI設計。

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
- 資料編號必須唯一
- 必填欄位必須驗證

### 6.4 業務邏輯
- 刪除資料前必須檢查是否有相關資料

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增資料成功
- [ ] 修改資料成功
- [ ] 刪除資料成功
- [ ] 查詢資料列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/MIRW000/*.ASP`

### 8.2 相關功能
- MIRH000-MIR模組系列（MIR人事資源管理模組）
- MIRV000-MIR模組系列（MIR憑證管理模組）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

