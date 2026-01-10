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
    private readonly ExportHelper _exportHelper;

    public ShopFloorQueryService(
        IShopFloorRepository repository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
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
            _logger.LogInfo("匯出商店查詢結果");

            // 查詢所有符合條件的資料（不分頁）
            var query = new StoreQueryDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = dto.Filters
            };

            var result = await QueryShopFloorsAsync(query);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "ShopId", DisplayName = "商店代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopName", DisplayName = "商店名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopNameEn", DisplayName = "商店名稱(英文)", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FloorId", DisplayName = "樓層代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FloorName", DisplayName = "樓層名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopType", DisplayName = "商店類型", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Address", DisplayName = "地址", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "City", DisplayName = "城市", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Zone", DisplayName = "區域", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PostalCode", DisplayName = "郵遞區號", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Phone", DisplayName = "電話", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Fax", DisplayName = "傳真", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Email", DisplayName = "電子郵件", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ManagerName", DisplayName = "店長姓名", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ManagerPhone", DisplayName = "店長電話", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OpenDate", DisplayName = "開店日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "CloseDate", DisplayName = "關店日期", DataType = ExportDataType.Date },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PosEnabled", DisplayName = "POS啟用", DataType = ExportDataType.Boolean },
                new ExportColumn { PropertyName = "PosSystemId", DisplayName = "POS系統代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "PosTerminalId", DisplayName = "POS終端代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Notes", DisplayName = "備註", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime }
            };

            // 根據格式匯出
            if (dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                return _exportHelper.ExportToPdf(result.Items, columns, dto.Title ?? "商店查詢報表");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "商店查詢", dto.Title ?? "商店查詢報表");
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

