using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 銀行帳戶 Repository 實作 (銀行帳戶維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class BankAccountRepository : BaseRepository, IBankAccountRepository
{
    public BankAccountRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<BankAccount?> GetByIdAsync(string bankAccountId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM BankAccounts 
                WHERE BankAccountId = @BankAccountId";

            return await QueryFirstOrDefaultAsync<BankAccount>(sql, new { BankAccountId = bankAccountId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行帳戶失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<BankAccount>> QueryAsync(BankAccountQuery query)
    {
        try
        {
            var sql = @"
                SELECT ba.*
                FROM BankAccounts ba
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.BankAccountId))
            {
                sql += " AND ba.BankAccountId LIKE @BankAccountId";
                parameters.Add("BankAccountId", $"%{query.BankAccountId}%");
            }

            if (!string.IsNullOrEmpty(query.BankId))
            {
                sql += " AND ba.BankId = @BankId";
                parameters.Add("BankId", query.BankId);
            }

            if (!string.IsNullOrEmpty(query.AccountName))
            {
                sql += " AND ba.AccountName LIKE @AccountName";
                parameters.Add("AccountName", $"%{query.AccountName}%");
            }

            if (!string.IsNullOrEmpty(query.AccountNumber))
            {
                sql += " AND ba.AccountNumber LIKE @AccountNumber";
                parameters.Add("AccountNumber", $"%{query.AccountNumber}%");
            }

            if (!string.IsNullOrEmpty(query.AccountType))
            {
                sql += " AND ba.AccountType = @AccountType";
                parameters.Add("AccountType", query.AccountType);
            }

            if (!string.IsNullOrEmpty(query.CurrencyId))
            {
                sql += " AND ba.CurrencyId = @CurrencyId";
                parameters.Add("CurrencyId", query.CurrencyId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND ba.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = !string.IsNullOrEmpty(query.SortField) ? query.SortField : "BankAccountId";
            var sortOrder = !string.IsNullOrEmpty(query.SortOrder) ? query.SortOrder : "ASC";
            sql += $" ORDER BY ba.{sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<BankAccount>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銀行帳戶列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(BankAccountQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM BankAccounts ba
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.BankAccountId))
            {
                sql += " AND ba.BankAccountId LIKE @BankAccountId";
                parameters.Add("BankAccountId", $"%{query.BankAccountId}%");
            }

            if (!string.IsNullOrEmpty(query.BankId))
            {
                sql += " AND ba.BankId = @BankId";
                parameters.Add("BankId", query.BankId);
            }

            if (!string.IsNullOrEmpty(query.AccountName))
            {
                sql += " AND ba.AccountName LIKE @AccountName";
                parameters.Add("AccountName", $"%{query.AccountName}%");
            }

            if (!string.IsNullOrEmpty(query.AccountNumber))
            {
                sql += " AND ba.AccountNumber LIKE @AccountNumber";
                parameters.Add("AccountNumber", $"%{query.AccountNumber}%");
            }

            if (!string.IsNullOrEmpty(query.AccountType))
            {
                sql += " AND ba.AccountType = @AccountType";
                parameters.Add("AccountType", query.AccountType);
            }

            if (!string.IsNullOrEmpty(query.CurrencyId))
            {
                sql += " AND ba.CurrencyId = @CurrencyId";
                parameters.Add("CurrencyId", query.CurrencyId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND ba.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銀行帳戶數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string bankAccountId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM BankAccounts 
                WHERE BankAccountId = @BankAccountId";

            var count = await ExecuteScalarAsync<int>(sql, new { BankAccountId = bankAccountId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查銀行帳戶是否存在失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByAccountNumberAsync(string accountNumber)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM BankAccounts 
                WHERE AccountNumber = @AccountNumber";

            var count = await ExecuteScalarAsync<int>(sql, new { AccountNumber = accountNumber });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查帳戶號碼是否存在失敗: {accountNumber}", ex);
            throw;
        }
    }

    public async Task<BankAccount> CreateAsync(BankAccount bankAccount)
    {
        try
        {
            const string sql = @"
                INSERT INTO BankAccounts (
                    BankAccountId, BankId, AccountName, AccountNumber, AccountType, CurrencyId, Status,
                    Balance, OpeningDate, ClosingDate, ContactPerson, ContactPhone, ContactEmail,
                    BranchName, BranchCode, SwiftCode, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                ) VALUES (
                    @BankAccountId, @BankId, @AccountName, @AccountNumber, @AccountType, @CurrencyId, @Status,
                    @Balance, @OpeningDate, @ClosingDate, @ContactPerson, @ContactPhone, @ContactEmail,
                    @BranchName, @BranchCode, @SwiftCode, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                bankAccount.BankAccountId,
                bankAccount.BankId,
                bankAccount.AccountName,
                bankAccount.AccountNumber,
                bankAccount.AccountType,
                bankAccount.CurrencyId,
                bankAccount.Status,
                bankAccount.Balance,
                bankAccount.OpeningDate,
                bankAccount.ClosingDate,
                bankAccount.ContactPerson,
                bankAccount.ContactPhone,
                bankAccount.ContactEmail,
                bankAccount.BranchName,
                bankAccount.BranchCode,
                bankAccount.SwiftCode,
                bankAccount.Notes,
                bankAccount.CreatedBy,
                bankAccount.CreatedAt,
                bankAccount.UpdatedBy,
                bankAccount.UpdatedAt,
                bankAccount.CreatedPriority,
                bankAccount.CreatedGroup
            });

            bankAccount.TKey = tKey;
            return bankAccount;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增銀行帳戶失敗: {bankAccount.BankAccountId}", ex);
            throw;
        }
    }

    public async Task<BankAccount> UpdateAsync(BankAccount bankAccount)
    {
        try
        {
            const string sql = @"
                UPDATE BankAccounts SET
                    BankId = @BankId,
                    AccountName = @AccountName,
                    AccountNumber = @AccountNumber,
                    AccountType = @AccountType,
                    CurrencyId = @CurrencyId,
                    Status = @Status,
                    Balance = @Balance,
                    OpeningDate = @OpeningDate,
                    ClosingDate = @ClosingDate,
                    ContactPerson = @ContactPerson,
                    ContactPhone = @ContactPhone,
                    ContactEmail = @ContactEmail,
                    BranchName = @BranchName,
                    BranchCode = @BranchCode,
                    SwiftCode = @SwiftCode,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE BankAccountId = @BankAccountId";

            await ExecuteAsync(sql, new
            {
                bankAccount.BankAccountId,
                bankAccount.BankId,
                bankAccount.AccountName,
                bankAccount.AccountNumber,
                bankAccount.AccountType,
                bankAccount.CurrencyId,
                bankAccount.Status,
                bankAccount.Balance,
                bankAccount.OpeningDate,
                bankAccount.ClosingDate,
                bankAccount.ContactPerson,
                bankAccount.ContactPhone,
                bankAccount.ContactEmail,
                bankAccount.BranchName,
                bankAccount.BranchCode,
                bankAccount.SwiftCode,
                bankAccount.Notes,
                bankAccount.UpdatedBy,
                bankAccount.UpdatedAt
            });

            return bankAccount;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銀行帳戶失敗: {bankAccount.BankAccountId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string bankAccountId)
    {
        try
        {
            const string sql = @"
                DELETE FROM BankAccounts 
                WHERE BankAccountId = @BankAccountId";

            await ExecuteAsync(sql, new { BankAccountId = bankAccountId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銀行帳戶失敗: {bankAccountId}", ex);
            throw;
        }
    }

    public async Task<decimal?> GetBalanceAsync(string bankAccountId)
    {
        try
        {
            const string sql = @"
                SELECT Balance FROM BankAccounts 
                WHERE BankAccountId = @BankAccountId";

            return await QueryFirstOrDefaultAsync<decimal?>(sql, new { BankAccountId = bankAccountId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銀行帳戶餘額失敗: {bankAccountId}", ex);
            throw;
        }
    }
}
