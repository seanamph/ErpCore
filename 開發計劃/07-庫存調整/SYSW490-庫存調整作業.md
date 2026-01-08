# SYSW490 - 庫存調整作業_NEW 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW490
- **功能名稱**: 庫存調整作業_NEW
- **功能描述**: 提供庫存調整單的新增、修改、刪除、查詢功能，用於調整庫存數量，包含調整原因、調整數量、調整成本等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSW000/SYSW490_PR2.aspx` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW490_PR2.rpt` (報表定義)
  - `WEB/IMS_CORE/SYSW000/SYSW490_PR2.xsd` (資料架構)

### 1.2 業務需求
- 管理庫存調整單
- 支援調整原因設定
- 支援調整數量（增加/減少）
- 支援調整成本計算
- 支援調整單狀態管理（草稿、已確認、已取消）
- 支援調整單審核流程
- 支援調整單來源追蹤（盤點單、調撥單等）
- 支援多店別管理
- 支援調整單報表列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `InventoryAdjustments` (庫存調整單主檔)

```sql
CREATE TABLE [dbo].[InventoryAdjustments] (
    [AdjustmentId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 調整單號 (SP_NO)
    [AdjustmentDate] DATETIME2 NOT NULL, -- 調整日期 (SP_DATE)
    [ShopId] NVARCHAR(50) NOT NULL, -- 分店代碼 (SHOP_ID)
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'D', -- 狀態 (STATUS, D:草稿, C:已確認, X:已取消)
    [AdjustmentType] NVARCHAR(20) NULL, -- 調整類型 (SP_TYPE)
    [AdjustmentUser] NVARCHAR(50) NULL, -- 調整人員 (SP_USER)
    [Memo] NVARCHAR(500) NULL, -- 備註 (MEMO)
    [Memo2] NVARCHAR(500) NULL, -- 備註2 (MEMO2)
    [SourceNo] NVARCHAR(50) NULL, -- 來源單號 (SRC_NO)
    [SourceNum] NVARCHAR(50) NULL, -- 來源序號 (SRC_NUM)
    [SourceCheckDate] DATETIME2 NULL, -- 來源檢查日期 (SRC_CHECK_DATE)
    [SourceSuppId] NVARCHAR(50) NULL, -- 來源供應商 (SRC_SUPP_ID)
    [SiteId] NVARCHAR(50) NULL, -- 分公司代碼 (SITE_ID)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_InventoryAdjustments] PRIMARY KEY CLUSTERED ([AdjustmentId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_ShopId] ON [dbo].[InventoryAdjustments] ([ShopId]);
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_Status] ON [dbo].[InventoryAdjustments] ([Status]);
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_AdjustmentDate] ON [dbo].[InventoryAdjustments] ([AdjustmentDate]);
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustments_SourceNo] ON [dbo].[InventoryAdjustments] ([SourceNo]);
```

### 2.2 相關資料表

#### 2.2.1 `InventoryAdjustmentDetails` - 庫存調整單明細
```sql
CREATE TABLE [dbo].[InventoryAdjustmentDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [AdjustmentId] NVARCHAR(50) NOT NULL, -- 調整單號
    [LineNum] INT NOT NULL, -- 行號
    [GoodsId] NVARCHAR(50) NOT NULL, -- 商品編號 (G_ID)
    [BarcodeId] NVARCHAR(50) NULL, -- 條碼編號 (BC_ID)
    [AdjustmentQty] DECIMAL(18, 4) NOT NULL DEFAULT 0, -- 調整數量 (DIFF_QTY)
    [BeforeQty] DECIMAL(18, 4) NULL, -- 調整前數量
    [AfterQty] DECIMAL(18, 4) NULL, -- 調整後數量
    [UnitCost] DECIMAL(18, 4) NULL, -- 單位成本 (G_COST)
    [AdjustmentCost] DECIMAL(18, 4) NULL, -- 調整成本 (DIFF_COST)
    [AdjustmentAmount] DECIMAL(18, 4) NULL, -- 調整金額 (DIFF_AMT)
    [Reason] NVARCHAR(200) NULL, -- 調整原因
    [Memo] NVARCHAR(500) NULL, -- 備註
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_InventoryAdjustmentDetails_InventoryAdjustments] FOREIGN KEY ([AdjustmentId]) REFERENCES [dbo].[InventoryAdjustments] ([AdjustmentId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustmentDetails_AdjustmentId] ON [dbo].[InventoryAdjustmentDetails] ([AdjustmentId]);
CREATE NONCLUSTERED INDEX [IX_InventoryAdjustmentDetails_GoodsId] ON [dbo].[InventoryAdjustmentDetails] ([GoodsId]);
```

