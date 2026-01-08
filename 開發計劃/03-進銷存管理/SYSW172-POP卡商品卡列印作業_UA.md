# SYSW172 - POP卡＆商品卡列印作業_UA 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW172
- **功能名稱**: POP卡＆商品卡列印作業_UA
- **功能描述**: 提供POP卡和商品卡的列印功能（UA版本），為SYSW170的UA變體版本，主要差異在於列印格式和設定選項
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP` (基礎列印功能)
  - `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx` (報表)
  - UA版本專屬報表格式模板

### 1.2 業務需求
- 支援POP卡列印（UA版本格式）
- 支援商品卡列印（UA版本格式）
- 支援多種條碼格式 (UPC, EAN, Code39等)
- 支援UA專屬列印格式
- 支援列印設定 (頁面大小、填補、除錯模式等)
- 支援品牌權限控制
- 支援Excel匯出
- UA版本特有的列印參數和格式設定

### 1.3 與SYSW170的差異
- 使用UA專屬的報表格式模板
- 列印參數設定可能有所不同
- 列印版面配置針對UA版本優化

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

本功能使用與SYSW170相同的資料表結構，但需要額外支援UA版本的列印設定：

#### 2.1.1 `Products` - 商品主檔
- 參考: `開發計劃/03-進銷存管理/SYSW170-POP卡商品卡列印作業.md`

#### 2.1.2 `PopPrintSettings` - POP列印設定（擴展）
- 參考: `開發計劃/03-進銷存管理/SYSW171-POP卡商品卡列印作業_AP.md`
- 版本標記: `Version = 'UA'`

#### 2.1.3 `PopPrintLogs` - POP列印記錄（擴展）
- 參考: `開發計劃/03-進銷存管理/SYSW171-POP卡商品卡列印作業_AP.md`
- 版本標記: `Version = 'UA'`

### 2.2 資料字典

#### PopPrintSettings 資料表（UA版本）
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Version | NVARCHAR | 20 | YES | 'UA' | 版本標記 | AP, UA, STANDARD |
| UaSpecificSettings | NVARCHAR(MAX) | - | YES | - | UA版本專屬設定 | JSON格式儲存 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢商品列表 (用於列印)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ua/products`
- **說明**: 查詢可列印的商品列表，支援多條件篩選（UA版本）
- **請求參數**: 同SYSW170
- **回應格式**: 同SYSW170

#### 3.1.2 取得列印資料 (UA版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ua/generate`
- **說明**: 根據選取的商品和UA列印格式產生列印資料
- **請求格式**:
  ```json
  {
    "goodsIds": ["G001", "G002", "G003"],
    "printType": "POP",
    "printFormat": "PR1_UA", // UA版本專屬格式
    "shopId": "SHOP001",
    "version": "UA",
    "options": {
      "includeBarcode": true,
      "includePrice": true,
      "includeNote": false,
      "uaSpecificOptions": {} // UA版本專屬選項
    }
  }
  ```

#### 3.1.3 列印POP卡 (UA版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ua/print`
- **說明**: 執行UA版本列印作業並記錄列印日誌
- **請求格式**: 同SYSW171，但 `version = 'UA'`

#### 3.1.4 取得列印設定 (UA版本)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ua/settings/{shopId}`
- **說明**: 取得指定分店的UA版本列印設定
- **回應格式**: 同SYSW171，但 `version = 'UA'`

#### 3.1.5 更新列印設定 (UA版本)
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/pop-print-ua/settings/{shopId}`
- **說明**: 更新指定分店的UA版本列印設定

#### 3.1.6 查詢列印記錄 (UA版本)
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/pop-print-ua/logs`
- **說明**: 查詢UA版本列印記錄
- **請求參數**: 同SYSW170，但會自動過濾 `version = 'UA'`

#### 3.1.7 匯出Excel (UA版本)
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/pop-print-ua/export-excel`
- **說明**: 匯出UA版本列印資料為Excel檔案

### 3.2 後端實作類別

#### 3.2.1 Controller: `PopPrintUaController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/pop-print-ua")]
    [Authorize]
    public class PopPrintUaController : ControllerBase
    {
        private readonly IPopPrintService _popPrintService;
        
        public PopPrintUaController(IPopPrintService popPrintService)
        {
            _popPrintService = popPrintService;
        }
        
        // 實作方式與PopPrintApController相同，但版本標記為UA
        // 參考: PopPrintApController.cs
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 POP列印查詢頁面 (UA版本) (`PopPrintUaQuery.vue`)
- **路徑**: `/inventory/pop-print-ua/query`
- **功能**: 查詢商品並選擇UA版本列印格式
- **主要元件**: 同SYSW171，但標記為UA版本

### 4.2 UI 元件設計

#### 4.2.1 列印設定對話框 (UA版本) (`PrintSettingUaDialog.vue`)
- 結構同SYSW171的AP版本對話框
- 標題改為「列印設定 (UA版本)」
- 列印格式選項改為UA版本專屬格式

### 4.3 API 呼叫 (`popPrintUa.api.ts`)
```typescript
import request from '@/utils/request';

// 使用與SYSW171相同的結構，但API路徑為 `/api/v1/pop-print-ua`
export const getPopPrintUaProducts = (query: PopPrintProductQueryDto) => {
  return request.get<ApiResponse<PagedResult<ProductDto>>>('/api/v1/pop-print-ua/products', { params: query });
};

export const generatePrintUaData = (data: GeneratePrintDataDto) => {
  data.version = 'UA';
  return request.post<ApiResponse<PopPrintDataDto>>('/api/v1/pop-print-ua/generate', data);
};

// 其他API函數類似，但路徑和版本標記不同
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構支援UA版本
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] 建立UA版本Controller
- [ ] 實作UA版本專屬列印邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] 建立UA版本API呼叫函數
- [ ] 建立UA版本查詢頁面
- [ ] 建立UA版本列印設定對話框
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 列印功能測試（UA版本）
- [ ] 端對端測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6天

---

## 六、注意事項

### 6.1 版本區分
- 必須明確標記版本為UA
- 列印記錄必須記錄版本資訊
- 列印設定必須支援版本區分

### 6.2 報表模板
- 使用UA專屬的報表模板
- 模板路徑: `/templates/pop-print/ua/PR{1-6}_UA.rpt`

### 6.3 向後相容
- 必須與SYSW170共用核心邏輯
- 差異僅在於報表模板和部分設定選項

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢商品列表成功（UA版本）
- [ ] 產生列印資料成功（UA版本）
- [ ] 列印成功（UA版本）
- [ ] 取得列印設定成功（UA版本）
- [ ] 更新列印設定成功（UA版本）

### 7.2 整合測試
- [ ] 完整列印流程測試（UA版本）
- [ ] 版本區分測試
- [ ] Excel匯出測試（UA版本）

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW170_PR.ASP`
- `WEB/IMS_CORE/SYSW000/SYSW170_PR.aspx`

### 8.2 相關開發計劃
- `開發計劃/03-進銷存管理/SYSW170-POP卡商品卡列印作業.md`
- `開發計劃/03-進銷存管理/SYSW171-POP卡商品卡列印作業_AP.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

