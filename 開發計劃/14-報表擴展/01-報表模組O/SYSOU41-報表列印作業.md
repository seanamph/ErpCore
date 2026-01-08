# SYSOU41 - 報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSOU41
- **功能名稱**: 報表列印作業
- **功能描述**: 提供報表模組O的報表列印功能，支援報表查詢、列印、匯出等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSO000/SYSOU41_PR_P.rdlc` (報表定義檔)
  - `WEB/IMS_CORE/ASP/SYSO000/SYSOU41_PR.asp` (報表列印頁面)

### 1.2 業務需求
- 提供報表查詢功能
- 支援報表列印功能
- 支援報表匯出功能（PDF、Excel）
- 支援報表參數設定
- 與報表模組O資料整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表
根據報表模組O的業務需求，參考相關資料表設計

### 2.2 相關資料表
- 根據實際報表需求定義

### 2.3 資料字典
- 根據實際報表需求定義

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢報表資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/reports/sysou41/data`
- **說明**: 查詢報表資料，支援參數篩選
- **請求參數**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "param1": "",
    "param2": ""
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [...],
      "summary": {...}
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 產生報表PDF
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysou41/pdf`
- **說明**: 產生報表PDF檔案
- **請求格式**: 同查詢報表資料
- **回應格式**: PDF檔案（binary）

#### 3.1.3 產生報表Excel
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/reports/sysou41/excel`
- **說明**: 產生報表Excel檔案
- **請求格式**: 同查詢報表資料
- **回應格式**: Excel檔案（binary）

### 3.2 後端實作類別

#### 3.2.1 Controller: `SYSOU41ReportController.cs`
```csharp
[ApiController]
[Route("api/v1/reports/sysou41")]
public class SYSOU41ReportController : ControllerBase
{
    private readonly ISYSOU41ReportService _reportService;

    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<SYSOU41ReportDataDto>>> GetReportData([FromQuery] SYSOU41ReportQueryDto query)
    {
        // 實作查詢報表資料邏輯
    }

    [HttpPost("pdf")]
    public async Task<IActionResult> GeneratePdf([FromBody] SYSOU41ReportQueryDto query)
    {
        // 實作產生PDF邏輯
    }

    [HttpPost("excel")]
    public async Task<IActionResult> GenerateExcel([FromBody] SYSOU41ReportQueryDto query)
    {
        // 實作產生Excel邏輯
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 報表查詢頁面 (`SYSOU41Report.vue`)
```vue
<template>
  <div class="sysou41-report">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>SYSOU41 報表列印</span>
        </div>
      </template>
      
      <!-- 查詢表單 -->
      <el-form :model="queryForm" :inline="true" class="query-form">
        <el-form-item label="開始日期">
          <el-date-picker
            v-model="queryForm.startDate"
            type="date"
            placeholder="請選擇開始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item label="結束日期">
          <el-date-picker
            v-model="queryForm.endDate"
            type="date"
            placeholder="請選擇結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">查詢</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handlePrint">列印</el-button>
          <el-button type="warning" @click="handleExportPdf">匯出PDF</el-button>
          <el-button type="info" @click="handleExportExcel">匯出Excel</el-button>
        </el-form-item>
      </el-form>

      <!-- 報表資料 -->
      <el-table
        :data="reportData"
        border
        style="margin-top: 16px;"
      >
        <!-- 根據實際報表欄位定義 -->
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { getSYSOU41ReportData, generateSYSOU41Pdf, generateSYSOU41Excel } from '@/api/reports/sysou41.api';

// 查詢表單
const queryForm = ref({
  startDate: '',
  endDate: '',
  param1: '',
  param2: ''
});

// 報表資料
const reportData = ref([]);

// 查詢
const handleQuery = async () => {
  try {
    const response = await getSYSOU41ReportData(queryForm.value);
    if (response.success) {
      reportData.value = response.data.items;
    }
  } catch (error) {
    console.error('查詢報表資料失敗:', error);
  }
};

// 重置
const handleReset = () => {
  queryForm.value = {
    startDate: '',
    endDate: '',
    param1: '',
    param2: ''
  };
};

// 列印
const handlePrint = () => {
  // 開啟新視窗列印報表
  window.open(`/api/v1/reports/sysou41/pdf?${new URLSearchParams(queryForm.value)}`, '_blank');
};

// 匯出PDF
const handleExportPdf = async () => {
  try {
    const response = await generateSYSOU41Pdf(queryForm.value);
    // 下載PDF檔案
    const blob = new Blob([response.data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `SYSOU41_${new Date().getTime()}.pdf`;
    link.click();
  } catch (error) {
    console.error('匯出PDF失敗:', error);
  }
};

// 匯出Excel
const handleExportExcel = async () => {
  try {
    const response = await generateSYSOU41Excel(queryForm.value);
    // 下載Excel檔案
    const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `SYSOU41_${new Date().getTime()}.xlsx`;
    link.click();
  } catch (error) {
    console.error('匯出Excel失敗:', error);
  }
};
</script>
```

### 4.2 API 呼叫 (`sysou41.api.ts`)
```typescript
import request from '@/utils/request';
import type { ApiResponse } from '@/types/api';

export const getSYSOU41ReportData = (params: any) => {
  return request.get<ApiResponse<any>>('/api/v1/reports/sysou41/data', { params });
};

export const generateSYSOU41Pdf = (params: any) => {
  return request.post('/api/v1/reports/sysou41/pdf', params, {
    responseType: 'blob'
  });
};

export const generateSYSOU41Excel = (params: any) => {
  return request.post('/api/v1/reports/sysou41/excel', params, {
    responseType: 'blob'
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 確認報表相關資料表結構
- [ ] 確認索引設計

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 報表產生邏輯（PDF、Excel）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 報表查詢頁面開發
- [ ] 報表列印功能
- [ ] 報表匯出功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 報表列印測試
- [ ] 報表匯出測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 8天

---

## 六、注意事項

### 6.1 業務邏輯
- 報表參數需進行驗證
- 報表資料需支援大量資料查詢
- PDF和Excel產生需考慮效能

### 6.2 資料驗證
- 日期範圍需驗證
- 參數需進行格式驗證

### 6.3 效能
- 大量資料報表需使用分頁或批次處理
- PDF和Excel產生需使用非同步處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢報表資料成功
- [ ] 產生PDF成功
- [ ] 產生Excel成功

### 7.2 整合測試
- [ ] 完整報表查詢流程測試
- [ ] 報表列印流程測試
- [ ] 報表匯出流程測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSO000/SYSOU41_PR_P.rdlc`
- `WEB/IMS_CORE/ASP/SYSO000/SYSOU41_PR.asp`

### 8.2 相關功能
- `SYSOU42-報表列印作業.md` - 類似報表功能
- `SYSOU43-報表列印作業.md` - 類似報表功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

