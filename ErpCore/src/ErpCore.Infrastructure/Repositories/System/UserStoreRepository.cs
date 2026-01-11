using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者7X承租分店權限 Repository 實作 (SYS0111)
/// </summary>
public class UserStoreRepository : BaseRepository, IUserStoreRepository
{
    public UserStoreRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserStore>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserStores 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserStore>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者7X承租分店權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserStore> CreateAsync(UserStore userStore)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserStores (
                    UserId, StoreId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @StoreId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserStore>(sql, userStore);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者7X承租分店權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者7X承租分店權限失敗: {userStore.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserStore> userStores)
    {
        try
        {
            if (userStores == null || userStores.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserStores (
                    UserId, StoreId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @StoreId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userStores);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者7X承租分店權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserStores WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者7X承租分店權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserStores WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除7X承租分店權限失敗: {id}", ex);
            throw;
        }
    }
}
