using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 組別 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class GroupRepository : BaseRepository, IGroupRepository
{
    public GroupRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Group?> GetByIdAsync(string groupId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Groups 
                WHERE GroupId = @GroupId";

            return await QueryFirstOrDefaultAsync<Group>(sql, new { GroupId = groupId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢組別失敗: {groupId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Group>> QueryAsync(GroupQuery query)
    {
        try
        {
            var sql = @"
                SELECT g.* 
                FROM Groups g
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.GroupId))
            {
                sql += " AND g.GroupId LIKE @GroupId";
                parameters.Add("GroupId", $"%{query.GroupId}%");
            }

            if (!string.IsNullOrEmpty(query.GroupName))
            {
                sql += " AND g.GroupName LIKE @GroupName";
                parameters.Add("GroupName", $"%{query.GroupName}%");
            }

            if (!string.IsNullOrEmpty(query.DeptId))
            {
                sql += " AND g.DeptId = @DeptId";
                parameters.Add("DeptId", query.DeptId);
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND g.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND g.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "g.GroupId" : $"g.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Group>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Groups g
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.GroupId))
            {
                countSql += " AND g.GroupId LIKE @GroupId";
                countParameters.Add("GroupId", $"%{query.GroupId}%");
            }
            if (!string.IsNullOrEmpty(query.GroupName))
            {
                countSql += " AND g.GroupName LIKE @GroupName";
                countParameters.Add("GroupName", $"%{query.GroupName}%");
            }
            if (!string.IsNullOrEmpty(query.DeptId))
            {
                countSql += " AND g.DeptId = @DeptId";
                countParameters.Add("DeptId", query.DeptId);
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND g.OrgId = @OrgId";
                countParameters.Add("OrgId", query.OrgId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND g.Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Group>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢組別列表失敗", ex);
            throw;
        }
    }

    public async Task<Group> CreateAsync(Group group)
    {
        try
        {
            const string sql = @"
                INSERT INTO Groups (
                    GroupId, GroupName, DeptId, OrgId, SeqNo, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @GroupId, @GroupName, @DeptId, @OrgId, @SeqNo, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Group>(sql, group);
            if (result == null)
            {
                throw new InvalidOperationException("新增組別失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增組別失敗: {group.GroupId}", ex);
            throw;
        }
    }

    public async Task<Group> UpdateAsync(Group group)
    {
        try
        {
            const string sql = @"
                UPDATE Groups SET
                    GroupName = @GroupName,
                    DeptId = @DeptId,
                    OrgId = @OrgId,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE GroupId = @GroupId";

            var result = await QueryFirstOrDefaultAsync<Group>(sql, group);
            if (result == null)
            {
                throw new InvalidOperationException($"組別不存在: {group.GroupId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改組別失敗: {group.GroupId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string groupId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Groups 
                WHERE GroupId = @GroupId";

            await ExecuteAsync(sql, new { GroupId = groupId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除組別失敗: {groupId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string groupId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Groups 
                WHERE GroupId = @GroupId";

            var count = await QuerySingleAsync<int>(sql, new { GroupId = groupId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查組別是否存在失敗: {groupId}", ex);
            throw;
        }
    }
}

