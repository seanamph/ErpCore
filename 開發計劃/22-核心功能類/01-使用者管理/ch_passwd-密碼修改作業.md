# ch_passwd - 密碼修改作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ch_passwd
- **功能名稱**: 密碼修改作業
- **功能描述**: 提供使用者修改個人密碼的功能，包含密碼驗證規則檢查
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/ch_passwd.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/ch_passwd.aspx.cs`
  - `WEB/IMS_CORE/Kernel/ch_passwd.aspx`

### 1.2 業務需求
- 使用者可修改個人密碼
- 必須輸入舊密碼進行驗證
- 新密碼必須符合密碼規則（長度、大小寫、數字、特殊字元等）
- 新密碼必須與舊密碼不同（可設定）
- 密碼修改後需記錄變更日期
- 支援密碼規則動態配置

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Users`
參考 `SYS0110-使用者基本資料維護.md` 的 `Users` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `PasswordRules` - 密碼規則設定
```sql
CREATE TABLE [dbo].[PasswordRules] (
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [RuleName] NVARCHAR(50) NOT NULL,
    [PasswordLength] INT NOT NULL DEFAULT 8,
    [LowerCaseCnt] INT NOT NULL DEFAULT 0, -- -1:不可包含, 0:不限制, >0:至少包含數量
    [UpperCaseCnt] INT NOT NULL DEFAULT 0,
    [NumberCnt] INT NOT NULL DEFAULT 0,
    [SpecialCharCnt] INT NOT NULL DEFAULT 0,
    [AllowSamePwd] BIT NOT NULL DEFAULT 0, -- 是否允許與舊密碼相同
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 預設規則
INSERT INTO [dbo].[PasswordRules] ([RuleName], [PasswordLength], [LowerCaseCnt], [UpperCaseCnt], [NumberCnt], [SpecialCharCnt], [AllowSamePwd])
VALUES ('Default', 8, 1, 1, 1, 1, 0);
```

#### 2.2.2 `PasswordHistory` - 密碼變更歷史（可選）
```sql
CREATE TABLE [dbo].[PasswordHistory] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [ChangedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ChangedBy] NVARCHAR(50) NULL,
    CONSTRAINT [FK_PasswordHistory_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PasswordHistory_UserId] ON [dbo].[PasswordHistory] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_PasswordHistory_ChangedAt] ON [dbo].[PasswordHistory] ([ChangedAt]);
```

### 2.3 資料字典

#### PasswordRules 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | INT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| RuleName | NVARCHAR | 50 | NO | - | 規則名稱 | - |
| PasswordLength | INT | - | NO | 8 | 密碼長度 | - |
| LowerCaseCnt | INT | - | NO | 0 | 小寫字母數量 | -1:不可包含, 0:不限制, >0:至少包含 |
| UpperCaseCnt | INT | - | NO | 0 | 大寫字母數量 | -1:不可包含, 0:不限制, >0:至少包含 |
| NumberCnt | INT | - | NO | 0 | 數字數量 | -1:不可包含, 0:不限制, >0:至少包含 |
| SpecialCharCnt | INT | - | NO | 0 | 特殊字元數量 | -1:不可包含, 0:不限制, >0:至少包含 |
| AllowSamePwd | BIT | - | NO | 0 | 是否允許與舊密碼相同 | 1:允許, 0:不允許 |
| Status | NVARCHAR | 10 | NO | 'A' | 狀態 | A:啟用, I:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢密碼規則
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/users/password-rules`
- **說明**: 查詢當前系統的密碼規則設定
- **認證**: 需要JWT Token
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "passwordLength": 8,
      "lowerCaseCnt": 1,
      "upperCaseCnt": 1,
      "numberCnt": 1,
      "specialCharCnt": 1,
      "allowSamePwd": false,
      "rules": [
        "至少必須包含1個英文小寫",
        "至少必須包含1個英文大寫",
        "至少必須包含1個數字",
        "至少必須包含1個特殊字元",
        "密碼長度至少8碼",
        "新密碼不允許與舊密碼相同"
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 修改密碼
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/users/password`
- **說明**: 修改當前使用者的密碼
- **認證**: 需要JWT Token
- **請求格式**:
  ```json
  {
    "oldPassword": "OldPassword123!",
    "newPassword": "NewPassword123!"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "密碼修改成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```
