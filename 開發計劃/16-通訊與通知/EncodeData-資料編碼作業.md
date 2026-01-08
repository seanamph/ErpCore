# EncodeData - 資料編碼作業 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: EncodeData
- **功能名稱**: 資料編碼作業
- **功能描述**: 提供系統資料編碼/解碼功能，包含Base64編碼、字串加密、日期加密、資料編碼等
- **參考舊程式**: 
  - `WEB/IMS_CORE/SYS5000/EncodeData.aspx`
  - `IMS3/HANSHIN/RSL.KERNEL/DATA_REG.cs`
  - `IMS3/HANSHIN/RSL_CLASS/UTILITY/UtilClass.cs`
  - `IMS3/HANSHIN/IMS3/KERNEL/Encode_String.aspx`
  - `WEB/IMS_CORE/ASP/UTIL/Base64.asp`

### 1.2 業務需求
- 支援Base64編碼/解碼
- 支援字串加密/解密（字元轉3位數字->Base64）
- 支援日期加密/解密
- 支援資料編碼（用於系統註冊、授權等）
- 支援多種編碼方式
- 提供編碼/解碼工具介面

---

## 二、資料庫設計 (Schema)

### 2.1 編碼記錄表: `EncodeLogs` (編碼記錄)

```sql
CREATE TABLE [dbo].[EncodeLogs] (
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [EncodeType] NVARCHAR(50) NOT NULL, -- 編碼類型 (Base64, String, Date, Data)
    [OriginalData] NVARCHAR(MAX) NULL, -- 原始資料
    [EncodedData] NVARCHAR(MAX) NULL, -- 編碼後資料
    [KeyKind] NVARCHAR(50) NULL, -- 金鑰類型（用於字串加密）
    [CreatedBy] NVARCHAR(50) NULL, -- 建立者
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), -- 建立時間
    [Purpose] NVARCHAR(100) NULL -- 用途說明
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_EncodeLogs_EncodeType] ON [dbo].[EncodeLogs] ([EncodeType]);
CREATE NONCLUSTERED INDEX [IX_EncodeLogs_CreatedAt] ON [dbo].[EncodeLogs] ([CreatedAt]);
```

### 2.2 資料字典

