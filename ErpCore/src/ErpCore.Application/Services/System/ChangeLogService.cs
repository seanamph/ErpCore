using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
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
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IProgramRepository _programRepository;

    public ChangeLogService(
        IChangeLogRepository repository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IProgramRepository programRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _programRepository = programRepository;
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
                foreach (var userId in userIds)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        userNames[userId] = user.UserName ?? userId;
                    }
                }
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

            // 查詢使用者名稱
            string? changeUserName = null;
            if (!string.IsNullOrEmpty(entity.ChangeUserId))
            {
                var user = await _userRepository.GetByIdAsync(entity.ChangeUserId);
                changeUserName = user?.UserName ?? entity.ChangeUserId;
            }

            var dto = new ChangeLogDto
            {
                LogId = entity.LogId,
                ProgramId = entity.ProgramId,
                ChangeUserId = entity.ChangeUserId,
                ChangeUserName = changeUserName,
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
                foreach (var userId in userIds)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        userNames[userId] = user.UserName ?? userId;
                    }
                }
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

            return await ConvertToUserRoleChangeLogDtoListAsync(result);
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

            return await ConvertToDtoListAsync(result);
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

            return await ConvertToDtoListAsync(result);
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

            return await ConvertToDtoListWithProgramNameAsync(result);
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
    private async Task<PagedResult<ChangeLogDto>> ConvertToDtoListAsync(PagedResult<ChangeLog> result)
    {
        // 查詢使用者名稱
        var userIds = result.Items
            .Where(x => !string.IsNullOrEmpty(x.ChangeUserId))
            .Select(x => x.ChangeUserId!)
            .Distinct()
            .ToList();

        var userNames = new Dictionary<string, string>();
        if (userIds.Any())
        {
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    userNames[userId] = user.UserName ?? userId;
                }
            }
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

    /// <summary>
    /// 轉換為 DTO 列表（包含作業名稱）
    /// </summary>
    private async Task<PagedResult<ChangeLogDto>> ConvertToDtoListWithProgramNameAsync(PagedResult<ChangeLog> result)
    {
        // 查詢使用者名稱
        var userIds = result.Items
            .Where(x => !string.IsNullOrEmpty(x.ChangeUserId))
            .Select(x => x.ChangeUserId!)
            .Distinct()
            .ToList();

        var userNames = new Dictionary<string, string>();
        if (userIds.Any())
        {
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    userNames[userId] = user.UserName ?? userId;
                }
            }
        }

        // 查詢作業名稱
        var programIds = result.Items
            .Where(x => !string.IsNullOrEmpty(x.ProgramId))
            .Select(x => x.ProgramId)
            .Distinct()
            .ToList();

        var programNames = new Dictionary<string, string>();
        if (programIds.Any())
        {
            foreach (var programId in programIds)
            {
                var program = await _programRepository.GetByIdAsync(programId);
                if (program != null)
                {
                    programNames[programId] = program.ProgramName ?? programId;
                }
            }
        }

        var dtos = result.Items.Select(x =>
        {
            var dto = new ChangeLogDto
            {
                LogId = x.LogId,
                ProgramId = x.ProgramId,
                ProgramName = programNames.ContainsKey(x.ProgramId)
                    ? programNames[x.ProgramId]
                    : x.ProgramId,
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

    /// <summary>
    /// 轉換為使用者角色對應設定異動記錄 DTO 列表 (SYS0630)
    /// </summary>
    private async Task<PagedResult<ChangeLogDto>> ConvertToUserRoleChangeLogDtoListAsync(PagedResult<ChangeLog> result)
    {
        // 查詢使用者名稱
        var userIds = result.Items
            .Where(x => !string.IsNullOrEmpty(x.ChangeUserId))
            .Select(x => x.ChangeUserId!)
            .Distinct()
            .ToList();

        var userNames = new Dictionary<string, string>();
        if (userIds.Any())
        {
            foreach (var userId in userIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    userNames[userId] = user.UserName ?? userId;
                }
            }
        }

        // 收集所有需要查詢的使用者ID和角色ID（從 OldValue 和 NewValue 中解析）
        var allUserIds = new HashSet<string>();
        var allRoleIds = new HashSet<string>();

        foreach (var item in result.Items)
        {
            // 解析 OldValue
            if (!string.IsNullOrEmpty(item.OldValue))
            {
                var oldValueParts = ParseUserRoleValue(item.OldValue);
                if (!string.IsNullOrEmpty(oldValueParts.UserId))
                {
                    allUserIds.Add(oldValueParts.UserId);
                }
                if (!string.IsNullOrEmpty(oldValueParts.RoleId))
                {
                    allRoleIds.Add(oldValueParts.RoleId);
                }
            }

            // 解析 NewValue
            if (!string.IsNullOrEmpty(item.NewValue))
            {
                var newValueParts = ParseUserRoleValue(item.NewValue);
                if (!string.IsNullOrEmpty(newValueParts.UserId))
                {
                    allUserIds.Add(newValueParts.UserId);
                }
                if (!string.IsNullOrEmpty(newValueParts.RoleId))
                {
                    allRoleIds.Add(newValueParts.RoleId);
                }
            }
        }

        // 批量查詢使用者名稱
        var valueUserNames = new Dictionary<string, string>();
        foreach (var userId in allUserIds)
        {
            if (!userNames.ContainsKey(userId))
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    valueUserNames[userId] = user.UserName ?? userId;
                }
            }
        }

        // 批量查詢角色名稱
        var roleNames = new Dictionary<string, string>();
        foreach (var roleId in allRoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role != null)
            {
                roleNames[roleId] = role.RoleName ?? roleId;
            }
        }

        // 合併使用者名稱字典
        foreach (var kvp in valueUserNames)
        {
            userNames[kvp.Key] = kvp.Value;
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

            // 解析並設置使用者角色值顯示物件
            if (!string.IsNullOrEmpty(x.OldValue))
            {
                var oldValueParts = ParseUserRoleValue(x.OldValue);
                dto.OldValueDisplayObj = new UserRoleValueDisplayDto
                {
                    UserId = oldValueParts.UserId,
                    UserName = !string.IsNullOrEmpty(oldValueParts.UserId) && userNames.ContainsKey(oldValueParts.UserId)
                        ? userNames[oldValueParts.UserId]
                        : null,
                    RoleId = oldValueParts.RoleId,
                    RoleName = !string.IsNullOrEmpty(oldValueParts.RoleId) && roleNames.ContainsKey(oldValueParts.RoleId)
                        ? roleNames[oldValueParts.RoleId]
                        : null
                };
            }

            if (!string.IsNullOrEmpty(x.NewValue))
            {
                var newValueParts = ParseUserRoleValue(x.NewValue);
                dto.NewValueDisplayObj = new UserRoleValueDisplayDto
                {
                    UserId = newValueParts.UserId,
                    UserName = !string.IsNullOrEmpty(newValueParts.UserId) && userNames.ContainsKey(newValueParts.UserId)
                        ? userNames[newValueParts.UserId]
                        : null,
                    RoleId = newValueParts.RoleId,
                    RoleName = !string.IsNullOrEmpty(newValueParts.RoleId) && roleNames.ContainsKey(newValueParts.RoleId)
                        ? roleNames[newValueParts.RoleId]
                        : null
                };
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
    /// 解析使用者角色值（格式：USER_ID,ROLE_ID 或 USER_ID|ROLE_ID）
    /// </summary>
    private (string? UserId, string? RoleId) ParseUserRoleValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return (null, null);
        }

        // 嘗試多種分隔符號：逗號、豎線、分號等
        var separators = new[] { ',', '|', ';', '\t' };
        var parts = value.Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();

        if (parts.Count == 0)
        {
            return (null, null);
        }

        // 如果只有一個值，嘗試判斷是用戶ID還是角色ID
        if (parts.Count == 1)
        {
            // 根據欄位名稱判斷，但這裡我們無法知道，所以先假設是用戶ID
            return (parts[0], null);
        }

        // 如果有兩個值，第一個是用戶ID，第二個是角色ID
        if (parts.Count >= 2)
        {
            return (parts[0], parts[1]);
        }

        return (null, null);
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

