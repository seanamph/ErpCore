using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者組織權限 Repository 實作 (SYS0114)
/// </summary>
public class UserOrganizationRepository : BaseRepository, IUserOrganizationRepository
{
    public UserOrganizationRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserOrganization>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserOrganizations 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserOrganization>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者組織權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserOrganization> CreateAsync(UserOrganization userOrganization)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserOrganizations (
                    UserId, OrgId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @OrgId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserOrganization>(sql, userOrganization);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者組織權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者組織權限失敗: {userOrganization.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserOrganization> userOrganizations)
    {
        try
        {
            if (userOrganizations == null || userOrganizations.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserOrganizations (
                    UserId, OrgId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @OrgId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userOrganizations);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者組織權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserOrganizations WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者組織權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserOrganizations WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除組織權限失敗: {id}", ex);
            throw;
        }
    }
}
