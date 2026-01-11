using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 項目權限 Repository 實作 (SYS0360)
/// </summary>
public class ItemPermissionRepository : BaseRepository, IItemPermissionRepository
{
    public ItemPermissionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<ItemPermissionDto>> QueryPermissionsAsync(string itemId, ItemPermissionQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    IP.TKey,
                    IP.ItemId,
                    IP.ProgramId,
                    IP.PageId,
                    IP.ButtonId,
                    IP.ButtonKey,
                    CS.SystemId,
                    CS.SystemName,
                    CSS.SubSystemId,
                    CSS.SubSystemName,
                    CP.ProgramName,
                    CB.ButtonName,
                    IP.CreatedBy,
                    IP.CreatedAt,
                    IP.UpdatedBy,
                    IP.UpdatedAt
                FROM ItemPermissions IP
                INNER JOIN ConfigButtons CB ON IP.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON IP.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE IP.ItemId = @ItemId";

            var parameters = new DynamicParameters();
            parameters.Add("ItemId", itemId);

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND CS.SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.MenuId))
            {
                sql += " AND CSS.SubSystemId = @MenuId";
                parameters.Add("MenuId", query.MenuId);
            }

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND CP.ProgramId = @ProgramId";
                parameters.Add("ProgramId", query.ProgramId);
            }

            if (query.ButtonKey.HasValue)
            {
                sql += " AND IP.ButtonKey = @ButtonKey";
                parameters.Add("ButtonKey", query.ButtonKey.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "IP.CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ItemPermissionDto>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*)
                FROM ItemPermissions IP
                INNER JOIN ConfigButtons CB ON IP.ButtonId = CB.ButtonId
                INNER JOIN ConfigPrograms CP ON IP.ProgramId = CP.ProgramId
                LEFT JOIN ConfigSubSystems CSS ON CP.SubSystemId = CSS.SubSystemId
                LEFT JOIN ConfigSystems CS ON CP.SystemId = CS.SystemId
                WHERE IP.ItemId = @ItemId";

            var countParameters = new DynamicParameters();
            countParameters.Add("ItemId", itemId);

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND CS.SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.MenuId))
            {
                countSql += " AND CSS.SubSystemId = @MenuId";
                countParameters.Add("MenuId", query.MenuId);
            }

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND CP.ProgramId = @ProgramId";
                countParameters.Add("ProgramId", query.ProgramId);
            }

            if (query.ButtonKey.HasValue)
            {
                countSql += " AND IP.ButtonKey = @ButtonKey";
                countParameters.Add("ButtonKey", query.ButtonKey.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ItemPermissionDto>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目權限列表失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemSystemDto>> GetSystemListAsync(string itemId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CS.SystemId,
                    CS.SystemName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalCount,
                    COUNT(DISTINCT IP.ButtonId) AS PermissionCount
                FROM ConfigSystems CS
                INNER JOIN ConfigPrograms CP ON CS.SystemId = CP.SystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN ItemPermissions IP ON IP.ItemId = @ItemId 
                    AND IP.ProgramId = CP.ProgramId 
                    AND IP.PageId = CB.PageId 
                    AND IP.ButtonId = CB.ButtonId
                GROUP BY CS.SystemId, CS.SystemName";

            var items = await QueryAsync<ItemSystemDto>(sql, new { ItemId = itemId });

            foreach (var item in items)
            {
                if (item.PermissionCount == 0)
                {
                    item.Status = "未選";
                }
                else if (item.PermissionCount == item.TotalCount)
                {
                    item.Status = "全選";
                }
                else
                {
                    item.Status = "部份";
                }
            }

            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目系統列表失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemMenuDto>> GetMenuListAsync(string itemId, string systemId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CSS.SubSystemId AS MenuId,
                    CSS.SubSystemName AS MenuName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalCount,
                    COUNT(DISTINCT IP.ButtonId) AS PermissionCount
                FROM ConfigSubSystems CSS
                INNER JOIN ConfigPrograms CP ON CSS.SubSystemId = CP.SubSystemId
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN ItemPermissions IP ON IP.ItemId = @ItemId 
                    AND IP.ProgramId = CP.ProgramId 
                    AND IP.PageId = CB.PageId 
                    AND IP.ButtonId = CB.ButtonId
                WHERE CP.SystemId = @SystemId
                GROUP BY CSS.SubSystemId, CSS.SubSystemName";

            var items = await QueryAsync<ItemMenuDto>(sql, new { ItemId = itemId, SystemId = systemId });

            foreach (var item in items)
            {
                if (item.PermissionCount == 0)
                {
                    item.Status = "未選";
                }
                else if (item.PermissionCount == item.TotalCount)
                {
                    item.Status = "全選";
                }
                else
                {
                    item.Status = "部份";
                }
            }

            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目選單列表失敗: {itemId} - {systemId}", ex);
            throw;
        }
    }

    public async Task<List<ItemProgramDto>> GetProgramListAsync(string itemId, string menuId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CP.ProgramId,
                    CP.ProgramName,
                    COUNT(DISTINCT CB.ButtonId) AS TotalCount,
                    COUNT(DISTINCT IP.ButtonId) AS PermissionCount
                FROM ConfigPrograms CP
                INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                LEFT JOIN ItemPermissions IP ON IP.ItemId = @ItemId 
                    AND IP.ProgramId = CP.ProgramId 
                    AND IP.PageId = CB.PageId 
                    AND IP.ButtonId = CB.ButtonId
                WHERE CP.SubSystemId = @MenuId
                GROUP BY CP.ProgramId, CP.ProgramName";

            var items = await QueryAsync<ItemProgramDto>(sql, new { ItemId = itemId, MenuId = menuId });

            foreach (var item in items)
            {
                if (item.PermissionCount == 0)
                {
                    item.Status = "未選";
                }
                else if (item.PermissionCount == item.TotalCount)
                {
                    item.Status = "全選";
                }
                else
                {
                    item.Status = "部份";
                }
            }

            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目作業列表失敗: {itemId} - {menuId}", ex);
            throw;
        }
    }

    public async Task<List<ItemButtonDto>> GetButtonListAsync(string itemId, string programId)
    {
        try
        {
            const string sql = @"
                SELECT 
                    CB.TKey AS ButtonKey,
                    CB.ButtonId,
                    CB.ButtonName,
                    CASE WHEN IP.TKey IS NOT NULL THEN 1 ELSE 0 END AS IsAuthorized
                FROM ConfigButtons CB
                LEFT JOIN ItemPermissions IP ON IP.ItemId = @ItemId 
                    AND IP.ProgramId = @ProgramId 
                    AND IP.PageId = CB.PageId 
                    AND IP.ButtonId = CB.ButtonId
                WHERE CB.ProgramId = @ProgramId
                ORDER BY CB.ButtonId";

            var items = await QueryAsync<ItemButtonDto>(sql, new { ItemId = itemId, ProgramId = programId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目按鈕列表失敗: {itemId} - {programId}", ex);
            throw;
        }
    }

    public async Task<ItemPermission?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ItemPermissions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<ItemPermission>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目權限失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ItemPermission> CreateAsync(ItemPermission itemPermission)
    {
        try
        {
            const string sql = @"
                INSERT INTO ItemPermissions (
                    ItemId, ProgramId, PageId, ButtonId, ButtonKey,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ItemId, @ProgramId, @PageId, @ButtonId, @ButtonKey,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ItemPermission>(sql, itemPermission);
            if (result == null)
            {
                throw new InvalidOperationException("新增項目權限失敗");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增項目權限失敗: {itemPermission.ItemId}", ex);
            throw;
        }
    }

    public async Task<int> BatchCreateAsync(string itemId, List<ItemPermissionItem> items, string? createdBy)
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

                foreach (var item in items)
                {
                    // 檢查是否已存在
                    const string checkSql = @"
                        SELECT COUNT(*) FROM ItemPermissions 
                        WHERE ItemId = @ItemId 
                            AND ProgramId = @ProgramId 
                            AND PageId = @PageId 
                            AND ButtonId = @ButtonId";

                    var exists = await connection.QuerySingleAsync<int>(checkSql, new
                    {
                        ItemId = itemId,
                        item.ProgramId,
                        item.PageId,
                        item.ButtonId
                    }, transaction);

                    if (exists > 0)
                    {
                        continue;
                    }

                    const string insertSql = @"
                        INSERT INTO ItemPermissions (
                            ItemId, ProgramId, PageId, ButtonId, ButtonKey,
                            CreatedBy, CreatedAt
                        )
                        VALUES (
                            @ItemId, @ProgramId, @PageId, @ButtonId, @ButtonKey,
                            @CreatedBy, @CreatedAt
                        )";

                    await connection.ExecuteAsync(insertSql, new
                    {
                        ItemId = itemId,
                        item.ProgramId,
                        item.PageId,
                        item.ButtonId,
                        item.ButtonKey,
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
            _logger.LogError($"批量新增項目權限失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<ItemPermission> UpdateAsync(ItemPermission itemPermission)
    {
        try
        {
            const string sql = @"
                UPDATE ItemPermissions SET
                    ProgramId = @ProgramId,
                    PageId = @PageId,
                    ButtonId = @ButtonId,
                    ButtonKey = @ButtonKey,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<ItemPermission>(sql, itemPermission);
            if (result == null)
            {
                throw new InvalidOperationException($"項目權限不存在: {itemPermission.TKey}");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改項目權限失敗: {itemPermission.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"DELETE FROM ItemPermissions WHERE TKey = @TKey";
            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除項目權限失敗: {tKey}", ex);
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

            const string sql = @"DELETE FROM ItemPermissions WHERE TKey IN @TKeys";
            return await ExecuteAsync(sql, new { TKeys = tKeys });
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除項目權限失敗", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsBySystemAsync(string itemId, string systemId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var count = 0;

                if (isAuthorized)
                {
                    // 授予權限：為該系統下的所有按鈕新增權限
                    const string insertSql = @"
                        INSERT INTO ItemPermissions (ItemId, ProgramId, PageId, ButtonId, ButtonKey, CreatedBy, CreatedAt)
                        SELECT 
                            @ItemId,
                            CP.ProgramId,
                            CB.PageId,
                            CB.ButtonId,
                            CB.TKey,
                            @CreatedBy,
                            @CreatedAt
                        FROM ConfigSystems CS
                        INNER JOIN ConfigPrograms CP ON CS.SystemId = CP.SystemId
                        INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                        WHERE CS.SystemId = @SystemId
                            AND NOT EXISTS (
                                SELECT 1 FROM ItemPermissions IP
                                WHERE IP.ItemId = @ItemId
                                    AND IP.ProgramId = CP.ProgramId
                                    AND IP.PageId = CB.PageId
                                    AND IP.ButtonId = CB.ButtonId
                            )";

                    count = await connection.ExecuteAsync(insertSql, new
                    {
                        ItemId = itemId,
                        SystemId = systemId,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }, transaction);
                }
                else
                {
                    // 撤銷權限：刪除該系統下的所有權限
                    const string deleteSql = @"
                        DELETE IP
                        FROM ItemPermissions IP
                        INNER JOIN ConfigButtons CB ON IP.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE IP.ItemId = @ItemId
                            AND CP.SystemId = @SystemId";

                    count = await connection.ExecuteAsync(deleteSql, new
                    {
                        ItemId = itemId,
                        SystemId = systemId
                    }, transaction);
                }

                transaction.Commit();
                return count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目系統權限失敗: {itemId} - {systemId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsByMenuAsync(string itemId, string menuId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var count = 0;

                if (isAuthorized)
                {
                    const string insertSql = @"
                        INSERT INTO ItemPermissions (ItemId, ProgramId, PageId, ButtonId, ButtonKey, CreatedBy, CreatedAt)
                        SELECT 
                            @ItemId,
                            CP.ProgramId,
                            CB.PageId,
                            CB.ButtonId,
                            CB.TKey,
                            @CreatedBy,
                            @CreatedAt
                        FROM ConfigSubSystems CSS
                        INNER JOIN ConfigPrograms CP ON CSS.SubSystemId = CP.SubSystemId
                        INNER JOIN ConfigButtons CB ON CP.ProgramId = CB.ProgramId
                        WHERE CSS.SubSystemId = @MenuId
                            AND NOT EXISTS (
                                SELECT 1 FROM ItemPermissions IP
                                WHERE IP.ItemId = @ItemId
                                    AND IP.ProgramId = CP.ProgramId
                                    AND IP.PageId = CB.PageId
                                    AND IP.ButtonId = CB.ButtonId
                            )";

                    count = await connection.ExecuteAsync(insertSql, new
                    {
                        ItemId = itemId,
                        MenuId = menuId,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }, transaction);
                }
                else
                {
                    const string deleteSql = @"
                        DELETE IP
                        FROM ItemPermissions IP
                        INNER JOIN ConfigButtons CB ON IP.ButtonId = CB.ButtonId
                        INNER JOIN ConfigPrograms CP ON CB.ProgramId = CP.ProgramId
                        WHERE IP.ItemId = @ItemId
                            AND CP.SubSystemId = @MenuId";

                    count = await connection.ExecuteAsync(deleteSql, new
                    {
                        ItemId = itemId,
                        MenuId = menuId
                    }, transaction);
                }

                transaction.Commit();
                return count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目選單權限失敗: {itemId} - {menuId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsByProgramAsync(string itemId, string programId, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var count = 0;

                if (isAuthorized)
                {
                    const string insertSql = @"
                        INSERT INTO ItemPermissions (ItemId, ProgramId, PageId, ButtonId, ButtonKey, CreatedBy, CreatedAt)
                        SELECT 
                            @ItemId,
                            @ProgramId,
                            CB.PageId,
                            CB.ButtonId,
                            CB.TKey,
                            @CreatedBy,
                            @CreatedAt
                        FROM ConfigButtons CB
                        WHERE CB.ProgramId = @ProgramId
                            AND NOT EXISTS (
                                SELECT 1 FROM ItemPermissions IP
                                WHERE IP.ItemId = @ItemId
                                    AND IP.ProgramId = @ProgramId
                                    AND IP.PageId = CB.PageId
                                    AND IP.ButtonId = CB.ButtonId
                            )";

                    count = await connection.ExecuteAsync(insertSql, new
                    {
                        ItemId = itemId,
                        ProgramId = programId,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }, transaction);
                }
                else
                {
                    const string deleteSql = @"
                        DELETE FROM ItemPermissions
                        WHERE ItemId = @ItemId
                            AND ProgramId = @ProgramId";

                    count = await connection.ExecuteAsync(deleteSql, new
                    {
                        ItemId = itemId,
                        ProgramId = programId
                    }, transaction);
                }

                transaction.Commit();
                return count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目作業權限失敗: {itemId} - {programId}", ex);
            throw;
        }
    }

    public async Task<int> SetPermissionsByButtonAsync(string itemId, List<long> buttonKeys, bool isAuthorized, string? createdBy)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = DateTime.Now;
                var count = 0;

                if (isAuthorized)
                {
                    // 授予權限：根據 ButtonKey 查詢按鈕資訊並新增權限
                    const string insertSql = @"
                        INSERT INTO ItemPermissions (ItemId, ProgramId, PageId, ButtonId, ButtonKey, CreatedBy, CreatedAt)
                        SELECT 
                            @ItemId,
                            CB.ProgramId,
                            CB.PageId,
                            CB.ButtonId,
                            CB.TKey,
                            @CreatedBy,
                            @CreatedAt
                        FROM ConfigButtons CB
                        WHERE CB.TKey IN @ButtonKeys
                            AND NOT EXISTS (
                                SELECT 1 FROM ItemPermissions IP
                                WHERE IP.ItemId = @ItemId
                                    AND IP.ProgramId = CB.ProgramId
                                    AND IP.PageId = CB.PageId
                                    AND IP.ButtonId = CB.ButtonId
                            )";

                    count = await connection.ExecuteAsync(insertSql, new
                    {
                        ItemId = itemId,
                        ButtonKeys = buttonKeys,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }, transaction);
                }
                else
                {
                    // 撤銷權限：根據 ButtonKey 刪除權限
                    const string deleteSql = @"
                        DELETE FROM ItemPermissions
                        WHERE ItemId = @ItemId
                            AND ButtonKey IN @ButtonKeys";

                    count = await connection.ExecuteAsync(deleteSql, new
                    {
                        ItemId = itemId,
                        ButtonKeys = buttonKeys
                    }, transaction);
                }

                transaction.Commit();
                return count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定項目按鈕權限失敗: {itemId}", ex);
            throw;
        }
    }
}

