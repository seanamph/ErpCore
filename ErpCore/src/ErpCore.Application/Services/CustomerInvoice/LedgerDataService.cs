using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Repositories.CustomerInvoice;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 總帳資料服務實作 (SYS2000 - 總帳資料維護)
/// </summary>
public class LedgerDataService : BaseService, ILedgerDataService
{
    private readonly ILedgerDataRepository _repository;

    public LedgerDataService(
        ILedgerDataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GeneralLedgerDto>> GetGeneralLedgersAsync(GeneralLedgerQueryDto query)
    {
        try
        {
            var repositoryQuery = new GeneralLedgerQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                LedgerId = query.LedgerId,
                AccountId = query.AccountId,
                Period = query.Period,
                LedgerDateFrom = query.LedgerDateFrom,
                LedgerDateTo = query.LedgerDateTo,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<GeneralLedgerDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢總帳列表失敗", ex);
            throw;
        }
    }

    public async Task<GeneralLedgerDto> GetGeneralLedgerByIdAsync(string ledgerId)
    {
        try
        {
            var ledger = await _repository.GetByIdAsync(ledgerId);
            if (ledger == null)
            {
                throw new KeyNotFoundException($"總帳不存在: {ledgerId}");
            }

            return MapToDto(ledger);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢總帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task<string> CreateGeneralLedgerAsync(CreateGeneralLedgerDto dto)
    {
        try
        {
            var ledgerId = $"GL{DateTime.Now:yyyyMMddHHmmss}";
            var period = dto.LedgerDate.ToString("yyyyMM");

            var ledger = new GeneralLedger
            {
                LedgerId = ledgerId,
                LedgerDate = dto.LedgerDate,
                AccountId = dto.AccountId,
                VoucherNo = dto.VoucherNo,
                Description = dto.Description,
                DebitAmount = dto.DebitAmount,
                CreditAmount = dto.CreditAmount,
                Balance = dto.DebitAmount - dto.CreditAmount,
                CurrencyId = dto.CurrencyId,
                Period = period,
                Status = "DRAFT",
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            await _repository.CreateAsync(ledger);

            _logger.LogInfo($"新增總帳成功: {ledgerId}");
            return ledgerId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增總帳失敗", ex);
            throw;
        }
    }

    public async Task UpdateGeneralLedgerAsync(string ledgerId, UpdateGeneralLedgerDto dto)
    {
        try
        {
            var ledger = await _repository.GetByIdAsync(ledgerId);
            if (ledger == null)
            {
                throw new KeyNotFoundException($"總帳不存在: {ledgerId}");
            }

            if (ledger.Status != "DRAFT")
            {
                throw new InvalidOperationException("僅草稿狀態的總帳可修改");
            }

            var period = dto.LedgerDate.ToString("yyyyMM");

            ledger.LedgerDate = dto.LedgerDate;
            ledger.AccountId = dto.AccountId;
            ledger.VoucherNo = dto.VoucherNo;
            ledger.Description = dto.Description;
            ledger.DebitAmount = dto.DebitAmount;
            ledger.CreditAmount = dto.CreditAmount;
            ledger.Balance = dto.DebitAmount - dto.CreditAmount;
            ledger.CurrencyId = dto.CurrencyId;
            ledger.Period = period;
            ledger.Memo = dto.Memo;
            ledger.UpdatedBy = GetCurrentUserId();

            await _repository.UpdateAsync(ledger);

            _logger.LogInfo($"修改總帳成功: {ledgerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改總帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task DeleteGeneralLedgerAsync(string ledgerId)
    {
        try
        {
            var ledger = await _repository.GetByIdAsync(ledgerId);
            if (ledger == null)
            {
                throw new KeyNotFoundException($"總帳不存在: {ledgerId}");
            }

            if (ledger.Status != "DRAFT")
            {
                throw new InvalidOperationException("僅草稿狀態的總帳可刪除");
            }

            await _repository.DeleteAsync(ledgerId);

            _logger.LogInfo($"刪除總帳成功: {ledgerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除總帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task PostLedgerAsync(string ledgerId)
    {
        try
        {
            var ledger = await _repository.GetByIdAsync(ledgerId);
            if (ledger == null)
            {
                throw new KeyNotFoundException($"總帳不存在: {ledgerId}");
            }

            if (ledger.Status == "POSTED")
            {
                throw new InvalidOperationException("總帳已過帳");
            }

            // 更新科目餘額
            var balance = await _repository.GetAccountBalanceAsync(ledger.AccountId, ledger.Period);
            if (balance == null)
            {
                balance = new AccountBalance
                {
                    AccountId = ledger.AccountId,
                    Period = ledger.Period,
                    OpeningBalance = 0,
                    DebitAmount = 0,
                    CreditAmount = 0,
                    ClosingBalance = 0,
                    CreatedBy = GetCurrentUserId(),
                    UpdatedBy = GetCurrentUserId()
                };
            }

            balance.DebitAmount += ledger.DebitAmount;
            balance.CreditAmount += ledger.CreditAmount;
            balance.ClosingBalance = balance.OpeningBalance + balance.DebitAmount - balance.CreditAmount;
            balance.UpdatedBy = GetCurrentUserId();

            await _repository.UpdateAccountBalanceAsync(balance);

            // 更新總帳狀態
            await _repository.UpdateStatusAsync(ledgerId, "POSTED");

            _logger.LogInfo($"過帳成功: {ledgerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"過帳失敗: {ledgerId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AccountBalanceDto>> GetAccountBalancesAsync(AccountBalanceQueryDto query)
    {
        try
        {
            var repositoryQuery = new AccountBalanceQuery
            {
                AccountId = query.AccountId,
                Period = query.Period,
                PeriodFrom = query.PeriodFrom,
                PeriodTo = query.PeriodTo
            };

            var balances = await _repository.GetAccountBalancesAsync(repositoryQuery);

            return balances.Select(x => new AccountBalanceDto
            {
                AccountId = x.AccountId,
                Period = x.Period,
                OpeningBalance = x.OpeningBalance,
                DebitAmount = x.DebitAmount,
                CreditAmount = x.CreditAmount,
                ClosingBalance = x.ClosingBalance
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢科目餘額失敗", ex);
            throw;
        }
    }

    private GeneralLedgerDto MapToDto(GeneralLedger ledger)
    {
        return new GeneralLedgerDto
        {
            TKey = ledger.TKey,
            LedgerId = ledger.LedgerId,
            LedgerDate = ledger.LedgerDate,
            AccountId = ledger.AccountId,
            VoucherNo = ledger.VoucherNo,
            Description = ledger.Description,
            DebitAmount = ledger.DebitAmount,
            CreditAmount = ledger.CreditAmount,
            Balance = ledger.Balance,
            CurrencyId = ledger.CurrencyId,
            Period = ledger.Period,
            Status = ledger.Status,
            Memo = ledger.Memo
        };
    }
}

