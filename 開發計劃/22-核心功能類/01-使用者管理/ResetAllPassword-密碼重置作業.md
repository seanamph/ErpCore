# ResetAllPassword - 密碼重置作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ResetAllPassword
- **功能名稱**: 密碼重置作業
- **功能描述**: 提供管理員批量或單一重置使用者密碼的功能，通常用於系統初始化或緊急情況
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/ResetAllPassword.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/ResetAllPassword.aspx.cs`
  - `WEB/IMS_CORE/Kernel/ResetAllPassword.aspx`

### 1.2 業務需求
- 管理員可批量重置所有使用者密碼
- 管理員可重置單一使用者密碼
- 重置後的密碼通常設為使用者編號（或預設值）
- 重置後需記錄操作日誌
- 僅限系統管理員使用

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Users`
參考 `SYS0110-使用者基本資料維護.md` 的 `Users` 資料表設計

### 2.2 相關資料表

#### 2.2.1 `PasswordResetLogs` - 密碼重置記錄
```sql
CREATE TABLE [dbo].[PasswordResetLogs] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [ResetBy] NVARCHAR(50) NOT NULL,
    [ResetType] NVARCHAR(20) NOT NULL, -- SINGLE:單一, BATCH:批量
    [ResetAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Notes] NVARCHAR(500) NULL,
    CONSTRAINT [FK_PasswordResetLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_PasswordResetLogs_UserId] ON [dbo].[PasswordResetLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_PasswordResetLogs_ResetBy] ON [dbo].[PasswordResetLogs] ([ResetBy]);
CREATE NONCLUSTERED INDEX [IX_PasswordResetLogs_ResetAt] ON [dbo].[PasswordResetLogs] ([ResetAt]);
```

### 2.3 資料字典

