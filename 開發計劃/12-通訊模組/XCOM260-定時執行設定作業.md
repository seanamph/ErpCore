# XCOM260 - 定時執行設定作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM260
- **功能名稱**: 定時執行設定作業
- **功能描述**: 提供定時任務(CRON Job)的新增、修改、刪除、查詢功能，包含任務代碼、主系統代碼、下次執行時間、執行區間、執行區間類型、執行任務、任務描述等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FS.ASP` (排序)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM260_PR.ASP` (報表)

### 1.2 業務需求
- 管理系統定時任務設定
- 支援任務的新增、修改、刪除、查詢
- 支援執行區間設定（分鐘、小時、天、週、月）
- 支援下次執行時間設定
- 記錄任務的建立與變更資訊
- 與排程系統整合（Hangfire/Quartz.NET）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `CronJobs` (定時任務，對應舊系統 `CRONJOB_M`)

```sql
CREATE TABLE [dbo].[CronJobs] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [JobId] NVARCHAR(50) NOT NULL, -- JOB代碼
    [SysId] NVARCHAR(50) NULL, -- 主系統代碼
    [NextDate] DATETIME2 NULL, -- 下次被執行時間
    [Interval] INT NULL, -- 執行區間
    [IntervalType] NVARCHAR(20) NULL, -- 執行區間類型 (MINUTE, HOUR, DAY, WEEK, MONTH)
    [What] NVARCHAR(500) NULL, -- 執行任務
    [Notes] NVARCHAR(500) NULL, -- 任務描述
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [BUser] NVARCHAR(50) NULL, -- 建立者
    [BTime] DATETIME2 NULL, -- 建立時間
    [CUser] NVARCHAR(50) NULL, -- 變更者
    [CTime] DATETIME2 NULL, -- 變更時間
    [CPriority] INT NULL, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者組別
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_CronJobs_JobId] UNIQUE ([JobId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_CronJobs_JobId] ON [dbo].[CronJobs] ([JobId]);
CREATE NONCLUSTERED INDEX [IX_CronJobs_SysId] ON [dbo].[CronJobs] ([SysId]);
CREATE NONCLUSTERED INDEX [IX_CronJobs_NextDate] ON [dbo].[CronJobs] ([NextDate]);
CREATE NONCLUSTERED INDEX [IX_CronJobs_Status] ON [dbo].[CronJobs] ([Status]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| JobId | NVARCHAR | 50 | NO | - | JOB代碼 | 必填，唯一 |
| SysId | NVARCHAR | 50 | YES | - | 主系統代碼 | - |
| NextDate | DATETIME2 | - | YES | - | 下次被執行時間 | - |
| Interval | INT | - | YES | - | 執行區間 | - |
| IntervalType | NVARCHAR | 20 | YES | - | 執行區間類型 | MINUTE, HOUR, DAY, WEEK, MONTH |
| What | NVARCHAR | 500 | YES | - | 執行任務 | - |
| Notes | NVARCHAR | 500 | YES | - | 任務描述 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| BUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| BTime | DATETIME2 | - | YES | - | 建立時間 | - |
| CUser | NVARCHAR | 50 | YES | - | 變更者 | - |
| CTime | DATETIME2 | - | YES | - | 變更時間 | - |
| CPriority | INT | - | YES | - | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者組別 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢定時任務列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom260/cronjobs`
- **說明**: 查詢定時任務列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆定時任務
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom260/cronjobs/{tKey}`
- **說明**: 根據主鍵查詢單筆定時任務資料
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增定時任務
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom260/cronjobs`
- **說明**: 新增定時任務資料
- **請求格式**: 標準新增格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改定時任務
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom260/cronjobs/{tKey}`
- **說明**: 修改定時任務資料
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除定時任務
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom260/cronjobs/{tKey}`
- **說明**: 刪除定時任務資料
- **回應格式**: 標準刪除回應格式

