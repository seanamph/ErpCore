using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 稅務報表查詢服務實作 (SYST411-SYST452)
/// </summary>
public class TaxReportService : BaseService, ITaxReportService
{
    private readonly ITaxReportRepository _repository;
    private readonly ExportHelper _exportHelper;

    public TaxReportService(
        ITaxReportRepository repository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<InvoiceVoucherDto>> GetVouchersAsync(TaxReportVoucherQueryDto query)
    {
        try
        {
            var repositoryQuery = new TaxReportVoucherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                DateFrom = query.DateFrom,
                DateTo = query.DateTo,
                VoucherIdFrom = query.VoucherIdFrom,
                VoucherIdTo = query.VoucherIdTo,
                VoucherKinds = query.VoucherKinds,
                VoucherStatuses = query.VoucherStatuses,
                CreatedBy = query.CreatedBy,
                CreatedDateFrom = query.CreatedDateFrom,
                CreatedDateTo = query.CreatedDateTo,
                SiteId = query.SiteId
            };

            var result = await _repository.GetVouchersAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToInvoiceVoucherDto(x)).ToList();

            return new PagedResult<InvoiceVoucherDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<List<VoucherDetailDto>> GetVoucherDetailsAsync(string voucherId)
    {
        try
        {
            var details = await _repository.GetVoucherDetailsAsync(voucherId);

            return details.Select(x => new VoucherDetailDto
            {
                TKey = x.TKey,
                VoucherId = x.VoucherId,
                SeqNo = x.SeqNo,
                StypeId = x.StypeId,
                Dc = x.Dc,
                Amount = x.Amount,
                Description = x.Description
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票明細失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<object> GetFinancialReportsAsync(FinancialReportQueryDto query)
    {
        try
        {
            // TODO: 實作財務報表查詢邏輯
            await Task.CompletedTask;
            return new { };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢財務報表失敗", ex);
            throw;
        }
    }

    public async Task<object> GetTaxStatisticsAsync(TaxStatisticsQueryDto query)
    {
        try
        {
            // TODO: 實作稅務統計報表查詢邏輯
            await Task.CompletedTask;
            return new { };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢稅務統計報表失敗", ex);
            throw;
        }
    }

    public async Task<PrintResultDto> PrintVouchersAsync(PrintVoucherDto dto)
    {
        try
        {
            _logger.LogInfo($"列印傳票: {string.Join(",", dto.VoucherIds)}");

            // 查詢傳票資料
            var vouchers = new List<InvoiceVoucherDto>();
            foreach (var voucherId in dto.VoucherIds)
            {
                var query = new TaxReportVoucherQueryDto
                {
                    PageIndex = 1,
                    PageSize = 1,
                    VoucherIdFrom = voucherId,
                    VoucherIdTo = voucherId
                };

                var result = await GetVouchersAsync(query);
                if (result.Items.Any())
                {
                    vouchers.Add(result.Items.First());
                }
            }

            if (!vouchers.Any())
            {
                throw new InvalidOperationException("找不到要列印的傳票");
            }

            // 定義列印欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "VoucherId", DisplayName = "傳票編號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VoucherDate", DisplayName = "傳票日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "VoucherKind", DisplayName = "傳票種類", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorId", DisplayName = "廠商代號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "VendorName", DisplayName = "廠商名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String }
            };

            // 產生 PDF
            var pdfBytes = _exportHelper.ExportToPdf(vouchers, columns, "傳票列印");

            // 儲存檔案（實際應儲存到檔案系統或雲端儲存）
            var fileName = $"傳票列印_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine("temp", "prints", fileName);

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            _logger.LogInfo($"傳票列印完成: {fileName}");

            return new PrintResultDto
            {
                FileName = fileName,
                FileUrl = $"/api/v1/tax-accounting/tax-reports/download/{fileName}",
                FileSize = pdfBytes.Length
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("列印傳票失敗", ex);
            throw;
        }
    }

    #region 私有方法

    private InvoiceVoucherDto MapToInvoiceVoucherDto(Voucher entity)
    {
        return new InvoiceVoucherDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherDate = entity.VoucherDate,
            VoucherKind = entity.VoucherKind,
            VoucherStatus = entity.VoucherStatus,
            Notes = entity.Description,
            InvYn = entity.InvYn,
            TypeId = entity.VoucherTypeId,
            SiteId = entity.SiteId,
            VendorId = entity.VendorId,
            VendorName = entity.VendorName,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    #endregion
}

