# URDEF_REMOVE_FUN - 使用者定義功能移除 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: URDEF_REMOVE_FUN
- **功能名稱**: 使用者定義功能移除
- **功能描述**: 提供移除使用者自訂功能的功能，可清除使用者的所有自訂功能設定
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/URDEF_REMOVE_FUN.ASP` (功能移除主程式)

### 1.2 業務需求
- 移除使用者的所有自訂功能設定
- 支援單一使用者功能移除
- 支援特定系統的功能移除
- 操作後自動導向系統列表頁面

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `UserDefinedFunctions` (使用者定義功能，對應舊系統 `XCOM_USER_PROFILE1`)

```sql
CREATE TABLE [dbo].[UserDefinedFunctions] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL, -- 使用者代碼 (USER_ID)
    [ProgId] NVARCHAR(50) NOT NULL, -- 作業代碼 (PROG_ID)
    [SysId] NVARCHAR(50) NULL, -- 系統代碼 (SYS_ID)
    [MenuId] NVARCHAR(50) NULL, -- 選單代碼 (MENU_ID)
    [SeqNo] INT NULL, -- 排序序號 (SEQ_NO)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_UserDefinedFunctions_UserId_ProgId] UNIQUE ([UserId], [ProgId]),
    CONSTRAINT [FK_UserDefinedFunctions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserDefinedFunctions_UserId] ON [dbo].[UserDefinedFunctions] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserDefinedFunctions_ProgId] ON [dbo].[UserDefinedFunctions] ([ProgId]);
