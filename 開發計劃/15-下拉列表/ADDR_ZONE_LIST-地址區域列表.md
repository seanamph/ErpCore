# ADDR_ZONE_LIST - 地址區域列表 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: ADDR_ZONE_LIST
- **功能名稱**: 地址區域列表
- **功能描述**: 提供地址區域選擇的下拉列表功能，支援區域查詢、篩選、選擇等功能，用於地址輸入時的區域選擇，需配合城市選擇使用
- **參考舊程式**: 
  - `IMS3/HANSHIN/IMS3/ETEK_LIST/ADDR_ZONE_LIST.aspx` (ASP.NET版本)
  - `WEB/IMS_CORE/ASP/Kernel/ADDR_ZONE_LIST.asp` (ASP版本)
  - `WEB/IMS_CORE/ASP/Kernel/ADDR_ZONE_LIST1.asp` (ASP版本1)
  - `WEB/IMS_CORE/ASP/Kernel/ADDR_ZONE_LIST2.asp` (ASP版本2)
  - `IMS3/HANSHIN/RSL_CLASS/IMS3_BASE/ZONEClass.cs` (業務邏輯)

### 1.2 業務需求
- 提供區域列表查詢功能
- 支援依城市篩選區域
- 支援區域名稱篩選
- 支援區域選擇並回傳區域代碼和郵遞區號
- 支援多選模式（可選）
- 支援排序功能
- 與地址城市列表（ADDR_CITY_LIST）整合
- 與地址其他列表（ADDR_OTHER_LIST）整合

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表: `Zones` (區域主檔)

```sql
CREATE TABLE [dbo].[Zones] (
    [ZoneId] NVARCHAR(20) NOT NULL PRIMARY KEY, -- 區域代碼 (ZONE)
    [ZoneName] NVARCHAR(100) NOT NULL, -- 區域名稱 (ZONE_NAME)
    [CityId] NVARCHAR(20) NOT NULL, -- 城市代碼 (CITY，外鍵至Cities)
    [ZipCode] NVARCHAR(10) NULL, -- 郵遞區號 (AREA_CODE)
    [SeqNo] INT NULL, -- 排序序號
    [Status] NVARCHAR(10) NOT NULL DEFAULT '1', -- 狀態 (1:啟用, 0:停用)
    [CreatedBy] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED ([ZoneId] ASC),
    CONSTRAINT [FK_Zones_Cities] FOREIGN KEY ([CityId]) REFERENCES [dbo].[Cities] ([CityId])
);

-- 索引
CREATE NONCLUSTERED INDEX [IX_Zones_ZoneName] ON [dbo].[Zones] ([ZoneName]);
CREATE NONCLUSTERED INDEX [IX_Zones_CityId] ON [dbo].[Zones] ([CityId]);
CREATE NONCLUSTERED INDEX [IX_Zones_ZipCode] ON [dbo].[Zones] ([ZipCode]);
CREATE NONCLUSTERED INDEX [IX_Zones_Status] ON [dbo].[Zones] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Zones_SeqNo] ON [dbo].[Zones] ([SeqNo]);
```

### 2.2 相關資料表

#### 2.2.1 `Cities` - 城市主檔
- 參考: `開發計劃/15-下拉列表/ADDR_CITY_LIST-地址城市列表.md`

### 2.3 資料字典

#### Zones 資料表

