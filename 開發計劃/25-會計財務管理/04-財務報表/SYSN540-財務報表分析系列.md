# SYSN540 - 財務報表分析系列 開發計劃

## 一、功能概述

### 1.1 功能說明
- **功能代碼**: SYSN540系列
- **功能名稱**: 財務報表分析系列
- **功能描述**: 提供財務報表的分析功能，包含財務比率分析、趨勢分析等
- **參考舊程式**: 
  - `WEB/IMS_CORE/ASP/SYSN000/SYSN540_FQ.ASP` (查詢)

### 1.2 業務需求
- 支援多種財務分析類型
- 支援分析資料匯出
- 支援分析圖表顯示

---

## 二、資料庫設計 (Schema)

### 2.1 主要資料表

參考 SYSN510-SYSN511-財務報表查詢系列.md 的資料表設計，使用相同的財務交易、會計科目等相關資料表。

### 2.2 財務分析視圖

#### 2.2.1 `v_FinancialReportAnalysis` - 財務報表分析視圖
```sql
CREATE VIEW [dbo].[v_FinancialReportAnalysis] AS
SELECT 
    ft.TransactionId,
    ft.TransactionDate,
    ft.AccountSubjectId,
    asub.AccountSubjectName,
    asub.AccountSubjectType,
    ft.DebitAmount,
    ft.CreditAmount,
    ft.Balance,
    -- 分析所需欄位
    YEAR(ft.TransactionDate) AS AnalysisYear,
    MONTH(ft.TransactionDate) AS AnalysisMonth,
    -- 財務比率計算欄位
    CASE 
        WHEN ft.CreditAmount > 0 THEN ft.DebitAmount / NULLIF(ft.CreditAmount, 0)
        ELSE NULL
    END AS RatioValue
FROM [dbo].[FinancialTransactions] ft
LEFT JOIN [dbo].[AccountSubjects] asub ON ft.AccountSubjectId = asub.AccountSubjectId;
```

### 2.3 資料字典

本功能為報表分析功能，主要使用財務交易、會計科目等相關資料表，不涉及資料的新增、修改、刪除操作。

---

## 三、後端 API 設計

### 3.1 API 端點列表

#### 3.1.1 查詢財務分析
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-reports/analysis`
- **說明**: 查詢財務分析資料，支援多種分析類型（財務比率分析、趨勢分析等）
- **請求格式**:
  ```json
  {
    "analysisType": "RATIO", // RATIO:比率分析, TREND:趨勢分析, COMPARISON:比較分析
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "accountSubjectIds": ["SUB001", "SUB002"],
    "analysisPeriod": "MONTHLY" // DAILY:日, MONTHLY:月, QUARTERLY:季, YEARLY:年
  }
  ```
- **回應格式**:
  ```json
  {
    "success": true,
    "code": 200,
    "message": "查詢成功",
    "data": {
      "analysisType": "RATIO",
      "period": "MONTHLY",
      "items": [
        {
          "period": "2024-01",
          "accountSubjectId": "SUB001",
          "accountSubjectName": "現金",
          "ratioValue": 1.25,
          "trendValue": 100.5,
          "comparisonValue": 105.2
        }
      ],
      "summary": {
        "totalRatio": 1.25,
        "averageTrend": 100.5,
        "maxValue": 150.0,
        "minValue": 80.0
      }
    },
    "timestamp": "2024-01-01T00:00:00Z"
  }
  ```

#### 3.1.2 匯出財務分析資料
- **HTTP 方法**: `POST`
- **路徑**: `/api/v1/financial-reports/analysis/export`
- **說明**: 匯出財務分析資料（Excel、PDF）
- **請求格式**: 同查詢財務分析
- **回應格式**: 檔案下載

### 3.2 後端實作類別

#### 3.2.1 Controller: `FinancialReportAnalysisController.cs`
```csharp
namespace RSL.IMS3.Api.Controllers
{
    [ApiController]
    [Route("api/v1/financial-reports/analysis")]
    [Authorize]
    public class FinancialReportAnalysisController : ControllerBase
    {
        private readonly IFinancialReportAnalysisService _service;
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FinancialAnalysisDto>>> GetAnalysis([FromBody] FinancialAnalysisQueryDto dto)
        {
            var result = await _service.GetAnalysisAsync(dto);
            return Ok(ApiResponse<FinancialAnalysisDto>.Success(result));
        }
        
