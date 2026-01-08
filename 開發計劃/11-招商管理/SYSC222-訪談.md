# SYSC222 - 訪談 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSC222
- **功能名稱**: 訪談
- **功能描述**: 提供潛客訪談記錄的新增、修改、刪除、查詢功能，包含訪談日期、訪談內容、訪談結果、後續行動等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSC000/SYSC222*.asp` (相關功能)
  - `IMS3/HANSHIN/IMS3/SYSC000/SYSC222.ascx` (如存在)

### 1.2 業務需求
- 管理潛客訪談記錄資訊
- 支援訪談記錄的新增、修改、刪除、查詢
- 記錄訪談的建立與變更資訊
- 支援訪談與潛客的關聯
- 支援訪談結果分類（成功、待追蹤、取消等）
- 支援訪談後續行動管理
- 支援訪談報表查詢與列印

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Interviews` (對應舊系統 `RIM_INTERVIEW` 或類似)

```sql
CREATE TABLE [dbo].[Interviews] (
    [InterviewId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), -- 訪談ID (INTERVIEW_ID)
    [ProspectId] NVARCHAR(50) NOT NULL, -- 潛客代碼 (PROSPECT_ID)
    [InterviewDate] DATETIME2 NOT NULL, -- 訪談日期 (INTERVIEW_DATE)
    [InterviewTime] TIME NULL, -- 訪談時間 (INTERVIEW_TIME)
    [InterviewType] NVARCHAR(20) NULL, -- 訪談類型 (INTERVIEW_TYPE) PHONE:電話, FACE_TO_FACE:面對面, ONLINE:線上
    [Interviewer] NVARCHAR(50) NULL, -- 訪談人員 (INTERVIEWER)
    [InterviewLocation] NVARCHAR(200) NULL, -- 訪談地點 (INTERVIEW_LOCATION)
    [InterviewContent] NVARCHAR(MAX) NULL, -- 訪談內容 (INTERVIEW_CONTENT)
    [InterviewResult] NVARCHAR(20) NULL, -- 訪談結果 (INTERVIEW_RESULT) SUCCESS:成功, FOLLOW_UP:待追蹤, CANCELLED:取消, NO_SHOW:未到
    [NextAction] NVARCHAR(200) NULL, -- 後續行動 (NEXT_ACTION)
    [NextActionDate] DATETIME2 NULL, -- 後續行動日期 (NEXT_ACTION_DATE)
    [FollowUpDate] DATETIME2 NULL, -- 追蹤日期 (FOLLOW_UP_DATE)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'ACTIVE', -- 狀態 (STATUS) ACTIVE:有效, CANCELLED:取消
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Interviews] PRIMARY KEY CLUSTERED ([InterviewId] ASC),
    CONSTRAINT [FK_Interviews_Prospects] FOREIGN KEY ([ProspectId]) REFERENCES [dbo].[Prospects] ([ProspectId]) ON DELETE CASCADE
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Interviews_ProspectId] ON [dbo].[Interviews] ([ProspectId]);
CREATE NONCLUSTERED INDEX [IX_Interviews_InterviewDate] ON [dbo].[Interviews] ([InterviewDate]);
CREATE NONCLUSTERED INDEX [IX_Interviews_InterviewResult] ON [dbo].[Interviews] ([InterviewResult]);
CREATE NONCLUSTERED INDEX [IX_Interviews_Status] ON [dbo].[Interviews] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Interviews_NextActionDate] ON [dbo].[Interviews] ([NextActionDate]);
CREATE NONCLUSTERED INDEX [IX_Interviews_FollowUpDate] ON [dbo].[Interviews] ([FollowUpDate]);
CREATE NONCLUSTERED INDEX [IX_Interviews_CreatedAt] ON [dbo].[Interviews] ([CreatedAt]);
```

### 2.2 相關資料表

#### 2.2.1 `Prospects` - 潛客主檔
- 用於查詢潛客列表
- 參考: `開發計劃/11-招商管理/SYSC180-潛客.md`

