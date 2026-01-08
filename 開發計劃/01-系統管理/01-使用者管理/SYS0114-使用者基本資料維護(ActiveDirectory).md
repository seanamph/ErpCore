# SYS0114 - 使用者基本資料維護(ActiveDirectory) 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYS0114
- **功能名稱**: 使用者基本資料維護(ActiveDirectory)
- **功能描述**: 提供使用者基本資料的新增、修改、刪除、查詢功能，支援Active Directory整合，並包含業種權限設定，支援多系統切換設定
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/SYS0000/SYS0114.ascx`
  - `IMS3/HANSHIN/IMS3/SYS0000/SYS0114.ascx.cs`

### 1.2 業務需求
- 管理系統使用者帳號資訊
- 支援Active Directory整合（可選）
- 支援使用者業種權限設定（大分類、中分類、小分類）
- 支援多系統切換設定（限制某些系統的修改權限）
- 支援使用者組織權限設定
- 支援使用者帳號的啟用/停用
- 記錄使用者登入資訊
- 支援密碼管理（若使用AD則不提供重設密碼）
- 支援使用者權限設定
- 支援多組織架構

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Users`
參考 `SYS0110-使用者基本資料維護.md` 的 `Users` 資料表設計，並新增以下欄位：

```sql
ALTER TABLE [dbo].[Users] ADD 
    [UseActiveDirectory] BIT NOT NULL DEFAULT 0, -- 是否使用Active Directory
    [AdDomain] NVARCHAR(100) NULL, -- AD網域
    [AdUserPrincipalName] NVARCHAR(255) NULL; -- AD使用者主體名稱

-- 索引
CREATE NONCLUSTERED INDEX [IX_Users_UseActiveDirectory] ON [dbo].[Users] ([UseActiveDirectory]);
CREATE NONCLUSTERED INDEX [IX_Users_AdUserPrincipalName] ON [dbo].[Users] ([AdUserPrincipalName]);
```

### 2.2 相關資料表

#### 2.2.1 `UserBusinessTypes` - 使用者業種權限
參考 `SYS0111-使用者基本資料維護(業種儲位).md` 的 `UserBusinessTypes` 資料表設計

#### 2.2.2 `UserOrganizations` - 使用者組織權限
```sql
CREATE TABLE [dbo].[UserOrganizations] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(50) NOT NULL,
    [OrgId] NVARCHAR(50) NOT NULL, -- 組織代號
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_UserOrganizations_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_UserId] ON [dbo].[UserOrganizations] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserOrganizations_OrgId] ON [dbo].[UserOrganizations] ([OrgId]);
```

### 2.3 資料字典

#### Users 資料表新增欄位

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| UseActiveDirectory | BIT | - | NO | 0 | 是否使用Active Directory | 1:是, 0:否 |
| AdDomain | NVARCHAR | 100 | YES | - | AD網域 | - |
| AdUserPrincipalName | NVARCHAR | 255 | YES | - | AD使用者主體名稱 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢使用者列表
參考 `SYS0110-使用者基本資料維護.md`

#### 3.1.2 查詢單筆使用者（含AD和業種資訊）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/users/{userId}/detail`
- **說明**: 根據使用者編號查詢單筆使用者資料，包含Active Directory資訊、業種、組織權限
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "userId": "U001",
      "userName": "張三",
      "useActiveDirectory": true,
      "adDomain": "example.com",
      "adUserPrincipalName": "zhang.san@example.com",
      "businessTypes": [...],
      "organizations": [...]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增使用者（含AD和業種設定）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users`
- **說明**: 新增使用者資料，包含Active Directory設定、業種、組織權限
- **請求格式**:
  ```json
  {
    "userId": "U001",
    "userName": "張三",
    "userPassword": "Password123!",
    "useActiveDirectory": false,
    "adDomain": null,
    "adUserPrincipalName": null,
    "businessTypes": [...],
    "organizations": [...]
  }
  ```

#### 3.1.4 修改使用者（含AD和業種設定）
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/users/{userId}`
- **說明**: 修改使用者資料，包含Active Directory設定、業種、組織權限
- **注意事項**: 若使用多系統切換設定，某些欄位可能為唯讀

