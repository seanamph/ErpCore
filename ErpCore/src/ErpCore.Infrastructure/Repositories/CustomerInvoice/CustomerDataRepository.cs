using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 客戶資料 Repository 實作 (SYS2000 - 客戶資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CustomerDataRepository : BaseRepository, ICustomerDataRepository
{
    public CustomerDataRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<CustomerData?> GetByIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CustomerData 
                WHERE CustomerId = @CustomerId";

            return await QueryFirstOrDefaultAsync<CustomerData>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CustomerData>> QueryAsync(CustomerDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CustomerData
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.CustomerId}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                sql += " AND CustomerName LIKE @CustomerName";
                parameters.Add("CustomerName", $"%{query.CustomerName}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND CustomerType = @CustomerType";
                parameters.Add("CustomerType", query.CustomerType);
            }

            if (!string.IsNullOrEmpty(query.TaxId))
            {
                sql += " AND TaxId LIKE @TaxId";
                parameters.Add("TaxId", $"%{query.TaxId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "CustomerId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CustomerData>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<CustomerData>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(CustomerDataQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM CustomerData
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                sql += " AND CustomerId LIKE @CustomerId";
                parameters.Add("CustomerId", $"%{query.CustomerId}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                sql += " AND CustomerName LIKE @CustomerName";
                parameters.Add("CustomerName", $"%{query.CustomerName}%");
            }

            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND CustomerType = @CustomerType";
                parameters.Add("CustomerType", query.CustomerType);
            }

            if (!string.IsNullOrEmpty(query.TaxId))
            {
                sql += " AND TaxId LIKE @TaxId";
                parameters.Add("TaxId", $"%{query.TaxId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢客戶總數失敗", ex);
            throw;
        }
    }

    public async Task<CustomerData> CreateAsync(CustomerData customer, List<CustomerContact>? contacts = null, List<CustomerAddress>? addresses = null, List<CustomerBankAccount>? bankAccounts = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            customer.CreatedAt = DateTime.Now;
            customer.UpdatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO CustomerData 
                (CustomerId, CustomerName, CustomerType, TaxId, ContactPerson, ContactPhone, ContactEmail, ContactFax,
                 Address, CityId, ZoneId, ZipCode, PaymentTerm, CreditLimit, CurrencyId, Status, Memo,
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@CustomerId, @CustomerName, @CustomerType, @TaxId, @ContactPerson, @ContactPhone, @ContactEmail, @ContactFax,
                 @Address, @CityId, @ZoneId, @ZipCode, @PaymentTerm, @CreditLimit, @CurrencyId, @Status, @Memo,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var tKey = await connection.QuerySingleAsync<long>(insertSql, customer, transaction);
            customer.TKey = tKey;

            // 新增聯絡人
            if (contacts != null && contacts.Any())
            {
                foreach (var contact in contacts)
                {
                    contact.CustomerId = customer.CustomerId;
                    contact.CreatedAt = DateTime.Now;
                    contact.UpdatedAt = DateTime.Now;

                    const string insertContactSql = @"
                        INSERT INTO CustomerContacts 
                        (CustomerId, ContactName, ContactTitle, ContactPhone, ContactMobile, ContactEmail, ContactFax, IsPrimary, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @ContactName, @ContactTitle, @ContactPhone, @ContactMobile, @ContactEmail, @ContactFax, @IsPrimary, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertContactSql, contact, transaction);
                }
            }

            // 新增地址
            if (addresses != null && addresses.Any())
            {
                foreach (var address in addresses)
                {
                    address.CustomerId = customer.CustomerId;
                    address.CreatedAt = DateTime.Now;
                    address.UpdatedAt = DateTime.Now;

                    const string insertAddressSql = @"
                        INSERT INTO CustomerAddresses 
                        (CustomerId, AddressType, Address, CityId, ZoneId, ZipCode, IsDefault, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @AddressType, @Address, @CityId, @ZoneId, @ZipCode, @IsDefault, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertAddressSql, address, transaction);
                }
            }

            // 新增銀行帳戶
            if (bankAccounts != null && bankAccounts.Any())
            {
                foreach (var bankAccount in bankAccounts)
                {
                    bankAccount.CustomerId = customer.CustomerId;
                    bankAccount.CreatedAt = DateTime.Now;
                    bankAccount.UpdatedAt = DateTime.Now;

                    const string insertBankAccountSql = @"
                        INSERT INTO CustomerBankAccounts 
                        (CustomerId, BankId, AccountNo, AccountName, IsDefault, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @BankId, @AccountNo, @AccountName, @IsDefault, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertBankAccountSql, bankAccount, transaction);
                }
            }

            transaction.Commit();
            _logger.LogInfo($"建立客戶成功: {customer.CustomerId}");
            return customer;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"建立客戶失敗: {customer.CustomerId}", ex);
            throw;
        }
    }

    public async Task<CustomerData> UpdateAsync(CustomerData customer, List<CustomerContact>? contacts = null, List<CustomerAddress>? addresses = null, List<CustomerBankAccount>? bankAccounts = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            customer.UpdatedAt = DateTime.Now;

            const string updateSql = @"
                UPDATE CustomerData SET
                    CustomerName = @CustomerName,
                    CustomerType = @CustomerType,
                    TaxId = @TaxId,
                    ContactPerson = @ContactPerson,
                    ContactPhone = @ContactPhone,
                    ContactEmail = @ContactEmail,
                    ContactFax = @ContactFax,
                    Address = @Address,
                    CityId = @CityId,
                    ZoneId = @ZoneId,
                    ZipCode = @ZipCode,
                    PaymentTerm = @PaymentTerm,
                    CreditLimit = @CreditLimit,
                    CurrencyId = @CurrencyId,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE CustomerId = @CustomerId";

            await connection.ExecuteAsync(updateSql, customer, transaction);

            // 更新聯絡人（先刪除再新增）
            if (contacts != null)
            {
                const string deleteContactsSql = "DELETE FROM CustomerContacts WHERE CustomerId = @CustomerId";
                await connection.ExecuteAsync(deleteContactsSql, new { CustomerId = customer.CustomerId }, transaction);

                foreach (var contact in contacts)
                {
                    contact.CustomerId = customer.CustomerId;
                    contact.CreatedAt = DateTime.Now;
                    contact.UpdatedAt = DateTime.Now;

                    const string insertContactSql = @"
                        INSERT INTO CustomerContacts 
                        (CustomerId, ContactName, ContactTitle, ContactPhone, ContactMobile, ContactEmail, ContactFax, IsPrimary, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @ContactName, @ContactTitle, @ContactPhone, @ContactMobile, @ContactEmail, @ContactFax, @IsPrimary, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertContactSql, contact, transaction);
                }
            }

            // 更新地址（先刪除再新增）
            if (addresses != null)
            {
                const string deleteAddressesSql = "DELETE FROM CustomerAddresses WHERE CustomerId = @CustomerId";
                await connection.ExecuteAsync(deleteAddressesSql, new { CustomerId = customer.CustomerId }, transaction);

                foreach (var address in addresses)
                {
                    address.CustomerId = customer.CustomerId;
                    address.CreatedAt = DateTime.Now;
                    address.UpdatedAt = DateTime.Now;

                    const string insertAddressSql = @"
                        INSERT INTO CustomerAddresses 
                        (CustomerId, AddressType, Address, CityId, ZoneId, ZipCode, IsDefault, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @AddressType, @Address, @CityId, @ZoneId, @ZipCode, @IsDefault, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertAddressSql, address, transaction);
                }
            }

            // 更新銀行帳戶（先刪除再新增）
            if (bankAccounts != null)
            {
                const string deleteBankAccountsSql = "DELETE FROM CustomerBankAccounts WHERE CustomerId = @CustomerId";
                await connection.ExecuteAsync(deleteBankAccountsSql, new { CustomerId = customer.CustomerId }, transaction);

                foreach (var bankAccount in bankAccounts)
                {
                    bankAccount.CustomerId = customer.CustomerId;
                    bankAccount.CreatedAt = DateTime.Now;
                    bankAccount.UpdatedAt = DateTime.Now;

                    const string insertBankAccountSql = @"
                        INSERT INTO CustomerBankAccounts 
                        (CustomerId, BankId, AccountNo, AccountName, IsDefault, Memo,
                         CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                        VALUES 
                        (@CustomerId, @BankId, @AccountNo, @AccountName, @IsDefault, @Memo,
                         @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);";

                    await connection.ExecuteAsync(insertBankAccountSql, bankAccount, transaction);
                }
            }

            transaction.Commit();
            _logger.LogInfo($"更新客戶成功: {customer.CustomerId}");
            return customer;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"更新客戶失敗: {customer.CustomerId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string customerId)
    {
        try
        {
            // 軟刪除：將狀態設為停用
            const string sql = @"
                UPDATE CustomerData 
                SET Status = 'I', UpdatedAt = GETDATE()
                WHERE CustomerId = @CustomerId";

            await ExecuteAsync(sql, new { CustomerId = customerId });
            _logger.LogInfo($"刪除客戶成功: {customerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM CustomerData 
                WHERE CustomerId = @CustomerId";

            var count = await ExecuteScalarAsync<int>(sql, new { CustomerId = customerId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查客戶編號是否存在失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerContact>> GetContactsByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CustomerContacts 
                WHERE CustomerId = @CustomerId 
                ORDER BY IsPrimary DESC, TKey";

            return await QueryAsync<CustomerContact>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶聯絡人失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CustomerAddresses 
                WHERE CustomerId = @CustomerId 
                ORDER BY IsDefault DESC, TKey";

            return await QueryAsync<CustomerAddress>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶地址失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<CustomerBankAccount>> GetBankAccountsByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CustomerBankAccounts 
                WHERE CustomerId = @CustomerId 
                ORDER BY IsDefault DESC, TKey";

            return await QueryAsync<CustomerBankAccount>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶銀行帳戶失敗: {customerId}", ex);
            throw;
        }
    }
}

