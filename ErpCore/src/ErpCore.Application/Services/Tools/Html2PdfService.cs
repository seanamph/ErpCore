using ErpCore.Application.DTOs.Tools;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Tools;
using ErpCore.Infrastructure.Repositories.Tools;
using ErpCore.Infrastructure.Services.FileStorage;
using ErpCore.Shared.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ErpCore.Application.Services.Tools;

/// <summary>
/// HTML轉PDF服務實作
/// 使用 QuestPDF 產生 PDF
/// </summary>
public class Html2PdfService : BaseService, IHtml2PdfService
{
    private readonly IPdfConversionLogRepository _repository;
    private readonly IFileStorageService _fileStorageService;

    public Html2PdfService(
        IPdfConversionLogRepository repository,
        IFileStorageService fileStorageService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<Html2PdfResponseDto> ConvertHtmlToPdfAsync(Html2PdfRequestDto request)
    {
        try
        {
            _logger.LogInfo($"開始轉換HTML為PDF: {request.FileName}");

            // 建立轉換記錄
            var log = new PdfConversionLog
            {
                LogId = Guid.NewGuid(),
                SourceHtml = request.HtmlContent,
                FileName = request.FileName,
                ConversionStatus = "PENDING",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(log);

            try
            {
                // 使用 QuestPDF 產生 PDF
                var pdfBytes = GeneratePdfFromHtml(request);

                // 儲存PDF檔案
                var subDirectory = "pdf";
                var filePath = await _fileStorageService.SaveFileAsync(pdfBytes, request.FileName, subDirectory);

                // 更新轉換記錄
                log.PdfFilePath = filePath;
                log.FileSize = pdfBytes.Length;
                log.ConversionStatus = "SUCCESS";
                log.CompletedAt = DateTime.Now;
                await _repository.UpdateAsync(log);

                _logger.LogInfo($"HTML轉PDF成功: {log.LogId}");

                return new Html2PdfResponseDto
                {
                    LogId = log.LogId,
                    PdfFilePath = filePath,
                    FileName = request.FileName,
                    FileSize = pdfBytes.Length
                };
            }
            catch (Exception ex)
            {
                // 更新轉換記錄為失敗
                log.ConversionStatus = "FAILED";
                log.ErrorMessage = ex.Message;
                log.CompletedAt = DateTime.Now;
                await _repository.UpdateAsync(log);

                _logger.LogError($"HTML轉PDF失敗: {log.LogId}", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"轉換HTML為PDF失敗: {request.FileName}", ex);
            throw;
        }
    }

    public async Task<byte[]?> DownloadPdfAsync(Guid logId)
    {
        try
        {
            var log = await _repository.GetByIdAsync(logId);
            if (log == null || log.ConversionStatus != "SUCCESS" || string.IsNullOrEmpty(log.PdfFilePath))
            {
                return null;
            }

            var fileBytes = await _fileStorageService.ReadFileAsync(log.PdfFilePath);
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載PDF失敗: {logId}", ex);
            throw;
        }
    }

    public async Task<PdfConversionLogDto?> GetConversionLogAsync(Guid logId)
    {
        try
        {
            var log = await _repository.GetByIdAsync(logId);
            if (log == null)
            {
                return null;
            }

            return new PdfConversionLogDto
            {
                LogId = log.LogId,
                PdfFilePath = log.PdfFilePath,
                FileName = log.FileName,
                FileSize = log.FileSize,
                ConversionStatus = log.ConversionStatus,
                ErrorMessage = log.ErrorMessage,
                CreatedBy = log.CreatedBy,
                CreatedAt = log.CreatedAt,
                CompletedAt = log.CompletedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢PDF轉換記錄失敗: {logId}", ex);
            throw;
        }
    }

    private byte[] GeneratePdfFromHtml(Html2PdfRequestDto request)
    {
        // 簡化版：將HTML內容轉換為PDF
        // 注意：QuestPDF 不直接支援 HTML，這裡使用簡化的方式
        // 實際應用中可能需要使用其他庫（如 PuppeteerSharp）或將 HTML 轉換為 QuestPDF 的 Document 結構

        var pageSize = request.Options?.PageSize ?? "A4";
        var orientation = request.Options?.Orientation ?? "Portrait";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(GetPageSize(pageSize));
                page.Margin(GetMargin(request.Options?.Margin));
                page.Content().Column(column =>
                {
                    // 簡化處理：將HTML內容作為文字顯示
                    // 實際應用中需要解析HTML並轉換為QuestPDF的結構
                    column.Item().Text(request.HtmlContent);
                });
            });
        });

        return document.GeneratePdf();
    }

    private QuestPDF.Infrastructure.PageSize GetPageSize(string pageSize)
    {
        return pageSize.ToUpperInvariant() switch
        {
            "A4" => PageSizes.A4,
            "LETTER" => PageSizes.Letter,
            _ => PageSizes.A4
        };
    }

    private QuestPDF.Infrastructure.Unit GetMargin(PdfMarginDto? margin)
    {
        if (margin == null)
        {
            return QuestPDF.Infrastructure.Unit.FromPoint(10);
        }
        return QuestPDF.Infrastructure.Unit.FromPoint(margin.Top);
    }
}

