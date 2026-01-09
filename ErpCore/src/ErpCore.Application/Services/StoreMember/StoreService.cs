using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Repositories.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 商店服務實作 (SYS3000 - 商店資料維護)
/// </summary>
public class StoreService : BaseService, IStoreService
{
    private readonly IStoreRepository _repository;

    public StoreService(
        IStoreRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ShopDto>> GetShopsAsync(ShopQueryDto query)
    {
        try
        {
            var repositoryQuery = new StoreQuery
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
                FloorId = query.FloorId,
                AreaId = query.AreaId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<ShopDto>
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

    public async Task<ShopDto> GetShopByIdAsync(string shopId)
    {
        try
        {
            var shop = await _repository.GetByIdAsync(shopId);
            if (shop == null)
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            return MapToDto(shop);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task<string> CreateShopAsync(CreateShopDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.ShopId))
            {
                throw new InvalidOperationException($"商店編號已存在: {dto.ShopId}");
            }

            var shop = new Shop
            {
                ShopId = dto.ShopId,
                ShopName = dto.ShopName,
                ShopNameEn = dto.ShopNameEn,
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
                FloorId = dto.FloorId,
                AreaId = dto.AreaId,
                PosEnabled = dto.PosEnabled,
                PosSystemId = dto.PosSystemId,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            await _repository.CreateAsync(shop);

            _logger.LogInfo($"新增商店成功: {dto.ShopId}");
            return shop.ShopId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增商店失敗: {dto.ShopId}", ex);
            throw;
        }
    }

    public async Task UpdateShopAsync(string shopId, UpdateShopDto dto)
    {
        try
        {
            var shop = await _repository.GetByIdAsync(shopId);
            if (shop == null)
            {
                throw new KeyNotFoundException($"商店不存在: {shopId}");
            }

            shop.ShopName = dto.ShopName;
            shop.ShopNameEn = dto.ShopNameEn;
            shop.ShopType = dto.ShopType;
            shop.Address = dto.Address;
            shop.City = dto.City;
            shop.Zone = dto.Zone;
            shop.PostalCode = dto.PostalCode;
            shop.Phone = dto.Phone;
            shop.Fax = dto.Fax;
            shop.Email = dto.Email;
            shop.ManagerName = dto.ManagerName;
            shop.ManagerPhone = dto.ManagerPhone;
            shop.OpenDate = dto.OpenDate;
            shop.CloseDate = dto.CloseDate;
            shop.Status = dto.Status;
            shop.FloorId = dto.FloorId;
            shop.AreaId = dto.AreaId;
            shop.PosEnabled = dto.PosEnabled;
            shop.PosSystemId = dto.PosSystemId;
            shop.Notes = dto.Notes;
            shop.UpdatedBy = GetCurrentUserId();

            await _repository.UpdateAsync(shop);

            _logger.LogInfo($"修改商店成功: {shopId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改商店失敗: {shopId}", ex);
            throw;
        }
    }

    public async Task DeleteShopAsync(string shopId)
    {
        try
        {
            await _repository.DeleteAsync(shopId);
            _logger.LogInfo($"刪除商店成功: {shopId}");
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
            await _repository.UpdateStatusAsync(shopId, status);
            _logger.LogInfo($"更新商店狀態成功: {shopId}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新商店狀態失敗: {shopId}", ex);
            throw;
        }
    }

    private static ShopDto MapToDto(Shop shop)
    {
        return new ShopDto
        {
            TKey = shop.TKey,
            ShopId = shop.ShopId,
            ShopName = shop.ShopName,
            ShopNameEn = shop.ShopNameEn,
            ShopType = shop.ShopType,
            Address = shop.Address,
            City = shop.City,
            Zone = shop.Zone,
            PostalCode = shop.PostalCode,
            Phone = shop.Phone,
            Fax = shop.Fax,
            Email = shop.Email,
            ManagerName = shop.ManagerName,
            ManagerPhone = shop.ManagerPhone,
            OpenDate = shop.OpenDate,
            CloseDate = shop.CloseDate,
            Status = shop.Status,
            FloorId = shop.FloorId,
            AreaId = shop.AreaId,
            PosEnabled = shop.PosEnabled,
            PosSystemId = shop.PosSystemId,
            Notes = shop.Notes,
            CreatedBy = shop.CreatedBy,
            CreatedAt = shop.CreatedAt,
            UpdatedBy = shop.UpdatedBy,
            UpdatedAt = shop.UpdatedAt
        };
    }
}

