# SYSF210-SYSF220 - 合同處理作業系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSF210-SYSF220 系列
- **功能名稱**: 合同處理作業系列
- **功能描述**: 提供合同處理作業的新增、修改、刪除、查詢功能，包含合同處理編號、處理類型、處理日期、處理金額、處理狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF210_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF220_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF220_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSF000/SYSF220_PR.ASP` (報表)

### 1.2 業務需求
- 管理合同處理作業資料
- 支援合同處理類型管理（付款、收款、變更、終止等）
- 支援合同處理狀態管理（待處理、處理中、已完成、已取消）
- 支援合同處理金額計算
- 支援合同處理報表列印
- 支援合同處理歷史記錄查詢

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ContractProcesses` (合同處理主檔)

```sql
CREATE TABLE [dbo].[ContractProcesses] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ProcessId] NVARCHAR(50) NOT NULL, -- 處理編號 (PROCESS_ID)
    [ContractId] NVARCHAR(50) NOT NULL, -- 合同編號 (CONTRACT_ID)
    [Version] NVARCHAR(10) NOT NULL, -- 版本號 (VERSION)
    [ProcessType] NVARCHAR(20) NOT NULL, -- 處理類型 (PROCESS_TYPE, PAY:付款, REC:收款, CHG:變更, TER:終止)
    [ProcessDate] DATETIME2 NOT NULL, -- 處理日期 (PROCESS_DATE)
    [ProcessAmount] DECIMAL(18, 4) NULL DEFAULT 0, -- 處理金額 (PROCESS_AMT)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'P', -- 狀態 (STATUS, P:待處理, I:處理中, C:已完成, X:已取消)
    [ProcessUserId] NVARCHAR(50) NULL, -- 處理人員 (PROCESS_USER)
    [ProcessMemo] NVARCHAR(500) NULL, -- 處理備註 (PROCESS_MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ContractProcesses] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_ContractProcesses_ProcessId] UNIQUE ([ProcessId]),
    CONSTRAINT [FK_ContractProcesses_Contracts] FOREIGN KEY ([ContractId], [Version]) REFERENCES [dbo].[Contracts] ([ContractId], [Version])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ContractId] ON [dbo].[ContractProcesses] ([ContractId], [Version]);
CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ProcessType] ON [dbo].[ContractProcesses] ([ProcessType]);
CREATE NONCLUSTERED INDEX [IX_ContractProcesses_Status] ON [dbo].[ContractProcesses] ([Status]);
CREATE NONCLUSTERED INDEX [IX_ContractProcesses_ProcessDate] ON [dbo].[ContractProcesses] ([ProcessDate]);
```

### 2.2 資料字典

#### 2.2.1 ContractProcesses 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ProcessId | NVARCHAR | 50 | NO | - | 處理編號 | 唯一，PROCESS_ID |
| ContractId | NVARCHAR | 50 | NO | - | 合同編號 | 外鍵至Contracts |
| Version | NVARCHAR | 10 | NO | - | 版本號 | 外鍵至Contracts |
| ProcessType | NVARCHAR | 20 | NO | - | 處理類型 | PAY:付款, REC:收款, CHG:變更, TER:終止 |
| ProcessDate | DATETIME2 | - | NO | - | 處理日期 | PROCESS_DATE |
| ProcessAmount | DECIMAL | 18,4 | YES | 0 | 處理金額 | PROCESS_AMT |
| Status | NVARCHAR | 10 | NO | 'P' | 狀態 | P:待處理, I:處理中, C:已完成, X:已取消 |
| ProcessUserId | NVARCHAR | 50 | YES | - | 處理人員 | 外鍵至使用者表 |
| ProcessMemo | NVARCHAR | 500 | YES | - | 處理備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢合同處理列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contract-processes`
- **說明**: 查詢合同處理列表，支援分頁、排序、篩選
- **請求參數**: 支援合同編號、處理類型、狀態、處理日期範圍等篩選
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆合同處理
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/contract-processes/{processId}`
- **說明**: 根據處理編號查詢單筆合同處理資料

