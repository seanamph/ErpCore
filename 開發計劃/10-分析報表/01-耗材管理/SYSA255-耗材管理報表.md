# SYSA255 - 耗材管理報表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSA255
- **功能名稱**: 耗材管理報表
- **功能描述**: 提供耗材管理報表的查詢與列印功能，包含耗材基本資料、庫存狀況、使用狀況、成本分析等資訊，支援多種查詢條件和報表格式
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSA000/SYSA255.ascx` (報表頁面)
  - `WEB/IMS_CORE/SYSA000/SYSA255.ascx.cs` (後端邏輯)
  - `WEB/IMS_CORE/SYSA000/js/SYSA255.js` (前端邏輯)

### 1.2 業務需求
- 支援耗材基本資料查詢
- 支援耗材庫存狀況查詢
- 支援耗材使用狀況查詢
- 支援耗材成本分析
- 支援多種查詢條件（店別、分類、狀態、日期範圍等）
- 支援報表列印與匯出（Excel、PDF）
- 支援多種報表格式

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `Consumables` - 耗材主檔
參考 `開發計劃/10-分析報表/01-耗材管理/SYSA254-耗材標籤列印作業.md` 的 `Consumables` 資料表設計

#### 2.1.2 `ConsumableTransactions` - 耗材異動記錄
```sql
CREATE TABLE [dbo].[ConsumableTransactions] (
    [TransactionId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ConsumableId] NVARCHAR(50) NOT NULL, -- 耗材編號
    [TransactionType] NVARCHAR(10) NOT NULL, -- 異動類型 (1:入庫, 2:出庫, 3:退貨, 4:報廢, 5:出售, 6:領用)
    [TransactionDate] DATETIME2 NOT NULL, -- 異動日期
    [Quantity] DECIMAL(18, 2) NOT NULL, -- 數量
    [UnitPrice] DECIMAL(18, 2) NULL, -- 單價
    [Amount] DECIMAL(18, 2) NULL, -- 金額
    [SiteId] NVARCHAR(50) NULL, -- 店別代碼
    [WarehouseId] NVARCHAR(50) NULL, -- 庫別代碼
    [SourceId] NVARCHAR(50) NULL, -- 來源單號
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_ConsumableTransactions_Consumables] FOREIGN KEY ([ConsumableId]) REFERENCES [dbo].[Consumables] ([ConsumableId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_ConsumableId] ON [dbo].[ConsumableTransactions] ([ConsumableId]);
CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_TransactionDate] ON [dbo].[ConsumableTransactions] ([TransactionDate]);
CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_TransactionType] ON [dbo].[ConsumableTransactions] ([TransactionType]);
CREATE NONCLUSTERED INDEX [IX_ConsumableTransactions_SiteId] ON [dbo].[ConsumableTransactions] ([SiteId]);
```

#### 2.1.3 `ConsumableUsage` - 耗材使用統計視圖（可選）
```sql
CREATE VIEW [dbo].[ConsumableUsage] AS
SELECT 
    c.[ConsumableId],
    c.[ConsumableName],
    c.[CategoryId],
    c.[SiteId],
    c.[WarehouseId],
    SUM(CASE WHEN t.[TransactionType] = '1' THEN t.[Quantity] ELSE 0 END) AS [InQty], -- 入庫數量
    SUM(CASE WHEN t.[TransactionType] IN ('2', '6') THEN t.[Quantity] ELSE 0 END) AS [OutQty], -- 出庫數量
    SUM(CASE WHEN t.[TransactionType] = '1' THEN t.[Amount] ELSE 0 END) AS [InAmt], -- 入庫金額
    SUM(CASE WHEN t.[TransactionType] IN ('2', '6') THEN t.[Amount] ELSE 0 END) AS [OutAmt], -- 出庫金額
    c.[Quantity] AS [CurrentQty], -- 當前庫存數量
    c.[Price] * c.[Quantity] AS [CurrentAmt] -- 當前庫存金額
