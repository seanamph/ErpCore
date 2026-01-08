using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 員餐卡報表服務實作 (SYSL210)
/// </summary>
public class EmployeeMealCardReportService : BaseService, IEmployeeMealCardReportService
{
    private readonly IEmployeeMealCardReportRepository _repository;

    public EmployeeMealCardReportService(
        IEmployeeMealCardReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EmployeeMealCardReportDto>> GetReportsAsync(EmployeeMealCardReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new EmployeeMealCardReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ReportType = query.ReportType,
                SiteId = query.SiteId,
                OrgId = query.OrgId,
                YearMonth = query.YearMonth,
                ActionType = query.ActionType,
                TxnNo = query.TxnNo,
                CardSurfaceId = query.CardSurfaceId
            };

            var result = await _repository.QueryReportAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new EmployeeMealCardReportDto
            {
                SiteId = x.SiteId,
                SiteName = x.SiteName,
                OrgId = x.OrgId,
                OrgName = x.OrgName,
                CardSurfaceId = x.CardSurfaceId,
                TxnNo = x.TxnNo,
                Act1 = x.ActionType,
                Act1Name = x.ActionTypeName,
                Amt1 = x.Amt1,
                Amt4 = x.Amt4,
                Amt5 = x.Amt5,
                ReportName = $"員餐卡報表-{query.ReportType}"
            }).ToList();

            return new PagedResult<EmployeeMealCardReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員餐卡報表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> PrintReportAsync(string reportType, BusinessReportPrintRequestDto request)
    {
        try
        {
            // 產生報表檔案（實際實作需根據業務需求）
            var reportName = $"員餐卡報表-{reportType}";
            var fileName = $"{reportName}-{DateTime.Now:yyyyMMddHHmmss}.{request.PrintFormat.ToLower()}";
            var fileUrl = $"/api/v1/files/download/{fileName}";

            return new BusinessReportPrintResultDto
            {
                PrintLogId = 0,
                FileUrl = fileUrl,
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印員餐卡報表失敗: ReportType={reportType}", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> ExportReportAsync(string reportType, BusinessReportExportRequestDto request)
    {
        try
        {
            // 產生匯出檔案（實際實作需根據業務需求）
            var reportName = $"員餐卡報表-{reportType}";
            var fileName = $"{reportName}-{DateTime.Now:yyyyMMddHHmmss}.{request.ExportFormat.ToLower()}";
            var fileUrl = $"/api/v1/files/download/{fileName}";

            return new BusinessReportPrintResultDto
            {
                PrintLogId = 0,
                FileUrl = fileUrl,
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出員餐卡報表失敗: ReportType={reportType}", ex);
            throw;
        }
    }

    public async Task<List<ReportTypeOptionDto>> GetReportTypesAsync()
    {
        try
        {
            return new List<ReportTypeOptionDto>
            {
                new ReportTypeOptionDto { Value = "SYSL21001", Label = "員餐卡報表類型1" },
                new ReportTypeOptionDto { Value = "SYSL21002", Label = "員餐卡報表類型2" },
                new ReportTypeOptionDto { Value = "SYSL21003", Label = "員餐卡報表類型3" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("取得報表類型選項失敗", ex);
            throw;
        }
    }
}