#### 3.1.5 驗證Active Directory使用者
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/users/validate-ad-user`
- **說明**: 驗證Active Directory使用者是否存在
- **請求格式**:
  ```json
  {
    "adDomain": "example.com",
    "adUserPrincipalName": "zhang.san@example.com"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "驗證成功",
    "data": {
      "exists": true,
      "userName": "張三",
      "email": "zhang.san@example.com"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Service: `ActiveDirectoryService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IActiveDirectoryService
    {
        Task<AdUserInfo> ValidateUserAsync(string domain, string userPrincipalName);
        Task<bool> AuthenticateAsync(string domain, string userPrincipalName, string password);
        Task<AdUserInfo> GetUserInfoAsync(string domain, string userPrincipalName);
    }
    
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        // 實作Active Directory整合邏輯
    }
}
```

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 新增/修改對話框 (`UserDialog.vue`)
參考 `SYS0111-使用者基本資料維護(業種儲位).md`，並新增Active Directory設定區塊：

```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1200px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <!-- 基本資料區塊 -->
      <!-- 參考 SYS0110 -->
      
      <!-- Active Directory設定區塊 -->
      <el-divider>Active Directory設定</el-divider>
      <el-form-item label="使用Active Directory">
        <el-switch v-model="form.useActiveDirectory" @change="handleAdChange" />
      </el-form-item>
      <template v-if="form.useActiveDirectory">
        <el-form-item label="AD網域" prop="adDomain">
          <el-input v-model="form.adDomain" placeholder="請輸入AD網域" />
        </el-form-item>
        <el-form-item label="AD使用者主體名稱" prop="adUserPrincipalName">
          <el-input v-model="form.adUserPrincipalName" placeholder="請輸入AD使用者主體名稱" />
          <el-button type="primary" size="small" @click="validateAdUser" style="margin-left: 10px;">驗證</el-button>
        </el-form-item>
      </template>
      
      <!-- 業種權限設定區塊 -->
      <!-- 參考 SYS0111 -->
      
      <!-- 組織權限設定區塊 -->
      <el-divider>組織權限設定</el-divider>
      <el-table :data="form.organizations" border>
        <el-table-column type="selection" width="55" />
        <el-table-column label="組織" width="200">
          <template #default="{ row }">
            <el-select v-model="row.orgId" placeholder="請選擇">
              <el-option 
                v-for="item in orgList" 
                :key="item.orgId" 
                :label="item.orgName" 
                :value="item.orgId" 
              />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="removeOrganization($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-button type="primary" size="small" @click="addOrganization" style="margin-top: 10px;">新增組織</el-button>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
const handleAdChange = (value: boolean) => {
  if (value) {
    // 使用AD時，隱藏密碼欄位
    // 根據多系統切換設定，某些欄位可能為唯讀
  } else {
    // 不使用AD時，顯示密碼欄位
  }
};

const validateAdUser = async () => {
  // 驗證AD使用者
};
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1.5天)
- [ ] 修改 Users 資料表（新增AD欄位）
- [ ] 建立 UserOrganizations 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（含AD整合）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] Active Directory整合服務
- [ ] 驗證邏輯實作
- [ ] 多系統切換設定邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發（含AD和業種設定）
- [ ] AD使用者驗證功能
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 多系統切換設定處理
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] Active Directory整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件
- [ ] Active Directory設定文件

**總計**: 13.5天

---

## 六、注意事項

### 6.1 Active Directory整合
- 需配置AD連線設定（網域、伺服器位址等）
- 需實作AD使用者驗證邏輯
- 使用AD時，系統密碼欄位應隱藏或唯讀
- 使用AD時，不提供重設密碼功能
- AD使用者資訊可自動同步（使用者名稱、Email等）

### 6.2 多系統切換設定
- 根據系統設定，某些欄位可能為唯讀
- 需檢查 `SelSystem` 和 `SelSystem_Master` 設定
- 非主系統時，限制某些欄位的修改權限

### 6.3 業務邏輯
- 業種設定為階層式（大分類→中分類→小分類）
- 組織可多選
- 刪除使用者時需同步刪除相關的業種、組織權限

### 6.4 安全性
- AD連線需使用加密傳輸
- AD使用者資訊需安全儲存
- 需記錄AD相關操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增使用者（含AD設定）成功
- [ ] 修改使用者（含AD設定）成功
- [ ] 驗證AD使用者成功
- [ ] AD使用者驗證失敗處理
- [ ] 多系統切換設定測試

### 7.2 整合測試
- [ ] Active Directory整合測試
- [ ] 完整 CRUD 流程測試（含AD和業種）
- [ ] 多系統切換設定流程測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/SYS0000/SYS0114.ascx`
- `IMS3/HANSHIN/IMS3/SYS0000/SYS0114.ascx.cs`

### 8.2 相關功能
- `SYS0110-使用者基本資料維護.md` - 基本使用者管理功能
- `SYS0111-使用者基本資料維護(業種儲位).md` - 業種權限設定

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

