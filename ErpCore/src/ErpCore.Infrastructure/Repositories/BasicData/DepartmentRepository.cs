using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 部別 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class DepartmentRepository : BaseRepository, IDepartmentRepository
{
    public DepartmentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Department?> GetByIdAsync(string deptId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Departments 
                WHERE DeptId = @DeptId";

            var result = await QueryFirstOrDefaultAsync<Department>(sql, new { DeptId = deptId });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢部別失敗: {deptId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Department>> QueryAsync(DepartmentQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Departments d
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.DeptId))
            {
                sql += " AND DeptId LIKE @DeptId";
                parameters.Add("DeptId", $"%{query.DeptId}%");
            }

            if (!string.IsNullOrEmpty(query.DeptName))
            {
                sql += " AND DeptName LIKE @DeptName";
                parameters.Add("DeptName", $"%{query.DeptName}%");
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

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "d.DeptId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Department>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Departments
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.DeptId))
            {
                countSql += " AND DeptId LIKE @DeptId";
                countParameters.Add("DeptId", $"%{query.DeptId}%");
            }
            if (!string.IsNullOrEmpty(query.DeptName))
            {
                countSql += " AND DeptName LIKE @DeptName";
                countParameters.Add("DeptName", $"%{query.DeptName}%");
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

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Department>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢部別列表失敗", ex);
            throw;
        }
    }

    public async Task<Department> CreateAsync(Department department)
    {
        try
        {
            const string sql = @"
                INSERT INTO Departments (
                    DeptId, DeptName, OrgId, SeqNo, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @DeptId, @DeptName, @OrgId, @SeqNo, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Department>(sql, department);
            if (result == null)
            {
                throw new InvalidOperationException("新增部別失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增部別失敗: {department.DeptId}", ex);
            throw;
        }
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        try
        {
            const string sql = @"
                UPDATE Departments SET
                    DeptName = @DeptName,
                    OrgId = @OrgId,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE DeptId = @DeptId";

            var result = await QueryFirstOrDefaultAsync<Department>(sql, department);
            if (result == null)
            {
                throw new InvalidOperationException($"部別不存在: {department.DeptId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改部別失敗: {department.DeptId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string deptId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Departments 
                WHERE DeptId = @DeptId";

            await ExecuteAsync(sql, new { DeptId = deptId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除部別失敗: {deptId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string deptId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Departments 
                WHERE DeptId = @DeptId";

            var count = await QuerySingleAsync<int>(sql, new { DeptId = deptId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查部別是否存在失敗: {deptId}", ex);
            throw;
        }
    }
}
