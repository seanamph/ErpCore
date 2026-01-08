# SYS2B25 - 其他統計功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS2B25
- **功能名稱**: 其他統計功能 (退貨發票號碼清空作業)
- **功能描述**: 提供退貨發票號碼清空作業功能，支援根據發票年月和發票號碼查詢退貨發票，並可執行清空作業
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB1.asp` (瀏覽版本1)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB2.ASP` (瀏覽版本2)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB3.asp` (瀏覽版本3)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB4.asp` (瀏覽版本4)
  - `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_RESEND.asp` (重送)

### 1.2 業務需求
- 根據發票年月和發票號碼查詢退貨發票
- 顯示退貨發票詳細資料
- 執行退貨發票號碼清空作業
- 支援重送功能
- 清空後需提醒重新執行「發票匯總作業」

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReturnInvoiceClearLog` (退貨發票清空記錄)

```sql
CREATE TABLE [dbo].[ReturnInvoiceClearLog] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 記錄ID
    [InvoiceYear] INT NOT NULL, -- 發票年月 (年份)
    [InvoiceMonth] INT NOT NULL, -- 發票年月 (月份)
    [InvoiceNo] NVARCHAR(10) NOT NULL, -- 發票號碼
    [OriginalInvoiceNo] NVARCHAR(10) NULL, -- 原發票號碼
    [ClearStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 清空狀態 (PENDING, COMPLETED, FAILED)
    [ClearDate] DATETIME2 NULL, -- 清空日期
    [ClearBy] NVARCHAR(50) NULL, -- 清空者
    [ClearNote] NVARCHAR(500) NULL, -- 清空備註
    [ResendStatus] NVARCHAR(20) NULL, -- 重送狀態
    [ResendDate] DATETIME2 NULL, -- 重送日期
    [ResendBy] NVARCHAR(50) NULL, -- 重送者
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ReturnInvoiceClearLog] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReturnInvoiceClearLog_InvoiceYearMonth] ON [dbo].[ReturnInvoiceClearLog] ([InvoiceYear], [InvoiceMonth]);
CREATE NONCLUSTERED INDEX [IX_ReturnInvoiceClearLog_InvoiceNo] ON [dbo].[ReturnInvoiceClearLog] ([InvoiceNo]);
CREATE NONCLUSTERED INDEX [IX_ReturnInvoiceClearLog_ClearStatus] ON [dbo].[ReturnInvoiceClearLog] ([ClearStatus]);
CREATE NONCLUSTERED INDEX [IX_ReturnInvoiceClearLog_ClearDate] ON [dbo].[ReturnInvoiceClearLog] ([ClearDate]);
```

### 2.2 相關資料表

#### 2.2.1 `Invoices` - 發票資料表
- 參考: 需參考發票相關資料表結構

#### 2.2.2 `ReturnInvoices` - 退貨發票資料表
- 參考: 需參考退貨發票相關資料表結構

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LogId | BIGINT | - | NO | IDENTITY(1,1) | 記錄ID | 主鍵 |
| InvoiceYear | INT | - | NO | - | 發票年月 (年份) | - |
| InvoiceMonth | INT | - | NO | - | 發票年月 (月份) | - |
| InvoiceNo | NVARCHAR | 10 | NO | - | 發票號碼 | - |
| OriginalInvoiceNo | NVARCHAR | 10 | YES | - | 原發票號碼 | - |
| ClearStatus | NVARCHAR | 20 | NO | 'PENDING' | 清空狀態 | PENDING, COMPLETED, FAILED |
| ClearDate | DATETIME2 | - | YES | - | 清空日期 | - |
| ClearBy | NVARCHAR | 50 | YES | - | 清空者 | - |
| ClearNote | NVARCHAR | 500 | YES | - | 清空備註 | - |
| ResendStatus | NVARCHAR | 20 | YES | - | 重送狀態 | - |
| ResendDate | DATETIME2 | - | YES | - | 重送日期 | - |
| ResendBy | NVARCHAR | 50 | YES | - | 重送者 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢退貨發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sys2b25/return-invoices`
- **說明**: 根據發票年月和發票號碼查詢退貨發票列表
- **請求參數**:
  - `invoiceYear`: 發票年月 (年份) (必填)
  - `invoiceMonth`: 發票年月 (月份) (必填)
  - `invoiceNo`: 發票號碼 (必填)
  - `pageIndex`: 頁碼 (預設: 1)
  - `pageSize`: 每頁筆數 (預設: 20)
