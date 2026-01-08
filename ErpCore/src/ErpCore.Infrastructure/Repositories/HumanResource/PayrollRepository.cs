using Dapper;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 薪資 Repository 實作 (SYSH210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PayrollRepository : BaseRepository, IPayrollRepository
{
    public PayrollRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Payroll?> GetByIdAsync(string payrollId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payrolls 
                WHERE PayrollId = @PayrollId";

            return await QueryFirstOrDefaultAsync<Payroll>(sql, new { PayrollId = payrollId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task<Payroll?> GetByEmployeeYearMonthAsync(string employeeId, int year, int month)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Payrolls 
                WHERE EmployeeId = @EmployeeId 
                AND PayrollYear = @PayrollYear 
                AND PayrollMonth = @PayrollMonth";

            return await QueryFirstOrDefaultAsync<Payroll>(sql, new 
            { 
                EmployeeId = employeeId,
                PayrollYear = year,
                PayrollMonth = month
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢薪資失敗: {employeeId}/{year}/{month}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Payroll>> QueryAsync(PayrollQuery query)
    {
        try
        {
            var sql = @"
                SELECT p.*, e.EmployeeName 
                FROM Payrolls p
                LEFT JOIN Employees e ON p.EmployeeId = e.EmployeeId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND p.EmployeeId LIKE @EmployeeId";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (query.PayrollYear.HasValue)
            {
                sql += " AND p.PayrollYear = @PayrollYear";
                parameters.Add("PayrollYear", query.PayrollYear.Value);
            }

            if (query.PayrollMonth.HasValue)
            {
                sql += " AND p.PayrollMonth = @PayrollMonth";
                parameters.Add("PayrollMonth", query.PayrollMonth.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND p.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "p.PayrollYear DESC, p.PayrollMonth DESC" : $"p.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Payroll>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Payrolls p
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND p.EmployeeId LIKE @EmployeeId";
                countParameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }
            if (query.PayrollYear.HasValue)
            {
                countSql += " AND p.PayrollYear = @PayrollYear";
                countParameters.Add("PayrollYear", query.PayrollYear.Value);
            }
            if (query.PayrollMonth.HasValue)
            {
                countSql += " AND p.PayrollMonth = @PayrollMonth";
                countParameters.Add("PayrollMonth", query.PayrollMonth.Value);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND p.Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Payroll>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢薪資列表失敗", ex);
            throw;
        }
    }

    public async Task<Payroll> CreateAsync(Payroll payroll)
    {
        try
        {
            // 生成薪資編號
            if (string.IsNullOrEmpty(payroll.PayrollId))
            {
                payroll.PayrollId = $"PAY{payroll.EmployeeId}{payroll.PayrollYear:0000}{payroll.PayrollMonth:00}";
            }

            // 計算總薪資
            payroll.TotalSalary = payroll.BaseSalary + payroll.Allowance + payroll.Bonus - payroll.Deduction;

            const string sql = @"
                INSERT INTO Payrolls (
                    PayrollId, EmployeeId, PayrollYear, PayrollMonth,
                    BaseSalary, Allowance, Bonus, Deduction, TotalSalary,
                    Status, PayDate, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @PayrollId, @EmployeeId, @PayrollYear, @PayrollMonth,
                    @BaseSalary, @Allowance, @Bonus, @Deduction, @TotalSalary,
                    @Status, @PayDate, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, new
            {
                payroll.PayrollId,
                payroll.EmployeeId,
                payroll.PayrollYear,
                payroll.PayrollMonth,
                payroll.BaseSalary,
                payroll.Allowance,
                payroll.Bonus,
                payroll.Deduction,
                payroll.TotalSalary,
                payroll.Status,
                payroll.PayDate,
                payroll.Notes,
                payroll.CreatedBy,
                payroll.CreatedAt,
                payroll.UpdatedBy,
                payroll.UpdatedAt
            });

            return payroll;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增薪資失敗: {payroll.PayrollId}", ex);
            throw;
        }
    }

    public async Task<Payroll> UpdateAsync(Payroll payroll)
    {
        try
        {
            // 重新計算總薪資
            payroll.TotalSalary = payroll.BaseSalary + payroll.Allowance + payroll.Bonus - payroll.Deduction;

            const string sql = @"
                UPDATE Payrolls SET
                    BaseSalary = @BaseSalary,
                    Allowance = @Allowance,
                    Bonus = @Bonus,
                    Deduction = @Deduction,
                    TotalSalary = @TotalSalary,
                    Status = @Status,
                    PayDate = @PayDate,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE PayrollId = @PayrollId";

            await ExecuteAsync(sql, new
            {
                payroll.PayrollId,
                payroll.BaseSalary,
                payroll.Allowance,
                payroll.Bonus,
                payroll.Deduction,
                payroll.TotalSalary,
                payroll.Status,
                payroll.PayDate,
                payroll.Notes,
                payroll.UpdatedBy,
                payroll.UpdatedAt
            });

            return payroll;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改薪資失敗: {payroll.PayrollId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string payrollId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Payrolls
                WHERE PayrollId = @PayrollId";

            await ExecuteAsync(sql, new { PayrollId = payrollId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除薪資失敗: {payrollId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string employeeId, int year, int month)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Payrolls
                WHERE EmployeeId = @EmployeeId 
                AND PayrollYear = @PayrollYear 
                AND PayrollMonth = @PayrollMonth";

            var count = await QuerySingleAsync<int>(sql, new 
            { 
                EmployeeId = employeeId,
                PayrollYear = year,
                PayrollMonth = month
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查薪資是否存在失敗: {employeeId}/{year}/{month}", ex);
            throw;
        }
    }
}

