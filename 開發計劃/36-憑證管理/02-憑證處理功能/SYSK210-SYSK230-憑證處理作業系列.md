# SYSK210-SYSK230 - 憑證處理作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSK210-SYSK230 系列
- **功能名稱**: 憑證處理作業系列
- **功能描述**: 提供憑證處理相關功能，包含憑證檢查、憑證導入、憑證列印、憑證匯出等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK210_*.ASP` (憑證處理相關)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK220_*.ASP` (憑證檢查相關)
  - `WEB/IMS_CORE/ASP/SYSK000/SYSK230_*.ASP` (憑證列印相關)

### 1.2 業務需求
- 支援憑證資料檢查（會計平衡檢查、資料完整性檢查）
- 支援憑證批量導入
- 支援憑證列印功能
- 支援憑證匯出功能（Excel、PDF）
- 支援憑證批量處理
- 支援憑證狀態批量更新

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表
本功能主要使用 `Vouchers` 和 `VoucherDetails` 資料表，參考「SYSK110-SYSK150-憑證資料維護系列」的資料庫設計。

### 2.2 處理記錄表: `VoucherProcessLogs` (憑證處理記錄)

```sql
CREATE TABLE [dbo].[VoucherProcessLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證編號
    [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (CHECK:檢查, IMPORT:導入, PRINT:列印, EXPORT:匯出)
    [ProcessStatus] NVARCHAR(10) NOT NULL, -- 處理狀態 (SUCCESS:成功, FAILED:失敗)
    [ProcessMessage] NVARCHAR(500) NULL, -- 處理訊息
    [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員
    [ProcessDate] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 處理時間
    [ProcessData] NVARCHAR(MAX) NULL, -- 處理資料（JSON格式）
    CONSTRAINT [FK_VoucherProcessLogs_Vouchers] FOREIGN KEY ([VoucherId]) REFERENCES [dbo].[Vouchers] ([VoucherId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_VoucherProcessLogs_VoucherId] ON [dbo].[VoucherProcessLogs] ([VoucherId]);
CREATE NONCLUSTERED INDEX [IX_VoucherProcessLogs_ProcessDate] ON [dbo].[VoucherProcessLogs] ([ProcessDate]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 憑證檢查
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/check`
- **說明**: 檢查憑證資料（會計平衡、資料完整性）
- **請求格式**:
  ```json
  {
    "voucherIds": ["VCH001", "VCH002"]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "檢查完成",
    "data": {
      "results": [
        {
          "voucherId": "VCH001",
          "status": "SUCCESS",
          "message": "檢查通過",
          "errors": []
        },
        {
          "voucherId": "VCH002",
          "status": "FAILED",
          "message": "借方貸方不平衡",
          "errors": ["借方總額與貸方總額不一致"]
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 憑證導入
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/import`
- **說明**: 批量導入憑證資料（Excel格式）
- **請求格式**: Multipart/form-data
  - `file`: Excel檔案
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "導入完成",
    "data": {
      "totalCount": 100,
      "successCount": 95,
      "failedCount": 5,
      "results": [
        {
          "rowNum": 1,
          "voucherId": "VCH001",
          "status": "SUCCESS",
          "message": "導入成功"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 憑證列印
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/print`
- **說明**: 列印憑證
- **請求格式**:
  ```json
  {
    "voucherIds": ["VCH001", "VCH002"],
    "printFormat": "PDF",
    "templateId": "VOUCHER_TEMPLATE_01"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "列印成功",
    "data": {
      "fileUrl": "/api/v1/files/print/vouchers_20240101.pdf"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 憑證匯出
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/vouchers/export`
- **說明**: 匯出憑證資料（Excel、PDF格式）
- **請求格式**:
  ```json
  {
    "voucherIds": ["VCH001", "VCH002"],
    "exportFormat": "EXCEL",
    "includeDetails": true
  }
  ```
- **回應格式**: 同憑證列印

#### 3.1.5 批量更新憑證狀態
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/vouchers/batch-status`
- **說明**: 批量更新憑證狀態
- **請求格式**:
  ```json
  {
    "voucherIds": ["VCH001", "VCH002"],
    "status": "A"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 憑證處理頁面 (`VoucherProcess.vue`)
- **路徑**: `/vouchers/process`
- **功能**: 提供憑證處理相關功能
- **主要元件**:
  - 憑證選擇區域
  - 處理功能按鈕（檢查、導入、列印、匯出）
  - 處理結果顯示區域

### 4.2 UI 元件設計

#### 4.2.1 憑證處理工具列 (`VoucherProcessToolbar.vue`)
```vue
<template>
  <div class="voucher-process-toolbar">
    <el-button type="primary" @click="handleCheck">檢查</el-button>
    <el-button type="success" @click="handleImport">導入</el-button>
    <el-button type="info" @click="handlePrint">列印</el-button>
    <el-button type="warning" @click="handleExport">匯出</el-button>
    <el-button type="danger" @click="handleBatchUpdateStatus">批量更新狀態</el-button>
  </div>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (3天)
- [ ] 憑證檢查邏輯實作
- [ ] 憑證導入邏輯實作
- [ ] 憑證列印邏輯實作
- [ ] 憑證匯出邏輯實作
- [ ] 批量處理邏輯實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] 處理工具列開發
- [ ] 處理結果顯示開發
- [ ] 檔案上傳元件開發
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試

**總計**: 6天

---

## 六、注意事項

### 6.1 資料驗證
- 導入前必須驗證資料格式
- 導入後必須檢查會計平衡
- 批量操作必須記錄操作日誌

### 6.2 效能
- 批量操作必須使用批次處理
- 大量資料匯出必須使用非同步處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 憑證檢查成功
- [ ] 憑證檢查失敗（不平衡）
- [ ] 憑證導入成功
- [ ] 憑證導入失敗（格式錯誤）
- [ ] 憑證列印成功
- [ ] 憑證匯出成功

### 7.2 整合測試
- [ ] 完整處理流程測試
- [ ] 批量操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSK000/SYSK210_*.ASP`
- `WEB/IMS_CORE/ASP/SYSK000/SYSK220_*.ASP`
- `WEB/IMS_CORE/ASP/SYSK000/SYSK230_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