- **回應格式**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "invoiceNo": "AB12345678",
        "originalInvoiceNo": "AB12345677",
        "invoiceDate": "2024-01-15",
        "invoiceAmount": 1000.00,
        "returnReason": "退貨原因",
        "clearStatus": "PENDING"
      }
    ],
    "totalCount": 10,
    "pageIndex": 1,
    "pageSize": 20
  },
  "message": null
}
```

#### 3.1.2 查詢單筆退貨發票
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sys2b25/return-invoices/{invoiceNo}`
- **說明**: 根據發票號碼查詢單筆退貨發票詳細資料
- **路徑參數**:
  - `invoiceNo`: 發票號碼
- **回應格式**: 返回退貨發票詳細資料

#### 3.1.3 執行清空作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sys2b25/return-invoices/clear`
- **說明**: 執行退貨發票號碼清空作業
- **請求格式**:
```json
{
  "invoiceYear": 2024,
  "invoiceMonth": 1,
  "invoiceNo": "AB12345678",
  "clearNote": "清空備註"
}
```
- **回應格式**:
```json
{
  "success": true,
  "data": {
    "logId": 1,
    "clearStatus": "COMPLETED",
    "clearDate": "2024-01-15T10:30:00"
  },
  "message": "清空作業完成，請記得重新執行「發票匯總作業」"
}
```

#### 3.1.4 重送功能
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/sys2b25/return-invoices/resend`
- **說明**: 重送退貨發票
- **請求格式**:
```json
{
  "invoiceNo": "AB12345678",
  "resendType": "EINV",
  "resendNote": "重送備註"
}
```
- **回應格式**: 返回重送結果

#### 3.1.5 查詢清空記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/sys2b25/clear-logs`
- **說明**: 查詢清空記錄列表
- **請求參數**: 分頁參數、篩選參數
- **回應格式**: 返回清空記錄列表

### 3.2 後端實作類別

