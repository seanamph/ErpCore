# FrameHead - 框架標題功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: FrameHead
- **功能名稱**: 框架標題功能
- **功能描述**: 提供系統框架的標題區域功能，顯示系統Logo、系統名稱、歡迎訊息、當前日期時間、系統註冊到期日等資訊
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameHead.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/FrameHead.aspx.cs`

### 1.2 業務需求
- 顯示系統Logo圖片
- 顯示系統名稱（中文、英文）
- 顯示系統版本號（可選）
- 顯示歡迎訊息（含使用者名稱）
- 顯示當前日期時間（即時更新）
- 顯示系統註冊到期日（特定使用者可見）
- 顯示測試環境標記（測試環境時顯示）
- 支援多語言顯示
- 支援頁面轉場效果設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

本功能主要使用配置檔和系統設定，不需要額外的資料表設計。相關資料表參考：
- `Users` - 使用者主檔（取得使用者資訊）
- `Systems` - 系統主檔（取得系統資訊）

### 2.2 配置資料

系統標題相關配置儲存在 `appsettings.json` 或資料庫配置表中：

```json
{
  "SystemSettings": {
    "ProjectTitle": "IMS3 系統",
    "ProjectTitleEn": "IMS3 System",
    "Welcome": "歡迎",
    "LogoPath": "/images/logo_ims.jpg",
    "ShowHeadVersion": true,
    "AlwaysShowExpDate": false,
    "InitShowTransEffect": false
  }
}
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統標題資訊

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system/header-info`
- **說明**: 查詢系統標題區域所需的資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "projectTitle": "IMS3 系統",
      "projectTitleEn": "IMS3 System",
      "version": "v1.0.0",
      "welcome": "歡迎",
      "userName": "張三",
      "logoPath": "/images/logo_ims.jpg",
      "showVersion": true,
      "currentDate": "2024-01-01T12:00:00Z",
      "expireDate": "2024-12-31T23:59:59Z",
      "showExpireDate": false,
      "isTestEnvironment": false
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢當前時間

- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/system/current-time`
- **說明**: 查詢伺服器當前時間（用於時間同步）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "currentTime": "2024-01-01T12:00:00Z",
      "timezone": "Asia/Taipei"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `SystemController.cs`

```csharp
[ApiController]
[Route("api/v1/system")]
[Authorize]
public class SystemController : ControllerBase
{
    private readonly ISystemService _systemService;
    private readonly ICurrentUserService _currentUserService;

    public SystemController(
        ISystemService systemService,
        ICurrentUserService currentUserService)
    {
        _systemService = systemService;
        _currentUserService = currentUserService;
    }

    [HttpGet("header-info")]
    public async Task<ActionResult<ApiResponse<SystemHeaderInfoDto>>> GetHeaderInfo()
    {
        var result = await _systemService.GetHeaderInfoAsync();
        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("current-time")]
    public ActionResult<ApiResponse<CurrentTimeDto>> GetCurrentTime()
    {
        var result = _systemService.GetCurrentTime();
        return Ok(ApiResponse.Success(result));
    }
}
```

#### 3.2.2 Service: `ISystemService.cs` / `SystemService.cs`

