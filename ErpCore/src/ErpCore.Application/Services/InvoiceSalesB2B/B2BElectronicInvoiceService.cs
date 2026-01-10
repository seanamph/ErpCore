using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.InvoiceSalesB2B;
using ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using ErpCore.Infrastructure.Services.FileStorage;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票服務實作 (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public class B2BElectronicInvoiceService : BaseService, IB2BElectronicInvoiceService
{
    private readonly IB2BElectronicInvoiceRepository _repository;
    private readonly ExportHelper _exportHelper;
    private readonly IFileStorageService _fileStorageService;

    public B2BElectronicInvoiceService(
        IB2BElectronicInvoiceRepository repository,
        ExportHelper exportHelper,
        IFileStorageService fileStorageService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
        _fileStorageService = fileStorageService;
    }

    public async Task<PagedResult<B2BElectronicInvoiceDto>> GetB2BElectronicInvoicesAsync(B2BElectronicInvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new B2BElectronicInvoiceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                InvoiceId = query.InvoiceId,
                PosId = query.PosId,
                InvYm = query.InvYm,
                Track = query.Track,
                PrizeType = query.PrizeType,
                TransferType = query.TransferType,
                Status = query.Status,
                B2BFlag = query.B2BFlag ?? "Y"
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<B2BElectronicInvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B電子發票列表失敗", ex);
            throw;
        }
    }

    public async Task<B2BElectronicInvoiceDto> GetB2BElectronicInvoiceByIdAsync(long tKey)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(tKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"B2B電子發票不存在: {tKey}");
            }

            return MapToDto(electronicInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢B2B電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateB2BElectronicInvoiceAsync(CreateB2BElectronicInvoiceDto dto)
    {
        try
        {
            // 驗證發票編號唯一性
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId);
            if (exists)
            {
                throw new ArgumentException($"發票編號已存在: {dto.InvoiceId}");
            }

            // 驗證發票年月格式
            if (dto.InvYm.Length != 6 || !dto.InvYm.All(char.IsDigit))
            {
                throw new ArgumentException("發票年月格式錯誤，必須為YYYYMM格式");
            }

            var electronicInvoice = new B2BElectronicInvoice
            {
                InvoiceId = dto.InvoiceId,
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
                B2BFlag = "Y",
                TransferType = dto.TransferType,
                TransferStatus = dto.TransferStatus,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(electronicInvoice);
            _logger.LogInfo($"新增B2B電子發票成功: {dto.InvoiceId} (TKey: {tKey})");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增B2B電子發票失敗: {dto.InvoiceId}", ex);
            throw;
        }
    }

    public async Task UpdateB2BElectronicInvoiceAsync(UpdateB2BElectronicInvoiceDto dto)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(dto.TKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"B2B電子發票不存在: {dto.TKey}");
            }

            // 驗證發票編號唯一性
            var exists = await _repository.ExistsByInvoiceIdAsync(dto.InvoiceId, dto.TKey);
            if (exists)
            {
                throw new ArgumentException($"發票編號已存在: {dto.InvoiceId}");
            }

            // 驗證發票年月格式
            if (dto.InvYm.Length != 6 || !dto.InvYm.All(char.IsDigit))
            {
                throw new ArgumentException("發票年月格式錯誤，必須為YYYYMM格式");
            }

            electronicInvoice.InvoiceId = dto.InvoiceId;
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
            electronicInvoice.TransferType = dto.TransferType;
            electronicInvoice.TransferStatus = dto.TransferStatus;
            electronicInvoice.Status = dto.Status;
            electronicInvoice.UpdatedBy = GetCurrentUserId();
            electronicInvoice.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(electronicInvoice);
            _logger.LogInfo($"修改B2B電子發票成功: {dto.InvoiceId} (TKey: {dto.TKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改B2B電子發票失敗: {dto.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteB2BElectronicInvoiceAsync(long tKey)
    {
        try
        {
            var electronicInvoice = await _repository.GetByIdAsync(tKey);
            if (electronicInvoice == null)
            {
                throw new KeyNotFoundException($"B2B電子發票不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除B2B電子發票成功: {electronicInvoice.InvoiceId} (TKey: {tKey})");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除B2B電子發票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<B2BPrintDataDto> ManualPrintAsync(B2BManualPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"B2B電子發票手動取號列印: {string.Join(",", dto.TKeys)}");

            // 查詢選定的B2B電子發票
            var invoices = new List<B2BElectronicInvoice>();
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
                throw new InvalidOperationException("未找到選定的B2B電子發票");
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvoiceId", DisplayName = "發票編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PosId", DisplayName = "POS代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvYm", DisplayName = "發票年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Track", DisplayName = "字軌", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNoB", DisplayName = "發票號碼起", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNoE", DisplayName = "發票號碼迄", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrintCode", DisplayName = "列印條碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvoiceDate", DisplayName = "發票日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "PrizeType", DisplayName = "獎項類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeAmt", DisplayName = "獎項金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "TransferType", DisplayName = "傳輸類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "TransferStatus", DisplayName = "傳輸狀態", DataType = ExportDataType.String }
            };

            // 產生PDF
            var pdfBytes = _exportHelper.ExportToPdf(invoices, columns, "B2B電子發票列印");

            // 儲存檔案
            var reportId = $"B2BRPT{DateTime.Now:yyyyMMddHHmmss}";
            var fileName = $"B2B電子發票列印_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = await _fileStorageService.SaveFileAsync(pdfBytes, fileName, "B2BElectronicInvoices");

            return new B2BPrintDataDto
            {
                ReportId = reportId,
                FileUrl = $"/api/v1/b2b-electronic-invoices/download/{reportId}",
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("B2B電子發票手動取號列印失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<B2BElectronicInvoiceAwardDto>> GetAwardListAsync(B2BAwardListQueryDto query)
    {
        try
        {
            var repositoryQuery = new B2BAwardListQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                InvYm = query.InvYm,
                PrizeType = query.PrizeType,
                B2BFlag = query.B2BFlag ?? "Y"
            };

            var result = await _repository.QueryAwardListAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToAwardDto).ToList();

            return new PagedResult<B2BElectronicInvoiceAwardDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B中獎清冊失敗", ex);
            throw;
        }
    }

    public async Task<B2BPrintDataDto> AwardPrintAsync(B2BAwardPrintDto dto)
    {
        try
        {
            _logger.LogInfo($"B2B中獎清冊列印: InvYm={dto.InvYm}, PrizeType={dto.PrizeType}");

            // 查詢B2B中獎清冊資料
            var query = new B2BAwardListQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                InvYm = dto.InvYm,
                PrizeType = dto.PrizeType,
                B2BFlag = "Y"
            };

            var result = await GetAwardListAsync(query);

            if (result.Items.Count == 0)
            {
                throw new InvalidOperationException("未找到B2B中獎清冊資料");
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "InvYm", DisplayName = "發票年月", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Track", DisplayName = "字軌", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "InvNo", DisplayName = "發票號碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeType", DisplayName = "獎項類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PrizeAmt", DisplayName = "獎項金額", DataType = ExportDataType.Decimal },
                new ExportColumn { PropertyName = "AwardDate", DisplayName = "中獎日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "B2BFlag", DisplayName = "B2B標記", DataType = ExportDataType.String }
            };

            // 產生PDF
            var pdfBytes = _exportHelper.ExportToPdf(result.Items, columns, "B2B中獎清冊");

            // 儲存檔案
            var reportId = $"B2BAWARD{DateTime.Now:yyyyMMddHHmmss}";
            var fileName = $"B2B中獎清冊_{dto.InvYm ?? "ALL"}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            _logger.LogInfo($"B2B中獎清冊列印: InvYm={dto.InvYm}, PrizeType={dto.PrizeType}");

            return new B2BPrintDataDto
            {
                ReportId = reportId,
                FileUrl = $"/api/v1/b2b-electronic-invoices/award-download/{reportId}",
                FileName = fileName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("B2B中獎清冊列印失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private B2BElectronicInvoiceDto MapToDto(B2BElectronicInvoice electronicInvoice)
    {
        return new B2BElectronicInvoiceDto
        {
            TKey = electronicInvoice.TKey,
            InvoiceId = electronicInvoice.InvoiceId,
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
            B2BFlag = electronicInvoice.B2BFlag,
            TransferType = electronicInvoice.TransferType,
            TransferStatus = electronicInvoice.TransferStatus,
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
    private B2BElectronicInvoiceAwardDto MapToAwardDto(B2BElectronicInvoice electronicInvoice)
    {
        return new B2BElectronicInvoiceAwardDto
        {
            TKey = electronicInvoice.TKey,
            InvoiceId = electronicInvoice.InvoiceId,
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

