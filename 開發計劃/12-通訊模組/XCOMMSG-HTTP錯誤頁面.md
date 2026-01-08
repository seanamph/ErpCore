# XCOMMSG - HTTP錯誤頁面 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOMMSG-HTTP
- **功能名稱**: HTTP錯誤頁面
- **功能描述**: 提供HTTP錯誤頁面功能，包含401未授權、404未找到、500內部伺服器錯誤等頁面
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOMMSG/401.asp` (HTTP 401錯誤)
  - `WEB/IMS_CORE/ASP/XCOMMSG/404.asp` (HTTP 404錯誤)
  - `WEB/IMS_CORE/ASP/XCOMMSG/500.asp` (HTTP 500錯誤)

### 1.2 業務需求
- 提供統一的HTTP錯誤頁面顯示
- 支援401未授權錯誤頁面
- 支援404未找到錯誤頁面
- 支援500內部伺服器錯誤頁面
- 提供友好的錯誤訊息顯示
- 支援錯誤頁面的多語言顯示
- 記錄錯誤到日誌系統

---

## 二、資料庫設計 (Schema)

### 2.1 使用現有資料表
- 使用 `ErrorMessages` 資料表記錄HTTP錯誤
- 使用 `ErrorMessageTemplates` 資料表管理錯誤訊息模板

### 2.2 相關資料表
參考: `開發計劃/12-通訊模組/XCOMMSG-錯誤訊息處理.md`

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得HTTP錯誤頁面資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/error-pages/{statusCode}`
- **說明**: 根據HTTP狀態碼取得錯誤頁面資訊
- **路徑參數**:
  - `statusCode`: HTTP狀態碼 (401, 404, 500等)
- **查詢參數**:
  - `language`: 語言代碼（預設: zh-TW）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "statusCode": 404,
      "title": "找不到頁面",
      "message": "抱歉，您要查找的頁面不存在",
      "description": "請檢查網址是否正確，或返回首頁",
      "suggestions": [
        "檢查網址是否正確",
        "返回首頁",
        "聯繫系統管理員"
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `ErrorPagesController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/error-pages")]
    public class ErrorPagesController : ControllerBase
    {
        private readonly IErrorPageService _errorPageService;
        
        public ErrorPagesController(IErrorPageService errorPageService)
        {
            _errorPageService = errorPageService;
        }
        
        [HttpGet("{statusCode}")]
        public async Task<ActionResult<ApiResponse<ErrorPageDto>>> GetErrorPage(int statusCode, [FromQuery] string language = "zh-TW")
        {
            // 實作查詢邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 HTTP錯誤頁面元件 (`HttpErrorPage.vue`)
- **路徑**: 用於顯示各種HTTP錯誤頁面
- **功能**: 顯示友好的HTTP錯誤訊息，包含錯誤代碼、錯誤訊息、返回按鈕等

### 4.2 UI 元件設計

#### 4.2.1 HTTP錯誤頁面元件 (`HttpErrorPage.vue`)
```vue
<template>
  <div class="http-error-page">
    <div class="error-content">
      <div class="error-icon">
        <el-icon :size="120">
          <component :is="errorIcon" />
        </el-icon>
      </div>
      <h1 class="error-code">{{ statusCode }}</h1>
      <h2 class="error-title">{{ errorTitle }}</h2>
      <p class="error-message">{{ errorMessage }}</p>
      <div class="error-suggestions" v-if="suggestions && suggestions.length > 0">
        <h3>建議操作：</h3>
        <ul>
          <li v-for="(suggestion, index) in suggestions" :key="index">{{ suggestion }}</li>
        </ul>
      </div>
      <div class="error-actions">
        <el-button type="primary" @click="goHome">返回首頁</el-button>
        <el-button @click="goBack">返回上一頁</el-button>
        <el-button @click="contactSupport" v-if="showContactSupport">聯繫支援</el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getErrorPage } from '@/api/errorMessage.api';

const route = useRoute();
const router = useRouter();

const statusCode = ref<number>(parseInt(route.params.statusCode as string) || 404);
const errorTitle = ref<string>('');
const errorMessage = ref<string>('');
const suggestions = ref<string[]>([]);
const showContactSupport = ref<boolean>(true);

const errorIcon = computed(() => {
  switch (statusCode.value) {
    case 401:
      return 'Lock';
    case 404:
      return 'Search';
    case 500:
      return 'Warning';
    default:
      return 'Warning';
  }
});

onMounted(async () => {
  try {
    const response = await getErrorPage(statusCode.value);
    if (response.data.success) {
      errorTitle.value = response.data.data.title;
      errorMessage.value = response.data.data.message;
      suggestions.value = response.data.data.suggestions || [];
    }
  } catch (error) {
    console.error('取得錯誤頁面資訊失敗:', error);
  }
});

const goHome = () => {
  router.push('/');
};

const goBack = () => {
  router.back();
};

const contactSupport = () => {
  // 聯繫支援邏輯
};
</script>
```

### 4.3 API 呼叫 (`errorMessage.api.ts`)
```typescript
export interface ErrorPageDto {
  statusCode: number;
  title: string;
  message: string;
  description?: string;
  suggestions?: string[];
}

export const getErrorPage = (statusCode: number, language: string = 'zh-TW') => {
  return request.get<ApiResponse<ErrorPageDto>>(`/api/v1/error-pages/${statusCode}`, {
    params: { language }
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (1天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 錯誤訊息模板管理
- [ ] 單元測試

### 5.2 階段二: 前端開發 (1天)
- [ ] HTTP錯誤頁面元件開發
- [ ] 路由配置
- [ ] 多語言支援
- [ ] 元件測試

### 5.3 階段三: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 錯誤頁面顯示測試
- [ ] 多語言測試

**總計**: 2.5天

---

## 六、注意事項

### 6.1 安全性
- 敏感錯誤訊息不應暴露給使用者
- 500錯誤不應顯示詳細的技術資訊給一般使用者

### 6.2 使用者體驗
- 提供友好的錯誤訊息
- 提供明確的操作建議
- 支援多語言顯示

### 6.3 業務邏輯
- 錯誤頁面應記錄到日誌系統
- 支援自訂錯誤頁面模板
- 與系統錯誤處理機制整合

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

