using Dapper;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 加班發放報表 Repository 實作 (SYSL510)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class OvertimePaymentReportRepository : BaseRepository, IOvertimePaymentReportRepository
{
    public OvertimePaymentReportRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PagedResult<OvertimePaymentReportEntity>> QueryReportAsync(OvertimePaymentReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    op.Id,
                    op.PaymentNo,
                    op.PaymentDate,
                    op.EmployeeId,
                    op.EmployeeName,
                    op.DepartmentId,
                    d.DepartmentName,
                    op.OvertimeHours,
                    op.OvertimeAmount,
                    op.StartDate,
                    op.EndDate,
                    op.Status,
                    op.ApprovedBy,
                    u.UserName AS ApprovedByName,
                    op.ApprovedAt,
                    op.Notes
                FROM OvertimePayments op
                LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId
                LEFT JOIN Users u ON op.ApprovedBy = u.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.StartDate.HasValue)
            {
                sql += " AND op.PaymentDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND op.PaymentDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND op.DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND (op.EmployeeId LIKE @EmployeeId OR op.EmployeeName LIKE @EmployeeId)";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND op.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM OvertimePayments op WHERE 1=1";
            if (query.StartDate.HasValue)
            {
                countSql += " AND op.PaymentDate >= @StartDate";
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND op.PaymentDate <= @EndDate";
            }
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                countSql += " AND op.DepartmentId = @DepartmentId";
            }
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND (op.EmployeeId LIKE @EmployeeId OR op.EmployeeName LIKE @EmployeeId)";
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND op.Status = @Status";
            }

            var totalCount = await QuerySingleAsync<int>(countSql, parameters);

            // 排序
            var sortField = query.SortField ?? "PaymentDate";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY op.{sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = (await QueryAsync<OvertimePaymentReportEntity>(sql, parameters)).ToList();

            return new PagedResult<OvertimePaymentReportEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢加班發放報表失敗", ex);
            throw;
        }
    }

    public async Task<OvertimePaymentReportSummary> GetSummaryAsync(OvertimePaymentReportQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS TotalCount,
                    SUM(op.OvertimeHours) AS TotalOvertimeHours,
                    SUM(op.OvertimeAmount) AS TotalOvertimeAmount
                FROM OvertimePayments op
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (query.StartDate.HasValue)
            {
                sql += " AND op.PaymentDate >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND op.PaymentDate <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND op.DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND (op.EmployeeId LIKE @EmployeeId OR op.EmployeeName LIKE @EmployeeId)";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND op.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var result = await QuerySingleAsync<OvertimePaymentReportSummary>(sql, parameters);
            return result ?? new OvertimePaymentReportSummary();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢加班發放報表統計資訊失敗", ex);
            throw;
        }
    }
}

