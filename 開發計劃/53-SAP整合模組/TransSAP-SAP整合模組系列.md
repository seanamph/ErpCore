# TransSAP - SAP整合模組系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: TransSAP 系列
- **功能名稱**: SAP整合模組系列
- **功能描述**: 提供與SAP系統的資料傳輸與整合功能，包含資料交換、憑證處理、CSV檔案管理等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/TransSAP/TransSAP.asp` (主要SAP傳輸處理檔案)
  - `WEB/IMS_CORE/ASP/TransSAP/SAP_init.inc` (SAP初始化包含檔)
  - `WEB/IMS_CORE/ASP/TransSAP/SYS2000_TransSAP.asp` (SYS2000模組SAP整合)
  - `WEB/IMS_CORE/ASP/TransSAP/SAP_VOUOCHER/` (SAP憑證處理目錄)
  - `WEB/IMS_CORE/ASP/TransSAP/TXTFILES/` (傳輸檔案目錄)
  - `WEB/IMS_CORE/ASP/TransSAP/SYS2E02/CSV/` (SYS2E02模組CSV檔案目錄)
  - `WEB/IMS_CORE/ASP/TransSAP/SYSB200/CSV/` (SYSB200模組CSV檔案目錄)
  - `WEB/IMS_CORE/ASP/TransSAP/SYSTA30/CSV/` (SYSTA30模組CSV檔案目錄)
  - `WEB/IMS_CORE/ASP/TransSAP/SYSTA40/CSV/` (SYSTA40模組CSV檔案目錄)

### 1.2 業務需求
- 提供與SAP系統的資料傳輸功能
- 支援多種資料格式（CSV、TXT、憑證等）
- 支援多個模組的SAP整合（SYS2000、SYS2E02、SYSB200、SYSTA30、SYSTA40等）
- 支援資料格式轉換和驗證
- 支援傳輸日誌記錄和查詢
- 支援憑證處理和管理
- 支援FTP傳輸功能

### 1.3 子模組清單
- **TransSAP**: SAP資料傳輸主模組
- **SAP_VOUOCHER**: SAP憑證處理模組
- **SYS2000_TransSAP**: SYS2000模組SAP整合
- **SYS2E02_TransSAP**: SYS2E02模組SAP整合
- **SYSB200_TransSAP**: SYSB200模組SAP整合
- **SYSTA30_TransSAP**: SYSTA30模組SAP整合
- **SYSTA40_TransSAP**: SYSTA40模組SAP整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `TransSAPData`

```sql
CREATE TABLE [dbo].[TransSAPData] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransId] NVARCHAR(50) NOT NULL, -- 傳輸代碼
    [ModuleCode] NVARCHAR(20) NOT NULL, -- 模組代碼 (SYS2000, SYS2E02, SYSB200, SYSTA30, SYSTA40等)
    [TransType] NVARCHAR(20) NOT NULL, -- 傳輸類型 (CSV, TXT, VOUCHER等)
    [FileName] NVARCHAR(200) NOT NULL, -- 檔案名稱
    [FilePath] NVARCHAR(500) NULL, -- 檔案路徑
    [FileSize] BIGINT NULL, -- 檔案大小（位元組）
    [RecordCount] INT NULL, -- 記錄筆數
    [TransStatus] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 傳輸狀態 (PENDING:待處理, PROCESSING:處理中, SUCCESS:成功, FAILED:失敗)
    [TransDate] DATETIME2 NULL, -- 傳輸日期
    [SAPResponse] NVARCHAR(MAX) NULL, -- SAP回應內容
    [ErrorMessage] NVARCHAR(MAX) NULL, -- 錯誤訊息
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_TransSAPData_TransId] UNIQUE ([TransId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransSAPData_ModuleCode] ON [dbo].[TransSAPData] ([ModuleCode]);
CREATE NONCLUSTERED INDEX [IX_TransSAPData_TransType] ON [dbo].[TransSAPData] ([TransType]);
CREATE NONCLUSTERED INDEX [IX_TransSAPData_TransStatus] ON [dbo].[TransSAPData] ([TransStatus]);
CREATE NONCLUSTERED INDEX [IX_TransSAPData_TransDate] ON [dbo].[TransSAPData] ([TransDate]);
```

### 2.2 SAP憑證資料表: `SAPVoucher`

```sql
CREATE TABLE [dbo].[SAPVoucher] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [VoucherId] NVARCHAR(50) NOT NULL, -- 憑證代碼
    [VoucherType] NVARCHAR(20) NOT NULL, -- 憑證類型 (SALES, SHARE等)
    [CompanyCode] NVARCHAR(20) NULL, -- 公司代碼
    [VoucherDate] DATETIME2 NULL, -- 憑證日期
    [Amount] DECIMAL(18,2) NULL, -- 金額
    [Currency] NVARCHAR(10) NULL DEFAULT 'TWD', -- 幣別
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- 狀態 (PENDING:待處理, PROCESSED:已處理, FAILED:失敗)
    [SAPDocNo] NVARCHAR(50) NULL, -- SAP文件編號
    [FilePath] NVARCHAR(500) NULL, -- 檔案路徑
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_SAPVoucher_VoucherId] UNIQUE ([VoucherId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SAPVoucher_VoucherType] ON [dbo].[SAPVoucher] ([VoucherType]);
CREATE NONCLUSTERED INDEX [IX_SAPVoucher_CompanyCode] ON [dbo].[SAPVoucher] ([CompanyCode]);
CREATE NONCLUSTERED INDEX [IX_SAPVoucher_Status] ON [dbo].[SAPVoucher] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SAPVoucher_VoucherDate] ON [dbo].[SAPVoucher] ([VoucherDate]);
```

### 2.3 SAP傳輸日誌表: `TransSAPLog`

```sql
CREATE TABLE [dbo].[TransSAPLog] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [TransId] NVARCHAR(50) NOT NULL, -- 傳輸代碼
    [LogLevel] NVARCHAR(20) NOT NULL, -- 日誌級別 (INFO, WARNING, ERROR)
    [LogMessage] NVARCHAR(MAX) NOT NULL, -- 日誌訊息
    [LogData] NVARCHAR(MAX) NULL, -- 日誌資料（JSON格式）
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_TransSAPLog_TransSAPData] FOREIGN KEY ([TransId]) REFERENCES [dbo].[TransSAPData]([TransId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_TransSAPLog_TransId] ON [dbo].[TransSAPLog] ([TransId]);
CREATE NONCLUSTERED INDEX [IX_TransSAPLog_LogLevel] ON [dbo].[TransSAPLog] ([LogLevel]);
CREATE NONCLUSTERED INDEX [IX_TransSAPLog_CreatedAt] ON [dbo].[TransSAPLog] ([CreatedAt]);
```

### 2.4 資料字典

#### TransSAPData 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| TransId | NVARCHAR | 50 | NO | - | 傳輸代碼 | 唯一 |
| ModuleCode | NVARCHAR | 20 | NO | - | 模組代碼 | SYS2000, SYS2E02等 |
| TransType | NVARCHAR | 20 | NO | - | 傳輸類型 | CSV, TXT, VOUCHER |
| FileName | NVARCHAR | 200 | NO | - | 檔案名稱 | - |
| FilePath | NVARCHAR | 500 | YES | - | 檔案路徑 | - |
| FileSize | BIGINT | - | YES | - | 檔案大小 | 位元組 |
| RecordCount | INT | - | YES | - | 記錄筆數 | - |
| TransStatus | NVARCHAR | 20 | NO | 'PENDING' | 傳輸狀態 | PENDING, PROCESSING, SUCCESS, FAILED |
| TransDate | DATETIME2 | - | YES | - | 傳輸日期 | - |
| SAPResponse | NVARCHAR(MAX) | - | YES | - | SAP回應內容 | - |
| ErrorMessage | NVARCHAR(MAX) | - | YES | - | 錯誤訊息 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 查詢傳輸資料列表

```http
GET /api/v1/transsap/data?moduleCode={moduleCode}&transType={transType}&transStatus={transStatus}&startDate={startDate}&endDate={endDate}&page={page}&pageSize={pageSize}
```

**請求參數**:
- `moduleCode`: 模組代碼（選填）
- `transType`: 傳輸類型（選填）
- `transStatus`: 傳輸狀態（選填）
- `startDate`: 開始日期（選填）
- `endDate`: 結束日期（選填）
- `page`: 頁碼（預設1）
- `pageSize`: 每頁筆數（預設20）

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "transId": "TRANS001",
        "moduleCode": "SYS2000",
        "transType": "CSV",
        "fileName": "data.csv",
        "filePath": "/files/transsap/data.csv",
        "fileSize": 1024,
        "recordCount": 100,
        "transStatus": "SUCCESS",
        "transDate": "2024-01-01T00:00:00Z",
        "sapResponse": "成功",
        "errorMessage": null,
        "memo": "備註",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedBy": "USER001",
        "updatedAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

### 3.2 新增傳輸資料

```http
POST /api/v1/transsap/data
```

**請求體**:
```json
{
  "moduleCode": "SYS2000",
  "transType": "CSV",
  "fileName": "data.csv",
  "filePath": "/files/transsap/data.csv",
  "memo": "備註"
}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "新增成功",
  "data": {
    "tKey": 1,
    "transId": "TRANS001",
    "moduleCode": "SYS2000",
    "transType": "CSV",
    "fileName": "data.csv",
    "filePath": "/files/transsap/data.csv",
    "transStatus": "PENDING",
    "createdBy": "USER001",
    "createdAt": "2024-01-01T00:00:00Z"
  }
}
```

### 3.3 執行SAP傳輸

```http
POST /api/v1/transsap/data/{transId}/execute
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "傳輸執行成功",
  "data": {
    "transId": "TRANS001",
    "transStatus": "SUCCESS",
    "sapResponse": "傳輸成功",
    "transDate": "2024-01-01T00:00:00Z"
  }
}
```

### 3.4 上傳檔案

```http
POST /api/v1/transsap/upload
Content-Type: multipart/form-data
```

**請求體**:
- `file`: 檔案（multipart/form-data）
- `moduleCode`: 模組代碼
- `transType`: 傳輸類型

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "上傳成功",
  "data": {
    "transId": "TRANS001",
    "fileName": "data.csv",
    "filePath": "/files/transsap/data.csv",
    "fileSize": 1024
  }
}
```

