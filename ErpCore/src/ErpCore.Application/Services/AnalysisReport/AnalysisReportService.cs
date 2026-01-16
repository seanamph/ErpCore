using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Infrastructure.Repositories.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 分析報表服務實作 (SYSA1011)
/// </summary>
public class AnalysisReportService : BaseService, IAnalysisReportService
{
    private readonly IAnalysisReportRepository _repository;
    private readonly ExportHelper _exportHelper;

    public AnalysisReportService(
        IAnalysisReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = new ExportHelper(logger);
    }

    public async Task<PagedResult<SYSA1011ReportDto>> GetSYSA1011ReportAsync(SYSA1011QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1011Query
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1011ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1011ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                SeqNo = item.SeqNo,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                Qty = item.Qty,
                SafeQty = item.SafeQty,
                SelectType = item.SelectType
            }).ToList();

            return new PagedResult<SYSA1011ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1011ReportAsync(SYSA1011QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1011QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1011ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "狀態", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1011)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1011)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1011ReportAsync(SYSA1011QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1011QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1011ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "狀態", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1011)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<Dictionary<string, object>>> GetAnalysisReportAsync(string reportId, AnalysisReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new AnalysisReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SiteId = query.SiteId,
                YearMonth = query.YearMonth,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                FilterType = query.FilterType,
                OrgId = query.OrgId,
                Vendor = query.Vendor,
                Use = query.Use,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                StartMonth = query.StartMonth,
                EndMonth = query.EndMonth,
                DateType = query.DateType,
                MaintainEmp = query.MaintainEmp,
                BelongOrg = query.BelongOrg,
                ApplyType = query.ApplyType,
                OtherCondition1 = query.OtherCondition1,
                OtherCondition2 = query.OtherCondition2
            };

            var result = await _repository.GetAnalysisReportAsync(reportId, repositoryQuery);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢進銷存分析報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportAnalysisReportAsync(string reportId, AnalysisReportQueryDto query, string format)
    {
        try
        {
            // TODO: 實作 Excel/PDF 匯出功能
            // 目前先返回空陣列，後續可整合 EPPlus 或 NPOI 等套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出進銷存分析報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintAnalysisReportAsync(string reportId, AnalysisReportQueryDto query)
    {
        try
        {
            // TODO: 實作 PDF 列印功能
            // 目前先返回空陣列，後續可整合 iTextSharp 或其他 PDF 套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印進銷存分析報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<GoodsCategoryDto>> GetGoodsCategoriesAsync(string categoryType, string? parentId = null)
    {
        try
        {
            var categories = await _repository.GetGoodsCategoriesAsync(categoryType, parentId);
            return categories.Select(c => new GoodsCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryType = c.CategoryType,
                ParentId = c.ParentId,
                SeqNo = c.SeqNo,
                Status = c.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分類列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1012ReportDto>> GetSYSA1012ReportAsync(SYSA1012QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1012Query
            {
                SiteId = query.SiteId,
                GoodsId = query.GoodsId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                ReportMonth = query.ReportMonth,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1012ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1012ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                ReportMonth = item.ReportMonth,
                BeginQty = item.BeginQty,
                BeginAmt = item.BeginAmt,
                InQty = item.InQty,
                InAmt = item.InAmt,
                OutQty = item.OutQty,
                OutAmt = item.OutAmt,
                EndQty = item.EndQty,
                EndAmt = item.EndAmt
            }).ToList();

            return new PagedResult<SYSA1012ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢進銷存月報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1012ReportAsync(SYSA1012QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1012QueryDto
            {
                SiteId = query.SiteId,
                GoodsId = query.GoodsId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                ReportMonth = query.ReportMonth,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1012ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportMonth", DisplayName = "報表月份", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BeginQty", DisplayName = "期初數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "BeginAmt", DisplayName = "期初金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InQty", DisplayName = "進貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InAmt", DisplayName = "進貨金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutQty", DisplayName = "銷貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutAmt", DisplayName = "銷貨金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "EndQty", DisplayName = "期末數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "EndAmt", DisplayName = "期末金額", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "進銷存月報表 (SYSA1012)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "進銷存月報表", "進銷存月報表 (SYSA1012)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出進銷存月報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1012ReportAsync(SYSA1012QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1012QueryDto
            {
                SiteId = query.SiteId,
                GoodsId = query.GoodsId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                ReportMonth = query.ReportMonth,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1012ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportMonth", DisplayName = "報表月份", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BeginQty", DisplayName = "期初數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "BeginAmt", DisplayName = "期初金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InQty", DisplayName = "進貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "InAmt", DisplayName = "進貨金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutQty", DisplayName = "銷貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutAmt", DisplayName = "銷貨金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "EndQty", DisplayName = "期末數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "EndAmt", DisplayName = "期末金額", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "進銷存月報表 (SYSA1012)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印進銷存月報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1013ReportDto>> GetSYSA1013ReportAsync(SYSA1013QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1013Query
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                SupplierId = query.SupplierId,
                Use = query.Use,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1013ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1013ReportDto
            {
                TxnNo = item.TxnNo,
                TxnDate = item.TxnDate,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                Amt = item.Amt,
                ApplyQty = item.ApplyQty,
                Qty = item.Qty,
                NAmt = item.NAmt,
                Use = item.Use,
                Vendor = item.Vendor,
                StocksType = item.StocksType,
                OrgId = item.OrgId,
                OrgAllocation = item.OrgAllocation
            }).ToList();

            return new PagedResult<SYSA1013ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢耗材出庫明細表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1013ReportAsync(SYSA1013QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1013QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                SupplierId = query.SupplierId,
                Use = query.Use,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1013ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "TxnNo", DisplayName = "出庫單號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TxnDate", DisplayName = "出庫日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Amt", DisplayName = "單價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ApplyQty", DisplayName = "申請數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "NAmt", DisplayName = "未稅金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Use", DisplayName = "用途", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Vendor", DisplayName = "廠商", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StocksType", DisplayName = "庫存類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgAllocation", DisplayName = "單位分攤", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "耗材出庫明細表 (SYSA1013)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "耗材出庫明細表", "耗材出庫明細表 (SYSA1013)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出耗材出庫明細表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1013ReportAsync(SYSA1013QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1013QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                SupplierId = query.SupplierId,
                Use = query.Use,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1013ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "TxnNo", DisplayName = "出庫單號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TxnDate", DisplayName = "出庫日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Amt", DisplayName = "單價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ApplyQty", DisplayName = "申請數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "NAmt", DisplayName = "未稅金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "Use", DisplayName = "用途", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Vendor", DisplayName = "廠商", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StocksType", DisplayName = "庫存類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgAllocation", DisplayName = "單位分攤", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "耗材出庫明細表 (SYSA1013)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印耗材出庫明細表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1014ReportDto>> GetSYSA1014ReportAsync(SYSA1014QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1014Query
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1014ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1014ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                SelectDate = item.SelectDate,
                SelectType = item.SelectType,
                SeqNo = item.SeqNo,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                PurchaseQty = item.PurchaseQty,
                SalesQty = item.SalesQty,
                StockQty = item.StockQty,
                BeginDate = item.BeginDate,
                EndDate = item.EndDate
            }).ToList();

            return new PagedResult<SYSA1014ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1014ReportAsync(SYSA1014QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1014QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1014ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PurchaseQty", DisplayName = "進貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesQty", DisplayName = "銷貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "StockQty", DisplayName = "庫存數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectDate", DisplayName = "日期範圍", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1014)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1014)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1014ReportAsync(SYSA1014QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1014QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                OrgId = query.OrgId,
                GoodsId = query.GoodsId,
                BeginDate = query.BeginDate,
                EndDate = query.EndDate,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1014ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PurchaseQty", DisplayName = "進貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SalesQty", DisplayName = "銷貨數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "StockQty", DisplayName = "庫存數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectDate", DisplayName = "日期範圍", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1014)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1015ReportDto>> GetSYSA1015ReportAsync(SYSA1015QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1015Query
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1015ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1015ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                SeqNo = item.SeqNo,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                Qty = item.Qty,
                SafeQty = item.SafeQty,
                SelectType = item.SelectType,
                YearMonth = item.YearMonth
            }).ToList();

            return new PagedResult<SYSA1015ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1015ReportAsync(SYSA1015QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1015QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1015ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1015)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1015)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1015ReportAsync(SYSA1015QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1015QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1015ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1015)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1016ReportDto>> GetSYSA1016ReportAsync(SYSA1016QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1016Query
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1016ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1016ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                SeqNo = item.SeqNo,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                Qty = item.Qty,
                SafeQty = item.SafeQty,
                SelectType = item.SelectType,
                YearMonth = item.YearMonth
            }).ToList();

            return new PagedResult<SYSA1016ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1016ReportAsync(SYSA1016QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1016QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1016ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1016)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1016)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1016ReportAsync(SYSA1016QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1016QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1016ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1016)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1017ReportDto>> GetSYSA1017ReportAsync(SYSA1017QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1017Query
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1017ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1017ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                SeqNo = item.SeqNo,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                PackUnit = item.PackUnit,
                Unit = item.Unit,
                Qty = item.Qty,
                SafeQty = item.SafeQty,
                SelectType = item.SelectType,
                YearMonth = item.YearMonth,
                OrgId = item.OrgId
            }).ToList();

            return new PagedResult<SYSA1017ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1017ReportAsync(SYSA1017QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1017QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1017ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1017)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1017)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1017ReportAsync(SYSA1017QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1017QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1017ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PackUnit", DisplayName = "包裝單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Unit", DisplayName = "單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SelectType", DisplayName = "篩選類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1017)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1018ReportDto>> GetSYSA1018ReportAsync(SYSA1018QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1018Query
            {
                OrgId = query.OrgId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1018ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1018ReportDto
            {
                OrgId = item.OrgId,
                OrgName = item.OrgName,
                ReportName = item.ReportName,
                YearMonth = item.YearMonth,
                MaintenanceType = item.MaintenanceType,
                MaintenanceStatus = item.MaintenanceStatus,
                ItemCount = item.ItemCount,
                TotalCount = item.TotalCount
            }).ToList();

            return new PagedResult<SYSA1018ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修件數統計報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1018ReportAsync(SYSA1018QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1018QueryDto
            {
                OrgId = query.OrgId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1018ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintenanceType", DisplayName = "維修類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintenanceStatus", DisplayName = "維修狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ItemCount", DisplayName = "維修件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalCount", DisplayName = "總件數", DataType = ExportDataType.Integer }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "工務維修件數統計表 (SYSA1018)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "工務維修件數統計表", "工務維修件數統計表 (SYSA1018)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出工務維修件數統計報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1018ReportAsync(SYSA1018QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1018QueryDto
            {
                OrgId = query.OrgId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1018ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintenanceType", DisplayName = "維修類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintenanceStatus", DisplayName = "維修狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ItemCount", DisplayName = "維修件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalCount", DisplayName = "總件數", DataType = ExportDataType.Integer }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "工務維修件數統計表 (SYSA1018)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印工務維修件數統計報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1019ReportDto>> GetSYSA1019ReportAsync(SYSA1019QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1019Query
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1019ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1019ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                OrgId = item.OrgId,
                OrgName = item.OrgName,
                ReportName = item.ReportName,
                YearMonth = item.YearMonth,
                FilterType = item.FilterType,
                SeqNo = item.SeqNo,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName
            }).ToList();

            return new PagedResult<SYSA1019ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1019ReportAsync(SYSA1019QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1019QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1019ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "請修單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FilterType", DisplayName = "篩選類型", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1019)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1019)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1019ReportAsync(SYSA1019QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1019QueryDto
            {
                OrgId = query.OrgId,
                SiteId = query.SiteId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1019ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "請修單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FilterType", DisplayName = "篩選類型", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1019)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1020ReportDto>> GetSYSA1020ReportAsync(SYSA1020QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1020Query
            {
                SiteId = query.SiteId,
                PlanId = query.PlanId,
                ShowType = query.ShowType,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1020ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1020ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                PlanId = item.PlanId,
                PlanName = item.PlanName,
                ShowType = item.ShowType,
                FilterType = item.FilterType,
                SeqNo = item.SeqNo,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName
            }).ToList();

            return new PagedResult<SYSA1020ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1020ReportAsync(SYSA1020QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1020QueryDto
            {
                SiteId = query.SiteId,
                PlanId = query.PlanId,
                ShowType = query.ShowType,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1020ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PlanName", DisplayName = "計劃名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShowType", DisplayName = "顯示類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FilterType", DisplayName = "篩選類型", DataType = ExportDataType.String }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1020)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商品分析報表", "商品分析報表 (SYSA1020)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1020ReportAsync(SYSA1020QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1020QueryDto
            {
                SiteId = query.SiteId,
                PlanId = query.PlanId,
                ShowType = query.ShowType,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1020ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "序號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PlanName", DisplayName = "計劃名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShowType", DisplayName = "顯示類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FilterType", DisplayName = "篩選類型", DataType = ExportDataType.String }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "商品分析報表 (SYSA1020)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印商品分析報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1021ReportDto>> GetSYSA1021ReportAsync(SYSA1021QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1021Query
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1021ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1021ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                YearMonth = item.YearMonth,
                BId = item.BId,
                MId = item.MId,
                SId = item.SId,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                Qty = item.Qty,
                CostAmount = item.CostAmount,
                AvgCost = item.AvgCost
            }).ToList();

            return new PagedResult<SYSA1021ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢月成本報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1021ReportAsync(SYSA1021QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1021QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1021ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CostAmount", DisplayName = "成本金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AvgCost", DisplayName = "平均成本", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "月成本報表 (SYSA1021)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "月成本報表", "月成本報表 (SYSA1021)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出月成本報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1021ReportAsync(SYSA1021QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1021QueryDto
            {
                SiteId = query.SiteId,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                GoodsId = query.GoodsId,
                YearMonth = query.YearMonth,
                FilterType = query.FilterType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1021ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "YearMonth", DisplayName = "年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BId", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MId", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SId", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "品項編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "品項名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Qty", DisplayName = "數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CostAmount", DisplayName = "成本金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AvgCost", DisplayName = "平均成本", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "月成本報表 (SYSA1021)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印月成本報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1022ReportDto>> GetSYSA1022ReportAsync(SYSA1022QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1022Query
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1022ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1022ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                BelongStatus = item.BelongStatus,
                ApplyDateB = item.ApplyDateB,
                ApplyDateE = item.ApplyDateE,
                BelongOrg = item.BelongOrg,
                MaintainEmp = item.MaintainEmp,
                ApplyType = item.ApplyType,
                RequestCount = item.RequestCount,
                TotalAmount = item.TotalAmount
            }).ToList();

            return new PagedResult<SYSA1022ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1022ReportAsync(SYSA1022QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1022QueryDto
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1022ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表 (SYSA1022)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "工務維修統計報表", "工務維修統計報表 (SYSA1022)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出工務維修統計報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1022ReportAsync(SYSA1022QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1022QueryDto
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1022ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表 (SYSA1022)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印工務維修統計報表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1023ReportDto>> GetSYSA1023ReportAsync(SYSA1023QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1023Query
            {
                ReportType = query.ReportType,
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1023ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1023ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                ReportType = item.ReportType,
                BelongStatus = item.BelongStatus,
                ApplyDateB = item.ApplyDateB,
                ApplyDateE = item.ApplyDateE,
                BelongOrg = item.BelongOrg,
                MaintainEmp = item.MaintainEmp,
                ApplyType = item.ApplyType,
                RequestCount = item.RequestCount,
                TotalAmount = item.TotalAmount
            }).ToList();

            return new PagedResult<SYSA1023ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表(報表類型)失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1023ReportAsync(SYSA1023QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1023QueryDto
            {
                ReportType = query.ReportType,
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1023ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportType", DisplayName = "報表類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表(報表類型) (SYSA1023)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "工務維修統計報表(報表類型)", "工務維修統計報表(報表類型) (SYSA1023)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出工務維修統計報表(報表類型)失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1023ReportAsync(SYSA1023QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1023QueryDto
            {
                ReportType = query.ReportType,
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1023ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ReportType", DisplayName = "報表類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表(報表類型) (SYSA1023)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印工務維修統計報表(報表類型)失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSA1024ReportDto>> GetSYSA1024ReportAsync(SYSA1024QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSA1024Query
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                OtherCondition1 = query.OtherCondition1,
                OtherCondition2 = query.OtherCondition2,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSA1024ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSA1024ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                ReportName = item.ReportName,
                BelongStatus = item.BelongStatus,
                ApplyDateB = item.ApplyDateB,
                ApplyDateE = item.ApplyDateE,
                BelongOrg = item.BelongOrg,
                MaintainEmp = item.MaintainEmp,
                ApplyType = item.ApplyType,
                OtherCondition1 = item.OtherCondition1,
                OtherCondition2 = item.OtherCondition2,
                RequestCount = item.RequestCount,
                TotalAmount = item.TotalAmount
            }).ToList();

            return new PagedResult<SYSA1024ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢工務維修統計報表(其他)失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSA1024ReportAsync(SYSA1024QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1024QueryDto
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                OtherCondition1 = query.OtherCondition1,
                OtherCondition2 = query.OtherCondition2,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1024ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OtherCondition1", DisplayName = "其他條件1", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OtherCondition2", DisplayName = "其他條件2", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表(其他) (SYSA1024)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "工務維修統計報表(其他)", "工務維修統計報表(其他) (SYSA1024)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出工務維修統計報表(其他)失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSA1024ReportAsync(SYSA1024QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSA1024QueryDto
            {
                SiteId = query.SiteId,
                BelongStatus = query.BelongStatus,
                ApplyDateB = query.ApplyDateB,
                ApplyDateE = query.ApplyDateE,
                BelongOrg = query.BelongOrg,
                MaintainEmp = query.MaintainEmp,
                ApplyType = query.ApplyType,
                OtherCondition1 = query.OtherCondition1,
                OtherCondition2 = query.OtherCondition2,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSA1024ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongStatus", DisplayName = "費用負擔", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateB", DisplayName = "日統計表起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyDateE", DisplayName = "日統計表迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BelongOrg", DisplayName = "費用歸屬單位", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MaintainEmp", DisplayName = "維保人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ApplyType", DisplayName = "請修類別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OtherCondition1", DisplayName = "其他條件1", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OtherCondition2", DisplayName = "其他條件2", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RequestCount", DisplayName = "申請件數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "工務維修統計報表(其他) (SYSA1024)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印工務維修統計報表(其他)失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SYSWC10ReportDto>> GetSYSWC10ReportAsync(SYSWC10QueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSWC10Query
            {
                GoodsIdFrom = query.GoodsIdFrom,
                GoodsIdTo = query.GoodsIdTo,
                GoodsName = query.GoodsName,
                SiteIds = query.SiteIds,
                WarehouseIds = query.WarehouseIds,
                CategoryIds = query.CategoryIds,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                MinQty = query.MinQty,
                MaxQty = query.MaxQty,
                Status = query.Status,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSYSWC10ReportAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SYSWC10ReportDto
            {
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                GoodsId = item.GoodsId,
                GoodsName = item.GoodsName,
                BigCategoryId = item.BigCategoryId,
                BigCategoryName = item.BigCategoryName,
                MidCategoryId = item.MidCategoryId,
                MidCategoryName = item.MidCategoryName,
                SmallCategoryId = item.SmallCategoryId,
                SmallCategoryName = item.SmallCategoryName,
                WarehouseId = item.WarehouseId,
                WarehouseName = item.WarehouseName,
                InQty = item.InQty,
                OutQty = item.OutQty,
                CurrentQty = item.CurrentQty,
                CurrentAmt = item.CurrentAmt,
                LastStockDate = item.LastStockDate,
                SafeQty = item.SafeQty,
                IsLowStock = item.IsLowStock,
                IsOverStock = item.IsOverStock
            }).ToList();

            return new PagedResult<SYSWC10ReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢庫存分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSYSWC10ReportAsync(SYSWC10QueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSWC10QueryDto
            {
                GoodsIdFrom = query.GoodsIdFrom,
                GoodsIdTo = query.GoodsIdTo,
                GoodsName = query.GoodsName,
                SiteIds = query.SiteIds,
                WarehouseIds = query.WarehouseIds,
                CategoryIds = query.CategoryIds,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                MinQty = query.MinQty,
                MaxQty = query.MaxQty,
                Status = query.Status,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSWC10ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BigCategoryName", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MidCategoryName", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SmallCategoryName", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "WarehouseName", DisplayName = "庫別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InQty", DisplayName = "入庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutQty", DisplayName = "出庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CurrentQty", DisplayName = "當前庫存數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CurrentAmt", DisplayName = "當前庫存金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "IsLowStock", DisplayName = "低庫存", DataType = ExportDataType.Boolean },
                new ExportColumn { PropertyName = "IsOverStock", DisplayName = "過量庫存", DataType = ExportDataType.Boolean },
                new ExportColumn { PropertyName = "LastStockDate", DisplayName = "最後庫存異動日期", DataType = ExportDataType.Date }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "庫存分析報表 (SYSWC10)");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "庫存分析報表", "庫存分析報表 (SYSWC10)");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出庫存分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSYSWC10ReportAsync(SYSWC10QueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SYSWC10QueryDto
            {
                GoodsIdFrom = query.GoodsIdFrom,
                GoodsIdTo = query.GoodsIdTo,
                GoodsName = query.GoodsName,
                SiteIds = query.SiteIds,
                WarehouseIds = query.WarehouseIds,
                CategoryIds = query.CategoryIds,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                MinQty = query.MinQty,
                MaxQty = query.MaxQty,
                Status = query.Status,
                BId = query.BId,
                MId = query.MId,
                SId = query.SId,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSYSWC10ReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "SiteId", DisplayName = "店別代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "GoodsName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BigCategoryName", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MidCategoryName", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SmallCategoryName", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "WarehouseName", DisplayName = "庫別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InQty", DisplayName = "入庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OutQty", DisplayName = "出庫數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CurrentQty", DisplayName = "當前庫存數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "CurrentAmt", DisplayName = "當前庫存金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "SafeQty", DisplayName = "安全庫存量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "IsLowStock", DisplayName = "低庫存", DataType = ExportDataType.Boolean },
                new ExportColumn { PropertyName = "IsOverStock", DisplayName = "過量庫存", DataType = ExportDataType.Boolean },
                new ExportColumn { PropertyName = "LastStockDate", DisplayName = "最後庫存異動日期", DataType = ExportDataType.Date }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "庫存分析報表 (SYSWC10)");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印庫存分析報表失敗", ex);
            throw;
        }
    }

    public async Task<SalesAnalysisReportResult> GetSalesAnalysisReportAsync(SalesAnalysisQueryDto query)
    {
        try
        {
            var repositoryQuery = new SalesAnalysisQuery
            {
                SiteId = query.SiteId,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                BigClassId = query.BigClassId,
                MidClassId = query.MidClassId,
                SmallClassId = query.SmallClassId,
                ProductId = query.ProductId,
                VendorId = query.VendorId,
                SalesPersonId = query.SalesPersonId,
                CustomerId = query.CustomerId,
                ReportType = query.ReportType,
                GroupBy = query.GroupBy,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var result = await _repository.GetSalesAnalysisReportAsync(repositoryQuery);
            var summary = await _repository.GetSalesAnalysisSummaryAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new SalesAnalysisReportDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                BigClassId = item.BigClassId,
                BigClassName = item.BigClassName,
                MidClassId = item.MidClassId,
                MidClassName = item.MidClassName,
                SmallClassId = item.SmallClassId,
                SmallClassName = item.SmallClassName,
                VendorId = item.VendorId,
                VendorName = item.VendorName,
                SiteId = item.SiteId,
                SiteName = item.SiteName,
                SalesPersonId = item.SalesPersonId,
                SalesPersonName = item.SalesPersonName,
                TotalQuantity = item.TotalQuantity,
                TotalAmount = item.TotalAmount,
                TotalCost = item.TotalCost,
                TotalProfit = item.TotalProfit,
                ProfitRate = item.ProfitRate,
                OrderCount = item.OrderCount,
                AvgUnitPrice = item.AvgUnitPrice,
                AvgQuantity = item.AvgQuantity
            }).ToList();

            return new SalesAnalysisReportResult
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                Summary = new SalesAnalysisSummaryDto
                {
                    TotalQuantity = summary.TotalQuantity,
                    TotalAmount = summary.TotalAmount,
                    TotalCost = summary.TotalCost,
                    TotalProfit = summary.TotalProfit,
                    AvgProfitRate = summary.AvgProfitRate,
                    TotalOrderCount = summary.TotalOrderCount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportSalesAnalysisReportAsync(SalesAnalysisQueryDto query, string format)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SalesAnalysisQueryDto
            {
                SiteId = query.SiteId,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                BigClassId = query.BigClassId,
                MidClassId = query.MidClassId,
                SmallClassId = query.SmallClassId,
                ProductId = query.ProductId,
                VendorId = query.VendorId,
                SalesPersonId = query.SalesPersonId,
                CustomerId = query.CustomerId,
                ReportType = query.ReportType,
                GroupBy = query.GroupBy,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSalesAnalysisReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ProductId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProductName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BigClassName", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MidClassName", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SmallClassName", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorName", DisplayName = "廠商", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SalesPersonName", DisplayName = "銷售人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TotalQuantity", DisplayName = "總數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalCost", DisplayName = "總成本", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalProfit", DisplayName = "總毛利", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ProfitRate", DisplayName = "毛利率(%)", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderCount", DisplayName = "筆數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "AvgUnitPrice", DisplayName = "平均單價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AvgQuantity", DisplayName = "平均數量", DataType = ExportDataType.Decimal }
            };

            // 根据格式导出
            if (format.ToLower() == "pdf")
            {
                return _exportHelper.ExportToPdf(result.Items, columns, "銷售分析報表");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "銷售分析報表", "銷售分析報表");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出銷售分析報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> PrintSalesAnalysisReportAsync(SalesAnalysisQueryDto query)
    {
        try
        {
            // 获取所有数据（不分页）
            var allDataQuery = new SalesAnalysisQueryDto
            {
                SiteId = query.SiteId,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                BigClassId = query.BigClassId,
                MidClassId = query.MidClassId,
                SmallClassId = query.SmallClassId,
                ProductId = query.ProductId,
                VendorId = query.VendorId,
                SalesPersonId = query.SalesPersonId,
                CustomerId = query.CustomerId,
                ReportType = query.ReportType,
                GroupBy = query.GroupBy,
                PageIndex = 1,
                PageSize = int.MaxValue // 获取所有数据
            };

            var result = await GetSalesAnalysisReportAsync(allDataQuery);

            // 定义导出列
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ProductId", DisplayName = "商品代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ProductName", DisplayName = "商品名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "BigClassName", DisplayName = "大分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "MidClassName", DisplayName = "中分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SmallClassName", DisplayName = "小分類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorName", DisplayName = "廠商", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SiteName", DisplayName = "店別", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SalesPersonName", DisplayName = "銷售人員", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TotalQuantity", DisplayName = "總數量", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalAmount", DisplayName = "總金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalCost", DisplayName = "總成本", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TotalProfit", DisplayName = "總毛利", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "ProfitRate", DisplayName = "毛利率(%)", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "OrderCount", DisplayName = "筆數", DataType = ExportDataType.Integer },
                new ExportColumn { PropertyName = "AvgUnitPrice", DisplayName = "平均單價", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AvgQuantity", DisplayName = "平均數量", DataType = ExportDataType.Decimal }
            };

            // 生成 PDF
            return _exportHelper.ExportToPdf(result.Items, columns, "銷售分析報表");
        }
        catch (Exception ex)
        {
            _logger.LogError("列印銷售分析報表失敗", ex);
            throw;
        }
    }
}
