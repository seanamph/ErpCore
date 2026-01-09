using ErpCore.Application.DTOs.InvoiceSalesB2B;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSalesB2B;

/// <summary>
/// B2B銷售查詢服務接口 (SYSG000_B2B - B2B銷售查詢作業)
/// </summary>
public interface IB2BSalesOrderQueryService
{
    /// <summary>
    /// 查詢B2B銷售單列表
    /// </summary>
    Task<PagedResult<B2BSalesOrderQueryDto>> QueryAsync(B2BSalesOrderQueryConditionDto query);

    /// <summary>
    /// 查詢B2B銷售單統計
    /// </summary>
    Task<B2BSalesOrderStatisticsDto> GetStatisticsAsync(B2BSalesOrderStatisticsQueryDto query);
}

