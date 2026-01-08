using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購其他功能服務介面 (SYSP510-SYSP530)
/// </summary>
public interface IProcurementOtherService
{
    /// <summary>
    /// 查詢採購其他功能列表
    /// </summary>
    Task<PagedResult<ProcurementOtherDto>> GetProcurementOthersAsync(ProcurementOtherQueryDto query);

    /// <summary>
    /// 查詢單筆採購其他功能（根據主鍵）
    /// </summary>
    Task<ProcurementOtherDto> GetProcurementOtherByTKeyAsync(long tKey);

    /// <summary>
    /// 查詢單筆採購其他功能（根據功能代碼）
    /// </summary>
    Task<ProcurementOtherDto> GetProcurementOtherByFunctionIdAsync(string functionId);

    /// <summary>
    /// 新增採購其他功能
    /// </summary>
    Task<long> CreateProcurementOtherAsync(CreateProcurementOtherDto dto);

    /// <summary>
    /// 修改採購其他功能
    /// </summary>
    Task UpdateProcurementOtherAsync(long tKey, UpdateProcurementOtherDto dto);

    /// <summary>
    /// 刪除採購其他功能
    /// </summary>
    Task DeleteProcurementOtherAsync(long tKey);

    /// <summary>
    /// 檢查採購其他功能是否存在
    /// </summary>
    Task<bool> ExistsAsync(string functionId);
}

