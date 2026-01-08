# XCOM150 - 最大T_KEY值查詢 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM150
- **功能名稱**: 最大T_KEY值查詢
- **功能描述**: 提供資料庫表最大T_KEY值查詢功能，用於查詢指定資料庫使用者下所有資料表的最大T_KEY值，協助系統維護和資料分析
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM150_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM150_FQ.asp` (查詢)

### 1.2 業務需求
- 查詢指定資料庫使用者下所有資料表的最大T_KEY值
- 支援多個資料庫使用者查詢
- 顯示資料表名稱和對應的最大T_KEY值
- 統計所有資料表中的最大T_KEY值
- 協助系統維護和資料分析

---

## 二、資料庫設計 (Schema)

### 2.1 查詢功能說明
本功能為查詢工具，不涉及資料表建立，主要查詢系統資料表的中繼資料。

### 2.2 查詢邏輯
- 查詢 `DBA_USERS` 或 `ALL_USERS` 取得資料庫使用者列表
- 查詢 `ALL_TAB_COLUMNS` 確認資料表是否有 `T_KEY` 欄位
- 查詢各資料表的 `MAX(T_KEY)` 值
- 統計所有資料表中的最大T_KEY值

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 說明 | 備註 |
|---------|---------|------|------|
| DBUSER | NVARCHAR(50) | 資料庫使用者名稱 | 查詢條件 |
| TABLE_NAME | NVARCHAR(128) | 資料表名稱 | 查詢結果 |
| MAX_T_KEY | BIGINT | 最大T_KEY值 | 查詢結果 |
| HAS_T_KEY | BIT | 是否有T_KEY欄位 | 查詢結果 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢資料庫使用者列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom150/db-users`
- **說明**: 查詢所有符合條件的資料庫使用者列表
- **請求參數**: 無
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "dbUsers": [
        "RSL_USER1",
        "RSL_USER2"
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢資料表最大T_KEY值
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom150/max-tkeys`
- **說明**: 查詢指定資料庫使用者下所有資料表的最大T_KEY值
- **請求參數**:
  ```json
  {
    "dbUser": "RSL_USER1" // 可選，不提供則查詢所有RSL開頭的使用者
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "results": [
        {
          "dbUser": "RSL_USER1",
          "tableName": "USERS",
          "maxTKey": 1000,
          "hasTKey": true
        },
        {
          "dbUser": "RSL_USER1",
          "tableName": "ROLES",
          "maxTKey": 500,
          "hasTKey": true
        }
      ],
      "globalMaxTKey": 1000
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCOM150Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom150")]
    [Authorize]
    public class XCOM150Controller : ControllerBase
    {
        private readonly IXCOM150Service _service;
        
        public XCOM150Controller(IXCOM150Service service)
        {
            _service = service;
        }
        
        [HttpGet("db-users")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetDbUsers()
        {
            var result = await _service.GetDbUsersAsync();
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("max-tkeys")]
        public async Task<ActionResult<ApiResponse<MaxTKeyResultDto>>> GetMaxTKeys([FromQuery] string dbUser = null)
        {
            var result = await _service.GetMaxTKeysAsync(dbUser);
            return Ok(ApiResponse.Success(result));
        }
    }
}
```

#### 3.2.2 Service: `XCOM150Service.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IXCOM150Service
    {
        Task<List<string>> GetDbUsersAsync();
        Task<MaxTKeyResultDto> GetMaxTKeysAsync(string dbUser = null);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 最大T_KEY值查詢頁面 (`XCOM150Query.vue`)
- **路徑**: `/xcom150/query`
- **功能**: 查詢資料表最大T_KEY值
- **主要元件**:
  - 查詢表單 (DbUserSelect)
  - 資料表格 (MaxTKeyTable)
  - 統計資訊顯示

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`DbUserSelect.vue`)
```vue
<template>
  <el-form :model="queryForm" inline>
    <el-form-item label="資料庫使用者">
      <el-select v-model="queryForm.dbUser" placeholder="請選擇資料庫使用者" clearable>
        <el-option label="全部" value="" />
        <el-option v-for="user in dbUserList" :key="user" :label="user" :value="user" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleQuery">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`MaxTKeyTable.vue`)
```vue
<template>
  <div>
    <el-table :data="tableData" v-loading="loading" border>
      <el-table-column prop="dbUser" label="資料庫使用者" width="150" />
      <el-table-column prop="tableName" label="資料表名稱" width="200" />
      <el-table-column prop="maxTKey" label="最大T_KEY值" width="150" align="right">
        <template #default="{ row }">
          <span v-if="row.hasTKey">{{ row.maxTKey }}</span>
          <span v-else style="color: red;">X</span>
        </template>
      </el-table-column>
      <el-table-column prop="hasTKey" label="是否有T_KEY" width="120" align="center">
        <template #default="{ row }">
          <el-tag :type="row.hasTKey ? 'success' : 'danger'">
            {{ row.hasTKey ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    <div v-if="globalMaxTKey !== null" style="margin-top: 20px; padding: 10px; background: #f5f5f5;">
      <strong>目前最大 T_KEY 值: {{ globalMaxTKey }}</strong>
    </div>
  </div>
</template>
```

### 4.3 API 呼叫 (`xcom150.api.ts`)
```typescript
import request from '@/utils/request';

export interface MaxTKeyItem {
  dbUser: string;
  tableName: string;
  maxTKey: number | null;
  hasTKey: boolean;
}

export interface MaxTKeyResultDto {
  results: MaxTKeyItem[];
  globalMaxTKey: number | null;
}

export const getDbUsers = () => {
  return request.get<ApiResponse<string[]>>('/api/v1/xcom150/db-users');
};

export const getMaxTKeys = (dbUser?: string) => {
  return request.get<ApiResponse<MaxTKeyResultDto>>('/api/v1/xcom150/max-tkeys', {
    params: { dbUser }
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 後端開發 (2天)
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 查詢邏輯實作
- [ ] 單元測試

### 5.2 階段二: 前端開發 (2天)
- [ ] API 呼叫函數
- [ ] 查詢頁面開發
- [ ] 資料表格開發
- [ ] 統計資訊顯示
- [ ] 元件測試

### 5.3 階段三: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 5天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查，僅允許系統管理員使用
- 查詢結果不應包含敏感資料
- 必須防止SQL注入攻擊

### 6.2 效能
- 大量資料表查詢可能較慢，需要優化查詢邏輯
- 考慮使用非同步查詢
- 可以考慮快取查詢結果

### 6.3 資料庫相容性
- 需要支援 SQL Server 的系統檢視表
- 查詢語法需要適配 SQL Server
- 注意權限設定，需要適當的資料庫權限

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢資料庫使用者列表成功
- [ ] 查詢指定使用者資料表最大T_KEY值成功
- [ ] 查詢所有使用者資料表最大T_KEY值成功
- [ ] 處理沒有T_KEY欄位的資料表

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 權限檢查測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM150_FB.asp`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM150_FQ.asp`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

