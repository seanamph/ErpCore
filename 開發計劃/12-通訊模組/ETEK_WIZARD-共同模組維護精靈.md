# ETEK_WIZARD - 共同模組維護精靈 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ETEK_WIZARD
- **功能名稱**: 共同模組維護精靈
- **功能描述**: 提供共同模組的快速建立與維護功能，包含資料表、權限、選單等自動化設定
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/ETEK_WIZARD.ASP`

### 1.2 業務需求
- 快速建立新的共同模組
- 自動建立資料表結構
- 自動設定系統權限
- 自動加入選單項目
- 支援交易處理
- 自動加入權限予使用者

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表

此功能會動態建立資料表，主要涉及以下系統表：

#### 2.1.1 `Systems` - 系統主檔
```sql
CREATE TABLE [dbo].[Systems] (
    [SysId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [SysName] NVARCHAR(100) NOT NULL,
    [SysDesc] NVARCHAR(500) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Systems] PRIMARY KEY CLUSTERED ([SysId] ASC)
);
```

#### 2.1.2 `Programs` - 作業主檔
```sql
CREATE TABLE [dbo].[Programs] (
    [ProgId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [SysId] NVARCHAR(50) NOT NULL,
    [ProgName] NVARCHAR(100) NOT NULL,
    [ProgDesc] NVARCHAR(500) NULL,
    [ProgType] NVARCHAR(20) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_Programs_Systems] FOREIGN KEY ([SysId]) REFERENCES [dbo].[Systems] ([SysId])
);
```

#### 2.1.3 `UserPermissions` - 使用者權限
```sql
CREATE TABLE [dbo].[UserPermissions] (
    [UserId] NVARCHAR(50) NOT NULL,
    [ProgId] NVARCHAR(50) NOT NULL,
    [Permission] NVARCHAR(20) NOT NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED ([UserId], [ProgId], [Permission]),
    CONSTRAINT [FK_UserPermissions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserPermissions_Programs] FOREIGN KEY ([ProgId]) REFERENCES [dbo].[Programs] ([ProgId])
);
```

### 2.2 動態資料表結構

精靈會根據使用者輸入的欄位資訊動態建立資料表，基本結構包含：
- 主鍵欄位
- 使用者定義欄位
- 系統欄位（建立者、建立時間、更新者、更新時間等）

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 步驟一：建立系統與作業
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-wizard/step1`
- **說明**: 建立系統與作業基本資料
- **請求格式**:
  ```json
  {
    "sysId": "SYS001",
    "sysName": "系統名稱",
    "sysDesc": "系統描述",
    "progId": "PROG001",
    "progName": "作業名稱",
    "progDesc": "作業描述",
    "progType": "MAINTENANCE"
  }
  ```

#### 3.1.2 步驟二：建立資料表結構
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-wizard/step2`
- **說明**: 根據欄位定義建立資料表
- **請求格式**:
  ```json
  {
    "tableName": "TableName",
    "columns": [
      {
        "columnName": "Column1",
        "dataType": "NVARCHAR",
        "length": 50,
        "nullable": false,
        "isPrimaryKey": true
      }
    ]
  }
  ```

#### 3.1.3 步驟三：設定權限
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-wizard/step3`
- **說明**: 設定使用者或角色權限
- **請求格式**:
  ```json
  {
    "progId": "PROG001",
    "userIds": ["U001", "U002"],
    "roleIds": ["R001"]
  }
  ```

