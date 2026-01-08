# SYSL510 - 加班發放管理 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL510
- **功能名稱**: 加班發放管理
- **功能描述**: 提供加班發放資料的新增、修改、刪除、查詢功能，包含加班發放單據維護、審核、報表等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL510.js`
  - `WEB/IMS_CORE/SYSL000/SYSL510 - 01.rdlc`
  - `WEB/IMS_CORE/SYSL000/SYSL511.js`
  - `WEB/IMS_CORE/SYSL000/SYSL512.js`
  - `WEB/IMS_CORE/SYSL000/SYSL513.js`
  - `WEB/IMS_CORE/SYSL000/SYSL514.js`
  - `WEB/IMS_CORE/SYSL000/SYSL525.js`
  - `WEB/IMS_CORE/SYSL000/SYSL526.js`
  - `WEB/IMS_CORE/SYSL000/SYSL527.js`

### 1.2 業務需求
- 管理加班發放單據
- 支援加班發放資料的新增、修改、刪除、查詢
- 支援加班發放單據審核
- 支援加班發放報表查詢與列印
- 支援加班發放資料匯入
- 支援加班發放統計報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `OvertimePayments` (加班發放主檔)

```sql
CREATE TABLE [dbo].[OvertimePayments] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PaymentNo] NVARCHAR(50) NOT NULL, -- 發放單號
    [PaymentDate] DATETIME2 NOT NULL, -- 發放日期
    [EmployeeId] NVARCHAR(50) NOT NULL, -- 員工編號
    [EmployeeName] NVARCHAR(100) NULL, -- 員工姓名
    [DepartmentId] NVARCHAR(50) NULL, -- 部門編號
    [OvertimeHours] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 加班時數
    [OvertimeAmount] DECIMAL(18,2) NOT NULL DEFAULT 0, -- 加班金額
    [StartDate] DATETIME2 NOT NULL, -- 開始日期
    [EndDate] DATETIME2 NOT NULL, -- 結束日期
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft', -- 狀態 (Draft/Submitted/Approved/Rejected)
    [ApprovedBy] NVARCHAR(50) NULL, -- 審核者
    [ApprovedAt] DATETIME2 NULL, -- 審核時間
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [UQ_OvertimePayments_PaymentNo] UNIQUE ([PaymentNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_OvertimePayments_PaymentNo] ON [dbo].[OvertimePayments] ([PaymentNo]);
CREATE NONCLUSTERED INDEX [IX_OvertimePayments_EmployeeId] ON [dbo].[OvertimePayments] ([EmployeeId]);
CREATE NONCLUSTERED INDEX [IX_OvertimePayments_PaymentDate] ON [dbo].[OvertimePayments] ([PaymentDate]);
CREATE NONCLUSTERED INDEX [IX_OvertimePayments_Status] ON [dbo].[OvertimePayments] ([Status]);
```

### 2.2 資料字典

#### OvertimePayments 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| PaymentNo | NVARCHAR | 50 | NO | - | 發放單號 | 唯一 |
| PaymentDate | DATETIME2 | - | NO | - | 發放日期 | - |
| EmployeeId | NVARCHAR | 50 | NO | - | 員工編號 | - |
| EmployeeName | NVARCHAR | 100 | YES | - | 員工姓名 | - |
| DepartmentId | NVARCHAR | 50 | YES | - | 部門編號 | - |
| OvertimeHours | DECIMAL | 18,2 | NO | 0 | 加班時數 | - |
| OvertimeAmount | DECIMAL | 18,2 | NO | 0 | 加班金額 | - |
| StartDate | DATETIME2 | - | NO | - | 開始日期 | - |
| EndDate | DATETIME2 | - | NO | - | 結束日期 | - |
| Status | NVARCHAR | 20 | NO | 'Draft' | 狀態 | Draft/Submitted/Approved/Rejected |
| ApprovedBy | NVARCHAR | 50 | YES | - | 審核者 | - |
| ApprovedAt | DATETIME2 | - | YES | - | 審核時間 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢加班發放列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/overtime-payments`
- **說明**: 查詢加班發放列表，支援分頁、排序、篩選

#### 3.1.2 查詢單筆加班發放
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/overtime-payments/{paymentNo}`
- **說明**: 根據發放單號查詢單筆加班發放資料

#### 3.1.3 新增加班發放
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/overtime-payments`
- **說明**: 新增加班發放資料

#### 3.1.4 修改加班發放
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/overtime-payments/{paymentNo}`
- **說明**: 修改加班發放資料

#### 3.1.5 刪除加班發放
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/overtime-payments/{paymentNo}`
- **說明**: 刪除加班發放資料

#### 3.1.6 審核加班發放
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/overtime-payments/{paymentNo}/approve`
- **說明**: 審核加班發放單據

#### 3.1.7 匯入加班發放資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/overtime-payments/import`
- **說明**: 匯入加班發放資料（Excel格式）

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 加班發放查詢頁面 (`OvertimePayments.vue`)
- 查詢表單（發放單號、員工編號、日期範圍、狀態）
- 資料表格（顯示加班發放資料）
- 分頁元件
- 新增按鈕
- 修改按鈕
- 刪除按鈕
- 審核按鈕
- 匯入按鈕

#### 4.1.2 加班發放新增/修改對話框 (`OvertimePaymentDialog.vue`)
- 發放單號（新增時自動產生）
- 發放日期
- 員工編號（下拉選單）
- 員工姓名（自動帶出）
- 部門編號（下拉選單）
- 加班時數
- 加班金額
- 開始日期
- 結束日期
- 備註
- 儲存按鈕
- 取消按鈕

#### 4.1.3 加班發放報表頁面 (`OvertimePaymentReport.vue`)
- 報表查詢表單
- 報表顯示區域
- 列印按鈕
- 匯出按鈕

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 建立 OvertimePayments 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (10天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 審核邏輯實作
- [ ] 匯入功能實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (8天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 新增/修改對話框開發
- [ ] 審核功能開發
- [ ] 匯入功能開發
- [ ] 報表頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (3天)
- [ ] API 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 審核流程測試
- [ ] 匯入功能測試
- [ ] 報表功能測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 24天

---

## 六、注意事項

### 6.1 審核流程
- 需實作審核流程
- 需記錄審核記錄
- 需支援審核退回

### 6.2 資料匯入
- 需支援Excel格式匯入
- 需實作資料驗證
- 需處理匯入錯誤

### 6.3 報表功能
- 需支援多種報表格式
- 需支援報表列印
- 需支援報表匯出

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

