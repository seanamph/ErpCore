# SYS6A04-SYS6A19 - POS查詢作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS6A04-SYS6A19 系列
- **功能名稱**: POS查詢作業系列
- **功能描述**: 提供POS系統資料的查詢、報表功能，包含POS終端查詢、POS交易查詢、POS統計報表等查詢功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS6000/SYS6A04_*.ASP` (POS查詢相關功能)
  - `WEB/IMS_CORE/ASP/SYS6000/SYS6A19_*.ASP` (POS查詢相關功能)

### 1.2 業務需求
- 查詢POS終端基本資料資訊
- 支援多條件組合查詢
- 支援POS交易資料查詢
- 支援POS統計資訊查詢
- 支援POS報表列印
- 支援POS資料匯出

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PosTerminals` (POS終端主檔)

參考 [SYS6610-SYS6999-POS資料維護系列.md](./SYS6610-SYS6999-POS資料維護系列.md) 中的資料表定義

### 2.2 相關資料表

#### 2.2.1 `PosTransactions` - POS交易資料（用於查詢）
```sql
CREATE TABLE [dbo].[PosTransactions] (
    [TransactionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PosTerminalId] NVARCHAR(50) NOT NULL,
    [TransactionDate] DATETIME2 NOT NULL,
    [TransactionType] NVARCHAR(50) NULL,
    [Amount] DECIMAL(18,2) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_PosTransactions_PosTerminals] FOREIGN KEY ([PosTerminalId]) REFERENCES [dbo].[PosTerminals] ([PosTerminalId])
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢POS終端列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos-terminals/query`
- **說明**: 查詢POS終端列表，支援分頁、排序、篩選

#### 3.1.2 POS交易查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos-transactions/query`
- **說明**: 查詢POS交易資料

#### 3.1.3 POS統計查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos-terminals/statistics`
- **說明**: 查詢POS統計資訊

---

## 四、前端 UI 設計

參考 [SYS6381-SYS63A0-樓層查詢作業系列.md](../02-樓層管理/SYS6381-SYS63A0-樓層查詢作業系列.md) 的前端UI設計

---

## 五、開發時程

參考POS維護系列的開發時程

---

## 六、注意事項

參考POS維護系列的注意事項

---

## 七、測試案例

參考POS維護系列的測試案例

---

## 八、參考資料

### 8.1 相關開發計劃
- [SYS6610-SYS6999-POS資料維護系列.md](./SYS6610-SYS6999-POS資料維護系列.md)

### 8.2 技術文件
- Element Plus Table 文件
- ECharts 圖表庫文件

### 8.3 舊程式參考
- `WEB/IMS_CORE/ASP/SYS6000/SYS6A04_*.ASP`
- `WEB/IMS_CORE/ASP/SYS6000/SYS6A19_*.ASP`

