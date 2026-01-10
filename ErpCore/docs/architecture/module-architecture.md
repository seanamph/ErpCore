# 模組架構文件

## 概述

本文檔描述 ErpCore 系統的模組架構。

## 模組架構

### 後端架構（四層架構）

#### 1. Domain 層（領域層）
- **職責**: 定義實體模型和領域邏輯
- **位置**: `ErpCore.Domain`
- **包含**: Entities（實體類別）

#### 2. Application 層（應用層）
- **職責**: 業務邏輯處理
- **位置**: `ErpCore.Application`
- **包含**: Services（服務類別）、DTOs（資料傳輸物件）

#### 3. Infrastructure 層（基礎設施層）
- **職責**: 資料存取和外部服務
- **位置**: `ErpCore.Infrastructure`
- **包含**: Repositories（資料存取）、外部服務整合

#### 4. API 層（表現層）
- **職責**: API 端點和請求處理
- **位置**: `ErpCore.Api`
- **包含**: Controllers（控制器）

### 前端架構

#### Vue CLI 專案結構
- **Views**: Vue 元件（.vue 檔案）
- **API**: API 呼叫封裝
- **Router**: 路由配置
- **Store**: 狀態管理
- **Components**: 共用元件

### 功能模組

系統包含 57 個功能模組，涵蓋：
- 系統管理
- 基本資料管理
- 進銷存管理
- 採購管理
- 調撥管理
- 盤點管理
- 庫存調整
- 電子發票
- 客戶管理
- 分析報表
- 業務報表
- 其他功能模組

