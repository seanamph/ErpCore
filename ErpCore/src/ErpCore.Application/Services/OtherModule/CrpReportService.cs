using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Repositories.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// CRP報表服務實作
/// 提供 Crystal Reports 報表生成、下載等功能
/// </summary>
public class CrpReportService : BaseService, ICrpReportService
{
    private readonly ICrpReportRepository _repository;
    private static readonly Dictionary<long, byte[]> _reportFiles = new();

    public CrpReportService(
        ICrpReportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<List<CrystalReportDto>> GetReportsAsync()
    {
        try
        {
            _logger.LogInfo("取得報表設定列表");

            var reports = await _repository.GetAllAsync();
            return reports.Select(r => new CrystalReportDto
            {
                ReportId = r.ReportId,
                ReportCode = r.ReportCode,
                ReportName = r.ReportName,
                ReportPath = r.ReportPath,
                MdbName = r.MdbName,
                Parameters = r.Parameters,
                ExportOptions = r.ExportOptions,
                Status = r.Status,
                CreatedBy = r.CreatedBy,
                CreatedAt = r.CreatedAt,
                UpdatedBy = r.UpdatedBy,
                UpdatedAt = r.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得報表設定列表失敗", ex);
            throw;
        }
    }

    public async Task<CrystalReportDto?> GetReportByCodeAsync(string reportCode)
    {
        try
        {
            _logger.LogInfo($"取得報表設定: {reportCode}");

            var report = await _repository.GetByReportCodeAsync(reportCode);
            if (report == null)
            {
                return null;
            }

            return new CrystalReportDto
            {
                ReportId = report.ReportId,
                ReportCode = report.ReportCode,
                ReportName = report.ReportName,
                ReportPath = report.ReportPath,
                MdbName = report.MdbName,
                Parameters = report.Parameters,
                ExportOptions = report.ExportOptions,
                Status = report.Status,
                CreatedBy = report.CreatedBy,
                CreatedAt = report.CreatedAt,
                UpdatedBy = report.UpdatedBy,
                UpdatedAt = report.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得報表設定失敗: {reportCode}", ex);
            throw;
        }
    }

    public async Task<GenerateReportResponseDto> GenerateReportAsync(GenerateReportRequestDto request)
    {
        try
        {
            _logger.LogInfo($"生成報表: {request.ReportCode}");

            var report = await _repository.GetByReportCodeAsync(request.ReportCode);
            if (report == null)
            {
                throw new InvalidOperationException($"報表設定不存在: {request.ReportCode}");
            }

            // 這裡應該調用 Crystal Reports 引擎生成報表
            // 由於 Crystal Reports 需要額外的授權和設定，這裡僅提供框架
            var startTime = DateTime.Now;
            var parametersJson = request.Parameters != null ? JsonSerializer.Serialize(request.Parameters) : null;

            // 模擬報表生成（實際應該調用 Crystal Reports API）
            var reportBytes = new byte[0]; // 實際應該從 Crystal Reports 生成
            var duration = (int)(DateTime.Now - startTime).TotalMilliseconds;

            // 儲存報表檔案（實際應該儲存到檔案系統或雲端儲存）
            var reportId = report.ReportId;
            _reportFiles[reportId] = reportBytes;

            // 記錄操作日誌
            var log = new CrystalReportLog
            {
                ReportId = report.ReportId,
                ReportCode = report.ReportCode,
                OperationType = "GENERATE",
                Parameters = parametersJson,
                Status = "SUCCESS",
                FileSize = reportBytes.Length,
                Duration = duration,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };
            await _repository.CreateLogAsync(log);

            return new GenerateReportResponseDto
            {
                ReportId = reportId,
                DownloadUrl = $"/api/v1/other-module/crp/reports/{reportId}/download"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"生成報表失敗: {request.ReportCode}", ex);
            throw;
        }
    }

    public async Task<byte[]?> DownloadReportAsync(long reportId)
    {
        try
        {
            _logger.LogInfo($"下載報表: {reportId}");

            if (_reportFiles.TryGetValue(reportId, out var reportBytes))
            {
                return reportBytes;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載報表失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<long> CreateReportAsync(CreateCrystalReportDto dto)
    {
        try
        {
            _logger.LogInfo($"新增報表設定: {dto.ReportCode}");

            var report = new CrystalReport
            {
                ReportCode = dto.ReportCode,
                ReportName = dto.ReportName,
                ReportPath = dto.ReportPath,
                MdbName = dto.MdbName,
                Parameters = dto.Parameters,
                ExportOptions = dto.ExportOptions,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(report);
            return result.ReportId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增報表設定失敗: {dto.ReportCode}", ex);
            throw;
        }
    }

    public async Task UpdateReportAsync(long reportId, CreateCrystalReportDto dto)
    {
        try
        {
            _logger.LogInfo($"修改報表設定: {reportId}");

            var existingReport = await _repository.GetByReportCodeAsync(dto.ReportCode);
            if (existingReport == null || existingReport.ReportId != reportId)
            {
                throw new InvalidOperationException($"報表設定不存在: {reportId}");
            }

            existingReport.ReportName = dto.ReportName;
            existingReport.ReportPath = dto.ReportPath;
            existingReport.MdbName = dto.MdbName;
            existingReport.Parameters = dto.Parameters;
            existingReport.ExportOptions = dto.ExportOptions;
            existingReport.Status = dto.Status;
            existingReport.UpdatedBy = GetCurrentUserId();
            existingReport.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existingReport);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改報表設定失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task DeleteReportAsync(long reportId)
    {
        try
        {
            _logger.LogInfo($"刪除報表設定: {reportId}");

            await _repository.DeleteAsync(reportId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除報表設定失敗: {reportId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CrystalReportLogDto>> GetLogsAsync(string? reportCode, int pageIndex, int pageSize)
    {
        try
        {
            _logger.LogInfo($"查詢操作記錄列表: reportCode={reportCode}");

            var result = await _repository.GetLogsAsync(reportCode, pageIndex, pageSize);
            return new PagedResult<CrystalReportLogDto>
            {
                Items = result.Items.Select(l => new CrystalReportLogDto
                {
                    LogId = l.LogId,
                    ReportId = l.ReportId,
                    ReportCode = l.ReportCode,
                    OperationType = l.OperationType,
                    Parameters = l.Parameters,
                    Status = l.Status,
                    ErrorMessage = l.ErrorMessage,
                    FileSize = l.FileSize,
                    Duration = l.Duration,
                    CreatedBy = l.CreatedBy,
                    CreatedAt = l.CreatedAt
                }).ToList(),
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢操作記錄列表失敗", ex);
            throw;
        }
    }
}

