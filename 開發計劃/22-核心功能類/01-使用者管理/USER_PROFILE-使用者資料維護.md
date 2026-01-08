# USER_PROFILE - 使用者資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: USER_PROFILE
- **功能名稱**: 使用者資料維護
- **功能描述**: 提供使用者查看和維護個人基本資料、權限資訊、自訂項目和登入資訊的功能
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/USER_PROFILE.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/USER_PROFILE.aspx.cs`
  - `WEB/IMS_CORE/Kernel/USER_PROFILE.aspx`
  - `WEB/IMS_CORE/Kernel/USER_PROFILE2.aspx`
  - `WEB/IMS_CORE/Kernel/USER_PROFILE3.aspx`
  - `WEB/IMS_CORE/Kernel/USER_PROFILE4.aspx`

### 1.2 業務需求
- 使用者可查看個人基本資料（使用者代碼、名稱、職稱、組織等）
- 使用者可查看個人權限資訊
- 使用者可查看自訂項目
- 使用者可查看登入資訊（最後登入時間、IP等）
- 支援多種使用者資料顯示模式（USER_PROFILE、USER_PROFILE2、USER_PROFILE3、USER_PROFILE4）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Users`
參考 `SYS0110-使用者基本資料維護.md` 的 `Users` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `UserOrganizations` - 使用者組織權限
```sql
CREATE TABLE [dbo].[UserOrganizations] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [OrgId] NVARCHAR(50) NULL,
    [FloorId] NVARCHAR(50) NULL,
    [AreaId] NVARCHAR(50) NULL,
    [StoreId] NVARCHAR(50) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserOrganizations_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_UserId] ON [dbo].[UserOrganizations] ([UserId]);
```

#### 2.2.2 `Parameters` - 參數設定（用於使用者型態等）
參考 `SYSBC40-參數資料設定維護作業.md` 的 `Parameters` 資料表設計

#### 2.2.3 `OrgGroup` - 組織群組
```sql
CREATE TABLE [dbo].[OrgGroup] (
    [OrgId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [OrgName] NVARCHAR(100) NOT NULL,
    [ParentOrgId] NVARCHAR(50) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

### 2.3 資料字典

#### UserOrganizations 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users表 |
| OrgId | NVARCHAR | 50 | YES | - | 組織代號 | 'xx'表示全部 |
| FloorId | NVARCHAR | 50 | YES | - | 樓層代號 | 'xx'表示全部 |
| AreaId | NVARCHAR | 50 | YES | - | 區域代號 | 'xx'表示全部 |
| StoreId | NVARCHAR | 50 | YES | - | 專櫃代號 | 'xx'表示全部 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢當前使用者資料
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/users/profile`
- **說明**: 查詢當前登入使用者的完整資料，包含基本資料、權限、組織等資訊
- **認證**: 需要JWT Token
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "userId": "U001",
      "userName": "張三",
      "title": "經理",
      "userType": "INTERNAL",
      "userTypeName": "內部使用者",
      "orgId": "ORG001",
      "orgName": "資訊部",
      "startDate": "2024-01-01",
      "endDate": null,
      "lastLoginDate": "2024-01-15T10:00:00",
      "lastLoginIp": "192.168.1.100",
      "loginCount": 150,
      "organizations": [
        {
          "orgId": "ORG001",
          "orgName": "資訊部",
          "floorId": "xx",
          "areaId": "xx",
          "storeId": "xx"
        }
      ],
      "permissions": [],
      "customFields": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 更新使用者個人資料
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/users/profile`
- **說明**: 更新當前使用者的個人資料（僅限可修改的欄位）
- **認證**: 需要JWT Token
- **請求格式**:
  ```json
  {
    "title": "資深經理",
    "notes": "更新備註"
  }
  ```
- **回應格式**: 同查詢

### 3.2 後端實作類別

#### 3.2.1 Controller: `UserProfileController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users/profile")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var profile = await _userProfileService.GetUserProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.Success(profile));
        }
        
        [HttpPut]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var profile = await _userProfileService.UpdateUserProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.Success(profile));
        }
    }
}
```

#### 3.2.2 Service: `UserProfileService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto);
    }
    
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IParameterRepository _parameterRepository;
        private readonly IOrgRepository _orgRepository;
        
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"使用者 {userId} 不存在");
            }
            
            var userTypeName = await _parameterRepository.GetParameterContentAsync("USER_TYPE", user.UserType);
            var orgName = await _orgRepository.GetOrgNameAsync(user.OrgId);
            var organizations = await _userRepository.GetUserOrganizationsAsync(userId);
            
            return new UserProfileDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Title = user.Title,
                UserType = user.UserType,
                UserTypeName = userTypeName,
                OrgId = user.OrgId,
                OrgName = orgName,
                StartDate = user.StartDate,
                EndDate = user.EndDate,
                LastLoginDate = user.LastLoginDate,
                LastLoginIp = user.LastLoginIp,
                LoginCount = user.LoginCount,
                Organizations = organizations.Select(o => new UserOrgDto
                {
                    OrgId = o.OrgId,
                    OrgName = o.OrgName,
                    FloorId = o.FloorId,
                    AreaId = o.AreaId,
                    StoreId = o.StoreId
                }).ToList()
            };
        }
        
        public async Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"使用者 {userId} 不存在");
            }
            
            // 僅允許修改特定欄位
            if (!string.IsNullOrEmpty(dto.Title))
            {
                user.Title = dto.Title;
            }
            if (!string.IsNullOrEmpty(dto.Notes))
            {
                user.Notes = dto.Notes;
            }
            
            user.UpdatedBy = userId;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.UpdateAsync(user);
            
            return await GetUserProfileAsync(userId);
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 使用者資料維護頁面 (`UserProfile.vue`)
- **路徑**: `/user/profile`
- **功能**: 顯示使用者個人資料，支援查看和部分修改
- **主要元件**:
  - 基本資料區塊 (BasicInfoSection)
  - 權限資訊區塊 (PermissionSection)
  - 自訂項目區塊 (CustomFieldsSection)
  - 登入資訊區塊 (LoginInfoSection)

