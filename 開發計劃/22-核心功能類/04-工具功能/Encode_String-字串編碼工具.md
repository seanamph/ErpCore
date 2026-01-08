# Encode_String - 字串編碼工具 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: Encode_String
- **功能名稱**: 字串編碼工具
- **功能描述**: 提供字串編碼/解碼工具，用於資料庫連線字串等敏感資訊的編碼處理
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/KERNEL/Encode_String.aspx`
  - `IMS3/HANSHIN/IMS3/KERNEL/Encode_String.aspx.cs`
  - `WEB/IMS_CORE/Kernel/Encode_String.aspx`

### 1.2 業務需求
- 提供字串編碼功能
- 提供字串解碼功能
- 支援多種編碼類型
- 用於敏感資訊處理

---

## 二、資料庫設計 (Schema)

### 2.1 說明
此功能為工具類功能，不需要額外的資料表設計，主要使用系統現有的編碼/解碼功能。

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 編碼字串
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/encode-string/encode`
- **說明**: 編碼字串
- **請求格式**:
  ```json
  {
    "text": "原始字串",
    "encodeType": "1" // 1: User ID, 2: Password
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "編碼成功",
    "data": {
      "encodedText": "編碼後的字串"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 解碼字串
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/kernel/encode-string/decode`
- **說明**: 解碼字串
- **請求格式**:
  ```json
  {
    "encodedText": "編碼後的字串",
    "encodeType": "1"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `EncodeStringController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/kernel/encode-string")]
    [Authorize]
    public class EncodeStringController : ControllerBase
    {
        private readonly IEncodeStringService _encodeStringService;
        
        public EncodeStringController(IEncodeStringService encodeStringService)
        {
            _encodeStringService = encodeStringService;
        }
        
        [HttpPost("encode")]
        public async Task<ActionResult<ApiResponse<EncodeResultDto>>> Encode([FromBody] EncodeRequestDto dto)
        {
            // 實作編碼邏輯
        }
        
        [HttpPost("decode")]
        public async Task<ActionResult<ApiResponse<DecodeResultDto>>> Decode([FromBody] DecodeRequestDto dto)
        {
            // 實作解碼邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 字串編碼工具頁面 (`EncodeString.vue`)
- **路徑**: `/kernel/encode-string`
- **功能**: 提供字串編碼/解碼工具介面

### 4.2 UI 元件設計

#### 4.2.1 字串編碼工具元件 (`EncodeString.vue`)
```vue
<template>
  <div class="encode-string">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>字串編碼工具</span>
        </div>
      </template>
      
      <el-tabs v-model="activeTab">
        <el-tab-pane label="編碼" name="encode">
          <el-form :model="encodeForm" label-width="150px">
            <el-form-item label="編碼類型">
              <el-select v-model="encodeForm.encodeType" placeholder="請選擇編碼類型">
                <el-option label="User ID" value="1" />
                <el-option label="Password" value="2" />
              </el-select>
            </el-form-item>
            <el-form-item label="原始字串">
              <el-input 
                v-model="encodeForm.text" 
                type="textarea" 
                :rows="3"
                placeholder="請輸入要編碼的字串"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleEncode">編碼</el-button>
              <el-button @click="handleClear">清除</el-button>
            </el-form-item>
            <el-form-item label="編碼結果" v-if="encodeResult">
              <el-input 
                v-model="encodeResult" 
                readonly
                type="textarea" 
                :rows="3"
              />
            </el-form-item>
          </el-form>
        </el-tab-pane>
        
        <el-tab-pane label="解碼" name="decode">
          <el-form :model="decodeForm" label-width="150px">
            <el-form-item label="編碼類型">
              <el-select v-model="decodeForm.encodeType" placeholder="請選擇編碼類型">
                <el-option label="User ID" value="1" />
                <el-option label="Password" value="2" />
              </el-select>
            </el-form-item>
            <el-form-item label="編碼字串">
              <el-input 
                v-model="decodeForm.encodedText" 
                type="textarea" 
                :rows="3"
                placeholder="請輸入要解碼的字串"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleDecode">解碼</el-button>
              <el-button @click="handleClear">清除</el-button>
            </el-form-item>
            <el-form-item label="解碼結果" v-if="decodeResult">
              <el-input 
                v-model="decodeResult" 
                readonly
                type="textarea" 
                :rows="3"
              />
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>
```

### 4.3 API 呼叫 (`encodeString.api.ts`)
```typescript
import request from '@/utils/request';

export interface EncodeRequestDto {
  text: string;
  encodeType: '1' | '2';
}

export interface DecodeRequestDto {
  encodedText: string;
  encodeType: '1' | '2';
}

export interface EncodeResultDto {
  encodedText: string;
}

export interface DecodeResultDto {
  decodedText: string;
}

// API 函數
export const encodeString = (data: EncodeRequestDto) => {
  return request.post<ApiResponse<EncodeResultDto>>('/api/v1/kernel/encode-string/encode', data);
};

export const decodeString = (data: DecodeRequestDto) => {
  return request.post<ApiResponse<DecodeResultDto>>('/api/v1/kernel/encode-string/decode', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (2天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 編碼/解碼邏輯實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 字串編碼工具頁面開發
- [ ] 編碼/解碼功能開發
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試

### 5.4 階段四: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須驗證輸入參數
- 敏感資訊必須加密傳輸

### 6.2 資料驗證
- 輸入字串必須驗證
- 編碼類型必須驗證

---

## 七、測試案例

### 7.1 單元測試
- [ ] 編碼字串成功
- [ ] 解碼字串成功
- [ ] 編碼/解碼一致性測試

### 7.2 整合測試
- [ ] 完整編碼/解碼流程測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/IMS3/KERNEL/Encode_String.aspx.cs`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

