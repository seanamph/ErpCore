using Dapper;
using ErpCore.Domain.Entities.Customer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Customer;

/// <summary>
/// 客戶 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class CustomerRepository : BaseRepository, ICustomerRepository
{
    public CustomerRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Customer?> GetByIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE CustomerId = @CustomerId";

            return await QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<Customer?> GetByGuiIdAsync(string guiId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Customers 
                WHERE GuiId = @GuiId";

            return await QueryFirstOrDefaultAsync<Customer>(sql, new { GuiId = guiId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶失敗(統一編號): {guiId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Customer>> QueryAsync(CustomerQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Customers
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

            if (!string.IsNullOrEmpty(query.GuiId))
            {
                sql += " AND GuiId LIKE @GuiId";
                parameters.Add("GuiId", $"%{query.GuiId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.SalesId))
            {
                sql += " AND SalesId = @SalesId";
                parameters.Add("SalesId", query.SalesId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CustomerId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Customer>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Customers
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                countSql += " AND CustomerId LIKE @CustomerId";
                countParameters.Add("CustomerId", $"%{query.CustomerId}%");
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                countSql += " AND CustomerName LIKE @CustomerName";
                countParameters.Add("CustomerName", $"%{query.CustomerName}%");
            }
            if (!string.IsNullOrEmpty(query.GuiId))
            {
                countSql += " AND GuiId LIKE @GuiId";
                countParameters.Add("GuiId", $"%{query.GuiId}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.SalesId))
            {
                countSql += " AND SalesId = @SalesId";
                countParameters.Add("SalesId", query.SalesId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Customer>
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

    public async Task<Customer> CreateAsync(Customer customer)
    {
        try
        {
            const string sql = @"
                INSERT INTO Customers (
                    CustomerId, GuiId, GuiType, CustomerName, CustomerNameE, ShortName,
                    ContactStaff, HomeTel, CompTel, Fax, Cell, Email, Sex, Title,
                    City, Canton, Addr, TaxAddr, DelyAddr, PostId,
                    DiscountYn, DiscountNo, SalesId, TransDate, TransNo, AccAmt, MonthlyYn,
                    Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @CustomerId, @GuiId, @GuiType, @CustomerName, @CustomerNameE, @ShortName,
                    @ContactStaff, @HomeTel, @CompTel, @Fax, @Cell, @Email, @Sex, @Title,
                    @City, @Canton, @Addr, @TaxAddr, @DelyAddr, @PostId,
                    @DiscountYn, @DiscountNo, @SalesId, @TransDate, @TransNo, @AccAmt, @MonthlyYn,
                    @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Customer>(sql, customer);
            if (result == null)
            {
                throw new InvalidOperationException("新增客戶失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增客戶失敗: {customer.CustomerId}", ex);
            throw;
        }
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        try
        {
            const string sql = @"
                UPDATE Customers SET
                    GuiId = @GuiId,
                    GuiType = @GuiType,
                    CustomerName = @CustomerName,
                    CustomerNameE = @CustomerNameE,
                    ShortName = @ShortName,
                    ContactStaff = @ContactStaff,
                    HomeTel = @HomeTel,
                    CompTel = @CompTel,
                    Fax = @Fax,
                    Cell = @Cell,
                    Email = @Email,
                    Sex = @Sex,
                    Title = @Title,
                    City = @City,
                    Canton = @Canton,
                    Addr = @Addr,
                    TaxAddr = @TaxAddr,
                    DelyAddr = @DelyAddr,
                    PostId = @PostId,
                    DiscountYn = @DiscountYn,
                    DiscountNo = @DiscountNo,
                    SalesId = @SalesId,
                    TransDate = @TransDate,
                    TransNo = @TransNo,
                    AccAmt = @AccAmt,
                    MonthlyYn = @MonthlyYn,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE CustomerId = @CustomerId";

            var result = await QueryFirstOrDefaultAsync<Customer>(sql, customer);
            if (result == null)
            {
                throw new InvalidOperationException($"客戶不存在: {customer.CustomerId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改客戶失敗: {customer.CustomerId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string customerId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Customers 
                WHERE CustomerId = @CustomerId";

            await ExecuteAsync(sql, new { CustomerId = customerId });
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
                SELECT COUNT(*) FROM Customers 
                WHERE CustomerId = @CustomerId";

            var count = await QuerySingleAsync<int>(sql, new { CustomerId = customerId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查客戶是否存在失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByGuiIdAsync(string guiId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Customers 
                WHERE GuiId = @GuiId";

            var count = await QuerySingleAsync<int>(sql, new { GuiId = guiId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查統一編號是否存在失敗: {guiId}", ex);
            throw;
        }
    }

    public async Task<List<CustomerContact>> GetContactsByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM CustomerContacts 
                WHERE CustomerId = @CustomerId
                ORDER BY IsPrimary DESC, ContactName";

            var items = await QueryAsync<CustomerContact>(sql, new { CustomerId = customerId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶聯絡人失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<CustomerContact> CreateContactAsync(CustomerContact contact)
    {
        try
        {
            const string sql = @"
                INSERT INTO CustomerContacts (
                    ContactId, CustomerId, ContactName, ContactTitle, ContactTel, ContactCell, ContactEmail, IsPrimary,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ContactId, @CustomerId, @ContactName, @ContactTitle, @ContactTel, @ContactCell, @ContactEmail, @IsPrimary,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<CustomerContact>(sql, contact);
            if (result == null)
            {
                throw new InvalidOperationException("新增客戶聯絡人失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增客戶聯絡人失敗: {contact.CustomerId}", ex);
            throw;
        }
    }

    public async Task DeleteContactsByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                DELETE FROM CustomerContacts 
                WHERE CustomerId = @CustomerId";

            await ExecuteAsync(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除客戶聯絡人失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Customer>> AdvancedQueryAsync(CustomerAdvancedQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Customers
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

            if (!string.IsNullOrEmpty(query.GuiId))
            {
                sql += " AND GuiId LIKE @GuiId";
                parameters.Add("GuiId", $"%{query.GuiId}%");
            }

            if (!string.IsNullOrEmpty(query.GuiType))
            {
                sql += " AND GuiType = @GuiType";
                parameters.Add("GuiType", query.GuiType);
            }

            if (!string.IsNullOrEmpty(query.ContactStaff))
            {
                sql += " AND ContactStaff LIKE @ContactStaff";
                parameters.Add("ContactStaff", $"%{query.ContactStaff}%");
            }

            if (!string.IsNullOrEmpty(query.CompTel))
            {
                sql += " AND CompTel LIKE @CompTel";
                parameters.Add("CompTel", $"%{query.CompTel}%");
            }

            if (!string.IsNullOrEmpty(query.Cell))
            {
                sql += " AND Cell LIKE @Cell";
                parameters.Add("Cell", $"%{query.Cell}%");
            }

            if (!string.IsNullOrEmpty(query.Email))
            {
                sql += " AND Email LIKE @Email";
                parameters.Add("Email", $"%{query.Email}%");
            }

            if (!string.IsNullOrEmpty(query.City))
            {
                sql += " AND City = @City";
                parameters.Add("City", query.City);
            }

            if (!string.IsNullOrEmpty(query.Canton))
            {
                sql += " AND Canton = @Canton";
                parameters.Add("Canton", query.Canton);
            }

            if (!string.IsNullOrEmpty(query.SalesId))
            {
                sql += " AND SalesId = @SalesId";
                parameters.Add("SalesId", query.SalesId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.DiscountYn))
            {
                sql += " AND DiscountYn = @DiscountYn";
                parameters.Add("DiscountYn", query.DiscountYn);
            }

            if (!string.IsNullOrEmpty(query.MonthlyYn))
            {
                sql += " AND MonthlyYn = @MonthlyYn";
                parameters.Add("MonthlyYn", query.MonthlyYn);
            }

            if (query.TransDateFrom.HasValue)
            {
                sql += " AND TransDate >= @TransDateFrom";
                parameters.Add("TransDateFrom", query.TransDateFrom.Value);
            }

            if (query.TransDateTo.HasValue)
            {
                sql += " AND TransDate <= @TransDateTo";
                parameters.Add("TransDateTo", query.TransDateTo.Value);
            }

            if (query.AccAmtFrom.HasValue)
            {
                sql += " AND AccAmt >= @AccAmtFrom";
                parameters.Add("AccAmtFrom", query.AccAmtFrom.Value);
            }

            if (query.AccAmtTo.HasValue)
            {
                sql += " AND AccAmt <= @AccAmtTo";
                parameters.Add("AccAmtTo", query.AccAmtTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CustomerId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Customer>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Customers
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.CustomerId))
            {
                countSql += " AND CustomerId LIKE @CustomerId";
                countParameters.Add("CustomerId", $"%{query.CustomerId}%");
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                countSql += " AND CustomerName LIKE @CustomerName";
                countParameters.Add("CustomerName", $"%{query.CustomerName}%");
            }
            if (!string.IsNullOrEmpty(query.GuiId))
            {
                countSql += " AND GuiId LIKE @GuiId";
                countParameters.Add("GuiId", $"%{query.GuiId}%");
            }
            if (!string.IsNullOrEmpty(query.GuiType))
            {
                countSql += " AND GuiType = @GuiType";
                countParameters.Add("GuiType", query.GuiType);
            }
            if (!string.IsNullOrEmpty(query.ContactStaff))
            {
                countSql += " AND ContactStaff LIKE @ContactStaff";
                countParameters.Add("ContactStaff", $"%{query.ContactStaff}%");
            }
            if (!string.IsNullOrEmpty(query.CompTel))
            {
                countSql += " AND CompTel LIKE @CompTel";
                countParameters.Add("CompTel", $"%{query.CompTel}%");
            }
            if (!string.IsNullOrEmpty(query.Cell))
            {
                countSql += " AND Cell LIKE @Cell";
                countParameters.Add("Cell", $"%{query.Cell}%");
            }
            if (!string.IsNullOrEmpty(query.Email))
            {
                countSql += " AND Email LIKE @Email";
                countParameters.Add("Email", $"%{query.Email}%");
            }
            if (!string.IsNullOrEmpty(query.City))
            {
                countSql += " AND City = @City";
                countParameters.Add("City", query.City);
            }
            if (!string.IsNullOrEmpty(query.Canton))
            {
                countSql += " AND Canton = @Canton";
                countParameters.Add("Canton", query.Canton);
            }
            if (!string.IsNullOrEmpty(query.SalesId))
            {
                countSql += " AND SalesId = @SalesId";
                countParameters.Add("SalesId", query.SalesId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.DiscountYn))
            {
                countSql += " AND DiscountYn = @DiscountYn";
                countParameters.Add("DiscountYn", query.DiscountYn);
            }
            if (!string.IsNullOrEmpty(query.MonthlyYn))
            {
                countSql += " AND MonthlyYn = @MonthlyYn";
                countParameters.Add("MonthlyYn", query.MonthlyYn);
            }
            if (query.TransDateFrom.HasValue)
            {
                countSql += " AND TransDate >= @TransDateFrom";
                countParameters.Add("TransDateFrom", query.TransDateFrom.Value);
            }
            if (query.TransDateTo.HasValue)
            {
                countSql += " AND TransDate <= @TransDateTo";
                countParameters.Add("TransDateTo", query.TransDateTo.Value);
            }
            if (query.AccAmtFrom.HasValue)
            {
                countSql += " AND AccAmt >= @AccAmtFrom";
                countParameters.Add("AccAmtFrom", query.AccAmtFrom.Value);
            }
            if (query.AccAmtTo.HasValue)
            {
                countSql += " AND AccAmt <= @AccAmtTo";
                countParameters.Add("AccAmtTo", query.AccAmtTo.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Customer>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("進階查詢客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<List<Customer>> SearchAsync(string keyword, int limit)
    {
        try
        {
            var sql = @"
                SELECT TOP (@Limit)
                    CustomerId, CustomerName, GuiId, ShortName, CompTel, Cell
                FROM Customers
                WHERE 1=1
                    AND (CustomerId LIKE @Keyword
                        OR CustomerName LIKE @Keyword
                        OR GuiId LIKE @Keyword
                        OR CompTel LIKE @Keyword
                        OR Cell LIKE @Keyword)
                ORDER BY CustomerId";

            var parameters = new DynamicParameters();
            parameters.Add("Keyword", $"%{keyword}%");
            parameters.Add("Limit", limit);

            var items = await QueryAsync<Customer>(sql, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"快速搜尋客戶失敗: {keyword}", ex);
            throw;
        }
    }

    public async Task<PagedResult<CustomerTransaction>> GetTransactionsAsync(CustomerTransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM CustomerTransactions
                WHERE CustomerId = @CustomerId";

            var parameters = new DynamicParameters();
            parameters.Add("CustomerId", query.CustomerId);

            if (query.DateFrom.HasValue)
            {
                sql += " AND TransactionDate >= @DateFrom";
                parameters.Add("DateFrom", query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                sql += " AND TransactionDate <= @DateTo";
                parameters.Add("DateTo", query.DateTo.Value);
            }

            sql += " ORDER BY TransactionDate DESC";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<CustomerTransaction>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM CustomerTransactions
                WHERE CustomerId = @CustomerId";

            var countParameters = new DynamicParameters();
            countParameters.Add("CustomerId", query.CustomerId);

            if (query.DateFrom.HasValue)
            {
                countSql += " AND TransactionDate >= @DateFrom";
                countParameters.Add("DateFrom", query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                countSql += " AND TransactionDate <= @DateTo";
                countParameters.Add("DateTo", query.DateTo.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<CustomerTransaction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢客戶交易記錄失敗: {query.CustomerId}", ex);
            throw;
        }
    }

    public async Task<QueryHistory> SaveQueryHistoryAsync(QueryHistory history)
    {
        try
        {
            const string sql = @"
                INSERT INTO QueryHistory (
                    HistoryId, UserId, ModuleCode, QueryName, QueryConditions, IsFavorite,
                    CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @HistoryId, @UserId, @ModuleCode, @QueryName, @QueryConditions, @IsFavorite,
                    @CreatedBy, @CreatedAt
                )";

            if (history.HistoryId == Guid.Empty)
            {
                history.HistoryId = Guid.NewGuid();
            }

            if (history.CreatedAt == default)
            {
                history.CreatedAt = DateTime.Now;
            }

            var result = await QueryFirstOrDefaultAsync<QueryHistory>(sql, history);
            if (result == null)
            {
                throw new InvalidOperationException("儲存查詢歷史記錄失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"儲存查詢歷史記錄失敗: {history.UserId}", ex);
            throw;
        }
    }

    public async Task<List<QueryHistory>> GetQueryHistoryAsync(string userId, string moduleCode)
    {
        try
        {
            const string sql = @"
                SELECT * FROM QueryHistory
                WHERE UserId = @UserId AND ModuleCode = @ModuleCode
                ORDER BY IsFavorite DESC, CreatedAt DESC";

            var items = await QueryAsync<QueryHistory>(sql, new { UserId = userId, ModuleCode = moduleCode });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢歷史記錄失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteQueryHistoryAsync(Guid historyId)
    {
        try
        {
            const string sql = @"
                DELETE FROM QueryHistory 
                WHERE HistoryId = @HistoryId";

            await ExecuteAsync(sql, new { HistoryId = historyId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除查詢歷史記錄失敗: {historyId}", ex);
            throw;
        }
    }
}