### 4.2 UI 元件設計

#### 4.2.1 使用者資料維護頁面 (`UserProfile.vue`)
```vue
<template>
  <div class="user-profile">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>使用者基本資料</span>
        </div>
      </template>
      
      <el-descriptions :column="2" border>
        <el-descriptions-item label="使用者代碼">{{ profile.userId }}</el-descriptions-item>
        <el-descriptions-item label="使用者名稱">{{ profile.userName }}</el-descriptions-item>
        <el-descriptions-item label="職稱">
          <el-input v-if="editing" v-model="form.title" />
          <span v-else>{{ profile.title }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="使用者型態">{{ profile.userTypeName }}</el-descriptions-item>
        <el-descriptions-item label="組織">{{ profile.orgName }}</el-descriptions-item>
        <el-descriptions-item label="有效起始日">{{ formatDate(profile.startDate) }}</el-descriptions-item>
        <el-descriptions-item label="有效終止日">{{ formatDate(profile.endDate) }}</el-descriptions-item>
      </el-descriptions>
    </el-card>
    
    <el-card class="mt-4">
      <template #header>
        <div class="card-header">
          <span>使用者權限</span>
        </div>
      </template>
      
      <el-descriptions :column="2" border>
        <el-descriptions-item label="組織權限" :span="2">
          <el-tag v-for="org in profile.organizations" :key="org.orgId" class="mr-2">
            {{ org.orgName }}
            <span v-if="org.floorId !== 'xx'"> - {{ org.floorId }}</span>
            <span v-if="org.areaId !== 'xx'"> - {{ org.areaId }}</span>
            <span v-if="org.storeId !== 'xx'"> - {{ org.storeId }}</span>
          </el-tag>
        </el-descriptions-item>
      </el-descriptions>
    </el-card>
    
    <el-card class="mt-4">
      <template #header>
        <div class="card-header">
          <span>登入資訊</span>
        </div>
      </template>
      
      <el-descriptions :column="2" border>
        <el-descriptions-item label="最後登入時間">{{ formatDateTime(profile.lastLoginDate) }}</el-descriptions-item>
        <el-descriptions-item label="最後登入IP">{{ profile.lastLoginIp }}</el-descriptions-item>
        <el-descriptions-item label="登入次數">{{ profile.loginCount }}</el-descriptions-item>
      </el-descriptions>
    </el-card>
    
    <div class="mt-4 text-right">
      <el-button v-if="!editing" type="primary" @click="handleEdit">編輯</el-button>
      <template v-else>
        <el-button @click="handleCancel">取消</el-button>
        <el-button type="primary" @click="handleSave">儲存</el-button>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getUserProfile, updateUserProfile } from '@/api/user-profile';
import type { UserProfileDto, UpdateUserProfileDto } from '@/api/user-profile';

const profile = ref<UserProfileDto>({} as UserProfileDto);
const editing = ref(false);
const form = ref<UpdateUserProfileDto>({
  title: '',
  notes: ''
});

const loadProfile = async () => {
  try {
    const response = await getUserProfile();
    profile.value = response.data;
    form.value.title = profile.value.title || '';
  } catch (error) {
    console.error('載入使用者資料失敗', error);
  }
};

const handleEdit = () => {
  editing.value = true;
};

const handleCancel = () => {
  editing.value = false;
  form.value.title = profile.value.title || '';
};

const handleSave = async () => {
  try {
    const response = await updateUserProfile(form.value);
    profile.value = response.data;
    editing.value = false;
    ElMessage.success('更新成功');
  } catch (error) {
    console.error('更新失敗', error);
    ElMessage.error('更新失敗');
  }
};

const formatDate = (date: string | null) => {
  if (!date) return '-';
  return new Date(date).toLocaleDateString('zh-TW');
};

const formatDateTime = (date: string | null) => {
  if (!date) return '-';
  return new Date(date).toLocaleString('zh-TW');
};

onMounted(() => {
  loadProfile();
});
</script>
```

