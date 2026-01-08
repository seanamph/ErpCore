# SYSP510-SYSP530 - 採購其他功能系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSP510-SYSP530系列
- **功能名稱**: 採購其他功能系列
- **功能描述**: 提供採購其他功能的新增、修改、刪除、查詢功能，包含採購輔助功能、採購工具等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP510_*.ASP`
  - `WEB/IMS_CORE/ASP/SYSP000/SYSP530_*.ASP`

### 1.2 業務需求
- 管理採購輔助功能
- 支援採購工具使用
- 支援採購資料處理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOtherFunctions` (採購其他功能)

```sql
CREATE TABLE [dbo].[PurchaseOtherFunctions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [FunctionId] NVARCHAR(50) NOT NULL, -- 功能代碼
    [FunctionName] NVARCHAR(100) NOT NULL, -- 功能名稱
    [FunctionType] NVARCHAR(20) NULL, -- 功能類型 (TOOL:工具, AUX:輔助, PROCESS:處理)
    [FunctionDesc] NVARCHAR(500) NULL, -- 功能說明
    [FunctionConfig] NVARCHAR(MAX) NULL, -- 功能配置 (JSON格式)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (A:啟用, I:停用)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_PurchaseOtherFunctions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_PurchaseOtherFunctions_FunctionId] UNIQUE ([FunctionId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionId] ON [dbo].[PurchaseOtherFunctions] ([FunctionId]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_FunctionType] ON [dbo].[PurchaseOtherFunctions] ([FunctionType]);
CREATE NONCLUSTERED INDEX [IX_PurchaseOtherFunctions_Status] ON [dbo].[PurchaseOtherFunctions] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| FunctionId | NVARCHAR | 50 | NO | - | 功能代碼 | 唯一 |
| FunctionName | NVARCHAR | 100 | NO | - | 功能名稱 | - |
| FunctionType | NVARCHAR | 20 | YES | - | 功能類型 | TOOL:工具, AUX:輔助, PROCESS:處理 |
| FunctionDesc | NVARCHAR | 500 | YES | - | 功能說明 | - |
| FunctionConfig | NVARCHAR(MAX) | - | YES | - | 功能配置 | JSON格式 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢採購其他功能列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-others`
- **說明**: 查詢採購其他功能列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "FunctionId",
    "sortOrder": "ASC",
    "filters": {
      "functionId": "",
      "functionName": "",
      "functionType": "",
      "status": ""
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
          "tKey": 1,
          "functionId": "FUNC001",
          "functionName": "採購輔助功能1",
          "functionType": "AUX",
          "functionDesc": "功能說明",
          "status": "A",
          "seqNo": 1
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

#### 3.1.2 查詢單筆採購其他功能
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-others/{tKey}`
- **說明**: 根據主鍵查詢單筆採購其他功能資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 根據功能代碼查詢
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-others/by-id/{functionId}`
- **說明**: 根據功能代碼查詢採購其他功能資料
- **路徑參數**:
  - `functionId`: 功能代碼
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.4 新增採購其他功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-others`
- **說明**: 新增採購其他功能資料
- **請求格式**:
  ```json
  {
    "functionId": "FUNC001",
    "functionName": "採購輔助功能1",
    "functionType": "AUX",
    "functionDesc": "功能說明",
    "functionConfig": "{}",
    "status": "A",
    "seqNo": 1,
    "memo": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改採購其他功能
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-others/{tKey}`
- **說明**: 修改採購其他功能資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `functionId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除採購其他功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-others/{tKey}`
- **說明**: 刪除採購其他功能資料（軟刪除或硬刪除）
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購其他功能頁面 (`PurchaseOtherFunctions.vue`)
- **路徑**: `/procurement/purchase-others`
- **功能**: 顯示採購其他功能管理介面

### 4.2 頁面元件

#### 4.2.1 查詢條件區塊
- 功能代碼輸入框
- 功能名稱輸入框
- 功能類型下拉選單（TOOL:工具, AUX:輔助, PROCESS:處理）
- 狀態下拉選單（A:啟用, I:停用）
- 查詢按鈕
- 重置按鈕

#### 4.2.2 資料列表區塊
- 資料表格顯示：
  - 功能代碼
  - 功能名稱
  - 功能類型
  - 功能說明
  - 狀態
  - 排序序號
  - 操作按鈕（新增、修改、刪除、查詢）
- 分頁控制元件
- 排序功能

#### 4.2.3 新增/修改表單區塊
- 功能代碼輸入框（新增時必填，修改時唯讀）
- 功能名稱輸入框（必填）
- 功能類型下拉選單（必填）
- 功能說明文字區塊
- 功能配置JSON編輯器
- 狀態下拉選單（必填）
- 排序序號輸入框
- 備註文字區塊
- 儲存按鈕
- 取消按鈕

### 4.3 表單驗證規則

- 功能代碼：必填，長度1-50，唯一
- 功能名稱：必填，長度1-100
- 功能類型：必填，必須為TOOL、AUX或PROCESS之一
- 狀態：必填，必須為A或I之一
- 排序序號：數字，範圍0-9999

---

## 五、開發時程

**總計**: 10天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者權限
- 敏感資料必須加密傳輸

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增採購其他功能成功
- [ ] 修改採購其他功能成功
- [ ] 刪除採購其他功能成功
- [ ] 查詢採購其他功能列表成功

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSP000/SYSP510_*.ASP`
- `WEB/IMS_CORE/ASP/SYSP000/SYSP530_*.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

