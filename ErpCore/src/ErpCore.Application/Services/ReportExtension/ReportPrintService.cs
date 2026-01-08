using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportExtension;
using ErpCore.Infrastructure.Repositories.ReportExtension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表列印服務實作 (SYS7B10-SYS7B40)
/// </summary>
public class ReportPrintService : BaseService, IReportPrintService
{
    private readonly IReportPrintLogRepository _repository;
    private readonly ExportHelper _exportHelper;

    public ReportPrintService(
        IReportPrintLogRepository repository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
    }

    public async Task<ReportPrintLogDto> PrintReportAsync(ReportPrintRequestDto request)
    {
        try
        {
            _logger.LogInfo($"列印報表: {request.ReportCode}");

            // 產生報表檔案
            byte[] fileData;
            string fileName;
            string filePath;

            if (request.PrintFormat.ToUpper() == "PDF")
            {
                fileData = await _exportHelper.ExportToPdfAsync(new List<Dictionary<string, object>>(), request.ReportName);
                fileName = $"{request.ReportCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            }
            else if (request.PrintFormat.ToUpper() == "EXCEL")
            {
                fileData = await _exportHelper.ExportToExcelAsync(new List<Dictionary<string, object>>(), request.ReportName);
                fileName = $"{request.ReportCode}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            }
            else
            {
                throw new Exception($"不支援的列印格式: {request.PrintFormat}");
            }

            // 儲存檔案（實際應儲存到檔案系統或雲端儲存）
            filePath = $"reports/{fileName}";

            // 建立列印記錄
            var log = new ReportPrintLog
            {
                ReportCode = request.ReportCode,
                ReportName = request.ReportName,
                PrintType = request.PrintType,
                PrintFormat = request.PrintFormat,
                FilePath = filePath,
                FileName = fileName,
                FileSize = fileData.Length,
                PrintStatus = "Completed",
                PrintCount = 1,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                PrintedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(log);

            return new ReportPrintLogDto
            {
                PrintLogId = result.PrintLogId,
                ReportCode = result.ReportCode,
                ReportName = result.ReportName,
                PrintType = result.PrintType,
                PrintFormat = result.PrintFormat,
                FilePath = result.FilePath,
                FileName = result.FileName,
                FileSize = result.FileSize,
                PrintStatus = result.PrintStatus,
                PrintCount = result.PrintCount,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                PrintedAt = result.PrintedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印報表失敗: {request.ReportCode}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReportPrintLogDto>> GetPrintLogsAsync(ReportPrintLogQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢報表列印記錄");

            var repositoryQuery = new ReportPrintLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ReportCode = query.ReportCode,
                PrintStatus = query.PrintStatus,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ReportPrintLogDto
            {
                PrintLogId = x.PrintLogId,
                ReportCode = x.ReportCode,
                ReportName = x.ReportName,
                PrintType = x.PrintType,
                PrintFormat = x.PrintFormat,
                FilePath = x.FilePath,
                FileName = x.FileName,
                FileSize = x.FileSize,
                PrintStatus = x.PrintStatus,
                PrintCount = x.PrintCount,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                PrintedAt = x.PrintedAt
            }).ToList();

            return new PagedResult<ReportPrintLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表列印記錄失敗", ex);
            throw;
        }
    }
}