- **錯誤回應**:
  ```json
  {
    "success": false,
    "code": 400,
    "message": "舊密碼錯誤",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 驗證密碼強度
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users/password/validate`
- **說明**: 驗證密碼是否符合規則（不實際修改密碼）
- **認證**: 需要JWT Token
- **請求格式**:
  ```json
  {
    "password": "NewPassword123!"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "密碼驗證成功",
    "data": {
      "isValid": true,
      "errors": []
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `PasswordController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users/password")]
    [Authorize]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;
        
        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }
        
        [HttpGet("rules")]
        public async Task<ActionResult<ApiResponse<PasswordRuleDto>>> GetPasswordRules()
        {
            var rules = await _passwordService.GetPasswordRulesAsync();
            return Ok(ApiResponse<PasswordRuleDto>.Success(rules));
        }
        
        [HttpPut]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            await _passwordService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);
            return Ok(ApiResponse.Success("密碼修改成功"));
        }
        
        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponse<PasswordValidationResult>>> ValidatePassword([FromBody] ValidatePasswordDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _passwordService.ValidatePasswordAsync(dto.Password, userId);
            return Ok(ApiResponse<PasswordValidationResult>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `PasswordService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPasswordService
    {
        Task<PasswordRuleDto> GetPasswordRulesAsync();
        Task ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<PasswordValidationResult> ValidatePasswordAsync(string password, string userId = null);
    }
    
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRuleRepository _passwordRuleRepository;
        private readonly IPasswordHasher _passwordHasher;
        
        public async Task<PasswordRuleDto> GetPasswordRulesAsync()
        {
            var rule = await _passwordRuleRepository.GetActiveRuleAsync();
            if (rule == null)
            {
                rule = await _passwordRuleRepository.GetDefaultRuleAsync();
            }
            
            var rules = new List<string>();
            if (rule.LowerCaseCnt > 0)
            {
                rules.Add($"至少必須包含{rule.LowerCaseCnt}個英文小寫");
            }
            else if (rule.LowerCaseCnt == -1)
            {
                rules.Add("不可包含英文小寫");
            }
            
            if (rule.UpperCaseCnt > 0)
            {
                rules.Add($"至少必須包含{rule.UpperCaseCnt}個英文大寫");
            }
            else if (rule.UpperCaseCnt == -1)
            {
                rules.Add("不可包含英文大寫");
            }
            
            if (rule.NumberCnt > 0)
            {
                rules.Add($"至少必須包含{rule.NumberCnt}個數字");
            }
            else if (rule.NumberCnt == -1)
            {
                rules.Add("不可包含數字");
            }
            
            if (rule.SpecialCharCnt > 0)
            {
                rules.Add($"至少必須包含{rule.SpecialCharCnt}個特殊字元");
            }
            else if (rule.SpecialCharCnt == -1)
            {
                rules.Add("不可包含特殊字元");
            }
            
            rules.Add($"密碼長度至少{rule.PasswordLength}碼");
            rules.Add(rule.AllowSamePwd ? "新密碼允許與舊密碼相同" : "新密碼不允許與舊密碼相同");
            
            return new PasswordRuleDto
            {
                PasswordLength = rule.PasswordLength,
                LowerCaseCnt = rule.LowerCaseCnt,
                UpperCaseCnt = rule.UpperCaseCnt,
                NumberCnt = rule.NumberCnt,
                SpecialCharCnt = rule.SpecialCharCnt,
                AllowSamePwd = rule.AllowSamePwd,
                Rules = rules
            };
        }
        
        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"使用者 {userId} 不存在");
            }
            
            // 驗證舊密碼
            if (!_passwordHasher.VerifyPassword(oldPassword, user.UserPassword))
            {
                throw new BusinessException("舊密碼錯誤");
            }
            
            // 驗證新密碼
            var validationResult = await ValidatePasswordAsync(newPassword, userId);
            if (!validationResult.IsValid)
            {
                throw new BusinessException(string.Join(", ", validationResult.Errors));
            }
            
            // 檢查是否與舊密碼相同
            if (!validationResult.AllowSamePwd && _passwordHasher.VerifyPassword(newPassword, user.UserPassword))
            {
                throw new BusinessException("新密碼不能與舊密碼相同");
            }
            
            // 更新密碼
            user.UserPassword = _passwordHasher.HashPassword(newPassword);
            user.ChangePwdDate = DateTime.UtcNow;
            user.UpdatedBy = userId;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.UpdateAsync(user);
        }
        
        public async Task<PasswordValidationResult> ValidatePasswordAsync(string password, string userId = null)
        {
            var rule = await _passwordRuleRepository.GetActiveRuleAsync();
            if (rule == null)
            {
                rule = await _passwordRuleRepository.GetDefaultRuleAsync();
            }
            
            var errors = new List<string>();
            
            // 檢查長度
            if (password.Length < rule.PasswordLength)
            {
                errors.Add($"密碼長度至少{rule.PasswordLength}碼");
            }
            
            // 檢查小寫字母
            if (rule.LowerCaseCnt > 0)
            {
                var lowerCaseCount = password.Count(c => char.IsLower(c));
                if (lowerCaseCount < rule.LowerCaseCnt)
                {
                    errors.Add($"至少必須包含{rule.LowerCaseCnt}個英文小寫");
                }
            }
            else if (rule.LowerCaseCnt == -1 && password.Any(c => char.IsLower(c)))
            {
                errors.Add("不可包含英文小寫");
            }
            
            // 檢查大寫字母
            if (rule.UpperCaseCnt > 0)
            {
                var upperCaseCount = password.Count(c => char.IsUpper(c));
                if (upperCaseCount < rule.UpperCaseCnt)
                {
                    errors.Add($"至少必須包含{rule.UpperCaseCnt}個英文大寫");
                }
            }
            else if (rule.UpperCaseCnt == -1 && password.Any(c => char.IsUpper(c)))
            {
                errors.Add("不可包含英文大寫");
            }
            
            // 檢查數字
            if (rule.NumberCnt > 0)
            {
                var numberCount = password.Count(c => char.IsDigit(c));
                if (numberCount < rule.NumberCnt)
                {
                    errors.Add($"至少必須包含{rule.NumberCnt}個數字");
                }
            }
            else if (rule.NumberCnt == -1 && password.Any(c => char.IsDigit(c)))
            {
                errors.Add("不可包含數字");
            }
            
            // 檢查特殊字元
            if (rule.SpecialCharCnt > 0)
            {
                var specialCharCount = password.Count(c => !char.IsLetterOrDigit(c));
                if (specialCharCount < rule.SpecialCharCnt)
                {
                    errors.Add($"至少必須包含{rule.SpecialCharCnt}個特殊字元");
                }
            }
            else if (rule.SpecialCharCnt == -1 && password.Any(c => !char.IsLetterOrDigit(c)))
            {
                errors.Add("不可包含特殊字元");
            }
            
            return new PasswordValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                AllowSamePwd = rule.AllowSamePwd
            };
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 密碼修改頁面 (`ChangePassword.vue`)
- **路徑**: `/user/change-password`
- **功能**: 提供使用者修改密碼的介面
- **主要元件**:
  - 密碼規則說明區塊
  - 舊密碼輸入欄位
  - 新密碼輸入欄位
  - 確認新密碼輸入欄位

### 4.2 UI 元件設計

#### 4.2.1 密碼修改頁面 (`ChangePassword.vue`)
```vue
<template>
  <div class="change-password">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>修改密碼</span>
        </div>
      </template>
      
      <el-alert
        v-if="passwordRules.rules && passwordRules.rules.length > 0"
        :title="'密碼驗證規則：'"
        type="info"
        :closable="false"
        class="mb-4"
      >
        <ul class="password-rules">
          <li v-for="(rule, index) in passwordRules.rules" :key="index">{{ rule }}</li>
        </ul>
      </el-alert>
      
      <el-form
        :model="form"
        :rules="rules"
        ref="formRef"
        label-width="120px"
        class="password-form"
      >
        <el-form-item label="使用者代碼">
          <el-input v-model="userInfo.userId" disabled />
        </el-form-item>
        
        <el-form-item label="使用者名稱">
          <el-input v-model="userInfo.userName" disabled />
        </el-form-item>
        
        <el-form-item label="舊密碼" prop="oldPassword">
          <el-input
            v-model="form.oldPassword"
            type="password"
            placeholder="請輸入舊密碼"
            show-password
            @blur="validateOldPassword"
          />
        </el-form-item>
        
        <el-form-item label="新密碼" prop="newPassword">
          <el-input
            v-model="form.newPassword"
            type="password"
            placeholder="請輸入新密碼"
            show-password
            @blur="validateNewPassword"
          />
          <div v-if="passwordStrength" class="password-strength">
            <el-progress
              :percentage="passwordStrength.percentage"
              :color="passwordStrength.color"
              :show-text="false"
            />
            <span :class="passwordStrength.textClass">{{ passwordStrength.text }}</span>
          </div>
        </el-form-item>
        
        <el-form-item label="確認新密碼" prop="confirmPassword">
          <el-input
            v-model="form.confirmPassword"
            type="password"
            placeholder="請再次輸入新密碼"
            show-password
          />
        </el-form-item>
      </el-form>
      
      <div class="text-right mt-4">
        <el-button @click="handleReset">清除</el-button>
        <el-button type="primary" @click="handleSubmit" :loading="loading">確定</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { getPasswordRules, changePassword, validatePassword } from '@/api/password';
import type { PasswordRuleDto } from '@/api/password';
import { useUserStore } from '@/stores/user';

const userStore = useUserStore();
const formRef = ref();
const loading = ref(false);

const userInfo = reactive({
  userId: userStore.userId,
  userName: userStore.userName
});

const form = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
});

