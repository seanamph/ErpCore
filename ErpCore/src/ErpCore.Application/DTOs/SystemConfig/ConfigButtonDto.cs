namespace ErpCore.Application.DTOs.SystemConfig;

/// <summary>
/// 系統功能按鈕資料傳輸物件
/// </summary>
public class ConfigButtonDto
{
    public string ButtonId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string? ProgramName { get; set; }
    public string ButtonName { get; set; } = string.Empty;
    public string? ButtonType { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 系統功能按鈕查詢 DTO
/// </summary>
public class ConfigButtonQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? ButtonId { get; set; }
    public string? ButtonName { get; set; }
    public string? ProgramId { get; set; }
    public string? ButtonType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增系統功能按鈕 DTO
/// </summary>
public class CreateConfigButtonDto
{
    public string ButtonId { get; set; } = string.Empty;
    public string ProgramId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public string? ButtonType { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改系統功能按鈕 DTO
/// </summary>
public class UpdateConfigButtonDto
{
    public string ProgramId { get; set; } = string.Empty;
    public string ButtonName { get; set; } = string.Empty;
    public string? ButtonType { get; set; }
    public int? SeqNo { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteConfigButtonDto
{
    public List<string> ButtonIds { get; set; } = new();
}

