using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者業種權限 Repository 實作 (SYS0111)
/// </summary>
public class UserBusinessTypeRepository : BaseRepository, IUserBusinessTypeRepository
{
    public UserBusinessTypeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserBusinessType>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserBusinessTypes 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserBusinessType>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者業種權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserBusinessType> CreateAsync(UserBusinessType userBusinessType)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserBusinessTypes (
                    UserId, BtypeMId, BtypeId, PtypeId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @BtypeMId, @BtypeId, @PtypeId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserBusinessType>(sql, userBusinessType);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者業種權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者業種權限失敗: {userBusinessType.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserBusinessType> userBusinessTypes)
    {
        try
        {
            if (userBusinessTypes == null || userBusinessTypes.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserBusinessTypes (
                    UserId, BtypeMId, BtypeId, PtypeId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @BtypeMId, @BtypeId, @PtypeId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userBusinessTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者業種權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserBusinessTypes WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者業種權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserBusinessTypes WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業種權限失敗: {id}", ex);
            throw;
        }
    }
}
