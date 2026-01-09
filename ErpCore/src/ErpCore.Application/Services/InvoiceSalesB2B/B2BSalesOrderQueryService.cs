using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.InvoiceSalesB2B;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B銷售查詢服務實作 (SYSG000_B2B - B2B銷售查詢作業)
/// </summary>
public class B2BSalesOrderQueryService : BaseService, IB2BSalesOrderQueryService
{
    private readonly IB2BSalesOrderQueryRepository _repository;

    public B2BSalesOrderQueryService(
        IB2BSalesOrderQueryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<B2BSalesOrderQueryDto>> QueryAsync(B2BSalesOrderQueryConditionDto query)
    {
        try
        {
            _logger.LogInfo($"查詢B2B銷售單列表 - 使用者: {GetCurrentUserId()}");

            var repositoryQuery = new B2BSalesOrderQuery
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
                B2BFlag = query.B2BFlag ?? "Y"
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<B2BSalesOrderQueryDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢B2B銷售單列表失敗", ex);
            throw;
        }
    }

    public async Task<B2BSalesOrderStatisticsDto> GetStatisticsAsync(B2BSalesOrderStatisticsQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢B2B銷售單統計 - 使用者: {GetCurrentUserId()}");

            var repositoryQuery = new B2BSalesOrderStatisticsQuery
            {
                ShopId = query.ShopId,
                OrderType = query.OrderType,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                B2BFlag = query.B2BFlag ?? "Y"
            };

            var result = await _repository.GetStatisticsAsync(repositoryQuery);

            return new B2BSalesOrderStatisticsDto
            {
                OrderCount = result.OrderCount,
                TotalAmount = result.TotalAmount,
                TotalQty = result.TotalQty,
                AvgAmount = result.AvgAmount,
                ByShop = result.ByShop.Select(x => new B2BSalesOrderStatisticsByShopDto
                {
                    ShopId = x.ShopId,
                    ShopName = x.ShopName,
                    OrderCount = x.OrderCount,
                    TotalAmount = x.TotalAmount
                }).ToList(),
                ByStatus = result.ByStatus.Select(x => new B2BSalesOrderStatisticsByStatusDto
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
            _logger.LogError("查詢B2B銷售單統計失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Repository 結果轉換為 DTO
    /// </summary>
    private B2BSalesOrderQueryDto MapToDto(B2BSalesOrderQueryResult result)
    {
        return new B2BSalesOrderQueryDto
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
            TransferType = result.TransferType,
            TransferStatus = result.TransferStatus,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}

