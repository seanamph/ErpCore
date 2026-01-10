# 資料庫設計文件

## 概述

本文檔描述 ErpCore 系統的資料庫設計架構。

## 資料庫架構

### 資料庫命名規範
- 所有資料表使用 PascalCase 命名
- 所有欄位使用 PascalCase 命名
- 所有索引使用 `IX_TableName_ColumnName` 格式
- 所有外鍵使用 `FK_TableName_ReferencedTable` 格式

### 資料庫結構

#### 系統管理類 (SYS0000)
- Users - 使用者資料表
- Roles - 角色資料表
- Permissions - 權限資料表
- UserRoles - 使用者角色對應表
- RolePermissions - 角色權限對應表

#### 基本資料管理類 (SYSB000)
- Parameters - 參數設定表
- Regions - 地區設定表
- Areas - 區域設定表
- Banks - 金融機構表
- Vendors - 廠商客戶表
- Departments - 部別資料表
- Groups - 組別資料表
- Warehouses - 庫別資料表
- Shops - 店別資料表
- ProductCategories - 商品分類表

#### 進銷存管理類 (SYSW000)
- Products - 商品資料表
- SupplierGoods - 供應商商品資料表
- ProductGoodsIds - 商品進銷碼表
- PriceChanges - 商品永久變價表
- Stocks - 庫存資料表
- StockTransactions - 庫存交易表

#### 採購管理類 (SYSP000)
- PurchaseOrders - 採購單表
- PurchaseOrderDetails - 採購單明細表
- PurchaseReceipts - 採購單驗收表
- PurchaseReceiptDetails - 採購單驗收明細表

#### 調撥管理類 (SYSW000)
- TransferReceipts - 調撥單驗收表
- TransferReceiptDetails - 調撥單驗收明細表
- TransferReturns - 調撥單驗退表
- TransferReturnDetails - 調撥單驗退明細表
- TransferShortages - 調撥短溢表
- TransferShortageDetails - 調撥短溢明細表

## 資料庫存取規範

### Dapper 使用規範
- 所有資料庫存取使用 Dapper
- 使用參數化查詢，避免 SQL 注入
- 使用 DynamicParameters 傳遞參數
- 所有 Repository 都有 try-catch 和 NLog 日誌記錄

### 日誌記錄
- 所有資料庫操作都記錄日誌
- 日誌檔名以小時為單位：yyyyMMdd-HH.log
- 使用 NLog 進行日誌記錄

