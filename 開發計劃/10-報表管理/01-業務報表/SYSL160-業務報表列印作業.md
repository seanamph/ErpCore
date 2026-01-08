# SYSL160 - 業務報表列印作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL160
- **功能名稱**: 業務報表列印作業
- **功能描述**: 提供業務報表列印功能，包含請假扣款、動作事件、扣款數量等資訊管理，支援批次處理、資料驗證等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/js/SYSL160.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL160.css` (樣式)

### 1.2 業務需求
- 管理業務報表列印資料
- 支援請假扣款功能
- 支援動作事件設定
- 支援扣款數量計算
- 支援批次處理功能
- 支援資料驗證
- 記錄業務報表的建立與變更資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `BusinessReportPrintDetail` (對應舊系統業務報表列印明細)

```sql
CREATE TABLE [dbo].[BusinessReportPrintDetail] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [PrintId] BIGINT NOT NULL, -- 報表列印ID (外鍵至BusinessReportPrint)
    [LeaveId] NVARCHAR(50) NULL, -- 請假代碼 (LEAVE_ID)
    [LeaveName] NVARCHAR(100) NULL, -- 請假名稱 (LEAVE_NAME)
    [ActEvent] NVARCHAR(50) NULL, -- 動作事件 (ACT_EVENT)
    [DeductionQty] DECIMAL(18,2) NULL DEFAULT 0, -- 扣款數量 (DEDUCTION_QTY)
    [DeductionQtyDefaultEmpty] NVARCHAR(1) NULL DEFAULT 'N', -- 扣款數量預設為空 (DEDUCTION_QTY_DEFAULT_EMPTY)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [FK_BusinessReportPrintDetail_BusinessReportPrint] FOREIGN KEY ([PrintId]) REFERENCES [dbo].[BusinessReportPrint] ([TKey]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_PrintId] ON [dbo].[BusinessReportPrintDetail] ([PrintId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_LeaveId] ON [dbo].[BusinessReportPrintDetail] ([LeaveId]);
CREATE NONCLUSTERED INDEX [IX_BusinessReportPrintDetail_ActEvent] ON [dbo].[BusinessReportPrintDetail] ([ActEvent]);
```

### 2.2 相關資料表

#### 2.2.1 `BusinessReportPrint` - 業務報表列印主檔
- 參考: `開發計劃/10-報表管理/01-業務報表/SYSL150-業務報表列印作業.md` 的 `BusinessReportPrint` 資料表結構

#### 2.2.2 `LeaveTypes` - 請假類型主檔
```sql
CREATE TABLE [dbo].[LeaveTypes] (
    [LeaveId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [LeaveName] NVARCHAR(100) NOT NULL,
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢業務報表列印明細
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/business-reports/print-details`
- **說明**: 查詢業務報表列印明細資料
- **請求參數**:
  ```json
  {
    "printId": 1,
    "leaveId": "",
    "actEvent": "",
    "pageIndex": 1,
    "pageSize": 20
  }
  ```

#### 3.1.2 新增業務報表列印明細
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/print-details`
- **說明**: 新增業務報表列印明細
- **請求格式**:
  ```json
  {
    "printId": 1,
    "leaveId": "L001",
    "leaveName": "請假類型",
    "actEvent": "EVENT001",
    "deductionQty": 1.0,
    "deductionQtyDefaultEmpty": "N"
  }
  ```

#### 3.1.3 修改業務報表列印明細
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/business-reports/print-details/{id}`
- **說明**: 修改業務報表列印明細

#### 3.1.4 刪除業務報表列印明細
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/business-reports/print-details/{id}`
- **說明**: 刪除業務報表列印明細

#### 3.1.5 批次處理業務報表列印明細
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/business-reports/print-details/batch`
- **說明**: 批次處理業務報表列印明細（新增、修改、刪除）

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 業務報表列印明細維護頁面 (`BusinessReportPrintDetail.vue`)
```vue
<template>
  <div class="business-report-print-detail">
    <el-table :data="detailData" border stripe>
      <el-table-column type="selection" width="55" />
      <el-table-column prop="leaveId" label="請假代碼" width="120">
        <template #default="{ row }">
          <el-select v-model="row.leaveId" placeholder="請選擇" clearable filterable>
            <el-option 
              v-for="item in leaveTypeList" 
              :key="item.leaveId" 
              :label="item.leaveName" 
              :value="item.leaveId" 
            />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="leaveName" label="請假名稱" width="150" />
      <el-table-column prop="actEvent" label="動作事件" width="150">
        <template #default="{ row }">
          <el-input v-model="row.actEvent" placeholder="請輸入動作事件" />
        </template>
      </el-table-column>
      <el-table-column prop="deductionQty" label="扣款數量" width="120">
        <template #default="{ row }">
          <el-input-number 
            v-model="row.deductionQty" 
            :precision="2" 
            :min="0"
            :disabled="!row.actEvent || row.deductionQtyDefaultEmpty === 'Y'"
          />
        </template>
      </el-table-column>
      <el-table-column prop="deductionQtyDefaultEmpty" label="扣款數量預設為空" width="150">
        <template #default="{ row }">
          <el-switch 
            v-model="row.deductionQtyDefaultEmpty" 
            active-value="Y" 
            inactive-value="N"
            @change="handleDeductionQtyDefaultEmptyChange(row)"
          />
        </template>
      </el-table-column>
    </el-table>
    <el-button type="primary" @click="handleSave">儲存</el-button>
    <el-button @click="handleAdd">新增</el-button>
    <el-button type="danger" @click="handleDelete">刪除</el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { businessReportApi } from '@/api/business.api';

const detailData = ref([]);
const leaveTypeList = ref([]);

const handleDeductionQtyDefaultEmptyChange = (row: any) => {
  if (row.deductionQtyDefaultEmpty === 'Y') {
    row.deductionQty = null;
  }
};

const handleSave = async () => {
  // 儲存邏輯
};

const handleAdd = () => {
  detailData.value.push({
    leaveId: '',
    leaveName: '',
    actEvent: '',
    deductionQty: 0,
    deductionQtyDefaultEmpty: 'N'
  });
};

const handleDelete = async () => {
  // 刪除邏輯
};

onMounted(() => {
  loadData();
  loadLeaveTypeList();
});
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 BusinessReportPrintDetail 資料表
- [ ] 建立 LeaveTypes 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 批次處理邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 業務報表列印明細維護頁面開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 批次處理功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 9天

---

## 六、注意事項

### 6.1 業務邏輯
- 扣款數量需在動作事件有值時才能輸入
- 扣款數量預設為空時，扣款數量欄位應為空
- 批次處理需驗證資料完整性

### 6.2 效能優化
- 大量資料處理需使用批次處理
- 查詢需使用適當索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增業務報表列印明細成功
- [ ] 修改業務報表列印明細成功
- [ ] 刪除業務報表列印明細成功
- [ ] 批次處理成功
- [ ] 資料驗證測試

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 批次處理流程測試
- [ ] 資料驗證測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSL000/js/SYSL160.js`
- `WEB/IMS_CORE/SYSL000/style/SYSL160.css`

### 8.2 相關功能
- `SYSL150-業務報表列印作業.md` - 業務報表列印主檔功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

