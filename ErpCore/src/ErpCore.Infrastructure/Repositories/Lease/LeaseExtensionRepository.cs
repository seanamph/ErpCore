using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Lease;

/// <summary>
/// 租賃擴展 Repository 實作 (SYS8A10-SYS8A45)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LeaseExtensionRepository : BaseRepository, ILeaseExtensionRepository
{
    public LeaseExtensionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LeaseExtension?> GetByIdAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseExtensions 
                WHERE ExtensionId = @ExtensionId";

            return await QueryFirstOrDefaultAsync<LeaseExtension>(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseExtension>> QueryAsync(LeaseExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LeaseExtensions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                sql += " AND ExtensionId LIKE @ExtensionId";
                parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY ExtensionId DESC";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LeaseExtension>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LeaseExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LeaseExtensions 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                sql += " AND ExtensionId LIKE @ExtensionId";
                parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }

            if (!string.IsNullOrEmpty(query.LeaseId))
            {
                sql += " AND LeaseId = @LeaseId";
                parameters.Add("LeaseId", query.LeaseId);
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
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
            _logger.LogError("查詢租賃擴展數量失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseExtension>> GetByLeaseIdAsync(string leaseId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LeaseExtensions 
                WHERE LeaseId = @LeaseId
                ORDER BY SeqNo, ExtensionId";

            return await QueryAsync<LeaseExtension>(sql, new { LeaseId = leaseId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢擴展列表失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM LeaseExtensions 
                WHERE ExtensionId = @ExtensionId";

            var count = await ExecuteScalarAsync<int>(sql, new { ExtensionId = extensionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃擴展是否存在失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<LeaseExtension> CreateAsync(LeaseExtension extension)
    {
        try
        {
            const string sql = @"
                INSERT INTO LeaseExtensions (
                    ExtensionId, LeaseId, ExtensionType, ExtensionName, ExtensionValue,
                    StartDate, EndDate, Status, SeqNo, Memo,
                    SiteId, OrgId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @ExtensionId, @LeaseId, @ExtensionType, @ExtensionName, @ExtensionValue,
                    @StartDate, @EndDate, @Status, @SeqNo, @Memo,
                    @SiteId, @OrgId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await ExecuteScalarAsync<long>(sql, new
            {
                extension.ExtensionId,
                extension.LeaseId,
                extension.ExtensionType,
                extension.ExtensionName,
                extension.ExtensionValue,
                extension.StartDate,
                extension.EndDate,
                extension.Status,
                extension.SeqNo,
                extension.Memo,
                extension.SiteId,
                extension.OrgId,
                extension.CreatedBy,
                extension.CreatedAt,
                extension.UpdatedBy,
                extension.UpdatedAt
            });

            extension.TKey = tKey;
            return extension;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃擴展失敗: {extension.ExtensionId}", ex);
            throw;
        }
    }

    public async Task<LeaseExtension> UpdateAsync(LeaseExtension extension)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseExtensions SET
                    LeaseId = @LeaseId,
                    ExtensionType = @ExtensionType,
                    ExtensionName = @ExtensionName,
                    ExtensionValue = @ExtensionValue,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Status = @Status,
                    SeqNo = @SeqNo,
                    Memo = @Memo,
                    SiteId = @SiteId,
                    OrgId = @OrgId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ExtensionId = @ExtensionId";

            await ExecuteAsync(sql, new
            {
                extension.ExtensionId,
                extension.LeaseId,
                extension.ExtensionType,
                extension.ExtensionName,
                extension.ExtensionValue,
                extension.StartDate,
                extension.EndDate,
                extension.Status,
                extension.SeqNo,
                extension.Memo,
                extension.SiteId,
                extension.OrgId,
                extension.UpdatedBy,
                extension.UpdatedAt
            });

            return extension;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃擴展失敗: {extension.ExtensionId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LeaseExtensions 
                WHERE ExtensionId = @ExtensionId";

            await ExecuteAsync(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string extensionId, string status)
    {
        try
        {
            const string sql = @"
                UPDATE LeaseExtensions SET
                    Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE ExtensionId = @ExtensionId";

            await ExecuteAsync(sql, new { ExtensionId = extensionId, Status = status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃擴展狀態失敗: {extensionId}, Status: {status}", ex);
            throw;
        }
    }
}

