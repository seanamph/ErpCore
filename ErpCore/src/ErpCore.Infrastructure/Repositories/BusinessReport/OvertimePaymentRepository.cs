using Dapper;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 加班發放 Repository 實作 (SYSL510)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class OvertimePaymentRepository : BaseRepository, IOvertimePaymentRepository
{
    public OvertimePaymentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<OvertimePayment?> GetByIdAsync(long id)
    {
        try
        {
            const string sql = @"
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
                    op.Notes,
                    op.CreatedBy,
                    op.CreatedAt,
                    op.UpdatedBy,
                    op.UpdatedAt
                FROM OvertimePayments op
                LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId
                LEFT JOIN Users u ON op.ApprovedBy = u.UserId
                WHERE op.Id = @Id";

            return await QueryFirstOrDefaultAsync<OvertimePayment>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢加班發放失敗: {id}", ex);
            throw;
        }
    }

    public async Task<OvertimePayment?> GetByPaymentNoAsync(string paymentNo)
    {
        try
        {
            const string sql = @"
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
                    op.Notes,
                    op.CreatedBy,
                    op.CreatedAt,
                    op.UpdatedBy,
                    op.UpdatedAt
                FROM OvertimePayments op
                LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId
                LEFT JOIN Users u ON op.ApprovedBy = u.UserId
                WHERE op.PaymentNo = @PaymentNo";

            return await QueryFirstOrDefaultAsync<OvertimePayment>(sql, new { PaymentNo = paymentNo });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢加班發放失敗: {paymentNo}", ex);
            throw;
        }
    }

    public async Task<PagedResult<OvertimePayment>> QueryAsync(OvertimePaymentQuery query)
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
                    op.Notes,
                    op.CreatedBy,
                    op.CreatedAt,
                    op.UpdatedBy,
                    op.UpdatedAt
                FROM OvertimePayments op
                LEFT JOIN Departments d ON op.DepartmentId = d.DepartmentId
                LEFT JOIN Users u ON op.ApprovedBy = u.UserId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PaymentNo))
            {
                sql += " AND op.PaymentNo LIKE @PaymentNo";
                parameters.Add("PaymentNo", $"%{query.PaymentNo}%");
            }

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND op.EmployeeId = @EmployeeId";
                parameters.Add("EmployeeId", query.EmployeeId);
            }

            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                sql += " AND op.DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", query.DepartmentId);
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND op.StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom.Value);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND op.StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo.Value);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND op.EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom.Value);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND op.EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND op.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 計算總筆數
            var countSql = "SELECT COUNT(*) FROM OvertimePayments op WHERE 1=1";
            if (!string.IsNullOrEmpty(query.PaymentNo))
            {
                countSql += " AND op.PaymentNo LIKE @PaymentNo";
            }
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND op.EmployeeId = @EmployeeId";
            }
            if (!string.IsNullOrEmpty(query.DepartmentId))
            {
                countSql += " AND op.DepartmentId = @DepartmentId";
            }
            if (query.StartDateFrom.HasValue)
            {
                countSql += " AND op.StartDate >= @StartDateFrom";
            }
            if (query.StartDateTo.HasValue)
            {
                countSql += " AND op.StartDate <= @StartDateTo";
            }
            if (query.EndDateFrom.HasValue)
            {
                countSql += " AND op.EndDate >= @EndDateFrom";
            }
            if (query.EndDateTo.HasValue)
            {
                countSql += " AND op.EndDate <= @EndDateTo";
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

            var items = (await QueryAsync<OvertimePayment>(sql, parameters)).ToList();

            return new PagedResult<OvertimePayment>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢加班發放列表失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(OvertimePayment overtimePayment)
    {
        try
        {
            const string sql = @"
                INSERT INTO OvertimePayments 
                (PaymentNo, PaymentDate, EmployeeId, EmployeeName, DepartmentId, OvertimeHours, OvertimeAmount, 
                 StartDate, EndDate, Status, ApprovedBy, ApprovedAt, Notes, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@PaymentNo, @PaymentDate, @EmployeeId, @EmployeeName, @DepartmentId, @OvertimeHours, @OvertimeAmount, 
                 @StartDate, @EndDate, @Status, @ApprovedBy, @ApprovedAt, @Notes, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, overtimePayment);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增加班發放失敗", ex);
            throw;
        }
    }

    public async Task UpdateAsync(OvertimePayment overtimePayment)
    {
        try
        {
            const string sql = @"
                UPDATE OvertimePayments SET
                    PaymentDate = @PaymentDate,
                    EmployeeId = @EmployeeId,
                    EmployeeName = @EmployeeName,
                    DepartmentId = @DepartmentId,
                    OvertimeHours = @OvertimeHours,
                    OvertimeAmount = @OvertimeAmount,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Status = @Status,
                    ApprovedBy = @ApprovedBy,
                    ApprovedAt = @ApprovedAt,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            await ExecuteAsync(sql, overtimePayment);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改加班發放失敗: {overtimePayment.Id}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            const string sql = "DELETE FROM OvertimePayments WHERE Id = @Id";
            await ExecuteAsync(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除加班發放失敗: {id}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsByPaymentNoAsync(string paymentNo)
    {
        try
        {
            const string sql = "SELECT COUNT(*) FROM OvertimePayments WHERE PaymentNo = @PaymentNo";
            var count = await QuerySingleAsync<int>(sql, new { PaymentNo = paymentNo });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查發放單號是否存在失敗: {paymentNo}", ex);
            throw;
        }
    }
}

