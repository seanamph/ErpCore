using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者總公司/分店權限 Repository 實作 (SYS0113)
/// </summary>
public class UserShopRepository : BaseRepository, IUserShopRepository
{
    public UserShopRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserShop>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserShops 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserShop>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者總公司/分店權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserShop> CreateAsync(UserShop userShop)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserShops (
                    UserId, PShopId, ShopId, SiteId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @PShopId, @ShopId, @SiteId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserShop>(sql, userShop);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者總公司/分店權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者總公司/分店權限失敗: {userShop.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserShop> userShops)
    {
        try
        {
            if (userShops == null || userShops.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserShops (
                    UserId, PShopId, ShopId, SiteId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @PShopId, @ShopId, @SiteId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userShops);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者總公司/分店權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserShops WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者總公司/分店權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserShops WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除總公司/分店權限失敗: {id}", ex);
            throw;
        }
    }
}
