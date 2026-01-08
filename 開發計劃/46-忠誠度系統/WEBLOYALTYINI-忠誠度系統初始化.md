# WEBLOYALTYINI - 忠誠度系統初始化 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: WEBLOYALTYINI
- **功能名稱**: 忠誠度系統初始化
- **功能描述**: 提供忠誠度系統的初始化功能，包含系統參數設定、會員參數設定、點數規則設定、獎勵規則設定等初始化作業
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSLPS/WEBLOYALTYINI.ASP` (忠誠度系統初始化)

### 1.2 業務需求
- 初始化忠誠度系統參數
- 設定會員參數（MEMBER_MID, MEMBER_TID等）
- 設定POS伺服器IP（POS_SERVER_IP）
- 設定點數規則
- 設定獎勵規則
- 設定系統環境參數
- 驗證初始化設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `LoyaltySystemConfigs` (忠誠度系統設定)

```sql
CREATE TABLE [dbo].[LoyaltySystemConfigs] (
    [ConfigId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [ConfigName] NVARCHAR(100) NOT NULL,
    [ConfigValue] NVARCHAR(500) NULL,
    [ConfigType] NVARCHAR(20) NOT NULL, -- PARAM, RULE, ENV
    [Description] NVARCHAR(500) NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A', -- A:啟用, I:停用
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_LoyaltySystemConfigs_ConfigType] ON [dbo].[LoyaltySystemConfigs] ([ConfigType]);
CREATE NONCLUSTERED INDEX [IX_LoyaltySystemConfigs_Status] ON [dbo].[LoyaltySystemConfigs] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `LoyaltySystemInitLogs` - 忠誠度系統初始化記錄
```sql
CREATE TABLE [dbo].[LoyaltySystemInitLogs] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [InitId] NVARCHAR(50) NOT NULL, -- 初始化批次編號
    [InitStatus] NVARCHAR(20) NOT NULL, -- 初始化狀態 (SUCCESS, FAILED)
    [InitDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [InitMessage] NVARCHAR(1000) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [UQ_LoyaltySystemInitLogs_InitId] UNIQUE ([InitId])
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/loyalty-system/configs`
- **說明**: 查詢忠誠度系統設定列表
- **請求參數**: 
  ```json
  {
    "configType": "" // PARAM, RULE, ENV
  }
  ```
- **回應格式**: 標準列表回應格式

#### 3.1.2 查詢單筆系統設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/loyalty-system/configs/{configId}`
- **說明**: 查詢單筆系統設定

#### 3.1.3 新增系統設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/loyalty-system/configs`
- **說明**: 新增系統設定

#### 3.1.4 修改系統設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/loyalty-system/configs/{configId}`
- **說明**: 修改系統設定

#### 3.1.5 刪除系統設定
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/loyalty-system/configs/{configId}`
- **說明**: 刪除系統設定

#### 3.1.6 執行系統初始化
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/loyalty-system/initialize`
- **說明**: 執行忠誠度系統初始化作業
- **請求格式**:
  ```json
  {
    "memberMid": "1",
    "memberTid": "18_1",
    "posServerIp": "1",
    "configs": [
      {
        "configId": "MEMBER_MID",
        "configValue": "1"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "初始化成功",
    "data": {
      "initId": "INIT001",
      "initStatus": "SUCCESS"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 忠誠度系統初始化頁面 (`LoyaltySystemInit.vue`)
- **路徑**: `/loyalty-system/initialize`
- **功能**: 顯示系統設定、執行初始化、查看初始化記錄

### 4.2 UI 元件設計

#### 4.2.1 系統設定表單
```vue
<template>
  <el-form :model="initForm" :rules="rules" ref="formRef" label-width="150px">
    <el-form-item label="會員MID" prop="memberMid">
      <el-input v-model="initForm.memberMid" placeholder="請輸入會員MID" />
    </el-form-item>
    <el-form-item label="會員TID" prop="memberTid">
      <el-input v-model="initForm.memberTid" placeholder="請輸入會員TID" />
    </el-form-item>
    <el-form-item label="POS伺服器IP" prop="posServerIp">
      <el-input v-model="initForm.posServerIp" placeholder="請輸入POS伺服器IP" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleInitialize">執行初始化</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
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
- [ ] 初始化功能實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 初始化頁面開發
- [ ] 設定表單開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 初始化功能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 敏感資料必須加密傳輸 (HTTPS)
- 必須實作權限檢查
- 初始化操作必須記錄日誌

### 6.2 效能
- 初始化操作必須支援非同步處理
- 必須提供進度追蹤

### 6.3 資料驗證
- 必填欄位必須驗證
- 參數值必須驗證

### 6.4 業務邏輯
- 初始化前必須驗證系統狀態
- 初始化失敗必須回滾
- 初始化結果必須記錄

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增系統設定成功
- [ ] 修改系統設定成功
- [ ] 執行初始化成功
- [ ] 初始化失敗處理

### 7.2 整合測試
- [ ] 完整初始化流程測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSLPS/WEBLOYALTYINI.ASP`

### 8.2 相關功能
- LPS-忠誠度系統維護（忠誠度系統維護功能）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

