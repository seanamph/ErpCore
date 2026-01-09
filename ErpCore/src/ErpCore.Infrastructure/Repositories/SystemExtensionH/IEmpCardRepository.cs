using ErpCore.Domain.Entities.SystemExtensionH;

namespace ErpCore.Infrastructure.Repositories.SystemExtensionH;

/// <summary>
/// 員工感應卡 Repository 介面 (SYSPH00 - 系統擴展PH)
/// </summary>
public interface IEmpCardRepository
{
    Task<EmpCard?> GetByIdAsync(long tKey);
    Task<EmpCard?> GetByCardNoAsync(string cardNo);
    Task<IEnumerable<EmpCard>> QueryAsync(EmpCardQuery query);
    Task<int> GetCountAsync(EmpCardQuery query);
    Task<long> CreateAsync(EmpCard entity);
    Task UpdateAsync(EmpCard entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsByCardNoAsync(string cardNo);
    Task<int> CreateBatchAsync(IEnumerable<EmpCard> entities);
}

/// <summary>
/// 員工感應卡查詢條件
/// </summary>
public class EmpCardQuery
{
    public string? CardNo { get; set; }
    public string? EmpId { get; set; }
    public string? CardStatus { get; set; }
    public DateTime? BeginDateFrom { get; set; }
    public DateTime? BeginDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

