using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 停車位資料服務實作 (SYSM111-SYSM138)
/// </summary>
public class ParkingSpaceService : BaseService, IParkingSpaceService
{
    private readonly IParkingSpaceRepository _repository;

    public ParkingSpaceService(
        IParkingSpaceRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ParkingSpaceDto>> GetParkingSpacesAsync(ParkingSpaceQueryDto query)
    {
        try
        {
            var repositoryQuery = new ParkingSpaceQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ParkingSpaceId = query.ParkingSpaceId,
                ShopId = query.ShopId,
                FloorId = query.FloorId,
                Status = query.Status,
                LeaseId = query.LeaseId
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ParkingSpaceDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢停車位列表失敗", ex);
            throw;
        }
    }

    public async Task<ParkingSpaceDto> GetParkingSpaceByIdAsync(string parkingSpaceId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(parkingSpaceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"停車位不存在: {parkingSpaceId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢停車位失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ParkingSpaceDto>> GetAvailableParkingSpacesAsync(string? shopId)
    {
        try
        {
            var items = await _repository.GetAvailableParkingSpacesAsync(shopId);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可用停車位失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<ParkingSpaceDto> CreateParkingSpaceAsync(CreateParkingSpaceDto dto)
    {
        try
        {
            // 檢查停車位編號是否已存在
            if (await _repository.ExistsAsync(dto.ParkingSpaceId))
            {
                throw new InvalidOperationException($"停車位編號已存在: {dto.ParkingSpaceId}");
            }

            var entity = new ParkingSpace
            {
                ParkingSpaceId = dto.ParkingSpaceId,
                ParkingSpaceNo = dto.ParkingSpaceNo,
                ShopId = dto.ShopId,
                FloorId = dto.FloorId,
                Area = dto.Area,
                Status = dto.Status,
                LeaseId = dto.LeaseId,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增停車位成功: {dto.ParkingSpaceId}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增停車位失敗: {dto.ParkingSpaceId}", ex);
            throw;
        }
    }

    public async Task UpdateParkingSpaceAsync(string parkingSpaceId, UpdateParkingSpaceDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(parkingSpaceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"停車位不存在: {parkingSpaceId}");
            }

            entity.ParkingSpaceNo = dto.ParkingSpaceNo;
            entity.ShopId = dto.ShopId;
            entity.FloorId = dto.FloorId;
            entity.Area = dto.Area;
            entity.Status = dto.Status;
            entity.LeaseId = dto.LeaseId;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改停車位成功: {parkingSpaceId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改停車位失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task DeleteParkingSpaceAsync(string parkingSpaceId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(parkingSpaceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"停車位不存在: {parkingSpaceId}");
            }

            await _repository.DeleteAsync(parkingSpaceId);
            _logger.LogInfo($"刪除停車位成功: {parkingSpaceId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除停車位失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task UpdateParkingSpaceStatusAsync(string parkingSpaceId, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(parkingSpaceId);
            if (entity == null)
            {
                throw new InvalidOperationException($"停車位不存在: {parkingSpaceId}");
            }

            await _repository.UpdateStatusAsync(parkingSpaceId, status);
            _logger.LogInfo($"更新停車位狀態成功: {parkingSpaceId} -> {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新停車位狀態失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string parkingSpaceId)
    {
        try
        {
            return await _repository.ExistsAsync(parkingSpaceId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查停車位是否存在失敗: {parkingSpaceId}", ex);
            throw;
        }
    }

    private ParkingSpaceDto MapToDto(ParkingSpace entity)
    {
        return new ParkingSpaceDto
        {
            TKey = entity.TKey,
            ParkingSpaceId = entity.ParkingSpaceId,
            ParkingSpaceNo = entity.ParkingSpaceNo,
            ShopId = entity.ShopId,
            FloorId = entity.FloorId,
            Area = entity.Area,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            LeaseId = entity.LeaseId,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string? GetStatusName(string status)
    {
        return status switch
        {
            "A" => "可用",
            "U" => "使用中",
            "M" => "維護中",
            _ => status
        };
    }
}

