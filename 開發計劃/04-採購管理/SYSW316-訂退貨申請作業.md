# SYSW316 - 訂退貨申請作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW316
- **功能名稱**: 訂退貨申請作業
- **功能描述**: 提供訂退貨申請單的新增、修改、刪除、查詢功能，為 SYSW315 的變體版本，用於管理採購訂單和退貨申請，包含供應商、商品明細、數量、價格等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW316_*.asp` (相關功能頁面)
  - `WEB/IMS_CORE/SYSW000/SYSW316_PR.rpt` (報表，如有)

### 1.2 業務需求
- 管理採購訂單申請
- 管理退貨申請
- 支援供應商選擇
- 支援商品明細維護
- 支援數量、單價、金額計算
- 支援申請單狀態管理（草稿、已送出、已審核、已取消）
- 支援申請單審核流程
- 支援多店別管理
- 支援申請單報表列印

**注意**: SYSW316 與 SYSW315 功能類似，但可能有不同的業務流程或介面設計，需參考舊程式確認差異

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseOrders` (採購單主檔)

**說明**: 與 SYSW315 共用同一資料表 `PurchaseOrders`

```sql
-- 參考 SYSW315 的 PurchaseOrders 資料表結構
-- 可考慮增加欄位：SourceProgram (來源程式: SYSW315/SYSW316)
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseOrderDetails` - 採購單明細

**說明**: 與 SYSW315 共用同一資料表

```sql
-- 參考 SYSW315 的 PurchaseOrderDetails 資料表結構
```

### 2.3 資料字典

**說明**: 資料表結構與 SYSW315 相同，請參考 SYSW315 開發計劃

---

## 三、後端 API 設計

### 3.1 API 端點列表

**說明**: API 設計與 SYSW315 相同，但路徑可能不同

#### 3.1.1 查詢採購單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders-v2` 或 `/api/v1/purchase-orders` (共用)
- **說明**: 查詢採購單列表，支援分頁、排序、篩選
- **請求參數**: 參考 SYSW315 開發計劃

#### 3.1.2 查詢單筆採購單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}` 或 `/api/v1/purchase-orders/{orderId}` (共用)
- **說明**: 根據採購單號查詢單筆採購單資料（含明細）

#### 3.1.3 新增採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders-v2` 或 `/api/v1/purchase-orders` (共用)
- **說明**: 新增採購單（含明細）

#### 3.1.4 修改採購單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}` 或 `/api/v1/purchase-orders/{orderId}` (共用)
- **說明**: 修改採購單（僅草稿狀態可修改）

#### 3.1.5 刪除採購單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}` 或 `/api/v1/purchase-orders/{orderId}` (共用)
- **說明**: 刪除採購單（僅草稿狀態可刪除）

#### 3.1.6 送出採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}/submit` 或 `/api/v1/purchase-orders/{orderId}/submit` (共用)
- **說明**: 送出採購單進行審核

#### 3.1.7 審核採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}/approve` 或 `/api/v1/purchase-orders/{orderId}/approve` (共用)
- **說明**: 審核通過採購單

#### 3.1.8 取消採購單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-orders-v2/{orderId}/cancel` 或 `/api/v1/purchase-orders/{orderId}/cancel` (共用)
- **說明**: 取消採購單

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseOrdersV2Controller.cs` 或共用 `PurchaseOrdersController.cs`

**說明**: 可考慮與 SYSW315 共用 Controller，或建立獨立的 Controller

```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-orders-v2")] // 或共用路徑
    [Authorize]
    public class PurchaseOrdersV2Controller : ControllerBase
    {
        // 實作參考 SYSW315
    }
}
```

#### 3.2.2 Service: `PurchaseOrderService.cs`

**說明**: 可考慮與 SYSW315 共用 Service

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 採購單列表頁面 (`PurchaseOrderV2List.vue`)
- **路徑**: `/procurement/purchase-orders-v2` 或共用路徑
- **功能**: 顯示採購單列表，支援查詢、新增、修改、刪除、送出、審核、取消
- **主要元件**: 參考 SYSW315 開發計劃

#### 4.1.2 採購單詳細頁面 (`PurchaseOrderV2Detail.vue`)
- **路徑**: `/procurement/purchase-orders-v2/:orderId` 或共用路徑
- **功能**: 顯示採購單詳細資料（含明細），支援修改、送出、審核、取消

### 4.2 UI 元件設計

**說明**: UI 設計與 SYSW315 類似，但可能有不同的介面佈局或操作流程，需參考舊程式確認差異

#### 4.2.1 查詢表單元件 (`PurchaseOrderV2SearchForm.vue`)
- 參考 SYSW315 的 `PurchaseOrderSearchForm.vue`

#### 4.2.2 資料表格元件 (`PurchaseOrderV2DataTable.vue`)
- 參考 SYSW315 的 `PurchaseOrderDataTable.vue`

#### 4.2.3 新增/修改對話框 (`PurchaseOrderV2Dialog.vue`)
- 參考 SYSW315 的 `PurchaseOrderDialog.vue`

### 4.3 API 呼叫 (`purchaseOrderV2.api.ts`)

**說明**: 可考慮與 SYSW315 共用 API 函數，或建立獨立的 API 函數

```typescript
// 參考 SYSW315 的 purchaseOrder.api.ts
// 可共用或建立獨立版本
```

---

## 五、開發時程

### 5.1 階段一: 需求分析與差異確認 (0.5天)
- [ ] 分析 SYSW316 與 SYSW315 的差異
- [ ] 確認業務流程差異
- [ ] 確認介面設計差異

### 5.2 階段二: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（與 SYSW315 共用或獨立）
- [ ] 建立索引（如需要）
- [ ] 資料庫遷移腳本

### 5.3 階段三: 後端開發 (2-3天)
- [ ] 確認是否共用 Service 或建立獨立 Service
- [ ] Controller 實作
- [ ] 業務邏輯差異實作
- [ ] 單元測試

### 5.4 階段四: 前端開發 (2-3天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 差異功能實作
- [ ] 元件測試

### 5.5 階段五: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 與 SYSW315 的差異測試

### 5.6 階段六: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 6.5-8.5天（視差異程度而定）

---

## 六、注意事項

### 6.1 與 SYSW315 的差異
- 需詳細分析舊程式，確認 SYSW316 與 SYSW315 的具體差異
- 可能差異點：業務流程、介面設計、資料驗證規則、報表格式等

### 6.2 程式碼重用
- 考慮與 SYSW315 共用 Service、Repository、DTO 等
- 透過參數或設定區分不同版本的業務邏輯

### 6.3 其他注意事項
- 參考 SYSW315 開發計劃的注意事項

---

## 七、測試案例

### 7.1 單元測試
- 參考 SYSW315 的測試案例
- 增加 SYSW316 特有功能的測試案例

### 7.2 整合測試
- 參考 SYSW315 的測試案例
- 測試與 SYSW315 的差異功能

### 7.3 差異測試
- [ ] 確認 SYSW316 與 SYSW315 的業務流程差異
- [ ] 確認資料處理差異
- [ ] 確認介面操作差異

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW316_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW316_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW315-訂退貨申請作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

