using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Repositories.CommunicationModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CommunicationModule;

/// <summary>
/// 系統通訊設定服務實作
/// </summary>
public class SystemCommunicationService : BaseService, ISystemCommunicationService
{
    private readonly ISystemCommunicationRepository _repository;

    public SystemCommunicationService(
        ISystemCommunicationRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SystemCommunicationDto>> GetSystemCommunicationsAsync(SystemCommunicationQueryDto query)
    {
        try
        {
            var repositoryQuery = new SystemCommunicationQuery
            {
                SystemCode = query.SystemCode,
                SystemName = query.SystemName,
                CommunicationType = query.CommunicationType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<SystemCommunicationDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統通訊設定列表失敗", ex);
            throw;
        }
    }

    public async Task<SystemCommunicationDto?> GetSystemCommunicationByIdAsync(long communicationId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(communicationId);
            return entity == null ? null : MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統通訊設定失敗: {communicationId}", ex);
            throw;
        }
    }

    public async Task<SystemCommunicationDto?> GetSystemCommunicationByCodeAsync(string systemCode)
    {
        try
        {
            var entity = await _repository.GetBySystemCodeAsync(systemCode);
            return entity == null ? null : MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統通訊設定失敗: {systemCode}", ex);
            throw;
        }
    }

    public async Task<long> CreateSystemCommunicationAsync(CreateSystemCommunicationDto dto)
    {
        try
        {
            var entity = new SystemCommunication
            {
                SystemCode = dto.SystemCode,
                SystemName = dto.SystemName,
                CommunicationType = dto.CommunicationType,
                EndpointUrl = dto.EndpointUrl,
                ApiKey = dto.ApiKey,
                ApiSecret = dto.ApiSecret,
                ConfigData = dto.ConfigData,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var id = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增系統通訊設定成功: {id}");
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增系統通訊設定失敗", ex);
            throw;
        }
    }

    public async Task UpdateSystemCommunicationAsync(long communicationId, UpdateSystemCommunicationDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(communicationId);
            if (entity == null)
            {
                throw new Exception($"系統通訊設定不存在: {communicationId}");
            }

            entity.SystemName = dto.SystemName;
            entity.CommunicationType = dto.CommunicationType;
            entity.EndpointUrl = dto.EndpointUrl;
            entity.ApiKey = dto.ApiKey;
            entity.ApiSecret = dto.ApiSecret;
            entity.ConfigData = dto.ConfigData;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改系統通訊設定成功: {communicationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改系統通訊設定失敗: {communicationId}", ex);
            throw;
        }
    }

    public async Task DeleteSystemCommunicationAsync(long communicationId)
    {
        try
        {
            await _repository.DeleteAsync(communicationId);
            _logger.LogInfo($"刪除系統通訊設定成功: {communicationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除系統通訊設定失敗: {communicationId}", ex);
            throw;
        }
    }

    private static SystemCommunicationDto MapToDto(SystemCommunication entity)
    {
        return new SystemCommunicationDto
        {
            CommunicationId = entity.CommunicationId,
            SystemCode = entity.SystemCode,
            SystemName = entity.SystemName,
            CommunicationType = entity.CommunicationType,
            EndpointUrl = entity.EndpointUrl,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

