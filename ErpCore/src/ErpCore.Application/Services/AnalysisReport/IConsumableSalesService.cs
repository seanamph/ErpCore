using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.AnalysisReport;

/// <summary>
/// 耗材出售單服務介面 (SYSA297)
/// </summary>
public interface IConsumableSalesService
{
    /// <summary>
    /// 查詢耗材出售單列表
    /// </summary>
    Task<PagedResult<ConsumableSalesDto>> GetSalesAsync(ConsumableSalesQueryDto query);

    /// <summary>
    /// 根據交易單號查詢耗材出售單
    /// </summary>
    Task<ConsumableSalesDetailDto> GetSalesDetailAsync(string txnNo);

    /// <summary>
    /// 新增耗材出售單
    /// </summary>
    Task<string> CreateSalesAsync(CreateConsumableSalesDto dto, string userId);

    /// <summary>
    /// 更新耗材出售單
    /// </summary>
    Task UpdateSalesAsync(string txnNo, UpdateConsumableSalesDto dto, string userId);

    /// <summary>
    /// 刪除耗材出售單
    /// </summary>
    Task DeleteSalesAsync(string txnNo, string userId);

    /// <summary>
    /// 審核耗材出售單
    /// </summary>
    Task ApproveSalesAsync(string txnNo, ApproveSalesDto dto, string userId);

    /// <summary>
    /// 取消耗材出售單
    /// </summary>
    Task CancelSalesAsync(string txnNo, string userId);

    /// <summary>
    /// 生成交易單號
    /// </summary>
    Task<string> GenerateTxnNoAsync(string siteId, DateTime date);
}
