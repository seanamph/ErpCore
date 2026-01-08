# XCOM110 - 跑馬燈維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM110
- **功能名稱**: 跑馬燈維護作業
- **功能描述**: 提供系統登入頁面跑馬燈訊息的新增、修改功能，包含標題跑馬燈和抬頭跑馬燈訊息維護
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM110_FU.asp` (修改)

### 1.2 業務需求
- 管理系統登入頁面的跑馬燈訊息
- 支援標題跑馬燈訊息維護（最多60個中文字）
- 支援抬頭跑馬燈訊息維護（多行訊息）
- 訊息即時生效，無需重啟系統

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `XCOM_LOGINPARA`

```sql
CREATE TABLE [dbo].[XCOM_LOGINPARA] (
    [PARA_NAME] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [PARA_DATA] NVARCHAR(130) NULL,
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_XCOM_LOGINPARA] PRIMARY KEY CLUSTERED ([PARA_NAME] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_XCOM_LOGINPARA_PARA_NAME] ON [dbo].[XCOM_LOGINPARA] ([PARA_NAME]);
```

### 2.2 檔案儲存: `marquee.txt`

- **路徑**: `/XCOMMSG/marquee.txt`
- **格式**: 純文字檔案，每行一則訊息
- **說明**: 抬頭跑馬燈訊息儲存在檔案系統中，每行為一則訊息

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| PARA_NAME | NVARCHAR | 50 | NO | - | 參數名稱 | 主鍵，如 'TITLE1' |
| PARA_DATA | NVARCHAR | 130 | YES | - | 參數資料 | 標題跑馬燈訊息，最多60個中文字 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢跑馬燈設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom/marquee`
- **說明**: 查詢跑馬燈設定資訊
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "titleMarquee": "歡迎使用IMS3系統",
      "headerMarquee": [
        "五樓大特賣，來喔快來～",
        "十樓男裝特價六折半",
        "珍寶黑鮪魚一斤五十元"
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 更新標題跑馬燈
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom/marquee/title`
- **說明**: 更新標題跑馬燈訊息
- **請求格式**:
  ```json
  {
    "paraData": "歡迎使用IMS3系統"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "更新成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 更新抬頭跑馬燈
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom/marquee/header`
- **說明**: 更新抬頭跑馬燈訊息（多行）
- **請求格式**:
  ```json
  {
    "messages": [
      "五樓大特賣，來喔快來～",
      "十樓男裝特價六折半",
      "珍寶黑鮪魚一斤五十元"
    ]
  }
  ```
- **回應格式**: 同更新標題跑馬燈

### 3.2 後端實作類別

#### 3.2.1 Controller: `MarqueeController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom/marquee")]
    [Authorize]
    public class MarqueeController : ControllerBase
    {
        private readonly IMarqueeService _marqueeService;
        
        public MarqueeController(IMarqueeService marqueeService)
        {
            _marqueeService = marqueeService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<MarqueeDto>>> GetMarquee()
        {
            var result = await _marqueeService.GetMarqueeAsync();
            return Ok(ApiResponse<MarqueeDto>.Success(result));
        }
        
        [HttpPut("title")]
        public async Task<ActionResult<ApiResponse>> UpdateTitleMarquee([FromBody] UpdateTitleMarqueeDto dto)
        {
            await _marqueeService.UpdateTitleMarqueeAsync(dto.ParaData);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("header")]
        public async Task<ActionResult<ApiResponse>> UpdateHeaderMarquee([FromBody] UpdateHeaderMarqueeDto dto)
        {
            await _marqueeService.UpdateHeaderMarqueeAsync(dto.Messages);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `MarqueeService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IMarqueeService
    {
        Task<MarqueeDto> GetMarqueeAsync();
        Task UpdateTitleMarqueeAsync(string paraData);
        Task UpdateHeaderMarqueeAsync(List<string> messages);
    }
    
    public class MarqueeService : IMarqueeService
    {
        private readonly IMarqueeRepository _repository;
        private readonly IFileStorageService _fileStorage;
        
        public async Task<MarqueeDto> GetMarqueeAsync()
        {
            var para = await _repository.GetByParaNameAsync("TITLE1");
            var headerMessages = await _fileStorage.ReadMarqueeFileAsync();
            
            return new MarqueeDto
            {
                TitleMarquee = para?.ParaData ?? "",
                HeaderMarquee = headerMessages
            };
        }
        
        public async Task UpdateTitleMarqueeAsync(string paraData)
        {
            if (paraData.Length > 130)
                throw new BusinessException("標題跑馬燈訊息最多60個中文字");
            
            await _repository.UpdateParaAsync("TITLE1", paraData);
        }
        
        public async Task UpdateHeaderMarqueeAsync(List<string> messages)
        {
            await _fileStorage.WriteMarqueeFileAsync(messages);
        }
    }
}
```

#### 3.2.3 Repository: `MarqueeRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IMarqueeRepository
    {
        Task<XcomLoginPara> GetByParaNameAsync(string paraName);
        Task UpdateParaAsync(string paraName, string paraData);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 跑馬燈維護頁面 (`MarqueeMaintenance.vue`)
- **路徑**: `/system/xcom/marquee`
- **功能**: 顯示跑馬燈設定，支援修改標題和抬頭跑馬燈

### 4.2 UI 元件設計

#### 4.2.1 跑馬燈維護表單 (`MarqueeForm.vue`)
```vue
<template>
  <div class="marquee-maintenance">
    <el-card>
      <template #header>
        <span>跑馬燈維護作業</span>
      </template>
      
      <el-form :model="form" :rules="rules" ref="formRef" label-width="200px">
        <!-- 標題跑馬燈 -->
        <el-form-item label="標題跑馬燈訊息" prop="titleMarquee">
          <el-input
            v-model="form.titleMarquee"
            placeholder="請輸入跑馬燈訊息，最多60個中文字"
            maxlength="130"
            show-word-limit
          />
          <div class="form-hint">最多60個中文字</div>
        </el-form-item>
        
        <!-- 抬頭跑馬燈 -->
        <el-form-item label="抬頭跑馬燈訊息" prop="headerMarquee">
          <el-input
            v-model="form.headerMarquee"
            type="textarea"
            :rows="25"
            placeholder="請輸入抬頭跑馬燈訊息，一行為一則訊息"
            maxlength="2000"
            show-word-limit
          />
          <div class="form-hint">一行為一則訊息</div>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleSubmit" :loading="loading">儲存</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { getMarquee, updateTitleMarquee, updateHeaderMarquee } from '@/api/xcom.api';

const formRef = ref();
const loading = ref(false);

const form = reactive({
  titleMarquee: '',
  headerMarquee: ''
});

const rules = {
  titleMarquee: [
    { required: true, message: '請輸入標題跑馬燈訊息', trigger: 'blur' },
    { max: 130, message: '最多60個中文字', trigger: 'blur' }
  ]
};

const loadData = async () => {
  try {
    const response = await getMarquee();
    if (response.data.success) {
      form.titleMarquee = response.data.data.titleMarquee || '';
      form.headerMarquee = response.data.data.headerMarquee?.join('\n') || '';
    }
  } catch (error) {
    ElMessage.error('載入資料失敗');
  }
};

const handleSubmit = async () => {
  if (!formRef.value) return;
  
  await formRef.value.validate(async (valid: boolean) => {
    if (!valid) return;
    
    loading.value = true;
    try {
      // 更新標題跑馬燈
      await updateTitleMarquee({ paraData: form.titleMarquee });
      
      // 更新抬頭跑馬燈
      const messages = form.headerMarquee
        .split('\n')
        .map(line => line.trim())
        .filter(line => line.length > 0);
      await updateHeaderMarquee({ messages });
      
      ElMessage.success('儲存成功');
    } catch (error) {
      ElMessage.error('儲存失敗');
    } finally {
      loading.value = false;
    }
  });
};

const handleReset = () => {
  loadData();
};

onMounted(() => {
  loadData();
});
</script>
```

### 4.3 API 呼叫 (`xcom.api.ts`)
```typescript
import request from '@/utils/request';

export interface MarqueeDto {
  titleMarquee: string;
  headerMarquee: string[];
}

export interface UpdateTitleMarqueeDto {
  paraData: string;
}

export interface UpdateHeaderMarqueeDto {
  messages: string[];
}

// API 函數
export const getMarquee = () => {
  return request.get<ApiResponse<MarqueeDto>>('/api/v1/xcom/marquee');
};

export const updateTitleMarquee = (data: UpdateTitleMarqueeDto) => {
  return request.put<ApiResponse>('/api/v1/xcom/marquee/title', data);
};

export const updateHeaderMarquee = (data: UpdateHeaderMarqueeDto) => {
  return request.put<ApiResponse>('/api/v1/xcom/marquee/header', data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立檔案儲存目錄結構

### 5.2 階段二: 後端開發 (1.5天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 檔案儲存服務實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 維護頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 檔案儲存測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 4.5天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查（僅系統管理員可維護）
- 檔案儲存路徑必須安全
- 必須驗證輸入內容，防止XSS攻擊

### 6.2 效能
- 檔案讀寫使用非同步操作
- 考慮快取機制，減少檔案讀取次數

### 6.3 資料驗證
- 標題跑馬燈最多60個中文字（130字元）
- 抬頭跑馬燈每行訊息長度限制
- 必須驗證輸入內容格式

### 6.4 業務邏輯
- 更新後立即生效，無需重啟系統
- 檔案儲存失敗時必須回滾資料庫更新
- 必須記錄操作日誌

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢跑馬燈設定成功
- [ ] 更新標題跑馬燈成功
- [ ] 更新抬頭跑馬燈成功
- [ ] 更新標題跑馬燈失敗（超過長度限制）
- [ ] 檔案讀寫測試

### 7.2 整合測試
- [ ] 完整更新流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量訊息處理測試
- [ ] 並發更新測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM110_FU.asp`

### 8.2 資料庫 Schema
- `XCOM_LOGINPARA` 資料表
- `marquee.txt` 檔案格式

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

