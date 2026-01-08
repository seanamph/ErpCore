using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購其他功能 Repository 實作 (SYSP510-SYSP530)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProcurementOtherRepository : BaseRepository, IProcurementOtherRepository
{
    public ProcurementOtherRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ProcurementOther?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseOtherFunctions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ProcurementOther>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購其他功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ProcurementOther?> GetByFunctionIdAsync(string functionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseOtherFunctions 
                WHERE FunctionId = @FunctionId";

            return await QueryFirstOrDefaultAsync<ProcurementOther>(sql, new { FunctionId = functionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購其他功能失敗: {functionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ProcurementOther>> QueryAsync(ProcurementOtherQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseOtherFunctions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FunctionId))
            {
                sql += " AND FunctionId LIKE @FunctionId";
                parameters.Add("FunctionId", $"%{query.FunctionId}%");
            }

            if (!string.IsNullOrEmpty(query.FunctionName))
            {
                sql += " AND FunctionName LIKE @FunctionName";
                parameters.Add("FunctionName", $"%{query.FunctionName}%");
            }

            if (!string.IsNullOrEmpty(query.FunctionType))
            {
                sql += " AND FunctionType = @FunctionType";
                parameters.Add("FunctionType", query.FunctionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY SeqNo, FunctionId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<ProcurementOther>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購其他功能列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(ProcurementOtherQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PurchaseOtherFunctions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FunctionId))
            {
                sql += " AND FunctionId LIKE @FunctionId";
                parameters.Add("FunctionId", $"%{query.FunctionId}%");
            }

            if (!string.IsNullOrEmpty(query.FunctionName))
            {
                sql += " AND FunctionName LIKE @FunctionName";
                parameters.Add("FunctionName", $"%{query.FunctionName}%");
            }

            if (!string.IsNullOrEmpty(query.FunctionType))
            {
                sql += " AND FunctionType = @FunctionType";
                parameters.Add("FunctionType", query.FunctionType);
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
            _logger.LogError("查詢採購其他功能數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string functionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PurchaseOtherFunctions 
                WHERE FunctionId = @FunctionId";

            var count = await ExecuteScalarAsync<int>(sql, new { FunctionId = functionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購其他功能是否存在失敗: {functionId}", ex);
            throw;
        }
    }

    public async Task<ProcurementOther> CreateAsync(ProcurementOther procurementOther)
    {
        try
        {
            const string sql = @"
                INSERT INTO PurchaseOtherFunctions (
                    FunctionId, FunctionName, FunctionType, FunctionDesc, FunctionConfig,
                    Status, SeqNo, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @FunctionId, @FunctionName, @FunctionType, @FunctionDesc, @FunctionConfig,
                    @Status, @SeqNo, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                procurementOther.FunctionId,
                procurementOther.FunctionName,
                procurementOther.FunctionType,
                procurementOther.FunctionDesc,
                procurementOther.FunctionConfig,
                procurementOther.Status,
                procurementOther.SeqNo,
                procurementOther.Memo,
                procurementOther.CreatedBy,
                procurementOther.CreatedAt,
                procurementOther.UpdatedBy,
                procurementOther.UpdatedAt
            });

            procurementOther.TKey = tKey;
            return procurementOther;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購其他功能失敗: {procurementOther.FunctionId}", ex);
            throw;
        }
    }

    public async Task<ProcurementOther> UpdateAsync(ProcurementOther procurementOther)
    {
        try
        {
            const string sql = @"
                UPDATE PurchaseOtherFunctions SET
                    FunctionName = @FunctionName,
                    FunctionType = @FunctionType,
                    FunctionDesc = @FunctionDesc,
                    FunctionConfig = @FunctionConfig,
                    Status = @Status,
                    SeqNo = @SeqNo,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                procurementOther.TKey,
                procurementOther.FunctionName,
                procurementOther.FunctionType,
                procurementOther.FunctionDesc,
                procurementOther.FunctionConfig,
                procurementOther.Status,
                procurementOther.SeqNo,
                procurementOther.Memo,
                procurementOther.UpdatedBy,
                procurementOther.UpdatedAt
            });

            return procurementOther;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購其他功能失敗: {procurementOther.FunctionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM PurchaseOtherFunctions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購其他功能失敗: {tKey}", ex);
            throw;
        }
    }
}

