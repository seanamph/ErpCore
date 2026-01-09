using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SapIntegration;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SapIntegration;

/// <summary>
/// SAP整合資料 Repository 實作 (TransSAP系列)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class TransSapRepository : BaseRepository, ITransSapRepository
{
    public TransSapRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<TransSap?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TransSap 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<TransSap>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢SAP整合資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<TransSap?> GetByTransIdAsync(string transId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM TransSap 
                WHERE TransId = @TransId";

            return await QueryFirstOrDefaultAsync<TransSap>(sql, new { TransId = transId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢SAP整合資料失敗: {transId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<TransSap>> QueryAsync(TransSapQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM TransSap
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

            if (!string.IsNullOrEmpty(query.SapSystemCode))
            {
                sql += " AND SapSystemCode = @SapSystemCode";
                parameters.Add("SapSystemCode", query.SapSystemCode);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.TransDateFrom.HasValue)
            {
                sql += " AND TransDate >= @TransDateFrom";
                parameters.Add("TransDateFrom", query.TransDateFrom);
            }

            if (query.TransDateTo.HasValue)
            {
                sql += " AND TransDate <= @TransDateTo";
                parameters.Add("TransDateTo", query.TransDateTo);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (TransId LIKE @Keyword OR TransType LIKE @Keyword)";
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

            return await QueryAsync<TransSap>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP整合資料列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(TransSapQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM TransSap
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

            if (!string.IsNullOrEmpty(query.SapSystemCode))
            {
                sql += " AND SapSystemCode = @SapSystemCode";
                parameters.Add("SapSystemCode", query.SapSystemCode);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.TransDateFrom.HasValue)
            {
                sql += " AND TransDate >= @TransDateFrom";
                parameters.Add("TransDateFrom", query.TransDateFrom);
            }

            if (query.TransDateTo.HasValue)
            {
                sql += " AND TransDate <= @TransDateTo";
                parameters.Add("TransDateTo", query.TransDateTo);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (TransId LIKE @Keyword OR TransType LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢SAP整合資料總數失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(TransSap entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO TransSap 
                (TransId, TransType, SapSystemCode, TransDate, Status, RequestData, ResponseData, ErrorMessage, RetryCount, Memo, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@TransId, @TransType, @SapSystemCode, @TransDate, @Status, @RequestData, @ResponseData, @ErrorMessage, @RetryCount, @Memo, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var parameters = new DynamicParameters();
            parameters.Add("TransId", entity.TransId);
            parameters.Add("TransType", entity.TransType);
            parameters.Add("SapSystemCode", entity.SapSystemCode);
            parameters.Add("TransDate", entity.TransDate);
            parameters.Add("Status", entity.Status);
            parameters.Add("RequestData", entity.RequestData);
            parameters.Add("ResponseData", entity.ResponseData);
            parameters.Add("ErrorMessage", entity.ErrorMessage);
            parameters.Add("RetryCount", entity.RetryCount);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("CreatedBy", entity.CreatedBy);
            parameters.Add("CreatedAt", entity.CreatedAt);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            return await QuerySingleAsync<long>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增SAP整合資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(TransSap entity)
    {
        try
        {
            const string sql = @"
                UPDATE TransSap 
                SET TransType = @TransType, 
                    SapSystemCode = @SapSystemCode, 
                    TransDate = @TransDate, 
                    Status = @Status, 
                    RequestData = @RequestData, 
                    ResponseData = @ResponseData, 
                    ErrorMessage = @ErrorMessage, 
                    RetryCount = @RetryCount, 
                    Memo = @Memo, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            var parameters = new DynamicParameters();
            parameters.Add("TKey", entity.TKey);
            parameters.Add("TransType", entity.TransType);
            parameters.Add("SapSystemCode", entity.SapSystemCode);
            parameters.Add("TransDate", entity.TransDate);
            parameters.Add("Status", entity.Status);
            parameters.Add("RequestData", entity.RequestData);
            parameters.Add("ResponseData", entity.ResponseData);
            parameters.Add("ErrorMessage", entity.ErrorMessage);
            parameters.Add("RetryCount", entity.RetryCount);
            parameters.Add("Memo", entity.Memo);
            parameters.Add("UpdatedBy", entity.UpdatedBy);
            parameters.Add("UpdatedAt", entity.UpdatedAt);

            await ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改SAP整合資料失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM TransSap 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除SAP整合資料失敗: {tKey}", ex);
            throw;
        }
    }
}

