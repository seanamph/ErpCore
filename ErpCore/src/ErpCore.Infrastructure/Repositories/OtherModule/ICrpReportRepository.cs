using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// CRP報表 Repository 介面
/// </summary>
public interface ICrpReportRepository
{
    /// <summary>
    /// 根據報表代碼查詢報表設定
    /// </summary>
    Task<CrystalReport?> GetByReportCodeAsync(string reportCode);

    /// <summary>
    /// 查詢報表設定列表
    /// </summary>
    Task<List<CrystalReport>> GetAllAsync();

    /// <summary>
    /// 新增報表設定
    /// </summary>
    Task<CrystalReport> CreateAsync(CrystalReport report);

    /// <summary>
    /// 修改報表設定
    /// </summary>
    Task<CrystalReport> UpdateAsync(CrystalReport report);

    /// <summary>
    /// 刪除報表設定
    /// </summary>
    Task DeleteAsync(long reportId);

    /// <summary>
    /// 新增操作記錄
    /// </summary>
    Task<CrystalReportLog> CreateLogAsync(CrystalReportLog log);

    /// <summary>
    /// 查詢操作記錄列表
    /// </summary>
    Task<PagedResult<CrystalReportLog>> GetLogsAsync(string? reportCode, int pageIndex, int pageSize);
}

