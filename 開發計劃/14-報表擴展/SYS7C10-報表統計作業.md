# SYS7C10 - 報表統計作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS7C10
- **功能名稱**: 報表統計作業
- **功能描述**: 提供報表模組7的報表統計功能，支援資料統計、圖表展示、統計報表列印等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYS7000/SYS7C10.ascx.cs` (使用者控制項後端程式碼)

### 1.2 業務需求
- 提供報表資料統計功能
- 支援多種統計維度（日期、類別、區域等）
- 支援統計圖表展示（柱狀圖、折線圖、圓餅圖等）
- 支援統計報表列印
- 支援統計資料匯出（Excel、PDF）
- 記錄統計查詢記錄

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReportStatistics` (報表統計記錄)

```sql
CREATE TABLE [dbo].[ReportStatistics] (
    [StatId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 統計ID
    [ReportCode] NVARCHAR(50) NOT NULL, -- 報表代碼 (SYS7C10)
    [ReportName] NVARCHAR(200) NOT NULL, -- 報表名稱
    [StatParams] NVARCHAR(MAX) NULL, -- 統計參數 (JSON格式)
    [StatType] NVARCHAR(50) NULL, -- 統計類型 (SUM, AVG, COUNT, MAX, MIN等)
    [StatResult] NVARCHAR(MAX) NULL, -- 統計結果 (JSON格式)
    [ChartType] NVARCHAR(50) NULL, -- 圖表類型 (BAR, LINE, PIE等)
    [StatStatus] NVARCHAR(20) NOT NULL DEFAULT 'COMPLETED', -- 統計狀態
    [QueriedBy] NVARCHAR(50) NULL, -- 查詢者
    [QueriedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 查詢時間
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReportStatistics] PRIMARY KEY CLUSTERED ([StatId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReportStatistics_ReportCode] ON [dbo].[ReportStatistics] ([ReportCode]);
CREATE NONCLUSTERED INDEX [IX_ReportStatistics_QueriedBy] ON [dbo].[ReportStatistics] ([QueriedBy]);
CREATE NONCLUSTERED INDEX [IX_ReportStatistics_QueriedAt] ON [dbo].[ReportStatistics] ([QueriedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `ReportQueries` - 報表查詢設定
- 參考: `開發計劃/14-報表擴展/SYS7000-報表模組7-報表查詢.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| StatId | BIGINT | - | NO | IDENTITY(1,1) | 統計ID | 主鍵 |
| ReportCode | NVARCHAR | 50 | NO | - | 報表代碼 | SYS7C10 |
| ReportName | NVARCHAR | 200 | NO | - | 報表名稱 | - |
| StatParams | NVARCHAR(MAX) | - | YES | - | 統計參數 | JSON格式 |
| StatType | NVARCHAR | 50 | YES | - | 統計類型 | SUM, AVG, COUNT等 |
| StatResult | NVARCHAR(MAX) | - | YES | - | 統計結果 | JSON格式 |
| ChartType | NVARCHAR | 50 | YES | - | 圖表類型 | BAR, LINE, PIE等 |
| StatStatus | NVARCHAR | 20 | NO | 'COMPLETED' | 統計狀態 | - |
| QueriedBy | NVARCHAR | 50 | YES | - | 查詢者 | - |
| QueriedAt | DATETIME2 | - | NO | GETDATE() | 查詢時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表統計記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7c10/statistics`
- **說明**: 查詢報表統計記錄列表，支援分頁、排序、篩選

#### 3.1.2 執行報表統計
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7c10/statistics/calculate`
- **說明**: 執行報表統計，計算統計結果

#### 3.1.3 查詢單筆統計記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sys7000/sys7c10/statistics/{statId}`
- **說明**: 根據統計ID查詢單筆統計記錄

#### 3.1.4 匯出統計報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sys7000/sys7c10/statistics/{statId}/export`
- **說明**: 匯出統計報表（Excel、PDF）

#### 3.1.5 刪除統計記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/reports/sys7000/sys7c10/statistics/{statId}`
- **說明**: 刪除統計記錄

---

## 四、前端 UI 設計

### 4.1 報表統計頁面 (`SYS7C10Statistics.vue`)

#### 4.1.1 頁面結構
- 查詢條件表單
- 統計結果展示（表格、圖表）
- 統計記錄列表
- 匯出功能

#### 4.1.2 圖表展示
- 使用 Chart.js 或 ECharts 展示統計圖表
- 支援多種圖表類型（柱狀圖、折線圖、圓餅圖等）

---

## 五、後端實作

### 5.1 Controller (`SYS7C10ReportStatisticsController.cs`)
實作報表統計相關的 API 端點

### 5.2 Service (`ReportStatisticsService.cs`)
實作報表統計的業務邏輯，包括資料統計計算、圖表資料生成等

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
2. **後端 API 開發** (3 天)
3. **前端 UI 開發** (2.5 天)
4. **測試與優化** (0.5 天)

**總計**: 6.5 天

---

## 七、注意事項

### 7.1 統計計算
- 注意大量資料的統計效能
- 考慮使用資料庫的聚合函數
- 支援非同步統計計算

### 7.2 圖表展示
- 選擇合適的圖表庫（Chart.js、ECharts等）
- 注意圖表資料格式
- 支援圖表匯出

### 7.3 效能優化
- 大量資料時使用分頁查詢
- 統計計算使用非同步處理
- 考慮快取統計結果

---

## 八、測試案例

### 8.1 功能測試
1. **統計計算測試**
   - 正常統計計算
   - 不同統計類型測試
   - 大量資料統計測試

2. **圖表展示測試**
   - 圖表資料正確性
   - 不同圖表類型展示

3. **匯出測試**
   - Excel匯出
   - PDF匯出

### 8.2 效能測試
- 大量資料統計效能
- 並發統計測試