| 欄位名稱 | 資料類型 | 長度 | 允許NULL | 預設值 | 說明 | 備註 |
|---------|---------|------|---------|--------|------|------|
| ZoneId | NVARCHAR | 20 | NO | - | 區域代碼 | 主鍵 |
| ZoneName | NVARCHAR | 100 | NO | - | 區域名稱 | - |
| CityId | NVARCHAR | 20 | NO | - | 城市代碼 | 外鍵至Cities |
| ZipCode | NVARCHAR | 10 | YES | - | 郵遞區號 | - |
| SeqNo | INT | - | YES | - | 排序序號 | - |
| Status | NVARCHAR | 10 | NO | '1' | 狀態 | 1:啟用, 0:停用 |
| CreatedBy | NVARCHAR | 50 | YES | - | 建立者 | - |
| CreatedAt | DATETIME2 | - | NO | GETDATE() | 建立時間 | - |
| UpdatedBy | NVARCHAR | 50 | YES | - | 更新者 | - |
| UpdatedAt | DATETIME2 | - | NO | GETDATE() | 更新時間 | - |

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢區域列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/zones`
- **說明**: 查詢區域列表，支援依城市篩選、區域名稱篩選、排序、分頁
- **請求參數**:
  ```json
  {
    "pageIndex": 1,
    "pageSize": 50,
    "sortField": "ZoneName",
    "sortOrder": "ASC",
    "filters": {
      "cityId": "",
      "zoneName": "",
      "zipCode": "",
      "status": "1"
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
          "zoneId": "Z001",
          "zoneName": "中正區",
          "cityId": "C001",
          "cityName": "台北市",
          "zipCode": "100",
          "seqNo": 1,
          "status": "1"
        }
      ],
      "totalCount": 100,
      "pageIndex": 1,
      "pageSize": 50,
      "totalPages": 2
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 查詢單筆區域
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/zones/{zoneId}`
- **說明**: 根據區域代碼查詢單筆區域資料
- **路徑參數**:
  - `zoneId`: 區域代碼
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "zoneId": "Z001",
      "zoneName": "中正區",
      "cityId": "C001",
      "cityName": "台北市",
      "zipCode": "100",
      "seqNo": 1,
      "status": "1"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.3 依城市查詢區域列表
- **HTTP 方法**: `GET`
- **路徑**: `/api/v1/lists/zones/by-city/{cityId}`
- **說明**: 根據城市代碼查詢該城市下的所有區域
- **路徑參數**:
  - `cityId`: 城市代碼
- **回應格式**: 同查詢區域列表

#### 3.1.4 新增區域
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/lists/zones`
- **說明**: 新增區域資料
- **請求格式**:
  ```json
  {
    "zoneId": "Z001",
    "zoneName": "中正區",
    "cityId": "C001",
    "zipCode": "100",
    "seqNo": 1,
    "status": "1"
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "新增成功",
    "data": {
      "zoneId": "Z001"
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.5 修改區域
- **HTTP 方法**: `PUT`
- **路徑**: `/api/v1/lists/zones/{zoneId}`
- **說明**: 修改區域資料
- **路徑參數**:
  - `zoneId`: 區域代碼
- **請求格式**: 同新增，但 `zoneId` 不可修改
- **回應格式**: 同新增

#### 3.1.6 刪除區域
- **HTTP 方法**: `DELETE`
- **路徑**: `/api/v1/lists/zones/{zoneId}`
- **說明**: 刪除區域資料（需檢查是否有關聯資料）
- **路徑參數**:
  - `zoneId`: 區域代碼
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

---

## 四、前端 UI 設計

### 4.1 區域選擇下拉列表組件 (`AddrZoneList.vue`)

#### 4.1.1 組件結構
```vue
<template>
  <el-select
    v-model="selectedZone"
    :placeholder="placeholder"
    :clearable="clearable"
    :filterable="filterable"
    :multiple="multiple"
    :disabled="disabled"
    @change="handleChange"
    @visible-change="handleVisibleChange"
  >
    <el-option
      v-for="zone in zoneList"
      :key="zone.zoneId"
      :label="zone.zoneName"
      :value="zone.zoneId"
    >
      <span>{{ zone.zoneName }}</span>
      <span style="color: #8492a6; font-size: 13px; margin-left: 10px">
        ({{ zone.zipCode }})
      </span>
    </el-option>
  </el-select>
</template>
```

#### 4.1.2 組件屬性
- `cityId`: 城市代碼（必填，用於篩選區域）
- `modelValue`: 選中的區域代碼
- `placeholder`: 提示文字
- `clearable`: 是否可清除
- `filterable`: 是否可搜尋
- `multiple`: 是否多選
- `disabled`: 是否禁用

#### 4.1.3 組件事件
- `update:modelValue`: 選中值變更時觸發
- `change`: 選中值變更時觸發，回傳選中的區域資料

### 4.2 區域選擇對話框 (`AddrZoneListDialog.vue`)

#### 4.2.1 對話框結構
參考 `ADDR_CITY_LIST-地址城市列表.md` 的對話框設計，主要差異在於：
- 需先選擇城市
- 顯示區域列表和郵遞區號
- 選擇後回傳區域代碼和郵遞區號

---

## 五、後端實作

### 5.1 Controller (`ZoneListController.cs`)

```csharp
[ApiController]
[Route("api/v1/lists/zones")]
[Authorize]
public class ZoneListController : ControllerBase
{
    private readonly IZoneListService _zoneListService;
    private readonly ILogger<ZoneListController> _logger;

