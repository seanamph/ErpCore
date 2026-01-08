# XCOM1A0 - 系統環境資料設定作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM1A0
- **功能名稱**: 系統環境資料設定作業
- **功能描述**: 提供系統環境參數的新增、修改、查詢功能，包含主機IP、系統版本、系統名稱、客戶名稱抬頭、繁體中文版欄位可修改開放變數、版本出庫狀態、國碼預設值、區碼預設值、電話號碼預設值、分機號碼預設值、電話及傳真國碼文字方塊大小、電話及傳真區碼文字方塊大小、電話及傳真號碼文字方塊大小、分機號碼文字方塊大小、國碼最大可輸入字串長度、區碼最大可輸入字串長度、電話號碼最大可輸入字串長度、分機號碼最大可輸入字串長度、EXCEL安裝路徑、EXCEL開啟檔案路徑、小數點位數等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_PR.ASP` (報表)

### 1.2 業務需求
- 管理系統環境參數設定
- 支援主機IP設定
- 支援系統版本設定
- 支援系統名稱設定
- 支援客戶名稱抬頭設定
- 支援繁體中文版欄位可修改開放變數設定（Y:可開放修改;N:不可開放修改）
- 支援版本出庫狀態設定（如要出庫至客戶端請將此變數改成Y）
- 支援國碼、區碼、電話號碼、分機號碼預設值設定
- 支援電話及傳真相關文字方塊大小設定
- 支援電話及傳真相關最大可輸入字串長度設定
- 支援EXCEL安裝路徑和開啟檔案路徑設定
- 支援小數點位數設定
- 支援資料查詢與報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EnvironmentSetup` (系統環境設定，對應舊系統 `MNG_ENV_SETUP`)

