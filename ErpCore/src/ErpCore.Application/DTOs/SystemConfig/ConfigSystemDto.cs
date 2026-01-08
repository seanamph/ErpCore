namespace ErpCore.Application.DTOs.SystemConfig;

/// <summary>
/// 主系統項目資料傳輸物件
/// </summary>
public class ConfigSystemDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 主系統項目查詢 DTO
/// </summary>
public class ConfigSystemQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? SystemId { get; set; }
    public string? SystemName { get; set; }
    public string? SystemType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增主系統項目 DTO
/// </summary>
public class CreateConfigSystemDto
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? DbPass { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改主系統項目 DTO
/// </summary>
public class UpdateConfigSystemDto
{
    public string SystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string? SystemType { get; set; }
    public string? ServerIp { get; set; }
    public string? ModuleId { get; set; }
    public string? DbUser { get; set; }
    public string? DbPass { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteConfigSystemDto
{
    public List<string> SystemIds { get; set; } = new();
}