#### 2.2.2 `AdjustmentReasons` - 調整原因主檔
```sql
CREATE TABLE [dbo].[AdjustmentReasons] (
    [ReasonId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [ReasonName] NVARCHAR(100) NOT NULL,
    [ReasonType] NVARCHAR(20) NULL, -- 增加/減少
    [Status] NVARCHAR(10) NOT NULL DEFAULT 'A',
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

### 2.3 資料字典

#### InventoryAdjustments 資料表
| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| AdjustmentId | NVARCHAR | 50 | NO | - | 調整單號 | 主鍵 |
| AdjustmentDate | DATETIME2 | - | NO | - | 調整日期 | - |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | - |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, C:已確認, X:已取消 |
| AdjustmentType | NVARCHAR | 20 | YES | - | 調整類型 | - |
| AdjustmentUser | NVARCHAR | 50 | YES | - | 調整人員 | - |
| Memo | NVARCHAR | 500 | YES | - | 備註 | - |
| Memo2 | NVARCHAR | 500 | YES | - | 備註2 | - |
| SourceNo | NVARCHAR | 50 | YES | - | 來源單號 | - |
| SourceNum | NVARCHAR | 50 | YES | - | 來源序號 | - |
| SourceCheckDate | DATETIME2 | - | YES | - | 來源檢查日期 | - |
| SourceSuppId | NVARCHAR | 50 | YES | - | 來源供應商 | - |
| SiteId | NVARCHAR | 50 | YES | - | 分公司代碼 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢調整單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/inventory-adjustments`
- **說明**: 查詢庫存調整單列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "AdjustmentDate",
    "sortOrder": "DESC",
    "filters": {
      "adjustmentId": "",
      "shopId": "",
      "status": "",
      "adjustmentDateFrom": "",
      "adjustmentDateTo": "",
      "adjustmentUser": ""
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
          "adjustmentId": "ADJ001",
          "adjustmentDate": "2024-01-01",
          "shopId": "SHOP001",
          "shopName": "分店名稱",
          "status": "C",
          "statusName": "已確認",
          "adjustmentType": "MANUAL",
          "adjustmentUser": "U001",
          "userName": "使用者名稱",
          "totalQty": 100,
          "totalCost": 10000.00,
          "memo": "備註"
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

#### 3.1.2 查詢單筆調整單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/inventory-adjustments/{adjustmentId}`
- **說明**: 根據調整單號查詢單筆調整單資料（含明細）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "adjustmentId": "ADJ001",
      "adjustmentDate": "2024-01-01",
      "shopId": "SHOP001",
      "status": "C",
      "adjustmentType": "MANUAL",
      "adjustmentUser": "U001",
      "memo": "備註",
      "details": [
        {
          "detailId": "D001",
          "lineNum": 1,
          "goodsId": "G001",
          "goodsName": "商品名稱",
          "barcodeId": "BC001",
          "adjustmentQty": 10,
          "beforeQty": 100,
          "afterQty": 110,
          "unitCost": 100.00,
          "adjustmentCost": 1000.00,
          "reason": "盤點差異"
        }
      ]
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 新增調整單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/inventory-adjustments`
- **說明**: 新增庫存調整單
- **請求格式**:
  ```json
  {
    "adjustmentDate": "2024-01-01",
    "shopId": "SHOP001",
    "adjustmentType": "MANUAL",
    "memo": "備註",
    "sourceNo": "",
    "details": [
      {
        "goodsId": "G001",
        "barcodeId": "BC001",
        "adjustmentQty": 10,
        "reason": "盤點差異",
        "memo": "明細備註"
      }
    ]
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "adjustmentId": "ADJ001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改調整單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/inventory-adjustments/{adjustmentId}`
- **說明**: 修改庫存調整單（僅限草稿狀態）
- **請求格式**: 同新增，但 `adjustmentId` 不可修改

#### 3.1.5 刪除調整單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/inventory-adjustments/{adjustmentId}`
- **說明**: 刪除庫存調整單（僅限草稿狀態）

#### 3.1.6 確認調整單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/inventory-adjustments/{adjustmentId}/confirm`
- **說明**: 確認調整單並更新庫存
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "確認成功",
    "data": {
      "adjustmentId": "ADJ001",
      "updatedInventory": true
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.7 取消調整單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/inventory-adjustments/{adjustmentId}/cancel`
- **說明**: 取消調整單

#### 3.1.8 取得調整原因列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/inventory-adjustments/reasons`
- **說明**: 取得調整原因列表

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 調整單列表頁面 (`InventoryAdjustmentList.vue`)
- **路徑**: `/inventory/adjustments`
- **功能**: 顯示調整單列表，支援查詢、新增、修改、刪除、確認

#### 4.1.2 調整單詳細頁面 (`InventoryAdjustmentDetail.vue`)
- **路徑**: `/inventory/adjustments/:adjustmentId`
- **功能**: 顯示調整單詳細資料，支援修改、確認、取消

### 4.2 UI 元件設計

#### 4.2.1 調整單表單元件 (`InventoryAdjustmentForm.vue`)
```vue
<template>
  <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="調整單號" prop="adjustmentId">
          <el-input v-model="form.adjustmentId" :disabled="isEdit" placeholder="自動產生" />
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="調整日期" prop="adjustmentDate">
          <el-date-picker v-model="form.adjustmentDate" type="date" placeholder="請選擇日期" />
        </el-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="12">
        <el-form-item label="分店" prop="shopId">
          <el-select v-model="form.shopId" placeholder="請選擇分店">
            <el-option v-for="shop in shopList" :key="shop.shopId" :label="shop.shopName" :value="shop.shopId" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :span="12">
        <el-form-item label="調整類型" prop="adjustmentType">
          <el-select v-model="form.adjustmentType" placeholder="請選擇調整類型">
            <el-option label="手動調整" value="MANUAL" />
            <el-option label="盤點調整" value="STOCKTAKING" />
            <el-option label="調撥調整" value="TRANSFER" />
          </el-select>
        </el-form-item>
      </el-col>
    </el-row>
    <el-form-item label="備註" prop="memo">
      <el-input v-model="form.memo" type="textarea" :rows="3" placeholder="請輸入備註" />
    </el-form-item>
    <el-form-item label="明細">
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="goodsId" label="商品編號" width="120">
          <template #default="{ row, $index }">
            <el-select v-model="row.goodsId" filterable placeholder="請選擇商品" @change="handleGoodsChange($index)">
              <el-option v-for="goods in goodsList" :key="goods.goodsId" :label="goods.goodsName" :value="goods.goodsId" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column prop="barcodeId" label="條碼" width="120" />
        <el-table-column prop="beforeQty" label="調整前數量" width="120" />
        <el-table-column prop="adjustmentQty" label="調整數量" width="120">
          <template #default="{ row }">
            <el-input-number v-model="row.adjustmentQty" :precision="4" @change="handleQtyChange(row)" />
          </template>
        </el-table-column>
        <el-table-column prop="afterQty" label="調整後數量" width="120" />
        <el-table-column prop="unitCost" label="單位成本" width="120" />
        <el-table-column prop="adjustmentCost" label="調整成本" width="120" />
        <el-table-column prop="reason" label="調整原因" width="150">
          <template #default="{ row }">
            <el-select v-model="row.reason" placeholder="請選擇原因">
              <el-option v-for="reason in reasonList" :key="reason.reasonId" :label="reason.reasonName" :value="reason.reasonId" />
            </el-select>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="handleRemoveDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
    </el-form-item>
  </el-form>
</template>
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
- [ ] Service 實作（包含庫存更新邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 庫存更新邏輯
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 表單元件開發
- [ ] 明細管理功能
- [ ] 確認/取消功能
- [ ] 元件測試

### 5.4 階段四: 整合測試 (2天)
- [ ] API 整合測試
- [ ] 庫存更新測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 12天

---

## 六、注意事項

### 6.1 庫存更新
- 確認調整單時必須更新庫存
- 必須使用事務處理確保資料一致性
- 必須記錄庫存異動日誌

### 6.2 權限控制
- 僅有權限的使用者可以確認調整單
- 僅建立者或管理員可以修改/刪除調整單

### 6.3 資料驗證
- 調整數量必須驗證
- 調整後數量不能為負數
- 必須驗證商品是否存在

### 6.4 業務邏輯
- 已確認的調整單不可修改/刪除
- 取消調整單時必須還原庫存
- 必須記錄調整單異動歷史

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增調整單成功
- [ ] 修改調整單成功
- [ ] 刪除調整單成功
- [ ] 確認調整單成功
- [ ] 取消調整單成功
- [ ] 庫存更新正確

### 7.2 整合測試
- [ ] 完整調整流程測試
- [ ] 庫存更新測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試

### 7.3 效能測試
- [ ] 大量明細處理測試
- [ ] 並發確認測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/SYSW000/SYSW490_PR2.aspx`
- `WEB/IMS_CORE/SYSW000/SYSW490_PR2.rpt`
- `WEB/IMS_CORE/SYSW000/SYSW490_PR2.xsd`

### 8.2 資料庫 Schema
- `WEB/IMS_CORE/SYSW000/SYSW490_PR2.xsd`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