FROM [dbo].[Consumables] c
LEFT JOIN [dbo].[ConsumableTransactions] t ON c.[ConsumableId] = t.[ConsumableId]
GROUP BY c.[ConsumableId], c.[ConsumableName], c.[CategoryId], c.[SiteId], c.[WarehouseId], c.[Quantity], c.[Price];
```

### 2.2 資料字典

#### ConsumableTransactions 資料表欄位

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TransactionId | BIGINT | - | NO | IDENTITY(1,1) | 異動記錄ID | 主鍵 |
| ConsumableId | NVARCHAR | 50 | NO | - | 耗材編號 | 外鍵至Consumables |
| TransactionType | NVARCHAR | 10 | NO | - | 異動類型 | 1:入庫, 2:出庫, 3:退貨, 4:報廢, 5:出售, 6:領用 |
| TransactionDate | DATETIME2 | - | NO | - | 異動日期 | - |
| Quantity | DECIMAL | 18,2 | NO | - | 數量 | - |
| UnitPrice | DECIMAL | 18,2 | YES | - | 單價 | - |
| Amount | DECIMAL | 18,2 | YES | - | 金額 | - |
| SiteId | NVARCHAR | 50 | YES | - | 店別代碼 | - |
| WarehouseId | NVARCHAR | 50 | YES | - | 庫別代碼 | - |
| SourceId | NVARCHAR | 50 | YES | - | 來源單號 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢耗材管理報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumables/report`
- **說明**: 查詢耗材管理報表資料，支援多種查詢條件
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ConsumableId",
    "sortOrder": "ASC",
    "filters": {
      "consumableId": "",
      "consumableName": "",
      "categoryId": "",
      "siteIds": [],
      "warehouseIds": [],
      "status": "",
      "assetStatus": "",
      "dateFrom": "",
      "dateTo": "",
      "reportType": "Summary"
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "consumableId": "C001",
          "consumableName": "耗材名稱",
          "categoryId": "CAT001",
          "categoryName": "分類名稱",
          "siteId": "S001",
          "siteName": "總店",
          "warehouseId": "W001",
          "warehouseName": "庫別名稱",
          "unit": "個",
          "specification": "規格",
          "brand": "品牌",
          "model": "型號",
          "barCode": "1234567890",
          "status": "1",
          "statusName": "正常",
          "assetStatus": "A",
          "assetStatusName": "使用中",
          "location": "位置",
          "quantity": 100.00,
          "minQuantity": 20.00,
          "maxQuantity": 200.00,
          "price": 50.00,
          "currentQty": 100.00,
          "currentAmt": 5000.00,
          "inQty": 200.00,
          "outQty": 100.00,
          "inAmt": 10000.00,
          "outAmt": 5000.00,
          "isLowStock": false,
          "isOverStock": false
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5,
      "summary": {
        "totalConsumables": 100,
        "totalCurrentQty": 10000.00,
        "totalCurrentAmt": 500000.00,
        "totalInQty": 20000.00,
        "totalOutQty": 10000.00,
        "totalInAmt": 1000000.00,
        "totalOutAmt": 500000.00
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出耗材管理報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/consumables/report/export`
- **說明**: 匯出耗材管理報表（Excel、PDF）
- **請求格式**:
  ```json
  {
    "exportType": "Excel",
    "reportType": "Summary",
    "filters": {
      "consumableId": "",
      "consumableName": "",
      "categoryId": "",
      "siteIds": [],
      "warehouseIds": [],
      "status": "",
      "dateFrom": "",
      "dateTo": ""
    }
  }
  ```
- **回應格式**: 檔案下載

#### 3.1.3 查詢耗材使用明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/consumables/{consumableId}/transactions`
- **說明**: 查詢指定耗材的使用明細
- **路徑參數**:
  - `consumableId`: 耗材編號
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "dateFrom": "",
    "dateTo": "",
    "transactionType": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "transactionId": 1,
          "transactionType": "1",
          "transactionTypeName": "入庫",
          "transactionDate": "2024-01-01T00:00:00",
          "quantity": 100.00,
          "unitPrice": 50.00,
          "amount": 5000.00,
          "siteId": "S001",
          "siteName": "總店",
          "warehouseId": "W001",
          "warehouseName": "庫別名稱",
          "sourceId": "PO001",
          "notes": "備註",
          "createdBy": "U001",
          "createdAt": "2024-01-01T00:00:00"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 查詢頁面 (`ConsumableReportQuery.vue`)

#### 4.1.1 查詢條件區域
- **耗材編號**: 耗材編號（文字輸入框）
- **耗材名稱**: 耗材名稱（文字輸入框，支援模糊查詢）
- **分類**: 分類下拉列表
- **店別**: 多選下拉列表（支援多選）
- **庫別**: 多選下拉列表（支援多選）
- **狀態**: 狀態下拉列表（正常、停用等）
- **資產狀態**: 資產狀態下拉列表
- **日期範圍**: 日期起、日期迄（日期選擇器）
- **報表類型**: 報表類型下拉列表（摘要、明細、成本分析等）

#### 4.1.2 查詢結果區域
- **資料表格**: 
  - 耗材編號、耗材名稱、分類、店別、庫別、單位、規格、品牌、型號、條碼
  - 狀態、資產狀態、位置、當前庫存數量、當前庫存金額
  - 入庫數量、出庫數量、入庫金額、出庫金額
  - 最小庫存量、最大庫存量、是否低庫存、是否過量庫存
- **分頁控制**: 頁碼、每頁筆數
- **排序功能**: 點擊欄位標題排序
- **匯出功能**: Excel匯出、PDF匯出按鈕

#### 4.1.3 統計資訊區域
- **總耗材數**: 統計值
- **總當前庫存數量**: 統計值
- **總當前庫存金額**: 統計值
- **總入庫數量**: 統計值
- **總出庫數量**: 統計值
- **總入庫金額**: 統計值
- **總出庫金額**: 統計值

### 4.2 報表列印頁面 (`ConsumableReportPrint.vue`)
- **報表標題**: 耗材管理報表
- **報表內容**: 與查詢結果相同格式
- **列印按鈕**: 列印功能
- **匯出按鈕**: Excel、PDF匯出

### 4.3 耗材使用明細頁面 (`ConsumableTransactionDetail.vue`)
- **耗材基本資訊**: 顯示耗材基本資料
- **使用明細表格**: 
  - 異動日期、異動類型、數量、單價、金額、店別、庫別、來源單號、備註
- **分頁控制**: 頁碼、每頁筆數
- **匯出功能**: Excel匯出、PDF匯出按鈕

---

## 五、業務邏輯

### 5.1 查詢邏輯
1. **參數驗證**: 驗證查詢參數的有效性
2. **權限檢查**: 檢查使用者是否有查詢權限
3. **資料查詢**: 根據查詢條件從資料庫查詢耗材管理報表資料
4. **資料統計**: 計算總耗材數、總當前庫存數量、總當前庫存金額、總入庫數量、總出庫數量、總入庫金額、總出庫金額
5. **庫存預警**: 判斷是否低庫存、過量庫存
6. **資料分頁**: 對查詢結果進行分頁處理
7. **資料排序**: 根據排序欄位和排序方向排序

### 5.2 匯出邏輯
1. **資料查詢**: 根據查詢條件查詢所有符合條件的資料（不分頁）
2. **格式轉換**: 將資料轉換為Excel或PDF格式
3. **檔案生成**: 生成匯出檔案
4. **檔案下載**: 提供檔案下載連結

### 5.3 使用明細邏輯
1. **參數驗證**: 驗證查詢參數的有效性
2. **權限檢查**: 檢查使用者是否有查詢權限
3. **資料查詢**: 根據耗材編號和查詢條件從資料庫查詢使用明細
4. **資料分頁**: 對查詢結果進行分頁處理
5. **資料排序**: 根據排序欄位和排序方向排序

---

## 六、開發任務

### 6.1 後端開發
- [ ] 建立 `ConsumableReportController` 控制器
- [ ] 建立 `ConsumableReportService` 服務層
- [ ] 建立 `ConsumableReportRepository` 資料存取層
- [ ] 建立 `ConsumableTransactionRepository` 資料存取層
- [ ] 建立 `ConsumableReportDto` 資料傳輸物件
- [ ] 建立 `ConsumableTransactionDto` 資料傳輸物件
- [ ] 實作查詢API (`GET /api/v1/consumables/report`)
- [ ] 實作匯出API (`POST /api/v1/consumables/report/export`)
- [ ] 實作使用明細API (`GET /api/v1/consumables/{consumableId}/transactions`)
- [ ] 建立資料庫視圖 `ConsumableUsage`（可選）
- [ ] 單元測試

### 6.2 前端開發
- [ ] 建立 `ConsumableReportQuery.vue` 查詢頁面
- [ ] 建立 `ConsumableReportPrint.vue` 報表頁面
- [ ] 建立 `ConsumableTransactionDetail.vue` 使用明細頁面
- [ ] 建立 `consumableReportApi.ts` API服務
- [ ] 建立 `consumableReportStore.ts` 狀態管理（如需要）
- [ ] 實作查詢功能
- [ ] 實作匯出功能（Excel、PDF）
- [ ] 實作使用明細功能
- [ ] 單元測試

### 6.3 測試
- [ ] 單元測試
- [ ] 整合測試
- [ ] 效能測試
- [ ] 使用者驗收測試

---

## 七、注意事項

1. **效能優化**: 耗材管理報表可能涉及大量資料，需要優化查詢效能，考慮使用索引、分頁、快取等技術
2. **資料一致性**: 確保耗材資料的一致性，考慮使用事務處理
3. **權限控制**: 確保只有有權限的使用者才能查詢和匯出報表
4. **報表格式**: 確保匯出的Excel和PDF格式符合需求
5. **庫存預警**: 庫存預警功能需要即時更新，可以考慮使用快取技術

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

