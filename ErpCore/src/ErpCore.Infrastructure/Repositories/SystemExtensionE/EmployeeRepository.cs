using System.Data;
using Dapper;
using ErpCore.Domain.Entities.SystemExtensionE;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionE;

/// <summary>
/// 員工 Repository 實作 (SYSPE10-SYSPE11 - 員工資料維護)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmployeeRepository : BaseRepository, IEmployeeRepository
{
    public EmployeeRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Employee?> GetByIdAsync(string employeeId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Employees 
                WHERE EmployeeId = @EmployeeId";

            return await QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeId = employeeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢員工失敗: {employeeId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Employee>> QueryAsync(EmployeeQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Employees
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND EmployeeId LIKE @EmployeeId";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (!string.IsNullOrEmpty(query.EmployeeName))
            {
                sql += " AND EmployeeName LIKE @EmployeeName";
                parameters.Add("EmployeeName", $"%{query.EmployeeName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.PositionId))
            {
                sql += " AND PositionId = @PositionId";
                parameters.Add("PositionId", query.PositionId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<Employee>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EmployeeQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Employees
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND EmployeeId LIKE @EmployeeId";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (!string.IsNullOrEmpty(query.EmployeeName))
            {
                sql += " AND EmployeeName LIKE @EmployeeName";
                parameters.Add("EmployeeName", $"%{query.EmployeeName}%");
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.PositionId))
            {
                sql += " AND PositionId = @PositionId";
                parameters.Add("PositionId", query.PositionId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(Employee entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Employees (
                    EmployeeId, EmployeeName, IdNumber, DepartmentId, PositionId,
                    HireDate, ResignDate, Status, Email, Phone, Address,
                    BirthDate, Gender, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                )
                VALUES (
                    @EmployeeId, @EmployeeName, @IdNumber, @DepartmentId, @PositionId,
                    @HireDate, @ResignDate, @Status, @Email, @Phone, @Address,
                    @BirthDate, @Gender, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, entity);
            return entity.EmployeeId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增員工失敗: {entity.EmployeeId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Employee entity)
    {
        try
        {
            const string sql = @"
                UPDATE Employees SET
                    EmployeeName = @EmployeeName,
                    IdNumber = @IdNumber,
                    DepartmentId = @DepartmentId,
                    PositionId = @PositionId,
                    HireDate = @HireDate,
                    ResignDate = @ResignDate,
                    Status = @Status,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    BirthDate = @BirthDate,
                    Gender = @Gender,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE EmployeeId = @EmployeeId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員工失敗: {entity.EmployeeId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string employeeId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Employees 
                WHERE EmployeeId = @EmployeeId";

            await ExecuteAsync(sql, new { EmployeeId = employeeId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除員工失敗: {employeeId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string employeeId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Employees 
                WHERE EmployeeId = @EmployeeId";

            var count = await ExecuteScalarAsync<int>(sql, new { EmployeeId = employeeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查員工是否存在失敗: {employeeId}", ex);
            throw;
        }
    }
}

