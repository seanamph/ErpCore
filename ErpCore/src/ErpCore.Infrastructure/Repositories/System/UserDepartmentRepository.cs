using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者部門權限 Repository 實作 (SYS0113)
/// </summary>
public class UserDepartmentRepository : BaseRepository, IUserDepartmentRepository
{
    public UserDepartmentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<List<UserDepartment>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserDepartments 
                WHERE UserId = @UserId";

            var result = await QueryAsync<UserDepartment>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者部門權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserDepartment> CreateAsync(UserDepartment userDepartment)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserDepartments (
                    UserId, DeptId, CreatedBy, CreatedAt
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @DeptId, @CreatedBy, @CreatedAt
                )";

            var result = await QueryFirstOrDefaultAsync<UserDepartment>(sql, userDepartment);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者部門權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者部門權限失敗: {userDepartment.UserId}", ex);
            throw;
        }
    }

    public async Task CreateBatchAsync(List<UserDepartment> userDepartments)
    {
        try
        {
            if (userDepartments == null || userDepartments.Count == 0)
                return;

            const string sql = @"
                INSERT INTO UserDepartments (
                    UserId, DeptId, CreatedBy, CreatedAt
                )
                VALUES (
                    @UserId, @DeptId, @CreatedBy, @CreatedAt
                )";

            await ExecuteAsync(sql, userDepartments);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次新增使用者部門權限失敗", ex);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserDepartments WHERE UserId = @UserId";
            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者部門權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = @"DELETE FROM UserDepartments WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除部門權限失敗: {id}", ex);
            throw;
        }
    }
}
