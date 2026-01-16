using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 銀行帳戶服務實作 (銀行帳戶維護)
/// </summary>
public class BankAccountService : BaseService, IBankAccountService
{
    private readonly IBankAccountRepository _repository;
    private readonly IBankRepository _bankRepository;

    public BankAccountService(
        IBankAccountRepository repository,
        IBankRepository bankRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _bankRepository = bankRepository;
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
                Status = query.Status,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            // 獲取所有銀行ID並查詢銀行名稱
            var bankIds = items.Select(x => x.BankId).Distinct().ToList();
            var bankNameMap = new Dictionary<string, string>();
            foreach (var bankId in bankIds)
            {
                var bank = await _bankRepository.GetByIdAsync(bankId);
                if (bank != null)
                {
                    bankNameMap[bankId] = bank.BankName;
                }
            }

            var dtos = items.Select(x => MapToDto(x, bankNameMap.GetValueOrDefault(x.BankId))).ToList();

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

            // 獲取銀行名稱
            string? bankName = null;
            if (!string.IsNullOrEmpty(bankAccount.BankId))
            {
                var bank = await _bankRepository.GetByIdAsync(bankAccount.BankId);
                bankName = bank?.BankName;
            }

            return MapToDto(bankAccount, bankName);
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
            if (await _repository.ExistsByAccountNumberAsync(dto.AccountNumber))
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
                Balance = dto.Balance ?? 0,
                OpeningDate = dto.OpeningDate,
                ClosingDate = dto.ClosingDate,
                ContactPerson = dto.ContactPerson,
                ContactPhone = dto.ContactPhone,
                ContactEmail = dto.ContactEmail,
                BranchName = dto.BranchName,
                BranchCode = dto.BranchCode,
                SwiftCode = dto.SwiftCode,
                Notes = dto.Notes,
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
            var existingAccount = await _repository.GetByIdAsync(bankAccountId);
            if (existingAccount != null && existingAccount.AccountNumber != dto.AccountNumber)
            {
                if (await _repository.ExistsByAccountNumberAsync(dto.AccountNumber))
                {
                    throw new InvalidOperationException($"帳戶號碼已被使用: {dto.AccountNumber}");
                }
            }

            bankAccount.BankId = dto.BankId;
            bankAccount.AccountName = dto.AccountName;
            bankAccount.AccountNumber = dto.AccountNumber;
            bankAccount.AccountType = dto.AccountType;
            bankAccount.CurrencyId = dto.CurrencyId ?? "TWD";
            bankAccount.Status = dto.Status;
            bankAccount.Balance = dto.Balance ?? 0;
            bankAccount.OpeningDate = dto.OpeningDate;
            bankAccount.ClosingDate = dto.ClosingDate;
            bankAccount.ContactPerson = dto.ContactPerson;
            bankAccount.ContactPhone = dto.ContactPhone;
            bankAccount.ContactEmail = dto.ContactEmail;
            bankAccount.BranchName = dto.BranchName;
            bankAccount.BranchCode = dto.BranchCode;
            bankAccount.SwiftCode = dto.SwiftCode;
            bankAccount.Notes = dto.Notes;
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

            // TODO: 檢查是否有相關付款單，如果有則不允許刪除

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

            _logger.LogInfo($"更新銀行帳戶狀態成功: {bankAccountId}, 狀態: {status}");
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

            var balance = await _repository.GetBalanceAsync(bankAccountId);

            return new BankAccountBalanceDto
            {
                BankAccountId = bankAccount.BankAccountId,
                AccountName = bankAccount.AccountName,
                Balance = balance,
                CurrencyId = bankAccount.CurrencyId,
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

    private BankAccountDto MapToDto(BankAccount bankAccount, string? bankName = null)
    {
        return new BankAccountDto
        {
            TKey = bankAccount.TKey,
            BankAccountId = bankAccount.BankAccountId,
            BankId = bankAccount.BankId,
            BankName = bankName,
            AccountName = bankAccount.AccountName,
            AccountNumber = bankAccount.AccountNumber,
            AccountType = bankAccount.AccountType,
            AccountTypeName = GetAccountTypeName(bankAccount.AccountType),
            CurrencyId = bankAccount.CurrencyId,
            Status = bankAccount.Status,
            StatusName = GetStatusName(bankAccount.Status),
            Balance = bankAccount.Balance,
            OpeningDate = bankAccount.OpeningDate,
            ClosingDate = bankAccount.ClosingDate,
            ContactPerson = bankAccount.ContactPerson,
            ContactPhone = bankAccount.ContactPhone,
            ContactEmail = bankAccount.ContactEmail,
            BranchName = bankAccount.BranchName,
            BranchCode = bankAccount.BranchCode,
            SwiftCode = bankAccount.SwiftCode,
            Notes = bankAccount.Notes,
            CreatedBy = bankAccount.CreatedBy,
            CreatedAt = bankAccount.CreatedAt,
            UpdatedBy = bankAccount.UpdatedBy,
            UpdatedAt = bankAccount.UpdatedAt,
            CreatedPriority = bankAccount.CreatedPriority,
            CreatedGroup = bankAccount.CreatedGroup
        };
    }

    private string? GetAccountTypeName(string? accountType)
    {
        return accountType switch
        {
            "1" => "活期",
            "2" => "定期",
            "3" => "外幣",
            _ => accountType
        };
    }

    private string? GetStatusName(string status)
    {
        return status switch
        {
            "1" => "啟用",
            "0" => "停用",
            _ => status
        };
    }
}
