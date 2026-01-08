using System.Text.Json;
using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemExtension;
using ErpCore.Infrastructure.Repositories.SystemExtension;
using ErpCore.Infrastructure.Services.FileStorage;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemExtension;

/// <summary>
/// 系統擴展報表服務實作 (SYSX140)
/// </summary>
public class SystemExtensionReportService : BaseService, ISystemExtensionReportService
{
    private readonly ISystemExtensionRepository _extensionRepository;
    private readonly ISystemExtensionReportRepository _reportRepository;
    private readonly ExportHelper _exportHelper;
    private readonly IFileStorageService _fileStorageService;

    public SystemExtensionReportService(
        ISystemExtensionRepository extensionRepository,
        ISystemExtensionReportRepository reportRepository,
        ExportHelper exportHelper,
        IFileStorageService fileStorageService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _extensionRepository = extensionRepository;
        _reportRepository = reportRepository;
        _exportHelper = exportHelper;
        _fileStorageService = fileStorageService;
    }

    public async Task<SystemExtensionReportQueryResultDto> QueryReportDataAsync(SystemExtensionReportQueryRequestDto request)
    {
        try
        {
            // 構建查詢條件
            var query = new SystemExtensionQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                ExtensionId = request.Filters?.ExtensionId,
                ExtensionName = request.Filters?.ExtensionName,
                ExtensionType = request.Filters?.ExtensionType,
                Status = request.Filters?.Status,
                CreatedDateFrom = request.Filters?.CreatedDateFrom,
                CreatedDateTo = request.Filters?.CreatedDateTo
            };

            // 設定排序
            if (request.OrderBy != null && request.OrderBy.Count > 0)
            {
                query.SortField = request.OrderBy[0];
                query.SortOrder = request.OrderBy.Count > 1 ? request.OrderBy[1] : "ASC";
            }

            // 查詢資料
            var result = await _extensionRepository.QueryAsync(query);

            // 查詢統計資訊
            var statisticsQuery = new SystemExtensionStatisticsQuery
            {
                ExtensionType = request.Filters?.ExtensionType,
                Status = request.Filters?.Status
            };
            var statistics = await _extensionRepository.GetStatisticsAsync(statisticsQuery);

            // 轉換為 DTO
            var items = result.Items.Select(x => new SystemExtensionDto
            {
                TKey = x.TKey,
                ExtensionId = x.ExtensionId,
                ExtensionName = x.ExtensionName,
                ExtensionType = x.ExtensionType,
                ExtensionValue = x.ExtensionValue,
                ExtensionConfig = x.ExtensionConfig,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                CreatedPriority = x.CreatedPriority,
                CreatedGroup = x.CreatedGroup
            }).ToList();

            var statisticsDto = new SystemExtensionStatisticsDto
            {
                TotalCount = statistics.TotalCount,
                ActiveCount = statistics.ActiveCount,
                InactiveCount = statistics.InactiveCount,
                ByType = statistics.ByType.Select(x => new SystemExtensionTypeCountDto
                {
                    ExtensionType = x.ExtensionType,
                    Count = x.Count
                }).ToList()
            };

            return new SystemExtensionReportQueryResultDto
            {
                Items = items,
                TotalCount = result.TotalCount,
                Statistics = statisticsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展報表資料失敗", ex);
            throw;
        }
    }

