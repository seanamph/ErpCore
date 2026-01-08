using Dapper;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.HumanResource;

/// <summary>
/// 考勤 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class AttendanceRepository : BaseRepository, IAttendanceRepository
{
    public AttendanceRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Attendance?> GetByIdAsync(string attendanceId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Attendances 
                WHERE AttendanceId = @AttendanceId";

            return await QueryFirstOrDefaultAsync<Attendance>(sql, new { AttendanceId = attendanceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢考勤失敗: {attendanceId}", ex);
            throw;
        }
    }

    public async Task<Attendance?> GetByEmployeeDateAsync(string employeeId, DateTime date)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Attendances 
                WHERE EmployeeId = @EmployeeId 
                AND CAST(AttendanceDate AS DATE) = CAST(@AttendanceDate AS DATE)";

            return await QueryFirstOrDefaultAsync<Attendance>(sql, new 
            { 
                EmployeeId = employeeId,
                AttendanceDate = date.Date
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢考勤失敗: {employeeId}/{date:yyyy-MM-dd}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Attendance>> QueryAsync(AttendanceQuery query)
    {
        try
        {
            var sql = @"
                SELECT a.*, e.EmployeeName 
                FROM Attendances a
                LEFT JOIN Employees e ON a.EmployeeId = e.EmployeeId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                sql += " AND a.EmployeeId LIKE @EmployeeId";
                parameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND CAST(a.AttendanceDate AS DATE) >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value.Date);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND CAST(a.AttendanceDate AS DATE) <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND a.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "a.AttendanceDate DESC" : $"a.{query.SortField}";
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Attendance>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Attendances a
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.EmployeeId))
            {
                countSql += " AND a.EmployeeId LIKE @EmployeeId";
                countParameters.Add("EmployeeId", $"%{query.EmployeeId}%");
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND CAST(a.AttendanceDate AS DATE) >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value.Date);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND CAST(a.AttendanceDate AS DATE) <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND a.Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Attendance>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢考勤列表失敗", ex);
            throw;
        }
    }

    public async Task<Attendance> CreateAsync(Attendance attendance)
    {
        try
        {
            // 生成考勤編號
            if (string.IsNullOrEmpty(attendance.AttendanceId))
            {
                attendance.AttendanceId = $"ATT{attendance.EmployeeId}{attendance.AttendanceDate:yyyyMMdd}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
            }

            // 計算工作時數
            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                var timeSpan = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
                attendance.WorkHours = (decimal)timeSpan.TotalHours;
            }

            const string sql = @"
                INSERT INTO Attendances (
                    AttendanceId, EmployeeId, AttendanceDate,
                    CheckInTime, CheckOutTime, WorkHours, OvertimeHours,
                    LeaveType, LeaveHours, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt
                ) VALUES (
                    @AttendanceId, @EmployeeId, @AttendanceDate,
                    @CheckInTime, @CheckOutTime, @WorkHours, @OvertimeHours,
                    @LeaveType, @LeaveHours, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt
                )";

            await ExecuteAsync(sql, new
            {
                attendance.AttendanceId,
                attendance.EmployeeId,
                attendance.AttendanceDate,
                attendance.CheckInTime,
                attendance.CheckOutTime,
                attendance.WorkHours,
                attendance.OvertimeHours,
                attendance.LeaveType,
                attendance.LeaveHours,
                attendance.Status,
                attendance.Notes,
                attendance.CreatedBy,
                attendance.CreatedAt,
                attendance.UpdatedBy,
                attendance.UpdatedAt
            });

            return attendance;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增考勤失敗: {attendance.AttendanceId}", ex);
            throw;
        }
    }

    public async Task<Attendance> UpdateAsync(Attendance attendance)
    {
        try
        {
            // 重新計算工作時數
            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                var timeSpan = attendance.CheckOutTime.Value - attendance.CheckInTime.Value;
                attendance.WorkHours = (decimal)timeSpan.TotalHours;
            }

            const string sql = @"
                UPDATE Attendances SET
                    CheckInTime = @CheckInTime,
                    CheckOutTime = @CheckOutTime,
                    WorkHours = @WorkHours,
                    OvertimeHours = @OvertimeHours,
                    LeaveType = @LeaveType,
                    LeaveHours = @LeaveHours,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE AttendanceId = @AttendanceId";

            await ExecuteAsync(sql, new
            {
                attendance.AttendanceId,
                attendance.CheckInTime,
                attendance.CheckOutTime,
                attendance.WorkHours,
                attendance.OvertimeHours,
                attendance.LeaveType,
                attendance.LeaveHours,
                attendance.Status,
                attendance.Notes,
                attendance.UpdatedBy,
                attendance.UpdatedAt
            });

            return attendance;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改考勤失敗: {attendance.AttendanceId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string attendanceId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Attendances
                WHERE AttendanceId = @AttendanceId";

            await ExecuteAsync(sql, new { AttendanceId = attendanceId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除考勤失敗: {attendanceId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string employeeId, DateTime date)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Attendances
                WHERE EmployeeId = @EmployeeId 
                AND CAST(AttendanceDate AS DATE) = CAST(@AttendanceDate AS DATE)";

            var count = await QuerySingleAsync<int>(sql, new 
            { 
                EmployeeId = employeeId,
                AttendanceDate = date.Date
            });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查考勤是否存在失敗: {employeeId}/{date:yyyy-MM-dd}", ex);
            throw;
        }
    }
}

