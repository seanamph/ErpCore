# XCOM210 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM210
- **功能名稱**: 通訊資料維護（系統環境資料設定）
- **功能描述**: 提供系統登入參數資料的維護功能，包含登入標題、跑馬燈訊息等系統環境設定
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM210_FU.asp` (修改)
  - 註：此功能實際上是XCOM110（跑馬燈維護作業）的一部分，用於維護系統登入參數

### 1.2 業務需求
- 管理系統登入參數資料
- 支援登入標題設定
- 支援跑馬燈訊息設定
- 與XCOM110功能整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XComLoginPara` (登入參數，對應舊系統 `XCOM_LOGINPARA`)

```sql
CREATE TABLE [dbo].[XComLoginPara] (
    [ParaName] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 參數名稱
    [ParaData] NVARCHAR(500) NULL, -- 參數資料
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XComLoginPara_ParaName] ON [dbo].[XComLoginPara] ([ParaName]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ParaName | NVARCHAR | 50 | NO | - | 參數名稱 | 主鍵 |
| ParaData | NVARCHAR | 500 | YES | - | 參數資料 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢登入參數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom210/login-para/{paraName}`
- **說明**: 根據參數名稱查詢登入參數資料
- **回應格式**: 標準單筆回應格式

#### 3.1.2 修改登入參數
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom210/login-para/{paraName}`
- **說明**: 修改登入參數資料
- **請求格式**:
  ```json
  {
    "paraData": "參數資料內容"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 登入參數維護頁面 (`LoginParaMaintenance.vue`)
- **路徑**: `/xcom/login-para`
- **功能**: 顯示登入參數設定，支援修改
- **主要元件**:
  - 參數設定表單 (LoginParaForm)

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引

### 5.2 階段二: 後端開發 (1天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立

### 5.3 階段三: 前端開發 (1天)
- [ ] API 呼叫函數
- [ ] 參數設定頁面開發
- [ ] 表單驗證

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試

**總計**: 3天

---

## 六、注意事項

### 6.1 安全性
- 參數修改必須有管理員權限
- 必須記錄所有操作日誌

### 6.2 業務邏輯
- 此功能與XCOM110（跑馬燈維護作業）整合
- 參數名稱必須在允許清單中

---

## 七、參考資料

### 7.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM210_FU.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

