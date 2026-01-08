# SYSN210 - 水費轉檔維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN210
- **功能名稱**: 水費轉檔維護作業
- **功能描述**: 提供水費轉檔資料的新增、修改、刪除、查詢功能，包含樓別、戶號、租戶編號、資料所屬年月、費用期間、用水度數、每度金額、水費金額、公共區域水費、其他水費、總計水費等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FB.ASP` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FI.ASP` (新增)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FU.ASP` (修改)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FD.ASP` (刪除)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_FQ.ASP` (查詢)
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN210_PR.ASP` (報表)

### 1.2 業務需求
- 管理水費轉檔資料
- 支援水費資料的新增、修改、刪除、查詢
- 記錄水費資料的建立與變更資訊
- 支援資料所屬年月結帳檢查（已結帳不得新增或修改）
- 支援費用期間設定
- 支援用水度數、每度金額、水費金額等計算
- 支援公共區域水費、其他水費、總計水費計算
- 與會計系統整合（檢查是否已結帳）

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `WaterBills` (對應舊系統 `WATER_LIST`)

```sql
CREATE TABLE [dbo].[WaterBills] (
    [TKey] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 主鍵 (T_KEY)
    [FloorId] NVARCHAR(3) NOT NULL, -- 樓別 (FLOOR_ID)
    [TenantNo] NVARCHAR(2) NOT NULL, -- 戶號 (TENANT_NO)
    [StoreId] NVARCHAR(6) NOT NULL, -- 租戶編號 (STORE_ID)
    [DataYm] NVARCHAR(6) NOT NULL, -- 資料所屬年月 (DATA_YM) 格式: YYYYMM
    [PeriodDateB] DATE NOT NULL, -- 費用期間(起) (PERIOD_DATE_B)
    [PeriodDateE] DATE NOT NULL, -- 費用期間(迄) (PERIOD_DATE_E)
    [UsedDegNum] DECIMAL(10,2) NULL DEFAULT 0, -- 用水度數 (USED_DEG_NUM)
    [PerDegAmt] DECIMAL(10,2) NULL DEFAULT 0, -- 每度金額 (PER_DEG_AMT)
    [WaterAmt] DECIMAL(10,2) NULL DEFAULT 0, -- 水費金額 (WATER_AMT)
    [PublicAmt] DECIMAL(10,2) NULL DEFAULT 0, -- 公共區域水費 (PUBLIC_AMT)
    [OtherCharge] DECIMAL(10,2) NULL DEFAULT 0, -- 其他水費 (OTHER_CHARGE)
    [TotalAmt] DECIMAL(10,2) NULL DEFAULT 0, -- 總計水費 (TOTAL_AMT)
    [CheckYn] NVARCHAR(1) NULL DEFAULT 'N', -- 檢查標誌 (CHECK_YN) Y:已檢查, N:未檢查
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_WaterBills] PRIMARY KEY CLUSTERED ([TKey] ASC),
    CONSTRAINT [UQ_WaterBills_Unique] UNIQUE ([FloorId], [TenantNo], [StoreId], [PeriodDateB], [PeriodDateE])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_WaterBills_FloorId] ON [dbo].[WaterBills] ([FloorId]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_TenantNo] ON [dbo].[WaterBills] ([TenantNo]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_StoreId] ON [dbo].[WaterBills] ([StoreId]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_DataYm] ON [dbo].[WaterBills] ([DataYm]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_PeriodDateB] ON [dbo].[WaterBills] ([PeriodDateB]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_PeriodDateE] ON [dbo].[WaterBills] ([PeriodDateE]);
CREATE NONCLUSTERED INDEX [IX_WaterBills_CreatedAt] ON [dbo].[WaterBills] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `VoucherTempM` - 會計憑證暫存表
- 用於檢查資料所屬年月是否已結帳
- 參考: `VOU_TMP_M` (ACCT_YM, VOUCHER_TYPE='TO')

#### 2.2.2 `PresentPotential` - 租戶主檔
- 用於查詢租戶編號和租戶名稱
- 參考: `PRESENT_POTENTIAL` (STORE_ID, VENDOR_NAME)

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| TKey | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| FloorId | NVARCHAR | 3 | NO | - | 樓別 | - |
| TenantNo | NVARCHAR | 2 | NO | - | 戶號 | - |
| StoreId | NVARCHAR | 6 | NO | - | 租戶編號 | 外鍵至租戶主檔 |
| DataYm | NVARCHAR | 6 | NO | - | 資料所屬年月 | 格式: YYYYMM |
| PeriodDateB | DATE | - | NO | - | 費用期間(起) | - |
| PeriodDateE | DATE | - | NO | - | 費用期間(迄) | - |
| UsedDegNum | DECIMAL | 10,2 | YES | 0 | 用水度數 | - |
| PerDegAmt | DECIMAL | 10,2 | YES | 0 | 每度金額 | - |
| WaterAmt | DECIMAL | 10,2 | YES | 0 | 水費金額 | - |
| PublicAmt | DECIMAL | 10,2 | YES | 0 | 公共區域水費 | - |
| OtherCharge | DECIMAL | 10,2 | YES | 0 | 其他水費 | - |
| TotalAmt | DECIMAL | 10,2 | YES | 0 | 總計水費 | - |
| CheckYn | NVARCHAR | 1 | YES | 'N' | 檢查標誌 | Y:已檢查, N:未檢查 |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢水費轉檔列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/water-bills`
- **說明**: 查詢水費轉檔列表，支援分頁、排序、篩選
- **請求參數**:
  - `pageIndex`: 頁碼 (預設: 1)
  - `pageSize`: 每頁筆數 (預設: 20)
  - `floorId`: 樓別 (模糊查詢)
  - `tenantNo`: 戶號 (模糊查詢)
  - `storeId`: 租戶編號 (模糊查詢)
  - `dataYm`: 資料所屬年月 (模糊查詢)
  - `periodDateBFrom`: 費用期間(起) 起始日期
  - `periodDateBTo`: 費用期間(起) 結束日期
  - `periodDateEFrom`: 費用期間(迄) 起始日期
  - `periodDateETo`: 費用期間(迄) 結束日期
  - `usedDegNumFrom`: 用水度數 起始值
  - `usedDegNumTo`: 用水度數 結束值
  - `perDegAmtFrom`: 每度金額 起始值
  - `perDegAmtTo`: 每度金額 結束值
  - `waterAmtFrom`: 水費金額 起始值
  - `waterAmtTo`: 水費金額 結束值
  - `publicAmtFrom`: 公共區域水費 起始值
  - `publicAmtTo`: 公共區域水費 結束值
  - `otherChargeFrom`: 其他水費 起始值
  - `otherChargeTo`: 其他水費 結束值
  - `totalAmtFrom`: 總計水費 起始值
  - `totalAmtTo`: 總計水費 結束值
  - `sortBy`: 排序欄位 (預設: CreatedAt)
  - `sortOrder`: 排序方向 (ASC/DESC, 預設: DESC)
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
          "floorId": "001",
          "tenantNo": "01",
          "storeId": "ST001",
          "storeName": "租戶名稱",
          "dataYm": "202401",
          "periodDateB": "2024-01-01",
          "periodDateE": "2024-01-31",
          "usedDegNum": 100.00,
          "perDegAmt": 10.00,
          "waterAmt": 1000.00,
          "publicAmt": 200.00,
          "otherCharge": 50.00,
          "totalAmt": 1250.00,
          "checkYn": "N",
          "notes": "備註",
          "createdBy": "U001",
          "createdAt": "2024-01-01T00:00:00Z",
          "updatedBy": "U001",
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