#### 3.1.3 新增合同處理
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/contract-processes`
- **說明**: 新增合同處理資料

#### 3.1.4 修改合同處理
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contract-processes/{processId}`
- **說明**: 修改合同處理資料（僅待處理或處理中狀態可修改）

#### 3.1.5 刪除合同處理
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/contract-processes/{processId}`
- **說明**: 刪除合同處理資料（僅待處理狀態可刪除）

#### 3.1.6 完成合同處理
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contract-processes/{processId}/complete`
- **說明**: 完成合同處理（狀態改為已完成）

#### 3.1.7 取消合同處理
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/contract-processes/{processId}/cancel`
- **說明**: 取消合同處理（狀態改為已取消）

### 3.2 後端實作類別

#### 3.2.1 Controller: `ContractProcessesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/contract-processes")]
    [Authorize]
    public class ContractProcessesController : ControllerBase
    {
        private readonly IContractProcessService _contractProcessService;
        
        public ContractProcessesController(IContractProcessService contractProcessService)
        {
            _contractProcessService = contractProcessService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ContractProcessDto>>>> GetContractProcesses([FromQuery] ContractProcessQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{processId}")]
        public async Task<ActionResult<ApiResponse<ContractProcessDto>>> GetContractProcess(string processId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ContractProcessCreateResultDto>>> CreateContractProcess([FromBody] CreateContractProcessDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{processId}")]
        public async Task<ActionResult<ApiResponse>> UpdateContractProcess(string processId, [FromBody] UpdateContractProcessDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{processId}")]
        public async Task<ActionResult<ApiResponse>> DeleteContractProcess(string processId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPut("{processId}/complete")]
        public async Task<ActionResult<ApiResponse>> CompleteContractProcess(string processId)
        {
            // 實作完成邏輯
        }
        
        [HttpPut("{processId}/cancel")]
        public async Task<ActionResult<ApiResponse>> CancelContractProcess(string processId)
        {
            // 實作取消邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 合同處理列表頁面 (`ContractProcessList.vue`)
- **路徑**: `/contract-processes`
- **功能**: 顯示合同處理列表，支援查詢、新增、修改、刪除、完成、取消

#### 4.1.2 合同處理詳細頁面 (`ContractProcessDetail.vue`)
- **路徑**: `/contract-processes/:processId`
- **功能**: 顯示合同處理詳細資料，支援修改、完成、取消

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`ContractProcessSearchForm.vue`)
- 支援合同編號、處理類型、狀態、處理日期範圍等查詢條件

#### 4.2.2 資料表格元件 (`ContractProcessDataTable.vue`)
- 顯示處理編號、合同編號、處理類型、處理日期、處理金額、狀態等欄位
- 支援修改、刪除、完成、取消等操作按鈕

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 狀態流程邏輯實作

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 狀態流程操作

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 狀態流程測試
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 12天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 狀態變更必須記錄操作人員和時間

### 6.2 業務邏輯
- 刪除合同處理前必須檢查狀態（僅待處理可刪除）
- 修改合同處理前必須檢查狀態（僅待處理或處理中可修改）
- 完成合同處理前必須檢查狀態（僅待處理或處理中可完成）
- 取消合同處理前必須檢查狀態（僅待處理或處理中可取消）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增合同處理成功
- [ ] 修改合同處理成功
- [ ] 刪除合同處理成功
- [ ] 完成合同處理成功
- [ ] 取消合同處理成功
- [ ] 查詢合同處理列表成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 狀態流程測試
- [ ] 權限檢查測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF210_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF210_PR.ASP` (報表)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF220_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSF000/SYSF220_PR.ASP` (報表)

### 8.2 相關功能
- `開發計劃/28-合同管理/01-合同基礎功能/SYSF110-SYSF140-合同資料維護系列.md`
- `開發計劃/28-合同管理/03-合同擴展功能/SYSF350-SYSF540-合同擴展維護系列.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

