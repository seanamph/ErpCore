using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 加班發放 Repository 介面 (SYSL510)
/// </summary>
public interface IOvertimePaymentRepository
{
    /// <summary>
    /// 根據主鍵查詢加班發放
    /// </summary>
    Task<OvertimePayment?> GetByIdAsync(long id);

    /// <summary>
    /// 根據發放單號查詢加班發放
    /// </summary>
    Task<OvertimePayment?> GetByPaymentNoAsync(string paymentNo);

    /// <summary>
    /// 查詢加班發放列表（分頁）
    /// </summary>
    Task<PagedResult<OvertimePayment>> QueryAsync(OvertimePaymentQuery query);

    /// <summary>
    /// 新增加班發放
    /// </summary>
    Task<long> CreateAsync(OvertimePayment overtimePayment);

    /// <summary>
    /// 修改加班發放
    /// </summary>
    Task UpdateAsync(OvertimePayment overtimePayment);

    /// <summary>
    /// 刪除加班發放
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 檢查發放單號是否存在
    /// </summary>
    Task<bool> ExistsByPaymentNoAsync(string paymentNo);
}

/// <summary>
/// 加班發放查詢條件
/// </summary>
public class OvertimePaymentQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PaymentNo { get; set; }
    public string? EmployeeId { get; set; }
    public string? DepartmentId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public string? Status { get; set; }
}

