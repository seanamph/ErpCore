using System.Data;
using Dapper;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.MirModule;

/// <summary>
/// MIRH000 人事 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MirH000PersonnelRepository : BaseRepository, IMirH000PersonnelRepository
{
    public MirH000PersonnelRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<MirH000Personnel?> GetByIdAsync(string personnelId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM MirH000Personnel 
                WHERE PersonnelId = @PersonnelId";

            return await QueryFirstOrDefaultAsync<MirH000Personnel>(sql, new { PersonnelId = personnelId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事資料失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MirH000Personnel>> QueryAsync(MirH000PersonnelQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM MirH000Personnel 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId LIKE @PersonnelId";
                parameters.Add("PersonnelId", $"%{query.PersonnelId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelName))
            {
                sql += " AND PersonnelName LIKE @PersonnelName";
                parameters.Add("PersonnelName", $"%{query.PersonnelName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            sql += " ORDER BY PersonnelId";
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (query.PageIndex - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<MirH000Personnel>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(MirH000PersonnelQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM MirH000Personnel 
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PersonnelId))
            {
                sql += " AND PersonnelId LIKE @PersonnelId";
                parameters.Add("PersonnelId", $"%{query.PersonnelId}%");
            }

            if (!string.IsNullOrEmpty(query.PersonnelName))
            {
                sql += " AND PersonnelName LIKE @PersonnelName";
                parameters.Add("PersonnelName", $"%{query.PersonnelName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(MirH000Personnel entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO MirH000Personnel 
                (PersonnelId, PersonnelName, DepartmentId, PositionId, HireDate, ResignDate, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@PersonnelId, @PersonnelName, @DepartmentId, @PositionId, @HireDate, @ResignDate, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var tKey = await QuerySingleAsync<long>(sql, entity);
            return entity.PersonnelId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增人事資料失敗: {entity.PersonnelId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(MirH000Personnel entity)
    {
        try
        {
            const string sql = @"
                UPDATE MirH000Personnel 
                SET PersonnelName = @PersonnelName,
                    DepartmentId = @DepartmentId,
                    PositionId = @PositionId,
                    HireDate = @HireDate,
                    ResignDate = @ResignDate,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PersonnelId = @PersonnelId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改人事資料失敗: {entity.PersonnelId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string personnelId)
    {
        try
        {
            const string sql = @"
                DELETE FROM MirH000Personnel 
                WHERE PersonnelId = @PersonnelId";

            await ExecuteAsync(sql, new { PersonnelId = personnelId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除人事資料失敗: {personnelId}", ex);
            throw;
        }
    }
}

