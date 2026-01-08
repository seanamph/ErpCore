# SYSE210-SYSE230 - 租賃擴展維護系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSE210-SYSE230 系列
- **功能名稱**: 租賃擴展維護系列
- **功能描述**: 提供租賃擴展資料的新增、修改、刪除、查詢功能，包含租賃擴展資訊、特殊條件、附加條款、擴展設定等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE220_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE220_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE220_PR.ASP` (報表)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE230_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSE000/SYSE230_FU.ASP` (修改)

### 1.2 業務需求
- 管理租賃擴展基本資料
- 支援租賃擴展資訊的新增、修改、刪除、查詢
- 支援特殊條件設定
- 支援附加條款管理
- 支援擴展設定維護
- 支援租賃擴展報表列印
- 支援租賃擴展歷史記錄查詢
- 支援多店別管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LeaseExtensions` (租賃擴展主檔)

```sql
CREATE TABLE [dbo].[LeaseExtensions] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號 (EXTENSION_ID)
    [LeaseId] NVARCHAR(50) NOT NULL, -- 租賃編號 (LEASE_ID)
    [ExtensionType] NVARCHAR(20) NOT NULL, -- 擴展類型 (EXTENSION_TYPE, CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定)
    [ExtensionName] NVARCHAR(200) NULL, -- 擴展名稱 (EXTENSION_NAME)
    [ExtensionValue] NVARCHAR(MAX) NULL, -- 擴展值 (EXTENSION_VALUE)
    [StartDate] DATETIME2 NULL, -- 開始日期 (START_DATE)
    [EndDate] DATETIME2 NULL, -- 結束日期 (END_DATE)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- 狀態 (STATUS, A:啟用, I:停用)
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼 (ORG_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_LeaseExtensions] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_LeaseExtensions_ExtensionId] UNIQUE ([ExtensionId]),
    CONSTRAINT [FK_LeaseExtensions_Leases] FOREIGN KEY ([LeaseId]) REFERENCES [dbo].[Leases] ([LeaseId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionId] ON [dbo].[LeaseExtensions] ([ExtensionId]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_LeaseId] ON [dbo].[LeaseExtensions] ([LeaseId]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_ExtensionType] ON [dbo].[LeaseExtensions] ([ExtensionType]);
CREATE NONCLUSTERED INDEX [IX_LeaseExtensions_Status] ON [dbo].[LeaseExtensions] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `LeaseExtensionDetails` - 租賃擴展明細
```sql
CREATE TABLE [dbo].[LeaseExtensionDetails] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [ExtensionId] NVARCHAR(50) NOT NULL, -- 擴展編號
    [LineNum] INT NOT NULL, -- 行號 (LINE_NUM)
    [FieldName] NVARCHAR(100) NULL, -- 欄位名稱 (FIELD_NAME)
    [FieldValue] NVARCHAR(MAX) NULL, -- 欄位值 (FIELD_VALUE)
    [FieldType] NVARCHAR(20) NULL, -- 欄位類型 (FIELD_TYPE, TEXT:文字, NUMBER:數字, DATE:日期, BOOLEAN:布林)
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LeaseExtensionDetails_LeaseExtensions] FOREIGN KEY ([ExtensionId]) REFERENCES [dbo].[LeaseExtensions] ([ExtensionId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LeaseExtensionDetails_ExtensionId] ON [dbo].[LeaseExtensionDetails] ([ExtensionId]);
```

### 2.3 資料字典

#### 2.3.1 LeaseExtensions 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| ExtensionId | NVARCHAR | 50 | NO | - | 擴展編號 | 唯一，EXTENSION_ID |
| LeaseId | NVARCHAR | 50 | NO | - | 租賃編號 | 外鍵至租賃表 |
| ExtensionType | NVARCHAR | 20 | NO | - | 擴展類型 | CONDITION:特殊條件, TERM:附加條款, SETTING:擴展設定 |
| ExtensionName | NVARCHAR | 200 | YES | - | 擴展名稱 | - |
| ExtensionValue | NVARCHAR | MAX | YES | - | 擴展值 | - |
| StartDate | DATETIME2 | - | YES | - | 開始日期 | - |
| EndDate | DATETIME2 | - | YES | - | 結束日期 | - |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢租賃擴展列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-extensions`
- **說明**: 查詢租賃擴展列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ExtensionId",
    "sortOrder": "ASC",
    "filters": {
      "extensionId": "",
      "leaseId": "",
      "extensionType": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆租賃擴展
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 根據擴展編號查詢單筆租賃擴展資料（含明細）
- **路徑參數**:
  - `extensionId`: 擴展編號
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增租賃擴展
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lease-extensions`
- **說明**: 新增租賃擴展資料（含明細）
- **請求格式**: 標準新增格式
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改租賃擴展
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 修改租賃擴展資料（僅啟用狀態可修改）
- **路徑參數**:
  - `extensionId`: 擴展編號
- **請求格式**: 同新增，但 `extensionId` 不可修改
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除租賃擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-extensions/{extensionId}`
- **說明**: 刪除租賃擴展資料（僅啟用狀態可刪除）
- **路徑參數**:
  - `extensionId`: 擴展編號
- **回應格式**: 標準刪除回應格式

#### 3.1.6 批次刪除租賃擴展
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lease-extensions/batch`
- **說明**: 批次刪除多筆租賃擴展（僅啟用狀態可刪除）
- **請求格式**: 標準批次刪除格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `LeaseExtensionsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/lease-extensions")]
    [Authorize]
    public class LeaseExtensionsController : ControllerBase
    {
        private readonly ILeaseExtensionService _leaseExtensionService;
        
        public LeaseExtensionsController(ILeaseExtensionService leaseExtensionService)
        {
            _leaseExtensionService = leaseExtensionService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<LeaseExtensionDto>>>> GetLeaseExtensions([FromQuery] LeaseExtensionQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{extensionId}")]
        public async Task<ActionResult<ApiResponse<LeaseExtensionDto>>> GetLeaseExtension(string extensionId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateLeaseExtension([FromBody] CreateLeaseExtensionDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{extensionId}")]
        public async Task<ActionResult<ApiResponse>> UpdateLeaseExtension(string extensionId, [FromBody] UpdateLeaseExtensionDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{extensionId}")]
        public async Task<ActionResult<ApiResponse>> DeleteLeaseExtension(string extensionId)
        {
            // 實作刪除邏輯
        }
    }
}
```

#### 3.2.2 Service: `LeaseExtensionService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ILeaseExtensionService
    {
        Task<PagedResult<LeaseExtensionDto>> GetLeaseExtensionsAsync(LeaseExtensionQueryDto query);
        Task<LeaseExtensionDto> GetLeaseExtensionByIdAsync(string extensionId);
        Task<string> CreateLeaseExtensionAsync(CreateLeaseExtensionDto dto);
        Task UpdateLeaseExtensionAsync(string extensionId, UpdateLeaseExtensionDto dto);
        Task DeleteLeaseExtensionAsync(string extensionId);
    }
}
```

#### 3.2.3 Repository: `LeaseExtensionRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface ILeaseExtensionRepository
    {
        Task<LeaseExtension> GetByIdAsync(string extensionId);
        Task<PagedResult<LeaseExtension>> GetPagedAsync(LeaseExtensionQuery query);
        Task<LeaseExtension> CreateAsync(LeaseExtension leaseExtension);
        Task<LeaseExtension> UpdateAsync(LeaseExtension leaseExtension);
        Task DeleteAsync(string extensionId);
        Task<bool> ExistsAsync(string extensionId);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 租賃擴展列表頁面 (`LeaseExtensionList.vue`)
- **路徑**: `/lease/lease-extensions`
- **功能**: 顯示租賃擴展列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (LeaseExtensionSearchForm)
  - 資料表格 (LeaseExtensionDataTable)
  - 新增/修改對話框 (LeaseExtensionDialog)
  - 刪除確認對話框

#### 4.1.2 租賃擴展詳細頁面 (`LeaseExtensionDetail.vue`)
- **路徑**: `/lease/lease-extensions/:extensionId`
- **功能**: 顯示租賃擴展詳細資料（含明細），支援修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`LeaseExtensionSearchForm.vue`)
- 擴展編號查詢
- 租賃編號查詢
- 擴展類型查詢
- 狀態查詢
- 日期範圍查詢

