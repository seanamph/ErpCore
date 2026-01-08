# XCOM170 - 縣市別多語系維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM170
- **功能名稱**: 縣市別多語系維護作業
- **功能描述**: 提供縣市別多語系資料的新增、修改、刪除、查詢功能，包含基準字、轉換語系1、轉換語系2、建立時間、變更時間等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM170_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM170_FU.ASP` (修改)

### 1.2 業務需求
- 管理縣市別多語系對應關係
- 支援基準字（BASE_KEY）維護
- 支援轉換語系1（TRANS_1）維護
- 支援轉換語系2（TRANS_2）維護
- 支援多筆資料批次維護
- 支援資料查詢與瀏覽

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `MultiLangs` (多語系對應表，對應舊系統 `MULTI_LANGS`)

```sql
CREATE TABLE [dbo].[MultiLangs] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [BASE_KEY] NVARCHAR(200) NOT NULL, -- 基準字
    [TRANS_1] NVARCHAR(200) NULL, -- 轉換語系1
    [TRANS_2] NVARCHAR(200) NULL, -- 轉換語系2
    [BTIME] NVARCHAR(100) NULL, -- 建立時間（字串格式）
    [CUser] NVARCHAR(50) NULL, -- 建立者
    [CTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [BUser] NVARCHAR(50) NULL, -- 變更者
    [BTime] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 變更時間
    [CPriority] INT NULL DEFAULT 0, -- 建立者等級
    [CGroup] NVARCHAR(50) NULL, -- 建立者群組
    CONSTRAINT [PK_MultiLangs] PRIMARY KEY CLUSTERED ([T_KEY] ASC),
    CONSTRAINT [UQ_MultiLangs_BASE_KEY] UNIQUE ([BASE_KEY])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_MultiLangs_BASE_KEY] ON [dbo].[MultiLangs] ([BASE_KEY]);
