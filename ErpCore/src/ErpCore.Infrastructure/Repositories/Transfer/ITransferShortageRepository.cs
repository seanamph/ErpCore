using System.Data;
using ErpCore.Domain.Entities.Transfer;

namespace ErpCore.Infrastructure.Repositories.Transfer;

/// <summary>
/// 調撥短溢單 Repository 介面 (SYSW384)
/// </summary>
public interface ITransferShortageRepository
{
    Task<TransferShortage?> GetByIdAsync(string shortageId);
    Task<TransferShortageDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<TransferShortageDetail>> GetDetailsByShortageIdAsync(string shortageId);
    Task<IEnumerable<TransferShortage>> QueryAsync(TransferShortageQuery query);
    Task<int> GetCountAsync(TransferShortageQuery query);
    Task<string> CreateAsync(TransferShortage entity, List<TransferShortageDetail> details);
    Task UpdateAsync(TransferShortage entity, List<TransferShortageDetail> details);
    Task DeleteAsync(string shortageId);
    Task UpdateStatusAsync(string shortageId, string status, global::System.Data.IDbTransaction? transaction = null);
    Task<string> GenerateShortageIdAsync();
}

/// <summary>
/// 查詢條件
/// </summary>
public class TransferShortageQuery
{
    public string? ShortageId { get; set; }
    public string? TransferId { get; set; }
    public string? ReceiptId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public string? ProcessType { get; set; }
    public DateTime? ShortageDateFrom { get; set; }
    public DateTime? ShortageDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
