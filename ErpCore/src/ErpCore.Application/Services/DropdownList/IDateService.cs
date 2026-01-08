namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 日期服務介面
/// </summary>
public interface IDateService
{
    /// <summary>
    /// 取得系統日期格式設定
    /// </summary>
    Task<DateFormatDto> GetDateFormatAsync();

    /// <summary>
    /// 驗證日期格式
    /// </summary>
    Task<DateValidationDto> ValidateDateAsync(string dateString, string? dateFormat = null);

    /// <summary>
    /// 解析日期字串
    /// </summary>
    Task<DateTime?> ParseDateAsync(string dateString, string? dateFormat = null);
}

