using ErpCore.Application.DTOs.Pos;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Pos;
using ErpCore.Infrastructure.Repositories.Pos;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Pos;

/// <summary>
/// POS交易服務實作
/// </summary>
public class PosTransactionService : BaseService, IPosTransactionService
{
    private readonly IPosTransactionRepository _transactionRepository;
    private readonly IPosTransactionDetailRepository _detailRepository;

    public PosTransactionService(
        IPosTransactionRepository transactionRepository,
        IPosTransactionDetailRepository detailRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _transactionRepository = transactionRepository;
        _detailRepository = detailRepository;
    }

    public async Task<PagedResult<PosTransactionDto>> GetPosTransactionsAsync(PosTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new PosTransactionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                TransactionId = query.TransactionId,
                StoreId = query.StoreId,
                PosId = query.PosId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                TransactionType = query.TransactionType,
                Status = query.Status
            };

            var result = await _transactionRepository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new PosTransactionDto
            {
                Id = x.Id,
                TransactionId = x.TransactionId,
                StoreId = x.StoreId,
                PosId = x.PosId,
                TransactionDate = x.TransactionDate,
                TransactionType = x.TransactionType,
                TotalAmount = x.TotalAmount,
                PaymentMethod = x.PaymentMethod,
                CustomerId = x.CustomerId,
                Status = x.Status,
                SyncAt = x.SyncAt,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PosTransactionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS交易列表失敗", ex);
            throw;
        }
    }

    public async Task<PosTransactionDto?> GetPosTransactionByIdAsync(string transactionId)
    {
        try
        {
            var transaction = await _transactionRepository.GetByTransactionIdAsync(transactionId);
            if (transaction == null)
            {
                return null;
            }

            var details = await _detailRepository.GetByTransactionIdAsync(transactionId);

            return new PosTransactionDto
            {
                Id = transaction.Id,
                TransactionId = transaction.TransactionId,
                StoreId = transaction.StoreId,
                PosId = transaction.PosId,
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType,
                TotalAmount = transaction.TotalAmount,
                PaymentMethod = transaction.PaymentMethod,
                CustomerId = transaction.CustomerId,
                Status = transaction.Status,
                SyncAt = transaction.SyncAt,
                ErrorMessage = transaction.ErrorMessage,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt,
                Details = details.Select(d => new PosTransactionDetailDto
                {
                    Id = d.Id,
                    TransactionId = d.TransactionId,
                    LineNo = d.LineNo,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount,
                    Discount = d.Discount,
                    CreatedAt = d.CreatedAt
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS交易失敗: {transactionId}", ex);
            throw;
        }
    }

    public async Task<PosSyncResultDto> SyncTransactionsAsync(PosSyncRequestDto request)
    {
        try
        {
            // 這裡應該實作實際的同步邏輯
            // 目前僅返回基本結構
            _logger.LogInfo($"開始同步POS交易資料: StoreId={request.StoreId}, StartDate={request.StartDate}, EndDate={request.EndDate}");

            return new PosSyncResultDto
            {
                SyncLogId = 0,
                ProcessedCount = 0,
                SuccessCount = 0,
                FailedCount = 0,
                Status = "Completed"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("同步POS交易資料失敗", ex);
            throw;
        }
    }
}