#### EncodeLogs 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| Id | BIGINT | - | NO | IDENTITY(1,1) | 主鍵 | 自動遞增 |
| EncodeType | NVARCHAR | 50 | NO | - | 編碼類型 | Base64, String, Date, Data |
| OriginalData | NVARCHAR(MAX) | - | YES | - | 原始資料 | - |
| EncodedData | NVARCHAR(MAX) | - | YES | - | 編碼後資料 | - |
| KeyKind | NVARCHAR | 50 | YES | - | 金鑰類型 | 用於字串加密 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| Purpose | NVARCHAR | 100 | YES | - | 用途說明 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 Base64編碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/base64`
- **說明**: Base64編碼
- **請求格式**:
  ```json
  {
    "data": "原始資料",
    "saveLog": false
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "編碼成功",
    "data": {
      "originalData": "原始資料",
      "encodedData": "5Lit5paH5LiA5LiH5Lq65Lq6",
      "encodeType": "Base64"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 Base64解碼
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/base64/decode`
- **說明**: Base64解碼
- **請求格式**: 同編碼，但 `data` 為編碼後的資料
- **回應格式**: 同編碼

#### 3.1.3 字串加密
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/string`
- **說明**: 字串加密（字元轉3位數字->Base64）
- **請求格式**:
  ```json
  {
    "data": "原始資料",
    "keyKind": "1",
    "saveLog": false
  }
  ```
- **回應格式**: 同Base64編碼

#### 3.1.4 字串解密
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/string/decode`
- **說明**: 字串解密
- **請求格式**: 同加密，但 `data` 為加密後的資料
- **回應格式**: 同Base64編碼

#### 3.1.5 日期加密
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/date`
- **說明**: 日期加密
- **請求格式**:
  ```json
  {
    "data": "2024-01-01",
    "saveLog": false
  }
  ```
- **回應格式**: 同Base64編碼

#### 3.1.6 日期解密
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/date/decode`
- **說明**: 日期解密
- **請求格式**: 同加密，但 `data` 為加密後的資料
- **回應格式**: 同Base64編碼

#### 3.1.7 資料編碼（系統註冊用）
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/encode/data`
- **說明**: 資料編碼（用於系統註冊、授權等）
- **請求格式**:
  ```json
  {
    "data": {
      "projectId": "PRJ001",
      "userId": "U001",
      "expireDate": "2024-12-31"
    },
    "useDownGo": "0",
    "saveLog": false
  }
  ```
- **回應格式**: 同Base64編碼

---

## 四、前端 UI 設計

### 4.1 編碼工具頁面 (`EncodeData.vue`)

#### 4.1.1 頁面結構
```vue
<template>
  <div class="encode-data">
    <el-card>
      <template #header>
        <span>資料編碼工具</span>
      </template>
      
      <el-tabs v-model="activeTab">
        <!-- Base64編碼 -->
        <el-tab-pane label="Base64編碼" name="base64">
          <el-form :model="base64Form" label-width="120px">
            <el-form-item label="原始資料">
              <el-input 
                v-model="base64Form.data" 
                type="textarea"
                :rows="5"
                placeholder="請輸入要編碼的資料"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleBase64Encode">編碼</el-button>
              <el-button @click="handleBase64Decode">解碼</el-button>
              <el-button @click="handleBase64Clear">清除</el-button>
            </el-form-item>
            <el-form-item label="編碼結果">
              <el-input 
                v-model="base64Form.result" 
                type="textarea"
                :rows="5"
                readonly
              />
            </el-form-item>
          </el-form>
        </el-tab-pane>
        
        <!-- 字串加密 -->
        <el-tab-pane label="字串加密" name="string">
          <el-form :model="stringForm" label-width="120px">
            <el-form-item label="原始資料">
              <el-input 
                v-model="stringForm.data" 
                type="textarea"
                :rows="5"
                placeholder="請輸入要加密的資料"
              />
            </el-form-item>
            <el-form-item label="金鑰類型">
              <el-select v-model="stringForm.keyKind" placeholder="請選擇金鑰類型">
                <el-option label="金鑰1" value="1" />
                <el-option label="金鑰2" value="2" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleStringEncode">加密</el-button>
              <el-button @click="handleStringDecode">解密</el-button>
              <el-button @click="handleStringClear">清除</el-button>
            </el-form-item>
            <el-form-item label="加密結果">
              <el-input 
                v-model="stringForm.result" 
                type="textarea"
                :rows="5"
                readonly
              />
            </el-form-item>
          </el-form>
        </el-tab-pane>
        
        <!-- 日期加密 -->
        <el-tab-pane label="日期加密" name="date">
          <el-form :model="dateForm" label-width="120px">
            <el-form-item label="原始日期">
              <el-date-picker
                v-model="dateForm.data"
                type="date"
                placeholder="請選擇日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleDateEncode">加密</el-button>
              <el-button @click="handleDateDecode">解密</el-button>
              <el-button @click="handleDateClear">清除</el-button>
            </el-form-item>
            <el-form-item label="加密結果">
              <el-input 
                v-model="dateForm.result" 
                placeholder="加密後的資料"
                readonly
              />
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>
```

#### 4.1.2 腳本邏輯
```typescript
<script setup lang="ts">
import { ref, reactive } from 'vue'
import { ElMessage } from 'element-plus'
import { encodeApi } from '@/api/encode'

const activeTab = ref('base64')

// Base64表單
const base64Form = reactive({
  data: '',
  result: ''
})

// 字串表單
const stringForm = reactive({
  data: '',
  keyKind: '1',
  result: ''
})

// 日期表單
const dateForm = reactive({
  data: '',
  result: ''
})

// Base64編碼
const handleBase64Encode = async () => {
  if (!base64Form.data) {
    ElMessage.warning('請輸入要編碼的資料')
    return
  }
  
  try {
    const response = await encodeApi.base64Encode({
      data: base64Form.data,
      saveLog: false
    })
    
    if (response.success) {
      base64Form.result = response.data.encodedData
      ElMessage.success('編碼成功')
    } else {
      ElMessage.error(response.message || '編碼失敗')
    }
  } catch (error) {
    ElMessage.error('編碼失敗：' + error.message)
  }
}

// Base64解碼
const handleBase64Decode = async () => {
  if (!base64Form.data) {
    ElMessage.warning('請輸入要解碼的資料')
    return
  }
  
  try {
    const response = await encodeApi.base64Decode({
      data: base64Form.data,
      saveLog: false
    })
    
    if (response.success) {
      base64Form.result = response.data.originalData
      ElMessage.success('解碼成功')
    } else {
      ElMessage.error(response.message || '解碼失敗')
    }
  } catch (error) {
    ElMessage.error('解碼失敗：' + error.message)
  }
}

// Base64清除
const handleBase64Clear = () => {
  base64Form.data = ''
  base64Form.result = ''
}

// 字串加密
const handleStringEncode = async () => {
  if (!stringForm.data) {
    ElMessage.warning('請輸入要加密的資料')
    return
  }
  
  try {
    const response = await encodeApi.stringEncode({
      data: stringForm.data,
      keyKind: stringForm.keyKind,
      saveLog: false
    })
    
    if (response.success) {
      stringForm.result = response.data.encodedData
      ElMessage.success('加密成功')
    } else {
      ElMessage.error(response.message || '加密失敗')
    }
  } catch (error) {
    ElMessage.error('加密失敗：' + error.message)
  }
}

// 字串解密
const handleStringDecode = async () => {
  if (!stringForm.data) {
    ElMessage.warning('請輸入要解密的資料')
    return
  }
  
  try {
    const response = await encodeApi.stringDecode({
      data: stringForm.data,
      keyKind: stringForm.keyKind,
      saveLog: false
    })
    
    if (response.success) {
      stringForm.result = response.data.originalData
      ElMessage.success('解密成功')
    } else {
      ElMessage.error(response.message || '解密失敗')
    }
  } catch (error) {
    ElMessage.error('解密失敗：' + error.message)
  }
}

// 字串清除
const handleStringClear = () => {
  stringForm.data = ''
  stringForm.result = ''
}

// 日期加密
const handleDateEncode = async () => {
  if (!dateForm.data) {
    ElMessage.warning('請選擇要加密的日期')
    return
  }
  
  try {
    const response = await encodeApi.dateEncode({
      data: dateForm.data,
      saveLog: false
    })
    
    if (response.success) {
      dateForm.result = response.data.encodedData
      ElMessage.success('加密成功')
    } else {
      ElMessage.error(response.message || '加密失敗')
    }
  } catch (error) {
    ElMessage.error('加密失敗：' + error.message)
  }
}

// 日期解密
const handleDateDecode = async () => {
  if (!dateForm.result) {
    ElMessage.warning('請輸入要解密的資料')
    return
  }
  
  try {
    const response = await encodeApi.dateDecode({
      data: dateForm.result,
      saveLog: false
    })
    
    if (response.success) {
      dateForm.data = response.data.originalData
      ElMessage.success('解密成功')
    } else {
      ElMessage.error(response.message || '解密失敗')
    }
  } catch (error) {
    ElMessage.error('解密失敗：' + error.message)
  }
}

// 日期清除
const handleDateClear = () => {
  dateForm.data = ''
  dateForm.result = ''
}
</script>
```

---

## 五、後端實作

### 5.1 Service (`EncodeService.cs`)

```csharp
public class EncodeService : IEncodeService
{
    private readonly IDbConnection _db;
    private readonly ILogger<EncodeService> _logger;

    public EncodeService(
        IDbConnection db,
        ILogger<EncodeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<EncodeResultDto> Base64EncodeAsync(string data, bool saveLog = false)
    {
        try
        {
            var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));

            if (saveLog)
            {
                await SaveEncodeLogAsync("Base64", data, encodedData, null);
            }

            return new EncodeResultDto
            {
                OriginalData = data,
                EncodedData = encodedData,
                EncodeType = "Base64"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Base64編碼失敗");
            throw new BusinessException("Base64編碼失敗：" + ex.Message);
        }
    }

    public async Task<EncodeResultDto> Base64DecodeAsync(string data, bool saveLog = false)
    {
        try
        {
            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(data));

            if (saveLog)
            {
                await SaveEncodeLogAsync("Base64", decodedData, data, null);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = data,
                EncodeType = "Base64"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Base64解碼失敗");
            throw new BusinessException("Base64解碼失敗：" + ex.Message);
        }
    }

    public async Task<EncodeResultDto> StringEncodeAsync(string data, string keyKind = "1", bool saveLog = false)
    {
        try
        {
            // 字元轉3位數字
            var charEncoded = CharEncGen(data, keyKind);
            
            // Base64編碼
            var encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(charEncoded));

            if (saveLog)
            {
                await SaveEncodeLogAsync("String", data, encodedData, keyKind);
            }

            return new EncodeResultDto
            {
                OriginalData = data,
                EncodedData = encodedData,
                EncodeType = "String"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "字串加密失敗");
            throw new BusinessException("字串加密失敗：" + ex.Message);
        }
    }

    public async Task<EncodeResultDto> StringDecodeAsync(string data, string keyKind = "1", bool saveLog = false)
    {
        try
        {
            // Base64解碼
            var base64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            
            // 3位數字轉字元
            var decodedData = CharDecGen(base64Decoded, keyKind);

            if (saveLog)
            {
                await SaveEncodeLogAsync("String", decodedData, data, keyKind);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = data,
                EncodeType = "String"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "字串解密失敗");
            throw new BusinessException("字串解密失敗：" + ex.Message);
        }
    }

    public async Task<EncodeResultDto> DateEncodeAsync(string data, bool saveLog = false)
    {
        try
        {
            var encodedData = DateEncGen(data);

            if (saveLog)
            {
                await SaveEncodeLogAsync("Date", data, encodedData, null);
            }

            return new EncodeResultDto
            {
                OriginalData = data,
                EncodedData = encodedData,
                EncodeType = "Date"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "日期加密失敗");
            throw new BusinessException("日期加密失敗：" + ex.Message);
        }
    }

    public async Task<EncodeResultDto> DateDecodeAsync(string data, bool saveLog = false)
    {
        try
        {
            var decodedData = DateDecGen(data);

            if (saveLog)
            {
                await SaveEncodeLogAsync("Date", decodedData, data, null);
            }

            return new EncodeResultDto
            {
                OriginalData = decodedData,
                EncodedData = data,
                EncodeType = "Date"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "日期解密失敗");
            throw new BusinessException("日期解密失敗：" + ex.Message);
        }
    }

    private string CharEncGen(string source, string keyKind = "1")
    {
        // 實作字元轉3位數字的邏輯
        // 參考舊程式 UtilClass.CharEncGen
        var result = "";
        foreach (var ch in source)
        {
            var charCode = (int)ch;
            result += charCode.ToString("D3");
        }
        return result;
    }

    private string CharDecGen(string source, string keyKind = "1")
    {
        // 實作3位數字轉字元的邏輯
        // 參考舊程式 UtilClass.CharDecGen
        var result = "";
        for (int i = 0; i < source.Length; i += 3)
        {
            if (i + 3 <= source.Length)
            {
                var charCode = int.Parse(source.Substring(i, 3));
                result += (char)charCode;
            }
        }
        return result;
    }

    private string DateEncGen(string dateStr)
    {
        // 實作日期加密邏輯
        // 參考舊程式 UtilClass.DateEncGen
        var result = "";
        foreach (var ch in dateStr)
        {
            var j = (255 - (int)ch) - 180;
            result += j.ToString("D2");
        }
        return result;
    }

    private string DateDecGen(string encodedStr)
    {
        // 實作日期解密邏輯
        // 參考舊程式 UtilClass.DateDecGen
        var result = "";
        for (int i = 0; i < encodedStr.Length; i += 2)
        {
            if (i + 2 <= encodedStr.Length)
            {
                var j = int.Parse(encodedStr.Substring(i, 2)) + 180;
                result += (char)(255 - j);
            }
        }
        return result;
    }

    private async Task SaveEncodeLogAsync(string encodeType, string originalData, string encodedData, string keyKind)
    {
        var sql = @"
            INSERT INTO EncodeLogs (EncodeType, OriginalData, EncodedData, KeyKind, CreatedBy, CreatedAt)
            VALUES (@EncodeType, @OriginalData, @EncodedData, @KeyKind, @CreatedBy, GETDATE());
        ";

        await _db.ExecuteAsync(sql, new
        {
            EncodeType = encodeType,
            OriginalData = originalData,
            EncodedData = encodedData,
            KeyKind = keyKind,
            CreatedBy = GetCurrentUserId()
        });
    }

    private string GetCurrentUserId()
    {
        // 從 JWT Token 或 Session 取得目前使用者 ID
        return "U001"; // 暫時返回固定值，需實作
    }
}
```

---

## 六、開發時程

### 6.1 階段一: 資料庫設計 (0.5天)
- [ ] 建立 EncodeLogs 資料表
- [ ] 建立索引
- [ ] 資料庫遷移腳本

### 6.2 階段二: 後端開發 (2天)
- [ ] Entity 類別建立
- [ ] EncodeService 實作
- [ ] Controller 實作
- [ ] DTO 類別建立
- [ ] 編碼/解碼演算法實作
- [ ] 單元測試

### 6.3 階段三: 前端開發 (1.5天)
- [ ] API 呼叫函數
- [ ] 編碼工具頁面開發
- [ ] 表單驗證
- [ ] 元件測試

### 6.4 階段四: 整合測試 (0.5天)
- [ ] API 整合測試
- [ ] 編碼/解碼正確性測試
- [ ] 端對端測試

**總計**: 4.5天

---

## 七、注意事項

### 7.1 安全性
- 編碼記錄可能包含敏感資料，需考慮是否記錄
- 提供選項控制是否記錄編碼日誌
- 考慮資料加密儲存

### 7.2 編碼演算法
- 需確保編碼/解碼演算法與舊系統一致
- 需測試各種資料格式的編碼/解碼
- 需處理特殊字元編碼

---

## 八、測試案例

### 8.1 功能測試
1. **Base64編碼/解碼測試**
   - 正常資料編碼/解碼
   - 特殊字元編碼/解碼
   - 中文資料編碼/解碼
   - 空資料處理

2. **字串加密/解密測試**
   - 正常資料加密/解密
   - 不同金鑰類型測試
   - 特殊字元處理

3. **日期加密/解密測試**
   - 正常日期加密/解密
   - 不同日期格式處理

### 8.2 正確性測試
- 編碼後再解碼應與原始資料一致
- 與舊系統編碼結果比對

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

