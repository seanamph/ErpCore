using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Infrastructure.Repositories.StoreFloor;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 樓層查詢服務實作 (SYS6381-SYS63A0 - 樓層查詢作業)
/// </summary>
public class FloorQueryService : BaseService, IFloorQueryService
{
    private readonly IFloorRepository _repository;
    private readonly ExportHelper _exportHelper;

    public FloorQueryService(
        IFloorRepository repository,
        ExportHelper exportHelper,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _exportHelper = exportHelper;
    }

    public async Task<PagedResult<FloorQueryDto>> QueryFloorsAsync(FloorQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢樓層列表（進階查詢）");

            var filters = request.Filters ?? new FloorQueryFilters();

            var query = new FloorQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder,
                FloorId = filters.FloorId,
                FloorName = filters.FloorName,
                Status = filters.Status
            };

            var result = await _repository.QueryAsync(query);

            var dtos = new List<FloorQueryDto>();
            foreach (var floor in result.Items)
            {
                var shopCount = await _repository.GetShopCountAsync(floor.FloorId);
                dtos.Add(new FloorQueryDto
                {
                    FloorId = floor.FloorId,
                    FloorName = floor.FloorName,
                    FloorNameEn = floor.FloorNameEn,
                    FloorNumber = floor.FloorNumber,
                    Description = floor.Description,
                    Status = floor.Status,
                    ShopCount = shopCount,
                    CreatedAt = floor.CreatedAt
                });
            }

            return new PagedResult<FloorQueryDto>
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

    public async Task<FloorStatisticsDto> GetFloorStatisticsAsync(FloorStatisticsRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢樓層統計資訊");

            var query = new FloorQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue
            };

            if (!string.IsNullOrEmpty(request.FloorId))
            {
                query.FloorId = request.FloorId;
            }

            var result = await _repository.QueryAsync(query);

            var statistics = new FloorStatisticsDto
            {
                TotalFloors = result.TotalCount,
                ActiveFloors = result.Items.Count(f => f.Status == "A"),
                FloorStatistics = new List<FloorStatisticsItemDto>()
            };

            foreach (var floor in result.Items)
            {
                var shopCount = await _repository.GetShopCountAsync(floor.FloorId);
                statistics.TotalShops += shopCount;

                statistics.FloorStatistics.Add(new FloorStatisticsItemDto
                {
                    FloorId = floor.FloorId,
                    FloorName = floor.FloorName,
                    ShopCount = shopCount,
                    ActiveShopCount = shopCount // 簡化處理，實際應查詢啟用商店數
                });
            }

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢樓層統計資訊失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportFloorsAsync(FloorExportDto dto)
    {
        try
        {
            _logger.LogInfo("匯出樓層查詢結果");

            // 查詢所有符合條件的資料（不分頁）
            var request = new FloorQueryRequestDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Filters = dto.Filters
            };

            var result = await QueryFloorsAsync(request);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "FloorId", DisplayName = "樓層代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FloorName", DisplayName = "樓層名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FloorNameEn", DisplayName = "樓層名稱(英文)", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "FloorNumber", DisplayName = "樓層編號", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "Description", DisplayName = "說明", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "ShopCount", DisplayName = "商店數量", DataType = ExportDataType.Number },
                new ExportColumn { PropertyName = "CreatedAt", DisplayName = "建立時間", DataType = ExportDataType.DateTime }
            };

            // 根據格式匯出
            if (dto.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                return _exportHelper.ExportToPdf(result.Items, columns, dto.Title ?? "樓層查詢報表");
            }
            else
            {
                return _exportHelper.ExportToExcel(result.Items, columns, "樓層查詢", dto.Title ?? "樓層查詢報表");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出樓層查詢結果失敗", ex);
            throw;
        }
    }
}

