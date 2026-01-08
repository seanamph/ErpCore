using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 銀行帳戶服務實作
/// </summary>
public class BankAccountService : BaseService, IBankAccountService
{
    private readonly IBankAccountRepository _repository;

    public BankAccountService(
        IBankAccountRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BankAccountDto>> GetBankAccountsAsync(BankAccountQueryDto query)
    {
        try
        {
            var repositoryQuery = new BankAccountQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                BankAccountId = query.BankAccountId,
                BankId = query.BankId,
                AccountName = query.AccountName,
                AccountNumber = query.AccountNumber,
                AccountType = query.AccountType,
                CurrencyId = query.CurrencyId,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BankAccountDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銀行帳戶列表失敗", ex);
            throw;
        }
    }

    public async Task<BankAccountDto> GetBankAccountByIdAsync(string bankAccountId)
    {
        try
        {
            var bankAccount = await _repository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"銀行帳戶不存在: {bankAccountId}");
            }

            return MapToDto(bankAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行帳戶失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<string> CreateBankAccountAsync(CreateBankAccountDto dto)
    {
        try
        {
            // 檢查銀行帳戶編號是否已存在
            if (await _repository.ExistsAsync(dto.BankAccountId))
            {
                throw new InvalidOperationException($"銀行帳戶編號已存在: {dto.BankAccountId}");
            }

            // 檢查帳戶號碼是否已存在
            if (!string.IsNullOrEmpty(dto.AccountNumber) && await _repository.ExistsByAccountNumberAsync(dto.AccountNumber))
            {
                throw new InvalidOperationException($"帳戶號碼已存在: {dto.AccountNumber}");
            }

            var bankAccount = new BankAccount
            {
                BankAccountId = dto.BankAccountId,
                BankId = dto.BankId,
                AccountName = dto.AccountName,
                AccountNumber = dto.AccountNumber,
                AccountType = dto.AccountType,
                CurrencyId = dto.CurrencyId ?? "TWD",
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(bankAccount);

            _logger.LogInfo($"新增銀行帳戶成功: {dto.BankAccountId}");
            return bankAccount.BankAccountId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銀行帳戶失敗: {dto.BankAccountId}", ex);
            throw;
        }
    }

    public async Task UpdateBankAccountAsync(string bankAccountId, UpdateBankAccountDto dto)
    {
        try
        {
            var bankAccount = await _repository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"銀行帳戶不存在: {bankAccountId}");
            }

            // 檢查帳戶號碼是否已被其他帳戶使用
            if (!string.IsNullOrEmpty(dto.AccountNumber) && 
                dto.AccountNumber != bankAccount.AccountNumber &&
                await _repository.ExistsByAccountNumberAsync(dto.AccountNumber))
            {
                throw new InvalidOperationException($"帳戶號碼已被使用: {dto.AccountNumber}");
            }

            bankAccount.BankId = dto.BankId;
            bankAccount.AccountName = dto.AccountName;
            bankAccount.AccountNumber = dto.AccountNumber;
            bankAccount.AccountType = dto.AccountType;
            bankAccount.CurrencyId = dto.CurrencyId ?? "TWD";
            bankAccount.Status = dto.Status;
            bankAccount.Memo = dto.Memo;
            bankAccount.UpdatedBy = GetCurrentUserId();
            bankAccount.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(bankAccount);

            _logger.LogInfo($"修改銀行帳戶成功: {bankAccountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銀行帳戶失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task DeleteBankAccountAsync(string bankAccountId)
    {
        try
        {
            var bankAccount = await _repository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"銀行帳戶不存在: {bankAccountId}");
            }

            // TODO: 檢查是否有相關付款單使用此帳戶

            await _repository.DeleteAsync(bankAccountId);

            _logger.LogInfo($"刪除銀行帳戶成功: {bankAccountId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銀行帳戶失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string bankAccountId, string status)
    {
        try
        {
            var bankAccount = await _repository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"銀行帳戶不存在: {bankAccountId}");
            }

            bankAccount.Status = status;
            bankAccount.UpdatedBy = GetCurrentUserId();
            bankAccount.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(bankAccount);

            _logger.LogInfo($"更新銀行帳戶狀態成功: {bankAccountId} -> {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新銀行帳戶狀態失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<BankAccountBalanceDto> GetBalanceAsync(string bankAccountId)
    {
        try
        {
            var bankAccount = await _repository.GetByIdAsync(bankAccountId);
            if (bankAccount == null)
            {
                throw new InvalidOperationException($"銀行帳戶不存在: {bankAccountId}");
            }

            // TODO: 從付款單等相關表計算實際餘額
            // 目前先返回 0，實際應從相關交易記錄計算
            return new BankAccountBalanceDto
            {
                BankAccountId = bankAccount.BankAccountId,
                AccountName = bankAccount.AccountName,
                Balance = 0, // TODO: 計算實際餘額
                CurrencyId = bankAccount.CurrencyId ?? "TWD",
                LastUpdateDate = bankAccount.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行帳戶餘額失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string bankAccountId)
    {
        try
        {
            return await _repository.ExistsAsync(bankAccountId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銀行帳戶是否存在失敗: {bankAccountId}", ex);
            throw;
        }
    }

    private BankAccountDto MapToDto(BankAccount bankAccount)
    {
        return new BankAccountDto
        {
            TKey = bankAccount.TKey,
            BankAccountId = bankAccount.BankAccountId,
            BankId = bankAccount.BankId,
            AccountName = bankAccount.AccountName,
            AccountNumber = bankAccount.AccountNumber,
            AccountType = bankAccount.AccountType,
            CurrencyId = bankAccount.CurrencyId,
            Status = bankAccount.Status,
            Memo = bankAccount.Memo,
            CreatedBy = bankAccount.CreatedBy,
            CreatedAt = bankAccount.CreatedAt,
            UpdatedBy = bankAccount.UpdatedBy,
            UpdatedAt = bankAccount.UpdatedAt
        };
    }
}

