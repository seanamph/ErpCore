# SYSL310 - 銷退卡管理 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSL310
- **功能名稱**: 銷退卡管理
- **功能描述**: 提供銷退卡資料的新增、修改、刪除、查詢功能，包含UUID、組織、店別等資訊管理，支援年度切換、月份切換等功能
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSL000/js/SYSL310.js` (前端邏輯)
  - `WEB/IMS_CORE/SYSL000/style/SYSL310.css` (樣式)

### 1.2 業務需求
- 管理銷退卡資料
- 支援銷退卡的新增、修改、刪除、查詢
- 支援UUID管理
- 支援組織、店別設定
- 支援年度切換功能
- 支援月份切換功能
- 記錄銷退卡的建立與變更資訊

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `ReturnCards` (對應舊系統銷退卡)

```sql
CREATE TABLE [dbo].[ReturnCards] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵
    [Uuid] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- UUID
    [SiteId] NVARCHAR(50) NOT NULL, -- 店別代碼
    [OrgId] NVARCHAR(50) NULL, -- 組織代碼
    [CardYear] INT NOT NULL, -- 卡片年度
    [CardMonth] INT NOT NULL, -- 卡片月份
    [CardType] NVARCHAR(20) NULL, -- 卡片類型
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態
    [Notes] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間
    CONSTRAINT [PK_ReturnCards] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_ReturnCards_Uuid] UNIQUE ([Uuid])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_ReturnCards_SiteId] ON [dbo].[ReturnCards] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_ReturnCards_OrgId] ON [dbo].[ReturnCards] ([OrgId]);
CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardYear] ON [dbo].[ReturnCards] ([CardYear]);
CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardMonth] ON [dbo].[ReturnCards] ([CardMonth]);
CREATE NONCLUSTERED INDEX [IX_ReturnCards_CardYear_CardMonth] ON [dbo].[ReturnCards] ([CardYear], [CardMonth]);
```

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢銷退卡列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/return-cards`
- **說明**: 查詢銷退卡列表
- **請求參數**:
  ```json
  {
    "siteId": "",
    "orgId": "",
    "cardYear": 2024,
    "cardMonth": 1,
    "pageIndex": 1,
    "pageSize": 20
  }
  ```

#### 3.1.2 查詢單筆銷退卡
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/return-cards/{uuid}`
- **說明**: 根據UUID查詢單筆銷退卡資料

#### 3.1.3 新增銷退卡
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/return-cards`
- **說明**: 新增銷退卡資料

#### 3.1.4 修改銷退卡
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/return-cards/{uuid}`
- **說明**: 修改銷退卡資料

#### 3.1.5 刪除銷退卡
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/return-cards/{uuid}`
- **說明**: 刪除銷退卡資料

---

## 四、前端 UI 設計

### 4.1 UI 元件設計

#### 4.1.1 銷退卡管理頁面 (`ReturnCardManagement.vue`)
```vue
<template>
  <div class="return-card-management">
    <el-form :model="queryForm" :rules="rules" ref="queryFormRef" label-width="120px" inline>
      <el-form-item label="店別" prop="siteId">
        <el-select v-model="queryForm.siteId" placeholder="請選擇" clearable filterable>
          <el-option 
            v-for="item in siteList" 
            :key="item.siteId" 
            :label="item.siteName" 
            :value="item.siteId" 
          />
        </el-select>
      </el-form-item>
      <el-form-item label="組織" prop="orgId">
        <el-select v-model="queryForm.orgId" placeholder="請選擇" clearable filterable>
          <el-option 
            v-for="item in orgList" 
            :key="item.orgId" 
            :label="item.orgName" 
            :value="item.orgId" 
          />
        </el-select>
      </el-form-item>
      <el-form-item label="年度" prop="cardYear">
        <el-date-picker
          v-model="queryForm.cardYear"
          type="year"
          placeholder="請選擇年度"
          format="YYYY"
          value-format="YYYY"
        />
      </el-form-item>
      <el-form-item label="月份" prop="cardMonth">
        <el-select v-model="queryForm.cardMonth" placeholder="請選擇" clearable>
          <el-option 
            v-for="month in 12" 
            :key="month" 
            :label="month + '月'" 
            :value="month" 
          />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查詢</el-button>
        <el-button @click="handleReset">重置</el-button>
        <el-button type="success" @click="handleAdd">新增</el-button>
      </el-form-item>
    </el-form>
    
    <el-table :data="cardData" border stripe>
      <el-table-column prop="uuid" label="UUID" width="250" />
      <el-table-column prop="siteName" label="店別" width="120" />
      <el-table-column prop="orgName" label="組織" width="120" />
      <el-table-column prop="cardYear" label="年度" width="80" />
      <el-table-column prop="cardMonth" label="月份" width="80" />
      <el-table-column prop="cardType" label="卡片類型" width="120" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" size="small" @click="handleEdit(row)">修改</el-button>
          <el-button type="danger" size="small" @click="handleDelete(row)">刪除</el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { returnCardApi } from '@/api/business.api';

const queryFormRef = ref();
const queryForm = reactive({
  siteId: '',
  orgId: '',
  cardYear: new Date().getFullYear(),
  cardMonth: null
});

const cardData = ref([]);
const siteList = ref([]);
const orgList = ref([]);

const handleQuery = async () => {
  // 查詢邏輯
};

const handleReset = () => {
  queryFormRef.value.resetFields();
};

const handleAdd = () => {
  // 新增邏輯
};

const handleEdit = (row: any) => {
  // 修改邏輯
};

const handleDelete = async (row: any) => {
  // 刪除邏輯
};

onMounted(() => {
  loadData();
  loadSiteList();
  loadOrgList();
});
</script>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立 ReturnCards 資料表
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 銷退卡管理頁面開發
- [ ] 資料表格開發
- [ ] 表單驗證
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
- UUID需自動產生
- 年度、月份需驗證
- 組織、店別需驗證

### 6.2 效能優化
- 大量資料查詢需使用分頁
- 查詢需使用適當索引

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增銷退卡成功
- [ ] 修改銷退卡成功
- [ ] 刪除銷退卡成功
- [ ] 查詢銷退卡成功
- [ ] UUID驗證測試

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 資料驗證測試
- [ ] 年度、月份切換測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSL000/js/SYSL310.js`
- `WEB/IMS_CORE/SYSL000/style/SYSL310.css`

### 8.2 相關功能
- `SYSL206-員餐卡管理.md` - 類似功能

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

