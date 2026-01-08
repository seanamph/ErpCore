# LPS - 忠誠度系統維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: LPS
- **功能名稱**: 忠誠度系統維護
- **功能描述**: 提供忠誠度系統的維護功能，包含會員點數調整、點數交易處理、會員卡管理、點數查詢、交易查詢等作業
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSLPS/lps.asp` (忠誠度系統維護)

### 1.2 業務需求
- 管理會員點數調整
- 處理點數交易（累積、扣減、取消）
- 管理會員卡資料
- 查詢點數交易記錄
- 支援多種交易類型（累積、扣減、取消、調整等）
- 支援交易取消（VOID）功能
- 支援交易查詢功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LoyaltyPointTransactions` (點數交易主檔)

```sql
CREATE TABLE [dbo].[LoyaltyPointTransactions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [RRN] NVARCHAR(50) NOT NULL, -- 交易編號
    [CardNo] NVARCHAR(50) NOT NULL, -- 會員卡號
    [TraceNo] NVARCHAR(50) NULL, -- 追蹤編號
    [ExpDate] NVARCHAR(10) NULL, -- 到期日
    [AwardPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 累積點數
    [RedeemPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 扣減點數
    [ReversalFlag] NVARCHAR(10) NULL, -- 取消標記
    [Amount] DECIMAL(18, 4) NULL, -- 交易金額
    [VoidFlag] NVARCHAR(10) NULL, -- 作廢標記
    [AuthCode] NVARCHAR(50) NULL, -- 授權碼
    [ForceDate] DATETIME2 NULL, -- 強制日期
    [Invoice] NVARCHAR(50) NULL, -- 發票號碼
    [TransType] NVARCHAR(20) NULL, -- 交易類型 (2, 3, 4, 11, 13, 16, 18等)
    [TxnType] NVARCHAR(20) NULL, -- 交易類型代碼 (2, 3, 4, 5, 7, 8, 9等)
    [TransTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 交易時間
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'SUCCESS', -- 交易狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_LoyaltyPointTransactions_RRN] UNIQUE ([RRN])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_RRN] ON [dbo].[LoyaltyPointTransactions] ([RRN]);
CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_CardNo] ON [dbo].[LoyaltyPointTransactions] ([CardNo]);
CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_TransTime] ON [dbo].[LoyaltyPointTransactions] ([TransTime]);
CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_TransType] ON [dbo].[LoyaltyPointTransactions] ([TransType]);
CREATE NONCLUSTERED INDEX [IX_LoyaltyPointTransactions_Status] ON [dbo].[LoyaltyPointTransactions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `LoyaltyMembers` - 會員主檔
```sql
CREATE TABLE [dbo].[LoyaltyMembers] (
    [CardNo] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [MemberName] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(100) NULL,
    [TotalPoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 總點數
    [AvailablePoints] DECIMAL(18, 4) NULL DEFAULT 0, -- 可用點數
    [ExpDate] NVARCHAR(10) NULL, -- 到期日
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢點數交易列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/loyalty-point-transactions`
- **說明**: 查詢點數交易列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數（pageIndex, pageSize, sortField, sortOrder, filters）
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆點數交易
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/loyalty-point-transactions/{rrn}`
- **說明**: 查詢單筆點數交易資料

#### 3.1.3 新增點數交易
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/loyalty-point-transactions`
- **說明**: 新增點數交易（累積、扣減等）
- **請求格式**:
  ```json
  {
    "cardNo": "CARD001",
    "traceNo": "TRACE001",
    "expDate": "20241231",
    "awardPoints": 100,
    "redeemPoints": 0,
    "amount": 1000,
    "invoice": "INV001",
    "transType": "2"
  }
  ```

#### 3.1.4 取消點數交易
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/loyalty-point-transactions/{rrn}/void`
- **說明**: 取消點數交易
- **請求格式**:
  ```json
  {
    "reversalFlag": "Y",
    "voidFlag": "Y"
  }
  ```

#### 3.1.5 查詢會員點數
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/loyalty-members/{cardNo}/points`
- **說明**: 查詢會員點數資訊

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 點數交易列表頁面 (`LoyaltyPointTransactionList.vue`)
- **路徑**: `/loyalty-point-transactions`
- **功能**: 顯示點數交易列表，支援查詢、新增、取消

#### 4.1.2 點數交易詳細頁面 (`LoyaltyPointTransactionDetail.vue`)
- **路徑**: `/loyalty-point-transactions/:rrn`
- **功能**: 顯示點數交易詳細資料

### 4.2 UI 元件設計

參考一般交易維護功能的UI設計。

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
- [ ] 交易處理功能實作
- [ ] 取消功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/取消對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 交易處理測試
- [ ] 取消功能測試

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
- 交易操作必須記錄日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 交易處理必須支援非同步處理

### 6.3 資料驗證
- 交易編號必須唯一
- 必填欄位必須驗證
- 點數計算必須驗證

### 6.4 業務邏輯
- 點數累積必須更新會員總點數
- 點數扣減必須檢查可用點數
- 取消交易必須回滾點數
- 交易類型必須正確對應

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增點數交易成功
- [ ] 取消點數交易成功
- [ ] 查詢點數交易列表成功
- [ ] 查詢會員點數成功

### 7.2 整合測試
- [ ] 完整交易流程測試
- [ ] 取消功能測試
- [ ] 點數計算測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSLPS/lps.asp`

### 8.2 相關功能
- WEBLOYALTYINI-忠誠度系統初始化（忠誠度系統初始化功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