const passwordRules = ref<PasswordRuleDto>({} as PasswordRuleDto);
const passwordStrength = ref<any>(null);

const validateConfirmPassword = (rule: any, value: string, callback: any) => {
  if (value !== form.newPassword) {
    callback(new Error('兩次輸入的密碼不一致'));
  } else {
    callback();
  }
};

const rules = reactive({
  oldPassword: [
    { required: true, message: '請輸入舊密碼', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '請輸入新密碼', trigger: 'blur' },
    { min: passwordRules.value.passwordLength || 8, message: `密碼長度至少${passwordRules.value.passwordLength || 8}碼`, trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '請確認新密碼', trigger: 'blur' },
    { validator: validateConfirmPassword, trigger: 'blur' }
  ]
});

const loadPasswordRules = async () => {
  try {
    const response = await getPasswordRules();
    passwordRules.value = response.data;
  } catch (error) {
    console.error('載入密碼規則失敗', error);
  }
};

const validateOldPassword = async () => {
  // 可選：即時驗證舊密碼
};

const validateNewPassword = async () => {
  if (!form.newPassword) return;
  
  try {
    const response = await validatePassword(form.newPassword);
    if (!response.data.isValid) {
      // 顯示錯誤訊息
    }
    
    // 計算密碼強度
    calculatePasswordStrength(form.newPassword);
  } catch (error) {
    console.error('驗證密碼失敗', error);
  }
};

