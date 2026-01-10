using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// POS終端 Repository 介面 (SYS6610-SYS6999 - POS資料維護)
/// </summary>
public interface IPosTerminalRepository
{
    Task<PosTerminal?> GetByIdAsync(string posTerminalId);
    Task<PagedResult<PosTerminal>> QueryAsync(PosTerminalQuery query);
    Task<int> GetCountAsync(PosTerminalQuery query);
    Task<PosTerminal> CreateAsync(PosTerminal posTerminal);
    Task<PosTerminal> UpdateAsync(PosTerminal posTerminal);
    Task DeleteAsync(string posTerminalId);
    Task<bool> ExistsAsync(string posTerminalId);
    Task UpdateLastSyncDateAsync(string posTerminalId, DateTime syncDate);
    Task<int> GetTransactionCountAsync(string posTerminalId, DateTime? startDate = null, DateTime? endDate = null);
}

/// <summary>
/// POS終端查詢條件
/// </summary>
public class PosTerminalQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PosTerminalId { get; set; }
    public string? PosSystemId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
}

