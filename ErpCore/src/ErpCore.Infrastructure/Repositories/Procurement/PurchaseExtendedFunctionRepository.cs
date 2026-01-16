using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購擴展功能 Repository 實作 (SYSP610)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PurchaseExtendedFunctionRepository : BaseRepository, IPurchaseExtendedFunctionRepository
{
    public PurchaseExtendedFunctionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PurchaseExtendedFunction?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseExtendedFunctions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<PurchaseExtendedFunction>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedFunction?> GetByExtFunctionIdAsync(string extFunctionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PurchaseExtendedFunctions 
                WHERE ExtFunctionId = @ExtFunctionId";

            return await QueryFirstOrDefaultAsync<PurchaseExtendedFunction>(sql, new { ExtFunctionId = extFunctionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購擴展功能失敗: {extFunctionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<PurchaseExtendedFunction>> QueryAsync(PurchaseExtendedFunctionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PurchaseExtendedFunctions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtFunctionId))
            {
                sql += " AND ExtFunctionId LIKE @ExtFunctionId";
                parameters.Add("ExtFunctionId", $"%{query.ExtFunctionId}%");
            }

            if (!string.IsNullOrEmpty(query.ExtFunctionName))
            {
                sql += " AND ExtFunctionName LIKE @ExtFunctionName";
                parameters.Add("ExtFunctionName", $"%{query.ExtFunctionName}%");
            }

            if (!string.IsNullOrEmpty(query.ExtFunctionType))
            {
                sql += " AND ExtFunctionType = @ExtFunctionType";
                parameters.Add("ExtFunctionType", query.ExtFunctionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY SeqNo, ExtFunctionId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<PurchaseExtendedFunction>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購擴展功能列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(PurchaseExtendedFunctionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM PurchaseExtendedFunctions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtFunctionId))
            {
                sql += " AND ExtFunctionId LIKE @ExtFunctionId";
                parameters.Add("ExtFunctionId", $"%{query.ExtFunctionId}%");
            }

            if (!string.IsNullOrEmpty(query.ExtFunctionName))
            {
                sql += " AND ExtFunctionName LIKE @ExtFunctionName";
                parameters.Add("ExtFunctionName", $"%{query.ExtFunctionName}%");
            }

            if (!string.IsNullOrEmpty(query.ExtFunctionType))
            {
                sql += " AND ExtFunctionType = @ExtFunctionType";
                parameters.Add("ExtFunctionType", query.ExtFunctionType);
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
            _logger.LogError("查詢採購擴展功能數量失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string extFunctionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM PurchaseExtendedFunctions 
                WHERE ExtFunctionId = @ExtFunctionId";

            var count = await ExecuteScalarAsync<int>(sql, new { ExtFunctionId = extFunctionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購擴展功能是否存在失敗: {extFunctionId}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedFunction> CreateAsync(PurchaseExtendedFunction purchaseExtendedFunction)
    {
        try
        {
            const string sql = @"
                INSERT INTO PurchaseExtendedFunctions (
                    ExtFunctionId, ExtFunctionName, ExtFunctionType, ExtFunctionDesc, ExtFunctionConfig, ParameterConfig,
                    Status, SeqNo, Memo,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ExtFunctionId, @ExtFunctionName, @ExtFunctionType, @ExtFunctionDesc, @ExtFunctionConfig, @ParameterConfig,
                    @Status, @SeqNo, @Memo,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                purchaseExtendedFunction.ExtFunctionId,
                purchaseExtendedFunction.ExtFunctionName,
                purchaseExtendedFunction.ExtFunctionType,
                purchaseExtendedFunction.ExtFunctionDesc,
                purchaseExtendedFunction.ExtFunctionConfig,
                purchaseExtendedFunction.ParameterConfig,
                purchaseExtendedFunction.Status,
                purchaseExtendedFunction.SeqNo,
                purchaseExtendedFunction.Memo,
                purchaseExtendedFunction.CreatedBy,
                purchaseExtendedFunction.CreatedAt,
                purchaseExtendedFunction.UpdatedBy,
                purchaseExtendedFunction.UpdatedAt
            });

            purchaseExtendedFunction.TKey = tKey;
            return purchaseExtendedFunction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購擴展功能失敗: {purchaseExtendedFunction.ExtFunctionId}", ex);
            throw;
        }
    }

    public async Task<PurchaseExtendedFunction> UpdateAsync(PurchaseExtendedFunction purchaseExtendedFunction)
    {
        try
        {
            const string sql = @"
                UPDATE PurchaseExtendedFunctions SET
                    ExtFunctionName = @ExtFunctionName,
                    ExtFunctionType = @ExtFunctionType,
                    ExtFunctionDesc = @ExtFunctionDesc,
                    ExtFunctionConfig = @ExtFunctionConfig,
                    ParameterConfig = @ParameterConfig,
                    Status = @Status,
                    SeqNo = @SeqNo,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new
            {
                purchaseExtendedFunction.TKey,
                purchaseExtendedFunction.ExtFunctionName,
                purchaseExtendedFunction.ExtFunctionType,
                purchaseExtendedFunction.ExtFunctionDesc,
                purchaseExtendedFunction.ExtFunctionConfig,
                purchaseExtendedFunction.ParameterConfig,
                purchaseExtendedFunction.Status,
                purchaseExtendedFunction.SeqNo,
                purchaseExtendedFunction.Memo,
                purchaseExtendedFunction.UpdatedBy,
                purchaseExtendedFunction.UpdatedAt
            });

            return purchaseExtendedFunction;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購擴展功能失敗: {purchaseExtendedFunction.ExtFunctionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM PurchaseExtendedFunctions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購擴展功能失敗: {tKey}", ex);
            throw;
        }
    }
}
