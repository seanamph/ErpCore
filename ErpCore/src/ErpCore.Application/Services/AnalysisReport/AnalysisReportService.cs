using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 進銷存分析報表服務實作 (SYSA1000)
/// </summary>
public class AnalysisReportService : BaseService, IAnalysisReportService
{
    private readonly IAnalysisReportRepository _repository;
    private readonly ExportHelper _exportHelper;

    public AnalysisReportService(
        IAnalysisReportRepository repository,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
    }

    public async Task<AnalysisReportDto> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢進銷存分析報表: ReportId={reportId}");

            var result = await _repository.GetAnalysisReportAsync(reportId, query);

            var reportName = GetReportName(reportId);

            return new AnalysisReportDto
            {
                ReportId = reportId,
                ReportName = reportName,
                SiteName = query.SiteId, // 可從查詢參數或資料庫取得店別名稱
                Items = result.Items,
                TotalCount = result.TotalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢進銷存分析報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportAnalysisReportAsync(string reportId, ExportAnalysisReportDto dto)
    {
        try
        {
            _logger.LogInfo($"匯出進銷存分析報表: ReportId={reportId}, Format={dto.Format}");

            // 查詢所有資料（不分頁）
            var allDataQuery = new AnalysisReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = dto.QueryParams.SortField,
                SortOrder = dto.QueryParams.SortOrder,
                SiteId = dto.QueryParams.SiteId,
                YearMonth = dto.QueryParams.YearMonth,
                BeginDate = dto.QueryParams.BeginDate,
                EndDate = dto.QueryParams.EndDate,
                BId = dto.QueryParams.BId,
                MId = dto.QueryParams.MId,
                SId = dto.QueryParams.SId,
                GoodsId = dto.QueryParams.GoodsId,
                FilterType = dto.QueryParams.FilterType,
                OrgId = dto.QueryParams.OrgId,
                Vendor = dto.QueryParams.Vendor,
                Use = dto.QueryParams.Use,
                BelongStatus = dto.QueryParams.BelongStatus,
                ApplyDateB = dto.QueryParams.ApplyDateB,
                ApplyDateE = dto.QueryParams.ApplyDateE,
                StartMonth = dto.QueryParams.StartMonth,
                EndMonth = dto.QueryParams.EndMonth,
                DateType = dto.QueryParams.DateType,
                MaintainEmp = dto.QueryParams.MaintainEmp,
                BelongOrg = dto.QueryParams.BelongOrg,
                ApplyType = dto.QueryParams.ApplyType
            };

            var reportData = await GetAnalysisReportAsync(reportId, allDataQuery);

            // 根據報表類型定義不同的匯出欄位
            var columns = GetExportColumnsForReportType(reportId);
            var reportName = GetReportName(reportId);

            // 根據格式匯出
            if (dto.Format.Equals("Excel", StringComparison.OrdinalIgnoreCase))
            {
                return _exportHelper.ExportToExcel(reportData.Items, columns, reportName, $"進銷存分析報表 - {reportName}");
            }
            else if (dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                return _exportHelper.ExportToPdf(reportData.Items, columns, $"進銷存分析報表 - {reportName}");
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {dto.Format}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出進銷存分析報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintAnalysisReportAsync(string reportId, PrintAnalysisReportDto dto)
    {
        try
        {
            _logger.LogInfo($"列印進銷存分析報表: ReportId={reportId}, Format={dto.Format}");

            // 查詢所有資料（不分頁）
            var allDataQuery = new AnalysisReportQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                SortField = dto.QueryParams.SortField,
                SortOrder = dto.QueryParams.SortOrder,
                SiteId = dto.QueryParams.SiteId,
                YearMonth = dto.QueryParams.YearMonth,
                BeginDate = dto.QueryParams.BeginDate,
                EndDate = dto.QueryParams.EndDate,
                BId = dto.QueryParams.BId,
                MId = dto.QueryParams.MId,
                SId = dto.QueryParams.SId,
                GoodsId = dto.QueryParams.GoodsId,
                FilterType = dto.QueryParams.FilterType,
                OrgId = dto.QueryParams.OrgId,
                Vendor = dto.QueryParams.Vendor,
                Use = dto.QueryParams.Use,
                BelongStatus = dto.QueryParams.BelongStatus,
                ApplyDateB = dto.QueryParams.ApplyDateB,
                ApplyDateE = dto.QueryParams.ApplyDateE,
                StartMonth = dto.QueryParams.StartMonth,
                EndMonth = dto.QueryParams.EndMonth,
                DateType = dto.QueryParams.DateType,
                MaintainEmp = dto.QueryParams.MaintainEmp,
                BelongOrg = dto.QueryParams.BelongOrg,
                ApplyType = dto.QueryParams.ApplyType
            };

            var reportData = await GetAnalysisReportAsync(reportId, allDataQuery);

            // 根據報表類型定義不同的列印欄位
            var columns = GetExportColumnsForReportType(reportId);
            var reportName = GetReportName(reportId);

            // 建立列印標題（包含日期時間）
            var printTitle = $"進銷存分析報表 - {reportName}";
            if (dto.IncludeDateTime)
            {
                printTitle += $" ({DateTime.Now:yyyy/MM/dd HH:mm:ss})";
            }

            // 列印格式固定為 PDF，但使用優化的格式
            if (dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                return _exportHelper.ExportToPdf(reportData.Items, columns, printTitle);
            }
            else
            {
                throw new ArgumentException($"列印僅支援 PDF 格式，不支援: {dto.Format}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印進銷存分析報表失敗: ReportId={reportId}", ex);
            throw;
        }
    }

    public async Task<List<Consumable>> GetConsumablesForPrintAsync(ConsumablePrintQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢耗材列表（用於列印）");

            var result = await _repository.GetConsumablesForPrintAsync(query);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得報表名稱
    /// </summary>
    private string GetReportName(string reportId)
    {
        return reportId switch
        {
            "SYSA1011" => "耗材庫存查詢表",
            "SYSA1012" => "耗材入庫明細表",
            "SYSA1013" => "耗材出庫明細表",
            "SYSA1014" => "耗材領用發料退回明細表",
            "SYSA1015" => "固定資產數量彙總表",
            "SYSA1016" => "庫房領料進價成本分攤表",
            "SYSA1017" => "工務修繕扣款報表",
            "SYSA1018" => "工務維修件數統計表",
            "SYSA1019" => "工務維修類別統計表",
            "SYSA1020" => "盤點差異明細表",
            "SYSA1021" => "耗材進銷存月報表",
            "SYSA1022" => "公務費用歸屬統計表",
            _ => "進銷存分析報表"
        };
    }

    /// <summary>
    /// 根據報表類型取得匯出欄位定義
    /// </summary>
    private List<ExportColumn> GetExportColumnsForReportType(string reportId)
    {
        return reportId switch
        {
            "SYSA1011" => GetConsumableStockColumns(), // 耗材庫存查詢表
            "SYSA1012" => GetConsumableInboundColumns(), // 耗材入庫明細表
            "SYSA1013" => GetConsumableOutboundColumns(), // 耗材出庫明細表
            "SYSA1014" => GetConsumableIssueReturnColumns(), // 耗材領用發料退回明細表
            "SYSA1015" => GetFixedAssetSummaryColumns(), // 固定資產數量彙總表
            "SYSA1016" => GetMaterialCostAllocationColumns(), // 庫房領料進價成本分攤表
            "SYSA1017" => GetMaintenanceDeductionColumns(), // 工務修繕扣款報表
            "SYSA1018" => GetMaintenanceCountColumns(), // 工務維修件數統計表
            "SYSA1019" => GetMaintenanceCategoryColumns(), // 工務維修類別統計表
            "SYSA1020" => GetInventoryDifferenceColumns(), // 盤點差異明細表
            "SYSA1021" => GetMonthlyCostColumns(), // 耗材進銷存月報表
            "SYSA1022" => GetPublicExpenseColumns(), // 公務費用歸屬統計表
            _ => GetBaseColumns() // 預設使用基本欄位
        };
    }

    /// <summary>
    /// 基本欄位（所有報表通用）
    /// </summary>
    private List<ExportColumn> GetBaseColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 耗材庫存查詢表欄位 (SYSA1011)
    /// </summary>
    private List<ExportColumn> GetConsumableStockColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "庫存數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 耗材入庫明細表欄位 (SYSA1012)
    /// </summary>
    private List<ExportColumn> GetConsumableInboundColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "入庫數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Price", DisplayName = "單價", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Amount", DisplayName = "金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StocksDate", DisplayName = "入庫日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "SourceId", DisplayName = "來源單號", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 耗材出庫明細表欄位 (SYSA1013)
    /// </summary>
    private List<ExportColumn> GetConsumableOutboundColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "出庫數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Price", DisplayName = "單價", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Amount", DisplayName = "金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StocksDate", DisplayName = "出庫日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "SourceId", DisplayName = "來源單號", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 耗材領用發料退回明細表欄位 (SYSA1014)
    /// </summary>
    private List<ExportColumn> GetConsumableIssueReturnColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StocksDate", DisplayName = "日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "SourceId", DisplayName = "來源單號", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "StatusName", DisplayName = "狀態", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 固定資產數量彙總表欄位 (SYSA1015)
    /// </summary>
    private List<ExportColumn> GetFixedAssetSummaryColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "分類代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "資產代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "資產名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 庫房領料進價成本分攤表欄位 (SYSA1016)
    /// </summary>
    private List<ExportColumn> GetMaterialCostAllocationColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "領料數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Price", DisplayName = "進價", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "Amount", DisplayName = "成本金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StocksDate", DisplayName = "領料日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "SourceId", DisplayName = "來源單號", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 工務修繕扣款報表欄位 (SYSA1017)
    /// </summary>
    private List<ExportColumn> GetMaintenanceDeductionColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "組織單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "工務代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "工務名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "扣款金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "MaintainDate", DisplayName = "維修日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維修人員", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 工務維修件數統計表欄位 (SYSA1018)
    /// </summary>
    private List<ExportColumn> GetMaintenanceCountColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "組織單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "請修類別", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "請修類別名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "維修件數", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "MaintainDate", DisplayName = "維修日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維修人員", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 工務維修類別統計表欄位 (SYSA1019)
    /// </summary>
    private List<ExportColumn> GetMaintenanceCategoryColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "組織單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "請修類別", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "請修類別名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "維修件數", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "MaintainDate", DisplayName = "維修日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維修人員", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 盤點差異明細表欄位 (SYSA1020)
    /// </summary>
    private List<ExportColumn> GetInventoryDifferenceColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "實際數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "SafeQty", DisplayName = "帳面數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "DifferenceQty", DisplayName = "差異數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StocksDate", DisplayName = "盤點日期", DataType = ExportDataType.DateTime }
        };
    }

    /// <summary>
    /// 耗材進銷存月報表欄位 (SYSA1021)
    /// </summary>
    private List<ExportColumn> GetMonthlyCostColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "InboundQty", DisplayName = "入庫數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "OutboundQty", DisplayName = "出庫數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "StockQty", DisplayName = "庫存數量", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String }
        };
    }

    /// <summary>
    /// 公務費用歸屬統計表欄位 (SYSA1022)
    /// </summary>
    private List<ExportColumn> GetPublicExpenseColumns()
    {
        return new List<ExportColumn>
        {
            new ExportColumn { PropertyName = "SiteId", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsId", DisplayName = "請修類別", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "GoodsName", DisplayName = "請修類別名稱", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "Qty", DisplayName = "費用金額", DataType = ExportDataType.Decimal },
            new ExportColumn { PropertyName = "MaintainDate", DisplayName = "維修日期", DataType = ExportDataType.DateTime },
            new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維修人員", DataType = ExportDataType.String },
            new ExportColumn { PropertyName = "SelectType", DisplayName = "選擇類型", DataType = ExportDataType.String }
        };
    }
}