#### 2.2.2 `Users` - 使用者主檔
- 用於查詢訪談人員列表
- 參考: `開發計劃/01-系統管理/01-使用者管理/SYS0110-使用者基本資料維護.md`

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| InterviewId | BIGINT | - | NO | IDENTITY(1,1) | 訪談ID | 主鍵，自動遞增 |
| ProspectId | NVARCHAR | 50 | NO | - | 潛客代碼 | 外鍵至Prospects表 |
| InterviewDate | DATETIME2 | - | NO | - | 訪談日期 | - |
| InterviewTime | TIME | - | YES | - | 訪談時間 | - |
| InterviewType | NVARCHAR | 20 | YES | - | 訪談類型 | PHONE:電話, FACE_TO_FACE:面對面, ONLINE:線上 |
| Interviewer | NVARCHAR | 50 | YES | - | 訪談人員 | 外鍵至Users表 |
| InterviewLocation | NVARCHAR | 200 | YES | - | 訪談地點 | - |
| InterviewContent | NVARCHAR(MAX) | - | YES | - | 訪談內容 | - |
| InterviewResult | NVARCHAR | 20 | YES | - | 訪談結果 | SUCCESS:成功, FOLLOW_UP:待追蹤, CANCELLED:取消, NO_SHOW:未到 |
| NextAction | NVARCHAR | 200 | YES | - | 後續行動 | - |
| NextActionDate | DATETIME2 | - | YES | - | 後續行動日期 | - |
| FollowUpDate | DATETIME2 | - | YES | - | 追蹤日期 | - |
| Notes | NVARCHAR | 500 | YES | - | 備註 | - |
| Status | NVARCHAR | 20 | NO | 'ACTIVE' | 狀態 | ACTIVE:有效, CANCELLED:取消 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |
| CreatedPriority | INT | - | YES | - | 建立者等級 | - |
| CreatedGroup | NVARCHAR | 50 | YES | - | 建立者群組 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢訪談列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/interviews`
- **說明**: 查詢訪談列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "InterviewDate",
    "sortOrder": "DESC",
    "filters": {
      "prospectId": "",
      "interviewDateFrom": "",
      "interviewDateTo": "",
      "interviewResult": "",
      "status": "",
      "interviewer": ""
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
          "interviewId": 1,
          "prospectId": "P001",
          "prospectName": "潛客名稱",
          "interviewDate": "2024-01-01T00:00:00",
          "interviewType": "FACE_TO_FACE",
          "interviewResult": "SUCCESS",
          "interviewer": "U001",
          "createdAt": "2024-01-01T00:00:00"
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

#### 3.1.2 根據潛客查詢訪談列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/interviews/by-prospect/{prospectId}`
- **說明**: 根據潛客代碼查詢該潛客的所有訪談記錄
- **路徑參數**:
  - `prospectId`: 潛客代碼

#### 3.1.3 查詢單筆訪談
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/interviews/{interviewId}`
- **說明**: 根據訪談ID查詢單筆訪談資料
- **路徑參數**:
  - `interviewId`: 訪談ID
- **回應格式**: 包含完整訪談資料

#### 3.1.4 新增訪談
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/interviews`
- **說明**: 新增訪談記錄
- **請求格式**:
  ```json
  {
    "prospectId": "P001",
    "interviewDate": "2024-01-01T00:00:00",
    "interviewTime": "14:00:00",
    "interviewType": "FACE_TO_FACE",
    "interviewer": "U001",
    "interviewLocation": "會議室A",
    "interviewContent": "訪談內容...",
    "interviewResult": "SUCCESS",
    "nextAction": "後續行動",
    "nextActionDate": "2024-01-15T00:00:00",
    "notes": "備註"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "interviewId": 1
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改訪談
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/interviews/{interviewId}`
- **說明**: 修改訪談記錄
- **路徑參數**:
  - `interviewId`: 訪談ID
- **請求格式**: 同新增，但 `interviewId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除訪談
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/interviews/{interviewId}`
- **說明**: 刪除訪談記錄
- **路徑參數**:
  - `interviewId`: 訪談ID
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

