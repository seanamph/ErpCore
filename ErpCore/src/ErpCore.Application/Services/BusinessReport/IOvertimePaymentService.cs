using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 加班發放服務介面 (SYSL510)
/// </summary>
public interface IOvertimePaymentService
{
    /// <summary>
    /// 查詢加班發放列表
    /// </summary>
    Task<PagedResult<OvertimePaymentDto>> GetOvertimePaymentsAsync(OvertimePaymentQueryDto query);

    /// <summary>
    /// 根據發放單號查詢單筆加班發放
    /// </summary>
    Task<OvertimePaymentDto?> GetOvertimePaymentByPaymentNoAsync(string paymentNo);

    /// <summary>
    /// 新增加班發放
    /// </summary>
    Task<string> CreateOvertimePaymentAsync(CreateOvertimePaymentDto dto);

    /// <summary>
    /// 修改加班發放
    /// </summary>
    Task UpdateOvertimePaymentAsync(string paymentNo, UpdateOvertimePaymentDto dto);

    /// <summary>
    /// 刪除加班發放
    /// </summary>
    Task DeleteOvertimePaymentAsync(string paymentNo);

    /// <summary>
    /// 審核加班發放
    /// </summary>
    Task ApproveOvertimePaymentAsync(string paymentNo, ApproveOvertimePaymentDto dto);
}

