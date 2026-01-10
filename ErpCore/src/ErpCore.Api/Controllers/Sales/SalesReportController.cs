using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Sales;
using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Application.Services.InvoiceSales;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Sales;

/// <summary>
/// 銷售報表查詢控制器 (SYSD310-SYSD430)
/// </summary>
[Route("api/v1/sales-orders/reports")]
public class SalesReportController : BaseController
{
    private readonly ISalesReportQueryService _salesReportQueryService;

    public SalesReportController(
        ISalesReportQueryService salesReportQueryService,
        ILoggerService logger) : base(logger)
    {
        _salesReportQueryService = salesReportQueryService;
    }

    /// <summary>
    /// 銷售明細報表查詢
    /// </summary>
    [HttpPost("detail")]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportDetailDto>>>> GetSalesDetailReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 轉換 Sales DTO 到 InvoiceSales DTO
            var invoiceSalesQuery = new InvoiceSales.SalesReportQueryDto
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _salesReportQueryService.QueryDetailReportAsync(invoiceSalesQuery);
            return result;
        }, "查詢銷售明細報表失敗");
    }

    /// <summary>
    /// 銷售統計報表查詢
    /// </summary>
    [HttpPost("statistics")]
    public async Task<ActionResult<ApiResponse<PagedResult<SalesReportSummaryDto>>>> GetSalesStatisticsReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 轉換 Sales DTO 到 InvoiceSales DTO
            var invoiceSalesQuery = new InvoiceSales.SalesReportQueryDto
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            var result = await _salesReportQueryService.QuerySummaryReportAsync(invoiceSalesQuery);
            return result;
        }, "查詢銷售統計報表失敗");
    }

    /// <summary>
    /// 銷售分析報表查詢
    /// </summary>
    [HttpPost("analysis")]
    public async Task<ActionResult<ApiResponse<SalesAnalysisReportDto>>> GetSalesAnalysisReport(
        [FromBody] SalesReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            // 轉換 Sales DTO 到 InvoiceSales DTO
            var invoiceSalesQuery = new InvoiceSales.SalesReportQueryDto
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                CustomerId = query.CustomerId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo
            };

            // 查詢明細和彙總資料進行分析
            var detailResult = await _salesReportQueryService.QueryDetailReportAsync(invoiceSalesQuery);
            var summaryResult = await _salesReportQueryService.QuerySummaryReportAsync(invoiceSalesQuery);

            // 計算分析數據
            var totalAmount = summaryResult.Items.Sum(x => x.TotalAmount);
            var totalQty = summaryResult.Items.Sum(x => x.TotalQty);
            var orderCount = summaryResult.Items.Sum(x => x.OrderCount);
            var avgAmount = orderCount > 0 ? totalAmount / orderCount : 0;
            var avgQty = orderCount > 0 ? totalQty / orderCount : 0;

            // 按店別分組統計
            var shopStatistics = summaryResult.Items
                .GroupBy(x => new { x.ShopId, x.ShopName })
                .Select(g => new ShopSalesStatisticsDto
                {
                    ShopId = g.Key.ShopId,
                    ShopName = g.Key.ShopName,
                    OrderCount = g.Sum(x => x.OrderCount),
                    TotalAmount = g.Sum(x => x.TotalAmount),
                    TotalQty = g.Sum(x => x.TotalQty),
                    AvgAmount = g.Sum(x => x.OrderCount) > 0 
                        ? g.Sum(x => x.TotalAmount) / g.Sum(x => x.OrderCount) 
                        : 0
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            // 按客戶分組統計
            var customerStatistics = summaryResult.Items
                .Where(x => !string.IsNullOrEmpty(x.CustomerId))
                .GroupBy(x => new { x.CustomerId, x.CustomerName })
                .Select(g => new CustomerSalesStatisticsDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    OrderCount = g.Sum(x => x.OrderCount),
                    TotalAmount = g.Sum(x => x.TotalAmount),
                    TotalQty = g.Sum(x => x.TotalQty)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(10)
                .ToList();

            return new SalesAnalysisReportDto
            {
                TotalAmount = totalAmount,
                TotalQty = totalQty,
                OrderCount = orderCount,
                AvgAmount = avgAmount,
                AvgQty = avgQty,
                ShopStatistics = shopStatistics,
                CustomerStatistics = customerStatistics,
                DetailData = detailResult.Items.Take(100).ToList() // 限制明細資料數量
            };
        }, "查詢銷售分析報表失敗");
    }
}

/// <summary>
/// 銷售分析報表 DTO
/// </summary>
public class SalesAnalysisReportDto
{
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public int OrderCount { get; set; }
    public decimal AvgAmount { get; set; }
    public decimal AvgQty { get; set; }
    public List<ShopSalesStatisticsDto> ShopStatistics { get; set; } = new();
    public List<CustomerSalesStatisticsDto> CustomerStatistics { get; set; } = new();
    public List<SalesReportDetailDto> DetailData { get; set; } = new();
}

/// <summary>
/// 店別銷售統計 DTO
/// </summary>
public class ShopSalesStatisticsDto
{
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public decimal AvgAmount { get; set; }
}

/// <summary>
/// 客戶銷售統計 DTO
/// </summary>
public class CustomerSalesStatisticsDto
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
}

