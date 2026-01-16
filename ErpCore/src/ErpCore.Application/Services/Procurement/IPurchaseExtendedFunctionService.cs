using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購擴展功能服務介面 (SYSP610)
/// </summary>
public interface IPurchaseExtendedFunctionService
{
    /// <summary>
    /// 查詢採購擴展功能列表
    /// </summary>
    Task<PagedResult<PurchaseExtendedFunctionDto>> GetPurchaseExtendedFunctionsAsync(PurchaseExtendedFunctionQueryDto query);

    /// <summary>
    /// 查詢單筆採購擴展功能（根據主鍵）
    /// </summary>
    Task<PurchaseExtendedFunctionDto> GetPurchaseExtendedFunctionByTKeyAsync(long tKey);

    /// <summary>
    /// 查詢單筆採購擴展功能（根據功能代碼）
    /// </summary>
    Task<PurchaseExtendedFunctionDto> GetPurchaseExtendedFunctionByExtFunctionIdAsync(string extFunctionId);

    /// <summary>
    /// 新增採購擴展功能
    /// </summary>
    Task<long> CreatePurchaseExtendedFunctionAsync(CreatePurchaseExtendedFunctionDto dto);

    /// <summary>
    /// 修改採購擴展功能
    /// </summary>
    Task UpdatePurchaseExtendedFunctionAsync(long tKey, UpdatePurchaseExtendedFunctionDto dto);

    /// <summary>
    /// 刪除採購擴展功能
    /// </summary>
    Task DeletePurchaseExtendedFunctionAsync(long tKey);

    /// <summary>
    /// 檢查採購擴展功能是否存在
    /// </summary>
    Task<bool> ExistsAsync(string extFunctionId);
}
