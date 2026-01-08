# SYSW337 - 已日結退貨單驗退調整作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSW337
- **功能名稱**: 已日結退貨單驗退調整作業
- **功能描述**: 提供已日結退貨單的驗退調整功能，用於處理已日結的退貨單驗退作業，包含驗退單新增、修改、刪除、查詢功能，支援已日結退貨單的驗退數量、價格調整等資訊管理。與 SYSW530 功能類似，但可能有不同的業務流程或介面設計
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FB.asp` (瀏覽)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FI.asp` (新增)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FU.asp` (修改)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FD.asp` (刪除)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FQ.asp` (查詢)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_FS.asp` (儲存)
  - `WEB/IMS_CORE/ASP/SYSW000/SYSW337_PR.asp` (報表)
  - `WEB/IMS_CORE/SYSW000/SYSW337_PR.rpt` (報表定義)

### 1.2 業務需求
- 處理已日結的退貨單驗退作業
- 支援驗退單新增、修改、刪除、查詢
- 支援驗退數量、價格調整
- 支援驗退單狀態管理（草稿、已審核、已日結）
- 支援驗退單審核流程
- 支援多店別管理
- 支援驗退單報表列印
- 已日結退貨單的特殊處理邏輯

**注意**: SYSW337 與 SYSW530 功能類似，但可能有不同的業務流程或介面設計，需參考舊程式確認差異

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `PurchaseReceipts` (採購驗收單主檔)

**說明**: 與 SYSW530 共用同一資料表 `PurchaseReceipts`，用於處理退貨單驗退作業

```sql
-- 參考 SYSW530 的 PurchaseReceipts 資料表結構
-- PurchaseOrderType = '2' 表示退貨單
-- 可考慮增加欄位：
-- [IsSettledAdjustment] BIT NOT NULL DEFAULT 0, -- 是否為已日結調整
-- [OriginalReceiptId] NVARCHAR(50) NULL, -- 原始驗退單號（如有）
-- [AdjustmentReason] NVARCHAR(500) NULL, -- 調整原因
```

### 2.2 相關資料表

#### 2.2.1 `PurchaseReceiptDetails` - 採購驗收單明細

**說明**: 與 SYSW530 共用同一資料表

```sql
-- 參考 SYSW530 的 PurchaseReceiptDetails 資料表結構
-- 可考慮增加欄位：
-- [OriginalCheckQty] DECIMAL(18, 4) NULL, -- 原始驗退數量
-- [AdjustmentQty] DECIMAL(18, 4) NULL, -- 調整數量
-- [OriginalCheckPrice] DECIMAL(18, 4) NULL, -- 原始驗退單價
-- [AdjustmentPrice] DECIMAL(18, 4) NULL, -- 調整單價
```

### 2.3 資料字典

**說明**: 資料表結構與 SYSW530 相同，請參考 SYSW530 開發計劃

---

## 三、後端 API 設計

### 3.1 API 端點列表

**說明**: API 設計與 SYSW530 相同，但路徑可能不同

#### 3.1.1 查詢已日結退貨單驗退調整列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2` 或 `/api/v1/purchase-receipts/closed-return-adjustments` (共用)
- **說明**: 查詢已日結退貨單驗退調整列表，支援分頁、排序、篩選
- **請求參數**: 參考 SYSW530 開發計劃

#### 3.1.2 查詢單筆已日結退貨單驗退調整
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2/{receiptId}` 或 `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}` (共用)
- **說明**: 根據驗退單號查詢單筆已日結退貨單驗退調整資料（含明細）

#### 3.1.3 查詢可用已日結退貨單
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/purchase-receipts/closed-return-orders-v2` 或 `/api/v1/purchase-receipts/closed-return-orders` (共用)
- **說明**: 查詢可用於調整的已日結退貨單列表
- **請求參數**: 參考 SYSW530 開發計劃

#### 3.1.4 新增已日結退貨單驗退調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2` 或 `/api/v1/purchase-receipts/closed-return-adjustments` (共用)
- **說明**: 新增已日結退貨單驗退調整（含明細）
- **請求格式**: 參考 SYSW530 開發計劃

#### 3.1.5 修改已日結退貨單驗退調整
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2/{receiptId}` 或 `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}` (共用)
- **說明**: 修改已日結退貨單驗退調整（僅未審核狀態可修改）

#### 3.1.6 刪除已日結退貨單驗退調整
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2/{receiptId}` 或 `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}` (共用)
- **說明**: 刪除已日結退貨單驗退調整（僅未審核狀態可刪除）

