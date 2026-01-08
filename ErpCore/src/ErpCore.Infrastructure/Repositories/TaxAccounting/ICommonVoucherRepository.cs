using ErpCore.Domain.Entities.TaxAccounting;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.TaxAccounting;

/// <summary>
/// 常用傳票 Repository 介面 (SYST123)
/// </summary>
public interface ICommonVoucherRepository
{
    Task<CommonVoucher?> GetByTKeyAsync(long tKey);
    Task<CommonVoucher?> GetByIdAsync(string voucherId);
    Task<PagedResult<CommonVoucher>> QueryAsync(CommonVoucherQuery query);
    Task<CommonVoucher> CreateAsync(CommonVoucher commonVoucher);
    Task<CommonVoucher> UpdateAsync(CommonVoucher commonVoucher);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string voucherId);
    Task<List<CommonVoucherDetail>> GetDetailsAsync(long voucherTKey);
    Task CreateDetailAsync(CommonVoucherDetail detail);
    Task UpdateDetailAsync(CommonVoucherDetail detail);
    Task DeleteDetailsAsync(long voucherTKey);
}

/// <summary>
/// 常用傳票查詢條件
/// </summary>
public class CommonVoucherQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? VoucherId { get; set; }
    public string? VoucherName { get; set; }
    public string? VoucherType { get; set; }
    public string? SiteId { get; set; }
}

