using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 廠商 Repository 介面
/// </summary>
public interface IVendorRepository
{
    /// <summary>
    /// 根據廠商編號查詢廠商
    /// </summary>
    Task<Vendor?> GetByIdAsync(string vendorId);

    /// <summary>
    /// 根據統一編號查詢廠商
    /// </summary>
    Task<Vendor?> GetByGuiIdAsync(string guiId);

    /// <summary>
    /// 查詢廠商列表（分頁）
    /// </summary>
    Task<PagedResult<Vendor>> QueryAsync(VendorQuery query);

    /// <summary>
    /// 新增廠商
    /// </summary>
    Task<Vendor> CreateAsync(Vendor vendor);

    /// <summary>
    /// 修改廠商
    /// </summary>
    Task<Vendor> UpdateAsync(Vendor vendor);

    /// <summary>
    /// 刪除廠商
    /// </summary>
    Task DeleteAsync(string vendorId);

    /// <summary>
    /// 檢查統一編號是否存在
    /// </summary>
    Task<bool> ExistsByGuiIdAsync(string guiId);

    /// <summary>
    /// 取得下一個流水號
    /// </summary>
    Task<int> GetNextSequenceAsync(string guiId);
}

/// <summary>
/// 廠商查詢條件
/// </summary>
public class VendorQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VendorId { get; set; }
    public string? GuiId { get; set; }
    public string? VendorName { get; set; }
    public string? Status { get; set; }
    public string? SysId { get; set; }
    public string? OrgId { get; set; }
}

