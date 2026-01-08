# SYSW322 - 現場打單作業(訂退貨申請作業) 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW322
- **功能名稱**: 現場打單作業(訂退貨申請作業)
- **功能描述**: 提供現場打單方式的訂退貨申請單新增、修改、刪除、查詢功能，用於現場快速建立採購訂單和退貨申請，包含供應商、商品明細、數量、價格等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FS.ASP` (儲存)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW322_PR.asp` (報表)

### 1.2 業務需求
- 現場快速建立採購訂單申請
- 現場快速建立退貨申請
- 支援供應商選擇
- 支援商品明細快速輸入（條碼掃描）
- 支援數量、單價、金額計算
- 支援申請單狀態管理（草稿、已送出、已審核、已取消）
- 支援申請單審核流程
- 支援多店別管理
- 支援申請單報表列印
- 現場打單特殊需求：快速輸入、條碼掃描、即時計算

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOrders` (採購單主檔)

**說明**: 與 SYSW315 共用同一資料表，但現場打單作業有特殊的業務邏輯處理

```sql
-- 參考 SYSW315 的 PurchaseOrders 資料表結構
-- 現場打單作業使用相同的資料表，但可能有額外的欄位標記
-- 可考慮增加欄位：SourceType (來源類型: NORMAL/ON_SITE)
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseOrderDetails` - 採購單明細

**說明**: 與 SYSW315 共用同一資料表

```sql
-- 參考 SYSW315 的 PurchaseOrderDetails 資料表結構
```

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| OrderId | NVARCHAR | 50 | NO | - | 採購單號 | 主鍵，唯一 |
| OrderDate | DATETIME2 | - | NO | - | 採購日期 | - |
| OrderType | NVARCHAR | 20 | NO | - | 單據類型 | PO:採購, RT:退貨 |
| ShopId | NVARCHAR | 50 | NO | - | 分店代碼 | 外鍵至分店表 |
| SupplierId | NVARCHAR | 50 | NO | - | 供應商代碼 | 外鍵至供應商表 |
| Status | NVARCHAR | 10 | NO | 'D' | 狀態 | D:草稿, S:已送出, A:已審核, X:已取消 |
| SourceType | NVARCHAR | 20 | YES | 'NORMAL' | 來源類型 | NORMAL:一般, ON_SITE:現場打單 |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢現場打單申請單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/on-site-purchase-orders`
- **說明**: 查詢現場打單方式的採購單列表，支援分頁、排序、篩選
- **請求參數**: 同 SYSW315，但預設篩選 SourceType = 'ON_SITE'
- **回應格式**: 同 SYSW315

#### 3.1.2 查詢單筆現場打單申請單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/on-site-purchase-orders/{orderId}`
- **說明**: 根據採購單號查詢單筆現場打單申請單資料（含明細）
- **路徑參數**:
  - `orderId`: 採購單號
- **回應格式**: 同 SYSW315

#### 3.1.3 新增現場打單申請單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/on-site-purchase-orders`
- **說明**: 新增現場打單方式的採購單（含明細），自動設定 SourceType = 'ON_SITE'
- **請求格式**: 同 SYSW315，但 SourceType 自動設定

#### 3.1.4 修改現場打單申請單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/on-site-purchase-orders/{orderId}`
- **說明**: 修改現場打單申請單（僅草稿狀態可修改）
- **路徑參數**:
  - `orderId`: 採購單號
- **請求格式**: 同 SYSW315

#### 3.1.5 刪除現場打單申請單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/on-site-purchase-orders/{orderId}`
- **說明**: 刪除現場打單申請單（僅草稿狀態可刪除）
- **路徑參數**:
  - `orderId`: 採購單號

#### 3.1.6 送出現場打單申請單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/on-site-purchase-orders/{orderId}/submit`
- **說明**: 送出現場打單申請單進行審核

