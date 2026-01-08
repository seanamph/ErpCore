# Identify - 系統識別功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Identify
- **功能名稱**: 系統識別功能
- **功能描述**: 提供系統框架識別與初始化功能，用於系統主框架頁面，包含選單設定、工具列設定、訊息設定、EIP整合等功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/Identify.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/Identify.aspx.cs`
  - `WEB/IMS_CORE/Kernel/Identify.aspx`

### 1.2 業務需求
- 系統框架識別與初始化
- 選單設定與顯示
- 工具列設定與顯示
- 訊息設定與顯示
- EIP系統整合
- 使用者登入狀態檢查
- 系統標題設定
- 框架功能初始化

### 1.3 使用場景
- 系統主框架頁面
- 系統初始化
- 使用者登入後的主頁面
- EIP系統整合入口

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `SystemIdentities` (系統識別設定)

```sql
CREATE TABLE [dbo].[SystemIdentities] (
    [IdentityId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SystemId] NVARCHAR(50) NOT NULL, -- 系統代碼
    [ProjectTitle] NVARCHAR(200) NOT NULL, -- 專案標題
    [CompanyTitle] NVARCHAR(200) NULL, -- 公司標題
    [EipUrl] NVARCHAR(500) NULL, -- EIP系統URL
    [EipEmbedded] NVARCHAR(10) NULL DEFAULT 'N', -- EIP嵌入模式 (Y/N)
    [ShowTransEffect] NVARCHAR(10) NULL DEFAULT 'N', -- 顯示轉場效果 (Y/N)
    [InitShowTransEffect] NVARCHAR(10) NULL DEFAULT 'N', -- 初始顯示轉場效果 (Y/N)
    [NoResizeFrame] BIT NOT NULL DEFAULT 1, -- 不調整框架大小
    [DebugUser] NVARCHAR(50) NULL, -- 除錯使用者
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 1:啟用, 0:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_SystemIdentities_SystemId] UNIQUE ([SystemId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemIdentities_SystemId] ON [dbo].[SystemIdentities] ([SystemId]);
CREATE NONCLUSTERED INDEX [IX_SystemIdentities_Status] ON [dbo].[SystemIdentities] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `SystemIdentityLogs` - 系統識別操作日誌
```sql
CREATE TABLE [dbo].[SystemIdentityLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [IdentityId] BIGINT NULL,
    [UserId] NVARCHAR(50) NOT NULL,
    [OperationType] NVARCHAR(20) NOT NULL, -- INIT, LOGIN, LOGOUT, CONFIG
    [SystemId] NVARCHAR(50) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_SystemIdentityLogs_SystemIdentities] FOREIGN KEY ([IdentityId]) REFERENCES [dbo].[SystemIdentities] ([IdentityId]),
    CONSTRAINT [FK_SystemIdentityLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_IdentityId] ON [dbo].[SystemIdentityLogs] ([IdentityId]);
CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_UserId] ON [dbo].[SystemIdentityLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_SystemIdentityLogs_CreatedAt] ON [dbo].[SystemIdentityLogs] ([CreatedAt]);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| IdentityId | BIGINT | - | NO | IDENTITY(1,1) | 識別設定ID | 主鍵 |
| SystemId | NVARCHAR | 50 | NO | - | 系統代碼 | 唯一 |
| ProjectTitle | NVARCHAR | 200 | NO | - | 專案標題 | - |
| CompanyTitle | NVARCHAR | 200 | YES | - | 公司標題 | - |
| EipUrl | NVARCHAR | 500 | YES | - | EIP系統URL | - |
| EipEmbedded | NVARCHAR | 10 | YES | 'N' | EIP嵌入模式 | Y/N |
| ShowTransEffect | NVARCHAR | 10 | YES | 'N' | 顯示轉場效果 | Y/N |
| InitShowTransEffect | NVARCHAR | 10 | YES | 'N' | 初始顯示轉場效果 | Y/N |
| NoResizeFrame | BIT | - | NO | 1 | 不調整框架大小 | - |
| DebugUser | NVARCHAR | 50 | YES | - | 除錯使用者 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得系統識別資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/system-identity`
- **說明**: 取得系統識別資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "identityId": 1,
      "systemId": "IMS3",
      "projectTitle": "IMS3 系統",
      "companyTitle": "RSL 公司",
      "eipUrl": "https://eip.example.com",
      "eipEmbedded": "Y",
      "showTransEffect": "N",
      "initShowTransEffect": "Y",
      "noResizeFrame": true,
      "debugUser": "DEBUG001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 取得選單設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/system-identity/menu`
- **說明**: 取得系統選單設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "selectMessage": "請選擇功能",
      "selectToolbar": "工具列設定",
      "showLeftButton": "Y"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 更新系統識別設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/kernel/system-identity`
- **說明**: 更新系統識別設定
- **請求格式**:
  ```json
  {
    "projectTitle": "IMS3 系統",
    "companyTitle": "RSL 公司",
    "eipUrl": "https://eip.example.com",
    "eipEmbedded": "Y",
    "showTransEffect": "N",
    "initShowTransEffect": "Y",
    "noResizeFrame": true,
    "debugUser": "DEBUG001"
  }
  ```

#### 3.1.4 系統初始化
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/system-identity/initialize`
- **說明**: 系統初始化
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "初始化成功",
    "data": {
      "systemId": "IMS3",
      "projectTitle": "IMS3 系統",
      "companyTitle": "RSL 公司",
      "eipUrl": "https://eip.example.com",
      "menuConfig": {},
      "toolbarConfig": {},
      "messageConfig": {}
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SystemIdentityController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/system-identity")]
    [Authorize]
    public class SystemIdentityController : ControllerBase
    {
        private readonly ISystemIdentityService _systemIdentityService;
        
        public SystemIdentityController(ISystemIdentityService systemIdentityService)
        {
            _systemIdentityService = systemIdentityService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<SystemIdentityDto>>> GetSystemIdentity()
        {
            // 實作取得系統識別資訊邏輯
        }
        
        [HttpGet("menu")]
        public async Task<ActionResult<ApiResponse<MenuConfigDto>>> GetMenuConfig()
        {
            // 實作取得選單設定邏輯
        }
        
        [HttpPut]
        public async Task<ActionResult<ApiResponse>> UpdateSystemIdentity([FromBody] UpdateSystemIdentityDto dto)
        {
            // 實作更新系統識別設定邏輯
        }
        
        [HttpPost("initialize")]
        public async Task<ActionResult<ApiResponse<SystemInitDto>>> Initialize()
        {
            // 實作系統初始化邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統識別頁面 (`Identify.vue`)
- **路徑**: `/system/kernel/identify`
- **功能**: 系統框架識別與初始化
- **主要元件**:
  - 系統標題顯示
  - 選單框架
  - 工具列框架
  - 訊息框架
  - EIP整合框架

### 4.2 UI 元件設計

#### 4.2.1 系統識別主頁面 (`Identify.vue`)
```vue
<template>
  <div class="identify-container">
    <!-- 系統標題 -->
    <div class="system-header">
      <h1>{{ systemIdentity.projectTitle }} - {{ systemIdentity.companyTitle }}</h1>
    </div>
    
    <!-- 框架容器 -->
    <div class="frame-container">
      <!-- 左側選單 -->
      <div class="left-frame">
        <FrameMenu />
      </div>
      
      <!-- 主內容區 -->
      <div class="main-frame">
        <FrameMainMenu />
        <FrameMessage />
        <FrameToolbar />
        <router-view />
      </div>
      
      <!-- EIP整合框架 (可選) -->
      <div class="eip-frame" v-if="systemIdentity.eipEmbedded === 'Y'">
        <iframe :src="eipUrl" frameborder="0"></iframe>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getSystemIdentity, initializeSystem } from '@/api/kernel/systemIdentity.api';
import FrameMenu from '@/components/kernel/FrameMenu.vue';
import FrameMainMenu from '@/components/kernel/FrameMainMenu.vue';
import FrameMessage from '@/components/kernel/FrameMessage.vue';
import FrameToolbar from '@/components/kernel/FrameToolbar.vue';

const systemIdentity = ref<any>({});
const eipUrl = ref('');

onMounted(async () => {
  const response = await initializeSystem();
  if (response.data.success) {
    systemIdentity.value = response.data.data;
    if (systemIdentity.value.eipEmbedded === 'Y') {
      eipUrl.value = systemIdentity.value.eipUrl || '/frameEIP.aspx?URL=home/index.php';
    }
  }
});
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 系統初始化邏輯
- [ ] 選單設定邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 系統識別主頁面開發
- [ ] 框架元件整合
- [ ] EIP整合功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 系統初始化測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 5.5天

---

## 六、注意事項

### 6.1 安全性
- 必須驗證使用者登入狀態
- 必須驗證系統識別資訊的合法性
- EIP整合必須使用安全連線

### 6.2 效能
- 系統初始化必須快速
- 必須使用快取機制
- 框架載入必須優化

### 6.3 資料驗證
- 系統識別資訊必須驗證
- EIP URL必須驗證
- 設定值必須驗證

### 6.4 業務邏輯
- 必須正確初始化系統框架
- 必須正確載入選單設定
- 必須正確處理EIP整合

---

## 七、測試案例

### 7.1 單元測試
- [ ] 系統識別資訊取得成功
- [ ] 選單設定取得成功
- [ ] 系統初始化成功
- [ ] EIP整合成功

### 7.2 整合測試
- [ ] 完整系統初始化流程測試
- [ ] 框架載入測試
- [ ] EIP整合測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/Identify.aspx.cs`
- `WEB/IMS_CORE/Kernel/Identify.aspx`

### 8.2 相關功能
- FrameMenu - 框架選單功能
- FrameMainMenu - 框架主選單功能
- FrameMessage - 框架訊息功能
- IMS2EIP - EIP系統整合

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

