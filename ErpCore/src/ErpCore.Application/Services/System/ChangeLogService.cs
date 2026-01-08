using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 異動記錄服務實作 (SYS0610, SYS0620)
/// </summary>
public class ChangeLogService : BaseService, IChangeLogService
{
    private readonly IChangeLogRepository _repository;

    public ChangeLogService(
        IChangeLogRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ChangeLogDto>> GetUserChangeLogsAsync(UserChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetUserChangeLogsAsync(
                "SYS0110",
                query.ChangeUserId,
                query.TargetUserId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            // 查詢使用者名稱
            var userIds = result.Items
                .Where(x => !string.IsNullOrEmpty(x.ChangeUserId))
                .Select(x => x.ChangeUserId!)
                .Distinct()
                .ToList();

            var userNames = new Dictionary<string, string>();
            if (userIds.Any())
            {
                // 這裡需要查詢使用者名稱，暫時先留空，後續可以加入使用者查詢邏輯
                // var users = await _userRepository.GetUsersByIdsAsync(userIds);
                // userNames = users.ToDictionary(x => x.UserId, x => x.UserName);
            }

            var dtos = result.Items.Select(x =>
            {
                var dto = new ChangeLogDto
                {
                    LogId = x.LogId,
                    ProgramId = x.ProgramId,
                    ChangeUserId = x.ChangeUserId,
                    ChangeUserName = userNames.ContainsKey(x.ChangeUserId ?? string.Empty)
                        ? userNames[x.ChangeUserId!]
                        : x.ChangeUserId,
                    ChangeDate = x.ChangeDate,
                    ChangeStatus = x.ChangeStatus,
                    ChangeField = x.ChangeField,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue
                };

                // 處理異動狀態名稱
                dto.ChangeStatusName = GetChangeStatusName(x.ChangeStatus);

                // 處理異動欄位和值列表
                if (!string.IsNullOrEmpty(x.ChangeField))
                {
                    dto.ChangeFields = x.ChangeField.Split(',').Select(f => f.Trim()).ToList();
                    dto.ChangeFieldDisplay = x.ChangeField;
                }
                else
                {
                    dto.ChangeFields = new List<string>();
                    dto.ChangeFieldDisplay = string.Empty;
                }

                if (!string.IsNullOrEmpty(x.OldValue))
                {
                    dto.OldValues = x.OldValue.Split(',').Select(v => v.Trim()).ToList();
                    dto.OldValueDisplay = x.OldValue;
                }
                else
                {
                    dto.OldValues = new List<string>();
                    dto.OldValueDisplay = "--";
                }

                if (!string.IsNullOrEmpty(x.NewValue))
                {
                    dto.NewValues = x.NewValue.Split(',').Select(v => v.Trim()).ToList();
                    dto.NewValueDisplay = x.NewValue;
                }
                else
                {
                    dto.NewValues = new List<string>();
                    dto.NewValueDisplay = "--";
                }

                return dto;
            }).ToList();

            return new PagedResult<ChangeLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<ChangeLogDto?> GetChangeLogByIdAsync(long logId)
    {
        try
        {
            var entity = await _repository.GetChangeLogByIdAsync(logId);
            if (entity == null)
            {
                return null;
            }

            var dto = new ChangeLogDto
            {
                LogId = entity.LogId,
                ProgramId = entity.ProgramId,
                ChangeUserId = entity.ChangeUserId,
                ChangeDate = entity.ChangeDate,
                ChangeStatus = entity.ChangeStatus,
                ChangeField = entity.ChangeField,
                OldValue = entity.OldValue,
                NewValue = entity.NewValue
            };

            // 處理異動狀態名稱
            dto.ChangeStatusName = GetChangeStatusName(entity.ChangeStatus);

            // 處理異動欄位和值列表
            if (!string.IsNullOrEmpty(entity.ChangeField))
            {
                dto.ChangeFields = entity.ChangeField.Split(',').Select(f => f.Trim()).ToList();
                dto.ChangeFieldDisplay = entity.ChangeField;
            }
            else
            {
                dto.ChangeFields = new List<string>();
                dto.ChangeFieldDisplay = string.Empty;
            }

            if (!string.IsNullOrEmpty(entity.OldValue))
            {
                dto.OldValues = entity.OldValue.Split(',').Select(v => v.Trim()).ToList();
                dto.OldValueDisplay = entity.OldValue;
            }
            else
            {
                dto.OldValues = new List<string>();
                dto.OldValueDisplay = "--";
            }

            if (!string.IsNullOrEmpty(entity.NewValue))
            {
                dto.NewValues = entity.NewValue.Split(',').Select(v => v.Trim()).ToList();
                dto.NewValueDisplay = entity.NewValue;
            }
            else
            {
                dto.NewValues = new List<string>();
                dto.NewValueDisplay = "--";
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢異動記錄失敗: {logId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLogDto>> GetRoleChangeLogsAsync(RoleChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetUserChangeLogsAsync(
                "SYS0210",
                query.ChangeUserId,
                query.RoleId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            // 查詢使用者名稱
            var userIds = result.Items
                .Where(x => !string.IsNullOrEmpty(x.ChangeUserId))
                .Select(x => x.ChangeUserId!)
                .Distinct()
                .ToList();

            var userNames = new Dictionary<string, string>();
            if (userIds.Any())
            {
                // 這裡需要查詢使用者名稱，暫時先留空，後續可以加入使用者查詢邏輯
                // var users = await _userRepository.GetUsersByIdsAsync(userIds);
                // userNames = users.ToDictionary(x => x.UserId, x => x.UserName);
            }

            var dtos = result.Items.Select(x =>
            {
                var dto = new ChangeLogDto
                {
                    LogId = x.LogId,
                    ProgramId = x.ProgramId,
                    ChangeUserId = x.ChangeUserId,
                    ChangeUserName = userNames.ContainsKey(x.ChangeUserId ?? string.Empty)
                        ? userNames[x.ChangeUserId!]
                        : x.ChangeUserId,
                    ChangeDate = x.ChangeDate,
                    ChangeStatus = x.ChangeStatus,
                    ChangeField = x.ChangeField,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue
                };

                // 處理異動狀態名稱
                dto.ChangeStatusName = GetChangeStatusName(x.ChangeStatus);

                // 處理異動欄位和值列表
                if (!string.IsNullOrEmpty(x.ChangeField))
                {
                    dto.ChangeFields = x.ChangeField.Split(',').Select(f => f.Trim()).ToList();
                    dto.ChangeFieldDisplay = x.ChangeField;
                }
                else
                {
                    dto.ChangeFields = new List<string>();
                    dto.ChangeFieldDisplay = string.Empty;
                }

                if (!string.IsNullOrEmpty(x.OldValue))
                {
                    dto.OldValues = x.OldValue.Split(',').Select(v => v.Trim()).ToList();
                    dto.OldValueDisplay = x.OldValue;
                }
                else
                {
                    dto.OldValues = new List<string>();
                    dto.OldValueDisplay = "--";
                }

                if (!string.IsNullOrEmpty(x.NewValue))
                {
                    dto.NewValues = x.NewValue.Split(',').Select(v => v.Trim()).ToList();
                    dto.NewValueDisplay = x.NewValue;
                }
                else
                {
                    dto.NewValues = new List<string>();
                    dto.NewValueDisplay = "--";
                }

                return dto;
            }).ToList();

            return new PagedResult<ChangeLogDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLogDto>> GetUserRoleChangeLogsAsync(UserRoleChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetUserRoleChangeLogsAsync(
                query.ChangeUserId,
                query.SearchUserId,
                query.SearchRoleId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            return ConvertToDtoList(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者角色對應設定異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLogDto>> GetSystemPermissionChangeLogsAsync(SystemPermissionChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetSystemPermissionChangeLogsAsync(
                query.ChangeUserId,
                query.SearchUserId,
                query.SearchRoleId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            return ConvertToDtoList(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統權限異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLogDto>> GetControllableFieldChangeLogsAsync(ControllableFieldChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetControllableFieldChangeLogsAsync(
                query.ChangeUserId,
                query.SearchUserId,
                query.FieldId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            return ConvertToDtoList(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢可管控欄位異動記錄失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<ChangeLogDto>> GetOtherChangeLogsAsync(OtherChangeLogQueryDto query)
    {
        try
        {
            var result = await _repository.GetOtherChangeLogsAsync(
                query.ChangeUserId,
                query.ProgramId,
                query.BeginDate,
                query.EndDate,
                query.PageIndex ?? 1,
                query.PageSize ?? 20
            );

            return ConvertToDtoList(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢其他異動記錄失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 轉換為 DTO 列表
    /// </summary>
    private PagedResult<ChangeLogDto> ConvertToDtoList(PagedResult<ChangeLog> result)
    {
        var dtos = result.Items.Select(x =>
        {
            var dto = new ChangeLogDto
            {
                LogId = x.LogId,
                ProgramId = x.ProgramId,
                ChangeUserId = x.ChangeUserId,
                ChangeUserName = x.ChangeUserId, // 暫時使用代碼，後續可加入使用者查詢邏輯
                ChangeDate = x.ChangeDate,
                ChangeStatus = x.ChangeStatus,
                ChangeField = x.ChangeField,
                OldValue = x.OldValue,
                NewValue = x.NewValue
            };

            // 處理異動狀態名稱
            dto.ChangeStatusName = GetChangeStatusName(x.ChangeStatus);

            // 處理異動欄位和值列表
            if (!string.IsNullOrEmpty(x.ChangeField))
            {
                dto.ChangeFields = x.ChangeField.Split(',').Select(f => f.Trim()).ToList();
                dto.ChangeFieldDisplay = x.ChangeField;
            }
            else
            {
                dto.ChangeFields = new List<string>();
                dto.ChangeFieldDisplay = string.Empty;
            }

            if (!string.IsNullOrEmpty(x.OldValue))
            {
                dto.OldValues = x.OldValue.Split(',').Select(v => v.Trim()).ToList();
                dto.OldValueDisplay = x.OldValue;
            }
            else
            {
                dto.OldValues = new List<string>();
                dto.OldValueDisplay = "--";
            }

            if (!string.IsNullOrEmpty(x.NewValue))
            {
                dto.NewValues = x.NewValue.Split(',').Select(v => v.Trim()).ToList();
                dto.NewValueDisplay = x.NewValue;
            }
            else
            {
                dto.NewValues = new List<string>();
                dto.NewValueDisplay = "--";
            }

            return dto;
        }).ToList();

        return new PagedResult<ChangeLogDto>
        {
            Items = dtos,
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            PageSize = result.PageSize
        };
    }

    /// <summary>
    /// 取得異動狀態名稱
    /// </summary>
    private string GetChangeStatusName(string status)
    {
        return status switch
        {
            "1" => "新增",
            "2" => "刪除",
            "3" => "修改",
            _ => status
        };
    }
}