    public async Task<SystemExtensionReportDto> GeneratePdfReportAsync(GenerateSystemExtensionReportDto request)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var query = new SystemExtensionQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                ExtensionId = request.Filters?.ExtensionId,
                ExtensionName = request.Filters?.ExtensionName,
                ExtensionType = request.Filters?.ExtensionType,
                Status = request.Filters?.Status,
                CreatedDateFrom = request.Filters?.CreatedDateFrom,
                CreatedDateTo = request.Filters?.CreatedDateTo
            };

            var result = await _extensionRepository.QueryAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ExtensionId", DisplayName = "擴展功能代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionName", DisplayName = "擴展功能名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionType", DisplayName = "擴展類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionValue", DisplayName = "擴展值", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "排序序號", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedBy", DisplayName = "建立者", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime }
            };

            // 產生 PDF
            var pdfBytes = _exportHelper.ExportToPdf(result.Items, columns, request.ReportName);

            // 儲存檔案
            var fileName = $"{request.ReportName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = await _fileStorageService.SaveFileAsync(pdfBytes, fileName, "SystemExtensionReports");

            // 建立報表記錄
            var report = new SystemExtensionReport
            {
                ReportName = request.ReportName,
                ReportType = "PDF",
                ReportTemplate = request.Template,
                QueryConditions = request.Filters != null ? JsonSerializer.Serialize(request.Filters) : null,
                GeneratedDate = DateTime.Now,
                GeneratedBy = GetCurrentUserId(),
                FileUrl = filePath, // 儲存相對路徑
                FileSize = pdfBytes.Length,
                Status = "COMPLETED",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var createdReport = await _reportRepository.CreateAsync(report);

            return MapToDto(createdReport);
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生 PDF 報表失敗: {request.ReportName}", ex);
            throw;
        }
    }

    public async Task<SystemExtensionReportDto> GenerateExcelReportAsync(GenerateSystemExtensionReportDto request)
    {
        try
        {
            // 查詢所有資料（不分頁）
            var query = new SystemExtensionQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                ExtensionId = request.Filters?.ExtensionId,
                ExtensionName = request.Filters?.ExtensionName,
                ExtensionType = request.Filters?.ExtensionType,
                Status = request.Filters?.Status,
                CreatedDateFrom = request.Filters?.CreatedDateFrom,
                CreatedDateTo = request.Filters?.CreatedDateTo
            };

            var result = await _extensionRepository.QueryAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ExtensionId", DisplayName = "擴展功能代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionName", DisplayName = "擴展功能名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionType", DisplayName = "擴展類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ExtensionValue", DisplayName = "擴展值", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "SeqNo", DisplayName = "排序序號", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedBy", DisplayName = "建立者", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime }
            };

            // 產生 Excel
            var excelBytes = _exportHelper.ExportToExcel(result.Items, columns, request.ReportName, request.ReportName);

            // 儲存檔案
            var fileName = $"{request.ReportName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var filePath = await _fileStorageService.SaveFileAsync(excelBytes, fileName, "SystemExtensionReports");

            // 建立報表記錄
            var report = new SystemExtensionReport
            {
                ReportName = request.ReportName,
                ReportType = "Excel",
                ReportTemplate = request.Template,
                QueryConditions = request.Filters != null ? JsonSerializer.Serialize(request.Filters) : null,
                GeneratedDate = DateTime.Now,
                GeneratedBy = GetCurrentUserId(),
                FileUrl = filePath, // 儲存相對路徑
                FileSize = excelBytes.Length,
                Status = "COMPLETED",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var createdReport = await _reportRepository.CreateAsync(report);

            return MapToDto(createdReport);
        }
        catch (Exception ex)
        {
            _logger.LogError($"產生 Excel 報表失敗: {request.ReportName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SystemExtensionReportDto>> GetReportsAsync(SystemExtensionReportQueryDto query)
    {
        try
        {
            var repositoryQuery = new SystemExtensionReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ReportName = query.ReportName,
                ReportType = query.ReportType,
                StartDate = query.StartDate,
                EndDate = query.EndDate
            };

            var result = await _reportRepository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<SystemExtensionReportDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展報表記錄失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> DownloadReportAsync(long reportId)
    {
        try
        {
            var report = await _reportRepository.GetByReportIdAsync(reportId);
            if (report == null)
            {
                throw new InvalidOperationException($"報表記錄不存在: {reportId}");
            }

            if (string.IsNullOrEmpty(report.FileUrl))
            {
                throw new InvalidOperationException($"報表檔案不存在: {reportId}");
            }

            // FileUrl 儲存的是相對路徑
            var filePath = report.FileUrl;

            // 讀取檔案
            var fileBytes = await _fileStorageService.ReadFileAsync(filePath);
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task DeleteReportAsync(long reportId)
    {
        try
        {
            var report = await _reportRepository.GetByReportIdAsync(reportId);
            if (report == null)
            {
                throw new InvalidOperationException($"報表記錄不存在: {reportId}");
            }

            // 刪除檔案（如果存在）
            if (!string.IsNullOrEmpty(report.FileUrl))
            {
                try
                {
                    // FileUrl 儲存的是相對路徑
                    await _fileStorageService.DeleteFileAsync(report.FileUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"刪除報表檔案失敗: {reportId}, {ex.Message}");
                    // 繼續刪除記錄，即使檔案刪除失敗
                }
            }

            // 刪除記錄
            await _reportRepository.DeleteAsync(reportId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除報表記錄失敗: {reportId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private SystemExtensionReportDto MapToDto(SystemExtensionReport entity)
    {
        return new SystemExtensionReportDto
        {
            ReportId = entity.ReportId,
            ReportName = entity.ReportName,
            ReportType = entity.ReportType,
            ReportTemplate = entity.ReportTemplate,
            QueryConditions = entity.QueryConditions,
            GeneratedDate = entity.GeneratedDate,
            GeneratedBy = entity.GeneratedBy,
            FileUrl = entity.FileUrl,
            FileSize = entity.FileSize,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