#### 3.1.2 查詢單筆水費轉檔
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/water-bills/{tKey}`
- **說明**: 根據主鍵查詢單筆水費轉檔資料
- **路徑參數**:
  - `tKey`: 主鍵
- **回應格式**: 同查詢列表的單筆資料格式

#### 3.1.3 新增水費轉檔
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/water-bills`
- **說明**: 新增水費轉檔資料
- **請求格式**:
  ```json
  {
    "floorId": "001",
    "tenantNo": "01",
    "storeId": "ST001",
    "dataYm": "202401",
    "periodDateB": "2024-01-01",
    "periodDateE": "2024-01-31",
    "usedDegNum": 100.00,
    "perDegAmt": 10.00,
    "waterAmt": 1000.00,
    "publicAmt": 200.00,
    "otherCharge": 50.00,
    "totalAmt": 1250.00,
    "checkYn": "N",
    "notes": "備註"
  }
  ```
- **業務規則**:
  - 檢查資料所屬年月是否已結帳（查詢 VoucherTempM 表，ACCT_YM = dataYm, VOUCHER_TYPE = 'TO'），已結帳不得新增
  - 檢查樓別、戶號、租戶編號、費用期間組合是否已存在，已存在不得新增
  - 自動計算總計水費 = 水費金額 + 公共區域水費 + 其他水費
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "tKey": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.4 修改水費轉檔
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/water-bills/{tKey}`
- **說明**: 修改水費轉檔資料
- **路徑參數**:
  - `tKey`: 主鍵
- **請求格式**: 同新增，但 `tKey` 不可修改
- **業務規則**:
  - 檢查資料所屬年月是否已結帳，已結帳不得修改
  - 檢查樓別、戶號、租戶編號、費用期間組合是否已存在（排除自己），已存在不得修改
  - 自動計算總計水費
- **回應格式**: 同新增

#### 3.1.5 刪除水費轉檔
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/water-bills/{tKey}`
- **說明**: 刪除水費轉檔資料
- **路徑參數**:
  - `tKey`: 主鍵