        [HttpPost("export")]
        public async Task<IActionResult> ExportAnalysis([FromBody] FinancialAnalysisQueryDto dto)
        {
            var file = await _service.ExportAnalysisAsync(dto);
            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
```

### 3.3 功能說明

**本功能為報表分析功能，僅提供查詢和匯出功能，不涉及資料的新增、修改、刪除操作。**

---

## 四、前端 UI 設計

### 4.1 頁面結構

#### 4.1.1 財務報表分析頁面 (`FinancialReportAnalysis.vue`)
- **路徑**: `/accounting/financial-reports/analysis`
- **功能**: 顯示財務報表分析介面，支援分析圖表、資料匯出

### 4.2 UI 元件設計

#### 4.2.1 分析條件表單
- 分析類型選擇（比率分析、趨勢分析、比較分析）
- 日期範圍選擇
- 會計科目選擇（多選）
- 分析期間選擇（日、月、季、年）

#### 4.2.2 分析結果顯示
- 圖表顯示（折線圖、柱狀圖、餅圖等）
- 資料表格顯示
- 統計摘要顯示

#### 4.2.3 匯出功能
- Excel匯出
- PDF匯出

### 4.3 頁面範例

```vue
<template>
  <div class="financial-report-analysis">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>財務報表分析</span>
          <el-button type="primary" @click="handleExport" :icon="Download">匯出</el-button>
        </div>
      </template>
      
      <!-- 查詢條件 -->
      <el-form :model="queryForm" :inline="true">
        <el-form-item label="分析類型">
          <el-select v-model="queryForm.analysisType" placeholder="請選擇分析類型">
            <el-option label="比率分析" value="RATIO" />
            <el-option label="趨勢分析" value="TREND" />
            <el-option label="比較分析" value="COMPARISON" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="日期範圍">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleQuery" :icon="Search">查詢</el-button>
          <el-button @click="handleReset" :icon="Refresh">重置</el-button>
        </el-form-item>
      </el-form>
      
      <!-- 分析結果圖表 -->
      <div class="analysis-chart">
        <el-card>
          <template #header>分析圖表</template>
          <div ref="chartContainer" style="height: 400px;"></div>
        </el-card>
      </div>
      
      <!-- 分析結果表格 -->
      <el-table :data="analysisData" border stripe>
        <el-table-column prop="period" label="期間" width="120" />
        <el-table-column prop="accountSubjectName" label="會計科目" width="200" />
        <el-table-column prop="ratioValue" label="比率值" width="120" align="right" />
        <el-table-column prop="trendValue" label="趨勢值" width="120" align="right" />
        <el-table-column prop="comparisonValue" label="比較值" width="120" align="right" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Search, Refresh, Download } from '@element-plus/icons-vue';
import { getFinancialAnalysis, exportFinancialAnalysis } from '@/api/financial-report.api';
import * as echarts from 'echarts';

const queryForm = reactive({
  analysisType: 'RATIO',
  startDate: '',
  endDate: '',
  accountSubjectIds: [],
  analysisPeriod: 'MONTHLY'
});

const dateRange = ref([]);
const analysisData = ref([]);
const chartContainer = ref(null);

const handleQuery = async () => {
  try {
    const response = await getFinancialAnalysis(queryForm);
    if (response.data.success) {
      analysisData.value = response.data.data.items || [];
      renderChart(response.data.data);
    }
  } catch (error) {
    ElMessage.error('查詢失敗');
  }
};

const handleExport = async () => {
  try {
    await exportFinancialAnalysis(queryForm);
    ElMessage.success('匯出成功');
  } catch (error) {
    ElMessage.error('匯出失敗');
  }
};

const renderChart = (data: any) => {
  // 使用 echarts 渲染圖表
  if (chartContainer.value) {
    const chart = echarts.init(chartContainer.value);
    const option = {
      // 圖表配置
    };
    chart.setOption(option);
  }
};

onMounted(() => {
  handleQuery();
});
</script>
```

---

## 五、開發時程

**總計**: 10天
- 資料庫設計: 1天（視圖設計）
- 後端API開發: 3天
- 前端UI開發: 4天（包含圖表整合）
- 測試與修正: 2天

---

## 六、注意事項

### 6.1 業務邏輯
- 本功能為報表分析功能，僅提供查詢和匯出功能
- 分析資料必須準確
- 分析圖表必須清晰易懂
- 支援多種分析類型和期間

### 6.2 效能
- 大量資料分析需考慮效能
- 圖表渲染需優化
- 匯出功能需支援大量資料

### 6.3 資料驗證
- 日期範圍必須有效
- 會計科目必須存在
- 分析類型必須有效

---

## 七、測試案例

### 7.1 單元測試
- [ ] 查詢財務分析成功（比率分析）
- [ ] 查詢財務分析成功（趨勢分析）
- [ ] 查詢財務分析成功（比較分析）
- [ ] 匯出財務分析資料成功（Excel）
- [ ] 匯出財務分析資料成功（PDF）
- [ ] 日期範圍驗證成功
- [ ] 會計科目驗證成功

### 7.2 整合測試
- [ ] 完整查詢流程測試
- [ ] 圖表渲染測試
- [ ] 匯出功能測試
- [ ] 錯誤處理測試

---

## 八、參考資料

### 8.1 舊程式碼
- `WEB/IMS_CORE/ASP/SYSN000/SYSN540_FQ.ASP`

---

**文件版本**: v1.0  
**建立日期**: 2024-01-01  
**最後更新**: 2024-01-01

