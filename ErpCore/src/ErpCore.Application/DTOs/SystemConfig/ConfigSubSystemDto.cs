namespace ErpCore.Application.DTOs.SystemConfig;

/// <summary>
/// 子系統項目資料傳輸物件
/// </summary>
public class ConfigSubSystemDto
{
    public long TKey { get; set; }
    public string SubSystemId { get; set; } = string.Empty;
    public string SubSystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? SystemName { get; set; }
    public string? ParentSubSystemId { get; set; }
    public string? ParentSubSystemName { get; set; }
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 子系統項目查詢 DTO
/// </summary>
public class ConfigSubSystemQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "ASC";
    public string? SubSystemId { get; set; }
    public string? SubSystemName { get; set; }
    public string? SystemId { get; set; }
    public string? ParentSubSystemId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 新增子系統項目 DTO
/// </summary>
public class CreateConfigSubSystemDto
{
    public string SubSystemId { get; set; } = string.Empty;
    public string SubSystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? ParentSubSystemId { get; set; } = "0";
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改子系統項目 DTO
/// </summary>
public class UpdateConfigSubSystemDto
{
    public string SubSystemName { get; set; } = string.Empty;
    public int? SeqNo { get; set; }
    public string SystemId { get; set; } = string.Empty;
    public string? ParentSubSystemId { get; set; } = "0";
    public string Status { get; set; } = "A";
    public string? Notes { get; set; }
}

/// <summary>
/// 批次刪除 DTO
/// </summary>
public class BatchDeleteConfigSubSystemDto
{
    public List<string> SubSystemIds { get; set; } = new();
}

