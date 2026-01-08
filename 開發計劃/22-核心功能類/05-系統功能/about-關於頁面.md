# about - 關於頁面 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: about
- **功能名稱**: 關於頁面
- **功能描述**: 顯示系統相關資訊，包含系統版本、開發公司資訊等
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/about.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/about.aspx.cs`
  - `WEB/IMS_CORE/Kernel/about.aspx`

### 1.2 業務需求
- 顯示系統版本資訊
- 顯示開發公司資訊
- 顯示系統說明
- 支援多語言顯示

---

## 二、資料庫設計 (Schema)

### 2.1 說明
此功能為靜態資訊顯示功能，不需要額外的資料表設計，系統資訊可儲存在配置檔或系統參數表中。

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 取得系統資訊
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/kernel/about`
- **說明**: 取得系統相關資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "systemName": "IMS3 系統",
      "version": "1.0.0",
      "developer": "宏誌科技股份有限公司",
      "description": "系統描述",
      "contact": {
        "phone": "02-8647-2545",
        "address": "台北縣汐止市大同路三段214號之一5樓",
        "website": "http://www.rsl.com.tw"
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `AboutController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/about")]
    public class AboutController : ControllerBase
    {
        private readonly IAboutService _aboutService;
        
        public AboutController(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<AboutDto>>> GetAbout()
        {
            // 實作取得系統資訊邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 關於頁面 (`About.vue`)
- **路徑**: `/kernel/about`
- **功能**: 顯示系統相關資訊

### 4.2 UI 元件設計

#### 4.2.1 關於頁面元件 (`About.vue`)
```vue
<template>
  <div class="about">
    <el-card>
      <div class="about-content">
        <div class="about-header">
          <img src="/images/about_03.jpg" alt="系統Logo" />
        </div>
        <div class="about-body">
          <h2>{{ aboutInfo.systemName }}</h2>
          <p><strong>版本:</strong> {{ aboutInfo.version }}</p>
          <p v-html="aboutInfo.description"></p>
          <div class="contact-info">
            <p><strong>開發公司:</strong> {{ aboutInfo.developer }}</p>
            <p><strong>電話:</strong> {{ aboutInfo.contact.phone }}</p>
            <p><strong>地址:</strong> {{ aboutInfo.contact.address }}</p>
            <p><strong>網站:</strong> 
              <a :href="aboutInfo.contact.website" target="_blank">
                {{ aboutInfo.contact.website }}
              </a>
            </p>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`about.api.ts`)
```typescript
import request from '@/utils/request';

export interface AboutDto {
  systemName: string;
  version: string;
  developer: string;
  description: string;
  contact: {
    phone: string;
    address: string;
    website: string;
  };
}

// API 函數
export const getAbout = () => {
  return request.get<ApiResponse<AboutDto>>('/api/v1/kernel/about');
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (1天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 系統資訊取得邏輯實作

### 5.2 階段二: 前端開發 (1天)
- [ ] API 呼叫函數
- [ ] 關於頁面開發
- [ ] 樣式設計

### 5.3 階段三: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試

### 5.4 階段四: 文件與部署 (0.5天)
- [ ] API 文件更新

**總計**: 3天

---

## 六、注意事項

### 6.1 多語言支援
- 必須支援多語言顯示
- 系統資訊必須可配置

---

## 七、測試案例

### 7.1 單元測試
- [ ] 取得系統資訊成功

### 7.2 整合測試
- [ ] 完整顯示流程測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/about.aspx.cs`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

