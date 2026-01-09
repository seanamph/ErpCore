using System.Data;
using Dapper;
using ErpCore.Domain.Entities.InvoiceExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.InvoiceExtension;

/// <summary>
/// 電子發票擴展 Repository 實作
/// </summary>
public class EInvoiceExtensionRepository : BaseRepository, IEInvoiceExtensionRepository
{
    public EInvoiceExtensionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EInvoiceExtension?> GetByIdAsync(long extensionId)
    {
        try
        {
            const string sql = @"SELECT * FROM EInvoiceExtensions WHERE ExtensionId = @ExtensionId";
            return await QueryFirstOrDefaultAsync<EInvoiceExtension>(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢電子發票擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EInvoiceExtension>> QueryAsync(EInvoiceExtensionQuery query)
    {
        try
        {
            var sql = @"SELECT * FROM EInvoiceExtensions WHERE 1=1";
            var parameters = new DynamicParameters();

            if (query.InvoiceId.HasValue)
            {
                sql += " AND InvoiceId = @InvoiceId";
                parameters.Add("InvoiceId", query.InvoiceId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            sql += " ORDER BY CreatedAt DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<EInvoiceExtension>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EInvoiceExtensionQuery query)
    {
        try
        {
            var sql = @"SELECT COUNT(*) FROM EInvoiceExtensions WHERE 1=1";
            var parameters = new DynamicParameters();

            if (query.InvoiceId.HasValue)
            {
                sql += " AND InvoiceId = @InvoiceId";
                parameters.Add("InvoiceId", query.InvoiceId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢電子發票擴展數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(EInvoiceExtension entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO EInvoiceExtensions 
                (InvoiceId, ExtensionType, ExtensionData, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@InvoiceId, @ExtensionType, @ExtensionData, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.QueryFirstOrDefaultAsync<long>(sql, entity);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增電子發票擴展失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(EInvoiceExtension entity)
    {
        try
        {
            const string sql = @"
                UPDATE EInvoiceExtensions 
                SET ExtensionType = @ExtensionType, ExtensionData = @ExtensionData, 
                    UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE ExtensionId = @ExtensionId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改電子發票擴展失敗: {entity.ExtensionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long extensionId)
    {
        try
        {
            const string sql = @"DELETE FROM EInvoiceExtensions WHERE ExtensionId = @ExtensionId";
            await ExecuteAsync(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除電子發票擴展失敗: {extensionId}", ex);
            throw;
        }
    }
}