#### 3.1.7 批次刪除訪談
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/interviews/batch`
- **說明**: 批次刪除多筆訪談記錄
- **請求格式**:
  ```json
  {
    "items": [1, 2, 3]
  }
  ```

#### 3.1.8 更新訪談狀態
- **HTTP 方法**: `PATCH`
- **路徑**: `/api/v1/interviews/{interviewId}/status`
- **說明**: 更新訪談狀態
- **請求格式**:
  ```json
  {
    "status": "CANCELLED"
  }
  ```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 訪談列表頁面 (`InterviewList.vue`)
- **路徑**: `/recruitment/interviews`
- **功能**: 顯示訪談列表，支援查詢、新增、修改、刪除
- **主要元件**:
  - 查詢表單 (InterviewSearchForm)
  - 資料表格 (InterviewDataTable)
  - 新增/修改對話框 (InterviewDialog)
  - 刪除確認對話框

#### 4.1.2 訪談詳細頁面 (`InterviewDetail.vue`)
- **路徑**: `/recruitment/interviews/:interviewId`
- **功能**: 顯示訪談詳細資料，支援修改

#### 4.1.3 潛客訪談記錄頁面 (`ProspectInterviews.vue`)
- **路徑**: `/recruitment/prospects/:prospectId/interviews`
- **功能**: 顯示指定潛客的所有訪談記錄

### 4.2 UI 元件設計

#### 4.2.1 查詢表單元件 (`InterviewSearchForm.vue`)
- 支援依潛客、訪談日期範圍、訪談結果、狀態、訪談人員查詢

#### 4.2.2 資料表格元件 (`InterviewDataTable.vue`)
- 顯示訪談基本資訊、潛客名稱、訪談日期、訪談結果
- 支援排序、分頁
- 提供修改、刪除、查看詳細資料操作

#### 4.2.3 新增/修改對話框 (`InterviewDialog.vue`)
- 表單欄位包含：潛客選擇、訪談日期時間、訪談類型、訪談人員、訪談地點、訪談內容、訪談結果、後續行動等
- 支援表單驗證

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 建立外鍵約束
- [ ] 建立預設值
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (4天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (4天)
- [ ] API 呼叫函數
- [ ] 列表頁面開發
- [ ] 新增/修改對話框開發
- [ ] 查詢表單開發
- [ ] 資料表格開發
- [ ] 表單驗證
- [ ] 元件測試

### 5.4 階段四: 整合測試 (1天)
- [ ] API 整合測試
- [ ] 端對端測試
- [ ] 效能測試
- [ ] 安全性測試

### 5.5 階段五: 文件與部署 (1天)
- [ ] API 文件更新
- [ ] 使用者手冊
- [ ] 部署文件

**總計**: 11天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 訪談記錄變更必須記錄操作日誌
- 敏感資訊必須加密儲存

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引
- 關聯查詢應使用 JOIN 優化

### 6.3 資料驗證
- 必填欄位必須驗證
- 訪談日期必須驗證
- 訪談結果值必須在允許範圍內
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 訪談記錄必須關聯到有效的潛客
- 訪談日期不能晚於今天
- 後續行動日期必須晚於訪談日期
- 訪談資料變更必須記錄變更資訊

### 6.5 關聯資料
- 與潛客的關聯（必填）
- 與使用者的關聯（訪談人員）

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增訪談成功
- [ ] 新增訪談失敗 (必填欄位驗證)
- [ ] 修改訪談成功
- [ ] 修改訪談失敗 (日期驗證)
- [ ] 刪除訪談成功
- [ ] 查詢訪談列表成功
- [ ] 查詢單筆訪談成功
- [ ] 根據潛客查詢訪談列表成功
- [ ] 更新訪談狀態成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試
- [ ] 關聯資料測試

### 7.3 效能測試
- [ ] 大量資料查詢測試
- [ ] 並發操作測試
- [ ] 關聯查詢效能測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSC000/SYSC222*.asp` (如存在)
- `IMS3/HANSHIN/IMS3/SYSC000/SYSC222.ascx` (如存在)

### 8.2 資料庫 Schema
- 舊系統相關資料表結構

---

