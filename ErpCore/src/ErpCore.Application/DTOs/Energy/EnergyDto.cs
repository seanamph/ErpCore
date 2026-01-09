namespace ErpCore.Application.DTOs.Energy;

/// <summary>
/// 能源基礎資料 DTO (SYSO100-SYSO130 - 能源基礎功能)
/// </summary>
public class EnergyBaseDto
{
    public long TKey { get; set; }
    public string EnergyId { get; set; } = string.Empty;
    public string EnergyName { get; set; } = string.Empty;
    public string? EnergyType { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 能源基礎查詢 DTO
/// </summary>
public class EnergyBaseQueryDto
{
    public string? EnergyId { get; set; }
    public string? EnergyName { get; set; }
    public string? EnergyType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立能源基礎資料 DTO
/// </summary>
public class CreateEnergyBaseDto
{
    public string EnergyId { get; set; } = string.Empty;
    public string EnergyName { get; set; } = string.Empty;
    public string? EnergyType { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改能源基礎資料 DTO
/// </summary>
public class UpdateEnergyBaseDto
{
    public string EnergyName { get; set; } = string.Empty;
    public string? EnergyType { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 能源處理資料 DTO (SYSO310 - 能源處理功能)
/// </summary>
public class EnergyProcessDto
{
    public long TKey { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public string EnergyId { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string? ProcessType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Cost { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 能源處理查詢 DTO
/// </summary>
public class EnergyProcessQueryDto
{
    public string? ProcessId { get; set; }
    public string? EnergyId { get; set; }
    public DateTime? ProcessDateFrom { get; set; }
    public DateTime? ProcessDateTo { get; set; }
    public string? ProcessType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立能源處理資料 DTO
/// </summary>
public class CreateEnergyProcessDto
{
    public string ProcessId { get; set; } = string.Empty;
    public string EnergyId { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string? ProcessType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Cost { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改能源處理資料 DTO
/// </summary>
public class UpdateEnergyProcessDto
{
    public string EnergyId { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string? ProcessType { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Cost { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 能源擴展資料 DTO (SYSOU10-SYSOU33 - 能源擴展功能)
/// </summary>
public class EnergyExtensionDto
{
    public long TKey { get; set; }
    public string ExtensionId { get; set; } = string.Empty;
    public string EnergyId { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 能源擴展查詢 DTO
/// </summary>
public class EnergyExtensionQueryDto
{
    public string? ExtensionId { get; set; }
    public string? EnergyId { get; set; }
    public string? ExtensionType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立能源擴展資料 DTO
/// </summary>
public class CreateEnergyExtensionDto
{
    public string ExtensionId { get; set; } = string.Empty;
    public string EnergyId { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改能源擴展資料 DTO
/// </summary>
public class UpdateEnergyExtensionDto
{
    public string EnergyId { get; set; } = string.Empty;
    public string? ExtensionType { get; set; }
    public string? ExtensionValue { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