    public ZoneListController(
        IZoneListService zoneListService,
        ILogger<ZoneListController> logger)
    {
        _zoneListService = zoneListService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢區域列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ZoneDto>>>> GetZones(
        [FromQuery] ZoneListQueryDto query)
    {
        try
        {
            var result = await _zoneListService.GetZonesAsync(query);
            return Ok(ApiResponse<PagedResult<ZoneDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢區域列表失敗");
            return BadRequest(ApiResponse<PagedResult<ZoneDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 查詢單筆區域
    /// </summary>
    [HttpGet("{zoneId}")]
    public async Task<ActionResult<ApiResponse<ZoneDto>>> GetZone(string zoneId)
    {
        try
        {
            var result = await _zoneListService.GetZoneByIdAsync(zoneId);
            if (result == null)
            {
                return NotFound(ApiResponse<ZoneDto>.Error("區域不存在"));
            }
            return Ok(ApiResponse<ZoneDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢區域失敗");
            return BadRequest(ApiResponse<ZoneDto>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 依城市查詢區域列表
    /// </summary>
    [HttpGet("by-city/{cityId}")]
    public async Task<ActionResult<ApiResponse<List<ZoneDto>>>> GetZonesByCity(string cityId)
    {
        try
        {
            var result = await _zoneListService.GetZonesByCityIdAsync(cityId);
            return Ok(ApiResponse<List<ZoneDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢區域列表失敗");
            return BadRequest(ApiResponse<List<ZoneDto>>.Error("查詢失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 新增區域
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateZone([FromBody] CreateZoneDto dto)
    {
        try
        {
            await _zoneListService.CreateZoneAsync(dto);
            return Ok(ApiResponse<object>.Success(new { zoneId = dto.ZoneId }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增區域失敗");
            return BadRequest(ApiResponse<object>.Error("新增失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 修改區域
    /// </summary>
    [HttpPut("{zoneId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateZone(
        string zoneId,
        [FromBody] UpdateZoneDto dto)
    {
        try
        {
            dto.ZoneId = zoneId;
            await _zoneListService.UpdateZoneAsync(dto);
            return Ok(ApiResponse<object>.Success(null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改區域失敗");
            return BadRequest(ApiResponse<object>.Error("修改失敗：" + ex.Message));
        }
    }

    /// <summary>
    /// 刪除區域
    /// </summary>
    [HttpDelete("{zoneId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteZone(string zoneId)
    {
        try
        {
            await _zoneListService.DeleteZoneAsync(zoneId);
            return Ok(ApiResponse<object>.Success(null));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除區域失敗");
            return BadRequest(ApiResponse<object>.Error("刪除失敗：" + ex.Message));
        }
    }
}
```

### 5.2 Service (`ZoneListService.cs`)

參考 `ADDR_CITY_LIST-地址城市列表.md` 的 Service 實作，主要差異在於：
- 查詢需依城市篩選
- 包含郵遞區號欄位

---

## 六、開發時程

### 6.1 開發階段
1. **資料庫設計** (0.5 天)
   - 建立 Zones 資料表
   - 建立索引
   - 建立外鍵約束

2. **後端 API 開發** (2 天)
   - 實作 Service 層
   - 實作 Controller 層
   - 單元測試

3. **前端 UI 開發** (1.5 天)
   - 區域選擇下拉列表組件
   - 區域選擇對話框
   - 整合測試

4. **測試與優化** (0.5 天)
   - 功能測試
   - 效能測試
   - Bug 修復

**總計**: 4.5 天

---

## 七、注意事項

### 7.1 資料關聯
- 區域必須屬於某個城市
- 刪除城市時需處理區域資料（級聯刪除或限制刪除）

### 7.2 效能優化
- 依城市查詢時使用索引
- 大量資料時使用分頁查詢
- 快取常用的區域列表

### 7.3 資料驗證
- 區域代碼唯一性驗證
- 城市代碼存在性驗證
- 郵遞區號格式驗證

---

## 八、測試案例

### 8.1 功能測試
1. **查詢測試**
   - 查詢所有區域
   - 依城市查詢區域
   - 依區域名稱篩選
   - 依郵遞區號篩選
   - 分頁查詢

2. **新增測試**
   - 正常新增區域
   - 區域代碼重複驗證
   - 城市代碼不存在驗證

3. **修改測試**
   - 正常修改區域
   - 修改城市代碼驗證

4. **刪除測試**
   - 刪除區域
   - 有關聯資料時限制刪除

### 8.2 整合測試
- 與城市列表整合
- 與地址輸入表單整合