### 3.5 查詢傳輸日誌

```http
GET /api/v1/transsap/data/{transId}/logs?logLevel={logLevel}&page={page}&pageSize={pageSize}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "transId": "TRANS001",
        "logLevel": "INFO",
        "logMessage": "開始傳輸",
        "logData": null,
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
}
```

### 3.6 查詢憑證列表

```http
GET /api/v1/transsap/vouchers?voucherType={voucherType}&status={status}&startDate={startDate}&endDate={endDate}&page={page}&pageSize={pageSize}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "查詢成功",
  "data": {
    "items": [
      {
        "tKey": 1,
        "voucherId": "VOUCHER001",
        "voucherType": "SALES",
        "companyCode": "COMP001",
        "voucherDate": "2024-01-01T00:00:00Z",
        "amount": 1000.00,
        "currency": "TWD",
        "status": "PROCESSED",
        "sapDocNo": "SAP001",
        "filePath": "/files/vouchers/voucher001.txt",
        "memo": "備註",
        "createdBy": "USER001",
        "createdAt": "2024-01-01T00:00:00Z"
      }
    ],
    "total": 50,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  }
}
```

### 3.7 刪除傳輸資料

```http
DELETE /api/v1/transsap/data/{transId}
```

**回應格式**:
```json
{
  "success": true,
  "code": 200,
  "message": "刪除成功",
  "data": null
}
```

