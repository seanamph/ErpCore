using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 銷退卡報表服務實作 (SYSL310)
/// </summary>
public class ReturnCardReportService : BaseService, IReturnCardReportService
{
    private readonly IReturnCardReportRepository _repository;

    public ReturnCardReportService(
        IReturnCardReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<ReturnCardReportResultDto> GetReportAsync(ReturnCardReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new ReturnCardReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SiteId = query.SiteId,
                OrgId = query.OrgId,
                CardYear = query.CardYear,
                CardMonth = query.CardMonth,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                EmployeeId = query.EmployeeId,
                ReportType = query.ReportType
            };

            var result = await _repository.QueryReportAsync(repositoryQuery);
            var summary = await _repository.GetSummaryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ReturnCardReportDto
            {
                SiteId = x.SiteId,
                SiteName = x.SiteName,
                OrgId = x.OrgId,
                OrgName = x.OrgName,
                CardYear = x.CardYear,
                CardMonth = x.CardMonth,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                ReturnDate = x.ReturnDate,
                ReturnReason = x.ReturnReason,
                Amount = x.Amount,
                Status = x.Status,
                ReportName = $"銷退卡報表-{query.ReportType ?? "detail"}"
            }).ToList();

            return new ReturnCardReportResultDto
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize),
                Summary = new ReturnCardReportSummaryDto
                {
                    TotalCount = summary.TotalCount,
                    TotalAmount = summary.TotalAmount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷退卡報表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> PrintReportAsync(BusinessReportPrintRequestDto request)
    {
        try
        {
            // 產生報表檔案（實際實作需根據業務需求）
            var reportName = "銷退卡報表";
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
            _logger.LogError("列印銷退卡報表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> ExportReportAsync(BusinessReportExportRequestDto request)
    {
        try
        {
            // 產生匯出檔案（實際實作需根據業務需求）
            var reportName = "銷退卡報表";
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
            _logger.LogError("匯出銷退卡報表失敗", ex);
            throw;
        }
    }
}

