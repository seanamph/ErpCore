# XCOM280 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM280
- **功能名稱**: 通訊資料維護（系統作業與功能列表）
- **功能描述**: 提供系統作業與功能列表的查詢與報表功能，包含作業代碼、作業名稱、功能按鈕等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM280_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM280_PR.ASP` (報表)

### 1.2 業務需求
- 查詢系統作業列表
- 查詢功能按鈕列表
- 支援作業與功能的對應關係查詢
- 支援報表列印功能
- 支援多條件查詢與排序

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Programs` (系統作業，對應舊系統 `PROG`)

```sql
CREATE TABLE [dbo].[Programs] (
    [ProgId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 作業代碼 (PG_ID)
    [ProgName] NVARCHAR(100) NULL, -- 作業名稱 (PG_NAME)
    [SystemId] NVARCHAR(50) NULL, -- 系統ID (SYS_ID)
    [SubSystemId] NVARCHAR(50) NULL, -- 子系統ID (SUB_SYS_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS) A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    CONSTRAINT [PK_Programs] PRIMARY KEY CLUSTERED ([ProgId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Programs_SystemId] ON [dbo].[Programs] ([SystemId]);
CREATE NONCLUSTERED INDEX [IX_Programs_SubSystemId] ON [dbo].[Programs] ([SubSystemId]);
CREATE NONCLUSTERED INDEX [IX_Programs_Status] ON [dbo].[Programs] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `Buttons` - 功能按鈕主檔
```sql
CREATE TABLE [dbo].[Buttons] (
    [ButtonId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 按鈕代碼 (BUTTON_ID)
    [ButtonName] NVARCHAR(100) NULL, -- 按鈕名稱 (BUTTON_NAME)
    [ProgId] NVARCHAR(50) NULL, -- 作業代碼 (PG_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Buttons_Programs] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Programs] ([ProgId])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ProgId | NVARCHAR | 50 | NO | - | 作業代碼 | 主鍵 |
| ProgName | NVARCHAR | 100 | YES | - | 作業名稱 | - |
| SystemId | NVARCHAR | 50 | YES | - | 系統ID | 外鍵至Systems表 |
| SubSystemId | NVARCHAR | 50 | YES | - | 子系統ID | 外鍵至SubSystems表 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統作業列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom280/programs`
- **說明**: 查詢系統作業列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ProgId",
    "sortOrder": "ASC",
    "filters": {
      "progId": "",
      "progName": "",
      "systemId": "",
      "subSystemId": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢功能按鈕列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom280/buttons`
- **說明**: 查詢功能按鈕列表，支援分頁、排序、篩選
- **請求參數**: 標準查詢參數
- **回應格式**: 標準分頁回應格式

#### 3.1.3 查詢作業與功能對應
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom280/programs/{progId}/buttons`
- **說明**: 查詢指定作業的功能按鈕列表
- **回應格式**: 標準列表回應格式

#### 3.1.4 產生報表
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom280/report`
- **說明**: 產生系統作業與功能列表報表
- **請求格式**:
  ```json
  {
    "reportType": "PROGRAM_LIST", // PROGRAM_LIST, BUTTON_LIST, PROGRAM_BUTTON_LIST
    "filters": {
      "systemId": "",
      "subSystemId": "",
      "status": ""
    }
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom280Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom280")]
    [Authorize]
    public class XCom280Controller : ControllerBase
    {
        private readonly IProgramService _programService;
        
        public XCom280Controller(IProgramService programService)
        {
            _programService = programService;
        }
        
        [HttpGet("programs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProgramDto>>>> GetPrograms([FromQuery] ProgramQueryDto query)
        {
            var result = await _programService.GetProgramsAsync(query);
            return Ok(ApiResponse<PagedResult<ProgramDto>>.Success(result));
        }
        
        [HttpGet("buttons")]
        public async Task<ActionResult<ApiResponse<PagedResult<ButtonDto>>>> GetButtons([FromQuery] ButtonQueryDto query)
        {
            var result = await _programService.GetButtonsAsync(query);
            return Ok(ApiResponse<PagedResult<ButtonDto>>.Success(result));
        }
        
        [HttpGet("programs/{progId}/buttons")]
        public async Task<ActionResult<ApiResponse<List<ButtonDto>>>> GetProgramButtons(string progId)
        {
            var result = await _programService.GetProgramButtonsAsync(progId);
            return Ok(ApiResponse<List<ButtonDto>>.Success(result));
        }
        
        [HttpPost("report")]
        public async Task<ActionResult> GenerateReport([FromBody] ReportRequestDto request)
        {
            var result = await _programService.GenerateReportAsync(request);
            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統作業列表查詢頁面 (`ProgramList.vue`)
- **路徑**: `/xcom/programs`
- **功能**: 顯示系統作業列表，支援查詢、排序、報表列印
- **主要元件**:
  - 查詢表單 (ProgramSearchForm)
  - 資料表格 (ProgramDataTable)
  - 報表列印功能

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ProgramSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="作業代碼">
      <el-input v-model="searchForm.progId" placeholder="請輸入作業代碼" />
    </el-form-item>
    <el-form-item label="作業名稱">
      <el-input v-model="searchForm.progName" placeholder="請輸入作業名稱" />
    </el-form-item>
    <el-form-item label="系統">
      <el-select v-model="searchForm.systemId" placeholder="請選擇系統">
        <el-option v-for="sys in systemList" :key="sys.systemId" :label="sys.systemName" :value="sys.systemId" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handlePrint">列印報表</el-button>
    </el-form-item>
  </el-form>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構
- [ ] 建立索引

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 報表產生功能

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 報表列印功能

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 功能測試
- [ ] 報表測試

**總計**: 5.5天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 報表產生必須記錄操作日誌

### 6.2 效能
- 大量資料查詢必須使用分頁
- 報表產生可以非同步處理

### 6.3 業務邏輯
- 作業與功能的對應關係必須正確
- 報表格式必須符合需求

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢系統作業列表成功
- [ ] 查詢功能按鈕列表成功
- [ ] 查詢作業與功能對應成功
- [ ] 報表產生成功

### 7.2 整合測試
- [ ] API 端點測試
- [ ] 前端頁面顯示測試
- [ ] 報表列印功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM280_FQ.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM280_PR.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

