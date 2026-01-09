using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 樓層服務實作 (SYS6310-SYS6370 - 樓層資料維護)
/// </summary>
public class FloorService : BaseService, IFloorService
{
    private readonly IFloorRepository _repository;

    public FloorService(
        IFloorRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<FloorDto>> GetFloorsAsync(FloorQueryDto query)
    {
        try
        {
            var repositoryQuery = new FloorQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                FloorId = query.FloorId,
                FloorName = query.FloorName,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(async floor =>
            {
                var dto = MapToDto(floor);
                dto.ShopCount = await _repository.GetShopCountAsync(floor.FloorId);
                return dto;
            }).Select(t => t.Result).ToList();

            return new PagedResult<FloorDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢樓層列表失敗", ex);
            throw;
        }
    }

    public async Task<FloorDto> GetFloorByIdAsync(string floorId)
    {
        try
        {
            var floor = await _repository.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new KeyNotFoundException($"樓層不存在: {floorId}");
            }

            var dto = MapToDto(floor);
            dto.ShopCount = await _repository.GetShopCountAsync(floorId);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢樓層失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task<string> CreateFloorAsync(CreateFloorDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.FloorId))
            {
                throw new InvalidOperationException($"樓層代碼已存在: {dto.FloorId}");
            }

            var floor = new Floor
            {
                FloorId = dto.FloorId,
                FloorName = dto.FloorName,
                FloorNameEn = dto.FloorNameEn,
                FloorNumber = dto.FloorNumber,
                Description = dto.Description,
                Status = dto.Status,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(floor);

            return floor.FloorId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增樓層失敗: {dto.FloorId}", ex);
            throw;
        }
    }

    public async Task UpdateFloorAsync(string floorId, UpdateFloorDto dto)
    {
        try
        {
            var floor = await _repository.GetByIdAsync(floorId);
            if (floor == null)
            {
                throw new KeyNotFoundException($"樓層不存在: {floorId}");
            }

            floor.FloorName = dto.FloorName;
            floor.FloorNameEn = dto.FloorNameEn;
            floor.FloorNumber = dto.FloorNumber;
            floor.Description = dto.Description;
            floor.Status = dto.Status;
            floor.UpdatedBy = _userContext.UserId;
            floor.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(floor);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改樓層失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task DeleteFloorAsync(string floorId)
    {
        try
        {
            if (!await _repository.ExistsAsync(floorId))
            {
                throw new KeyNotFoundException($"樓層不存在: {floorId}");
            }

            // 檢查是否有相關商店
            var shopCount = await _repository.GetShopCountAsync(floorId);
            if (shopCount > 0)
            {
                throw new InvalidOperationException($"樓層仍有 {shopCount} 間商店，無法刪除");
            }

            await _repository.DeleteAsync(floorId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除樓層失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string floorId)
    {
        try
        {
            return await _repository.ExistsAsync(floorId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查樓層代碼是否存在失敗: {floorId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string floorId, string status)
    {
        try
        {
            if (!await _repository.ExistsAsync(floorId))
            {
                throw new KeyNotFoundException($"樓層不存在: {floorId}");
            }

            await _repository.UpdateStatusAsync(floorId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新樓層狀態失敗: {floorId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private FloorDto MapToDto(Floor floor)
    {
        return new FloorDto
        {
            FloorId = floor.FloorId,
            FloorName = floor.FloorName,
            FloorNameEn = floor.FloorNameEn,
            FloorNumber = floor.FloorNumber,
            Description = floor.Description,
            Status = floor.Status,
            CreatedBy = floor.CreatedBy,
            CreatedAt = floor.CreatedAt,
            UpdatedBy = floor.UpdatedBy,
            UpdatedAt = floor.UpdatedAt
        };
    }
}

