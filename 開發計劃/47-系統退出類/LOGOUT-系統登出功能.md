# LOGOUT - 系統登出功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: LOGOUT
- **功能名稱**: 系統登出功能
- **功能描述**: 提供使用者登出系統功能，清除使用者Session、Token等認證資訊，並記錄登出時間
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYS9999/LOGOUT.ASP`
  - `IMS3/HANSHIN/IMS3/ASP/SYS9999/LOGOUT.ASP`

### 1.2 業務需求
- 清除使用者Session資訊
- 清除JWT Token
- 記錄使用者登出時間
- 記錄使用者登出IP
- 清除前端快取資料
- 導向登入頁面

---

## 二、資料庫設計 (Schema)

### 2.1 相關資料表

#### 2.1.1 `Users` - 使用者主檔
- 用於更新使用者最後登出時間
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md`

#### 2.1.2 `LoginLogs` - 登入日誌
```sql
CREATE TABLE [dbo].[LoginLogs] (
    [LogId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [LoginTime] DATETIME2 NOT NULL,
    [LogoutTime] DATETIME2 NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'SUCCESS', -- SUCCESS, FAILED, TIMEOUT
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_LoginLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LoginLogs_UserId] ON [dbo].[LoginLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_LoginLogs_LoginTime] ON [dbo].[LoginLogs] ([LoginTime]);
CREATE NONCLUSTERED INDEX [IX_LoginLogs_LogoutTime] ON [dbo].[LoginLogs] ([LogoutTime]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| LogId | BIGINT | - | NO | IDENTITY(1,1) | 日誌編號 | 主鍵，自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users表 |
| LoginTime | DATETIME2 | - | NO | - | 登入時間 | - |
| LogoutTime | DATETIME2 | - | YES | - | 登出時間 | - |
| IpAddress | NVARCHAR | 50 | YES | - | IP位址 | - |
| UserAgent | NVARCHAR | 500 | YES | - | 使用者代理 | - |
| Status | NVARCHAR | 10 | NO | 'SUCCESS' | 狀態 | SUCCESS, FAILED, TIMEOUT |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 登出系統
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/auth/logout`
- **說明**: 使用者登出系統，清除認證資訊
- **請求格式**: 
  ```json
  {
    "token": "JWT_TOKEN"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "登出成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 強制登出
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/auth/logout/force`
- **說明**: 管理員強制登出指定使用者
- **權限**: 需要管理員權限
- **請求格式**:
  ```json
  {
    "userId": "U001"
  }
  ```
- **回應格式**: 同登出系統

### 3.2 後端實作類別

#### 3.2.1 Controller: `AuthController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout([FromBody] LogoutDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _authService.LogoutAsync(userId, dto.Token);
            return Ok(new ApiResponse { Success = true, Message = "登出成功" });
        }
        
        [HttpPost("logout/force")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> ForceLogout([FromBody] ForceLogoutDto dto)
        {
            await _authService.ForceLogoutAsync(dto.UserId);
            return Ok(new ApiResponse { Success = true, Message = "強制登出成功" });
        }
    }
}
```

#### 3.2.2 Service: `AuthService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IAuthService
    {
        Task LogoutAsync(string userId, string token);
        Task ForceLogoutAsync(string userId);
        Task UpdateLogoutTimeAsync(string userId, string ipAddress);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 登出處理
- **功能**: 自動處理登出邏輯，清除前端認證資訊
- **觸發時機**: 
  - 使用者點擊登出按鈕
  - Token過期
  - 系統自動登出（閒置時間過長）

### 4.2 UI 元件設計

#### 4.2.1 登出按鈕元件 (`LogoutButton.vue`)
```vue
<template>
  <el-button type="danger" @click="handleLogout">登出</el-button>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { logout } from '@/api/auth.api';

const authStore = useAuthStore();
const router = useRouter();

const handleLogout = async () => {
  try {
    await logout();
    authStore.clearAuth();
    router.push('/login');
    ElMessage.success('登出成功');
  } catch (error) {
    ElMessage.error('登出失敗');
  }
};
</script>
```

### 4.3 API 呼叫 (`auth.api.ts`)
```typescript
import request from '@/utils/request';

export interface LogoutDto {
  token: string;
}

export interface ForceLogoutDto {
  userId: string;
}

// 登出系統
export const logout = (token?: string) => {
  return request.post<ApiResponse>('/api/v1/auth/logout', { 
    token: token || localStorage.getItem('token') 
  });
};

// 強制登出
export const forceLogout = (userId: string) => {
  return request.post<ApiResponse>('/api/v1/auth/logout/force', { userId });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立LoginLogs資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (1天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] Token黑名單機制
- [ ] 單元測試

### 5.3 階段三: 前端開發 (0.5天)
- [ ] API 呼叫函數
- [ ] 登出按鈕元件
- [ ] 登出處理邏輯
- [ ] 自動登出機制
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 3天

---

## 六、注意事項

### 6.1 安全性
- Token必須加入黑名單，防止重複使用
- 必須清除所有Session資訊
- 必須清除前端快取資料
- 必須記錄登出時間和IP

### 6.2 效能
- Token黑名單必須使用快取機制（Redis）
- 必須設定Token黑名單過期時間

### 6.3 業務邏輯
- 登出時必須更新使用者最後登出時間
- 登出時必須記錄登出日誌
- 登出時必須清除所有相關快取

---

## 七、測試案例

### 7.1 單元測試
- [ ] 登出成功
- [ ] 登出失敗（Token無效）
- [ ] 強制登出成功
- [ ] 強制登出失敗（無權限）

### 7.2 整合測試
- [ ] 完整登出流程測試
- [ ] Token黑名單測試
- [ ] 登出日誌記錄測試
- [ ] 自動登出測試

### 7.3 安全性測試
- [ ] Token重複使用測試
- [ ] 未授權登出測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYS9999/LOGOUT.ASP`
- `IMS3/HANSHIN/IMS3/ASP/SYS9999/LOGOUT.ASP`

### 8.2 相關功能
- 登入功能
- Token管理
- Session管理

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

