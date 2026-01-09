using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Repositories.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源擴展服務實作 (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
public class EnergyExtensionService : BaseService, IEnergyExtensionService
{
    private readonly IEnergyExtensionRepository _repository;

    public EnergyExtensionService(
        IEnergyExtensionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EnergyExtensionDto>> GetEnergyExtensionsAsync(EnergyExtensionQueryDto query)
    {
        try
        {
            var repositoryQuery = new EnergyExtensionQuery
            {
                ExtensionId = query.ExtensionId,
                EnergyId = query.EnergyId,
                ExtensionType = query.ExtensionType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<EnergyExtensionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源擴展資料列表失敗", ex);
            throw;
        }
    }

    public async Task<EnergyExtensionDto?> GetEnergyExtensionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源擴展資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyExtensionDto?> GetEnergyExtensionByExtensionIdAsync(string extensionId)
    {
        try
        {
            var entity = await _repository.GetByExtensionIdAsync(extensionId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源擴展資料失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateEnergyExtensionAsync(CreateEnergyExtensionDto dto)
    {
        try
        {
            var entity = new EnergyExtension
            {
                ExtensionId = dto.ExtensionId,
                EnergyId = dto.EnergyId,
                ExtensionType = dto.ExtensionType,
                ExtensionValue = dto.ExtensionValue,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增能源擴展資料成功: {dto.ExtensionId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源擴展資料失敗: {dto.ExtensionId}", ex);
            throw;
        }
    }

    public async Task UpdateEnergyExtensionAsync(long tKey, UpdateEnergyExtensionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源擴展資料不存在: {tKey}");
            }

            entity.EnergyId = dto.EnergyId;
            entity.ExtensionType = dto.ExtensionType;
            entity.ExtensionValue = dto.ExtensionValue;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改能源擴展資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源擴展資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteEnergyExtensionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源擴展資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除能源擴展資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源擴展資料失敗: {tKey}", ex);
            throw;
        }
    }

    private EnergyExtensionDto MapToDto(EnergyExtension entity)
    {
        return new EnergyExtensionDto
        {
            TKey = entity.TKey,
            ExtensionId = entity.ExtensionId,
            EnergyId = entity.EnergyId,
            ExtensionType = entity.ExtensionType,
            ExtensionValue = entity.ExtensionValue,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