#### 3.1.7 條碼掃描查詢商品
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/on-site-purchase-orders/goods-by-barcode`
- **說明**: 根據條碼快速查詢商品資訊（現場打單專用）
- **請求參數**:
  ```json
  {
    "barcode": "1234567890123"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "goodsId": "G001",
      "goodsName": "商品名稱",
      "barcodeId": "1234567890123",
      "unitPrice": 100.00,
      "unit": "PCS"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

### 3.2 後端實作類別

#### 3.2.1 Controller: `OnSitePurchaseOrdersController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/on-site-purchase-orders")]
    [Authorize]
    public class OnSitePurchaseOrdersController : ControllerBase
    {
        private readonly IOnSitePurchaseOrderService _onSitePurchaseOrderService;
        
        public OnSitePurchaseOrdersController(IOnSitePurchaseOrderService onSitePurchaseOrderService)
        {
            _onSitePurchaseOrderService = onSitePurchaseOrderService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PurchaseOrderDto>>>> GetOnSitePurchaseOrders([FromQuery] PurchaseOrderQueryDto query)
        {
            // 實作查詢邏輯，預設篩選 SourceType = 'ON_SITE'
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<PurchaseOrderDetailDto>>> GetOnSitePurchaseOrder(string orderId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateOnSitePurchaseOrder([FromBody] CreatePurchaseOrderDto dto)
        {
            // 實作新增邏輯，自動設定 SourceType = 'ON_SITE'
        }
        
        [HttpPut("{orderId}")]
        public async Task<ActionResult<ApiResponse>> UpdateOnSitePurchaseOrder(string orderId, [FromBody] UpdatePurchaseOrderDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{orderId}")]
        public async Task<ActionResult<ApiResponse>> DeleteOnSitePurchaseOrder(string orderId)
        {
            // 實作刪除邏輯
        }
        
        [HttpPost("{orderId}/submit")]
        public async Task<ActionResult<ApiResponse>> SubmitOnSitePurchaseOrder(string orderId)
        {
            // 實作送出邏輯
        }
        
        [HttpGet("goods-by-barcode")]
        public async Task<ActionResult<ApiResponse<GoodsByBarcodeDto>>> GetGoodsByBarcode([FromQuery] string barcode)
        {
            // 實作條碼查詢商品邏輯
        }
    }
}
```

#### 3.2.2 Service: `OnSitePurchaseOrderService.cs`
```csharp
namespace RSL.IMS3.Application.Services
{
    public interface IOnSitePurchaseOrderService
    {
        Task<PagedResult<PurchaseOrderDto>> GetOnSitePurchaseOrdersAsync(PurchaseOrderQueryDto query);
        Task<PurchaseOrderDetailDto> GetOnSitePurchaseOrderByIdAsync(string orderId);
        Task<string> CreateOnSitePurchaseOrderAsync(CreatePurchaseOrderDto dto);
        Task UpdateOnSitePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto);
        Task DeleteOnSitePurchaseOrderAsync(string orderId);
        Task SubmitOnSitePurchaseOrderAsync(string orderId);
        Task<GoodsByBarcodeDto> GetGoodsByBarcodeAsync(string barcode);
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 現場打單申請單列表頁面 (`OnSitePurchaseOrderList.vue`)
- **路徑**: `/procurement/on-site-purchase-orders`
- **功能**: 顯示現場打單申請單列表，支援查詢、新增、修改、刪除、送出
- **主要元件**:
  - 查詢表單 (OnSitePurchaseOrderSearchForm)
  - 資料表格 (OnSitePurchaseOrderDataTable)
  - 新增/修改對話框 (OnSitePurchaseOrderDialog)
  - 刪除確認對話框
  - 送出按鈕

#### 4.1.2 現場打單申請單詳細頁面 (`OnSitePurchaseOrderDetail.vue`)
- **路徑**: `/procurement/on-site-purchase-orders/:orderId`
- **功能**: 顯示現場打單申請單詳細資料（含明細），支援修改、送出
- **特殊功能**: 條碼掃描輸入、快速商品選擇

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`OnSitePurchaseOrderSearchForm.vue`)
```vue
<template>
  <el-form :model="searchForm" inline>
    <!-- 同 SYSW315 的查詢表單，但標題改為「現場打單申請單」 -->
  </el-form>
</template>
```

#### 4.2.2 資料表格元件 (`OnSitePurchaseOrderDataTable.vue`)
```vue
<template>
  <div>
    <el-table :data="orderList" v-loading="loading">
      <!-- 同 SYSW315 的資料表格 -->
    </el-table>
  </div>
</template>
```

#### 4.2.3 新增/修改對話框 (`OnSitePurchaseOrderDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="1400px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="120px">
      <!-- 主檔表單同 SYSW315 -->
      
      <!-- 明細表格 - 現場打單特殊設計 -->
      <el-divider>商品明細（支援條碼掃描）</el-divider>
      
      <!-- 條碼掃描輸入區 -->
      <el-row :gutter="20" style="margin-bottom: 10px;">
        <el-col :span="12">
          <el-input
            v-model="barcodeInput"
            placeholder="請掃描或輸入條碼"
            @keyup.enter="handleBarcodeScan"
            ref="barcodeInputRef"
          >
            <template #prepend>條碼</template>
            <template #append>
              <el-button @click="handleBarcodeScan">查詢</el-button>
            </template>
          </el-input>
        </el-col>
        <el-col :span="12">
          <el-button type="primary" @click="handleQuickAddGoods">快速新增商品</el-button>
        </el-col>
      </el-row>
      
      <el-table :data="form.details" border>
        <el-table-column type="index" label="序號" width="60" />
        <el-table-column prop="barcodeId" label="條碼" width="150">
          <template #default="{ row, $index }">
            <el-input v-model="row.barcodeId" @blur="handleBarcodeChange($index)" />
          </template>
        </el-table-column>
        <el-table-column prop="goodsId" label="商品編號" width="120" />
        <el-table-column prop="goodsName" label="商品名稱" width="200" />
        <el-table-column prop="orderQty" label="數量" width="120">
          <template #default="{ row, $index }">
            <el-input-number 
              v-model="row.orderQty" 
              :min="0" 
              :precision="4" 
              @change="handleQtyChange($index)"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column prop="unitPrice" label="單價" width="120">
          <template #default="{ row, $index }">
            <el-input-number 
              v-model="row.unitPrice" 
              :min="0" 
              :precision="4" 
              @change="handlePriceChange($index)"
              style="width: 100%"
            />
          </template>
        </el-table-column>
        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="{ row }">
            {{ formatCurrency(row.amount) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ $index }">
            <el-button type="danger" size="small" @click="handleDeleteDetail($index)">刪除</el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <div style="margin-top: 10px;">
        <el-button type="primary" @click="handleAddDetail">新增明細</el-button>
        <span style="margin-left: 20px; font-weight: bold;">
          總金額: {{ formatCurrency(totalAmount) }}
        </span>
      </div>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { getGoodsByBarcode } from '@/api/onSitePurchaseOrder.api';

const barcodeInput = ref('');
const barcodeInputRef = ref();

// 條碼掃描處理
const handleBarcodeScan = async () => {
  if (!barcodeInput.value) return;
  
  try {
    const response = await getGoodsByBarcode(barcodeInput.value);
    if (response.data.success) {
      // 自動新增商品到明細
      const goods = response.data.data;
      form.value.details.push({
        lineNum: form.value.details.length + 1,
        goodsId: goods.goodsId,
        goodsName: goods.goodsName,
        barcodeId: goods.barcodeId,
        orderQty: 1,
        unitPrice: goods.unitPrice,
        amount: goods.unitPrice
      });
      barcodeInput.value = '';
      barcodeInputRef.value?.focus();
      calculateTotal();
    }
  } catch (error) {
    ElMessage.error('查詢商品失敗');
  }
};

// 條碼變更處理
const handleBarcodeChange = async (index: number) => {
  const detail = form.value.details[index];
  if (detail.barcodeId) {
    try {
      const response = await getGoodsByBarcode(detail.barcodeId);
      if (response.data.success) {
        const goods = response.data.data;
        detail.goodsId = goods.goodsId;
        detail.goodsName = goods.goodsName;
        detail.unitPrice = goods.unitPrice;
        handlePriceChange(index);
      }
    } catch (error) {
      ElMessage.error('查詢商品失敗');
    }
  }
};
</script>
```

### 4.3 API 呼叫 (`onSitePurchaseOrder.api.ts`)
```typescript
import request from '@/utils/request';
import type { 
  PurchaseOrderDto, 
  PurchaseOrderDetailDto, 
  PurchaseOrderQueryDto,
  CreatePurchaseOrderDto,
  UpdatePurchaseOrderDto
} from '@/api/purchaseOrder.api';

export interface GoodsByBarcodeDto {
  goodsId: string;
  goodsName: string;
  barcodeId: string;
  unitPrice: number;
  unit: string;
}

// API 函數
export const getOnSitePurchaseOrderList = (query: PurchaseOrderQueryDto) => {
  return request.get<ApiResponse<PagedResult<PurchaseOrderDto>>>('/api/v1/on-site-purchase-orders', { params: query });
};

export const getOnSitePurchaseOrderById = (orderId: string) => {
  return request.get<ApiResponse<PurchaseOrderDetailDto>>(`/api/v1/on-site-purchase-orders/${orderId}`);
};

export const createOnSitePurchaseOrder = (data: CreatePurchaseOrderDto) => {
  return request.post<ApiResponse<string>>('/api/v1/on-site-purchase-orders', data);
};

export const updateOnSitePurchaseOrder = (orderId: string, data: UpdatePurchaseOrderDto) => {
  return request.put<ApiResponse>(`/api/v1/on-site-purchase-orders/${orderId}`, data);
};

export const deleteOnSitePurchaseOrder = (orderId: string) => {
  return request.delete<ApiResponse>(`/api/v1/on-site-purchase-orders/${orderId}`);
};

export const submitOnSitePurchaseOrder = (orderId: string) => {
  return request.post<ApiResponse>(`/api/v1/on-site-purchase-orders/${orderId}/submit`);
};

export const getGoodsByBarcode = (barcode: string) => {
  return request.get<ApiResponse<GoodsByBarcodeDto>>('/api/v1/on-site-purchase-orders/goods-by-barcode', { 
    params: { barcode } 
  });
};
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（與 SYSW315 共用）
- [ ] 確認是否需要新增 SourceType 欄位
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (3天)
- [ ] Entity 類別建立（可共用 SYSW315）
- [ ] Repository 實作（可共用 SYSW315）
- [ ] Service 實作（現場打單特殊邏輯）
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 條碼查詢商品邏輯實作
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 明細表格開發（條碼掃描功能）
- [ ] 條碼掃描輸入功能
- [ ] 快速商品選擇功能
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 條碼掃描功能測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 9天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查（申請、審核權限分離）
- 敏感資料必須加密傳輸 (HTTPS)
- 必須記錄操作日誌
- 條碼輸入必須驗證格式

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 條碼查詢必須快速回應（建議使用快取）
- 明細資料量大時需考慮分批載入

### 6.3 資料驗證
- 採購單號必須唯一
- 必填欄位必須驗證
- 數量、單價必須大於0
- 金額計算必須正確
- 狀態轉換必須符合業務規則
- 條碼格式必須驗證

### 6.4 業務邏輯
- 僅草稿狀態可修改、刪除
- 送出後不可修改，需審核
- 審核通過後不可修改
- 刪除採購單前必須檢查是否有相關驗收單
- 金額計算：金額 = 數量 × 單價
- 總金額 = 所有明細金額總和
- 條碼掃描後自動填入商品資訊
- 條碼不存在時提示錯誤

### 6.5 現場打單特殊需求
- 條碼掃描輸入必須快速響應
- 支援鍵盤快捷鍵操作
- 支援自動聚焦到條碼輸入框
- 支援快速新增商品功能
- 介面設計需考慮現場操作便利性

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增現場打單申請單成功
- [ ] 新增現場打單申請單失敗 (必填欄位未填)
- [ ] 修改現場打單申請單成功
- [ ] 修改現場打單申請單失敗 (非草稿狀態)
- [ ] 刪除現場打單申請單成功
- [ ] 刪除現場打單申請單失敗 (非草稿狀態)
- [ ] 送出現場打單申請單成功
- [ ] 查詢現場打單申請單列表成功
- [ ] 查詢單筆現場打單申請單成功
- [ ] 條碼查詢商品成功
- [ ] 條碼查詢商品失敗 (條碼不存在)

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 條碼掃描流程測試
- [ ] 快速新增商品流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 條碼查詢響應時間測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FB.asp` (瀏覽)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FI.asp` (新增)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FU.asp` (修改)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FD.asp` (刪除)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FQ.asp` (查詢)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_FS.ASP` (儲存)
- `WEB/IMS_CORE/ASP/SYSW000/SYSW322_PR.asp` (報表)

### 8.2 資料庫 Schema
- 參考 SYSW315 的資料表結構
- 參考舊系統 `RIM_SUBPOENAM` (主檔)
- 參考舊系統 `RIM_SUBPOENAD` (明細)

### 8.3 相關功能
- SYSW315 - 訂退貨申請作業（一般版本）
- SYSW316 - 訂退貨申請作業（另一個版本）

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

