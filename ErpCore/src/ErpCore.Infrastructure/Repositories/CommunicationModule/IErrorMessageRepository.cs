using ErpCore.Domain.Entities.CommunicationModule;
using ErpCore.Infrastructure.Repositories;

namespace ErpCore.Infrastructure.Repositories.CommunicationModule;

/// <summary>
/// 錯誤訊息 Repository 接口
/// </summary>
public interface IErrorMessageRepository
{
    /// <summary>
    /// 根據主鍵查詢錯誤訊息
    /// </summary>
    Task<ErrorMessage?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢錯誤訊息列表
    /// </summary>
    Task<(IEnumerable<ErrorMessage> Items, int TotalCount)> QueryAsync(ErrorMessageQuery query);

    /// <summary>
    /// 新增錯誤訊息
    /// </summary>
    Task<long> CreateAsync(ErrorMessage entity);

    /// <summary>
    /// 根據錯誤代碼查詢錯誤訊息模板
    /// </summary>
    Task<ErrorMessageTemplate?> GetTemplateByErrorCodeAsync(string errorCode, string language = "zh-TW");

    /// <summary>
    /// 查詢錯誤訊息模板列表
    /// </summary>
    Task<IEnumerable<ErrorMessageTemplate>> GetTemplatesAsync(string? errorCode = null, string? language = null);
}

/// <summary>
/// 錯誤訊息查詢條件
/// </summary>
public class ErrorMessageQuery
{
    public string? ErrorCode { get; set; }
    public string? ErrorType { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "DESC";
}

