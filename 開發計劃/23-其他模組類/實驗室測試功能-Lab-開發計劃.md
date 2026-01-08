# Lab - 實驗室測試功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Lab
- **功能名稱**: 實驗室測試功能
- **功能描述**: 提供測試和開發相關功能，用於系統測試、連線測試、功能驗證等
- **參考舊程式**: 
  - `WEB/IMS_CORE/Lab/default.aspx`
  - `WEB/IMS_CORE/Lab/default.aspx.cs`
  - `WEB/IMS_CORE/Lab/conn_test.asp`

### 1.2 業務需求
- 資料庫連線測試
- 系統功能測試
- 效能測試
- 開發除錯功能
- 測試記錄管理

### 1.3 使用場景
- 系統開發階段
- 系統測試階段
- 問題除錯
- 效能驗證

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TestResults` (測試結果)

```sql
CREATE TABLE [dbo].[TestResults] (
    [TestId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TestName] NVARCHAR(200) NOT NULL, -- 測試名稱
    [TestType] NVARCHAR(50) NOT NULL, -- CONNECTION, FUNCTION, PERFORMANCE
    [TestData] NVARCHAR(MAX) NULL, -- 測試資料 (JSON格式)
    [TestResult] NVARCHAR(MAX) NULL, -- 測試結果 (JSON格式)
    [Status] NVARCHAR(20) NOT NULL, -- SUCCESS, FAILED, PENDING
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [Duration] INT NULL, -- 執行時間 (毫秒)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_TestResults] PRIMARY KEY CLUSTERED ([TestId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TestResults_TestType] ON [dbo].[TestResults] ([TestType]);
CREATE NONCLUSTERED INDEX [IX_TestResults_Status] ON [dbo].[TestResults] ([Status]);
CREATE NONCLUSTERED INDEX [IX_TestResults_CreatedAt] ON [dbo].[TestResults] ([CreatedAt]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 資料庫連線測試
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lab/test/connection`
- **說明**: 測試資料庫連線
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "連線測試成功",
    "data": {
      "status": "SUCCESS",
      "duration": 100,
      "connectionString": "***"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 執行測試
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lab/test/execute`
- **說明**: 執行測試
- **請求格式**:
  ```json
  {
    "testName": "連線測試",
    "testType": "CONNECTION",
    "testData": {}
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 實驗室測試頁面 (`LabTest.vue`)
- **路徑**: `/lab/test`
- **功能**: 系統測試功能
- **主要元件**:
  - 測試功能選單
  - 測試結果顯示
  - 測試記錄查詢

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 測試邏輯實作

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 測試頁面開發

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 測試功能僅限開發/測試環境使用
- 必須限制測試功能的存取權限
- 必須記錄所有測試操作

### 6.2 效能
- 測試必須快速執行
- 必須處理測試超時

---

## 七、參考資料

### 7.1 舊程式碼
- `WEB/IMS_CORE/Lab/default.aspx.cs`
- `WEB/IMS_CORE/Lab/conn_test.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

