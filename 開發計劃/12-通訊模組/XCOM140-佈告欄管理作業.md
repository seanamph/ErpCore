# XCOM140 - 佈告欄管理作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM140
- **功能名稱**: 佈告欄管理作業
- **功能描述**: 提供系統佈告欄的新增、修改、刪除、查詢功能，包含公告時間、對象、內容等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM140_PR.asp` (報表)

### 1.2 業務需求
- 管理系統佈告欄公告
- 支援公告時間範圍設定（起始時間、結束時間）
- 支援公告對象設定（角色、使用者、組織）
- 支援公告類別設定
- 支援公告啟用/停用
- 支援公告內容維護（最多2000字元）
- 支援公告查詢與報表

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `BULLETIN`

```sql
CREATE TABLE [dbo].[BULLETIN] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [START_TIME] DATETIME2 NOT NULL,
    [END_TIME] DATETIME2 NOT NULL,
    [FORGROUP] NVARCHAR(50) NULL,
    [FORUSER] NVARCHAR(50) NULL,
    [FORORG] NVARCHAR(50) NULL,
    [ISONLINE] NVARCHAR(10) NOT NULL DEFAULT 'Y', -- Y:啟用, N:停用
    [BULLETIN_TYPE] NVARCHAR(10) NOT NULL,
    [NOTES] NVARCHAR(2000) NULL,
    [BUSER] NVARCHAR(50) NULL,
    [BTIME] DATETIME2 NULL,
    [CUSER] NVARCHAR(50) NOT NULL,
    [CTIME] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [CPRIORITY] INT NULL,
    [CGROUP] NVARCHAR(50) NULL,
    CONSTRAINT [PK_BULLETIN] PRIMARY KEY CLUSTERED ([T_KEY] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BULLETIN_START_TIME] ON [dbo].[BULLETIN] ([START_TIME]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_END_TIME] ON [dbo].[BULLETIN] ([END_TIME]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_ISONLINE] ON [dbo].[BULLETIN] ([ISONLINE]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_BULLETIN_TYPE] ON [dbo].[BULLETIN] ([BULLETIN_TYPE]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_FORGROUP] ON [dbo].[BULLETIN] ([FORGROUP]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_FORUSER] ON [dbo].[BULLETIN] ([FORUSER]);
