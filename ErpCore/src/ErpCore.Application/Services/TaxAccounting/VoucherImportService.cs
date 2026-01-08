using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 傳票轉入服務實作 (SYST002-SYST003)
/// </summary>
public class VoucherImportService : BaseService, IVoucherImportService
{
    private readonly IVoucherImportRepository _repository;

    public VoucherImportService(
        IVoucherImportRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<VoucherImportLogDto> UploadAhmFileAsync(IFormFile file)
    {
        try
        {
            // 驗證檔案
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("檔案不能為空");
            }

            if (file.Length > 100_000_000) // 100MB
            {
                throw new ArgumentException("檔案大小不能超過 100MB");
            }

            // 儲存檔案
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";
            var filePath = Path.Combine("temp", "imports", fileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 建立轉入記錄
            var importLog = new VoucherImportLog
            {
                ImportType = "AHM",
                FileName = file.FileName,
                FilePath = filePath,
                ImportDate = DateTime.Now,
                TotalCount = 0,
                SuccessCount = 0,
                FailCount = 0,
                SkipCount = 0,
                Status = "P",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateImportLogAsync(importLog);

            // TODO: 非同步處理檔案解析和轉入

            _logger.LogInfo($"上傳住金傳票檔案成功: {file.FileName}");

            return MapToDto(importLog, tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError("上傳住金傳票檔案失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<VoucherImportLogDto>> GetImportLogsAsync(VoucherImportLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new VoucherImportLogQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ImportType = query.ImportType,
                Status = query.Status,
                ImportDateFrom = query.ImportDateFrom,
                ImportDateTo = query.ImportDateTo,
                FileName = query.FileName
            };

            var result = await _repository.GetImportLogsPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x, x.TKey)).ToList();

            return new PagedResult<VoucherImportLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢轉入記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherImportLogDetailDto> GetImportLogDetailsAsync(long tKey, VoucherImportDetailQueryDto query)
    {
        try
        {
            var importLog = await _repository.GetImportLogByIdAsync(tKey);
            if (importLog == null)
            {
                throw new InvalidOperationException($"轉入記錄不存在: {tKey}");
            }

            var repositoryQuery = new VoucherImportDetailQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Status = query.Status
            };

            var details = await _repository.GetImportDetailsAsync(tKey, repositoryQuery);

            return new VoucherImportLogDetailDto
            {
                ImportLog = MapToDto(importLog, tKey),
                Details = details.Select(x => MapToDetailDto(x)).ToList(),
                TotalCount = details.Count,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(details.Count / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢轉入記錄明細失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ImportResultDto> ImportHtvDailyAsync(ImportHtvDto dto)
    {
        try
        {
            // TODO: 實作日立日結傳票轉入邏輯
            throw new NotImplementedException("日立日結傳票轉入功能尚未實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("日立日結傳票轉入失敗", ex);
            throw;
        }
    }

    public async Task<ImportResultDto> ImportHtvMonthlyAsync(ImportHtvDto dto)
    {
        try
        {
            // TODO: 實作日立月結傳票轉入邏輯
            throw new NotImplementedException("日立月結傳票轉入功能尚未實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("日立月結傳票轉入失敗", ex);
            throw;
        }
    }

    public async Task<ImportResultDto> ImportHtvSupplierAsync(ImportHtvDto dto)
    {
        try
        {
            // TODO: 實作日立供應商資料轉入邏輯
            throw new NotImplementedException("日立供應商資料轉入功能尚未實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("日立供應商資料轉入失敗", ex);
            throw;
        }
    }

    public async Task<ImportProgressDto> GetImportProgressAsync(long tKey)
    {
        try
        {
            var importLog = await _repository.GetImportLogByIdAsync(tKey);
            if (importLog == null)
            {
                throw new InvalidOperationException($"轉入記錄不存在: {tKey}");
            }

            var progress = importLog.TotalCount > 0
                ? (double)(importLog.SuccessCount + importLog.FailCount + importLog.SkipCount) / importLog.TotalCount * 100
                : 0;

            return new ImportProgressDto
            {
                TKey = tKey,
                Status = importLog.Status,
                TotalCount = importLog.TotalCount,
                ProcessedCount = importLog.SuccessCount + importLog.FailCount + importLog.SkipCount,
                SuccessCount = importLog.SuccessCount,
                FailCount = importLog.FailCount,
                SkipCount = importLog.SkipCount,
                Progress = progress
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢轉入進度失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ImportResultDto> RetryImportAsync(long tKey)
    {
        try
        {
            // TODO: 實作重新處理邏輯
            throw new NotImplementedException("重新處理功能尚未實作");
        }
        catch (Exception ex)
        {
            _logger.LogError($"重新處理轉入記錄失敗: {tKey}", ex);
            throw;
        }
    }

    #region 私有方法

    private VoucherImportLogDto MapToDto(VoucherImportLog entity, long tKey)
    {
        return new VoucherImportLogDto
        {
            TKey = tKey,
            ImportType = entity.ImportType,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            ImportDate = entity.ImportDate,
            TotalCount = entity.TotalCount,
            SuccessCount = entity.SuccessCount,
            FailCount = entity.FailCount,
            SkipCount = entity.SkipCount,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage
        };
    }

    private VoucherImportDetailDto MapToDetailDto(VoucherImportDetail entity)
    {
        return new VoucherImportDetailDto
        {
            TKey = entity.TKey,
            RowNumber = entity.RowNumber,
            VoucherTKey = entity.VoucherTKey,
            Status = entity.Status,
            ErrorMessage = entity.ErrorMessage,
            SourceData = entity.SourceData
        };
    }

    #endregion
}

