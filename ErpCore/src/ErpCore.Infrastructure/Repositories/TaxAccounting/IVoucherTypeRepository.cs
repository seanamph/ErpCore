using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 傳票型態 Repository 介面 (SYST121-SYST122)
/// </summary>
public interface IVoucherTypeRepository
{
    Task<VoucherType?> GetByIdAsync(string voucherId);
    Task<PagedResult<VoucherType>> QueryAsync(VoucherTypeQuery query);
    Task<VoucherType> CreateAsync(VoucherType voucherType);
    Task<VoucherType> UpdateAsync(VoucherType voucherType);
    Task DeleteAsync(string voucherId);
    Task<bool> ExistsAsync(string voucherId);
    Task<bool> IsInUseAsync(string voucherId);
}

/// <summary>
/// 傳票型態查詢條件
/// </summary>
public class VoucherTypeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherName { get; set; }
    public string? Status { get; set; }
}

