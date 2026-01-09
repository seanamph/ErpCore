using ErpCore.Application.DTOs.InvoiceSales;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InvoiceSales;

/// <summary>
/// 銷售查詢服務接口 (SYSG510-SYSG5D0 - 銷售查詢作業)
/// </summary>
public interface ISalesOrderQueryService
{
    /// <summary>
    /// 查詢銷售單列表
    /// </summary>
    Task<PagedResult<SalesOrderQueryDto>> QueryAsync(SalesOrderQueryConditionDto query);

    /// <summary>
    /// 查詢銷售單統計資料
    /// </summary>
    Task<SalesOrderStatisticsDto> GetStatisticsAsync(SalesOrderStatisticsQueryDto query);
}

