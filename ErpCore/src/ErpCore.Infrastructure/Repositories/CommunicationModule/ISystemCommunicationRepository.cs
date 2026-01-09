using ErpCore.Domain.Entities.CommunicationModule;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// 系統通訊設定 Repository 介面
/// </summary>
public interface ISystemCommunicationRepository
{
    Task<SystemCommunication?> GetByIdAsync(long communicationId);
    Task<SystemCommunication?> GetBySystemCodeAsync(string systemCode);
    Task<IEnumerable<SystemCommunication>> QueryAsync(SystemCommunicationQuery query);
    Task<int> GetCountAsync(SystemCommunicationQuery query);
    Task<long> CreateAsync(SystemCommunication entity);
    Task UpdateAsync(SystemCommunication entity);
    Task DeleteAsync(long communicationId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class SystemCommunicationQuery
{
    public string? SystemCode { get; set; }
    public string? SystemName { get; set; }
    public string? CommunicationType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
}

