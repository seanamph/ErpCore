using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN客戶 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class JgjNCustomerRepository : BaseRepository, IJgjNCustomerRepository
{
    public JgjNCustomerRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<JgjNCustomer?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNCustomer 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<JgjNCustomer>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN客戶失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNCustomer?> GetByCustomerIdAsync(string customerId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM JgjNCustomer 
                WHERE CustomerId = @CustomerId";

            return await QueryFirstOrDefaultAsync<JgjNCustomer>(sql, new { CustomerId = customerId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN客戶失敗: {customerId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<JgjNCustomer>> QueryAsync(JgjNCustomerQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM JgjNCustomer
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND CustomerType = @CustomerType";
                parameters.Add("CustomerType", query.CustomerType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (CustomerId LIKE @Keyword OR CustomerName LIKE @Keyword OR Phone LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";
            }

            return await QueryAsync<JgjNCustomer>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN客戶列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(JgjNCustomerQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM JgjNCustomer
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.CustomerType))
            {
                sql += " AND CustomerType = @CustomerType";
                parameters.Add("CustomerType", query.CustomerType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (CustomerId LIKE @Keyword OR CustomerName LIKE @Keyword OR Phone LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN客戶數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(JgjNCustomer entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO JgjNCustomer 
                (CustomerId, CustomerName, CustomerType, ContactPerson, Phone, Email, Address, Status, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@CustomerId, @CustomerName, @CustomerType, @ContactPerson, @Phone, @Email, @Address, @Status, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增JGJN客戶失敗: {entity.CustomerId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(JgjNCustomer entity)
    {
        try
        {
            const string sql = @"
                UPDATE JgjNCustomer 
                SET CustomerName = @CustomerName, CustomerType = @CustomerType, ContactPerson = @ContactPerson, 
                    Phone = @Phone, Email = @Email, Address = @Address, Status = @Status, Memo = @Memo, 
                    UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新JGJN客戶失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM JgjNCustomer 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN客戶失敗: {tKey}", ex);
            throw;
        }
    }
}

