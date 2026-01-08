# RSL_BARCODE - 條碼處理工具 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: RSL_BARCODE
- **功能名稱**: 條碼處理工具
- **功能描述**: 提供條碼生成、讀取、驗證等功能，支援多種條碼格式（Code128、EAN-13、QR Code等）
- **參考舊程式**: 
  - `WEB/IMS_CORE/UTIL/RSL_BARCODE.dll` (條碼處理DLL)
  - 相關使用條碼功能的模組

### 1.2 業務需求
- 提供條碼生成功能
- 支援條碼讀取功能
- 支援條碼驗證功能
- 支援多種條碼格式
- 提供條碼圖片產生功能

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表
本功能為工具類，不涉及資料庫設計

### 2.2 相關資料表
無

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 產生條碼圖片
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/utils/barcode/generate`
- **說明**: 根據條碼內容產生條碼圖片
- **請求格式**:
  ```json
  {
    "content": "1234567890",
    "format": "Code128",
    "width": 200,
    "height": 100,
    "showText": true
  }
  ```
- **回應格式**: 條碼圖片（PNG格式，binary）

#### 3.1.2 驗證條碼格式
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/utils/barcode/validate`
- **說明**: 驗證條碼格式是否正確
- **請求格式**:
  ```json
  {
    "content": "1234567890",
    "format": "Code128"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "驗證成功",
    "data": {
      "isValid": true,
      "format": "Code128"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Service: `BarcodeService.cs`
```csharp
public interface IBarcodeService
{
    Task<byte[]> GenerateBarcodeAsync(BarcodeGenerateDto dto);
    Task<BarcodeValidationResult> ValidateBarcodeAsync(BarcodeValidateDto dto);
}
```

---

## 四、前端 UI 設計

### 4.1 工具類別設計

#### 4.1.1 條碼工具類 (`barcode.util.ts`)
```typescript
import { generateBarcode, validateBarcode } from '@/api/barcode.api';

export class BarcodeUtil {
  /**
   * 產生條碼圖片
   */
  static async generateBarcodeImage(
    content: string,
    format: string = 'Code128',
    width: number = 200,
    height: number = 100,
    showText: boolean = true
  ): Promise<string> {
    try {
      const response = await generateBarcode({
        content,
        format,
        width,
        height,
        showText
      });
      
      // 將binary轉換為base64
      const blob = new Blob([response.data], { type: 'image/png' });
      return URL.createObjectURL(blob);
    } catch (error) {
      console.error('產生條碼圖片失敗:', error);
      throw error;
    }
  }

  /**
   * 驗證條碼格式
   */
  static async validateBarcode(content: string, format: string): Promise<boolean> {
    try {
      const response = await validateBarcode({ content, format });
      return response.data.isValid;
    } catch (error) {
      console.error('驗證條碼失敗:', error);
      return false;
    }
  }
}
```

### 4.2 元件設計

#### 4.2.1 條碼顯示元件 (`BarcodeDisplay.vue`)
```vue
<template>
  <div class="barcode-display">
    <img :src="barcodeImageUrl" v-if="barcodeImageUrl" alt="條碼" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { BarcodeUtil } from '@/utils/barcode.util';

const props = defineProps<{
  content: string;
  format?: string;
  width?: number;
  height?: number;
  showText?: boolean;
}>();

const barcodeImageUrl = ref<string>('');

const generateBarcode = async () => {
  if (!props.content) return;
  
  try {
    barcodeImageUrl.value = await BarcodeUtil.generateBarcodeImage(
      props.content,
      props.format || 'Code128',
      props.width || 200,
      props.height || 100,
      props.showText ?? true
    );
  } catch (error) {
    console.error('產生條碼失敗:', error);
  }
};

watch(() => props.content, generateBarcode);
onMounted(generateBarcode);
</script>
```

### 4.3 API 呼叫 (`barcode.api.ts`)
```typescript
import request from '@/utils/request';
import type { ApiResponse } from '@/types/api';

export const generateBarcode = (params: any) => {
  return request.post('/api/v1/utils/barcode/generate', params, {
    responseType: 'blob'
  });
};

export const validateBarcode = (params: any) => {
  return request.post<ApiResponse<any>>('/api/v1/utils/barcode/validate', params);
};
```

---

## 五、開發時程

### 5.1 階段一: 技術選型 (1天)
- [ ] 選擇條碼處理套件（如 ZXing.NET）
- [ ] 確認支援的條碼格式
- [ ] 技術驗證

### 5.2 階段二: 後端開發 (2天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] 條碼產生邏輯
- [ ] 條碼驗證邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 條碼工具類別
- [ ] 條碼顯示元件
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 條碼產生測試
- [ ] 條碼驗證測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用說明文件

**總計**: 5.5天

---

## 六、注意事項

### 6.1 技術選型
- 後端: 使用 ZXing.NET 或類似套件
- 前端: 使用 jsbarcode 或類似套件（可選，用於前端直接產生）

### 6.2 效能
- 條碼圖片產生需考慮快取機制
- 大量條碼產生需使用批次處理

### 6.3 支援格式
- Code128
- EAN-13
- QR Code
- Code39
- 其他常用格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] 產生條碼圖片成功
- [ ] 驗證條碼格式成功
- [ ] 不同格式條碼產生測試

### 7.2 整合測試
- [ ] 完整條碼產生流程測試
- [ ] 條碼驗證流程測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/UTIL/RSL_BARCODE.dll`

### 8.2 相關功能
- 商品進銷碼維護作業 - 使用條碼功能
- 標籤列印作業 - 使用條碼功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

