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

    public AnalysisReportService(
        IAnalysisReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
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
            // TODO: 實作 Excel/PDF 匯出功能
            // 目前先返回空陣列，後續可整合 EPPlus 或 NPOI 等套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
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
            // TODO: 實作 PDF 列印功能
            // 目前先返回空陣列，後續可整合 iTextSharp 或其他 PDF 套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
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
                ApplyType = query.ApplyType
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
            // TODO: 實作 Excel/PDF 匯出功能
            // 目前先返回空陣列，後續可整合 EPPlus 或 NPOI 等套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
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
            // TODO: 實作 PDF 列印功能
            // 目前先返回空陣列，後續可整合 iTextSharp 或其他 PDF 套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
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
            // TODO: 實作 Excel/PDF 匯出功能
            // 目前先返回空陣列，後續可整合 EPPlus 或 NPOI 等套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
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
            // TODO: 實作 PDF 列印功能
            // 目前先返回空陣列，後續可整合 iTextSharp 或其他 PDF 套件
            await Task.CompletedTask;
            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError("列印耗材出庫明細表失敗", ex);
            throw;
        }
    }
}
