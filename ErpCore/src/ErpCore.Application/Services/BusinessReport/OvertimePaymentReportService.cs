using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 加班發放報表服務實作 (SYSL510)
/// </summary>
public class OvertimePaymentReportService : BaseService, IOvertimePaymentReportService
{
    private readonly IOvertimePaymentReportRepository _repository;

    public OvertimePaymentReportService(
        IOvertimePaymentReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<OvertimePaymentReportResultDto> GetReportAsync(OvertimePaymentReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new OvertimePaymentReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                DepartmentId = query.DepartmentId,
                EmployeeId = query.EmployeeId,
                Status = query.Status
            };

            var result = await _repository.QueryReportAsync(repositoryQuery);
            var summary = await _repository.GetSummaryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new OvertimePaymentReportDto
            {
                Id = x.Id,
                PaymentNo = x.PaymentNo,
                PaymentDate = x.PaymentDate,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                OvertimeHours = x.OvertimeHours,
                OvertimeAmount = x.OvertimeAmount,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status,
                ApprovedBy = x.ApprovedBy,
                ApprovedByName = x.ApprovedByName,
                ApprovedAt = x.ApprovedAt,
                Notes = x.Notes
            }).ToList();

            return new OvertimePaymentReportResultDto
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize),
                Summary = new OvertimePaymentReportSummaryDto
                {
                    TotalCount = summary.TotalCount,
                    TotalOvertimeHours = summary.TotalOvertimeHours,
                    TotalOvertimeAmount = summary.TotalOvertimeAmount
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢加班發放報表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> PrintReportAsync(BusinessReportPrintRequestDto request)
    {
        try
        {
            // 產生報表檔案（實際實作需根據業務需求）
            var reportName = "加班發放報表";
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
            _logger.LogError("列印加班發放報表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportPrintResultDto> ExportReportAsync(BusinessReportExportRequestDto request)
    {
        try
        {
            // 產生匯出檔案（實際實作需根據業務需求）
            var reportName = "加班發放報表";
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
            _logger.LogError("匯出加班發放報表失敗", ex);
            throw;
        }
    }
}

