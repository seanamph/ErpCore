# SYSPH00 - 系統擴展PH功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSPH00 系列
- **功能名稱**: 系統擴展PH功能系列（感應卡維護作業）
- **功能描述**: 提供感應卡維護作業的多筆新增功能，包含感應卡號、員工代號、有效期間、卡片狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSPH00/SYSH3D0_FMI.ASP` (多筆新增)

### 1.2 業務需求
- 管理感應卡基本資料
- 支援感應卡的多筆新增功能
- 支援感應卡號管理
- 支援員工代號對應
- 支援有效期間設定（開始日期、結束日期）
- 支援卡片狀態管理（啟用、停用等）
- 支援批量新增功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EmpCard` (員工感應卡主檔)

```sql
CREATE TABLE [dbo].[EmpCard] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CardNo] NVARCHAR(50) NOT NULL, -- 感應卡號
    [EmpId] NVARCHAR(50) NOT NULL, -- 員工代號
    [BeginDate] DATETIME2 NULL, -- 開始日期
    [EndDate] DATETIME2 NULL, -- 結束日期
    [CardStatus] NVARCHAR(20) NOT NULL DEFAULT '1', -- 卡片狀態 (1:啟用, 0:停用)
    [Notes] NVARCHAR(500) NULL, -- 備註
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 更新者
    [CTime] DATETIME2 NULL, -- 更新時間
    [CPriority] INT NULL, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_EmpCard] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_EmpCard_CardNo] UNIQUE ([CardNo])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmpCard_EmpId] ON [dbo].[EmpCard] ([EmpId]);
CREATE NONCLUSTERED INDEX [IX_EmpCard_CardStatus] ON [dbo].[EmpCard] ([CardStatus]);
CREATE NONCLUSTERED INDEX [IX_EmpCard_BeginDate] ON [dbo].[EmpCard] ([BeginDate]);
CREATE NONCLUSTERED INDEX [IX_EmpCard_EndDate] ON [dbo].[EmpCard] ([EndDate]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| CardNo | NVARCHAR | 50 | NO | - | 感應卡號 | 唯一鍵 |
| EmpId | NVARCHAR | 50 | NO | - | 員工代號 | - |
| BeginDate | DATETIME2 | - | YES | - | 開始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| CardStatus | NVARCHAR | 20 | NO | '1' | 卡片狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| BUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| BTime | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| CUser | NVARCHAR | 50 | YES | - | 更新者 | - |
| CTime | DATETIME2 | - | YES | - | 更新時間 | - |
| CPriority | INT | - | YES | - | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢感應卡列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/emp-cards`
- **說明**: 查詢感應卡列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（包含cardNo、empId、cardStatus等篩選條件）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆感應卡
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/emp-cards/{tKey}`
- **說明**: 根據主鍵查詢單筆感應卡資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增感應卡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/emp-cards`
- **說明**: 新增感應卡資料
- **請求格式**: 標準新增請求格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 批量新增感應卡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/emp-cards/batch`
- **說明**: 批量新增感應卡資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "cardNo": "CARD001",
        "empId": "EMP001",
        "beginDate": "2024-01-01",
        "endDate": "2024-12-31",
        "cardStatus": "1",
        "notes": "備註1"
      },
      {
        "cardNo": "CARD002",
        "empId": "EMP002",
        "beginDate": "2024-01-01",
        "endDate": "2024-12-31",
        "cardStatus": "1",
        "notes": "備註2"
      }
    ]
  }
  ```
- **回應格式**: 批量新增回應格式

#### 3.1.5 修改感應卡
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/emp-cards/{tKey}`
- **說明**: 修改感應卡資料
- **請求格式**: 標準修改請求格式
- **回應格式**: 標準修改回應格式

#### 3.1.6 刪除感應卡
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/emp-cards/{tKey}`
- **說明**: 刪除感應卡資料（軟刪除或硬刪除）
- **回應格式**: 標準刪除回應格式

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 感應卡維護頁面 (`EmpCard.vue`)
- **路徑**: `/system/emp-cards`
- **功能**: 顯示感應卡列表，支援新增、修改、刪除、查詢、批量新增

### 4.2 主要元件

#### 4.2.1 感應卡列表表格
- 顯示感應卡號、員工代號、員工姓名、開始日期、結束日期、卡片狀態等欄位
- 支援按感應卡號、員工代號、卡片狀態篩選
- 支援排序、分頁

#### 4.2.2 感應卡表單（單筆）
- 感應卡號（必填）
- 員工代號（必填，可從V_EMP_USER選擇）
- 開始日期（可選）
- 結束日期（可選）
- 卡片狀態（必填，下拉選單：啟用、停用）
- 備註

#### 4.2.3 感應卡批量新增表單
- 支援多筆資料輸入（可設定筆數，預設10筆）
- 每筆包含：選擇標記、感應卡號、員工代號、開始日期、結束日期、卡片狀態、備註
- 支援批量儲存功能

---

## 五、開發時程

**總計**: 10天
- 資料庫設計: 1天
- 後端API開發: 3天
- 前端UI開發: 4天
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸
- 感應卡號不可重複

### 6.2 效能
- 查詢結果需支援分頁
- 大量資料需使用索引優化
- 批量新增需使用批次處理

### 6.3 業務邏輯
- 感應卡號不可重複
- 員工代號需存在於員工資料表
- 結束日期需大於開始日期
- 批量新增時需檢查每筆資料的有效性

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增感應卡成功
- [ ] 修改感應卡成功
- [ ] 刪除感應卡成功
- [ ] 查詢感應卡列表成功
- [ ] 批量新增感應卡成功
- [ ] 感應卡號重複檢查
- [ ] 員工代號存在檢查
- [ ] 日期有效性檢查

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSPH00/SYSH3D0_FMI.ASP` - 感應卡維護作業（多筆新增）
- `WEB/IMS_CORE/ASP/include/SYSH000/SYSH000.inc` - SYSH000包含檔案
- `WEB/IMS_CORE/ASP/util/SYSH000/hr_util.asp` - 人資工具函數

### 8.2 相關文件
- DOTNET_Core_Vue_系統架構設計.md
- 系統架構分析.md

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

