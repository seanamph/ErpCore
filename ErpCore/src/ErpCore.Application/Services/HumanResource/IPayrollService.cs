using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.HumanResource;

/// <summary>
/// 薪資服務介面 (SYSH210)
/// </summary>
public interface IPayrollService
{
    /// <summary>
    /// 查詢薪資列表
    /// </summary>
    Task<PagedResult<PayrollDto>> GetPayrollsAsync(PayrollQueryDto query);

    /// <summary>
    /// 根據薪資編號查詢薪資
    /// </summary>
    Task<PayrollDto> GetPayrollByIdAsync(string payrollId);

    /// <summary>
    /// 新增薪資
    /// </summary>
    Task<string> CreatePayrollAsync(CreatePayrollDto dto);

    /// <summary>
    /// 修改薪資
    /// </summary>
    Task UpdatePayrollAsync(string payrollId, UpdatePayrollDto dto);

    /// <summary>
    /// 刪除薪資
    /// </summary>
    Task DeletePayrollAsync(string payrollId);

    /// <summary>
    /// 確認薪資（狀態變更為已確認）
    /// </summary>
    Task ConfirmPayrollAsync(string payrollId);

    /// <summary>
    /// 計算薪資（不儲存）
    /// </summary>
    Task<PayrollCalculationResultDto> CalculatePayrollAsync(CalculatePayrollDto dto);
}