---

## 四、前端 UI 設計

### 4.1 傳輸資料列表頁面

#### 4.1.1 路由配置
- **路由**: `/transsap/data`
- **組件**: `TransSAPDataList.vue`
- **權限**: `TRANS_SAP_DATA_VIEW`

#### 4.1.2 頁面結構
```
┌─────────────────────────────────────────┐
│  SAP整合傳輸資料維護                     │
├─────────────────────────────────────────┤
│  [模組] [類型] [狀態] [日期範圍] [查詢]  │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐  │
│  │ 傳輸代碼 │ 模組 │ 檔案 │ 狀態 │ 操作│  │
│  ├───────────────────────────────────┤  │
│  │ TRANS001│SYS2000│data.csv│成功│ ...│  │
│  │ TRANS002│SYS2E02│data.txt│失敗│ ...│  │
│  └───────────────────────────────────┘  │
│  [上傳檔案] [執行傳輸]      [1][2][3]    │
└─────────────────────────────────────────┘
```

#### 4.1.3 功能說明
- **篩選區**:
  - 模組代碼下拉選單（選填）
  - 傳輸類型下拉選單（CSV, TXT, VOUCHER等）
  - 傳輸狀態下拉選單（全部/待處理/處理中/成功/失敗）
  - 日期範圍選擇器（開始日期、結束日期）
  - 查詢按鈕