```csharp
public interface ISystemService
{
    Task<SystemHeaderInfoDto> GetHeaderInfoAsync();
    CurrentTimeDto GetCurrentTime();
}

public class SystemService : ISystemService
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public SystemService(
        IConfiguration configuration,
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    public async Task<SystemHeaderInfoDto> GetHeaderInfoAsync()
    {
        var userId = _currentUserService.GetUserId();
        var user = await _userRepository.GetUserAsync(userId);

        var settings = _configuration.GetSection("SystemSettings");

        var result = new SystemHeaderInfoDto
        {
            ProjectTitle = settings["ProjectTitle"] ?? "IMS3 系統",
            ProjectTitleEn = settings["ProjectTitleEn"] ?? "IMS3 System",
            Version = GetVersion(),
            Welcome = settings["Welcome"] ?? "歡迎",
            UserName = user?.UserName ?? "",
            LogoPath = settings["LogoPath"] ?? "/images/logo_ims.jpg",
            ShowVersion = settings.GetValue<bool>("ShowHeadVersion", true),
            CurrentDate = DateTime.Now,
            ExpireDate = GetExpireDate(),
            ShowExpireDate = ShouldShowExpireDate(userId),
            IsTestEnvironment = IsTestEnvironment()
        };

        return result;
    }

    public CurrentTimeDto GetCurrentTime()
    {
        return new CurrentTimeDto
        {
            CurrentTime = DateTime.Now,
            Timezone = TimeZoneInfo.Local.Id
        };
    }

    private string GetVersion()
    {
        // 從組件資訊取得版本號
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version?.ToString() ?? "v1.0.0";
    }

    private DateTime? GetExpireDate()
    {
        // 從註冊檔案讀取到期日
        var companyId = _configuration["CompanyId"];
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "kernel",
            $"RegSys_{companyId}.dll"
        );

        if (File.Exists(filePath))
        {
            try
            {
                var content = File.ReadAllText(filePath);
                var expireDateStr = DecodeBase64(content);
                if (DateTime.TryParse(expireDateStr, out var expireDate))
                {
                    return expireDate;
                }
            }
            catch
            {
                // 忽略錯誤
            }
        }

        return null;
    }

    private bool ShouldShowExpireDate(string userId)
    {
        var alwaysShow = _configuration.GetValue<bool>("SystemSettings:AlwaysShowExpDate", false);
        return alwaysShow && userId == "xcom";
    }

    private bool IsTestEnvironment()
    {
        var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
        return environment == "Development" || environment == "Test";
    }

    private string DecodeBase64(string base64String)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 標題區域元件 (`FrameHead.vue`)

```vue
<template>
  <div class="frame-head">
    <div class="head-container">
      <!-- 左側：Logo 和系統名稱 -->
      <div class="head-left">
        <img :src="headerInfo.logoPath" alt="Logo" class="logo" />
        <div class="system-info">
          <div class="system-title">{{ headerInfo.projectTitle }}</div>
          <div class="system-title-en">{{ headerInfo.projectTitleEn }}</div>
          <div v-if="headerInfo.showVersion" class="system-version">
            Version：{{ headerInfo.version }}
          </div>
        </div>
      </div>

      <!-- 中間：測試環境標記和到期日 -->
      <div class="head-center">
        <div v-if="headerInfo.isTestEnvironment" class="test-badge">
          {{ $t('system.testEnvironment') }}
        </div>
        <div v-if="headerInfo.showExpireDate && headerInfo.expireDate" class="expire-date">
          <span class="expire-label">{{ $t('system.expireDate') }}：</span>
          <span class="expire-value">{{ formatDate(headerInfo.expireDate) }}</span>
        </div>
      </div>

      <!-- 右側：日期時間和歡迎訊息 -->
      <div class="head-right">
        <div class="current-date">
          {{ $t('system.todayDate') }}：<span id="sys-time">{{ currentTime }}</span>
        </div>
        <div class="welcome-message">
          ~ {{ headerInfo.welcome }} {{ headerInfo.userName }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { getHeaderInfo, getCurrentTime } from '@/api/system.api';
import type { SystemHeaderInfoDto } from '@/types/system';

const { t } = useI18n();

const headerInfo = ref<SystemHeaderInfoDto>({
  projectTitle: '',
  projectTitleEn: '',
  version: '',
  welcome: '',
  userName: '',
  logoPath: '',
  showVersion: false,
  currentDate: new Date(),
  expireDate: null,
  showExpireDate: false,
  isTestEnvironment: false
});

const currentTime = ref<string>('');
let timeInterval: number | null = null;

const loadHeaderInfo = async () => {
  try {
    const response = await getHeaderInfo();
    headerInfo.value = response.data;
    updateCurrentTime();
  } catch (error) {
    console.error('Failed to load header info:', error);
  }
};

const updateCurrentTime = () => {
  const now = new Date();
  currentTime.value = formatDateTime(now);
  
  // 每秒更新時間
  if (timeInterval) {
    clearInterval(timeInterval);
  }
  
  timeInterval = window.setInterval(() => {
    const now = new Date();
    currentTime.value = formatDateTime(now);
    const timeElement = document.getElementById('sys-time');
    if (timeElement) {
      timeElement.textContent = formatDateTime(now);
    }
  }, 1000);
};

const formatDateTime = (date: Date): string => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  const seconds = String(date.getSeconds()).padStart(2, '0');
  
  return `${year}/${month}/${day} ${hours}:${minutes}:${seconds}`;
};

const formatDate = (date: Date | string | null): string => {
  if (!date) return '';
  const d = typeof date === 'string' ? new Date(date) : date;
  return d.toLocaleDateString('zh-TW');
};

