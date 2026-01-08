# XCOMMSG - 錯誤訊息處理 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOMMSG
- **功能名稱**: 錯誤訊息處理
- **功能描述**: 提供系統錯誤訊息處理功能，包含錯誤頁面、錯誤訊息顯示、錯誤記錄等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOMMSG/ERRORPAGE.ASP` (錯誤頁面)
  - `WEB/IMS_CORE/ASP/XCOMMSG/401.asp` (HTTP 401錯誤)
  - `WEB/IMS_CORE/ASP/XCOMMSG/404.asp` (HTTP 404錯誤)
  - `WEB/IMS_CORE/ASP/XCOMMSG/500.asp` (HTTP 500錯誤)

### 1.2 業務需求
- 提供統一的錯誤訊息處理機制
- 支援HTTP錯誤頁面（401、404、500）
- 記錄錯誤訊息到日誌
- 提供友好的錯誤訊息顯示
- 支援錯誤訊息的多語言顯示
- 與系統日誌模組整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ErrorMessages` (錯誤訊息記錄)

```sql
CREATE TABLE [dbo].[ErrorMessages] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ErrorCode] NVARCHAR(50) NOT NULL, -- 錯誤代碼
    [ErrorType] NVARCHAR(20) NOT NULL, -- 錯誤類型 (HTTP, APPLICATION, SYSTEM)
    [HttpStatusCode] INT NULL, -- HTTP狀態碼 (401, 404, 500等)
    [ErrorMessage] NVARCHAR(500) NOT NULL, -- 錯誤訊息
    [ErrorDetail] NVARCHAR(MAX) NULL, -- 錯誤詳細資訊
    [RequestUrl] NVARCHAR(500) NULL, -- 請求URL
    [RequestMethod] NVARCHAR(10) NULL, -- 請求方法 (GET, POST等)
    [UserId] NVARCHAR(50) NULL, -- 使用者ID
    [UserIp] NVARCHAR(50) NULL, -- 使用者IP
    [UserAgent] NVARCHAR(500) NULL, -- 使用者代理
    [StackTrace] NVARCHAR(MAX) NULL, -- 堆疊追蹤
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 發生時間
    CONSTRAINT [PK_ErrorMessages] PRIMARY KEY CLUSTERED ([TKey] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ErrorMessages_ErrorCode] ON [dbo].[ErrorMessages] ([ErrorCode]);
CREATE NONCLUSTERED INDEX [IX_ErrorMessages_ErrorType] ON [dbo].[ErrorMessages] ([ErrorType]);
CREATE NONCLUSTERED INDEX [IX_ErrorMessages_HttpStatusCode] ON [dbo].[ErrorMessages] ([HttpStatusCode]);
CREATE NONCLUSTERED INDEX [IX_ErrorMessages_UserId] ON [dbo].[ErrorMessages] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_ErrorMessages_CreatedAt] ON [dbo].[ErrorMessages] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `ErrorMessageTemplates` - 錯誤訊息模板
```sql
CREATE TABLE [dbo].[ErrorMessageTemplates] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ErrorCode] NVARCHAR(50) NOT NULL, -- 錯誤代碼
    [Language] NVARCHAR(10) NOT NULL DEFAULT 'zh-TW', -- 語言代碼
    [Title] NVARCHAR(200) NOT NULL, -- 錯誤標題
    [Message] NVARCHAR(500) NOT NULL, -- 錯誤訊息
    [Description] NVARCHAR(1000) NULL, -- 錯誤描述
    [Solution] NVARCHAR(1000) NULL, -- 解決方案
    [IsActive] BIT NOT NULL DEFAULT 1, -- 是否啟用
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_ErrorMessageTemplates_ErrorCode_Language] UNIQUE ([ErrorCode], [Language])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| ErrorCode | NVARCHAR | 50 | NO | - | 錯誤代碼 | - |
| ErrorType | NVARCHAR | 20 | NO | - | 錯誤類型 | HTTP, APPLICATION, SYSTEM |
| HttpStatusCode | INT | - | YES | - | HTTP狀態碼 | 401, 404, 500等 |
| ErrorMessage | NVARCHAR | 500 | NO | - | 錯誤訊息 | - |
| ErrorDetail | NVARCHAR | MAX | YES | - | 錯誤詳細資訊 | - |
| RequestUrl | NVARCHAR | 500 | YES | - | 請求URL | - |
| RequestMethod | NVARCHAR | 10 | YES | - | 請求方法 | GET, POST等 |
| UserId | NVARCHAR | 50 | YES | - | 使用者ID | - |
| UserIp | NVARCHAR | 50 | YES | - | 使用者IP | - |
| UserAgent | NVARCHAR | 500 | YES | - | 使用者代理 | - |
| StackTrace | NVARCHAR | MAX | YES | - | 堆疊追蹤 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 發生時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢錯誤訊息列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/error-messages`
- **說明**: 查詢錯誤訊息列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "CreatedAt",
    "sortOrder": "DESC",
    "filters": {
      "errorCode": "",
      "errorType": "",
      "httpStatusCode": null,
      "userId": "",
      "startDate": "2024-01-01",
      "endDate": "2024-01-31"
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
          "errorCode": "404",
          "errorType": "HTTP",
          "httpStatusCode": 404,
          "errorMessage": "找不到請求的資源",
          "requestUrl": "/api/v1/users/999",
          "requestMethod": "GET",
          "userId": "U001",
          "userIp": "192.168.1.1",
          "createdAt": "2024-01-01T10:00:00"
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

#### 3.1.2 查詢單筆錯誤訊息
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/error-messages/{tKey}`
- **說明**: 根據主鍵查詢單筆錯誤訊息詳細資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 包含完整的錯誤訊息詳細資料，包括堆疊追蹤等

