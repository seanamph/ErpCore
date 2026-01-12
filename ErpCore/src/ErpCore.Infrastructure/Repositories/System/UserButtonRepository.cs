using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者按鈕權限 Repository 實作 (SYS0113)
/// </summary>
public class UserButtonRepository : BaseRepository, IUserButtonRepository
{
    public UserButtonRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserButton>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserButtons 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserButton>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者按鈕權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserButton> CreateAsync(UserButton userButton)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserButtons (
                    UserId, ButtonId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @ButtonId, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<UserButton>(sql, userButton);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者按鈕權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者按鈕權限失敗: {userButton.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserButton> userButtons)
    {
        try
        {
            if (userButtons == null || userButtons.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserButtons (
                    UserId, ButtonId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                )
                VALUES (
                    @UserId, @ButtonId, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                )";

            await ExecuteAsync(sql, userButtons);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者按鈕權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserButtons WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者按鈕權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM UserButtons WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除按鈕權限失敗: {tKey}", ex);
            throw;
        }
    }
}
