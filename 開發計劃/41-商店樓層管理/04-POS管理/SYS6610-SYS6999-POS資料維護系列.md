# SYS6610-SYS6999 - POS資料維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS6610-SYS6999 系列
- **功能名稱**: POS資料維護系列
- **功能描述**: 提供POS系統資料的新增、修改、刪除、查詢功能，包含POS終端設定、POS系統整合、POS交易資料等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS6000/SYS6610_*.ASP` (POS相關功能)
  - `WEB/IMS_CORE/ASP/SYS6000/SYS6710_*.ASP` (POS相關功能)
  - `WEB/IMS_CORE/ASP/SYS6000/SYS6999_*.ASP` (POS相關功能)
  - `WEB/IMS_CORE/ASP/SYS6000/FLOOR_LIST_FOR_POS.ASP` (POS樓層列表)

### 1.2 業務需求
- 管理POS終端基本資料資訊
- 支援POS終端的新增、修改、刪除、查詢
- 支援POS系統整合設定
- 支援POS交易資料管理
- 記錄POS終端的建立與變更資訊
- 支援POS終端狀態管理（啟用/停用）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PosTerminals` (POS終端主檔)

```sql
CREATE TABLE [dbo].[PosTerminals] (
    [PosTerminalId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PosSystemId] NVARCHAR(50) NOT NULL, -- POS系統代碼
    [TerminalCode] NVARCHAR(50) NOT NULL, -- 終端代碼
    [TerminalName] NVARCHAR(200) NOT NULL, -- 終端名稱
    [ShopId] NVARCHAR(50) NULL, -- 商店編號
    [FloorId] NVARCHAR(50) NULL, -- 樓層代碼
    [TerminalType] NVARCHAR(50) NULL, -- 終端類型
    [IpAddress] NVARCHAR(50) NULL, -- IP位址
    [Port] INT NULL, -- 連接埠
    [Config] NVARCHAR(MAX) NULL, -- JSON格式的設定
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [LastSyncDate] DATETIME2 NULL, -- 最後同步時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PosTerminals] PRIMARY KEY CLUSTERED ([PosTerminalId] ASC),
    CONSTRAINT [FK_PosTerminals_ShopFloors] FOREIGN KEY ([ShopId]) REFERENCES [dbo].[ShopFloors] ([ShopId]),
    CONSTRAINT [FK_PosTerminals_Floors] FOREIGN KEY ([FloorId]) REFERENCES [dbo].[Floors] ([FloorId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PosTerminals_PosSystemId] ON [dbo].[PosTerminals] ([PosSystemId]);
CREATE NONCLUSTERED INDEX [IX_PosTerminals_ShopId] ON [dbo].[PosTerminals] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_PosTerminals_Status] ON [dbo].[PosTerminals] ([Status]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢POS終端列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pos-terminals`
- **說明**: 查詢POS終端列表，支援分頁、排序、篩選
- **請求參數**: 參考商店維護系列的查詢API
- **回應格式**: 參考商店維護系列的查詢API

#### 3.1.2 新增POS終端
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pos-terminals`
- **說明**: 新增POS終端資料

#### 3.1.3 修改POS終端
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pos-terminals/{posTerminalId}`
- **說明**: 修改POS終端資料

#### 3.1.4 刪除POS終端
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/pos-terminals/{posTerminalId}`
- **說明**: 刪除POS終端資料

#### 3.1.5 POS資料同步
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pos-terminals/{posTerminalId}/sync`
- **說明**: 同步POS終端資料

---

## 四、前端 UI 設計

參考 [SYS6110-SYS6140-商店資料維護系列.md](../01-商店管理/SYS6110-SYS6140-商店資料維護系列.md) 的前端UI設計

---

## 五、開發時程

參考商店維護系列的開發時程

---

## 六、注意事項

### 6.1 POS系統整合
- 需要與外部POS系統進行整合
- 需要處理POS資料同步
- 需要處理POS交易資料

---

## 七、測試案例

參考商店維護系列的測試案例

---

## 八、參考資料

### 8.1 相關開發計劃
- [SYS6A04-SYS6A19-POS查詢作業系列.md](./SYS6A04-SYS6A19-POS查詢作業系列.md)
- [SYS6110-SYS6140-商店資料維護系列.md](../01-商店管理/SYS6110-SYS6140-商店資料維護系列.md)

### 8.2 技術文件
- POS系統整合文件

### 8.3 舊程式參考
- `WEB/IMS_CORE/ASP/SYS6000/SYS6610_*.ASP`
- `WEB/IMS_CORE/ASP/SYS6000/SYS6710_*.ASP`
- `WEB/IMS_CORE/ASP/SYS6000/SYS6999_*.ASP`