#### 3.1.3 記錄錯誤訊息
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/error-messages`
- **說明**: 記錄新的錯誤訊息
- **請求格式**:
  ```json
  {
    "errorCode": "404",
    "errorType": "HTTP",
    "httpStatusCode": 404,
    "errorMessage": "找不到請求的資源",
    "errorDetail": "詳細錯誤資訊",
    "requestUrl": "/api/v1/users/999",
    "requestMethod": "GET",
    "userId": "U001",
    "userIp": "192.168.1.1",
    "userAgent": "Mozilla/5.0...",
    "stackTrace": "堆疊追蹤資訊"
  }
  ```

#### 3.1.4 查詢錯誤訊息模板
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/error-message-templates`
- **說明**: 查詢錯誤訊息模板列表
- **請求參數**:
  ```json
  {
    "errorCode": "",
    "language": "zh-TW"
  }
  ```

#### 3.1.5 取得錯誤訊息模板
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/error-message-templates/{errorCode}`
- **說明**: 根據錯誤代碼取得錯誤訊息模板
- **路徑參數**:
  - `errorCode`: 錯誤代碼
- **查詢參數**:
  - `language`: 語言代碼（預設: zh-TW）

### 3.2 後端實作類別

#### 3.2.1 Controller: `ErrorMessagesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/error-messages")]
    [Authorize]
    public class ErrorMessagesController : ControllerBase
    {
        private readonly IErrorMessageService _errorMessageService;
        
        public ErrorMessagesController(IErrorMessageService errorMessageService)
        {
            _errorMessageService = errorMessageService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ErrorMessageDto>>>> GetErrorMessages([FromQuery] ErrorMessageQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<ErrorMessageDto>>> GetErrorMessage(long tKey)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateErrorMessage([FromBody] CreateErrorMessageDto dto)
        {
            // 實作記錄錯誤邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 錯誤訊息列表頁面 (`ErrorMessageList.vue`)
- **路徑**: `/system/error-messages`
- **功能**: 顯示錯誤訊息列表，支援查詢、篩選、詳細查看

#### 4.1.2 錯誤頁面元件 (`ErrorPage.vue`)
- **路徑**: 用於顯示各種HTTP錯誤頁面
- **功能**: 顯示友好的錯誤訊息，包含錯誤代碼、錯誤訊息、返回按鈕等

### 4.2 UI 元件設計

#### 4.2.1 錯誤頁面元件 (`ErrorPage.vue`)
```vue
<template>
  <div class="error-page">
    <div class="error-content">
      <h1 class="error-code">{{ errorCode }}</h1>
      <h2 class="error-title">{{ errorTitle }}</h2>
      <p class="error-message">{{ errorMessage }}</p>
      <div class="error-actions">
        <el-button type="primary" @click="goHome">返回首頁</el-button>
        <el-button @click="goBack">返回上一頁</el-button>
      </div>
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`errorMessage.api.ts`)
```typescript
import request from '@/utils/request';

export interface ErrorMessageDto {
  tKey: number;
  errorCode: string;
  errorType: string;
  httpStatusCode?: number;
  errorMessage: string;
  errorDetail?: string;
  requestUrl?: string;
  requestMethod?: string;
  userId?: string;
  userIp?: string;
  userAgent?: string;
  stackTrace?: string;
  createdAt: string;
}

export const getErrorMessages = (query: ErrorMessageQueryDto) => {
  return request.get<ApiResponse<PagedResult<ErrorMessageDto>>>('/api/v1/error-messages', { params: query });
};

export const getErrorMessage = (tKey: number) => {
  return request.get<ApiResponse<ErrorMessageDto>>(`/api/v1/error-messages/${tKey}`);
};

export const createErrorMessage = (data: CreateErrorMessageDto) => {
  return request.post<ApiResponse<number>>('/api/v1/error-messages', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 錯誤處理中介軟體
- [ ] 單元測試

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 錯誤頁面元件開發
- [ ] 錯誤訊息列表頁面開發
- [ ] 錯誤處理攔截器
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 錯誤處理流程測試
- [ ] 多語言錯誤訊息測試

**總計**: 4.5天

---

## 六、注意事項

### 6.1 安全性
- 敏感錯誤訊息不應暴露給使用者
- 堆疊追蹤僅在開發環境顯示
- 記錄使用者IP和操作資訊

### 6.2 效能
- 錯誤訊息記錄應使用非同步處理
- 大量錯誤訊息查詢必須使用分頁
- 必須建立適當的索引

### 6.3 業務邏輯
- 錯誤訊息應分類管理
- 支援多語言錯誤訊息
- 與系統日誌模組整合

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

