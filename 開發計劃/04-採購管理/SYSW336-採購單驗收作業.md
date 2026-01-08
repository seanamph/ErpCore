# SYSW336 - 採購單驗收作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW336
- **功能名稱**: 採購單驗收作業
- **功能描述**: 提供採購單驗收作業的新增、修改、刪除、查詢功能，為 SYSW324 的變體版本，用於管理採購單的驗收流程，包含驗收數量、驗收日期、驗收人員、驗收狀態等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW336_*.asp` (相關功能頁面)
  - `WEB/IMS_CORE/SYSW000/SYSW336_PR.rpt` (報表，如有)

### 1.2 業務需求
- 管理採購單驗收作業
- 支援依採購單號查詢待驗收單據
- 支援驗收數量維護（可部分驗收）
- 支援驗收日期、驗收人員記錄
- 支援驗收狀態管理（待驗收、部分驗收、已驗收）
- 支援驗收單據列印
- 支援驗收後自動更新庫存
- 支援驗收後自動更新採購單已收數量

**注意**: SYSW336 與 SYSW324 功能類似，但可能有不同的業務流程或介面設計，需參考舊程式確認差異

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseReceipts` (採購驗收單主檔)

**說明**: 與 SYSW324 共用同一資料表 `PurchaseReceipts`

```sql
-- 參考 SYSW324 的 PurchaseReceipts 資料表結構
-- 可考慮增加欄位：SourceProgram (來源程式: SYSW324/SYSW336)
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseReceiptDetails` - 採購驗收單明細

**說明**: 與 SYSW324 共用同一資料表

```sql
-- 參考 SYSW324 的 PurchaseReceiptDetails 資料表結構
```

### 2.3 資料字典

**說明**: 資料表結構與 SYSW324 相同，請參考 SYSW324 開發計劃

---

## 三、後端 API 設計

### 3.1 API 端點列表

**說明**: API 設計與 SYSW324 相同，但路徑可能不同

#### 3.1.1 查詢待驗收採購單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts-v2/pending-orders` 或 `/api/v1/purchase-receipts/pending-orders` (共用)
- **說明**: 查詢待驗收的採購單列表
- **請求參數**: 參考 SYSW324 開發計劃

#### 3.1.2 查詢驗收單列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts-v2` 或 `/api/v1/purchase-receipts` (共用)
- **說明**: 查詢驗收單列表，支援分頁、排序、篩選
- **請求參數**: 參考 SYSW324 開發計劃

#### 3.1.3 查詢單筆驗收單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts-v2/{receiptId}` 或 `/api/v1/purchase-receipts/{receiptId}` (共用)
- **說明**: 根據驗收單號查詢單筆驗收單資料（含明細）

#### 3.1.4 依採購單號建立驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts-v2/from-order/{orderId}` 或 `/api/v1/purchase-receipts/from-order/{orderId}` (共用)
- **說明**: 依採購單號建立驗收單（帶入採購單明細）

#### 3.1.5 新增驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts-v2` 或 `/api/v1/purchase-receipts` (共用)
- **說明**: 新增驗收單（含明細）

#### 3.1.6 修改驗收單
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-receipts-v2/{receiptId}` 或 `/api/v1/purchase-receipts/{receiptId}` (共用)
- **說明**: 修改驗收單（僅未日結且未取消狀態可修改）

#### 3.1.7 刪除驗收單
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-receipts-v2/{receiptId}` 或 `/api/v1/purchase-receipts/{receiptId}` (共用)
- **說明**: 刪除驗收單（僅未日結且未取消狀態可刪除）

#### 3.1.8 確認驗收
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts-v2/{receiptId}/confirm` 或 `/api/v1/purchase-receipts/{receiptId}/confirm` (共用)
- **說明**: 確認驗收，更新庫存及採購單已收數量

#### 3.1.9 取消驗收單
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts-v2/{receiptId}/cancel` 或 `/api/v1/purchase-receipts/{receiptId}/cancel` (共用)
- **說明**: 取消驗收單

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseReceiptsController.cs`

**說明**: 可與 SYSW324 共用同一個 Controller，或建立新的 Controller

```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-receipts-v2")]
    [Authorize]
    public class PurchaseReceiptsV2Controller : ControllerBase
    {
        private readonly IPurchaseReceiptService _purchaseReceiptService;
        
        // 實作與 SYSW324 相同的 API 端點
        // 參考 SYSW324 開發計劃
    }
}
```

#### 3.2.2 Service: `PurchaseReceiptService.cs`

**說明**: 可與 SYSW324 共用同一個 Service

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 待驗收採購單列表頁面 (`PendingPurchaseOrderListV2.vue`)
- **路徑**: `/procurement/pending-receipts-v2`
- **功能**: 顯示待驗收的採購單列表，支援查詢、建立驗收單

#### 4.1.2 驗收單列表頁面 (`PurchaseReceiptListV2.vue`)
- **路徑**: `/procurement/purchase-receipts-v2`
- **功能**: 顯示驗收單列表，支援查詢、新增、修改、刪除、確認驗收、取消

#### 4.1.3 驗收單詳細頁面 (`PurchaseReceiptDetailV2.vue`)
- **路徑**: `/procurement/purchase-receipts-v2/:receiptId`
- **功能**: 顯示驗收單詳細資料（含明細），支援修改、確認驗收、取消

### 4.2 UI 元件設計

#### 4.2.1 待驗收採購單列表元件 (`PendingPurchaseOrderListV2.vue`)
- 顯示待驗收的採購單
- 支援依採購單建立驗收單

#### 4.2.2 驗收單列表元件 (`PurchaseReceiptDataTableV2.vue`)
- 顯示驗收單列表
- 支援查詢、新增、修改、刪除、確認驗收、取消

#### 4.2.3 驗收單新增/修改對話框 (`PurchaseReceiptDialogV2.vue`)
- 支援新增驗收單
- 支援修改驗收單
- 支援驗收數量維護
- 顯示訂購數量、已收數量、本次驗收數量

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（與 SYSW324 共用）
- [ ] 確認是否需要新增欄位

### 5.2 階段二: 後端開發 (3天)
- [ ] Controller 實作（或共用現有 Controller）
- [ ] Service 實作（或共用現有 Service）
- [ ] DTO 類別建立（或共用現有 DTO）
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 待驗收採購單列表頁面開發
- [ ] 驗收單列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1.5天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 8.5天

---

## 六、注意事項

### 6.1 與 SYSW324 的差異
- 需確認 SYSW336 與 SYSW324 的業務流程差異
- 需確認介面設計差異
- 需確認權限控制差異

### 6.2 資料表共用
- 與 SYSW324 共用同一資料表
- 可考慮增加 `SourceProgram` 欄位區分來源程式

### 6.3 其他注意事項
- 參考 SYSW324 開發計劃的注意事項

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增驗收單成功
- [ ] 修改驗收單成功
- [ ] 刪除驗收單成功
- [ ] 確認驗收成功

### 7.2 整合測試
- [ ] 完整驗收流程測試
- [ ] 與 SYSW324 的差異測試

### 7.3 效能測試
- [ ] 大量資料查詢測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW336_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW336_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW324-採購單驗收作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

