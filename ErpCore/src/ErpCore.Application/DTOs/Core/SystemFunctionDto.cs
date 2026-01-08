namespace ErpCore.Application.DTOs.Core;

/// <summary>
/// 系統識別設定 DTO (Identify)
/// </summary>
public class SystemIdentityDto
{
    public long IdentityId { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string ProjectTitle { get; set; } = string.Empty;
    public string? CompanyTitle { get; set; }
    public string? EipUrl { get; set; }
    public string? EipEmbedded { get; set; } = "N";
    public string? ShowTransEffect { get; set; } = "N";
    public string? InitShowTransEffect { get; set; } = "N";
    public bool NoResizeFrame { get; set; } = true;
    public string? DebugUser { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 更新系統識別設定 DTO
/// </summary>
public class UpdateSystemIdentityDto
{
    public string? ProjectTitle { get; set; }
    public string? CompanyTitle { get; set; }
    public string? EipUrl { get; set; }
    public string? EipEmbedded { get; set; }
    public string? ShowTransEffect { get; set; }
    public string? InitShowTransEffect { get; set; }
    public bool? NoResizeFrame { get; set; }
    public string? DebugUser { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 選單設定 DTO
/// </summary>
public class MenuConfigDto
{
    public string SelectMessage { get; set; } = "請選擇功能";
    public string SelectToolbar { get; set; } = "工具列設定";
    public string ShowLeftButton { get; set; } = "Y";
}

/// <summary>
/// 系統初始化 DTO
/// </summary>
public class SystemInitDto
{
    public string SystemId { get; set; } = string.Empty;
    public string ProjectTitle { get; set; } = string.Empty;
    public string? CompanyTitle { get; set; }
    public string? EipUrl { get; set; }
    public MenuConfigDto? MenuConfig { get; set; }
    public Dictionary<string, object>? ToolbarConfig { get; set; }
    public Dictionary<string, object>? MessageConfig { get; set; }
}

/// <summary>
/// 硬體資訊 DTO (MakeRegFile)
/// </summary>
public class HardwareInfoDto
{
    public string CpuNumber { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
}

/// <summary>
/// 生成註冊檔案請求 DTO
/// </summary>
public class GenerateRegistrationDto
{
    public string CompanyId { get; set; } = string.Empty;
    public string CpuNumber { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string? UseDownGo { get; set; }
    public string? Ticket { get; set; }
}

/// <summary>
/// 註冊結果 DTO
/// </summary>
public class RegistrationResultDto
{
    public long RegistrationId { get; set; }
    public string RegistrationKey { get; set; } = string.Empty;
    public string? DownloadUrl { get; set; }
}

/// <summary>
/// 驗證註冊檔案請求 DTO
/// </summary>
public class VerifyRegistrationDto
{
    public string RegistrationKey { get; set; } = string.Empty;
    public string CpuNumber { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
}

/// <summary>
/// 驗證結果 DTO
/// </summary>
public class VerificationResultDto
{
    public bool IsValid { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? DaysRemaining { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 系統註冊記錄 DTO
/// </summary>
public class SystemRegistrationDto
{
    public long RegistrationId { get; set; }
    public string CompanyId { get; set; } = string.Empty;
    public string CpuNumber { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 系統註冊查詢 DTO
/// </summary>
public class SystemRegistrationQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? CompanyId { get; set; }
    public string? CpuNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 網頁註冊請求 DTO
/// </summary>
public class WebRegisterDto
{
    public string TKey { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EDate { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string CpuNumber { get; set; } = string.Empty;
    public string ComputerName { get; set; } = string.Empty;
    public string? RslCoding { get; set; }
}

/// <summary>
/// 關於頁面資訊 DTO (about)
/// </summary>
public class AboutInfoDto
{
    public string SystemName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Copyright { get; set; }
    public string? Description { get; set; }
    public DateTime? BuildDate { get; set; }
}