const calculatePasswordStrength = (password: string) => {
  let strength = 0;
  let percentage = 0;
  let color = '#f56c6c';
  let text = '弱';
  let textClass = 'text-danger';
  
  if (password.length >= 8) strength++;
  if (/[a-z]/.test(password)) strength++;
  if (/[A-Z]/.test(password)) strength++;
  if (/[0-9]/.test(password)) strength++;
  if (/[^a-zA-Z0-9]/.test(password)) strength++;
  
  percentage = (strength / 5) * 100;
  
  if (strength >= 4) {
    color = '#67c23a';
    text = '強';
    textClass = 'text-success';
  } else if (strength >= 3) {
    color = '#e6a23c';
    text = '中';
    textClass = 'text-warning';
  }
  
  passwordStrength.value = {
    percentage,
    color,
    text,
    textClass
  };
};

const handleReset = () => {
  form.oldPassword = '';
  form.newPassword = '';
  form.confirmPassword = '';
  passwordStrength.value = null;
  formRef.value?.resetFields();
};

const handleSubmit = async () => {
  if (!formRef.value) return;
  
  await formRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    loading.value = true;
    try {
      await changePassword({
        oldPassword: form.oldPassword,
        newPassword: form.newPassword
      });
      
      ElMessage.success('密碼修改成功');
      handleReset();
    } catch (error: any) {
      ElMessage.error(error.message || '密碼修改失敗');
    } finally {
      loading.value = false;
    }
  });
};