CREATE NONCLUSTERED INDEX [IX_BULLETIN_FORORG] ON [dbo].[BULLETIN] ([FORORG]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY | 主鍵 | 自動遞增 |
| START_TIME | DATETIME2 | - | NO | - | 起始時間 | - |
| END_TIME | DATETIME2 | - | NO | - | 結束時間 | - |
| FORGROUP | NVARCHAR | 50 | YES | - | 公告角色 | 外鍵至角色表 |
| FORUSER | NVARCHAR | 50 | YES | - | 公告使用者 | 外鍵至使用者表 |
| FORORG | NVARCHAR | 50 | YES | - | 公告組織 | 外鍵至組織表 |
| ISONLINE | NVARCHAR | 10 | NO | 'Y' | 是否啟用 | Y:啟用, N:停用 |
| BULLETIN_TYPE | NVARCHAR | 10 | NO | - | 公告類別 | 參數表設定 |
| NOTES | NVARCHAR | 2000 | YES | - | 公告內容 | 最多2000字元 |
| BUSER | NVARCHAR | 50 | YES | - | 建立者 | - |
| BTIME | DATETIME2 | - | YES | - | 建立時間 | - |
| CUSER | NVARCHAR | 50 | NO | - | 建立者 | - |
| CTIME | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| CPRIORITY | INT | - | YES | - | 建立者等級 | - |
| CGROUP | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢佈告欄列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom/bulletin`
- **說明**: 查詢佈告欄列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "START_TIME",
    "sortOrder": "DESC",
    "filters": {
      "startTimeFrom": "",
      "startTimeTo": "",
      "endTimeFrom": "",
      "endTimeTo": "",
      "forgroup": "",
      "foruser": "",
      "fororg": "",
      "isonline": "",
      "bulletinType": ""
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
          "startTime": "2024-01-01T00:00:00",
          "endTime": "2024-12-31T23:59:59",
          "forgroup": "ROLE001",
          "forgroupName": "系統管理員",
          "foruser": null,
          "foruserName": null,
          "fororg": null,
          "fororgName": null,
          "isonline": "Y",
          "bulletinType": "INFO",
          "bulletinTypeName": "一般公告",
          "notes": "系統維護公告",
          "cuser": "U001",
          "cuserName": "張三",
          "ctime": "2024-01-01T10:00:00"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 20,
      "totalPages": 5
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆佈告欄
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom/bulletin/{tKey}`
- **說明**: 根據主鍵查詢單筆佈告欄資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料

#### 3.1.3 新增佈告欄
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom/bulletin`
- **說明**: 新增佈告欄資料
- **請求格式**:
  ```json
  {
    "startTime": "2024-01-01T00:00:00",
    "endTime": "2024-12-31T23:59:59",
    "forgroup": "ROLE001",
    "foruser": null,
    "fororg": null,
    "isonline": "Y",
    "bulletinType": "INFO",
    "notes": "系統維護公告"
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

#### 3.1.4 修改佈告欄
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom/bulletin/{tKey}`
- **說明**: 修改佈告欄資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **回應格式**: 同新增

#### 3.1.5 刪除佈告欄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom/bulletin/{tKey}`
- **說明**: 刪除佈告欄資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 批次刪除佈告欄
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom/bulletin/batch`
- **說明**: 批次刪除多筆佈告欄
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

#### 3.1.7 啟用/停用佈告欄
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom/bulletin/{tKey}/status`
- **說明**: 啟用或停用佈告欄
- **請求格式**:
  ```json
  {
    "isonline": "Y" // Y:啟用, N:停用
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `BulletinController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom/bulletin")]
    [Authorize]
    public class BulletinController : ControllerBase
    {
        private readonly IBulletinService _bulletinService;
        
        public BulletinController(IBulletinService bulletinService)
        {
            _bulletinService = bulletinService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<BulletinDto>>>> GetBulletins([FromQuery] BulletinQueryDto query)
        {
            var result = await _bulletinService.GetBulletinsAsync(query);
            return Ok(ApiResponse<PagedResult<BulletinDto>>.Success(result));
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<BulletinDto>>> GetBulletin(long tKey)
        {
            var result = await _bulletinService.GetBulletinByIdAsync(tKey);
            return Ok(ApiResponse<BulletinDto>.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<long>>> CreateBulletin([FromBody] CreateBulletinDto dto)
        {
            var tKey = await _bulletinService.CreateBulletinAsync(dto);
            return Ok(ApiResponse<long>.Success(tKey));
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateBulletin(long tKey, [FromBody] UpdateBulletinDto dto)
        {
            await _bulletinService.UpdateBulletinAsync(tKey, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteBulletin(long tKey)
        {
            await _bulletinService.DeleteBulletinAsync(tKey);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("batch")]
        public async Task<ActionResult<ApiResponse>> DeleteBulletins([FromBody] BatchDeleteDto dto)
        {
            await _bulletinService.DeleteBulletinsAsync(dto.TKeys);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPut("{tKey}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateStatus(long tKey, [FromBody] UpdateStatusDto dto)
        {
            await _bulletinService.UpdateStatusAsync(tKey, dto.Isonline);
            return Ok(ApiResponse.Success());
        }
    }
}
```

#### 3.2.2 Service: `BulletinService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IBulletinService
    {
        Task<PagedResult<BulletinDto>> GetBulletinsAsync(BulletinQueryDto query);
        Task<BulletinDto> GetBulletinByIdAsync(long tKey);
        Task<long> CreateBulletinAsync(CreateBulletinDto dto);
        Task UpdateBulletinAsync(long tKey, UpdateBulletinDto dto);
        Task DeleteBulletinAsync(long tKey);
        Task DeleteBulletinsAsync(List<long> tKeys);
        Task UpdateStatusAsync(long tKey, string isonline);
    }
}
```

#### 3.2.3 Repository: `BulletinRepository.cs`
```csharp
namespace RSL.IMS3.Infrastructure.Data.Repositories
{
    public interface IBulletinRepository
    {
        Task<Bulletin> GetByIdAsync(long tKey);
        Task<PagedResult<Bulletin>> GetPagedAsync(BulletinQuery query);
        Task<Bulletin> CreateAsync(Bulletin bulletin);
        Task<Bulletin> UpdateAsync(Bulletin bulletin);
        Task DeleteAsync(long tKey);
        Task DeleteBatchAsync(List<long> tKeys);
        Task<bool> ExistsAsync(long tKey);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 佈告欄列表頁面 (`BulletinList.vue`)
- **路徑**: `/system/xcom/bulletin`
- **功能**: 顯示佈告欄列表，支援查詢、新增、修改、刪除

#### 4.1.2 佈告欄詳細頁面 (`BulletinDetail.vue`)
- **路徑**: `/system/xcom/bulletin/:tKey`
- **功能**: 顯示佈告欄詳細資料，支援修改

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`BulletinSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="起始時間">
      <el-date-picker
        v-model="searchForm.startTimeFrom"
        type="datetime"
        placeholder="從"
        format="YYYY/MM/DD HH:mm:ss"
      />
      <span style="margin: 0 8px">至</span>
      <el-date-picker
        v-model="searchForm.startTimeTo"
        type="datetime"
        placeholder="到"
        format="YYYY/MM/DD HH:mm:ss"
      />
    </el-form-item>
    <el-form-item label="結束時間">
      <el-date-picker
        v-model="searchForm.endTimeFrom"
        type="datetime"
        placeholder="從"
        format="YYYY/MM/DD HH:mm:ss"
      />
      <span style="margin: 0 8px">至</span>
      <el-date-picker
        v-model="searchForm.endTimeTo"
        type="datetime"
        placeholder="到"
        format="YYYY/MM/DD HH:mm:ss"
      />
    </el-form-item>
    <el-form-item label="公告角色">
      <el-select v-model="searchForm.forgroup" placeholder="請選擇角色" clearable>
        <el-option v-for="role in roleList" :key="role.roleId" :label="role.roleName" :value="role.roleId" />
      </el-select>
    </el-form-item>
    <el-form-item label="是否啟用">
      <el-select v-model="searchForm.isonline" placeholder="請選擇" clearable>
        <el-option label="啟用" value="Y" />
        <el-option label="停用" value="N" />
      </el-select>
    </el-form-item>
    <el-form-item label="公告類別">
      <el-select v-model="searchForm.bulletinType" placeholder="請選擇類別" clearable>
        <el-option v-for="type in bulletinTypeList" :key="type.tag" :label="type.content" :value="type.tag" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`BulletinDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="bulletinList" v-loading="loading" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="tKey" label="編號" width="80" />
      <el-table-column prop="startTime" label="起始時間" width="160" />
      <el-table-column prop="endTime" label="結束時間" width="160" />
      <el-table-column prop="forgroupName" label="公告角色" width="120" />
      <el-table-column prop="foruserName" label="公告使用者" width="120" />
      <el-table-column prop="fororgName" label="公告組織" width="120" />
      <el-table-column prop="isonline" label="狀態" width="80">
        <template #default="{ row }">
          <el-tag :type="row.isonline === 'Y' ? 'success' : 'danger'">
            {{ row.isonline === 'Y' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="bulletinTypeName" label="公告類別" width="100" />
      <el-table-column prop="notes" label="公告內容" show-overflow-tooltip />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
      v-model:current-page="pagination.pageIndex"
      v-model:page-size="pagination.pageSize"
      :total="pagination.totalCount"
      :page-sizes="[10, 20, 50, 100]"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
</template>
```

#### 4.2.3 新增/修改對話框 (`BulletinDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="800px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <el-form-item label="起始時間" prop="startTime">
        <el-date-picker
          v-model="form.startTime"
          type="datetime"
          placeholder="請選擇起始時間"
          format="YYYY/MM/DD HH:mm:ss"
        />
      </el-form-item>
      <el-form-item label="結束時間" prop="endTime">
        <el-date-picker
          v-model="form.endTime"
          type="datetime"
          placeholder="請選擇結束時間"
          format="YYYY/MM/DD HH:mm:ss"
        />
      </el-form-item>
      <el-form-item label="公告角色" prop="forgroup">
        <el-select v-model="form.forgroup" placeholder="請選擇角色" clearable>
          <el-option v-for="role in roleList" :key="role.roleId" :label="role.roleName" :value="role.roleId" />
        </el-select>
      </el-form-item>
      <el-form-item label="公告使用者" prop="foruser">
        <el-select v-model="form.foruser" placeholder="請選擇使用者" clearable filterable>
          <el-option v-for="user in userList" :key="user.userId" :label="user.userName" :value="user.userId" />
        </el-select>
      </el-form-item>
      <el-form-item label="公告組織" prop="fororg">
        <el-select v-model="form.fororg" placeholder="請選擇組織" clearable>
          <el-option v-for="org in orgList" :key="org.orgId" :label="org.orgName" :value="org.orgId" />
        </el-select>
      </el-form-item>
      <el-form-item label="是否啟用" prop="isonline">
        <el-select v-model="form.isonline" placeholder="請選擇">
          <el-option label="啟用" value="Y" />
          <el-option label="停用" value="N" />
        </el-select>
      </el-form-item>
      <el-form-item label="公告類別" prop="bulletinType">
        <el-select v-model="form.bulletinType" placeholder="請選擇類別">
          <el-option v-for="type in bulletinTypeList" :key="type.tag" :label="type.content" :value="type.tag" />
        </el-select>
      </el-form-item>
      <el-form-item label="公告內容" prop="notes">
        <el-input
          v-model="form.notes"
          type="textarea"
          :rows="5"
          placeholder="請輸入公告內容"
          maxlength="2000"
          show-word-limit
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

### 4.3 API 呼叫 (`xcom.api.ts`)
```typescript
import request from '@/utils/request';

export interface BulletinDto {
  tKey: number;
  startTime: string;
  endTime: string;
  forgroup?: string;
  forgroupName?: string;
  foruser?: string;
  foruserName?: string;
  fororg?: string;
  fororgName?: string;
  isonline: string;
  bulletinType: string;
  bulletinTypeName?: string;
  notes?: string;
  cuser: string;
  cuserName?: string;
  ctime: string;
}

export interface BulletinQueryDto {
  pageIndex: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: 'ASC' | 'DESC';
  filters?: {
    startTimeFrom?: string;
    startTimeTo?: string;
    endTimeFrom?: string;
    endTimeTo?: string;
    forgroup?: string;
    foruser?: string;
    fororg?: string;
    isonline?: string;
    bulletinType?: string;
  };
}

export interface CreateBulletinDto {
  startTime: string;
  endTime: string;
  forgroup?: string;
  foruser?: string;
  fororg?: string;
  isonline: string;
  bulletinType: string;
  notes?: string;
}

export interface UpdateBulletinDto extends CreateBulletinDto {}

// API 函數
export const getBulletinList = (query: BulletinQueryDto) => {
  return request.get<ApiResponse<PagedResult<BulletinDto>>>('/api/v1/xcom/bulletin', { params: query });
};

export const getBulletinById = (tKey: number) => {
  return request.get<ApiResponse<BulletinDto>>(`/api/v1/xcom/bulletin/${tKey}`);
};

export const createBulletin = (data: CreateBulletinDto) => {
  return request.post<ApiResponse<number>>('/api/v1/xcom/bulletin', data);
};

export const updateBulletin = (tKey: number, data: UpdateBulletinDto) => {
  return request.put<ApiResponse>(`/api/v1/xcom/bulletin/${tKey}`, data);
};

export const deleteBulletin = (tKey: number) => {
  return request.delete<ApiResponse>(`/api/v1/xcom/bulletin/${tKey}`);
};

export const deleteBulletins = (tKeys: number[]) => {
  return request.delete<ApiResponse>('/api/v1/xcom/bulletin/batch', { data: { tKeys } });
};

export const updateBulletinStatus = (tKey: number, isonline: string) => {
  return request.put<ApiResponse>(`/api/v1/xcom/bulletin/${tKey}/status`, { isonline });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 必須驗證輸入內容，防止XSS攻擊
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 必須使用快取機制

### 6.3 資料驗證
- 起始時間必須早於結束時間
- 公告內容最多2000字元
- 必填欄位必須驗證
- 日期時間格式必須驗證

### 6.4 業務邏輯
- 刪除佈告欄前必須檢查是否有相關資料
- 停用佈告欄時必須檢查時間範圍
- 必須記錄操作日誌
- 公告對象設定邏輯（角色、使用者、組織）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增佈告欄成功
- [ ] 新增佈告欄失敗 (時間範圍錯誤)
- [ ] 修改佈告欄成功
- [ ] 修改佈告欄失敗 (不存在)
- [ ] 刪除佈告欄成功
- [ ] 查詢佈告欄列表成功
- [ ] 查詢單筆佈告欄成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FI.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FU.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FD.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FQ.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_FB.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM140_PR.asp`

### 8.2 資料庫 Schema
- `BULLETIN` 資料表

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

