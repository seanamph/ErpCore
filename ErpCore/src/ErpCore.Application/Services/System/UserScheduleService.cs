using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者排程服務實作 (SYS0116)
/// </summary>
public class UserScheduleService : BaseService, IUserScheduleService
{
    private readonly IUserScheduleRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public UserScheduleService(
        IUserScheduleRepository repository,
        IUserRepository userRepository,
        IUserService userService,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _userRepository = userRepository;
        _userService = userService;
    }

    public async Task<PagedResult<UserScheduleDto>> GetSchedulesAsync(UserScheduleQueryDto query)
    {
        try
        {
            var repositoryQuery = new UserScheduleQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                UserId = query.UserId,
                Status = query.Status,
                ScheduleType = query.ScheduleType,
                ScheduleDateFrom = query.ScheduleDateFrom,
                ScheduleDateTo = query.ScheduleDateTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new UserScheduleDto
            {
                ScheduleId = x.ScheduleId,
                UserId = x.UserId,
                ScheduleDate = x.ScheduleDate,
                ScheduleType = x.ScheduleType,
                Status = x.Status,
                ScheduleData = x.ScheduleData,
                ExecuteResult = x.ExecuteResult,
                ErrorMessage = x.ErrorMessage,
                ExecutedAt = x.ExecutedAt,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<UserScheduleDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢排程列表失敗", ex);
            throw;
        }
    }

    public async Task<UserScheduleDto> GetScheduleByIdAsync(Guid scheduleId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(scheduleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"排程不存在: {scheduleId}");
            }

            return new UserScheduleDto
            {
                ScheduleId = entity.ScheduleId,
                UserId = entity.UserId,
                ScheduleDate = entity.ScheduleDate,
                ScheduleType = entity.ScheduleType,
                Status = entity.Status,
                ScheduleData = entity.ScheduleData,
                ExecuteResult = entity.ExecuteResult,
                ErrorMessage = entity.ErrorMessage,
                ExecutedAt = entity.ExecutedAt,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task<Guid> CreateScheduleAsync(CreateUserScheduleDto dto)
    {
        try
        {
            // 驗證使用者是否存在
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"使用者不存在: {dto.UserId}");
            }

            // 驗證排程時間必須在未來
            if (dto.ScheduleDate <= DateTime.Now)
            {
                throw new ArgumentException("排程執行時間必須在未來");
            }

            // 序列化排程資料
            string? scheduleDataJson = null;
            if (dto.ScheduleData != null)
            {
                scheduleDataJson = JsonSerializer.Serialize(dto.ScheduleData);
            }

            var entity = new UserSchedule
            {
                ScheduleId = Guid.NewGuid(),
                UserId = dto.UserId,
                ScheduleDate = dto.ScheduleDate,
                ScheduleType = dto.ScheduleType,
                Status = "PENDING",
                ScheduleData = scheduleDataJson,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            _logger.LogInfo($"新增排程成功: {entity.ScheduleId}, 使用者: {dto.UserId}, 類型: {dto.ScheduleType}");

            return entity.ScheduleId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增排程失敗: {dto.UserId}", ex);
            throw;
        }
    }

    public async Task UpdateScheduleAsync(Guid scheduleId, UpdateUserScheduleDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(scheduleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"排程不存在: {scheduleId}");
            }

            // 僅待執行狀態的排程可修改
            if (entity.Status != "PENDING")
            {
                throw new InvalidOperationException($"僅待執行狀態的排程可修改，目前狀態: {entity.Status}");
            }

            // 驗證排程時間必須在未來
            if (dto.ScheduleDate.HasValue && dto.ScheduleDate.Value <= DateTime.Now)
            {
                throw new ArgumentException("排程執行時間必須在未來");
            }

            // 更新欄位
            if (dto.ScheduleDate.HasValue)
            {
                entity.ScheduleDate = dto.ScheduleDate.Value;
            }

            if (!string.IsNullOrEmpty(dto.ScheduleType))
            {
                entity.ScheduleType = dto.ScheduleType;
            }

            if (dto.ScheduleData != null)
            {
                entity.ScheduleData = JsonSerializer.Serialize(dto.ScheduleData);
            }

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"修改排程成功: {scheduleId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task CancelScheduleAsync(Guid scheduleId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(scheduleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"排程不存在: {scheduleId}");
            }

            // 僅待執行狀態的排程可取消
            if (entity.Status != "PENDING")
            {
                throw new InvalidOperationException($"僅待執行狀態的排程可取消，目前狀態: {entity.Status}");
            }

            entity.Status = "CANCELLED";
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"取消排程成功: {scheduleId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task ExecuteScheduleAsync(Guid scheduleId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(scheduleId);
            if (entity == null)
            {
                throw new InvalidOperationException($"排程不存在: {scheduleId}");
            }

            // 更新狀態為執行中
            await _repository.UpdateStatusAsync(scheduleId, "EXECUTING");

