using ErpCore.Domain.Entities.SapIntegration;

namespace ErpCore.Infrastructure.Repositories.SapIntegration;

/// <summary>
/// SAP整合資料 Repository 介面 (TransSAP系列)
/// </summary>
public interface ITransSapRepository
{
    Task<TransSap?> GetByIdAsync(long tKey);
    Task<TransSap?> GetByTransIdAsync(string transId);
    Task<IEnumerable<TransSap>> QueryAsync(TransSapQuery query);
    Task<int> GetCountAsync(TransSapQuery query);
    Task<long> CreateAsync(TransSap entity);
    Task UpdateAsync(TransSap entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// SAP整合資料查詢條件
/// </summary>
public class TransSapQuery
{
    public string? TransId { get; set; }
    public string? TransType { get; set; }
    public string? SapSystemCode { get; set; }
    public string? Status { get; set; }
    public DateTime? TransDateFrom { get; set; }
    public DateTime? TransDateTo { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

