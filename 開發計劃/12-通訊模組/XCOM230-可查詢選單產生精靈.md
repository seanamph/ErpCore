# XCOM230 - 可查詢選單產生精靈 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM230
- **功能名稱**: 可查詢選單產生精靈
- **功能描述**: 提供可查詢選單配置的新增、修改、刪除、查詢功能，包含標題、傳回欄位列表、中文名稱列表、Focus欄位、SELECT字串、FROM字串、WHERE字串、ORDER字串、可查詢欄位列表、選單代碼、選單型態、欄位顯示大小列表等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FS.ASP` (保存)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM230_PR.ASP` (報表)

### 1.2 業務需求
- 管理系統可查詢選單配置
- 支援選單代碼(LIST_ID)唯一性檢查
- 支援SQL查詢字串配置（SELECT、FROM、WHERE、ORDER）
- 支援欄位列表配置（傳回欄位、中文名稱、可查詢欄位）
- 支援選單型態設定
- 支援欄位顯示大小配置
- 支援資料查詢與瀏覽
- 支援資料報表列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ListConfig` (選單配置，對應舊系統 `XCOM_LIST_CFG`)

```sql
CREATE TABLE [dbo].[ListConfig] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Title] NVARCHAR(100) NOT NULL, -- 標題
    [RetColumnList] NVARCHAR(500) NOT NULL, -- 傳回欄位列表
    [CColumnList] NVARCHAR(500) NOT NULL, -- 中文名稱列表
    [FocusColumn] NVARCHAR(100) NULL, -- Focus欄位
    [SelectStr] NVARCHAR(1000) NOT NULL, -- SELECT字串
    [FromStr] NVARCHAR(500) NOT NULL, -- FROM字串
    [WhereStr] NVARCHAR(1000) NULL, -- WHERE字串
    [OrderStr] NVARCHAR(500) NULL, -- ORDER字串
    [SearchColumnList] NVARCHAR(500) NULL, -- 可查詢欄位列表
    [ListId] NVARCHAR(50) NOT NULL, -- 選單代碼
    [ListType] NVARCHAR(20) NULL, -- 選單型態
    [ColumnWidthList] NVARCHAR(500) NULL, -- 欄位顯示大小列表
    [CUser] NVARCHAR(50) NULL, -- 建立者
    [CTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [CPriority] INT NULL DEFAULT 0, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者群組
    [BUser] NVARCHAR(50) NULL, -- 變更者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 變更時間
    CONSTRAINT [PK_ListConfig] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_ListConfig_ListId] UNIQUE ([ListId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ListConfig_ListId] ON [dbo].[ListConfig] ([ListId]);
CREATE NONCLUSTERED INDEX [IX_ListConfig_Title] ON [dbo].[ListConfig] ([Title]);
CREATE NONCLUSTERED INDEX [IX_ListConfig_ListType] ON [dbo].[ListConfig] ([ListType]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| Title | NVARCHAR | 100 | NO | - | 標題 | - |
| RetColumnList | NVARCHAR | 500 | NO | - | 傳回欄位列表 | 逗號分隔 |
| CColumnList | NVARCHAR | 500 | NO | - | 中文名稱列表 | 逗號分隔 |
| FocusColumn | NVARCHAR | 100 | YES | - | Focus欄位 | - |
| SelectStr | NVARCHAR | 1000 | NO | - | SELECT字串 | SQL SELECT語句 |
| FromStr | NVARCHAR | 500 | NO | - | FROM字串 | SQL FROM語句 |
| WhereStr | NVARCHAR | 1000 | YES | - | WHERE字串 | SQL WHERE語句 |
| OrderStr | NVARCHAR | 500 | YES | - | ORDER字串 | SQL ORDER語句 |
| SearchColumnList | NVARCHAR | 500 | YES | - | 可查詢欄位列表 | 逗號分隔 |
| ListId | NVARCHAR | 50 | NO | - | 選單代碼 | 唯一 |
| ListType | NVARCHAR | 20 | YES | - | 選單型態 | - |
| ColumnWidthList | NVARCHAR | 500 | YES | - | 欄位顯示大小列表 | 逗號分隔 |
| CUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| CTime | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| CPriority | INT | - | YES | 0 | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |
| BUser | NVARCHAR | 50 | YES | - | 變更者 | - |
| BTime | DATETIME2 | - | NO | GETDATE() | 變更時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢選單配置列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom230/list-configs`
- **說明**: 查詢選單配置列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "ListId",
    "sortOrder": "ASC",
    "filters": {
      "listId": "",
      "title": "",
      "listType": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆選單配置
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom230/list-configs/{listId}`
- **說明**: 根據選單代碼查詢單筆選單配置
- **路徑參數**:
  - `listId`: 選單代碼
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增選單配置
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom230/list-configs`
- **說明**: 新增選單配置
- **請求格式**:
  ```json
  {
    "title": "選單標題",
    "retColumnList": "COL1,COL2,COL3",
    "cColumnList": "欄位1,欄位2,欄位3",
    "focusColumn": "COL1",
    "selectStr": "SELECT COL1, COL2, COL3",
    "fromStr": "FROM TABLE_NAME",
    "whereStr": "WHERE STATUS = '1'",
    "orderStr": "ORDER BY COL1",
    "searchColumnList": "COL1,COL2",
    "listId": "LIST001",
    "listType": "SINGLE",
    "columnWidthList": "100,150,200"
  }
  ```
- **回應格式**: 標準新增回應格式

#### 3.1.4 修改選單配置
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom230/list-configs/{listId}`
- **說明**: 修改選單配置
- **路徑參數**:
  - `listId`: 選單代碼
- **請求格式**: 同新增，但 `listId` 不可修改
- **回應格式**: 標準修改回應格式

#### 3.1.5 刪除選單配置
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom230/list-configs/{listId}`
- **說明**: 刪除選單配置
- **路徑參數**:
  - `listId`: 選單代碼
- **回應格式**: 標準刪除回應格式

#### 3.1.6 測試選單查詢
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom230/list-configs/{listId}/test`
- **說明**: 測試選單SQL查詢
- **請求格式**:
  ```json
  {
    "searchParams": {
      "COL1": "value1",
      "COL2": "value2"
    }
  }
  ```
- **回應格式**: 標準查詢回應格式

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCom230Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom230")]
    [Authorize]
    public class XCom230Controller : ControllerBase
    {
        private readonly IListConfigService _listConfigService;
        
        public XCom230Controller(IListConfigService listConfigService)
        {
            _listConfigService = listConfigService;
        }
        
        [HttpGet("list-configs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ListConfigDto>>>> GetListConfigs([FromQuery] ListConfigQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("list-configs/{listId}")]
        public async Task<ActionResult<ApiResponse<ListConfigDto>>> GetListConfig(string listId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost("list-configs")]
        public async Task<ActionResult<ApiResponse<string>>> CreateListConfig([FromBody] CreateListConfigDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("list-configs/{listId}")]
        public async Task<ActionResult<ApiResponse>> UpdateListConfig(string listId, [FromBody] UpdateListConfigDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("list-configs/{listId}")]
        public async Task<ActionResult<ApiResponse>> DeleteListConfig(string listId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("list-configs/{listId}/test")]
        public async Task<ActionResult<ApiResponse<object>>> TestListConfig(string listId, [FromBody] TestListConfigDto dto)
        {
            // 實作測試查詢邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 選單配置列表頁面 (`ListConfigList.vue`)
- **路徑**: `/xcom/list-config/list`
- **功能**: 顯示選單配置列表，支援查詢、新增、修改、刪除

#### 4.1.2 選單配置新增/修改頁面 (`ListConfigForm.vue`)
- **路徑**: `/xcom/list-config/form/:listId?`
- **功能**: 新增或修改選單配置

### 4.2 UI 元件設計

#### 4.2.1 選單配置表單元件 (`ListConfigForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="180px">
    <el-form-item label="標題" prop="title">
      <el-input v-model="form.title" placeholder="請輸入標題" maxlength="100" />
    </el-form-item>
    <el-form-item label="選單代碼" prop="listId">
      <el-input v-model="form.listId" :disabled="isEdit" placeholder="請輸入選單代碼" maxlength="50" />
    </el-form-item>
    <el-form-item label="選單型態" prop="listType">
      <el-select v-model="form.listType" placeholder="請選擇選單型態">
        <el-option label="單選" value="SINGLE" />
        <el-option label="多選" value="MULTI" />
      </el-select>
    </el-form-item>
    <el-form-item label="SELECT字串" prop="selectStr">
      <el-input v-model="form.selectStr" type="textarea" :rows="3" placeholder="請輸入SELECT字串" maxlength="1000" />
    </el-form-item>
    <el-form-item label="FROM字串" prop="fromStr">
      <el-input v-model="form.fromStr" placeholder="請輸入FROM字串" maxlength="500" />
    </el-form-item>
    <el-form-item label="WHERE字串" prop="whereStr">
      <el-input v-model="form.whereStr" type="textarea" :rows="2" placeholder="請輸入WHERE字串" maxlength="1000" />
    </el-form-item>
    <el-form-item label="ORDER字串" prop="orderStr">
      <el-input v-model="form.orderStr" placeholder="請輸入ORDER字串" maxlength="500" />
    </el-form-item>
    <el-form-item label="傳回欄位列表" prop="retColumnList">
      <el-input v-model="form.retColumnList" placeholder="請輸入傳回欄位列表（逗號分隔）" maxlength="500" />
    </el-form-item>
    <el-form-item label="中文名稱列表" prop="cColumnList">
      <el-input v-model="form.cColumnList" placeholder="請輸入中文名稱列表（逗號分隔）" maxlength="500" />
    </el-form-item>
    <el-form-item label="可查詢欄位列表" prop="searchColumnList">
      <el-input v-model="form.searchColumnList" placeholder="請輸入可查詢欄位列表（逗號分隔）" maxlength="500" />
    </el-form-item>
    <el-form-item label="Focus欄位" prop="focusColumn">
      <el-input v-model="form.focusColumn" placeholder="請輸入Focus欄位" maxlength="100" />
    </el-form-item>
    <el-form-item label="欄位顯示大小列表" prop="columnWidthList">
      <el-input v-model="form.columnWidthList" placeholder="請輸入欄位顯示大小列表（逗號分隔）" maxlength="500" />
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSubmit" :loading="loading">確定</el-button>
      <el-button @click="handleCancel">取消</el-button>
      <el-button @click="handleTest" v-if="isEdit">測試查詢</el-button>
    </el-form-item>
  </el-form>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立唯一約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作（包含SQL驗證邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] SQL查詢測試功能
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改表單開發
- [ ] 查詢表單開發
- [ ] 表單驗證
- [ ] SQL測試功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] SQL查詢測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- SQL字串必須驗證，防止SQL注入
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 業務邏輯
- 選單代碼必須唯一
- SQL字串必須驗證語法正確性
- 欄位列表數量必須一致
- 必須驗證SQL查詢可執行性

### 6.3 資料驗證
- 標題必須驗證長度
- 選單代碼必須驗證格式
- SQL字串必須驗證語法
- 欄位列表必須驗證格式

### 6.4 效能
- SQL查詢必須優化
- 必須建立適當的索引
- 大量資料查詢必須使用分頁

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增選單配置成功
- [ ] 新增選單配置失敗 (重複代碼)
- [ ] 修改選單配置成功
- [ ] 刪除選單配置成功
- [ ] 查詢選單配置列表成功
- [ ] 測試SQL查詢成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] SQL查詢測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FI.ASP` (新增)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FU.ASP` (修改)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FD.ASP` (刪除)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FQ.ASP` (查詢)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FB.ASP` (瀏覽)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_FS.ASP` (保存)
- `WEB/IMS_CORE/ASP/XCOM000/XCOM230_PR.ASP` (報表)

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

