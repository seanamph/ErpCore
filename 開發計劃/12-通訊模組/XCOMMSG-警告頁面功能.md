# XCOMMSG - 警告頁面功能 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOMMSG-Warning
- **功能名稱**: 警告頁面功能
- **功能描述**: 提供系統警告頁面功能，用於顯示警告訊息、確認操作等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOMMSG/Warning.ASP` (警告頁面)

### 1.2 業務需求
- 提供統一的警告頁面顯示
- 支援警告訊息的顯示
- 支援操作確認功能
- 提供友好的警告訊息顯示
- 支援警告頁面的多語言顯示
- 記錄警告到日誌系統

---

## 二、資料庫設計 (Schema)

### 2.1 使用現有資料表
- 使用 `ErrorMessages` 資料表記錄警告訊息（ErrorType = 'WARNING'）
- 使用 `ErrorMessageTemplates` 資料表管理警告訊息模板

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得警告頁面資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/warning-pages/{warningCode}`
- **說明**: 根據警告代碼取得警告頁面資訊
- **路徑參數**:
  - `warningCode`: 警告代碼
- **查詢參數**:
  - `language`: 語言代碼（預設: zh-TW）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "warningCode": "W001",
      "title": "操作警告",
      "message": "此操作將刪除所有相關資料，是否確定繼續？",
      "type": "CONFIRM", // CONFIRM, INFO, WARNING, ERROR
      "confirmText": "確定",
      "cancelText": "取消"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 警告頁面元件 (`WarningPage.vue`)
- **路徑**: 用於顯示警告頁面
- **功能**: 顯示警告訊息，支援確認/取消操作

### 4.2 UI 元件設計

#### 4.2.1 警告頁面元件 (`WarningPage.vue`)
```vue
<template>
  <div class="warning-page">
    <div class="warning-content">
      <div class="warning-icon">
        <el-icon :size="80" :color="iconColor">
          <component :is="warningIcon" />
        </el-icon>
      </div>
      <h2 class="warning-title">{{ warningTitle }}</h2>
      <p class="warning-message">{{ warningMessage }}</p>
      <div class="warning-actions">
        <el-button type="primary" @click="handleConfirm" v-if="showConfirm">
          {{ confirmText }}
        </el-button>
        <el-button @click="handleCancel" v-if="showCancel">
          {{ cancelText }}
        </el-button>
      </div>
    </div>
  </div>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (0.5天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (0.5天)
- [ ] 警告頁面元件開發
- [ ] 路由配置
- [ ] 元件測試

**總計**: 1天

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

