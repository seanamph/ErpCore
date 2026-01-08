# MULTI_AREA_LIST - 多選區域列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: MULTI_AREA_LIST
- **功能名稱**: 多選區域列表
- **功能描述**: 提供多選區域選擇的下拉列表功能，支援區域查詢、篩選、多選等功能，用於需要選擇多個區域的業務場景
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_AREA_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/MULTI_AREA_LIST.aspx.cs`
  - `WEB/IMS_CORE/ETEK_LIST/MULTI_AREA_LIST.aspx`

### 1.2 業務需求
- 提供區域列表查詢功能
- 支援區域名稱篩選
- 支援多選區域功能
- 支援「全部區域」和「不屬於任一區域」選項
- 支援已選區域的清除功能
- 支援選擇結果回傳至父視窗
- 與區域主檔（AREA）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Areas` (區域主檔，對應舊系統 `AREA`)

```sql
CREATE TABLE [dbo].[Areas] (
    [AreaId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 區域代碼 (AREA_ID)
    [AreaName] NVARCHAR(100) NOT NULL, -- 區域名稱 (AREA_NAME)
    [SeqNo] INT NULL, -- 排序序號
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([AreaId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Areas_AreaName] ON [dbo].[Areas] ([AreaName]);
CREATE NONCLUSTERED INDEX [IX_Areas_Status] ON [dbo].[Areas] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Areas_SeqNo] ON [dbo].[Areas] ([SeqNo]);
```

### 2.2 相關資料表

無

### 2.3 資料字典

#### Areas 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| AreaId | NVARCHAR | 50 | NO | - | 區域代碼 | 主鍵 |
| AreaName | NVARCHAR | 100 | NO | - | 區域名稱 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢區域列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/areas`
- **說明**: 查詢區域列表，支援篩選、排序
- **請求參數**:
  - `areaName`: 區域名稱（模糊查詢，可選）
  - `status`: 狀態（預設為'1'）
  - `sortField`: 排序欄位（預設為'AreaId'）
  - `sortOrder`: 排序方向（ASC/DESC，預設為'ASC'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "items": [
        {
          "areaId": "A001",
          "areaName": "北區",
          "seqNo": 1,
          "status": "1"
        }
      ],
      "totalCount": 10
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆區域
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/areas/{areaId}`
- **說明**: 根據區域代碼查詢單筆區域資料
- **路徑參數**:
  - `areaId`: 區域代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "areaId": "A001",
      "areaName": "北區",
      "seqNo": 1,
      "status": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢區域選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/areas/options`
- **說明**: 取得區域選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `status`: 狀態（預設為'1'）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "A001",
        "label": "北區"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 主要元件
- **區域列表表格**: 顯示所有區域資料
- **篩選輸入框**: 支援區域名稱模糊查詢
- **選擇按鈕**: 支援多選功能
- **關閉按鈕**: 關閉視窗並回傳選擇結果

#### 4.1.2 特殊選項
- **全部區域**: 選擇所有區域（代碼: 'xx'）
- **不屬於任一區域**: 選擇不屬於任何區域的選項（代碼: 'yy'）

### 4.2 頁面佈局

```
┌─────────────────────────────────────┐
│  多選區域列表                         │
├─────────────────────────────────────┤
│  符合條件者共 X 筆                    │
├─────────────────────────────────────┤
│  [不屬於任一區域]                     │
│  [全部區域]                          │
├─────────────────────────────────────┤
│  序號 │ 區域編號 │ 區域名稱          │
├─────────────────────────────────────┤
│   1  │  A001    │ 北區              │
│   2  │  A002    │ 南區              │
│  ... │  ...     │ ...              │
└─────────────────────────────────────┘
```

### 4.3 互動功能

#### 4.3.1 多選功能
- 點擊表格列可選擇/取消選擇區域
- 已選擇的區域會標記顯示
- 支援選擇結果累積

#### 4.3.2 回傳功能
- 選擇完成後，將選擇結果回傳至父視窗的指定控制項
- 回傳格式: `區域代碼1,區域代碼2,...`
- 支援選擇結果的清除確認

#### 4.3.3 驗證功能
- 驗證選擇結果是否超過最大長度
- 驗證選擇結果格式

### 4.4 Vue 元件設計

#### 4.4.1 元件結構
```vue
<template>
  <div class="multi-area-list">
    <el-card>
      <div slot="header">
        <span>多選區域列表</span>
        <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
      </div>
      
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="請輸入區域名稱"
          clearable
          @input="handleSearch"
        />
      </div>
      
      <div class="special-options">
        <el-button @click="handleSelectAll">全部區域</el-button>
        <el-button @click="handleSelectNone">不屬於任一區域</el-button>
      </div>
      
      <el-table
        :data="areaList"
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="80" />
        <el-table-column prop="areaId" label="區域編號" width="120" />
        <el-table-column prop="areaName" label="區域名稱" />
        <el-table-column label="操作" width="100">
          <template slot-scope="scope">
            <el-checkbox
              v-model="selectedAreas"
              :label="scope.row.areaId"
              @change="handleSelectionChange"
            />
          </template>
        </el-table-column>
      </el-table>
      
      <div class="footer">
        <el-button @click="handleConfirm">確認選擇</el-button>
        <el-button @click="handleClose">關閉</el-button>
      </div>
    </el-card>
  </div>
