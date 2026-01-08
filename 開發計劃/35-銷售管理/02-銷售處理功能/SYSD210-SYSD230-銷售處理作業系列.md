# SYSD210-SYSD230 - 銷售處理作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSD210-SYSD230 系列
- **功能名稱**: 銷售處理作業系列
- **功能描述**: 提供銷售處理相關功能，包含銷售單出貨處理、銷售單退貨處理、銷售單取消處理等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD210_*.ASP` (銷售出貨處理相關)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD220_*.ASP` (銷售退貨處理相關)
  - `WEB/IMS_CORE/ASP/SYSD000/SYSD230_*.ASP` (銷售取消處理相關)

### 1.2 業務需求
- 支援銷售單出貨處理
- 支援銷售單退貨處理
- 支援銷售單取消處理
- 支援銷售單狀態更新
- 支援庫存自動扣減/回補
- 支援銷售單處理記錄

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表
本功能主要使用 `SalesOrders` 和 `SalesOrderDetails` 資料表，參考「SYSD110-SYSD140-銷售資料維護系列」的資料庫設計。

### 2.2 處理記錄表: `SalesProcessLogs` (銷售處理記錄)

```sql
CREATE TABLE [dbo].[SalesProcessLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [OrderId] NVARCHAR(50) NOT NULL, -- 銷售單號
    [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (SHIP:出貨, RETURN:退貨, CANCEL:取消)
    [ProcessStatus] NVARCHAR(10) NOT NULL, -- 處理狀態 (SUCCESS:成功, FAILED:失敗)
    [ProcessMessage] NVARCHAR(500) NULL, -- 處理訊息
    [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
    [ProcessDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 處理時間
    [ProcessData] NVARCHAR(MAX) NULL, -- 處理資料（JSON格式）
    CONSTRAINT [FK_SalesProcessLogs_SalesOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[SalesOrders] ([OrderId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SalesProcessLogs_OrderId] ON [dbo].[SalesProcessLogs] ([OrderId]);
CREATE NONCLUSTERED INDEX [IX_SalesProcessLogs_ProcessDate] ON [dbo].[SalesProcessLogs] ([ProcessDate]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 銷售單出貨處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/ship`
- **說明**: 處理銷售單出貨
- **請求格式**:
  ```json
  {
    "shipDate": "2024-01-01",
    "shipDetails": [
      {
        "lineNum": 1,
        "shippedQty": 10.00
      }
    ],
    "memo": "出貨備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "出貨成功",
    "data": {
      "orderId": "SO001",
      "status": "O",
      "shipDate": "2024-01-01"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 銷售單退貨處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/return`
- **說明**: 處理銷售單退貨
- **請求格式**: 同出貨處理
- **回應格式**: 同出貨處理

#### 3.1.3 銷售單取消處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sales-orders/{orderId}/cancel`
- **說明**: 處理銷售單取消
- **請求格式**:
  ```json
  {
    "cancelReason": "客戶取消",
    "memo": "取消備註"
  }
  ```
- **回應格式**: 同出貨處理

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 銷售處理頁面 (`SalesProcess.vue`)
- **路徑**: `/sales-orders/process`
- **功能**: 提供銷售處理相關功能
- **主要元件**:
  - 銷售單選擇區域
  - 處理功能按鈕（出貨、退貨、取消）
  - 處理結果顯示區域

---

## 五、開發時程

### 5.1 階段一: 後端開發 (3天)
- [ ] 出貨處理邏輯實作
- [ ] 退貨處理邏輯實作
- [ ] 取消處理邏輯實作
- [ ] 庫存處理邏輯實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] 處理頁面開發
- [ ] 處理結果顯示開發
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試

**總計**: 6天

---

## 六、注意事項

### 6.1 業務邏輯
- 出貨前必須檢查庫存
- 退貨後必須回補庫存
- 取消後必須回補庫存
- 已出貨的銷售單不可取消

---

## 七、測試案例

### 7.1 單元測試
- [ ] 出貨處理成功
- [ ] 出貨處理失敗（庫存不足）
- [ ] 退貨處理成功
- [ ] 取消處理成功

### 7.2 整合測試
- [ ] 完整處理流程測試
- [ ] 庫存處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSD000/SYSD210_*.ASP`
- `WEB/IMS_CORE/ASP/SYSD000/SYSD220_*.ASP`
- `WEB/IMS_CORE/ASP/SYSD000/SYSD230_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

