# RslHtml2Pdf - HTML轉PDF工具 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: RslHtml2Pdf
- **功能名稱**: HTML轉PDF工具
- **功能描述**: 提供將HTML內容轉換為PDF文件的功能，支援報表、文件等格式轉換
- **參考舊程式**: 
  - `WEB/IMS_CORE/RslHtml2Pdf/` 目錄下的相關文件

### 1.2 業務需求
- 將HTML格式的報表轉換為PDF文件
- 支援自訂PDF樣式與格式
- 支援批量轉換功能
- 支援PDF文件下載與列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PdfConversionLogs`

```sql
CREATE TABLE [dbo].[PdfConversionLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [SourceHtml] NVARCHAR(MAX) NULL,
    [PdfFilePath] NVARCHAR(500) NULL,
    [FileName] NVARCHAR(255) NULL,
    [FileSize] BIGINT NULL,
    [ConversionStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CompletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_PdfConversionLogs] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PdfConversionLogs_Status] ON [dbo].[PdfConversionLogs] ([ConversionStatus]);
CREATE NONCLUSTERED INDEX [IX_PdfConversionLogs_CreatedAt] ON [dbo].[PdfConversionLogs] ([CreatedAt]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LogId | UNIQUEIDENTIFIER | - | NO | NEWID() | 記錄編號 | 主鍵 |
| SourceHtml | NVARCHAR | MAX | YES | - | 來源HTML內容 | - |
| PdfFilePath | NVARCHAR | 500 | YES | - | PDF檔案路徑 | - |
| FileName | NVARCHAR | 255 | YES | - | 檔案名稱 | - |
| FileSize | BIGINT | - | YES | - | 檔案大小(位元組) | - |
| ConversionStatus | NVARCHAR | 20 | NO | 'PENDING' | 轉換狀態 | PENDING, SUCCESS, FAILED |
| ErrorMessage | NVARCHAR | MAX | YES | - | 錯誤訊息 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| CompletedAt | DATETIME2 | - | YES | - | 完成時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 HTML轉PDF
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pdf/convert`
- **說明**: 將HTML內容轉換為PDF文件
- **請求格式**:
  ```json
  {
    "htmlContent": "<html>...</html>",
    "fileName": "report.pdf",
    "options": {
      "pageSize": "A4",
      "orientation": "Portrait",
      "margin": {
        "top": 10,
        "right": 10,
        "bottom": 10,
        "left": 10
      }
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "轉換成功",
    "data": {
      "logId": "guid",
      "pdfFilePath": "/files/pdf/report.pdf",
      "fileName": "report.pdf",
      "fileSize": 102400
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢轉換記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pdf/logs`
- **說明**: 查詢PDF轉換記錄列表
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "status": "",
    "startDate": "",
    "endDate": ""
  }
  ```
- **回應格式**: 同標準分頁回應格式

#### 3.1.3 下載PDF文件
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pdf/download/{logId}`
- **說明**: 下載轉換後的PDF文件
- **路徑參數**:
  - `logId`: 轉換記錄編號
- **回應格式**: PDF文件流

#### 3.1.4 刪除轉換記錄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/pdf/logs/{logId}`
- **說明**: 刪除轉換記錄及對應的PDF文件
- **路徑參數**:
  - `logId`: 轉換記錄編號

### 3.2 後端實作類別

#### 3.2.1 Controller: `PdfController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/pdf")]
    [Authorize]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;
        
        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }
        
        [HttpPost("convert")]
        public async Task<ActionResult<ApiResponse<PdfConversionResultDto>>> ConvertHtmlToPdf([FromBody] ConvertHtmlToPdfDto dto)
        {
            // 實作轉換邏輯
        }
        
        [HttpGet("logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<PdfConversionLogDto>>>> GetConversionLogs([FromQuery] PdfConversionLogQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("download/{logId}")]
        public async Task<IActionResult> DownloadPdf(Guid logId)
        {
            // 實作下載邏輯
        }
        
        [HttpDelete("logs/{logId}")]
        public async Task<ActionResult<ApiResponse>> DeleteConversionLog(Guid logId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `PdfService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPdfService
    {
        Task<PdfConversionResultDto> ConvertHtmlToPdfAsync(ConvertHtmlToPdfDto dto);
        Task<PagedResult<PdfConversionLogDto>> GetConversionLogsAsync(PdfConversionLogQueryDto query);
        Task<byte[]> GetPdfFileAsync(Guid logId);
        Task DeleteConversionLogAsync(Guid logId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 PDF轉換頁面 (`PdfConversion.vue`)
- **路徑**: `/tools/pdf-conversion`
- **功能**: HTML轉PDF轉換工具
- **主要元件**:
  - HTML編輯器 (HtmlEditor)
  - 轉換選項表單 (ConversionOptionsForm)
  - 轉換結果顯示 (ConversionResult)

#### 4.1.2 PDF轉換記錄頁面 (`PdfConversionLogs.vue`)
- **路徑**: `/tools/pdf-conversion-logs`
- **功能**: 顯示PDF轉換記錄列表

### 4.2 UI 元件設計

#### 4.2.1 HTML編輯器元件 (`HtmlEditor.vue`)
```vue
<template>
  <div>
    <el-input
      v-model="htmlContent"
      type="textarea"
      :rows="20"
      placeholder="請輸入或貼上HTML內容"
    />
  </div>
</template>
```

#### 4.2.2 轉換選項表單 (`ConversionOptionsForm.vue`)
```vue
<template>
  <el-form :model="options" label-width="120px">
    <el-form-item label="檔案名稱">
      <el-input v-model="options.fileName" placeholder="請輸入檔案名稱" />
    </el-form-item>
    <el-form-item label="頁面大小">
      <el-select v-model="options.pageSize">
        <el-option label="A4" value="A4" />
        <el-option label="A3" value="A3" />
        <el-option label="Letter" value="Letter" />
      </el-select>
    </el-form-item>
    <el-form-item label="方向">
      <el-radio-group v-model="options.orientation">
        <el-radio label="Portrait">直向</el-radio>
        <el-radio label="Landscape">橫向</el-radio>
      </el-radio-group>
    </el-form-item>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`pdf.api.ts`)
```typescript
import request from '@/utils/request';

export interface ConvertHtmlToPdfDto {
  htmlContent: string;
  fileName: string;
  options?: {
    pageSize?: string;
    orientation?: string;
    margin?: {
      top?: number;
      right?: number;
      bottom?: number;
      left?: number;
    };
  };
}

export interface PdfConversionResultDto {
  logId: string;
  pdfFilePath: string;
  fileName: string;
  fileSize: number;
}

// API 函數
export const convertHtmlToPdf = (data: ConvertHtmlToPdfDto) => {
  return request.post<ApiResponse<PdfConversionResultDto>>('/api/v1/pdf/convert', data);
};

export const getConversionLogs = (query: any) => {
  return request.get<ApiResponse<PagedResult<any>>>('/api/v1/pdf/logs', { params: query });
};

export const downloadPdf = (logId: string) => {
  return request.get(`/api/v1/pdf/download/${logId}`, {
    responseType: 'blob'
  });
};

export const deleteConversionLog = (logId: string) => {
  return request.delete<ApiResponse>(`/api/v1/pdf/logs/${logId}`);
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
- [ ] Service 實作（整合PDF轉換庫）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] HTML編輯器元件
- [ ] 轉換選項表單元件
- [ ] 轉換結果顯示元件
- [ ] 轉換記錄列表頁面
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 5.5天

---

## 六、注意事項

### 6.1 技術選型
- 推薦使用 iTextSharp 或 DinkToPdf 進行HTML轉PDF
- 考慮使用 Puppeteer 或 Playwright 進行更複雜的HTML渲染
- 需要處理CSS樣式與字體支援

### 6.2 效能
- 大量轉換時考慮使用背景任務處理
- PDF文件需要適當的儲存管理
- 定期清理過期的PDF文件

### 6.3 安全性
- 驗證HTML內容，防止XSS攻擊
- 限制檔案大小
- 限制轉換頻率

### 6.4 業務邏輯
- 轉換失敗時需要記錄錯誤訊息
- 支援轉換進度查詢
- 支援轉換取消功能

---

## 七、測試案例

### 7.1 單元測試
- [ ] HTML轉PDF成功
- [ ] HTML轉PDF失敗（無效HTML）
- [ ] 查詢轉換記錄成功
- [ ] 下載PDF文件成功
- [ ] 刪除轉換記錄成功

### 7.2 整合測試
- [ ] 完整轉換流程測試
- [ ] 錯誤處理測試
- [ ] 檔案下載測試

### 7.3 效能測試
- [ ] 大量HTML轉換測試
- [ ] 大檔案轉換測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/RslHtml2Pdf/` 目錄下的相關文件

### 8.2 技術文件
- iTextSharp 官方文件
- DinkToPdf 官方文件
- Puppeteer 官方文件

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

