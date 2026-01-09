using ErpCore.Application.DTOs.CommunicationModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CommunicationModule;

/// <summary>
/// 錯誤訊息 Service 接口
/// </summary>
public interface IErrorMessageService
{
    /// <summary>
    /// 查詢錯誤訊息列表
    /// </summary>
    Task<PagedResult<ErrorMessageDto>> GetErrorMessagesAsync(ErrorMessageQueryDto query);

    /// <summary>
    /// 查詢單筆錯誤訊息
    /// </summary>
    Task<ErrorMessageDto?> GetErrorMessageByIdAsync(long tKey);

    /// <summary>
    /// 記錄錯誤訊息
    /// </summary>
    Task<long> CreateErrorMessageAsync(CreateErrorMessageDto dto);

    /// <summary>
    /// 取得HTTP錯誤頁面資訊
    /// </summary>
    Task<ErrorPageDto> GetErrorPageAsync(int statusCode, string language = "zh-TW");

    /// <summary>
    /// 取得警告頁面資訊
    /// </summary>
    Task<WarningPageDto> GetWarningPageAsync(string warningCode, string language = "zh-TW");

    /// <summary>
    /// 查詢錯誤訊息模板列表
    /// </summary>
    Task<IEnumerable<ErrorMessageTemplateDto>> GetTemplatesAsync(string? errorCode = null, string? language = null);
}

