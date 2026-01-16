namespace ErpCore.Application.DTOs.SystemConfig;

/// <summary>
/// 系統作業資料傳輸物件
/// </summary>
public class ConfigProgramDto
{
    public long TKey { get; set; }
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? SystemName { get; set; }
    public string? SubSystemId { get; set; }
    public string? SubSystemName { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 系統作業查詢 DTO
/// </summary>
public class ConfigProgramQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? ProgramId { get; set; }
    public string? ProgramName { get; set; }
    public string? SystemId { get; set; }
    public string? SubSystemId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增系統作業 DTO
/// </summary>
public class CreateConfigProgramDto
{
    public string ProgramId { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? SubSystemId { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改系統作業 DTO
/// </summary>
public class UpdateConfigProgramDto
{
    public string ProgramName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? SubSystemId { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteConfigProgramDto
{
    public List<string> ProgramIds { get; set; } = new();
}
