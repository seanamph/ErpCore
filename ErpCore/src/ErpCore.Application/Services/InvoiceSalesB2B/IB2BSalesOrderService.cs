using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B銷售單服務接口 (SYSG000_B2B - B2B銷售資料維護)
/// </summary>
public interface IB2BSalesOrderService
{
    /// <summary>
    /// 查詢B2B銷售單列表
    /// </summary>
    Task<PagedResult<B2BSalesOrderDto>> GetB2BSalesOrdersAsync(B2BSalesOrderQueryDto query);

    /// <summary>
    /// 查詢單筆B2B銷售單
    /// </summary>
    Task<B2BSalesOrderDto> GetB2BSalesOrderByIdAsync(long tKey);

    /// <summary>
    /// 根據銷售單號查詢B2B銷售單
    /// </summary>
    Task<B2BSalesOrderDto> GetB2BSalesOrderByOrderIdAsync(string orderId);

    /// <summary>
    /// 新增B2B銷售單
    /// </summary>
    Task<long> CreateB2BSalesOrderAsync(CreateB2BSalesOrderDto dto);

    /// <summary>
    /// 修改B2B銷售單
    /// </summary>
    Task UpdateB2BSalesOrderAsync(UpdateB2BSalesOrderDto dto);

    /// <summary>
    /// 刪除B2B銷售單
    /// </summary>
    Task DeleteB2BSalesOrderAsync(long tKey);

    /// <summary>
    /// B2B銷售單傳輸
    /// </summary>
    Task TransferB2BSalesOrderAsync(string orderId);
}

