using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 收款沖帳傳票 Repository 介面 (SYSR310-SYSR450)
/// </summary>
public interface IReceiptVoucherTransferRepository
{
    Task<ReceiptVoucherTransfer?> GetByIdAsync(long tKey);
    Task<IEnumerable<ReceiptVoucherTransfer>> GetAllAsync();
    Task<PagedResult<ReceiptVoucherTransfer>> QueryAsync(ReceiptVoucherTransferQuery query);
    Task<IEnumerable<ReceiptVoucherTransfer>> GetByReceiptNoAsync(string receiptNo);
    Task<IEnumerable<ReceiptVoucherTransfer>> GetByVoucherNoAsync(string voucherNo);
    Task<IEnumerable<ReceiptVoucherTransfer>> GetByTransferStatusAsync(string transferStatus);
    Task<ReceiptVoucherTransfer> CreateAsync(ReceiptVoucherTransfer entity);
    Task<ReceiptVoucherTransfer> UpdateAsync(ReceiptVoucherTransfer entity);
    Task DeleteAsync(long tKey);
    Task<int> BatchUpdateTransferStatusAsync(List<long> tKeys, string transferStatus, string transferUser, DateTime transferDate);
}

/// <summary>
/// 收款沖帳傳票查詢條件
/// </summary>
public class ReceiptVoucherTransferQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "ReceiptDate";
    public string? SortOrder { get; set; } = "DESC";
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
    public string? TransferStatus { get; set; }
    public string? ObjectId { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
}

