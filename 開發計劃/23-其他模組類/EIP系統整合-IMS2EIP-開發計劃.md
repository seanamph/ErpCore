# IMS2EIP - EIP系統整合 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: IMS2EIP
- **功能名稱**: EIP系統整合
- **功能描述**: 提供IMS系統與EIP (Enterprise Information Portal) 系統的整合功能，包含單一登入、表單傳送、資料同步等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/IMS2EIP.aspx`
  - `IMS3/HANSHIN/IMS3/IMS2EIP.aspx.cs`
  - `WEB/IMS_CORE/ASP/SYS2000/SYS2G01_EIP.ASP`
  - `WEB/IMS_CORE/ASP/SYS2000/SYS2S80_FU.asp` (EIP送件功能)

### 1.2 業務需求
- EIP系統單一登入 (SSO)
- IMS表單資料傳送至EIP
- EIP表單資料回傳至IMS
- EIP簽核流程整合
- EIP資料查詢
- EIP使用者對應

### 1.3 使用場景
- 電子簽核流程
- 表單送審
- 跨系統資料交換
- 單一登入整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EipIntegrations` (EIP整合設定)

```sql
CREATE TABLE [dbo].[EipIntegrations] (
    [IntegrationId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProgId] NVARCHAR(50) NOT NULL, -- 作業編號
    [PageId] NVARCHAR(50) NOT NULL, -- 頁面代碼
    [EipUrl] NVARCHAR(500) NOT NULL, -- EIP系統URL
    [Fid] NVARCHAR(100) NULL, -- 表單ID
    [SingleField] NVARCHAR(MAX) NULL, -- 單筆欄位對應 (JSON格式)
    [MultiField] NVARCHAR(MAX) NULL, -- 多筆欄位對應 (JSON格式)
    [DetailTable] NVARCHAR(200) NULL, -- 明細資料表名稱
    [MultiMSeqNo] NVARCHAR(50) NULL, -- 多筆主檔序號欄位
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_EipIntegrations_ProgId_PageId] UNIQUE ([ProgId], [PageId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EipIntegrations_ProgId] ON [dbo].[EipIntegrations] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_EipIntegrations_Status] ON [dbo].[EipIntegrations] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `EipTransactions` - EIP交易記錄
```sql
CREATE TABLE [dbo].[EipTransactions] (
    [TransactionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [IntegrationId] BIGINT NOT NULL,
    [ProgId] NVARCHAR(50) NOT NULL,
    [PageId] NVARCHAR(50) NOT NULL,
    [FlowId] NVARCHAR(100) NULL, -- EIP流程ID
    [RequestData] NVARCHAR(MAX) NULL, -- 請求資料 (JSON格式)
    [ResponseData] NVARCHAR(MAX) NULL, -- 回應資料 (JSON格式)
    [Status] NVARCHAR(20) NOT NULL, -- PENDING, APPROVED, REJECTED, CANCELLED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_EipTransactions_EipIntegrations] FOREIGN KEY ([IntegrationId]) REFERENCES [dbo].[EipIntegrations] ([IntegrationId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EipTransactions_IntegrationId] ON [dbo].[EipTransactions] ([IntegrationId]);
CREATE NONCLUSTERED INDEX [IX_EipTransactions_FlowId] ON [dbo].[EipTransactions] ([FlowId]);
CREATE NONCLUSTERED INDEX [IX_EipTransactions_Status] ON [dbo].[EipTransactions] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 EIP單一登入
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/eip/sso`
- **說明**: EIP系統單一登入
- **回應格式**: 重導向到EIP系統

#### 3.1.2 傳送表單至EIP
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/eip/send-form`
- **說明**: 傳送表單資料至EIP系統
- **請求格式**:
  ```json
  {
    "progId": "SYS2S80",
    "pageId": "U",
    "formData": {
      "field1": "value1",
      "field2": "value2"
    },
    "detailData": [
      {
        "field1": "value1",
        "field2": "value2"
      }
    ]
  }
  ```

#### 3.1.3 查詢EIP整合設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/eip/integrations`
- **說明**: 查詢EIP整合設定列表

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 EIP整合管理頁面 (`EipIntegration.vue`)
- **路徑**: `/system/eip/integration`
- **功能**: EIP整合設定管理、EIP交易記錄查詢
- **主要元件**:
  - EIP整合設定列表
  - EIP交易記錄查詢表格
  - 新增/修改對話框

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] EIP API整合
- [ ] 單一登入邏輯
- [ ] 表單傳送邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] EIP整合管理頁面開發
- [ ] EIP交易記錄查詢頁面開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] EIP系統整合測試
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 安全性
- EIP連線必須使用HTTPS
- 必須驗證EIP系統身份
- 敏感資料必須加密傳輸
- 必須記錄所有EIP操作

### 6.2 效能
- EIP通訊必須使用非同步處理
- 必須處理EIP系統回應超時
- 必須使用快取機制

### 6.3 資料驗證
- 表單資料必須驗證
- 必須處理EIP系統錯誤回應
- 必須處理資料格式錯誤

---

## 七、參考資料

### 7.1 舊程式碼
- `IMS3/HANSHIN/IMS3/IMS2EIP.aspx.cs`
- `WEB/IMS_CORE/ASP/SYS2000/SYS2G01_EIP.ASP`
- `WEB/IMS_CORE/ASP/SYS2000/SYS2S80_FU.asp`

### 7.2 相關功能
- 系統通訊整合
- 表單處理
- 簽核流程

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

