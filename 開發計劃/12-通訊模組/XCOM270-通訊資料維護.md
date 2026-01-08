# XCOM270 - 通訊資料維護 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM270
- **功能名稱**: 通訊資料維護（樣式列表查詢）
- **功能描述**: 提供CSS樣式列表的查詢與瀏覽功能，用於顯示系統中所有可用的CSS樣式類別及其效果預覽
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM270_FQ.ASP` (查詢)

### 1.2 業務需求
- 查詢系統中所有CSS樣式類別
- 顯示樣式類別名稱與效果預覽
- 支援樣式列表排序
- 用於開發與維護參考

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表
此功能主要讀取CSS檔案，不需要額外的資料表設計。樣式資訊直接從CSS檔案中解析取得。

### 2.2 資料來源
- CSS檔案路徑: `/ims_test/style/style.css`
- 資料格式: CSS類別定義

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢樣式列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom270/styles`
- **說明**: 查詢系統中所有CSS樣式類別列表
- **請求參數**: 無
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "styles": [
        {
          "className": "style1",
          "fullDefinition": ".style1 { color: red; }",
          "preview": "樣式效果預覽"
        }
      ],
      "totalCount": 100
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 解析CSS檔案
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom270/parse-css`
- **說明**: 解析指定CSS檔案並返回樣式列表
- **請求格式**:
  ```json
  {
    "cssFilePath": "/ims_test/style/style.css"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom270Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom270")]
    [Authorize]
    public class XCom270Controller : ControllerBase
    {
        private readonly ICssStyleService _cssStyleService;
        
        public XCom270Controller(ICssStyleService cssStyleService)
        {
            _cssStyleService = cssStyleService;
        }
        
        [HttpGet("styles")]
        public async Task<ActionResult<ApiResponse<StyleListDto>>> GetStyles()
        {
            var result = await _cssStyleService.ParseCssFileAsync();
            return Ok(ApiResponse<StyleListDto>.Success(result));
        }
        
        [HttpPost("parse-css")]
        public async Task<ActionResult<ApiResponse<StyleListDto>>> ParseCssFile([FromBody] ParseCssDto dto)
        {
            var result = await _cssStyleService.ParseCssFileAsync(dto.CssFilePath);
            return Ok(ApiResponse<StyleListDto>.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `CssStyleService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface ICssStyleService
    {
        Task<StyleListDto> ParseCssFileAsync(string cssFilePath = null);
    }
    
    public class CssStyleService : ICssStyleService
    {
        public async Task<StyleListDto> ParseCssFileAsync(string cssFilePath = null)
        {
            // 實作CSS檔案解析邏輯
            // 讀取CSS檔案
            // 解析樣式類別
            // 返回樣式列表
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 樣式列表查詢頁面 (`StyleList.vue`)
- **路徑**: `/xcom/styles`
- **功能**: 顯示CSS樣式列表，支援查詢、排序
- **主要元件**:
  - 樣式列表表格 (StyleListTable)
  - 樣式預覽區 (StylePreview)

### 4.2 UI 元件設計

#### 4.2.1 樣式列表表格 (`StyleListTable.vue`)
```vue
<template>
  <div>
    <el-table :data="styleList" v-loading="loading">
      <el-table-column prop="className" label="Class Name" width="200" />
      <el-table-column prop="fullDefinition" label="Style 定義" width="300" />
      <el-table-column label="Style 效果" width="200">
        <template #default="{ row }">
          <span :class="row.className">{{ row.fullDefinition }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
```

### 4.3 API 呼叫 (`xcom270.api.ts`)
```typescript
import request from '@/utils/request';

export interface StyleDto {
  className: string;
  fullDefinition: string;
  preview?: string;
}

export interface StyleListDto {
  styles: StyleDto[];
  totalCount: number;
}

export interface ParseCssDto {
  cssFilePath: string;
}

// API 函數
export const getStyles = () => {
  return request.get<ApiResponse<StyleListDto>>('/api/v1/xcom270/styles');
};

export const parseCssFile = (data: ParseCssDto) => {
  return request.post<ApiResponse<StyleListDto>>('/api/v1/xcom270/parse-css', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (1.5天)
- [ ] CSS檔案解析服務實作
- [ ] Controller 實作
- [ ] Service 實作
- [ ] DTO 類別建立

### 5.2 階段二: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 樣式列表頁面開發
- [ ] 樣式預覽功能開發
- [ ] 表格元件開發

### 5.3 階段三: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 功能測試
- [ ] CSS檔案解析測試

**總計**: 3.5天

---

## 六、注意事項

### 6.1 安全性
- CSS檔案路徑必須驗證，防止路徑遍歷攻擊
- 必須實作權限檢查

### 6.2 效能
- CSS檔案解析結果可以快取
- 大量樣式時需要分頁顯示

### 6.3 業務邏輯
- CSS檔案必須存在才能解析
- 樣式類別名稱必須正確解析
- 支援多種CSS語法格式

---

## 七、測試案例

### 7.1 單元測試
- [ ] CSS檔案解析成功
- [ ] CSS檔案不存在處理
- [ ] 樣式類別解析正確性

### 7.2 整合測試
- [ ] API 端點測試
- [ ] 前端頁面顯示測試
- [ ] 樣式預覽功能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM270_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