// 同步伺服器時間
const syncServerTime = async () => {
  try {
    const response = await getCurrentTime();
    const serverTime = new Date(response.data.currentTime);
    const clientTime = new Date();
    const diff = serverTime.getTime() - clientTime.getTime();
    
    // 如果時間差超過1秒，調整本地時間
    if (Math.abs(diff) > 1000) {
      updateCurrentTime();
    }
  } catch (error) {
    console.error('Failed to sync server time:', error);
  }
};

onMounted(() => {
  loadHeaderInfo();
  syncServerTime();
  
  // 每分鐘同步一次伺服器時間
  const syncInterval = window.setInterval(syncServerTime, 60000);
  
  onUnmounted(() => {
    if (timeInterval) {
      clearInterval(timeInterval);
    }
    clearInterval(syncInterval);
  });
});
</script>

<style scoped lang="scss">
.frame-head {
  background: linear-gradient(to right, #ffffff 0%, #f0f0f0 100%);
  border-bottom: 1px solid #e4e7ed;
  padding: 10px 20px;

  .head-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    max-width: 100%;

    .head-left {
      display: flex;
      align-items: center;
      gap: 15px;

      .logo {
        width: 63px;
        height: 42px;
        object-fit: contain;
      }

      .system-info {
        .system-title {
          font-size: 16px;
          font-weight: bold;
          color: #303133;
          margin-bottom: 5px;
        }

        .system-title-en {
          font-size: 14px;
          color: #606266;
          margin-bottom: 5px;
        }

        .system-version {
          font-size: 12px;
          color: #909399;
        }
      }
    }

    .head-center {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 5px;

      .test-badge {
        background-color: #f56c6c;
        color: white;
        padding: 4px 12px;
        border-radius: 4px;
        font-size: 12px;
        font-weight: bold;
      }

      .expire-date {
        font-size: 12px;

        .expire-label {
          color: #606266;
        }

        .expire-value {
          color: #f56c6c;
          font-weight: bold;
        }
      }
    }

    .head-right {
      display: flex;
      flex-direction: column;
      align-items: flex-end;
      gap: 5px;

      .current-date {
        font-size: 12px;
        color: #606266;

        #sys-time {
          font-weight: bold;
          color: #303133;
        }
      }

      .welcome-message {
        font-size: 12px;
        color: #606266;
      }
    }
  }
}

@media (max-width: 768px) {
  .frame-head {
    .head-container {
      flex-direction: column;
      gap: 10px;

      .head-left,
      .head-center,
      .head-right {
        width: 100%;
        align-items: flex-start;
      }
    }
  }
}
</style>
```

### 4.2 API 呼叫 (`system.api.ts`)

```typescript
import request from '@/utils/request';
import type { ApiResponse } from '@/types/common';
import type { SystemHeaderInfoDto, CurrentTimeDto } from '@/types/system';

export const getHeaderInfo = () => {
  return request.get<ApiResponse<SystemHeaderInfoDto>>('/api/v1/system/header-info');
};

export const getCurrentTime = () => {
  return request.get<ApiResponse<CurrentTimeDto>>('/api/v1/system/current-time');
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (1.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 配置管理
- [ ] 單元測試

### 5.2 階段二: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 標題區域元件開發
- [ ] 時間同步功能開發
- [ ] 響應式設計
- [ ] 元件測試

### 5.3 階段三: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 時間同步測試

### 5.4 階段四: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 4天

---

## 六、注意事項

### 6.1 業務邏輯
- 系統標題資訊需從配置檔讀取
- 時間顯示需即時更新（每秒）
- 時間需與伺服器同步（每分鐘）
- 到期日僅特定使用者可見
- 測試環境標記需根據環境變數顯示

### 6.2 資料驗證
- Logo 路徑必須有效
- 版本號需從組件資訊取得
- 到期日需從註冊檔案讀取

### 6.3 效能
- 時間更新需使用 setInterval
- 伺服器時間同步需使用適當的間隔
- 避免頻繁的 API 呼叫

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢系統標題資訊成功
- [ ] 查詢當前時間成功
- [ ] 時間格式化正確
- [ ] 到期日顯示邏輯正確

### 7.2 整合測試
- [ ] 完整標題顯示流程測試
- [ ] 時間同步功能測試
- [ ] 響應式設計測試
- [ ] 多語言顯示測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/FrameHead.aspx`
- `IMS3/HANSHIN/IMS3/KERNEL/FrameHead.aspx.cs`

### 8.2 相關功能
- `FrameMenu-框架選單功能.md` - 框架選單功能
- `FrameMainMenu-框架主選單功能.md` - 框架主選單功能
- `FrameMessage-框架訊息功能.md` - 框架訊息功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

