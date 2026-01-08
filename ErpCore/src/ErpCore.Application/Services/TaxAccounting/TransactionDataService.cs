using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Repositories.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 交易資料處理服務實作 (SYST221, SYST311-SYST352)
/// </summary>
public class TransactionDataService : BaseService, ITransactionDataService
{
    private readonly ITransactionDataRepository _repository;
    private readonly IInvoiceDataRepository _invoiceDataRepository;

    public TransactionDataService(
        ITransactionDataRepository repository,
        IInvoiceDataRepository invoiceDataRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _invoiceDataRepository = invoiceDataRepository;
    }

    public async Task<PagedResult<InvoiceVoucherDto>> GetConfirmVouchersAsync(VoucherConfirmQueryDto query)
    {
        try
        {
            var repositoryQuery = new VoucherConfirmQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                VoucherNoFrom = query.VoucherNoFrom,
                VoucherNoTo = query.VoucherNoTo,
                VoucherDateFrom = query.VoucherDateFrom,
                VoucherDateTo = query.VoucherDateTo,
                VoucherTypes = query.VoucherTypes,
                VoucherStatus = query.VoucherStatus ?? "1",
                ConfirmDateFrom = query.ConfirmDateFrom,
                ConfirmDateTo = query.ConfirmDateTo
            };

            var result = await _repository.GetConfirmVouchersAsync(repositoryQuery);

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
            _logger.LogError("查詢傳票確認列表失敗", ex);
            throw;
        }
    }

    public async Task<BatchConfirmResultDto> BatchConfirmVouchersAsync(BatchConfirmVoucherDto dto)
    {
        try
        {
            var successCount = await _repository.BatchConfirmVouchersAsync(
                dto.VoucherNos,
                dto.ConfirmDate,
                GetCurrentUserId());

            return new BatchConfirmResultDto
            {
                SuccessCount = successCount,
                FailCount = dto.VoucherNos.Count - successCount,
                FailItems = new List<BatchConfirmFailItem>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次確認傳票失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<InvoiceVoucherDto>> GetPostingVouchersAsync(VoucherPostingQueryDto query)
    {
        try
        {
            var repositoryQuery = new VoucherPostingQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PostingYearMonth = query.PostingYearMonth,
                VoucherDateFrom = query.VoucherDateFrom,
                VoucherDateTo = query.VoucherDateTo,
                VoucherTypes = query.VoucherTypes,
                VoucherStatus = query.VoucherStatus ?? "2",
                PostingByDetail = query.PostingByDetail
            };

            var result = await _repository.GetPostingVouchersAsync(repositoryQuery);

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
            _logger.LogError("查詢傳票過帳列表失敗", ex);
            throw;
        }
    }

    public async Task<BatchPostingResultDto> BatchPostingVouchersAsync(BatchPostingVoucherDto dto)
    {
        try
        {
            if (dto.VoucherNos == null || dto.VoucherNos.Count == 0)
            {
                throw new ArgumentException("傳票編號列表不能為空");
            }

            var successCount = await _repository.BatchPostingVouchersAsync(
                dto.VoucherNos,
                dto.PostingYearMonth,
                dto.PostingDate,
                GetCurrentUserId());

            return new BatchPostingResultDto
            {
                SuccessCount = successCount,
                FailCount = dto.VoucherNos.Count - successCount,
                FailItems = new List<BatchPostingFailItem>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批次過帳傳票失敗", ex);
            throw;
        }
    }

    public async Task<VoucherStatusCountDto> GetVoucherStatusCountAsync(string postingYearMonth)
    {
        try
        {
            var result = await _repository.GetVoucherStatusCountAsync(postingYearMonth);

            return new VoucherStatusCountDto
            {
                PostingYearMonth = result.PostingYearMonth,
                CreateCount = result.CreateCount,
                ConfirmCount = result.ConfirmCount,
                PostingCount = result.PostingCount,
                CreateAmount = result.CreateAmount,
                ConfirmAmount = result.ConfirmAmount,
                PostingAmount = result.PostingAmount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票狀態統計失敗: {postingYearMonth}", ex);
            throw;
        }
    }

    public async Task<PagedResult<InvoiceVoucherDto>> QueryVouchersAsync(InvoiceVoucherQueryDto query)
    {
        try
        {
            // 重用 InvoiceDataService 的查詢邏輯
            var invoiceDataService = new InvoiceDataService(_invoiceDataRepository, _logger, _userContext);
            return await invoiceDataService.GetVouchersAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<InvoiceVoucherDto>> GetReverseYearEndVouchersAsync(ReverseYearEndQueryDto query)
    {
        try
        {
            var repositoryQuery = new ReverseYearEndQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                Year = query.Year
            };

            var result = await _repository.GetReverseYearEndVouchersAsync(repositoryQuery);

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
            _logger.LogError("查詢反過帳資料年結處理失敗", ex);
            throw;
        }
    }

    public async Task ReversePostingVoucherAsync(string voucherId, ReversePostingDto dto)
    {
        try
        {
            await _repository.ReversePostingVoucherAsync(
                voucherId,
                dto.ReversePostingDate,
                GetCurrentUserId());

            _logger.LogInfo($"反過帳傳票成功: {voucherId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"反過帳傳票失敗: {voucherId}", ex);
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

