using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// EIP整合 Repository 介面
/// </summary>
public interface IEipIntegrationRepository
{
    /// <summary>
    /// 根據作業編號和頁面代碼查詢整合設定
    /// </summary>
    Task<EipIntegration?> GetByProgIdAndPageIdAsync(string progId, string pageId);

    /// <summary>
    /// 查詢整合設定列表
    /// </summary>
    Task<PagedResult<EipIntegration>> QueryAsync(EipIntegrationQuery query);

    /// <summary>
    /// 新增整合設定
    /// </summary>
    Task<EipIntegration> CreateAsync(EipIntegration integration);

    /// <summary>
    /// 修改整合設定
    /// </summary>
    Task<EipIntegration> UpdateAsync(EipIntegration integration);

    /// <summary>
    /// 刪除整合設定
    /// </summary>
    Task DeleteAsync(long integrationId);

    /// <summary>
    /// 新增交易記錄
    /// </summary>
    Task<EipTransaction> CreateTransactionAsync(EipTransaction transaction);

    /// <summary>
    /// 查詢交易記錄列表
    /// </summary>
    Task<PagedResult<EipTransaction>> GetTransactionsAsync(string? progId, string? pageId, int pageIndex, int pageSize);
}

/// <summary>
/// EIP整合查詢條件
/// </summary>
public class EipIntegrationQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProgId { get; set; }
    public string? PageId { get; set; }
    public string? Status { get; set; }
}

