using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 業務報表列印明細 Repository 介面 (SYSL160)
/// </summary>
public interface IBusinessReportPrintDetailRepository
{
    /// <summary>
    /// 查詢業務報表列印明細列表
    /// </summary>
    Task<PagedResult<BusinessReportPrintDetail>> QueryAsync(BusinessReportPrintDetailQuery query);

    /// <summary>
    /// 根據主鍵查詢單筆資料
    /// </summary>
    Task<BusinessReportPrintDetail?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據 PrintId 查詢明細列表
    /// </summary>
    Task<List<BusinessReportPrintDetail>> GetByPrintIdAsync(long printId);

    /// <summary>
    /// 新增業務報表列印明細
    /// </summary>
    Task<long> CreateAsync(BusinessReportPrintDetail entity);

    /// <summary>
    /// 修改業務報表列印明細
    /// </summary>
    Task<bool> UpdateAsync(BusinessReportPrintDetail entity);

    /// <summary>
    /// 刪除業務報表列印明細
    /// </summary>
    Task<bool> DeleteAsync(long tKey);

    /// <summary>
    /// 批次處理業務報表列印明細
    /// </summary>
    Task<BatchProcessResult> BatchProcessAsync(List<BusinessReportPrintDetail> createItems, List<BusinessReportPrintDetail> updateItems, List<long> deleteTKeys);
}

/// <summary>
/// 業務報表列印明細查詢條件
/// </summary>
public class BusinessReportPrintDetailQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public long? PrintId { get; set; }
    public string? LeaveId { get; set; }
    public string? ActEvent { get; set; }
}

/// <summary>
/// 批次處理結果
/// </summary>
public class BatchProcessResult
{
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public int FailCount { get; set; }
}

