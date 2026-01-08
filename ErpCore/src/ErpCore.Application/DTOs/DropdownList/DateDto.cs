namespace ErpCore.Application.DTOs.DropdownList;

/// <summary>
/// 日期格式 DTO
/// </summary>
public class DateFormatDto
{
    public string DateFormat { get; set; } = "yyyy/MM/dd";
    public string TimeFormat { get; set; } = "HH:mm:ss";
    public string DateTimeFormat { get; set; } = "yyyy/MM/dd HH:mm:ss";
}

/// <summary>
/// 日期驗證 DTO
/// </summary>
public class DateValidationDto
{
    public bool IsValid { get; set; }
    public DateTime? ParsedDate { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 日期驗證請求 DTO
/// </summary>
public class DateValidationRequestDto
{
    public string DateString { get; set; } = string.Empty;
    public string? DateFormat { get; set; }
}

