using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 商店查詢服務實作 (SYS6210-SYS6270 - 商店查詢作業)
/// </summary>
public class ShopFloorQueryService : BaseService, IShopFloorQueryService
{
    private readonly IShopFloorRepository _repository;

    public ShopFloorQueryService(
        IShopFloorRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ShopFloorDto>> QueryShopFloorsAsync(StoreQueryDto query)
    {
        try
        {
            var filters = query.Filters ?? new StoreQueryFilters();
            
            var repositoryQuery = new ShopFloorQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ShopId = filters.ShopId,
                ShopName = filters.ShopName ?? filters.Keyword, // 關鍵字搜尋商店名稱
                ShopType = filters.ShopType,
                Status = filters.Status,
                City = filters.City,
                Zone = filters.Zone,
                FloorId = filters.FloorId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 如果設定了開店日期範圍，需要額外過濾
            if (filters.OpenDateFrom.HasValue || filters.OpenDateTo.HasValue)
            {
                var filteredItems = result.Items.Where(item =>
                {
                    if (filters.OpenDateFrom.HasValue && item.OpenDate.HasValue && item.OpenDate < filters.OpenDateFrom)
                        return false;
                    if (filters.OpenDateTo.HasValue && item.OpenDate.HasValue && item.OpenDate > filters.OpenDateTo)
                        return false;
                    return true;
                }).ToList();

                result = new PagedResult<ShopFloor>
                {
                    Items = filteredItems,
                    TotalCount = filteredItems.Count,
                    PageIndex = result.PageIndex,
                    PageSize = result.PageSize
                };
            }

            // 如果設定了POS啟用條件，需要額外過濾
            if (filters.PosEnabled.HasValue)
            {
                var filteredItems = result.Items.Where(item =>
                    item.PosEnabled == filters.PosEnabled.Value).ToList();

                result = new PagedResult<ShopFloor>
                {
                    Items = filteredItems,
                    TotalCount = filteredItems.Count,
                    PageIndex = result.PageIndex,
                    PageSize = result.PageSize
                };
            }

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

    public async Task<byte[]> ExportShopFloorsAsync(StoreExportDto dto)
    {
        try
        {
            // 查詢所有符合條件的資料（不分頁）
            var query = new StoreQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = dto.Filters
            };

            var result = await QueryShopFloorsAsync(query);

            // 根據格式匯出
            if (dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                // TODO: 實作PDF匯出
                throw new NotImplementedException("PDF匯出功能尚未實作");
            }
            else
            {
                // Excel匯出
                // TODO: 實作Excel匯出
                throw new NotImplementedException("Excel匯出功能尚未實作");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商店查詢結果失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private ShopFloorDto MapToDto(Domain.Entities.StoreFloor.ShopFloor shopFloor)
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