#### 3.2.1 Controller: `Sys2b25Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/sys2b25")]
    [Authorize]
    public class Sys2b25Controller : ControllerBase
    {
        private readonly ISys2b25Service _service;
        
        public Sys2b25Controller(ISys2b25Service service)
        {
            _service = service;
        }
        
        [HttpGet("return-invoices")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReturnInvoiceDto>>>> GetReturnInvoices([FromQuery] ReturnInvoiceQueryDto query)
        {
            var result = await _service.GetReturnInvoicesAsync(query);
            return Ok(ApiResponse<PagedResult<ReturnInvoiceDto>>.Success(result));
        }
        
        [HttpGet("return-invoices/{invoiceNo}")]
        public async Task<ActionResult<ApiResponse<ReturnInvoiceDto>>> GetReturnInvoice(string invoiceNo)
        {
            var result = await _service.GetReturnInvoiceByNoAsync(invoiceNo);
            return Ok(ApiResponse<ReturnInvoiceDto>.Success(result));
        }
        
        [HttpPost("return-invoices/clear")]
        public async Task<ActionResult<ApiResponse<ClearResultDto>>> ClearInvoice([FromBody] ClearInvoiceDto dto)
        {
            var result = await _service.ClearInvoiceAsync(dto);
            return Ok(ApiResponse<ClearResultDto>.Success(result, "清空作業完成，請記得重新執行「發票匯總作業」"));
        }
        
        [HttpPost("return-invoices/resend")]
        public async Task<ActionResult<ApiResponse<ResendResultDto>>> ResendInvoice([FromBody] ResendInvoiceDto dto)
        {
            var result = await _service.ResendInvoiceAsync(dto);
            return Ok(ApiResponse<ResendResultDto>.Success(result));
        }
        
        [HttpGet("clear-logs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ClearLogDto>>>> GetClearLogs([FromQuery] ClearLogQueryDto query)
        {
            var result = await _service.GetClearLogsAsync(query);
            return Ok(ApiResponse<PagedResult<ClearLogDto>>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `Sys2b25Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISys2b25Service
    {
        Task<PagedResult<ReturnInvoiceDto>> GetReturnInvoicesAsync(ReturnInvoiceQueryDto query);
        Task<ReturnInvoiceDto> GetReturnInvoiceByNoAsync(string invoiceNo);
        Task<ClearResultDto> ClearInvoiceAsync(ClearInvoiceDto dto);
        Task<ResendResultDto> ResendInvoiceAsync(ResendInvoiceDto dto);
        Task<PagedResult<ClearLogDto>> GetClearLogsAsync(ClearLogQueryDto query);
    }
}
```

#### 3.2.3 Repository: `Sys2b25Repository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ISys2b25Repository
    {
        Task<PagedResult<ReturnInvoice>> GetReturnInvoicesAsync(ReturnInvoiceQuery query);
        Task<ReturnInvoice> GetReturnInvoiceByNoAsync(string invoiceNo);
        Task<ReturnInvoiceClearLog> CreateClearLogAsync(ReturnInvoiceClearLog log);
        Task<bool> ClearInvoiceNoAsync(string invoiceNo, int year, int month);
        Task<bool> ResendInvoiceAsync(string invoiceNo, string resendType);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 SYS2B25查詢頁面 (`Sys2b25Query.vue`)
- **路徑**: `/xcom/sys2b25/query`
- **功能**: 顯示查詢表單，支援發票年月和發票號碼查詢
- **主要元件**:
  - 查詢表單 (Sys2b25QueryForm)
  - 退貨發票列表 (Sys2b25InvoiceList)
  - 清空確認對話框

#### 4.1.2 SYS2B25瀏覽頁面 (`Sys2b25Browse.vue`)
- **路徑**: `/xcom/sys2b25/browse`
- **功能**: 顯示退貨發票詳細資料，支援清空作業和重送功能

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`Sys2b25QueryForm.vue`)
```vue
<template>
  <el-form :model="queryForm" :rules="rules" ref="formRef">
    <el-form-item label="原發票年月" prop="invoiceYearMonth">
      <el-date-picker
        v-model="queryForm.invoiceYearMonth"
        type="month"
        placeholder="請選擇發票年月"
        format="YYYY-MM"
        value-format="YYYY-MM"
      />
    </el-form-item>
    <el-form-item label="發票號碼" prop="invoiceNo">
      <el-input 
        v-model="queryForm.invoiceNo" 
        placeholder="請輸入發票號碼 (長度為 10 碼)" 
        maxlength="10"
        clearable
        @blur="handleInvoiceNoBlur"
      />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleQuery">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
    <el-alert
      title="注意事項"
      type="warning"
      :closable="false"
      style="margin-top: 10px"
    >
      <template #default>
        <div>1、此作業修正後記得重新執行「發票匯總作業」!!</div>
      </template>
    </el-alert>
  </el-form>
</template>
```

#### 4.2.2 退貨發票列表元件 (`Sys2b25InvoiceList.vue`)
```vue
<template>
  <div>
    <el-table :data="invoiceList" v-loading="loading">
      <el-table-column prop="invoiceNo" label="發票號碼" width="150" />
      <el-table-column prop="originalInvoiceNo" label="原發票號碼" width="150" />
      <el-table-column prop="invoiceDate" label="發票日期" width="120" />
      <el-table-column prop="invoiceAmount" label="發票金額" width="120" align="right">
        <template #default="{ row }">
          {{ formatCurrency(row.invoiceAmount) }}
        </template>
      </el-table-column>
      <el-table-column prop="returnReason" label="退貨原因" min-width="200" />
      <el-table-column prop="clearStatus" label="清空狀態" width="120">
        <template #default="{ row }">
          <el-tag :type="getStatusType(row.clearStatus)">
            {{ getStatusText(row.clearStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleView(row)">瀏覽</el-button>
          <el-button 
            type="danger" 
            size="small" 
            @click="handleClear(row)"
            :disabled="row.clearStatus === 'COMPLETED'"
          >
            清空
          </el-button>
          <el-button type="warning" size="small" @click="handleResend(row)">重送</el-button>
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

#### 4.2.3 清空確認對話框 (`Sys2b25ClearDialog.vue`)
```vue
<template>
  <el-dialog
    title="確認清空"
    v-model="visible"
    width="500px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="發票號碼">
        <el-input v-model="form.invoiceNo" disabled />
      </el-form-item>
      <el-form-item label="原發票年月">
        <el-input v-model="form.invoiceYearMonth" disabled />
      </el-form-item>
      <el-form-item label="清空備註" prop="clearNote">
        <el-input 
          v-model="form.clearNote" 
          type="textarea" 
          :rows="3" 
          placeholder="請輸入清空備註"
        />
      </el-form-item>
    </el-form>
    <el-alert
      title="注意"
      type="warning"
      :closable="false"
      style="margin-top: 10px"
    >
      <template #default>
        <div>清空後請記得重新執行「發票匯總作業」!!</div>
      </template>
    </el-alert>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="danger" @click="handleSubmit">確認清空</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`sys2b25.api.ts`)
```typescript
import request from '@/utils/request';

export interface ReturnInvoiceDto {
  invoiceNo: string;
  originalInvoiceNo?: string;
  invoiceDate: string;
  invoiceAmount: number;
  returnReason?: string;
  clearStatus: string;
}

export interface ReturnInvoiceQueryDto {
  invoiceYear: number;
  invoiceMonth: number;
  invoiceNo: string;
  pageIndex: number;
  pageSize: number;
}

export interface ClearInvoiceDto {
  invoiceYear: number;
  invoiceMonth: number;
  invoiceNo: string;
  clearNote?: string;
}

export interface ResendInvoiceDto {
  invoiceNo: string;
  resendType: string;
  resendNote?: string;
}

// API 函數
export const getReturnInvoices = (query: ReturnInvoiceQueryDto) => {
  return request.get<ApiResponse<PagedResult<ReturnInvoiceDto>>>('/api/v1/sys2b25/return-invoices', { params: query });
};

export const getReturnInvoiceByNo = (invoiceNo: string) => {
  return request.get<ApiResponse<ReturnInvoiceDto>>(`/api/v1/sys2b25/return-invoices/${invoiceNo}`);
};

export const clearInvoice = (dto: ClearInvoiceDto) => {
  return request.post<ApiResponse<ClearResultDto>>('/api/v1/sys2b25/return-invoices/clear', dto);
};

export const resendInvoice = (dto: ResendInvoiceDto) => {
  return request.post<ApiResponse<ResendResultDto>>('/api/v1/sys2b25/return-invoices/resend', dto);
};

export const getClearLogs = (query: ClearLogQueryDto) => {
  return request.get<ApiResponse<PagedResult<ClearLogDto>>>('/api/v1/sys2b25/clear-logs', { params: query });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立與發票相關資料表的關聯
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 清空作業邏輯實作
- [ ] 重送功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 退貨發票列表開發
- [ ] 清空確認對話框開發
- [ ] 重送功能整合
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 清空作業測試
- [ ] 重送功能測試
- [ ] 錯誤處理測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 清空作業必須記錄操作日誌
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 清空作業必須使用交易處理

### 6.3 資料驗證
- 發票年月必須驗證
- 發票號碼必須驗證格式 (長度為 10 碼)
- 清空作業前必須確認發票狀態

### 6.4 業務邏輯
- 清空作業必須更新相關發票資料表
- 清空後必須提醒重新執行「發票匯總作業」
- 重送功能必須支援多種重送類型

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢退貨發票成功
- [ ] 查詢單筆退貨發票成功
- [ ] 執行清空作業成功
- [ ] 執行清空作業失敗 (發票不存在)
- [ ] 重送功能成功
- [ ] 查詢清空記錄成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 清空作業流程測試
- [ ] 重送功能流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發清空作業測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FQ.ASP` - 查詢畫面
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB.asp` - 瀏覽畫面
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB1.asp` - 瀏覽畫面版本1
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB2.ASP` - 瀏覽畫面版本2
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB3.asp` - 瀏覽畫面版本3
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_FB4.asp` - 瀏覽畫面版本4
- `WEB/IMS_CORE/ASP/XCOM000/SYS2B25_RESEND.asp` - 重送畫面

### 8.2 資料庫 Schema
- 舊系統資料表：需參考舊系統實際資料表結構
- 主要功能：退貨發票號碼清空作業，支援重送功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

