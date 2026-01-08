# SYSID_LIST - 系統ID列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSID_LIST
- **功能名稱**: 系統ID列表
- **功能描述**: 提供系統ID選擇的下拉列表功能，支援系統查詢、篩選、選擇等功能，用於系統選單設定、權限設定等場景的系統選擇
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/SYSID_LIST.aspx`
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/SYSID_LIST.aspx.cs`
  - `WEB/IMS_CORE/ETEK_LIST/SYSID_LIST.aspx`

### 1.2 業務需求
- 提供系統列表查詢功能
- 支援系統ID和系統名稱篩選
- 支援系統選擇並回傳系統ID
- 只顯示有選單、作業、按鈕的系統
- 排除特定系統（EIP0000、CFG0000、XCOM000，除非是xcom使用者）
- 支援狀態篩選（啟用/停用）
- 與系統主檔（MNG_SYS）、選單主檔（MNG_MENU）、作業主檔（MNG_PROG）、按鈕主檔（MNG_BUTTON）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Systems` (系統主檔，對應舊系統 `MNG_SYS`)

```sql
CREATE TABLE [dbo].[Systems] (
    [SystemId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 系統ID (SYS_ID)
    [SystemName] NVARCHAR(100) NOT NULL, -- 系統名稱 (SYS_NAME)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用, NULL:啟用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Systems] PRIMARY KEY CLUSTERED ([SystemId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Systems_SystemName] ON [dbo].[Systems] ([SystemName]);
CREATE NONCLUSTERED INDEX [IX_Systems_Status] ON [dbo].[Systems] ([Status]);
```

### 2.2 相關資料表

#### 2.2.1 `Menus` - 選單主檔
- 參考: `開發計劃/15-下拉列表/MENU_LIST-選單列表.md`

#### 2.2.2 `Programs` - 作業主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md`

#### 2.2.3 `Buttons` - 按鈕主檔
- 參考: `開發計劃/01-系統管理/04-系統設定/SYS0440-系統功能按鈕資料維護.md`

### 2.3 資料字典

#### Systems 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| SystemId | NVARCHAR | 50 | NO | - | 系統ID | 主鍵 |
| SystemName | NVARCHAR | 100 | NO | - | 系統名稱 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用, NULL:啟用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢系統列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/systems`
- **說明**: 查詢系統列表，只顯示有選單、作業、按鈕的系統，支援篩選、排序
- **請求參數**:
  - `systemId`: 系統ID（模糊查詢，可選）
  - `systemName`: 系統名稱（模糊查詢，可選）
  - `status`: 狀態（預設為'1'或NULL）
  - `excludeSystems`: 排除的系統ID列表（預設排除EIP0000、CFG0000、XCOM000，除非是xcom使用者）
  - `sortField`: 排序欄位（預設為'SystemId'）
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
          "systemId": "SYS0000",
          "systemName": "系統管理",
          "status": "1"
        }
      ],
      "totalCount": 10
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆系統
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/systems/{systemId}`
- **說明**: 根據系統ID查詢單筆系統資料
- **路徑參數**:
  - `systemId`: 系統ID
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "systemId": "SYS0000",
      "systemName": "系統管理",
      "status": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 查詢系統選項（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/systems/options`
- **說明**: 取得系統選項列表（簡化版，用於下拉選單）
- **請求參數**:
  - `status`: 狀態（預設為'1'或NULL）
  - `excludeSystems`: 排除的系統ID列表
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "value": "SYS0000",
        "label": "系統管理"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 主要元件
- **系統列表表格**: 顯示所有系統資料
- **篩選輸入框**: 支援系統ID和系統名稱模糊查詢
- **選擇功能**: 點擊表格列選擇系統
- **關閉按鈕**: 關閉視窗

### 4.2 頁面佈局

```
┌─────────────────────────────────────┐
│  系統ID列表                          │
├─────────────────────────────────────┤
│  [篩選輸入框]  [開始查詢] [關閉]     │
├─────────────────────────────────────┤
│  序號 │ 系統ID │ 系統名稱            │
├─────────────────────────────────────┤
│   1  │ SYS0000 │ 系統管理            │
│   2  │ SYSB000 │ 基本資料管理        │
│  ... │  ...   │ ...                │
└─────────────────────────────────────┘
```

### 4.3 互動功能

#### 4.3.1 選擇功能
- 點擊表格列可選擇系統
- 選擇後會導向選單列表頁面（MENU_LIST）

#### 4.3.2 回傳功能
- 選擇完成後，將選擇結果回傳至父視窗的指定控制項
- 回傳格式: `系統ID`

### 4.4 Vue 元件設計

#### 4.4.1 元件結構
```vue
<template>
  <div class="sysid-list">
    <el-card>
      <div slot="header">
        <span>系統ID列表</span>
        <el-button type="text" @click="handleClose" style="float: right;">關閉</el-button>
      </div>
      
      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="請輸入系統ID或系統名稱"
          clearable
          @keyup.enter="handleSearch"
        />
        <el-button type="primary" @click="handleSearch">開始查詢</el-button>
      </div>
      
      <el-table
        :data="systemList"
        highlight-current-row
        @row-click="handleRowClick"
        style="width: 100%"
      >
        <el-table-column type="index" label="序號" width="80" />
        <el-table-column prop="systemId" label="系統ID" width="120" />
        <el-table-column prop="systemName" label="系統名稱" />
      </el-table>
      
      <div class="footer">
        <el-button @click="handleClose">關閉</el-button>
      </div>
    </el-card>
  </div>
</template>
```

#### 4.4.2 資料結構
```typescript
interface System {
  systemId: string;
  systemName: string;
  status: string;
}

interface SysIdListData {
  systemList: System[];
  searchText: string;
  loading: boolean;
}
```

#### 4.4.3 方法設計
```typescript
methods: {
  // 載入系統列表
  async loadSystemList(): Promise<void>;
  
  // 搜尋系統
  handleSearch(): void;
  
  // 點擊表格列
  handleRowClick(row: System): void;
  
  // 關閉視窗
  handleClose(): void;
}
```

---

## 五、開發計劃

### 5.1 開發階段

#### 階段一：資料庫設計與建立（1天）
- [ ] 確認 `Systems` 資料表結構
- [ ] 確認相關資料表（Menus、Programs、Buttons）
- [ ] 建立測試資料

#### 階段二：後端 API 開發（2天）
- [ ] 實作查詢系統列表 API（包含關聯查詢邏輯）
- [ ] 實作查詢單筆系統 API
- [ ] 實作查詢系統選項 API
- [ ] 實作系統排除邏輯（EIP0000、CFG0000、XCOM000）
- [ ] 單元測試

#### 階段三：前端 UI 開發（2天）
- [ ] 建立 Vue 元件
- [ ] 實作系統列表顯示
- [ ] 實作搜尋功能
- [ ] 實作選擇功能
- [ ] 實作與選單列表的整合
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
- 使用複雜的 JOIN 查詢來過濾有選單、作業、按鈕的系統
- 使用 Entity Framework Core 進行資料驗證
- 使用 AutoMapper 進行物件對應
- 使用 Serilog 進行日誌記錄

#### 5.2.2 前端技術
- 使用 Vue 3 Composition API
- 使用 Element Plus UI 框架
- 使用 TypeScript 進行型別檢查
- 使用 Axios 進行 API 呼叫

### 5.3 注意事項

1. **系統過濾邏輯**: 只顯示有選單、作業、按鈕的系統，需要複雜的 JOIN 查詢
2. **系統排除邏輯**: 預設排除EIP0000、CFG0000、XCOM000，除非是xcom使用者
3. **狀態處理**: 狀態為NULL時視為啟用
4. **與選單列表整合**: 選擇系統後會導向選單列表頁面
5. **權限控制**: 需要確認是否有權限限制

---

## 六、測試計劃

### 6.1 單元測試
- [ ] 測試系統列表查詢功能
- [ ] 測試系統篩選功能
- [ ] 測試系統過濾邏輯（只顯示有選單、作業、按鈕的系統）
- [ ] 測試系統排除邏輯
- [ ] 測試選擇功能

### 6.2 整合測試
- [ ] 測試前後端整合
- [ ] 測試與選單列表的整合
- [ ] 測試錯誤處理

### 6.3 效能測試
- [ ] 測試大量資料的載入效能
- [ ] 測試複雜 JOIN 查詢的效能

---

## 七、相關文件

- [主系統項目資料維護](../01-系統管理/04-系統設定/SYS0410-主系統項目資料維護.md)
- [系統作業資料維護](../01-系統管理/04-系統設定/SYS0430-系統作業資料維護.md)
- [系統功能按鈕資料維護](../01-系統管理/04-系統設定/SYS0440-系統功能按鈕資料維護.md)
- [選單列表](./MENU_LIST-選單列表.md)