- **資料表格**:
  - 顯示欄位：傳輸代碼、模組代碼、傳輸類型、檔案名稱、檔案大小、記錄筆數、傳輸狀態、傳輸日期、錯誤訊息
  - 操作欄位：查看日誌、重新執行、刪除按鈕
  - 支援排序（點擊欄位標題）
  - 支援分頁
- **操作按鈕**:
  - 上傳檔案按鈕：上傳檔案並建立傳輸記錄
  - 執行傳輸按鈕：執行選定的傳輸任務
  - 查看日誌按鈕：查看傳輸日誌
  - 刪除按鈕：確認後刪除

### 4.2 檔案上傳對話框

#### 4.2.1 上傳對話框
- **組件**: `TransSAPUploadDialog.vue`
- **功能**: 上傳檔案並建立傳輸記錄
- **表單欄位**:
  - 模組代碼（必填，下拉選單）
  - 傳輸類型（必填，下拉選單：CSV, TXT, VOUCHER等）
  - 檔案（必填，檔案上傳）
  - 備註（選填，文字區域）

### 4.3 傳輸日誌對話框

#### 4.3.1 日誌對話框
- **組件**: `TransSAPLogDialog.vue`
- **功能**: 顯示傳輸日誌
- **顯示內容**:
  - 日誌列表（時間、級別、訊息）
  - 日誌資料（JSON格式，可展開）
  - 篩選功能（按日誌級別）

### 4.4 憑證管理頁面

#### 4.4.1 憑證列表頁面
- **路由**: `/transsap/vouchers`
- **組件**: `SAPVoucherList.vue`
- **功能**: 管理SAP憑證
- **顯示欄位**: 憑證代碼、憑證類型、公司代碼、憑證日期、金額、幣別、狀態、SAP文件編號

---

## 五、後端實作類別

### 5.1 Controller: `TransSAPController.cs`

```csharp
[ApiController]
[Route("api/v1/transsap")]
[Authorize]
public class TransSAPController : ControllerBase
{
    private readonly ITransSAPService _transSAPService;
    private readonly ILogger<TransSAPController> _logger;

    public TransSAPController(ITransSAPService transSAPService, ILogger<TransSAPController> logger)
    {
        _transSAPService = transSAPService;
        _logger = logger;
    }

    [HttpGet("data")]
    public async Task<ActionResult<ApiResponse<PagedResult<TransSAPDataDto>>>> GetDataList(
        [FromQuery] string? moduleCode,
        [FromQuery] string? transType,
        [FromQuery] string? transStatus,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _transSAPService.GetDataListAsync(moduleCode, transType, transStatus, startDate, endDate, page, pageSize);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("data")]
    public async Task<ActionResult<ApiResponse<TransSAPDataDto>>> CreateData([FromBody] CreateTransSAPDataRequest request)
    {
        var result = await _transSAPService.CreateDataAsync(request);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("data/{transId}/execute")]
    public async Task<ActionResult<ApiResponse<TransSAPExecuteResult>>> ExecuteTransmission(string transId)
    {
        var result = await _transSAPService.ExecuteTransmissionAsync(transId);
        return Ok(ApiResponse.Success(result));
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<TransSAPUploadResult>>> UploadFile(
        IFormFile file,
        [FromForm] string moduleCode,
        [FromForm] string transType)
    {
        var result = await _transSAPService.UploadFileAsync(file, moduleCode, transType);
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("data/{transId}/logs")]
    public async Task<ActionResult<ApiResponse<PagedResult<TransSAPLogDto>>>> GetLogs(
        string transId,
        [FromQuery] string? logLevel,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _transSAPService.GetLogsAsync(transId, logLevel, page, pageSize);
        return Ok(ApiResponse.Success(result));
    }

    [HttpDelete("data/{transId}")]
    public async Task<ActionResult<ApiResponse>> DeleteData(string transId)
    {
        await _transSAPService.DeleteDataAsync(transId);
        return Ok(ApiResponse.Success());
    }
}
```

### 5.2 Service Interface: `ITransSAPService.cs`

```csharp
public interface ITransSAPService
{
    Task<PagedResult<TransSAPDataDto>> GetDataListAsync(string? moduleCode, string? transType, string? transStatus, DateTime? startDate, DateTime? endDate, int page, int pageSize);
    Task<TransSAPDataDto> CreateDataAsync(CreateTransSAPDataRequest request);
    Task<TransSAPExecuteResult> ExecuteTransmissionAsync(string transId);
    Task<TransSAPUploadResult> UploadFileAsync(IFormFile file, string moduleCode, string transType);
    Task<PagedResult<TransSAPLogDto>> GetLogsAsync(string transId, string? logLevel, int page, int pageSize);
    Task DeleteDataAsync(string transId);
    Task<PagedResult<SAPVoucherDto>> GetVouchersAsync(string? voucherType, string? status, DateTime? startDate, DateTime? endDate, int page, int pageSize);
}
```

