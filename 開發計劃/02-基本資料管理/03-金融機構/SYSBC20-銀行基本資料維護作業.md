# SYSBC20 - 銀行基本資料維護作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSBC20
- **功能名稱**: 銀行基本資料維護作業
- **功能描述**: 提供銀行基本資料的新增、修改、刪除、查詢功能，包含銀行代號、銀行名稱、帳號長度、狀態、銀行種類等資訊管理
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYSB000/SYSBC20_FI1.aspx` (新增)
  - `WEB/IMS_CORE/SYSB000/SYSBC20_FU1.aspx` (修改)
  - `WEB/IMS_CORE/SYSB000/SYSBC20_FD.aspx` (刪除)
  - `WEB/IMS_CORE/SYSB000/SYSBC20_PR.rdlc` (報表)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/BANK.cs` (業務邏輯)

### 1.2 業務需求
- 管理系統銀行基本資料
- 支援銀行的新增、修改、刪除、查詢
- 記錄銀行的建立與變更資訊
- 支援帳號長度驗證設定
- 支援銀行種類分類
- 支援銀行狀態管理（啟用/停用）
- 支援排序序號設定

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Banks` (對應舊系統 `BANK`)

```sql
CREATE TABLE [dbo].[Banks] (
    [BankId] NVARCHAR(50) NOT NULL PRIMARY KEY, -- 銀行代號 (BANK_ID)
    [BankName] NVARCHAR(100) NOT NULL, -- 銀行名稱 (BANK_NAME)
    [AcctLen] INT NULL, -- 帳號最小長度 (ACCT_LEN)
    [AcctLenMax] INT NULL, -- 帳號最大長度 (ACCT_LEN_MAX)
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (STATUS) 1:啟用, 0:停用
    [BankKind] NVARCHAR(10) NULL, -- 銀行種類 (BANK_KIND) 1:銀行, 2:郵局, 3:信用合作社
    [SeqNo] INT NULL DEFAULT 0, -- 排序序號 (SEQ_NO)
    [Notes] NVARCHAR(500) NULL, -- 備註 (NOTES)
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者 (BUSER)
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間 (BTIME)
    [UpdatedBy] NVARCHAR(50) NULL, -- 更新者 (CUSER)
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 更新時間 (CTIME)
    [CreatedPriority] INT NULL, -- 建立者等級 (CPRIORITY)
    [CreatedGroup] NVARCHAR(50) NULL, -- 建立者群組 (CGROUP)
    CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED ([BankId] ASC)
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Banks_BankName] ON [dbo].[Banks] ([BankName]);
CREATE NONCLUSTERED INDEX [IX_Banks_Status] ON [dbo].[Banks] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Banks_BankKind] ON [dbo].[Banks] ([BankKind]);
CREATE NONCLUSTERED INDEX [IX_Banks_SeqNo] ON [dbo].[Banks] ([SeqNo]);
```

### 2.2 相關資料表
無

### 2.3 資料字典

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| BankId | NVARCHAR | 50 | NO | - | 銀行代號 | 主鍵 |
| BankName | NVARCHAR | 100 | NO | - | 銀行名稱 | - |
| AcctLen | INT | - | YES | - | 帳號最小長度 | - |
| AcctLenMax | INT | - | YES | - | 帳號最大長度 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| BankKind | NVARCHAR | 10 | YES | - | 銀行種類 | 1:銀行, 2:郵局, 3:信用合作社 |
| SeqNo | INT | - | YES | 0 | 排序序號 | - |
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

#### 3.1.1 查詢銀行列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/banks`
- **說明**: 查詢銀行列表，支援分頁、排序、篩選
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 20,
    "sortField": "BankId",
    "sortOrder": "ASC",
    "filters": {
      "bankId": "",
      "bankName": "",
      "status": "",
      "bankKind": ""
    }
  }
  ```
- **回應格式**: 同地區設定

#### 3.1.2 查詢單筆銀行
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/banks/{bankId}`
- **說明**: 根據銀行代號查詢單筆銀行資料

#### 3.1.3 新增銀行
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/banks`
- **說明**: 新增銀行資料
- **請求格式**:
  ```json
  {
    "bankId": "B001",
    "bankName": "台灣銀行",
    "acctLen": 10,
    "acctLenMax": 14,
    "status": "1",
    "bankKind": "1",
    "seqNo": 1,
    "notes": "備註"
  }
  ```

#### 3.1.4 修改銀行
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/banks/{bankId}`
- **說明**: 修改銀行資料