CREATE NONCLUSTERED INDEX [IX_MultiLangs_TRANS_1] ON [dbo].[MultiLangs] ([TRANS_1]);
CREATE NONCLUSTERED INDEX [IX_MultiLangs_TRANS_2] ON [dbo].[MultiLangs] ([TRANS_2]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| BASE_KEY | NVARCHAR | 200 | NO | - | 基準字 | 唯一，必填 |
| TRANS_1 | NVARCHAR | 200 | YES | - | 轉換語系1 | - |
| TRANS_2 | NVARCHAR | 200 | YES | - | 轉換語系2 | - |
| BTIME | NVARCHAR | 100 | YES | - | 建立時間（字串） | - |
| CUser | NVARCHAR | 50 | YES | - | 建立者 | - |
| CTime | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| BUser | NVARCHAR | 50 | YES | - | 變更者 | - |
| BTime | DATETIME2 | - | NO | GETDATE() | 變更時間 | - |
| CPriority | INT | - | YES | 0 | 建立者等級 | - |
| CGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢多語系列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom170/multi-langs`
- **說明**: 查詢多語系列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "BASE_KEY",
    "sortOrder": "ASC",
    "filters": {
      "baseKey": "",
      "trans1": "",
      "trans2": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 查詢單筆多語系
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom170/multi-langs/{baseKey}`
- **說明**: 根據基準字查詢單筆多語系資料
- **路徑參數**:
  - `baseKey`: 基準字
- **回應格式**: 標準單筆回應格式

#### 3.1.3 新增多語系
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom170/multi-langs`
- **說明**: 新增多語系資料
- **請求格式**:
  ```json
  {
    "baseKey": "基準字",
    "trans1": "轉換語系1",
    "trans2": "轉換語系2",
    "btime": "建立時間字串"
  }
  ```

#### 3.1.4 修改多語系
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom170/multi-langs/{baseKey}`
- **說明**: 修改多語系資料（根據BASE_KEY更新）

#### 3.1.5 刪除多語系
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom170/multi-langs/{baseKey}`
- **說明**: 刪除多語系資料

#### 3.1.6 批次新增/修改
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom170/multi-langs/batch`
- **說明**: 批次新增/修改多語系資料
- **請求格式**:
  ```json
  {
    "items": [
      {
        "baseKey": "基準字1",
        "trans1": "轉換語系1-1",
        "trans2": "轉換語系2-1",
        "btime": "建立時間1"
      },
      {
        "baseKey": "基準字2",
        "trans1": "轉換語系1-2",
        "trans2": "轉換語系2-2",
        "btime": "建立時間2"
      }
    ]
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCOM170Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom170")]
    [Authorize]
    public class XCOM170Controller : ControllerBase
    {
        private readonly IXCOM170Service _service;
        
        public XCOM170Controller(IXCOM170Service service)
        {
            _service = service;
        }
        
        [HttpGet("multi-langs")]
        public async Task<ActionResult<ApiResponse<PagedResult<MultiLangDto>>>> GetMultiLangs([FromQuery] MultiLangQueryDto query)
        {
            var result = await _service.GetMultiLangsAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("multi-langs/{baseKey}")]
        public async Task<ActionResult<ApiResponse<MultiLangDto>>> GetMultiLang(string baseKey)
        {
            var result = await _service.GetMultiLangByBaseKeyAsync(baseKey);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("multi-langs")]
        public async Task<ActionResult<ApiResponse<long>>> CreateMultiLang([FromBody] CreateMultiLangDto dto)
        {
            var result = await _service.CreateMultiLangAsync(dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPut("multi-langs/{baseKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateMultiLang(string baseKey, [FromBody] UpdateMultiLangDto dto)
        {
            await _service.UpdateMultiLangAsync(baseKey, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("multi-langs/{baseKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteMultiLang(string baseKey)
        {
            await _service.DeleteMultiLangAsync(baseKey);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPost("multi-langs/batch")]
        public async Task<ActionResult<ApiResponse>> BatchUpdateMultiLangs([FromBody] BatchMultiLangDto dto)
        {
            await _service.BatchUpdateMultiLangsAsync(dto);
            return Ok(ApiResponse.Success());
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 多語系維護頁面 (`XCOM170Maintain.vue`)
- **路徑**: `/xcom170/maintain`
- **功能**: 維護多語系資料
- **主要元件**:
  - 查詢表單 (MultiLangSearchForm)
  - 資料表格 (MultiLangTable)
  - 批次維護功能

### 4.2 UI 元件設計

#### 4.2.1 資料表格元件 (`MultiLangTable.vue`)
```vue
<template>
  <div>
    <el-table :data="multiLangList" v-loading="loading" border>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="baseKey" label="基準字" width="200" />
      <el-table-column prop="trans1" label="轉換語系1" width="200" />
      <el-table-column prop="trans2" label="轉換語系2" width="200" />
      <el-table-column prop="btime" label="建立時間" width="150" />
      <el-table-column prop="cTime" label="系統建立時間" width="180" />
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <div style="margin-top: 10px;">
      <el-button type="primary" @click="handleAddRow">新增一筆</el-button>
      <el-button type="success" @click="handleBatchSave">批次儲存</el-button>
      <el-button type="danger" @click="handleBatchDelete">批次刪除</el-button>
    </div>
  </div>
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
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 批次維護功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

**總計**: 8天

---

## 六、注意事項

### 6.1 業務邏輯
- 基準字（BASE_KEY）必須唯一
- 修改時根據BASE_KEY更新，如果不存在則新增
- 批次操作時，需要檢查基準字是否重複
- 刪除時需要檢查是否被其他系統引用

### 6.2 資料驗證
- 基準字不能為空
- 基準字長度限制為200字元
- 轉換語系1和轉換語系2長度限制為200字元

### 6.3 安全性
- 必須實作權限檢查
- 批次操作需要事務處理
- 敏感資料需要適當的權限控制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增多語系成功
- [ ] 新增多語系失敗 (重複基準字)
- [ ] 修改多語系成功
- [ ] 刪除多語系成功
- [ ] 批次操作成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 批次操作測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM170_FQ.ASP`
- `WEB/IMS_CORE/ASP/XCOM000/XCOM170_FU.ASP`
- `IMS3/HANSHIN/RSL_CLASS/IMS3_SYS/MULTI_LANGS.cs`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

