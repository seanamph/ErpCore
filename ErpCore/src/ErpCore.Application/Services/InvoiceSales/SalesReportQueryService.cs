using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Repositories.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售報表查詢服務實作 (SYSG610-SYSG640 - 報表查詢作業)
/// </summary>
public class SalesReportQueryService : BaseService, ISalesReportQueryService
{
    private readonly ISalesReportQueryRepository _repository;

    public SalesReportQueryService(
        ISalesReportQueryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SalesReportDetailDto>> QueryDetailReportAsync(SalesReportQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售報表明細 - 使用者: {_userContext.UserId}");

            var repositoryQuery = new Infrastructure.Repositories.InvoiceSales.SalesReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                GoodsId = query.GoodsId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _repository.QueryDetailReportAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDetailDto).ToList();

            return new PagedResult<SalesReportDetailDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表明細失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<SalesReportSummaryDto>> QuerySummaryReportAsync(SalesReportQueryDto query)
    {
        try
        {
            _logger.LogInfo($"查詢銷售報表彙總 - 使用者: {_userContext.UserId}");

            var repositoryQuery = new Infrastructure.Repositories.InvoiceSales.SalesReportQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                GoodsId = query.GoodsId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _repository.QuerySummaryReportAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToSummaryDto).ToList();

            return new PagedResult<SalesReportSummaryDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷售報表彙總失敗", ex);
            throw;
        }
    }

    private SalesReportDetailDto MapToDetailDto(Infrastructure.Repositories.InvoiceSales.SalesReportDetailResult result)
    {
        return new SalesReportDetailDto
        {
            OrderId = result.OrderId,
            OrderDate = result.OrderDate,
            OrderType = result.OrderType,
            ShopId = result.ShopId,
            ShopName = result.ShopName,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            GoodsId = result.GoodsId,
            GoodsName = result.GoodsName,
            OrderQty = result.OrderQty,
            UnitPrice = result.UnitPrice,
            Amount = result.Amount,
            ShippedQty = result.ShippedQty,
            ReturnQty = result.ReturnQty,
            Status = result.Status,
            CurrencyId = result.CurrencyId,
            ExchangeRate = result.ExchangeRate,
            ApplyUserId = result.ApplyUserId,
            ApplyUserName = result.ApplyUserName,
            ApplyDate = result.ApplyDate,
            ApproveUserId = result.ApproveUserId,
            ApproveUserName = result.ApproveUserName,
            ApproveDate = result.ApproveDate
        };
    }

    private SalesReportSummaryDto MapToSummaryDto(Infrastructure.Repositories.InvoiceSales.SalesReportSummaryResult result)
    {
        return new SalesReportSummaryDto
        {
            ShopId = result.ShopId,
            ShopName = result.ShopName,
            CustomerId = result.CustomerId,
            CustomerName = result.CustomerName,
            GoodsId = result.GoodsId,
            GoodsName = result.GoodsName,
            OrderCount = result.OrderCount,
            TotalQty = result.TotalQty,
            TotalAmount = result.TotalAmount,
            AvgUnitPrice = result.AvgUnitPrice,
            ShippedQty = result.ShippedQty,
            ReturnQty = result.ReturnQty
        };
    }
}

