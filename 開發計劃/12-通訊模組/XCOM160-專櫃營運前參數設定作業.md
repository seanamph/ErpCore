# XCOM160 - 專櫃營運前參數設定作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: XCOM160
- **功能名稱**: 專櫃營運前參數設定作業
- **功能描述**: 提供專櫃營運前POS系統事件列表參數的新增、修改、刪除、查詢功能，包含支付類型、POS收銀機代碼、發票標記、事件代碼、支付代碼、找零設定、壞帳標記、退貨標記、序號、支付種類、EDC銀行、VIP卡標記、交易記錄、使用類型、狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/XCOM000/XCOM160_FI.ASP` (新增)

### 1.2 業務需求
- 管理專櫃營運前POS系統事件列表參數
- 支援支付類型參數設定（固定代碼：現金10、悠遊卡50、一卡通52、微信支付53、支付寶54、愛金卡56、即享券57、歐付寶58、街口支付59等）
- 支援實收審核型態設定（S:審核時可調整金額、R:第三方支付不用審核、A:要審核但不可修改）
- 支援找零設定（可找零、可溢收）
- 支援事件類型設定（F:固定、P:額外維護）
- 支援狀態管理（Y:啟用、N:停用）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EventList` (事件列表，對應舊系統 `tmp_event_list`)

```sql
CREATE TABLE [dbo].[EventList] (
    [T_KEY] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [PAY_TYPE_NAME] NVARCHAR(100) NOT NULL, -- 支付類型名稱
    [POS_TENDER] NVARCHAR(10) NOT NULL, -- POS收銀機代碼
    [PAY_TYPE_INVOICE_FLAG] NVARCHAR(10) NULL, -- 發票標記
    [POS_EVENT_ID] NVARCHAR(10) NOT NULL, -- POS事件代碼
    [PAY_TYPE_ID] NVARCHAR(50) NOT NULL, -- 支付代碼
    [GIVE_CHANGE_YN] NVARCHAR(1) NOT NULL DEFAULT 'N', -- 可找零 (Y/N)
    [EXTRA_AMT_YN] NVARCHAR(1) NOT NULL DEFAULT 'N', -- 可溢收 (Y/N)
    [BAD_YN] NVARCHAR(1) NOT NULL DEFAULT 'N', -- 壞帳標記 (Y/N)
    [RETURN_YN] NVARCHAR(1) NOT NULL DEFAULT 'N', -- 退貨標記 (Y/N)
    [SEQ_NO] INT NULL, -- 序號
    [PAY_KIND] NVARCHAR(50) NULL, -- 支付種類
    [EDC_BANK] NVARCHAR(50) NULL, -- EDC銀行
    [VIP_CARD_YN] NVARCHAR(1) NOT NULL DEFAULT 'N', -- VIP卡標記 (Y/N)
    [VCH_TRANS_LOG] NVARCHAR(1) NOT NULL DEFAULT 'N', -- 交易記錄 (Y/N)
    [USE_TYPE] NVARCHAR(50) NULL, -- 使用類型
    [STATUS] NVARCHAR(1) NOT NULL DEFAULT 'Y', -- 狀態 (Y:啟用, N:停用)
    [EVENT_TYPE] NVARCHAR(1) NOT NULL DEFAULT 'F', -- 事件類型 (F:固定, P:額外維護)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_EventList] PRIMARY KEY CLUSTERED ([T_KEY] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EventList_PAY_TYPE_ID] ON [dbo].[EventList] ([PAY_TYPE_ID]);
CREATE NONCLUSTERED INDEX [IX_EventList_POS_TENDER] ON [dbo].[EventList] ([POS_TENDER]);
CREATE NONCLUSTERED INDEX [IX_EventList_EVENT_TYPE] ON [dbo].[EventList] ([EVENT_TYPE]);
CREATE NONCLUSTERED INDEX [IX_EventList_STATUS] ON [dbo].[EventList] ([STATUS]);
CREATE NONCLUSTERED INDEX [IX_EventList_POS_EVENT_ID] ON [dbo].[EventList] ([POS_EVENT_ID]);
```

### 2.2 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| T_KEY | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| PAY_TYPE_NAME | NVARCHAR | 100 | NO | - | 支付類型名稱 | - |
| POS_TENDER | NVARCHAR | 10 | NO | - | POS收銀機代碼 | 固定代碼：現金10、悠遊卡50等 |
| PAY_TYPE_INVOICE_FLAG | NVARCHAR | 10 | YES | - | 發票標記 | - |
| POS_EVENT_ID | NVARCHAR | 10 | NO | - | POS事件代碼 | - |
| PAY_TYPE_ID | NVARCHAR | 50 | NO | - | 支付代碼 | - |
| GIVE_CHANGE_YN | NVARCHAR | 1 | NO | 'N' | 可找零 | Y:可找零, N:不可找零 |
| EXTRA_AMT_YN | NVARCHAR | 1 | NO | 'N' | 可溢收 | Y:可溢收, N:不可溢收 |
| BAD_YN | NVARCHAR | 1 | NO | 'N' | 壞帳標記 | Y:是, N:否 |
| RETURN_YN | NVARCHAR | 1 | NO | 'N' | 退貨標記 | Y:是, N:否 |
| SEQ_NO | INT | - | YES | - | 序號 | - |
| PAY_KIND | NVARCHAR | 50 | YES | - | 支付種類 | - |
| EDC_BANK | NVARCHAR | 50 | YES | - | EDC銀行 | - |
| VIP_CARD_YN | NVARCHAR | 1 | NO | 'N' | VIP卡標記 | Y:是, N:否 |
| VCH_TRANS_LOG | NVARCHAR | 1 | NO | 'N' | 交易記錄 | Y:是, N:否 |
| USE_TYPE | NVARCHAR | 50 | YES | - | 使用類型 | - |
| STATUS | NVARCHAR | 1 | NO | 'Y' | 狀態 | Y:啟用, N:停用 |
| EVENT_TYPE | NVARCHAR | 1 | NO | 'F' | 事件類型 | F:固定, P:額外維護 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢事件列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/xcom160/event-list`
- **說明**: 查詢事件列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "SEQ_NO",
    "sortOrder": "ASC",
    "filters": {
      "payTypeId": "",
      "posTender": "",
      "eventType": "",
      "status": ""
    }
  }
  ```
