# CUSHANSHIN - 客戶定制模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: CUSHANSHIN 系列
- **功能名稱**: 阪神客戶定制模組系列
- **功能描述**: 提供阪神客戶定制功能，此模組為特定阪神客戶的定制化功能，包含會員管理、促銷活動等阪神客戶特定功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/CUSHANSHIN/*.ASP` (阪神客戶定制功能)

### 1.2 業務需求
- 管理阪神客戶特定的會員資料
- 支援阪神客戶特定的促銷活動
- 符合阪神客戶業務流程
- 支援阪神客戶特定報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CusHanshinData` (阪神客戶資料主檔)

```sql
CREATE TABLE [dbo].[CusHanshinData] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [DataId] NVARCHAR(50) NOT NULL, -- 資料編號
    [DataName] NVARCHAR(100) NOT NULL, -- 資料名稱
    [HanshinSpecificField] NVARCHAR(200) NULL, -- 阪神特定欄位
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_CusHanshinData_DataId] UNIQUE ([DataId])
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢阪神資料列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/cus-hanshin/data`
- **說明**: 查詢阪神資料列表

#### 3.1.2 新增阪神資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/cus-hanshin/data`
- **說明**: 新增阪神資料

#### 3.1.3 修改阪神資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/cus-hanshin/data/{dataId}`
- **說明**: 修改阪神資料

#### 3.1.4 刪除阪神資料
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/cus-hanshin/data/{dataId}`
- **說明**: 刪除阪神資料

---

## 四、前端 UI 設計

### 4.1 頁面結構

參考一般資料維護功能的UI設計，但針對阪神客戶需求進行優化。

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

### 6.1 客戶定制需求
- 必須與阪神客戶確認功能需求
- 必須符合阪神客戶業務流程
- 必須支援阪神客戶特定報表

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增阪神資料成功
- [ ] 修改阪神資料成功
- [ ] 刪除阪神資料成功
- [ ] 查詢阪神資料列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 阪神客戶驗收測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/CUSHANSHIN/*.ASP`

### 8.2 相關功能
- CUS3000-客戶定制模組系列（一般客戶定制功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