#### 3.1.4 完成精靈
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/etek-wizard/complete`
- **說明**: 完成所有步驟並提交
- **請求格式**:
  ```json
  {
    "wizardId": "WIZARD001",
    "steps": {
      "step1": {...},
      "step2": {...},
      "step3": {...}
    }
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `EtekWizardController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/etek-wizard")]
    [Authorize]
    public class EtekWizardController : ControllerBase
    {
        private readonly IEtekWizardService _etekWizardService;
        
        public EtekWizardController(IEtekWizardService etekWizardService)
        {
            _etekWizardService = etekWizardService;
        }
        
        [HttpPost("step1")]
        public async Task<ActionResult<ApiResponse>> Step1([FromBody] Step1Dto dto)
        {
            // 實作步驟一邏輯
        }
        
        [HttpPost("step2")]
        public async Task<ActionResult<ApiResponse>> Step2([FromBody] Step2Dto dto)
        {
            // 實作步驟二邏輯
        }
        
        [HttpPost("step3")]
        public async Task<ActionResult<ApiResponse>> Step3([FromBody] Step3Dto dto)
        {
            // 實作步驟三邏輯
        }
        
        [HttpPost("complete")]
        public async Task<ActionResult<ApiResponse>> Complete([FromBody] CompleteWizardDto dto)
        {
            // 實作完成邏輯，使用交易處理
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 精靈頁面 (`EtekWizard.vue`)
- **路徑**: `/system/etek-wizard`
- **功能**: 多步驟精靈介面，引導使用者完成模組建立
- **主要元件**:
  - 步驟指示器 (StepIndicator)
  - 步驟一表單 (Step1Form)
  - 步驟二表單 (Step2Form)
  - 步驟三表單 (Step3Form)
  - 完成確認 (CompleteDialog)

### 4.2 UI 元件設計

#### 4.2.1 步驟指示器 (`StepIndicator.vue`)
```vue
<template>
  <el-steps :active="currentStep" finish-status="success">
    <el-step title="建立系統與作業" />
    <el-step title="建立資料表結構" />
    <el-step title="設定權限" />
    <el-step title="完成" />
  </el-steps>
</template>
```

#### 4.2.2 步驟一表單 (`Step1Form.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
    <el-form-item label="系統編號" prop="sysId">
      <el-input v-model="form.sysId" placeholder="請輸入系統編號" />
    </el-form-item>
    <el-form-item label="系統名稱" prop="sysName">
      <el-input v-model="form.sysName" placeholder="請輸入系統名稱" />
    </el-form-item>
    <el-form-item label="系統描述" prop="sysDesc">
      <el-input v-model="form.sysDesc" type="textarea" :rows="3" placeholder="請輸入系統描述" />
    </el-form-item>
    <el-form-item label="作業編號" prop="progId">
      <el-input v-model="form.progId" placeholder="請輸入作業編號" />
    </el-form-item>
    <el-form-item label="作業名稱" prop="progName">
      <el-input v-model="form.progName" placeholder="請輸入作業名稱" />
    </el-form-item>
    <el-form-item label="作業描述" prop="progDesc">
      <el-input v-model="form.progDesc" type="textarea" :rows="3" placeholder="請輸入作業描述" />
    </el-form-item>
  </el-form>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (2天)
- [ ] 分析動態資料表建立邏輯
- [ ] 設計系統表結構
- [ ] 設計作業表結構
- [ ] 設計權限表結構

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含動態SQL建立）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 交易處理邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 精靈頁面開發
- [ ] 步驟表單開發
- [ ] 步驟指示器開發
- [ ] 表單驗證
- [ ] Loading 狀態處理
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 交易處理測試
- [ ] 錯誤處理測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 14天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查（僅系統管理員可使用）
- 動態SQL必須防範SQL注入
- 必須驗證使用者輸入

### 6.2 交易處理
- 所有步驟必須在交易中執行
- 失敗時必須回滾所有變更
- 必須記錄操作日誌

### 6.3 資料驗證
- 系統編號必須唯一
- 作業編號必須唯一
- 資料表名稱必須符合命名規範
- 欄位定義必須驗證

### 6.4 業務邏輯
- 建立資料表前必須檢查是否已存在
- 設定權限前必須檢查使用者/角色是否存在
- 必須自動加入權限予建立者

---

## 七、測試案例

### 7.1 單元測試
- [ ] 步驟一：建立系統與作業成功
- [ ] 步驟一：建立系統與作業失敗（重複編號）
- [ ] 步驟二：建立資料表成功
- [ ] 步驟二：建立資料表失敗（表名已存在）
- [ ] 步驟三：設定權限成功
- [ ] 完成精靈：所有步驟成功
- [ ] 完成精靈：部分步驟失敗，交易回滾

### 7.2 整合測試
- [ ] 完整精靈流程測試
- [ ] 權限檢查測試
- [ ] 交易處理測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量欄位建立測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/ETEK_WIZARD.ASP`

### 8.2 相關功能
- 系統管理功能
- 權限管理功能
- 資料表管理功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

