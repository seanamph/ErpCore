using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Repositories.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源基礎服務實作 (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
public class EnergyBaseService : BaseService, IEnergyBaseService
{
    private readonly IEnergyBaseRepository _repository;

    public EnergyBaseService(
        IEnergyBaseRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EnergyBaseDto>> GetEnergyBasesAsync(EnergyBaseQueryDto query)
    {
        try
        {
            var repositoryQuery = new EnergyBaseQuery
            {
                EnergyId = query.EnergyId,
                EnergyName = query.EnergyName,
                EnergyType = query.EnergyType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<EnergyBaseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源基礎資料列表失敗", ex);
            throw;
        }
    }

    public async Task<EnergyBaseDto?> GetEnergyBaseByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyBaseDto?> GetEnergyBaseByEnergyIdAsync(string energyId)
    {
        try
        {
            var entity = await _repository.GetByEnergyIdAsync(energyId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源基礎資料失敗: {energyId}", ex);
            throw;
        }
    }

    public async Task<long> CreateEnergyBaseAsync(CreateEnergyBaseDto dto)
    {
        try
        {
            var entity = new EnergyBase
            {
                EnergyId = dto.EnergyId,
                EnergyName = dto.EnergyName,
                EnergyType = dto.EnergyType,
                Unit = dto.Unit,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增能源基礎資料成功: {dto.EnergyId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源基礎資料失敗: {dto.EnergyId}", ex);
            throw;
        }
    }

    public async Task UpdateEnergyBaseAsync(long tKey, UpdateEnergyBaseDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源基礎資料不存在: {tKey}");
            }

            entity.EnergyName = dto.EnergyName;
            entity.EnergyType = dto.EnergyType;
            entity.Unit = dto.Unit;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改能源基礎資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteEnergyBaseAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源基礎資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除能源基礎資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    private EnergyBaseDto MapToDto(EnergyBase entity)
    {
        return new EnergyBaseDto
        {
            TKey = entity.TKey,
            EnergyId = entity.EnergyId,
            EnergyName = entity.EnergyName,
            EnergyType = entity.EnergyType,
            Unit = entity.Unit,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