            try
            {
                // 根據排程類型執行對應操作
                switch (entity.ScheduleType)
                {
                    case "PASSWORD_RESET":
                        await ExecutePasswordResetAsync(entity);
                        break;
                    case "USER_UPDATE":
                        await ExecuteUserUpdateAsync(entity);
                        break;
                    case "STATUS_CHANGE":
                        await ExecuteStatusChangeAsync(entity);
                        break;
                    default:
                        throw new InvalidOperationException($"不支援的排程類型: {entity.ScheduleType}");
                }

                // 更新狀態為已完成
                await _repository.UpdateStatusAsync(scheduleId, "COMPLETED", null, "執行成功");
                entity.ExecutedAt = DateTime.Now;

                _logger.LogInfo($"執行排程成功: {scheduleId}, 類型: {entity.ScheduleType}");
            }
            catch (Exception ex)
            {
                // 更新狀態為失敗
                await _repository.UpdateStatusAsync(scheduleId, "FAILED", ex.Message, null);
                _logger.LogError($"執行排程失敗: {scheduleId}", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行排程失敗: {scheduleId}", ex);
            throw;
        }
    }

    public async Task<List<UserScheduleDto>> GetPendingSchedulesAsync(DateTime executeTime)
    {
        try
        {
            var entities = await _repository.GetPendingSchedulesAsync(executeTime);

            return entities.Select(x => new UserScheduleDto
            {
                ScheduleId = x.ScheduleId,
                UserId = x.UserId,
                ScheduleDate = x.ScheduleDate,
                ScheduleType = x.ScheduleType,
                Status = x.Status,
                ScheduleData = x.ScheduleData,
                ExecuteResult = x.ExecuteResult,
                ErrorMessage = x.ErrorMessage,
                ExecutedAt = x.ExecutedAt,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待執行排程失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行密碼重設
    /// </summary>
    private async Task ExecutePasswordResetAsync(UserSchedule schedule)
    {
        try
        {
            if (string.IsNullOrEmpty(schedule.ScheduleData))
            {
                throw new InvalidOperationException("排程資料為空");
            }

            var scheduleData = JsonSerializer.Deserialize<PasswordResetScheduleData>(schedule.ScheduleData);
            if (scheduleData == null)
            {
                throw new InvalidOperationException("排程資料格式錯誤");
            }

            string newPassword;
            if (scheduleData.AutoGenerate)
            {
                // 自動產生密碼
                newPassword = GenerateRandomPassword();
            }
            else if (!string.IsNullOrEmpty(scheduleData.NewPassword))
            {
                newPassword = scheduleData.NewPassword;
            }
            else
            {
                throw new InvalidOperationException("新密碼不能為空");
            }

            // 重設密碼
            var resetPasswordDto = new ResetPasswordDto
            {
                NewPassword = newPassword
            };

            await _userService.ResetPasswordAsync(schedule.UserId, resetPasswordDto);

            _logger.LogInfo($"排程密碼重設成功: {schedule.UserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行密碼重設排程失敗: {schedule.ScheduleId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行使用者資料更新
    /// </summary>
    private async Task ExecuteUserUpdateAsync(UserSchedule schedule)
    {
        try
        {
            if (string.IsNullOrEmpty(schedule.ScheduleData))
            {
                throw new InvalidOperationException("排程資料為空");
            }

            var scheduleData = JsonSerializer.Deserialize<UserUpdateScheduleData>(schedule.ScheduleData);
            if (scheduleData == null)
            {
                throw new InvalidOperationException("排程資料格式錯誤");
            }

            // 更新使用者資料
            var updateDto = new UpdateUserDto
            {
                UserName = scheduleData.UserName,
                Title = scheduleData.Title,
                OrgId = scheduleData.OrgId,
                Status = scheduleData.Status,
                Notes = scheduleData.Notes
            };

            await _userService.UpdateUserAsync(schedule.UserId, updateDto);

            _logger.LogInfo($"排程使用者資料更新成功: {schedule.UserId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行使用者資料更新排程失敗: {schedule.ScheduleId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 執行狀態變更
    /// </summary>
    private async Task ExecuteStatusChangeAsync(UserSchedule schedule)
    {
        try
        {
            if (string.IsNullOrEmpty(schedule.ScheduleData))
            {
                throw new InvalidOperationException("排程資料為空");
            }

            var scheduleData = JsonSerializer.Deserialize<StatusChangeScheduleData>(schedule.ScheduleData);
            if (scheduleData == null)
            {
                throw new InvalidOperationException("排程資料格式錯誤");
            }

            // 更新使用者狀態
            var updateStatusDto = new UpdateStatusDto
            {
                Status = scheduleData.NewStatus
            };

            await _userService.UpdateStatusAsync(schedule.UserId, updateStatusDto);

            _logger.LogInfo($"排程狀態變更成功: {schedule.UserId}, 新狀態: {scheduleData.NewStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行狀態變更排程失敗: {schedule.ScheduleId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 產生隨機密碼
    /// </summary>
    private string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        var password = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            password.Append(validChars[random.Next(validChars.Length)]);
        }

        return password.ToString();
    }
}

/// <summary>
/// 密碼重設排程資料
/// </summary>
public class PasswordResetScheduleData
{
    public string? NewPassword { get; set; }
    public bool AutoGenerate { get; set; }
}

/// <summary>
/// 使用者資料更新排程資料
/// </summary>
public class UserUpdateScheduleData
{
    public string? UserName { get; set; }
    public string? Title { get; set; }
    public string? OrgId { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 狀態變更排程資料
/// </summary>
public class StatusChangeScheduleData
{
    public string NewStatus { get; set; } = string.Empty;
}
