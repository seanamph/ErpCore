using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSales;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using ErpCore.Infrastructure.Services.FileStorage;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 電子發票服務實作 (SYSG210-SYSG2B0 - 電子發票列印)
/// </summary>
public class ElectronicInvoiceService : BaseService, IElectronicInvoiceService
{
    private readonly IElectronicInvoiceRepository _repository;
    private readonly ExportHelper _exportHelper;
    private readonly IFileStorageService _fileStorageService;

    public ElectronicInvoiceService(
        IElectronicInvoiceRepository repository,
        ExportHelper exportHelper,
        IFileStorageService fileStorageService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<ElectronicInvoiceDto>> GetElectronicInvoicesAsync(ElectronicInvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new ElectronicInvoiceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                InvYm = query.InvYm,
                Track = query.Track,
                PosId = query.PosId,
                PrizeType = query.PrizeType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<ElectronicInvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<ElectronicInvoiceDto> GetElectronicInvoiceByIdAsync(long tKey)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(tKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"電子發票不存在: {tKey}");
            }

            return MapToDto(electronicInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateElectronicInvoiceAsync(CreateElectronicInvoiceDto dto)
    {
        try
        {
            // 驗證發票年月格式
            if (dto.InvYm.Length != 6 || !dto.InvYm.All(char.IsDigit))
            {
                throw new ArgumentException("發票年月格式錯誤，必須為YYYYMM格式");
            }

            var electronicInvoice = new ElectronicInvoice
            {
                PosId = dto.PosId,
                InvYm = dto.InvYm,
                Track = dto.Track,
                InvNoB = dto.InvNoB,
                InvNoE = dto.InvNoE,
                PrintCode = dto.PrintCode,
                InvoiceDate = dto.InvoiceDate,
                PrizeType = dto.PrizeType,
                PrizeAmt = dto.PrizeAmt,
                CarrierIdClear = dto.CarrierIdClear,
                AwardPrint = dto.AwardPrint,
                AwardPos = dto.AwardPos,
                AwardDate = dto.AwardDate,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(electronicInvoice);
            _logger.LogInfo($"新增電子發票成功: {dto.InvYm} (TKey: {tKey})");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增電子發票失敗: {dto.InvYm}", ex);
            throw;
        }
    }

    public async Task UpdateElectronicInvoiceAsync(UpdateElectronicInvoiceDto dto)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(dto.TKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"電子發票不存在: {dto.TKey}");
            }

            // 驗證發票年月格式
            if (dto.InvYm.Length != 6 || !dto.InvYm.All(char.IsDigit))
            {
                throw new ArgumentException("發票年月格式錯誤，必須為YYYYMM格式");
            }

            electronicInvoice.PosId = dto.PosId;
            electronicInvoice.InvYm = dto.InvYm;
            electronicInvoice.Track = dto.Track;
            electronicInvoice.InvNoB = dto.InvNoB;
            electronicInvoice.InvNoE = dto.InvNoE;
            electronicInvoice.PrintCode = dto.PrintCode;
            electronicInvoice.InvoiceDate = dto.InvoiceDate;
            electronicInvoice.PrizeType = dto.PrizeType;
            electronicInvoice.PrizeAmt = dto.PrizeAmt;
            electronicInvoice.CarrierIdClear = dto.CarrierIdClear;
            electronicInvoice.AwardPrint = dto.AwardPrint;
            electronicInvoice.AwardPos = dto.AwardPos;
            electronicInvoice.AwardDate = dto.AwardDate;
            electronicInvoice.Status = dto.Status;
            electronicInvoice.UpdatedBy = GetCurrentUserId();
            electronicInvoice.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(electronicInvoice);
            _logger.LogInfo($"修改電子發票成功: {dto.InvYm} (TKey: {dto.TKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改電子發票失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteElectronicInvoiceAsync(long tKey)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(tKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"電子發票不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除電子發票成功: {electronicInvoice.InvYm} (TKey: {tKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PrintDataDto> ManualPrintAsync(ManualPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"電子發票手動取號列印: {string.Join(",", dto.TKeys)}");

            // 查詢選定的電子發票
            var invoices = new List<ElectronicInvoice>();
            foreach (var tKey in dto.TKeys)
            {
                var invoice = await _repository.GetByIdAsync(tKey);
                if (invoice != null)
                {
                    invoices.Add(invoice);
                }
            }

            if (invoices.Count == 0)
            {
                throw new InvalidOperationException("未找到選定的電子發票");
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "PosId", DisplayName = "POS代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvYm", DisplayName = "發票年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Track", DisplayName = "字軌", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNoB", DisplayName = "發票號碼起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNoE", DisplayName = "發票號碼迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrintCode", DisplayName = "列印條碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvoiceDate", DisplayName = "發票日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "PrizeType", DisplayName = "獎項類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeAmt", DisplayName = "獎項金額", DataType = ExportDataType.Decimal }
            };

            // 產生PDF
            var pdfBytes = _exportHelper.ExportToPdf(invoices, columns, "電子發票列印");

            // 儲存檔案
            var reportId = $"RPT{DateTime.Now:yyyyMMddHHmmss}";
            var fileName = $"電子發票列印_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = await _fileStorageService.SaveFileAsync(pdfBytes, fileName, "ElectronicInvoices");

            return new PrintDataDto
            {
                ReportId = reportId,
                FileUrl = $"/api/v1/electronic-invoices/download/{reportId}",
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("電子發票手動取號列印失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ElectronicInvoiceAwardDto>> GetAwardListAsync(AwardListQueryDto query)
    {
        try
        {
            var repositoryQuery = new AwardListQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                InvYm = query.InvYm,
                PrizeType = query.PrizeType
            };

            var result = await _repository.QueryAwardListAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToAwardDto).ToList();

            return new PagedResult<ElectronicInvoiceAwardDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢中獎清冊失敗", ex);
            throw;
        }
    }

    public async Task<PrintDataDto> AwardPrintAsync(AwardPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"中獎清冊列印: InvYm={dto.InvYm}, PrizeType={dto.PrizeType}");

            // 查詢中獎清冊資料
            var query = new AwardListQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                InvYm = dto.InvYm,
                PrizeType = dto.PrizeType
            };

            var result = await GetAwardListAsync(query);

            if (result.Items.Count == 0)
            {
                throw new InvalidOperationException("未找到中獎清冊資料");
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvYm", DisplayName = "發票年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Track", DisplayName = "字軌", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNo", DisplayName = "發票號碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeType", DisplayName = "獎項類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeAmt", DisplayName = "獎項金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AwardDate", DisplayName = "中獎日期", DataType = ExportDataType.Date }
            };

            // 產生PDF
            var pdfBytes = _exportHelper.ExportToPdf(result.Items, columns, "中獎清冊");

            // 儲存檔案
            var reportId = $"AWARD{DateTime.Now:yyyyMMddHHmmss}";
            var fileName = $"中獎清冊_{dto.InvYm ?? "ALL"}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = await _fileStorageService.SaveFileAsync(pdfBytes, fileName, "AwardLists");

            return new PrintDataDto
            {
                ReportId = reportId,
                FileUrl = $"/api/v1/electronic-invoices/award-download/{reportId}",
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("中獎清冊列印失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private ElectronicInvoiceDto MapToDto(ElectronicInvoice electronicInvoice)
    {
        return new ElectronicInvoiceDto
        {
            TKey = electronicInvoice.TKey,
            PosId = electronicInvoice.PosId,
            InvYm = electronicInvoice.InvYm,
            Track = electronicInvoice.Track,
            InvNoB = electronicInvoice.InvNoB,
            InvNoE = electronicInvoice.InvNoE,
            PrintCode = electronicInvoice.PrintCode,
            InvoiceDate = electronicInvoice.InvoiceDate,
            PrizeType = electronicInvoice.PrizeType,
            PrizeAmt = electronicInvoice.PrizeAmt,
            CarrierIdClear = electronicInvoice.CarrierIdClear,
            AwardPrint = electronicInvoice.AwardPrint,
            AwardPos = electronicInvoice.AwardPos,
            AwardDate = electronicInvoice.AwardDate,
            Status = electronicInvoice.Status,
            CreatedBy = electronicInvoice.CreatedBy,
            CreatedAt = electronicInvoice.CreatedAt,
            UpdatedBy = electronicInvoice.UpdatedBy,
            UpdatedAt = electronicInvoice.UpdatedAt
        };
    }

    /// <summary>
    /// 將 Entity 轉換為中獎 DTO
    /// </summary>
    private ElectronicInvoiceAwardDto MapToAwardDto(ElectronicInvoice electronicInvoice)
    {
        return new ElectronicInvoiceAwardDto
        {
            TKey = electronicInvoice.TKey,
            PosId = electronicInvoice.PosId,
            InvYm = electronicInvoice.InvYm,
            Track = electronicInvoice.Track,
            InvNoB = electronicInvoice.InvNoB,
            InvNoE = electronicInvoice.InvNoE,
            PrintCode = electronicInvoice.PrintCode,
            InvoiceDate = electronicInvoice.InvoiceDate,
            PrizeType = electronicInvoice.PrizeType,
            PrizeAmt = electronicInvoice.PrizeAmt,
            CarrierIdClear = electronicInvoice.CarrierIdClear,
            AwardPrint = electronicInvoice.AwardPrint,
            AwardPos = electronicInvoice.AwardPos,
            AwardDate = electronicInvoice.AwardDate
        };
    }
}

