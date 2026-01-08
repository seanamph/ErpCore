# XCOM360 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM360
- **功能名稱**: 通訊資料維護（上下傳記錄報表）
- **功能描述**: 提供檔案上下傳記錄報表的查詢與列印功能，包含上傳記錄、下載記錄的統計分析，支援多種查詢條件和報表格式
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM360_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM360_PR.ASP` (報表)

### 1.2 業務需求
- 查詢檔案上傳記錄統計
- 查詢檔案下載記錄統計
- 支援依上傳使用者代碼查詢
- 支援依下載使用者代碼查詢
- 支援依上傳/下載時間範圍查詢
- 支援依檔案大小範圍查詢
- 支援依附檔名查詢
- 支援報表列印與匯出（Excel、PDF）
- 支援統計資訊顯示（總上傳數、總下載數、總檔案大小等）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

#### 2.1.1 `FileUploads` - 檔案上傳記錄
- 參考: `開發計劃/12-通訊模組/XCOM320-通訊資料維護.md` 的 `FileUploads` 資料表結構

#### 2.1.2 `FileDownloads` - 檔案下傳記錄
- 參考: `開發計劃/12-通訊模組/XCOM330-通訊資料維護.md` 的 `FileDownloads` 資料表結構

### 2.2 統計視圖: `V_FileUploadDownloadStats` (檔案上下傳統計視圖)

```sql
CREATE VIEW [dbo].[V_FileUploadDownloadStats] AS
SELECT 
    u.UploadId,
    u.UserId AS UploadUserId,
    u.UserName AS UploadUserName,
    u.ProgId,
    u.ProgName,
    u.FileName,
    u.FileSize,
    u.FileExt,
    u.UploadTime,
    COUNT(DISTINCT d.DownloadId) AS DownloadCount,
    COUNT(DISTINCT d.UserId) AS DownloadUserCount,
    MAX(d.DownloadTime) AS LastDownloadTime,
    u.Status
FROM [dbo].[FileUploads] u
LEFT JOIN [dbo].[FileDownloads] d ON u.UploadId = d.UploadId
WHERE u.Status = '1'
GROUP BY 
    u.UploadId,
    u.UserId,
    u.UserName,
    u.ProgId,
    u.ProgName,
    u.FileName,
    u.FileSize,
    u.FileExt,
    u.UploadTime,
    u.Status;
