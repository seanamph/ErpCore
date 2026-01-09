using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Infrastructure.Repositories.StoreMember;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 商店查詢服務實作 (SYS3210-SYS3299 - 商店查詢作業)
/// </summary>
public class StoreQueryService : BaseService, IStoreQueryService
{
    private readonly IStoreRepository _repository;

    public StoreQueryService(
        IStoreRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ShopQueryDto>> QueryShopsAsync(ShopQueryRequestDto request)
    {
        try
        {
            _logger.LogInfo("查詢商店列表（進階查詢）");

            var query = new StoreQuery
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortField = request.SortField,
                SortOrder = request.SortOrder
            };

            if (request.Filters != null)
            {
                query.ShopId = request.Filters.ShopId;
                query.ShopName = request.Filters.ShopName;
                query.ShopType = request.Filters.ShopType;
                query.Status = request.Filters.Status;
                query.City = request.Filters.City;
            }

            var result = await _repository.QueryAsync(query);

            var dtos = result.Items.Select(x => new ShopQueryDto
            {
                ShopId = x.ShopId,
                ShopName = x.ShopName,
                ShopType = x.ShopType,
                Status = x.Status,
                City = x.City,
                Zone = x.Zone,
                ManagerName = x.ManagerName,
                Phone = x.Phone,
                CreatedAt = x.CreatedAt
            }).ToList();

            return new PagedResult<ShopQueryDto>
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

    public async Task<byte[]> ExportShopsAsync(ShopExportRequestDto request)
    {
        try
        {
            _logger.LogInfo("匯出商店查詢結果");

            // 建立查詢條件
            var queryRequest = new ShopQueryRequestDto
            {
                PageIndex = 1,
                PageSize = int.MaxValue, // 匯出所有資料
                Filters = request.Filters
            };

            var result = await QueryShopsAsync(queryRequest);

            // 簡化實作：返回 CSV 格式（實際應使用 Excel 或 PDF 庫）
            var csv = "商店編號,商店名稱,商店類型,狀態,城市,區域,店長,電話,建立時間\n";
            foreach (var item in result.Items)
            {
                csv += $"{item.ShopId},{item.ShopName},{item.ShopType ?? ""},{item.Status},{item.City ?? ""},{item.Zone ?? ""},{item.ManagerName ?? ""},{item.Phone ?? ""},{item.CreatedAt:yyyy-MM-dd HH:mm:ss}\n";
            }

            return System.Text.Encoding.UTF8.GetBytes(csv);
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出商店查詢結果失敗", ex);
            throw;
        }
    }
}