#### 4.2.2 資料表格元件 (`LeaseExtensionDataTable.vue`)
- 顯示擴展編號、租賃編號、擴展類型、擴展名稱、狀態等欄位
- 支援操作按鈕（修改、刪除）

#### 4.2.3 新增/修改對話框 (`LeaseExtensionDialog.vue`)
- 基本資料表單
- 明細表格
- 表單驗證

### 4.3 API 呼叫 (`leaseExtension.api.ts`)
- `getLeaseExtensionList`: 查詢列表
- `getLeaseExtensionById`: 查詢單筆
- `createLeaseExtension`: 新增
- `updateLeaseExtension`: 修改
- `deleteLeaseExtension`: 刪除

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)
- 狀態變更必須記錄操作人員和時間

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 擴展編號必須唯一
- 必填欄位必須驗證
- 日期範圍必須驗證
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除租賃擴展前必須檢查狀態（僅啟用可刪除）
- 修改租賃擴展前必須檢查狀態（僅啟用可修改）
- 租賃編號必須存在於租賃表中

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增租賃擴展成功
- [ ] 新增租賃擴展失敗 (重複編號)
- [ ] 修改租賃擴展成功
- [ ] 修改租賃擴展失敗 (非啟用狀態)
- [ ] 刪除租賃擴展成功
- [ ] 刪除租賃擴展失敗 (非啟用狀態)
- [ ] 查詢租賃擴展列表成功
- [ ] 查詢單筆租賃擴展成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE210_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE220_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE220_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE220_PR.ASP` (報表)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE230_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/SYSE000/SYSE230_FU.ASP` (修改)

### 8.2 相關功能
- `開發計劃/30-租賃管理SYSE/01-租賃基礎功能/SYSE110-SYSE140-租賃資料維護系列.md`
- `開發計劃/30-租賃管理SYSE/03-費用管理/SYSE310-SYSE430-費用資料維護系列.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

