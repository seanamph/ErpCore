using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Repositories.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 發票列印服務實作 (SYS2000 - 發票列印作業)
/// </summary>
public class InvoicePrintService : BaseService, IInvoicePrintService
{
    private readonly IInvoicePrintRepository _repository;

    public InvoicePrintService(
        IInvoicePrintRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<InvoiceDto>> GetInvoicesAsync(InvoiceQueryDto query)
    {
        try
        {
            var repositoryQuery = new InvoiceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                InvoiceNo = query.InvoiceNo,
                CustomerId = query.CustomerId,
                InvoiceDateFrom = query.InvoiceDateFrom,
                InvoiceDateTo = query.InvoiceDateTo,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = new List<InvoiceDto>();
            foreach (var item in result.Items)
            {
                var details = await _repository.GetDetailsByInvoiceNoAsync(item.InvoiceNo);
                dtos.Add(MapToDto(item, details));
            }

            return new PagedResult<InvoiceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢發票列表失敗", ex);
            throw;
        }
    }

    public async Task<InvoiceDto> GetInvoiceByIdAsync(string invoiceNo)
    {
        try
        {
            var invoice = await _repository.GetByIdAsync(invoiceNo);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"發票不存在: {invoiceNo}");
            }

            var details = await _repository.GetDetailsByInvoiceNoAsync(invoiceNo);
            return MapToDto(invoice, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢發票失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task PrintInvoiceAsync(string invoiceNo, InvoicePrintRequestDto request)
    {
        try
        {
            var invoice = await _repository.GetByIdAsync(invoiceNo);
            if (invoice == null)
            {
                throw new KeyNotFoundException($"發票不存在: {invoiceNo}");
            }

            // 記錄列印記錄
            var printLog = new InvoicePrintLog
            {
                InvoiceNo = invoiceNo,
                PrintUser = GetCurrentUserId(),
                PrintFormat = request.PrintFormat,
                PrintType = "NORMAL",
                PrintCount = 1,
                CreatedBy = GetCurrentUserId()
            };

            await _repository.CreatePrintLogAsync(printLog);

            // 更新發票列印資訊
            await _repository.UpdatePrintInfoAsync(invoiceNo, request.PrintFormat, GetCurrentUserId());

            _logger.LogInfo($"列印發票成功: {invoiceNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"列印發票失敗: {invoiceNo}", ex);
            throw;
        }
    }

    public async Task BatchPrintInvoicesAsync(BatchPrintInvoiceDto dto)
    {
        try
        {
            foreach (var invoiceNo in dto.InvoiceNos)
            {
                var request = new InvoicePrintRequestDto
                {
                    PrintFormat = dto.PrintFormat,
                    IncludeCover = dto.IncludeCover
                };
                await PrintInvoiceAsync(invoiceNo, request);
            }

            _logger.LogInfo($"批次列印發票成功: {dto.InvoiceNos.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次列印發票失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<InvoicePrintLogDto>> GetPrintLogsAsync(string invoiceNo)
    {
        try
        {
            var logs = await _repository.GetPrintLogsByInvoiceNoAsync(invoiceNo);
            return logs.Select(x => new InvoicePrintLogDto
            {
                TKey = x.TKey,
                InvoiceNo = x.InvoiceNo,
                PrintDate = x.PrintDate,
                PrintUser = x.PrintUser,
                PrintFormat = x.PrintFormat,
                PrintType = x.PrintType,
                PrinterName = x.PrinterName,
                PrintCount = x.PrintCount,
                Memo = x.Memo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢列印記錄失敗: {invoiceNo}", ex);
            throw;
        }
    }

    private InvoiceDto MapToDto(Invoice invoice, IEnumerable<InvoiceDetail> details)
    {
        return new InvoiceDto
        {
            TKey = invoice.TKey,
            InvoiceNo = invoice.InvoiceNo,
            InvoiceType = invoice.InvoiceType,
            InvoiceDate = invoice.InvoiceDate,
            CustomerId = invoice.CustomerId,
            StoreId = invoice.StoreId,
            TotalAmount = invoice.TotalAmount,
            TaxAmount = invoice.TaxAmount,
            Amount = invoice.Amount,
            CurrencyId = invoice.CurrencyId,
            Status = invoice.Status,
            PrintCount = invoice.PrintCount,
            LastPrintDate = invoice.LastPrintDate,
            LastPrintUser = invoice.LastPrintUser,
            PrintFormat = invoice.PrintFormat,
            Memo = invoice.Memo,
            Details = details.Select(x => new InvoiceDetailDto
            {
                TKey = x.TKey,
                InvoiceNo = x.InvoiceNo,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                GoodsName = x.GoodsName,
                Qty = x.Qty,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                TaxRate = x.TaxRate,
                TaxAmount = x.TaxAmount,
                UnitId = x.UnitId,
                Memo = x.Memo
            }).ToList()
        };
    }
}

