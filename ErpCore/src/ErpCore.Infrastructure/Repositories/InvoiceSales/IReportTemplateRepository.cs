using ErpCore.Domain.Entities.InvoiceSales;

namespace ErpCore.Infrastructure.Repositories.InvoiceSales;

/// <summary>
/// 報表模板 Repository 接口 (SYSG710-SYSG7I0 - 報表列印作業)
/// </summary>
public interface IReportTemplateRepository
{
    /// <summary>
    /// 根據模板編號查詢
    /// </summary>
    Task<ReportTemplate?> GetByIdAsync(string templateId);

    /// <summary>
    /// 根據報表類型查詢模板列表
    /// </summary>
    Task<List<ReportTemplate>> GetByReportTypeAsync(string reportType, string? status = null);

    /// <summary>
    /// 新增模板
    /// </summary>
    Task<int> CreateAsync(ReportTemplate template);

    /// <summary>
    /// 修改模板
    /// </summary>
    Task<int> UpdateAsync(ReportTemplate template);

    /// <summary>
    /// 刪除模板
    /// </summary>
    Task<int> DeleteAsync(string templateId);
}

/// <summary>
/// 報表列印記錄 Repository 接口
/// </summary>
public interface IReportPrintLogRepository
{
    /// <summary>
    /// 根據主鍵查詢
    /// </summary>
    Task<ReportPrintLog?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據報表編號查詢
    /// </summary>
    Task<ReportPrintLog?> GetByReportIdAsync(string reportId);

    /// <summary>
    /// 新增記錄
    /// </summary>
    Task<long> CreateAsync(ReportPrintLog log);

    /// <summary>
    /// 修改記錄
    /// </summary>
    Task<int> UpdateAsync(ReportPrintLog log);
}