#### PasswordResetLogs 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者編號 | 外鍵至Users表 |
| ResetBy | NVARCHAR | 50 | NO | - | 重置操作者 | - |
| ResetType | NVARCHAR | 20 | NO | - | 重置類型 | SINGLE:單一, BATCH:批量 |
| ResetAt | DATETIME2 | - | NO | GETDATE() | 重置時間 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 重置單一使用者密碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users/{userId}/reset-password`
- **說明**: 重置指定使用者的密碼
- **認證**: 需要JWT Token，且需系統管理員權限
- **請求格式**:
  ```json
  {
    "newPassword": "U001",
    "notes": "系統初始化"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "密碼重置成功",
    "data": {
      "userId": "U001",
      "resetAt": "2024-01-01T10:00:00"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 批量重置密碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users/reset-password/batch`
- **說明**: 批量重置多個使用者的密碼
- **認證**: 需要JWT Token，且需系統管理員權限
- **請求格式**:
  ```json
  {
    "userIds": ["U001", "U002", "U003"],
    "defaultPassword": "DEFAULT123!",
    "useUserIdAsPassword": true,
    "notes": "批量重置"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "批量重置完成",
    "data": {
      "totalCount": 3,
      "successCount": 3,
      "failedCount": 0,
      "results": [
        {
          "userId": "U001",
          "success": true,
          "message": "重置成功"
        },
        {
          "userId": "U002",
          "success": true,
          "message": "重置成功"
        },
        {
          "userId": "U003",
          "success": true,
          "message": "重置成功"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 重置所有使用者密碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users/reset-password/all`
- **說明**: 重置所有使用者的密碼（危險操作）
- **認證**: 需要JWT Token，且需系統管理員權限
- **請求格式**:
  ```json
  {
    "useUserIdAsPassword": true,
    "defaultPassword": "",
    "notes": "系統初始化"
  }
  ```
- **回應格式**: 同批量重置

### 3.2 後端實作類別

#### 3.2.1 Controller: `PasswordResetController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize(Roles = "Administrator")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        
        public PasswordResetController(IPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }
        
        [HttpPost("{userId}/reset-password")]
        public async Task<ActionResult<ApiResponse<PasswordResetResult>>> ResetPassword(
            string userId,
            [FromBody] ResetPasswordDto dto)
        {
            var resetBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _passwordResetService.ResetPasswordAsync(
                userId,
                dto.NewPassword,
                resetBy,
                "SINGLE",
                dto.Notes
            );
            return Ok(ApiResponse<PasswordResetResult>.Success(result));
        }
        
        [HttpPost("reset-password/batch")]
        public async Task<ActionResult<ApiResponse<BatchPasswordResetResult>>> BatchResetPassword(
            [FromBody] BatchResetPasswordDto dto)
        {
            var resetBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _passwordResetService.BatchResetPasswordAsync(
                dto.UserIds,
                dto.UseUserIdAsPassword ? null : dto.DefaultPassword,
                dto.UseUserIdAsPassword,
                resetBy,
                dto.Notes
            );
            return Ok(ApiResponse<BatchPasswordResetResult>.Success(result));
        }
        
        [HttpPost("reset-password/all")]
        public async Task<ActionResult<ApiResponse<BatchPasswordResetResult>>> ResetAllPasswords(
            [FromBody] ResetAllPasswordsDto dto)
        {
            var resetBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _passwordResetService.ResetAllPasswordsAsync(
                dto.UseUserIdAsPassword ? null : dto.DefaultPassword,
                dto.UseUserIdAsPassword,
                resetBy,
                dto.Notes
            );
            return Ok(ApiResponse<BatchPasswordResetResult>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `PasswordResetService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IPasswordResetService
    {
        Task<PasswordResetResult> ResetPasswordAsync(
            string userId,
            string newPassword,
            string resetBy,
            string resetType,
            string notes);
        Task<BatchPasswordResetResult> BatchResetPasswordAsync(
            List<string> userIds,
            string defaultPassword,
            bool useUserIdAsPassword,
            string resetBy,
            string notes);
        Task<BatchPasswordResetResult> ResetAllPasswordsAsync(
            string defaultPassword,
            bool useUserIdAsPassword,
            string resetBy,
            string notes);
    }
    
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetLogRepository _passwordResetLogRepository;
        private readonly IPasswordHasher _passwordHasher;
        
        public async Task<PasswordResetResult> ResetPasswordAsync(
            string userId,
            string newPassword,
            string resetBy,
            string resetType,
            string notes)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"使用者 {userId} 不存在");
            }
            
            // 重置密碼
            user.UserPassword = _passwordHasher.HashPassword(newPassword);
            user.ChangePwdDate = DateTime.UtcNow;
            user.UpdatedBy = resetBy;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.UpdateAsync(user);
            
            // 記錄日誌
            await _passwordResetLogRepository.CreateAsync(new PasswordResetLog
            {
                UserId = userId,
                ResetBy = resetBy,
                ResetType = resetType,
                Notes = notes
            });
            
            return new PasswordResetResult
            {
                UserId = userId,
                ResetAt = DateTime.UtcNow
            };
        }
        
        public async Task<BatchPasswordResetResult> BatchResetPasswordAsync(
            List<string> userIds,
            string defaultPassword,
            bool useUserIdAsPassword,
            string resetBy,
            string notes)
        {
            var results = new List<PasswordResetItemResult>();
            int successCount = 0;
            int failedCount = 0;
            
            foreach (var userId in userIds)
            {
                try
                {
                    var newPassword = useUserIdAsPassword ? userId : defaultPassword;
                    await ResetPasswordAsync(userId, newPassword, resetBy, "BATCH", notes);
                    
                    results.Add(new PasswordResetItemResult
                    {
                        UserId = userId,
                        Success = true,
                        Message = "重置成功"
                    });
                    successCount++;
                }
                catch (Exception ex)
                {
                    results.Add(new PasswordResetItemResult
                    {
                        UserId = userId,
                        Success = false,
                        Message = ex.Message
                    });
                    failedCount++;
                }
            }
            
            return new BatchPasswordResetResult
            {
                TotalCount = userIds.Count,
                SuccessCount = successCount,
                FailedCount = failedCount,
                Results = results
            };
        }
        
        public async Task<BatchPasswordResetResult> ResetAllPasswordsAsync(
            string defaultPassword,
            bool useUserIdAsPassword,
            string resetBy,
            string notes)
        {
            var allUsers = await _userRepository.GetAllAsync();
            var userIds = allUsers.Select(u => u.UserId).ToList();
            
            return await BatchResetPasswordAsync(userIds, defaultPassword, useUserIdAsPassword, resetBy, notes);
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 密碼重置頁面 (`PasswordReset.vue`)
- **路徑**: `/admin/password-reset`
- **功能**: 提供管理員重置使用者密碼的介面
- **主要元件**:
  - 單一重置表單
  - 批量重置表單
  - 重置記錄查詢

### 4.2 UI 元件設計

#### 4.2.1 密碼重置頁面 (`PasswordReset.vue`)
```vue
<template>
  <div class="password-reset">
    <el-tabs v-model="activeTab">
      <el-tab-pane label="單一重置" name="single">
        <el-card>
          <el-form :model="singleForm" :rules="singleRules" ref="singleFormRef" label-width="120px">
            <el-form-item label="使用者編號" prop="userId">
              <el-input v-model="singleForm.userId" placeholder="請輸入使用者編號" />
            </el-form-item>
            <el-form-item label="新密碼" prop="newPassword">
              <el-input
                v-model="singleForm.newPassword"
                type="password"
                placeholder="請輸入新密碼"
                show-password
              />
            </el-form-item>
            <el-form-item label="備註" prop="notes">
              <el-input v-model="singleForm.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleSingleReset" :loading="loading">重置</el-button>
              <el-button @click="handleSingleReset">清除</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>
      
      <el-tab-pane label="批量重置" name="batch">
        <el-card>
          <el-form :model="batchForm" :rules="batchRules" ref="batchFormRef" label-width="150px">
            <el-form-item label="使用者編號列表" prop="userIds">
              <el-input
                v-model="batchForm.userIdsText"
                type="textarea"
                :rows="5"
                placeholder="請輸入使用者編號，每行一個"
              />
            </el-form-item>
            <el-form-item label="重置方式">
              <el-radio-group v-model="batchForm.useUserIdAsPassword">
                <el-radio :label="true">使用使用者編號作為密碼</el-radio>
                <el-radio :label="false">使用預設密碼</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item
              v-if="!batchForm.useUserIdAsPassword"
              label="預設密碼"
              prop="defaultPassword"
            >
              <el-input
                v-model="batchForm.defaultPassword"
                type="password"
                placeholder="請輸入預設密碼"
                show-password
              />
            </el-form-item>
            <el-form-item label="備註" prop="notes">
              <el-input v-model="batchForm.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleBatchReset" :loading="loading">批量重置</el-button>
              <el-button @click="handleBatchReset">清除</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>
      
      <el-tab-pane label="重置所有" name="all">
        <el-card>
          <el-alert
            title="警告"
            type="warning"
            :closable="false"
            class="mb-4"
          >
            此操作將重置所有使用者的密碼，請謹慎使用！
          </el-alert>
          
          <el-form :model="allForm" :rules="allRules" ref="allFormRef" label-width="150px">
            <el-form-item label="重置方式">
              <el-radio-group v-model="allForm.useUserIdAsPassword">
                <el-radio :label="true">使用使用者編號作為密碼</el-radio>
                <el-radio :label="false">使用預設密碼</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item
              v-if="!allForm.useUserIdAsPassword"
              label="預設密碼"
              prop="defaultPassword"
            >
              <el-input
                v-model="allForm.defaultPassword"
                type="password"
                placeholder="請輸入預設密碼"
                show-password
              />
            </el-form-item>
            <el-form-item label="備註" prop="notes">
              <el-input v-model="allForm.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
            </el-form-item>
            <el-form-item>
              <el-button type="danger" @click="handleResetAll" :loading="loading">重置所有</el-button>
              <el-button @click="handleResetAll">清除</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </el-tab-pane>
    </el-tabs>
    
    <!-- 結果顯示 -->
    <el-card v-if="resetResult" class="mt-4">
      <template #header>
        <div class="card-header">
          <span>重置結果</span>
        </div>
      </template>
      
      <el-descriptions :column="2" border>
        <el-descriptions-item label="總數">{{ resetResult.totalCount }}</el-descriptions-item>
        <el-descriptions-item label="成功數">
          <span class="text-success">{{ resetResult.successCount }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="失敗數">
          <span class="text-danger">{{ resetResult.failedCount }}</span>
        </el-descriptions-item>
      </el-descriptions>
      
      <el-table :data="resetResult.results" class="mt-4">
        <el-table-column prop="userId" label="使用者編號" width="120" />
        <el-table-column prop="success" label="狀態" width="80">
          <template #default="{ row }">
            <el-tag :type="row.success ? 'success' : 'danger'">
              {{ row.success ? '成功' : '失敗' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="message" label="訊息" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import {
  resetPassword,
  batchResetPassword,
  resetAllPasswords
} from '@/api/password-reset';
import type { BatchPasswordResetResult } from '@/api/password-reset';

const activeTab = ref('single');
const loading = ref(false);
const resetResult = ref<BatchPasswordResetResult | null>(null);

const singleFormRef = ref();
const batchFormRef = ref();
const allFormRef = ref();

const singleForm = reactive({
  userId: '',
  newPassword: '',
  notes: ''
});

const batchForm = reactive({
  userIdsText: '',
  useUserIdAsPassword: true,
  defaultPassword: '',
  notes: ''
});

const allForm = reactive({
  useUserIdAsPassword: true,
  defaultPassword: '',
  notes: ''
});

const singleRules = reactive({
  userId: [{ required: true, message: '請輸入使用者編號', trigger: 'blur' }],
  newPassword: [{ required: true, message: '請輸入新密碼', trigger: 'blur' }]
});

const batchRules = reactive({
  userIdsText: [{ required: true, message: '請輸入使用者編號列表', trigger: 'blur' }],
  defaultPassword: [
    {
      validator: (rule: any, value: string, callback: any) => {
        if (!batchForm.useUserIdAsPassword && !value) {
          callback(new Error('請輸入預設密碼'));
        } else {
          callback();
        }
      },
      trigger: 'blur'
    }
  ]
});

const allRules = reactive({
  defaultPassword: [
    {
      validator: (rule: any, value: string, callback: any) => {
        if (!allForm.useUserIdAsPassword && !value) {
          callback(new Error('請輸入預設密碼'));
        } else {
          callback();
        }
      },
      trigger: 'blur'
    }
  ]
});

const handleSingleReset = async () => {
  if (!singleFormRef.value) return;
  
  await singleFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    loading.value = true;
    try {
      await resetPassword(singleForm.userId, {
        newPassword: singleForm.newPassword,
        notes: singleForm.notes
      });
      
      ElMessage.success('密碼重置成功');
      singleForm.userId = '';
      singleForm.newPassword = '';
      singleForm.notes = '';
    } catch (error: any) {
      ElMessage.error(error.message || '密碼重置失敗');
    } finally {
      loading.value = false;
    }
  });
};

const handleBatchReset = async () => {
  if (!batchFormRef.value) return;
  
  await batchFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    const userIds = batchForm.userIdsText
      .split('\n')
      .map(id => id.trim())
      .filter(id => id);
    
    if (userIds.length === 0) {
      ElMessage.warning('請輸入至少一個使用者編號');
      return;
    }
    
    loading.value = true;
    try {
      const response = await batchResetPassword({
        userIds,
        useUserIdAsPassword: batchForm.useUserIdAsPassword,
        defaultPassword: batchForm.defaultPassword,
        notes: batchForm.notes
      });
      
      resetResult.value = response.data;
      ElMessage.success(`批量重置完成：成功 ${response.data.successCount}，失敗 ${response.data.failedCount}`);
    } catch (error: any) {
      ElMessage.error(error.message || '批量重置失敗');
    } finally {
      loading.value = false;
    }
  });
};

const handleResetAll = async () => {
  if (!allFormRef.value) return;
  
  await allFormRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    try {
      await ElMessageBox.confirm(
        '此操作將重置所有使用者的密碼，確定要繼續嗎？',
        '警告',
        {
          confirmButtonText: '確定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      );
    } catch {
      return;
    }
    
    loading.value = true;
    try {
      const response = await resetAllPasswords({
        useUserIdAsPassword: allForm.useUserIdAsPassword,
        defaultPassword: allForm.defaultPassword,
        notes: allForm.notes
      });
      
      resetResult.value = response.data;
      ElMessage.success(`重置完成：成功 ${response.data.successCount}，失敗 ${response.data.failedCount}`);
    } catch (error: any) {
      ElMessage.error(error.message || '重置失敗');
    } finally {
      loading.value = false;
    }
  });
};
</script>
```

### 4.3 API 呼叫 (`password-reset.api.ts`)
```typescript
import request from '@/utils/request';

export interface ResetPasswordDto {
  newPassword: string;
  notes?: string;
}

export interface BatchResetPasswordDto {
  userIds: string[];
  useUserIdAsPassword: boolean;
  defaultPassword?: string;
  notes?: string;
}

export interface ResetAllPasswordsDto {
  useUserIdAsPassword: boolean;
  defaultPassword?: string;
  notes?: string;
}

export interface PasswordResetResult {
  userId: string;
  resetAt: string;
}

export interface PasswordResetItemResult {
  userId: string;
  success: boolean;
  message: string;
}

export interface BatchPasswordResetResult {
  totalCount: number;
  successCount: number;
  failedCount: number;
  results: PasswordResetItemResult[];
}

// API 函數
export const resetPassword = (userId: string, data: ResetPasswordDto) => {
  return request.post<ApiResponse<PasswordResetResult>>(
    `/api/v1/users/${userId}/reset-password`,
    data
  );
};

export const batchResetPassword = (data: BatchResetPasswordDto) => {
  return request.post<ApiResponse<BatchPasswordResetResult>>(
    '/api/v1/users/reset-password/batch',
    data
  );
};

export const resetAllPasswords = (data: ResetAllPasswordsDto) => {
  return request.post<ApiResponse<BatchPasswordResetResult>>(
    '/api/v1/users/reset-password/all',
    data
  );
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立密碼重置記錄資料表
- [ ] 建立索引

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 權限檢查實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 密碼重置頁面開發
- [ ] 單一重置功能
- [ ] 批量重置功能
- [ ] 重置所有功能
- [ ] 結果顯示
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 權限測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 僅限系統管理員使用
- 必須記錄所有重置操作
- 重置操作需有確認機制
- 批量操作需有進度提示

### 6.2 業務邏輯
- 重置後密碼通常設為使用者編號
- 重置後需更新密碼變更日期
- 批量操作需處理部分失敗的情況

### 6.3 效能
- 批量操作需考慮效能
- 大量使用者重置時需使用異步處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 單一重置成功
- [ ] 批量重置成功
- [ ] 重置所有成功
- [ ] 權限檢查測試
- [ ] 部分失敗處理測試

### 7.2 整合測試
- [ ] 完整重置流程測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/ResetAllPassword.aspx.cs`
- `WEB/IMS_CORE/Kernel/ResetAllPassword.aspx`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

