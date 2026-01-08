using ErpCore.Application.DTOs.Sales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Sales;

/// <summary>
/// 銷售單服務介面 (SYSD110-SYSD140)
/// </summary>
public interface ISalesOrderService
{
    Task<PagedResult<SalesOrderDto>> GetSalesOrdersAsync(SalesOrderQueryDto query);
    Task<SalesOrderDto> GetSalesOrderByIdAsync(string orderId);
    Task<string> CreateSalesOrderAsync(CreateSalesOrderDto dto);
    Task UpdateSalesOrderAsync(string orderId, UpdateSalesOrderDto dto);
    Task DeleteSalesOrderAsync(string orderId);
    Task ApproveSalesOrderAsync(string orderId, ApproveSalesOrderDto dto);
    Task ShipSalesOrderAsync(string orderId, ShipSalesOrderDto dto);
    Task CancelSalesOrderAsync(string orderId, CancelSalesOrderDto dto);
}

