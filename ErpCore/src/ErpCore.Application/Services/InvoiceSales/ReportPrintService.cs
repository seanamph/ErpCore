using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Logging;
using System.Text.Json;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 報表列印服務實作 (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
public class ReportPrintService : BaseService, IReportPrintService
{
    private readonly IReportTemplateRepository _templateRepository;
    private readonly IReportPrintLogRepository _logRepository;

    public ReportPrintService(
        IReportTemplateRepository templateRepository,
        IReportPrintLogRepository logRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _templateRepository = templateRepository;
        _logRepository = logRepository;
    }

    public async Task<ReportPrintResultDto> PrintReportAsync(ReportPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"列印報表 - 報表類型: {dto.ReportType}, 使用者: {_userContext.UserId}");

            // 產生報表編號
            var reportId = $"RPT{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

            // 查詢模板
            ReportTemplate? template = null;
            if (!string.IsNullOrEmpty(dto.TemplateId))
            {
                template = await _templateRepository.GetByIdAsync(dto.TemplateId);
                if (template == null)
                {
                    throw new Exception($"報表模板不存在: {dto.TemplateId}");
                }
            }

            // 產生報表檔案（這裡簡化處理，實際應該根據模板產生PDF/Excel/Word）
            var fileName = $"{dto.ReportType}_{DateTime.Now:yyyyMMddHHmmss}.{dto.PrintFormat.ToLower()}";
            var fileUrl = $"/api/v1/reports/download/{reportId}";

            // 記錄列印記錄
            var log = new ReportPrintLog
            {
                ReportId = reportId,
                ReportType = dto.ReportType,
                TemplateId = dto.TemplateId,
                PrintUserId = _userContext.UserId ?? string.Empty,
                PrintDate = DateTime.Now,
                PrintFormat = dto.PrintFormat,
                FileUrl = fileUrl,
                Parameters = JsonSerializer.Serialize(dto.Parameters),
                Status = "S",
                CreatedAt = DateTime.Now
            };

            await _logRepository.CreateAsync(log);

            return new ReportPrintResultDto
            {
                ReportId = reportId,
                FileUrl = fileUrl,
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("列印報表失敗", ex);

            // 記錄失敗記錄
            try
            {
                var reportId = $"RPT{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
                var log = new ReportPrintLog
                {
                    ReportId = reportId,
                    ReportType = dto.ReportType,
                    TemplateId = dto.TemplateId,
                    PrintUserId = _userContext.UserId ?? string.Empty,
                    PrintDate = DateTime.Now,
                    PrintFormat = dto.PrintFormat,
                    Parameters = JsonSerializer.Serialize(dto.Parameters),
                    Status = "F",
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.Now
                };
                await _logRepository.CreateAsync(log);
            }
            catch
            {
                // 忽略記錄失敗
            }

            throw;
        }
    }

    public async Task<string> PreviewReportAsync(ReportPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"預覽報表 - 報表類型: {dto.ReportType}, 使用者: {_userContext.UserId}");

            // 查詢模板
            ReportTemplate? template = null;
            if (!string.IsNullOrEmpty(dto.TemplateId))
            {
                template = await _templateRepository.GetByIdAsync(dto.TemplateId);
            }

            // 產生預覽HTML（這裡簡化處理，實際應該根據模板產生）
            var html = $@"
                <html>
                <head>
                    <title>報表預覽 - {dto.ReportType}</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; margin: 20px; }}
                        h1 {{ color: #333; }}
                        table {{ border-collapse: collapse; width: 100%; }}
                        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                        th {{ background-color: #f2f2f2; }}
                    </style>
                </head>
                <body>
                    <h1>報表預覽 - {dto.ReportType}</h1>
                    <p>報表參數: {JsonSerializer.Serialize(dto.Parameters)}</p>
                    <p>列印格式: {dto.PrintFormat}</p>
                </body>
                </html>";

            return html;
        }
        catch (Exception ex)
        {
            _logger.LogError("預覽報表失敗", ex);
            throw;
        }
    }

    public async Task<List<ReportTemplateDto>> GetTemplatesAsync(string reportType, string? status = null)
    {
        try
        {
            _logger.LogInfo($"查詢報表模板列表 - 報表類型: {reportType}");

            var templates = await _templateRepository.GetByReportTypeAsync(reportType, status);

            return templates.Select(x => new ReportTemplateDto
            {
                TemplateId = x.TemplateId,
                TemplateName = x.TemplateName,
                TemplateType = x.TemplateType,
                ReportType = x.ReportType,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢報表模板列表失敗", ex);
            throw;
        }
    }
}

