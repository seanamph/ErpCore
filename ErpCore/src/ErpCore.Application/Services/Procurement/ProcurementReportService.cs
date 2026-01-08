using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購報表服務實作 (SYSP410-SYSP4I0)
/// </summary>
public class ProcurementReportService : BaseService, IProcurementReportService
{
    private readonly ISupplierRepository _supplierRepository;

    public ProcurementReportService(
        ISupplierRepository supplierRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<ProcurementReportDto>> QueryReportAsync(ProcurementReportQueryDto query)
    {
        try
        {
            // TODO: 實作採購報表查詢邏輯
            // 目前先返回空結果，實際應從採購單相關表查詢

            _logger.LogInfo($"查詢採購報表: {query.ReportType}");

            return new PagedResult<ProcurementReportDto>
            {
                Items = new List<ProcurementReportDto>(),
                TotalCount = 0,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購報表失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(ExportProcurementReportDto dto)
    {
        try
        {
            // TODO: 實作報表匯出邏輯（Excel、PDF）
            // 目前先返回空陣列，實際應使用 ExportHelper 匯出

            _logger.LogInfo($"匯出採購報表: {dto.ExportType}");

            return Array.Empty<byte>();
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出採購報表失敗", ex);
            throw;
        }
    }
}