```sql
CREATE TABLE [dbo].[EnvironmentSetup] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [SERVER_IP] NVARCHAR(16) NOT NULL, -- 主機IP
    [XCOMVERSION] NVARCHAR(16) NULL, -- 系統版本
    [PROJECTTITLE] NVARCHAR(30) NULL, -- 系統名稱
    [COMPANYTITLE] NVARCHAR(30) NULL, -- 客戶名稱抬頭
    [TCSETUPYN] NVARCHAR(10) NULL DEFAULT 'Y', -- 繁體中文版欄位可修改開放變數 (Y:可開放修改;N:不可開放修改)
    [DELIVERYSTATUS] NVARCHAR(10) NULL DEFAULT 'N', -- 版本出庫狀態 (Y:出庫至客戶端;N:不出庫)
    [NATVALUE] NVARCHAR(10) NULL, -- 國碼預設值
    [AREAVALUE] NVARCHAR(10) NULL, -- 區碼預設值
    [TELVALUE] NVARCHAR(10) NULL, -- 電話號碼預設值
    [EXTVALUE] NVARCHAR(10) NULL, -- 分機號碼預設值
    [NATSIZE] NVARCHAR(10) NULL, -- 電話及傳真國碼文字方塊大小
    [AREASIZE] NVARCHAR(10) NULL, -- 電話及傳真區碼文字方塊大小
    [TELSIZE] NVARCHAR(10) NULL, -- 電話及傳真號碼文字方塊大小
    [EXTSIZE] NVARCHAR(10) NULL, -- 分機號碼文字方塊大小
    [NATMAXLEN] NVARCHAR(10) NULL, -- 國碼最大可輸入字串長度
    [AREAMAXLEN] NVARCHAR(10) NULL, -- 區碼最大可輸入字串長度
    [TELMAXLEN] NVARCHAR(10) NULL, -- 電話號碼最大可輸入字串長度
    [EXTMAXLEN] NVARCHAR(10) NULL, -- 分機號碼最大可輸入字串長度
    [EXCELPATH] NVARCHAR(80) NULL, -- EXCEL安裝路徑
    [OPENEXCELFILE] NVARCHAR(80) NULL, -- EXCEL開啟檔案路徑
    [DOT] NVARCHAR(1) NULL DEFAULT '2', -- 小數點位數
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_EnvironmentSetup] PRIMARY KEY CLUSTERED ([T_KEY] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EnvironmentSetup_SERVER_IP] ON [dbo].[EnvironmentSetup] ([SERVER_IP]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| SERVER_IP | NVARCHAR | 16 | NO | - | 主機IP | 必填 |
| XCOMVERSION | NVARCHAR | 16 | YES | - | 系統版本 | - |
| PROJECTTITLE | NVARCHAR | 30 | YES | - | 系統名稱 | - |
| COMPANYTITLE | NVARCHAR | 30 | YES | - | 客戶名稱抬頭 | - |
| TCSETUPYN | NVARCHAR | 10 | YES | 'Y' | 繁體中文版欄位可修改開放變數 | Y:可開放修改;N:不可開放修改 |
| DELIVERYSTATUS | NVARCHAR | 10 | YES | 'N' | 版本出庫狀態 | Y:出庫至客戶端;N:不出庫 |
| NATVALUE | NVARCHAR | 10 | YES | - | 國碼預設值 | - |
| AREAVALUE | NVARCHAR | 10 | YES | - | 區碼預設值 | - |
| TELVALUE | NVARCHAR | 10 | YES | - | 電話號碼預設值 | - |
| EXTVALUE | NVARCHAR | 10 | YES | - | 分機號碼預設值 | - |
| NATSIZE | NVARCHAR | 10 | YES | - | 電話及傳真國碼文字方塊大小 | - |
| AREASIZE | NVARCHAR | 10 | YES | - | 電話及傳真區碼文字方塊大小 | - |
| TELSIZE | NVARCHAR | 10 | YES | - | 電話及傳真號碼文字方塊大小 | - |
| EXTSIZE | NVARCHAR | 10 | YES | - | 分機號碼文字方塊大小 | - |
| NATMAXLEN | NVARCHAR | 10 | YES | - | 國碼最大可輸入字串長度 | - |
| AREAMAXLEN | NVARCHAR | 10 | YES | - | 區碼最大可輸入字串長度 | - |
| TELMAXLEN | NVARCHAR | 10 | YES | - | 電話號碼最大可輸入字串長度 | - |
| EXTMAXLEN | NVARCHAR | 10 | YES | - | 分機號碼最大可輸入字串長度 | - |
| EXCELPATH | NVARCHAR | 80 | YES | - | EXCEL安裝路徑 | - |
| OPENEXCELFILE | NVARCHAR | 80 | YES | - | EXCEL開啟檔案路徑 | - |
| DOT | NVARCHAR | 1 | YES | '2' | 小數點位數 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統環境設定
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a0/environment-setup`
- **說明**: 查詢系統環境設定資料（通常只有一筆）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "tKey": 1,
      "serverIp": "192.168.1.100",
      "xcomVersion": "1.0.0",
      "projectTitle": "IMS3系統",
      "companyTitle": "宏誌科技",
      "tcSetupYn": "Y",
      "deliveryStatus": "N",
      "natValue": "886",
      "areaValue": "02",
      "telValue": "",
      "extValue": "",
      "natSize": "5",
      "areaSize": "5",
      "telSize": "10",
      "extSize": "5",
      "natMaxLen": "5",
      "areaMaxLen": "5",
      "telMaxLen": "10",
      "extMaxLen": "5",
      "excelPath": "C:\\Program Files\\Microsoft Office\\Office16",
      "openExcelFile": "C:\\Temp",
      "dot": "2"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 新增系統環境設定
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom1a0/environment-setup`
- **說明**: 新增系統環境設定資料（通常只會有一筆）
- **請求格式**:
  ```json
  {
    "serverIp": "192.168.1.100",
    "xcomVersion": "1.0.0",
    "projectTitle": "IMS3系統",
    "companyTitle": "宏誌科技",
    "tcSetupYn": "Y",
    "deliveryStatus": "N",
    "natValue": "886",
    "areaValue": "02",
    "telValue": "",
    "extValue": "",
    "natSize": "5",
    "areaSize": "5",
    "telSize": "10",
    "extSize": "5",
    "natMaxLen": "5",
    "areaMaxLen": "5",
    "telMaxLen": "10",
    "extMaxLen": "5",
    "excelPath": "C:\\Program Files\\Microsoft Office\\Office16",
    "openExcelFile": "C:\\Temp",
    "dot": "2"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 修改系統環境設定
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom1a0/environment-setup/{tKey}`
- **說明**: 修改系統環境設定資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.4 查詢系統環境設定列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom1a0/environment-setup/list`
- **說明**: 查詢系統環境設定列表（支援分頁、排序、篩選）
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "T_KEY",
    "sortOrder": "ASC",
    "filters": {
      "serverIp": "",
      "projectTitle": "",
      "companyTitle": ""
    }
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "tKey": 1,
          "serverIp": "192.168.1.100",
          "projectTitle": "IMS3系統",
          "companyTitle": "宏誌科技"
        }
      ],
      "totalCount": 1,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `EnvironmentSetupController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom1a0/environment-setup")]
    [Authorize]
    public class EnvironmentSetupController : ControllerBase
    {
        private readonly IEnvironmentSetupService _service;
        
        public EnvironmentSetupController(IEnvironmentSetupService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<EnvironmentSetupDto>>> GetEnvironmentSetup()
        {
            var result = await _service.GetEnvironmentSetupAsync();
            return Ok(ApiResponse<EnvironmentSetupDto>.Success(result));
        }
        
        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<PagedResult<EnvironmentSetupDto>>>> GetEnvironmentSetupList([FromQuery] EnvironmentSetupQueryDto query)
        {
            var result = await _service.GetEnvironmentSetupListAsync(query);
            return Ok(ApiResponse<PagedResult<EnvironmentSetupDto>>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateEnvironmentSetup([FromBody] CreateEnvironmentSetupDto dto)
        {
            var tKey = await _service.CreateEnvironmentSetupAsync(dto);
            return Ok(ApiResponse<long>.Success(tKey));
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateEnvironmentSetup(long tKey, [FromBody] UpdateEnvironmentSetupDto dto)
        {
            await _service.UpdateEnvironmentSetupAsync(tKey, dto);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `EnvironmentSetupService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IEnvironmentSetupService
    {
        Task<EnvironmentSetupDto> GetEnvironmentSetupAsync();
        Task<PagedResult<EnvironmentSetupDto>> GetEnvironmentSetupListAsync(EnvironmentSetupQueryDto query);
        Task<long> CreateEnvironmentSetupAsync(CreateEnvironmentSetupDto dto);
        Task UpdateEnvironmentSetupAsync(long tKey, UpdateEnvironmentSetupDto dto);
    }
}
```

#### 3.2.3 Repository: `EnvironmentSetupRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IEnvironmentSetupRepository
    {
        Task<EnvironmentSetup> GetByIdAsync(long tKey);
        Task<EnvironmentSetup> GetFirstAsync();
        Task<PagedResult<EnvironmentSetup>> GetPagedAsync(EnvironmentSetupQuery query);
        Task<EnvironmentSetup> CreateAsync(EnvironmentSetup entity);
        Task<EnvironmentSetup> UpdateAsync(EnvironmentSetup entity);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 系統環境設定頁面 (`EnvironmentSetup.vue`)
- **路徑**: `/system/environment-setup`
- **功能**: 顯示系統環境設定，支援修改
- **主要元件**:
  - 設定表單 (EnvironmentSetupForm)
  - 查詢表單 (EnvironmentSetupSearchForm)
  - 資料表格 (EnvironmentSetupDataTable)

### 4.2 UI 元件設計

#### 4.2.1 設定表單元件 (`EnvironmentSetupForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="200px">
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="主機IP" prop="serverIp">
          <el-input v-model="form.serverIp" placeholder="請輸入主機IP" maxlength="16" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="系統版本" prop="xcomVersion">
          <el-input v-model="form.xcomVersion" placeholder="請輸入系統版本" maxlength="16" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="系統名稱" prop="projectTitle">
          <el-input v-model="form.projectTitle" placeholder="請輸入系統名稱" maxlength="30" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="客戶名稱抬頭" prop="companyTitle">
          <el-input v-model="form.companyTitle" placeholder="請輸入客戶名稱抬頭" maxlength="30" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="繁體中文版欄位可修改開放變數" prop="tcSetupYn">
          <el-select v-model="form.tcSetupYn" placeholder="請選擇">
            <el-option label="可開放修改" value="Y" />
            <el-option label="不可開放修改" value="N" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="版本出庫狀態" prop="deliveryStatus">
          <el-select v-model="form.deliveryStatus" placeholder="請選擇">
            <el-option label="出庫至客戶端" value="Y" />
            <el-option label="不出庫" value="N" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <!-- 其他欄位... -->
    <el-form-item>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

### 4.3 API 呼叫 (`environmentSetup.api.ts`)
```typescript
import request from '@/utils/request';

export interface EnvironmentSetupDto {
  tKey: number;
  serverIp: string;
  xcomVersion?: string;
  projectTitle?: string;
  companyTitle?: string;
  tcSetupYn?: string;
  deliveryStatus?: string;
  natValue?: string;
  areaValue?: string;
  telValue?: string;
  extValue?: string;
  natSize?: string;
  areaSize?: string;
  telSize?: string;
  extSize?: string;
  natMaxLen?: string;
  areaMaxLen?: string;
  telMaxLen?: string;
  extMaxLen?: string;
  excelPath?: string;
  openExcelFile?: string;
  dot?: string;
}

export interface CreateEnvironmentSetupDto extends Omit<EnvironmentSetupDto, 'tKey'> {}
export interface UpdateEnvironmentSetupDto extends CreateEnvironmentSetupDto {}

export interface EnvironmentSetupQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    serverIp?: string;
    projectTitle?: string;
    companyTitle?: string;
  };
}

// API 函數
export const getEnvironmentSetup = () => {
  return request.get<ApiResponse<EnvironmentSetupDto>>('/api/v1/xcom1a0/environment-setup');
};

export const getEnvironmentSetupList = (query: EnvironmentSetupQueryDto) => {
  return request.get<ApiResponse<PagedResult<EnvironmentSetupDto>>>('/api/v1/xcom1a0/environment-setup/list', { params: query });
};

export const createEnvironmentSetup = (data: CreateEnvironmentSetupDto) => {
  return request.post<ApiResponse<number>>('/api/v1/xcom1a0/environment-setup', data);
};

export const updateEnvironmentSetup = (tKey: number, data: UpdateEnvironmentSetupDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom1a0/environment-setup/${tKey}`, data);
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 設定頁面開發
- [ ] 表單元件開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 7天

---

## 六、注意事項

### 6.1 業務邏輯
- 系統環境設定通常只有一筆資料
- 主機IP必須符合IP格式驗證
- 小數點位數必須為0-9的數字
- EXCEL路徑必須驗證路徑有效性

### 6.2 資料驗證
- 主機IP必須必填
- 各欄位長度必須符合資料庫限制
- 選項欄位必須在允許範圍內

### 6.3 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增系統環境設定成功
- [ ] 修改系統環境設定成功
- [ ] 查詢系統環境設定成功
- [ ] IP格式驗證測試

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_FU.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_FQ.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM1A0_PR.ASP`

### 8.2 資料庫 Schema
- 對應舊系統 `MNG_ENV_SETUP` 資料表

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