```

### 2.3 資料字典

#### V_FileUploadDownloadStats 視圖欄位

| 欄位名稱 | 資料類型 | 說明 | 備註 |
|---------|---------|------|------|
| UploadId | NVARCHAR | 上傳記錄ID | - |
| UploadUserId | NVARCHAR | 上傳使用者代碼 | - |
| UploadUserName | NVARCHAR | 上傳使用者名稱 | - |
| ProgId | NVARCHAR | 系統代碼 | - |
| ProgName | NVARCHAR | 系統名稱 | - |
| FileName | NVARCHAR | 檔案名稱 | - |
| FileSize | BIGINT | 檔案大小 | 單位：位元組 |
| FileExt | NVARCHAR | 附檔名 | - |
| UploadTime | DATETIME2 | 上傳時間 | - |
| DownloadCount | INT | 下載次數 | 統計值 |
| DownloadUserCount | INT | 下載使用者數 | 統計值 |
| LastDownloadTime | DATETIME2 | 最後下載時間 | - |
| Status | NVARCHAR | 狀態 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢上下傳記錄報表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom360/file-upload-download-report`
- **說明**: 查詢檔案上下傳記錄報表，支援多種查詢條件和統計
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "UploadTime",
    "sortOrder": "DESC",
    "filters": {
      "uploadUserId": "",
      "downloadUserId": "",
      "progId": "",
      "fileExt": "",
      "uploadTimeFrom": "",
      "uploadTimeTo": "",
      "downloadTimeFrom": "",
      "downloadTimeTo": "",
      "fileSizeFrom": null,
      "fileSizeTo": null,
      "status": ""
    },
    "reportType": "upload" // upload, download, both
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
          "uploadId": "UP001",
          "uploadUserId": "U001",
          "uploadUserName": "使用者名稱",
          "progId": "SYS0000",
          "progName": "系統管理",
          "fileName": "test.pdf",
          "fileSize": 1024000,
          "fileSizeFormatted": "1.00 MB",
          "fileExt": "pdf",
          "uploadTime": "2024-01-01T10:00:00",
          "downloadCount": 10,
          "downloadUserCount": 5,
          "lastDownloadTime": "2024-01-15T14:00:00",
          "status": "1"
        }
      ],
      "summary": {
        "totalUploadCount": 100,
        "totalDownloadCount": 500,
        "totalFileSize": 1024000000,
        "totalFileSizeFormatted": "1.00 GB",
        "totalUploadUserCount": 20,
        "totalDownloadUserCount": 50
      },
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出上下傳記錄報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom360/file-upload-download-report/export`
- **說明**: 匯出檔案上下傳記錄報表（Excel、PDF）
- **請求格式**: 同查詢參數，加上 `exportFormat` (excel, pdf)
- **回應格式**: 檔案下載

#### 3.1.3 列印上下傳記錄報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom360/file-upload-download-report/print`
- **說明**: 列印檔案上下傳記錄報表
- **請求格式**: 同查詢參數
- **回應格式**: PDF 檔案

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom360FileUploadDownloadReportController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom360/file-upload-download-report")]
    [Authorize]
    public class XCom360FileUploadDownloadReportController : ControllerBase
    {
        private readonly IXCom360FileUploadDownloadReportService _reportService;
        
        public XCom360FileUploadDownloadReportController(IXCom360FileUploadDownloadReportService reportService)
        {
            _reportService = reportService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<FileUploadDownloadReportDto>>>> GetReport([FromQuery] FileUploadDownloadReportQueryDto query)
        {
            var result = await _reportService.GetReportAsync(query);
            return Ok(ApiResponse<PagedResult<FileUploadDownloadReportDto>>.Success(result));
        }
        
        [HttpPost("export")]
        public async Task<ActionResult> ExportReport([FromBody] FileUploadDownloadReportExportDto dto)
        {
            var fileBytes = await _reportService.ExportReportAsync(dto);
            var fileName = $"檔案上下傳記錄報表_{DateTime.Now:yyyyMMddHHmmss}.{dto.ExportFormat.ToLower()}";
            return File(fileBytes, GetContentType(dto.ExportFormat), fileName);
        }
        
        [HttpPost("print")]
        public async Task<ActionResult> PrintReport([FromBody] FileUploadDownloadReportQueryDto query)
        {
            var pdfBytes = await _reportService.PrintReportAsync(query);
            var fileName = $"檔案上下傳記錄報表_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        
        private string GetContentType(string format)
        {
            return format.ToLower() switch
            {
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
```

#### 3.2.2 Service: `XCom360FileUploadDownloadReportService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCom360FileUploadDownloadReportService
    {
        Task<PagedResult<FileUploadDownloadReportDto>> GetReportAsync(FileUploadDownloadReportQueryDto query);
        Task<byte[]> ExportReportAsync(FileUploadDownloadReportExportDto dto);
        Task<byte[]> PrintReportAsync(FileUploadDownloadReportQueryDto query);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 上下傳記錄報表查詢頁面 (`FileUploadDownloadReportQuery.vue`)
- **路徑**: `/xcom/file-upload-download-report`
- **功能**: 顯示檔案上下傳記錄報表查詢和結果
- **主要元件**:
  - 查詢表單 (FileUploadDownloadReportSearchForm)
  - 統計資訊顯示 (FileUploadDownloadReportSummary)
  - 資料表格 (FileUploadDownloadReportDataTable)
  - 匯出/列印按鈕

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`FileUploadDownloadReportSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="報表類型">
      <el-select v-model="searchForm.reportType" placeholder="請選擇報表類型">
        <el-option label="上傳記錄" value="upload" />
        <el-option label="下載記錄" value="download" />
        <el-option label="全部" value="both" />
      </el-select>
    </el-form-item>
    <el-form-item label="上傳使用者代碼">
      <el-input v-model="searchForm.uploadUserId" placeholder="請輸入使用者代碼" clearable />
    </el-form-item>
    <el-form-item label="下載使用者代碼">
      <el-input v-model="searchForm.downloadUserId" placeholder="請輸入使用者代碼" clearable />
    </el-form-item>
    <el-form-item label="系統代碼">
      <el-select v-model="searchForm.progId" placeholder="請選擇系統" clearable filterable>
        <el-option v-for="prog in progList" :key="prog.progId" :label="prog.progName" :value="prog.progId" />
      </el-select>
    </el-form-item>
    <el-form-item label="附檔名">
      <el-input v-model="searchForm.fileExt" placeholder="請輸入附檔名" clearable />
    </el-form-item>
    <el-form-item label="上傳時間">
      <el-date-picker
        v-model="uploadDateRange"
        type="datetimerange"
        range-separator="至"
        start-placeholder="開始時間"
        end-placeholder="結束時間"
        format="YYYY-MM-DD HH:mm:ss"
        value-format="YYYY-MM-DD HH:mm:ss"
      />
    </el-form-item>
    <el-form-item label="下載時間">
      <el-date-picker
        v-model="downloadDateRange"
        type="datetimerange"
        range-separator="至"
        start-placeholder="開始時間"
        end-placeholder="結束時間"
        format="YYYY-MM-DD HH:mm:ss"
        value-format="YYYY-MM-DD HH:mm:ss"
      />
    </el-form-item>
    <el-form-item label="檔案大小">
      <el-input-number v-model="searchForm.fileSizeFrom" :min="0" placeholder="最小" style="width: 120px" />
      <span style="margin: 0 10px">至</span>
      <el-input-number v-model="searchForm.fileSizeTo" :min="0" placeholder="最大" style="width: 120px" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 統計資訊元件 (`FileUploadDownloadReportSummary.vue`)
```vue
<template>
  <el-card v-if="summary" style="margin-bottom: 20px">
    <template #header>
      <span>統計資訊</span>
    </template>
    <el-row :gutter="20">
      <el-col :span="6">
        <el-statistic title="總上傳數" :value="summary.totalUploadCount" />
      </el-col>
      <el-col :span="6">
        <el-statistic title="總下載數" :value="summary.totalDownloadCount" />
      </el-col>
      <el-col :span="6">
        <el-statistic title="總檔案大小" :value="summary.totalFileSizeFormatted" />
      </el-col>
      <el-col :span="6">
        <el-statistic title="上傳使用者數" :value="summary.totalUploadUserCount" />
      </el-col>
    </el-row>
    <el-row :gutter="20" style="margin-top: 20px">
      <el-col :span="6">
        <el-statistic title="下載使用者數" :value="summary.totalDownloadUserCount" />
      </el-col>
    </el-row>
  </el-card>
</template>
```

#### 4.2.3 資料表格元件 (`FileUploadDownloadReportDataTable.vue`)
```vue
<template>
  <div>
    <div style="margin-bottom: 10px">
      <el-button type="success" icon="Download" @click="handleExport">匯出</el-button>
      <el-button type="warning" icon="Printer" @click="handlePrint">列印</el-button>
    </div>
    <el-table :data="reportList" v-loading="loading" border stripe>
      <el-table-column type="index" label="序號" width="60" align="center" />
      <el-table-column prop="uploadId" label="上傳記錄ID" width="150" />
      <el-table-column prop="uploadUserName" label="上傳使用者" width="120" />
      <el-table-column prop="progName" label="系統" width="150" />
      <el-table-column prop="fileName" label="檔案名稱" min-width="200" />
      <el-table-column prop="fileSizeFormatted" label="檔案大小" width="100" align="right" />
      <el-table-column prop="fileExt" label="附檔名" width="80" />
      <el-table-column prop="uploadTime" label="上傳時間" width="160" />
      <el-table-column prop="downloadCount" label="下載次數" width="100" align="right" />
      <el-table-column prop="downloadUserCount" label="下載使用者數" width="120" align="right" />
      <el-table-column prop="lastDownloadTime" label="最後下載時間" width="160" />
      <el-table-column prop="status" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '正常' : '已刪除' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

### 4.3 API 呼叫 (`xcom360.api.ts`)
```typescript
import request from '@/utils/request';

export interface FileUploadDownloadReportDto {
  uploadId: string;
  uploadUserId: string;
  uploadUserName?: string;
  progId?: string;
  progName?: string;
  fileName: string;
  fileSize: number;
  fileSizeFormatted?: string;
  fileExt?: string;
  uploadTime: string;
  downloadCount: number;
  downloadUserCount: number;
  lastDownloadTime?: string;
  status: string;
}

export interface FileUploadDownloadReportSummaryDto {
  totalUploadCount: number;
  totalDownloadCount: number;
  totalFileSize: number;
  totalFileSizeFormatted: string;
  totalUploadUserCount: number;
  totalDownloadUserCount: number;
}

export interface FileUploadDownloadReportQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    uploadUserId?: string;
    downloadUserId?: string;
    progId?: string;
    fileExt?: string;
    uploadTimeFrom?: string;
    uploadTimeTo?: string;
    downloadTimeFrom?: string;
    downloadTimeTo?: string;
    fileSizeFrom?: number;
    fileSizeTo?: number;
    status?: string;
  };
  reportType?: 'upload' | 'download' | 'both';
}

export interface FileUploadDownloadReportExportDto extends FileUploadDownloadReportQueryDto {
  exportFormat: 'excel' | 'pdf';
}

// API 函數
export const getFileUploadDownloadReport = (query: FileUploadDownloadReportQueryDto) => {
  return request.get<ApiResponse<PagedResult<FileUploadDownloadReportDto>> & { summary: FileUploadDownloadReportSummaryDto }>('/api/v1/xcom360/file-upload-download-report', { params: query });
};

export const exportFileUploadDownloadReport = (data: FileUploadDownloadReportExportDto) => {
  return request.post('/api/v1/xcom360/file-upload-download-report/export', data, {
    responseType: 'blob'
  });
};

export const printFileUploadDownloadReport = (data: FileUploadDownloadReportQueryDto) => {
  return request.post('/api/v1/xcom360/file-upload-download-report/print', data, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立統計視圖
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表查詢邏輯實作
- [ ] 統計計算邏輯實作
- [ ] Excel 匯出功能實作
- [ ] PDF 列印功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 統計資訊顯示
- [ ] 資料表格開發
- [ ] 匯出功能開發
- [ ] 列印功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 報表匯出列印測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 10天

---

## 六、注意事項

### 6.1 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 統計視圖必須優化
- 報表匯出必須使用非同步處理

### 6.2 資料驗證
- 查詢條件必須驗證
- 日期範圍必須驗證

### 6.3 業務邏輯
- 統計計算必須準確
- 報表格式必須符合需求
- 支援多種報表類型（上傳、下載、全部）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢上下傳記錄報表成功
- [ ] 統計計算正確
- [ ] 匯出報表成功（Excel、PDF）
- [ ] 列印報表成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 報表匯出列印測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 報表匯出效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM360_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/XCOM360_PR.ASP` - 報表畫面

### 8.2 相關功能
- XCOM320 - 檔案上傳作業
- XCOM330 - 檔案下傳作業

### 8.3 資料庫 Schema
- 舊系統資料表：`FILE_UPLOAD`, `FILE_DOWNLOAD`
- 主要欄位：USER_ID, UPD_TIME, FILE_SIZE, FILE_EXT

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

