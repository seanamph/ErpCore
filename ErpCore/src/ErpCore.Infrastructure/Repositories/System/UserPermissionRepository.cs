using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者權限 Repository 實作 (SYS0320)
/// </summary>
public class UserPermissionRepository : BaseRepository, IUserPermissionRepository
{
    public UserPermissionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<UserPermissionDto>> QueryPermissionsAsync(string userId, UserPermissionQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    UB.TKey,
                    UB.UserId,
                    UB.ButtonId,
                    CS.SystemId,
                    CS.SystemName,
                    CSS.SubSystemId,
                    CSS.SubSystemName,
                    CP.ProgramId,
                    CP.ProgramName,
                    CB.ButtonName,
                    UB.CreatedBy,
                    UB.CreatedAt
                FROM UserButtons UB
                INNER JOIN ConfigButtons CB ON UB.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE UB.UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND CS.SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                sql += " AND CSS.SubSystemId = @SubSystemId";
                parameters.Add("SubSystemId", query.SubSystemId);
            }

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND CP.ProgramId = @ProgramId";
                parameters.Add("ProgramId", query.ProgramId);
            }

            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                sql += " AND CB.ButtonId LIKE @ButtonId";
                parameters.Add("ButtonId", $"%{query.ButtonId}%");
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "UB.CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<UserPermissionDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM UserButtons UB
                INNER JOIN ConfigButtons CB ON UB.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE UB.UserId = @UserId";

            var countParameters = new DynamicParameters();
            countParameters.Add("UserId", userId);

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND CS.SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                countSql += " AND CSS.SubSystemId = @SubSystemId";
                countParameters.Add("SubSystemId", query.SubSystemId);
            }

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND CP.ProgramId = @ProgramId";
                countParameters.Add("ProgramId", query.ProgramId);
            }

            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                countSql += " AND CB.ButtonId LIKE @ButtonId";
                countParameters.Add("ButtonId", $"%{query.ButtonId}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<UserPermissionDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<List<UserPermissionStatsDto>> GetSystemStatsAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CS.SystemId,
                    CS.SystemName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalButtons,
                    COUNT(DISTINCT UB.ButtonId) AS AuthorizedButtons
                FROM ConfigSystems CS
                INNER JOIN ConfigPrograms CP ON CS.SystemId = CP.SystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN UserButtons UB ON UB.UserId = @UserId AND UB.ButtonId = CB.ButtonId
                WHERE CS.Status = 'A' AND CP.Status = 'A' AND CB.Status = 'A'
                GROUP BY CS.SystemId, CS.SystemName";

            var results = await QueryAsync<dynamic>(sql, new { UserId = userId });

            return results.Select(x => new UserPermissionStatsDto
            {
                SystemId = x.SystemId,
                SystemName = x.SystemName,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.AuthorizedButtons == x.TotalButtons,
                AuthorizedRate = x.TotalButtons > 0 ? (double)x.AuthorizedButtons / x.TotalButtons : 0
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限統計失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserButton?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserButtons 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<UserButton>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限失敗: {tKey}", ex);
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
                throw new InvalidOperationException("新增使用者權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者權限失敗: {userButton.UserId} - {userButton.ButtonId}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(string userId, List<string> buttonIds, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var addedCount = 0;
                var now = DateTime.Now;

                foreach (var buttonId in buttonIds)
                {
                    // 檢查是否已存在
                    const string checkSql = @"
                        SELECT COUNT(*) FROM UserButtons 
                        WHERE UserId = @UserId AND ButtonId = @ButtonId";

                    var exists = await connection.QuerySingleAsync<int>(checkSql, new { UserId = userId, ButtonId = buttonId }, transaction);
                    if (exists > 0)
                    {
                        continue;
                    }

                    const string insertSql = @"
                        INSERT INTO UserButtons (
                            UserId, ButtonId, CreatedBy, CreatedAt
                        )
                        VALUES (
                            @UserId, @ButtonId, @CreatedBy, @CreatedAt
                        )";

                    await connection.ExecuteAsync(insertSql, new
                    {
                        UserId = userId,
                        ButtonId = buttonId,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }, transaction);

                    addedCount++;
                }

                transaction.Commit();
                return addedCount;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"批量新增使用者權限失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserButton> UpdateAsync(UserButton userButton)
    {
        try
        {
            const string sql = @"
                UPDATE UserButtons SET
                    ButtonId = @ButtonId
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<UserButton>(sql, userButton);
            if (result == null)
            {
                throw new InvalidOperationException($"使用者權限不存在: {userButton.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者權限失敗: {userButton.TKey}", ex);
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
            _logger.LogError($"刪除使用者權限失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteAsync(List<long> tKeys)
    {
        try
        {
            if (tKeys == null || tKeys.Count == 0)
            {
                return 0;
            }

            const string sql = @"DELETE FROM UserButtons WHERE TKey IN @TKeys";
            return await ExecuteAsync(sql, new { TKeys = tKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除使用者權限失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string userId, string buttonId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserButtons 
                WHERE UserId = @UserId AND ButtonId = @ButtonId";

            var count = await QuerySingleAsync<int>(sql, new { UserId = userId, ButtonId = buttonId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查使用者權限是否存在失敗: {userId} - {buttonId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsBySystemAsync(string userId, string systemId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 查詢該系統下所有按鈕
                const string getButtonsSql = @"
                    SELECT CB.ButtonId
                    FROM ConfigButtons CB
                    INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                    WHERE CP.SystemId = @SystemId AND CB.Status = 'A' AND CP.Status = 'A'";

                var buttonIds = (await connection.QueryAsync<string>(getButtonsSql, new { SystemId = systemId }, transaction)).ToList();

                if (isAuthorized)
                {
                    // 新增權限
                    var addedCount = 0;
                    var now = DateTime.Now;

                    foreach (var buttonId in buttonIds)
                    {
                        const string checkSql = @"
                            SELECT COUNT(*) FROM UserButtons 
                            WHERE UserId = @UserId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { UserId = userId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO UserButtons (UserId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@UserId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            UserId = userId,
                            ButtonId = buttonId,
                            CreatedBy = createdBy,
                            CreatedAt = now
                        }, transaction);

                        addedCount++;
                    }

                    transaction.Commit();
                    return addedCount;
                }
                else
                {
                    // 刪除權限
                    const string deleteSql = @"
                        DELETE UB FROM UserButtons UB
                        INNER JOIN ConfigButtons CB ON UB.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE UB.UserId = @UserId AND CP.SystemId = @SystemId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { UserId = userId, SystemId = systemId }, transaction);

                    transaction.Commit();
                    return deletedCount;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定使用者系統權限失敗: {userId} - {systemId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsBySubSystemAsync(string userId, string subSystemId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 查詢該子系統下所有按鈕
                const string getButtonsSql = @"
                    SELECT CB.ButtonId
                    FROM ConfigButtons CB
                    INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                    WHERE CP.SubSystemId = @SubSystemId AND CB.Status = 'A' AND CP.Status = 'A'";

                var buttonIds = (await connection.QueryAsync<string>(getButtonsSql, new { SubSystemId = subSystemId }, transaction)).ToList();

                if (isAuthorized)
                {
                    // 新增權限
                    var addedCount = 0;
                    var now = DateTime.Now;

                    foreach (var buttonId in buttonIds)
                    {
                        const string checkSql = @"
                            SELECT COUNT(*) FROM UserButtons 
                            WHERE UserId = @UserId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { UserId = userId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO UserButtons (UserId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@UserId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            UserId = userId,
                            ButtonId = buttonId,
                            CreatedBy = createdBy,
                            CreatedAt = now
                        }, transaction);

                        addedCount++;
                    }

                    transaction.Commit();
                    return addedCount;
                }
                else
                {
                    // 刪除權限
                    const string deleteSql = @"
                        DELETE UB FROM UserButtons UB
                        INNER JOIN ConfigButtons CB ON UB.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE UB.UserId = @UserId AND CP.SubSystemId = @SubSystemId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { UserId = userId, SubSystemId = subSystemId }, transaction);

                    transaction.Commit();
                    return deletedCount;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定使用者子系統權限失敗: {userId} - {subSystemId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsByProgramAsync(string userId, string programId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // 查詢該作業下所有按鈕
                const string getButtonsSql = @"
                    SELECT ButtonId FROM ConfigButtons
                    WHERE ProgramId = @ProgramId AND Status = 'A'";

                var buttonIds = (await connection.QueryAsync<string>(getButtonsSql, new { ProgramId = programId }, transaction)).ToList();

                if (isAuthorized)
                {
                    // 新增權限
                    var addedCount = 0;
                    var now = DateTime.Now;

                    foreach (var buttonId in buttonIds)
                    {
                        const string checkSql = @"
                            SELECT COUNT(*) FROM UserButtons 
                            WHERE UserId = @UserId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { UserId = userId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO UserButtons (UserId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@UserId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            UserId = userId,
                            ButtonId = buttonId,
                            CreatedBy = createdBy,
                            CreatedAt = now
                        }, transaction);

                        addedCount++;
                    }

                    transaction.Commit();
                    return addedCount;
                }
                else
                {
                    // 刪除權限
                    const string deleteSql = @"
                        DELETE UB FROM UserButtons UB
                        INNER JOIN ConfigButtons CB ON UB.ButtonId = CB.ButtonId
                        WHERE UB.UserId = @UserId AND CB.ProgramId = @ProgramId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { UserId = userId, ProgramId = programId }, transaction);

                    transaction.Commit();
                    return deletedCount;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定使用者作業權限失敗: {userId} - {programId}", ex);
            throw;
        }
    }

    public async Task<int> DeleteByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"DELETE FROM UserButtons WHERE UserId = @UserId";
            return await ExecuteAsync(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者所有直接權限失敗: {userId}", ex);
            throw;
        }
    }
}