CREATE NONCLUSTERED INDEX [IX_UserDefinedFunctions_SysId] ON [dbo].[UserDefinedFunctions] ([SysId]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| UserId | NVARCHAR | 50 | NO | - | 使用者代碼 | 外鍵至Users |
| ProgId | NVARCHAR | 50 | NO | - | 作業代碼 | - |
| SysId | NVARCHAR | 50 | YES | - | 系統代碼 | - |
| MenuId | NVARCHAR | 50 | YES | - | 選單代碼 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 移除使用者所有自訂功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/urdef/functions`
- **說明**: 移除當前使用者的所有自訂功能設定
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "移除成功",
    "data": {
      "removedCount": 10
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 移除使用者特定系統的自訂功能
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/urdef/functions/system/{sysId}`
- **說明**: 移除當前使用者在特定系統的自訂功能設定
- **路徑參數**:
  - `sysId`: 系統代碼
- **回應格式**: 標準回應格式

#### 3.1.3 移除特定使用者的所有自訂功能（管理員功能）
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/urdef/functions/user/{userId}`
- **說明**: 移除特定使用者的所有自訂功能設定（需管理員權限）
- **路徑參數**:
  - `userId`: 使用者代碼
- **回應格式**: 標準回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `UrdefRemoveFunController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/urdef/functions")]
    [Authorize]
    public class UrdefRemoveFunController : ControllerBase
    {
        private readonly IUserDefinedFunctionService _userDefinedFunctionService;
        
        public UrdefRemoveFunController(IUserDefinedFunctionService userDefinedFunctionService)
        {
            _userDefinedFunctionService = userDefinedFunctionService;
        }
        
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<RemoveResultDto>>> RemoveAllFunctions()
        {
            var userId = User.Identity.Name;
            var result = await _userDefinedFunctionService.RemoveAllFunctionsAsync(userId);
            return Ok(new ApiResponse<RemoveResultDto>
            {
                Success = true,
                Code = 200,
                Message = "移除成功",
                Data = result
            });
        }
        
        [HttpDelete("system/{sysId}")]
        public async Task<ActionResult<ApiResponse<RemoveResultDto>>> RemoveFunctionsBySystem(string sysId)
        {
            var userId = User.Identity.Name;
            var result = await _userDefinedFunctionService.RemoveFunctionsBySystemAsync(userId, sysId);
            return Ok(new ApiResponse<RemoveResultDto>
            {
                Success = true,
                Code = 200,
                Message = "移除成功",
                Data = result
            });
        }
        
        [HttpDelete("user/{userId}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApiResponse<RemoveResultDto>>> RemoveUserFunctions(string userId)
        {
            var result = await _userDefinedFunctionService.RemoveAllFunctionsAsync(userId);
            return Ok(new ApiResponse<RemoveResultDto>
            {
                Success = true,
                Code = 200,
                Message = "移除成功",
                Data = result
            });
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 功能移除確認頁面 (`UrdefRemoveFun.vue`)
- **路徑**: `/urdef/remove-functions`
- **功能**: 顯示移除確認對話框，執行移除操作
- **主要元件**:
  - 確認對話框 (RemoveConfirmationDialog)
  - 移除結果提示

### 4.2 UI 元件設計

#### 4.2.1 移除確認對話框 (`RemoveConfirmationDialog.vue`)
```vue
<template>
  <el-dialog
    title="移除使用者定義功能"
    v-model="visible"
    width="500px"
    @close="handleClose"
  >
    <div>
      <el-alert
        title="警告"
        type="warning"
        :closable="false"
        show-icon
      >
        <template #default>
          <p>確定要移除所有自訂功能設定嗎？</p>
          <p>此操作無法復原。</p>
        </template>
      </el-alert>
    </div>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="danger" @click="handleConfirm" :loading="loading">確定移除</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import { removeAllFunctions } from '@/api/urdef.api';
import { useRouter } from 'vue-router';

const props = defineProps<{
  visible: boolean;
  sysId?: string;
}>();

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void;
  (e: 'removed'): void;
}>();

const router = useRouter();
const loading = ref(false);

const handleClose = () => {
  emit('update:visible', false);
};

const handleConfirm = async () => {
  loading.value = true;
  try {
    if (props.sysId) {
      await removeFunctionsBySystem(props.sysId);
    } else {
      await removeAllFunctions();
    }
    ElMessage.success('移除成功');
    emit('removed');
    emit('update:visible', false);
    // 導向系統列表頁面
    router.push('/urdef/sys-list');
  } catch (error) {
    ElMessage.error('移除失敗');
  } finally {
    loading.value = false;
  }
};
</script>
```

### 4.3 API 呼叫 (`urdef.api.ts`)
```typescript
import request from '@/utils/request';

export interface RemoveResultDto {
  removedCount: number;
}

// API 函數
export const removeAllFunctions = () => {
  return request.delete<ApiResponse<RemoveResultDto>>('/api/v1/urdef/functions');
};

export const removeFunctionsBySystem = (sysId: string) => {
  return request.delete<ApiResponse<RemoveResultDto>>(`/api/v1/urdef/functions/system/${sysId}`);
};

export const removeUserFunctions = (userId: string) => {
  return request.delete<ApiResponse<RemoveResultDto>>(`/api/v1/urdef/functions/user/${userId}`);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（使用現有的UserDefinedFunctions表）
- [ ] 確認索引

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 交易處理實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 移除確認對話框開發
- [ ] 移除結果提示
- [ ] 頁面導向功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 交易測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 4.5天

---

## 六、注意事項

### 6.1 業務邏輯
- 移除操作需使用資料庫交易，確保資料一致性
- 移除後需記錄操作日誌
- 移除成功後自動導向系統列表頁面
- 支援移除所有功能或特定系統的功能

### 6.2 資料驗證
- 使用者代碼必須存在
- 系統代碼必須存在（如果提供）

### 6.3 安全性
- 移除操作需驗證使用者權限
- 管理員可移除其他使用者的功能
- 一般使用者只能移除自己的功能

### 6.4 效能
- 大量資料移除需使用批次處理
- 必須使用資料庫交易確保一致性

---

## 七、測試案例

### 7.1 單元測試
- [ ] 移除所有功能成功
- [ ] 移除特定系統功能成功
- [ ] 移除不存在的功能（無錯誤）
- [ ] 移除失敗（交易回滾）

### 7.2 整合測試
- [ ] 完整移除流程測試
- [ ] 權限檢查測試
- [ ] 交易測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/URDEF_REMOVE_FUN.ASP` - 功能移除主程式

### 8.2 資料庫 Schema
- 舊系統資料表：`XCOM_USER_PROFILE1` (使用者定義功能)
- 主要欄位：USER_ID, PROG_ID, SYS_ID

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

