using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色權限 Repository 實作 (SYS0310)
/// </summary>
public class RolePermissionRepository : BaseRepository, IRolePermissionRepository
{
    public RolePermissionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<RolePermissionDto>> QueryPermissionsAsync(string roleId, RolePermissionQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    RB.TKey,
                    RB.RoleId,
                    RB.ButtonId,
                    CS.SystemId,
                    CS.SystemName,
                    CSS.SubSystemId,
                    CSS.SubSystemName,
                    CP.ProgramId,
                    CP.ProgramName,
                    CB.ButtonName,
                    RB.CreatedBy,
                    RB.CreatedAt
                FROM RoleButtons RB
                INNER JOIN ConfigButtons CB ON RB.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE RB.RoleId = @RoleId";

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", roleId);

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
            var sortField = string.IsNullOrEmpty(query.SortField) ? "RB.CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<RolePermissionDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM RoleButtons RB
                INNER JOIN ConfigButtons CB ON RB.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE RB.RoleId = @RoleId";

            var countParameters = new DynamicParameters();
            countParameters.Add("RoleId", roleId);

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

            return new PagedResult<RolePermissionDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色權限列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RolePermissionStatsDto>> GetSystemStatsAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CS.SystemId,
                    CS.SystemName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalButtons,
                    COUNT(DISTINCT RB.ButtonId) AS AuthorizedButtons
                FROM ConfigSystems CS
                INNER JOIN ConfigPrograms CP ON CS.SystemId = CP.SystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN RoleButtons RB ON RB.RoleId = @RoleId AND RB.ButtonId = CB.ButtonId
                WHERE CS.Status = 'A' AND CP.Status = 'A' AND CB.Status = 'A'
                GROUP BY CS.SystemId, CS.SystemName";

            var results = await QueryAsync<dynamic>(sql, new { RoleId = roleId });

            return results.Select(x => new RolePermissionStatsDto
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
            _logger.LogError($"查詢角色權限統計失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<RoleButton?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM RoleButtons 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<RoleButton>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色權限失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<RoleButton> CreateAsync(RoleButton roleButton)
    {
        try
        {
            const string sql = @"
                INSERT INTO RoleButtons (
                    RoleId, ButtonId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @RoleId, @ButtonId, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<RoleButton>(sql, roleButton);
            if (result == null)
            {
                throw new InvalidOperationException("新增角色權限失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增角色權限失敗: {roleButton.RoleId} - {roleButton.ButtonId}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(string roleId, List<string> buttonIds, string? createdBy)
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
                        SELECT COUNT(*) FROM RoleButtons 
                        WHERE RoleId = @RoleId AND ButtonId = @ButtonId";

                    var exists = await connection.QuerySingleAsync<int>(checkSql, new { RoleId = roleId, ButtonId = buttonId }, transaction);
                    if (exists > 0)
                    {
                        continue;
                    }

                    const string insertSql = @"
                        INSERT INTO RoleButtons (
                            RoleId, ButtonId, CreatedBy, CreatedAt
                        )
                        VALUES (
                            @RoleId, @ButtonId, @CreatedBy, @CreatedAt
                        )";

                    await connection.ExecuteAsync(insertSql, new
                    {
                        RoleId = roleId,
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
            _logger.LogError($"批量新增角色權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<RoleButton> UpdateAsync(RoleButton roleButton)
    {
        try
        {
            const string sql = @"
                UPDATE RoleButtons SET
                    ButtonId = @ButtonId
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<RoleButton>(sql, roleButton);
            if (result == null)
            {
                throw new InvalidOperationException($"角色權限不存在: {roleButton.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改角色權限失敗: {roleButton.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM RoleButtons WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色權限失敗: {tKey}", ex);
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

            const string sql = @"DELETE FROM RoleButtons WHERE TKey IN @TKeys";
            return await ExecuteAsync(sql, new { TKeys = tKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除角色權限失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string roleId, string buttonId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM RoleButtons 
                WHERE RoleId = @RoleId AND ButtonId = @ButtonId";

            var count = await QuerySingleAsync<int>(sql, new { RoleId = roleId, ButtonId = buttonId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查角色權限是否存在失敗: {roleId} - {buttonId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsBySystemAsync(string roleId, string systemId, bool isAuthorized, string? createdBy)
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
                            SELECT COUNT(*) FROM RoleButtons 
                            WHERE RoleId = @RoleId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { RoleId = roleId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO RoleButtons (RoleId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@RoleId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            RoleId = roleId,
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
                        DELETE RB FROM RoleButtons RB
                        INNER JOIN ConfigButtons CB ON RB.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE RB.RoleId = @RoleId AND CP.SystemId = @SystemId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { RoleId = roleId, SystemId = systemId }, transaction);

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
            _logger.LogError($"設定角色系統權限失敗: {roleId} - {systemId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsBySubSystemAsync(string roleId, string subSystemId, bool isAuthorized, string? createdBy)
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
                            SELECT COUNT(*) FROM RoleButtons 
                            WHERE RoleId = @RoleId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { RoleId = roleId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO RoleButtons (RoleId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@RoleId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            RoleId = roleId,
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
                        DELETE RB FROM RoleButtons RB
                        INNER JOIN ConfigButtons CB ON RB.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE RB.RoleId = @RoleId AND CP.SubSystemId = @SubSystemId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { RoleId = roleId, SubSystemId = subSystemId }, transaction);

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
            _logger.LogError($"設定角色子系統權限失敗: {roleId} - {subSystemId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsByProgramAsync(string roleId, string programId, bool isAuthorized, string? createdBy)
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
                            SELECT COUNT(*) FROM RoleButtons 
                            WHERE RoleId = @RoleId AND ButtonId = @ButtonId";

                        var exists = await connection.QuerySingleAsync<int>(checkSql, new { RoleId = roleId, ButtonId = buttonId }, transaction);
                        if (exists > 0)
                        {
                            continue;
                        }

                        const string insertSql = @"
                            INSERT INTO RoleButtons (RoleId, ButtonId, CreatedBy, CreatedAt)
                            VALUES (@RoleId, @ButtonId, @CreatedBy, @CreatedAt)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            RoleId = roleId,
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
                        DELETE RB FROM RoleButtons RB
                        INNER JOIN ConfigButtons CB ON RB.ButtonId = CB.ButtonId
                        WHERE RB.RoleId = @RoleId AND CB.ProgramId = @ProgramId";

                    var deletedCount = await connection.ExecuteAsync(deleteSql, new { RoleId = roleId, ProgramId = programId }, transaction);

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
            _logger.LogError($"設定角色作業權限失敗: {roleId} - {programId}", ex);
            throw;
        }
    }

    public async Task DeleteByRoleIdAsync(string roleId, System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = "DELETE FROM RoleButtons WHERE RoleId = @RoleId";
            
            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, new { RoleId = roleId }, transaction);
            }
            else
            {
                await ExecuteAsync(sql, new { RoleId = roleId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色權限失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<int> CopyFromRoleAsync(string sourceRoleId, string targetRoleId, string createdBy, System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                INSERT INTO RoleButtons (RoleId, ButtonId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup)
                SELECT @TargetRoleId, ButtonId, @CreatedBy, GETDATE(),
                       (SELECT CreatedPriority FROM Users WHERE UserId = @CreatedBy),
                       (SELECT CreatedGroup FROM Users WHERE UserId = @CreatedBy)
                FROM RoleButtons
                WHERE RoleId = @SourceRoleId";

            var parameters = new
            {
                SourceRoleId = sourceRoleId,
                TargetRoleId = targetRoleId,
                CreatedBy = createdBy
            };

            if (transaction != null)
            {
                return await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                return await ExecuteAsync(sql, parameters);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製角色權限失敗: {sourceRoleId} -> {targetRoleId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<RoleButton>> GetByRoleIdAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM RoleButtons 
                WHERE RoleId = @RoleId
                ORDER BY CreatedAt DESC";

            return await QueryAsync<RoleButton>(sql, new { RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色權限列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleSystemListDto>> GetRoleSystemsAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CS.SystemId,
                    CS.SystemName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalButtons,
                    COUNT(DISTINCT CASE WHEN RB.ButtonId IS NOT NULL THEN CB.ButtonId END) AS AuthorizedButtons
                FROM ConfigSystems CS
                INNER JOIN ConfigPrograms CP ON CS.SystemId = CP.SystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId AND RB.RoleId = @RoleId
                WHERE CS.Status = 'A' AND CP.Status = 'A' AND CB.Status = 'A'
                GROUP BY CS.SystemId, CS.SystemName
                ORDER BY CS.SystemId";

            var results = await QueryAsync<dynamic>(sql, new { RoleId = roleId });
            
            return results.Select(x => new RoleSystemListDto
            {
                SystemId = x.SystemId,
                SystemName = x.SystemName ?? string.Empty,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.TotalButtons > 0 && x.AuthorizedButtons == x.TotalButtons,
                AuthorizedRate = x.TotalButtons > 0 ? (double)x.AuthorizedButtons / x.TotalButtons : 0
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色系統列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleMenuListDto>> GetRoleMenusAsync(string roleId, string? systemId = null)
    {
        try
        {
            var sql = @"
                SELECT 
                    CSS.SubSystemId AS MenuId,
                    CSS.SubSystemName AS MenuName,
                    CS.SystemId,
                    COUNT(DISTINCT CB.ButtonId) AS TotalButtons,
                    COUNT(DISTINCT CASE WHEN RB.ButtonId IS NOT NULL THEN CB.ButtonId END) AS AuthorizedButtons
                FROM ConfigSubSystems CSS
                INNER JOIN ConfigPrograms CP ON CSS.SubSystemId = CP.SubSystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                INNER JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                LEFT JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId AND RB.RoleId = @RoleId
                WHERE CSS.Status = 'A' AND CP.Status = 'A' AND CB.Status = 'A' AND CS.Status = 'A'";

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", roleId);

            if (!string.IsNullOrEmpty(systemId))
            {
                sql += " AND CS.SystemId = @SystemId";
                parameters.Add("SystemId", systemId);
            }

            sql += " GROUP BY CSS.SubSystemId, CSS.SubSystemName, CS.SystemId ORDER BY CSS.SubSystemId";

            var results = await QueryAsync<dynamic>(sql, parameters);
            
            return results.Select(x => new RoleMenuListDto
            {
                MenuId = x.MenuId,
                MenuName = x.MenuName ?? string.Empty,
                SystemId = x.SystemId,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.TotalButtons > 0 && x.AuthorizedButtons == x.TotalButtons
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色選單列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleProgramListDto>> GetRoleProgramsAsync(string roleId, string? menuId = null)
    {
        try
        {
            var sql = @"
                SELECT 
                    CP.ProgramId,
                    CP.ProgramName,
                    CP.SubSystemId AS MenuId,
                    COUNT(DISTINCT CB.ButtonId) AS TotalButtons,
                    COUNT(DISTINCT CASE WHEN RB.ButtonId IS NOT NULL THEN CB.ButtonId END) AS AuthorizedButtons
                FROM ConfigPrograms CP
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId AND RB.RoleId = @RoleId
                WHERE CP.Status = 'A' AND CB.Status = 'A'";

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", roleId);

            if (!string.IsNullOrEmpty(menuId))
            {
                sql += " AND CP.SubSystemId = @MenuId";
                parameters.Add("MenuId", menuId);
            }

            sql += " GROUP BY CP.ProgramId, CP.ProgramName, CP.SubSystemId ORDER BY CP.ProgramId";

            var results = await QueryAsync<dynamic>(sql, parameters);
            
            return results.Select(x => new RoleProgramListDto
            {
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName ?? string.Empty,
                MenuId = x.MenuId,
                TotalButtons = x.TotalButtons,
                AuthorizedButtons = x.AuthorizedButtons,
                IsFullyAuthorized = x.TotalButtons > 0 && x.AuthorizedButtons == x.TotalButtons
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色作業列表失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<List<RoleButtonListDto>> GetRoleButtonsAsync(string roleId, string? programId = null)
    {
        try
        {
            var sql = @"
                SELECT 
                    CB.ButtonId,
                    CB.ButtonName,
                    CB.ProgramId,
                    CB.Funs,
                    CB.PageId,
                    CASE WHEN RB.ButtonId IS NOT NULL THEN 1 ELSE 0 END AS IsAuthorized
                FROM ConfigButtons CB
                LEFT JOIN RoleButtons RB ON CB.ButtonId = RB.ButtonId AND RB.RoleId = @RoleId
                WHERE CB.Status = 'A'";

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", roleId);

            if (!string.IsNullOrEmpty(programId))
            {
                sql += " AND CB.ProgramId = @ProgramId";
                parameters.Add("ProgramId", programId);
            }

            sql += " ORDER BY CB.ProgramId, CB.ButtonId";

            var results = await QueryAsync<dynamic>(sql, parameters);
            
            return results.Select(x => new RoleButtonListDto
            {
                ButtonId = x.ButtonId,
                ButtonName = x.ButtonName ?? string.Empty,
                ProgramId = x.ProgramId,
                Funs = x.Funs,
                PageId = x.PageId,
                IsAuthorized = x.IsAuthorized == 1
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色按鈕列表失敗: {roleId}", ex);
            throw;
        }
    }
}

