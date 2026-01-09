using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售單服務接口 (SYSG410-SYSG460 - 銷售資料維護)
/// </summary>
public interface ISalesOrderService
{
    /// <summary>
    /// 查詢銷售單列表
    /// </summary>
    Task<PagedResult<SalesOrderDto>> GetSalesOrdersAsync(SalesOrderQueryDto query);

    /// <summary>
    /// 查詢單筆銷售單（含明細）
    /// </summary>
    Task<SalesOrderDto> GetSalesOrderByIdAsync(long tKey);

    /// <summary>
    /// 根據銷售單號查詢銷售單（含明細）
    /// </summary>
    Task<SalesOrderDto> GetSalesOrderByOrderIdAsync(string orderId);

    /// <summary>
    /// 新增銷售單（含明細）
    /// </summary>
    Task<long> CreateSalesOrderAsync(CreateSalesOrderDto dto);

    /// <summary>
    /// 修改銷售單（含明細）
    /// </summary>
    Task UpdateSalesOrderAsync(UpdateSalesOrderDto dto);

    /// <summary>
    /// 刪除銷售單
    /// </summary>
    Task DeleteSalesOrderAsync(long tKey);
}

