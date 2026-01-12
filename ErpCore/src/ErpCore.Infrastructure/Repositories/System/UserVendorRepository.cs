using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者廠商權限 Repository 實作 (SYS0113)
/// </summary>
public class UserVendorRepository : BaseRepository, IUserVendorRepository
{
    public UserVendorRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserVendor>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserVendors 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserVendor>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者廠商權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserVendor> CreateAsync(UserVendor userVendor)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserVendors (
                    UserId, VendorId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @VendorId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserVendor>(sql, userVendor);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者廠商權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者廠商權限失敗: {userVendor.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserVendor> userVendors)
    {
        try
        {
            if (userVendors == null || userVendors.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserVendors (
                    UserId, VendorId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @VendorId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userVendors);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者廠商權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserVendors WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者廠商權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserVendors WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除廠商權限失敗: {id}", ex);
            throw;
        }
    }
}
