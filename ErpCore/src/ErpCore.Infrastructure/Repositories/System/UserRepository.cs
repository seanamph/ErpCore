using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Users 
                WHERE UserId = @UserId";

            return await QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<User>> QueryAsync(UserQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Users
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                sql += " AND UserName LIKE @UserName";
                parameters.Add("UserName", $"%{query.UserName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.UserType))
            {
                sql += " AND UserType = @UserType";
                parameters.Add("UserType", query.UserType);
            }

            if (!string.IsNullOrEmpty(query.Title))
            {
                sql += " AND Title LIKE @Title";
                parameters.Add("Title", $"%{query.Title}%");
            }

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                sql += " AND ShopId = @ShopId";
                parameters.Add("ShopId", query.ShopId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "UserId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<User>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Users
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.UserId))
            {
                countSql += " AND UserId LIKE @UserId";
                countParameters.Add("UserId", $"%{query.UserId}%");
            }
            if (!string.IsNullOrEmpty(query.UserName))
            {
                countSql += " AND UserName LIKE @UserName";
                countParameters.Add("UserName", $"%{query.UserName}%");
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND OrgId = @OrgId";
                countParameters.Add("OrgId", query.OrgId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (!string.IsNullOrEmpty(query.UserType))
            {
                countSql += " AND UserType = @UserType";
                countParameters.Add("UserType", query.UserType);
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                countSql += " AND Title LIKE @Title";
                countParameters.Add("Title", $"%{query.Title}%");
            }
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                countSql += " AND ShopId = @ShopId";
                countParameters.Add("ShopId", query.ShopId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<User>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者列表失敗", ex);
            throw;
        }
    }

    public async Task<User> CreateAsync(User user)
    {
        try
        {
            const string sql = @"
                INSERT INTO Users (
                    UserId, UserName, UserPassword, Title, OrgId, StartDate, EndDate, Status, UserType, Notes,
                    UserPriority, ShopId, LoginCount, ChangePwdDate, FloorId, AreaId, BtypeId, StoreId,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @UserId, @UserName, @UserPassword, @Title, @OrgId, @StartDate, @EndDate, @Status, @UserType, @Notes,
                    @UserPriority, @ShopId, @LoginCount, @ChangePwdDate, @FloorId, @AreaId, @BtypeId, @StoreId,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<User>(sql, user);
            if (result == null)
            {
                throw new InvalidOperationException("新增使用者失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者失敗: {user.UserId}", ex);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        try
        {
            const string sql = @"
                UPDATE Users SET
                    UserName = @UserName,
                    UserPassword = @UserPassword,
                    Title = @Title,
                    OrgId = @OrgId,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Status = @Status,
                    UserType = @UserType,
                    Notes = @Notes,
                    UserPriority = @UserPriority,
                    ShopId = @ShopId,
                    FloorId = @FloorId,
                    AreaId = @AreaId,
                    BtypeId = @BtypeId,
                    StoreId = @StoreId,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE UserId = @UserId";

            var result = await QueryFirstOrDefaultAsync<User>(sql, user);
            if (result == null)
            {
                throw new InvalidOperationException($"使用者不存在: {user.UserId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者失敗: {user.UserId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string userId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Users 
                WHERE UserId = @UserId";

            await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Users 
                WHERE UserId = @UserId";

            var count = await QuerySingleAsync<int>(sql, new { UserId = userId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查使用者是否存在失敗: {userId}", ex);
            throw;
        }
    }

    public async Task UpdatePasswordAsync(string userId, string hashedPassword)
    {
        try
        {
            const string sql = @"
                UPDATE Users SET
                    UserPassword = @UserPassword,
                    ChangePwdDate = @ChangePwdDate,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId";

            await ExecuteAsync(sql, new
            {
                UserId = userId,
                UserPassword = hashedPassword,
                ChangePwdDate = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新密碼失敗: {userId}", ex);
            throw;
        }
    }

    public async Task UpdateLoginInfoAsync(string userId, string ipAddress)
    {
        try
        {
            const string sql = @"
                UPDATE Users SET
                    LastLoginDate = @LastLoginDate,
                    LastLoginIp = @LastLoginIp,
                    LoginCount = ISNULL(LoginCount, 0) + 1,
                    UpdatedAt = @UpdatedAt
                WHERE UserId = @UserId";

            await ExecuteAsync(sql, new
            {
                UserId = userId,
                LastLoginDate = DateTime.Now,
                LastLoginIp = ipAddress,
                UpdatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新登入資訊失敗: {userId}", ex);
            throw;
        }
    }

    public async Task ResetAllPasswordsAsync(string hashedPassword, string updatedBy)
    {
        try
        {
            const string sql = @"
                UPDATE Users SET
                    UserPassword = @UserPassword,
                    ChangePwdDate = @ChangePwdDate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt";

            await ExecuteAsync(sql, new
            {
                UserPassword = hashedPassword,
                ChangePwdDate = DateTime.Now,
                UpdatedBy = updatedBy,
                UpdatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("重置所有使用者密碼失敗", ex);
            throw;
        }
    }
}

