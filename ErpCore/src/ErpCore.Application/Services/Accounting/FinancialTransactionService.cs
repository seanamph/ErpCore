using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Accounting;
using ErpCore.Infrastructure.Repositories.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Accounting;

/// <summary>
/// 財務交易服務實作 (SYSN210)
/// </summary>
public class FinancialTransactionService : BaseService, IFinancialTransactionService
{
    private readonly IFinancialTransactionRepository _repository;

    public FinancialTransactionService(
        IFinancialTransactionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<FinancialTransactionDto>> GetFinancialTransactionsAsync(FinancialTransactionQueryDto query)
    {
        try
        {
            var repositoryQuery = new FinancialTransactionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                TxnNo = query.TxnNo,
                TxnDateFrom = query.TxnDateFrom,
                TxnDateTo = query.TxnDateTo,
                TxnType = query.TxnType,
                StypeId = query.StypeId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<FinancialTransactionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢財務交易列表失敗", ex);
            throw;
        }
    }

    public async Task<FinancialTransactionDto> GetFinancialTransactionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<FinancialTransactionDto> GetFinancialTransactionByTxnNoAsync(string txnNo)
    {
        try
        {
            var entity = await _repository.GetByTxnNoAsync(txnNo);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {txnNo}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢財務交易失敗: {txnNo}", ex);
            throw;
        }
    }

    public async Task<long> CreateFinancialTransactionAsync(CreateFinancialTransactionDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.TxnNo))
            {
                throw new ArgumentException("交易單號不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.StypeId))
            {
                throw new ArgumentException("會計科目不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.Dc))
            {
                throw new ArgumentException("借貸方向不能為空");
            }

            // 檢查交易單號是否已存在
            var exists = await _repository.ExistsAsync(dto.TxnNo);
            if (exists)
            {
                throw new InvalidOperationException($"交易單號已存在: {dto.TxnNo}");
            }

            var entity = new FinancialTransaction
            {
                TxnNo = dto.TxnNo,
                TxnDate = dto.TxnDate,
                TxnType = dto.TxnType,
                StypeId = dto.StypeId,
                Dc = dto.Dc,
                Amount = dto.Amount,
                Description = dto.Description,
                Status = "DRAFT",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增財務交易成功: {result.TxnNo}");

            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增財務交易失敗: {dto.TxnNo}", ex);
            throw;
        }
    }

    public async Task UpdateFinancialTransactionAsync(long tKey, UpdateFinancialTransactionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {tKey}");
            }

            // 僅允許修改草稿狀態的交易
            if (entity.Status != "DRAFT")
            {
                throw new InvalidOperationException("僅能修改草稿狀態的交易");
            }

            entity.TxnDate = dto.TxnDate;
            entity.TxnType = dto.TxnType;
            entity.StypeId = dto.StypeId;
            entity.Dc = dto.Dc;
            entity.Amount = dto.Amount;
            entity.Description = dto.Description;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改財務交易成功: {entity.TxnNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteFinancialTransactionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {tKey}");
            }

            // 僅允許刪除草稿狀態的交易
            if (entity.Status != "DRAFT")
            {
                throw new InvalidOperationException("僅能刪除草稿狀態的交易");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除財務交易成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task ConfirmFinancialTransactionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {tKey}");
            }

            if (entity.Status != "DRAFT")
            {
                throw new InvalidOperationException("僅能確認草稿狀態的交易");
            }

            entity.Status = "CONFIRMED";
            entity.Verifier = GetCurrentUserId();
            entity.VerifyDate = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"確認財務交易成功: {entity.TxnNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task PostFinancialTransactionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"財務交易不存在: {tKey}");
            }

            if (entity.Status != "CONFIRMED")
            {
                throw new InvalidOperationException("僅能過帳已確認的交易");
            }

            entity.Status = "POSTED";
            entity.PostedBy = GetCurrentUserId();
            entity.PostedDate = DateTime.Now;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"過帳財務交易成功: {entity.TxnNo}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"過帳財務交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BalanceCheckDto> CheckBalanceAsync(List<CreateFinancialTransactionDto> transactions)
    {
        try
        {
            var entities = transactions.Select(dto => new FinancialTransaction
            {
                Dc = dto.Dc,
                Amount = dto.Amount
            }).ToList();

            var result = await _repository.CheckBalanceAsync(entities);

            return new BalanceCheckDto
            {
                IsBalanced = result.IsBalanced,
                DebitTotal = result.DebitTotal,
                CreditTotal = result.CreditTotal,
                Difference = result.Difference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查借貸平衡失敗", ex);
            throw;
        }
    }

    private FinancialTransactionDto MapToDto(FinancialTransaction entity)
    {
        return new FinancialTransactionDto
        {
            TKey = entity.TKey,
            TxnNo = entity.TxnNo,
            TxnDate = entity.TxnDate,
            TxnType = entity.TxnType,
            StypeId = entity.StypeId,
            Dc = entity.Dc,
            Amount = entity.Amount,
            Description = entity.Description,
            Status = entity.Status,
            Verifier = entity.Verifier,
            VerifyDate = entity.VerifyDate,
            PostedBy = entity.PostedBy,
            PostedDate = entity.PostedDate,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

