using ErpCore.Application.DTOs.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 廠商服務介面
/// </summary>
public interface IVendorService
{
    /// <summary>
    /// 查詢廠商列表
    /// </summary>
    Task<PagedResult<VendorDto>> GetVendorsAsync(VendorQueryDto query);

    /// <summary>
    /// 查詢單筆廠商
    /// </summary>
    Task<VendorDto> GetVendorByIdAsync(string vendorId);

    /// <summary>
    /// 檢查統一編號是否存在
    /// </summary>
    Task<CheckGuiIdResultDto> CheckGuiIdAsync(string guiId);

    /// <summary>
    /// 新增廠商
    /// </summary>
    Task<string> CreateVendorAsync(CreateVendorDto dto);

    /// <summary>
    /// 修改廠商
    /// </summary>
    Task UpdateVendorAsync(string vendorId, UpdateVendorDto dto);

    /// <summary>
    /// 刪除廠商
    /// </summary>
    Task DeleteVendorAsync(string vendorId);

    /// <summary>
    /// 批次刪除廠商
    /// </summary>
    Task DeleteVendorsBatchAsync(BatchDeleteVendorDto dto);

    /// <summary>
    /// 產生廠商編號
    /// </summary>
    Task<string> GenerateVendorIdAsync(string guiId);
}
