# Kiosk - 自助服務終端資料處理作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Kiosk
- **功能名稱**: 自助服務終端資料處理作業
- **功能描述**: 提供自助服務終端（Kiosk）與IMS系統的資料處理功能，包含會員資料處理、點數查詢、卡片管理等
- **參考舊程式**: 
  - `WEB/IMS_CORE/kiosk/WebLoyaltyDataTr.asp`
  - `WEB/IMS_CORE/kiosk/WEBLOYALTYINI.ASP`
  - `WEB/IMS_CORE/kiosk/DB_UTIL.ASP`
  - `WEB/IMS_CORE/kiosk/flashpass.ashx`

### 1.2 業務需求
- 處理會員卡號、密碼驗證
- 處理會員點數查詢
- 處理會員密碼變更
- 處理會員快速開卡
- 處理會員資料補登
- 支援多種功能命令（O2, A1, C2, D4, D8等）
- 支援XML格式資料交換
- 支援資料編碼處理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `KioskTransactions` (Kiosk交易主檔)

```sql
CREATE TABLE [dbo].[KioskTransactions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransactionId] NVARCHAR(50) NOT NULL, -- 交易編號
    [KioskId] NVARCHAR(50) NOT NULL, -- Kiosk機號
    [FunctionCode] NVARCHAR(10) NOT NULL, -- 功能代碼 (O2, A1, C2, D4, D8等)
    [CardNumber] NVARCHAR(50) NULL, -- 卡片編號
    [MemberId] NVARCHAR(50) NULL, -- 會員編號
    [TransactionDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 交易日期時間
    [RequestData] NVARCHAR(MAX) NULL, -- 請求資料（JSON格式）
    [ResponseData] NVARCHAR(MAX) NULL, -- 回應資料（JSON格式）
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Success', -- 狀態 (Success/Failed)
    [ReturnCode] NVARCHAR(10) NULL, -- 回應碼
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    CONSTRAINT [UQ_KioskTransactions_TransactionId] UNIQUE ([TransactionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_KioskId] ON [dbo].[KioskTransactions] ([KioskId]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_FunctionCode] ON [dbo].[KioskTransactions] ([FunctionCode]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_TransactionDate] ON [dbo].[KioskTransactions] ([TransactionDate]);
CREATE NONCLUSTERED INDEX [IX_KioskTransactions_CardNumber] ON [dbo].[KioskTransactions] ([CardNumber]);
```

### 2.2 資料字典

#### KioskTransactions 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| TransactionId | NVARCHAR | 50 | NO | - | 交易編號 | 唯一 |
| KioskId | NVARCHAR | 50 | NO | - | Kiosk機號 | - |
| FunctionCode | NVARCHAR | 10 | NO | - | 功能代碼 | O2, A1, C2, D4, D8等 |
| CardNumber | NVARCHAR | 50 | YES | - | 卡片編號 | - |
| MemberId | NVARCHAR | 50 | YES | - | 會員編號 | - |
| TransactionDate | DATETIME2 | - | NO | GETDATE() | 交易日期時間 | - |
| RequestData | NVARCHAR(MAX) | - | YES | - | 請求資料 | JSON格式 |
| ResponseData | NVARCHAR(MAX) | - | YES | - | 回應資料 | JSON格式 |
| Status | NVARCHAR | 20 | NO | 'Success' | 狀態 | Success/Failed |
| ReturnCode | NVARCHAR | 10 | YES | - | 回應碼 | - |
| ErrorMessage | NVARCHAR(MAX) | - | YES | - | 錯誤訊息 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 處理Kiosk請求
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kiosk/process`
- **說明**: 處理Kiosk終端的各種功能請求
- **請求格式**:
  ```json
  {
    "funCmdId": "A1",
    "kioskId": "KIOSK001",
    "loyalSysCard": "1234567890",
    "sysCardPWD": "password",
    "pid": "A123456789",
    "pnm": "張三",
    "otherData": {}
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "處理成功",
    "data": {
      "returnCode": "0000",
      "returnMessage": "成功",
      "responseData": {}
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢Kiosk交易記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kiosk/transactions`
- **說明**: 查詢Kiosk交易記錄，支援分頁、排序、篩選

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 Kiosk交易查詢頁面 (`KioskTransactions.vue`)
- 查詢表單（Kiosk機號、功能代碼、日期範圍、卡片編號）
- 資料表格（顯示交易記錄）
- 分頁元件
- 查看詳情對話框

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 建立 KioskTransactions 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (8天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] KioskService 實作
- [ ] 各種功能命令處理邏輯（O2, A1, C2, D4, D8等）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 資料編碼處理
- [ ] 錯誤處理
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] Kiosk交易查詢頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 各種功能命令測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 15.5天

---

## 六、注意事項

### 6.1 功能命令處理
- O2: 網路線上快速開卡
- A1: 確認會員卡號、密碼
- C2: 密碼變更
- D4: 網路會員點數資訊
- D8: 實體會員點數資訊
- 需實作所有功能命令的處理邏輯

### 6.2 資料編碼
- 需處理資料編碼/解碼
- 需處理XML格式資料交換
- 需處理特殊字元編碼

### 6.3 安全性
- 需驗證Kiosk機號
- 需處理密碼加密
- 需記錄所有交易記錄

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

