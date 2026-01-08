# MakeRegFile/RegSys - 系統註冊功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MakeRegFile / RegSys
- **功能名稱**: 系統註冊功能
- **功能描述**: 提供系統註冊檔案生成與驗證功能，用於系統授權管理，包含CPU編號、電腦名稱、MAC地址等硬體資訊的驗證
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/MakeRegFile.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/MakeRegFile.aspx.cs`
  - `WEB/IMS_CORE/Kernel/MakeRegFile.aspx`
  - `WEB/IMS_CORE/Kernel/RegSys_*.dll` (註冊檔案)

### 1.2 業務需求
- 生成系統註冊檔案
- 驗證系統註冊檔案
- 讀取硬體資訊 (CPU編號、電腦名稱、MAC地址)
- 加密註冊資料
- 驗證註冊檔案的有效性
- 支援註冊檔案到期日管理
- 支援網頁註冊功能
- 記錄註冊操作日誌

### 1.3 使用場景
- 系統授權管理
- 硬體綁定驗證
- 系統使用期限控制
- 多系統註冊管理

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemRegistrations` (系統註冊記錄)

```sql
CREATE TABLE [dbo].[SystemRegistrations] (
    [RegistrationId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CompanyId] NVARCHAR(50) NOT NULL, -- 公司代碼
    [CpuNumber] NVARCHAR(100) NOT NULL, -- CPU編號
    [ComputerName] NVARCHAR(100) NOT NULL, -- 電腦名稱
    [MacAddress] NVARCHAR(100) NOT NULL, -- MAC地址
    [RegistrationKey] NVARCHAR(500) NOT NULL, -- 註冊金鑰
    [ExpiryDate] DATETIME2 NOT NULL, -- 到期日
    [LastDate] NVARCHAR(50) NULL, -- 最後日期 (加密)
    [UseDownGo] NVARCHAR(10) NULL, -- 使用下載標記
    [Ticket] NVARCHAR(10) NULL, -- 票證
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', -- ACTIVE, EXPIRED, REVOKED
    [RegistrationFile] VARBINARY(MAX) NULL, -- 註冊檔案內容
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_SystemRegistrations_CompanyId_CpuNumber] UNIQUE ([CompanyId], [CpuNumber])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_CompanyId] ON [dbo].[SystemRegistrations] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_CpuNumber] ON [dbo].[SystemRegistrations] ([CpuNumber]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_Status] ON [dbo].[SystemRegistrations] ([Status]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrations_ExpiryDate] ON [dbo].[SystemRegistrations] ([ExpiryDate]);
```

### 2.2 相關資料表

#### 2.2.1 `SystemRegistrationLogs` - 系統註冊操作日誌
```sql
CREATE TABLE [dbo].[SystemRegistrationLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [RegistrationId] BIGINT NULL,
    [CompanyId] NVARCHAR(50) NOT NULL,
    [OperationType] NVARCHAR(20) NOT NULL, -- CREATE, VERIFY, UPDATE, REVOKE
    [CpuNumber] NVARCHAR(100) NULL,
    [ComputerName] NVARCHAR(100) NULL,
    [MacAddress] NVARCHAR(100) NULL,
    [Result] NVARCHAR(20) NOT NULL, -- SUCCESS, FAILED
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SystemRegistrationLogs_SystemRegistrations] FOREIGN KEY ([RegistrationId]) REFERENCES [dbo].[SystemRegistrations] ([RegistrationId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_RegistrationId] ON [dbo].[SystemRegistrationLogs] ([RegistrationId]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_CompanyId] ON [dbo].[SystemRegistrationLogs] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_OperationType] ON [dbo].[SystemRegistrationLogs] ([OperationType]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationLogs_CreatedAt] ON [dbo].[SystemRegistrationLogs] ([CreatedAt]);
```

