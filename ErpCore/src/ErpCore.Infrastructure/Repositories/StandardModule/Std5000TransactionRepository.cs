using System.Data;
using Dapper;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 交易 Repository 實作 (SYS5310-SYS53C6 - 交易管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Std5000TransactionRepository : BaseRepository, IStd5000TransactionRepository
{
    public Std5000TransactionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Std5000Transaction?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000Transactions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Std5000Transaction>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000Transaction?> GetByTransIdAsync(string transId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Std5000Transactions 
                WHERE TransId = @TransId";

            return await QueryFirstOrDefaultAsync<Std5000Transaction>(sql, new { TransId = transId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000交易失敗: {transId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Std5000Transaction>> QueryAsync(Std5000TransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Std5000Transactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TransId))
            {
                sql += " AND TransId LIKE @TransId";
                parameters.Add("TransId", $"%{query.TransId}%");
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId = @MemberId";
                parameters.Add("MemberId", query.MemberId);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND TransDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND TransDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (TransId LIKE @Keyword)";
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
                sql += " ORDER BY TransDate DESC, TransId DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Std5000Transaction>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000交易列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Std5000TransactionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Std5000Transactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.TransId))
            {
                sql += " AND TransId LIKE @TransId";
                parameters.Add("TransId", $"%{query.TransId}%");
            }

            if (!string.IsNullOrEmpty(query.TransType))
            {
                sql += " AND TransType = @TransType";
                parameters.Add("TransType", query.TransType);
            }

            if (!string.IsNullOrEmpty(query.MemberId))
            {
                sql += " AND MemberId = @MemberId";
                parameters.Add("MemberId", query.MemberId);
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND TransDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND TransDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (TransId LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000交易總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Std5000Transaction entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Std5000Transactions 
                (TransId, TransDate, TransType, MemberId, ShopId, Amount, Points, Status, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@TransId, @TransDate, @TransType, @MemberId, @ShopId, @Amount, @Points, @Status, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("TransId", entity.TransId);
            parameters.Add("TransDate", entity.TransDate);
            parameters.Add("TransType", entity.TransType);
            parameters.Add("MemberId", entity.MemberId);
            parameters.Add("ShopId", entity.ShopId);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Points", entity.Points);
            parameters.Add("Status", entity.Status);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000交易失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Std5000Transaction entity)
    {
        try
        {
            const string sql = @"
                UPDATE Std5000Transactions 
                SET TransType = @TransType,
                    MemberId = @MemberId,
                    ShopId = @ShopId,
                    Amount = @Amount,
                    Points = @Points,
                    Status = @Status,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("TransType", entity.TransType);
            parameters.Add("MemberId", entity.MemberId);
            parameters.Add("ShopId", entity.ShopId);
            parameters.Add("Amount", entity.Amount);
            parameters.Add("Points", entity.Points);
            parameters.Add("Status", entity.Status);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000交易失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Std5000Transactions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000交易失敗: {tKey}", ex);
            throw;
        }
    }
}

