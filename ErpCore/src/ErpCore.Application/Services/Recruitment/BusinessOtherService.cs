using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Repositories.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 招商其他功能服務實作 (SYSC999)
/// </summary>
public class BusinessOtherService : BaseService, IBusinessOtherService
{
    private readonly ITenantLocationRepository _repository;

    public BusinessOtherService(
        ITenantLocationRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TenantLocationDto>> GetTenantLocationsAsync(TenantLocationQueryDto query)
    {
        try
        {
            var repositoryQuery = new TenantLocationQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                AgmTKey = query.AgmTKey,
                LocationId = query.LocationId,
                AreaId = query.AreaId,
                FloorId = query.FloorId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<TenantLocationDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租戶位置列表失敗", ex);
            throw;
        }
    }

    public async Task<TenantLocationDto> GetTenantLocationAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByKeyAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租戶位置不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租戶位置失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<TenantLocationDto>> GetTenantLocationsByTenantAsync(long agmTKey)
    {
        try
        {
            var entities = await _repository.GetByTenantAsync(agmTKey);
            return entities.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租戶查詢位置列表失敗: {agmTKey}", ex);
            throw;
        }
    }

    public async Task<TenantLocationKeyDto> CreateTenantLocationAsync(CreateTenantLocationDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (dto.AgmTKey <= 0)
            {
                throw new ArgumentException("租戶主檔主鍵不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.LocationId))
            {
                throw new ArgumentException("位置代碼不能為空");
            }

            // 驗證狀態值
            if (dto.Status != "1" && dto.Status != "0")
            {
                throw new ArgumentException("狀態值必須為 1 (啟用) 或 0 (停用)");
            }

            var entity = new TenantLocation
            {
                AgmTKey = dto.AgmTKey,
                LocationId = dto.LocationId,
                AreaId = dto.AreaId,
                FloorId = dto.FloorId,
                Status = dto.Status ?? "1",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
            };

            var result = await _repository.CreateAsync(entity);
            return new TenantLocationKeyDto { TKey = result.TKey };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租戶位置失敗: {dto.LocationId}", ex);
            throw;
        }
    }

    public async Task UpdateTenantLocationAsync(long tKey, UpdateTenantLocationDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.LocationId))
            {
                throw new ArgumentException("位置代碼不能為空");
            }

            // 檢查租戶位置是否存在
            var existing = await _repository.GetByKeyAsync(tKey);
            if (existing == null)
            {
                throw new InvalidOperationException($"租戶位置不存在: {tKey}");
            }

            // 驗證狀態值
            if (dto.Status != "1" && dto.Status != "0")
            {
                throw new ArgumentException("狀態值必須為 1 (啟用) 或 0 (停用)");
            }

            existing.LocationId = dto.LocationId;
            existing.AreaId = dto.AreaId;
            existing.FloorId = dto.FloorId;
            existing.Status = dto.Status ?? existing.Status;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租戶位置失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteTenantLocationAsync(long tKey)
    {
        try
        {
            // 檢查租戶位置是否存在
            var exists = await _repository.ExistsAsync(tKey);
            if (!exists)
            {
                throw new InvalidOperationException($"租戶位置不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租戶位置失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteTenantLocationsBatchAsync(BatchDeleteTenantLocationDto dto)
    {
        try
        {
            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new ArgumentException("刪除項目不能為空");
            }

            foreach (var tKey in dto.Items)
            {
                await DeleteTenantLocationAsync(tKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除租戶位置失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private TenantLocationDto MapToDto(TenantLocation entity)
    {
        return new TenantLocationDto
        {
            TKey = entity.TKey,
            AgmTKey = entity.AgmTKey,
            LocationId = entity.LocationId,
            AreaId = entity.AreaId,
            FloorId = entity.FloorId,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

