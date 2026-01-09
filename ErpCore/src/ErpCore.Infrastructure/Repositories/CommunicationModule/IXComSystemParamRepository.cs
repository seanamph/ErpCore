using ErpCore.Domain.Entities.CommunicationModule;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// XCOM系統參數 Repository 介面
/// </summary>
public interface IXComSystemParamRepository
{
    Task<XComSystemParam?> GetByIdAsync(string paramCode);
    Task<IEnumerable<XComSystemParam>> QueryAsync(XComSystemParamQuery query);
    Task<int> GetCountAsync(XComSystemParamQuery query);
    Task CreateAsync(XComSystemParam entity);
    Task UpdateAsync(XComSystemParam entity);
    Task DeleteAsync(string paramCode);
    Task<bool> ExistsAsync(string paramCode);
}

/// <summary>
/// 查詢條件
/// </summary>
public class XComSystemParamQuery
{
    public string? ParamCode { get; set; }
    public string? ParamName { get; set; }
    public string? ParamType { get; set; }
    public string? Status { get; set; }
    public string? SystemId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; } = "ASC";
}

