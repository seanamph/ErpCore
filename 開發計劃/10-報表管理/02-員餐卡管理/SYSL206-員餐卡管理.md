# SYSL206 - 員餐卡管理 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL206
- **功能名稱**: 員餐卡管理
- **功能描述**: 提供員餐卡資料的新增、修改、刪除、查詢功能，包含卡片類型、動作類型、其他類型、欄位名稱等資訊管理，支援載入上一筆名稱、切換Y/N值等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/SYSL206.ascx` (查詢/維護頁面)
  - `WEB/IMS_CORE/SYSL000/js/SYSL206.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL206.css` (樣式)

### 1.2 業務需求
- 管理員餐卡資料
- 支援員餐卡的新增、修改、刪除、查詢
- 支援卡片類型、動作類型、其他類型設定
- 支援欄位名稱維護
- 支援載入上一筆名稱功能
- 支援切換Y/N值功能
- 支援欄位顏色標記（區分不同類型）
- 記錄員餐卡的建立與變更資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `EmployeeMealCardFields` (對應舊系統相關表)

```sql
CREATE TABLE [dbo].[EmployeeMealCardFields] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [FieldId] NVARCHAR(50) NOT NULL, -- 欄位ID (FIELD_ID)
    [FieldName] NVARCHAR(200) NULL, -- 欄位名稱 (FIELD_NAME)
    [CardType] NVARCHAR(20) NULL, -- 卡片類型 (CARD_TYPE)
    [ActionType] NVARCHAR(20) NULL, -- 動作類型 (ACTION_TYPE)
    [OtherType] NVARCHAR(20) NULL, -- 其他類型 (OTHER_TYPE)
    [MustKeyinYn] NVARCHAR(1) NULL DEFAULT 'N', -- 必填標誌 (MUST_KEYIN_YN) Y:必填, N:非必填
    [ReadonlyYn] NVARCHAR(1) NULL DEFAULT 'N', -- 唯讀標誌 (READONLY_YN) Y:唯讀, N:可編輯
    [BtnEtekYn] NVARCHAR(1) NULL DEFAULT 'N', -- 按鈕標誌 (BTN_ETEK_YN) Y:顯示按鈕, N:不顯示
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 1:啟用, 0:停用
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_EmployeeMealCardFields] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_EmployeeMealCardFields_FieldId] UNIQUE ([FieldId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_CardType] ON [dbo].[EmployeeMealCardFields] ([CardType]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_ActionType] ON [dbo].[EmployeeMealCardFields] ([ActionType]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_OtherType] ON [dbo].[EmployeeMealCardFields] ([OtherType]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_Status] ON [dbo].[EmployeeMealCardFields] ([Status]);
CREATE NONCLUSTERED INDEX [IX_EmployeeMealCardFields_SeqNo] ON [dbo].[EmployeeMealCardFields] ([SeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `CardType` - 卡片類型主檔
- 參考: `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md` 的 `CardType` 資料表結構

#### 2.2.2 `ActionType` - 動作類型主檔
- 參考: `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md` 的 `ActionType` 資料表結構

#### 2.2.3 `OtherType` - 其他類型主檔
```sql
CREATE TABLE [dbo].[OtherType] (
    [OtherId] NVARCHAR(20) NOT NULL, -- 其他類型代碼
    [ActionId] NVARCHAR(20) NOT NULL, -- 動作類型代碼
    [OtherName] NVARCHAR(100) NOT NULL, -- 其他類型名稱
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_OtherType] PRIMARY KEY CLUSTERED ([OtherId], [ActionId]),
    CONSTRAINT [FK_OtherType_ActionType] FOREIGN KEY ([ActionId]) REFERENCES [dbo].[ActionType] ([ActionId])
);
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| FieldId | NVARCHAR | 50 | NO | - | 欄位ID | 唯一鍵 |
| FieldName | NVARCHAR | 200 | YES | - | 欄位名稱 | - |
| CardType | NVARCHAR | 20 | YES | - | 卡片類型 | 外鍵至CardType表 |
| ActionType | NVARCHAR | 20 | YES | - | 動作類型 | 外鍵至ActionType表 |
| OtherType | NVARCHAR | 20 | YES | - | 其他類型 | 外鍵至OtherType表 |
| MustKeyinYn | NVARCHAR | 1 | YES | 'N' | 必填標誌 | Y:必填, N:非必填 |
| ReadonlyYn | NVARCHAR | 1 | YES | 'N' | 唯讀標誌 | Y:唯讀, N:可編輯 |
| BtnEtekYn | NVARCHAR | 1 | YES | 'N' | 按鈕標誌 | Y:顯示按鈕, N:不顯示 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢員餐卡欄位列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employee-meal-cards/fields`
- **說明**: 查詢員餐卡欄位列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "TKey",
    "sortOrder": "DESC",
    "filters": {
      "fieldId": "",
      "fieldName": "",
      "cardType": "",
      "actionType": "",
      "otherType": "",
      "status": ""
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
          "fieldId": "FIELD001",
          "fieldName": "欄位名稱",
          "cardType": "CARD001",
          "cardTypeName": "卡片類型名稱",
          "actionType": "ACTION001",
          "actionTypeName": "動作類型名稱",
          "otherType": "OTHER001",
          "otherTypeName": "其他類型名稱",
          "mustKeyinYn": "Y",
          "readonlyYn": "N",
          "btnEtekYn": "N",
          "seqNo": 1,
          "status": "1",
          "notes": "備註",
          "createdBy": "USER001",
          "createdAt": "2024-01-01T00:00:00Z",
          "updatedBy": "USER001",
          "updatedAt": "2024-01-01T00:00:00Z"
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

#### 3.1.2 查詢單筆員餐卡欄位
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employee-meal-cards/fields/{tKey}`
- **說明**: 根據主鍵查詢單筆員餐卡欄位資料

#### 3.1.3 新增員餐卡欄位
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employee-meal-cards/fields`
- **說明**: 新增員餐卡欄位資料
- **請求格式**:
  ```json
  {
    "fieldId": "FIELD001",
    "fieldName": "欄位名稱",
    "cardType": "CARD001",
    "actionType": "ACTION001",
    "otherType": "OTHER001",
    "mustKeyinYn": "Y",
    "readonlyYn": "N",
    "btnEtekYn": "N",
    "seqNo": 1,
    "status": "1",
    "notes": "備註"
  }
  ```

#### 3.1.4 修改員餐卡欄位
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/employee-meal-cards/fields/{tKey}`
- **說明**: 修改員餐卡欄位資料
- **請求格式**: 同新增

#### 3.1.5 刪除員餐卡欄位
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/employee-meal-cards/fields/{tKey}`
- **說明**: 刪除員餐卡欄位資料

#### 3.1.6 載入上一筆名稱
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/employee-meal-cards/fields/previous/{fieldId}`
- **說明**: 載入上一筆欄位名稱

#### 3.1.7 切換Y/N值
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/employee-meal-cards/fields/{tKey}/toggle-yn`
- **說明**: 切換必填/唯讀/按鈕標誌的Y/N值
- **請求格式**:
  ```json
  {
    "fieldType": "mustKeyinYn", // mustKeyinYn, readonlyYn, btnEtekYn
    "currentValue": "Y"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `EmployeeMealCardFieldsController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/employee-meal-cards/fields")]
    [Authorize]
    public class EmployeeMealCardFieldsController : ControllerBase
    {
        private readonly IEmployeeMealCardFieldService _fieldService;
        
        public EmployeeMealCardFieldsController(IEmployeeMealCardFieldService fieldService)
        {
            _fieldService = fieldService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeeMealCardFieldDto>>>> GetFields([FromQuery] EmployeeMealCardFieldQueryDto query)
        {
            var result = await _fieldService.GetFieldsAsync(query);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpGet("{tKey}")]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> GetField(long tKey)
        {
            var result = await _fieldService.GetFieldAsync(tKey);
            if (result == null)
                return NotFound(ApiResponse.Error("資料不存在"));
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> CreateField([FromBody] CreateEmployeeMealCardFieldDto dto)
        {
            var result = await _fieldService.CreateFieldAsync(dto);
            return CreatedAtAction(nameof(GetField), new { tKey = result.TKey }, ApiResponse.Success(result));
        }
        
        [HttpPut("{tKey}")]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> UpdateField(long tKey, [FromBody] UpdateEmployeeMealCardFieldDto dto)
        {
            var result = await _fieldService.UpdateFieldAsync(tKey, dto);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpDelete("{tKey}")]
        public async Task<ActionResult<ApiResponse>> DeleteField(long tKey)
        {
            await _fieldService.DeleteFieldAsync(tKey);
            return Ok(ApiResponse.Success());
        }
        
        [HttpGet("previous/{fieldId}")]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> GetPreviousField(string fieldId)
        {
            var result = await _fieldService.GetPreviousFieldAsync(fieldId);
            return Ok(ApiResponse.Success(result));
        }
        
        [HttpPost("{tKey}/toggle-yn")]
        public async Task<ActionResult<ApiResponse<EmployeeMealCardFieldDto>>> ToggleYn(long tKey, [FromBody] ToggleYnDto dto)
        {
            var result = await _fieldService.ToggleYnAsync(tKey, dto);
            return Ok(ApiResponse.Success(result));
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 員餐卡欄位管理頁面 (`SYSL206EmployeeMealCardFields.vue`)
- **路徑**: `/business/employee-meal-cards/fields`
- **功能**: 顯示員餐卡欄位管理列表與維護功能
- **主要元件**:
  - 查詢表單 (EmployeeMealCardFieldSearchForm)
  - 資料表格 (EmployeeMealCardFieldTable)
  - 新增/修改對話框 (EmployeeMealCardFieldDialog)

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`EmployeeMealCardFieldSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <el-form-item label="欄位ID">
      <el-input v-model="searchForm.fieldId" placeholder="請輸入欄位ID" clearable />
    </el-form-item>
    <el-form-item label="欄位名稱">
      <el-input v-model="searchForm.fieldName" placeholder="請輸入欄位名稱" clearable />
    </el-form-item>
    <el-form-item label="卡片類型">
      <el-select v-model="searchForm.cardType" placeholder="請選擇卡片類型" clearable>
        <el-option v-for="card in cardTypeList" :key="card.cardId" :label="card.cardName" :value="card.cardId" />
      </el-select>
    </el-form-item>
    <el-form-item label="動作類型">
      <el-select v-model="searchForm.actionType" placeholder="請選擇動作類型" clearable>
        <el-option v-for="action in actionTypeList" :key="action.actionId" :label="action.actionName" :value="action.actionId" />
      </el-select>
    </el-form-item>
    <el-form-item label="狀態">
      <el-select v-model="searchForm.status" placeholder="請選擇狀態" clearable>
        <el-option label="啟用" value="1" />
        <el-option label="停用" value="0" />
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="handleSearch">查詢</el-button>
      <el-button @click="handleReset">重置</el-button>
      <el-button type="success" @click="handleAdd">新增</el-button>
    </el-form-item>
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`EmployeeMealCardFieldTable.vue`)
```vue
<template>
  <div>
    <el-table :data="fieldList" v-loading="loading" border>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="fieldId" label="欄位ID" width="120" />
      <el-table-column prop="fieldName" label="欄位名稱" width="200" />
      <el-table-column prop="cardTypeName" label="卡片類型" width="120" />
      <el-table-column prop="actionTypeName" label="動作類型" width="120" />
      <el-table-column prop="otherTypeName" label="其他類型" width="120" />
      <el-table-column prop="mustKeyinYn" label="必填" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.mustKeyinYn === 'Y' ? 'success' : 'info'">
            {{ row.mustKeyinYn === 'Y' ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="readonlyYn" label="唯讀" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.readonlyYn === 'Y' ? 'warning' : 'info'">
            {{ row.readonlyYn === 'Y' ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="btnEtekYn" label="按鈕" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.btnEtekYn === 'Y' ? 'success' : 'info'">
            {{ row.btnEtekYn === 'Y' ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="seqNo" label="排序" width="80" align="right" />
      <el-table-column prop="status" label="狀態" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.status === '1' ? 'success' : 'danger'">
            {{ row.status === '1' ? '啟用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
          <el-button type="info" size="small" @click="handleLoadPrevious(row)">載入上一筆</el-button>
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

---

## 五、開發任務清單

### 5.1 資料庫設計 (1天)
- [ ] 建立 EmployeeMealCardFields 資料表
- [ ] 建立 OtherType 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束

### 5.2 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 載入上一筆名稱功能
- [ ] 切換Y/N值功能
- [ ] 單元測試

### 5.3 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 新增/修改對話框開發
- [ ] 載入上一筆名稱功能
- [ ] 切換Y/N值功能
- [ ] 欄位顏色標記功能
- [ ] 元件測試

### 5.4 整合測試 (1天)
- [ ] API 整合測試
- [ ] CRUD 流程測試
- [ ] 載入上一筆名稱測試
- [ ] 切換Y/N值測試
- [ ] 權限檢查測試

**總計**: 8天

---

## 六、注意事項

### 6.1 業務邏輯
- 欄位ID必須唯一
- 載入上一筆名稱時，應根據欄位ID查找上一筆資料
- 切換Y/N值時，應即時更新資料庫
- 欄位顏色標記應根據卡片類型、動作類型、其他類型進行區分

### 6.2 UI/UX
- 欄位顏色標記應清晰易辨
- 載入上一筆名稱按鈕應放在欄位名稱旁邊
- 切換Y/N值按鈕應放在對應欄位旁邊
- 表格應支援排序與篩選

### 6.3 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 載入上一筆名稱應使用快取機制

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增員餐卡欄位成功
- [ ] 修改員餐卡欄位成功
- [ ] 刪除員餐卡欄位成功
- [ ] 查詢員餐卡欄位成功
- [ ] 載入上一筆名稱成功
- [ ] 切換Y/N值成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 載入上一筆名稱流程測試
- [ ] 切換Y/N值流程測試
- [ ] 欄位顏色標記測試
- [ ] 權限檢查測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSL000/SYSL206.ascx`
- `WEB/IMS_CORE/SYSL000/js/SYSL206.js`
- `WEB/IMS_CORE/SYSL000/style/SYSL206.css`

### 8.2 相關開發計劃
- `開發計劃/10-報表管理/01-業務報表/SYSL130-業務報表查詢作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

