using ErpCore.Application.DTOs.Extension;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Extension;
using ErpCore.Infrastructure.Repositories.Extension;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Extension;

/// <summary>
/// 擴展功能服務實作
/// </summary>
public class ExtensionFunctionService : BaseService, IExtensionFunctionService
{
    private readonly IExtensionFunctionRepository _repository;

    public ExtensionFunctionService(
        IExtensionFunctionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ExtensionFunctionDto>> GetExtensionFunctionsAsync(ExtensionFunctionQueryDto query)
    {
        try
        {
            var repositoryQuery = new ExtensionFunctionQuery
            {
                ExtensionId = query.ExtensionId,
                ExtensionName = query.ExtensionName,
                ExtensionType = query.ExtensionType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new ExtensionFunctionDto
            {
                TKey = x.TKey,
                ExtensionId = x.ExtensionId,
                ExtensionName = x.ExtensionName,
                ExtensionType = x.ExtensionType,
                ExtensionValue = x.ExtensionValue,
                ExtensionConfig = x.ExtensionConfig,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Version = x.Version,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                CreatedPriority = x.CreatedPriority,
                CreatedGroup = x.CreatedGroup
            }).ToList();

            return new PagedResult<ExtensionFunctionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢擴展功能列表失敗", ex);
            throw;
        }
    }

    public async Task<ExtensionFunctionDto> GetExtensionFunctionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"擴展功能不存在: {tKey}");
            }

            return new ExtensionFunctionDto
            {
                TKey = entity.TKey,
                ExtensionId = entity.ExtensionId,
                ExtensionName = entity.ExtensionName,
                ExtensionType = entity.ExtensionType,
                ExtensionValue = entity.ExtensionValue,
                ExtensionConfig = entity.ExtensionConfig,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                Version = entity.Version,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                CreatedPriority = entity.CreatedPriority,
                CreatedGroup = entity.CreatedGroup
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ExtensionFunctionDto> GetExtensionFunctionByExtensionIdAsync(string extensionId)
    {
        try
        {
            var entity = await _repository.GetByExtensionIdAsync(extensionId);
            if (entity == null)
            {
                throw new Exception($"擴展功能不存在: {extensionId}");
            }

            return new ExtensionFunctionDto
            {
                TKey = entity.TKey,
                ExtensionId = entity.ExtensionId,
                ExtensionName = entity.ExtensionName,
                ExtensionType = entity.ExtensionType,
                ExtensionValue = entity.ExtensionValue,
                ExtensionConfig = entity.ExtensionConfig,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                Version = entity.Version,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                CreatedPriority = entity.CreatedPriority,
                CreatedGroup = entity.CreatedGroup
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢擴展功能失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateExtensionFunctionAsync(CreateExtensionFunctionDto dto)
    {
        try
        {
            // 檢查擴展功能代碼是否已存在
            if (await _repository.ExistsAsync(dto.ExtensionId))
            {
                throw new Exception($"擴展功能代碼已存在: {dto.ExtensionId}");
            }

            var entity = new ExtensionFunction
            {
                ExtensionId = dto.ExtensionId,
                ExtensionName = dto.ExtensionName,
                ExtensionType = dto.ExtensionType,
                ExtensionValue = dto.ExtensionValue,
                ExtensionConfig = dto.ExtensionConfig,
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status,
                Version = dto.Version,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = dto.CreatedPriority,
                CreatedGroup = dto.CreatedGroup
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增擴展功能成功: {dto.ExtensionId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增擴展功能失敗: {dto.ExtensionId}", ex);
            throw;
        }
    }

    public async Task UpdateExtensionFunctionAsync(long tKey, UpdateExtensionFunctionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"擴展功能不存在: {tKey}");
            }

            entity.ExtensionName = dto.ExtensionName;
            entity.ExtensionType = dto.ExtensionType;
            entity.ExtensionValue = dto.ExtensionValue;
            entity.ExtensionConfig = dto.ExtensionConfig;
            entity.SeqNo = dto.SeqNo ?? 0;
            entity.Status = dto.Status;
            entity.Version = dto.Version;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;
            entity.CreatedPriority = dto.CreatedPriority;
            entity.CreatedGroup = dto.CreatedGroup;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改擴展功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteExtensionFunctionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"擴展功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除擴展功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除擴展功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task BatchCreateExtensionFunctionsAsync(BatchCreateExtensionFunctionDto dto)
    {
        try
        {
            // 檢查是否有重複的擴展功能代碼
            foreach (var item in dto.Items)
            {
                if (await _repository.ExistsAsync(item.ExtensionId))
                {
                    throw new Exception($"擴展功能代碼已存在: {item.ExtensionId}");
                }
            }

            var entities = dto.Items.Select(x => new ExtensionFunction
            {
                ExtensionId = x.ExtensionId,
                ExtensionName = x.ExtensionName,
                ExtensionType = x.ExtensionType,
                ExtensionValue = x.ExtensionValue,
                ExtensionConfig = x.ExtensionConfig,
                SeqNo = x.SeqNo ?? 0,
                Status = x.Status,
                Version = x.Version,
                Notes = x.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = x.CreatedPriority,
                CreatedGroup = x.CreatedGroup
            }).ToList();

            await _repository.BatchCreateAsync(entities);
            _logger.LogInfo($"批次新增擴展功能成功: {dto.Items.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次新增擴展功能失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(long tKey, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"擴展功能不存在: {tKey}");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"更新擴展功能狀態成功: {tKey}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新擴展功能狀態失敗: {tKey}", ex);
            throw;
        }
    }
}