</template>
```

#### 4.4.2 資料結構
```typescript
interface Area {
  areaId: string;
  areaName: string;
  seqNo?: number;
  status: string;
}

interface MultiAreaListData {
  areaList: Area[];
  searchText: string;
  selectedAreas: string[];
  loading: boolean;
}
```

#### 4.4.3 方法設計
```typescript
methods: {
  // 載入區域列表
  async loadAreaList(): Promise<void>;
  
  // 搜尋區域
  handleSearch(): void;
  
  // 選擇全部區域
  handleSelectAll(): void;
  
  // 選擇不屬於任一區域
  handleSelectNone(): void;
  
  // 點擊表格列
  handleRowClick(row: Area): void;
  
  // 選擇變更
  handleSelectionChange(): void;
  
  // 確認選擇
  handleConfirm(): void;
  
  // 關閉視窗
  handleClose(): void;
}
```

---

## 五、開發計劃

### 5.1 開發階段

#### 階段一：資料庫設計與建立（1天）
- [ ] 建立 `Areas` 資料表
- [ ] 建立索引
- [ ] 建立測試資料

#### 階段二：後端 API 開發（2天）
- [ ] 實作查詢區域列表 API
- [ ] 實作查詢單筆區域 API
- [ ] 實作查詢區域選項 API
- [ ] 單元測試

#### 階段三：前端 UI 開發（3天）
- [ ] 建立 Vue 元件
- [ ] 實作區域列表顯示
- [ ] 實作多選功能
- [ ] 實作搜尋功能
- [ ] 實作特殊選項（全部區域、不屬於任一區域）
- [ ] 實作回傳功能
- [ ] 單元測試

#### 階段四：整合測試（1天）
- [ ] 前後端整合測試
- [ ] 功能測試
- [ ] 效能測試

#### 階段五：文件與部署（1天）
- [ ] 更新 API 文件
- [ ] 更新使用者手冊
- [ ] 部署至測試環境

### 5.2 技術要點

#### 5.2.1 後端技術
- 使用 Dapper 進行資料庫查詢
- 使用 Entity Framework Core 進行資料驗證
- 使用 AutoMapper 進行物件對應
- 使用 Serilog 進行日誌記錄

#### 5.2.2 前端技術
- 使用 Vue 3 Composition API
- 使用 Element Plus UI 框架
- 使用 TypeScript 進行型別檢查
- 使用 Axios 進行 API 呼叫

### 5.3 注意事項

1. **多選功能**: 需要支援多個區域的選擇，並將選擇結果以逗號分隔的字串回傳
2. **特殊選項**: 「全部區域」和「不屬於任一區域」是特殊的選項，需要特別處理
3. **回傳格式**: 需要確認父視窗接收選擇結果的格式要求
4. **效能優化**: 如果區域資料量大，需要實作分頁或虛擬滾動
5. **權限控制**: 需要確認是否有權限限制

---

## 六、測試計劃

### 6.1 單元測試
- [ ] 測試區域列表查詢功能
- [ ] 測試區域篩選功能
- [ ] 測試多選功能
- [ ] 測試特殊選項功能
- [ ] 測試回傳功能

### 6.2 整合測試
- [ ] 測試前後端整合
- [ ] 測試與父視窗的互動
- [ ] 測試錯誤處理

### 6.3 效能測試
- [ ] 測試大量資料的載入效能
- [ ] 測試搜尋功能的響應時間

---

## 七、相關文件

- [區域基本資料維護作業](../02-基本資料管理/02-地區設定/SYSB450-區域基本資料維護作業.md)
- [地址區域列表](./ADDR_ZONE_LIST-地址區域列表.md)