#### 3.1.5 刪除銀行
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/banks/{bankId}`
- **說明**: 刪除銀行資料

### 3.2 後端實作類別

#### 3.2.1 Controller: `BanksController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/banks")]
    [Authorize]
    public class BanksController : ControllerBase
    {
        private readonly IBankService _bankService;
        
        public BanksController(IBankService bankService)
        {
            _bankService = bankService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<BankDto>>>> GetBanks([FromQuery] BankQueryDto query)
        {
            // 實作查詢邏輯
        }
        
        [HttpGet("{bankId}")]
        public async Task<ActionResult<ApiResponse<BankDto>>> GetBank(string bankId)
        {
            // 實作查詢單筆邏輯
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateBank([FromBody] CreateBankDto dto)
        {
            // 實作新增邏輯
        }
        
        [HttpPut("{bankId}")]
        public async Task<ActionResult<ApiResponse>> UpdateBank(string bankId, [FromBody] UpdateBankDto dto)
        {
            // 實作修改邏輯
        }
        
        [HttpDelete("{bankId}")]
        public async Task<ActionResult<ApiResponse>> DeleteBank(string bankId)
        {
            // 實作刪除邏輯
        }
    }
}
```

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 銀行列表頁面 (`BankList.vue`)
- **路徑**: `/master-data/banks`
- **功能**: 顯示銀行列表，支援查詢、新增、修改、刪除

### 4.2 UI 元件設計

#### 4.2.1 新增/修改對話框 (`BankDialog.vue`)
```vue
<template>
  <el-dialog
    :title="dialogTitle"
    v-model="visible"
    width="700px"
    @close="handleClose"
  >
    <el-form :model="form" :rules="rules" ref="formRef" label-width="140px">
      <el-form-item label="銀行代號" prop="bankId">
        <el-input v-model="form.bankId" :disabled="isEdit" placeholder="請輸入銀行代號" />
      </el-form-item>
      <el-form-item label="銀行名稱" prop="bankName">
        <el-input v-model="form.bankName" placeholder="請輸入銀行名稱" />
      </el-form-item>
      <el-form-item label="帳號最小長度" prop="acctLen">
        <el-input-number v-model="form.acctLen" :min="0" :max="20" />
      </el-form-item>
      <el-form-item label="帳號最大長度" prop="acctLenMax">
        <el-input-number v-model="form.acctLenMax" :min="0" :max="20" />
      </el-form-item>
      <el-form-item label="狀態" prop="status">
        <el-select v-model="form.status" placeholder="請選擇狀態">
          <el-option label="啟用" value="1" />
          <el-option label="停用" value="0" />
        </el-select>
      </el-form-item>
      <el-form-item label="銀行種類" prop="bankKind">
        <el-select v-model="form.bankKind" placeholder="請選擇銀行種類">
          <el-option label="銀行" value="1" />
          <el-option label="郵局" value="2" />
          <el-option label="信用合作社" value="3" />
        </el-select>
      </el-form-item>
      <el-form-item label="排序序號" prop="seqNo">
        <el-input-number v-model="form.seqNo" :min="0" />
      </el-form-item>
      <el-form-item label="備註" prop="notes">
        <el-input v-model="form.notes" type="textarea" :rows="3" placeholder="請輸入備註" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">取消</el-button>
      <el-button type="primary" @click="handleSubmit">確定</el-button>
    </template>
  </el-dialog>
</template>
```

---

## 五、開發時程

### 5.1 階段一: 資料庫設計 (1天)
- [ ] 建立資料表結構
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 5.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] Repository 實作
- [ ] Service 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 驗證邏輯實作
- [ ] 單元測試

### 5.3 階段三: 前端開發 (2天)
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

### 5.5 階段五: 文件與部署 (0.5天)
- [ ] API 文件更新
- [ ] 使用者手冊

**總計**: 6.5天

---

## 六、注意事項

### 6.1 安全性
- 必須實作權限檢查
- 敏感資料必須加密傳輸 (HTTPS)

### 6.2 效能
- 大量資料查詢必須使用分頁
- 必須建立適當的索引

### 6.3 資料驗證
- 銀行代號必須唯一
- 銀行名稱必填
- 帳號長度必須合理（最小長度 <= 最大長度）
- 狀態值必須在允許範圍內

### 6.4 業務邏輯
- 刪除銀行前必須檢查是否有相關資料引用
- 銀行代號不可修改

---

## 七、測試案例

### 7.1 單元測試
- [ ] 新增銀行成功
- [ ] 新增銀行失敗 (重複代號)
- [ ] 修改銀行成功
- [ ] 修改銀行失敗 (不存在)
- [ ] 刪除銀行成功
- [ ] 查詢銀行列表成功
- [ ] 查詢單筆銀行成功

### 7.2 整合測試
- [ ] 完整 CRUD 流程測試
- [ ] 權限檢查測試
- [ ] 資料驗證測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/BANK.cs`
- `WEB/IMS_CORE/SYSB000/SYSBC20_FI1.aspx`
- `WEB/IMS_CORE/SYSB000/SYSBC20_FU1.aspx`
- `WEB/IMS_CORE/SYSB000/SYSBC20_FD.aspx`

### 8.2 資料庫 Schema
- 舊系統: `BANK` 表

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

