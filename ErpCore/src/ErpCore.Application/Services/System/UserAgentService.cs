using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者權限代理服務實作
/// </summary>
public class UserAgentService : BaseService, IUserAgentService
{
    private readonly IUserAgentRepository _repository;
    private readonly IUserRepository _userRepository;

    public UserAgentService(
        IUserAgentRepository repository,
        IUserRepository userRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserAgentDto>> GetUserAgentsAsync(UserAgentQueryDto query)
    {
        try
        {
            var repositoryQuery = new UserAgentQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                PrincipalUserId = query.PrincipalUserId,
                AgentUserId = query.AgentUserId,
                Status = query.Status,
                BeginTimeFrom = query.BeginTimeFrom,
                BeginTimeTo = query.BeginTimeTo,
                EndTimeFrom = query.EndTimeFrom,
                EndTimeTo = query.EndTimeTo
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = new List<UserAgentDto>();
            foreach (var item in result.Items)
            {
                var dto = await MapToDtoAsync(item);
                dtos.Add(dto);
            }

            return new PagedResult<UserAgentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者權限代理列表失敗", ex);
            throw;
        }
    }

    public async Task<UserAgentDto> GetUserAgentAsync(Guid agentId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(agentId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者權限代理不存在: {agentId}");
            }

            return await MapToDtoAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者權限代理失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgentDto>> GetUserAgentsByPrincipalAsync(string userId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var result = await _repository.GetByPrincipalUserIdAsync(userId, pageIndex, pageSize);

            var dtos = new List<UserAgentDto>();
            foreach (var item in result.Items)
            {
                var dto = await MapToDtoAsync(item);
                dtos.Add(dto);
            }

            return new PagedResult<UserAgentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢委託人代理記錄失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgentDto>> GetUserAgentsByAgentAsync(string userId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var result = await _repository.GetByAgentUserIdAsync(userId, pageIndex, pageSize);

            var dtos = new List<UserAgentDto>();
            foreach (var item in result.Items)
            {
                var dto = await MapToDtoAsync(item);
                dtos.Add(dto);
            }

            return new PagedResult<UserAgentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢代理人代理記錄失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserAgentDto>> GetActiveUserAgentsAsync(string? principalUserId = null, string? agentUserId = null, DateTime? currentTime = null)
    {
        try
        {
            var entities = await _repository.GetActiveAgentsAsync(principalUserId, agentUserId, currentTime);

            var dtos = new List<UserAgentDto>();
            foreach (var item in entities)
            {
                var dto = await MapToDtoAsync(item);
                dtos.Add(dto);
            }

            return new PagedResult<UserAgentDto>
            {
                Items = dtos,
                TotalCount = dtos.Count,
                PageIndex = 1,
                PageSize = dtos.Count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢有效代理記錄失敗", ex);
            throw;
        }
    }

    public async Task<Guid> CreateUserAgentAsync(CreateUserAgentDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查委託人和代理人是否存在
            var principalUser = await _userRepository.GetByIdAsync(dto.PrincipalUserId);
            if (principalUser == null)
            {
                throw new InvalidOperationException($"委託人不存在: {dto.PrincipalUserId}");
            }

            var agentUser = await _userRepository.GetByIdAsync(dto.AgentUserId);
            if (agentUser == null)
            {
                throw new InvalidOperationException($"代理人不存在: {dto.AgentUserId}");
            }

            // 檢查委託人和代理人是否不同
            if (dto.PrincipalUserId == dto.AgentUserId)
            {
                throw new InvalidOperationException("委託人和代理人不能相同");
            }

            // 檢查時間範圍
            if (dto.EndTime <= dto.BeginTime)
            {
                throw new InvalidOperationException("結束時間必須大於開始時間");
            }

            var currentUserId = GetCurrentUserId();
            var now = DateTime.Now;

            var entity = new UserAgent
            {
                AgentId = Guid.NewGuid(),
                PrincipalUserId = dto.PrincipalUserId,
                AgentUserId = dto.AgentUserId,
                BeginTime = dto.BeginTime,
                EndTime = dto.EndTime,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = currentUserId,
                CreatedAt = now,
                UpdatedBy = currentUserId,
                UpdatedAt = now
            };

            await _repository.CreateAsync(entity);

            _logger.LogInfo($"新增使用者權限代理成功: {entity.AgentId}");

            return entity.AgentId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增使用者權限代理失敗", ex);
            throw;
        }
    }

    public async Task UpdateUserAgentAsync(Guid agentId, UpdateUserAgentDto dto)
    {
        try
        {
            // 驗證代理記錄是否存在
            var entity = await _repository.GetByIdAsync(agentId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者權限代理不存在: {agentId}");
            }

            // 驗證資料
            ValidateUpdateDto(dto);

            // 檢查委託人和代理人是否存在
            var principalUser = await _userRepository.GetByIdAsync(dto.PrincipalUserId);
            if (principalUser == null)
            {
                throw new InvalidOperationException($"委託人不存在: {dto.PrincipalUserId}");
            }

            var agentUser = await _userRepository.GetByIdAsync(dto.AgentUserId);
            if (agentUser == null)
            {
                throw new InvalidOperationException($"代理人不存在: {dto.AgentUserId}");
            }

            // 檢查委託人和代理人是否不同
            if (dto.PrincipalUserId == dto.AgentUserId)
            {
                throw new InvalidOperationException("委託人和代理人不能相同");
            }

            // 檢查時間範圍
            if (dto.EndTime <= dto.BeginTime)
            {
                throw new InvalidOperationException("結束時間必須大於開始時間");
            }

            var currentUserId = GetCurrentUserId();
            var now = DateTime.Now;

            entity.PrincipalUserId = dto.PrincipalUserId;
            entity.AgentUserId = dto.AgentUserId;
            entity.BeginTime = dto.BeginTime;
            entity.EndTime = dto.EndTime;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = currentUserId;
            entity.UpdatedAt = now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"修改使用者權限代理成功: {agentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改使用者權限代理失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task DeleteUserAgentAsync(Guid agentId)
    {
        try
        {
            // 驗證代理記錄是否存在
            var exists = await _repository.ExistsAsync(agentId);
            if (!exists)
            {
                throw new InvalidOperationException($"使用者權限代理不存在: {agentId}");
            }

            await _repository.DeleteAsync(agentId);

            _logger.LogInfo($"刪除使用者權限代理成功: {agentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者權限代理失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task DeleteUserAgentsBatchAsync(BatchDeleteUserAgentDto dto)
    {
        try
        {
            if (dto.AgentIds == null || dto.AgentIds.Count == 0)
            {
                throw new InvalidOperationException("請選擇要刪除的代理記錄");
            }

            await _repository.DeleteBatchAsync(dto.AgentIds);

            _logger.LogInfo($"批次刪除使用者權限代理成功: {dto.AgentIds.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除使用者權限代理失敗", ex);
            throw;
        }
    }

    public async Task UpdateUserAgentStatusAsync(Guid agentId, UpdateUserAgentStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(agentId);
            if (entity == null)
            {
                throw new InvalidOperationException($"使用者權限代理不存在: {agentId}");
            }

            var currentUserId = GetCurrentUserId();
            var now = DateTime.Now;

            entity.Status = dto.Status;
            entity.UpdatedBy = currentUserId;
            entity.UpdatedAt = now;

            await _repository.UpdateAsync(entity);

            _logger.LogInfo($"更新使用者權限代理狀態成功: {agentId}, 狀態: {dto.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新使用者權限代理狀態失敗: {agentId}", ex);
            throw;
        }
    }

    public async Task<CheckAgentPermissionResultDto> CheckAgentPermissionAsync(CheckAgentPermissionDto dto)
    {
        try
        {
            var currentTime = dto.CheckTime ?? DateTime.Now;
            var activeAgents = await _repository.GetActiveAgentsAsync(
                dto.PrincipalUserId,
                dto.AgentUserId,
                currentTime);

            var agent = activeAgents.FirstOrDefault(a =>
                a.PrincipalUserId == dto.PrincipalUserId &&
                a.AgentUserId == dto.AgentUserId &&
                a.BeginTime <= currentTime &&
                a.EndTime >= currentTime);

            if (agent != null)
            {
                return new CheckAgentPermissionResultDto
                {
                    HasPermission = true,
                    AgentId = agent.AgentId,
                    BeginTime = agent.BeginTime,
                    EndTime = agent.EndTime
                };
            }

            return new CheckAgentPermissionResultDto
            {
                HasPermission = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查代理權限失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將實體轉換為 DTO
    /// </summary>
    private async Task<UserAgentDto> MapToDtoAsync(UserAgent entity)
    {
        var dto = new UserAgentDto
        {
            AgentId = entity.AgentId,
            PrincipalUserId = entity.PrincipalUserId,
            AgentUserId = entity.AgentUserId,
            BeginTime = entity.BeginTime,
            EndTime = entity.EndTime,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };

        // 查詢使用者名稱
        var principalUser = await _userRepository.GetByIdAsync(entity.PrincipalUserId);
        if (principalUser != null)
        {
            dto.PrincipalUserName = principalUser.UserName;
        }

        var agentUser = await _userRepository.GetByIdAsync(entity.AgentUserId);
        if (agentUser != null)
        {
            dto.AgentUserName = agentUser.UserName;
        }

        return dto;
    }

    /// <summary>
    /// 驗證新增 DTO
    /// </summary>
    private void ValidateCreateDto(CreateUserAgentDto dto)
    {
        if (string.IsNullOrEmpty(dto.PrincipalUserId))
        {
            throw new InvalidOperationException("委託人不能為空");
        }

        if (string.IsNullOrEmpty(dto.AgentUserId))
        {
            throw new InvalidOperationException("代理人不能為空");
        }

        if (dto.EndTime <= dto.BeginTime)
        {
            throw new InvalidOperationException("結束時間必須大於開始時間");
        }
    }

    /// <summary>
    /// 驗證修改 DTO
    /// </summary>
    private void ValidateUpdateDto(UpdateUserAgentDto dto)
    {
        if (string.IsNullOrEmpty(dto.PrincipalUserId))
        {
            throw new InvalidOperationException("委託人不能為空");
        }

        if (string.IsNullOrEmpty(dto.AgentUserId))
        {
            throw new InvalidOperationException("代理人不能為空");
        }

        if (dto.EndTime <= dto.BeginTime)
        {
            throw new InvalidOperationException("結束時間必須大於開始時間");
        }
    }
}