- **回應格式**: 標準分頁回應格式

#### 3.1.2 新增事件
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom160/event-list`
- **說明**: 新增事件列表資料
- **請求格式**: 包含所有欄位的DTO

#### 3.1.3 修改事件
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/xcom160/event-list/{tKey}`
- **說明**: 修改事件列表資料

#### 3.1.4 刪除事件
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/xcom160/event-list/{tKey}`
- **說明**: 刪除事件列表資料

#### 3.1.5 批次新增/修改
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/xcom160/event-list/batch`
- **說明**: 批次新增/修改事件列表資料

### 3.2 後端實作類別

#### 3.2.1 Controller: `XCOM160Controller.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/xcom160")]
    [Authorize]
    public class XCOM160Controller : ControllerBase
    {
        private readonly IXCOM160Service _service;
        
        public XCOM160Controller(IXCOM160Service service)
        {
            _service = service;
        }
        
        [HttpGet("event-list")]
        public async Task<ActionResult<ApiResponse<PagedResult<EventListDto>>>> GetEventList([FromQuery] EventListQueryDto query)
        {
            var result = await _service.GetEventListAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("event-list")]
        public async Task<ActionResult<ApiResponse<long>>> CreateEventList([FromBody] CreateEventListDto dto)
        {
            var result = await _service.CreateEventListAsync(dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPut("event-list/{tKey}")]
        public async Task<ActionResult<ApiResponse>> UpdateEventList(long tKey, [FromBody] UpdateEventListDto dto)
        {
            await _service.UpdateEventListAsync(tKey, dto);
            return Ok(ApiResponse.Success());
        }
        
        [HttpDelete("event-list/{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteEventList(long tKey)
        {
            await _service.DeleteEventListAsync(tKey);
            return Ok(ApiResponse.Success());
        }
        
        [HttpPost("event-list/batch")]
        public async Task<ActionResult<ApiResponse>> BatchUpdateEventList([FromBody] BatchEventListDto dto)
        {
            await _service.BatchUpdateEventListAsync(dto);
            return Ok(ApiResponse.Success());
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 事件列表維護頁面 (`XCOM160Maintain.vue`)
- **路徑**: `/xcom160/maintain`
- **功能**: 維護事件列表資料
- **主要元件**:
  - 查詢表單 (EventListSearchForm)
  - 資料表格 (EventListTable)
  - 新增/修改對話框 (EventListDialog)
  - 批次操作功能

### 4.2 UI 元件設計

#### 4.2.1 資料表格元件 (`EventListTable.vue`)
```vue
<template>
  <div>
    <el-table :data="eventList" v-loading="loading" border>
      <el-table-column prop="payTypeName" label="支付類型名稱" width="150" />
      <el-table-column prop="posTender" label="POS收銀機代碼" width="120" />
      <el-table-column prop="payTypeId" label="支付代碼" width="120" />
      <el-table-column prop="posEventId" label="POS事件代碼" width="120" />
      <el-table-column prop="giveChangeYn" label="可找零" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.giveChangeYn === 'Y' ? 'success' : 'info'">
            {{ row.giveChangeYn === 'Y' ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="extraAmtYn" label="可溢收" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.extraAmtYn === 'Y' ? 'success' : 'info'">
            {{ row.extraAmtYn === 'Y' ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.status === 'Y' ? 'success' : 'danger'">
            {{ row.status === 'Y' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立預設值
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
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 批次操作功能
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
- 固定代碼不可修改（現金10、悠遊卡50等）
- 事件類型F（固定）和P（額外維護）的處理邏輯不同
- 刪除P類型事件時，需要檢查是否被F類型事件引用
- 找零設定（GIVE_CHANGE_YN）和溢收設定（EXTRA_AMT_YN）互斥

### 6.2 資料驗證
- 支付代碼必須唯一
- POS收銀機代碼必須符合固定代碼規範
- 序號必須為正整數
- 狀態值必須為Y或N

### 6.3 安全性
- 必須實作權限檢查
- 固定代碼（F類型）僅允許查詢，不允許修改
- 批次操作需要事務處理

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增事件成功
- [ ] 新增事件失敗 (重複支付代碼)
- [ ] 修改事件成功
- [ ] 刪除事件成功
- [ ] 批次操作成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 批次操作測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/XCOM000/XCOM160_FI.ASP`

### 8.2 固定代碼說明
- 現金: 10
- 悠遊卡: 50
- 一卡通: 52
- 微信支付: 53
- 支付寶: 54
- 愛金卡: 56
- 即享券: 57
- 歐付寶: 58
- 街口支付: 59
- 街口折抵: A1
- 代用券一: D0
- 代用券二: D1

### 8.3 實收審核型態
- S: 審核時可調整金額 (EX:代用券)
- R: 第三方支付,不用審核 (EX:一卡通)
- A: 要審核但不可修改

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

