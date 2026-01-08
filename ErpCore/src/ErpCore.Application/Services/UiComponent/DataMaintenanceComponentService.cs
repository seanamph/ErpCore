using ErpCore.Application.DTOs.UiComponent;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.UiComponent;
using ErpCore.Infrastructure.Repositories.UiComponent;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.UiComponent;

/// <summary>
/// 資料維護UI組件服務實作
/// </summary>
public class DataMaintenanceComponentService : BaseService, IDataMaintenanceComponentService
{
    private readonly IUIComponentRepository _repository;

    public DataMaintenanceComponentService(
        IUIComponentRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UIComponentDto>> GetComponentsAsync(UIComponentQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢UI組件列表: {query.ComponentCode}");

            var repositoryQuery = new UIComponentQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ComponentCode = query.ComponentCode,
                ComponentType = query.ComponentType,
                ComponentVersion = query.ComponentVersion,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new UIComponentDto
            {
                ComponentId = x.ComponentId,
                ComponentCode = x.ComponentCode,
                ComponentName = x.ComponentName,
                ComponentType = x.ComponentType,
                ComponentVersion = x.ComponentVersion,
                ConfigJson = x.ConfigJson,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<UIComponentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢UI組件列表失敗", ex);
            throw;
        }
    }

    public async Task<UIComponentDto?> GetComponentByIdAsync(long componentId)
    {
        try
        {
            _logger.LogInfo($"查詢UI組件: {componentId}");

            var entity = await _repository.GetByIdAsync(componentId);
            if (entity == null)
            {
                return null;
            }

            return new UIComponentDto
            {
                ComponentId = entity.ComponentId,
                ComponentCode = entity.ComponentCode,
                ComponentName = entity.ComponentName,
                ComponentType = entity.ComponentType,
                ComponentVersion = entity.ComponentVersion,
                ConfigJson = entity.ConfigJson,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢UI組件失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<UIComponentDto> CreateComponentAsync(CreateUIComponentDto dto)
    {
        try
        {
            _logger.LogInfo($"新增UI組件: {dto.ComponentCode}");

            // 檢查組件代碼是否已存在
            var existing = await _repository.GetByCodeAndVersionAsync(dto.ComponentCode, dto.ComponentVersion);
            if (existing != null)
            {
                throw new Exception($"組件代碼 {dto.ComponentCode} (版本 {dto.ComponentVersion}) 已存在");
            }

            var entity = new UIComponent
            {
                ComponentCode = dto.ComponentCode,
                ComponentName = dto.ComponentName,
                ComponentType = dto.ComponentType,
                ComponentVersion = dto.ComponentVersion,
                ConfigJson = dto.ConfigJson,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);

            return new UIComponentDto
            {
                ComponentId = result.ComponentId,
                ComponentCode = result.ComponentCode,
                ComponentName = result.ComponentName,
                ComponentType = result.ComponentType,
                ComponentVersion = result.ComponentVersion,
                ConfigJson = result.ConfigJson,
                Status = result.Status,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedBy = result.UpdatedBy,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增UI組件失敗: {dto.ComponentCode}", ex);
            throw;
        }
    }

    public async Task<UIComponentDto> UpdateComponentAsync(long componentId, UpdateUIComponentDto dto)
    {
        try
        {
            _logger.LogInfo($"修改UI組件: {componentId}");

            var entity = await _repository.GetByIdAsync(componentId);
            if (entity == null)
            {
                throw new Exception($"UI組件不存在: {componentId}");
            }

            entity.ComponentName = dto.ComponentName;
            entity.ConfigJson = dto.ConfigJson;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);

            return new UIComponentDto
            {
                ComponentId = entity.ComponentId,
                ComponentCode = entity.ComponentCode,
                ComponentName = entity.ComponentName,
                ComponentType = entity.ComponentType,
                ComponentVersion = entity.ComponentVersion,
                ConfigJson = entity.ConfigJson,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改UI組件失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteComponentAsync(long componentId)
    {
        try
        {
            _logger.LogInfo($"刪除UI組件: {componentId}");

            var entity = await _repository.GetByIdAsync(componentId);
            if (entity == null)
            {
                throw new Exception($"UI組件不存在: {componentId}");
            }

            return await _repository.DeleteAsync(componentId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除UI組件失敗: {componentId}", ex);
            throw;
        }
    }

    public async Task<List<UIComponentUsageDto>> GetUsagesAsync(long componentId)
    {
        try
        {
            _logger.LogInfo($"查詢UI組件使用記錄: {componentId}");

            var usages = await _repository.GetUsagesAsync(componentId);

            return usages.Select(x => new UIComponentUsageDto
            {
                UsageId = x.UsageId,
                ComponentId = x.ComponentId,
                ModuleCode = x.ModuleCode,
                ModuleName = x.ModuleName,
                UsageCount = x.UsageCount,
                LastUsedAt = x.LastUsedAt,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢UI組件使用記錄失敗: {componentId}", ex);
            throw;
        }
    }
}

