using Dapper;
using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// EIP整合 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EipIntegrationRepository : BaseRepository, IEipIntegrationRepository
{
    public EipIntegrationRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EipIntegration?> GetByProgIdAndPageIdAsync(string progId, string pageId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EipIntegrations 
                WHERE ProgId = @ProgId AND PageId = @PageId";

            return await QueryFirstOrDefaultAsync<EipIntegration>(sql, new { ProgId = progId, PageId = pageId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢EIP整合設定失敗: {progId}/{pageId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EipIntegration>> QueryAsync(EipIntegrationQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EipIntegrations
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProgId))
            {
                sql += " AND ProgId LIKE @ProgId";
                parameters.Add("ProgId", $"%{query.ProgId}%");
            }

            if (!string.IsNullOrEmpty(query.PageId))
            {
                sql += " AND PageId LIKE @PageId";
                parameters.Add("PageId", $"%{query.PageId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ProgId, PageId";

            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EipIntegration>(sql, parameters);

            var countSql = @"
                SELECT COUNT(*) FROM EipIntegrations
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProgId))
            {
                countSql += " AND ProgId LIKE @ProgId";
                countParameters.Add("ProgId", $"%{query.ProgId}%");
            }
            if (!string.IsNullOrEmpty(query.PageId))
            {
                countSql += " AND PageId LIKE @PageId";
                countParameters.Add("PageId", $"%{query.PageId}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EipIntegration>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢EIP整合設定列表失敗", ex);
            throw;
        }
    }

    public async Task<EipIntegration> CreateAsync(EipIntegration integration)
    {
        try
        {
            const string sql = @"
                INSERT INTO EipIntegrations (
                    ProgId, PageId, EipUrl, Fid, SingleField, MultiField, DetailTable, MultiMSeqNo, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProgId, @PageId, @EipUrl, @Fid, @SingleField, @MultiField, @DetailTable, @MultiMSeqNo, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<EipIntegration>(sql, integration);
            if (result == null)
            {
                throw new InvalidOperationException("新增EIP整合設定失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增EIP整合設定失敗: {integration.ProgId}/{integration.PageId}", ex);
            throw;
        }
    }

    public async Task<EipIntegration> UpdateAsync(EipIntegration integration)
    {
        try
        {
            const string sql = @"
                UPDATE EipIntegrations SET
                    EipUrl = @EipUrl,
                    Fid = @Fid,
                    SingleField = @SingleField,
                    MultiField = @MultiField,
                    DetailTable = @DetailTable,
                    MultiMSeqNo = @MultiMSeqNo,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE IntegrationId = @IntegrationId";

            var result = await QueryFirstOrDefaultAsync<EipIntegration>(sql, integration);
            if (result == null)
            {
                throw new InvalidOperationException($"EIP整合設定不存在: {integration.IntegrationId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改EIP整合設定失敗: {integration.IntegrationId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long integrationId)
    {
        try
        {
            const string sql = @"
                DELETE FROM EipIntegrations 
                WHERE IntegrationId = @IntegrationId";

            await ExecuteAsync(sql, new { IntegrationId = integrationId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除EIP整合設定失敗: {integrationId}", ex);
            throw;
        }
    }

    public async Task<EipTransaction> CreateTransactionAsync(EipTransaction transaction)
    {
        try
        {
            const string sql = @"
                INSERT INTO EipTransactions (
                    IntegrationId, ProgId, PageId, FlowId, RequestData, ResponseData, Status, ErrorMessage,
                    CreatedBy, CreatedAt, UpdatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @IntegrationId, @ProgId, @PageId, @FlowId, @RequestData, @ResponseData, @Status, @ErrorMessage,
                    @CreatedBy, @CreatedAt, @UpdatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<EipTransaction>(sql, transaction);
            if (result == null)
            {
                throw new InvalidOperationException("新增EIP交易記錄失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增EIP交易記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<EipTransaction>> GetTransactionsAsync(string? progId, string? pageId, int pageIndex, int pageSize)
    {
        try
        {
            var sql = @"
                SELECT * FROM EipTransactions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(progId))
            {
                sql += " AND ProgId = @ProgId";
                parameters.Add("ProgId", progId);
            }

            if (!string.IsNullOrEmpty(pageId))
            {
                sql += " AND PageId = @PageId";
                parameters.Add("PageId", pageId);
            }

            sql += " ORDER BY CreatedAt DESC";

            var offset = (pageIndex - 1) * pageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<EipTransaction>(sql, parameters);

            var countSql = @"
                SELECT COUNT(*) FROM EipTransactions
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(progId))
            {
                countSql += " AND ProgId = @ProgId";
                countParameters.Add("ProgId", progId);
            }
            if (!string.IsNullOrEmpty(pageId))
            {
                countSql += " AND PageId = @PageId";
                countParameters.Add("PageId", pageId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EipTransaction>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢EIP交易記錄列表失敗", ex);
            throw;
        }
    }
}

