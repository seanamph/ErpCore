using Dapper;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 員工 Repository 實作 (SYSH110)
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

    public async Task<PagedResult<Employee>> QueryAsync(EmployeeQuery query)
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
            var sortField = string.IsNullOrEmpty(query.SortField) ? "EmployeeId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Employee>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Employees
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND EmployeeId LIKE @EmployeeId";
                countParameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }
            if (!string.IsNullOrEmpty(query.EmployeeName))
            {
                countSql += " AND EmployeeName LIKE @EmployeeName";
                countParameters.Add("EmployeeName", $"%{query.EmployeeName}%");
            }
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                countSql += " AND DepartmentId = @DepartmentId";
                countParameters.Add("DepartmentId", query.DepartmentId);
            }
            if (!string.IsNullOrEmpty(query.PositionId))
            {
                countSql += " AND PositionId = @PositionId";
                countParameters.Add("PositionId", query.PositionId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Employee>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢員工列表失敗", ex);
            throw;
        }
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        try
        {
            const string sql = @"
                INSERT INTO Employees (
                    EmployeeId, EmployeeName, IdNumber, DepartmentId, PositionId,
                    HireDate, ResignDate, Status, Email, Phone, Address,
                    BirthDate, Gender, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @EmployeeId, @EmployeeName, @IdNumber, @DepartmentId, @PositionId,
                    @HireDate, @ResignDate, @Status, @Email, @Phone, @Address,
                    @BirthDate, @Gender, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, new
            {
                employee.EmployeeId,
                employee.EmployeeName,
                employee.IdNumber,
                employee.DepartmentId,
                employee.PositionId,
                employee.HireDate,
                employee.ResignDate,
                employee.Status,
                employee.Email,
                employee.Phone,
                employee.Address,
                employee.BirthDate,
                employee.Gender,
                employee.Notes,
                employee.CreatedBy,
                employee.CreatedAt,
                employee.UpdatedBy,
                employee.UpdatedAt
            });

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增員工失敗: {employee.EmployeeId}", ex);
            throw;
        }
    }

    public async Task<Employee> UpdateAsync(Employee employee)
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

            await ExecuteAsync(sql, new
            {
                employee.EmployeeId,
                employee.EmployeeName,
                employee.IdNumber,
                employee.DepartmentId,
                employee.PositionId,
                employee.HireDate,
                employee.ResignDate,
                employee.Status,
                employee.Email,
                employee.Phone,
                employee.Address,
                employee.BirthDate,
                employee.Gender,
                employee.Notes,
                employee.UpdatedBy,
                employee.UpdatedAt
            });

            return employee;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改員工失敗: {employee.EmployeeId}", ex);
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

            var count = await QuerySingleAsync<int>(sql, new { EmployeeId = employeeId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查員工是否存在失敗: {employeeId}", ex);
            throw;
        }
    }
}

