namespace ErpCore.Application.DTOs.CommunicationModule;

/// <summary>
/// 系統通訊設定 DTO
/// </summary>
public class SystemCommunicationDto
{
    public long CommunicationId { get; set; }
    public string SystemCode { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string CommunicationType { get; set; } = string.Empty;
    public string? EndpointUrl { get; set; }
    public string Status { get; set; } = "1";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立系統通訊設定 DTO
/// </summary>
public class CreateSystemCommunicationDto
{
    public string SystemCode { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string CommunicationType { get; set; } = string.Empty;
    public string? EndpointUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? ConfigData { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 修改系統通訊設定 DTO
/// </summary>
public class UpdateSystemCommunicationDto
{
    public string SystemName { get; set; } = string.Empty;
    public string CommunicationType { get; set; } = string.Empty;
    public string? EndpointUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? ConfigData { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// 系統通訊設定查詢 DTO
/// </summary>
public class SystemCommunicationQueryDto
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

/// <summary>
/// XCOM系統參數 DTO
/// </summary>
public class XComSystemParamDto
{
    public string ParamCode { get; set; } = string.Empty;
    public string ParamName { get; set; } = string.Empty;
    public string? ParamValue { get; set; }
    public string? ParamType { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
    public string? SystemId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立XCOM系統參數 DTO
/// </summary>
public class CreateXComSystemParamDto
{
    public string ParamCode { get; set; } = string.Empty;
    public string ParamName { get; set; } = string.Empty;
    public string? ParamValue { get; set; }
    public string? ParamType { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
    public string? SystemId { get; set; }
}

/// <summary>
/// 修改XCOM系統參數 DTO
/// </summary>
public class UpdateXComSystemParamDto
{
    public string ParamName { get; set; } = string.Empty;
    public string? ParamValue { get; set; }
    public string? ParamType { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
    public string? SystemId { get; set; }
}

/// <summary>
/// XCOM系統參數查詢 DTO
/// </summary>
public class XComSystemParamQueryDto
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

/// <summary>
/// 錯誤訊息 DTO
/// </summary>
public class ErrorMessageDto
{
    public long TKey { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; }
    public string ErrorMessageText { get; set; } = string.Empty;
    public string? ErrorDetail { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? UserId { get; set; }
    public string? UserIp { get; set; }
    public string? UserAgent { get; set; }
    public string? StackTrace { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立錯誤訊息 DTO
/// </summary>
public class CreateErrorMessageDto
{
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public int? HttpStatusCode { get; set; }
    public string ErrorMessageText { get; set; } = string.Empty;
    public string? ErrorDetail { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? UserId { get; set; }
    public string? UserIp { get; set; }
    public string? UserAgent { get; set; }
    public string? StackTrace { get; set; }
}

/// <summary>
/// 錯誤訊息查詢 DTO
/// </summary>
public class ErrorMessageQueryDto
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

/// <summary>
/// HTTP錯誤頁面 DTO
/// </summary>
public class ErrorPageDto
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string>? Suggestions { get; set; }
}

/// <summary>
/// 警告頁面 DTO
/// </summary>
public class WarningPageDto
{
    public string WarningCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "WARNING"; // CONFIRM, INFO, WARNING, ERROR
    public string ConfirmText { get; set; } = "確定";
    public string CancelText { get; set; } = "取消";
}

/// <summary>
/// 錯誤訊息模板 DTO
/// </summary>
public class ErrorMessageTemplateDto
{
    public long TKey { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string Language { get; set; } = "zh-TW";
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Solution { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

