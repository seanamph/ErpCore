using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售查詢服務實作 (SYSG510-SYSG5D0 - 銷售查詢作業)
/// </summary>
public class SalesOrderQueryService : BaseService, ISalesOrderQueryService
{
    private readonly ISalesOrderQueryRepository _repository;

    public SalesOrderQueryService(
        ISalesOrderQueryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SalesOrderQueryDto>> QueryAsync(SalesOrderQueryConditionDto query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售單列表 - 使用者: {_userContext.UserId}");

            var repositoryQuery = new Infrastructure.Repositories.InvoiceSales.SalesOrderQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                ApplyUserId = query.ApplyUserId,
                ApproveUserId = query.ApproveUserId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<SalesOrderQueryDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<SalesOrderStatisticsDto> GetStatisticsAsync(SalesOrderStatisticsQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售單統計 - 使用者: {_userContext.UserId}");

            var repositoryQuery = new Infrastructure.Repositories.InvoiceSales.SalesOrderStatisticsQuery
            {
                ShopId = query.ShopId,
                OrderType = query.OrderType,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _repository.GetStatisticsAsync(repositoryQuery);

            return new SalesOrderStatisticsDto
            {
                OrderCount = result.OrderCount,
                TotalAmount = result.TotalAmount,
                TotalQty = result.TotalQty,
                AvgAmount = result.AvgAmount,
                ByShop = result.ByShop.Select(x => new SalesOrderStatisticsByShopDto
                {
                    ShopId = x.ShopId,
                    ShopName = x.ShopName,
                    OrderCount = x.OrderCount,
                    TotalAmount = x.TotalAmount
                }).ToList(),
                ByStatus = result.ByStatus.Select(x => new SalesOrderStatisticsByStatusDto
                {
                    Status = x.Status,
                    StatusName = x.StatusName,
                    OrderCount = x.OrderCount,
                    TotalAmount = x.TotalAmount
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售單統計失敗", ex);
            throw;
        }
    }

    private SalesOrderQueryDto MapToDto(Infrastructure.Repositories.InvoiceSales.SalesOrderQueryResult result)
    {
        return new SalesOrderQueryDto
        {
            TKey = result.TKey,
            OrderId = result.OrderId,
            OrderDate = result.OrderDate,
            OrderType = result.OrderType,
            ShopId = result.ShopId,
            ShopName = result.ShopName,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            Status = result.Status,
            TotalAmount = result.TotalAmount,
            TotalQty = result.TotalQty,
            ApplyUserId = result.ApplyUserId,
            ApplyUserName = result.ApplyUserName,
            ApplyDate = result.ApplyDate,
            ApproveUserId = result.ApproveUserId,
            ApproveUserName = result.ApproveUserName,
            ApproveDate = result.ApproveDate,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}

