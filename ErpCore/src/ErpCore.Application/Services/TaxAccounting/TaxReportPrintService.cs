using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Text;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 稅務報表列印服務實作 (SYST510-SYST530)
/// </summary>
public class TaxReportPrintService : BaseService, ITaxReportPrintService
{
    private readonly ITaxReportPrintRepository _repository;

    public TaxReportPrintService(
        ITaxReportPrintRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SapBankTotalDto>> GetSapBankTotalAsync(SapBankTotalQueryDto query)
    {
        try
        {
            var repositoryQuery = new SapBankTotalQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                CompId = query.CompId,
                SapStypeId = query.SapStypeId
            };

            var result = await _repository.GetSapBankTotalPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<SapBankTotalDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP銀行往來資料失敗", ex);
            throw;
        }
    }

    public async Task<CsvFileDto> GenerateSapBankTotalCsvAsync(GenerateCsvDto dto)
    {
        try
        {
            var repositoryQuery = new SapBankTotalQuery
            {
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                CompId = dto.CompId
            };

            var data = await _repository.GetSapBankTotalListAsync(repositoryQuery);

            // 產生CSV內容
            var csv = new StringBuilder();
            csv.AppendLine("SapDate,SapStypeId,CompId,BankAmt,BankBalance");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.SapDate},{item.SapStypeId ?? ""},{item.CompId},{item.BankAmt},{item.BankBalance}");
            }

            // 產生檔案名稱
            var fileName = $"{dto.CompId}_{dto.DateTo:yyyyMMdd}_BANKTOTAL.CSV";
            var filePath = Path.Combine("temp", fileName);

            // 確保目錄存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 寫入檔案
            await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);

            var fileInfo = new FileInfo(filePath);

            return new CsvFileDto
            {
                FileName = fileName,
                FileUrl = $"/api/v1/tax-accounting/tax-report-prints/syst510/download/{fileName}",
                FileSize = fileInfo.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("產生SAP銀行往來CSV檔案失敗", ex);
            throw;
        }
    }

    public async Task<Stream> DownloadCsvAsync(string fileName)
    {
        try
        {
            var filePath = Path.Combine("temp", fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"檔案不存在: {fileName}");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下載CSV檔案失敗: {fileName}", ex);
            throw;
        }
    }

    public async Task<PagedResult<TaxReportPrintDto>> GetPrintLogsAsync(TaxReportPrintQueryDto query)
    {
        try
        {
            var repositoryQuery = new TaxReportPrintQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ReportType = query.ReportType,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                PrintStatus = query.PrintStatus
            };

            var result = await _repository.GetPrintLogsPagedAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<TaxReportPrintDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢列印記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreatePrintLogAsync(CreateTaxReportPrintDto dto)
    {
        try
        {
            var entity = new TaxReportPrint
            {
                ReportType = dto.ReportType,
                ReportDate = dto.ReportDate,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                CompId = dto.CompId,
                FileName = dto.FileName,
                FileFormat = dto.FileFormat,
                PrintStatus = dto.PrintStatus,
                PrintCount = 0,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var created = await _repository.CreatePrintLogAsync(entity);

            _logger.LogInfo($"新增列印記錄成功: {created.TKey}");

            return created.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增列印記錄失敗", ex);
            throw;
        }
    }

    public async Task UpdatePrintLogAsync(long tKey, UpdateTaxReportPrintDto dto)
    {
        try
        {
            var entity = await _repository.GetPrintLogByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"列印記錄不存在: {tKey}");
            }

            entity.ReportDate = dto.ReportDate;
            entity.DateFrom = dto.DateFrom;
            entity.DateTo = dto.DateTo;
            entity.CompId = dto.CompId;
            entity.FileName = dto.FileName;
            entity.FileFormat = dto.FileFormat;
            entity.PrintStatus = dto.PrintStatus;
            entity.Notes = dto.Notes;

            await _repository.UpdatePrintLogAsync(entity);

            _logger.LogInfo($"修改列印記錄成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeletePrintLogAsync(long tKey)
    {
        try
        {
            await _repository.DeletePrintLogAsync(tKey);

            _logger.LogInfo($"刪除列印記錄成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除列印記錄失敗: {tKey}", ex);
            throw;
        }
    }

    #region 私有方法

    private SapBankTotalDto MapToDto(SapBankTotal entity)
    {
        return new SapBankTotalDto
        {
            TKey = entity.TKey,
            SapDate = entity.SapDate,
            SapStypeId = entity.SapStypeId,
            CompId = entity.CompId,
            BankAmt = entity.BankAmt,
            BankBalance = entity.BankBalance,
            Notes = entity.Notes
        };
    }

    private TaxReportPrintDto MapToDto(TaxReportPrint entity)
    {
        return new TaxReportPrintDto
        {
            TKey = entity.TKey,
            ReportType = entity.ReportType,
            ReportDate = entity.ReportDate,
            DateFrom = entity.DateFrom,
            DateTo = entity.DateTo,
            CompId = entity.CompId,
            FileName = entity.FileName,
            FileFormat = entity.FileFormat,
            PrintStatus = entity.PrintStatus,
            PrintCount = entity.PrintCount,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }

    #endregion
}