#### 3.1.7 審核已日結退貨單驗退調整
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/purchase-receipts/closed-return-adjustments-v2/{receiptId}/approve` 或 `/api/v1/purchase-receipts/closed-return-adjustments/{receiptId}/approve` (共用)
- **說明**: 審核已日結退貨單驗退調整
- **請求格式**: 參考 SYSW530 開發計劃

### 3.2 後端實作類別

#### 3.2.1 Controller: `PurchaseReceiptsController.cs`

**說明**: 可與 SYSW530 共用同一個 Controller，或建立新的 Controller

```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/purchase-receipts")]
    [Authorize]
    public class PurchaseReceiptsController : ControllerBase
    {
        private readonly IPurchaseReceiptService _purchaseReceiptService;
        
        // 實作與 SYSW530 相同的 API 端點
        // 參考 SYSW530 開發計劃
    }
}
```

#### 3.2.2 Service: `PurchaseReceiptService.cs`

**說明**: 可與 SYSW530 共用同一個 Service

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 已日結退貨單驗退調整列表頁面 (`ClosedReturnAdjustmentListV2.vue`)
- **路徑**: `/purchase/closed-return-adjustments-v2`
- **功能**: 顯示已日結退貨單驗退調整列表，支援查詢、新增、修改、刪除、審核

#### 4.1.2 已日結退貨單驗退調整詳細頁面 (`ClosedReturnAdjustmentDetailV2.vue`)
- **路徑**: `/purchase/closed-return-adjustments-v2/:receiptId`
- **功能**: 顯示已日結退貨單驗退調整詳細資料（含明細），支援修改、審核

#### 4.1.3 已日結退貨單驗退調整新增/修改頁面 (`ClosedReturnAdjustmentFormV2.vue`)
- **路徑**: `/purchase/closed-return-adjustments-v2/new` 或 `/purchase/closed-return-adjustments-v2/:receiptId/edit`
- **功能**: 新增或修改已日結退貨單驗退調整

### 4.2 UI 元件設計

#### 4.2.1 已日結退貨單驗退調整列表元件 (`ClosedReturnAdjustmentDataTableV2.vue`)
- 顯示已日結退貨單驗退調整列表
- 支援查詢、新增、修改、刪除、審核

#### 4.2.2 已日結退貨單驗退調整新增/修改對話框 (`ClosedReturnAdjustmentDialogV2.vue`)
- 支援新增已日結退貨單驗退調整
- 支援修改已日結退貨單驗退調整
- 支援驗退數量、價格調整
- 顯示原始驗退數量、調整數量、調整後數量
- 顯示原始單價、調整單價、調整後單價

#### 4.2.3 已日結退貨單選擇元件 (`ClosedReturnOrderSelectorV2.vue`)
- 選擇可用於調整的已日結退貨單
- 顯示退貨單基本資訊

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (0.5天)
- [ ] 確認資料表結構（與 SYSW530 共用）
- [ ] 確認是否需要新增欄位

### 5.2 階段二: 後端開發 (3天)
- [ ] Controller 實作（或共用現有 Controller）
- [ ] Service 實作（或共用現有 Service）
- [ ] DTO 類別建立（或共用現有 DTO）
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (3天)
- [ ] API 呼叫函數
- [ ] 已日結退貨單驗退調整列表頁面開發
- [ ] 已日結退貨單驗退調整新增/修改頁面開發
- [ ] 已日結退貨單選擇元件開發
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

### 6.1 與 SYSW530 的差異
- 需確認 SYSW337 與 SYSW530 的業務流程差異
- 需確認介面設計差異
- 需確認權限控制差異

### 6.2 資料表共用
- 與 SYSW530 共用同一資料表
- 可考慮增加 `SourceProgram` 欄位區分來源程式

### 6.3 其他注意事項
- 參考 SYSW530 開發計劃的注意事項

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增已日結退貨單驗退調整成功
- [ ] 修改已日結退貨單驗退調整成功
- [ ] 刪除已日結退貨單驗退調整成功
- [ ] 審核已日結退貨單驗退調整成功

### 7.2 整合測試
- [ ] 完整已日結退貨單驗退調整流程測試
- [ ] 與 SYSW530 的差異測試

### 7.3 效能測試
- [ ] 大量資料查詢測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSW000/SYSW337_*.asp`
- `WEB/IMS_CORE/SYSW000/SYSW337_PR.rpt` (如有)

### 8.2 相關開發計劃
- `開發計劃/04-採購管理/SYSW530-已日結退貨單驗退調整作業.md`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