### 4.3 API 呼叫 (`user-profile.api.ts`)
```typescript
import request from '@/utils/request';

export interface UserProfileDto {
  userId: string;
  userName: string;
  title?: string;
  userType: string;
  userTypeName?: string;
  orgId?: string;
  orgName?: string;
  startDate?: string;
  endDate?: string;
  lastLoginDate?: string;
  lastLoginIp?: string;
  loginCount?: number;
  organizations: UserOrgDto[];
  permissions: any[];
  customFields: any[];
}

export interface UserOrgDto {
  orgId: string;
  orgName?: string;
  floorId?: string;
  areaId?: string;
  storeId?: string;
}

export interface UpdateUserProfileDto {
  title?: string;
  notes?: string;
}

// API 函數
export const getUserProfile = () => {
  return request.get<ApiResponse<UserProfileDto>>('/api/v1/users/profile');
};

export const updateUserProfile = (data: UpdateUserProfileDto) => {
  return request.put<ApiResponse<UserProfileDto>>('/api/v1/users/profile', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 使用者資料維護頁面開發
- [ ] 基本資料區塊開發
- [ ] 權限資訊區塊開發
- [ ] 登入資訊區塊開發
- [ ] 編輯功能開發
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 使用者只能查看和修改自己的資料
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查

### 6.2 業務邏輯
- 使用者只能修改允許的欄位（如職稱、備註）
- 使用者型態、組織等關鍵欄位不可由使用者自行修改
- 登入資訊為唯讀

### 6.3 資料顯示
- 組織權限中的 'xx' 表示全部，需顯示為「全部」
- 日期時間需格式化顯示
- 空值需顯示為 '-'

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢使用者資料成功
- [ ] 更新使用者資料成功
- [ ] 更新使用者資料失敗 (無權限欄位)

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 完整更新流程測試
- [ ] 權限檢查測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/USER_PROFILE.aspx.cs`
- `WEB/IMS_CORE/Kernel/USER_PROFILE.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