- **業務規則**:
  - 檢查資料所屬年月是否已結帳，已結帳不得刪除
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "刪除成功",
    "data": null,
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.6 批次刪除水費轉檔
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/water-bills/batch`
- **說明**: 批次刪除多筆水費轉檔
- **請求格式**:
  ```json
  {
    "tKeys": [1, 2, 3]
  }
  ```

#### 3.1.7 檢查資料所屬年月是否已結帳
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/water-bills/check-settled/{dataYm}`
- **說明**: 檢查指定年月是否已結帳
- **路徑參數**:
  - `dataYm`: 資料所屬年月 (格式: YYYYMM)
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "dataYm": "202401",
      "isSettled": true,
      "settledDate": "2024-02-01T00:00:00Z"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.8 取得租戶列表（用於下拉選單）
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/water-bills/stores`
- **說明**: 取得租戶列表，用於下拉選單
- **請求參數**:
  - `keyword`: 關鍵字（租戶編號或名稱）
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": [
      {
        "storeId": "ST001",
        "storeName": "租戶名稱"
      }
    ],
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 查詢頁面 (`SYSN210Query.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="sysn210-query">
    <el-card>
      <template #header>
        <span>水費轉檔維護作業 - 查詢</span>
      </template>
      
      <el-form :model="queryForm" ref="queryFormRef" label-width="150px" inline>
        <!-- 樓別 -->
        <el-form-item label="樓別">
          <el-input 
            v-model="queryForm.floorId" 
            placeholder="請輸入樓別"
            clearable
            style="width: 120px"
            maxlength="3"
          />
        </el-form-item>
        
        <!-- 戶號 -->
        <el-form-item label="戶號">
          <el-input 
            v-model="queryForm.tenantNo" 
            placeholder="請輸入戶號"
            clearable
            style="width: 120px"
            maxlength="2"
          />
        </el-form-item>
        
        <!-- 租戶編號 -->
        <el-form-item label="租戶編號">
          <el-input 
            v-model="queryForm.storeId" 
            placeholder="請輸入租戶編號"
            clearable
            style="width: 150px"
            maxlength="6"
          />
          <el-button type="primary" link @click="handleSelectStore">選擇</el-button>
        </el-form-item>
        
        <!-- 資料所屬年月 -->
        <el-form-item label="資料所屬年月">
          <el-date-picker
            v-model="queryForm.dataYm"
            type="month"
            placeholder="請選擇年月"
            format="YYYYMM"
            value-format="YYYYMM"
            style="width: 150px"
          />
        </el-form-item>
        
        <!-- 費用期間(起) -->
        <el-form-item label="費用期間(起)">
          <el-date-picker
            v-model="queryForm.periodDateBFrom"
            type="date"
            placeholder="起始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 150px"
          />
          <span style="margin: 0 10px">~</span>
          <el-date-picker
            v-model="queryForm.periodDateBTo"
            type="date"
            placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 150px"
          />
        </el-form-item>
        
        <!-- 費用期間(迄) -->
        <el-form-item label="費用期間(迄)">
          <el-date-picker
            v-model="queryForm.periodDateEFrom"
            type="date"
            placeholder="起始日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 150px"
          />
          <span style="margin: 0 10px">~</span>
          <el-date-picker
            v-model="queryForm.periodDateETo"
            type="date"
            placeholder="結束日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 150px"
          />
        </el-form-item>
        
        <!-- 用水度數 -->
        <el-form-item label="用水度數">
          <el-input-number
            v-model="queryForm.usedDegNumFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.usedDegNumTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 每度金額 -->
        <el-form-item label="每度金額">
          <el-input-number
            v-model="queryForm.perDegAmtFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.perDegAmtTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 水費金額 -->
        <el-form-item label="水費金額">
          <el-input-number
            v-model="queryForm.waterAmtFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.waterAmtTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 公共區域水費 -->
        <el-form-item label="公共區域水費">
          <el-input-number
            v-model="queryForm.publicAmtFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.publicAmtTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 其他水費 -->
        <el-form-item label="其他水費">
          <el-input-number
            v-model="queryForm.otherChargeFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.otherChargeTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 總計水費 -->
        <el-form-item label="總計水費">
          <el-input-number
            v-model="queryForm.totalAmtFrom"
            :precision="2"
            :min="0"
            placeholder="起始值"
            style="width: 120px"
          />
          <span style="margin: 0 10px">~</span>
          <el-input-number
            v-model="queryForm.totalAmtTo"
            :precision="2"
            :min="0"
            placeholder="結束值"
            style="width: 120px"
          />
        </el-form-item>
        
        <!-- 查詢按鈕 -->
        <el-form-item>
          <el-button type="primary" icon="Search" @click="handleSearch">查詢</el-button>
          <el-button icon="Refresh" @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    
    <!-- 查詢結果 -->
    <el-card v-if="searchResult.length > 0" style="margin-top: 20px">
      <template #header>
        <div style="display: flex; justify-content: space-between; align-items: center">
          <span>查詢結果 (共 {{ totalCount }} 筆)</span>
          <div>
            <el-button type="primary" icon="Plus" @click="handleAdd">新增</el-button>
            <el-button type="danger" icon="Delete" @click="handleBatchDelete" :disabled="selectedRows.length === 0">批次刪除</el-button>
            <el-button type="success" icon="Printer" @click="handlePrint">列印</el-button>
          </div>
        </div>
      </template>
      
      <el-table 
        :data="searchResult" 
        border 
        stripe 
        style="width: 100%"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column type="index" label="序號" width="60" align="center" />
        <el-table-column prop="floorId" label="樓別" width="80" sortable />
        <el-table-column prop="tenantNo" label="戶號" width="80" sortable />
        <el-table-column prop="storeId" label="租戶編號" width="120" sortable />
        <el-table-column prop="storeName" label="租戶名稱" width="200" />
        <el-table-column prop="dataYm" label="資料所屬年月" width="120" sortable />
        <el-table-column prop="periodDateB" label="費用期間(起)" width="120" sortable />
        <el-table-column prop="periodDateE" label="費用期間(迄)" width="120" sortable />
        <el-table-column prop="usedDegNum" label="用水度數" width="100" align="right" sortable>
          <template #default="scope">
            {{ formatNumber(scope.row.usedDegNum) }}
          </template>
        </el-table-column>
        <el-table-column prop="perDegAmt" label="每度金額" width="100" align="right" sortable>
          <template #default="scope">
            {{ formatCurrency(scope.row.perDegAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="waterAmt" label="水費金額" width="120" align="right" sortable>
          <template #default="scope">
            {{ formatCurrency(scope.row.waterAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="publicAmt" label="公共區域水費" width="140" align="right" sortable>
          <template #default="scope">
            {{ formatCurrency(scope.row.publicAmt) }}
          </template>
        </el-table-column>
        <el-table-column prop="otherCharge" label="其他水費" width="120" align="right" sortable>
          <template #default="scope">
            {{ formatCurrency(scope.row.otherCharge) }}
          </template>
        </el-table-column>
        <el-table-column prop="totalAmt" label="總計水費" width="120" align="right" sortable>
          <template #default="scope">
            <strong>{{ formatCurrency(scope.row.totalAmt) }}</strong>
          </template>
        </el-table-column>
        <el-table-column prop="checkYn" label="檢查標誌" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.checkYn === 'Y' ? 'success' : 'info'">
              {{ scope.row.checkYn === 'Y' ? '已檢查' : '未檢查' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" align="center" fixed="right">
          <template #default="scope">
            <el-button type="primary" link @click="handleEdit(scope.row)">修改</el-button>
            <el-button type="danger" link @click="handleDelete(scope.row)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分頁 -->
      <el-pagination
        v-model:current-page="pagination.pageIndex"
        v-model:page-size="pagination.pageSize"
        :total="pagination.totalCount"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { waterBillApi } from '@/api/water-bill'
import type { WaterBill, WaterBillQueryParams } from '@/types/water-bill'

// 查詢表單
const queryForm = reactive<WaterBillQueryParams>({
  floorId: '',
  tenantNo: '',
  storeId: '',
  dataYm: '',
  periodDateBFrom: '',
  periodDateBTo: '',
  periodDateEFrom: '',
  periodDateETo: '',
  usedDegNumFrom: undefined,
  usedDegNumTo: undefined,
  perDegAmtFrom: undefined,
  perDegAmtTo: undefined,
  waterAmtFrom: undefined,
  waterAmtTo: undefined,
  publicAmtFrom: undefined,
  publicAmtTo: undefined,
  otherChargeFrom: undefined,
  otherChargeTo: undefined,
  totalAmtFrom: undefined,
  totalAmtTo: undefined,
  pageIndex: 1,
  pageSize: 20,
  sortBy: 'CreatedAt',
  sortOrder: 'DESC'
})

// 查詢結果
const searchResult = ref<WaterBill[]>([])
const totalCount = ref(0)
const selectedRows = ref<WaterBill[]>([])

// 分頁
const pagination = reactive({
  pageIndex: 1,
  pageSize: 20,
  totalCount: 0
})

// 查詢
const handleSearch = async () => {
  try {
    const response = await waterBillApi.getList(queryForm)
    if (response.success) {
      searchResult.value = response.data.items
      pagination.totalCount = response.data.totalCount
      pagination.pageIndex = response.data.pageIndex
      pagination.pageSize = response.data.pageSize
    }
  } catch (error) {
    ElMessage.error('查詢失敗')
  }
}

// 重置
const handleReset = () => {
  Object.assign(queryForm, {
    floorId: '',
    tenantNo: '',
    storeId: '',
    dataYm: '',
    periodDateBFrom: '',
    periodDateBTo: '',
    periodDateEFrom: '',
    periodDateETo: '',
    usedDegNumFrom: undefined,
    usedDegNumTo: undefined,
    perDegAmtFrom: undefined,
    perDegAmtTo: undefined,
    waterAmtFrom: undefined,
    waterAmtTo: undefined,
    publicAmtFrom: undefined,
    publicAmtTo: undefined,
    otherChargeFrom: undefined,
    otherChargeTo: undefined,
    totalAmtFrom: undefined,
    totalAmtTo: undefined,
    pageIndex: 1,
    pageSize: 20,
    sortBy: 'CreatedAt',
    sortOrder: 'DESC'
  })
  searchResult.value = []
  selectedRows.value = []
}

// 選擇租戶
const handleSelectStore = () => {
  // 開啟租戶選擇視窗
}

// 新增
const handleAdd = () => {
  router.push('/water-bills/add')
}

// 修改
const handleEdit = (row: WaterBill) => {
  router.push(`/water-bills/edit/${row.tKey}`)
}

// 刪除
const handleDelete = async (row: WaterBill) => {
  try {
    await ElMessageBox.confirm('確定要刪除此筆資料嗎？', '確認刪除', {
      type: 'warning'
    })
    const response = await waterBillApi.delete(row.tKey)
    if (response.success) {
      ElMessage.success('刪除成功')
      handleSearch()
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

// 批次刪除
const handleBatchDelete = async () => {
  try {
    await ElMessageBox.confirm(`確定要刪除選取的 ${selectedRows.value.length} 筆資料嗎？`, '確認刪除', {
      type: 'warning'
    })
    const tKeys = selectedRows.value.map(row => row.tKey)
    const response = await waterBillApi.batchDelete({ tKeys })
    if (response.success) {
      ElMessage.success('批次刪除成功')
      handleSearch()
    }
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('批次刪除失敗')
    }
  }
}

// 列印
const handlePrint = () => {
  // 開啟列印視窗
}

// 選擇變更
const handleSelectionChange = (selection: WaterBill[]) => {
  selectedRows.value = selection
}

// 分頁變更
const handleSizeChange = (size: number) => {
  queryForm.pageSize = size
  handleSearch()
}

const handlePageChange = (page: number) => {
  queryForm.pageIndex = page
  handleSearch()
}

// 格式化數字
const formatNumber = (value: number | null) => {
  if (value === null || value === undefined) return '0.00'
  return value.toFixed(2)
}

// 格式化貨幣
const formatCurrency = (value: number | null) => {
  if (value === null || value === undefined) return '$0.00'
  return `$${value.toLocaleString('zh-TW', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
}

onMounted(() => {
  // 初始化
})
</script>
```

### 4.2 新增/修改頁面 (`SYSN210Form.vue`)

#### 4.2.1 頁面結構
```vue
<template>
  <div class="sysn210-form">
    <el-card>
      <template #header>
        <span>{{ isEdit ? '水費轉檔維護作業 - 修改' : '水費轉檔維護作業 - 新增' }}</span>
      </template>
      
      <el-form 
        :model="formData" 
        :rules="formRules" 
        ref="formRef" 
        label-width="150px"
      >
        <!-- 樓別 -->
        <el-form-item label="樓別" prop="floorId">
          <el-input 
            v-model="formData.floorId" 
            placeholder="請輸入樓別"
            maxlength="3"
            style="width: 200px"
            :disabled="isEdit"
          />
        </el-form-item>
        
        <!-- 戶號 -->
        <el-form-item label="戶號" prop="tenantNo">
          <el-input 
            v-model="formData.tenantNo" 
            placeholder="請輸入戶號"
            maxlength="2"
            style="width: 200px"
            :disabled="isEdit"
          />
        </el-form-item>
        
        <!-- 租戶編號 -->
        <el-form-item label="租戶編號" prop="storeId">
          <el-input 
            v-model="formData.storeId" 
            placeholder="請輸入租戶編號"
            maxlength="6"
            style="width: 200px"
            :disabled="isEdit"
          >
            <template #append>
              <el-button @click="handleSelectStore">選擇</el-button>
            </template>
          </el-input>
          <span v-if="formData.storeName" style="margin-left: 10px; color: #409eff">
            {{ formData.storeName }}
          </span>
        </el-form-item>
        
        <!-- 資料所屬年月 -->
        <el-form-item label="資料所屬年月" prop="dataYm">
          <el-date-picker
            v-model="formData.dataYm"
            type="month"
            placeholder="請選擇年月"
            format="YYYYMM"
            value-format="YYYYMM"
            style="width: 200px"
            :disabled="isEdit"
          />
          <el-button 
            type="primary" 
            link 
            @click="handleCheckSettled"
            style="margin-left: 10px"
          >
            檢查是否已結帳
          </el-button>
          <el-tag v-if="isSettled" type="danger" style="margin-left: 10px">
            已結帳，不得異動
          </el-tag>
        </el-form-item>
        
        <!-- 費用期間(起) -->
        <el-form-item label="費用期間(起)" prop="periodDateB">
          <el-date-picker
            v-model="formData.periodDateB"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 費用期間(迄) -->
        <el-form-item label="費用期間(迄)" prop="periodDateE">
          <el-date-picker
            v-model="formData.periodDateE"
            type="date"
            placeholder="請選擇日期"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 200px"
          />
        </el-form-item>
        
        <!-- 用水度數 -->
        <el-form-item label="用水度數" prop="usedDegNum">
          <el-input-number
            v-model="formData.usedDegNum"
            :precision="2"
            :min="0"
            style="width: 200px"
            @change="handleCalculateTotal"
          />
        </el-form-item>
        
        <!-- 每度金額 -->
        <el-form-item label="每度金額" prop="perDegAmt">
          <el-input-number
            v-model="formData.perDegAmt"
            :precision="2"
            :min="0"
            style="width: 200px"
            @change="handleCalculateWaterAmt"
          />
        </el-form-item>
        
        <!-- 水費金額 -->
        <el-form-item label="水費金額" prop="waterAmt">
          <el-input-number
            v-model="formData.waterAmt"
            :precision="2"
            :min="0"
            style="width: 200px"
            @change="handleCalculateTotal"
          />
        </el-form-item>
        
        <!-- 公共區域水費 -->
        <el-form-item label="公共區域水費" prop="publicAmt">
          <el-input-number
            v-model="formData.publicAmt"
            :precision="2"
            :min="0"
            style="width: 200px"
            @change="handleCalculateTotal"
          />
        </el-form-item>
        
        <!-- 其他水費 -->
        <el-form-item label="其他水費" prop="otherCharge">
          <el-input-number
            v-model="formData.otherCharge"
            :precision="2"
            :min="0"
            style="width: 200px"
            @change="handleCalculateTotal"
          />
        </el-form-item>
        
        <!-- 總計水費 -->
        <el-form-item label="總計水費">
          <el-input-number
            v-model="formData.totalAmt"
            :precision="2"
            :min="0"
            style="width: 200px"
            disabled
          />
          <span style="margin-left: 10px; color: #909399; font-size: 12px">
            (自動計算: 水費金額 + 公共區域水費 + 其他水費)
          </span>
        </el-form-item>
        
        <!-- 檢查標誌 -->
        <el-form-item label="檢查標誌" prop="checkYn">
          <el-radio-group v-model="formData.checkYn">
            <el-radio label="Y">已檢查</el-radio>
            <el-radio label="N">未檢查</el-radio>
          </el-radio-group>
        </el-form-item>
        
        <!-- 備註 -->
        <el-form-item label="備註" prop="notes">
          <el-input
            v-model="formData.notes"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
            maxlength="500"
            show-word-limit
            style="width: 500px"
          />
        </el-form-item>
        
        <!-- 按鈕 -->
        <el-form-item>
          <el-button type="primary" @click="handleSubmit">確定</el-button>
          <el-button @click="handleCancel">取消</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>
```

#### 4.2.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { waterBillApi } from '@/api/water-bill'
import type { WaterBill } from '@/types/water-bill'

const route = useRoute()
const router = useRouter()

const isEdit = computed(() => !!route.params.tKey)
const formRef = ref()

// 表單資料
const formData = reactive<Partial<WaterBill>>({
  floorId: '',
  tenantNo: '',
  storeId: '',
  storeName: '',
  dataYm: '',
  periodDateB: '',
  periodDateE: '',
  usedDegNum: 0,
  perDegAmt: 0,
  waterAmt: 0,
  publicAmt: 0,
  otherCharge: 0,
  totalAmt: 0,
  checkYn: 'N',
  notes: ''
})

// 表單驗證規則
const formRules = {
  floorId: [
    { required: true, message: '請輸入樓別', trigger: 'blur' },
    { max: 3, message: '樓別長度不得超過3碼', trigger: 'blur' }
  ],
  tenantNo: [
    { required: true, message: '請輸入戶號', trigger: 'blur' },
    { max: 2, message: '戶號長度不得超過2碼', trigger: 'blur' }
  ],
  storeId: [
    { required: true, message: '請輸入租戶編號', trigger: 'blur' },
    { max: 6, message: '租戶編號長度不得超過6碼', trigger: 'blur' }
  ],
  dataYm: [
    { required: true, message: '請選擇資料所屬年月', trigger: 'change' }
  ],
  periodDateB: [
    { required: true, message: '請選擇費用期間(起)', trigger: 'change' }
  ],
  periodDateE: [
    { required: true, message: '請選擇費用期間(迄)', trigger: 'change' }
  ]
}

// 是否已結帳
const isSettled = ref(false)

// 計算水費金額
const handleCalculateWaterAmt = () => {
  if (formData.usedDegNum && formData.perDegAmt) {
    formData.waterAmt = Number((formData.usedDegNum * formData.perDegAmt).toFixed(2))
    handleCalculateTotal()
  }
}

// 計算總計水費
const handleCalculateTotal = () => {
  const waterAmt = formData.waterAmt || 0
  const publicAmt = formData.publicAmt || 0
  const otherCharge = formData.otherCharge || 0
  formData.totalAmt = Number((waterAmt + publicAmt + otherCharge).toFixed(2))
}

// 檢查是否已結帳
const handleCheckSettled = async () => {
  if (!formData.dataYm) {
    ElMessage.warning('請先選擇資料所屬年月')
    return
  }
  try {
    const response = await waterBillApi.checkSettled(formData.dataYm)
    if (response.success) {
      isSettled.value = response.data.isSettled
      if (response.data.isSettled) {
        ElMessage.warning(`資料所屬年月 ${formData.dataYm} 已結帳，不得異動`)
      } else {
        ElMessage.success(`資料所屬年月 ${formData.dataYm} 尚未結帳`)
      }
    }
  } catch (error) {
    ElMessage.error('檢查失敗')
  }
}

// 選擇租戶
const handleSelectStore = () => {
  // 開啟租戶選擇視窗
}

// 提交
const handleSubmit = async () => {
  try {
    await formRef.value.validate()
    
    // 檢查是否已結帳
    if (formData.dataYm) {
      const response = await waterBillApi.checkSettled(formData.dataYm)
      if (response.success && response.data.isSettled) {
        ElMessage.error('資料所屬年月已結帳，不得異動')
        return
      }
    }
    
    if (isEdit.value) {
      const response = await waterBillApi.update(Number(route.params.tKey), formData)
      if (response.success) {
        ElMessage.success('修改成功')
        router.push('/water-bills')
      }
    } else {
      const response = await waterBillApi.create(formData)
      if (response.success) {
        ElMessage.success('新增成功')
        router.push('/water-bills')
      }
    }
  } catch (error) {
    ElMessage.error(isEdit.value ? '修改失敗' : '新增失敗')
  }
}

// 取消
const handleCancel = () => {
  router.push('/water-bills')
}

// 載入資料
const loadData = async () => {
  if (isEdit.value) {
    try {
      const response = await waterBillApi.getById(Number(route.params.tKey))
      if (response.success) {
        Object.assign(formData, response.data)
        
        // 檢查是否已結帳
        if (formData.dataYm) {
          const settledResponse = await waterBillApi.checkSettled(formData.dataYm)
          if (settledResponse.success) {
            isSettled.value = settledResponse.data.isSettled
          }
        }
      }
    } catch (error) {
      ElMessage.error('載入資料失敗')
      router.push('/water-bills')
    }
  }
}

onMounted(() => {
  loadData()
})
</script>
```

---

## 五、開發任務清單

### 5.1 後端開發
- [ ] 建立 `WaterBills` 資料表
- [ ] 建立 `WaterBill` Entity 類別
- [ ] 建立 `WaterBillRepository` 介面與實作
- [ ] 建立 `WaterBillService` 介面與實作
- [ ] 建立 `WaterBillController` API 控制器
- [ ] 實作查詢列表 API
- [ ] 實作查詢單筆 API
- [ ] 實作新增 API（含業務規則檢查）
- [ ] 實作修改 API（含業務規則檢查）
- [ ] 實作刪除 API（含業務規則檢查）
- [ ] 實作批次刪除 API
- [ ] 實作檢查是否已結帳 API
- [ ] 實作取得租戶列表 API
- [ ] 撰寫單元測試
- [ ] 撰寫整合測試

### 5.2 前端開發
- [ ] 建立 `WaterBill` TypeScript 型別定義
- [ ] 建立 `water-bill.ts` API 服務
- [ ] 建立 `SYSN210Query.vue` 查詢頁面
- [ ] 建立 `SYSN210Form.vue` 新增/修改頁面
- [ ] 實作查詢功能（含分頁、排序、篩選）
- [ ] 實作新增功能
- [ ] 實作修改功能
- [ ] 實作刪除功能
- [ ] 實作批次刪除功能
- [ ] 實作檢查是否已結帳功能
- [ ] 實作租戶選擇功能
- [ ] 實作自動計算功能（水費金額、總計水費）
- [ ] 實作表單驗證
- [ ] 實作錯誤處理
- [ ] 撰寫單元測試

### 5.3 測試
- [ ] 單元測試（後端）
- [ ] 單元測試（前端）
- [ ] 整合測試
- [ ] 功能測試
- [ ] 效能測試

### 5.4 文件
- [ ] API 文件（Swagger）
- [ ] 使用者手冊
- [ ] 操作手冊

---

## 六、測試計劃

### 6.1 單元測試
- 測試資料表 CRUD 操作
- 測試業務規則驗證
- 測試 API 端點
- 測試前端元件

### 6.2 整合測試
- 測試前後端整合
- 測試資料庫整合
- 測試會計系統整合

### 6.3 功能測試
- 測試新增功能（含各種驗證）
- 測試修改功能（含各種驗證）
- 測試刪除功能（含各種驗證）
- 測試查詢功能（含各種篩選條件）
- 測試結帳檢查功能
- 測試自動計算功能

### 6.4 效能測試
- 測試大量資料查詢效能
- 測試批次操作效能

---

## 七、部署說明

### 7.1 資料庫部署
- 執行資料表建立 SQL
- 執行索引建立 SQL
- 執行初始資料匯入（如需要）

### 7.2 後端部署
- 部署 API 服務
- 設定連線字串
- 設定權限

### 7.3 前端部署
- 建置前端專案
- 部署靜態檔案
- 設定 API 端點

---

## 八、注意事項

1. **結帳檢查**: 新增、修改、刪除時必須檢查資料所屬年月是否已結帳，已結帳不得異動
2. **唯一性檢查**: 樓別、戶號、租戶編號、費用期間組合必須唯一
3. **自動計算**: 水費金額 = 用水度數 × 每度金額，總計水費 = 水費金額 + 公共區域水費 + 其他水費
4. **資料驗證**: 所有必填欄位必須驗證，數值欄位必須為正數
5. **權限控制**: 必須檢查使用者權限，只有有權限的使用者才能進行新增、修改、刪除操作

