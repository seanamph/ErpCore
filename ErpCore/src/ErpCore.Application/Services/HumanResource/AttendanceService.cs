using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.HumanResource;
using ErpCore.Infrastructure.Repositories.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 考勤服務實作
/// </summary>
public class AttendanceService : BaseService, IAttendanceService
{
    private readonly IAttendanceRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public AttendanceService(
        IAttendanceRepository repository,
        IEmployeeRepository employeeRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public async Task<PagedResult<AttendanceDto>> GetAttendancesAsync(AttendanceQueryDto query)
    {
        try
        {
            var repositoryQuery = new AttendanceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                EmployeeId = query.EmployeeId,
                StartDate = query.StartDate,
                EndDate = query.EndDate,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            // 補充員工名稱
            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.EmployeeId))
                {
                    var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
                    if (employee != null)
                    {
                        dto.EmployeeName = employee.EmployeeName;
                    }
                }
            }

            return new PagedResult<AttendanceDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢考勤列表失敗", ex);
            throw;
        }
    }

    public async Task<AttendanceDto> GetAttendanceByIdAsync(string attendanceId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(attendanceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"考勤不存在: {attendanceId}");
            }

            var dto = MapToDto(entity);

            // 補充員工名稱
            if (!string.IsNullOrEmpty(dto.EmployeeId))
            {
                var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
                if (employee != null)
                {
                    dto.EmployeeName = employee.EmployeeName;
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢考勤失敗: {attendanceId}", ex);
            throw;
        }
    }

    public async Task<string> CreateAttendanceAsync(CreateAttendanceDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
            {
                throw new ArgumentException("員工編號不能為空");
            }

            // 檢查員工是否存在
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"員工不存在: {dto.EmployeeId}");
            }

            // 檢查該日期是否已有考勤記錄
            var exists = await _repository.ExistsAsync(dto.EmployeeId, dto.AttendanceDate);
            if (exists)
            {
                throw new InvalidOperationException($"該員工 {dto.AttendanceDate:yyyy-MM-dd} 的考勤記錄已存在");
            }

            // 計算工作時數
            decimal workHours = 0;
            if (dto.CheckInTime.HasValue && dto.CheckOutTime.HasValue)
            {
                var timeSpan = dto.CheckOutTime.Value - dto.CheckInTime.Value;
                workHours = (decimal)timeSpan.TotalHours;
            }

            var entity = new Attendance
            {
                AttendanceId = string.Empty, // Repository 會自動生成
                EmployeeId = dto.EmployeeId,
                AttendanceDate = dto.AttendanceDate.Date,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                WorkHours = workHours,
                OvertimeHours = dto.OvertimeHours,
                LeaveType = dto.LeaveType,
                LeaveHours = dto.LeaveHours,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.AttendanceId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增考勤失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    public async Task UpdateAttendanceAsync(string attendanceId, UpdateAttendanceDto dto)
    {
        try
        {
            // 檢查考勤是否存在
            var existing = await _repository.GetByIdAsync(attendanceId);
            if (existing == null)
            {
                throw new InvalidOperationException($"考勤不存在: {attendanceId}");
            }

            // 計算工作時數
            decimal workHours = 0;
            if (dto.CheckInTime.HasValue && dto.CheckOutTime.HasValue)
            {
                var timeSpan = dto.CheckOutTime.Value - dto.CheckInTime.Value;
                workHours = (decimal)timeSpan.TotalHours;
            }

            existing.CheckInTime = dto.CheckInTime;
            existing.CheckOutTime = dto.CheckOutTime;
            existing.WorkHours = workHours;
            existing.OvertimeHours = dto.OvertimeHours;
            existing.LeaveType = dto.LeaveType;
            existing.LeaveHours = dto.LeaveHours;
            existing.Status = dto.Status;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改考勤失敗: {attendanceId}", ex);
            throw;
        }
    }

    public async Task DeleteAttendanceAsync(string attendanceId)
    {
        try
        {
            // 檢查考勤是否存在
            var existing = await _repository.GetByIdAsync(attendanceId);
            if (existing == null)
            {
                throw new InvalidOperationException($"考勤不存在: {attendanceId}");
            }

            await _repository.DeleteAsync(attendanceId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除考勤失敗: {attendanceId}", ex);
            throw;
        }
    }

    public async Task CheckInAsync(CheckInOutDto dto)
    {
        try
        {
            // 檢查員工是否存在
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"員工不存在: {dto.EmployeeId}");
            }

            var today = dto.CheckTime.Date;
            var existing = await _repository.GetByEmployeeDateAsync(dto.EmployeeId, today);

            if (existing == null)
            {
                // 新增考勤記錄
                var attendance = new Attendance
                {
                    AttendanceId = string.Empty,
                    EmployeeId = dto.EmployeeId,
                    AttendanceDate = today,
                    CheckInTime = dto.CheckTime,
                    CheckOutTime = null,
                    WorkHours = 0,
                    OvertimeHours = 0,
                    LeaveType = null,
                    LeaveHours = 0,
                    Status = "N",
                    Notes = $"打卡位置: {dto.Location}",
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now,
                    UpdatedBy = GetCurrentUserId(),
                    UpdatedAt = DateTime.Now
                };

                await _repository.CreateAsync(attendance);
            }
            else
            {
                // 更新考勤記錄
                existing.CheckInTime = dto.CheckTime;
                existing.Notes = $"{existing.Notes}\n打卡位置: {dto.Location}";
                existing.UpdatedBy = GetCurrentUserId();
                existing.UpdatedAt = DateTime.Now;

                await _repository.UpdateAsync(existing);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"上班打卡失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    public async Task CheckOutAsync(CheckInOutDto dto)
    {
        try
        {
            // 檢查員工是否存在
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"員工不存在: {dto.EmployeeId}");
            }

            var today = dto.CheckTime.Date;
            var existing = await _repository.GetByEmployeeDateAsync(dto.EmployeeId, today);

            if (existing == null)
            {
                throw new InvalidOperationException("請先進行上班打卡");
            }

            // 更新考勤記錄
            existing.CheckOutTime = dto.CheckTime;

            // 重新計算工作時數
            if (existing.CheckInTime.HasValue)
            {
                var timeSpan = dto.CheckTime - existing.CheckInTime.Value;
                existing.WorkHours = (decimal)timeSpan.TotalHours;
            }

            existing.Notes = $"{existing.Notes}\n下班打卡位置: {dto.Location}";
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"下班打卡失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    public async Task<string> SupplementAttendanceAsync(SupplementAttendanceDto dto)
    {
        try
        {
            // 檢查員工是否存在
            var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"員工不存在: {dto.EmployeeId}");
            }

            // 檢查該日期是否已有考勤記錄
            var existing = await _repository.GetByEmployeeDateAsync(dto.EmployeeId, dto.AttendanceDate);
            if (existing != null)
            {
                throw new InvalidOperationException($"該員工 {dto.AttendanceDate:yyyy-MM-dd} 的考勤記錄已存在");
            }

            // 計算工作時數
            decimal workHours = 0;
            if (dto.CheckInTime.HasValue && dto.CheckOutTime.HasValue)
            {
                var timeSpan = dto.CheckOutTime.Value - dto.CheckInTime.Value;
                workHours = (decimal)timeSpan.TotalHours;
            }

            var attendance = new Attendance
            {
                AttendanceId = string.Empty,
                EmployeeId = dto.EmployeeId,
                AttendanceDate = dto.AttendanceDate.Date,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                WorkHours = workHours,
                OvertimeHours = 0,
                LeaveType = null,
                LeaveHours = 0,
                Status = "N",
                Notes = $"補打卡原因: {dto.Reason}",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(attendance);

            return attendance.AttendanceId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"補打卡失敗: {dto.EmployeeId}", ex);
            throw;
        }
    }

    private AttendanceDto MapToDto(Attendance entity)
    {
        return new AttendanceDto
        {
            AttendanceId = entity.AttendanceId,
            EmployeeId = entity.EmployeeId,
            AttendanceDate = entity.AttendanceDate,
            CheckInTime = entity.CheckInTime,
            CheckOutTime = entity.CheckOutTime,
            WorkHours = entity.WorkHours,
            OvertimeHours = entity.OvertimeHours,
            LeaveType = entity.LeaveType,
            LeaveHours = entity.LeaveHours,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

