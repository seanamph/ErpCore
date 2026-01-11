using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者儲位權限 Repository 實作 (SYS0111)
/// </summary>
public class UserWarehouseAreaRepository : BaseRepository, IUserWarehouseAreaRepository
{
    public UserWarehouseAreaRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserWarehouseArea>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserWarehouseAreas 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserWarehouseArea>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者儲位權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserWarehouseArea> CreateAsync(UserWarehouseArea userWarehouseArea)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserWarehouseAreas (
                    UserId, WareaId1, WareaId2, WareaId3, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @WareaId1, @WareaId2, @WareaId3, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserWarehouseArea>(sql, userWarehouseArea);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者儲位權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者儲位權限失敗: {userWarehouseArea.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserWarehouseArea> userWarehouseAreas)
    {
        try
        {
            if (userWarehouseAreas == null || userWarehouseAreas.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserWarehouseAreas (
                    UserId, WareaId1, WareaId2, WareaId3, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @WareaId1, @WareaId2, @WareaId3, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userWarehouseAreas);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者儲位權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserWarehouseAreas WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者儲位權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserWarehouseAreas WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除儲位權限失敗: {id}", ex);
            throw;
        }
    }
}
