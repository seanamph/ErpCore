using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 商店樓層服務實作 (SYS6000 - 商店資料維護)
/// </summary>
public class ShopFloorService : BaseService, IShopFloorService
{
    private readonly IShopFloorRepository _repository;

    public ShopFloorService(
        IShopFloorRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ShopFloorDto>> GetShopFloorsAsync(ShopFloorQueryDto query)
    {
        try
        {
            var repositoryQuery = new ShopFloorQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ShopId = query.ShopId,
                ShopName = query.ShopName,
                ShopType = query.ShopType,
                Status = query.Status,
                City = query.City,
                FloorId = query.FloorId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<ShopFloorDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢商店列表失敗", ex);
            throw;
        }
    }

    public async Task<ShopFloorDto> GetShopFloorByIdAsync(string shopId)
    {
        try
        {
            var shopFloor = await _repository.GetByIdAsync(shopId);
            if (shopFloor == null)
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            return MapToDto(shopFloor);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<string> CreateShopFloorAsync(CreateShopFloorDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.ShopId))
            {
                throw new InvalidOperationException($"商店編號已存在: {dto.ShopId}");
            }

            var shopFloor = new ShopFloor
            {
                ShopId = dto.ShopId,
                ShopName = dto.ShopName,
                ShopNameEn = dto.ShopNameEn,
                FloorId = dto.FloorId,
                ShopType = dto.ShopType,
                Address = dto.Address,
                City = dto.City,
                Zone = dto.Zone,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Fax = dto.Fax,
                Email = dto.Email,
                ManagerName = dto.ManagerName,
                ManagerPhone = dto.ManagerPhone,
                OpenDate = dto.OpenDate,
                CloseDate = dto.CloseDate,
                Status = dto.Status,
                PosEnabled = dto.PosEnabled,
                PosSystemId = dto.PosSystemId,
                PosTerminalId = dto.PosTerminalId,
                Notes = dto.Notes,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(shopFloor);

            return shopFloor.ShopId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商店失敗: {dto.ShopId}", ex);
            throw;
        }
    }

    public async Task UpdateShopFloorAsync(string shopId, UpdateShopFloorDto dto)
    {
        try
        {
            var shopFloor = await _repository.GetByIdAsync(shopId);
            if (shopFloor == null)
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            shopFloor.ShopName = dto.ShopName;
            shopFloor.ShopNameEn = dto.ShopNameEn;
            shopFloor.FloorId = dto.FloorId;
            shopFloor.ShopType = dto.ShopType;
            shopFloor.Address = dto.Address;
            shopFloor.City = dto.City;
            shopFloor.Zone = dto.Zone;
            shopFloor.PostalCode = dto.PostalCode;
            shopFloor.Phone = dto.Phone;
            shopFloor.Fax = dto.Fax;
            shopFloor.Email = dto.Email;
            shopFloor.ManagerName = dto.ManagerName;
            shopFloor.ManagerPhone = dto.ManagerPhone;
            shopFloor.OpenDate = dto.OpenDate;
            shopFloor.CloseDate = dto.CloseDate;
            shopFloor.Status = dto.Status;
            shopFloor.PosEnabled = dto.PosEnabled;
            shopFloor.PosSystemId = dto.PosSystemId;
            shopFloor.PosTerminalId = dto.PosTerminalId;
            shopFloor.Notes = dto.Notes;
            shopFloor.UpdatedBy = _userContext.UserId;
            shopFloor.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(shopFloor);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task DeleteShopFloorAsync(string shopId)
    {
        try
        {
            if (!await _repository.ExistsAsync(shopId))
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            await _repository.DeleteAsync(shopId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string shopId)
    {
        try
        {
            return await _repository.ExistsAsync(shopId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查商店編號是否存在失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string shopId, string status)
    {
        try
        {
            if (!await _repository.ExistsAsync(shopId))
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            await _repository.UpdateStatusAsync(shopId, status);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商店狀態失敗: {shopId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private ShopFloorDto MapToDto(ShopFloor shopFloor)
    {
        return new ShopFloorDto
        {
            TKey = shopFloor.TKey,
            ShopId = shopFloor.ShopId,
            ShopName = shopFloor.ShopName,
            ShopNameEn = shopFloor.ShopNameEn,
            FloorId = shopFloor.FloorId,
            FloorName = shopFloor.FloorName,
            ShopType = shopFloor.ShopType,
            Address = shopFloor.Address,
            City = shopFloor.City,
            Zone = shopFloor.Zone,
            PostalCode = shopFloor.PostalCode,
            Phone = shopFloor.Phone,
            Fax = shopFloor.Fax,
            Email = shopFloor.Email,
            ManagerName = shopFloor.ManagerName,
            ManagerPhone = shopFloor.ManagerPhone,
            OpenDate = shopFloor.OpenDate,
            CloseDate = shopFloor.CloseDate,
            Status = shopFloor.Status,
            PosEnabled = shopFloor.PosEnabled,
            PosSystemId = shopFloor.PosSystemId,
            PosTerminalId = shopFloor.PosTerminalId,
            Notes = shopFloor.Notes,
            CreatedBy = shopFloor.CreatedBy,
            CreatedAt = shopFloor.CreatedAt,
            UpdatedBy = shopFloor.UpdatedBy,
            UpdatedAt = shopFloor.UpdatedAt
        };
    }
}