#### 2.2.2 `SystemRegistrationRequests` - 系統註冊申請
```sql
CREATE TABLE [dbo].[SystemRegistrationRequests] (
    [RequestId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CompanyId] NVARCHAR(50) NOT NULL,
    [UserId] NVARCHAR(50) NOT NULL,
    [CpuNumber] NVARCHAR(100) NOT NULL,
    [ComputerName] NVARCHAR(100) NOT NULL,
    [MacAddress] NVARCHAR(100) NOT NULL,
    [ExpiryDate] DATETIME2 NOT NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'PENDING', -- PENDING, APPROVED, REJECTED
    [ApprovedBy] NVARCHAR(50) NULL,
    [ApprovedAt] DATETIME2 NULL,
    [RejectReason] NVARCHAR(500) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SystemRegistrationRequests_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_CompanyId] ON [dbo].[SystemRegistrationRequests] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_UserId] ON [dbo].[SystemRegistrationRequests] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_SystemRegistrationRequests_Status] ON [dbo].[SystemRegistrationRequests] ([Status]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| RegistrationId | BIGINT | - | NO | IDENTITY(1,1) | 註冊記錄ID | 主鍵 |
| CompanyId | NVARCHAR | 50 | NO | - | 公司代碼 | - |
| CpuNumber | NVARCHAR | 100 | NO | - | CPU編號 | - |
| ComputerName | NVARCHAR | 100 | NO | - | 電腦名稱 | - |
| MacAddress | NVARCHAR | 100 | NO | - | MAC地址 | - |
| RegistrationKey | NVARCHAR | 500 | NO | - | 註冊金鑰 | 加密儲存 |
| ExpiryDate | DATETIME2 | - | NO | - | 到期日 | - |
| LastDate | NVARCHAR | 50 | YES | - | 最後日期 | 加密 |
| UseDownGo | NVARCHAR | 10 | YES | - | 使用下載標記 | - |
| Ticket | NVARCHAR | 10 | YES | - | 票證 | - |
| Status | NVARCHAR | 20 | NO | 'ACTIVE' | 狀態 | ACTIVE, EXPIRED, REVOKED |
| RegistrationFile | VARBINARY(MAX) | - | YES | - | 註冊檔案內容 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得硬體資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/system-registration/hardware-info`
- **說明**: 取得當前系統的硬體資訊 (CPU編號、電腦名稱、MAC地址)
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "cpuNumber": "CPU123456",
      "computerName": "SERVER01",
      "macAddress": "00-11-22-33-44-55"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 生成註冊檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/system-registration/generate`
- **說明**: 生成系統註冊檔案
- **請求格式**:
  ```json
  {
    "companyId": "COMP001",
    "cpuNumber": "CPU123456",
    "computerName": "SERVER01",
    "macAddress": "00-11-22-33-44-55",
    "expiryDate": "2025-12-31",
    "useDownGo": "Y",
    "ticket": "12345"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "生成成功",
    "data": {
      "registrationId": 1,
      "registrationKey": "0F1234ABCDEF...",
      "downloadUrl": "/api/v1/kernel/system-registration/files/1/download"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 驗證註冊檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/system-registration/verify`
- **說明**: 驗證系統註冊檔案
- **請求格式**:
  ```json
  {
    "registrationKey": "0F1234ABCDEF...",
    "cpuNumber": "CPU123456",
    "computerName": "SERVER01",
    "macAddress": "00-11-22-33-44-55"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "驗證成功",
    "data": {
      "isValid": true,
      "expiryDate": "2025-12-31",
      "daysRemaining": 365,
      "status": "ACTIVE"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 上傳註冊檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/system-registration/upload`
- **說明**: 上傳註冊檔案進行驗證
- **請求格式**: `multipart/form-data`
  - `file`: 註冊檔案 (reg.dll 或 RegSys_*.dll)
- **回應格式**: 同驗證註冊檔案

#### 3.1.5 下載註冊檔案
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/system-registration/files/{registrationId}/download`
- **說明**: 下載註冊檔案
- **回應格式**: 檔案下載

#### 3.1.6 查詢註冊記錄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/system-registrations`
- **說明**: 查詢系統註冊記錄
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "companyId": "",
    "cpuNumber": "",
    "status": "",
    "startDate": "",
    "endDate": ""
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
          "registrationId": 1,
          "companyId": "COMP001",
          "cpuNumber": "CPU123456",
          "computerName": "SERVER01",
          "macAddress": "00-11-22-33-44-55",
          "expiryDate": "2025-12-31",
          "status": "ACTIVE",
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

#### 3.1.7 撤銷註冊
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/system-registrations/{registrationId}/revoke`
- **說明**: 撤銷系統註冊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "撤銷成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 網頁註冊
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/system-registration/web-register`
- **說明**: 透過網頁進行註冊
- **請求格式**:
  ```json
  {
    "tKey": "T_KEY_VALUE",
    "userId": "U001",
    "eDate": "2025-12-31",
    "macAddress": "00-11-22-33-44-55",
    "cpuNumber": "CPU123456",
    "computerName": "SERVER01",
    "rslCoding": "Y"
  }
  ```
- **回應格式**: 返回註冊資料字串 (格式: "OK:LAST_DATE^_^KEYS")

### 3.2 後端實作類別

#### 3.2.1 Controller: `SystemRegistrationController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/system-registration")]
    [Authorize]
    public class SystemRegistrationController : ControllerBase
    {
        private readonly ISystemRegistrationService _systemRegistrationService;
        
        public SystemRegistrationController(ISystemRegistrationService systemRegistrationService)
        {
            _systemRegistrationService = systemRegistrationService;
        }
        
        [HttpGet("hardware-info")]
        public async Task<ActionResult<ApiResponse<HardwareInfoDto>>> GetHardwareInfo()
        {
            // 實作取得硬體資訊邏輯
        }
        
        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<RegistrationResultDto>>> GenerateRegistration([FromBody] GenerateRegistrationDto dto)
        {
            // 實作生成註冊檔案邏輯
        }
        
        [HttpPost("verify")]
        public async Task<ActionResult<ApiResponse<VerificationResultDto>>> VerifyRegistration([FromBody] VerifyRegistrationDto dto)
        {
            // 實作驗證註冊檔案邏輯
        }
        
        [HttpPost("upload")]
        public async Task<ActionResult<ApiResponse<VerificationResultDto>>> UploadRegistrationFile(IFormFile file)
        {
            // 實作上傳註冊檔案邏輯
        }
        
        [HttpGet("files/{registrationId}/download")]
        public async Task<ActionResult> DownloadRegistrationFile(long registrationId)
        {
            // 實作下載註冊檔案邏輯
        }
        
        [HttpPost("web-register")]
        public async Task<ActionResult<string>> WebRegister([FromBody] WebRegisterDto dto)
        {
            // 實作網頁註冊邏輯
        }
    }
}
```

#### 3.2.2 Service: `SystemRegistrationService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ISystemRegistrationService
    {
        Task<HardwareInfoDto> GetHardwareInfoAsync();
        Task<RegistrationResultDto> GenerateRegistrationAsync(GenerateRegistrationDto dto);
        Task<VerificationResultDto> VerifyRegistrationAsync(VerifyRegistrationDto dto);
        Task<VerificationResultDto> VerifyRegistrationFileAsync(byte[] fileContent);
        Task<byte[]> GetRegistrationFileAsync(long registrationId);
        Task<PagedResult<SystemRegistrationDto>> GetSystemRegistrationsAsync(SystemRegistrationQueryDto query);
        Task RevokeRegistrationAsync(long registrationId);
        Task<string> WebRegisterAsync(WebRegisterDto dto);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統註冊管理頁面 (`SystemRegistration.vue`)
- **路徑**: `/system/kernel/system-registration`
- **功能**: 系統註冊檔案生成、驗證、管理
- **主要元件**:
  - 硬體資訊顯示
  - 註冊檔案生成表單
  - 註冊檔案上傳驗證
  - 註冊記錄查詢表格

#### 4.1.2 系統註冊記錄查詢頁面 (`SystemRegistrationLog.vue`)
- **路徑**: `/system/kernel/system-registration-logs`
- **功能**: 顯示系統註冊操作日誌
- **主要元件**:
  - 查詢表單
  - 資料表格

### 4.2 UI 元件設計

#### 4.2.1 系統註冊管理表單 (`SystemRegistrationForm.vue`)
```vue
<template>
  <el-card>
    <template #header>
      <span>系統註冊管理</span>
    </template>
    
    <!-- 硬體資訊顯示 -->
    <el-descriptions title="硬體資訊" :column="2" border>
      <el-descriptions-item label="CPU編號">{{ hardwareInfo.cpuNumber }}</el-descriptions-item>
      <el-descriptions-item label="電腦名稱">{{ hardwareInfo.computerName }}</el-descriptions-item>
      <el-descriptions-item label="MAC地址">{{ hardwareInfo.macAddress }}</el-descriptions-item>
    </el-descriptions>
    
    <el-divider />
    
    <!-- 註冊檔案生成 -->
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="公司代碼" prop="companyId">
        <el-input v-model="form.companyId" placeholder="請輸入公司代碼" />
      </el-form-item>
      <el-form-item label="到期日" prop="expiryDate">
        <el-date-picker v-model="form.expiryDate" type="date" placeholder="請選擇到期日" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleGenerate">生成註冊檔案</el-button>
        <el-button @click="handleUpload">上傳註冊檔案</el-button>
        <el-button @click="handleVerify">驗證註冊檔案</el-button>
      </el-form-item>
    </el-form>
    
    <!-- 註冊檔案下載 -->
    <el-divider />
    <el-button type="success" @click="handleDownload" v-if="registrationId">下載註冊檔案</el-button>
  </el-card>
</template>
```

### 4.3 API 呼叫 (`systemRegistration.api.ts`)
```typescript
import request from '@/utils/request';

export interface HardwareInfoDto {
  cpuNumber: string;
  computerName: string;
  macAddress: string;
}

export interface GenerateRegistrationDto {
  companyId: string;
  cpuNumber: string;
  computerName: string;
  macAddress: string;
  expiryDate: string;
  useDownGo?: string;
  ticket?: string;
}

export interface VerifyRegistrationDto {
  registrationKey: string;
  cpuNumber: string;
  computerName: string;
  macAddress: string;
}

export interface SystemRegistrationDto {
  registrationId: number;
  companyId: string;
  cpuNumber: string;
  computerName: string;
  macAddress: string;
  expiryDate: string;
  status: string;
  createdAt: string;
}

// API 函數
export const getHardwareInfo = () => {
  return request.get<ApiResponse<HardwareInfoDto>>('/api/v1/kernel/system-registration/hardware-info');
};

export const generateRegistration = (data: GenerateRegistrationDto) => {
  return request.post<ApiResponse<RegistrationResultDto>>('/api/v1/kernel/system-registration/generate', data);
};

export const verifyRegistration = (data: VerifyRegistrationDto) => {
  return request.post<ApiResponse<VerificationResultDto>>('/api/v1/kernel/system-registration/verify', data);
};

export const uploadRegistrationFile = (file: File) => {
  const formData = new FormData();
  formData.append('file', file);
  return request.post<ApiResponse<VerificationResultDto>>('/api/v1/kernel/system-registration/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
};

export const downloadRegistrationFile = (registrationId: number) => {
  return request.get(`/api/v1/kernel/system-registration/files/${registrationId}/download`, {
    responseType: 'blob'
  });
};
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
- [ ] 加密/解密邏輯實作
- [ ] 硬體資訊讀取邏輯
- [ ] 註冊檔案生成邏輯
- [ ] 註冊檔案驗證邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 系統註冊管理頁面開發
- [ ] 註冊記錄查詢頁面開發
- [ ] 硬體資訊顯示元件
- [ ] 註冊檔案上傳元件
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 註冊檔案生成測試
- [ ] 註冊檔案驗證測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 8天

---

## 六、注意事項

### 6.1 安全性
- 註冊金鑰必須加密儲存
- 必須使用安全的加密演算法
- 必須防止註冊檔案被篡改
- 必須驗證硬體資訊的真實性
- 必須記錄所有註冊操作

### 6.2 效能
- 註冊檔案生成必須快速
- 註冊檔案驗證必須高效
- 必須使用快取機制

### 6.3 資料驗證
- 硬體資訊必須驗證
- 到期日必須驗證
- 註冊金鑰格式必須驗證

### 6.4 業務邏輯
- 註冊檔案必須包含所有必要資訊
- 必須正確計算到期日
- 必須正確處理註冊檔案格式
- 必須支援多種註冊檔案格式

### 6.5 相容性
- 必須相容舊版註冊檔案格式
- 必須支援多種硬體資訊格式
- 必須處理各種邊界情況

---

## 七、測試案例

### 7.1 單元測試
- [ ] 硬體資訊讀取成功
- [ ] 註冊檔案生成成功
- [ ] 註冊檔案驗證成功
- [ ] 註冊檔案驗證失敗 (過期)
- [ ] 註冊檔案驗證失敗 (硬體不符)
- [ ] 加密/解密功能測試

### 7.2 整合測試
- [ ] 完整註冊流程測試
- [ ] 網頁註冊功能測試
- [ ] 註冊檔案上傳測試
- [ ] 註冊檔案下載測試
- [ ] 錯誤處理測試

### 7.3 安全性測試
- [ ] 註冊檔案篡改測試
- [ ] 硬體資訊偽造測試
- [ ] 加密強度測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/MakeRegFile.aspx.cs`
- `WEB/IMS_CORE/Kernel/MakeRegFile.aspx`
- `IMS3/HANSHIN/RSL_CLASS/UTILITY/BasePage.cs` (註冊驗證邏輯)

### 8.2 相關功能
- 加密/解密功能
- 檔案處理功能
- 硬體資訊讀取

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

