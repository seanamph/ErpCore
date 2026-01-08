using Dapper;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 部別查詢結果（包含組織名稱）
/// </summary>
internal class DepartmentWithOrgName
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? CreatedPriority { get; set; }
    public string? CreatedGroup { get; set; }
}

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

            return await QueryFirstOrDefaultAsync<Department>(sql, new { DeptId = deptId });
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
                SELECT d.*, o.OrgName 
                FROM Departments d
                LEFT JOIN Organizations o ON d.OrgId = o.OrgId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.DeptId))
            {
                sql += " AND d.DeptId LIKE @DeptId";
                parameters.Add("DeptId", $"%{query.DeptId}%");
            }

            if (!string.IsNullOrEmpty(query.DeptName))
            {
                sql += " AND d.DeptName LIKE @DeptName";
                parameters.Add("DeptName", $"%{query.DeptName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND d.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND d.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "d.DeptId" : $"d.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<DepartmentWithOrgName>(sql, parameters);
            
            // 轉換為 Department 實體
            var departments = items.Select(x => new Department
            {
                DeptId = x.DeptId,
                DeptName = x.DeptName,
                OrgId = x.OrgId,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                CreatedPriority = x.CreatedPriority,
                CreatedGroup = x.CreatedGroup
            });

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Departments d
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.DeptId))
            {
                countSql += " AND d.DeptId LIKE @DeptId";
                countParameters.Add("DeptId", $"%{query.DeptId}%");
            }
            if (!string.IsNullOrEmpty(query.DeptName))
            {
                countSql += " AND d.DeptName LIKE @DeptName";
                countParameters.Add("DeptName", $"%{query.DeptName}%");
            }
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                countSql += " AND d.OrgId = @OrgId";
                countParameters.Add("OrgId", query.OrgId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND d.Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            // 需要保存 OrgName 以便 Service 層使用
            // 由於 Department 實體不包含 OrgName，我們需要通過其他方式傳遞
            // 這裡先返回 Department，OrgName 將在 Service 層另外查詢
            return new PagedResult<Department>
            {
                Items = departments.ToList(),
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