#### 3.1.6 啟用/停用定時任務
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom260/cronjobs/{tKey}/status`
- **說明**: 啟用或停用定時任務
- **請求格式**:
  ```json
  {
    "status": "A" // A:啟用, I:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom260Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom260")]
    [Authorize]
    public class XCom260Controller : ControllerBase
    {
        private readonly IXCom260Service _service;
        
        public XCom260Controller(IXCom260Service service)
        {
            _service = service;
        }
        
        [HttpGet("cronjobs")]
        public async Task<ActionResult<ApiResponse<PagedResult<CronJobDto>>>> GetCronJobs([FromQuery] CronJobQueryDto query)
        {
            var result = await _service.GetCronJobsAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("cronjobs/{tKey}")]
        public async Task<ActionResult<ApiResponse<CronJobDto>>> GetCronJob(long tKey)
        {
            var result = await _service.GetCronJobByIdAsync(tKey);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("cronjobs")]
        public async Task<ActionResult<ApiResponse<long>>> CreateCronJob([FromBody] CreateCronJobDto dto)
        {
            var result = await _service.CreateCronJobAsync(dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPut("cronjobs/{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateCronJob(long tKey, [FromBody] UpdateCronJobDto dto)
        {
            await _service.UpdateCronJobAsync(tKey, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("cronjobs/{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteCronJob(long tKey)
        {
            await _service.DeleteCronJobAsync(tKey);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("cronjobs/{tKey}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateCronJobStatus(long tKey, [FromBody] UpdateStatusDto dto)
        {
            await _service.UpdateCronJobStatusAsync(tKey, dto.Status);
            return Ok(ApiResponse.Success());
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 定時任務列表頁面 (`CronJobList.vue`)
- **路徑**: `/xcom/cronjobs`
- **功能**: 顯示定時任務列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (CronJobSearchForm)
  - 資料表格 (CronJobDataTable)
  - 新增/修改對話框 (CronJobDialog)

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`CronJobSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="主系統代碼">
      <el-input v-model="searchForm.sysId" placeholder="請輸入主系統代碼" />
    </el-form-item>
    <el-form-item label="執行區間類型">
      <el-select v-model="searchForm.intervalType" placeholder="請選擇執行區間類型">
        <el-option label="全部" value="" />
        <el-option label="分鐘" value="MINUTE" />
        <el-option label="小時" value="HOUR" />
        <el-option label="天" value="DAY" />
        <el-option label="週" value="WEEK" />
        <el-option label="月" value="MONTH" />
      </el-select>
    </el-form-item>
    <el-form-item label="執行任務">
      <el-input v-model="searchForm.what" placeholder="請輸入執行任務" />
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態">
        <el-option label="全部" value="" />
        <el-option label="啟用" value="A" />
        <el-option label="停用" value="I" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 與排程系統整合（Hangfire/Quartz.NET）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 排程系統整合測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 8天

---

## 六、注意事項

### 6.1 安全性
- 定時任務執行必須有管理員權限
- 必須驗證任務代碼的唯一性
- 必須記錄所有操作日誌

### 6.2 效能
- 定時任務查詢必須使用分頁
- 必須建立適當的索引
- 必須優化排程系統效能

### 6.3 資料驗證
- 任務代碼必須唯一
- 執行區間類型必須在允許範圍內
- 下次執行時間必須在未來

### 6.4 業務邏輯
- 必須與排程系統（Hangfire/Quartz.NET）整合
- 停用任務時必須從排程系統中移除
- 啟用任務時必須加入排程系統
- 必須支援CRON表達式解析

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增定時任務成功
- [ ] 新增定時任務失敗 (重複代碼)
- [ ] 修改定時任務成功
- [ ] 刪除定時任務成功
- [ ] 查詢定時任務列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 排程系統整合測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FB.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FI.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FU.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FD.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM260_FQ.ASP`

### 8.2 排程系統文件
- Hangfire Documentation
- Quartz.NET Documentation

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