onMounted(() => {
  loadPasswordRules();
});
</script>

<style scoped>
.password-rules {
  margin: 10px 0 0 0;
  padding-left: 20px;
}

.password-strength {
  margin-top: 10px;
}

.text-danger {
  color: #f56c6c;
}

.text-warning {
  color: #e6a23c;
}

.text-success {
  color: #67c23a;
}
</style>
```

### 4.3 API 呼叫 (`password.api.ts`)
```typescript
import request from '@/utils/request';

export interface PasswordRuleDto {
  passwordLength: number;
  lowerCaseCnt: number;
  upperCaseCnt: number;
  numberCnt: number;
  specialCharCnt: number;
  allowSamePwd: boolean;
  rules: string[];
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
}

export interface ValidatePasswordDto {
  password: string;
}

export interface PasswordValidationResult {
  isValid: boolean;
  errors: string[];
  allowSamePwd: boolean;
}

// API 函數
export const getPasswordRules = () => {
  return request.get<ApiResponse<PasswordRuleDto>>('/api/v1/users/password-rules');
};

export const changePassword = (data: ChangePasswordDto) => {
  return request.put<ApiResponse>('/api/v1/users/password', data);
};

export const validatePassword = (password: string) => {
  return request.post<ApiResponse<PasswordValidationResult>>('/api/v1/users/password/validate', { password });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立密碼規則資料表
- [ ] 建立密碼歷史資料表（可選）
- [ ] 建立索引

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（密碼驗證邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 密碼加密實作（BCrypt）
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 密碼修改頁面開發
- [ ] 密碼規則顯示
- [ ] 密碼強度顯示
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 安全性測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 密碼必須使用BCrypt加密儲存
- 舊密碼必須正確驗證
- 密碼傳輸必須使用HTTPS
- 密碼不可記錄在日誌中

### 6.2 密碼規則
- 密碼規則必須可配置
- 密碼驗證必須在前端和後端都實作
- 密碼強度提示可選

### 6.3 業務邏輯
- 新密碼必須符合所有規則
- 新密碼不能與舊密碼相同（除非規則允許）
- 密碼修改後需更新變更日期

---

## 七、測試案例

### 7.1 單元測試
- [ ] 密碼修改成功
- [ ] 舊密碼錯誤
- [ ] 新密碼不符合規則
- [ ] 新密碼與舊密碼相同（不允許）
- [ ] 密碼驗證邏輯測試

### 7.2 整合測試
- [ ] 完整密碼修改流程測試
- [ ] 密碼規則驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/ch_passwd.aspx.cs`
- `WEB/IMS_CORE/Kernel/ch_passwd.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