---

## 六、開發時程

### 階段一：資料庫設計（1週）
- 資料表設計
- 索引設計
- 資料字典建立
- SAP連接配置設計

### 階段二：後端API開發（2週）
- 查詢API開發
- 新增API開發
- 執行傳輸API開發
- 檔案上傳API開發
- 日誌查詢API開發
- SAP整合服務開發

### 階段三：前端UI開發（2週）
- 傳輸資料列表頁面開發
- 檔案上傳對話框開發
- 傳輸日誌對話框開發
- 憑證管理頁面開發
- 表單驗證實作

### 階段四：測試與優化（1週）
- 單元測試
- 整合測試
- SAP連接測試
- 效能優化
- 文件整理

**總計**: 約 6 週

---

## 七、注意事項

### 7.1 SAP連接配置
- SAP連接參數需安全儲存（使用配置檔或加密）
- 連接超時設定需適當
- 錯誤處理需完善

### 7.2 資料驗證
- 檔案格式需驗證
- 檔案大小需限制
- 資料內容需驗證

### 7.3 權限控制
- SAP傳輸需特定權限
- 檔案上傳需特定權限
- 刪除功能需確認對話框

### 7.4 效能考量
- 大量資料傳輸需使用批次處理
- 檔案上傳需使用非同步處理
- 日誌查詢需使用分頁

### 7.5 錯誤處理
- SAP連接錯誤需記錄日誌
- 傳輸失敗需記錄錯誤訊息
- 錯誤訊息需清楚易懂

### 7.6 資料備份
- 傳輸檔案需定期備份
- 傳輸記錄需保留歷史資料
- 刪除操作需記錄日誌

---

## 八、測試案例

### 8.1 檔案上傳測試
- **測試案例1**: 正常上傳CSV檔案
  - 輸入：有效的CSV檔案
  - 預期：成功上傳並建立傳輸記錄
- **測試案例2**: 上傳過大檔案
  - 輸入：超過限制大小的檔案
  - 預期：顯示錯誤訊息「檔案大小超過限制」
- **測試案例3**: 上傳無效格式
  - 輸入：非支援格式檔案
  - 預期：顯示錯誤訊息「不支援的檔案格式」

### 8.2 SAP傳輸測試
- **測試案例1**: 正常執行傳輸
  - 輸入：有效的傳輸記錄
  - 預期：成功執行並更新狀態
- **測試案例2**: SAP連接失敗
  - 輸入：SAP連接失敗的情況
  - 預期：記錄錯誤訊息並更新狀態為失敗
- **測試案例3**: 資料格式錯誤
  - 輸入：資料格式不符合SAP要求
  - 預期：記錄錯誤訊息並更新狀態為失敗

### 8.3 日誌查詢測試
- **測試案例1**: 正常查詢日誌
  - 輸入：有效的傳輸代碼
  - 預期：成功查詢並顯示日誌列表
- **測試案例2**: 按日誌級別篩選
  - 輸入：日誌級別「ERROR」
  - 預期：只顯示錯誤級別的日誌

---

## 九、參考資料

### 9.1 舊程式參考
- `WEB/IMS_CORE/ASP/TransSAP/TransSAP.asp` - 主要SAP傳輸處理檔案
- `WEB/IMS_CORE/ASP/TransSAP/SAP_init.inc` - SAP初始化包含檔
- `WEB/IMS_CORE/ASP/TransSAP/SYS2000_TransSAP.asp` - SYS2000模組SAP整合
- `WEB/IMS_CORE/ASP/TransSAP/SAP_VOUOCHER/` - SAP憑證處理目錄
- `WEB/IMS_CORE/ASP/TransSAP/TXTFILES/` - 傳輸檔案目錄

### 9.2 相關文件
- 系統架構分析.md - TransSAP 目錄分析
- 目錄掃描狀態統計.md - TransSAP 模組狀態

### 9.3 技術文件
- .NET Core 8.0 API 開發指南
- Vue 3 開發指南
- Dapper 使用手冊
- SQL Server 資料庫設計指南
- SAP RFC 連接指南
- FTP 傳輸協議文件

